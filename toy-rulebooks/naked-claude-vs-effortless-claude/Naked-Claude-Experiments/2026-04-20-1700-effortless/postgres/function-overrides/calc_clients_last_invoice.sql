-- FunctionOverride: calc_clients_last_invoice
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).
-- The generated version tries SUM((order_date)::numeric) which fails;
-- the correct aggregation for a last-invoice timestamp is MAX(order_date).

CREATE OR REPLACE FUNCTION public.calc_clients_last_invoice(p_client_id text)
 RETURNS timestamp with time zone
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN (SELECT MAX(order_date) FROM invoices WHERE client = p_client_id);
END;
$function$;
