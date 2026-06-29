export interface CaseCell {
  case_cell_id: string;
  study: string;
  stratum_label: string;
  treatment_label: string;
  successes: number;
  cases: number;
  cell_success_rate: number;
  total_cases_for_treatment?: number;
  treatment_exposure_fraction?: number;
}

export interface SandboxEvaluateResult {
  ranking: TreatmentRanking | null;
  summaries: StratumSummary[];
  cells: CaseCell[];
}

export interface Study {
  study_id: string;
  name: string;
  title: string;
  source: string;
  source_url: string;
  publication_year: number | null;
  domain: string | null;
  is_synthetic: boolean | null;
  total_cases: number;
  cell_count: number;
  distortion_type: string | null;
  policy_implication: string | null;
  allocation_distortion: number | null;
  signal_purity: number | null;
  is_sign_flip: boolean | null;
  corrected_winner: string | null;
  confounding_variable: string | null;
  causal_role: string | null;
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
  is_sign_flip: boolean;
  paradox_strength: number;
  reversal_intensity: number;
  threshold_margin: number;
  allocation_distortion: number;
  distortion_ratio: number;
  screening_tier: string;
  signed_pooled_gap: number;
  weighted_stratum_gap_sum: number;
  pooled_gap: number;
  distortion_type: string;
  policy_implication: string;
  confounders_in_study: number;
  arm_size_ratio: number;
  symmetry_departure: number;
  max_stratum_imbalance: number;
  max_stratum_gap: number;
  is_paradox_explained: boolean;
  signal_purity: number | null;
  allocation_direction: string | null;
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

export interface AllocationSweepRow {
  sweep_id: string;
  study_id: string;
  alloc_fraction_a: number;
  is_original: boolean;
  n_sweep_stratum_total: number;
  n_sweep_a: number;
  n_sweep_b: number;
  n_fixed_a: number;
  n_fixed_b: number;
  sweep_rate_a: number;
  sweep_rate_b: number;
  fixed_rate_a: number;
  fixed_rate_b: number;
  sweep_pooled_rate_a: number;
  sweep_pooled_rate_b: number;
  sweep_pooled_gap: number;
  sweep_corrected_gap: number;
  allocation_distortion_witness: number;
}

export interface SweepStudySummary {
  sweep_study_id: string;
  distortion_type_label: string;
  sweep_stratum_label: string;
  corrected_gap_constant: number;
  sweep_corrected_gap_range: number;
  sweep_pooled_gap_max: number;
  sweep_pooled_gap_min: number;
  sweep_pooled_gap_range: number;
  pooled_gap_crosses_zero: boolean;
  invariant_witness: string;
}

export interface SyntheticPhaseRow {
  phase_id: string;
  param_stratum_fraction: number;
  param_stratum_gap1: number;
  param_stratum_gap2: number;
  param_allocation_bias: number;
  phase_signed_pooled_gap: number;
  phase_corrected_gap: number;
  phase_allocation_distortion: number;
  phase_distortion_type: string;
  phase_is_sign_flip: boolean;
  phase_reversal_intensity: number;
}

export interface PhaseDiagramSummary {
  phase_diagram_id: string;
  phase_point_count: number;
  phase_type_a_count: number;
  phase_type_b_count: number;
  phase_type_c_plus_count: number;
  phase_type_c_minus_count: number;
  phase_type_d_count: number;
  all_five_types_present: boolean;
  phase_taxonomy_witness: string;
  phase_witness_note: string;
}

export interface ModelSummary {
  model_summary_id: string;
  name: string;
  reversal_count: number;
  non_reversal_count: number;
  study_count: number;
  explained_count: number;
  total_paradox_strength: number;
  avg_paradox_strength: number;
  type_a_count: number;
  type_b_count: number;
  type_d_count: number;
  distortion_only_count: number;
  min_ratio_type_a: number;
  max_ratio_type_a: number;
  min_ratio_type_b: number;
  max_ratio_type_b: number;
  danger_tier_count: number;
  caution_tier_count: number;
  safe_tier_count: number;
  type_c_plus_count: number;
  type_c_minus_count: number;
  avg_signal_purity: number | null;
  avg_signal_purity_reversal: number | null;
  avg_signal_purity_non_reversal: number | null;
  signal_purity_gap: number | null;
  synthetic_phase_count: number | null;
  phase_diagram_complete: boolean | null;
  phase_taxonomy_coverage: string | null;
  catalog_entry_count: number | null;
  pending_import_count: number | null;
  ready_to_encode_count: number | null;
  import_session_ready: boolean | null;
  catalog_witness_note: string | null;
  latent_type_d_count?: number | null;
  latent_type_d_fraction?: number | null;
  type_d_count?: number | null;
  discovery_witness_note?: string | null;
}

export interface CorpusCatalogSummary {
  catalog_summary_id: string;
  total_catalog_entries: number;
  imported_count: number;
  candidate_count: number;
  blocked_count: number;
  ready_to_encode_count: number;
  high_priority_count: number;
  import_session_ready: boolean;
  catalog_witness_note: string;
}

export interface CandidateStudyRow {
  candidate_id: string;
  proposed_study_id: string;
  title: string;
  citation: string;
  source_url: string | null;
  domain: string;
  stratum_variable_name: string;
  expected_distortion_type: string;
  ingestion_status: string;
  priority: number;
  stratum_count_estimate: number;
  data_source_note: string;
  linked_study_id: string | null;
  publication_year: number | null;
  is_ready_to_encode: boolean;
  is_imported: boolean;
  observed_distortion_type: string | null;
  type_prediction_match: boolean | null;
}

export interface DomainExpansionTarget {
  domain_target_id: string;
  domain: string;
  target_min_count: number;
  current_imported_count: number;
  candidate_queued_count: number;
  projected_count: number;
  gap_count: number;
  is_under_represented: boolean;
}

export interface StudyImportTemplateStep {
  template_step_id: string;
  sort_order: number;
  target_table: string;
  row_description: string;
  required_fields: string;
  mechanical_check: string;
}

export interface Conclusion {
  conclusion_id: string;
  name: string;
  category: string;
  status: string;
  title: string;
  evidence: string | null;
  witnessed_in_loop: string | null;
  target_loop: string | null;
  witnessed_in_loop_commit_hash: string | null;
  witnessed_in_loop_commit_short: string | null;
  witnessed_in_loop_commit_date: string | null;
  witnessed_in_loop_git_tag: string | null;
  tradition_id: string | null;
  researcher_id: string | null;
  challenges_researcher: string | null;
  invariant_protecting_count: number | null;
}

export interface DiscoveryHypothesis {
  hypothesis_id: string;
  name: string;
  statement: string;
  expected_outcome: string;
  registered_in_loop: string;
  tradition_id: string | null;
}

export interface DiscoveryFinding {
  finding_id: string;
  name: string;
  hypothesis_id: string;
  hypothesis_statement: string | null;
  observed_metric: string | null;
  is_confirmed: boolean | null;
  evidence: string | null;
  witnessed_in_loop: string;
}

export interface InvariantCheck {
  invariant_check_id: string;
  name: string;
  natural_language: string;
  source_table: string;
  pass_count: number;
  fail_count: number;
  is_green: boolean;
  status_label: string;
  severity: string;
  protects_conclusion: string | null;
}
