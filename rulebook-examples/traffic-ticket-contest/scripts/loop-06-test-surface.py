#!/usr/bin/env python3
"""
LOOP 6 — Create the test surface (8 new tables) and a dense, derived conformance
corpus. TestCases are GENERATED from the existing spec (one per feature, per
APIEndpoint, per state-transition rule, per business rule) so coverage is
guaranteed and stays in sync. Each TestCase gets >=1 TestExpectation (given/when/
then). TestRun/TestResult/TestResultAssertion are created as empty result sinks
with full schema. Also adds ERBTables/ERBFields catalog rows for the 8 new tables.

Merge-by-id. Re-running = 0 changes.

  python3 scripts/loop-06-test-surface.py --dry-run
  python3 scripts/loop-06-test-surface.py
"""
import json, os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T19:30:00Z"
WHO = "admin-example"


def audit(row):
    row.setdefault("CreatedAt", NOW); row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW); row.setdefault("ModifiedBy", WHO)
    return row


def F(name, dtype, ftype, desc, formula=None, related=None, nullable=True):
    f = {"name": name, "datatype": dtype, "type": ftype, "nullable": nullable, "Description": desc}
    if formula:
        f["formula"] = formula
    if related:
        f["RelatedTo"] = related
    return f


def name_field(pk):
    return F("Name", "string", "calculated", f"Echoes {pk}.", formula="={{%s}}" % pk, nullable=True)


AUDIT_FIELDS = [
    F("CreatedAt", "string", "raw", "Audit: created timestamp."),
    F("CreatedBy", "string", "raw", "Audit: created by."),
    F("ModifiedAt", "string", "raw", "Audit: modified timestamp."),
    F("ModifiedBy", "string", "raw", "Audit: modified by."),
]

# ---- the 8 table schemas ----
SCHEMAS = {
    "TestCategory": [
        F("TestCategoryId", "string", "raw", "PK = category key.", nullable=False),
        name_field("TestCategoryId"),
        F("Title", "string", "raw", "Human-readable category title."),
        F("Description", "string", "raw", "What this category groups."),
        F("OrderIndex", "number", "raw", "Display order."),
    ] + AUDIT_FIELDS,
    "TestSurface": [
        F("TestSurfaceId", "string", "raw", "PK = surface key.", nullable=False),
        name_field("TestSurfaceId"),
        F("Title", "string", "raw", "Surface title."),
        F("Description", "string", "raw", "What this surface tests."),
        F("Layer", "string", "raw", "Layer: db | view | api | ui | rules."),
        F("OrderIndex", "number", "raw", "Display order."),
    ] + AUDIT_FIELDS,
    "TestTechnology": [
        F("TestTechnologyId", "string", "raw", "PK = technology key.", nullable=False),
        name_field("TestTechnologyId"),
        F("Title", "string", "raw", "Technology title."),
        F("Description", "string", "raw", "What runner/tooling this is."),
        F("RunnerKind", "string", "raw", "Runner: sql | vitest | playwright | manual."),
        F("IsImplemented", "boolean", "raw", "Whether a runner exists yet."),
    ] + AUDIT_FIELDS,
    "TestCase": [
        F("TestCaseId", "string", "raw", "PK = test case key.", nullable=False),
        name_field("TestCaseId"),
        F("Title", "string", "raw", "Test case title."),
        F("Description", "string", "raw", "Given/when/then summary."),
        F("TestCategory", "string", "relationship", "FK -> TestCategory.", related="TestCategory"),
        F("TestSurface", "string", "relationship", "FK -> TestSurface.", related="TestSurface"),
        F("TestTechnology", "string", "relationship", "FK -> TestTechnology.", related="TestTechnology"),
        F("TargetFeature", "string", "raw", "ERBFeatures id this case verifies (if any)."),
        F("TargetEndpoint", "string", "raw", "APIEndpoints id this case exercises (if any)."),
        F("TargetTable", "string", "raw", "Table/view this case reads (if any)."),
        F("TargetTransition", "string", "raw", "StateTransitionRules id this case exercises (if any)."),
        F("BusinessRuleRefs", "string", "raw", "CSV of BusinessRule codes this case proves."),
        F("Severity", "string", "raw", "critical | high | medium | low."),
        F("IsEnabled", "boolean", "raw", "Whether the case is active."),
        F("OrderIndex", "number", "raw", "Display order."),
        F("ExpectationCount", "number", "aggregation",
          "Count of child TestExpectations.", formula="=COUNT(TestExpectation!{{TestCase}}=TestCaseId)"),
    ] + AUDIT_FIELDS,
    "TestExpectation": [
        F("TestExpectationId", "string", "raw", "PK = expectation key.", nullable=False),
        name_field("TestExpectationId"),
        F("TestCase", "string", "relationship", "FK -> TestCase.", related="TestCase"),
        F("Kind", "string", "raw", "status | view-value | state | permission | http."),
        F("Selector", "string", "raw", "What is being checked (column, path, field)."),
        F("Operator", "string", "raw", "equals | contains | gte | lte | is-null | not-null."),
        F("ExpectedValue", "string", "raw", "The expected value/outcome."),
        F("OrderIndex", "number", "raw", "Display order."),
    ] + AUDIT_FIELDS,
    "TestRun": [
        F("TestRunId", "string", "raw", "PK = run key.", nullable=False),
        name_field("TestRunId"),
        F("SuiteSelector", "string", "raw", "Which suite/filter ran."),
        F("TriggeredByRole", "string", "raw", "Role that triggered the run."),
        F("StartedAt", "string", "raw", "Run start."),
        F("FinishedAt", "string", "raw", "Run finish."),
        F("Status", "string", "raw", "running | passed | failed | errored."),
    ] + AUDIT_FIELDS,
    "TestResult": [
        F("TestResultId", "string", "raw", "PK = result key.", nullable=False),
        name_field("TestResultId"),
        F("TestRun", "string", "relationship", "FK -> TestRun.", related="TestRun"),
        F("TestCase", "string", "relationship", "FK -> TestCase.", related="TestCase"),
        F("Status", "string", "raw", "passed | failed | not-implemented | errored | skipped."),
        F("Message", "string", "raw", "Result summary."),
        F("ActualValue", "string", "raw", "Observed value."),
    ] + AUDIT_FIELDS,
    "TestResultAssertion": [
        F("TestResultAssertionId", "string", "raw", "PK = assertion key.", nullable=False),
        name_field("TestResultAssertionId"),
        F("TestResult", "string", "relationship", "FK -> TestResult.", related="TestResult"),
        F("ExpectationKind", "string", "raw", "Mirrors TestExpectation.Kind."),
        F("Selector", "string", "raw", "What was checked."),
        F("ExpectedValue", "string", "raw", "Expected."),
        F("ActualValue", "string", "raw", "Actual."),
        F("Passed", "boolean", "raw", "Whether this assertion passed."),
    ] + AUDIT_FIELDS,
}

