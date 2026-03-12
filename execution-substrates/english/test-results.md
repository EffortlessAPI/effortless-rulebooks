# Test Results: english

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 122 |
| Passed | 106 |
| Failed | 16 |
| Score | 86.9% |
| Duration | 5m 33s |

## Results by Entity

### roles

- Fields: 15/15 (100.0%)
- Computed columns: name

### workflow_steps

- Fields: 20/20 (100.0%)
- Computed columns: name

### workflows

- Fields: 45/45 (100.0%)
- Computed columns: name, count_of_non_proposed_steps, has_more_than1_step

### precedes_steps

- Fields: 0/16 (0.0%)
- Computed columns: display_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assign-task-to-team | display_name | Step-16 | assign-task-to-team |
| step-1 | display_name | Step-1 | submit-request |
| step-10 | display_name | Step-10 | customer-feedback-collection |
| step-11 | display_name | Step-11 | legal-compliance-check |
| step-12 | display_name | Step-12 | assign-task-to-team |
| step-13 | display_name | Step-13 | team-lead-review |
| step-14 | display_name | Step-14 | generate-report |
| step-15 | display_name | Step-15 | close-workflow |
| step-2 | display_name | Step-2 | manager-review |
| step-3 | display_name | Step-3 | automated-eligibility-check |
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
