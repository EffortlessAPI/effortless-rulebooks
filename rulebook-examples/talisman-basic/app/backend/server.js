// ---------------------------------------------------------------------------
// Talisman BASIC — Workflow app backend (Express)
//
// Responsibilities, and ONLY these:
//   1. Own the database: app/backend/db.json (raw, editable rows).
//   2. Drive a CHOSEN execution substrate for every read that needs a derived
//      field — either the OWL/SHACL reasoner (reasoner/reason.py) or the
//      Postgres views — via the DAL router (dal/index.js). The frontend's
//      login page picks which; api.js attaches it as the X-ERB-Backend header.
//   3. Serve the API the Vite frontend hits.
//
// What this file deliberately does NOT do: compute a single workflow field.
// No closure walk, no COUNTIFS, no "isStale", no formula parsing. Those are the
// substrate's job — and BOTH substrates are generated from the same rulebook,
// so this file is engine-agnostic: it hands the raw db to whichever engine the
// request named and arranges the answer into a story. (Project doctrine: "the
// view IS the contract" — the reasoned graph / the vw_<entity> view is the
// view; we read it, we don't re-derive it.)
// ---------------------------------------------------------------------------
import express from "express";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { reason, backendOf, dataOf, BACKENDS, DEFAULT_BACKEND } from "./dal/index.js";
import { runConformance, fieldTypesFromSchemaMap } from "./dal/conformance.js";
import { loadSchemaMap } from "./dal/schema-map.js";
import {
  readRawStore,
  insertRow,
  patchRow,
  deleteRow,
  applyMutation,
} from "./dal/store.js";
import { registerControlRoutes } from "./control.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const DB_PATH = path.join(__dirname, "db.json");
const RULEBOOK_PATH =
  process.env.ERB_RULEBOOK_PATH ||
  path.join(__dirname, "..", "..", "effortless-rulebook", "talisman-basic-rulebook.json");
const FRONTEND_DIST = path.join(__dirname, "..", "frontend", "dist");
const PORT = process.env.PORT || 8088;

const app = express();
app.use(express.json({ limit: "4mb" }));

// --- raw-store access (engine-owned, Model B) -----------------------------
// Each engine owns its OWN raw store (reasoner -> db.json; postgres -> the
// base tables). `readStore(req)` reads whichever store the request's backend
// owns, always fresh — no in-memory cache (that would be the forbidden
// "bespoke cache without an invalidation contract"). The stores can drift
// between rebuilds; that drift is surfaced, never silently reconciled.
async function readStore(req) {
  return await readRawStore(dataOf(req));
}

// --- the execution-substrate bridge ---------------------------------------
// Hand the CHOSEN substrate the CURRENT raw rows from ITS OWN store, get back
// the fully reasoned JSON (raw + every derived field + competency answers). The
// choice comes off the request (X-ERB-Backend header / ?backend=); see
// dal/index.js. If the substrate fails we surface its error verbatim and 500 —
// we never fall back to the OTHER engine or to raw, unreasoned rows, because
// that would hand the frontend numbers the chosen substrate never produced.
// (CLAUDE.md: Avoid Silent Fallbacks.)
//
// `compute(req)` is the one call every read handler makes; it reads the backend
// off the request so a single endpoint serves either engine with no branching.
async function compute(req) {
  const rules = backendOf(req);   // which engine computes
  const data = dataOf(req);       // which store the rows come from
  // The two axes are independent: a cross-run (e.g. Postgres-view rules over the
  // reasoner's db.json rows) reads one store and computes with the other engine.
  // The natural pairings have rules === data.
  return reason(await readRawStore(data), rules);
}

function wrap(handler) {
  return async (req, res) => {
    try {
      await handler(req, res);
    } catch (e) {
      console.error(e);
      res.status(500).json({ error: e.message });
    }
  };
}

// --- API ------------------------------------------------------------------

// Which substrates exist, and which is the default. The login page reads this
// to render the toggle (no hardcoded engine list in the UI).
app.get("/api/backends", (_req, res) => {
  res.json({
    default: DEFAULT_BACKEND,
    backends: Object.entries(BACKENDS).map(([id, b]) => ({ id, label: b.label })),
  });
});

