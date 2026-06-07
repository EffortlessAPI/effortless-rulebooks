-- FunctionOverride: calc_citations_payment_due_date
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).
-- Fix: transpiler emitted invalid cast (date -> numeric) for ResponseDueDate.
-- Correct formula: date + integer = date (Postgres native date arithmetic).

CREATE OR REPLACE FUNCTION public.calc_citations_payment_due_date(p_citation_id text)
 RETURNS date
 LANGUAGE sql
 STABLE
AS $function$
  SELECT (
    calc_citations_response_due_date(p_citation_id)
    + COALESCE(calc_citations_days_to_pay_after_ruling(p_citation_id), 0)
  )::date;
$function$;
