// ============================================================================
//  dag-oracle.js — THE WITNESSED INFERENCE ORACLE
// ============================================================================
//  This is the answer key for the entire IsClinicallyActionable DAG, for all
//  seven oracle patients (A..G). It is the test-first contract for the app.
//
//  Every entry is a triple the harness checks against the running app:
//    - rawFacts : the leaf OBSERVATIONS this node bottoms out in (the only
//                 hand-entered inputs anywhere in the model).
//    - witness  : the expert-verifiable line of reasoning — the derivation
//                 from bootstrap/derivation-spec.md §3 that takes the raw facts
//                 to the expected value. A human domain expert can read this and
//                 confirm the inference is sound WITHOUT trusting the code.
//    - expected : the derived value the APP must surface (captured live from
//                 the vw_* views; this is ground truth, frozen as the oracle).
//
//  Levels (bottom-up, matching the dependency tree in the spec):
//    L0  raw observations (no derivation — included so the witness is complete)
//    L1  per-evidence / per-replication / per-control / per-bin atoms
//    L2  mechanism aggregations
//    L3  mechanism verdicts -> IsCausalArchitectureNode / IsAncestryTransportable
//    L4  individual rollups
//    L5  prediction lookups + PredictedValue + CalibratedUncertainty
//    L6  the four gates
//    L7  the keystone: IsClinicallyActionable
//
//  DO NOT "fix" a failing test by editing `expected` here. `expected` is the
//  oracle. If a test is red, either (a) the app does not yet surface the node
//  (correct — that is the point of Loop 0.5), or (b) a rule changed and the
//  oracle must be deliberately re-captured as part of that rule-change loop.
// ============================================================================

export const PATIENTS = [
  { key: 'A', individual: 'ind-a-reyes', mechanism: 'cm-a', prediction: 'pred-a', variant: 'var-a-irf5', name: 'Ana Reyes', ancestry: 'European',            holdout: false, decidingGate: 'all-pass',           actionable: true  },
  { key: 'B', individual: 'ind-b-okafor', mechanism: 'cm-b', prediction: 'pred-b', variant: 'var-b-irf5', name: 'Bili Okafor', ancestry: 'European',           holdout: false, decidingGate: 'calibration',        actionable: false },
  { key: 'C', individual: 'ind-c-chen',  mechanism: 'cm-c', prediction: 'pred-c', variant: 'var-c-stat4', name: 'Chen Wei',   ancestry: 'East Asian',          holdout: false, decidingGate: 'spurious-mechanism', actionable: false },
  { key: 'D', individual: 'ind-d-santos', mechanism: 'cm-d', prediction: 'pred-d', variant: 'var-d-irf5', name: 'Diego Santos', ancestry: 'European',          holdout: false, decidingGate: 'cryptic-relatedness', actionable: false },
  { key: 'E', individual: 'ind-e-mensah', mechanism: 'cm-e', prediction: 'pred-e', variant: 'var-e-ctla4', name: 'Esi Mensah', ancestry: 'African',            holdout: false, decidingGate: 'falsifiability',     actionable: false },
  { key: 'F', individual: 'ind-f-haidar', mechanism: 'cm-f', prediction: 'pred-f', variant: 'var-f-il23r', name: 'Faisal Haidar', ancestry: 'Indigenous American', holdout: true,  decidingGate: 'ancestry-transport', actionable: false },
  { key: 'G', individual: 'ind-g-lin',   mechanism: 'cm-g', prediction: 'pred-g', variant: 'var-g-il23r', name: 'Grace Lin',  ancestry: 'Indigenous American', holdout: true,  decidingGate: 'transport-passes',   actionable: true  },
  // v2 Step 10 — the cohort expanded to 12 full claim-bearing members (H..L), each
  // cloned from a known-good template profile so its keystone derives correctly.
  // Oracle values below were CAPTURED LIVE from vw_* (capture-new-members.mjs).
  { key: 'H', individual: 'ind-h-yamamoto', mechanism: 'cm-h', prediction: 'pred-h', variant: 'var-h-irf5', name: 'Hana Yamamoto', ancestry: 'East Asian',        holdout: false, decidingGate: 'all-pass',           actionable: true  },
  { key: 'I', individual: 'ind-i-conteh',  mechanism: 'cm-i', prediction: 'pred-i', variant: 'var-i-irf5', name: 'Ibrahim Conteh', ancestry: 'African',           holdout: false, decidingGate: 'cryptic-relatedness', actionable: false },
  { key: 'J', individual: 'ind-j-brooks',  mechanism: 'cm-j', prediction: 'pred-j', variant: 'var-j-irf5', name: 'Jamal Brooks',  ancestry: 'Hispanic/Latino',   holdout: false, decidingGate: 'calibration',        actionable: false },
  { key: 'K', individual: 'ind-k-nair',    mechanism: 'cm-k', prediction: 'pred-k', variant: 'var-k-il23r', name: 'Kavya Nair',   ancestry: 'South Asian',        holdout: true,  decidingGate: 'ancestry-transport', actionable: false },
  { key: 'L', individual: 'ind-l-brandt',  mechanism: 'cm-l', prediction: 'pred-l', variant: 'var-l-il23r', name: 'Lena Brandt',  ancestry: 'Indigenous American', holdout: true,  decidingGate: 'transport-passes',   actionable: true  },
];

export const byPrediction = (id) => PATIENTS.find((p) => p.prediction === id);

