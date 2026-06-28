# Canonical Derivation Spec — IsClinicallyActionable

> Produced by a multi-pass design workflow (3 independent designs + adversarial judges + synthesis).
> This is the implementation contract for converting the rulebook from data-entry to solve-by-inference.

Verified. The live rulebook uses only IF/AND/OR/NOT/TRUE/FALSE, single-key INDEX-MATCH, single-FK COUNTIFS, `&` concat, and basic arithmetic. SUMIFS, AVERAGEIFS, MIN, MAX, ABS, ROUND, POWER, IFERROR have **zero** precedent. The dialect spec *allows* them, but the final design must minimize reliance on the unproven ones and provide nested-IF fallbacks. I now have everything needed.

---

# FINAL CANONICAL SPEC — `IndividualPredictions.IsClinicallyActionable` as a strictly derived keystone

## 0. Decision: backbone + grafts

**Backbone chosen: `falsifiability-first` (Angle 3, 30/40).** It has the cleanest mechanism→individual→prediction bridge (it routes per-individual causal mass through `CausalMechanisms.Individual` and `IndividualPredictions.Individual`, both verified-present FKs, so it needs **no** many-to-many link table and **no** array formulas), the best anti-spurious layer (replication + negative-control + cross-modality), and the strongest calibration sub-chain.

