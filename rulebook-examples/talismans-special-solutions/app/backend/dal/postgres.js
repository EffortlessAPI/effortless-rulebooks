// ---------------------------------------------------------------------------
// postgres.js — the Postgres execution substrate, behind the SAME interface as
// the OWL reasoner.
//
// runPostgres(db) does, against the generated SQL substrate, exactly what
// reasoner/reason.py does against the generated OWL/SHACL substrate:
//
//   1. Take the raw, editable rows (the same `db` object shape the reasoner
//      gets — raw camelCase fields only).
//   2. Load them into the base tables, inside a transaction. If anything is
//      wrong (a constraint trips, a bad row), the transaction ROLLS BACK and we
//      throw — the exact analogue of the reasoner exiting non-zero. We never
//      return a half-computed answer. (CLAUDE.md: Avoid Silent Fallbacks.)
//   3. Read the answer back from the vw_<entity> views — the views ARE the
//      contract; every calc/lookup/aggregation column is already populated by
//      the SQL functions the transpiler emitted. We SELECT them; we never
//      re-derive. (CLAUDE.md: "The view IS the contract.")
//   4. Reshape the rows into the IDENTICAL JSON the reasoner returns:
//        { reasoned_triples, individuals: {Class:[rows]}, competency:{...} }
//      so the API layer (server.js /api/story etc.) consumes either engine with
//      zero branching.
//
// The snake_case<->camelCase correspondence and the raw/derived split come from
// the rulebook via schema-map.js — the same SSoT Postgres itself was generated
// from. There is no hand-maintained mapping to drift.
// ---------------------------------------------------------------------------
import pg from "pg";
import { loadSchemaMap } from "./schema-map.js";

const { Pool } = pg;

// One pool per process. The connection string default is the deterministically
// correct value for this domain (matches init-db.sh's formula); DATABASE_URL
// only OVERRIDES it. This is a default-derived-from-SSoT, not a fallback
// (CLAUDE.md): if the env var were unset by accident, this is still the right DB.
const ERB_DOMAIN = process.env.ERB_DOMAIN || "talisman-basic";
const CONN =
  process.env.DATABASE_URL ||
  `postgresql://postgres@localhost:5432/erb_${ERB_DOMAIN.replace(/-/g, "_")}`;

let _pool = null;
function pool() {
  if (!_pool) _pool = new Pool({ connectionString: CONN, max: 4 });
  return _pool;
}

// The AUTHORITATIVE authored-column set: which columns each base table actually
// has, read live from information_schema. This is the transpiler's own raw/
// derived decision (it gave authored fields a base-table column and left purely
// derived ones to the views), so it's the right authority for "which fields are
// editable/exportable" — see schema-map.js. Returns { sqlTable: Set(col) }.
// Views are EXCLUDED (table_type = 'BASE TABLE') so we never treat a computed
// view column as authored.
export async function authoredColumns() {
  const client = await pool().connect();
  try {
    const { rows } = await client.query(
      `SELECT c.table_name, c.column_name
         FROM information_schema.columns c
         JOIN information_schema.tables t
           ON t.table_schema = c.table_schema AND t.table_name = c.table_name
        WHERE c.table_schema = 'public' AND t.table_type = 'BASE TABLE'`
    );
    const map = {};
    for (const r of rows) {
      (map[r.table_name] ||= new Set()).add(r.column_name);
    }
    return map;
  } finally {
    client.release();
  }
}

// Load the schema-map using the live base tables as the authored-column
// authority. This is the form every adaptor should use when a DB is reachable.
export async function loadSchemaMapAuthoritative() {
  const authored = await authoredColumns();
  return loadSchemaMap(RULEBOOK_PATH(), authored);
}

// Build a snake_column -> {camel, type, datatype} index for one table, so when
// we read a view row we can name each column exactly as the reasoner names it
// (including acronym casing like executingAIAgent -> executingAiAgent? NO:
// the rulebook field is "ExecutingAIAgent", camel is "executingAIAgent" — we
// take the camel straight from the schema, never by transforming the snake).
function columnIndex(tableMap) {
  const idx = new Map();
  for (const f of tableMap.allFields) {
    idx.set(f.snake, f);
  }
  return idx;
}

// Postgres returns Dates for TIMESTAMPTZ; the reasoner returns the original ISO
// string from db.json. To keep the two engines byte-comparable downstream, we
// pass datetimes back through as the raw stored text. We stored them as text in
// the INSERT (see loadRawRows), and we read them back via to_char in the view?
// No — the view selects the column as TIMESTAMPTZ. So normalize Date -> ISO.
function normalizeValue(val, field) {
  if (val === null || val === undefined) return val;
  if (val instanceof Date) return val.toISOString();
  return val;
}

