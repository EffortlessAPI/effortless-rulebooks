# 📘 traffic-ticket-contest — RuleSpeak

_A production-grade ERB platform (admin/state-machine/jurisdiction/routing/feature-catalog machinery, extracted from the industrial-ui-services portal) with traffic-ticket contest as the example domain. The platform tables carry the full production conventions; tickets are one domain riding on top._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Business Rule** | A business rule tracked by the business. |
| **Business Rule Category** | A business rule category tracked by the business. |
| Rule Count | Count of BusinessRules in this category. |
| **Glossary Category** | A glossary category tracked by the business. |
| Term Count | Count of GlossaryTerms in this category. |
| **Glossary Term** | A glossary term tracked by the business. |
| **Role** | A role tracked by the business. |
| App User Count | Count of AppUsers with this role. |
| **Audit Log Entry** | An audit log entry tracked by the business. |
| Is Override Action | Calculated flag — TRUE when this audit log entry represents a manager override action (vs a routine action). |
| Entry Age Hours | Calculated — hours elapsed since EntryTimestamp. Used to age audit log entries. |
| **Platform Naviation** | A platform naviation tracked by the business. |
| Package is Active | Lookup: ERBPackage.IsActive — is this route's owning package enabled? UI hides the route when false. |
| Package is Licensed | Lookup: ERBPackage.IsLicensed — is this route's owning package licensed? UI hides the route from non-Effortless users when false. |
| Admin Can Create | Derived: AdminCRUD contains 'C' (BLANK when AdminCRUD is NULL). |
| Admin Can Read | Derived: AdminCRUD contains 'R' (BLANK when AdminCRUD is NULL). |
| Admin Can Update | Derived: AdminCRUD contains 'U' (BLANK when AdminCRUD is NULL). |
| Admin Can Delete | Derived: AdminCRUD contains 'D' (BLANK when AdminCRUD is NULL). |
| Manager Can Create | Derived: ManagerCRUD contains 'C' (BLANK when ManagerCRUD is NULL). |
| Manager Can Read | Derived: ManagerCRUD contains 'R' (BLANK when ManagerCRUD is NULL). |
| Manager Can Update | Derived: ManagerCRUD contains 'U' (BLANK when ManagerCRUD is NULL). |
| Manager Can Delete | Derived: ManagerCRUD contains 'D' (BLANK when ManagerCRUD is NULL). |
| Representative Can Create | Derived: RepresentativeCRUD contains 'C' (BLANK when RepresentativeCRUD is NULL). |
| Representative Can Read | Derived: RepresentativeCRUD contains 'R' (BLANK when RepresentativeCRUD is NULL). |
| Representative Can Update | Derived: RepresentativeCRUD contains 'U' (BLANK when RepresentativeCRUD is NULL). |
| Representative Can Delete | Derived: RepresentativeCRUD contains 'D' (BLANK when RepresentativeCRUD is NULL). |
| External Llm Can Create | Derived: External LLMCRUD contains 'C' (BLANK when External LLMCRUD is NULL). |
| External Llm Can Read | Derived: External LLMCRUD contains 'R' (BLANK when External LLMCRUD is NULL). |
| External Llm Can Update | Derived: External LLMCRUD contains 'U' (BLANK when External LLMCRUD is NULL). |
| External Llm Can Delete | Derived: External LLMCRUD contains 'D' (BLANK when External LLMCRUD is NULL). |
| Depth | Calculated — nesting depth in the menu tree. 0 = top-level (no parent); otherwise the number of dot-segments in the dotted RouteKey above the root (e.g. 'library.rules.detail' = 2). Drives N-deep indentation. |
| Full Path | Calculated — canonical role-agnostic URL path (equals Route). The SPA renders this under each permitted role as /:role + FullPath and resolves it only when that role's *CanRead is true; the role is an implicit prefix, never stored on the row. |
| Handler Base Name | Calculated — space-delimited form of the dotted RouteKey (dots and hyphens become spaces), e.g. 'library rules detail'. The client PascalCases this and prefixes the viewer role to derive the handler component deterministically: {Role} + PascalCase(HandlerBaseName) -> e.g. AdminLibraryRulesDetail, RepresentativeLibraryRulesDetail. No per-role handler is stored; edge cases get their own single-role route row instead. |
| **Jurisdiction** | A jurisdiction tracked by the business. |
| Parent Jurisdiction Name | The name of the jurisdiction's parent jurisdiction. |
| Is Root Jurisdiction | True when `If(ParentJurisdiction` is `Blank(), True(), False())`. |
| Child Jurisdiction Count | The number of jurisdictions related to the jurisdiction. |
| Relative Path | Concrete relative URL for this jurisdiction's explorer/detail page. Self-contained (no route-table lookup) so it is always populated. Anywhere a jurisdiction is referenced, link to this path. |
| Rule Count | How many JurisdictionRules belong to this jurisdiction (drives the explorer badges). |
| Source Document Count | The number of jurisdiction source documents related to the jurisdiction. |
| **ERB Version** | An ERB version tracked by the business. |
| **ERB Customization** | An ERB customization tracked by the business. |
| **Jurisdiction Source Document** | A jurisdiction source document tracked by the business. |
| Jurisdiction Name | The name of the jurisdiction source document's jurisdiction. |
| Relative Path | Concrete relative URL for this document's detail page in the portal. |
| Rule Count | The number of jurisdiction rules related to the jurisdiction source document. |
| **Jurisdiction Rule** | A jurisdiction rule tracked by the business. |
| Route Path | The route template (with :params) pulled from the linked PlatformNaviation route. |
| Relative Path | Concrete relative URL for this rule's detail page. Self-contained (dedicated /library/jurisdiction-rules/ space) so it never collides with the business-rule library at /library/rules. Anywhere a jurisdiction rule is referenced, link to this path. |
| Jurisdiction Name | The name of the jurisdiction rule's jurisdiction. |
| Jurisdiction Type | The jurisdiction type of the jurisdiction rule's jurisdiction. |
| Is Federal | True when `If(JurisdictionType` is `"Country", True(), False())`. |
| **App User** | An app user tracked by the business. |
| Name Redacted | Redacted (masked) view of Name. Redacted roles read this instead of Name. |
| Email Address Redacted | Redacted (masked) view of EmailAddress. Redacted roles read this instead of EmailAddress. |
| Role Title | Lookup: Role.Title. |
| Role Description | Lookup: Role.Description. |
| **Magic Link Config** | A magic link config tracked by the business. |
| **Site Branding** | A site branding tracked by the business. |
| **Reference Document** | A reference document tracked by the business. |
| Is Appeal Board Decision | TRUE when Library = appeal-board-decisions. |
| **State Machine** | A state machine tracked by the business. |
| Package is Active | Lookup: ERBPackage.IsActive — is this machine's owning package enabled? UI hides the machine when false. |
| State Count | Count of MachineStates in this machine. |
| Transition Rule Count | Count of StateTransitionRules in this machine. |
| **Machine State** | A machine state tracked by the business. |
| **State Transition Rule** | A state transition rule tracked by the business. |
| From State Key | Lookup: FromState.StateKey. |
| To State Key | Lookup: ToState.StateKey. |
| Is Forward Edge | TRUE when ToState is not the machine's initial state. |
| **State Transition** | A state transition tracked by the business. |
| Is Forward | Generalizes the old =NOT(ToStage="DRAFT"): TRUE when ToStateKey is not the machine's initial state. |
| **Work Queue Item** | A work queue item tracked by the business. |
| Due in Days | Calculated — days until DueDate (negative when overdue). |
| Is Overdue | TRUE when DueInDays < 0. |
| Urgency Bucket | URGENT (overdue or due today) / DUE_3_DAYS / UPCOMING / FOLLOW_UP (no due date). |
| Is Urgent | TRUE when UrgencyBucket = URGENT. |
| **Ai Model** | An ai model tracked by the business. |
| Pricing Version Count | Count of pricing versions on this model. |
| Turn Count | Count of assistant turns served by this model. |
| Total Cost | Rollup — total USD spend across all turns served by this model. |
| **Model Pricing Version** | A model pricing version tracked by the business. |
| Ai Model Title | Lookup: AiModel.Title. |
| Turn Count | Count of assistant turns priced with this version. |
| **Assistant Turn** | An assistant turn tracked by the business. |
| Total Tokens | Input + output tokens. |
| Billable Input Tokens | Non-cached input tokens = InputTokens - CachedInputTokens. |
| Ai Model Title | Lookup: AiModel.Title. |
| Input Price Per M Tok | Lookup: ModelPricingVersion.InputPricePerMTok. |
| Cached Input Price Per M Tok | Lookup: ModelPricingVersion.CachedInputPricePerMTok. |
| Output Price Per M Tok | Lookup: ModelPricingVersion.OutputPricePerMTok. |
| Input Cost | USD cost of input: (BillableInputTokens × InputPrice + CachedInputTokens × CachedInputPrice) / 1,000,000. |
| Output Cost | USD cost of output: OutputTokens × OutputPrice / 1,000,000. |
| Total Cost | Total USD cost for this turn = InputCost + OutputCost. Rolls up to Client and Claim. |
| **Platform** | A platform tracked by the business. |
| **ERB Package** | An ERB package tracked by the business. |
| Feature Count | Count of child ERBFeatures rows. |
| Shipped Feature Count | Count of child features with Status = SHIPPED. |
| **ERB Feature Status** | An ERB feature status tracked by the business. |
| Feature Count | Count of ERBFeatures in this status. |
| **ERB Feature Category** | An ERB feature category tracked by the business. |
| Feature Count | Count of ERBFeatures in this category. |
| **ERB Feature** | An ERB feature tracked by the business. |
| Route Path | The route template (with :params) pulled from the linked PlatformNaviation route. |
| Relative Path | Concrete relative URL for this row — the route template with its :param(s) substituted by this row's own id(s). |
| Package is Licensed | Lookup: ERBPackage.IsLicensed — is this feature's owning package licensed? UI hides the feature from non-Effortless users when false. |
| Category Title | Lookup: Category.Title — human-readable category name. |
| Status Title | Lookup: Status.Title. |
| Status Description | Lookup: Status.Description. |
| Status is Active | Lookup: Status.IsActive — whether this feature is part of the official shared view. |
| **ERB Table** | An ERB table tracked by the business. |
| Package is Active | Lookup: ERBPackage.IsActive — is this table's owning package enabled? |
| Package is Licensed | Lookup: ERBPackage.IsLicensed — is this table's owning package licensed? UI hides the table from non-Effortless users when false. |
| Admin Can Create | Derived: AdminCRUD contains 'C' (BLANK when AdminCRUD is NULL = inherits role default). |
| Admin Can Read | Derived: AdminCRUD contains 'R' (BLANK when AdminCRUD is NULL = inherits role default). |
| Admin Can Update | Derived: AdminCRUD contains 'U' (BLANK when AdminCRUD is NULL = inherits role default). |
| Admin Can Delete | Derived: AdminCRUD contains 'D' (BLANK when AdminCRUD is NULL = inherits role default). |
| Manager Can Create | Derived: ManagerCRUD contains 'C' (BLANK when ManagerCRUD is NULL = inherits role default). |
| Manager Can Read | Derived: ManagerCRUD contains 'R' (BLANK when ManagerCRUD is NULL = inherits role default). |
| Manager Can Update | Derived: ManagerCRUD contains 'U' (BLANK when ManagerCRUD is NULL = inherits role default). |
| Manager Can Delete | Derived: ManagerCRUD contains 'D' (BLANK when ManagerCRUD is NULL = inherits role default). |
| Representative Can Create | Derived: RepresentativeCRUD contains 'C' (BLANK when RepresentativeCRUD is NULL = inherits role default). |
| Representative Can Read | Derived: RepresentativeCRUD contains 'R' (BLANK when RepresentativeCRUD is NULL = inherits role default). |
| Representative Can Update | Derived: RepresentativeCRUD contains 'U' (BLANK when RepresentativeCRUD is NULL = inherits role default). |
| Representative Can Delete | Derived: RepresentativeCRUD contains 'D' (BLANK when RepresentativeCRUD is NULL = inherits role default). |
| External Llm Can Create | Derived: External LLMCRUD contains 'C' (BLANK when External LLMCRUD is NULL = inherits role default). |
| External Llm Can Read | Derived: External LLMCRUD contains 'R' (BLANK when External LLMCRUD is NULL = inherits role default). |
| External Llm Can Update | Derived: External LLMCRUD contains 'U' (BLANK when External LLMCRUD is NULL = inherits role default). |
| External Llm Can Delete | Derived: External LLMCRUD contains 'D' (BLANK when External LLMCRUD is NULL = inherits role default). |
| **ERB Field** | An ERB field tracked by the business. |
| Table Package is Active | Lookup: ERBTable.PackageIsActive — is the package owning this field's table enabled? (field inherits its table's package). |
| Is Calculated | TRUE for calculated/lookup/aggregation (read-only at the data layer). |
| Admin Can Create | Derived: AdminCRUD contains 'C' (BLANK when NULL = inherits table/role). |
| Admin Can Read | Derived: AdminCRUD contains 'R' (BLANK when NULL = inherits table/role). |
| Admin Can Update | Derived: AdminCRUD contains 'U' (BLANK when NULL = inherits table/role). |
| Admin Can Delete | Derived: AdminCRUD contains 'D' (BLANK when NULL = inherits table/role). |
| Manager Can Create | Derived: ManagerCRUD contains 'C' (BLANK when NULL = inherits table/role). |
| Manager Can Read | Derived: ManagerCRUD contains 'R' (BLANK when NULL = inherits table/role). |
| Manager Can Update | Derived: ManagerCRUD contains 'U' (BLANK when NULL = inherits table/role). |
| Manager Can Delete | Derived: ManagerCRUD contains 'D' (BLANK when NULL = inherits table/role). |
| Representative Can Create | Derived: RepresentativeCRUD contains 'C' (BLANK when NULL = inherits table/role). |
| Representative Can Read | Derived: RepresentativeCRUD contains 'R' (BLANK when NULL = inherits table/role). |
| Representative Can Update | Derived: RepresentativeCRUD contains 'U' (BLANK when NULL = inherits table/role). |
| Representative Can Delete | Derived: RepresentativeCRUD contains 'D' (BLANK when NULL = inherits table/role). |
| External Llm Can Create | Derived: External LLMCRUD contains 'C' (BLANK when NULL = inherits table/role). |
| External Llm Can Read | Derived: External LLMCRUD contains 'R' (BLANK when NULL = inherits table/role). |
| External Llm Can Update | Derived: External LLMCRUD contains 'U' (BLANK when NULL = inherits table/role). |
| External Llm Can Delete | Derived: External LLMCRUD contains 'D' (BLANK when NULL = inherits table/role). |
| **API Endpoint** | An API endpoint tracked by the business. |
| **Subject State Instance** | A subject state instance tracked by the business. |
| Is Current | TRUE when ExitedAt IS NULL — this is the subject's active state. |
| Has Complete Lineage | TRUE when the PriorInstance chain walks back to SequenceIndex=1 (the initial state occupancy). Validates lineage completeness. |
| **Violation Type** | A violation type tracked by the business. |
| Jurisdiction Label | Display name of the governing jurisdiction. |
| Is School Eligible by Cap | Whether this violation's points fall at or below the jurisdiction's traffic-school point cap (jurisdiction rule applied to the violation). |
| Count of Citations | Number of citations issued for this violation type. |
| **Driver** | A driver tracked by the business. |
| Full Name | Display name, last-comma-first. |
| Home Jurisdiction Label | Display name of the driver's home jurisdiction. |
| Suspension Threshold | Point-suspension threshold pulled from the driver's home jurisdiction. |
| Warning Threshold | Point-warning threshold pulled from the driver's home jurisdiction. |
| Count of Citations | Number of citations issued to this driver. |
| Active Points | Sum of license points across this driver's citations that resulted in a conviction (guilty/unpaid-default) and are still point-active. |
| License Status | License-points state machine for the driver: Suspended at/above the suspension threshold, Warning at/above the warning threshold, otherwise Valid. |
| **Citation** | A citation tracked by the business. |
| Driver Label | Display name of the cited driver. |
| Violation Label | Description of the cited violation. |
| Jurisdiction Label | Display name of the issuing jurisdiction. |
| Base Fine USD | Base fine for the cited violation, pulled from the ViolationType. |
| Violation Points | License points for the cited violation, pulled from the ViolationType. |
| Days to Respond | Response window in days, pulled from the issuing jurisdiction. |
| Days to Pay After Ruling | Days-to-pay window, pulled from the issuing jurisdiction. |
| Late Penalty Pct | Late-penalty percentage, pulled from the issuing jurisdiction. |
| Days Late to Collections | Days-late-to-collections window, pulled from the issuing jurisdiction. |
| Response Due Date | Deadline to respond: IssuedOn + the jurisdiction's response window. |
| Days Until Response Due | Days remaining (negative if overdue) until the response deadline, measured from AsOfDate. |
| Is Response Overdue | True when no response was filed and the response deadline has passed as of AsOfDate. |
| Count of Hearings | Number of hearing records attached to this citation. |
| Latest Hearing Outcome | Outcome of the most recent hearing for this citation (Pending if a hearing exists but has no outcome; blank if no hearing). |
| Contest Status | Contest/Hearing state machine: NotContested when the driver did not elect to contest; otherwise HearingRequested -> Scheduled -> Heard, reflected from the latest hearing's outcome. |
| Is Dismissed | True when the latest hearing outcome dismissed the citation. |
| Is Guilty | True when the driver is liable: either found guilty/upheld at hearing, or defaulted by missing the response deadline without contesting. |
| Amount Due USD | Amount currently owed: 0 if dismissed; otherwise the base fine plus the jurisdiction's late penalty if the payment is late. |
| Payment Due Date | Date the fine is due: the later anchor (response deadline) plus the jurisdiction's days-to-pay window. |
| Is Payment Late | True when the driver is liable, has not paid in full, and the payment due date has passed as of AsOfDate. |
| Is in Collections | True when a late payment has remained unpaid past the jurisdiction's collections window. |
| Payment Status | Payment/Penalty state machine: NotOwed (dismissed) -> Paid -> Collections -> Late -> Due. Evaluated in priority order. |
| Effective Points | License points this citation actually contributes to the driver: the violation's points if the driver is liable and not dismissed, otherwise 0. Drives the driver's ActivePoints rollup. |
| Citation Status | Citation lifecycle state machine: Issued -> Responded -> InContest -> Adjudicated -> Closed. The top-level status synthesizing the other tracks. |
| **Hearing** | A hearing tracked by the business. |
| Citation Label | Citation number this hearing concerns. |
| **Payment** | A payment tracked by the business. |
| Citation Label | Citation number this payment concerns. |
| **Case Event** | A case event tracked by the business. |
| Citation Label | Citation number this event concerns. |

