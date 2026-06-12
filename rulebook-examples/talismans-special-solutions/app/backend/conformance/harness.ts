// ===========================================================================
// conformance/harness.ts — the engine-agnostic conformance test runner.
//
// WHAT THIS IS
//   A test client that flexes every element of the NTWF model against EVERY
//   execution substrate, with NO knowledge of how any substrate works. It calls
//   the same `reason(rows, engine)` interface the read handlers use — so it is
//   not OWL-specific and not Postgres-specific. Adding a third engine to
//   dal/index.ts's RULES map would make every test run against it for free.
//
// WHERE THE TESTS COME FROM
//   The rulebook's first-class `ConformanceTests` table (one row per assertion).
//   The harness reads that table like any other rulebook table — it invents no
//   tests of its own. Each row's TestKind selects an executor below.
//
// WHERE THE EXPECTED ANSWERS COME FROM (doctrine: never compute in the harness)
//   - sweep / field-match: the ORACLE is testing/answer-keys/*.json, which are
//     the Postgres-computed view rows written back into the rulebook. The
//     harness reads the key; it does not re-derive INDEX/MATCH / COUNTIFS /
//     closure. (CLAUDE.md: "Never compute answers in Python" — same rule here.)
//   - closure-contains: the ORACLE is the engine's own competency closure
//     payload (the reasoner's recursive view / the SQL WITH RECURSIVE view).
//   - engines-agree: the ORACLE is the cross-engine diff (diffPayloads).
//   - mutation: the ORACLE is the engine itself, re-reasoned after an in-memory
//     edit. We assert the NEW computed value flipped as the rule dictates; we
//     never hardcode what the post-edit value "should" be beyond the boolean the
//     business rule promises.
//
// ISOLATION
//   Mutations clone the seed store in memory and run the engine over the clone.
//   No store (db.json, Postgres tables) is ever written. A mutation test leaves
//   zero residue.
// ===========================================================================
import { readFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { reason, RULES } from "../dal/index";
import { seedStoreFromRulebook } from "../dal/export-to-rulebook";
import { diffPayloads, classify } from "../dal/conformance";
import { loadSchemaMap, type SchemaMap, type AuthoredColumns, pascalToSnake } from "../dal/schema-map";
import { authoredColumns } from "../dal/postgres";
import type { ReasonedWorld, RawDb } from "../dal/reasoner";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const PROJECT_ROOT = path.join(__dirname, "..", "..", "..");
const RULEBOOK_PATH =
  process.env.ERB_RULEBOOK_PATH ||
  path.join(PROJECT_ROOT, "effortless-rulebook", "talismans-special-solutions-rulebook.json");
const ANSWER_KEYS_DIR = path.join(PROJECT_ROOT, "testing", "answer-keys");

// The engines to run a test against. The list comes from dal/index.ts's RULES
// map — the SAME registry the app uses. We never name "owl" / "postgres"
// literally in test logic; we iterate whatever engines exist.
export const ENGINE_IDS = Object.keys(RULES);

// ---- the rulebook shapes we read ------------------------------------------
interface ConformanceTestRow {
  conformanceTestId: string;
  displayName: string;
  featureRef: string;
  section: string;
  testKind: string;
  targetRef: string;
  expect: string;
  explanation: string;
  sortOrder: number;
  isEnabled: boolean;
}

// ---- the result shapes the API / CLI / UI consume -------------------------
// One engine's verdict on one test.
export interface EngineResult {
  engine: string;
  // pass  — the assertion held on this engine
  // fail  — a real disagreement with the oracle (the one that matters)
  // gap   — the engine did not emit the value at all (a coverage gap, not a
  //         wrong answer — surfaced, never hidden; see the known lookup-column
  //         gap on ApprovalGates.GateRole/GateApproverHuman)
  // error — the test could not run (bad ref, engine threw)
  status: "pass" | "fail" | "gap" | "error";
  detail: string;
  expected?: unknown;
  actual?: unknown;
}

// The full verdict for one test across all engines.
export interface TestResult {
  id: string;
  displayName: string;
  featureRef: string;
  section: string;
  testKind: string;
  targetRef: string;
  explanation: string;
  enabled: boolean;
  engines: EngineResult[];
  // overall = worst across engines (error > fail > gap > pass); "skipped" if
  // disabled. The UI colors the row by this.
  overall: "pass" | "fail" | "gap" | "error" | "skipped";
}

export interface RunSummary {
  startedAt: string;
  finishedAt: string;
  durationMs: number;
  engines: string[];
  total: number;
  passed: number;
  failed: number;
  gaps: number;
  errors: number;
  skipped: number;
  // the strong claim: every enabled test passed on every engine
  allGreen: boolean;
}

export interface RunReport {
  summary: RunSummary;
  results: TestResult[];
}

// ---- helpers ---------------------------------------------------------------

// Parse "Entity", "Entity/pk", or "Entity/pk#Field" into parts.
function parseTargetRef(ref: string): { entity: string; pk: string | null; field: string | null } {
  const hashIdx = ref.indexOf("#");
  const field = hashIdx >= 0 ? ref.slice(hashIdx + 1) : null;
  const beforeHash = hashIdx >= 0 ? ref.slice(0, hashIdx) : ref;
  const slashIdx = beforeHash.indexOf("/");
  const entity = slashIdx >= 0 ? beforeHash.slice(0, slashIdx) : beforeHash;
  const pk = slashIdx >= 0 ? beforeHash.slice(slashIdx + 1) : null;
  return { entity, pk, field };
}

// PascalCase entity -> snake answer-key file name (matches keygen output).
function answerKeyFile(entity: string): string {
  return path.join(ANSWER_KEYS_DIR, pascalToSnake(entity) + ".json");
}

// Index an engine's individuals for one class by primary-key VALUE.
function indexIndividuals(
  world: ReasonedWorld,
  entity: string,
  pkCamel: string
): Map<string, Record<string, unknown>> {
  const rows = (world.individuals[entity] as Record<string, unknown>[] | undefined) || [];
  const m = new Map<string, Record<string, unknown>>();
  for (const r of rows) m.set(String(r[pkCamel]), r);
  return m;
}

// Compare one field's engine value to the answer-key value, using the same
// classify() the live conformance differential uses — so "41" vs 41, an ISO
// date vs the reasoner's spelling, and a superset relationship all count as a
// match, while a flipped boolean or a different count is a real fail. An engine
// value that is null/absent where the key HAS a value is a `gap` (coverage),
// not a `fail` (wrong answer) — we keep that distinction visible.
function compareField(
  engineVal: unknown,
  keyVal: unknown,
  ftype: string
): { status: "pass" | "fail" | "gap"; detail: string } {
  const kind = classify(engineVal, keyVal, ftype);
  if (kind === "equal" || kind === "representation") {
    return { status: "pass", detail: kind === "representation" ? "match (representation)" : "match" };
  }
  if (kind === "presence") {
    const engineEmpty = engineVal == null || engineVal === "";
    // present-in-key, absent-in-engine = the engine didn't emit it = gap.
    // present-in-engine, absent-in-key would be the key being stale — treat as
    // gap too, but say which side is empty.
    return {
      status: "gap",
      detail: engineEmpty ? "engine emitted no value (key has one)" : "key has no value (engine emitted one)",
    };
  }
  return { status: "fail", detail: `value differs (engine=${JSON.stringify(engineVal)} key=${JSON.stringify(keyVal)})` };
}

// ---- the harness ----------------------------------------------------------
export class ConformanceHarness {
  private schemaMap!: SchemaMap;
  private seedStore!: RawDb;
  private tests!: ConformanceTestRow[];
  // answer-key cache: entity -> (pkValue -> snake-keyed row)
  private keyCache = new Map<string, Map<string, Record<string, unknown>>>();
  // per-engine reasoned world over the SEED store (computed once, reused by all
  // sweep/field-match/closure tests). Mutations get their own ephemeral world.
  private seedWorlds = new Map<string, ReasonedWorld>();

  // Load the rulebook (schema-map + ConformanceTests rows + seed store).
  //
  // CRITICAL: we seed the SAME way the running app seeds db.json — using the live
  // base-table columns as the raw/derived authority (authoredColumns()), NOT the
  // rulebook type alone. The `null` authority drops relationship fields whose
  // prose marks them inverse (`workflow`, `approvalGate`, `precedes`, …); without
  // those FK columns the aggregations have no children to count and every rollup
  // reads 0/false — a false RED that's a harness artifact, not an engine bug.
  // The app avoids this exact trap via seed-dbjson.sh (which uses authoredColumns);
  // the harness must match it to test what the app actually runs.
  //
  // If the DB is unreachable we DON'T silently fall back to the lossy `null` path
  // (that would mask the very facts we test). We surface the failure — the
  // Postgres engine needs the DB up anyway, so a missing DB is a real blocker.
  async load(): Promise<void> {
    let authored: AuthoredColumns | null = null;
    try {
      authored = await authoredColumns();
    } catch (err) {
      throw new Error(
        `conformance harness: could not read the live base-table columns needed to seed the ` +
        `store faithfully (${(err as Error).message}). Run init-db.sh (effortless build) so the ` +
        `Postgres base tables exist; the harness seeds exactly as the app does and refuses the ` +
        `lossy rulebook-type-only path that would silently drop FK columns.`
      );
    }
    this.schemaMap = await loadSchemaMap(RULEBOOK_PATH, authored);
    this.seedStore = await seedStoreFromRulebook(RULEBOOK_PATH, authored);
    const rb = JSON.parse(await readFile(RULEBOOK_PATH, "utf8")) as Record<string, { data?: Record<string, unknown>[] }>;
    // rulebook data is PascalCase-keyed; project the fields we use to camel.
    this.tests = (rb.ConformanceTests?.data || []).map((r: Record<string, unknown>) => ({
      conformanceTestId: r.ConformanceTestId as string,
      displayName: r.DisplayName as string,
      featureRef: (r.FeatureRef as string) || "",
      section: r.Section as string,
      testKind: r.TestKind as string,
      targetRef: (r.TargetRef as string) || "",
      expect: (r.Expect as string) || "",
      explanation: (r.Explanation as string) || "",
      sortOrder: (r.SortOrder as number) ?? 0,
      isEnabled: r.IsEnabled !== false,
    }));
    this.tests.sort((a, b) => a.sortOrder - b.sortOrder);
  }

  // Just the metadata, for the admin list view (no execution).
  listTests(): Omit<TestResult, "engines" | "overall">[] {
    return this.tests.map((t) => ({
      id: t.conformanceTestId,
      displayName: t.displayName,
      featureRef: t.featureRef,
      section: t.section,
      testKind: t.testKind,
      targetRef: t.targetRef,
      explanation: t.explanation,
      enabled: t.isEnabled,
    }));
  }

  // Reason the seed store through one engine, memoized.
  private async seedWorld(engine: string): Promise<ReasonedWorld> {
    let w = this.seedWorlds.get(engine);
    if (!w) {
      w = await reason(this.seedStore, engine);
      this.seedWorlds.set(engine, w);
    }
    return w;
  }

  // Load + cache an answer-key file, indexed by pk VALUE.
  private async answerKey(entity: string): Promise<Map<string, Record<string, unknown>>> {
    let m = this.keyCache.get(entity);
    if (m) return m;
    const pkSnake = this.schemaMap[entity]?.pk.snake;
    if (!pkSnake) throw new Error(`no schema-map entry for entity '${entity}'`);
    const rows = JSON.parse(await readFile(answerKeyFile(entity), "utf8")) as Record<string, unknown>[];
    m = new Map();
    for (const r of rows) m.set(String(r[pkSnake]), r);
    this.keyCache.set(entity, m);
    return m;
  }

  // ---- executors (one per TestKind) ---------------------------------------

  // SWEEP: every row, every field of `entity` vs the answer key, on one engine.
  private async runSweep(entity: string, engine: string): Promise<EngineResult> {
    const tm = this.schemaMap[entity];
    if (!tm) return { engine, status: "error", detail: `unknown entity '${entity}'` };
    const world = await this.seedWorld(engine);
    const key = await this.answerKey(entity);
    const eng = indexIndividuals(world, entity, tm.pk.camel);

    let compared = 0, fails = 0, gaps = 0;
    const problems: string[] = [];
    for (const [pkVal, keyRow] of key) {
      const engRow = eng.get(pkVal);
      if (!engRow) { gaps++; problems.push(`${pkVal}: row absent in engine`); continue; }
      for (const f of tm.allFields) {
        if (key.size && !(f.snake in keyRow)) continue; // key didn't carry this column
        compared++;
        const r = compareField(engRow[f.camel], keyRow[f.snake], f.type);
        if (r.status === "fail") { fails++; if (problems.length < 8) problems.push(`${pkVal}.${f.camel}: ${r.detail}`); }
        else if (r.status === "gap") { gaps++; if (problems.length < 8) problems.push(`${pkVal}.${f.camel}: ${r.detail}`); }
      }
    }
    const status = fails > 0 ? "fail" : gaps > 0 ? "gap" : "pass";
    const detail =
      status === "pass"
        ? `${compared} fields across ${key.size} rows match`
        : `${fails} fail / ${gaps} gap of ${compared} fields — ${problems.slice(0, 8).join("; ")}`;
    return { engine, status, detail };
  }

  // FIELD-MATCH: one row's one field vs the answer key, on one engine.
  private async runFieldMatch(targetRef: string, engine: string): Promise<EngineResult> {
    const { entity, pk, field } = parseTargetRef(targetRef);
    const tm = this.schemaMap[entity];
    if (!tm || !pk || !field) return { engine, status: "error", detail: `bad TargetRef '${targetRef}'` };
    const fieldDef = tm.allFields.find((f) => f.pascal === field);
    if (!fieldDef) return { engine, status: "error", detail: `no field '${field}' on ${entity}` };

    const world = await this.seedWorld(engine);
    const engRow = indexIndividuals(world, entity, tm.pk.camel).get(pk);
    if (!engRow) return { engine, status: "gap", detail: `row ${entity}/${pk} absent in engine` };
    const keyRow = (await this.answerKey(entity)).get(pk);
    if (!keyRow) return { engine, status: "error", detail: `row ${entity}/${pk} absent in answer key` };

    const r = compareField(engRow[fieldDef.camel], keyRow[fieldDef.snake], fieldDef.type);
    return {
      engine,
      status: r.status,
      detail: r.detail,
      expected: keyRow[fieldDef.snake],
      actual: engRow[fieldDef.camel],
    };
  }

  // CLOSURE-CONTAINS: the (from→to) pair must appear in the engine's computed
  // transitive closure. The engine is the oracle (its recursive view).
  private async runClosureContains(expectJson: string, engine: string): Promise<EngineResult> {
    let spec: { closure: string; from: string; to: string };
    try { spec = JSON.parse(expectJson); } catch { return { engine, status: "error", detail: "bad Expect JSON" }; }
    const world = await this.seedWorld(engine);
    const comp = world.competency as Record<string, unknown>;

    if (spec.closure === "precedence") {
      const pc = comp.precedence_closure as { pairs?: Array<{ from_id: string; to_id: string; is_inferred?: boolean }> };
      const pairs = pc?.pairs || [];
      const hit = pairs.find((p) => p.from_id === spec.from && p.to_id === spec.to);
      if (!hit) return { engine, status: "fail", detail: `pair ${spec.from}→${spec.to} not in precedence closure (${pairs.length} pairs)` };
      return { engine, status: "pass", detail: hit.is_inferred ? "present (inferred ✓)" : "present (asserted)" };
    }
    if (spec.closure === "delegation") {
      // delegation comes as { fromRoleId: [toRoleId, ...] } DIRECT edges; the
      // engine pre-expands transitively in the roles closure. Walk it to confirm
      // reachability (this is reading the engine's edges, not re-deriving the rule).
      const deleg = (comp.delegation as Record<string, string[]>) || {};
      const reach = new Set<string>();
      const stack = [...(deleg[spec.from] || [])];
      while (stack.length) {
        const n = stack.pop()!;
        if (reach.has(n)) continue;
        reach.add(n);
        for (const nxt of deleg[n] || []) stack.push(nxt);
      }
      if (!reach.has(spec.to)) return { engine, status: "fail", detail: `${spec.to} not reachable from ${spec.from} via delegatesTo` };
      const direct = (deleg[spec.from] || []).includes(spec.to);
      return { engine, status: "pass", detail: direct ? "present (asserted)" : "present (inferred ✓)" };
    }
    return { engine, status: "error", detail: `unknown closure '${spec.closure}'` };
  }

  // MUTATION: clone the seed store, apply the edits in memory, re-reason on this
  // engine, assert each post-edit value. The store is never written.
  private async runMutation(expectJson: string, engine: string): Promise<EngineResult> {
    let spec: {
      edits: Array<{ class: string; id: string; set: Record<string, unknown> }>;
      assert: Array<{ class: string; id: string; field: string; equals: unknown }>;
    };
    try { spec = JSON.parse(expectJson); } catch { return { engine, status: "error", detail: "bad Expect JSON" }; }

    // deep clone the seed store (structuredClone is available in the node runtime)
    const clone: RawDb = structuredClone(this.seedStore);
    for (const e of spec.edits) {
      const rows = clone[e.class];
      if (!rows) return { engine, status: "error", detail: `mutation class '${e.class}' not in store` };
      const pkCamel = this.schemaMap[e.class]?.pk.camel;
      const row = rows.find((r) => String(r[pkCamel]) === e.id);
      if (!row) return { engine, status: "error", detail: `mutation row ${e.class}/${e.id} not found` };
      Object.assign(row, e.set);
    }

    let world: ReasonedWorld;
    try { world = await reason(clone, engine); }
    catch (err) { return { engine, status: "error", detail: `engine threw after mutation: ${(err as Error).message}` }; }

    const fails: string[] = [];
    for (const a of spec.assert) {
      const tm = this.schemaMap[a.class];
      const row = indexIndividuals(world, a.class, tm.pk.camel).get(a.id);
      if (!row) { fails.push(`${a.class}/${a.id}: row absent post-mutation`); continue; }
      const got = row[a.field];
      // booleans may come back as true / "true" / "t" depending on engine — compare loosely on the boolean meaning
      const norm = (v: unknown) => (typeof v === "boolean" ? v : v === "true" || v === "t" ? true : v === "false" || v === "f" ? false : v);
      if (norm(got) !== a.equals) fails.push(`${a.class}/${a.id}.${a.field}: expected ${JSON.stringify(a.equals)}, got ${JSON.stringify(got)}`);
    }
    if (fails.length) return { engine, status: "fail", detail: fails.join("; ") };
    return { engine, status: "pass", detail: `${spec.assert.length} assertion(s) held after in-memory edit` };
  }

  // ENGINES-AGREE: run the live cross-engine differential; pass iff zero value
  // diffs. This one test isn't per-engine (it IS the comparison), so we report it
  // under a synthetic "both" engine row.
  private async runEnginesAgree(): Promise<EngineResult[]> {
    // need at least two engines to compare
    if (ENGINE_IDS.length < 2) return [{ engine: "both", status: "gap", detail: "fewer than two engines registered" }];
    const worlds = await Promise.all(ENGINE_IDS.slice(0, 2).map((e) => this.seedWorld(e)));
    const pkByClass: Record<string, string> = {};
    const fieldTypeByClass: Record<string, Record<string, string>> = {};
    for (const [cls, t] of Object.entries(this.schemaMap)) {
      pkByClass[cls] = t.pk.camel;
      fieldTypeByClass[cls] = {};
      for (const f of t.allFields) fieldTypeByClass[cls][f.camel] = f.type;
    }
    const diff = diffPayloads(worlds[0], worlds[1], pkByClass, fieldTypeByClass);
    const s = diff.summary;
    if (s.agree) {
      return [{ engine: "both", status: "pass", detail: `${s.identical}/${s.fieldsCompared} identical, ${s.representationDiffs} repr, ${s.presenceDiffs} presence, 0 value diffs` }];
    }
    const valueDiffs = diff.diffs.filter((d) => d.kind === "value").slice(0, 6);
    return [{
      engine: "both",
      status: "fail",
      detail: `${s.valueDiffs} value diff(s): ` + valueDiffs.map((d) => `${d.class}/${d.pk}.${d.field}`).join(", "),
    }];
  }

  // Run ONE test across all engines and fold into a TestResult.
  private async runOne(t: ConformanceTestRow): Promise<TestResult> {
    const base = {
      id: t.conformanceTestId, displayName: t.displayName, featureRef: t.featureRef,
      section: t.section, testKind: t.testKind, targetRef: t.targetRef,
      explanation: t.explanation, enabled: t.isEnabled,
    };
    if (!t.isEnabled) return { ...base, engines: [], overall: "skipped" };

    let engines: EngineResult[];
    try {
      if (t.testKind === "engines-agree") {
        engines = await this.runEnginesAgree();
      } else {
        engines = await Promise.all(
          ENGINE_IDS.map(async (e) => {
            switch (t.testKind) {
              case "sweep": return this.runSweep(t.targetRef, e);
              case "field-match": return this.runFieldMatch(t.targetRef, e);
              case "closure-contains": return this.runClosureContains(t.expect, e);
              case "mutation": return this.runMutation(t.expect, e);
              default: return { engine: e, status: "error", detail: `unknown TestKind '${t.testKind}'` } as EngineResult;
            }
          })
        );
      }
    } catch (err) {
      engines = ENGINE_IDS.map((e) => ({ engine: e, status: "error" as const, detail: (err as Error).message }));
    }

    // overall = worst across engines
    const rank = { error: 3, fail: 2, gap: 1, pass: 0 } as const;
    let worst: EngineResult["status"] = "pass";
    for (const er of engines) if (rank[er.status] > rank[worst]) worst = er.status;
    return { ...base, engines, overall: worst };
  }

  // Run the whole suite (optionally filtered to a subset of ids).
  async run(opts: { only?: string[] } = {}): Promise<RunReport> {
    const startedAt = new Date().toISOString();
    const t0 = performance.now();
    const toRun = opts.only ? this.tests.filter((t) => opts.only!.includes(t.conformanceTestId)) : this.tests;
    const results: TestResult[] = [];
    for (const t of toRun) results.push(await this.runOne(t)); // serial: engines already parallel within a test
    const t1 = performance.now();
    const finishedAt = new Date().toISOString();

    const passed = results.filter((r) => r.overall === "pass").length;
    const failed = results.filter((r) => r.overall === "fail").length;
    const gaps = results.filter((r) => r.overall === "gap").length;
    const errors = results.filter((r) => r.overall === "error").length;
    const skipped = results.filter((r) => r.overall === "skipped").length;

    return {
      summary: {
        startedAt, finishedAt, durationMs: Math.round(t1 - t0),
        engines: ENGINE_IDS, total: results.length,
        passed, failed, gaps, errors, skipped,
        allGreen: failed === 0 && errors === 0,
      },
      results,
    };
  }
}
