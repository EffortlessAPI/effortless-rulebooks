#!/usr/bin/env python3
"""Run the registered test corpus and record the outcome as first-class rulebook rows.

Selects targets from the `TestSuites` registry (not from a filesystem sweep — a
suite runs because it is DECLARED registered), then for each one walks the same
three phases and writes the result after every transition so the run can be
watched live and reattached to after a page reload:

    build       `effortless build` in the project directory
    db          createdb + postgres-bootstrap/reset-rulebook-db.sh
    conformance the cross-substrate harness, via scripts/run-conformance.py

Live artifact (the explorer tails this):
    orchestration/corpus-runs/<run-id>/status.json
    orchestration/corpus-runs/<run-id>/logs/<slug>.log

At the end it appends one `CorpusRuns` row and one `CorpusDomainRuns` row per
target to the root rulebook, moves the `IsLatest` flag onto the new run, and
runs ONE root `effortless build` so every derived score recomputes. Per-project
conformance rows are written by run-conformance.py as each project finishes,
so they survive even if a later project blows up.

Builds are sequential on purpose: every `effortless build` goes through the
ssotme-proxy on :4242 and concurrent builds corrupt each other.

Usage:
    python3 scripts/run-corpus.py --mode build-only
    python3 scripts/run-corpus.py --mode full --kind example
    python3 scripts/run-corpus.py --mode full --only acme-llc,star-trek
    python3 scripts/run-corpus.py --mode full --note "baseline after the 09-09 refactor"
"""

from __future__ import annotations

import argparse
import datetime as dt
import json
import os
import subprocess
import sys
import time
from collections import OrderedDict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from erb_project import build_project, find_domain_dir, first_error, reset_db  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parent.parent
RULEBOOK_PATH = REPO_ROOT / "effortless-rulebook" / "effortless-rulebook.json"
RUNS_DIR = REPO_ROOT / "orchestration" / "corpus-runs"

PENDING, RUNNING, PASS, FAIL, SKIPPED = "pending", "running", "pass", "fail", "skipped"


# ---------------------------------------------------------------------------
# Target selection — from the registry, never from a directory sweep
# ---------------------------------------------------------------------------
def select_targets(rulebook: dict, kinds: list[str] | None, only: list[str] | None) -> list[dict]:
    domains = {d["DomainId"]: d for d in rulebook["RulebookDomains"]["data"]}
    targets = []
    for suite in rulebook["TestSuites"]["data"]:
        if not suite.get("IsRegistered"):
            continue
        # Mirrors the IsRunnable formula. This process runs BEFORE the build that
        # would refresh vw_TestSuites, so it reads the rows it was handed.
        if suite["SuiteKind"] != "pytest" and not suite.get("HasEffortlessJson"):
            continue
        domain_id = suite.get("Domain")
        if not domain_id:
            raise SystemExit(f"{suite['TestSuiteId']} has no Domain — every suite must name the project it tests.")
        domain = domains.get(domain_id)
        if domain is None:
            raise SystemExit(f"{suite['TestSuiteId']} references {domain_id}, which is not a RulebookDomains row.")
        slug = "root" if domain["Kind"] == "root" else domain_id[len("domain-"):]
        if kinds and domain["Kind"] not in kinds:
            continue
        if only and slug not in only:
            continue
        targets.append({"suite": suite, "domain": domain, "slug": slug})
    if only:
        missing = set(only) - {t["slug"] for t in targets}
        if missing:
            raise SystemExit(
                f"--only named {sorted(missing)}, which matched no registered, runnable suite. "
                f"Check TestSuites.IsRegistered for those projects.")
    if not targets:
        raise SystemExit("no registered, runnable suites matched the selection — nothing to run.")
    return targets


