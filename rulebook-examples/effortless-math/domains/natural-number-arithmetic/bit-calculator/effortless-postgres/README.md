# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-07-15 19:27:43 UTC

## Parsing Rulebook

Found **7** tables in rulebook


  - **GateTypes** (6 fields, 4 records)
  - **GateTruthRows** (6 fields, 14 records)
  - **Components** (5 fields, 9 records)
  - **ComponentPins** (7 fields, 105 records)
  - **ComponentInstances** (7 fields, 78 records)
  - **ComponentConnections** (7 fields, 281 records)
  - **Computations** (11 fields, 4 records)

Generated **7** table definitions with **42** raw fields (mode=check-add)
Generated **7** calculation functions
Generated **7** views
Enabled RLS on **7** tables
Generated insert statements for **495** records
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