// Health: also proves the CHOSEN substrate is reachable and computes.
app.get("/api/health", wrap(async (req, res) => {
  const reasoned = await compute(req);
  res.json({ status: "ok", engine: reasoned.engine, reasoned_triples: reasoned.reasoned_triples });
}));

// The raw rows of the active engine's store (what's editable). No derivation
// here on purpose — this is the raw side of whichever store the request names.
app.get("/api/db", wrap(async (req, res) => {
  res.json(await readStore(req));
}));

// The fully reasoned world from the chosen engine: every individual with its
// derived fields, plus the competency-question answers.
app.get("/api/reasoned", wrap(async (req, res) => {
  res.json(await compute(req));
}));

// Just the individuals of one class (reasoned). e.g. /api/individuals/Workflows
app.get("/api/individuals/:cls", wrap(async (req, res) => {
  const reasoned = await compute(req);
  const rows = reasoned.individuals[req.params.cls];
  if (!rows) {
    return res.status(404).json({ error: `unknown class: ${req.params.cls}` });
  }
  res.json(rows);
}));

// The competency-question answers only (the "look what the engine inferred").
app.get("/api/competency", wrap(async (req, res) => {
  const reasoned = await compute(req);
  res.json(reasoned.competency);
}));

// ---------------------------------------------------------------------------
// /api/conformance — the live differential between the two substrates.
//
// Runs BOTH engines on the current db and reports every field where their
// reasoned answers disagree, classified (equal / representation / presence /
// value). This is the toggle's headline: not just "switch engines" but PROVE
// they compute the same answers — and when they don't, show exactly which
// field. We never hide a mismatch (CLAUDE.md: Avoid Silent Fallbacks); a
// `value` disagreement on a calculated/aggregation field is a real bug in one
// of the generated substrates, and the UI shows it as such.
// ---------------------------------------------------------------------------
let _schemaMapCache = null;
async function schemaMap() {
  // The rulebook only changes on a Rebuild (which restarts this process), so a
  // process-lifetime memo here has a real invalidation contract: the process
  // dies and is replaced on every rebuild. It is not a bespoke cache of a value
  // that can drift under us.
  if (!_schemaMapCache) _schemaMapCache = await loadSchemaMap(RULEBOOK_PATH);
  return _schemaMapCache;
}

app.get("/api/conformance", wrap(async (req, res) => {
  const sm = await schemaMap();
  const pkByClass = {};
  for (const [cls, t] of Object.entries(sm)) pkByClass[cls] = t.pk.camel;
  const fieldTypeByClass = fieldTypesFromSchemaMap(sm);
  // Run BOTH engines on the SAME raw rows — the active data store — so the diff
  // isolates the RULES axis (do the two generated engines agree?) from the DATA
  // axis. By default that's the request's store; ?data= overrides for comparing
  // on the other store.
  const rows = await readRawStore(dataOf(req));
  const result = await runConformance(rows, pkByClass, fieldTypeByClass);
  res.json(result);
}));

// ---------------------------------------------------------------------------
// /api/story — the narrative payload.
//
// The frontend tells a business story (a software release, start to finish),
// not an ontology browser. This endpoint does the ONE job the UI shouldn't:
// stitch the reasoned individuals into the shape that story needs — the
// workflow, its steps in order with each step's resolved executing agent and
// type, the team grouped by kind, the approval gate, the delegation chain, and
// the final compliance verdict with the three facts that drive it.
//
// Every value here is READ from the reasoned graph (reasoned.individuals /
// reasoned.competency). We do not compute a single derived field — we arrange
// already-derived ones into a narrative order. (Same doctrine as the rest of
// this file: the reasoner is the view; we read it.)
// ---------------------------------------------------------------------------
function byId(rows, idKey) {
  const m = new Map();
  for (const r of rows || []) m.set(r[idKey], r);
  return m;
}

