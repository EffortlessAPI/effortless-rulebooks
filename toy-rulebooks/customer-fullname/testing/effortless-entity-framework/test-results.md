# Test Results: effortless-entity-framework

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 3 |
| Passed | 0 |
| Failed | 3 |
| Score | 0.0% |
| Duration | 28s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 0 | 3 | 0.0% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | — | 0 | n/a |

## Results by Entity

### customers

- Fields: 0/3 (0.0%)
- Computed columns: full_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cust0001 | full_name | Smith, Jane | None |
| cust0002 | full_name | Doe, John | None |
| cust0003 | full_name | Jones, Emily | None |
