# Talisman's South Park Ontology Explainer, with a rulebook underneath

Jessica Talisman's [What is an Ontology?](https://jessicatalisman.substack.com/p/what-is-an-ontology)
explains ontologies through South Park, and her
[interactive explainer](https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/)
lets you switch facts and rules on and watch conclusions appear. This project is that
explainer rebuilt on one rulebook,
[`effortless-rulebook/talismans-southpark-ontology-rulebook.json`](effortless-rulebook/talismans-southpark-ontology-rulebook.json),
and then opened up: the same page runs on three interchangeable execution engines, and the
model behind it can be edited from the page, an admin, a generated rulebook editor, Excel,
or the file itself.

The page has two views, switched at the top:

- **The original explainer.** What her page shows and does, and nothing else: the graph,
  the fact list and her seven buttons, the five rule switches, the conclusions with their
  provenance lines, the contradiction alert, and the Add fact form taking any name or value.
- **With the rulebook tools.** The same page plus everything the rulebook brings, listed
  below. None of it changes how the explainer behaves.

The South Park characters are the work of Trey Parker and Matt Stone; the portraits are
Fandom images, linked (not copied) from the `Portraits` table. The page says it is a
rebuild of her explainer and links to the original.

```bash
effortless build                                                               # every substrate
python3 ../../scripts/erb_project.py talismans-southpark-ontology --reset-db   # load Postgres
./start.sh                                                                     # the app: http://localhost:43210/  (admin at /admin.html)
./start.sh --editor                                                            # optional: the rulebook editor, http://localhost:43612
```

## What the rulebook tools add

| Tool | What it adds | Where |
|---|---|---|
| Interchangeable engines | The page computed by the **Postgres views**, the **generated TypeScript**, or the **generated OWL and SHACL rules**. Same rows, same page; a switch picks the engine. | The "Computed by" bar |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | "Compare the engines" |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | Every computed value on the page and in the admin |
| Documentation | Every rule in plain language, in 16 languages. | `/rulespeak/rulespeak.html` |
| Admin | Every table of the rulebook as an editable grid: rules, properties and their wiring, wording, layouts, buttons, the cast, the engines themselves. Edits apply at once on every engine. Save writes the rulebook file. | `/admin.html` |
| Excel | Export the current rows as a workbook with every rule as a live formula (about 30 s). Edit it in Excel and import it again (about 2 s): the edits arrive as unsaved changes. | The admin's Export and Import |
| Rulebook editor | The platform's generated editor for tables, fields and formulas, in its own container. | `./start.sh --editor` |

The tools list itself is the rulebook's `RulebookTools` table, and the engines are its
`Engines` table.

## One set of rows, three engines

The app keeps one copy of the rulebook's stored rows as they are right now (the
*workspace*). Every click on the page and every admin edit writes there. An engine computes
everything else from those rows:

| Engine | How it computes | A click takes about |
|---|---|---|
| Postgres views | The rows are mirrored into the generated base tables; values are columns of the generated `vw_*` views. | 0.4 s |
| TypeScript | The generated `effortless-typescript/` SDK, imported into the app's own process: a click is one `evaluate(rows)` call over the live rows. No database, no other process, no files, nothing regenerated. | 0.1 s |
| OWL and SHACL rules | The generated `effortless-owl/` ontology and rules are run over individuals written from the rows, by the harness's own OWL runner (a SPARQL engine executing the rules in `sh:order`). | 2.2 s |

The app holds no rule of its own on any engine. A fact such as "Stan attends South Park
Elementary" is a row, so ticking it is a data change: the TypeScript engine calls the same
compiled SDK again with the new rows. OWL goes through
[`orchestration/live_substrates.py`](../../orchestration/live_substrates.py), a platform
tool that runs a substrate on live rows and compares substrates' answers with
`test-orchestrator.compare_values`. It works for any project that builds those substrates.

A request names its engine with the `X-ERB-Backend` header (`postgres`, `typescript`,
`owl`); the page remembers the choice.

## Does it behave like her page?

The tests take her live page as the reference and download it each run, so they also
report when her page changes.

