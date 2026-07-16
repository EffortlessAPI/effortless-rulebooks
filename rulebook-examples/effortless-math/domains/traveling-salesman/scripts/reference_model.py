#!/usr/bin/env python3
"""Independent Python execution substrate for the initial TSP rulebook.

The rulebook is canonical. This module consumes only raw rulebook rows and
recomputes the initial normalization and supplied-tour certificate so Postgres
can be checked as a peer projection.
"""
from __future__ import annotations

import json
from dataclasses import dataclass, asdict
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


def evaluate_graph(rulebook: dict[str, Any]) -> dict[str, dict[str, Any]]:
    stops = _rows(rulebook, "InstanceStops")
    edges = _rows(rulebook, "TravelEdges")
    by_instance: dict[str, dict[str, Any]] = {}

    for instance in _rows(rulebook, "TSPInstances"):
        iid = instance["TSPInstanceId"]
        instance_stops = [s for s in stops if s["TSPInstance"] == iid]
        required = [s for s in instance_stops if s["IsRequired"]]
        instance_edges = [e for e in edges if e["TSPInstance"] == iid]
        pairs = [frozenset((e["FromStop"], e["ToStop"])) for e in instance_edges]
        admissible = all(
            e["IsAvailable"]
            and e["FromStop"] != e["ToStop"]
            and any(s["InstanceStopId"] == e["FromStop"] and s["TSPInstance"] == iid for s in instance_stops)
            and any(s["InstanceStopId"] == e["ToStop"] and s["TSPInstance"] == iid for s in instance_stops)
            for e in instance_edges
        )
        expected = len(instance_stops) * (len(instance_stops) - 1) // 2
        by_instance[iid] = {
            "count_of_stops": len(instance_stops),
            "count_of_required_stops": len(required),
            "count_of_travel_edges": len(instance_edges),
            "expected_undirected_edge_count": expected,
            "is_complete_undirected_graph": (
                admissible
                and len(instance_edges) == expected
                and len(set(pairs)) == len(pairs)
                and all(len(p) == 2 for p in pairs)
            ),
        }
    return by_instance


def evaluate_tours(rulebook: dict[str, Any]) -> dict[str, TourResult]:
    graph = evaluate_graph(rulebook)
    instance_stops = {r["InstanceStopId"]: r for r in _rows(rulebook, "InstanceStops")}
    edges = {r["TravelEdgeId"]: r for r in _rows(rulebook, "TravelEdges")}
    tour_stops = _rows(rulebook, "TourStops")
    tour_legs = _rows(rulebook, "TourLegs")
    out: dict[str, TourResult] = {}

    for tour in _rows(rulebook, "CandidateTours"):
        tid = tour["CandidateTourId"]
        iid = tour["TSPInstance"]
        required_stop_ids = {
            s["InstanceStopId"]
            for s in instance_stops.values()
            if s["TSPInstance"] == iid and s["IsRequired"]
        }
        ordered = sorted(
            (s for s in tour_stops if s["CandidateTour"] == tid),
            key=lambda r: r["SequencePosition"],
        )
        legs = sorted(
            (l for l in tour_legs if l["CandidateTour"] == tid),
            key=lambda r: r["LegOrder"],
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
        depot_stop_ids = {
            s["InstanceStopId"]
            for s in instance_stops.values()
            if s["TSPInstance"] == iid
            and s["Address"] == next(i["DepotAddress"] for i in _rows(rulebook, "TSPInstances") if i["TSPInstanceId"] == iid)
        }
        depot_visits = [s for s in ordered if s["InstanceStop"] in depot_stop_ids]
        depot_ok = len(depot_visits) == 1 and depot_visits[0]["SequencePosition"] == 1

        stops_by_id = {s["TourStopId"]: s for s in ordered}
        total_cost = 0.0
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
            endpoint_match = {
                from_stop,
                to_stop,
            } == {edge["FromStop"], edge["ToStop"]}
            edge_valid = (
                edge["TSPInstance"] == iid
                and edge["IsAvailable"]
                and edge["FromStop"] != edge["ToStop"]
            )
            leg_valid = candidate_membership and sequence_transition and endpoint_match and edge_valid
            legs_valid = legs_valid and leg_valid
            if leg_valid:
                total_cost += float(edge["TravelCost"])

        valid = (
            graph[iid]["is_complete_undirected_graph"]
            and len(ordered) == len(required_stop_ids)
            and len(legs) == len(required_stop_ids)
            and unique_visits
            and unique_positions
            and all_in_instance
            and all_required
            and set(stop_ids) == required_stop_ids
            and depot_ok
            and legs_valid
        )
        out[tid] = TourResult(
            candidate_tour_id=tid,
            required_stop_count=len(required_stop_ids),
            tour_stop_count=len(ordered),
            tour_leg_count=len(legs),
            total_travel_cost=total_cost,
            is_hamiltonian_cycle_witness=valid,
            is_optimality_proved=False,
            residual_claim=(
                "VALID_TOUR_NOT_OPTIMALITY_PROOF" if valid else "INVALID_TOUR"
            ),
        )
    return out


def as_json(path: Path | str = DEFAULT_RULEBOOK) -> str:
    rb = load_rulebook(path)
    payload = {
        "graph": evaluate_graph(rb),
        "tours": {k: asdict(v) for k, v in evaluate_tours(rb).items()},
    }
    return json.dumps(payload, indent=2, sort_keys=True)


if __name__ == "__main__":
    print(as_json())
