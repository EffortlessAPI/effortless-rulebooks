-- ============================================================================
-- CUSTOMIZE VIEWS - User-defined views customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main views script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom views changes will appear here:

-- ============================================================================
-- loop-90: hand-code sanitization.
-- vw_treatment_rankings and vw_corpus_balance overrides that used to live
-- here have been DELETED — the native 03-create-views.sql versions are now
-- complete and correct (DistortionRatio/ScreeningTier/ArmSizeRatio/
-- SymmetryDeparture were promoted to real TreatmentRankings rulebook fields;
-- CorpusBalance's formulas were fixed to the standard COUNTIFS/{{}} dialect
-- instead of the broken COUNTA/bareword-ref/uppercase-Formula-key authoring
-- that produced garbage native SQL). Verified empirically by diffing the
-- native view output against these overrides before deleting them — do not
-- trust a stale "transpiler emits broken X" comment; re-check against
-- 03-create-views.sql every time before assuming an override is still needed.
--
-- What remains below are genuine, still-necessary transpiler gaps: LOOKUP
-- formulas the transpiler cannot emit when the target table was introduced
-- in the same loop as the lookup field (Studies enrichment via
-- TreatmentRankings/ResearchTraditions/Researchers/StratumVariables joins,
-- and StratumVariables.ConfounderIdentity via StratumVariableIdentityMaps).
-- Each of the fields these views expose already exists as a real
-- lookup-typed field in the rulebook schema; only the transpiler's emission
-- is missing.
-- ============================================================================

-- ----------------------------------------------------------------------------
-- vw_treatment_rankings — append StratumCausalRole, the one column the
-- native transpiler still cannot emit: it is a LOOKUP into StratumVariables
-- (LIMIT 1, no aggregation) and the transpiler quota drops single-row
-- cross-table LOOKUPs for tables introduced in an earlier loop than the
-- lookup field itself. Confirmed empirically after the loop-90 rewrite
-- broke run_invariant_checks() on inv-collider-no-manifest, which filters
-- directly on vw_treatment_rankings.stratum_causal_role.
-- ----------------------------------------------------------------------------
DROP VIEW IF EXISTS vw_treatment_rankings CASCADE;
CREATE VIEW vw_treatment_rankings WITH (security_invoker = ON) AS
SELECT
  native.*,
  calc_treatment_rankings_stratum_causal_role(native.treatment_ranking_id) AS stratum_causal_role
