#!/usr/bin/env python3
"""
author-platform-catalog.py  (idempotent)

Seeds the traffic-ticket-contest rulebook's self-describing PLATFORM CATALOG with
the CORE-PLATFORM feature documentation lifted (and genericized) from the richer
source portal `industrial-ui-services-effortless-portal`.

It does NOT move any IUI / unemployment-insurance domain content: only the 5
domain-agnostic packages (core, platform-meta, access-control, state-machines,
assistant) and the 59 features under them, the 8 core feature categories, and a
self-describing ERBTables/ERBFields catalog generated from THIS rulebook's own
table schemas.

Every pass is merge-by-id: existing rows are never mutated, only missing rows /
missing schema keys are added. Re-running is a no-op. The rulebook JSON is the
SSoT; this is a build-time author tool, not a runtime dependency.

Run:  python3 scripts/author-platform-catalog.py [--dry-run]
"""
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
TTC_ROOT = os.path.dirname(HERE)
RB_PATH = os.path.join(
    TTC_ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json"
)
# Source portal rulebook (read-only content source).
SRC_PATH = os.path.normpath(
    os.path.join(
        TTC_ROOT,
        "..", "..", "..",  # rulebook-examples -> effortless-rulebooks -> my-projects
        "industrial-ui-services-effortless-portal",
        "effortless-rulebook", "effortless-rulebook.json",
    )
)

DRY = "--dry-run" in sys.argv

NOW = "2026-06-07T00:00:00Z"
WHO = "admin-example"

# ---------------------------------------------------------------------------
# Static mappings (verified against both rulebooks)
# ---------------------------------------------------------------------------
CORE_PKG_IDS = {"core", "platform-meta", "access-control", "state-machines", "assistant"}

# id -> (SortOrder in TTC, IsKey) ; citations stays at 1 and is untouched.
PKG_META = {
    "core":            (2, True),
    "platform-meta":   (3, True),
    "access-control":  (4, False),
    "state-machines":  (5, False),
    "assistant":       (6, False),
}

# The 8 core categories we add (source category id -> TTC SortOrder).
CORE_CATEGORY_IDS = {
    "platform", "devops", "navigation", "permissions",
    "licensing", "auth", "dashboard", "data-model",
}
CATEGORY_SORT = {
    "platform": 11, "devops": 12, "navigation": 13, "permissions": 14,
    "licensing": 15, "auth": 16, "dashboard": 17, "data-model": 18,
}

# Features remapped off categories we did NOT bring over.
FEATURE_CATEGORY_REMAP = {
    "as-portal-assistant": "platform",  # source category 'integration' is not imported
}

# The 15 KEY (starred) features.
STARRED_FEATURE_IDS = {
    "core-enforced-crud-api", "core-role-permission-model", "core-derived-rls",
    "core-role-specific-schemas", "core-route-designer", "core-live-sidebar",
    "core-table-catalog", "core-fields-catalog", "core-explainer-dag",
    "core-feature-catalog-grid", "core-magic-link-auth", "core-build-pipeline",
    "core-save-to-rulebook", "core-state-machine-backbone", "as-portal-assistant",
}

# table name -> ERBPackage for the self-describing ERBTables catalog.
TABLE_PACKAGE = {
    # access-control
    "AppUsers": "access-control", "Roles": "access-control",
    # assistant
    "AiModels": "assistant", "AssistantTurns": "assistant",
    "ModelPricingVersions": "assistant",
    # core
    "MagicLinkConfig": "core", "PlatformNaviation": "core", "SiteBranding": "core",
    "WorkQueueItems": "core", "ERBPackages": "core",
    # platform-meta
    "APIEndpoints": "platform-meta", "BusinessRules": "platform-meta",
    "BusinessRuleCategories": "platform-meta", "ERBCustomizations": "platform-meta",
    "ERBVersions": "platform-meta", "GlossaryCategories": "platform-meta",
    "GlossaryTerms": "platform-meta", "ERBFeatureCategories": "platform-meta",
    "ERBFeatureStatuses": "platform-meta", "ERBFeatures": "platform-meta",
    "ERBFields": "platform-meta", "ERBTables": "platform-meta",
    "Platforms": "platform-meta", "ReferenceDocuments": "platform-meta",
    # state-machines
    "MachineStates": "state-machines", "StateMachines": "state-machines",
    "StateTransitionRules": "state-machines", "StateTransitions": "state-machines",
    "SubjectStateInstances": "state-machines",
    # domain tables -> existing citations package
    "Citations": "citations", "Drivers": "citations", "Hearings": "citations",
    "Payments": "citations", "CaseEvents": "citations", "Jurisdictions": "citations",
    "JurisdictionRules": "citations", "JurisdictionSourceDocuments": "citations",
    "ViolationTypes": "citations", "AuditLogEntries": "citations",
}

