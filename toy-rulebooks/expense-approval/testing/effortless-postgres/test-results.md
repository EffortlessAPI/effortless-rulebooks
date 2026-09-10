# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 22 |
| Passed | 22 |
| Failed | 0 |
| Score | 100.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 13 | 13 | 100.0% |
| Lookup (INDEX/MATCH) | 6 | 6 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 3 | 3 | 100.0% |

## Results by Entity

### employees

- Fields: 4/4 (100.0%)
- Computed columns: name

### expense_reports

- Fields: 18/18 (100.0%)
- Computed columns: employee_name, employee_budget_limit, total_amount, is_over_budget, is_approved, requires_escalation
