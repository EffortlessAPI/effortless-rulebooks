#!/usr/bin/env python3
"""Loop 0 transform: convert the autoimmune rulebook from data-entry to solve-by-inference.

Implements bootstrap/derivation-spec.md:
  - delete the 7 hand-entered 'answer' fields
  - add 4 new evidence/replication/control/calibration tables
  - rewire CausalMechanisms / Individuals / IndividualPredictions / GenomicVariants
    into a derived DAG that bottoms out only in raw observations
  - reseed mock data: 7 named patients (A-G), each failing exactly one gate (+ positives)

Re-runnable: reads the .bak, writes effortless-rulebook.json. Idempotent w.r.t. the .bak source.
"""
import json, os, collections

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RB = os.path.join(ROOT, "effortless-rulebook", "effortless-rulebook.json")
BAK = RB + ".bak"

d = json.load(open(BAK), object_pairs_hook=collections.OrderedDict)

def F(name, datatype, ftype, desc, formula=None, related=None, nullable=True):
    o = collections.OrderedDict()
    o["name"] = name; o["datatype"] = datatype; o["type"] = ftype; o["nullable"] = nullable
    o["Description"] = desc
    if formula is not None: o["formula"] = formula
    if related is not None: o["RelatedTo"] = related
    return o

def schema(t): return d[t]["schema"]
def fieldnames(t): return [f["name"] for f in schema(t)]
def drop(t, *names):
    d[t]["schema"] = [f for f in schema(t) if f["name"] not in names]
def replace_or_add(t, field):
    s = schema(t)
    for i, f in enumerate(s):
        if f["name"] == field["name"]:
            s[i] = field; return
    s.append(field)
def insert_after(t, after_name, field):
    s = schema(t);
    for i, f in enumerate(s):
        if f["name"] == after_name:
            s.insert(i+1, field); return
    s.append(field)

# ---------------------------------------------------------------------------
# 1. NEW TABLES
# ---------------------------------------------------------------------------
def new_table(desc, fields):
    o = collections.OrderedDict()
    o["Description"] = desc; o["schema"] = fields; o["data"] = []
    return o

EvidenceItems = new_table(
    "One observed support signal for a causal mechanism, measured by one omics assay. Mechanism confidence is an aggregation over these rows.",
    [
        F("EvidenceItemId","string","raw","Stable id for this evidence observation.",nullable=False),
        F("EvidenceLabel","string","raw","Human label for the evidence row.",nullable=False),
        F("CausalMechanism","string","raw","FK to the mechanism this evidence supports.",related="CausalMechanisms",nullable=False),
        F("OmicsAssay","string","raw","FK to the omics assay this signal was measured in.",related="OmicsAssays",nullable=False),
        F("EffectSize","number","raw","OBSERVATION: measured (absolute) effect magnitude.",nullable=False),
        F("StandardError","number","raw","OBSERVATION: measured standard error of the effect.",nullable=False),
        F("IsCrossModality","boolean","raw","OBSERVATION: TRUE if this signal comes from a different omics modality than the mechanism's primary one.",nullable=False),
        F("IsNegativeControlArm","boolean","raw","OBSERVATION: TRUE if this row is a measured control arm rather than support.",nullable=False),
        F("IsAdjustedForAncestryPCs","boolean","raw","OBSERVATION: TRUE if the analysis adjusted for ancestry principal components.",nullable=False),
        F("IsAdjustedForBatch","boolean","raw","OBSERVATION: TRUE if the analysis adjusted for batch.",nullable=False),
        F("Name","string","calculated","Display name.","={{EvidenceLabel}}",nullable=False),
        F("AssayIsHighQuality","boolean","lookup","Whether the supporting assay passed quality control.",
          "=INDEX(OmicsAssays!{{IsHighQualityAssay}}, MATCH({{OmicsAssay}}, OmicsAssays!{{OmicsAssayId}}, 0))"),
        F("ZStat","number","calculated","Derived signal-to-noise ratio (effect / SE), 0 if SE non-positive.",
          "=IF({{StandardError}} > 0, {{EffectSize}} / {{StandardError}}, 0)"),
        F("IsConfoundControlled","boolean","calculated","Derived: both ancestry-PC and batch adjustment present.",
          "=IF(AND({{IsAdjustedForAncestryPCs}}, {{IsAdjustedForBatch}}), TRUE(), FALSE())"),
        F("IsQualifiedEvidence","boolean","calculated","Derived: clean assay, real support arm, signal-to-noise >= 2, confound-controlled.",
          "=IF(AND({{AssayIsHighQuality}}, NOT({{IsNegativeControlArm}}), {{ZStat}} >= 2, {{IsConfoundControlled}}), TRUE(), FALSE())"),
    ])

