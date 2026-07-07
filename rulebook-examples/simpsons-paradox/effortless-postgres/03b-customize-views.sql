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
-- vw_treatment_rankings — add loop-36/38 fields + DistortionRatio (presentation layer pruned loop-60)
--   distortion_ratio = signed_pooled_gap / weighted_stratum_gap_sum
--   screening_tier     = DANGER / CAUTION / SAFE (from DistortionType)
--   arm_size_ratio, symmetry_departure, max_stratum_imbalance / max_stratum_gap
-- MUST come before vw_model_summary (which references this view)
-- ============================================================================
DROP VIEW IF EXISTS vw_treatment_rankings CASCADE;
CREATE VIEW vw_treatment_rankings WITH (security_invoker = ON) AS
SELECT
  t.treatment_ranking_id,
  m.name,
  t.study,
  t.treatment_a,
  t.treatment_b,
  m.total_cases_a,
  m.total_successes_a,
  m.pooled_rate_a,
  m.total_cases_b,
  m.total_successes_b,
  m.pooled_rate_b,
  m.pooled_winner,
  m.stratum_count,
  m.strata_won_by_a,
  m.strata_won_by_b,
  m.per_stratum_winner,
  m.is_reversal,
  m.confounders_in_study,
  m.is_paradox_explained,
  m.pooled_gap,
  m.strata_won_by_loser,
  m.paradox_strength,
  m.pooled_rate_from_weights_a,
  m.pooled_rate_from_weights_b,
  m.reversal_intensity,
  m.threshold_margin,
  m.signed_pooled_gap,
  m.weighted_stratum_gap_sum,
  m.is_sign_flip,
  m.allocation_distortion,
  CASE
    WHEN m.weighted_stratum_gap_sum = 0 THEN NULL
    ELSE m.signed_pooled_gap / m.weighted_stratum_gap_sum
  END AS distortion_ratio,
  m.distortion_type,
  CASE
    WHEN m.distortion_type IN ('A','B') THEN 'DANGER'
    WHEN m.distortion_type LIKE 'C%'   THEN 'CAUTION'
    WHEN m.distortion_type = 'D'       THEN 'SAFE'
    ELSE NULL
  END AS screening_tier,
  m.policy_implication,
  m.corrected_gap,
  m.corrected_winner,
  m.corrected_vs_pooled_agreement,
  m.corrected_policy_implication,
  m.confirmed_causal_role_count,
  m.mediator_risk_count,
  m.contested_stratum_count,
  m.unknown_causal_role_count,
  m.causal_claim_status,
  m.adjustment_appropriate,
  t.adjustment_rationale,
  CASE
    WHEN m.total_cases_a + m.total_cases_b = 0 THEN NULL
    ELSE m.total_cases_a::numeric / (m.total_cases_a + m.total_cases_b)
  END AS arm_size_ratio,
  CASE
    WHEN m.total_cases_a + m.total_cases_b = 0 THEN NULL
    ELSE ABS(m.total_cases_a::numeric / (m.total_cases_a + m.total_cases_b) - 0.5)
  END AS symmetry_departure,
  m.max_stratum_imbalance,
  m.max_stratum_gap,
  m.allocation_direction,
  m.signal_purity,
  m.pooled_gap_crosses_zero,
  m.sweep_pooled_gap_range,
  m.latent_flip_potential,
  m.allocation_fragility,
  m.study_domain,
  m.stratum_causal_role,
  m.is_latent_only_flip,
  m.is_stratum_unanimous,
  m.is_sweep_fragile
FROM treatment_rankings t
JOIN _erb_tr_metrics m ON m.treatment_ranking_id = t.treatment_ranking_id;

