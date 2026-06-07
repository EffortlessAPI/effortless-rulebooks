#!/usr/bin/env python3
"""
LOOP 4 — Enumerate every non-CRUD action of the traffic-ticket platform as an
APIEndpoints row: contest/payment/hearing/determination/points actions, intake
imports, assistant, reports, and platform-meta actions (SAVE, drift-guard,
licensing toggles). Each carries method, path, type, subject table, triggered
state machine, role visibility, and a domain-correct WHERE policy.

Merge-by-id (APIEndpointId). The existing 9 endpoints are sacred.

  python3 scripts/loop-04-api-endpoints.py --dry-run
  python3 scripts/loop-04-api-endpoints.py
"""
import json, os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T19:00:00Z"
WHO = "admin-example"

A = "admin,manager,representative,external-llm"
AM = "admin,manager"
AMR = "admin,manager,representative"
ADM = "admin"

# Domain-correct row scopes (generic SQL; reference real tables/columns).
REP_OWN = "representative = current_user()"
DRIVER_OWN = "driver = current_driver()"

# id, title, method, path, type, subject, triggers_sm, roles, where, status, desc
ENDPOINTS = [
    # ---- contest-track ----
    ("citation-contest", "Open a contest", "POST", "/api/citations/:id/contest", "action", "Citations", "contest-track", AMR, REP_OWN, "designed",
     "Opens a contest on a Citation, capturing the selected contest grounds and transitioning the contest-track to its initial contested state; writes a CaseEvents entry."),
    ("citation-withdraw-contest", "Withdraw a contest", "POST", "/api/citations/:id/withdraw-contest", "action", "Citations", "contest-track", AMR, REP_OWN, "designed",
     "Withdraws an open contest, returning the Citation to the pay-or-contest decision point and logging the withdrawal."),
    ("hearing-reschedule", "Reschedule a hearing", "POST", "/api/hearings/:id/reschedule", "action", "Hearings", "contest-track", AMR, REP_OWN, "designed",
     "Reschedules a Hearing to a new date/time, checking calendar conflicts and writing the change to CaseEvents."),
    ("hearing-prep", "Mark hearing prepped", "POST", "/api/hearings/:id/prep", "action", "Hearings", "contest-track", AMR, REP_OWN, "designed",
     "Marks a Hearing as prepared (evidence and grounds assembled), advancing the contest-track readiness state."),
    ("determination-issue", "Issue a determination", "POST", "/api/citations/:id/issue-determination", "action", "Citations", "contest-track", AM, None, "designed",
     "Records the Hearing determination (upheld/dismissed/reduced) on the Citation, advancing the citation-lifecycle to adjudicated and triggering fee/point consequences."),
    ("determination-letter-generate", "Generate determination letter", "POST", "/api/citations/:id/determination-letter", "action", "Citations", None, AM, None, "designed",
     "Generates the formal determination letter from the jurisdiction template and attaches it to the Citation."),
    ("citation-dismiss", "Dismiss a citation", "POST", "/api/citations/:id/dismiss", "action", "Citations", "citation-lifecycle", AM, None, "designed",
     "Dismisses a Citation (e.g. successful contest), zeroing the amount due, reversing any assessed points, and closing the lifecycle."),

    # ---- payment-track ----
    ("payment-refund", "Refund a payment", "POST", "/api/payments/:id/refund", "action", "Payments", "payment-track", AM, None, "designed",
     "Reverses a Payment when a paid Citation is later dismissed, restoring make-whole state and removing points; barred from external-llm."),
    ("payment-plan-enroll", "Enroll in a payment plan", "POST", "/api/citations/:id/payment-plan", "action", "Citations", "payment-track", AMR, REP_OWN, "designed",
     "Enrolls an eligible Driver in an installment plan, generating the schedule of Payments and checking JurisdictionRules eligibility."),
    ("payment-plan-installment", "Record an installment", "POST", "/api/payments/:id/installment", "action", "Payments", "payment-track", AMR, None, "designed",
     "Records an installment payment against a plan, advancing the payment-track and accruing a late penalty if past due."),
    ("late-penalty-accrue", "Accrue late penalties", "POST", "/api/jobs/late-penalty-accrue", "action", "Payments", "payment-track", ADM, None, "designed",
     "Scheduled job that accrues late penalties on overdue Payments per JurisdictionRules and flags them in the work queue."),

    # ---- license-track / points ----
    ("points-assess", "Assess license points", "POST", "/api/citations/:id/assess-points", "action", "Citations", "license-track", AM, None, "designed",
     "Assesses license points for an adjudicated Citation per the jurisdiction point schedule and rolls them up to the Driver total."),
    ("points-recompute", "Recompute driver points", "POST", "/api/drivers/:id/recompute-points", "action", "Drivers", "license-track", AM, None, "designed",
     "Recomputes a Driver's total license points across all adjudicated Citations, used after a dismissal reverses points."),
    ("license-suspension-check", "Check suspension threshold", "GET", "/api/drivers/:id/suspension-check", "report", "Drivers", "license-track", AMR, None, "designed",
     "Reports whether a Driver has crossed a jurisdiction suspension threshold based on accumulated points."),

    # ---- intake ----
    ("citation-import", "Bulk import citations", "POST", "/api/admin/citation-import", "action", "Citations", "citation-lifecycle", AM, None, "designed",
     "Validates and imports a batch of Citations from a jurisdiction feed, reporting per-row conflicts before commit."),
    ("citation-ocr", "OCR a citation document", "POST", "/api/citations/:id/ocr", "action", "Citations", None, AMR, REP_OWN, "designed",
     "Runs OCR on an uploaded citation image to extract structured fields, writing cost telemetry for the job."),
    ("driver-lookup", "Driver self-service lookup", "GET", "/api/portal/citation-lookup", "report", "Citations", None, A, DRIVER_OWN, "designed",
     "Public lookup that returns a Driver's own Citation by number and last name for the self-service portal, redaction-scoped."),

    # ---- citation-lifecycle ----
    ("citation-advance-state", "Advance citation state", "POST", "/api/citations/:id/advance-state", "action", "Citations", "citation-lifecycle", AM, None, "designed",
     "Manually advances a Citation to a valid next lifecycle state via the state-machine backbone, with guard enforcement and CaseEvents logging."),

    # ---- assistant / cost ----
    ("ocr-cost-estimate", "Estimate OCR cost", "POST", "/api/assistant/ocr-estimate", "report", "AssistantTurns", None, AMR, None, "shipped",
     "Projects the per-document OCR cost from page count and current model pricing before a job runs."),
    ("cost-rollup", "Cost rollup report", "GET", "/api/reports/cost-rollup", "report", "AssistantTurns", None, AM, None, "shipped",
     "Aggregates assistant and OCR spend by model, role, jurisdiction, and day for the cost dashboard."),

    # ---- reports ----
    ("deadline-report", "Deadline report", "GET", "/api/reports/deadlines", "report", "Citations", None, AMR, REP_OWN, "designed",
     "Lists every Citation, contest, and Payment approaching or past its deadline, sorted by urgency."),
    ("case-timeline-report", "Case timeline", "GET", "/api/citations/:id/timeline", "report", "CaseEvents", None, AMR, REP_OWN, "designed",
     "Returns the chronological CaseEvents history for a Citation for the timeline view."),

    # ---- platform-meta actions ----
    ("config-save", "SAVE to rulebook", "POST", "/api/admin/save", "action", "ERBCustomizations", None, ADM, None, "shipped",
     "Captures live config-table edits back into the rulebook JSON and triggers a rebuild; surfaces a diff before writing."),
    ("config-drift-check", "Drift check", "GET", "/api/admin/drift", "report", "ERBCustomizations", None, ADM, None, "shipped",
     "Reports rows where live DB config has diverged from the rulebook, the interlock on the SAVE/rebuild cycle."),
    ("package-toggle", "Toggle a package", "POST", "/api/admin/packages/:id/toggle", "action", "ERBPackages", None, ADM, None, "shipped",
     "Enables or disables a feature package, adding or removing its screens, tables, and endpoints from the platform."),
    ("feature-toggle-license", "Toggle feature license", "POST", "/api/admin/features/:id/license", "action", "ERBFeatures", None, ADM, None, "shipped",
     "Toggles entitlement of an individual feature, honored by gating, the sidebar, and the unlicensed-reveal surface."),
    ("impersonate-role", "Impersonate a role", "POST", "/api/admin/impersonate", "action", "Roles", None, ADM, None, "shipped",
     "Starts a view-as-role session so an admin sees the exact rows, fields, and redactions a chosen role experiences."),
]


