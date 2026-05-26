# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-26 01:01:54 UTC

## Parsing Rulebook

Found **9** tables in rulebook


  - **ProjectMetadata** (5 fields, 1 records)
  - **ExecutionSubstrates** (9 fields, 10 records)
  - **OrchestrationComponents** (6 fields, 7 records)
  - **AirtableIntegration** (5 fields, 3 records)
  - **TestingFramework** (5 fields, 4 records)
  - **RulebookDomains** (8 fields, 7 records)
  - **CoreDataFlows** (5 fields, 5 records)
  - **ProjectConfiguration** (6 fields, 9 records)
  - **Dependencies** (6 fields, 9 records)

Generated **9** table definitions with **55** raw fields (mode=check-add)
Generated **0** calculation functions
Generated **9** views
Enabled RLS on **9** tables
Generated insert statements for **55** records
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

