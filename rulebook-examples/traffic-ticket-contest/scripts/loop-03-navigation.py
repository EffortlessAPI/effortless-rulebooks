#!/usr/bin/env python3
"""
loop-03-navigation.py  (idempotent)

Loop 3: extend the EXISTING dotted PlatformNaviation tree into the complete app
route map, fill RoleVisibility on every nav row, and WIRE every ERBFeatures.Route
to a real nav PK (the deferred fk_erb_features_route FK from Loop 1).

Verified facts:
- nav PK = "nav-" + RouteKey with dots->dashes (RouteKey "citations.detail"
  -> PK "nav-citations-detail").
- ERBFeatures.Route is an FK -> platform_naviation(platform_naviation_id); a
  feature's Route value must be a nav PK. Absent/NULL is OK; '' is NOT.
- ParentRouteKey references another row's RouteKey (dotted), not its PK.
- PrimaryView must equal "vw_" + snake(PrimaryTable).
- ERBPackage is an FK -> erb_packages.

We EXTEND the existing 22-row dotted scheme (no parallel flat tree), so the final
nav is one coherent hierarchy.

Run:
  python3 scripts/loop-03-navigation.py --dry-run
  python3 scripts/loop-03-navigation.py
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

ALL_ROLES = "admin,manager,representative,external-llm"
NO_LLM = "admin,manager,representative"
ADMIN_MGR = "admin,manager"
ADMIN_ONLY = "admin"


def audit(row):
    row.setdefault("CreatedAt", NOW)
    row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW)
    row.setdefault("ModifiedBy", WHO)
    return row


def pk(routekey):
    return "nav-" + routekey.replace(".", "-")


def snake(name):
    # Acronym-aware: matches the transpiler's vw_<name> rule exactly
    # (ERBTables -> erb_tables, APIEndpoints -> api_endpoints).
    s = re.sub(r"(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", "_", name)
    return s.lower()


# Backfill RoleVisibility onto EXISTING rows (keyed by their RouteKey).
EXISTING_VIS = {
    "dashboard": ALL_ROLES, "citations": ALL_ROLES, "citations.detail": ALL_ROLES,
    "hearings": NO_LLM, "payments": NO_LLM, "drivers": ALL_ROLES,
    "jurisdictions": ALL_ROLES, "work-queue": NO_LLM, "assistant": NO_LLM,
    "library": ALL_ROLES, "library.glossary": ALL_ROLES,
    "library.business-rules": ALL_ROLES, "library.jurisdiction-rules": ALL_ROLES,
    "library.jurisdiction-docs": ALL_ROLES,
    "admin": ADMIN_ONLY, "admin.features": ADMIN_ONLY,
    "admin.feature-detail": ADMIN_ONLY, "admin.tables": ADMIN_ONLY,
    "admin.navigation": ADMIN_ONLY, "admin.audit-log": ADMIN_MGR,
    "admin.branding": ADMIN_ONLY, "admin.versions": ADMIN_ONLY,
}

# New nav rows. Tuple positions:
# (RouteKey, DisplayName, Route, Parent, NavLevel, PrimaryTable, RoleVis, Icon,
#  BuildPhase, Status, ERBPackage, repCRUD, llmCRUD, Pin, Dyn, ReqCtx, RuleRefs, Desc)
NEW_NAV = [
    ("violation-types", "Violation Types", "/violation-types", "", "top",
     "ViolationTypes", ALL_ROLES, "list-tree", 2, "shipped", "citations", "R", "R",
     False, False, False, "TT-FEE-01",
     "Catalog of violation codes with base fines and point values."),
    ("case-events", "Case Events", "/case-events", "", "top", "CaseEvents", NO_LLM,
     "history", 2, "shipped", "citations", "R", "", False, False, False,
     "TT-AUDIT-01", "The append-only lifecycle event log across all citations."),
    ("citations.contest", "Contest a Citation", "/citations/:citationId/contest",
     "citations.detail", "sub", "Citations", NO_LLM, "gavel", 3, "shipped",
     "citations", "RU", "", False, True, True, "TT-CONTEST-01,TT-CONTEST-02",
     "Request a hearing to contest a citation before its deadline."),
    ("citations.pay", "Pay a Citation", "/citations/:citationId/pay",
     "citations.detail", "sub", "Payments", NO_LLM, "credit-card", 3, "shipped",
     "citations", "RU", "", False, True, True, "TT-PAYMENT-01,TT-PAYMENT-03",
     "Pay a citation's fine online and close the case."),
    ("library.violation-types", "Violation Catalog", "/library/violation-types",
     "library", "sub", "ViolationTypes", ALL_ROLES, "list-tree", 5, "shipped",
     "citations", "R", "R", False, False, False, "TT-FEE-01",
     "Reference view of every violation code, fine, and point value."),
    ("assistant.costs", "Assistant Costs", "/assistant/costs", "assistant", "sub",
     "AssistantTurns", ADMIN_MGR, "dollar-sign", 5, "in-progress", "assistant", "",
     "", False, False, False, "TT-ASSISTANT-02",
     "Per-model assistant spend and token volume over time."),
    ("assistant.models", "AI Models", "/assistant/models", "assistant", "sub",
     "AiModels", ADMIN_ONLY, "cpu", 5, "shipped", "assistant", "", "", False,
     False, False, "TT-ASSISTANT-02",
     "The AI models and pricing versions the assistant can use."),
    ("admin.roles", "Roles", "/admin/roles", "admin", "sub", "Roles", ADMIN_ONLY,
     "users", 5, "shipped", "access-control", "", "", False, False, False,
     "TT-RBAC-01", "The canonical four roles and their summaries."),
    ("admin.users", "Users", "/admin/users", "admin", "sub", "AppUsers",
     ADMIN_ONLY, "user-cog", 5, "shipped", "access-control", "", "", False, False,
     False, "TT-AUTH-01,TT-RBAC-01",
     "App-users directory: create users and assign roles."),
    ("admin.permissions", "Permissions", "/admin/permissions", "admin", "sub",
     "ERBFields", ADMIN_ONLY, "shield-check", 5, "shipped", "access-control", "",
     "", False, False, False, "TT-RBAC-02,TT-RBAC-03,TT-RBAC-04",
     "Per-role, per-table, per-field CRUD and row-scoping policy editor."),
    ("admin.packages", "Packages", "/admin/packages", "admin", "sub",
     "ERBPackages", ADMIN_ONLY, "package", 5, "shipped", "platform-meta", "", "",
     False, False, False, "TT-LICENSE-PKG-01",
     "Enable/disable and license whole packages of features and tables."),
    ("admin.fields", "Fields", "/admin/fields", "admin", "sub", "ERBFields",
     ADMIN_ONLY, "columns", 5, "shipped", "platform-meta", "", "", False, False,
     False, "TT-PLATFORM-02",
     "Self-describing fields catalog: every column, type, and description."),
    ("admin.route-designer", "Route Designer", "/admin/route-designer", "admin",
     "sub", "PlatformNaviation", ADMIN_ONLY, "route", 5, "in-progress", "core", "",
     "", False, False, False, "TT-NAV-01,TT-NAV-02",
     "Drag-and-drop editor for the navigation tree and per-route access."),
    ("admin.endpoints", "API Endpoints", "/admin/endpoints", "admin", "sub",
     "APIEndpoints", ADMIN_ONLY, "webhook", 5, "shipped", "platform-meta", "", "",
     False, False, False, "TT-PLATFORM-01,TT-RBAC-01",
     "Registry of every custom (non-CRUD) API endpoint and its access."),
    ("admin.state-machines", "State Machines", "/admin/state-machines", "admin",
     "sub", "StateMachines", ADMIN_ONLY, "git-branch", 5, "shipped",
     "state-machines", "", "", False, False, False, "TT-CITATION-01",
     "View and edit the four lifecycle state machines and transitions."),
    ("admin.explainer-dag", "Explainer DAG", "/admin/explainer-dag", "admin",
     "sub", "ERBFields", ALL_ROLES, "workflow", 5, "shipped", "platform-meta", "R",
     "R", False, False, False, "TT-PLATFORM-02",
     "Click any calculated field to trace its derivation graph."),
    ("admin.build-pipeline", "Build Pipeline", "/admin/build-pipeline", "admin",
     "sub", "ERBVersions", ADMIN_ONLY, "hammer", 5, "shipped", "core", "", "",
     False, False, False, "TT-PLATFORM-01,TT-PLATFORM-03",
     "The rulebook-first build pipeline, version history, and drift guard."),
    ("admin.customizations", "Customizations", "/admin/customizations", "admin",
     "sub", "ERBCustomizations", ADMIN_ONLY, "wrench", 5, "shipped",
     "platform-meta", "", "", False, False, False, "TT-PLATFORM-02",
     "Hand-authored SQL customizations layered onto the generated substrate."),
]

# Per-feature -> nav PK (the feature's screen). PKs reference EXISTING or NEW rows.
FEATURE_NAVPK = {
    "citation-intake": pk("citations.detail"), "driver-response": pk("citations.detail"),
    "citation-statemachine": pk("citations.detail"), "contest-track": pk("citations.detail"),
    "payment-track": pk("payments"), "license-points": pk("drivers"),
    "jurisdiction-rules": pk("jurisdictions"), "hearing-scheduling": pk("hearings"),
    "payment-checkout": pk("citations.pay"), "work-queue": pk("work-queue"),
    "audit-trail": pk("admin.audit-log"), "ai-assistant": pk("assistant"),
    "glossary": pk("library.glossary"), "business-rules-browser": pk("library.business-rules"),
    "jurisdiction-library": pk("library.jurisdiction-docs"),
    "multi-jurisdiction-dashboard": pk("dashboard"),
    "core-roles-vocabulary": pk("admin.roles"), "core-role-permission-model": pk("admin.permissions"),
    "core-table-where-policies": pk("admin.permissions"), "core-role-specific-schemas": pk("admin.tables"),
    "core-enforced-crud-api": pk("admin.endpoints"), "core-rls-plan": pk("admin.permissions"),
    "core-api-endpoints": pk("admin.endpoints"), "core-schema-catalog": pk("admin.tables"),
    "core-state-machine-backbone": pk("admin.state-machines"), "core-work-queue": pk("work-queue"),
    "core-feature-status-tracking": pk("admin.features"), "core-build-pipeline": pk("admin.build-pipeline"),
    "core-magic-link-auth": pk("admin.users"), "core-roles-catalog-admin": pk("admin.roles"),
    "core-app-users-directory": pk("admin.users"), "core-admin-equivalent-access": pk("admin.permissions"),
    "core-role-url-worlds": pk("admin.route-designer"), "core-role-impersonation": pk("admin.users"),
    "core-default-crud-editor": pk("admin.permissions"), "core-table-where-policy-editor": pk("admin.permissions"),
    "core-field-crud-grid": pk("admin.permissions"), "core-redaction-sweep": pk("admin.permissions"),
    "core-redacted-projection": pk("admin.endpoints"), "core-derived-rls": pk("admin.permissions"),
    "core-live-identity": pk("admin.users"), "core-route-designer": pk("admin.route-designer"),
    "core-live-sidebar": pk("admin.route-designer"), "core-pin-to-top": pk("admin.route-designer"),
    "core-route-galleries": pk("admin.route-designer"), "core-route-access-gating": pk("admin.route-designer"),
    "core-nav-backed-pages": pk("admin.route-designer"), "core-dev-quick-connect": pk("admin.users"),
    "core-unlicensed-reveal": pk("admin.packages"), "core-save-to-rulebook": pk("admin.build-pipeline"),
    "core-drift-guard": pk("admin.build-pipeline"), "core-config-export-scope": pk("admin.build-pipeline"),
    "core-permission-sync": pk("admin.permissions"), "core-endpoint-access-editor": pk("admin.endpoints"),
    "core-package-toggle": pk("admin.packages"), "core-package-licensing": pk("admin.packages"),
    "core-feature-licensing": pk("admin.features"), "core-table-route-licensing": pk("admin.packages"),
    "core-table-catalog": pk("admin.tables"), "core-fields-catalog": pk("admin.fields"),
    "core-explainer-dag": pk("admin.explainer-dag"), "core-feature-catalog-grid": pk("admin.features"),
    "core-feature-detail-editor": pk("admin.feature-detail"), "core-feature-vocab-editors": pk("admin.features"),
    "core-feature-logo-system": pk("admin.features"), "core-glossary-admin": pk("library.glossary"),
    "core-business-rule-admin": pk("library.business-rules"), "core-site-branding": pk("admin.branding"),
    "core-global-ui-settings": pk("admin.branding"), "core-state-machine-admin": pk("admin.state-machines"),
    "core-claim-lifecycle-sim": pk("admin.state-machines"), "as-portal-assistant": pk("assistant"),
    "as-cost-dashboard": pk("assistant.costs"), "as-cost-telemetry": pk("assistant.costs"),
    "as-ocr-cost-estimator": pk("assistant.costs"),
}


def main():
    rb = json.load(open(RB_PATH))
    nav = rb["PlatformNaviation"]["data"]
    feats = rb["ERBFeatures"]["data"]
    valid_pkgs = {p["ERBPackageId"] for p in rb["ERBPackages"]["data"]}
    rule_codes = {r["RuleCode"] for r in rb["BusinessRules"]["data"]}
    all_tables = {k for k, v in rb.items() if isinstance(v, dict) and "schema" in v}

    # 0. PURGE the flat-scheme nav rows a prior (crashed) run injected. The
    #    canonical scheme is dotted RouteKey + "nav-" PK. A row whose PK equals
    #    its (non-empty) RouteKey and does NOT start with "nav-" is an artifact;
    #    remove it so we have one coherent tree, not a parallel flat one.
    purged = [r["PlatformNaviationId"] for r in nav
              if r.get("RouteKey")
              and r["PlatformNaviationId"] == r["RouteKey"]
              and not r["PlatformNaviationId"].startswith("nav-")]
    if purged:
        rb["PlatformNaviation"]["data"] = [
            r for r in nav if r["PlatformNaviationId"] not in set(purged)]
        nav = rb["PlatformNaviation"]["data"]

    have_rk = {r["RouteKey"]: r for r in nav}
    have_pk = {r["PlatformNaviationId"] for r in nav}

    # 1. backfill RoleVisibility on existing rows
    vis_filled = 0
    for r in nav:
        rk = r.get("RouteKey")
        if rk in EXISTING_VIS and not (r.get("RoleVisibility") or "").strip():
            r["RoleVisibility"] = EXISTING_VIS[rk]
            vis_filled += 1

    # 2. add new nav rows
    nav_added = 0
    for (rk, disp, route, parent, lvl, ptbl, vis, icon, bp, status, epkg, repc,
         llmc, pin, dyn, rcc, rrefs, desc) in NEW_NAV:
        if rk in have_rk:
            continue
        row = audit({
            "PlatformNaviationId": pk(rk), "DisplayName": disp, "Route": route,
            "Description": desc, "SortOrder": 30 + nav_added, "ParentRouteKey": parent,
            "RouteKey": rk, "NavLevel": lvl, "RoleVisibility": vis,
            "PrimaryTable": ptbl, "ERBPackage": epkg, "PrimaryView": "vw_" + snake(ptbl),
            "BusinessRuleRefs": rrefs, "BuildPhase": bp, "Status": status,
            "IsLicensed": True, "RequiresClaimContext": rcc, "PinToTop": pin,
            "IsDynamic": dyn, "IconHint": icon, "AdminCRUD": "CRUD",
            "ManagerCRUD": "CRUD" if epkg == "citations" else "",
            "RepresentativeCRUD": repc, "ExternalLlmCRUD": llmc,
        })
        nav.append(row)
        have_rk[rk] = row
        have_pk.add(pk(rk))
        nav_added += 1

    # 3. wire ERBFeatures.Route -> nav PK
    route_wired = 0
    feat_unmapped = []
    for f in feats:
        navpk = FEATURE_NAVPK.get(f["ERBFeatureId"])
        if not navpk:
            feat_unmapped.append(f["ERBFeatureId"])
            continue
        if navpk not in have_pk:
            feat_unmapped.append((f["ERBFeatureId"], navpk))
            continue
        if f.get("Route") != navpk:
            f["Route"] = navpk
            route_wired += 1
        audit(f)

    # ---- gates ----
    rk_set = set(have_rk)
    bad_feat_fk = [(f["ERBFeatureId"], f.get("Route")) for f in feats
                   if (f.get("Route") or "") and f["Route"] not in have_pk]
    bad_parent = [(r["RouteKey"], r.get("ParentRouteKey")) for r in nav
                  if (r.get("ParentRouteKey") or "") and r["ParentRouteKey"] not in rk_set]
    bad_ptable = [(r["RouteKey"], r.get("PrimaryTable")) for r in nav
                  if (r.get("PrimaryTable") or "") and r["PrimaryTable"] not in all_tables]
    bad_pview = []
    for r in nav:
        pt, pv = r.get("PrimaryTable"), r.get("PrimaryView")
        if pt and pv and pv != "vw_" + snake(pt):
            bad_pview.append((r["RouteKey"], pv, "vw_" + snake(pt)))
    bad_pkg = [(r["RouteKey"], r.get("ERBPackage")) for r in nav
               if r.get("ERBPackage") and r["ERBPackage"] not in valid_pkgs]
    bad_rule = sorted({tok.strip() for r in nav
                       for tok in (r.get("BusinessRuleRefs") or "").split(",")
                       if tok.strip() and tok.strip() not in rule_codes})
    # every nav row has RoleVisibility
    no_vis = [r["RouteKey"] for r in nav if not (r.get("RoleVisibility") or "").strip()]

    print("=== loop-03 navigation ===")
    print("flat-scheme artifacts purged:", len(purged), purged[:8])
    print("RoleVisibility backfilled :", vis_filled)
    print("nav rows added            : +%d (total %d)" % (nav_added, len(nav)))
    print("feature Route wired       :", route_wired,
          "(features with Route now: %d/%d)"
          % (sum(1 for f in feats if f.get("Route")), len(feats)))
    print("--- gates (all 0) ---")
    print("feature Route -> bad nav PK :", len(bad_feat_fk), bad_feat_fk)
    print("features unmapped           :", len(feat_unmapped), feat_unmapped)
    print("bad ParentRouteKey          :", len(bad_parent), bad_parent)
    print("bad PrimaryTable            :", len(bad_ptable), bad_ptable)
    print("bad PrimaryView (!=vw_snake):", len(bad_pview), bad_pview[:10])
    print("bad ERBPackage              :", len(bad_pkg), bad_pkg)
    print("nav BusinessRuleRefs unres  :", len(bad_rule), bad_rule)
    print("nav rows w/o RoleVisibility  :", len(no_vis), no_vis)

    if any([bad_feat_fk, feat_unmapped, bad_parent, bad_ptable, bad_pview,
            bad_pkg, bad_rule, no_vis]):
        print("\nABORT: gate(s) failed -- not writing.")
        sys.exit(1)

    if DRY:
        print("\n[dry-run] no write")
        return
    with open(RB_PATH, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB_PATH)


if __name__ == "__main__":
    main()
