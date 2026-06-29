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
ALTER TABLE clients DROP CONSTRAINT IF EXISTS fk_clients_category;
ALTER TABLE clients ADD CONSTRAINT fk_clients_category
  FOREIGN KEY (category) REFERENCES client_categories (client_categorie_id);
ALTER TABLE clients DROP CONSTRAINT IF EXISTS fk_clients_status;
ALTER TABLE clients ADD CONSTRAINT fk_clients_status
  FOREIGN KEY (status) REFERENCES statuses (statuse_id);
ALTER TABLE clients DROP CONSTRAINT IF EXISTS fk_clients_addresses;
ALTER TABLE clients ADD CONSTRAINT fk_clients_addresses
  FOREIGN KEY (addresses) REFERENCES addresses (addresse_id);

-- ClientApprovals
ALTER TABLE client_approvals DROP CONSTRAINT IF EXISTS fk_client_approvals_client;
ALTER TABLE client_approvals ADD CONSTRAINT fk_client_approvals_client
  FOREIGN KEY (client) REFERENCES clients (client_id);
ALTER TABLE client_approvals DROP CONSTRAINT IF EXISTS fk_client_approvals_approved_by;
ALTER TABLE client_approvals ADD CONSTRAINT fk_client_approvals_approved_by
  FOREIGN KEY (approved_by) REFERENCES app_users (app_user_id);

-- Statuses
ALTER TABLE statuses DROP CONSTRAINT IF EXISTS fk_statuses_display_name;
ALTER TABLE statuses ADD CONSTRAINT fk_statuses_display_name
  FOREIGN KEY (display_name) REFERENCES statuses (statuse_id);

-- Products
ALTER TABLE products DROP CONSTRAINT IF EXISTS fk_products_sku;
ALTER TABLE products ADD CONSTRAINT fk_products_sku
  FOREIGN KEY (sku) REFERENCES products (product_id);
ALTER TABLE products DROP CONSTRAINT IF EXISTS fk_products_inventory_adjustments;
ALTER TABLE products ADD CONSTRAINT fk_products_inventory_adjustments
  FOREIGN KEY (inventory_adjustments) REFERENCES inventory_adjustments (inventory_adjustment_id);

-- Invoices
ALTER TABLE invoices DROP CONSTRAINT IF EXISTS fk_invoices_client;
ALTER TABLE invoices ADD CONSTRAINT fk_invoices_client
  FOREIGN KEY (client) REFERENCES clients (client_id);

-- InvoiceLineItems
ALTER TABLE invoice_line_items DROP CONSTRAINT IF EXISTS fk_invoice_line_items_invoice;
ALTER TABLE invoice_line_items ADD CONSTRAINT fk_invoice_line_items_invoice
  FOREIGN KEY (invoice) REFERENCES invoices (invoice_id);
ALTER TABLE invoice_line_items DROP CONSTRAINT IF EXISTS fk_invoice_line_items_product;
ALTER TABLE invoice_line_items ADD CONSTRAINT fk_invoice_line_items_product
  FOREIGN KEY (product) REFERENCES products (product_id);

-- InventoryAdjustments
ALTER TABLE inventory_adjustments DROP CONSTRAINT IF EXISTS fk_inventory_adjustments_product;
ALTER TABLE inventory_adjustments ADD CONSTRAINT fk_inventory_adjustments_product
  FOREIGN KEY (product) REFERENCES products (product_id);

-- Payments
ALTER TABLE payments DROP CONSTRAINT IF EXISTS fk_payments_invoice;
ALTER TABLE payments ADD CONSTRAINT fk_payments_invoice
  FOREIGN KEY (invoice) REFERENCES invoices (invoice_id);

-- Addresses
ALTER TABLE addresses DROP CONSTRAINT IF EXISTS fk_addresses_clients;
ALTER TABLE addresses ADD CONSTRAINT fk_addresses_clients
  FOREIGN KEY (clients) REFERENCES clients (client_id);
ALTER TABLE addresses DROP CONSTRAINT IF EXISTS fk_addresses_type_of_address;
ALTER TABLE addresses ADD CONSTRAINT fk_addresses_type_of_address
  FOREIGN KEY (type_of_address) REFERENCES types_of_addresses (types_of_addresse_id);
ALTER TABLE addresses DROP CONSTRAINT IF EXISTS fk_addresses_state;
ALTER TABLE addresses ADD CONSTRAINT fk_addresses_state
  FOREIGN KEY (state) REFERENCES states (state_id);

-- 16 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