## 2 Fact Types

- a **business rule** may reference one **business rule category**
- a **glossary term** may reference one **glossary category**
- an **audit log entry** may reference one **citation**
- an **audit log entry** may reference one **app user**
- a **platform naviation** may reference one **ERB package**
- a **jurisdiction** may reference one **jurisdiction**
- a **jurisdiction** may reference one **jurisdiction source document**
- a **jurisdiction source document** may reference one **jurisdiction**
- a **jurisdiction rule** may reference one **platform naviation**
- a **jurisdiction rule** may reference one **jurisdiction source document**
- a **jurisdiction rule** may reference one **jurisdiction**
- an **app user** may reference one **role**
- a **reference document** may reference one **jurisdiction**
- a **state machine** may reference one **ERB package**
- a **machine state** references exactly one **state machine**
- a **machine state** may reference one **from transition rule**
- a **machine state** may reference one **to transition rule**
- a **state transition rule** references exactly one **state machine**
- a **state transition rule** references exactly one **from state**
- a **state transition rule** references exactly one **to state**
- a **state transition** references exactly one **state machine**
- a **state transition** may reference one **app user**
- a **work queue item** may reference one **app user**
- a **model pricing version** references exactly one **ai model**
- an **assistant turn** may reference one **citation**
- an **assistant turn** references exactly one **ai model**
- an **assistant turn** references exactly one **model pricing version**
- an **ERB feature** may reference one **platform naviation**
- an **ERB feature** references exactly one **ERB package**
- an **ERB feature** may reference one **ERB feature category**
- an **ERB feature** may reference one **ERB feature status**
- an **ERB table** references exactly one **ERB package**
- an **ERB table** references exactly one **platform**
- an **ERB field** references exactly one **ERB table**
- a **subject state instance** references exactly one **state machine**
- a **subject state instance** may reference one **subject state instance**
- a **subject state instance** may reference one **state transition**
- a **violation type** references exactly one **jurisdiction**
- a **driver** references exactly one **jurisdiction**
- a **citation** references exactly one **driver**
- a **citation** references exactly one **violation type**
- a **citation** references exactly one **jurisdiction**
- a **hearing** references exactly one **citation**
- a **payment** references exactly one **citation**
- a **case event** references exactly one **citation**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Rule Count** | A business rule category's rule count is rolled up from its related records (``). |
| **DR-2 Term Count** | A glossary category's term count is rolled up from its related records (``). |
| **DR-3 App User Count** | A role's app user count is rolled up from its related records (``). |
| **DR-4 Is Override Action** | An audit log entry is considered an override action if `If(Lower(ActionType & "")` is `"override", True(), False())`. |
| **DR-5 Entry Age Hours** | An audit log entry's entry age hours is computed as `Datetime_diff(Now(), Timestamp, 'hours')`. |
| **DR-6 Package is Active** | A platform naviation's package is active is true when the platform naviation's ERB package is active. |
| **DR-7 Package is Licensed** | A platform naviation's package is licensed is true when the platform naviation's ERB package is licensed. |
| **DR-8 Admin Can Create** | A platform naviation is flagged admin can create if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",AdminCRUD))))`. |
| **DR-9 Admin Can Read** | A platform naviation is flagged admin can read if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",AdminCRUD))))`. |
| **DR-10 Admin Can Update** | A platform naviation is flagged admin can update if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",AdminCRUD))))`. |
| **DR-11 Admin Can Delete** | A platform naviation is flagged admin can delete if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",AdminCRUD))))`. |
| **DR-12 Manager Can Create** | A platform naviation is flagged manager can create if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",ManagerCRUD))))`. |
| **DR-13 Manager Can Read** | A platform naviation is flagged manager can read if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",ManagerCRUD))))`. |
| **DR-14 Manager Can Update** | A platform naviation is flagged manager can update if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",ManagerCRUD))))`. |
| **DR-15 Manager Can Delete** | A platform naviation is flagged manager can delete if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",ManagerCRUD))))`. |
| **DR-16 Representative Can Create** | A platform naviation is flagged representative can create if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",RepresentativeCRUD))))`. |
| **DR-17 Representative Can Read** | A platform naviation is flagged representative can read if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",RepresentativeCRUD))))`. |
| **DR-18 Representative Can Update** | A platform naviation is flagged representative can update if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",RepresentativeCRUD))))`. |
| **DR-19 Representative Can Delete** | A platform naviation is flagged representative can delete if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",RepresentativeCRUD))))`. |
| **DR-20 External Llm Can Create** | A platform naviation is flagged external llm can create if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",ExternalLlmCRUD))))`. |
| **DR-21 External Llm Can Read** | A platform naviation is flagged external llm can read if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",ExternalLlmCRUD))))`. |
| **DR-22 External Llm Can Update** | A platform naviation is flagged external llm can update if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",ExternalLlmCRUD))))`. |
| **DR-23 External Llm Can Delete** | A platform naviation is flagged external llm can delete if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",ExternalLlmCRUD))))`. |
| **DR-24 Depth** | The platform naviation's depth is determined by the following priority:<br>1. 0, if the parent route key is `Blank()`;<br>2. otherwise `Len(RouteKey) - Len(Replace(RouteKey, ".", ""))`. |
| **DR-25 Full Path** | A platform naviation's full path is computed as the route. |
| **DR-26 Handler Base Name** | A platform naviation's handler base name is computed as the route key with every a period replaced by a space with every a hyphen replaced by a space. |
| **DR-27 Parent Jurisdiction Name** | A jurisdiction's parent jurisdiction name is the name of the jurisdiction's parent jurisdiction. |
| **DR-28 Is Root Jurisdiction** | A jurisdiction is considered a root jurisdiction if `If(ParentJurisdiction` is `Blank(), True(), False())`. |
| **DR-29 Child Jurisdiction Count** | A jurisdiction's child jurisdiction count is the number of jurisdictions related to the jurisdiction. |
| **DR-30 Relative Path** | A jurisdiction's relative path is computed as the literal “/library/jurisdictions/”, followed by the jurisdiction ID. |
| **DR-31 Rule Count** | A jurisdiction's rule count is the number of jurisdiction rules related to the jurisdiction. |
| **DR-32 Source Document Count** | A jurisdiction's source document count is the number of jurisdiction source documents related to the jurisdiction. |
| **DR-33 Jurisdiction Name** | A jurisdiction source document's jurisdiction name is the name of the jurisdiction source document's jurisdiction. |
| **DR-34 Relative Path** | A jurisdiction source document's relative path is computed as the literal “/library/jurisdiction-docs/”, followed by the jurisdiction source document ID. |
| **DR-35 Rule Count** | A jurisdiction source document's rule count is the number of jurisdiction rules related to the jurisdiction source document. |
| **DR-36 Route Path** | A jurisdiction rule's route path is the route of the jurisdiction rule's route. |
| **DR-37 Relative Path** | A jurisdiction rule's relative path is computed as the literal “/library/jurisdiction-rules/”, followed by the jurisdiction rule ID. |
| **DR-38 Jurisdiction Name** | A jurisdiction rule's jurisdiction name is the name of the jurisdiction rule's jurisdiction. |
| **DR-39 Jurisdiction Type** | A jurisdiction rule's jurisdiction type is the jurisdiction type of the jurisdiction rule's jurisdiction. |
| **DR-40 Is Federal** | A jurisdiction rule is considered a federal if `If(JurisdictionType` is `"Country", True(), False())`. |
| **DR-41 Name Redacted** | An app user's name redacted is computed as the name. |
| **DR-42 Email Address Redacted** | An app user's email address redacted is computed as the email address. |
| **DR-43 Role Title** | An app user's role title is the title of the app user's role. |
| **DR-44 Role Description** | An app user's role description is the description of the app user's role. |
| **DR-45 Is Appeal Board Decision** | A reference document is considered an appeal board decision if the library is the literal “appeal-board-decisions”. |
| **DR-46 Package is Active** | A state machine's package is active is true when the state machine's ERB package is active. |
| **DR-47 State Count** | A state machine's state count is rolled up from its related records (``). |
| **DR-48 Transition Rule Count** | A state machine's transition rule count is rolled up from its related records (``). |
| **DR-49 From State Key** | A state transition rule's from state key is the state key of the state transition rule's from state. |
| **DR-50 To State Key** | A state transition rule's to state key is the state key of the state transition rule's to state. |
| **DR-51 Is Forward Edge** | A state transition rule is considered a forward edge if it is not the case that the to state key is the literal “draft”. |
| **DR-52 Is Forward** | A state transition is considered a forward if all of the following hold: it is not the case that the to state key is the literal “draft”; it is not the case that the to state key is the literal “new”; it is not the case that the to state key is the literal “pending”; it is not the case that the to state key is the literal “open”; and it is not the case that the to state key is the literal “issued”. |
| **DR-53 Due in Days** | A work queue item's due in days is computed as `Datetime_diff(DueDate, Today(), 'days')`. |
| **DR-54 Is Overdue** | A work queue item is considered an overdue if `If(DueInDays` is `Blank(),False(),DueInDays<0)`. |
| **DR-55 Urgency Bucket** | The work queue item's urgency bucket is determined by the following priority:<br>1. the literal “follow-up”, if the due in days is `Blank()`;<br>2. the literal “urgent”, if the due in days is at most 0;<br>3. the literal “due-3-days”, if the due in days is at most 3;<br>4. otherwise the literal “upcoming”. |
| **DR-56 Is Urgent** | A work queue item is considered an urgent if the urgency bucket is the literal “urgent”. |
| **DR-57 Pricing Version Count** | An ai model's pricing version count is the number of model pricing versions related to the ai model. |
| **DR-58 Turn Count** | An ai model's turn count is the number of assistant turns related to the ai model. |
| **DR-59 Total Cost** | An ai model's total cost is the total total cost across the assistant turns related to the ai model. |
| **DR-60 Ai Model Title** | A model pricing version's ai model title is the title of the model pricing version's ai model. |
| **DR-61 Turn Count** | A model pricing version's turn count is the number of assistant turns related to the model pricing version. |
| **DR-62 Total Tokens** | An assistant turn's total tokens is computed as `InputTokens+OutputTokens`. |
| **DR-63 Billable Input Tokens** | An assistant turn's billable input tokens is computed as `InputTokens-CachedInputTokens`. |
| **DR-64 Ai Model Title** | An assistant turn's ai model title is the title of the assistant turn's ai model. |
| **DR-65 Input Price Per M Tok** | An assistant turn's input price per m tok is the input price per m tok of the assistant turn's model pricing version. |
| **DR-66 Cached Input Price Per M Tok** | An assistant turn's cached input price per m tok is the cached input price per m tok of the assistant turn's model pricing version. |
| **DR-67 Output Price Per M Tok** | An assistant turn's output price per m tok is the output price per m tok of the assistant turn's model pricing version. |
| **DR-68 Input Cost** | An assistant turn's input cost is computed as `((BillableInputTokens*InputPricePerMTok)+(CachedInputTokens*CachedInputPricePerMTok))/1000000`. |
| **DR-69 Output Cost** | An assistant turn's output cost is computed as `(OutputTokens*OutputPricePerMTok)/1000000`. |
| **DR-70 Total Cost** | An assistant turn's total cost is computed as `InputCost+OutputCost`. |
| **DR-71 Feature Count** | An ERB package's feature count is computed as `Count(ERBFeatures)`. |
| **DR-72 Shipped Feature Count** | An ERB package's shipped feature count is computed as `Countif({{ERBFeatures.Status}}, "shipped")`. |
| **DR-73 Feature Count** | An ERB feature status's feature count is rolled up from its related records (``). |
| **DR-74 Feature Count** | An ERB feature category's feature count is rolled up from its related records (``). |
| **DR-75 Route Path** | An ERB feature's route path is the route of the ERB feature's route. |
| **DR-76 Relative Path** | An ERB feature's relative path is computed as the route path with every the literal “:featureId” replaced by the ERB feature ID. |
| **DR-77 Package is Licensed** | An ERB feature's package is licensed is true when the ERB feature's ERB package is licensed. |
| **DR-78 Category Title** | An ERB feature's category title is the title of the ERB feature's category. |
| **DR-79 Status Title** | An ERB feature's status title is the title of the ERB feature's status. |
| **DR-80 Status Description** | An ERB feature's status description is the description of the ERB feature's status. |
| **DR-81 Status is Active** | An ERB feature's status is active is true when the ERB feature's status is active. |
| **DR-82 Package is Active** | An ERB table's package is active is true when the ERB table's ERB package is active. |
| **DR-83 Package is Licensed** | An ERB table's package is licensed is true when the ERB table's ERB package is licensed. |
| **DR-84 Admin Can Create** | An ERB table is flagged admin can create if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",AdminCRUD))))`. |
| **DR-85 Admin Can Read** | An ERB table is flagged admin can read if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",AdminCRUD))))`. |
| **DR-86 Admin Can Update** | An ERB table is flagged admin can update if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",AdminCRUD))))`. |
| **DR-87 Admin Can Delete** | An ERB table is flagged admin can delete if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",AdminCRUD))))`. |
| **DR-88 Manager Can Create** | An ERB table is flagged manager can create if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",ManagerCRUD))))`. |
| **DR-89 Manager Can Read** | An ERB table is flagged manager can read if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",ManagerCRUD))))`. |
| **DR-90 Manager Can Update** | An ERB table is flagged manager can update if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",ManagerCRUD))))`. |
| **DR-91 Manager Can Delete** | An ERB table is flagged manager can delete if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",ManagerCRUD))))`. |
| **DR-92 Representative Can Create** | An ERB table is flagged representative can create if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",RepresentativeCRUD))))`. |
| **DR-93 Representative Can Read** | An ERB table is flagged representative can read if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",RepresentativeCRUD))))`. |
| **DR-94 Representative Can Update** | An ERB table is flagged representative can update if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",RepresentativeCRUD))))`. |
| **DR-95 Representative Can Delete** | An ERB table is flagged representative can delete if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",RepresentativeCRUD))))`. |
| **DR-96 External Llm Can Create** | An ERB table is flagged external llm can create if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",ExternalLlmCRUD))))`. |
| **DR-97 External Llm Can Read** | An ERB table is flagged external llm can read if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",ExternalLlmCRUD))))`. |
| **DR-98 External Llm Can Update** | An ERB table is flagged external llm can update if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",ExternalLlmCRUD))))`. |
| **DR-99 External Llm Can Delete** | An ERB table is flagged external llm can delete if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",ExternalLlmCRUD))))`. |
| **DR-100 Table Package is Active** | An ERB field's table package is active is true when the ERB field's ERB table is active. |
| **DR-101 Is Calculated** | An ERB field is considered calculated if at least one of the following holds: the field type is the literal “calculated”; the field type is the literal “lookup”; or the field type is the literal “aggregation”. |
| **DR-102 Admin Can Create** | An ERB field is flagged admin can create if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",AdminCRUD))))`. |
| **DR-103 Admin Can Read** | An ERB field is flagged admin can read if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",AdminCRUD))))`. |
| **DR-104 Admin Can Update** | An ERB field is flagged admin can update if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",AdminCRUD))))`. |
| **DR-105 Admin Can Delete** | An ERB field is flagged admin can delete if `If(AdminCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",AdminCRUD))))`. |
| **DR-106 Manager Can Create** | An ERB field is flagged manager can create if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",ManagerCRUD))))`. |
| **DR-107 Manager Can Read** | An ERB field is flagged manager can read if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",ManagerCRUD))))`. |
| **DR-108 Manager Can Update** | An ERB field is flagged manager can update if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",ManagerCRUD))))`. |
| **DR-109 Manager Can Delete** | An ERB field is flagged manager can delete if `If(ManagerCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",ManagerCRUD))))`. |
| **DR-110 Representative Can Create** | An ERB field is flagged representative can create if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",RepresentativeCRUD))))`. |
| **DR-111 Representative Can Read** | An ERB field is flagged representative can read if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",RepresentativeCRUD))))`. |
| **DR-112 Representative Can Update** | An ERB field is flagged representative can update if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",RepresentativeCRUD))))`. |
| **DR-113 Representative Can Delete** | An ERB field is flagged representative can delete if `If(RepresentativeCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",RepresentativeCRUD))))`. |
| **DR-114 External Llm Can Create** | An ERB field is flagged external llm can create if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("C",ExternalLlmCRUD))))`. |
| **DR-115 External Llm Can Read** | An ERB field is flagged external llm can read if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("R",ExternalLlmCRUD))))`. |
| **DR-116 External Llm Can Update** | An ERB field is flagged external llm can update if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("U",ExternalLlmCRUD))))`. |
| **DR-117 External Llm Can Delete** | An ERB field is flagged external llm can delete if `If(ExternalLlmCRUD` is `Blank(),Blank(),Not(Iserror(Find("D",ExternalLlmCRUD))))`. |
| **DR-118 Is Current** | A subject state instance is considered a current if `Isblank(ExitedAt)`. |
| **DR-119 Has Complete Lineage** | A subject state instance is considered to have a complete lineage if the sequence index is at least 1. |
| **DR-120 Jurisdiction Label** | A violation type's jurisdiction label is the display name of the violation type's jurisdiction. |
| **DR-121 Is School Eligible by Cap** | A violation type is considered a school eligible by cap if `If(Points` is at most `Lookup(Jurisdictions.TrafficSchoolPointCap, Match(Jurisdiction, Jurisdictions.Name, 0)), TRUE, FALSE)`. |
| **DR-122 Count of Citations** | A violation type's count of citations is the number of citations related to the violation type. |
| **DR-123 Full Name** | A driver's full name is computed as the last name, followed by a comma followed by a space, followed by the first name. |
| **DR-124 Home Jurisdiction Label** | A driver's home jurisdiction label is the display name of the driver's home jurisdiction. |
| **DR-125 Suspension Threshold** | A driver's suspension threshold is the point suspension threshold of the driver's home jurisdiction. |
| **DR-126 Warning Threshold** | A driver's warning threshold is the point warning threshold of the driver's home jurisdiction. |
| **DR-127 Count of Citations** | A driver's count of citations is the number of citations related to the driver. |
| **DR-128 Active Points** | A driver's active points is the total effective points across the citations related to the driver. |
| **DR-129 License Status** | The driver's license status is determined by the following priority:<br>1. the literal “Suspended”, if the active points is at least the suspension threshold;<br>2. the literal “Warning”, if the active points is at least the warning threshold;<br>3. otherwise the literal “Valid”. |
| **DR-130 Driver Label** | A citation's driver label is the full name of the citation's driver. |
| **DR-131 Violation Label** | A citation's violation label is the description of the citation's violation type. |
| **DR-132 Jurisdiction Label** | A citation's jurisdiction label is the display name of the citation's jurisdiction. |
| **DR-133 Base Fine USD** | A citation's base fine USD is the base fine USD of the citation's violation type. |
| **DR-134 Violation Points** | A citation's violation points is the points of the citation's violation type. |
| **DR-135 Days to Respond** | A citation's days to respond is the days to respond of the citation's jurisdiction. |
| **DR-136 Days to Pay After Ruling** | A citation's days to pay after ruling is the days to pay after ruling of the citation's jurisdiction. |
| **DR-137 Late Penalty Pct** | A citation's late penalty pct is the late penalty pct of the citation's jurisdiction. |
| **DR-138 Days Late to Collections** | A citation's days late to collections is the days late to collections of the citation's jurisdiction. |
| **DR-139 Response Due Date** | A citation's response due date is computed as `IssuedOn + DaysToRespond`. |
| **DR-140 Days Until Response Due** | A citation's days until response due is computed as `ResponseDueDate - AsOfDate`. |
| **DR-141 Is Response Overdue** | A citation is considered a response overdue if `If(And(Isblank(RespondedOn), AsOfDate` is greater than `ResponseDueDate), TRUE, FALSE)`. |
| **DR-142 Count of Hearings** | A citation's count of hearings is the number of hearings related to the citation. |
| **DR-143 Latest Hearing Outcome** | A citation's latest hearing outcome is rolled up from its related records (`Maxifs(Hearings.Outcome, Hearings.Citation, Name, Hearings.ScheduledFor, Maxifs(Hearings.ScheduledFor, Hearings.Citation, Name))`). |
| **DR-144 Contest Status** | The citation's contest status is determined by the following priority:<br>1. the literal “NotContested”, if it is not the case that the contest requested flag is set;<br>2. the literal “HearingRequested”, if the count of hearings is 0;<br>3. the literal “Scheduled”, if at least one of the following holds: the latest hearing outcome is the literal “Pending” or `Isblank(LatestHearingOutcome)`;<br>4. otherwise the literal “Heard”. |
| **DR-145 Is Dismissed** | A citation is considered dismissed if `If(LatestHearingOutcome` is `"Dismissed", TRUE, FALSE)`. |
| **DR-146 Is Guilty** | A citation is considered a guilty if `If(Or(LatestHearingOutcome` is `"Guilty", LatestHearingOutcome = "Upheld", And(IsResponseOverdue, Not(ContestRequested))), TRUE, FALSE)`. |
| **DR-147 Amount Due USD** | The citation's amount due USD is determined by the following priority:<br>1. 0, if the is dismissed flag is set;<br>2. `BaseFineUsd * (1 + LatePenaltyPct)`, if the is payment late flag is set;<br>3. otherwise the base fine USD. |
| **DR-148 Payment Due Date** | A citation's payment due date is computed as `ResponseDueDate + DaysToPayAfterRuling`. |
| **DR-149 Is Payment Late** | A citation is considered a payment late if `If(And(IsGuilty, Isblank(PaidOn), AsOfDate` is greater than `PaymentDueDate), TRUE, FALSE)`. |
| **DR-150 Is in Collections** | A citation is considered in collections if `If(And(IsPaymentLate, AsOfDate` is greater than `(PaymentDueDate + DaysLateToCollections)), TRUE, FALSE)`. |
| **DR-151 Payment Status** | The citation's payment status is determined by the following priority:<br>1. the literal “NotOwed”, if the is dismissed flag is set;<br>2. the literal “Paid”, if it is not the case that `Isblank(PaidOn)`;<br>3. the literal “Collections”, if the is in collections flag is set;<br>4. the literal “Late”, if the is payment late flag is set;<br>5. the literal “Due”, if the is guilty flag is set;<br>6. otherwise the literal “Pending”. |
| **DR-152 Effective Points** | The citation's effective points is determined by the following priority:<br>1. the violation points, if all of the following hold: the is guilty flag is set and it is not the case that the is dismissed flag is set;<br>2. otherwise 0. |
| **DR-153 Citation Status** | The citation's citation status is determined by the following priority:<br>1. the literal “Closed”, if at least one of the following holds: it is not the case that `Isblank(PaidOn)` or the is dismissed flag is set;<br>2. the literal “Adjudicated”, if at least one of the following holds: the latest hearing outcome is the literal “Guilty”; the latest hearing outcome is the literal “Upheld”; or all of the following hold: the is response overdue flag is set and it is not the case that the contest requested flag is set;<br>3. the literal “InContest”, if all of the following hold: the contest requested flag is set and the count of hearings is greater than 0;<br>4. the literal “Responded”, if it is not the case that `Isblank(RespondedOn)`;<br>5. otherwise the literal “Issued”. |
| **DR-154 Citation Label** | A hearing's citation label is the citation number of the hearing's citation. |
| **DR-155 Citation Label** | A payment's citation label is the citation number of the payment's citation. |
| **DR-156 Citation Label** | A case event's citation label is the citation number of the case event's citation. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **BusinessRuleCategories.RuleCount** | rollup | — |
| **GlossaryCategories.TermCount** | rollup | — |
| **Roles.AppUserCount** | rollup | — |
| **AuditLogEntries.IsOverrideAction** | formula | `If(Lower(ActionType & "") = "override", True(), False())` |
| **AuditLogEntries.EntryAgeHours** | formula | `Datetime_diff(Now(), Timestamp, 'hours')` |
| **PlatformNaviation.PackageIsActive** | lookup | `Lookup(ERBPackages.IsActive via ERBPackage)` |
| **PlatformNaviation.PackageIsLicensed** | lookup | `Lookup(ERBPackages.IsLicensed via ERBPackage)` |
| **PlatformNaviation.AdminCanCreate** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("C",AdminCRUD))))` |
| **PlatformNaviation.AdminCanRead** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("R",AdminCRUD))))` |
| **PlatformNaviation.AdminCanUpdate** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("U",AdminCRUD))))` |
| **PlatformNaviation.AdminCanDelete** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("D",AdminCRUD))))` |
| **PlatformNaviation.ManagerCanCreate** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("C",ManagerCRUD))))` |
| **PlatformNaviation.ManagerCanRead** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("R",ManagerCRUD))))` |
| **PlatformNaviation.ManagerCanUpdate** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("U",ManagerCRUD))))` |
| **PlatformNaviation.ManagerCanDelete** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("D",ManagerCRUD))))` |
| **PlatformNaviation.RepresentativeCanCreate** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("C",RepresentativeCRUD))))` |
| **PlatformNaviation.RepresentativeCanRead** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("R",RepresentativeCRUD))))` |
| **PlatformNaviation.RepresentativeCanUpdate** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("U",RepresentativeCRUD))))` |
| **PlatformNaviation.RepresentativeCanDelete** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("D",RepresentativeCRUD))))` |
| **PlatformNaviation.ExternalLlmCanCreate** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("C",ExternalLlmCRUD))))` |
| **PlatformNaviation.ExternalLlmCanRead** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("R",ExternalLlmCRUD))))` |
| **PlatformNaviation.ExternalLlmCanUpdate** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("U",ExternalLlmCRUD))))` |
| **PlatformNaviation.ExternalLlmCanDelete** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("D",ExternalLlmCRUD))))` |
| **PlatformNaviation.Depth** | formula | `If(ParentRouteKey=Blank(), 0, Len(RouteKey) - Len(Replace(RouteKey, ".", "")))` |
| **PlatformNaviation.FullPath** | formula | `Route` |
| **PlatformNaviation.HandlerBaseName** | formula | `Replace(Replace(RouteKey, ".", " "), "-", " ")` |
| **Jurisdictions.ParentJurisdictionName** | lookup | `Lookup(Jurisdictions.Name via ParentJurisdiction)` |
| **Jurisdictions.IsRootJurisdiction** | formula | `If(ParentJurisdiction=Blank(), True(), False())` |
| **Jurisdictions.ChildJurisdictionCount** | rollup | `Count(Jurisdictions via ParentJurisdiction)` |
| **Jurisdictions.RelativePath** | formula | `"/library/jurisdictions/" & JurisdictionId` |
| **Jurisdictions.RuleCount** | rollup | `Count(JurisdictionRules via Jurisdiction)` |
| **Jurisdictions.SourceDocumentCount** | rollup | `Count(JurisdictionSourceDocuments via Jurisdiction)` |
| **JurisdictionSourceDocuments.JurisdictionName** | lookup | `Lookup(Jurisdictions.Name via Jurisdiction)` |
| **JurisdictionSourceDocuments.RelativePath** | formula | `"/library/jurisdiction-docs/" & JurisdictionSourceDocumentId` |
| **JurisdictionSourceDocuments.RuleCount** | rollup | `Count(JurisdictionRules via SourceDocument)` |
| **JurisdictionRules.RoutePath** | lookup | `Lookup(PlatformNaviation.Route via Route)` |
| **JurisdictionRules.RelativePath** | formula | `"/library/jurisdiction-rules/" & JurisdictionRuleId` |
| **JurisdictionRules.JurisdictionName** | lookup | `Lookup(Jurisdictions.Name via Jurisdiction)` |
| **JurisdictionRules.JurisdictionType** | lookup | `Lookup(Jurisdictions.JurisdictionType via Jurisdiction)` |
| **JurisdictionRules.IsFederal** | formula | `If(JurisdictionType = "Country", True(), False())` |
| **AppUsers.NameRedacted** | formula | `Name` |
| **AppUsers.EmailAddressRedacted** | formula | `EmailAddress` |
| **AppUsers.RoleTitle** | lookup | `Lookup(Roles.Title via Role)` |
| **AppUsers.RoleDescription** | lookup | `Lookup(Roles.Description via Role)` |
| **ReferenceDocuments.IsAppealBoardDecision** | formula | `Library="appeal-board-decisions"` |
| **StateMachines.PackageIsActive** | lookup | `Lookup(ERBPackages.IsActive via ERBPackage)` |
| **StateMachines.StateCount** | rollup | — |
| **StateMachines.TransitionRuleCount** | rollup | — |
| **StateTransitionRules.FromStateKey** | lookup | `Lookup(MachineStates.StateKey via FromState)` |
| **StateTransitionRules.ToStateKey** | lookup | `Lookup(MachineStates.StateKey via ToState)` |
| **StateTransitionRules.IsForwardEdge** | formula | `Not(ToStateKey="draft")` |
| **StateTransitions.IsForward** | formula | `And(Not(ToStateKey="draft"),Not(ToStateKey="new"),Not(ToStateKey="pending"),Not(ToStateKey="open"),Not(ToStateKey="issued"))` |
| **WorkQueueItems.DueInDays** | formula | `Datetime_diff(DueDate, Today(), 'days')` |
| **WorkQueueItems.IsOverdue** | formula | `If(DueInDays=Blank(),False(),DueInDays<0)` |
| **WorkQueueItems.UrgencyBucket** | formula | `If(DueInDays=Blank(),"follow-up",If(DueInDays<=0,"urgent",If(DueInDays<=3,"due-3-days","upcoming")))` |
| **WorkQueueItems.IsUrgent** | formula | `UrgencyBucket="urgent"` |
| **AiModels.PricingVersionCount** | rollup | `Count(ModelPricingVersions via AiModel)` |
| **AiModels.TurnCount** | rollup | `Count(AssistantTurns via AiModel)` |
| **AiModels.TotalCost** | rollup | `Sum(AssistantTurns.TotalCost via AiModel)` |
| **ModelPricingVersions.AiModelTitle** | lookup | `Lookup(AiModels.Title via AiModel)` |
| **ModelPricingVersions.TurnCount** | rollup | `Count(AssistantTurns via ModelPricingVersion)` |
| **AssistantTurns.TotalTokens** | formula | `InputTokens+OutputTokens` |
| **AssistantTurns.BillableInputTokens** | formula | `InputTokens-CachedInputTokens` |
| **AssistantTurns.AiModelTitle** | lookup | `Lookup(AiModels.Title via AiModel)` |
| **AssistantTurns.InputPricePerMTok** | lookup | `Lookup(ModelPricingVersions.InputPricePerMTok via ModelPricingVersion)` |
| **AssistantTurns.CachedInputPricePerMTok** | lookup | `Lookup(ModelPricingVersions.CachedInputPricePerMTok via ModelPricingVersion)` |
| **AssistantTurns.OutputPricePerMTok** | lookup | `Lookup(ModelPricingVersions.OutputPricePerMTok via ModelPricingVersion)` |
| **AssistantTurns.InputCost** | formula | `((BillableInputTokens*InputPricePerMTok)+(CachedInputTokens*CachedInputPricePerMTok))/1000000` |
| **AssistantTurns.OutputCost** | formula | `(OutputTokens*OutputPricePerMTok)/1000000` |
| **AssistantTurns.TotalCost** | formula | `InputCost+OutputCost` |
| **ERBPackages.FeatureCount** | formula | `Count(ERBFeatures)` |
| **ERBPackages.ShippedFeatureCount** | formula | `Countif({{ERBFeatures.Status}}, "shipped")` |
| **ERBFeatureStatuses.FeatureCount** | rollup | — |
| **ERBFeatureCategories.FeatureCount** | rollup | — |
| **ERBFeatures.RoutePath** | lookup | `Lookup(PlatformNaviation.Route via Route)` |
| **ERBFeatures.RelativePath** | formula | `Replace(RoutePath, ":featureId", ERBFeatureId)` |
| **ERBFeatures.PackageIsLicensed** | lookup | `Lookup(ERBPackages.IsLicensed via ERBPackage)` |
| **ERBFeatures.CategoryTitle** | lookup | `Lookup(ERBFeatureCategories.Title via Category)` |
| **ERBFeatures.StatusTitle** | lookup | `Lookup(ERBFeatureStatuses.Title via Status)` |
| **ERBFeatures.StatusDescription** | lookup | `Lookup(ERBFeatureStatuses.Description via Status)` |
| **ERBFeatures.StatusIsActive** | lookup | `Lookup(ERBFeatureStatuses.IsActive via Status)` |
| **ERBTables.PackageIsActive** | lookup | `Lookup(ERBPackages.IsActive via ERBPackage)` |
| **ERBTables.PackageIsLicensed** | lookup | `Lookup(ERBPackages.IsLicensed via ERBPackage)` |
| **ERBTables.AdminCanCreate** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("C",AdminCRUD))))` |
| **ERBTables.AdminCanRead** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("R",AdminCRUD))))` |
| **ERBTables.AdminCanUpdate** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("U",AdminCRUD))))` |
| **ERBTables.AdminCanDelete** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("D",AdminCRUD))))` |
| **ERBTables.ManagerCanCreate** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("C",ManagerCRUD))))` |
| **ERBTables.ManagerCanRead** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("R",ManagerCRUD))))` |
| **ERBTables.ManagerCanUpdate** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("U",ManagerCRUD))))` |
| **ERBTables.ManagerCanDelete** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("D",ManagerCRUD))))` |
| **ERBTables.RepresentativeCanCreate** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("C",RepresentativeCRUD))))` |
| **ERBTables.RepresentativeCanRead** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("R",RepresentativeCRUD))))` |
| **ERBTables.RepresentativeCanUpdate** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("U",RepresentativeCRUD))))` |
| **ERBTables.RepresentativeCanDelete** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("D",RepresentativeCRUD))))` |
| **ERBTables.ExternalLlmCanCreate** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("C",ExternalLlmCRUD))))` |
| **ERBTables.ExternalLlmCanRead** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("R",ExternalLlmCRUD))))` |
| **ERBTables.ExternalLlmCanUpdate** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("U",ExternalLlmCRUD))))` |
| **ERBTables.ExternalLlmCanDelete** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("D",ExternalLlmCRUD))))` |
| **ERBFields.TablePackageIsActive** | lookup | `Lookup(ERBTables.PackageIsActive via ERBTable)` |
| **ERBFields.IsCalculated** | formula | `Or(FieldType="calculated",FieldType="lookup",FieldType="aggregation")` |
| **ERBFields.AdminCanCreate** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("C",AdminCRUD))))` |
| **ERBFields.AdminCanRead** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("R",AdminCRUD))))` |
| **ERBFields.AdminCanUpdate** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("U",AdminCRUD))))` |
| **ERBFields.AdminCanDelete** | formula | `If(AdminCRUD=Blank(),Blank(),Not(Iserror(Find("D",AdminCRUD))))` |
| **ERBFields.ManagerCanCreate** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("C",ManagerCRUD))))` |
| **ERBFields.ManagerCanRead** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("R",ManagerCRUD))))` |
| **ERBFields.ManagerCanUpdate** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("U",ManagerCRUD))))` |
| **ERBFields.ManagerCanDelete** | formula | `If(ManagerCRUD=Blank(),Blank(),Not(Iserror(Find("D",ManagerCRUD))))` |
| **ERBFields.RepresentativeCanCreate** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("C",RepresentativeCRUD))))` |
| **ERBFields.RepresentativeCanRead** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("R",RepresentativeCRUD))))` |
| **ERBFields.RepresentativeCanUpdate** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("U",RepresentativeCRUD))))` |
| **ERBFields.RepresentativeCanDelete** | formula | `If(RepresentativeCRUD=Blank(),Blank(),Not(Iserror(Find("D",RepresentativeCRUD))))` |
| **ERBFields.ExternalLlmCanCreate** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("C",ExternalLlmCRUD))))` |
| **ERBFields.ExternalLlmCanRead** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("R",ExternalLlmCRUD))))` |
| **ERBFields.ExternalLlmCanUpdate** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("U",ExternalLlmCRUD))))` |
| **ERBFields.ExternalLlmCanDelete** | formula | `If(ExternalLlmCRUD=Blank(),Blank(),Not(Iserror(Find("D",ExternalLlmCRUD))))` |
| **SubjectStateInstances.IsCurrent** | formula | `Isblank(ExitedAt)` |
| **SubjectStateInstances.HasCompleteLineage** | formula | `SequenceIndex>=1` |
| **ViolationTypes.JurisdictionLabel** | lookup | `Lookup(Jurisdictions.DisplayName via Jurisdiction)` |
| **ViolationTypes.IsSchoolEligibleByCap** | formula | `If(Points <= Lookup(Jurisdictions.TrafficSchoolPointCap, Match(Jurisdiction, Jurisdictions.Name, 0)), TRUE, FALSE)` |
| **ViolationTypes.CountOfCitations** | rollup | `Count(Citations via ViolationType)` |
| **Drivers.FullName** | formula | `LastName & ", " & FirstName` |
| **Drivers.HomeJurisdictionLabel** | lookup | `Lookup(Jurisdictions.DisplayName via HomeJurisdiction)` |
| **Drivers.SuspensionThreshold** | lookup | `Lookup(Jurisdictions.PointSuspensionThreshold via HomeJurisdiction)` |
| **Drivers.WarningThreshold** | lookup | `Lookup(Jurisdictions.PointWarningThreshold via HomeJurisdiction)` |
| **Drivers.CountOfCitations** | rollup | `Count(Citations via Driver)` |
| **Drivers.ActivePoints** | rollup | `Sum(Citations.EffectivePoints via Driver)` |
| **Drivers.LicenseStatus** | formula | `If(ActivePoints >= SuspensionThreshold, "Suspended", If(ActivePoints >= WarningThreshold, "Warning", "Valid"))` |
| **Citations.DriverLabel** | lookup | `Lookup(Drivers.FullName via Driver)` |
| **Citations.ViolationLabel** | lookup | `Lookup(ViolationTypes.Description via ViolationType)` |
| **Citations.JurisdictionLabel** | lookup | `Lookup(Jurisdictions.DisplayName via Jurisdiction)` |
| **Citations.BaseFineUsd** | lookup | `Lookup(ViolationTypes.BaseFineUsd via ViolationType)` |
| **Citations.ViolationPoints** | lookup | `Lookup(ViolationTypes.Points via ViolationType)` |
| **Citations.DaysToRespond** | lookup | `Lookup(Jurisdictions.DaysToRespond via Jurisdiction)` |
| **Citations.DaysToPayAfterRuling** | lookup | `Lookup(Jurisdictions.DaysToPayAfterRuling via Jurisdiction)` |
| **Citations.LatePenaltyPct** | lookup | `Lookup(Jurisdictions.LatePenaltyPct via Jurisdiction)` |
| **Citations.DaysLateToCollections** | lookup | `Lookup(Jurisdictions.DaysLateToCollections via Jurisdiction)` |
| **Citations.ResponseDueDate** | formula | `IssuedOn + DaysToRespond` |
| **Citations.DaysUntilResponseDue** | formula | `ResponseDueDate - AsOfDate` |
| **Citations.IsResponseOverdue** | formula | `If(And(Isblank(RespondedOn), AsOfDate > ResponseDueDate), TRUE, FALSE)` |
| **Citations.CountOfHearings** | rollup | `Count(Hearings via Citation)` |
| **Citations.LatestHearingOutcome** | rollup | `Maxifs(Hearings.Outcome, Hearings.Citation, Name, Hearings.ScheduledFor, Maxifs(Hearings.ScheduledFor, Hearings.Citation, Name))` |
| **Citations.ContestStatus** | formula | `If(Not(ContestRequested), "NotContested", If(CountOfHearings = 0, "HearingRequested", If(Or(LatestHearingOutcome = "Pending", Isblank(LatestHearingOutcome)), "Scheduled", "Heard")))` |
| **Citations.IsDismissed** | formula | `If(LatestHearingOutcome = "Dismissed", TRUE, FALSE)` |
| **Citations.IsGuilty** | formula | `If(Or(LatestHearingOutcome = "Guilty", LatestHearingOutcome = "Upheld", And(IsResponseOverdue, Not(ContestRequested))), TRUE, FALSE)` |
| **Citations.AmountDueUsd** | formula | `If(IsDismissed, 0, If(IsPaymentLate, BaseFineUsd * (1 + LatePenaltyPct), BaseFineUsd))` |
| **Citations.PaymentDueDate** | formula | `ResponseDueDate + DaysToPayAfterRuling` |
| **Citations.IsPaymentLate** | formula | `If(And(IsGuilty, Isblank(PaidOn), AsOfDate > PaymentDueDate), TRUE, FALSE)` |
| **Citations.IsInCollections** | formula | `If(And(IsPaymentLate, AsOfDate > (PaymentDueDate + DaysLateToCollections)), TRUE, FALSE)` |
| **Citations.PaymentStatus** | formula | `If(IsDismissed, "NotOwed", If(Not(Isblank(PaidOn)), "Paid", If(IsInCollections, "Collections", If(IsPaymentLate, "Late", If(IsGuilty, "Due", "Pending")))))` |
| **Citations.EffectivePoints** | formula | `If(And(IsGuilty, Not(IsDismissed)), ViolationPoints, 0)` |
| **Citations.CitationStatus** | formula | `If(Or(Not(Isblank(PaidOn)), IsDismissed), "Closed", If(Or(LatestHearingOutcome = "Guilty", LatestHearingOutcome = "Upheld", And(IsResponseOverdue, Not(ContestRequested))), "Adjudicated", If(And(ContestRequested, CountOfHearings > 0), "InContest", If(Not(Isblank(RespondedOn)), "Responded", "Issued"))))` |
| **Hearings.CitationLabel** | lookup | `Lookup(Citations.CitationNumber via Citation)` |
| **Payments.CitationLabel** | lookup | `Lookup(Citations.CitationNumber via Citation)` |
| **CaseEvents.CitationLabel** | lookup | `Lookup(Citations.CitationNumber via Citation)` |