| Test | What it compares | Result (2026-11-26) |
|---|---|---|
| [`compare-app-with-her-app.mjs`](testing/her-app/compare-app-with-her-app.mjs) | Her whole page script, run against a stub document, and this app's real server receive the same random clicks: fact and rule checkboxes, her seven buttons, Remove, and the Add fact form with the original cast **and names and values the rulebook has never seen** (Kenny, Mr Garrison, grade 6). After every click: the fact list, rule switches, conclusions panel (each card word for word, in order), alerts, the Add next fact button, the record summary, the button note and the form message. Run once per engine. | Postgres 115 of 115 screens, TypeScript 115 of 115, OWL 115 of 115 |
| [`compare-with-her-app.mjs`](testing/her-app/compare-with-her-app.mjs) | Her reasoner against the Postgres views, in every combination of the five rule switches for her starting facts, the two facts she suggests trying, and her buttons' fact changes. | 224 of 224 states, in her order too |
| [`testing/app/app.test.mjs`](testing/app/app.test.mjs) | What her page cannot do, on each engine: a new property wired to the Symmetric rule by adding a row, a new name that finds its portrait, new wording, Save, Discard, and the engines still agreeing afterwards. Also the candidate generator against the rulebook's stored rows, and Save's refusal to overwrite a file someone else changed. | 7 of 7 |
| [`scripts/tests/test_live_substrates.py`](../../scripts/tests/test_live_substrates.py) | The platform tool: TypeScript and OWL follow a change to the live rows, agree on every computed value, and a wrong value is named. | 4 of 4 |

```bash
node testing/her-app/compare-app-with-her-app.mjs --engine=all --walks=3 --steps=40
node testing/her-app/compare-with-her-app.mjs
node --test testing/app/app.test.mjs
python3 ../../scripts/tests/test_live_substrates.py
```

The app tests run their own server on their own database
(`erb_talismans_southpark_ontology_test`, created on first use), so they never disturb a
running app, and they save only into temporary copies of the rulebook.

## Conformance

Two levels, one rule for when two values are equal (`test-orchestrator.compare_values`):

- **Live, in the app.** "Compare the engines" computes every table on every available
  engine from the rows as they are at that moment and lists any value that differs. It
  found two real disagreements while this project was built, both recorded as findings
  against the OWL generator in the root rulebook (`cr-27-04`: a sum with a blank operand and
  a lookup through an empty link come out blank in the generated OWL rules; the formulas
  here were rewritten to avoid both).
- **The repo's harness.** `python3 ../../scripts/run-conformance.py talismans-southpark-ontology --reset-db --skip-build`
  grades each built substrate against the answer keys baked into the rulebook, and asks
  HermiT (through ROBOT, in Docker) whether the generated ontology is consistent.

| Run `run-talismans-southpark-ontology-20261126-030532` | Graded values | Score |
|---|---|---|
| effortless-postgres | 31,456 | 100% |
| effortless-typescript | 31,456 | 100% |
| effortless-python | 31,456 | 100% |
| effortless-xlsx | 31,456 | 100% |
| effortless-owl | 31,456 | 100%; HermiT: OWL 2 DL, consistent, no unsatisfiable class |

The run before it scored the same values but HermiT found the generated ontology
inconsistent: the OWL generator had declared the property-to-property links on
`Properties` to be inverses of one another. Those links now live in `PropertyAxioms`, which
the generator reads correctly; the generator's defect is finding `cr-28-01`.

## Editing the model

| Angle | What you can change | How it reaches the page |
|---|---|---|
| The page | Facts (any name, any value), rule switches, her buttons | At once |
| The admin (`/admin.html`) | Any stored value of any row of any table; add and delete rows | At once, on every engine |
| Excel | The stored rows, in a workbook where every rule is a live formula | As unsaved changes in the admin, to review and save. A changed formula, field or table is reported, not applied |
| The rulebook editor (`./start.sh --editor`) | Tables, fields, formulas, rows | Its Save writes the rulebook file; the app then says the file changed and reloads it on Discard; a formula change needs a rebuild |
| The rulebook file | Anything | Rebuild |

**Save** (in the admin) writes the stored rows back into the rulebook file and nothing
else: schema, descriptions and order stay as they are. It refuses if the file changed on
disk since the app loaded it. The computed copies that the build's first step
(`compile-rulebook`) bakes into the rows are dropped by Save and baked again by the next
build. **Discard** reloads the rows from the file. **Rebuild** runs `effortless build` and
reloads the database.

Rows that are data, and so editable without touching a formula: the rule explanations and
card wording, which properties are symmetric or typed (`Properties`), and which are inverse, chained or
disjoint (`PropertyAxioms`), her buttons and what each does (`Scenarios`, `ScenarioFactChanges`,
`ScenarioRuleSettings`), node positions (`NodeLayouts`), every string on the page
(`PageTexts`), the portraits and the names they answer to, the engines and the tools.
Adding a property row and ticking its `IsSymmetric` box is enough for the Symmetric rule to
apply to it, on all three engines.

### Candidate rows

