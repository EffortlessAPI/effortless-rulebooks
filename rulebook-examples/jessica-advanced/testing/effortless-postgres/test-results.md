# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 165 |
| Passed | 46 |
| Failed | 119 |
| Score | 27.9% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 30 | 118 | 25.4% |
| Lookup (INDEX/MATCH) | 16 | 32 | 50.0% |
| Aggregation (COUNTIFS/SUMIFS) | 0 | 15 | 0.0% |

## Results by Entity

### roles

- Fields: 0/15 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| administrator | name | administrator | None |
| analyst | name | analyst | None |
| contributor | name | contributor | None |
| developer | name | developer | None |
| editor | name | editor | None |
| finance | name | finance | None |
| guest | name | guest | None |
| hr | name | hr | None |
| manager | name | manager | None |
| moderator | name | moderator | None |
| owner | name | owner | None |
| support | name | support | None |
| tester | name | tester | None |
| trainer | name | trainer | None |
| viewer | name | viewer | None |

### workflow_steps

- Fields: 32/64 (50.0%)
- Computed columns: name, execution_actor_type, assigned_role_department, approval_gate_escalation_threshold_hours

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| archive-request | name | archive-request | None |
| archive-request | assigned_role_department | procurement | None |
| assign-task-to-team | name | assign-task-to-team | None |
| assign-task-to-team | assigned_role_department | public-relations | None |
| automated-eligibility-check | name | automated-eligibility-check | None |
| automated-eligibility-check | assigned_role_department | information-technology | None |
| close-workflow | name | close-workflow | None |
| close-workflow | assigned_role_department | training-and-development | None |
| customer-feedback-collection | name | customer-feedback-collection | None |
| customer-feedback-collection | assigned_role_department | operations | None |
| document-verification | name | document-verification | None |
| document-verification | assigned_role_department | sales | None |
| final-approval | name | final-approval | None |
| final-approval | assigned_role_department | legal | None |
| finance-approval | name | finance-approval | None |
| finance-approval | assigned_role_department | marketing | None |
| generate-report | name | generate-report | None |
| generate-report | assigned_role_department | facilities-management | None |
| legal-compliance-check | name | legal-compliance-check | None |
| legal-compliance-check | assigned_role_department | administration | None |
| ... | ... | (12 more) | ... |

### workflows

- Fields: 14/45 (31.1%)
- Computed columns: name, count_of_steps, has_more_than1_step

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| asset-management | name | asset-management | None |
| asset-management | count_of_steps | 2 | None |
| asset-management | has_more_than1_step | True | None |
| change-management | name | change-management | None |
| change-management | count_of_steps | 1 | None |
| content-publishing | name | content-publishing | None |
| content-publishing | count_of_steps | 1 | None |
| contract-review | name | contract-review | None |
| contract-review | count_of_steps | 1 | None |
| customer-feedback | name | customer-feedback | None |
| customer-feedback | count_of_steps | 1 | None |
| expense-reimbursement | name | expense-reimbursement | None |
| expense-reimbursement | count_of_steps | 1 | None |
| incident-reporting | name | incident-reporting | None |
| incident-reporting | count_of_steps | 1 | None |
| invoice-approval | name | invoice-approval | None |
| invoice-approval | count_of_steps | 1 | None |
| it-support | name | it-support | None |
| it-support | count_of_steps | 1 | None |
| leave-request | name | leave-request | None |
| ... | ... | (11 more) | ... |

### precedes_steps

- Fields: 0/16 (0.0%)
- Computed columns: display_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assign-task-to-team | display_name | Step-16 | None |
| step-1 | display_name | Step-1 | None |
| step-10 | display_name | Step-10 | None |
| step-11 | display_name | Step-11 | None |
| step-12 | display_name | Step-12 | None |
| step-13 | display_name | Step-13 | None |
| step-14 | display_name | Step-14 | None |
| step-15 | display_name | Step-15 | None |
| step-2 | display_name | Step-2 | None |
| step-3 | display_name | Step-3 | None |
| step-4 | display_name | Step-4 | None |
| step-5 | display_name | Step-5 | None |
| step-6 | display_name | Step-6 | None |
| step-7 | display_name | Step-7 | None |
| step-8 | display_name | Step-8 | None |
| step-9 | display_name | Step-9 | None |

### departments

- Fields: 0/15 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| administration | name | administration | None |
| customer-service | name | customer-service | None |
| facilities-management | name | facilities-management | None |
| finance | name | finance | None |
| human-resources | name | human-resources | None |
| information-technology | name | information-technology | None |
| legal | name | legal | None |
| marketing | name | marketing | None |
| operations | name | operations | None |
| procurement | name | procurement | None |
| public-relations | name | public-relations | None |
| quality-assurance | name | quality-assurance | None |
| research-and-development | name | research-and-development | None |
| sales | name | sales | None |
| training-and-development | name | training-and-development | None |

### approvals

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| compliance-check | name | compliance-check | None |
| customer-confirmation | name | customer-confirmation | None |
| executive-sign-off | name | executive-sign-off | None |
| final-authorization | name | final-authorization | None |
| finance-approval | name | finance-approval | None |
| initial-review | name | initial-review | None |
| it-security-gate | name | it-security-gate | None |
| legal-review | name | legal-review | None |
| manager-approval | name | manager-approval | None |
| quality-assurance | name | quality-assurance | None |
