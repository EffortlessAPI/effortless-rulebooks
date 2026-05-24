-- FunctionOverride: calc_orders_last_payment_date
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_orders_last_payment_date
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_orders_last_payment_date(p_order_id text)
 RETURNS timestamp with time zone
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN (SELECT MAX(payment_date) FROM payments WHERE "order" = p_order_id);
END;
$function$;
