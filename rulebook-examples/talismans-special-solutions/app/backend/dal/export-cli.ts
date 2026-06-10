#!/usr/bin/env -S npx tsx
// ---------------------------------------------------------------------------
// export-cli.ts — CLI wrapper so the rebuild scripts can export a data store
// into the rulebook.
//
//   node export-cli.ts reasoner   # export db.json        -> rulebook data:[]
//   node export-cli.ts postgres   # export Postgres tables -> rulebook data:[]
//
// Reads the chosen store, writes the rulebook's data arrays (raw fields only),
// prints a one-line-per-table summary. Exits non-zero on any failure so the
// calling shell script (set -e) aborts the rebuild rather than building from a
// half-written rulebook.
// ---------------------------------------------------------------------------
import path from "node:path";
import { fileURLToPath } from "node:url";
import { readRawStore } from "./store";
import { exportStoreToRulebook, writeDbJsonFromRulebook } from "./export-to-rulebook";
import { authoredColumns } from "./postgres";

// Local shapes for the values these sibling modules return. The sibling DAL
// files are still plain JS at this point in the migration, so their exports are
// untyped; we describe only the fields this CLI actually consumes.
interface ExportSummary {
  tables: { table: string; rows: number }[];
  rowsWritten: number;
}
interface SeedSummary {
  tables: number;
  rows: number;
}

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const RULEBOOK_PATH =
  process.env.ERB_RULEBOOK_PATH ||
  path.join(__dirname, "..", "..", "..", "effortless-rulebook", "talismans-special-solutions-rulebook.json");
const DB_JSON_PATH = path.join(__dirname, "..", "db.json");

const verb: string | undefined = process.argv[2];

try {
  // The base-table column set is the authority for which fields are raw/
  // authored (see schema-map.js). The DB is up at every point these run (right
  // after a build), so we always pass it.
  const authored = await authoredColumns();
  if (verb === "reasoner" || verb === "postgres") {
    // export <store> -> rulebook data:[]
    const data = await readRawStore(verb);
    const summary = (await exportStoreToRulebook(data, RULEBOOK_PATH, authored)) as ExportSummary;
    for (const t of summary.tables) console.log(`  ${t.table}: ${t.rows} rows`);
    console.log(`Exported ${summary.rowsWritten} rows from '${verb}' store into rulebook data:[].`);
  } else if (verb === "seed-dbjson") {
    // rulebook data:[] -> db.json (relock the reasoner store after a build)
    const s = (await writeDbJsonFromRulebook(RULEBOOK_PATH, DB_JSON_PATH, authored)) as SeedSummary;
    console.log(`Seeded db.json from rulebook: ${s.rows} rows across ${s.tables} tables.`);
  } else {
    console.error("usage: export-cli.ts <reasoner|postgres|seed-dbjson>");
    process.exit(2);
  }
  process.exit(0);
} catch (e) {
  console.error(`export-cli failed: ${(e as Error).message}`);
  process.exit(1);
}
