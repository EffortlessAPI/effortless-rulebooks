#!/usr/bin/env python3
"""Finalize TSP loop 587 after live generated-Postgres conformance.

This is intentionally standalone. Earlier retry wrappers remain historical code,
but closure no longer depends on executing their chain. The finalizer:

1. re-runs peer conformance against the live database;
2. verifies key generated view facts;
3. records every prior blocked/failed attempt as append-only rulebook data;
4. appends the successful attempt and artifact hashes;
5. closes only the Postgres substrate obligation;
6. reconciles and validates all canonical summary projections.

It never upgrades a failed attempt or erases its evidence.
"""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path
from typing import Any

import complete_loop_587 as base

REPO = base.REPO
DOMAIN = base.DOMAIN
RULEBOOK = base.RULEBOOK
CONTRACT = base.CONTRACT
README = base.README
PG_DIR = base.PG_DIR
RULESPEAK_DIR = base.RULESPEAK_DIR
LOG = base.LOG
SUCCESS_JSON = base.SUCCESS_JSON
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-final")
CLI_SOURCE = os.environ.get(
    "EFFORTLESS_CLI_SOURCE",
    "https://github.com/EffortlessAPI/cli/archive/1551cb82d5a8992ad09e4c82d08e28493f92f7b4.tar.gz",
)
CLI_COMMIT = os.environ.get(
    "EFFORTLESS_CLI_COMMIT", "1551cb82d5a8992ad09e4c82d08e28493f92f7b4"
)
CLI_VERSION = os.environ.get("EFFORTLESS_CLI_VERSION", "2026-06-09.06.13")
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-9"

FAILURE_ARTIFACTS = [
    (
        "postgres-commissioning-loop-587-retry-3",
        REPO / ".tsp-loop-587-cli-install.log",
        "CLI_INSTALL_FAILURE_TRANSCRIPT",
    ),
    (
        "postgres-commissioning-loop-587-retry-4",
        REPO / ".tsp-loop-587-cli-install-retry-4.log",
        "CLI_INSTALL_FAILURE_TRANSCRIPT",
    ),
    (
        "postgres-commissioning-loop-587-retry-6",
        DOMAIN / "testing" / "postgres-commissioning-retry-6.log",
        "WORKFLOW_FAILURE_TRANSCRIPT",
    ),
    (
        "postgres-commissioning-loop-587-retry-7",
        DOMAIN / "testing" / "postgres-commissioning-retry-7.log",
        "GENERATED_SUBSTRATE_AND_STALE_ORACLE_TRANSCRIPT",
    ),
]


def load(path: Path) -> dict[str, Any]:
    if not path.is_file():
        raise FileNotFoundError(f"missing required file: {path}")
    return json.loads(path.read_text())


