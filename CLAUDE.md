# Effortlessly Invariant Rulesbooks (ERB)

## CRITICAL: Two categorically different kinds of rulebooks live in this repo

There are **two** kinds of rulebook here and they are NOT the same thing. Confusing them will break the project. Read this before touching anything that references a rulebook path.

### 1. The top-level meta-rulebook — the orchestration tool itself

- **Path:** `./effortless-rulebook/effortless-rulebook.json`
- **What it is:** The rulebook describing the ERB orchestration repo / admin tool itself. It is committed, hand-edited via VS Code, and built with manual `effortless build` runs at the repo root. **The admin app IS its own admin tool** — there is no second-level wrapper around this rulebook.
- **Code that operates on it:** `tag-commit.sh`, `admin-portal/server.js` (`META_RULEBOOK`), `orchestration/rulebook-cache.py`, `orchestration/cache-manager.py`, `ssotme-proxy/server.py`, the root `effortless.json`. These all use the literal path `./effortless-rulebook/effortless-rulebook.json`.
- **Do not** rewrite these to look up the active demo. They are *categorically* not about the demos.

### 2. The per-project demo rulebooks — what the orchestration manages

- **Path:** `./rulebook-examples/<project>/effortless-rulebook/<project>-rulebook.json`
- **What they are:** The 8 example ontologies (`acme-corporation`, `acme-llc`, `customer-fullname`, `effortless-rulesbooks`, `is-everything-a-language`, `jessica-advanced`, `jessica-basic`, `star-trek`). These are containers/demos/widgets. The entire orchestration website/process/repo exists to manage *these*.
- **Code that operates on them:** `orchestration/orchestrate.sh` (via `get_domain_rulebook_path`), `orchestration/shared.py` (`get_rulebook_path`), all `execution-substrates/*/inject-into-*.py` injectors, all `execution-substrates/*/take-test.py` runners, `orchestration/generate-report.py`, `orchestration/test-orchestrator.py`, `devops/rebuild-on-trigger.sh`. These resolve the active demo via `orchestration/active-domain.txt` or the `ERB_RULEBOOK_PATH` env var.
- **Per-project filename is `<project>-rulebook.json`**, NOT `effortless-rulebook.json`. Each project's `effortless.json` (`ProjectTranspilers[*].CommandLine`) carries the `-o <project>-rulebook.json` flag accordingly.

### Rules for agents

- Before changing any code that references a rulebook path, decide which of the two categories above the code belongs to. The decision is non-negotiable — there is no "smart fallback" that handles both.
- The top-level `./effortless-rulebook/effortless-rulebook.json` is a **fixed literal path**. Never rewrite it to read the active domain.
- Per-project rulebook paths must be resolved dynamically — never hardcode `effortless-rulebook.json` for them. Use `get_domain_rulebook_path` (bash) or `get_rulebook_path()` (python).

<!-- rulebook-authoritative-banner -->
## Authoritative Source: `effortless-rulebook/effortless-rulebook.json` is HEAD

For every project under this repo (the top-level repo itself AND every `rulebook-examples/<project>/`), **`effortless-rulebook/effortless-rulebook.json` is the single, authoritative source of truth.** Edit it directly. All other artifacts (Postgres SQL, Python, Go, Excel, OWL, etc.) are mechanically derived from it via `effortless build`.

### Airtable pull is disabled by default

The `airtabletorulebook` transpiler in each project's `effortless.json` is set to `IsDisabled: true` / `Enabled: false` so that **a routine `effortless build` will never silently overwrite JSON edits with whatever is in Airtable**.

If you want to re-enable Airtable pull as the source of truth for a single build, you must do it explicitly and with intent:

1. Confirm with the user that the rulebook JSON should be overwritten from Airtable.
2. Either:
   - Run the transpiler one-off:
     ```bash
     effortless airtabletorulebook -o effortless-rulebook/effortless-rulebook.json -account airtable -p "view=Grid view"
     ```
   - Or flip `IsDisabled` back to `false` in `effortless.json`, run `effortless build`, then flip it back to `true`.

**Default rule for any agent operating in this repo:** treat the JSON as authoritative; do not enable or invoke `airtabletorulebook` without explicit user consent on that specific turn. Memory or "we usually pull from Airtable" is not consent — every pull must be a fresh, in-context decision.

### Never silently revert `effortless-rulebook.json`

**Treat `effortless-rulebook/effortless-rulebook.json` as sacred.** It contains human-authored business rules and CANNOT be casually overwritten. Before running any command that could touch this file — `effortless build`, `effortless airtabletorulebook`, any transpiler that writes to it, any sync script, any `git checkout`/`git restore` against it — first run `git status` / `git diff` on it.

- If the file has uncommitted changes that you (the agent) did NOT just make this turn, **stop**. Ask the user before proceeding.
- It is only acceptable to let a regeneration touch this file when you are certain the *only* uncommitted edits in it are ones you just made in the current turn.
- This applies to every `**/effortless-rulebook/effortless-rulebook.json` in this repo, not just the project-local one.

