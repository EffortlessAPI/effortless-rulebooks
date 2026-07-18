#!/usr/bin/env python3
"""Finalize loop 587 with Decimal-normalized live numeric comparisons.

Retry 9 proved the complete generated substrate and peer conformance but its
ledger sealer compared PostgreSQL NUMERIC text (`14.0000000000000000`) to the
presentation string `14`. This wrapper normalizes numeric text, runs the
standalone finalizer under retry 10, and records retry 9 as a failed certificate
persistence attempt rather than a semantic or substrate failure.
"""
from __future__ import annotations

import json
import os
import sys
from decimal import Decimal
from pathlib import Path
from typing import Any

import complete_loop_587_final as final

REPO = final.REPO
DOMAIN = final.DOMAIN
RULEBOOK = final.RULEBOOK
CONTRACT = final.CONTRACT
README = final.README
SUCCESS_JSON = final.SUCCESS_JSON
RETRY9_FINALIZATION_LOG = (
    DOMAIN / "testing" / "postgres-commissioning-finalization-retry-9.log"
)
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-10"
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry-10")

_original_scalar = final.base.psql_scalar
_original_readme_project = final.readme_project
_extra_blocked_projection = (
    "\n**Postgres commissioning: BLOCKED.** The generated backend and peer comparisons "
    "have executed successfully in retry 8, but the canonical substrate obligation "
    "remains blocked until the success certificate, execution history, artifact hashes, "
    "and summary projections are durably sealed and validated.\n"
)


def normalize_decimal_text(value: str) -> str:
    number = Decimal(value)
    if number == number.to_integral_value():
        return str(number.quantize(Decimal("1")))
    return format(number.normalize(), "f")


def normalized_scalar(sql: str) -> str:
    value = _original_scalar(sql)
    if "vw_candidate_tours" in sql and value.count(",") == 2:
        valid, optimal, cost = value.split(",")
        return f"{valid},{optimal},{normalize_decimal_text(cost)}"
    if "vw_instance_lower_bounds" in sql and value.count(",") == 1:
        certified, cost = value.split(",")
        return f"{certified},{normalize_decimal_text(cost)}"
    return value


def readme_project(text: str) -> str:
    return _original_readme_project(text.replace(_extra_blocked_projection, "\n"))


def load(path: Path) -> dict[str, Any]:
    return json.loads(path.read_text())


def write(path: Path, value: Any) -> None:
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n")


def rename_ids(table: dict[str, Any], id_field: str, old: str, new: str) -> None:
    for row in table["data"]:
        if old in row[id_field]:
            row[id_field] = row[id_field].replace(old, new)


