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
-- SqlFilter) against the view named by SourceTable, then writes pass_count and
-- fail_count back into the table. Routes by source_table:
--   TreatmentRankings → vw_treatment_rankings (per-row assertions)
--   ModelSummary      → vw_model_summary (scalar aggregate assertions, 1 row)
-- ============================================================================
-- ============================================================================
-- Cross-domain ModelSummary functions (loop-47/48)
-- These reference studies.domain and studies.is_synthetic — columns added in
-- 01b-customize-schema.sql — so they must live here, not in the transpiler output.
-- ============================================================================
CREATE OR REPLACE FUNCTION calc_model_summary_real_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM studies WHERE is_synthetic = false;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_medicine_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM studies WHERE domain = 'medicine' AND is_synthetic = false;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_epidemiology_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM studies WHERE domain = 'epidemiology' AND is_synthetic = false;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_other_domain_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT (calc_model_summary_real_study_count(p_model_summary_id)
          - calc_model_summary_medicine_study_count(p_model_summary_id)
          - calc_model_summary_epidemiology_study_count(p_model_summary_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_domain_diversity_note(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT CONCAT('medicine:', calc_model_summary_medicine_study_count(p_model_summary_id),
                ' epidemiology:', calc_model_summary_epidemiology_study_count(p_model_summary_id),
                ' other:', calc_model_summary_other_domain_study_count(p_model_summary_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_avg_signal_purity_reversal(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT AVG(signal_purity)
  FROM _erb_tr_metrics
  WHERE allocation_direction = 'reversal';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_avg_signal_purity_non_reversal(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT AVG(signal_purity)
  FROM _erb_tr_metrics
  WHERE allocation_direction <> 'reversal';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_signal_purity_gap(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_model_summary_avg_signal_purity_non_reversal(p_model_summary_id)
       - calc_model_summary_avg_signal_purity_reversal(p_model_summary_id);
$$ LANGUAGE sql STABLE;

-- Fix TypeCCount: transpiler emits C+/C- subtypes; aggregate both
CREATE OR REPLACE FUNCTION calc_model_summary_type_c_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type LIKE 'C%';
$$ LANGUAGE sql STABLE;

-- IngestionProtocol witnesses (loop-49): transpiler cannot emit LOOKUP inside calculated fields
CREATE OR REPLACE FUNCTION calc_studies_ingestion_cell_parity(p_study_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (
    (SELECT stratum_count FROM _erb_tr_metrics tr WHERE tr.study = p_study_id LIMIT 1) >= 2
    AND calc_studies_cell_count(p_study_id) =
        (SELECT stratum_count FROM _erb_tr_metrics tr WHERE tr.study = p_study_id LIMIT 1) * 2
  );
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_studies_ingestion_compliance(p_study_id TEXT)
RETURNS TEXT AS $$
  SELECT CASE
    WHEN COALESCE((SELECT is_synthetic FROM studies WHERE study_id = p_study_id), false)
      THEN 'exempt-synthetic'
    WHEN calc_studies_ingestion_cell_parity(p_study_id)
     AND NULLIF((SELECT variable_name FROM stratum_variables WHERE study = p_study_id LIMIT 1), '') IS NOT NULL
      THEN 'all'
    ELSE 'partial'
  END;
$$ LANGUAGE sql STABLE;

-- IngestionSummary COUNTIFS mistranslation fix (loop-49): "<>" became literal filter
CREATE OR REPLACE FUNCTION calc_ingestion_summary_protocol_item_count(p_ingestion_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM ingestion_protocol;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_ingestion_summary_corpus_cell_count(p_ingestion_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM case_cells;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_ingestion_summary_study_count(p_ingestion_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM studies;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_ingestion_protocol_item_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM ingestion_protocol;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_corpus_passes_ingestion_contract(p_model_summary_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT calc_ingestion_summary_ingestion_contract_passes('ingestion-v1');
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_ingestion_witness_note(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_ingestion_summary_ingestion_witness_note('ingestion-v1');
$$ LANGUAGE sql STABLE;

-- AllocationSweep + SweepStudySummary LOOKUP overrides (loop-56)
-- Transpiler cannot emit LOOKUP(); join sweep_study_config / treatment_rankings instead.

CREATE OR REPLACE FUNCTION calc_allocation_sweep_is_original(p_sweep_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT ABS(s.alloc_fraction_a - c.original_alloc_fraction_a) < 0.03
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_n_sweep_stratum_total(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.n_sweep_stratum_total
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_n_fixed_a(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.n_fixed_a
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_n_fixed_b(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.n_fixed_b
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_sweep_rate_a(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.sweep_rate_a
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_sweep_rate_b(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.sweep_rate_b
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_fixed_rate_a(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.fixed_rate_a
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_fixed_rate_b(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.fixed_rate_b
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_allocation_sweep_sweep_corrected_gap(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.sweep_corrected_gap
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_distortion_type_label(p_sweep_study_id TEXT)
RETURNS TEXT AS $$
  SELECT distortion_type FROM _erb_tr_metrics WHERE study = p_sweep_study_id LIMIT 1;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_stratum_label(p_sweep_study_id TEXT)
RETURNS TEXT AS $$
  SELECT c.sweep_stratum_label
  FROM sweep_study_config c
  WHERE c.study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION run_invariant_checks()
RETURNS TABLE(invariant_check_id TEXT, pass_count INTEGER, fail_count INTEGER, status_label TEXT)
LANGUAGE plpgsql AS $$
DECLARE
  rec        RECORD;
  view_name  TEXT;
  total_sql  TEXT;
  pass_sql   TEXT;
  v_total    INTEGER;
  v_pass     INTEGER;
  v_fail     INTEGER;
BEGIN
  FOR rec IN SELECT ic.invariant_check_id, ic.sql_filter, ic.sql_assertion, ic.source_table
             FROM invariant_checks ic
             ORDER BY ic.invariant_check_id
  LOOP
    -- Route to the correct view based on SourceTable
    IF rec.source_table = 'ModelSummary' THEN
      view_name := 'vw_model_summary';
    ELSIF rec.source_table = 'SweepStudySummary' THEN
      view_name := 'vw_sweep_study_summary';
    ELSIF rec.source_table = 'PhaseDiagramSummary' THEN
      view_name := 'vw_phase_diagram_summary';
    ELSIF rec.source_table = 'IngestionSummary' THEN
      view_name := 'vw_ingestion_summary';
    ELSIF rec.source_table = 'CorpusCatalogSummary' THEN
      view_name := 'vw_corpus_catalog_summary';
    ELSIF rec.source_table = 'CandidateStudyCatalog' THEN
      view_name := 'vw_candidate_study_catalog';
    ELSIF rec.source_table = 'DiscoveryFindings' THEN
      view_name := 'vw_discovery_findings';
    ELSE
      view_name := 'vw_treatment_rankings';
    END IF;

    -- Build the universe count query
    IF rec.sql_filter IS NOT NULL AND rec.sql_filter <> '' THEN
      total_sql := format('SELECT COUNT(*) FROM %I WHERE %s', view_name, rec.sql_filter);
      pass_sql  := format('SELECT COUNT(*) FROM %I WHERE (%s) AND (%s)', view_name, rec.sql_filter, rec.sql_assertion);
    ELSE
      total_sql := format('SELECT COUNT(*) FROM %I', view_name);
      pass_sql  := format('SELECT COUNT(*) FROM %I WHERE %s', view_name, rec.sql_assertion);
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

-- ============================================================================
-- Performance: snapshot transpiler calcs, cache refresh, fast calc_* overrides
-- Snapshots clone calc_* from 02-create-functions.sql BEFORE overrides below.
-- refresh_* runs once in 05b-customize-data.sql; runtime reads are O(1).
-- ============================================================================

DO $snap_tr$
DECLARE
  fn RECORD;
  newdef TEXT;
  newname TEXT;
BEGIN
  FOR fn IN
    SELECT p.oid, p.proname
    FROM pg_proc p
    JOIN pg_namespace n ON n.oid = p.pronamespace
    WHERE n.nspname = 'public'
      AND p.proname LIKE 'calc_treatment_rankings_%'
  LOOP
    newname := replace(fn.proname, 'calc_treatment_rankings_', '_erb_snap_tr_');
    newdef := pg_get_functiondef(fn.oid);
    newdef := replace(newdef, 'FUNCTION public.' || fn.proname || '(', 'FUNCTION public.' || newname || '(');
    newdef := replace(newdef, 'FUNCTION ' || fn.proname || '(', 'FUNCTION ' || newname || '(');
    EXECUTE newdef;
  END LOOP;
END;
$snap_tr$;

DO $snap_sp$
DECLARE
  fn RECORD;
  newdef TEXT;
  newname TEXT;
BEGIN
  FOR fn IN
    SELECT p.oid, p.proname
    FROM pg_proc p
    JOIN pg_namespace n ON n.oid = p.pronamespace
    WHERE n.nspname = 'public'
      AND p.proname LIKE 'calc_synthetic_phase_%'
  LOOP
    newname := replace(fn.proname, 'calc_synthetic_phase_', '_erb_snap_sp_');
    newdef := pg_get_functiondef(fn.oid);
    newdef := replace(newdef, 'FUNCTION public.' || fn.proname || '(', 'FUNCTION public.' || newname || '(');
    newdef := replace(newdef, 'FUNCTION ' || fn.proname || '(', 'FUNCTION ' || newname || '(');
    EXECUTE newdef;
  END LOOP;
END;
$snap_sp$;

-- Snap bodies copied from calc_* still call calc_* by name. After calc overrides
-- read _erb_*_metrics, refresh would see an empty cache and write zeros. Rewrite
-- every snap to call sibling _erb_snap_* functions instead of calc_*.
DO $rewrite_snaps$
DECLARE
  fn RECORD;
  newdef TEXT;
BEGIN
  FOR fn IN
    SELECT p.oid, p.proname
    FROM pg_proc p
    JOIN pg_namespace n ON n.oid = p.pronamespace
    WHERE n.nspname = 'public'
      AND p.proname LIKE '_erb_snap_sp_%'
  LOOP
    newdef := pg_get_functiondef(fn.oid);
    newdef := replace(newdef, 'calc_synthetic_phase_', '_erb_snap_sp_');
    EXECUTE newdef;
  END LOOP;

  FOR fn IN
    SELECT p.oid, p.proname
    FROM pg_proc p
    JOIN pg_namespace n ON n.oid = p.pronamespace
    WHERE n.nspname = 'public'
      AND p.proname LIKE '_erb_snap_tr_%'
  LOOP
    newdef := pg_get_functiondef(fn.oid);
    newdef := replace(newdef, 'calc_treatment_rankings_', '_erb_snap_tr_');
    EXECUTE newdef;
  END LOOP;
END;
$rewrite_snaps$;

CREATE OR REPLACE FUNCTION refresh_erb_tr_metrics()
RETURNS void
LANGUAGE plpgsql AS $$
BEGIN
  TRUNCATE _erb_tr_metrics;

  INSERT INTO _erb_tr_metrics (
    treatment_ranking_id, name, study,
    total_cases_a, total_successes_a, pooled_rate_a,
    total_cases_b, total_successes_b, pooled_rate_b,
    pooled_winner, stratum_count, strata_won_by_a, strata_won_by_b, per_stratum_winner,
    is_reversal, confounders_in_study, is_paradox_explained,
    pooled_gap, strata_won_by_loser, paradox_strength,
    pooled_rate_from_weights_a, pooled_rate_from_weights_b,
    reversal_intensity, threshold_margin, signed_pooled_gap, weighted_stratum_gap_sum,
    is_sign_flip, allocation_distortion, distortion_type, policy_implication,
    corrected_gap, corrected_winner, corrected_vs_pooled_agreement, corrected_policy_implication,
    confirmed_causal_role_count, mediator_risk_count, contested_stratum_count, unknown_causal_role_count,
    causal_claim_status, adjustment_appropriate,
    allocation_direction, signal_purity, max_stratum_imbalance, max_stratum_gap,
    pooled_gap_crosses_zero, sweep_pooled_gap_range, latent_flip_potential,
    allocation_fragility, study_domain
  )
  SELECT
    tr.treatment_ranking_id,
    _erb_snap_tr_name(tr.treatment_ranking_id),
    tr.study,
    _erb_snap_tr_total_cases_a(tr.treatment_ranking_id),
    _erb_snap_tr_total_successes_a(tr.treatment_ranking_id),
    _erb_snap_tr_pooled_rate_a(tr.treatment_ranking_id),
    _erb_snap_tr_total_cases_b(tr.treatment_ranking_id),
    _erb_snap_tr_total_successes_b(tr.treatment_ranking_id),
    _erb_snap_tr_pooled_rate_b(tr.treatment_ranking_id),
    _erb_snap_tr_pooled_winner(tr.treatment_ranking_id),
    _erb_snap_tr_stratum_count(tr.treatment_ranking_id),
    _erb_snap_tr_strata_won_by_a(tr.treatment_ranking_id),
    _erb_snap_tr_strata_won_by_b(tr.treatment_ranking_id),
    _erb_snap_tr_per_stratum_winner(tr.treatment_ranking_id),
    _erb_snap_tr_is_reversal(tr.treatment_ranking_id),
    _erb_snap_tr_confounders_in_study(tr.treatment_ranking_id),
    _erb_snap_tr_is_paradox_explained(tr.treatment_ranking_id),
    _erb_snap_tr_pooled_gap(tr.treatment_ranking_id),
    _erb_snap_tr_strata_won_by_loser(tr.treatment_ranking_id),
    _erb_snap_tr_paradox_strength(tr.treatment_ranking_id),
    _erb_snap_tr_pooled_rate_from_weights_a(tr.treatment_ranking_id),
    _erb_snap_tr_pooled_rate_from_weights_b(tr.treatment_ranking_id),
    _erb_snap_tr_reversal_intensity(tr.treatment_ranking_id),
    _erb_snap_tr_threshold_margin(tr.treatment_ranking_id),
    _erb_snap_tr_signed_pooled_gap(tr.treatment_ranking_id),
    _erb_snap_tr_weighted_stratum_gap_sum(tr.treatment_ranking_id),
    _erb_snap_tr_is_sign_flip(tr.treatment_ranking_id),
    _erb_snap_tr_allocation_distortion(tr.treatment_ranking_id),
    _erb_snap_tr_distortion_type(tr.treatment_ranking_id),
    _erb_snap_tr_policy_implication(tr.treatment_ranking_id),
    _erb_snap_tr_corrected_gap(tr.treatment_ranking_id),
    _erb_snap_tr_corrected_winner(tr.treatment_ranking_id),
    _erb_snap_tr_corrected_vs_pooled_agreement(tr.treatment_ranking_id),
    _erb_snap_tr_corrected_policy_implication(tr.treatment_ranking_id),
    _erb_snap_tr_confirmed_causal_role_count(tr.treatment_ranking_id),
    _erb_snap_tr_mediator_risk_count(tr.treatment_ranking_id),
    _erb_snap_tr_contested_stratum_count(tr.treatment_ranking_id),
    _erb_snap_tr_unknown_causal_role_count(tr.treatment_ranking_id),
    _erb_snap_tr_causal_claim_status(tr.treatment_ranking_id),
    _erb_snap_tr_adjustment_appropriate(tr.treatment_ranking_id),
    _erb_snap_tr_allocation_direction(tr.treatment_ranking_id),
    _erb_snap_tr_signal_purity(tr.treatment_ranking_id),
    (SELECT MAX(ABS(ss.allocation_bias))
     FROM vw_stratum_summaries ss
     WHERE ss.study = tr.study AND ss.treatment_label = tr.treatment_a),
    (SELECT MAX(ABS(ss.stratum_gap))
     FROM vw_stratum_summaries ss
     WHERE ss.study = tr.study AND ss.treatment_label = tr.treatment_a),
    calc_sweep_study_summary_pooled_gap_crosses_zero(tr.study),
    calc_sweep_study_summary_sweep_pooled_gap_range(tr.study),
    (_erb_snap_tr_distortion_type(tr.treatment_ranking_id) = 'D'
      AND calc_sweep_study_summary_pooled_gap_crosses_zero(tr.study) = TRUE),
    CASE
      WHEN _erb_snap_tr_pooled_gap(tr.treatment_ranking_id) IS NULL
        OR _erb_snap_tr_pooled_gap(tr.treatment_ranking_id) = 0
        OR calc_sweep_study_summary_sweep_pooled_gap_range(tr.study) IS NULL
      THEN NULL
      ELSE calc_sweep_study_summary_sweep_pooled_gap_range(tr.study)
           / ABS(_erb_snap_tr_pooled_gap(tr.treatment_ranking_id))
    END,
    (SELECT s.domain FROM studies s WHERE s.study_id = tr.study)
  FROM treatment_rankings tr;
END;
$$;

CREATE OR REPLACE FUNCTION refresh_erb_sp_metrics()
RETURNS void
LANGUAGE plpgsql AS $$
BEGIN
  TRUNCATE _erb_sp_metrics;

  INSERT INTO _erb_sp_metrics (
    phase_id, name,
    phase_s1_total, phase_na1, phase_nb1, phase_na2, phase_nb2,
    phase_rate_a1, phase_rate_b1, phase_rate_a2, phase_rate_b2,
    phase_pooled_rate_a, phase_pooled_rate_b, phase_signed_pooled_gap,
    phase_corrected_gap, phase_strata_won_by_loser, phase_reversal_intensity,
    phase_is_sign_flip, phase_allocation_distortion, phase_distortion_type
  )
  SELECT
    sp.phase_id,
    _erb_snap_sp_name(sp.phase_id),
    _erb_snap_sp_phase_s1_total(sp.phase_id),
    _erb_snap_sp_phase_na1(sp.phase_id),
    _erb_snap_sp_phase_nb1(sp.phase_id),
    _erb_snap_sp_phase_na2(sp.phase_id),
    _erb_snap_sp_phase_nb2(sp.phase_id),
    _erb_snap_sp_phase_rate_a1(sp.phase_id),
    _erb_snap_sp_phase_rate_b1(sp.phase_id),
    _erb_snap_sp_phase_rate_a2(sp.phase_id),
    _erb_snap_sp_phase_rate_b2(sp.phase_id),
    _erb_snap_sp_phase_pooled_rate_a(sp.phase_id),
    _erb_snap_sp_phase_pooled_rate_b(sp.phase_id),
    _erb_snap_sp_phase_signed_pooled_gap(sp.phase_id),
    _erb_snap_sp_phase_corrected_gap(sp.phase_id),
    _erb_snap_sp_phase_strata_won_by_loser(sp.phase_id),
    _erb_snap_sp_phase_reversal_intensity(sp.phase_id),
    _erb_snap_sp_phase_is_sign_flip(sp.phase_id),
    _erb_snap_sp_phase_allocation_distortion(sp.phase_id),
    _erb_snap_sp_phase_distortion_type(sp.phase_id)
  FROM synthetic_phase sp;
END;
$$;

-- Thin calc_* overrides — O(1) cache lookups
CREATE OR REPLACE FUNCTION calc_treatment_rankings_name(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT name FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_total_cases_a(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT total_cases_a FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_total_successes_a(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT total_successes_a FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_rate_a(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT pooled_rate_a FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_total_cases_b(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT total_cases_b FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_total_successes_b(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT total_successes_b FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_rate_b(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT pooled_rate_b FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_winner(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT pooled_winner FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_stratum_count(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT stratum_count FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_strata_won_by_a(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT strata_won_by_a FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_strata_won_by_b(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT strata_won_by_b FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_per_stratum_winner(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT per_stratum_winner FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_is_reversal(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT is_reversal FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_confounders_in_study(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT confounders_in_study FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_is_paradox_explained(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT is_paradox_explained FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_gap(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT pooled_gap FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_strata_won_by_loser(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT strata_won_by_loser FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_paradox_strength(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT paradox_strength FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_rate_from_weights_a(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT pooled_rate_from_weights_a FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_rate_from_weights_b(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT pooled_rate_from_weights_b FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_reversal_intensity(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT reversal_intensity FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_threshold_margin(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT threshold_margin FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_signed_pooled_gap(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT signed_pooled_gap FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_weighted_stratum_gap_sum(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT weighted_stratum_gap_sum FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_is_sign_flip(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT is_sign_flip FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_allocation_distortion(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT allocation_distortion FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_distortion_type(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT distortion_type FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_policy_implication(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT policy_implication FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_corrected_gap(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT corrected_gap FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_corrected_winner(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT corrected_winner FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_corrected_vs_pooled_agreement(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT corrected_vs_pooled_agreement FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_corrected_policy_implication(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT corrected_policy_implication FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_allocation_direction(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT allocation_direction FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_signal_purity(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT signal_purity FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_gap_crosses_zero(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT pooled_gap_crosses_zero FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_sweep_pooled_gap_range(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_range FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_latent_flip_potential(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT latent_flip_potential FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_allocation_fragility(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT allocation_fragility FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_study_domain(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT study_domain FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_confirmed_causal_role_count(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT confirmed_causal_role_count FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_mediator_risk_count(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT mediator_risk_count FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_contested_stratum_count(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT contested_stratum_count FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_unknown_causal_role_count(p_treatment_ranking_id TEXT) RETURNS INTEGER AS $$
  SELECT unknown_causal_role_count FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_causal_claim_status(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT causal_claim_status FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_adjustment_appropriate(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT adjustment_appropriate FROM _erb_tr_metrics WHERE treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;

-- ModelSummary aggregates — single scan of _erb_tr_metrics
CREATE OR REPLACE FUNCTION calc_model_summary_reversal_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE is_reversal = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_non_reversal_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE is_reversal = false;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_study_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_explained_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE is_paradox_explained = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_total_paradox_strength(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COALESCE(SUM(paradox_strength), 0) FROM _erb_tr_metrics;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_avg_paradox_strength(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT AVG(paradox_strength) FROM _erb_tr_metrics;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_a_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type = 'A';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_b_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type = 'B';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_d_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type = 'D';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_a_fraction(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*) FILTER (WHERE distortion_type = 'A')::numeric / NULLIF(COUNT(*), 0) FROM _erb_tr_metrics;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_c_amplification_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type = 'C+';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_c_compression_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type = 'C-';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_avg_signal_purity(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT AVG(signal_purity) FROM _erb_tr_metrics;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_distortion_only_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM _erb_tr_metrics WHERE distortion_type LIKE 'C%';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_distortion_taxonomy_coverage(p_model_summary_id TEXT) RETURNS TEXT AS $$
  SELECT CONCAT(
    'A:', calc_model_summary_type_a_count(p_model_summary_id),
    ' B:', calc_model_summary_type_b_count(p_model_summary_id),
    ' C+:', calc_model_summary_c_amplification_count(p_model_summary_id),
    ' C-:', calc_model_summary_c_compression_count(p_model_summary_id),
    ' D:', calc_model_summary_type_d_count(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

-- loop-61 discovery aggregates — single scan of _erb_tr_metrics
CREATE OR REPLACE FUNCTION calc_model_summary_latent_type_d_count(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric FROM _erb_tr_metrics WHERE latent_flip_potential = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_stable_type_d_count(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric FROM _erb_tr_metrics WHERE distortion_type = 'D' AND latent_flip_potential = false;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_latent_type_d_fraction(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*) FILTER (WHERE latent_flip_potential = true)::numeric / NULLIF(COUNT(*) FILTER (WHERE distortion_type = 'D'), 0)
  FROM _erb_tr_metrics;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_cross_zero_count(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric FROM _erb_tr_metrics WHERE pooled_gap_crosses_zero = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_sign_flip_signal_purity_max(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT MAX(signal_purity) FROM _erb_tr_metrics WHERE is_sign_flip = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_economics_sign_flip_count(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric FROM _erb_tr_metrics WHERE study_domain = 'economics' AND is_sign_flip = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_avg_pooled_gap_latent_d(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT AVG(pooled_gap) FROM _erb_tr_metrics WHERE latent_flip_potential = true;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_avg_pooled_gap_stable_d(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT AVG(pooled_gap) FROM _erb_tr_metrics WHERE distortion_type = 'D' AND latent_flip_potential = false;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_epidemiology_avg_distortion(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT AVG(allocation_distortion) FROM _erb_tr_metrics WHERE study_domain = 'epidemiology';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_education_avg_distortion(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT AVG(allocation_distortion) FROM _erb_tr_metrics WHERE study_domain = 'education';
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_discovery_witness_note(p_model_summary_id TEXT) RETURNS TEXT AS $$
  SELECT CONCAT(
    'LatentTypeD=', calc_model_summary_latent_type_d_count(p_model_summary_id),
    '/', calc_model_summary_type_d_count(p_model_summary_id),
    '; SignFlipMaxPurity=', calc_model_summary_sign_flip_signal_purity_max(p_model_summary_id),
    '; stableD=', calc_model_summary_avg_pooled_gap_stable_d(p_model_summary_id),
    ' latentD=', calc_model_summary_avg_pooled_gap_latent_d(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

-- loop-61 discovery findings
CREATE OR REPLACE FUNCTION calc_discovery_findings_is_confirmed(p_finding_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE (SELECT hypothesis_id FROM discovery_findings WHERE finding_id = p_finding_id)
    WHEN 'H-latent-d' THEN calc_model_summary_latent_type_d_fraction('simpsons-paradox-v1') > 0.5
    WHEN 'H-purity' THEN calc_model_summary_sign_flip_signal_purity_max('simpsons-paradox-v1') < 0.5
    WHEN 'H-small-effect' THEN calc_model_summary_avg_pooled_gap_stable_d('simpsons-paradox-v1')
                              > calc_model_summary_avg_pooled_gap_latent_d('simpsons-paradox-v1')
    WHEN 'H-econ-zero' THEN calc_model_summary_economics_sign_flip_count('simpsons-paradox-v1') = 0
    WHEN 'H-domain-dist' THEN calc_model_summary_epidemiology_avg_distortion('simpsons-paradox-v1')
                              > calc_model_summary_education_avg_distortion('simpsons-paradox-v1')
    ELSE FALSE
  END;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_discovery_findings_observed_metric(p_finding_id TEXT) RETURNS TEXT AS $$
  SELECT CASE (SELECT hypothesis_id FROM discovery_findings WHERE finding_id = p_finding_id)
    WHEN 'H-latent-d' THEN CONCAT('fraction=', calc_model_summary_latent_type_d_fraction('simpsons-paradox-v1'))
    WHEN 'H-purity' THEN CONCAT('maxPurity=', calc_model_summary_sign_flip_signal_purity_max('simpsons-paradox-v1'))
    WHEN 'H-small-effect' THEN CONCAT('stable=', calc_model_summary_avg_pooled_gap_stable_d('simpsons-paradox-v1'),
                                      ' latent=', calc_model_summary_avg_pooled_gap_latent_d('simpsons-paradox-v1'))
    WHEN 'H-econ-zero' THEN CONCAT('flips=', calc_model_summary_economics_sign_flip_count('simpsons-paradox-v1'))
    WHEN 'H-domain-dist' THEN CONCAT('epi=', calc_model_summary_epidemiology_avg_distortion('simpsons-paradox-v1'),
                                     ' edu=', calc_model_summary_education_avg_distortion('simpsons-paradox-v1'))
    ELSE ''
  END;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_discovery_findings_evidence(p_finding_id TEXT) RETURNS TEXT AS $$
  SELECT CASE
    WHEN calc_discovery_findings_is_confirmed(p_finding_id) THEN
      CONCAT('PASS: ', calc_discovery_findings_observed_metric(p_finding_id))
    WHEN calc_discovery_findings_is_confirmed(p_finding_id) = FALSE THEN
      CONCAT('FAIL: ', calc_discovery_findings_observed_metric(p_finding_id))
    ELSE ''
  END;
$$ LANGUAGE sql STABLE;

-- SyntheticPhase calc_* overrides
CREATE OR REPLACE FUNCTION calc_synthetic_phase_name(p_phase_id TEXT) RETURNS TEXT AS $$
  SELECT name FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_s1_total(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_s1_total FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_na1(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_na1 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_nb1(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_nb1 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_na2(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_na2 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_nb2(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_nb2 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_rate_a1(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_rate_a1 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_rate_b1(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_rate_b1 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_rate_a2(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_rate_a2 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_rate_b2(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_rate_b2 FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_pooled_rate_a(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_pooled_rate_a FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_pooled_rate_b(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_pooled_rate_b FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_signed_pooled_gap(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_signed_pooled_gap FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_corrected_gap(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_corrected_gap FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_strata_won_by_loser(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_strata_won_by_loser FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_reversal_intensity(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_reversal_intensity FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_is_sign_flip(p_phase_id TEXT) RETURNS BOOLEAN AS $$
  SELECT phase_is_sign_flip FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_allocation_distortion(p_phase_id TEXT) RETURNS NUMERIC AS $$
  SELECT phase_allocation_distortion FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_synthetic_phase_phase_distortion_type(p_phase_id TEXT) RETURNS TEXT AS $$
  SELECT phase_distortion_type FROM _erb_sp_metrics WHERE phase_id = p_phase_id;
$$ LANGUAGE sql STABLE;
