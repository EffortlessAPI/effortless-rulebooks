# Scenarios — Small Business Banking Client Manager

Concrete scenarios that exercise each business rule and feature from the narrative. Mock data in this directory is sized to support every scenario below.

---

## S1. A new business onboards through the branch banker portal

**Actors:** `BranchBanker` Maya Chen; `Business` "Stillwater Roasters LLC"

Maya, on a tablet at the Stillwater branch, runs `KYB` for a new commercial customer. Stillwater Roasters has two beneficial owners: Jordan Park (60%) and Sam Rivera (40%). Both are recorded as `BeneficialOwner` rows since each exceeds the 25% threshold. No `ControlPerson` row is needed because Park already exceeds 25%. SSNs are stored under `FieldLevelEncryption`. `SanctionsScreening` against `OFAC` runs and clears both owners. A `BusinessProfile` is created with NAICS 311920 (Coffee and Tea Manufacturing). The business opens a primary checking `DepositAccount`. An `Onboarding`-type `Interaction` is logged.

**Exercises:** `KYB`, `BeneficialOwner` 25% threshold rule, `ControlPerson` rule (not needed here), `BusinessProfile`, `NAICS`, `SanctionsScreening` / `OFAC`, `FieldLevelEncryption`, `EncryptedAtRest`, `DepositAccount`, `BranchBankerPortal`.

## S2. A solo founder requires a control person record

**Actors:** `RM` Devon Marshall; `Business` "Ridgeway Consulting LLC"; `ControlPerson` Alex Ridgeway (CEO, 0% equity)

Ridgeway Consulting is owned 100% by a holding entity, so no individual hits the 25% threshold. Per FinCEN's CDD rule, a single `ControlPerson` must be designated. Alex Ridgeway (CEO) is recorded as the control person. No `BeneficialOwner` rows exist for individuals (the holding entity is recorded separately).

**Exercises:** `CDD` rule edge case — `ControlPerson` required when no owner crosses 25%; FinCEN compliance.

## S3. RM logs a mixed activity day on the portfolio

**Actors:** `RM` Devon Marshall; `Business` records "Stillwater Roasters LLC", "Ridgeway Consulting LLC", "Pine Hardware Inc."

Devon's day produces six `Interaction` rows of various `InteractionType` values:
- `Note` on Stillwater after reading their Q2 P&L
- `Call` logged with Sam Rivera (Stillwater) about a wire-transfer fee question
- `Visit` to Pine Hardware to discuss a refinance
- `Meeting` scheduled with Ridgeway for `AnnualReview`
- `Task` due in three days: "follow up on Pine refinance term sheet"
- `SystemEvent` auto-generated: "Covenant test approaching on Pine Hardware Loan L-0042"

All six appear in the unified `InteractionLog` on each respective `Business`. The `RMDashboard` shows the task in action items and the system event in alerts.

**Exercises:** `Interaction`, every `InteractionType`, `InteractionLog`, `RMDashboard`, the design choice that system events live in the same stream as human-logged activity.

## S4. Loan origination — Stillwater applies for a $250K equipment loan

**Actors:** `RM` Devon Marshall; `Underwriter` Priya Iyer; `Business` Stillwater Roasters

The journey runs through every `UnderwritingStage`:
1. `Inquiry` — Jordan Park calls Devon asking about financing a $250K roaster.
2. `PreQualification` — passes; deposit history and revenue support it.
3. `LoanApplication` L-0051 created.
4. **Automated** — `KYB` refreshed (no changes); `CreditPull` fires to `Experian`, `DnB`, and `Equifax`; `UCCLienSearch` clears; `SecretaryOfState` confirms good standing; `CourtRecord` clear.
5. **RM packaging** — Devon collects two years of `FinancialStatement` submissions plus a Q3 interim into the `DocumentVault`; `OCR` indexes them.
6. `CreditAnalysis` — Priya does `StatementSpreading`. Computes `DSCR` = 1.42, `LTV` = 78%, `GlobalCashFlow` including owner K-1 income. Pulls `AccountingFeed` from `QuickBooks` via `Plaid` for fresh trailing-90 numbers.
7. `CommitteeApproval` — loan exceeds $200K threshold; `ApprovalRouting` configured in `WorkflowEngine` sends to credit committee; approved with conditions.
8. `ClosingPrep` — Devon re-enters the flow as `SinglePointOfContact`; conditions cleared; loan docs drafted in `DocumentVault`.
9. **Signing & Funding** — Jordan executes via `ESignature`; `Funding` disburses $250K. The whole journey took 19 days, within the 14–21 day `SLA`.

