<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-query/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-query
description: >
  Use when querying an effortless-rulebook.json file — listing tables, extracting
  schema without data, finding FK relationships, inspecting calculated fields and
  formulas. Activates for any project with effortless-rulebook.json or
  effortless-rulebook/ directory.

  **Scope (load gate):** Effortless projects only — project root must contain `effortless.json` AND a CLAUDE.md identifying the project as ERB methodology. Do NOT load otherwise.
audience: customer
---

# Querying the Effortless Rulebook

## CRITICAL: This Is Your ONLY Source of Schema Truth

**The rulebook is the single source of truth. NEVER read generated files (SQL, Go,
Python, etc.) to understand the schema.** Those files are projections — the rulebook
is the source. Query it with targeted one-liners that produce minimal output.

### Token Discipline

> The canonical Token Discipline statement lives in `effortless-orchestrator`.
> This section restates the rule from the rulebook-querying angle: **never read
> the full `effortless-rulebook.json` — query it.**

The rulebook JSON can be megabytes (it includes data rows). The targeted queries
below extract ONLY what you need:

- **List tables**: ~5 lines of output
- **Show one table's schema**: ~10-30 lines
- **Find relationships**: ~5-20 lines

These queries cost almost nothing. Reading the full file costs thousands of tokens
that then pollute your context for the entire rest of the conversation.

### The Pattern

```
Need schema info? → Run a one-liner query → Get 10-30 lines → Done
```

**NEVER:**
- `cat effortless-rulebook.json` (full file into context)
- `Read` the rulebook file directly
- Read generated SQL/Go/Python to understand schema
- Load the full JSON "to get a sense of things"

**ALWAYS:**
- Use the python one-liner queries below
- Or `psql -c "\d vw_tablename"` if the database is running
- Extract ONLY the specific table/field you need

## Common Queries

### List All Tables

```bash
cat effortless-rulebook.json | python3 -c "
import sys,json; d=json.load(sys.stdin)
skip={'\$schema','Name','Description','_meta'}
for k in d:
  if k not in skip and isinstance(d[k],dict) and 'schema' in d[k]:
    fields=d[k]['schema']
    print(f'  {k}: {len(fields)} fields, {len(d[k].get(\"data\",[]))} rows')
"
```

### Show Schema for One Table

```bash
cat effortless-rulebook.json | python3 -c "
import sys,json; d=json.load(sys.stdin)
for f in d['TableName']['schema']:
  print(f'  {f[\"name\"]:30s} {f[\"type\"]:15s} {f[\"datatype\"]:10s} {f.get(\"Description\",\"\")[:60]}')
"
```

### Find All FK Relationships

```bash
cat effortless-rulebook.json | python3 -c "
import sys,json; d=json.load(sys.stdin)
for k,v in d.items():
  if isinstance(v,dict) and 'schema' in v:
    for f in v['schema']:
      if f['type']=='relationship':
        print(f'  {k}.{f[\"name\"]} -> {f[\"RelatedTo\"]}')
"
```

### Find All Calculated Fields and Formulas

```bash
cat effortless-rulebook.json | python3 -c "
import sys,json; d=json.load(sys.stdin)
for k,v in d.items():
  if isinstance(v,dict) and 'schema' in v:
    for f in v['schema']:
      if f['type'] in ('calculated','aggregation','lookup'):
        print(f'  {k}.{f[\"name\"]} ({f[\"type\"]}): {f.get(\"formula\",\"\")}')
"
```

### Count Tables and Fields Summary

```bash
cat effortless-rulebook.json | python3 -c "
import sys,json; d=json.load(sys.stdin)
tables=[(k,v) for k,v in d.items() if isinstance(v,dict) and 'schema' in v]
print(f'{len(tables)} tables, {sum(len(v[\"schema\"]) for k,v in tables)} total fields')
for k,v in tables:
  raw=len([f for f in v['schema'] if f['type']=='raw'])
  calc=len([f for f in v['schema'] if f['type'] in ('calculated','lookup','aggregation')])
  rel=len([f for f in v['schema'] if f['type']=='relationship'])
  print(f'  {k}: {raw} raw, {calc} derived, {rel} relationships')
"
```

---

## See also

- `effortless-orchestrator` — canonical Token Discipline; this skill provides the queries it refers to.
- `effortless-schema` — for what each field type / datatype actually means in the JSON.
- `effortless-sql` — for `psql -c "\d vw_tablename"` as the post-build alternative when the DB is up.
- `effortless-diagnostics` — for the same query patterns applied to validation (broken FKs, DAG cycles).
