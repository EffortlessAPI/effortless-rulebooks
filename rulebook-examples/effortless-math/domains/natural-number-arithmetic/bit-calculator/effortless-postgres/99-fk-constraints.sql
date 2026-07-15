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

-- GateTruthRows
ALTER TABLE gate_truth_rows DROP CONSTRAINT IF EXISTS fk_gate_truth_rows_gate_type_id;
ALTER TABLE gate_truth_rows ADD CONSTRAINT fk_gate_truth_rows_gate_type_id
  FOREIGN KEY (gate_type_id) REFERENCES gate_types (gate_type_id);

-- Computations
ALTER TABLE computations DROP CONSTRAINT IF EXISTS fk_computations_gate_type_id;
ALTER TABLE computations ADD CONSTRAINT fk_computations_gate_type_id
  FOREIGN KEY (gate_type_id) REFERENCES gate_types (gate_type_id);

-- 2 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
