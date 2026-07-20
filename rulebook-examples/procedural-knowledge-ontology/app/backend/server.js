// ============================================================================
// PKO Procedure Register — API
//
// THE VIEW IS THE CONTRACT. Every read below is `SELECT ... FROM vw_<entity>`.
// There is no formula evaluator here, no lookup resolver, no "effective name"
// helper. The transpiler already emitted SQL for every calc/lookup/aggregation
// field in the rulebook; this server opens the door and walks in.
//
// If a view is missing or a query fails, we RAISE — a 500 with the real
// Postgres error. We never substitute a plausible-looking default, because a
// silent fallback would make a broken read look like an empty result set.
// ============================================================================
import express from "express";
import pg from "pg";
import path from "node:path";
import { readFile } from "node:fs/promises";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// The default IS the deterministically-correct target for this domain (it
// matches orchestrate.sh's erb_<domain-with-underscores> formula), not a guess.
// DATABASE_URL only overrides. See CLAUDE.md "Defaults derived from the SSoT".
const ERB_DOMAIN = process.env.ERB_DOMAIN || "procedural-knowledge-ontology";
const DATABASE_URL =
  process.env.DATABASE_URL ||
  `postgresql://postgres@localhost:5432/erb_${ERB_DOMAIN.replace(/-/g, "_")}`;

const PORT = Number(process.env.PORT || 8099);
const pool = new pg.Pool({ connectionString: DATABASE_URL });

const app = express();
app.use(express.json());

// --- one place that talks to Postgres --------------------------------------
// Deliberately thin: a view name and an optional WHERE. Anything that looks
// like it wants a JOIN here is a sign the rulebook should carry a lookup field
// instead — the view would then already have the column.
async function readView(view, { where, params = [], orderBy } = {}) {
  const sql =
    `SELECT * FROM ${view}` +
    (where ? ` WHERE ${where}` : "") +
    (orderBy ? ` ORDER BY ${orderBy}` : "");
  const { rows } = await pool.query(sql, params);
  return rows;
}

// Async handler wrapper: any rejection becomes a real 500 with the real error.
const h = (fn) => (req, res) =>
  fn(req, res).catch((err) => {
    console.error(`[api] ${req.method} ${req.path} failed:`, err.message);
    res.status(500).json({ error: err.message, detail: err.detail ?? null });
  });

// --- meta -------------------------------------------------------------------
app.get("/api/health", h(async (_req, res) => {
  const { rows } = await pool.query("SELECT current_database() AS db, now() AS at");
  res.json({ ok: true, db: rows[0].db, at: rows[0].at, domain: ERB_DOMAIN });
}));

// Project-level metadata (tagline, motif palette, PKO version IRIs, ...) lives
// in the rulebook's __meta__ table. rulebook-to-postgres does NOT emit a table
// or view for __meta__ — it is the metadata protocol itself, not domain data —
// so this is the one read that legitimately comes from the rulebook JSON rather
// than from a view. Everything else in this file reads vw_<entity>.
const RULEBOOK = path.join(
  __dirname,
  "../../effortless-rulebook/procedural-knowledge-ontology-rulebook.json"
);
app.get("/api/meta", h(async (_req, res) => {
  const rb = JSON.parse(await readFile(RULEBOOK, "utf8"));
  const rows = rb.__meta__?.data;
  if (!rows) {
    throw new Error(`No __meta__ table in ${RULEBOOK}`);
  }
  // Fold the typed-row hybrid back to an object, the same way
  // rulebookMeta.js/metaAsObject does on the platform side.
  const meta = {};
  for (const r of rows) {
    meta[r.MetaKey] =
      r.ValueType === "string" ? r.StringValue : JSON.parse(r.JsonValue ?? "null");
  }
  res.json(meta);
}));

