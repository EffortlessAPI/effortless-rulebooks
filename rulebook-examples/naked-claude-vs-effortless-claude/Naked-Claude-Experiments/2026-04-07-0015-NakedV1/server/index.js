import express from 'express';
import pg from 'pg';

const { Pool } = pg;
const pool = new Pool({
  database: process.env.PGDATABASE || 'effortless-demo',
  host: process.env.PGHOST || 'localhost',
  port: Number(process.env.PGPORT || 5432),
  user: process.env.PGUSER || 'postgres'
});

const app = express();
app.use(express.json());

// Map a DB row to API shape, deriving is_stopped from color.
function shape(row) {
  return {
    id: row.id,
    slug: row.slug,
    name: row.name,
    notes: row.notes ?? '',
    color: row.color ?? '',
    is_stopped: row.color === 'Green'
  };
}

function slugify(s) {
  return String(s)
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '') || 'customer';
}

app.get('/api/customers', async (_req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT id, slug, name, notes, color FROM naked_v1.customers ORDER BY name ASC'
    );
    res.json(rows.map(shape));
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.get('/api/summary', async (_req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT color FROM naked_v1.customers'
    );
    const total = rows.length;
    const stopped = rows.filter(r => r.color === 'Green').length;
    const colors = {};
    for (const r of rows) {
      const c = r.color || '(none)';
      colors[c] = (colors[c] || 0) + 1;
    }
    res.json({ total, stopped, colors });
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.get('/api/customers/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT id, slug, name, notes, color FROM naked_v1.customers WHERE id = $1',
      [req.params.id]
    );
    if (rows.length === 0) return res.status(404).json({ error: 'not found' });
    res.json(shape(rows[0]));
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.post('/api/customers', async (req, res) => {
  try {
    const name = String(req.body.name || '').trim();
    if (!name) return res.status(400).json({ error: 'name is required' });
    const notes = String(req.body.notes || '');
    const color = String(req.body.color || '');
    let slug = String(req.body.slug || '').trim();
    if (!slug) slug = slugify(name);

    // Ensure unique slug
    let unique = slug;
    let n = 2;
    while (true) {
      const { rows } = await pool.query(
        'SELECT 1 FROM naked_v1.customers WHERE slug = $1',
        [unique]
      );
      if (rows.length === 0) break;
      unique = `${slug}-${n++}`;
    }

    const { rows } = await pool.query(
      `INSERT INTO naked_v1.customers (slug, name, notes, color)
       VALUES ($1, $2, $3, $4)
       RETURNING id, slug, name, notes, color`,
      [unique, name, notes, color]
    );
    res.status(201).json(shape(rows[0]));
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.put('/api/customers/:id', async (req, res) => {
  try {
    const { rows: existing } = await pool.query(
      'SELECT id, slug, name, notes, color FROM naked_v1.customers WHERE id = $1',
      [req.params.id]
    );
    if (existing.length === 0) return res.status(404).json({ error: 'not found' });
    const cur = existing[0];

    const name = req.body.name !== undefined ? String(req.body.name) : cur.name;
    const notes = req.body.notes !== undefined ? String(req.body.notes) : cur.notes;
    const color = req.body.color !== undefined ? String(req.body.color) : cur.color;

    const { rows } = await pool.query(
      `UPDATE naked_v1.customers
       SET name = $1, notes = $2, color = $3
       WHERE id = $4
       RETURNING id, slug, name, notes, color`,
      [name, notes, color, req.params.id]
    );
    res.json(shape(rows[0]));
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.delete('/api/customers/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query(
      'DELETE FROM naked_v1.customers WHERE id = $1',
      [req.params.id]
    );
    if (rowCount === 0) return res.status(404).json({ error: 'not found' });
    res.status(204).end();
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

const PORT = Number(process.env.PORT || 8787);
app.listen(PORT, () => {
  console.log(`naked-v1 server listening on http://localhost:${PORT}`);
});
