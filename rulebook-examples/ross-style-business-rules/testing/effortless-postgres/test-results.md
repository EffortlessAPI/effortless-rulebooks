# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 110 |
| Passed | 110 |
| Failed | 0 |
| Score | 100.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 60 | 60 | 100.0% |
| Lookup (INDEX/MATCH) | 42 | 42 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 8 | 8 | 100.0% |

## Results by Entity

### policies

- Fields: 6/6 (100.0%)
- Computed columns: name, count_of_covered_claimants

### claimants

- Fields: 20/20 (100.0%)
- Computed columns: name, count_of_incidents, is_high_risk, policy_is_active

### incidents

- Fields: 14/14 (100.0%)
- Computed columns: name, incident_claimant_name

### claims

- Fields: 70/70 (100.0%)
- Computed columns: name, references_incident, has_additional_claimant, incident_claimant, claimant_of_record, incident_claimant_policy_active, additional_claimant_policy_active, additional_claimant_favorite_color, claimant_of_record_incident_count, claimant_of_record_is_high_risk, is_valid, validity_deciding_factor, is_approvable, approvability_deciding_factor
