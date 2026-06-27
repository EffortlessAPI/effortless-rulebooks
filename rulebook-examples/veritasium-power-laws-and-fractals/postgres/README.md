# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2025-12-11 18:54:38 UTC

## Parsing Rulebook

Found **3** tables in rulebook

  - **systems** (8 fields, 4 records)
  - **scales** (8 fields, 16 records)
  - **system_stats** (8 fields, 4 records)

Generated **3** table definitions with **12** raw fields
Generated **9** calculation functions
Generated **3** views
Enabled RLS on **3** tables
Generated insert statements for **24** records
## Script Generation Complete

Generated files:
- `00-drop-all.sql` - Drop all existing objects
- `01-create-tables.sql` - Create tables with raw fields
- `02-create-functions.sql` - Create calculation functions
- `03-create-views.sql` - Create views with calculated fields
- `04-create-policies.sql` - Create RLS policies
- `05-insert-data.sql` - Insert data from rulebook
- `init-db.sh` - Database initialization script