// ---------------------------------------------------------------------------
//  L7 — KEYSTONE. IsClinicallyActionable for each patient, with the single
//  deciding gate as the witness.
// ---------------------------------------------------------------------------
export const KEYSTONE = {
  'pred-a': {
    rawFacts: 'European in-training; rare IRF5 variant w/ ASE; 3 qualified evidence (ZStat 4.55/3.84/3.22, 2 cross-modality); 3 replications (2 concordant, 2 cross-ancestry); 2 neg-controls both survive; 1 intervention target; 5 calibration bins coverage 29-34 absErr<=0.02.',
    witness: 'IsHighConfidencePrediction (calibratedUncertainty 0.988>=0.7 AND NOT spurious) ∧ IsFalsifiabilityBacked (1 confirmed node) ∧ IsAncestryTransportSafe (in-training ⇒ vacuous) ∧ PredictedValue 3.02>0  ⇒  TRUE.',
    expected: { is_clinically_actionable: true },
  },
  'pred-b': {
    rawFacts: 'Same confirmed mechanism as A (own distinct effect sizes), but all 5 calibration bins are under-covered (coverage 7-14, each <20).',
    witness: 'CoverageCount 7-14 each <20 ⇒ 0 well-calibrated bins ⇒ WellCalibratedFraction 0 ⇒ CalibratedUncertainty 0<0.7 ⇒ IsHighConfidencePrediction FALSE  ⇒  KEYSTONE FALSE on the CALIBRATION gate alone (mechanism is fully confirmed).',
    expected: { is_clinically_actionable: false },
  },
  'pred-c': {
    rawFacts: 'STAT4 mechanism; replication signs +1/-1/+1 with rep-c-3 p=0.27 (>0.05) ⇒ 1 concordant; one neg-control permutation 0.46 (>0.1 null).',
    witness: 'Only 1 concordant replication (<2) ⇒ NOT ReplicatesAcrossCohorts; control 0.46>0.1 ⇒ NOT SurvivesNegativeControls ⇒ IsSpuriousDerived TRUE ⇒ NOT a node ⇒ 0 confirmed nodes ⇒ HasSpuriousCorrelationFlag TRUE  ⇒  KEYSTONE FALSE on the SPURIOUS-MECHANISM gate.',
    expected: { is_clinically_actionable: false },
  },
  'pred-d': {
    rawFacts: 'Mechanism fully confirmed like A; calibration good like A; but Individuals.HasCrypticRelatednessFlag = TRUE.',
    witness: 'Cryptic relatedness ⇒ IndividualHasCrypticRelatedness TRUE ⇒ HasSpuriousCorrelationFlag TRUE ⇒ IsHighConfidencePrediction FALSE  ⇒  KEYSTONE FALSE on the CRYPTIC-RELATEDNESS (leakage) branch of the spurious gate, despite a valid mechanism and good calibration.',
    expected: { is_clinically_actionable: false },
  },
  'pred-e': {
    rawFacts: 'CTLA4 mechanism: qualified, replicated, controls survive — BUT 0 intervention targets.',
    witness: 'CountInterventionTargets 0 ⇒ IsExperimentallyFalsifiable FALSE ⇒ NOT a node ⇒ 0 confirmed nodes ⇒ IsFalsifiabilityBacked FALSE  ⇒  KEYSTONE FALSE on the FALSIFIABILITY gate.',
    expected: { is_clinically_actionable: false },
  },
  'pred-f': {
    rawFacts: 'Indigenous-American HOLDOUT; confirmed IL23R node; all 3 replications ran in Indigenous-American (same ancestry as mechanism).',
    witness: 'All replications same-ancestry ⇒ CountCrossAncestryConcordant 0 ⇒ IsAncestryTransportable FALSE ⇒ IndividualCrossAncestryNodeCount 0 ⇒ holdout & no measured transport ⇒ IsAncestryTransportSafe FALSE  ⇒  KEYSTONE FALSE on the ANCESTRY-TRANSPORT gate (the marquee requirement).',
    expected: { is_clinically_actionable: false },
  },
  'pred-g': {
    rawFacts: 'Indigenous-American HOLDOUT (twin of F); IL23R effect replicated in European AND East-Asian cohorts (2 cross-ancestry concordant).',
    witness: 'IL23R replicated in 2 DIFFERENT ancestries ⇒ CountCrossAncestryConcordant 2 ⇒ IsAncestryTransportable TRUE ⇒ IsTransportableToAbsentAncestry TRUE ⇒ IsAncestryTransportSafe TRUE; all other gates pass  ⇒  KEYSTONE TRUE — actionable despite holdout, on MEASURED transport, not holdout-status.',
    expected: { is_clinically_actionable: true },
  },
  // v2 Step 10 — expanded cohort (cloned from the template profile noted on each).
  'pred-h': {
    rawFacts: 'Clone of A (all-pass): confirmed IFN-pathway mechanism, well-calibrated, in-training (East Asian). Serology rising-dsDNA + falling-complement + proteinuria 0.7 ⇒ EarlyNephritis.',
    witness: 'All four gates pass exactly as A ⇒ TRUE; independently, the disease-state simulator places her at EarlyNephritis (actionable AND progressing).',
    expected: { is_clinically_actionable: true },
  },
  'pred-i': {
    rawFacts: 'Clone of D (cryptic-relatedness): mechanism confirmed + calibrated, but HasCrypticRelatednessFlag = TRUE. Serology proteinuria 1.5 ⇒ RenalFlareRisk.',
    witness: 'Cryptic relatedness ⇒ HasSpuriousCorrelationFlag ⇒ IsHighConfidencePrediction FALSE ⇒ KEYSTONE FALSE — yet the disease IS progressing (RenalFlareRisk): the SECOND disease-vs-evidence disagreement.',
    expected: { is_clinically_actionable: false },
  },
  'pred-j': {
    rawFacts: 'Clone of B (calibration): confirmed mechanism, but all 5 calibration bins under-covered.',
    witness: 'WellCalibratedFraction 0 ⇒ CalibratedUncertainty 0 ⇒ IsHighConfidencePrediction FALSE ⇒ KEYSTONE FALSE on the CALIBRATION gate.',
    expected: { is_clinically_actionable: false },
  },
  'pred-k': {
    rawFacts: 'Clone of F (ancestry-transport): South-Asian HOLDOUT; confirmed IL23R node, but all replications pinned to South Asian (mechanism ancestry) ⇒ 0 cross-ancestry concordant.',
    witness: 'Holdout AND CountCrossAncestryConcordant 0 ⇒ IsTransportableToAbsentAncestry FALSE ⇒ IsAncestryTransportSafe FALSE ⇒ KEYSTONE FALSE on the ANCESTRY-TRANSPORT gate.',
    expected: { is_clinically_actionable: false },
  },
  'pred-l': {
    rawFacts: 'Clone of G (transport-passes): Indigenous-American HOLDOUT; IL23R node replicated cross-ancestry ⇒ 2 cross-ancestry concordant.',
    witness: 'Holdout but transport passes (cross-ancestry replicated), all gates pass ⇒ TRUE — the positive holdout twin.',
    expected: { is_clinically_actionable: true },
  },
};

