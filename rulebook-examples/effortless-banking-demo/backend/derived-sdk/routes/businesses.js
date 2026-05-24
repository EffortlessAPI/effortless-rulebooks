import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_businesses ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_businesses WHERE business_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { legal_name, business_structure, naics_code, naics_description, annual_revenue_usd, status, onboarded_at } = req.body;
    const r = await pool.query(
      'INSERT INTO businesses (legal_name, business_structure, naics_code, naics_description, annual_revenue_usd, status, onboarded_at) VALUES ($1, $2, $3, $4, $5, $6, $7) RETURNING *',
      [legal_name, business_structure, naics_code, naics_description, annual_revenue_usd, status, onboarded_at]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { legal_name, business_structure, naics_code, naics_description, annual_revenue_usd, status, onboarded_at } = req.body;
    const r = await pool.query(
      'UPDATE businesses SET legal_name = $1, business_structure = $2, naics_code = $3, naics_description = $4, annual_revenue_usd = $5, status = $6, onboarded_at = $7 WHERE business_id = $8 RETURNING *',
      [legal_name, business_structure, naics_code, naics_description, annual_revenue_usd, status, onboarded_at, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM businesses WHERE business_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