There is no upstream to "restore from." The JSON is the upstream.
<!-- /rulebook-authoritative-banner -->

## Architecture Overview

**Three Effortless tools** form a hub-and-spokes around Airtable: **airtable-to-rulebook**, **rulebook-to-postgres**, **rulebook-to-airtable**. The rulebook is a disposable IR. **PostgreSQL has no limitations** — it fully supports complex aggregations, JOINs, and all formula types. All other substrates in this repo are local implementations with demonstration gaps.

This repo follows a **three-layer architecture**:

```
┌─────────────────────────────────────────────────────────┐
│                    RULEBOOK (Core)                      │
│         effortless-rulebook/effortless-rulebook.json    │
│   - Single source of truth for all business logic      │
│   - Entity schemas with field types and descriptions   │
│   - Formulas for calculated fields (Excel-style)       │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  ORCHESTRATION                          │
│                  orchestration/                         │
│   - inject.py: Dispatches to all substrate injectors   │
│   - shared.py: Common utilities (load_rulebook, etc.)  │
│   - formula_parser.py: Parses Excel-style formulas     │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              EXECUTION SUBSTRATES                       │
│              execution-substrates/*/                    │
│   - postgres/: Effortless tool — no limitations        │
│   - python/: Local: Python dataclasses + calc functions│
│   - golang/: Local: Go structs + business logic        │
│   - binary/: ARM64 assembly (proof of concept)         │
│   - csv/xlsx/: Spreadsheet exports                     │
│   - uml/: PlantUML diagrams + OCL constraints          │
│   - owl/: RDF/OWL ontology                             │
│   - explain-dag/: JSON spec for derivation tracing     │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              CONFORMANCE TESTING                        │
│   - test-cases/: YAML test scenarios                   │
│   - Each substrate has take-test.py for verification   │
│   - Tests prove substrates compute identically         │
└─────────────────────────────────────────────────────────┘
```

## Key Principles

1. **Rulebook is the source of truth** - Never hardcode business logic in substrates. All formulas, types, and descriptions come from the rulebook JSON.

2. **Modify injectors, not generated files** - The `inject-into-*.py` scripts in each substrate folder generate the code. Edit those, not the output files.

3. **Formula semantics are Excel-compatible** - IF(), AND(), OR(), CONCAT(), LEFT(), RIGHT(), etc. The formula_parser.py handles parsing.

4. **Every substrate must pass the same tests** - Conformance testing ensures Python, Go, binary, etc. all produce identical results for the same inputs.

## Quick Reference

| Task | Location |
|------|----------|
| Change business logic | `effortless-rulebook/effortless-rulebook.json` |
| Change code generation | `execution-substrates/*/inject-into-*.py` |
| Add test cases | `test-cases/*.yaml` |
| Run all injectors | `orchestration/inject.py` |
| Shared utilities | `orchestration/shared.py` |

## Common Commands

```bash
# Regenerate all substrates from rulebook
python orchestration/inject.py

# Clean generated files
python orchestration/inject.py --clean

# Run tests for a specific substrate
cd execution-substrates/python && python take-test.py
```

---

## ssotme-proxy (localhost:4242)

`ssotme-proxy` is a local HTTP server that makes the repo's injector scripts
behave like first-class ssotme:// transpilers. It does NOT wrap a separate
mechanism — it IS the mechanism. The injectors already work; the proxy just
exposes them over HTTP using the ssotme fileset protocol.

### Wire protocol

Each transpiler is a **route**, not a body parameter:

```
POST http://localhost:4242/rulebook-to-python
POST http://localhost:4242/rulebook-to-postgres
POST http://localhost:4242/airtable-to-rulebook
```

The request body carries the ssotme fileset (input files as a JSON map of
`filename → content`). The response is a fileset of output files.

The server runs the corresponding injector script with:
- `ERB_RULEBOOK_PATH` — path to the input `effortless-rulebook.json`
- `ERB_OUTPUT_DIR` — the project-scoped output folder (e.g. `acme-llc/python/`)

The injector runs, writes its files to `ERB_OUTPUT_DIR`, and the proxy
collects the written files and returns them as a fileset response.

### Installing a transpiler into a project

The `effortless` CLI handles registration — never edit `effortless.json` by hand
to add transpiler entries. Run from the project folder:

```bash
cd rulebook-examples/acme-llc/python/
effortless -install http://localhost:4242/rulebook-to-python \
    -i ../effortless-rulebook/effortless-rulebook.json
```

The CLI writes the correct `CommandLine`, `RelativePath`, and input/output
mapping into `effortless.json`. After that, `effortless build` runs it like
any other transpiler and clean works automatically.

### Key distinction

The orchestrator's `[B] BUILD` calls `run_project_transpilers`, which reads
`effortless.json` transpiler entries that have `ProxyUrl` set and POSTs to
the proxy route. The proxy delegates to the same injector scripts that have
always existed — there is no duplication.

---
Local CLAUDE.md is also in ../api.effortlessapi.com/CLAUDE.md