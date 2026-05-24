# Glossary — Small Business Banking Client Manager

Formal definitions of the platform vocabulary, grouped by area. Each entry is one or two sentences explaining how the term is used inside this platform — not its general meaning in banking at large.

---

## Core entities

- **Business** — A small-business customer of the bank, treated as the central entity in the data model. Most other records (loans, accounts, contacts, interactions) hang off a single `Business` record.
- **BeneficialOwner** — Any individual who owns 25%+ of a `Business`, plus one designated control person, per FinCEN's CDD rule. The platform stores their identifying PII (including SSN, date of birth, address) under field-level encryption.
- **ControlPerson** — The single individual designated as having significant control over the business when no one owner crosses the 25% ownership threshold; collected alongside `BeneficialOwner` records during KYB.
- **Contact** — A non-owner individual associated with a business: officers, AP clerks, authorized signers, and other points of contact the RM speaks with.
- **Account** — A deposit relationship the business holds at the bank (checking, savings, money market). Balances feed global cash flow calculations on loan applications.
- **Loan** — A credit facility extended to a `Business`, tracked through its full lifecycle from application through funding, servicing, and payoff.
- **Covenant** — A condition attached to a `Loan` that must be tested on a recurring schedule (e.g., minimum DSCR each quarter). The covenant monitoring module fires reminders before each test date.
- **Document** — A file attached to either a `Business` (tax returns, formation docs) or a `Loan` (note, security agreement, appraisals). The foreign key is intentionally nullable to allow either parent.
- **User** — A bank employee with a role: relationship manager, underwriter, admin, or branch banker. Used for portfolio assignment and audit trails.
- **Interaction** — A unified activity-log entry against a `Business`. Meeting notes, logged calls, visits, tasks, and system-generated events (e.g., "covenant test failed") are all `Interaction` records distinguished by `InteractionType`.
- **InteractionType** — The discriminator on an `Interaction` row: note, call, visit, task, meeting, or system event.
- **RiskRatingHistory** — Time-series record of `Loan.risk_rating` changes; the live rating is denormalized onto the `Loan`, but every migration of the grade is captured here because regulators audit rating drift.

## People & roles

- **RelationshipManager** (RM) — A bank employee who owns 40-80 small-business customer relationships end-to-end. The RM is the customer's single point of contact through every interaction, including the loan lifecycle.
- **Underwriter** — A bank employee responsible for credit analysis, statement spreading, and risk grading. Works the technical side of the loan while the RM stays the customer-facing voice.
- **BranchBanker** — A bank employee operating from a physical branch, typically on a tablet, primarily during in-branch onboarding sessions.
- **Admin** — A bank employee with elevated permissions for compliance configuration, audit review, and user management.
- **Officer** — A named officer of a `Business` (CEO, CFO, president, etc.), stored as a `Contact`.
- **Signer** — An individual authorized to sign on a `Business`'s accounts; stored as a `Contact` with an authorization flag.
- **APClerk** — A clerical staff member at a customer business (accounts-payable clerk) the RM may need to interact with for invoicing and operational queries.
- **Customer** — A `Business` viewed from the customer-facing side of the platform (the business client portal). One business is one customer.

## Bank context

- **CommunityBank** — The kind of institution this platform serves: typically $10M-$1B in assets, competing on relationship depth rather than scale or rate.
- **RegionalBank** — Larger competitor banks the community bank competes against.
- **Fintech** — Pure-play digital lenders against whom community banks differentiate by keeping the RM in the loop throughout the loan journey.
- **CoreBanking** — The bank's authoritative system of record for accounts and balances. Most community banks run on Jack Henry, FIS, or Fiserv.
- **JackHenry** / **FIS** / **Fiserv** — The three most common core-banking vendors community banks use.

## Client management domain

