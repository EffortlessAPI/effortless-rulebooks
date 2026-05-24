# Test Results: effortless-xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 6 |
| Passed | 3 |
| Failed | 3 |
| Score | 50.0% |
| Duration | 33s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 3 | 6 | 50.0% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | — | 0 | n/a |

## Results by Entity

### customers

- Fields: 3/6 (50.0%)
- Computed columns: name, full_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| emily-jones-email-com | full_name | Jones, Mary | Mary Jones |
| jane-smith-email-com | full_name | Smith, Bobby | Bobby Smith |
| john-doe-email-com | full_name | Doe, Jimmy | Jimmy Doe |
