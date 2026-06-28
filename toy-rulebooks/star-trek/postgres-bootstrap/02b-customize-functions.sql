-- ============================================================================
-- SOURCE: ERBCustomizations table, record: 03b-customize-functions.sql
-- If you see SQL errors below, check this customization in Airtable
-- ============================================================================

-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined calculation functions
-- ============================================================================
-- This file is for YOUR custom PostgreSQL functions that should persist
-- across regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional calc_* functions for computed fields
--   - Helper functions for complex business logic
--   - Utility functions for data transformation
--   - Trigger functions for custom automation
--
-- IMPORTANT:
--   - This file runs AFTER 02-create-functions.sql
--   - Base ERB calc functions already exist when this runs
--   - This file will NOT be overwritten by ERB regeneration
--   - Functions defined here can be used in 03b-customize-views.sql
--
-- ERB FUNCTION NAMING CONVENTIONS:
--   - calc_tablename_fieldname() - Returns a computed value for a view field
--   - Helper functions can use any naming convention
--
-- EXAMPLES:
--
--   -- Custom calculation function for a view field
--   CREATE OR REPLACE FUNCTION calc_orders_total_with_tax(p_order_id TEXT)
--   RETURNS NUMERIC AS $$
--   BEGIN
--       RETURN (SELECT subtotal * 1.08 FROM orders WHERE order_id = p_order_id);
--   END;
--   $$ LANGUAGE plpgsql STABLE;
--
--   -- Utility function
--   CREATE OR REPLACE FUNCTION format_currency(p_amount NUMERIC)
--   RETURNS TEXT AS $$
--   BEGIN
--       RETURN '$' || TO_CHAR(p_amount, 'FM999,999,990.00');
--   END;
--   $$ LANGUAGE plpgsql IMMUTABLE;
--
-- ============================================================================

-- Your custom functions go here:

CREATE OR REPLACE FUNCTION public.calc_series_rating(p_serie_id text)
 RETURNS numeric
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN ((SELECT COALESCE(COUNT(rating), 0) FROM ratings WHERE series = p_serie_id));
END;
$function$