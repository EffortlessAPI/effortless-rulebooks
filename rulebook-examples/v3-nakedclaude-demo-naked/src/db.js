import pg from 'pg';

const connectionString = process.env.DATABASE_URL || 'postgres:///naked_claude_naked_test';

export const pool = new pg.Pool({ connectionString });

export async function query(text, params) {
  return pool.query(text, params);
}
