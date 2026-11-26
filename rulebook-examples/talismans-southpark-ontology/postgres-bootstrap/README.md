# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-11-26 09:05:51 UTC

## Parsing Rulebook

Found **19** tables in rulebook


  - **Graphs** (22 fields, 1 records)
  - **Rules** (7 fields, 5 records)
  - **FiringKinds** (6 fields, 6 records)
  - **Things** (20 fields, 9 records)
  - **Portraits** (9 fields, 13 records)
  - **PortraitAliases** (3 fields, 32 records)
  - **Properties** (16 fields, 10 records)
  - **PropertyAxioms** (6 fields, 4 records)
  - **Statements** (62 fields, 270 records)
  - **Firings** (43 fields, 465 records)
  - **DisjointChecks** (13 fields, 42 records)
  - **Scenarios** (14 fields, 7 records)
  - **ScenarioFactChanges** (9 fields, 6 records)
  - **ScenarioRuleSettings** (5 fields, 10 records)
  - **NodeLayouts** (6 fields, 26 records)
  - **LayoutSlots** (5 fields, 8 records)
  - **PageTexts** (3 fields, 102 records)
  - **Engines** (7 fields, 3 records)
  - **RulebookTools** (7 fields, 8 records)

Generated **19** table definitions with **92** raw fields (mode=drop-all)
Generated **193** calculation functions
Generated **19** views
Enabled RLS on **19** tables
Generated insert statements for **1027** records
Generated **19** table definitions with **92** raw fields (mode=drop-all)
Generated **193** calculation functions
Generated **19** views
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


