#!/usr/bin/env python3
"""Independent Python execution substrate for the TSP rulebook.

The rulebook is canonical. This module consumes only raw rulebook rows and
independently evaluates graph normalization, supplied-cycle validity, local
lower-bound witnesses, finite optimality certificates, and search accounting so
Postgres can be checked as a peer projection.
"""
from __future__ import annotations

import json
from collections import Counter, defaultdict
from dataclasses import asdict, dataclass
from decimal import Decimal
from pathlib import Path
from typing import Any

HERE = Path(__file__).resolve().parent
DEFAULT_RULEBOOK = HERE.parent / "effortless-rulebook" / "traveling-salesman-rulebook.json"


@dataclass(frozen=True)
class TourResult:
    candidate_tour_id: str
    required_stop_count: int
    tour_stop_count: int
    tour_leg_count: int
    total_travel_cost: float
    is_hamiltonian_cycle_witness: bool
    is_optimality_proved: bool
    residual_claim: str


@dataclass(frozen=True)
class LocalDegreeBoundResult:
    local_degree_bound_id: str
    tsp_instance_id: str
    instance_stop_id: str
    required_dominance_check_count: int
    count_of_dominance_checks: int
    count_of_failed_dominance_checks: int
    is_two_cheapest_witness: bool
    local_bound_cost: float


@dataclass(frozen=True)
class InstanceLowerBoundResult:
    instance_lower_bound_id: str
    tsp_instance_id: str
    count_of_local_degree_bounds: int
    count_of_invalid_local_degree_bounds: int
    total_local_degree_bound_cost: float
    lower_bound_cost: float
    is_certified: bool


@dataclass(frozen=True)
class OptimalityCertificateResult:
    optimality_certificate_id: str
    candidate_tour_id: str
    instance_lower_bound_id: str
    candidate_travel_cost: float
    lower_bound_cost: float
    is_same_instance: bool
    is_bound_tight: bool
    is_passing: bool
    scope_claim: str


def _rows(rulebook: dict[str, Any], table: str) -> list[dict[str, Any]]:
    try:
        return rulebook[table]["data"]
    except KeyError as exc:
        raise KeyError(f"missing canonical table {table}") from exc


def load_rulebook(path: Path | str = DEFAULT_RULEBOOK) -> dict[str, Any]:
    p = Path(path)
    if not p.is_file():
        raise FileNotFoundError(f"missing canonical rulebook: {p}")
    return json.loads(p.read_text())


def _expected_pair_key(from_stop: str, to_stop: str) -> str:
    return "|".join(sorted((from_stop, to_stop)))


def _edge_state(rulebook: dict[str, Any]) -> dict[str, dict[str, Any]]:
    stops = {row["InstanceStopId"]: row for row in _rows(rulebook, "InstanceStops")}
    edges = _rows(rulebook, "TravelEdges")
    pair_counts = Counter((row["TSPInstance"], row["CanonicalPairKey"]) for row in edges)
    out: dict[str, dict[str, Any]] = {}
    for edge in edges:
        iid = edge["TSPInstance"]
        from_stop = edge["FromStop"]
        to_stop = edge["ToStop"]
        endpoints_exist = from_stop in stops and to_stop in stops
        within_instance = (
            endpoints_exist
            and stops[from_stop]["TSPInstance"] == iid
            and stops[to_stop]["TSPInstance"] == iid
        )
        expected_key = _expected_pair_key(from_stop, to_stop)
        multiplicity = pair_counts[(iid, edge["CanonicalPairKey"])]
        admissible = (
            bool(edge["IsAvailable"])
            and from_stop != to_stop
            and within_instance
            and from_stop < to_stop
            and edge["CanonicalPairKey"] == expected_key
            and multiplicity == 1
        )
        out[edge["TravelEdgeId"]] = {
            "expected_pair_key": expected_key,
            "pair_multiplicity": multiplicity,
            "is_unique_canonical_pair": multiplicity == 1,
            "is_admissible": admissible,
        }
    return out


