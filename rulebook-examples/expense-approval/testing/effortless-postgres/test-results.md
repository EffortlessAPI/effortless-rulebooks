# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 22 |
| Passed | 6 |
| Failed | 16 |
| Score | 27.3% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 6 | 13 | 46.2% |
| Lookup (INDEX/MATCH) | 0 | 6 | 0.0% |
| Aggregation (COUNTIFS/SUMIFS) | 0 | 3 | 0.0% |

## Results by Entity

### expense_reports

- Fields: 6/18 (33.3%)
- Computed columns: employee_name, employee_budget_limit, total_amount, is_over_budget, is_approved, requires_escalation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-nyc-client-visit | employee_name | Alice Anderson | None |
| alice-nyc-client-visit | employee_budget_limit | 500 | None |
| alice-nyc-client-visit | total_amount | 695 | None |
| alice-nyc-client-visit | is_over_budget | True | None |
| alice-nyc-client-visit | requires_escalation | True | None |
| alice-office-supplies | employee_name | Alice Anderson | None |
| alice-office-supplies | employee_budget_limit | 500 | None |
| alice-office-supplies | total_amount | 142 | None |
| alice-office-supplies | is_approved | True | None |
| dave-team-lunch | employee_name | Dave Davies | None |
| dave-team-lunch | employee_budget_limit | 200 | None |
| dave-team-lunch | total_amount | 87 | None |

### employees

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-employee | name | Alice Anderson | None |
| bob-manager | name | Bob Burton | None |
| carol-finance | name | Carol Chen | None |
| dave-employee | name | Dave Davies | None |
