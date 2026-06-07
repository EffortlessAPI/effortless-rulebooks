#!/usr/bin/env python3
"""
loop-02-business-rules.py  (idempotent)

Loop 2: build out the BusinessRules library so EVERY RuleRef emitted by Loop 1
resolves to a real BusinessRules row, and the whole traffic-ticket domain is
covered by a precise, numbered rule library (the TT-* scheme).

- Adds new BusinessRuleCategories for the rule families Loop 1 references
  (rbac, navigation, auth, licensing, rules-engine, platform) on top of the
  existing 8 domain categories.
- Adds a BusinessRules row for every TT-* code (precise Description +
  SchemaLocation). Existing R# rows are left untouched (sacred).
- Merge-by-id: re-running = 0 changes.

Gate: every distinct RuleRefs token across ERBFeatures resolves to a
BusinessRules.RuleCode.

Run:
  python3 scripts/loop-02-business-rules.py --dry-run
  python3 scripts/loop-02-business-rules.py
"""
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
TTC_ROOT = os.path.dirname(HERE)
RB_PATH = os.path.join(TTC_ROOT, "effortless-rulebook",
                       "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T00:00:00Z"
WHO = "admin-example"


def audit(row):
    row.setdefault("CreatedAt", NOW)
    row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW)
    row.setdefault("ModifiedBy", WHO)
    return row


# New categories (id, Title, Description, SortOrder) -- added after existing 8.
NEW_CATEGORIES = [
    ("rules-engine", "Rules Engine", "Multi-jurisdiction regulation as data: deadlines, fines, penalties, and thresholds are rows, not constants.", 9),
    ("rbac", "Access Control & RBAC", "Who may create/read/update/delete which table and field, and which rows each role may see.", 10),
    ("navigation", "Navigation & Routing", "How the route tree, sidebar, and per-route access are derived from PlatformNaviation rows.", 11),
    ("auth", "Identity & Auth", "Passwordless magic-link sign-in and per-request identity resolution to an AppUsers role.", 12),
    ("licensing", "Packages & Licensing", "How packages, features, tables, and routes are gated by license/active flags.", 13),
    ("platform", "Platform & Build", "Self-description, the rulebook-first build pipeline, and the SSoT drift guard.", 14),
    ("glossary", "Glossary & Vocabulary", "The canonical domain vocabulary that features and rules are written against.", 15),
]