# ---------------------------------------------------------------------------
# Live status file
# ---------------------------------------------------------------------------
class Status:
    """The run's live artifact. Written atomically after every phase transition
    so a reader never sees a half-written file."""

    def __init__(self, run_dir: Path, run_id: str, mode: str, note: str, targets: list[dict]):
        self.path = run_dir / "status.json"
        self.doc = OrderedDict([
            ("run_id", run_id),
            ("mode", mode),
            ("note", note),
            ("pid", os.getpid()),
            ("started_on", dt.datetime.now().isoformat(timespec="seconds")),
            ("finished_on", None),
            ("current", None),
            ("target_count", len(targets)),
            ("domains", OrderedDict(
                (t["slug"], OrderedDict([
                    ("slug", t["slug"]),
                    ("domain_id", t["domain"]["DomainId"]),
                    ("domain_name", t["domain"].get("DomainName", t["slug"])),
                    ("kind", t["domain"]["Kind"]),
                    ("suite_id", t["suite"]["TestSuiteId"]),
                    ("suite_kind", t["suite"]["SuiteKind"]),
                    ("state", PENDING),
                    ("build", PENDING),
                    ("db", PENDING),
                    ("conformance", PENDING),
                    ("conformance_outcome", None),
                    ("substrates_tested", None),
                    ("substrates_passed", None),
                    ("conformance_run_id", None),
                    ("duration_seconds", None),
                    ("first_error", None),
                    ("log_path", str((run_dir / "logs" / f"{t['slug']}.log").relative_to(REPO_ROOT))),
                ])) for t in targets)),
        ])
        self.flush()

    def domain(self, slug: str) -> dict:
        return self.doc["domains"][slug]

    def flush(self) -> None:
        tmp = self.path.with_suffix(".json.tmp")
        tmp.write_text(json.dumps(self.doc, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        tmp.replace(self.path)


# ---------------------------------------------------------------------------
# One project
# ---------------------------------------------------------------------------
def run_pytest_suite(suite: dict, log) -> None:
    """A repo-root pytest suite. Its Runner is declared in the rulebook; this
    executes exactly that, from the repo root."""
    cmd = suite["Runner"].split()
    log(f"[pytest] {' '.join(cmd)}")
    proc = subprocess.Popen(cmd, cwd=str(REPO_ROOT), stdout=subprocess.PIPE,
                            stderr=subprocess.STDOUT, text=True, bufsize=1)
    lines, last = [], ""
    for line in proc.stdout:
        line = line.rstrip("\n")
        if line:
            last = line
            lines.append(line)
        log(line)
    code = proc.wait()
    if code != 0:
        # pytest's own summary line is the useful one, ahead of the generic patterns.
        summary = next((l for l in reversed(lines) if " failed" in l and "passed" in l), None)
        raise SystemExit(f"pytest exited {code}: {summary or first_error(lines, last)}")


def run_conformance_for(slug: str, log) -> dict:
    """Delegate to the existing single-project path. --skip-build because this
    runner does ONE root build at the end instead of forty. The rows it appends
    to the rulebook land immediately, so a later project failing cannot lose
    this project's result."""
    cmd = [sys.executable, "scripts/run-conformance.py", slug, "--skip-build"]
    log(f"[conformance] {' '.join(cmd)}")
    proc = subprocess.Popen(cmd, cwd=str(REPO_ROOT), stdout=subprocess.PIPE,
                            stderr=subprocess.STDOUT, text=True, bufsize=1)
    lines, last_json, last = [], None, ""
    for line in proc.stdout:
        line = line.rstrip("\n")
        if line:
            last = line
            lines.append(line)
            if line.startswith("{"):
                try:
                    last_json = json.loads(line)
                except json.JSONDecodeError:
                    pass
        log(line)
    code = proc.wait()
    if code != 0:
        raise SystemExit(f"run-conformance.py exited {code}: {first_error(lines, last)}")
    if last_json is None:
        raise SystemExit("run-conformance.py exited 0 but printed no summary JSON — refusing to record a guess.")
    return last_json


def run_one(target: dict, mode: str, status: Status, run_dir: Path) -> None:
    slug = target["slug"]
    row = status.domain(slug)
    log_path = run_dir / "logs" / f"{slug}.log"
    started = time.time()

    with log_path.open("w", encoding="utf-8") as fh:
        def log(line: str) -> None:
            fh.write(line + "\n")
            fh.flush()
            print(f"  {slug} | {line}", flush=True)

        def phase(name: str, value: str) -> None:
            row[name] = value
            status.flush()

        def fail(name: str, error: str) -> None:
            phase(name, FAIL)
            row["first_error"] = error
            row["state"] = "done"
            row["duration_seconds"] = round(time.time() - started, 1)
            status.flush()
            log(f"[FAIL:{name}] {error}")

        row["state"] = RUNNING
        status.doc["current"] = slug
        status.flush()

        # A pytest suite has no project build and no database — say so as
        # "skipped", never as a silent pass.
        if target["suite"]["SuiteKind"] == "pytest":
            row["build"] = SKIPPED
            row["db"] = SKIPPED
            phase("conformance", RUNNING)
            try:
                run_pytest_suite(target["suite"], log)
            except SystemExit as e:
                row["conformance_outcome"] = "tests-failed"
                fail("conformance", str(e))
                return
            row["conformance_outcome"] = "tests-passed"
            phase("conformance", PASS)
            row["state"] = "done"
            row["duration_seconds"] = round(time.time() - started, 1)
            status.flush()
            return

        domain_dir = find_domain_dir(slug)

        phase("build", RUNNING)
        try:
            build_project(slug, domain_dir, log)
        except SystemExit as e:
            fail("build", str(e))
            return
        phase("build", PASS)

        phase("db", RUNNING)
        try:
            reset_db(slug, domain_dir, log)
        except SystemExit as e:
            fail("db", str(e))
            return
        phase("db", PASS)

        if mode == "build-only":
            row["conformance"] = SKIPPED
            row["conformance_outcome"] = SKIPPED
            row["state"] = "done"
            row["duration_seconds"] = round(time.time() - started, 1)
            status.flush()
            return

        phase("conformance", RUNNING)
        try:
            summary = run_conformance_for(slug, log)
        except SystemExit as e:
            row["conformance_outcome"] = "harness-error"
            fail("conformance", str(e))
            return

        tested = summary.get("substrates") or 0
        passed = summary.get("substrates_passed") or 0
        row["conformance_run_id"] = summary.get("run_id")
        row["substrates_tested"] = tested
        row["substrates_passed"] = passed

        # The harness exiting 0 only means it RAN. Green means every substrate
        # it graded agreed with the answer keys — that is the bar the corpus is
        # being driven toward, so anything less is recorded as a failure here
        # rather than as a pass with a footnote.
        if tested == 0:
            row["conformance_outcome"] = "harness-error"
            fail("conformance", "the harness graded zero substrates")
            return
        if passed < tested:
            row["conformance_outcome"] = "substrate-mismatch"
            fail("conformance", f"{tested - passed} of {tested} substrates did not score 100")
            return

        row["conformance_outcome"] = "all-substrates-passed"
        phase("conformance", PASS)
        row["state"] = "done"
        row["duration_seconds"] = round(time.time() - started, 1)
        status.flush()


# ---------------------------------------------------------------------------
# Recording
# ---------------------------------------------------------------------------
def record(status: Status, targets: list[dict]) -> None:
    """Append the fan-out to the rulebook. Re-reads the file fresh because
    run-conformance.py has been appending ConformanceRuns rows to it throughout
    this run."""
    rb = json.loads(RULEBOOK_PATH.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)
    doc = status.doc
    run_id = doc["run_id"]

    existing = {r["CorpusRunId"] for r in rb["CorpusRuns"]["data"]}
    if run_id in existing:
        raise SystemExit(f"CorpusRunId {run_id!r} is already recorded — refusing to write it twice.")

    for prior in rb["CorpusRuns"]["data"]:
        prior["IsLatest"] = False

    rb["CorpusRuns"]["data"].append(OrderedDict([
        ("CorpusRunId", run_id),
        ("Mode", doc["mode"]),
        ("StartedOn", doc["started_on"]),
        ("FinishedOn", doc["finished_on"]),
        ("IsLatest", True),
        ("TargetCount", doc["target_count"]),
        ("StatusPath", str(status.path.relative_to(REPO_ROOT))),
        ("Notes", doc["note"]),
    ]))

    by_slug = {t["slug"]: t for t in targets}
    for slug, row in doc["domains"].items():
        target = by_slug[slug]
        rb["CorpusDomainRuns"]["data"].append(OrderedDict([
            ("CorpusDomainRunId", f"{run_id}:{slug}"),
            ("CorpusRun", run_id),
            ("Domain", target["domain"]["DomainId"]),
            ("Suite", target["suite"]["TestSuiteId"]),
            ("BuildStatus", row["build"]),
            ("DbStatus", row["db"]),
            ("ConformanceStatus", row["conformance"]),
            ("ConformanceOutcome", row["conformance_outcome"]),
            ("SubstratesTested", row["substrates_tested"]),
            ("SubstratesPassed", row["substrates_passed"]),
            ("DurationSeconds", row["duration_seconds"]),
            ("FirstError", row["first_error"]),
            ("LogPath", row["log_path"]),
            ("ConformanceRun", row["conformance_run_id"]),
        ]))

    RULEBOOK_PATH.write_text(json.dumps(rb, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"[corpus] recorded {run_id}: 1 CorpusRuns row, {len(doc['domains'])} CorpusDomainRuns rows", flush=True)


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--mode", choices=["build-only", "full"], default="full",
                    help="build-only stops after the database reset; full also grades every substrate")
    ap.add_argument("--kind", action="append", choices=["root", "toy", "example"],
                    help="restrict to declared kinds (repeatable)")
    ap.add_argument("--only", help="comma-separated slugs to run (must be registered and runnable)")
    ap.add_argument("--note", default="", help="why this fan-out was run")
    ap.add_argument("--skip-record", action="store_true",
                    help="run and write status.json, but do not append rulebook rows or build the root")
    args = ap.parse_args()

    rulebook = json.loads(RULEBOOK_PATH.read_text(encoding="utf-8"))
    if "TestSuites" not in rulebook:
        raise SystemExit("TestSuites table missing — run scripts/migrate-phase6-corpus-testing.py first.")

    only = [s.strip() for s in args.only.split(",") if s.strip()] if args.only else None
    targets = select_targets(rulebook, args.kind, only)

    run_id = "corpus-" + dt.datetime.now().strftime("%Y%m%d-%H%M%S")
    run_dir = RUNS_DIR / run_id
    (run_dir / "logs").mkdir(parents=True, exist_ok=True)
    status = Status(run_dir, run_id, args.mode, args.note, targets)

    print(f"[corpus] {run_id} mode={args.mode} targets={len(targets)}", flush=True)
    print(f"[corpus] status: {status.path.relative_to(REPO_ROOT)}", flush=True)

    for i, target in enumerate(targets, 1):
        print(f"[corpus] ({i}/{len(targets)}) {target['slug']}", flush=True)
        run_one(target, args.mode, status, run_dir)

    status.doc["current"] = None
    status.doc["finished_on"] = dt.datetime.now().isoformat(timespec="seconds")
    status.flush()

    green = sum(1 for r in status.doc["domains"].values()
                if FAIL not in (r["build"], r["db"], r["conformance"]))
    print(f"[corpus] {green}/{len(targets)} green", flush=True)

    if args.skip_record:
        print("[corpus] --skip-record: rulebook untouched", flush=True)
        return

    record(status, targets)
    print("[corpus] running the single root effortless build so every derived score recomputes", flush=True)
    result = subprocess.run(["effortless", "build"], cwd=str(REPO_ROOT))
    if result.returncode != 0:
        raise SystemExit(f"root effortless build exited {result.returncode}")
    print(json.dumps({"run_id": run_id, "targets": len(targets), "green": green,
                      "status_path": str(status.path.relative_to(REPO_ROOT))}), flush=True)


if __name__ == "__main__":
    main()
