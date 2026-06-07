-- FunctionOverride: calc_citations_is_in_collections
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).
-- Fix: transpiler emitted invalid cast (date -> numeric) for PaymentDueDate.
-- Correct formula: compare as_of_date > (payment_due_date + integer).

CREATE OR REPLACE FUNCTION public.calc_citations_is_in_collections(p_citation_id text)
 RETURNS boolean
 LANGUAGE sql
 STABLE
AS $function$
  SELECT CASE
    WHEN calc_citations_is_payment_late(p_citation_id)
      AND (SELECT as_of_date FROM citations WHERE citation_id = p_citation_id)
          > (calc_citations_payment_due_date(p_citation_id)
             + COALESCE(calc_citations_days_late_to_collections(p_citation_id), 0))
    THEN TRUE
    ELSE FALSE
  END;
$function$;
