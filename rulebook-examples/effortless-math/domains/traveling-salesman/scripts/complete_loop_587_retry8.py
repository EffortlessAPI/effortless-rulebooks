#!/usr/bin/env python3
"""Close TSP loop 587 after retry 8 commissions generated Postgres.

Retry 7 reached the complete generated backend: CLI, transpilers, database,
views, data, and every substantive peer comparison passed. Its only failure was
a stale hand-maintained frontier-count assertion (8/0/4) after loops 587-596 had
expanded the canonical ledger to 16/0/13. Retry 8 uses rulebook-derived frontier
expectations and records retry 7 as a failed test-oracle attempt, not a substrate
or mathematical failure.
"""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path
from typing import Any

import complete_loop_587 as base
import complete_loop_587_retry7 as prior

REPO = base.REPO
DOMAIN = base.DOMAIN
RULEBOOK = base.RULEBOOK
CONTRACT = base.CONTRACT
README = base.README
SUCCESS_JSON = base.SUCCESS_JSON
RETRY7_LOG = DOMAIN / "testing" / "postgres-commissioning-retry-7.log"
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-8"
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry-8")
CLI_ARCHIVE = os.environ.get(
    "EFFORTLESS_CLI_SOURCE",
    "https://github.com/EffortlessAPI/cli/archive/1551cb82d5a8992ad09e4c82d08e28493f92f7b4.tar.gz",
)
CLI_COMMIT = os.environ.get(
    "EFFORTLESS_CLI_COMMIT", "1551cb82d5a8992ad09e4c82d08e28493f92f7b4"
)
CLI_VERSION = os.environ.get("EFFORTLESS_CLI_VERSION", "2026-06-09.06.13")


def load(path: Path) -> dict[str, Any]:
    return json.loads(path.read_text())


def write(path: Path, value: Any) -> None:
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n")


def rename_ids(table: dict[str, Any], id_field: str, old: str, new: str) -> None:
    for row in table["data"]:
        if old in row[id_field]:
            row[id_field] = row[id_field].replace(old, new)


def main() -> None:
    if not RETRY7_LOG.is_file():
        raise FileNotFoundError(
            "retry-7 generated-substrate transcript is missing; its successful build "
            "and stale-oracle failure must remain auditable"
        )

    prior.SUCCESS_RUN_ID = SUCCESS_RUN_ID
    prior.RUN_ID = RUN_ID
    prior.CLI_ARCHIVE = CLI_ARCHIVE
    prior.CLI_COMMIT = CLI_COMMIT
    prior.CLI_VERSION = CLI_VERSION
    prior.main()

    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)
    success = load(SUCCESS_JSON)
    retry7_sha = base.sha256_file(RETRY7_LOG)

    base.upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [
            {
                "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-7",
                "TSPLoop": "tsp-loop-587",
                "Substrate": "POSTGRES_GENERATED_VIEWS",
                "AttemptedAt": "29632306954",
                "BuildCommand": (
                    "npm install -g <pinned GitHub archive>; bash start.sh build; "
                    "bash start.sh db; bash start.sh test; bash start.sh show"
                ),
                "BuildSucceeded": True,
                "DatabaseInitialized": True,
                "ConformanceSucceeded": False,
                "Status": "FAILED",
                "FailureReason": (
                    "The generated backend, database initialization, and all substantive "
                    "graph/tour/bound/optimality/invariant/search comparisons passed. "
                    "The final harness assertion was stale: it expected the pre-expansion "
                    "frontier ledger 8/0/4 while the canonical rulebook and Postgres both "
                    "correctly contained 16 total, 0 imported, 13 closed."
                ),
            }
        ],
    )

    rename_ids(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        "loop-587-retry-7-",
        "loop-587-retry-8-",
    )
    rename_ids(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        "loop-587-retry-7-",
        "loop-587-retry-8-",
    )
    base.upsert_rows(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        [
            {
                "TSPArtifactId": "loop-587-retry-7-generated-substrate-log",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-7",
                "ArtifactKind": "GENERATED_SUBSTRATE_AND_STALE_ORACLE_TRANSCRIPT",
                "RelativePath": str(RETRY7_LOG.relative_to(REPO)),
                "SHA256": retry7_sha,
                "IsPresent": True,
            }
        ],
    )
    base.upsert_rows(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-7-frontier-answer-key",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-7",
                "CheckKind": "TEST_ORACLE_ALIGNMENT",
                "SubjectId": "TSPFrontierObligations aggregate",
                "ExpectedValue": "8,0,4 (stale hard-coded oracle)",
                "ActualValue": "16,0,13 (canonical rulebook and Postgres)",
                "Status": "FAIL",
            }
        ],
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["AfterState"] = (
        "Attempts 1-6 preserved missing-CLI, npm, and workflow-boundary failures. "
        "Retry 7 successfully generated and initialized Postgres and passed every "
        "substantive peer comparison, then failed on a stale hard-coded frontier count. "
        "Retry 8 derived that expectation from canonical frontier rows, reran generation, "
        "database initialization, and all peer checks, and closed the substrate obligation."
    )
    loop["WitnessSummary"] = (
        "Pinned CLI archive generation and live Postgres/Python conformance passed with "
        "frontier counts derived from the rulebook; all seven earlier attempts remain rows."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_8"

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_8"

    contract.setdefault("ArtifactHashes", {})[
        "retry_7_generated_substrate_transcript"
    ] = f"sha256:{retry7_sha}"

    success["successful_attempt"] = 8
    success["attempt_history"] = [
        {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
        {"attempt": 2, "status": "FAILED", "reason": "npm step failed before transcript persistence"},
        {"attempt": 3, "status": "FAILED", "reason": "npm ENOTDIR in shared hosted-toolcache prefix"},
        {"attempt": 4, "status": "FAILED", "reason": "npm ENOTDIR persisted in isolated prefix during git-dependency preparation"},
        {"attempt": 5, "status": "FAILED", "reason": "archive installed; downstream step failed before transcript persistence"},
        {"attempt": 6, "status": "FAILED", "reason": "unbound cross-step npm-prefix variable stopped before build"},
        {"attempt": 7, "status": "FAILED", "reason": "backend passed; stale hard-coded frontier-count oracle failed"},
        {"attempt": 8, "status": "SUCCEEDED", "reason": "rulebook-derived oracle and full live substrate conformance passed"},
    ]
    success.setdefault("hashes", {})[
        "retry_7_generated_substrate_transcript"
    ] = retry7_sha

    base.write(RULEBOOK, rulebook)
    base.run([sys.executable, str(base.FORMATTER), str(RULEBOOK)], cwd=REPO)
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success)

    readme = README.read_text().replace(
        "Attempts 1–6 remain recorded as blocked or failed evidence. Retry 6 proved the pinned archive and CLI preflight work but stopped before generation on an unbound cross-step environment variable. Retry 7 exported the npm prefix at job scope, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. Artifact hashes and all available failure/success transcripts are retained.",
        "Attempts 1–7 remain recorded as blocked or failed evidence. Retry 7 generated Postgres and passed every substantive comparison, but correctly remained failed because a stale hand-maintained frontier-count oracle disagreed with both canonical data and Postgres. Retry 8 derives that expectation from the rulebook, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed full Python/Postgres conformance. Artifact hashes and all available failure/success transcripts are retained.",
    )
    README.write_text(readme)

    base.run([sys.executable, str(base.RECONCILER)])
    base.run([sys.executable, str(base.VALIDATOR)])

    print("TSP loop 587 commissioning retry 8: PASS")
    print(f"  cli_archive={CLI_ARCHIVE}")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")


if __name__ == "__main__":
    main()
