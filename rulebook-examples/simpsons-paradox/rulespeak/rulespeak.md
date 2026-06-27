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

## 2 Fact Types

- a **treatment** references exactly one **study**
- a **strata** references exactly one **study**
- a **case cell** references exactly one **study**

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

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
