# 📘 Expense Approval — Docs

_Employees submit line-item reports; totals, over-budget, and escalation flags cascade automatically._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Employee** | An employee is identified by its name. | — |
| Name | Computed as the first name, followed by a space, followed by the last name. | _Display name (FirstName + LastName)._ |
| First Name | A defined attribute. | — |
| Last Name | A defined attribute. | — |
| Email Address | A defined attribute. | _Used as the click-to-login identity._ |
| Role | A defined attribute. | _employee \| manager \| finance_ |
| Budget Limit | A defined attribute. | _Per-report ceiling above which reports are flagged over-budget. Applies to submitters; reviewers (manager/finance) can be left null._ |

<details open>
<summary>Example data</summary>

| Name ƒ | First Name | Last Name | Email Address | Role | Budget Limit |
|---|---|---|---|---|---|
| Alice Anderson | Alice | Anderson | alice@example.com | employee | 500 |
| Dave Davies | Dave | Davies | dave@example.com | employee | 200 |
| Bob Burton | Bob | Burton | bob@example.com | manager | — |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Expense Report** | An expense report is identified by its name and is related to optionally an employee. | — |
| Name | A defined attribute. | _Short title of the report._ |
| Employee | A defined attribute. | _Submitter._ |
| Employee Name | Taken from the linked employee. | — |
| Employee Budget Limit | Taken from the linked employee. | — |
| Approval Status | A defined attribute. | _pending \| approved \| rejected_ |
| Total Amount | The total amount across the expense items related to the expense report. | _Sum of all line-item amounts on this report._ |
| Is Over Budget | True when all of the following hold: the employee budget limit is greater than 0 and the total amount is greater than the employee budget limit. | _True when TotalAmount exceeds the employee's BudgetLimit._ |
| Is Approved | True when the approval status is “approved”. | _True iff ApprovalStatus is 'approved'._ |
| Requires Escalation | True when all of the following hold: the is over budget is true and the is approved is false. | _Over-budget AND not yet approved — manager's escalation queue._ |

<details open>
<summary>Example data</summary>

| Name | Employee Name ƒ | Employee Budget Limit ƒ | Approval Status | Total Amount ƒ | Is Over Budget ƒ | Is Approved ƒ | Requires Escalation ƒ |
|---|---|---|---|---|---|---|---|
| NYC Client Visit | Alice Anderson | 500.0 | pending | 695.0 | true | false | true |
| Office Supplies Q1 | Alice Anderson | 500.0 | approved | 142.0 | false | true | false |
| Team Lunch | Dave Davies | 200.0 | pending | 87.0 | false | false | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Expense Item** | An expense item is identified by its name and is related to optionally an expense report. | — |
| Name | A defined attribute. | _Short description of the expense._ |
| Expense Report | A defined attribute. | _Parent report._ |
| Amount | A defined attribute. | _Dollar amount of this line item._ |

<details open>
<summary>Example data</summary>

| Name | Amount |
|---|---|
| Flight to NYC | 425 |
| Hotel (1 night) | 185 |
| Client Dinner | 85 |

</details>

## 2 Fact Types

- an **expense report** may reference one **employee**
- an **expense item** may reference one **expense report**

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
| **DR-1 Name** | An employee's name is computed as the first name, followed by a space, followed by the last name. |
| **DR-2 Employee Name** | An expense report's employee name — taken from the linked employee. |
| **DR-3 Employee Budget Limit** | An expense report's employee budget limit — taken from the linked employee. |
| **DR-4 Total Amount** | An expense report's total amount is the total amount across the expense items related to the expense report. |
| **DR-5 Is Over Budget** | An expense report is considered an over budget if all of the following hold: the employee budget limit is greater than 0 and the total amount is greater than the employee budget limit. |
| **DR-6 Is Approved** | An expense report is considered approved if the approval status is “approved”. |
| **DR-7 Requires Escalation** | An expense report is considered to require an escalation if all of the following hold: the is over budget is true and the is approved is false. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Employees.Name** | formula | `FirstName & " " & LastName` |
| **ExpenseReports.EmployeeName** | lookup | `Lookup(Employees.Name via Employee)` |
| **ExpenseReports.EmployeeBudgetLimit** | lookup | `Lookup(Employees.BudgetLimit via Employee)` |
| **ExpenseReports.TotalAmount** | rollup | `Sum(ExpenseItems.Amount via ExpenseReport)` |
| **ExpenseReports.IsOverBudget** | formula | `If(And(EmployeeBudgetLimit > 0, TotalAmount > EmployeeBudgetLimit), TRUE, FALSE)` |
| **ExpenseReports.IsApproved** | formula | `If(ApprovalStatus = "approved", TRUE, FALSE)` |
| **ExpenseReports.RequiresEscalation** | formula | `If(And(IsOverBudget = TRUE, IsApproved = FALSE), TRUE, FALSE)` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
