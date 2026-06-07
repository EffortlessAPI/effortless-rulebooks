#!/usr/bin/env python3
"""
loop-01-feature-spine.py  (idempotent)

Loop 1 of the 10-loop expansion plan. Fills the PROVENANCE SPINE of every
ERBFeatures row: rich multi-sentence SourceText (>= 3 sentences, names the
tables/views it touches + the user action it enables), RuleRefs (CSV of
BusinessRule RuleCodes in the TT-<CAT>-NN scheme that Loop 2 will guarantee),
SourceFiles (our docs), and Route (FK -> PlatformNaviation route key, which
Loop 3 will guarantee) for every screen-rendering feature.

Merge-by-id, never clobber: a non-empty existing SourceText/RuleRefs/Route is
left as-is; only empties are filled. Re-running = 0 changes.

FK convention (verified): a relationship value = the TARGET row's `Name`
natural key (lower-kebab) = PK with type-prefix stripped.

  Route  -> PlatformNaviation RouteKey (lower-kebab), e.g. citations-list

RoutePath / RelativePath are lookup/calculated -- DO NOT set, they derive
from Route.

Run:
  python3 scripts/loop-01-feature-spine.py --dry-run
  python3 scripts/loop-01-feature-spine.py
"""
import json
import os
import re
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
    row["ModifiedAt"] = row.get("ModifiedAt", NOW)
    row["ModifiedBy"] = row.get("ModifiedBy", WHO)
    return row


# ---------------------------------------------------------------------------
# Per-feature provenance. Each entry: (SourceText, RuleRefs, SourceFiles, Route)
# SourceText >= 3 sentences, names tables/views + the user action.
# RuleRefs use the TT-<CAT>-NN scheme Loop 2 will create.
# Route is the lower-kebab PlatformNaviation RouteKey Loop 3 will create
# (empty string => non-screen feature).
# ---------------------------------------------------------------------------
SF_DOMAIN = "README.md,rulespeak/rulespeak.md"
SF_PLATFORM = "README.md,EXPANSION-PLAN.md"
SF_BOTH = "README.md,rulespeak/rulespeak.md,EXPANSION-PLAN.md"

