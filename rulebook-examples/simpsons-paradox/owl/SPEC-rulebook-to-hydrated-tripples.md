# Spec: `rulebook-to-hydrated-tripples`

**Author this document gives to:** the developer of the new transpiler tool.  
**Verified by:** the test harness at `owl/test-hydrated-tripples.py` (written alongside this spec).

---

## Problem Being Solved

The existing `rulebook-to-tripples` tool generates `individuals.ttl` containing only
**asserted (leaf) facts** — raw field values and forward-FK edges, no derived fields.
The existing `rulebook-to-owl` + SHACL reasoning path then runs 366 SPARQL CONSTRUCT
rules over those individuals in a Python fixpoint loop (pyshacl + rdflib) to derive
calculated fields. This is slow (~minutes on the Simpson's Paradox corpus of ~7,600
individuals).

**The core insight:** every derived field is already computed and sitting in Postgres
views (`vw_treatment_rankings`, `vw_stratum_summaries`, `vw_stratum_variables`, …).
A new tool — `rulebook-to-hydrated-tripples` — should generate `individuals.ttl` with
those computed values **pre-baked in**, so the OWL conformance path becomes:

1. Load the pre-baked `individuals.ttl` (no reasoning needed)
2. Optionally run SHACL *shape validation only* (not CONSTRUCT derivation) to confirm
   the asserted graph is self-consistent
3. Compare against Postgres — they should match because Postgres **is** the source

This turns a multi-minute fixpoint derivation into a sub-second graph load.

---

## What the Tool Must Do

### Inputs

The tool accepts the same request envelope as all other ERB transpilers:

- **Rulebook JSON** (`-i` flag / `InputFileSet` file named `effortless-rulebook.json`
  or `<domain>-rulebook.json`) — the single input. The tool derives everything else
  from it: table names, field names, field types, PK fields, OWL property names, and
  the `SubstrateConformanceFields` compare manifest.

### Internal Pipeline

This tool is **self-contained**. It does not require a pre-existing Postgres database.
Instead it runs the following pipeline internally:

1. **Invoke `rulebook-to-postgres`** on the rulebook JSON — this generates the full
   SQL schema + seed data + view definitions (the same output that a normal
   `effortless build` would write to `effortless-postgres/`).

2. **Spin up a temporary Postgres instance** (or use a temp schema in a local
   Postgres) and execute the generated SQL — creating all tables, inserting seed data,
   and creating all `vw_*` views with their compiled formula expressions.

3. **Query every `vw_<table>`** to read the fully-computed rows — raw fields,
   calculated fields, lookups, aggregations, closures — all already resolved by
   Postgres.

4. **Emit `individuals-hydrated.ttl`** — one individual per row, one triple per
   non-null column, using camelCase OWL property names derived from the rulebook field
   names (and the `SubstrateConformanceFields` manifest for manifest-covered tables).

5. **Tear down the temp schema / database.**

The caller passes only the rulebook JSON. No `DATABASE_URL`. No pre-built Postgres
instance. The tool is fully portable — give it a rulebook, get back a hydrated triple
file.

### Outputs

A single output file: `individuals-hydrated.ttl`

This is a **superset** of what `rulebook-to-tripples` currently emits. It includes
everything the existing tool writes (raw fields + FK edges), **plus** one additional
triple per individual per derived field for every field that has a value in the
corresponding Postgres view.

### IRI Convention

Identical to the existing tool and to `rulebook-to-owl`:

```
@prefix effortless-ntwf: <https://w3id.org/effortless-ntwf#> .

effortless-ntwf:<pk-value> a effortless-ntwf:<ClassName> ;
    effortless-ntwf:<camelCaseField> <typed-literal> .
```

PK values are URL-slug-safe: spaces replaced with `-`, lowercased. This must match
exactly what `rulebook-to-owl` emits so the ontology + hydrated individuals graph
can be loaded together without IRI mismatches.

### XSD Type Mapping

| Rulebook datatype | XSD type                  | Turtle literal example          |
|-------------------|---------------------------|---------------------------------|
| `string`          | `xsd:string` (bare)       | `"kidney-1986"`                 |
| `float`           | `xsd:decimal`             | `0.591`                         |
| `int`             | `xsd:integer`             | `42`                            |
| `boolean`         | `xsd:boolean`             | `true` / `false`                |

