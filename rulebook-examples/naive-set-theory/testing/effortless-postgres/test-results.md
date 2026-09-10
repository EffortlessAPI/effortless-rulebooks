# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 207 |
| Passed | 149 |
| Failed | 58 |
| Score | 72.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 121 | 124 | 97.6% |
| Lookup (INDEX/MATCH) | 10 | 51 | 19.6% |
| Aggregation (COUNTIFS/SUMIFS) | 18 | 32 | 56.2% |

## Results by Entity

### truth_values

- Fields: 9/12 (75.0%)
- Computed columns: name, is_classical, is_undefined, count_of_facts

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| false | count_of_facts | None | 2 |
| null | count_of_facts | None | 1 |
| true | count_of_facts | None | 7 |

### connectives

- Fields: 3/6 (50.0%)
- Computed columns: name, count_of_truth_table_rows

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| conjunction | count_of_truth_table_rows | None | 9 |
| disjunction | count_of_truth_table_rows | None | 9 |
| negation | count_of_truth_table_rows | None | 3 |

### truth_table_rows

- Fields: 25/42 (59.5%)
- Computed columns: output_symbol, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| conjunction|null|null|null | output_symbol | None | N |
| conjunction|null|true|null | output_symbol | None | N |
| conjunction|true|null|null | output_symbol | None | N |
| conjunction|true|true|true | output_symbol | None | T |
| disjunction|false|null|null | output_symbol | None | N |
| disjunction|false|true|true | output_symbol | None | T |
| disjunction|null|false|null | output_symbol | None | N |
| disjunction|null|null|null | output_symbol | None | N |
| disjunction|null|true|true | output_symbol | None | T |
| disjunction|true|false|true | output_symbol | None | T |
| disjunction|true|null|true | output_symbol | None | T |
| disjunction|true|true|true | output_symbol | None | T |
| negation|false|_|true | output_symbol | None | T |
| negation|false|_|true | name | negation: false ,  -> true | negation: false -> true |
| negation|null|_|null | output_symbol | None | N |
| negation|null|_|null | name | negation: null ,  -> null | negation: null -> null |
| negation|true|_|false | name | negation: true ,  -> false | negation: true -> false |

### set_rules

- Fields: 36/36 (100.0%)
- Computed columns: name, is_classical, is_the_missing_rule

### sets

- Fields: 25/32 (78.1%)
- Computed columns: name, is_russell_set, count_of_memberships, count_of_null_memberships

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| empty-set | count_of_memberships | None | 1 |
| nested | count_of_memberships | None | 1 |
| pair-ab | count_of_memberships | None | 2 |
| russell-set | count_of_memberships | None | 4 |
| russell-set | count_of_null_memberships | None | 1 |
| singleton-a | count_of_memberships | None | 1 |
| universal-u | count_of_memberships | None | 1 |

### membership_facts

- Fields: 42/70 (60.0%)
- Computed columns: name, container_label, membership_value_symbol, is_bivalent, is_null, is_grounded, count_of_evaluation_steps

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| a-in-empty-set | container_label | None | ∅ |
| a-in-empty-set | is_bivalent | None | True |
| a-in-pair-ab | container_label | None | {a, b} |
| a-in-pair-ab | membership_value_symbol | None | T |
| a-in-pair-ab | is_bivalent | None | True |
| a-in-russell-set | container_label | None | R |
| a-in-russell-set | membership_value_symbol | None | T |
| a-in-russell-set | is_bivalent | None | True |
| a-in-singleton-a | container_label | None | {a} |
| a-in-singleton-a | membership_value_symbol | None | T |
| a-in-singleton-a | is_bivalent | None | True |
| b-in-pair-ab | container_label | None | {a, b} |
| b-in-pair-ab | membership_value_symbol | None | T |
| b-in-pair-ab | is_bivalent | None | True |
| empty-set-in-russell-set | container_label | None | R |
| empty-set-in-russell-set | membership_value_symbol | None | T |
| empty-set-in-russell-set | is_bivalent | None | True |
| russell-set-in-russell-set | container_label | None | R |
| russell-set-in-russell-set | membership_value_symbol | None | N |
| russell-set-in-russell-set | count_of_evaluation_steps | None | 3 |
| ... | ... | (8 more) | ... |

### evaluation_steps

- Fields: 9/9 (100.0%)
- Computed columns: name, is_stable, outcome
