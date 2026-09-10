# Effortless Banking

Community-bank Small Business Banking Client Manager demo, built as an
**Effortless Rulebook (ERB)** project. The hand-authored
[`effortless-rulebook/effortless-banking-rulebook.json`](effortless-rulebook/effortless-banking-rulebook.json)
is the single source of truth; `effortless build` regenerates Postgres
schema, functions, views, and seed data under [`postgres/`](postgres/)
and rebuilds the local DB via `reset-rulebook-db.sh`.

The platform models a commercial RM workflow that competes on relationship
depth: loan origination from inquiry through underwriting, committee
approval, closing, and funding; post-funding servicing, covenant
monitoring, risk-grade migration, and the document vault — with the four
surfaces (RM dashboard, branch portal, business-client portal, admin
console) all reading from the same DAG.

## Tables

- **Users** — Bank employees: RMs, underwriters, branch bankers, admins.
- **Businesses** — Small-business customers and prospects.
- **BeneficialOwners** — 25%+ owners and control persons (FinCEN CDD).
- **Contacts** — Non-owner officers, signers, AP clerks.
- **Accounts** — Deposit accounts (checking, savings, MM).
- **Loans** — Credit facilities from inquiry through payoff.
- **Covenants** — Recurring loan conditions tested on a tickler schedule.
- **RiskRatingHistory** — Time-series of risk-grade changes per loan.
- **Documents** — DocumentVault files attached to a Business or Loan.
- **Interactions** — Unified activity-log stream (notes, calls, tasks, system events).

## Layout

- [`effortless-rulebook/`](effortless-rulebook/) — the SSoT rulebook JSON
- [`postgres/`](postgres/) — generated SQL (`0*.sql`), `reset-rulebook-db.sh`, `*b-customize-*` seams
- [`bootstrap/`](bootstrap/) — narrative, glossary, vocabulary, diagrams, mock data
- [`effortless.json`](effortless.json) — build pipeline configuration

## Working on this project

```bash
effortless build   # regen SQL + drop + rebuild the local DB
psql -d first_valley_bank -c "\dv vw_*"
```

Schema changes go through the rulebook → `effortless build`. **No
migrations, no `ALTER TABLE`** — the DB is regenerated from scratch each
build. See [`CLAUDE.md`](CLAUDE.md) for full conventions.

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
