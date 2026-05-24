<!-- GENERATED FILE — DO NOT EDIT. -->
<!-- Source: effortless-platform/effortless-rulebook/effortless-rulebook.json (table: `AppNavigation`) -->
<!-- Regenerate with: cd effortless-platform && effortless build -->
<!-- Generated: 2026-05-25T22:56:16Z -->

# Admin Portal Navigation

Primary navigation tree for the admin portal. Drives the left sidebar. Each node has a role gate and a target screen. This is the developer's narrative through a rulebook project.

- **Home** → `screen-home` _(min role: role-viewer)_ — What this project is, why it exists, who's working on it. Cards: rulebook stats, last build, last test pass-rate, active spokes.
  - **Project Flavours** → `screen-flavors` _(min role: role-viewer)_ — Classification of every demo rulebook — crud-template, computation-heavy, aggregation-heavy, graph-ontology, etc.
  - **Platform Features** → `screen-features` _(min role: role-viewer)_ — What ERB actually does — headline features (ADP, clean, hub-and-spoke, convergent build, substrate equivalence, …) and additional ones. Each row links to its per-feature README; developers can edit the catalog here.
- **Rulebook** _(min role: role-viewer)_ — The business semantics of the project — every table, field, formula, and sample row.
  - **Entities** → `screen-entities` _(min role: role-viewer)_ — List all entities. Click one to drill in to its fields, sample data, and computed columns.
  - **Formulas** → `screen-formulas` _(min role: role-viewer)_ — All calculated fields in one place. Click one to see its DAG (inputs → output) live from the rulebook.
  - **Relationships** → `screen-relationships` _(min role: role-viewer)_ — FK graph of the project. Hover a node to highlight its inbound and outbound relationships.
  - **Sample Data** → `screen-sample-data` _(min role: role-viewer)_ — What the project looks like populated. Editable for developers; read-only for viewers.
  - **Framing** → `screen-framing` _(min role: role-viewer)_ — The mistakes-to-avoid catalog and the axioms it protects. Read this before claiming any substrate is 'the reference'.
- **Substrates** → `screen-substrates` _(min role: role-viewer)_ — Every output substrate this project generates (Python, Go, Postgres, Excel, OWL, etc.). Click one to see the generated source and the conformance status.
  - **Add Tool** → `screen-add-tool` _(min role: role-developer)_ — Pick from the catalog of 15+ transpilers and install one into the active project. Same code path as `effortless -install` on the CLI.
- **Builds** → `screen-builds` _(min role: role-viewer)_ — Build history: when, what changed, which substrates regenerated, how long. Developers can trigger a build here.
- **Tests** → `screen-tests` _(min role: role-viewer)_ — Conformance matrix: which substrate computed which test case correctly. Drill in to see input → expected → actual per substrate.
- **Input Spokes** → `screen-input-spokes` _(min role: role-viewer)_ — Where edits come from: admin portal, Airtable, LLM, manual JSON. Pull / push controls live here.
- **Users** → `screen-users` _(min role: role-viewer)_ — Default dev/test users from the rulebook + their roles.
  - **Roles & Personas** → `screen-roles` _(min role: role-viewer)_ — Persona cards for each role — what they care about, where they land, who's assigned. Click anything to navigate.
- **Tech Tools** _(min role: role-developer)_ — Developer-only escape hatches. Raw Postgres, proxy logs, file system, manual injection. Not part of the daily workflow.
  - **Postgres Explorer** → `screen-tech-postgres` _(min role: role-developer)_ — Raw editor-DB browser: run SQL, inspect rows, drop/reset DB.
  - **ssotme-proxy** → `screen-tech-proxy` _(min role: role-developer)_ — Live proxy status: registered routes, recent calls, response sizes, last error per route.
  - **Files** → `screen-tech-files` _(min role: role-developer)_ — Project filesystem browser. View any generated or hand-written file in the active project.
  - **Rulebook JSON** → `screen-tech-json` _(min role: role-developer)_ — Raw rulebook JSON viewer/editor. Save here goes through the same write-through invariant as the UI.
  - **Reset Editor** → `screen-tech-reset` _(min role: role-developer)_ — Drop the editor Postgres DB and re-bootstrap from rulebook JSON. Safe — JSON is SSoT.

