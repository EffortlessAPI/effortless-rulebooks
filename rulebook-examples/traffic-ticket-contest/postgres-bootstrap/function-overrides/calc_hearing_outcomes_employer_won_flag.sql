-- FunctionOverride: calc_hearing_outcomes_employer_won_flag
-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).

CREATE OR REPLACE FUNCTION public.calc_attendance_details_latest_occurrence_date(p_attendance_detail_id text)
RETURNS timestamp with time zone
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN (SELECT MAX(occurrence_date) FROM attendance_occurrences
          WHERE attendance_detail = p_attendance_detail_id);
END;
$function$;

-- 2. is_override_action: generated code uses double-quoted "override" (an identifier).
CREATE OR REPLACE FUNCTION public.calc_audit_log_entries_is_override_action(p_audit_log_entrie_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN LOWER(COALESCE((SELECT action_type FROM audit_log_entries
                         WHERE audit_log_entrie_id = p_audit_log_entrie_id), '')) = 'override';
END;
$function$;

-- 3. tenure_years_approx: cast 0.3 to integer fails when text is fractional.
CREATE OR REPLACE FUNCTION public.calc_claimants_tenure_years_approx(p_claimant_id text)
RETURNS integer
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
DECLARE
  v_hire date;
BEGIN
  SELECT hire_date INTO v_hire FROM claimants WHERE claimant_id = p_claimant_id;
  IF v_hire IS NULL THEN RETURN NULL; END IF;
  RETURN ROUND((CURRENT_DATE - v_hire)::numeric / 365.0)::integer;
END;
$function$;

-- 4. avg_client_hygiene_served: AVGs a text value (STRONG/MIXED/WEAK).
--    Map to numeric scale: STRONG=3, MIXED=2, WEAK=1, then average.
CREATE OR REPLACE FUNCTION public.calc_claims_reps_avg_client_hygiene_served(p_claims_rep_id text)
RETURNS integer
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE(ROUND((SELECT AVG(
      CASE UPPER(calc_unemployment_claims_client_hygiene_rating(unemployment_claim_id))
        WHEN 'STRONG' THEN 3
        WHEN 'MIXED'  THEN 2
        WHEN 'WEAK'   THEN 1
        ELSE NULL
      END
    ) FROM unemployment_claims WHERE claims_rep = p_claims_rep_id)), 0)::integer;
END;
$function$;

-- 5. has_reference: generated CASE WHEN '" = "' is gibberish.
CREATE OR REPLACE FUNCTION public.calc_evidence_items_has_reference(p_evidence_item_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE(NULLIF((SELECT document_reference FROM evidence_items
                          WHERE evidence_item_id = p_evidence_item_id), ''), NULL) IS NOT NULL;
END;
$function$;

-- 6. is_uploaded_and_referenced: compared a boolean to integer 1.
CREATE OR REPLACE FUNCTION public.calc_evidence_items_is_uploaded_and_referenced(p_evidence_item_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE((SELECT document_uploaded FROM evidence_items
                   WHERE evidence_item_id = p_evidence_item_id), FALSE)
         AND calc_evidence_items_has_reference(p_evidence_item_id);
END;
$function$;

-- 7. employer_won_flag: double-quoted "employer-won" parsed as identifier.
CREATE OR REPLACE FUNCTION public.calc_hearing_outcomes_employer_won_flag(p_hearing_outcome_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN LOWER(COALESCE((SELECT result FROM hearing_outcomes
                         WHERE hearing_outcome_id = p_hearing_outcome_id), '')) = 'employer-win';
END;
$function$;

-- 8. employee_responded_flag: same broken concat pattern.
CREATE OR REPLACE FUNCTION public.calc_insubordination_details_employee_responded_flag(p_insubordination_detail_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE(NULLIF((SELECT employee_response_text FROM insubordination_details
                          WHERE insubordination_detail_id = p_insubordination_detail_id), ''), NULL) IS NOT NULL;
END;
$function$;

-- 9. jurisdiction_strictness_flag: double-quoted state codes parsed as identifiers.
CREATE OR REPLACE FUNCTION public.calc_misconduct_checklists_jurisdiction_strictness_flag(p_misconduct_checklist_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN LOWER(COALESCE(calc_misconduct_checklists_jurisdiction_state(p_misconduct_checklist_id), ''))
         IN ('ca','ny','nj','wa');
END;
$function$;

-- 10. latest_hearing_date: SUM of timestamp; want MAX.
CREATE OR REPLACE FUNCTION public.calc_misconduct_checklists_latest_hearing_date(p_misconduct_checklist_id text)
RETURNS timestamp with time zone
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN (SELECT MAX(hearing_date) FROM hearing_outcomes
          WHERE misconduct_checklist = p_misconduct_checklist_id);
END;
$function$;

-- 11. overall_readiness_rating: compares boolean flags using = 0/= 1.
CREATE OR REPLACE FUNCTION public.calc_misconduct_checklists_overall_readiness_rating(p_misconduct_checklist_id text)
RETURNS text
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  IF calc_misconduct_checklists_raw_readiness_score(p_misconduct_checklist_id) >= 20
     AND NOT calc_misconduct_checklists_weak_documentation_flag(p_misconduct_checklist_id)
     AND calc_misconduct_checklists_witness_corroboration_flag(p_misconduct_checklist_id)
     AND NOT calc_misconduct_checklists_manager_override_heavy_flag(p_misconduct_checklist_id)
  THEN
    RETURN 'ready';
  ELSIF calc_misconduct_checklists_raw_readiness_score(p_misconduct_checklist_id) >= 12 THEN
    RETURN 'needs-review';
  ELSE
    RETURN 'not-ready';
  END IF;
END;
$function$;

-- 12. readiness_gap_narrative: same boolean = integer issue.
CREATE OR REPLACE FUNCTION public.calc_misconduct_checklists_readiness_gap_narrative(p_misconduct_checklist_id text)
RETURNS text
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  IF calc_misconduct_checklists_weak_documentation_flag(p_misconduct_checklist_id) THEN
    RETURN 'weak-documentation';
  ELSIF NOT calc_misconduct_checklists_witness_corroboration_flag(p_misconduct_checklist_id) THEN
    RETURN 'missing-witness';
  ELSIF calc_misconduct_checklists_manager_override_heavy_flag(p_misconduct_checklist_id) THEN
    RETURN 'override-heavy';
  ELSIF calc_misconduct_checklists_jurisdiction_strictness_flag(p_misconduct_checklist_id) THEN
    RETURN 'strict-jurisdiction-double-check';
  ELSE
    RETURN 'ok';
  END IF;
END;
$function$;

-- 13. severe_behavior_max_rollup: MAX of boolean cast to numeric. Use bool_or.
CREATE OR REPLACE FUNCTION public.calc_misconduct_checklists_severe_behavior_max_rollup(p_misconduct_checklist_id text)
RETURNS integer
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN CASE WHEN COALESCE((SELECT bool_or(calc_behavioral_details_is_severe_behavior(behavioral_detail_id))
                             FROM behavioral_details
                             WHERE misconduct_checklist = p_misconduct_checklist_id), FALSE)
              THEN 1 ELSE 0 END;
END;
$function$;

-- 14. severe_incident_flag: depends on severe_behavior_max_rollup; rollup now returns int.
CREATE OR REPLACE FUNCTION public.calc_misconduct_checklists_severe_incident_flag(p_misconduct_checklist_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN calc_misconduct_checklists_severe_behavior_max_rollup(p_misconduct_checklist_id) = 1;
END;
$function$;

-- 15. long_standing_issue_flag: cast "4 months" to numeric fails. Parse leading number + unit.
CREATE OR REPLACE FUNCTION public.calc_performance_details_long_standing_issue_flag(p_performance_detail_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
DECLARE
  v_text text;
  v_num  numeric;
  v_days numeric;
BEGIN
  SELECT issue_duration INTO v_text FROM performance_details
   WHERE performance_detail_id = p_performance_detail_id;
  IF v_text IS NULL OR v_text = '' THEN RETURN FALSE; END IF;
  v_num := NULLIF((regexp_match(v_text, '([0-9]+(\.[0-9]+)?)'))[1], '')::numeric;
  IF v_num IS NULL THEN RETURN FALSE; END IF;
  v_days := CASE
              WHEN v_text ILIKE '%year%'  THEN v_num * 365
              WHEN v_text ILIKE '%month%' THEN v_num * 30
              WHEN v_text ILIKE '%week%'  THEN v_num * 7
              ELSE v_num
            END;
  RETURN v_days >= 60;
END;
$function$;

-- 16. any_progressive_discipline_flag: SUMs a boolean. Use bool_or → int.
CREATE OR REPLACE FUNCTION public.calc_unemployment_claims_any_progressive_discipline_flag(p_unemployment_claim_id text)
RETURNS integer
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN CASE WHEN COALESCE((SELECT bool_or(calc_misconduct_checklists_progressive_discipline_flag(misconduct_checklist_id))
                             FROM misconduct_checklists
                             WHERE unemployment_claim = p_unemployment_claim_id), FALSE)
              THEN 1 ELSE 0 END;
END;
$function$;

-- 17. any_weak_documentation_flag: same.
CREATE OR REPLACE FUNCTION public.calc_unemployment_claims_any_weak_documentation_flag(p_unemployment_claim_id text)
RETURNS integer
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN CASE WHEN COALESCE((SELECT bool_or(calc_misconduct_checklists_weak_documentation_flag(misconduct_checklist_id))
                             FROM misconduct_checklists
                             WHERE unemployment_claim = p_unemployment_claim_id), FALSE)
              THEN 1 ELSE 0 END;
END;
$function$;

-- 18. claim_risk_baseline: compares hygiene rating (text) to integers.
--     Map STRONG/MIXED/WEAK to numeric scale, then bucket.
CREATE OR REPLACE FUNCTION public.calc_unemployment_claims_claim_risk_baseline(p_unemployment_claim_id text)
RETURNS text
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
DECLARE
  v_score int;
BEGIN
  v_score := CASE UPPER(COALESCE(calc_unemployment_claims_client_hygiene_rating(p_unemployment_claim_id), ''))
               WHEN 'STRONG' THEN 4
               WHEN 'MIXED'  THEN 3
               WHEN 'WEAK'   THEN 1
               ELSE 0
             END;
  IF v_score < 3 THEN RETURN 'high';
  ELSIF v_score < 4 THEN RETURN 'medium';
  ELSE RETURN 'low';
  END IF;
END;
$function$;

-- 19. composite_claim_readiness: has_checklist_flag is boolean, compared to 0.
CREATE OR REPLACE FUNCTION public.calc_unemployment_claims_composite_claim_readiness(p_unemployment_claim_id text)
RETURNS text
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
DECLARE
  v_readiness int;
BEGIN
  IF NOT calc_unemployment_claims_has_checklist_flag(p_unemployment_claim_id) THEN
    RETURN 'no-checklist';
  END IF;
  v_readiness := calc_unemployment_claims_latest_checklist_raw_readiness(p_unemployment_claim_id);
  IF calc_unemployment_claims_claim_risk_baseline(p_unemployment_claim_id) = 'high'
     AND v_readiness < 20 THEN
    RETURN 'high-risk';
  ELSIF v_readiness >= 20 THEN
    RETURN 'ready';
  ELSIF v_readiness >= 12 THEN
    RETURN 'needs-review';
  ELSE
    RETURN 'not-ready';
  END IF;
END;
$function$;

-- 20. escalate_to_manager_flag: any_weak_documentation_flag now returns int again, OK,
--     but we also need to handle the any_progressive_discipline_flag side.
CREATE OR REPLACE FUNCTION public.calc_unemployment_claims_escalate_to_manager_flag(p_unemployment_claim_id text)
RETURNS integer
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN CASE
    WHEN calc_unemployment_claims_any_weak_documentation_flag(p_unemployment_claim_id) = 1
      OR (calc_unemployment_claims_claim_risk_baseline(p_unemployment_claim_id) = 'high'
          AND calc_unemployment_claims_any_progressive_discipline_flag(p_unemployment_claim_id) = 0)
    THEN 1 ELSE 0 END;
END;
$function$;

-- 21. has_contact_info: same broken concat pattern.
CREATE OR REPLACE FUNCTION public.calc_witnesses_has_contact_info(p_witnesse_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE(NULLIF((SELECT contact_info FROM witnesses
                          WHERE witnesse_id = p_witnesse_id), ''), NULL) IS NOT NULL;
END;
$function$;

-- 22. has_usable_statement: depended on has_contact_info = 1.
CREATE OR REPLACE FUNCTION public.calc_witnesses_has_usable_statement(p_witnesse_id text)
RETURNS boolean
LANGUAGE plpgsql STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN COALESCE((SELECT statement_provided_flag FROM witnesses
                   WHERE witnesse_id = p_witnesse_id), FALSE)
         AND calc_witnesses_has_contact_info(p_witnesse_id);
END;
$function$;
