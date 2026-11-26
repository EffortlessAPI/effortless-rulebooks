# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-11-26 06:57:21 UTC

## Parsing Rulebook

Found **8** tables in rulebook


  - **Graphs** (20 fields, 1 records)
  - **Rules** (7 fields, 5 records)
  - **FiringKinds** (6 fields, 5 records)
  - **Things** (16 fields, 9 records)
  - **Properties** (15 fields, 10 records)
  - **Statements** (51 fields, 270 records)
  - **Firings** (36 fields, 465 records)
  - **DisjointChecks** (13 fields, 42 records)

Generated **8** table definitions with **41** raw fields (mode=drop-all)
Generated **136** calculation functions
Generated **8** views
Enabled RLS on **8** tables
Generated insert statements for **807** records
Generated **8** table definitions with **41** raw fields (mode=drop-all)
Generated **136** calculation functions
Generated **8** views
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


