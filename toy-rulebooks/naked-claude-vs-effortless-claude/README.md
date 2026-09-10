# naked-claude-vs-effortless-claude

An experiment tree comparing a hand-coded ("naked Claude") build with a rulebook-first build of the same brief. It deliberately carries no rulebook of its own and is flagged `IsIntentionalException` in the root rulebook; the graded projects are its `nakedclaude-v1` … `v4` siblings.

## Rebuild

```bash
effortless build   # regenerates every registered output from the rulebook
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
