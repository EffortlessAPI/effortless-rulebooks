// ---------------------------------------------------------------------------
// schema-map.js — the rulebook IS the mapping.
//
// Both execution substrates (the OWL reasoner and Postgres) are generated from
// the SAME rulebook. So the camelCase <-> snake_case correspondence, and the
// raw-vs-derived split, are not facts we get to invent here — they are facts
// the rulebook already states. This module reads the rulebook schema and
// derives them. If the schema changes, this mapping changes with it; there is
// no hand-maintained parallel table to drift out of sync.
//
// Per the project doctrine (CLAUDE.md): we do NOT re-derive what the substrate
// already computes. This file only describes the SHAPE of each table's RAW
// fields so we can (a) load raw rows into Postgres base tables and (b) export
// raw rows back to the rulebook. It never touches computed values.
// ---------------------------------------------------------------------------
import { readFile } from "node:fs/promises";

// A field is RAW/editable iff its rulebook `type` is one the user types by
// hand. `calculated`, `lookup`, and `aggregation` are DERIVED — the substrate
// computes them, and they must never be written back as data (that would bake a
// stale computed value into the SSoT). `relationship` is a foreign key the user
// sets (one row pointing at another), so it is *potentially* editable.
//
// BUT: the rulebook does NOT machine-flag which relationship fields are pure
// BACK-references (the inverse side, populated by the other table) vs authored
// FKs. The transpiler decides this when it generates the base tables — some
// back-refs get a base-table column (and authored seed values), some don't, and
// the prose "Back-reference" label is unreliable. So the AUTHORITY for "which
// fields are authored/raw" is the generated base-table column set: a field is
// raw iff the base table has a column for it. We read that live from
// information_schema (see authoredColumns()), which is generated from the same
// rulebook — still SSoT-faithful, and it matches exactly what the views and the
// reasoner treat as authored. When the base-table columns are unavailable (no
// DB), we fall back to the rulebook type alone, EXCLUDING relationship fields
// whose prose marks them inverse — and we say so loudly rather than guess.
const EDITABLE_TYPES = new Set(["raw", "relationship"]);
const DERIVED_TYPES = new Set(["calculated", "lookup", "aggregation"]);

// PascalCase field name (rulebook) -> camelCase property (reasoner JSON / db.json).
// e.g. "WorkflowId" -> "workflowId", "AIAgentId" -> "aIAgentId" (matches the
// reasoner's existing convention seen in db.json keys like `aIAgentId`).
export function pascalToCamel(name) {
  return name.charAt(0).toLowerCase() + name.slice(1);
}

// PascalCase field name (rulebook) -> snake_case column (Postgres).
// e.g. "StepDurationMinutes" -> "step_duration_minutes",
//      "AIAgentId"           -> "ai_agent_id",
//      "FilledByAIAgent"     -> "filled_by_ai_agent",
//      "HasMoreThan1Step"    -> "has_more_than1_step".
// This MUST match what rulebook-to-postgres emits. The transpiler lowercases
// runs of capitals together and treats a digit as gluing to the preceding word.
export function pascalToSnake(name) {
  return name
    // boundary between a lowercase/digit and an uppercase letter: stepDuration -> step_Duration
    .replace(/([a-z0-9])([A-Z])/g, "$1_$2")
    // boundary between an acronym run and a following Word: AIAgent -> AI_Agent
    .replace(/([A-Z]+)([A-Z][a-z])/g, "$1_$2")
    .toLowerCase();
}

// PascalCase table key (rulebook top-level) -> snake_case table name (Postgres).
// e.g. "WorkflowSteps" -> "workflow_steps", "AIAgents" -> "ai_agents".
export function tableToSnake(tableName) {
  return pascalToSnake(tableName);
}

// Load the rulebook and return, per table, the field-level mapping the adaptors
// need. We key by the rulebook's top-level table name (PascalCase).
//
// Returns:
//   {
//     <TableName>: {
//       table: <TableName>,
//       sqlTable: "<snake_table>",
//       view: "vw_<snake_table>",
//       pk: { pascal, camel, snake },         // primary-key field
//       rawFields: [ {pascal, camel, snake, datatype, type, isBackref} ],   // exportable
//       derivedFields: [ {pascal, camel, snake, type} ],                    // never exported
//       allFields: [ ... both ... ],
//     },
//     ...
//   }
export async function loadSchemaMap(rulebookPath, authoredCols = null) {
  const rb = JSON.parse(await readFile(rulebookPath, "utf8"));
  const map = {};

  for (const [tableName, table] of Object.entries(rb)) {
    if (tableName.startsWith("__")) continue; // __meta__ etc.
    if (!table || !Array.isArray(table.schema)) continue;

    const sqlTable = tableToSnake(tableName);
    // The authoritative authored-column set for this table, if provided.
    const authored = authoredCols ? authoredCols[sqlTable] : null;

    const rawFields = [];
    const derivedFields = [];
    const allFields = [];
    let pk = null;

    for (const f of table.schema) {
      const pascal = f.name;
      const entry = {
        pascal,
        camel: pascalToCamel(pascal),
        snake: pascalToSnake(pascal),
        datatype: f.datatype || "string",
        type: f.type || "raw",
        relatedTo: f.RelatedTo || null,
        // Prose fallback: only consulted when we have NO base-table authority.
        // Unreliable (see header), so used only as a last resort.
        proseBackref: /back-?reference|inverse of/i.test(f.Description || ""),
      };
      allFields.push(entry);

      // Primary key: first field whose name ends with "Id".
      if (!pk && /Id$/.test(pascal)) pk = entry;

      // Decide raw vs derived.
      let isRaw;
      if (DERIVED_TYPES.has(entry.type)) {
        isRaw = false; // calculated/lookup/aggregation: never authored
      } else if (authored) {
        // AUTHORITATIVE path: raw iff the base table has this column.
        isRaw = authored.has(entry.snake);
      } else if (EDITABLE_TYPES.has(entry.type)) {
        // No base-table authority available: trust the rulebook type, but drop
        // relationship fields whose prose marks them inverse (best effort).
        isRaw = !(entry.type === "relationship" && entry.proseBackref);
      } else {
        isRaw = false;
      }

      (isRaw ? rawFields : derivedFields).push(entry);
    }

    if (!pk) {
      throw new Error(
        `schema-map: table '${tableName}' has no *Id primary-key field; ` +
        `cannot map it to a substrate. Fix the rulebook schema.`
      );
    }
    // The PK must always be authored/raw even if it somehow didn't land in
    // rawFields (it always should — it's the base-table PK column).
    if (!rawFields.includes(pk) && authored && authored.has(pk.snake)) {
      rawFields.unshift(pk);
      const di = derivedFields.indexOf(pk);
      if (di >= 0) derivedFields.splice(di, 1);
    }

    map[tableName] = {
      table: tableName,
      sqlTable,
      view: `vw_${sqlTable}`,
      pk,
      rawFields,
      derivedFields,
      allFields,
    };
  }

  return map;
}
