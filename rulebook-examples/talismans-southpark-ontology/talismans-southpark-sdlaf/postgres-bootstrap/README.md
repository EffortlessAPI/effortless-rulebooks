# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `erb_talismans_southpark_sdlaf`
**Timestamp:** 2026-11-28 03:38:52 UTC

## Parsing Rulebook

Found **9** tables in rulebook


  - **Reasoning** (28 fields, 1 records)
  - **Portraits** (9 fields, 7 records)
  - **Individuals** (48 fields, 7 records)
  - **Parenthoods** (16 fields, 3 records)
  - **Marriages** (10 fields, 1 records)
  - **Friendships** (10 fields, 1 records)
  - **Grandparenthoods** (14 fields, 9 records)
  - **GradeLevelRecords** (11 fields, 1 records)
  - **RoleRecords** (11 fields, 1 records)

Generated **9** table definitions with **33** raw fields (mode=drop-all)
Generated **128** calculation functions
Generated **9** views
Generated **1** transitive-closure view(s)
Enabled RLS on **9** tables
Generated insert statements for **31** records
Generated **9** table definitions with **33** raw fields (mode=drop-all)
Generated **128** calculation functions
Generated **9** views
Generated **1** transitive-closure view(s)
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


