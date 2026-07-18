#!/usr/bin/env python3
"""Close TSP loop 587 after retry 6 commissions generated Postgres.

Retry 5 proved the pinned archive can be installed and the `effortless` binary
can run, but its downstream build/conformance step exited nonzero before that
workflow revision persisted the transcript. Retry 6 adds failure persistence;
when it succeeds this wrapper retains retry 5 as a failed execution attempt and
records retry 6 as the first complete substrate certificate.
"""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path
from typing import Any

import complete_loop_587 as base
import complete_loop_587_retry5 as prior

REPO = base.REPO
DOMAIN = base.DOMAIN
RULEBOOK = base.RULEBOOK
CONTRACT = base.CONTRACT
README = base.README
SUCCESS_JSON = base.SUCCESS_JSON
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-6"
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry-6")
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
    prior.SUCCESS_RUN_ID = SUCCESS_RUN_ID
    prior.RUN_ID = RUN_ID
    prior.CLI_ARCHIVE = CLI_ARCHIVE
    prior.CLI_COMMIT = CLI_COMMIT
    prior.CLI_VERSION = CLI_VERSION
    prior.main()

    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)
    success = load(SUCCESS_JSON)

    base.upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [
            {
                "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-5",
                "TSPLoop": "tsp-loop-587",
                "Substrate": "POSTGRES_GENERATED_VIEWS",
                "AttemptedAt": "29631879613",
                "BuildCommand": (
                    "npm install -g <pinned GitHub archive>; bash start.sh build; "
                    "bash start.sh db; bash start.sh test; bash start.sh show"
                ),
                "BuildSucceeded": False,
                "DatabaseInitialized": False,
                "ConformanceSucceeded": False,
                "Status": "FAILED",
                "FailureReason": (
                    "The pinned archive installed successfully and the CLI preflight passed, "
                    "but the downstream generated-build/database/conformance step exited "
                    "nonzero in GitHub Actions run 29631879613. That workflow revision did "
                    "not persist its build transcript; retry 6 was introduced specifically "
                    "to make the downstream failure surface durable."
                ),
            }
        ],
    )

    rename_ids(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        "loop-587-retry-5-",
        "loop-587-retry-6-",
    )
    rename_ids(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        "loop-587-retry-5-",
        "loop-587-retry-6-",
    )
    base.upsert_rows(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-5-downstream-build",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-5",
                "CheckKind": "GENERATED_BUILD_DATABASE_CONFORMANCE",
                "SubjectId": "start.sh build/db/test/show",
                "ExpectedValue": "PASS",
                "ActualValue": "FAIL",
                "Status": "FAIL",
            }
        ],
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["AfterState"] = (
        "Attempts 1-4 preserved the missing-CLI and npm git-preparation failures. "
        "Retry 5 installed the pinned archive but exposed an unpersisted downstream "
        "build/conformance failure. Retry 6 persisted that boundary, generated the "
        "vw_* projection, initialized erb_traveling_salesman, and passed Python/Postgres conformance."
    )
    loop["WitnessSummary"] = (
        "The pinned CLI archive generated the backend and live Postgres agrees with Python. "
        "Every earlier blocked or failed execution remains an explicit attempt row."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_6"

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_6"

    success["successful_attempt"] = 6
    success["attempt_history"] = [
        {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
        {"attempt": 2, "status": "FAILED", "reason": "npm step failed before transcript persistence"},
        {"attempt": 3, "status": "FAILED", "reason": "npm ENOTDIR in shared hosted-toolcache prefix"},
        {"attempt": 4, "status": "FAILED", "reason": "npm ENOTDIR persisted in isolated prefix during git-dependency preparation"},
        {"attempt": 5, "status": "FAILED", "reason": "archive installed; downstream build/conformance failed before transcript persistence"},
        {"attempt": 6, "status": "SUCCEEDED", "reason": "archive install and durable generated-substrate conformance completed"},
    ]

    base.write(RULEBOOK, rulebook)
    base.run([sys.executable, str(base.FORMATTER), str(RULEBOOK)], cwd=REPO)
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success)

    readme = README.read_text().replace(
        "Attempts 1–4 remain recorded as blocked or failed evidence. Retry 5 installed the pinned GitHub archive `https://github.com/EffortlessAPI/cli/archive/1551cb82d5a8992ad09e4c82d08e28493f92f7b4.tar.gz` with npm, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. The archive path bypasses npm's failing git-dependency preparation while retaining the exact CLI commit; artifact hashes and failure/success transcripts are retained.",
        "Attempts 1–5 remain recorded as blocked or failed evidence. Retry 5 proved the pinned GitHub archive installs correctly but exposed a downstream build/conformance failure. Retry 6 retained that boundary, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. Artifact hashes and all available failure/success transcripts are retained.",
    )
    README.write_text(readme)

    base.run([sys.executable, str(base.RECONCILER)])
    base.run([sys.executable, str(base.VALIDATOR)])

    print("TSP loop 587 commissioning retry 6: PASS")
    print(f"  cli_archive={CLI_ARCHIVE}")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")


if __name__ == "__main__":
    main()