CohortReplications = new_table(
    "One re-test of a causal mechanism in another federated cohort. Replication and cross-ancestry transport are aggregations over these rows.",
    [
        F("CohortReplicationId","string","raw","Stable id for this replication attempt.",nullable=False),
        F("ReplicationLabel","string","raw","Human label.",nullable=False),
        F("CausalMechanism","string","raw","FK to the mechanism being re-tested.",related="CausalMechanisms",nullable=False),
        F("FederatedDataset","string","raw","FK to the cohort node the re-test ran in.",related="FederatedDatasets",nullable=False),
        F("ReplicationEffectSign","integer","raw","OBSERVATION: sign of the re-estimated effect (+1 / -1 / 0).",nullable=False),
        F("ReplicationPValue","number","raw","OBSERVATION: p-value of the re-test.",nullable=False),
        F("ReplicationAncestryLabel","string","raw","OBSERVATION: ancestry of the cohort the re-test ran in.",nullable=False),
        F("Name","string","calculated","Display name.","={{ReplicationLabel}}",nullable=False),
        F("ReplicatedAtNominalSig","boolean","calculated","Derived: reproduced the predicted (positive) sign at nominal significance.",
          "=IF(AND({{ReplicationPValue}} <= 0.05, {{ReplicationEffectSign}} = 1), TRUE(), FALSE())"),
        F("MechanismPrimaryAncestry","string","lookup","Ancestry of the individual the mechanism was discovered in.",
          "=INDEX(CausalMechanisms!{{IndividualAncestryLabel}}, MATCH({{CausalMechanism}}, CausalMechanisms!{{CausalMechanismId}}, 0))"),
        F("IsDifferentAncestryReplication","boolean","calculated","Derived: the re-test ran in a different ancestry than discovery.",
          '=IF({{ReplicationAncestryLabel}} = {{MechanismPrimaryAncestry}}, FALSE(), TRUE())'),
        F("IsCrossAncestryConcordant","boolean","calculated","Derived: replicated at significance AND in a different ancestry (the transportability atom).",
          "=IF(AND({{ReplicatedAtNominalSig}}, {{IsDifferentAncestryReplication}}), TRUE(), FALSE())"),
    ])

NegativeControlTests = new_table(
    "One stratification / permutation control on a causal mechanism. A true causal signal collapses under the control.",
    [
        F("NegativeControlTestId","string","raw","Stable id for this control test.",nullable=False),
        F("ControlLabel","string","raw","Human label.",nullable=False),
        F("CausalMechanism","string","raw","FK to the mechanism being controlled.",related="CausalMechanisms",nullable=False),
        F("TestKind","string","raw","OBSERVATION: kind of control (ancestry-permutation / batch-stratified / negative-control).",nullable=False),
        F("PermutationEffectSize","number","raw","OBSERVATION: (absolute) effect measured under the null/permuted control.",nullable=False),
        F("NullThreshold","number","raw","OBSERVATION: pre-registered null-band half-width the control must stay within.",nullable=False),
        F("Name","string","calculated","Display name.","={{ControlLabel}}",nullable=False),
        F("IsSurvived","boolean","calculated","Derived: signal collapses under the control (within the null band), as a true causal effect should.",
          "=IF({{PermutationEffectSize}} <= {{NullThreshold}}, TRUE(), FALSE())"),
    ])

