#!/usr/bin/env python3
"""
LOOP 8 — Deepen the domain so the world is believable and every calc/lookup/
aggregation field is exercised: more Drivers, Citations, Hearings, Payments,
CaseEvents, plus 4 new related tables driven by the rules engine:
  - FeeSchedules     : base fine per (ViolationType, Jurisdiction).
  - DeadlineRules    : contest/payment windows per Jurisdiction.
  - ContestGrounds   : eligible legal grounds per ViolationType.
  - DriverLicensePoints : point-history rows per Driver per adjudicated Citation.

FK convention: relationship value = TARGET's Name natural key (lower-kebab).
  Drivers      -> lower(LicenseNumber)   d1234567
  Citations    -> lower(CitationNumber)  tc-2026-0002
  ViolationTypes -> lower(Code)          cvc-22350
  Jurisdictions  -> lower(Code)          ca-la

Adds ERBTables/ERBFields catalog rows for the 4 new tables. Merge-by-id.

  python3 scripts/loop-08-domain-depth.py --dry-run
  python3 scripts/loop-08-domain-depth.py
"""
import json, os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T20:00:00Z"
WHO = "admin-example"


def audit(row):
    row.setdefault("CreatedAt", NOW); row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW); row.setdefault("ModifiedBy", WHO)
    return row


def F(name, dtype, ftype, desc, formula=None, related=None, nullable=True):
    f = {"name": name, "datatype": dtype, "type": ftype, "nullable": nullable, "Description": desc}
    if formula: f["formula"] = formula
    if related: f["RelatedTo"] = related
    return f


AUD = [F("CreatedAt","string","raw","Audit."),F("CreatedBy","string","raw","Audit."),
       F("ModifiedAt","string","raw","Audit."),F("ModifiedBy","string","raw","Audit.")]

SCHEMAS = {
    "FeeSchedules": [
        F("FeeScheduleId","string","raw","PK = jurisdiction + violation code.",nullable=False),
        F("Name","string","calculated","Echoes FeeScheduleId.",formula="={{FeeScheduleId}}"),
        F("Jurisdiction","string","relationship","FK -> Jurisdictions.",related="Jurisdictions"),
        F("ViolationType","string","relationship","FK -> ViolationTypes.",related="ViolationTypes"),
        F("BaseFineUsd","decimal","raw","Base fine before surcharges."),
        F("SurchargePct","decimal","raw","Jurisdiction surcharge percentage."),
        F("EffectiveOn","date","raw","Date this schedule took effect."),
    ] + AUD,
    "DeadlineRules": [
        F("DeadlineRuleId","string","raw","PK = jurisdiction + kind.",nullable=False),
        F("Name","string","calculated","Echoes DeadlineRuleId.",formula="={{DeadlineRuleId}}"),
        F("Jurisdiction","string","relationship","FK -> Jurisdictions.",related="Jurisdictions"),
        F("Kind","string","raw","contest | payment | appeal."),
        F("WindowDays","number","raw","Days from issuance to the deadline."),
        F("LatePenaltyUsd","decimal","raw","Flat penalty when missed."),
        F("Description","string","raw","Plain-language rule."),
    ] + AUD,
    "ContestGrounds": [
        F("ContestGroundId","string","raw","PK = violation code + ground handle.",nullable=False),
        F("Name","string","calculated","Echoes ContestGroundId.",formula="={{ContestGroundId}}"),
        F("ViolationType","string","relationship","FK -> ViolationTypes.",related="ViolationTypes"),
        F("Title","string","raw","Short name of the legal ground."),
        F("Description","string","raw","When this ground applies."),
        F("EvidenceHint","string","raw","Evidence that supports this ground."),
        F("OrderIndex","number","raw","Display order."),
    ] + AUD,
    "DriverLicensePoints": [
        F("DriverLicensePointId","string","raw","PK = citation + driver.",nullable=False),
        F("Name","string","calculated","Echoes DriverLicensePointId.",formula="={{DriverLicensePointId}}"),
        F("Driver","string","relationship","FK -> Drivers.",related="Drivers"),
        F("Citation","string","relationship","FK -> Citations.",related="Citations"),
        F("Points","number","raw","Points assessed for this citation."),
        F("AssessedOn","date","raw","Date assessed."),
        F("ReversedOn","date","raw","Date reversed (if dismissed)."),
        F("IsActive","boolean","raw","Whether the points currently count."),
    ] + AUD,
}

