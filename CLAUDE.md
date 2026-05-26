# Avoid Silent Fallbacks

If a file isn't where you expect, fail loudly with the exact expected path rather than silently checking a second location or defaulting to a "legacy" name. Silent fallbacks hide bugs.

Watch for: `if not os.path.exists` returning `{}`, `try/except` swallowing missing paths, or anything that quietly accepts the wrong value when it should error.

## Defaults derived from the SSoT are NOT fallbacks

A **default** computed from the single source of truth is fine — even encouraged. The thing that makes a fallback bad is that it silently substitutes a *guess* for the right value. A default derived from `orchestration/active-domain.txt` is not a guess; it's the deterministically-correct value computed from the SSoT. The environment variable just exists for override.

- ✅ `os.environ.get("DATABASE_URL") or f"postgresql://postgres@localhost:5432/erb_{active_domain.replace('-','_')}"` — the default IS the right answer (matches the formula in `orchestrate.sh`); DATABASE_URL only overrides.
- ✅ `os.environ.get("ERB_TESTING_DIR") or f"{repo_root}/rulebook-examples/{active_domain}/testing"` — same shape.
- ❌ `os.environ.get("DATABASE_URL") or "postgresql://postgres@localhost:5432/postgres"` — defaults to a generic DB that has nothing to do with the active domain. This is the kind of fallback that masks bugs as 100% conformance.
- ❌ `path or "/some/legacy/location"` when the active path is missing — guessing.

The test: if your default would still be correct after the env var was unset by accident, it's a default. If it would silently run against the wrong thing, it's a fallback — delete it and fail loudly instead.

---

# THE PROJECT RULEBOOK ≠ A DEMO RULEBOOK

**This repo is a project that WRAPS a bunch of demo rulebooks. The project rulebook is the PARENT. The demo rulebooks are CHILDREN. They are NEVER mixed.**

