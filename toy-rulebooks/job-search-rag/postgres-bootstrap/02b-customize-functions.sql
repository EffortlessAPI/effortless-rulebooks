-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined functions customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main functions script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom functions changes will appear here:

-- ============================================================================
-- MISSING LOOKUP FUNCTIONS
-- These were not generated in 02-create-functions.sql but are referenced
-- by views in 03-create-views.sql
-- ============================================================================

-- SearchUrls: lookup JobBoard name via FK
CREATE OR REPLACE FUNCTION calc_search_urls_job_board_name(p_search_url_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jb.name FROM job_boards jb
          JOIN search_urls su ON su.job_board = jb.job_board_id
          WHERE su.search_url_id = p_search_url_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- JobListings: lookup JobBoard name via FK
CREATE OR REPLACE FUNCTION calc_job_listings_job_board_name(p_job_listing_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jb.name FROM job_boards jb
          JOIN job_listings jl ON jl.job_board = jb.job_board_id
          WHERE jl.job_listing_id = p_job_listing_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ScoreResults: lookup JobListing name via FK
CREATE OR REPLACE FUNCTION calc_score_results_job_listing_name(p_score_result_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jl.name FROM job_listings jl
          JOIN score_results sr ON sr.job_listing = jl.job_listing_id
          WHERE sr.score_result_id = p_score_result_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ScoreResults: lookup SearchRun name via FK
CREATE OR REPLACE FUNCTION calc_score_results_search_run_name(p_score_result_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT r.name FROM search_runs r
          JOIN score_results sr ON sr.search_run = r.search_run_id
          WHERE sr.score_result_id = p_score_result_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ScoreResults: lookup Company via JobListing FK
CREATE OR REPLACE FUNCTION calc_score_results_company(p_score_result_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jl.company FROM job_listings jl
          JOIN score_results sr ON sr.job_listing = jl.job_listing_id
          WHERE sr.score_result_id = p_score_result_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- Decisions: lookup JobListing name via FK
CREATE OR REPLACE FUNCTION calc_decisions_job_listing_name(p_decision_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jl.name FROM job_listings jl
          JOIN decisions d ON d.job_listing = jl.job_listing_id
          WHERE d.decision_id = p_decision_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- Decisions: lookup Company via JobListing FK
CREATE OR REPLACE FUNCTION calc_decisions_company(p_decision_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jl.company FROM job_listings jl
          JOIN decisions d ON d.job_listing = jl.job_listing_id
          WHERE d.decision_id = p_decision_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- Decisions: lookup Board via JobListing FK
CREATE OR REPLACE FUNCTION calc_decisions_board(p_decision_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT jl.job_board FROM job_listings jl
          JOIN decisions d ON d.job_listing = jl.job_listing_id
          WHERE d.decision_id = p_decision_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ============================================================================
-- RESUME FUNCTIONS
-- ============================================================================

-- Resumes: count of sections
CREATE OR REPLACE FUNCTION calc_resumes_resume_sections(p_resume_id TEXT)
RETURNS INTEGER AS $$
BEGIN
  RETURN (SELECT COUNT(*) FROM resume_sections WHERE resume = p_resume_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- Resumes: count of search runs
CREATE OR REPLACE FUNCTION calc_resumes_search_runs(p_resume_id TEXT)
RETURNS INTEGER AS $$
BEGIN
  RETURN (SELECT COUNT(*) FROM search_runs WHERE resume = p_resume_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- Resumes: getter for name (used by lookups)
CREATE OR REPLACE FUNCTION get_resumes_name(p_resume_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT name FROM resumes WHERE resume_id = p_resume_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ResumeSections: lookup resume name via FK
CREATE OR REPLACE FUNCTION calc_resume_sections_resume_name(p_resume_section_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT r.name FROM resumes r
          JOIN resume_sections rs ON rs.resume = r.resume_id
          WHERE rs.resume_section_id = p_resume_section_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- SearchRuns: lookup resume name via FK
CREATE OR REPLACE FUNCTION calc_search_runs_resume_name(p_search_run_id TEXT)
RETURNS TEXT AS $$
BEGIN
  RETURN (SELECT r.name FROM resumes r
          JOIN search_runs sr ON sr.resume = r.resume_id
          WHERE sr.search_run_id = p_search_run_id);
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

-- ============================================================================
-- FIX: calc_score_results_final_score
-- The generated version failed to translate MAX(a, b) → GREATEST(a, b)
-- ============================================================================
CREATE OR REPLACE FUNCTION calc_score_results_final_score(p_score_result_id TEXT)
RETURNS NUMERIC AS $$
DECLARE
  rec score_results%ROWTYPE;
BEGIN
  SELECT * INTO rec FROM score_results WHERE score_result_id = p_score_result_id;
  IF rec.is_disqualified THEN
    RETURN 0;
  END IF;
  RETURN GREATEST(0,
    rec.fit_score + rec.archetype_score + rec.history_score
    + rec.comp_score + rec.culture_score - rec.negative_score
  );
END;
$$ LANGUAGE plpgsql STABLE SECURITY DEFINER;

