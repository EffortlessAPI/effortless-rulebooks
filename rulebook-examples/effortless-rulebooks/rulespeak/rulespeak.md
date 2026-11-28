# 📘 ERB Self-Describing Rulebook — Docs

_The rulebook that describes the ERB project itself — the platform eating its own dog food._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Project Metadata** | Project overview | — |
| Name | A defined attribute. | _Effortlessly Invariant Rulesbooks (ERB)_ |
| Purpose | A defined attribute. | _Demonstrate that a single declarative rulebook can generate working code in multiple execution substrates (Python, Go, SQL, etc.) that all produce identical results_ |
| Architecture | A defined attribute. | _Hub-and-spokes around Airtable; rulebook is the disposable IR_ |
| Repository Root | A defined attribute. | _effortlessly-invariant-rulesbooks/_ |

<details open>
<summary>Example data</summary>

| Name | Purpose | Architecture | Repository Root |
|---|---|---|---|
| Effortlessly Invariant Rulesbooks | Multi-substrate code generation from declarative rulebooks | Airtable (UI) → effortless-rulebook.json (IR) → Substrates (Postgres, Python, Go, Excel, OWL, etc.) | effortlessly-invariant-rulesbooks/ |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Execution Substrate** | Runtime environments that execute business rules derived from the rulebook | — |
| Name | A defined attribute. | _Human-readable substrate name_ |
| Technology | A defined attribute. | _Language/platform (PostgreSQL, Python, Go, Excel, OWL, PlantUML, etc.)_ |
| Relative Path | A defined attribute. | _Path within repo: execution-substrates/{technology}/_ |
| Injector Script | A defined attribute. | _Code generation script: inject-into-{technology}.py_ |
| Test Script | A defined attribute. | _Conformance test script: take-test.py_ |
| Is Production | True when an empty string. | _True if backed by an official Effortless tool; false if local implementation_ |
| Status | A defined attribute. | _Operational status (active, proof-of-concept, deprecated)_ |
| Description | A defined attribute. | _Purpose and capabilities_ |

<details open>
<summary>Example data</summary>

| Name | Technology | Relative Path | Injector Script | Test Script | Is Production | Status | Description |
|---|---|---|---|---|---|---|---|
| Binary (ARM64) | ARM64 Assembly | execution-substrates/binary/ | inject-into-binary.py | take-test.py | false | proof-of-concept | ARM64 assembly proof-of-concept; demonstrates lowest-level code generation |
| Python | Python | execution-substrates/python/ | inject-into-python.py | take-test.py | false | active | Local Python implementation: dataclasses + calculated field functions |
| Go | Go | execution-substrates/golang/ | inject-into-golang.py | take-test.py | false | active | Local Go implementation: structs + business logic |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Orchestration Component** | Central orchestration logic that coordinates rulebook loading, injection, and testing | — |
| Name | A defined attribute. | — |
| File Path | A defined attribute. | _Relative path from repo root_ |
| Language | A defined attribute. | — |
| Purpose | A defined attribute. | — |
| Dependencies | A defined attribute. | _Comma-separated list of other components_ |

<details open>
<summary>Example data</summary>

| Name | File Path | Language | Purpose | Dependencies |
|---|---|---|---|---|
| Shared Utilities | orchestration/shared.py | Python | Common functions: load_rulebook(), parse formulas, type mapping | None |
| Formula Parser | orchestration/formula_parser.py | Python | Parse and execute Excel-style formulas (IF, AND, OR, CONCAT, LEFT, RIGHT, etc.) | shared.py |
| Main Injector | orchestration/inject.py | Python | Dispatch rulebook to all substrate injectors; optionally clean generated files | shared.py, all substrate injectors |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Airtable Integration** | Airtable as input spoke: schema + data source pulled into rulebook | — |
| Name | A defined attribute. | — |
| File Path | A defined attribute. | — |
| Purpose | A defined attribute. | — |
| Role | A defined attribute. | _input (pull from Airtable), output (push to Airtable), or bidirectional_ |

<details open>
<summary>Example data</summary>

| Name | File Path | Purpose | Role |
|---|---|---|---|
| airtable-to-rulebook CLI | — | Official Effortless tool: exports schema + data from Airtable base to effortless-rulebook.json | input |
| rulebook-to-airtable CLI | — | Official Effortless tool: push formula updates back to Airtable (reverse sync) | output |
| ACME Corporation Bases | — | Demo Airtable bases for BASIC and ADVANCED ontologies; can be swapped at runtime | input |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Testing Framework** | Conformance testing: prove all substrates compute identically | — |
| Name | A defined attribute. | — |
| File Path | A defined attribute. | — |
| Purpose | A defined attribute. | — |
| Scope | A defined attribute. | _global (all substrates) or per-substrate_ |

<details open>
<summary>Example data</summary>

