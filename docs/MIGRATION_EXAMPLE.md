# Migration Example: ACME Bases → effortless-rulebooks

This document shows how the old `bases/` pattern has been migrated to the new **rulebook-as-hub** pattern in `effortless-rulebooks/`.

## Before: Old Pattern

```
bases/
├── acme-corporation/
│   ├── README.md
│   └── effortless-rulebook.json
└── acme-llc/
    ├── README.md
    └── effortless-rulebook.json

(No project structure, no config, no orchestration at the ontology level)
```

**Problem:** Rulebooks were isolated artifacts. Airtable sync and base switching lived in the root orchestration layer. Each "base" was just a JSON file with narrative docs.

---

## After: New Pattern

```
effortless-rulebooks/
├── README.md  (Hub documentation & quick-start guide)
├── TEMPLATE-effortless.json
├── TEMPLATE-CLAUDE.md
│
├── acme-corporation/
│   ├── effortless.json              ← Airtable base ID hardcoded here
│   ├── CLAUDE.md                    ← Project instructions
│   ├── README.md                    ← Narrative about the domain
│   └── effortless-rulebook/
│       └── effortless-rulebook.json ← The rulebook (SSoT)
│
└── acme-llc/
    ├── effortless.json              ← Airtable base ID hardcoded here
    ├── CLAUDE.md                    ← Project instructions
    ├── README.md                    ← Narrative about the domain
    └── effortless-rulebook/
        └── effortless-rulebook.json ← The rulebook (SSoT)
```

**Benefits:**
- Each ontology is a **self-contained Effortless project**
- Airtable integration is **local to the project** (base ID in `effortless.json`)
- The rulebook is explicitly recognized as **the hub**
- Each project has its own orchestration (once we wire up `start.sh` per project)
- Narratives are co-located with the rulebook and config

---

## Key Files Created

### Hub-level

| File | Purpose |
|------|---------|
| `effortless-rulebooks/README.md` | Explains the hub pattern, directory structure, quick-start |
| `effortless-rulebooks/TEMPLATE-effortless.json` | Copy this to new ontologies; fill in base ID |
| `effortless-rulebooks/TEMPLATE-CLAUDE.md` | Copy this; customize for domain |

### Per-ontology (example: acme-corporation)

| File | Purpose |
|------|---------|
| `effortless.json` | Project config with hardcoded base ID: `appzkcmBFPWFGBtRo` |
| `CLAUDE.md` | Instructions for this project (how to edit, build, test) |
| `README.md` | Narrative: what this ontology models, key tables, formulas, use cases |
| `effortless-rulebook/effortless-rulebook.json` | The rulebook — SSoT |

---

## Workflow Changes

### Old Way (from root)

```bash
# Switch base in root orchestration
./start.sh  # menu: [B] to switch base
# Then [A] to run all substrates for CURRENT base
```

**Coupling:** Root orchestration manages multiple bases; hard to reason about which base is active.

### New Way (navigate to ontology)

```bash
cd effortless-rulebooks/acme-corporation/

# Option 1: Pull fresh from Airtable
effortless airtabletorulebook
effortless build

# Option 2: Edit rulebook directly
# (edit effortless-rulebook/effortless-rulebook.json)
effortless build

# Option 3: Test conformance (once substrates are in place)
./start.sh  # or bash orchestration/orchestrate.sh
```

**Decoupling:** Each ontology is independent. Base ID is hardcoded in the project's `effortless.json`. No central base registry.

---

## Migration Checklist

For each ontology, to complete the migration:

- [x] Create folder: `effortless-rulebooks/<domain-name>/`
- [x] Copy `effortless.json` template, fill in base ID
- [x] Copy `CLAUDE.md` template, customize instructions
- [x] Create/update `README.md` with domain narrative
- [x] Seed `effortless-rulebook/effortless-rulebook.json` from cache or Airtable
- [ ] (Future) Copy/link `orchestration/` into project folder (or keep root-level)
- [ ] (Future) Copy/link `execution-substrates/` into project folder (or keep root-level)
- [ ] (Future) Run `effortless build` to generate substrates
- [ ] (Future) Verify conformance tests pass

---

## Why This Matters

**The rulebook was always the hub.** It's only *rhetorically* described as downstream of Airtable. This pattern makes that fact **architecturally visible**:

1. **Each ontology is a project**, not just a data artifact
2. **Airtable is one optional spoke**, not the hub
3. **The rulebook is in the center** of each project's folder structure
4. **Dependencies flow in one direction**: Airtable → rulebook → all substrates
5. **No more central base registry** (`bases.json`); each project declares its source

---

## Next Steps

1. **Verify the pattern works**: Try building ACME Corporation substrates from its rulebook
2. **Migrate remaining bases**: star-trek, jessica-advanced, jessica-basic, customer-fullname, is-everything-a-language
3. **Refactor orchestration**: Move `start.sh` and `orchestration/` into each project (or keep centralized with symlinks)
4. **Update root-level docs**: Update CLAUDE.md, README.md to reflect new hub pattern
5. **Archive old `/bases/` folder** once all migrations are complete

---

**The rulebook is the hub. Airtable is a spoke.**
