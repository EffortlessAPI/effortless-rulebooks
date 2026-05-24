const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const app = express();
const PORT = process.env.PORT || 3001;

// PostgreSQL connection pool
const pool = new Pool({
  database: process.env.PGDATABASE || 'effortless-demo',
  host: process.env.PGHOST || 'localhost',
  port: process.env.PGPORT || 5432,
  user: process.env.PGUSER || 'postgres',
});

app.use(cors());
app.use(express.json());

// Helper: slugify a name
function slugify(name) {
  return name
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');
}

// Helper: ensure unique slug
async function uniqueSlug(baseName, excludeId = null) {
  const base = slugify(baseName);
  let candidate = base || 'customer';
  let i = 2;
  while (true) {
    const params = excludeId
      ? [candidate, excludeId]
      : [candidate];
    const query = excludeId
      ? 'SELECT id FROM customers WHERE slug = $1 AND id != $2 LIMIT 1'
      : 'SELECT id FROM customers WHERE slug = $1 LIMIT 1';
    const { rows } = await pool.query(query, params);
    if (rows.length === 0) return candidate;
    candidate = `${base}-${i++}`;
  }
}

// GET /api/customers — list all customers alphabetically
app.get('/api/customers', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT id, name, slug, notes, color, stopped, created_at, updated_at FROM customers ORDER BY name ASC'
    );
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// GET /api/summary — counts and color distribution
app.get('/api/summary', async (req, res) => {
  try {
    const { rows } = await pool.query(`
      SELECT
        COUNT(*)::int                              AS total,
        COUNT(*) FILTER (WHERE stopped)::int       AS stopped,
        COUNT(*) FILTER (WHERE color = 'Green')::int  AS green,
        COUNT(*) FILTER (WHERE color = 'Red')::int    AS red,
        COUNT(*) FILTER (WHERE color = 'Yellow')::int AS yellow,
        COUNT(*) FILTER (WHERE color = 'Blue')::int   AS blue
      FROM customers
    `);
    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// GET /api/customers/:id
app.get('/api/customers/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT id, name, slug, notes, color, stopped, created_at, updated_at FROM customers WHERE id = $1',
      [req.params.id]
    );
    if (rows.length === 0) return res.status(404).json({ error: 'Not found' });
    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// POST /api/customers — create
app.post('/api/customers', async (req, res) => {
  try {
    const { name, notes = '', color = '' } = req.body;
    if (!name || !name.trim()) {
      return res.status(400).json({ error: 'name is required' });
    }
    const slug = await uniqueSlug(name);
    const { rows } = await pool.query(
      `INSERT INTO customers (name, slug, notes, color)
       VALUES ($1, $2, $3, $4)
       RETURNING id, name, slug, notes, color, stopped, created_at, updated_at`,
      [name.trim(), slug, notes, color]
    );
    res.status(201).json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// PUT /api/customers/:id — update
app.put('/api/customers/:id', async (req, res) => {
  try {
    const { name, notes, color } = req.body;
    const id = req.params.id;

    // Get current customer
    const current = await pool.query('SELECT * FROM customers WHERE id = $1', [id]);
    if (current.rows.length === 0) return res.status(404).json({ error: 'Not found' });
    const c = current.rows[0];

    const newName  = name  !== undefined ? name.trim()  : c.name;
    const newNotes = notes !== undefined ? notes         : c.notes;
    const newColor = color !== undefined ? color         : c.color;

    // Regenerate slug only if name changed
    let newSlug = c.slug;
    if (name !== undefined && name.trim() !== c.name) {
      newSlug = await uniqueSlug(newName, id);
    }

    const { rows } = await pool.query(
      `UPDATE customers
       SET name = $1, slug = $2, notes = $3, color = $4, updated_at = NOW()
       WHERE id = $5
       RETURNING id, name, slug, notes, color, stopped, created_at, updated_at`,
      [newName, newSlug, newNotes, newColor, id]
    );
    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// DELETE /api/customers/:id
app.delete('/api/customers/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query(
      'DELETE FROM customers WHERE id = $1',
      [req.params.id]
    );
    if (rowCount === 0) return res.status(404).json({ error: 'Not found' });
    res.json({ ok: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.listen(PORT, () => {
  console.log(`Customer Tracker API listening on port ${PORT}`);
});
