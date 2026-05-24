# Small Business Banking Client Manager

A system design for a community bank's commercial relationship management platform. This package contains three Figma-importable diagrams documenting the architecture, the loan origination user journey, and the core data model.

## Contents


| File                           | What it shows                                                                              |
| ------------------------------ | ------------------------------------------------------------------------------------------ |
| `01_system_architecture.svg`   | Layered system map: presentation, domain modules, platform services, external integrations |
| `02_loan_origination_flow.svg` | Cross-functional swimlane showing the loan journey from inquiry to funding                 |
|                                |                                                                                            |


## What the system does

The platform is built for a community bank's commercial relationship managers (RMs) — the people who own the bank's small business customer relationships end to end. Community banks compete with regional banks and fintechs on relationship depth, so the design centers the RM's daily workflow and makes the lending and compliance machinery serve that workflow rather than dictate it.

A typical RM at a community bank manages 40-80 business clients ranging from sole proprietors to companies with $50M in revenue. They need to know each client's full picture (deposits, loans, treasury services, recent interactions, upcoming reviews) without bouncing between five systems.

## Architecture overview

The architecture is organized in four horizontal tiers, top to bottom in `01_system_architecture.svg`:

### Presentation layer

Four distinct surfaces, each tuned to a user role. The RM dashboard is the primary workspace — portfolio summary, action items, client search. The branch banker portal is optimized for tablet use during in-branch onboarding sessions. The business client portal is the customer-facing surface where business owners upload documents, request loans, and message their RM. The admin console handles compliance configuration, audit review, and user management.

### Client management domain

The customer-of-record system. Five modules: the `Business Profile` (identity, structure, NAICS code, officers); `KYB and onboarding` (beneficial ownership collection per FinCEN's CDD rule, document gathering, identity verification); the `Interaction log` (notes, calls, visits, tasks — the CRM bones); `Accounts and treasury` (deposit accounts, ACH, wires, cards, cash management services); and `Pipeline tracking` for prospects, referrals, and cross-sell opportunities.

### Lending and risk domain

Where the credit lifecycle lives. The `Loan pipeline` tracks applications through underwriting stages. `Credit analysis` handles financial statement spreading, debt service coverage ratio (DSCR) calculation, loan-to-value (LTV) computation, and global cash flow analysis — the quantitative work underwriters do. `Covenant monitoring` runs the tickler calendar that fires when a covenant test date approaches and tracks exceptions and waivers. `Risk rating and review` handles annual reviews, risk grade migration, and watchlist management for classified assets. `Loan servicing and collections` runs the back end: payment processing, delinquency workflows, loss mitigation.

The two domains share data heavily. A loan application originates against a `Business` record; covenant breaches surface as interaction-log entries; account balances feed global cash flow calculations.

### Platform services

Cross-cutting services every module uses: the `Document vault` (with OCR and e-signature), `Reporting and analytics` (portfolio dashboards, CECL/ALLL calculations, board packs, regulatory reports), `Notifications` across channels, the `Workflow engine` (configurable approval routing with SLA tracking), `Identity and access` (SSO, MFA, role-based access with segregation-of-duties enforcement), and `Search and indexing` for global search across clients and documents.

### External integrations

Six categories, all dashed in the diagram to mark the system boundary:

- **Core banking** — the bank's system of record for accounts and balances. Most community banks run on Jack Henry, FIS, or Fiserv
- **BSA/AML** — transaction monitoring, CTR/SAR filing, OFAC sanctions screening (Verafin is common)
- **Credit bureaus** — Experian, D&B, Equifax for commercial credit pulls
- **Public records** — Secretary of State filings, UCC lien searches, court records
- **Accounting data** — direct feeds from QuickBooks, Xero, or via Plaid for live financials during underwriting
- **Regulatory** — FFIEC reporting, Call Report data, HMDA where applicable

## Loan origination journey

`02_loan_origination_flow.svg` shows the end-to-end loan journey as a swimlane diagram across four actors: customer, relationship manager, underwriter, and system. The nine steps run inquiry → pre-qualification → application → automated KYB and bureau pull → RM packaging → credit analysis → committee approval → closing prep → signing and funding.

The diagram captures something community banks care about specifically: the RM stays in the loop throughout. Unlike pure-play digital lenders where the underwriter and customer never speak, the community bank model keeps the RM as the customer's single point of contact even while the underwriter does the technical work. The diagram makes this visible by routing the closing-prep step back through the RM lane before the customer signs.

The SLA targets at the bottom (14-21 days inquiry to funded) reflect community bank pace — slower than fintechs by design, since the relationship and the underwriting depth are the product.

## Data model

`03_data_model.svg` shows the eight primary entities and their relationships.

`Business` is the central entity — most things hang off it. A `Business` has many `BeneficialOwners` (per FinCEN, anyone with 25%+ ownership plus one control person), many `Contacts` (officers, AP clerks, authorized signers), many `Accounts` (deposit relationships), many `Loans`, and many `Interactions` (the CRM activity log).

A `Loan` has many `Covenants`, each with its own test schedule. `Documents` can attach to either a `Business` (tax returns, formation docs) or a `Loan` (note, security agreement, appraisals) — that's the nullable foreign key.

`User` represents bank employees: RMs, underwriters, admins. The `Business.relationship_manager_id` foreign key creates the portfolio assignment. Interactions and audit references trace back to the user who performed them.

A few specifics worth flagging:

- `BeneficialOwner.ssn` is encrypted at rest — the diagram notes it inline. PII handling extends to dates of birth, addresses, and any document containing the same. Use field-level encryption with separate key management
- `Loan.risk_rating` is a denormalized field — the actual rating history lives in a `RiskRatingHistory` table not shown in this overview diagram, since rating migration tracking is one of the things regulators look at
- `Interaction` is intentionally generic. A meeting note, a logged call, a system-generated "covenant test failed" event, and a "documents requested" task are all interactions with different `type` values. This keeps the activity feed unified

## Design conventions

The diagrams use a consistent visual language across all three files:

- **Teal** marks the client management domain — the relationship-side of the system
- **Purple** marks parties and people (beneficial owners, contacts) and also the lending domain on the architecture diagram
- **Coral** marks financial entities — accounts, loans, covenants
- **Gray** is for platform infrastructure and structural containers
- **Dashed outlines** mark system boundaries (external integrations) or weak references (audit trails)

Rounded rectangles with light fills and 1px borders. Arrowheads on directional relationships. No gradients, no shadows. The goal is a diagram that prints cleanly and reads on a projector.

## What's not in this package

A few things you'd want before building this for real:

- **Detailed screen mockups** beyond the system map — wireframes for the RM dashboard, loan application, credit analysis worksheet, and client profile. I can produce those as a follow-up
- **Sequence diagrams** for the API interactions between modules (especially loan-pipeline ↔ core-banking ↔ document-vault during closing)
- **Deployment topology** — VPC layout, database segmentation, DR strategy. Community banks typically deploy on a mix of on-prem (for the core integration) and cloud, with strict data residency requirements
- **Detailed regulatory mapping** — which controls satisfy which FFIEC, OCC, and state banking regulator requirements
- **Pricing model and vendor sourcing** — build vs buy decisions for each module, with realistic cost ranges for a community bank scale (typically $10M-$1B in assets)

Happy to extend any of the above.