const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const app = express();
const PORT = process.env.PORT || 3001;

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/effortless-rulebook-demo'
});

app.use(cors());
app.use(express.json());

// GET /api/summary - summary stats from the view
app.get('/api/summary', async (req, res) => {
  try {
    const result = await pool.query(`
      SELECT
        COUNT(*)::int                                          AS total,
        COUNT(*) FILTER (WHERE is_stopped = true)::int        AS stopped,
        COUNT(*) FILTER (WHERE current_color = 'Green')::int  AS green,
        COUNT(*) FILTER (WHERE current_color = 'Red')::int    AS red,
        COUNT(*) FILTER (WHERE current_color = 'Yellow')::int AS yellow,
        COUNT(*) FILTER (WHERE current_color = 'Blue')::int   AS blue,
        COUNT(*) FILTER (WHERE current_color NOT IN ('Green','Red','Yellow','Blue') AND current_color IS NOT NULL)::int AS other
      FROM vw_customers
    `);
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// GET /api/customers - all customers from view, alphabetical
app.get('/api/customers', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_customers ORDER BY name ASC'
    );
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// GET /api/customers/:id
app.get('/api/customers/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_customers WHERE customer_id = $1',
      [req.params.id]
    );
    if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// POST /api/customers - create a new customer
app.post('/api/customers', async (req, res) => {
  const { customer_id, name, notes, current_color } = req.body;
  try {
    await pool.query(
      'INSERT INTO customers (customer_id, name, notes, current_color) VALUES ($1, $2, $3, $4)',
      [customer_id, name, notes || '', current_color]
    );
    const result = await pool.query(
      'SELECT * FROM vw_customers WHERE customer_id = $1',
      [customer_id]
    );
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// PUT /api/customers/:id - update a customer
app.put('/api/customers/:id', async (req, res) => {
  const { name, notes, current_color } = req.body;
  try {
    const upd = await pool.query(
      'UPDATE customers SET name = $1, notes = $2, current_color = $3 WHERE customer_id = $4',
      [name, notes || '', current_color, req.params.id]
    );
    if (upd.rowCount === 0) return res.status(404).json({ error: 'Not found' });
    const result = await pool.query(
      'SELECT * FROM vw_customers WHERE customer_id = $1',
      [req.params.id]
    );
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// DELETE /api/customers/:id
app.delete('/api/customers/:id', async (req, res) => {
  try {
    const del = await pool.query(
      'DELETE FROM customers WHERE customer_id = $1',
      [req.params.id]
    );
    if (del.rowCount === 0) return res.status(404).json({ error: 'Not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.listen(PORT, () => {
  console.log(`Backend listening on http://localhost:${PORT}`);
});