app.get("/api/story", wrap(async (req, res) => {
  const reasoned = await compute(req);
  const I = reasoned.individuals;

  const humans = byId(I.HumanAgents, "humanAgentId");
  const ais = byId(I.AIAgents, "aIAgentId");
  const pipes = byId(I.AutomatedPipelines, "automatedPipelineId");
  const roles = byId(I.Roles, "roleId");
  const gates = I.ApprovalGates || [];

  // Resolve an agent id -> a friendly {id, name, kind} regardless of which
  // of the three disjoint agent pools it lives in.
  const resolveAgent = (id) => {
    if (!id) return null;
    if (humans.has(id)) return { id, name: humans.get(id).displayName, kind: "human" };
    if (ais.has(id)) return { id, name: ais.get(id).displayName, kind: "ai" };
    if (pipes.has(id)) return { id, name: pipes.get(id).displayName, kind: "pipeline" };
    return { id, name: id, kind: "unknown" };
  };

  const roleFiller = (roleId) => {
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
        executingAgentType: s.executingAgentType,     // reasoner-resolved (Human/AI/Pipeline)
        role: filler ? filler.roleName : s.assignedRole,
        agent: filler ? filler.agent : null,
        durationMinutes: s.stepDurationMinutes,
        isApprovalGate: !!gate,
        gateName: gate ? gate.displayName : null,
        // The reasoner's consistency witness: TRUE iff this step requires human
        // approval but is NOT filled by a human. A clean ABox has this false for
        // every step; hand an approval step to an AI and it lights up.
        consistencyViolation: !!s.approvalConsistencyViolation,
        precedes: Array.isArray(s.precedesStep) ? s.precedesStep : s.precedesStep ? [s.precedesStep] : [],
      };
    });

  // Team, grouped by the three disjoint kinds.
  const team = {
    humans: (I.HumanAgents || []).map((h) => ({ id: h.humanAgentId, name: h.displayName })),
    ais: (I.AIAgents || []).map((a) => ({ id: a.aIAgentId, name: a.displayName })),
    pipelines: (I.AutomatedPipelines || []).map((p) => ({ id: p.automatedPipelineId, name: p.displayName })),
  };

  // Delegation chain: the reasoned closure is keyed/valued by role id. Resolve
  // each id to its display name via a lookup over the reasoned Roles, so the UI
  // renders human-readable names without inventing them. (A name lookup is a
  // legitimate read of an already-derived field — not a re-derivation.)
  const roleName = (id) => (roles.get(id)?.displayName) || id;
  const delegRaw = reasoned.competency.delegation || {};
  const deleg = {};
  for (const [fromId, toIds] of Object.entries(delegRaw)) {
    deleg[fromId] = {
      from: roleName(fromId),
      to: toIds.map((id) => ({ id, name: roleName(id) })),
    };
  }

  const verdictRow = (I.ComplianceVerdicts || [])[0] || {};
  const verdict = {
    workflowTitle: verdictRow.workflowTitle,
    monthsSinceReview: verdictRow.monthsSinceReview,
    isStale: !!verdictRow.isStale,
    // The rulebook field is "AIStepCount" -> camel "aIStepCount" in BOTH engines
    // (the camel comes straight from the PascalCase schema name). Read that key.
    aiStepCount: verdictRow.aIStepCount,
    hasAIExecutedStep: !!verdictRow.hasAIExecutedStep,
    // Time-budget rule (extension beyond the article): the third verdict input.
    totalPlanMinutes: verdictRow.totalPlanMinutes,
    timeBudgetMinutes: verdictRow.timeBudgetMinutes,
    isOverTimeBudget: !!verdictRow.isOverTimeBudget,
    isAtComplianceRisk: !!verdictRow.isAtComplianceRisk,
    statement: verdictRow.verdict,
  };

  // Per-step role assignment, exposed so the UI can edit Roles.filledBy*.
  const rolesList = (I.Roles || []).map((r) => ({
    id: r.roleId,
    name: r.displayName,
    fillerType: r.fillerType,
    filledByHumanAgent: r.filledByHumanAgent || null,
    filledByAIAgent: r.filledByAIAgent || null,
    filledByAutomatedPipeline: r.filledByAutomatedPipeline || null,
  }));

  // Raw step facts the inline editor needs (separate from the narrative `steps`).
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

  // Asserted precedence edges, for the rewire UI. Read from the RAW db rows, not
  // the reasoned individuals: the reasoner's StepPrecedence output folds fromStep
  // and toStep together (both are WorkflowSteps FKs), so the single directed
  // edge isn't recoverable there. The asserted edge is raw data anyway — no
  // derivation needed — so the raw row is the right source.
  const rawDb = await readStore(req);
  const edges = (rawDb.StepPrecedence || []).map((e) => ({
    id: e.stepPrecedenceId,
    from: e.fromStep,
    to: e.toStep,
  }));

  // Option lists for the editors: every agent (grouped), every role, every step.
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
    company: "Special Solutions",
    workflow: {
      id: wf.workflowId,
      title: wf.displayName,
      status: wf.workflowStatus,
      countAISteps: wf.countAISteps,
      countHumanSteps: wf.countHumanSteps,
      // Time-budget rule (extension): the live sum vs. the configurable budget,
      // so the console can draw the green→yellow→red time bar from derived values.
      totalPlanMinutes: wf.countTotalPlanMinutes,
      maxPlanMinutes: wf.maxPlanMinutes,
      isOverTimeBudget: !!wf.isOverTimeBudget,
      // Staleness rule (CQ5): the review date (raw, editable), the derived gap,
      // the configurable policy threshold, and the derived verdict — so the
      // console can draw the staleness timeline/gauge entirely from these values.
      modified: wf.modified,
      monthsSinceModified: wf.monthsSinceModified,
      stalenessThresholdMonths: wf.stalenessThresholdMonths,
      isStale: !!wf.isStale,
    },
    team,
    steps,
    stepFacts,
    roles: rolesList,
    edges,
    options,
    delegation: deleg,
    closure: reasoned.competency.precedence_closure,
    verdict,
    engine: reasoned.engine,
    reasoned_triples: reasoned.reasoned_triples,
  });
}));