// ---------------------------------------------------------------------------
//  L6 — THE FOUR GATES (per prediction). Each gate is its own witnessed value.
// ---------------------------------------------------------------------------
export const GATES = {
  // is_high_confidence_prediction  (calibrated AND not-spurious)
  // is_falsifiability_backed
  // is_ancestry_transport_safe
  // predicted_value > 0
  'pred-a': { is_high_confidence_prediction: true,  is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-b': { is_high_confidence_prediction: false, is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-c': { is_high_confidence_prediction: false, is_falsifiability_backed: false, is_ancestry_transport_safe: true,  predicted_value_positive: false },
  'pred-d': { is_high_confidence_prediction: false, is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-e': { is_high_confidence_prediction: false, is_falsifiability_backed: false, is_ancestry_transport_safe: true,  predicted_value_positive: false },
  'pred-f': { is_high_confidence_prediction: true,  is_falsifiability_backed: true,  is_ancestry_transport_safe: false, predicted_value_positive: true  },
  'pred-g': { is_high_confidence_prediction: true,  is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-h': { is_high_confidence_prediction: true,  is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-i': { is_high_confidence_prediction: false, is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-j': { is_high_confidence_prediction: false, is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
  'pred-k': { is_high_confidence_prediction: true,  is_falsifiability_backed: true,  is_ancestry_transport_safe: false, predicted_value_positive: true  },
  'pred-l': { is_high_confidence_prediction: true,  is_falsifiability_backed: true,  is_ancestry_transport_safe: true,  predicted_value_positive: true  },
};

// ---------------------------------------------------------------------------
//  L6b — THE SINGLE DECIDING GATE (derived `deciding_gate`, keystone altitude).
//  This is the witnessed answer to "five fail, each on one named gate": the
//  keystone is an AND, and deciding_gate names where the AND actually breaks
//  (in keystone priority order), or 'AllGatesPass' when it holds. C and E both
//  read 'NoValidatedMechanism' on purpose: with no confirmed causal node,
//  Falsifiability/Confidence/Magnitude are one finding, not three split gates.
//  Invariant the app must satisfy: deciding_gate === 'AllGatesPass'  ⇔  keystone.
// ---------------------------------------------------------------------------
export const DECIDING_GATE = {
  'pred-a': 'AllGatesPass',
  'pred-b': 'Calibration',
  'pred-c': 'NoValidatedMechanism',
  'pred-d': 'CrypticRelatedness',
  'pred-e': 'NoValidatedMechanism',
  'pred-f': 'AncestryTransport',
  'pred-g': 'AllGatesPass',
  'pred-h': 'AllGatesPass',
  'pred-i': 'CrypticRelatedness',
  'pred-j': 'Calibration',
  'pred-k': 'AncestryTransport',
  'pred-l': 'AllGatesPass',
};

// ---------------------------------------------------------------------------
//  L5 — PREDICTION-LEVEL derived scalars (lookups + PredictedValue + calibration).
// ---------------------------------------------------------------------------
export const PREDICTION_LEVEL = {
  'pred-a': { individual_causal_mass: 0.7583333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: false, predicted_value: 3.0166667, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1.0, calibrated_uncertainty: 0.988, rests_on_confirmed_mechanism: true,  has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: false, patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-b': { individual_causal_mass: 0.7583333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: false, predicted_value: 3.0166667, count_bins: 5, count_well_calibrated_bins: 0, well_calibrated_fraction: 0.0, calibrated_uncertainty: 0.0,   rests_on_confirmed_mechanism: true,  has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: false, patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-c': { individual_causal_mass: 0.0,       individual_confirmed_node_count: 0, individual_cross_ancestry_node_count: 0, individual_has_cryptic_relatedness: false, predicted_value: 0.0,       count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1.0, calibrated_uncertainty: 0.984, rests_on_confirmed_mechanism: false, has_spurious_correlation_flag: true,  is_transportable_to_absent_ancestry: false, patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-d': { individual_causal_mass: 0.7583333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: true,  predicted_value: 3.0166667, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1.0, calibrated_uncertainty: 0.988, rests_on_confirmed_mechanism: true,  has_spurious_correlation_flag: true,  is_transportable_to_absent_ancestry: false, patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-e': { individual_causal_mass: 0.0,       individual_confirmed_node_count: 0, individual_cross_ancestry_node_count: 0, individual_has_cryptic_relatedness: false, predicted_value: 0.0,       count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1.0, calibrated_uncertainty: 0.988, rests_on_confirmed_mechanism: false, has_spurious_correlation_flag: true,  is_transportable_to_absent_ancestry: false, patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-f': { individual_causal_mass: 0.8583333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 0, individual_has_cryptic_relatedness: false, predicted_value: 3.2166667, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1.0, calibrated_uncertainty: 0.986, rests_on_confirmed_mechanism: true,  has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: false, patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-g': { individual_causal_mass: 0.8583333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: false, predicted_value: 3.2166667, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1.0, calibrated_uncertainty: 0.984, rests_on_confirmed_mechanism: true,  has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: true,  patient_stratification_tier: 'Low-Risk Pathway' },
  'pred-h': { individual_causal_mass: 0.7583333333333333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: false, predicted_value: 3.0166666666666666, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1, calibrated_uncertainty: 0.988, rests_on_confirmed_mechanism: true, has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: false, patient_stratification_tier: "Low-Risk Pathway" },
  'pred-i': { individual_causal_mass: 0.7583333333333333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: true, predicted_value: 3.0166666666666666, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1, calibrated_uncertainty: 0.988, rests_on_confirmed_mechanism: true, has_spurious_correlation_flag: true, is_transportable_to_absent_ancestry: false, patient_stratification_tier: "Low-Risk Pathway" },
  'pred-j': { individual_causal_mass: 0.7583333333333333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: false, predicted_value: 3.0166666666666666, count_bins: 5, count_well_calibrated_bins: 0, well_calibrated_fraction: 0, calibrated_uncertainty: 0, rests_on_confirmed_mechanism: true, has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: false, patient_stratification_tier: "Low-Risk Pathway" },
  'pred-k': { individual_causal_mass: 0.8583333333333333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 0, individual_has_cryptic_relatedness: false, predicted_value: 3.216666666666667, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1, calibrated_uncertainty: 0.986, rests_on_confirmed_mechanism: true, has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: false, patient_stratification_tier: "Low-Risk Pathway" },
  'pred-l': { individual_causal_mass: 0.8583333333333333, individual_confirmed_node_count: 1, individual_cross_ancestry_node_count: 1, individual_has_cryptic_relatedness: false, predicted_value: 3.216666666666667, count_bins: 5, count_well_calibrated_bins: 5, well_calibrated_fraction: 1, calibrated_uncertainty: 0.984, rests_on_confirmed_mechanism: true, has_spurious_correlation_flag: false, is_transportable_to_absent_ancestry: true, patient_stratification_tier: "Low-Risk Pathway" },
};

export const PREDICTION_WITNESS = {
  individual_causal_mass:               'SumConfirmedCausalConfidence over the individual\'s confirmed nodes (0 if none confirmed).',
  predicted_value:                      'min(10, 2*causalMass + 1.5*confirmedNodeCount) — rides validated causal mass only, never ancestry correlation.',
  calibrated_uncertainty:               '(1 - meanBinAbsError) * wellCalibratedFraction; 0 when no bin clears coverage>=20 (structural worst-case for absent ancestry).',
  has_spurious_correlation_flag:        'TRUE if NOT rests-on-confirmed-mechanism OR cryptic-relatedness leakage.',
  is_transportable_to_absent_ancestry:  'holdout AND >=1 cross-ancestry-replicated confirmed node AND not spurious.',
};

// ---------------------------------------------------------------------------
//  L5b — SEVERITY prediction (Loop 4). A SECOND derived prediction grounded in
//  ClinicalPhenotypes.SeverityScore, GATED on the onset mechanism gates so it
//  can never be actionable on a debunked/spurious mechanism. Purely additive —
//  it does not touch the onset keystone (A,G actionable; B–F not).
//
//  IsSeverityActionable = HasHighSeverityPhenotype (max severity > 7)
//                         ∧ RestsOnConfirmedMechanism ∧ NOT HasSpuriousCorrelationFlag.
//  Threshold: tier Severe at >7 (matches ClinicalPhenotypes.IsHighSeverity), Moderate >=4, else Mild.
// ---------------------------------------------------------------------------
export const SEVERITY_LEVEL = {
  // A: severe AND onset-actionable — the full pathway.
  'pred-a': { individual_max_severity_score: 9, severity_tier: 'Severe',   is_severity_actionable: true,  severity_deciding_factor: 'HighSeverityOnConfirmedMechanism' },
  // B (marquee): onset FAILS on calibration, yet severity is independently actionable —
  //   the mechanism is confirmed/non-spurious, so the severity claim stands on its own.
  'pred-b': { individual_max_severity_score: 8, severity_tier: 'Severe',   is_severity_actionable: true,  severity_deciding_factor: 'HighSeverityOnConfirmedMechanism' },
  // C: high severity score, but mechanism is debunked ⇒ the chain blocks severity.
  'pred-c': { individual_max_severity_score: 9, severity_tier: 'Severe',   is_severity_actionable: false, severity_deciding_factor: 'NoValidatedMechanism' },
  // D: severe + confirmed mechanism, but a spurious (cryptic-relatedness) flag ⇒ blocked.
  'pred-d': { individual_max_severity_score: 8, severity_tier: 'Severe',   is_severity_actionable: false, severity_deciding_factor: 'SpuriousFlag' },
  // E: mild AND debunked — fails on severity (lower of the two reasons reported).
  'pred-e': { individual_max_severity_score: 3, severity_tier: 'Mild',     is_severity_actionable: false, severity_deciding_factor: 'NotHighSeverity' },
  // F: confirmed mechanism but mild ⇒ fails on severity alone.
  'pred-f': { individual_max_severity_score: 2, severity_tier: 'Mild',     is_severity_actionable: false, severity_deciding_factor: 'NotHighSeverity' },
  // G: onset-actionable but only Moderate severity ⇒ correctly NOT severity-actionable.
  'pred-g': { individual_max_severity_score: 6, severity_tier: 'Moderate', is_severity_actionable: false, severity_deciding_factor: 'NotHighSeverity' },
  'pred-h': { individual_max_severity_score: 9, severity_tier: "Severe", is_severity_actionable: true, severity_deciding_factor: "HighSeverityOnConfirmedMechanism" },
  'pred-i': { individual_max_severity_score: 8, severity_tier: "Severe", is_severity_actionable: false, severity_deciding_factor: "SpuriousFlag" },
  'pred-j': { individual_max_severity_score: 8, severity_tier: "Severe", is_severity_actionable: true, severity_deciding_factor: "HighSeverityOnConfirmedMechanism" },
  'pred-k': { individual_max_severity_score: 2, severity_tier: "Mild", is_severity_actionable: false, severity_deciding_factor: "NotHighSeverity" },
  'pred-l': { individual_max_severity_score: 6, severity_tier: "Moderate", is_severity_actionable: false, severity_deciding_factor: "NotHighSeverity" },
};

export const SEVERITY_WITNESS = {
  individual_max_severity_score: 'MaxSeverityScore = MAXIFS over the individual\'s ClinicalPhenotypes.SeverityScore (0 if none).',
  severity_tier:                 'Severe at SeverityScore>7 (== IsHighSeverity), Moderate at >=4, else Mild.',
  is_severity_actionable:        'high-severity phenotype AND rests-on-confirmed-mechanism AND NOT spurious — chained to the onset gates so severity can\'t fire on a debunked mechanism.',
  severity_deciding_factor:      'the single deciding reason: HighSeverityOnConfirmedMechanism | NotHighSeverity | NoValidatedMechanism | SpuriousFlag.',
};

// ---------------------------------------------------------------------------
//  L5c — TREATMENT-RESPONSE prediction (Loop 5). A THIRD derived prediction,
//  grounded in a MECHANISM MATCH: a Treatment targets a CausalMechanism (FK), and
//  the prediction fires only when that mechanism is a CONFIRMED causal-architecture
//  node AND the treatment is effective (response∈{Complete,Partial} ∧ not adverse).
//  Independent of onset/severity — pred-b again diverges (onset false, txresp true).
//
//  IsTreatmentResponseActionable = HasPredictedTreatmentResponse, where a treatment is
//  predicted iff IsEffectiveTreatment ∧ IsMechanismMatched (target mechanism confirmed).
// ---------------------------------------------------------------------------
export const TREATMENT_LEVEL = {
  // A: anifrolumab targets confirmed cm-a, Partial, no AE ⇒ predicted.
  'pred-a': { is_treatment_response_actionable: true,  treatment_response_deciding_factor: 'EffectiveOnConfirmedMechanism' },
  // B (marquee): onset FAILS on calibration, yet treatment-response is actionable —
  //   the targeted mechanism is confirmed and the drug responds (Complete).
  'pred-b': { is_treatment_response_actionable: true,  treatment_response_deciding_factor: 'EffectiveOnConfirmedMechanism' },
  // C: drug targets cm-c, which failed validation ⇒ no mechanism match.
  'pred-c': { is_treatment_response_actionable: false, treatment_response_deciding_factor: 'NoConfirmedMechanism' },
  // D: targets confirmed cm-d and responds (Complete), but an adverse effect was observed
  //   ⇒ not an effective treatment ⇒ not predicted.
  'pred-d': { is_treatment_response_actionable: false, treatment_response_deciding_factor: 'NoEffectiveTreatmentOnMechanism' },
  // E: targets debunked cm-e, no response ⇒ no mechanism match.
  'pred-e': { is_treatment_response_actionable: false, treatment_response_deciding_factor: 'NoConfirmedMechanism' },
  // F: targets confirmed cm-f but the patient did NOT respond (None) ⇒ not effective.
  'pred-f': { is_treatment_response_actionable: false, treatment_response_deciding_factor: 'NoEffectiveTreatmentOnMechanism' },
  // G: secukinumab targets confirmed cm-g, Partial, no AE ⇒ predicted.
  'pred-g': { is_treatment_response_actionable: true,  treatment_response_deciding_factor: 'EffectiveOnConfirmedMechanism' },
  'pred-h': { is_treatment_response_actionable: true, treatment_response_deciding_factor: "EffectiveOnConfirmedMechanism" },
  'pred-i': { is_treatment_response_actionable: false, treatment_response_deciding_factor: "NoEffectiveTreatmentOnMechanism" },
  'pred-j': { is_treatment_response_actionable: true, treatment_response_deciding_factor: "EffectiveOnConfirmedMechanism" },
  'pred-k': { is_treatment_response_actionable: false, treatment_response_deciding_factor: "NoEffectiveTreatmentOnMechanism" },
  'pred-l': { is_treatment_response_actionable: true, treatment_response_deciding_factor: "EffectiveOnConfirmedMechanism" },
};

export const TREATMENT_WITNESS = {
  is_treatment_response_actionable:    'the individual has a treatment that is effective (Complete/Partial, not adverse) AND whose target mechanism is a confirmed causal node — the mechanism match.',
  treatment_response_deciding_factor:  'EffectiveOnConfirmedMechanism | NoEffectiveTreatmentOnMechanism (adverse or no response) | NoConfirmedMechanism (drug targets a debunked mechanism).',
};

// ---------------------------------------------------------------------------
//  L4 — INDIVIDUAL rollups.
// ---------------------------------------------------------------------------
export const INDIVIDUAL_LEVEL = {
  'ind-a-reyes':  { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: false },
  'ind-b-okafor': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: false },
  'ind-c-chen':   { count_confirmed_causal_nodes: 0, count_cross_ancestry_confirmed_nodes: 0, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: false },
  'ind-d-santos': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: true,  is_ancestry_absent_from_training: false },
  'ind-e-mensah': { count_confirmed_causal_nodes: 0, count_cross_ancestry_confirmed_nodes: 0, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: false },
  'ind-f-haidar': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 0, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: true  },
  'ind-g-lin':    { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: true  },
  'ind-h-yamamoto': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: false },
  'ind-i-conteh': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: true, is_ancestry_absent_from_training: false },
  'ind-j-brooks': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: false },
  'ind-k-nair': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 0, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: true },
  'ind-l-brandt': { count_confirmed_causal_nodes: 1, count_cross_ancestry_confirmed_nodes: 1, has_cryptic_relatedness_flag: false, is_ancestry_absent_from_training: true },
};

