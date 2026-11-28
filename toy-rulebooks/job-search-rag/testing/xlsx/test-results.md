# Test Results: xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 18 |
| Passed | 2 |
| Failed | 16 |
| Score | 11.1% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | — | 0 | n/a |
| Lookup (INDEX/MATCH) | 0 | 12 | 0.0% |
| Aggregation (COUNTIFS/SUMIFS) | 2 | 6 | 33.3% |

## Results by Entity

### job_boards

- Fields: 2/4 (50.0%)
- Computed columns: job_listings

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| weworkremotely | job_listings | 1 | None |
| ziprecruiter | job_listings | 5 | None |

### search_urls

- Fields: 0/8 (0.0%)
- Computed columns: job_board_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| indeed-principal-data-platform | job_board_name | Indeed | None |
| indeed-staff-platform-architect | job_board_name | Indeed | None |
| li-staff-platform-architect | job_board_name | LinkedIn | None |
| wwr-principal-architect | job_board_name | WeWorkRemotely | None |
| wwr-staff-engineer | job_board_name | WeWorkRemotely | None |
| zr-devrel-engineer | job_board_name | ZipRecruiter | None |
| zr-principal-data-platform | job_board_name | ZipRecruiter | None |
| zr-staff-platform-architect | job_board_name | ZipRecruiter | None |

### resumes

- Fields: 0/2 (0.0%)
- Computed columns: resume_sections, search_runs

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| resume-platform-2026q1 | resume_sections | 8 | None |
| resume-platform-2026q1 | search_runs | 1 | None |

### resume_sections

- Fields: 0/4 (0.0%)
- Computed columns: resume_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| resume-platform-2026q1-core-strengths | resume_name | Platform Engineer - 2026 Q1 | None |
| resume-platform-2026q1-earlier-roles | resume_name | Platform Engineer - 2026 Q1 | None |
| resume-platform-2026q1-experience | resume_name | Platform Engineer - 2026 Q1 | None |
| resume-platform-2026q1-summary | resume_name | Platform Engineer - 2026 Q1 | None |
