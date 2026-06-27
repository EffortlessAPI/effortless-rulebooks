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
| **DR-20 Is Confounder** | A stratum variable is considered a confounder if all of the following hold: the affects treatment assignment flag is set; the affects outcome flag is set; and the causal role is “confounder”. |
| **DR-21 Total Cases a** | A treatment ranking's total cases a is the total cases across the case cells related to the treatment ranking. |
| **DR-22 Total Successes a** | A treatment ranking's total successes a is the total successes across the case cells related to the treatment ranking. |
| **DR-23 Pooled Rate a** | The treatment ranking's pooled rate a is determined by the following priority:<br>1. an empty string, if the total cases a is 0;<br>2. in all other cases, the total successes a divided by the total cases a. |
| **DR-24 Total Cases B** | A treatment ranking's total cases b is the total cases across the case cells related to the treatment ranking. |
| **DR-25 Total Successes B** | A treatment ranking's total successes b is the total successes across the case cells related to the treatment ranking. |
| **DR-26 Pooled Rate B** | The treatment ranking's pooled rate b is determined by the following priority:<br>1. an empty string, if the total cases b is 0;<br>2. in all other cases, the total successes b divided by the total cases b. |
| **DR-27 Pooled Winner** | The treatment ranking's pooled winner is determined by the following priority:<br>1. the treatment a, if the pooled rate a is greater than the pooled rate b;<br>2. in all other cases, the treatment b. |
| **DR-28 Stratum Count** | A treatment ranking's stratum count is the number of stratum summaries related to the treatment ranking. |
| **DR-29 Strata Won by a** | A treatment ranking's strata won by a is the number of stratum summaries related to the treatment ranking. |
| **DR-30 Strata Won by B** | A treatment ranking's strata won by b is the number of stratum summaries related to the treatment ranking. |
| **DR-31 Per Stratum Winner** | The treatment ranking's per stratum winner is determined by the following priority:<br>1. the treatment a, if the strata won by a is the stratum count;<br>2. the treatment b, if the strata won by b is the stratum count;<br>3. in all other cases, “none”. |
| **DR-32 Is Reversal** | A treatment ranking is considered a reversal if all of the following hold: the per stratum winner is not “none” and the pooled winner is not the per stratum winner. |
| **DR-33 Confounders in Study** | A treatment ranking's confounders in study is the number of the treatment ranking's stratum variables that are confounders. |
| **DR-34 Is Paradox Explained** | A treatment ranking is considered paradox-explained if all of the following hold: the reversal flag is set and the confounders in study is greater than 0. |
| **DR-35 Pooled Gap** | The treatment ranking's pooled gap is determined by the following priority:<br>1. an empty string, if the pooled rate a is blank;<br>2. in all other cases, the absolute value of the pooled rate a minus the pooled rate b. |
| **DR-36 Strata Won by Loser** | The treatment ranking's strata won by loser is determined by the following priority:<br>1. the strata won by b, if the pooled winner is the treatment a;<br>2. in all other cases, the strata won by a. |
| **DR-37 Paradox Strength** | The treatment ranking's paradox strength is determined by the following priority:<br>1. an empty string, if the stratum count is 0;<br>2. in all other cases, the pooled gap times the strata won by loser divided by the stratum count. |

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

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
