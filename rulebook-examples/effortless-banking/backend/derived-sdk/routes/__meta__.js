import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw___meta__ ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw___meta__ WHERE __meta___id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { meta_key, value_type, string_value, json_value } = req.body;
    const r = await pool.query(
      'INSERT INTO __meta__ (meta_key, value_type, string_value, json_value) VALUES ($1, $2, $3, $4) RETURNING *',
      [meta_key, value_type, string_value, json_value]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { meta_key, value_type, string_value, json_value } = req.body;
    const r = await pool.query(
      'UPDATE __meta__ SET meta_key = $1, value_type = $2, string_value = $3, json_value = $4 WHERE __meta___id = $5 RETURNING *',
      [meta_key, value_type, string_value, json_value, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM __meta__ WHERE __meta___id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
