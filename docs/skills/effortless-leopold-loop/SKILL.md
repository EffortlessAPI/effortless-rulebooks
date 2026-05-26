<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-leopold-loop/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-leopold-loop
description: >
  Use whenever the user mentions the "Leopold loop", "the loop", "a turn of the loop",
  "do a turn", "rebuild the rulebook", "update the app to match the rules", or any
  reference to the iterative ERB development cycle. This is the user's name for the
  CHANGE-RULE → REBUILD → CONSUME-VIEWS workflow that makes ERB feel effortless.
  Load this skill on first mention so you understand what the user expects to happen.

  **Scope (load gate):** Effortless projects only — project root must contain `effortless.json` AND a CLAUDE.md identifying the project as ERB methodology. Do NOT load otherwise.
audience: customer
---

# The Leopold Loop

The "Leopold loop" is the user's name for the iterative ERB development cycle. It is the **core workflow** that makes ERB feel effortless compared to hand-coding without the rulebook (a mode the user calls **"naked Claude"** — every layer of schema, migration, DTO, ORM model, API serializer, and client type written and maintained by hand). When the user mentions the loop in any form, they are invoking this entire mental model — load this skill so you respond in the right paradigm.

> **"Naked Claude"** (used in passing throughout this skill): coding without
> the rulebook — i.e. hand-writing every schema/migration/DTO/serializer
> layer instead of generating them from a single rulebook source. The
> Leopold loop's whole purpose is to eliminate that mode.

## The Loop

```
   1. CHANGE THE RULE (once, in the hub — effortless-rulebook.json, the SSoT)
      via whichever input spoke fits: rulebook-direct (LLM/hand-edit),
      Airtable (if connected), or reverse-sync.
            |
            v
   2. effortless build  (one command)
            |
            v
   3. EVERY DOWNSTREAM LAYER UPDATES AUTOMATICALLY
      - effortless-rulebook.json (the hub, now updated)
      - postgres/01-05*.sql (tables, functions, views, seed data)
      - ODXML schema
      - C#/Go/Python/etc. base classes, ORM context, sync services
            |
            v
   4. APP CODE (server, client) JUST CONSUMES THE GENERATED VIEWS
      - reads from vw_* views
      - treats calculated fields (e.g. is_stopped) as opaque
      - NEVER reimplements business logic that lives in the rulebook
            |
            v
   5. NEXT TURN OF THE LOOP — repeat from step 1
```

## Why it's "effortless"

A single rule change propagates through every layer with **zero hand-written migrations, DTOs, ORM updates, API serializers, or client types**. The business logic ("a customer is stopped when CurrentColor is Red") lives in **exactly one place** — the rulebook hub (authored directly or via Airtable) → generated SQL function → exposed in the view as `is_stopped`. The app just reads `is_stopped`. If the rule flips ("now Green means stopped"), the loop runs once and *no app code changes*.

Compare to **naked Claude** (defined above — hand-coding every layer): the same change requires editing a migration, seed data, DTO, ORM model, API serializer, client type, and client logic — and probably missing one and shipping a bug. The Leopold loop exists specifically to eliminate that class of failure.

## Phrases that mean "do a turn of the loop"

When the user says any of these, they expect the same sequence of actions:

- *"Do a turn of the loop"* / *"Run the loop"* / *"Take a turn"*
- *"Rebuild the rulebook"*
- *"Update the app to match the current rules"*
- *"Re-sync everything"*
- *"Push the rule change through"*
- *"Make the app reflect the new schema"*

All of these mean: **propagate the current rulebook state through every downstream layer, then update only the app's schema-surface code.** (If the project is Airtable-connected, the build pulls Airtable into the rulebook first.)

## What "do a turn of the Leopold loop" actually entails

0. **Pre-build: check the tree first.** Run `git status --porcelain` (read-only).
   If non-empty, **pause and ask the user for permission to build** — they may want to commit or stash first
   so the resulting diff cleanly isolates the build output. Do not offer to commit, stash, or `git add`
   anything yourself; the user owns their git state. Once the user gives the go-ahead (or the tree is clean),
   proceed.