def evaluate_graph(rulebook: dict[str, Any]) -> dict[str, dict[str, Any]]:
    stops = _rows(rulebook, "InstanceStops")
    edges = _rows(rulebook, "TravelEdges")
    edge_state = _edge_state(rulebook)
    by_instance: dict[str, dict[str, Any]] = {}

    for instance in _rows(rulebook, "TSPInstances"):
        iid = instance["TSPInstanceId"]
        instance_stops = [s for s in stops if s["TSPInstance"] == iid]
        required = [s for s in instance_stops if s["IsRequired"]]
        instance_edges = [e for e in edges if e["TSPInstance"] == iid]
        expected = len(instance_stops) * (len(instance_stops) - 1) // 2
        inadmissible = sum(
            not edge_state[e["TravelEdgeId"]]["is_admissible"] for e in instance_edges
        )
        non_unique = sum(
            not edge_state[e["TravelEdgeId"]]["is_unique_canonical_pair"]
            for e in instance_edges
        )
        by_instance[iid] = {
            "count_of_stops": len(instance_stops),
            "count_of_required_stops": len(required),
            "count_of_travel_edges": len(instance_edges),
            "count_of_inadmissible_edges": inadmissible,
            "count_of_non_unique_edge_pair_rows": non_unique,
            "expected_undirected_edge_count": expected,
            "is_complete_undirected_graph": (
                len(instance_edges) == expected and inadmissible == 0 and non_unique == 0
            ),
        }
    return by_instance


def _evaluate_tour_bases(rulebook: dict[str, Any]) -> dict[str, dict[str, Any]]:
    graph = evaluate_graph(rulebook)
    edge_state = _edge_state(rulebook)
    instances = {r["TSPInstanceId"]: r for r in _rows(rulebook, "TSPInstances")}
    instance_stops = {r["InstanceStopId"]: r for r in _rows(rulebook, "InstanceStops")}
    edges = {r["TravelEdgeId"]: r for r in _rows(rulebook, "TravelEdges")}
    tour_stops = _rows(rulebook, "TourStops")
    tour_legs = _rows(rulebook, "TourLegs")
    out: dict[str, dict[str, Any]] = {}

    for tour in _rows(rulebook, "CandidateTours"):
        tid = tour["CandidateTourId"]
        iid = tour["TSPInstance"]
        if iid not in graph or iid not in instances:
            raise KeyError(f"candidate {tid} references unknown instance {iid}")
        required_stop_ids = {
            s["InstanceStopId"]
            for s in instance_stops.values()
            if s["TSPInstance"] == iid and s["IsRequired"]
        }
        ordered = sorted(
            (s for s in tour_stops if s["CandidateTour"] == tid),
            key=lambda row: row["SequencePosition"],
        )
        legs = sorted(
            (l for l in tour_legs if l["CandidateTour"] == tid),
            key=lambda row: row["LegOrder"],
        )

        stop_ids = [s["InstanceStop"] for s in ordered]
        positions = [s["SequencePosition"] for s in ordered]
        unique_visits = len(stop_ids) == len(set(stop_ids))
        unique_positions = len(positions) == len(set(positions))
        all_in_instance = all(
            sid in instance_stops and instance_stops[sid]["TSPInstance"] == iid
            for sid in stop_ids
        )
        all_required = all(
            sid in instance_stops and instance_stops[sid]["IsRequired"]
            for sid in stop_ids
        )
        depot_address = instances[iid]["DepotAddress"]
        depot_stop_ids = {
            s["InstanceStopId"]
            for s in instance_stops.values()
            if s["TSPInstance"] == iid and s["Address"] == depot_address
        }
        depot_visits = [s for s in ordered if s["InstanceStop"] in depot_stop_ids]
        depot_ok = len(depot_visits) == 1 and depot_visits[0]["SequencePosition"] == 1

        stops_by_id = {s["TourStopId"]: s for s in ordered}
        leg_order_counts = Counter(leg["LegOrder"] for leg in legs)
        outgoing_counts = Counter(leg["FromTourStop"] for leg in legs)
        incoming_counts = Counter(leg["ToTourStop"] for leg in legs)
        cycle_degree_ok = all(
            outgoing_counts[row["TourStopId"]] == 1
            and incoming_counts[row["TourStopId"]] == 1
            for row in ordered
        )

        total_cost = Decimal("0")
        legs_valid = True
        n = graph[iid]["count_of_required_stops"]
        for leg in legs:
            from_row = stops_by_id.get(leg["FromTourStop"])
            to_row = stops_by_id.get(leg["ToTourStop"])
            edge = edges.get(leg["TravelEdge"])
            if from_row is None or to_row is None or edge is None:
                legs_valid = False
                continue
            from_stop = from_row["InstanceStop"]
            to_stop = to_row["InstanceStop"]
            candidate_membership = (
                from_row["CandidateTour"] == tid and to_row["CandidateTour"] == tid
            )
            sequence_transition = (
                to_row["SequencePosition"] == from_row["SequencePosition"] + 1
                or (
                    from_row["SequencePosition"] == n
                    and to_row["SequencePosition"] == 1
                )
            )
            endpoint_match = {from_stop, to_stop} == {edge["FromStop"], edge["ToStop"]}
            edge_valid = (
                edge["TSPInstance"] == iid
                and edge_state[edge["TravelEdgeId"]]["is_admissible"]
            )
            unique_leg_order = leg_order_counts[leg["LegOrder"]] == 1
            leg_valid = (
                candidate_membership
                and sequence_transition
                and endpoint_match
                and edge_valid
                and unique_leg_order
            )
            legs_valid = legs_valid and leg_valid
            if leg_valid:
                total_cost += Decimal(str(edge["TravelCost"]))

        valid = (
            len(ordered) == len(required_stop_ids)
            and len(legs) == len(required_stop_ids)
            and unique_visits
            and unique_positions
            and all_in_instance
            and all_required
            and set(stop_ids) == required_stop_ids
            and depot_ok
            and legs_valid
            and cycle_degree_ok
        )
        out[tid] = {
            "candidate_tour_id": tid,
            "tsp_instance_id": iid,
            "required_stop_count": len(required_stop_ids),
            "tour_stop_count": len(ordered),
            "tour_leg_count": len(legs),
            "total_travel_cost": total_cost,
            "is_hamiltonian_cycle_witness": valid,
        }
    return out


