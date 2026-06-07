# 📘 ERB Self-Describing Rulebook — RuleSpeak

_The rulebook that describes the ERB project itself — the platform eating its own dog food._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Project Metadata** | Project overview |
| **Execution Substrate** | Runtime environments that execute business rules derived from the rulebook |
| **Orchestration Component** | Central orchestration logic that coordinates rulebook loading, injection, and testing |
| **Airtable Integration** | Airtable as input spoke: schema + data source pulled into rulebook |
| **Testing Framework** | Conformance testing: prove all substrates compute identically |
| **Rulebook Domain** | Customer ontologies: each domain has its own rulebook + substrate generation |
| **Core Data Flow** | End-to-end flows from rulebook to execution and testing |
| **Project Configuration** | Configuration files and their purposes |
| **Dependency** | External tools and their roles |

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| — | _This rulebook defines no calculated fields; all data is raw._ |
