#!/usr/bin/env python3
"""Validate that TSP summary projections agree with the canonical rulebook.

The rulebook remains canonical. This guard prevents problem-contract.json and
README.md from silently retaining stale counts, loop statuses, substrate state,
or headline claims after later semantic loops.
"""
from __future__ import annotations

import json
from pathlib import Path
from typing import Any

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"
README = DOMAIN / "README.md"


def load_json(path: Path) -> dict[str, Any]:
    if not path.is_file():
        raise FileNotFoundError(f"missing required file: {path}")
    return json.loads(path.read_text())


def canonical_tables(rulebook: dict[str, Any]) -> dict[str, dict[str, Any]]:
    return {
        key: value
        for key, value in rulebook.items()
        if key != "__meta__"
        and isinstance(value, dict)
        and isinstance(value.get("schema"), list)
        and isinstance(value.get("data"), list)
    }


def assert_equal(label: str, actual: Any, expected: Any) -> None:
    if actual != expected:
        raise AssertionError(f"{label}: contract={actual!r}, rulebook={expected!r}")


def main() -> None:
    rulebook = load_json(RULEBOOK)
    contract = load_json(CONTRACT)
    readme = README.read_text()
    tables = canonical_tables(rulebook)
    acceptance = contract["Acceptance"]

    expected_counts = {
        "RulebookTables": len(tables),
        "Cities": len(rulebook["Cities"]["data"]),
        "Neighborhoods": len(rulebook["Neighborhoods"]["data"]),
        "Addresses": len(rulebook["Addresses"]["data"]),
        "TSPInstances": len(rulebook["TSPInstances"]["data"]),
        "InstanceStops": len(rulebook["InstanceStops"]["data"]),
        "TravelEdges": len(rulebook["TravelEdges"]["data"]),
        "CandidateTours": len(rulebook["CandidateTours"]["data"]),
        "TourStops": len(rulebook["TourStops"]["data"]),
        "TourLegs": len(rulebook["TourLegs"]["data"]),
        "TSPLoops": len(rulebook["TSPLoops"]["data"]),
        "FrontierObligations": len(rulebook["TSPFrontierObligations"]["data"]),
        "ExecutionRuns": len(rulebook["TSPExecutionRuns"]["data"]),
        "InferenceStates": len(rulebook["TSPInferenceStates"]["data"]),
        "InferenceApplications": len(rulebook["TSPInferenceApplications"]["data"]),
        "InferenceAntecedents": len(rulebook["TSPInferenceAntecedents"]["data"]),
        "EdgeStates": len(rulebook["TSPEdgeStates"]["data"]),
        "EdgeSupports": len(rulebook["TSPEdgeSupports"]["data"]),
        "DerivedEdgeSets": len(rulebook["TSPDerivedEdgeSets"]["data"]),
        "DerivedEdgeSetMembers": len(rulebook["TSPDerivedEdgeSetMembers"]["data"]),
        "ConnectedDegreeTwoCertificates": len(rulebook["TSPConnectedDegreeTwoCertificates"]["data"]),
        "RouteReconstructions": len(rulebook["TSPRouteReconstructions"]["data"]),
        "RouteReconstructionSteps": len(rulebook["TSPRouteReconstructionSteps"]["data"]),
        "SearchCertificates": len(rulebook["TSPSearchCertificates"]["data"]),
        "ConstraintRounds": len(rulebook["TSPConstraintRounds"]["data"]),
        "ConstraintDecisions": len(rulebook["TSPConstraintDecisions"]["data"]),
        "ClusterBoundaryStates": len(rulebook["TSPClusterBoundaryStates"]["data"]),
        "ClusterBoundaryStateMembers": len(rulebook["TSPClusterBoundaryStateMembers"]["data"]),
        "ClusterContractionCertificates": len(rulebook["TSPClusterContractionCertificates"]["data"]),
    }
    for key, expected in expected_counts.items():
        assert_equal(key, acceptance.get(key), expected)

    rulebook_loops = {
        int(row["LoopOrder"]): row for row in rulebook["TSPLoops"]["data"]
    }
    contract_loops = {
        int(row["LoopOrder"]): row for row in contract["Loops"]
    }
    if sorted(rulebook_loops) != list(range(577, 597)):
        raise AssertionError("canonical loop sequence must be contiguous from 577 through 596")
    if set(contract_loops) != set(rulebook_loops):
        raise AssertionError("contract and rulebook loop sets disagree")
    for order, row in rulebook_loops.items():
        assert_equal(f"loop {order} status", contract_loops[order]["Status"], row["Status"])

    assert_equal("highest completed loop", acceptance["HighestCompletedLoop"], 596)
    assert_equal("last planned loop", acceptance["LastPlannedLoop"], 596)
    postgres_status = rulebook_loops[587]["Status"]
    if postgres_status not in {"BLOCKED", "CLOSED"}:
        raise AssertionError(f"unexpected Postgres commissioning status: {postgres_status}")
    assert_equal("Postgres commissioning status", acceptance["PostgresCommissioningStatus"], postgres_status)
    assert_equal("active imports", acceptance["ActiveImportedDependencies"], 0)

    if contract["Claims"]["RouteReconstructedWithoutSuppliedAntecedent"] is not True:
        raise AssertionError("route-reconstruction claim must remain true after loop 591")
    commissioned = postgres_status == "CLOSED"
    if contract["Claims"].get("PostgresConformanceCommissioned") is not commissioned:
        raise AssertionError(
            "PostgresConformanceCommissioned must agree with the canonical loop-587 status"
        )

    loop_587_runs = [
        row for row in rulebook["TSPExecutionRuns"]["data"]
        if row["TSPLoop"] == "tsp-loop-587"
    ]
    if not loop_587_runs:
        raise AssertionError("loop 587 has no execution-attempt rows")
    if commissioned:
        successful = [
            row for row in loop_587_runs
            if row["Status"] == "SUCCEEDED"
            and row["BuildSucceeded"] is True
            and row["DatabaseInitialized"] is True
            and row["ConformanceSucceeded"] is True
        ]
        if not successful:
            raise AssertionError("closed loop 587 lacks a successful execution run")
        postgres_marker = "Postgres commissioning: CLOSED"
    else:
        blocked = [row for row in loop_587_runs if row["Status"] == "BLOCKED"]
        if not blocked or any(not row.get("FailureReason") for row in blocked):
            raise AssertionError("blocked loop 587 lacks an exact failure record")
        postgres_marker = "Postgres commissioning: BLOCKED"

    required_markers = [
        "Loops 577–596",
        "12 → 1",
        postgres_marker,
        "36 → 9",
        "CandidateUsedAsAntecedent=false",
    ]
    for marker in required_markers:
        if marker not in readme:
            raise AssertionError(f"README missing required marker: {marker}")

    print("traveling-salesman summary alignment: PASS")
    print(
        f"tables={len(tables)} loops={len(rulebook_loops)} "
        f"instances={len(rulebook['TSPInstances']['data'])} "
        f"edges={len(rulebook['TravelEdges']['data'])} "
        f"postgres={postgres_status}"
    )


if __name__ == "__main__":
    main()