# --- new domain rows. FK conventions (verified from existing rows):
#   Driver PK/FK     = d-<lowercase-license>     d-d2468013
#   Citation PK/FK   = c-NNNN                     c-0007
#   ViolationType FK = vt-<lowercase-code>        vt-cvc-22350
#   Jurisdiction FK  = j-<lowercase-code>         j-ca-la
#   Hearing PK = h-NNNN | Payment PK = p-NNNN | CaseEvent PK = ev-NNNN
NEW_DRIVERS = [
    ("D2468013", "Maria", "Gonzalez", "j-ca-la"),
    ("D1357924", "James", "Whitfield", "j-tx-aus"),
    ("N5544332", "Aisha", "Rahman", "j-ny-nyc"),
]
NEW_CITATIONS = [
    # (pk, num, driver-fk, violation-fk, jur-fk, issued, responded, contest, paidon, amount)
    ("c-0007", "TC-2026-0007", "d-d2468013", "vt-cvc-22350", "j-ca-la", "2026-04-02", "2026-04-10", True, None, None),
    ("c-0008", "TC-2026-0008", "d-d1357924", "vt-ttc-545351", "j-tx-aus", "2026-04-15", None, False, None, None),
    ("c-0009", "TC-2026-0009", "d-n5544332", "vt-vtl-1180b", "j-ny-nyc", "2026-03-20", "2026-03-25", False, "2026-03-26", 138.0),
    ("c-0010", "TC-2026-0010", "d-d2468013", "vt-cvc-23123", "j-ca-la", "2026-05-01", "2026-05-05", True, None, None),
    ("c-0011", "TC-2026-0011", "d-d1357924", "vt-ttc-544007", "j-tx-aus", "2026-05-10", None, False, None, None),
    ("c-0012", "TC-2026-0012", "d-n5544332", "vt-vtl-1111d", "j-ny-nyc", "2026-05-18", "2026-05-20", True, None, None),
]
NEW_HEARINGS = [
    ("h-0003", "HRG-2026-0003", "c-0007", "2026-05-12", "scheduled"),
    ("h-0004", "HRG-2026-0004", "c-0010", "2026-06-02", "scheduled"),
    ("h-0005", "HRG-2026-0005", "c-0012", "2026-06-20", "requested"),
]
NEW_PAYMENTS = [
    ("p-0002", "PMT-2026-0009", "c-0009", 138.0, "2026-03-26", "paid"),
    ("p-0003", "PMT-2026-0001", "c-0001", 95.0, "2026-04-01", "paid"),
]
NEW_CASE_EVENTS = [
    # (pk, number, citation-fk, occurredon, track, fromstate, tostate, note)
    ("ev-0008", "EV-2026-0008", "c-0007", "2026-04-02", "Citation", None, "Issued", "Citation issued."),
    ("ev-0009", "EV-2026-0009", "c-0007", "2026-04-10", "Contest", "Issued", "InContest", "Driver opened a contest."),
    ("ev-0010", "EV-2026-0010", "c-0009", "2026-03-26", "Payment", "Due", "Paid", "Driver paid in full."),
    ("ev-0011", "EV-2026-0011", "c-0010", "2026-05-05", "Contest", "Issued", "InContest", "Contest filed on equipment grounds."),
    ("ev-0012", "EV-2026-0012", "c-0012", "2026-05-20", "Contest", "Issued", "InContest", "Contest filed; hearing requested."),
]


def cols(rb, t):
    sch = rb[t]["schema"]; fs = sch["fields"] if isinstance(sch, dict) else sch
    return {f["name"] for f in fs}


