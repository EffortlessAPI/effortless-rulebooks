# PKO-Native Procedural Knowledge Rulebook

A canonical Effortless Rulebook profile aligned to the Procedural Knowledge Ontology (PKO) 2.0.0. It represents procedure specifications separately from executions and includes versioning, status changes, steps, transitions, actions, software functions, tools, requirements, verifications, resources, agents, roles in time, issues, errors, questions, feedback, FAQs, explanations, governance, tacit/implicit/explicit knowledge capture, operational data bindings, learning, and communication policy projections. PKO-native terms are mapped exactly; enterprise knowledge-governance additions are explicitly identified as ERB-PKO extensions.

## Semantic contract

- Canonical rulebook release: **1.0.0**
- ERB-PKO profile: **1.0.0**
- PKO core version IRI: **https://w3id.org/pko/2.0.0**
- PKO industry version IRI: **https://w3id.org/pko/industry/2.0.0**
- Procedure specifications and executions are represented as different entities.
- Native PKO mappings and ERB-PKO extensions are distinguished in `SemanticMappings`.

## Quarter-End Financial Close — v1.1.0

**Type:** Financial Standard Operating Procedure  
**Status:** Approved  
**Purpose:** Close the general ledger accurately, produce evidence, and obtain independent approval.  
**Target:** Corporate general ledger and reporting package  
**Owner:** ACME Finance  
**Version motivation:** Capture FX timestamp failure mode and AI-assisted variance triage.

### Procedure

| # | Step | Role | Current agent type | Expected | Human confirmation |
|---:|---|---|---|---:|---|
| 01 | Freeze transaction entry | Close Automation Operator |  | 5 min | No |
| 02 | Extract trial balance and feed timestamps | Close Automation Operator |  | 10 min | No |
| 03 | Reconcile material accounts | Finance Analyst |  | 120 min | No |
| 04 | Classify and investigate variances | Variance Review Agent |  | 90 min | No |
| 05 | Controller review | Corporate Controller |  | 60 min | Yes |
| 06 | CFO approval | Chief Financial Officer |  | 30 min | Yes |
| 07 | Post close and publish package | Close Automation Operator |  | 20 min | No |
| 08 | Archive evidence and hold retrospective | Procedural Knowledge Steward |  | 45 min | No |

#### 01. Freeze transaction entry

Lock subledgers and prevent late postings.

- **Responsible role:** Close Automation Operator
- **Current role filler:** Close Automation Pipeline (AutomatedPipeline)
- **Expertise:** Senior
- **Human actions:** Freeze ledgers
- **Tools:** ERP General Ledger
- **Requirements:** Close cutoff enforced
- **Verification:** The ERP reports the close period locked. (`ledger-lock-state` = `LOCKED`)

#### 02. Extract trial balance and feed timestamps

Extract the trial balance and source-system timestamps.

- **Responsible role:** Close Automation Operator
- **Current role filler:** Close Automation Pipeline (AutomatedPipeline)
- **Expertise:** Senior
- **Software functions:** Extract trial balance
- **Tools:** ERP General Ledger
- **Verification:** All source feeds are no more than 15 minutes old. (`max-feed-age-minutes` = `15`)
- **Exception — ERP source unavailable:** Use a digitally signed, timestamped snapshot and open a blocking incident; do not silently substitute a spreadsheet.

#### 03. Reconcile material accounts

Reconcile material accounts and attach evidence.

- **Responsible role:** Finance Analyst
- **Current role filler:** Maria Chen (Human)
- **Expertise:** Senior
- **Human actions:** Reconcile account
- **Tools:** Close Workpaper Workbook
- **Requirements:** Reconciled balance integrity; Evidence retention
- **Verification:** No unexplained material variance remains. (`unexplained-material-variance` = `0`)

#### 04. Classify and investigate variances

Use AI triage, then apply human situated judgment to material or anomalous variances.

- **Responsible role:** Variance Review Agent
- **Current role filler:** Variance Analysis AI (AIAgent)
- **Expertise:** Expert
- **Human actions:** Investigate variance
- **Software functions:** Classify variance
- **Tools:** Variance AI Console; Close Workpaper Workbook
- **Rationale:** The model accelerates triage; materiality, anomaly context, and acceptance remain human judgments.
- **Tacit knowledge:** When FX variance spikes near a daylight-saving cutover, compare each feed's source timestamp before investigating economics. (confidence: High, status: Approved)
- **Implicit knowledge:** The controller normally reviews the ten largest unexplained variances even when each is below the individual materiality threshold. (confidence: Medium, status: Approved)
- **SituatedJudgment knowledge:** Escalate a variance when its pattern is anomalous or recurring, even if the net amount is below the numeric threshold. (confidence: High, status: Approved)

#### 05. Controller review