SPINE = {
    # ---- domain (citations package) ----
    "citation-intake": (
        "Records a new traffic citation as a row in the Citations table -- driver (FK to Drivers), "
        "violation type (FK to ViolationTypes), issuing jurisdiction (FK to Jurisdictions), and issue date -- "
        "and immediately derives the response deadline from the jurisdiction's DaysToRespond rule via vw_citations. "
        "An admin or representative captures the citation off a paper ticket and the computed ResponseDeadline column "
        "appears the moment the row is saved, with no manual date math. This is the front door of the whole "
        "citation-lifecycle state machine.",
        "TT-INTAKE-01,TT-INTAKE-02,TT-DEADLINE-01", SF_DOMAIN, "citation-detail"),
    "driver-response": (
        "Captures the driver's choice -- pay now or request a hearing -- writing it onto the Citations row and "
        "advancing the citation from Issued to Responded in vw_citations before the response window closes. "
        "A representative records the decision on behalf of the cited driver, and the contest-track state machine "
        "branches on it. The response and its timestamp become CaseEvents rows so the audit trail explains every move.",
        "TT-INTAKE-03,TT-CONTEST-01", SF_DOMAIN, "citation-detail"),
    "citation-statemachine": (
        "Shows the live Citation status (Issued -> Responded -> InContest -> Adjudicated -> Closed) re-derived on "
        "every read from facts and child CaseEvents rows in vw_citations -- never a stored, drifting column. "
        "A user opens a citation and sees a glowing status path that always reflects current reality. "
        "The same computed CitationStatus column drives the work queue and the dashboards.",
        "TT-CITATION-01,TT-CITATION-02", SF_BOTH, "citation-detail"),
    "contest-track": (
        "Derives the contest track (NotContested | HearingRequested -> Scheduled -> Heard) from the latest Hearings "
        "row, surfacing it as a calculated column on vw_citations that drives liability downstream. "
        "A representative tracking a contested ticket sees exactly where the hearing process stands. "
        "The track feeds the determination logic that decides whether a fine is owed.",
        "TT-CONTEST-02,TT-CONTEST-03,TT-HEARING-01", SF_BOTH, "citation-detail"),
    "payment-track": (
        "Computes the payment track (Pending -> Due -> Late -> Collections | Paid | NotOwed) on vw_citations, "
        "including the jurisdiction-specific late penalty once the pay-by date passes, by joining Citations, "
        "Payments, and the jurisdiction's penalty rule. A representative or driver sees the live amount owed and its "
        "escalation status. The Late and Collections transitions are the ones the deadline-driven work queue watches most.",
        "TT-PAYMENT-01,TT-PAYMENT-02,TT-FEE-01", SF_BOTH, "payments-list"),
    "license-points": (
        "Rolls up a driver's active points across their Citations and drives the license track "
        "(Valid -> Warning -> Suspended) against each jurisdiction's warning and suspension thresholds, exposed on "
        "vw_drivers. A manager reviewing a driver sees the points gauge and the resulting license standing. "
        "The aggregation re-runs on every read so a dismissed citation instantly lowers the active-point total.",
        "TT-LICENSE-01,TT-LICENSE-02", SF_BOTH, "drivers-list"),
    "jurisdiction-rules": (
        "Lets an admin edit a jurisdiction's deadlines, fines, penalty percentages, and point thresholds as data in "
        "the Jurisdictions and JurisdictionRules tables -- and every citation under it re-derives instantly through "
        "vw_citations with no code change and no migration. A manager turns a single dial and the whole region's "
        "computed deadlines and penalties move. This is the rules-as-data thesis of the entire platform.",
        "TT-RULES-01,TT-DEADLINE-01,TT-FEE-01", SF_BOTH, "jurisdictions"),
    "hearing-scheduling": (
        "Requests a hearing, lets a manager schedule it, and records the outcome as a Hearings row, driving the "
        "contest track and the citation's liability on vw_citations. A representative requests, a manager assigns a "
        "date, and the recorded outcome flows back to determine whether a fine survives. Each step writes a CaseEvents "
        "row so the timeline is complete.",
        "TT-HEARING-01,TT-HEARING-02,TT-CONTEST-03", SF_DOMAIN, "hearings-list"),
    "payment-checkout": (
        "Pays a citation's fine online in full, records a Payments row, and closes the case on vw_citations -- the "
        "path Aisha Khan took on citation TC-2026-0005. A representative or driver completes checkout and the payment "
        "track flips to Paid. The closing transition is logged to CaseEvents and removes the item from the work queue.",
        "TT-PAYMENT-01,TT-PAYMENT-03", SF_DOMAIN, "payments-list"),
    "work-queue": (
        "Surfaces a single prioritized list of everything needing attention now -- overdue responses, due payments, "
        "upcoming hearings -- as WorkQueueItems rows bucketed by urgency, computed from the deadline columns on "
        "vw_citations. A representative starts their day here and works top-down by urgency. Each queue item links "
        "back to the citation, driver, and the rule that put it there.",
        "TT-DEADLINE-01,TT-QUEUE-01", SF_BOTH, "work-queue"),
    "audit-trail": (
        "Logs every status change, override, and edit on a citation append-only into AuditLogEntries and CaseEvents, "
        "a full history that sits alongside -- and explains -- the computed current state in vw_citations. "
        "A manager investigating a dispute opens the trail and sees exactly who changed what and when. "
        "The append-only log never contradicts the derived state; it narrates how the state got there.",
        "TT-AUDIT-01,TT-AUDIT-02", SF_DOMAIN, "audit-log"),
    "ai-assistant": (
        "Answers plain-language questions about a citation, the rules, or the portal by writing AssistantTurns rows, "
        "with input/output token usage and per-model cost tracked for every turn against AiModels and "
        "ModelPricingVersions. A representative asks 'what's my deadline on TC-2026-0002?' and gets a grounded answer "
        "plus a logged cost. Every turn is auditable and billable down to the token.",
        "TT-ASSISTANT-01,TT-ASSISTANT-02", SF_DOMAIN, "assistant"),
    "glossary": (
        "Presents plain-language definitions of every domain term -- citation, jurisdiction, contest, collections, "
        "points -- as GlossaryTerms rows grouped by GlossaryCategories. Any user clicks a term in the UI and reads "
        "what it means in this platform. The glossary is the canonical vocabulary that features and rules are written against.",
        "TT-GLOSSARY-01", SF_DOMAIN, "glossary"),
    "business-rules-browser": (
        "Browse the full set of numbered business rules (the TT-* library) grouped by BusinessRuleCategories, each row "
        "linked via SchemaLocation to exactly where in the schema it is enforced. A representative or manager looks up "
        "why a deadline or penalty applies and jumps straight to the governing rule. It reads BusinessRules and "
        "BusinessRuleCategories rows and is the human-facing index of the rule library.",
        "TT-PLATFORM-02,TT-GLOSSARY-01", SF_BOTH, "business-rules"),
    "jurisdiction-library": (
        "Browses the source documents (appeal-board decisions, statutes) and AI-distilled rule summaries backing each "
        "jurisdiction, stored as JurisdictionSourceDocuments and ReferenceDocuments rows linked to Jurisdictions. "
        "A representative researching a contest reads the actual authority behind a deadline or penalty. "
        "It closes the loop between the rules-as-data engine and the legal text that justifies each row.",
        "TT-RULES-02,TT-GLOSSARY-01", SF_DOMAIN, "jurisdiction-library"),
    "multi-jurisdiction-dashboard": (
        "Aggregates citation volume, contest rates, payment status, and average points by jurisdiction into a "
        "dashboard reading from vw_citations and vw_drivers across all Jurisdictions rows. A manager compares CA-LA, "
        "NY-NYC, and TX-AUS at a glance and spots where deadlines or penalties are biting. Every tile is a live "
        "aggregation, never a cached snapshot.",
        "TT-RULES-01,TT-QUEUE-01", SF_BOTH, "dashboard"),
    # ---- access-control / core / platform-meta / state-machines / assistant ----
    "core-roles-vocabulary": (
        "Defines the canonical four-role vocabulary (admin, manager, representative, external-llm) as Roles rows that "
        "every permission, navigation, and redaction decision references. An admin manages the role catalog from one "
        "screen and the rest of the platform inherits it. There is exactly one source of truth for who the actors are.",
        "TT-RBAC-01", SF_PLATFORM, "roles"),
    "core-role-permission-model": (
        "Encodes the Role -> Table -> Field CRUD permission model as data so that what each role may create, read, "
        "update, or delete is a row, not a hardcoded guard. An admin edits a grid and the enforced CRUD API changes "
        "behavior immediately. This declarative matrix is the backbone of the platform's RBAC.",
        "TT-RBAC-01,TT-RBAC-02", SF_PLATFORM, "permissions"),
    "core-table-where-policies": (
        "Stores a per-role row-scoping WHERE policy per table so a representative sees only their own citations while "
        "an admin sees all, expressed as data the enforced API compiles into vw_* reads. An admin authors the policy "
        "once and every query for that role is filtered. Row-level security becomes configuration, not code.",
        "TT-RBAC-02,TT-RBAC-03", SF_PLATFORM, "permissions"),
    "core-role-specific-schemas": (
        "Generates role-specific, column-limited projections of each table so external-llm receives a redacted schema "
        "while admin gets the full set, derived from the field-level CRUD grid. A developer sees exactly the columns a "
        "role is entitled to. The same rulebook yields a different, safe view per audience.",
        "TT-RBAC-02,TT-RBAC-04", SF_PLATFORM, "tables"),
    "core-enforced-crud-api": (
        "Provides the single enforced CRUD API that reads the Role -> Table -> Field permission rows and the WHERE "
        "policies before every operation, so no caller can bypass RBAC. Every app screen and every external client "
        "goes through it. It is the one door, and the door checks the rulebook.",
        "TT-RBAC-01,TT-RBAC-02,TT-RBAC-03", SF_PLATFORM, "endpoints"),
    "core-rls-plan": (
        "Compiles the per-role WHERE policies into a derived row-level-security plan that the enforced API applies to "
        "every vw_* read, turning declarative scoping rows into actual filtered results. An admin reviews the plan to "
        "confirm a representative truly cannot see another rep's citations. The plan is regenerated from the rulebook, "
        "never hand-maintained.",
        "TT-RBAC-03", SF_PLATFORM, "permissions"),
    "core-api-endpoints": (
        "Maintains a registry of every custom (non-CRUD) API endpoint as APIEndpoints rows -- method, path, subject "
        "table, role visibility, and which state machine it triggers. An admin browses the verb surface of the whole "
        "platform in one place. Loop 4 populates this registry with every traffic-ticket action.",
        "TT-PLATFORM-01,TT-RBAC-01", SF_PLATFORM, "endpoints"),
    "core-schema-catalog": (
        "Exposes a self-describing schema catalog built from ERBTables and ERBFields rows so the platform can render "
        "its own data model without external documentation. A developer inspects any table's fields, types, and "
        "descriptions in the UI. The catalog stays in sync because it is generated from the rulebook itself.",
        "TT-PLATFORM-02", SF_PLATFORM, "tables"),
    "core-state-machine-backbone": (
        "Provides the generic state-machine backbone -- StateMachines, MachineStates, StateTransitionRules, "
        "StateTransitions, SubjectStateInstances -- that the four traffic-ticket tracks are expressed in as data. "
        "An admin defines states and guarded transitions as rows and the engine enforces them. Every lifecycle in the "
        "platform rides this one substrate.",
        "TT-CITATION-01,TT-PLATFORM-01", SF_PLATFORM, "state-machines"),
    "core-work-queue": (
        "Implements the unified work queue as WorkQueueItems rows computed from deadline and status columns across "
        "vw_citations, vw_hearings, and vw_payments. Any role sees the prioritized list scoped to what they may act "
        "on. It is the single 'what needs me now' surface for the whole platform.",
        "TT-QUEUE-01,TT-DEADLINE-01", SF_PLATFORM, "work-queue"),
    "core-feature-status-tracking": (
        "Tracks the build status of every capability via ERBFeatures.Status (FK to ERBFeatureStatuses), so the "
        "feature catalog shows designed/planned/in-progress/shipped at a glance. A product owner sees exactly what is "
        "live. This very spine row is part of that catalog.",
        "TT-PLATFORM-02", SF_PLATFORM, "features"),
    "core-build-pipeline": (
        "Drives the rulebook-first build pipeline: editing the rulebook JSON and running effortless build regenerates "
        "Postgres, Python, explain-dag, and rulespeak with no hand-written substrate code. A developer changes a rule "
        "and rebuilds; every spoke updates. The pipeline is the mechanism that makes the SSoT actually single.",
        "TT-PLATFORM-01,TT-PLATFORM-03", SF_BOTH, "build-pipeline"),
    "core-magic-link-auth": (
        "Authenticates users with passwordless email magic links configured in MagicLinkConfig, resolving each "
        "request to an AppUsers identity and role. A user signs in with a code and the platform knows who they are. "
        "Identity feeds every RBAC and row-scoping decision downstream.",
        "TT-AUTH-01,TT-RBAC-01", SF_PLATFORM, "users"),
    "core-roles-catalog-admin": (
        "Gives admins a roles catalog with a per-role dashboard summarizing each role's table/field grants and visible "
        "navigation, reading Roles, the permission grid, and PlatformNaviation. An admin audits a role's full reach "
        "from one screen. It turns the abstract RBAC matrix into a legible per-role story.",
        "TT-RBAC-01,TT-RBAC-02", SF_PLATFORM, "roles"),
    "core-app-users-directory": (
        "Provides the App-Users directory backed by the AppUsers table for creating, editing, and assigning roles to "
        "users. An admin manages who can log in and what role they hold. Every identity the magic-link flow resolves "
        "lives here.",
        "TT-AUTH-01,TT-RBAC-01", SF_PLATFORM, "users"),
    "core-admin-equivalent-access": (
        "Grants admin (and admin-equivalent) roles cross-world access so they can read and act across every "
        "jurisdiction and every representative's caseload, enforced via the WHERE policies. An admin troubleshoots any "
        "citation regardless of owner. The breadth is itself a configured policy, not a hardcoded superuser bypass.",
        "TT-RBAC-03,TT-RBAC-01", SF_PLATFORM, "permissions"),
    "core-role-url-worlds": (
        "Routes each role into its own URL world (/admin, /reps, /llm) so navigation, scoping, and chrome match the "
        "actor, driven by PlatformNaviation RoleVisibility. A representative lands in /reps and sees only their tools. "
        "The same rulebook produces a tailored app per role.",
        "TT-RBAC-04,TT-NAV-01", SF_PLATFORM, "route-designer"),
    "core-role-impersonation": (
        "Lets an admin view-as-role to see precisely what a representative or external-llm would see, applying that "
        "role's permission and WHERE policies live. An admin verifies redaction and scoping by walking in the role's "
        "shoes. It is a debugging superpower that never weakens the real policies.",
        "TT-RBAC-04,TT-AUTH-01", SF_PLATFORM, "users"),
    "core-default-crud-editor": (
        "Provides a per-role default CRUD editor that sets the baseline create/read/update/delete a role gets on a "
        "table before field-level overrides, stored as permission rows. An admin sets sane defaults once. "
        "Field grids then refine the exceptions.",
        "TT-RBAC-02", SF_PLATFORM, "permissions"),
    "core-table-where-policy-editor": (
        "A per-table CRUD + WHERE policy editor where an admin authors the row-scoping expression that filters a "
        "role's reads of that table's vw_*. An admin writes 'driver = current_user' once for representatives. "
        "The enforced API compiles it on every query.",
        "TT-RBAC-03,TT-RBAC-02", SF_PLATFORM, "permissions"),
    "core-field-crud-grid": (
        "A per-field CRUD permission grid that controls, column by column, what each role may read or write across "
        "every table, persisted as permission rows that the role-specific schemas derive from. An admin redacts a PII "
        "column for external-llm with one toggle. Granularity is data, not branching code.",
        "TT-RBAC-02,TT-RBAC-04", SF_PLATFORM, "permissions"),
    "core-redaction-sweep": (
        "Runs a role-wide PII redaction sweep that flags every sensitive field (driver address, license number) and "
        "ensures the field grid denies it to lower-trust roles. An admin confirms no PII leaks to external-llm in one "
        "pass. The sweep reads the field catalog and the grid together.",
        "TT-RBAC-04", SF_PLATFORM, "permissions"),
    "core-redacted-projection": (
        "Produces the redacted-response projection the enforced API returns to a role, stripping denied columns from "
        "every vw_* result before it leaves the server. An external-llm receives a citation with PII columns absent, "
        "not nulled-after-the-fact. Redaction happens at projection time, deterministically.",
        "TT-RBAC-04", SF_PLATFORM, "endpoints"),
    "core-derived-rls": (
        "Enforces derived row-level security by applying the compiled WHERE-policy plan to every read, so a "
        "representative's query of vw_citations is silently scoped to their own rows. A rep simply cannot fetch "
        "another rep's data. The enforcement is generated from the rulebook, audited, and uniform.",
        "TT-RBAC-03", SF_PLATFORM, "permissions"),
    "core-live-identity": (
        "Resolves live per-request identity from the magic-link session to an AppUsers row and role on every call, "
        "feeding RBAC and scoping with the actual current actor. The platform always knows who is asking right now. "
        "Stale or assumed identity is never used.",
        "TT-AUTH-01,TT-RBAC-01", SF_PLATFORM, "users"),
    "core-route-designer": (
        "Offers a drag-and-drop Route Designer that edits the PlatformNaviation tree -- parent/child, level, "
        "role-visibility, pin-to-top -- as data that the live sidebar renders. An admin reorganizes the whole app's "
        "navigation without touching code. The nav tree is rulebook rows end to end.",
        "TT-NAV-01,TT-NAV-02", SF_PLATFORM, "route-designer"),
    "core-live-sidebar": (
        "Renders a live DB-driven sidebar straight from PlatformNaviation rows filtered by the current role's "
        "RoleVisibility. A user's menu always matches the rulebook and their permissions. There is no hand-maintained "
        "menu file to drift.",
        "TT-NAV-01,TT-RBAC-04", SF_PLATFORM, "route-designer"),
    "core-pin-to-top": (
        "Supports pin-to-top routes via a PlatformNaviation.PinToTop flag so high-traffic screens (Work Queue, "
        "Citations) float above the long tail. An admin pins the screens reps use most. Ordering is data the sidebar "
        "honors.",
        "TT-NAV-02", SF_PLATFORM, "route-designer"),
    "core-route-galleries": (
        "Generates By-Role and By-Package route galleries from PlatformNaviation so an admin can audit the app from "
        "either lens -- what each role sees, or what each package contributes. An admin reviews coverage gaps "
        "visually. Both galleries are projections of the same nav rows.",
        "TT-NAV-01,TT-PLATFORM-02", SF_PLATFORM, "route-designer"),
    "core-route-access-gating": (
        "Gates each route by RoleVisibility so navigating to a screen a role may not see returns a clean denial "
        "instead of a broken page, enforced from PlatformNaviation. A representative who edits the URL still cannot "
        "reach an admin screen. Access is checked against the rulebook on every navigation.",
        "TT-NAV-01,TT-RBAC-04", SF_PLATFORM, "route-designer"),
    "core-nav-backed-pages": (
        "Renders generic nav-backed pages: any PlatformNaviation row with a PrimaryTable/PrimaryView gets a working "
        "list-or-detail screen with no bespoke component, reading vw_* directly. An admin adds a screen by adding a "
        "nav row. The frontend is a renderer of rulebook data.",
        "TT-NAV-01,TT-NAV-02", SF_PLATFORM, "route-designer"),
    "core-dev-quick-connect": (
        "Provides a dev quick-connect login that authenticates as any seeded AppUsers role for local testing without "
        "the full magic-link round trip. A developer hops between admin and representative in one click. It is a "
        "convenience that still resolves a real identity and real policies.",
        "TT-AUTH-01", SF_PLATFORM, "users"),
    "core-unlicensed-reveal": (
        "Reveals unlicensed surfaces in a muted state -- features, tables, or routes whose ERBPackage is not licensed "
        "are shown as upgrade prompts rather than hidden -- driven by the IsLicensed flags. A buyer sees what the next "
        "tier unlocks. Licensing is presentation data, not a deleted feature.",
        "TT-LICENSE-PKG-01", SF_PLATFORM, "packages"),
    "core-save-to-rulebook": (
        "Implements SAVE: edits made in the admin UI write back to the rulebook JSON and trigger a rebuild, so the DB "
        "-> rulebook -> rebuild loop closes from inside the app. An admin changes a deadline and the rulebook plus all "
        "substrates update. The app edits its own source of truth.",
        "TT-PLATFORM-01,TT-PLATFORM-03", SF_BOTH, "build-pipeline"),
    "core-drift-guard": (
        "Adds a rulebook drift guard that detects when the live DB diverges from the rulebook JSON and surfaces it "
        "before a rebuild silently reverts edits. An admin is warned instead of losing work. The guard protects the "
        "SSoT from accidental clobber.",
        "TT-PLATFORM-03", SF_PLATFORM, "build-pipeline"),
    "core-config-export-scope": (
        "Lets an admin choose the config-export scope -- which packages, tables, and routes ship in an exported "
        "rulebook -- so a tenant gets exactly the licensed subset. An admin exports a representative-only build. "
        "Scope is selected from the catalog, not edited by hand.",
        "TT-PLATFORM-03,TT-LICENSE-PKG-01", SF_PLATFORM, "build-pipeline"),
    "core-permission-sync": (
        "Performs non-destructive permission sync: when new tables or fields appear, default grants are added without "
        "overwriting an admin's existing customizations in the permission grid. An admin upgrades schema and keeps "
        "their RBAC edits. Sync is additive by design.",
        "TT-RBAC-02,TT-PLATFORM-03", SF_PLATFORM, "permissions"),
    "core-endpoint-access-editor": (
        "A per-endpoint access editor that sets RoleVisibility on each APIEndpoints row, so a contest action is "
        "available to representatives but a refund only to managers. An admin tunes who can fire which verb. "
        "Endpoint security is a row, edited in the UI.",
        "TT-RBAC-01,TT-PLATFORM-01", SF_PLATFORM, "endpoints"),
    "core-package-toggle": (
        "Enables or disables whole ERBPackages via an IsActive toggle, lighting up or dimming every feature, table, "
        "and route under that package at once. An admin turns the assistant package off for a tenant. The toggle "
        "cascades through the catalog.",
        "TT-LICENSE-PKG-01", SF_PLATFORM, "packages"),
    "core-package-licensing": (
        "Controls per-package licensing with an IsLicensed flag on ERBPackages that the unlicensed-reveal surface "
        "reads to gate paid capabilities. An admin sells the rules-engine package as a tier. Licensing state is data "
        "the whole UI respects.",
        "TT-LICENSE-PKG-01", SF_PLATFORM, "packages"),
    "core-feature-licensing": (
        "Controls per-feature licensing via ERBFeatures.IsLicensed so individual capabilities can be gated even within "
        "a licensed package. An admin offers the AI assistant as an add-on. Granular licensing is a per-row flag.",
        "TT-LICENSE-PKG-01", SF_PLATFORM, "features"),
    "core-table-route-licensing": (
        "Adds per-table and per-route licensing toggles (IsLicensed on ERBTables and PlatformNaviation) so a tier can "
        "expose a screen while withholding the underlying editable table. An admin shows read-only jurisdictions to a "
        "lower tier. Every surface has its own license switch.",
        "TT-LICENSE-PKG-01", SF_PLATFORM, "packages"),
    "core-table-catalog": (
        "Renders the self-describing table catalog from ERBTables rows -- name, package, field count, catalog flag, "
        "per-role CRUD -- so the platform documents its own schema. A developer browses every table and its "
        "governance from one screen. The catalog is generated, never written by hand.",
        "TT-PLATFORM-02", SF_PLATFORM, "tables"),
    "core-fields-catalog": (
        "Renders the fields catalog from ERBFields rows -- table, field, type, datatype, description -- the "
        "column-level companion to the table catalog. A developer inspects exactly how each calculated or lookup field "
        "is defined. It is the field-level self-description of the rulebook.",
        "TT-PLATFORM-02", SF_PLATFORM, "tables"),
    "core-explainer-dag": (
        "Builds the Explainer DAG that visualizes calculated-field provenance -- click any computed cell and trace raw "
        "inputs -> lookups -> calcs -> aggregations -- generated from the rulebook's formula graph. A user understands "
        "exactly how ResponseDeadline or ActivePoints was derived. It makes the inference graph witnessable.",
        "TT-PLATFORM-02", SF_BOTH, "explainer-dag"),
    "core-feature-catalog-grid": (
        "Presents the feature catalog as a filterable grid of cards from ERBFeatures, each with title, status, "
        "package, and cartoon logo. A product owner filters by status or package to see the platform's surface. "
        "This is the home of the very rows Loop 1 enriches.",
        "TT-PLATFORM-02", SF_PLATFORM, "features"),
    "core-feature-detail-editor": (
        "A feature detail editor for one ERBFeatures row -- title, description, SourceText, RuleRefs, status, route, "
        "image -- writing back to the rulebook. An admin documents a capability in place. The editor maintains the "
        "provenance spine this loop seeds.",
        "TT-PLATFORM-02,TT-PLATFORM-01", SF_PLATFORM, "features"),
    "core-feature-vocab-editors": (
        "Provides editors for the feature vocabulary -- ERBFeatureStatuses and ERBFeatureCategories rows -- so the "
        "controlled lists behind the catalog are themselves editable data. An admin renames a status or adds a "
        "category. The taxonomy is rulebook-managed.",
        "TT-PLATFORM-02", SF_PLATFORM, "features"),
    "core-feature-logo-system": (
        "Runs the cartoon feature-logo system that attaches a generated ImageUrl to each ERBFeatures row for the "
        "catalog grid. An admin regenerates a logo and the card updates. Imagery is a per-feature data attribute.",
        "TT-PLATFORM-02", SF_PLATFORM, "features"),
    "core-glossary-admin": (
        "Gives admins glossary administration over GlossaryTerms and GlossaryCategories rows so the domain vocabulary "
        "stays current and complete. An admin adds 'collections' the moment a rule references it. The glossary is "
        "edited as first-class data.",
        "TT-GLOSSARY-01", SF_PLATFORM, "glossary"),
    "core-business-rule-admin": (
        "Provides business-rule library administration over BusinessRules and BusinessRuleCategories rows -- the "
        "numbered TT-* rules that features cite via RuleRefs. An admin authors a rule and links its SchemaLocation. "
        "Loop 2 fills this library so every RuleRef resolves.",
        "TT-PLATFORM-02", SF_PLATFORM, "business-rules"),
    "core-site-branding": (
        "A site-branding editor over the SiteBranding row -- company name, colors, logos, contact info, OG tags -- so "
        "the portal's identity is data. An admin rebrands the whole app from one screen. Branding is configuration, "
        "not CSS edits.",
        "TT-PLATFORM-02", SF_PLATFORM, "branding"),
    "core-global-ui-settings": (
        "Exposes global UI settings (theme, density, defaults) backed by branding and meta rows that the frontend "
        "reads at boot. An admin tunes the look and feel for every user. Settings are stored, not compiled.",
        "TT-PLATFORM-02", SF_PLATFORM, "branding"),
    "core-state-machine-admin": (
        "Offers visual state-machine administration -- view and edit StateMachines, MachineStates, and "
        "StateTransitionRules rows -- for the four traffic-ticket tracks. An admin adds a guarded transition as data. "
        "Loop 5 completes these machines.",
        "TT-CITATION-01,TT-PLATFORM-02", SF_PLATFORM, "state-machines"),
    "core-claim-lifecycle-sim": (
        "Provides an entity lifecycle simulator that walks a sample citation through its state machine -- firing "
        "transitions, checking guards, logging StateTransitions -- to validate the model before real data. A developer "
        "dry-runs the citation lifecycle. It proves the machine is well-formed.",
        "TT-CITATION-01,TT-CITATION-02", SF_PLATFORM, "state-machines"),
    "as-portal-assistant": (
        "The context-aware portal assistant chats about the current screen, citation, or rule, logging each exchange "
        "as AssistantTurns rows with model and cost. A user asks the assistant about whatever they are looking at. "
        "It grounds answers in the live vw_* data and the rulebook.",
        "TT-ASSISTANT-01,TT-ASSISTANT-02", SF_PLATFORM, "assistant"),
    "as-cost-dashboard": (
        "A cost-management dashboard aggregating AssistantTurns by model and day against ModelPricingVersions to show "
        "spend and token volume. A manager watches assistant cost trend over time. Every figure is a live aggregation "
        "of logged turns.",
        "TT-ASSISTANT-02", SF_PLATFORM, "assistant-costs"),
    "as-cost-telemetry": (
        "Captures per-turn cost telemetry on each AssistantTurns row -- input tokens, output tokens, model, computed "
        "cost via ModelPricingVersions -- so spend is attributable to the turn. A developer audits a single expensive "
        "exchange. Cost is a stored, derived fact per turn.",
        "TT-ASSISTANT-02", SF_PLATFORM, "assistant-costs"),
    "as-ocr-cost-estimator": (
        "An OCR cost estimator that projects the token and dollar cost of processing a batch of citation images "
        "before running it, using AiModels and ModelPricingVersions rates. A manager budgets an intake batch up front. "
        "The estimate uses the same pricing rows that bill real turns.",
        "TT-ASSISTANT-02", SF_PLATFORM, "assistant-costs"),
}

