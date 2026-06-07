# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-06-07 21:28:55 UTC

## Parsing Rulebook

Found **39** tables in rulebook

Skipped emitting **ERBVersions** (emit_erb_internals=false; bases-only).
Skipped emitting **ERBCustomizations** (emit_erb_internals=false; bases-only).

  - **BusinessRules** (13 fields, 18 records)
  - **BusinessRuleCategories** (12 fields, 8 records)
  - **GlossaryCategories** (12 fields, 4 records)
  - **GlossaryTerms** (11 fields, 16 records)
  - **Roles** (15 fields, 4 records)
  - **AuditLogEntries** (17 fields, 7 records)
  - **PlatformNaviation** (51 fields, 22 records)
  - **Jurisdictions** (29 fields, 3 records)
  - **JurisdictionSourceDocuments** (18 fields, 5 records)
  - **JurisdictionRules** (24 fields, 5 records)
  - **AppUsers** (14 fields, 7 records)
  - **MagicLinkConfig** (14 fields, 1 records)
  - **SiteBranding** (37 fields, 1 records)
  - **ReferenceDocuments** (15 fields, 4 records)
  - **StateMachines** (18 fields, 4 records)
  - **MachineStates** (15 fields, 18 records)
  - **StateTransitionRules** (15 fields, 15 records)
  - **StateTransitions** (17 fields, 16 records)
  - **WorkQueueItems** (17 fields, 4 records)
  - **AiModels** (18 fields, 3 records)
  - **ModelPricingVersions** (19 fields, 4 records)
  - **AssistantTurns** (30 fields, 5 records)
  - **Platforms** (12 fields, 1 records)
  - **ERBPackages** (21 fields, 1 records)
  - **ERBFeatureStatuses** (14 fields, 5 records)
  - **ERBFeatureCategories** (13 fields, 10 records)
  - **ERBFeatures** (26 fields, 16 records)
  - **ERBTables** (45 fields, 12 records)
  - **ERBFields** (34 fields, 12 records)
  - **APIEndpoints** (17 fields, 9 records)
  - **SubjectStateInstances** (18 fields, 8 records)
  - **ViolationTypes** (18 fields, 7 records)
  - **Drivers** (20 fields, 3 records)
  - **Citations** (45 fields, 6 records)
  - **Hearings** (15 fields, 2 records)
  - **Payments** (14 fields, 1 records)
  - **CaseEvents** (16 fields, 7 records)

## Function Overrides

  - **calc_hearing_outcomes_employer_won_flag** (overridden)

## ERB Customizations

  - **02b-customize-override-functions.sql** (22 bytes)

Generated **37** table definitions with **497** raw fields (mode=drop-all)
Generated **560** calculation functions
Generated **37** views
Enabled RLS on **37** tables
Generated insert statements for **274** records
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

