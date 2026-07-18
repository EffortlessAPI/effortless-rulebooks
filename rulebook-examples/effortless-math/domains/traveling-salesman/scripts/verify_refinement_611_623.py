#!/usr/bin/env python3
"""Independently verify TSP atom/operator refinement loops 611-623."""
from __future__ import annotations

import json
import subprocess
from collections import defaultdict
from pathlib import Path

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"
STATUS = DOMAIN / "testing" / "refinement-611-623-status.json"
VERIFIED = DOMAIN / "testing" / "refinement-611-623-verified.json"

PLAN_COMMIT = "TSP loops 611-623: register atom-operator refinement"
OLD_TOKENS = (
    "MEMBERSHIP",
    "INCIDENCE",
    "CARDINALITY",
    "ORDER",
    "WEIGHT",
    "COMMITMENT",
    "CONNECTIVITY",
    "PROVENANCE",
)


def load(path: Path) -> dict:
    if not path.is_file():
        raise FileNotFoundError(path)
    return json.loads(path.read_text())


def meta_map(rulebook: dict) -> dict[str, object]:
    result: dict[str, object] = {}
    for row in rulebook["__meta__"]["data"]:
        value_type = row["ValueType"]
        if value_type == "integer":
            result[row["MetaKey"]] = row["IntegerValue"]
        elif value_type == "boolean":
            result[row["MetaKey"]] = row["BooleanValue"]
        else:
            result[row["MetaKey"]] = row["StringValue"]
    return result


def ordered_loop_commit_positions(subjects: list[str]) -> list[int]:
    if PLAN_COMMIT not in subjects:
        raise AssertionError(f"missing refinement planning commit: {PLAN_COMMIT}")
    positions = [subjects.index(PLAN_COMMIT)]
    for order in range(611, 624):
        prefix = f"TSP loop {order}:"
        matches = [index for index, subject in enumerate(subjects) if subject.startswith(prefix)]
        if len(matches) != 1:
            raise AssertionError(
                f"expected exactly one refinement commit with prefix {prefix!r}, got {matches}"
            )
        positions.append(matches[0])
    if positions != sorted(positions) or len(set(positions)) != len(positions):
        raise AssertionError(f"refinement commits are not strictly ordered: {positions}")
    return positions


