// ---------------------------------------------------------------------------
// store.js — engine-owned persistence (Model B).
//
// Each substrate owns its OWN raw store:
//   - reasoner: app/backend/db.json (a file)
//   - postgres: the Postgres base tables (committed rows)
//
// When the user is logged into an engine and edits a row, the edit persists in
// THAT engine's store. The two stores can drift between rebuilds — by design;
// the login page surfaces the drift and a "Rebuild from X" round-trips one
// store through the rulebook hub to relock both. We never silently auto-sync
// one store from the other (that would hide the very drift we want visible).
//
// This module gives the server one CRUD interface keyed by backend name, so the
// write endpoints don't branch on the engine. After every mutation we RE-READ
// through the same engine to validate (a bad edit throws -> 500, and for the
// file store we re-read before persisting so db.json is never corrupted).
// ---------------------------------------------------------------------------
import { readFile, writeFile } from "node:fs/promises";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { reason } from "./index.js";
import { pgInsert, pgUpdate, pgDelete, pgRawStore } from "./postgres.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const DB_PATH = path.join(__dirname, "..", "db.json");

// --- db.json (the reasoner's store) ---------------------------------------
async function readDbFile() {
  if (!existsSync(DB_PATH)) throw new Error(`db.json missing at ${DB_PATH}`);
  return JSON.parse(await readFile(DB_PATH, "utf8"));
}
async function writeDbFile(db) {
  await writeFile(DB_PATH, JSON.stringify(db, null, 2) + "\n", "utf8");
}

function findIdx(rows, id) {
  return rows.findIndex((r) =>
    Object.entries(r).some(([k, v]) => k.endsWith("Id") && v === id)
  );
}

// --- the unified, engine-keyed store interface ----------------------------
// Each method returns { engine, reasoned_triples } so the API echoes which
// engine handled the write and how big the resulting model is.

export async function readRawStore(backend) {
  if (backend === "postgres") return await pgRawStore();
  return await readDbFile();
}

export async function insertRow(backend, cls, row) {
  if (backend === "postgres") {
    await pgInsert(cls, row);
    const reasoned = await reason(await pgRawStore(), "postgres");
    return { engine: "postgres", reasoned_triples: reasoned.reasoned_triples };
  }
  // reasoner / file store: re-reason BEFORE persisting (never corrupt db.json).
  const db = await readDbFile();
  if (!Array.isArray(db[cls])) db[cls] = [];
  db[cls].push(row);
  const reasoned = await reason(db, "reasoner");
  await writeDbFile(db);
  return { engine: "reasoner", reasoned_triples: reasoned.reasoned_triples };
}

export async function patchRow(backend, cls, id, patch) {
  if (backend === "postgres") {
    await pgUpdate(cls, id, patch);
    const reasoned = await reason(await pgRawStore(), "postgres");
    return { engine: "postgres", reasoned_triples: reasoned.reasoned_triples };
  }
  const db = await readDbFile();
  const rows = db[cls];
  if (!Array.isArray(rows)) throw new Error(`unknown class: ${cls}`);
  const idx = findIdx(rows, id);
  if (idx < 0) throw new Error(`no ${cls} with id ${id}`);
  rows[idx] = { ...rows[idx], ...patch };
  const reasoned = await reason(db, "reasoner");
  await writeDbFile(db);
  return { engine: "reasoner", reasoned_triples: reasoned.reasoned_triples };
}

export async function deleteRow(backend, cls, id) {
  if (backend === "postgres") {
    await pgDelete(cls, id);
    const reasoned = await reason(await pgRawStore(), "postgres");
    return { engine: "postgres", reasoned_triples: reasoned.reasoned_triples };
  }
  const db = await readDbFile();
  const rows = db[cls];
  if (!Array.isArray(rows)) throw new Error(`unknown class: ${cls}`);
  const idx = findIdx(rows, id);
  if (idx < 0) throw new Error(`no ${cls} with id ${id}`);
  const next = { ...db, [cls]: rows.filter((_, i) => i !== idx) };
  const reasoned = await reason(next, "reasoner");
  await writeDbFile(next);
  return { engine: "reasoner", reasoned_triples: reasoned.reasoned_triples };
}

// Apply a function that mutates a raw-db object (used by scenarios). For the
// file store we mutate-then-persist; for Postgres we read the store, mutate the
// in-memory copy, then replay it as a full reload (truncate+insert) so the
// committed tables reflect the scenario. Both re-compute to validate.
export async function applyMutation(backend, mutate) {
  if (backend === "postgres") {
    const store = await pgRawStore();
    mutate(store);
    // Persist by replaying the mutated store into the base tables.
    await replacePgStore(store);
    const reasoned = await reason(await pgRawStore(), "postgres");
    return { engine: "postgres", reasoned_triples: reasoned.reasoned_triples };
  }
  const db = await readDbFile();
  mutate(db);
  const reasoned = await reason(db, "reasoner");
  await writeDbFile(db);
  return { engine: "reasoner", reasoned_triples: reasoned.reasoned_triples };
}

// Replace the entire Postgres raw store with `store` (a db.json-shaped object),
// committed. Used by scenarios and by Reset-to-store flows on the pg side.
async function replacePgStore(store) {
  // Reuse the loader the read path uses, but commit instead of rolling back.
  const { default: pg } = await import("pg");
  const { loadSchemaMapAuthoritative } = await import("./postgres.js");
  const sm = await loadSchemaMapAuthoritative();
  const ERB_DOMAIN = process.env.ERB_DOMAIN || "talisman-basic";
  const CONN =
    process.env.DATABASE_URL ||
    `postgresql://postgres@localhost:5432/erb_${ERB_DOMAIN.replace(/-/g, "_")}`;
  const client = new pg.Client({ connectionString: CONN });
  await client.connect();
  try {
    await client.query("BEGIN");
    await client.query("SET CONSTRAINTS ALL DEFERRED");
    for (const t of Object.values(sm)) await client.query(`TRUNCATE TABLE ${t.sqlTable} CASCADE`);
    for (const t of Object.values(sm)) {
      const rows = store[t.table];
      if (!Array.isArray(rows)) continue;
      for (const row of rows) {
        const cols = [], ph = [], vals = [];
        let i = 1;
        for (const f of t.rawFields) {
          if (Object.prototype.hasOwnProperty.call(row, f.camel)) {
            cols.push(f.snake); ph.push(`$${i++}`); vals.push(row[f.camel]);
          }
        }
        if (cols.length === 0) continue;
        await client.query(
          `INSERT INTO ${t.sqlTable} (${cols.join(", ")}) VALUES (${ph.join(", ")})`,
          vals
        );
      }
    }
    await client.query("COMMIT");
  } catch (e) {
    await client.query("ROLLBACK").catch(() => {});
    throw e;
  } finally {
    await client.end();
  }
}