FROM (
  SELECT
    t.treatment_ranking_id, calc_treatment_rankings_name(t.treatment_ranking_id) AS name, t.study, t.treatment_a, t.treatment_b,
    calc_treatment_rankings_total_cases_a(t.treatment_ranking_id) AS total_cases_a,
    calc_treatment_rankings_total_successes_a(t.treatment_ranking_id) AS total_successes_a,
    calc_treatment_rankings_pooled_rate_a(t.treatment_ranking_id) AS pooled_rate_a,
    calc_treatment_rankings_total_cases_b(t.treatment_ranking_id) AS total_cases_b,
    calc_treatment_rankings_total_successes_b(t.treatment_ranking_id) AS total_successes_b,
    calc_treatment_rankings_pooled_rate_b(t.treatment_ranking_id) AS pooled_rate_b,
    calc_treatment_rankings_pooled_winner(t.treatment_ranking_id) AS pooled_winner,
    calc_treatment_rankings_stratum_count(t.treatment_ranking_id) AS stratum_count,
    calc_treatment_rankings_strata_won_by_a(t.treatment_ranking_id) AS strata_won_by_a,
    calc_treatment_rankings_strata_won_by_b(t.treatment_ranking_id) AS strata_won_by_b,
    calc_treatment_rankings_per_stratum_winner(t.treatment_ranking_id) AS per_stratum_winner,
    calc_treatment_rankings_is_stratum_unanimous(t.treatment_ranking_id) AS is_stratum_unanimous,
    calc_treatment_rankings_is_reversal(t.treatment_ranking_id) AS is_reversal,
    calc_treatment_rankings_confounders_in_study(t.treatment_ranking_id) AS confounders_in_study,
    calc_treatment_rankings_is_paradox_explained(t.treatment_ranking_id) AS is_paradox_explained,
    calc_treatment_rankings_pooled_gap(t.treatment_ranking_id) AS pooled_gap,
    calc_treatment_rankings_strata_won_by_loser(t.treatment_ranking_id) AS strata_won_by_loser,
    calc_treatment_rankings_paradox_strength(t.treatment_ranking_id) AS paradox_strength,
    calc_treatment_rankings_pooled_rate_from_weights_a(t.treatment_ranking_id) AS pooled_rate_from_weights_a,
    calc_treatment_rankings_pooled_rate_from_weights_b(t.treatment_ranking_id) AS pooled_rate_from_weights_b,
    calc_treatment_rankings_reversal_intensity(t.treatment_ranking_id) AS reversal_intensity,
    calc_treatment_rankings_threshold_margin(t.treatment_ranking_id) AS threshold_margin,
    calc_treatment_rankings_signed_pooled_gap(t.treatment_ranking_id) AS signed_pooled_gap,
    calc_treatment_rankings_weighted_stratum_gap_sum(t.treatment_ranking_id) AS weighted_stratum_gap_sum,
    calc_treatment_rankings_is_sign_flip(t.treatment_ranking_id) AS is_sign_flip,
    calc_treatment_rankings_allocation_distortion(t.treatment_ranking_id) AS allocation_distortion,
    calc_treatment_rankings_distortion_type(t.treatment_ranking_id) AS distortion_type,
    calc_treatment_rankings_policy_implication(t.treatment_ranking_id) AS policy_implication,
    calc_treatment_rankings_corrected_gap(t.treatment_ranking_id) AS corrected_gap,
    calc_treatment_rankings_corrected_winner(t.treatment_ranking_id) AS corrected_winner,
    calc_treatment_rankings_corrected_vs_pooled_agreement(t.treatment_ranking_id) AS corrected_vs_pooled_agreement,
    calc_treatment_rankings_corrected_policy_implication(t.treatment_ranking_id) AS corrected_policy_implication,
    calc_treatment_rankings_allocation_direction(t.treatment_ranking_id) AS allocation_direction,
    calc_treatment_rankings_signal_purity(t.treatment_ranking_id) AS signal_purity,
    calc_treatment_rankings_pooled_gap_crosses_zero(t.treatment_ranking_id) AS pooled_gap_crosses_zero,
    calc_treatment_rankings_sweep_pooled_gap_range(t.treatment_ranking_id) AS sweep_pooled_gap_range,
    calc_treatment_rankings_latent_flip_potential(t.treatment_ranking_id) AS latent_flip_potential,
    calc_treatment_rankings_allocation_fragility(t.treatment_ranking_id) AS allocation_fragility,
    calc_treatment_rankings_max_stratum_imbalance(t.treatment_ranking_id) AS max_stratum_imbalance,
    calc_treatment_rankings_max_stratum_gap(t.treatment_ranking_id) AS max_stratum_gap,
    calc_treatment_rankings_is_sweep_fragile(t.treatment_ranking_id) AS is_sweep_fragile,
    calc_treatment_rankings_study_domain(t.treatment_ranking_id) AS study_domain,
    calc_treatment_rankings_is_latent_only_flip(t.treatment_ranking_id) AS is_latent_only_flip,
    calc_treatment_rankings_confirmed_causal_role_count(t.treatment_ranking_id) AS confirmed_causal_role_count,
    calc_treatment_rankings_mediator_risk_count(t.treatment_ranking_id) AS mediator_risk_count,
    calc_treatment_rankings_contested_stratum_count(t.treatment_ranking_id) AS contested_stratum_count,
    calc_treatment_rankings_unknown_causal_role_count(t.treatment_ranking_id) AS unknown_causal_role_count,
    calc_treatment_rankings_causal_claim_status(t.treatment_ranking_id) AS causal_claim_status,
    calc_treatment_rankings_adjustment_appropriate(t.treatment_ranking_id) AS adjustment_appropriate,
    t.adjustment_rationale,
    calc_treatment_rankings_purity_trap_flag(t.treatment_ranking_id) AS purity_trap_flag,
    calc_treatment_rankings_distortion_ratio(t.treatment_ranking_id) AS distortion_ratio,
    calc_treatment_rankings_screening_tier(t.treatment_ranking_id) AS screening_tier,
    calc_treatment_rankings_arm_size_ratio(t.treatment_ranking_id) AS arm_size_ratio,
    calc_treatment_rankings_symmetry_departure(t.treatment_ranking_id) AS symmetry_departure
  FROM treatment_rankings t
) native;