// ---------------------------------------------------------------------------
//  L3 — MECHANISM verdicts.  L2 — MECHANISM aggregations.
//  (one row per mechanism; aggregations + verdicts together)
// ---------------------------------------------------------------------------
export const MECHANISM_LEVEL = {
  'cm-a': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true,  count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true,  count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true,  is_spurious_derived: false, causal_confidence: 0.7583333, variant_is_causal_candidate: true, is_causal_architecture_node: true,  is_ancestry_transportable: true  },
  'cm-b': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true,  count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true,  count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true,  is_spurious_derived: false, causal_confidence: 0.7583333, variant_is_causal_candidate: true, is_causal_architecture_node: true,  is_ancestry_transportable: true  },
  'cm-c': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true,  count_replications: 3, count_concordant_replications: 1, count_cross_ancestry_concordant: 0, replicates_across_cohorts: false, count_neg_control_tests: 2, count_neg_control_survived: 1, survives_negative_controls: false, is_spurious_derived: true,  causal_confidence: 0.4583333, variant_is_causal_candidate: true, is_causal_architecture_node: false, is_ancestry_transportable: false },
  'cm-d': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true,  count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true,  count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true,  is_spurious_derived: false, causal_confidence: 0.7583333, variant_is_causal_candidate: true, is_causal_architecture_node: true,  is_ancestry_transportable: true  },
  'cm-e': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 0, is_experimentally_falsifiable: false, count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true,  count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true,  is_spurious_derived: false, causal_confidence: 0.7583333, variant_is_causal_candidate: true, is_causal_architecture_node: false, is_ancestry_transportable: false },
  'cm-f': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true,  count_replications: 3, count_concordant_replications: 3, count_cross_ancestry_concordant: 0, replicates_across_cohorts: true,  count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true,  is_spurious_derived: false, causal_confidence: 0.8583333, variant_is_causal_candidate: true, is_causal_architecture_node: true,  is_ancestry_transportable: false },
  'cm-g': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true,  count_replications: 3, count_concordant_replications: 3, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true,  count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true,  is_spurious_derived: false, causal_confidence: 0.8583333, variant_is_causal_candidate: true, is_causal_architecture_node: true,  is_ancestry_transportable: true  },
  'cm-h': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true, count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 1, replicates_across_cohorts: true, count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true, is_spurious_derived: false, causal_confidence: 0.7583333333333333, variant_is_causal_candidate: true, is_causal_architecture_node: true, is_ancestry_transportable: true },
  'cm-i': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true, count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 1, replicates_across_cohorts: true, count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true, is_spurious_derived: false, causal_confidence: 0.7583333333333333, variant_is_causal_candidate: true, is_causal_architecture_node: true, is_ancestry_transportable: true },
  'cm-j': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true, count_replications: 3, count_concordant_replications: 2, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true, count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true, is_spurious_derived: false, causal_confidence: 0.7583333333333333, variant_is_causal_candidate: true, is_causal_architecture_node: true, is_ancestry_transportable: true },
  'cm-k': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true, count_replications: 3, count_concordant_replications: 3, count_cross_ancestry_concordant: 0, replicates_across_cohorts: true, count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true, is_spurious_derived: false, causal_confidence: 0.8583333333333333, variant_is_causal_candidate: true, is_causal_architecture_node: true, is_ancestry_transportable: false },
  'cm-l': { count_qualified_evidence: 3, count_modalities_supporting: 2, count_intervention_targets: 1, is_experimentally_falsifiable: true, count_replications: 3, count_concordant_replications: 3, count_cross_ancestry_concordant: 2, replicates_across_cohorts: true, count_neg_control_tests: 2, count_neg_control_survived: 2, survives_negative_controls: true, is_spurious_derived: false, causal_confidence: 0.8583333333333333, variant_is_causal_candidate: true, is_causal_architecture_node: true, is_ancestry_transportable: true },
};

