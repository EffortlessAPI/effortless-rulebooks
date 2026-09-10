# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-09-13 06:12:16 UTC

## Parsing Rulebook

Found **1** tables in rulebook


  - **HelloWhos** (3 fields, 3 records)

Generated **1** table definitions with **2** raw fields (mode=check-add)
Generated **1** calculation functions
Generated **1** views
Enabled RLS on **1** tables
Generated insert statements for **3** records
Generated **1** table definitions with **2** raw fields (mode=check-add)
Generated **1** calculation functions
Generated **1** views
## Script Generation Complete

Generated files:
- `00-bootstrap.sql` - Bootstrap (overwrite Never); includes commented-out drop-all script
- `01-drop-and-create-tables.sql` - Drop and recreate tables with raw fields and FK indexes
- `02-create-functions.sql` - Create calculation functions
- `03-create-views.sql` - Create views with calculated fields
- `04-create-policies.sql` - Create RLS policies
- `05-insert-data.sql` - Insert data from rulebook
- `99-fk-constraints.sql` - FK constraints (skipped unless EFFORTLESS_ENFORCE_FKS=true)
- `reset-rulebook-db.sh` - DESTRUCTIVE localhost reset: rebuild the dev DB from the rulebook
- `init-db.sh` - Legacy compatibility entry point that delegates to reset-rulebook-db.sh
- `update-effortless-schema.sql` - Rebuild the derived layer (views / calc_* / RLS). Safe in production.

## Where things are defined

The rulebook is the source of truth. Everything above is derived from it.
Tables and columns are **migrated** (additive, forever). Views, `calc_*`
functions and RLS policies are **derived** — they hold no data, so they are
dropped and recreated from the rulebook rather than migrated. Never
hand-write a migration that recreates a view or a calc function.


