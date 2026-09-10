# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 43 |
| Passed | 43 |
| Failed | 0 |
| Score | 100.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 6 | 6 | 100.0% |
| Lookup (INDEX/MATCH) | 30 | 30 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 7 | 7 | 100.0% |

## Results by Entity

### client

- Fields: 3/3 (100.0%)
- Computed columns: full_name

### projects

- Fields: 24/24 (100.0%)
- Computed columns: project_type_name, project_type_description, project_type_requires_manager_approval, is_approved, approved_by_role_is_manager, approved_by_name, approved_by_email_address, approved_by_phone_number

### employees

- Fields: 9/9 (100.0%)
- Computed columns: role_name, role_description, role_is_manager

### roles

- Fields: 3/3 (100.0%)
- Computed columns: count_of_employees

### types_of_project

- Fields: 4/4 (100.0%)
- Computed columns: count_of_projects
