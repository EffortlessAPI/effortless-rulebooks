<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-workflow/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-workflow
description: >
  Use when making changes to an ERB project — modifying effortless-rulebook.json
  directly, editing via Airtable when connected, or running effortless build.
  `effortless-rulebook.json` is the hub/SSoT; Airtable, LLM-direct edits, and
  reverse-sync are peer input spokes. Covers the input-spoke options and permission
  checkpoints.

  **Scope (load gate):** Effortless projects only — project root must contain `effortless.json` AND a CLAUDE.md identifying the project as ERB methodology. Do NOT load otherwise.
audience: customer
---

# ERB Change Workflow

## CRITICAL: Always Ask Before Modifying

**Before modifying `effortless-rulebook.json` (directly or via Airtable, reverse-sync, or any other input spoke), or running `effortless build`, ALWAYS ask the user for permission.** These are consequential operations that affect the hub (single source of truth) and trigger code regeneration across every output spoke.

## NO MIGRATIONS — read this before writing any SQL

This is an ERB project. The local Postgres DB is **regenerated from scratch on every `effortless build`** via `init-db.sh` (drop + recreate). **There is no `migrations/` folder, no migrations tracking table, no incremental SQL deltas in this paradigm.** That entire pattern belongs to a different deployment shape (`bases.effortlessapi.com` — the only exception, covered below).

**To change schema, RLS, calculated fields, or seed data, the answer is always one of these — never a migration. All of them write to the same hub (`effortless-rulebook.json`); they differ only in which input spoke you use:**

1. **Edit `effortless-rulebook.json` directly** (with permission) → `effortless build`. The simplest path for LLMs — the rulebook is just JSON, and edits are surgical.
2. **Edit via Airtable** (when the project is Airtable-connected) → `effortless build` pulls Airtable into the rulebook, then regenerates downstream.
3. **Edit the rulebook directly, then reverse-sync to Airtable** so the human-friendly editing surface stays in step → `effortless build -id` from `push-to-airtable/`, then normal `effortless build` from root.
4. **Edit a `*b-customize-*.sql` file** ONLY for infrastructure the rulebook genuinely cannot model (auth tenants, JWT helpers, role GRANTs) — never for business entities. See `effortless-sql`.

**If you find yourself about to do any of these, stop and reread the rule:**

