#!/usr/bin/env python3
"""State-aware validator for TSP loops 577-610 and the convergence experiment."""
from __future__ import annotations

import sys
from collections import defaultdict
from pathlib import Path
from typing import Any

import validate_rulebook_v3 as v3
from reference_model import (
    evaluate_graph,
    evaluate_instance_lower_bounds,
    evaluate_optimality_certificates,
    evaluate_tours,
)

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"


def fail(message: str) -> None:
    raise AssertionError(message)


def table_index(rulebook: dict[str, Any], table: str) -> dict[str, dict[str, Any]]:
    identifier = rulebook[table]["schema"][0]["name"]
    return {row[identifier]: row for row in rulebook[table]["data"]}


def terminal(row: dict[str, Any]) -> bool:
    return row["Status"] in {"CLOSED", "BLOCKED", "FALSIFIED", "DEFERRED"}


def concept_rows(rulebook: dict[str, Any], *, kind: str | None = None, status: str | None = None) -> list[dict[str, Any]]:
    rows = rulebook.get("TSPConcepts", {}).get("data", [])
    if kind is not None:
        rows = [row for row in rows if row["ConceptKind"] == kind]
    if status is not None:
        rows = [row for row in rows if row["Status"] == status]
    return rows


def validate_baseline(rulebook: dict[str, Any], loops: dict[int, dict[str, Any]]) -> None:
    graph = evaluate_graph(rulebook)
    tours = evaluate_tours(rulebook)
    if not graph["tsp-gridville-5"]["is_complete_undirected_graph"]:
        fail("Gridville graph no longer complete")
    reference = tours["tour-reference-ring"]
    if not reference.is_hamiltonian_cycle_witness or reference.total_travel_cost != 14:
        fail(f"Gridville reference witness drift: {reference}")

    if terminal(loops[587]):
        runs = table_index(rulebook, "TSPExecutionRuns")
        successful = [row for row in runs.values() if row["TSPLoop"] == "tsp-loop-587" and row["Status"] == "SUCCEEDED"]
        if not successful:
            fail("loop 587 is terminal without a successful Postgres commissioning run")

    selected = [
        row for row in rulebook["TSPEdgeStates"]["data"]
        if row["InferenceState"] == "state-gridville-local-bound-selections"
    ]
    if len(selected) != 5 or any(row["DecisionStatus"] != "SELECTED" for row in selected):
        fail("loop 588 selected-edge witness drift")

    grid_members = [
        row for row in rulebook["TSPDerivedEdgeSetMembers"]["data"]
        if row["DerivedEdgeSet"] == "edge-set-gridville-local-bound-union"
    ]
    if len(grid_members) != 5 or not all(row["SelectedAtBothEndpoints"] for row in grid_members):
        fail("loop 589 derived edge union drift")

    cert = table_index(rulebook, "TSPConnectedDegreeTwoCertificates")[
        "connected-cycle-gridville-local-bound-union"
    ]
    expected = (5, 0, 1, 0, 4)
    actual = (
        cert["EdgeCount"],
        cert["DegreeViolationCount"],
        cert["ComponentCount"],
        cert["ProperSubtourCount"],
        cert["SpanningTreeEdgeCount"],
    )
    if actual != expected:
        fail(f"loop 590 connected-degree-two certificate drift: {actual}")

    route = table_index(rulebook, "TSPRouteReconstructions")[
        "reconstruction-gridville-local-bound-union"
    ]
    steps = sorted(
        (
            row for row in rulebook["TSPRouteReconstructionSteps"]["data"]
            if row["RouteReconstruction"] == route["TSPRouteReconstructionId"]
        ),
        key=lambda row: row["StepOrder"],
    )
    if route["CandidateUsedAsAntecedent"] or len(steps) != 5 or route["TotalCost"] != 14:
        fail("loop 591 route reconstruction drift")

    search = table_index(rulebook, "TSPSearchCertificates")[
        "search-gridville-reconstructed-route"
    ]
    if (
        search["InitialRouteClassCount"] != 12
        or search["SurvivingRouteClassCount"] != 1
        or search["BranchDecisionCount"] != 0
        or search["ResidualAmbiguityCount"] != 0
    ):
        fail("loop 592 search accounting drift")

    twin_stops = [
        row for row in rulebook["InstanceStops"]["data"]
        if row["TSPInstance"] == "tsp-twin-triangles-6"
    ]
    twin_edges = [
        row for row in rulebook["TravelEdges"]["data"]
        if row["TSPInstance"] == "tsp-twin-triangles-6"
    ]
    if len(twin_stops) != 6 or len(twin_edges) != 15:
        fail("loop 593 twin-triangles fixture drift")
    twin_set = table_index(rulebook, "TSPDerivedEdgeSets")[
        "edge-set-twin-triangles-local-bound-union"
    ]
    if twin_set["ConnectedComponentCount"] != 2 or twin_set["ProperSubtourCount"] != 2:
        fail("loop 593 must retain two selected components and two proper subtours")

    forced = [
        row for row in rulebook["TSPConstraintDecisions"]["data"]
        if row["DecisionStatus"] == "FORCED"
        and row["ConstraintRound"] == "constraint-round-sparse-forcing-1"
    ]
    if len(forced) != 5 or any(not row["IsDeterministic"] for row in forced):
        fail("loop 594 forced-edge certificate drift")

    decisions = table_index(rulebook, "TSPConstraintDecisions")
    if decisions["forbid-sparse-edge-b-d-degree-saturation"]["ReasonCode"] != "DEGREE_TWO_SATURATION":
        fail("loop 595 degree-saturation certificate drift")
    if decisions["forbid-twin-edge-wa-wc-proper-subtour"]["ReasonCode"] != "WOULD_CLOSE_PROPER_SUBTOUR":
        fail("loop 595 subtour-forbidding certificate drift")

    cluster_states = [
        row for row in rulebook["TSPClusterBoundaryStates"]["data"]
        if row["TSPInstance"] == "tsp-twin-triangles-6"
    ]
    if len(cluster_states) != 6 or any(not row["IsHamiltonianPath"] or row["IsDominated"] for row in cluster_states):
        fail("loop 596 cluster-state drift")