CalibrationBins = new_table(
    "Per-prediction reliability-curve bins, seeded from the held-out coverage of this individual's own ancestry x disease. Calibration is an aggregation over these rows.",
    [
        F("CalibrationBinId","string","raw","Stable id for this bin.",nullable=False),
        F("BinLabel","string","raw","Human label.",nullable=False),
        F("IndividualPrediction","string","raw","FK to the prediction this bin calibrates.",related="IndividualPredictions",nullable=False),
        F("PredictedProbabilityBand","number","raw","OBSERVATION: predicted-probability midpoint of this bin (0-1).",nullable=False),
        F("ObservedEventRate","number","raw","OBSERVATION: empirical event rate among held-out cases in this band, matched to this individual's ancestry x disease.",nullable=False),
        F("CoverageCount","integer","raw","OBSERVATION: number of held-out outcomes in this band for this ancestry x disease.",nullable=False),
        F("Name","string","calculated","Display name.","={{BinLabel}}",nullable=False),
        F("BinAbsError","number","calculated","Derived: absolute gap between predicted band and observed rate.",
          "=IF({{PredictedProbabilityBand}} >= {{ObservedEventRate}}, {{PredictedProbabilityBand}} - {{ObservedEventRate}}, {{ObservedEventRate}} - {{PredictedProbabilityBand}})"),
        F("IsWellCalibratedBin","boolean","calculated","Derived: enough held-out coverage AND small reliability gap.",
          "=IF(AND({{CoverageCount}} >= 20, {{BinAbsError}} <= 0.1), TRUE(), FALSE())"),
    ])

# Insert new tables right after their parents for readability
def insert_table_after(after_key, key, table):
    keys = list(d.keys()); newd = collections.OrderedDict()
    for k in keys:
        newd[k] = d[k]
        if k == after_key: newd[key] = table
    return newd

d = insert_table_after("OmicsAssays", "EvidenceItems", EvidenceItems)
d = insert_table_after("EvidenceItems", "CohortReplications", CohortReplications)
d = insert_table_after("CohortReplications", "NegativeControlTests", NegativeControlTests)
d = insert_table_after("IndividualPredictions", "CalibrationBins", CalibrationBins)

# ---------------------------------------------------------------------------
# 2. REWIRE EXISTING TABLES
# ---------------------------------------------------------------------------
# 2.1 GenomicVariants: delete raw IsTrueCausalCandidate; add VariantClassIsRare lookup; rewrite IsCausalCandidate
drop("GenomicVariants", "IsTrueCausalCandidate")
insert_after("GenomicVariants","VariantTypeLabel",
    F("VariantClassIsRare","boolean","lookup","Whether this variant's class is a rare-variant class (observed type property).",
      "=INDEX(VariantTypes!{{IsRareVariantClass}}, MATCH({{VariantType}}, VariantTypes!{{VariantTypeId}}, 0))"))
replace_or_add("GenomicVariants",
    F("IsCausalCandidate","boolean","calculated","Derived: rare (by frequency or class) AND shows allele-specific expression.",
      "=IF(AND(OR({{IsRareVariant}}, {{VariantClassIsRare}}), {{HasAlleleSpecificExpression}}), TRUE(), FALSE())"))

