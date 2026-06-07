# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-06-07 20:38:47 UTC

## Parsing Rulebook

Found **14** tables in rulebook


  - **Workflows** (23 fields, 1 records)
  - **WorkflowSteps** (23 fields, 6 records)
  - **ApprovalGates** (7 fields, 1 records)
  - **StepPrecedence** (5 fields, 5 records)
  - **Roles** (17 fields, 6 records)
  - **Departments** (5 fields, 2 records)
  - **HumanAgents** (5 fields, 4 records)
  - **AIAgents** (6 fields, 1 records)
  - **AutomatedPipelines** (5 fields, 1 records)
  - **WorkflowStatusConcepts** (6 fields, 4 records)
  - **AgentCapabilityConcepts** (6 fields, 5 records)
  - **Datasets** (6 fields, 1 records)
  - **WorkflowArtifacts** (12 fields, 5 records)
  - **ComplianceVerdicts** (10 fields, 1 records)

Generated **14** table definitions with **56** raw fields (mode=check-add)
Generated **90** calculation functions
Generated **14** views
Generated **2** transitive-closure view(s)
Enabled RLS on **14** tables
Generated insert statements for **43** records
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

