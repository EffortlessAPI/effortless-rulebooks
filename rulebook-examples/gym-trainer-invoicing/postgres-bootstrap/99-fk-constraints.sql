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

-- Clients
ALTER TABLE clients DROP CONSTRAINT IF EXISTS fk_clients_trainer;
ALTER TABLE clients ADD CONSTRAINT fk_clients_trainer
  FOREIGN KEY (trainer) REFERENCES trainers (trainer_id);

-- Invoices
ALTER TABLE invoices DROP CONSTRAINT IF EXISTS fk_invoices_client;
ALTER TABLE invoices ADD CONSTRAINT fk_invoices_client
  FOREIGN KEY (client) REFERENCES clients (client_id);

-- Sessions
ALTER TABLE sessions DROP CONSTRAINT IF EXISTS fk_sessions_client;
ALTER TABLE sessions ADD CONSTRAINT fk_sessions_client
  FOREIGN KEY (client) REFERENCES clients (client_id);
ALTER TABLE sessions DROP CONSTRAINT IF EXISTS fk_sessions_invoice;
ALTER TABLE sessions ADD CONSTRAINT fk_sessions_invoice
  FOREIGN KEY (invoice) REFERENCES invoices (invoice_id);

-- 4 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
