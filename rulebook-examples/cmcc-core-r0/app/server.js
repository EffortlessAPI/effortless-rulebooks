import express from 'express';
import pg from 'pg';

const { Pool } = pg;

// Database default is derived from the SSoT: project slug cmcc-core-r0 -> erb_cmcc_core_r0.
const pool = new Pool({
  host: process.env.PGHOST || 'localhost',
  user: process.env.PGUSER || 'postgres',
  password: process.env.PGPASSWORD || 'postgres',
  database: process.env.PGDATABASE || 'erb_cmcc_core_r0',
  port: Number(process.env.PGPORT || 5432),
});

const app = express();
const VIEW_NAME = /^vw_[a-z0-9_]+$/;

// The view IS the contract: the app reads ONLY vw_* views. If there are none,
// the rulebook-to-postgres build has not been loaded into this database.
app.get('/api/views', async (_req, res) => {
  try {
    const { rows } = await pool.query(`
      SELECT table_name
      FROM information_schema.views
      WHERE table_schema = 'public' AND table_name LIKE 'vw\\_%'
      ORDER BY table_name
    `);
    if (rows.length === 0) {
      return res.status(500).json({
        error: `No vw_* views found in database ${pool.options.database}. Run ./init-db.sh to load postgres/*.sql.`,
      });
    }
    res.json(rows.map(r => r.table_name));
  } catch (e) {
    res.status(500).json({ error: `database ${pool.options.database} unreachable: ${e.message}` });
  }
});

app.get('/api/views/:name', async (req, res) => {
  const { name } = req.params;
  if (!VIEW_NAME.test(name)) {
    return res.status(400).json({ error: `invalid view name "${name}"; expected ^vw_[a-z0-9_]+$` });
  }
  try {
    const { rows, fields } = await pool.query(`SELECT * FROM "${name}"`);
    res.json({ columns: fields.map(f => f.name), rows });
  } catch (e) {
    res.status(500).json({ error: `SELECT * FROM "${name}" failed: ${e.message}` });
  }
});

// Domain route: the conformance matrix, joined server-side because the UI
// wants substrate x condition/input-history cells, not three flat tables.
// Every value still comes straight from vw_substrates / vw_answer_key_results
// / vw_reflection_results -- this only reshapes rows, it computes nothing.
app.get('/api/conformance-matrix', async (_req, res) => {
  try {
    const substrates = await pool.query(
      'SELECT * FROM vw_substrates ORDER BY substrate_id'
    );
    const inputHistories = await pool.query(
      'SELECT * FROM vw_input_histories ORDER BY input_history_id'
    );
    const conditions = await pool.query(
      'SELECT * FROM vw_reflection_conditions ORDER BY condition_number'
    );
    const answerKey = await pool.query('SELECT * FROM vw_answer_key_results');
    const reflection = await pool.query('SELECT * FROM vw_reflection_results');
    res.json({
      substrates: substrates.rows,
      inputHistories: inputHistories.rows,
      conditions: conditions.rows,
      answerKeyResults: answerKey.rows,
      reflectionResults: reflection.rows,
    });
  } catch (e) {
    res.status(500).json({ error: `conformance matrix query failed: ${e.message}` });
  }
});

const PORT = Number(process.env.PORT || 43306);
app.listen(PORT, () => {
  console.log(`API listening on http://localhost:${PORT} (database ${pool.options.database})`);
});