Independently review reconciliations, evidence, and unresolved items.

- **Responsible role:** Corporate Controller
- **Current role filler:** Devon Okafor (Human)
- **Expertise:** Expert
- **Human actions:** Review close package
- **Requirements:** Preparer/approver separation
- **Verification:** Controller approval record exists. (`controller-approval` = `APPROVED`)
- **Rationale:** Independent review prevents self-certification and makes accountability legible.

#### 06. CFO approval

Approve the close package or return it with required corrections.

- **Responsible role:** Chief Financial Officer
- **Current role filler:** Priya Raman (Human)
- **Expertise:** Master
- **Human actions:** Approve close
- **Requirements:** Human final approval
- **Verification:** CFO approval record exists. (`cfo-approval` = `APPROVED`)
- **Exception — CFO unavailable during close:** Delegate approval to the formally designated Acting CFO; never to the preparer.

#### 07. Post close and publish package

Post approved entries and publish the reporting package.

- **Responsible role:** Close Automation Operator
- **Current role filler:** Close Automation Pipeline (AutomatedPipeline)
- **Expertise:** Senior
- **Software functions:** Post close entries

#### 08. Archive evidence and hold retrospective

Archive evidence and capture questions, issues, and lessons learned.

- **Responsible role:** Procedural Knowledge Steward
- **Current role filler:** Elena Garcia (Human)
- **Expertise:** Senior
- **Requirements:** Evidence retention
- **Explicit knowledge:** Close evidence is retained for seven years and must remain linked to the execution that produced it. (confidence: High, status: Approved)

### Referenced resources

- **Quarter-End Close SOP PDF** — Document; `doc://finance/close-sop-v1.0`
- **ERP Ledger API** — API; `api://erp/general-ledger/v3`
- **Close Control Checklist** — Checklist; `doc://finance/close-checklist-v1.1`

### Governance and knowledge health

- **Steward:** Procedural Knowledge Steward; **authority:** Procedural Knowledge Authority; review every 90 days.
- **Knowledge gap (Resolved, High):** No validated fallback exists for simultaneous ERP and signed-snapshot repository outage.
- **Change request (Approved):** Add source timestamp as blocking verification
- **Review:** QuarterlySemanticReview on 2026-07-02T16:00:00-05:00 — PassedWithChange

### Execution witnesses

- **Quarter ended 2026-06-30** — Completed, 2026-06-30T17:00:00-05:00 → 2026-07-02T16:45:00-05:00; `execution://finance/close/2026-q2`

## Workforce Policy Change and Employee Notification — v1.0.0

**Type:** Employee Communication Policy Procedure  
**Status:** Approved  
**Purpose:** Turn an approved workforce policy change into governed, channel-appropriate employee communications and acknowledgements.  
**Target:** All affected employees and workforce-policy records  
**Owner:** ACME People Operations  
**Version motivation:** Initial governed policy-notification procedure.

### Procedure

| # | Step | Role | Current agent type | Expected | Human confirmation |
|---:|---|---|---|---:|---|
| 01 | Identify policy trigger and affected population | People Policy Owner |  | 30 min | No |
| 02 | Elicit process and practitioner knowledge | Procedural Knowledge Steward |  | 180 min | No |
| 03 | Draft policy and channel variants | Policy Drafting Agent |  | 90 min | No |
| 04 | Legal and privacy review | Employment Counsel |  | 120 min | Yes |
| 05 | Human authority approval | People Policy Owner |  | 45 min | Yes |
| 06 | Generate approved communications | Policy Drafting Agent |  | 30 min | No |
| 07 | Apply channel controls and send | Notification Publisher |  | 60 min | No |
| 08 | Collect acknowledgements and exceptions | Notification Publisher |  | 1440 min | No |
| 09 | Review feedback and update knowledge | Procedural Knowledge Steward |  | 120 min | No |

#### 01. Identify policy trigger and affected population

Establish the legal/business trigger and population affected.

- **Responsible role:** People Policy Owner
- **Current role filler:** Elena Garcia (Human)
- **Expertise:** Senior

#### 02. Elicit process and practitioner knowledge

Interview practitioners, inspect source artifacts, and record tacit/implicit/explicit knowledge.

- **Responsible role:** Procedural Knowledge Steward
- **Current role filler:** Elena Garcia (Human)
- **Expertise:** Expert
- **Human actions:** Interview practitioner
- **Tools:** Knowledge Capture Form

#### 03. Draft policy and channel variants

Draft the policy and proposed email/SMS variants from approved sources.

- **Responsible role:** Policy Drafting Agent
- **Current role filler:** Policy Drafting AI (AIAgent)
- **Expertise:** Senior
- **Software functions:** Draft policy
- **Tools:** Policy Registry
- **Requirements:** Approved source only
- **Verification:** Draft contains no unapproved source. (`unapproved-source-count` = `0`)
- **Rationale:** Drafting is reversible assistance; legal and organizational commitments require human authority.
- **Explicit knowledge:** AI may draft from approved sources but may not create new legal commitments or approve meaning. (confidence: High, status: Approved)