# Every rule: RuleCode -> (Title, Category, Description, SchemaLocation)
RULES = {
    # ---- intake / deadlines ----
    "TT-INTAKE-01": ("Citation requires driver, violation, jurisdiction", "intake",
        "A citation must reference exactly one Driver, one ViolationType, and one issuing Jurisdiction; none may be null at intake.",
        "Citations.Driver / Citations.ViolationType / Citations.Jurisdiction"),
    "TT-INTAKE-02": ("Issue date present and not future-dated", "intake",
        "Every citation records an IssueDate that is on or before today; a future issue date is rejected at intake.",
        "Citations.IssueDate"),
    "TT-INTAKE-03": ("Driver response is pay-or-contest before the deadline", "intake",
        "The driver's response is one of {pay, contest} and must be recorded on or before the computed ResponseDeadline.",
        "Citations.DriverResponse / Citations.ResponseDeadline"),
    "TT-DEADLINE-01": ("Response deadline derives from jurisdiction DaysToRespond", "rules-engine",
        "ResponseDeadline = IssueDate + the issuing jurisdiction's DaysToRespond; it recomputes whenever the rule changes.",
        "Citations.ResponseDeadline <- Jurisdictions.DaysToRespond"),
    # ---- citation lifecycle ----
    "TT-CITATION-01": ("Citation status is derived, never stored", "citation-lifecycle",
        "CitationStatus (Issued -> Responded -> InContest -> Adjudicated -> Closed) is computed on read from facts and child CaseEvents; no stored status column is authoritative.",
        "Citations.CitationStatus"),
    "TT-CITATION-02": ("Status order is monotonic", "citation-lifecycle",
        "A citation may only advance forward through its lifecycle states; backward transitions require an explicit, logged override.",
        "Citations.CitationStatus / CaseEvents"),
    # ---- contest / hearings ----
    "TT-CONTEST-01": ("Contesting requires a timely contest response", "contest",
        "The contest track only opens when DriverResponse = contest was recorded before the ResponseDeadline.",
        "Citations.DriverResponse / Citations.ContestStatus"),
    "TT-CONTEST-02": ("Contest track derives from the latest hearing", "contest",
        "ContestStatus (NotContested | HearingRequested -> Scheduled -> Heard) is derived from the most recent Hearings row for the citation.",
        "Citations.ContestStatus <- Hearings"),
    "TT-CONTEST-03": ("Determination decides liability", "contest",
        "A hearing outcome of dismissed sets liability to none and zero fine owed; upheld confirms the original fine and points.",
        "Hearings.Outcome -> Citations.AmountOwed / Citations.PointsAssessed"),
    "TT-HEARING-01": ("Hearing requires a requested contest", "contest",
        "A Hearings row may only be created for a citation whose ContestStatus is HearingRequested or later.",
        "Hearings.Citation / Citations.ContestStatus"),
    "TT-HEARING-02": ("Scheduling is manager-or-admin only", "contest",
        "Only manager or admin roles may set a hearing's ScheduledDate; representatives request, they do not schedule.",
        "Hearings.ScheduledDate / Roles"),
    # ---- payment / penalties ----
    "TT-PAYMENT-01": ("Payment track derives from payments and pay-by date", "payment",
        "PaymentStatus (Pending -> Due -> Late -> Collections | Paid | NotOwed) is computed from Payments rows and the jurisdiction pay-by date.",
        "Citations.PaymentStatus <- Payments / Jurisdictions.DaysToPay"),
    "TT-PAYMENT-02": ("Late penalty applies after the pay-by date", "payment",
        "Once the pay-by date passes with no full payment, the jurisdiction's LatePenaltyPercent is added to AmountOwed.",
        "Citations.AmountOwed <- Jurisdictions.LatePenaltyPercent"),
    "TT-PAYMENT-03": ("Paid in full closes the case", "payment",
        "When recorded Payments equal AmountOwed, PaymentStatus becomes Paid and the citation advances toward Closed.",
        "Payments.Amount / Citations.PaymentStatus"),
    "TT-FEE-01": ("Base fine comes from the violation and jurisdiction", "rules-engine",
        "The base fine is the ViolationType base amount adjusted by the jurisdiction's fee schedule; it is data, not a constant.",
        "Citations.AmountOwed <- ViolationTypes.BaseFine / Jurisdictions"),
    # ---- license points ----
    "TT-LICENSE-01": ("Active points roll up across a driver's citations", "license",
        "A driver's ActivePoints is the sum of points from non-dismissed, non-expired citations under each jurisdiction's point window.",
        "Drivers.ActivePoints <- Citations.PointsAssessed"),
    "TT-LICENSE-02": ("License track follows jurisdiction thresholds", "license",
        "LicenseStatus (Valid -> Warning -> Suspended) is derived by comparing ActivePoints against the jurisdiction's warning and suspension thresholds.",
        "Drivers.LicenseStatus <- Jurisdictions.WarningThreshold / SuspensionThreshold"),
    # ---- work queue ----
    "TT-QUEUE-01": ("Work queue is computed from deadlines and status", "workqueue",
        "WorkQueueItems are derived from overdue responses, due/late payments, and upcoming hearings, bucketed by urgency; nothing is hand-enqueued.",
        "WorkQueueItems <- Citations / Hearings / Payments"),
    # ---- audit ----
    "TT-AUDIT-01": ("Every state-affecting change is logged", "audit",
        "Each status change, override, payment, and edit writes an append-only AuditLogEntries (and where lifecycle-relevant, a CaseEvents) row.",
        "AuditLogEntries / CaseEvents"),
    "TT-AUDIT-02": ("The log never contradicts the derived state", "audit",
        "The append-only history explains how current computed state was reached; it is additive narration, never an alternate source of truth.",
        "AuditLogEntries / Citations.CitationStatus"),
    # ---- assistant ----
    "TT-ASSISTANT-01": ("Assistant answers are grounded in live data", "assistant",
        "The assistant answers about a citation/rule/portal from live vw_* reads and the rulebook, recording each exchange as an AssistantTurns row.",
        "AssistantTurns / vw_citations"),
    "TT-ASSISTANT-02": ("Every turn is cost-accounted", "assistant",
        "Each AssistantTurns row records input/output tokens and a computed cost via ModelPricingVersions for the turn's AiModel.",
        "AssistantTurns.InputTokens / OutputTokens / Cost <- ModelPricingVersions"),
    # ---- rules-engine ----
    "TT-RULES-01": ("Jurisdiction knobs are editable data", "rules-engine",
        "Deadlines, fines, penalty percentages, and point thresholds live in Jurisdictions/JurisdictionRules and every dependent value re-derives on edit.",
        "Jurisdictions / JurisdictionRules"),
    "TT-RULES-02": ("Each jurisdiction rule cites its source", "rules-engine",
        "A JurisdictionRules row links to the JurisdictionSourceDocuments / ReferenceDocuments authority that justifies it.",
        "JurisdictionRules -> JurisdictionSourceDocuments / ReferenceDocuments"),
    # ---- rbac ----
    "TT-RBAC-01": ("Four canonical roles govern access", "rbac",
        "Exactly four roles exist (admin, manager, representative, external-llm) and every permission decision references one of them.",
        "Roles"),
    "TT-RBAC-02": ("CRUD is per role, per table, per field", "rbac",
        "What each role may create/read/update/delete is data at the table and field grain; the enforced API consults it on every operation.",
        "Roles / ERBTables.*CRUD / ERBFields"),
    "TT-RBAC-03": ("Row access follows per-role WHERE policies", "rbac",
        "Each role-table pair carries a WHERE policy; a representative sees only their own citations while admin sees all, enforced on every vw_* read.",
        "ERBTables / vw_citations (row-level security)"),
    "TT-RBAC-04": ("Lower-trust roles get redacted projections", "rbac",
        "Fields denied to a role (e.g. PII for external-llm) are stripped from responses at projection time, not nulled afterward.",
        "ERBFields (field CRUD) / role-specific views"),
    # ---- navigation ----
    "TT-NAV-01": ("Navigation is derived from PlatformNaviation rows", "navigation",
        "The sidebar and route tree render from PlatformNaviation filtered by the current role's RoleVisibility; there is no hand-maintained menu.",
        "PlatformNaviation.RoleVisibility / ParentRouteKey"),
    "TT-NAV-02": ("Route order and pinning are data", "navigation",
        "NavLevel, SortOrder, and PinToTop on PlatformNaviation determine ordering and which screens float to the top.",
        "PlatformNaviation.NavLevel / SortOrder / PinToTop"),
    # ---- auth ----
    "TT-AUTH-01": ("Identity is resolved per request from the magic-link session", "auth",
        "Each request resolves to an AppUsers row and role via the MagicLinkConfig flow; identity feeds every RBAC and scoping decision.",
        "AppUsers / MagicLinkConfig"),
    # ---- licensing ----
    "TT-LICENSE-PKG-01": ("Surfaces are gated by license/active flags", "licensing",
        "Packages, features, tables, and routes carry IsLicensed/IsActive flags; unlicensed surfaces are revealed as upgrade prompts, not deleted.",
        "ERBPackages.IsLicensed / ERBFeatures.IsLicensed / ERBTables.IsLicensed / PlatformNaviation.IsLicensed"),
    # ---- platform ----
    "TT-PLATFORM-01": ("The rulebook JSON is the single source of truth", "platform",
        "All substrates (Postgres, Python, explain-dag, rulespeak) are mechanically derived from the rulebook by effortless build; edits go to the rulebook.",
        "traffic-ticket-contest-rulebook.json"),
    "TT-PLATFORM-02": ("The platform describes itself", "platform",
        "ERBTables/ERBFields/ERBFeatures/ERBPackages catalog every table, field, capability, and package so the app can render its own model.",
        "ERBTables / ERBFields / ERBFeatures / ERBPackages"),
    "TT-PLATFORM-03": ("Build is generate + load; drift is guarded", "platform",
        "effortless build regenerates DDL/views/inserts and reloads the DB; a drift guard warns before a rebuild would clobber un-saved DB edits.",
        "postgres-bootstrap/* / build pipeline"),
    # ---- glossary ----
    "TT-GLOSSARY-01": ("Domain terms are defined as data", "glossary",
        "Every domain term used in a feature or rule is defined in GlossaryTerms under a GlossaryCategories group.",
        "GlossaryTerms / GlossaryCategories"),
}


