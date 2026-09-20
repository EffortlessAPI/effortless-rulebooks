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



-- ============================================================================
-- CODEGEN-BUG REPAIR — lookup fields read as base-table columns.
-- ----------------------------------------------------------------------------
-- WHY: when a formula references a LOOKUP field, rulebook-to-postgres emits
--   (SELECT <lookup_col> FROM <base_table> WHERE <pk> = p_<pk>)
-- but a lookup is NOT stored on the base table — it exists only as a computed
-- column on vw_<entity>, backed by calc_<table>_<lookup_col>(). The generated
-- body therefore references a column that does not exist.
--
-- This stays INVISIBLE at build time: a SQL-function body is validated at call
-- time, so the database loads clean and `effortless build` reports green. It
-- only surfaces when something selects the column — here, the whole
-- IndividualPredictions DAG (and with it /api/cohort-individuals, the keystone
-- view, and every diagnosis writeup) was dead with:
--   ERROR: column "individual_has_cryptic_relatedness" does not exist
--
-- FIX: call the lookup's own calc_ function instead of reading the phantom
-- column. Value-identical by construction — that function IS what the view
-- column is computed from. 14 functions across 3 tables:
--   causal_mechanisms     : variant_is_causal_candidate
--   treatments            : is_mechanism_matched
--   individual_predictions: individual_causal_mass, individual_confirmed_node_count,
--                           individual_cross_ancestry_node_count,
--                           individual_has_cryptic_relatedness,
--                           individual_has_high_severity_phenotype,
--                           individual_has_predicted_treatment_response,
--                           individual_max_severity_score
--
-- NOTE: two of these calc_ names exceed Postgres's 63-char identifier limit and
-- are referenced by their TRUNCATED names, which is what pg_proc actually holds.
--
-- This is a transpiler defect, not a rulebook error — the formulas are correct
-- and reference the lookups exactly as the dialect intends. Remove this block
-- once rulebook-to-postgres resolves lookup references properly.
-- ============================================================================
CREATE OR REPLACE FUNCTION calc_treatments_is_treatment_response_predicted(p_treatment_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN (calc_treatments_is_effective_treatment(p_treatment_id) AND COALESCE(calc_treatments_is_mechanism_matched(p_treatment_id), FALSE)) THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_treatments_treatment_response_deciding_factor(p_treatment_id TEXT)
RETURNS TEXT AS $$
  SELECT (CASE WHEN calc_treatments_is_treatment_response_predicted(p_treatment_id) THEN ('EffectiveOnConfirmedMechanism')::text ELSE (CASE WHEN NOT (COALESCE(calc_treatments_is_mechanism_matched(p_treatment_id), FALSE)) THEN ('NoConfirmedMechanism')::text ELSE (CASE WHEN COALESCE((SELECT has_adverse_effect FROM treatments WHERE treatment_id = p_treatment_id), FALSE) THEN ('AdverseEffect')::text ELSE (CASE WHEN ((SELECT NULLIF(treatment_response, '') FROM treatments WHERE treatment_id = p_treatment_id) = 'None' OR (SELECT NULLIF(treatment_response, '') FROM treatments WHERE treatment_id = p_treatment_id) = 'Adverse') THEN ('NoResponse')::text ELSE ('Undetermined')::text END)::text END)::text END)::text END)::text;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_causal_mechanisms_is_causal_architecture_node(p_causal_mechanism_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN ((calc_causal_mechanisms_causal_confidence(p_causal_mechanism_id))::NUMERIC >= 0.7 AND calc_causal_mechanisms_is_experimentally_falsifiable(p_causal_mechanism_id) AND NOT (calc_causal_mechanisms_is_spurious_derived(p_causal_mechanism_id)) AND (COALESCE(calc_causal_mechanisms_variant_is_causal_candidate(p_causal_mechanism_id), FALSE) OR (SELECT NULLIF(environmental_exposure, '') FROM causal_mechanisms WHERE causal_mechanism_id = p_causal_mechanism_id) IS NOT NULL)) THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_predicted_value(p_individual_prediction_id TEXT)
RETURNS NUMERIC AS $$
  SELECT (CASE WHEN ((COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT ((COALESCE(2, 0) * COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT (calc_individual_predictions_individual_causal_mass(p_individual_prediction_id)) AS v) __safe_numeric), 0))) AS v) __safe_numeric), 0) + COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT ((COALESCE(1.5, 0) * COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT (calc_individual_predictions_individual_confirmed_node_count(p_individual_prediction_id)) AS v) __safe_numeric), 0))) AS v) __safe_numeric), 0)))::NUMERIC > 10 THEN (10)::text ELSE ((COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT ((COALESCE(2, 0) * COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT (calc_individual_predictions_individual_causal_mass(p_individual_prediction_id)) AS v) __safe_numeric), 0))) AS v) __safe_numeric), 0) + COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT ((COALESCE(1.5, 0) * COALESCE((SELECT CASE WHEN v::text ~ '^-?[0-9]*\.?[0-9]+$' THEN v::numeric ELSE NULL END FROM (SELECT (calc_individual_predictions_individual_confirmed_node_count(p_individual_prediction_id)) AS v) __safe_numeric), 0))) AS v) __safe_numeric), 0)))::text END)::numeric;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN (calc_individual_predictions_individual_confirmed_node_count(p_individual_prediction_id))::NUMERIC >= 1 THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_has_spurious_correlation_flag(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN (NOT (calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id)) OR COALESCE(calc_individual_predictions_individual_has_cryptic_relatedness(p_individual_prediction_id), FALSE)) THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_is_falsifiability_backed(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN (calc_individual_predictions_individual_confirmed_node_count(p_individual_prediction_id))::NUMERIC >= 1 THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_is_transportable_to_absent_ancestry(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN (calc_individual_predictions_is_ancestry_holdout(p_individual_prediction_id) AND (calc_individual_predictions_individual_cross_ancestry_node_coun(p_individual_prediction_id))::NUMERIC >= 1 AND NOT (calc_individual_predictions_has_spurious_correlation_flag(p_individual_prediction_id))) THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_predicted_severity_value(p_individual_prediction_id TEXT)
RETURNS NUMERIC AS $$
  SELECT (calc_individual_predictions_individual_max_severity_score(p_individual_prediction_id))::numeric;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_is_severity_actionable(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN (COALESCE(calc_individual_predictions_individual_has_high_severity_phenot(p_individual_prediction_id), FALSE) AND calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id) AND NOT (calc_individual_predictions_has_spurious_correlation_flag(p_individual_prediction_id))) THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_severity_deciding_factor(p_individual_prediction_id TEXT)
RETURNS TEXT AS $$
  SELECT (CASE WHEN calc_individual_predictions_is_severity_actionable(p_individual_prediction_id) THEN ('HighSeverityOnConfirmedMechanism')::text ELSE (CASE WHEN NOT (COALESCE(calc_individual_predictions_individual_has_high_severity_phenot(p_individual_prediction_id), FALSE)) THEN ('NotHighSeverity')::text ELSE (CASE WHEN NOT (calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id)) THEN ('NoValidatedMechanism')::text ELSE (CASE WHEN calc_individual_predictions_has_spurious_correlation_flag(p_individual_prediction_id) THEN ('SpuriousFlag')::text ELSE ('Undetermined')::text END)::text END)::text END)::text END)::text;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_is_treatment_response_actionable(p_individual_prediction_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT (CASE WHEN COALESCE(calc_individual_predictions_individual_has_predicted_treatment_(p_individual_prediction_id), FALSE) THEN TRUE ELSE FALSE END)::boolean;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_lifecycle_state_key(p_individual_prediction_id TEXT)
RETURNS TEXT AS $$
  WITH __erb_dedup_v1 AS (SELECT calc_individual_predictions_is_falsifiability_backed(p_individual_prediction_id) AS val), __erb_dedup_v2 AS (SELECT calc_individual_predictions_is_ancestry_transport_safe(p_individual_prediction_id) AS val) SELECT (CASE WHEN (calc_individual_predictions_is_high_confidence_prediction(p_individual_prediction_id) AND (SELECT val FROM __erb_dedup_v1) AND (SELECT val FROM __erb_dedup_v2) AND (calc_individual_predictions_predicted_value(p_individual_prediction_id))::NUMERIC > 0) THEN ('Actionable')::text ELSE (CASE WHEN (NOT (calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id)) OR NOT ((SELECT val FROM __erb_dedup_v1))) THEN ('NotActionable')::text ELSE (CASE WHEN COALESCE(calc_individual_predictions_individual_has_cryptic_relatedness(p_individual_prediction_id), FALSE) THEN ('NotActionable')::text ELSE (CASE WHEN (calc_individual_predictions_calibrated_uncertainty(p_individual_prediction_id))::NUMERIC < 0.7 THEN ('NotActionable')::text ELSE (CASE WHEN NOT ((SELECT val FROM __erb_dedup_v2)) THEN ('NotActionable')::text ELSE ('Actionable')::text END)::text END)::text END)::text END)::text END)::text;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_individual_predictions_deciding_gate(p_individual_prediction_id TEXT)
RETURNS TEXT AS $$
  SELECT (CASE WHEN calc_individual_predictions_is_clinically_actionable(p_individual_prediction_id) THEN ('AllGatesPass')::text ELSE (CASE WHEN NOT (calc_individual_predictions_rests_on_confirmed_mechanism(p_individual_prediction_id)) THEN ('NoValidatedMechanism')::text ELSE (CASE WHEN COALESCE(calc_individual_predictions_individual_has_cryptic_relatedness(p_individual_prediction_id), FALSE) THEN ('CrypticRelatedness')::text ELSE (CASE WHEN (calc_individual_predictions_calibrated_uncertainty(p_individual_prediction_id))::NUMERIC < 0.7 THEN ('Calibration')::text ELSE (CASE WHEN NOT (calc_individual_predictions_is_ancestry_transport_safe(p_individual_prediction_id)) THEN ('AncestryTransport')::text ELSE ('Undetermined')::text END)::text END)::text END)::text END)::text END)::text;
$$ LANGUAGE sql STABLE;