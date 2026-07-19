#!/usr/bin/env python3
"""Read the computed witness values out of Postgres and back into the rulebook.

The substrate is the oracle. Every value written here was COMPUTED by Postgres
from the rulebook's own formulas — nothing is recomputed in Python, per the
project doctrine that a conformance answer is produced by a substrate and never
re-derived by hand.

Two things get written:

1. RoleQuestions.WitnessedAnswer — for each question, the current reading of
   its predicates: how many rows the witness fires on, out of how many. This
   is what makes loop N+1 derivative of loop N rather than freshly imagined:
   the next round of questions is planned against materialized answers.

2. WitnessLoops.<stats> — the loop's own summary.

Usage: tools/extract_computed_answers.py [--loop 1]
"""
from __future__ import annotations

import argparse
import json
import re
import subprocess
import sys
from collections import OrderedDict
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
DB = "erb_procedural_knowledge_ontology"


def snake(name: str) -> str:
    return re.sub(r"(?<!^)(?=[A-Z])", "_", name).lower()


def psql(sql: str) -> str:
    out = subprocess.run(["psql", "-qtA", "-d", DB, "-c", sql],
                         capture_output=True, text=True)
    if out.returncode != 0:
        raise SystemExit(f"psql failed:\n{out.stderr}")
    return out.stdout.strip()


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--loop", type=int, default=1)
    args = ap.parse_args()

    with RB.open() as fh:
        rb = json.load(fh, object_pairs_hook=OrderedDict)

    # Map each question to the fields invented for it.
    by_question: dict[str, list[tuple[str, str]]] = {}
    for row in rb["RulebookFields"]["data"]:
        qid = row.get("InventedForQuestion")
        if qid:
            by_question.setdefault(qid, []).append(
                (row["TargetTable"], row["FieldName"]))

    # Which boolean witness columns actually exist in the substrate?
    existing = set()
    for line in psql(
        "SELECT table_name || '.' || column_name FROM information_schema.columns "
        "WHERE table_schema='public' AND table_name LIKE 'vw\\_%' "
        "AND data_type='boolean'"
    ).splitlines():
        if line:
            existing.add(line)

    written = 0
    for q in rb["RoleQuestions"]["data"]:
        qid = q["RoleQuestionId"]
        readings = []
        for table, field in sorted(by_question.get(qid, [])):
            key = f"vw_{snake(table)}.{snake(field)}"
            if key not in existing:
                continue  # not a boolean witness; nothing to read
            view, col = key.split(".")
            res = psql(f"SELECT count(*) FILTER (WHERE {col}), count(*) FROM {view}")
            if not res:
                continue
            fires, total = (int(x) for x in res.split("|"))
            readings.append(f"{table}.{field}={fires}/{total}")

        if readings:
            q["WitnessedAnswer"] = "; ".join(readings)
            written += 1
        else:
            q["WitnessedAnswer"] = None

    # Loop-level summary, also read from the substrate.
    fields_total = int(psql("SELECT count(*) FROM vw_rulebook_fields"))
    witnessed = int(psql("SELECT count(*) FROM vw_rulebook_fields WHERE is_witness"))
    derived = int(psql("SELECT count(*) FROM vw_rulebook_fields WHERE is_derived"))
    for loop in rb["WitnessLoops"]["data"]:
        if loop["LoopNumber"] == args.loop:
            loop["FieldsAfter"] = fields_total
            loop["WitnessedAfter"] = witnessed
            loop["DerivedAfter"] = derived

    with RB.open("w") as fh:
        json.dump(rb, fh, indent=2)
        fh.write("\n")

    print(f"wrote WitnessedAnswer for {written} of {len(rb['RoleQuestions']['data'])} questions")
    print(f"loop {args.loop}: {fields_total} fields, {derived} derived, {witnessed} witnessed")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
