import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_risk_rating_history ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_risk_rating_history WHERE risk_rating_history_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { effective_date, prior_grade, new_grade, reason } = req.body;
    const r = await pool.query(
      'INSERT INTO risk_rating_history (effective_date, prior_grade, new_grade, reason) VALUES ($1, $2, $3, $4) RETURNING *',
      [effective_date, prior_grade, new_grade, reason]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { effective_date, prior_grade, new_grade, reason } = req.body;
    const r = await pool.query(
      'UPDATE risk_rating_history SET effective_date = $1, prior_grade = $2, new_grade = $3, reason = $4 WHERE risk_rating_history_id = $5 RETURNING *',
      [effective_date, prior_grade, new_grade, reason, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM risk_rating_history WHERE risk_rating_history_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
