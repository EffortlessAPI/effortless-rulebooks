-- Computed view for Customers
-- The view IS the contract: all calculated/lookup/aggregation columns are materialized here.
-- Application code reads from this view, never from the raw table.

CREATE OR REPLACE VIEW vw_customers AS
SELECT
  "customer_id",  "first_name",  "last_name",  "first_name" || ' ' || "last_name" AS "name",  SUBSTRING("first_name" FROM LENGTH("first_name") - 1 + 1) || '.' || SUBSTRING("last_name" FROM LENGTH("last_name") - 1 + 1) || '.' AS "initials"
FROM customers;
