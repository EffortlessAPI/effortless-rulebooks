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

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.
