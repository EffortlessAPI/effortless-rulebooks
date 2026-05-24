#!/usr/bin/env node
// Live-mirror rulebook → xlsx workflow:
//   1) Take the source rulebook and delete all of its `data` arrays.
//   2) Export every row from each vw_<table> view and repopulate the data.
//   3) Write the result to xlsx/exported-rulebook.json.
//   4) Run `effortless -buildLocal` from xlsx/ so the rulebook-to-xlsx
//      transpiler turns that live JSON into xlsx/rulebook.xlsx.

import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { spawnSync } from "node:child_process";
import pg from "pg";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");
const RULEBOOK_SRC = path.join(ROOT, "effortless-rulebook/effortless-rulebook.json");
const XLSX_DIR = path.join(ROOT, "xlsx");
const LIVE_JSON = path.join(XLSX_DIR, "exported-rulebook.json");

const DATABASE_URL =
  process.env.DATABASE_URL ||
  "postgresql://postgres@localhost:5432/erb_customer_crm_demo";

// PascalCase → snake_case (matches rulebook-to-postgres). IsVIP → is_vip,
// CustomerFirstName → customer_first_name, OrdersId → orders_id.
function snake(s) {
  return s
    .replace(/([a-z0-9])([A-Z])/g, "$1_$2")
    .replace(/([A-Z]+)([A-Z][a-z])/g, "$1_$2")
    .toLowerCase();
}

function isEntity(v) {
  return !!v && typeof v === "object" && Array.isArray(v.schema);
}

// pg returns NUMERIC / DECIMAL / BIGINT as strings (to avoid precision loss).
// For the rulebook export we want real JSON numbers / booleans / ISO dates so
// downstream tooling (the xlsx transpiler, diff tools) sees the right types.
const NUMERIC_TYPES = new Set(["number", "integer", "double", "decimal", "float", "currency"]);
const BOOLEAN_TYPES = new Set(["boolean", "bool"]);
const DATE_TYPES = new Set(["date", "datetime", "timestamp", "timestamptz"]);

function coerce(value, datatype) {
  if (value === null || value === undefined) return value;
  const dt = (datatype ?? "").toLowerCase();

  if (NUMERIC_TYPES.has(dt)) {
    if (typeof value === "number") return value;
    if (typeof value === "string" && value.trim() !== "") {
      const n = Number(value);
      return Number.isFinite(n) ? n : value;
    }
    return value;
  }

  if (BOOLEAN_TYPES.has(dt)) {
    if (typeof value === "boolean") return value;
    if (typeof value === "string") {
      const v = value.toLowerCase();
      if (v === "true" || v === "t" || v === "1") return true;
      if (v === "false" || v === "f" || v === "0") return false;
    }
    return value;
  }

  if (DATE_TYPES.has(dt)) {
    if (value instanceof Date) return value.toISOString().slice(0, 10);
    return value;
  }

  return value;
}

async function main() {
  const rulebook = JSON.parse(fs.readFileSync(RULEBOOK_SRC, "utf-8"));

  // Step 1: wipe every entity's data.
  for (const [key, val] of Object.entries(rulebook)) {
    if (key.startsWith("$") || key.startsWith("_") || !isEntity(val)) continue;
    val.data = [];
  }

  // Step 2: populate from postgres views.
  const client = new pg.Client({ connectionString: DATABASE_URL });
  await client.connect();
  try {
    for (const [key, val] of Object.entries(rulebook)) {
      if (key.startsWith("$") || key.startsWith("_") || !isEntity(val)) continue;
      const view = `vw_${snake(key)}`;
      try {
        const { rows } = await client.query(`SELECT * FROM ${view}`);
        val.data = rows.map((row) => {
          const out = {};
          for (const f of val.schema) {
            const col = snake(f.name);
            if (col in row) out[f.name] = coerce(row[col], f.datatype);
          }
          return out;
        });
        console.log(`[refresh] ${key}: ${rows.length} rows ← ${view}`);
      } catch (err) {
        console.warn(`[refresh] skipping ${key} (${view}): ${err.message}`);
      }
    }
  } finally {
    await client.end();
  }

  // Step 3: write live JSON.
  fs.mkdirSync(XLSX_DIR, { recursive: true });
  fs.writeFileSync(LIVE_JSON, JSON.stringify(rulebook, null, 2));
  console.log(`[refresh] wrote ${path.relative(ROOT, LIVE_JSON)}`);

  // Step 4: run rulebook-to-xlsx via effortless -buildLocal from xlsx/.
  const res = spawnSync("effortless", ["-buildLocal"], {
    cwd: XLSX_DIR,
    stdio: "inherit",
  });
  if (res.status !== 0) {
    throw new Error(`effortless -buildLocal exited ${res.status}`);
  }
}

main().catch((e) => {
  console.error(e);
  process.exit(1);
});
