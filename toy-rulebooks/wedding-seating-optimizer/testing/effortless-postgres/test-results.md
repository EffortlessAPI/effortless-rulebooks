# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 523 |
| Passed | 523 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 257 | 257 | 100.0% |
| Lookup (INDEX/MATCH) | 149 | 149 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 117 | 117 | 100.0% |

## Results by Entity

### tables

- Fields: 50/50 (100.0%)
- Computed columns: head_count, open_seats, over_capacity, raw_happiness, violation_count, happiness, grade, bride_side_count, groom_side_count, side_skew

### guests

- Fields: 253/253 (100.0%)
- Computed columns: table_label, table_seats, table_head_count, relationships_as_a, relationships_as_b, satisfaction_a, satisfaction_b, satisfaction, mood, bride_flag, groom_flag

### relationships

- Fields: 220/220 (100.0%)
- Computed columns: guest_a_name, guest_b_name, guest_a_table, guest_b_table, same_table, seated_table, effective_score, is_must_not_violation, is_satisfied, violation_table, violation_flag