// --- the generic table read -------------------------------------------------
// The frontend asks for whole views by name. The allowlist is READ FROM THE
// DATABASE at boot rather than hardcoded here: the transpiler decides which
// views exist, so a hand-maintained list would be a second source of truth that
// silently drifts every time the rulebook gains or loses a table.
//
// It doubles as a startup assertion. If the views are missing (nobody ran
// init-db.sh), we fail loudly at boot with the fix, instead of serving 404s
// that look like "this domain has no data".
let VIEWS = [];
async function loadViews() {
  const { rows } = await pool.query(
    `SELECT table_name FROM information_schema.views
      WHERE table_schema = 'public' AND table_name LIKE 'vw\\_%'
      ORDER BY table_name`
  );
  VIEWS = rows.map((r) => r.table_name.replace(/^vw_/, ""));
  if (VIEWS.length === 0) {
    throw new Error(
      `No vw_* views in ${DATABASE_URL}. The database exists but was never ` +
        `loaded. Fix:  ./postgres-bootstrap/init-db.sh`
    );
  }
  console.log(`[api] ${VIEWS.length} views available`);
}

app.get("/api/t/:table", h(async (req, res) => {
  const t = String(req.params.table).toLowerCase();
  if (!VIEWS.includes(t)) {
    return res.status(404).json({
      error: `Unknown view '${t}'. Known views: ${VIEWS.join(", ")}`,
    });
  }
  res.json(await readView(`vw_${t}`));
}));

// --- the whole register, in one shot ---------------------------------------
// The console is a read-mostly document viewer; one round trip beats forty.
// Every entry here is still a plain view read.
app.get("/api/register", h(async (_req, res) => {
  const out = {};
  await Promise.all(
    VIEWS.map(async (t) => {
      out[t] = await readView(`vw_${t}`);
    })
  );
  res.json(out);
}));

// --- the signature read: specification ↔ execution --------------------------
// The drift between what a procedure SAYS and what an execution DID is the
// whole point of PKO's spec/execution split. Both sides come straight from
// their views; the only thing this endpoint does is put them side by side.
app.get("/api/ledger/:executionId", h(async (req, res) => {
  const [execution] = await readView("vw_procedure_executions", {
    where: "procedure_execution_id = $1",
    params: [req.params.executionId],
  });
  if (!execution) {
    return res.status(404).json({
      error: `No ProcedureExecution '${req.params.executionId}'`,
    });
  }

  const specSteps = await readView("vw_steps", {
    where: "procedure_version = $1",
    params: [execution.procedure_version],
    orderBy: "step_number",
  });
  const stepExecutions = await readView("vw_step_executions", {
    where: "procedure_execution = $1",
    params: [req.params.executionId],
  });

  const byStep = new Map(stepExecutions.map((se) => [se.step, se]));
  res.json({
    execution,
    rows: specSteps.map((spec) => ({
      spec,
      execution: byStep.get(spec.step_id) ?? null, // null = specified, never performed
    })),
  });
}));

// ===========================================================================
// ADMIN — the witness layer, looking at itself
//
// Every read below is still a plain view read. The test outcomes, the fire
// counts, the loop rollups: all of it was computed by Postgres from the
// rulebook's own formulas. Nothing here re-derives a verdict in JavaScript.
// ===========================================================================

