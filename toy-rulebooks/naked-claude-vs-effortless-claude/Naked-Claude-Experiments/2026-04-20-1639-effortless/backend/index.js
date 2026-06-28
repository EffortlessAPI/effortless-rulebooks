import express from 'express';
import cors from 'cors';
import pg from 'pg';

const { Pool } = pg;

const app = express();
const PORT = process.env.PORT || 3001;

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/2026-04-20-1639-effortless'
});

app.use(cors());
app.use(express.json());

// GET all customers
app.get('/api/customers', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_customers ORDER BY name');
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// GET summary stats
app.get('/api/summary', async (req, res) => {
  try {
    const result = await pool.query(`
      SELECT
        COUNT(*) AS total,
        COUNT(*) FILTER (WHERE is_stopped = true) AS stopped,
        COUNT(*) FILTER (WHERE current_color = 'Green') AS green,
        COUNT(*) FILTER (WHERE current_color = 'Red') AS red,
        COUNT(*) FILTER (WHERE current_color = 'Yellow') AS yellow,
        COUNT(*) FILTER (WHERE current_color = 'Blue') AS blue,
        COUNT(*) FILTER (WHERE current_color NOT IN ('Green','Red','Yellow','Blue') AND current_color IS NOT NULL) AS other
      FROM vw_customers
    `);
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// GET single customer
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

// POST create customer
app.post('/api/customers', async (req, res) => {
  const { name, notes, current_color } = req.body;
  if (!name) return res.status(400).json({ error: 'Name is required' });

  // Generate slug from name
  const customer_id = name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
  
  // Ensure uniqueness by appending a timestamp if needed
  try {
    const existing = await pool.query('SELECT customer_id FROM customers WHERE customer_id = $1', [customer_id]);
    let finalId = customer_id;
    if (existing.rows.length > 0) {
      finalId = `${customer_id}-${Date.now()}`;
    }

    const result = await pool.query(
      'INSERT INTO customers (customer_id, name, notes, current_color) VALUES ($1, $2, $3, $4) RETURNING *',
      [finalId, name, notes || null, current_color || null]
    );

    // Return the view version (with is_stopped)
    const viewResult = await pool.query(
      'SELECT * FROM vw_customers WHERE customer_id = $1',
      [finalId]
    );
    res.status(201).json(viewResult.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// PUT update customer
app.put('/api/customers/:id', async (req, res) => {
  const { name, notes, current_color } = req.body;
  try {
    await pool.query(
      'UPDATE customers SET name = $1, notes = $2, current_color = $3 WHERE customer_id = $4',
      [name, notes !== undefined ? notes : null, current_color || null, req.params.id]
    );

    const viewResult = await pool.query(
      'SELECT * FROM vw_customers WHERE customer_id = $1',
      [req.params.id]
    );
    if (viewResult.rows.length === 0) return res.status(404).json({ error: 'Not found' });
    res.json(viewResult.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// DELETE customer
app.delete('/api/customers/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'DELETE FROM customers WHERE customer_id = $1',
      [req.params.id]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.listen(PORT, () => {
  console.log(`Backend running on http://localhost:${PORT}`);
});
