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

-- Customers
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_status;
ALTER TABLE customers ADD CONSTRAINT fk_customers_status
  FOREIGN KEY (status) REFERENCES statuses (statuse_id);

-- Statuses
ALTER TABLE statuses DROP CONSTRAINT IF EXISTS fk_statuses_display_name;
ALTER TABLE statuses ADD CONSTRAINT fk_statuses_display_name
  FOREIGN KEY (display_name) REFERENCES statuses (statuse_id);
ALTER TABLE statuses DROP CONSTRAINT IF EXISTS fk_statuses_customers;
ALTER TABLE statuses ADD CONSTRAINT fk_statuses_customers
  FOREIGN KEY (customers) REFERENCES customers (customer_id);

-- 3 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
