<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-schema/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-schema
description: >
  Use to understand the **structure** of effortless-rulebook.json — top-level
  keys, table objects, the field schema definition, field types (raw,
  calculated, lookup, relationship, aggregation), datatypes, formula syntax,
  and the `_meta` section. This skill is JSON-structure only; for naming /
  DAG / FK *rules*, use effortless-conventions.

  **Scope (load gate):** Effortless projects only — project root must contain `effortless.json` AND a CLAUDE.md identifying the project as ERB methodology. Do NOT load otherwise.
audience: customer
---

# Rulebook JSON Structure

This skill describes the **shape** of `effortless-rulebook.json`. It does NOT cover naming/DAG/FK rules — those are in **effortless-conventions**.

## Top-level

```json
{
  "$schema": "https://example.com/cmcc-schema/v1",
  "Name": "Project Display Name",
  "Description": "Rulebook generated from Airtable base 'Base Name'.",
  "TableName": { "Description": "...", "schema": [...], "data": [...] },
  "AnotherTable": { ... },
  "_meta": { /* conversion metadata */ }
}
```

| Key | Purpose |
|---|---|
| `$schema` | Always `https://example.com/cmcc-schema/v1` |
| `Name` / `Description` | Project metadata |
| `{TableName}` | One key per entity table |
| `_meta` | Conversion metadata, type mappings, tool version |

## Table object

```json
{
  "Description": "Table: TableName",
  "schema": [ /* field definitions */ ],
  "data": [ /* row records */ ]
}
```

## Field schema object

```json
{
  "name": "FieldName",
  "datatype": "string",
  "type": "raw",
  "nullable": true,
  "Description": "What this field represents.",
  "formula": "=CONCAT({{FirstName}}, \" \", {{LastName}})",
  "RelatedTo": "OtherTable"
}
```

| Property | Required | Values |
|---|---|---|
| `name` | yes | field identifier (PascalCase per conventions) |
| `datatype` | yes | `string`, `integer`, `number`, `boolean`, `datetime` |
| `type` | yes | `raw`, `calculated`, `lookup`, `relationship`, `aggregation` |
| `nullable` | yes | `true` / `false` |
| `Description` | should | free text |
| `formula` | if calculated/lookup/aggregation | Excel-dialect (see below) |
| `RelatedTo` | if relationship | target table name |

## Field types

| Type | Stored In | Meaning |
|---|---|---|
| `raw` | Base table | Direct user input |
| `calculated` | View (via function) | Derived from formula on same-row fields |
| `lookup` | View (via function) | Pulled from a related table via FK |
| `relationship` | Base table (as ID) | Foreign key to another table |
| `aggregation` | View (via function) | Rollup/count/sum over related rows |

## Datatype mapping

| Datatype | Postgres | Go | Python | Airtable source |
|---|---|---|---|---|
| `string` | `TEXT` | `string` | `str` | singleLineText, multilineText, email, url, phoneNumber, singleSelect |
| `integer` | `INTEGER` | `int` | `int` | number (whole) |
| `number` | `NUMERIC` | `float64` | `float` | number (decimal) |
| `boolean` | `BOOLEAN` | `bool` | `bool` | checkbox |
| `datetime` | `TIMESTAMPTZ` | `time.Time` | `datetime` | date, dateTime |

## Formula syntax (Excel dialect)

`={{FieldName}}` references same-row fields. Cross-table uses `Table!{{Field}}`.

```
={{LastName}} & ", " & {{FirstName}}
=IF({{Status}} = "Active", TRUE(), FALSE())
=AND({{HasSyntax}}, {{IsParsed}}, NOT({{CanBeHeld}}))
=INDEX(Roles!{{Label}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0))
=COUNTIFS(WorkflowSteps!{{IsStepOf}}, Workflows!{{WorkflowId}})
=SUMIFS(Orders!{{Amount}}, Orders!{{Customer}}, Customers!{{CustomerId}})
=SUBSTITUTE(LOWER({{CompanyName}}), " ", "-")
```

**Functions:** IF, AND, OR, NOT, TRUE, FALSE, CONCAT, SUBSTITUTE, LOWER, UPPER, LEFT, RIGHT, MID, LEN, TRIM, FIND, SEARCH, TEXT, VALUE, SUM, COUNT, COUNTIFS, SUMIFS, AVERAGEIFS, MIN, MAX, INDEX, MATCH, POWER, LOG, LOG10, ABS, ROUND, COALESCE/IFERROR.

## `_meta`

```json
"_meta": {
  "_CMCC_Summary": "Airtable export with schema-first type mapping...",
  "_conversion_metadata": {
    "source_base_id": "appXXXXXXXXXXXX",
    "table_count": 5,
    "tool_version": "2.0.0",
    "field_type_mapping": "checkbox->boolean, number->number/integer, multipleRecordLinks->relationship...",
    "export_mode": "schema_first_type_mapping",
    "type_inference": {
      "priority": "airtable_metadata (NO COERCION) -> formula_analysis -> data_analysis (fallback only)",
      "error_value_handling": "#NUM!, #ERROR!, #N/A, #REF!, #DIV/0!, #VALUE!, #NAME? are treated as NULL"
    }
  }
}
```

## See also

- `effortless-conventions` — naming, DAG, FK rules. THIS skill is structure-only.
- `effortless-query` — one-liners that extract this structure without reading the full file.
- `effortless-sql` — how each field type / datatype maps to generated Postgres tables, functions, views.
- `effortless-orchestrator` — Token Discipline rule: query this JSON, never read it whole.
