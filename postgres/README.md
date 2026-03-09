# PostgreSQL - Most Reliable Mirror

PostgreSQL serves as the **default answer-key substrate** for the ERB system. It is not privileged or canonical—it's simply the substrate used by default to generate the `answer-key.json` that other substrates are tested against.

## Why Postgres is the Default (Not Master)

PostgreSQL is **not** a source of truth—the `effortless-rulebook.json` is. Postgres is one of many mirrors that can reflect the rulebook's semantics. It happens to be:

- **Well-tested**: Mature SQL semantics with predictable behavior
- **Deterministic**: No floating-point drift, consistent ordering
- **Convenient**: Easy to query during development

But any substrate achieving 100% conformance could serve as the answer-key generator. Postgres has no philosophical privilege—only practical convenience.

## Role in Three-Phase Contract

PostgreSQL generates the answer key by default, but this is a pragmatic choice, not an architectural constraint.

### Phase 1: Inject

Generate SQL DDL + calc functions + views + seed data from the rulebook:

| Artifact | Purpose |
|----------|---------|
| `01-drop-and-create-tables.sql` | Base tables with raw fields |
| `02-create-functions.sql` | `calc_*` functions implementing the formula DAG |
| `03-create-views.sql` | `vw_*` views with all computed fields |
| `04-create-policies.sql` | Row Level Security policies |
| `05-insert-data.sql` | Seed data from rulebook |
| `init-db.sh` | Database initialization script |

### Phase 2: Execute

Compute all derived fields via the canonical view (the "all computed fields included" view):

```bash
./init-db.sh  # Initialize database and compute via views
```

### Phase 3: Emit (Generate Test Artifacts)

Export the computed results as the test baseline:

| Output | Purpose |
|--------|---------|
| `answer-key.json` | Complete view output — the reference for conformance testing |
| `blank-test.json` | Same rows but computed fields nulled — test input |

These files go into `testing/` and are used by all other substrates.

### Grade

Other substrates compare their results to what Postgres computed. This makes Postgres the *default reference*, not the *source of truth*. Any 100%-conformant substrate could theoretically generate the answer key.

## Technology

**PostgreSQL** is an advanced open-source relational database with powerful SQL features. Its function system allows complex calculations to be expressed declaratively.

Key characteristics:
- **SQL functions**: `CREATE FUNCTION calc_*` implements each derived field
- **Views**: Pre-computed joins that materialize the full entity with all calculated fields
- **Referential integrity**: Foreign keys ensure data consistency
- **Row Level Security**: Fine-grained access control

## Files

| File | Description |
|------|-------------|
| `01-drop-and-create-tables.sql` | Drop and recreate tables with raw fields |
| `02-create-functions.sql` | Create calculation functions |
| `03-create-views.sql` | Create views with calculated fields |
| `04-create-policies.sql` | Create RLS policies |
| `05-insert-data.sql` | Insert data from rulebook |
| `init-db.sh` | Database initialization script |

## Usage

```bash
# Initialize the database
./init-db.sh

# Query the computed view
psql -d demo -c "SELECT workflow_id, name, display_name, has_more_than_1_step FROM vw_workflows"
```

## DAG Execution Order

```
Level 0: Raw fields (from tables)
Level 1: name (slug), count_of_steps (aggregation)
Level 2: has_more_than_1_step (depends on count_of_steps)
```

## Generation Report

**Schema:** `public`
**Database:** `demo`

Found multiple tables in rulebook:
- **Workflows** (workflow fields + calculated)
- **WorkflowSteps** (step fields + calculated)
- **Roles** (role fields + calculated)

Generated:
- Table definitions with raw fields
- Calculation functions (calc_*)
- Views (vw_*)
- RLS policies
- Insert statements for seed data

## Source

Generated from: `effortless-rulebook/effortless-rulebook.json`
