# Rulebook Hub Promotion

Promote `effortless-rulebook.json` from a disposable IR to the true HEAD/hub.
Airtable becomes one of N optional input spokes. The runtime reorganizes around
an `effortless-rulebooks/` folder with one subfolder per domain.

---

## Todo List

- [ ] **Audit current start.sh** — Map every assumption that Airtable is HEAD so we know exactly what breaks.
  - `orchestration/orchestrate.sh`: Lines 131-152 (SUBSTRATE_ORDER array hardcodes "airtable" as first substrate)
  - `orchestration/orchestrate.sh`: Lines 252, 285 (menu displays Airtable link from effortless.json base ID)
  - `orchestration/orchestrate.sh`: Lines 297-387 (action_pull_airtable() with 3-step flow: rulebook-cache.py sync → answer keys → inject all)
  - `orchestration/orchestrate.sh`: Lines 389-567 (action_change_base_id() manages Airtable base swapping via base-manager.py)
  - `orchestration/orchestrate.sh`: Lines 770-798 (STEP 0: airtable inject run BEFORE answer keys, assume it's rulebook source)
  - `effortless.json`: airtabletorulebook transpiler with -account airtable flag (line 1 of transpiler config)
  - `effortless.json`: rulebooktoairtable transpiler (reverse sync, optional but available)
  - `orchestration/rulebook-cache.py`: Pull from Airtable via API (lines 192-260); has offline fallback
  - `orchestration/base-manager.py`: Add/list/select Airtable bases; fetches base name from API (lines 38-327)
  - `execution-substrates/airtable/`: Full substrate for Airtable oracle (inject-into-airtable.py, take-test.sh, create-substrate-report.sh)
  - `devops/pull.sh`: Explicitly pulls from Airtable via effortless airtabletorulebook (lines 27-29)
  - `devops/rebuild-on-trigger.sh`: Complex webhook orchestrator that syncs Airtable before rebuild (multiple Airtable API calls)
  - `run-in-docker.sh`: Checks for AIRTABLE_API_KEY env var, validates connectivity (lines 78-192)
  - `cleanup-airtable.py`: Directly manipulates Airtable via REST API for Jessica ADVANCED base (lines 3-518)
  - `populate-acme-corp.py`: Seed script for populating Airtable with sample data (lines 3-100)
  - `orchestration/test-orchestrator.py`: Line 388 references "airtable" as the oracle baseline
  - `README.md` & `README.TECHNICAL.md`: Marketing/docs frame Airtable as "the hub" and rulebook as "disposable IR"
  - `CLAUDE.md`: Project instructions state "Three tools form hub-and-spokes around Airtable"
- [x] **Define effortless-rulebooks layout** — Nail the folder contract: what's required vs derived in each domain subfolder.
  - Created root `effortless-rulebooks/README.md` explaining the hub-and-spokes pattern
  - Each ontology is a self-contained Effortless project: `effortless.json` + `CLAUDE.md` + `effortless-rulebook/` + `README.md`
  - Base ID hardcoded in each project's `effortless.json` (no more central `bases.json`)
  - Templates provided: `TEMPLATE-effortless.json` + `TEMPLATE-CLAUDE.md`
- [x] **Identify extractable sidecar artifacts** — Decide which 1-2 artifacts (README? DAG diagram?) auto-generate from the rulebook.
  - **README.md (narrative)**: per-ontology, hand-written, explains what the domain models, key entities, example use cases
  - **Auto-generated candidates**: DAG diagram (from calculated-field dependencies), schema snapshot (JSON schema export)
- [ ] **Refactor runtime folder structure** — Restructure start.sh + inject.py to drive from `effortless-rulebooks/<domain>/` as root.
- [ ] **Demote Airtable to spoke** — Update effortless.json / pipeline so Airtable is one optional input spoke, not required SSoT.
- [ ] **Update conformance tests** — Ensure take-test.py and test-cases run cleanly against the new folder layout.

---

## Key Finding

**The clean pipe is already there.** One hop (airtable → rulebook-cache.py sync) fills `/effortless-rulebook/effortless-rulebook.json`, then 100% of the downstream (answer keys, inject, grading, test orchestration) is already rulebook-agnostic. The rulebook IS the hub; it's only *rhetorically* described as downstream of Airtable.

**The work is: decoupling source (Airtable/LLM/direct JSON edits) from consumption (everything else).**

## Implementation Progress

### Completed

1. **Hub structure & documentation**
   - Created `effortless-rulebooks/README.md` explaining the pattern
   - Each ontology is a self-contained project with its own `effortless.json`, `CLAUDE.md`, `README.md`
   - Templates provided (`TEMPLATE-effortless.json`, `TEMPLATE-CLAUDE.md`)

2. **ACME Corporation** (`effortless-rulebooks/acme-corporation/`)
   - ✅ `effortless.json` with hardcoded base ID: `appzkcmBFPWFGBtRo`
   - ✅ `CLAUDE.md` (project instructions)
   - ✅ `README.md` (narrative: business process modeling, inventory, aggregations, status derivation)
   - ✅ `effortless-rulebook/effortless-rulebook.json` (seeded from cache)

3. **ACME, LLC** (`effortless-rulebooks/acme-llc/`)
   - ✅ `effortless.json` with hardcoded base ID: `appWrXPvXbkgQGOxt`
   - ✅ `CLAUDE.md` (project instructions)
   - ✅ `README.md` (narrative: lean variant, simplified workflow)
   - ✅ `effortless-rulebook/effortless-rulebook.json` (seeded from cache)

### Ready to Test

- Navigate to `effortless-rulebooks/acme-corporation/`
- Run `effortless build` to generate all substrates from the rulebook
- Or run `effortless airtabletorulebook` to sync fresh from Airtable
- Run `./start.sh` (once substrate structure in place) to test conformance

## Test Plan: Verify It Works

1. **Navigate to an ontology**: `cd effortless-rulebooks/acme-corporation/`
2. **Inspect the structure**: Should have `effortless.json`, `CLAUDE.md`, `README.md`, `effortless-rulebook/`
3. **Pull fresh rulebook**: `effortless airtabletorulebook` (requires `AIRTABLE_API_KEY`)
4. **Build substrates**: `effortless build`
5. **Run conformance tests**: `./start.sh` (once orchestration is wired up)

See **MIGRATION_EXAMPLE.md** for detailed before/after comparison.

## Open Questions

- Should old `/bases/` folder be archived or deleted once migration is complete?
- Should orchestration tools (orchestrate.sh, start.sh) be moved into each ontology or kept at repo root?
- Auto-generated artifacts: DAG diagram, schema snapshot — where should these live?

---

## Notes

*(Add context, decisions, and discoveries here as work progresses.)*
