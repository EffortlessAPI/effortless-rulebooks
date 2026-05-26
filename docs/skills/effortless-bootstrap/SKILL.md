<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-bootstrap/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-bootstrap
description: >
  Use when the user wants to bootstrap a new effortless project from raw text,
  requirements, or a description of a platform. Also known as the "Shadle steps"
  or "effortless-shadle-steps". Covers the full pipeline from raw input text
  through vocabulary extraction, glossary, narrative, mock data, schema
  normalization, and initial Airtable setup via OMNI.

  **Scope (load gate):** Effortless projects, OR when the user explicitly asks to bootstrap a new Effortless project from raw text/requirements.
audience: customer
---

# Effortless Bootstrap — The Shadle Steps

This skill describes the full bootstrap process for turning raw requirements or platform descriptions into a formal effortless rulebook connected to Airtable. This is how a new project goes from "here's what we're building" to "we're in the Leopold loop."

## User-facing documentation discipline

Throughout this bootstrap — especially in any README or narrative description generated for the client — **lead with what the system does, not how it was built.**

The rulebook, Airtable integration, calculated fields, and DAG structure are implementation details. They belong in CLAUDE.md and developer guides, not in user-facing materials. When generating glossaries, narratives, or mockups, focus on the business vocabulary and workflows, not the ERB infrastructure.

## Overview

The bootstrap is a structured pipeline that progressively formalizes raw input:

```
Raw Text Input
    |
    v
Word Extraction (every word used to describe the platform)
    |
    v
Domain Vocabulary (trimmed to domain-specific terms only)
    |
    v
Glossary (grouped by area, 1-2 sentences per term)
    |
    v
Narrative Description (uses every vocabulary word)
    |
    v
Mock Data & Scenarios (exercises every business rule)
    |
    v
Normalized Schema (formal structure for every element)
    |
    v
effortless-rulebook.json (DAG-structured, every vocab word appears)
    |
    v
OMNI Prompts (per-table, Name + compound key fields only)
    |
    v
Airtable Tables Created via OMNI
    |
    v
API: Add descriptions to every table and field
    |
    v
API: Populate with mock data
    |
    v
Extend model with 1st, 2nd, higher-order inferences (comprehensive DAG)
    |
    v
airtable-to-rulebook syncs the Airtable spoke into the rulebook hub (SSoT)
    |
    v
Now in the Leopold Loop
```

## Step-by-Step

### Step 1: Word Extraction

Split the raw input text by word so that **every word** used to describe the platform is listed. This is an exhaustive, uncurated list.

### Step 2: Domain Vocabulary

Trim the word list down to **just domain-specific words** — terms that are unique or meaningful within this particular platform. Remove generic English words, articles, prepositions, etc.

### Step 3: Glossary

Create a `glossary.md` with the domain vocabulary:
- **Grouped by area** (e.g., "User Management", "Billing", "Workflow")
- **1-2 sentences per vocabulary word** defining its meaning in the platform context
- This formally defines the **vocabulary for the entire platform**

### Step 4: Narrative Description

Using **every one of those vocabulary words**, generate a narrative description of the platform. This is a comprehensive prose description that exercises the full vocabulary and describes how the platform works end-to-end.

### Step 5: Mock Data and Scenarios

Using every vocabulary word and the features described in the narrative:
- Create **mock data** for each entity
- Create **scenarios** that exercise each business rule and requirement
- These scenarios validate that the schema can support every use case

### Step 6: Normalized Schema

Combine the glossary, narrative, and mock data to generate a **rough schema** for every element of the platform. This is a first-pass structural definition.

### Step 7: effortless-rulebook.json

Create an `effortless-rulebook.json` based on the normalized schema:
- Must follow all ERB conventions (PascalCase tables, Name formula, DAG structure, no many-to-many)
- **Every vocabulary word must appear in at least 1 place** within the rulebook (as a table name, field name, description, or data value)
- See `effortless-conventions` and `effortless-schema` skills for structural requirements

### Step 8: OMNI Prompts for Initial Tables

Generate OMNI prompts that, **per table**, create:
- The `Name` field — a formula producing a kebab-cased-human-readable-compound-pk: `SUBSTITUTE(LOWER({Label}), " ", "-")`
- The fields needed to construct that compound key from unique elements on the row
- **Only these initial structural fields** — not the full schema yet

### Step 9: Create Tables via OMNI

Use OMNI (via `omni-send.mjs`) to create the basic table infrastructure in Airtable, authenticated as the developer:

```bash
node ~/.claude/skills/effortless-airtable-omni/omni-send.mjs <baseId> '<per-table prompt>'
```

