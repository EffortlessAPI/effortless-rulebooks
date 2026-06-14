// ---------------------------------------------------------------------------
// Talisman's Special Solutions — Workflow app backend (Express, TypeScript)
//
// Responsibilities, and ONLY these:
//   1. Own the database: app/backend/db.json (raw, editable rows).
//   2. Drive a CHOSEN execution substrate for every read that needs a derived
//      field — either the OWL/SHACL reasoner (reasoner/reason.py) or the
//      Postgres views — via the DAL router (dal/index.ts). The frontend's
//      login page picks which; api.ts attaches it as the X-ERB-Backend header.
//   3. Serve the API the Vite frontend hits, and (prod) the built SPA.
//
// What this file deliberately does NOT do: compute a single workflow field.
// No closure walk, no COUNTIFS, no "isStale", no formula parsing. Those are the
// substrate's job — and BOTH substrates are generated from the same rulebook,
// so this file is engine-agnostic. (Project doctrine: "the view IS the
// contract" — the reasoned graph / the vw_<entity> view is the view; we read
// it, we don't re-derive it.)
// ---------------------------------------------------------------------------
import express, { type Request, type Response, type RequestHandler } from "express";
import { existsSync } from "node:fs";
import { readFile, writeFile, mkdtemp, rm } from "node:fs/promises";
import { spawn } from "node:child_process";
import os from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { reason, backendOf, dataOf, BACKENDS, DEFAULT_BACKEND, isValidBackend } from "./dal/index";
import { runConformance, fieldTypesFromSchemaMap } from "./dal/conformance";
import { loadSchemaMap, tableToSnake, pascalToSnake, type SchemaMap } from "./dal/schema-map";
import { readRawStore, insertRow, patchRow, deleteRow, applyMutation } from "./dal/store";
import { registerControlRoutes } from "./control";
import { seedStoreFromRulebook } from "./dal/export-to-rulebook";
import { canonicalHash, projectStore, diffStores, readMarkers } from "./dal/edit-marker";
import { queryView } from "./dal/postgres";
import type { RawDb } from "./dal/reasoner";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const DB_PATH = path.join(__dirname, "db.json");
const RULEBOOK_PATH =
  process.env.ERB_RULEBOOK_PATH ||
  path.join(__dirname, "..", "..", "effortless-rulebook", "talismans-special-solutions-rulebook.json");
const FRONTEND_DIST = path.join(__dirname, "..", "frontend", "dist");
const PROJECT_ROOT = path.join(__dirname, "..", "..");
const PROJECT_NAME = process.env.PROJECT_NAME || "talismans-special-solutions";
const XLSX_INJECTOR = path.join(
  PROJECT_ROOT, "..", "..", "execution-substrates", "xlsx", "inject-into-xlsx.py"
);
const PORT = Number(process.env.PORT || 8088);

const app = express();
app.use(express.json({ limit: "4mb" }));

// --- raw-store access (engine-owned, Model B) -----------------------------
async function readStore(req: Request): Promise<RawDb> {
  return (await readRawStore(dataOf(req))) as RawDb;
}

// --- the execution-substrate bridge ---------------------------------------
// Hand the CHOSEN substrate the CURRENT raw rows from ITS OWN store, get back
// the fully reasoned JSON. If the substrate fails we surface its error verbatim
// and 500 — never a fallback to the OTHER engine or to raw rows. (CLAUDE.md.)
async function compute(req: Request) {
  const rules = backendOf(req);   // which engine computes
  const data = dataOf(req);       // which store the rows come from
  return reason((await readRawStore(data)) as RawDb, rules);
}

function wrap(handler: (req: Request, res: Response) => Promise<void>): RequestHandler {
  return async (req, res) => {
    try {
      await handler(req, res);
    } catch (e) {
      console.error(e);
      res.status(500).json({ error: (e as Error).message });
    }
  };
}

// --- API ------------------------------------------------------------------

app.get("/api/backends", (_req, res) => {
  res.json({
    default: DEFAULT_BACKEND,
    backends: Object.entries(BACKENDS).map(([id, b]) => ({ id, label: b.label })),
  });
});

app.get("/api/health", wrap(async (req, res) => {
  const reasoned = await compute(req);
  res.json({ status: "ok", engine: reasoned.engine, reasoned_triples: reasoned.reasoned_triples });
}));

app.get("/api/db", wrap(async (req, res) => {
  res.json(await readStore(req));
}));

app.get("/api/reasoned", wrap(async (req, res) => {
  res.json(await compute(req));
}));

