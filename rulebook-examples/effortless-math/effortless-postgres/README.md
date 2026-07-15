# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-07-15 17:47:20 UTC

## Parsing Rulebook

Found **15** tables in rulebook


  - **Domains** (10 fields, 9 records)
  - **ProofStatuses** (8 fields, 9 records)
  - **Theorems** (15 fields, 12 records)
  - **TheoremDependencies** (11 fields, 7 records)
  - **FoundationKernels** (12 fields, 8 records)
  - **ProofFacts** (10 fields, 135 records)
  - **Loops** (11 fields, 575 records)
  - **InvariantChecks** (11 fields, 313 records)
  - **Sources** (6 fields, 84 records)
  - **TrustBoundaries** (8 fields, 9 records)
  - **ArtifactRegistry** (9 fields, 11 records)
  - **MigrationMappings** (6 fields, 7 records)
  - **ProjectRoadmap** (7 fields, 6 records)
  - **Conclusions** (8 fields, 4 records)
  - **LegacyParentAudits** (11 fields, 9 records)

Generated **15** table definitions with **128** raw fields (mode=check-add)
Generated **15** calculation functions
Generated **15** views
Enabled RLS on **15** tables
Generated insert statements for **1198** records
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

