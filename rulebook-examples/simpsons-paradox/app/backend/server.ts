import express from 'express';
import cors from 'cors';
import path from 'path';
import { fileURLToPath } from 'url';
import { query } from './db.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const app = express();
const PORT = process.env.PORT ? parseInt(process.env.PORT) : 3001;

app.use(cors());
app.use(express.json());

// --- Studies ---

app.get('/api/studies', async (_req, res) => {
  const rows = await query('SELECT * FROM vw_studies ORDER BY study_id');
  res.json(rows);
});

app.get('/api/studies/:id', async (req, res) => {
  const rows = await query(
    'SELECT * FROM vw_studies WHERE study_id = $1',
    [req.params.id]
  );
  if (!rows.length) { res.status(404).json({ error: 'not found' }); return; }
  res.json(rows[0]);
});

// --- Strata ---

app.get('/api/strata', async (req, res) => {
  const { study } = req.query;
  if (study) {
    const rows = await query(
      'SELECT * FROM vw_strata WHERE study = $1 ORDER BY stratum_id',
      [study]
    );
    res.json(rows);
  } else {
    const rows = await query('SELECT * FROM vw_strata ORDER BY stratum_id');
    res.json(rows);
  }
});

// --- Stratum summaries ---

app.get('/api/stratum-summaries', async (req, res) => {
  const { study } = req.query;
  if (study) {
    const rows = await query(
      'SELECT * FROM vw_stratum_summaries WHERE study = $1 ORDER BY stratum_summary_id',
      [study]
    );
    res.json(rows);
  } else {
    const rows = await query(
      'SELECT * FROM vw_stratum_summaries ORDER BY stratum_summary_id'
    );
    res.json(rows);
  }
});

// --- Treatment rankings ---

app.get('/api/treatment-rankings', async (req, res) => {
  const { study } = req.query;
  if (study) {
    const rows = await query(
      'SELECT * FROM vw_treatment_rankings WHERE study = $1 ORDER BY treatment_ranking_id',
      [study]
    );
    res.json(rows);
  } else {
    const rows = await query(
      'SELECT * FROM vw_treatment_rankings ORDER BY treatment_ranking_id'
    );
    res.json(rows);
  }
});

// --- Treatments ---

app.get('/api/treatments', async (req, res) => {
  const { study } = req.query;
  if (study) {
    const rows = await query(
      'SELECT * FROM vw_treatments WHERE study = $1 ORDER BY treatment_id',
      [study]
    );
    res.json(rows);
  } else {
    const rows = await query('SELECT * FROM vw_treatments ORDER BY treatment_id');
    res.json(rows);
  }
});

// --- Model summary ---

app.get('/api/model-summary', async (_req, res) => {
  const rows = await query('SELECT * FROM vw_model_summary LIMIT 1');
  res.json(rows[0] ?? null);
});

// --- UI screens / components (from rulebook) ---

app.get('/api/ui-screens', async (_req, res) => {
  const rows = await query('SELECT * FROM vw_ui_screens ORDER BY ui_screen_id');
  res.json(rows);
});

app.get('/api/ui-components', async (_req, res) => {
  const rows = await query('SELECT * FROM vw_ui_components ORDER BY ui_component_id');
  res.json(rows);
});

// --- Methodology / Conclusions ---

app.get('/api/methodology', async (_req, res) => {
  const rows = await query('SELECT * FROM vw_methodology ORDER BY methodology_id');
  res.json(rows);
});

app.get('/api/conclusions', async (_req, res) => {
  const rows = await query('SELECT * FROM vw_conclusions ORDER BY conclusion_id');
  res.json(rows);
});

// Serve the standalone explorer HTML from the project root
const projectRoot = path.resolve(__dirname, '../../..');
app.get('/simpsons-paradox-explorer.html', (_req, res) => {
  res.sendFile(path.join(projectRoot, 'simpsons-paradox-explorer.html'));
});

app.listen(PORT, () => {
  console.log(`[simpsons-paradox api] http://localhost:${PORT}`);
});