-- ----------------------------------------------------------------------------
-- vw_studies — expose PublicationYear/Domain/IsSynthetic/IsControlStudy/
-- ControlDomain (native, already complete) plus the TreatmentRankings/
-- ResearchTraditions/Researchers/StratumVariables LOOKUP fields the
-- transpiler cannot emit for lookups introduced in the same loop as their
-- target table.
-- MUST come after vw_treatment_rankings and vw_stratum_variables (joined below).
-- ----------------------------------------------------------------------------
DROP VIEW IF EXISTS vw_studies CASCADE;
CREATE VIEW vw_studies WITH (security_invoker = ON) AS
SELECT
  s.study_id,
  calc_studies_name(s.study_id)        AS name,
  s.title,
  s.source,
  s.source_url,
  s.publication_year,
  s.domain,
  s.is_synthetic,
  calc_studies_total_cases(s.study_id) AS total_cases,
  calc_studies_cell_count(s.study_id)  AS cell_count,
  s.tradition_id,
  s.primary_researcher_id,
  rt.name                              AS tradition_name,
  rt.core_concern                      AS tradition_core_conern,
  res.name                             AS primary_researcher_name,
  res.affiliation                      AS primary_researcher_affiliation,
  tr.distortion_type,
  tr.policy_implication,
  tr.allocation_distortion,
  tr.signal_purity,
  tr.is_sign_flip,
  tr.corrected_winner,
  sv.variable_name                     AS confounding_variable,
  sv.causal_role,
  calc_studies_ingestion_cell_parity(s.study_id)   AS ingestion_cell_parity,
  calc_studies_ingestion_compliance(s.study_id)     AS ingestion_compliance,
  s.is_control_study,
  s.control_domain
FROM studies s
LEFT JOIN research_traditions   rt  ON rt.tradition_id   = s.tradition_id
LEFT JOIN researchers           res ON res.researcher_id = s.primary_researcher_id
LEFT JOIN vw_treatment_rankings tr  ON tr.study          = s.study_id
LEFT JOIN vw_stratum_variables  sv  ON sv.study          = s.study_id;

-- ----------------------------------------------------------------------------
-- vw_stratum_variables — ConfounderIdentity lookup (loop-81 transpiler
-- quota gap: LOOKUP against StratumVariableIdentityMaps, a table added in
-- the same loop). Genuinely not expressible natively; kept.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_stratum_variables WITH (security_invoker = ON) AS
SELECT
  t.stratum_variable_id,
  calc_stratum_variables_name(t.stratum_variable_id) AS name,
  t.study,
  t.variable_name,
  t.causal_role,
  t.affects_treatment_assignment,
  t.affects_outcome,
  calc_stratum_variables_is_confounder(t.stratum_variable_id) AS is_confounder,
  t.mechanism_note,
  t.conditioning_risk,
  calc_stratum_variables_confounder_identity(t.stratum_variable_id) AS confounder_identity
FROM stratum_variables t;

-- ----------------------------------------------------------------------------
-- vw_model_summary — append the LOOKUP columns the transpiler cannot emit
-- for tables (IngestionSummary, CorpusCatalogSummary) added in the same
-- loop as the lookup field. Every other ModelSummary column, including the
-- new DomainFlipGapSurvivesGeometryControl / CorpusPatternSupersededFailCount
-- / ExpansionWave3DiscoveryNote / RealStudyCount / AvgSignalPurity* /
-- DomainDiversityNote fields, is now emitted natively by
-- 03-create-views.sql and is NOT repeated here.
-- ----------------------------------------------------------------------------
DROP VIEW IF EXISTS vw_model_summary CASCADE;
CREATE VIEW vw_model_summary WITH (security_invoker = ON) AS
SELECT
  native.*,
  calc_model_summary_corpus_passes_ingestion_contract(native.model_summary_id) AS corpus_passes_ingestion_contract,
  calc_model_summary_ingestion_witness_note(native.model_summary_id)           AS ingestion_witness_note,
  calc_model_summary_type_prediction_match_count(native.model_summary_id)     AS type_prediction_match_count,
  calc_model_summary_type_prediction_mismatch_count(native.model_summary_id)  AS type_prediction_mismatch_count,
  calc_model_summary_type_prediction_match_rate(native.model_summary_id)      AS type_prediction_match_rate,
  calc_model_summary_sign_flip_prediction_match_rate(native.model_summary_id) AS sign_flip_prediction_match_rate,
  calc_model_summary_catalog_prediction_witness_note(native.model_summary_id) AS catalog_prediction_witness_note