def write(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n")


def readme_project(text: str) -> str:
    lines = text.splitlines()
    for index, line in enumerate(lines):
        if line.startswith("**Version:**"):
            lines[index] = "**Version:** 0.3.1"
            break
    else:
        raise AssertionError("README version marker is missing")
    text = "\n".join(lines) + "\n"

    old_frontier = (
        "The nearest open obligations are live Postgres commissioning, route reconstruction "
        "from inferred edge rows, a deliberately non-tight lower-bound fixture, degree-two "
        "forcing after pruning, neighborhood boundary-state contraction, subtour certificates, "
        "and explicit residual branching."
    )
    new_frontier = (
        "The nearest open obligations are stronger component/crossing lower bounds, general "
        "neighborhood-state soundness beyond symmetric three-stop clusters, and explicit "
        "branching only after deterministic closure leaves positive residual ambiguity."
    )
    if old_frontier in text:
        text = text.replace(old_frontier, new_frontier, 1)

    blocked = (
        "The rulebook records every loop first as PLANNED with a before-state and closure "
        "criterion, then preserves the after-state in the same row. Gridville is reconstructed "
        "from inferred edges with zero branch decisions; twin triangles preserves a sound "
        "non-tight lower bound and yields the first finite neighborhood boundary-state "
        "contraction certificate. Loop 587 remains honestly BLOCKED when live generated "
        "Postgres cannot be commissioned in the execution environment."
    )
    commissioned = (
        "The rulebook records every loop first as PLANNED with a before-state and closure "
        "criterion, then preserves the after-state in the same row. Gridville is reconstructed "
        "from inferred edges with zero branch decisions; twin triangles preserves a sound "
        "non-tight lower bound and yields the first finite neighborhood boundary-state "
        "contraction certificate.\n\n"
        "**Postgres commissioning: CLOSED.** Attempts 1–8 remain explicit blocked or failed "
        "execution rows. Retry 9 installed the pinned EffortlessAPI/cli archive with npm, "
        "generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed "
        "the complete Python/Postgres conformance surface. Generated artifact hashes and all "
        "available failure/success transcripts are retained."
    )
    if blocked in text:
        text = text.replace(blocked, commissioned, 1)
    elif "**Postgres commissioning: CLOSED.**" not in text:
        text += "\n## Postgres commissioning\n\n" + commissioned.split("\n\n", 1)[1] + "\n"
    return text


def main() -> None:
    if not PG_DIR.joinpath("init-db.sh").is_file():
        raise FileNotFoundError("generated Postgres initializer is missing after build")
    if not LOG.is_file():
        raise FileNotFoundError(f"successful commissioning transcript is missing: {LOG}")

    # This runs before the canonical status changes, so the database and the
    # rulebook share the same open-frontier input state.
    base.run([sys.executable, str(base.TAKE_TEST)])
    view_count = int(
        base.psql_scalar(
            "SELECT count(*) FROM pg_views WHERE schemaname='public' AND viewname LIKE 'vw_%'"
        )
    )
    if view_count < 40:
        raise AssertionError(f"expected at least 40 generated views, got {view_count}")

    expected_live = {
        "gridville_complete": "true",
        "reference_tour": "true,true,14",
        "degree_two_lower_bound": "true,14",
    }
    actual_live = {
        "gridville_complete": base.psql_scalar(
            "SELECT is_complete_undirected_graph::text FROM vw_tsp_instances "
            "WHERE tsp_instance_id='tsp-gridville-5'"
        ),
        "reference_tour": base.psql_scalar(
            "SELECT is_hamiltonian_cycle_witness::text || ',' || "
            "is_optimality_proved::text || ',' || total_travel_cost::text "
            "FROM vw_candidate_tours WHERE candidate_tour_id='tour-reference-ring'"
        ),
        "degree_two_lower_bound": base.psql_scalar(
            "SELECT is_certified::text || ',' || lower_bound_cost::text "
            "FROM vw_instance_lower_bounds "
            "WHERE instance_lower_bound_id='degree-two-lower-bound-gridville-5'"
        ),
    }
    if actual_live != expected_live:
        raise AssertionError(
            f"live acceptance mismatch: expected={expected_live!r} actual={actual_live!r}"
        )

    build_input_rulebook_sha = base.sha256_file(RULEBOOK)
    postgres_tree_sha = base.sha256_tree(PG_DIR)
    rulespeak_tree_sha = base.sha256_tree(RULESPEAK_DIR)
    success_log_sha = base.sha256_file(LOG)

    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)
    original = base.table_index(rulebook, "TSPExecutionRuns").get(
        "postgres-commissioning-loop-587"
    )
    if not original or original["Status"] != "BLOCKED":
        raise AssertionError("initial blocked commissioning attempt is missing")

    attempts = [
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-2",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29631462318",
            "BuildCommand": "npm install -g GitHub CLI URL",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": "npm installation exited nonzero before transcript persistence.",
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-3",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29631550495",
            "BuildCommand": "npm install -g git+https pinned CLI",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": "npm ENOTDIR in the hosted runner shared global module prefix.",
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-4",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29631700762",
            "BuildCommand": "isolated npm prefix with git dependency",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": "npm git-dependency preparation reproduced ENOTDIR in an isolated prefix.",
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-5",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29631879613",
            "BuildCommand": "npm install pinned archive; downstream build",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": "Archive installation succeeded; downstream step failed before transcript persistence.",
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-6",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29632044920",
            "BuildCommand": "archive install; diagnostic build wrapper",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": "Unbound cross-step NPM_CONFIG_PREFIX stopped the shell before build.",
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-7",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29632306954",
            "BuildCommand": "archive install; build; db; test; show",
            "BuildSucceeded": True,
            "DatabaseInitialized": True,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": (
                "All substantive comparisons passed; a stale hard-coded frontier oracle "
                "expected 8/0/4 instead of canonical and Postgres 16/0/13."
            ),
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-8",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29632561258",
            "BuildCommand": "archive install; canonical-oracle build; db; test; show",
            "BuildSucceeded": True,
            "DatabaseInitialized": True,
            "ConformanceSucceeded": True,
            "Status": "FAILED",
            "FailureReason": (
                "The generated substrate and complete peer conformance passed; the chained "
                "post-conformance ledger finalizer failed before durable closure."
            ),
        },
        {
            "TSPExecutionRunId": SUCCESS_RUN_ID,
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": RUN_ID,
            "BuildCommand": (
                "npm install pinned CLI archive; start.sh build; db; test; show; standalone finalizer"
            ),
            "BuildSucceeded": True,
            "DatabaseInitialized": True,
            "ConformanceSucceeded": True,
            "Status": "SUCCEEDED",
            "FailureReason": None,
        },
    ]
    base.upsert_rows(
        rulebook["TSPExecutionRuns"], "TSPExecutionRunId", attempts
    )

    artifact_rows: list[dict[str, Any]] = []
    for execution_run, path, kind in FAILURE_ARTIFACTS:
        if path.is_file():
            artifact_rows.append(
                {
                    "TSPArtifactId": f"{execution_run}-{kind.lower().replace('_', '-')}",
                    "ExecutionRun": execution_run,
                    "ArtifactKind": kind,
                    "RelativePath": str(path.relative_to(REPO)),
                    "SHA256": base.sha256_file(path),
                    "IsPresent": True,
                }
            )
    artifact_rows.extend(
        [
            {
                "TSPArtifactId": "loop-587-retry-9-build-input-rulebook",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "BUILD_INPUT_RULEBOOK",
                "RelativePath": str(RULEBOOK.relative_to(REPO)),
                "SHA256": build_input_rulebook_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-9-generated-postgres-tree",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "GENERATED_POSTGRES_TREE",
                "RelativePath": str(PG_DIR.relative_to(REPO)),
                "SHA256": postgres_tree_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-9-generated-rulespeak-tree",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "GENERATED_RULESPEAK_TREE",
                "RelativePath": str(RULESPEAK_DIR.relative_to(REPO)),
                "SHA256": rulespeak_tree_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-9-commissioning-transcript",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "COMMISSIONING_TRANSCRIPT",
                "RelativePath": str(LOG.relative_to(REPO)),
                "SHA256": success_log_sha,
                "IsPresent": True,
            },
        ]
    )
    base.upsert_rows(rulebook["TSPArtifacts"], "TSPArtifactId", artifact_rows)

    checks = [
        {
            "TSPConformanceCheckId": "loop-587-retry-7-stale-frontier-oracle",
            "ExecutionRun": "postgres-commissioning-loop-587-retry-7",
            "CheckKind": "TEST_ORACLE_ALIGNMENT",
            "SubjectId": "TSPFrontierObligations",
            "ExpectedValue": "8,0,4 (stale)",
            "ActualValue": "16,0,13 (canonical and Postgres)",
            "Status": "FAIL",
        },
        {
            "TSPConformanceCheckId": "loop-587-retry-8-ledger-finalization",
            "ExecutionRun": "postgres-commissioning-loop-587-retry-8",
            "CheckKind": "CERTIFICATE_PERSISTENCE",
            "SubjectId": "loop-587 closure transaction",
            "ExpectedValue": "PASS",
            "ActualValue": "FAIL",
            "Status": "FAIL",
        },
    ]
    for key, value in actual_live.items():
        checks.append(
            {
                "TSPConformanceCheckId": f"loop-587-retry-9-{key.replace('_', '-')}",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "LIVE_VIEW_VALUE",
                "SubjectId": key,
                "ExpectedValue": expected_live[key],
                "ActualValue": value,
                "Status": "PASS",
            }
        )
    checks.extend(
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-9-cli-installation",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "CLI_INSTALLATION",
                "SubjectId": f"EffortlessAPI/cli@{CLI_COMMIT}",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-9-generated-build",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "GENERATED_BUILD",
                "SubjectId": "traveling-salesman",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-9-database-initialization",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "DATABASE_INITIALIZATION",
                "SubjectId": base.DB,
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-9-python-postgres-conformance",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "PYTHON_POSTGRES_CONFORMANCE",
                "SubjectId": "testing/take-test.py",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
        ]
    )
    base.upsert_rows(
        rulebook["TSPConformanceChecks"], "TSPConformanceCheckId", checks
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["Status"] = "CLOSED"
    loop["AfterState"] = (
        "Attempts 1-8 preserve missing-CLI, npm, workflow, stale-oracle, and "
        "ledger-finalization failures. Retry 9 installed the pinned CLI archive, "
        f"generated {view_count} vw_* views, initialized {base.DB}, passed the "
        "rulebook-derived Python/Postgres conformance surface, and durably sealed the certificate."
    )
    loop["WitnessSummary"] = (
        "Generated Postgres and Python agree on graph, tour, lower-bound, optimality, "
        "invariant, search, and frontier semantics; prior attempts remain first-class data."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_9"

    frontier = base.table_index(rulebook, "TSPFrontierObligations")[
        "frontier-postgres-commissioning"
    ]
    frontier["Status"] = "CLOSED"
    frontier["ClosedByLoop"] = "tsp-loop-587"
    frontier["TrustDisposition"] = (
        "CLOSED_BY_PINNED_CLI_GENERATION_LIVE_DATABASE_AND_PEER_CONFORMANCE"
    )

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["Status"] = "CLOSED"
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_9"

    contract["Version"] = "0.3.1"
    contract["Scope"] = (
        "The represented scope includes Gridville route reconstruction from inferred "
        "edges, finite optimality by bound equality, negative non-tight and subtour "
        "fixtures, deterministic forcing/forbidding, neighborhood contraction, and "
        "a commissioned generated-Postgres peer substrate. No general solver or "
        "complexity-class claim is made."
    )
    contract["TrustBoundary"] = (
        "InputGraphKernel remains trusted input data. Structural certificates are "
        "internalized in the rulebook. Generated Postgres is commissioned as a peer "
        f"substrate using EffortlessAPI/cli {CLI_VERSION} ({CLI_COMMIT}) and checked against Python."
    )
    contract.setdefault("Claims", {})["PostgresConformanceCommissioned"] = True
    contract.setdefault("ArtifactHashes", {}).update(
        {
            "build_input_rulebook": f"sha256:{build_input_rulebook_sha}",
            "postgres_projection_tree": f"sha256:{postgres_tree_sha}",
            "rulespeak_tree": f"sha256:{rulespeak_tree_sha}",
            "commissioning_transcript": f"sha256:{success_log_sha}",
            "effortless_cli_commit": CLI_COMMIT,
            "effortless_cli_archive": CLI_SOURCE,
        }
    )
    cert_id = "gridville-postgres-substrate-conformance"
    certificates = contract.setdefault("CurrentCertificates", [])
    certificates = [row for row in certificates if row.get("CertificateId") != cert_id]
    certificates.append(
        {
            "CertificateId": cert_id,
            "Kind": "substrate-conformance-certificate",
            "Conclusion": (
                f"Generated Postgres ({view_count} vw_* views) and Python agree on the "
                f"declared TSP acceptance surface using EffortlessAPI/cli {CLI_VERSION}."
            ),
        }
    )
    contract["CurrentCertificates"] = certificates
    contract["RemainingFrontier"] = [
        item
        for item in contract.get("RemainingFrontier", [])
        if "Postgres" not in item and "postgres" not in item
    ]
    contract["ExecutionSubstrates"] = [
        f"postgres:generated vw_* views (commissioned via EffortlessAPI/cli {CLI_VERSION})",
        "python:scripts/reference_model.py",
    ]

    success_payload = {
        "status": "SUCCEEDED",
        "loop": 587,
        "successful_attempt": 9,
        "github_run_id": RUN_ID,
        "cli_source": CLI_SOURCE,
        "cli_commit": CLI_COMMIT,
        "cli_version": CLI_VERSION,
        "database": base.DB,
        "generated_view_count": view_count,
        "live_checks": actual_live,
        "attempt_history": [
            {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
            {"attempt": 2, "status": "FAILED", "reason": "npm failure before transcript"},
            {"attempt": 3, "status": "FAILED", "reason": "shared-prefix npm ENOTDIR"},
            {"attempt": 4, "status": "FAILED", "reason": "isolated-prefix git-preparation ENOTDIR"},
            {"attempt": 5, "status": "FAILED", "reason": "unpersisted downstream failure"},
            {"attempt": 6, "status": "FAILED", "reason": "unbound cross-step npm prefix"},
            {"attempt": 7, "status": "FAILED", "reason": "stale frontier-count oracle"},
            {"attempt": 8, "status": "FAILED", "reason": "post-conformance ledger finalizer"},
            {"attempt": 9, "status": "SUCCEEDED", "reason": "standalone finalizer sealed live conformance"},
        ],
        "hashes": {
            "build_input_rulebook": build_input_rulebook_sha,
            "postgres_projection_tree": postgres_tree_sha,
            "rulespeak_tree": rulespeak_tree_sha,
            "commissioning_transcript": success_log_sha,
        },
    }

    write(RULEBOOK, rulebook)
    base.run([sys.executable, str(base.FORMATTER), str(RULEBOOK)], cwd=REPO)
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success_payload)
    README.write_text(readme_project(README.read_text()))

    base.run([sys.executable, str(base.RECONCILER)])
    base.run([sys.executable, str(base.VALIDATOR)])

    print("TSP loop 587 standalone finalization: PASS")
    print(f"  views={view_count} database={base.DB}")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")
    print(f"  postgres_tree_sha256={postgres_tree_sha}")


if __name__ == "__main__":
    main()