def main():
    rb = json.load(open(RB_PATH))
    cats = rb["BusinessRuleCategories"]["data"]
    rules = rb["BusinessRules"]["data"]

    have_cat = {c["BusinessRuleCategoryId"] for c in cats}
    have_rule = {r["RuleCode"] for r in rules}

    added_cat = added_rule = 0
    for cid, title, desc, order in NEW_CATEGORIES:
        if cid in have_cat:
            continue
        cats.append(audit({"BusinessRuleCategoryId": cid, "Title": title,
                           "Description": desc, "SortOrder": order}))
        have_cat.add(cid)
        added_cat += 1

    # assign SortOrder per code in stable insertion order, offset past R# rules
    base = 100
    for i, (code, (title, cat, desc, loc)) in enumerate(RULES.items()):
        if code in have_rule:
            continue
        rid = "rule-" + code.lower()
        rules.append(audit({
            "BusinessRuleId": rid, "RuleCode": code, "Title": title,
            "Category": cat, "SortOrder": base + i,
            "Description": desc, "SchemaLocation": loc,
        }))
        have_rule.add(code)
        added_rule += 1

    # ---- gate: every ERBFeatures RuleRef resolves ----
    all_codes = {r["RuleCode"] for r in rules}
    cat_ids = {c["BusinessRuleCategoryId"] for c in cats}
    unresolved = set()
    for f in rb["ERBFeatures"]["data"]:
        for tok in (f.get("RuleRefs") or "").split(","):
            tok = tok.strip()
            if tok and tok not in all_codes:
                unresolved.add(tok)
    bad_cat = [(r["RuleCode"], r["Category"]) for r in rules if r["Category"] not in cat_ids]

    print("=== loop-02 business-rules ===")
    print("categories added : +%d (total %d)" % (added_cat, len(cats)))
    print("rules added      : +%d (total %d)" % (added_rule, len(rules)))
    print("--- gate (all 0) ---")
    print("unresolved RuleRefs :", len(unresolved), sorted(unresolved))
    print("rules w/ bad category:", len(bad_cat), bad_cat)

    if unresolved:
        print("\nABORT: some ERBFeatures RuleRefs do not resolve to a BusinessRule.")
        sys.exit(1)
    if bad_cat:
        print("\nABORT: rule(s) reference a missing category.")
        sys.exit(1)

    if DRY:
        print("\n[dry-run] no write")
        return
    with open(RB_PATH, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB_PATH)


if __name__ == "__main__":
    main()
