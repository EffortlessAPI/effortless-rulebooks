# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-06-28 01:56:18 UTC

## Parsing Rulebook

Found **15** tables in rulebook


  - **Loops** (8 fields, 35 records)
  - **Studies** (7 fields, 16 records)
  - **Treatments** (8 fields, 20 records)
  - **Strata** (6 fields, 40 records)
  - **CaseCells** (10 fields, 80 records)
  - **StratumSummaries** (28 fields, 80 records)
  - **ModelSummary** (21 fields, 1 records)
  - **StratumVariables** (10 fields, 16 records)
  - **TreatmentRankings** (47 fields, 16 records)
  - **InvariantChecks** (15 fields, 7 records)
  - **Methodology** (7 fields, 10 records)
  - **Conclusions** (8 fields, 11 records)
  - **UIScreens** (8 fields, 9 records)
  - **UIComponents** (8 fields, 10 records)
  - **InstrumentSpec** (9 fields, 20 records)

Generated **15** table definitions with **83** raw fields (mode=check-add)
Generated **114** calculation functions
Generated **15** views
Enabled RLS on **15** tables
Generated insert statements for **371** records
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

