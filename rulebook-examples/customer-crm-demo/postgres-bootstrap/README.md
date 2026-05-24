# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-20 06:17:19 UTC

## Parsing Rulebook

Found **4** tables in rulebook


  - **Users** (7 fields, 2 records)
  - **Customers** (10 fields, 5 records)
  - **Orders** (17 fields, 10 records)
  - **Payments** (16 fields, 9 records)

Generated **4** table definitions with **20** raw fields (mode=check-add)
Generated **37** calculation functions
Generated **4** views
Enabled RLS on **4** tables
Generated insert statements for **26** records
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

