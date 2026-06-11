# Talisman's Special Solutions — Effortless Project

<!-- rulebook-authoritative-banner -->
## Authoritative Source: `effortless-rulebook/talisman-basic-rulebook.json` is HEAD

For this project, **`effortless-rulebook/talisman-basic-rulebook.json` is the single, authoritative source of truth.** Edit it directly. All other artifacts (Postgres SQL, Python, Go, Excel, OWL, etc.) are mechanically derived from it via `effortless build`.

### Rulebook-first: no Airtable in this project

This is a **rulebook-first** project. There is no Airtable base, no `airtabletorulebook` transpiler, and no `rulebooktoairtable` reverse-sync. The rulebook JSON is the one and only source of truth. Edit it directly. There is nothing to pull from or push to — the file IS the spec.

**Rule for Claude / any agent operating in this project:** do not suggest, add, or invoke any Airtable transpilers or sync commands. This project does not use Airtable at any layer.
### Never silently revert `talisman-basic-rulebook.json`

**Treat `effortless-rulebook/talisman-basic-rulebook.json` as sacred.** It contains human-authored business rules and CANNOT be casually overwritten. Before running any command that could touch this file — `effortless build`, `effortless airtabletorulebook`, any transpiler that writes to it, any sync script, any `git checkout`/`git restore` against it — first run `git status` / `git diff` on it.

- If the file has uncommitted changes that you (the agent) did NOT just make this turn, **stop**. Ask the user before proceeding.
- It is only acceptable to let a regeneration touch this file when you are certain the *only* uncommitted edits in it are ones you just made in the current turn.
- This applies to every `**/effortless-rulebook/talisman-basic-rulebook.json` in this repo, not just the project-local one.

There is no upstream to "restore from." The JSON is the upstream.

<!-- /rulebook-authoritative-banner -->

This folder is a **self-contained Effortless Rulebook (ERB) project**. The rulebook is the single source of truth. All other artifacts (Postgres, Python, Go, substrates) are mechanically derived from it.

## All seed data is MOCK data. Every build resets it. That is the whole point.

There is **no "production vs. non-production" data** in this rulebook. *Everything* in the `data`
arrays is mock/demo data. The seed data is not "test data sitting next to real data" — it is the
authoritative content of the domain, and the dev database is a clean slate on every build.

- The dev Postgres bootstrap runs with `drop_all=true` (`mode=drop-all`, `stage=dev` — see
  `effortless.json` and `postgres-bootstrap/init-db.sh`). So **`init-db.sh` drops and recreates
  everything, then reloads exactly the rulebook seed data**, on every `effortless build`. Nothing
  you typed into the dev DB by hand survives a build — and that is intended.
- The **only** difference between this dev instance and a hypothetical production instance is the
  destructive reset: in production you would **add** rows (no drop); here you **drop and reload**.
  The same rulebook, the same mock rows, could legitimately exist in production as starter / training
  data. There is nothing "fake" about it that production would reject.
- **Corollary: no rulebook seed row is "sacred" in the way the *file* is.** The file (its rules and
  structure) is sacred — see the banner above; never silently revert it. But the *rows* exist to
  tell a story, and curating them (adding, removing, or rewriting mock rows to tell a better,
  complete, tip-to-tail story) is normal, expected work — not data loss. Do not preserve junk or
  filler rows out of misplaced caution. The "perfect" seed data is the data that tells the whole
  story: every class, every property, every state, exercised by real connected rows.

If you ever feel tempted to write a migration, an `ALTER TABLE`, or a "preserve existing data"
guard for the dev DB, stop: the dev DB is regenerated from the rulebook from scratch every build.
Edit the rulebook and rebuild.

## What This Demonstrates

This rulebook models **NTWF** — the workflow ontology from Jessica Talisman's *Intentional
Arrangement* series — as **one curated worked example**, the *Production Deployment Workflow*:

- **Role–agent separation (Heuristic 2)**: steps assign to Roles; Roles are filled by exactly one
  agent (human, AI, or pipeline). Personnel/model changes are one edge, not a rewrite.
- **Three disjoint agent types**: `HumanAgents`, `AIAgents`, `AutomatedPipelines` (mutually disjoint).
- **Approval gate as a step subtype**: `ApprovalGates` specializes a `WorkflowStep` via a 1:1 FK —
  a real subtype, not a collapsed node.