# Tables that are meta/admin/reference (IsCatalog=true). Everything else is a
# live domain entity (false).
CATALOG_TABLES = (
    {  # whole platform-meta layer
        "APIEndpoints", "BusinessRules", "BusinessRuleCategories", "ERBCustomizations",
        "ERBVersions", "GlossaryCategories", "GlossaryTerms", "ERBFeatureCategories",
        "ERBFeatureStatuses", "ERBFeatures", "ERBFields", "ERBTables", "Platforms",
        "ReferenceDocuments", "ERBPackages",
    }
    | {"Roles", "AiModels", "ModelPricingVersions", "AppUsers", "MagicLinkConfig",
       "SiteBranding", "PlatformNaviation", "AuditLogEntries", "WorkQueueItems",
       "AssistantTurns"}
)

# Domain-operational tables get Representative CRUD; everything else Representative R.
OPERATIONAL_TABLES = {
    "Citations", "Drivers", "Hearings", "Payments", "CaseEvents",
    "Jurisdictions", "JurisdictionRules", "JurisdictionSourceDocuments",
    "ViolationTypes", "WorkQueueItems",
}

NOT_A_TABLE = {"$schema", "Name", "Description", "_meta", "__meta__"}

# ---------------------------------------------------------------------------
# Genericization sweep
# ---------------------------------------------------------------------------
# Ordered exact-string renames (table/role identifiers).
EXACT_SUBS = [
    ("IUIFeatureStatuses", "ERBFeatureStatuses"),
    ("IUIFeatureCategories", "ERBFeatureCategories"),
    ("IUIPackages", "ERBPackages"),
    ("IUIFeatures", "ERBFeatures"),
    ("IUITables", "ERBTables"),
    ("IUIFields", "ERBFields"),
    ("Rep/Operations", "Rep/External-LLM"),
    ("Operations", "External-LLM"),
    ("analyzer packages", "packages"),
    ("ClaimsReps and AppUsers", "AppUsers and the domain actor tables"),
    ("ClaimsReps", "domain actor tables"),
    ("Claim lifecycle", "Entity lifecycle"),
    ("SIDES/charges/determinations/documents", "the platform's lifecycle entities"),
]
# Word-boundary regex subs (case-sensitive, preserve plurals first).
REGEX_SUBS = [
    (re.compile(r"\bUnemploymentClaims\b"), "the platform's records"),
    (re.compile(r"\bUnemploymentClaim\b"), "the platform's record"),
    (re.compile(r"\bclaims\b"), "records"),
    (re.compile(r"\bclaim\b"), "record"),
    (re.compile(r"\bclients\b"), "accounts"),
    (re.compile(r"\bclient\b"), "account"),
    (re.compile(r"\bchecklist\b"), "related entity"),
]
RESIDUAL = re.compile(r"IUI|claim|unemployment|SIDES|Operations", re.IGNORECASE)


def genericize(text):
    if not text:
        return text
    for a, b in EXACT_SUBS:
        text = text.replace(a, b)
    for rx, b in REGEX_SUBS:
        text = rx.sub(b, text)
    return text


# ---------------------------------------------------------------------------
def audit(row):
    row.setdefault("CreatedAt", NOW)
    row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW)
    row.setdefault("ModifiedBy", WHO)
    return row


def schema_fields(table_obj):
    sch = table_obj.get("schema")
    if isinstance(sch, dict):
        return sch.get("fields", [])
    if isinstance(sch, list):
        return sch
    return []