CATEGORIES = [
    ("smoke", "Smoke", "Basic reachability and render of every screen.", 1),
    ("permissions", "Permissions", "Role-based CRUD, WHERE scoping, and redaction.", 2),
    ("lifecycle", "Lifecycle", "State-machine transitions, guards, and terminal states.", 3),
    ("rules", "Business Rules", "Each numbered business rule is enforced.", 4),
    ("api", "API Actions", "Non-CRUD endpoints behave and gate correctly.", 5),
    ("calc", "Calculations", "Computed fields (fees, deadlines, points) derive correctly.", 6),
    ("data-integrity", "Data Integrity", "FKs resolve and required fields are present.", 7),
]
SURFACES = [
    ("db", "Database", "Raw table and constraint behavior.", "db", 1),
    ("view", "Views", "vw_* computed columns.", "view", 2),
    ("api", "API", "HTTP endpoints and CRUD gateway.", "api", 3),
    ("ui", "UI", "Generated screens and nav.", "ui", 4),
    ("rules", "Rules Engine", "Business-rule enforcement.", "rules", 5),
]
TECHS = [
    ("sql", "SQL Assertions", "Direct SQL queries against the DB.", "sql", True),
    ("vitest", "Vitest", "Unit/integration tests in TypeScript.", "vitest", False),
    ("playwright", "Playwright", "End-to-end browser tests.", "playwright", False),
    ("manual", "Manual", "Human-verified acceptance checks.", "manual", True),
]


def snake(t):
    out = []
    for i, c in enumerate(t):
        if c.isupper() and i and not t[i-1].isupper():
            out.append("_")
        out.append(c.lower())
    return "".join(out)


