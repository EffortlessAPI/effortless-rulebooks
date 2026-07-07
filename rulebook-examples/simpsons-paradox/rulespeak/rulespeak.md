# 📘 simpsons-paradox — RuleSpeak

_Digital mirror of the Simpson's Paradox domain. The entities are Studies, Treatments, Strata, and CaseCells (raw patient counts). SuccessRates, TreatmentSummaries, and TreatmentRankings are derived. The paradox falls out of the DAG as an emergent derived fact — it is never modeled directly._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Loop** | A loop is identified by its name. | — |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Core Conern | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern]). | _Lookup: ResearchTraditions.CoreConcern via TraditionId — which intellectual concern drove this loop._ |
| Commit Short | Computed as the first 7 character(s) of the commit hash. | _First 7 characters of CommitHash for display and git checkout._ |
| **Study** | A study is identified by its name. | — |
| Total Cases | The total cases across the case cells related to the study. | _Total number of cases (trials) across all CaseCells in this study._ |
| Cell Count | The number of case cells related to the study. | _Number of CaseCell rows for this study._ |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Core Conern | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern]). | _Lookup: ResearchTraditions.CoreConcern via TraditionId._ |
| Primary Researcher Name | The value of LOOKUP(PrimaryResearcherId, Researchers[ResearcherId], Researchers[Name]). | _Lookup: Researchers.Name via PrimaryResearcherId._ |
| Primary Researcher Affiliation | The value of LOOKUP(PrimaryResearcherId, Researchers[ResearcherId], Researchers[Affiliation]). | _Lookup: Researchers.Affiliation via PrimaryResearcherId._ |
| Distortion Type | The value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). | _Lookup: TreatmentRankings.DistortionType for this study — the geometric classification of its paradox._ |
| Policy Implication | The value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedPolicyImplication]). | _Lookup: TreatmentRankings.CorrectedPolicyImplication for this study._ |
| Allocation Distortion | The value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[AllocationDistortion]). | _Lookup: TreatmentRankings.AllocationDistortion for this study — the scalar severity of allocation-induced distortion._ |
| Signal Purity | The value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[SignalPurity]). | _Lookup: TreatmentRankings.SignalPurity for this study — ratio of true signal to total gap width._ |
| Is Sign Flip | The value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[IsSignFlip]). | _Lookup: TreatmentRankings.IsSignFlip for this study — true if the paradox satisfies the continuous sign-flip definition._ |
| Corrected Winner | The value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedWinner]). | _Lookup: TreatmentRankings.CorrectedWinner for this study — which treatment wins after allocation correction._ |
| Confounding Variable | The value of LOOKUP(StudyId, StratumVariables[Study], StratumVariables[VariableName]). | _Lookup: StratumVariables.VariableName for this study — the named stratifying variable._ |
| Causal Role | The value of LOOKUP(StudyId, StratumVariables[Study], StratumVariables[CausalRole]). | _Lookup: StratumVariables.CausalRole for this study — confounder, mediator, collider, proxy, or contested._ |
| Ingestion Cell Parity | True when the value of AND(LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[StratumCount]) >= 2, CellCount = LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[StratumCount]) * 2). | _TRUE when CellCount = 2 x StratumCount._ |
| Ingestion Compliance | Computed as the value of IF(IsSynthetic, "exempt-synthetic", IF(AND(IngestionCellParity, LOOKUP(StudyId, StratumVariables[Study], StratumVariables[VariableName]) <> ""), "all", "partial")). | _all \| partial \| exempt-synthetic._ |
| **Treatment** | A treatment is identified by its name and is related to a study. | — |
| Total Cases | The total cases across the case cells related to the treatment. | _Total cases for this treatment across all strata._ |
| Total Successes | The total successes across the case cells related to the treatment. | _Total successes for this treatment across all strata._ |
| Pooled Success Rate | Determined by priority: an empty string if the total cases is 0; in all other cases, the total successes divided by the total cases. | _Pooled (across all strata) success rate for this treatment: TotalSuccesses / TotalCases._ |
| **Strata** | A strata is identified by its name and is related to a study. | — |
| Total Cases | The total cases across the case cells related to the strata. | _Total cases in this stratum across all treatments._ |
| **Case Cell** | A case cell is identified by its name and is related to a study. | — |
| Cell Success Rate | Determined by priority: an empty string if the cases is 0; in all other cases, the successes divided by the cases. | _Success rate within this cell: Successes / Cases. The first derived fact in the DAG._ |
| Total Cases for Treatment | The total cases across the case cells related to the case cell. | _Total cases across ALL strata for this treatment in this study — the denominator for TreatmentExposureFraction._ |
| Treatment Exposure Fraction | Determined by priority: an empty string if the total cases for treatment is 0; in all other cases, the cases divided by the total cases for treatment. | _Fraction of this treatment's total cases that fall in this stratum: Cases / TotalCasesForTreatment. High imbalance across strata is the mechanism of confounding — when treatments are allocated very differently across strata, the pooled rate conflates treatment effect with stratum difficulty._ |
| Is Valid Ingestion Cell | True when all of the following hold: the cases is greater than 0; the successes is at least 0; and the successes is at most the cases. | _TRUE when Cases>0, Successes>=0, and Successes<=Cases._ |
| **Stratum Summary** | A stratum summary is identified by its name and is related to a study. | — |
| Stratum Successes | The total successes across the case cells related to the stratum summary. | _Total successes in this (stratum, treatment) cell for this study._ |
| Stratum Cases | The total cases across the case cells related to the stratum summary. | _Total cases in this (stratum, treatment) cell for this study._ |
| Stratum Success Rate | Determined by priority: an empty string if the stratum cases is 0; in all other cases, the stratum successes divided by the stratum cases. | _Success rate within this stratum for this treatment: StratumSuccesses / StratumCases. The true per-stratum picture._ |
| Stratum Successes a | The total successes across the case cells related to the stratum summary. | _Successes for treatment A in this (study, stratum) — used to determine the per-stratum winner._ |
| Stratum Cases a | The total cases across the case cells related to the stratum summary. | _Cases for treatment A in this (study, stratum)._ |
| Stratum Rate a | Determined by priority: an empty string if the stratum cases a is 0; in all other cases, the stratum successes a divided by the stratum cases a. | _Success rate for treatment A in this stratum._ |
| Stratum Successes B | The total successes across the case cells related to the stratum summary. | _Successes for treatment B in this (study, stratum)._ |
| Stratum Cases B | The total cases across the case cells related to the stratum summary. | _Cases for treatment B in this (study, stratum)._ |
| Stratum Rate B | Determined by priority: an empty string if the stratum cases b is 0; in all other cases, the stratum successes b divided by the stratum cases b. | _Success rate for treatment B in this stratum._ |
| Stratum Winner | Determined by priority: “A” if the stratum rate a is greater than the stratum rate b; in all other cases, “B”. | _Which treatment wins in this stratum: 'A' if StratumRateA > StratumRateB, else 'B'. Used by TreatmentRankings to detect paradox._ |
| Study Total Cases | The total cases across the case cells related to the stratum summary. | _Total cases across ALL strata and ALL treatments in this study. The denominator for StratumFraction._ |
| Stratum Total Cases | The total cases across the case cells related to the stratum summary. | _Total cases in this stratum across both treatments (A + B combined). Numerator for StratumFraction._ |
| Stratum Fraction | Determined by priority: an empty string if the study total cases is 0; in all other cases, the stratum total cases divided by the study total cases. | _Fraction of the study's total cases that fall in this stratum: StratumTotalCases / StudyTotalCases. The weight the confounder assigns to each stratum. A large StratumFraction on a low-success stratum pulls the pooled rate down; when that stratum is also over-allocated to one treatment, it creates the reversal._ |
| Weighted Stratum Rate | Determined by priority: an empty string if the stratum fraction is blank; in all other cases, the stratum fraction times the stratum success rate. | _StratumFraction × StratumSuccessRate. The contribution of this (stratum, treatment) row to the study-level pooled rate. Summing WeightedStratumRate across all strata for a given treatment reconstructs that treatment's pooled rate from stratum weights — witnessing the mechanism equation._ |
| Treatment a Cases Here | The total cases across the case cells related to the stratum summary. | _Cases for treatment A in this stratum — used to compute TreatmentA's allocation fraction in this stratum._ |
| Treatment B Cases Here | The total cases across the case cells related to the stratum summary. | _Cases for treatment B in this stratum — used to compute TreatmentB's allocation fraction in this stratum._ |
| Treatment a Total Cases | The total cases across the stratum summary's case cells that have a treatment label of “A”. | _Total cases for treatment A across all strata in this study — denominator for AllocationFractionA._ |
| Treatment B Total Cases | The total cases across the stratum summary's case cells that have a treatment label of “B”. | _Total cases for treatment B across all strata in this study — denominator for AllocationFractionB._ |
| Allocation Fraction a | Determined by priority: an empty string if the treatment a total cases is 0; in all other cases, the treatment a cases here divided by the treatment a total cases. | _Fraction of treatment A's total cases that land in this stratum: TreatmentACasesHere / TreatmentATotalCases. Equivalent to TreatmentExposureFraction for A._ |
| Allocation Fraction B | Determined by priority: an empty string if the treatment b total cases is 0; in all other cases, the treatment b cases here divided by the treatment b total cases. | _Fraction of treatment B's total cases that land in this stratum: TreatmentBCasesHere / TreatmentBTotalCases._ |
| Allocation Bias | Determined by priority: an empty string if the allocation fraction a is blank; in all other cases, the allocation fraction a minus the allocation fraction b. | _AllocationFractionA minus AllocationFractionB: how much more of treatment A (relative to B) is concentrated in this stratum. Positive = A is over-allocated here; negative = B is over-allocated here. When AllocationBias is large and negative in a low-success stratum, treatment A is under-represented in the hard cases — making A look worse in the pooled view than it is in any stratum. This is the confounder's fingerprint as a signed number._ |
| Stratum Gap | Determined by priority: an empty string if the stratum rate a is blank; in all other cases, the stratum rate a minus the stratum rate b. | _Signed rate difference within this stratum: StratumRateA minus StratumRateB. Positive = A wins here; negative = B wins here; zero = tied. When every stratum has StratumGap > 0 but the pooled gap is negative, the contradiction IS Simpson's Paradox made arithmetically visible in one column. Multiply StratumGap × StratumFraction and sum across strata to get the equal-weight pooled gap (the signal allocation would produce without confounding)._ |
| Weighted Stratum Gap | Determined by priority: an empty string if the stratum gap is blank; in all other cases, the stratum gap times the stratum fraction. | _StratumGap × StratumFraction: the contribution of this stratum's treatment-rate difference to the equal-weight pooled gap. Summing WeightedStratumGap across all TreatmentA rows for a study gives WeightedStratumGapSum — the pooled gap the study would show under balanced allocation. This is the per-stratum building block of the sign-flip test._ |
| **Model Summary** | A model summary is identified by its name. | — |
| Reversal Count | The number of treatment rankings related to the model summary. | _Number of studies with IsReversal=TRUE (sign-flip / allocation-driven reversal)._ |
| Non Reversal Count | The number of treatment rankings related to the model summary. | _Number of studies with IsReversal=FALSE._ |
| Study Count | Computed as the reversal count plus the non reversal count. | _Total number of TreatmentRankings in this model: ReversalCount + NonReversalCount._ |
| Explained Count | The number of treatment rankings related to the model summary. | _Number of studies with IsParadoxExplained=TRUE (reversal AND confirmed confounder)._ |
| Total Paradox Strength | The total paradox strength across the treatment rankings related to the model summary. | _Sum of ParadoxStrength across all studies. Used to compute average._ |
| Avg Paradox Strength | Determined by priority: an empty string if the study count is 0; in all other cases, the total paradox strength divided by the study count. | _Average ParadoxStrength across all studies: TotalParadoxStrength / StudyCount. A scalar summary of how paradox-rich this dataset is._ |
| Type a Count | The number of treatment rankings related to the model summary. | _Studies classified as Type-A: sign flip AND full reversal (ReversalIntensity=1). The canonical Simpson's Paradox — allocation distortion reverses the pooled winner._ |
| Type B Count | The number of treatment rankings related to the model summary. | _Studies classified as Type-B: sign flip but partial reversal (ReversalIntensity<1). Allocation distortion creates a sign flip but the pooled signal partially reflects per-stratum reality._ |
| Type D Count | The number of treatment rankings related to the model summary. | _Studies classified as Type-D: no sign flip, negligible distortion. Pooled analysis is trustworthy — allocation is sufficiently balanced._ |
| Type a Fraction | Determined by priority: an empty string if the study count is 0; in all other cases, the type a count divided by the study count. | _Fraction of studies that are Type-A (full canonical reversals). TypeACount / StudyCount._ |
| Distortion Taxonomy Coverage | Computed as “A:”, followed by the type a count, followed by “ B:”, followed by the type b count, followed by “ C+:”, followed by the c amplification count, followed by “ C-:”, followed by the c compression count, followed by “ D:”, followed by the type d count. | _Human-readable summary of the four-type distribution: e.g. 'A:2 B:1 C:1 D:0'. The model's self-portrait of its own distortion geometry._ |
| Distortion Only Count | Computed as the c amplification count plus the c compression count. | _Number of studies with real allocation distortion but no sign flip: TypeCCount. The pooled winner is correct but the effect size is biased. Distinct from ZeroStrengthCount, which conflates Type-C (real distortion) with Type-D (neutral)._ |
| C Amplification Count | The number of treatment rankings related to the model summary. | _Number of studies classified as type C+ (allocation-amplified): pooled winner is correct but pooled gap overstates true advantage. Causes researcher overconfidence — may accept higher cost/risk than true effect warrants._ |
| C Compression Count | The number of treatment rankings related to the model summary. | _Number of studies classified as type C- (allocation-compressed): pooled winner is correct but pooled gap understates true advantage. Causes researcher underconfidence — effective treatment may be prematurely abandoned._ |
| Avg Signal Purity | An aggregated value computed across the model summary's related records. | _Average SignalPurity across all TreatmentRankings. Below 0.5 means allocation noise exceeds true signal on average — the corpus is predominantly misleading at the pooled level. Above 0.5 means the corpus is predominantly trustworthy._ |
| Sweep Corrected Gap Max | The largest sweep corrected gap across the allocation sweep related to the model summary. | _Maximum SweepCorrectedGap across all AllocationSweep rows. Should equal SweepCorrectedGapMin — the invariant._ |
| Sweep Corrected Gap Min | The smallest sweep corrected gap across the allocation sweep related to the model summary. | _Minimum SweepCorrectedGap across all AllocationSweep rows. Should equal SweepCorrectedGapMax — the invariant._ |
| Sweep Corrected Gap Range | Determined by priority: an empty string if the sweep corrected gap max is blank; in all other cases, the sweep corrected gap max minus the sweep corrected gap min. | _SweepCorrectedGapMax − SweepCorrectedGapMin. The allocation-invariance witness: this should be zero (up to floating-point rounding). If nonzero, the corrected gap is not allocation-free._ |
| Sweep Pooled Gap Range | An aggregated value computed across the model summary's related records. | _MAX(SweepPooledGap) − MIN(SweepPooledGap) across all AllocationSweep rows. Expected to be large (≈0.21) — the pooled gap wanders as allocation shifts._ |
| Ingestion Protocol Item Count | The number of ingestion protocol related to the model summary. | _Number of rows in IngestionProtocol — the documented contract items._ |
| Corpus Passes Ingestion Contract | The value of LOOKUP("ingestion-v1", IngestionSummary[IngestionSummaryId], IngestionSummary[IngestionContractPasses]). | _Lookup: IngestionSummary.IngestionContractPasses._ |
| Ingestion Witness Note | The value of LOOKUP("ingestion-v1", IngestionSummary[IngestionSummaryId], IngestionSummary[IngestionWitnessNote]). | _Human-readable ingestion contract witness from IngestionSummary._ |
| Catalog Entry Count | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TotalCatalogEntries]). | _Total catalog rows._ |
| Pending Import Count | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CandidateCount]). | _Candidate studies queued._ |
| Ready to Encode Count | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ReadyToEncodeCount]). | _Encode-ready candidates._ |
| Import Session Ready | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ImportSessionReady]). | _Bulk import session readiness flag._ |
| Catalog Witness Note | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogWitnessNote]). | _Import backlog witness string._ |
| Latent Type D Count | The number of treatment rankings related to the model summary. | _Type-D studies with LatentFlipPotential=TRUE._ |
| Stable Type D Count | The number of the model summary's treatment rankings that are not latent flip potential. | _Type-D studies without latent flip potential (allocation-stable at observed and counterfactual sweep)._ |
| Latent Type D Fraction | Determined by priority: an empty string if the type d count is 0; in all other cases, the latent type d count divided by the type d count. | _Fraction of Type-D studies with latent flip potential._ |
| Cross Zero Count | The number of treatment rankings related to the model summary. | _Studies whose pooled gap crosses zero under allocation sweep._ |
| Sign Flip Signal Purity Max | The largest signal purity across the treatment rankings related to the model summary. | _Maximum SignalPurity among sign-flip studies. Must stay below 0.5 for the signal-purity theorem._ |
| Economics Sign Flip Count | The number of the model summary's treatment rankings that are sign flips. | _Sign-flip count in economics domain._ |
| Avg Pooled Gap Latent D | Determined by priority: an empty string if the latent type d count is 0; in all other cases, the average pooled gap across the treatment rankings related to the model summary. | _Average PooledGap among Type-D studies with latent flip potential._ |
| Avg Pooled Gap Stable D | Determined by priority: an empty string if the stable type d count is 0; in all other cases, the average pooled gap across the model summary's treatment rankings that are not latent flip potential. | _Average PooledGap among allocation-stable Type-D studies._ |
| Epidemiology Avg Distortion | The average allocation distortion across the treatment rankings related to the model summary. | _Mean AllocationDistortion in epidemiology studies._ |
| Education Avg Distortion | The average allocation distortion across the treatment rankings related to the model summary. | _Mean AllocationDistortion in education studies._ |
| Confounder Sign Flip Count | The number of the model summary's treatment rankings that are sign flips. | _Sign-flip count among confounder-labeled studies._ |
| Confounder Latent Only Count | The number of the model summary's treatment rankings that are latent only flips. | _Latent-only flip count among confounder-labeled studies._ |
| Collider Selection Count | Computed as the number of treatment rankings related to the model summary plus the number of treatment rankings related to the model summary. | _Studies tagged collider or selection on StratumVariables._ |
| Collider Selection Manifest Count | Computed as the number of the model summary's treatment rankings that are sign flips plus the number of the model summary's treatment rankings that are sign flips. | _Manifest sign-flips among collider/selection studies._ |
| Collider Selection Latent Only Count | Computed as the number of the model summary's treatment rankings that are latent only flips plus the number of the model summary's treatment rankings that are latent only flips. | _Latent-only flips among collider/selection studies._ |
| Explained Confounder Count | The number of treatment rankings related to the model summary. | _Count of TreatmentRankings rows where IsParadoxExplained=TRUE (should equal ConfounderSignFlipCount)._ |
| Contested or Mediator Explained Count | The number of the model summary's treatment rankings that are paradox-explained and are not adjustment appropriate. | _Count of contested or mediator sign-flip rows where IsParadoxExplained=TRUE (should be zero)._ |
| Discovery Witness Note | Computed as “sweep: latentD=”, followed by the latent type d fraction, followed by “ purityMax=”, followed by the sign flip signal purity max, followed by “ catalogExact=”, followed by the type prediction match rate, followed by “ domainGapSurvives=”, followed by the domain flip gap survives geometry control. | _Research sweep synthesis rollup (loops 61–66)._ |
| Type Prediction Match Count | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMatchCount]). | _Lookup: CorpusCatalogSummary.TypePredictionMatchCount._ |
| Type Prediction Mismatch Count | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMismatchCount]). | _Lookup: CorpusCatalogSummary.TypePredictionMismatchCount._ |
| Type Prediction Match Rate | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMatchRate]). | _Lookup: CorpusCatalogSummary.TypePredictionMatchRate._ |
| Sign Flip Prediction Match Rate | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[SignFlipPredictionMatchRate]). | _Lookup: CorpusCatalogSummary.SignFlipPredictionMatchRate._ |
| Catalog Prediction Witness Note | The value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogPredictionWitnessNote]). | _Lookup: CorpusCatalogSummary.CatalogPredictionWitnessNote._ |
| C Plus Avg Distortion | The average allocation distortion across the treatment rankings related to the model summary. | _Mean AllocationDistortion among Type-C+ studies._ |
| C Minus Avg Distortion | The average allocation distortion across the treatment rankings related to the model summary. | _Mean AllocationDistortion among Type-C- studies._ |
| Type D Avg Distortion | The average allocation distortion across the treatment rankings related to the model summary. | _Mean AllocationDistortion among Type-D studies._ |
| Sweep Fragile Count | The number of the model summary's treatment rankings that are signal purity; are not allocation distortion; have a sweep pooled gap range of “>0.3”; and have a allocation fragility of “>=10”. | _Type-D studies with signal_purity=1, allocation_distortion=0, sweep_pooled_gap_range>0.3, allocation_fragility>=10._ |
| Expansion Wave1 Economics Expected a Count | The number of the model summary's candidate study catalog that are imported; have a domain of “economics”; and have a expected distortion type of “A”. | _Imported expansion-wave-1 economics catalog rows tagged Expected A._ |
| Expansion Wave1 Economics Expected AD Count | The number of the model summary's candidate study catalog that are imported; have a domain of “economics”; have a expected distortion type of “A”; and have a observed distortion type of “D”. | _Expansion-wave-1 economics Expected-A imports observed as Type D._ |
| Economics Expected a Mismatch Rate | Determined by priority: an empty string if the expansion wave1 economics expected a count is 0; in all other cases, the expansion wave1 economics expected AD count divided by the expansion wave1 economics expected a count. | _Fraction of expansion-wave-1 economics Expected-A imports observed as Type D._ |
| **Stratum Variable** | A stratum variable is identified by its name and is related to a study. | — |
| Is Confounder | True when all of the following hold: the affects treatment assignment flag is set; the affects outcome flag is set; and the causal role is “confounder”. | _TRUE when AffectsTreatmentAssignment AND AffectsOutcome AND CausalRole = 'confounder'. A confounder is the classic driver of Simpson's Paradox._ |
| **Treatment Ranking** | A treatment ranking is identified by its name and is related to a study. | — |
| Total Cases a | The total cases across the case cells related to the treatment ranking. | _Total cases for TreatmentA across all strata in this study._ |
| Total Successes a | The total successes across the case cells related to the treatment ranking. | _Total successes for TreatmentA across all strata._ |
| Pooled Rate a | Determined by priority: an empty string if the total cases a is 0; in all other cases, the total successes a divided by the total cases a. | _Pooled success rate for TreatmentA: TotalSuccessesA / TotalCasesA._ |
| Total Cases B | The total cases across the case cells related to the treatment ranking. | _Total cases for TreatmentB across all strata._ |
| Total Successes B | The total successes across the case cells related to the treatment ranking. | _Total successes for TreatmentB across all strata._ |
| Pooled Rate B | Determined by priority: an empty string if the total cases b is 0; in all other cases, the total successes b divided by the total cases b. | _Pooled success rate for TreatmentB: TotalSuccessesB / TotalCasesB._ |
| Pooled Winner | Determined by priority: an empty string if at least one of the following holds: the pooled rate a is blank or the pooled rate b is blank; “tie” if the absolute value of the pooled rate a minus the pooled rate b is less than 0.0000001; the treatment a if the pooled rate a is greater than the pooled rate b; in all other cases, the treatment b. | _Which treatment wins when strata are ignored. Returns tie when pooled rates are equal within floating-point tolerance._ |
| Stratum Count | The number of stratum summaries related to the treatment ranking. | _Number of strata in this study. Counted from StratumSummaries filtered to TreatmentA rows (one row per stratum)._ |
| Strata Won by a | The number of stratum summaries related to the treatment ranking. | _Number of strata where A wins: count StratumSummaries rows whose TreatmentLabel=TreatmentA AND StratumWinner=TreatmentA. These are the one-per-stratum sentinel rows._ |
| Strata Won by B | The number of stratum summaries related to the treatment ranking. | _Number of strata where B wins: count StratumSummaries rows whose TreatmentLabel=TreatmentA AND StratumWinner=TreatmentB._ |
| Per Stratum Winner | Determined by priority: the treatment a if the strata won by a is the stratum count; the treatment b if the strata won by b is the stratum count; in all other cases, “none”. | _The treatment that wins in every stratum — 'A' if StrataWonByA = StratumCount, 'B' if StrataWonByB = StratumCount, else 'none' (no unanimous per-stratum winner)._ |
| Is Reversal | True when the sign flip flag is set. | _TRUE when the equal-weight pooled signal (WeightedStratumGapSum) and the actual pooled signal (SignedPooledGap) point in opposite directions — allocation has flipped the direction of the aggregate conclusion. This is Simpson's Paradox as a derived fact._ |
| Confounders in Study | The number of the treatment ranking's stratum variables that are confounders. | _Count of StratumVariables in this study whose IsConfounder = TRUE. When > 0, the paradox has a causal explanation._ |
| Is Paradox Explained | True when all of the following hold: the reversal flag is set and the confounders in study is greater than 0. | _TRUE when IsReversal is present AND at least one confirmed confounder exists in the study._ |
| Pooled Gap | Determined by priority: an empty string if the pooled rate a is blank; in all other cases, the absolute value of the pooled rate a minus the pooled rate b. | _Absolute difference between the two pooled rates: \|PooledRateA - PooledRateB\|. The size of the aggregate misleading signal._ |
| Strata Won by Loser | Determined by priority: the strata won by b if the pooled winner is the treatment a; in all other cases, the strata won by a. | _Number of strata won by the pooled loser — the counter-signal. For full reversals this equals StratumCount; for partial paradoxes it is between 0 and StratumCount._ |
| Paradox Strength | Determined by priority: an empty string if the stratum count is 0; in all other cases, the pooled gap times the strata won by loser divided by the stratum count. | _Scalar severity of the paradox: PooledGap × (StrataWonByLoser / StratumCount). Zero when no strata go against the pooled winner. Positive for partial paradoxes. Maximum when every stratum contradicts the pooled result._ |
| Pooled Rate From Weights a | The total weighted stratum rate across the stratum summaries related to the treatment ranking. | _Pooled rate for TreatmentA reconstructed as a weighted average of stratum rates: SUM(WeightedStratumRate) across all stratum rows for TreatmentA. Must match PooledRateA — this witnesses the mechanism equation: the pooled rate IS a weighted average of stratum rates, weighted by StratumFraction._ |
| Pooled Rate From Weights B | The total weighted stratum rate across the stratum summaries related to the treatment ranking. | _Pooled rate for TreatmentB reconstructed as a weighted average. Must match PooledRateB. If it does, the mechanism equation is verified: reversal is purely a consequence of differential weighting._ |
| Reversal Intensity | Determined by priority: an empty string if the stratum count is 0; in all other cases, the strata won by loser divided by the stratum count. | _StrataWonByLoser / StratumCount: the fraction of strata that go against the pooled winner. Zero for no paradox, 1.0 for full reversal, between 0 and 1 for partial. This is the allocation-side measure of how deeply the confounding has penetrated the stratified view._ |
| Threshold Margin | Determined by priority: an empty string if the stratum count is 0; in all other cases, the reversal intensity minus 0.5. | _ReversalIntensity minus 0.5: positive when more than half the strata go against the pooled winner (reversal is robust), zero at the tipping point (exactly half), negative when fewer than half oppose the pooled winner (reversal is fragile or absent). A study with ThresholdMargin > 0 is robustly paradoxical; one with ThresholdMargin < 0 has a weak or absent paradox._ |
| Signed Pooled Gap | Determined by priority: an empty string if the pooled rate a is blank; in all other cases, the pooled rate a minus the pooled rate b. | _Signed difference between pooled rates: PooledRateA minus PooledRateB. Positive = A wins pooled; negative = B wins pooled. Unlike PooledGap (which is absolute), this preserves direction — essential for detecting sign flips between the equal-weight and actual pooled signals._ |
| Weighted Stratum Gap Sum | The total weighted stratum gap across the stratum summaries related to the treatment ranking. | _Sum of WeightedStratumGap (= StratumGap × StratumFraction) across all StratumSummaries rows where TreatmentLabel = TreatmentA for this study. This is the equal-weight pooled gap — the gap the pooled analysis would produce if allocation were perfectly balanced across strata. When this has the opposite sign to SignedPooledGap, the allocation has not merely compressed the signal: it has flipped it entirely._ |
| Is Sign Flip | True when the signed pooled gap is less than 0 if the weighted stratum gap sum is greater than 0, in all other cases the signed pooled gap is greater than 0. | _TRUE when WeightedStratumGapSum and SignedPooledGap have opposite signs. Same condition as IsReversal; retained as the coordinate-level name in the DAG._ |
| Allocation Distortion | Determined by priority: an empty string if the weighted stratum gap sum is blank; in all other cases, the absolute value of the weighted stratum gap sum minus the signed pooled gap. | _The magnitude of how far the allocation has bent the pooled signal: \|WeightedStratumGapSum − SignedPooledGap\|. Zero means the allocation is neutral — the pooled analysis faithfully represents the equal-weight stratum evidence. A large AllocationDistortion means the allocation is doing most of the work: the pooled number is mostly noise from how cases were assigned, not signal about which treatment is better. This measure does NOT require a sign flip to be nonzero — it captures any allocation-induced distortion, not just reversals._ |
| Distortion Type | Determined by priority: an empty string if the allocation distortion is blank; “B” if all of the following hold: the weighted stratum gap sum is not 0 and the signed pooled gap divided by the weighted stratum gap sum is less than the negative of 1, in all other cases “A” if all of the following hold: the weighted stratum gap sum is not 0 and the signed pooled gap divided by the weighted stratum gap sum is less than 0, in all other cases “A” if the reversal intensity is 1, in all other cases “B” if the sign flip flag is set; “C+” if all of the following hold: it is not the case that the sign flip flag is set; the allocation distortion is greater than 0.01; and the absolute value of the signed pooled gap is greater than the absolute value of the corrected gap plus 0.001; “C-” if all of the following hold: it is not the case that the sign flip flag is set; the allocation distortion is greater than 0.01; and the absolute value of the signed pooled gap is less than the absolute value of the corrected gap minus 0.001; in all other cases, “D”. | _Geometric classification (five types A/B/C+/C-/D). Sign-flip types A vs B are split by DistortionRatio when available: ratio < -1 → B (amplified reversal); ratio in (-1,0) → A (attenuated reversal). Falls back to ReversalIntensity when ratio is degenerate._ |
| Policy Implication | Determined by priority: an empty string if the distortion type is blank; “stratify-immediately” if the distortion type is “A”; “investigate-confounder” if the distortion type is “B”; “check-allocation-bias” if at least one of the following holds: the distortion type is “C+” or the distortion type is “C-”; in all other cases, “pooled-analysis-trustworthy”. | _The researcher action implied by the geometric classification. Derived from DistortionType: type A (full reversal) → 'stratify-immediately' — the pooled conclusion is directionally wrong; do not act on it without stratifying. Type B (partial sign-flip) → 'investigate-confounder' — the equal-weight signal opposes the pooled signal, but the per-stratum picture is mixed; the causal mechanism needs examination before trusting either estimate. Type C (compression without flip) → 'check-allocation-bias' — the pooled winner is probably correct but the effect size is distorted; report adjusted or standardized margins rather than the raw pooled gap. Type D (neutral) → 'pooled-analysis-trustworthy' — allocation is not materially distorting the signal; the pooled conclusion is safe with respect to this stratification._ |
| Corrected Gap | The same as its weighted stratum gap sum. | _The allocation-corrected treatment gap: WeightedStratumGapSum, i.e. what SignedPooledGap would be if allocation were equal across strata. Positive means A is favoured in the corrected world; negative means B is favoured._ |
| Corrected Winner | Determined by priority: an empty string if the corrected gap is blank; “tie” if the absolute value of the corrected gap is less than 0.0001; the treatment a if the corrected gap is greater than 0; the treatment b if the corrected gap is less than 0; in all other cases, “tie”. | _Which treatment wins once allocation bias is removed. Derived from CorrectedGap: positive → TreatmentA; negative → TreatmentB; near-zero → tie._ |
| Corrected Vs Pooled Agreement | True when the corrected winner is the pooled winner. | _TRUE when the allocation-corrected winner matches the pooled winner. FALSE when removing allocation bias would change which treatment appears to win — the machine-readable definition of a reversal recovery._ |
| Corrected Policy Implication | Determined by priority: an empty string if the distortion type is blank; “use-corrected-winner” if the distortion type is “A”; “use-corrected-winner-with-caution” if the distortion type is “B”; “check-allocation-bias” if at least one of the following holds: the distortion type is “C+” or the distortion type is “C-”; in all other cases, “pooled-analysis-trustworthy”. | _The allocation-aware researcher action: what to do when the corrected verdict is available. Derived from DistortionType and CorrectedWinner together, making the instrument self-consistent with Reversal Recovery (loop-27). Type A (full sign-flip, unanimous per-stratum reversal): CorrectedWinner is the true signal — 'use-corrected-winner'. The pooled analysis was directionally wrong; act on CorrectedWinner, not PooledWinner. Type B (partial sign-flip, non-unanimous): CorrectedWinner points against the pooled signal but strata disagree among themselves — 'use-corrected-winner-with-caution'. Trust the corrected direction but acknowledge residual uncertainty. Type C (compression, no sign flip): PooledWinner is directionally correct; the distortion is in the magnitude, not the direction — 'check-allocation-bias'. Same as PolicyImplication. Type D (neutral): allocation is not materially distorting the signal — 'pooled-analysis-trustworthy'. Same as PolicyImplication. For Types C and D, CorrectedPolicyImplication and PolicyImplication always agree; for Types A and B, CorrectedPolicyImplication supersedes PolicyImplication with the allocation-corrected verdict._ |
| Allocation Direction | Determined by priority: an empty string if the corrected gap is blank; “reversal” if the sign flip flag is set; “amplification” if the absolute value of the signed pooled gap is greater than the absolute value of the corrected gap plus 0.001; “compression” if the absolute value of the signed pooled gap is less than the absolute value of the corrected gap minus 0.001; in all other cases, “neutral”. | _Whether the allocation is reversing, amplifying, or compressing the true treatment signal (CorrectedGap). 'reversal': allocation changed the sign of the pooled gap relative to CorrectedGap — worst case; pooled winner is wrong (types A and B). 'amplification': allocation inflated the pooled gap beyond CorrectedGap in the same direction — pooled winner is correct but effect size is overstated (type C+). 'compression': allocation shrank the pooled gap below CorrectedGap in the same direction — pooled winner is correct but effect size is understated (type C-). 'neutral': allocation had negligible effect on the pooled gap (type D). Policy consequences: reversal → wrong decision; amplification → overconfidence; compression → underconfidence / premature abandonment; neutral → trustworthy._ |
| Signal Purity | Determined by priority: an empty string if the corrected gap is blank; 1 if the absolute value of the corrected gap plus the allocation distortion is 0; in all other cases, the absolute value of the corrected gap divided by the absolute value of the corrected gap plus the allocation distortion. | _The fraction of total observed variation (\|CorrectedGap\| + AllocationDistortion) that is genuine treatment signal rather than allocation noise. SignalPurity = \|CorrectedGap\| / (\|CorrectedGap\| + AllocationDistortion). Range [0,1]: 1.0 = allocation contributed nothing (type D); 0 = corrected gap is zero (pure noise). Critical threshold is 0.5: SignalPurity < 0.5 means allocation noise exceeds true signal. The signal-purity theorem (loop-44): AllocationDirection='reversal' → SignalPurity < 0.5. Equivalently: a sign flip requires the confound to produce more distortion than the treatment produced signal. This is the mechanistic account of why reversals happen: not study-design accidents but algebraic necessities._ |
| Pooled Gap Crosses Zero | True when the value of LOOKUP(Study, SweepStudySummary[SweepStudyId], SweepStudySummary[PooledGapCrossesZero]). | _Lookup: SweepStudySummary.PooledGapCrossesZero via Study. TRUE when reweighting allocation would change the sign of the pooled gap._ |
| Sweep Pooled Gap Range | Computed as the value of LOOKUP(Study, SweepStudySummary[SweepStudyId], SweepStudySummary[SweepPooledGapRange]). | _Lookup: SweepStudySummary.SweepPooledGapRange via Study. Width of pooled-gap wander under allocation sweep._ |
| Latent Flip Potential | True when all of the following hold: the distortion type is “D” and the pooled gap crosses zero is true. | _TRUE when DistortionType=D but PooledGapCrossesZero=TRUE — SAFE at observed allocation but flip-capable under counterfactual reweighting._ |
| Allocation Fragility | Determined by priority: an empty string if at least one of the following holds: the sweep pooled gap range is blank; the pooled gap is blank; or the pooled gap is 0; in all other cases, the sweep pooled gap range divided by the absolute value of the pooled gap. | _SweepPooledGapRange divided by \|PooledGap\|. High values mean small observed effect but wide counterfactual wander._ |
| Study Domain | Computed as the value of LOOKUP(Study, Studies[StudyId], Studies[Domain]). | _Lookup: Studies.Domain via Study._ |
| Stratum Causal Role | The value of LOOKUP(Study, StratumVariables[Study], StratumVariables[CausalRole]). | _Lookup: StratumVariables.CausalRole for this study — confounder, mediator, collider, selection, contested, etc._ |
| Is Latent Only Flip | True when all of the following hold: the pooled gap crosses zero is true and the is sign flip is false. | _TRUE when allocation sweep crosses zero but observed allocation shows no sign flip — latent-only reversal._ |
| Confirmed Causal Role Count | The number of the treatment ranking's stratum variables that have a causal role of “confounder”. | _Count of StratumVariables in this study whose CausalRole is "confounder" — includes variables that confound the outcome regardless of whether allocation was balanced._ |
| Mediator Risk Count | The number of the treatment ranking's mediator risks. | _Count of StratumVariables in this study whose ConditioningRisk is mediator or collider — the hazardous conditioning types._ |
| Contested Stratum Count | The number of the treatment ranking's stratum variables that have a causal role of “contested”. | _Count of StratumVariables in this study whose CausalRole is contested — the causal interpretation is disputed._ |
| Unknown Causal Role Count | The number of the treatment ranking's stratum variables that have a causal role of “unknown”. | _Count of StratumVariables in this study whose CausalRole is unknown — no causal account has been established._ |
| Causal Claim Status | Determined by priority: “contested” if the contested stratum count is greater than 0; “geometric-only” if the unknown causal role count is greater than 0; “established” if the confirmed causal role count is greater than 0; in all other cases, “geometric-only”. | _The epistemic status of the causal interpretation for this study. Values: established (all stratum variables are confirmed confounders with known mechanisms), contested (at least one stratum variable has a disputed causal role — e.g. Berkeley 1973 where department choice may be endogenous), geometric-only (no stratum variable has an established causal role — the distortion is a geometric fact, not a causal claim). Derived from StratumVariables.CausalRole and ConditioningRisk._ |
| Adjustment Appropriate | True when all of the following hold: the mediator risk count is 0 and the causal claim status is “established”. | _TRUE only when it is epistemically safe to act on CorrectedGap as the causal treatment effect estimate: no mediator/collider conditioning risk AND causal claim is established (not contested or geometric-only). FALSE for the birth-weight paradox (mediator), berkeley-1973 (contested), and synthetic/degenerate studies (geometric-only)._ |
| **Invariant Check** | An invariant check is identified by its name. | — |
| Universe Count | Computed as the pass count plus the fail count. | _Total rows in scope (PassCount + FailCount)._ |
| Is Green | True when the fail count is 0. | _TRUE when FailCount = 0._ |
| Status Label | Determined by priority: “PASS” if the green flag is set; in all other cases, “FAIL(”, followed by the fail count, followed by “)”. | _PASS or FAIL(n) summary for display._ |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Key Claim Summary | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim]). | _Lookup: ResearchTraditions.KeyClaim via TraditionId — a failed invariant would falsify this tradition's key claim._ |
| Protects Conclusion Title | The value of LOOKUP(ProtectsConclusion, Conclusions[ConclusionId], Conclusions[Title]). | _Lookup: Conclusions.Title via ProtectsConclusion._ |
| Protects Conclusion Category | The value of LOOKUP(ProtectsConclusion, Conclusions[ConclusionId], Conclusions[Category]). | _Lookup: Conclusions.Category via ProtectsConclusion._ |
| **Methodology** | A methodology is identified by its name. | — |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Primary Venue | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[PrimaryVenue]). | _Lookup: ResearchTraditions.PrimaryVenue via TraditionId._ |
| Pioneering Researcher Name | The value of LOOKUP(PioneeringResearcher, Researchers[ResearcherId], Researchers[Name]). | _Lookup: Researchers.Name via PioneeringResearcher._ |
| Pioneering Researcher Affiliation | The value of LOOKUP(PioneeringResearcher, Researchers[ResearcherId], Researchers[Affiliation]). | _Lookup: Researchers.Affiliation via PioneeringResearcher._ |
| Loop Count | The number of loops related to the methodology. | _Count of Loops whose TraditionId matches this methodology entry's TraditionId — how many build iterations this methodological tradition has driven._ |
| **Conclusion** | A conclusion is identified by its name. | — |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Key Claim Summary | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim]). | _Lookup: ResearchTraditions.KeyClaim via TraditionId — shows which tradition's central claim this conclusion validates or refines._ |
| Researcher Name | The value of LOOKUP(ResearcherId, Researchers[ResearcherId], Researchers[Name]). | _Lookup: Researchers.Name via ResearcherId._ |
| Researcher Affiliation | The value of LOOKUP(ResearcherId, Researchers[ResearcherId], Researchers[Affiliation]). | _Lookup: Researchers.Affiliation via ResearcherId._ |
| Challenges Researcher Name | The value of LOOKUP(ChallengesResearcher, Researchers[ResearcherId], Researchers[Name]). | _Lookup: Researchers.Name via ChallengesResearcher._ |
| Witnessed in Loop Title | The value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[Title]). | _Lookup: Loops.Title via WitnessedInLoop._ |
| Witnessed in Loop Commit Hash | The value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitHash]). | _Lookup: Loops.CommitHash via WitnessedInLoop — git checkout this SHA to replay the instrument state when this conclusion was first witnessed._ |
| Witnessed in Loop Commit Short | The value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitShort]). | _Lookup: Loops.CommitShort via WitnessedInLoop._ |
| Witnessed in Loop Commit Date | The value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitDate]). | _Lookup: Loops.CommitDate via WitnessedInLoop._ |
| Witnessed in Loop Git Tag | The value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[GitTag]). | _Lookup: Loops.GitTag via WitnessedInLoop — preferred replay entry point when a milestone tag exists._ |
| Invariant Protecting Count | The number of invariant checks related to the conclusion. | _Count of InvariantChecks whose ProtectsConclusion FK points to this conclusion — how many algebraic invariants stand behind this finding._ |
| **UI Screen** | A UI screen is identified by its name. | — |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Core Conern | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern]). | _Lookup: ResearchTraditions.CoreConcern via TraditionId — the lens this screen applies._ |
| Primary Conclusion Title | The value of LOOKUP(PrimaryConclusion, Conclusions[ConclusionId], Conclusions[Title]). | _Lookup: Conclusions.Title via PrimaryConclusion._ |
| Primary Conclusion Evidence | The value of LOOKUP(PrimaryConclusion, Conclusions[ConclusionId], Conclusions[Evidence]). | _Lookup: Conclusions.Evidence via PrimaryConclusion._ |
| **UI Component** | A UI component is identified by its name. | — |
| **Instrument Spec** | An instrument spec is identified by its name. | — |
| **Allocation Sweep** | An allocation sweep is identified by its name. | — |
| Is Original | True when the value of ABS(AllocFractionA - LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[OriginalAllocFractionA])) < 0.03. | _TRUE for the row closest to the actual original study allocation._ |
| N Sweep Stratum Total | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NSweepStratumTotal]). | _Total cases in the sweep stratum (the stratum whose allocation is being varied). Fixed per study._ |
| N Sweep a | Computed as the n sweep stratum total times the alloc fraction a rounded to the nearest whole number. | _Cases in the sweep stratum assigned to treatment A = ROUND(NSweepStratumTotal * AllocFractionA)._ |
| N Sweep B | Computed as the n sweep stratum total minus the n sweep a. | _Cases in the sweep stratum assigned to treatment B = NSweepStratumTotal - NSweepA._ |
| N Fixed a | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NFixedA]). | _Total A-treatment cases in all NON-swept strata (held fixed across sweep rows). For kidney-1986 = 87 (small-stratum A). For berkeley-1973 = 1302 (depts B+C+D male). For compressed-synthetic = 80 (easy-stratum A). For balanced-synthetic = 100 (hard-stratum A)._ |
| N Fixed B | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NFixedB]). | _Total B-treatment cases in all NON-swept strata (held fixed across sweep rows). For kidney-1986 = 270 (small-stratum B). For berkeley-1973 = 993 (depts B+C+D female). For compressed-synthetic = 120 (easy-stratum B). For balanced-synthetic = 100 (hard-stratum B)._ |
| Sweep Rate a | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepRateA]). | _Per-cell success rate for treatment A in the sweep stratum. Fixed per study._ |
| Sweep Rate B | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepRateB]). | _Per-cell success rate for treatment B in the sweep stratum. Fixed per study._ |
| Fixed Rate a | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[FixedRateA]). | _Weighted success rate for treatment A across all fixed (non-swept) strata. Constant per study._ |
| Fixed Rate B | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[FixedRateB]). | _Weighted success rate for treatment B across all fixed (non-swept) strata. Constant per study._ |
| Sweep Pooled Rate a | Determined by priority: an empty string if the n sweep a plus the n fixed a is 0; in all other cases, the n sweep a times the sweep rate a plus the n fixed a times the fixed rate a divided by the n sweep a plus the n fixed a. | _Pooled success rate for treatment A = (NSweepA * SweepRateA + NFixedA * FixedRateA) / (NSweepA + NFixedA). Changes as AllocFractionA varies._ |
| Sweep Pooled Rate B | Determined by priority: an empty string if the n sweep b plus the n fixed b is 0; in all other cases, the n sweep b times the sweep rate b plus the n fixed b times the fixed rate b divided by the n sweep b plus the n fixed b. | _Pooled success rate for treatment B = (NSweepB * SweepRateB + NFixedB * FixedRateB) / (NSweepB + NFixedB). Changes as AllocFractionA varies._ |
| Sweep Pooled Gap | Determined by priority: an empty string if the sweep pooled rate a is blank; in all other cases, the sweep pooled rate a minus the sweep pooled rate b. | _SweepPooledRateA minus SweepPooledRateB. Wanders as AllocFractionA varies because pooled rates mix stratum sizes unequally. Crosses zero at some allocation (the paradox reversal point)._ |
| Sweep Corrected Gap | Computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepCorrectedGap]). | _Allocation-free signal: sum over strata of (n_s/N) * (rate_As - rate_Bs). Constant across the sweep by construction — the allocation variable does not appear. This is the conserved quantity under re-stratification._ |
| Allocation Distortion Witness | Determined by priority: an empty string if the sweep pooled gap is blank; in all other cases, the sweep pooled gap minus the sweep corrected gap. | _SweepPooledGap minus SweepCorrectedGap. The allocation distortion: how much the pooled signal has been inflated or deflated by differential stratum allocation. Zero only when allocation is balanced._ |
| **Sweep Study Summary** | A sweep study summary is identified by its name. | — |
| Distortion Type Label | Computed as the value of LOOKUP(SweepStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). | _DistortionType for this study (A, B, C, or D) — provides context for what the sweep should show._ |
| Corrected Gap Constant | The smallest sweep corrected gap across the allocation sweep related to the sweep study summary. | _The allocation-free corrected gap for this study, computed analytically._ |
| Sweep Corrected Gap Max | The largest sweep corrected gap across the allocation sweep related to the sweep study summary. | _Maximum SweepCorrectedGap across all allocation fractions for this study._ |
| Sweep Corrected Gap Min | The smallest sweep corrected gap across the allocation sweep related to the sweep study summary. | _Minimum SweepCorrectedGap across all allocation fractions for this study._ |
| Sweep Corrected Gap Range | Computed as the sweep corrected gap max minus the sweep corrected gap min. | _Range of SweepCorrectedGap across the sweep = Max - Min. Should be effectively zero (< 0.0001) for all studies if CorrectedGap is truly allocation-free._ |
| Sweep Pooled Gap Max | The largest sweep pooled gap across the allocation sweep related to the sweep study summary. | _Maximum SweepPooledGap across all allocation fractions for this study._ |
| Sweep Pooled Gap Min | The smallest sweep pooled gap across the allocation sweep related to the sweep study summary. | _Minimum SweepPooledGap across all allocation fractions for this study._ |
| Sweep Pooled Gap Range | Computed as the sweep pooled gap max minus the sweep pooled gap min. | _Range of SweepPooledGap across the sweep = Max - Min. Should be large for Type A/B studies (pooled gap wanders) and smaller for Type D (balanced allocation = less wandering)._ |
| Pooled Gap Crosses Zero | True when all of the following hold: the sweep pooled gap min is less than 0 and the sweep pooled gap max is greater than 0. | _TRUE when SweepPooledGapMin < 0 and SweepPooledGapMax > 0 — the pooled gap changes sign somewhere in the sweep, meaning the paradox can be created or destroyed by pure allocation._ |
| Invariant Witness | Determined by priority: “PASS: CorrectedGap invariant across allocation sweep” if the sweep corrected gap range is less than 0.0001; in all other cases, “FAIL: CorrectedGap varies — formula error”. | _Summary witness statement: PASS if CorrectedGap is invariant (range < 0.0001), FAIL otherwise._ |
| Sweep Stratum Label | Computed as the value of LOOKUP(SweepStudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepStratumLabel]). | _Which stratum is being swept for this study._ |
| **Research Tradition** | A research tradition is identified by its name. | — |
| Illustrated by Study Title | The value of LOOKUP(IllustratedByStudy, Studies[StudyId], Studies[Title]). | _Lookup: Studies.Title via IllustratedByStudy._ |
| Illustrated by Study Source | The value of LOOKUP(IllustratedByStudy, Studies[StudyId], Studies[Source]). | _Lookup: Studies.Source via IllustratedByStudy — the citation for the study that best illustrates this tradition._ |
| Illustrated by Study Distortion Type | The value of LOOKUP(IllustratedByStudy, TreatmentRankings[Study], TreatmentRankings[DistortionType]). | _Lookup: TreatmentRankings.DistortionType for the illustrated study._ |
| Supporting Conclusion Title | The value of LOOKUP(SupportingConclusion, Conclusions[ConclusionId], Conclusions[Title]). | _Lookup: Conclusions.Title via SupportingConclusion._ |
| Supporting Conclusion Category | The value of LOOKUP(SupportingConclusion, Conclusions[ConclusionId], Conclusions[Category]). | _Lookup: Conclusions.Category via SupportingConclusion._ |
| Researcher Count | The number of researchers related to the research tradition. | _Count of researchers in this tradition._ |
| Study Count | The number of studies related to the research tradition. | _Count of studies in this model whose TraditionId matches this tradition._ |
| Loop Count | The number of loops related to the research tradition. | _Count of Leopold Loops that drew primarily on this tradition's conceptual vocabulary._ |
| **Researcher** | A researcher is identified by its name. | — |
| Tradition Name | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). | _Lookup: ResearchTraditions.Name via TraditionId._ |
| Tradition Era | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Era]). | _Lookup: ResearchTraditions.Era via TraditionId._ |
| Canonical Study Title | The value of LOOKUP(CanonicalStudyId, Studies[StudyId], Studies[Title]). | _Lookup: Studies.Title via CanonicalStudyId._ |
| Canonical Study Distortion Type | The value of LOOKUP(CanonicalStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). | _Lookup: TreatmentRankings.DistortionType for the canonical study — the geometric type of paradox this researcher's work exemplifies._ |
| Canonical Study Policy Implication | The value of LOOKUP(CanonicalStudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedPolicyImplication]). | _Lookup: TreatmentRankings.CorrectedPolicyImplication for the canonical study._ |
| Illustrates Conclusion Title | The value of LOOKUP(IllustratesConclusion, Conclusions[ConclusionId], Conclusions[Title]). | _Lookup: Conclusions.Title via IllustratesConclusion._ |
| Tradition Key Claim Summary | The value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim]). | _Lookup: ResearchTraditions.KeyClaim via TraditionId — the tradition's signature thesis this researcher contributed to._ |
| **Synthetic Phase** | A synthetic phase is identified by its name. | — |
| Phase S1 Total | Computed as 200 times the param stratum fraction rounded to the nearest whole number. | _Total cases in stratum 1 = ROUND(200 × ParamStratumFraction)._ |
| Phase NA1 | Computed as the phase s1 total times 0.5 plus the param allocation bias divided by 2 rounded to the nearest whole number. | _Treatment-A cases in stratum 1._ |
| Phase NB1 | Computed as the phase s1 total minus the phase NA1. | _Treatment-B cases in stratum 1._ |
| Phase NA2 | Computed as 100 minus the phase NA1. | _Treatment-A cases in stratum 2._ |
| Phase NB2 | Computed as 100 minus the phase NB1. | _Treatment-B cases in stratum 2._ |
| Phase Rate A1 | Computed as 0.45 plus the param stratum gap1 divided by 2. | _Success rate for A in hard stratum._ |
| Phase Rate B1 | Computed as 0.45 minus the param stratum gap1 divided by 2. | _Success rate for B in hard stratum._ |
| Phase Rate A2 | Computed as 0.75 plus the param stratum gap2 divided by 2. | _Success rate for A in easy stratum._ |
| Phase Rate B2 | Computed as 0.75 minus the param stratum gap2 divided by 2. | _Success rate for B in easy stratum._ |
| Phase Pooled Rate a | Computed as the phase NA1 times the phase rate a1 plus the phase NA2 times the phase rate a2 divided by 100. | _Pooled success rate for treatment A._ |
| Phase Pooled Rate B | Computed as the phase NB1 times the phase rate b1 plus the phase NB2 times the phase rate b2 divided by 100. | _Pooled success rate for treatment B._ |
| Phase Signed Pooled Gap | Computed as the phase pooled rate a minus the phase pooled rate b. | _Pooled rate gap (A minus B)._ |
| Phase Corrected Gap | Computed as the param stratum fraction times the param stratum gap1 plus 1 minus the param stratum fraction times the param stratum gap2. | _Allocation-free equal-weight gap = w×g1 + (1−w)×g2._ |
| Phase Strata Won by Loser | Determined by priority: the count of the following that hold: the param stratum gap1 is less than 0 and the param stratum gap2 is less than 0 if the phase signed pooled gap is greater than 0; in all other cases, the count of the following that hold: the param stratum gap1 is greater than 0 and the param stratum gap2 is greater than 0. | _Count of strata whose per-stratum winner disagrees with the pooled winner._ |
| Phase Reversal Intensity | Computed as the phase strata won by loser divided by 2. | _PhaseStrataWonByLoser / 2 for this 2-stratum template._ |
| Phase is Sign Flip | True when the phase signed pooled gap is less than 0 if the phase corrected gap is greater than 0, in all other cases the phase signed pooled gap is greater than 0. | _TRUE when equal-weight and pooled gaps have opposite signs._ |
| Phase Allocation Distortion | Computed as the absolute value of the phase corrected gap minus the phase signed pooled gap. | _\|PhaseCorrectedGap − PhaseSignedPooledGap\|._ |
| Phase Distortion Type | Determined by priority: “A” if all of the following hold: the phase is sign flip flag is set and the phase reversal intensity is 1; “B” if all of the following hold: the phase is sign flip flag is set and the phase reversal intensity is less than 1; “C+” if all of the following hold: it is not the case that the phase is sign flip flag is set; the phase allocation distortion is greater than 0.01; and the absolute value of the phase signed pooled gap is greater than the absolute value of the phase corrected gap plus 0.001; “C-” if all of the following hold: it is not the case that the phase is sign flip flag is set; the phase allocation distortion is greater than 0.01; and the absolute value of the phase signed pooled gap is less than the absolute value of the phase corrected gap minus 0.001; in all other cases, “D”. | _Five-type geometric classification at this parameter point._ |
| **Phase Diagram Summary** | A phase diagram summary is identified by its name. | — |
| Phase Point Count | Computed as the phase type a count plus the phase type b count plus the phase type c plus count plus the phase type c minus count plus the phase type d count. | _Total grid points in SyntheticPhase (sum of type counts). Refreshed from vw_synthetic_phase by 05b-customize-data.sql._ |
| All Five Types Present | True when all of the following hold: the phase type a count is greater than 0; the phase type b count is greater than 0; the phase type c plus count is greater than 0; the phase type c minus count is greater than 0; and the phase type d count is greater than 0. | _TRUE when every DistortionType cell has at least one grid point._ |
| Phase Taxonomy Witness | Computed as “A:”, followed by the phase type a count, followed by “ B:”, followed by the phase type b count, followed by “ C+:”, followed by the phase type c plus count, followed by “ C-:”, followed by the phase type c minus count, followed by “ D:”, followed by the phase type d count. | _Human-readable witness string of type counts across the grid._ |
| Phase Witness Note | Determined by priority: “PASS: all five distortion types populated in parameter space” if the all five types present flag is set; in all other cases, “FAIL: missing distortion type in grid”. | _PASS/FAIL summary for taxonomy completeness._ |
| **Ingestion Protocol** | An ingestion protocol is identified by its name. | — |
| **Ingestion Summary** | An ingestion summary is identified by its name. | — |
| Protocol Item Count | The number of ingestion protocol related to the ingestion summary. | _Total contract items._ |
| Corpus Cell Count | The number of case cells related to the ingestion summary. | _Total CaseCell rows._ |
| Valid Cell Count | The number of case cells related to the ingestion summary. | _Valid CaseCells._ |
| All Cells Valid | True when the valid cell count is the corpus cell count. | _Every cell passes validity predicate._ |
| Study Count | The number of studies related to the ingestion summary. | _Total studies._ |
| Structural Compliant Count | The number of studies related to the ingestion summary. | _Studies with IngestionCellParity._ |
| All Studies Structural | True when the structural compliant count is the study count. | _All studies structurally valid._ |
| Real Study Count | The number of studies related to the ingestion summary. | _Non-synthetic studies._ |
| Real Fully Compliant Count | The number of studies related to the ingestion summary. | _Real studies with IngestionCompliance=all._ |
| All Real Studies Compliant | True when the real fully compliant count is the real study count. | _All real studies fully compliant._ |
| Ingestion Contract Passes | True when all of the following hold: the all cells valid flag is set; the all studies structural flag is set; and the all real studies compliant flag is set. | _Full contract passes._ |
| Ingestion Witness Note | Determined by priority: “PASS: ”, followed by the protocol item count, followed by “ contract items; ”, followed by the corpus cell count, followed by “ cells valid; ”, followed by the real fully compliant count, followed by a slash, followed by the real study count, followed by “ real studies fully compliant” if the ingestion contract passes flag is set; in all other cases, “FAIL: ingestion contract violated”. | _PASS/FAIL summary._ |
| **Candidate Study Catalog** | A candidate study catalog is identified by its name. | — |
| Is Ready to Encode | True when all of the following hold: the ingestion status is “candidate”; the proposed study ID has a value; the citation has a value; and the stratum variable name has a value. | _TRUE when candidate has citation, proposed id, and stratum variable — ready for an import session._ |
| Is Imported | True when the ingestion status is “imported”. | _TRUE when already in corpus._ |
| Observed Distortion Type | The value of LOOKUP(LinkedStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). | _Actual DistortionType from TreatmentRankings when imported._ |
| Type Prediction Match | True when the imported flag is set and the observed distortion type is the expected distortion type. | _TRUE when observed type matches expected (imported rows only)._ |
| Expected Sign Flip | True when at least one of the following holds: the expected distortion type is “A” or the expected distortion type is “B”. | _TRUE when curator tagged ExpectedDistortionType as A or B (sign-flip expectation)._ |
| Observed Sign Flip Type | True when the imported flag is set and at least one of the following holds: the observed distortion type is “A” or the observed distortion type is “B”. | _TRUE when observed DistortionType is A or B (imported rows only)._ |
| Sign Flip Prediction Match | True when all of the following hold: the imported flag is set and the expected sign flip flag is set and the observed sign flip type flag is set. | _TRUE when Expected A∪B and observed is A∪B — weaker flip-type prediction test._ |
| Is Data Ready | True when at least one of the following holds: the data acquisition status is “downloaded” or the data acquisition status is “manual_only”. | _TRUE when stratified data is downloaded or available from published tables._ |
| **Corpus Catalog Summary** | A corpus catalog summary is identified by its name. | — |
| Total Catalog Entries | The number of candidate study catalog related to the corpus catalog summary. | _All catalog rows._ |
| Imported Count | The number of candidate study catalog related to the corpus catalog summary. | _Rows already imported._ |
| Candidate Count | The number of candidate study catalog related to the corpus catalog summary. | _Rows queued for import._ |
| Blocked Count | The number of candidate study catalog related to the corpus catalog summary. | _Duplicate or blocked rows._ |
| Ready to Encode Count | The number of candidate study catalog related to the corpus catalog summary. | _Candidates passing IsReadyToEncode._ |
| High Priority Count | The number of the corpus catalog summary's high priorities. | _Candidates with Priority <= 2._ |
| Import Session Ready | True when all of the following hold: the candidate count is at least 20 and the ready to encode count is at least 15. | _TRUE when backlog is large enough for a bulk import session._ |
| Type Prediction Match Count | The number of candidate study catalog related to the corpus catalog summary. | _Imported catalog rows where ObservedDistortionType exactly matches ExpectedDistortionType._ |
| Type Prediction Mismatch Count | The number of the corpus catalog summary's candidate study catalog that are not type prediction match. | _Imported catalog rows where exact type prediction failed._ |
| Sign Flip Prediction Eligible Count | The number of the corpus catalog summary's candidate study catalog that are expected sign flip. | _Imported rows where ExpectedDistortionType was A or B._ |
| Sign Flip Prediction Match Count | The number of candidate study catalog related to the corpus catalog summary. | _Expected A∪B rows where observed type is also A∪B._ |
| Type Prediction Match Rate | Determined by priority: an empty string if the imported count is 0; in all other cases, the type prediction match count divided by the imported count. | _Exact-match rate across imported catalog rows._ |
| Sign Flip Prediction Match Rate | Determined by priority: an empty string if the sign flip prediction eligible count is 0; in all other cases, the sign flip prediction match count divided by the sign flip prediction eligible count. | _Sign-flip-type match rate among Expected A∪B rows._ |
| Catalog Witness Note | Determined by priority: “READY: ”, followed by the ready to encode count, followed by “ encode-ready (”, followed by the data ready count, followed by “ data-ready) of ”, followed by the candidate count, followed by “ candidates” if the import session ready flag is set; in all other cases, “BUILDING: ”, followed by the candidate count, followed by “ candidates — ”, followed by the data ready count, followed by “ data-ready, ”, followed by the ready to encode count, followed by “ encode-ready”. | _Human-readable readiness summary._ |
| Catalog Prediction Witness Note | Computed as the value of CONCAT("exact=", TypePredictionMatchCount, "/", ImportedCount, " (", ROUND(TypePredictionMatchRate * 100, 1), "%); flipPred=", SignFlipPredictionMatchCount, "/", SignFlipPredictionEligibleCount, " (", ROUND(SignFlipPredictionMatchRate * 100, 1), "%)")). | _Loop-65 type-prediction audit summary._ |
| Data Ready Count | The number of the corpus catalog summary's candidate study catalog that have a ingestion status of “candidate”. | _Candidates with IsDataReady=TRUE._ |
| Encode Pipeline Ready Count | The number of the corpus catalog summary's candidate study catalog that are data readies. | _Candidates both data-ready and encode-ready._ |
| Expansion Candidate Count | The number of candidate study catalog related to the corpus catalog summary. | _Loop-67 expansion-wave-1 candidates._ |
| **Domain Expansion Target** | A domain expansion target is identified by its name. | — |
| Current Imported Count | The number of the domain expansion target's studies that are not synthetic. | _Real studies in corpus matching LegacyDomainAlias (or Domain when no alias)._ |
| Candidate Queued Count | The number of the domain expansion target's candidate study catalog that have a ingestion status of “candidate”. | _Catalog candidates in this domain._ |
| Projected Count | Computed as the current imported count plus the candidate queued count. | _Current + queued candidates._ |
| Gap Count | Computed as the largest of 0 and the target min count minus the projected count. | _Studies still needed to hit target after queued imports._ |
| Is Under Represented | True when the projected count is less than the target min count. | _TRUE when projected count still below target._ |
| **Study Import Template** | A study import template is identified by its name. | — |
| **Sweep Study Config** | A sweep study config is identified by its name. | — |
| **Substrate Conformance Field** | A substrate conformance field is identified by its name. | — |
| **Discovery Hypothes** | A discovery hypothes is identified by its name. | — |
| **Discovery Finding** | A discovery finding is identified by its name. | — |
| Hypothesis Statement | The value of LOOKUP(HypothesisId, DiscoveryHypotheses[HypothesisId], DiscoveryHypotheses[Statement]). | _Lookup: DiscoveryHypotheses.Statement._ |
| Observed Metric | Computed as the value of IF(HypothesisId = "H-latent-d", CONCAT("fraction=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[LatentTypeDFraction])), IF(HypothesisId = "H-purity", CONCAT("maxPurity=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SignFlipSignalPurityMax])), IF(HypothesisId = "H-small-effect", CONCAT("stable=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapStableD]), " latent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapLatentD])), IF(HypothesisId = "H-econ-zero", CONCAT("flips=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EconomicsSignFlipCount])), IF(HypothesisId = "H-domain-dist", CONCAT("epi=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EpidemiologyAvgDistortion]), " edu=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EducationAvgDistortion])), IF(HypothesisId = "H-causal-manifest", CONCAT("confLatent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderLatentOnlyCount]), " confFlip=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount])), IF(HypothesisId = "H-causal-latent", CONCAT("collManifest=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]), " collLatent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionLatentOnlyCount]), " collN=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionCount])), IF(HypothesisId = "H-explained-confounder", CONCAT("ExplainedConfounderCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]), ", ConfounderSignFlipCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount])), IF(HypothesisId = "H-unexplained-nonconfounder", CONCAT("ContestedOrMediatorExplainedCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ContestedOrMediatorExplainedCount])), "")))))). | _Witnessed metric value from ModelSummary for this hypothesis._ |
| Is Confirmed | True when the value of IF(HypothesisId = "H-latent-d", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[LatentTypeDFraction]) > 0.5, IF(HypothesisId = "H-purity", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SignFlipSignalPurityMax]) < 0.5, IF(HypothesisId = "H-small-effect", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapStableD]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapLatentD]), IF(HypothesisId = "H-econ-zero", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EconomicsSignFlipCount]) = 0, IF(HypothesisId = "H-domain-dist", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EpidemiologyAvgDistortion]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EducationAvgDistortion]), IF(HypothesisId = "H-causal-manifest", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]), LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]) >= 10), IF(HypothesisId = "H-causal-latent", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]) = 0, LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionLatentOnlyCount]) >= 5), IF(HypothesisId = "H-explained-confounder", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]) = LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]), LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]) >= 10), IF(HypothesisId = "H-unexplained-nonconfounder", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ContestedOrMediatorExplainedCount]) = 0, FALSE()))))). | _TRUE when the witnessed metric satisfies the pre-registered ExpectedOutcome._ |
| Evidence | Determined by priority: “PASS: ”, followed by the observed metric if the is confirmed is true; “FAIL: ”, followed by the observed metric if the is confirmed is false; in all other cases, an empty string. | _Pass/fail witness string for UI and conclusions._ |
| **Corpus Domain** | A corpus domain is identified by its name. | — |

