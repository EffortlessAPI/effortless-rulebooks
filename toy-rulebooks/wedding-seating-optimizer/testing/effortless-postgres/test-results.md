# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 523 |
| Passed | 214 |
| Failed | 309 |
| Score | 40.9% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 170 | 257 | 66.1% |
| Lookup (INDEX/MATCH) | 0 | 149 | 0.0% |
| Aggregation (COUNTIFS/SUMIFS) | 44 | 117 | 37.6% |

## Results by Entity

### tables

- Fields: 13/50 (26.0%)
- Computed columns: head_count, open_seats, over_capacity, raw_happiness, violation_count, happiness, grade, bride_side_count, groom_side_count, side_skew

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| t-family-b | head_count | None | 4 |
| t-family-b | open_seats | 8 | 4 |
| t-family-b | raw_happiness | None | 23 |
| t-family-b | happiness | 0 | 19 |
| t-family-b | grade | Cold | Great |
| t-family-b | bride_side_count | None | 4 |
| t-family-b | side_skew | 0 | 4 |
| t-family-g | head_count | None | 4 |
| t-family-g | open_seats | 8 | 4 |
| t-family-g | raw_happiness | None | 30 |
| t-family-g | happiness | 0 | 26 |
| t-family-g | grade | Cold | Great |
| t-family-g | groom_side_count | None | 4 |
| t-family-g | side_skew | 0 | 4 |
| t-friends | head_count | None | 4 |
| t-friends | open_seats | 6 | 2 |
| t-friends | raw_happiness | None | 21 |
| t-friends | happiness | 0 | 21 |
| t-friends | grade | Cold | Great |
| t-friends | bride_side_count | None | 2 |
| ... | ... | (17 more) | ... |

### guests

- Fields: 91/253 (36.0%)
- Computed columns: table_label, table_seats, table_head_count, relationships_as_a, relationships_as_b, satisfaction_a, satisfaction_b, satisfaction, mood, bride_flag, groom_flag

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| g-alex | table_label | None | Head Table |
| g-alex | table_seats | None | 6 |
| g-alex | table_head_count | None | 6 |
| g-alex | relationships_as_a | None | 1 |
| g-alex | relationships_as_b | None | 3 |
| g-alex | satisfaction_a | None | 30 |
| g-alex | satisfaction_b | None | 20 |
| g-alex | satisfaction | 0 | 50 |
| g-alex | mood | Neutral | Happy |
| g-aunt-b | table_label | None | Bride Family |
| g-aunt-b | table_seats | None | 8 |
| g-aunt-b | table_head_count | None | 4 |
| g-aunt-b | relationships_as_a | None | 2 |
| g-aunt-b | satisfaction_a | None | 15 |
| g-aunt-b | satisfaction | 0 | 15 |
| g-aunt-b | mood | Neutral | Happy |
| g-cuz-b1 | table_label | None | Bride Family |
| g-cuz-b1 | table_seats | None | 8 |
| g-cuz-b1 | table_head_count | None | 4 |
| g-cuz-b1 | relationships_as_a | None | 1 |
| ... | ... | (142 more) | ... |

### relationships

- Fields: 110/220 (50.0%)
- Computed columns: guest_a_name, guest_b_name, guest_a_table, guest_b_table, same_table, seated_table, effective_score, is_must_not_violation, is_satisfied, violation_table, violation_flag

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| r-aunt-ex | guest_a_name | None | Carol Stone |
| r-aunt-ex | guest_b_name | None | Drew Hart (the ex) |
| r-aunt-ex | guest_a_table | None | t-family-b |
| r-aunt-ex | guest_b_table | None | t-misc |
| r-aunt-ex | same_table | True | False |
| r-aunt-ex | effective_score | -15 | 0 |
| r-aunt-ex | is_satisfied | False | True |
| r-aunt-uncle | guest_a_name | None | Carol Stone |
| r-aunt-uncle | guest_b_name | None | Pete Stone |
| r-aunt-uncle | guest_a_table | None | t-family-b |
| r-aunt-uncle | guest_b_table | None | t-family-b |
| r-aunt-uncle | seated_table | None | t-family-b |
| r-couple | guest_a_name | None | Alex Stone |
| r-couple | guest_b_name | None | Sam Rivera |
| r-couple | guest_a_table | None | t-head |
| r-couple | guest_b_table | None | t-head |
| r-couple | seated_table | None | t-head |
| r-cousins-b | guest_a_name | None | Jamie Stone |
| r-cousins-b | guest_b_name | None | Riley Stone |
| r-cousins-b | guest_a_table | None | t-family-b |
| ... | ... | (90 more) | ... |
