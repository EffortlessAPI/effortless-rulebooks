# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-26 01:00:42 UTC

## Parsing Rulebook

Found **6** tables in rulebook

Skipped emitting **ERBVersions** (emit_erb_internals=false; bases-only).
Skipped emitting **ERBCustomizations** (emit_erb_internals=false; bases-only).

  - **Series** (16 fields, 10 records)
  - **Seasons** (14 fields, 3 records)
  - **Episodes** (15 fields, 292 records)
  - **Ratings** (14 fields, 13 records)

## ERB Customizations

  - **03a-customize-schema.sql** (1439 bytes)
  - **03b-customize-functions.sql** (1915 bytes)
  - **03c-customize-views.sql** (1818 bytes)
  - **03d-customize-rls.sql** (1827 bytes)
  - **03e-customize-data.sql** (1835 bytes)

Generated **4** table definitions with **31** raw fields (mode=check-add)
Generated **50** calculation functions
Generated **4** views
Enabled RLS on **4** tables
Generated insert statements for **318** records
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

