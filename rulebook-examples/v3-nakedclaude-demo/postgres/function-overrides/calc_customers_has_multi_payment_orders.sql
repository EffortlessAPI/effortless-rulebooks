-- FunctionOverride: calc_customers_has_multi_payment_orders
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_customers_has_multi_payment_orders
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_customers_has_multi_payment_orders(p_customer_id text)
 RETURNS BOOLEAN
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN EXISTS (
    SELECT 1
      FROM orders o
     WHERE o.customer = p_customer_id
       AND (SELECT COUNT(*) FROM payments p WHERE p."order" = o.order_id) > 1
  );
END;
$function$;
