const express = require("express");
const cors = require("cors");
const { Pool } = require("pg");

const app = express();
app.use(cors());
app.use(express.json());

const pool = new Pool({
  connectionString:
    process.env.DATABASE_URL || "postgresql://postgres@localhost:5432/demo",
});

// ---------------------------------------------------------------------------
// Table / view metadata – drives both API routes and the React UI
// ---------------------------------------------------------------------------
const TABLES = [
  {
    key: "job_boards",
    label: "Job Boards",
    view: "vw_job_boards",
    table: "job_boards",
    pk: "job_board_id",
    columns: [
      "job_board_id",
      "name",
      "description",
      "is_enabled",
      "max_pages",
      "is_headless",
      "browser_channel",
    ],
    viewExtra: ["job_listings"],
  },
  {
    key: "search_urls",
    label: "Search URLs",
    view: "vw_search_urls",
    table: "search_urls",
    pk: "search_url_id",
    columns: ["search_url_id", "name", "job_board", "url", "is_enabled"],
    viewExtra: ["job_board_name"],
  },
  {
    key: "role_archetypes",
    label: "Role Archetypes",
    view: "vw_role_archetypes",
    table: "role_archetypes",
    pk: "role_archetype_id",
    columns: [
      "role_archetype_id",
      "name",
      "description",
      "signals_positive",
      "signals_negative",
    ],
    viewExtra: [],
  },
  {
    key: "rubric_dimensions",
    label: "Rubric Dimensions",
    view: "vw_rubric_dimensions",
    table: "rubric_dimensions",
    pk: "rubric_dimension_id",
    columns: [
      "rubric_dimension_id",
      "name",
      "description",
      "signals_positive",
      "signals_negative",
    ],
    viewExtra: [],
  },
  {
    key: "resumes",
    label: "Resumes",
    view: "vw_resumes",
    table: "resumes",
    pk: "resume_id",
    columns: ["resume_id", "name", "description", "file_path", "indexed_at"],
    viewExtra: ["resume_sections", "search_runs"],
  },
  {
    key: "resume_sections",
    label: "Resume Sections",
    view: "vw_resume_sections",
    table: "resume_sections",
    pk: "resume_section_id",
    columns: [
      "resume_section_id",
      "name",
      "resume",
      "section_order",
      "content",
      "chroma_doc_id",
    ],
    viewExtra: ["resume_name"],
  },
  {
    key: "search_runs",
    label: "Search Runs",
    view: "vw_search_runs",
    table: "search_runs",
    pk: "search_run_id",
    columns: [
      "search_run_id",
      "name",
      "resume",
      "run_date",
      "archetype_weight",
      "fit_weight",
      "history_weight",
      "comp_weight",
      "negative_weight",
      "culture_weight",
      "base_salary",
      "min_score_threshold",
      "disqualify_on_llm_flag",
      "total_found",
      "total_scored",
      "total_excluded",
      "total_deduplicated",
      "failed_listings",
    ],
    viewExtra: ["resume_name", "score_results"],
  },
  {
    key: "job_listings",
    label: "Job Listings",
    view: "vw_job_listings",
    table: "job_listings",
    pk: "job_listing_id",
    columns: [
      "job_listing_id",
      "name",
      "job_board",
      "external_id",
      "company",
      "location",
      "url",
      "posted_at",
      "comp_min",
      "comp_max",
      "comp_source",
      "comp_text",
    ],
    viewExtra: ["job_board_name", "score_results", "decisions"],
  },
  {
    key: "score_results",
    label: "Score Results",
    view: "vw_score_results",
    table: "score_results",
    pk: "score_result_id",
    columns: [
      "score_result_id",
      "job_listing",
      "search_run",
      "fit_score",
      "archetype_score",
      "history_score",
      "comp_score",
      "negative_score",
      "culture_score",
      "is_disqualified",
      "disqualifier_reason",
      "duplicate_boards",
    ],
    viewExtra: [
      "name",
      "final_score",
      "job_listing_name",
      "search_run_name",
      "company",
    ],
  },
  {
    key: "decisions",
    label: "Decisions",
    view: "vw_decisions",
    table: "decisions",
    pk: "decision_id",
    columns: [
      "decision_id",
      "job_listing",
      "verdict",
      "reason",
      "recorded_at",
    ],
    viewExtra: [
      "name",
      "is_scoring_signal",
      "job_listing_name",
      "company",
      "board",
    ],
  },
];

