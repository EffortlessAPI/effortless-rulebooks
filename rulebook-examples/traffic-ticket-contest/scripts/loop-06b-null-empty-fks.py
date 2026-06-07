#!/usr/bin/env python3
"""
loop-06b-null-empty-fks.py  (idempotent repair)

The transpiler does VALUE-BASED FK inference: a raw string column whose non-empty
values are all PKs of another table gets an emitted FK. When such a column also
holds EMPTY STRINGS (''), the load fails -- there is no PK '' to satisfy the FK.
An ABSENT key serializes to NULL (which the FK skips); an empty string does not.

This repair removes empty-string values on the optional-FK fields so they become
NULL. Detected fields (verified):
  TestCase.TargetFeature   -> ERBFeatures
  TestCase.TargetEndpoint  -> APIEndpoints
  TestCase.TargetTransition-> StateTransitionRules
  ScreenSections.RelatedTable -> ERBTables

We DELETE the key entirely when its value is '' (so it serializes to NULL/absent),
preserving every non-empty value. Idempotent: 0 changes once clean.

Run:
  python3 scripts/loop-06b-null-empty-fks.py --dry-run
  python3 scripts/loop-06b-null-empty-fks.py
"""
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
TTC_ROOT = os.path.dirname(HERE)
RB_PATH = os.path.join(TTC_ROOT, "effortless-rulebook",
                       "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv

# (table, field) pairs whose empty strings must become NULL/absent.
TARGETS = [
    ("TestCase", "TargetFeature"),
    ("TestCase", "TargetEndpoint"),
    ("TestCase", "TargetTransition"),
    ("ScreenSections", "RelatedTable"),
]


def main():
    rb = json.load(open(RB_PATH))
    total = 0
    per = {}
    for tname, fld in TARGETS:
        if tname not in rb:
            continue
        n = 0
        for row in rb[tname]["data"]:
            if fld in row and (row.get(fld) is None or str(row.get(fld)).strip() == ""):
                del row[fld]
                n += 1
        per[(tname, fld)] = n
        total += n

    print("=== loop-06b null-empty-fks ===")
    for (t, f), n in per.items():
        print(f"  {t}.{f}: stripped {n} empty-string values")
    print("total empty-string keys removed:", total)

    if DRY:
        print("\n[dry-run] no write")
        return
    with open(RB_PATH, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB_PATH)


if __name__ == "__main__":
    main()
