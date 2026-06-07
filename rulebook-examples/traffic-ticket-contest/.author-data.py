#!/usr/bin/env python3
"""
Populate the 20 empty tables of traffic-ticket-contest-rulebook.json with a
coherent, story-driven dataset anchored on the existing 6 citations / 3 drivers
/ 3 jurisdictions (as-of 2026-06-06).

Usage:
  python3 .author-data.py --dry-run     # build rows, print counts, validate FKs, no write
  python3 .author-data.py               # also inject into the rulebook JSON
  python3 .author-data.py --images      # also (re)generate DALL-E feature images

FK convention (verified): a relationship value is the TARGET's `Name` natural key
(lower-kebab) = PK with its type-prefix stripped.
  FK -> Citations      = lower(CitationNumber)   e.g. tc-2026-0002
  FK -> Drivers        = lower(LicenseNumber)    e.g. d1234567
  FK -> Jurisdictions  = lower(Code)             e.g. ca-la
  FK -> ViolationTypes = lower(Code)             e.g. cvc-22350
  FK -> AppUsers       = AppUserId               e.g. representative-example
"""
import json, sys, os

HERE = os.path.dirname(os.path.abspath(__file__))
RB_PATH = os.path.join(HERE, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")

REP, MGR, ADMIN, EXLLM = "representative-example", "manager-example", "admin-example", "external-llm-example"

def audit(by=ADMIN, at="2026-06-06T12:00:00Z", model=None):
    d = {"CreatedAt": at, "CreatedBy": by, "ModifiedAt": at, "ModifiedBy": by}
    if model: d["ModifiedByModel"] = model
    return d

ROWS = {}

# ============================================================================
# SiteBranding
# ============================================================================
ROWS["SiteBranding"] = [{
    "SiteBrandingId": "site-branding",
    "CompanyName": "TicketContest Portal", "ShortName": "TicketContest", "BrandAbbreviation": "TC",
    "SiteTitle": "TicketContest — Fight & Resolve Traffic Citations",
    "MetaDescription": "A case-management portal for responding to, contesting, and resolving traffic citations across multiple jurisdictions — where every deadline, fine, and penalty is a rule, not code.",
    "Tagline": "Every ticket, every deadline, every rule — in one place.",
    "SubTagline": "Four state machines and a multi-jurisdiction rules engine, working for the driver.",
    "WebsiteUrl": "https://ticketcontest.example.com",
    "LogoUrl": "/assets/branding/logo.svg", "FaviconUrl": "/assets/branding/favicon.png",
    "AppleTouchIconUrl": "/assets/branding/apple-touch-icon.png", "OgImageUrl": "/assets/branding/og-image.png",
    "OgType": "website", "OgLocale": "en_US", "TwitterCard": "summary_large_image",
    "PrimaryColor": "#1d4ed8", "SecondaryColor": "#0f172a", "AccentColor": "#f59e0b",
    "TextColor": "#0f172a", "BackgroundColor": "#f8fafc", "TileColor": "#1d4ed8",
    "ContactEmail": "support@ticketcontest.example.com", "ContactPhone": "+1-800-555-0142",
    "AddressLine1": "200 Civic Center Plaza", "City": "Los Angeles", "StateRegion": "CA", "PostalCode": "90012",
    "LinkedInUrl": "https://www.linkedin.com/company/ticketcontest", "YouTubeUrl": "https://www.youtube.com/@ticketcontest",
    "Copyright": "© 2026 TicketContest Portal. All rights reserved.",
    **audit(),
}]

# ============================================================================
# ERBVersions
# ============================================================================
BASE_ID = "appEBe2X4jsw5thxT"
ROWS["ERBVersions"] = [
    {"ERBVersionId": "v-1-0-0", "Name": "v1.0.0", "BaseId": BASE_ID, "Version": "1.0.0",
     "Message": "Initial four-state-machine traffic-ticket model.",
     "Notes": "Citation, Contest, Payment, and License tracks expressed as calculated fields on Citations/Drivers. Jurisdictions table holds all regulatory knobs. Seed data exercises every branch across CA-LA, NY-NYC, and TX-AUS.",
     "CommitDate": "2026-05-01", "IsPublished": True, "Author": "ej@ssot.me", **audit(at="2026-05-01T09:00:00Z")},
    {"ERBVersionId": "v-1-1-0", "Name": "v1.1.0", "BaseId": BASE_ID, "Version": "1.1.0",
     "Message": "Add explicit StateMachines / MachineStates / StateTransitionRules catalog.",
     "Notes": "The four computed tracks are now also described declaratively as data: 4 machines, 18 states, 15 transition rules — the substrate the event log and occupancy records build on.",
     "CommitDate": "2026-05-20", "IsPublished": True, "Author": "ej@ssot.me", **audit(at="2026-05-20T09:00:00Z")},
    {"ERBVersionId": "v-1-2-0", "Name": "v1.2.0", "BaseId": BASE_ID, "Version": "1.2.0",
     "Message": "Graft the admin-portal scaffolding onto the demo.",
     "Notes": "Adds the portal layer: PlatformNaviation route tree with per-role CRUD, SiteBranding, ERBTables/ERBFields catalog, ERBFeatures with cartoon imagery, BusinessRules + Glossary library, AuditLogEntries, WorkQueueItems, and AssistantTurns.",
     "CommitDate": "2026-06-06", "IsPublished": True, "Author": "eejai42@gmail.com", **audit(model="gpt-image-1")},
    {"ERBVersionId": "v-1-3-0-draft", "Name": "v1.3.0-draft", "BaseId": BASE_ID, "Version": "1.3.0",
     "Message": "Jurisdiction rules library (appeal-board decisions) — draft.",
     "Notes": "Begins ingesting per-jurisdiction source documents and AI-distilled rule summaries into JurisdictionSourceDocuments / JurisdictionRules / ReferenceDocuments. Not yet published.",
     "CommitDate": "2026-06-06", "IsPublished": False, "Author": "eejai42@gmail.com", **audit()},
]

# ============================================================================
# ERBFeatureCategories
# ============================================================================
ROWS["ERBFeatureCategories"] = [
    {"ERBFeatureCategoryId": "intake", "Title": "Intake & Response", "Icon": "📝", "SortOrder": 1,
     "Description": "Getting a citation into the system and capturing the driver's response (pay or contest) before the deadline.", **audit()},
    {"ERBFeatureCategoryId": "state-machine", "Title": "State Machines", "Icon": "🔁", "SortOrder": 2,
     "Description": "The four computed tracks — Citation, Contest, Payment, License — that derive current status from facts + rules on every read.", **audit()},
    {"ERBFeatureCategoryId": "rules-engine", "Title": "Rules Engine", "Icon": "⚖️", "SortOrder": 3,
     "Description": "Multi-jurisdiction regulations as data: deadlines, fines, penalties, and point thresholds are rows, not hardcoded constants.", **audit()},
    {"ERBFeatureCategoryId": "hearings", "Title": "Hearings & Contests", "Icon": "🏛️", "SortOrder": 4,
     "Description": "Requesting, scheduling, and recording the outcome of a contested-citation hearing.", **audit()},
    {"ERBFeatureCategoryId": "payments", "Title": "Payments & Penalties", "Icon": "💳", "SortOrder": 5,
     "Description": "Paying fines, computing late penalties, and tracking the slide into collections.", **audit()},
    {"ERBFeatureCategoryId": "license", "Title": "License & Points", "Icon": "🪪", "SortOrder": 6,
     "Description": "Rolling up active points to drive the Valid → Warning → Suspended license track.", **audit()},
    {"ERBFeatureCategoryId": "workqueue", "Title": "Work Queue & Deadlines", "Icon": "⏰", "SortOrder": 7,
     "Description": "Surfacing what needs attention now: overdue responses, due payments, and upcoming hearings.", **audit()},
    {"ERBFeatureCategoryId": "audit", "Title": "Audit & History", "Icon": "🧾", "SortOrder": 8,
     "Description": "The append-only event log and per-row audit trail that coexist with the computed current state.", **audit()},
    {"ERBFeatureCategoryId": "assistant", "Title": "AI Assistant", "Icon": "🤖", "SortOrder": 9,
     "Description": "An in-app assistant answering questions about a citation, the rulebook, or the portal — with full token/cost accounting.", **audit()},
    {"ERBFeatureCategoryId": "library", "Title": "Reference Library", "Icon": "📚", "SortOrder": 10,
     "Description": "Glossary, business rules, and per-jurisdiction source documents that explain how the system decides.", **audit()},
]

# ============================================================================
# ERBFeatures  (image-bearing). _img_prompt is stripped before writing.
# ============================================================================
def feat(fid, title, cat, status, order, desc, rulerefs, img):
    return {"ERBFeatureId": fid, "Title": title, "Category": cat, "Status": status,
            "ERBPackage": "citations", "IsLicensed": True, "SortOrder": order,
            "Description": desc, "RuleRefs": rulerefs, "SourceText": desc, "SourceFiles": "README.md",
            "ImageUrl": f"/assets/feature-images/{fid}.png", "_img_prompt": img, **audit()}

ROWS["ERBFeatures"] = [
    feat("citation-intake", "Citation Intake", "intake", "shipped", 1,
         "Record a new traffic citation — driver, violation type, issuing jurisdiction, issue date — and immediately derive its response deadline from the jurisdiction's DaysToRespond rule.", "R1,R2",
         "A friendly cartoon traffic officer handing a paper ticket to a smiling driver leaning out of a car window on a sunny street."),
    feat("driver-response", "Driver Response", "intake", "shipped", 2,
         "Capture the driver's choice — pay now or request a hearing — and advance the citation from Issued to Responded before the response window closes.", "R3",
         "A cartoon person at a laptop choosing between two big buttons labeled PAY and CONTEST, a thought bubble with a scale and a dollar sign."),
    feat("citation-statemachine", "Citation Lifecycle Tracker", "state-machine", "shipped", 3,
         "Show the live Citation status (Issued → Responded → InContest → Adjudicated → Closed) re-derived from facts and child events — never a stored, drifting column.", "R10,R11",
         "A cartoon flowchart of five connected boxes lighting up in sequence like a board-game path, with a glowing token moving along it."),
    feat("contest-track", "Contest Tracker", "state-machine", "shipped", 4,
         "Derive the contest track (NotContested | HearingRequested → Scheduled → Heard) from the latest hearing, driving liability downstream.", "R12",
         "A cartoon gavel and a calendar shaking hands, a small courthouse in the background, flat bright style."),
    feat("payment-track", "Payment & Penalty Tracker", "state-machine", "in-progress", 5,
         "Compute the payment track (Pending → Due → Late → Collections | Paid | NotOwed) including the jurisdiction-specific late penalty once the pay-by date passes.", "R20,R21,R22",
         "A cartoon piggy bank beside a ticket with a ticking clock turning from green to red, coins dropping in."),
    feat("license-points", "License Points Monitor", "license", "in-progress", 6,
         "Roll up a driver's active points and drive the license track (Valid → Warning → Suspended) against each jurisdiction's warning and suspension thresholds.", "R30,R31",
         "A cartoon driver's license card with a points gauge needle swinging from green VALID toward red SUSPENDED."),
    feat("jurisdiction-rules", "Jurisdiction Rules Engine", "rules-engine", "shipped", 7,
         "Edit a jurisdiction's deadlines, fines, penalty percentages, and point thresholds as data — and watch every citation under it re-derive instantly, with no code change and no migration.", "R40",
         "A cartoon control panel covered in labeled knobs and dials (DAYS, FINE, PENALTY), a hand turning one dial while a small three-region map updates."),
    feat("hearing-scheduling", "Hearing Scheduling", "hearings", "shipped", 8,
         "Request a hearing, let a manager schedule it, and record the outcome — driving the contest track and the citation's liability.", "R12,R13",
         "A cartoon courthouse calendar with a hearing appointment pinned, a tiny friendly judge waving, sticky notes around it."),
    feat("payment-checkout", "Online Payment", "payments", "shipped", 9,
         "Pay a citation's fine online in full, record the payment, and close the case — the path Aisha Khan took on citation TC-2026-0005.", "R20,R23",
         "A cartoon hand tapping a credit card on a phone, a big green checkmark and a PAID stamp over a traffic ticket."),
    feat("work-queue", "Work Queue", "workqueue", "shipped", 10,
         "A single prioritized list of everything needing attention now — overdue responses, due payments, upcoming hearings — bucketed by urgency.", "R50,R51",
         "A cartoon to-do list on a clipboard with color-coded urgent/due/upcoming tags and a red alarm clock at the top."),
    feat("audit-trail", "Audit Trail", "audit", "shipped", 11,
         "Every status change, override, and edit on a citation is logged append-only — a full history that sits alongside, and explains, the computed current state.", "R60",
         "A cartoon magnifying glass over a long receipt-style timeline with stamped entries, a friendly detective vibe, flat style."),
    feat("ai-assistant", "AI Case Assistant", "assistant", "in-progress", 12,
         "Ask plain-language questions about a citation, the rules, or the portal and get an answer — with input/output token usage and per-model cost tracked for every turn.", "R70",
         "A cartoon friendly robot wearing a small headset pointing at a speech bubble containing a traffic ticket and a question mark."),
    feat("glossary", "Glossary", "library", "shipped", 13,
         "Plain-language definitions of every term in the domain — citation, jurisdiction, contest, collections, points — grouped by category.", "",
         "A cartoon open dictionary book with a friendly bookmark, a few key words floating out as little labeled tags."),
    feat("business-rules-browser", "Business Rules Browser", "library", "shipped", 14,
         "Browse the full set of business rules (R1..R38) grouped by category, each linked to exactly where in the schema it is enforced.", "",
         "A cartoon rulebook with numbered tabs and a checkmark seal, beside a tiny gear, flat bright colors."),
    feat("jurisdiction-library", "Jurisdiction Document Library", "library", "planned", 15,
         "Browse the source documents (appeal-board decisions, statutes) and AI-distilled rule summaries backing each jurisdiction's behavior.", "",
         "A cartoon filing cabinet with three labeled drawers (CA, NY, TX) open, legal documents and a small robot summarizing one."),
    feat("multi-jurisdiction-dashboard", "Multi-Jurisdiction Dashboard", "rules-engine", "designed", 16,
         "A top-level dashboard comparing the three jurisdictions side by side — response windows, fines, penalties, point thresholds — so the rules engine is visible at a glance.", "R40",
         "A cartoon dashboard with three side-by-side region cards (LA, NYC, AUSTIN) each showing little dials and a map pin, flat infographic style."),
]

# ============================================================================
# BusinessRuleCategories  +  BusinessRules (R1..R38, grouped)
# ============================================================================
ROWS["BusinessRuleCategories"] = [
    {"BusinessRuleCategoryId": "intake", "Title": "Intake & Deadlines", "SortOrder": 1,
     "Description": "How a citation enters the system and how its response deadline is derived.", **audit()},
    {"BusinessRuleCategoryId": "citation-lifecycle", "Title": "Citation Lifecycle", "SortOrder": 2,
     "Description": "Rules governing the Issued → Responded → InContest → Adjudicated → Closed track.", **audit()},
    {"BusinessRuleCategoryId": "contest", "Title": "Contest & Hearings", "SortOrder": 3,
     "Description": "Rules for requesting, scheduling, and resolving a hearing.", **audit()},
    {"BusinessRuleCategoryId": "payment", "Title": "Payment & Penalties", "SortOrder": 4,
     "Description": "Rules for fines, late penalties, and collections.", **audit()},
    {"BusinessRuleCategoryId": "license", "Title": "License Points", "SortOrder": 5,
     "Description": "Rules rolling points up to warning and suspension thresholds.", **audit()},
    {"BusinessRuleCategoryId": "workqueue", "Title": "Work Queue & Urgency", "SortOrder": 6,
     "Description": "Rules that bucket and prioritize outstanding work.", **audit()},
    {"BusinessRuleCategoryId": "audit", "Title": "Audit & Provenance", "SortOrder": 7,
     "Description": "Rules requiring an immutable trail and override justification.", **audit()},
    {"BusinessRuleCategoryId": "assistant", "Title": "AI Assistant & Cost", "SortOrder": 8,
     "Description": "Rules for assistant turns and their token/cost accounting.", **audit()},
]

def rule(code, title, cat, order, desc, loc):
    return {"BusinessRuleId": "rule-" + code.lower(), "RuleCode": code, "Title": title,
            "Category": cat, "SortOrder": order, "Description": desc, "SchemaLocation": loc, **audit()}

ROWS["BusinessRules"] = [
    rule("R1", "Citation requires driver, violation and jurisdiction", "intake", 1,
         "A citation must reference exactly one driver, one violation type, and one issuing jurisdiction.", "Citations.Driver / Citations.ViolationType / Citations.Jurisdiction"),
    rule("R2", "Response due date is derived from the jurisdiction", "intake", 2,
         "ResponseDueDate = IssuedOn + the issuing jurisdiction's DaysToRespond. Changing the jurisdiction's rule re-derives every citation under it.", "Citations.ResponseDueDate (formula) ← Jurisdictions.DaysToRespond"),
    rule("R3", "A response is pay or contest", "intake", 3,
         "Responding means either paying the fine or requesting a hearing; either sets RespondedOn and advances the citation to Responded.", "Citations.RespondedOn / Citations.ContestRequested"),
    rule("R10", "Citation status is computed, never stored", "citation-lifecycle", 1,
         "CitationStatus is re-derived on every read from raw facts and child events; there is no stored status column to drift.", "Citations.CitationStatus (formula)"),
    rule("R11", "No response by deadline is a default judgment", "citation-lifecycle", 2,
         "If the response window passes with no RespondedOn, the citation moves to Adjudicated by default.", "Citations.CitationStatus (formula) ← Citations.IsResponseOverdue"),
    rule("R12", "Contest status follows the latest hearing", "contest", 1,
         "ContestStatus is derived from the newest related hearing: NotContested | HearingRequested → Scheduled → Heard.", "Citations.ContestStatus (formula) ← Hearings"),
    rule("R13", "Only a manager schedules a hearing", "contest", 2,
         "Transitioning a hearing from HearingRequested to Scheduled is a manager-role action.", "StateTransitionRules.contest-track--hearingrequested->scheduled"),
    rule("R20", "Fine is the violation's base fine", "payment", 1,
         "The amount owed starts at the violation type's BaseFineUsd for the citation's jurisdiction.", "Citations.AmountOwed ← ViolationTypes.BaseFineUsd"),
    rule("R21", "Late payment incurs a jurisdiction penalty", "payment", 2,
         "If payment is not made by the pay-by date, a penalty of LatePenaltyPct of the fine is added.", "Citations.PaymentStatus / Penalty ← Jurisdictions.LatePenaltyPct"),
    rule("R22", "Prolonged non-payment goes to collections", "payment", 3,
         "A Late citation becomes Collections after DaysLateToCollections days past the pay-by date.", "Citations.PaymentStatus (formula) ← Jurisdictions.DaysLateToCollections"),
    rule("R23", "A dismissed citation owes nothing", "payment", 4,
         "If a hearing dismisses the citation, PaymentStatus is NotOwed regardless of the fine.", "Citations.PaymentStatus (formula) ← Hearings.Outcome='Dismissed'"),
    rule("R30", "Active points roll up to the driver", "license", 1,
         "A driver's ActivePoints is the sum of points from adjudicated, non-dismissed citations.", "Drivers.ActivePoints (aggregation)"),
    rule("R31", "Points drive license warning then suspension", "license", 2,
         "LicenseStatus is Warning at the jurisdiction's PointWarningThreshold and Suspended at PointSuspensionThreshold.", "Drivers.LicenseStatus (formula) ← Jurisdictions thresholds"),
    rule("R40", "Regulations are data, not code", "license", 3,
         "Every regulatory knob — deadlines, fines, penalties, thresholds, school caps — is a column on Jurisdictions, editable without code changes.", "Jurisdictions.*"),
    rule("R50", "Overdue work is urgent", "workqueue", 1,
         "A work-queue item whose due date is in the past is urgent and sorts to the top.", "WorkQueueItems.UrgencyBucket (formula)"),
    rule("R51", "Work items mirror their subject's deadline", "workqueue", 2,
         "A work item's DueDate is carried from its subject (e.g. Citations.ResponseDueDate or PaymentDueDate).", "WorkQueueItems.DueDate"),
    rule("R60", "Overrides require a reason", "audit", 1,
         "Any OVERRIDE audit entry must carry a non-empty Reason; the trail is append-only.", "AuditLogEntries.ActionType='OVERRIDE' / AuditLogEntries.Reason"),
    rule("R70", "Every assistant turn is costed", "assistant", 1,
         "Each assistant turn records input/output tokens and the exact pricing version used, so TotalCost is fully derivable.", "AssistantTurns.TotalCost (formula) ← ModelPricingVersions"),
]

# ============================================================================
# GlossaryCategories + GlossaryTerms
# ============================================================================
ROWS["GlossaryCategories"] = [
    {"GlossaryCategoryId": "people", "Title": "People & Roles", "SortOrder": 1,
     "Description": "The actors in the system — drivers, representatives, managers.", **audit()},
    {"GlossaryCategoryId": "lifecycle", "Title": "Lifecycle & States", "SortOrder": 2,
     "Description": "Terms for the four computed status tracks and their states.", **audit()},
    {"GlossaryCategoryId": "legal", "Title": "Legal & Jurisdiction", "SortOrder": 3,
     "Description": "Terms for citations, jurisdictions, hearings, and the rules engine.", **audit()},
    {"GlossaryCategoryId": "money", "Title": "Money & Penalties", "SortOrder": 4,
     "Description": "Terms for fines, payments, penalties, and collections.", **audit()},
]

def term(t, cat, order, definition):
    return {"GlossaryTermId": "term-" + t.lower().replace(" ", "-"), "Term": t,
            "Category": cat, "SortOrder": order, "Definition": definition, **audit()}

ROWS["GlossaryTerms"] = [
    term("Driver", "people", 1, "The cited party — the person who received the traffic citation and whose license points roll up across their citations."),
    term("Representative", "people", 2, "A line agent who owns cases and completes domain workflows on behalf of, or alongside, the driver."),
    term("Manager", "people", 3, "A supervisor who schedules hearings and reviews/approves submissions."),
    term("Citation", "legal", 1, "A traffic ticket: one driver, one violation type, one issuing jurisdiction, an issue date, and the case hub from which all four status tracks are derived."),
    term("Jurisdiction", "legal", 2, "The regulatory authority (county or city) that issued a citation. Its row holds every deadline, fine, penalty, and point threshold that governs cases under it."),
    term("Hearing", "legal", 3, "A contest event: requested by the driver, scheduled by a manager, and concluded with an outcome (e.g. Dismissed) that drives liability."),
    term("Rules Engine", "legal", 4, "The pattern of expressing regulations as data on the Jurisdictions table, so changing a value re-derives every dependent citation with no code change."),
    term("Citation Status", "lifecycle", 1, "The top-level computed track: Issued → Responded → InContest → Adjudicated → Closed."),
    term("Contest Status", "lifecycle", 2, "The hearing track: NotContested | HearingRequested → Scheduled → Heard."),
    term("Payment Status", "lifecycle", 3, "The money track: Pending → Due → Late → Collections | Paid | NotOwed."),
    term("License Status", "lifecycle", 4, "The driver track: Valid → Warning → Suspended, driven by active points against jurisdiction thresholds."),
    term("Default Judgment", "lifecycle", 5, "What happens when a citation's response window passes with no response — it is adjudicated against the driver automatically."),
    term("Fine", "money", 1, "The base monetary penalty for a violation, taken from the violation type's BaseFineUsd."),
    term("Late Penalty", "money", 2, "An additional charge of LatePenaltyPct of the fine, applied when payment misses the pay-by date."),
    term("Collections", "money", 3, "The terminal payment state reached when a citation remains unpaid beyond the jurisdiction's DaysLateToCollections window."),
    term("Points", "money", 4, "Demerit points attached to a violation type; they accumulate on the driver and can trigger a license warning or suspension."),
]

# ============================================================================
# JurisdictionSourceDocuments  (FK Jurisdiction -> lower(Code): ca-la / ny-nyc / tx-aus)
# ============================================================================
def jdoc(jid, j, title, dtype, fname, url, text, notes):
    return {"JurisdictionSourceDocumentId": jid, "Title": title, "Jurisdiction": j,
            "DocType": dtype, "RelativeFilePath": f"git-ignored-examples/jurisdiction-rules/{fname}",
            "SourceUrl": url, "ExtractedText": text, "Notes": notes, **audit()}

ROWS["JurisdictionSourceDocuments"] = [
    jdoc("doc-ca-vc-deadlines", "ca-la", "California Vehicle Code — Arraignment & Bail Deadlines", "statute",
         "ca-vc-arraignment.pdf", "https://leginfo.legislature.ca.gov/faces/codes_displayText.xhtml?lawCode=VEH",
         "A defendant cited for an infraction must appear or pay bail within the time set by the court, generally within 21 days of the notice to appear. Failure to appear may result in a civil assessment and referral for collection.",
         "Source for CA-LA DaysToRespond = 21 and the default-judgment behavior."),
    jdoc("doc-ca-traffic-school", "ca-la", "Los Angeles County Traffic School Eligibility Guidance", "guidance",
         "ca-la-traffic-school.pdf", "https://www.courts.ca.gov/traffic.htm",
         "Eligible drivers may attend traffic school once every 18 months to mask one point from a qualifying moving violation. Commercial license holders and certain violations are excluded.",
         "Source for CA-LA TrafficSchoolPointCap = 1."),
    jdoc("doc-ny-vtl-points", "ny-nyc", "New York Vehicle & Traffic Law — Point System", "statute",
         "ny-vtl-points.pdf", "https://dmv.ny.gov/tickets/about-nys-driver-point-system",
         "The NYS driver point system assigns points by violation. Accumulating 11 points within 18 months may result in license suspension; the DMV may also act on fewer points for serious offenses.",
         "Source for NY-NYC PointSuspensionThreshold = 11 and PointWarningThreshold = 6."),
    jdoc("doc-tx-transportation-code", "tx-aus", "Texas Transportation Code — Speed & Signal Offenses", "statute",
         "tx-transportation-code.pdf", "https://statutes.capitol.texas.gov/Docs/TN/htm/TN.545.htm",
         "An operator shall comply with posted speed limits and traffic-control signals. A defendant must respond to a citation within the period stated on the notice, commonly 14 days, or a warrant and additional fees may issue.",
         "Source for TX-AUS DaysToRespond = 14 and signal/speed violation definitions."),
    jdoc("doc-tx-aus-appeal-2025-114", "tx-aus", "Travis County Appeal Board Decision 2025-114 (Radar Calibration)", "appeal-board-decision",
         "tx-aus-appeal-2025-114.pdf", "https://www.traviscountytx.gov/justice-of-the-peace",
         "Where the issuing agency cannot produce radar/lidar calibration records upon timely request, the speed measurement is unreliable and the citation must be dismissed. Calibration logs must be retained and produced at hearing.",
         "Directly supports the dismissal of citation TC-2026-0003 (Carlos Mendez) at hearing on 2026-04-15."),
]

# ============================================================================
# JurisdictionRules  (extracted from the docs above)
#   SourceDocument FK -> JurisdictionSourceDocuments.Name = lower-kebab(Title).
#   We resolve a doc id to that computed Name so the FK stays self-consistent.
# ============================================================================
def doc_name(doc_id):
    title = next(d["Title"] for d in ROWS["JurisdictionSourceDocuments"]
                 if d["JurisdictionSourceDocumentId"] == doc_id)
    return title.lower().replace(" ", "-")

def jrule(rid, num, title, jur, cat, eff, desc, ai, model, src_doc_id, url):
    return {"JurisdictionRuleId": rid, "RuleNumber": num, "Title": title, "Jurisdiction": jur,
            "Category": cat, "EffectiveDate": eff, "Description": desc,
            "AiVersion": ai, "AiVersionUpdatedAt": "2026-06-06T12:00:00Z", "AiVersionModel": model,
            "SourceDocument": doc_name(src_doc_id), "CitationURL": url, **audit(model=model)}

ROWS["JurisdictionRules"] = [
    jrule("jr-ca-respond-21", "CA-VC-§40519", "21-Day Response Window", "ca-la", "deadline", "2026-01-01",
          "An infraction defendant must appear or post bail within 21 days of the notice to appear; otherwise a civil assessment and collections referral may follow.",
          "In LA County you have 21 days from your ticket date to pay or ask for a hearing. Miss it and the court can decide against you and add a collection fee.",
          "gpt-4o", "doc-ca-vc-deadlines", "https://leginfo.legislature.ca.gov/faces/codes_displayText.xhtml?lawCode=VEH"),
    jrule("jr-ca-school-1pt", "CA-LA-TS-1", "Traffic School Masks One Point", "ca-la", "license", "2026-01-01",
          "An eligible driver may attend traffic school once per 18 months to mask one point from a qualifying moving violation.",
          "In LA County you can take traffic school once every 18 months to hide one point from your record.",
          "gpt-4o-mini", "doc-ca-traffic-school", "https://www.courts.ca.gov/traffic.htm"),
    jrule("jr-ny-suspend-11", "NY-VTL-§510", "11-Point Suspension Threshold", "ny-nyc", "license", "2026-01-01",
          "Accumulating 11 points within 18 months may result in license suspension; a warning is appropriate well before that.",
          "In NYC, 11 points in 18 months can get your license suspended — we flag a warning at 6.",
          "gpt-4o", "doc-ny-vtl-points", "https://dmv.ny.gov/tickets/about-nys-driver-point-system"),
    jrule("jr-tx-respond-14", "TX-TC-§543", "14-Day Response Window", "tx-aus", "deadline", "2026-01-01",
          "A defendant must respond to a citation within the period stated on the notice, commonly 14 days, or a warrant and additional fees may issue.",
          "In Travis County you usually have 14 days to respond to a ticket before a warrant and extra fees can be added.",
          "gpt-4o-mini", "doc-tx-transportation-code", "https://statutes.capitol.texas.gov/Docs/TN/htm/TN.545.htm"),
    jrule("jr-tx-radar-dismiss", "TX-AUS-AB-2025-114", "No Calibration Records → Dismissal", "tx-aus", "evidence", "2025-11-01",
          "If the agency cannot produce radar/lidar calibration records on timely request, the speed reading is unreliable and the citation must be dismissed.",
          "In Travis County, if the police can't show their radar was calibrated, a speeding ticket can be thrown out — exactly what happened on TC-2026-0003.",
          "gpt-4o", "doc-tx-aus-appeal-2025-114", "https://www.traviscountytx.gov/justice-of-the-peace"),
]

# ============================================================================
# ReferenceDocuments  (library: interpretation-series + appeal-board-decisions)
# ============================================================================
def refdoc(rid, lib, series, title, state, jur, fname, url):
    return {"ReferenceDocumentId": rid, "Library": lib, "SeriesNumber": series, "Title": title,
            "ReferenceState": state, "Jurisdiction": jur, "FileName": fname, "CitationURL": url, **audit()}

ROWS["ReferenceDocuments"] = [
    refdoc("ref-ca-is-001", "interpretation-series", "IS-CA-001", "Interpreting the 21-Day Window for Mailed Notices", "CA", "ca-la",
           "is-ca-001.pdf", "https://www.courts.ca.gov/traffic.htm"),
    refdoc("ref-ny-is-006", "interpretation-series", "IS-NY-006", "Counting Points Within the 18-Month Lookback", "NY", "ny-nyc",
           "is-ny-006.pdf", "https://dmv.ny.gov/tickets/about-nys-driver-point-system"),
    refdoc("ref-tx-ab-2025-114", "appeal-board-decisions", "AB-2025-114", "Radar Calibration Records Required at Hearing", "TX", "tx-aus",
           "ab-2025-114.pdf", "https://www.traviscountytx.gov/justice-of-the-peace"),
    refdoc("ref-tx-ab-2024-088", "appeal-board-decisions", "AB-2024-088", "Late-Filed Hearing Requests and Good Cause", "TX", "tx-aus",
           "ab-2024-088.pdf", "https://www.traviscountytx.gov/justice-of-the-peace"),
]

# ============================================================================
# ERBTables  (catalog of the demo's own tables) + ERBFields for a few of them
#   ERBPackage -> citations ; Platform -> ticket-portal
# ============================================================================
def erbtable(name, desc, fieldcount, is_catalog=False, rep_crud="CRUD", rep_where=None):
    r = {"ERBTableId": name, "TableName": name, "Description": desc, "ERBPackage": "citations",
         "Platform": "ticket-portal", "IsLicensed": True, "FieldCount": fieldcount, "IsCatalog": is_catalog,
         "AdminCRUD": "CRUD", "ManagerCRUD": "CRUD", "RepresentativeCRUD": rep_crud, "ExternalLlmCRUD": "R",
         **audit()}
    if rep_where: r["RepresentativeWHERE"] = rep_where
    return r

ROWS["ERBTables"] = [
    erbtable("Jurisdictions", "Rules source: deadlines, fines, penalties, point thresholds.", 13, rep_crud="R"),
    erbtable("ViolationTypes", "Catalog of violations: base fine, points, school eligibility.", 6, rep_crud="R"),
    erbtable("Drivers", "The cited party; rolls up ActivePoints into LicenseStatus.", 6),
    erbtable("Citations", "The case hub; derives all four status tracks from facts + rules.", 12,
             rep_where="driver = app.jwt_email_driver()"),
    erbtable("Hearings", "Contest-track events; latest outcome drives ContestStatus.", 8),
    erbtable("Payments", "Payment-track records against a citation.", 7),
    erbtable("CaseEvents", "Append-only log mirroring each citation transition.", 9, rep_crud="CR"),
    erbtable("AuditLogEntries", "Per-row audit trail of status changes, overrides, and edits.", 12, is_catalog=True, rep_crud="R"),
    erbtable("WorkQueueItems", "Prioritized list of outstanding work, bucketed by urgency.", 11, is_catalog=True),
    erbtable("AssistantTurns", "AI assistant turns with token usage and per-model cost.", 25, is_catalog=True, rep_crud="CR"),
    erbtable("BusinessRules", "The R1..R38 business rules grouped by category.", 9, is_catalog=True, rep_crud="R"),
    erbtable("GlossaryTerms", "Plain-language definitions of domain terms.", 7, is_catalog=True, rep_crud="R"),
]

# ERBFields — illustrative field catalog for the Citations table (the hub).
def erbfield(table, fname, ftype, dt, desc):
    return {"ERBFieldId": f"{table}.{fname}", "ERBTable": table, "FieldName": fname,
            "FieldType": ftype, "Datatype": dt, "Description": desc, **audit()}

ROWS["ERBFields"] = [
    erbfield("Citations", "CitationNumber", "raw", "string", "Human-readable citation number, e.g. TC-2026-0002."),
    erbfield("Citations", "Driver", "relationship", "string", "FK to the cited driver."),
    erbfield("Citations", "ViolationType", "relationship", "string", "FK to the violation type."),
    erbfield("Citations", "Jurisdiction", "relationship", "string", "FK to the issuing jurisdiction (source of all deadlines/fines)."),
    erbfield("Citations", "IssuedOn", "raw", "datetime", "Date the citation was issued."),
    erbfield("Citations", "ResponseDueDate", "calculated", "datetime", "IssuedOn + Jurisdiction.DaysToRespond. Re-derived whenever the rule changes."),
    erbfield("Citations", "CitationStatus", "calculated", "string", "Computed top-level status track; never stored."),
    erbfield("Citations", "ContestStatus", "calculated", "string", "Computed contest track from the latest hearing."),
    erbfield("Citations", "PaymentStatus", "calculated", "string", "Computed payment track including late penalty / collections."),
    erbfield("Drivers", "ActivePoints", "aggregation", "number", "Sum of points from the driver's adjudicated, non-dismissed citations."),
    erbfield("Drivers", "LicenseStatus", "calculated", "string", "Valid → Warning → Suspended against the jurisdiction thresholds."),
    erbfield("Hearings", "Outcome", "raw", "string", "Pending | Dismissed | Upheld — drives downstream liability."),
]

# ============================================================================
# PlatformNaviation  (the route tree; ERBPackage -> citations)
# ============================================================================
def nav(rkey, parent, display, route, order, primary_table, primary_view, desc,
        admin="CRUD", mgr="CRUD", rep="R", llm="", icon="circle", phase=2, status="shipped",
        rule_refs="", pin=False, dynamic=False, requires_claim=False):
    return {
        "PlatformNaviationId": "nav-" + rkey.replace(".", "-"),
        "DisplayName": display, "Route": route, "Description": desc, "SortOrder": order,
        "ParentRouteKey": parent or "", "RouteKey": rkey, "NavLevel": ("top" if not parent else "sub"),
        "PrimaryTable": primary_table, "ERBPackage": "citations", "PrimaryView": primary_view,
        "BusinessRuleRefs": rule_refs, "BuildPhase": phase, "Status": status, "IsLicensed": True,
        "RequiresClaimContext": requires_claim, "PinToTop": pin, "IsDynamic": dynamic, "IconHint": icon,
        "AdminCRUD": admin, "ManagerCRUD": mgr, "RepresentativeCRUD": rep, "ExternalLlmCRUD": llm,
        **audit(),
    }

ROWS["PlatformNaviation"] = [
    # Top level
    nav("dashboard", "", "Dashboard", "/dashboard", 1, "Citations", "vw_citations",
        "Portfolio overview: open citations, due payments, upcoming hearings.", icon="layout-dashboard", phase=1, pin=True, rule_refs="R50"),
    nav("citations", "", "Citations", "/citations", 2, "Citations", "vw_citations",
        "All citations across drivers and jurisdictions — the case hub.", admin="CRUD", mgr="CRUD", rep="CRU", icon="file-text", rule_refs="R1,R10"),
    nav("citations.detail", "citations", "Citation Detail", "/citations/:citationId", 1, "Citations", "vw_citations",
        "A single citation with its four computed status tracks, events, hearings and payments.", admin="CRUD", mgr="CRUD", rep="RU", icon="file-search", dynamic=True, requires_claim=True, rule_refs="R10,R12,R20"),
    nav("hearings", "", "Hearings", "/hearings", 3, "Hearings", "vw_hearings",
        "Contest hearings: requested, scheduled, and heard.", admin="CRUD", mgr="CRUD", rep="R", icon="gavel", rule_refs="R12,R13"),
    nav("payments", "", "Payments", "/payments", 4, "Payments", "vw_payments",
        "Payments recorded against citations.", admin="CRUD", mgr="CRUD", rep="CR", icon="credit-card", rule_refs="R20,R23"),
    nav("drivers", "", "Drivers", "/drivers", 5, "Drivers", "vw_drivers",
        "Drivers and their rolled-up license points and status.", admin="CRUD", mgr="CRUD", rep="R", icon="user", rule_refs="R30,R31"),
    nav("jurisdictions", "", "Jurisdictions", "/jurisdictions", 6, "Jurisdictions", "vw_jurisdictions",
        "The rules engine: every deadline, fine, penalty and threshold as data.", admin="CRUD", mgr="RU", rep="R", icon="scale", rule_refs="R40"),
    nav("work-queue", "", "Work Queue", "/work-queue", 7, "WorkQueueItems", "vw_work_queue_items",
        "Everything that needs attention now, bucketed by urgency.", admin="CRUD", mgr="CRUD", rep="RU", icon="alarm-clock", rule_refs="R50,R51"),
    nav("assistant", "", "AI Assistant", "/assistant", 8, "AssistantTurns", "vw_assistant_turns",
        "Ask questions about a citation, the rules, or the portal.", admin="CRUD", mgr="CRUD", rep="CR", llm="R", icon="bot", phase=3, status="in-progress", rule_refs="R70"),
    # Library section (top + children)
    nav("library", "", "Library", "/library", 9, "ReferenceDocuments", "vw_reference_documents",
        "Glossary, business rules, and jurisdiction source documents.", admin="CRUD", mgr="R", rep="R", llm="R", icon="book-open"),
    nav("library.glossary", "library", "Glossary", "/library/glossary", 1, "GlossaryTerms", "vw_glossary_terms",
        "Plain-language definitions grouped by category.", admin="CRUD", mgr="R", rep="R", llm="R", icon="book-a"),
    nav("library.business-rules", "library", "Business Rules", "/library/business-rules", 2, "BusinessRules", "vw_business_rules",
        "The R1..R38 rules grouped by category, each linked to its schema location.", admin="CRUD", mgr="R", rep="R", llm="R", icon="list-checks", rule_refs="R10,R40"),
    nav("library.jurisdiction-rules", "library", "Jurisdiction Rules", "/library/jurisdiction-rules", 3, "JurisdictionRules", "vw_jurisdiction_rules",
        "AI-distilled rule summaries per jurisdiction, with sources.", admin="CRUD", mgr="R", rep="R", llm="R", icon="landmark", phase=4, status="planned"),
    nav("library.jurisdiction-docs", "library", "Source Documents", "/library/jurisdiction-docs", 4, "JurisdictionSourceDocuments", "vw_jurisdiction_source_documents",
        "Statutes and appeal-board decisions backing the rules.", admin="CRUD", mgr="R", rep="R", llm="R", icon="file-stack", phase=4, status="planned"),
    # Admin section (top + children) — admin only
    nav("admin", "", "Admin", "/admin", 20, "ERBTables", "vw_erb_tables",
        "Platform administration: schema catalog, features, navigation, audit, branding.", admin="CRUD", mgr="", rep="", llm="", icon="settings", phase=5),
    nav("admin.features", "admin", "Features", "/admin/features", 1, "ERBFeatures", "vw_erb_features",
        "The feature catalog with status, category and cartoon imagery.", admin="CRUD", mgr="", rep="", llm="", icon="sparkles", phase=5),
    nav("admin.feature-detail", "admin.features", "Feature Detail", "/admin/features/:featureId", 1, "ERBFeatures", "vw_erb_features",
        "A single feature: description, rules implemented, status, and image.", admin="CRUD", mgr="", rep="", llm="", icon="image", phase=5, dynamic=True),
    nav("admin.tables", "admin", "Schema Catalog", "/admin/tables", 2, "ERBTables", "vw_erb_tables",
        "Every table in the rulebook with its per-role CRUD grants.", admin="CRUD", mgr="", rep="", llm="", icon="table", phase=5),
    nav("admin.navigation", "admin", "Navigation", "/admin/navigation", 3, "PlatformNaviation", "vw_platform_naviation",
        "This very navigation tree, edited as data.", admin="CRUD", mgr="", rep="", llm="", icon="route", phase=5),
    nav("admin.audit-log", "admin", "Audit Log", "/admin/audit-log", 4, "AuditLogEntries", "vw_audit_log_entries",
        "Append-only history of every change, override, and decision.", admin="CRUD", mgr="R", rep="", llm="", icon="scroll-text", phase=5, rule_refs="R60"),
    nav("admin.branding", "admin", "Branding", "/admin/branding", 5, "SiteBranding", "vw_site_branding",
        "Site title, colors, logos, and contact info.", admin="CRUD", mgr="", rep="", llm="", icon="palette", phase=6),
    nav("admin.versions", "admin", "Versions", "/admin/versions", 6, "ERBVersions", "vw_erb_versions",
        "Release history of the rulebook itself.", admin="CRUD", mgr="R", rep="", llm="", icon="git-branch", phase=6),
]

# ============================================================================
# StateTransitions  (event log of edges actually traversed by our citations)
#   SubjectTableName/SubjectId are polymorphic raw strings.
#   For Citations the SubjectId is lower(CitationNumber) to match the FK convention.
#   TriggeredBy -> AppUsers.AppUserId (null for system).
# ============================================================================
def trans(tid, machine, subj_table, subj_id, frm, to, at, by, role, reason):
    return {"StateTransitionId": tid, "StateMachine": machine, "SubjectTableName": subj_table,
            "SubjectId": subj_id, "FromStateKey": frm, "ToStateKey": to, "TransitionAt": at,
            "TriggeredBy": by, "TriggeredByRole": role, "Reason": reason, **audit(by=by or ADMIN, at=at)}

ROWS["StateTransitions"] = [
    # tc-2026-0002 (Jane Doe, contesting): issued -> responded -> incontest ; contest requested -> scheduled
    trans("st-0002-cl-issued", "citation-lifecycle", "Citations", "tc-2026-0002", None, "Issued", "2026-05-20T08:00:00Z", None, "system", "Citation issued for failure to stop at a red signal."),
    trans("st-0002-cl-responded", "citation-lifecycle", "Citations", "tc-2026-0002", "Issued", "Responded", "2026-05-25T14:30:00Z", REP, "representative", "Driver responded by requesting a hearing within the 21-day window."),
    trans("st-0002-cl-incontest", "citation-lifecycle", "Citations", "tc-2026-0002", "Responded", "InContest", "2026-05-26T09:10:00Z", MGR, "manager", "Hearing scheduled; citation entered the contest track."),
    trans("st-0002-ct-requested", "contest-track", "Citations", "tc-2026-0002", "NotContested", "HearingRequested", "2026-05-25T14:31:00Z", REP, "representative", "Driver contests red-light timing."),
    trans("st-0002-ct-scheduled", "contest-track", "Citations", "tc-2026-0002", "HearingRequested", "Scheduled", "2026-05-26T09:10:00Z", MGR, "manager", "Hearing scheduled for 2026-06-20."),
    # tc-2026-0003 (Carlos, dismissed): issued->responded->incontest->adjudicated->closed ; contest req->scheduled->heard ; payment pending->notowed
    trans("st-0003-cl-issued", "citation-lifecycle", "Citations", "tc-2026-0003", None, "Issued", "2026-03-01T08:00:00Z", None, "system", "Citation issued for exceeding the posted speed limit."),
    trans("st-0003-ct-requested", "contest-track", "Citations", "tc-2026-0003", "NotContested", "HearingRequested", "2026-03-10T10:00:00Z", REP, "representative", "Driver requested a hearing, disputing the radar reading."),
    trans("st-0003-ct-scheduled", "contest-track", "Citations", "tc-2026-0003", "HearingRequested", "Scheduled", "2026-03-12T11:00:00Z", MGR, "manager", "Hearing scheduled for 2026-04-15."),
    trans("st-0003-ct-heard", "contest-track", "Citations", "tc-2026-0003", "Scheduled", "Heard", "2026-04-15T15:00:00Z", MGR, "manager", "Hearing held; agency could not produce radar calibration records."),
    trans("st-0003-pay-notowed", "payment-track", "Citations", "tc-2026-0003", "Pending", "NotOwed", "2026-04-15T15:05:00Z", None, "system", "Citation dismissed at hearing — nothing owed (per Travis County AB-2025-114)."),
    trans("st-0003-cl-closed", "citation-lifecycle", "Citations", "tc-2026-0003", "Adjudicated", "Closed", "2026-04-15T15:10:00Z", None, "system", "Dismissed citation closed."),
    # tc-2026-0005 (Aisha, paid): issued->responded->adjudicated->closed ; payment due->paid
    trans("st-0005-cl-issued", "citation-lifecycle", "Citations", "tc-2026-0005", None, "Issued", "2026-05-15T08:00:00Z", None, "system", "Citation issued for speeding 11-20 mph over the limit."),
    trans("st-0005-pay-paid", "payment-track", "Citations", "tc-2026-0005", "Due", "Paid", "2026-05-18T19:22:00Z", REP, "representative", "Driver paid the $300 fine in full online."),
    trans("st-0005-cl-closed", "citation-lifecycle", "Citations", "tc-2026-0005", "Adjudicated", "Closed", "2026-05-18T19:23:00Z", None, "system", "Paid in full; citation closed."),
    # tc-2026-0004 (Carlos, defaulting): issued->adjudicated(default) ; payment due->late
    trans("st-0004-cl-issued", "citation-lifecycle", "Citations", "tc-2026-0004", None, "Issued", "2026-02-01T08:00:00Z", None, "system", "Citation issued for disregarding a red light."),
    trans("st-0004-pay-late", "payment-track", "Citations", "tc-2026-0004", "Due", "Late", "2026-03-01T00:00:00Z", None, "system", "Pay-by date passed unpaid; 30% Travis County late penalty applied."),
]

# ============================================================================
# SubjectStateInstances  (occupancy records — current state per subject)
#   EnteredViaTransition -> StateTransitions.Name ; PriorInstance -> self Name
# ============================================================================
def ssi(table, subj, machine, state, entered, exited, seq, prior, via):
    sid = f"ssi-{table.lower()}-{subj}-{state.lower()}-{seq}"
    return {"SubjectStateInstanceId": sid, "StateMachine": machine, "SubjectTableName": table,
            "SubjectId": subj, "StateKey": state, "EnteredAt": entered, "ExitedAt": exited,
            "SequenceIndex": seq, "PriorInstance": prior, "EnteredViaTransition": via,
            **audit(at=entered)}

ROWS["SubjectStateInstances"] = [
    # tc-2026-0002 citation-lifecycle path: Issued -> Responded -> InContest(current)
    ssi("Citations", "tc-2026-0002", "citation-lifecycle", "Issued", "2026-05-20T08:00:00Z", "2026-05-25T14:30:00Z", 1, None, "st-0002-cl-issued"),
    ssi("Citations", "tc-2026-0002", "citation-lifecycle", "Responded", "2026-05-25T14:30:00Z", "2026-05-26T09:10:00Z", 2, "ssi-citations-tc-2026-0002-issued-1", "st-0002-cl-responded"),
    ssi("Citations", "tc-2026-0002", "citation-lifecycle", "InContest", "2026-05-26T09:10:00Z", None, 3, "ssi-citations-tc-2026-0002-responded-2", "st-0002-cl-incontest"),
    # tc-2026-0003 citation-lifecycle: ... Closed(current, terminal)
    ssi("Citations", "tc-2026-0003", "citation-lifecycle", "Issued", "2026-03-01T08:00:00Z", "2026-03-10T10:00:00Z", 1, None, "st-0003-cl-issued"),
    ssi("Citations", "tc-2026-0003", "citation-lifecycle", "Closed", "2026-04-15T15:10:00Z", None, 2, "ssi-citations-tc-2026-0003-issued-1", "st-0003-cl-closed"),
    # tc-2026-0003 contest-track: Heard(current, terminal)
    ssi("Citations", "tc-2026-0003", "contest-track", "Heard", "2026-04-15T15:00:00Z", None, 1, None, "st-0003-ct-heard"),
    # tc-2026-0005 payment-track: Paid(current, terminal)
    ssi("Citations", "tc-2026-0005", "payment-track", "Paid", "2026-05-18T19:22:00Z", None, 1, None, "st-0005-pay-paid"),
    # tc-2026-0004 payment-track: Late(current)
    ssi("Citations", "tc-2026-0004", "payment-track", "Late", "2026-03-01T00:00:00Z", None, 1, None, "st-0004-pay-late"),
]

# ============================================================================
# AuditLogEntries  (Citation FK -> lower(CitationNumber); Actor -> AppUserId)
# ============================================================================
def alog(aid, citation, actor, ts, action, frm, to, reason, source):
    return {"AuditLogEntrieId": aid, "Citation": citation, "Actor": actor, "Timestamp": ts,
            "ActionType": action, "FromValue": frm, "ToValue": to, "Reason": reason, "Source": source,
            **audit(by=actor, at=ts)}

ROWS["AuditLogEntries"] = [
    alog("audit-0002-hearing-req", "tc-2026-0002", REP, "2026-05-25T14:30:00Z", "STATUS_CHANGE", "NotContested", "HearingRequested", "Driver elected to contest the red-light citation.", "PORTAL"),
    alog("audit-0002-scheduled", "tc-2026-0002", MGR, "2026-05-26T09:10:00Z", "STATUS_CHANGE", "HearingRequested", "Scheduled", "Hearing scheduled for 2026-06-20.", "UI"),
    alog("audit-0003-dismissed", "tc-2026-0003", MGR, "2026-04-15T15:00:00Z", "review-decision", "Scheduled", "Heard", "Hearing outcome recorded: Dismissed (no radar calibration records).", "UI"),
    alog("audit-0003-notowed-override", "tc-2026-0003", ADMIN, "2026-04-15T15:05:00Z", "OVERRIDE", "Due", "NotOwed", "Manual override to zero the balance after dismissal, per Travis County AB-2025-114.", "UI"),
    alog("audit-0005-paid", "tc-2026-0005", REP, "2026-05-18T19:22:00Z", "STATUS_CHANGE", "Due", "Paid", "Online payment of $300 confirmed.", "API"),
    alog("audit-0004-late", "tc-2026-0004", None, "2026-03-01T00:00:00Z", "STATUS_CHANGE", "Due", "Late", "Pay-by date passed unpaid; 30% late penalty applied automatically.", "SYSTEM"),
    alog("audit-0001-overdue", "tc-2026-0001", None, "2026-05-22T00:00:00Z", "STATUS_CHANGE", "Issued", "Issued", "Response window (21 days) elapsed with no response; flagged for default.", "SYSTEM"),
]
# Fix actor=None rows: SYSTEM actions have no actor; leave Actor unset rather than null FK
for r in ROWS["AuditLogEntries"]:
    if r.get("Actor") is None:
        r.pop("Actor", None)
        r["Source"] = "SYSTEM"

# ============================================================================
# WorkQueueItems  (AssignedTo -> AppUserId; SubjectId = lower(CitationNumber))
#   DueDate carried from the subject; calc fields (DueInDays/UrgencyBucket) derived by substrate.
# ============================================================================
def wq(wid, table, subj, itype, state, due, assigned):
    return {"WorkQueueItemId": wid, "SubjectTableName": table, "SubjectId": subj, "ItemType": itype,
            "CurrentStateKey": state, "DueDate": due, "AssignedTo": assigned, **audit()}

ROWS["WorkQueueItems"] = [
    # tc-2026-0001: overdue response (due 2026-05-22, well past as-of 2026-06-06) -> urgent
    wq("wq-citations-tc-2026-0001", "Citations", "tc-2026-0001", "response-due", "Issued", "2026-05-22", REP),
    # tc-2026-0006: fresh NY ticket, response due 2026-06-24 (within window) -> due/follow-up
    wq("wq-citations-tc-2026-0006", "Citations", "tc-2026-0006", "response-due", "Issued", "2026-06-24", REP),
    # tc-2026-0002: hearing scheduled 2026-06-20 (upcoming)
    wq("wq-citations-tc-2026-0002", "Citations", "tc-2026-0002", "hearing-prep", "InContest", "2026-06-20", MGR),
    # tc-2026-0004: late payment, collections window approaching (TX 45 days from 2026-03-01 pay-by)
    wq("wq-citations-tc-2026-0004", "Citations", "tc-2026-0004", "payment-due", "Late", "2026-04-15", MGR),
]

# ============================================================================
# AssistantTurns  (Citation FK -> lower(CitationNumber); AiModel/ModelPricingVersion FKs)
#   Token counts drive the computed cost columns.
# ============================================================================
def turn(tid, created, state, kind, route_path, route_key, citation, user, model, pricing,
         umsg, reply, in_tok, cached, out_tok):
    raw = json.dumps({"system_prompt_kind": kind, "finish_reason": "stop",
                      "usage": {"input_tokens": in_tok, "cached_input_tokens": cached, "output_tokens": out_tok}})
    return {"AssistantTurnId": tid, "CreatedAt": created, "CurrentStateKey": state, "QuestionKind": kind,
            "RoutePath": route_path, "RouteKey": route_key, "Citation": citation, "AppUserId": user,
            "AiModel": model, "ModelPricingVersion": pricing, "UserMessage": umsg, "AssistantReply": reply,
            "RawExchange": raw, "InputTokens": in_tok, "CachedInputTokens": cached, "OutputTokens": out_tok,
            "ModifiedAt": created, "ModifiedBy": user, "CreatedBy": user, "ModifiedByModel": model}

ROWS["AssistantTurns"] = [
    turn("turn-0002-deadline", "2026-05-25T14:25:00Z", "responding", "claim",
         "/citations/tc-2026-0002", "citations.detail", "tc-2026-0002", REP, "gpt-4o-mini", "gpt-4o-mini-2025-07",
         "What's the deadline to contest citation TC-2026-0002?",
         "Citation TC-2026-0002 was issued 2026-05-20 in Los Angeles County, which allows 21 days to respond — so the deadline is 2026-06-10. Requesting a hearing now (today is 2026-05-25) is well within the window.",
         1850, 1200, 140),
    turn("turn-0003-dismiss-basis", "2026-04-15T15:30:00Z", "responding", "rulebook",
         "/citations/tc-2026-0003", "citations.detail", "tc-2026-0003", MGR, "gpt-4o", "gpt-4o-2024-08",
         "On what basis can TC-2026-0003 be dismissed?",
         "Under Travis County Appeal Board decision AB-2025-114, if the agency cannot produce radar/lidar calibration records on timely request, the speed measurement is unreliable and the citation must be dismissed. The hearing notes confirm calibration records were not produced, so dismissal is supported.",
         3100, 1900, 220),
    turn("turn-0004-penalty", "2026-03-02T09:00:00Z", "responding", "claim",
         "/citations/tc-2026-0004", "citations.detail", "tc-2026-0004", REP, "gpt-4o-mini", "gpt-4o-mini-2025-07",
         "How much does Carlos owe now on TC-2026-0004?",
         "TC-2026-0004 has a base fine of $275 (Travis County red-light). The pay-by date passed on 2026-03-01, so Travis County's 30% late penalty applies: $275 × 1.30 = $357.50. If it stays unpaid past the 45-day collections window, it moves to Collections.",
         1700, 1100, 160),
    turn("turn-routing-company", "2026-06-06T10:00:00Z", "responding", "company",
         "/dashboard", "dashboard", None, MGR, "erb-internal-classifier", "erb-internal-classifier-2026-01",
         "How many of our citations are currently unpaid and overdue?",
         "As of 2026-06-06, one citation is overdue on response (TC-2026-0001) and one is late on payment heading toward collections (TC-2026-0004). The work queue lists both as urgent.",
         900, 0, 90),
    turn("turn-failed-example", "2026-06-06T10:05:00Z", "failed", "rulebook",
         "/library/business-rules", "library.business-rules", None, REP, "gpt-4o-mini", "gpt-4o-mini-2025-07",
         "Summarize rule R99.",
         "I couldn't find a rule R99 — the business rules in this rulebook range from R1 to R70. Please check the rule code and try again.",
         1200, 800, 60),
]
# AssistantTurns rows whose Citation is None: drop the key so it isn't a dangling FK
for r in ROWS["AssistantTurns"]:
    if r.get("Citation") is None:
        r.pop("Citation", None)

# ============================================================================
# APIEndpoints  (action/RPC endpoints beyond plain CRUD)
# ============================================================================
def ep(eid, title, method, path, etype, subject, roles, status, desc, where=None, sm=None):
    r = {"APIEndpointId": eid, "Title": title, "HttpMethod": method, "Path": path, "EndpointType": etype,
         "SubjectTableName": subject, "RoleVisibility": roles, "Status": status, "Description": desc, **audit()}
    if where: r["WhereClause"] = where
    if sm: r["TriggersStateMachine"] = sm
    return r

ROWS["APIEndpoints"] = [
    ep("citation-respond", "Respond to Citation", "POST", "/api/citations/:id/respond", "ACTION", "Citations",
       "Admin,Manager,Representative", "shipped", "Record a driver's response (pay or request hearing), advancing the citation from Issued to Responded.", sm="citation-lifecycle"),
    ep("citation-request-hearing", "Request Hearing", "POST", "/api/citations/:id/request-hearing", "ACTION", "Citations",
       "Admin,Manager,Representative", "shipped", "Create a hearing request for a citation and advance the contest track to HearingRequested.", sm="contest-track"),
    ep("hearing-schedule", "Schedule Hearing", "POST", "/api/hearings/:id/schedule", "ACTION", "Hearings",
       "Admin,Manager", "shipped", "Manager schedules a requested hearing (HearingRequested → Scheduled).", sm="contest-track"),
    ep("hearing-record-outcome", "Record Hearing Outcome", "POST", "/api/hearings/:id/outcome", "ACTION", "Hearings",
       "Admin,Manager", "shipped", "Record a hearing outcome (Dismissed/Upheld), driving liability and the payment track.", sm="contest-track"),
    ep("payment-checkout", "Pay Citation", "POST", "/api/citations/:id/pay", "ACTION", "Payments",
       "Admin,Manager,Representative", "shipped", "Take an online payment in full and close the citation (Due → Paid).", sm="payment-track"),
    ep("jurisdiction-recompute", "Recompute Jurisdiction Cases", "POST", "/api/jurisdictions/:id/recompute", "RPC", "Jurisdictions",
       "Admin", "shipped", "Force re-derivation of all citations under a jurisdiction after a rule change (normally automatic via the view)."),
    ep("work-queue-report", "Work Queue Report", "GET", "/api/reports/work-queue", "REPORT", "WorkQueueItems",
       "Admin,Manager,Representative", "shipped", "Return the prioritized outstanding-work list, bucketed by urgency.",
       where="assigned_to = app.jwt_email()"),
    ep("assistant-ask", "Ask the Assistant", "POST", "/api/assistant/ask", "ACTION", "AssistantTurns",
       "Admin,Manager,Representative", "in-progress", "Submit a question; classify it, answer it, and persist a fully-costed AssistantTurn.", sm=None),
    ep("collections-sweep", "Nightly Collections Sweep", "POST", "/api/jobs/collections-sweep", "BATCH", "Citations",
       "Admin", "planned", "Scheduled job that moves Late citations past the collections window into Collections."),
]

# ----------------------------------------------------------------------------
# Injection + validation
# ----------------------------------------------------------------------------
def load_rb():
    with open(RB_PATH) as f:
        return json.load(f)

def fk_keys(rb):
    """Build the set of valid natural keys for FK validation, per target table."""
    def names(table, fn):
        return set(fn(r) for r in rb.get(table, {}).get("data", []))
    return {
        "Citations": set(r["CitationNumber"].lower() for r in rb["Citations"]["data"]),
        "Drivers": set(r["LicenseNumber"].lower() for r in rb["Drivers"]["data"]),
        "Jurisdictions": set(r["Code"].lower() for r in rb["Jurisdictions"]["data"]),
        "ViolationTypes": set(r["Code"].lower() for r in rb["ViolationTypes"]["data"]),
        "AppUsers": set(r["AppUserId"] for r in rb["AppUsers"]["data"]),
        "AiModels": set(r["AiModelId"] for r in rb["AiModels"]["data"]),
        "ModelPricingVersions": set(r["ModelPricingVersionId"] for r in rb["ModelPricingVersions"]["data"]),
        "StateMachines": set(r["StateMachineId"] for r in rb["StateMachines"]["data"]),
        "ERBPackages": set(r["ERBPackageId"] for r in rb["ERBPackages"]["data"]),
        "Platforms": set(r["PlatformId"] for r in rb["Platforms"]["data"]),
        "ERBFeatureStatuses": set(r["ERBFeatureStatusId"] for r in rb["ERBFeatureStatuses"]["data"]),
    }

# FK column -> target table, validated after we add our own rows (for self/intra refs).
FK_MAP = {
    "ERBFeatures": {"Category": None, "Status": "ERBFeatureStatuses", "ERBPackage": "ERBPackages"},
    "ERBTables": {"ERBPackage": "ERBPackages", "Platform": "Platforms"},
    "ERBFields": {"ERBTable": None},
    "PlatformNaviation": {"ERBPackage": "ERBPackages"},
    "BusinessRules": {"Category": None},
    "GlossaryTerms": {"Category": None},
    "JurisdictionRules": {"Jurisdiction": "Jurisdictions", "SourceDocument": None},
    "JurisdictionSourceDocuments": {"Jurisdiction": "Jurisdictions"},
    "ReferenceDocuments": {"Jurisdiction": "Jurisdictions"},
    "StateTransitions": {"StateMachine": "StateMachines", "TriggeredBy": "AppUsers"},
    "SubjectStateInstances": {"StateMachine": "StateMachines"},
    "AuditLogEntries": {"Citation": "Citations", "Actor": "AppUsers"},
    "WorkQueueItems": {"AssignedTo": "AppUsers"},
    "AssistantTurns": {"Citation": "Citations", "AiModel": "AiModels", "ModelPricingVersion": "ModelPricingVersions"},
}

def strip_private(rows):
    return [{k: v for k, v in r.items() if not k.startswith("_")} for r in rows]

def validate(rb, keys):
    # Add our own table natural keys to the keyset for intra-batch FKs.
    keys = dict(keys)
    keys["ERBFeatureCategories"] = set(r["ERBFeatureCategoryId"] for r in ROWS["ERBFeatureCategories"])
    keys["BusinessRuleCategories"] = set(r["BusinessRuleCategoryId"] for r in ROWS["BusinessRuleCategories"])
    keys["GlossaryCategories"] = set(r["GlossaryCategoryId"] for r in ROWS["GlossaryCategories"])
    keys["ERBTables"] = set(r["ERBTableId"] for r in ROWS["ERBTables"])
    keys["JurisdictionSourceDocuments"] = set("doc" not in "" and r["JurisdictionSourceDocumentId"] or
                                              ("-".join(r["Title"].lower().split())) for r in ROWS["JurisdictionSourceDocuments"])
    errors = []
    for table, fkmap in FK_MAP.items():
        for r in ROWS.get(table, []):
            for col, target in fkmap.items():
                if col not in r or r[col] in (None, ""):
                    continue
                vals = [v.strip() for v in str(r[col]).split(",")] if col in ("Category",) else [r[col]]
                # resolve special targets
                tgt = target
                if table == "ERBFeatures" and col == "Category": tgt = "ERBFeatureCategories"
                if table == "BusinessRules" and col == "Category": tgt = "BusinessRuleCategories"
                if table == "GlossaryTerms" and col == "Category": tgt = "GlossaryCategories"
                if table == "ERBFields" and col == "ERBTable": tgt = "ERBTables"
                if table == "JurisdictionRules" and col == "SourceDocument":
                    valid = set(d["Title"].lower().replace(" ", "-") for d in ROWS["JurisdictionSourceDocuments"])
                    if r[col] not in valid:
                        errors.append(f"{table}.{col} = '{r[col]}' not in JurisdictionSourceDocuments.Name")
                    continue
                if not tgt: continue
                valid = keys.get(tgt, set())
                for v in vals:
                    if v and v not in valid:
                        errors.append(f"{table}.{col} = '{v}' not in {tgt} ({sorted(valid)[:4]}...)")
    return errors

def main():
    rb = load_rb()
    keys = fk_keys(rb)
    counts = {k: len(v) for k, v in ROWS.items()}
    print("ROW COUNTS:", json.dumps(counts, indent=2))
    print("TOTAL TABLES:", len(ROWS), "TOTAL ROWS:", sum(counts.values()))

    errs = validate(rb, keys)
    if errs:
        print("\n!!! FK VALIDATION ERRORS:")
        for e in errs: print("  -", e)
    else:
        print("\nFK validation: OK (all FKs incl. SourceDocument)")

    if "--dry-run" in sys.argv:
        print("\n(dry-run — not writing)")
        return

    # Inject: set each table's data to our stripped rows. Confirm table currently empty.
    for table, rows in ROWS.items():
        cur = rb[table]["data"]
        if cur:
            raise SystemExit(f"REFUSING: {table} already has {len(cur)} rows (expected empty).")
        rb[table]["data"] = strip_private(rows)

    with open(RB_PATH, "w") as f:
        json.dump(rb, f, ensure_ascii=False, indent=2)
        f.write("\n")
    print(f"\nWROTE {RB_PATH}")

if __name__ == "__main__":
    main()