// ---------------------------------------------------------------------------
// Metadata endpoint – the React app fetches this once to build the UI
// ---------------------------------------------------------------------------
app.get("/api/meta", (_req, res) => {
  res.json(TABLES);
});

// ---------------------------------------------------------------------------
// Generic CRUD for every table
// ---------------------------------------------------------------------------
// LIST  – GET  /api/:tableKey
// GET   – GET  /api/:tableKey/:id
// CREATE – POST /api/:tableKey
// UPDATE – PUT  /api/:tableKey/:id
// DELETE – DELETE /api/:tableKey/:id
// ---------------------------------------------------------------------------

function findMeta(key) {
  return TABLES.find((t) => t.key === key);
}

// LIST (reads from the view so calculated/lookup fields are included)
app.get("/api/:tableKey", async (req, res) => {
  const meta = findMeta(req.params.tableKey);
  if (!meta) return res.status(404).json({ error: "Unknown table" });
  try {
    const { rows } = await pool.query(
      `SELECT * FROM ${meta.view} ORDER BY ${meta.pk} LIMIT 1000`
    );
    res.json(rows);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// GET single row
app.get("/api/:tableKey/:id", async (req, res) => {
  const meta = findMeta(req.params.tableKey);
  if (!meta) return res.status(404).json({ error: "Unknown table" });
  try {
    const { rows } = await pool.query(
      `SELECT * FROM ${meta.view} WHERE ${meta.pk} = $1`,
      [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: "Not found" });
    res.json(rows[0]);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// CREATE
app.post("/api/:tableKey", async (req, res) => {
  const meta = findMeta(req.params.tableKey);
  if (!meta) return res.status(404).json({ error: "Unknown table" });
  try {
    const cols = meta.columns.filter((c) => req.body[c] !== undefined);
    const vals = cols.map((c) => req.body[c]);
    const placeholders = cols.map((_, i) => `$${i + 1}`);
    const sql = `INSERT INTO ${meta.table} (${cols.join(", ")}) VALUES (${placeholders.join(", ")}) RETURNING *`;
    const { rows } = await pool.query(sql, vals);
    res.status(201).json(rows[0]);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// UPDATE
app.put("/api/:tableKey/:id", async (req, res) => {
  const meta = findMeta(req.params.tableKey);
  if (!meta) return res.status(404).json({ error: "Unknown table" });
  try {
    const cols = meta.columns.filter(
      (c) => c !== meta.pk && req.body[c] !== undefined
    );
    if (!cols.length) return res.status(400).json({ error: "Nothing to update" });
    const sets = cols.map((c, i) => `${c} = $${i + 1}`);
    const vals = cols.map((c) => req.body[c]);
    vals.push(req.params.id);
    const sql = `UPDATE ${meta.table} SET ${sets.join(", ")} WHERE ${meta.pk} = $${vals.length} RETURNING *`;
    const { rows } = await pool.query(sql, vals);
    if (!rows.length) return res.status(404).json({ error: "Not found" });
    res.json(rows[0]);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// DELETE
app.delete("/api/:tableKey/:id", async (req, res) => {
  const meta = findMeta(req.params.tableKey);
  if (!meta) return res.status(404).json({ error: "Unknown table" });
  try {
    const { rowCount } = await pool.query(
      `DELETE FROM ${meta.table} WHERE ${meta.pk} = $1`,
      [req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: "Not found" });
    res.json({ deleted: true });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ---------------------------------------------------------------------------
const PORT = process.env.PORT || 3001;
app.listen(PORT, () => console.log(`Admin API listening on :${PORT}`));
