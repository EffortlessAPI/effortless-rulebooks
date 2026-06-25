# 📘 Effortless Banking — RuleSpeak

_Community-bank commercial RM platform — loans, deposits, covenants, BSA/AML._

> Deklarative Geschäftsregeln, aus dem Regelbuch gerendert. Jede Aussage
> unten drückt eine Wahrheit der Geschäftsdomäne aus — sie ist weder eine
> Prozedur noch ein Befehl. Die Formeln des Regelbuchs sind die einzige Quelle
> der Wahrheit; dieses Dokument ist ihre klarsprachliche Lesart.

## 1 Geschäftsvokabular

| Begriff | Beschreibung | Erläuternder Kommentar |
|---------|--------------|------------------------|
| **User** | Bank employees: relationship managers, underwriters, branch bankers, and admins. Used for portfolio assignment, audit trails, and segregation-of-duties enforcement. | — |
| Name | Berechnet als das kleingeschriebene full name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased compound primary key derived from FullName._ |
| Is RM | Wahr, wenn der role ist der Wert „RM“. | _True when Role = 'RM' (relationship manager)._ |
| Is Underwriter | Wahr, wenn der role ist der Wert „Underwriter“. | _True when Role = 'Underwriter'._ |
| Is Branch Banker | Wahr, wenn der role ist der Wert „BranchBanker“. | _True when Role = 'BranchBanker'._ |
| Is Admin | Wahr, wenn der role ist der Wert „Admin“. | _True when Role = 'Admin'._ |
| Count of Portfolio Businesses | Die Anzahl der mit dem user verbundenen businesses. | _Number of Businesses whose RelationshipManager is this user._ |
| Count of Originated Loans | Die Anzahl der mit dem user verbundenen loans. | _Number of Loans this user originated as the RM of record._ |
| Count of Underwritten Loans | Die Anzahl der mit dem user verbundenen loans. | _Number of Loans this user underwrote._ |
| **Business** | Small-business customers (and prospects) of the bank. Central entity: BeneficialOwners, Contacts, Accounts, Loans, Documents, and Interactions all hang off a Business. BusinessProfile information (legal name, structure, NAICS code) lives directly on this table. | — |
| Name | Berechnet als das kleingeschriebene legal name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird, wobei jedes ein Punkt durch eine leere Zeichenkette ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased compound PK derived from LegalName (punctuation stripped)._ |
| Relationship Manager Label | Der full name des zugehörigen relationship manager des business. | _Display label of the assigned RM, pulled from Users._ |
| Is Customer | Wahr, wenn der status ist der Wert „Customer“. | _True when Status = 'Customer'._ |
| Is Prospect | Wahr, wenn der status ist der Wert „Prospect“. | _True when Status = 'Prospect'._ |
| Was Referred | Wahr, wenn der referral source (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat. | _True when this business was referred in by another business._ |
| Count of Beneficial Owners | Die Anzahl der mit dem business verbundenen beneficial owners. | _Number of BeneficialOwner rows attached._ |
| Count of Contacts | Die Anzahl der mit dem business verbundenen contacts. | _Number of Contact rows attached._ |
| Count of Accounts | Die Anzahl der mit dem business verbundenen accounts. | _Number of Account rows attached._ |
| Count of Loans | Die Anzahl der mit dem business verbundenen loans. | _Number of Loan rows attached._ |
| Count of Interactions | Die Anzahl der mit dem business verbundenen interactions. | _Number of Interaction rows attached (the activity feed)._ |
| Count of Documents | Die Anzahl der mit dem business verbundenen documents. | _Number of Documents attached directly to this Business._ |
| Total Deposit Balance USD | Die Gesamtsumme von current balance USD über die mit dem business verbundenen accounts. | _Sum of CurrentBalanceUsd across this Business's Accounts._ |
| Total Loan Principal USD | Die Gesamtsumme von principal USD über die mit dem business verbundenen loans. | _Sum of PrincipalUsd across this Business's Loans._ |
| Count of Classified Loans | Die Anzahl der loans des business, die classified asset sind. | _Number of Loans on this Business whose risk grade is substandard or worse (>=7)._ |
| Has Classified Loan | Wahr, wenn der count of classified loans größer ist als 0. | _True when at least one loan on this Business is a classified asset._ |
| Beneficial Owners At CDD Threshold | Die Anzahl der beneficial owners des business, die CDD threshold erfüllen. | _Count of BeneficialOwners meeting FinCEN's 25% threshold or marked as the control person._ |
| Meets CDD Rule | Wahr, wenn der beneficial owners at CDD threshold größer ist als 0. | _True when CDD beneficial-ownership collection is satisfied: at least one owner crosses the 25% threshold, or a control person is designated._ |
| Portfolio Priority | Nach Priorität bestimmt: der Wert „High“, wenn das has classified loan-Kennzeichen gesetzt ist; der Wert „Medium“, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das meets CDD rule-Kennzeichen gesetzt ist; das is prospect-Kennzeichen gesetzt ist; oder der count of loans ist 0; andernfalls der Wert „Low“. | _Higher-order: portfolio-management priority bucket for this business. 'High' when classified loan present, 'Medium' when CDD or onboarding incomplete or no loans yet, otherwise 'Low'._ |
| **Beneficial Owner** | Individuals owning 25%+ of a Business plus designated control persons, per FinCEN's CDD rule. PII (SSN, DOB, address) is stored encrypted at rest via field-level encryption. | — |
| Name | Berechnet als der business, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene full name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased compound PK: {Business}-{FullName slug}._ |
| Business Label | Der legal name des zugehörigen business des beneficial owner. | _Display label of the parent Business._ |
| Meets25 Percent Threshold | Wahr, wenn der ownership percentage mindestens 25 ist. | _True when OwnershipPercentage >= 25._ |
| Meets CDD Threshold | Wahr, wenn mindestens eines des Folgenden gilt: das meets25 percent threshold-Kennzeichen gesetzt ist oder das is control person-Kennzeichen gesetzt ist. | _True when this row counts toward CDD compliance: meets 25% OR is the control person._ |
| **Contact** | Non-owner individuals associated with a Business: officers, AP clerks, authorized signers, additional points of contact. Distinct from BeneficialOwners (which carries PII/CDD weight). | — |
| Name | Berechnet als der business, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene full name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene title, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased compound PK: {Business}-{FullName slug}-{Title slug}._ |
| Business Label | Der legal name des zugehörigen business des contact. | _Display label of the parent Business._ |
| Is Officer | Wahr, wenn der contact type ist der Wert „Officer“. | _True when ContactType = 'Officer'._ |
| Is AP Clerk | Wahr, wenn der contact type ist der Wert „APClerk“. | _True when ContactType = 'APClerk'._ |
| **Account** | Deposit accounts the Business holds at the bank (checking, savings, money market). Balances feed GlobalCashFlow during credit analysis. TreasuryServices enrollment flags (ACH, Wire, Card) are stored per-account. | — |
| Name | Berechnet als der business, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene account type, gefolgt von ein Bindestrich, gefolgt von der account number last4. | _Kebab-cased compound PK: {Business}-{AccountType}-{Last4}._ |
| Business Label | Der legal name des zugehörigen business des account. | _Display label of the parent Business._ |
| Treasury Service Count | Berechnet als die Anzahl der folgenden zutreffenden Bedingungen: das has ACH-Kennzeichen gesetzt ist; das has wire-Kennzeichen gesetzt ist; und das has card-Kennzeichen gesetzt ist. | _Number of treasury services enrolled on this account._ |
| Has Any Treasury Service | Wahr, wenn der treasury service count größer ist als 0. | _True when at least one treasury service is enrolled._ |
| **Loan** | Credit facilities extended to a Business, tracked from inquiry through funding, servicing, and payoff. RiskRating is denormalized here for fast reads; every change is captured in RiskRatingHistory. | — |
| Name | Berechnet als das kleingeschriebene loan number, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased PK derived from LoanNumber (e.g. 'L-0051' -> 'l-0051')._ |
| Business Label | Der legal name des zugehörigen business des loan. | _Display label of the borrower Business._ |
| Business NAICS Code | Der NAICS code des zugehörigen business des loan. | _NAICS code of the borrower Business (concentration analytics)._ |
| Originating RM Label | Der full name des zugehörigen originating RM des loan. | _Display label of the originating RM._ |
| Underwriter is Admin | Wahr, wenn der zugehörige underwriter des loan ist ein admin. | _True when the loan's underwriter holds the Admin role (looked up from Users.IsAdmin)._ |
| Is Funded | Wahr, wenn der underwriting stage ist der Wert „Funded“. | _True when UnderwritingStage = 'Funded'._ |
| Is Classified Asset | Wahr, wenn der risk rating mindestens 7 ist. | _True when RiskRating is substandard (7) or worse._ |
| DSCR in Band | Wahr, wenn der DSCR (ein fehlender Wert zählt als 0) mindestens 1.20 ist. | _True when DSCR meets or exceeds the 1.20 minimum band._ |
| LTV in Band | Wahr, wenn der LTV (ein fehlender Wert zählt als 0) höchstens 0.80 ist. | _True when LTV is at or below 0.80, or null (unsecured)._ |
| Segregation of Duties Ok | Wahr, wenn es nicht der Fall ist, dass der originating RM ist der underwriter. | _True when the originating RM and the underwriter are different Users (segregation-of-duties check)._ |
| Count of Covenants | Die Anzahl der mit dem loan verbundenen covenants. | _Number of Covenants attached to this loan._ |
| Count of Breached Covenants | Die Anzahl der covenants des loan, die breached sind. | _Number of Covenants currently in Breached status on this loan._ |
| Count of Risk Rating History | Die Anzahl der mit dem loan verbundenen risk rating history. | _Number of RiskRatingHistory rows for this loan._ |
| Count of Documents | Die Anzahl der mit dem loan verbundenen documents. | _Number of Documents attached directly to this Loan._ |
| Has Breached Covenant | Wahr, wenn der count of breached covenants größer ist als 0. | _True when at least one covenant is in breach._ |
| On Watchlist | Wahr, wenn mindestens eines des Folgenden gilt: das is classified asset-Kennzeichen gesetzt ist oder das has breached covenant-Kennzeichen gesetzt ist. | _Higher-order: on the watchlist when the loan is a classified asset OR has any breached covenant._ |
| Health Score | Berechnet als die Anzahl der folgenden zutreffenden Bedingungen: das DSCR in band-Kennzeichen gesetzt ist; das LTV in band-Kennzeichen gesetzt ist; das is classified asset-Kennzeichen nicht gesetzt ist; und das has breached covenant-Kennzeichen nicht gesetzt ist. | _Higher-order composite health score (0-4): +1 for DscrInBand, +1 for LtvInBand, +1 for NOT IsClassifiedAsset, +1 for NOT HasBreachedCovenant._ |
| **Covenant** | Conditions attached to a Loan that must be tested on a recurring schedule (e.g. minimum DSCR each quarter). CovenantMonitoring runs the tickler calendar ahead of NextTestDate; breaches surface as SystemEvent Interactions. | — |
| Name | Berechnet als der loan, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene covenant type, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased compound PK: {Loan}-{CovenantType slug}._ |
| Loan Label | Der loan number des zugehörigen loan des covenant. | _Display label of the parent Loan._ |
| Loan Business | Der business des zugehörigen loan des covenant. | _Business of the parent Loan (chained lookup)._ |
| Is Breached | Wahr, wenn der status ist der Wert „Breached“. | _True when Status = 'Breached'._ |
| Has Active Waiver | Wahr, wenn der current waiver through (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat. | _True when CurrentWaiverThrough is set (waiver is on file)._ |
| **Risk Rating History** | Time-series of risk-grade changes on a Loan. Regulators audit rating drift, so every migration is captured here. AnnualReview also writes a row even when the grade is reaffirmed unchanged. | — |
| Name | Berechnet als der loan, gefolgt von ein Bindestrich, gefolgt von der TEXT von der effective date und der Wert „yyyy-mm-dd“, gefolgt von der Wert „-grade-“, gefolgt von der TEXT von der new grade und der Wert „0“. | _Kebab-cased compound PK: {Loan}-{EffectiveDate}-grade-{NewGrade}._ |
| Loan Label | Der loan number des zugehörigen loan des risk rating history. | _Display label of the parent Loan._ |
| Changed by User Label | Der full name des zugehörigen changed by user des risk rating history. | _Display label of the user who recorded the change._ |
| Grade Delta | Berechnet als der new grade minus der prior grade (ein fehlender Wert zählt als der new grade). | _NewGrade - PriorGrade (positive = downgrade in this scale); equals 0 on initial rating._ |
| Is Downgrade | Wahr, wenn der grade delta größer ist als 0. | _True when the migration represents a downgrade (higher grade number = worse risk)._ |
| Is Initial Rating | Wahr, wenn der prior grade (ein fehlender Wert zählt als 0) ist 0. | _True when this is the loan's first rating row (no PriorGrade)._ |
| Crossed Classified Threshold | Wahr, wenn alles Folgende gilt: der prior grade (ein fehlender Wert zählt als 0) kleiner ist als 7 und der new grade mindestens 7 ist. | _Higher-order: true when this migration is the moment a loan crossed from non-classified to classified (PriorGrade < 7 and NewGrade >= 7)._ |
| **Document** | Files in the DocumentVault. A document attaches to either a Business (tax returns, formation docs) or a Loan (note, security agreement, appraisal). Both FKs are nullable so a document can hang off the appropriate parent. | — |
| Name | Berechnet als das kleingeschriebene filename, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird, wobei jedes ein Punkt durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased PK derived from the filename._ |
| Business Label | Der legal name des zugehörigen business des document. | _Display label of the attached Business (if any)._ |
| Loan Label | Der loan number des zugehörigen loan des document. | _Display label of the attached Loan (if any)._ |
| Uploaded by User Label | Der full name des zugehörigen uploaded by user des document. | _Display label of the uploader (if known)._ |
| Attached to | Nach Priorität bestimmt: der Wert „Loan“, wenn der loan (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat; der Wert „Business“, wenn der business (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat; andernfalls der Wert „Orphan“. | _Which parent this document hangs off: 'Loan', 'Business', or 'Orphan' (neither set)._ |
| From Customer Portal | Wahr, wenn der uploaded via ist der Wert „BusinessClientPortal“. | _True when uploaded by the customer themselves via the BusinessClientPortal._ |
| **Interaction** | Unified activity-log feed for a Business. An Interaction is intentionally generic: InteractionType discriminates Note, Call, Visit, Task, Meeting, or SystemEvent. Covenant breaches, document requests, and other machine actions are written as SystemEvent interactions so they appear in the same stream as human-logged activity. | — |
| Name | Berechnet als der business, gefolgt von ein Bindestrich, gefolgt von der TEXT von der interaction date und der Wert „yyyy-mm-dd“, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene interaction type, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene subject, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Kebab-cased compound PK: {Business}-{InteractionDate}-{InteractionType slug}-{Subject slug}._ |
| Business Label | Der legal name des zugehörigen business des interaction. | _Display label of the parent Business._ |
| User Label | Der full name des zugehörigen user des interaction. | _Display label of the logging user (if any)._ |
| Is System Event | Wahr, wenn der interaction type ist der Wert „SystemEvent“. | _True for system-generated rows._ |
| Is Task | Wahr, wenn der interaction type ist der Wert „Task“. | _True for Task-type rows._ |
| From Customer | Wahr, wenn der source ist der Wert „BusinessClientPortal“. | _True when sourced from the BusinessClientPortal (customer-originated)._ |
| Is Covenant Event | Wahr, wenn alles Folgende gilt: das is system event-Kennzeichen gesetzt ist und der subject der Wert „covenant“ erwähnt. | _Higher-order: true when this is a covenant-related system event (subject contains 'covenant')._ |

## 2 Faktentypen

- ein **business** verweist auf genau einen **user**
- ein **business** kann auf einen **business** verweisen
- ein **beneficial owner** verweist auf genau einen **business**
- ein **contact** verweist auf genau einen **business**
- ein **account** verweist auf genau einen **business**
- ein **loan** verweist auf genau einen **business**
- ein **loan** verweist auf genau einen **user**
- ein **covenant** verweist auf genau einen **loan**
- ein **risk rating history** verweist auf genau einen **loan**
- ein **risk rating history** verweist auf genau einen **user**
- ein **document** kann auf einen **business** verweisen
- ein **document** kann auf einen **loan** verweisen
- ein **document** kann auf einen **user** verweisen
- eine **interaction** verweist auf genau einen **business**
- eine **interaction** kann auf einen **user** verweisen

## 3 Operative Regeln

_Operative Regeln drücken aus, was das Geschäft **verpflichtet**, **untersagt** oder
empfiehlt (**sollte**). Strukturelle Regeln stammen aus Pflichtfeldern und Fremdschlüsseln;
semantische Regeln stammen aus der Constraints-Tabelle und stützen sich jeweils auf einen booleschen
Wert, den das Regelbuch bereits berechnet (unten in den definitorischen Regeln als DR-N referenziert)._

### Strukturelle Einschränkungen (aus dem Schema)

- Ein user **muss** einen full name, einen role und einen email haben.
- Ein business **muss** auf genau einen user als seinen relationship manager verweisen.
- Ein business **muss** einen legal name, einen business structure, einen NAICS code und einen status haben.
- Ein beneficial owner **muss** auf genau einen business verweisen.
- Ein beneficial owner **muss** einen full name, einen date of birth, einen SSN, einen address und einen ownership percentage haben und erfassen, ob es ist ein control person.
- Ein contact **muss** auf genau einen business verweisen.
- Ein contact **muss** einen full name, einen title und einen contact type haben und erfassen, ob es ist ein authorized signer.
- Ein account **muss** auf genau einen business verweisen.
- Ein account **muss** einen account type, einen account number last4, einen current balance USD und einen opened at haben und erfassen, ob es hat einen ACH, ob es hat einen wire und ob es hat einen card.
- Ein loan **muss** auf genau einen business verweisen.
- Ein loan **muss** auf genau einen user als seinen originating RM verweisen.
- Ein loan **muss** auf genau einen user als seinen underwriter verweisen.
- Ein loan **muss** einen loan number, einen loan purpose, einen principal USD, einen rate pct, einen term months, einen underwriting stage, einen risk rating, einen risk rating label und einen originated at haben.
- Ein covenant **muss** auf genau einen loan verweisen.
- Ein covenant **muss** einen covenant type, einen test frequency, einen next test date und einen status haben.
- Ein risk rating history **muss** auf genau einen loan verweisen.
- Ein risk rating history **muss** auf genau einen user als seinen changed by user verweisen.
- Ein risk rating history **muss** einen effective date, einen new grade und einen reason haben.
- Ein document **muss** einen filename, einen document type und einen uploaded at haben und erfassen, ob es ist ocr indexed.
- Eine interaction **muss** auf genau einen business verweisen.
- Eine interaction **muss** eine interaction type, einen subject und eine interaction date haben.

### BSA/AML (CDD)

- **biz-cdd** *(verbindlich)* — Ein business **muss** einen beneficial owners at CDD threshold von mehr als 0 haben.
  - _Bei Verstoß:_ „Beneficial-ownership (CDD) collection is incomplete: no owner crosses the 25% threshold and no control person is designated.“
  - _Quelle:_ FinCEN CDD Rule, 31 CFR 1010.230
  - _Stützt sich auf:_ Meets CDD Rule (siehe DR-25).

### Credit Policy

- **loan-dscr** *(aufhebbar)* — Ein loan **muss** einen DSCR von mindestens 1.20 haben *(durch einen autorisierten Genehmiger aufhebbar)*.
  - _Bei Verstoß:_ „DSCR is below the 1.20 minimum; a credit-committee waiver is required to fund.“
  - _Quelle:_ Internal Credit Policy §2.1 (DSCR floor)
  - _Stützt sich auf:_ DSCR in Band (siehe DR-46).
- **loan-ltv** *(aufhebbar)* — Ein loan **muss** einen LTV von höchstens 0.80 haben *(durch einen autorisierten Genehmiger aufhebbar)*.
  - _Bei Verstoß:_ „LTV exceeds the 0.80 ceiling; a credit-committee waiver is required to fund.“
  - _Quelle:_ Internal Credit Policy §2.2 (LTV ceiling)
  - _Stützt sich auf:_ LTV in Band (siehe DR-47).

### Portfolio Monitoring

- **loan-watchlist** *(empfehlend)* — Ein loan **sollte** erfüllen, dass mindestens eines des Folgenden gilt: das is classified asset-Kennzeichen gesetzt ist oder das has breached covenant-Kennzeichen gesetzt ist.
  - _Bei Verstoß:_ „This loan is a classified asset or has a breached covenant but is not flagged on the watchlist.“
  - _Quelle:_ Internal Portfolio Risk Procedure §6 (watchlist)
  - _Stützt sich auf:_ On Watchlist (siehe DR-54).

### Segregation of Duties

- **loan-sod** *(verbindlich)* — A loan **must not** be funded by the same user who originated it: the originating relationship manager and the underwriter must be different users.
  - _Bei Verstoß:_ „The same user originated and underwrote this loan — segregation of duties is violated.“
  - _Quelle:_ Internal Credit Policy §4.2

## 4 Definitorische Regeln

_Alle Aussagen drücken eine Wahrheit der Geschäftsdomäne aus; sie sind weder
Prozeduren noch Befehle. "genau dann, wenn" wird zugunsten von "nur dann, wenn"
vermieden, damit eine einseitige Notwendigkeit nicht mit einer Äquivalenz verwechselt
wird. Ein **⚠︎ mechanisch**-Chip kennzeichnet eine Regel, deren deterministische
Formulierung getreu, aber holprig ist — ein Hinweis für eine optionale spätere
Umformulierung, kein Fehler._

| ID | Deklarative Regel |
|----|-------------------|
| **DR-1 Name** | Ein user: Name wird berechnet als das kleingeschriebene full name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-2 Is RM** | Ein user gilt als ein RM, wenn der role ist der Wert „RM“. |
| **DR-3 Is Underwriter** | Ein user gilt als ein underwriter, wenn der role ist der Wert „Underwriter“. |
| **DR-4 Is Branch Banker** | Ein user gilt als ein branch banker, wenn der role ist der Wert „BranchBanker“. |
| **DR-5 Is Admin** | Ein user gilt als ein admin, wenn der role ist der Wert „Admin“. |
| **DR-6 Count of Portfolio Businesses** | Ein user: Count of portfolio businesses ist die Anzahl der mit dem user verbundenen businesses. |
| **DR-7 Count of Originated Loans** | Ein user: Count of originated loans ist die Anzahl der mit dem user verbundenen loans. |
| **DR-8 Count of Underwritten Loans** | Ein user: Count of underwritten loans ist die Anzahl der mit dem user verbundenen loans. |
| **DR-9 Name** | Ein business: Name wird berechnet als das kleingeschriebene legal name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird, wobei jedes ein Punkt durch eine leere Zeichenkette ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-10 Relationship Manager Label** | Ein business: Relationship manager label ist der full name des zugehörigen relationship manager des business. |
| **DR-11 Is Customer** | Ein business gilt als ein customer, wenn der status ist der Wert „Customer“. |
| **DR-12 Is Prospect** | Ein business gilt als ein prospect, wenn der status ist der Wert „Prospect“. |
| **DR-13 Was Referred** | Ein business gilt als referred gewesen, wenn der referral source (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat. |
| **DR-14 Count of Beneficial Owners** | Ein business: Count of beneficial owners ist die Anzahl der mit dem business verbundenen beneficial owners. |
| **DR-15 Count of Contacts** | Ein business: Count of contacts ist die Anzahl der mit dem business verbundenen contacts. |
| **DR-16 Count of Accounts** | Ein business: Count of accounts ist die Anzahl der mit dem business verbundenen accounts. |
| **DR-17 Count of Loans** | Ein business: Count of loans ist die Anzahl der mit dem business verbundenen loans. |
| **DR-18 Count of Interactions** | Ein business: Count of interactions ist die Anzahl der mit dem business verbundenen interactions. |
| **DR-19 Count of Documents** | Ein business: Count of documents ist die Anzahl der mit dem business verbundenen documents. |
| **DR-20 Total Deposit Balance USD** | Ein business: Total deposit balance USD ist die Gesamtsumme von current balance USD über die mit dem business verbundenen accounts. |
| **DR-21 Total Loan Principal USD** | Ein business: Total loan principal USD ist die Gesamtsumme von principal USD über die mit dem business verbundenen loans. |
| **DR-22 Count of Classified Loans** | Ein business: Count of classified loans ist die Anzahl der loans des business, die classified asset sind. |
| **DR-23 Has Classified Loan** | Ein business gilt als über einen classified loan verfügend, wenn der count of classified loans größer ist als 0. |
| **DR-24 Beneficial Owners At CDD Threshold** | Ein business: Beneficial owners at CDD threshold ist die Anzahl der beneficial owners des business, die CDD threshold erfüllen. |
| **DR-25 Meets CDD Rule** | Ein business gilt als CDD rule erfüllend, wenn der beneficial owners at CDD threshold größer ist als 0. |
| **DR-26 Portfolio Priority** | Der portfolio priority des business wird nach folgender Priorität bestimmt:<br>1. der Wert „High“, wenn das has classified loan-Kennzeichen gesetzt ist;<br>2. der Wert „Medium“, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das meets CDD rule-Kennzeichen gesetzt ist; das is prospect-Kennzeichen gesetzt ist; oder der count of loans ist 0;<br>3. andernfalls der Wert „Low“. |
| **DR-27 Name** | Ein beneficial owner: Name wird berechnet als der business, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene full name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-28 Business Label** | Ein beneficial owner: Business label ist der legal name des zugehörigen business des beneficial owner. |
| **DR-29 Meets25 Percent Threshold** | Ein beneficial owner ist markiert als meets25 percent threshold, wenn der ownership percentage mindestens 25 ist. |
| **DR-30 Meets CDD Threshold** | Ein beneficial owner gilt als CDD threshold erfüllend, wenn mindestens eines des Folgenden gilt: das meets25 percent threshold-Kennzeichen gesetzt ist oder das is control person-Kennzeichen gesetzt ist. |
| **DR-31 Name** | Ein contact: Name wird berechnet als der business, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene full name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene title, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-32 Business Label** | Ein contact: Business label ist der legal name des zugehörigen business des contact. |
| **DR-33 Is Officer** | Ein contact gilt als ein officer, wenn der contact type ist der Wert „Officer“. |
| **DR-34 Is AP Clerk** | Ein contact gilt als ein AP clerk, wenn der contact type ist der Wert „APClerk“. |
| **DR-35 Name** | Ein account: Name wird berechnet als der business, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene account type, gefolgt von ein Bindestrich, gefolgt von der account number last4. |
| **DR-36 Business Label** | Ein account: Business label ist der legal name des zugehörigen business des account. |
| **DR-37 Treasury Service Count** | Ein account: Treasury service count wird berechnet als die Anzahl der folgenden zutreffenden Bedingungen: das has ACH-Kennzeichen gesetzt ist; das has wire-Kennzeichen gesetzt ist; und das has card-Kennzeichen gesetzt ist. |
| **DR-38 Has Any Treasury Service** | Ein account gilt als über einen any treasury service verfügend, wenn der treasury service count größer ist als 0. |
| **DR-39 Name** | Ein loan: Name wird berechnet als das kleingeschriebene loan number, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-40 Business Label** | Ein loan: Business label ist der legal name des zugehörigen business des loan. |
| **DR-41 Business NAICS Code** | Ein loan: Business NAICS code ist der NAICS code des zugehörigen business des loan. |
| **DR-42 Originating RM Label** | Ein loan: Originating RM label ist der full name des zugehörigen originating RM des loan. |
| **DR-43 Underwriter is Admin** | Ein loan: Underwriter is admin ist wahr, wenn der zugehörige underwriter des loan ist ein admin. |
| **DR-44 Is Funded** | Ein loan gilt als funded, wenn der underwriting stage ist der Wert „Funded“. |
| **DR-45 Is Classified Asset** | Ein loan gilt als ein classified asset, wenn der risk rating mindestens 7 ist. |
| **DR-46 DSCR in Band** | Ein loan ist markiert als DSCR in band, wenn der DSCR (ein fehlender Wert zählt als 0) mindestens 1.20 ist. |
| **DR-47 LTV in Band** | Ein loan ist markiert als LTV in band, wenn der LTV (ein fehlender Wert zählt als 0) höchstens 0.80 ist. |
| **DR-48 Segregation of Duties Ok** | Ein loan ist markiert als segregation of duties ok, wenn es nicht der Fall ist, dass der originating RM ist der underwriter. |
| **DR-49 Count of Covenants** | Ein loan: Count of covenants ist die Anzahl der mit dem loan verbundenen covenants. |
| **DR-50 Count of Breached Covenants** | Ein loan: Count of breached covenants ist die Anzahl der covenants des loan, die breached sind. |
| **DR-51 Count of Risk Rating History** | Ein loan: Count of risk rating history ist die Anzahl der mit dem loan verbundenen risk rating history. |
| **DR-52 Count of Documents** | Ein loan: Count of documents ist die Anzahl der mit dem loan verbundenen documents. |
| **DR-53 Has Breached Covenant** | Ein loan gilt als über einen breached covenant verfügend, wenn der count of breached covenants größer ist als 0. |
| **DR-54 On Watchlist** | Ein loan ist markiert als on watchlist, wenn mindestens eines des Folgenden gilt: das is classified asset-Kennzeichen gesetzt ist oder das has breached covenant-Kennzeichen gesetzt ist. |
| **DR-55 Health Score** | Ein loan: Health score wird berechnet als die Anzahl der folgenden zutreffenden Bedingungen: das DSCR in band-Kennzeichen gesetzt ist; das LTV in band-Kennzeichen gesetzt ist; das is classified asset-Kennzeichen nicht gesetzt ist; und das has breached covenant-Kennzeichen nicht gesetzt ist. |
| **DR-56 Name** | Ein covenant: Name wird berechnet als der loan, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene covenant type, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-57 Loan Label** | Ein covenant: Loan label ist der loan number des zugehörigen loan des covenant. |
| **DR-58 Loan Business** | Ein covenant: Loan business ist der business des zugehörigen loan des covenant. |
| **DR-59 Is Breached** | Ein covenant gilt als breached, wenn der status ist der Wert „Breached“. |
| **DR-60 Has Active Waiver** | Ein covenant gilt als über einen active waiver verfügend, wenn der current waiver through (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat. |
| **DR-61 Name** | Ein risk rating history: Name wird berechnet als der loan, gefolgt von ein Bindestrich, gefolgt von der TEXT von der effective date und der Wert „yyyy-mm-dd“, gefolgt von der Wert „-grade-“, gefolgt von der TEXT von der new grade und der Wert „0“. |
| **DR-62 Loan Label** | Ein risk rating history: Loan label ist der loan number des zugehörigen loan des risk rating history. |
| **DR-63 Changed by User Label** | Ein risk rating history: Changed by user label ist der full name des zugehörigen changed by user des risk rating history. |
| **DR-64 Grade Delta** | Ein risk rating history: Grade delta wird berechnet als der new grade minus der prior grade (ein fehlender Wert zählt als der new grade). |
| **DR-65 Is Downgrade** | Ein risk rating history gilt als ein downgrade, wenn der grade delta größer ist als 0. |
| **DR-66 Is Initial Rating** | Ein risk rating history gilt als initial rating, wenn der prior grade (ein fehlender Wert zählt als 0) ist 0. |
| **DR-67 Crossed Classified Threshold** | Ein risk rating history ist markiert als crossed classified threshold, wenn alles Folgende gilt: der prior grade (ein fehlender Wert zählt als 0) kleiner ist als 7 und der new grade mindestens 7 ist. |
| **DR-68 Name** | Ein document: Name wird berechnet als das kleingeschriebene filename, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird, wobei jedes ein Punkt durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-69 Business Label** | Ein document: Business label ist der legal name des zugehörigen business des document. |
| **DR-70 Loan Label** | Ein document: Loan label ist der loan number des zugehörigen loan des document. |
| **DR-71 Uploaded by User Label** | Ein document: Uploaded by user label ist der full name des zugehörigen uploaded by user des document. |
| **DR-72 Attached to** | Der attached to des document wird nach folgender Priorität bestimmt:<br>1. der Wert „Loan“, wenn der loan (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat;<br>2. der Wert „Business“, wenn der business (ein fehlender Wert zählt als eine leere Zeichenkette) einen Wert hat;<br>3. andernfalls der Wert „Orphan“. |
| **DR-73 From Customer Portal** | Ein document ist markiert als from customer portal, wenn der uploaded via ist der Wert „BusinessClientPortal“. |
| **DR-74 Name** | Eine interaction: Name wird berechnet als der business, gefolgt von ein Bindestrich, gefolgt von der TEXT von der interaction date und der Wert „yyyy-mm-dd“, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene interaction type, gefolgt von ein Bindestrich, gefolgt von das kleingeschriebene subject, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-75 Business Label** | Eine interaction: Business label ist der legal name des zugehörigen business des interaction. |
| **DR-76 User Label** | Eine interaction: User label ist der full name des zugehörigen user des interaction. |
| **DR-77 Is System Event** | Eine interaction gilt als ein system event, wenn der interaction type ist der Wert „SystemEvent“. |
| **DR-78 Is Task** | Eine interaction gilt als ein task, wenn der interaction type ist der Wert „Task“. |
| **DR-79 From Customer** | Eine interaction ist markiert als from customer, wenn der source ist der Wert „BusinessClientPortal“. |
| **DR-80 Is Covenant Event** | Eine interaction gilt als ein covenant event, wenn alles Folgende gilt: das is system event-Kennzeichen gesetzt ist und der subject der Wert „covenant“ erwähnt. |

## 5 Rückverfolgbarkeit zum Schema

_Die Ausdruck-Spalte ist die Definition der Regel in RuleSpeak-Notation —
dieselbe Logik, die das Regelbuch speichert, für einen Geschäftsleser geschrieben._

| Schema-Element | Art | Ausdruck |
|----------------|-----|----------|
| **Users.Name** | Formel | `Ersetzen(Lower(FullName), " ", "-")` |
| **Users.IsRM** | Formel | `Role = "RM"` |
| **Users.IsUnderwriter** | Formel | `Role = "Underwriter"` |
| **Users.IsBranchBanker** | Formel | `Role = "BranchBanker"` |
| **Users.IsAdmin** | Formel | `Role = "Admin"` |
| **Users.CountOfPortfolioBusinesses** | Aggregation | `Anzahl(Businesses über RelationshipManager)` |
| **Users.CountOfOriginatedLoans** | Aggregation | `Anzahl(Loans über OriginatingRm)` |
| **Users.CountOfUnderwrittenLoans** | Aggregation | `Anzahl(Loans über Underwriter)` |
| **Businesses.Name** | Formel | `Ersetzen(Ersetzen(Lower(LegalName), " ", "-"), ".", "")` |
| **Businesses.RelationshipManagerLabel** | Verweis | `Verweis(Users.FullName über RelationshipManager)` |
| **Businesses.IsCustomer** | Formel | `Status = "Customer"` |
| **Businesses.IsProspect** | Formel | `Status = "Prospect"` |
| **Businesses.WasReferred** | Formel | `Not(Coalesce(ReferralSource, "") = "")` |
| **Businesses.CountOfBeneficialOwners** | Aggregation | `Anzahl(BeneficialOwners über Business)` |
| **Businesses.CountOfContacts** | Aggregation | `Anzahl(Contacts über Business)` |
| **Businesses.CountOfAccounts** | Aggregation | `Anzahl(Accounts über Business)` |
| **Businesses.CountOfLoans** | Aggregation | `Anzahl(Loans über Business)` |
| **Businesses.CountOfInteractions** | Aggregation | `Anzahl(Interactions über Business)` |
| **Businesses.CountOfDocuments** | Aggregation | `Anzahl(Documents über Business)` |
| **Businesses.TotalDepositBalanceUsd** | Aggregation | `Summe(Accounts.CurrentBalanceUsd über Business)` |
| **Businesses.TotalLoanPrincipalUsd** | Aggregation | `Summe(Loans.PrincipalUsd über Business)` |
| **Businesses.CountOfClassifiedLoans** | Aggregation | `Anzahl(Loans über Business)` |
| **Businesses.HasClassifiedLoan** | Formel | `CountOfClassifiedLoans > 0` |
| **Businesses.BeneficialOwnersAtCddThreshold** | Aggregation | `Anzahl(BeneficialOwners über Business)` |
| **Businesses.MeetsCddRule** | Formel | `BeneficialOwnersAtCddThreshold > 0` |
| **Businesses.PortfolioPriority** | Formel | `If(HasClassifiedLoan, "High", If(Or(Not(MeetsCddRule), IsProspect, CountOfLoans = 0), "Medium", "Low"))` |
| **BeneficialOwners.Name** | Formel | `Business & "-" & Ersetzen(Lower(FullName), " ", "-")` |
| **BeneficialOwners.BusinessLabel** | Verweis | `Verweis(Businesses.LegalName über Business)` |
| **BeneficialOwners.Meets25PercentThreshold** | Formel | `OwnershipPercentage >= 25` |
| **BeneficialOwners.MeetsCddThreshold** | Formel | `Or(Meets25PercentThreshold, IsControlPerson)` |
| **Contacts.Name** | Formel | `Business & "-" & Ersetzen(Lower(FullName), " ", "-") & "-" & Ersetzen(Lower(Title), " ", "-")` |
| **Contacts.BusinessLabel** | Verweis | `Verweis(Businesses.LegalName über Business)` |
| **Contacts.IsOfficer** | Formel | `ContactType = "Officer"` |
| **Contacts.IsApClerk** | Formel | `ContactType = "APClerk"` |
| **Accounts.Name** | Formel | `Business & "-" & Lower(AccountType) & "-" & AccountNumberLast4` |
| **Accounts.BusinessLabel** | Verweis | `Verweis(Businesses.LegalName über Business)` |
| **Accounts.TreasuryServiceCount** | Formel | `If(HasAch, 1, 0) + If(HasWire, 1, 0) + If(HasCard, 1, 0)` |
| **Accounts.HasAnyTreasuryService** | Formel | `TreasuryServiceCount > 0` |
| **Loans.Name** | Formel | `Ersetzen(Lower(LoanNumber), " ", "-")` |
| **Loans.BusinessLabel** | Verweis | `Verweis(Businesses.LegalName über Business)` |
| **Loans.BusinessNaicsCode** | Verweis | `Verweis(Businesses.NaicsCode über Business)` |
| **Loans.OriginatingRmLabel** | Verweis | `Verweis(Users.FullName über OriginatingRm)` |
| **Loans.UnderwriterIsAdmin** | Verweis | `Verweis(Users.IsAdmin über Underwriter)` |
| **Loans.IsFunded** | Formel | `UnderwritingStage = "Funded"` |
| **Loans.IsClassifiedAsset** | Formel | `RiskRating >= 7` |
| **Loans.DscrInBand** | Formel | `Coalesce(Dscr, 0) >= 1.20` |
| **Loans.LtvInBand** | Formel | `Coalesce(Ltv, 0) <= 0.80` |
| **Loans.SegregationOfDutiesOk** | Formel | `Not(OriginatingRm = Underwriter)` |
| **Loans.CountOfCovenants** | Aggregation | `Anzahl(Covenants über Loan)` |
| **Loans.CountOfBreachedCovenants** | Aggregation | `Anzahl(Covenants über Loan)` |
| **Loans.CountOfRiskRatingHistory** | Aggregation | `Anzahl(RiskRatingHistory über Loan)` |
| **Loans.CountOfDocuments** | Aggregation | `Anzahl(Documents über Loan)` |
| **Loans.HasBreachedCovenant** | Formel | `CountOfBreachedCovenants > 0` |
| **Loans.OnWatchlist** | Formel | `Or(IsClassifiedAsset, HasBreachedCovenant)` |
| **Loans.HealthScore** | Formel | `If(DscrInBand, 1, 0) + If(LtvInBand, 1, 0) + If(IsClassifiedAsset, 0, 1) + If(HasBreachedCovenant, 0, 1)` |
| **Covenants.Name** | Formel | `Loan & "-" & Ersetzen(Lower(CovenantType), " ", "-")` |
| **Covenants.LoanLabel** | Verweis | `Verweis(Loans.LoanNumber über Loan)` |
| **Covenants.LoanBusiness** | Verweis | `Verweis(Loans.Business über Loan)` |
| **Covenants.IsBreached** | Formel | `Status = "Breached"` |
| **Covenants.HasActiveWaiver** | Formel | `Not(Coalesce(CurrentWaiverThrough, "") = "")` |
| **RiskRatingHistory.Name** | Formel | `Loan & "-" & Text(EffectiveDate, "yyyy-mm-dd") & "-grade-" & Text(NewGrade, "0")` |
| **RiskRatingHistory.LoanLabel** | Verweis | `Verweis(Loans.LoanNumber über Loan)` |
| **RiskRatingHistory.ChangedByUserLabel** | Verweis | `Verweis(Users.FullName über ChangedByUser)` |
| **RiskRatingHistory.GradeDelta** | Formel | `NewGrade - Coalesce(PriorGrade, NewGrade)` |
| **RiskRatingHistory.IsDowngrade** | Formel | `GradeDelta > 0` |
| **RiskRatingHistory.IsInitialRating** | Formel | `Coalesce(PriorGrade, 0) = 0` |
| **RiskRatingHistory.CrossedClassifiedThreshold** | Formel | `And(Coalesce(PriorGrade, 0) < 7, NewGrade >= 7)` |
| **Documents.Name** | Formel | `Ersetzen(Ersetzen(Lower(Filename), " ", "-"), ".", "-")` |
| **Documents.BusinessLabel** | Verweis | `Verweis(Businesses.LegalName über Business)` |
| **Documents.LoanLabel** | Verweis | `Verweis(Loans.LoanNumber über Loan)` |
| **Documents.UploadedByUserLabel** | Verweis | `Verweis(Users.FullName über UploadedByUser)` |
| **Documents.AttachedTo** | Formel | `If(Not(Coalesce(Loan, "") = ""), "Loan", If(Not(Coalesce(Business, "") = ""), "Business", "Orphan"))` |
| **Documents.FromCustomerPortal** | Formel | `UploadedVia = "BusinessClientPortal"` |
| **Interactions.Name** | Formel | `Business & "-" & Text(InteractionDate, "yyyy-mm-dd") & "-" & Lower(InteractionType) & "-" & Ersetzen(Lower(Subject), " ", "-")` |
| **Interactions.BusinessLabel** | Verweis | `Verweis(Businesses.LegalName über Business)` |
| **Interactions.UserLabel** | Verweis | `Verweis(Users.FullName über User)` |
| **Interactions.IsSystemEvent** | Formel | `InteractionType = "SystemEvent"` |
| **Interactions.IsTask** | Formel | `InteractionType = "Task"` |
| **Interactions.FromCustomer** | Formel | `Source = "BusinessClientPortal"` |
| **Interactions.IsCovenantEvent** | Formel | `And(IsSystemEvent, Not(Iferror(Find("covenant", Lower(Subject)), 0) = 0))` |

---

_Dieses Dokument ist in **RuleSpeak®** gerendert, der deklarativen Geschäftsregel-
Notation von **Ronald G. Ross**, und folgt den Konventionen von
**SBVR** (Semantics of Business Vocabulary and Business Rules). Mit Dank an
Ronald G. Ross für RuleSpeak und seine grundlegende Arbeit zu Geschäftsregeln —
[www.RonRoss.info](https://www.RonRoss.info)._