-- ============================================================================
-- vw_model_summary — extend with ratio statistics and tier counts (loop-41)
-- MUST come after vw_treatment_rankings (references it via subqueries)
-- ============================================================================
DROP VIEW IF EXISTS vw_model_summary CASCADE;
CREATE VIEW vw_model_summary WITH (security_invoker = ON) AS
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
  calc_model_summary_sweep_corrected_gap_max(t.model_summary_id) AS sweep_corrected_gap_max,
  calc_model_summary_sweep_corrected_gap_min(t.model_summary_id) AS sweep_corrected_gap_min,
  calc_model_summary_sweep_corrected_gap_range(t.model_summary_id) AS sweep_corrected_gap_range,
  calc_model_summary_sweep_pooled_gap_range(t.model_summary_id) AS sweep_pooled_gap_range,
  -- Ratio statistics per type (loop-41) — from metrics cache
  (SELECT MIN(signed_pooled_gap / NULLIF(weighted_stratum_gap_sum, 0)) FROM _erb_tr_metrics WHERE distortion_type = 'A') AS min_ratio_type_a,
  (SELECT MAX(signed_pooled_gap / NULLIF(weighted_stratum_gap_sum, 0)) FROM _erb_tr_metrics WHERE distortion_type = 'A') AS max_ratio_type_a,
  (SELECT MIN(signed_pooled_gap / NULLIF(weighted_stratum_gap_sum, 0)) FROM _erb_tr_metrics WHERE distortion_type = 'B') AS min_ratio_type_b,
  (SELECT MAX(signed_pooled_gap / NULLIF(weighted_stratum_gap_sum, 0)) FROM _erb_tr_metrics WHERE distortion_type = 'B') AS max_ratio_type_b,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type IN ('A','B')) AS danger_tier_count,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type LIKE 'C%') AS caution_tier_count,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type = 'D') AS safe_tier_count,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type = 'C+') AS type_c_plus_count,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type = 'C-') AS type_c_minus_count,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type = 'C+') AS c_amplification_count,
  (SELECT COUNT(*) FROM _erb_tr_metrics WHERE distortion_type = 'C-') AS c_compression_count,
  (SELECT AVG(signal_purity) FROM _erb_tr_metrics) AS avg_signal_purity,
  -- Cross-domain and SignalPurity theorem fields (loops 47-48)
  calc_model_summary_real_study_count(t.model_summary_id)                    AS real_study_count,
  calc_model_summary_medicine_study_count(t.model_summary_id)                AS medicine_study_count,
  calc_model_summary_epidemiology_study_count(t.model_summary_id)            AS epidemiology_study_count,
  calc_model_summary_other_domain_study_count(t.model_summary_id)            AS other_domain_study_count,
  calc_model_summary_domain_diversity_note(t.model_summary_id)               AS domain_diversity_note,
  calc_model_summary_avg_signal_purity_reversal(t.model_summary_id)          AS avg_signal_purity_reversal,
  calc_model_summary_avg_signal_purity_non_reversal(t.model_summary_id)      AS avg_signal_purity_non_reversal,
  calc_model_summary_signal_purity_gap(t.model_summary_id)                   AS signal_purity_gap,
  (SELECT phase_point_count FROM vw_phase_diagram_summary LIMIT 1)           AS synthetic_phase_count,
  (SELECT all_five_types_present FROM vw_phase_diagram_summary LIMIT 1)      AS phase_diagram_complete,
  (SELECT phase_taxonomy_witness FROM vw_phase_diagram_summary LIMIT 1)      AS phase_taxonomy_coverage,
  calc_model_summary_ingestion_protocol_item_count(t.model_summary_id)       AS ingestion_protocol_item_count,
  calc_model_summary_corpus_passes_ingestion_contract(t.model_summary_id)      AS corpus_passes_ingestion_contract,
  calc_model_summary_ingestion_witness_note(t.model_summary_id)              AS ingestion_witness_note,
  calc_model_summary_latent_type_d_count(t.model_summary_id)                 AS latent_type_d_count,
  calc_model_summary_stable_type_d_count(t.model_summary_id)                 AS stable_type_d_count,
  calc_model_summary_latent_type_d_fraction(t.model_summary_id)              AS latent_type_d_fraction,
  calc_model_summary_cross_zero_count(t.model_summary_id)                    AS cross_zero_count,
  calc_model_summary_sign_flip_signal_purity_max(t.model_summary_id)         AS sign_flip_signal_purity_max,
  calc_model_summary_economics_sign_flip_count(t.model_summary_id)           AS economics_sign_flip_count,
  calc_model_summary_avg_pooled_gap_latent_d(t.model_summary_id)             AS avg_pooled_gap_latent_d,
  calc_model_summary_avg_pooled_gap_stable_d(t.model_summary_id)             AS avg_pooled_gap_stable_d,
  calc_model_summary_epidemiology_avg_distortion(t.model_summary_id)         AS epidemiology_avg_distortion,
  calc_model_summary_education_avg_distortion(t.model_summary_id)            AS education_avg_distortion,
  calc_model_summary_confounder_sign_flip_count(t.model_summary_id)          AS confounder_sign_flip_count,
  calc_model_summary_confounder_latent_only_count(t.model_summary_id)        AS confounder_latent_only_count,
  calc_model_summary_collider_selection_count(t.model_summary_id)            AS collider_selection_count,
  calc_model_summary_collider_selection_manifest_count(t.model_summary_id)   AS collider_selection_manifest_count,
  calc_model_summary_collider_selection_latent_only_count(t.model_summary_id) AS collider_selection_latent_only_count,
  calc_model_summary_discovery_witness_note(t.model_summary_id)                AS discovery_witness_note,
  calc_model_summary_type_prediction_match_count(t.model_summary_id)            AS type_prediction_match_count,
  calc_model_summary_type_prediction_mismatch_count(t.model_summary_id)          AS type_prediction_mismatch_count,
  calc_model_summary_type_prediction_match_rate(t.model_summary_id)              AS type_prediction_match_rate,
  calc_model_summary_sign_flip_prediction_match_rate(t.model_summary_id)         AS sign_flip_prediction_match_rate,
  calc_model_summary_catalog_prediction_witness_note(t.model_summary_id)         AS catalog_prediction_witness_note,
  calc_model_summary_high_imbalance_sign_flip_threshold(t.model_summary_id)      AS high_imbalance_sign_flip_threshold,
  calc_model_summary_economics_high_imbalance_sign_flip_count(t.model_summary_id) AS economics_high_imbalance_sign_flip_count,
  calc_model_summary_epidemiology_high_imbalance_sign_flip_rate(t.model_summary_id) AS epidemiology_high_imbalance_sign_flip_rate,
  calc_model_summary_legal_high_imbalance_sign_flip_rate(t.model_summary_id)      AS legal_high_imbalance_sign_flip_rate,
  calc_model_summary_sports_high_imbalance_sign_flip_rate(t.model_summary_id)    AS sports_high_imbalance_sign_flip_rate,
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
  calc_model_summary_expansion_wave2_study_count(t.model_summary_id) AS expansion_wave2_study_count