- The **project rulebook** is `./effortless-platform/effortless-rulebook/effortless-rulebook.json`. It describes ERB itself — the orchestration tool, the admin portal, the build pipeline, the conformance testing framework. Things like `UserRoles`, `AppUsers`, `AppPermissions`, `AppNavigation`, `AppScreens`, `AppAPIs`, `AddToolCatalog`, `BuildPipeline`, `AdminPortalRuntime` belong **here and only here**. They are about *the wrapper*. (See `./effortless-platform/README.md` — the platform folder is the wrapper "eating its own dog food.")
- The **demo rulebooks** are `./rulebook-examples/<domain>/effortless-rulebook/<domain>-rulebook.json`. Each one describes ONE business domain (acme-llc's customers, star-trek's episodes, jessica-basic's tasks, etc.). They contain **only that domain's tables**. They never contain portal config.

The admin portal reads portal config from the **project rulebook** (always). It reads the active demo's domain data from the **demo rulebook** for whichever domain is in `active-domain.txt`. These are two separate file reads, two separate concerns. Never merge them. Never put portal entities in a demo rulebook. Never put a domain's business tables in the project rulebook.

**If you find portal-config tables (`UserRoles` / `AppUsers` / etc.) in a demo rulebook, that is a bug — remove them and put them where they belong (the project rulebook). If you find a domain's business tables (`Customers`, `Episodes`, etc.) in the project rulebook, same bug — remove them.**

---

# THE ADMIN PORTAL ≠ A DOMAIN (same category error, different surface)

**The admin portal is like Microsoft Word. A domain (acme-llc, star-trek, jessica-basic, etc.) is like a .docx document.** Word's install directory is not named after any document. A document is not named after the app. They are categorically different things and must never be mixed in any identifier — DB name, file path, function name, env var, table prefix, anything.

**Database naming (enforced):**
- `erb_admin_portal` — the admin portal's own database. **Singular.** Holds portal config (`AppUsers`, `AppRoles`, `AppNav`, etc.). The word `admin` appears here and **only** here. Never with a domain suffix.
- `erb_<domain>` — per-domain document database (`erb_acme_llc`, `erb_star_trek`, `erb_jessica_basic`, …). Holds that domain's business data. The word `admin` **never** appears in these names.

**Forbidden pattern: `erb_admin_<domain>`.** This is the category-error red flag. If you see it in code, the code is wrong. If you are about to write it, stop — the naming itself proves you have conflated the app with a document.

The admin portal opens two connections for two different reasons:
1. To `erb_admin_portal` — to read/write its own state (which user is logged in, navigation, etc.).
2. To `erb_<domain>` — to edit the document currently open (whichever domain is in `active-domain.txt`).

Two connections. Two purposes. Two categories. Same logic as the rulebook split above.

**Rule for agents:** When CLAUDE.md declares a category boundary and the code appears to cross it, the code is the suspect — not the doctrine. Flag it as "this looks like a category-error bug in the code" rather than reporting it as fact. Do not pattern-match on bad names and assume they describe reality.

---

# Effortlessly Invariant Rulesbooks (ERB)

## Two categorically different kinds of rulebooks live in this repo

There are **two** kinds of rulebook here and they are NOT the same thing. Confusing them will break the project.

### 1. The top-level orchestration rulebook — the admin tool itself

- **Path (literal, fixed):** `./effortless-platform/effortless-rulebook/effortless-rulebook.json`
- **What it is:** The rulebook describing the ERB orchestration repo / admin tool itself. Hand-edited, built with `effortless build` run from inside `./effortless-platform/`. The admin app IS its own admin tool — no second-level wrapper.
- **Code that operates on it:** `tag-commit.sh`, `effortless-platform/admin-portal/server.js`, `orchestration/rulebook-cache.py`, `orchestration/cache-manager.py`, `effortless-platform/ssotme-proxy/server.py`, `effortless-platform/effortless.json`. These use the literal path.
- Do not rewrite these to look up the active demo. They are *categorically* not about the demos.
- **Why under `effortless-platform/`:** the platform folder is the wrapper "eating its own dog food" — the admin portal, postgres scripts, ssotme-proxy, the meta-rulebook and its own `effortless.json` live there together because they are *one* effortless project that happens to manage all the other ones. They are NOT required to run the tooling itself. See `./effortless-platform/README.md`.

### 2. The per-project demo rulebooks — what the orchestration manages

- **Path:** `./rulebook-examples/<project>/effortless-rulebook/<project>-rulebook.json`
- **What they are:** Example ontologies (`acme-corporation`, `acme-llc`, `customer-fullname`, `effortless-rulesbooks`, `is-everything-a-language`, `jessica-advanced`, `jessica-basic`, `star-trek`). The entire orchestration website/process/repo exists to manage *these*.
- **Code that operates on them:** `orchestration/orchestrate.sh` (via `get_domain_rulebook_path`), `orchestration/shared.py` (`get_rulebook_path`), all `execution-substrates/*/inject-into-*.py`, all `execution-substrates/*/take-test.py`, `orchestration/generate-report.py`, `orchestration/test-orchestrator.py`, `devops/rebuild-on-trigger.sh`. These resolve the active demo via `orchestration/active-domain.txt` or `ERB_RULEBOOK_PATH`.
- **Filename is `<project>-rulebook.json`**, NOT `effortless-rulebook.json`. Each project's `effortless.json` carries `-o <project>-rulebook.json` accordingly.

### Rules for agents

- Before changing any code that references a rulebook path, decide which of the two categories above the code belongs to. There is no "smart fallback" that handles both.
- The top-level path is a fixed literal. Never rewrite it to read the active domain.
- Per-project rulebook paths must be resolved dynamically — never hardcode `effortless-rulebook.json` for them. Use `get_domain_rulebook_path` (bash) or `get_rulebook_path()` (python).

## Authoritative Source: rulebook JSON is HEAD

For every project (the top-level repo AND every `rulebook-examples/<project>/`), the rulebook JSON is the single, authoritative source of truth. Edit it directly. All other artifacts (Postgres SQL, Python, Go, Excel, OWL, etc.) are mechanically derived via `effortless build`.

### Airtable pull is disabled by default

The `airtabletorulebook` transpiler in each project's `effortless.json` is set to `IsDisabled: true` so a routine `effortless build` will never silently overwrite JSON edits with whatever is in Airtable.

If you want to re-enable Airtable pull for a single build, do it explicitly:

1. Confirm with the user that the rulebook JSON should be overwritten from Airtable.
2. Either run the transpiler one-off, or flip `IsDisabled` to `false` in `effortless.json`, run `effortless build`, then flip it back to `true`.

Default rule: treat the JSON as authoritative; do not invoke `airtabletorulebook` without explicit user consent on that specific turn.

### Never silently revert a rulebook JSON

Treat each rulebook JSON as sacred — it contains human-authored business rules. Before running any command that could touch the file (`effortless build`, `airtabletorulebook`, any sync script, any `git checkout`/`git restore`), run `git status` / `git diff` on it first.

- If the file has uncommitted changes you did NOT just make this turn, **stop** and ask before proceeding.
- It is only acceptable to let a regeneration touch this file when the only uncommitted edits in it are ones you just made this turn.

There is no upstream to "restore from." The JSON is the upstream.

## Architecture Overview

**Three Effortless tools** form a hub-and-spokes around Airtable: **airtable-to-rulebook**, **rulebook-to-postgres**, **rulebook-to-airtable**. The rulebook is a disposable IR. **PostgreSQL has no limitations** — it fully supports complex aggregations, JOINs, and all formula types. Other substrates in this repo are local implementations with demonstration gaps.

Three-layer architecture:

```
┌─────────────────────────────────────────────────────────┐
│                    RULEBOOK (Core)                      │
│   - Single source of truth for all business logic       │
│   - Entity schemas with field types and descriptions    │
│   - Formulas for calculated fields (Excel-style)        │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  ORCHESTRATION                          │
│   - orchestrate.sh: BUILD = generate + test + regen +   │
│                    open report (always; no skips)       │
│   - shared.py: Common utilities                         │
│   - formula_parser.py: Parses Excel-style formulas      │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              EXECUTION SUBSTRATES                       │
│              execution-substrates/*/                    │
│   - Each substrate MUST have: inject-substrate.sh,      │
│     take-test.sh, create-substrate-report.sh            │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              CONFORMANCE TESTING                        │
│   - Tests prove all substrates compute identically      │
│   - Reports regenerate AND open on every build          │
└─────────────────────────────────────────────────────────┘
```

## Build = generate + test + regen report + open. ALWAYS.

There is no "build without testing." There is no "regenerate the report without running tests." The menu has one rebuild path:

- **[B] BUILD** runs all transpilers, then runs every substrate's conformance test, then regenerates `orchestration-report.html`, then opens it.
- **[1-9] (individual transpiler)** runs that one transpiler, then runs that substrate's conformance test, then regenerates the report, then opens it.

There is no `[T]est` menu option. Testing is part of build. If a substrate has no `take-test.sh`, that is a bug to fix — not a case to handle.

## Key Principles

1. **Rulebook is the source of truth** — never hardcode business logic in substrates.
2. **Modify injectors, not generated files** — `inject-into-*.py` scripts generate the code.
3. **Formula semantics are Excel-compatible** — IF(), AND(), OR(), CONCAT(), LEFT(), RIGHT(), etc.
4. **Every substrate must pass the same tests** — conformance ensures identical results.

## ssotme-proxy (localhost:4242)

`ssotme-proxy` is a local HTTP server that makes the repo's injector scripts behave like first-class ssotme:// transpilers. It IS the mechanism, not a wrapper.

### Wire protocol

Each transpiler is a route:

```
POST http://localhost:4242/rulebook-to-python
POST http://localhost:4242/rulebook-to-postgres
POST http://localhost:4242/airtable-to-rulebook
```

The server runs the corresponding injector with:
- `ERB_RULEBOOK_PATH` — input rulebook path
- `ERB_OUTPUT_DIR` — output folder (e.g. `acme-llc/python/`)

### Installing a transpiler into a project

The `effortless` CLI handles registration — never edit `effortless.json` by hand to add transpiler entries:

```bash
cd rulebook-examples/acme-llc/python/
effortless -install http://localhost:4242/rulebook-to-python \
    -i ../effortless-rulebook/acme-llc-rulebook.json
```
