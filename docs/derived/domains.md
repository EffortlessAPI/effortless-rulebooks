<!-- GENERATED FILE — DO NOT EDIT. -->
<!-- Source: effortless-platform/effortless-rulebook/effortless-rulebook.json (table: `RulebookDomains`) -->
<!-- Regenerate with: cd effortless-platform && effortless build -->

# Rulebook Domains

Customer ontologies: each domain has its own rulebook + substrate generation. Domains form a TREE — ParentDomainId links a more-elaborate domain back to the simpler one it grew out of (e.g. Talisman ADVANCED ← Talisman BASIC). The UI uses this to present related rulebooks as a set rather than a flat list, and to drive 'next step in the progression' navigation.

## Customer FullName

_complexity: minimal · tables: 1_

Hello World: demonstrates basic schema and calculated field (CONCAT formula)

**Key features.** string concatenation

_Rulebook path:_ `rulebook-examples/customer-fullname/effortless-rulebook/customer-fullname-rulebook.json`

## Talisman BASIC

_complexity: moderate · tables: 9_

Real workflow modeling: task management with assigned agents

**Key features.** relationships, aggregations, role-agent separation

_Rulebook path:_ `rulebook-examples/talisman-basic/effortless-rulebook/talisman-basic-rulebook.json`

## Talisman ADVANCED

_complexity: advanced · tables: 9_

Business rule complexity: demonstrates advanced formula patterns

**Key features.** cross-entity lookups, conditional IF logic, complex aggregations

_Rulebook path:_ `rulebook-examples/talisman-advanced/effortless-rulebook/talisman-advanced-rulebook.json`

## StarTrek

_complexity: moderate · tables: 7_

Media catalog: TV shows, seasons, episodes; demonstrates polymorphism

**Key features.** hierarchical rollups, polymorphic foreign keys

_Rulebook path:_ `rulebook-examples/star-trek/effortless-rulebook/star-trek-rulebook.json`

## Is Everything a Language?

_complexity: philosophical · tables: 3_

Formal argument modeling: demonstrates schema expressivity for abstract domains

**Key features.** 8-predicate AND logic, meta-ontology

_Rulebook path:_ `rulebook-examples/is-everything-a-language/effortless-rulebook/is-everything-a-language-rulebook.json`

## ACME Corporation

_complexity: advanced_

Enterprise demo: ACME Corp operations

**Key features.** Hub promotion candidate; demonstrates rulebook-first workflow

_Rulebook path:_ `rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json`

## ACME LLC

_complexity: advanced_

Enterprise demo: ACME LLC operations

**Key features.** Hub promotion candidate; demonstrates rulebook-first workflow

_Rulebook path:_ `rulebook-examples/acme-llc/effortless-rulebook/acme-llc-rulebook.json`

## Effortless Rulesbooks (self-referential demo)

_complexity: philosophical_

Demonstrates ERB modeling its own architecture as a domain rulebook

**Key features.** self-referential; rulebook describes the ERB project itself

_Rulebook path:_ `rulebook-examples/effortless-rulesbooks/effortless-rulebook/effortless-rulesbooks-rulebook.json`

## Guessing Game

_complexity: minimal · tables: 2_

Number-guessing game tracking guesses, hints, and best-score records per player.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/guessing-game/effortless-rulebook/guessing-game-rulebook.json`

## NakedClaude v1

_complexity: minimal · tables: 1_

Rulebook generated from Airtable base 'v1: NakedClaude Demo'.

**Key features.** calculated fields, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/nakedclaude-v1/effortless-rulebook/nakedclaude-v1-rulebook.json`

## NakedClaude v2

_complexity: moderate · tables: 3_

Rulebook generated from Airtable base 'v2: NakedClaude Demo'.

**Key features.** relationships, calculated fields, lookups, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/nakedclaude-v2/effortless-rulebook/nakedclaude-v2-rulebook.json`

## NakedClaude v3

_complexity: moderate · tables: 8_

Rulebook generated from Airtable base 'v3: NakedClaude Demo'.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/nakedclaude-v3/effortless-rulebook/nakedclaude-v3-rulebook.json`

