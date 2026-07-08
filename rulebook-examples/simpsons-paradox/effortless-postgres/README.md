# Rulebook to PostgreSQL Script Generation Report

**Schema:** `public`
**Database:** `demo`
**Timestamp:** 2026-07-08 06:25:20 UTC

## Parsing Rulebook

Found **32** tables in rulebook


  - **Loops** (16 fields, 76 records)
  - **Studies** (26 fields, 238 records)
  - **Treatments** (8 fields, 464 records)
  - **Strata** (6 fields, 497 records)
  - **CaseCells** (11 fields, 994 records)
  - **StratumSummaries** (28 fields, 994 records)
  - **ModelSummary** (91 fields, 1 records)
  - **StratumVariables** (10 fields, 238 records)
  - **TreatmentRankings** (54 fields, 238 records)
  - **InvariantChecks** (21 fields, 29 records)
  - **Methodology** (14 fields, 10 records)
  - **Conclusions** (22 fields, 33 records)
  - **UIScreens** (14 fields, 10 records)
  - **UIComponents** (8 fields, 11 records)
  - **InstrumentSpec** (9 fields, 24 records)
  - **AllocationSweep** (19 fields, 2380 records)
  - **SweepStudySummary** (13 fields, 238 records)
  - **ResearchTraditions** (18 fields, 6 records)
  - **Researchers** (19 fields, 18 records)
  - **SyntheticPhase** (24 fields, 240 records)
  - **PhaseDiagramSummary** (11 fields, 1 records)
  - **IngestionProtocol** (8 fields, 17 records)
  - **IngestionSummary** (14 fields, 1 records)
  - **CandidateStudyCatalog** (27 fields, 237 records)
  - **CorpusCatalogSummary** (20 fields, 1 records)
  - **DomainExpansionTargets** (10 fields, 11 records)
  - **StudyImportTemplate** (7 fields, 8 records)
  - **SweepStudyConfig** (13 fields, 238 records)
  - **SubstrateConformanceFields** (9 fields, 26 records)
  - **DiscoveryHypotheses** (7 fields, 15 records)
  - **DiscoveryFindings** (8 fields, 15 records)
  - **CorpusDomains** (6 fields, 11 records)

Generated **32** table definitions with **226** raw fields (mode=check-add)
Generated **282** calculation functions
Generated **32** views
Enabled RLS on **32** tables
Generated insert statements for **7320** records
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

