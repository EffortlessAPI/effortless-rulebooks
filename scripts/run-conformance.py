#!/usr/bin/env python3
"""Run the cross-substrate conformance harness for one project and record the
result as first-class rows in the root rulebook (ConformanceRuns / ConformanceResults).

This does NOT reimplement the harness. It shells out to the existing
orchestration/test-orchestrator.py (its DOMAIN_DIR resolution checks
rulebook-examples/ then toy-rulebooks/ like orchestrate.sh's find_domain_dir
already does; its run_substrate_test() also sets ERB_NO_OPEN=1 on each
substrate subprocess so a run never pops one browser window per substrate,
matching what orchestrate.sh's own substrate loop already did), reads the
harness's own testing/_substrate_results.json output, and appends rows to
effortless-rulebook/effortless-rulebook.json the same way scripts/record-finding.py
and scripts/scan-project-slots.py add witnessed rows: edit the JSON directly, then
`effortless build` projects the new rows into Postgres. There is no direct-to-Postgres
write path here — the rulebook JSON is the only place a new row can originate,
per this repo's "Rulebook JSON is HEAD" doctrine.

After recording, this also shells out to orchestration/generate-report.py (again
unmodified except for now honoring ERB_NO_OPEN=1 to skip its browser popup) to
build the aggregate <domain>/orchestration-report.html, then prints a curated,
ctrl+click-able list of report file:// paths (the aggregate report first, then
every substrate's own substrate-report.html) instead of opening any of them.

Usage:
    python3 scripts/run-conformance.py <project-slug> [--skip-build]

<project-slug> must match a RulebookDomains.DomainId's slug (the part after
"domain-"), e.g. "acme-llc" for DomainId "domain-acme-llc", and must resolve to
a directory under rulebook-examples/ or toy-rulebooks/ that test-orchestrator.py
can run against (an Effortless project with a conformance-enabled effortless.json).

--skip-build records the rulebook rows but does not run `effortless build` — use
this only when the caller (e.g. the explorer's trigger endpoint) will run the
build itself right after, to avoid running it twice.

Fails loudly: no fallback path is substituted if the project has no
effortless.json, no testing/_substrate_results.json is produced, or the harness
process exits non-zero.
"""

from __future__ import annotations

import argparse
import datetime as dt
import json
import os
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from erb_project import build_project, reset_db  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parent.parent
RULEBOOK_PATH = REPO_ROOT / "effortless-rulebook" / "effortless-rulebook.json"
ORCHESTRATOR_DIR = REPO_ROOT / "orchestration"
ORCHESTRATOR_SCRIPT = ORCHESTRATOR_DIR / "test-orchestrator.py"


def find_domain_dir(slug: str) -> Path:
    """Mirror orchestrate.sh's find_domain_dir(): rulebook-examples/ first,
    then toy-rulebooks/. Fails loudly if the slug is in neither."""
    examples_dir = REPO_ROOT / "rulebook-examples" / slug
    toy_dir = REPO_ROOT / "toy-rulebooks" / slug
    if examples_dir.is_dir():
        return examples_dir
    if toy_dir.is_dir():
        return toy_dir
    raise SystemExit(
        f"project slug {slug!r} not found under rulebook-examples/ or toy-rulebooks/ "
        f"(checked {examples_dir} and {toy_dir})"
    )


def resolve_domain_id(rulebook: dict, slug: str) -> str:
    """Find the RulebookDomains row for this slug, matched by RelativePath.
    Fails loudly if the project isn't modeled as a domain yet."""
    candidates = [
        f"rulebook-examples/{slug}/",
        f"toy-rulebooks/{slug}/",
    ]
    for row in rulebook["RulebookDomains"]["data"]:
        if row.get("RelativePath") in candidates:
            return row["DomainId"]
    raise SystemExit(
        f"no RulebookDomains row has RelativePath in {candidates!r}. "
        f"Add a RulebookDomains row for this project before recording conformance runs."
    )


