# Narrative — Small Business Banking Client Manager

This narrative describes the platform end-to-end. It is written to exercise every term in `vocabulary.txt`; every domain word appears at least once so the rulebook generated from this narrative inherits the full domain language.

---

## What we are building

The Small Business Banking Client Manager is a commercial relationship-management platform built for a `CommunityBank`. A community bank sits between a `RegionalBank` and a `Fintech` in the market: it competes on `RelationshipDepth`, not on rate or scale, and it does so by promising the `Customer` a `SinglePointOfContact` across every product and every interaction. That single point of contact is the `RelationshipManager`, abbreviated `RM` everywhere in this system.

A typical RM at a community bank manages a `Portfolio` of forty to eighty `Business` records. A `Business` may be a sole proprietor, a small partnership, or a company with up to fifty million dollars in revenue. The RM needs to see the full picture of each `Business` — its `DepositAccount` records, its outstanding `Loan` records, its enrolled `TreasuryServices`, every recent `Interaction`, and every upcoming `AnnualReview` — without bouncing between five separate systems. That single view, anchored on the `Business`, is the primary value the platform delivers.

## Bank context and core systems

The platform is not the `SystemOfRecord` for everything. The bank's `CoreBanking` system — typically `JackHenry`, `FIS`, or `Fiserv` — is authoritative for `DepositAccount` records and live `Balance` data. The platform consumes that data, surfaces it in the RM's view, and writes back only where authorized.

## Roles and access

A `User` represents a bank employee with a role: `RM`, `Underwriter`, `BranchBanker`, or `Admin`. `IdentityAndAccess` enforces `SSO` for login, requires `MFA` on sensitive operations, applies `RoleBasedAccess` to scope what each user can see, and enforces `SegregationOfDuties` so the same user cannot both underwrite and approve the same `Loan`. The `AdminConsole` is where an `Admin` configures compliance rules, reviews the `AuditTrail`, and manages user accounts.

## Onboarding a new business

When a `Prospect` becomes a `Customer`, the `Onboarding` workflow runs through `KYB` — Know Your Business — to satisfy FinCEN's `CDD` rule. KYB collects the `BeneficialOwnership` graph: every `BeneficialOwner` who holds 25% or more `OwnershipPercentage` in the `Business`, plus a designated `ControlPerson` if no owner crosses that threshold. Each beneficial owner record stores `PII` — name, date of birth, address, and `SSN` — under `FieldLevelEncryption` and `EncryptedAtRest` storage, managed with a separate key-management system.

Onboarding also records the `BusinessProfile`: legal name, structure, `NAICS` code, and the `Officer` list. Other people associated with the business — `APClerk` staff, `Signer` authorizations, additional `Contact` records — are captured alongside the officers. `SanctionsScreening` against `OFAC` lists runs at onboarding and on an ongoing basis.

A typical onboarding happens at a branch on a tablet through the `BranchBankerPortal`, which is tuned for in-person sessions where the customer signs documents and uploads supporting files on the spot.

## Accounts, treasury, and the relationship

Once onboarded, the `Business` opens one or more `Account` records — specifically `DepositAccount` records in this platform — and may enroll in `CashManagement` — `ACH` origination, outbound `Wire` transfers, business `Card` products, and other `TreasuryServices`. Every transaction routed through these products feeds `TransactionMonitoring`, typically `Verafin`, which is the bank's `BSA`/`AML` engine. Suspicious patterns trigger a `SAR`; cash transactions over the threshold trigger a `CTR`. Both filings flow to `FinCEN` through `RegulatoryReporting`.

Throughout the relationship, the RM logs activity in the `InteractionLog`. An `Interaction` is intentionally generic: an `InteractionType` distinguishes a `Note`, a `Call`, a `Visit`, a `Task`, a `Meeting`, or a `SystemEvent`. This keeps the entire activity feed unified — a covenant breach surfaces in the same stream as a meeting note, and a `Task` lives alongside a logged call. From the `RMDashboard` the RM can scan the recent activity for any business in their portfolio at a glance.

The `BusinessClientPortal` is the customer-facing surface. A business owner uses it to upload documents, request a new loan, and message their RM directly — preserving the single-point-of-contact promise while still letting the customer self-serve simple actions.

## The loan origination journey

Loan origination is the platform's most elaborate workflow. The journey runs through the `LoanPipeline` — one of two `Pipeline` workflows in the system, the other being the prospect/cross-sell pipeline — with a named `UnderwritingStage` advancing through nine steps and `SLA` targets in the range of fourteen to twenty-one days from `Inquiry` to `Funding`. The stages are:

