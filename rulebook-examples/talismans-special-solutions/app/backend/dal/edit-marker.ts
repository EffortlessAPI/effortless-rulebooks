// ---------------------------------------------------------------------------
// edit-marker.ts — the drift signal behind the login-page triangle.
//
// Three raw stores can drift between rebuilds (Model B; see store.js):
//
//        RULEBOOK (hub, the head)
//        /                      \
//   db.json (reasoner leg)   Postgres tables (postgres leg)
//
// The triangle on the login page needs to answer one question: "which leg is
// AHEAD of the rulebook?" — i.e. which store carries edits the rulebook hasn't
// absorbed yet. We answer it WITHOUT a bespoke cache (CLAUDE.md forbids those):
// the answer is computed LIVE from the three stores every time /api/triangle is
// hit. We project each store to its editable raw fields, hash it, and compare.
//
// The only thing we persist is a tiny "who-touched-it-last, when" sidecar
// (.store-state.json) so a tie can be broken and the UI can show a timestamp.
// That sidecar is NOT the source of truth for drift — the live hashes are. The
// sidecar only records edit *recency*; if it is missing or stale, drift is still
// correctly detected from the hashes (the timestamp just shows "unknown"). So it
// is a convenience marker, not a cache that could mask a wrong answer.
//
// Convergence contract: every control action (Reset / Rebuild / Rebuild-from-X)
// ends by reseeding BOTH stores from the one rulebook. After that, all three
// projected hashes are identical -> aheadOf === null -> the triangle goes quiet.
// That is the whole point of the round-trip, and it is why the projection +
// canonicalization MUST be identical across all three stores (see projectStore).
// ---------------------------------------------------------------------------
import { readFile, writeFile } from "node:fs/promises";
import { existsSync } from "node:fs";
import { createHash } from "node:crypto";
import path from "node:path";
import { fileURLToPath } from "node:url";
import type { SchemaMap } from "./schema-map";

// A single row in a raw store: a property bag keyed by camelCase field name.
// Values are whatever the engine round-trips (strings, numbers, booleans, null).
type StoreRow = Record<string, unknown>;
// A raw store is { ClassName: [ rowObj, ... ] }. Some entries may not be arrays
// (a malformed/partial store) so the code guards with Array.isArray everywhere.
type RawStore = Record<string, StoreRow[] | unknown>;
// A projected store is the same shape but every class is guaranteed an array of
// rows narrowed to the schema-map's raw fields.
type ProjectedStore = Record<string, StoreRow[]>;

// One per-field difference: HEAD is what OTHER would BECOME if HEAD wins.
interface DiffFieldEntry {
  field: string;
  head: unknown;
  other: unknown;
}

// One per-row difference, aligned by primary key within a table.
interface DiffRowEntry {
  pk: string;
  kind: "added" | "removed" | "changed";
  fields: DiffFieldEntry[];
}

// All row diffs for a single table.
interface DiffTableEntry {
  table: string;
  rows: DiffRowEntry[];
}

// The result of diffStores: the DiffAgainst-ish shape the triangle endpoint
// returns. Counts let the UI badge the magnitude.
interface DiffResult {
  tables: DiffTableEntry[];
  changedFields: number;
  changedRows: number;
}

// The recency sidecar shape: per app-stamped store, when it was last edited.
interface StoreMarker {
  lastEditAt: string;
}
type StoreState = Partial<Record<"reasoner" | "postgres", StoreMarker>>;

const __dirname = path.dirname(fileURLToPath(import.meta.url));
// The recency sidecar. Lives next to db.json under app/backend/. It only carries
// edit recency; a Rebuild overwrites the stores and the hashes recompute from
// scratch, so a stale or absent sidecar can never mask real drift.
const STATE_PATH = path.join(__dirname, "..", ".store-state.json");

// The three legs of the triangle. "rulebook" is the head; the other two are the
// engine-owned data stores. These ids match the DATA axis in dal/index.js plus
// the special "rulebook" head, and the control-action engine names.
export const LEGS = ["rulebook", "reasoner", "postgres"] as const;

// --- canonical projection + hashing ----------------------------------------
// A store is { ClassName: [ rowObj, ... ] }. Two stores are "the same" when
// their editable raw rows carry the same values — regardless of (a) which extra
// derived/back-reference columns each engine's store happens to include, (b) row
// order, (c) key order, or (d) numeric-vs-string spelling a round-trip yields.

