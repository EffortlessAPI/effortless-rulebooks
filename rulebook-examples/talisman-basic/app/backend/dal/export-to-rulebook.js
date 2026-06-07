// ---------------------------------------------------------------------------
// export-to-rulebook.js — write a raw data store BACK into the rulebook's
// data:[] arrays. This is the "Rebuild from X" round-trip's first half:
//
//     <store> --(this)--> rulebook data:[]  --(effortless build)--> both spokes
//
// CRITICAL CORRECTNESS BOUNDARY (the user signed off on exactly this):
//   - We write ONLY raw/editable fields (rulebook field type raw/relationship,
//     never calculated/lookup/aggregation, never back-references). Writing a
//     computed value back as data would bake a stale derived value into the
//     SSoT — the exact poison the doctrine forbids. We key off the schema-map's
//     rawFields, so a derived column can never leak in even if the store row
//     carries one.
//   - We touch ONLY each table's data:[] array. We NEVER touch `schema`
//     (the rules) or any other rulebook structure. Rules are sacred; only the
//     mock data rows round-trip.
//   - The rulebook JSON keeps the field NAMES it already uses in its data rows
//     (PascalCase, e.g. "WorkflowId") — NOT the camelCase the store uses. We map
//     store camel -> rulebook Pascal via the schema-map so the rewritten rows
//     match the rulebook's existing convention exactly.
//
// We do NOT git-guard here (that's the orchestration script's job, and the user
// chose git-reset as the backup story). This module is a pure data transform +
// file write; the caller decides when it's safe to run.
// ---------------------------------------------------------------------------
import { readFile, writeFile } from "node:fs/promises";
import { loadSchemaMap } from "./schema-map.js";

// Coerce a raw value to the JSON the rulebook stores for that field's datatype.
// The rulebook stores numbers as numbers, booleans as booleans, dates as ISO
// strings, everything else as strings. The store already holds native types
// (pg returns numbers/booleans; db.json holds whatever was typed), so mostly we
// pass through — but we normalize empty/undefined to "" for string/relationship
// fields (the rulebook's absent-FK convention) and leave null for nullable
// numerics.
function coerce(val, field) {
  if (val === undefined) val = null;
  const dt = field.datatype;
  if (val === null) {
    // relationship / string absent value is "" in the rulebook (not null), to
    // match how the transpiler reads absent FKs.
    if (field.type === "relationship" || dt === "string") return "";
    return null;
  }
  if (dt === "integer") return typeof val === "number" ? val : parseInt(val, 10);
  if (dt === "number" || dt === "float" || dt === "decimal")
    return typeof val === "number" ? val : parseFloat(val);
  if (dt === "boolean") return val === true || val === "true" || val === "t";
  return String(val);
}

// Build one rulebook data row (PascalCase keyed) from a store row (camelCase),
// including ONLY the table's raw fields.
function storeRowToRulebookRow(tableMap, storeRow) {
  const out = {};
  for (const f of tableMap.rawFields) {
    out[f.pascal] = coerce(storeRow[f.camel], f);
  }
  return out;
}

// Export `store` (a { ClassName: [camel rows] } object — db.json shape or
// pgRawStore() output) into the rulebook at `rulebookPath`. Rewrites each
// table's data:[] with the store's rows (raw fields only). Returns a summary of
// what changed for the caller to log.
export async function exportStoreToRulebook(store, rulebookPath, authoredCols = null) {
  const schemaMap = await loadSchemaMap(rulebookPath, authoredCols);
  const rb = JSON.parse(await readFile(rulebookPath, "utf8"));

  const summary = { tables: [], rowsWritten: 0 };

  for (const [cls, tableMap] of Object.entries(schemaMap)) {
    const storeRows = store[cls];
    if (!Array.isArray(storeRows)) {
      // The store has no rows for a table the rulebook declares. That is a real
      // state to surface, not silently treat as "empty" — but emptying the
      // rulebook table because the store happens to lack it would be data loss.
      // We REFUSE: a missing store table means the export is incomplete; the
      // caller should fix the store, not let us blank the rulebook.
      throw new Error(
        `export refused: store has no rows array for table '${cls}', ` +
        `which the rulebook declares. Refusing to blank the rulebook table. ` +
        `(Is the data store fully populated?)`
      );
    }
    if (!rb[cls] || typeof rb[cls] !== "object") {
      throw new Error(`rulebook has no table object for '${cls}' — schema drift?`);
    }
    const rows = storeRows.map((r) => storeRowToRulebookRow(tableMap, r));
    rb[cls].data = rows;            // ONLY data:[] — schema untouched
    summary.tables.push({ table: cls, rows: rows.length });
    summary.rowsWritten += rows.length;
  }

  await writeFile(rulebookPath, JSON.stringify(rb, null, 2) + "\n", "utf8");
  return summary;
}

// ---------------------------------------------------------------------------
// The OTHER direction: seed a db.json-shaped store FROM the rulebook's data:[]
// arrays (raw fields only, camelCase keyed). This is how the reasoner's store
// gets back in lockstep after a Rebuild: `effortless build` regenerates the
// Postgres tables from the rulebook, but db.json is the app's own store and no
// transpiler writes it — so we project the rulebook's data into it here.
//
// Same raw-only boundary: we read each table's data rows and keep ONLY the raw
// fields, mapped Pascal -> camel. Derived columns in the rulebook data (some
// seed rows carry e.g. "Name") are dropped — they'll be recomputed by the
// reasoner, never stored.
export async function seedStoreFromRulebook(rulebookPath, authoredCols = null) {
  const schemaMap = await loadSchemaMap(rulebookPath, authoredCols);
  const rb = JSON.parse(await readFile(rulebookPath, "utf8"));
  const store = {};
  for (const [cls, tableMap] of Object.entries(schemaMap)) {
    const data = (rb[cls] && Array.isArray(rb[cls].data)) ? rb[cls].data : [];
    store[cls] = data.map((row) => {
      const out = {};
      for (const f of tableMap.rawFields) {
        // rulebook data rows are PascalCase-keyed; project to camel for the store
        out[f.camel] = coerce(row[f.pascal], f);
      }
      return out;
    });
  }
  return store;
}

// Write the rulebook-derived store to db.json (the reasoner's store file).
export async function writeDbJsonFromRulebook(rulebookPath, dbJsonPath, authoredCols = null) {
  const store = await seedStoreFromRulebook(rulebookPath, authoredCols);
  await writeFile(dbJsonPath, JSON.stringify(store, null, 2) + "\n", "utf8");
  let rows = 0;
  for (const arr of Object.values(store)) rows += arr.length;
  return { tables: Object.keys(store).length, rows };
}
