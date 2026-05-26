# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-26 01:40:42 UTC

## Parsing Rulebook

Found **10** tables in rulebook


  - **Users** (12 fields, 5 records)
  - **Businesses** (28 fields, 5 records)
  - **BeneficialOwners** (12 fields, 5 records)
  - **Contacts** (12 fields, 4 records)
  - **Accounts** (13 fields, 5 records)
  - **Loans** (34 fields, 3 records)
  - **Covenants** (13 fields, 4 records)
  - **RiskRatingHistory** (14 fields, 5 records)
  - **Documents** (15 fields, 5 records)
  - **Interactions** (16 fields, 9 records)

Generated **10** table definitions with **73** raw fields (mode=check-add)
Generated **103** calculation functions
Generated **10** views
Enabled RLS on **10** tables
Generated insert statements for **50** records
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