function normValue(v: unknown): string {
  if (v === null || v === undefined) return "~";          // absent sentinel
  if (typeof v === "boolean") return v ? "true" : "false";
  if (typeof v === "number") return String(v);            // 5 and 5.0 both -> "5"
  const s = String(v);
  // A numeric-looking string hashes the same as the number, so a relationship
  // FK stored as "3" vs 3 across stores does not read as drift.
  if (/^-?\d+$/.test(s)) return String(parseInt(s, 10));
  if (/^-?\d*\.\d+$/.test(s)) return String(parseFloat(s));
  return s;
}

function pkValue(row: StoreRow): string {
  // The first *Id key is the primary key in every table here (WorkflowId,
  // StepId, ...). Fall back to a stable JSON of the row if a table somehow has
  // no Id (so sorting is still deterministic rather than throwing).
  for (const k of Object.keys(row)) {
    if (k.endsWith("Id") && row[k] != null && row[k] !== "") return String(row[k]);
  }
  return JSON.stringify(row);
}

function canonicalRow(row: StoreRow): string {
  const keys = Object.keys(row).sort();
  return keys.map((k) => k + "=" + normValue(row[k])).join("|");
}

// Project a raw store down to ONLY the fields that round-trip through the hub —
// the schema-map's rawFields per table. This is the crux of correct drift
// detection: the engines' raw stores carry extra reverse-FK / back-reference
// columns (roles, precededBy, producedByStep, ...) the rulebook projection never
// has, and those are DERIVED inverses, not editable data. Comparing them would
// report phantom "drift" on a freshly-locked build. By projecting every store
// through the SAME rawFields the exporter uses, the hash depends only on the
// editable rows that actually round-trip — exactly what "ahead" should mean.
// Tables absent from the schema-map are ignored on every leg symmetrically, so
// they cannot create asymmetric drift.
//
// Raw fields that are bookkeeping, NOT content — a different value here is never
// "drift". `Modified` is a dct:modified last-touched timestamp (raw datetime on
// Workflows / Datasets): an engine restamps it on any edit, so two stores can
// carry identical content yet differ only on when they were last poked. Comparing
// it would flag that as drift and the triangle would never go quiet even though
// the data round-trips perfectly. We drop it from the projection so it leaves BOTH
// the hash (triangle verdict) and the field-level diff symmetrically on every leg.
// Match is on the camelCase key, lower-cased, so it is robust to casing.
const IGNORED_RAW_FIELDS = new Set(["modified"]);

// `schemaMap` is the loadSchemaMap() result: { Class: { rawFields:[{camel}] } }.
export function projectStore(store: RawStore, schemaMap: SchemaMap): ProjectedStore {
  const out: ProjectedStore = {};
  for (const [cls, tableMap] of Object.entries(schemaMap)) {
    const rows = Array.isArray(store[cls]) ? (store[cls] as StoreRow[]) : [];
    out[cls] = rows.map((r) => {
      const row: StoreRow = {};
      for (const f of tableMap.rawFields) {
        if (IGNORED_RAW_FIELDS.has(f.camel.toLowerCase())) continue;
        row[f.camel] = r[f.camel];
      }
      return row;
    });
  }
  return out;
}

// The canonical fingerprint of a store. Pure function of its data. Pass a store
// that has ALREADY been run through projectStore() so the three legs are
// compared on the same raw-field basis.
export function canonicalHash(store: ProjectedStore): string {
  const classes = Object.keys(store).sort();
  const h = createHash("sha256");
  for (const cls of classes) {
    const rows = Array.isArray(store[cls]) ? store[cls] : [];
    const sorted = [...rows].sort((a, b) => {
      const pa = pkValue(a), pb = pkValue(b);
      return pa < pb ? -1 : pa > pb ? 1 : 0;
    });
    h.update("#" + cls + "[" + sorted.length + "]");
    for (const r of sorted) h.update(canonicalRow(r) + ";;");
  }
  return h.digest("hex");
}

// Index a projected store's rows by primary-key value, per table, so two stores
// can be aligned row-for-row for diffing.
function indexByPk(store: ProjectedStore): Record<string, Map<string, StoreRow>> {
  const out: Record<string, Map<string, StoreRow>> = {};
  for (const [cls, rows] of Object.entries(store)) {
    out[cls] = new Map<string, StoreRow>();
    for (const r of (Array.isArray(rows) ? rows : [])) out[cls].set(pkValue(r), r);
  }
  return out;
}

