# Build-All-Rulebooks Plan

**Goal:** Every rulebook under `rulebook-examples/` must `effortless build` cleanly. That is the conformance litmus test for these ontologies.

## Classification rule (mechanical)

For each project, read its `effortless.json`:

- **Airtable-first**: `airtabletorulebook` is registered AND `IsDisabled: false` AND `Enabled: true`. Airtable is the SSoT. The project's CLAUDE.md must say so. `effortless build` pulls from Airtable, overwriting the local JSON, before running downstream transpilers.
- **Rulebook-first**: everything else. The local JSON is the SSoT. CLAUDE.md says "JSON is authoritative" (the default ERB banner). `effortless build` runs downstream transpilers against the committed JSON. If `airtabletorulebook` is registered-but-disabled, leave it that way.
- **Not a rulebook project**: no `effortless.json`, or an `effortless.json` with no rulebook output. Either fix or delete; do not try to build.

## Fix discipline

1. First try: fix the rulebook JSON / fix the Airtable schema until generated SQL builds cleanly.
2. **Last resort only:** override a function in `02b-customize-functions.sql` (or `03b-customize-views.sql` for view conflicts, etc.). This should be rare — flag it explicitly per project when used.

## Side-effect awareness

`effortless build` runs the whole transpiler chain in order. If `rulebooktopostgres` + `execute ./init-db.sh` are registered, every build will (re)create a Postgres DB. Per-domain DB names follow `erb_<domain_underscored>` (see global `CLAUDE.md`). Plan accordingly — a "build" is also a DB rebuild.

---

## Status legend

- ✅ Builds cleanly end-to-end
- 🟡 Builds the rulebook (airtable pull or JSON-only) but downstream transpilers fail
- ❌ Does not build at all (missing `effortless.json`, missing rulebook, etc.)
- ❓ Unknown — not yet attempted this session
- ➖ N/A (not a rulebook project — umbrella / placeholder / spec-only)

---

## Inventory (28 entries under `rulebook-examples/`)

### Group 1 — Airtable-first (4)

`airtabletorulebook` registered, `IsDisabled: false`, `Enabled: true`. Airtable is the SSoT for these. Building WILL overwrite the local JSON.

| # | Project | Base ID | Status | Notes |
|---|---|---|---|---|
| 1 | `v1-nakedclaude-demo` | `appgjoEcFNxluhbvK` | 🟡 | Airtable pull works, schema regen succeeds, but `04b-customize-policies.sql:31` and `:36` fail with `ERROR: column "owner_email" does not exist`. RLS-policy customization references a column the current Airtable schema doesn't have. Fix requires understanding the intent of that RLS policy — either restore `owner_email` to the rulebook or update/remove the policy. CLAUDE.md banner: flipped to Airtable-first this session. |
| 2 | `v2-nakedclaude-demo` | `app7G5emeY7miM4WN` | ❓ | Was working before. CLAUDE.md banner: needs flip to Airtable-first. |
| 3 | `v3-nakedclaude-demo` | `appKLygCIXweUUKtM` | ❓ | New this session — never built. CLAUDE.md banner: needs flip to Airtable-first. |
| 4 | `v4-nakedclaude-demo` | `appeUOAaOIdoqPSx3` | ❓ | Renamed from old mislabeled "v3" this session. CLAUDE.md banner: needs flip to Airtable-first. |

### Group 2 — Rulebook-first (19)

Local JSON is the SSoT. Default behavior. Build downstream transpilers against the committed JSON. Some have `airtabletorulebook` registered-but-disabled — leave it disabled; do not flip without explicit per-turn user consent.

**Registered-but-disabled (8)** — has a `baseId`, Airtable pull available but not active. JSON is HEAD.

