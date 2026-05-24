-- FunctionOverride: calc_customers_average_order_value
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_customers_average_order_value
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_customers_average_order_value(p_customer_id text)
 RETURNS TEXT
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
DECLARE
  v_avg numeric;
BEGIN
  SELECT AVG(calc_orders_order_total(order_id))
    INTO v_avg
    FROM orders
   WHERE customer = p_customer_id;
  RETURN COALESCE(ROUND(v_avg, 2)::text, '0.00');
END;
$function$;
