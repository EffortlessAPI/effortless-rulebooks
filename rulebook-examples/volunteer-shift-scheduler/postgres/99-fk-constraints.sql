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

-- Shifts
ALTER TABLE shifts DROP CONSTRAINT IF EXISTS fk_shifts_event;
ALTER TABLE shifts ADD CONSTRAINT fk_shifts_event
  FOREIGN KEY (event) REFERENCES events (event_id);

-- Assignments
ALTER TABLE assignments DROP CONSTRAINT IF EXISTS fk_assignments_volunteer;
ALTER TABLE assignments ADD CONSTRAINT fk_assignments_volunteer
  FOREIGN KEY (volunteer) REFERENCES volunteers (volunteer_id);
ALTER TABLE assignments DROP CONSTRAINT IF EXISTS fk_assignments_shift;
ALTER TABLE assignments ADD CONSTRAINT fk_assignments_shift
  FOREIGN KEY (shift) REFERENCES shifts (shift_id);

-- 3 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