- **BusinessProfile** — The identity slice of a `Business` — legal name, structure, NAICS code, officers — distinct from the relationship and lending slices.
- **KYB** — Know Your Business: the onboarding process that collects beneficial ownership, gathers required documents, and verifies identity per FinCEN's CDD rule.
- **Onboarding** — The end-to-end new-customer workflow, including KYB, account opening, and welcome interactions.
- **InteractionLog** — The CRM-style activity feed for a `Business`: notes, calls, visits, tasks, and system events, unified into one `Interaction` stream.
- **TreasuryServices** — Non-deposit cash-management products attached to an account: ACH origination, wires, cards, lockbox, etc.
- **Pipeline** — Generic term for a stage-tracked workflow; both `LoanPipeline` and the prospect pipeline use the same concept.
- **Prospect** — A `Business` the bank is trying to win as a customer; tracked in pipeline form before becoming a full customer.
- **Referral** — An inbound prospect introduced by a third party; tracked as an attribute on a `Prospect` record.
- **CrossSell** — An opportunity to sell additional products (e.g., treasury services) to an existing customer.
- **Portfolio** — The set of businesses assigned to a given RM via `Business.relationship_manager_id`.

## Lending and risk domain

- **LoanPipeline** — The stage-tracked workflow that moves a `Loan` from application through underwriting stages to closing.
- **LoanOrigination** — The end-to-end loan journey: inquiry → pre-qualification → application → KYB and bureau pull → packaging → credit analysis → committee approval → closing prep → signing and funding.
- **LoanApplication** — The formal application record created when an inquiry advances past pre-qualification.
- **LoanServicing** — Post-funding loan management: payment processing, delinquency workflows, loss mitigation, payoff.
- **Inquiry** — The earliest step in the loan journey, when a `Business` first asks about borrowing.
- **PreQualification** — The lightweight check that determines whether an `Inquiry` should become a full `LoanApplication`.
- **CreditAnalysis** — The underwriter's quantitative work: financial statement spreading, DSCR and LTV computation, global cash flow.
- **CreditPull** — A request to a credit bureau (Experian, D&B, Equifax) for commercial credit information; runs automatically once an application is submitted.
- **FinancialStatement** — A submitted income statement, balance sheet, or cash-flow statement from a `Business`, used as input to `StatementSpreading`.
- **StatementSpreading** — The structured re-keying of a `FinancialStatement` into a standardized template so ratios and trends can be computed.
- **DebtServiceCoverageRatio** (DSCR) — A ratio of cash flow available to debt service; one of the primary underwriting metrics computed during credit analysis.
- **LoanToValue** (LTV) — A ratio of loan amount to collateral value; the other primary underwriting metric for secured loans.
- **GlobalCashFlow** — Combined cash flow across a `Business`, its owners, and affiliated entities; computed during credit analysis.
- **CovenantMonitoring** — The module that runs the tickler calendar firing reminders ahead of each `Covenant` test date.
- **CovenantBreach** — A failed covenant test; surfaces as an `Interaction` of type system event against the `Business`.
- **CovenantWaiver** — A formal allowance for a `CovenantBreach`; recorded against the `Covenant`.
- **CovenantException** — A noted-but-not-formally-waived deviation from a `Covenant` test; lighter-weight than a waiver.
- **TestSchedule** — The recurrence definition for when a `Covenant` must be tested (quarterly, annually, etc.).
- **TicklerCalendar** — The scheduling engine that fires `CovenantMonitoring` reminders before each test date.
- **RiskRating** — A graded assessment of `Loan` credit risk. Lives denormalized on the `Loan` and historically in `RiskRatingHistory`.
- **RiskGrade** — A specific value on the risk-rating scale (e.g., pass grades 1-5, special-mention 6, substandard 7, etc.).
- **RiskGradeMigration** — A change to a `Loan.risk_rating`, captured as a new `RiskRatingHistory` row.
- **AnnualReview** — A mandatory yearly re-underwrite of an existing `Loan`, refreshing financials, ratings, and covenant compliance.
- **Watchlist** — The set of loans currently classified or otherwise needing heightened management attention.
- **ClassifiedAsset** — A `Loan` whose risk grade has fallen into the substandard / doubtful / loss range and is therefore on the `Watchlist`.
- **CollectionsWorkflow** — The structured workflow that runs once a loan is past due past a threshold.
- **DelinquencyWorkflow** — The earlier-stage workflow that fires when a payment is missed but before full collections.
- **LossMitigation** — Activities (modification, restructuring, foreclosure) intended to minimize loss on a troubled `Loan`.
- **PaymentProcessing** — The servicing module that posts payments and tracks application of principal, interest, fees, and escrow.
- **UnderwritingStage** — The named stage a `LoanApplication` currently sits at within the `LoanPipeline`.
- **CommitteeApproval** — The credit-committee sign-off step required before a `Loan` can move to closing prep.
- **ClosingPrep** — The pre-funding step where documents are drafted, conditions are cleared, and the loan is staged for signing. The RM re-enters the flow here.
- **Signing** — The customer-facing execution of loan documents; uses `ESignature`.
- **Funding** — The disbursement of loan proceeds, ending the origination journey.

