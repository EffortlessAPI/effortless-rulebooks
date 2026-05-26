import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_covenants ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_covenants WHERE covenant_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { covenant_type, threshold_value, test_frequency, next_test_date, status, current_waiver_through } = req.body;
    const r = await pool.query(
      'INSERT INTO covenants (covenant_type, threshold_value, test_frequency, next_test_date, status, current_waiver_through) VALUES ($1, $2, $3, $4, $5, $6) RETURNING *',
      [covenant_type, threshold_value, test_frequency, next_test_date, status, current_waiver_through]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { covenant_type, threshold_value, test_frequency, next_test_date, status, current_waiver_through } = req.body;
    const r = await pool.query(
      'UPDATE covenants SET covenant_type = $1, threshold_value = $2, test_frequency = $3, next_test_date = $4, status = $5, current_waiver_through = $6 WHERE covenant_id = $7 RETURNING *',
      [covenant_type, threshold_value, test_frequency, next_test_date, status, current_waiver_through, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM covenants WHERE covenant_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
