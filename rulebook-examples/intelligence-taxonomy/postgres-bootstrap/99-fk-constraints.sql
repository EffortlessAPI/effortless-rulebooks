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

-- Assessments
ALTER TABLE assessments DROP CONSTRAINT IF EXISTS fk_assessments_intelligence;
ALTER TABLE assessments ADD CONSTRAINT fk_assessments_intelligence
  FOREIGN KEY (intelligence) REFERENCES intelligences (intelligences_id);
ALTER TABLE assessments DROP CONSTRAINT IF EXISTS fk_assessments_capability;
ALTER TABLE assessments ADD CONSTRAINT fk_assessments_capability
  FOREIGN KEY (capability) REFERENCES capabilities (capabilities_id);

-- 2 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