def audit(row):
    row.setdefault("CreatedAt", NOW); row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW); row.setdefault("ModifiedBy", WHO)
    return row


def main():
    rb = json.load(open(RB))
    eps = rb["APIEndpoints"]["data"]
    tables = {k for k, v in rb.items() if isinstance(v, dict) and "schema" in v}
    machines = {s.get("StateMachineId") or s.get("Name") for s in rb["StateMachines"]["data"]}
    have = {e["APIEndpointId"] for e in eps}
    added = 0
    for (eid, title, method, path, etype, subj, sm, roles, where, status, desc) in ENDPOINTS:
        if eid in have:
            continue
        eps.append(audit({
            "APIEndpointId": eid, "Title": title, "Description": desc,
            "HttpMethod": method, "Path": path, "EndpointType": etype,
            "SubjectTableName": subj, "RoleVisibility": roles,
            "WhereClause": where, "TriggersStateMachine": sm, "Status": status,
        }))
        have.add(eid); added += 1

    bad_subj = [e["APIEndpointId"] for e in eps if e.get("SubjectTableName") and e["SubjectTableName"] not in tables]
    bad_sm = [e["APIEndpointId"] for e in eps if e.get("TriggersStateMachine") and e["TriggersStateMachine"] not in machines]

    print("=== LOOP 4: API endpoints ===")
    print("added:", added, "| total endpoints:", len(eps))
    print("SubjectTableName unresolved:", len(bad_subj), bad_subj[:8])
    print("TriggersStateMachine unresolved:", len(bad_sm), bad_sm[:8])
    # action coverage by lifecycle
    by_sm = {}
    for e in eps:
        by_sm.setdefault(e.get("TriggersStateMachine") or "(none)", 0)
        by_sm[e.get("TriggersStateMachine") or "(none)"] += 1
    print("endpoints per machine:", by_sm)
    if bad_subj or bad_sm:
        print("\nABORT: endpoint FK gaps."); sys.exit(1)
    if DRY:
        print("\n[dry-run] no write"); return
    with open(RB, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB)


if __name__ == "__main__":
    main()
