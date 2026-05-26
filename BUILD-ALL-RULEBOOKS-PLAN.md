# Build-All-Rulebooks Plan

**Goal:** Every rulebook under `rulebook-examples/` must `effortless build` cleanly. That is the conformance litmus test for these ontologies.

**Authorship rule:**
- If a rulebook has a real Airtable base (per `orchestration/bases.json`), Airtable is the SSoT. `airtabletorulebook` must be a registered, enabled tool, and the pulled JSON must build.
- If a rulebook has no Airtable base (or only a placeholder like `appXXXXXXXXXXXXXX`), the JSON itself is the SSoT. No `airtabletorulebook` entry. Build straight from the local JSON.

**Fix discipline:**
1. First try: fix the rulebook JSON / fix the Airtable schema until generated SQL builds cleanly.
2. **Last resort only:** override a function in `02b-customize-functions.sql` (or `03b-customize-views.sql` for view conflicts, etc.). This should be rare — flag it explicitly per project when used.

**Side-effect awareness:** `effortless build` runs the whole transpiler chain in order. If `rulebooktopostgres` + `execute ./init-db.sh` are registered, every build will (re)create a Postgres DB. Per-domain DB names follow `erb_<domain_underscored>` (see global `CLAUDE.md`). Plan accordingly — a "build" is also a DB rebuild.

---

## Status legend

- ✅ Builds cleanly end-to-end
- 🟡 Builds the rulebook (airtable pull or JSON-only) but downstream transpilers fail
- ❌ Does not build at all (missing `effortless.json`, missing rulebook, etc.)
- ❓ Unknown — not yet attempted this session
- ➖ N/A (not a rulebook project — umbrella / placeholder / spec-only)

---

## Inventory (28 entries under `rulebook-examples/`)

### Group A — Airtable-backed (real base IDs from `orchestration/bases.json`)

These must have `airtabletorulebook` registered and enabled. The Airtable base is the SSoT; the local rulebook JSON is the IR.

| # | Project | Base ID | Status | Notes |
|---|---|---|---|---|
| 1 | `acme-llc` | `appWrXPvXbkgQGOxt` | ❓ | Default base. Postgres present. The reference simplest case. |
| 2 | `acme-corporation` | `appzkcmBFPWFGBtRo` | ❓ | Extended ACME. No postgres dir. |
| 3 | `customer-fullname` | `appWrXPvXbkgQGOxt` | ❓ | **Shares ACME LLC's base** — verify this is intentional, not a copy/paste leftover. |
| 4 | `is-everything-a-language` | `appC8XTj95lubn6hz` | ❓ | Meta-ontology. No postgres dir. |
| 5 | `jessica-advanced` | `appwN9EAp8IeIxM23` | ❓ | No postgres dir. |
| 6 | `jessica-basic` | `applThn0rikpCR9C3` | ❓ | **Base ID is NOT in `bases.json`** — either add it there or replace with the right ID. |
| 7 | `star-trek` | `appqwWQxIWFtyDsiL` | ❓ | No postgres dir. |
| 8 | `v1-nakedclaude-demo` | `appgjoEcFNxluhbvK` | 🟡 | Airtable pull succeeded this session. `effortless build` fails at `03-create-views.sql:20: cannot drop columns from view`. |
| 9 | `v2-nakedclaude-demo` | `app7G5emeY7miM4WN` | ❓ | Not yet rebuilt this session. Was working before. |
| 10 | `v3-nakedclaude-demo` | `appKLygCIXweUUKtM` | ❓ | New this session — never built. |
| 11 | `v4-nakedclaude-demo` | `appeUOAaOIdoqPSx3` | ❓ | Renamed from old mislabeled "v3" this session. |

### Group B — Airtable-backed with placeholder ID

| # | Project | Base ID | Status | Notes |
|---|---|---|---|---|
| 12 | `guessing-game` | `appXXXXXXXXXXXXXX` | ❓ | **Placeholder base ID.** Either reclassify as Group C (JSON-only) and remove the `airtabletorulebook` entry, or replace with a real Airtable base. |

### Group C — JSON-only rulebooks (no Airtable SSoT)

These should have NO `airtabletorulebook` entry. The local JSON is the source of truth. They must still build downstream (postgres, etc.) cleanly.

| # | Project | Has postgres? | Status | Notes |
|---|---|---|---|---|
| 13 | `community-event-planner-demo` | Y | ❓ | |
| 14 | `customer-crm-demo` | Y | ❓ | |
| 15 | `effortless-banking-demo` | Y | ❓ | |
| 16 | `effortless-rulesbooks` | N | ❓ | |
| 17 | `fantasy-football-demo` | Y | ❓ | |
| 18 | `gym-trainer-invoicing` | Y | ❓ | |
| 19 | `intelligence-taxonomy-demo` | Y | ❓ | |
| 20 | `jobsearch-rag` | Y | ❓ | |
| 21 | `llm-enigma-test` | Y | ❓ | |
| 22 | `product-inventory-demo` | Y | ❓ | |
| 23 | `therapist-helper-portal` | Y | ❓ | |
| 24 | `wedding-seating-optimizer` | Y | ❓ | |