def evaluate_local_degree_bounds(
    rulebook: dict[str, Any],
) -> dict[str, LocalDegreeBoundResult]:
    edge_state = _edge_state(rulebook)
    stops = {r["InstanceStopId"]: r for r in _rows(rulebook, "InstanceStops")}
    edges = {r["TravelEdgeId"]: r for r in _rows(rulebook, "TravelEdges")}
    checks_by_bound: dict[str, list[dict[str, Any]]] = defaultdict(list)
    for check in _rows(rulebook, "IncidentDominanceChecks"):
        checks_by_bound[check["LocalDegreeBound"]].append(check)

    out: dict[str, LocalDegreeBoundResult] = {}
    for bound in _rows(rulebook, "LocalDegreeBounds"):
        bid = bound["LocalDegreeBoundId"]
        iid = bound["TSPInstance"]
        stop_id = bound["InstanceStop"]
        if stop_id not in stops:
            raise KeyError(f"local bound {bid} references unknown stop {stop_id}")
        first = edges.get(bound["FirstEdge"])
        second = edges.get(bound["SecondEdge"])
        if first is None or second is None:
            raise KeyError(f"local bound {bid} references unknown selected edge")

        incident_admissible = [
            edge
            for edge in edges.values()
            if edge["TSPInstance"] == iid
            and edge_state[edge["TravelEdgeId"]]["is_admissible"]
            and stop_id in {edge["FromStop"], edge["ToStop"]}
        ]
        required_checks = len(incident_admissible) - 2
        checks = checks_by_bound.get(bid, [])
        other_counts = Counter(check["OtherEdge"] for check in checks)
        failed = 0
        for check in checks:
            other = edges.get(check["OtherEdge"])
            passing = (
                other is not None
                and other["TSPInstance"] == iid
                and edge_state[other["TravelEdgeId"]]["is_admissible"]
                and stop_id in {other["FromStop"], other["ToStop"]}
                and other["TravelEdgeId"] not in {first["TravelEdgeId"], second["TravelEdgeId"]}
                and other_counts[other["TravelEdgeId"]] == 1
                and Decimal(str(other["TravelCost"]))
                >= Decimal(str(second["TravelCost"]))
            )
            failed += not passing

        selected_ok = (
            stops[stop_id]["TSPInstance"] == iid
            and first["TSPInstance"] == iid
            and second["TSPInstance"] == iid
            and edge_state[first["TravelEdgeId"]]["is_admissible"]
            and edge_state[second["TravelEdgeId"]]["is_admissible"]
            and stop_id in {first["FromStop"], first["ToStop"]}
            and stop_id in {second["FromStop"], second["ToStop"]}
            and first["TravelEdgeId"] != second["TravelEdgeId"]
            and Decimal(str(first["TravelCost"])) <= Decimal(str(second["TravelCost"]))
        )
        valid = selected_ok and len(checks) == required_checks and failed == 0
        local_cost = (
            Decimal(str(first["TravelCost"])) + Decimal(str(second["TravelCost"]))
            if valid
            else Decimal("0")
        )
        out[bid] = LocalDegreeBoundResult(
            local_degree_bound_id=bid,
            tsp_instance_id=iid,
            instance_stop_id=stop_id,
            required_dominance_check_count=required_checks,
            count_of_dominance_checks=len(checks),
            count_of_failed_dominance_checks=failed,
            is_two_cheapest_witness=valid,
            local_bound_cost=float(local_cost),
        )
    return out


