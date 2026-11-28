# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 88 |
| Passed | 88 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 30 | 30 | 100.0% |
| Lookup (INDEX/MATCH) | 36 | 36 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 22 | 22 | 100.0% |

## Results by Entity

### events

- Fields: 10/10 (100.0%)
- Computed columns: shift_count, total_slots_needed, total_slots_filled, coverage_percent, staffing_grade

### volunteers

- Fields: 20/20 (100.0%)
- Computed columns: name, assignment_count, assigned_hours, load_status

### shifts

- Fields: 18/18 (100.0%)
- Computed columns: event_name, filled_count, coverage_status

### assignments

- Fields: 40/40 (100.0%)
- Computed columns: name, volunteer_name, shift_name, shift_duration_hours
