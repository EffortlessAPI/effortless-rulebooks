-- FunctionOverride: calc_orders_item_count
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_orders_item_count
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_orders_item_count(p_order_id text)
 RETURNS integer
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN ((SELECT COUNT(*) FROM order_line_items WHERE "order" = p_order_id))::integer;
END;
$function$;