Three `Covenant` records are attached to L-0051: minimum DSCR 1.20 (quarterly `TestSchedule`), maximum debt/EBITDA 3.0 (annual), and annual financial-statement submission.

**Exercises:** every step of `LoanOrigination`, `LoanPipeline`, `UnderwritingStage` sequence, `CreditPull` to all three bureaus, `PublicRecords` checks, `StatementSpreading`, `DSCR` / `LTV` / `GlobalCashFlow` computation, `AccountingFeed` from `QuickBooks`/`Plaid`, `WorkflowEngine` / `ApprovalRouting`, `CommitteeApproval`, `ClosingPrep` RM re-entry, `ESignature`, `Funding`, `SLA` enforcement, `Covenant` creation with `TestSchedule`.

## S5. Covenant breach surfaces as a system event

**Actors:** `Business` Pine Hardware Inc.; `Loan` L-0042; `RM` Devon Marshall

Pine Hardware's Q3 financials come in. Priya's `StatementSpreading` produces DSCR of 1.08 — below the covenant minimum of 1.20. The result is a `CovenantBreach`. `CovenantMonitoring` records it; a `SystemEvent` `Interaction` posts to Pine Hardware's `InteractionLog`; Devon receives a `Notification` via email and in-app `NotificationChannel`. Devon documents the customer's response (a one-time spike in raw-material costs) and the committee approves a `CovenantWaiver` valid through the next test. The waiver is logged against the covenant; a lighter `CovenantException` would have been used had the deviation not required formal sign-off.

**Exercises:** `CovenantMonitoring`, `TicklerCalendar` (fired ahead of the test date), `CovenantBreach`, `CovenantWaiver` vs `CovenantException`, breach surfaces as `SystemEvent` `Interaction`, multi-channel `Notification`.

## S6. Risk grade migration and watchlist

**Actors:** `Loan` L-0042 Pine Hardware

Following S5, the underwriter recommends a risk-grade migration from 4 (Pass) to 6 (Special Mention). The change writes a new `RiskRatingHistory` row capturing the prior grade, new grade, date, reason, and user. The denormalized `Loan.risk_rating` updates to 6. L-0042 does not yet hit the substandard threshold so it is **not** a `ClassifiedAsset` and is not added to the `Watchlist`. Three months later the grade drops to 7 (Substandard); the loan now becomes a classified asset and lands on the watchlist.

**Exercises:** `RiskRating`, `RiskGrade`, `RiskGradeMigration`, `RiskRatingHistory` time-series, `ClassifiedAsset` threshold rule, `Watchlist` membership.

## S7. Annual review re-underwrites every loan

**Actors:** `Loan` L-0051 (Stillwater)

Twelve months post-funding, an `AnnualReview` fires for L-0051. The RM gathers fresh `FinancialStatement` files; the underwriter re-spreads and re-computes DSCR/LTV; covenant compliance is re-checked; the risk grade is reaffirmed at 4. A new `RiskRatingHistory` row is created (even if grade unchanged, the review date is captured).

**Exercises:** `AnnualReview`, repeat `StatementSpreading`, `RiskRatingHistory` entry on every review.

## S8. Delinquency → collections → loss mitigation

**Actors:** `Loan` L-0042 (Pine Hardware)

Pine Hardware misses a payment by 10 days. `PaymentProcessing` flags the miss; the `DelinquencyWorkflow` fires reminders. At 60 days the `CollectionsWorkflow` opens. The bank negotiates a modification (interest-only for 6 months) — a `LossMitigation` action — captured against the loan.

