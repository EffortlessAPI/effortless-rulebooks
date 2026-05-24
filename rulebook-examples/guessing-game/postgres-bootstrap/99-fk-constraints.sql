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

-- 边
ALTER TABLE 边 DROP CONSTRAINT IF EXISTS fk_边_形状;
ALTER TABLE 边 ADD CONSTRAINT fk_边_形状
  FOREIGN KEY (形状) REFERENCES 形状 (形状id);

-- 1 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