def ensure_iskey_schema(table_obj, for_label):
    """Add a raw boolean IsKey field after SortOrder if not present."""
    sch = table_obj.get("schema")
    fields = sch.get("fields") if isinstance(sch, dict) else sch
    if any(f.get("name") == "IsKey" for f in fields):
        return False
    desc = (
        "Star flag — marks the headline %s so users can focus on the top "
        "platform capabilities amid the long tail. Blank/false = ordinary %s."
        % (for_label, for_label)
    )
    field = {"name": "IsKey", "datatype": "boolean", "type": "raw",
             "nullable": False, "Description": desc}
    # insert right after SortOrder if found, else append
    idx = next((i for i, f in enumerate(fields) if f.get("name") == "SortOrder"), None)
    if idx is None:
        fields.append(field)
    else:
        fields.insert(idx + 1, field)
    return True


def main():
    rb = json.load(open(RB_PATH))
    src = json.load(open(SRC_PATH))

    stats = {"schema_iskey": 0, "packages": 0, "categories": 0, "features": 0,
             "erbtables": 0, "erbfields": 0}
    residual_hits = []

    # --- 1. schema: add IsKey to ERBFeatures + ERBPackages ---
    if ensure_iskey_schema(rb["ERBFeatures"], "features"):
        stats["schema_iskey"] += 1
    if ensure_iskey_schema(rb["ERBPackages"], "packages"):
        stats["schema_iskey"] += 1

    # --- 2. packages ---
    src_pkgs = {p["IUIPackageId"]: p for p in src["IUIPackages"]["data"]}
    have_pkg = {p["ERBPackageId"] for p in rb["ERBPackages"]["data"]}
    for pid, (sort, is_key) in PKG_META.items():
        if pid in have_pkg:
            continue
        sp = src_pkgs[pid]
        title = genericize(sp["Title"])
        desc = genericize(sp.get("Description", ""))
        residual_hits += [(f"pkg {pid}", m) for m in RESIDUAL.findall(desc)]
        rb["ERBPackages"]["data"].append({
            "ERBPackageId": pid, "Title": title, "Description": desc,
            "Status": "shipped", "IsActive": True, "IsLicensed": True,
            "SortOrder": sort, "IsKey": is_key,
        })
        stats["packages"] += 1

    # --- 3. feature categories ---
    src_cats = {c["IUIFeatureCategoryId"]: c for c in src["IUIFeatureCategories"]["data"]}
    have_cat = {c["ERBFeatureCategoryId"] for c in rb["ERBFeatureCategories"]["data"]}
    for cid in CORE_CATEGORY_IDS:
        if cid in have_cat:
            continue
        sc = src_cats[cid]
        desc = genericize(sc.get("Description", ""))
        residual_hits += [(f"cat {cid}", m) for m in RESIDUAL.findall(desc)]
        rb["ERBFeatureCategories"]["data"].append(audit({
            "ERBFeatureCategoryId": cid, "Title": sc["Title"],
            "Icon": sc.get("Icon"), "SortOrder": CATEGORY_SORT[cid],
            "Description": desc,
        }))
        stats["categories"] += 1

    # --- 4. features ---
    have_feat = {f["ERBFeatureId"] for f in rb["ERBFeatures"]["data"]}
    for sf in src["IUIFeatures"]["data"]:
        if sf.get("IUIPackage") not in CORE_PKG_IDS:
            continue
        fid = sf["IUIFeatureId"]
        if fid in have_feat:
            continue
        title = genericize(sf["Title"])
        desc = genericize(sf.get("Description", ""))
        category = FEATURE_CATEGORY_REMAP.get(fid, sf.get("Category"))
        residual_hits += [(f"feat {fid}", m) for m in RESIDUAL.findall(title + " " + desc)]
        rb["ERBFeatures"]["data"].append(audit({
            "ERBFeatureId": fid, "Title": title,
            "Category": category, "Status": sf.get("Status"),
            "ERBPackage": sf["IUIPackage"], "IsLicensed": True,
            "SortOrder": sf.get("SortOrder"), "IsKey": fid in STARRED_FEATURE_IDS,
            "Description": desc, "RuleRefs": "", "SourceText": "",
            "SourceFiles": "", "ImageUrl": "",
        }))
        stats["features"] += 1

    # --- 5. self-describing ERBTables / ERBFields for ALL modeled tables ---
    src_table_desc = {t["IUITableId"]: t.get("Description", "")
                      for t in src["IUITables"]["data"]}
    have_tbl = {t["ERBTableId"] for t in rb["ERBTables"]["data"]}
    have_fld = {f["ERBFieldId"] for f in rb["ERBFields"]["data"]}

    for key, obj in rb.items():
        if key in NOT_A_TABLE or not isinstance(obj, dict) or "schema" not in obj:
            continue
        table = key
        fields = schema_fields(obj)
        # ERBTables row
        if table not in have_tbl:
            sdesc = src_table_desc.get(table, "")
            # only reuse source desc if domain-agnostic (no IUI residue)
            if sdesc and not RESIDUAL.search(sdesc):
                desc = genericize(sdesc)
            else:
                desc = "Table: %s" % table
            rep = "CRUD" if table in OPERATIONAL_TABLES else "R"
            rb["ERBTables"]["data"].append(audit({
                "ERBTableId": table, "TableName": table, "Description": desc,
                "ERBPackage": TABLE_PACKAGE.get(table, "citations"),
                "Platform": "ticket-portal", "IsLicensed": True,
                "FieldCount": len(fields), "IsCatalog": table in CATALOG_TABLES,
                "AdminCRUD": "CRUD", "ManagerCRUD": "CRUD",
                "RepresentativeCRUD": rep, "ExternalLlmCRUD": "R",
            }))
            stats["erbtables"] += 1
        # ERBFields rows
        for fld in fields:
            fname = fld.get("name")
            if not fname:
                continue
            fid = "%s.%s" % (table, fname)
            if fid in have_fld:
                continue
            rb["ERBFields"]["data"].append(audit({
                "ERBFieldId": fid, "ERBTable": table, "FieldName": fname,
                "FieldType": fld.get("type"),
                "Datatype": fld.get("datatype"),
                "Description": fld.get("Description") or fld.get("description") or "",
            }))
            have_fld.add(fid)
            stats["erbfields"] += 1

    # --- FK integrity + report ---
    pkg_ids = {p["ERBPackageId"] for p in rb["ERBPackages"]["data"]}
    cat_ids = {c["ERBFeatureCategoryId"] for c in rb["ERBFeatureCategories"]["data"]}
    tbl_ids = {t["ERBTableId"] for t in rb["ERBTables"]["data"]}
    bad = []
    for f in rb["ERBFeatures"]["data"]:
        if f.get("ERBPackage") not in pkg_ids:
            bad.append(("feature.ERBPackage", f["ERBFeatureId"], f.get("ERBPackage")))
        if f.get("Category") not in cat_ids:
            bad.append(("feature.Category", f["ERBFeatureId"], f.get("Category")))
    for f in rb["ERBFields"]["data"]:
        if f.get("ERBTable") not in tbl_ids:
            bad.append(("field.ERBTable", f["ERBFieldId"], f.get("ERBTable")))

    starred_feats = sum(1 for f in rb["ERBFeatures"]["data"] if f.get("IsKey"))
    starred_pkgs = sum(1 for p in rb["ERBPackages"]["data"] if p.get("IsKey"))

    print("=== author-platform-catalog ===")
    print("schema IsKey added :", stats["schema_iskey"], "(expect 2 first run, 0 after)")
    print("packages added     : +%d  (total %d)" % (stats["packages"], len(rb["ERBPackages"]["data"])))
    print("categories added   : +%d  (total %d)" % (stats["categories"], len(rb["ERBFeatureCategories"]["data"])))
    print("features added     : +%d  (total %d)" % (stats["features"], len(rb["ERBFeatures"]["data"])))
    print("ERBTables added    : +%d  (total %d)" % (stats["erbtables"], len(rb["ERBTables"]["data"])))
    print("ERBFields added    : +%d  (total %d)" % (stats["erbfields"], len(rb["ERBFields"]["data"])))
    print("starred features   :", starred_feats, "| starred packages:", starred_pkgs)
    print("FK unresolved      :", len(bad))
    for b in bad:
        print("   BAD FK:", b)
    print("residual IUI terms :", len(residual_hits))
    for h in residual_hits:
        print("   residual:", h)

    if bad:
        print("\nABORT: FK integrity failed — not writing.")
        sys.exit(1)
    if residual_hits:
        print("\nWARNING: residual domain terms remain (review the sweep).")

    if DRY:
        print("\n[dry-run] no write")
        return

    with open(RB_PATH, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB_PATH)


if __name__ == "__main__":
    main()
