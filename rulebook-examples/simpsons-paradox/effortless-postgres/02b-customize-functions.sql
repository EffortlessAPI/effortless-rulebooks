-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined functions customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main functions script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom functions changes will appear here:

-- ============================================================================
-- run_invariant_checks()
-- Reads every row in invariant_checks and evaluates SqlAssertion (with optional
-- SqlFilter) against vw_treatment_rankings, then writes pass_count and
-- fail_count back into the table. The entire test harness is driven from the
-- rulebook — no assertion SQL lives outside the InvariantChecks table.
-- ============================================================================
CREATE OR REPLACE FUNCTION run_invariant_checks()
RETURNS TABLE(invariant_check_id TEXT, pass_count INTEGER, fail_count INTEGER, status_label TEXT)
LANGUAGE plpgsql AS $$
DECLARE
  rec RECORD;
  total_sql  TEXT;
  pass_sql   TEXT;
  v_total    INTEGER;
  v_pass     INTEGER;
  v_fail     INTEGER;
BEGIN
  FOR rec IN SELECT ic.invariant_check_id, ic.sql_filter, ic.sql_assertion
             FROM invariant_checks ic
             ORDER BY ic.invariant_check_id
  LOOP
    -- Build the universe count query
    IF rec.sql_filter IS NOT NULL AND rec.sql_filter <> '' THEN
      total_sql := format('SELECT COUNT(*) FROM vw_treatment_rankings WHERE %s', rec.sql_filter);
      pass_sql  := format('SELECT COUNT(*) FROM vw_treatment_rankings WHERE (%s) AND (%s)', rec.sql_filter, rec.sql_assertion);
    ELSE
      total_sql := 'SELECT COUNT(*) FROM vw_treatment_rankings';
      pass_sql  := format('SELECT COUNT(*) FROM vw_treatment_rankings WHERE %s', rec.sql_assertion);
    END IF;

    EXECUTE total_sql INTO v_total;
    EXECUTE pass_sql  INTO v_pass;
    v_fail := v_total - v_pass;

    UPDATE invariant_checks
    SET pass_count = v_pass, fail_count = v_fail
    WHERE invariant_checks.invariant_check_id = rec.invariant_check_id;

    RETURN QUERY
      SELECT rec.invariant_check_id,
             v_pass,
             v_fail,
             CASE WHEN v_fail = 0 THEN 'PASS' ELSE 'FAIL(' || v_fail || ')' END;
  END LOOP;
END;
$$;

