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
-- PERFORMANCE OVERRIDES — one-pass rewrites of the hottest DAG functions.
-- ----------------------------------------------------------------------------
-- WHY: the generated calc_ functions are LANGUAGE sql STABLE and re-call their
-- sub-functions multiple times in a single expression (Postgres does NOT
-- memoize STABLE calls within a statement). The generated predicted_value calls
-- individual_causal_mass + individual_confirmed_node_count FOUR times each;
-- patient_stratification_tier then calls predicted_value TWICE — so one tier
-- value re-runs the whole mechanism aggregation ~8x. Over the keystone view's
-- ~30 derived columns this fanned out to ~700ms PER ROW.
--
-- These overrides preserve the EXACT formula/value (verified against a golden
-- baseline; the 303-check harness must stay green) but evaluate each expensive
-- sub-value ONCE via a CTE. Plain views are unchanged. No materialization.
--
-- Override order matters: predicted_value must be redefined before
-- patient_stratification_tier so the tier reuses the cheap version. (Within one
-- statement we still bind it once in a CTE regardless.)
-- ============================================================================

-- predicted_value = LEAST(10, 2*causal_mass + 1.5*confirmed_node_count)
-- (same numeric-guarded formula; sub-values bound once each instead of 4x).
CREATE OR REPLACE FUNCTION calc_individual_predictions_predicted_value(p_individual_prediction_id TEXT)
RETURNS NUMERIC AS $$
  WITH v AS (
    SELECT
      COALESCE(calc_individual_predictions_individual_causal_mass(p_individual_prediction_id), 0)           AS mass,
      COALESCE(calc_individual_predictions_individual_confirmed_node_count(p_individual_prediction_id), 0)  AS nodes
  )
  SELECT LEAST(10::numeric, (2 * v.mass + 1.5 * v.nodes))::numeric
  FROM v;
$$ LANGUAGE sql STABLE;

-- patient_stratification_tier = banding over predicted_value
-- (call predicted_value ONCE, not twice).
CREATE OR REPLACE FUNCTION calc_individual_predictions_patient_stratification_tier(p_individual_prediction_id TEXT)
RETURNS TEXT AS $$
  WITH v AS (
    SELECT (calc_individual_predictions_predicted_value(p_individual_prediction_id))::numeric AS pv
  )
  SELECT (CASE
            WHEN v.pv >= 7 THEN 'High-Risk Pathway'
            WHEN v.pv >= 4 THEN 'Moderate-Risk Pathway'
            ELSE 'Low-Risk Pathway'
          END)::text
  FROM v;
$$ LANGUAGE sql STABLE;

