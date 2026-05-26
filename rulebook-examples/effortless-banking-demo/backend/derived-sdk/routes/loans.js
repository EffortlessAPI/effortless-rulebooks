import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_loans ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_loans WHERE loan_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { loan_number, loan_purpose, principal_usd, rate_pct, term_months, underwriting_stage, risk_rating, risk_rating_label, dscr, ltv, global_cash_flow_usd, originated_at, funded_at } = req.body;
    const r = await pool.query(
      'INSERT INTO loans (loan_number, loan_purpose, principal_usd, rate_pct, term_months, underwriting_stage, risk_rating, risk_rating_label, dscr, ltv, global_cash_flow_usd, originated_at, funded_at) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13) RETURNING *',
      [loan_number, loan_purpose, principal_usd, rate_pct, term_months, underwriting_stage, risk_rating, risk_rating_label, dscr, ltv, global_cash_flow_usd, originated_at, funded_at]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { loan_number, loan_purpose, principal_usd, rate_pct, term_months, underwriting_stage, risk_rating, risk_rating_label, dscr, ltv, global_cash_flow_usd, originated_at, funded_at } = req.body;
    const r = await pool.query(
      'UPDATE loans SET loan_number = $1, loan_purpose = $2, principal_usd = $3, rate_pct = $4, term_months = $5, underwriting_stage = $6, risk_rating = $7, risk_rating_label = $8, dscr = $9, ltv = $10, global_cash_flow_usd = $11, originated_at = $12, funded_at = $13 WHERE loan_id = $14 RETURNING *',
      [loan_number, loan_purpose, principal_usd, rate_pct, term_months, underwriting_stage, risk_rating, risk_rating_label, dscr, ltv, global_cash_flow_usd, originated_at, funded_at, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM loans WHERE loan_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
