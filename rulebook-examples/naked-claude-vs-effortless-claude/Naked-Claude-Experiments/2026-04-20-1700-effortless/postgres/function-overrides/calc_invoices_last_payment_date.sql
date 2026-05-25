-- FunctionOverride: calc_invoices_last_payment_date
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).
-- The generated version tries SUM((payment_date)::numeric) which fails;
-- the correct aggregation for a last-payment timestamp is MAX(payment_date).

CREATE OR REPLACE FUNCTION public.calc_invoices_last_payment_date(p_invoice_id text)
 RETURNS timestamp with time zone
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN (SELECT MAX(payment_date) FROM payments WHERE invoice = p_invoice_id);
END;
$function$;
