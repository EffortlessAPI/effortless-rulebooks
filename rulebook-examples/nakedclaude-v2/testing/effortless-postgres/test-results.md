# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 40 |
| Passed | 7 |
| Failed | 33 |
| Score | 17.5% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 4 | 25 | 16.0% |
| Lookup (INDEX/MATCH) | 3 | 15 | 20.0% |
| Aggregation (COUNTIFS/SUMIFS) | — | 0 | n/a |

## Results by Entity

### app_users

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-example-com | name | alice-example.com | None |
| eejai42-gmail-com | name | eejai42-gmail.com | None |
| ej-ssot-me | name | ej-ssot.me | None |

### customers

- Fields: 7/30 (23.3%)
- Computed columns: name, full_name, status_display_name, status_is_blocking, is_stopped, status_description

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson | name | alice-johnson | None |
| alice-johnson | full_name | Alice Johnson | None |
| alice-johnson | status_display_name | Pending | None |
| alice-johnson | status_is_blocking | True | None |
| alice-johnson | status_description | This status shows the customer | None |
| bob | name | bob- | None |
| bob | full_name | Bob  | None |
| bob | status_display_name | On-Hold | None |
| bob | status_is_blocking | True | None |
| bob | is_stopped | True | None |
| bob | status_description | This status indicates the cust | None |
| brian-lee | name | brian-lee | None |
| brian-lee | full_name | Brian Lee | None |
| brian-lee | status_display_name | New | None |
| brian-lee | status_description | This status indicates a newly  | None |
| carla-smith | name | carla-smith | None |
| carla-smith | full_name | Carla Smith | None |
| carla-smith | status_display_name | Delayed | None |
| carla-smith | status_description | This status is used when the c | None |
| caroline | name | caroline- | None |
| ... | ... | (3 more) | ... |

### statuses

- Fields: 0/7 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cancelled | name | cancelled | None |
| delayed | name | delayed | None |
| in-review | name | in-review | None |
| new | name | new | None |
| on-hold | name | on-hold | None |
| pending | name | pending | None |
| processing | name | processing | None |
