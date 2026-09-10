# naive-set-theory

Rulebook formalizing naive set theory with three-valued (Strong Kleene) membership and Rule 12 (NULL membership), which dissolves Russell's paradox into a single ungrounded membership fact.

**Rulebook:** [`effortless-rulebook/effortless-rulebook.json`](effortless-rulebook/effortless-rulebook.json) — 7 tables: `TruthValues`, `Connectives`, `TruthTableRows`, `SetRules`, `Sets`, `MembershipFacts`, `EvaluationSteps`.

## App

`app/` is an Express + React explorer that reads only the `vw_*` views of `erb_naive_set_theory`. One screen tells the story: the eight sets with their derived columns, a membership matrix of every `MembershipFacts` row (the single **N** cell is the ungrounded Russell fact `R ∈ R`, `is_null = ✓`), the Strong Kleene truth tables rendered from `TruthTableRows` per connective, the twelve `SetRules` with Rule 12 called out as the missing rule, and the `EvaluationSteps` fixed-point walk-through. A second tab browses every view as a raw table.

```bash
cd rulebook-examples/naive-set-theory
./reset-rulebook-db.sh   # once: load postgres/*.sql into erb_naive_set_theory
./start.sh     # web http://localhost:43102 · API http://localhost:43302/api/views
```

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