### Group D — Broken / incomplete (need decision before building)

| # | Project | Problem | Status | Notes |
|---|---|---|---|---|
| 25 | `expense-approval-demo` | No `effortless.json`, no rulebook. Only README. | ❌ | **Decide:** scaffold from template, or delete. |
| 26 | `volunteer-shift-scheduler-demo` | No `effortless.json`, no rulebook. Has `.xlsx`, `schema/`, `server/`, `web/`, etc. — looks like a vibe-coded shape with a spec doc. | ❌ | **Decide:** generate rulebook from spec, or delete. |
| 27 | `naked-claude-vs-effortless-claude` | Umbrella folder. Has `effortless.json` but no `effortless-rulebook/` dir. Holds `v1_SPECIFICATION.md`…`v4_SPECIFICATION.md`, `tale-orchestrator.mjs`, comparison HTMLs, `Naked-Claude-Experiments/` archive. | ➖ | Not an ontology — it's the comparison harness. Either remove the stray `effortless.json` or accept that it won't `effortless build`. |
| 28 | `v3-nakedclaude-demo-naked` | Empty dir (only `.DS_Store`) — leftover from this session's cleanup. | ➖ | Delete `.DS_Store` and `rmdir`. |

---

## Per-project recipe

For each project in Groups A, B, C, in order:

### 0. Pre-flight
```bash
cd rulebook-examples/<project>/
git status .           # rulebook JSON must not have uncommitted edits not made this turn
```

### 1. Audit `effortless.json`
- `Name` matches dir name.
- `SSoTmeProjectId` is unique.
- For Group A: `baseId` matches `bases.json` AND matches what `airtabletorulebook` expects.
- For Group A: `airtabletorulebook` entry exists, `IsDisabled: false`, `Enabled: true`.
- For Group C: NO `airtabletorulebook` entry. (Remove if present.)
- `CommandLine` strings reference the correct `<project>-rulebook.json` filename.

### 2. Audit `CLAUDE.md`
- Project name and rulebook filename references are correct.
- "Authoritative Source" line matches the chosen SSoT (Airtable for A, JSON for C).

### 3. Run `effortless build`
- For Group A: this will pull from Airtable (overwriting the local JSON), then run downstream transpilers.
- For Group C: this just runs downstream transpilers against the local JSON.
- **Watch for postgres side effects** — every build with a `rulebooktopostgres` + `execute ./init-db.sh` chain will (re)create the per-domain Postgres DB.

### 4. Fix any errors
- **Schema-level errors** (table/column conflicts, FK targets missing) → fix the rulebook JSON or Airtable schema. Rebuild.
- **View regeneration errors** (`cannot drop columns from view`) → typically means a previous run left an incompatible view. First try `DROP DATABASE erb_<domain>` and re-build clean. If the error persists from a clean DB, the rulebook itself has a structural problem — fix it.
- **Function errors** → fix the rulebook formula. Only as last resort, override the function body in `02b-customize-functions.sql`.

### 5. Re-disable Airtable pull (Group A only)
Per global `CLAUDE.md`: after a successful Airtable pull, flip `IsDisabled` back to `true` and `Enabled` back to `false` so routine builds don't silently re-pull.

### 6. Mark status in this doc
Update the status column (`✅`, `🟡`, `❌`) with a one-line note on what was fixed.

### 7. Commit per-project
One commit per project, scoped to that project's directory + this doc's status update. Makes failure recovery surgical.

---

## Execution order

Suggested sweep order (simplest first, hardest last):

1. **Group A simple cases:** `acme-llc`, `acme-corporation`, `customer-fullname`, `jessica-basic`, `jessica-advanced`, `star-trek`, `is-everything-a-language`
2. **Group A v1–v4 (this session's focus):** `v1-`, `v2-`, `v3-`, `v4-nakedclaude-demo`
3. **Group B decision:** `guessing-game` — reclassify, then build
4. **Group C postgres-backed:** all 11 (`community-event-planner-demo` … `wedding-seating-optimizer`)
5. **Group C JSON-only:** `effortless-rulesbooks`
6. **Group D decisions:** `expense-approval-demo`, `volunteer-shift-scheduler-demo`, `naked-claude-vs-effortless-claude`, `v3-nakedclaude-demo-naked`

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

## Open questions to resolve before sweeping

1. `customer-fullname` shares `acme-llc`'s base — intentional dual-projection, or stale copy?
2. `jessica-basic`'s base ID (`applThn0rikpCR9C3`) isn't in `bases.json` — add it or replace it?
3. `guessing-game`'s placeholder base ID — is there a real base for it, or is it JSON-only?
4. `expense-approval-demo` and `volunteer-shift-scheduler-demo` — build or delete?
5. `naked-claude-vs-effortless-claude/effortless.json` — keep as umbrella config (and accept it won't build) or remove?
