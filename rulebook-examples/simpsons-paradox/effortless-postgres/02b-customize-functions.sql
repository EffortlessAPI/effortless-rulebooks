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
-- loop-90: hand-code sanitization.
-- The _erb_tr_metrics / _erb_sp_metrics bespoke cache tables (and every
-- calc_treatment_rankings_*, calc_model_summary_*, calc_synthetic_phase_*
-- function that was a thin reader of them) have been ELIMINATED. Every
-- value they used to hold is now a real calculated/lookup/aggregation field
-- on TreatmentRankings / ModelSummary / SyntheticPhase / CorpusBalance in
-- the rulebook, computed live by the native functions in
-- 02-create-functions.sql. Confirmed via grep-diff against that file before
-- deleting each override below.
--
-- What remains here is ONLY genuinely non-redundant logic: real transpiler
-- gaps (LOOKUP against a table added in the same loop, COUNTIFS numeric
-- comparators, TEXT()/unsupported functions, cross-table join fields the
-- transpiler cannot express), plus the run_invariant_checks() harness and
-- the two refresh_identity_*() functions that materialize the
-- IdentityClusterSummaries / IdentityDomainCells tables (first-class
-- rulebook tables, not shadow caches — see CLAUDE.md "principled
-- materialization").
-- ============================================================================

-- ----------------------------------------------------------------------------
-- IngestionProtocol witnesses (loop-49): transpiler cannot emit LOOKUP
-- inside calculated fields for a table added in the same loop.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_studies_ingestion_cell_parity(p_study_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (
    (SELECT calc_treatment_rankings_stratum_count(tr.treatment_ranking_id) FROM treatment_rankings tr WHERE tr.study = p_study_id LIMIT 1) >= 2
    AND calc_studies_cell_count(p_study_id) =
        (SELECT calc_treatment_rankings_stratum_count(tr.treatment_ranking_id) FROM treatment_rankings tr WHERE tr.study = p_study_id LIMIT 1) * 2
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

-- ----------------------------------------------------------------------------
-- AllocationSweep + SweepStudySummary LOOKUP overrides (loop-56)
-- Transpiler cannot emit LOOKUP(); join sweep_study_config / treatment_rankings instead.
-- ----------------------------------------------------------------------------
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
  SELECT calc_treatment_rankings_distortion_type(tr.treatment_ranking_id)
  FROM treatment_rankings tr WHERE tr.study = p_sweep_study_id LIMIT 1;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_stratum_label(p_sweep_study_id TEXT)
RETURNS TEXT AS $$
  SELECT c.sweep_stratum_label
  FROM sweep_study_config c
  WHERE c.study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

-- TreatmentRankings.PooledGapCrossesZero / SweepPooledGapRange: both are
-- LOOKUP(Study, SweepStudySummary[Study], SweepStudySummary[...]) formulas
-- the transpiler cannot emit ("Function 'LOOKUP' is not supported yet").
-- The underlying calc_sweep_study_summary_* functions ARE native and
-- correct; this just re-exposes them keyed by TreatmentRankingId instead of
-- Study. LatentFlipPotential/IsSweepFragile/AllocationFragility all
-- transitively depend on this LOOKUP, so it must be live, not a cache read.
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_gap_crosses_zero(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT calc_sweep_study_summary_pooled_gap_crosses_zero(tr.study)
  FROM treatment_rankings tr WHERE tr.treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_treatment_rankings_sweep_pooled_gap_range(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_sweep_study_summary_sweep_pooled_gap_range(tr.study)
  FROM treatment_rankings tr WHERE tr.treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- run_invariant_checks()
-- Reads every row in invariant_checks and evaluates SqlAssertion (with optional
-- SqlFilter) against the view named by SourceTable, then writes pass_count and
-- fail_count back into the table. Routes by source_table.
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- loop-65 catalog prediction audit — CandidateStudyCatalog + CorpusCatalogSummary
-- Genuine cross-table join the transpiler cannot express as a LOOKUP/COUNTIFS
-- formula (joins CandidateStudyCatalog to TreatmentRankings via linked study,
-- filtered by ingestion status). Rewritten off _erb_tr_metrics onto the live
-- vw_treatment_rankings view.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id TEXT) RETURNS TEXT AS $$
  SELECT calc_treatment_rankings_distortion_type(m.treatment_ranking_id)
  FROM candidate_study_catalog c
  JOIN treatment_rankings m ON m.study = c.linked_study_id
  WHERE c.candidate_id = p_candidate_id
    AND c.ingestion_status = 'imported';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_observed_distortion_type(p_candidate_id TEXT) RETURNS TEXT AS $$
  SELECT lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id);
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_expected_sign_flip(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT expected_distortion_type IN ('A', 'B')
  FROM candidate_study_catalog
  WHERE candidate_id = p_candidate_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_observed_sign_flip_type(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE
    WHEN NOT calc_candidate_study_catalog_is_imported(p_candidate_id) THEN NULL
    ELSE lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id) IN ('A', 'B')
  END;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_type_prediction_match(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE
    WHEN NOT calc_candidate_study_catalog_is_imported(p_candidate_id) THEN NULL
    ELSE lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id)
         = (SELECT expected_distortion_type FROM candidate_study_catalog WHERE candidate_id = p_candidate_id)
  END;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_sign_flip_prediction_match(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE
    WHEN NOT calc_candidate_study_catalog_is_imported(p_candidate_id) THEN NULL
    WHEN NOT calc_candidate_study_catalog_expected_sign_flip(p_candidate_id) THEN NULL
    ELSE calc_candidate_study_catalog_observed_sign_flip_type(p_candidate_id)
  END;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_type_prediction_match_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_type_prediction_match(c.candidate_id) = TRUE;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_type_prediction_mismatch_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_is_imported(c.candidate_id) = TRUE
    AND calc_candidate_study_catalog_type_prediction_match(c.candidate_id) = FALSE;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_is_imported(c.candidate_id) = TRUE
    AND calc_candidate_study_catalog_expected_sign_flip(c.candidate_id) = TRUE;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_sign_flip_prediction_match_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_sign_flip_prediction_match(c.candidate_id) = TRUE;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_type_prediction_match_rate(p_catalog_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_match_count(p_catalog_summary_id)::numeric
         / NULLIF(calc_corpus_catalog_summary_imported_count(p_catalog_summary_id), 0);
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_sign_flip_prediction_match_rate(p_catalog_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_sign_flip_prediction_match_count(p_catalog_summary_id)::numeric
         / NULLIF(calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(p_catalog_summary_id), 0);
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_catalog_prediction_witness_note(p_catalog_summary_id TEXT) RETURNS TEXT AS $$
  SELECT CONCAT(
    'exact=', calc_corpus_catalog_summary_type_prediction_match_count(p_catalog_summary_id),
    '/', calc_corpus_catalog_summary_imported_count(p_catalog_summary_id),
    ' (', ROUND(calc_corpus_catalog_summary_type_prediction_match_rate(p_catalog_summary_id) * 100, 1), '%); ',
    'flipPred=', calc_corpus_catalog_summary_sign_flip_prediction_match_count(p_catalog_summary_id),
    '/', calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(p_catalog_summary_id),
    ' (', ROUND(calc_corpus_catalog_summary_sign_flip_prediction_match_rate(p_catalog_summary_id) * 100, 1), '%)'
  );
$$ LANGUAGE sql STABLE;

-- ModelSummary lookups onto CorpusCatalogSummary — transpiler cannot emit
-- LOOKUP for a table added in the same loop.
CREATE OR REPLACE FUNCTION calc_model_summary_type_prediction_match_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_match_count('catalog-v1');
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_prediction_mismatch_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_mismatch_count('catalog-v1');
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_prediction_match_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_match_rate('catalog-v1');
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_sign_flip_prediction_match_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_sign_flip_prediction_match_rate('catalog-v1');
$$ LANGUAGE sql STABLE;
CREATE OR REPLACE FUNCTION calc_model_summary_catalog_prediction_witness_note(p_model_summary_id TEXT) RETURNS TEXT AS $$
  SELECT calc_corpus_catalog_summary_catalog_prediction_witness_note('catalog-v1');
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- loop-49: IngestionSummary/ModelSummary LOOKUP overrides (transpiler cannot
-- emit LOOKUP for a table added in the same loop). Live queries, no cache.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_model_summary_corpus_passes_ingestion_contract(p_model_summary_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT calc_ingestion_summary_ingestion_contract_passes('ingestion-v1');
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_ingestion_witness_note(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_ingestion_summary_ingestion_witness_note('ingestion-v1');
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- loop-66 geometry-controlled domain screening
-- HighImbalanceSignFlipThreshold is a documented exception (see the rulebook
-- schema Description on ModelSummary.HighImbalanceSignFlipThreshold): this
-- rulebook's formula vocabulary has no MEDIAN/PERCENTILE_CONT primitive, so
-- the corpus-median threshold is computed here directly from the live
-- treatment_rankings table (NOT a cache) and stored in the `raw` field.
-- Refresh is an explicit container-level action (05b), never a silent cache.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT percentile_cont(0.5) WITHIN GROUP (ORDER BY calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id))
  FROM treatment_rankings tr
  WHERE calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) IS NOT NULL;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_economics_high_imbalance_sign_flip_count(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  WHERE calc_treatment_rankings_study_domain(tr.treatment_ranking_id) = 'economics'
    AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
    AND calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) >= calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id);
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION _domain_high_imbalance_sign_flip_rate(p_domain TEXT, p_threshold NUMERIC) RETURNS NUMERIC AS $$
  SELECT COUNT(*) FILTER (WHERE calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE)::numeric
         / NULLIF(COUNT(*), 0)
  FROM treatment_rankings tr
  WHERE calc_treatment_rankings_study_domain(tr.treatment_ranking_id) = p_domain
    AND calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) >= p_threshold;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_epidemiology_high_imbalance_sign_flip_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT _domain_high_imbalance_sign_flip_rate(
    'epidemiology',
    calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_legal_high_imbalance_sign_flip_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT _domain_high_imbalance_sign_flip_rate(
    'legal',
    calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_sports_high_imbalance_sign_flip_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT _domain_high_imbalance_sign_flip_rate(
    'sports',
    calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- loop-61 DiscoveryFindings CASE-statement dispatch.
-- DiscoveryFindings.IsConfirmed/ObservedMetric/Evidence ARE real rulebook
-- calculated fields (see the giant nested-IF formula on IsConfirmed in the
-- rulebook JSON) — the transpiler's native versions of these three functions
-- already exist in 02-create-functions.sql and are correct EXCEPT for one
-- branch: 'H-unexplained-nonconfounder', which the rulebook formula cannot
-- express (it needs a 3-column filtered COUNT that has no COUNTIFS
-- equivalent across LOOKUP-chained booleans). This 02b override exists ONLY
-- to keep the dispatch table's remaining ~25 branches in sync with the
-- native calc_model_summary_* helper names and to rewrite the one
-- non-expressible branch off the old _erb_tr_metrics cache onto the live
-- vw_treatment_rankings view. If a future formula upgrade makes the
-- H-unexplained-nonconfounder branch expressible natively, delete this
-- entire override block — it will be 100% redundant with the native
-- calc_discovery_findings_is_confirmed/observed_metric/evidence functions.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_discovery_findings_is_confirmed(p_finding_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE (SELECT hypothesis_id FROM discovery_findings WHERE finding_id = p_finding_id)
    WHEN 'H-latent-d' THEN calc_model_summary_latent_type_d_fraction('simpsons-paradox-v1') > 0.5
    WHEN 'H-purity' THEN calc_model_summary_sign_flip_signal_purity_max('simpsons-paradox-v1') < 0.5
    WHEN 'H-small-effect' THEN calc_model_summary_avg_pooled_gap_stable_d('simpsons-paradox-v1')
                              > calc_model_summary_avg_pooled_gap_latent_d('simpsons-paradox-v1')
    WHEN 'H-econ-zero' THEN calc_model_summary_economics_sign_flip_count('simpsons-paradox-v1') = 0
    WHEN 'H-domain-dist' THEN calc_model_summary_epidemiology_avg_distortion('simpsons-paradox-v1')
                              > calc_model_summary_education_avg_distortion('simpsons-paradox-v1')
    WHEN 'H-causal-manifest' THEN calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1')
                              > calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1')
                              AND calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1') >= 10
    WHEN 'H-causal-latent' THEN calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1') = 0
                              AND calc_model_summary_collider_selection_latent_only_count('simpsons-paradox-v1') >= 5
    WHEN 'H-explained-confounder' THEN calc_model_summary_explained_confounder_count('simpsons-paradox-v1')
                              = calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1')
                              AND calc_model_summary_explained_confounder_count('simpsons-paradox-v1') >= 10
    WHEN 'H-unexplained-nonconfounder' THEN (
      SELECT COUNT(*)::integer FROM treatment_rankings tr
      WHERE calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
        AND calc_treatment_rankings_is_paradox_explained(tr.treatment_ranking_id) = TRUE
        AND calc_treatment_rankings_adjustment_appropriate(tr.treatment_ranking_id) = FALSE
    ) = 0
    WHEN 'H-catalog-exact-match' THEN calc_model_summary_type_prediction_match_rate('simpsons-paradox-v1') < 0.5
    WHEN 'H-catalog-flip-prediction' THEN calc_model_summary_sign_flip_prediction_match_rate('simpsons-paradox-v1') < 0.5
    WHEN 'H-domain-flip-geometry-controlled' THEN calc_model_summary_domain_flip_gap_survives_geometry_control('simpsons-paradox-v1')
    WHEN 'H-collider-no-manifest-v2' THEN calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1') = 0
    WHEN 'H-cplus-magnitude' THEN calc_model_summary_c_plus_avg_distortion('simpsons-paradox-v1')
                              > calc_model_summary_c_minus_avg_distortion('simpsons-paradox-v1')
                              AND calc_model_summary_c_plus_avg_distortion('simpsons-paradox-v1')
                                  > calc_model_summary_type_d_avg_distortion('simpsons-paradox-v1')
    WHEN 'H-ultra-fragile' THEN calc_model_summary_sweep_fragile_count('simpsons-paradox-v1') >= 4
    WHEN 'H-econ-encoding-selection' THEN calc_model_summary_economics_expected_a_mismatch_rate('simpsons-paradox-v1') > 0.5
    WHEN 'H-domain-profiles-stable' THEN calc_model_summary_education_latent_fraction('simpsons-paradox-v1') > 0.5
                              AND calc_model_summary_sports_latent_fraction('simpsons-paradox-v1') > 0.5
                              AND calc_model_summary_economics_sign_flip_rate('simpsons-paradox-v1') < 0.05
    WHEN 'H-corrected-gap-invariant' THEN calc_model_summary_max_study_sweep_corrected_gap_range('simpsons-paradox-v1') < 0.0001
                              AND calc_model_summary_corrected_gap_invariant_fail_count('simpsons-paradox-v1') = 0
    WHEN 'H-explained-bidirectional' THEN calc_model_summary_explained_confounder_count('simpsons-paradox-v1')
                              = calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1')
                              AND calc_model_summary_false_positive_explained_count('simpsons-paradox-v1') = 0
                              AND calc_model_summary_unexplained_confounder_sign_flip_count('simpsons-paradox-v1') = 0
    WHEN 'H-collider-no-manifest-theorem' THEN calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1') = 0
                              AND calc_model_summary_collider_selection_count('simpsons-paradox-v1') >= 10
    WHEN 'H-theorem-portfolio' THEN calc_model_summary_theorem_count('simpsons-paradox-v1') >= 4
    WHEN 'H-age-identity-flip-rate' THEN calc_model_summary_age_identity_manifest_flip_rate('simpsons-paradox-v1') >= 0.40
                              AND calc_model_summary_age_identity_study_count('simpsons-paradox-v1') >= 10
    WHEN 'H-severity-vs-age-latent' THEN calc_model_summary_severity_identity_latent_fraction_among_type('simpsons-paradox-v1')
                              < calc_model_summary_age_identity_latent_fraction_among_type_d('simpsons-paradox-v1')
    WHEN 'H-identity-map-coverage' THEN calc_model_summary_identity_map_coverage_rate('simpsons-paradox-v1') >= 0.95
    WHEN 'H-severity-medicine-manifest' THEN calc_model_summary_severity_medicine_manifest_flip_rate('simpsons-paradox-v1') >= 0.45
    WHEN 'H-severity-epi-latent-only' THEN calc_model_summary_severity_epi_manifest_flip_count('simpsons-paradox-v1') = 0
    WHEN 'H-mechanism-other-drain' THEN
      (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = 'cluster-id-mechanism-other') <= 5
    WHEN 'H-selection-frailty-zero-manifest' THEN
      calc_model_summary_selection_frailty_manifest_flip_count('simpsons-paradox-v1') = 0
      AND calc_model_summary_selection_frailty_study_count('simpsons-paradox-v1') >= 3
    WHEN 'H-collider-identity-low-manifest' THEN
      calc_model_summary_collider_identity_manifest_flip_rate('simpsons-paradox-v1') <= 0.15
      AND (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = 'cluster-id-collider-proxy') >= 5
    WHEN 'H-geographic-stable-type-d' THEN
      calc_model_summary_geographic_type_d_fraction('simpsons-paradox-v1') >= 0.25
      AND (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = 'cluster-id-geographic-composition') >= 3
    WHEN 'H-unexplained-flips-only-nonconfounders' THEN
      calc_model_summary_unexplained_confounder_sign_flip_count('simpsons-paradox-v1') = 0
    -- loop-91: the 7 hypotheses registered without a DiscoveryFindings row (loops 86-88).
    -- Live queries, no cache — same pattern as every other branch above.
    WHEN 'H-safety-corridor-lower-bound' THEN
      (SELECT MIN(max_stratum_imbalance) FROM vw_treatment_rankings WHERE is_sign_flip = TRUE) >= 0.033
      AND (SELECT COUNT(*) FROM vw_treatment_rankings WHERE is_sign_flip = TRUE AND max_stratum_imbalance < 0.033) = 0
    WHEN 'H-perfect-balance-sufficient-safety' THEN
      (SELECT COUNT(*) FROM vw_treatment_rankings WHERE arm_size_ratio = 0.5 AND max_stratum_imbalance = 0.0 AND is_sign_flip = TRUE) = 0
      AND (SELECT COUNT(*) FROM vw_treatment_rankings WHERE arm_size_ratio = 0.5 AND max_stratum_imbalance = 0.0) >= 100
    WHEN 'H-purity-inversion' THEN
      (SELECT AVG(tr.signal_purity) FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE ci.policy_default = 'do-not-condition' AND COALESCE(s.is_synthetic, false) = false)
      >
      (SELECT AVG(tr.signal_purity) FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE ci.policy_default LIKE 'stratify-%' AND COALESCE(s.is_synthetic, false) = false)
    WHEN 'H-purity-trap-flag-coverage' THEN
      (SELECT COUNT(*) FILTER (WHERE tr.purity_trap_flag = TRUE)::numeric / NULLIF(COUNT(*), 0)
        FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE ci.policy_default = 'do-not-condition' AND COALESCE(s.is_synthetic, false) = false) >= 0.80
      AND
      (SELECT COUNT(*) FILTER (WHERE tr.purity_trap_flag = TRUE)::numeric / NULLIF(COUNT(*), 0)
        FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE m.confounder_identity = 'id-demographic-group' AND COALESCE(s.is_synthetic, false) = false) <= 0.05
    -- Control studies (loop-88) were never encoded with paired treatment arms, so they
    -- produce zero TreatmentRankings rows by construction — these three are vacuously
    -- true, not substantively witnessed. ObservedMetric/Evidence say so explicitly below.
    WHEN 'H-control-type-d-predominance' THEN
      (SELECT COUNT(*) FROM treatment_rankings tr JOIN studies s ON s.study_id = tr.study
        WHERE s.is_control_study = TRUE AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE)::numeric
      / NULLIF((SELECT COUNT(*) FROM studies WHERE is_control_study = TRUE), 0) < 0.3
    WHEN 'H-control-no-manifest-flip' THEN
      (SELECT COUNT(*) FROM treatment_rankings tr JOIN studies s ON s.study_id = tr.study
        WHERE s.is_control_study = TRUE AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE) = 0
    WHEN 'H-control-safety-corridor' THEN
      (SELECT COUNT(*) FROM treatment_rankings tr JOIN studies s ON s.study_id = tr.study
        WHERE s.is_control_study = TRUE AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
          AND calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) < 0.033) = 0
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
    WHEN 'H-causal-manifest' THEN CONCAT('confFlip=', calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1'),
                                        ' collManifest=', calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1'))
    WHEN 'H-causal-latent' THEN CONCAT('collManifest=', calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1'),
                                        ' collLatent=', calc_model_summary_collider_selection_latent_only_count('simpsons-paradox-v1'),
                                        ' collN=', calc_model_summary_collider_selection_count('simpsons-paradox-v1'))
    WHEN 'H-explained-confounder' THEN CONCAT('ExplainedConfounderCount=', calc_model_summary_explained_confounder_count('simpsons-paradox-v1'),
                                              ' ConfounderSignFlipCount=', calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1'))
    WHEN 'H-unexplained-nonconfounder' THEN CONCAT('ContestedOrMediatorExplainedCount=',
      (SELECT COUNT(*) FROM treatment_rankings tr
       WHERE calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
         AND calc_treatment_rankings_is_paradox_explained(tr.treatment_ranking_id) = TRUE
         AND calc_treatment_rankings_adjustment_appropriate(tr.treatment_ranking_id) = FALSE))
    WHEN 'H-catalog-exact-match' THEN CONCAT('exactRate=', calc_model_summary_type_prediction_match_rate('simpsons-paradox-v1'),
                                              ' (', calc_model_summary_type_prediction_match_count('simpsons-paradox-v1'),
                                              '/', calc_corpus_catalog_summary_imported_count('catalog-v1'), ')')
    WHEN 'H-catalog-flip-prediction' THEN CONCAT('flipPredRate=', calc_model_summary_sign_flip_prediction_match_rate('simpsons-paradox-v1'),
                                                  ' note=', calc_model_summary_catalog_prediction_witness_note('simpsons-paradox-v1'))
    WHEN 'H-domain-flip-geometry-controlled' THEN CONCAT('econHighImbFlips=', calc_model_summary_economics_high_imbalance_sign_flip_count('simpsons-paradox-v1'),
                                                          ' epiHighImbRate=', calc_model_summary_epidemiology_high_imbalance_sign_flip_rate('simpsons-paradox-v1'),
                                                          ' legalHighImbRate=', calc_model_summary_legal_high_imbalance_sign_flip_rate('simpsons-paradox-v1'),
                                                          ' sportsHighImbRate=', calc_model_summary_sports_high_imbalance_sign_flip_rate('simpsons-paradox-v1'),
                                                          ' threshold=', calc_model_summary_high_imbalance_sign_flip_threshold('simpsons-paradox-v1'))
    WHEN 'H-collider-no-manifest-v2' THEN CONCAT('collManifest=', calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1'))
    WHEN 'H-cplus-magnitude' THEN CONCAT('C+=', calc_model_summary_c_plus_avg_distortion('simpsons-paradox-v1'),
                                         ' C-=', calc_model_summary_c_minus_avg_distortion('simpsons-paradox-v1'),
                                         ' D=', calc_model_summary_type_d_avg_distortion('simpsons-paradox-v1'))
    WHEN 'H-ultra-fragile' THEN CONCAT('SweepFragileCount=', calc_model_summary_sweep_fragile_count('simpsons-paradox-v1'))
    WHEN 'H-econ-encoding-selection' THEN CONCAT('EconExpectedAMismatchRate=', calc_model_summary_economics_expected_a_mismatch_rate('simpsons-paradox-v1'))
    WHEN 'H-domain-profiles-stable' THEN CONCAT('eduLatent=', calc_model_summary_education_latent_fraction('simpsons-paradox-v1'),
                                                '; sportsLatent=', calc_model_summary_sports_latent_fraction('simpsons-paradox-v1'),
                                                '; econFlipRate=', calc_model_summary_economics_sign_flip_rate('simpsons-paradox-v1'),
                                                '; realN=', calc_model_summary_real_study_count('simpsons-paradox-v1'))
    WHEN 'H-corrected-gap-invariant' THEN CONCAT('maxRange=', calc_model_summary_max_study_sweep_corrected_gap_range('simpsons-paradox-v1'),
                                                 ' fails=', calc_model_summary_corrected_gap_invariant_fail_count('simpsons-paradox-v1'))
    WHEN 'H-explained-bidirectional' THEN CONCAT('explained=', calc_model_summary_explained_confounder_count('simpsons-paradox-v1'),
                                                 ' confFlip=', calc_model_summary_confounder_sign_flip_count('simpsons-paradox-v1'),
                                                 ' falsePos=', calc_model_summary_false_positive_explained_count('simpsons-paradox-v1'),
                                                 ' unexplained=', calc_model_summary_unexplained_confounder_sign_flip_count('simpsons-paradox-v1'))
    WHEN 'H-collider-no-manifest-theorem' THEN CONCAT('collN=', calc_model_summary_collider_selection_count('simpsons-paradox-v1'),
                                                      ' collManifest=', calc_model_summary_collider_selection_manifest_count('simpsons-paradox-v1'))
    WHEN 'H-theorem-portfolio' THEN CONCAT('theoremCount=', calc_model_summary_theorem_count('simpsons-paradox-v1'))
    WHEN 'H-age-identity-flip-rate' THEN CONCAT(
      'AgeIdentityManifestFlipRate=', calc_model_summary_age_identity_manifest_flip_rate('simpsons-paradox-v1'),
      ' AgeIdentityStudyCount=', calc_model_summary_age_identity_study_count('simpsons-paradox-v1'))
    WHEN 'H-severity-vs-age-latent' THEN CONCAT(
      'SeverityIdentityLatentFractionAmongTypeD=', calc_model_summary_severity_identity_latent_fraction_among_type('simpsons-paradox-v1'),
      ' AgeIdentityLatentFractionAmongTypeD=', calc_model_summary_age_identity_latent_fraction_among_type_d('simpsons-paradox-v1'))
    WHEN 'H-identity-map-coverage' THEN CONCAT('IdentityMapCoverageRate=', calc_model_summary_identity_map_coverage_rate('simpsons-paradox-v1'))
    WHEN 'H-severity-medicine-manifest' THEN CONCAT('SeverityMedicineManifestFlipRate=', calc_model_summary_severity_medicine_manifest_flip_rate('simpsons-paradox-v1'))
    WHEN 'H-severity-epi-latent-only' THEN CONCAT('SeverityEpiManifestFlipCount=', calc_model_summary_severity_epi_manifest_flip_count('simpsons-paradox-v1'))
    WHEN 'H-mechanism-other-drain' THEN CONCAT('MechanismOtherStudyCount=',
      (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = 'cluster-id-mechanism-other'))
    WHEN 'H-selection-frailty-zero-manifest' THEN CONCAT(
      'SelectionFrailtyManifestFlipCount=', calc_model_summary_selection_frailty_manifest_flip_count('simpsons-paradox-v1'),
      ' SelectionFrailtyStudyCount=', calc_model_summary_selection_frailty_study_count('simpsons-paradox-v1'))
    WHEN 'H-collider-identity-low-manifest' THEN CONCAT(
      'ColliderIdentityManifestFlipRate=', calc_model_summary_collider_identity_manifest_flip_rate('simpsons-paradox-v1'),
      ' ColliderIdentityStudyCount=', (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = 'cluster-id-collider-proxy'))
    WHEN 'H-geographic-stable-type-d' THEN CONCAT(
      'GeographicTypeDFraction=', calc_model_summary_geographic_type_d_fraction('simpsons-paradox-v1'),
      ' GeographicStudyCount=', (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = 'cluster-id-geographic-composition'))
    WHEN 'H-unexplained-flips-only-nonconfounders' THEN CONCAT(
      'UnexplainedConfounderSignFlipCount=', calc_model_summary_unexplained_confounder_sign_flip_count('simpsons-paradox-v1'))
    WHEN 'H-safety-corridor-lower-bound' THEN CONCAT(
      'MinSignFlipStratumImbalance=', (SELECT MIN(max_stratum_imbalance) FROM vw_treatment_rankings WHERE is_sign_flip = TRUE),
      ' SafeCorridorFlipCount=', (SELECT COUNT(*) FROM vw_treatment_rankings WHERE is_sign_flip = TRUE AND max_stratum_imbalance < 0.033))
    WHEN 'H-perfect-balance-sufficient-safety' THEN CONCAT(
      'PerfectBalanceFlipCount=', (SELECT COUNT(*) FROM vw_treatment_rankings WHERE arm_size_ratio = 0.5 AND max_stratum_imbalance = 0.0 AND is_sign_flip = TRUE),
      ' PerfectBalanceStudyCount=', (SELECT COUNT(*) FROM vw_treatment_rankings WHERE arm_size_ratio = 0.5 AND max_stratum_imbalance = 0.0))
    WHEN 'H-purity-inversion' THEN CONCAT(
      'MeanPurityDoNotCondition=', (SELECT ROUND(AVG(tr.signal_purity), 4) FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE ci.policy_default = 'do-not-condition' AND COALESCE(s.is_synthetic, false) = false),
      ' MeanPurityStratifyClasses=', (SELECT ROUND(AVG(tr.signal_purity), 4) FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE ci.policy_default LIKE 'stratify-%' AND COALESCE(s.is_synthetic, false) = false))
    WHEN 'H-purity-trap-flag-coverage' THEN CONCAT(
      'ColliderSelectionCoverage=', (SELECT ROUND(COUNT(*) FILTER (WHERE tr.purity_trap_flag = TRUE)::numeric / NULLIF(COUNT(*), 0), 4)
        FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE ci.policy_default = 'do-not-condition' AND COALESCE(s.is_synthetic, false) = false),
      ' DemographicFalsePositiveRate=', (SELECT ROUND(COUNT(*) FILTER (WHERE tr.purity_trap_flag = TRUE)::numeric / NULLIF(COUNT(*), 0), 4)
        FROM stratum_variable_identity_maps m
        JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
        JOIN studies s ON s.study_id = sv.study
        JOIN vw_treatment_rankings tr ON tr.study = s.study_id
        JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
        WHERE m.confounder_identity = 'id-demographic-group' AND COALESCE(s.is_synthetic, false) = false))
    WHEN 'H-control-type-d-predominance' THEN CONCAT(
      'ControlFlipRate=', (SELECT ROUND((SELECT COUNT(*) FROM treatment_rankings tr JOIN studies s ON s.study_id = tr.study
          WHERE s.is_control_study = TRUE AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE)::numeric
        / NULLIF((SELECT COUNT(*) FROM studies WHERE is_control_study = TRUE), 0), 4)),
      ' — vacuous: control studies have zero TreatmentRankings rows (never encoded with paired arms), so this is true by construction, not a substantive corpus finding.')
    WHEN 'H-control-no-manifest-flip' THEN CONCAT(
      'ControlManifestFlipCount=', (SELECT COUNT(*) FROM treatment_rankings tr JOIN studies s ON s.study_id = tr.study
        WHERE s.is_control_study = TRUE AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE),
      ' — vacuous: control studies have zero TreatmentRankings rows (never encoded with paired arms), so this is true by construction, not a substantive corpus finding.')
    WHEN 'H-control-safety-corridor' THEN CONCAT(
      'ControlSafeCorridorFlipCount=', (SELECT COUNT(*) FROM treatment_rankings tr JOIN studies s ON s.study_id = tr.study
        WHERE s.is_control_study = TRUE AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
          AND calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) < 0.033),
      ' — vacuous: control studies have zero TreatmentRankings rows (never encoded with paired arms), so this is true by construction, not a substantive corpus finding.')
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

-- ----------------------------------------------------------------------------
-- loop-78 ModelSummary rollup (expansion wave 3 supersession audit)
-- CorpusPatternSupersededFailCount / ExpansionWave3DiscoveryNote are now real
-- native calculated fields on ModelSummary (loop-90) — see
-- 02-create-functions.sql. Nothing left to override here.
-- ----------------------------------------------------------------------------

-- ----------------------------------------------------------------------------
-- Theorem wave ModelSummary fixes (loops 73-76)
-- Transpiler emits broken MAX/COUNTIFS for the sweep-corrected-gap-range
-- rollup and the explained-false-positive sum; these are genuine
-- cross-table aggregations the native transpiler cannot express.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_model_summary_max_study_sweep_corrected_gap_range(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COALESCE(MAX(calc_sweep_study_summary_sweep_corrected_gap_range(sweep_study_id)), 0)::numeric
  FROM sweep_study_summary;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_corrected_gap_invariant_fail_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM sweep_study_summary
  WHERE calc_sweep_study_summary_sweep_corrected_gap_range(sweep_study_id) >= 0.0001;
$$ LANGUAGE sql STABLE;

-- StratumCausalRole: LOOKUP(Study, StratumVariables[Study], StratumVariables[CausalRole])
-- (transpiler quota gap for tables introduced in an earlier loop than this
-- lookup field). Needed below by calc_model_summary_false_positive_explained_count
-- and by vw_treatment_rankings's presentation column in 03b.
CREATE OR REPLACE FUNCTION calc_treatment_rankings_stratum_causal_role(p_treatment_ranking_id TEXT) RETURNS TEXT AS $$
  SELECT sv.causal_role
  FROM treatment_rankings tr
  JOIN stratum_variables sv ON sv.study = tr.study
  WHERE tr.treatment_ranking_id = p_treatment_ranking_id
  LIMIT 1;
$$ LANGUAGE sql STABLE;

-- loop-91: PurityTrapFlag (introduced loop-87) had no formula in the rulebook and the
-- transpiler emitted a stub NULL::boolean. TRUE when SignalPurity >= 0.90 AND the
-- study's confounder identity has PolicyDefault IN ('do-not-condition',
-- 'do-not-naively-adjust') — a 3-hop join (TreatmentRankings -> StratumVariables ->
-- StratumVariableIdentityMaps -> ConfounderIdentities) the formula language cannot
-- express as a single LOOKUP chain. Live query, no cache.
CREATE OR REPLACE FUNCTION calc_treatment_rankings_purity_trap_flag(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT
    calc_treatment_rankings_signal_purity(p_treatment_ranking_id) >= 0.90
    AND EXISTS (
      SELECT 1
      FROM treatment_rankings tr
      JOIN stratum_variables sv ON sv.study = tr.study
      JOIN stratum_variable_identity_maps m ON m.stratum_variable = sv.stratum_variable_id
      JOIN confounder_identities ci ON ci.confounder_identity_id = m.confounder_identity
      WHERE tr.treatment_ranking_id = p_treatment_ranking_id
        AND ci.policy_default IN ('do-not-condition', 'do-not-naively-adjust')
    );
$$ LANGUAGE sql STABLE;

-- loop-92: ColliderSelectionCount / ColliderSelectionManifestCount / ColliderSelectionLatentOnlyCount.
-- The native transpiler output for these three (02-create-functions.sql) has two bugs:
-- (1) known COUNTIFS-drops-2nd-criterion bug (documented project-wide) -- ManifestCount's
-- native body is identical to Count's, ignoring IsSignFlip entirely; (2) none of the three
-- exclude IsSynthetic studies, so loop-89's deliberately-constructed collider-purity-boundary-flip
-- (an adversarial stress test, not a real-data claim) counts as a "manifest" collider flip and
-- falsely breaks inv-collider-no-manifest's real-data theorem (see loop-92 InvariantChecks note).
-- Live queries, no cache.
CREATE OR REPLACE FUNCTION calc_model_summary_collider_selection_count(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  JOIN studies s ON s.study_id = tr.study
  WHERE calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) IN ('collider', 'selection')
    AND COALESCE(s.is_synthetic, false) = false;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_collider_selection_manifest_count(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  JOIN studies s ON s.study_id = tr.study
  WHERE calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) IN ('collider', 'selection')
    AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
    AND COALESCE(s.is_synthetic, false) = false;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_collider_selection_latent_only_count(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  JOIN studies s ON s.study_id = tr.study
  WHERE calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) IN ('collider', 'selection')
    AND calc_treatment_rankings_is_latent_only_flip(tr.treatment_ranking_id) = TRUE
    AND COALESCE(s.is_synthetic, false) = false;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_false_positive_explained_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT (
    (SELECT COUNT(*) FROM treatment_rankings tr
     WHERE calc_treatment_rankings_is_paradox_explained(tr.treatment_ranking_id) = TRUE
       AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = FALSE)
    +
    (SELECT COUNT(*) FROM treatment_rankings tr
     WHERE calc_treatment_rankings_is_paradox_explained(tr.treatment_ranking_id) = TRUE
       AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
       AND calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) <> 'confounder')
  )::integer;
$$ LANGUAGE sql STABLE;

-- ============================================================================
-- Loop-81: ConfounderIdentity cluster witness infrastructure.
-- IdentityClusterSummaries / IdentityDomainCells are first-class rulebook
-- tables (principled materialization per CLAUDE.md — an N-way join over a
-- sealed set of stratum-variable identity maps, refreshed as an explicit
-- container-level action, not a silent bespoke cache). Kept as-is.
-- ============================================================================
CREATE OR REPLACE FUNCTION refresh_identity_cluster_summaries()
RETURNS void
LANGUAGE plpgsql AS $$
BEGIN
  WITH study_identity AS (
    SELECT
      m.confounder_identity,
      s.study_id,
      s.domain,
      s.is_synthetic,
      tr.is_sign_flip,
      tr.latent_flip_potential,
      tr.distortion_type,
      tr.allocation_distortion
    FROM stratum_variable_identity_maps m
    JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
    JOIN studies s ON s.study_id = sv.study
    LEFT JOIN vw_treatment_rankings tr ON tr.study = s.study_id
    WHERE COALESCE(s.is_synthetic, false) = false
  ),
  agg AS (
    SELECT
      confounder_identity,
      COUNT(*)::integer AS study_count,
      COUNT(*) FILTER (WHERE is_sign_flip)::integer AS manifest_flip_count,
      COUNT(*) FILTER (WHERE latent_flip_potential AND NOT is_sign_flip)::integer AS latent_flip_count,
      COUNT(*) FILTER (WHERE distortion_type = 'D')::integer AS type_d_count,
      AVG(allocation_distortion) AS avg_allocation_distortion,
      COUNT(*) FILTER (WHERE distortion_type = 'D' AND latent_flip_potential)::numeric
        / NULLIF(COUNT(*) FILTER (WHERE distortion_type = 'D'), 0) AS latent_fraction_among_type_d,
      COUNT(DISTINCT domain)::integer AS domain_count
    FROM study_identity
    GROUP BY confounder_identity
  )
  UPDATE identity_cluster_summaries ics SET
    study_count = COALESCE(a.study_count, 0),
    manifest_flip_count = COALESCE(a.manifest_flip_count, 0),
    latent_flip_count = COALESCE(a.latent_flip_count, 0),
    type_d_count = COALESCE(a.type_d_count, 0),
    avg_allocation_distortion = a.avg_allocation_distortion,
    latent_fraction_among_type_d = a.latent_fraction_among_type_d,
    domain_count = COALESCE(a.domain_count, 0)
  FROM agg a
  WHERE ics.confounder_identity = a.confounder_identity;

  UPDATE identity_cluster_summaries ics SET
    study_count = 0,
    manifest_flip_count = 0,
    latent_flip_count = 0,
    type_d_count = 0,
    avg_allocation_distortion = NULL,
    latent_fraction_among_type_d = NULL,
    domain_count = 0
  WHERE ics.study_count IS NULL
     OR NOT EXISTS (
       SELECT 1
       FROM stratum_variable_identity_maps m
       JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
       JOIN studies s ON s.study_id = sv.study
       WHERE m.confounder_identity = ics.confounder_identity
         AND COALESCE(s.is_synthetic, false) = false
     );
END;
$$;

CREATE OR REPLACE FUNCTION calc_model_summary_identity_map_coverage_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT (
    SELECT COUNT(*)::numeric
    FROM stratum_variable_identity_maps m
    JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
    JOIN studies s ON s.study_id = sv.study
    WHERE COALESCE(s.is_synthetic, false) = false
  ) / NULLIF((
    SELECT COUNT(*)::numeric
    FROM stratum_variables sv
    JOIN studies s ON s.study_id = sv.study
    WHERE COALESCE(s.is_synthetic, false) = false
  ), 0);
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_age_identity_manifest_flip_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_identity_cluster_summaries_manifest_flip_rate('cluster-id-age-composition');
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_age_identity_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT study_count FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-age-composition';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_age_identity_latent_fraction_among_type_d(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT latent_fraction_among_type_d FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-age-composition';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_severity_identity_latent_fraction_among_type(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT latent_fraction_among_type_d FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-disease-severity';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_identity_cluster_witness_note(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT CONCAT(
    'ageFlip=', calc_model_summary_age_identity_manifest_flip_rate(p_model_summary_id),
    ' ageN=', calc_model_summary_age_identity_study_count(p_model_summary_id),
    ' coverage=', calc_model_summary_identity_map_coverage_rate(p_model_summary_id),
    ' severityLatentD=', calc_model_summary_severity_identity_latent_fraction_among_type(p_model_summary_id),
    ' ageLatentD=', calc_model_summary_age_identity_latent_fraction_among_type_d(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_stratum_variables_confounder_identity(p_stratum_variable_id TEXT)
RETURNS TEXT AS $$
  SELECT m.confounder_identity
  FROM stratum_variable_identity_maps m
  WHERE m.stratum_variable = p_stratum_variable_id
  LIMIT 1;
$$ LANGUAGE sql STABLE;

-- Loop-82: refresh_identity_domain_cells()
-- Populates IdentityDomainCells counts from live joins, same pattern as
-- refresh_identity_cluster_summaries() — principled materialization, kept.
CREATE OR REPLACE FUNCTION refresh_identity_domain_cells()
RETURNS void
LANGUAGE plpgsql AS $$
BEGIN
  WITH study_identity AS (
    SELECT
      m.confounder_identity,
      s.study_id,
      s.domain,
      s.is_synthetic,
      tr.is_sign_flip,
      tr.latent_flip_potential,
      tr.distortion_type
    FROM stratum_variable_identity_maps m
    JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
    JOIN studies s ON s.study_id = sv.study
    LEFT JOIN vw_treatment_rankings tr ON tr.study = s.study_id
    WHERE COALESCE(s.is_synthetic, false) = false
  ),
  agg AS (
    SELECT
      confounder_identity,
      domain,
      COUNT(*)::integer AS study_count,
      COUNT(*) FILTER (WHERE is_sign_flip)::integer AS manifest_flip_count,
      COUNT(*) FILTER (WHERE latent_flip_potential AND NOT is_sign_flip)::integer AS latent_flip_count,
      COUNT(*) FILTER (WHERE distortion_type = 'D')::integer AS type_d_count,
      COUNT(*) FILTER (WHERE distortion_type = 'D' AND latent_flip_potential)::numeric
        / NULLIF(COUNT(*) FILTER (WHERE distortion_type = 'D'), 0) AS latent_fraction_among_type_d
    FROM study_identity
    GROUP BY confounder_identity, domain
  )
  UPDATE identity_domain_cells idc SET
    study_count             = COALESCE(a.study_count, 0),
    manifest_flip_count     = COALESCE(a.manifest_flip_count, 0),
    latent_flip_count       = COALESCE(a.latent_flip_count, 0),
    type_d_count            = COALESCE(a.type_d_count, 0),
    latent_fraction_among_type_d = a.latent_fraction_among_type_d
  FROM agg a
  WHERE idc.confounder_identity = a.confounder_identity
    AND idc.domain = a.domain;

  -- Zero out any seed cells with no matching studies
  UPDATE identity_domain_cells idc SET
    study_count = 0,
    manifest_flip_count = 0,
    latent_flip_count = 0,
    type_d_count = 0,
    latent_fraction_among_type_d = NULL
  WHERE idc.study_count IS NULL;
END;
$$;

-- Fix: ManifestFlipRate for IdentityDomainCells and IdentityClusterSummaries.
-- The transpiler emits ''::numeric for the "" empty-string branch, which fails.
-- Override to return NULL when study_count = 0.
CREATE OR REPLACE FUNCTION calc_identity_domain_cells_manifest_flip_rate(p_cell_id TEXT)
RETURNS NUMERIC AS $$
  SELECT CASE
    WHEN COALESCE((SELECT study_count FROM identity_domain_cells WHERE cell_id = p_cell_id), 0) = 0
    THEN NULL
    ELSE (SELECT manifest_flip_count FROM identity_domain_cells WHERE cell_id = p_cell_id)::NUMERIC
       / (SELECT study_count FROM identity_domain_cells WHERE cell_id = p_cell_id)::NUMERIC
  END;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_identity_cluster_summaries_manifest_flip_rate(p_identity_cluster_id TEXT)
RETURNS NUMERIC AS $$
  SELECT CASE
    WHEN COALESCE((SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = p_identity_cluster_id), 0) = 0
    THEN NULL
    ELSE (SELECT manifest_flip_count FROM identity_cluster_summaries WHERE identity_cluster_id = p_identity_cluster_id)::NUMERIC
       / (SELECT study_count FROM identity_cluster_summaries WHERE identity_cluster_id = p_identity_cluster_id)::NUMERIC
  END;
$$ LANGUAGE sql STABLE;

-- Loop-82/84: ModelSummary LOOKUP overrides for IdentityDomainCells and IdentityClusterSummaries.
-- The transpiler emits NULL stubs for LOOKUP against tables added in the same loop.
CREATE OR REPLACE FUNCTION calc_model_summary_severity_medicine_manifest_flip_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_identity_domain_cells_manifest_flip_rate('cell-id-disease-severity-x-medicine');
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_severity_epi_manifest_flip_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT manifest_flip_count FROM identity_domain_cells
  WHERE cell_id = 'cell-id-disease-severity-x-epidemiology';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_identity_domain_cell_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::INTEGER FROM identity_domain_cells;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_selection_frailty_manifest_flip_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT manifest_flip_count FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-selection-frailty';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_selection_frailty_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT study_count FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-selection-frailty';
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_collider_identity_manifest_flip_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_identity_cluster_summaries_manifest_flip_rate('cluster-id-collider-proxy');
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_model_summary_geographic_type_d_fraction(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT CASE WHEN COALESCE(study_count, 0) = 0 THEN NULL
    ELSE type_d_count::NUMERIC / study_count::NUMERIC
  END
  FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-geographic-composition';
$$ LANGUAGE sql STABLE;

-- ============================================================================
-- Loop-93: SweepStudySummary principled materialization (see MaterializedEntities
-- table, mat-sweep-study-summary). AllocationSweep/SweepStudyConfig are sealed
-- per build; every calc_allocation_sweep_* function is cheap in isolation
-- (~4.5ms) but was being fanned out with zero memoization: every ModelSummary
-- aggregate or DiscoveryFindings row touching PooledGapCrossesZero/
-- SweepPooledGapRange re-triggered a 242-study x 10-row-per-study nested
-- function-call scan from scratch (measured 22.5s for calc_model_summary_latent_type_d_count
-- alone). This refresh function computes each AllocationSweep row's derived
-- values ONCE via a set-based query, aggregates MAX/MIN/AND per study, and
-- writes the result into sweep_study_summary's now-raw columns — collapsing
-- every downstream reader to a single indexed row lookup.
-- ============================================================================
CREATE OR REPLACE FUNCTION refresh_sweep_study_summary()
RETURNS void
LANGUAGE plpgsql AS $$
BEGIN
  WITH sweep_values AS (
    SELECT
      s.study_id,
      calc_allocation_sweep_sweep_corrected_gap(s.sweep_id) AS corrected_gap,
      calc_allocation_sweep_sweep_pooled_gap(s.sweep_id) AS pooled_gap
    FROM allocation_sweep s
  ),
  agg AS (
    SELECT
      study_id,
      MIN(corrected_gap) AS corrected_gap_constant,
      MAX(corrected_gap) AS sweep_corrected_gap_max,
      MIN(corrected_gap) AS sweep_corrected_gap_min,
      MAX(pooled_gap) AS sweep_pooled_gap_max,
      MIN(pooled_gap) AS sweep_pooled_gap_min
    FROM sweep_values
    GROUP BY study_id
  )
  UPDATE sweep_study_summary sss SET
    corrected_gap_constant = a.corrected_gap_constant,
    sweep_corrected_gap_max = a.sweep_corrected_gap_max,
    sweep_corrected_gap_min = a.sweep_corrected_gap_min,
    sweep_corrected_gap_range = a.sweep_corrected_gap_max - a.sweep_corrected_gap_min,
    sweep_pooled_gap_max = a.sweep_pooled_gap_max,
    sweep_pooled_gap_min = a.sweep_pooled_gap_min,
    sweep_pooled_gap_range = a.sweep_pooled_gap_max - a.sweep_pooled_gap_min,
    pooled_gap_crosses_zero = (a.sweep_pooled_gap_min < 0 AND a.sweep_pooled_gap_max > 0)
  FROM agg a
  WHERE sss.sweep_study_id = a.study_id;
END;
$$;

-- Repoint the expensive live-calculated fields at the materialized columns.
-- Same signatures as the (now-orphaned) 02-create-functions.sql definitions,
-- so every existing caller keeps working unchanged.
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_corrected_gap_constant(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT corrected_gap_constant FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_corrected_gap_max(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_corrected_gap_max FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_corrected_gap_min(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_corrected_gap_min FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_corrected_gap_range(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_corrected_gap_range FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_pooled_gap_max(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_max FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_pooled_gap_min(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_min FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_pooled_gap_range(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_range FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_sweep_study_summary_pooled_gap_crosses_zero(p_sweep_study_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT pooled_gap_crosses_zero FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;
