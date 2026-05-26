-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- SearchUrls
ALTER TABLE search_urls DROP CONSTRAINT IF EXISTS fk_search_urls_job_board;
ALTER TABLE search_urls ADD CONSTRAINT fk_search_urls_job_board
  FOREIGN KEY (job_board) REFERENCES job_boards (job_board_id);

-- ResumeSections
ALTER TABLE resume_sections DROP CONSTRAINT IF EXISTS fk_resume_sections_resume;
ALTER TABLE resume_sections ADD CONSTRAINT fk_resume_sections_resume
  FOREIGN KEY (resume) REFERENCES resumes (resume_id);

-- SearchRuns
ALTER TABLE search_runs DROP CONSTRAINT IF EXISTS fk_search_runs_resume;
ALTER TABLE search_runs ADD CONSTRAINT fk_search_runs_resume
  FOREIGN KEY (resume) REFERENCES resumes (resume_id);

-- JobListings
ALTER TABLE job_listings DROP CONSTRAINT IF EXISTS fk_job_listings_job_board;
ALTER TABLE job_listings ADD CONSTRAINT fk_job_listings_job_board
  FOREIGN KEY (job_board) REFERENCES job_boards (job_board_id);

-- ScoreResults
ALTER TABLE score_results DROP CONSTRAINT IF EXISTS fk_score_results_job_listing;
ALTER TABLE score_results ADD CONSTRAINT fk_score_results_job_listing
  FOREIGN KEY (job_listing) REFERENCES job_listings (job_listing_id);
ALTER TABLE score_results DROP CONSTRAINT IF EXISTS fk_score_results_search_run;
ALTER TABLE score_results ADD CONSTRAINT fk_score_results_search_run
  FOREIGN KEY (search_run) REFERENCES search_runs (search_run_id);

-- Decisions
ALTER TABLE decisions DROP CONSTRAINT IF EXISTS fk_decisions_job_listing;
ALTER TABLE decisions ADD CONSTRAINT fk_decisions_job_listing
  FOREIGN KEY (job_listing) REFERENCES job_listings (job_listing_id);

-- 7 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
