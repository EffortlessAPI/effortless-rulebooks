# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-07-19 21:20:33 UTC

## Parsing Rulebook

Found **50** tables in rulebook

Skipped emitting **ERBVersions** (emit_erb_internals=false; bases-only).
Skipped emitting **ERBCustomizations** (emit_erb_internals=false; bases-only).

  - **RulebookReleases** (11 fields, 1 records)
  - **OntologyProfiles** (8 fields, 11 records)
  - **Organizations** (6 fields, 4 records)
  - **Agents** (8 fields, 10 records)
  - **Roles** (8 fields, 12 records)
  - **RoleAssignments** (10 fields, 8 records)
  - **CommunitiesOfPractice** (8 fields, 2 records)
  - **Mentorships** (10 fields, 1 records)
  - **ProcedureTypes** (5 fields, 2 records)
  - **Procedures** (11 fields, 2 records)
  - **ProcedureVersions** (17 fields, 3 records)
  - **ProcedureVersionLinks** (6 fields, 1 records)
  - **ProcedureStatusChanges** (9 fields, 5 records)
  - **Steps** (14 fields, 17 records)
  - **StepTransitions** (9 fields, 19 records)
  - **Actions** (5 fields, 9 records)
  - **Functions** (6 fields, 8 records)
  - **Tools** (5 fields, 8 records)
  - **StepActions** (4 fields, 9 records)
  - **StepFunctions** (4 fields, 8 records)
  - **StepTools** (4 fields, 10 records)
  - **Requirements** (8 fields, 13 records)
  - **StepRequirements** (4 fields, 15 records)
  - **StepVerifications** (8 fields, 11 records)
  - **Rationales** (9 fields, 4 records)
  - **Exceptions** (10 fields, 4 records)
  - **Resources** (9 fields, 8 records)
  - **ProcedureResources** (6 fields, 8 records)
  - **ElicitationSessions** (11 fields, 3 records)
  - **KnowledgeFragments** (15 fields, 7 records)
  - **KnowledgeGaps** (13 fields, 2 records)
  - **FAQs** (9 fields, 3 records)
  - **Explanations** (7 fields, 2 records)
  - **ProcedureExecutions** (10 fields, 2 records)
  - **StepExecutions** (14 fields, 12 records)
  - **RequirementSatisfactions** (9 fields, 6 records)
  - **Errors** (6 fields, 2 records)
  - **IssueOccurrences** (10 fields, 2 records)
  - **UserQuestions** (10 fields, 2 records)
  - **UserFeedback** (9 fields, 2 records)
  - **StewardshipAssignments** (9 fields, 2 records)
  - **ChangeRequests** (13 fields, 2 records)
  - **ReviewEvents** (11 fields, 2 records)
  - **LearningActivities** (10 fields, 2 records)
  - **OperationalBindings** (13 fields, 5 records)
  - **CommunicationPolicies** (16 fields, 2 records)
  - **MessageTemplates** (9 fields, 2 records)
  - **SemanticMappings** (8 fields, 32 records)

Generated **48** table definitions with **273** raw fields (mode=check-add)
Generated **182** calculation functions
Generated **48** views
Enabled RLS on **48** tables
Generated insert statements for **307** records
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

