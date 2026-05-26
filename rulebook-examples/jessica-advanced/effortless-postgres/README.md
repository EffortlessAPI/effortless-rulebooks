# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-26 01:00:14 UTC

## Parsing Rulebook

Found **10** tables in rulebook


  - **Workflows** (10 fields, 15 records)
  - **WorkflowSteps** (12 fields, 16 records)
  - **Approvals** (5 fields, 10 records)
  - **PrecedesSteps** (5 fields, 16 records)
  - **Roles** (12 fields, 15 records)
  - **Departments** (5 fields, 15 records)
  - **HumanAgents** (5 fields, 15 records)
  - **AIAgents** (6 fields, 15 records)
  - **AutomatedPipelines** (5 fields, 15 records)
  - **SampleDataGovernance** (7 fields, 34 records)

Generated **10** table definitions with **43** raw fields (mode=check-add)
Generated **41** calculation functions
Generated **10** views
Enabled RLS on **10** tables
Generated insert statements for **166** records
## Script Generation Complete

Generated files:
- `00-bootstrap.sql` - Bootstrap (overwrite Never); includes commented-out drop-all script
- `01-drop-and-create-tables.sql` - Drop and recreate tables with raw fields and FK indexes
- `01b-customize-schema.sql` - User customizations for schema
- `02-create-functions.sql` - Create calculation functions
- `02b-customize-functions.sql` - User customizations for functions
- `03-create-views.sql` - Create views with calculated fields
- `03b-customize-views.sql` - User customizations for views
- `04-create-policies.sql` - Create RLS policies
- `04b-customize-policies.sql` - User customizations for RLS policies
- `05-insert-data.sql` - Insert data from rulebook
- `05b-customize-data.sql` - User customizations for seed data
- `99-fk-constraints.sql` - FK constraints (skipped unless EFFORTLESS_ENFORCE_FKS=true)
- `init-db.sh` - Database initialization script