# 2.2 CausalMechanisms: delete 3 raw answers; add the derived evidence/replication/control chain
drop("CausalMechanisms", "CausalConfidence", "IsExperimentallyFalsifiable", "HasSpuriousCorrelationFlag")
cm_adds = [
    F("CountQualifiedEvidence","integer","aggregation","Count of qualified evidence items supporting this mechanism.",
      "=COUNTIFS(EvidenceItems!{{CausalMechanism}}, {{CausalMechanismId}}, EvidenceItems!{{IsQualifiedEvidence}}, TRUE())"),
    F("CountModalitiesSupporting","integer","aggregation","Count of qualified cross-modality evidence items (multi-omic corroboration).",
      "=COUNTIFS(EvidenceItems!{{CausalMechanism}}, {{CausalMechanismId}}, EvidenceItems!{{IsCrossModality}}, TRUE(), EvidenceItems!{{IsQualifiedEvidence}}, TRUE())"),
    F("CountInterventionTargets","integer","aggregation","Count of perturbable intervention targets for this mechanism (falsifiability requires >=1).",
      "=COUNTIFS(InterventionTargets!{{CausalMechanism}}, {{CausalMechanismId}})"),
    F("IsExperimentallyFalsifiable","boolean","calculated","Derived: a measurable qualified effect exists AND a named intervention can perturb it.",
      "=IF(AND({{CountInterventionTargets}} >= 1, {{CountQualifiedEvidence}} >= 1), TRUE(), FALSE())"),
    F("CountReplications","integer","aggregation","Total cross-cohort re-tests of this mechanism.",
      "=COUNTIFS(CohortReplications!{{CausalMechanism}}, {{CausalMechanismId}})"),
    F("CountConcordantReplications","integer","aggregation","Re-tests reproducing the predicted sign at significance.",
      "=COUNTIFS(CohortReplications!{{CausalMechanism}}, {{CausalMechanismId}}, CohortReplications!{{ReplicatedAtNominalSig}}, TRUE())"),
    F("CountCrossAncestryConcordant","integer","aggregation","Concordant re-tests that ran in a DIFFERENT ancestry (the transportability measurement).",
      "=COUNTIFS(CohortReplications!{{CausalMechanism}}, {{CausalMechanismId}}, CohortReplications!{{IsCrossAncestryConcordant}}, TRUE())"),
    F("ReplicationFraction","number","calculated","Derived: fraction of re-tests that were concordant (guarded division).",
      "=IF({{CountReplications}} > 0, {{CountConcordantReplications}} / {{CountReplications}}, 0)"),
    F("ReplicatesAcrossCohorts","boolean","calculated","Derived: >=2 independent re-tests and >=2 concordant.",
      "=IF(AND({{CountReplications}} >= 2, {{CountConcordantReplications}} >= 2), TRUE(), FALSE())"),
    F("CountNegControlTests","integer","aggregation","Negative-control tests run on this mechanism.",
      "=COUNTIFS(NegativeControlTests!{{CausalMechanism}}, {{CausalMechanismId}})"),
    F("CountNegControlSurvived","integer","aggregation","Negative-control tests the mechanism survived (collapsed under the null).",
      "=COUNTIFS(NegativeControlTests!{{CausalMechanism}}, {{CausalMechanismId}}, NegativeControlTests!{{IsSurvived}}, TRUE())"),
    F("SurvivesNegativeControls","boolean","calculated","Derived: at least one control run AND all of them survived.",
      "=IF(AND({{CountNegControlTests}} >= 1, {{CountNegControlSurvived}} = {{CountNegControlTests}}), TRUE(), FALSE())"),
    F("IsSpuriousDerived","boolean","calculated","Derived: spurious unless replicated, survives controls, has >=2 modalities, and is not purely pleiotropic.",
      "=IF(OR(NOT({{ReplicatesAcrossCohorts}}), NOT({{SurvivesNegativeControls}}), {{CountModalitiesSupporting}} < 2, {{HasPleiotropy}}), TRUE(), FALSE())"),
    F("CausalConfidence","number","calculated","Derived bounded blend of qualified-evidence count, modality breadth, replication rate, and control survival.",
      "=IF(((0.30 * IF({{CountQualifiedEvidence}} >= 4, 1, {{CountQualifiedEvidence}} / 4)) + (0.20 * IF({{CountModalitiesSupporting}} >= 3, 1, {{CountModalitiesSupporting}} / 3)) + (0.30 * {{ReplicationFraction}}) + (0.20 * IF({{SurvivesNegativeControls}}, 1, 0))) > 1, 1, (0.30 * IF({{CountQualifiedEvidence}} >= 4, 1, {{CountQualifiedEvidence}} / 4)) + (0.20 * IF({{CountModalitiesSupporting}} >= 3, 1, {{CountModalitiesSupporting}} / 3)) + (0.30 * {{ReplicationFraction}}) + (0.20 * IF({{SurvivesNegativeControls}}, 1, 0)))"),
    F("VariantIsCausalCandidate","boolean","lookup","Whether the linked variant is itself a derived causal candidate (empty-guarded).",
      '=IF({{GenomicVariant}} = "", FALSE(), INDEX(GenomicVariants!{{IsCausalCandidate}}, MATCH({{GenomicVariant}}, GenomicVariants!{{GenomicVariantId}}, 0)))'),
    F("IsCausalArchitectureNode","boolean","calculated","Derived: a confirmed causal edge - confident, falsifiable, non-spurious, and grounded in a candidate variant or a real exposure.",
      '=IF(AND({{CausalConfidence}} >= 0.7, {{IsExperimentallyFalsifiable}}, NOT({{IsSpuriousDerived}}), OR({{VariantIsCausalCandidate}}, {{EnvironmentalExposure}} <> "")), TRUE(), FALSE())'),
    F("IsAncestryTransportable","boolean","calculated","Derived: a confirmed node whose effect replicated in >=1 different ancestry (measured invariance).",
      "=IF(AND({{IsCausalArchitectureNode}}, {{CountCrossAncestryConcordant}} >= 1), TRUE(), FALSE())"),
]
# old IsCausalArchitectureNode existed as raw-confidence-based calc; remove then re-add cleanly
drop("CausalMechanisms","IsCausalArchitectureNode")
# place new fields before the InterventionTargets relationship (keep relationships last)
rel_idx = next((i for i,f in enumerate(schema("CausalMechanisms")) if f["type"]=="relationship"), len(schema("CausalMechanisms")))
for off,fld in enumerate(cm_adds):
    schema("CausalMechanisms").insert(rel_idx+off, fld)