def main():
    rb = json.load(open(RB))

    created = []
    for tname, fields in SCHEMAS.items():
        if tname not in rb:
            rb[tname] = {"schema": [dict(f) for f in fields], "data": []}
            created.append(tname)

    def upsert(tbl, key, row):
        data = rb[tbl]["data"]
        if any(r.get(key) == row[key] for r in data):
            return 0
        # only keep keys that exist in the table schema
        valid = cols(rb, tbl)
        clean = {k: v for k, v in row.items() if k in valid}
        data.append(audit(clean)); return 1

    counts = {}

    # drivers (PK = d-<lowercase-license>)
    n = 0
    for (lic, fn, ln, jur) in NEW_DRIVERS:
        n += upsert("Drivers", "DriverId", {
            "DriverId": "d-" + lic.lower(), "LicenseNumber": lic, "FirstName": fn, "LastName": ln,
            "HomeJurisdiction": jur, "IsMockData": True})
    counts["Drivers"] = n

    # citations (explicit c-NNNN PK)
    n = 0
    for (pk, num, drv, vio, jur, iss, resp, contest, paid, amt) in NEW_CITATIONS:
        n += upsert("Citations", "CitationId", {
            "CitationId": pk, "CitationNumber": num, "Driver": drv,
            "ViolationType": vio, "Jurisdiction": jur, "IssuedOn": iss, "AsOfDate": "2026-06-07",
            "RespondedOn": resp, "ContestRequested": contest, "PaidOn": paid,
            "AmountPaidUsd": amt, "IsMockData": True})
    counts["Citations"] = n

    # hearings (h-NNNN PK)
    hcols = cols(rb, "Hearings")
    n = 0
    for (hid, num, cit, when, status) in NEW_HEARINGS:
        row = {"HearingId": hid, "Citation": cit, "IsMockData": True}
        for cand, val in (("HearingNumber", num), ("ScheduledOn", when), ("ScheduledFor", when),
                          ("HearingDate", when), ("Status", status), ("Outcome", "")):
            if cand in hcols:
                row.setdefault(cand, val)
        n += upsert("Hearings", "HearingId", row)
    counts["Hearings"] = n

    # payments (p-NNNN PK)
    pcols = cols(rb, "Payments")
    n = 0
    for (pid, num, cit, amt, when, status) in NEW_PAYMENTS:
        row = {"PaymentId": pid, "Citation": cit, "IsMockData": True}
        for cand, val in (("PaymentNumber", num), ("AmountUsd", amt), ("AmountPaidUsd", amt),
                          ("PaidOn", when), ("Status", status), ("Method", "online")):
            if cand in pcols:
                row.setdefault(cand, val)
        n += upsert("Payments", "PaymentId", row)
    counts["Payments"] = n

    # case events (ev-NNNN PK; matches existing EventNumber/OccurredOn/Track/FromState/ToState/Note)
    cecols = cols(rb, "CaseEvents")
    n = 0
    for (ceid, num, cit, when, track, frm, to, note) in NEW_CASE_EVENTS:
        row = {"CaseEventId": ceid, "Citation": cit, "IsMockData": True}
        for cand, val in (("EventNumber", num), ("OccurredOn", when), ("Track", track),
                          ("FromState", frm), ("ToState", to), ("Note", note)):
            if cand in cecols:
                row.setdefault(cand, val)
        n += upsert("CaseEvents", "CaseEventId", row)
    counts["CaseEvents"] = n

    # --- new related tables (FK targets use type-prefixed PKs) ---
    JURS = ["j-ca-la", "j-ny-nyc", "j-tx-aus"]
    VIOS = ["vt-cvc-22350", "vt-cvc-21453a", "vt-cvc-23123", "vt-vtl-1180b", "vt-vtl-1111d", "vt-ttc-545351", "vt-ttc-544007"]
    FINES = {"vt-cvc-22350": 235, "vt-cvc-21453a": 490, "vt-cvc-23123": 162, "vt-vtl-1180b": 138,
             "vt-vtl-1111d": 278, "vt-ttc-545351": 205, "vt-ttc-544007": 175}
    n = 0
    for j in JURS:
        for v in VIOS:
            fid = f"{j}--{v}"
            n += upsert("FeeSchedules", "FeeScheduleId", {
                "FeeScheduleId": fid, "Jurisdiction": j, "ViolationType": v,
                "BaseFineUsd": FINES[v], "SurchargePct": 0.20 if j == "ca-la" else 0.15,
                "EffectiveOn": "2026-01-01"})
    counts["FeeSchedules"] = n

    n = 0
    for j in JURS:
        for (kind, days, pen, desc) in (
            ("contest", 21, 0, "Days from issuance to file a contest."),
            ("payment", 30, 25, "Days from issuance to pay before late penalty."),
            ("appeal", 30, 0, "Days from determination to appeal.")):
            n += upsert("DeadlineRules", "DeadlineRuleId", {
                "DeadlineRuleId": f"{j}--{kind}", "Jurisdiction": j, "Kind": kind,
                "WindowDays": days, "LatePenaltyUsd": pen, "Description": desc})
    counts["DeadlineRules"] = n

    GROUNDS = {
        "vt-cvc-22350": [("signage", "Improper or missing signage", "Speed limit sign obscured or absent.", "Photos of the location.")],
        "vt-cvc-21453a": [("malfunction", "Signal malfunction", "Traffic signal was malfunctioning.", "Maintenance log / video.")],
        "vt-cvc-23123": [("emergency", "Emergency use", "Phone used for a genuine emergency call.", "Call records.")],
        "vt-vtl-1180b": [("calibration", "Radar miscalibration", "Speed device not properly calibrated.", "Calibration certificate.")],
        "vt-vtl-1111d": [("obstruction", "Obstructed view", "Signal view obstructed by foliage.", "Site photos.")],
        "vt-ttc-545351": [("speedometer", "Speedometer error", "Vehicle speedometer reads low.", "Mechanic statement.")],
        "vt-ttc-544007": [("mistaken-id", "Mistaken identity", "Another vehicle matched the description.", "Witness statement.")],
    }
    n = 0
    for v, gs in GROUNDS.items():
        for i, (h, title, desc, ev) in enumerate(gs):
            n += upsert("ContestGrounds", "ContestGroundId", {
                "ContestGroundId": f"{v}--{h}", "ViolationType": v, "Title": title,
                "Description": desc, "EvidenceHint": ev, "OrderIndex": i + 1})
    # add generic grounds available to all
    for v in VIOS:
        n += upsert("ContestGrounds", "ContestGroundId", {
            "ContestGroundId": f"{v}--procedural", "ViolationType": v,
            "Title": "Procedural defect", "Description": "Officer failed to follow required procedure.",
            "EvidenceHint": "Citation copy showing the defect.", "OrderIndex": 9})
    counts["ContestGrounds"] = n

    # assess points for adjudicated/older citations (FKs: Citation=c-NNNN, Driver=d-<lic>)
    PT_ROWS = [
        ("c-0001", "d-d1234567", 1, "2026-02-15", None, True),
        ("c-0009", "d-n5544332", 3, "2026-03-30", None, True),
        ("c-0003", "d-d7654321", 2, "2026-03-01", "2026-03-20", False),
    ]
    n = 0
    for (cit, drv, pts, assessed, reversed_, active) in PT_ROWS:
        n += upsert("DriverLicensePoints", "DriverLicensePointId", {
            "DriverLicensePointId": f"{cit}--{drv}", "Driver": drv, "Citation": cit,
            "Points": pts, "AssessedOn": assessed, "ReversedOn": reversed_, "IsActive": active})
    counts["DriverLicensePoints"] = n

    # --- catalog the 4 new tables ---
    et = rb["ERBTables"]["data"]; ef = rb["ERBFields"]["data"]
    have_et = {x["ERBTableId"] for x in et}; have_ef = {x["ERBFieldId"] for x in ef}
    et_added = ef_added = 0
    for tname, fields in SCHEMAS.items():
        if tname not in have_et:
            et.append(audit({
                "ERBTableId": tname, "TableName": tname,
                "Description": f"Domain rules/data table: {tname}.",
                "ERBPackage": "citations", "Platform": "ticket-portal",
                "IsLicensed": True, "FieldCount": len(fields),
                "IsCatalog": tname in ("FeeSchedules", "DeadlineRules", "ContestGrounds"),
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

    # consistency: FK targets exist (use actual PKs)
    drv_keys = {d["DriverId"] for d in rb["Drivers"]["data"]}
    cit_keys = {c["CitationId"] for c in rb["Citations"]["data"]}
    vio_keys = {v.get("ViolationTypeId") for v in rb["ViolationTypes"]["data"]}
    jur_keys = {j.get("JurisdictionId") for j in rb["Jurisdictions"]["data"]}
    bad = []
    for r in rb["FeeSchedules"]["data"]:
        if r.get("Jurisdiction") not in jur_keys: bad.append(("fee.jur", r["FeeScheduleId"], r.get("Jurisdiction")))
        if r.get("ViolationType") not in vio_keys: bad.append(("fee.vio", r["FeeScheduleId"], r.get("ViolationType")))
    for r in rb["ContestGrounds"]["data"]:
        if r.get("ViolationType") not in vio_keys: bad.append(("ground.vio", r["ContestGroundId"]))
    for r in rb["DriverLicensePoints"]["data"]:
        if r.get("Driver") not in drv_keys: bad.append(("pts.drv", r["DriverLicensePointId"], r.get("Driver")))
        if r.get("Citation") not in cit_keys: bad.append(("pts.cit", r["DriverLicensePointId"], r.get("Citation")))
    for r in rb["Citations"]["data"]:
        if r.get("Driver") and r["Driver"] not in drv_keys: bad.append(("cit.drv", r["CitationId"], r.get("Driver")))

    print("=== LOOP 8: domain depth ===")
    print("tables created:", created)
    print("rows added:", counts)
    print("totals: Drivers=%d Citations=%d Hearings=%d Payments=%d CaseEvents=%d" % (
        len(rb["Drivers"]["data"]), len(rb["Citations"]["data"]), len(rb["Hearings"]["data"]),
        len(rb["Payments"]["data"]), len(rb["CaseEvents"]["data"])))
    print("ERBTables +%d | ERBFields +%d" % (et_added, ef_added))
    print("FK consistency unresolved:", len(bad), bad[:10])
    if bad:
        print("\nABORT."); sys.exit(1)
    if DRY:
        print("\n[dry-run] no write"); return
    with open(RB, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB)


if __name__ == "__main__":
    main()
