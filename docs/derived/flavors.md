<!-- GENERATED FILE — DO NOT EDIT. -->
<!-- Source: effortless-platform/effortless-rulebook/effortless-rulebook.json (table: `RulebookFlavors`) -->
<!-- Regenerate with: cd effortless-platform && effortless build -->
<!-- Generated: 2026-05-25T22:56:16Z -->

# Rulebook Flavors

Classification of each demo rulebook under rulebook-examples/. Lets the UI group projects by what they're TEACHING — a tutorial ladder is a different beast from a computation-heavy ontology. Density numbers come from a static analysis of each rulebook (calculated/aggregation/lookup counts).

| Project | Name | Flavor | Complexity | Entities | Calcs | Aggs | Lookups | Focus | Answer Key For |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| acme-corporation | ACME Corporation (template) | crud-template | basic | 5 | 1 | 2 | 10 | Classic relational CRUD template — clients, projects, employees, roles. The 'starter sized' demo for new authors. | postgres,entity-framework |
| acme-llc | ACME, LLC (template) | tutorial-ladder | minimal | 3 | 2 | 0 | 0 | Smallest viable rulebook with a calculated field — the 'Hello, formulas' tutorial. | — |
| customer-fullname | Customer FullName (tutorial) | tutorial-ladder | minimal | 1 | 2 | 0 | 0 | One entity, one calculated field (CONCAT(First, ' ', Last)). The 'absolute minimum' demo — proves the toolchain end-to-end with no relationships. | — |
| effortless-rulesbooks | ERB self-describing rulebook | meta-rulebook | advanced | 9 | 0 | 0 | 0 | The rulebook that describes the ERB project itself (sibling of this platform rulebook). Demonstrates eating-the-dog-food — meta over business data. | — |
| is-everything-a-language | Is Everything A Language? (semiotics) | graph-ontology | advanced | 3 | 8 | 0 | 0 | Heavy formulas evaluating linguistic / semiotic candidates. Shows that the substrate equality claim holds even for non-CRUD ontologies. | owl,python |
| jessica-advanced | Jessica Talisman — Advanced (workflows) | computation-heavy | advanced | 9 | 8 | 1 | 2 | Workflow/approval ontology with intermediate computed fields. Demonstrates multi-step derivation across entities. | postgres |
| jessica-basic | Jessica Talisman — Basic (workflows) | tutorial-ladder | basic | 9 | 7 | 1 | 0 | Same workflow concepts as advanced but without cross-entity lookups — useful for stepping authors up the formula ladder. | — |
| star-trek | Star Trek (series/seasons/episodes) | aggregation-heavy | advanced | 6 | 7 | 4 | 9 | Aggregations over nested relationships (avg rating per episode, per season, per series). The canonical aggregation demo. | postgres,xlsx |
| community-event-planner-demo | community-event-planner-demo | aggregation-heavy | basic | 6 | 19 | 4 | 9 | Auto-discovered demo. Replace this stub with a one-line description of what community-event-planner-demo is designed to teach. | — |
| customer-crm-demo | customer-crm-demo | aggregation-heavy | advanced | 7 | 20 | 11 | 21 | Auto-discovered demo. Replace this stub with a one-line description of what customer-crm-demo is designed to teach. | — |
| effortless-banking-demo | effortless-banking-demo | aggregation-heavy | advanced | 10 | 46 | 17 | 17 | Auto-discovered demo. Replace this stub with a one-line description of what effortless-banking-demo is designed to teach. | — |
| fantasy-football-demo | fantasy-football-demo | aggregation-heavy | basic | 6 | 13 | 5 | 15 | Auto-discovered demo. Replace this stub with a one-line description of what fantasy-football-demo is designed to teach. | — |
| guessing-game | guessing-game | aggregation-heavy | minimal | 2 | 5 | 4 | 6 | Auto-discovered demo. Replace this stub with a one-line description of what guessing-game is designed to teach. | — |
| gym-trainer-invoicing | gym-trainer-invoicing | aggregation-heavy | basic | 5 | 14 | 8 | 11 | Auto-discovered demo. Replace this stub with a one-line description of what gym-trainer-invoicing is designed to teach. | — |
| intelligence-taxonomy-demo | intelligence-taxonomy-demo | crud-template | minimal | 3 | 5 | 2 | 4 | Auto-discovered demo. Replace this stub with a one-line description of what intelligence-taxonomy-demo is designed to teach. | — |
| jobsearch-rag | jobsearch-rag | aggregation-heavy | advanced | 10 | 4 | 6 | 10 | Auto-discovered demo. Replace this stub with a one-line description of what jobsearch-rag is designed to teach. | — |
| llm-enigma-test | llm-enigma-test | aggregation-heavy | advanced | 26 | 33 | 42 | 22 | Auto-discovered demo. Replace this stub with a one-line description of what llm-enigma-test is designed to teach. | — |
| product-inventory-demo | product-inventory-demo | computation-heavy | minimal | 3 | 6 | 1 | 3 | Auto-discovered demo. Replace this stub with a one-line description of what product-inventory-demo is designed to teach. | — |
| therapist-helper-portal | therapist-helper-portal | aggregation-heavy | basic | 5 | 12 | 11 | 10 | Auto-discovered demo. Replace this stub with a one-line description of what therapist-helper-portal is designed to teach. | — |
| v2-nakedclaude-demo | v2-nakedclaude-demo | crud-template | minimal | 3 | 5 | 0 | 3 | Auto-discovered demo. Replace this stub with a one-line description of what v2-nakedclaude-demo is designed to teach. | — |
| v3-nakedclaude-demo | v3-nakedclaude-demo | aggregation-heavy | advanced | 15 | 30 | 13 | 43 | Auto-discovered demo. Replace this stub with a one-line description of what v3-nakedclaude-demo is designed to teach. | — |
| wedding-seating-optimizer | wedding-seating-optimizer | aggregation-heavy | basic | 4 | 16 | 9 | 7 | Auto-discovered demo. Replace this stub with a one-line description of what wedding-seating-optimizer is designed to teach. | — |