| Name | File Path | Purpose | Scope |
|---|---|---|---|
| Test Case Definitions | test-cases/*.yaml | YAML scenarios defining inputs and expected outputs; shared across all substrates | global |
| Substrate-Specific Take-Test | execution-substrates/{technology}/take-test.py | Load rulebook, execute test cases, compare results against expected | per-substrate |
| Test Orchestrator | orchestration/test-orchestrator.py | Run all substrate tests; aggregate pass/fail; identify conformance gaps | global |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rulebook Domain** | Customer ontologies: each domain has its own rulebook + substrate generation | — |
| Domain Name | A defined attribute. | — |
| Relative Path | A defined attribute. | _rulebook-examples/{domain}/ — each domain is a self-contained Effortless project_ |
| Rulebook Path | A defined attribute. | _Path to effortless-rulebook.json within domain_ |
| Complexity Level | A defined attribute. | _minimal, moderate, advanced, or philosophical_ |
| Table Count | A defined attribute. | — |
| Key Features | A defined attribute. | _Comma-separated: string concat, relationships, aggregations, IF logic, meta-ontology, etc._ |
| Purpose | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Domain Name | Relative Path | Rulebook Path | Complexity Level | Table Count | Key Features | Purpose |
|---|---|---|---|---|---|---|
| Customer FullName | rulebook-examples/customer-fullname/ | rulebook-examples/customer-fullname/effortless-rulebook/customer-fullname-rulebook.json | minimal | 1 | string concatenation | Hello World: demonstrates basic schema and calculated field (CONCAT formula) |
| Talisman BASIC | rulebook-examples/talisman-basic/ | rulebook-examples/talisman-basic/effortless-rulebook/talisman-basic-rulebook.json | moderate | 9 | relationships, aggregations, role-agent separation | Real workflow modeling: task management with assigned agents |
| Talisman ADVANCED | rulebook-examples/talisman-advanced/ | rulebook-examples/talisman-advanced/effortless-rulebook/talisman-advanced-rulebook.json | advanced | 9 | cross-entity lookups, conditional IF logic, complex aggregations | Business rule complexity: demonstrates advanced formula patterns |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Core Data Flow** | End-to-end flows from rulebook to execution and testing | — |
| Name | A defined attribute. | — |
| Steps | A defined attribute. | _Pipe-delimited sequence of steps_ |
| Triggers | A defined attribute. | _When this flow runs_ |
| Outputs | A defined attribute. | _Artifacts produced_ |

<details open>
<summary>Example data</summary>

| Name | Steps | Triggers | Outputs |
|---|---|---|---|
| Standard Rulebook → Substrates | Load rulebook.json \| Dispatch to all injectors \| Generate substrate code \| Package artifacts | User selects option A in orchestrator menu | Generated code in each execution-substrates/{tech}/ folder |
| Pull from Airtable | Call airtable-to-rulebook CLI \| Write effortless-rulebook.json \| Cache offline version | User selects option P (Pull & Inject) or changes base via option B | effortless-rulebook/effortless-rulebook.json, rulebook cache |
| Conformance Testing | Load test cases YAML \| Run each substrate's take-test.py \| Collect outputs \| Compare results \| Generate report | User selects option V (View Results) or automatic after injection | conformance-report.html, pass/fail matrix, mismatch details |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Project Configuration** | Configuration files and their purposes | — |
| File Name | A defined attribute. | — |
| File Path | A defined attribute. | — |
| Format | A defined attribute. | — |
| Purpose | A defined attribute. | — |
| Maintained by | A defined attribute. | _human (manual edits) or tool (auto-generated)_ |

<details open>
<summary>Example data</summary>

| File Name | File Path | Format | Purpose | Maintained by |
|---|---|---|---|---|
| effortless.json | ./effortless.json | JSON | SSoTme project definition: transpilers, versions, base IDs, API keys | human |
| CLAUDE.md | ./CLAUDE.md | Markdown | Project instructions and architecture overview for Claude Code | human |
| bases.json | orchestration/bases.json | JSON | Catalog of swappable Airtable bases with IDs, names, complexity levels | human |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Dependency** | External tools and their roles | — |
| Name | A defined attribute. | — |
| Version | A defined attribute. | — |
| Type | A defined attribute. | _Language, tool, service, or external API_ |
| Purpose | A defined attribute. | — |
| Required | True when an empty string. | — |

<details open>
<summary>Example data</summary>

| Name | Version | Type | Purpose | Required |
|---|---|---|---|---|
| Python | 3.8+ | Language | Orchestration scripts, injectors, test runners | true |
| Bash | latest | Language | Menu orchestration, subprocess management | true |
| Go | 1.19+ | Language | Go substrate code generation and testing | false |

</details>

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A project metadata **must** have a name and a purpose.
- An execution substrate **must** have a name; a technology; a relative path; and an injector script, and record whether it is a production.
- An orchestration component **must** have a name; a file path; a language; and a purpose.
- An airtable integration **must** have a name; a purpose; and a role.
- A testing framework **must** have a name; a file path; and a purpose.
- A rulebook domain **must** have a domain name; a relative path; and a rulebook path.
- A core data flow **must** have a name and a steps.
- A project configuration **must** have a file name; a file path; a format; and a purpose.
- A dependency **must** have a name; a type; and a purpose, and record whether it is required.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| — | _This rulebook defines no calculated fields; all data is raw._ |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
