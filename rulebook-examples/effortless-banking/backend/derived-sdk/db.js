import pg from 'pg';
const { Pool } = pg;
export const pool = new Pool({ connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/effortless_banking' });