- Running `CREATE TABLE` / `ALTER TABLE` / `DROP TABLE` / `INSERT INTO ...` against the local DB by hand or via psql to "make a schema change persist."
- Creating a file under `postgres/migrations/` (folder shouldn't exist for local-dev projects).
- Inserting into a `migrations` (or `schema_migrations`, `_migrations`, `applied_migrations`, etc.) tracking table to "register" a change.
- Editing a generated `0*.sql` file (they get overwritten on the next build).

**The internalized redirect:** if the answer feels like "write a migration," the answer is **"edit the rulebook (directly, or via Airtable if connected) and rerun `effortless build`."** Say that sentence to yourself before reaching for SQL.

### Bases is the only exception

`bases.effortlessapi.com`-hosted databases (tell: `BASES_DATABASE_URL` in `.env.example`, or the project's CLAUDE.md has the "Bases is migration-only" block) cannot be dropped + recreated, so they DO use migrations — applied via `postgres/apply-migration.sh`, never `psql` directly. **Even there, schema still originates in the rulebook**; the migration file is a delivery mechanism for a rulebook delta, not a place to author schema from scratch. See `effortless-bases` for the full bases pattern.

**Tell which path you're on before touching Postgres:** no `BASES_DATABASE_URL` and no "Bases is migration-only" block in CLAUDE.md → you are on the local-dev path and the rule is **no migrations, ever**.

## Input Spokes for Editing the Hub

The hub is `effortless-rulebook.json`. Every workflow below mutates that hub —
they differ only in *how* the mutation gets there. Pick by project shape and
ergonomics, not by historical preference.

### Spoke 1: Rulebook-direct (LLM + ERB + Postgres)

The cleanest path, and the default for new LLM-driven projects. The rulebook is
JSON — LLMs edit it natively.

1. **Edit `effortless-rulebook.json` directly** (with permission). Add/modify table objects, fields, formulas, lookups.
2. **`effortless build`** — regenerates Postgres + every other output spoke.
3. If the project is also Airtable-connected and you want the human-friendly view in step: reverse-sync (Spoke 3) before the build.

```
LLM / hand-edit (you edit here)
    |
    v
effortless-rulebook.json  ← THE HUB
    |  rulebook-to-postgres, rulebook-to-*
    v
All output spokes (regenerated)
```

### Spoke 2: Airtable-connected

Use when the project has `airtable-to-rulebook` configured and the human prefers
Airtable's UI for editing schema/data. Airtable is one good input spoke — not the
SSoT.

1. **Modify Airtable** — Add/change fields, data, or formulas in the Airtable base
   - API key resolution: `AIRTABLE_API_KEY` env var > `~/.ssotme/ssotme.key` (`APIKeys.airtable`) > project settings
   - Set the key via: `effortless -setAccountAPIKey airtable=patXXXXXXXX.XXXXXXXX`
   - If no API key is available, tell the user — they may set it or make the change in Airtable UI
2. **`effortless build`** from the project root — `airtable-to-rulebook` pulls Airtable into the hub, then downstream transpilers regenerate every output spoke. Mandatory after every Airtable modification.

```
Airtable (you edit here)
    |  airtable-to-rulebook
    v
effortless-rulebook.json  ← THE HUB (updated)
    |  rulebook-to-postgres, etc.
    v
All output spokes (regenerated)
```

### Spoke 3: Reverse-sync (rulebook → Airtable)

For Airtable-connected projects, when it's more practical to edit the JSON
directly but you still want Airtable mirrored.

1. **Edit `effortless-rulebook.json` directly** — Make your changes in the JSON hub
2. **Push changes back to Airtable** — Run `effortless build -id` from the `effortless-rulebook/push-to-airtable/` subfolder
   - The `-id` flag tells effortless to include disabled transpilers — `rulebook-to-airtable` is typically disabled in `effortless.json` because reverse-sync is opt-in
   - This must be run directly from the `push-to-airtable/` subfolder, not from the project root
3. **Then run the normal build** — `effortless build` from project root to regenerate downstream files

```
effortless-rulebook.json  ← THE HUB (you edit here)
    |  rulebook-to-airtable (via build -id from push-to-airtable/)
    v
Airtable (mirrored back, for human convenience)
    |  then normal: effortless build from root
    v
All output spokes (regenerated)
```

## Permission Checkpoints

**STOP and ask the user before ANY of these actions:**
- Editing `effortless-rulebook.json`
- Modifying Airtable schema or data (via API or any other method)
- Running `effortless build` (from root or any subfolder)
- Running any individual transpiler

These are not routine file edits — they affect the source of truth and trigger cascading regeneration.

## "Just add a small table" is the #1 trap

When the user says "make a Foo table" / "add a Bar entity" / "I need an X table" — that goes in the **rulebook hub**. Edit `effortless-rulebook.json` directly (or via Airtable, if connected), then `effortless build` regenerates `public.foo` + `vw_foo`. Do NOT hand-write the table in `01b-customize-schema.sql`, do NOT write a migration file, and do NOT `CREATE TABLE` against the local DB. The `01b-05b` files are for infrastructure the rulebook cannot model (auth tenants, JWT helpers, role GRANTs) — never for business entities. If you're typing `CREATE TABLE app.users (...)` or anything that looks like a migration, you've taken a wrong turn — see the "NO MIGRATIONS" section above.

## Don't drive git on the user's behalf

Effortless skills are read-only with respect to git. Before `effortless build`, check the tree with
`git status --porcelain` (read-only); if non-obvious changes are present, **ask the user for permission
to build** — they may want to commit or stash first so the resulting diff cleanly isolates the regenerated
output from hand-written follow-ons. After the build, do NOT auto-commit; leave the dirty tree for the
user to commit when they choose.

(Exception: `effortless-setup-postgres` performs one-time bootstrap commits during initial project
creation — that flow is explicitly authorized to drive git. Nothing else is.)

## NO SILENT FALLBACK ALLOWED

If you cannot complete a change through the input spoke you started with (e.g., Airtable API limitations for formula fields, no API key, no Airtable connection at all):
1. **STOP** - Do not silently fall back to manual edits of generated files
2. **ASK THE USER** - Explain the blocker and present the options:
   - **Rulebook-direct**: You edit `effortless-rulebook.json` directly, then `effortless build`
   - **Airtable UI**: User makes the change manually in Airtable's UI, then runs `effortless build`
   - **Reverse-sync**: Edit the JSON directly, push to Airtable via `effortless build -id` from `push-to-airtable/`, then `effortless build`
   - **Customization files**: Use `*b-customize-*` files only for logic the rulebook genuinely cannot model
3. **Wait for user direction** - do not proceed without explicit permission

---

## See also

- `effortless-orchestrator` — for the bigger mental model and the schema-change decision tree this skill operationalizes.
- `effortless-leopold-loop` — for the iterative cycle every spoke produces.
- `effortless-airtable` / `effortless-airtable-omni` — for *how* to make the change via the Airtable spoke (API vs OMNI).
- `effortless-pipeline` — for the `-id` flag mechanics referenced in the reverse-sync spoke.
- `effortless-sql` — for the rule that `*b-customize-*.sql` is the ONLY hand-edited surface, and never for business entities.