// --- writes: edit RAW rows, then re-reason -------------------------------
// Add a row to a class. The body is raw fields only. We do NOT accept derived
// fields — if the caller sends one, it is ignored by the reasoner anyway (the
// SHACL rule overwrites it), but we reject obvious computed keys to keep the db
// honest.
const COMPUTED_KEYS = new Set([
  "iri", "relativePath", "parentPath", "name", "isStale",
  "monthsSinceModified", "countAISteps", "hasAIAgentStep",
  "precedesStep", "_iri", "_classes",
]);

app.post("/api/individuals/:cls", wrap(async (req, res) => {
  const cls = req.params.cls;
  const row = req.body || {};
  for (const k of Object.keys(row)) {
    if (COMPUTED_KEYS.has(k)) {
      return res.status(400).json({
        error: `'${k}' is a derived field — it is computed by the reasoner, not stored. Send raw fields only.`,
      });
    }
  }
  // The write lands in the ACTIVE DATA store (db.json for the reasoner pairing,
  // the Postgres base tables for the postgres pairing — Model B, each engine
  // owns its store). store.js re-computes to validate BEFORE persisting (file)
  // / inside the txn semantics (pg), so a broken edit 500s without corrupting
  // the store. The edit is visible on the next read of THAT store.
  const result = await insertRow(dataOf(req), cls, row);
  res.status(201).json({ ok: true, ...result });
}));

// Patch a row (by its *Id primary key) with raw fields.
app.patch("/api/individuals/:cls/:id", wrap(async (req, res) => {
  const { cls, id } = req.params;
  const patch = req.body || {};
  for (const k of Object.keys(patch)) {
    if (COMPUTED_KEYS.has(k)) {
      return res.status(400).json({ error: `'${k}' is a derived field; cannot set it.` });
    }
  }
  const result = await patchRow(dataOf(req), cls, id, patch);
  res.json({ ok: true, ...result });
}));

// Delete a row by its *Id primary key. Same contract as the writes above:
// validate (re-compute) before persisting, so a delete that would break the
// model (orphan a reference) surfaces as a 500 and never corrupts the store.
app.delete("/api/individuals/:cls/:id", wrap(async (req, res) => {
  const { cls, id } = req.params;
  const result = await deleteRow(dataOf(req), cls, id);
  res.json({ ok: true, ...result });
}));

// ---------------------------------------------------------------------------
// /api/scenario/:name — apply a curated preset (several raw edits at once),
// then re-reason. Each preset is just a function that mutates the raw db; the
// drama is entirely in what the reasoner DERIVES afterward (a verdict flips, a
// consistency violation lights up). Presets never touch a derived field.
// ---------------------------------------------------------------------------
const RM = "ntwf-release-manager-role";
const RISK_ROLE = "ntwf-risk-analysis-role";
const LEGAL_ROLE = "ntwf-legal-compliance-role";