### Step 10: API — Add Descriptions

Use the Airtable REST API to add descriptions to **every table and field**:

```bash
# Add field description
curl -s -X PATCH "https://api.airtable.com/v0/meta/bases/{BASE_ID}/tables/{TABLE_ID}/fields/{FIELD_ID}" \
  -H "Authorization: Bearer {API_KEY}" \
  -H "Content-Type: application/json" \
  -d '{"description": "..."}'
```

### Step 11: API — Populate Mock Data

Use the Airtable REST API to populate every table with the mock data from Step 5.

### Step 12: Extend the Model

Extend the model with:
- **1st-order inferences** — direct lookups and calculations from raw fields
- **2nd-order inferences** — calculations that depend on 1st-order fields
- **Higher-order inferences** — progressively derived fields creating a comprehensive DAG

This builds out the full analytical power of the rulebook.

### Step 13: Sync the Airtable spoke into the hub

Run the airtable-to-rulebook transpiler to pull the now-fully-seeded Airtable base into the rulebook hub (the SSoT). This establishes Airtable as a *connected input spoke*, not as the SSoT itself — `effortless-rulebook.json` is the SSoT, and Airtable is one editing surface for it.

```bash
cd effortless-rulebook/
effortless airtable-to-rulebook -account airtable -o effortless-rulebook.json
```

At this point, we are fully in the **Leopold loop** — future changes can flow in via Airtable, via direct edits to the rulebook JSON (LLM-direct), or via reverse-sync, followed by `effortless build`.

## CLI Tool for Bootstrap

The `raw-text-to-rulebook` transpiler can generate a rough (not guaranteed internally consistent) starting-point rulebook:

```bash
cd bootstrap/
effortless -install raw-text-to-rulebook -i requirements.txt -o bootstrap-rulebook.json
effortless build
```

This output is a **starting point** — it needs to be reviewed, normalized, and extended through the Shadle steps above before it becomes a production rulebook.

## Artifacts Produced

| Artifact | Location | Purpose |
|----------|----------|---------|
| Word list | `bootstrap/words.txt` | Every word from the raw input |
| Domain vocabulary | `bootstrap/vocabulary.txt` | Domain-specific terms only |
| Glossary | `bootstrap/glossary.md` | Formal definitions grouped by area |
| Narrative | `bootstrap/narrative.md` | Full prose description using all vocab |
| Mock data | `bootstrap/mock-data/` | Test data and scenarios |
| Bootstrap rulebook | `bootstrap/bootstrap-rulebook.json` | Rough first-pass rulebook |
| Final rulebook | `effortless-rulebook/effortless-rulebook.json` | Production rulebook (after Airtable sync) |

## See also

- `effortless-cli` — the `-install` and build commands used throughout.
- `effortless-conventions` — naming and DAG rules the rulebook must follow.
- `effortless-schema` — JSON structure the rulebook must conform to.
- `effortless-airtable-omni` — OMNI automation for creating tables (Step 9).
- `effortless-airtable` — API calls for descriptions and data (Steps 10–11).
- `effortless-leopold-loop` — what you enter at Step 13 once the rulebook hub is seeded and synced.
- `effortless-setup-postgres` — for projects that target Postgres, run after Step 13.

## LOCALHOST MODE — read before doing anything

Before any magic-links / bases work, check whether `MAGICLINK_BASE_URL` is set to a `http://localhost:*` URL (or the operator said "use the local dev stack" / "localhost"). If so, follow [MAGIC_LINKS_REFACTOR.md §13](../../MAGIC_LINKS_REFACTOR.md#13-localhost-mode--opt-in-via-env-vars) — production URLs become localhost URLs, the magic-link email loop is replaced by debug code `424242`, and bases registration goes against the local bases server.

Operator quick-ref in localhost mode:
- magiclink server: `http://localhost:4787` (admin UI at `/`)
- bases server:     `http://localhost:4788` (admin UI at `/`)
- unified dash:     `http://localhost:4789`
- env file:         `magic-links-refactor/test-env/dev/.env` — `source` it before any curl recipe; gives you `OWNER_JWT`, `MAGICLINK_BASE_URL`, `BASES_BASE_URL`, etc.

Up-check (run before assuming the stack is live):
```
curl -fsS http://localhost:4787/install-magic-links/v1.sql >/dev/null && echo "magiclink up" || echo "magiclink DOWN"
curl -fsS http://localhost:4788/health >/dev/null && echo "bases up" || echo "bases DOWN"
```
If down, run `bash magic-links-refactor/test-env/scripts/dev-stack-up.sh`.
