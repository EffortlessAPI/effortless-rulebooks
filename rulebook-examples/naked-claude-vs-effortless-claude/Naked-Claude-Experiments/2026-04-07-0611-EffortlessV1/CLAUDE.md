# Project: naked-claude-vs-effortless-claude

This is an Effortless Rulebook (ERB) project.

## Airtable Base
- Base ID: appKLygCIXweUUKtM (v3 — switched 2026-04-06)
- Use the Airtable REST API for scalar field changes and all CRUD operations.
- Use OMNI (via Playwright) for formula, lookup, and rollup fields:
  `node ~/.claude/skills/effortless-airtable-omni/omni-send.mjs appKLygCIXweUUKtM '<prompt>'`
- First-time OMNI use requires login: `node ~/.claude/skills/effortless-airtable-omni/omni-send.mjs appKLygCIXweUUKtM --login`

## Schema Rules
- Query effortless-rulebook.json FIRST, generated code SECOND.
- NEVER edit generated files (00-05 in postgres/).
- Always read from vw_* views, never base tables.
- Ask permission before modifying the rulebook, Airtable, or running effortless build.

## ERB Skills
All conventions live in the effortless-* skills (not in memory files):
effortless-orchestrator, effortless-conventions, effortless-schema,
effortless-query, effortless-sql, effortless-pipeline, effortless-workflow,
effortless-airtable, effortless-airtable-omni, effortless-diagnostics.

When in doubt, consult **effortless-orchestrator** first — it routes to the other skills.

## Project Conventions
- Run `./start.sh` after updating app code.
