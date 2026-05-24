import express from 'express';
import pg from 'pg';

const { Pool } = pg;

const pool = new Pool({
  host: process.env.PGHOST || 'localhost',
  user: process.env.PGUSER || 'postgres',
  password: process.env.PGPASSWORD || 'postgres',
  database: process.env.PGDATABASE || 'effortless-demo',
  port: Number(process.env.PGPORT || 5432),
});

const app = express();

// List user views (prefer vw_* views; fall back to base tables)
app.get('/api/tables', async (_req, res) => {
  try {
    const { rows } = await pool.query(`
      SELECT table_name
      FROM information_schema.tables
      WHERE table_schema = 'public'
      ORDER BY
        CASE WHEN table_name LIKE 'vw\\_%' THEN 0 ELSE 1 END,
        table_name
    `);
    res.json(rows.map(r => r.table_name));
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

// Fetch rows from a table/view (with safe identifier quoting)
app.get('/api/tables/:name', async (req, res) => {
  const { name } = req.params;
  if (!/^[A-Za-z_][A-Za-z0-9_]*$/.test(name)) {
    return res.status(400).json({ error: 'invalid table name' });
  }
  try {
    const { rows, fields } = await pool.query(`SELECT * FROM "${name}" LIMIT 200`);
    res.json({
      columns: fields.map(f => f.name),
      rows,
    });
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

const PORT = process.env.PORT || 3001;
app.listen(PORT, () => {
  console.log(`API listening on http://localhost:${PORT}`);
});