app.get("/api/individuals/:cls", wrap(async (req, res) => {
  const reasoned = await compute(req);
  const rows = reasoned.individuals[req.params.cls];
  if (!rows) {
    res.status(404).json({ error: `unknown class: ${req.params.cls}` });
    return;
  }
  res.json(rows);
}));

app.get("/api/competency", wrap(async (req, res) => {
  const reasoned = await compute(req);
  res.json(reasoned.competency);
}));

// --- conformance: the live differential between the two substrates ---------
let _schemaMapCache: SchemaMap | null = null;
async function schemaMap(): Promise<SchemaMap> {
  // The rulebook only changes on a Rebuild (which restarts this process), so a
  // process-lifetime memo here has a real invalidation contract.
  if (!_schemaMapCache) _schemaMapCache = await loadSchemaMap(RULEBOOK_PATH);
  return _schemaMapCache;
}

app.get("/api/conformance", wrap(async (req, res) => {
  const sm = await schemaMap();
  const pkByClass: Record<string, string> = {};
  for (const [cls, t] of Object.entries(sm)) pkByClass[cls] = t.pk.camel;
  const fieldTypeByClass = fieldTypesFromSchemaMap(sm);
  const rows = (await readRawStore(dataOf(req))) as RawDb;
  const result = await runConformance(rows, pkByClass, fieldTypeByClass);
  res.json(result);
}));

// --- /api/story — the narrative payload ------------------------------------
function byId<T extends Record<string, unknown>>(rows: T[] | undefined, idKey: string): Map<unknown, T> {
  const m = new Map<unknown, T>();
  for (const r of rows || []) m.set(r[idKey], r);
  return m;
}

interface ResolvedAgent { id: string; name: string; kind: "human" | "ai" | "pipeline" | "unknown"; }