Null / empty values: **omit the triple entirely** (do not emit `""` or `0` as a proxy
for null).

### Field Inclusion Rules

For each individual (row) in each table:

1. **Raw fields** — always included (same as `rulebook-to-tripples`).
2. **Forward-FK fields** — edge triple to the target individual's IRI (same as existing
   tool). Back-references are suppressed (same as existing tool).
3. **Calculated / lookup / aggregation / closure fields** — included **if and only if**
   the Postgres view for this table has a non-null value for this row's column. The
   value comes from Postgres, not from re-evaluating the formula.
4. **`__meta__` table** — skip entirely (no individuals emitted for it).

### Table → View Mapping

The tool must resolve the Postgres view name for each table. Convention:

```
<TableName>  →  vw_<snake_case(TableName)>
```

Examples: `TreatmentRankings` → `vw_treatment_rankings`, `CaseCells` → `vw_case_cells`.

If `vw_<table>` does not exist in the database, fall back to the raw table
`<snake_case(TableName)>`. If neither exists, **raise** — do not silently skip.

### Cross-Join Inference from Rulebook Structure

This is the key capability that distinguishes this tool from `rulebook-to-tripples`.

The rulebook fully describes every derived field — its formula, its source table, its
FK relationships. The tool reads that structure and uses it to determine which Postgres
view column to pull for each field, **without needing SHACL rules at all**. The
reasoning is:

> "This field is a `lookup` of `FieldX` on the table reached via FK `RelatedTo`. The
> view for this table already has that lookup resolved as `column_x`. Pull it from
> there and emit it as a triple."

Concretely, for each table the tool must:

1. **Read the full schema** from the rulebook: field name, field type, datatype,
   formula (for calculated fields), FK target table (for lookup/aggregation fields).

2. **Query `vw_<table>`** — which already contains every raw + derived column,
   because `rulebook-to-postgres` compiled all formulas into the view definition.

3. **Emit one triple per non-null column** in the view row, using the camelCase
   property name derived from the rulebook field name (or the `OwlLocalName` from
   `SubstrateConformanceFields` where available).

The result is that **every field in every table** — raw, calculated, lookup,
aggregation, closure — gets a triple in `individuals-hydrated.ttl`, as long as
Postgres has a non-null value for it. No formula evaluation. No SPARQL CONSTRUCT.
No fixpoint loop. The view is the oracle.

**Why this works:** `rulebook-to-postgres` already walked the full DAG and compiled
every formula into SQL. The view's row IS the fully-inferred individual. Reading from
it is not a shortcut — it's the correct, authoritative answer for every field type:

| Field type    | How it ends up in the view                         | Triple source          |
|---------------|----------------------------------------------------|------------------------|
| raw           | direct column from seed data                       | view row               |
| calculated    | SQL expression from formula                        | view row               |
| lookup        | JOIN to related table's view, project field        | view row               |
| aggregation   | COUNT/SUM/AVG sub-query over child rows            | view row               |
| closure       | recursive CTE or transitive join                   | view row               |

The tool does **not** need to parse formulas, resolve FK chains, or run any inference.
It only needs to know the camelCase OWL property name for each Postgres column — which
comes from the rulebook field name (or the `SubstrateConformanceFields` manifest for
tables that have one).

### SubstrateConformanceFields Integration

The rulebook carries a `SubstrateConformanceFields` table whose rows declare which
fields are in the cross-substrate compare set (`InCompareSet: true`) and which OWL
local name maps to which Postgres column (`OwlLocalName`, `PgColumn`, `SourceTable`).

The tool **must** use this manifest to drive the hydration of those specific fields —
ensuring that the triple names written to `individuals.ttl` are exactly the
`OwlLocalName` values declared in the manifest (camelCase), not an independent
camelCase transformation that might diverge.

For tables **not** in the manifest, use the standard camelCase-of-field-name
convention for property IRIs.

---

## Output File Structure