// --- the board -------------------------------------------------------------
app.get("/api/admin/board", h(async (_req, res) => {
  const suites = await readView("vw_test_suites", { orderBy: "test_suite_id" });
  const [totals] = await pool.query(`
    SELECT count(*)::int                                        AS total,
           count(*) FILTER (WHERE last_outcome = 'PASS')::int    AS pass,
           count(*) FILTER (WHERE last_outcome = 'WARN')::int    AS warn,
           count(*) FILTER (WHERE last_outcome = 'FAIL')::int    AS fail,
           count(*) FILTER (WHERE last_outcome = 'SKIP')::int    AS skip,
           count(*) FILTER (WHERE needs_attention)::int          AS blocking_fail,
           max(last_run_at)                                      AS last_run_at
    FROM vw_test_cases`).then((r) => r.rows);

  const byKind = (await pool.query(`
    SELECT test_kind,
           count(*)::int                                      AS total,
           count(*) FILTER (WHERE last_outcome = 'PASS')::int  AS pass,
           count(*) FILTER (WHERE last_outcome = 'WARN')::int  AS warn,
           count(*) FILTER (WHERE last_outcome = 'FAIL')::int  AS fail,
           count(*) FILTER (WHERE last_outcome = 'SKIP')::int  AS skip
    FROM vw_test_cases GROUP BY 1 ORDER BY 2 DESC`)).rows;

  // IsGreen is a computed column on the suite, not a JS opinion about one.
  res.json({ totals, byKind, suites, isGreen: totals.blocking_fail === 0 });
}));

// --- individual checks, filterable ----------------------------------------
app.get("/api/admin/tests", h(async (req, res) => {
  const { kind, outcome, suite, q } = req.query;
  const where = [];
  const params = [];
  for (const [col, val] of [["test_kind", kind], ["last_outcome", outcome], ["suite", suite]]) {
    if (val) { params.push(val); where.push(`${col} = $${params.length}`); }
  }
  if (q) {
    params.push(`%${String(q).toLowerCase()}%`);
    where.push(`(lower(subject) LIKE $${params.length} OR lower(test_case_id) LIKE $${params.length})`);
  }
  res.json(await readView("vw_test_cases", {
    where: where.length ? where.join(" AND ") : undefined,
    params,
    orderBy: "CASE last_outcome WHEN 'FAIL' THEN 0 WHEN 'WARN' THEN 1 WHEN 'SKIP' THEN 2 ELSE 3 END, test_case_id",
  }));
}));

// --- run the suite ---------------------------------------------------------
// Shells out to the same runner the CLI uses. One code path, so the browser
// and the terminal can never disagree about what the suite says.
app.post("/api/admin/tests/run", h(async (_req, res) => {
  const { execFile } = await import("node:child_process");
  const root = path.join(__dirname, "../..");
  const out = await new Promise((resolve) => {
    execFile("python3", ["tools/run_test_suite.py", "--write"],
      { cwd: root, maxBuffer: 32 * 1024 * 1024 },
      (err, stdout, stderr) => resolve({ code: err?.code ?? 0, stdout, stderr }));
  });
  res.json(out);
}));

// --- the witness board -----------------------------------------------------
// Every boolean witness with its live fire count, straight from the views.
app.get("/api/admin/witnesses", h(async (_req, res) => {
  const { rows: cols } = await pool.query(`
    SELECT c.table_name, c.column_name
    FROM information_schema.columns c
    WHERE c.table_schema = 'public' AND c.table_name LIKE 'vw\\_%'
      AND c.data_type = 'boolean'
    ORDER BY 1, 2`);

  // One pass over the substrate: count true/false per witness column.
  const parts = cols.map(({ table_name, column_name }) =>
    `SELECT '${table_name}' AS view, '${column_name}' AS col,
            count(*) FILTER (WHERE "${column_name}")::int AS t,
            count(*) FILTER (WHERE NOT "${column_name}")::int AS f,
            count(*)::int AS total
     FROM ${table_name}`);
  const { rows } = await pool.query(parts.join(" UNION ALL "));

  // Attach provenance: which question invented this field, if any.
  const { rows: prov } = await pool.query(`
    SELECT target_table, field_name, invented_for_question, is_witness
    FROM vw_rulebook_fields WHERE datatype = 'boolean'`);
  const key = (t, f) => `${t}|${f}`.toLowerCase().replace(/_/g, "");
  const byField = new Map(prov.map((p) => [key(p.target_table, p.field_name), p]));

  res.json(rows.map((r) => {
    const p = byField.get(key(r.view.replace(/^vw_/, ""), r.col));
    const verdict = r.total === 0 ? "empty"
      : r.t === 0 ? "never true"
      : r.f === 0 ? "always true"
      : "discriminates";
    return { ...r, table: r.view.replace(/^vw_/, ""), verdict,
             question: p?.invented_for_question ?? null,
             isWitness: p?.is_witness ?? false };
  }));
}));

