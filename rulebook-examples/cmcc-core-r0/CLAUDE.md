# cmcc-core-r0 — doctrine

This project follows the Effortless Rulebook (ERB) methodology. Load the `effortless-orchestrator` skill first. The repo-wide doctrine lives in the root `CLAUDE.md`; this file covers what is specific to this project.

## `effortless-rulebook/effortless-rulebook.json` is HEAD

**`effortless-rulebook/effortless-rulebook.json` is the single, authoritative source of truth for this project.** Edit it directly; everything else (Postgres SQL, RuleSpeak, any app) is derived by `effortless build`. Never edit generated output; trace a wrong artifact back to the rulebook entry that produced it.

Before any command that could touch the rulebook (`effortless build`, any sync, any `git checkout`/`restore` against it), run `git status` / `git diff` on it. If it carries uncommitted edits you did not make this turn, stop and ask. There is no upstream to restore from.

## What this project is

The runnable companion to the **CMCC-Core Representation Theorem** paper (an external research artifact, retrofitted into this repo's canonical project shape — see the root CLAUDE.md's "Active continuation" history for what that retrofit means generally). It specifies an R0 program (a warehouse-network Datalog-with-negation/aggregation/bitemporal-events spec, `r0/spec.json`), computes its permitted traces two independent ways (a reference evaluator + a SQLite oracle) to build an answer key, and checks three substrates (`transparent`, `tangled`, `interpreter`) against two gates: the answer key (all three pass, §11) and reflection — structural transparency (only `transparent` passes, §13; `tangled` and `interpreter` fail on purpose, as controls). See `README.md` for the full account, which is unchanged from the original artifact.

## Direction of truth — the one rule that must never be violated here

This project exists to demonstrate that three *independent* mechanisms (the reference evaluator, the SQLite oracle, and each substrate under test) agree on R0 semantics. **The rulebook must never become a fourth implementation of that semantics.**

- `EDBRelations`, `Commands`, `Rules`, `Transitions`, `TransitionEffects`, `TransitionEvents`, `IntegrityConstraints`, `Observables`, `SeedFacts`, `InputHistories`, `ReflectionConditions` and `Substrates`' own descriptive fields are a **structural transcription** of `r0/spec.json`, seeded by `effortless-rulebook/seed-spec-tables.py` — a faithful mirror, not a computation. This is a one-time authoring script, not part of the routine build/start loop: re-run it by hand only if `r0/spec.json`'s structure changes, then re-run `effortless build`.
- `AnswerKeyResults` and `ReflectionResults` are **witnessed rows**, pushed in by `conformance/ingest_conformance.py` reading a fresh `conformance/matrix.json` (itself produced only by `conformance/run.py`, i.e. the reference evaluator cross-checked against the independent SQLite oracle). Their `Outcome` column is never recomputed by a rulebook formula — it is copied verbatim from the pipeline's own `ok` / `conditions.*` values.
- `Substrates.AnswerKeyOk`, `.ReflectionOk` and `.Conformant` are the **one legitimate rulebook computation** here: an `AND`/`COUNTIFS` rollup over the already-witnessed `AnswerKeyResults` / `ReflectionResults` rows. This is bookkeeping over witnessed booleans, not a re-derivation of stratified-Datalog/bitemporal semantics, and it must stay that way — never add a calculated field that re-implements a rule's head/body logic, a stratification order, or a bitemporal snapshot.

If you ever find yourself writing a rulebook formula that evaluates a `Rules.BodyJson`/`AggJson` blob, stop — that is the forbidden fourth implementation.

## Known transpiler gotcha

`rulebook-to-rulespeak` (v2026.09.17.2011) treats a table **literally named `Constraints`** as a reserved word and fails the entire build with `Entity '' does not resolve to any table.` The table modeling `r0/spec.json`'s integrity constraints is therefore named `IntegrityConstraints`, not `Constraints`. Do not rename it back without first confirming the transpiler bug is fixed upstream.

## Shape

This is an ordinary governed project of the effortless-rulebooks repository: `effortless.json`, the hub under `effortless-rulebook/`, a typed `__meta__` table, this file, a README ending with the *Local transpiler bus* section, and an executable `./start.sh` that starts the project's intended local experience and prints its URLs. Readiness and consistency are derived in the root rulebook from witnessed slots; do not hand-assert them here.

The project also carries its own pre-existing, self-contained pipeline and test suite (`conformance/`, `r0/`, `substrates/`, `tools/`, `tests/`) from the original research artifact — that pipeline is the oracle the rulebook's `AnswerKeyResults`/`ReflectionResults` are fed from, and it is registered in the root rulebook's `TestSuites` as its own `pytest`-kind suite, separate from the repo's generic per-project conformance harness.

## App

`app/` (Express `server.js` on port 43306, Vite + React on port 43106, launched by `./start.sh`) reads views only: every value on screen is a column of a `vw_*` view in `erb_cmcc_core_r0` (`PGDATABASE` overrides), reshaped server-side (joined, never recomputed) for the conformance-matrix screen. It never recomputes a derived value in JS and never falls back silently; a missing view or unreachable database is a 500 that names what was expected, shown in the UI. `./init-db.sh` loads the rulebook-to-postgres output; `./start.sh` additionally rebuilds the Python pipeline and re-ingests `conformance/matrix.json` before serving.

## Loop

```bash
effortless build       # regenerate postgres/ + rulespeak/ from the rulebook
./init-db.sh            # once (or after a schema change): recreate erb_cmcc_core_r0
./start.sh              # rebuild substrates + matrix.json, run tests, ingest results, launch the app
```
