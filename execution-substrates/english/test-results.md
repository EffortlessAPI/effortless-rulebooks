# Test Results: english

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 122 |
| Passed | 106 |
| Failed | 16 |
| Score | 86.9% |
| Duration | 2m 37s |

## Results by Entity

### roles

- Fields: 5/15 (33.3%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| administrator | name | administrator | admin |
| analyst | name | analyst | data-analyst |
| contributor | name | contributor | content-contributor |
| developer | name | developer | software-developer |
| editor | name | editor | content-editor |
| finance | name | finance | finance-officer |
| guest | name | guest | temporary-access |
| hr | name | hr | human-resources |
| manager | name | manager | project-manager |
| moderator | name | moderator | forum-moderator |

### workflow_steps

- Fields: 19/20 (95.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| recwwXHLqxKPhj6Mt | name |  | recwwxhlqxkphj6mt |

### workflows

- Fields: 40/45 (88.9%)
- Computed columns: name, count_of_non_proposed_steps, has_more_than1_step

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| onboarding | name | onboarding | employee-onboarding |
| performance-review | name | performance-review | annual-performance-review |
| procurement | name | procurement | procurement-workflow |
| project-kickoff | name | project-kickoff | project-kickoff-process |
| recruitment | name | recruitment | recruitment-workflow |

### precedes_steps

- Fields: 16/16 (100.0%)
- Computed columns: display_name

### departments

- Fields: 15/15 (100.0%)
- Computed columns: name

### approval_gates

- Fields: 11/11 (100.0%)
- Computed columns: name
