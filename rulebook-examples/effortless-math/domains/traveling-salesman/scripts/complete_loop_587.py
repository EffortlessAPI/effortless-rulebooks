#!/usr/bin/env python3
"""Close TSP loop 587 after a live generated-Postgres conformance run.

The initial BLOCKED attempt remains immutable evidence. This script appends a
successful retry, closes the existing substrate obligation, reconciles summary
projections, and validates the completed state. It must run only after:

    start.sh build
    start.sh db
    start.sh test
    start.sh show

have all exited successfully against the same checkout and database.
"""
from __future__ import annotations

import hashlib
import json
import os
import subprocess
import sys
from pathlib import Path
from typing import Any, Iterable

REPO = Path(__file__).resolve().parents[5]
DOMAIN = REPO / "rulebook-examples" / "effortless-math" / "domains" / "traveling-salesman"
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"
README = DOMAIN / "README.md"
PG_DIR = DOMAIN / "effortless-postgres"
RULESPEAK_DIR = DOMAIN / "rulespeak"
LOG = DOMAIN / "testing" / "postgres-commissioning-success.log"
SUCCESS_JSON = DOMAIN / "testing" / "postgres-commissioning-success.json"
FORMATTER = DOMAIN.parents[1] / "scripts" / "format-rulebook.py"
RECONCILER = DOMAIN / "scripts" / "reconcile_summary.py"
VALIDATOR = DOMAIN / "scripts" / "validate_rulebook.py"
TAKE_TEST = DOMAIN / "testing" / "take-test.py"

RUN_ID = os.environ.get("GITHUB_RUN_ID", "local-loop-587-retry")
CLI_SOURCE = os.environ.get(
    "EFFORTLESS_CLI_SOURCE",
    "https://github.com/EffortlessAPI/cli.git#1551cb82d5a8992ad09e4c82d08e28493f92f7b4",
)
CLI_COMMIT = os.environ.get(
    "EFFORTLESS_CLI_COMMIT", "1551cb82d5a8992ad09e4c82d08e28493f92f7b4"
)
CLI_VERSION = os.environ.get("EFFORTLESS_CLI_VERSION", "2026-06-09.06.13")
DB = os.environ.get("TSP_DB", "erb_traveling_salesman")
PGHOST = os.environ.get("PGHOST", "localhost")
PGUSER = os.environ.get("PGUSER", "postgres")
SUCCESS_RUN_ID = "postgres-commissioning-loop-587-retry-2"


def load(path: Path) -> dict[str, Any]:
    if not path.is_file():
        raise FileNotFoundError(f"missing required file: {path}")
    return json.loads(path.read_text())


