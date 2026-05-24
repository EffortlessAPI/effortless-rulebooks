import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_contacts ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_contacts WHERE contact_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { full_name, title, email, phone, contact_type, is_authorized_signer } = req.body;
    const r = await pool.query(
      'INSERT INTO contacts (full_name, title, email, phone, contact_type, is_authorized_signer) VALUES ($1, $2, $3, $4, $5, $6) RETURNING *',
      [full_name, title, email, phone, contact_type, is_authorized_signer]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { full_name, title, email, phone, contact_type, is_authorized_signer } = req.body;
    const r = await pool.query(
      'UPDATE contacts SET full_name = $1, title = $2, email = $3, phone = $4, contact_type = $5, is_authorized_signer = $6 WHERE contact_id = $7 RETURNING *',
      [full_name, title, email, phone, contact_type, is_authorized_signer, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM contacts WHERE contact_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
