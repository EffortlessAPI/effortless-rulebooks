#!/usr/bin/env python3
"""Close TSP loop 587 after retry 7 commissions generated Postgres.

Retry 6 installed and validated the CLI, but a diagnostic-shell variable was
not exported across Actions steps. The durable transcript proves no build was
attempted. Retry 7 moves the npm prefix into job-level environment state and,
when successful, records retry 6 as a workflow failure rather than a substrate
or semantic failure.
"""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path
from typing import Any

import complete_loop_587 as base
import complete_loop_587_retry6 as prior

REPO = base.REPO
DOMAIN = base.DOMAIN
RULEBOOK = base.RULEBOOK
CONTRACT = base.CONTRACT
README = base.README
SUCCESS_JSON = base.SUCCESS_JSON
RETRY6_FAILURE_LOG = DOMAIN / "testing" / "postgres-commissioning-retry-6.log"
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-7"
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry-7")
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
    if not RETRY6_FAILURE_LOG.is_file():
        raise FileNotFoundError(
            "retry-6 downstream transcript is missing; the workflow defect must remain auditable"
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
    retry6_sha = base.sha256_file(RETRY6_FAILURE_LOG)

    base.upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [
            {
                "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-6",
                "TSPLoop": "tsp-loop-587",
                "Substrate": "POSTGRES_GENERATED_VIEWS",
                "AttemptedAt": "29632044920",
                "BuildCommand": (
                    "npm archive install; diagnostic start.sh build/db/test/show wrapper"
                ),
                "BuildSucceeded": False,
                "DatabaseInitialized": False,
                "ConformanceSucceeded": False,
                "Status": "FAILED",
                "FailureReason": (
                    "The pinned archive and CLI preflight succeeded, but the later shell "
                    "referenced NPM_CONFIG_PREFIX under set -u without job-level export. "
                    "The shell exited before start.sh build; no generated-substrate claim was attempted."
                ),
            }
        ],
    )

    rename_ids(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        "loop-587-retry-6-",
        "loop-587-retry-7-",
    )
    rename_ids(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        "loop-587-retry-6-",
        "loop-587-retry-7-",
    )
    base.upsert_rows(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        [
            {
                "TSPArtifactId": "loop-587-retry-6-workflow-failure-log",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-6",
                "ArtifactKind": "WORKFLOW_FAILURE_TRANSCRIPT",
                "RelativePath": str(RETRY6_FAILURE_LOG.relative_to(REPO)),
                "SHA256": retry6_sha,
                "IsPresent": True,
            }
        ],
    )
    base.upsert_rows(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-6-job-environment",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-6",
                "CheckKind": "WORKFLOW_ENVIRONMENT",
                "SubjectId": "NPM_CONFIG_PREFIX",
                "ExpectedValue": "EXPORTED",
                "ActualValue": "UNBOUND",
                "Status": "FAIL",
            }
        ],
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["AfterState"] = (
        "Attempts 1-5 preserved missing-CLI, npm git-preparation, and downstream-run failures. "
        "Retry 6 proved the archive and CLI preflight work but stopped before build on an "
        "unbound cross-step npm-prefix variable. Retry 7 exported the prefix at job scope, "
        "generated the vw_* projection, initialized erb_traveling_salesman, and passed "
        "Python/Postgres conformance."
    )
    loop["WitnessSummary"] = (
        "The pinned archive generated the backend and live Postgres agrees with Python. "
        "All six earlier blocked or failed attempts remain explicit execution rows."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_7"

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_7"

    contract.setdefault("ArtifactHashes", {})[
        "retry_6_workflow_failure_transcript"
    ] = f"sha256:{retry6_sha}"

    success["successful_attempt"] = 7
    success["attempt_history"] = [
        {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
        {"attempt": 2, "status": "FAILED", "reason": "npm step failed before transcript persistence"},
        {"attempt": 3, "status": "FAILED", "reason": "npm ENOTDIR in shared hosted-toolcache prefix"},
        {"attempt": 4, "status": "FAILED", "reason": "npm ENOTDIR persisted in isolated prefix during git-dependency preparation"},
        {"attempt": 5, "status": "FAILED", "reason": "archive installed; downstream step failed before transcript persistence"},
        {"attempt": 6, "status": "FAILED", "reason": "unbound cross-step npm-prefix variable stopped before build"},
        {"attempt": 7, "status": "SUCCEEDED", "reason": "job-level prefix and durable generated-substrate conformance completed"},
    ]
    success.setdefault("hashes", {})[
        "retry_6_workflow_failure_transcript"
    ] = retry6_sha

    base.write(RULEBOOK, rulebook)
    base.run([sys.executable, str(base.FORMATTER), str(RULEBOOK)], cwd=REPO)
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success)

    readme = README.read_text().replace(
        "Attempts 1–5 remain recorded as blocked or failed evidence. Retry 5 proved the pinned GitHub archive installs correctly but exposed a downstream build/conformance failure. Retry 6 retained that boundary, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. Artifact hashes and all available failure/success transcripts are retained.",
        "Attempts 1–6 remain recorded as blocked or failed evidence. Retry 6 proved the pinned archive and CLI preflight work but stopped before generation on an unbound cross-step environment variable. Retry 7 exported the npm prefix at job scope, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. Artifact hashes and all available failure/success transcripts are retained.",
    )
    README.write_text(readme)

    base.run([sys.executable, str(base.RECONCILER)])
    base.run([sys.executable, str(base.VALIDATOR)])

    print("TSP loop 587 commissioning retry 7: PASS")
    print(f"  cli_archive={CLI_ARCHIVE}")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")


if __name__ == "__main__":
    main()