// Helpers that edit raw rows in a db object.
function findRow(db, cls, id) {
  return (db[cls] || []).find((r) =>
    Object.entries(r).some(([k, v]) => k.endsWith("Id") && v === id)
  );
}
function setRoleFiller(db, roleId, kind, agentId) {
  const r = findRow(db, "Roles", roleId);
  if (!r) return;
  r.filledByHumanAgent = kind === "human" ? agentId : "";
  r.filledByAIAgent = kind === "ai" ? agentId : "";
  r.filledByAutomatedPipeline = kind === "pipeline" ? agentId : "";
}
function setWorkflowModified(db, iso) {
  const wf = (db.Workflows || [])[0];
  if (wf) wf.modified = iso;
}

const FRESH = "2026-04-03T00:00:00-05:00";
const STALE = "2023-01-01T00:00:00-06:00";

// The canonical baseline assignments (mirror the rulebook seed). "reset"
// restores these so a demo can always get back to a known-good state.
const BASELINE = {
  modified: FRESH,
  fillers: [
    [RM, "human", "ntwf-maria-gonzalez"],
    [RISK_ROLE, "ai", "ntwf-risk-ai"],
    [LEGAL_ROLE, "human", "ntwf-james-okafor"],
    ["ntwf-ci-executor-role", "pipeline", "ntwf-ci-pipeline"],
    ["ntwf-vp-engineering-role", "human", "ntwf-david-chen"],
    ["ntwf-cto-role", "human", "ntwf-sarah-kim"],
    ["ntwf-deployment-health-role", "ai", "ntwf-health-ai"],
  ],
};
function applyBaseline(db) {
  setWorkflowModified(db, BASELINE.modified);
  for (const [role, kind, agent] of BASELINE.fillers) setRoleFiller(db, role, kind, agent);
}

const SCENARIOS = {
  // Make the workflow stale → since it already has AI steps, the verdict flips
  // to AT RISK. The single most direct "watch the reasoner react" demo.
  "trigger-risk": {
    label: "Trigger compliance risk",
    apply: (db) => { applyBaseline(db); setWorkflowModified(db, STALE); },
  },
  // Replace every AI/pipeline filler with a human → countAISteps goes to 0, and
  // the verdict can no longer be at-risk (no AI step), regardless of staleness.
  "all-human": {
    label: "All-human release",
    apply: (db) => {
      applyBaseline(db);
      setRoleFiller(db, RISK_ROLE, "human", "ntwf-david-chen");
      setRoleFiller(db, "ntwf-deployment-health-role", "human", "ntwf-sarah-kim");
      setRoleFiller(db, "ntwf-ci-executor-role", "human", "ntwf-james-okafor");
    },
  },
  // Hand the human approval-gate role (Release Manager) to an AI → the step-3
  // approvalConsistencyViolation witness lights up: a requires-human-approval
  // step is no longer human-filled. The reasoner catches the rule break.
  "ai-at-gate": {
    label: "AI at the approval gate",
    apply: (db) => { applyBaseline(db); setRoleFiller(db, RM, "ai", "ntwf-risk-ai"); },
  },
  reset: { label: "Reset to baseline", apply: applyBaseline },
};

app.get("/api/scenarios", (_req, res) => {
  res.json(Object.entries(SCENARIOS).map(([id, s]) => ({ id, label: s.label })));
});

app.post("/api/scenario/:name", wrap(async (req, res) => {
  const s = SCENARIOS[req.params.name];
  if (!s) return res.status(404).json({ error: `unknown scenario: ${req.params.name}` });
  // Apply the preset's raw mutations to the ACTIVE DATA store, then re-compute
  // to validate before persisting (store.js handles file-vs-pg persistence).
  const result = await applyMutation(dataOf(req), s.apply);
  res.json({ ok: true, scenario: req.params.name, ...result });
}));

// --- control plane (Reset / Rebuild) --------------------------------------
// The login page's Reset / Rebuild-from-X / Rebuild actions: each shells out to
// an orchestration script and streams progress (SSE). Defined in control.js so
// this file stays about reading/writing rows, not running builds.
registerControlRoutes(app);

// --- serve the built frontend (production) --------------------------------
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
