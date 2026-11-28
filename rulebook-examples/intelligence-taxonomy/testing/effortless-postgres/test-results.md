# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 116 |
| Passed | 116 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 44 | 44 | 100.0% |
| Lookup (INDEX/MATCH) | 64 | 64 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 8 | 8 | 100.0% |

## Results by Entity

### capabilities

- Fields: 4/4 (100.0%)
- Computed columns: name

### intelligences

- Fields: 16/16 (100.0%)
- Computed columns: name, assessment_count, total_weighted_score, taxonomy_class

### assessments

- Fields: 96/96 (100.0%)
- Computed columns: name, intelligence_name, capability_name, capability_tier, capability_weight, weighted_score