-- is_clinically_actionable = HC ∧ FB ∧ ATS ∧ predicted_value > 0
-- (bind each gate once; same AND-chain).
CREATE OR REPLACE FUNCTION calc_individual_predictions_is_clinically_actionable(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  WITH g AS (
    SELECT
      calc_individual_predictions_is_high_confidence_prediction(p_individual_prediction_id) AS hc,
      calc_individual_predictions_is_falsifiability_backed(p_individual_prediction_id)       AS fb,
      calc_individual_predictions_is_ancestry_transport_safe(p_individual_prediction_id)     AS ats,
      (calc_individual_predictions_predicted_value(p_individual_prediction_id))::numeric     AS pv
  )
  SELECT (g.hc AND g.fb AND g.ats AND g.pv > 0)::boolean
  FROM g;
$$ LANGUAGE sql STABLE;

-- lifecycle_state_key = the deciding-gate ladder (same branches, each gate once).
CREATE OR REPLACE FUNCTION calc_individual_predictions_lifecycle_state_key(p_individual_prediction_id TEXT)
RETURNS TEXT AS $$
  WITH g AS (
    SELECT
      calc_individual_predictions_is_high_confidence_prediction(p_individual_prediction_id)   AS hc,
      calc_individual_predictions_is_falsifiability_backed(p_individual_prediction_id)         AS fb,
      calc_individual_predictions_is_ancestry_transport_safe(p_individual_prediction_id)       AS ats,
      calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id)     AS rests,
      calc_individual_predictions_individual_has_cryptic_relatedness(p_individual_prediction_id) AS cryptic,
      (calc_individual_predictions_calibrated_uncertainty(p_individual_prediction_id))::numeric AS cu,
      (calc_individual_predictions_predicted_value(p_individual_prediction_id))::numeric        AS pv
  )
  SELECT (CASE
            WHEN (g.hc AND g.fb AND g.ats AND g.pv > 0) THEN 'Actionable'
            WHEN (NOT g.rests OR NOT g.fb)              THEN 'NotActionable'
            WHEN g.cryptic                              THEN 'NotActionable'
            WHEN g.cu < 0.7                             THEN 'NotActionable'
            WHEN NOT g.ats                              THEN 'NotActionable'
            ELSE 'Actionable'
          END)::text
  FROM g;
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- MECHANISM-LEVEL: causal_confidence is the deepest hot spot. The generated
-- body pastes the FULL weighted-sum expression inside every CASE/COALESCE
-- branch, so each cheap sub-score (qualified-evidence count, modality count,
-- replication fraction, control survival) gets re-evaluated dozens of times in
-- one call. We bind each sub-score ONCE and compute the same weighted sum:
--   raw = 0.30*min(1, qual/4) + 0.20*min(1, modalities/3)
--       + 0.30*replication_fraction + 0.20*(survives_controls ? 1 : 0)
--   causal_confidence = LEAST(1, raw)
-- Verified value-identical to the generated function for all 7 mechanisms.
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_causal_mechanisms_causal_confidence(p_causal_mechanism_id TEXT)
RETURNS NUMERIC AS $$
  WITH s AS (
    SELECT
      LEAST(1::numeric, COALESCE(calc_causal_mechanisms_count_qualified_evidence(p_causal_mechanism_id), 0)::numeric / 4) AS evidence_score,
      LEAST(1::numeric, COALESCE(calc_causal_mechanisms_count_modalities_supporting(p_causal_mechanism_id), 0)::numeric / 3) AS modality_score,
      COALESCE(calc_causal_mechanisms_replication_fraction(p_causal_mechanism_id), 0)::numeric AS replication_fraction,
      (CASE WHEN calc_causal_mechanisms_survives_negative_controls(p_causal_mechanism_id) THEN 1 ELSE 0 END)::numeric AS control_score
  )
  SELECT LEAST(
           1::numeric,
           0.30 * s.evidence_score + 0.20 * s.modality_score + 0.30 * s.replication_fraction + 0.20 * s.control_score
         )::numeric
  FROM s;
$$ LANGUAGE sql STABLE;

-- is_causal_architecture_node: bind causal_confidence + the cheap predicates once.
-- (variant-candidate OR environmental-exposure clause preserved exactly.)
CREATE OR REPLACE FUNCTION calc_causal_mechanisms_is_causal_architecture_node(p_causal_mechanism_id TEXT)
RETURNS BOOLEAN AS $$
  WITH m AS (
    SELECT
      (calc_causal_mechanisms_causal_confidence(p_causal_mechanism_id))::numeric AS cc,
      calc_causal_mechanisms_is_experimentally_falsifiable(p_causal_mechanism_id) AS falsifiable,
      calc_causal_mechanisms_is_spurious_derived(p_causal_mechanism_id)           AS spurious,
      calc_causal_mechanisms_variant_is_causal_candidate(p_causal_mechanism_id)   AS variant_candidate,
      (SELECT NULLIF(environmental_exposure, '') FROM causal_mechanisms WHERE causal_mechanism_id = p_causal_mechanism_id) AS env_exposure
  )
  SELECT (m.cc >= 0.7 AND m.falsifiable AND NOT m.spurious AND (m.variant_candidate OR m.env_exposure IS NOT NULL))::boolean
  FROM m;
$$ LANGUAGE sql STABLE;

-- is_ancestry_transportable: bind the node verdict + cross-ancestry count once.
CREATE OR REPLACE FUNCTION calc_causal_mechanisms_is_ancestry_transportable(p_causal_mechanism_id TEXT)
RETURNS BOOLEAN AS $$
  WITH m AS (
    SELECT
      calc_causal_mechanisms_is_causal_architecture_node(p_causal_mechanism_id)               AS node,
      (calc_causal_mechanisms_count_cross_ancestry_concordant(p_causal_mechanism_id))::numeric AS cross_anc
  )
  SELECT (m.node AND m.cross_anc >= 1)::boolean
  FROM m;
$$ LANGUAGE sql STABLE;