# 2.3 Individuals: add confirmed-node aggregations
ind_adds = [
    F("CountConfirmedCausalNodes","integer","aggregation","Count of this individual's confirmed causal-architecture nodes.",
      "=COUNTIFS(CausalMechanisms!{{Individual}}, {{IndividualId}}, CausalMechanisms!{{IsCausalArchitectureNode}}, TRUE())"),
    F("SumConfirmedCausalConfidence","number","aggregation","Summed confidence of this individual's confirmed causal nodes (derived causal mass).",
      "=SUMIFS(CausalMechanisms!{{CausalConfidence}}, CausalMechanisms!{{Individual}}, {{IndividualId}}, CausalMechanisms!{{IsCausalArchitectureNode}}, TRUE())"),
    F("CountCrossAncestryConfirmedNodes","integer","aggregation","Confirmed nodes that also showed cross-ancestry replication.",
      "=COUNTIFS(CausalMechanisms!{{Individual}}, {{IndividualId}}, CausalMechanisms!{{IsAncestryTransportable}}, TRUE())"),
]
rel_idx = next((i for i,f in enumerate(schema("Individuals")) if f["type"]=="relationship"), len(schema("Individuals")))
for off,fld in enumerate(ind_adds):
    schema("Individuals").insert(rel_idx+off, fld)

