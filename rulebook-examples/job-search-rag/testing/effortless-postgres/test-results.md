# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 18 |
| Passed | 18 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | — | 0 | n/a |
| Lookup (INDEX/MATCH) | 12 | 12 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 6 | 6 | 100.0% |

## Results by Entity

### resumes

- Fields: 2/2 (100.0%)
- Computed columns: resume_sections, search_runs

### search_urls

- Fields: 8/8 (100.0%)
- Computed columns: job_board_name

### job_boards

- Fields: 4/4 (100.0%)
- Computed columns: job_listings

### resume_sections

- Fields: 4/4 (100.0%)
- Computed columns: resume_name
