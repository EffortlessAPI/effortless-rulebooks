# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-06-27 19:41:08 UTC

## Parsing Rulebook

Found **9** tables in rulebook


  - **Loops** (8 fields, 13 records)
  - **Studies** (7 fields, 3 records)
  - **Treatments** (8 fields, 6 records)
  - **Strata** (6 fields, 8 records)
  - **CaseCells** (10 fields, 16 records)
  - **StratumSummaries** (26 fields, 16 records)
  - **ModelSummary** (10 fields, 1 records)
  - **StratumVariables** (9 fields, 3 records)
  - **TreatmentRankings** (24 fields, 3 records)

Generated **9** table definitions with **35** raw fields (mode=check-add)
Generated **70** calculation functions
Generated **9** views
Enabled RLS on **9** tables
Generated insert statements for **69** records
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

