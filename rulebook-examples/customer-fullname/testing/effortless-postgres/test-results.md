# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 6 |
| Passed | 0 |
| Failed | 6 |
| Score | 0.0% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 0 | 6 | 0.0% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | — | 0 | n/a |

## Results by Entity

### customers

- Fields: 0/6 (0.0%)
- Computed columns: name, full_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| emily-jones-email-com | name | emily.jones-email.com | None |
| emily-jones-email-com | full_name | Mary Jones | None |
| jane-smith-email-com | name | jane.smith-email.com | None |
| jane-smith-email-com | full_name | Jane Smith | None |
| john-doe-email-com | name | john.doe-email.com | None |
| john-doe-email-com | full_name | Jimmy Doe | None |
