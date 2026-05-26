# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-26 01:01:23 UTC

## Parsing Rulebook

Found **3** tables in rulebook

Skipped emitting **ERBCustomizations** (emit_erb_internals=false; bases-only).

  - **LanguageCandidates** (26 fields, 33 records)
  - **IsEverythingALanguage** (11 fields, 16 records)

## ERB Customizations

  - **01b-customize-schema.sql** (865 bytes)
  - **02b-customize-functions.sql** (2129 bytes)
  - **03b-customize-views.sql** (619 bytes)
  - **04b-customize-rls.sql** (635 bytes)
  - **05b-customize-data.sql** (628 bytes)

Generated **2** table definitions with **29** raw fields (mode=check-add)
Generated **8** calculation functions
Generated **2** views
Enabled RLS on **2** tables
Generated insert statements for **49** records
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