app.get("/api/story", wrap(async (req, res) => {
  const reasoned = await compute(req);
  const I = reasoned.individuals as Record<string, Array<Record<string, any>>>;

  const humans = byId(I.HumanAgents, "humanAgentId");
  const ais = byId(I.AIAgents, "aIAgentId");
  const pipes = byId(I.AutomatedPipelines, "automatedPipelineId");
  const roles = byId(I.Roles, "roleId");
  const gates = I.ApprovalGates || [];

  const resolveAgent = (id: string | null | undefined): ResolvedAgent | null => {
    if (!id) return null;
    if (humans.has(id)) return { id, name: humans.get(id)!.displayName, kind: "human" };
    if (ais.has(id)) return { id, name: ais.get(id)!.displayName, kind: "ai" };
    if (pipes.has(id)) return { id, name: pipes.get(id)!.displayName, kind: "pipeline" };
    return { id, name: id, kind: "unknown" };
  };

  const roleFiller = (roleId: string) => {
    const r = roles.get(roleId);
    if (!r) return null;
    const fid = r.filledByHumanAgent || r.filledByAIAgent || r.filledByAutomatedPipeline;
    return { roleId, roleName: r.displayName, agent: resolveAgent(fid), fillerType: r.fillerType };
  };

  const wf = (I.Workflows || [])[0] || {};
  const steps = (I.WorkflowSteps || [])
    .slice()
    .sort((a, b) => (a.sequencePosition || 0) - (b.sequencePosition || 0))
    .map((s) => {
      const filler = roleFiller(s.assignedRole);
      const gate = gates.find((g) => g.workflowStep === s.workflowStepId);
      return {
        id: s.workflowStepId,
        position: s.sequencePosition,
        title: s.displayName,
        roleId: s.assignedRole,
        requiresHumanApproval: !!s.requiresHumanApproval,
        executingAgentType: s.executingAgentType,
        role: filler ? filler.roleName : s.assignedRole,
        agent: filler ? filler.agent : null,
        durationMinutes: s.stepDurationMinutes,
        isApprovalGate: !!gate,
        gateName: gate ? gate.displayName : null,
        // The gate's defining properties. `escalationThresholdHours` is a raw field
        // the substrate emits directly. The two lookups — GateRole and
        // GateApproverHuman — are NOT in the engines' ApprovalGates individuals
        // payload (both substrates currently drop lookup columns from that export),
        // so we resolve them by following the SAME FK chain the rulebook formula
        // declares, over individuals already in hand:
        //   GateRole          = WorkflowStep.AssignedRole          → this step's role
        //   GateApproverHuman = GateRole.FilledByHumanAgent        → this step's human filler
        // Because the gate is matched to `s` by workflowStep === s.workflowStepId,
        // the gate's step IS this step, so `filler` already carries both. This is
        // chasing existing FKs, not re-deriving a calc the view owns.
        escalationThresholdHours: gate ? gate.escalationThresholdHours ?? null : null,
        gateRole: gate ? (filler ? filler.roleName : s.assignedRole) : null,
        gateApproverHuman: gate ? (filler && filler.agent?.kind === "human" ? filler.agent.name : null) : null,
        consistencyViolation: !!s.approvalConsistencyViolation,
        precedes: Array.isArray(s.precedesStep) ? s.precedesStep : s.precedesStep ? [s.precedesStep] : [],
      };
    });

  const team = {
    humans: (I.HumanAgents || []).map((h) => ({ id: h.humanAgentId, name: h.displayName })),
    ais: (I.AIAgents || []).map((a) => ({ id: a.aIAgentId, name: a.displayName })),
    pipelines: (I.AutomatedPipelines || []).map((p) => ({ id: p.automatedPipelineId, name: p.displayName })),
  };

  // --- CQ-backing projections ------------------------------------------------
  // The competency-question scoreboard reads its answers from these. NONE of
  // these computes a derived field: each value is lifted from a column the
  // substrate already populated (vw_<entity> / the reasoned individuals). We
  // only ARRANGE — resolve FK ids to labels and group by narrative order — the
  // same thing the rest of /api/story does. (Doctrine: the view IS the contract.)
  const stepById = new Map(steps.map((s) => [s.id, s]));
  const stepTitleById = new Map<string, string>(steps.map((s) => [s.id, s.title]));
  const stepAgentById = new Map<string, ResolvedAgent | null>(steps.map((s) => [s.id, s.agent]));
  const asList = (v: unknown): string[] =>
    Array.isArray(v) ? (v as string[]) : v ? [v as string] : [];

  // CQ4 — artifact provenance: producer step, wasDerivedFrom parent, the agent
  // it was attributedTo (human/AI/pipeline — whichever arm is set), downstream
  // consumers. All straight from WorkflowArtifacts' already-computed columns.
  const artifacts = (I.WorkflowArtifacts || [])
    .slice()
    .map((a) => {
      const attributedId = a.attributedToHumanAgent || a.attributedToAIAgent || a.attributedToAutomatedPipeline || null;
      return {
        id: a.artifactId,
        title: a.title,
        artifactType: a.artifactType || null,
        producedByStep: a.producedByStep || null,
        producedByStepTitle: a.producedByStep ? (stepTitleById.get(a.producedByStep) || a.producedByStep) : null,
        producedByStepPosition: a.producedByStep ? (stepById.get(a.producedByStep)?.position ?? null) : null,
        derivedFromArtifact: a.derivedFromArtifact || null,
        hasDerivationParent: !!a.hasDerivationParent,
        requiredBySteps: asList(a.requiredBySteps).map((sid) => ({ id: sid, title: stepTitleById.get(sid) || sid })),
        attributedTo: resolveAgent(attributedId),
        producingAgentType: a.producingAgentType || null,
      };
    })
    .sort((x, y) => (x.producedByStepPosition ?? 99) - (y.producedByStepPosition ?? 99));

  // CQ8 — datasets the review consumes + the AI agent that processed each:
  // Datasets.ConsumedBySteps gives the consuming step; that step's resolved
  // filler IS the processing agent (already resolved in `steps`).
  const datasets = (I.Datasets || []).map((d) => {
    const consumerIds = asList(d.consumedBySteps);
    const consumers = consumerIds.map((sid) => ({
      id: sid,
      title: stepTitleById.get(sid) || sid,
      agent: stepAgentById.get(sid) || null,
    }));
    return {
      id: d.datasetId,
      title: d.title,
      identifier: d.identifier || null,
      consumedBySteps: consumers,
    };
  });

  // The competency questions themselves — first-class rulebook rows. The
  // scoreboard renders these (question, target field, expected answer); the LIVE
  // answer is read from TargetTable.TargetField in the payload, never recomputed.
  const competencyQuestions = (I.CompetencyQuestions || [])
    .slice()
    .filter((c) => c.isActive !== false)
    .map((c) => ({
      id: c.competencyQuestionId,
      number: c.number,
      displayName: c.displayName,
      questionText: c.questionText,
      targetTable: c.targetTable,
      targetField: c.targetField,
      answerKind: c.answerKind,
      expectedAnswer: c.expectedAnswer,
      explanation: c.explanation || null,
      sortOrder: c.sortOrder ?? c.number,
      // The scenario this card's "Simulate" button applies — the minimal raw-fact
      // edit that moves THIS question's answer (isolated where one exists; for cq-2
      // it also ripples to cq-3). FK to Scenarios; null = no simulate button.
      simulateScenario: c.simulateScenario || null,
    }))
    .sort((x, y) => (x.sortOrder ?? 99) - (y.sortOrder ?? 99));

  const roleName = (id: string): string => (roles.get(id)?.displayName) || id;
  const delegRaw = (reasoned.competency as Record<string, any>).delegation || {};
  const deleg: Record<string, { from: string; to: { id: string; name: string }[] }> = {};
  for (const [fromId, toIds] of Object.entries(delegRaw)) {
    deleg[fromId] = {
      from: roleName(fromId),
      to: (toIds as string[]).map((id) => ({ id, name: roleName(id) })),
    };
  }

  const rolesList = (I.Roles || []).map((r) => ({
    id: r.roleId,
    name: r.displayName,
    fillerType: r.fillerType,
    filledByHumanAgent: r.filledByHumanAgent || null,
    filledByAIAgent: r.filledByAIAgent || null,
    filledByAutomatedPipeline: r.filledByAutomatedPipeline || null,
    // The escalation edge (raw) + the derived witness the org chart reads. Both
    // are columns the substrate already populated; we only lift them through.
    delegatesTo: r.delegatesTo || null,
    fillsApprovalGate: Number(r.fillsApprovalGate ?? 0),
    escalationViolation: !!r.escalationViolation,
  }));

  const stepFacts = (I.WorkflowSteps || [])
    .slice()
    .sort((a, b) => (a.sequencePosition || 0) - (b.sequencePosition || 0))
    .map((s) => ({
      id: s.workflowStepId,
      title: s.displayName,
      position: s.sequencePosition,
      assignedRole: s.assignedRole,
      requiresHumanApproval: !!s.requiresHumanApproval,
      stepDurationMinutes: s.stepDurationMinutes,
      consumesDataset: s.consumesDataset || null,
    }));

  const rawDb = await readStore(req);
  const edges = ((rawDb.StepPrecedence as Array<Record<string, any>>) || []).map((e) => ({
    id: e.stepPrecedenceId,
    from: e.fromStep,
    to: e.toStep,
  }));

  const options = {
    agents: [
      ...team.humans.map((h) => ({ ...h, kind: "human" })),
      ...team.ais.map((a) => ({ ...a, kind: "ai" })),
      ...team.pipelines.map((p) => ({ ...p, kind: "pipeline" })),
    ],
    roles: rolesList.map((r) => ({ id: r.id, name: r.name })),
    steps: stepFacts.map((s) => ({ id: s.id, title: s.title, position: s.position })),
    datasets: (I.Datasets || []).map((d) => ({ id: d.datasetId, name: d.displayName || d.datasetId })),
  };

  res.json({
    company: "Talisman's Special Solutions",
    workflow: {
      id: wf.workflowId,
      title: wf.displayName,
      status: wf.workflowStatus,
      countAISteps: wf.countAISteps,
      countHumanSteps: wf.countHumanSteps,
      modified: wf.modified,
      monthsSinceModified: wf.monthsSinceModified,
      stalenessThresholdMonths: wf.stalenessThresholdMonths,
      isStale: !!wf.isStale,
      // CQ7 backing — the cross-department derivation and its two input counts.
      involvesEngineeringAndLegal: !!wf.involvesEngineeringAndLegal,
      countEngineeringOwnedSteps: wf.countEngineeringOwnedSteps ?? 0,
      countLegalOwnedSteps: wf.countLegalOwnedSteps ?? 0,
    },
    team,
    steps,
    stepFacts,
    roles: rolesList,
    edges,
    options,
    delegation: deleg,
    closure: (reasoned.competency as Record<string, any>).precedence_closure,
    // CQ-backing projections (CQ4 artifacts, CQ8 datasets, the CQ suite itself).
    artifacts,
    datasets,
    competencyQuestions,
    engine: reasoned.engine,
    reasoned_triples: reasoned.reasoned_triples,
  });
}));

