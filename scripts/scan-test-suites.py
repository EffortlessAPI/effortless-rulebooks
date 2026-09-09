#!/usr/bin/env python3
"""Seed and refresh the TestSuites registry from what is actually on disk.

Two different kinds of column live on a TestSuites row and this script treats
them differently:

* DECLARED (`IsRegistered`, `ExpectedSubstrateCount`, `Runner`, `SuiteKind`,
  `Notes`) — a statement of intent. This script SEEDS them when it creates a
  row and never touches them again. Changing whether a suite is part of the
  corpus is a human edit to the rulebook, not something a file count decides.
* WITNESSED (`AnswerKeyCount`, `HasEffortlessJson`, `HasPostgresBootstrap`,
  `LastScannedOn`) — refreshed on every scan. These exist so the derived
  `RegistrationState` can say WHY a registered suite is not yet runnable
  ("needs-answer-keys") instead of the runner silently skipping it.

Rows are never deleted; a project that disappears from disk is reported as a
hard error rather than being quietly dropped.

Usage:
    python3 scripts/scan-test-suites.py effortless-rulebook/effortless-rulebook.json

Exits nonzero if a modeled project directory is missing.
"""

from __future__ import annotations

import datetime as dt
import json
import sys
from collections import OrderedDict
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent

# The repo-root suites: platform tests that are not about any one project.
# These are the "root conformance harness that should always work".
ROOT_SUITES = [
    {
        "TestSuiteId": "suite-root-formula-parser",
        "Domain": "domain-root",
        "SuiteKind": "pytest",
        "Runner": "python3 -m pytest orchestration/tests -q",
        "IsRegistered": True,
        "ExpectedSubstrateCount": 0,
        "Notes": "The formula-dialect regression suite behind orchestration/formula_parser.py. "
                 "Answer-key generation for every domain depends on it, so it is the one suite "
                 "that must be green before a corpus run means anything.",
    },
]


def domain_dir_for(row: dict) -> Path:
    """Resolve a RulebookDomains row to its directory. RelativePath is modeled
    data — if it does not resolve, that is a real inconsistency, not something
    to paper over with a guess."""
    rel = row.get("RelativePath")
    if not rel:
        raise SystemExit(f"{row['DomainId']} has no RelativePath — cannot locate the project on disk.")
    return (REPO_ROOT / rel).resolve()


def witness(project_dir: Path) -> dict:
    answer_keys = project_dir / "testing" / "answer-keys"
    key_count = len([p for p in answer_keys.iterdir() if p.is_file()]) if answer_keys.is_dir() else 0
    return {
        "AnswerKeyCount": key_count,
        "HasEffortlessJson": (project_dir / "effortless.json").is_file(),
        "HasPostgresBootstrap": (project_dir / "postgres-bootstrap" / "reset-rulebook-db.sh").is_file(),
    }


def seed_row(row: dict, project_dir: Path, obs: dict) -> dict:
    """Build a brand-new TestSuites row. The DECLARED columns are seeded from
    evidence ONCE, here; later scans leave them alone."""
    slug = row["DomainId"][len("domain-"):]
    excepted = bool(row.get("IsIntentionalException"))

    # ExpectedSubstrateCount is a declaration. Seed it from the harness's own
    # record of what it graded last time when that record exists; otherwise 0,
    # meaning "nobody has declared what this suite should grade yet".
    results_path = project_dir / "testing" / "_substrate_results.json"
    if results_path.is_file():
        expected = len(json.loads(results_path.read_text(encoding="utf-8")))
    else:
        expected = 0

    if excepted:
        registered, note = False, (
            "Intentional exception in RulebookDomains: this container carries no rulebook of its own, "
            "so there is nothing for the conformance harness to grade.")
    elif not obs["HasEffortlessJson"]:
        registered, note = False, "No effortless.json — the project cannot be built, so it cannot be graded."
    else:
        registered, note = True, "Seeded by scan-test-suites.py from a project with an effortless.json."

    return OrderedDict([
        ("TestSuiteId", f"suite-{slug}"),
        ("Domain", row["DomainId"]),
        ("SuiteKind", "conformance"),
        ("Runner", f"python3 scripts/run-conformance.py {slug}"),
        ("IsRegistered", registered),
        ("ExpectedSubstrateCount", expected),
        ("Notes", note),
    ])


def main() -> None:
    if len(sys.argv) != 2:
        raise SystemExit(__doc__)
    path = Path(sys.argv[1])
    rb = json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)

    if "TestSuites" not in rb:
        raise SystemExit("TestSuites table missing — run scripts/migrate-phase6-corpus-testing.py first.")

    suites = rb["TestSuites"]["data"]
    by_id = {r["TestSuiteId"]: r for r in suites}
    scanned_on = dt.datetime.now().isoformat(timespec="seconds")

    created, refreshed = 0, 0

    for spec in ROOT_SUITES:
        row = by_id.get(spec["TestSuiteId"])
        if row is None:
            row = OrderedDict(spec)
            suites.append(row)
            by_id[row["TestSuiteId"]] = row
            created += 1
        # A pytest suite has no project prerequisites; witness that plainly
        # rather than leaving the columns null.
        row["AnswerKeyCount"] = 0
        row["HasEffortlessJson"] = True
        row["HasPostgresBootstrap"] = True
        row["LastScannedOn"] = scanned_on
        refreshed += 1

    for domain in rb["RulebookDomains"]["data"]:
        if domain.get("Kind") == "root":
            continue  # the root's own suites are the ROOT_SUITES above
        project_dir = domain_dir_for(domain)
        if not project_dir.is_dir():
            raise SystemExit(
                f"{domain['DomainId']} models {domain['RelativePath']} but that directory does not exist. "
                f"Fix the RulebookDomains row (or restore the directory) before scanning test suites.")
        obs = witness(project_dir)
        suite_id = f"suite-{domain['DomainId'][len('domain-'):]}"
        row = by_id.get(suite_id)
        if row is None:
            row = seed_row(domain, project_dir, obs)
            suites.append(row)
            by_id[suite_id] = row
            created += 1
        row.update(obs)
        row["LastScannedOn"] = scanned_on
        refreshed += 1

    path.write_text(json.dumps(rb, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    registered = [r for r in suites if r.get("IsRegistered")]
    # Summary only — the authoritative version of every one of these counts is a
    # vw_TestSuites column (RunnableFlag / GradableFlag / RegistrationState). This
    # scan runs BEFORE the build that refreshes those views, so it prints its own
    # count of the rows it just wrote rather than querying a stale view.
    runnable = [r for r in registered if r["SuiteKind"] == "pytest" or r.get("HasEffortlessJson")]
    gradable = [r for r in runnable if r["SuiteKind"] == "pytest" or r.get("AnswerKeyCount", 0) > 0]
    print(f"TestSuites: {len(suites)} rows ({created} created, {refreshed} witnessed) @ {scanned_on}")
    print(f"  registered:      {len(registered)}")
    print(f"  runnable:        {len(runnable)}")
    print(f"  already graded:  {len(gradable)}")
    print(f"  never exercised: {len(runnable) - len(gradable)} (their first run generates answer keys)")
    for r in sorted(registered, key=lambda r: r["TestSuiteId"]):
        if r not in runnable:
            print(f"    {r['TestSuiteId']:<48} not runnable: no effortless.json")


if __name__ == "__main__":
    main()
