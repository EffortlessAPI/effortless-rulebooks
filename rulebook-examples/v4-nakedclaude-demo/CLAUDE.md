# V4 Nakedclaude Demo — Effortless Project

## Authoritative Source: Airtable base `appeUOAaOIdoqPSx3` is HEAD

For this project, **Airtable base `appeUOAaOIdoqPSx3` is the single, authoritative source of truth.** Edit the schema and data in Airtable. The local `effortless-rulebook/v4-nakedclaude-demo-rulebook.json` is a generated IR — every `effortless build` pulls fresh from Airtable and overwrites it.

This is a project-specific override of the default ERB pattern. Most projects in this repo are rulebook-first (JSON is HEAD). This one is Airtable-first because `airtabletorulebook` in `effortless.json` is `IsDisabled: false` / `Enabled: true`.

### What this means in practice

- **Do not hand-edit `v4-nakedclaude-demo-rulebook.json` and expect changes to stick.** The next `effortless build` will overwrite them with whatever is in Airtable.
- To change schema or business rules: change them in Airtable, then run `effortless build`.
- To freeze the JSON temporarily (e.g. to bisect a regression): flip `airtabletorulebook` to `IsDisabled: true` / `Enabled: false`, work against the frozen JSON, then re-enable when done.

### Reversibility

To switch this project to rulebook-first (JSON becomes HEAD): flip `airtabletorulebook` in `effortless.json` to `IsDisabled: true` / `Enabled: false`, then update this banner to the rulebook-first version (see e.g. `rulebook-examples/acme-llc/CLAUDE.md`). Until that flip happens, Airtable wins.

## Quick Start

```bash
effortless build
```

## Key Files

- `effortless.json` — Project config and transpiler list
- `effortless-rulebook/v4-nakedclaude-demo-rulebook.json` — The rulebook (SSoT)
- `README.md` — Narrative documentation (if present)
- `execution-substrates/` or per-substrate dirs — Generated code

---

**The rulebook is the specification. Everything else is derived.**