def run_harness(slug: str, domain_dir: Path) -> Path:
    """Invoke the existing, unreimplemented harness against this domain.
    Returns the path to the testing/_substrate_results.json it produced."""
    effortless_json = domain_dir / "effortless.json"
    if not effortless_json.is_file():
        raise SystemExit(
            f"{effortless_json} does not exist — this project has no "
            f"effortless.json and cannot be conformance-tested."
        )
    if not ORCHESTRATOR_SCRIPT.is_file():
        raise SystemExit(f"harness script missing: {ORCHESTRATOR_SCRIPT}")

    db_name = f"erb_{slug.replace('-', '_')}"
    env = dict(os.environ)
    env["ERB_DOMAIN"] = slug
    env.setdefault("DATABASE_URL", f"postgresql://postgres@localhost:5432/{db_name}")
    # Suppress every per-substrate report popup and the aggregate report's own
    # popup below — this path (CLI-triggered or portal-triggered) prints a
    # curated report-link list instead of opening up to ~17 browser windows.
    env["ERB_NO_OPEN"] = "1"
    # test-orchestrator.py resolves its own TESTING_DIR/RULEBOOK_PATH from
    # ERB_DOMAIN, but the individual substrate take-test.sh scripts it shells
    # out to (execution-substrates/*/take-test.py)
    # do NOT inherit those computed paths automatically — they read
    # ERB_TESTING_DIR / ERB_RULEBOOK_PATH directly from the environment. This
    # matches exactly what orchestrate.sh's run_substrates() exports before
    # invoking the same machinery; without it every substrate fails with
    # "ERB_TESTING_DIR is not set" and grades 0% on missing test-answers.
    env["ERB_TESTING_DIR"] = str(domain_dir / "testing")
    env["ERB_RULEBOOK_PATH"] = str(domain_dir / "effortless-rulebook" / f"{slug}-rulebook.json")

    print(f"[run-conformance] running test-orchestrator.py for ERB_DOMAIN={slug}", flush=True)
    result = subprocess.run(
        [sys.executable, str(ORCHESTRATOR_SCRIPT)],
        cwd=str(ORCHESTRATOR_DIR),
        env=env,
    )
    if result.returncode != 0:
        raise SystemExit(
            f"test-orchestrator.py exited {result.returncode} for ERB_DOMAIN={slug}. "
            f"Not recording a partial/guessed result — fix the harness failure and rerun."
        )

    results_path = domain_dir / "testing" / "_substrate_results.json"
    if not results_path.is_file():
        raise SystemExit(
            f"test-orchestrator.py exited 0 but did not produce {results_path}. "
            f"Refusing to record a conformance run with no results file."
        )
    return results_path


def run_report_generator(slug: str, domain_dir: Path) -> Path:
    """Build the aggregate orchestration-report.html the same way orchestrate.sh
    does (generate-report.py), with ERB_NO_OPEN=1 already set on the parent
    env so it prints its file:// path instead of popping a browser window."""
    report_script = ORCHESTRATOR_DIR / "generate-report.py"
    if not report_script.is_file():
        raise SystemExit(f"report generator missing: {report_script}")

    rulebook_path = domain_dir / "effortless-rulebook" / f"{slug}-rulebook.json"
    output_path = domain_dir / "orchestration-report.html"
    print(f"[run-conformance] generating aggregate report for {slug}", flush=True)
    result = subprocess.run(
        [
            sys.executable, str(report_script),
            "--rulebook", str(rulebook_path),
            "--output", str(output_path),
        ],
        cwd=str(ORCHESTRATOR_DIR),
        env=dict(os.environ, ERB_NO_OPEN="1"),
    )
    if result.returncode != 0:
        raise SystemExit(f"generate-report.py exited {result.returncode} for {slug}")
    return output_path


def print_curated_report_list(domain_dir: Path, aggregate_report: Path) -> None:
    """Print a ctrl+click-able list of report paths instead of opening a
    browser window per substrate. The aggregate report is listed first; it
    is the one file worth opening by default. Per-substrate reports are
    listed for anyone who wants to drill into one substrate's detail."""
    print(flush=True)
    print("[run-conformance] Reports (ctrl+click to open):", flush=True)
    print(f"  Aggregate : file://{aggregate_report.resolve()}", flush=True)
    substrates_dir = REPO_ROOT / "execution-substrates"
    for substrate_dir in sorted(substrates_dir.iterdir()):
        report = substrate_dir / "substrate-report.html"
        if report.is_file():
            print(f"  {substrate_dir.name:<20}: file://{report.resolve()}", flush=True)
    print(flush=True)


