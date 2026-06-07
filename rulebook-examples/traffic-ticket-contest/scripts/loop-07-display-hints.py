#!/usr/bin/env python3
"""
LOOP 7 — Encode per-screen UI design decisions AS DATA in 3 new first-class
tables so the future frontend can render generically:
  - ScreenLayouts: one per nav screen (layout kind, primary view, role visibility).
  - ScreenSections: sections within a screen (header, detail, related, actions).
  - FieldDisplayHints: per (table, field) presentation (label, widget, format,
    column order, sortable/filterable/searchable, redaction by role).

ScreenLayouts are derived from PlatformNaviation (one per screen with a
PrimaryTable). FieldDisplayHints are derived from each domain/admin table's real
schema fields, so every hint's table.field exists. Adds ERBTables/ERBFields
catalog rows for the 3 new tables.

  python3 scripts/loop-07-display-hints.py --dry-run
  python3 scripts/loop-07-display-hints.py
"""
import json, os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T19:45:00Z"
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


AUDIT_FIELDS = [
    F("CreatedAt", "string", "raw", "Audit: created timestamp."),
    F("CreatedBy", "string", "raw", "Audit: created by."),
    F("ModifiedAt", "string", "raw", "Audit: modified timestamp."),
    F("ModifiedBy", "string", "raw", "Audit: modified by."),
]

SCHEMAS = {
    "ScreenLayouts": [
        F("ScreenLayoutId", "string", "raw", "PK = layout key (mirrors a nav RouteKey).", nullable=False),
        F("Name", "string", "calculated", "Echoes ScreenLayoutId.", formula="={{ScreenLayoutId}}"),
        F("RouteKey", "string", "relationship", "FK -> PlatformNaviation.", related="PlatformNaviation"),
        F("Title", "string", "raw", "Screen title."),
        F("LayoutKind", "string", "raw", "list | detail | split | dashboard | wizard | form."),
        F("PrimaryTable", "string", "raw", "The table/view this screen is built on."),
        F("PrimaryView", "string", "raw", "The vw_* the screen reads."),
        F("RoleVisibility", "string", "raw", "CSV of roles that can see this screen."),
        F("EmptyStateText", "string", "raw", "Message shown when there are no rows."),
        F("OrderIndex", "number", "raw", "Display order."),
    ] + AUDIT_FIELDS,
    "ScreenSections": [
        F("ScreenSectionId", "string", "raw", "PK = layout key + section handle.", nullable=False),
        F("Name", "string", "calculated", "Echoes ScreenSectionId.", formula="={{ScreenSectionId}}"),
        F("ScreenLayout", "string", "relationship", "FK -> ScreenLayouts.", related="ScreenLayouts"),
        F("Title", "string", "raw", "Section heading."),
        F("SectionKind", "string", "raw", "summary | fields | related | actions | timeline | chart."),
        F("RelatedTable", "string", "raw", "For related sections: the child table shown."),
        F("OrderIndex", "number", "raw", "Order within the screen."),
    ] + AUDIT_FIELDS,
    "FieldDisplayHints": [
        F("FieldDisplayHintId", "string", "raw", "PK = Table.Field.", nullable=False),
        F("Name", "string", "calculated", "Echoes FieldDisplayHintId.", formula="={{FieldDisplayHintId}}"),
        F("ERBTable", "string", "relationship", "FK -> ERBTables.", related="ERBTables"),
        F("FieldName", "string", "raw", "The field this hint styles."),
        F("Label", "string", "raw", "Human label for the field."),
        F("Widget", "string", "raw", "text | number | select | date | currency | badge | toggle | textarea | fk-link."),
        F("Format", "string", "raw", "Optional format (e.g. USD, YYYY-MM-DD, points)."),
        F("ColumnOrder", "number", "raw", "Order in list/grid views."),
        F("IsSortable", "boolean", "raw", "Sortable in list views."),
        F("IsFilterable", "boolean", "raw", "Filterable in list views."),
        F("IsSearchable", "boolean", "raw", "Included in free-text search."),
        F("ShowInList", "boolean", "raw", "Visible in the list/grid view."),
        F("ShowInDetail", "boolean", "raw", "Visible in the detail view."),
        F("RedactForExternalLlm", "boolean", "raw", "Masked for the external-llm role (PII)."),
    ] + AUDIT_FIELDS,
}

