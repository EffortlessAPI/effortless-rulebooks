import { Router } from 'express';
import { pool } from '../db.js';

const router = Router();

router.get('/', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_documents ORDER BY name ASC');
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/:id', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_documents WHERE document_id = $1', [req.params.id]);
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/', async (req, res) => {
  try {
    const { filename, document_type, ocr_indexed, uploaded_via, uploaded_at } = req.body;
    const r = await pool.query(
      'INSERT INTO documents (filename, document_type, ocr_indexed, uploaded_via, uploaded_at) VALUES ($1, $2, $3, $4, $5) RETURNING *',
      [filename, document_type, ocr_indexed, uploaded_via, uploaded_at]
    );
    res.status(201).json(r.rows[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.put('/:id', async (req, res) => {
  try {
    const { filename, document_type, ocr_indexed, uploaded_via, uploaded_at } = req.body;
    const r = await pool.query(
      'UPDATE documents SET filename = $1, document_type = $2, ocr_indexed = $3, uploaded_via = $4, uploaded_at = $5 WHERE document_id = $6 RETURNING *',
      [filename, document_type, ocr_indexed, uploaded_via, uploaded_at, req.params.id]
    );
    r.rows.length ? res.json(r.rows[0]) : res.sendStatus(404);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.delete('/:id', async (req, res) => {
  try {
    await pool.query('DELETE FROM documents WHERE document_id = $1', [req.params.id]);
    res.sendStatus(204);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

export default router;
