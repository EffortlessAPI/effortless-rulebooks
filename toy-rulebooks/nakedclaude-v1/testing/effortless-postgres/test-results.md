# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 5 |
| Passed | 4 |
| Failed | 1 |
| Score | 80.0% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 4 | 5 | 80.0% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | — | 0 | n/a |

## Results by Entity

### customers

- Fields: 4/5 (80.0%)
- Computed columns: is_approved

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bob | is_approved | True | None |