1. **Inquiry** — the business asks about borrowing; the RM logs an `Interaction`.
2. **PreQualification** — a lightweight check that filters out non-viable requests.
3. **LoanApplication** — the formal application is created.
4. **Automated KYB and CreditPull** — `CreditPull` requests fire to the `CreditBureau` partners `Experian`, `DnB`, and `Equifax`. KYB is refreshed if stale.
5. **RM packaging** — the RM gathers `FinancialStatement` submissions and any other supporting files into the `DocumentVault`.
6. **CreditAnalysis** — an `Underwriter` performs `StatementSpreading` on each `FinancialStatement`, computes `DebtServiceCoverageRatio` (`DSCR`), `LoanToValue` (`LTV`), and `GlobalCashFlow` across the business, its owners, and affiliated entities. `PublicRecords` are checked: `SecretaryOfState` filings for good standing, a `UCC` lien lookup (`UCCLienSearch`) for existing collateral encumbrances, and `CourtRecord` lookups for litigation. Live `AccountingFeed` data from `QuickBooks`, `Xero`, or via `Plaid` can be pulled directly into the underwriting workspace.
7. **CommitteeApproval** — the credit committee signs off; this step is routed through `ApprovalRouting` configured in the `WorkflowEngine`.
8. **ClosingPrep** — the loan is staged for signing. The RM re-enters the flow here, because the community-bank model demands the customer's single point of contact stay visible all the way through.
9. **Signing and Funding** — documents execute via `ESignature` powered by the `DocumentVault`, and `Funding` disburses proceeds.

Once funded, the `Loan` exits `LoanOrigination` and enters `LoanServicing`.

## Servicing, covenants, and risk

`LoanServicing` runs `PaymentProcessing`, the `DelinquencyWorkflow` when a payment is missed, the `CollectionsWorkflow` when delinquency thresholds are crossed, and `LossMitigation` activities for troubled loans.

Every `Loan` may carry one or more `Covenant` records, each with its own `TestSchedule`. The `CovenantMonitoring` module runs a `TicklerCalendar` that fires reminders ahead of each test date. When a test fails, the result becomes a `CovenantBreach`; the breach is recorded as a `SystemEvent` `Interaction` on the parent business so it appears in the activity feed. A breach may resolve into a `CovenantWaiver` (a formal allowance) or a `CovenantException` (a noted-but-not-formally-waived deviation).

`RiskRating` lives denormalized on the `Loan` for fast reads, with every change captured in `RiskRatingHistory` as a `RiskGradeMigration`. The live `RiskGrade` may be a pass grade, a special-mention grade, or a substandard / doubtful / loss grade. Any loan with a substandard-or-worse grade is a `ClassifiedAsset` and lands on the `Watchlist`. An `AnnualReview` re-underwrites every existing loan once a year, refreshing financial statements, covenant compliance, and risk grade.

## Reporting and analytics

`ReportingAndAnalytics` produces the `PortfolioDashboard` the RM uses every day, the `BoardPack` packets prepared for board meetings, and the regulatory outputs: `CECL` and `ALLL` calculations, `CallReport` data, and `HMDA` submissions where applicable. Reports surface back into the RM's workspace, into the `AdminConsole`, and through `RegulatoryReporting` to the `FFIEC` and the `OCC`.

## Workflow, notifications, and search

The `WorkflowEngine` is shared across modules. Any approval — a loan above a dollar threshold, a `CovenantWaiver`, a credit grade override — routes through configurable `ApprovalRouting` with `SLA` tracking. Resulting `Notification` deliveries fan out across `NotificationChannel` options (email, SMS, in-app) to the right user.

`SearchAndIndexing` powers `GlobalSearch`, a single search box that spans `Business` records, `Loan` records, `Document` files in the `DocumentVault`, and `Interaction` entries. `OCR` is applied to every document on upload so the indexed content is searchable, not just filenames.

## Documents

The `DocumentVault` holds every file the bank receives or generates. A `Document` attaches to either a `Business` (tax returns, formation docs, ownership filings) or a `Loan` (the note, the security agreement, appraisals, closing packages). The foreign key to either parent is nullable on the opposite side so a document can hang off the right entity.

## Compliance posture

The platform's compliance posture is anchored on `BSA` and `AML` obligations. `KYB` and `BeneficialOwnership` collection at onboarding satisfy `CDD`. `SanctionsScreening` against `OFAC` runs continuously. `TransactionMonitoring` feeds `CTR` and `SAR` filings to `FinCEN`. `RegulatoryReporting` covers `FFIEC` examinations, the `OCC` and state banking regulator submissions, `CallReport` filings, and `HMDA` where the bank's mortgage activity makes it applicable. `SegregationOfDuties` enforced by `IdentityAndAccess` keeps the same user from underwriting and approving the same loan. The `AuditTrail` traces every change back to the `User` who made it. `PII` is handled under `FieldLevelEncryption` and `EncryptedAtRest`; `DataResidency` requirements mean some deployment topologies keep specific records inside designated jurisdictions.

## Presentation surfaces

Four surfaces serve four user populations:

- `RMDashboard` — the relationship manager's primary workspace; portfolio summary, action items, global search, recent activity.
- `BranchBankerPortal` — tablet-optimized in-branch onboarding surface.
- `BusinessClientPortal` — the customer-facing surface for `Customer` self-service: documents, loan requests, RM messaging.
- `AdminConsole` — compliance configuration, audit review, user management.

## How everything connects

The two big domains — client management and lending — share data heavily. A `LoanApplication` originates against a `Business` record. A `CovenantBreach` surfaces as an `Interaction` entry. `Balance` data on `DepositAccount` records feeds `GlobalCashFlow` calculations during `CreditAnalysis`. `User`-on-record information traces every action back through the `AuditTrail`. The `RM` is the customer's `SinglePointOfContact` from `Prospect` through `Onboarding`, through every `Loan`, through every `CrossSell` opportunity such as adding new `TreasuryServices`, and through every `Referral` they introduce — that is the community-bank promise, and the data model is laid out to keep it visible at every stage.
