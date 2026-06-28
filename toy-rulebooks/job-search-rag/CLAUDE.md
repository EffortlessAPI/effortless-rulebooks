# Job Search RAG — Effortless Project

## Authoritative Source: `effortless-rulebook/job-search-rag-rulebook.json` is HEAD

For this project, **`effortless-rulebook/job-search-rag-rulebook.json` is the single, authoritative source of truth.** Edit it directly. All other artifacts (Postgres SQL, Python, Go, Excel, OWL, etc.) are mechanically derived from it via `effortless build`.

### Airtable pull is disabled by default

The `airtabletorulebook` transpiler in `effortless.json` is set to `IsDisabled: true` / `Enabled: false` so that **a routine `effortless build` will never silently overwrite your JSON edits with whatever is in Airtable**.

If you want to re-enable Airtable pull as the source of truth for a single build, you must do it explicitly and with intent:

1. Confirm with the user that the rulebook JSON should be overwritten from Airtable.
2. Either run the transpiler one-off, or flip `IsDisabled` to `false` in `effortless.json`, run `effortless build`, then flip it back to `true`.

**Default rule for Claude / any agent operating in this project:** treat the JSON as authoritative; do not enable or invoke `airtabletorulebook` without explicit user consent on that specific turn. Memory or "we usually pull from Airtable" is not consent — every pull must be a fresh, in-context decision.

### Never silently revert `job-search-rag-rulebook.json`

**Treat `effortless-rulebook/job-search-rag-rulebook.json` as sacred.** It contains human-authored business rules and CANNOT be casually overwritten. Before running any command that could touch this file — `effortless build`, `effortless airtabletorulebook`, any transpiler that writes to it, any sync script, any `git checkout`/`git restore` against it — first run `git status` / `git diff` on it.

- If the file has uncommitted changes that you (the agent) did NOT just make this turn, **stop**. Ask the user before proceeding.
- It is only acceptable to let a regeneration touch this file when you are certain the *only* uncommitted edits in it are ones you just made in the current turn.

There is no upstream to "restore from." The JSON is the upstream.

## Quick Start

```bash
effortless build
```

## Key Files

- `effortless.json` — Project config and transpiler list
- `effortless-rulebook/job-search-rag-rulebook.json` — The rulebook (SSoT)
- `README.md` — Narrative documentation (if present)
- `execution-substrates/` or per-substrate dirs — Generated code

---

**The rulebook is the specification. Everything else is derived.**
