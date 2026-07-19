# Financial Standard Operating Procedures

Generated from `PKO-Native Procedural Knowledge Rulebook`; PKO profile https://w3id.org/pko/2.0.0.

## Quarter-End Financial Close — version 1.1.0

**Control objective:** Close the general ledger accurately, produce evidence, and obtain independent approval.

### Roles and segregation of duties

- **Close Automation Operator** — Deterministic extraction, posting, and archival operator.; current filler: Close Automation Pipeline (AutomatedPipeline).
- **Finance Analyst** — Preparer of reconciliations and variance evidence.; current filler: Maria Chen (Human).
- **Variance Review Agent** — AI assistant that classifies and prioritizes variances.; current filler: Variance Analysis AI (AIAgent).
- **Corporate Controller** — Owner of close controls and first approval authority.; current filler: Devon Okafor (Human).
- **Chief Financial Officer** — Final authority for the financial close.; current filler: Priya Raman (Human).
- **Procedural Knowledge Steward** — Maintains procedural knowledge health and review cadence.; current filler: Elena Garcia (Human).

### Required controls

- **Reconciled balance integrity:** Every material account must reconcile or carry an approved exception. _Rationale: Financial reporting accuracy_
- **Close cutoff enforced:** No ledger entry may be posted after cutoff without controller-approved reopening. _Rationale: SOX control design_
- **Evidence retention:** Evidence must remain retrievable for 7 years. _Rationale: Audit and legal retention_
- **Human final approval:** Controller and CFO approvals must be human-confirmed. _Rationale: Accountability_
- **Preparer/approver separation:** The preparer may not be the final approver. _Rationale: Segregation of duties_

### Procedure

01. **Freeze transaction entry** — Lock subledgers and prevent late postings.
   - Owner: Close Automation Operator
   - Expected duration: 5 minutes
   - Tools: ERP General Ledger
   - Control evidence: The ERP reports the close period locked.
02. **Extract trial balance and feed timestamps** — Extract the trial balance and source-system timestamps.
   - Owner: Close Automation Operator
   - Expected duration: 10 minutes
   - Tools: ERP General Ledger
   - Control evidence: All source feeds are no more than 15 minutes old.
   - Exception: ERP source unavailable → Use a digitally signed, timestamped snapshot and open a blocking incident; do not silently substitute a spreadsheet.
03. **Reconcile material accounts** — Reconcile material accounts and attach evidence.
   - Owner: Finance Analyst
   - Expected duration: 120 minutes
   - Tools: Close Workpaper Workbook
   - Control evidence: No unexplained material variance remains.
04. **Classify and investigate variances** — Use AI triage, then apply human situated judgment to material or anomalous variances.
   - Owner: Variance Review Agent
   - Expected duration: 90 minutes
   - Tools: Variance AI Console; Close Workpaper Workbook
05. **Controller review** — Independently review reconciliations, evidence, and unresolved items.
   - Owner: Corporate Controller
   - Expected duration: 60 minutes
   - Control evidence: Controller approval record exists.
06. **CFO approval** — Approve the close package or return it with required corrections.
   - Owner: Chief Financial Officer
   - Expected duration: 30 minutes
   - Control evidence: CFO approval record exists.
   - Exception: CFO unavailable during close → Delegate approval to the formally designated Acting CFO; never to the preparer.
07. **Post close and publish package** — Post approved entries and publish the reporting package.
   - Owner: Close Automation Operator
   - Expected duration: 20 minutes
08. **Archive evidence and hold retrospective** — Archive evidence and capture questions, issues, and lessons learned.
   - Owner: Procedural Knowledge Steward
   - Expected duration: 45 minutes

### Evidence, audit, and change control

- Quarter-End Close SOP PDF — `doc://finance/close-sop-v1.0`
- ERP Ledger API — `api://erp/general-ledger/v3`
- Close Control Checklist — `doc://finance/close-checklist-v1.1`
- Change request **Add source timestamp as blocking verification**: Approved

## Vendor Payment Approval — version 1.0.0

**Control objective:** Approve valid vendor payments while preventing duplicate, unauthorized, or self-approved disbursements.

### Roles and segregation of duties

- **Accounts Payable Specialist** — Validate invoice identity, purchase order, receipt, and payment details.; current filler: Maria Chen (Human).
- **Payment Approver** — Independently approve or reject the payment batch.; current filler: Devon Okafor (Human).

### Required controls

- **Unique invoice identifier:** The vendor invoice number must be present and unique for the vendor. _Rationale: Prevents duplicate payment._
- **Three-way match:** Invoice, purchase order, and receipt must agree within the approved tolerance. _Rationale: Validates the liability._
- **Independent approval:** The payment approver must not be the preparer. _Rationale: Segregation of duties._

### Procedure

01. **Capture invoice** — Capture the vendor invoice and link it to the vendor master record.
   - Owner: Accounts Payable Specialist
   - Expected duration: 10 minutes
02. **Perform three-way match** — Match invoice, purchase order, and receipt; document any exception.
   - Owner: Accounts Payable Specialist
   - Expected duration: 20 minutes
   - Exception: Receipt is unavailable for an approved service invoice → Require a documented service-owner confirmation before approval.
03. **Approve payment** — Independently approve the payment batch after reviewing evidence and exceptions.
   - Owner: Payment Approver
   - Expected duration: 15 minutes

### Evidence, audit, and change control
