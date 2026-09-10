# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 116 |
| Passed | 26 |
| Failed | 90 |
| Score | 22.4% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 26 | 44 | 59.1% |
| Lookup (INDEX/MATCH) | 0 | 64 | 0.0% |
| Aggregation (COUNTIFS/SUMIFS) | 0 | 8 | 0.0% |

## Results by Entity

### capabilities

- Fields: 4/4 (100.0%)
- Computed columns: name

### intelligences

- Fields: 5/16 (31.2%)
- Computed columns: name, assessment_count, total_weighted_score, taxonomy_class

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| calculator | assessment_count | None | 4 |
| calculator | total_weighted_score | None | 183.5 |
| gpt-5 | assessment_count | None | 4 |
| gpt-5 | total_weighted_score | None | 326.5 |
| gpt-5 | taxonomy_class | Narrow | Broad |
| human | assessment_count | None | 4 |
| human | total_weighted_score | None | 395.5 |
| human | taxonomy_class | Narrow | Generalist |
| octopus | assessment_count | None | 4 |
| octopus | total_weighted_score | None | 339.5 |
| octopus | taxonomy_class | Narrow | Broad |

### assessments

- Fields: 17/96 (17.7%)
- Computed columns: name, intelligence_name, capability_name, capability_tier, capability_weight, weighted_score

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| calculator-creativity | intelligence_name | None | calculator |
| calculator-creativity | capability_name | None | creativity |
| calculator-creativity | capability_tier | None | emergent |
| calculator-creativity | capability_weight | None | 1.5 |
| calculator-memory | intelligence_name | None | calculator |
| calculator-memory | capability_name | None | memory |
| calculator-memory | capability_tier | None | foundational |
| calculator-memory | capability_weight | None | 1.0 |
| calculator-memory | weighted_score | 0 | 50.0 |
| calculator-perception | intelligence_name | None | calculator |
| calculator-perception | capability_name | None | perception |
| calculator-perception | capability_tier | None | foundational |
| calculator-perception | capability_weight | None | 1.0 |
| calculator-perception | weighted_score | 0 | 10.0 |
| calculator-reasoning | intelligence_name | None | calculator |
| calculator-reasoning | capability_name | None | reasoning |
| calculator-reasoning | capability_tier | None | composite |
| calculator-reasoning | capability_weight | None | 1.3 |
| calculator-reasoning | weighted_score | 0 | 123.5 |
| gpt-5-creativity | intelligence_name | None | gpt-5 |
| ... | ... | (59 more) | ... |
