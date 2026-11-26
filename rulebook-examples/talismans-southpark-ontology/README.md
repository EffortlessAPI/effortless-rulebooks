# Talisman's South Park Ontology Explainer, with a rulebook underneath

Jessica Talisman's [What is an Ontology?](https://jessicatalisman.substack.com/p/what-is-an-ontology)
explains ontologies through South Park, and her
[interactive explainer](https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/)
lets you switch facts and rules on and watch conclusions appear. This project is that
explainer rebuilt on one rulebook,
[`effortless-rulebook/talismans-southpark-ontology-rulebook.json`](effortless-rulebook/talismans-southpark-ontology-rulebook.json).

The page in [`app/`](app/) looks and behaves like hers, but runs no reasoner. Her facts,
rule switches, property wiring, buttons, node layouts, provenance wording and interface
text are rows, and every conclusion, card, alert, count and button state is a column of a
Postgres view. The server reads views and writes base tables; the page draws what it is
given. From the same file the build also produces TypeScript, Python, an Excel workbook,
OWL, documentation in 16 languages, and the explainer: hover any dotted value on the page
for its rule in plain words and the values it came from, down to the stated facts.

The South Park characters are the work of Trey Parker and Matt Stone; the portraits are
Fandom images, linked (not copied) from each `Things` row.

```bash
effortless build                                                               # every substrate
python3 ../../scripts/erb_project.py talismans-southpark-ontology --reset-db   # load Postgres
./start.sh                                                                     # the app on http://localhost:43210/
```

## Does it behave like her page?

Two tests take her live page as the reference. Both download it each run, so they also
report when her page changes.

| Test | What it compares | Result (2026-11-26) |
|---|---|---|
| [`compare-app-with-her-app.mjs`](testing/her-app/compare-app-with-her-app.mjs) | Her whole page script, run against a stub document, and this app's real server receive the same random clicks: fact and rule checkboxes, the seven buttons, Remove, and the Add fact form. After every click: the fact list, rule switches, conclusions panel (each card word for word, in order), alerts, the Add next fact button, the current-record summary, the button note and the form message. | 1,153 of 1,153 screens agree (20 walks of up to 60 clicks) |
| [`compare-with-her-app.mjs`](testing/her-app/compare-with-her-app.mjs) | Her reasoner against the database views, in every combination of the five rule switches for her starting facts, the two facts she suggests trying, and her buttons' fact changes: every fact that holds, every conclusion card, every alert. | 224 of 224 states agree, in her order too |

Each test was also run with one row broken in the database, to show it can fail:

| Break | Result |
|---|---|
| `range rule` wording changed to `range rules` | 64 of 224 states fail: exactly those where the range rule fires |
| "Show Stan's details" set to leave the domain rule off | The click test fails on the first press of that button |

```bash
node testing/her-app/compare-app-with-her-app.mjs [walks] [steps] [seed]   # starts its own server on :43298
node testing/her-app/compare-with-her-app.mjs
```

Both change the project's dev database while they run and put the seed state back at the
end.

## How the model works

| Table | Rows | What it holds |
|---|---|---|
| `Graphs` | 1 | Wording variables (source lines, empty-state message, literal syntax) and totals |
| `Rules` | 5 | Inverse, Symmetric, Property chain, Domain and range, Disjoint; `IsEnabled` is the checkbox |
| `FiringKinds` | 5 | How each rule names itself on a conclusion card |
| `Things` | 9 | The seven individuals her page draws and the classes Person and School, with portrait credits and crops |
| `Properties` | 10 | Her properties and the wiring every rule reads: inverse, symmetric, chain, domain, range, disjoint, datatype, one current value |
| `Statements` | 270 | Every fact the page can state or conclude about the cast; `IsStated` is the checkbox |
| `Firings` | 465 | Every way a rule could conclude a fact; the first one in her reasoner's order is the card shown |
| `DisjointChecks` | 42 | Every pair the disjoint rule watches |
| `Scenarios`, `ScenarioFactChanges`, `ScenarioRuleSettings` | 7, 6, 10 | Her seven buttons: a fact baseline, explicit fact changes, rule settings |
| `NodeLayouts`, `LayoutSlots` | 26, 8 | Where the graph draws each node, per layout |
| `PageTexts` | 67 | The page's wording |

The seed data is her page's starting state: the ten teaching facts on, every rule off. A
value the page keeps one of per person (grade, role) is current when it was switched on
most recently, so switching grade 5 on puts grade 4 out of effect without deleting it.

The rules apply in three fixed strata: inverse and symmetric, then the property chain,
then domain and range. That matches her page for every property it has; wiring that made a
later rule feed an earlier one would need repeated rounds.

## Differences from her page

- **New names and new numbers.** Her Add fact form takes any name and any number. Here,
  every fact the page can hold is a row over the declared cast (the seven people and
  places, and the values 10, 4, 5 and Student), written when the rulebook was authored. Any
  other name or value gets a message saying it is not in the rulebook yet; nothing in the
  platform generates the new rows.
- **Her prose and illustrations.** The long explanatory sections of her page, her article,
  and her Person, School and Student illustrations are hers and are not copied. Those three
  nodes use simple icons, and the page links to her article instead.
- **Attribution.** The page says it is a rebuild of her explainer and links to the original.
- **One shared state.** Every visitor to a running server sees and changes the same database.

## What is not done yet

- The rulebook editor (`effortless-rulebook-editor`) is not set up for this project.
- The TypeScript, Python, Excel and OWL outputs are built but not yet graded through the
  repo's conformance harness. (The page's explainer computes its witnesses with the
  TypeScript output.)

## Building notes

`rulespeak/rulespeak.html` holds all 16 languages and is 13 MB, so it is generated by the
build and not committed; the per-language Markdown files (`rulespeak/rulespeak.<lang>.md`)
are.

The build's last step (`init-db`) fails on most builds: the CLI regenerates
`postgres-bootstrap/reset-rulebook-db.sh` without its executable bit (cr-20-02). The step
is last so every generator still runs; `erb_project.py --reset-db` loads the database
with `bash`.

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
