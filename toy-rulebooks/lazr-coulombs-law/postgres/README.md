# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-06-30 16:11:45 UTC

## Parsing Rulebook

Found **10** tables in rulebook


  - **Loops** (10 fields, 4 records)
  - **PhysicsConstants** (6 fields, 6 records)
  - **Particles** (6 fields, 6 records)
  - **Charges** (14 fields, 7 records)
  - **Systems** (11 fields, 3 records)
  - **ChargeInteractions** (28 fields, 5 records)
  - **ForceVectors** (17 fields, 5 records)
  - **ElectricFieldPoints** (24 fields, 7 records)
  - **InvariantChecks** (6 fields, 8 records)
  - **SystemSummary** (15 fields, 3 records)

Generated **10** table definitions with **76** raw fields (mode=check-add)
Generated **58** calculation functions
Generated **10** views
Enabled RLS on **10** tables
Generated insert statements for **54** records
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

