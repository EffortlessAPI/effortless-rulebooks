-- FunctionOverride: calc_orders_payment_count
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_orders_payment_count
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_orders_payment_count(p_order_id text)
 RETURNS integer
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN ((SELECT COUNT(*) FROM payments WHERE "order" = p_order_id))::integer;
END;
$function$;