#### 04. Legal and privacy review

Review legal commitments, privacy, consent, retention, and jurisdictional requirements.

- **Responsible role:** Employment Counsel
- **Current role filler:** Noah Williams (Human)
- **Expertise:** Expert
- **Human actions:** Review employment policy
- **Requirements:** Legal review required
- **Verification:** Employment counsel approval exists. (`legal-approval` = `APPROVED`)

#### 05. Human authority approval

Approve the policy meaning and all channel constraints.

- **Responsible role:** People Policy Owner
- **Current role filler:** Elena Garcia (Human)
- **Expertise:** Master
- **Human actions:** Approve employment policy
- **Requirements:** Human meaning approval
- **Verification:** Human policy-owner approval exists. (`policy-owner-approval` = `APPROVED`)

#### 06. Generate approved communications

Render approved email and SMS messages without changing policy meaning.

- **Responsible role:** Policy Drafting Agent
- **Current role filler:** Policy Drafting AI (AIAgent)
- **Expertise:** Senior
- **Software functions:** Render email; Render SMS
- **Requirements:** Plain-language and accessibility

#### 07. Apply channel controls and send

Enforce consent, quiet hours, opt-out text, recipient scope, and delivery logging.

- **Responsible role:** Notification Publisher
- **Current role filler:** Employee Notification Pipeline (AutomatedPipeline)
- **Expertise:** Senior
- **Software functions:** Send notification
- **Tools:** Employee Consent Registry; Email Gateway; SMS Gateway
- **Requirements:** SMS consent required; Quiet hours enforced; Opt-out text required; Communication evidence retention
- **Verification:** No SMS recipient lacks active consent. (`sms-without-consent-count` = `0`)
- **Verification:** No message was sent during quiet hours. (`quiet-hours-violation-count` = `0`)
- **Exception — Recipient lacks SMS consent:** Suppress SMS and send approved email only.
- **Rationale:** Email carries complete policy context; consented SMS provides timely notice and a link without becoming the legal record.
- **Tacit knowledge:** Employees respond faster to concise SMS notices, but disputes are resolved against the complete approved email and policy record. (confidence: High, status: Approved)

#### 08. Collect acknowledgements and exceptions

Collect acknowledgements and create exception tasks for unreachable recipients.

- **Responsible role:** Notification Publisher
- **Current role filler:** Employee Notification Pipeline (AutomatedPipeline)
- **Expertise:** Senior
- **Software functions:** Collect acknowledgement
- **Requirements:** Communication evidence retention
- **Verification:** All recipients acknowledged or have an exception task. (`unresolved-recipient-count` = `0`)
- **Exception — Recipient unreachable:** Create an HR case and route to the employee's manager without marking acknowledgement complete.
- **Implicit knowledge:** When an employee is unreachable for 48 hours, the communications team normally routes a case to the employee's manager. (confidence: Medium, status: Reviewed)

#### 09. Review feedback and update knowledge

Review questions, feedback, delivery failures, and proposed knowledge changes.

- **Responsible role:** Procedural Knowledge Steward
- **Current role filler:** Elena Garcia (Human)
- **Expertise:** Expert
- **Human actions:** Review procedural feedback

### Referenced resources

- **Approved Workforce Policy Source** — Document; `doc://people/policy-remote-work-v3`
- **Employee Communication Consent Registry** — Dataset; `data://hr/communication-consent`
- **Employee Policy Email Template** — Template; `template://people/policy-email-v2`
- **Employee Policy SMS Template** — Template; `template://people/policy-sms-v2`
- **Policy Change Ticket HR-4821** — OperationalRecord; `ticket://hr/HR-4821`

### Governance and knowledge health

- **Steward:** Procedural Knowledge Steward; **authority:** People Policy Owner; review every 60 days.
- **Knowledge gap (Open, Medium):** SMS provider delivery receipts are not always final for ported numbers.
- **Change request (UnderReview):** Add secondary delivery receipt reconciliation
- **Review:** PreLaunchReview on 2026-07-18T16:00:00-05:00 — Passed

### Execution witnesses

- **Remote-work policy revision HR-4821** — InProgress, 2026-07-18T09:00:00-05:00 → —; `execution://people/policy/HR-4821`

## Cross-cutting knowledge infrastructure

- Elicitation sessions: **3**
- Captured knowledge fragments: **7**
- Communities of practice: **2**
- Historical role assignments: **8**
- Live operational bindings: **5**
- Semantic mappings: **32**

This document is derivative. Change the canonical rulebook, then regenerate it.
