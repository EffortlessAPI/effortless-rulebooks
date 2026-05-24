const { Pool } = require('pg');

const pool = new Pool({
  host: 'localhost',
  database: '2026-04-20-1639-naked',
  user: process.env.PGUSER || process.env.USER || 'postgres',
  password: process.env.PGPASSWORD || '',
  port: parseInt(process.env.PGPORT || '5432', 10),
});

module.exports = pool;
