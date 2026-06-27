# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-04-20 21:34:48 UTC

## Parsing Rulebook

Found **9** tables in rulebook

  - **Name** (0 fields, 0 records)
  - **Customers** (18 fields, 5 records)
  - **Statuses** (7 fields, 7 records)
  - **Products** (10 fields, 8 records)
  - **Orders** (24 fields, 7 records)
  - **OrderLineItems** (12 fields, 18 records)
  - **Payments** (18 fields, 9 records)
  - **ERBVersions** (9 fields, 3 records)
  - **ERBCustomizations** (6 fields, 3 records)

## Function Overrides

  - **calc_orders_item_count** (overridden)
  - **calc_orders_last_payment_date** (overridden)
  - **calc_orders_payment_count** (overridden)

Generated **9** table definitions with **59** raw fields
Generated **76** calculation functions
Generated **9** views
Enabled RLS on **9** tables
Generated insert statements for **60** records
## Script Generation Complete

Generated files:
- `00-bootstrap.sql` - Bootstrap (overwrite Never); includes commented-out drop-all script
- `01-drop-and-create-tables.sql` - Drop and recreate tables with raw fields
- `01b-customize-schema.sql` - User customizations for schema
- `02-create-functions.sql` - Create calculation functions
- `02b-customize-functions.sql` - User customizations for functions
- `03-create-views.sql` - Create views with calculated fields
- `03b-customize-views.sql` - User customizations for views
- `04-create-policies.sql` - Create RLS policies
- `04b-customize-policies.sql` - User customizations for RLS policies
- `05-insert-data.sql` - Insert data from rulebook
- `05b-customize-data.sql` - User customizations for seed data
- `init-db.sh` - Database initialization script

