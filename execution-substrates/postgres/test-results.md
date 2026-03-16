# Test Results: postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 13 |
| Passed | 5 |
| Failed | 8 |
| Score | 38.5% |
| Duration | 2s |

## Results by Entity

### roles

- Fields: 3/3 (100.0%)
- Computed columns: count_of_workflow_steps

### workflows

- Fields: 1/1 (100.0%)
- Computed columns: count_of_workflow_steps

### human_agents

- Fields: 1/9 (11.1%)
- Computed columns: count_of_roles

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| james-okafor | count_of_roles | 0 | None |
| james-okafor | count_of_roles | 0 | None |
| james-okafor | count_of_roles | 0 | None |
| james-okafor | count_of_roles | 0 | None |
| maria-gonzalez | count_of_roles | 0 | 1 |
| maria-gonzalez | count_of_roles | 0 | 1 |
| maria-gonzalez | count_of_roles | 0 | 1 |
| maria-gonzalez | count_of_roles | 0 | 1 |
