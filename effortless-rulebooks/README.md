# Effortless Rulebooks Hub

The canonical collection of example ontologies and domain-specific rulesets, each demonstrating the Effortless Rulebook (ERB) methodology.

## What is This?

This folder contains **multiple independent ERB projects**, each with a complete rulebook and README. Every ontology here:

- Is a **self-contained Effortless project** (has `effortless.json` + `CLAUDE.md`)
- Contains **one rulebook** (`effortless-rulebook.json`) as the single source of truth
- Can pull from **Airtable** (optional, base ID hardcoded in its local `effortless.json`)
- Can be edited **directly** (JSON or LLM-direct, no Airtable required)
- Generates **all substrates** (Postgres, Python, Go, Excel, OWL, etc.) from its rulebook
- Comes with **narrative documentation** (domain-specific README.md)

The rulebook is the **hub**. Airtable is one optional **spoke** per ontology.

## Structure

```
effortless-rulebooks/
├── README.md (this file)
│
├── star-trek/
│   ├── effortless.json          # Project config (hardcoded base ID for Airtable)
│   ├── CLAUDE.md                # Project instructions (ERB methodology)
│   ├── effortless-rulebook/
│   │   └── effortless-rulebook.json  # The rulebook — source of truth
│   ├── README.md                # Narrative: what this ontology demonstrates
│   └── execution-substrates/    # Generated: Postgres, Python, Go, etc.
│
├── jessica-advanced/
│   ├── effortless.json
│   ├── CLAUDE.md
│   ├── effortless-rulebook/
│   │   └── effortless-rulebook.json
│   ├── README.md
│   └── execution-substrates/
│
├── customer-fullname/
│   ├── (same structure)
│
└── [more ontologies...]
```

## Quick Start: Adding a New Ontology

To add a new domain/ontology to this hub:

1. **Create a new subfolder**: `mkdir effortless-rulebooks/<domain-name>`

2. **Initialize as an Effortless project**:
   ```bash
   cd effortless-rulebooks/<domain-name>
   effortless -init
   ```
   This creates `effortless.json`, `CLAUDE.md`, and the standard directory structure.

3. **Set up the rulebook source** (choose one):
   - **From Airtable**: Edit `effortless.json`, set the base ID under `airtabletorulebook` transpiler, then run `effortless airtabletorulebook`
   - **Directly**: Hand-edit `effortless-rulebook/effortless-rulebook.json` (start from a template or copy from an existing ontology)

4. **Write narrative documentation**: Create `README.md` with:
   - Brief description of what the ontology models
   - Key tables and relationships
   - Example use cases or demonstrations
   - Credit/source if adapted from elsewhere

5. **Build substrates**:
   ```bash
   effortless build
   ```

## Naming Conventions

- **Folder names**: kebab-case (e.g., `star-trek`, `customer-fullname`, `jessica-advanced`)
- **Rulebook tables**: PascalCase, plural (e.g., `Series`, `Episodes`, `Ratings`)
- **The `Name` field**: Always first, always a formula (the logical primary key per ERB convention)

## Examples Included

| Ontology | What It Demonstrates | Status |
|----------|----------------------|--------|
| `star-trek` | Hierarchical relationships, polymorphic FKs, multi-level aggregations | Complete |
| `jessica-advanced` | Complex workflow modeling, computed statuses, temporal reasoning | Complete |
| `jessica-basic` | Simple task tracking, basic formulas, aggregations | Complete |
| `customer-fullname` | Name derivation, string concatenation | Complete |
| `is-everything-a-language` | Linguistic ontology, self-referential models | Complete |

## Philosophy

Each ontology is a **proof of concept** for a specific domain pattern:

- Can the rules be expressed in the rulebook? (Always yes — CMCC holds)
- How do multiple substrates (Postgres, Python, Go, binary) all compute identically? (Conformance testing proves this)
- What does the rulebook alone reveal about the domain? (Narrative documentation)

The rulebook is the specification. Everything else — Postgres migrations, Python classes, Go structs, UI code — is **mechanically derived** from it.

## Editing Workflows

### If your ontology is Airtable-connected:

1. Edit schema/data in Airtable UI
2. Pull the rulebook: `effortless airtabletorulebook` (from the ontology folder)
3. Rebuild substrates: `effortless build`

### If your ontology is rulebook-direct (no Airtable):

1. Edit `effortless-rulebook/effortless-rulebook.json` directly (or via LLM)
2. Rebuild substrates: `effortless build`

### Reverse-sync (rulebook → Airtable):

If you've edited the rulebook directly and want to push changes back to Airtable:

```bash
effortless rulebooktoairtable
```

(This is optional; the rulebook is the SSoT regardless.)

## Testing & Conformance

Each ontology can run conformance tests to verify that all substrates compute identically:

```bash
cd effortless-rulebooks/<domain>/orchestration
bash orchestrate.sh  # or ./start.sh from project root
```

This generates answer keys from the rulebook seed data, injects into all substrates, and grades them.

---

**The rulebook is the hub. Everything else is mechanically derived.**
