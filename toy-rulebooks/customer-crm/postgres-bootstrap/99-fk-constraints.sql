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

-- Orders
ALTER TABLE orders DROP CONSTRAINT IF EXISTS fk_orders_customer;
ALTER TABLE orders ADD CONSTRAINT fk_orders_customer
  FOREIGN KEY (customer) REFERENCES customers (customers_id);

-- Payments
ALTER TABLE payments DROP CONSTRAINT IF EXISTS fk_payments_order_id;
ALTER TABLE payments ADD CONSTRAINT fk_payments_order_id
  FOREIGN KEY (order_id) REFERENCES orders (orders_id);

-- FlightControlSystems
ALTER TABLE flight_control_systems DROP CONSTRAINT IF EXISTS fk_flight_control_systems_jet_model_id;
ALTER TABLE flight_control_systems ADD CONSTRAINT fk_flight_control_systems_jet_model_id
  FOREIGN KEY (jet_model_id) REFERENCES jet_models (jet_models_id);

-- OrderLines
ALTER TABLE order_lines DROP CONSTRAINT IF EXISTS fk_order_lines_order_id;
ALTER TABLE order_lines ADD CONSTRAINT fk_order_lines_order_id
  FOREIGN KEY (order_id) REFERENCES orders (orders_id);
ALTER TABLE order_lines DROP CONSTRAINT IF EXISTS fk_order_lines_fcs_id;
ALTER TABLE order_lines ADD CONSTRAINT fk_order_lines_fcs_id
  FOREIGN KEY (fcs_id) REFERENCES flight_control_systems (flight_control_systems_id);

-- 5 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
