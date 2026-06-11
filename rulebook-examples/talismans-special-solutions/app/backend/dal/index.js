// ---------------------------------------------------------------------------
// dal/index.js — the execution-substrate router, modeled as TWO independent
// axes:
//
//     RULES  — which generated engine computes the derived fields:
//                "reasoner"  (OWL/SHACL, reasoner/reason.py)
//                "postgres"  (the vw_<entity> SQL views)
//     DATA   — which raw-row store the rules run against:
//                "reasoner"  (app/backend/db.json)
//                "postgres"  (the Postgres base tables)
//
// Both engines are generated from the SAME rulebook, and raw rows are just rows
// — so rules and data are ORTHOGONAL. Any (rules × data) cell is meaningful:
//   reasoner×reasoner and postgres×postgres are the two "natural" pairings the
//   login page ships today; the off-diagonal cells (same rules, the OTHER
//   database's mock data) are the easter-egg cross-runs, and they need no new
//   architecture — only a UI that exposes them.
//
// A "backend" is therefore a {rules, data} pair. The login page sends it as the
// X-ERB-Backend header (a pair id like "reasoner" / "postgres", or an explicit
// "rules:data" form like "postgres:reasoner" for a cross-run). There is no
// hidden global "active engine"; every request names its pair explicitly.
// ---------------------------------------------------------------------------
import { runReasoner } from "./reasoner.js";
import { runPostgres } from "./postgres.js";

// The two RULES engines. Each takes the raw rows and returns the reasoned shape.
export const RULES = {
  reasoner: { label: "OWL / SHACL Reasoner", run: runReasoner },
  postgres: { label: "PostgreSQL Views", run: runPostgres },
};

// The two DATA stores. (The actual read/write of each store lives in store.js;
// here we just enumerate the axis so the router and UI can reason about it.)
export const DATA = {
  reasoner: { label: "db.json" },
  postgres: { label: "Postgres tables" },
};

// The pairings the login page offers TODAY: the two natural, matched cells.
// Adding the off-diagonal cells later is purely a matter of listing them here
// (and letting the UI render them) — the run path already handles any pair.
export const BACKENDS = {
  reasoner: { label: "OWL / SHACL Reasoner", rules: "reasoner", data: "reasoner" },
  postgres: { label: "PostgreSQL Views", rules: "postgres", data: "postgres" },
};

export const DEFAULT_BACKEND = "reasoner";

export function isValidBackend(b) {
  return Object.prototype.hasOwnProperty.call(BACKENDS, b);
}

// Parse a backend token into { rules, data }. Accepts:
//   "reasoner" / "postgres"      — a named pair from BACKENDS
//   "postgres:reasoner"          — an explicit rules:data cross-run (easter egg)
// Unknown/missing -> the default pair.
export function pairOf(token) {
  const t = (token || "").toString().trim();
  if (t.includes(":")) {
    const [rules, data] = t.split(":");
    if (RULES[rules] && DATA[data]) return { rules, data };
  }
  if (BACKENDS[t]) return { rules: BACKENDS[t].rules, data: BACKENDS[t].data };
  return { rules: BACKENDS[DEFAULT_BACKEND].rules, data: BACKENDS[DEFAULT_BACKEND].data };
}

// Resolve the backend pair from a request. The login page stores the choice and
// api.js attaches it as X-ERB-Backend; we also accept ?backend= for manual
// testing / side-by-side panes.
export function backendOf(req) {
  const fromHeader = req.get && req.get("X-ERB-Backend");
  const fromQuery = req.query && req.query.backend;
  // We return the RULES name as the "backend" for the common code paths that
  // only care which engine computes (compute/read). Callers that need the data
  // axis (the store layer) call dataOf(req). This keeps existing call sites
  // working while the data axis is wired through store.js.
  return pairOf(fromHeader || fromQuery).rules;
}

// The DATA-store axis of the request (which raw store to read/write).
export function dataOf(req) {
  const fromHeader = req.get && req.get("X-ERB-Backend");
  const fromQuery = req.query && req.query.backend;
  return pairOf(fromHeader || fromQuery).data;
}

// The one call read handlers make: run the chosen RULES engine over already-read
// raw rows. Throws on any substrate failure — no fallback to the other engine,
// no empty graph. A 500 here is a true 500.
export async function reason(rows, rules = DEFAULT_BACKEND) {
  if (!RULES[rules]) throw new Error(`unknown rules engine '${rules}'`);
  return RULES[rules].run(rows);
}
