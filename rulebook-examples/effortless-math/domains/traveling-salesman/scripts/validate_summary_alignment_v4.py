#!/usr/bin/env python3
"""Validate TSP v0.4 summary projections against the canonical convergence rulebook."""
from __future__ import annotations

import json
from pathlib import Path
from typing import Any

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"
README = DOMAIN / "README.md"


def load(path: Path) -> dict[str, Any]:
    if not path.is_file():
        raise FileNotFoundError(path)
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


def main() -> None:
    rulebook = load(RULEBOOK)
    contract = load(CONTRACT)
    readme = README.read_text()
    tables = canonical_tables(rulebook)
    acceptance = contract["Acceptance"]
    loops = {int(row["LoopOrder"]): row for row in rulebook["TSPLoops"]["data"]}
    contract_loops = {int(row["LoopOrder"]): row for row in contract["Loops"]}

    expected_counts = {
        "RulebookTables": len(tables),
        "TSPLoops": len(rulebook["TSPLoops"]["data"]),
        "FrontierObligations": len(rulebook["TSPFrontierObligations"]["data"]),
    }
    optional = {
        "ConceptRows": "TSPConcepts",
        "TraversalStates": "TSPTraversalStates",
        "TraversalMembers": "TSPTraversalMembers",
        "EdgeCommitments": "TSPEdgeCommitments",
        "CostCertificates": "TSPCostCertificates",
        "Defects": "TSPDefects",
        "Regions": "TSPRegions",
    }
    for key, table in optional.items():
        if table in rulebook:
            expected_counts[key] = len(rulebook[table]["data"])
    for key, expected in expected_counts.items():
        actual = acceptance.get(key)
        if actual != expected:
            raise AssertionError(f"{key}: contract={actual!r} rulebook={expected!r}")

    if set(loops) != set(contract_loops):
        raise AssertionError("contract and rulebook loop sets disagree")
    for order, row in loops.items():
        if contract_loops[order]["Status"] != row["Status"]:
            raise AssertionError(f"loop {order} status disagreement")

    if acceptance["LastPlannedLoop"] != max(loops):
        raise AssertionError("LastPlannedLoop drift")
    terminal = {"CLOSED", "BLOCKED", "FALSIFIED", "DEFERRED"}
    highest = max(order for order, row in loops.items() if row["Status"] in terminal)
    if acceptance["HighestCompletedLoop"] != highest:
        raise AssertionError("HighestCompletedLoop drift")

    if loops.get(610, {}).get("Status") == "CLOSED":
        required_markers = [
            "Predicate convergence prediction",
            "32 surface concepts → 8 canonical primitives",
            "Balanced edge exchange",
            "Component debt",
            "Residual kernel",
            "Gauge ambiguity",
            "Postgres convergence build: PASS",
        ]
        for marker in required_markers:
            if marker not in readme:
                raise AssertionError(f"README missing convergence marker: {marker}")
        claims = contract["Claims"]
        if claims.get("ModelConvergenceObserved") is not True:
            raise AssertionError("final convergence claim missing")
        if claims.get("GeneralConceptConvergenceProved") is not False:
            raise AssertionError("general convergence must remain a non-claim")
        if claims.get("TwinTrianglesOptimalityProved") is not True:
            raise AssertionError("twin-triangles finite optimality claim missing")

    print("traveling-salesman summary v4 alignment: PASS")
    print(
        f"tables={len(tables)} loops={len(loops)} "
        f"last={max(loops)} postgres={acceptance.get('PostgresCommissioningStatus')}"
    )


if __name__ == "__main__":
    main()
