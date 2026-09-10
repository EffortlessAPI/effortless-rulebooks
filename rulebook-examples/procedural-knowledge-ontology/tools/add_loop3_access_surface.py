#!/usr/bin/env python3
"""Loop 3's field grants alone weren't enough: RoleSchemaViews (which views a
principal's Postgres schema actually gets) and AccessPolicies (row-level
security) are BOTH declared tables, not derived from FieldGrants. Without
rows here, the 4 new loop-3 tables exist, witness cleanly, and have field
grants -- but never actually appear as a queryable view in either principal's
narrowed schema, and RulebookTables.IsUnsecured would read TRUE for them
(every other real domain table has at least one AccessPolicy).

Idempotent: skips anything that already exists.
"""
from __future__ import annotations

import json
import re
from collections import OrderedDict
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
EXT = "urn:effortless:pko-extension#"
PRINCIPALS = [("principal-process-steward", "processsteward", "schema-process-steward"),
              ("principal-knowledge-authority", "knowledgeauthority", "schema-knowledge-authority")]
NEW_TABLES = ["ProcessMiningRuns", "Vocabularies", "VocabularyTerms", "KnowledgeBrokerLinks"]


def snake(name: str) -> str:
    s = re.sub(r"(?<!^)(?=[A-Z])", "_", name)
    return s.lower()


def main() -> int:
    with RB.open() as fh:
        rb = json.load(fh, object_pairs_hook=OrderedDict)

    # ---- RulebookTables ---------------------------------------------------
    rt = rb["RulebookTables"]["data"]
    have_rt = {r["RulebookTableId"] for r in rt}
    added_rt = 0
    for table in NEW_TABLES:
        if table in have_rt:
            continue
        physical = snake(table)
        rt.append(OrderedDict([
            ("RulebookTableId", table), ("TableName", table),
            ("PhysicalTable", physical), ("PhysicalView", f"vw_{physical}"),
            ("SubjectArea", "knowledge"), ("IsExtension", True),
            ("SemanticTypeIri", f"{EXT}RulebookTable"),
        ]))
        added_rt += 1

    # ---- RoleSchemaViews ----------------------------------------------------
    rsv = rb["RoleSchemaViews"]["data"]
    have_rsv = {r["RoleSchemaViewId"] for r in rsv}
    added_rsv = 0
    for principal, short, role_schema in PRINCIPALS:
        for table in NEW_TABLES:
            view_name = snake(table)
            rid = f"rsv-{principal.removeprefix('principal-')}-{view_name}"
            if rid in have_rsv:
                continue
            rsv.append(OrderedDict([
                ("RoleSchemaViewId", rid), ("RoleSchema", role_schema), ("Principal", principal),
                ("TargetTable", table), ("ViewName", view_name),
                ("SemanticTypeIri", f"{EXT}RoleSchemaView"),
            ]))
            have_rsv.add(rid)
            added_rsv += 1

    # ---- AccessPolicies -----------------------------------------------------
    ap = rb["AccessPolicies"]["data"]
    have_ap = {r["AccessPolicyId"] for r in ap}
    added_ap = 0
    for principal, short, _ in PRINCIPALS:
        for table in NEW_TABLES:
            view_name = snake(table)
            pid = f"pol-{short}-{view_name}-select"
            if pid in have_ap:
                continue
            ap.append(OrderedDict([
                ("AccessPolicyId", pid), ("Principal", principal), ("TargetTable", table),
                ("Command", "SELECT"), ("RowPredicate", ""), ("CheckPredicate", ""),
                ("Rationale", "Steward-level role: unrestricted read of this table, matching "
                              "this principal's existing broad reach across the model."),
                ("SemanticTypeIri", f"{EXT}AccessPolicy"),
            ]))
            have_ap.add(pid)
            added_ap += 1

    with RB.open("w") as fh:
        json.dump(rb, fh, indent=1, ensure_ascii=False)
        fh.write("\n")
    print(f"added {added_rt} RulebookTables, {added_rsv} RoleSchemaViews, {added_ap} AccessPolicies")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
