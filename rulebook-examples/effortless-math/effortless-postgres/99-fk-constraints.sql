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

-- LegacyParentAudits
ALTER TABLE legacy_parent_audits DROP CONSTRAINT IF EXISTS fk_legacy_parent_audits_previous_status;
ALTER TABLE legacy_parent_audits ADD CONSTRAINT fk_legacy_parent_audits_previous_status
  FOREIGN KEY (previous_status) REFERENCES proof_statuses (proof_status_id);

-- 1 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
