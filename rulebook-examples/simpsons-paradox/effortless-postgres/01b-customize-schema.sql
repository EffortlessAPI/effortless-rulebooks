-- ============================================================================
-- CUSTOMIZE SCHEMA - User-defined schema customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main schema script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom schema changes will appear here:

-- ============================================================================
-- Studies enrichment (loop-47): PublicationYear, Domain, IsSynthetic
-- These raw stratification fields are not emitted by the transpiler quota.
-- Domain values: medicine | epidemiology | social-science | education |
--                legal | sports | economics | synthetic
-- ============================================================================
ALTER TABLE studies ADD COLUMN IF NOT EXISTS publication_year INTEGER;
COMMENT ON COLUMN studies.publication_year IS 'Year the study was published. Used for temporal stratification and pre-registration dating.';

ALTER TABLE studies ADD COLUMN IF NOT EXISTS domain TEXT;
COMMENT ON COLUMN studies.domain IS 'Research domain: medicine | epidemiology | social-science | education | legal | sports | economics | synthetic. Enables cross-domain SignalPurity stratification.';

ALTER TABLE studies ADD COLUMN IF NOT EXISTS is_synthetic BOOLEAN;
COMMENT ON COLUMN studies.is_synthetic IS 'TRUE for constructed/counterfactual studies; FALSE for real published studies. Filters corpus for empirical claims.';

-- ============================================================================
-- Performance cache tables (02b-customize-functions.sql)
-- Materialized once per init-db via refresh_erb_*(); calc_* overrides read here.
-- ============================================================================
CREATE UNLOGGED TABLE IF NOT EXISTS _erb_tr_metrics (
  treatment_ranking_id          TEXT PRIMARY KEY,
  study                         TEXT NOT NULL,
  name                          TEXT,
  total_cases_a                 INTEGER,
  total_successes_a             INTEGER,
  pooled_rate_a                 NUMERIC,
  total_cases_b                 INTEGER,
  total_successes_b             INTEGER,
  pooled_rate_b                 NUMERIC,
  pooled_winner                 TEXT,
  stratum_count                 INTEGER,
  strata_won_by_a               INTEGER,
  strata_won_by_b               INTEGER,
  per_stratum_winner            TEXT,
  is_reversal                   BOOLEAN,
  confounders_in_study          INTEGER,
  is_paradox_explained          BOOLEAN,
  pooled_gap                    NUMERIC,
  strata_won_by_loser           INTEGER,
  paradox_strength              NUMERIC,
  pooled_rate_from_weights_a    NUMERIC,
  pooled_rate_from_weights_b    NUMERIC,
  reversal_intensity            NUMERIC,
  threshold_margin              NUMERIC,
  signed_pooled_gap             NUMERIC,
  weighted_stratum_gap_sum      NUMERIC,
  is_sign_flip                  BOOLEAN,
  allocation_distortion         NUMERIC,
  distortion_type               TEXT,
  policy_implication            TEXT,
  corrected_gap                 NUMERIC,
  corrected_winner              TEXT,
  corrected_vs_pooled_agreement BOOLEAN,
  corrected_policy_implication  TEXT,
  confirmed_causal_role_count   INTEGER,
  mediator_risk_count           INTEGER,
  contested_stratum_count       INTEGER,
  unknown_causal_role_count     INTEGER,
  causal_claim_status           TEXT,
  adjustment_appropriate        BOOLEAN,
  allocation_direction          TEXT,
  signal_purity                 NUMERIC,
  max_stratum_imbalance         NUMERIC,
  max_stratum_gap               NUMERIC,
  pooled_gap_crosses_zero       BOOLEAN,
  sweep_pooled_gap_range        NUMERIC,
  latent_flip_potential         BOOLEAN,
  allocation_fragility          NUMERIC,
  study_domain                  TEXT
);

CREATE INDEX IF NOT EXISTS idx_erb_tr_metrics_study ON _erb_tr_metrics (study);

ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS pooled_gap_crosses_zero BOOLEAN;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS sweep_pooled_gap_range NUMERIC;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS latent_flip_potential BOOLEAN;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS allocation_fragility NUMERIC;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS study_domain TEXT;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS stratum_causal_role TEXT;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS is_latent_only_flip BOOLEAN;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS is_stratum_unanimous BOOLEAN;
ALTER TABLE _erb_tr_metrics ADD COLUMN IF NOT EXISTS is_sweep_fragile BOOLEAN;

CREATE UNLOGGED TABLE IF NOT EXISTS _erb_sp_metrics (
  phase_id                      TEXT PRIMARY KEY,
  name                          TEXT,
  phase_s1_total                NUMERIC,
  phase_na1                     NUMERIC,
  phase_nb1                     NUMERIC,
  phase_na2                     NUMERIC,
  phase_nb2                     NUMERIC,
  phase_rate_a1                 NUMERIC,
  phase_rate_b1                 NUMERIC,
  phase_rate_a2                 NUMERIC,
  phase_rate_b2                 NUMERIC,
  phase_pooled_rate_a           NUMERIC,
  phase_pooled_rate_b           NUMERIC,
  phase_signed_pooled_gap       NUMERIC,
  phase_corrected_gap           NUMERIC,
  phase_strata_won_by_loser     NUMERIC,
  phase_reversal_intensity      NUMERIC,
  phase_is_sign_flip            BOOLEAN,
  phase_allocation_distortion   NUMERIC,
  phase_distortion_type         TEXT
);