def evaluate_instance_lower_bounds(
    rulebook: dict[str, Any],
) -> dict[str, InstanceLowerBoundResult]:
    graph = evaluate_graph(rulebook)
    local = evaluate_local_degree_bounds(rulebook)
    out: dict[str, InstanceLowerBoundResult] = {}
    for row in _rows(rulebook, "InstanceLowerBounds"):
        lower_id = row["InstanceLowerBoundId"]
        iid = row["TSPInstance"]
        bounds = [item for item in local.values() if item.tsp_instance_id == iid]
        invalid = sum(not item.is_two_cheapest_witness for item in bounds)
        total = sum((Decimal(str(item.local_bound_cost)) for item in bounds), Decimal("0"))
        lower = total / Decimal("2")
        certified = (
            iid in graph
            and len(bounds) == graph[iid]["count_of_required_stops"]
            and invalid == 0
        )
        out[lower_id] = InstanceLowerBoundResult(
            instance_lower_bound_id=lower_id,
            tsp_instance_id=iid,
            count_of_local_degree_bounds=len(bounds),
            count_of_invalid_local_degree_bounds=invalid,
            total_local_degree_bound_cost=float(total),
            lower_bound_cost=float(lower),
            is_certified=certified,
        )
    return out


def evaluate_optimality_certificates(
    rulebook: dict[str, Any],
) -> dict[str, OptimalityCertificateResult]:
    tour_bases = _evaluate_tour_bases(rulebook)
    lower_bounds = evaluate_instance_lower_bounds(rulebook)
    out: dict[str, OptimalityCertificateResult] = {}
    for row in _rows(rulebook, "OptimalityCertificates"):
        cid = row["OptimalityCertificateId"]
        candidate_id = row["CandidateTour"]
        lower_id = row["InstanceLowerBound"]
        candidate = tour_bases.get(candidate_id)
        lower = lower_bounds.get(lower_id)
        if candidate is None or lower is None:
            raise KeyError(f"optimality certificate {cid} references unknown child")
        candidate_cost = candidate["total_travel_cost"]
        lower_cost = Decimal(str(lower.lower_bound_cost))
        same_instance = candidate["tsp_instance_id"] == lower.tsp_instance_id
        bound_tight = candidate_cost == lower_cost
        passing = (
            candidate["is_hamiltonian_cycle_witness"]
            and lower.is_certified
            and same_instance
            and bound_tight
        )
        out[cid] = OptimalityCertificateResult(
            optimality_certificate_id=cid,
            candidate_tour_id=candidate_id,
            instance_lower_bound_id=lower_id,
            candidate_travel_cost=float(candidate_cost),
            lower_bound_cost=float(lower_cost),
            is_same_instance=same_instance,
            is_bound_tight=bound_tight,
            is_passing=passing,
            scope_claim=(
                "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE"
                if passing
                else "OPTIMALITY_NOT_CERTIFIED"
            ),
        )
    return out


def evaluate_tours(rulebook: dict[str, Any]) -> dict[str, TourResult]:
    bases = _evaluate_tour_bases(rulebook)
    certificates = evaluate_optimality_certificates(rulebook)
    passing_by_candidate = Counter(
        cert.candidate_tour_id for cert in certificates.values() if cert.is_passing
    )
    out: dict[str, TourResult] = {}
    for tid, base in bases.items():
        optimal = base["is_hamiltonian_cycle_witness"] and passing_by_candidate[tid] > 0
        if optimal:
            claim = "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE"
        elif base["is_hamiltonian_cycle_witness"]:
            claim = "VALID_TOUR_NOT_OPTIMALITY_PROOF"
        else:
            claim = "INVALID_TOUR"
        out[tid] = TourResult(
            candidate_tour_id=tid,
            required_stop_count=base["required_stop_count"],
            tour_stop_count=base["tour_stop_count"],
            tour_leg_count=base["tour_leg_count"],
            total_travel_cost=float(base["total_travel_cost"]),
            is_hamiltonian_cycle_witness=base["is_hamiltonian_cycle_witness"],
            is_optimality_proved=optimal,
            residual_claim=claim,
        )
    return out


