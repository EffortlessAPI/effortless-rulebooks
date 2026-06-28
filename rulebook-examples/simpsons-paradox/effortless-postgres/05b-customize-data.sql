-- All study data (synthetic and real) now lives in the rulebook JSON SSoT.
-- It is generated into 05-insert-data.sql by the transpiler on every build.

-- ----------------------------------------------------------------------------
-- InvariantChecks: populate PassCount and FailCount from live view queries.
-- Each UPDATE runs the exact assertion against vw_treatment_rankings and
-- writes the witness counts back into invariant_checks. Any fail_count > 0
-- after init-db.sh is a build-breaking bug in the DAG.
-- ----------------------------------------------------------------------------

-- inv-taxonomy-complete: every row has a non-null, non-empty DistortionType
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type IS NOT NULL AND distortion_type <> ''),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type IS NULL OR distortion_type = '')
WHERE invariant_check_id = 'inv-taxonomy-complete';

-- inv-type-ab-sign-flip: DistortionType IN (A,B) → IsSignFlip=TRUE
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type IN ('A','B') AND is_sign_flip = TRUE),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type IN ('A','B') AND (is_sign_flip IS NULL OR is_sign_flip = FALSE))
WHERE invariant_check_id = 'inv-type-ab-sign-flip';

-- inv-type-cd-no-sign-flip: DistortionType IN (C,D) → IsSignFlip=FALSE
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type IN ('C','D') AND (is_sign_flip = FALSE OR is_sign_flip IS NULL)),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type IN ('C','D') AND is_sign_flip = TRUE)
WHERE invariant_check_id = 'inv-type-cd-no-sign-flip';

-- inv-sign-flip-winner: IsSignFlip=TRUE → CorrectedVsPooledAgreement=FALSE
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE is_sign_flip = TRUE AND corrected_vs_pooled_agreement = FALSE),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE is_sign_flip = TRUE AND (corrected_vs_pooled_agreement IS NULL OR corrected_vs_pooled_agreement = TRUE))
WHERE invariant_check_id = 'inv-sign-flip-winner';

-- inv-type-d-agreement: DistortionType=D → CorrectedVsPooledAgreement=TRUE
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type = 'D' AND corrected_vs_pooled_agreement = TRUE),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type = 'D' AND (corrected_vs_pooled_agreement IS NULL OR corrected_vs_pooled_agreement = FALSE))
WHERE invariant_check_id = 'inv-type-d-agreement';

-- inv-type-d-low-distortion: DistortionType=D → AllocationDistortion < 0.01
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type = 'D' AND allocation_distortion < 0.01),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings WHERE distortion_type = 'D' AND (allocation_distortion IS NULL OR allocation_distortion >= 0.01))
WHERE invariant_check_id = 'inv-type-d-low-distortion';

-- inv-corrected-gap-sign: SIGN(CorrectedGap) = SIGN(WeightedStratumGapSum)
-- CorrectedGap IS WeightedStratumGapSum by definition, so this catches any
-- formula mis-wiring where the two columns diverge.
UPDATE invariant_checks SET
  pass_count = (SELECT COUNT(*) FROM vw_treatment_rankings
                WHERE weighted_stratum_gap_sum IS NOT NULL
                  AND SIGN(corrected_gap) = SIGN(weighted_stratum_gap_sum)),
  fail_count = (SELECT COUNT(*) FROM vw_treatment_rankings
                WHERE weighted_stratum_gap_sum IS NOT NULL
                  AND SIGN(corrected_gap) <> SIGN(weighted_stratum_gap_sum))
WHERE invariant_check_id = 'inv-corrected-gap-sign';