## Compliance and regulatory

- **BSA** — Bank Secrecy Act; the regulatory backbone that requires transaction monitoring, CTR/SAR filing, and customer due diligence.
- **AML** — Anti-Money Laundering; the program of controls that satisfies BSA obligations.
- **CDD** — Customer Due Diligence; FinCEN's rule mandating beneficial ownership collection at onboarding.
- **CTR** — Currency Transaction Report; a regulatory filing required for cash transactions above the reporting threshold.
- **SAR** — Suspicious Activity Report; a regulatory filing when activity matches red-flag patterns.
- **OFAC** — Office of Foreign Assets Control; the agency whose sanctions lists must be screened against during onboarding and ongoing monitoring.
- **FFIEC** — Federal Financial Institutions Examination Council; publishes the examination standards community banks are measured against.
- **OCC** — Office of the Comptroller of the Currency; a federal regulator of nationally chartered banks.
- **FinCEN** — Financial Crimes Enforcement Network; the agency that owns CDD/KYB rules and receives CTR/SAR filings.
- **HMDA** — Home Mortgage Disclosure Act; reporting required where applicable to a community bank's mortgage activity.
- **NAICS** — North American Industry Classification System code stored on `BusinessProfile`.
- **UCC** — Uniform Commercial Code; the framework for `UCCLienSearch` against `PublicRecords`.
- **CECL** — Current Expected Credit Losses accounting standard; drives `ALLL` calculations in `ReportingAndAnalytics`.
- **ALLL** — Allowance for Loan and Lease Losses; the reserve calculation produced by reporting.
- **CallReport** — A standardized regulatory report community banks file each quarter; data is sourced from the platform.
- **SegregationOfDuties** — A control enforced by `IdentityAndAccess` that prevents the same `User` from doing conflicting steps (e.g., underwriting and approving) on the same `Loan`.
- **BeneficialOwnership** — The collected ownership graph behind a `Business`, captured as `BeneficialOwner` records.
- **SanctionsScreening** — Automated OFAC check fired at onboarding and on an ongoing basis.
- **TransactionMonitoring** — BSA/AML system that watches transactions for red flags; typically Verafin.
- **AuditTrail** — Per-record, per-field history of who changed what and when, traced back via `User`.
- **PII** — Personally Identifiable Information; subject to encryption-at-rest and access controls.
- **EncryptedAtRest** — Storage-layer encryption for sensitive fields (e.g., `BeneficialOwner.ssn`).
- **FieldLevelEncryption** — Per-field encryption applied to PII; managed via a separate key-management system.

## Accounts and treasury

- **DepositAccount** — A specific account record at the bank for a `Business`; one of the entities the platform syncs from `CoreBanking`.
- **ACH** — Automated Clearing House; one of the treasury-services products attached to an account.
- **Wire** — Wire transfer; another treasury-services product.
- **Card** — Business credit/debit card; another treasury product.
- **CashManagement** — Umbrella term for treasury services (ACH, wires, cards, etc.).
- **Balance** — The current balance on a `DepositAccount`; feeds global cash flow calculations.

## External integrations