def build_rows(domain_id: str, slug: str, results: dict, ran_on: str) -> tuple[dict, list[dict]]:
    run_id = f"run-{slug}-{ran_on.replace(':', '').replace('-', '').replace('T', '-')}"
    run_row = {
        "ConformanceRunId": run_id,
        "Domain": domain_id,
        "RanOn": ran_on,
        "RawResultsJson": json.dumps(results, ensure_ascii=False),
    }
    result_rows = []
    for substrate_name, substrate in sorted(results.items()):
        # last_run reflects the most recent attempt, which can be a transient
        # infra error (a cold cloud transpiler timing out) even when the
        # substrate's own translation is correct. last_successful_run is the
        # harness's own durable record of the last attempt that actually
        # graded the substrate's output — that is what a conformance SCORE
        # means, so record scoring fields from it, not from last_run.
        last_run = substrate.get("last_run") or {}
        # The harness only writes last_successful_run when a substrate scored a
        # full 100 (test-orchestrator.update_run_metadata), so it is null for
        # every substrate that has never been perfect. When it IS present it is
        # the record to score from: last_run can be a transient infra error (a
        # cold cloud transpiler timing out) on a substrate whose translation is
        # correct. When it is absent the substrate has genuinely never passed,
        # and last_run carries the real status and score — the per-field counts
        # simply do not exist in that record, so they are recorded as null
        # rather than invented.
        last_success = substrate.get("last_successful_run") or {}
        test_results = last_success.get("test_results") or {}
        if last_success:
            status = last_success.get("status", "")
            score = test_results.get("score", last_success.get("score"))
            duration = last_success.get("duration_seconds")
        else:
            status = last_run.get("status", "")
            score = last_run.get("score")
            duration = last_run.get("duration_seconds")
        result_rows.append({
            "ConformanceResultId": f"{run_id}:{substrate_name}",
            "Run": run_id,
            "SubstrateName": substrate_name,
            "Status": status,
            "Score": score,
            "FieldsTested": test_results.get("total_fields_tested"),
            "FieldsPassed": test_results.get("fields_passed"),
            "FieldsFailed": test_results.get("fields_failed"),
            "DurationSeconds": duration,
        })
    return run_row, result_rows


def record_rows(rulebook_path: Path, run_row: dict, result_rows: list[dict]) -> None:
    rulebook = json.loads(rulebook_path.read_text(encoding="utf-8"))
    existing_run_ids = {r["ConformanceRunId"] for r in rulebook["ConformanceRuns"]["data"]}
    if run_row["ConformanceRunId"] in existing_run_ids:
        raise SystemExit(f"ConformanceRunId {run_row['ConformanceRunId']!r} already recorded")
    rulebook["ConformanceRuns"]["data"].append(run_row)
    rulebook["ConformanceResults"]["data"].extend(result_rows)
    rulebook_path.write_text(json.dumps(rulebook, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def run_effortless_build() -> None:
    print("[run-conformance] running effortless build at repo root", flush=True)
    result = subprocess.run(["effortless", "build"], cwd=str(REPO_ROOT))
    if result.returncode != 0:
        raise SystemExit(f"effortless build exited {result.returncode}")


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("slug", help="project slug, e.g. acme-llc")
    ap.add_argument("--skip-build", action="store_true",
                    help="record rows but do not run the ROOT effortless build (the caller will)")
    ap.add_argument("--project-build", action="store_true",
                    help="run `effortless build` in the project directory before grading")
    ap.add_argument("--reset-db", action="store_true",
                    help="createdb + postgres-bootstrap/reset-rulebook-db.sh before grading")
    args = ap.parse_args()

    domain_dir = find_domain_dir(args.slug)
    rulebook = json.loads(RULEBOOK_PATH.read_text(encoding="utf-8"))
    domain_id = resolve_domain_id(rulebook, args.slug)

    def log(line):
        print(line, flush=True)

    # Build and database-reset are opt-in so the existing single-project path
    # keeps its old behaviour (grade whatever is already in the DB), while the
    # corpus runner can ask for the full build -> reset -> grade sequence
    # without a second implementation of either phase.
    if args.project_build:
        build_project(args.slug, domain_dir, log)
    if args.reset_db:
        reset_db(args.slug, domain_dir, log)

    results_path = run_harness(args.slug, domain_dir)
    results = json.loads(results_path.read_text(encoding="utf-8"))
    if not results:
        raise SystemExit(f"{results_path} is empty — refusing to record a run with zero substrates")

    ran_on = dt.datetime.now().isoformat(timespec="seconds")
    run_row, result_rows = build_rows(domain_id, args.slug, results, ran_on)
    record_rows(RULEBOOK_PATH, run_row, result_rows)
    print(f"[run-conformance] recorded {run_row['ConformanceRunId']} with {len(result_rows)} substrate results")

    aggregate_report = run_report_generator(args.slug, domain_dir)
    print_curated_report_list(domain_dir, aggregate_report)

    if not args.skip_build:
        run_effortless_build()

    # The final stdout line is the machine-readable summary every caller parses
    # (the explorer's SSE plugin and scripts/run-corpus.py). substrates_passed
    # counts substrates that scored a full 100 — the same bar ConformanceResults.
    # IsPassing applies, so the corpus row and the view agree.
    passed = sum(1 for r in result_rows if r["Status"] == "success" and (r["Score"] or 0) >= 100)
    print(json.dumps({
        "run_id": run_row["ConformanceRunId"],
        "substrates": len(result_rows),
        "substrates_passed": passed,
        "report_path": str(aggregate_report),
    }))


if __name__ == "__main__":
    main()
