# 📘 v2: NakedClaude Demo — RuleSpeak

_Rulebook generated from Airtable base 'v2: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Customer** | A customer tracked by the business. |
| Full Name | Computed as the first name, followed by a space, followed by the last name. |
| Status Display Name | The display name of the customer's status. |
| Status is Blocking | True when the customer's status is blocking. |
| Is Stopped | True when `If(And(StatusIsBlocking, FirstName` is `"Bob"), True(), False())`. |
| Status Description | The description of the customer's status. |
| **Status** | A status tracked by the business. |
| **App User** | An app user tracked by the business. |

## 2 Fact Types

- a **customer** may reference one **status**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Full Name** | A customer's full name is computed as the first name, followed by a space, followed by the last name. |
| **DR-2 Status Display Name** | A customer's status display name is the display name of the customer's status. |
| **DR-3 Status is Blocking** | A customer's status is blocking is true when the customer's status is blocking. |
| **DR-4 Is Stopped** | A customer is considered stopped if `If(And(StatusIsBlocking, FirstName` is `"Bob"), True(), False())`. |
| **DR-5 Status Description** | A customer's status description is the description of the customer's status. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Customers.FullName** | formula | `FirstName & " " & LastName` |
| **Customers.StatusDisplayName** | lookup | `Lookup(Statuses.DisplayName via Status)` |
| **Customers.StatusIsBlocking** | lookup | `Lookup(Statuses.IsBlocking via Status)` |
| **Customers.IsStopped** | formula | `If(And(StatusIsBlocking, FirstName = "Bob"), True(), False())` |
| **Customers.StatusDescription** | lookup | `Lookup(Statuses.Description via Status)` |
