#!/usr/bin/env python3
"""Record the latest conformance-harness grades as rows in this project's rulebook.

The harness (orchestration/test-orchestrator.py, driven by
scripts/run-conformance.py) grades every substrate cell by cell and writes
testing/_conformance_grades.json. This script TRANSCRIBES that file into the
conformance tables added by tools/add_conformance_tables.py. It does not grade,
compare or recompute anything: every pass/fail it records was decided by the
harness, and every score, count and flag shown in the app is a formula on those
rows, computed by the substrates at the next build.

    ConformanceSubstrates  upserted (authored prose below + effortless.json)
    ConformanceRuns        appended; IsLatest moves to the new run
    SubstrateRunScores     appended
    TableConformance       replaced   (latest run only)
    FieldDisagreements     replaced   (latest run only)
    CellDisagreements      replaced   (latest run only; sampled per field)

Usage (from the project root, after the harness has run):
    python3 tools/record_conformance.py [--notes "..."]
then `effortless build` so compile-rulebook and Postgres compute the scores.

Fails loudly when the grades file is missing, when a graded entity or field is
not in the rulebook, or when a substrate in the grades has no descriptor below.
"""
from __future__ import annotations

import argparse
import datetime as dt
import json
import subprocess
import sys
from collections import Counter
from pathlib import Path

HERE = Path(__file__).resolve().parent.parent
REPO_ROOT = HERE.parent.parent
sys.path.insert(0, str(REPO_ROOT / "orchestration"))
from shared import to_snake_case  # noqa: E402  (the harness's own entity/field naming)

RB = HERE / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json"
GRADES = HERE / "testing" / "_conformance_grades.json"
EFFORTLESS_JSON = HERE / "effortless.json"
IRI = "urn:effortless:pko-extension#"
ANSWER_KEY_AUTHOR = "compile-rulebook"
CELL_SAMPLE_PER_FIELD = 20

# Authored prose: what each substrate is, for a curious reader. Transpiler and
# OutputFolder are read from effortless.json, not repeated here.
SUBSTRATES = {
    "compile-rulebook": dict(
        Label="compile-rulebook", SortOrder=0, Role="answer-key",
        Engine="Python formula engine (shared with rulebook-to-python)",
        HowItComputes="Runs first in the build and rewrites the rulebook in place: every calculated, lookup and aggregation value is computed across all tables to a fixed point and upserted into the rows. Those stored values become the answer keys. It is not graded against itself; when another substrate disagrees, either side can be the one that is wrong."),
    "effortless-postgres": dict(
        Label="PostgreSQL", SortOrder=1, Role="graded",
        Engine="PostgreSQL views and calc_* SQL functions",
        HowItComputes="rulebook-to-postgres emits a table, a set of calc_* functions and a vw_<entity> view per table. The database is reset from the rulebook, and the harness reads SELECT * from every view."),
    "effortless-python": dict(
        Label="Python", SortOrder=2, Role="graded",
        Engine="CPython",
        HowItComputes="rulebook-to-python emits an SDK with one computed property per derived field. The harness loads the blank test rows (raw fields only) and asks the SDK for every derived value."),
    "effortless-golang": dict(
        Label="Go", SortOrder=3, Role="graded",
        Engine="Go toolchain (compiled)",
        HowItComputes="rulebook-to-go emits a Go module. The harness compiles it with go build, runs it over the blank test rows, and reads back every derived value."),
    "effortless-typescript": dict(
        Label="TypeScript", SortOrder=4, Role="graded",
        Engine="Node.js running tsc output",
        HowItComputes="rulebook-to-typescript emits a typed SDK. The harness compiles it with tsc and runs it on Node over the blank test rows."),
    "effortless-entity-framework": dict(
        Label="C# / Entity Framework", SortOrder=5, Role="graded",
        Engine=".NET (C# Formula* properties)",
        HowItComputes="rulebook-to-entity-framework emits C# data classes whose Formula* properties compute derived fields. The harness compiles them with EF Core into a runner, loads the blank test rows into the generated context over a throwaway SQLite database, and reads every computed property back per row."),
    "effortless-xlsx": dict(
        Label="Excel", SortOrder=6, Role="graded",
        Engine="Spreadsheet formulas (cached values read with openpyxl)",
        HowItComputes="rulebook-to-xlsx compiles every formula into a real Excel formula over cells in the same workbook. The harness reads the workbook's cached values, after a LibreOffice recalc when LibreOffice is installed; a value that was never calculated reads as blank."),
    "effortless-owl": dict(
        Label="OWL / SHACL", SortOrder=7, Role="graded",
        Engine="SPARQL 1.1 (pyoxigraph) executing SHACL-AF rules",
        HowItComputes="rulebook-to-owl emits an ontology, an ABox of individuals carrying raw fields only, and one SHACL-AF SPARQL rule per derived field. The harness executes the rules in dependency order and reads the inferred triples back."),
}