export const MECHANISM_WITNESS = {
  is_causal_architecture_node:  'CausalConfidence>=0.7 ∧ IsExperimentallyFalsifiable ∧ NOT IsSpuriousDerived ∧ (variant candidate OR exposure).',
  is_spurious_derived:          'spurious if NOT ReplicatesAcrossCohorts OR NOT SurvivesNegativeControls OR <2 modalities OR pleiotropy.',
  is_experimentally_falsifiable:'>=1 intervention target ∧ >=1 qualified evidence.',
  is_ancestry_transportable:    'a confirmed node whose effect replicated in >=1 DIFFERENT ancestry (CountCrossAncestryConcordant>=1).',
  causal_confidence:            '0.30*min(1,qualEv/4)+0.20*min(1,modalities/3)+0.30*replFraction+0.20*survivesControls, capped at 1.',
};

// ---------------------------------------------------------------------------
//  L1 — ATOMS: per-evidence, per-replication, per-control, per-bin derivations.
//  These are the rows that the L2 aggregations count over. We assert a
//  representative, story-critical atom per mechanism/prediction.
// ---------------------------------------------------------------------------
export const EVIDENCE_ATOMS = {
  // mechanism -> the qualified-evidence count its rows produce, with the ZStat witness
  'cm-a': { count_qualified: 3, witness: 'ZStat = |effect|/SE = 0.91/0.20 = 4.55 (and 3.84, 3.22) all >=2, high-quality assay, confound-controlled, not control-arm ⇒ all 3 qualify.' },
  'cm-c': { count_qualified: 3, witness: 'Same evidence quality as A (3 qualify) — C\'s failure is at replication/control, NOT evidence.' },
  'cm-e': { count_qualified: 3, witness: 'Evidence qualifies (3) — E\'s failure is the missing intervention target, NOT evidence.' },
};

