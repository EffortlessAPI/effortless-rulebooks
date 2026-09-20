# The GitHub Pages site (`docs/`)

`tools/generate_pages_site.py` emits a static documentation site into `docs/`, which
GitHub Pages serves from the `main` branch. It is idempotent: run it as many times as
you like, the output is byte-identical for an unchanged database.

```bash
python3 tools/generate_pages_site.py
```

It needs nothing but a loaded database. It does **not** build, does not touch the
rulebook JSON, and does not reload Postgres — run `bash init-db.sh` yourself first if
the model has changed since the last load.

To read it locally:

```bash
cd docs && python3 -m http.server 8931 --bind 127.0.0.1
# http://127.0.0.1:8931/
```

## The view is the contract, and the site says so on every number

Every value on every page is a column SELECTed from a generated `vw_*` view of
`erb_procedural_knowledge_ontology`. The generator computes nothing: no counting rows
in Python, no filtering a list to get a total, no reading the rulebook JSON. Each
metric renders the `view.column` it came from underneath itself, so any figure on the
site can be checked with one `SELECT`.

The connection string is `DATABASE_URL`, defaulting to the deterministically correct
`postgresql://postgres@localhost:5432/erb_procedural_knowledge_ontology`.

**There are no fallbacks.** A view that does not exist, or a column that does not exist
on a view it needs, raises `ViewContractError` naming exactly what was expected and
what the view actually has. An empty `vw_roles` raises rather than emitting an empty
site. A blank value renders as an em dash — that is presentation of a real blank, not a
substituted default.

Closure views (`vw_step_transitions_closure`, `..._closure_where_*`,
`vw_artifact_handoffs_closure`) are excluded by name wherever views are enumerated, and
`ViewReader.require()` refuses to read one as an entity.

## What the site covers today

| Path | Contents |
|---|---|
| `docs/index.html` | Landing page: the live section, and the named sections still to come |
| `docs/roles/index.html` | Every role in `vw_roles`, with holder and headline counts |
| `docs/roles/<role-id>/index.html` | One page per role |
| `docs/assets/site.css` | The shared stylesheet, emitted by the generator |

Each role page carries five sections:

- **Identity** — label, responsibility, owning organization, current holder, seniority,
  specialization, escalation backup, semantic type IRI.
- **Role metrics** — every count and every boolean witness that `vw_roles` computes for
  a role: assignments, awaited decisions, approval steps, capability tags, unresolved
  source mentions, unescalated refusals, ungrounded authority boundaries.
- **Who holds it, and when** — `vw_role_assignments` joined to `vw_agents`, with the
  validity period and the time-sensitive flags, because assignments are time-bounded
  and the model can answer who held a role on a past date.
- **The questions this role asks** — every `vw_role_questions` row in the role's own
  voice, with `why_it_matters`, the substrate-computed `witnessed_answer`, the
  predicate count, and the witness loop that made the question askable.
- **What this role may see** — the `vw_access_principals` row for the role, its policy /
  grant / visible-table counts, its sealed role schema, and every RLS policy with its
  predicate and denial-test count.

## Views the site reads

`vw_roles`, `vw_organizations`, `vw_agents`, `vw_role_assignments`, `vw_role_questions`,
`vw_witness_loops`, `vw_app_role_profiles`, `vw_app_routes`, `vw_app_nav_groups`,
`vw_app_route_questions`, `vw_access_principals`, `vw_access_policies`, `vw_role_schemas`.

## Known gap

**No view holds a per-role tally of questions asked versus answered.** `vw_roles` has no
`role_question_count` or `answered_role_question_count` rollup, and no other view
carries one (`vw_witness_loops.question_count` counts a loop, not a role; the
`question_count` on `vw_app_routes` counts a route). Rather than count the rows in
Python, the role page lists each question with its own
`vw_role_questions.is_answered` column and says plainly on the page that no tally is
stated. Adding those two rollups to `Roles` in the rulebook would close this.

## Adding a section

The site is a registry. `SECTIONS` in the generator holds one `Section` per top-level
area; each has a builder `build(db, out) -> {"pages": n, ...}` that writes its own pages
through `SiteWriter.write()`. The landing page renders from the registry, so a new
section appears on it automatically. `PLANNED_SECTIONS` names the areas that are not
built yet — they render as dashed cards, never as links to pages that do not exist.

Shared rendering helpers (`page`, `section_block`, `metric_grid`, `flag_grid`,
`data_table`, `definition_list`, `cell`, `source`) live in the same file and are meant
to be reused by every future section, so pages stay consistent as the site grows.

## Two rules the generator enforces in code

1. **`docs/ns/` is reserved** for the published ontology vocabulary, a separate
   workstream. `SiteWriter` refuses to write any path beneath it.
2. **Pruning is limited to what the generator itself emitted.** `docs/.pages-site-manifest.json`
   records the files of the last run; on the next run, files that were emitted before
   and are not emitted now are removed (so a deleted role does not leave a stale page),
   and nothing else is ever deleted.

`docs/.nojekyll` is emitted so GitHub Pages serves the tree literally rather than
running it through Jekyll.
