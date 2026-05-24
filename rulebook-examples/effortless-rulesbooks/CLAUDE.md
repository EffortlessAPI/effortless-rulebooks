# Effortless Rulesbooks — Effortless Project

This folder is a **self-contained Effortless Rulebook (ERB) project**. The rulebook is the single source of truth. All other artifacts (Postgres, Python, Go, substrates) are mechanically derived from it.

## Rulebook

**Location:** `effortless-rulebook/effortless-rulebook.json`

This is a **meta-ontology**: the ERB project describing itself. Tables:

| Table | Description |
|-------|-------------|
| `ProjectMetadata` | Top-level project identity and architecture |
| `ExecutionSubstrates` | Runtime environments (Python, Go, Postgres, OWL, etc.) |
| `OrchestrationComponents` | Scripts that coordinate substrate generation and testing |
| `AirtableIntegration` | Tools that sync between Airtable and the rulebook |
| `TestingFramework` | Conformance test scripts and answer-key generators |
| `RulebookDomains` | The example ontologies included in the repo |
| `CoreDataFlows` | The key data pipelines (Airtable → rulebook → substrates → tests) |
| `ProjectConfiguration` | Config files (.env, effortless.json, docker-compose.yml, etc.) |
| `Dependencies` | External tools and libraries required to run the project |

## App

**Location:** `app/`

A React + Express Postgres table browser. Run with:

```bash
cd app && npm install && npm run dev
```

Serves a sidebar of tables/views and a grid of rows for whichever table is selected.

## Editing

### Option 1: Direct (no Airtable)

Edit `effortless-rulebook/effortless-rulebook.json` directly. Then rebuild:

```bash
effortless build
```

### Option 2: Reverse-sync (rulebook → Airtable)

If you've edited the rulebook directly and want to push back to Airtable:

```bash
effortless rulebooktoairtable
```

## Key Files

| File | Purpose |
|------|---------|
| `effortless.json` | Project config + transpiler settings |
| `CLAUDE.md` | This file |
| `effortless-rulebook/effortless-rulebook.json` | The rulebook (SSoT) |
| `app/` | React + Express Postgres table browser |
| `execution-substrates/*/` | Generated: do not edit |

---

**The rulebook is the specification. Everything else is derived.**