# 2.4 IndividualPredictions: delete 3 raw answers; add the derived prediction + gate + keystone chain
drop("IndividualPredictions","PredictedValue","CalibratedUncertainty","HasSpuriousCorrelationFlag")
# old IsHighConfidencePrediction & PatientStratificationTier exist as calc; we will replace their formulas
ip_adds = [
    F("IndividualCausalMass","number","lookup","Summed confirmed causal confidence for this individual (empty-guarded).",
      '=IF({{Individual}} = "", 0, INDEX(Individuals!{{SumConfirmedCausalConfidence}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))'),
    F("IndividualConfirmedNodeCount","integer","lookup","Count of this individual's confirmed causal nodes (empty-guarded).",
      '=IF({{Individual}} = "", 0, INDEX(Individuals!{{CountConfirmedCausalNodes}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))'),
    F("IndividualCrossAncestryNodeCount","integer","lookup","Count of this individual's cross-ancestry-replicated confirmed nodes (empty-guarded).",
      '=IF({{Individual}} = "", 0, INDEX(Individuals!{{CountCrossAncestryConfirmedNodes}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))'),
    F("IndividualHasCrypticRelatedness","boolean","lookup","Whether this individual carries a cryptic-relatedness leakage flag (empty-guarded).",
      '=IF({{Individual}} = "", FALSE(), INDEX(Individuals!{{HasCrypticRelatednessFlag}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))'),
    F("PredictedValue","number","calculated","Derived risk magnitude (0-10), a monotone function of validated causal mass only - rides mechanism, not ancestry correlation.",
      "=IF(((2 * {{IndividualCausalMass}}) + (1.5 * {{IndividualConfirmedNodeCount}})) > 10, 10, (2 * {{IndividualCausalMass}}) + (1.5 * {{IndividualConfirmedNodeCount}}))"),
    F("CountBins","integer","aggregation","Total reliability bins for this prediction.",
      "=COUNTIFS(CalibrationBins!{{IndividualPrediction}}, {{IndividualPredictionId}})"),
    F("CountWellCalibratedBins","integer","aggregation","Bins passing coverage and accuracy.",
      "=COUNTIFS(CalibrationBins!{{IndividualPrediction}}, {{IndividualPredictionId}}, CalibrationBins!{{IsWellCalibratedBin}}, TRUE())"),
    F("SumBinAbsError","number","aggregation","Summed reliability gap across this prediction's bins.",
      "=SUMIFS(CalibrationBins!{{BinAbsError}}, CalibrationBins!{{IndividualPrediction}}, {{IndividualPredictionId}})"),
    F("MeanBinAbsError","number","calculated","Derived mean reliability gap; defaults to 1 (worst) when no bins exist.",
      "=IF({{CountBins}} > 0, {{SumBinAbsError}} / {{CountBins}}, 1)"),
    F("WellCalibratedFraction","number","calculated","Derived fraction of trustworthy bins (guarded division).",
      "=IF({{CountBins}} > 0, {{CountWellCalibratedBins}} / {{CountBins}}, 0)"),
    F("CalibratedUncertainty","number","calculated","Derived reliability (HIGH = trustworthy): (1 - mean gap) scaled by well-covered-bin fraction; 0 for uncovered predictions.",
      "=IF((1 - {{MeanBinAbsError}}) < 0, 0, (1 - {{MeanBinAbsError}})) * {{WellCalibratedFraction}}"),
    F("RestsOnConfirmedMechanism","boolean","calculated","Derived: grounded in >=1 validated mechanism.",
      "=IF({{IndividualConfirmedNodeCount}} >= 1, TRUE(), FALSE())"),
    F("HasSpuriousCorrelationFlag","boolean","calculated","Derived: spurious if no validated mechanism OR cryptic-relatedness leakage.",
      "=IF(OR(NOT({{RestsOnConfirmedMechanism}}), {{IndividualHasCrypticRelatedness}}), TRUE(), FALSE())"),
    F("IsFalsifiabilityBacked","boolean","calculated","Derived: inherits falsifiability - every confirmed node required IsExperimentallyFalsifiable.",
      "=IF({{IndividualConfirmedNodeCount}} >= 1, TRUE(), FALSE())"),
    F("IsTransportableToAbsentAncestry","boolean","calculated","Derived: a holdout individual is transportable only with >=1 cross-ancestry-replicated node and no spurious flag.",
      "=IF(AND({{IsAncestryHoldout}}, {{IndividualCrossAncestryNodeCount}} >= 1, NOT({{HasSpuriousCorrelationFlag}})), TRUE(), FALSE())"),
    F("IsAncestryTransportSafe","boolean","calculated","Derived: holdout requires measured transport; in-training is vacuously safe.",
      "=IF({{IsAncestryHoldout}}, {{IsTransportableToAbsentAncestry}}, TRUE())"),
]
rel_idx = next((i for i,f in enumerate(schema("IndividualPredictions")) if f["type"]=="relationship"), len(schema("IndividualPredictions")))
# Drop the existing IsHighConfidencePrediction & PatientStratificationTier so we can re-add in chain order
drop("IndividualPredictions","IsHighConfidencePrediction","PatientStratificationTier")
rel_idx = next((i for i,f in enumerate(schema("IndividualPredictions")) if f["type"]=="relationship"), len(schema("IndividualPredictions")))
for off,fld in enumerate(ip_adds):
    schema("IndividualPredictions").insert(rel_idx+off, fld)
