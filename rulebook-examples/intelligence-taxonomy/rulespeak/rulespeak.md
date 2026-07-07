# 📘 Taxonomy of Intelligence — RuleSpeak

_Classifies intelligences (humans, animals, AI) by per-capability assessments through a multi-hop DAG._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Capability** | Cognitive capabilities being assessed. Tiered foundational/composite/emergent and weighted by importance. | — |
| **Intelligence** | Agents being classified by the taxonomy. Each Intelligence's TaxonomyClass is derived from the rollup of its Assessments. | — |
| Assessment Count | The number of assessments related to the intelligence. | _How many capabilities this intelligence has been assessed on._ |
| Total Weighted Score | The total weighted score across the assessments related to the intelligence. | _Sum of WeightedScore across all this intelligence's assessments. Second hop in the DAG._ |
| Taxonomy Class | Determined by priority: “Generalist” if the total weighted score is at least 350; “Broad” if the total weighted score is at least 220; in all other cases, “Narrow”. | _Taxonomic bucket derived from TotalWeightedScore. Third hop in the DAG._ |
| **Assessment** | Per-capability scores for each intelligence. The junction table whose calculated WeightedScore feeds the rollup on Intelligences. | — |
| Intelligence Name | Taken from the linked intelligence. | _Display name pulled from the related Intelligences row._ |
| Capability Name | Taken from the linked capability. | _Display name pulled from the related Capabilities row._ |
| Capability Tier | Taken from the linked capability. | _Tier (foundational/composite/emergent) pulled from the related Capabilities row._ |
| Capability Weight | Taken from the linked capability. | _Importance multiplier pulled from the related Capabilities row._ |
| Weighted Score | Computed as the raw score times the capability weight. | _RawScore scaled by the capability's Weight. First hop in the DAG._ |

## 2 Fact Types

- an **assessment** references exactly one **intelligence**
- an **assessment** references exactly one **capability**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A capability **must** have a tier and a weight.
- An intelligence **must** have a substrate.
- An assessment **must** reference exactly one intelligence.
- An assessment **must** reference exactly one capability.
- An assessment **must** have a raw score.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Assessment Count** | An intelligence's assessment count is the number of assessments related to the intelligence. |
| **DR-2 Total Weighted Score** | An intelligence's total weighted score is the total weighted score across the assessments related to the intelligence. |
| **DR-3 Taxonomy Class** | The intelligence's taxonomy class is determined by the following priority:<br>1. “Generalist”, if the total weighted score is at least 350;<br>2. “Broad”, if the total weighted score is at least 220;<br>3. in all other cases, “Narrow”. |
| **DR-4 Intelligence Name** | An assessment's intelligence name — taken from the linked intelligence. |
| **DR-5 Capability Name** | An assessment's capability name — taken from the linked capability. |
| **DR-6 Capability Tier** | An assessment's capability tier — taken from the linked capability. |
| **DR-7 Capability Weight** | An assessment's capability weight — taken from the linked capability. |
| **DR-8 Weighted Score** | An assessment's weighted score is computed as the raw score times the capability weight. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Intelligences.AssessmentCount** | rollup | `Count(Assessments via Intelligence)` |
| **Intelligences.TotalWeightedScore** | rollup | `Sum(Assessments.WeightedScore via Intelligence)` |
| **Intelligences.TaxonomyClass** | formula | `If(TotalWeightedScore >= 350, "Generalist", If(TotalWeightedScore >= 220, "Broad", "Narrow"))` |
| **Assessments.IntelligenceName** | lookup | `Lookup(Intelligences.Name via Intelligence)` |
| **Assessments.CapabilityName** | lookup | `Lookup(Capabilities.Name via Capability)` |
| **Assessments.CapabilityTier** | lookup | `Lookup(Capabilities.Tier via Capability)` |
| **Assessments.CapabilityWeight** | lookup | `Lookup(Capabilities.Weight via Capability)` |
| **Assessments.WeightedScore** | formula | `RawScore * CapabilityWeight` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
