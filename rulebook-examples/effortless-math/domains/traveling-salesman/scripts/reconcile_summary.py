#!/usr/bin/env python3
"""Reconcile TSP summary projections from the canonical rulebook.

This script intentionally updates only mechanically-derived projection fields:
row/table counts, loop statuses, the highest completed/planned loop, and the
canonical rulebook hash. It does not invent or promote mathematical claims.
"""
from __future__ import annotations

import hashlib
import json
from pathlib import Path
from typing import Any

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"


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


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main() -> None:
    rulebook = load_json(RULEBOOK)
    contract = load_json(CONTRACT)
    tables = canonical_tables(rulebook)
    acceptance = contract.setdefault("Acceptance", {})

    count_tables = {
        "Cities": "Cities",
        "Neighborhoods": "Neighborhoods",
        "Addresses": "Addresses",
        "TSPInstances": "TSPInstances",
        "InstanceStops": "InstanceStops",
        "TravelEdges": "TravelEdges",
        "CandidateTours": "CandidateTours",
        "TourStops": "TourStops",
        "TourLegs": "TourLegs",
        "TSPLoops": "TSPLoops",
        "FrontierObligations": "TSPFrontierObligations",
        "ExecutionRuns": "TSPExecutionRuns",
        "InferenceStates": "TSPInferenceStates",
        "InferenceApplications": "TSPInferenceApplications",
        "InferenceAntecedents": "TSPInferenceAntecedents",
        "EdgeStates": "TSPEdgeStates",
        "EdgeSupports": "TSPEdgeSupports",
        "DerivedEdgeSets": "TSPDerivedEdgeSets",
        "DerivedEdgeSetMembers": "TSPDerivedEdgeSetMembers",
        "ConnectedDegreeTwoCertificates": "TSPConnectedDegreeTwoCertificates",
        "RouteReconstructions": "TSPRouteReconstructions",
        "RouteReconstructionSteps": "TSPRouteReconstructionSteps",
        "SearchCertificates": "TSPSearchCertificates",
        "ConstraintRounds": "TSPConstraintRounds",
        "ConstraintDecisions": "TSPConstraintDecisions",
        "ClusterBoundaryStates": "TSPClusterBoundaryStates",
        "ClusterBoundaryStateMembers": "TSPClusterBoundaryStateMembers",
        "ClusterContractionCertificates": "TSPClusterContractionCertificates",
    }
    acceptance["RulebookTables"] = len(tables)
    for contract_key, table_name in count_tables.items():
        acceptance[contract_key] = len(rulebook[table_name]["data"])

    rulebook_loops = {
        int(row["LoopOrder"]): row for row in rulebook["TSPLoops"]["data"]
    }
    contract_loops = {
        int(row["LoopOrder"]): row for row in contract["Loops"]
    }
    if set(rulebook_loops) != set(contract_loops):
        missing = sorted(set(rulebook_loops) - set(contract_loops))
        extra = sorted(set(contract_loops) - set(rulebook_loops))
        raise AssertionError(
            f"contract loop set differs from canonical rulebook; missing={missing}, extra={extra}"
        )
    for order, rulebook_row in rulebook_loops.items():
        contract_loops[order]["Status"] = rulebook_row["Status"]

    acceptance["LastPlannedLoop"] = max(rulebook_loops)
    terminal = {"CLOSED", "BLOCKED", "FALSIFIED", "DEFERRED"}
    acceptance["HighestCompletedLoop"] = max(
        order for order, row in rulebook_loops.items() if row["Status"] in terminal
    )
    acceptance["PostgresCommissioningStatus"] = rulebook_loops[587]["Status"]
    acceptance["ActiveImportedDependencies"] = sum(
        1
        for row in rulebook["TSPFrontierObligations"]["data"]
        if row.get("IsImportedDependency") is True and row.get("Status") != "CLOSED"
    )

    contract.setdefault("ArtifactHashes", {})["rulebook"] = f"sha256:{sha256(RULEBOOK)}"
    CONTRACT.write_text(json.dumps(contract, indent=2, ensure_ascii=False) + "\n")

    print("traveling-salesman summary reconciliation: PASS")
    print(
        f"tables={len(tables)} loops={len(rulebook_loops)} "
        f"highest={acceptance['HighestCompletedLoop']} "
        f"rulebook_sha256={sha256(RULEBOOK)}"
    )


if __name__ == "__main__":
    main()