```turtle
@prefix rdf:  <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
@prefix xsd:  <http://www.w3.org/2001/XMLSchema#> .
@prefix effortless-ntwf: <https://w3id.org/effortless-ntwf#> .

# === Individuals: AllocationSweep ===

effortless-ntwf:kidney-1986-f005 a effortless-ntwf:AllocationSweep ;
    effortless-ntwf:sweepId "kidney-1986-f005" ;
    effortless-ntwf:studyId "kidney-1986" ;
    effortless-ntwf:allocFractionA 0.05 ;
    effortless-ntwf:nSweepA 3 ;          # ← hydrated from Postgres view
    effortless-ntwf:sweepRateA 0.312 .   # ← hydrated from Postgres view

# === Individuals: TreatmentRankings ===

effortless-ntwf:kidney-1986-treatment-a a effortless-ntwf:TreatmentRankings ;
    effortless-ntwf:rankingId "kidney-1986-treatment-a" ;
    effortless-ntwf:pooledRateA 0.591 ;   # ← SubstrateConformanceFields manifest
    effortless-ntwf:isReversal true ;      # ← SubstrateConformanceFields manifest
    ...
```

Tables are emitted in alphabetical order. Within each table, rows are emitted in PK
order. Properties within a row are emitted: `rdf:type` first, then raw fields in
schema order, then derived fields in schema order.

---

## Local Testing Layout

The tool should write its output to:

```
<project-root>/owl/src/individuals-hydrated.ttl
```

This sits alongside the existing `individuals.ttl` (asserted-only). The test harness
at `owl/test-hydrated-tripples.py` diffs the hydrated file against Postgres and
against the SHACL-derived graph to confirm all three agree.

The tool should be invocable as a local HTTP server on **port 30041** (one above the
existing `rulebook-to-tripples` port 30040) with the same wire protocol:

```
POST http://localhost:30041/
Content-Type: application/json
{ "InputFileSet": { "Files": [{ "RelativePath": "simpsons-paradox-rulebook.json",
                                 "Content": "<base64-or-text>" }] } }
```

No `DATABASE_URL` parameter — the tool manages its own Postgres context internally.

Add an `effortless.json` entry (disabled by default):

```json
{
  "IsSSoTTranspiler": false,
  "Name": "rulebooktohydratedtripples",
  "RelativePath": "/owl",
  "CommandLine": "rulebook-to-hydrated-tripples -i ../effortless-rulebook/simpsons-paradox-rulebook.json",
  "IsDisabled": true
}
```

---

## Publishing

Once the tool passes the test harness locally:

1. The tool source lives under:
   ```
   .../Versioned-Stable-SSoTme-Tools/tools/effortless/rulebook-to-hydrated-tripples/
   ```
   Modeled exactly on `rulebook-to-tripples` (same Dockerfile shape, same C# project
   structure, same EffortlessHelper wire protocol). Port in `cpln/workload.yaml`: 30041
   (local) / 30000 (cloud, standard).

2. Publish via:
   ```bash
   cd .../Versioned-Stable-SSoTme-Tools
   scripts/publish-tool.sh rulebook-to-hydrated-tripples
   ```
   (transpiler-server must be running: `cd transpiler-server && npm run dev`)

3. After publishing, install into the simpsons-paradox project:
   ```bash
   cd rulebook-examples/simpsons-paradox/owl/
   effortless -install https://<published-url>/rulebook-to-hydrated-tripples \
       -i ../effortless-rulebook/simpsons-paradox-rulebook.json
   ```

---

## Success Criteria (what the test harness checks)

1. `individuals-hydrated.ttl` can be parsed by rdflib without errors.
2. Every individual IRI in `individuals.ttl` (asserted-only) also appears in
   `individuals-hydrated.ttl` with the same asserted triples (hydrated is a superset).
3. For every row in `SubstrateConformanceFields` where `InCompareSet=true`, the
   hydrated graph contains the correct triple matching the Postgres view value
   (within float tolerance 1e-6).
4. No triple in the hydrated graph contradicts a Postgres view value.
5. The SHACL conformance path (`owl/reason.py` with `USE_HYDRATED=true`) reports 0
   mismatches when it loads `individuals-hydrated.ttl` instead of running derivation.
