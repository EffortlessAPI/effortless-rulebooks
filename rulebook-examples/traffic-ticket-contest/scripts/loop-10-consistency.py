#!/usr/bin/env python3
"""
LOOP 10 — Grand consistency pass + completeness critic. A ruthless, read-mostly
audit of the whole rulebook with targeted backfills:
  - Every relationship-field FK value points at an existing target-table PK.
  - Every cross-reference resolves: feature.RuleRefs/Category/ERBPackage,
    nav.PrimaryTable/ParentRouteKey/BusinessRuleRefs, endpoint.SubjectTableName/
    TriggersStateMachine, transition.From/To/RuleRefs/TriggerEndpoint,
    testcase targets, hint.ERBTable, glossary.Category.
  - Every modeled table has an ERBTables + ERBFields catalog entry (self-describing
    catalog stays complete).
  - Every feature has SourceText + RuleRefs + SourceFiles.
  - No empty-string values on relationship fields (must be null).
  - Each state machine: exactly 1 initial, >=1 terminal.

Backfills it can do safely: null empty relationship values; add missing ERBTables/
ERBFields catalog rows for any uncatalogued modeled table. Anything it cannot
auto-fix is reported as a hard failure.

  python3 scripts/loop-10-consistency.py --dry-run
  python3 scripts/loop-10-consistency.py
"""
import json, os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T20:30:00Z"
WHO = "admin-example"
NOT_TABLE = {"$schema", "Name", "Description", "_meta", "__meta__"}


def audit(row):
    row.setdefault("CreatedAt", NOW); row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW); row.setdefault("ModifiedBy", WHO)
    return row


def fields_of(obj):
    sch = obj["schema"]
    return sch["fields"] if isinstance(sch, dict) else sch


def pk_of(obj):
    for f in fields_of(obj):
        if f.get("type") == "raw" and f["name"].endswith("Id"):
            return f["name"]
    # fallback: first raw field
    for f in fields_of(obj):
        if f.get("type") == "raw":
            return f["name"]
    return None