A formula decides whether a fact holds, but only for a fact that is a row. So every fact a
rule could conclude over the cast is a row of `Statements`, every way a rule could conclude
it is a row of `Firings`, and every pair the disjoint rule watches is a row of
`DisjointChecks`. [`app/lib/candidates.mjs`](app/lib/candidates.mjs) is the one place those
rows come from: the rulebook's own candidates are exactly what it produces for the seed
cast (a test checks this), and the app calls it when a thing, a literal value, a property
or an axiom is added or removed. A new name adds about 90 candidate facts and a few
hundred ways to conclude them; the count grows with the cube of the cast because of the
property chain.

## How the model works

| Table | Rows | What it holds |
|---|---|---|
| `Graphs` | 1 | Wording variables and totals, and the next numbers a write needs |
| `Rules` | 5 | Inverse, Symmetric, Property chain, Domain and range, Disjoint; `IsEnabled` is the checkbox |
| `FiringKinds` | 6 | How each rule names itself on a conclusion card |
| `Things` | 9 | The individuals and classes; a new name becomes a row here |
| `Portraits`, `PortraitAliases` | 13, 32 | Her Fandom portraits and the names each answers to |
| `Properties`, `PropertyAxioms` | 10, 4 | Her properties, and the axioms between them (inverse, chain, disjoint) that the rules read |
| `Statements` | 270 | Every fact the page can state or conclude about the cast; `IsStated` is the checkbox |
| `Firings` | 465 | Every way a rule could conclude a fact; the first one in her reasoner's order is the card shown |
| `DisjointChecks` | 42 | Every pair the disjoint rule watches |
| `Scenarios`, `ScenarioFactChanges`, `ScenarioRuleSettings` | 7, 6, 10 | Her seven buttons |
| `NodeLayouts`, `LayoutSlots` | 26, 8 | Where the graph draws each node |
| `PageTexts` | about 100 | Every string on the page and in the admin |
| `Engines`, `RulebookTools` | 3, 8 | The engine switcher and the tools panel |

The seed data is her page's starting state: the ten teaching facts on, every rule off. A
value the page keeps one of per person (grade, role) is current when it was switched on
most recently, so switching grade 5 on puts grade 4 out of effect without deleting it.

The rules apply in three fixed strata: inverse and symmetric, then the property chain,
then domain and range. The conclusions are ordered the way her reasoner finds them: a
conclusion whose premises are all stated facts first, then those that needed a concluded
fact. This matches her page for every property it has; wiring that made a later rule feed
an earlier one would need repeated rounds.

## Differences from her page

- **Her prose and illustrations.** The long explanatory sections of her page, her article,
  and her Person, School and Student illustrations are hers and are not copied. Those three
  nodes use simple icons, and the page links to her article instead.
- **Attribution.** The page says it is a rebuild of her explainer and links to the original.
- **One shared state.** Every visitor to a running server sees and changes the same rows.
- **A link to a class.** Her page lets a class name (Person) be typed as an ordinary name.
  Here that fact is stored and shown, but no rule concludes anything from it.

## Known gaps

- **An Excel import carries rows, not rules.** The catalog's `xlsx-to-rulebook` syncs the
  workbook into a copy of the live rulebook and the app loads the stored rows from the
  result. A formula, field or table changed in the workbook is listed after the import and
  not applied; make that change in the rulebook editor or the file.
- **The rulebook editor has its own database.** An edit there reaches this app only after
  the editor's Save (which rewrites the rulebook file) and a Discard here (which reloads
  it). A formula or table change also needs a rebuild.
- **Schema changes are not live.** The admin edits rows. Fields, formulas and tables
  change in the rulebook editor or the file, followed by a rebuild.

## How the Excel import came to work

Until 2026-11-26 the import never answered for this rulebook. Its workbook holds 31,456
formula cells, and the catalog's `xlsx-to-rulebook` recalculated all of them in Python
before reading the file: 107 seconds and 4.1 GB on a laptop, against the published tool's
half a CPU, 1 GB and 120-second limit. The recalculation also produced nothing; its results
were thrown away by two coding errors, on every workbook of any size. The values were in the
file all along, because Excel and `rulebook-to-xlsx` both save each formula's result next to
it. The tool now reads them (root finding `cr-29-01`), and
`testing/app/app.test.mjs` exports, edits one cell and imports on every run.

## Building notes

`rulespeak/rulespeak.html` holds all 16 languages and is 13 MB, so it is generated by the
build and not committed; the per-language Markdown files (`rulespeak/rulespeak.<lang>.md`)
are.

The build's last step (`init-db`) fails on most builds: the CLI regenerates
`postgres-bootstrap/reset-rulebook-db.sh` without its executable bit (cr-20-02). The step
is last so every generator still runs; `erb_project.py --reset-db` loads the database
with `bash`.

`./start.sh` keeps the server in the foreground. On a machine with Postgres.app, a
detached server is refused by its client check, and the Postgres engine then reports
itself unavailable (the other engines keep working).

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