- **CreditBureau** — Generic third-party credit-data provider; the platform integrates with Experian, D&B, and Equifax.
- **Experian** / **DnB** / **Equifax** — The specific commercial credit bureaus the platform pulls from.
- **PublicRecords** — Generic category covering Secretary of State filings, UCC lien searches, and court records.
- **SecretaryOfState** — Source for business formation and good-standing data.
- **UCCLienSearch** — Lookup against UCC filings to identify existing collateral encumbrances.
- **CourtRecord** — Litigation history pulled during underwriting.
- **AccountingFeed** — A direct feed of customer accounting data (P&L, balance sheet) from `QuickBooks`, `Xero`, or via `Plaid`.
- **QuickBooks** / **Xero** — Specific accounting platforms the bank pulls from for live underwriting data.
- **Plaid** — Aggregator used for accounting and bank-data feeds.
- **Verafin** — Common BSA/AML transaction-monitoring vendor.
- **RegulatoryReporting** — The category of external integrations covering FFIEC submissions, Call Report data, HMDA where applicable.

## Platform services

- **DocumentVault** — Central document store with OCR and e-signature capabilities; documents in the vault attach to a `Business` or a `Loan`.
- **OCR** — Optical character recognition applied to documents in the vault to extract text for indexing and pre-fill.
- **ESignature** — Digital signing of documents during `ClosingPrep` and `Signing`.
- **ReportingAndAnalytics** — Module that produces portfolio dashboards, CECL/ALLL calculations, board packs, and regulatory reports.
- **PortfolioDashboard** — An RM-facing analytics surface summarizing the businesses in their portfolio.
- **BoardPack** — Periodic packet of portfolio and risk reports prepared for the bank's board.
- **Notification** — A single delivered message to a `User` or `Customer` across the configured channels.
- **NotificationChannel** — Delivery medium for a `Notification` (email, SMS, in-app, etc.).
- **WorkflowEngine** — The configurable approval-routing engine with SLA tracking that powers approvals across modules.
- **ApprovalRouting** — A specific routing path through the `WorkflowEngine` (e.g., loan over $X routes to committee).
- **SLA** — Service-level agreement; per-stage or per-step time targets enforced by the workflow engine.
- **IdentityAndAccess** — The module providing SSO, MFA, role-based access, and segregation-of-duties enforcement.
- **SSO** — Single sign-on integration for bank `User` accounts.
- **MFA** — Multi-factor authentication required on sensitive operations.
- **RoleBasedAccess** — Permission model based on `User` role (RM, underwriter, admin, branch banker).
- **SearchAndIndexing** — Underlying capability that powers `GlobalSearch` across clients and documents.
- **GlobalSearch** — The single search box that searches across businesses, loans, documents, and interactions.

## Presentation surfaces

- **RMDashboard** — The relationship-manager's primary workspace: portfolio summary, action items, client search.
- **BranchBankerPortal** — Tablet-optimized surface for in-branch onboarding sessions.
- **BusinessClientPortal** — Customer-facing surface where business owners upload documents, request loans, and message their RM.
- **AdminConsole** — Compliance configuration, audit review, and user management surface.

## Activity / event types

- **Note** — Free-text `Interaction` recorded by a `User`.
- **Call** — Logged call `Interaction`.
- **Visit** — Logged in-person visit `Interaction`.
- **Task** — Actionable `Interaction` with an owner and due date.
- **Meeting** — Scheduled meeting `Interaction`, typically with notes attached.
- **SystemEvent** — System-generated `Interaction` (e.g., "covenant test failed", "documents requested"); makes machine actions visible in the same activity feed as human ones.

## Other domain concepts

- **SystemOfRecord** — Whichever system is authoritative for a given dataset; for accounts and balances, `CoreBanking` is the system of record, not this platform.
- **SinglePointOfContact** — The community-bank promise that one RM owns the customer relationship across every interaction, including loan origination.
- **OwnershipPercentage** — Stored on `BeneficialOwner` to identify who crosses FinCEN's 25% threshold.
- **RelationshipDepth** — The qualitative measure community banks compete on: how deeply the RM knows the customer and their business.
- **DataResidency** — Strict requirements that some bank data not leave designated jurisdictions; affects deployment topology.
