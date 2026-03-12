-- ============================================================================
-- SOURCE: ERBCustomizations table, record: 02b-customize-functions.sql
-- If you see SQL errors below, check this customization in Airtable
-- ============================================================================

-- Prediction Fail: Returns TRUE when predicted_answer doesn't match is_language
DROP FUNCTION calc_language_candidates_question;

CREATE OR REPLACE FUNCTION public.calc_language_candidates_question(p_language_candidate_id text)
 RETURNS text
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
BEGIN
  RETURN ('Is ' || (SELECT NULLIF(name, '''') FROM language_candidates WHERE language_candidate_id = p_language_candidate_id) || ' a language?')::text;
END;
$function$;

CREATE OR REPLACE FUNCTION public.calc_language_candidates_bio_hockett_score(p_language_candidate_id text)
 RETURNS integer
 LANGUAGE plpgsql
 STABLE SECURITY DEFINER
AS $function$
DECLARE
    candidate_record RECORD;
BEGIN
    -- Select the required attributes for the given language candidate ID
    SELECT bio_has_semanticity, bio_has_arbitrariness, bio_has_discreteness, 
           bio_has_duality_of_patterning, bio_has_productivity, 
           bio_has_displacement, bio_has_cultural_transmission, 
           bio_has_interchangeability, bio_has_feedback, 
           bio_has_broadcast_transmission, bio_has_rapid_fading
    INTO candidate_record
    FROM language_candidates
    WHERE language_candidate_id = p_language_candidate_id;

    -- Calculate and return the Hockett score by summing up the boolean attributes
    RETURN 
        COALESCE(candidate_record.bio_has_semanticity, FALSE)::int +
        COALESCE(candidate_record.bio_has_arbitrariness, FALSE)::int +
        COALESCE(candidate_record.bio_has_discreteness, FALSE)::int +
        COALESCE(candidate_record.bio_has_duality_of_patterning, FALSE)::int +
        COALESCE(candidate_record.bio_has_productivity, FALSE)::int +
        COALESCE(candidate_record.bio_has_displacement, FALSE)::int +
        COALESCE(candidate_record.bio_has_cultural_transmission, FALSE)::int +
        COALESCE(candidate_record.bio_has_interchangeability, FALSE)::int +
        COALESCE(candidate_record.bio_has_feedback, FALSE)::int +
        COALESCE(candidate_record.bio_has_broadcast_transmission, FALSE)::int +
        COALESCE(candidate_record.bio_has_rapid_fading, FALSE)::int;
END;
$function$;
