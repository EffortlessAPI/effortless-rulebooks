<!-- GENERATED FILE — DO NOT EDIT. -->
<!-- Source: effortless-platform/effortless-rulebook/effortless-rulebook.json (table: `AppScreens`) -->
<!-- Regenerate with: cd effortless-platform && effortless build -->

# Admin Portal Screens

Every screen in the admin portal. Each screen names the entities it reads/writes, the role it requires, and the story it tells.

| ID | Path | Title | Min Role | Layout | Reads | Writes | Primary Action |
| --- | --- | --- | --- | --- | --- | --- | --- |
| screen-home | / | Home | role-viewer | dashboard | ProjectMetadata,RulebookProjects,RulebookSourceSpokes,ExecutionSubstrates | — | Switch project |
| screen-entities | /developer/rulebook/entities | Entities | role-viewer | split-detail | <active-project-rulebook> | <active-project-rulebook> | Add entity |
| screen-formulas | /developer/rulebook/formulas | Formulas | role-viewer | split-detail | <active-project-rulebook> | <active-project-rulebook> | Edit formula |
| screen-relationships | /developer/rulebook/relationships | Relationships | role-viewer | grid | <active-project-rulebook> | — | — |
| screen-sample-data | /developer/rulebook/data | Sample Data | role-viewer | grid | <active-project-rulebook>.data | <active-project-rulebook>.data | Add row |
| screen-substrates | /developer/substrates | Substrates | role-viewer | split-detail | ExecutionSubstrates,SsotmeProxy | — | Rebuild substrate |
| screen-add-tool | /developer/tools/add | Add Tool | role-developer | grid | AddToolCatalog,SsotmeProxy | <active-project>/effortless.json (via effortless CLI) | Install |
| screen-builds | /developer/builds | Builds | role-viewer | list | BuildHistory | BuildHistory | Trigger build |
| screen-tests | /developer/tests | Tests | role-viewer | grid | TestingFramework,TestRuns | TestRuns | Run all tests |
| screen-input-spokes | /developer/spokes | Input Spokes | role-viewer | list | RulebookSourceSpokes | — | Pull from spoke |
| screen-users | /admin/users | Users | role-viewer | list | AppUsers,UserRoles | AppUsers | Add user |
| screen-tech-postgres | /developer/tech/postgres | Postgres Explorer | role-developer | editor | <editor-postgres-tables> | <editor-postgres-tables> | Run query |
| screen-tech-proxy | /developer/tech/proxy | ssotme-proxy | role-developer | list | SsotmeProxy | — | Ping proxy |
| screen-tech-files | /developer/tech/files | Project Files | role-developer | split-detail | <project-filesystem> | — | — |
| screen-tech-json | /developer/tech/rulebook-json | Raw Rulebook JSON | role-developer | editor | <active-project-rulebook> | <active-project-rulebook> | Save JSON |
| screen-tech-reset | /developer/tech/reset | Reset Editor | role-developer | dashboard | — | — | Reset now |
| screen-framing | /developer/rulebook/framing | Framing | role-viewer | split-detail | FramingInvariants,OntologyAxioms | — | — |
| screen-roles | /admin/users/roles | Roles & Personas | role-viewer | split-detail | UserRoles,AppUsers,RoleScreenHints | UserRoles | — |
| screen-flavors | /projects/flavors | Project Flavours | role-viewer | grid | RulebookFlavors,RulebookProjects | — | — |
| screen-features | /features | Platform Features | role-viewer | split-detail | PlatformFeatures,OntologyAxioms | PlatformFeatures | Edit feature |
| screen-docs-home | /docs | Docs Home | role-viewer | dashboard | FramingInvariants,OntologyAxioms,FieldTypeTaxonomy,Glossary | — | — |
| screen-docs-framing | /docs/framing | Framing Invariants | role-viewer | split-detail | FramingInvariants | — | — |
| screen-docs-method | /docs/methodology | Methodology | role-viewer | split-detail | OntologyAxioms,FramingInvariants | — | — |
| screen-docs-ftypes | /docs/field-types | Field Types | role-viewer | split-detail | FieldTypeTaxonomy | — | — |
| screen-docs-glossary | /docs/glossary | Glossary | role-viewer | list | Glossary | — | — |
| screen-admin-landing | /admin | Admin | role-viewer | dashboard | AppUsers,UserRoles,AppPermissions | — | — |
| screen-admin-perms | /admin/permissions | Permissions | role-viewer | list | AppPermissions,UserRoles | — | — |
| screen-admin-nav | /admin/navigation | Navigation | role-developer | list | AppNavigation,AppScreens | — | — |
| screen-admin-screens | /admin/screens | Screens | role-developer | list | AppScreens | — | — |

