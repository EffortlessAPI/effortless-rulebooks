# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 207 |
| Passed | 207 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 124 | 124 | 100.0% |
| Lookup (INDEX/MATCH) | 51 | 51 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 32 | 32 | 100.0% |

## Results by Entity

### truth_values

- Fields: 12/12 (100.0%)
- Computed columns: name, is_classical, is_undefined, count_of_facts

### connectives

- Fields: 6/6 (100.0%)
- Computed columns: name, count_of_truth_table_rows

### truth_table_rows

- Fields: 42/42 (100.0%)
- Computed columns: output_symbol, name

### set_rules

- Fields: 36/36 (100.0%)
- Computed columns: name, is_classical, is_the_missing_rule

### sets

- Fields: 32/32 (100.0%)
- Computed columns: name, is_russell_set, count_of_memberships, count_of_null_memberships

### membership_facts

- Fields: 70/70 (100.0%)
- Computed columns: name, container_label, membership_value_symbol, is_bivalent, is_null, is_grounded, count_of_evaluation_steps

### evaluation_steps

- Fields: 9/9 (100.0%)
- Computed columns: name, is_stable, outcome
