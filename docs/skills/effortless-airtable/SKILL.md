<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-airtable/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-airtable
description: >
  Use when making schema or data changes via the Airtable API in an
  Airtable-connected ERB project — adding fields, creating tables, modifying
  existing fields, or when you need to understand Airtable API limitations
  (e.g., formula fields cannot be created via API). Only relevant if the
  project is *explicitly* Airtable-connected (see "Is this an Airtable project?"
  below); otherwise this is a Rulebook-First project and edits go to the hub
  directly.

  **Scope (load gate):** Effortless projects only — project root must contain `effortless.json` AND a CLAUDE.md identifying the project as ERB methodology. Do NOT load otherwise.
audience: customer
---

# Airtable as One (Optional) Input Spoke

In ERB, **`effortless-rulebook.json` is the hub / single source of truth.** The
**default** is Rulebook-First: edits go to the hub (LLM-direct or hand-edits)
and Airtable is — at most — a *downstream consumer* mirrored via
`rulebook-to-airtable`. Airtable only becomes an input spoke when the project
has *explicitly opted in*.

## Is this an Airtable project?

A project is Airtable-connected as an **input spoke** only if **both** of these
are true in `effortless.json`:

1. `ProjectSettings` has a `baseId` entry with a real Airtable base ID
   (`appXXXXXXXX...`), AND
2. `ProjectTranspilers` has an `airtable-to-rulebook` entry with
   `IsDisabled: false` (or no `IsDisabled` field at all — defaults to enabled).

Quick check:

```bash
python3 - <<'PY'
import json
with open("effortless.json") as f: cfg = json.load(f)
base_id = next((s["Value"] for s in cfg.get("ProjectSettings", []) if s.get("Name") == "baseId"), None)
a2r = next((t for t in cfg.get("ProjectTranspilers", []) if t.get("Name") == "airtable-to-rulebook"), None)
a2r_enabled = a2r is not None and not a2r.get("IsDisabled", False)
print(f"baseId: {base_id!r}")
print(f"airtable-to-rulebook enabled: {a2r_enabled}")
print(f"=> Airtable-connected: {bool(base_id) and a2r_enabled}")
PY
```

**If both are true → Airtable is a live input spoke. This skill applies.**

**If either is false → Rulebook-First project.** Don't reach for the Airtable
API for schema/data edits — edit `effortless-rulebook.json` directly (see
`effortless-workflow`). Airtable may still be wired as a *downstream* spoke
(`rulebook-to-airtable` only), but in that direction it's an output, not an
input — never edit Airtable first, because `effortless build` will overwrite
your edits with whatever the hub says.

This skill covers the **Airtable input spoke** specifically: how to read/write
Airtable via its API so that `effortless build` picks up your changes into the
hub.

**Airtable-spoke flow:** Edit Airtable → `effortless build` → `airtable-to-rulebook` updates the hub → downstream transpilers regenerate every output spoke (Postgres, etc.).

**Reverse-sync flow:** Edit `effortless-rulebook.json` directly, then push back to Airtable via `effortless build -id` from the `push-to-airtable/` subfolder. Use when the hub is being edited directly but you still want Airtable mirrored.

**In either direction, always ask the user for permission before modifying the rulebook or Airtable.**

## Getting the API Key

**IMPORTANT: Always check the environment variable FIRST — do not grep config files or search the filesystem before trying this.**

```bash
echo "$AIRTABLE_API_KEY"
```

If `AIRTABLE_API_KEY` is set and non-empty, use it immediately. Only if it is empty, fall back in this order:
1. `~/.ssotme/ssotme.key` → parse JSON → `APIKeys.airtable`
2. `effortless.json` → `ProjectSettings` → `_apikey_`

### Reading from `~/.ssotme/ssotme.key`

The file is JSON:
```json
{
  "EmailAddress": "user@example.com",
  "Secret": "...",
  "APIKeys": {
    "airtable": "patXXXXXXXX.XXXXXXXX"
  }
}
```

Extract the key:
```bash
cat ~/.ssotme/ssotme.key | python3 -c "import sys,json; print(json.load(sys.stdin)['APIKeys']['airtable'])"
```

### Setting the API Key

```bash
effortless -setAccountAPIKey airtable=patXXXXXXXX.XXXXXXXX
```

This stores the key in `~/.ssotme/ssotme.key` under `APIKeys.airtable`.

### The `-account airtable` Flag

When using effortless CLI transpilers that talk to Airtable, **always pass `-account airtable`**. This tells the CLI to send the API key configured in `~/.ssotme/ssotme.key`:

```bash
effortless airtable-to-rulebook -o effortless-rulebook.json -account airtable
effortless rulebook-to-airtable -i ../effortless-rulebook.json -account airtable
```

### `effortless.env`

An `effortless.env` file in the project root can also store keys as environment variables (standard dotenv format). This is an alternative for project-scoped secrets.

## Getting the Base ID

The Airtable base ID for the project is stored in the project settings file (`effortless.json`) as `baseId`:

```bash
cat effortless.json | python3 -c "import sys,json; d=json.load(sys.stdin); print(next(s['Value'] for s in d['ProjectSettings'] if s['Name']=='baseId'))"
```

