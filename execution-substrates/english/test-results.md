# Test Results: english

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 103 |
| Passed | 87 |
| Failed | 16 |
| Score | 84.5% |
| Duration | 4m 52s |

## Results by Entity

### roles

- Fields: 15/15 (100.0%)
- Computed columns: name

### workflow_steps

- Fields: 16/16 (100.0%)
- Computed columns: name

### workflows

- Fields: 30/30 (100.0%)
- Computed columns: name, has_more_than1_step

### precedes_steps

- Fields: 0/16 (0.0%)
- Computed columns: display_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assign-task-to-team | display_name | Step-16 | Assign Task to Team |
| step-1 | display_name | Step-1 | Step 1 |
| step-10 | display_name | Step-10 | Step 10 |
| step-11 | display_name | Step-11 | Step 11 |
| step-12 | display_name | Step-12 | Step 12 |
| step-13 | display_name | Step-13 | Step 13 |
| step-14 | display_name | Step-14 | Step 14 |
| step-15 | display_name | Step-15 | Step 15 |
| step-2 | display_name | Step-2 | Step 2 |
| step-3 | display_name | Step-3 | Step 3 |
| step-4 | display_name | Step-4 | finance-approval |
| step-5 | display_name | Step-5 | document-verification |
| step-6 | display_name | Step-6 | quality-assurance-review |
| step-7 | display_name | Step-7 | system-notification-sent |
| step-8 | display_name | Step-8 | final-approval |
| step-9 | display_name | Step-9 | archive-request |

### departments

- Fields: 15/15 (100.0%)
- Computed columns: name

### approval_gates

- Fields: 11/11 (100.0%)
- Computed columns: name
