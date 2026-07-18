#!/usr/bin/env python3
"""Close TSP loop 587 after retry 4 commissions generated Postgres.

Attempts remain append-only evidence:

1. Initial attempt: BLOCKED because `effortless` was absent.
2. Retry 2: npm step failed before transcript persistence.
3. Retry 3: explicit git install reached npm but failed with ENOTDIR in the
   hosted runner's shared global module prefix; its log is committed.
4. Retry 4: clean per-run npm prefix, generated build, live database, and
   Python/Postgres conformance. Only this successful attempt closes the loop.
"""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path

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
FAILURE_LOG = REPO / ".tsp-loop-587-cli-install.log"
RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry-4")
CLI_SOURCE = os.environ.get(
    "EFFORTLESS_CLI_SOURCE",
    "git+https://github.com/EffortlessAPI/cli.git#1551cb82d5a8992ad09e4c82d08e28493f92f7b4",
)
CLI_COMMIT = os.environ.get(
    "EFFORTLESS_CLI_COMMIT", "1551cb82d5a8992ad09e4c82d08e28493f92f7b4"
)
CLI_VERSION = os.environ.get("EFFORTLESS_CLI_VERSION", "2026-06-09.06.13")
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-4"


def main() -> None:
    if not PG_DIR.joinpath("init-db.sh").is_file():
        raise FileNotFoundError("generated Postgres initializer is missing after build")
    if not LOG.is_file():
        raise FileNotFoundError(f"commissioning transcript is missing: {LOG}")
    if not FAILURE_LOG.is_file():
        raise FileNotFoundError(
            "retry-3 npm failure log is missing; historical failure evidence must be preserved"
        )

    # Re-run peer conformance and inspect key live facts before changing status.
    base.run([sys.executable, str(base.TAKE_TEST)])
    view_count = int(
        base.psql_scalar(
            "SELECT count(*) FROM pg_views "
            "WHERE schemaname='public' AND viewname LIKE 'vw_%'"
        )
    )
    if view_count < 40:
        raise AssertionError(f"expected at least 40 generated vw_* views, got {view_count}")

    actual = {
        "gridville_complete": base.psql_scalar(
            "SELECT is_complete_undirected_graph::text "
            "FROM vw_tsp_instances WHERE tsp_instance_id='tsp-gridville-5'"
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
    expected = {
        "gridville_complete": "true",
        "reference_tour": "true,true,14",
        "degree_two_lower_bound": "true,14",
    }
    if actual != expected:
        raise AssertionError(f"live Postgres acceptance mismatch: {actual!r}")

    build_input_rulebook_sha = base.sha256_file(RULEBOOK)
    postgres_tree_sha = base.sha256_tree(PG_DIR)
    rulespeak_tree_sha = base.sha256_tree(RULESPEAK_DIR)
    transcript_sha = base.sha256_file(LOG)
    retry3_failure_sha = base.sha256_file(FAILURE_LOG)

    rulebook = base.load(RULEBOOK)
    contract = base.load(CONTRACT)
    execution_runs = base.table_index(rulebook, "TSPExecutionRuns")
    initial = execution_runs.get("postgres-commissioning-loop-587")
    if not initial or initial["Status"] != "BLOCKED":
        raise AssertionError("initial blocked loop-587 attempt is missing")

    failed_runs = [
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-2",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29631462318",
            "BuildCommand": "npm install -g https://github.com/EffortlessAPI/cli.git#1551cb82...",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": (
                "The npm installation step exited nonzero before an install transcript "
                "checkpoint existed; GitHub Actions run 29631462318 emitted no effortless binary."
            ),
        },
        {
            "TSPExecutionRunId": "postgres-commissioning-loop-587-retry-3",
            "TSPLoop": "tsp-loop-587",
            "Substrate": "POSTGRES_GENERATED_VIEWS",
            "AttemptedAt": "29631550495",
            "BuildCommand": "npm install -g git+https://github.com/EffortlessAPI/cli.git#1551cb82...",
            "BuildSucceeded": False,
            "DatabaseInitialized": False,
            "ConformanceSucceeded": False,
            "Status": "FAILED",
            "FailureReason": (
                "npm ENOTDIR while renaming the pre-existing hosted-toolcache path "
                "/opt/hostedtoolcache/node/22.23.1/x64/lib/node_modules/ssotme; "
                "the effortless binary was not installed."
            ),
        },
    ]
    success_run = {
        "TSPExecutionRunId": SUCCESS_RUN_ID,
        "TSPLoop": "tsp-loop-587",
        "Substrate": "POSTGRES_GENERATED_VIEWS",
        "AttemptedAt": RUN_ID,
        "BuildCommand": (
            "NPM_CONFIG_PREFIX=$RUNNER_TEMP/npm-global npm install -g <pinned CLI>; "
            "bash start.sh build; bash start.sh db; bash start.sh test; bash start.sh show"
        ),
        "BuildSucceeded": True,
        "DatabaseInitialized": True,
        "ConformanceSucceeded": True,
        "Status": "SUCCEEDED",
        "FailureReason": None,
    }
    base.upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [*failed_runs, success_run],
    )

    base.upsert_rows(
        rulebook["TSPArtifacts"],
        "TSPArtifactId",
        [
            {
                "TSPArtifactId": "loop-587-retry-3-cli-install-failure-log",
                "ExecutionRun": "postgres-commissioning-loop-587-retry-3",
                "ArtifactKind": "CLI_INSTALL_FAILURE_TRANSCRIPT",
                "RelativePath": str(FAILURE_LOG.relative_to(REPO)),
                "SHA256": retry3_failure_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-4-build-input-rulebook",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "BUILD_INPUT_RULEBOOK",
                "RelativePath": str(RULEBOOK.relative_to(REPO)),
                "SHA256": build_input_rulebook_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-4-generated-postgres-tree",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "GENERATED_POSTGRES_TREE",
                "RelativePath": str(PG_DIR.relative_to(REPO)),
                "SHA256": postgres_tree_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-4-rulespeak-tree",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "GENERATED_RULESPEAK_TREE",
                "RelativePath": str(RULESPEAK_DIR.relative_to(REPO)),
                "SHA256": rulespeak_tree_sha,
                "IsPresent": True,
            },
            {
                "TSPArtifactId": "loop-587-retry-4-transcript",
                "ExecutionRun": SUCCESS_RUN_ID,
                "ArtifactKind": "COMMISSIONING_TRANSCRIPT",
                "RelativePath": str(LOG.relative_to(REPO)),
                "SHA256": transcript_sha,
                "IsPresent": True,
            },
        ],
    )

    check_rows = [
        {
            "TSPConformanceCheckId": "loop-587-retry-2-cli-installation",
            "ExecutionRun": "postgres-commissioning-loop-587-retry-2",
            "CheckKind": "CLI_INSTALLATION",
            "SubjectId": "EffortlessAPI/cli",
            "ExpectedValue": "PASS",
            "ActualValue": "FAIL",
            "Status": "FAIL",
        },
        {
            "TSPConformanceCheckId": "loop-587-retry-3-cli-installation",
            "ExecutionRun": "postgres-commissioning-loop-587-retry-3",
            "CheckKind": "CLI_INSTALLATION",
            "SubjectId": "EffortlessAPI/cli",
            "ExpectedValue": "PASS",
            "ActualValue": "ENOTDIR",
            "Status": "FAIL",
        },
    ]
    for key, observed in actual.items():
        check_rows.append(
            {
                "TSPConformanceCheckId": f"loop-587-retry-4-{key.replace('_', '-')}",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "LIVE_VIEW_VALUE",
                "SubjectId": key,
                "ExpectedValue": expected[key],
                "ActualValue": observed,
                "Status": "PASS",
            }
        )
    check_rows.extend(
        [
            {
                "TSPConformanceCheckId": "loop-587-retry-4-cli-installation",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "CLI_INSTALLATION",
                "SubjectId": f"EffortlessAPI/cli@{CLI_COMMIT}",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-4-generated-build",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "GENERATED_BUILD",
                "SubjectId": "tsp-domain",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-4-database-initialization",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "DATABASE_INITIALIZATION",
                "SubjectId": base.DB,
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-4-python-postgres-conformance",
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
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        check_rows,
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["Status"] = "CLOSED"
    loop["AfterState"] = (
        "Attempt 1 was BLOCKED by a missing CLI. Retry 2 failed before transcript "
        "persistence. Retry 3 exposed npm ENOTDIR in the shared hosted-toolcache "
        "prefix. Retry 4 used an isolated npm prefix, installed the pinned CLI, "
        f"generated {view_count} vw_* views, initialized {base.DB}, and passed peer conformance."
    )
    loop["WitnessSummary"] = (
        "A pinned npm-installed Effortless CLI generated the backend; live Postgres "
        "and Python agree. All earlier blocked/failed attempts remain first-class rows."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY_4"

    frontier = base.table_index(rulebook, "TSPFrontierObligations")[
        "frontier-postgres-commissioning"
    ]
    frontier["Status"] = "CLOSED"
    frontier["ClosedByLoop"] = "tsp-loop-587"
    frontier["TrustDisposition"] = (
        "CLOSED_BY_ISOLATED_PINNED_NPM_CLI_AND_LIVE_POSTGRES_CONFORMANCE"
    )

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["Status"] = "CLOSED"
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY_4"

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
            "commissioning_transcript": f"sha256:{transcript_sha}",
            "retry_3_cli_failure_transcript": f"sha256:{retry3_failure_sha}",
            "effortless_cli_commit": CLI_COMMIT,
        }
    )
    certs = contract.setdefault("CurrentCertificates", [])
    cert_id = "gridville-postgres-substrate-conformance"
    if not any(row.get("CertificateId") == cert_id for row in certs):
        certs.append(
            {
                "CertificateId": cert_id,
                "Kind": "substrate-conformance-certificate",
                "Conclusion": (
                    f"Generated Postgres ({view_count} vw_* views) and Python agree on "
                    f"the declared TSP acceptance surface using EffortlessAPI/cli {CLI_VERSION}."
                ),
            }
        )
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
        "successful_attempt": 4,
        "github_run_id": RUN_ID,
        "cli_source": CLI_SOURCE,
        "cli_commit": CLI_COMMIT,
        "cli_version": CLI_VERSION,
        "database": base.DB,
        "generated_view_count": view_count,
        "live_checks": actual,
        "attempt_history": [
            {"attempt": 1, "status": "BLOCKED", "reason": "missing effortless executable"},
            {"attempt": 2, "status": "FAILED", "reason": "npm step failed before transcript persistence"},
            {"attempt": 3, "status": "FAILED", "reason": "npm ENOTDIR in shared hosted-toolcache prefix"},
            {"attempt": 4, "status": "SUCCEEDED", "reason": "isolated per-run npm prefix"},
        ],
        "hashes": {
            "build_input_rulebook": build_input_rulebook_sha,
            "postgres_projection_tree": postgres_tree_sha,
            "rulespeak_tree": rulespeak_tree_sha,
            "commissioning_transcript": transcript_sha,
            "retry_3_cli_failure_transcript": retry3_failure_sha,
        },
    }

    base.write(RULEBOOK, rulebook)
    base.run([sys.executable, str(base.FORMATTER), str(RULEBOOK)], cwd=REPO)
    base.write(CONTRACT, contract)
    base.write(SUCCESS_JSON, success_payload)

    readme = README.read_text()
    readme = base.replace_once(readme, "**Version:** 0.2.0", "**Version:** 0.3.1")
    readme = base.replace_once(
        readme,
        "The nearest open obligations are live Postgres commissioning, route reconstruction from inferred edge rows, a deliberately non-tight lower-bound fixture, degree-two forcing after pruning, neighborhood boundary-state contraction, subtour certificates, and explicit residual branching.",
        "The nearest open obligations are stronger component/crossing lower bounds, general neighborhood-state soundness beyond symmetric three-stop clusters, and explicit branching only after deterministic closure leaves positive residual ambiguity.",
    )
    readme = base.replace_once(
        readme,
        "The rulebook records every loop first as PLANNED with a before-state and closure criterion, then preserves the after-state in the same row. Gridville is reconstructed from inferred edges with zero branch decisions; twin triangles preserves a sound non-tight lower bound and yields the first finite neighborhood boundary-state contraction certificate. Loop 587 remains honestly BLOCKED when live generated Postgres cannot be commissioned in the execution environment.",
        "The rulebook records every loop first as PLANNED with a before-state and closure criterion, then preserves the after-state in the same row. Gridville is reconstructed from inferred edges with zero branch decisions; twin triangles preserves a sound non-tight lower bound and yields the first finite neighborhood boundary-state contraction certificate.\n\n**Postgres commissioning: CLOSED.** Attempts 1–3 remain recorded as blocked or failed evidence. Retry 4 installed `git+https://github.com/EffortlessAPI/cli.git#1551cb82d5a8992ad09e4c82d08e28493f92f7b4` into an isolated npm prefix, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. Artifact hashes and both failure and success transcripts are retained.",
    )
    README.write_text(readme)

    base.run([sys.executable, str(base.RECONCILER)])
    base.run([sys.executable, str(base.VALIDATOR)])

    print("TSP loop 587 commissioning retry 4: PASS")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")
    print(f"  views={view_count} database={base.DB}")
    print(f"  postgres_tree_sha256={postgres_tree_sha}")


if __name__ == "__main__":
    main()