## 2 Fact Types

- a **treatment** references exactly one **study**
- a **strata** references exactly one **study**
- a **case cell** references exactly one **study**
- a **stratum summary** references exactly one **study**
- a **stratum variable** references exactly one **study**
- a **treatment ranking** references exactly one **study**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A loop **must** have a title and a status.
- A treatment **must** reference exactly one study.
- A treatment **must** have a treatment label.
- A strata **must** reference exactly one study.
- A strata **must** have a stratum label.
- A case cell **must** reference exactly one study.
- A case cell **must** have a stratum label, a treatment label, a successes, and a cases.
- A stratum summary **must** reference exactly one study.
- A stratum summary **must** have a stratum label and a treatment label.
- A stratum variable **must** reference exactly one study.
- A stratum variable **must** have a variable name and a causal role, and record whether it is affects treatment assignment and whether it is affects outcome.
- A treatment ranking **must** reference exactly one study.
- A treatment ranking **must** have a treatment a and a treatment b.
- An invariant check **must** have an algebraic statement, a natural language, a source table, an assertion expression, an SQL assertion, a pass count, a fail count, and a severity.
- A methodology **must** have a phase, a title, and a statement.
- A conclusion **must** have a category, a status, and a title.
- A UI screen **must** have a title, a route, a primary entity, and a purpose.
- A UI component **must** have a component name, a primary entity, and a description.
- An instrument spec **must** have a component type, a field name, and a natural language.
- An allocation sweep **must** have an alloc fraction a.
- A research tradition **must** record whether it is verified by deep research.
- A researcher **must** have a key contribution, and record whether it is verified by deep research.
- A synthetic phase **must** have a param stratum fraction, a param stratum gap1, a param stratum gap2, and a param allocation bias.
- A phase diagram summary **must** have a phase type a count, a phase type b count, a phase type c plus count, a phase type c minus count, and a phase type d count.
- An ingestion protocol **must** have a rule type, a target table, a requirement statement, a mechanical check, and a sort order.
- A candidate study catalog **must** have a title, a citation, a domain, a stratum variable name, an expected distortion type, an ingestion status, a priority, a stratum count estimate, a data source note, a data acquisition status, a reversal mechanism, and a paradox confirmation.
- A domain expansion target **must** have a domain and a target min count.
- A study import template **must** have a sort order, a target table, a row description, a required fields, and a mechanical check.
- A sweep study config **must** have a sweep stratum label, a n sweep stratum total, a sweep rate a, a sweep rate b, a n fixed a, a n fixed b, a fixed rate a, a fixed rate b, an original alloc fraction a, and a sweep corrected gap.
- A substrate conformance field **must** have a source table, a field name, a pg column, an owl local name, and a data type, and record whether it is assert from postgres and whether it is in compare set.
- A discovery hypothes **must** have a statement, an expected outcome, a registered in loop, and an epistemic tier.
- A discovery finding **must** have a witnessed in loop.
- A corpus domain **must** have a display name, an expansion target, and a plan source section.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Tradition Name** | A loop's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-2 Tradition Core Conern** | A loop's tradition core conern is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern]). |
| **DR-3 Commit Short** | A loop's commit short is computed as the first 7 character(s) of the commit hash. |
| **DR-4 Total Cases** | A study's total cases is the total cases across the case cells related to the study. |
| **DR-5 Cell Count** | A study's cell count is the number of case cells related to the study. |
| **DR-6 Tradition Name** | A study's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-7 Tradition Core Conern** | A study's tradition core conern is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern]). |
| **DR-8 Primary Researcher Name** | A study's primary researcher name is the value of LOOKUP(PrimaryResearcherId, Researchers[ResearcherId], Researchers[Name]). |
| **DR-9 Primary Researcher Affiliation** | A study's primary researcher affiliation is the value of LOOKUP(PrimaryResearcherId, Researchers[ResearcherId], Researchers[Affiliation]). |
| **DR-10 Distortion Type** | A study's distortion type is the value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). |
| **DR-11 Policy Implication** | A study's policy implication is the value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedPolicyImplication]). |
| **DR-12 Allocation Distortion** | A study's allocation distortion is the value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[AllocationDistortion]). |
| **DR-13 Signal Purity** | A study's signal purity is the value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[SignalPurity]). |
| **DR-14 Is Sign Flip** | A study's is sign flip is the value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[IsSignFlip]). |
| **DR-15 Corrected Winner** | A study's corrected winner is the value of LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedWinner]). |
| **DR-16 Confounding Variable** | A study's confounding variable is the value of LOOKUP(StudyId, StratumVariables[Study], StratumVariables[VariableName]). |
| **DR-17 Causal Role** | A study's causal role is the value of LOOKUP(StudyId, StratumVariables[Study], StratumVariables[CausalRole]). |
| **DR-18 Ingestion Cell Parity** | A study is flagged ingestion cell parity if the value of AND(LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[StratumCount]) >= 2, CellCount = LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[StratumCount]) * 2). |
| **DR-19 Ingestion Compliance** | A study's ingestion compliance is computed as the value of IF(IsSynthetic, "exempt-synthetic", IF(AND(IngestionCellParity, LOOKUP(StudyId, StratumVariables[Study], StratumVariables[VariableName]) <> ""), "all", "partial")). |
| **DR-20 Total Cases** | A treatment's total cases is the total cases across the case cells related to the treatment. |
| **DR-21 Total Successes** | A treatment's total successes is the total successes across the case cells related to the treatment. |
| **DR-22 Pooled Success Rate** | The treatment's pooled success rate is determined by the following priority:<br>1. an empty string, if the total cases is 0;<br>2. in all other cases, the total successes divided by the total cases. |
| **DR-23 Total Cases** | A strata's total cases is the total cases across the case cells related to the strata. |
| **DR-24 Cell Success Rate** | The case cell's cell success rate is determined by the following priority:<br>1. an empty string, if the cases is 0;<br>2. in all other cases, the successes divided by the cases. |
| **DR-25 Total Cases for Treatment** | A case cell's total cases for treatment is the total cases across the case cells related to the case cell. |
| **DR-26 Treatment Exposure Fraction** | The case cell's treatment exposure fraction is determined by the following priority:<br>1. an empty string, if the total cases for treatment is 0;<br>2. in all other cases, the cases divided by the total cases for treatment. |
| **DR-27 Is Valid Ingestion Cell** | A case cell is considered a valid ingestion cell if all of the following hold: the cases is greater than 0; the successes is at least 0; and the successes is at most the cases. |
| **DR-28 Stratum Successes** | A stratum summary's stratum successes is the total successes across the case cells related to the stratum summary. |
| **DR-29 Stratum Cases** | A stratum summary's stratum cases is the total cases across the case cells related to the stratum summary. |
| **DR-30 Stratum Success Rate** | The stratum summary's stratum success rate is determined by the following priority:<br>1. an empty string, if the stratum cases is 0;<br>2. in all other cases, the stratum successes divided by the stratum cases. |
| **DR-31 Stratum Successes a** | A stratum summary's stratum successes a is the total successes across the case cells related to the stratum summary. |
| **DR-32 Stratum Cases a** | A stratum summary's stratum cases a is the total cases across the case cells related to the stratum summary. |
| **DR-33 Stratum Rate a** | The stratum summary's stratum rate a is determined by the following priority:<br>1. an empty string, if the stratum cases a is 0;<br>2. in all other cases, the stratum successes a divided by the stratum cases a. |
| **DR-34 Stratum Successes B** | A stratum summary's stratum successes b is the total successes across the case cells related to the stratum summary. |
| **DR-35 Stratum Cases B** | A stratum summary's stratum cases b is the total cases across the case cells related to the stratum summary. |
| **DR-36 Stratum Rate B** | The stratum summary's stratum rate b is determined by the following priority:<br>1. an empty string, if the stratum cases b is 0;<br>2. in all other cases, the stratum successes b divided by the stratum cases b. |
| **DR-37 Stratum Winner** | The stratum summary's stratum winner is determined by the following priority:<br>1. “A”, if the stratum rate a is greater than the stratum rate b;<br>2. in all other cases, “B”. |
| **DR-38 Study Total Cases** | A stratum summary's study total cases is the total cases across the case cells related to the stratum summary. |
| **DR-39 Stratum Total Cases** | A stratum summary's stratum total cases is the total cases across the case cells related to the stratum summary. |
| **DR-40 Stratum Fraction** | The stratum summary's stratum fraction is determined by the following priority:<br>1. an empty string, if the study total cases is 0;<br>2. in all other cases, the stratum total cases divided by the study total cases. |
| **DR-41 Weighted Stratum Rate** | The stratum summary's weighted stratum rate is determined by the following priority:<br>1. an empty string, if the stratum fraction is blank;<br>2. in all other cases, the stratum fraction times the stratum success rate. |
| **DR-42 Treatment a Cases Here** | A stratum summary's treatment a cases here is the total cases across the case cells related to the stratum summary. |
| **DR-43 Treatment B Cases Here** | A stratum summary's treatment b cases here is the total cases across the case cells related to the stratum summary. |
| **DR-44 Treatment a Total Cases** | A stratum summary's treatment a total cases is the total cases across the stratum summary's case cells that have a treatment label of “A”. |
| **DR-45 Treatment B Total Cases** | A stratum summary's treatment b total cases is the total cases across the stratum summary's case cells that have a treatment label of “B”. |
| **DR-46 Allocation Fraction a** | The stratum summary's allocation fraction a is determined by the following priority:<br>1. an empty string, if the treatment a total cases is 0;<br>2. in all other cases, the treatment a cases here divided by the treatment a total cases. |
| **DR-47 Allocation Fraction B** | The stratum summary's allocation fraction b is determined by the following priority:<br>1. an empty string, if the treatment b total cases is 0;<br>2. in all other cases, the treatment b cases here divided by the treatment b total cases. |
| **DR-48 Allocation Bias** | The stratum summary's allocation bias is determined by the following priority:<br>1. an empty string, if the allocation fraction a is blank;<br>2. in all other cases, the allocation fraction a minus the allocation fraction b. |
| **DR-49 Stratum Gap** | The stratum summary's stratum gap is determined by the following priority:<br>1. an empty string, if the stratum rate a is blank;<br>2. in all other cases, the stratum rate a minus the stratum rate b. |
| **DR-50 Weighted Stratum Gap** | The stratum summary's weighted stratum gap is determined by the following priority:<br>1. an empty string, if the stratum gap is blank;<br>2. in all other cases, the stratum gap times the stratum fraction. |
| **DR-51 Reversal Count** | A model summary's reversal count is the number of treatment rankings related to the model summary. |
| **DR-52 Non Reversal Count** | A model summary's non reversal count is the number of treatment rankings related to the model summary. |
| **DR-53 Study Count** | A model summary's study count is computed as the reversal count plus the non reversal count. |
| **DR-54 Explained Count** | A model summary's explained count is the number of treatment rankings related to the model summary. |
| **DR-55 Total Paradox Strength** | A model summary's total paradox strength is the total paradox strength across the treatment rankings related to the model summary. |
| **DR-56 Avg Paradox Strength** | The model summary's avg paradox strength is determined by the following priority:<br>1. an empty string, if the study count is 0;<br>2. in all other cases, the total paradox strength divided by the study count. |
| **DR-57 Type a Count** | A model summary's type a count is the number of treatment rankings related to the model summary. |
| **DR-58 Type B Count** | A model summary's type b count is the number of treatment rankings related to the model summary. |
| **DR-59 Type D Count** | A model summary's type d count is the number of treatment rankings related to the model summary. |
| **DR-60 Type a Fraction** | The model summary's type a fraction is determined by the following priority:<br>1. an empty string, if the study count is 0;<br>2. in all other cases, the type a count divided by the study count. |
| **DR-61 Distortion Taxonomy Coverage** | A model summary's distortion taxonomy coverage is computed as “A:”, followed by the type a count, followed by “ B:”, followed by the type b count, followed by “ C+:”, followed by the c amplification count, followed by “ C-:”, followed by the c compression count, followed by “ D:”, followed by the type d count. |
| **DR-62 Distortion Only Count** | A model summary's distortion only count is computed as the c amplification count plus the c compression count. |
| **DR-63 C Amplification Count** | A model summary's c amplification count is the number of treatment rankings related to the model summary. |
| **DR-64 C Compression Count** | A model summary's c compression count is the number of treatment rankings related to the model summary. |
| **DR-65 Avg Signal Purity** | A model summary's avg signal purity is an aggregated value computed across the model summary's related records. |
| **DR-66 Sweep Corrected Gap Max** | A model summary's sweep corrected gap max is the largest sweep corrected gap across the allocation sweep related to the model summary. |
| **DR-67 Sweep Corrected Gap Min** | A model summary's sweep corrected gap min is the smallest sweep corrected gap across the allocation sweep related to the model summary. |
| **DR-68 Sweep Corrected Gap Range** | The model summary's sweep corrected gap range is determined by the following priority:<br>1. an empty string, if the sweep corrected gap max is blank;<br>2. in all other cases, the sweep corrected gap max minus the sweep corrected gap min. |
| **DR-69 Sweep Pooled Gap Range** | A model summary's sweep pooled gap range is an aggregated value computed across the model summary's related records. |
| **DR-70 Ingestion Protocol Item Count** | A model summary's ingestion protocol item count is the number of ingestion protocol related to the model summary. |
| **DR-71 Corpus Passes Ingestion Contract** | A model summary's corpus passes ingestion contract is the value of LOOKUP("ingestion-v1", IngestionSummary[IngestionSummaryId], IngestionSummary[IngestionContractPasses]). |
| **DR-72 Ingestion Witness Note** | A model summary's ingestion witness note is the value of LOOKUP("ingestion-v1", IngestionSummary[IngestionSummaryId], IngestionSummary[IngestionWitnessNote]). |
| **DR-73 Catalog Entry Count** | A model summary's catalog entry count is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TotalCatalogEntries]). |
| **DR-74 Pending Import Count** | A model summary's pending import count is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CandidateCount]). |
| **DR-75 Ready to Encode Count** | A model summary's ready to encode count is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ReadyToEncodeCount]). |
| **DR-76 Import Session Ready** | A model summary's import session ready is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ImportSessionReady]). |
| **DR-77 Catalog Witness Note** | A model summary's catalog witness note is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogWitnessNote]). |
| **DR-78 Latent Type D Count** | A model summary's latent type d count is the number of treatment rankings related to the model summary. |
| **DR-79 Stable Type D Count** | A model summary's stable type d count is the number of the model summary's treatment rankings that are not latent flip potential. |
| **DR-80 Latent Type D Fraction** | The model summary's latent type d fraction is determined by the following priority:<br>1. an empty string, if the type d count is 0;<br>2. in all other cases, the latent type d count divided by the type d count. |
| **DR-81 Cross Zero Count** | A model summary's cross zero count is the number of treatment rankings related to the model summary. |
| **DR-82 Sign Flip Signal Purity Max** | A model summary's sign flip signal purity max is the largest signal purity across the treatment rankings related to the model summary. |
| **DR-83 Economics Sign Flip Count** | A model summary's economics sign flip count is the number of the model summary's treatment rankings that are sign flips. |
| **DR-84 Avg Pooled Gap Latent D** | The model summary's avg pooled gap latent d is determined by the following priority:<br>1. an empty string, if the latent type d count is 0;<br>2. in all other cases, the average pooled gap across the treatment rankings related to the model summary. |
| **DR-85 Avg Pooled Gap Stable D** | The model summary's avg pooled gap stable d is determined by the following priority:<br>1. an empty string, if the stable type d count is 0;<br>2. in all other cases, the average pooled gap across the model summary's treatment rankings that are not latent flip potential. |
| **DR-86 Epidemiology Avg Distortion** | A model summary's epidemiology avg distortion is the average allocation distortion across the treatment rankings related to the model summary. |
| **DR-87 Education Avg Distortion** | A model summary's education avg distortion is the average allocation distortion across the treatment rankings related to the model summary. |
| **DR-88 Confounder Sign Flip Count** | A model summary's confounder sign flip count is the number of the model summary's treatment rankings that are sign flips. |
| **DR-89 Confounder Latent Only Count** | A model summary's confounder latent only count is the number of the model summary's treatment rankings that are latent only flips. |
| **DR-90 Collider Selection Count** | A model summary's collider selection count is computed as the number of treatment rankings related to the model summary plus the number of treatment rankings related to the model summary. |
| **DR-91 Collider Selection Manifest Count** | A model summary's collider selection manifest count is computed as the number of the model summary's treatment rankings that are sign flips plus the number of the model summary's treatment rankings that are sign flips. |
| **DR-92 Collider Selection Latent Only Count** | A model summary's collider selection latent only count is computed as the number of the model summary's treatment rankings that are latent only flips plus the number of the model summary's treatment rankings that are latent only flips. |
| **DR-93 Explained Confounder Count** | A model summary's explained confounder count is the number of treatment rankings related to the model summary. |
| **DR-94 Contested or Mediator Explained Count** | A model summary's contested or mediator explained count is the number of the model summary's treatment rankings that are paradox-explained and are not adjustment appropriate. |
| **DR-95 Discovery Witness Note** | A model summary's discovery witness note is computed as “sweep: latentD=”, followed by the latent type d fraction, followed by “ purityMax=”, followed by the sign flip signal purity max, followed by “ catalogExact=”, followed by the type prediction match rate, followed by “ domainGapSurvives=”, followed by the domain flip gap survives geometry control. |
| **DR-96 Type Prediction Match Count** | A model summary's type prediction match count is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMatchCount]). |
| **DR-97 Type Prediction Mismatch Count** | A model summary's type prediction mismatch count is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMismatchCount]). |
| **DR-98 Type Prediction Match Rate** | A model summary's type prediction match rate is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMatchRate]). |
| **DR-99 Sign Flip Prediction Match Rate** | A model summary's sign flip prediction match rate is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[SignFlipPredictionMatchRate]). |
| **DR-100 Catalog Prediction Witness Note** | A model summary's catalog prediction witness note is the value of LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogPredictionWitnessNote]). |
| **DR-101 C Plus Avg Distortion** | A model summary's c plus avg distortion is the average allocation distortion across the treatment rankings related to the model summary. |
| **DR-102 C Minus Avg Distortion** | A model summary's c minus avg distortion is the average allocation distortion across the treatment rankings related to the model summary. |
| **DR-103 Type D Avg Distortion** | A model summary's type d avg distortion is the average allocation distortion across the treatment rankings related to the model summary. |
| **DR-104 Sweep Fragile Count** | A model summary's sweep fragile count is the number of the model summary's treatment rankings that are signal purity; are not allocation distortion; have a sweep pooled gap range of “>0.3”; and have a allocation fragility of “>=10”. |
| **DR-105 Expansion Wave1 Economics Expected a Count** | A model summary's expansion wave1 economics expected a count is the number of the model summary's candidate study catalog that are imported; have a domain of “economics”; and have a expected distortion type of “A”. |
| **DR-106 Expansion Wave1 Economics Expected AD Count** | A model summary's expansion wave1 economics expected AD count is the number of the model summary's candidate study catalog that are imported; have a domain of “economics”; have a expected distortion type of “A”; and have a observed distortion type of “D”. |
| **DR-107 Economics Expected a Mismatch Rate** | The model summary's economics expected a mismatch rate is determined by the following priority:<br>1. an empty string, if the expansion wave1 economics expected a count is 0;<br>2. in all other cases, the expansion wave1 economics expected AD count divided by the expansion wave1 economics expected a count. |
| **DR-108 Is Confounder** | A stratum variable is considered a confounder if all of the following hold: the affects treatment assignment flag is set; the affects outcome flag is set; and the causal role is “confounder”. |
| **DR-109 Total Cases a** | A treatment ranking's total cases a is the total cases across the case cells related to the treatment ranking. |
| **DR-110 Total Successes a** | A treatment ranking's total successes a is the total successes across the case cells related to the treatment ranking. |
| **DR-111 Pooled Rate a** | The treatment ranking's pooled rate a is determined by the following priority:<br>1. an empty string, if the total cases a is 0;<br>2. in all other cases, the total successes a divided by the total cases a. |
| **DR-112 Total Cases B** | A treatment ranking's total cases b is the total cases across the case cells related to the treatment ranking. |
| **DR-113 Total Successes B** | A treatment ranking's total successes b is the total successes across the case cells related to the treatment ranking. |
| **DR-114 Pooled Rate B** | The treatment ranking's pooled rate b is determined by the following priority:<br>1. an empty string, if the total cases b is 0;<br>2. in all other cases, the total successes b divided by the total cases b. |
| **DR-115 Pooled Winner** | The treatment ranking's pooled winner is determined by the following priority:<br>1. an empty string, if at least one of the following holds: the pooled rate a is blank or the pooled rate b is blank;<br>2. “tie”, if the absolute value of the pooled rate a minus the pooled rate b is less than 0.0000001;<br>3. the treatment a, if the pooled rate a is greater than the pooled rate b;<br>4. in all other cases, the treatment b. |
| **DR-116 Stratum Count** | A treatment ranking's stratum count is the number of stratum summaries related to the treatment ranking. |
| **DR-117 Strata Won by a** | A treatment ranking's strata won by a is the number of stratum summaries related to the treatment ranking. |
| **DR-118 Strata Won by B** | A treatment ranking's strata won by b is the number of stratum summaries related to the treatment ranking. |
| **DR-119 Per Stratum Winner** | The treatment ranking's per stratum winner is determined by the following priority:<br>1. the treatment a, if the strata won by a is the stratum count;<br>2. the treatment b, if the strata won by b is the stratum count;<br>3. in all other cases, “none”. |
| **DR-120 Is Reversal** | A treatment ranking is considered a reversal if the sign flip flag is set. |
| **DR-121 Confounders in Study** | A treatment ranking's confounders in study is the number of the treatment ranking's stratum variables that are confounders. |
| **DR-122 Is Paradox Explained** | A treatment ranking is considered paradox-explained if all of the following hold: the reversal flag is set and the confounders in study is greater than 0. |
| **DR-123 Pooled Gap** | The treatment ranking's pooled gap is determined by the following priority:<br>1. an empty string, if the pooled rate a is blank;<br>2. in all other cases, the absolute value of the pooled rate a minus the pooled rate b. |
| **DR-124 Strata Won by Loser** | The treatment ranking's strata won by loser is determined by the following priority:<br>1. the strata won by b, if the pooled winner is the treatment a;<br>2. in all other cases, the strata won by a. |
| **DR-125 Paradox Strength** | The treatment ranking's paradox strength is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the pooled gap times the strata won by loser divided by the stratum count. |
| **DR-126 Pooled Rate From Weights a** | A treatment ranking's pooled rate from weights a is the total weighted stratum rate across the stratum summaries related to the treatment ranking. |
| **DR-127 Pooled Rate From Weights B** | A treatment ranking's pooled rate from weights b is the total weighted stratum rate across the stratum summaries related to the treatment ranking. |
| **DR-128 Reversal Intensity** | The treatment ranking's reversal intensity is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the strata won by loser divided by the stratum count. |
| **DR-129 Threshold Margin** | The treatment ranking's threshold margin is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the reversal intensity minus 0.5. |
| **DR-130 Signed Pooled Gap** | The treatment ranking's signed pooled gap is determined by the following priority:<br>1. an empty string, if the pooled rate a is blank;<br>2. in all other cases, the pooled rate a minus the pooled rate b. |
| **DR-131 Weighted Stratum Gap Sum** | A treatment ranking's weighted stratum gap sum is the total weighted stratum gap across the stratum summaries related to the treatment ranking. |
| **DR-132 Is Sign Flip** | A treatment ranking is considered a sign flip if the signed pooled gap is less than 0 if the weighted stratum gap sum is greater than 0, in all other cases the signed pooled gap is greater than 0. |
| **DR-133 Allocation Distortion** | The treatment ranking's allocation distortion is determined by the following priority:<br>1. an empty string, if the weighted stratum gap sum is blank;<br>2. in all other cases, the absolute value of the weighted stratum gap sum minus the signed pooled gap. |
| **DR-134 Distortion Type** | The treatment ranking's distortion type is determined by the following priority:<br>1. an empty string, if the allocation distortion is blank;<br>2. “B” if all of the following hold: the weighted stratum gap sum is not 0 and the signed pooled gap divided by the weighted stratum gap sum is less than the negative of 1, in all other cases “A” if all of the following hold: the weighted stratum gap sum is not 0 and the signed pooled gap divided by the weighted stratum gap sum is less than 0, in all other cases “A” if the reversal intensity is 1, in all other cases “B”, if the sign flip flag is set;<br>3. “C+”, if all of the following hold: it is not the case that the sign flip flag is set; the allocation distortion is greater than 0.01; and the absolute value of the signed pooled gap is greater than the absolute value of the corrected gap plus 0.001;<br>4. “C-”, if all of the following hold: it is not the case that the sign flip flag is set; the allocation distortion is greater than 0.01; and the absolute value of the signed pooled gap is less than the absolute value of the corrected gap minus 0.001;<br>5. in all other cases, “D”. |
| **DR-135 Policy Implication** | The treatment ranking's policy implication is determined by the following priority:<br>1. an empty string, if the distortion type is blank;<br>2. “stratify-immediately”, if the distortion type is “A”;<br>3. “investigate-confounder”, if the distortion type is “B”;<br>4. “check-allocation-bias”, if at least one of the following holds: the distortion type is “C+” or the distortion type is “C-”;<br>5. in all other cases, “pooled-analysis-trustworthy”. |
| **DR-136 Corrected Gap** | A treatment ranking's corrected gap is the same as its weighted stratum gap sum. |
| **DR-137 Corrected Winner** | The treatment ranking's corrected winner is determined by the following priority:<br>1. an empty string, if the corrected gap is blank;<br>2. “tie”, if the absolute value of the corrected gap is less than 0.0001;<br>3. the treatment a, if the corrected gap is greater than 0;<br>4. the treatment b, if the corrected gap is less than 0;<br>5. in all other cases, “tie”. |
| **DR-138 Corrected Vs Pooled Agreement** | A treatment ranking is flagged corrected vs pooled agreement if the corrected winner is the pooled winner. |
| **DR-139 Corrected Policy Implication** | The treatment ranking's corrected policy implication is determined by the following priority:<br>1. an empty string, if the distortion type is blank;<br>2. “use-corrected-winner”, if the distortion type is “A”;<br>3. “use-corrected-winner-with-caution”, if the distortion type is “B”;<br>4. “check-allocation-bias”, if at least one of the following holds: the distortion type is “C+” or the distortion type is “C-”;<br>5. in all other cases, “pooled-analysis-trustworthy”. |
| **DR-140 Allocation Direction** | The treatment ranking's allocation direction is determined by the following priority:<br>1. an empty string, if the corrected gap is blank;<br>2. “reversal”, if the sign flip flag is set;<br>3. “amplification”, if the absolute value of the signed pooled gap is greater than the absolute value of the corrected gap plus 0.001;<br>4. “compression”, if the absolute value of the signed pooled gap is less than the absolute value of the corrected gap minus 0.001;<br>5. in all other cases, “neutral”. |
| **DR-141 Signal Purity** | The treatment ranking's signal purity is determined by the following priority:<br>1. an empty string, if the corrected gap is blank;<br>2. 1, if the absolute value of the corrected gap plus the allocation distortion is 0;<br>3. in all other cases, the absolute value of the corrected gap divided by the absolute value of the corrected gap plus the allocation distortion. |
| **DR-142 Pooled Gap Crosses Zero** | A treatment ranking is flagged pooled gap crosses zero if the value of LOOKUP(Study, SweepStudySummary[SweepStudyId], SweepStudySummary[PooledGapCrossesZero]). |
| **DR-143 Sweep Pooled Gap Range** | A treatment ranking's sweep pooled gap range is computed as the value of LOOKUP(Study, SweepStudySummary[SweepStudyId], SweepStudySummary[SweepPooledGapRange]). |
| **DR-144 Latent Flip Potential** | A treatment ranking is flagged latent flip potential if all of the following hold: the distortion type is “D” and the pooled gap crosses zero is true. |
| **DR-145 Allocation Fragility** | The treatment ranking's allocation fragility is determined by the following priority:<br>1. an empty string, if at least one of the following holds: the sweep pooled gap range is blank; the pooled gap is blank; or the pooled gap is 0;<br>2. in all other cases, the sweep pooled gap range divided by the absolute value of the pooled gap. |
| **DR-146 Study Domain** | A treatment ranking's study domain is computed as the value of LOOKUP(Study, Studies[StudyId], Studies[Domain]). |
| **DR-147 Stratum Causal Role** | A treatment ranking's stratum causal role is the value of LOOKUP(Study, StratumVariables[Study], StratumVariables[CausalRole]). |
| **DR-148 Is Latent Only Flip** | A treatment ranking is considered a latent only flip if all of the following hold: the pooled gap crosses zero is true and the is sign flip is false. |
| **DR-149 Confirmed Causal Role Count** | A treatment ranking's confirmed causal role count is the number of the treatment ranking's stratum variables that have a causal role of “confounder”. |
| **DR-150 Mediator Risk Count** | A treatment ranking's mediator risk count is the number of the treatment ranking's mediator risks. |
| **DR-151 Contested Stratum Count** | A treatment ranking's contested stratum count is the number of the treatment ranking's stratum variables that have a causal role of “contested”. |
| **DR-152 Unknown Causal Role Count** | A treatment ranking's unknown causal role count is the number of the treatment ranking's stratum variables that have a causal role of “unknown”. |
| **DR-153 Causal Claim Status** | The treatment ranking's causal claim status is determined by the following priority:<br>1. “contested”, if the contested stratum count is greater than 0;<br>2. “geometric-only”, if the unknown causal role count is greater than 0;<br>3. “established”, if the confirmed causal role count is greater than 0;<br>4. in all other cases, “geometric-only”. |
| **DR-154 Adjustment Appropriate** | A treatment ranking is flagged adjustment appropriate if all of the following hold: the mediator risk count is 0 and the causal claim status is “established”. |
| **DR-155 Universe Count** | An invariant check's universe count is computed as the pass count plus the fail count. |
| **DR-156 Is Green** | An invariant check is considered a green if the fail count is 0. |
| **DR-157 Status Label** | The invariant check's status label is determined by the following priority:<br>1. “PASS”, if the green flag is set;<br>2. in all other cases, “FAIL(”, followed by the fail count, followed by “)”. |
| **DR-158 Tradition Name** | An invariant check's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-159 Tradition Key Claim Summary** | An invariant check's tradition key claim summary is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim]). |
| **DR-160 Protects Conclusion Title** | An invariant check's protects conclusion title is the value of LOOKUP(ProtectsConclusion, Conclusions[ConclusionId], Conclusions[Title]). |
| **DR-161 Protects Conclusion Category** | An invariant check's protects conclusion category is the value of LOOKUP(ProtectsConclusion, Conclusions[ConclusionId], Conclusions[Category]). |
| **DR-162 Tradition Name** | A methodology's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-163 Tradition Primary Venue** | A methodology's tradition primary venue is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[PrimaryVenue]). |
| **DR-164 Pioneering Researcher Name** | A methodology's pioneering researcher name is the value of LOOKUP(PioneeringResearcher, Researchers[ResearcherId], Researchers[Name]). |
| **DR-165 Pioneering Researcher Affiliation** | A methodology's pioneering researcher affiliation is the value of LOOKUP(PioneeringResearcher, Researchers[ResearcherId], Researchers[Affiliation]). |
| **DR-166 Loop Count** | A methodology's loop count is the number of loops related to the methodology. |
| **DR-167 Tradition Name** | A conclusion's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-168 Tradition Key Claim Summary** | A conclusion's tradition key claim summary is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim]). |
| **DR-169 Researcher Name** | A conclusion's researcher name is the value of LOOKUP(ResearcherId, Researchers[ResearcherId], Researchers[Name]). |
| **DR-170 Researcher Affiliation** | A conclusion's researcher affiliation is the value of LOOKUP(ResearcherId, Researchers[ResearcherId], Researchers[Affiliation]). |
| **DR-171 Challenges Researcher Name** | A conclusion's challenges researcher name is the value of LOOKUP(ChallengesResearcher, Researchers[ResearcherId], Researchers[Name]). |
| **DR-172 Witnessed in Loop Title** | A conclusion's witnessed in loop title is the value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[Title]). |
| **DR-173 Witnessed in Loop Commit Hash** | A conclusion's witnessed in loop commit hash is the value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitHash]). |
| **DR-174 Witnessed in Loop Commit Short** | A conclusion's witnessed in loop commit short is the value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitShort]). |
| **DR-175 Witnessed in Loop Commit Date** | A conclusion's witnessed in loop commit date is the value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitDate]). |
| **DR-176 Witnessed in Loop Git Tag** | A conclusion's witnessed in loop git tag is the value of LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[GitTag]). |
| **DR-177 Invariant Protecting Count** | A conclusion's invariant protecting count is the number of invariant checks related to the conclusion. |
| **DR-178 Tradition Name** | A UI screen's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-179 Tradition Core Conern** | A UI screen's tradition core conern is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern]). |
| **DR-180 Primary Conclusion Title** | A UI screen's primary conclusion title is the value of LOOKUP(PrimaryConclusion, Conclusions[ConclusionId], Conclusions[Title]). |
| **DR-181 Primary Conclusion Evidence** | A UI screen's primary conclusion evidence is the value of LOOKUP(PrimaryConclusion, Conclusions[ConclusionId], Conclusions[Evidence]). |
| **DR-182 Is Original** | An allocation sweep is considered an original if the value of ABS(AllocFractionA - LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[OriginalAllocFractionA])) < 0.03. |
| **DR-183 N Sweep Stratum Total** | An allocation sweep's n sweep stratum total is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NSweepStratumTotal]). |
| **DR-184 N Sweep a** | An allocation sweep's n sweep a is computed as the n sweep stratum total times the alloc fraction a rounded to the nearest whole number. |
| **DR-185 N Sweep B** | An allocation sweep's n sweep b is computed as the n sweep stratum total minus the n sweep a. |
| **DR-186 N Fixed a** | An allocation sweep's n fixed a is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NFixedA]). |
| **DR-187 N Fixed B** | An allocation sweep's n fixed b is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NFixedB]). |
| **DR-188 Sweep Rate a** | An allocation sweep's sweep rate a is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepRateA]). |
| **DR-189 Sweep Rate B** | An allocation sweep's sweep rate b is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepRateB]). |
| **DR-190 Fixed Rate a** | An allocation sweep's fixed rate a is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[FixedRateA]). |
| **DR-191 Fixed Rate B** | An allocation sweep's fixed rate b is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[FixedRateB]). |
| **DR-192 Sweep Pooled Rate a** | The allocation sweep's sweep pooled rate a is determined by the following priority:<br>1. an empty string, if the n sweep a plus the n fixed a is 0;<br>2. in all other cases, the n sweep a times the sweep rate a plus the n fixed a times the fixed rate a divided by the n sweep a plus the n fixed a. |
| **DR-193 Sweep Pooled Rate B** | The allocation sweep's sweep pooled rate b is determined by the following priority:<br>1. an empty string, if the n sweep b plus the n fixed b is 0;<br>2. in all other cases, the n sweep b times the sweep rate b plus the n fixed b times the fixed rate b divided by the n sweep b plus the n fixed b. |
| **DR-194 Sweep Pooled Gap** | The allocation sweep's sweep pooled gap is determined by the following priority:<br>1. an empty string, if the sweep pooled rate a is blank;<br>2. in all other cases, the sweep pooled rate a minus the sweep pooled rate b. |
| **DR-195 Sweep Corrected Gap** | An allocation sweep's sweep corrected gap is computed as the value of LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepCorrectedGap]). |
| **DR-196 Allocation Distortion Witness** | The allocation sweep's allocation distortion witness is determined by the following priority:<br>1. an empty string, if the sweep pooled gap is blank;<br>2. in all other cases, the sweep pooled gap minus the sweep corrected gap. |
| **DR-197 Distortion Type Label** | A sweep study summary's distortion type label is computed as the value of LOOKUP(SweepStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). |
| **DR-198 Corrected Gap Constant** | A sweep study summary's corrected gap constant is the smallest sweep corrected gap across the allocation sweep related to the sweep study summary. |
| **DR-199 Sweep Corrected Gap Max** | A sweep study summary's sweep corrected gap max is the largest sweep corrected gap across the allocation sweep related to the sweep study summary. |
| **DR-200 Sweep Corrected Gap Min** | A sweep study summary's sweep corrected gap min is the smallest sweep corrected gap across the allocation sweep related to the sweep study summary. |
| **DR-201 Sweep Corrected Gap Range** | A sweep study summary's sweep corrected gap range is computed as the sweep corrected gap max minus the sweep corrected gap min. |
| **DR-202 Sweep Pooled Gap Max** | A sweep study summary's sweep pooled gap max is the largest sweep pooled gap across the allocation sweep related to the sweep study summary. |
| **DR-203 Sweep Pooled Gap Min** | A sweep study summary's sweep pooled gap min is the smallest sweep pooled gap across the allocation sweep related to the sweep study summary. |
| **DR-204 Sweep Pooled Gap Range** | A sweep study summary's sweep pooled gap range is computed as the sweep pooled gap max minus the sweep pooled gap min. |
| **DR-205 Pooled Gap Crosses Zero** | A sweep study summary is flagged pooled gap crosses zero if all of the following hold: the sweep pooled gap min is less than 0 and the sweep pooled gap max is greater than 0. |
| **DR-206 Invariant Witness** | The sweep study summary's invariant witness is determined by the following priority:<br>1. “PASS: CorrectedGap invariant across allocation sweep”, if the sweep corrected gap range is less than 0.0001;<br>2. in all other cases, “FAIL: CorrectedGap varies — formula error”. |
| **DR-207 Sweep Stratum Label** | A sweep study summary's sweep stratum label is computed as the value of LOOKUP(SweepStudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepStratumLabel]). |
| **DR-208 Illustrated by Study Title** | A research tradition's illustrated by study title is the value of LOOKUP(IllustratedByStudy, Studies[StudyId], Studies[Title]). |
| **DR-209 Illustrated by Study Source** | A research tradition's illustrated by study source is the value of LOOKUP(IllustratedByStudy, Studies[StudyId], Studies[Source]). |
| **DR-210 Illustrated by Study Distortion Type** | A research tradition's illustrated by study distortion type is the value of LOOKUP(IllustratedByStudy, TreatmentRankings[Study], TreatmentRankings[DistortionType]). |
| **DR-211 Supporting Conclusion Title** | A research tradition's supporting conclusion title is the value of LOOKUP(SupportingConclusion, Conclusions[ConclusionId], Conclusions[Title]). |
| **DR-212 Supporting Conclusion Category** | A research tradition's supporting conclusion category is the value of LOOKUP(SupportingConclusion, Conclusions[ConclusionId], Conclusions[Category]). |
| **DR-213 Researcher Count** | A research tradition's researcher count is the number of researchers related to the research tradition. |
| **DR-214 Study Count** | A research tradition's study count is the number of studies related to the research tradition. |
| **DR-215 Loop Count** | A research tradition's loop count is the number of loops related to the research tradition. |
| **DR-216 Tradition Name** | A researcher's tradition name is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name]). |
| **DR-217 Tradition Era** | A researcher's tradition era is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Era]). |
| **DR-218 Canonical Study Title** | A researcher's canonical study title is the value of LOOKUP(CanonicalStudyId, Studies[StudyId], Studies[Title]). |
| **DR-219 Canonical Study Distortion Type** | A researcher's canonical study distortion type is the value of LOOKUP(CanonicalStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). |
| **DR-220 Canonical Study Policy Implication** | A researcher's canonical study policy implication is the value of LOOKUP(CanonicalStudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedPolicyImplication]). |
| **DR-221 Illustrates Conclusion Title** | A researcher's illustrates conclusion title is the value of LOOKUP(IllustratesConclusion, Conclusions[ConclusionId], Conclusions[Title]). |
| **DR-222 Tradition Key Claim Summary** | A researcher's tradition key claim summary is the value of LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim]). |
| **DR-223 Phase S1 Total** | A synthetic phase's phase s1 total is computed as 200 times the param stratum fraction rounded to the nearest whole number. |
| **DR-224 Phase NA1** | A synthetic phase's phase NA1 is computed as the phase s1 total times 0.5 plus the param allocation bias divided by 2 rounded to the nearest whole number. |
| **DR-225 Phase NB1** | A synthetic phase's phase NB1 is computed as the phase s1 total minus the phase NA1. |
| **DR-226 Phase NA2** | A synthetic phase's phase NA2 is computed as 100 minus the phase NA1. |
| **DR-227 Phase NB2** | A synthetic phase's phase NB2 is computed as 100 minus the phase NB1. |
| **DR-228 Phase Rate A1** | A synthetic phase's phase rate a1 is computed as 0.45 plus the param stratum gap1 divided by 2. |
| **DR-229 Phase Rate B1** | A synthetic phase's phase rate b1 is computed as 0.45 minus the param stratum gap1 divided by 2. |
| **DR-230 Phase Rate A2** | A synthetic phase's phase rate a2 is computed as 0.75 plus the param stratum gap2 divided by 2. |
| **DR-231 Phase Rate B2** | A synthetic phase's phase rate b2 is computed as 0.75 minus the param stratum gap2 divided by 2. |
| **DR-232 Phase Pooled Rate a** | A synthetic phase's phase pooled rate a is computed as the phase NA1 times the phase rate a1 plus the phase NA2 times the phase rate a2 divided by 100. |
| **DR-233 Phase Pooled Rate B** | A synthetic phase's phase pooled rate b is computed as the phase NB1 times the phase rate b1 plus the phase NB2 times the phase rate b2 divided by 100. |
| **DR-234 Phase Signed Pooled Gap** | A synthetic phase's phase signed pooled gap is computed as the phase pooled rate a minus the phase pooled rate b. |
| **DR-235 Phase Corrected Gap** | A synthetic phase's phase corrected gap is computed as the param stratum fraction times the param stratum gap1 plus 1 minus the param stratum fraction times the param stratum gap2. |
| **DR-236 Phase Strata Won by Loser** | The synthetic phase's phase strata won by loser is determined by the following priority:<br>1. the count of the following that hold: the param stratum gap1 is less than 0 and the param stratum gap2 is less than 0, if the phase signed pooled gap is greater than 0;<br>2. in all other cases, the count of the following that hold: the param stratum gap1 is greater than 0 and the param stratum gap2 is greater than 0. |
| **DR-237 Phase Reversal Intensity** | A synthetic phase's phase reversal intensity is computed as the phase strata won by loser divided by 2. |
| **DR-238 Phase is Sign Flip** | A synthetic phase is flagged phase is sign flip if the phase signed pooled gap is less than 0 if the phase corrected gap is greater than 0, in all other cases the phase signed pooled gap is greater than 0. |
| **DR-239 Phase Allocation Distortion** | A synthetic phase's phase allocation distortion is computed as the absolute value of the phase corrected gap minus the phase signed pooled gap. |
| **DR-240 Phase Distortion Type** | The synthetic phase's phase distortion type is determined by the following priority:<br>1. “A”, if all of the following hold: the phase is sign flip flag is set and the phase reversal intensity is 1;<br>2. “B”, if all of the following hold: the phase is sign flip flag is set and the phase reversal intensity is less than 1;<br>3. “C+”, if all of the following hold: it is not the case that the phase is sign flip flag is set; the phase allocation distortion is greater than 0.01; and the absolute value of the phase signed pooled gap is greater than the absolute value of the phase corrected gap plus 0.001;<br>4. “C-”, if all of the following hold: it is not the case that the phase is sign flip flag is set; the phase allocation distortion is greater than 0.01; and the absolute value of the phase signed pooled gap is less than the absolute value of the phase corrected gap minus 0.001;<br>5. in all other cases, “D”. |
| **DR-241 Phase Point Count** | A phase diagram summary's phase point count is computed as the phase type a count plus the phase type b count plus the phase type c plus count plus the phase type c minus count plus the phase type d count. |
| **DR-242 All Five Types Present** | A phase diagram summary is flagged all five types present if all of the following hold: the phase type a count is greater than 0; the phase type b count is greater than 0; the phase type c plus count is greater than 0; the phase type c minus count is greater than 0; and the phase type d count is greater than 0. |
| **DR-243 Phase Taxonomy Witness** | A phase diagram summary's phase taxonomy witness is computed as “A:”, followed by the phase type a count, followed by “ B:”, followed by the phase type b count, followed by “ C+:”, followed by the phase type c plus count, followed by “ C-:”, followed by the phase type c minus count, followed by “ D:”, followed by the phase type d count. |
| **DR-244 Phase Witness Note** | The phase diagram summary's phase witness note is determined by the following priority:<br>1. “PASS: all five distortion types populated in parameter space”, if the all five types present flag is set;<br>2. in all other cases, “FAIL: missing distortion type in grid”. |
| **DR-245 Protocol Item Count** | An ingestion summary's protocol item count is the number of ingestion protocol related to the ingestion summary. |
| **DR-246 Corpus Cell Count** | An ingestion summary's corpus cell count is the number of case cells related to the ingestion summary. |
| **DR-247 Valid Cell Count** | An ingestion summary's valid cell count is the number of case cells related to the ingestion summary. |
| **DR-248 All Cells Valid** | An ingestion summary is flagged all cells valid if the valid cell count is the corpus cell count. |
| **DR-249 Study Count** | An ingestion summary's study count is the number of studies related to the ingestion summary. |
| **DR-250 Structural Compliant Count** | An ingestion summary's structural compliant count is the number of studies related to the ingestion summary. |
| **DR-251 All Studies Structural** | An ingestion summary is flagged all studies structural if the structural compliant count is the study count. |
| **DR-252 Real Study Count** | An ingestion summary's real study count is the number of studies related to the ingestion summary. |
| **DR-253 Real Fully Compliant Count** | An ingestion summary's real fully compliant count is the number of studies related to the ingestion summary. |
| **DR-254 All Real Studies Compliant** | An ingestion summary is flagged all real studies compliant if the real fully compliant count is the real study count. |
| **DR-255 Ingestion Contract Passes** | An ingestion summary is flagged ingestion contract passes if all of the following hold: the all cells valid flag is set; the all studies structural flag is set; and the all real studies compliant flag is set. |
| **DR-256 Ingestion Witness Note** | The ingestion summary's ingestion witness note is determined by the following priority:<br>1. “PASS: ”, followed by the protocol item count, followed by “ contract items; ”, followed by the corpus cell count, followed by “ cells valid; ”, followed by the real fully compliant count, followed by a slash, followed by the real study count, followed by “ real studies fully compliant”, if the ingestion contract passes flag is set;<br>2. in all other cases, “FAIL: ingestion contract violated”. |
| **DR-257 Is Ready to Encode** | A candidate study catalog is considered a ready to encode if all of the following hold: the ingestion status is “candidate”; the proposed study ID has a value; the citation has a value; and the stratum variable name has a value. |
| **DR-258 Is Imported** | A candidate study catalog is considered imported if the ingestion status is “imported”. |
| **DR-259 Observed Distortion Type** | A candidate study catalog's observed distortion type is the value of LOOKUP(LinkedStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType]). |
| **DR-260 Type Prediction Match** | A candidate study catalog is flagged type prediction match if the imported flag is set and the observed distortion type is the expected distortion type. |
| **DR-261 Expected Sign Flip** | A candidate study catalog is flagged expected sign flip if at least one of the following holds: the expected distortion type is “A” or the expected distortion type is “B”. |
| **DR-262 Observed Sign Flip Type** | A candidate study catalog is flagged observed sign flip type if the imported flag is set and at least one of the following holds: the observed distortion type is “A” or the observed distortion type is “B”. |
| **DR-263 Sign Flip Prediction Match** | A candidate study catalog is flagged sign flip prediction match if all of the following hold: the imported flag is set and the expected sign flip flag is set and the observed sign flip type flag is set. |
| **DR-264 Is Data Ready** | A candidate study catalog is considered a data ready if at least one of the following holds: the data acquisition status is “downloaded” or the data acquisition status is “manual_only”. |
| **DR-265 Total Catalog Entries** | A corpus catalog summary's total catalog entries is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-266 Imported Count** | A corpus catalog summary's imported count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-267 Candidate Count** | A corpus catalog summary's candidate count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-268 Blocked Count** | A corpus catalog summary's blocked count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-269 Ready to Encode Count** | A corpus catalog summary's ready to encode count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-270 High Priority Count** | A corpus catalog summary's high priority count is the number of the corpus catalog summary's high priorities. |
| **DR-271 Import Session Ready** | A corpus catalog summary is flagged import session ready if all of the following hold: the candidate count is at least 20 and the ready to encode count is at least 15. |
| **DR-272 Type Prediction Match Count** | A corpus catalog summary's type prediction match count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-273 Type Prediction Mismatch Count** | A corpus catalog summary's type prediction mismatch count is the number of the corpus catalog summary's candidate study catalog that are not type prediction match. |
| **DR-274 Sign Flip Prediction Eligible Count** | A corpus catalog summary's sign flip prediction eligible count is the number of the corpus catalog summary's candidate study catalog that are expected sign flip. |
| **DR-275 Sign Flip Prediction Match Count** | A corpus catalog summary's sign flip prediction match count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-276 Type Prediction Match Rate** | The corpus catalog summary's type prediction match rate is determined by the following priority:<br>1. an empty string, if the imported count is 0;<br>2. in all other cases, the type prediction match count divided by the imported count. |
| **DR-277 Sign Flip Prediction Match Rate** | The corpus catalog summary's sign flip prediction match rate is determined by the following priority:<br>1. an empty string, if the sign flip prediction eligible count is 0;<br>2. in all other cases, the sign flip prediction match count divided by the sign flip prediction eligible count. |
| **DR-278 Catalog Witness Note** | The corpus catalog summary's catalog witness note is determined by the following priority:<br>1. “READY: ”, followed by the ready to encode count, followed by “ encode-ready (”, followed by the data ready count, followed by “ data-ready) of ”, followed by the candidate count, followed by “ candidates”, if the import session ready flag is set;<br>2. in all other cases, “BUILDING: ”, followed by the candidate count, followed by “ candidates — ”, followed by the data ready count, followed by “ data-ready, ”, followed by the ready to encode count, followed by “ encode-ready”. |
| **DR-279 Catalog Prediction Witness Note** | A corpus catalog summary's catalog prediction witness note is computed as the value of CONCAT("exact=", TypePredictionMatchCount, "/", ImportedCount, " (", ROUND(TypePredictionMatchRate * 100, 1), "%); flipPred=", SignFlipPredictionMatchCount, "/", SignFlipPredictionEligibleCount, " (", ROUND(SignFlipPredictionMatchRate * 100, 1), "%)")). |
| **DR-280 Data Ready Count** | A corpus catalog summary's data ready count is the number of the corpus catalog summary's candidate study catalog that have a ingestion status of “candidate”. |
| **DR-281 Encode Pipeline Ready Count** | A corpus catalog summary's encode pipeline ready count is the number of the corpus catalog summary's candidate study catalog that are data readies. |
| **DR-282 Expansion Candidate Count** | A corpus catalog summary's expansion candidate count is the number of candidate study catalog related to the corpus catalog summary. |
| **DR-283 Current Imported Count** | A domain expansion target's current imported count is the number of the domain expansion target's studies that are not synthetic. |
| **DR-284 Candidate Queued Count** | A domain expansion target's candidate queued count is the number of the domain expansion target's candidate study catalog that have a ingestion status of “candidate”. |
| **DR-285 Projected Count** | A domain expansion target's projected count is computed as the current imported count plus the candidate queued count. |
| **DR-286 Gap Count** | A domain expansion target's gap count is computed as the largest of 0 and the target min count minus the projected count. |
| **DR-287 Is Under Represented** | A domain expansion target is considered under-represented if the projected count is less than the target min count. |
| **DR-288 Hypothesis Statement** | A discovery finding's hypothesis statement is the value of LOOKUP(HypothesisId, DiscoveryHypotheses[HypothesisId], DiscoveryHypotheses[Statement]). |
| **DR-289 Observed Metric** | A discovery finding's observed metric is computed as the value of IF(HypothesisId = "H-latent-d", CONCAT("fraction=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[LatentTypeDFraction])), IF(HypothesisId = "H-purity", CONCAT("maxPurity=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SignFlipSignalPurityMax])), IF(HypothesisId = "H-small-effect", CONCAT("stable=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapStableD]), " latent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapLatentD])), IF(HypothesisId = "H-econ-zero", CONCAT("flips=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EconomicsSignFlipCount])), IF(HypothesisId = "H-domain-dist", CONCAT("epi=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EpidemiologyAvgDistortion]), " edu=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EducationAvgDistortion])), IF(HypothesisId = "H-causal-manifest", CONCAT("confLatent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderLatentOnlyCount]), " confFlip=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount])), IF(HypothesisId = "H-causal-latent", CONCAT("collManifest=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]), " collLatent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionLatentOnlyCount]), " collN=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionCount])), IF(HypothesisId = "H-explained-confounder", CONCAT("ExplainedConfounderCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]), ", ConfounderSignFlipCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount])), IF(HypothesisId = "H-unexplained-nonconfounder", CONCAT("ContestedOrMediatorExplainedCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ContestedOrMediatorExplainedCount])), "")))))). |
| **DR-290 Is Confirmed** | A discovery finding is considered confirmed if the value of IF(HypothesisId = "H-latent-d", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[LatentTypeDFraction]) > 0.5, IF(HypothesisId = "H-purity", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SignFlipSignalPurityMax]) < 0.5, IF(HypothesisId = "H-small-effect", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapStableD]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapLatentD]), IF(HypothesisId = "H-econ-zero", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EconomicsSignFlipCount]) = 0, IF(HypothesisId = "H-domain-dist", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EpidemiologyAvgDistortion]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EducationAvgDistortion]), IF(HypothesisId = "H-causal-manifest", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]), LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]) >= 10), IF(HypothesisId = "H-causal-latent", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]) = 0, LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionLatentOnlyCount]) >= 5), IF(HypothesisId = "H-explained-confounder", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]) = LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]), LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]) >= 10), IF(HypothesisId = "H-unexplained-nonconfounder", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ContestedOrMediatorExplainedCount]) = 0, FALSE()))))). |
| **DR-291 Evidence** | The discovery finding's evidence is determined by the following priority:<br>1. “PASS: ”, followed by the observed metric, if the is confirmed is true;<br>2. “FAIL: ”, followed by the observed metric, if the is confirmed is false;<br>3. in all other cases, an empty string. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Loops.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **Loops.TraditionCoreConern** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern])` |
| **Loops.CommitShort** | formula | `Left(CommitHash, 7)` |
| **Studies.TotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **Studies.CellCount** | rollup | `Count(CaseCells via Study)` |
| **Studies.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **Studies.TraditionCoreConern** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern])` |
| **Studies.PrimaryResearcherName** | lookup | `LOOKUP(PrimaryResearcherId, Researchers[ResearcherId], Researchers[Name])` |
| **Studies.PrimaryResearcherAffiliation** | lookup | `LOOKUP(PrimaryResearcherId, Researchers[ResearcherId], Researchers[Affiliation])` |
| **Studies.DistortionType** | lookup | `LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType])` |
| **Studies.PolicyImplication** | lookup | `LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedPolicyImplication])` |
| **Studies.AllocationDistortion** | lookup | `LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[AllocationDistortion])` |
| **Studies.SignalPurity** | lookup | `LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[SignalPurity])` |
| **Studies.IsSignFlip** | lookup | `LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[IsSignFlip])` |
| **Studies.CorrectedWinner** | lookup | `LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedWinner])` |
| **Studies.ConfoundingVariable** | lookup | `LOOKUP(StudyId, StratumVariables[Study], StratumVariables[VariableName])` |
| **Studies.CausalRole** | lookup | `LOOKUP(StudyId, StratumVariables[Study], StratumVariables[CausalRole])` |
| **Studies.IngestionCellParity** | formula | `AND(LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[StratumCount]) >= 2, CellCount = LOOKUP(StudyId, TreatmentRankings[Study], TreatmentRankings[StratumCount]) * 2)` |
| **Studies.IngestionCompliance** | formula | `IF(IsSynthetic, "exempt-synthetic", IF(AND(IngestionCellParity, LOOKUP(StudyId, StratumVariables[Study], StratumVariables[VariableName]) <> ""), "all", "partial"))` |
| **Treatments.TotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **Treatments.TotalSuccesses** | rollup | `Sum(CaseCells.Successes via Study)` |
| **Treatments.PooledSuccessRate** | formula | `If(TotalCases = 0, "", TotalSuccesses / TotalCases)` |
| **Strata.TotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **CaseCells.CellSuccessRate** | formula | `If(Cases = 0, "", Successes / Cases)` |
| **CaseCells.TotalCasesForTreatment** | rollup | `Sum(CaseCells.Cases via Study)` |
| **CaseCells.TreatmentExposureFraction** | formula | `If(TotalCasesForTreatment = 0, "", Cases / TotalCasesForTreatment)` |
| **CaseCells.IsValidIngestionCell** | formula | `And(Cases > 0, Successes >= 0, Successes <= Cases)` |
| **StratumSummaries.StratumSuccesses** | rollup | `Sum(CaseCells.Successes via Study)` |
| **StratumSummaries.StratumCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.StratumSuccessRate** | formula | `If(StratumCases = 0, "", StratumSuccesses / StratumCases)` |
| **StratumSummaries.StratumSuccessesA** | rollup | `Sum(CaseCells.Successes via Study)` |
| **StratumSummaries.StratumCasesA** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.StratumRateA** | formula | `If(StratumCasesA = 0, "", StratumSuccessesA / StratumCasesA)` |
| **StratumSummaries.StratumSuccessesB** | rollup | `Sum(CaseCells.Successes via Study)` |
| **StratumSummaries.StratumCasesB** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.StratumRateB** | formula | `If(StratumCasesB = 0, "", StratumSuccessesB / StratumCasesB)` |
| **StratumSummaries.StratumWinner** | formula | `If(StratumRateA > StratumRateB, "A", "B")` |
| **StratumSummaries.StudyTotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.StratumTotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.StratumFraction** | formula | `If(StudyTotalCases = 0, "", StratumTotalCases / StudyTotalCases)` |
| **StratumSummaries.WeightedStratumRate** | formula | `If(StratumFraction = "", "", StratumFraction * StratumSuccessRate)` |
| **StratumSummaries.TreatmentACasesHere** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.TreatmentBCasesHere** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.TreatmentATotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.TreatmentBTotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **StratumSummaries.AllocationFractionA** | formula | `If(TreatmentATotalCases = 0, "", TreatmentACasesHere / TreatmentATotalCases)` |
| **StratumSummaries.AllocationFractionB** | formula | `If(TreatmentBTotalCases = 0, "", TreatmentBCasesHere / TreatmentBTotalCases)` |
| **StratumSummaries.AllocationBias** | formula | `If(AllocationFractionA = "", "", AllocationFractionA - AllocationFractionB)` |
| **StratumSummaries.StratumGap** | formula | `If(StratumRateA = "", "", StratumRateA - StratumRateB)` |
| **StratumSummaries.WeightedStratumGap** | formula | `If(StratumGap = "", "", StratumGap * StratumFraction)` |
| **ModelSummary.ReversalCount** | rollup | `Count(TreatmentRankings via IsReversal)` |
| **ModelSummary.NonReversalCount** | rollup | `Count(TreatmentRankings via IsReversal)` |
| **ModelSummary.StudyCount** | formula | `ReversalCount + NonReversalCount` |
| **ModelSummary.ExplainedCount** | rollup | `Count(TreatmentRankings via IsParadoxExplained)` |
| **ModelSummary.TotalParadoxStrength** | rollup | `Sum(TreatmentRankings.ParadoxStrength via TreatmentA)` |
| **ModelSummary.AvgParadoxStrength** | formula | `If(StudyCount = 0, "", TotalParadoxStrength / StudyCount)` |
| **ModelSummary.TypeACount** | rollup | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.TypeBCount** | rollup | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.TypeDCount** | rollup | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.TypeAFraction** | formula | `If(StudyCount = 0, "", TypeACount / StudyCount)` |
| **ModelSummary.DistortionTaxonomyCoverage** | formula | `Concat("A:", TypeACount, " B:", TypeBCount, " C+:", CAmplificationCount, " C-:", CCompressionCount, " D:", TypeDCount)` |
| **ModelSummary.DistortionOnlyCount** | formula | `CAmplificationCount + CCompressionCount` |
| **ModelSummary.CAmplificationCount** | rollup | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.CCompressionCount** | rollup | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.AvgSignalPurity** | rollup | `Sum(TreatmentRankings.SignalPurity via Study) / Count(TreatmentRankings via Study)` |
| **ModelSummary.SweepCorrectedGapMax** | rollup | `Max(AllocationSweep.SweepCorrectedGap via StudyId)` |
| **ModelSummary.SweepCorrectedGapMin** | rollup | `Min(AllocationSweep.SweepCorrectedGap via StudyId)` |
| **ModelSummary.SweepCorrectedGapRange** | formula | `If(SweepCorrectedGapMax = "", "", SweepCorrectedGapMax - SweepCorrectedGapMin)` |
| **ModelSummary.SweepPooledGapRange** | rollup | `Max(AllocationSweep.SweepPooledGap via StudyId) - Min(AllocationSweep.SweepPooledGap via StudyId)` |
| **ModelSummary.IngestionProtocolItemCount** | rollup | `Count(IngestionProtocol via ProtocolId)` |
| **ModelSummary.CorpusPassesIngestionContract** | lookup | `LOOKUP("ingestion-v1", IngestionSummary[IngestionSummaryId], IngestionSummary[IngestionContractPasses])` |
| **ModelSummary.IngestionWitnessNote** | lookup | `LOOKUP("ingestion-v1", IngestionSummary[IngestionSummaryId], IngestionSummary[IngestionWitnessNote])` |
| **ModelSummary.CatalogEntryCount** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TotalCatalogEntries])` |
| **ModelSummary.PendingImportCount** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CandidateCount])` |
| **ModelSummary.ReadyToEncodeCount** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ReadyToEncodeCount])` |
| **ModelSummary.ImportSessionReady** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ImportSessionReady])` |
| **ModelSummary.CatalogWitnessNote** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogWitnessNote])` |
| **ModelSummary.LatentTypeDCount** | formula | `Count(TreatmentRankings via LatentFlipPotential)` |
| **ModelSummary.StableTypeDCount** | formula | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.LatentTypeDFraction** | formula | `If(TypeDCount = 0, "", LatentTypeDCount / TypeDCount)` |
| **ModelSummary.CrossZeroCount** | formula | `Count(TreatmentRankings via PooledGapCrossesZero)` |
| **ModelSummary.SignFlipSignalPurityMax** | formula | `Max(TreatmentRankings.SignalPurity via IsSignFlip)` |
| **ModelSummary.EconomicsSignFlipCount** | formula | `Count(TreatmentRankings via StudyDomain)` |
| **ModelSummary.AvgPooledGapLatentD** | formula | `If(LatentTypeDCount = 0, "", Average(TreatmentRankings.PooledGap via LatentFlipPotential))` |
| **ModelSummary.AvgPooledGapStableD** | formula | `If(StableTypeDCount = 0, "", Average(TreatmentRankings.PooledGap via DistortionType))` |
| **ModelSummary.EpidemiologyAvgDistortion** | formula | `Average(TreatmentRankings.AllocationDistortion via StudyDomain)` |
| **ModelSummary.EducationAvgDistortion** | formula | `Average(TreatmentRankings.AllocationDistortion via StudyDomain)` |
| **ModelSummary.ConfounderSignFlipCount** | formula | `Count(TreatmentRankings via StratumCausalRole)` |
| **ModelSummary.ConfounderLatentOnlyCount** | formula | `Count(TreatmentRankings via StratumCausalRole)` |
| **ModelSummary.ColliderSelectionCount** | formula | `Count(TreatmentRankings via StratumCausalRole) + Count(TreatmentRankings via StratumCausalRole)` |
| **ModelSummary.ColliderSelectionManifestCount** | formula | `Count(TreatmentRankings via StratumCausalRole) + Count(TreatmentRankings via StratumCausalRole)` |
| **ModelSummary.ColliderSelectionLatentOnlyCount** | formula | `Count(TreatmentRankings via StratumCausalRole) + Count(TreatmentRankings via StratumCausalRole)` |
| **ModelSummary.ExplainedConfounderCount** | rollup | `Count(TreatmentRankings via IsParadoxExplained)` |
| **ModelSummary.ContestedOrMediatorExplainedCount** | rollup | `Count(TreatmentRankings via IsSignFlip)` |
| **ModelSummary.DiscoveryWitnessNote** | formula | `Concat("sweep: latentD=", LatentTypeDFraction, " purityMax=", SignFlipSignalPurityMax, " catalogExact=", TypePredictionMatchRate, " domainGapSurvives=", DomainFlipGapSurvivesGeometryControl)` |
| **ModelSummary.TypePredictionMatchCount** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMatchCount])` |
| **ModelSummary.TypePredictionMismatchCount** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMismatchCount])` |
| **ModelSummary.TypePredictionMatchRate** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TypePredictionMatchRate])` |
| **ModelSummary.SignFlipPredictionMatchRate** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[SignFlipPredictionMatchRate])` |
| **ModelSummary.CatalogPredictionWitnessNote** | lookup | `LOOKUP("catalog-v1", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogPredictionWitnessNote])` |
| **ModelSummary.CPlusAvgDistortion** | formula | `Average(TreatmentRankings.AllocationDistortion via DistortionType)` |
| **ModelSummary.CMinusAvgDistortion** | formula | `Average(TreatmentRankings.AllocationDistortion via DistortionType)` |
| **ModelSummary.TypeDAvgDistortion** | formula | `Average(TreatmentRankings.AllocationDistortion via DistortionType)` |
| **ModelSummary.SweepFragileCount** | formula | `Count(TreatmentRankings via DistortionType)` |
| **ModelSummary.ExpansionWave1EconomicsExpectedACount** | formula | `Count(CandidateStudyCatalog via ExpansionWave)` |
| **ModelSummary.ExpansionWave1EconomicsExpectedADCount** | formula | `Count(CandidateStudyCatalog via ExpansionWave)` |
| **ModelSummary.EconomicsExpectedAMismatchRate** | formula | `If(ExpansionWave1EconomicsExpectedACount = 0, "", ExpansionWave1EconomicsExpectedADCount / ExpansionWave1EconomicsExpectedACount)` |
| **StratumVariables.IsConfounder** | formula | `And(AffectsTreatmentAssignment, AffectsOutcome, CausalRole = "confounder")` |
| **TreatmentRankings.TotalCasesA** | rollup | `Sum(CaseCells.Cases via Study)` |
| **TreatmentRankings.TotalSuccessesA** | rollup | `Sum(CaseCells.Successes via Study)` |
| **TreatmentRankings.PooledRateA** | formula | `If(TotalCasesA = 0, "", TotalSuccessesA / TotalCasesA)` |
| **TreatmentRankings.TotalCasesB** | rollup | `Sum(CaseCells.Cases via Study)` |
| **TreatmentRankings.TotalSuccessesB** | rollup | `Sum(CaseCells.Successes via Study)` |
| **TreatmentRankings.PooledRateB** | formula | `If(TotalCasesB = 0, "", TotalSuccessesB / TotalCasesB)` |
| **TreatmentRankings.PooledWinner** | formula | `If(Or(PooledRateA = "", PooledRateB = ""), "", If(Abs(PooledRateA - PooledRateB) < 0.0000001, "tie", If(PooledRateA > PooledRateB, TreatmentA, TreatmentB)))` |
| **TreatmentRankings.StratumCount** | rollup | `Count(StratumSummaries via Study)` |
| **TreatmentRankings.StrataWonByA** | rollup | `Count(StratumSummaries via Study)` |
| **TreatmentRankings.StrataWonByB** | rollup | `Count(StratumSummaries via Study)` |
| **TreatmentRankings.PerStratumWinner** | formula | `If(StrataWonByA = StratumCount, TreatmentA, If(StrataWonByB = StratumCount, TreatmentB, "none"))` |
| **TreatmentRankings.IsReversal** | formula | `If(IsSignFlip = "", "", IsSignFlip)` |
| **TreatmentRankings.ConfoundersInStudy** | rollup | `Count(StratumVariables via Study)` |
| **TreatmentRankings.IsParadoxExplained** | formula | `And(IsReversal, ConfoundersInStudy > 0)` |
| **TreatmentRankings.PooledGap** | formula | `If(PooledRateA = "", "", Abs(PooledRateA - PooledRateB))` |
| **TreatmentRankings.StrataWonByLoser** | formula | `If(PooledWinner = TreatmentA, StrataWonByB, StrataWonByA)` |
| **TreatmentRankings.ParadoxStrength** | formula | `If(StratumCount = 0, "", PooledGap * StrataWonByLoser / StratumCount)` |
| **TreatmentRankings.PooledRateFromWeightsA** | rollup | `Sum(StratumSummaries.WeightedStratumRate via Study)` |
| **TreatmentRankings.PooledRateFromWeightsB** | rollup | `Sum(StratumSummaries.WeightedStratumRate via Study)` |
| **TreatmentRankings.ReversalIntensity** | formula | `If(StratumCount = 0, "", StrataWonByLoser / StratumCount)` |
| **TreatmentRankings.ThresholdMargin** | formula | `If(StratumCount = 0, "", ReversalIntensity - 0.5)` |
| **TreatmentRankings.SignedPooledGap** | formula | `If(PooledRateA = "", "", PooledRateA - PooledRateB)` |
| **TreatmentRankings.WeightedStratumGapSum** | rollup | `Sum(StratumSummaries.WeightedStratumGap via Study)` |
| **TreatmentRankings.IsSignFlip** | formula | `If(WeightedStratumGapSum = "", "", If(WeightedStratumGapSum > 0, SignedPooledGap < 0, SignedPooledGap > 0))` |
| **TreatmentRankings.AllocationDistortion** | formula | `If(WeightedStratumGapSum = "", "", Abs(WeightedStratumGapSum - SignedPooledGap))` |
| **TreatmentRankings.DistortionType** | formula | `If(AllocationDistortion = "", "", If(IsSignFlip, If(And(WeightedStratumGapSum <> 0, SignedPooledGap / WeightedStratumGapSum < -1), "B", If(And(WeightedStratumGapSum <> 0, SignedPooledGap / WeightedStratumGapSum < 0), "A", If(ReversalIntensity = 1, "A", "B"))), If(And(Not(IsSignFlip), AllocationDistortion > 0.01, Abs(SignedPooledGap) > Abs(CorrectedGap) + 0.001), "C+", If(And(Not(IsSignFlip), AllocationDistortion > 0.01, Abs(SignedPooledGap) < Abs(CorrectedGap) - 0.001), "C-", "D"))))` |
| **TreatmentRankings.PolicyImplication** | formula | `If(DistortionType = "", "", If(DistortionType = "A", "stratify-immediately", If(DistortionType = "B", "investigate-confounder", If(Or(DistortionType = "C+", DistortionType = "C-"), "check-allocation-bias", "pooled-analysis-trustworthy"))))` |
| **TreatmentRankings.CorrectedGap** | formula | `WeightedStratumGapSum` |
| **TreatmentRankings.CorrectedWinner** | formula | `If(CorrectedGap = "", "", If(Abs(CorrectedGap) < 0.0001, "tie", If(CorrectedGap > 0, TreatmentA, If(CorrectedGap < 0, TreatmentB, "tie"))))` |
| **TreatmentRankings.CorrectedVsPooledAgreement** | formula | `If(CorrectedWinner = "", "", CorrectedWinner = PooledWinner)` |
| **TreatmentRankings.CorrectedPolicyImplication** | formula | `If(DistortionType = "", "", If(DistortionType = "A", "use-corrected-winner", If(DistortionType = "B", "use-corrected-winner-with-caution", If(Or(DistortionType = "C+", DistortionType = "C-"), "check-allocation-bias", "pooled-analysis-trustworthy"))))` |
| **TreatmentRankings.AllocationDirection** | formula | `If(CorrectedGap = "", "", If(IsSignFlip, "reversal", If(Abs(SignedPooledGap) > Abs(CorrectedGap) + 0.001, "amplification", If(Abs(SignedPooledGap) < Abs(CorrectedGap) - 0.001, "compression", "neutral"))))` |
| **TreatmentRankings.SignalPurity** | formula | `If(CorrectedGap = "", "", If(Abs(CorrectedGap) + AllocationDistortion = 0, 1, Abs(CorrectedGap) / Abs(CorrectedGap) + AllocationDistortion))` |
| **TreatmentRankings.PooledGapCrossesZero** | formula | `LOOKUP(Study, SweepStudySummary[SweepStudyId], SweepStudySummary[PooledGapCrossesZero])` |
| **TreatmentRankings.SweepPooledGapRange** | formula | `LOOKUP(Study, SweepStudySummary[SweepStudyId], SweepStudySummary[SweepPooledGapRange])` |
| **TreatmentRankings.LatentFlipPotential** | formula | `If(DistortionType = "", "", And(DistortionType = "D", PooledGapCrossesZero = TRUE))` |
| **TreatmentRankings.AllocationFragility** | formula | `If(Or(SweepPooledGapRange = "", PooledGap = "", PooledGap = 0), "", SweepPooledGapRange / Abs(PooledGap))` |
| **TreatmentRankings.StudyDomain** | formula | `LOOKUP(Study, Studies[StudyId], Studies[Domain])` |
| **TreatmentRankings.StratumCausalRole** | lookup | `LOOKUP(Study, StratumVariables[Study], StratumVariables[CausalRole])` |
| **TreatmentRankings.IsLatentOnlyFlip** | formula | `If(PooledGapCrossesZero = "", "", And(PooledGapCrossesZero = True(), IsSignFlip = False()))` |
| **TreatmentRankings.ConfirmedCausalRoleCount** | rollup | `Count(StratumVariables via Study)` |
| **TreatmentRankings.MediatorRiskCount** | rollup | `Count(StratumVariables via Study) + Count(StratumVariables via Study)` |
| **TreatmentRankings.ContestedStratumCount** | rollup | `Count(StratumVariables via Study)` |
| **TreatmentRankings.UnknownCausalRoleCount** | rollup | `Count(StratumVariables via Study)` |
| **TreatmentRankings.CausalClaimStatus** | formula | `If(ContestedStratumCount > 0, "contested", If(UnknownCausalRoleCount > 0, "geometric-only", If(ConfirmedCausalRoleCount > 0, "established", "geometric-only")))` |
| **TreatmentRankings.AdjustmentAppropriate** | formula | `And(MediatorRiskCount = 0, CausalClaimStatus = "established")` |
| **InvariantChecks.UniverseCount** | formula | `PassCount + FailCount` |
| **InvariantChecks.IsGreen** | formula | `FailCount = 0` |
| **InvariantChecks.StatusLabel** | formula | `If(IsGreen, "PASS", Concat("FAIL(", FailCount, ")"))` |
| **InvariantChecks.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **InvariantChecks.TraditionKeyClaimSummary** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim])` |
| **InvariantChecks.ProtectsConclusionTitle** | lookup | `LOOKUP(ProtectsConclusion, Conclusions[ConclusionId], Conclusions[Title])` |
| **InvariantChecks.ProtectsConclusionCategory** | lookup | `LOOKUP(ProtectsConclusion, Conclusions[ConclusionId], Conclusions[Category])` |
| **Methodology.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **Methodology.TraditionPrimaryVenue** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[PrimaryVenue])` |
| **Methodology.PioneeringResearcherName** | lookup | `LOOKUP(PioneeringResearcher, Researchers[ResearcherId], Researchers[Name])` |
| **Methodology.PioneeringResearcherAffiliation** | lookup | `LOOKUP(PioneeringResearcher, Researchers[ResearcherId], Researchers[Affiliation])` |
| **Methodology.LoopCount** | rollup | `Count(Loops via TraditionId)` |
| **Conclusions.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **Conclusions.TraditionKeyClaimSummary** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim])` |
| **Conclusions.ResearcherName** | lookup | `LOOKUP(ResearcherId, Researchers[ResearcherId], Researchers[Name])` |
| **Conclusions.ResearcherAffiliation** | lookup | `LOOKUP(ResearcherId, Researchers[ResearcherId], Researchers[Affiliation])` |
| **Conclusions.ChallengesResearcherName** | lookup | `LOOKUP(ChallengesResearcher, Researchers[ResearcherId], Researchers[Name])` |
| **Conclusions.WitnessedInLoopTitle** | lookup | `LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[Title])` |
| **Conclusions.WitnessedInLoopCommitHash** | lookup | `LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitHash])` |
| **Conclusions.WitnessedInLoopCommitShort** | lookup | `LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitShort])` |
| **Conclusions.WitnessedInLoopCommitDate** | lookup | `LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[CommitDate])` |
| **Conclusions.WitnessedInLoopGitTag** | lookup | `LOOKUP(WitnessedInLoop, Loops[LoopId], Loops[GitTag])` |
| **Conclusions.InvariantProtectingCount** | rollup | `Count(InvariantChecks via ProtectsConclusion)` |
| **UIScreens.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **UIScreens.TraditionCoreConern** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[CoreConcern])` |
| **UIScreens.PrimaryConclusionTitle** | lookup | `LOOKUP(PrimaryConclusion, Conclusions[ConclusionId], Conclusions[Title])` |
| **UIScreens.PrimaryConclusionEvidence** | lookup | `LOOKUP(PrimaryConclusion, Conclusions[ConclusionId], Conclusions[Evidence])` |
| **AllocationSweep.IsOriginal** | formula | `ABS(AllocFractionA - LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[OriginalAllocFractionA])) < 0.03` |
| **AllocationSweep.NSweepStratumTotal** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NSweepStratumTotal])` |
| **AllocationSweep.NSweepA** | formula | `Round(NSweepStratumTotal * AllocFractionA)` |
| **AllocationSweep.NSweepB** | formula | `NSweepStratumTotal - NSweepA` |
| **AllocationSweep.NFixedA** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NFixedA])` |
| **AllocationSweep.NFixedB** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[NFixedB])` |
| **AllocationSweep.SweepRateA** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepRateA])` |
| **AllocationSweep.SweepRateB** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepRateB])` |
| **AllocationSweep.FixedRateA** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[FixedRateA])` |
| **AllocationSweep.FixedRateB** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[FixedRateB])` |
| **AllocationSweep.SweepPooledRateA** | formula | `If(NSweepA + NFixedA = 0, "", NSweepA * SweepRateA + NFixedA * FixedRateA / NSweepA + NFixedA)` |
| **AllocationSweep.SweepPooledRateB** | formula | `If(NSweepB + NFixedB = 0, "", NSweepB * SweepRateB + NFixedB * FixedRateB / NSweepB + NFixedB)` |
| **AllocationSweep.SweepPooledGap** | formula | `If(SweepPooledRateA = "", "", SweepPooledRateA - SweepPooledRateB)` |
| **AllocationSweep.SweepCorrectedGap** | formula | `LOOKUP(StudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepCorrectedGap])` |
| **AllocationSweep.AllocationDistortionWitness** | formula | `If(SweepPooledGap = "", "", SweepPooledGap - SweepCorrectedGap)` |
| **SweepStudySummary.DistortionTypeLabel** | formula | `LOOKUP(SweepStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType])` |
| **SweepStudySummary.CorrectedGapConstant** | formula | `Min(AllocationSweep.SweepCorrectedGap via StudyId)` |
| **SweepStudySummary.SweepCorrectedGapMax** | formula | `Max(AllocationSweep.SweepCorrectedGap via StudyId)` |
| **SweepStudySummary.SweepCorrectedGapMin** | formula | `Min(AllocationSweep.SweepCorrectedGap via StudyId)` |
| **SweepStudySummary.SweepCorrectedGapRange** | formula | `SweepCorrectedGapMax - SweepCorrectedGapMin` |
| **SweepStudySummary.SweepPooledGapMax** | formula | `Max(AllocationSweep.SweepPooledGap via StudyId)` |
| **SweepStudySummary.SweepPooledGapMin** | formula | `Min(AllocationSweep.SweepPooledGap via StudyId)` |
| **SweepStudySummary.SweepPooledGapRange** | formula | `SweepPooledGapMax - SweepPooledGapMin` |
| **SweepStudySummary.PooledGapCrossesZero** | formula | `And(SweepPooledGapMin < 0, SweepPooledGapMax > 0)` |
| **SweepStudySummary.InvariantWitness** | formula | `If(SweepCorrectedGapRange < 0.0001, "PASS: CorrectedGap invariant across allocation sweep", "FAIL: CorrectedGap varies — formula error")` |
| **SweepStudySummary.SweepStratumLabel** | formula | `LOOKUP(SweepStudyId, SweepStudyConfig[StudyId], SweepStudyConfig[SweepStratumLabel])` |
| **ResearchTraditions.IllustratedByStudyTitle** | lookup | `LOOKUP(IllustratedByStudy, Studies[StudyId], Studies[Title])` |
| **ResearchTraditions.IllustratedByStudySource** | lookup | `LOOKUP(IllustratedByStudy, Studies[StudyId], Studies[Source])` |
| **ResearchTraditions.IllustratedByStudyDistortionType** | lookup | `LOOKUP(IllustratedByStudy, TreatmentRankings[Study], TreatmentRankings[DistortionType])` |
| **ResearchTraditions.SupportingConclusionTitle** | lookup | `LOOKUP(SupportingConclusion, Conclusions[ConclusionId], Conclusions[Title])` |
| **ResearchTraditions.SupportingConclusionCategory** | lookup | `LOOKUP(SupportingConclusion, Conclusions[ConclusionId], Conclusions[Category])` |
| **ResearchTraditions.ResearcherCount** | rollup | `Count(Researchers via TraditionId)` |
| **ResearchTraditions.StudyCount** | rollup | `Count(Studies via TraditionId)` |
| **ResearchTraditions.LoopCount** | rollup | `Count(Loops via TraditionId)` |
| **Researchers.TraditionName** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Name])` |
| **Researchers.TraditionEra** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[Era])` |
| **Researchers.CanonicalStudyTitle** | lookup | `LOOKUP(CanonicalStudyId, Studies[StudyId], Studies[Title])` |
| **Researchers.CanonicalStudyDistortionType** | lookup | `LOOKUP(CanonicalStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType])` |
| **Researchers.CanonicalStudyPolicyImplication** | lookup | `LOOKUP(CanonicalStudyId, TreatmentRankings[Study], TreatmentRankings[CorrectedPolicyImplication])` |
| **Researchers.IllustratesConclusionTitle** | lookup | `LOOKUP(IllustratesConclusion, Conclusions[ConclusionId], Conclusions[Title])` |
| **Researchers.TraditionKeyClaimSummary** | lookup | `LOOKUP(TraditionId, ResearchTraditions[TraditionId], ResearchTraditions[KeyClaim])` |
| **SyntheticPhase.PhaseS1Total** | formula | `Round(200 * ParamStratumFraction)` |
| **SyntheticPhase.PhaseNA1** | formula | `Round(PhaseS1Total * 0.5 + ParamAllocationBias / 2)` |
| **SyntheticPhase.PhaseNB1** | formula | `PhaseS1Total - PhaseNA1` |
| **SyntheticPhase.PhaseNA2** | formula | `100 - PhaseNA1` |
| **SyntheticPhase.PhaseNB2** | formula | `100 - PhaseNB1` |
| **SyntheticPhase.PhaseRateA1** | formula | `0.45 + ParamStratumGap1 / 2` |
| **SyntheticPhase.PhaseRateB1** | formula | `0.45 - ParamStratumGap1 / 2` |
| **SyntheticPhase.PhaseRateA2** | formula | `0.75 + ParamStratumGap2 / 2` |
| **SyntheticPhase.PhaseRateB2** | formula | `0.75 - ParamStratumGap2 / 2` |
| **SyntheticPhase.PhasePooledRateA** | formula | `PhaseNA1 * PhaseRateA1 + PhaseNA2 * PhaseRateA2 / 100` |
| **SyntheticPhase.PhasePooledRateB** | formula | `PhaseNB1 * PhaseRateB1 + PhaseNB2 * PhaseRateB2 / 100` |
| **SyntheticPhase.PhaseSignedPooledGap** | formula | `PhasePooledRateA - PhasePooledRateB` |
| **SyntheticPhase.PhaseCorrectedGap** | formula | `ParamStratumFraction * ParamStratumGap1 + 1 - ParamStratumFraction * ParamStratumGap2` |
| **SyntheticPhase.PhaseStrataWonByLoser** | formula | `If(PhaseSignedPooledGap > 0, If(ParamStratumGap1 < 0, 1, 0) + If(ParamStratumGap2 < 0, 1, 0), If(ParamStratumGap1 > 0, 1, 0) + If(ParamStratumGap2 > 0, 1, 0))` |
| **SyntheticPhase.PhaseReversalIntensity** | formula | `PhaseStrataWonByLoser / 2` |
| **SyntheticPhase.PhaseIsSignFlip** | formula | `If(PhaseCorrectedGap > 0, PhaseSignedPooledGap < 0, PhaseSignedPooledGap > 0)` |
| **SyntheticPhase.PhaseAllocationDistortion** | formula | `Abs(PhaseCorrectedGap - PhaseSignedPooledGap)` |
| **SyntheticPhase.PhaseDistortionType** | formula | `If(And(PhaseIsSignFlip, PhaseReversalIntensity = 1), "A", If(And(PhaseIsSignFlip, PhaseReversalIntensity < 1), "B", If(And(Not(PhaseIsSignFlip), PhaseAllocationDistortion > 0.01, Abs(PhaseSignedPooledGap) > Abs(PhaseCorrectedGap) + 0.001), "C+", If(And(Not(PhaseIsSignFlip), PhaseAllocationDistortion > 0.01, Abs(PhaseSignedPooledGap) < Abs(PhaseCorrectedGap) - 0.001), "C-", "D"))))` |
| **PhaseDiagramSummary.PhasePointCount** | formula | `PhaseTypeACount + PhaseTypeBCount + PhaseTypeCPlusCount + PhaseTypeCMinusCount + PhaseTypeDCount` |
| **PhaseDiagramSummary.AllFiveTypesPresent** | formula | `And(PhaseTypeACount > 0, PhaseTypeBCount > 0, PhaseTypeCPlusCount > 0, PhaseTypeCMinusCount > 0, PhaseTypeDCount > 0)` |
| **PhaseDiagramSummary.PhaseTaxonomyWitness** | formula | `Concat("A:", PhaseTypeACount, " B:", PhaseTypeBCount, " C+:", PhaseTypeCPlusCount, " C-:", PhaseTypeCMinusCount, " D:", PhaseTypeDCount)` |
| **PhaseDiagramSummary.PhaseWitnessNote** | formula | `If(AllFiveTypesPresent, "PASS: all five distortion types populated in parameter space", "FAIL: missing distortion type in grid")` |
| **IngestionSummary.ProtocolItemCount** | rollup | `Count(IngestionProtocol via ProtocolId)` |
| **IngestionSummary.CorpusCellCount** | rollup | `Count(CaseCells via CaseCellId)` |
| **IngestionSummary.ValidCellCount** | rollup | `Count(CaseCells via IsValidIngestionCell)` |
| **IngestionSummary.AllCellsValid** | formula | `ValidCellCount = CorpusCellCount` |
| **IngestionSummary.StudyCount** | rollup | `Count(Studies via StudyId)` |
| **IngestionSummary.StructuralCompliantCount** | rollup | `Count(Studies via IngestionCellParity)` |
| **IngestionSummary.AllStudiesStructural** | formula | `StructuralCompliantCount = StudyCount` |
| **IngestionSummary.RealStudyCount** | rollup | `Count(Studies via IsSynthetic)` |
| **IngestionSummary.RealFullyCompliantCount** | rollup | `Count(Studies via IngestionCompliance)` |
| **IngestionSummary.AllRealStudiesCompliant** | formula | `RealFullyCompliantCount = RealStudyCount` |
| **IngestionSummary.IngestionContractPasses** | formula | `And(AllCellsValid, AllStudiesStructural, AllRealStudiesCompliant)` |
| **IngestionSummary.IngestionWitnessNote** | formula | `If(IngestionContractPasses, Concat("PASS: ", ProtocolItemCount, " contract items; ", CorpusCellCount, " cells valid; ", RealFullyCompliantCount, "/", RealStudyCount, " real studies fully compliant"), "FAIL: ingestion contract violated")` |
| **CandidateStudyCatalog.IsReadyToEncode** | formula | `And(IngestionStatus = "candidate", ProposedStudyId <> "", Citation <> "", StratumVariableName <> "")` |
| **CandidateStudyCatalog.IsImported** | formula | `IngestionStatus = "imported"` |
| **CandidateStudyCatalog.ObservedDistortionType** | lookup | `LOOKUP(LinkedStudyId, TreatmentRankings[Study], TreatmentRankings[DistortionType])` |
| **CandidateStudyCatalog.TypePredictionMatch** | formula | `If(IsImported, ObservedDistortionType = ExpectedDistortionType, "")` |
| **CandidateStudyCatalog.ExpectedSignFlip** | formula | `Or(ExpectedDistortionType = "A", ExpectedDistortionType = "B")` |
| **CandidateStudyCatalog.ObservedSignFlipType** | formula | `If(IsImported, Or(ObservedDistortionType = "A", ObservedDistortionType = "B"), "")` |
| **CandidateStudyCatalog.SignFlipPredictionMatch** | formula | `If(And(IsImported, ExpectedSignFlip), ObservedSignFlipType, "")` |
| **CandidateStudyCatalog.IsDataReady** | formula | `Or(DataAcquisitionStatus = "downloaded", DataAcquisitionStatus = "manual_only")` |
| **CorpusCatalogSummary.TotalCatalogEntries** | rollup | `Count(CandidateStudyCatalog via CandidateId)` |
| **CorpusCatalogSummary.ImportedCount** | rollup | `Count(CandidateStudyCatalog via IngestionStatus)` |
| **CorpusCatalogSummary.CandidateCount** | rollup | `Count(CandidateStudyCatalog via IngestionStatus)` |
| **CorpusCatalogSummary.BlockedCount** | rollup | `Count(CandidateStudyCatalog via IngestionStatus)` |
| **CorpusCatalogSummary.ReadyToEncodeCount** | rollup | `Count(CandidateStudyCatalog via IsReadyToEncode)` |
| **CorpusCatalogSummary.HighPriorityCount** | rollup | `Count(CandidateStudyCatalog via IngestionStatus) + Count(CandidateStudyCatalog via IngestionStatus)` |
| **CorpusCatalogSummary.ImportSessionReady** | formula | `And(CandidateCount >= 20, ReadyToEncodeCount >= 15)` |
| **CorpusCatalogSummary.TypePredictionMatchCount** | rollup | `Count(CandidateStudyCatalog via TypePredictionMatch)` |
| **CorpusCatalogSummary.TypePredictionMismatchCount** | rollup | `Count(CandidateStudyCatalog via IsImported)` |
| **CorpusCatalogSummary.SignFlipPredictionEligibleCount** | rollup | `Count(CandidateStudyCatalog via IsImported)` |
| **CorpusCatalogSummary.SignFlipPredictionMatchCount** | rollup | `Count(CandidateStudyCatalog via SignFlipPredictionMatch)` |
| **CorpusCatalogSummary.TypePredictionMatchRate** | formula | `If(ImportedCount = 0, "", TypePredictionMatchCount / ImportedCount)` |
| **CorpusCatalogSummary.SignFlipPredictionMatchRate** | formula | `If(SignFlipPredictionEligibleCount = 0, "", SignFlipPredictionMatchCount / SignFlipPredictionEligibleCount)` |
| **CorpusCatalogSummary.CatalogWitnessNote** | formula | `If(ImportSessionReady, Concat("READY: ", ReadyToEncodeCount, " encode-ready (", DataReadyCount, " data-ready) of ", CandidateCount, " candidates"), Concat("BUILDING: ", CandidateCount, " candidates — ", DataReadyCount, " data-ready, ", ReadyToEncodeCount, " encode-ready"))` |
| **CorpusCatalogSummary.CatalogPredictionWitnessNote** | formula | `CONCAT("exact=", TypePredictionMatchCount, "/", ImportedCount, " (", ROUND(TypePredictionMatchRate * 100, 1), "%); flipPred=", SignFlipPredictionMatchCount, "/", SignFlipPredictionEligibleCount, " (", ROUND(SignFlipPredictionMatchRate * 100, 1), "%)"))` |
| **CorpusCatalogSummary.DataReadyCount** | rollup | `Count(CandidateStudyCatalog via IsDataReady)` |
| **CorpusCatalogSummary.EncodePipelineReadyCount** | rollup | `Count(CandidateStudyCatalog via IsReadyToEncode)` |
| **CorpusCatalogSummary.ExpansionCandidateCount** | rollup | `Count(CandidateStudyCatalog via ExpansionWave)` |
| **DomainExpansionTargets.CurrentImportedCount** | rollup | `Count(Studies via Domain)` |
| **DomainExpansionTargets.CandidateQueuedCount** | rollup | `Count(CandidateStudyCatalog via Domain)` |
| **DomainExpansionTargets.ProjectedCount** | formula | `CurrentImportedCount + CandidateQueuedCount` |
| **DomainExpansionTargets.GapCount** | formula | `Max(0, TargetMinCount - ProjectedCount)` |
| **DomainExpansionTargets.IsUnderRepresented** | formula | `ProjectedCount < TargetMinCount` |
| **DiscoveryFindings.HypothesisStatement** | lookup | `LOOKUP(HypothesisId, DiscoveryHypotheses[HypothesisId], DiscoveryHypotheses[Statement])` |
| **DiscoveryFindings.ObservedMetric** | formula | `IF(HypothesisId = "H-latent-d", CONCAT("fraction=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[LatentTypeDFraction])), IF(HypothesisId = "H-purity", CONCAT("maxPurity=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SignFlipSignalPurityMax])), IF(HypothesisId = "H-small-effect", CONCAT("stable=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapStableD]), " latent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapLatentD])), IF(HypothesisId = "H-econ-zero", CONCAT("flips=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EconomicsSignFlipCount])), IF(HypothesisId = "H-domain-dist", CONCAT("epi=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EpidemiologyAvgDistortion]), " edu=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EducationAvgDistortion])), IF(HypothesisId = "H-causal-manifest", CONCAT("confLatent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderLatentOnlyCount]), " confFlip=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount])), IF(HypothesisId = "H-causal-latent", CONCAT("collManifest=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]), " collLatent=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionLatentOnlyCount]), " collN=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionCount])), IF(HypothesisId = "H-explained-confounder", CONCAT("ExplainedConfounderCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]), ", ConfounderSignFlipCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount])), IF(HypothesisId = "H-unexplained-nonconfounder", CONCAT("ContestedOrMediatorExplainedCount=", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ContestedOrMediatorExplainedCount])), ""))))))` |
| **DiscoveryFindings.IsConfirmed** | formula | `IF(HypothesisId = "H-latent-d", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[LatentTypeDFraction]) > 0.5, IF(HypothesisId = "H-purity", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SignFlipSignalPurityMax]) < 0.5, IF(HypothesisId = "H-small-effect", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapStableD]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[AvgPooledGapLatentD]), IF(HypothesisId = "H-econ-zero", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EconomicsSignFlipCount]) = 0, IF(HypothesisId = "H-domain-dist", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EpidemiologyAvgDistortion]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[EducationAvgDistortion]), IF(HypothesisId = "H-causal-manifest", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]) > LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]), LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]) >= 10), IF(HypothesisId = "H-causal-latent", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionManifestCount]) = 0, LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ColliderSelectionLatentOnlyCount]) >= 5), IF(HypothesisId = "H-explained-confounder", AND(LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]) = LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ConfounderSignFlipCount]), LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ExplainedConfounderCount]) >= 10), IF(HypothesisId = "H-unexplained-nonconfounder", LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[ContestedOrMediatorExplainedCount]) = 0, FALSE())))))` |
| **DiscoveryFindings.Evidence** | formula | `If(IsConfirmed = True(), Concat("PASS: ", ObservedMetric), If(IsConfirmed = False(), Concat("FAIL: ", ObservedMetric), ""))` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
