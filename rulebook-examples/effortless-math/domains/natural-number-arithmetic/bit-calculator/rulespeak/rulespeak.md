# 📘 bit-calculator — RuleSpeak

_A half adder, and nothing else. Two input bits a and b; the DB computes sum = a XOR b and carry = a AND b by LOOKING THEM UP in gate truth tables. No arithmetic is stored. The output bit of every computation is a calculated field the substrate derives from truth-table rows._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Gate Type** | A gate type is identified by its name. | — |
| Name | The same as its gate type ID. | _Name_ |
| **Gate Truth Row** | A gate truth row is identified by its name and is related to a gate type (its gate type ID). | — |
| Name | The same as its truth row ID. | _Name_ |
| **Computation** | A computation is identified by its name and is related to a gate type (its gate type ID). | — |
| Name | The same as its computation ID. | _Name_ |
| my lookup key | Computed as the gate type ID, followed by “|”, followed by the a, followed by “|”, followed by the b. | _This computation's gate+inputs; equals exactly one GateTruthRows.truth_row_id_ |
| out bit | Taken from the linked my lookup key. | _THE ANSWER. The DB looks up this computation's output bit in the gate truth table by matching its gate+inputs key against the truth row's id. Not stored, not hand-computed — derived by the substrate from truth-table rows._ |

## 2 Fact Types

- a **gate truth row** references exactly one **gate type**
- a **computation** references exactly one **gate type**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A gate type **must** have a gate type ID.
- A gate truth row **must** reference exactly one gate type as its gate type ID.
- A gate truth row **must** have a truth row ID, an in0, an in1, and an out bit.
- A computation **must** reference exactly one gate type as its gate type ID.
- A computation **must** have a computation ID, an a, and a b.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A gate type's name is the same as its gate type ID. |
| **DR-2 Name** | A gate truth row's name is the same as its truth row ID. |
| **DR-3 Name** | A computation's name is the same as its computation ID. |
| **DR-4 my lookup key** | A computation's my lookup key is computed as the gate type ID, followed by “|”, followed by the a, followed by “|”, followed by the b. |
| **DR-5 out bit** | A computation's out bit — taken from the linked my lookup key. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **GateTypes.Name** | formula | `gate_type_id` |
| **GateTruthRows.Name** | formula | `truth_row_id` |
| **Computations.Name** | formula | `computation_id` |
| **Computations.my_lookup_key** | formula | `Concat(gate_type_id, "\|", a, "\|", b)` |
| **Computations.out_bit** | lookup | `Lookup(GateTruthRows.out_bit via my_lookup_key)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
