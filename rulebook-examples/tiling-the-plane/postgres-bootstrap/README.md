# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-06-06 19:52:50 UTC

## Parsing Rulebook

Found **8** tables in rulebook


  - **SymmetryGroups** (16 fields, 17 records)
  - **Prototiles** (12 fields, 6 records)
  - **Tilings** (21 fields, 7 records)
  - **TilingPrototiles** (7 fields, 11 records)
  - **VertexFigures** (8 fields, 7 records)
  - **Regions** (13 fields, 3 records)
  - **Placements** (13 fields, 20 records)
  - **WallpaperDesigns** (12 fields, 3 records)

Generated **8** table definitions with **45** raw fields (mode=drop-all)
Generated **75** calculation functions
Generated **8** views
Enabled RLS on **8** tables
Generated insert statements for **74** records
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