def main() -> None:
    if not RETRY9_FINALIZATION_LOG.is_file():
        raise FileNotFoundError(
            "retry-9 finalization log is missing; numeric-format failure evidence must remain durable"
        )

    final.base.psql_scalar = normalized_scalar
    final.readme_project = readme_project
    final.SUCCESS_RUN_ID = SUCCESS_RUN_ID
    final.RUN_ID = RUN_ID
    final.main()

    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)
    success = load(SUCCESS_JSON)
    retry9_sha = final.base.sha256_file(RETRY9_FINALIZATION_LOG)

    final.base.upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [
            {
                "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-9",
                "TSPLoop": "tsp-loop-587",
                "Substrate": "POSTGRES_GENERATED_VIEWS",
                "AttemptedAt": "29632874612",
                "BuildCommand": (
                    "pinned archive; build; db; test; show; standalone ledger finalizer"
                ),
                "BuildSucceeded": True,
                "DatabaseInitialized": True,
                "ConformanceSucceeded": True,
                "Status": "FAILED",
                "FailureReason": (
                    "All generated-substrate and peer checks passed. The ledger finalizer "
                    "rejected PostgreSQL NUMERIC text 14.0000000000000000 against the "
                    "presentation string 14 before canonical closure was persisted."
                ),
            }
        ],
    )

    rename_ids(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        "loop-587-retry-9-",
        "loop-587-retry-10-",
    )
    rename_ids(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        "loop-587-retry-9-",
        "loop-587-retry-10-",
    )
    final.base.upsert_rows(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        [
            {
                "TSPArtifactId": "loop-587-retry-9-finalization-log",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-9",
                "ArtifactKind": "LEDGER_FINALIZATION_FAILURE_TRANSCRIPT",
                "RelativePath": str(RETRY9_FINALIZATION_LOG.relative_to(REPO)),
                "SHA256": retry9_sha,
                "IsPresent": True,
            }
        ],
    )
    final.base.upsert_rows(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-9-numeric-text-normalization",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-9",
                "CheckKind": "CERTIFICATE_PERSISTENCE",
                "SubjectId": "degree_two_lower_bound numeric text",
                "ExpectedValue": "14",
                "ActualValue": "14.0000000000000000",
                "Status": "FAIL",
            }
        ],
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["AfterState"] = (
        "Attempts 1-9 preserve missing-CLI, npm, workflow, stale-oracle, chained-finalizer, "
        "and numeric-presentation failures. Retry 10 installed the pinned CLI archive, "
        "generated the complete vw_* projection, initialized erb_traveling_salesman, "
        "passed rulebook-derived Python/Postgres conformance, normalized NUMERIC text "
        "through Decimal, and durably sealed the substrate certificate."
    )
    loop["WitnessSummary"] = (
        "Generated Postgres and Python agree on all declared semantics; Decimal-normalized "
        "live checks and append-only attempt history close the substrate obligation."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_10"

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_10"
    contract.setdefault("ArtifactHashes", {})[
        "retry_9_finalization_failure_transcript"
    ] = f"sha256:{retry9_sha}"

    success["successful_attempt"] = 10
    success["attempt_history"] = [
        {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
        {"attempt": 2, "status": "FAILED", "reason": "npm failure before transcript"},
        {"attempt": 3, "status": "FAILED", "reason": "shared-prefix npm ENOTDIR"},
        {"attempt": 4, "status": "FAILED", "reason": "isolated-prefix git-preparation ENOTDIR"},
        {"attempt": 5, "status": "FAILED", "reason": "unpersisted downstream failure"},
        {"attempt": 6, "status": "FAILED", "reason": "unbound cross-step npm prefix"},
        {"attempt": 7, "status": "FAILED", "reason": "stale frontier-count oracle"},
        {"attempt": 8, "status": "FAILED", "reason": "chained post-conformance finalizer"},
        {"attempt": 9, "status": "FAILED", "reason": "NUMERIC text presentation mismatch"},
        {"attempt": 10, "status": "SUCCEEDED", "reason": "Decimal-normalized standalone finalizer"},
    ]
    success.setdefault("hashes", {})[
        "retry_9_finalization_failure_transcript"
    ] = retry9_sha

    final.write(RULEBOOK, rulebook)
    final.base.run(
        [sys.executable, str(final.base.FORMATTER), str(RULEBOOK)], cwd=REPO
    )
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success)

    readme = README.read_text().replace(
        "**Postgres commissioning: CLOSED.** Attempts 1–8 remain explicit blocked or failed execution rows. Retry 9 installed the pinned EffortlessAPI/cli archive with npm, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed the complete Python/Postgres conformance surface. Generated artifact hashes and all available failure/success transcripts are retained.",
        "**Postgres commissioning: CLOSED.** Attempts 1–9 remain explicit blocked or failed execution rows. Retry 10 installed the pinned EffortlessAPI/cli archive with npm, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, passed the complete Python/Postgres conformance surface, normalized PostgreSQL NUMERIC text through `Decimal`, and durably sealed the canonical certificate. Generated artifact hashes and all available failure/success transcripts are retained.",
    )
    README.write_text(readme)

    final.base.run([sys.executable, str(final.base.RECONCILER)])
    final.base.run([sys.executable, str(final.base.VALIDATOR)])

    print("TSP loop 587 finalization retry 10: PASS")
    print(f"  cli={final.CLI_VERSION}@{final.CLI_COMMIT}")


if __name__ == "__main__":
    main()
