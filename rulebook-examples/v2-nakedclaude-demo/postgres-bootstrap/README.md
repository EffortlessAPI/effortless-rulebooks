# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-04-27 23:21:40 UTC

## Parsing Rulebook

Found **3** tables in rulebook

  - **Customers** (12 fields, 5 records)
  - **Statuses** (7 fields, 7 records)
  - **AppUsers** (4 fields, 3 records)

Generated **3** table definitions with **13** raw fields
Generated **16** calculation functions
Generated **3** views
Enabled RLS on **3** tables
Generated insert statements for **15** records
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

