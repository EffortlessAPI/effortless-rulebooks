# ross-style-business-rules — Effortless Rulebook Project

This project follows the **Effortless Rulebook (ERB) methodology**. The rulebook is the single source of truth. All other artifacts are mechanically derived from it.

## Rulebook

**Location:** `effortless-rulebook/ross-style-business-rules-rulebook.json`

Four entities — Policies, Claimants, Incidents, Claims — encoding five declarative business rules in Ronald Ross's corrected wording.

## Building

```bash
effortless build
```

Runs all enabled transpilers. Currently: `rulebook-to-rulespeak` → `rulespeak/rulespeak.html` + `rulespeak.md`.

## Key Files

| File | Purpose |
|------|---------|
| `effortless.json` | Project config + transpiler pipeline |
| `CLAUDE.md` | This file |
| `effortless-rulebook/ross-style-business-rules-rulebook.json` | The rulebook (SSoT) |
| `rulespeak/rulespeak.html` | Generated: plain-English RuleSpeak (primary human deliverable) |
| `rulespeak/rulespeak.md` | Generated: same content in Markdown |

Do not edit generated files. Edit the rulebook and rebuild.

## App

`app/` (Express `server.js` + Vite/React) is the domain app: `./start.sh` serves the web UI on `http://localhost:43104` and the API on `http://localhost:43304` against database `erb_ross_style_business_rules` (override with `PGHOST`/`PGUSER`/`PGPASSWORD`/`PGPORT`/`PGDATABASE`). It reads views only — `vw_claims`, `vw_policies`, `vw_claimants`, `vw_incidents` via `GET /api/views[/:name]` — and never recomputes a rule verdict in app code; `GET /api/rules` returns the Claims field descriptions (rule wording, metadata) from the hub. A missing view or a failing `SELECT` is a 500 with the exact expected thing named, shown in the UI.

## Local transpiler bus (`127.0.0.1:4242`)

> **All 11 local transpilers are hosted by the effortless CLI itself.** Once you
> run `./start.sh` from the repo root (or `effortless serve -port 4242` there),
> every tool under `effortless-tools/<name>/` — `oss-rulebook-to-python`,
> `oss-rulebook-to-golang`, `oss-rulebook-to-cobol`, `oss-rulebook-to-owl`, and
> more — is a first-class route any `effortless build` can call. `GET /` lists
> them.
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