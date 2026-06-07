# 📘 Gym Trainer Invoicing — RuleSpeak

_Sessions roll up into invoices; invoices roll up into client outstanding balances._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **User** | Login identities for the demo (stub auth). |
| **Trainer** | Personal trainers who log sessions and bill clients. |
| Total Sessions | The number of sessions related to the trainer. |
| Total Billed | The total total across the trainer's invoices. |
| Total Outstanding | The total balance across the trainer's invoices. |
| **Client** | Trainer's clients. |
| Trainer Name | The full name of the client's trainer. |
| Trainer Email | The email of the client's trainer. |
| Trainer Hourly Rate | The hourly rate of the client's trainer. |
| Session Count | The number of sessions related to the client. |
| Total Invoiced | The total total across the client's invoices. |
| Outstanding Balance | The total balance across the client's invoices. |
| Overdue Count | The number of the client's invoices that are overdues. |
| Status | Computed: `If(OverdueCount>0, "Overdue", If(OutstandingBalance>0, "Has Balance", "Paid Up"))`. |
| **Invoice** | An invoice bundles one or more sessions billed to a client. |
| Client Name | The full name of the invoice's client. |
| Client Email | The email of the invoice's client. |
| Trainer | The trainer of the invoice's client. |
| Trainer Name | The trainer name of the invoice's client. |
| Subtotal | The total line total across the invoice's sessions. |
| Tax Amount | Computed as `Subtotal * TaxRate`. |
| Grace Days | Grace period (in days) after the DueDate before an invoice is considered late. |
| Days Since Due Date | Raw whole days from DueDate to now (can be negative if not yet due). |
| Days Past Due | Whole days past DueDate after the GraceDays window; 0 inside the grace window. |
| Late Fee | $15 flat fee once an invoice is past the 45-day grace period. |
| Total | Computed as `Subtotal + TaxAmount + LateFee - DiscountAmount`. |
| Balance | Computed as `Total - PaidAmount`. |
| Is Paid | True when the balance is at most 0. |
| Is Overdue | True only after the GraceDays window has elapsed. |
| Status | Computed: `If(IsPaid, "Paid", If(IsOverdue, "Overdue", "Open"))`. |
| **Session** | A logged training session. The atomic line item that rolls up into an invoice. |
| Client Name | The full name of the session's client. |
| Trainer | The trainer of the session's client. |
| Client Hourly Rate | The trainer hourly rate of the session's client. |
| Effective Rate | Computed: `If(RateOverride>0, RateOverride, ClientHourlyRate)`. |
| Line Total | Computed as `EffectiveRate * DurationHours`. |
| Invoice Name | The name of the session's invoice. |
| Is Billed | True when it is not the case that `Isblank(Invoice)`. |

## 2 Fact Types

