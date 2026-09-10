# Effortless Rulesbooks

**A meta-ontology: the ERB project describing itself.**

This example demonstrates the ERB pattern on its own architecture — the rulebook models the very system that builds it.

## What It Models

| Table | Rows | Key Fields |
|-------|------|------------|
| `ProjectMetadata` | 1 | Name, Purpose, Architecture, RepositoryRoot |
| `ExecutionSubstrates` | 10 | Technology, RelativePath, InjectorScript, IsProduction, Status |
| `OrchestrationComponents` | ~6 | FilePath, Language, Purpose, Dependencies |
| `AirtableIntegration` | ~4 | FilePath, Purpose, Role |
| `TestingFramework` | ~5 | FilePath, Purpose, Scope |
| `RulebookDomains` | 7 | ComplexityLevel, TableCount, KeyFeatures |
| `CoreDataFlows` | 5 | Steps, Triggers, Outputs |
| `ProjectConfiguration` | ~8 | FileName, Format, MaintainedBy |
| `Dependencies` | ~10 | Version, Type, Required |

## App

A React + Express table browser for browsing the Postgres database generated from this rulebook.

```bash
cd app && npm install && npm run dev
```

## Quick Start

```bash
effortless build
```

Generates Postgres DDL, Python dataclasses, Go structs, and more from the rulebook.

## Why This Is Interesting

Most ontologies model external domains (customers, orders, media). This one models the tool itself — the substrates, the orchestration, the tests. It proves the ERB pattern is domain-agnostic: if you can describe it in tables and formulas, ERB can generate it.

---

## Local transpiler bus (`127.0.0.1:4242`)

> **All 11 local transpilers are hosted by the effortless CLI itself.** Start
> the bus with `effortless serve -port 4242` from the repo root; it serves every
> tool under `effortless-tools/<name>/` — `oss-postgres-calculated-to-rulebook`,
> `oss-rulebook-to-python`, `oss-rulebook-to-golang`, `oss-rulebook-to-cobol`,
> `oss-rulebook-to-owl`, and more — as a first-class route any `effortless
> build` can call. `GET /` lists them.
>
> **Address it as `127.0.0.1`, never `localhost`.** The host binds the literal
> prefix `http://127.0.0.1:<port>/`, so a request carrying a `localhost` Host
> header gets a bare 404 with no explanation.
>
> **Every repo-local route carries the `oss-` prefix.** The bare names
> (`rulebook-to-python`, `rulebook-to-xlsx`, `rulebook-to-owl`,
> `rulebook-to-airtable`, `airtable-to-rulebook`) belong to the commercial
> catalog as `effortless/effortless/<tool>`; the prefix is the only thing
> keeping a repo-local route from shadowing one.
> `orchestration/local_tool_shim.py` refuses to run if any tool is missing it.
