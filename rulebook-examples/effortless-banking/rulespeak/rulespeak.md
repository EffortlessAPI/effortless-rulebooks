# 📘 Effortless Banking — RuleSpeak

_Community-bank commercial RM platform — loans, deposits, covenants, BSA/AML._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **User** | Bank employees: relationship managers, underwriters, branch bankers, and admins. Used for portfolio assignment, audit trails, and segregation-of-duties enforcement. |
| Is RM | True when Role = 'RM' (relationship manager). |
| Is Underwriter | True when Role = 'Underwriter'. |
| Is Branch Banker | True when Role = 'BranchBanker'. |
| Is Admin | True when Role = 'Admin'. |
| Count of Portfolio Businesses | Number of Businesses whose RelationshipManager is this user. |
| Count of Originated Loans | Number of Loans this user originated as the RM of record. |
| Count of Underwritten Loans | Number of Loans this user underwrote. |
| **Business** | Small-business customers (and prospects) of the bank. Central entity: BeneficialOwners, Contacts, Accounts, Loans, Documents, and Interactions all hang off a Business. BusinessProfile information (legal name, structure, NAICS code) lives directly on this table. |
| Relationship Manager Label | Display label of the assigned RM, pulled from Users. |
| Is Customer | True when Status = 'Customer'. |
| Is Prospect | True when Status = 'Prospect'. |
| Was Referred | True when this business was referred in by another business. |
| Count of Beneficial Owners | Number of BeneficialOwner rows attached. |
| Count of Contacts | Number of Contact rows attached. |
| Count of Accounts | Number of Account rows attached. |
| Count of Loans | Number of Loan rows attached. |
| Count of Interactions | Number of Interaction rows attached (the activity feed). |
| Count of Documents | Number of Documents attached directly to this Business. |
| Total Deposit Balance USD | Sum of CurrentBalanceUsd across this Business's Accounts. |
| Total Loan Principal USD | Sum of PrincipalUsd across this Business's Loans. |
| Count of Classified Loans | Number of Loans on this Business whose risk grade is substandard or worse (>=7). |
| Has Classified Loan | True when at least one loan on this Business is a classified asset. |
| Beneficial Owners At CDD Threshold | Count of BeneficialOwners meeting FinCEN's 25% threshold or marked as the control person. |
| Meets CDD Rule | True when CDD beneficial-ownership collection is satisfied: at least one owner crosses the 25% threshold, or a control person is designated. |
| Portfolio Priority | Higher-order: portfolio-management priority bucket for this business. 'High' when classified loan present, 'Medium' when CDD or onboarding incomplete or no loans yet, otherwise 'Low'. |
| **Beneficial Owner** | Individuals owning 25%+ of a Business plus designated control persons, per FinCEN's CDD rule. PII (SSN, DOB, address) is stored encrypted at rest via field-level encryption. |
| Business Label | Display label of the parent Business. |
| Meets25 Percent Threshold | True when OwnershipPercentage >= 25. |
| Meets CDD Threshold | True when this row counts toward CDD compliance: meets 25% OR is the control person. |
| **Contact** | Non-owner individuals associated with a Business: officers, AP clerks, authorized signers, additional points of contact. Distinct from BeneficialOwners (which carries PII/CDD weight). |
| Business Label | Display label of the parent Business. |
| Is Officer | True when ContactType = 'Officer'. |
| Is AP Clerk | True when ContactType = 'APClerk'. |
| **Account** | Deposit accounts the Business holds at the bank (checking, savings, money market). Balances feed GlobalCashFlow during credit analysis. TreasuryServices enrollment flags (ACH, Wire, Card) are stored per-account. |
| Business Label | Display label of the parent Business. |
| Treasury Service Count | Number of treasury services enrolled on this account. |
| Has Any Treasury Service | True when at least one treasury service is enrolled. |
| **Loan** | Credit facilities extended to a Business, tracked from inquiry through funding, servicing, and payoff. RiskRating is denormalized here for fast reads; every change is captured in RiskRatingHistory. |
| Business Label | Display label of the borrower Business. |
| Business NAICS Code | NAICS code of the borrower Business (concentration analytics). |
| Originating RM Label | Display label of the originating RM. |
| Underwriter is Admin | True when the loan's underwriter holds the Admin role (looked up from Users.IsAdmin). |
| Is Funded | True when UnderwritingStage = 'Funded'. |
| Is Classified Asset | True when RiskRating is substandard (7) or worse. |
| DSCR in Band | True when DSCR meets or exceeds the 1.20 minimum band. |
| LTV in Band | True when LTV is at or below 0.80, or null (unsecured). |
| Segregation of Duties Ok | True when the originating RM and the underwriter are different Users (segregation-of-duties check). |
| Count of Covenants | Number of Covenants attached to this loan. |
| Count of Breached Covenants | Number of Covenants currently in Breached status on this loan. |
| Count of Risk Rating History | Number of RiskRatingHistory rows for this loan. |
| Count of Documents | Number of Documents attached directly to this Loan. |
| Has Breached Covenant | True when at least one covenant is in breach. |
| On Watchlist | Higher-order: on the watchlist when the loan is a classified asset OR has any breached covenant. |
| Health Score | Higher-order composite health score (0-4): +1 for DscrInBand, +1 for LtvInBand, +1 for NOT IsClassifiedAsset, +1 for NOT HasBreachedCovenant. |
| **Covenant** | Conditions attached to a Loan that must be tested on a recurring schedule (e.g. minimum DSCR each quarter). CovenantMonitoring runs the tickler calendar ahead of NextTestDate; breaches surface as SystemEvent Interactions. |
| Loan Label | Display label of the parent Loan. |
| Loan Business | Business of the parent Loan (chained lookup). |
| Is Breached | True when Status = 'Breached'. |
| Has Active Waiver | True when CurrentWaiverThrough is set (waiver is on file). |
| **Risk Rating History** | Time-series of risk-grade changes on a Loan. Regulators audit rating drift, so every migration is captured here. AnnualReview also writes a row even when the grade is reaffirmed unchanged. |
| Loan Label | Display label of the parent Loan. |
| Changed by User Label | Display label of the user who recorded the change. |
| Grade Delta | NewGrade - PriorGrade (positive = downgrade in this scale); equals 0 on initial rating. |
| Is Downgrade | True when the migration represents a downgrade (higher grade number = worse risk). |
| Is Initial Rating | True when this is the loan's first rating row (no PriorGrade). |
| Crossed Classified Threshold | Higher-order: true when this migration is the moment a loan crossed from non-classified to classified (PriorGrade < 7 and NewGrade >= 7). |
| **Document** | Files in the DocumentVault. A document attaches to either a Business (tax returns, formation docs) or a Loan (note, security agreement, appraisal). Both FKs are nullable so a document can hang off the appropriate parent. |
| Business Label | Display label of the attached Business (if any). |
| Loan Label | Display label of the attached Loan (if any). |
| Uploaded by User Label | Display label of the uploader (if known). |
| Attached to | Which parent this document hangs off: 'Loan', 'Business', or 'Orphan' (neither set). |
| From Customer Portal | True when uploaded by the customer themselves via the BusinessClientPortal. |
| **Interaction** | Unified activity-log feed for a Business. An Interaction is intentionally generic: InteractionType discriminates Note, Call, Visit, Task, Meeting, or SystemEvent. Covenant breaches, document requests, and other machine actions are written as SystemEvent interactions so they appear in the same stream as human-logged activity. |
| Business Label | Display label of the parent Business. |
| User Label | Display label of the logging user (if any). |
| Is System Event | True for system-generated rows. |
| Is Task | True for Task-type rows. |
| From Customer | True when sourced from the BusinessClientPortal (customer-originated). |
| Is Covenant Event | Higher-order: true when this is a covenant-related system event (subject contains 'covenant'). |