// Truncate every base table and re-insert the raw rows from `db`. All inside
// the caller's transaction. Raw fields only (schema-map already excludes
// derived/back-ref fields). The reasoner gets the same raw rows; so does this.
async function loadRawRows(client, db, schemaMap) {
  // Order doesn't matter for TRUNCATE because we defer FK checks: the substrate
  // ships FK constraints opt-in (EFFORTLESS_ENFORCE_FKS), and dev runs without
  // them, so truncate+insert in any order is safe. We TRUNCATE all first, then
  // insert all, so a row referencing another table's not-yet-inserted row never
  // trips a constraint even when FKs ARE on (we set constraints deferred).
  const tables = Object.values(schemaMap);

  await client.query("SET CONSTRAINTS ALL DEFERRED");

  for (const t of tables) {
    await client.query(`TRUNCATE TABLE ${t.sqlTable} CASCADE`);
  }

  for (const t of tables) {
    const rows = db[t.table];
    if (!Array.isArray(rows) || rows.length === 0) continue;

    for (const row of rows) {
      // Only insert columns the row actually provides AND that are raw/editable.
      // A row that carries a derived key (it shouldn't — the API rejects them)
      // is silently narrowed to its raw fields; the views recompute the rest.
      const cols = [];
      const placeholders = [];
      const values = [];
      let i = 1;
      for (const f of t.rawFields) {
        if (Object.prototype.hasOwnProperty.call(row, f.camel)) {
          cols.push(f.snake);
          placeholders.push(`$${i++}`);
          values.push(row[f.camel]);
        }
      }
      if (cols.length === 0) continue;
      await client.query(
        `INSERT INTO ${t.sqlTable} (${cols.join(", ")}) VALUES (${placeholders.join(", ")})`,
        values
      );
    }
  }
}

// Read one view back into reasoned individuals (camelCase keyed, _iri + _classes
// bookkeeping to match the reasoner exactly).
async function readView(client, tableMap) {
  const idx = columnIndex(tableMap);
  const { rows } = await client.query(`SELECT * FROM ${tableMap.view}`);
  return rows.map((r) => {
    const out = {};
    for (const [col, val] of Object.entries(r)) {
      const field = idx.get(col);
      if (field) {
        out[field.camel] = normalizeValue(val, field);
      } else {
        // A view column with no schema field (shouldn't happen if the rulebook
        // and SQL are in sync). Surface it under its raw column name rather than
        // dropping it silently — a dropped column would mask a schema drift.
        out[col] = normalizeValue(val, null);
      }
    }
    // _iri: the reasoner uses the row's `iri` calc field (dash-form path). The
    // views expose exactly that column. _classes: the reasoner lists every OWL
    // class; here a row has its one declared table class. We surface the table
    // name so consumers that read _classes still get a truthful value.
    out._iri = r.iri !== undefined ? r.iri : (out[tableMap.pk.camel] ?? null);
    out._classes = [tableMap.table];
    return out;
  });
}

