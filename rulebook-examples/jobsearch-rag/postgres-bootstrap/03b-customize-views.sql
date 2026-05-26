-- ============================================================================
-- CUSTOMIZE VIEWS - User-defined views customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main views script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom views changes will appear here:

-- ============================================================================
-- RESUME VIEWS
-- ============================================================================

CREATE OR REPLACE VIEW vw_resumes WITH (security_invoker = ON) AS
SELECT
  t.resume_id,
  t.name,
  t.description,
  t.file_path,
  t.indexed_at,
  calc_resumes_resume_sections(t.resume_id) AS resume_sections,
  calc_resumes_search_runs(t.resume_id) AS search_runs
FROM resumes t;

CREATE OR REPLACE VIEW vw_resume_sections WITH (security_invoker = ON) AS
SELECT
  t.resume_section_id,
  t.name,
  t.resume,
  t.section_order,
  t.content,
  t.chroma_doc_id,
  calc_resume_sections_resume_name(t.resume_section_id) AS resume_name
FROM resume_sections t;

-- Recreate vw_search_runs to include the resume FK and resume_name lookup
CREATE OR REPLACE VIEW vw_search_runs WITH (security_invoker = ON) AS
SELECT
  t.search_run_id,
  t.name,
  t.resume,
  calc_search_runs_resume_name(t.search_run_id) AS resume_name,
  t.run_date,
  t.archetype_weight,
  t.fit_weight,
  t.history_weight,
  t.comp_weight,
  t.negative_weight,
  t.culture_weight,
  t.base_salary,
  t.min_score_threshold,
  t.disqualify_on_llm_flag,
  t.total_found,
  t.total_scored,
  t.total_excluded,
  t.total_deduplicated,
  t.failed_listings,
  calc_search_runs_score_results(t.search_run_id) AS score_results
FROM search_runs t;