FROM model_summary t;

-- ============================================================================
-- vw_studies — expose PublicationYear, Domain, IsSynthetic (loop-47)
-- MUST come AFTER vw_treatment_rankings (joined below); otherwise the DROP
-- CASCADE on vw_treatment_rankings above would kill this view mid-build.
-- ============================================================================
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
  calc_studies_ingestion_compliance(s.study_id)     AS ingestion_compliance
FROM studies s
LEFT JOIN research_traditions   rt  ON rt.tradition_id   = s.tradition_id
LEFT JOIN researchers           res ON res.researcher_id = s.primary_researcher_id
LEFT JOIN vw_treatment_rankings tr  ON tr.study          = s.study_id
LEFT JOIN vw_stratum_variables  sv  ON sv.study          = s.study_id;

-- ============================================================================
-- vw_synthetic_phase — read from metrics cache (240 grid points)
-- ============================================================================
DROP VIEW IF EXISTS vw_synthetic_phase CASCADE;
CREATE VIEW vw_synthetic_phase WITH (security_invoker = ON) AS
SELECT
  sp.phase_id,
  m.name,
  sp.param_stratum_fraction,
  sp.param_stratum_gap1,
  sp.param_stratum_gap2,
  sp.param_allocation_bias,
  m.phase_s1_total,
  m.phase_na1,
  m.phase_nb1,
  m.phase_na2,
  m.phase_nb2,
  m.phase_rate_a1,
  m.phase_rate_b1,
  m.phase_rate_a2,
  m.phase_rate_b2,
  m.phase_pooled_rate_a,
  m.phase_pooled_rate_b,
  m.phase_signed_pooled_gap,
  m.phase_corrected_gap,
  m.phase_strata_won_by_loser,
  m.phase_reversal_intensity,
  m.phase_is_sign_flip,
  m.phase_allocation_distortion,
  m.phase_distortion_type
FROM synthetic_phase sp
JOIN _erb_sp_metrics m ON m.phase_id = sp.phase_id;

-- ============================================================================
-- vw_conclusions — join loop commit provenance for discovery replay (loop-62)
-- ============================================================================
DROP VIEW IF EXISTS vw_conclusions CASCADE;
CREATE VIEW vw_conclusions WITH (security_invoker = ON) AS
SELECT
  t.conclusion_id,
  calc_conclusions_name(t.conclusion_id) AS name,
  t.category,
  t.status,
  t.title,
  t.evidence,
  t.witnessed_in_loop,
  t.target_loop,
  t.tradition_id,
  t.researcher_id,
  t.challenges_researcher,
  calc_conclusions_invariant_protecting_count(t.conclusion_id) AS invariant_protecting_count,
  l.commit_hash AS witnessed_in_loop_commit_hash,
  calc_loops_commit_short(l.loop_id) AS witnessed_in_loop_commit_short,
  l.commit_date AS witnessed_in_loop_commit_date,
  l.git_tag AS witnessed_in_loop_git_tag