## 2 Fact Types

- a **business** references exactly one **user**
- a **business** may reference one **business**
- a **beneficial owner** references exactly one **business**
- a **contact** references exactly one **business**
- an **account** references exactly one **business**
- a **loan** references exactly one **business**
- a **loan** references exactly one **user**
- a **covenant** references exactly one **loan**
- a **risk rating history** references exactly one **loan**
- a **risk rating history** references exactly one **user**
- a **document** may reference one **business**
- a **document** may reference one **loan**
- a **document** may reference one **user**
- an **interaction** references exactly one **business**
- an **interaction** may reference one **user**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Is RM** | A user is considered an RM if the role is the literal “RM”. |
| **DR-2 Is Underwriter** | A user is considered an underwriter if the role is the literal “Underwriter”. |
| **DR-3 Is Branch Banker** | A user is considered a branch banker if the role is the literal “BranchBanker”. |
| **DR-4 Is Admin** | A user is considered an admin if the role is the literal “Admin”. |
| **DR-5 Count of Portfolio Businesses** | A user's count of portfolio businesses is the number of businesses related to the user. |
| **DR-6 Count of Originated Loans** | A user's count of originated loans is the number of loans related to the user. |
| **DR-7 Count of Underwritten Loans** | A user's count of underwritten loans is the number of loans related to the user. |
| **DR-8 Relationship Manager Label** | A business's relationship manager label is the full name of the business's relationship manager. |
| **DR-9 Is Customer** | A business is considered a customer if the status is the literal “Customer”. |
| **DR-10 Is Prospect** | A business is considered a prospect if the status is the literal “Prospect”. |
| **DR-11 Was Referred** | A business is considered to have been referred if the referral source is set. |
| **DR-12 Count of Beneficial Owners** | A business's count of beneficial owners is the number of beneficial owners related to the business. |
| **DR-13 Count of Contacts** | A business's count of contacts is the number of contacts related to the business. |
| **DR-14 Count of Accounts** | A business's count of accounts is the number of accounts related to the business. |
| **DR-15 Count of Loans** | A business's count of loans is the number of loans related to the business. |
| **DR-16 Count of Interactions** | A business's count of interactions is the number of interactions related to the business. |
| **DR-17 Count of Documents** | A business's count of documents is the number of documents related to the business. |
| **DR-18 Total Deposit Balance USD** | A business's total deposit balance USD is the total current balance USD across the accounts related to the business. |
| **DR-19 Total Loan Principal USD** | A business's total loan principal USD is the total principal USD across the loans related to the business. |
| **DR-20 Count of Classified Loans** | A business's count of classified loans is the number of the business's loans that are classified assets. |
| **DR-21 Has Classified Loan** | A business is considered to have a classified loan if the count of classified loans is greater than 0. |
| **DR-22 Beneficial Owners At CDD Threshold** | A business's beneficial owners at CDD threshold is the number of the business's beneficial owners that meet CDD threshold. |
| **DR-23 Meets CDD Rule** | A business is considered to meet CDD rule if the beneficial owners at CDD threshold is greater than 0. |
| **DR-24 Portfolio Priority** | The business's portfolio priority is determined by the following priority:<br>1. the literal “High”, if the has classified loan flag is set;<br>2. the literal “Medium”, if at least one of the following holds: it is not the case that the meets CDD rule flag is set; the is prospect flag is set; or the count of loans is 0;<br>3. otherwise the literal “Low”. |
| **DR-25 Business Label** | A beneficial owner's business label is the legal name of the beneficial owner's business. |
| **DR-26 Meets25 Percent Threshold** | A beneficial owner is flagged meets25 percent threshold if the ownership percentage is at least 25. |
| **DR-27 Meets CDD Threshold** | A beneficial owner is considered to meet CDD threshold if at least one of the following holds: the meets25 percent threshold flag is set or the is control person flag is set. |
| **DR-28 Business Label** | A contact's business label is the legal name of the contact's business. |
| **DR-29 Is Officer** | A contact is considered an officer if the contact type is the literal “Officer”. |
| **DR-30 Is AP Clerk** | A contact is considered an AP clerk if the contact type is the literal “APClerk”. |
| **DR-31 Business Label** | An account's business label is the legal name of the account's business. |
| **DR-32 Treasury Service Count** | An account's treasury service count is computed as the number of the following that hold: the has ACH flag is set; the has wire flag is set; and the has card flag is set. |
| **DR-33 Has Any Treasury Service** | An account is considered to have any treasury service if the treasury service count is greater than 0. |
| **DR-34 Business Label** | A loan's business label is the legal name of the loan's business. |
| **DR-35 Business NAICS Code** | A loan's business NAICS code is the NAICS code of the loan's business. |
| **DR-36 Originating RM Label** | A loan's originating RM label is the full name of the loan's originating RM. |
| **DR-37 Underwriter is Admin** | A loan's underwriter is admin is true when the loan's underwriter is an admin. |
| **DR-38 Is Funded** | A loan is considered funded if the underwriting stage is the literal “Funded”. |
| **DR-39 Is Classified Asset** | A loan is considered a classified asset if the risk rating is at least 7. |
| **DR-40 DSCR in Band** | A loan is flagged DSCR in band if the DSCR (a missing value counts as 0) is at least 1.20. |
| **DR-41 LTV in Band** | A loan is flagged LTV in band if the LTV (a missing value counts as 0) is at most 0.80. |
| **DR-42 Segregation of Duties Ok** | A loan is flagged segregation of duties ok if it is not the case that the originating RM is the underwriter. |
| **DR-43 Count of Covenants** | A loan's count of covenants is the number of covenants related to the loan. |
| **DR-44 Count of Breached Covenants** | A loan's count of breached covenants is the number of the loan's covenants that are breached. |
| **DR-45 Count of Risk Rating History** | A loan's count of risk rating history is the number of risk rating history related to the loan. |
| **DR-46 Count of Documents** | A loan's count of documents is the number of documents related to the loan. |
| **DR-47 Has Breached Covenant** | A loan is considered to have a breached covenant if the count of breached covenants is greater than 0. |
| **DR-48 On Watchlist** | A loan is flagged on watchlist if at least one of the following holds: the is classified asset flag is set or the has breached covenant flag is set. |
| **DR-49 Health Score** | A loan's health score is computed as `If(DscrInBand, 1, 0) + If(LtvInBand, 1, 0) + If(IsClassifiedAsset, 0, 1) + If(HasBreachedCovenant, 0, 1)`. |
| **DR-50 Loan Label** | A covenant's loan label is the loan number of the covenant's loan. |
| **DR-51 Loan Business** | A covenant's loan business is the business of the covenant's loan. |
| **DR-52 Is Breached** | A covenant is considered breached if the status is the literal “Breached”. |
| **DR-53 Has Active Waiver** | A covenant is considered to have an active waiver if the current waiver through is set. |
| **DR-54 Loan Label** | A risk rating history's loan label is the loan number of the risk rating history's loan. |
| **DR-55 Changed by User Label** | A risk rating history's changed by user label is the full name of the risk rating history's changed by user. |
| **DR-56 Grade Delta** | A risk rating history's grade delta is computed as `NewGrade - Coalesce(PriorGrade, NewGrade)`. |
| **DR-57 Is Downgrade** | A risk rating history is considered a downgrade if the grade delta is greater than 0. |
| **DR-58 Is Initial Rating** | A risk rating history is considered initial rating if the prior grade (a missing value counts as 0) is 0. |
| **DR-59 Crossed Classified Threshold** | A risk rating history is flagged crossed classified threshold if all of the following hold: the prior grade (a missing value counts as 0) is less than 7 and the new grade is at least 7. |
| **DR-60 Business Label** | A document's business label is the legal name of the document's business. |
| **DR-61 Loan Label** | A document's loan label is the loan number of the document's loan. |
| **DR-62 Uploaded by User Label** | A document's uploaded by user label is the full name of the document's uploaded by user. |
| **DR-63 Attached to** | The document's attached to is determined by the following priority:<br>1. the literal “Loan”, if the loan is set;<br>2. the literal “Business”, if the business is set;<br>3. otherwise the literal “Orphan”. |
| **DR-64 From Customer Portal** | A document is flagged from customer portal if the uploaded via is the literal “BusinessClientPortal”. |
| **DR-65 Business Label** | An interaction's business label is the legal name of the interaction's business. |
| **DR-66 User Label** | An interaction's user label is the full name of the interaction's user. |
| **DR-67 Is System Event** | An interaction is considered a system event if the interaction type is the literal “SystemEvent”. |
| **DR-68 Is Task** | An interaction is considered a task if the interaction type is the literal “Task”. |
| **DR-69 From Customer** | An interaction is flagged from customer if the source is the literal “BusinessClientPortal”. |
| **DR-70 Is Covenant Event** | An interaction is considered a covenant event if all of the following hold: the is system event flag is set and it is not the case that `Iferror(Find("covenant", Lower(Subject)), 0)` is 0. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Users.IsRM** | formula | `Role = "RM"` |
| **Users.IsUnderwriter** | formula | `Role = "Underwriter"` |
| **Users.IsBranchBanker** | formula | `Role = "BranchBanker"` |
| **Users.IsAdmin** | formula | `Role = "Admin"` |
| **Users.CountOfPortfolioBusinesses** | rollup | `Count(Businesses via RelationshipManager)` |
| **Users.CountOfOriginatedLoans** | rollup | `Count(Loans via OriginatingRm)` |
| **Users.CountOfUnderwrittenLoans** | rollup | `Count(Loans via Underwriter)` |
| **Businesses.RelationshipManagerLabel** | lookup | `Lookup(Users.FullName via RelationshipManager)` |
| **Businesses.IsCustomer** | formula | `Status = "Customer"` |
| **Businesses.IsProspect** | formula | `Status = "Prospect"` |
| **Businesses.WasReferred** | formula | `Not(Coalesce(ReferralSource, "") = "")` |
| **Businesses.CountOfBeneficialOwners** | rollup | `Count(BeneficialOwners via Business)` |
| **Businesses.CountOfContacts** | rollup | `Count(Contacts via Business)` |
| **Businesses.CountOfAccounts** | rollup | `Count(Accounts via Business)` |
| **Businesses.CountOfLoans** | rollup | `Count(Loans via Business)` |
| **Businesses.CountOfInteractions** | rollup | `Count(Interactions via Business)` |
| **Businesses.CountOfDocuments** | rollup | `Count(Documents via Business)` |
| **Businesses.TotalDepositBalanceUsd** | rollup | `Sum(Accounts.CurrentBalanceUsd via Business)` |
| **Businesses.TotalLoanPrincipalUsd** | rollup | `Sum(Loans.PrincipalUsd via Business)` |
| **Businesses.CountOfClassifiedLoans** | rollup | `Count(Loans.Business, Name, Loans.IsClassifiedAsset, True())` |
| **Businesses.HasClassifiedLoan** | formula | `CountOfClassifiedLoans > 0` |
| **Businesses.BeneficialOwnersAtCddThreshold** | rollup | `Count(BeneficialOwners.Business, Name, BeneficialOwners.MeetsCddThreshold, True())` |
| **Businesses.MeetsCddRule** | formula | `BeneficialOwnersAtCddThreshold > 0` |
| **Businesses.PortfolioPriority** | formula | `If(HasClassifiedLoan, "High", If(Or(Not(MeetsCddRule), IsProspect, CountOfLoans = 0), "Medium", "Low"))` |
| **BeneficialOwners.BusinessLabel** | lookup | `Lookup(Businesses.LegalName via Business)` |
| **BeneficialOwners.Meets25PercentThreshold** | formula | `OwnershipPercentage >= 25` |
| **BeneficialOwners.MeetsCddThreshold** | formula | `Or(Meets25PercentThreshold, IsControlPerson)` |
| **Contacts.BusinessLabel** | lookup | `Lookup(Businesses.LegalName via Business)` |
| **Contacts.IsOfficer** | formula | `ContactType = "Officer"` |
| **Contacts.IsApClerk** | formula | `ContactType = "APClerk"` |
| **Accounts.BusinessLabel** | lookup | `Lookup(Businesses.LegalName via Business)` |
| **Accounts.TreasuryServiceCount** | formula | `If(HasAch, 1, 0) + If(HasWire, 1, 0) + If(HasCard, 1, 0)` |
| **Accounts.HasAnyTreasuryService** | formula | `TreasuryServiceCount > 0` |
| **Loans.BusinessLabel** | lookup | `Lookup(Businesses.LegalName via Business)` |
| **Loans.BusinessNaicsCode** | lookup | `Lookup(Businesses.NaicsCode via Business)` |
| **Loans.OriginatingRmLabel** | lookup | `Lookup(Users.FullName via OriginatingRm)` |
| **Loans.UnderwriterIsAdmin** | lookup | `Lookup(Users.IsAdmin via Underwriter)` |
| **Loans.IsFunded** | formula | `UnderwritingStage = "Funded"` |
| **Loans.IsClassifiedAsset** | formula | `RiskRating >= 7` |
| **Loans.DscrInBand** | formula | `Coalesce(Dscr, 0) >= 1.20` |
| **Loans.LtvInBand** | formula | `Coalesce(Ltv, 0) <= 0.80` |
| **Loans.SegregationOfDutiesOk** | formula | `Not(OriginatingRm = Underwriter)` |
| **Loans.CountOfCovenants** | rollup | `Count(Covenants via Loan)` |
| **Loans.CountOfBreachedCovenants** | rollup | `Count(Covenants.Loan, Name, Covenants.IsBreached, True())` |
| **Loans.CountOfRiskRatingHistory** | rollup | `Count(RiskRatingHistory via Loan)` |
| **Loans.CountOfDocuments** | rollup | `Count(Documents via Loan)` |
| **Loans.HasBreachedCovenant** | formula | `CountOfBreachedCovenants > 0` |
| **Loans.OnWatchlist** | formula | `Or(IsClassifiedAsset, HasBreachedCovenant)` |
| **Loans.HealthScore** | formula | `If(DscrInBand, 1, 0) + If(LtvInBand, 1, 0) + If(IsClassifiedAsset, 0, 1) + If(HasBreachedCovenant, 0, 1)` |
| **Covenants.LoanLabel** | lookup | `Lookup(Loans.LoanNumber via Loan)` |
| **Covenants.LoanBusiness** | lookup | `Lookup(Loans.Business via Loan)` |
| **Covenants.IsBreached** | formula | `Status = "Breached"` |
| **Covenants.HasActiveWaiver** | formula | `Not(Coalesce(CurrentWaiverThrough, "") = "")` |
| **RiskRatingHistory.LoanLabel** | lookup | `Lookup(Loans.LoanNumber via Loan)` |
| **RiskRatingHistory.ChangedByUserLabel** | lookup | `Lookup(Users.FullName via ChangedByUser)` |
| **RiskRatingHistory.GradeDelta** | formula | `NewGrade - Coalesce(PriorGrade, NewGrade)` |
| **RiskRatingHistory.IsDowngrade** | formula | `GradeDelta > 0` |
| **RiskRatingHistory.IsInitialRating** | formula | `Coalesce(PriorGrade, 0) = 0` |
| **RiskRatingHistory.CrossedClassifiedThreshold** | formula | `And(Coalesce(PriorGrade, 0) < 7, NewGrade >= 7)` |
| **Documents.BusinessLabel** | lookup | `Lookup(Businesses.LegalName via Business)` |
| **Documents.LoanLabel** | lookup | `Lookup(Loans.LoanNumber via Loan)` |
| **Documents.UploadedByUserLabel** | lookup | `Lookup(Users.FullName via UploadedByUser)` |
| **Documents.AttachedTo** | formula | `If(Not(Coalesce(Loan, "") = ""), "Loan", If(Not(Coalesce(Business, "") = ""), "Business", "Orphan"))` |
| **Documents.FromCustomerPortal** | formula | `UploadedVia = "BusinessClientPortal"` |
| **Interactions.BusinessLabel** | lookup | `Lookup(Businesses.LegalName via Business)` |
| **Interactions.UserLabel** | lookup | `Lookup(Users.FullName via User)` |
| **Interactions.IsSystemEvent** | formula | `InteractionType = "SystemEvent"` |
| **Interactions.IsTask** | formula | `InteractionType = "Task"` |
| **Interactions.FromCustomer** | formula | `Source = "BusinessClientPortal"` |
| **Interactions.IsCovenantEvent** | formula | `And(IsSystemEvent, Not(Iferror(Find("covenant", Lower(Subject)), 0) = 0))` |
