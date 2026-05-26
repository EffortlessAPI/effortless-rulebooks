import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_interactions ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_interactions WHERE interaction_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { interaction_type, subject, body, interaction_date, due_date, source } = req.body;
    const r = await pool.query(
      'INSERT INTO interactions (interaction_type, subject, body, interaction_date, due_date, source) VALUES ($1, $2, $3, $4, $5, $6) RETURNING *',
      [interaction_type, subject, body, interaction_date, due_date, source]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { interaction_type, subject, body, interaction_date, due_date, source } = req.body;
    const r = await pool.query(
      'UPDATE interactions SET interaction_type = $1, subject = $2, body = $3, interaction_date = $4, due_date = $5, source = $6 WHERE interaction_id = $7 RETURNING *',
      [interaction_type, subject, body, interaction_date, due_date, source, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM interactions WHERE interaction_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
