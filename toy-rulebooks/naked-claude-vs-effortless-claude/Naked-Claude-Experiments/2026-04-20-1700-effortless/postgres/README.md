# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-04-21 04:57:36 UTC

## Parsing Rulebook

Found **16** tables in rulebook

  - **Name** (0 fields, 0 records)
  - **AppUsers** (8 fields, 3 records)
  - **Clients** (25 fields, 5 records)
  - **ClientApprovals** (14 fields, 3 records)
  - **ClientCategories** (6 fields, 3 records)
  - **Statuses** (8 fields, 7 records)
  - **Products** (15 fields, 8 records)
  - **Invoices** (29 fields, 7 records)
  - **InvoiceLineItems** (17 fields, 18 records)
  - **InventoryAdjustments** (9 fields, 8 records)
  - **Payments** (18 fields, 9 records)
  - **Addresses** (13 fields, 6 records)
  - **States** (6 fields, 50 records)
  - **TypesOfAddresses** (4 fields, 2 records)
  - **ERBVersions** (9 fields, 3 records)
  - **ERBCustomizations** (6 fields, 3 records)

## Function Overrides

  - **calc_orders_item_count** (overridden)
  - **calc_orders_last_payment_date** (overridden)
  - **calc_orders_payment_count** (overridden)

Generated **16** table definitions with **86** raw fields
Generated **144** calculation functions
Generated **16** views
Enabled RLS on **16** tables
Generated insert statements for **135** records
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

