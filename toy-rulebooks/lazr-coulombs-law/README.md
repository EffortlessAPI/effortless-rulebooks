# Lazr Coulomb's Law

An Effortless Rulebook project modeling **Coulomb's Law** of electrostatic force.

## About

This project demonstrates ERB (Effortless Rulebook) applied to physics: the fundamental law describing the electrostatic force between charged particles.

**Coulomb's Law:** F = k·q₁·q₂/r²

Where:
- F = electrostatic force (Newtons)
- k = Coulomb's constant (8.99 × 10⁹ N·m²/C²)
- q₁, q₂ = charges (Coulombs)
- r = distance between charges (meters)

## Tables

- **Charges** — individual charged particles with position and magnitude
- **ChargeInteractions** — pairs of charges with computed force, distance, and interaction type (attractive/repulsive)

## Getting Started

The rulebook and Postgres database are already set up. Verify the schema:

```bash
psql -d lazr_coulombs_law -c "\d vw_*"
```

Query interactions:

```bash
psql -d lazr_coulombs_law -c "SELECT * FROM vw_chargeinteractions LIMIT 1"
```

To rebuild the database after editing the rulebook:

```bash
effortless build
```

This will regenerate the SQL under `postgres/` and reinitialize the database via `reset-rulebook-db.sh`.

## Project Structure

```
lazr-coulombs-law/
├── CLAUDE.md                          # ERB project conventions
├── README.md                           # This file
├── effortless.json                    # Build pipeline config
├── effortless-rulebook/
│   └── effortless-rulebook.json      # Single source of truth
└── postgres/
    ├── 00-bootstrap.sql
    ├── 01-drop-and-create-tables.sql
    ├── 02-create-functions.sql
    ├── 03-create-views.sql
    ├── 04-create-policies.sql
    ├── 05-insert-data.sql
    └── reset-rulebook-db.sh                     # One-command DB setup
```

## Database Name

`lazr_coulombs_law` — set via `DATABASE_URL` env var or `postgres/reset-rulebook-db.sh` default.

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