export const REPLICATION_ATOMS = {
  // the single deciding replication fact per transport-relevant mechanism
  'cm-c': { count_cross_ancestry_concordant: 0, witness: 'rep-c-2 sign -1 (discordant) and rep-c-3 p=0.27 (>0.05) ⇒ only 1 concordant; cross-ancestry concordant 0.' },
  'cm-f': { count_cross_ancestry_concordant: 0, witness: 'All 3 replications ran in Indigenous-American = mechanism ancestry ⇒ IsDifferentAncestryReplication FALSE for all ⇒ 0 cross-ancestry concordant.' },
  'cm-g': { count_cross_ancestry_concordant: 2, witness: 'rep-g-1 (European, p=0.004,+1) and rep-g-2 (East Asian, p=0.012,+1) are sig + different-ancestry ⇒ 2 cross-ancestry concordant.' },
};

export const CONTROL_ATOMS = {
  'cm-a': { count_survived: 2, witness: 'nct-a-1 (0.012) and nct-a-2 (0.028) both within +/-0.1 null band ⇒ both survive ⇒ SurvivesNegativeControls.' },
  'cm-c': { count_survived: 1, witness: 'nct-c-2 permutationEffectSize 0.46 > 0.1 null threshold ⇒ does NOT collapse ⇒ NOT survived ⇒ SurvivesNegativeControls FALSE.' },
};

export const CALIBRATION_ATOMS = {
  'pred-a': { count_well_calibrated_bins: 5, witness: 'All 5 bins coverage 29-34 (>=20) and binAbsError<=0.02 (<=0.1) ⇒ all well-calibrated.' },
  'pred-b': { count_well_calibrated_bins: 0, witness: 'All 5 bins coverage 7-14 (<20) ⇒ NONE well-calibrated, regardless of low abs error ⇒ the calibration gate fails here alone.' },
};

