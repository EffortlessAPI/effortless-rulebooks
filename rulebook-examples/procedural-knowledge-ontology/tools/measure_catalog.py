#!/usr/bin/env python3
"""Measure every table and field in Postgres and write the measurements to the catalog.

The substrate is the oracle. This script counts; it never evaluates a formula:

  RulebookTables.MeasuredRowCount           rows in vw_<table>
  RulebookFields.MeasuredSubstantiveCount   rows whose value is substantive: not blank, not false, not zero
  RulebookFields.MeasuredDistinctValueCount distinct values, blank counted as one value

Those raw measurements feed IsDiscriminating / HasMeasuredData / HasMeasuredRows, which
ClaimEvidence.IsValid reads. A table with no generated view (the transpiler-ignored
__meta__, ERBVersions, ERBCustomizations) and a closure field (materialized as its own
vw_*_closure view) are written as NULL and listed, so they can never count as evidence. Any other mismatch between the rulebook and the database
raises: a stale database must not produce plausible-looking measurements.

Run after `effortless build && bash init-db.sh`, then build again.
"""
from __future__ import annotations

import os
import re
import subprocess
import time
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from rulebook_edit import dump, load  # noqa: E402

DB = os.environ.get("PGDATABASE") or "erb_procedural_knowledge_ontology"
NO_VIEW = {"__meta__", "ERBVersions", "ERBCustomizations"}
# The coverage ledger itself can never be evidence, and its views are the slowest to compute.
NOT_EVIDENCE = {"SourceArticles", "ArticleClaims", "ClaimEvidence"}


def norm(name: str) -> str:
    return re.sub(r"[^a-z0-9]", "", name.lower())


def psql(sql: str) -> list[list[str]]:
    out = subprocess.run(["psql", "-X", "-qtA", "-F", "\x1f", "-v", "ON_ERROR_STOP=1", "-d", DB, "-c", sql],
                         capture_output=True, text=True)
    if out.returncode != 0:
        raise SystemExit(f"psql failed:\n{sql[:400]}\n{out.stderr}")
    return [line.split("\x1f") for line in out.stdout.splitlines() if line]


def main() -> int:
    views = {}
    for (v,) in psql("SELECT table_name FROM information_schema.views WHERE table_schema='public' "
                     "AND table_name LIKE 'vw\\_%' AND table_name NOT LIKE '%\\_closure%'"):
        key = norm(v[3:])
        if key in views:
            raise SystemExit(f"two views normalize to one table: {views[key]} and {v}")
        views[key] = v
    columns: dict[str, dict[str, str]] = {}
    for v, c in psql("SELECT table_name, column_name FROM information_schema.columns "
                     "WHERE table_schema='public' AND table_name LIKE 'vw\\_%'"):
        columns.setdefault(v, {})[norm(c)] = c

    rb = load()
    tables = [k for k, val in rb.items() if isinstance(val, dict) and "schema" in val]
    table_rows, field_rows = {}, {}
    unmeasured = []
    for t in tables:
        if t in NO_VIEW or t in NOT_EVIDENCE:
            unmeasured.append(t)
            continue
        view = views.get(norm(t))
        if view is None:
            raise SystemExit(f"{t}: no vw_* view in {DB}. Rebuild and reload before measuring.")
        selects = ["count(*)"]
        names = []
        for f in rb[t]["schema"]:
            if f.get("type") == "closure":
                # A closure field materializes as its own vw_*_closure view, not a column.
                unmeasured.append(f"{t}.{f['name']}")
                continue
            col = columns[view].get(norm(f["name"]))
            if col is None:
                raise SystemExit(f"{t}.{f['name']}: no column in {view}. Rebuild and reload before measuring.")
            q = f'"{col}"'
            # Substantive: a blank, a false or a zero cannot show that a concept is present.
            selects.append(f"count(*) FILTER (WHERE {q} IS NOT NULL AND {q}::text NOT IN ('', 'false', 'f') "
                           f"AND {q}::text !~ '^-?0+(\\.0+)?$')")
            selects.append(f"count(DISTINCT coalesce(NULLIF({q}::text, ''), E'\\\\x01blank'))")
            names.append(f["name"])
        started = time.monotonic()
        (vals,) = psql(f"SELECT {', '.join(selects)} FROM {view}")
        elapsed = time.monotonic() - started
        if elapsed > 5:
            print(f"  slow view: {view} took {elapsed:.0f}s", flush=True)
        table_rows[t] = int(vals[0])
        for i, fname in enumerate(names):
            field_rows[f"{t}.{fname}"] = (int(vals[1 + 2 * i]), int(vals[2 + 2 * i]))

    # Re-read immediately before writing: another writer may have touched the file.
    rb = load()
    missing_tables = []
    for r in rb["RulebookTables"]["data"]:
        r["MeasuredRowCount"] = table_rows.get(r["RulebookTableId"])
    have_tables = {r["RulebookTableId"] for r in rb["RulebookTables"]["data"]}
    missing_tables = [t for t in tables if t not in have_tables]
    for r in rb["RulebookFields"]["data"]:
        nb, dv = field_rows.get(r["RulebookFieldId"], (None, None))
        r["MeasuredSubstantiveCount"] = nb
        r["MeasuredDistinctValueCount"] = dv
    catalog_ids = {r["RulebookFieldId"] for r in rb["RulebookFields"]["data"]}
    missing_fields = [fid for fid in field_rows if fid not in catalog_ids]
    if missing_tables or missing_fields:
        raise SystemExit(f"catalog is behind the schema. tables without a RulebookTables row: "
                         f"{missing_tables}; fields without a catalog row: {missing_fields[:10]} "
                         f"({len(missing_fields)}). Run tools/reconcile_field_catalog.py and add the table rows.")
    dump(rb)
    print(f"measured {len(table_rows)} tables and {len(field_rows)} fields in {DB}")
    print(f"not measurable (no generated view): {', '.join(unmeasured)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