def substrate_of(transpiler: dict) -> str:
    """Map an effortless.json step to the harness substrate name — the same
    mapping orchestration/shared.get_active_project_substrates applies."""
    if transpiler["Name"] == ANSWER_KEY_AUTHOR:
        return ANSWER_KEY_AUTHOR
    rp = (transpiler.get("RelativePath") or "").strip("/").rsplit("/", 1)[-1]
    return {"postgres-bootstrap": "effortless-postgres",
            "entity-framework": "effortless-entity-framework"}.get(rp, rp)


def git_head() -> str:
    out = subprocess.run(["git", "rev-parse", "--short", "HEAD"], cwd=HERE,
                         capture_output=True, text=True, check=True)
    return out.stdout.strip()


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--notes", default=None, help="Recorded on the ConformanceRuns row.")
    args = ap.parse_args()

    if not GRADES.is_file():
        raise SystemExit(f"{GRADES} does not exist. Run the harness first: "
                         f"python3 scripts/run-conformance.py procedural-knowledge-ontology --skip-build")
    grades = json.loads(GRADES.read_text(encoding="utf-8"))
    ran_on = dt.datetime.fromtimestamp(GRADES.stat().st_mtime).astimezone().isoformat(timespec="seconds")
    run_id = "run-" + ran_on[:19].replace("-", "").replace(":", "").replace("T", "-")

    unknown = sorted(set(grades) - set(SUBSTRATES))
    if unknown:
        raise SystemExit(f"graded substrates with no descriptor in {__file__}: {unknown}")

    # A folder can hold more than one step (postgres-bootstrap runs the
    # transpiler and then `-exec ./reset-rulebook-db.sh`); the substrate's tool
    # is the step that is not an -exec.
    steps = {substrate_of(t): t for t in json.loads(EFFORTLESS_JSON.read_text())["ProjectTranspilers"]
             if not t.get("IsDisabled") and not t["CommandLine"].startswith("-exec")}

    # Re-read immediately before writing: the rulebook is contended.
    rb = json.loads(RB.read_text(encoding="utf-8"))
    for table in ("ConformanceSubstrates", "ConformanceRuns", "SubstrateRunScores",
                  "TableConformance", "FieldDisagreements", "CellDisagreements"):
        if table not in rb:
            raise SystemExit(f"{table} is not in the rulebook. Run tools/add_conformance_tables.py first.")
    if any(r["ConformanceRunId"] == run_id for r in rb["ConformanceRuns"]["data"]):
        raise SystemExit(f"{run_id} is already recorded (grades file unchanged since the last recording).")

    table_of = {to_snake_case(t): t for t, v in rb.items() if isinstance(v, dict) and "schema" in v}
    field_of = {(t, to_snake_case(f["name"])): f["name"]
                for t, v in rb.items() if isinstance(v, dict) and "schema" in v for f in v["schema"]}
    field_ids = {r["RulebookFieldId"] for r in rb["RulebookFields"]["data"]}

    # --- substrates (upsert) ---
    existing = {r["ConformanceSubstrateId"]: r for r in rb["ConformanceSubstrates"]["data"]}
    for sid, desc in SUBSTRATES.items():
        if sid not in steps:
            continue
        row = existing.get(sid) or {"ConformanceSubstrateId": sid}
        row.update(desc)
        row["Transpiler"] = steps[sid]["CommandLine"].split()[0]
        row["OutputFolder"] = steps[sid]["RelativePath"]
        row["SemanticTypeIri"] = IRI + "ConformanceSubstrate"
        if sid not in existing:
            rb["ConformanceSubstrates"]["data"].append(row)

    # --- run + scores (append) ---
    for r in rb["ConformanceRuns"]["data"]:
        r["IsLatest"] = False
    rb["ConformanceRuns"]["data"].append({
        "ConformanceRunId": run_id,
        "RanOn": ran_on,
        "RulebookCommit": git_head(),
        "AnswerKeyAuthor": ANSWER_KEY_AUTHOR,
        "IsLatest": True,
        "Notes": args.notes,
        "SemanticTypeIri": IRI + "ConformanceRun",
    })

    tables, fields, cells = [], [], []
    for sid, g in grades.items():
        by = g["by_class"]
        rb["SubstrateRunScores"]["data"].append({
            "SubstrateRunScoreId": f"{run_id}|{sid}",
            "Run": run_id,
            "Substrate": sid,
            "HarnessError": g.get("error") or "",
            "DurationSeconds": round(g.get("elapsed_seconds") or 0, 1),
            "CellsTested": g["total_fields_tested"],
            "CellsPassed": g["fields_passed"],
            "CalculatedTested": by["calculated"]["tested"], "CalculatedPassed": by["calculated"]["passed"],
            "LookupTested": by["lookup"]["tested"], "LookupPassed": by["lookup"]["passed"],
            "AggregationTested": by["aggregation"]["tested"], "AggregationPassed": by["aggregation"]["passed"],
            "SemanticTypeIri": IRI + "SubstrateRunScore",
        })

        for entity, e in g["entities"].items():
            if entity not in table_of:
                raise SystemExit(f"graded entity {entity!r} ({sid}) has no rulebook table")
            table = table_of[entity]
            tc_id = f"{sid}|{table}"
            tables.append({
                "TableConformanceId": tc_id, "Run": run_id, "Substrate": sid, "RulebookTable": table,
                "RecordCount": e["total_records"], "DerivedFieldCount": len(e["computed_columns"]),
                "CellsTested": e["fields_tested"], "CellsPassed": e["fields_passed"],
                "IsMissingAnswerFile": bool(e["missing_file"]),
                "SemanticTypeIri": IRI + "TableConformance",
            })
            per_field: dict[str, list] = {}
            for fail in e["failures"]:
                per_field.setdefault(fail["field"], []).append(fail)
            for col, fails in per_field.items():
                if (table, col) not in field_of:
                    raise SystemExit(f"graded field {table}.{col} ({sid}) is not in the rulebook schema")
                fid = f"{table}.{field_of[(table, col)]}"
                if fid not in field_ids:
                    raise SystemExit(f"{fid} is not in RulebookFields. Run tools/reconcile_field_catalog.py first.")
                fd_id = f"{sid}|{fid}"
                fields.append({
                    "FieldDisagreementId": fd_id, "Substrate": sid, "RulebookField": fid,
                    "TableConformance": tc_id, "FieldClass": fails[0]["field_class"],
                    "CellsFailed": len(fails),
                    "DominantReason": Counter(x["reason"] for x in fails).most_common(1)[0][0],
                    "SemanticTypeIri": IRI + "FieldDisagreement",
                })
                for fail in fails[:CELL_SAMPLE_PER_FIELD]:
                    cells.append({
                        "CellDisagreementId": f"{fd_id}|{fail['pk']}", "FieldDisagreement": fd_id,
                        "RecordId": fail["pk"],
                        "ExpectedValue": json.dumps(fail["expected"], ensure_ascii=False, default=str),
                        "ActualValue": json.dumps(fail["actual"], ensure_ascii=False, default=str),
                        "Reason": fail["reason"],
                        "SemanticTypeIri": IRI + "CellDisagreement",
                    })

    rb["TableConformance"]["data"] = tables
    rb["FieldDisagreements"]["data"] = fields
    rb["CellDisagreements"]["data"] = cells

    with RB.open("w", encoding="utf-8") as fh:
        json.dump(rb, fh, indent=1, ensure_ascii=False)
        fh.write("\n")
    print(f"recorded {run_id}: {len(grades)} substrates, {len(tables)} table grades, "
          f"{len(fields)} field disagreements, {len(cells)} sampled cells")
    return 0


if __name__ == "__main__":
    sys.exit(main())
