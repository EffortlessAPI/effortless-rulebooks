const express = require('express');
const cors    = require('cors');
const path    = require('path');
const pool    = require('./db');

const app = express();
app.use(cors());
app.use(express.json());

// ─── Helpers ────────────────────────────────────────────────────────────────

function generateSlug(name) {
  return name
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');
}

// SELECT expression that derives is_stopped from color
const SELECT_COLS = `
  id,
  name,
  slug,
  notes,
  color,
  (color = 'Green') AS is_stopped,
  created_at
`;

// ─── API Routes ──────────────────────────────────────────────────────────────

// Summary stats
app.get('/api/summary', async (req, res) => {
  try {
    const { rows } = await pool.query(`
      SELECT
        COUNT(*)                                      AS total,
        COUNT(*) FILTER (WHERE color = 'Green')       AS stopped,
        COUNT(*) FILTER (WHERE color = 'Green')       AS green,
        COUNT(*) FILTER (WHERE color = 'Red')         AS red,
        COUNT(*) FILTER (WHERE color = 'Yellow')      AS yellow,
        COUNT(*) FILTER (WHERE color = 'Blue')        AS blue,
        COUNT(*) FILTER (WHERE color NOT IN ('Green','Red','Yellow','Blue') AND color <> '') AS other
      FROM customers
    `);
    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// List all customers (alphabetical by name)
app.get('/api/customers', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `SELECT ${SELECT_COLS} FROM customers ORDER BY name ASC`
    );
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// Get one customer
app.get('/api/customers/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `SELECT ${SELECT_COLS} FROM customers WHERE id = $1`,
      [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Customer not found' });
    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// Create a customer
app.post('/api/customers', async (req, res) => {
  try {
    const { name, notes = '', color = '' } = req.body;
    if (!name || !name.trim()) {
      return res.status(400).json({ error: 'Name is required' });
    }

    // Generate a unique slug
    const baseSlug = generateSlug(name.trim());
    let slug = baseSlug;
    let attempt = 1;
    while (true) {
      const { rows: existing } = await pool.query(
        'SELECT id FROM customers WHERE slug = $1', [slug]
      );
      if (!existing.length) break;
      slug = `${baseSlug}-${attempt++}`;
    }

    const { rows } = await pool.query(
      `INSERT INTO customers (name, slug, notes, color)
       VALUES ($1, $2, $3, $4)
       RETURNING ${SELECT_COLS}`,
      [name.trim(), slug, notes, color]
    );
    res.status(201).json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// Update a customer
app.put('/api/customers/:id', async (req, res) => {
  try {
    const { rows: existing } = await pool.query(
      'SELECT * FROM customers WHERE id = $1', [req.params.id]
    );
    if (!existing.length) return res.status(404).json({ error: 'Customer not found' });

    const current = existing[0];
    const name  = req.body.name  !== undefined ? req.body.name.trim() : current.name;
    const notes = req.body.notes !== undefined ? req.body.notes       : current.notes;
    const color = req.body.color !== undefined ? req.body.color       : current.color;

    if (!name) return res.status(400).json({ error: 'Name is required' });

    const { rows } = await pool.query(
      `UPDATE customers SET name = $1, notes = $2, color = $3
       WHERE id = $4
       RETURNING ${SELECT_COLS}`,
      [name, notes, color, req.params.id]
    );
    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// Delete a customer
app.delete('/api/customers/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'DELETE FROM customers WHERE id = $1 RETURNING id', [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Customer not found' });
    res.json({ success: true, id: rows[0].id });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ─── Serve built frontend ───────────────────────────────────────────────────

const DIST = path.join(__dirname, '..', 'client', 'dist');
app.use(express.static(DIST));
app.get('*', (req, res) => {
  res.sendFile(path.join(DIST, 'index.html'));
});

// ─── Start ──────────────────────────────────────────────────────────────────

const PORT = parseInt(process.env.PORT || '3000', 10);
app.listen(PORT, () => {
  console.log(`Customer Tracker server listening on http://localhost:${PORT}`);
});
