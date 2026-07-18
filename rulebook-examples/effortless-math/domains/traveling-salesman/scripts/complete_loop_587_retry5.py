#!/usr/bin/env python3
"""Close TSP loop 587 after retry 5 commissions generated Postgres.

This wrapper reuses the fully validated retry-4 completion machinery, but records
retry 4 as a failed isolated-prefix git-install attempt and retry 5 as the first
successful archive-based npm installation. Nothing from attempts 1-4 is erased.
"""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path
from typing import Any

import complete_loop_587 as base
import complete_loop_587_retry4 as prior

REPO = base.REPO
DOMAIN = base.DOMAIN
RULEBOOK = base.RULEBOOK
CONTRACT = base.CONTRACT
README = base.README
SUCCESS_JSON = base.SUCCESS_JSON
RETRY4_FAILURE_LOG = REPO / ".tsp-loop-587-cli-install-retry-4.log"
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-5"
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry-5")
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
        value = row[id_field]
        if old in value:
            row[id_field] = value.replace(old, new)


def main() -> None:
    if not RETRY4_FAILURE_LOG.is_file():
        raise FileNotFoundError(
            "retry-4 failure transcript is missing; append-only attempt history cannot be completed"
        )

    # Parameterize the prior completion engine so its successful execution row
    # is created under retry 5. It performs all live database checks first.
    prior.SUCCESS_RUN_ID = SUCCESS_RUN_ID
    prior.RUN_ID = RUN_ID
    prior.CLI_SOURCE = CLI_ARCHIVE
    prior.CLI_COMMIT = CLI_COMMIT
    prior.CLI_VERSION = CLI_VERSION
    prior.main()

    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)
    success = load(SUCCESS_JSON)
    retry4_sha = base.sha256_file(RETRY4_FAILURE_LOG)

    base.upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [
            {
                "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-4",
                "TSPLoop": "tsp-loop-587",
                "Substrate": "POSTGRES_GENERATED_VIEWS",
                "AttemptedAt": "29631700762",
                "BuildCommand": (
                    "NPM_CONFIG_PREFIX=$RUNNER_TEMP/npm-global npm install -g "
                    "git+https://github.com/EffortlessAPI/cli.git#1551cb82..."
                ),
                "BuildSucceeded": False,
                "DatabaseInitialized": False,
                "ConformanceSucceeded": False,
                "Status": "FAILED",
                "FailureReason": (
                    "npm git-dependency preparation still raised ENOTDIR inside the clean "
                    "per-run prefix while renaming lib/node_modules/ssotme; no effortless "
                    "binary or generated substrate was produced."
                ),
            }
        ],
    )

    # The reused completion engine emitted success artifact/check IDs containing
    # retry-4; rename those IDs only. Their ExecutionRun already points at retry 5.
    rename_ids(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        "loop-587-retry-4-",
        "loop-587-retry-5-",
    )
    rename_ids(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        "loop-587-retry-4-",
        "loop-587-retry-5-",
    )

    base.upsert_rows(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        [
            {
                "TSPArtifactId": "loop-587-retry-4-cli-install-failure-log",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-4",
                "ArtifactKind": "CLI_INSTALL_FAILURE_TRANSCRIPT",
                "RelativePath": str(RETRY4_FAILURE_LOG.relative_to(REPO)),
                "SHA256": retry4_sha,
                "IsPresent": True,
            }
        ],
    )
    base.upsert_rows(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-4-cli-installation",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-4",
                "CheckKind": "CLI_INSTALLATION",
                "SubjectId": "EffortlessAPI/cli git dependency in isolated npm prefix",
                "ExpectedValue": "PASS",
                "ActualValue": "ENOTDIR",
                "Status": "FAIL",
            }
        ],
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["AfterState"] = (
        "Attempt 1 was BLOCKED by a missing CLI. Retry 2 failed before transcript "
        "persistence. Retry 3 exposed npm ENOTDIR in the shared hosted-toolcache prefix. "
        "Retry 4 reproduced ENOTDIR in an isolated prefix, proving the failure belonged "
        "to npm git-dependency preparation. Retry 5 installed the pinned GitHub archive, "
        "generated the vw_* projection, initialized erb_traveling_salesman, and passed "
        "Python/Postgres conformance."
    )
    loop["WitnessSummary"] = (
        "The pinned CLI archive generated the backend and live Postgres agrees with Python. "
        "All four earlier blocked/failed attempts and both install transcripts remain data."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_5"

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_5"

    contract["TrustBoundary"] = (
        "InputGraphKernel remains trusted input data. Structural certificates are "
        "internalized in the rulebook. Generated Postgres is commissioned as a peer "
        f"substrate using the pinned EffortlessAPI/cli archive {CLI_VERSION} ({CLI_COMMIT}) "
        "and checked against Python."
    )
    contract.setdefault("ArtifactHashes", {})[
        "retry_4_cli_failure_transcript"
    ] = f"sha256:{retry4_sha}"
    contract.setdefault("ArtifactHashes", {})[
        "effortless_cli_archive"
    ] = CLI_ARCHIVE

    success["successful_attempt"] = 5
    success["cli_source"] = CLI_ARCHIVE
    success["attempt_history"] = [
        {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
        {"attempt": 2, "status": "FAILED", "reason": "npm step failed before transcript persistence"},
        {"attempt": 3, "status": "FAILED", "reason": "npm ENOTDIR in shared hosted-toolcache prefix"},
        {"attempt": 4, "status": "FAILED", "reason": "npm ENOTDIR persisted in isolated prefix during git-dependency preparation"},
        {"attempt": 5, "status": "SUCCEEDED", "reason": "pinned GitHub archive bypassed git-dependency preparation"},
    ]
    success.setdefault("hashes", {})[
        "retry_4_cli_failure_transcript"
    ] = retry4_sha

    base.write(RULEBOOK, rulebook)
    base.run([sys.executable, str(base.FORMATTER), str(RULEBOOK)], cwd=REPO)
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success)

    readme = README.read_text()
    readme = readme.replace(
        "Attempts 1–3 remain recorded as blocked or failed evidence. Retry 4 installed `git+https://github.com/EffortlessAPI/cli.git#1551cb82d5a8992ad09e4c82d08e28493f92f7b4` into an isolated npm prefix, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. Artifact hashes and both failure and success transcripts are retained.",
        "Attempts 1–4 remain recorded as blocked or failed evidence. Retry 5 installed the pinned GitHub archive `https://github.com/EffortlessAPI/cli/archive/1551cb82d5a8992ad09e4c82d08e28493f92f7b4.tar.gz` with npm, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. The archive path bypasses npm's failing git-dependency preparation while retaining the exact CLI commit; artifact hashes and failure/success transcripts are retained.",
    )
    README.write_text(readme)

    base.run([sys.executable, str(base.RECONCILER)])
    base.run([sys.executable, str(base.VALIDATOR)])

    print("TSP loop 587 commissioning retry 5: PASS")
    print(f"  cli_archive={CLI_ARCHIVE}")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")


if __name__ == "__main__":
    main()