This base ID is shared across all airtable-facing tools and should always be read from here.

## Making Schema Changes via the Airtable Spoke

When this project is Airtable-connected AND you're choosing to use that spoke, schema
changes flow Airtable → hub → output spokes. (If the project isn't Airtable-connected,
or rulebook-direct is more practical, see `effortless-workflow`.)

1. **Get the base ID** from `effortless.json` (the `baseId` setting)
2. **Get the API key** using the priority order above
3. **Use the Airtable API** to make changes
4. **ALWAYS run `effortless build`** from project root to regenerate all code — every time Airtable schema or data is modified, a build must follow

## Adding a Field to a Table

```bash
# 1. Get table schema to find table ID
curl -s "https://api.airtable.com/v0/meta/bases/{BASE_ID}/tables" \
  -H "Authorization: Bearer {API_KEY}" | jq '.tables[] | {id, name}'

# 2. Add the field
curl -s -X POST "https://api.airtable.com/v0/meta/bases/{BASE_ID}/tables/{TABLE_ID}/fields" \
  -H "Authorization: Bearer {API_KEY}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "FieldName",
    "type": "singleLineText",
    "description": "Field description"
  }'

# 3. Regenerate code
effortless build
```

## Common Airtable Field Types
- `singleLineText` - short text
- `multilineText` - long text
- `number` - numeric values
- `checkbox` - boolean
- `singleSelect` - dropdown
- `multipleSelects` - multi-select
- `date` - date only
- `dateTime` - date and time
- `email` - email address
- `url` - URL
- `formula` - calculated field (CANNOT be created/modified via API)
- `multipleRecordLinks` - foreign key relationship

## Modifying an Existing Field

```bash
curl -s -X PATCH "https://api.airtable.com/v0/meta/bases/{BASE_ID}/tables/{TABLE_ID}/fields/{FIELD_ID}" \
  -H "Authorization: Bearer {API_KEY}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "NewFieldName",
    "description": "Updated description"
  }'
```

## Creating a New Table

```bash
curl -s -X POST "https://api.airtable.com/v0/meta/bases/{BASE_ID}/tables" \
  -H "Authorization: Bearer {API_KEY}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TableName",
    "description": "Table description",
    "fields": [
      {"name": "Name", "type": "singleLineText"},
      {"name": "Status", "type": "singleSelect", "options": {"choices": [{"name": "Active"}, {"name": "Inactive"}]}}
    ]
  }'
```

## Generating OMNI Prompts for Base Setup

When creating prompts for Airtable's OMNI AI to set up tables, **always use the two-part split pattern** documented in the `effortless-omni-prompt` skill:

- **Part 1**: Raw fields + `Link to another record` FKs + Name formula. No other computed fields.
- **Part 2**: Lookups & formulas, **organized by table** (OMNI can only work on one table at a time). Excludes MANY-side rollups.

This pattern ensures linked records are established first, making lookups trivial in Part 2. Never combine into a single file — OMNI will produce incorrect field types.

### OMNI Prompt Field Rules

1. **Table names are PascalCase** — `ShaclShapes`, NOT `shacl_shapes`
2. **`Name` is ALWAYS the first field** — it is a formula that creates a lowercase, dash-separated compound key: `Name | formula | SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
3. **`DisplayName` (or `Title`/`Label`)** comes next — the human-readable natural language identifier
4. **NEVER include `{Entity}Id` fields** — surrogate keys are managed by the substrate off-screen
5. **Links use singular PascalCase**: `Schema | link:SDCSchemas`, `TargetClass | link:SDCTypes`

## When the Airtable API can't do something

Some operations (like creating or modifying formula fields) aren't exposed by
the Airtable API. When you hit one in an Airtable-connected project, surface
the blocker and let the user pick a path — don't silently switch to editing
generated files (those edits get overwritten on the next build, so the
apparent fix would evaporate).

Options to present, in order of preference:

1. **Rulebook-direct** — edit `effortless-rulebook.json` to add the formula/lookup field, then `effortless build`. The default for any non-Airtable project, and usually the most ergonomic path even for Airtable projects: the rulebook is JSON, LLMs edit it well, no Playwright/OMNI involved. (If the project is Airtable-connected, follow with a reverse-sync via `push-to-airtable/` so Airtable stays mirrored.)
2. **OMNI via Playwright** — for changes that genuinely need to flow through Airtable first, see `effortless-airtable-omni`.
3. **Airtable UI** — user makes the change manually in Airtable, then runs `effortless build`.
4. **Customization file** — appropriate for SQL that the hub can't model (auth, RLS helpers); not the right fit for a calculated business field.

Wait for direction before proceeding.

---

## See also

- `effortless-airtable-omni` — load this instead when the change is a formula, lookup, rollup, or new table (the API can't do those).
- `effortless-workflow` — for choosing between input spokes (rulebook-direct, Airtable, reverse-sync) and permission checkpoints around schema changes.
- `effortless-conventions` — for the naming / DAG rules that any new field or table must follow.
- `effortless-cli` — for `effortless -setAccountAPIKey airtable=...` and `~/.ssotme/ssotme.key` mechanics.
