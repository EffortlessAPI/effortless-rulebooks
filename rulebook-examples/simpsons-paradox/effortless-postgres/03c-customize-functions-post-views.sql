-- ============================================================================
-- CUSTOMIZE FUNCTIONS (POST-VIEWS) - functions that reference views
-- ============================================================================
-- This file runs AFTER 03b-customize-views.sql. It exists ONLY because
-- Postgres SQL-language (LANGUAGE sql) function bodies are validated against
-- real relations at CREATE time — unlike plpgsql, which is dynamically
-- checked. The functions below reference vw_treatment_rankings, which is
-- not created until 03b-customize-views.sql, so they cannot live in
-- 02b-customize-functions.sql (which runs before any view exists).
--
-- Moved from 02b-customize-functions.sql: this ordering bug was previously
-- invisible because the local DB was always incrementally patched rather
-- than dropped and recreated from nothing in a single init-db.sh pass.
-- ============================================================================

DROP FUNCTION IF EXISTS calc_discovery_findings_is_confirmed(TEXT) CASCADE;
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
    -- loop-92: case-mix/geography/situational archetypes should drift toward
    -- Type-D (rising or single-decade), age/institutional/SES should not (not rising).
    WHEN 'H-confounder-drift-type-d' THEN
      calc_model_summary_severity_drift_direction('simpsons-paradox-v1') IN ('rising', 'single-decade')
      AND calc_model_summary_geographic_drift_direction('simpsons-paradox-v1') IN ('rising', 'single-decade')
      AND calc_model_summary_situational_drift_direction('simpsons-paradox-v1') IN ('rising', 'single-decade')
      AND calc_model_summary_age_drift_direction('simpsons-paradox-v1') <> 'rising'
      AND calc_model_summary_institutional_drift_direction('simpsons-paradox-v1') <> 'rising'
      AND calc_model_summary_socioeconomic_drift_direction('simpsons-paradox-v1') <> 'rising'
    -- loop-89: all four named synthetic boundary studies must exist by StudyId.
    WHEN 'H-synthetic-edge-coverage' THEN
      (SELECT COUNT(*) FROM studies WHERE is_synthetic = TRUE) >= 4
      AND EXISTS (SELECT 1 FROM studies WHERE study_id = 'near-zero-distortion-boundary')
      AND EXISTS (SELECT 1 FROM studies WHERE study_id = 'equal-weight-signflip-boundary')
      AND EXISTS (SELECT 1 FROM studies WHERE study_id = 'zero-corrected-gap-boundary')
      AND EXISTS (SELECT 1 FROM studies WHERE study_id = 'collider-purity-boundary-flip')
    ELSE FALSE
  END;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_discovery_findings_observed_metric(TEXT) CASCADE;
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
    WHEN 'H-confounder-drift-type-d' THEN CONCAT(
      'severity=', calc_model_summary_severity_drift_direction('simpsons-paradox-v1'),
      ' geography=', calc_model_summary_geographic_drift_direction('simpsons-paradox-v1'),
      ' situational=', calc_model_summary_situational_drift_direction('simpsons-paradox-v1'),
      ' age=', calc_model_summary_age_drift_direction('simpsons-paradox-v1'),
      ' institutional=', calc_model_summary_institutional_drift_direction('simpsons-paradox-v1'),
      ' socioeconomic=', calc_model_summary_socioeconomic_drift_direction('simpsons-paradox-v1'),
      ' — expected severity/geography/situational in (rising,single-decade) and age/institutional/socioeconomic <> rising; institutional and socioeconomic both drifted rising, contradicting the hypothesis.')
    WHEN 'H-synthetic-edge-coverage' THEN CONCAT(
      'SyntheticStudyCount=', (SELECT COUNT(*) FROM studies WHERE is_synthetic = TRUE),
      ' near-zero-distortion=', (SELECT EXISTS (SELECT 1 FROM studies WHERE study_id = 'near-zero-distortion-boundary')),
      ' equal-weight-sign-flip=', (SELECT EXISTS (SELECT 1 FROM studies WHERE study_id = 'equal-weight-signflip-boundary')),
      ' zero-corrected-gap=', (SELECT EXISTS (SELECT 1 FROM studies WHERE study_id = 'zero-corrected-gap-boundary')),
      ' collider-purity=', (SELECT EXISTS (SELECT 1 FROM studies WHERE study_id = 'collider-purity-boundary-flip')))
    ELSE ''
  END;
$$ LANGUAGE sql STABLE;

DROP FUNCTION IF EXISTS calc_discovery_findings_evidence(TEXT) CASCADE;
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
-- Re-create the views that CASCADE-dropped when the three functions above
-- were redefined. Both are otherwise defined upstream (vw_discovery_findings
-- natively in 03-create-views.sql; vw_conclusions overridden in
-- 03b-customize-views.sql) — kept in sync with those definitions.
-- ----------------------------------------------------------------------------
DROP VIEW IF EXISTS vw_discovery_findings CASCADE;
CREATE VIEW vw_discovery_findings WITH (security_invoker = ON) AS
SELECT
  t.finding_id,
  calc_discovery_findings_name(t.finding_id) AS name,
  t.hypothesis_id,
  calc_discovery_findings_hypothesis_statement(t.finding_id) AS hypothesis_statement,
  calc_discovery_findings_observed_metric(t.finding_id) AS observed_metric,
  calc_discovery_findings_is_confirmed(t.finding_id) AS is_confirmed,
  calc_discovery_findings_evidence(t.finding_id) AS evidence,
  t.witnessed_in_loop
FROM discovery_findings t;

DROP VIEW IF EXISTS vw_conclusions CASCADE;
CREATE VIEW vw_conclusions WITH (security_invoker = ON) AS
SELECT
  native.*,
  (SELECT is_confirmed FROM vw_discovery_findings WHERE hypothesis_id = native.validating_hypothesis) AS live_status_confirmed,
  (SELECT observed_metric FROM vw_discovery_findings WHERE hypothesis_id = native.validating_hypothesis) AS live_status_observed_metric
FROM (
  SELECT
    t.conclusion_id,
    calc_conclusions_name(t.conclusion_id) AS name,
    t.category,
    t.status,
    t.report_tier,
    t.validating_hypothesis,
    t.title,
    t.evidence,
    t.witnessed_in_loop,
    t.target_loop,
    t.tradition_id,
    t.researcher_id,
    t.challenges_researcher,
    calc_conclusions_invariant_protecting_count(t.conclusion_id) AS invariant_protecting_count
  FROM conclusions t
) native;