# PII fields to flag for redaction (table -> set of fields).
PII = {
    "Drivers": {"LicenseNumber", "DateOfBirth", "Address", "Phone", "Email"},
    "AppUsers": {"EmailAddress", "Name"},
    "Citations": {"DriverName"},
}

# Widget inference by field name/type.
def widget_for(fld):
    n = fld["name"].lower(); t = fld.get("type"); dt = fld.get("datatype")
    if t == "relationship": return "fk-link"
    if "amount" in n or "fee" in n or "fine" in n or "penalty" in n or "balance" in n: return "currency"
    if "date" in n or "at" == n[-2:] or n.endswith("date"): return "date"
    if "status" in n or "state" in n: return "badge"
    if dt == "boolean": return "toggle"
    if dt == "number": return "number"
    if "description" in n or "notes" in n or "reason" in n: return "textarea"
    return "text"


def label_for(name):
    out = []
    for i, c in enumerate(name):
        if c.isupper() and i and not name[i-1].isupper():
            out.append(" ")
        out.append(c)
    return "".join(out)


# Which tables get screens (domain + key admin). Pull from nav PrimaryTable.
def main():
    rb = json.load(open(RB))

    created = []
    for tname, fields in SCHEMAS.items():
        if tname not in rb:
            rb[tname] = {"schema": [dict(f) for f in fields], "data": []}
            created.append(tname)

    nav = rb["PlatformNaviation"]["data"]
    tables = {k: v for k, v in rb.items() if isinstance(v, dict) and "schema" in v}
    et_ids = {x["ERBTableId"] for x in rb["ERBTables"]["data"]}
    rk_ids = {n.get("RouteKey") for n in nav}

    # 1. ScreenLayouts: one per nav row that has a PrimaryTable and is a screen (not a bare world)
    layouts = rb["ScreenLayouts"]["data"]
    have_l = {l["ScreenLayoutId"] for l in layouts}
    ladded = 0
    layout_keys = []
    for n in nav:
        rk = n.get("RouteKey")
        ptable = n.get("PrimaryTable")
        if not rk or not ptable:
            continue
        if rk in have_l:
            layout_keys.append(rk); continue
        lvl = n.get("NavLevel")
        kind = "dashboard" if "dashboard" in rk or rk in ("deadlines", "assistant-costs") else \
               "detail" if rk.endswith("-detail") or ":" in (n.get("Route") or "") else \
               "list"
        layouts.append(audit({
            "ScreenLayoutId": rk, "RouteKey": rk, "Title": n.get("DisplayName"),
            "LayoutKind": kind, "PrimaryTable": ptable,
            "PrimaryView": n.get("PrimaryView") or ("vw_" + ptable.lower()),
            "RoleVisibility": n.get("RoleVisibility"),
            "EmptyStateText": f"No {n.get('DisplayName','records').lower()} yet.",
            "OrderIndex": n.get("SortOrder") or 0,
        }))
        have_l.add(rk); layout_keys.append(rk); ladded += 1

    # 2. ScreenSections: standard sections per layout (summary + fields + actions; +related/timeline for detail)
    sections = rb["ScreenSections"]["data"]
    have_s = {s["ScreenSectionId"] for s in sections}
    sadded = 0
    for l in layouts:
        lid = l["ScreenLayoutId"]; kind = l.get("LayoutKind")
        secs = []
        if kind == "list":
            secs = [("toolbar", "Filters & Actions", "actions", ""), ("grid", "Records", "fields", "")]
        elif kind == "detail":
            secs = [("summary", "Summary", "summary", ""), ("fields", "Details", "fields", ""),
                    ("timeline", "History", "timeline", "CaseEvents"), ("actions", "Actions", "actions", "")]
        elif kind == "dashboard":
            secs = [("metrics", "Key Metrics", "chart", ""), ("urgent", "Needs Attention", "fields", "")]
        else:
            secs = [("body", "Content", "fields", "")]
        for i, (h, title, skind, rel) in enumerate(secs):
            sid = f"{lid}--{h}"
            if sid in have_s:
                continue
            sections.append(audit({
                "ScreenSectionId": sid, "ScreenLayout": lid, "Title": title,
                "SectionKind": skind, "RelatedTable": rel, "OrderIndex": i + 1,
            }))
            have_s.add(sid); sadded += 1

    # 3. FieldDisplayHints: per real field of each table that backs a screen
    hints = rb["FieldDisplayHints"]["data"]
    have_h = {h["FieldDisplayHintId"] for h in hints}
    hadded = 0
    screen_tables = {l["PrimaryTable"] for l in layouts if l.get("PrimaryTable") in tables}
    # also include related tables referenced in sections
    for s in sections:
        if s.get("RelatedTable") in tables:
            screen_tables.add(s["RelatedTable"])
    for tname in sorted(screen_tables):
        sch = tables[tname]["schema"]
        fs = sch["fields"] if isinstance(sch, dict) else sch
        col = 0
        for fld in fs:
            fn = fld["name"]
            if fn in ("CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "ModifiedByModel"):
                continue
            hid = f"{tname}.{fn}"
            if hid in have_h:
                continue
            col += 1
            is_pk = fn.endswith("Id") and fn[:-2].lower() in tname.lower()
            hints.append(audit({
                "FieldDisplayHintId": hid, "ERBTable": tname, "FieldName": fn,
                "Label": label_for(fn), "Widget": widget_for(fld),
                "Format": ("USD" if widget_for(fld) == "currency" else
                           "YYYY-MM-DD" if widget_for(fld) == "date" else ""),
                "ColumnOrder": col,
                "IsSortable": fld.get("type") in ("raw", "calculated", "lookup"),
                "IsFilterable": fld.get("type") in ("raw", "relationship", "lookup"),
                "IsSearchable": fld.get("datatype") == "string" and fld.get("type") == "raw",
                "ShowInList": col <= 6 and not is_pk,
                "ShowInDetail": True,
                "RedactForExternalLlm": fn in PII.get(tname, set()),
            }))
            have_h.add(hid); hadded += 1

    # 4. ERBTables/ERBFields catalog for the 3 new tables
    et = rb["ERBTables"]["data"]; ef = rb["ERBFields"]["data"]
    have_et = {x["ERBTableId"] for x in et}; have_ef = {x["ERBFieldId"] for x in ef}
    et_added = ef_added = 0
    for tname, fields in SCHEMAS.items():
        if tname not in have_et:
            et.append(audit({
                "ERBTableId": tname, "TableName": tname,
                "Description": f"UI design-as-data table: {tname}.",
                "ERBPackage": "platform-meta", "Platform": "ticket-portal",
                "IsLicensed": True, "FieldCount": len(fields), "IsCatalog": True,
                "AdminCRUD": "CRUD", "ManagerCRUD": "RU",
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

    # consistency
    bad = []
    for l in layouts:
        if l.get("RouteKey") not in rk_ids: bad.append(("layout.RouteKey", l["ScreenLayoutId"]))
        if l.get("PrimaryTable") not in tables: bad.append(("layout.PrimaryTable", l["ScreenLayoutId"]))
    for s in sections:
        if s.get("ScreenLayout") not in have_l: bad.append(("section.layout", s["ScreenSectionId"]))
        if s.get("RelatedTable") and s["RelatedTable"] not in tables: bad.append(("section.related", s["ScreenSectionId"]))
    for h in hints:
        if h.get("ERBTable") not in et_ids: bad.append(("hint.table", h["FieldDisplayHintId"]))

    print("=== LOOP 7: display hints ===")
    print("tables created:", created)
    print("ScreenLayouts +%d (%d) | ScreenSections +%d (%d) | FieldDisplayHints +%d (%d)" % (
        ladded, len(layouts), sadded, len(sections), hadded, len(hints)))
    print("ERBTables +%d | ERBFields +%d" % (et_added, ef_added))
    print("screens with layouts:", len(layouts), "| tables with field hints:", len(screen_tables))
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