**Exercises:** `PaymentProcessing`, `DelinquencyWorkflow`, `CollectionsWorkflow`, `LossMitigation`.

## S9. Cross-sell, referral, prospect

**Actors:** `RM` Devon; `Prospect` "Hilltop Bakery"

Pine Hardware refers Hilltop Bakery to Devon — recorded as a `Referral` on a new `Prospect` record. The `Pipeline` (non-loan variant) tracks the prospect through awareness → engaged → onboarded. Once Hilltop becomes a `Customer`, a `CrossSell` opportunity is logged to enroll them in `ACH` origination and a business `Card` (`TreasuryServices` / `CashManagement`).

**Exercises:** `Prospect`, `Referral`, `Pipeline` (non-loan), `CrossSell`, `TreasuryServices`, `CashManagement`, `ACH`, `Card`.

## S10. BSA/AML monitoring and SAR filing

**Actors:** `Business` "Sunset Auto Body LLC"; `Verafin` (external)

`TransactionMonitoring` via Verafin flags a pattern of just-under-$10K cash deposits across Sunset Auto's `DepositAccount`. A `CTR` had already been filed on a separate $11K cash deposit. The pattern triggers a `SAR`; the compliance officer reviews and files via `RegulatoryReporting` to `FinCEN`. The investigation and filing are captured under the `AuditTrail`.

**Exercises:** `BSA`, `AML`, `TransactionMonitoring` / `Verafin`, `CTR`, `SAR`, `RegulatoryReporting`, `FinCEN`, `AuditTrail`.

## S11. Document attaches to either a Business or a Loan

A `Document` "2025-Tax-Return.pdf" attaches to the `Business` Stillwater (no loan link). A `Document` "L-0051-Promissory-Note.pdf" attaches to `Loan` L-0051 (no business link directly; reached via the loan's parent). The nullable-FK rule is validated.

**Exercises:** `Document` dual-parent nullable-FK rule, `DocumentVault`.

## S12. Search across everything

Devon types "Stillwater" into `GlobalSearch`. Results return the `Business` record, two `Loan` records, three `Document` files (matched by `OCR` text content, not just filename), and seven `Interaction` entries. `SearchAndIndexing` powers the unified result list.

**Exercises:** `GlobalSearch`, `SearchAndIndexing`, `OCR` indexing of documents.

## S13. Reporting & analytics — board pack and regulatory output

A quarterly cycle generates a `BoardPack` containing portfolio composition, watchlist summary, classified-asset trend, and `CECL` / `ALLL` reserve calculations. The same cycle produces the `CallReport` data extract for filing and the `HMDA` LAR (where applicable). The `PortfolioDashboard` on every RM's `RMDashboard` reflects the same underlying data live.

**Exercises:** `ReportingAndAnalytics`, `PortfolioDashboard`, `BoardPack`, `CECL`, `ALLL`, `CallReport`, `HMDA`, `FFIEC`, `OCC`.

## S14. Segregation of duties blocks dual-role action

`Underwriter` Priya cannot self-approve a loan she underwrote. When she attempts to advance L-0051 from `CreditAnalysis` directly to `Funding`, `IdentityAndAccess` blocks the action — `SegregationOfDuties` requires a separate approver. The `AuditTrail` records the blocked attempt.

**Exercises:** `IdentityAndAccess`, `RoleBasedAccess`, `SegregationOfDuties`, `AuditTrail`.

## S15. Business client portal — customer-facing actions

Jordan Park (Stillwater) logs into the `BusinessClientPortal` via `SSO` with `MFA`, uploads a Q4 P&L (auto-indexed by `OCR`, attached to the Stillwater `Business`), submits a $25K LOC inquiry (creates a new `Inquiry` for Devon), and sends a message to Devon (creates an `Interaction` of type Note tagged "from customer").

**Exercises:** `BusinessClientPortal`, `SSO` / `MFA`, customer-initiated `Document` upload, customer-initiated `Inquiry`, customer messaging produces `Interaction`.