- a **client** may reference one **trainer**
- an **invoice** may reference one **client**
- a **session** may reference one **client**
- a **session** may reference one **invoice**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Total Sessions** | A trainer's total sessions is the number of sessions related to the trainer. |
| **DR-2 Total Billed** | A trainer's total billed is the total total across the invoices related to the trainer. |
| **DR-3 Total Outstanding** | A trainer's total outstanding is the total balance across the invoices related to the trainer. |
| **DR-4 Trainer Name** | A client's trainer name is the full name of the client's trainer. |
| **DR-5 Trainer Email** | A client's trainer email is the email of the client's trainer. |
| **DR-6 Trainer Hourly Rate** | A client's trainer hourly rate is the hourly rate of the client's trainer. |
| **DR-7 Session Count** | A client's session count is the number of sessions related to the client. |
| **DR-8 Total Invoiced** | A client's total invoiced is the total total across the invoices related to the client. |
| **DR-9 Outstanding Balance** | A client's outstanding balance is the total balance across the invoices related to the client. |
| **DR-10 Overdue Count** | A client's overdue count is the number of the client's invoices that are overdues. |
| **DR-11 Status** | The client's status is determined by the following priority:<br>1. the literal “Overdue”, if the overdue count is greater than 0;<br>2. the literal “Has Balance”, if the outstanding balance is greater than 0;<br>3. otherwise the literal “Paid Up”. |
| **DR-12 Client Name** | An invoice's client name is the full name of the invoice's client. |
| **DR-13 Client Email** | An invoice's client email is the email of the invoice's client. |
| **DR-14 Trainer** | An invoice's trainer is the trainer of the invoice's client. |
| **DR-15 Trainer Name** | An invoice's trainer name is the trainer name of the invoice's client. |
| **DR-16 Subtotal** | An invoice's subtotal is the total line total across the sessions related to the invoice. |
| **DR-17 Tax Amount** | An invoice's tax amount is computed as `Subtotal * TaxRate`. |
| **DR-18 Grace Days** | An invoice's grace days is computed as 45. |
| **DR-19 Days Since Due Date** | An invoice's days since due date is computed as `Datetime_diff(Now(), DueDate, "day")`. |
| **DR-20 Days Past Due** | An invoice's days past due is computed as `Max(0, DaysSinceDueDate - GraceDays)`. |
| **DR-21 Late Fee** | The invoice's late fee is determined by the following priority:<br>1. 15, if the days past due is greater than 0;<br>2. otherwise 0. |
| **DR-22 Total** | An invoice's total is computed as `Subtotal + TaxAmount + LateFee - DiscountAmount`. |
| **DR-23 Balance** | An invoice's balance is computed as `Total - PaidAmount`. |
| **DR-24 Is Paid** | An invoice is considered a paid if the balance is at most 0. |
| **DR-25 Is Overdue** | An invoice is considered an overdue if all of the following hold: it is not the case that the is paid flag is set and the days past due is greater than 0. |
| **DR-26 Status** | The invoice's status is determined by the following priority:<br>1. the literal “Paid”, if the is paid flag is set;<br>2. the literal “Overdue”, if the is overdue flag is set;<br>3. otherwise the literal “Open”. |
| **DR-27 Client Name** | A session's client name is the full name of the session's client. |
| **DR-28 Trainer** | A session's trainer is the trainer of the session's client. |
| **DR-29 Client Hourly Rate** | A session's client hourly rate is the trainer hourly rate of the session's client. |
| **DR-30 Effective Rate** | The session's effective rate is determined by the following priority:<br>1. the rate override, if the rate override is greater than 0;<br>2. otherwise the client hourly rate. |
| **DR-31 Line Total** | A session's line total is computed as `EffectiveRate * DurationHours`. |
| **DR-32 Invoice Name** | A session's invoice name is the name of the session's invoice. |
| **DR-33 Is Billed** | A session is considered billed if it is not the case that `Isblank(Invoice)`. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
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
| **Clients.OverdueCount** | rollup | `Count(Invoices.Client, Clients.ClientId, Invoices.IsOverdue, True())` |
| **Clients.Status** | formula | `If(OverdueCount>0, "Overdue", If(OutstandingBalance>0, "Has Balance", "Paid Up"))` |
| **Invoices.ClientName** | lookup | `Lookup(Clients.FullName via Client)` |
| **Invoices.ClientEmail** | lookup | `Lookup(Clients.Email via Client)` |
| **Invoices.Trainer** | lookup | `Lookup(Clients.Trainer via Client)` |
| **Invoices.TrainerName** | lookup | `Lookup(Clients.TrainerName via Client)` |
| **Invoices.Subtotal** | rollup | `Sum(Sessions.LineTotal via Invoice)` |
| **Invoices.TaxAmount** | formula | `Subtotal * TaxRate` |
| **Invoices.GraceDays** | formula | `45` |
| **Invoices.DaysSinceDueDate** | formula | `Datetime_diff(Now(), DueDate, "day")` |
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
| **Sessions.EffectiveRate** | formula | `If(RateOverride>0, RateOverride, ClientHourlyRate)` |
| **Sessions.LineTotal** | formula | `EffectiveRate * DurationHours` |
| **Sessions.InvoiceName** | lookup | `Lookup(Invoices.Name via Invoice)` |
| **Sessions.IsBilled** | formula | `Not(Isblank(Invoice))` |
