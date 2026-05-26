-- FunctionOverride: calc_customers_days_since_last_order
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_customers_days_since_last_order
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_customers_days_since_last_order(p_customer_id text)
 RETURNS TEXT
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
DECLARE
  v_last timestamptz;
BEGIN
  SELECT MAX(order_date::timestamptz)
    INTO v_last
    FROM orders
   WHERE customer = p_customer_id;
  IF v_last IS NULL THEN
    RETURN NULL;
  END IF;
  RETURN (CURRENT_DATE - v_last::date)::text;
END;
$function$;