// --- writes: edit RAW rows, then re-reason -------------------------------
const COMPUTED_KEYS = new Set<string>([
  "iri", "relativePath", "parentPath", "name", "isStale",
  "monthsSinceModified",
  "countAISteps", "hasAIAgentStep",
  "precedesStep", "_iri", "_classes",
]);

app.post("/api/individuals/:cls", wrap(async (req, res) => {
  const cls = req.params.cls;
  const row = (req.body || {}) as Record<string, unknown>;
  for (const k of Object.keys(row)) {
    if (COMPUTED_KEYS.has(k)) {
      res.status(400).json({
        error: `'${k}' is a derived field — it is computed by the reasoner, not stored. Send raw fields only.`,
      });
      return;
    }
  }
  const result = await insertRow(dataOf(req), cls, row);
  res.status(201).json({ ok: true, ...result });
}));

app.patch("/api/individuals/:cls/:id", wrap(async (req, res) => {
  const { cls, id } = req.params;
  const patch = (req.body || {}) as Record<string, unknown>;
  for (const k of Object.keys(patch)) {
    if (COMPUTED_KEYS.has(k)) {
      res.status(400).json({ error: `'${k}' is a derived field; cannot set it.` });
      return;
    }
  }
  const result = await patchRow(dataOf(req), cls, id, patch);
  res.json({ ok: true, ...result });
}));

