# Test Results: effortless-postgres

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
| weworkremotely | job_listings | None | 1 |
| ziprecruiter | job_listings | None | 5 |

### search_urls

- Fields: 0/8 (0.0%)
- Computed columns: job_board_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| indeed-principal-data-platform | job_board_name | None | Indeed |
| indeed-staff-platform-architect | job_board_name | None | Indeed |
| li-staff-platform-architect | job_board_name | None | LinkedIn |
| wwr-principal-architect | job_board_name | None | WeWorkRemotely |
| wwr-staff-engineer | job_board_name | None | WeWorkRemotely |
| zr-devrel-engineer | job_board_name | None | ZipRecruiter |
| zr-principal-data-platform | job_board_name | None | ZipRecruiter |
| zr-staff-platform-architect | job_board_name | None | ZipRecruiter |

### resumes

- Fields: 0/2 (0.0%)
- Computed columns: resume_sections, search_runs

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| resume-platform-2026q1 | resume_sections | None | 8 |
| resume-platform-2026q1 | search_runs | None | 1 |

### resume_sections

- Fields: 0/4 (0.0%)
- Computed columns: resume_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| resume-platform-2026q1-core-strengths | resume_name | None | Platform Engineer - 2026 Q1 |
| resume-platform-2026q1-earlier-roles | resume_name | None | Platform Engineer - 2026 Q1 |
| resume-platform-2026q1-experience | resume_name | None | Platform Engineer - 2026 Q1 |
| resume-platform-2026q1-summary | resume_name | None | Platform Engineer - 2026 Q1 |
