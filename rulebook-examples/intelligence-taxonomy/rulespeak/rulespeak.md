# 📘 Taxonomy of Intelligence — RuleSpeak

_Classifies intelligences (humans, animals, AI) by per-capability assessments through a multi-hop DAG._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Capability** | Cognitive capabilities being assessed. Tiered foundational/composite/emergent and weighted by importance. |
| **Intelligence** | Agents being classified by the taxonomy. Each Intelligence's TaxonomyClass is derived from the rollup of its Assessments. |
| Assessment Count | How many capabilities this intelligence has been assessed on. |
| Total Weighted Score | Sum of WeightedScore across all this intelligence's assessments. Second hop in the DAG. |
| Taxonomy Class | Taxonomic bucket derived from TotalWeightedScore. Third hop in the DAG. |
| **Assessment** | Per-capability scores for each intelligence. The junction table whose calculated WeightedScore feeds the rollup on Intelligences. |
| Intelligence Name | Display name pulled from the related Intelligences row. |
| Capability Name | Display name pulled from the related Capabilities row. |
| Capability Tier | Tier (foundational/composite/emergent) pulled from the related Capabilities row. |
| Capability Weight | Importance multiplier pulled from the related Capabilities row. |
| Weighted Score | RawScore scaled by the capability's Weight. First hop in the DAG. |

## 2 Fact Types

- an **assessment** references exactly one **intelligence**
- an **assessment** references exactly one **capability**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Assessment Count** | An intelligence's assessment count is the number of assessments related to the intelligence. |
| **DR-2 Total Weighted Score** | An intelligence's total weighted score is the total weighted score across the assessments related to the intelligence. |
| **DR-3 Taxonomy Class** | The intelligence's taxonomy class is determined by the following priority:<br>1. the literal “Generalist”, if the total weighted score is at least 350;<br>2. the literal “Broad”, if the total weighted score is at least 220;<br>3. otherwise the literal “Narrow”. |
| **DR-4 Intelligence Name** | An assessment's intelligence name is the name of the assessment's intelligence. |
| **DR-5 Capability Name** | An assessment's capability name is the name of the assessment's capability. |
| **DR-6 Capability Tier** | An assessment's capability tier is the tier of the assessment's capability. |
| **DR-7 Capability Weight** | An assessment's capability weight is the weight of the assessment's capability. |
| **DR-8 Weighted Score** | An assessment's weighted score is computed as `RawScore*CapabilityWeight`. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Intelligences.AssessmentCount** | rollup | `Count(Assessments via Intelligence)` |
| **Intelligences.TotalWeightedScore** | rollup | `Sum(Assessments.WeightedScore via Intelligence)` |
| **Intelligences.TaxonomyClass** | formula | `If(TotalWeightedScore>=350, "Generalist", If(TotalWeightedScore>=220, "Broad", "Narrow"))` |
| **Assessments.IntelligenceName** | lookup | `Lookup(Intelligences.Name via Intelligence)` |
| **Assessments.CapabilityName** | lookup | `Lookup(Capabilities.Name via Capability)` |
| **Assessments.CapabilityTier** | lookup | `Lookup(Capabilities.Tier via Capability)` |
| **Assessments.CapabilityWeight** | lookup | `Lookup(Capabilities.Weight via Capability)` |
| **Assessments.WeightedScore** | formula | `RawScore*CapabilityWeight` |
