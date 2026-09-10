# 📘 a1-effortless-init-sample — RuleSpeak®

_A starter rulebook. Replace the HelloWhos entity with your own._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Hello Who** | The smallest complete rulebook: an id, a display name, and a rule that derives a greeting from it. | — |
| Name | A defined attribute. | — |
| Introduction | Computed as “Hi, ”, followed by the name, followed by a period. | — |
| Is Bob | True when at least one of the following holds: the name is “Bob”; the name is “Bobby”; or the name is “Robert”. | — |

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A hello who **must** have a name.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Introduction** | A hello who's introduction is computed as “Hi, ”, followed by the name, followed by a period. |
| **DR-2 Is Bob** | A hello who is considered a bob if at least one of the following holds: the name is “Bob”; the name is “Bobby”; or the name is “Robert”. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **HelloWhos.Introduction** | formula | `"Hi, " & Name & "."` |
| **HelloWhos.IsBob** | formula | `Or(Name = "Bob", Name = "Bobby", Name = "Robert")` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