app.delete("/api/individuals/:cls/:id", wrap(async (req, res) => {
  const { cls, id } = req.params;
  const result = await deleteRow(dataOf(req), cls, id);
  res.json({ ok: true, ...result });
}));

// ---------------------------------------------------------------------------
// /api/export/xlsx — export the CURRENT live DB state as an Excel workbook.
//
// Doctrine-correct path (effortless-excel-export skill, adapted to this repo):
//   1. Load the rulebook (schema), CLEAR every table's data[].
//   2. Populate each table's data[] from its vw_<entity> view — the view IS the
//      contract, so every calculated/lookup/aggregation column comes for free.
//      Snake_case view columns are mapped back to the rulebook's PascalCase
//      field names via the schema-map naming functions.
//   3. Write the populated rulebook to a temp rulebook-export.json.
//   4. Run the repo-local rulebook-to-xlsx injector with ERB_RULEBOOK_PATH set
//      to that temp file (the injector reads ERB_RULEBOOK_PATH and writes
//      rulebook.xlsx into its cwd). NOTE: we invoke the injector DIRECTLY rather
//      than via the localhost:4242 proxy — the proxy derives its input from the
//      caller's cwd and would re-read the static rulebook, ignoring live edits.
//      We use spawn (async), never execSync.
//   5. Stream the workbook back as a download, then delete the temp files.
//
// Reserved top-level keys that are not tables (mirror the rulebook structure).
// ---------------------------------------------------------------------------
const RESERVED_RULEBOOK_KEYS = new Set([
  "$schema", "Name", "Description", "_meta", "__meta__",
]);

app.get("/api/export/xlsx", wrap(async (_req, res) => {
  const sm = await schemaMap();
  const rulebook = JSON.parse(await readFile(RULEBOOK_PATH, "utf8")) as Record<string, any>;
  const tableNames = Object.keys(rulebook).filter(
    (k) => !RESERVED_RULEBOOK_KEYS.has(k) && rulebook[k] && Array.isArray(rulebook[k].schema)
  );

  // Populate each table's data[] from its vw_<entity> view (snake -> Pascal).
  for (const table of tableNames) {
    rulebook[table].data = [];
    const viewName = "vw_" + tableToSnake(table);
    const schema: Array<{ name: string }> = rulebook[table].schema;
    // Read from the view — raise (don't swallow) if a view is missing: a missing
    // view means the build is wrong, and the export must not silently omit a sheet.
    const rows = await queryView(`SELECT * FROM ${viewName}`);
    rulebook[table].data = rows.map((row) => {
      const mapped: Record<string, unknown> = {};
      for (const field of schema) {
        const snakeKey = pascalToSnake(field.name);
        if (snakeKey in row) mapped[field.name] = row[snakeKey];
      }
      return mapped;
    });
  }

  // Write the populated export rulebook + run the injector from a temp dir.
  const tmpDir = await mkdtemp(path.join(os.tmpdir(), "erb-xlsx-"));
  const exportPath = path.join(tmpDir, "rulebook-export.json");
  const xlsxPath = path.join(tmpDir, "rulebook.xlsx");
  const xlsxFilename = `${PROJECT_NAME}-rulebook.xlsx`;
  await writeFile(exportPath, JSON.stringify(rulebook, null, 2));

  await new Promise<void>((resolve, reject) => {
    const proc = spawn("python3", [XLSX_INJECTOR], {
      cwd: tmpDir,
      env: { ...process.env, ERB_RULEBOOK_PATH: exportPath },
    });
    let err = "";
    proc.stderr.on("data", (d) => (err += d.toString()));
    proc.on("error", reject);
    proc.on("close", (code) =>
      code === 0 ? resolve() : reject(new Error(`rulebook-to-xlsx exited ${code}: ${err.trim()}`))
    );
  });

  if (!existsSync(xlsxPath)) {
    await rm(tmpDir, { recursive: true, force: true });
    throw new Error(`rulebook-to-xlsx produced no workbook at ${xlsxPath}`);
  }

  res.download(xlsxPath, xlsxFilename, () => {
    void rm(tmpDir, { recursive: true, force: true });
  });
}));