## NakedClaude v4

_complexity: advanced · tables: 15_

Rulebook generated from Airtable base 'v4: NakedClaude Demo'.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/nakedclaude-v4/effortless-rulebook/nakedclaude-v4-rulebook.json`

## Product Inventory

_complexity: moderate · tables: 3_

Products with transactions adjusting quantities and low-stock alerts.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/product-inventory/effortless-rulebook/product-inventory-rulebook.json`

## Expense Approval

_complexity: moderate · tables: 3_

Employees submit line-item reports; totals, over-budget, and escalation flags cascade automatically.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/expense-approval/effortless-rulebook/expense-approval-rulebook.json`

## Volunteer Shift Scheduler

_complexity: moderate · tables: 4_

Coverage status, volunteer load (under/ok/over), and event-level A–F staffing grade all fall out automatically.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/volunteer-shift-scheduler/effortless-rulebook/volunteer-shift-scheduler-rulebook.json`

## Wedding Seating Optimizer

_complexity: moderate · tables: 4_

Seating plan as a DAG — per-table happiness, capacity flags, per-guest satisfaction recompute on every move.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/wedding-seating-optimizer/effortless-rulebook/wedding-seating-optimizer-rulebook.json`

## Gym Trainer Invoicing

_complexity: moderate · tables: 5_

Sessions roll up into invoices; invoices roll up into client outstanding balances.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/gym-trainer-invoicing/effortless-rulebook/gym-trainer-invoicing-rulebook.json`

## Therapist Helper Portal

_complexity: moderate · tables: 5_

Sessions and treatment progress: GoalUpdate → Goal.ProgressPct → Client.IsAtRisk three-hop DAG.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/therapist-helper-portal/effortless-rulebook/therapist-helper-portal-rulebook.json`

## Community Event Planner

_complexity: moderate · tables: 6_

Venues, events, speakers, attendees with capacity, scheduling, and attendance-forecast cascades.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/community-event-planner/effortless-rulebook/community-event-planner-rulebook.json`

## Customer CRM

_complexity: advanced · tables: 7_

Fighter-jet FCS sales pipeline rolling revenue up by order, FCS variant, and jet model.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/customer-crm/effortless-rulebook/customer-crm-rulebook.json`

## Fantasy Football

_complexity: advanced · tables: 6_

Multi-hop DAG: raw player stats → roster aggregations → matchup scoring → standings & seeding.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/fantasy-football/effortless-rulebook/fantasy-football-rulebook.json`

## Taxonomy of Intelligence

_complexity: philosophical · tables: 3_

Classifies intelligences (humans, animals, AI) by per-capability assessments through a multi-hop DAG.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/intelligence-taxonomy/effortless-rulebook/intelligence-taxonomy-rulebook.json`

## Job Search RAG

_complexity: advanced · tables: 10_

Local LLM + RAG pipeline filtering jobs across boards using semantic search.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/job-search-rag/effortless-rulebook/job-search-rag-rulebook.json`

## Effortless Banking

_complexity: advanced · tables: 10_

Community-bank commercial RM platform — loans, deposits, covenants, BSA/AML.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/effortless-banking/effortless-rulebook/effortless-banking-rulebook.json`

## Mechanical Kitchen Timer

_complexity: advanced · tables: 26_

Five-part mechanical timer modeled with every README noun as a table — a hardware-ontology stress test.

**Key features.** relationships, calculated fields, lookups, aggregations, IF/AND/OR logic

_Rulebook path:_ `rulebook-examples/mechanical-kitchen-timer/effortless-rulebook/mechanical-kitchen-timer-rulebook.json`

## Effortless Math

_complexity: advanced · tables: 16_

Executable theorem network — Fermat's Last Theorem as flagship consumer over seven imported provider theorems; a certificate/status ledger for a proof network, not a prover.

**Key features.** provider/consumer theorem contracts, non-boolean proof-status ledger, trust-boundary DAG, versioned certificates, bitemporal witness/provenance

_Rulebook path:_ `rulebook-examples/effortless-math/effortless-rulebook/effortless-math-rulebook.json`

