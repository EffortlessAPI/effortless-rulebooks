# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-05-26 01:49:31 UTC

## Parsing Rulebook

Found **26** tables in rulebook


  - **Users** (9 fields, 8 records)
  - **Timers** (24 fields, 4 records)
  - **Cases** (3 fields, 4 records)
  - **Bells** (6 fields, 4 records)
  - **WindingKnobs** (10 fields, 4 records)
  - **Arbors** (3 fields, 4 records)
  - **Mainsprings** (6 fields, 4 records)
  - **GearTrains** (8 fields, 4 records)
  - **Gears** (8 fields, 12 records)
  - **OutputShafts** (3 fields, 4 records)
  - **DialPointers** (3 fields, 4 records)
  - **Escapements** (4 fields, 4 records)
  - **EscapementWheels** (3 fields, 4 records)
  - **Pallets** (4 fields, 4 records)
  - **Hairsprings** (3 fields, 4 records)
  - **Cams** (3 fields, 4 records)
  - **BellHammers** (5 fields, 4 records)
  - **HammerArms** (3 fields, 4 records)
  - **Strikers** (3 fields, 4 records)
  - **HammerCatches** (5 fields, 4 records)
  - **WindActions** (7 fields, 5 records)
  - **TickEvents** (7 fields, 23 records)
  - **RingEvents** (4 fields, 1 records)
  - **Cooks** (19 fields, 22 records)
  - **Recipes** (10 fields, 19 records)
  - **Kitchens** (29 fields, 1 records)

Generated **26** table definitions with **67** raw fields (mode=check-add)
Generated **121** calculation functions
Generated **26** views
Enabled RLS on **26** tables
Generated insert statements for **163** records
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