- **First-class step→step ordering**: `StepPrecedence` edges model the transitive `precedesStep`
  (4 asserted edges → 10-pair closure, including the never-asserted 1→5).
- **Delegation/escalation chain**: `Roles.DelegatesTo` (Release Manager → VP Engineering → CTO).
- **PROV provenance + DCAT datasets**: five artifacts in a `wasDerivedFrom` chain; one dataset.
- **SKOS controlled vocabularies**: workflow status + agent capability schemes.
- **Aggregations & boolean derivations**: `COUNTIFS` rollups, including conditional counts over
  *derived* child fields (e.g. counting steps where the calculated `IsExecutedByAI` is true).
- **Transitive closure (first-class)**: `ntwf:precedesStep` and `ntwf:delegatesTo` are materialized
  by the `rulebook-to-postgres` `closure` field type as `WITH RECURSIVE` views
  (`vw_step_precedence_closure`, `vw_roles_closure`) — asserted edges + inferred reachability, each
  row tagged `is_inferred` / `hop_distance`. This is what makes the article's headline inference fire.
- **Load-bearing lookups**: `INDEX/MATCH` lookups resolve the role→agent indirection and the
  gate→role→approver chain that the competency questions depend on. The project is **not** lookup-free
  (an earlier revision was); full article coverage made a small set of lookups load-bearing. It is
  **not** many-to-many anywhere; the model is a strict DAG (junction tables like `StepPrecedence`
  express what would otherwise be many-to-many).

## Rulebook

**Location:** `effortless-rulebook/talisman-basic-rulebook.json`

This is the canonical specification for the domain. It defines:
- Entity schemas (tables with fields)
- Field types (text, number, select, date, FK/reverse-FK, etc.)
- Calculated fields (formulas, lookups, aggregations)
- Seed data (test data for conformance)

## Editing

Edit `effortless-rulebook/talisman-basic-rulebook.json` directly. Then rebuild:

```bash
effortless build
```

## Building

```bash
effortless build
```

This runs all enabled transpilers and generates:
- `execution-substrates/postgres/`: SQL (views + base tables)
- `execution-substrates/python/`: Python dataclasses + computed fields
- `execution-substrates/golang/`: Go structs + business logic
- And 10+ other substrates (Excel, OWL, UML, etc.)

## Testing

From the project root:

```bash
./start.sh
# or
bash orchestration/orchestrate.sh
```

This:
1. Generates answer keys from the rulebook seed data
2. Injects the rulebook into all substrates
3. Grades each substrate against the answer key
4. Produces a conformance report

All substrates should produce identical results.

## Key Files

| File | Purpose |
|------|---------|
| `effortless.json` | Project config + transpiler settings |
| `CLAUDE.md` | This file |
| `effortless-rulebook/talisman-basic-rulebook.json` | The rulebook (SSoT) |
| `README.md` | Narrative: what this domain models |
| `execution-substrates/*/` | Generated: do not edit |

## Philosophy

The rulebook is a **declarative specification** of business logic. It is:

- **Implementation-agnostic**: express once, run everywhere (Postgres, Python, Go, etc.)
- **Temporally trackable**: the rulebook file is version-controlled; every schema change is a commit
- **Conformance-testable**: all substrates compute identically (verified by test suite)
- **Hub-and-spokes**: the rulebook is the hub; Postgres, Python, Go, etc. are spokes

**Do not hardcode logic in substrates.** If you're tempted to write imperative code in the generated files, the rule likely belongs in the rulebook as a Formula, Lookup, or Aggregation.

## Common Tasks

### Add a new entity (table)

Edit `talisman-basic-rulebook.json`: add a new top-level object with `schema` and `data` properties. Then `effortless build`.

### Add a calculated field

Edit the table's field list in `talisman-basic-rulebook.json`: add a field with `type: "formula"` or `"lookup"` or `"aggregation"`. Then `effortless build`.

### Change seed data

Edit the `data` section of a table in `talisman-basic-rulebook.json`. Then rebuild to regenerate answer keys and test files.

### Debug a formula

The rulebook uses **Excel-compatible formulas**: `IF()`, `AND()`, `CONCAT()`, `SUM()`, etc. Check the formula syntax against the Excel spec. Conformance tests will catch mismatches across substrates.

---

**The rulebook is the specification. Everything else is derived.**
