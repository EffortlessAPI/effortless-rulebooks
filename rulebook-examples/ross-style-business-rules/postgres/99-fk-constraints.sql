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

-- Claimants
ALTER TABLE claimants DROP CONSTRAINT IF EXISTS fk_claimants_claimant_name;
ALTER TABLE claimants ADD CONSTRAINT fk_claimants_claimant_name
  FOREIGN KEY (claimant_name) REFERENCES claimants (claimant_id);
ALTER TABLE claimants DROP CONSTRAINT IF EXISTS fk_claimants_policy;
ALTER TABLE claimants ADD CONSTRAINT fk_claimants_policy
  FOREIGN KEY (policy) REFERENCES policies (policy_id);

-- Claims
ALTER TABLE claims DROP CONSTRAINT IF EXISTS fk_claims_incident;
ALTER TABLE claims ADD CONSTRAINT fk_claims_incident
  FOREIGN KEY (incident) REFERENCES incidents (incident_id);

-- 3 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
