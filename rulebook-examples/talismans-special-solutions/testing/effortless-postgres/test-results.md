# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 21 |
| Passed | 21 |
| Failed | 0 |
| Score | 100.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 20 | 20 | 100.0% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | 1 | 1 | 100.0% |

## Results by Entity

### workflows

- Fields: 3/3 (100.0%)
- Computed columns: name, count_of_non_proposed_steps, has_more_than1_step

### workflow_steps

- Fields: 5/5 (100.0%)
- Computed columns: name

### approval_gates

- Fields: 1/1 (100.0%)
- Computed columns: name

### step_precedence

- Fields: 4/4 (100.0%)
- Computed columns: name

### roles

- Fields: 6/6 (100.0%)
- Computed columns: name

### departments

- Fields: 2/2 (100.0%)
- Computed columns: name