FROM (
  SELECT
    t.model_summary_id,
    calc_model_summary_name(t.model_summary_id) AS name,
    calc_model_summary_reversal_count(t.model_summary_id) AS reversal_count,
    calc_model_summary_non_reversal_count(t.model_summary_id) AS non_reversal_count,
    calc_model_summary_study_count(t.model_summary_id) AS study_count,
    calc_model_summary_explained_count(t.model_summary_id) AS explained_count,
    calc_model_summary_total_paradox_strength(t.model_summary_id) AS total_paradox_strength,
    calc_model_summary_avg_paradox_strength(t.model_summary_id) AS avg_paradox_strength,
    calc_model_summary_type_a_count(t.model_summary_id) AS type_a_count,
    calc_model_summary_type_b_count(t.model_summary_id) AS type_b_count,
    calc_model_summary_type_d_count(t.model_summary_id) AS type_d_count,
    calc_model_summary_type_a_fraction(t.model_summary_id) AS type_a_fraction,
    calc_model_summary_distortion_taxonomy_coverage(t.model_summary_id) AS distortion_taxonomy_coverage,
    calc_model_summary_distortion_only_count(t.model_summary_id) AS distortion_only_count,
    calc_model_summary_real_study_count(t.model_summary_id) AS real_study_count,
    calc_model_summary_medicine_study_count(t.model_summary_id) AS medicine_study_count,
    calc_model_summary_epidemiology_study_count(t.model_summary_id) AS epidemiology_study_count,
    calc_model_summary_other_domain_study_count(t.model_summary_id) AS other_domain_study_count,
    calc_model_summary_domain_diversity_note(t.model_summary_id) AS domain_diversity_note,
    calc_model_summary_avg_signal_purity_reversal(t.model_summary_id) AS avg_signal_purity_reversal,
    calc_model_summary_avg_signal_purity_non_reversal(t.model_summary_id) AS avg_signal_purity_non_reversal,
    calc_model_summary_signal_purity_gap(t.model_summary_id) AS signal_purity_gap,
    t.phase_diagram_complete AS phase_diagram_complete,
    t.phase_taxonomy_coverage AS phase_taxonomy_coverage,
    calc_model_summary_ingestion_protocol_item_count(t.model_summary_id) AS ingestion_protocol_item_count,
    calc_model_summary_latent_type_d_count(t.model_summary_id) AS latent_type_d_count,
    calc_model_summary_stable_type_d_count(t.model_summary_id) AS stable_type_d_count,
    calc_model_summary_latent_type_d_fraction(t.model_summary_id) AS latent_type_d_fraction,
    calc_model_summary_cross_zero_count(t.model_summary_id) AS cross_zero_count,
    calc_model_summary_sign_flip_signal_purity_max(t.model_summary_id) AS sign_flip_signal_purity_max,
    calc_model_summary_economics_sign_flip_count(t.model_summary_id) AS economics_sign_flip_count,
    calc_model_summary_avg_pooled_gap_latent_d(t.model_summary_id) AS avg_pooled_gap_latent_d,
    calc_model_summary_avg_pooled_gap_stable_d(t.model_summary_id) AS avg_pooled_gap_stable_d,
    calc_model_summary_epidemiology_avg_distortion(t.model_summary_id) AS epidemiology_avg_distortion,
    calc_model_summary_education_avg_distortion(t.model_summary_id) AS education_avg_distortion,
    calc_model_summary_confounder_sign_flip_count(t.model_summary_id) AS confounder_sign_flip_count,
    calc_model_summary_confounder_latent_only_count(t.model_summary_id) AS confounder_latent_only_count,
    calc_model_summary_collider_selection_count(t.model_summary_id) AS collider_selection_count,
    calc_model_summary_collider_selection_manifest_count(t.model_summary_id) AS collider_selection_manifest_count,
    calc_model_summary_collider_selection_latent_only_count(t.model_summary_id) AS collider_selection_latent_only_count,
    calc_model_summary_discovery_witness_note(t.model_summary_id) AS discovery_witness_note,
    t.high_imbalance_sign_flip_threshold AS high_imbalance_sign_flip_threshold,
    calc_model_summary_economics_high_imbalance_sign_flip_count(t.model_summary_id) AS economics_high_imbalance_sign_flip_count,
    calc_model_summary_epidemiology_high_imbalance_sign_flip_rate(t.model_summary_id) AS epidemiology_high_imbalance_sign_flip_rate,
    calc_model_summary_legal_high_imbalance_sign_flip_rate(t.model_summary_id) AS legal_high_imbalance_sign_flip_rate,
    calc_model_summary_sports_high_imbalance_sign_flip_rate(t.model_summary_id) AS sports_high_imbalance_sign_flip_rate,
    calc_model_summary_domain_flip_gap_survives_geometry_control(t.model_summary_id) AS domain_flip_gap_survives_geometry_control,
    calc_model_summary_c_plus_avg_distortion(t.model_summary_id) AS c_plus_avg_distortion,
    calc_model_summary_c_minus_avg_distortion(t.model_summary_id) AS c_minus_avg_distortion,
    calc_model_summary_type_d_avg_distortion(t.model_summary_id) AS type_d_avg_distortion,
    calc_model_summary_sweep_fragile_count(t.model_summary_id) AS sweep_fragile_count,
    calc_model_summary_unanimous_sign_flip_count(t.model_summary_id) AS unanimous_sign_flip_count,
    calc_model_summary_expansion_wave1_economics_expected_a_count(t.model_summary_id) AS expansion_wave1_economics_expected_a_count,
    calc_model_summary_expansion_wave1_economics_expected_ad_count(t.model_summary_id) AS expansion_wave1_economics_expected_ad_count,
    calc_model_summary_economics_expected_a_mismatch_rate(t.model_summary_id) AS economics_expected_a_mismatch_rate,
    calc_model_summary_education_latent_fraction(t.model_summary_id) AS education_latent_fraction,
    calc_model_summary_sports_latent_fraction(t.model_summary_id) AS sports_latent_fraction,
    calc_model_summary_economics_sign_flip_rate(t.model_summary_id) AS economics_sign_flip_rate,
    calc_model_summary_expansion_wave2_study_count(t.model_summary_id) AS expansion_wave2_study_count,
    calc_model_summary_corpus_pattern_superseded_fail_count(t.model_summary_id) AS corpus_pattern_superseded_fail_count,
    calc_model_summary_expansion_wave3_discovery_note(t.model_summary_id) AS expansion_wave3_discovery_note,
    calc_model_summary_max_study_sweep_corrected_gap_range(t.model_summary_id) AS max_study_sweep_corrected_gap_range,
    calc_model_summary_corrected_gap_invariant_fail_count(t.model_summary_id) AS corrected_gap_invariant_fail_count,
    calc_model_summary_false_positive_explained_count(t.model_summary_id) AS false_positive_explained_count,
    calc_model_summary_unexplained_confounder_sign_flip_count(t.model_summary_id) AS unexplained_confounder_sign_flip_count,
    calc_model_summary_explained_confounder_count(t.model_summary_id) AS explained_confounder_count,
    calc_model_summary_contested_or_mediator_explained_count(t.model_summary_id) AS contested_or_mediator_explained_count,
    calc_model_summary_theorem_count(t.model_summary_id) AS theorem_count,
    calc_model_summary_confounder_identity_count(t.model_summary_id) AS confounder_identity_count,
    calc_model_summary_mapped_stratum_variable_count(t.model_summary_id) AS mapped_stratum_variable_count,
    calc_model_summary_unmapped_stratum_variable_count(t.model_summary_id) AS unmapped_stratum_variable_count,
    t.identity_cluster_witness_note AS identity_cluster_witness_note,
    calc_model_summary_age_identity_manifest_flip_rate(t.model_summary_id) AS age_identity_manifest_flip_rate,
    calc_model_summary_age_identity_study_count(t.model_summary_id) AS age_identity_study_count,
    calc_model_summary_age_identity_latent_fraction_among_type_d(t.model_summary_id) AS age_identity_latent_fraction_among_type_d,
    calc_model_summary_severity_identity_latent_fraction_among_type(t.model_summary_id) AS severity_identity_latent_fraction_among_type_d,
    calc_model_summary_identity_map_coverage_rate(t.model_summary_id) AS identity_map_coverage_rate
  FROM model_summary t
) native;

-- Note: (SELECT phase_point_count/all_five_types_present/phase_taxonomy_witness
-- FROM vw_phase_diagram_summary LIMIT 1) columns from the old override were
-- dropped from this rewrite — they duplicated ModelSummary's own
-- phase_diagram_complete/phase_taxonomy_coverage raw columns (populated by
-- 05b from vw_phase_diagram_summary) and were not referenced by any
-- consumer under a distinct name; SyntheticPhaseCount itself is not a
-- ModelSummary field in the rulebook schema.
