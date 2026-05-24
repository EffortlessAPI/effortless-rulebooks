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

-- Products
ALTER TABLE products DROP CONSTRAINT IF EXISTS fk_products_sku;
ALTER TABLE products ADD CONSTRAINT fk_products_sku
  FOREIGN KEY (sku) REFERENCES products (product_id);

-- Orders
ALTER TABLE orders DROP CONSTRAINT IF EXISTS fk_orders_customer;
ALTER TABLE orders ADD CONSTRAINT fk_orders_customer
  FOREIGN KEY (customer) REFERENCES customers (customer_id);

-- OrderLineItems
ALTER TABLE order_line_items DROP CONSTRAINT IF EXISTS fk_order_line_items_order;
ALTER TABLE order_line_items ADD CONSTRAINT fk_order_line_items_order
  FOREIGN KEY ("order") REFERENCES orders (order_id);
ALTER TABLE order_line_items DROP CONSTRAINT IF EXISTS fk_order_line_items_product;
ALTER TABLE order_line_items ADD CONSTRAINT fk_order_line_items_product
  FOREIGN KEY (product) REFERENCES products (product_id);

-- Payments
ALTER TABLE payments DROP CONSTRAINT IF EXISTS fk_payments_order;
ALTER TABLE payments ADD CONSTRAINT fk_payments_order
  FOREIGN KEY ("order") REFERENCES orders (order_id);

-- 7 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
