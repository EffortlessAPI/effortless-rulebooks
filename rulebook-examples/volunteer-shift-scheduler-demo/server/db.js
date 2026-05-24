import pg from 'pg';

const { Pool } = pg;

export const pool = new Pool({
  database: process.env.PGDATABASE || 'volunteer_shift_scheduler',
  host: process.env.PGHOST || 'localhost',
  port: Number(process.env.PGPORT) || 5432,
  user: process.env.PGUSER || process.env.USER,
});

export async function q(text, params) {
  const res = await pool.query(text, params);
  return res.rows;
}

export async function q1(text, params) {
  const rows = await q(text, params);
  return rows[0] || null;
}