// --- scenarios (DATA-DRIVEN from the rulebook Scenarios table) -------------
// An edit is one of: set raw fields on an existing row (set), delete a row by id
// (delete), or insert/replace a row (add). delete/add exist because some
// scenarios must change STRUCTURE, not just values — e.g. cq-1 drops an ordering
// edge to shrink the precedence closure. (The reasoner settles cleanly on a row
// add or delete, but NOT on a `set` that mangles a surviving edge's endpoint, so
// structural scenarios must add/delete whole rows, never re-point an edge.)
interface ScenarioEdit {
  class: string;
  id?: string;
  match?: string;
  set?: Record<string, unknown>;
  delete?: boolean;
  add?: Record<string, unknown>;
}
interface ScenarioRow {
  id: string; label: string; icon: string; explanation: string;
  isReset: boolean; sortOrder: number; edits: ScenarioEdit[];
}

let _scenarioCache: ScenarioRow[] | null = null;
async function loadScenarios(): Promise<ScenarioRow[]> {
  if (_scenarioCache) return _scenarioCache;
  const rb = JSON.parse(await readFile(RULEBOOK_PATH, "utf8")) as Record<string, any>;
  const table = rb.Scenarios;
  if (!table || !Array.isArray(table.data)) {
    throw new Error(
      `No 'Scenarios' table in the rulebook (${RULEBOOK_PATH}). ` +
      `Scenarios are SSoT-driven — add the table to the rulebook, don't hardcode them here.`
    );
  }
  _scenarioCache = (table.data as Array<Record<string, any>>)
    .map((row) => ({
      id: row.ScenarioId,
      label: row.Label,
      icon: row.Icon || "",
      explanation: row.Explanation || "",
      isReset: !!row.IsReset,
      sortOrder: Number(row.SortOrder ?? 0),
      edits: JSON.parse(row.Edits) as ScenarioEdit[],
    }))
    .sort((a, b) => a.sortOrder - b.sortOrder);
  return _scenarioCache;
}

function locateRow(db: RawDb, cls: string, spec: ScenarioEdit): Record<string, unknown> {
  const rows = db[cls];
  if (!Array.isArray(rows)) throw new Error(`scenario references unknown class '${cls}'`);
  if (spec.match === "first") {
    if (!rows.length) throw new Error(`scenario targets first '${cls}' but the table is empty`);
    return rows[0];
  }
  const row = rows.find((r) =>
    Object.entries(r).some(([k, v]) => k.endsWith("Id") && v === spec.id)
  );
  if (!row) throw new Error(`scenario references '${cls}' row '${spec.id}' that does not exist`);
  return row;
}

function assertRawOnly(cls: string, fields: Record<string, unknown>): void {
  for (const k of Object.keys(fields)) {
    if (COMPUTED_KEYS.has(k)) {
      throw new Error(`scenario edit sets derived field '${k}' on ${cls}; scenarios edit raw facts only`);
    }
  }
}

function replayEdits(db: RawDb, edits: ScenarioEdit[]): void {
  // Apply STRUCTURAL edits (delete/add) before value edits (set), independent of
  // the order they appear in. This keeps reset robust: when one scenario deletes
  // a row (cq-1's drop-final-edge removes prec-4-5) and reset both re-adds the
  // row AND sets fields on it, the add must land before the set — otherwise the
  // set hits a missing row and throws, aborting reset half-applied.
  const structural = edits.filter((e) => e.delete || e.add);
  const sets = edits.filter((e) => !e.delete && !e.add);

  for (const e of structural) {
    const rows = db[e.class];
    if (!Array.isArray(rows)) throw new Error(`scenario references unknown class '${e.class}'`);
    if (e.delete) {
      // Remove the row by id. Idempotent (no-op if already absent) so a scenario
      // can be re-applied and reset stays a safe restore.
      if (!e.id) throw new Error(`scenario delete on '${e.class}' needs an id`);
      const i = rows.findIndex((r) => Object.entries(r).some(([k, v]) => k.endsWith("Id") && v === e.id));
      if (i >= 0) rows.splice(i, 1);
    } else if (e.add) {
      // Insert (or replace, by id) a whole raw row — reset uses this to restore a
      // structurally-removed row. Idempotent: same id replaces in place.
      assertRawOnly(e.class, e.add);
      const idKey = Object.keys(e.add).find((k) => k.endsWith("Id"));
      const id = idKey ? e.add[idKey] : undefined;
      const i = id !== undefined ? rows.findIndex((r) => r[idKey!] === id) : -1;
      if (i >= 0) rows[i] = { ...e.add };
      else rows.push({ ...e.add });
    }
  }

  for (const e of sets) {
    // set: mutate raw fields on an existing row (the common case).
    const row = locateRow(db, e.class, e);
    assertRawOnly(e.class, e.set || {});
    for (const [k, v] of Object.entries(e.set || {})) row[k] = v;
  }
}

