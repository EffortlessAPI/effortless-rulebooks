# 📘 Gym Trainer Invoicing — RuleSpeak®

_Sessions roll up into invoices; invoices roll up into client outstanding balances._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **User** | Login identities for the demo (stub auth). | — |
| Name | A defined attribute. | — |
| Email | A defined attribute. | — |
| Role | A defined attribute. | _trainer \| client \| admin_ |
| Display Name | A defined attribute. | — |
| **Trainer** | Personal trainers who log sessions and bill clients. | — |
| Name | A defined attribute. | — |
| Email | A defined attribute. | — |
| Full Name | A defined attribute. | — |
| Hourly Rate | A defined attribute. | _Default $/hour charged to clients._ |
| Total Sessions | The number of sessions related to the trainer. | — |
| Total Billed | The total total across the invoices related to the trainer. | — |
| Total Outstanding | The total balance across the invoices related to the trainer. | — |
| **Client** | Trainer's clients. | — |
| Name | A defined attribute. | — |
| Email | A defined attribute. | — |
| Full Name | A defined attribute. | — |
| Trainer | A defined attribute. | — |
| Trainer Name | The full name of the client's trainer. | — |
| Trainer Email | Taken from the linked trainer. | — |
| Trainer Hourly Rate | Taken from the linked trainer. | — |
| Session Count | The number of sessions related to the client. | — |
| Total Invoiced | The total total across the invoices related to the client. | — |
| Outstanding Balance | The total balance across the invoices related to the client. | — |
| Overdue Count | The number of the client's invoices that are overdues. | — |
| Status | Determined by priority: “Overdue” if the overdue count is greater than 0; “Has Balance” if the outstanding balance is greater than 0; in all other cases, “Paid Up”. | — |
| **Invoice** | An invoice bundles one or more sessions billed to a client. | — |
| Name | A defined attribute. | — |
| Client | A defined attribute. | — |
| Client Name | The full name of the invoice's client. | — |
| Client Email | Taken from the linked client. | — |
| Trainer | Taken from the linked client. | — |
| Trainer Name | Taken from the linked client. | — |
| Issue Date | A defined attribute. | — |
| Due Date | A defined attribute. | — |
| Tax Rate | A defined attribute. | _Decimal (e.g. 0.08 = 8%)._ |
| Discount Amount | A defined attribute. | — |
| Paid Amount | A defined attribute. | — |
| Subtotal | The total line total across the sessions related to the invoice. | — |
| Tax Amount | Computed as the subtotal times the tax rate. | — |
| Grace Days | Computed as 45. | _Grace period (in days) after the DueDate before an invoice is considered late._ |
| Days Since Due Date | Computed as the number of days from the due date to the current date and time. ⚠︎ mechanical <!-- rulespeak:reword --> | _Raw whole days from DueDate to now (can be negative if not yet due)._ |
| Days Past Due | Computed as the largest of 0 and the days since due date minus the grace days. | _Whole days past DueDate after the GraceDays window; 0 inside the grace window._ |
| Late Fee | Determined by priority: 15 if the days past due is greater than 0; in all other cases, 0. | _$15 flat fee once an invoice is past the 45-day grace period._ |
| Total | Computed as the subtotal plus the tax amount plus the late fee minus the discount amount. | — |
| Balance | Computed as the total minus the paid amount. | — |
| Is Paid | True when the balance is at most 0. | — |
| Is Overdue | True when all of the following hold: the paid flag is not set and the days past due is greater than 0. | _True only after the GraceDays window has elapsed._ |
| Status | Determined by priority: “Paid” if the paid flag is set; “Overdue” if the overdue flag is set; in all other cases, “Open”. | — |
| **Session** | A logged training session. The atomic line item that rolls up into an invoice. | — |
| Name | A defined attribute. | — |
| Client | A defined attribute. | — |
| Client Name | The full name of the session's client. | — |
| Trainer | Taken from the linked client. | — |
| Client Hourly Rate | The trainer hourly rate of the session's client. | — |
| Session Date | A defined attribute. | — |
| Duration Hours | A defined attribute. | — |
| Rate Override | A defined attribute. | _If > 0, overrides the trainer's HourlyRate for this session._ |
| Notes | A defined attribute. | — |
| Effective Rate | Determined by priority: the rate override if the rate override is greater than 0; in all other cases, the client hourly rate. | — |
| Line Total | Computed as the effective rate times the duration hours. | — |
| Invoice | A defined attribute. | — |
| Invoice Name | Taken from the linked invoice. | — |
| Is Billed | True when the invoice has a value. | — |

