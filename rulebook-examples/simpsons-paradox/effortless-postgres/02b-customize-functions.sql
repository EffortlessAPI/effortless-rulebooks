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
DROP FUNCTION IF EXISTS calc_studies_ingestion_cell_parity(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_studies_ingestion_cell_parity(p_study_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (
    (SELECT calc_treatment_rankings_stratum_count(tr.treatment_ranking_id) FROM treatment_rankings tr WHERE tr.study = p_study_id LIMIT 1) >= 2
    AND calc_studies_cell_count(p_study_id) =
        (SELECT calc_treatment_rankings_stratum_count(tr.treatment_ranking_id) FROM treatment_rankings tr WHERE tr.study = p_study_id LIMIT 1) * 2
  );
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_studies_ingestion_compliance(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS calc_allocation_sweep_is_original(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_is_original(p_sweep_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT ABS(s.alloc_fraction_a - c.original_alloc_fraction_a) < 0.03
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_n_sweep_stratum_total(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_n_sweep_stratum_total(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.n_sweep_stratum_total
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_n_fixed_a(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_n_fixed_a(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.n_fixed_a
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_n_fixed_b(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_n_fixed_b(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.n_fixed_b
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_sweep_rate_a(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_sweep_rate_a(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.sweep_rate_a
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_sweep_rate_b(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_sweep_rate_b(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.sweep_rate_b
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_fixed_rate_a(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_fixed_rate_a(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.fixed_rate_a
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_fixed_rate_b(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_fixed_rate_b(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.fixed_rate_b
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_allocation_sweep_sweep_corrected_gap(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_allocation_sweep_sweep_corrected_gap(p_sweep_id TEXT)
RETURNS NUMERIC AS $$
  SELECT c.sweep_corrected_gap
  FROM allocation_sweep s
  JOIN sweep_study_config c ON c.study_id = s.study_id
  WHERE s.sweep_id = p_sweep_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_distortion_type_label(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_distortion_type_label(p_sweep_study_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_treatment_rankings_distortion_type(tr.treatment_ranking_id)
  FROM treatment_rankings tr WHERE tr.study = p_sweep_study_id LIMIT 1;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_stratum_label(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_stratum_label(p_sweep_study_id TEXT)
RETURNS TEXT AS $$
  SELECT c.sweep_stratum_label
  FROM sweep_study_config c
  WHERE c.study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- run_invariant_checks()
-- Reads every row in invariant_checks and evaluates SqlAssertion (with optional
-- SqlFilter) against the view named by SourceTable, then writes pass_count and
-- fail_count back into the table. Routes by source_table.
-- ----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS run_invariant_checks() CASCADE;
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
DROP FUNCTION IF EXISTS lookup_candidate_study_catalog_observed_distortion_type(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id TEXT) RETURNS TEXT AS $$
  SELECT calc_treatment_rankings_distortion_type(m.treatment_ranking_id)
  FROM candidate_study_catalog c
  JOIN treatment_rankings m ON m.study = c.linked_study_id
  WHERE c.candidate_id = p_candidate_id
    AND c.ingestion_status = 'imported';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_candidate_study_catalog_observed_distortion_type(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_observed_distortion_type(p_candidate_id TEXT) RETURNS TEXT AS $$
  SELECT lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id);
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_candidate_study_catalog_expected_sign_flip(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_expected_sign_flip(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT expected_distortion_type IN ('A', 'B')
  FROM candidate_study_catalog
  WHERE candidate_id = p_candidate_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_candidate_study_catalog_observed_sign_flip_type(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_observed_sign_flip_type(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE
    WHEN NOT calc_candidate_study_catalog_is_imported(p_candidate_id) THEN NULL
    ELSE lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id) IN ('A', 'B')
  END;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_candidate_study_catalog_type_prediction_match(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_type_prediction_match(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE
    WHEN NOT calc_candidate_study_catalog_is_imported(p_candidate_id) THEN NULL
    ELSE lookup_candidate_study_catalog_observed_distortion_type(p_candidate_id)
         = (SELECT expected_distortion_type FROM candidate_study_catalog WHERE candidate_id = p_candidate_id)
  END;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_candidate_study_catalog_sign_flip_prediction_match(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_candidate_study_catalog_sign_flip_prediction_match(p_candidate_id TEXT) RETURNS BOOLEAN AS $$
  SELECT CASE
    WHEN NOT calc_candidate_study_catalog_is_imported(p_candidate_id) THEN NULL
    WHEN NOT calc_candidate_study_catalog_expected_sign_flip(p_candidate_id) THEN NULL
    ELSE calc_candidate_study_catalog_observed_sign_flip_type(p_candidate_id)
  END;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_type_prediction_match_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_type_prediction_match_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_type_prediction_match(c.candidate_id) = TRUE;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_type_prediction_mismatch_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_type_prediction_mismatch_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_is_imported(c.candidate_id) = TRUE
    AND calc_candidate_study_catalog_type_prediction_match(c.candidate_id) = FALSE;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_is_imported(c.candidate_id) = TRUE
    AND calc_candidate_study_catalog_expected_sign_flip(c.candidate_id) = TRUE;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_sign_flip_prediction_match_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_sign_flip_prediction_match_count(p_catalog_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM candidate_study_catalog c
  WHERE calc_candidate_study_catalog_sign_flip_prediction_match(c.candidate_id) = TRUE;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_type_prediction_match_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_type_prediction_match_rate(p_catalog_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_match_count(p_catalog_summary_id)::numeric
         / NULLIF(calc_corpus_catalog_summary_imported_count(p_catalog_summary_id), 0);
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_sign_flip_prediction_match_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_corpus_catalog_summary_sign_flip_prediction_match_rate(p_catalog_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_sign_flip_prediction_match_count(p_catalog_summary_id)::numeric
         / NULLIF(calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(p_catalog_summary_id), 0);
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_corpus_catalog_summary_catalog_prediction_witness_note(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS calc_model_summary_type_prediction_match_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_prediction_match_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_match_count('catalog-v1');
$$ LANGUAGE sql STABLE;
DROP FUNCTION IF EXISTS calc_model_summary_type_prediction_mismatch_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_prediction_mismatch_count(p_model_summary_id TEXT) RETURNS INTEGER AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_mismatch_count('catalog-v1');
$$ LANGUAGE sql STABLE;
DROP FUNCTION IF EXISTS calc_model_summary_type_prediction_match_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_type_prediction_match_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_type_prediction_match_rate('catalog-v1');
$$ LANGUAGE sql STABLE;
DROP FUNCTION IF EXISTS calc_model_summary_sign_flip_prediction_match_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_sign_flip_prediction_match_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_corpus_catalog_summary_sign_flip_prediction_match_rate('catalog-v1');
$$ LANGUAGE sql STABLE;
DROP FUNCTION IF EXISTS calc_model_summary_catalog_prediction_witness_note(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_catalog_prediction_witness_note(p_model_summary_id TEXT) RETURNS TEXT AS $$
  SELECT calc_corpus_catalog_summary_catalog_prediction_witness_note('catalog-v1');
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- loop-49: IngestionSummary/ModelSummary LOOKUP overrides (transpiler cannot
-- emit LOOKUP for a table added in the same loop). Live queries, no cache.
-- ----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS calc_model_summary_corpus_passes_ingestion_contract(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_corpus_passes_ingestion_contract(p_model_summary_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT calc_ingestion_summary_ingestion_contract_passes('ingestion-v1');
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_ingestion_witness_note(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS calc_model_summary_high_imbalance_sign_flip_threshold(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT percentile_cont(0.5) WITHIN GROUP (ORDER BY calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id))
  FROM treatment_rankings tr
  WHERE calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) IS NOT NULL;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_economics_high_imbalance_sign_flip_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_economics_high_imbalance_sign_flip_count(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  WHERE calc_treatment_rankings_study_domain(tr.treatment_ranking_id) = 'economics'
    AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
    AND calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) >= calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id);
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS _domain_high_imbalance_sign_flip_rate(TEXT, NUMERIC) CASCADE;
CREATE OR REPLACE FUNCTION _domain_high_imbalance_sign_flip_rate(p_domain TEXT, p_threshold NUMERIC) RETURNS NUMERIC AS $$
  SELECT COUNT(*) FILTER (WHERE calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE)::numeric
         / NULLIF(COUNT(*), 0)
  FROM treatment_rankings tr
  WHERE calc_treatment_rankings_study_domain(tr.treatment_ranking_id) = p_domain
    AND calc_treatment_rankings_max_stratum_imbalance(tr.treatment_ranking_id) >= p_threshold;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_epidemiology_high_imbalance_sign_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_epidemiology_high_imbalance_sign_flip_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT _domain_high_imbalance_sign_flip_rate(
    'epidemiology',
    calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_legal_high_imbalance_sign_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_legal_high_imbalance_sign_flip_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT _domain_high_imbalance_sign_flip_rate(
    'legal',
    calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_sports_high_imbalance_sign_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_sports_high_imbalance_sign_flip_rate(p_model_summary_id TEXT) RETURNS NUMERIC AS $$
  SELECT _domain_high_imbalance_sign_flip_rate(
    'sports',
    calc_model_summary_high_imbalance_sign_flip_threshold(p_model_summary_id)
  );
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- loop-78 ModelSummary rollup (expansion wave 3 supersession audit)
-- CorpusPatternSupersededFailCount / ExpansionWave3DiscoveryNote are now real
-- native calculated fields on ModelSummary (loop-90) — see
-- 02-create-functions.sql. Nothing left to override here.
-- ----------------------------------------------------------------------------

-- StratumCausalRole: LOOKUP(Study, StratumVariables[Study], StratumVariables[CausalRole])
-- (transpiler quota gap for tables introduced in an earlier loop than this
-- lookup field). Needed below by calc_model_summary_false_positive_explained_count
-- and by vw_treatment_rankings's presentation column in 03b.
DROP FUNCTION IF EXISTS calc_treatment_rankings_stratum_causal_role(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS calc_treatment_rankings_purity_trap_flag(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS calc_model_summary_collider_selection_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_collider_selection_count(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  JOIN studies s ON s.study_id = tr.study
  WHERE calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) IN ('collider', 'selection')
    AND COALESCE(s.is_synthetic, false) = false;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_collider_selection_manifest_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_collider_selection_manifest_count(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  JOIN studies s ON s.study_id = tr.study
  WHERE calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) IN ('collider', 'selection')
    AND calc_treatment_rankings_is_sign_flip(tr.treatment_ranking_id) = TRUE
    AND COALESCE(s.is_synthetic, false) = false;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_collider_selection_latent_only_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_collider_selection_latent_only_count(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COUNT(*)::numeric
  FROM treatment_rankings tr
  JOIN studies s ON s.study_id = tr.study
  WHERE calc_treatment_rankings_stratum_causal_role(tr.treatment_ranking_id) IN ('collider', 'selection')
    AND calc_treatment_rankings_is_latent_only_flip(tr.treatment_ranking_id) = TRUE
    AND COALESCE(s.is_synthetic, false) = false;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_false_positive_explained_count(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS refresh_identity_cluster_summaries() CASCADE;
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

DROP FUNCTION IF EXISTS calc_model_summary_identity_map_coverage_rate(TEXT) CASCADE;
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

DROP FUNCTION IF EXISTS calc_model_summary_age_identity_manifest_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_age_identity_manifest_flip_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_identity_cluster_summaries_manifest_flip_rate('cluster-id-age-composition');
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_age_identity_study_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_age_identity_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT study_count FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-age-composition';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_age_identity_latent_fraction_among_type_d(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_age_identity_latent_fraction_among_type_d(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT latent_fraction_among_type_d FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-age-composition';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_severity_identity_latent_fraction_among_type(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_severity_identity_latent_fraction_among_type(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT latent_fraction_among_type_d FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-disease-severity';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_identity_cluster_witness_note(TEXT) CASCADE;
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

DROP FUNCTION IF EXISTS calc_stratum_variables_confounder_identity(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS refresh_identity_domain_cells() CASCADE;
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
DROP FUNCTION IF EXISTS calc_identity_domain_cells_manifest_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_identity_domain_cells_manifest_flip_rate(p_cell_id TEXT)
RETURNS NUMERIC AS $$
  SELECT CASE
    WHEN COALESCE((SELECT study_count FROM identity_domain_cells WHERE cell_id = p_cell_id), 0) = 0
    THEN NULL
    ELSE (SELECT manifest_flip_count FROM identity_domain_cells WHERE cell_id = p_cell_id)::NUMERIC
       / (SELECT study_count FROM identity_domain_cells WHERE cell_id = p_cell_id)::NUMERIC
  END;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_identity_cluster_summaries_manifest_flip_rate(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS calc_model_summary_severity_medicine_manifest_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_severity_medicine_manifest_flip_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_identity_domain_cells_manifest_flip_rate('cell-id-disease-severity-x-medicine');
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_severity_epi_manifest_flip_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_severity_epi_manifest_flip_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT manifest_flip_count FROM identity_domain_cells
  WHERE cell_id = 'cell-id-disease-severity-x-epidemiology';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_identity_domain_cell_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_identity_domain_cell_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::INTEGER FROM identity_domain_cells;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_selection_frailty_manifest_flip_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_selection_frailty_manifest_flip_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT manifest_flip_count FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-selection-frailty';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_selection_frailty_study_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_selection_frailty_study_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT study_count FROM identity_cluster_summaries
  WHERE identity_cluster_id = 'cluster-id-selection-frailty';
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_collider_identity_manifest_flip_rate(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_collider_identity_manifest_flip_rate(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_identity_cluster_summaries_manifest_flip_rate('cluster-id-collider-proxy');
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_geographic_type_d_fraction(TEXT) CASCADE;
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
DROP FUNCTION IF EXISTS refresh_sweep_study_summary() CASCADE;
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
DROP FUNCTION IF EXISTS calc_sweep_study_summary_corrected_gap_constant(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_corrected_gap_constant(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT corrected_gap_constant FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_corrected_gap_max(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_corrected_gap_max(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_corrected_gap_max FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_corrected_gap_min(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_corrected_gap_min(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_corrected_gap_min FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_corrected_gap_range(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_corrected_gap_range(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_corrected_gap_range FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;
-- ----------------------------------------------------------------------------
-- Theorem wave ModelSummary fixes (loops 73-76)
-- Transpiler emits broken MAX/COUNTIFS for the sweep-corrected-gap-range
-- rollup and the explained-false-positive sum; these are genuine
-- cross-table aggregations the native transpiler cannot express.
-- ----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS calc_model_summary_max_study_sweep_corrected_gap_range(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_max_study_sweep_corrected_gap_range(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT COALESCE(MAX(calc_sweep_study_summary_sweep_corrected_gap_range(sweep_study_id)), 0)::numeric
  FROM sweep_study_summary;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_corrected_gap_invariant_fail_count(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_corrected_gap_invariant_fail_count(p_model_summary_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM sweep_study_summary
  WHERE calc_sweep_study_summary_sweep_corrected_gap_range(sweep_study_id) >= 0.0001;
$$ LANGUAGE sql STABLE;


DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_pooled_gap_max(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_pooled_gap_max(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_max FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_pooled_gap_min(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_pooled_gap_min(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_min FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_sweep_pooled_gap_range(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_sweep_pooled_gap_range(p_sweep_study_id TEXT)
RETURNS NUMERIC AS $$
  SELECT sweep_pooled_gap_range FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_sweep_study_summary_pooled_gap_crosses_zero(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_sweep_study_summary_pooled_gap_crosses_zero(p_sweep_study_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT pooled_gap_crosses_zero FROM sweep_study_summary WHERE sweep_study_id = p_sweep_study_id;
$$ LANGUAGE sql STABLE;
-- TreatmentRankings.PooledGapCrossesZero / SweepPooledGapRange: both are
-- LOOKUP(Study, SweepStudySummary[Study], SweepStudySummary[...]) formulas
-- the transpiler cannot emit ("Function 'LOOKUP' is not supported yet").
-- The underlying calc_sweep_study_summary_* functions ARE native and
-- correct; this just re-exposes them keyed by TreatmentRankingId instead of
-- Study. LatentFlipPotential/IsSweepFragile/AllocationFragility all
-- transitively depend on this LOOKUP, so it must be live, not a cache read.
DROP FUNCTION IF EXISTS calc_treatment_rankings_pooled_gap_crosses_zero(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_pooled_gap_crosses_zero(p_treatment_ranking_id TEXT) RETURNS BOOLEAN AS $$
  SELECT calc_sweep_study_summary_pooled_gap_crosses_zero(tr.study)
  FROM treatment_rankings tr WHERE tr.treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_treatment_rankings_sweep_pooled_gap_range(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_treatment_rankings_sweep_pooled_gap_range(p_treatment_ranking_id TEXT) RETURNS NUMERIC AS $$
  SELECT calc_sweep_study_summary_sweep_pooled_gap_range(tr.study)
  FROM treatment_rankings tr WHERE tr.treatment_ranking_id = p_treatment_ranking_id;
$$ LANGUAGE sql STABLE;


-- ModelSummary.AvgSignalPurity: =SUM(TreatmentRankings!{{SignalPurity}}) / COUNT(TreatmentRankings!{{TreatmentRankingId}})
-- Transpiler bug: emitted SUM(calc_treatment_rankings_signal_purity(p_treatment_ranking_id)) reusing the
-- outer scalar param instead of aggregating a per-row subquery over treatment_rankings, breaking
-- vw_model_summary for every consumer (both PDF exports). Override aggregates correctly.
DROP FUNCTION IF EXISTS calc_model_summary_avg_signal_purity(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_avg_signal_purity(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT (
    (SELECT COALESCE(SUM(calc_treatment_rankings_signal_purity(treatment_ranking_id)::numeric), 0) FROM treatment_rankings)
    / NULLIF((SELECT COUNT(*) FROM treatment_rankings), 0)
  )::numeric;
$$ LANGUAGE sql STABLE;

-- ConfounderDistortionTimeline.IdentityEarliestTypeDFraction / IdentityLatestTypeDFraction:
-- both are self-referential LOOKUP(CONCAT(ConfounderIdentity,"-x-",IdentityEarliest/LatestDecade), ...[CellId], ...[TypeDFraction]).
-- Transpiler bug: callers reference the 63-char-truncated names
-- (calc_confounder_distortion_timeline_identity_earliest_type_d_fr /
--  calc_confounder_distortion_timeline_identity_latest_type_d_frac) but no CREATE FUNCTION
-- was ever emitted for them — broke vw_confounder_distortion_timeline and DriftDirection.
DROP FUNCTION IF EXISTS calc_confounder_distortion_timeline_identity_earliest_type_d_fr(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_confounder_distortion_timeline_identity_earliest_type_d_fr(p_cell_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_confounder_distortion_timeline_type_d_fraction(other.cell_id)
  FROM confounder_distortion_timeline this
  JOIN confounder_distortion_timeline other
    ON other.cell_id = CONCAT(this.confounder_identity, '-x-', calc_confounder_distortion_timeline_identity_earliest_decade(this.cell_id))
  WHERE this.cell_id = p_cell_id;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_confounder_distortion_timeline_identity_latest_type_d_frac(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_confounder_distortion_timeline_identity_latest_type_d_frac(p_cell_id TEXT)
RETURNS NUMERIC AS $$
  SELECT calc_confounder_distortion_timeline_type_d_fraction(other.cell_id)
  FROM confounder_distortion_timeline this
  JOIN confounder_distortion_timeline other
    ON other.cell_id = CONCAT(this.confounder_identity, '-x-', calc_confounder_distortion_timeline_identity_latest_decade(this.cell_id))
  WHERE this.cell_id = p_cell_id;
$$ LANGUAGE sql STABLE;

-- ModelSummary.SweepPooledGapRange: =MAXIFS(AllocationSweep!{{SweepPooledGap}},...) - MINIFS(...)
-- Transpiler bug: the MAX/MIN-difference dedup CTE reused calc_allocation_sweep_sweep_pooled_gap(sweep_id)
-- with no FROM binding sweep_id (SweepCorrectedGapMax/Min, the single-aggregate siblings, transpile
-- correctly — only this combined MAX-minus-MIN case broke). Override aggregates per-row correctly.
DROP FUNCTION IF EXISTS calc_model_summary_sweep_pooled_gap_range(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_sweep_pooled_gap_range(p_model_summary_id TEXT)
RETURNS NUMERIC AS $$
  SELECT (
    COALESCE(MAX(calc_allocation_sweep_sweep_pooled_gap(sweep_id)::numeric), 0)
    - COALESCE(MIN(calc_allocation_sweep_sweep_pooled_gap(sweep_id)::numeric), 0)
  )::numeric
  FROM allocation_sweep WHERE study_id = 'kidney-1986';
$$ LANGUAGE sql STABLE;

-- Loop-92: ConfounderDistortionTimeline refresh. Same principled-materialization
-- pattern as refresh_identity_cluster_summaries()/refresh_identity_domain_cells()
-- (N-way join over a sealed, bounded study set) — populates the (ConfounderIdentity
-- x ValidTimeDecade) cells seeded in the rulebook from live StratumVariableIdentityMaps
-- x Studies x TreatmentRankings joins, bucketed by calc_studies_valid_time_decade().
CREATE OR REPLACE FUNCTION refresh_confounder_distortion_timeline()
RETURNS void
LANGUAGE plpgsql AS $$
BEGIN
  WITH study_identity_decade AS (
    SELECT
      m.confounder_identity,
      calc_studies_valid_time_decade(s.study_id) AS valid_time_decade,
      tr.distortion_type,
      tr.is_sign_flip,
      tr.signal_purity,
      tr.allocation_distortion
    FROM stratum_variable_identity_maps m
    JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
    JOIN studies s ON s.study_id = sv.study
    LEFT JOIN vw_treatment_rankings tr ON tr.study = s.study_id
    WHERE calc_studies_valid_time_decade(s.study_id) IS NOT NULL
  ),
  agg AS (
    SELECT
      confounder_identity,
      valid_time_decade,
      COUNT(*)::integer AS study_count,
      COUNT(*) FILTER (WHERE distortion_type = 'D')::integer AS type_d_count,
      COUNT(*) FILTER (WHERE is_sign_flip)::integer AS sign_flip_count,
      SUM(signal_purity) AS sum_signal_purity,
      SUM(allocation_distortion) AS sum_allocation_distortion
    FROM study_identity_decade
    GROUP BY confounder_identity, valid_time_decade
  )
  UPDATE confounder_distortion_timeline cdt SET
    study_count = COALESCE(a.study_count, 0),
    type_d_count = COALESCE(a.type_d_count, 0),
    sign_flip_count = COALESCE(a.sign_flip_count, 0),
    sum_signal_purity = a.sum_signal_purity,
    sum_allocation_distortion = a.sum_allocation_distortion
  FROM agg a
  WHERE cdt.confounder_identity = a.confounder_identity
    AND cdt.valid_time_decade = a.valid_time_decade;

  UPDATE confounder_distortion_timeline cdt SET
    study_count = 0,
    type_d_count = 0,
    sign_flip_count = 0,
    sum_signal_purity = NULL,
    sum_allocation_distortion = NULL
  WHERE NOT EXISTS (
    SELECT 1
    FROM stratum_variable_identity_maps m
    JOIN stratum_variables sv ON sv.stratum_variable_id = m.stratum_variable
    JOIN studies s ON s.study_id = sv.study
    WHERE m.confounder_identity = cdt.confounder_identity
      AND calc_studies_valid_time_decade(s.study_id) = cdt.valid_time_decade
  );
END;
$$;

-- ModelSummary.{Severity,Geographic,Situational,Age,Institutional,Socioeconomic}DriftDirection:
-- rulebook formula is LOOKUP(constant, ConfounderDistortionTimeline[ConfounderIdentity], ...[DriftDirection])
-- (matching on ConfounderIdentity, a non-PK column). Transpiler bug: native output
-- treats the constant as if it were CellId (the table's actual PK), emitting
-- calc_confounder_distortion_timeline_drift_direction('id-disease-severity') — a
-- CellId that never exists, since real CellIds are '<identity>-x-<decade>'. Every
-- native call silently returns NULL. DriftDirection is constant per ConfounderIdentity
-- across all of its decade rows by construction (see ConfounderDistortionTimeline's
-- DriftDirection formula), so any one matching row is correct; ORDER BY for determinism.
DROP FUNCTION IF EXISTS calc_model_summary_severity_drift_direction(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_severity_drift_direction(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_confounder_distortion_timeline_drift_direction(cell_id) FROM confounder_distortion_timeline
  WHERE confounder_identity = 'id-disease-severity'
  ORDER BY valid_time_decade LIMIT 1;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_geographic_drift_direction(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_geographic_drift_direction(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_confounder_distortion_timeline_drift_direction(cell_id) FROM confounder_distortion_timeline
  WHERE confounder_identity = 'id-geographic-composition'
  ORDER BY valid_time_decade LIMIT 1;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_situational_drift_direction(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_situational_drift_direction(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_confounder_distortion_timeline_drift_direction(cell_id) FROM confounder_distortion_timeline
  WHERE confounder_identity = 'id-situational-context'
  ORDER BY valid_time_decade LIMIT 1;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_age_drift_direction(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_age_drift_direction(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_confounder_distortion_timeline_drift_direction(cell_id) FROM confounder_distortion_timeline
  WHERE confounder_identity = 'id-age-composition'
  ORDER BY valid_time_decade LIMIT 1;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_institutional_drift_direction(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_institutional_drift_direction(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_confounder_distortion_timeline_drift_direction(cell_id) FROM confounder_distortion_timeline
  WHERE confounder_identity = 'id-institutional-unit'
  ORDER BY valid_time_decade LIMIT 1;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_model_summary_socioeconomic_drift_direction(TEXT) CASCADE;
CREATE OR REPLACE FUNCTION calc_model_summary_socioeconomic_drift_direction(p_model_summary_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_confounder_distortion_timeline_drift_direction(cell_id) FROM confounder_distortion_timeline
  WHERE confounder_identity = 'id-socioeconomic-status'
  ORDER BY valid_time_decade LIMIT 1;
$$ LANGUAGE sql STABLE;
