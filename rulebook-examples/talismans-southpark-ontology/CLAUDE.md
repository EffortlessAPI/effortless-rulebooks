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

- `node testing/her-app/compare-with-her-app.mjs` runs her live page's own reasoner and
  this project's Postgres views through the same 224 states and fails on any difference in
  facts that hold, conclusion cards or contradiction alerts. Run it after any rulebook
  change. It downloads her page each time, so it also reports when her page changes.
- When her page and the rulebook disagree, the rulebook is wrong until shown otherwise.
  Read her page's script (it is one inline script) before changing a rule.

## Rules for this project

- **Do not copy her prose or her embedded images into the repo.** Her article is
  subscriber-only and her page text and illustrations are hers. Short interface strings
  the model needs (rule explanations, source lines, card wording, alert wording) are rows;
  long explanatory text is not. Portraits are linked to Fandom from `Things`, never
  downloaded.
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