## 2 Fact Types

- a **client** may reference one **trainer**
- an **invoice** may reference one **client**
- a **session** may reference one **client**
- a **session** may reference one **invoice**

## 3 Operative Rules

_No operative rules yet. Required fields and foreign keys imply structural
`must`-rules automatically; to declare semantic obligations (`must` / `must not` / `should`), add a **Constraints** table whose rows point at
boolean calculated fields. See the tool README for the column contract._

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Total Sessions** | A trainer's total sessions is the number of sessions related to the trainer. |
| **DR-2 Total Billed** | A trainer's total billed is the total total across the invoices related to the trainer. |
| **DR-3 Total Outstanding** | A trainer's total outstanding is the total balance across the invoices related to the trainer. |
| **DR-4 Trainer Name** | A client's trainer name is the full name of the client's trainer. |
| **DR-5 Trainer Email** | A client's trainer email — taken from the linked trainer. |
| **DR-6 Trainer Hourly Rate** | A client's trainer hourly rate — taken from the linked trainer. |
| **DR-7 Session Count** | A client's session count is the number of sessions related to the client. |
| **DR-8 Total Invoiced** | A client's total invoiced is the total total across the invoices related to the client. |
| **DR-9 Outstanding Balance** | A client's outstanding balance is the total balance across the invoices related to the client. |
| **DR-10 Overdue Count** | A client's overdue count is the number of the client's invoices that are overdues. |
| **DR-11 Status** | The client's status is determined by the following priority:<br>1. “Overdue”, if the overdue count is greater than 0;<br>2. “Has Balance”, if the outstanding balance is greater than 0;<br>3. in all other cases, “Paid Up”. |
| **DR-12 Client Name** | An invoice's client name is the full name of the invoice's client. |
| **DR-13 Client Email** | An invoice's client email — taken from the linked client. |
| **DR-14 Trainer** | An invoice's trainer — taken from the linked client. |
| **DR-15 Trainer Name** | An invoice's trainer name — taken from the linked client. |
| **DR-16 Subtotal** | An invoice's subtotal is the total line total across the sessions related to the invoice. |
| **DR-17 Tax Amount** | An invoice's tax amount is computed as the subtotal times the tax rate. |
| **DR-18 Grace Days** | An invoice's grace days is computed as 45. |
| **DR-19 Days Since Due Date** | An invoice's days since due date is computed as the number of days from the due date to the current date and time. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-20 Days Past Due** | An invoice's days past due is computed as the largest of 0 and the days since due date minus the grace days. |
| **DR-21 Late Fee** | The invoice's late fee is determined by the following priority:<br>1. 15, if the days past due is greater than 0;<br>2. in all other cases, 0. |
| **DR-22 Total** | An invoice's total is computed as the subtotal plus the tax amount plus the late fee minus the discount amount. |
| **DR-23 Balance** | An invoice's balance is computed as the total minus the paid amount. |
| **DR-24 Is Paid** | An invoice is considered a paid if the balance is at most 0. |
| **DR-25 Is Overdue** | An invoice is considered an overdue if all of the following hold: the paid flag is not set and the days past due is greater than 0. |
| **DR-26 Status** | The invoice's status is determined by the following priority:<br>1. “Paid”, if the paid flag is set;<br>2. “Overdue”, if the overdue flag is set;<br>3. in all other cases, “Open”. |
| **DR-27 Client Name** | A session's client name is the full name of the session's client. |
| **DR-28 Trainer** | A session's trainer — taken from the linked client. |
| **DR-29 Client Hourly Rate** | A session's client hourly rate is the trainer hourly rate of the session's client. |
| **DR-30 Effective Rate** | The session's effective rate is determined by the following priority:<br>1. the rate override, if the rate override is greater than 0;<br>2. in all other cases, the client hourly rate. |
| **DR-31 Line Total** | A session's line total is computed as the effective rate times the duration hours. |
| **DR-32 Invoice Name** | A session's invoice name — taken from the linked invoice. |
| **DR-33 Is Billed** | A session is considered billed if the invoice has a value. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Trainers.TotalSessions** | rollup | `Count(Sessions via Trainer)` |
| **Trainers.TotalBilled** | rollup | `Sum(Invoices.Total via Trainer)` |
| **Trainers.TotalOutstanding** | rollup | `Sum(Invoices.Balance via Trainer)` |
| **Clients.TrainerName** | lookup | `Lookup(Trainers.FullName via Trainer)` |
| **Clients.TrainerEmail** | lookup | `Lookup(Trainers.Email via Trainer)` |
| **Clients.TrainerHourlyRate** | lookup | `Lookup(Trainers.HourlyRate via Trainer)` |
| **Clients.SessionCount** | rollup | `Count(Sessions via Client)` |
| **Clients.TotalInvoiced** | rollup | `Sum(Invoices.Total via Client)` |
| **Clients.OutstandingBalance** | rollup | `Sum(Invoices.Balance via Client)` |
| **Clients.OverdueCount** | rollup | `Count(Invoices via Client)` |
| **Clients.Status** | formula | `If(OverdueCount > 0, "Overdue", If(OutstandingBalance > 0, "Has Balance", "Paid Up"))` |
| **Invoices.ClientName** | lookup | `Lookup(Clients.FullName via Client)` |
| **Invoices.ClientEmail** | lookup | `Lookup(Clients.Email via Client)` |
| **Invoices.Trainer** | lookup | `Lookup(Clients.Trainer via Client)` |
| **Invoices.TrainerName** | lookup | `Lookup(Clients.TrainerName via Client)` |
| **Invoices.Subtotal** | rollup | `Sum(Sessions.LineTotal via Invoice)` |
| **Invoices.TaxAmount** | formula | `Subtotal * TaxRate` |
| **Invoices.GraceDays** | formula | `45` |
| **Invoices.DaysSinceDueDate** | formula | `DaysBetween(Now(), DueDate)` |
| **Invoices.DaysPastDue** | formula | `Max(0, DaysSinceDueDate - GraceDays)` |
| **Invoices.LateFee** | formula | `If(DaysPastDue > 0, 15, 0)` |
| **Invoices.Total** | formula | `Subtotal + TaxAmount + LateFee - DiscountAmount` |
| **Invoices.Balance** | formula | `Total - PaidAmount` |
| **Invoices.IsPaid** | formula | `Balance <= 0` |
| **Invoices.IsOverdue** | formula | `And(Not(IsPaid), DaysPastDue > 0)` |
| **Invoices.Status** | formula | `If(IsPaid, "Paid", If(IsOverdue, "Overdue", "Open"))` |
| **Sessions.ClientName** | lookup | `Lookup(Clients.FullName via Client)` |
| **Sessions.Trainer** | lookup | `Lookup(Clients.Trainer via Client)` |
| **Sessions.ClientHourlyRate** | lookup | `Lookup(Clients.TrainerHourlyRate via Client)` |
| **Sessions.EffectiveRate** | formula | `If(RateOverride > 0, RateOverride, ClientHourlyRate)` |
| **Sessions.LineTotal** | formula | `EffectiveRate * DurationHours` |
| **Sessions.InvoiceName** | lookup | `Lookup(Invoices.Name via Invoice)` |
| **Sessions.IsBilled** | formula | `Not(Isblank(Invoice))` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
