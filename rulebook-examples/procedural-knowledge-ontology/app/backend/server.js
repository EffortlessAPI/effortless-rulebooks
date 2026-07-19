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