# Now append the gate + keystone fields (after the adds, still before any relationships)
gate_fields = [
    F("IsHighConfidencePrediction","boolean","calculated","Derived: calibrated AND not spurious.",
      "=IF(AND({{CalibratedUncertainty}} >= 0.7, NOT({{HasSpuriousCorrelationFlag}})), TRUE(), FALSE())"),
    F("PatientStratificationTier","string","calculated","Derived risk tier from the derived PredictedValue.",
      '=IF({{PredictedValue}} >= 7, "High-Risk Pathway", IF({{PredictedValue}} >= 4, "Moderate-Risk Pathway", "Low-Risk Pathway"))'),
    F("IsClinicallyActionable","boolean","calculated","KEYSTONE: TRUE only when the prediction is high-confidence (calibrated + not spurious), falsifiability-backed, ancestry-transport-safe, and rests on a non-null derived magnitude.",
      "=IF(AND({{IsHighConfidencePrediction}}, {{IsFalsifiabilityBacked}}, {{IsAncestryTransportSafe}}, {{PredictedValue}} > 0), TRUE(), FALSE())"),
]
rel_idx = next((i for i,f in enumerate(schema("IndividualPredictions")) if f["type"]=="relationship"), len(schema("IndividualPredictions")))
for off,fld in enumerate(gate_fields):
    schema("IndividualPredictions").insert(rel_idx+off, fld)

# Add reverse-relationship placeholders so parents list new children (optional but tidy)
def add_rel(t, name, target, desc):
    if name in fieldnames(t): return
    schema(t).append(F(name,"string","relationship",desc,related=target))
add_rel("CausalMechanisms","EvidenceItems","EvidenceItems","Evidence items supporting this mechanism.")
add_rel("CausalMechanisms","CohortReplications","CohortReplications","Cross-cohort replications of this mechanism.")
add_rel("CausalMechanisms","NegativeControlTests","NegativeControlTests","Negative-control tests on this mechanism.")
add_rel("OmicsAssays","EvidenceItems","EvidenceItems","Evidence items measured in this assay.")
add_rel("FederatedDatasets","CohortReplications","CohortReplications","Replications run in this cohort node.")
add_rel("IndividualPredictions","CalibrationBins","CalibrationBins","Reliability bins for this prediction.")

# update meta
d["_meta"]["_CMCC_Summary"] = "Loop 0: solve-by-inference rulebook for the Causal Autoimmune Architecture Platform. The keystone IndividualPredictions.IsClinicallyActionable is derived through a calculated-field DAG bottoming out only in raw observations."
d["_meta"]["_conversion_metadata"]["table_count"] = len([k for k in d if k not in ['$schema','Name','Description','_meta']])
d["_meta"]["_conversion_metadata"]["tool_version"] = "loop-0-solve-by-inference-2026-06-19"

json.dump(d, open(RB,"w"), indent=2)
print("WROTE", RB)
print("tables:", [k for k in d if k not in ['$schema','Name','Description','_meta']])