1. **Run `effortless build`** from the project root. This is atomic — fire and forget.
   Do NOT read the generated files afterwards. (Ask permission first if your project's CLAUDE.md requires it.)
   **Do NOT commit the output yourself.** The user will commit when they choose to. You may proceed with the
   rest of the loop on the dirty tree the build just produced — but do not run `git add`, `git commit`, or
   any other git write command.
2. **Run `init-db.sh`** if the project has a postgres target — this **drops and recreates the database from scratch** using the freshly generated SQL. This is a full regeneration, not an incremental migration; it's also why ERB local-dev projects don't have a `migrations/` folder. (Bases-hosted DBs are the exception; never run `init-db.sh` against bases.)
3. **Query the rulebook for schema changes** — use a lightweight one-liner to see what
   tables/fields exist now (see `effortless-query`). Do NOT read generated SQL files.
   Or use `psql -c "\d vw_tablename"` to see the current view columns.
   Or — even better — use `git diff -- effortless-rulebook/effortless-rulebook.json` (read-only)
   against the unstaged build output. This is the highest-fidelity view of what changed.
4. **Update the app code only where it touches the schema surface** — column names that changed, new fields the UI now needs to display, removed tables to clean up references to. **Never reimplement** rule logic in the app; consume the calculated fields from the view as opaque truth.
5. **Restart the app** — run `./start.sh` from the project root.

## MANDATORY: Always Build After Hub Changes

**Every time** the rulebook hub is modified — directly, via Airtable (API/OMNI/UI), or via reverse-sync — an `effortless build` MUST follow. No exceptions. The build is what propagates the change through every output spoke. Without it, the generated code is stale and the app is out of sync with the SSoT.

## Anti-patterns (these BREAK the loop — never do them)

- **Writing a migration to "make a schema change persist."** This is the #1 way the loop gets bypassed. ERB has no `migrations/` folder, no migrations table, no incremental SQL deltas — `init-db.sh` drops and recreates the whole DB on every build. If the answer feels like "write a migration / `ALTER TABLE` / insert into a migrations log," the answer is **"edit the rulebook (directly or via Airtable if connected) and rerun `effortless build`."** Migrations only exist on `bases.effortlessapi.com`-hosted DBs, and even there schema still originates in the rulebook — see `effortless-workflow` "NO MIGRATIONS" section + `effortless-bases`.
- **Reimplementing rule logic in the client** — e.g. computing `isStopped = customer.color === 'Red'` in JS instead of using `customer.is_stopped` from the view. This duplicates the rule in two places. When the rule changes in Airtable next time, the client silently goes wrong because nobody updated the duplicated copy. **The whole point of the loop is that the rule lives in exactly one place.**
- **Hand-editing generated files** — `postgres/01-05*.sql`, `dotnet/.../BaseClasses/*.cs`, etc. They get blown away on the next build. If you need to override generated SQL, use the `*b-customize-*` files (see `effortless-sql` skill), and only after exhausting Airtable as the source of the change.
- **Adding columns/fields directly in SQL or C#** — changes must originate in the rulebook hub so they survive `effortless build`. Edit `effortless-rulebook.json` directly, or use the `effortless-airtable` skill for scalar fields and `effortless-airtable-omni` for formulas/lookups/rollups in Airtable-connected projects.
- **Caching the rulebook output and forgetting to rebuild** — always rebuild before reasoning about the current state. Stale generated code is a common source of confusion.
- **Skipping the build and editing generated SQL "just this once"** — there is no "just this once". The next build erases it and the bug returns. Always go around the loop.
- **Editing generated output spokes as if they were the source** — postgres SQL, Go, Python, OWL, XLSX, etc. are *outputs*. The hub is `effortless-rulebook.json`. Edit the hub (directly, or via Airtable if connected, or via reverse-sync — all with permission), never the generated spokes.

## See also

- `effortless-orchestrator` — the big-picture mental model; references this skill for the loop itself.
- `effortless-workflow` — choosing among input spokes (rulebook-direct, Airtable, reverse-sync).
- `effortless-pipeline` — the mechanics of `effortless build` itself.
- `effortless-airtable` / `effortless-airtable-omni` — *how* to make the rule change via the Airtable spoke (when connected). For rulebook-direct, just edit the JSON.
- `effortless-sql` — verifying step 3's generated output and using `*b-customize-*` overrides correctly.

## TL;DR for future-you

If the user says "the loop" or "Leopold loop" and you're not sure what to do: **load this skill, then run a turn of it.** Don't grep the project for "leopold". Don't ask the user to explain. The loop is a workflow, not a string.