def main():
    rb = json.load(open(RB))
    tables = {k: v for k, v in rb.items() if k not in NOT_TABLE and isinstance(v, dict) and "schema" in v}

    # PK sets per table
    pk_field = {t: pk_of(obj) for t, obj in tables.items()}
    pk_sets = {t: {r.get(pk_field[t]) for r in obj["data"]} for t, obj in tables.items()}

    fixes = {"nulled_empty_fk": 0, "catalog_tables": 0, "catalog_fields": 0}
    failures = []

    # 1. null empty-string relationship values across ALL tables
    for t, obj in tables.items():
        relfields = [f["name"] for f in fields_of(obj) if f.get("type") == "relationship"]
        for r in obj["data"]:
            for rf in relfields:
                if r.get(rf) == "":
                    r[rf] = None
                    fixes["nulled_empty_fk"] += 1

    # 2. validate every relationship FK resolves to its target PK set
    for t, obj in tables.items():
        for f in fields_of(obj):
            if f.get("type") != "relationship":
                continue
            target = f.get("RelatedTo") or f.get("formula")
            if not target or target not in pk_sets:
                continue
            fn = f["name"]
            for r in obj["data"]:
                v = r.get(fn)
                if v not in (None, "") and v not in pk_sets[target]:
                    failures.append(("FK", f"{t}.{fn}", v, "->" + target))

    # 3. ensure ERBTables/ERBFields catalog covers every modeled table
    et = rb["ERBTables"]["data"]; ef = rb["ERBFields"]["data"]
    have_et = {x["ERBTableId"] for x in et}
    have_ef = {x["ERBFieldId"] for x in ef}
    for t, obj in tables.items():
        if t in ("ERBTables", "ERBFields"):
            pass  # they catalog themselves; still want rows
        if t not in have_et:
            et.append(audit({
                "ERBTableId": t, "TableName": t, "Description": f"Table: {t}.",
                "ERBPackage": "platform-meta", "Platform": "ticket-portal",
                "IsLicensed": True, "FieldCount": len(fields_of(obj)), "IsCatalog": True,
                "AdminCRUD": "CRUD", "ManagerCRUD": "R", "RepresentativeCRUD": "R", "ExternalLlmCRUD": "R",
            }))
            have_et.add(t); fixes["catalog_tables"] += 1
        for fld in fields_of(obj):
            fid = f"{t}.{fld['name']}"
            if fid in have_ef:
                continue
            ef.append(audit({
                "ERBFieldId": fid, "ERBTable": t, "FieldName": fld["name"],
                "FieldType": fld.get("type"), "Datatype": fld.get("datatype"),
                "Description": fld.get("Description") or fld.get("description") or "",
            }))
            have_ef.add(fid); fixes["catalog_fields"] += 1

    # 4. cross-reference resolution checks (report-only)
    rule_codes = {r["RuleCode"] for r in rb["BusinessRules"]["data"] if r.get("RuleCode")}
    feat_ids = {f["ERBFeatureId"] for f in rb["ERBFeatures"]["data"]}
    cat_ids = {c["ERBFeatureCategoryId"] for c in rb["ERBFeatureCategories"]["data"]}
    pkg_ids = {p["ERBPackageId"] for p in rb["ERBPackages"]["data"]}
    nav_keys = {n["RouteKey"] for n in rb["PlatformNaviation"]["data"]}
    machine_ids = {s["StateMachineId"] for s in rb["StateMachines"]["data"]}
    state_ids = {s["MachineStateId"] for s in rb["MachineStates"]["data"]}
    ep_ids = {e["APIEndpointId"] for e in rb["APIEndpoints"]["data"]}
    et_ids = {x["ERBTableId"] for x in et}

    def check_csv(rows, label, field, valid):
        for r in rows:
            for c in (r.get(field) or "").split(","):
                c = c.strip()
                if c and c not in valid:
                    failures.append((label, field, c))

    # features
    for f in rb["ERBFeatures"]["data"]:
        if f.get("Category") not in cat_ids: failures.append(("feat.Category", f["ERBFeatureId"], f.get("Category")))
        if f.get("ERBPackage") not in pkg_ids: failures.append(("feat.Package", f["ERBFeatureId"], f.get("ERBPackage")))
        if not (f.get("SourceText") or "").strip(): failures.append(("feat.SourceText-empty", f["ERBFeatureId"], ""))
        if not (f.get("RuleRefs") or "").strip(): failures.append(("feat.RuleRefs-empty", f["ERBFeatureId"], ""))
        if not (f.get("SourceFiles") or "").strip(): failures.append(("feat.SourceFiles-empty", f["ERBFeatureId"], ""))
    check_csv(rb["ERBFeatures"]["data"], "feat.RuleRefs", "RuleRefs", rule_codes)

    # nav
    for n in rb["PlatformNaviation"]["data"]:
        if n.get("PrimaryTable") and n["PrimaryTable"] not in tables: failures.append(("nav.PrimaryTable", n["RouteKey"], n.get("PrimaryTable")))
        if n.get("ParentRouteKey") and n["ParentRouteKey"] not in nav_keys: failures.append(("nav.Parent", n["RouteKey"], n.get("ParentRouteKey")))
    check_csv(rb["PlatformNaviation"]["data"], "nav.RuleRefs", "BusinessRuleRefs", rule_codes)

    # endpoints
    for e in rb["APIEndpoints"]["data"]:
        if e.get("SubjectTableName") and e["SubjectTableName"] not in tables: failures.append(("ep.Subject", e["APIEndpointId"], e.get("SubjectTableName")))
        if e.get("TriggersStateMachine") and e["TriggersStateMachine"] not in machine_ids: failures.append(("ep.Machine", e["APIEndpointId"], e.get("TriggersStateMachine")))

    # transitions
    for r in rb["StateTransitionRules"]["data"]:
        if r.get("FromState") and r["FromState"] not in state_ids: failures.append(("tr.From", r["StateTransitionRuleId"], r.get("FromState")))
        if r.get("ToState") and r["ToState"] not in state_ids: failures.append(("tr.To", r["StateTransitionRuleId"], r.get("ToState")))
        if r.get("TriggerEndpoint") and r["TriggerEndpoint"] not in ep_ids: failures.append(("tr.Endpoint", r["StateTransitionRuleId"], r.get("TriggerEndpoint")))
    check_csv(rb["StateTransitionRules"]["data"], "tr.RuleRefs", "RuleRefs", rule_codes)

    # state machine init/terminal
    for m in machine_ids:
        ms = [s for s in rb["MachineStates"]["data"] if s.get("StateMachine") == m]
        ini = sum(1 for s in ms if s.get("IsInitial")); term = sum(1 for s in ms if s.get("IsTerminal"))
        if ini != 1: failures.append(("machine.initial", m, f"count={ini}"))
        if term < 1: failures.append(("machine.terminal", m, f"count={term}"))

    # test cases
    if "TestCase" in rb:
        for c in rb["TestCase"]["data"]:
            if c.get("TargetFeature") and c["TargetFeature"] not in feat_ids: failures.append(("tc.Feature", c["TestCaseId"], c.get("TargetFeature")))
            if c.get("TargetEndpoint") and c["TargetEndpoint"] not in ep_ids: failures.append(("tc.Endpoint", c["TestCaseId"], c.get("TargetEndpoint")))
            check_csv([c], "tc.RuleRefs", "BusinessRuleRefs", rule_codes)

    # display hints
    if "FieldDisplayHints" in rb:
        for h in rb["FieldDisplayHints"]["data"]:
            if h.get("ERBTable") not in et_ids: failures.append(("hint.ERBTable", h["FieldDisplayHintId"], h.get("ERBTable")))

    # glossary
    gcat = {c["GlossaryCategoryId"] for c in rb["GlossaryCategories"]["data"]}
    for g in rb["GlossaryTerms"]["data"]:
        if g.get("Category") not in gcat: failures.append(("glossary.Category", g["GlossaryTermId"], g.get("Category")))

    # ---- report ----
    print("=== LOOP 10: grand consistency pass ===")
    print("auto-fixes:", fixes)
    print("total modeled tables:", len(tables))
    print("ERBTables catalog rows:", len(et), "| ERBFields catalog rows:", len(ef))
    # spec completeness snapshot
    print("features:", len(rb["ERBFeatures"]["data"]),
          "| rules:", len(rb["BusinessRules"]["data"]),
          "| nav:", len(rb["PlatformNaviation"]["data"]),
          "| endpoints:", len(rb["APIEndpoints"]["data"]))
    print("states:", len(rb["MachineStates"]["data"]),
          "| transitions:", len(rb["StateTransitionRules"]["data"]),
          "| testcases:", len(rb.get("TestCase", {}).get("data", [])),
          "| hints:", len(rb.get("FieldDisplayHints", {}).get("data", [])))
    print("CONSISTENCY FAILURES:", len(failures))
    for f in failures[:40]:
        print("   ", f)
    if failures:
        print("\nFAIL: unresolved references remain."); sys.exit(1)
    if DRY:
        print("\n[dry-run] no write — ALL CONSISTENT"); return
    with open(RB, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB, "— FULLY CONSISTENT")


if __name__ == "__main__":
    main()
