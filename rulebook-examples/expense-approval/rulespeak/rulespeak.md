# 📘 Expense Approval — RuleSpeak

_Employees submit line-item reports; totals, over-budget, and escalation flags cascade automatically._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Employee** | An employee tracked by the business. |
| **Expense Report** | An expense report tracked by the business. |
| Employee Name | The name of the expense report's employee. |
| Employee Budget Limit | The budget limit of the expense report's employee. |
| Total Amount | Sum of all line-item amounts on this report. |
| Is Over Budget | True when TotalAmount exceeds the employee's BudgetLimit. |
| Is Approved | True iff ApprovalStatus is 'approved'. |
| Requires Escalation | Over-budget AND not yet approved — manager's escalation queue. |
| **Expense Item** | An expense item tracked by the business. |

## 2 Fact Types

- an **expense report** may reference one **employee**
- an **expense item** may reference one **expense report**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Employee Name** | An expense report's employee name is the name of the expense report's employee. |
| **DR-2 Employee Budget Limit** | An expense report's employee budget limit is the budget limit of the expense report's employee. |
| **DR-3 Total Amount** | An expense report's total amount is the total amount across the expense items related to the expense report. |
| **DR-4 Is Over Budget** | An expense report is considered an over budget if `If(And(EmployeeBudgetLimit` is greater than `0, TotalAmount>EmployeeBudgetLimit), TRUE, FALSE)`. |
| **DR-5 Is Approved** | An expense report is considered approved if `If(ApprovalStatus` is `"approved", TRUE, FALSE)`. |
| **DR-6 Requires Escalation** | An expense report is considered to require escalation if `If(And(IsOverBudget` is `TRUE, IsApproved=FALSE), TRUE, FALSE)`. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **ExpenseReports.EmployeeName** | lookup | `Lookup(Employees.Name via Employee)` |
| **ExpenseReports.EmployeeBudgetLimit** | lookup | `Lookup(Employees.BudgetLimit via Employee)` |
| **ExpenseReports.TotalAmount** | rollup | `Sum(ExpenseItems.Amount via ExpenseReport)` |
| **ExpenseReports.IsOverBudget** | formula | `If(And(EmployeeBudgetLimit>0, TotalAmount>EmployeeBudgetLimit), TRUE, FALSE)` |
| **ExpenseReports.IsApproved** | formula | `If(ApprovalStatus="approved", TRUE, FALSE)` |
| **ExpenseReports.RequiresEscalation** | formula | `If(And(IsOverBudget=TRUE, IsApproved=FALSE), TRUE, FALSE)` |
