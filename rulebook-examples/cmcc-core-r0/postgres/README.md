# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-09-29 17:06:02 UTC

## Parsing Rulebook

Found **14** tables in rulebook


  - **EDBRelations** (7 fields, 3 records)
  - **Commands** (6 fields, 5 records)
  - **Rules** (13 fields, 25 records)
  - **Transitions** (7 fields, 6 records)
  - **TransitionEffects** (6 fields, 6 records)
  - **TransitionEvents** (5 fields, 2 records)
  - **Constraints** (4 fields, 2 records)
  - **Observables** (4 fields, 9 records)
  - **SeedFacts** (7 fields, 7 records)
  - **InputHistories** (6 fields, 2 records)
  - **ReflectionConditions** (7 fields, 6 records)
  - **Substrates** (14 fields, 3 records)
  - **AnswerKeyResults** (7 fields, 0 records)
  - **ReflectionResults** (5 fields, 0 records)

Generated **14** table definitions with **51** raw fields (mode=check-add)
Generated **56** calculation functions
Generated **14** views
Enabled RLS on **14** tables
Generated insert statements for **76** records
Generated **14** table definitions with **51** raw fields (mode=check-add)
Generated **56** calculation functions
Generated **14** views
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


