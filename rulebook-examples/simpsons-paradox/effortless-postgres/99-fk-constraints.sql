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

-- Treatments
ALTER TABLE treatments DROP CONSTRAINT IF EXISTS fk_treatments_study;
ALTER TABLE treatments ADD CONSTRAINT fk_treatments_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- Strata
ALTER TABLE strata DROP CONSTRAINT IF EXISTS fk_strata_study;
ALTER TABLE strata ADD CONSTRAINT fk_strata_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- CaseCells
ALTER TABLE case_cells DROP CONSTRAINT IF EXISTS fk_case_cells_study;
ALTER TABLE case_cells ADD CONSTRAINT fk_case_cells_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- 3 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