## Story

### Home

Land here on ./start.sh. Cards: current project, rulebook size, # substrates, last build time, last test pass-rate, which spokes are active, who you're signed in as.

### Entities

Left: list of every entity in the active project's rulebook. Right: selected entity's fields, formulas, sample rows. Developer can add/rename/delete fields inline; viewer sees the same with controls disabled.

### Formulas

Every calculated field across all entities. Drill in to see live DAG visualization (inputs → intermediates → output) sourced from the rulebook's explain-dag substrate. Developer can edit Excel-style formula in place; portal auto-validates against the parser.

### Relationships

FK graph (interactive). Hover a node to highlight its inbound/outbound edges. Click an edge to see which formula or column declares it.

### Sample Data

Spreadsheet-style view of the data block of each entity. Developer can edit cells; computed columns are read-only and show the resolved value with a hoverable derivation popover.

### Substrates

Left: every substrate this project emits (with last-build timestamp + conformance status). Right: file tree of generated output + a preview pane for any file. Developer can trigger 'Rebuild just this substrate'.

### Add Tool

Browse the 15+ transpilers in the catalog. Click one, choose output path, hit Install. Portal shells out to `effortless -install <proxy-url>` so the result is byte-identical to the CLI path. Tool then shows up in /substrates and is ready for the next build.

### Builds

Chronological list of builds. Click one to see which transpilers ran, duration each, stdout/stderr per route, and which files changed. Trigger-build button is gated by CanRunBuilds.

### Tests

Matrix: rows = test cases, columns = substrates, cells = pass/fail. Click a failing cell to see input, expected, actual, and a diff. Trigger-tests button is gated by CanRunBuilds.

### Input Spokes

List of every configured input spoke for this project. Status (last-pulled time, last-error). Developer can trigger a pull from any spoke.

### Users

Default dev/test users from the rulebook + roles. Developer can add new users (saved through the write-through invariant, so they end up in rulebook JSON).

### Postgres Explorer

Raw SQL console + table browser for the editor Postgres DB. Developer-only escape hatch. Background banner reminds: rulebook JSON is the SSoT, this DB is rebuildable.

### ssotme-proxy

Live status of localhost:4242: registered routes, recent calls (route, duration, status, response size), last error per route. Restart button.

### Project Files

Browse the active project folder. View any file. Read-only — prevents drift from rulebook.

### Raw Rulebook JSON

Monaco editor on the raw effortless-rulebook.json. Save goes through the write-through invariant. Useful for bulk edits.

### Reset Editor

One-button reset: drops the editor Postgres DB and re-bootstraps from rulebook JSON. The reassurance screen — makes the JSON-as-SSoT promise tangible.

### Framing

Left: FramingInvariants — the mistakes-to-avoid catalog. Right: the wrong framing, the correct framing, why, and the axiom it violates. Toggle to Axioms tab to see the positive-form invariants. Reviewer's favourite page; required reading before defending the methodology.

### Roles & Personas

Left: each role with its colour-themed pill, persona, tagline. Right: the role's primary concerns, landing screen (clickable), bespoke screen overrides, and the users currently assigned to it (clickable to filter the Users page).

### Project Flavours

Each demo rulebook classified by Flavor (crud-template, computation-heavy, aggregation-heavy, graph-ontology, meta-rulebook, tutorial-ladder), Complexity, and density metrics. Click a flavour to filter the project switcher to that flavour.

### Platform Features

Two sections: Headline features (ADP, clean, hub-and-spoke, convergent build, substrate equivalence, conformance, local SSoT, portal/CLI parity, write-through) and Additional features (fail-loud, complete spec, dialect binding, etc.). Left list, right detail. Detail shows one-line summary, status, related axiom (clickable), README file path, IsReadmeStub badge, and the seed README content. Developer can edit summary/tier/priority/README path in place — saves write-through into rulebook JSON.

### Docs Home

Reference documentation index.

### Framing Invariants

Mistakes-to-avoid catalog.

### Methodology

Axioms and framing invariants.

### Field Types

ERB field-type taxonomy.

### Glossary

Term definitions.

### Admin

Admin landing.

### Permissions

Permission matrix.

### Navigation

Sidebar nav items.

### Screens

Screen registry.