// Field-level diff: "if HEAD overwrote OTHER, what exactly would change?"
//
// Both stores must ALREADY be projected (projectStore) so only round-tripping
// raw fields are compared — the same basis the triangle's hash uses, so a diff
// of an in-sync pair is empty exactly when the hashes match. We align rows by
// primary key within each table and classify every difference:
//
//   - changed : the row exists in both, but a field's value differs. We report
//               { field, head, other } so the UI can render "other-value → head-
//               value" (HEAD is what OTHER would BECOME if HEAD wins).
//   - added   : the row exists in HEAD but not OTHER (HEAD would CREATE it).
//   - removed : the row exists in OTHER but not HEAD (HEAD would DELETE it).
//
// Returns { tables: [ {table, rows:[ {pk, kind, fields:[{field, head, other}] } ]} ],
//           changedFields, changedRows } — counts let the UI badge the magnitude.
export function diffStores(
  headStore: ProjectedStore,
  otherStore: ProjectedStore,
  schemaMap: SchemaMap
): DiffResult {
  const headIdx = indexByPk(headStore);
  const otherIdx = indexByPk(otherStore);
  const tables: DiffTableEntry[] = [];
  let changedFields = 0, changedRows = 0;

  for (const cls of Object.keys(schemaMap).sort()) {
    const hMap = headIdx[cls] || new Map<string, StoreRow>();
    const oMap = otherIdx[cls] || new Map<string, StoreRow>();
    const pks = new Set([...hMap.keys(), ...oMap.keys()]);
    const rowDiffs: DiffRowEntry[] = [];

    for (const pk of [...pks].sort()) {
      const h = hMap.get(pk);
      const o = oMap.get(pk);
      if (h && !o) {
        rowDiffs.push({ pk, kind: "added", fields: Object.entries(h).map(([field, head]) => ({ field, head, other: null })) });
        changedRows++;
      } else if (!h && o) {
        rowDiffs.push({ pk, kind: "removed", fields: Object.entries(o).map(([field, other]) => ({ field, head: null, other })) });
        changedRows++;
      } else if (h && o) {
        const fields: DiffFieldEntry[] = [];
        const keys = new Set([...Object.keys(h), ...Object.keys(o)]);
        for (const field of [...keys].sort()) {
          // Compare through the same normalizer the hash uses, so 5 vs "5" is
          // NOT reported as a change (it isn't drift) — only real value changes.
          if (normValue(h[field]) !== normValue(o[field])) {
            fields.push({ field, head: h[field] ?? null, other: o[field] ?? null });
          }
        }
        if (fields.length) {
          rowDiffs.push({ pk, kind: "changed", fields });
          changedRows++;
          changedFields += fields.length;
        }
      }
    }
    if (rowDiffs.length) tables.push({ table: cls, rows: rowDiffs });
  }

  return { tables, changedFields, changedRows };
}

// --- the recency sidecar ---------------------------------------------------
// Shape: { reasoner: {lastEditAt}, postgres: {lastEditAt} }. lastEditAt is an
// ISO string of the last app-side edit to that store. The rulebook leg has no
// app stamp (hand-edited / git outside the app); its recency comes from the file
// mtime, resolved live by the caller.

async function readState(): Promise<StoreState> {
  if (!existsSync(STATE_PATH)) return {};
  try {
    return JSON.parse(await readFile(STATE_PATH, "utf8")) as StoreState;
  } catch {
    // A corrupt sidecar is NOT a reason to fail the triangle — it only carries
    // recency, and drift is detected from hashes. Treat as "no recency known".
    return {};
  }
}

async function writeState(state: StoreState): Promise<void> {
  await writeFile(STATE_PATH, JSON.stringify(state, null, 2) + "\n", "utf8");
}

// Record that `store` ("reasoner" | "postgres") was just edited through the app.
// Called at the tail of every mutation in store.js. We deliberately do NOT hash
// here (the hash is computed live at read time from the real store) — we only
// note WHEN, so the triangle can show "edited 12s ago" and break ahead-ties.
export async function stampEdit(store: string): Promise<void> {
  if (store !== "reasoner" && store !== "postgres") return; // rulebook isn't app-stamped
  const state = await readState();
  state[store] = { lastEditAt: new Date().toISOString() };
  await writeState(state);
}

// Read the recency markers (does not compute hashes). The caller (the triangle
// endpoint) supplies the live hashes; this just returns the timestamps it has.
export async function readMarkers(): Promise<StoreState> {
  return await readState();
}