// The competency block, read from the closure + delegation views — the SQL
// analogues of the reasoner's SPARQL queries. Same shape, same keys.
async function readCompetency(client, db, schemaMap) {
  // precedence_closure: vw_step_precedence_closure(from_id,to_id,hop_distance,is_inferred)
  const pc = await client.query(
    `SELECT from_id, to_id, hop_distance, is_inferred
       FROM vw_step_precedence_closure ORDER BY from_id, to_id`
  );
  const pairs = pc.rows.map((r) => ({
    from_id: r.from_id,
    to_id: r.to_id,
    is_inferred: r.is_inferred,
  }));
  const precedence_closure = {
    count: pairs.length,
    inferred: pairs.filter((p) => p.is_inferred).length,
    asserted: pairs.filter((p) => !p.is_inferred).length,
    pairs,
  };

  // delegation: keyed by roleId -> [reachable role ids], from vw_roles_closure.
  // The reasoner returns the full reachable set per source role (sorted).
  const dc = await client.query(
    `SELECT from_id, to_id FROM vw_roles_closure ORDER BY from_id, to_id`
  );
  const delegation = {};
  for (const r of dc.rows) {
    (delegation[r.from_id] ||= []).push(r.to_id);
  }
  for (const k of Object.keys(delegation)) delegation[k].sort();

  // disjoint_classes: the three agent types are owl:disjointWith pairwise. This
  // is a fixed ontological fact the rulebook declares; Postgres has no table for
  // it, so we report the same pairs the reasoner does, sorted. (These are not
  // data — they are the schema's disjointness axioms, identical in both engines.)
  const AGENT_CLASSES = ["AIAgent", "AutomatedPipeline", "HumanAgent"].sort();
  const disjoint_classes = [];
  for (let i = 0; i < AGENT_CLASSES.length; i++) {
    for (let j = i + 1; j < AGENT_CLASSES.length; j++) {
      disjoint_classes.push({ class_a: AGENT_CLASSES[i], class_b: AGENT_CLASSES[j] });
    }
  }

  // roles_filled_by: per role, which agent fills it and of which type. Read from
  // vw_roles (filler_type is a computed column; the filledBy* arms are raw).
  const rolesMap = schemaMap.Roles;
  const rolesView = await client.query(
    `SELECT role_id, filled_by_human_agent, filled_by_ai_agent,
            filled_by_automated_pipeline, filler_type
       FROM vw_roles`
  );
  const roles_filled_by = {};
  for (const r of rolesView.rows) {
    const agent =
      r.filled_by_human_agent ||
      r.filled_by_ai_agent ||
      r.filled_by_automated_pipeline ||
      null;
    // Map filler_type (the computed column: "HumanAgent"/"AIAgent"/...) through;
    // if blank (unfilled role), agent_type is null like the reasoner.
    roles_filled_by[r.role_id] = {
      agent: agent || null,
      agent_type: agent ? (r.filler_type || null) : null,
    };
  }

  return { precedence_closure, delegation, disjoint_classes, roles_filled_by };
}

// Attach the derived `precedesStep` multi-value to each WorkflowStep, so the
// reasoned individuals match the reasoner (which materializes the closure onto
// each step). Read from the closure view, grouped by from_id.
function attachPrecedesStep(individuals, precedence_closure) {
  const bySource = new Map();
  for (const p of precedence_closure.pairs) {
    if (!bySource.has(p.from_id)) bySource.set(p.from_id, []);
    bySource.get(p.from_id).push(p.to_id);
  }
  for (const step of individuals.WorkflowSteps || []) {
    const targets = (bySource.get(step.workflowStepId) || []).slice().sort();
    if (targets.length === 1) step.precedesStep = targets[0];
    else if (targets.length > 1) step.precedesStep = targets;
    // length 0: leave unset, exactly as the reasoner does for a terminal step.
  }
}

// reasoned_triples: the reasoner reports the size of the closed RDF graph. There
// is no triple count in Postgres. Rather than invent a number (a fallback), we
// report a transparent, deterministic surrogate: the total count of populated
// (subject, predicate, object) cells across all view rows — i.e. every non-null
// field of every reasoned individual. It is honestly labeled by the engine name
// in the payload, and it is computed, not guessed. The UI shows it as the
// engine's "reasoned facts" counter; the two engines need not agree on it (they
// count different things), and that is fine and visible.
function countCells(individuals) {
  let n = 0;
  for (const rows of Object.values(individuals)) {
    for (const row of rows) {
      for (const [k, v] of Object.entries(row)) {
        if (k.startsWith("_")) continue;
        if (v !== null && v !== undefined && v !== "") n++;
      }
    }
  }
  return n;
}

export async function runPostgres(db) {
  const schemaMap = await loadSchemaMapAuthoritative();
  const client = await pool().connect();
  try {
    await client.query("BEGIN");
    await loadRawRows(client, db, schemaMap);

    const individuals = {};
    for (const t of Object.values(schemaMap)) {
      individuals[t.table] = await readView(client, t);
    }

    const competency = await readCompetency(client, db, schemaMap);
    attachPrecedesStep(individuals, competency.precedence_closure);

    // We computed everything from a consistent snapshot; nothing was written to
    // persist (the rows we loaded ARE the db the caller will persist via the
    // file). Roll back so the DB returns to its pre-call state — the base tables
    // are scratch space for this read, not a second source of truth.
    await client.query("ROLLBACK");

    return {
      engine: "postgres",
      reasoned_triples: countCells(individuals),
      individuals,
      competency,
    };
  } catch (e) {
    try {
      await client.query("ROLLBACK");
    } catch {
      /* the original error is what matters; surface it */
    }
    // No fallback. A failure here is a real failure — the same contract as the
    // reasoner exiting non-zero. The caller 500s with this message.
    throw new Error(`postgres substrate failed: ${e.message}`);
  } finally {
    client.release();
  }
}