def evaluate_search_metrics(rulebook: dict[str, Any]) -> dict[str, dict[str, Any]]:
    out: dict[str, dict[str, Any]] = {}
    for row in _rows(rulebook, "SearchMetrics"):
        before = int(row["BranchCountBefore"])
        after = int(row["BranchCountAfter"])
        pct = 0.0 if before == 0 else round((before - after) / before * 100, 2)
        out[row["SearchMetricId"]] = {
            "search_question": row["SearchQuestion"],
            "branch_count_before": before,
            "branch_count_after": after,
            "search_elimination_pct": pct,
            "residual_ambiguity_count": int(row["ResidualAmbiguityCount"]),
        }
    return out


def as_json(path: Path | str = DEFAULT_RULEBOOK) -> str:
    rb = load_rulebook(path)
    payload = {
        "graph": evaluate_graph(rb),
        "tours": {key: asdict(value) for key, value in evaluate_tours(rb).items()},
        "local_degree_bounds": {
            key: asdict(value) for key, value in evaluate_local_degree_bounds(rb).items()
        },
        "instance_lower_bounds": {
            key: asdict(value) for key, value in evaluate_instance_lower_bounds(rb).items()
        },
        "optimality_certificates": {
            key: asdict(value)
            for key, value in evaluate_optimality_certificates(rb).items()
        },
        "search_metrics": evaluate_search_metrics(rb),
    }
    return json.dumps(payload, indent=2, sort_keys=True)


if __name__ == "__main__":
    print(as_json())


# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_FINAL
# The public result type is constructed by declared field names, so the
# composed lower bound is independent of historical dataclass field order.
def evaluate_instance_lower_bounds(rulebook):
    graph = evaluate_graph(rulebook)
    local = evaluate_local_degree_bounds(rulebook)
    result = {}
    terms_table = rulebook.get("TSPBoundTerms", {}).get("data", [])
    for row in _rows(rulebook, "InstanceLowerBounds"):
        lower_id = row["InstanceLowerBoundId"]
        iid = row["TSPInstance"]
        bounds = [item for item in local.values() if item.tsp_instance_id == iid]
        invalid = sum(not item.is_two_cheapest_witness for item in bounds)
        total = sum((Decimal(str(item.local_bound_cost)) for item in bounds), Decimal("0"))
        base_lower = total / Decimal("2")
        required_terms = int(row.get("RequiredSupplementalTermCount", 0))
        supplemental_terms = [
            term for term in terms_table
            if term.get("BoundCertificate") == lower_id
            and term.get("CountsTowardAdjustment") is True
        ]
        certified_terms = [term for term in supplemental_terms if term.get("IsCertified") is True]
        witnessed_adjustment = sum(
            (
                Decimal(str(term["Quantity"]))
                * Decimal(str(term["UnitWeight"]))
                * Decimal(str(term["Sign"]))
                for term in certified_terms
            ),
            Decimal("0"),
        )
        declared_adjustment = Decimal(str(row.get("SupplementalBoundAdjustment", witnessed_adjustment)))
        supplemental_certified = (
            len(supplemental_terms) == required_terms
            and len(certified_terms) == required_terms
            and declared_adjustment == witnessed_adjustment
        )
        lower = base_lower + declared_adjustment
        certified = (
            iid in graph
            and len(bounds) == graph[iid]["count_of_required_stops"]
            and invalid == 0
            and supplemental_certified
        )
        available = {
            "instance_lower_bound_id": lower_id,
            "lower_bound_id": lower_id,
            "tsp_instance": iid,
            "tsp_instance_id": iid,
            "count_of_local_degree_bounds": len(bounds),
            "count_of_invalid_local_degree_bounds": invalid,
            "total_local_degree_bound_cost": float(total),
            "base_lower_bound_cost": float(base_lower),
            "supplemental_bound_adjustment": float(declared_adjustment),
            "lower_bound_cost": float(lower),
            "is_certified": certified,
        }
        fields = getattr(InstanceLowerBoundResult, "__dataclass_fields__", {})
        if fields:
            missing = [name for name in fields if name not in available]
            if missing:
                raise AssertionError(f"unmapped InstanceLowerBoundResult fields: {missing}")
            result[lower_id] = InstanceLowerBoundResult(
                **{name: available[name] for name in fields}
            )
        else:
            result[lower_id] = InstanceLowerBoundResult(
                lower_id, iid, len(bounds), invalid, float(total), float(lower), certified
            )
    return result
