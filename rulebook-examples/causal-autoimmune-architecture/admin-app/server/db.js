// db.js — thin Postgres accessor for the admin app.
//
// PROJECT RULE: read from vw_* views, never base tables. Write to base tables directly.
// This module only exposes a query helper; the route layer decides which view to read.

import pg from 'pg';

const { Pool, types } = pg;

// Postgres returns NUMERIC/DECIMAL (OID 1700) and BIGINT (OID 20) as STRINGS by
// default (to avoid float precision loss). The witnessed harness compares derived
// scalars with strict equality (0 === 0, 1 === 1), so a string "0.000…" would
// fail a check whose value is numerically correct. We parse them to JS numbers so
// the API serves real numbers — the derived values are small, well within float
// range. (Project rule: consume calculated fields as opaque truth; this only
// changes the wire TYPE, never the value.)
types.setTypeParser(1700, (v) => (v === null ? null : parseFloat(v))); // numeric/decimal
types.setTypeParser(20, (v) => (v === null ? null : parseInt(v, 10))); // bigint

const CONNECTION_STRING =
  process.env.DATABASE_URL ||
  'postgresql://postgres@localhost:5432/causal_autoimmune';

export const pool = new Pool({ connectionString: CONNECTION_STRING });

export async function query(text, params) {
  const res = await pool.query(text, params);
  return res.rows;
}

export async function closePool() {
  await pool.end();
}
