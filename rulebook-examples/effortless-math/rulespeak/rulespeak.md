# 📘 effortless-math — RuleSpeak

_Executable theorem network starter. Fermat's Last Theorem is the deeply modeled flagship domain; seven foundation theorems begin as first-class provider contracts with imported universal content._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Domain** | A domain is identified by its name. | — |
| **Proof Status** | A proof status is identified by its name. | — |
| **Theorem** | A theorem is identified by its name. | — |
| **Theorem Dependency** | A theorem dependency is identified by its name. | — |
| **Foundation Kernel** | A foundation kernel is identified by its name. | — |
| Name | The same as its kernel ID. | _Name_ |
| **Proof Fact** | A proof fact is identified by its name. | — |
| **Loop** | A loop is identified by its name. | — |
| **Invariant Check** | An invariant check is identified by its name. | — |
| **Source** | A source is identified by its name. | — |
| **Trust Boundary** | A trust boundary is identified by its name. | — |
| **Artifact Registry** | An artifact registry is identified by its name. | — |
| **Migration Mapping** | A migration mapping is identified by its name. | — |
| **Project Roadmap** | A project roadmap is identified by its name. | — |
| **Conclusion** | A conclusion is identified by its name. | — |
| **Legacy Parent Audit** | A legacy parent audit is identified by its name. | — |
| Name | The same as its edge ID. | _Name_ |

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A domain **must** have a slug, a title, a role, a status, a description, and a relative path, and record whether it is a flagship.
- A proof status **must** have a sort order and a description, and record whether it is imported, whether it is derived, whether it is allows imported children, and whether it is a terminal.
- A theorem **must** have a title, a theorem kind, a statement, a scope, a trust boundary, an active import count, a contract path, a current version, and a next target, and record whether it is a flagship.
- A theorem dependency **must** have a required conclusion, a provider contract path, and a notes, and record whether it is load bearing, whether it is imported, and whether it is a shared kernel.
- A foundation kernel **must** have a kernel order, a kernel ID, a title, a mathematical scope, an input contract JSON, an output contract JSON, a finite compiler tables JSON, a source ID, a theorem content internalized, a status, and a next target.
- A proof fact **must** have a step no, a derived fact, an antecedents JSON, and a derivation kind, and record whether it is contradiction.
- A loop **must** have a loop order, a title, a status, a new concept, a domain question, a witness summary, a next frontier, and an epistemic tier.
- An invariant check **must** have a description, a tier, a pass count, a fail count, an universe count, a status, and an evidence.
- A source **must** have a citation, a source URL, a source kind, and a role.
- A trust boundary **must** have a boundary kind, an accepted external claims, a kernel or substrate, and a notes, and record whether it can claim zero imports.
- An artifact registry **must** have an artifact kind, a relative path, a version, a sha256, and a notes, and record whether it is a canonical migration source.
- A migration mapping **must** have a source table, a target table, a strategy, and a notes.
- A project roadmap **must** have a sequence no, a title, a status, an acceptance criteria, and a depends on.
- A conclusion **must** have a category, a status, a report tier, a title, an evidence, and a witnessed in loop.
- A legacy parent audit **must** have an edge order, an edge ID, a previous status, a current status, a parent removed, a replacement kernel ID, a kernel shared with parent count, a theorem content fully internalized, a status, and a next target.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A foundation kernel's name is the same as its kernel ID. |
| **DR-2 Name** | A legacy parent audit's name is the same as its edge ID. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **FoundationKernels.Name** | formula | `kernel_id` |
| **LegacyParentAudits.Name** | formula | `edge_id` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
