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

// List vw_* views. Per ERB methodology the app MUST read computed values from
// views, not base tables (base tables don't include calculated fields). If no
// views exist, something has gone off the rails — fail loudly, do not list
// base tables.
app.get('/api/tables', async (_req, res) => {
  try {
    const { rows } = await pool.query(`
      SELECT table_name
      FROM information_schema.views
      WHERE table_schema = 'public' AND table_name LIKE 'vw\\_%'
      ORDER BY table_name
    `);
    if (rows.length === 0) {
      return res.status(500).json({
        error: 'No vw_* views found in postgres. The rulebook-to-postgres build has not run, or it produced no views.'
      });
    }
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
