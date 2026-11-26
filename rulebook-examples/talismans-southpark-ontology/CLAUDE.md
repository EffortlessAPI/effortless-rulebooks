# talismans-southpark-ontology

This project follows the Effortless Rulebook (ERB) methodology. Load the
`effortless-orchestrator` skill first, and read the repo root's `CLAUDE.md`.

## What this project is

Jessica Talisman's interactive South Park Ontology Explainer
(https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/, companion to her
article https://jessicatalisman.substack.com/p/what-is-an-ontology), rebuilt on one
rulebook: `effortless-rulebook/talismans-southpark-ontology-rulebook.json`. The page in
`app/` looks and behaves like hers and can be computed by three interchangeable engines
(Postgres views, generated TypeScript, generated OWL and SHACL rules). Around it sit the
rulebook tools: an admin over every table, Save back to the rulebook file, Excel export
and import, the generated rulebook editor, the explainer, and live conformance between the
engines. The README is the user-facing account; this file is how to work on it.

## Her page is the reference

- `node testing/her-app/compare-app-with-her-app.mjs --engine=all` runs her whole page
  script against a stub document and this project's real server side by side, gives both
  the same random clicks (including names and values the rulebook has never seen), and
  fails on the first screen that differs, once per engine. Run it after any change to the
  rulebook or `app/`.
- `node testing/her-app/compare-with-her-app.mjs` runs her reasoner and the Postgres views
  through 224 fact-and-rule states.
- `node --test testing/app/app.test.mjs` covers what her page cannot do (admin edits, Save,
  the candidate generator), on each engine, and the Excel round trip (export, edit one cell,
  import) through the catalog's `rulebook-to-xlsx` and `xlsx-to-rulebook`.
- The tests download her page each time, so they also report when her page changes. When
  her page and the rulebook disagree, the rulebook is wrong until shown otherwise: read
  her page's script (one inline script) before changing a rule.
- The app tests use their own database (`erb_talismans_southpark_ontology_test`) and a
  temporary copy of the rulebook (`RULEBOOK_PATH`). `compare-with-her-app.mjs` writes the
  project's own database directly: stop the app before running it.

## The app holds no rules, on any engine

- `app/lib/workspace.mjs` holds the stored rows as they are now. Every write goes there.
- `app/lib/engines.mjs` computes everything else: Postgres reads `vw_*` views over the
  mirrored rows; TypeScript imports the generated SDK (`effortless-typescript/dist/erb_sdk.js`)
  into the server and calls its `evaluate(rows)` on the live rows, in-process (owner,
  2026-11-26: a TypeScript engine that went through Python defeated the point, which is
  that the ontology simply runs wherever it is needed); OWL runs the generated rules through
  the platform's `orchestration/live_substrates.py`. An engine is a row of the rulebook's
  `Engines` table plus an entry in `engines.mjs`.
- `app/lib/explainer.mjs` and `app/server.js` read computed values from the engine and
  stored values from the workspace. A conclusion, card, alert, count, button state, label,
  layout or "next number" the page uses is always a field or a row. If a behaviour needs
  code that decides something about facts or rules, the rulebook is missing a field: add it
  there, and it then works on all three engines.
- What the app may do itself is presentation and plumbing: drawing, dragging, selection,
  sorting rows for display, interpreting a button's declared baseline and changes
  (`runScenario`), and giving a new name its rows.
- Do not add another way to run a substrate, compare values, or write the rulebook.
  TypeScript is computed by the SDK's own `evaluate()`, which is the same computation
  (`erbCompute` in `erb_runtime.ts`) the conformance runner `main.ts` performs; a fix to
  how TypeScript computes goes in the `rulebook-to-typescript` tool, never in the app.
  Running OWL on live rows and comparing answers belong in
  `orchestration/live_substrates.py` (it reuses the harness's runner contract, OWL runner
  and `compare_values`); writing the rulebook file belongs in `app/lib/hub.mjs`.

## Candidate rows are data, with one generator

`Statements`, `Firings` and `DisjointChecks` hold every candidate over the cast.
`app/lib/candidates.mjs` is the only code that decides which candidate rows exist; a test
proves the rulebook file's own candidates are exactly its output. Anything that adds or
removes a thing, a literal fact, a property or a property axiom must call `syncCandidates`
afterwards (the admin's write path and the Add fact form do).

## This rulebook is also the definition of the ontology form

`orchestration/ontology_form.py` (the tools `oss-rulebook-to-ontology-rulebook` and `oss-ontology-rulebook-to-rulebook`)
reads the model tables here (`Graphs`, `Rules`, `FiringKinds`, `Things`, `Portraits`, `PortraitAliases`, `Properties`,
`PropertyAxioms`, `Statements`, `Firings`, `DisjointChecks`) as the shape any rulebook is written into. After changing
any of their fields, run `python3 ../../scripts/tests/test_ontology_form.py` from here.

## Formula traps found here (all three engines must agree)

- **Arithmetic on a blank, and a lookup through an empty link.** Postgres, TypeScript and
  Python coerce a blank number to 0 and compute a lookup through an empty link as the
  target's formula on a blank row; the generated OWL rules leave both blank. Write formulas
  so that neither is reached: guard the sum (`IF({{Holds}}, 100000 + {{Key}}, 0)`) and make
  a calculated field blank on a blank row (`IF({{Kind}} = "", "", ...)`). "Compare the
  engines" finds these at once.
- **Wide order keys.** `Firings.OrderKey` and its relatives are datatype `number`: they
  reach 2e15, which overflows a Postgres `integer`.
- **The rule strata are fixed**: inverse and symmetric, then the property chain, then
  domain and range. A `PropertyAxioms` row that makes a later stratum feed an earlier one
  silently gives wrong answers.

## Rules for this project

- **Do not copy her prose or her embedded images into the repo.** Her article is
  subscriber-only and her page text and illustrations are hers. Short interface strings
  (headings, hints, rule explanations, source lines, card and alert wording) are
  `PageTexts` and other rows; her long explanatory sections are not. Portraits are linked
  to Fandom from `Portraits`, never downloaded.
- **Never present the page as hers.** The byline and footer say it is a rebuild of her
  explainer and link to the original.
- **The original view stays the original.** Anything new (a control, a count, a badge)
  goes in the tools layer (`.tools-only`, or inside `mode === 'tools'`), and its wording in
  `PageTexts`.
- **Save writes stored rows only** (`app/lib/hub.mjs`): never schema, never computed
  values, and never over a file that changed on disk since it was loaded.
- **Loading the database**: the build's `init-db` step fails on most builds (cr-20-02,
  the reset script loses its executable bit). Use
  `python3 ../../scripts/erb_project.py talismans-southpark-ontology --reset-db`.
  The database is `erb_talismans_southpark_ontology`.
- **Start the app in the foreground** (`./start.sh`). A detached server is refused by
  Postgres.app's client check and the Postgres engine reports itself unavailable.
- **The rulebook editor** runs in its own container with its own database, on ports
  43611 (API), 43612 (UI), 43613 (Postgres), pinned in `start.sh`. Its Save rewrites the
  rulebook file with computed copies baked in; the app notices the file changed and
  reloads on Discard.

## Folders

- `app/` the server, the page and the admin. `testing/` the tests. `effortless-*/`,
  `postgres-bootstrap/`, `rulespeak/`, `explainer/` are build outputs; never edit them.
- `bootstrap/` is the earlier chat session's output (a first rulebook read off a
  screenshot, its workbook, docs and OWL). It is history, not input to the build.
- `conversation.txt` and `original-post.txt` are that session's transcript and the
  LinkedIn post that pointed at her article.
