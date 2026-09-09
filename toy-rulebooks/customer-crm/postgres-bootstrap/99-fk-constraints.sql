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

-- Users
ALTER TABLE users DROP CONSTRAINT IF EXISTS fk_users_email;
ALTER TABLE users ADD CONSTRAINT fk_users_email
  FOREIGN KEY (email) REFERENCES users (users_id);

-- Customers
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_email;
ALTER TABLE customers ADD CONSTRAINT fk_customers_email
  FOREIGN KEY (email) REFERENCES customers (customers_id);

-- Orders
ALTER TABLE orders DROP CONSTRAINT IF EXISTS fk_orders_order_number;
ALTER TABLE orders ADD CONSTRAINT fk_orders_order_number
  FOREIGN KEY (order_number) REFERENCES orders (orders_id);
ALTER TABLE orders DROP CONSTRAINT IF EXISTS fk_orders_customer;
ALTER TABLE orders ADD CONSTRAINT fk_orders_customer
  FOREIGN KEY (customer) REFERENCES customers (customers_id);

-- Payments
ALTER TABLE payments DROP CONSTRAINT IF EXISTS fk_payments_payment_number;
ALTER TABLE payments ADD CONSTRAINT fk_payments_payment_number
  FOREIGN KEY (payment_number) REFERENCES payments (payments_id);
ALTER TABLE payments DROP CONSTRAINT IF EXISTS fk_payments_order_id;
ALTER TABLE payments ADD CONSTRAINT fk_payments_order_id
  FOREIGN KEY (order_id) REFERENCES orders (orders_id);

-- JetModels
ALTER TABLE jet_models DROP CONSTRAINT IF EXISTS fk_jet_models_model_code;
ALTER TABLE jet_models ADD CONSTRAINT fk_jet_models_model_code
  FOREIGN KEY (model_code) REFERENCES jet_models (jet_models_id);

-- FlightControlSystems
ALTER TABLE flight_control_systems DROP CONSTRAINT IF EXISTS fk_flight_control_systems_fcs_code;
ALTER TABLE flight_control_systems ADD CONSTRAINT fk_flight_control_systems_fcs_code
  FOREIGN KEY (fcs_code) REFERENCES flight_control_systems (flight_control_systems_id);
ALTER TABLE flight_control_systems DROP CONSTRAINT IF EXISTS fk_flight_control_systems_jet_model_id;
ALTER TABLE flight_control_systems ADD CONSTRAINT fk_flight_control_systems_jet_model_id
  FOREIGN KEY (jet_model_id) REFERENCES jet_models (jet_models_id);

-- OrderLines
ALTER TABLE order_lines DROP CONSTRAINT IF EXISTS fk_order_lines_line_number;
ALTER TABLE order_lines ADD CONSTRAINT fk_order_lines_line_number
  FOREIGN KEY (line_number) REFERENCES order_lines (order_lines_id);
ALTER TABLE order_lines DROP CONSTRAINT IF EXISTS fk_order_lines_order_id;
ALTER TABLE order_lines ADD CONSTRAINT fk_order_lines_order_id
  FOREIGN KEY (order_id) REFERENCES orders (orders_id);
ALTER TABLE order_lines DROP CONSTRAINT IF EXISTS fk_order_lines_fcs_id;
ALTER TABLE order_lines ADD CONSTRAINT fk_order_lines_fcs_id
  FOREIGN KEY (fcs_id) REFERENCES flight_control_systems (flight_control_systems_id);

-- 12 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