app.get("/api/scenarios", wrap(async (_req, res) => {
  const scenarios = await loadScenarios();
  res.json(
    scenarios.map((s) => ({
      id: s.id, label: s.label, icon: s.icon, explanation: s.explanation, isReset: s.isReset,
    }))
  );
}));

app.post("/api/scenario/:name", wrap(async (req, res) => {
  const scenarios = await loadScenarios();
  const s = scenarios.find((x) => x.id === req.params.name);
  if (!s) {
    res.status(404).json({ error: `unknown scenario: ${req.params.name}` });
    return;
  }
  const result = await applyMutation(dataOf(req), (db: RawDb) => replayEdits(db, s.edits));
  res.json({ ok: true, scenario: req.params.name, ...result });
}));

// --- the triangle + diff: live drift across the three raw stores -----------
async function loadProjectedStores() {
  const sm = await schemaMap();
  const [rbStore, reasonerStore, postgresStore] = await Promise.all([
    seedStoreFromRulebook(RULEBOOK_PATH),
    readRawStore("reasoner"),
    readRawStore("postgres"),
  ]);
  return {
    sm,
    rulebook: projectStore(rbStore, sm),
    reasoner: projectStore(reasonerStore, sm),
    postgres: projectStore(postgresStore, sm),
  };
}

app.get("/api/triangle", wrap(async (_req, res) => {
  const proj = await loadProjectedStores();
  const hashes = {
    rulebook: canonicalHash(proj.rulebook),
    reasoner: canonicalHash(proj.reasoner),
    postgres: canonicalHash(proj.postgres),
  };

  const rEqRb = hashes.reasoner === hashes.rulebook;
  const pEqRb = hashes.postgres === hashes.rulebook;
  const rEqP = hashes.reasoner === hashes.postgres;

  let aheadOf: string | null = null;
  let action: string | null = null;
  if (rEqRb && pEqRb) {
    aheadOf = null;
  } else if (rEqRb && !pEqRb) {
    aheadOf = "postgres";
    action = "rebuild-from-postgres";
  } else if (pEqRb && !rEqRb) {
    aheadOf = "reasoner";
    action = "rebuild-from-reasoner";
  } else if (!rEqRb && !pEqRb && rEqP) {
    aheadOf = "rulebook";
    action = "rebuild";
  } else {
    aheadOf = "diverged";
    action = "reset";
  }

  const markers = await readMarkers();
  let rulebookMtime: string | null = null;
  try {
    const { stat } = await import("node:fs/promises");
    rulebookMtime = (await stat(RULEBOOK_PATH)).mtime.toISOString();
  } catch {
    /* mtime is best-effort adornment */
  }

  res.json({
    aheadOf,
    action,
    hashes,
    legs: {
      rulebook: { inSyncWithReasoner: rEqRb, inSyncWithPostgres: pEqRb, lastEditAt: rulebookMtime },
      reasoner: { inSyncWithRulebook: rEqRb, lastEditAt: markers.reasoner?.lastEditAt || null },
      postgres: { inSyncWithRulebook: pEqRb, lastEditAt: markers.postgres?.lastEditAt || null },
    },
  });
}));

app.get("/api/diff", wrap(async (req, res) => {
  const head = (req.query.head || "rulebook").toString();
  if (!["rulebook", "reasoner", "postgres"].includes(head)) {
    res.status(400).json({ error: `head must be rulebook|reasoner|postgres (got '${head}')` });
    return;
  }
  const proj = await loadProjectedStores();

  const tmpDir = await mkdtemp(path.join(os.tmpdir(), "erb-diff-"));
  const files: Record<string, string> = {};
  for (const store of ["rulebook", "reasoner", "postgres"] as const) {
    const fp = path.join(tmpDir, `${store}.projected.json`);
    await writeFile(fp, JSON.stringify(proj[store], null, 2) + "\n", "utf8");
    files[store] = fp;
  }

  const others = (["rulebook", "reasoner", "postgres"] as const).filter((s) => s !== head);
  const against: Record<string, unknown> = {};
  for (const other of others) {
    against[other] = diffStores(
      proj[head as "rulebook" | "reasoner" | "postgres"],
      proj[other],
      proj.sm
    );
  }

  res.json({
    head,
    files,
    against,
    hashes: {
      rulebook: canonicalHash(proj.rulebook),
      reasoner: canonicalHash(proj.reasoner),
      postgres: canonicalHash(proj.postgres),
    },
  });
}));

