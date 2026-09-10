# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 88 |
| Passed | 70 |
| Failed | 18 |
| Score | 79.5% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 22 | 30 | 73.3% |
| Lookup (INDEX/MATCH) | 36 | 36 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 12 | 22 | 54.5% |

## Results by Entity

### events

- Fields: 4/10 (40.0%)
- Computed columns: shift_count, total_slots_needed, total_slots_filled, coverage_percent, staffing_grade

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| food-bank-saturday | total_slots_filled | 6 | 2 |
| food-bank-saturday | coverage_percent | 1.0 | 0.33333333333333333333 |
| food-bank-saturday | staffing_grade | A | F |
| harvest-festival-2026 | total_slots_filled | 12 | 8 |
| harvest-festival-2026 | coverage_percent | 0.857 | 0.57142857142857142857 |
| harvest-festival-2026 | staffing_grade | B | D |

### volunteers

- Fields: 15/20 (75.0%)
- Computed columns: name, assignment_count, assigned_hours, load_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| jamie-rivera | assignment_count | 1 | 2 |
| jamie-rivera | assigned_hours | 3 | 6 |
| jamie-rivera | load_status | under | ok |
| morgan-lee | assignment_count | 2 | 3 |
| morgan-lee | assigned_hours | 7 | 10 |

### shifts

- Fields: 11/18 (61.1%)
- Computed columns: event_name, filled_count, coverage_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| foodbank-distrib-afternoon | filled_count | 3 | 1 |
| foodbank-distrib-afternoon | coverage_status | covered | understaffed |
| foodbank-sort-morning | filled_count | 3 | 1 |
| foodbank-sort-morning | coverage_status | covered | understaffed |
| harvest-food-afternoon | filled_count | 3 | 0 |
| harvest-teardown-evening | filled_count | 3 | 2 |
| harvest-teardown-evening | coverage_status | covered | understaffed |

### assignments

- Fields: 40/40 (100.0%)
- Computed columns: name, volunteer_name, shift_name, shift_duration_hours
