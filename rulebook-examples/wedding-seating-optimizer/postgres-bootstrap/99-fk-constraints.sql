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

-- Guests
ALTER TABLE guests DROP CONSTRAINT IF EXISTS fk_guests_assigned_table;
ALTER TABLE guests ADD CONSTRAINT fk_guests_assigned_table
  FOREIGN KEY (assigned_table) REFERENCES tables (table_id);

-- Relationships
ALTER TABLE relationships DROP CONSTRAINT IF EXISTS fk_relationships_guest_a;
ALTER TABLE relationships ADD CONSTRAINT fk_relationships_guest_a
  FOREIGN KEY (guest_a) REFERENCES guests (guest_id);
ALTER TABLE relationships DROP CONSTRAINT IF EXISTS fk_relationships_guest_b;
ALTER TABLE relationships ADD CONSTRAINT fk_relationships_guest_b
  FOREIGN KEY (guest_b) REFERENCES guests (guest_id);

-- 3 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
