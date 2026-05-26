# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 122 |
| Passed | 13 |
| Failed | 109 |
| Score | 10.7% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 13 | 107 | 12.1% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
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

- Fields: 1/20 (5.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| archive-request | name | archive-request | None |
| asdf | name | asdf | None |
| assign-task-to-team | name | assign-task-to-team | None |
| automated-eligibility-check | name | automated-eligibility-check | None |
| close-workflow | name | close-workflow | None |
| customer-feedback-collection | name | customer-feedback-collection | None |
| document-verification | name | document-verification | None |
| final-approval | name | final-approval | None |
| finance-approval | name | finance-approval | None |
| generate-report | name | generate-report | None |
| legal-compliance-check | name | legal-compliance-check | None |
| manager-review | name | manager-review | None |
| quality-assurance-review | name | quality-assurance-review | None |
| some-new-task | name | some-new-task | None |
| step-2 | name | step-2 | None |
| step1 | name | step1 | None |
| submit-request | name | submit-request | None |
| system-notification-sent | name | system-notification-sent | None |
| team-lead-review | name | team-lead-review | None |

### workflows

- Fields: 12/45 (26.7%)
- Computed columns: name, count_of_non_proposed_steps, has_more_than1_step

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| asset-management | name | asset-management | None |
| asset-management | count_of_non_proposed_steps | 2 | None |
| asset-management | has_more_than1_step | True | None |
| change-management | name | change-management | None |
| change-management | count_of_non_proposed_steps | 3 | None |
| change-management | has_more_than1_step | True | None |
| content-publishing | name | content-publishing | None |
| content-publishing | count_of_non_proposed_steps | 1 | None |
| contract-review | name | contract-review | None |
| contract-review | count_of_non_proposed_steps | 1 | None |
| customer-feedback | name | customer-feedback | None |
| customer-feedback | count_of_non_proposed_steps | 1 | None |
| expense-reimbursement | name | expense-reimbursement | None |
| expense-reimbursement | count_of_non_proposed_steps | 1 | None |
| incident-reporting | name | incident-reporting | None |
| incident-reporting | count_of_non_proposed_steps | 1 | None |
| invoice-approval | name | invoice-approval | None |
| invoice-approval | count_of_non_proposed_steps | 1 | None |
| it-support | name | it-support | None |
| it-support | count_of_non_proposed_steps | 1 | None |
| ... | ... | (13 more) | ... |

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

### approval_gates

- Fields: 0/11 (0.0%)
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
| some-new-gate | name | some-new-gate | None |