**Grafts applied:**
- **From Angle 1 (evidence-first):** the `EvidenceItems.SupportScore` shape — derive per-item credibility from raw `EffectSize`/`StandardError` → `ZStat`, and the confound-control gate from raw analysis-provenance flags (`IsAdjustedForAncestryPCs`/`IsAdjustedForBatch`). This **closes Angle 3's "shallow neg-control" leak**: negative-control survival is now *derived* from a measured permutation effect size vs. threshold, not a typed PASS/FAIL bit.
- **From Angle 1 & Angle 3 (the universally-praised idea):** the `CalibrationBins → BinAbsError → CalibratedUncertainty` measured-reliability pattern. Grafted **without** any composite-string-key MATCH and **without** range-band array-MATCH (both verdicts flagged these as the #1 transpilation hazard). Calibration bins are FK children of `IndividualPredictions` directly, so the join is a plain single-FK `COUNTIFS/SUMIFS/AVERAGEIFS` — exactly the proven pattern.
- **The fix BOTH adversaries demanded for the marquee requirement:** ancestry transport must be a **measured cross-ancestry-invariance fact**, not "has a confirmed node." We add `CohortReplications.ReplicationAncestryLabel` (raw observation) + an `IsDifferentAncestryReplication` derived bit, and require `CountCrossAncestryConcordant >= 1` for transportability. This kills the tautology Angle 3 was docked for.
- **Replication/neg-control "smuggled conclusion" fix (Angle 1 leak + Angle 3 dock):** `ReplicatedAtNominalSig` / `IsSurvived` become **derived** from raw measured statistics (`ReplicationPValue`, `PermutationEffectSize`), not typed verdicts.

**Expressibility guardrail (both verdicts' worst axis):** every formula below uses ONLY functions either already in the live rulebook (IF/AND/OR/NOT/INDEX-MATCH single-key/COUNTIFS single-or-multi-criterion/`&`/arithmetic) **or** the four aggregation/math functions the dialect explicitly names. We use `SUMIFS`/`AVERAGEIFS`/`MIN`/`MAX`/`ABS` where the dialect permits but provide the exact nested-IF / division-guard rewrite for any that the transpiler rejects (noted inline as **[fallback]**). **No `IFERROR`, no `ROUND` in gating logic, no composite-string MATCH, no array/CSE MATCH, no range-band MATCH anywhere.** Division-by-zero is guarded with `IF(denom>0, …, default)` (the live rulebook's proven pattern, see `Individuals.RareVariantBurdenScore`).

---

## 1. NEW TABLES (4) — full field lists

All four are clean 1-to-many leaf children (no cycles, no many-to-many). FK columns are raw strings matching the parent's `*Id`, exactly like every existing FK.

### 1.1 `EvidenceItems` — one observed support signal for a mechanism, from one assay
Parent FKs: `CausalMechanism`, `OmicsAssay`. *Justification:* mechanism confidence/falsifiability must be **aggregations over child rows**; there is nowhere today to put per-modality evidence. Replaces the typed `CausalConfidence`.

| Field | datatype | type | formula / note |
|---|---|---|---|
| EvidenceItemId | string | raw | PK |
| EvidenceLabel | string | raw | label |
| CausalMechanism | string | raw | FK → CausalMechanisms |
| OmicsAssay | string | raw | FK → OmicsAssays |
| **EffectSize** | number | raw | OBSERVATION: measured effect magnitude |
| **StandardError** | number | raw | OBSERVATION: measured SE of the effect |
| **IsCrossModality** | boolean | raw | OBSERVATION: signal from a different omics modality than the mechanism's primary |
| **IsNegativeControlArm** | boolean | raw | OBSERVATION: this row is a measured control arm (not support) |
| **IsAdjustedForAncestryPCs** | boolean | raw | OBSERVATION: analysis adjusted for ancestry PCs |
| **IsAdjustedForBatch** | boolean | raw | OBSERVATION: analysis adjusted for batch |
| Name | string | calc | `={{EvidenceLabel}}` |
| AssayIsHighQuality | boolean | lookup | `=INDEX(OmicsAssays!{{IsHighQualityAssay}}, MATCH({{OmicsAssay}}, OmicsAssays!{{OmicsAssayId}}, 0))` |
| AssayMeasurementError | number | lookup | `=INDEX(OmicsAssays!{{MeasurementErrorScore}}, MATCH({{OmicsAssay}}, OmicsAssays!{{OmicsAssayId}}, 0))` |
| ZStat | number | calc | `=IF({{StandardError}} > 0, ABS({{EffectSize}}) / {{StandardError}}, 0)` — **[fallback if ABS rejected]** require raw `EffectSize` already absolute, use `{{EffectSize}}/{{StandardError}}` |
| IsConfoundControlled | boolean | calc | `=IF(AND({{IsAdjustedForAncestryPCs}}, {{IsAdjustedForBatch}}), TRUE(), FALSE())` |
| IsQualifiedEvidence | boolean | calc | `=IF(AND({{AssayIsHighQuality}}, NOT({{IsNegativeControlArm}}), {{ZStat}} >= 2, {{IsConfoundControlled}}), TRUE(), FALSE())` — clean assay, real support arm, signal-to-noise ≥2, confound-controlled |

### 1.2 `CohortReplications` — one re-test of the mechanism in a different federated cohort
Parent FKs: `CausalMechanism`, `FederatedDataset`. *Justification:* replication count, sign-concordance, **and cross-ancestry concordance** are `COUNTIFS` over these rows — the anti-spurious + ancestry-transport core.

| Field | datatype | type | formula / note |
|---|---|---|---|
| CohortReplicationId | string | raw | PK |
| ReplicationLabel | string | raw | label |
| CausalMechanism | string | raw | FK → CausalMechanisms |
| FederatedDataset | string | raw | FK → FederatedDatasets |
| **ReplicationEffectSign** | integer | raw | OBSERVATION: sign of re-estimated effect (+1 / -1 / 0) |
| **ReplicationPValue** | number | raw | OBSERVATION: re-test p-value |
| **ReplicationAncestryLabel** | string | raw | OBSERVATION: ancestry of the cohort the re-test ran in |
| Name | string | calc | `={{ReplicationLabel}}` |
| ReplicatedAtNominalSig | boolean | calc | `=IF(AND({{ReplicationPValue}} <= 0.05, {{ReplicationEffectSign}} = 1), TRUE(), FALSE())` — **DERIVED** verdict, not typed (closes the smuggled-conclusion leak) |
| MechanismPrimaryAncestry | string | lookup | `=INDEX(CausalMechanisms!{{IndividualAncestryLabel}}, MATCH({{CausalMechanism}}, CausalMechanisms!{{CausalMechanismId}}, 0))` |
| IsDifferentAncestryReplication | boolean | calc | `=IF({{ReplicationAncestryLabel}} <> {{MechanismPrimaryAncestry}}, TRUE(), FALSE())` |
| IsCrossAncestryConcordant | boolean | calc | `=IF(AND({{ReplicatedAtNominalSig}}, {{IsDifferentAncestryReplication}}), TRUE(), FALSE())` — replicated **and** in a different ancestry: the transportability atom |

### 1.3 `NegativeControlTests` — one stratification / permutation control on a mechanism
Parent FK: `CausalMechanism`. *Justification:* "survives a negative control" must be auditable rows, and survival is **derived** from a measured permutation effect size, not a typed bit (closes the Angle-3 dock).

| Field | datatype | type | formula / note |
|---|---|---|---|
| NegativeControlTestId | string | raw | PK |
| ControlLabel | string | raw | label |
| CausalMechanism | string | raw | FK → CausalMechanisms |
| **TestKind** | string | raw | OBSERVATION: "ancestry-permutation" / "batch-stratified" / "negative-control" |
| **PermutationEffectSize** | number | raw | OBSERVATION: measured effect under the null/permuted control |
| **NullThreshold** | number | raw | OBSERVATION: pre-registered null band half-width |
| Name | string | calc | `={{ControlLabel}}` |
| IsSurvived | boolean | calc | `=IF(ABS({{PermutationEffectSize}}) <= {{NullThreshold}}, TRUE(), FALSE())` — **DERIVED**: a true causal signal collapses under the control. **[fallback if ABS rejected]** `=IF(AND({{PermutationEffectSize}} <= {{NullThreshold}}, {{PermutationEffectSize}} >= -{{NullThreshold}}), TRUE(), FALSE())` |

### 1.4 `CalibrationBins` — per-prediction reliability-curve bins
Parent FK: `IndividualPrediction`. *Justification:* calibration is coverage of observed outcomes across bins — a `SUMIFS/AVERAGEIFS` over child rows of the specific prediction. Single-FK join only; no composite key.

| Field | datatype | type | formula / note |
|---|---|---|---|
| CalibrationBinId | string | raw | PK |
| BinLabel | string | raw | label |
| IndividualPrediction | string | raw | FK → IndividualPredictions |
| **PredictedProbabilityBand** | number | raw | OBSERVATION: bin's predicted-probability midpoint (0–1) |
| **ObservedEventRate** | number | raw | OBSERVATION: empirical event rate among held-out cases in this band, **matched to this prediction's individual's ancestry × disease** at data-authoring time |
| **CoverageCount** | integer | raw | OBSERVATION: # held-out outcomes in this band for this ancestry × disease |
| Name | string | calc | `={{BinLabel}}` |
| BinAbsError | number | calc | `=ABS({{PredictedProbabilityBand}} - {{ObservedEventRate}})` — **[fallback if ABS rejected]** `=IF({{PredictedProbabilityBand}} >= {{ObservedEventRate}}, {{PredictedProbabilityBand}} - {{ObservedEventRate}}, {{ObservedEventRate}} - {{PredictedProbabilityBand}})` |
| IsWellCalibratedBin | boolean | calc | `=IF(AND({{CoverageCount}} >= 20, {{BinAbsError}} <= 0.1), TRUE(), FALSE())` |

> **Ancestry-equity design note:** Because `CalibrationBins` are authored per-prediction *and seeded from the held-out coverage of that individual's own ancestry × disease*, an absent-from-training ancestry simply has **no bins with `CoverageCount >= 20`** → `CountWellCalibratedBins = 0` → `CalibratedUncertainty = 0`. The IFERROR-to-worst-case behavior from Angle 1 is achieved *structurally* (no matching rows ⇒ aggregations return 0) with no error-swallowing and no composite key. This is the graft, de-risked.

---

## 2. EVERY FIELD TO CHANGE / ADD, TABLE BY TABLE

### 2.1 `GenomicVariants`
| Table | Field | datatype | type | formula | why |
|---|---|---|---|---|---|
| GenomicVariants | ~~IsTrueCausalCandidate~~ | boolean | **DELETE (raw conclusion)** | — | typed causal verdict; replaced by ASE + class observations |
| GenomicVariants | VariantClassIsRare | boolean | **lookup (NEW)** | `=INDEX(VariantTypes!{{IsRareVariantClass}}, MATCH({{VariantType}}, VariantTypes!{{VariantTypeId}}, 0))` | rarity-by-class is an observed type property pulled by FK |
| GenomicVariants | IsCausalCandidate | boolean | **calc (REWRITE)** | `=IF(AND(OR({{IsRareVariant}}, {{VariantClassIsRare}}), {{HasAlleleSpecificExpression}}), TRUE(), FALSE())` | **CONVERTED:** old formula ANDed in raw `IsTrueCausalCandidate`; now bottoms out in allele freq + observed ASE. Replaces raw input `IsTrueCausalCandidate` with raw observations `AlleleFrequency`, `HasAlleleSpecificExpression`, `VariantTypes.IsRareVariantClass` |

### 2.2 `CausalMechanisms`
| Table | Field | datatype | type | formula | why |
|---|---|---|---|---|---|
| CausalMechanisms | ~~CausalConfidence~~ | number | **DELETE (raw)** | — | reborn as E1 aggregation blend |
| CausalMechanisms | ~~IsExperimentallyFalsifiable~~ | boolean | **DELETE (raw)** | — | reborn as C2 |
| CausalMechanisms | ~~HasSpuriousCorrelationFlag~~ | boolean | **DELETE (raw)** | — | reborn as D8 |
| CausalMechanisms | CountQualifiedEvidence | integer | **agg (NEW)** | `=COUNTIFS(EvidenceItems!{{CausalMechanism}}, {{CausalMechanismId}}, EvidenceItems!{{IsQualifiedEvidence}}, TRUE())` | evidence numerator = count of child rows |
| CausalMechanisms | CountModalitiesSupporting | integer | **agg (NEW)** | `=COUNTIFS(EvidenceItems!{{CausalMechanism}}, {{CausalMechanismId}}, EvidenceItems!{{IsCrossModality}}, TRUE(), EvidenceItems!{{IsQualifiedEvidence}}, TRUE())` | multi-omic corroboration, counted |
| CausalMechanisms | SumEvidenceStrength | number | **agg (NEW)** | `=SUMIFS(EvidenceItems!{{ZStat}}, EvidenceItems!{{CausalMechanism}}, {{CausalMechanismId}}, EvidenceItems!{{IsQualifiedEvidence}}, TRUE())` — **[fallback if SUMIFS rejected]** drop this field and use `CountQualifiedEvidence` only in E1 | summed signal-to-noise over qualified evidence |
| CausalMechanisms | CountInterventionTargets | integer | **agg (NEW)** | `=COUNTIFS(InterventionTargets!{{CausalMechanism}}, {{CausalMechanismId}})` | falsifiable only if a perturbable target exists (FK verified present) |
| CausalMechanisms | IsExperimentallyFalsifiable | boolean | **calc (NEW, same name as deleted raw)** | `=IF(AND({{CountInterventionTargets}} >= 1, {{CountQualifiedEvidence}} >= 1), TRUE(), FALSE())` | **CONVERTED:** falsifiable = measurable qualified effect + a named intervention to perturb |
| CausalMechanisms | CountReplications | integer | **agg (NEW)** | `=COUNTIFS(CohortReplications!{{CausalMechanism}}, {{CausalMechanismId}})` | total cross-cohort re-tests |
| CausalMechanisms | CountConcordantReplications | integer | **agg (NEW)** | `=COUNTIFS(CohortReplications!{{CausalMechanism}}, {{CausalMechanismId}}, CohortReplications!{{ReplicatedAtNominalSig}}, TRUE())` | re-tests reproducing predicted sign at sig |
| CausalMechanisms | CountCrossAncestryConcordant | integer | **agg (NEW)** | `=COUNTIFS(CohortReplications!{{CausalMechanism}}, {{CausalMechanismId}}, CohortReplications!{{IsCrossAncestryConcordant}}, TRUE())` | **the transportability measurement** — replicated in a *different* ancestry |
| CausalMechanisms | ReplicationFraction | number | **calc (NEW)** | `=IF({{CountReplications}} > 0, {{CountConcordantReplications}} / {{CountReplications}}, 0)` | replication rate (guarded division, proven pattern) |
| CausalMechanisms | ReplicatesAcrossCohorts | boolean | **calc (NEW)** | `=IF(AND({{CountReplications}} >= 2, {{CountConcordantReplications}} >= 2), TRUE(), FALSE())` | ≥2 independent nodes, ≥2 concordant |
| CausalMechanisms | CountNegControlTests | integer | **agg (NEW)** | `=COUNTIFS(NegativeControlTests!{{CausalMechanism}}, {{CausalMechanismId}})` | controls actually run |
| CausalMechanisms | CountNegControlSurvived | integer | **agg (NEW)** | `=COUNTIFS(NegativeControlTests!{{CausalMechanism}}, {{CausalMechanismId}}, NegativeControlTests!{{IsSurvived}}, TRUE())` | controls passed |
| CausalMechanisms | SurvivesNegativeControls | boolean | **calc (NEW)** | `=IF(AND({{CountNegControlTests}} >= 1, {{CountNegControlSurvived}} = {{CountNegControlTests}}), TRUE(), FALSE())` | ≥1 run AND all passed |
| CausalMechanisms | IsSpuriousDerived | boolean | **calc (NEW)** | `=IF(OR(NOT({{ReplicatesAcrossCohorts}}), NOT({{SurvivesNegativeControls}}), {{CountModalitiesSupporting}} < 2, {{HasPleiotropy}}), TRUE(), FALSE())` | **CONVERTED replacement for the deleted spurious flag** — spurious unless replicated + survives controls + ≥2 modalities + not purely pleiotropic (`HasPleiotropy` kept raw, an observed property) |
| CausalMechanisms | CausalConfidence | number | **calc (NEW, same name as deleted raw)** | `=MIN(1, (0.30 * MIN(1, {{CountQualifiedEvidence}} / 4)) + (0.20 * MIN(1, {{CountModalitiesSupporting}} / 3)) + (0.30 * {{ReplicationFraction}}) + (0.20 * IF({{SurvivesNegativeControls}}, 1, 0)))` — **[fallback if MIN rejected]** define `CapEvidence = IF({{CountQualifiedEvidence}}>=4,1,{{CountQualifiedEvidence}}/4)` etc. as separate calc fields, then sum, then `IF(sum>1,1,sum)` | **CONVERTED:** bounded weighted blend of aggregated observations; no human number survives |
| CausalMechanisms | VariantIsCausalCandidate | boolean | **lookup (NEW)** | `=IF({{GenomicVariant}} = "", FALSE(), INDEX(GenomicVariants!{{IsCausalCandidate}}, MATCH({{GenomicVariant}}, GenomicVariants!{{GenomicVariantId}}, 0)))` | pulls derived variant candidacy (A3) across FK, empty-guarded (proven pattern) |
| CausalMechanisms | IsCausalArchitectureNode | boolean | **calc (REWRITE)** | `=IF(AND({{CausalConfidence}} >= 0.7, {{IsExperimentallyFalsifiable}}, NOT({{IsSpuriousDerived}}), OR({{VariantIsCausalCandidate}}, {{EnvironmentalExposure}} <> "")), TRUE(), FALSE())` | **REWRITTEN** to read only derived inputs: confident + falsifiable + non-spurious + grounded in a candidate variant or a real exposure |

### 2.3 `Individuals`
| Table | Field | datatype | type | formula | why |
|---|---|---|---|---|---|
| Individuals | CountConfirmedCausalNodes | integer | **agg (NEW)** | `=COUNTIFS(CausalMechanisms!{{Individual}}, {{IndividualId}}, CausalMechanisms!{{IsCausalArchitectureNode}}, TRUE())` | validated mechanisms this person carries (FK `CausalMechanisms.Individual` verified) |
| Individuals | SumConfirmedCausalConfidence | number | **agg (NEW)** | `=SUMIFS(CausalMechanisms!{{CausalConfidence}}, CausalMechanisms!{{Individual}}, {{IndividualId}}, CausalMechanisms!{{IsCausalArchitectureNode}}, TRUE())` — **[fallback if SUMIFS rejected]** use `CountConfirmedCausalNodes` alone in G3 | derived causal mass (mechanism-based, transportable risk basis) |
| Individuals | CountCrossAncestryConfirmedNodes | integer | **agg (NEW)** | `=COUNTIFS(CausalMechanisms!{{Individual}}, {{IndividualId}}, CausalMechanisms!{{IsCausalArchitectureNode}}, TRUE(), CausalMechanisms!{{IsAncestryTransportable}}, TRUE())` | confirmed nodes that ALSO showed cross-ancestry replication — see CausalMechanisms.IsAncestryTransportable below |

> **Add one more field to `CausalMechanisms`** (placed here for chain clarity), needed by `CountCrossAncestryConfirmedNodes`:

| Table | Field | datatype | type | formula | why |
|---|---|---|---|---|---|
| CausalMechanisms | IsAncestryTransportable | boolean | **calc (NEW)** | `=IF(AND({{IsCausalArchitectureNode}}, {{CountCrossAncestryConcordant}} >= 1), TRUE(), FALSE())` | a confirmed node whose effect **replicated in ≥1 different ancestry** — the measured invariance fact both adversaries demanded |

### 2.4 `IndividualPredictions`
| Table | Field | datatype | type | formula | why |
|---|---|---|---|---|---|
| IndividualPredictions | ~~PredictedValue~~ | number | **DELETE (raw)** | — | reborn as G3 |
| IndividualPredictions | ~~CalibratedUncertainty~~ | number | **DELETE (raw)** | — | reborn as H7 |
| IndividualPredictions | ~~HasSpuriousCorrelationFlag~~ | boolean | **DELETE (raw)** | — | reborn as I4 |
| IndividualPredictions | IndividualCausalMass | number | **lookup (NEW)** | `=IF({{Individual}} = "", 0, INDEX(Individuals!{{SumConfirmedCausalConfidence}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))` | pulls F2, empty-guarded |
| IndividualPredictions | IndividualConfirmedNodeCount | integer | **lookup (NEW)** | `=IF({{Individual}} = "", 0, INDEX(Individuals!{{CountConfirmedCausalNodes}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))` | pulls F1, empty-guarded |
| IndividualPredictions | IndividualCrossAncestryNodeCount | integer | **lookup (NEW)** | `=IF({{Individual}} = "", 0, INDEX(Individuals!{{CountCrossAncestryConfirmedNodes}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))` | pulls cross-ancestry-confirmed count, empty-guarded |
| IndividualPredictions | IndividualHasCrypticRelatedness | boolean | **lookup (NEW)** | `=IF({{Individual}} = "", FALSE(), INDEX(Individuals!{{HasCrypticRelatednessFlag}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))` | observed leakage confounder, empty-guarded |
| IndividualPredictions | PredictedValue | number | **calc (NEW, same name as deleted raw)** | `=MIN(10, (2 * {{IndividualCausalMass}}) + (1.5 * {{IndividualConfirmedNodeCount}}))` — **[fallback if MIN rejected]** `=IF((2*{{IndividualCausalMass}}+1.5*{{IndividualConfirmedNodeCount}})>10, 10, 2*{{IndividualCausalMass}}+1.5*{{IndividualConfirmedNodeCount}})` | **CONVERTED:** magnitude = monotone function of validated causal mass only (rides mechanism, not ancestry correlation) |
| IndividualPredictions | CountBins | integer | **agg (NEW)** | `=COUNTIFS(CalibrationBins!{{IndividualPrediction}}, {{IndividualPredictionId}})` | total reliability bins |
| IndividualPredictions | CountWellCalibratedBins | integer | **agg (NEW)** | `=COUNTIFS(CalibrationBins!{{IndividualPrediction}}, {{IndividualPredictionId}}, CalibrationBins!{{IsWellCalibratedBin}}, TRUE())` | bins passing coverage+accuracy |
| IndividualPredictions | MeanBinAbsError | number | **agg (NEW)** | `=AVERAGEIFS(CalibrationBins!{{BinAbsError}}, CalibrationBins!{{IndividualPrediction}}, {{IndividualPredictionId}})` — **[fallback if AVERAGEIFS rejected or 0 bins]** add `SumBinAbsError = SUMIFS(...)` and use `=IF({{CountBins}}>0, {{SumBinAbsError}}/{{CountBins}}, 1)` | mean reliability gap |
| IndividualPredictions | WellCalibratedFraction | number | **calc (NEW)** | `=IF({{CountBins}} > 0, {{CountWellCalibratedBins}} / {{CountBins}}, 0)` | guarded fraction of trustworthy bins |
| IndividualPredictions | CalibratedUncertainty | number | **calc (NEW, same name as deleted raw)** | `=MAX(0, (1 - {{MeanBinAbsError}})) * {{WellCalibratedFraction}}` — **[fallback if MAX rejected]** `=IF((1-{{MeanBinAbsError}})<0, 0, (1-{{MeanBinAbsError}})) * {{WellCalibratedFraction}}`; if `CountBins=0`, `MeanBinAbsError` defaults to 1 ⇒ value 0 | **CONVERTED:** HIGH=reliable. (1−gap) scaled by well-covered-bin fraction; 0 for uncovered (e.g. absent-ancestry) predictions |
| IndividualPredictions | RestsOnConfirmedMechanism | boolean | **calc (NEW)** | `=IF({{IndividualConfirmedNodeCount}} >= 1, TRUE(), FALSE())` | grounded in ≥1 validated mechanism |
| IndividualPredictions | HasSpuriousCorrelationFlag | boolean | **calc (NEW, same name as deleted raw)** | `=IF(OR(NOT({{RestsOnConfirmedMechanism}}), {{IndividualHasCrypticRelatedness}}), TRUE(), FALSE())` | **CONVERTED:** spurious if no validated mechanism OR cryptic-relatedness leakage |
| IndividualPredictions | IsHighConfidencePrediction | boolean | **calc (REWRITE)** | `=IF(AND({{CalibratedUncertainty}} >= 0.7, NOT({{HasSpuriousCorrelationFlag}})), TRUE(), FALSE())` | **REWRITTEN:** both inputs now derived (H7, I4) |
| IndividualPredictions | IsFalsifiabilityBacked | boolean | **calc (NEW)** | `=IF({{IndividualConfirmedNodeCount}} >= 1, TRUE(), FALSE())` | inherits falsifiability — every confirmed node required `IsExperimentallyFalsifiable=TRUE` |
| IndividualPredictions | IsTransportableToAbsentAncestry | boolean | **calc (NEW)** | `=IF(AND({{IsAncestryHoldout}}, {{IndividualCrossAncestryNodeCount}} >= 1, NOT({{HasSpuriousCorrelationFlag}})), TRUE(), FALSE())` | **the de-tautologized transport gate:** holdout individual is transportable ONLY if they carry ≥1 node with *measured cross-ancestry replication* |
| IndividualPredictions | IsAncestryTransportSafe | boolean | **calc (NEW)** | `=IF({{IsAncestryHoldout}}, {{IsTransportableToAbsentAncestry}}, TRUE())` | holdout ⇒ require measured transport; in-training ⇒ vacuously safe |
| IndividualPredictions | PatientStratificationTier | string | **calc (KEEP — re-points to derived PredictedValue)** | `=IF({{PredictedValue}} >= 7, "High-Risk Pathway", IF({{PredictedValue}} >= 4, "Moderate-Risk Pathway", "Low-Risk Pathway"))` | unchanged formula; now reads derived PredictedValue |
| IndividualPredictions | **IsClinicallyActionable** | boolean | **calc (NEW — KEYSTONE)** | see §3 | the single terminal boolean |

### 2.5 Untouched existing calculated fields that now (correctly) depend on the rewired chain
`Individuals.CausalArchitectureScore` already references `CountOfCausalMechanisms` (raw count, unchanged) — **no edit**. `IsAncestryHoldout`, `IndividualAncestryLabel` on predictions — **kept as-is** (they are observational lookups).

---

## 3. THE KEYSTONE FORMULA + FULL DEPENDENCY TREE

```
IndividualPredictions.IsClinicallyActionable =
=IF(AND(
     {{IsHighConfidencePrediction}},      ' calibrated AND not spurious   (H7 + I4)
     {{IsFalsifiabilityBacked}},          ' >=1 falsifiable confirmed node (J2)
     {{IsAncestryTransportSafe}},         ' OOD-safe via MEASURED transport (J3)
     {{PredictedValue}} > 0               ' non-null mechanism-derived magnitude (G3)
   ), TRUE(), FALSE())
```

*(Note: `IsHighConfidencePrediction` already ANDs `NOT(HasSpuriousCorrelationFlag)`, so we do not re-AND it — this fixes the "redundant keystone" sloppiness Angle-1's verdict flagged.)*

### Dependency tree (every leaf is a raw OBSERVATION)

```
IsClinicallyActionable
├─ IsHighConfidencePrediction
│   ├─ CalibratedUncertainty            (H7, derived)
│   │   ├─ MeanBinAbsError ─ CalibrationBins.BinAbsError
│   │   │     └─ PredictedProbabilityBand[obs], ObservedEventRate[obs]
│   │   └─ WellCalibratedFraction
│   │         ├─ CountWellCalibratedBins ─ IsWellCalibratedBin
│   │         │     └─ CoverageCount[obs], BinAbsError(←obs above)
│   │         └─ CountBins[row-count of CalibrationBins]
│   └─ HasSpuriousCorrelationFlag       (I4, derived)
│         ├─ RestsOnConfirmedMechanism ─ IndividualConfirmedNodeCount
│         │     └─ Individuals.CountConfirmedCausalNodes ─ CausalMechanisms.IsCausalArchitectureNode (▼CHAIN)
│         └─ IndividualHasCrypticRelatedness ─ Individuals.HasCrypticRelatednessFlag[obs]
├─ IsFalsifiabilityBacked ─ IndividualConfirmedNodeCount ─ (▼CHAIN)
├─ IsAncestryTransportSafe
│   ├─ IsAncestryHoldout ─ Individuals.IsAncestryAbsentFromTraining[obs]
│   └─ IsTransportableToAbsentAncestry
│         ├─ IndividualCrossAncestryNodeCount ─ Individuals.CountCrossAncestryConfirmedNodes
│         │     └─ CausalMechanisms.IsAncestryTransportable
│         │           ├─ IsCausalArchitectureNode (▼CHAIN)
│         │           └─ CountCrossAncestryConcordant ─ CohortReplications.IsCrossAncestryConcordant
│         │                 ├─ ReplicatedAtNominalSig ← ReplicationPValue[obs], ReplicationEffectSign[obs]
│         │                 └─ IsDifferentAncestryReplication ← ReplicationAncestryLabel[obs],
│         │                       CausalMechanisms.IndividualAncestryLabel ← Individuals.AncestryLabel[obs]
│         └─ HasSpuriousCorrelationFlag (above)
└─ PredictedValue                        (G3, derived)
      ├─ IndividualCausalMass ─ Individuals.SumConfirmedCausalConfidence ─ CausalConfidence (▼CHAIN)
      └─ IndividualConfirmedNodeCount ─ (▼CHAIN)

▼CHAIN  CausalMechanisms.IsCausalArchitectureNode
   ├─ CausalConfidence
   │    ├─ CountQualifiedEvidence ─ EvidenceItems.IsQualifiedEvidence
   │    │     ├─ AssayIsHighQuality ← OmicsAssays.IsHighQualityAssay ← MeasurementErrorScore[obs]
   │    │     ├─ IsNegativeControlArm[obs]
   │    │     ├─ ZStat ← EffectSize[obs], StandardError[obs]
   │    │     └─ IsConfoundControlled ← IsAdjustedForAncestryPCs[obs], IsAdjustedForBatch[obs]
   │    ├─ CountModalitiesSupporting ← IsCrossModality[obs] (+ IsQualifiedEvidence above)
   │    ├─ ReplicationFraction ← CountConcordantReplications/CountReplications
   │    │     └─ CohortReplications.ReplicatedAtNominalSig ← ReplicationPValue[obs], ReplicationEffectSign[obs]
   │    └─ SurvivesNegativeControls ← NegativeControlTests.IsSurvived
   │          └─ PermutationEffectSize[obs], NullThreshold[obs]
   ├─ IsExperimentallyFalsifiable ← CountInterventionTargets[InterventionTargets row-count],
   │                                 CountQualifiedEvidence (above)
   ├─ IsSpuriousDerived ← ReplicatesAcrossCohorts, SurvivesNegativeControls,
   │                      CountModalitiesSupporting, HasPleiotropy[obs]
   └─ VariantIsCausalCandidate ← GenomicVariants.IsCausalCandidate
         ← AlleleFrequency[obs], HasAlleleSpecificExpression[obs], VariantTypes.IsRareVariantClass[obs]
```

**Leaf observation set (the ONLY raw inputs the keystone bottoms out in):** `AlleleFrequency`, `HasAlleleSpecificExpression`, `VariantTypes.IsRareVariantClass`, `OmicsAssays.MeasurementErrorScore`, `EvidenceItems.{EffectSize, StandardError, IsCrossModality, IsNegativeControlArm, IsAdjustedForAncestryPCs, IsAdjustedForBatch}`, `CohortReplications.{ReplicationEffectSign, ReplicationPValue, ReplicationAncestryLabel}`, `NegativeControlTests.{PermutationEffectSize, NullThreshold}`, `CalibrationBins.{PredictedProbabilityBand, ObservedEventRate, CoverageCount}`, `CausalMechanisms.HasPleiotropy`, `Individuals.{AncestryLabel, IsAncestryAbsentFromTraining, HasCrypticRelatednessFlag}`, plus `InterventionTargets` row existence. **No hand-entered conclusion remains.** The 7 banned raw fields are deleted and reborn at A3, C2, D8, E1, G3, H7, I4.

---

## 4. SPARSE / UNUSED ONTOLOGY CONTEXT (kept, NOT gating the keystone)

These remain in the rulebook as domain context / future spokes but are **not** on the `IsClinicallyActionable` dependency path. They are deliberately not load-bearing so the keystone stays minimal and testable:

- **`DiseaseStages`** (incl. `IsPresymptomatic`), **`ClinicalPhenotypes`** (`SeverityScore`, `IsHighSeverity`, `IsPresymptomaticPhenotype`) — observed clinical context; could later feed outcome labels but the keystone is mechanism+calibration-driven.
- **`EpistaticInteractions`** (`EpistasisScore`, `IsHighOrderEpistasis`) — higher-order genetics; reserved (`HasPleiotropy` on `CausalMechanisms` is the only epistasis-adjacent signal used).
- **`CounterfactualTrajectories`** (`ProjectedSeverity`, `IsWorseningTrajectory`) — **deliberately NOT used for falsifiability** (Angle 2 mis-joined it on the wrong FK; it has no `CausalMechanism` FK, only `Individual`). Falsifiability is sourced from `InterventionTargets` instead. Kept as scenario context.
- **`Treatments`** (`TreatmentResponse`, `HasAdverseEffect`, `IsEffectiveTreatment`) — observed outcomes; the honest source for authoring `CalibrationBins.ObservedEventRate` but not directly wired into the gate.
- **`OmicsModalities.IsSingleCell`, `OmicsAssays.HasCellStateSpecificEffect`, `Tissues`, `BatchId`** — modality/tissue context (methylome / Hi-C / mtDNA / repertoire / mosaicism etc. live here as modality rows); only `MeasurementErrorScore`→`IsHighQualityAssay` is load-bearing.
- **`FederatedDatasets.{Region, IsPrivacyPreserving}`, `Individuals.{AgeYears, EnrollmentDate, IsDevelopmentWindow, IsAgingWindow, RareVariantBurdenScore, CausalArchitectureScore}`** — cohort/demographic context; not gating.
- **`AutoimmuneDiseases.IsComplexDisease`, `InterventionTargets.{IsValidated, IsGeneBasedTherapy, IsCellBasedTherapy}`** — only `InterventionTargets` row *existence* per mechanism gates falsifiability; the therapy-class flags are descriptive.

---

## 5. MOCK-DATA STORY — 6 named patients, one failing on EACH distinct gate

Each patient = 1 `Individuals` row + supporting child rows. Thresholds to clear: `ZStat>=2` & confound-controlled & high-quality assay ⇒ `IsQualifiedEvidence`; `CausalConfidence>=0.7`; `ReplicatesAcrossCohorts` (≥2 reps, ≥2 concordant); `SurvivesNegativeControls` (≥1 test, all survive); `CountModalitiesSupporting>=2`; `IsWellCalibratedBin` (`CoverageCount>=20` & `BinAbsError<=0.1`); `CalibratedUncertainty>=0.7`. The 6 gates exercised: (G) calibration, (G) spurious-mechanism, (G) cryptic-relatedness leakage, (G) falsifiability/no-target, (G) ancestry-transport, plus one full-PASS.

> Assay quality: give qualified evidence rows `OmicsAssay` pointing to assays with `MeasurementErrorScore = 0.05` (⇒ `IsHighQualityAssay = TRUE`).

### Patient A — "Ana Reyes" → `IsClinicallyActionable = TRUE` (full pass, in-training ancestry)
- Individual: AncestryLabel="European", IsAncestryAbsentFromTraining=FALSE, HasCrypticRelatednessFlag=FALSE.
- 1 GenomicVariant: AlleleFrequency=0.004 (rare), HasAlleleSpecificExpression=TRUE ⇒ IsCausalCandidate=TRUE.
- 1 CausalMechanism (GenomicVariant set), HasPleiotropy=FALSE.
  - 3 EvidenceItems, all high-quality assay, IsNegativeControlArm=FALSE, EffectSize=0.8/SE=0.2 (ZStat=4), both adjust flags TRUE; ≥2 have IsCrossModality=TRUE ⇒ CountQualifiedEvidence=3, CountModalitiesSupporting=2, SumZStat=12.
  - 3 CohortReplications: PValue 0.01/0.02/0.2, sign +1/+1/+1 ⇒ ReplicatedAtNominalSig on 2 ⇒ CountReplications=3, CountConcordant=2 ⇒ ReplicatesAcrossCohorts=TRUE. (≥1 in a different ancestry ⇒ CrossAncestryConcordant≥1, but not required since in-training.)
  - 2 NegativeControlTests: PermutationEffectSize 0.01 & 0.02, NullThreshold=0.1 ⇒ both survive ⇒ SurvivesNegativeControls=TRUE.
  - 1 InterventionTarget ⇒ CountInterventionTargets=1.
  - ⇒ CausalConfidence ≈ 0.30·(3/4)+0.20·(2/3)+0.30·(0.667)+0.20·1 = 0.225+0.133+0.20+0.20 = **0.758 ≥0.7**; IsExperimentallyFalsifiable=TRUE; IsSpuriousDerived=FALSE ⇒ **IsCausalArchitectureNode=TRUE**.
- Prediction: 6 CalibrationBins, each CoverageCount=30, BinAbsError≤0.05 ⇒ all WellCalibrated ⇒ WellCalibratedFraction=1, MeanBinAbsError≈0.05 ⇒ **CalibratedUncertainty≈0.95**. PredictedValue=min(10, 2·0.758+1.5·1)=**3.02>0**.
- **Outcome: TRUE** — confident+falsifiable+non-spurious node, well-calibrated, non-holdout (transport-safe vacuously). *(Reason: all gates pass.)*

### Patient B — "Bili Okafor" → `FALSE` because **calibration coverage/accuracy gate fails**
- Identical mechanism setup to A (IsCausalArchitectureNode=TRUE, RestsOnConfirmedMechanism=TRUE, not cryptic, in-training).
- Difference: CalibrationBins all have CoverageCount=8 (<20) → IsWellCalibratedBin=FALSE for all ⇒ WellCalibratedFraction=0 ⇒ **CalibratedUncertainty=0 (<0.7)** ⇒ IsHighConfidencePrediction=FALSE.
- **Outcome: FALSE** — *reason: poorly calibrated (insufficient held-out coverage), even though the mechanism is fully confirmed.* Isolates the calibration gate.

### Patient C — "Chen Wei" → `FALSE` because **mechanism is spurious (sign-flip / fails neg-control)**
- Same evidence/calibration as A (6 bins CoverageCount=30, well-calibrated), in-training, not cryptic.
- Difference in the mechanism's children: CohortReplications signs +1/−1 and one NegativeControlTest PermutationEffectSize=0.5 (>NullThreshold 0.1) ⇒ IsSurvived=FALSE ⇒ SurvivesNegativeControls=FALSE ⇒ **IsSpuriousDerived=TRUE** ⇒ IsCausalArchitectureNode=FALSE ⇒ CountConfirmedCausalNodes=0 ⇒ RestsOnConfirmedMechanism=FALSE ⇒ HasSpuriousCorrelationFlag=TRUE; PredictedValue=0.
- **Outcome: FALSE** — *reason: rests on no validated mechanism; correlation failed replication-sign + negative-control.* Isolates the anti-spurious mechanism gate.

### Patient D — "Diego Santos" → `FALSE` because **cryptic-relatedness leakage**
- Mechanism fully confirmed like A; calibration well-covered like A; in-training.
- Difference: Individuals.HasCrypticRelatednessFlag=**TRUE** ⇒ IndividualHasCrypticRelatedness=TRUE ⇒ **HasSpuriousCorrelationFlag=TRUE** ⇒ IsHighConfidencePrediction=FALSE.
- **Outcome: FALSE** — *reason: prediction flagged spurious due to cryptic relatedness (leakage), despite a valid mechanism and good calibration.* Isolates the leakage branch of the spurious gate.

### Patient E — "Esi Mensah" → `FALSE` because **not experimentally falsifiable (no intervention target)**
- Mechanism has qualified evidence (CountQualifiedEvidence≥1, ≥2 modalities), replicates, survives controls ⇒ confidence could be ≥0.7 and non-spurious — BUT **0 InterventionTargets** ⇒ CountInterventionTargets=0 ⇒ **IsExperimentallyFalsifiable=FALSE** ⇒ IsCausalArchitectureNode=FALSE ⇒ CountConfirmedCausalNodes=0 ⇒ IsFalsifiabilityBacked=FALSE (and RestsOnConfirmedMechanism=FALSE).
- Calibration well-covered; in-training; not cryptic.
- **Outcome: FALSE** — *reason: no perturbable intervention target ⇒ mechanism not experimentally falsifiable ⇒ prediction not falsifiability-backed.* Isolates the falsifiability gate.

### Patient F — "Faisal Haidar" → `FALSE` because **ancestry-holdout AND no cross-ancestry-replicated mechanism**
- Individuals.IsAncestryAbsentFromTraining=**TRUE** (e.g. AncestryLabel="Indigenous-American"), not cryptic.
- Mechanism is confirmed (IsCausalArchitectureNode=TRUE) BUT **all** its CohortReplications ran in the **same** ancestry as the mechanism ⇒ IsDifferentAncestryReplication=FALSE for all ⇒ CountCrossAncestryConcordant=0 ⇒ IsAncestryTransportable=FALSE ⇒ IndividualCrossAncestryNodeCount=0 ⇒ IsTransportableToAbsentAncestry=FALSE ⇒ **IsAncestryTransportSafe=FALSE**.
- To prove the gate is the *only* failure, also seed CalibrationBins with CoverageCount=30 well-calibrated *(authored as if held-out coverage existed)*; mechanism confirmed; RestsOnConfirmedMechanism=TRUE; not cryptic — so IsHighConfidencePrediction and IsFalsifiabilityBacked both PASS.
- **Outcome: FALSE** — *reason: out-of-distribution ancestry and the carried mechanism was never replicated in a different ancestry (no measured cross-ancestry invariance).* Isolates the ancestry-transport gate — the marquee requirement, now a measured fact.

> **Optional Patient G — "Grace Lin" → `TRUE` (holdout that DOES transport):** same as F but ≥1 CohortReplication with `ReplicationAncestryLabel` ≠ mechanism's primary ancestry and `ReplicationPValue<=0.05`, sign +1 ⇒ IsCrossAncestryConcordant=TRUE ⇒ IsAncestryTransportable=TRUE ⇒ IsTransportableToAbsentAncestry=TRUE ⇒ **IsClinicallyActionable=TRUE**. Proves the transport gate passes on real cross-ancestry invariance, not on holdout-status alone — the positive twin of F.

**Coverage matrix:** A=all-pass; B=calibration; C=anti-spurious-mechanism; D=cryptic-relatedness; E=falsifiability; F=ancestry-transport-fail; G=ancestry-transport-pass. Every distinct keystone gate is exercised by both a passing and a failing case.

---

## 6. Implementation order & build note

1. Add tables `EvidenceItems`, `CohortReplications`, `NegativeControlTests`, `CalibrationBins` (each with `Name` as field 1 per ERB convention, then the raw observation fields, then the calc/lookup/agg fields above).
2. In `GenomicVariants`, `CausalMechanisms`, `IndividualPredictions`: delete the 7 banned raw fields; add/rewrite the calc/lookup/agg fields in §2; add `IsClinicallyActionable`.
3. Seed mock data per §5 (patients A–G + their child rows).
4. **If the transpiler rejects any of `SUMIFS`/`AVERAGEIFS`/`MIN`/`MAX`/`ABS`** (none appear in the current rulebook), apply the inline **[fallback]** rewrites (guarded division + nested-IF caps), which use only IF/AND/OR/arithmetic that the live rulebook already proves. The DAG topology and keystone semantics are unchanged either way.
5. Per project rules: **ask permission before editing `effortless-rulebook.json` and before running `effortless build`.**

Rulebook hub to edit: `/Users/eejai42/effortlessapi-app-root/users/user_ee42ai73-18a9-47d5-8f99-954b00f6c041/my-projects/jholla-problem-to-solve/effortless-rulebook/effortless-rulebook.json`