// --- loops, questions, and their materialized answers ----------------------
app.get("/api/admin/loops", h(async (_req, res) => {
  const [loops, questions, roles] = await Promise.all([
    readView("vw_witness_loops", { orderBy: "loop_number" }),
    readView("vw_role_questions", { orderBy: "witness_loop, asking_role" }),
    readView("vw_roles", { orderBy: "role_id" }),
  ]);
  const { rows: counts } = await pool.query(
    `SELECT invented_for_question AS q, count(*)::int AS n
     FROM vw_rulebook_fields WHERE invented_for_question IS NOT NULL GROUP BY 1`);
  const byQ = new Map(counts.map((c) => [c.q, c.n]));
  res.json({
    loops,
    roles,
    questions: questions.map((q) => ({ ...q, predicateCount: byQ.get(q.role_question_id) ?? 0 })),
  });
}));

// --- provenance: trace any field back to the question that motivated it ----
app.get("/api/admin/provenance/:table/:field", h(async (req, res) => {
  const { table, field } = req.params;
  const [row] = await readView("vw_rulebook_fields", {
    where: "lower(target_table) = lower($1) AND lower(field_name) = lower($2)",
    params: [table, field],
  });
  if (!row) {
    return res.status(404).json({ error: `No catalog entry for ${table}.${field}` });
  }

  let question = null, loop = null, role = null, siblings = [];
  if (row.invented_for_question) {
    [question] = await readView("vw_role_questions", {
      where: "role_question_id = $1", params: [row.invented_for_question],
    });
    if (question) {
      [loop] = await readView("vw_witness_loops", {
        where: "witness_loop_id = $1", params: [question.witness_loop],
      });
      [role] = await readView("vw_roles", {
        where: "role_id = $1", params: [question.asking_role],
      });
      siblings = await readView("vw_rulebook_fields", {
        where: "invented_for_question = $1", params: [row.invented_for_question],
        orderBy: "target_table, field_name",
      });
    }
  }

  // What does the field currently READ? The whole point of provenance is to
  // end at a value, not at a description of one.
  let reading = null;
  if (row.is_derived) {
    const view = `vw_${String(row.target_table).replace(/([a-z0-9])([A-Z])/g, "$1_$2").toLowerCase()}`;
    const col = String(row.field_name).replace(/([a-z0-9])([A-Z])/g, "$1_$2").toLowerCase();
    const { rows: exists } = await pool.query(
      `SELECT 1 FROM information_schema.columns
       WHERE table_schema='public' AND table_name=$1 AND column_name=$2`, [view, col]);
    if (exists.length) {
      const { rows } = await pool.query(
        `SELECT count(*) FILTER (WHERE "${col}" IS NOT NULL)::int AS populated,
                count(*)::int AS total FROM ${view}`);
      reading = rows[0];
    }
  }

  res.json({ field: row, question, loop, role, siblings, reading });
}));

// --- static frontend (prod mode) -------------------------------------------
const dist = path.join(__dirname, "../frontend/dist");
app.use(express.static(dist));
app.get(/^(?!\/api\/).*/, (_req, res) => res.sendFile(path.join(dist, "index.html")));

// Boot only after the views are confirmed present — see loadViews().
loadViews()
  .then(() => {
    app.listen(PORT, () => {
      console.log(`[api] PKO Procedure Register on http://localhost:${PORT}`);
      console.log(`[api] reading views from ${DATABASE_URL}`);
    });
  })
  .catch((err) => {
    console.error(`[api] FATAL: ${err.message}`);
    process.exit(1);
  });