| # | Project | Base ID | Status | Notes |
|---|---|---|---|---|
| 5 | `acme-llc` | `appWrXPvXbkgQGOxt` | ✅ | Clean build, exit 0. No init-db step → no DB side effect. |
| 6 | `acme-corporation` | `appzkcmBFPWFGBtRo` | ✅ | Clean build, exit 0. Only `rulebooktopostgres` runs (generates SQL files, no init-db execution). |
| 7 | `is-everything-a-language` | `appC8XTj95lubn6hz` | ✅ | Clean build, exit 0. Generates postgres dir on build. |
| 8 | `jessica-advanced` | `appwN9EAp8IeIxM23` | ✅ | Clean build, exit 0. Generates postgres dir on build. |
| 9 | `jessica-basic` | `applThn0rikpCR9C3` | ✅ | Clean build, exit 0. Note: base ID still not in `orchestration/bases.json`. |
| 10 | `star-trek` | `appqwWQxIWFtyDsiL` | ✅ | Clean build, exit 0. Postgres SQL + xlsx generated. |
| 11 | `effortless-rulesbooks` | _(empty)_ | ✅ | Clean build, exit 0. SQL generated. |
| 12 | `guessing-game` | `appXXXXXXXXXXXXXX` | ✅ | Clean build, exit 0. Placeholder baseId; Airtable transpilers stay disabled. |

**Not registered (12)** — no Airtable wiring at all. Pure JSON projects.

| # | Project | Has postgres? | Status | Notes |
|---|---|---|---|---|
| 13 | `customer-fullname` | Y | ✅ | Clean build, exit 0. Reclassified to ERB-only this turn — no Airtable wiring at all. Postgres SQL + xlsx generated. |
| 14 | `community-event-planner-demo` | Y | ✅ | Clean build, exit 0. DB `erb_community_event_planner_demo` (re)created, 12 tables. Full app project (server+web+walkthrough). |
| 15 | `customer-crm-demo` | Y | ✅ | Clean build, exit 0. DB `erb_customer_crm_demo` (re)created via injected DROP+CREATE in init-db.sh. |
| 16 | `effortless-banking-demo` | Y | 🟡 | DB `erb_effortless_banking_demo` (re)created with 20 tables. `rulebook-to-react` transpiler in `effortless.json` is misconfigured (no `-i` arg), produces "No rulebook JSON provided" error.txt. Exit 0 but React output empty. |
| 17 | `fantasy-football-demo` | Y | ✅ | Clean build, exit 0. DB `erb_fantasy_football_demo` (re)created, 12 tables. React explainer-dag generated. |
| 18 | `gym-trainer-invoicing` | Y | ✅ | Clean build, exit 0. DB `erb_gym_trainer_invoicing` (re)created, 10 tables. |
| 19 | `intelligence-taxonomy-demo` | Y | ✅ | Clean build, exit 0. DB `erb_intelligence_taxonomy_demo` (re)created, 6 tables. Added missing `execute -exec ./init-db.sh` step to `effortless.json`. |
| 20 | `jobsearch-rag` | Y | ✅ | Clean build, exit 0. DB `erb_jobsearch_rag` (re)created, 20 tables. Fixed: transpiler `-i` paths (missing `effortless-rulebook/` prefix), added missing `execute` step, chmod +x on init-db.sh. |
| 21 | `llm-enigma-test` | Y | ✅ | Clean build, exit 0. DB `erb_llm_enigma_test` (re)created, 52 tables. Fixed: added missing `execute` step, re-injected DROP+CREATE in init-db.sh (had been clobbered by rulebook-to-postgres regen). |
| 22 | `product-inventory-demo` | Y | ✅ | Clean build, exit 0. DB `erb_product_inventory_demo` (re)created, 6 tables. |
| 23 | `therapist-helper-portal` | Y | ✅ | Clean build, exit 0. DB `erb_therapist_helper_portal` (re)created, 10 tables. |
| 24 | `wedding-seating-optimizer` | Y | ✅ | Clean build, exit 0. DB `erb_wedding_seating_optimizer` (re)created, 8 tables. |

(Counts: 8 registered-but-disabled + 12 not-registered = 20 in Group 2. Inventory totals: 4 Airtable-first + 20 rulebook-first + 4 broken = 28.)

### Group 3 — Broken / not a rulebook project (4)

| # | Project | Problem | Status | Decision needed |
|---|---|---|---|---|
| 25 | `expense-approval-demo` | No `effortless.json`, no rulebook. Only README. | ❌ | Scaffold from template, or delete. |
| 26 | `volunteer-shift-scheduler-demo` | No `effortless.json`, no rulebook. Has `.xlsx`, `schema/`, `server/`, `web/`. | ❌ | Generate rulebook from spec, or delete. |
| 27 | `naked-claude-vs-effortless-claude` | Comparison harness, not an ontology. Has `effortless.json` with `airtabletorulebook` active pointing at v1's base. | ➖ | Disable `airtabletorulebook` here (mechanical-classifier false positive) or remove the `effortless.json` entirely. |
| 28 | `v3-nakedclaude-demo-naked` | Empty dir (only `.DS_Store`) — leftover from this session's cleanup. | ➖ | Delete. |

