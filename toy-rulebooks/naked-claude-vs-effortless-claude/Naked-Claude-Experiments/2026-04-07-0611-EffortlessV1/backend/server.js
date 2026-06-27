import express from 'express';
import cors from 'cors';
import pg from 'pg';

const { Pool } = pg;
const pool = new Pool({
  database: process.env.PGDATABASE || 'effortless-demo',
});

const app = express();
app.use(cors());
app.use(express.json());

function slugify(s) {
  return s
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '') || 'customer';
}

// List all customers (alphabetical)
app.get('/api/customers', async (_req, res, next) => {
  try {
    const { rows } = await pool.query(
      'SELECT id, handle, name, notes, color, is_stopped FROM vw_customers ORDER BY name ASC'
    );
    res.json(rows);
  } catch (e) { next(e); }
});

// Summary: totals, stopped count, color breakdown
app.get('/api/summary', async (_req, res, next) => {
  try {
    const total = await pool.query('SELECT COUNT(*)::int AS n FROM vw_customers');
    const stopped = await pool.query('SELECT COUNT(*)::int AS n FROM vw_customers WHERE is_stopped');
    const breakdown = await pool.query(
      `SELECT color, COUNT(*)::int AS n
         FROM vw_customers
         GROUP BY color
         ORDER BY color`
    );
    const byColor = Object.fromEntries(breakdown.rows.map(r => [r.color || '(none)', r.n]));
    res.json({
      total: total.rows[0].n,
      stopped: stopped.rows[0].n,
      by_color: byColor,
      green: byColor.Green || 0,
      red: byColor.Red || 0,
      yellow: byColor.Yellow || 0,
      blue: byColor.Blue || 0,
    });
  } catch (e) { next(e); }
});

// Get one customer
app.get('/api/customers/:id', async (req, res, next) => {
  try {
    const { rows } = await pool.query(
      'SELECT id, handle, name, notes, color, is_stopped FROM vw_customers WHERE id = $1',
      [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'not found' });
    res.json(rows[0]);
  } catch (e) { next(e); }
});

// Create
app.post('/api/customers', async (req, res, next) => {
  try {
    const { name, notes = '', color = '' } = req.body || {};
    if (!name || !name.trim()) return res.status(400).json({ error: 'name is required' });

    // Generate a unique handle
    let base = (req.body.handle && req.body.handle.trim()) || slugify(name);
    let handle = base;
    let i = 2;
    // eslint-disable-next-line no-constant-condition
    while (true) {
      const { rows } = await pool.query('SELECT 1 FROM customers WHERE handle = $1', [handle]);
      if (!rows.length) break;
      handle = `${base}-${i++}`;
    }

    const { rows } = await pool.query(
      `INSERT INTO customers (handle, name, notes, color)
       VALUES ($1, $2, $3, $4)
       RETURNING id`,
      [handle, name.trim(), notes, color]
    );
    const id = rows[0].id;
    const fresh = await pool.query(
      'SELECT id, handle, name, notes, color, is_stopped FROM vw_customers WHERE id = $1',
      [id]
    );
    res.status(201).json(fresh.rows[0]);
  } catch (e) { next(e); }
});

// Update
app.put('/api/customers/:id', async (req, res, next) => {
  try {
    const { name, notes, color } = req.body || {};
    const { rows: existing } = await pool.query('SELECT id FROM customers WHERE id = $1', [req.params.id]);
    if (!existing.length) return res.status(404).json({ error: 'not found' });

    await pool.query(
      `UPDATE customers
          SET name  = COALESCE($2, name),
              notes = COALESCE($3, notes),
              color = COALESCE($4, color)
        WHERE id = $1`,
      [req.params.id, name ?? null, notes ?? null, color ?? null]
    );
    const { rows } = await pool.query(
      'SELECT id, handle, name, notes, color, is_stopped FROM vw_customers WHERE id = $1',
      [req.params.id]
    );
    res.json(rows[0]);
  } catch (e) { next(e); }
});

// Delete
app.delete('/api/customers/:id', async (req, res, next) => {
  try {
    const { rowCount } = await pool.query('DELETE FROM customers WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'not found' });
    res.status(204).end();
  } catch (e) { next(e); }
});

app.use((err, _req, res, _next) => {
  console.error(err);
  res.status(500).json({ error: err.message });
});

const PORT = process.env.PORT || 4000;
app.listen(PORT, () => console.log(`API on http://localhost:${PORT}`));