def validate_convergence(rulebook: dict[str, Any], contract: dict[str, Any], loops: dict[int, dict[str, Any]]) -> None:
    def closed(order: int) -> bool:
        return order in loops and loops[order]["Status"] == "CLOSED"

    if closed(597):
        if "TSPConcepts" not in rulebook:
            fail("loop 597 closed without TSPConcepts")
        legacy = concept_rows(rulebook, kind="LEGACY_SURFACE")
        canonical = concept_rows(rulebook, kind="CANONICAL_PRIMITIVE")
        prediction = table_index(rulebook, "TSPConcepts").get(
            "concept-predicate-convergence-prediction"
        )
        if len(legacy) != 32 or len(canonical) != 8 or prediction is None:
            fail(
                f"loop 597 concept inventory mismatch: legacy={len(legacy)} canonical={len(canonical)}"
            )
        prediction_status = contract.get("ConvergencePrediction", {}).get("Status")
        expected_prediction_status = (
            "OBSERVED_FOR_CURRENT_SCOPE" if closed(610) else "RECORDED_NOT_PROVED"
        )
        if prediction_status != expected_prediction_status:
            fail(
                "loop 597 convergence prediction status mismatch: "
                f"expected={expected_prediction_status} actual={prediction_status}"
            )

    if closed(598):
        states = table_index(rulebook, "TSPTraversalStates")
        required = {
            "traversal-supplied-gridville-reference",
            "traversal-reconstructed-gridville",
            "traversal-supplied-twin-feasible-24",
            "traversal-twin-local-two-factor",
        }
        if not required.issubset(states):
            fail(f"loop 598 missing traversal states: {sorted(required - set(states))}")
        cluster = [row for row in states.values() if row["StateKind"] == "REGION_PATH"]
        if len(cluster) != 6:
            fail(f"loop 598 expected six region-path states, got {len(cluster)}")
        if len(rulebook["TSPTraversalMembers"]["data"]) < 70:
            fail("loop 598 traversal-member projection is unexpectedly small")

    if closed(599):
        commitments = rulebook["TSPEdgeCommitments"]["data"]
        counts = defaultdict(int)
        for row in commitments:
            counts[row["Modality"]] += 1
        if counts["SELECTED"] != 11 or counts["FORCED"] != 5 or counts["FORBIDDEN"] != 2:
            fail(f"loop 599 edge commitment counts mismatch: {dict(counts)}")

    if closed(600):
        fields = {row["name"] for row in rulebook["TSPInferenceStates"]["schema"]}
        required_fields = {
            "RoundOrder",
            "BranchDecisionCount",
            "BacktrackCount",
            "ResidualAmbiguityCount",
            "AlternativeCount",
            "StateModality",
            "IsDeterministicFixedPoint",
        }
        if not required_fields.issubset(fields):
            fail(f"loop 600 closure-state fields missing: {sorted(required_fields - fields)}")
        states = table_index(rulebook, "TSPInferenceStates")
        if states["state-gridville-closure-fixed-point"]["ResidualAmbiguityCount"] != 0:
            fail("loop 600 Gridville closure state must have zero residual ambiguity")
        if states["state-twin-closure-fixed-point"]["ResidualAmbiguityCount"] != 60:
            fail("loop 600 twin closure state must preserve sixty raw alternatives")

    if closed(601):
        costs = table_index(rulebook, "TSPCostCertificates")
        expected_values = {
            "cost-gridville-degree-lower": 14,
            "cost-gridville-reconstructed-upper": 14,
            "cost-gridville-exact": 14,
            "cost-twin-degree-lower": 6,
            "cost-twin-feasible-upper": 24,
            "cost-twin-local-two-factor": 6,
        }
        for key, value in expected_values.items():
            if key not in costs or float(costs[key]["Value"]) != value:
                fail(f"loop 601 cost certificate mismatch for {key}")

    if closed(602):
        defects = table_index(rulebook, "TSPDefects")
        required = {
            "defect-twin-component-excess",
            "defect-twin-proper-subtours",
            "defect-twin-bound-gap",
            "defect-twin-residual-ambiguity",
        }
        if not required.issubset(defects):
            fail(f"loop 602 missing defects: {sorted(required - set(defects))}")

    if closed(603):
        repairs = [
            row for row in rulebook["TSPInferenceApplications"]["data"]
            if row.get("IsRepair") is True
        ]
        if len(repairs) < 5:
            fail(f"loop 603 expected at least five repair applications, got {len(repairs)}")

    if closed(604):
        states = table_index(rulebook, "TSPTraversalStates")
        patched = states["traversal-twin-patched-cycle"]
        if (
            float(patched["TotalCost"]) != 24
            or patched["ComponentCount"] != 1
            or patched["ProperSubtourCount"] != 0
            or patched["Status"] != "CONNECTED_CYCLE"
        ):
            fail(f"loop 604 balanced exchange mismatch: {patched}")
        exchange = [
            row for row in rulebook["TSPEdgeCommitments"]["data"]
            if row["SourceKind"] == "BALANCED_EDGE_EXCHANGE"
        ]
        modalities = sorted(row["Modality"] for row in exchange)
        if modalities != ["ADDED", "ADDED", "REMOVED", "REMOVED"]:
            fail(f"loop 604 exchange commitments mismatch: {modalities}")

    if closed(605):
        bounds = evaluate_instance_lower_bounds(rulebook)
        lower = bounds["component-debt-lower-bound-twin-triangles-6"]
        if not lower.is_certified or lower.lower_bound_cost != 24:
            fail(f"loop 605 component-debt lower bound mismatch: {lower}")
        certs = evaluate_optimality_certificates(rulebook)
        twin_cert = certs["optimality-twin-triangles-feasible-24"]
        if not twin_cert.is_passing:
            fail(f"loop 605 twin optimality certificate failed: {twin_cert}")
        twin = evaluate_tours(rulebook)["tour-twin-triangles-feasible-24"]
        if not twin.is_optimality_proved:
            fail("loop 605 twin candidate did not inherit the passing optimality certificate")

    if closed(606):
        regions = table_index(rulebook, "TSPRegions")
        if len(regions) != 6:
            fail(f"loop 606 expected six traversal regions, got {len(regions)}")
        if sum(row["RegionKind"] == "NEIGHBORHOOD" for row in regions.values()) != 2:
            fail("loop 606 neighborhood regions missing")
        if sum(row["RegionKind"] == "SELECTED_COMPONENT" for row in regions.values()) != 2:
            fail("loop 606 component regions missing")

    if closed(607):
        states = table_index(rulebook, "TSPTraversalStates")
        state = states["traversal-twin-boundary-composition"]
        if float(state["TotalCost"]) != 24 or state["ComponentCount"] != 1:
            fail("loop 607 boundary composition did not emit a cost-24 connected cycle")

    if closed(608):
        searches = table_index(rulebook, "TSPSearchCertificates")
        kernel = searches["search-twin-boundary-kernel"]
        if kernel["InitialRouteClassCount"] != 60 or kernel["SurvivingRouteClassCount"] != 9:
            fail("loop 608 residual-kernel projection must contract 60 alternatives to 9")

    if closed(609):
        searches = table_index(rulebook, "TSPSearchCertificates")
        gauge = searches["search-twin-gauge-orbit"]
        if (
            gauge["InitialRouteClassCount"] != 9
            or gauge["SurvivingRouteClassCount"] != 1
            or gauge["BranchDecisionCount"] != 0
            or gauge["ResidualAmbiguityCount"] != 0
        ):
            fail("loop 609 gauge-ambiguity collapse mismatch")

    if closed(610):
        if any(loops[order]["Status"] != "CLOSED" for order in range(597, 611)):
            fail("loop 610 convergence event requires every loop 597-610 to be CLOSED")
        legacy = concept_rows(rulebook, kind="LEGACY_SURFACE")
        canonical = concept_rows(rulebook, kind="CANONICAL_PRIMITIVE")
        invented = concept_rows(rulebook, kind="INVENTED_PREDICATE")
        if len(legacy) != 32 or len(canonical) != 8 or len(invented) < 6:
            fail(
                f"loop 610 concept counts mismatch: legacy={len(legacy)} "
                f"canonical={len(canonical)} invented={len(invented)}"
            )
        if any(row["Status"] != "COLLAPSED" for row in legacy):
            fail("loop 610 left one or more legacy surface concepts uncollapsed")
        claims = contract.get("Claims", {})
        if claims.get("ModelConvergenceObserved") is not True:
            fail("loop 610 convergence observation missing")
        if claims.get("GeneralConceptConvergenceProved") is not False:
            fail("loop 610 must not promote the convergence prediction into a general theorem")
        metrics = contract.get("ConvergenceMetrics", {})
        if (
            metrics.get("LegacySurfaceConceptCount") != 32
            or metrics.get("CanonicalPrimitiveCount") != 8
            or metrics.get("ConceptCompressionPct") != 75
        ):
            fail(f"loop 610 convergence metrics mismatch: {metrics}")