FROM conclusions t
LEFT JOIN loops l ON l.loop_id = t.witnessed_in_loop;

-- ============================================================================
-- vw_candidate_study_catalog — loop-65 catalog prediction audit fields
-- ============================================================================
DROP VIEW IF EXISTS vw_candidate_study_catalog CASCADE;
CREATE VIEW vw_candidate_study_catalog WITH (security_invoker = ON) AS
SELECT
  t.candidate_id,
  calc_candidate_study_catalog_name(t.candidate_id) AS name,
  t.proposed_study_id,
  t.title,
  t.citation,
  t.source_url,
  t.domain,
  t.stratum_variable_name,
  t.expected_distortion_type,
  t.ingestion_status,
  t.priority,
  t.stratum_count_estimate,
  t.data_source_note,
  t.linked_study_id,
  t.publication_year,
  calc_candidate_study_catalog_is_ready_to_encode(t.candidate_id) AS is_ready_to_encode,
  calc_candidate_study_catalog_is_imported(t.candidate_id) AS is_imported,
  lookup_candidate_study_catalog_observed_distortion_type(t.candidate_id) AS observed_distortion_type,
  calc_candidate_study_catalog_type_prediction_match(t.candidate_id) AS type_prediction_match,
  calc_candidate_study_catalog_expected_sign_flip(t.candidate_id) AS expected_sign_flip,
  calc_candidate_study_catalog_observed_sign_flip_type(t.candidate_id) AS observed_sign_flip_type,
  calc_candidate_study_catalog_sign_flip_prediction_match(t.candidate_id) AS sign_flip_prediction_match,
  t.data_acquisition_status,
  t.reversal_mechanism,
  t.paradox_confirmation,
  t.expansion_wave,
  calc_candidate_study_catalog_is_data_ready(t.candidate_id) AS is_data_ready
FROM candidate_study_catalog t;

-- ============================================================================
-- vw_corpus_catalog_summary — loop-65 prediction aggregates
-- ============================================================================
DROP VIEW IF EXISTS vw_corpus_catalog_summary CASCADE;
CREATE VIEW vw_corpus_catalog_summary WITH (security_invoker = ON) AS
SELECT
  t.catalog_summary_id,
  calc_corpus_catalog_summary_name(t.catalog_summary_id) AS name,
  calc_corpus_catalog_summary_total_catalog_entries(t.catalog_summary_id) AS total_catalog_entries,
  calc_corpus_catalog_summary_imported_count(t.catalog_summary_id) AS imported_count,
  calc_corpus_catalog_summary_candidate_count(t.catalog_summary_id) AS candidate_count,
  calc_corpus_catalog_summary_blocked_count(t.catalog_summary_id) AS blocked_count,
  calc_corpus_catalog_summary_ready_to_encode_count(t.catalog_summary_id) AS ready_to_encode_count,
  calc_corpus_catalog_summary_high_priority_count(t.catalog_summary_id) AS high_priority_count,
  calc_corpus_catalog_summary_import_session_ready(t.catalog_summary_id) AS import_session_ready,
  calc_corpus_catalog_summary_catalog_witness_note(t.catalog_summary_id) AS catalog_witness_note,
  calc_corpus_catalog_summary_type_prediction_match_count(t.catalog_summary_id) AS type_prediction_match_count,
  calc_corpus_catalog_summary_type_prediction_mismatch_count(t.catalog_summary_id) AS type_prediction_mismatch_count,
  calc_corpus_catalog_summary_sign_flip_prediction_eligible_count(t.catalog_summary_id) AS sign_flip_prediction_eligible_count,
  calc_corpus_catalog_summary_sign_flip_prediction_match_count(t.catalog_summary_id) AS sign_flip_prediction_match_count,
  calc_corpus_catalog_summary_type_prediction_match_rate(t.catalog_summary_id) AS type_prediction_match_rate,
  calc_corpus_catalog_summary_sign_flip_prediction_match_rate(t.catalog_summary_id) AS sign_flip_prediction_match_rate,
  calc_corpus_catalog_summary_catalog_prediction_witness_note(t.catalog_summary_id) AS catalog_prediction_witness_note
FROM corpus_catalog_summary t;

-- Repopulate metrics caches: customize views JOIN _erb_tr_metrics / _erb_sp_metrics.
-- Without this, vw_treatment_rankings and vw_model_summary return empty aggregates
-- if only 03b is re-applied (e.g. hot-fixing a view definition).
SELECT refresh_erb_tr_metrics();
SELECT refresh_erb_sp_metrics();