// ---------------------------------------------------------------------------
//  L0 — RAW OBSERVATIONS (variant candidacy is the lowest derived bit; the
//  allele-frequency / ASE facts beneath it are pure observations).
// ---------------------------------------------------------------------------
export const VARIANT_LEVEL = {
  'var-a-irf5':  { allele_frequency: 0.006, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-c-stat4': { allele_frequency: 0.008, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-f-il23r': { allele_frequency: 0.009, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-h-irf5': { allele_frequency: 0.006, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-i-irf5': { allele_frequency: 0.006, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-j-irf5': { allele_frequency: 0.006, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-k-il23r': { allele_frequency: 0.009, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
  'var-l-il23r': { allele_frequency: 0.009, is_rare_variant: true, has_allele_specific_expression: true, is_causal_candidate: true },
};

export const VARIANT_WITNESS =
  'IsCausalCandidate = (rare-by-frequency OR rare-by-class) AND HasAlleleSpecificExpression. ' +
  'All seven seed variants are rare + ASE ⇒ candidate TRUE; candidacy alone does NOT confirm a node.';

// Numeric tolerance for the few non-integer derived scalars.
export const TOL = 1e-3;

// ===========================================================================
//  L8 — LIFECYCLE PATHS (the case-by-case walk through the diagnosis machine).
// ===========================================================================
//  Each patient walks its OWN branch from Intake down to a terminal, stopping
//  at its single deciding gate. The path is DERIVED from the frozen decidingGate
//  above — NOT hand-typed independently — so this section can never drift from
//  the keystone oracle. The app must surface the same ordered StateKeys via
//  GET /api/predictions/:id/lifecycle (states[]), ending at `terminal`.
//
//  Machine spine: Intake → EvidenceAssessed → MechanismConfirmed →
//                 CalibrationChecked → TransportChecked → Actionable.
//  A failing gate diverts to NotActionable at the step that gate is checked.
// ---------------------------------------------------------------------------
const SPINE = ['Intake', 'EvidenceAssessed', 'MechanismConfirmed', 'CalibrationChecked', 'TransportChecked', 'Actionable'];

// decidingGate -> the spine state AFTER which the case diverts to NotActionable.
// (all-pass / transport-passes ride the full spine to Actionable.)
const DIVERT_AFTER = {
  'spurious-mechanism': 'EvidenceAssessed',   // C: not a node ⇒ stop after evidence
  'falsifiability': 'EvidenceAssessed',       // E: 0 intervention targets ⇒ stop after evidence
  'cryptic-relatedness': 'MechanismConfirmed', // D: leakage ⇒ stop after mechanism
  'calibration': 'CalibrationChecked',        // B: uncertainty<0.7 ⇒ stop after calibration
  'ancestry-transport': 'TransportChecked',   // F: not transport-safe ⇒ stop after transport
};

// Build the ordered lifecycle path for one patient from its decidingGate.
function pathFor(p) {
  if (p.actionable) return [...SPINE]; // A, G ride to Actionable
  const after = DIVERT_AFTER[p.decidingGate];
  const upto = SPINE.slice(0, SPINE.indexOf(after) + 1);
  return [...upto, 'NotActionable'];
}

export const LIFECYCLE_PATHS = Object.fromEntries(
  PATIENTS.map((p) => {
    const states = pathFor(p);
    return [p.prediction, {
      states,
      terminal: states[states.length - 1],
      is_clinically_actionable: p.actionable,
      witness: `${p.key} · ${p.name}: walks ${states.join(' → ')} — diverges at the ${p.decidingGate} gate; the IsCurrent occupancy's StateKey must equal the derived LifecycleStateKey.`,
    }];
  }),
);

// ===========================================================================
//  L9 — ROUTING: per-entity RelativePath + the role nav trees.
// ===========================================================================
//  RelativePath is a computed field assembling each entity's own page path,
//  chaining its parent's path where hierarchical. We assert one representative
//  row per depth so the chain (case → prediction → bins) is witnessed. The app
//  surfaces relative_path on every vw_* row (e.g. /api/predictions/:id).
export const ROUTING = {
  // entity endpoint + id field -> expected relative_path
  relativePaths: [
    { endpoint: '/api/individuals/ind-a-reyes', field: 'relative_path', expected: '/intake/new-patient/reyes-ana' },
    { endpoint: '/api/predictions/pred-a', field: 'relative_path', expected: '/intake/new-patient/reyes-ana/predictions/pred-a' },
    { endpoint: '/api/mechanisms/cm-a', field: 'relative_path', expected: '/intake/new-patient/reyes-ana/mechanisms/cm-a' },
    { endpoint: '/api/variants/var-a-irf5', field: 'relative_path', expected: '/intake/new-patient/reyes-ana/variants/var-a-irf5' },
  ],
  // role -> the set of TOP-LEVEL route_keys that role can see. Cohort discovery
  // (admin.cohort) is promoted to a top-level item EVERY role sees — the corpus-
  // level punchline sits at the top of the nav for everyone. admin still also sees
  // the intake + admin trees.
  navTrees: {
    'intake-clinician': ['admin.cohort', 'intake'],
    'diagnosing-doctor': ['admin.cohort', 'diagnosis'],
    'admin': ['admin.cohort', 'intake', 'diagnosis', 'admin'],
  },
};

// ===========================================================================
//  v2 — THE DISEASE-STATE SIMULATOR (the layer the v1 audit said didn't exist)
// ===========================================================================
//  v1 answered "is the evidence actionable?" (the gates). v2 answers "is the
//  DISEASE progressing?" — a witnessed state machine whose current state is
//  derived purely from raw serology leaves (anti-dsDNA trend, complement trend,
//  proteinuria, sediment). Captured live from vw_individuals / vw_individual_
//  predictions / vw_subject_state_instances; frozen here as the oracle.
//
//  ALL NINE individuals carry a disease state (the original 7 + 2 progression
//  demos H,I). Only the original 7 have predictions (and thus a treatment line).
// ===========================================================================

// The full progression-demo cohort (the 7 oracle patients + 2 new ones).
export const PROGRESSION_INDIVIDUALS = [
  ...PATIENTS.map((p) => ({ key: p.key, individual: p.individual, name: p.name })),
  // H..L now live in PATIENTS (full members), so PATIENTS.map above already
  // includes them — no separate rows needed.
];

// L11 — per-individual derived disease state (served by /api/individuals/:id).
export const DISEASE_STATE = {
  'ind-a-reyes':    { nephritis_progression_state_key: 'SerologicActive',            latest_sledai_score: 4, activity_tier: 'Mild',        is_disease_progressing: false },
  'ind-b-okafor':   { nephritis_progression_state_key: 'PresymptomaticAutoimmunity', latest_sledai_score: 0, activity_tier: 'Quiescent',   is_disease_progressing: false },
  'ind-c-chen':     { nephritis_progression_state_key: 'SerologicActive',            latest_sledai_score: 4, activity_tier: 'Mild',        is_disease_progressing: false },
  // D — THE COUNTER-EXAMPLE: progressing to biopsy at HIGH activity (SLEDAI 12), yet the
  //     prediction is NOT actionable (cryptic gate). Captured live from vw_individuals (:5432).
  'ind-d-santos':   { nephritis_progression_state_key: 'BiopsyIndicated',            latest_sledai_score: 12, activity_tier: 'High / flare', is_disease_progressing: true  },
  'ind-e-mensah':   { nephritis_progression_state_key: 'PresymptomaticAutoimmunity', latest_sledai_score: 0,  activity_tier: 'Quiescent',    is_disease_progressing: false },
  'ind-f-haidar':   { nephritis_progression_state_key: 'PresymptomaticAutoimmunity', latest_sledai_score: 0,  activity_tier: 'Quiescent',    is_disease_progressing: false },
  'ind-g-lin':      { nephritis_progression_state_key: 'PresymptomaticAutoimmunity', latest_sledai_score: 0,  activity_tier: 'Quiescent',    is_disease_progressing: false },
  'ind-h-yamamoto': { nephritis_progression_state_key: 'EarlyNephritis',             latest_sledai_score: 8,  activity_tier: 'Moderate',     is_disease_progressing: true  },
  'ind-i-conteh':   { nephritis_progression_state_key: 'RenalFlareRisk',             latest_sledai_score: 8,  activity_tier: 'Moderate',     is_disease_progressing: true  },
  'ind-j-brooks':   { nephritis_progression_state_key: 'SerologicActive',            latest_sledai_score: 4,  activity_tier: 'Mild',         is_disease_progressing: false },
  'ind-k-nair':     { nephritis_progression_state_key: 'PresymptomaticAutoimmunity', latest_sledai_score: 0,  activity_tier: 'Quiescent',    is_disease_progressing: false },
  'ind-l-brandt':   { nephritis_progression_state_key: 'PresymptomaticAutoimmunity', latest_sledai_score: 0,  activity_tier: 'Quiescent',    is_disease_progressing: false },
};

export const DISEASE_STATE_WITNESS = {
  nephritis_progression_state_key: 'DERIVED from the highest progression-state order reached across the individual\'s raw serology panels (proteinuria/sediment ⇒ BiopsyIndicated; proteinuria≥1 ⇒ RenalFlareRisk; ≥0.5 ⇒ EarlyNephritis; rising-dsDNA+falling-complement ⇒ SerologicActive; else Presymptomatic). Never hand-set.',
  latest_sledai_score:             'DERIVED peak SLEDAI-style activity = renal sub-score (0/4/8 from proteinuria+sediment) + serology sub-score (0/2/4 from low-complement+raised-dsDNA), MAXIFS across panels.',
  activity_tier:                   'DERIVED tier from peak SLEDAI: ≥12 High/flare, ≥6 Moderate, ≥1 Mild, else Quiescent.',
  is_disease_progressing:          'DERIVED: TRUE when the disease state is EarlyNephritis/RenalFlareRisk/BiopsyIndicated — INDEPENDENT of the actionability gate (that independence is the whole point).',
};

// L11b — per-individual progression WALK (served by /api/individuals/:id/progression).
export const PROGRESSION_PATHS = {
  'ind-a-reyes':    { states: ['PresymptomaticAutoimmunity', 'SerologicActive'], current: 'SerologicActive' },
  'ind-b-okafor':   { states: ['PresymptomaticAutoimmunity'], current: 'PresymptomaticAutoimmunity' },
  'ind-c-chen':     { states: ['PresymptomaticAutoimmunity', 'SerologicActive'], current: 'SerologicActive' },
  'ind-d-santos':   { states: ['PresymptomaticAutoimmunity', 'RenalFlareRisk', 'BiopsyIndicated'], current: 'BiopsyIndicated' },
  'ind-e-mensah':   { states: ['PresymptomaticAutoimmunity'], current: 'PresymptomaticAutoimmunity' },
  'ind-f-haidar':   { states: ['PresymptomaticAutoimmunity'], current: 'PresymptomaticAutoimmunity' },
  'ind-g-lin':      { states: ['PresymptomaticAutoimmunity'], current: 'PresymptomaticAutoimmunity' },
  'ind-h-yamamoto': { states: ['PresymptomaticAutoimmunity', 'EarlyNephritis'], current: 'EarlyNephritis' },
  'ind-i-conteh':   { states: ['EarlyNephritis', 'RenalFlareRisk'], current: 'RenalFlareRisk' },
  'ind-j-brooks':   { states: ['PresymptomaticAutoimmunity', 'SerologicActive'], current: 'SerologicActive' },
  'ind-k-nair':     { states: ['PresymptomaticAutoimmunity'], current: 'PresymptomaticAutoimmunity' },
  'ind-l-brandt':   { states: ['PresymptomaticAutoimmunity'], current: 'PresymptomaticAutoimmunity' },
};

// L13 — the EMERGENT PRE-NEPHRITIC SEROLOGY-SIGNATURE CLUSTER (served on
// /api/cohort-individuals). DERIVED per individual: the count of panels showing
// rising-anti-dsDNA + falling-complement, rolled up to a cluster-membership
// boolean + a 0/1/2 strength. The discovery claim made falsifiable at corpus
// scale: cluster membership EMERGES from the population's raw serology series and
// tracks the nephritis trajectory — no label is assigned anywhere.
//
// The separation invariant the test enforces (the actual "finding"):
//   every cluster MEMBER is active or progressing (SerologicActive or a nephritis
//   state); every NON-member is quiescent PresymptomaticAutoimmunity.
export const SIGNATURE_CLUSTER = {
  'ind-a-reyes':    { is_in_pre_nephritic_signature_cluster: true,  signature_strength: 1 },
  'ind-b-okafor':   { is_in_pre_nephritic_signature_cluster: false, signature_strength: 0 },
  'ind-c-chen':     { is_in_pre_nephritic_signature_cluster: true,  signature_strength: 1 },
  'ind-d-santos':   { is_in_pre_nephritic_signature_cluster: true,  signature_strength: 2 }, // persistent — most progressed
  'ind-e-mensah':   { is_in_pre_nephritic_signature_cluster: false, signature_strength: 0 },
  'ind-f-haidar':   { is_in_pre_nephritic_signature_cluster: false, signature_strength: 0 },
  'ind-g-lin':      { is_in_pre_nephritic_signature_cluster: false, signature_strength: 0 },
  'ind-h-yamamoto': { is_in_pre_nephritic_signature_cluster: true,  signature_strength: 1 },
  'ind-i-conteh':   { is_in_pre_nephritic_signature_cluster: true,  signature_strength: 1 },
  'ind-j-brooks':   { is_in_pre_nephritic_signature_cluster: true,  signature_strength: 1 },
  'ind-k-nair':     { is_in_pre_nephritic_signature_cluster: false, signature_strength: 0 },
  'ind-l-brandt':   { is_in_pre_nephritic_signature_cluster: false, signature_strength: 0 },
};

// L5d — TREATMENT-LINE selection (served by /api/predictions/:id). The audit's
// second worked example: MMF vs belimumab vs anifrolumab, with a single deciding
// reason, derived from the confirmed-mechanism pathway + the disease state.
export const TREATMENT_LINE = {
  'pred-a': { recommended_treatment_line: 'Anifrolumab',                              treatment_line_deciding_factor: 'IFNSignature-Anifrolumab',    progression_vs_actionability_disagree: false },
  'pred-b': { recommended_treatment_line: 'Anifrolumab',                              treatment_line_deciding_factor: 'IFNSignature-Anifrolumab',    progression_vs_actionability_disagree: false },
  'pred-c': { recommended_treatment_line: 'No targeted line — mechanism unconfirmed', treatment_line_deciding_factor: 'MechanismUnconfirmed',        progression_vs_actionability_disagree: false },
  // D — the counter-example: disease progressing (BiopsyIndicated) ⇒ MMF induction,
  //     yet the keystone is NOT actionable (cryptic) ⇒ disagree = TRUE.
  'pred-d': { recommended_treatment_line: 'Mycophenolate (induction)',                treatment_line_deciding_factor: 'ActiveNephritis-Induction',   progression_vs_actionability_disagree: true  },
  'pred-e': { recommended_treatment_line: 'No targeted line — mechanism unconfirmed', treatment_line_deciding_factor: 'MechanismUnconfirmed',        progression_vs_actionability_disagree: false },
  'pred-f': { recommended_treatment_line: 'Secukinumab',                              treatment_line_deciding_factor: 'IL17Axis-Secukinumab',        progression_vs_actionability_disagree: false },
  'pred-g': { recommended_treatment_line: 'Secukinumab',                              treatment_line_deciding_factor: 'IL17Axis-Secukinumab',        progression_vs_actionability_disagree: false },
  'pred-h': { recommended_treatment_line: "Anifrolumab", treatment_line_deciding_factor: "IFNSignature-Anifrolumab", progression_vs_actionability_disagree: false },
  'pred-i': { recommended_treatment_line: "Mycophenolate (induction)", treatment_line_deciding_factor: "ActiveNephritis-Induction", progression_vs_actionability_disagree: true },
  'pred-j': { recommended_treatment_line: "Anifrolumab", treatment_line_deciding_factor: "IFNSignature-Anifrolumab", progression_vs_actionability_disagree: false },
  'pred-k': { recommended_treatment_line: "Secukinumab", treatment_line_deciding_factor: "IL17Axis-Secukinumab", progression_vs_actionability_disagree: false },
  'pred-l': { recommended_treatment_line: "Secukinumab", treatment_line_deciding_factor: "IL17Axis-Secukinumab", progression_vs_actionability_disagree: false },
};

export const TREATMENT_LINE_WITNESS = {
  recommended_treatment_line:            'DERIVED: if mechanism unconfirmed ⇒ no targeted line; else if disease is in RenalFlareRisk/BiopsyIndicated ⇒ MMF induction (state overrides pathway); else by confirmed-mechanism TargetPathway: IFN⇒Anifrolumab, B-cell⇒Belimumab, IL-17/23⇒Secukinumab.',
  treatment_line_deciding_factor:        'the single reason for the line (mirrors DecidingGate): MechanismUnconfirmed | ActiveNephritis-Induction | IFNSignature-Anifrolumab | AutoantibodyDriven-Belimumab | IL17Axis-Secukinumab.',
  progression_vs_actionability_disagree: 'THE COUNTER-EXAMPLE: TRUE when the disease-state simulator says the disease IS progressing while the actionability gate says the prediction is NOT actionable. Proves the two layers are independent and both real.',
};
