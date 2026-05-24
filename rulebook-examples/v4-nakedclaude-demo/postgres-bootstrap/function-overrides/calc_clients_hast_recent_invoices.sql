-- FunctionOverride: calc_clients_hast_recent_invoices
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

-- FunctionOverride: calc_clients_hast_recent_invoices
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).
-- Original Airtable formula: =IF({{LastInvoice}} > DATEADD(TODAY(), -180, 'days'), TRUE(), FALSE())
-- Translation failed because DATEADD unit 'days' is unsupported. Reimplement directly.

CREATE OR REPLACE FUNCTION public.calc_clients_hast_recent_invoices(p_client_id text)
 RETURNS boolean
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE(
    (SELECT MAX(order_date) FROM invoices WHERE client = p_client_id) > (NOW() - INTERVAL '180 days'),
    FALSE
  );
END;
$function$;
