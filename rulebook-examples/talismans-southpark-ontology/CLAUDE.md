# talismans-southpark-ontology

This project follows the Effortless Rulebook (ERB) methodology. Load the
`effortless-orchestrator` skill first, and read the repo root's `CLAUDE.md`.

## What this project is

Jessica Talisman's interactive South Park Ontology Explainer
(https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/, companion to her
article https://jessicatalisman.substack.com/p/what-is-an-ontology), rebuilt on one
rulebook: `effortless-rulebook/talismans-southpark-ontology-rulebook.json`. The goal is a
page that looks and behaves exactly like hers, with every behaviour coming from the
rulebook's views and the same model also projected to TypeScript, Python, Excel, OWL and
documentation in many languages.

## Her page is the reference

- `node testing/her-app/compare-app-with-her-app.mjs` runs her whole page script against a
  stub document and this project's real server side by side, gives both the same random
  clicks, and fails on the first screen that differs. Run it after any change to the
  rulebook or `app/`.
- `node testing/her-app/compare-with-her-app.mjs` runs her reasoner and the Postgres views
  through 224 fact-and-rule states.
- Both download her page each time, so they also report when her page changes.

## The app holds no rules

`app/server.js` reads `vw_*` views and writes base tables; `app/public/app.js` draws what
the server sends. A conclusion, card, alert, count, button state, label or layout that the
page shows comes from a view column or a row (`PageTexts`, `Scenarios`, `NodeLayouts`...).
If a behaviour needs code that decides something about facts or rules, the rulebook is
missing a field: add it there. What the app may do itself is presentation: drawing,
dragging, selection, and interpreting a button's declared baseline and changes
(`runScenario` in `server.js`).
- When her page and the rulebook disagree, the rulebook is wrong until shown otherwise.
  Read her page's script (it is one inline script) before changing a rule.

## Rules for this project

- **Do not copy her prose or her embedded images into the repo.** Her article is
  subscriber-only and her page text and illustrations are hers. Short interface strings
  (headings, hints, rule explanations, source lines, card and alert wording) are
  `PageTexts` and other rows; her long explanatory sections are not. Portraits are linked
  to Fandom from `Things`, never downloaded.
- **Never present the page as hers.** The byline and footer say it is a rebuild of her
  explainer and link to the original.
- **Candidate rows are data.** `Statements`, `Firings` and `DisjointChecks` hold every
  candidate over the declared cast. A new individual, class or literal value needs its
  candidate rows added to the rulebook too (the cross product with the existing cast, and
  the firings the `Properties` wiring implies). No platform tool generates them yet; that
  is an open question for the owner, not something to solve with a script in this folder.
- **The rule strata are fixed**: inverse and symmetric, then the property chain, then
  domain and range. Changing `Properties` wiring so a later stratum feeds an earlier one
  silently gives wrong answers.
- **Loading the database**: the build's `init-db` step fails on most builds (cr-20-02,
  the reset script loses its executable bit). Use
  `python3 ../../scripts/erb_project.py talismans-southpark-ontology --reset-db`.
  The database is `erb_talismans_southpark_ontology`.

## Folders

- `bootstrap/` is the earlier chat session's output (a first rulebook read off a
  screenshot, its workbook, docs and OWL). It is history, not input to the build.
- `conversation.txt` and `original-post.txt` are that session's transcript and the
  LinkedIn post that pointed at her article.
