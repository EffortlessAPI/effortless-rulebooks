#!/usr/bin/env python3
"""Reconcile the RulebookFields catalog against the rulebook's real schemas.

The catalog is a census of every field in this rulebook. It is DERIVED, never
hand-maintained: this script walks the actual table schemas and rewrites the
catalog rows to match.

The one thing that is NOT derived is `InventedForQuestion` — that is authored
provenance, recording which role question motivated a field's existence. It is
preserved across reconciliation and re-attached by RulebookFieldId.

Fails loudly (exit 1) under --check if the catalog has drifted, so a stale
catalog can never silently misreport what fields exist.

Usage:
    reconcile_field_catalog.py [-i RULEBOOK] [--check]
"""
from __future__ import annotations

import argparse
import json
import sys
from collections import OrderedDict
from pathlib import Path

CATALOG = "RulebookFields"
DEFAULT_RB = "effortless-rulebook/procedural-knowledge-ontology-rulebook.json"
EXTENSION_IRI = "urn:effortless:pko-extension#RulebookField"

# Tables that describe the model itself rather than the procedural domain.
META_TABLES = {"WitnessLoops", "RoleQuestions", "RulebookFields", "__meta__",
               "ERBVersions", "ERBCustomizations"}


def load(path: Path) -> OrderedDict:
    with path.open() as fh:
        return json.load(fh, object_pairs_hook=OrderedDict)


def table_names(rb: OrderedDict) -> list[str]:
    return [k for k, v in rb.items() if isinstance(v, dict) and "schema" in v]


def build_rows(rb: OrderedDict, provenance: dict[str, str]) -> list[OrderedDict]:
    """Walk every schema and emit one catalog row per field."""
    rows: list[OrderedDict] = []
    for table in table_names(rb):
        for field in rb[table]["schema"]:
            if not isinstance(field, dict):
                continue
            fid = f"{table}.{field['name']}"
            rows.append(OrderedDict([
                ("RulebookFieldId", fid),
                ("TargetTable", table),
                ("FieldName", field["name"]),
                ("FieldType", field.get("type")),
                ("Datatype", field.get("datatype")),
                ("Formula", field.get("formula")),
                ("InventedForQuestion", provenance.get(fid)),
                ("SemanticTypeIri", EXTENSION_IRI),
            ]))
    return rows


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("-i", "--input", default=DEFAULT_RB)
    ap.add_argument("--check", action="store_true",
                    help="Exit 1 if the catalog is out of sync instead of rewriting it.")
    args = ap.parse_args()

    path = Path(args.input)
    rb = load(path)

    if CATALOG not in rb:
        print(f"error: {CATALOG} table not found in {path}", file=sys.stderr)
        return 1

    # Preserve authored provenance across the rebuild.
    provenance = {
        r["RulebookFieldId"]: r.get("InventedForQuestion")
        for r in rb[CATALOG].get("data", [])
        if r.get("InventedForQuestion")
    }

    fresh = build_rows(rb, provenance)
    current = rb[CATALOG].get("data", [])

    if args.check:
        if json.dumps(current, sort_keys=True) != json.dumps(fresh, sort_keys=True):
            cur_ids = {r.get("RulebookFieldId") for r in current}
            new_ids = {r["RulebookFieldId"] for r in fresh}
            missing = sorted(new_ids - cur_ids)
            stale = sorted(cur_ids - new_ids)
            print("error: RulebookFields catalog has drifted from the real schemas.",
                  file=sys.stderr)
            if missing:
                print(f"  fields in rulebook but not catalog ({len(missing)}): "
                      f"{', '.join(missing[:10])}", file=sys.stderr)
            if stale:
                print(f"  fields in catalog but not rulebook ({len(stale)}): "
                      f"{', '.join(stale[:10])}", file=sys.stderr)
            if not missing and not stale:
                print("  same field set, but definitions differ (type/datatype/formula).",
                      file=sys.stderr)
            return 1
        print(f"catalog in sync: {len(fresh)} fields")
        return 0

    rb[CATALOG]["data"] = fresh
    with path.open("w") as fh:
        json.dump(rb, fh, indent=2)
        fh.write("\n")

    witnessed = sum(1 for r in fresh if r.get("InventedForQuestion"))
    derived = sum(1 for r in fresh
                  if r.get("FieldType") in {"calculated", "lookup", "aggregation"})
    domain = sum(1 for r in fresh if r["TargetTable"] not in META_TABLES)
    print(f"catalog reconciled: {len(fresh)} fields across {len(table_names(rb))} tables")
    print(f"  domain fields : {domain}")
    print(f"  derived       : {derived}")
    print(f"  witnessed     : {witnessed} (invented for a role question)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