// The rulebook path, relative to this file. app/backend/dal/ -> talisman-basic/
import path from "node:path";
import { fileURLToPath } from "node:url";
const __dirname = path.dirname(fileURLToPath(import.meta.url));
function RULEBOOK_PATH() {
  return (
    process.env.ERB_RULEBOOK_PATH ||
    path.join(
      __dirname,
      "..",
      "..",
      "..",
      "effortless-rulebook",
      "talisman-basic-rulebook.json"
    )
  );
}

export async function postgresHealth() {
  const client = await pool().connect();
  try {
    await client.query("SELECT 1");
    return true;
  } finally {
    client.release();
  }
}

// ---------------------------------------------------------------------------
// Persistent writes — Model B: when the user is on the Postgres backend, edits
// land in the Postgres BASE TABLES for real (committed), so Postgres owns its
// store. The views recompute on the next read. A write that violates a
// constraint throws and rolls back — same loud-failure contract as everywhere.
//
// `cls` is the rulebook ClassName; `id` is the *Id primary-key value. Raw
// fields only (the server rejects computed keys before calling these). We map
// camel field names -> snake columns via the schema-map, so an unknown field
// (e.g. a derived key that slipped through) is rejected by Postgres rather than
// silently dropped.
// ---------------------------------------------------------------------------
async function tableFor(cls) {
  const sm = await loadSchemaMapAuthoritative();
  const t = sm[cls];
  if (!t) throw new Error(`unknown class: ${cls}`);
  return t;
}

// Translate a raw-field camel object to {snakeCol: value} using the schema,
// keeping only fields the class actually declares as raw/editable.
function rawToColumns(tableMap, obj) {
  const byCamel = new Map(tableMap.rawFields.map((f) => [f.camel, f]));
  const cols = {};
  for (const [k, v] of Object.entries(obj)) {
    const f = byCamel.get(k);
    if (!f) {
      throw new Error(
        `field '${k}' is not a raw/editable field of ${tableMap.table} ` +
        `(it is derived, a back-reference, or unknown) — refusing to write it`
      );
    }
    cols[f.snake] = v;
  }
  return cols;
}

export async function pgInsert(cls, row) {
  const t = await tableFor(cls);
  const cols = rawToColumns(t, row);
  const keys = Object.keys(cols);
  if (keys.length === 0) throw new Error(`no raw fields to insert for ${cls}`);
  const client = await pool().connect();
  try {
    await client.query(
      `INSERT INTO ${t.sqlTable} (${keys.join(", ")}) ` +
        `VALUES (${keys.map((_, i) => `$${i + 1}`).join(", ")})`,
      keys.map((k) => cols[k])
    );
  } finally {
    client.release();
  }
}

export async function pgUpdate(cls, id, patch) {
  const t = await tableFor(cls);
  const cols = rawToColumns(t, patch);
  const keys = Object.keys(cols);
  if (keys.length === 0) return; // nothing to change
  const client = await pool().connect();
  try {
    const set = keys.map((k, i) => `${k} = $${i + 1}`).join(", ");
    const r = await client.query(
      `UPDATE ${t.sqlTable} SET ${set} WHERE ${t.pk.snake} = $${keys.length + 1}`,
      [...keys.map((k) => cols[k]), id]
    );
    if (r.rowCount === 0) throw new Error(`no ${cls} with ${t.pk.camel} = ${id}`);
  } finally {
    client.release();
  }
}

export async function pgDelete(cls, id) {
  const t = await tableFor(cls);
  const client = await pool().connect();
  try {
    const r = await client.query(
      `DELETE FROM ${t.sqlTable} WHERE ${t.pk.snake} = $1`,
      [id]
    );
    if (r.rowCount === 0) throw new Error(`no ${cls} with ${t.pk.camel} = ${id}`);
  } finally {
    client.release();
  }
}

// Read the FULL raw store back out of the Postgres base tables, in the db.json
// shape (ClassName -> [camel rows]). This is what "export Postgres -> rulebook"
// and the drift check consume. RAW fields only — never the view's computed
// columns, so we read the base tables, not the vw_*.
export async function pgRawStore() {
  const sm = await loadSchemaMapAuthoritative();
  const client = await pool().connect();
  try {
    const store = {};
    for (const t of Object.values(sm)) {
      const { rows } = await client.query(
        `SELECT ${t.rawFields.map((f) => f.snake).join(", ")} FROM ${t.sqlTable}`
      );
      const byCol = new Map(t.rawFields.map((f) => [f.snake, f]));
      store[t.table] = rows.map((r) => {
        const out = {};
        for (const [col, val] of Object.entries(r)) {
          const f = byCol.get(col);
          out[f.camel] = normalizeValue(val, f);
        }
        return out;
      });
    }
    return store;
  } finally {
    client.release();
  }
}
