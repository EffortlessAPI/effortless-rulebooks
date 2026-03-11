# Test Results: golang

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 118 |
| Passed | 102 |
| Failed | 16 |
| Score | 86.4% |
| Duration | < 1s |

## Results by Entity

### roles

- Fields: 15/15 (100.0%)
- Computed columns: name

### workflow_steps

- Fields: 16/16 (100.0%)
- Computed columns: name

### workflows

- Fields: 29/45 (64.4%)
- Computed columns: name, count_of_steps, has_more_than1_step

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| asset-management | count_of_steps | 2 | None |
| asset-management | has_more_than1_step | True | False |
| change-management | count_of_steps | 1 | None |
| content-publishing | count_of_steps | 1 | None |
| contract-review | count_of_steps | 1 | None |
| customer-feedback | count_of_steps | 1 | None |
| expense-reimbursement | count_of_steps | 1 | None |
| incident-reporting | count_of_steps | 1 | None |
| invoice-approval | count_of_steps | 1 | None |
| it-support | count_of_steps | 1 | None |
| leave-request | count_of_steps | 1 | None |
| onboarding | count_of_steps | 1 | None |
| performance-review | count_of_steps | 1 | None |
| procurement | count_of_steps | 1 | None |
| project-kickoff | count_of_steps | 1 | None |
| recruitment | count_of_steps | 1 | None |

### precedes_steps

- Fields: 16/16 (100.0%)
- Computed columns: display_name

### departments

- Fields: 15/15 (100.0%)
- Computed columns: name

### approval_gates

- Fields: 11/11 (100.0%)
- Computed columns: name
