import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_beneficial_owners ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_beneficial_owners WHERE beneficial_owner_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { full_name, date_of_birth_encrypted, ssn_encrypted, address_encrypted, ownership_percentage, is_control_person } = req.body;
    const r = await pool.query(
      'INSERT INTO beneficial_owners (full_name, date_of_birth_encrypted, ssn_encrypted, address_encrypted, ownership_percentage, is_control_person) VALUES ($1, $2, $3, $4, $5, $6) RETURNING *',
      [full_name, date_of_birth_encrypted, ssn_encrypted, address_encrypted, ownership_percentage, is_control_person]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { full_name, date_of_birth_encrypted, ssn_encrypted, address_encrypted, ownership_percentage, is_control_person } = req.body;
    const r = await pool.query(
      'UPDATE beneficial_owners SET full_name = $1, date_of_birth_encrypted = $2, ssn_encrypted = $3, address_encrypted = $4, ownership_percentage = $5, is_control_person = $6 WHERE beneficial_owner_id = $7 RETURNING *',
      [full_name, date_of_birth_encrypted, ssn_encrypted, address_encrypted, ownership_percentage, is_control_person, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM beneficial_owners WHERE beneficial_owner_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
