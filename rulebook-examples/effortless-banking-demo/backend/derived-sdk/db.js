import pg from 'pg';
const { Pool } = pg;
export const pool = new Pool({ connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/small_business_banking_client_manager' });