// --- conformance test harness ---------------------------------------------
// The admin Conformance section lists the rulebook's ConformanceTests, runs the
// whole suite against BOTH engines on demand, and shows the last run. The harness
// is engine-agnostic (it calls the same reason() the rest of the app uses) and
// writes a run-log to testing/conformance-runs/ that survives a restart.
const CONFORMANCE_RUNS_DIR = path.join(PROJECT_ROOT, "testing", "conformance-runs");

// List every test (metadata only — no execution). Cheap; the admin list calls it.
app.get("/api/conformance/tests", wrap(async (_req, res) => {
  const { ConformanceHarness } = await import("./conformance/harness");
  const h = new ConformanceHarness();
  await h.load();
  res.json({ engines: (await import("./conformance/harness")).ENGINE_IDS, tests: h.listTests() });
}));

// The last run-log (committed latest.json), so the admin shows results without
// re-running. 404 (not 500, not a fake empty run) if no run has happened yet —
// the UI distinguishes "never run" from "ran and failed".
app.get("/api/conformance/latest", wrap(async (_req, res) => {
  const latest = path.join(CONFORMANCE_RUNS_DIR, "latest.json");
  if (!existsSync(latest)) { res.status(404).json({ error: "no conformance run yet — POST /api/conformance/run" }); return; }
  res.json(JSON.parse(await readFile(latest, "utf8")));
}));

// Run the suite NOW (optionally a subset via {only:[ids]}) and write the log.
// This actually executes both engines — it can take ~100s for the full suite, so
// the admin shows a spinner. We never fake or cache a result here.
app.post("/api/conformance/run", wrap(async (req, res) => {
  const { ConformanceHarness } = await import("./conformance/harness");
  const only = Array.isArray(req.body?.only) ? (req.body.only as string[]) : undefined;
  const h = new ConformanceHarness();
  await h.load();
  const report = await h.run({ only });
  // Persist the canonical baseline (latest.json) ONLY for a FULL run. A subset
  // run (the per-row ▶ button) returns its results for the client to merge into
  // the displayed table, but must NOT clobber latest.json — otherwise a refresh
  // would show "3 tests" instead of the full 72. We still archive every run
  // (full or subset) as its own timestamped log for the audit trail.
  const { mkdir } = await import("node:fs/promises");
  await mkdir(CONFORMANCE_RUNS_DIR, { recursive: true });
  const stamp = report.summary.startedAt.replace(/[:.]/g, "-");
  const tag = only ? `-subset-${only.length}` : "";
  await writeFile(path.join(CONFORMANCE_RUNS_DIR, `run-${stamp}${tag}.json`), JSON.stringify(report, null, 2) + "\n", "utf8");
  if (!only) {
    await writeFile(path.join(CONFORMANCE_RUNS_DIR, "latest.json"), JSON.stringify(report, null, 2) + "\n", "utf8");
  }
  res.json(report);
}));

// --- control plane (Reset / Rebuild) --------------------------------------
registerControlRoutes(app);

// --- serve the built frontend (production) --------------------------------
// react-router uses BrowserRouter (real URLs), so any non-/api path must serve
// index.html and let the client router resolve it — covers /console/graph,
// /dag/Workflows/IsStale, etc. deep links and reloads.
if (existsSync(FRONTEND_DIST)) {
  app.use(express.static(FRONTEND_DIST));
  app.get(/^(?!\/api\/).*/, (_req, res) => {
    res.sendFile(path.join(FRONTEND_DIST, "index.html"));
  });
}

app.listen(PORT, () => {
  console.log(`[backend] Talisman workflow API on http://localhost:${PORT}`);
  console.log(`[backend] rules engines: ${Object.keys(BACKENDS).join(", ")} (default: ${DEFAULT_BACKEND})`);
  console.log(`[backend] db.json:  ${DB_PATH}`);
});

// isValidBackend is re-exported by control.ts consumers; referenced here to keep
// the import meaningful if a future route needs it.
void isValidBackend;
