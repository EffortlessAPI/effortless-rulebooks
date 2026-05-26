import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_accounts ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_accounts WHERE account_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { account_type, account_number_last4, current_balance_usd, has_ach, has_wire, has_card, opened_at } = req.body;
    const r = await pool.query(
      'INSERT INTO accounts (account_type, account_number_last4, current_balance_usd, has_ach, has_wire, has_card, opened_at) VALUES ($1, $2, $3, $4, $5, $6, $7) RETURNING *',
      [account_type, account_number_last4, current_balance_usd, has_ach, has_wire, has_card, opened_at]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { account_type, account_number_last4, current_balance_usd, has_ach, has_wire, has_card, opened_at } = req.body;
    const r = await pool.query(
      'UPDATE accounts SET account_type = $1, account_number_last4 = $2, current_balance_usd = $3, has_ach = $4, has_wire = $5, has_card = $6, opened_at = $7 WHERE account_id = $8 RETURNING *',
      [account_type, account_number_last4, current_balance_usd, has_ach, has_wire, has_card, opened_at, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM accounts WHERE account_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