def main() -> None:
    rulebook = v3.load(RULEBOOK)
    contract = v3.load(CONTRACT)
    if rulebook.get("$schema") != "https://example.com/cmcc-schema/v1":
        fail("rulebook schema mismatch")
    if contract.get("Status") != "RESEARCH_PROGRAM":
        fail("contract must remain RESEARCH_PROGRAM")
    if contract.get("Claims", {}).get("PEqualsNP") is not False:
        fail("P=NP non-claim missing")

    v3.validate_shapes(rulebook)
    v3.validate_relationships(rulebook)
    v3.validate_formulas(rulebook)

    loops = {int(row["LoopOrder"]): row for row in rulebook["TSPLoops"]["data"]}
    planned = max(loops)
    if sorted(loops) != list(range(577, planned + 1)):
        fail(f"loop sequence is not contiguous through {planned}")
    contract_loops = {int(row["LoopOrder"]): row for row in contract["Loops"]}
    if set(contract_loops) != set(loops):
        fail("contract and rulebook loop sets disagree")
    for order in range(587, planned + 1):
        row = loops[order]
        if not row.get("BeforeState") or not row.get("PlannedClosureCriterion"):
            fail(f"loop {order} lacks recorded before-state or closure criterion")
        if terminal(row) and not row.get("AfterState"):
            fail(f"loop {order} is terminal without an after-state")
        if contract_loops[order]["Status"] != row["Status"]:
            fail(f"loop {order} contract/rulebook status disagreement")

    validate_baseline(rulebook, loops)
    validate_convergence(rulebook, contract, loops)

    statuses = ", ".join(
        f"{order}:{loops[order]['Status']}" for order in sorted(loops) if order >= 597
    )
    print("traveling-salesman rulebook v4 validation: PASS")
    print(f"tables: {len(v3.tables(rulebook))}")
    print(f"loops 597-{planned}: {statuses}")


if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print(f"traveling-salesman v4 validation: FAIL: {exc}", file=sys.stderr)
        raise
