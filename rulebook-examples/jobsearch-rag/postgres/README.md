# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-04-03 23:52:56 UTC

## Parsing Rulebook

Found **11** tables in rulebook

  - **Name** (0 fields, 0 records)
  - **JobBoards** (8 fields, 4 records)
  - **SearchUrls** (6 fields, 8 records)
  - **RoleArchetypes** (5 fields, 4 records)
  - **RubricDimensions** (5 fields, 10 records)
  - **Resumes** (7 fields, 1 records)
  - **ResumeSections** (7 fields, 4 records)
  - **SearchRuns** (20 fields, 0 records)
  - **JobListings** (16 fields, 0 records)
  - **ScoreResults** (17 fields, 0 records)
  - **Decisions** (10 fields, 0 records)

Generated **11** table definitions with **74** raw fields
Generated **46** calculation functions
Generated **11** views
Enabled RLS on **11** tables
Generated insert statements for **31** records
## Script Generation Complete

Generated files:
- `00-bootstrap.sql` - Bootstrap (overwrite Never); includes commented-out drop-all script
- `01-drop-and-create-tables.sql` - Drop and recreate tables with raw fields
- `01b-customize-schema.sql` - User customizations for schema
- `02-create-functions.sql` - Create calculation functions
- `02b-customize-functions.sql` - User customizations for functions
- `03-create-views.sql` - Create views with calculated fields
- `03b-customize-views.sql` - User customizations for views
- `04-create-policies.sql` - Create RLS policies
- `04b-customize-policies.sql` - User customizations for RLS policies
- `05-insert-data.sql` - Insert data from rulebook
- `05b-customize-data.sql` - User customizations for seed data
- `init-db.sh` - Database initialization script