# Route keys here are the lower-kebab nav RouteKeys Loop 3 will create.


def main():
    rb = json.load(open(RB_PATH))
    feats = rb["ERBFeatures"]["data"]

    valid_pkgs = {p["ERBPackageId"] for p in rb["ERBPackages"]["data"]}
    valid_cats = {c["ERBFeatureCategoryId"] for c in rb["ERBFeatureCategories"]["data"]}
    valid_stat = {s["ERBFeatureStatusId"] for s in rb["ERBFeatureStatuses"]["data"]}

    filled_st = added_rr = added_sf = added_route = 0
    rule_codes = set()
    route_keys = set()
    missing_in_spine = []

    for f in feats:
        fid = f["ERBFeatureId"]
        spec = SPINE.get(fid)
        if not spec:
            missing_in_spine.append(fid)
            # still record any pre-existing rulerefs/routes
            for code in (f.get("RuleRefs") or "").split(","):
                if code.strip():
                    rule_codes.add(code.strip())
            if (f.get("Route") or "").strip():
                route_keys.add(f["Route"].strip())
            continue
        st, rr, sf, route = spec
        # merge-by-id: only fill empties / weak values
        cur_st = (f.get("SourceText") or "").strip()
        if not cur_st or cur_st.count(".") < 2:
            f["SourceText"] = st
            filled_st += 1
        cur_rr = (f.get("RuleRefs") or "").strip()
        # Overwrite empty refs OR legacy R#-style refs with the curated TT-* set
        # (these rows all have hand-authored spine entries, so this is intentional,
        # not a clobber of unrelated work). A row already on the TT-* scheme is kept.
        is_legacy = bool(re.fullmatch(r"(R\d+)(,R\d+)*", cur_rr))
        if not cur_rr or is_legacy:
            f["RuleRefs"] = rr
            added_rr += 1
        if not (f.get("SourceFiles") or "").strip():
            f["SourceFiles"] = sf
            added_sf += 1
        # Route is an ENFORCED FK -> PlatformNaviation in the live DB. Those nav
        # rows are created in Loop 3, so setting Route here makes the deferred
        # fk_erb_features_route constraint fail to apply at build time. We DEFER
        # Route assignment to Loop 3 (which has the curated mapping below too).
        # Record the intended route key for Loop 3, but do NOT write it now; and
        # idempotently strip any forward-ref route a prior run of this script set.
        if route:
            route_keys.add(route)
        # The transpiler emits the deferred fk_erb_features_route FK against
        # platform_naviation. An ABSENT Route key -> NULL column -> FK passes.
        # An empty-string Route -> '' column -> FK fails (no PK ''). So we must
        # fully REMOVE any Route key on features (Loop 3 re-adds it with a real
        # nav PK, e.g. 'nav-citations-detail').
        if "Route" in f and not (f.get("Route") or "").strip():
            del f["Route"]
            added_route += 1
        audit(f)
        # collect codes from the (now-final) row (route keys collected above)
        for code in (f.get("RuleRefs") or "").split(","):
            if code.strip():
                rule_codes.add(code.strip())

    # ---- consistency sweep ----
    empty_st = [f["ERBFeatureId"] for f in feats
                if not (f.get("SourceText") or "").strip()
                or (f.get("SourceText") or "").count(".") < 2]
    empty_rr = [f["ERBFeatureId"] for f in feats if not (f.get("RuleRefs") or "").strip()]
    bad_fk = []
    for f in feats:
        if f.get("ERBPackage") not in valid_pkgs:
            bad_fk.append(("ERBPackage", f["ERBFeatureId"], f.get("ERBPackage")))
        if f.get("Category") not in valid_cats:
            bad_fk.append(("Category", f["ERBFeatureId"], f.get("Category")))
        if f.get("Status") and f.get("Status") not in valid_stat:
            bad_fk.append(("Status", f["ERBFeatureId"], f.get("Status")))

    print("=== loop-01 feature-spine ===")
    print("features total      :", len(feats))
    print("SourceText filled   : +%d" % filled_st)
    print("RuleRefs filled     : +%d" % added_rr)
    print("SourceFiles filled  : +%d" % added_sf)
    print("Route stripped(defer): %d  (Route is set in Loop 3, not here)" % added_route)
    print("not in SPINE map    :", len(missing_in_spine), missing_in_spine)
    print("--- consistency residuals (gate: all 0) ---")
    print("empty/weak SourceText:", len(empty_st), empty_st)
    print("empty RuleRefs       :", len(empty_rr), empty_rr)
    print("bad FK (pkg/cat/stat):", len(bad_fk), bad_fk)
    print("--- forward references for later loops ---")
    print("distinct RuleRef codes (-> Loop 2):", len(rule_codes))
    print("  " + ",".join(sorted(rule_codes)))
    print("distinct Route keys (-> Loop 3):", len(route_keys))
    print("  " + ",".join(sorted(route_keys)))

    if bad_fk:
        print("\nABORT: bad FK -- not writing.")
        sys.exit(1)
    if missing_in_spine:
        print("\nABORT: feature(s) absent from SPINE map -- add them before writing.")
        sys.exit(1)
    if empty_st or empty_rr:
        print("\nABORT: residuals remain -- not writing.")
        sys.exit(1)

    if DRY:
        print("\n[dry-run] no write")
        return
    with open(RB_PATH, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB_PATH)


if __name__ == "__main__":
    main()
