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

-- Transactions
ALTER TABLE transactions DROP CONSTRAINT IF EXISTS fk_transactions_product;
ALTER TABLE transactions ADD CONSTRAINT fk_transactions_product
  FOREIGN KEY (product) REFERENCES products (product_id);
ALTER TABLE transactions DROP CONSTRAINT IF EXISTS fk_transactions_transaction_type;
ALTER TABLE transactions ADD CONSTRAINT fk_transactions_transaction_type
  FOREIGN KEY (transaction_type) REFERENCES transaction_types (transaction_type_id);

-- 2 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