---

## Per-project recipe

For each project in Group 2 (rulebook-first) and Group 1 (Airtable-first), in order:

### 0. Pre-flight
```bash
cd rulebook-examples/<project>/
git status .           # rulebook JSON must not have uncommitted edits not made this turn
```

### 1. Audit `effortless.json`
- `Name` matches dir name.
- `SSoTmeProjectId` is unique.
- `CommandLine` strings reference the correct `<project>-rulebook.json` filename.
- Classification matches actual state:
  - Group 1 (Airtable-first): `airtabletorulebook` entry exists, `IsDisabled: false`, `Enabled: true`.
  - Group 2 (Rulebook-first): if `airtabletorulebook` is present, it must be `IsDisabled: true` and `Enabled: false`. Do not flip without per-turn user consent.

### 2. Audit `CLAUDE.md`
- Project name and rulebook filename references are correct.
- "Authoritative Source" line matches the classification:
  - Group 1: "Airtable base `<baseId>` is the authoritative source. `effortless build` pulls from Airtable and overwrites the local JSON."
  - Group 2: "Local rulebook JSON is the authoritative source. Airtable pull is disabled by default."

### 3. Run `effortless build`
- Group 1: Airtable pull runs first, overwriting the local JSON; then downstream transpilers.
- Group 2: just runs downstream transpilers against the local JSON.
- **Watch for postgres side effects** — every build with a `rulebooktopostgres` + `execute ./init-db.sh` chain will (re)create the per-domain Postgres DB.

### 4. Fix any errors
- **Schema-level errors** (table/column conflicts, FK targets missing) → fix the rulebook JSON or Airtable schema. Rebuild.
- **View regeneration errors** (`cannot drop columns from view`) → typically means a previous run left an incompatible view. First try `DROP DATABASE erb_<domain>` and re-build clean. If the error persists from a clean DB, the rulebook itself has a structural problem — fix it.
- **Function errors** → fix the rulebook formula. Only as last resort, override the function body in `02b-customize-functions.sql`.

### 5. Mark status in this doc
Update the status column (`✅`, `🟡`, `❌`) with a one-line note on what was fixed.

### 6. Commit per-project
One commit per project, scoped to that project's directory + this doc's status update. Makes failure recovery surgical.

---

## Execution order

Rulebook-first projects first (no upstream-overwrite risk), Airtable-first last:

1. **Group 2 — Registered-but-disabled (8):** `acme-llc`, `acme-corporation`, `jessica-basic`, `jessica-advanced`, `star-trek`, `is-everything-a-language`, `effortless-rulesbooks`, `guessing-game`
2. **Group 2 — Not registered (12):** `customer-fullname`, `community-event-planner-demo`, `customer-crm-demo`, `effortless-banking-demo`, `fantasy-football-demo`, `gym-trainer-invoicing`, `intelligence-taxonomy-demo`, `jobsearch-rag`, `llm-enigma-test`, `product-inventory-demo`, `therapist-helper-portal`, `wedding-seating-optimizer`
3. **Group 1 — Airtable-first (4):** `v1-nakedclaude-demo`, `v2-nakedclaude-demo`, `v3-nakedclaude-demo`, `v4-nakedclaude-demo`
4. **Group 3 — decisions:** `expense-approval-demo`, `volunteer-shift-scheduler-demo`, `naked-claude-vs-effortless-claude`, `v3-nakedclaude-demo-naked`

---

## Session boundaries

Each session should pick a contiguous slice and:
- Pull this doc up first to see what's `❓` vs done.
- Work projects in the order above.
- Stop at the end of a project (don't leave a project half-built).
- Update statuses in this doc as the last act of the session.
- Commit doc updates separately from project fixes so the doc reads as a clean ledger.

**Hard rule for any session:** never run a bulk `effortless build` across all projects at once. Per-project, sequential, with status review between each. The whole point is to catch and fix issues — batching defeats that.

---

## Open questions to resolve

1. `jessica-basic`'s base ID (`applThn0rikpCR9C3`) isn't in `bases.json` — add it.
2. `naked-claude-vs-effortless-claude/effortless.json` has `airtabletorulebook` enabled but is the harness, not a rulebook. Disable the entry or remove the whole `effortless.json`.