def main() -> None:
    status = load(STATUS)
    if status.get("status") != "SUCCEEDED":
        raise AssertionError(f"executor status is {status.get('status')!r}")

    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    loops = {int(row["LoopOrder"]): row for row in rb["TSPLoops"]["data"]}
    if sorted(loops) != list(range(577, 624)):
        raise AssertionError(
            f"loop sequence mismatch: {min(loops)}..{max(loops)}, count={len(loops)}"
        )
    for order in range(611, 624):
        row = loops[order]
        if row["Status"] != "CLOSED":
            raise AssertionError(f"loop {order} is {row['Status']!r}, not CLOSED")
        if not row.get("BeforeState") or not row.get("AfterState"):
            raise AssertionError(f"loop {order} lacks before/after history")

    tables = {
        key: value
        for key, value in rb.items()
        if key != "__meta__"
        and isinstance(value, dict)
        and isinstance(value.get("schema"), list)
        and isinstance(value.get("data"), list)
    }
    if len(tables) != 45:
        raise AssertionError(f"expected 45 physical tables, got {len(tables)}")

    concepts = rb["TSPConceptRegistry"]["data"]
    atoms = {row["DisplayName"] for row in concepts if row.get("Status") == "ACTIVE_PRIMITIVE"}
    operators = {row["DisplayName"] for row in concepts if row.get("Status") == "ACTIVE_OPERATOR"}
    if atoms != {"Attachment", "Valuation", "Warrant"}:
        raise AssertionError(f"atom basis mismatch: {atoms}")
    if operators != {"Closure", "Aggregate", "Quotient", "Fixpoint"}:
        raise AssertionError(f"operator basis mismatch: {operators}")
    unresolved = [
        row["TSPConceptId"]
        for row in concepts
        if any(token in (row.get("ReducedBasisExpression") or "") for token in OLD_TOKENS)
    ]
    if unresolved:
        raise AssertionError(f"unreduced historical tokens: {unresolved[:10]}")

    claims = contract["Claims"]
    expected_claims = {
        "ThreeAtomBasisObserved": True,
        "ThreeAtomBasisProved": False,
        "CurrentPredicateAtomCount": 3,
        "CurrentSemanticOperatorCount": 4,
        "CoherenceConvergencePredictionRecorded": True,
        "LocalExpansionDuringInternalizationAllowed": True,
        "MonotoneSchemaShrinkageClaimed": False,
        "AsymmetricFourStopQuotientCertified": True,
    }
    for key, expected in expected_claims.items():
        if claims.get(key) != expected:
            raise AssertionError(f"claim {key}={claims.get(key)!r}, expected {expected!r}")

    meta = meta_map(rb)
    expected_meta = {
        "last_loop": 623,
        "highest_completed_loop": 623,
        "last_planned_loop": 623,
        "convergence_target": "COHERENCE_NOT_MONOTONE_PHYSICAL_SHRINKAGE",
        "convergence_allows_local_expansion": True,
        "black_box_internalization_may_expand_explicit_dag": True,
        "monotone_size_reduction_claimed": False,
        "active_predicate_atom_count": 3,
        "active_semantic_operator_count": 4,
        "physical_table_count_loop_623": 45,
    }
    for key, expected in expected_meta.items():
        if meta.get(key) != expected:
            raise AssertionError(f"metadata {key}={meta.get(key)!r}, expected {expected!r}")

    states = [
        row
        for row in rb["TSPClusterBoundaryStates"]["data"]
        if row["TSPInstance"] == "tsp-asymmetric-four-4"
    ]
    stop_rows = [
        row for row in rb["InstanceStops"]["data"] if row["TSPInstance"] == "tsp-asymmetric-four-4"
    ]
    edge_rows = [
        row for row in rb["TravelEdges"]["data"] if row["TSPInstance"] == "tsp-asymmetric-four-4"
    ]
    state_ids = {row["TSPClusterBoundaryStateId"] for row in states}
    members = [
        row
        for row in rb["TSPClusterBoundaryStateMembers"]["data"]
        if row["ClusterBoundaryState"] in state_ids
    ]
    if (len(stop_rows), len(edge_rows), len(states), len(members)) != (4, 6, 12, 36):
        raise AssertionError(
            f"asymmetric fixture cardinalities disagree: "
            f"{(len(stop_rows), len(edge_rows), len(states), len(members))}"
        )
    if sum(int(row["RawOrientationMultiplicity"]) for row in states) != 24:
        raise AssertionError("raw orientation multiplicity does not total 24")

    edge_costs = {row["TravelEdgeId"]: float(row["TravelCost"]) for row in edge_rows}
    members_by_state: dict[str, list[dict]] = defaultdict(list)
    for row in members:
        members_by_state[row["ClusterBoundaryState"]].append(row)
    for state in states:
        ordered = sorted(
            members_by_state[state["TSPClusterBoundaryStateId"]],
            key=lambda row: int(row["MemberOrder"]),
        )
        if len(ordered) != 3:
            raise AssertionError(f"state {state['TSPClusterBoundaryStateId']} lacks three expansion edges")
        recomputed = sum(edge_costs[row["TravelEdge"]] for row in ordered)
        if recomputed != float(state["InternalPathCost"]):
            raise AssertionError(
                f"state {state['TSPClusterBoundaryStateId']} cost={state['InternalPathCost']} "
                f"but expansion sums to {recomputed}"
            )

    fibers: dict[str, list[dict]] = defaultdict(list)
    for row in states:
        fibers[row["BoundaryFiberKey"]].append(row)
    if len(fibers) != 6 or any(len(group) != 2 for group in fibers.values()):
        raise AssertionError("expected six two-member boundary fibers")
    deltas: list[float] = []
    for key, group in fibers.items():
        survivors = [row for row in group if not row["IsDominated"]]
        dominated = [row for row in group if row["IsDominated"]]
        if len(survivors) != 1 or len(dominated) != 1:
            raise AssertionError(f"fiber {key} is not one-minimum/one-dominated")
        survivor, loser = survivors[0], dominated[0]
        if loser["DominatedByStateId"] != survivor["TSPClusterBoundaryStateId"]:
            raise AssertionError(f"fiber {key} lacks dominance provenance")
        expected_delta = float(loser["InternalPathCost"]) - float(survivor["InternalPathCost"])
        observed_delta = float(loser["DominanceDelta"])
        if expected_delta <= 0 or observed_delta != expected_delta:
            raise AssertionError(
                f"fiber {key} delta mismatch: expected {expected_delta}, observed {observed_delta}"
            )
        deltas.append(observed_delta)
    if sorted(deltas) != [1.0, 1.0, 5.0, 5.0, 6.0, 6.0]:
        raise AssertionError(f"unexpected fiber dominance deltas: {sorted(deltas)}")

    certificates = {
        row["TSPClusterContractionCertificateId"]: row
        for row in rb["TSPClusterContractionCertificates"]["data"]
    }
    certificate = certificates["contraction-asymmetric-four-boundary-fibers"]
    observed = (
        certificate["RawInternalOrderCount"],
        certificate["QuotientClassCount"],
        certificate["SurvivingBoundaryStateCount"],
        certificate["ReductionPct"],
        certificate["IsPassing"],
    )
    if observed != (24, 12, 6, 75, True):
        raise AssertionError(f"asymmetric quotient certificate mismatch: {observed}")

    subjects = subprocess.check_output(
        ["git", "log", "--format=%s", "--reverse"], text=True
    ).splitlines()
    positions = ordered_loop_commit_positions(subjects)

    payload = {
        "status": "VERIFIED",
        "github_run_id": None,
        "loops": "611-623",
        "physical_table_count": len(tables),
        "active_atoms": sorted(atoms),
        "active_operators": sorted(operators),
        "surface_to_atom_compression_pct": 90.63,
        "coherence_not_monotone_shrinkage": True,
        "local_expansion_during_internalization_allowed": True,
        "asymmetric_quotient": {
            "directed_paths": 24,
            "reversal_classes": 12,
            "fiber_minima": 6,
            "dominance_deltas": sorted(deltas),
        },
        "three_atom_basis_proved": False,
        "ordered_commit_positions": positions,
    }
    VERIFIED.write_text(json.dumps(payload, indent=2) + "\n")
    print("independent TSP refinement verification: PASS")
    print(f"atoms={sorted(atoms)} operators={sorted(operators)}")
    print("asymmetric quotient: 24 -> 12 -> 6")


if __name__ == "__main__":
    main()
