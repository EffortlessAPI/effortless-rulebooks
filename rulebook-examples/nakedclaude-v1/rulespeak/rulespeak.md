# 📘 v1: NakedClaude Demo — RuleSpeak

_Rulebook generated from Airtable base 'v1: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Customer** | A customer tracked by the business. |
| Is Approved | True when `If(CurrentColor` is `"Green", True(), False())`. |

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Is Approved** | A customer is considered approved if `If(CurrentColor` is `"Green", True(), False())`. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Customers.IsApproved** | formula | `If(CurrentColor="Green", True(), False())` |
