export interface Study {
  study_id: string;
  name: string;
  title: string;
  source: string;
  source_url: string;
  total_cases: number;
  cell_count: number;
}

export interface Stratum {
  stratum_id: string;
  name: string;
  study: string;
  stratum_label: string;
  description: string;
}

export interface StratumSummary {
  stratum_summary_id: string;
  name: string;
  study: string;
  stratum_label: string;
  treatment_label: string;
  stratum_successes: number;
  stratum_cases: number;
  stratum_success_rate: number;
  stratum_rate_a: number;
  stratum_rate_b: number;
  stratum_winner: string;
  stratum_fraction: number;
  weighted_stratum_rate: number;
  allocation_fraction_a: number;
  allocation_fraction_b: number;
  allocation_bias: number;
  stratum_gap: number;
  weighted_stratum_gap: number;
}

export interface TreatmentRanking {
  treatment_ranking_id: string;
  name: string;
  study: string;
  treatment_a: string;
  treatment_b: string;
  pooled_rate_a: number;
  pooled_rate_b: number;
  pooled_winner: string;
  stratum_count: number;
  strata_won_by_a: number;
  strata_won_by_b: number;
  per_stratum_winner: string;
  is_reversal: boolean;
  is_reversal_v2: boolean;
  is_sign_flip: boolean;
  paradox_strength: number;
  reversal_intensity: number;
  threshold_margin: number;
  allocation_distortion: number;
  signed_pooled_gap: number;
  weighted_stratum_gap_sum: number;
  pooled_gap: number;
  distortion_type: string;
  policy_implication: string;
  definition_delta: boolean;
  strict_reversal_subtype: string;
  confounders_in_study: number;
  is_paradox_explained: boolean;
}

export interface Treatment {
  treatment_id: string;
  name: string;
  study: string;
  treatment_label: string;
  description: string;
  total_cases: number;
  total_successes: number;
  pooled_success_rate: number;
}

export interface ModelSummary {
  model_summary_id: string;
  name: string;
  reversal_count: number;
  non_reversal_count: number;
  study_count: number;
  explained_count: number;
  zero_strength_count: number;
  partial_count: number;
  total_paradox_strength: number;
  avg_paradox_strength: number;
  type_a_count: number;
  type_b_count: number;
  type_c_count: number;
  type_d_count: number;
}
