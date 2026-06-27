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
| **Study** | A study is identified by its name. | — |
| Total Cases | The total cases across the case cells related to the study. | _Total number of cases (trials) across all CaseCells in this study._ |
| Cell Count | The number of case cells related to the study. | _Number of CaseCell rows for this study._ |
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
| Reversal Count | The number of treatment rankings related to the model summary. | _Number of studies with IsReversal=TRUE (strict full reversal)._ |
| Non Reversal Count | The number of treatment rankings related to the model summary. | _Number of studies with IsReversal=FALSE._ |
| Study Count | Computed as the reversal count plus the non reversal count. | _Total number of TreatmentRankings in this model: ReversalCount + NonReversalCount._ |
| Explained Count | The number of treatment rankings related to the model summary. | _Number of studies with IsParadoxExplained=TRUE (reversal AND confirmed confounder)._ |
| Zero Strength Count | The number of treatment rankings related to the model summary. | _Studies with StrataWonByLoser=0: no strata go against the pooled winner. These are structurally paradox-free._ |
| Partial Count | Computed as the non reversal count minus the zero strength count. | _Studies with ParadoxStrength > 0 but IsReversal=FALSE: partial paradoxes real but not formally complete._ |
| Total Paradox Strength | The total paradox strength across the treatment rankings related to the model summary. | _Sum of ParadoxStrength across all studies. Used to compute average._ |
| Avg Paradox Strength | Determined by priority: an empty string if the study count is 0; in all other cases, the total paradox strength divided by the study count. | _Average ParadoxStrength across all studies: TotalParadoxStrength / StudyCount. A scalar summary of how paradox-rich this dataset is._ |
| **Stratum Variable** | A stratum variable is identified by its name and is related to a study. | — |
| Is Confounder | True when all of the following hold: the affects treatment assignment flag is set; the affects outcome flag is set; and the causal role is “confounder”. | _TRUE when AffectsTreatmentAssignment AND AffectsOutcome AND CausalRole = 'confounder'. A confounder is the classic driver of Simpson's Paradox._ |
| **Treatment Ranking** | A treatment ranking is identified by its name and is related to a study. | — |
| Total Cases a | The total cases across the case cells related to the treatment ranking. | _Total cases for TreatmentA across all strata in this study._ |
| Total Successes a | The total successes across the case cells related to the treatment ranking. | _Total successes for TreatmentA across all strata._ |
| Pooled Rate a | Determined by priority: an empty string if the total cases a is 0; in all other cases, the total successes a divided by the total cases a. | _Pooled success rate for TreatmentA: TotalSuccessesA / TotalCasesA._ |
| Total Cases B | The total cases across the case cells related to the treatment ranking. | _Total cases for TreatmentB across all strata._ |
| Total Successes B | The total successes across the case cells related to the treatment ranking. | _Total successes for TreatmentB across all strata._ |
| Pooled Rate B | Determined by priority: an empty string if the total cases b is 0; in all other cases, the total successes b divided by the total cases b. | _Pooled success rate for TreatmentB: TotalSuccessesB / TotalCasesB._ |
| Pooled Winner | Determined by priority: the treatment a if the pooled rate a is greater than the pooled rate b; in all other cases, the treatment b. | _Which treatment wins when strata are ignored: the one with the higher pooled rate._ |
| Stratum Count | The number of stratum summaries related to the treatment ranking. | _Number of strata in this study. Counted from StratumSummaries filtered to TreatmentA rows (one row per stratum)._ |
| Strata Won by a | The number of stratum summaries related to the treatment ranking. | _Number of strata where A wins: count StratumSummaries rows whose TreatmentLabel=TreatmentA AND StratumWinner=TreatmentA. These are the one-per-stratum sentinel rows._ |
| Strata Won by B | The number of stratum summaries related to the treatment ranking. | _Number of strata where B wins: count StratumSummaries rows whose TreatmentLabel=TreatmentA AND StratumWinner=TreatmentB._ |
| Per Stratum Winner | Determined by priority: the treatment a if the strata won by a is the stratum count; the treatment b if the strata won by b is the stratum count; in all other cases, “none”. | _The treatment that wins in every stratum — 'A' if StrataWonByA = StratumCount, 'B' if StrataWonByB = StratumCount, else 'none' (no unanimous per-stratum winner)._ |
| Is Reversal | True when all of the following hold: the per stratum winner is not “none” and the pooled winner is not the per stratum winner. | _TRUE when the pooled winner and the per-stratum winner disagree — i.e. Simpson's Paradox is present. FALSE otherwise. This is the paradox as a derived fact, not a modeled entity._ |
| Confounders in Study | The number of the treatment ranking's stratum variables that are confounders. | _Count of StratumVariables in this study whose IsConfounder = TRUE. When > 0, the paradox has a causal explanation._ |
| Is Paradox Explained | True when all of the following hold: the reversal flag is set and the confounders in study is greater than 0. | _TRUE when IsReversal is present AND at least one confirmed confounder exists in the study. The model witnesses its own explanatory completeness — or its limits._ |
| Pooled Gap | Determined by priority: an empty string if the pooled rate a is blank; in all other cases, the absolute value of the pooled rate a minus the pooled rate b. | _Absolute difference between the two pooled rates: \|PooledRateA - PooledRateB\|. The size of the aggregate misleading signal._ |
| Strata Won by Loser | Determined by priority: the strata won by b if the pooled winner is the treatment a; in all other cases, the strata won by a. | _Number of strata won by the pooled loser — the counter-signal. For full reversals this equals StratumCount; for partial paradoxes it is between 0 and StratumCount._ |
| Paradox Strength | Determined by priority: an empty string if the stratum count is 0; in all other cases, the pooled gap times the strata won by loser divided by the stratum count. | _Scalar severity of the paradox: PooledGap × (StrataWonByLoser / StratumCount). Zero when no strata go against the pooled winner. Positive for partial paradoxes. Maximum when every stratum contradicts the pooled result._ |
| Pooled Rate From Weights a | The total weighted stratum rate across the stratum summaries related to the treatment ranking. | _Pooled rate for TreatmentA reconstructed as a weighted average of stratum rates: SUM(WeightedStratumRate) across all stratum rows for TreatmentA. Must match PooledRateA — this witnesses the mechanism equation: the pooled rate IS a weighted average of stratum rates, weighted by StratumFraction._ |
| Pooled Rate From Weights B | The total weighted stratum rate across the stratum summaries related to the treatment ranking. | _Pooled rate for TreatmentB reconstructed as a weighted average. Must match PooledRateB. If it does, the mechanism equation is verified: reversal is purely a consequence of differential weighting._ |
| Reversal Intensity | Determined by priority: an empty string if the stratum count is 0; in all other cases, the strata won by loser divided by the stratum count. | _StrataWonByLoser / StratumCount: the fraction of strata that go against the pooled winner. Zero for no paradox, 1.0 for full reversal, between 0 and 1 for partial. This is the allocation-side measure of how deeply the confounding has penetrated the stratified view._ |
| Threshold Margin | Determined by priority: an empty string if the stratum count is 0; in all other cases, the reversal intensity minus 0.5. | _ReversalIntensity minus 0.5: positive when more than half the strata go against the pooled winner (reversal is robust), zero at the tipping point (exactly half), negative when fewer than half oppose the pooled winner (reversal is fragile or absent). A study with ThresholdMargin > 0 is robustly paradoxical; one with ThresholdMargin < 0 has a weak or absent paradox._ |
| Signed Pooled Gap | Determined by priority: an empty string if the pooled rate a is blank; in all other cases, the pooled rate a minus the pooled rate b. | _Signed difference between pooled rates: PooledRateA minus PooledRateB. Positive = A wins pooled; negative = B wins pooled. Unlike PooledGap (which is absolute), this preserves direction — essential for detecting sign flips between the equal-weight and actual pooled signals._ |
| Weighted Stratum Gap Sum | The total weighted stratum gap across the stratum summaries related to the treatment ranking. | _Sum of WeightedStratumGap (= StratumGap × StratumFraction) across all StratumSummaries rows where TreatmentLabel = TreatmentA for this study. This is the equal-weight pooled gap — the gap the pooled analysis would produce if allocation were perfectly balanced across strata. When this has the opposite sign to SignedPooledGap, the allocation has not merely compressed the signal: it has flipped it entirely._ |
| Is Sign Flip | True when the signed pooled gap is less than 0 if the weighted stratum gap sum is greater than 0, in all other cases the signed pooled gap is greater than 0. | _TRUE when WeightedStratumGapSum and SignedPooledGap have opposite signs — the allocation has reversed the direction of the pooled signal relative to what equal-weight strata would show. This is the continuous generalisation of IsReversal: it does not require every stratum to agree, only that the equal-weight estimate and the actual pooled estimate point in opposite directions. Berkeley 1973 satisfies IsSignFlip=TRUE despite IsReversal=FALSE._ |
| Allocation Distortion | Determined by priority: an empty string if the weighted stratum gap sum is blank; in all other cases, the absolute value of the weighted stratum gap sum minus the signed pooled gap. | _The magnitude of how far the allocation has bent the pooled signal: \|WeightedStratumGapSum − SignedPooledGap\|. Zero means the allocation is neutral — the pooled analysis faithfully represents the equal-weight stratum evidence. A large AllocationDistortion means the allocation is doing most of the work: the pooled number is mostly noise from how cases were assigned, not signal about which treatment is better. This measure does NOT require a sign flip to be nonzero — it captures any allocation-induced distortion, not just reversals._ |

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

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Total Cases** | A study's total cases is the total cases across the case cells related to the study. |
| **DR-2 Cell Count** | A study's cell count is the number of case cells related to the study. |
| **DR-3 Total Cases** | A treatment's total cases is the total cases across the case cells related to the treatment. |
| **DR-4 Total Successes** | A treatment's total successes is the total successes across the case cells related to the treatment. |
| **DR-5 Pooled Success Rate** | The treatment's pooled success rate is determined by the following priority:<br>1. an empty string, if the total cases is 0;<br>2. in all other cases, the total successes divided by the total cases. |
| **DR-6 Total Cases** | A strata's total cases is the total cases across the case cells related to the strata. |
| **DR-7 Cell Success Rate** | The case cell's cell success rate is determined by the following priority:<br>1. an empty string, if the cases is 0;<br>2. in all other cases, the successes divided by the cases. |
| **DR-8 Total Cases for Treatment** | A case cell's total cases for treatment is the total cases across the case cells related to the case cell. |
| **DR-9 Treatment Exposure Fraction** | The case cell's treatment exposure fraction is determined by the following priority:<br>1. an empty string, if the total cases for treatment is 0;<br>2. in all other cases, the cases divided by the total cases for treatment. |
| **DR-10 Stratum Successes** | A stratum summary's stratum successes is the total successes across the case cells related to the stratum summary. |
| **DR-11 Stratum Cases** | A stratum summary's stratum cases is the total cases across the case cells related to the stratum summary. |
| **DR-12 Stratum Success Rate** | The stratum summary's stratum success rate is determined by the following priority:<br>1. an empty string, if the stratum cases is 0;<br>2. in all other cases, the stratum successes divided by the stratum cases. |
| **DR-13 Stratum Successes a** | A stratum summary's stratum successes a is the total successes across the case cells related to the stratum summary. |
| **DR-14 Stratum Cases a** | A stratum summary's stratum cases a is the total cases across the case cells related to the stratum summary. |
| **DR-15 Stratum Rate a** | The stratum summary's stratum rate a is determined by the following priority:<br>1. an empty string, if the stratum cases a is 0;<br>2. in all other cases, the stratum successes a divided by the stratum cases a. |
| **DR-16 Stratum Successes B** | A stratum summary's stratum successes b is the total successes across the case cells related to the stratum summary. |
| **DR-17 Stratum Cases B** | A stratum summary's stratum cases b is the total cases across the case cells related to the stratum summary. |
| **DR-18 Stratum Rate B** | The stratum summary's stratum rate b is determined by the following priority:<br>1. an empty string, if the stratum cases b is 0;<br>2. in all other cases, the stratum successes b divided by the stratum cases b. |
| **DR-19 Stratum Winner** | The stratum summary's stratum winner is determined by the following priority:<br>1. “A”, if the stratum rate a is greater than the stratum rate b;<br>2. in all other cases, “B”. |
| **DR-20 Study Total Cases** | A stratum summary's study total cases is the total cases across the case cells related to the stratum summary. |
| **DR-21 Stratum Total Cases** | A stratum summary's stratum total cases is the total cases across the case cells related to the stratum summary. |
| **DR-22 Stratum Fraction** | The stratum summary's stratum fraction is determined by the following priority:<br>1. an empty string, if the study total cases is 0;<br>2. in all other cases, the stratum total cases divided by the study total cases. |
| **DR-23 Weighted Stratum Rate** | The stratum summary's weighted stratum rate is determined by the following priority:<br>1. an empty string, if the stratum fraction is blank;<br>2. in all other cases, the stratum fraction times the stratum success rate. |
| **DR-24 Treatment a Cases Here** | A stratum summary's treatment a cases here is the total cases across the case cells related to the stratum summary. |
| **DR-25 Treatment B Cases Here** | A stratum summary's treatment b cases here is the total cases across the case cells related to the stratum summary. |
| **DR-26 Treatment a Total Cases** | A stratum summary's treatment a total cases is the total cases across the stratum summary's case cells that have a treatment label of “A”. |
| **DR-27 Treatment B Total Cases** | A stratum summary's treatment b total cases is the total cases across the stratum summary's case cells that have a treatment label of “B”. |
| **DR-28 Allocation Fraction a** | The stratum summary's allocation fraction a is determined by the following priority:<br>1. an empty string, if the treatment a total cases is 0;<br>2. in all other cases, the treatment a cases here divided by the treatment a total cases. |
| **DR-29 Allocation Fraction B** | The stratum summary's allocation fraction b is determined by the following priority:<br>1. an empty string, if the treatment b total cases is 0;<br>2. in all other cases, the treatment b cases here divided by the treatment b total cases. |
| **DR-30 Allocation Bias** | The stratum summary's allocation bias is determined by the following priority:<br>1. an empty string, if the allocation fraction a is blank;<br>2. in all other cases, the allocation fraction a minus the allocation fraction b. |
| **DR-31 Stratum Gap** | The stratum summary's stratum gap is determined by the following priority:<br>1. an empty string, if the stratum rate a is blank;<br>2. in all other cases, the stratum rate a minus the stratum rate b. |
| **DR-32 Weighted Stratum Gap** | The stratum summary's weighted stratum gap is determined by the following priority:<br>1. an empty string, if the stratum gap is blank;<br>2. in all other cases, the stratum gap times the stratum fraction. |
| **DR-33 Reversal Count** | A model summary's reversal count is the number of treatment rankings related to the model summary. |
| **DR-34 Non Reversal Count** | A model summary's non reversal count is the number of treatment rankings related to the model summary. |
| **DR-35 Study Count** | A model summary's study count is computed as the reversal count plus the non reversal count. |
| **DR-36 Explained Count** | A model summary's explained count is the number of treatment rankings related to the model summary. |
| **DR-37 Zero Strength Count** | A model summary's zero strength count is the number of treatment rankings related to the model summary. |
| **DR-38 Partial Count** | A model summary's partial count is computed as the non reversal count minus the zero strength count. |
| **DR-39 Total Paradox Strength** | A model summary's total paradox strength is the total paradox strength across the treatment rankings related to the model summary. |
| **DR-40 Avg Paradox Strength** | The model summary's avg paradox strength is determined by the following priority:<br>1. an empty string, if the study count is 0;<br>2. in all other cases, the total paradox strength divided by the study count. |
| **DR-41 Is Confounder** | A stratum variable is considered a confounder if all of the following hold: the affects treatment assignment flag is set; the affects outcome flag is set; and the causal role is “confounder”. |
| **DR-42 Total Cases a** | A treatment ranking's total cases a is the total cases across the case cells related to the treatment ranking. |
| **DR-43 Total Successes a** | A treatment ranking's total successes a is the total successes across the case cells related to the treatment ranking. |
| **DR-44 Pooled Rate a** | The treatment ranking's pooled rate a is determined by the following priority:<br>1. an empty string, if the total cases a is 0;<br>2. in all other cases, the total successes a divided by the total cases a. |
| **DR-45 Total Cases B** | A treatment ranking's total cases b is the total cases across the case cells related to the treatment ranking. |
| **DR-46 Total Successes B** | A treatment ranking's total successes b is the total successes across the case cells related to the treatment ranking. |
| **DR-47 Pooled Rate B** | The treatment ranking's pooled rate b is determined by the following priority:<br>1. an empty string, if the total cases b is 0;<br>2. in all other cases, the total successes b divided by the total cases b. |
| **DR-48 Pooled Winner** | The treatment ranking's pooled winner is determined by the following priority:<br>1. the treatment a, if the pooled rate a is greater than the pooled rate b;<br>2. in all other cases, the treatment b. |
| **DR-49 Stratum Count** | A treatment ranking's stratum count is the number of stratum summaries related to the treatment ranking. |
| **DR-50 Strata Won by a** | A treatment ranking's strata won by a is the number of stratum summaries related to the treatment ranking. |
| **DR-51 Strata Won by B** | A treatment ranking's strata won by b is the number of stratum summaries related to the treatment ranking. |
| **DR-52 Per Stratum Winner** | The treatment ranking's per stratum winner is determined by the following priority:<br>1. the treatment a, if the strata won by a is the stratum count;<br>2. the treatment b, if the strata won by b is the stratum count;<br>3. in all other cases, “none”. |
| **DR-53 Is Reversal** | A treatment ranking is considered a reversal if all of the following hold: the per stratum winner is not “none” and the pooled winner is not the per stratum winner. |
| **DR-54 Confounders in Study** | A treatment ranking's confounders in study is the number of the treatment ranking's stratum variables that are confounders. |
| **DR-55 Is Paradox Explained** | A treatment ranking is considered paradox-explained if all of the following hold: the reversal flag is set and the confounders in study is greater than 0. |
| **DR-56 Pooled Gap** | The treatment ranking's pooled gap is determined by the following priority:<br>1. an empty string, if the pooled rate a is blank;<br>2. in all other cases, the absolute value of the pooled rate a minus the pooled rate b. |
| **DR-57 Strata Won by Loser** | The treatment ranking's strata won by loser is determined by the following priority:<br>1. the strata won by b, if the pooled winner is the treatment a;<br>2. in all other cases, the strata won by a. |
| **DR-58 Paradox Strength** | The treatment ranking's paradox strength is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the pooled gap times the strata won by loser divided by the stratum count. |
| **DR-59 Pooled Rate From Weights a** | A treatment ranking's pooled rate from weights a is the total weighted stratum rate across the stratum summaries related to the treatment ranking. |
| **DR-60 Pooled Rate From Weights B** | A treatment ranking's pooled rate from weights b is the total weighted stratum rate across the stratum summaries related to the treatment ranking. |
| **DR-61 Reversal Intensity** | The treatment ranking's reversal intensity is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the strata won by loser divided by the stratum count. |
| **DR-62 Threshold Margin** | The treatment ranking's threshold margin is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the reversal intensity minus 0.5. |
| **DR-63 Signed Pooled Gap** | The treatment ranking's signed pooled gap is determined by the following priority:<br>1. an empty string, if the pooled rate a is blank;<br>2. in all other cases, the pooled rate a minus the pooled rate b. |
| **DR-64 Weighted Stratum Gap Sum** | A treatment ranking's weighted stratum gap sum is the total weighted stratum gap across the stratum summaries related to the treatment ranking. |
| **DR-65 Is Sign Flip** | A treatment ranking is considered a sign flip if the signed pooled gap is less than 0 if the weighted stratum gap sum is greater than 0, in all other cases the signed pooled gap is greater than 0. |
| **DR-66 Allocation Distortion** | The treatment ranking's allocation distortion is determined by the following priority:<br>1. an empty string, if the weighted stratum gap sum is blank;<br>2. in all other cases, the absolute value of the weighted stratum gap sum minus the signed pooled gap. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Studies.TotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **Studies.CellCount** | rollup | `Count(CaseCells via Study)` |
| **Treatments.TotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **Treatments.TotalSuccesses** | rollup | `Sum(CaseCells.Successes via Study)` |
| **Treatments.PooledSuccessRate** | formula | `If(TotalCases = 0, "", TotalSuccesses / TotalCases)` |
| **Strata.TotalCases** | rollup | `Sum(CaseCells.Cases via Study)` |
| **CaseCells.CellSuccessRate** | formula | `If(Cases = 0, "", Successes / Cases)` |
| **CaseCells.TotalCasesForTreatment** | rollup | `Sum(CaseCells.Cases via Study)` |
| **CaseCells.TreatmentExposureFraction** | formula | `If(TotalCasesForTreatment = 0, "", Cases / TotalCasesForTreatment)` |
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
| **ModelSummary.ZeroStrengthCount** | rollup | `Count(TreatmentRankings via StrataWonByLoser)` |
| **ModelSummary.PartialCount** | formula | `NonReversalCount - ZeroStrengthCount` |
| **ModelSummary.TotalParadoxStrength** | rollup | `Sum(TreatmentRankings.ParadoxStrength via TreatmentA)` |
| **ModelSummary.AvgParadoxStrength** | formula | `If(StudyCount = 0, "", TotalParadoxStrength / StudyCount)` |
| **StratumVariables.IsConfounder** | formula | `And(AffectsTreatmentAssignment, AffectsOutcome, CausalRole = "confounder")` |
| **TreatmentRankings.TotalCasesA** | rollup | `Sum(CaseCells.Cases via Study)` |
| **TreatmentRankings.TotalSuccessesA** | rollup | `Sum(CaseCells.Successes via Study)` |
| **TreatmentRankings.PooledRateA** | formula | `If(TotalCasesA = 0, "", TotalSuccessesA / TotalCasesA)` |
| **TreatmentRankings.TotalCasesB** | rollup | `Sum(CaseCells.Cases via Study)` |
| **TreatmentRankings.TotalSuccessesB** | rollup | `Sum(CaseCells.Successes via Study)` |
| **TreatmentRankings.PooledRateB** | formula | `If(TotalCasesB = 0, "", TotalSuccessesB / TotalCasesB)` |
| **TreatmentRankings.PooledWinner** | formula | `If(PooledRateA > PooledRateB, TreatmentA, TreatmentB)` |
| **TreatmentRankings.StratumCount** | rollup | `Count(StratumSummaries via Study)` |
| **TreatmentRankings.StrataWonByA** | rollup | `Count(StratumSummaries via Study)` |
| **TreatmentRankings.StrataWonByB** | rollup | `Count(StratumSummaries via Study)` |
| **TreatmentRankings.PerStratumWinner** | formula | `If(StrataWonByA = StratumCount, TreatmentA, If(StrataWonByB = StratumCount, TreatmentB, "none"))` |
| **TreatmentRankings.IsReversal** | formula | `And(PerStratumWinner <> "none", PooledWinner <> PerStratumWinner)` |
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

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
