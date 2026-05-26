-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined functions customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main functions script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom functions changes will appear here:

-- Override: calc_clients_last_invoice
-- Generated version uses SUM(order_date)::timestamptz which is invalid.
-- LastInvoice should be the most-recent invoice date for the client.
CREATE OR REPLACE FUNCTION calc_clients_last_invoice(p_client_id TEXT)
RETURNS TIMESTAMPTZ AS $$
  SELECT MAX(order_date) FROM invoices WHERE client = p_client_id;
$$ LANGUAGE sql STABLE;

-- Override: calc_invoices_last_payment_date — same SUM-of-timestamp bug.
CREATE OR REPLACE FUNCTION calc_invoices_last_payment_date(p_invoice_id TEXT)
RETURNS TIMESTAMPTZ AS $$
  SELECT MAX(payment_date) FROM payments WHERE invoice = p_invoice_id;
$$ LANGUAGE sql STABLE;

-- Override: calc_products_count_of_vip_orders
-- Generated version omits the VIP filter and counts ALL line items for the product.
-- Restrict to line items whose invoice belongs to a VIP client.
CREATE OR REPLACE FUNCTION calc_products_count_of_vip_orders(p_product_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM invoice_line_items li
  JOIN invoices i ON i.invoice_id = li.invoice
  WHERE li.product = p_product_id
    AND calc_clients_is_vip(i.client) IS TRUE;
$$ LANGUAGE sql STABLE;
