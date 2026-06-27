# Simpson's Paradox — Effortless Project

This is an **Effortless Rulebook (ERB)** project. The SSoT is
`effortless-rulebook/simpsons-paradox-rulebook.json`. Rulebook-First — no Airtable.

## What this models

A **digital mirror** of the Simpson's Paradox domain. The entities are domain objects:
Studies, Treatments, Strata, CaseCells. The paradox emerges as a derived fact from
the DAG — it is NOT modeled directly. No `ReversalDetection` entity. No `PooledRate`
entity. The paradox falls out when you ask the right questions of the domain.

The model is built in 10 Leopold Loops. The `Loops` table IS the plan — each row
documents what domain concept is introduced and what natural-language question it answers.

## Relevant skills

- `effortless-orchestrator`, `effortless-schema`, `effortless-conventions`
- `effortless-setup-postgres`, `effortless-leopold-loop`
- `effortless-sql`, `effortless-query`, `effortless-rulespeak`

## NO MIGRATIONS

Local Postgres regenerated from scratch on every `effortless build` via
`effortless-postgres/init-db.sh`. Never write migrations. Edit rulebook → build.

## Build discipline

Before every `effortless build`, run `git status`. If dirty, pause and ask.
Never auto-commit generated files.