def main():
    rb = json.load(open(RB))

    # 1. create the 8 tables (schema + empty data) if missing
    created = []
    for tname, fields in SCHEMAS.items():
        if tname not in rb:
            rb[tname] = {"schema": [dict(f) for f in fields], "data": []}
            created.append(tname)

    # vocab rows
    def upsert(tbl, key, row):
        data = rb[tbl]["data"]
        if any(r.get(key) == row[key] for r in data):
            return 0
        data.append(audit(row)); return 1

    vadded = 0
    for (cid, title, desc, o) in CATEGORIES:
        vadded += upsert("TestCategory", "TestCategoryId",
                         {"TestCategoryId": cid, "Title": title, "Description": desc, "OrderIndex": o})
    for (sid, title, desc, layer, o) in SURFACES:
        vadded += upsert("TestSurface", "TestSurfaceId",
                         {"TestSurfaceId": sid, "Title": title, "Description": desc, "Layer": layer, "OrderIndex": o})
    for (tid, title, desc, runner, impl) in TECHS:
        vadded += upsert("TestTechnology", "TestTechnologyId",
                         {"TestTechnologyId": tid, "Title": title, "Description": desc,
                          "RunnerKind": runner, "IsImplemented": impl})

    # 2. GENERATE TestCases from the spec (guaranteed coverage)
    cases = rb["TestCase"]["data"]
    exps = rb["TestExpectation"]["data"]
    have_case = {c["TestCaseId"] for c in cases}
    have_exp = {e["TestExpectationId"] for e in exps}
    cadded = eadded = 0
    oi = 0

    def add_case(cid, title, desc, cat, surf, tech, sev, refs, feat=None, ep=None, tbl=None, tr=None):
        nonlocal cadded, oi
        if cid in have_case:
            return False
        oi += 1
        cases.append(audit({
            "TestCaseId": cid, "Title": title, "Description": desc,
            "TestCategory": cat, "TestSurface": surf, "TestTechnology": tech,
            "TargetFeature": feat or "", "TargetEndpoint": ep or "",
            "TargetTable": tbl or "", "TargetTransition": tr or "",
            "BusinessRuleRefs": refs or "", "Severity": sev,
            "IsEnabled": True, "OrderIndex": oi,
        }))
        have_case.add(cid); cadded += 1
        return True

    def add_exp(cid, idx, kind, selector, op, expected):
        nonlocal eadded
        eid = f"{cid}--exp{idx}"
        if eid in have_exp:
            return
        exps.append(audit({
            "TestExpectationId": eid, "TestCase": cid, "Kind": kind,
            "Selector": selector, "Operator": op, "ExpectedValue": expected, "OrderIndex": idx,
        }))
        have_exp.add(eid); eadded += 1

    # one smoke case per feature that renders a screen
    for f in rb["ERBFeatures"]["data"]:
        fid = f["ERBFeatureId"]
        route = (f.get("Route") or "").strip()
        cid = f"tc-feature-{fid}"
        if route and route not in ("feature-detail",):
            if add_case(cid, f"Feature renders: {f.get('Title')}",
                        f"The {f.get('Title')} feature is reachable at its route and renders without error.",
                        "smoke", "ui", "playwright", "high", f.get("RuleRefs"), feat=fid):
                add_exp(cid, 1, "status", "http.status", "equals", "200")
                add_exp(cid, 2, "view-value", "page.title", "contains", f.get("Title") or "")
        else:
            # documented-capability check (manual) for non-screen features
            if add_case(cid, f"Capability documented: {f.get('Title')}",
                        f"The {f.get('Title')} capability has a spec (SourceText + RuleRefs) and is catalogued.",
                        "smoke", "rules", "manual", "low", f.get("RuleRefs"), feat=fid):
                add_exp(cid, 1, "view-value", "erb_features.source_text", "not-null", "")

    # one case per API endpoint (behavior + gating)
    for e in rb["APIEndpoints"]["data"]:
        eid = e["APIEndpointId"]
        cid = f"tc-endpoint-{eid}"
        if add_case(cid, f"Endpoint: {e.get('Title')}",
                    f"{e.get('HttpMethod')} {e.get('Path')} performs its action and enforces role visibility ({e.get('RoleVisibility')}).",
                    "api", "api", "vitest", "critical", None, ep=eid, tbl=e.get("SubjectTableName")):
            add_exp(cid, 1, "http", "method+path", "equals", f"{e.get('HttpMethod')} {e.get('Path')}")
            add_exp(cid, 2, "permission", "role-visibility", "equals", e.get("RoleVisibility") or "")
            if e.get("TriggersStateMachine"):
                add_exp(cid, 3, "state", "triggers-machine", "equals", e["TriggersStateMachine"])

    # one case per state transition rule (guard + endpoint)
    for r in rb["StateTransitionRules"]["data"]:
        rid = r["StateTransitionRuleId"]
        cid = f"tc-transition-{rid}"
        if add_case(cid, f"Transition: {rid}",
                    f"Guard '{r.get('GuardDescription')}' allows {r.get('FromState')} -> {r.get('ToState')} for role {r.get('TriggeredByRole')}.",
                    "lifecycle", "rules", "sql", "critical", r.get("RuleRefs"),
                    tr=rid, ep=r.get("TriggerEndpoint")):
            add_exp(cid, 1, "state", "from->to", "equals", f"{r.get('FromState')}->{r.get('ToState')}")
            add_exp(cid, 2, "permission", "firing-role", "equals", r.get("TriggeredByRole") or "")

    # one case per business rule (enforcement)
    for br in rb["BusinessRules"]["data"]:
        code = br.get("RuleCode")
        if not code:
            continue
        cid = f"tc-rule-{code}"
        if add_case(cid, f"Rule enforced: {code} — {br.get('Title')}",
                    f"{br.get('Description')}",
                    "rules", "rules", "sql", "high", code, tbl=(br.get("SchemaLocation") or "").split(".")[0] or None):
            add_exp(cid, 1, "view-value", br.get("SchemaLocation") or "rule", "not-null", "")

    # 3. ERBTables/ERBFields catalog rows for the 8 new tables
    et = rb["ERBTables"]["data"]; ef = rb["ERBFields"]["data"]
    have_et = {x["ERBTableId"] for x in et}; have_ef = {x["ERBFieldId"] for x in ef}
    et_added = ef_added = 0
    for tname, fields in SCHEMAS.items():
        if tname not in have_et:
            et.append(audit({
                "ERBTableId": tname, "TableName": tname,
                "Description": f"Test-surface table: {tname}.",
                "ERBPackage": "platform-meta", "Platform": "ticket-portal",
                "IsLicensed": True, "FieldCount": len(fields), "IsCatalog": True,
                "AdminCRUD": "CRUD", "ManagerCRUD": "CRUD",
                "RepresentativeCRUD": "R", "ExternalLlmCRUD": "R",
            }))
            have_et.add(tname); et_added += 1
        for fld in fields:
            fid = f"{tname}.{fld['name']}"
            if fid in have_ef:
                continue
            ef.append(audit({
                "ERBFieldId": fid, "ERBTable": tname, "FieldName": fld["name"],
                "FieldType": fld["type"], "Datatype": fld["datatype"],
                "Description": fld.get("Description", ""),
            }))
            have_ef.add(fid); ef_added += 1

    # consistency: every TestCase FK resolves
    catids = {c["TestCategoryId"] for c in rb["TestCategory"]["data"]}
    surfids = {s["TestSurfaceId"] for s in rb["TestSurface"]["data"]}
    techids = {t["TestTechnologyId"] for t in rb["TestTechnology"]["data"]}
    featids = {f["ERBFeatureId"] for f in rb["ERBFeatures"]["data"]}
    epids = {e["APIEndpointId"] for e in rb["APIEndpoints"]["data"]}
    trids = {r["StateTransitionRuleId"] for r in rb["StateTransitionRules"]["data"]}
    rulecodes = {r["RuleCode"] for r in rb["BusinessRules"]["data"] if r.get("RuleCode")}
    bad = []
    for c in cases:
        if c.get("TestCategory") not in catids: bad.append(("cat", c["TestCaseId"]))
        if c.get("TestSurface") not in surfids: bad.append(("surf", c["TestCaseId"]))
        if c.get("TestTechnology") not in techids: bad.append(("tech", c["TestCaseId"]))
        if c.get("TargetFeature") and c["TargetFeature"] not in featids: bad.append(("feat", c["TestCaseId"]))
        if c.get("TargetEndpoint") and c["TargetEndpoint"] not in epids: bad.append(("ep", c["TestCaseId"], c["TargetEndpoint"]))
        if c.get("TargetTransition") and c["TargetTransition"] not in trids: bad.append(("tr", c["TestCaseId"]))
        for rc in (c.get("BusinessRuleRefs") or "").split(","):
            if rc.strip() and rc.strip() not in rulecodes: bad.append(("ruleref", c["TestCaseId"], rc.strip()))
    for e in exps:
        if e["TestCase"] not in have_case: bad.append(("exp.case", e["TestExpectationId"]))

    # coverage report
    feat_cov = len({c["TargetFeature"] for c in cases if c.get("TargetFeature")})
    ep_cov = len({c["TargetEndpoint"] for c in cases if c.get("TargetEndpoint")})
    tr_cov = len({c["TargetTransition"] for c in cases if c.get("TargetTransition")})

    print("=== LOOP 6: test surface ===")
    print("tables created:", created)
    print("vocab rows added:", vadded)
    print("TestCases +%d (%d) | TestExpectations +%d (%d)" % (cadded, len(cases), eadded, len(exps)))
    print("ERBTables +%d | ERBFields +%d" % (et_added, ef_added))
    print(f"coverage: features={feat_cov}/{len(featids)} endpoints={ep_cov}/{len(epids)} transitions={tr_cov}/{len(trids)}")
    print("consistency unresolved:", len(bad), bad[:10])
    if bad:
        print("\nABORT."); sys.exit(1)
    if DRY:
        print("\n[dry-run] no write"); return
    with open(RB, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB)


if __name__ == "__main__":
    main()
