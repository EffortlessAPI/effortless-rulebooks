-- FunctionOverride: calc_citations_days_until_response_due
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).
-- Fix: transpiler emitted invalid cast (date -> numeric). Date subtraction
-- in Postgres already yields integer days; no numeric cast needed.

CREATE OR REPLACE FUNCTION public.calc_citations_days_until_response_due(p_citation_id text)
 RETURNS integer
 LANGUAGE sql
 STABLE
AS $function$
  SELECT (
    calc_citations_response_due_date(p_citation_id)
    - (SELECT as_of_date FROM citations WHERE citation_id = p_citation_id)
  )::integer;
$function$;
