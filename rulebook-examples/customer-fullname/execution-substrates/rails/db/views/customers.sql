-- Computed view for Customers
-- The view IS the contract: all calculated/lookup/aggregation columns are materialized here.
-- Application code reads from this view, never from the raw table.

CREATE OR REPLACE VIEW vw_customers AS
SELECT
  "customer_id",  "last_name" || ", " || "first_name" AS "name",  "email_address",  SUBSTRING("first_name" FROM 1 FOR 1) || SUBSTRING("last_name" FROM 1 FOR 1) || "." AS "initials",  "first_name",  "last_name"
FROM customers;
