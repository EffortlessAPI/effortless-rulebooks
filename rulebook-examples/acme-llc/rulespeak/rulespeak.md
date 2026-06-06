# 📘 ACME, LLC — RuleSpeak

_Smallest viable rulebook with a calculated field — the "Hello, formulas" tutorial._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Customer** | A customer tracked by the business. |
| Initials | Computed as the first 1 character(s) of the first name, followed by the first 1 character(s) of the last name. |
| Full Name | Full name is computed from the first and last name of the customer |
| **ERB Version** | An ERB version tracked by the business. |
| **ERB Customization** | An ERB customization tracked by the business. |

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Initials** | A customer's initials is computed as the first 1 character(s) of the first name, followed by the first 1 character(s) of the last name. |
| **DR-2 Full Name** | A customer's full name is computed as the last name, followed by a comma followed by a space, followed by the first name. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Customers.Initials** | formula | `Left(FirstName, 1) & Left(LastName, 1)` |
| **Customers.FullName** | formula | `LastName & ", " & FirstName` |