def write(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n")


def run(
    cmd: list[str], *, cwd: Path = DOMAIN, capture: bool = False
) -> subprocess.CompletedProcess[str]:
    proc = subprocess.run(cmd, cwd=cwd, text=True, capture_output=capture)
    if proc.returncode != 0:
        detail = (proc.stdout or "") + "\n" + (proc.stderr or "")
        raise RuntimeError(f"command failed ({proc.returncode}): {' '.join(cmd)}\n{detail}")
    return proc


def sha256_file(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def sha256_tree(root: Path) -> str:
    if not root.is_dir():
        raise FileNotFoundError(f"missing required generated directory: {root}")
    digest = hashlib.sha256()
    files = sorted(path for path in root.rglob("*") if path.is_file())
    if not files:
        raise AssertionError(f"generated directory is empty: {root}")
    for path in files:
        rel = path.relative_to(root).as_posix().encode("utf-8")
        digest.update(len(rel).to_bytes(8, "big"))
        digest.update(rel)
        payload = path.read_bytes()
        digest.update(len(payload).to_bytes(8, "big"))
        digest.update(payload)
    return digest.hexdigest()


def table_index(rulebook: dict[str, Any], table: str) -> dict[str, dict[str, Any]]:
    identifier = rulebook[table]["schema"][0]["name"]
    return {row[identifier]: row for row in rulebook[table]["data"]}


def upsert_rows(
    table: dict[str, Any], identifier: str, new_rows: Iterable[dict[str, Any]]
) -> None:
    existing = {row[identifier]: row for row in table["data"]}
    order = [row[identifier] for row in table["data"]]
    for row in new_rows:
        row_id = row[identifier]
        if row_id not in existing:
            order.append(row_id)
        existing[row_id] = row
    table["data"] = [existing[row_id] for row_id in order]


def psql_scalar(sql: str) -> str:
    proc = run(
        [
            "psql",
            "-h",
            PGHOST,
            "-U",
            PGUSER,
            "-d",
            DB,
            "-v",
            "ON_ERROR_STOP=1",
            "-tA",
            "-c",
            sql,
        ],
        capture=True,
    )
    return proc.stdout.strip()


def replace_once(text: str, old: str, new: str) -> str:
    if old not in text:
        raise AssertionError(f"README projection marker not found: {old!r}")
    return text.replace(old, new, 1)


def main() -> None:
    if not PG_DIR.joinpath("init-db.sh").is_file():
        raise FileNotFoundError("generated Postgres initializer is missing after build")
    if not LOG.is_file():
        raise FileNotFoundError(f"commissioning transcript is missing: {LOG}")

    # Re-run the peer conformance test and verify key live view facts before any
    # canonical status is changed.
    run([sys.executable, str(TAKE_TEST)])
    view_count = int(
        psql_scalar(
            "SELECT count(*) FROM pg_views "
            "WHERE schemaname='public' AND viewname LIKE 'vw_%'"
        )
    )
    if view_count < 40:
        raise AssertionError(f"expected at least 40 generated vw_* views, got {view_count}")

    gridville = psql_scalar(
        "SELECT is_complete_undirected_graph::text "
        "FROM vw_tsp_instances WHERE tsp_instance_id='tsp-gridville-5'"
    )
    reference = psql_scalar(
        "SELECT is_hamiltonian_cycle_witness::text || ',' || "
        "is_optimality_proved::text || ',' || total_travel_cost::text "
        "FROM vw_candidate_tours WHERE candidate_tour_id='tour-reference-ring'"
    )
    lower_bound = psql_scalar(
        "SELECT is_certified::text || ',' || lower_bound_cost::text "
        "FROM vw_instance_lower_bounds "
        "WHERE instance_lower_bound_id='degree-two-lower-bound-gridville-5'"
    )
    expected = {
        "gridville_complete": "true",
        "reference_tour": "true,true,14",
        "degree_two_lower_bound": "true,14",
    }
    actual = {
        "gridville_complete": gridville,
        "reference_tour": reference,
        "degree_two_lower_bound": lower_bound,
    }
    if actual != expected:
        raise AssertionError(f"live Postgres acceptance mismatch: {actual!r}")

    build_input_rulebook_sha = sha256_file(RULEBOOK)
    postgres_tree_sha = sha256_tree(PG_DIR)
    rulespeak_tree_sha = sha256_tree(RULESPEAK_DIR)
    log_sha = sha256_file(LOG)

    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)

    blocked_run = table_index(rulebook, "TSPExecutionRuns").get(
        "postgres-commissioning-loop-587"
    )
    if not blocked_run or blocked_run["Status"] != "BLOCKED":
        raise AssertionError("the historical blocked loop-587 attempt is missing")

    upsert_rows(
        rulebook["TSPExecutionRuns"],
        "TSPExecutionRunId",
        [
            {
                "TSPExecutionRunId": SUCCESS_RUN_ID,
                "TSPLoop": "tsp-loop-587",
                "Substrate": "POSTGRES_GENERATED_VIEWS",
                "AttemptedAt": RUN_ID,
                "BuildCommand": "npm install -g <pinned EffortlessAPI/cli>; bash start.sh build; bash start.sh db; bash start.sh test; bash start.sh show",
                "BuildSucceeded": True,
                "DatabaseInitialized": True,
                "ConformanceSucceeded": True,
                "Status": "SUCCEEDED",
                "FailureReason": None,
            }
        ],
    )

    artifact_rows = [
        {
            "TSPArtifactId": "loop-587-retry-2-build-input-rulebook",
            "ExecutionRun": SUCCESS_RUN_ID,
            "ArtifactKind": "BUILD_INPUT_RULEBOOK",
            "RelativePath": str(RULEBOOK.relative_to(REPO)),
            "SHA256": build_input_rulebook_sha,
            "IsPresent": True,
        },
        {
            "TSPArtifactId": "loop-587-retry-2-generated-postgres-tree",
            "ExecutionRun": SUCCESS_RUN_ID,
            "ArtifactKind": "GENERATED_POSTGRES_TREE",
            "RelativePath": str(PG_DIR.relative_to(REPO)),
            "SHA256": postgres_tree_sha,
            "IsPresent": True,
        },
        {
            "TSPArtifactId": "loop-587-retry-2-rulespeak-tree",
            "ExecutionRun": SUCCESS_RUN_ID,
            "ArtifactKind": "GENERATED_RULESPEAK_TREE",
            "RelativePath": str(RULESPEAK_DIR.relative_to(REPO)),
            "SHA256": rulespeak_tree_sha,
            "IsPresent": True,
        },
        {
            "TSPArtifactId": "loop-587-retry-2-transcript",
            "ExecutionRun": SUCCESS_RUN_ID,
            "ArtifactKind": "COMMISSIONING_TRANSCRIPT",
            "RelativePath": str(LOG.relative_to(REPO)),
            "SHA256": log_sha,
            "IsPresent": True,
        },
    ]
    upsert_rows(rulebook["TSPArtifacts"], "TSPArtifactId", artifact_rows)

    check_rows = []
    for key, observed in actual.items():
        check_rows.append(
            {
                "TSPConformanceCheckId": f"loop-587-retry-2-{key.replace('_', '-')}",
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
                "TSPConformanceCheckId": "loop-587-retry-2-generated-build",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "GENERATED_BUILD",
                "SubjectId": "tsp-domain",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-2-database-initialization",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "DATABASE_INITIALIZATION",
                "SubjectId": DB,
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
            {
                "TSPConformanceCheckId": "loop-587-retry-2-python-postgres-conformance",
                "ExecutionRun": SUCCESS_RUN_ID,
                "CheckKind": "PYTHON_POSTGRES_CONFORMANCE",
                "SubjectId": "testing/take-test.py",
                "ExpectedValue": "PASS",
                "ActualValue": "PASS",
                "Status": "PASS",
            },
        ]
    )
    upsert_rows(
        rulebook["TSPConformanceChecks"],
        "TSPConformanceCheckId",
        check_rows,
    )

    loop = next(
        row for row in rulebook["TSPLoops"]["data"] if row["LoopOrder"] == 587
    )
    loop["Status"] = "CLOSED"
    loop["AfterState"] = (
        "The first attempt remains recorded as BLOCKED because the runner lacked "
        "the CLI. Retry 2 installed the pinned EffortlessAPI/cli package, generated "
        f"{view_count} vw_* views, initialized {DB}, and passed Python/Postgres conformance."
    )
    loop["WitnessSummary"] = (
        "Pinned CLI installation, generated build, database initialization, live view "
        "checks, and peer conformance all passed; the earlier blocked attempt remains in the ledger."
    )
    loop["CompletionDisposition"] = "CLOSED_ON_RETRY"

    frontier = table_index(rulebook, "TSPFrontierObligations")[
        "frontier-postgres-commissioning"
    ]
    frontier["Status"] = "CLOSED"
    frontier["ClosedByLoop"] = "tsp-loop-587"
    frontier["TrustDisposition"] = (
        "CLOSED_BY_PINNED_NPM_CLI_GENERATION_AND_LIVE_POSTGRES_CONFORMANCE"
    )

    contract_loop = next(row for row in contract["Loops"] if row["LoopOrder"] == 587)
    contract_loop["Status"] = "CLOSED"
    contract_loop["AfterState"] = loop["AfterState"]
    contract_loop["Result"] = loop["WitnessSummary"]
    contract_loop["CompletionDisposition"] = "CLOSED_ON_RETRY"

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
        "internalized in the rulebook. The generated Postgres projection is now a "
        "commissioned peer substrate, built with the pinned EffortlessAPI/cli revision "
        f"{CLI_COMMIT} and checked against Python."
    )
    contract.setdefault("Claims", {})["PostgresConformanceCommissioned"] = True
    contract.setdefault("ArtifactHashes", {}).update(
        {
            "build_input_rulebook": f"sha256:{build_input_rulebook_sha}",
            "postgres_projection_tree": f"sha256:{postgres_tree_sha}",
            "rulespeak_tree": f"sha256:{rulespeak_tree_sha}",
            "commissioning_transcript": f"sha256:{log_sha}",
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
                    "the declared TSP acceptance surface using pinned EffortlessAPI/cli "
                    f"{CLI_VERSION} ({CLI_COMMIT})."
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
        "historical_blocked_attempt_preserved": True,
        "github_run_id": RUN_ID,
        "cli_source": CLI_SOURCE,
        "cli_commit": CLI_COMMIT,
        "cli_version": CLI_VERSION,
        "database": DB,
        "generated_view_count": view_count,
        "live_checks": actual,
        "hashes": {
            "build_input_rulebook": build_input_rulebook_sha,
            "postgres_projection_tree": postgres_tree_sha,
            "rulespeak_tree": rulespeak_tree_sha,
            "commissioning_transcript": log_sha,
        },
    }

    write(RULEBOOK, rulebook)
    run([sys.executable, str(FORMATTER), str(RULEBOOK)], cwd=REPO)
    write(CONTRACT, contract)
    write(SUCCESS_JSON, success_payload)

    readme = README.read_text()
    readme = replace_once(readme, "**Version:** 0.2.0", "**Version:** 0.3.1")
    readme = replace_once(
        readme,
        "The nearest open obligations are live Postgres commissioning, route reconstruction from inferred edge rows, a deliberately non-tight lower-bound fixture, degree-two forcing after pruning, neighborhood boundary-state contraction, subtour certificates, and explicit residual branching.",
        "The nearest open obligations are stronger component/crossing lower bounds, general neighborhood-state soundness beyond symmetric three-stop clusters, and explicit branching only after deterministic closure leaves positive residual ambiguity.",
    )
    readme = replace_once(
        readme,
        "The rulebook records every loop first as PLANNED with a before-state and closure criterion, then preserves the after-state in the same row. Gridville is reconstructed from inferred edges with zero branch decisions; twin triangles preserves a sound non-tight lower bound and yields the first finite neighborhood boundary-state contraction certificate. Loop 587 remains honestly BLOCKED when live generated Postgres cannot be commissioned in the execution environment.",
        "The rulebook records every loop first as PLANNED with a before-state and closure criterion, then preserves the after-state in the same row. Gridville is reconstructed from inferred edges with zero branch decisions; twin triangles preserves a sound non-tight lower bound and yields the first finite neighborhood boundary-state contraction certificate.\n\n**Postgres commissioning: CLOSED.** The first loop-587 attempt remains recorded as BLOCKED. Retry 2 installed `https://github.com/EffortlessAPI/cli.git#1551cb82d5a8992ad09e4c82d08e28493f92f7b4` with npm, generated the Postgres and RuleSpeak projections, initialized `erb_traveling_salesman`, and passed Python/Postgres conformance. The live certificate records artifact hashes and does not erase the earlier failure.",
    )
    README.write_text(readme)

    # Reconcile all mechanically-derived counts and the final canonical hash only
    # after the successful execution rows have been written and formatted.
    run([sys.executable, str(RECONCILER)])
    run([sys.executable, str(VALIDATOR)])

    print("TSP loop 587 commissioning retry: PASS")
    print(f"  cli={CLI_VERSION}@{CLI_COMMIT}")
    print(f"  views={view_count} database={DB}")
    print(f"  postgres_tree_sha256={postgres_tree_sha}")


if __name__ == "__main__":
    main()
