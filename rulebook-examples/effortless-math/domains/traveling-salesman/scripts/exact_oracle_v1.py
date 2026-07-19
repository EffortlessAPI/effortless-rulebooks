#!/usr/bin/env python3
"""Independent exact small-instance TSP oracle used only for calibration.

The oracle fixes the declared depot, quotients route reversal, enumerates every
remaining route class, and reports exact value/choice data.  It does not promote
an oracle result into a structural proof; the rulebook records that distinction.
"""
from __future__ import annotations

import hashlib
import itertools
import json
from collections import defaultdict, deque
from math import factorial
from typing import Any, Iterable


def rows(rb: dict[str, Any], name: str) -> list[dict[str, Any]]:
    return rb[name]["data"]


def index(rb: dict[str, Any], name: str) -> dict[str, dict[str, Any]]:
    ident = rb[name]["schema"][0]["name"]
    return {row[ident]: row for row in rows(rb, name)}


def pair(a: str, b: str) -> tuple[str, str]:
    return (a, b) if a < b else (b, a)


def route_pairs(route: tuple[str, ...]) -> tuple[tuple[str, str], ...]:
    return tuple(pair(route[i], route[(i + 1) % len(route)]) for i in range(len(route)))


def canonical_cycle(route: Iterable[str], depot: str) -> tuple[str, ...]:
    seq = tuple(route)
    if depot not in seq:
        raise ValueError(f"depot {depot!r} absent from route")
    pos = seq.index(depot)
    rotated = seq[pos:] + seq[:pos]
    reverse = (depot,) + tuple(reversed(rotated[1:]))
    return min(rotated, reverse)


def graph_for_instance(rb: dict[str, Any], instance_id: str) -> dict[str, Any]:
    instances = index(rb, "TSPInstances")
    addresses = index(rb, "Addresses")
    instance = instances[instance_id]
    stop_rows = sorted(
        (row for row in rows(rb, "InstanceStops") if row["TSPInstance"] == instance_id and row["IsRequired"]),
        key=lambda row: row["InstanceStopId"],
    )
    stops = [row["InstanceStopId"] for row in stop_rows]
    depot_address = instance["DepotAddress"]
    depot_matches = [row["InstanceStopId"] for row in stop_rows if row["Address"] == depot_address]
    if len(depot_matches) != 1:
        raise AssertionError(f"{instance_id}: expected one depot stop, got {depot_matches}")
    depot = depot_matches[0]
    costs: dict[tuple[str, str], float] = {}
    available: set[tuple[str, str]] = set()
    duplicate_pairs: set[tuple[str, str]] = set()
    for edge in rows(rb, "TravelEdges"):
        if edge["TSPInstance"] != instance_id:
            continue
        key = pair(edge["FromStop"], edge["ToStop"])
        if key in costs:
            duplicate_pairs.add(key)
        costs[key] = float(edge["TravelCost"])
        if edge.get("IsAvailable", True):
            available.add(key)
    return {
        "instance": instance,
        "stops": stops,
        "depot": depot,
        "costs": costs,
        "available": available,
        "duplicate_pairs": duplicate_pairs,
        "address_count": len(addresses),
    }


def connected_components(vertices: Iterable[str], edges: Iterable[tuple[str, str]]) -> list[set[str]]:
    vertices = set(vertices)
    adjacency: dict[str, set[str]] = defaultdict(set)
    for a, b in edges:
        adjacency[a].add(b)
        adjacency[b].add(a)
    unseen = set(vertices)
    result: list[set[str]] = []
    while unseen:
        root = min(unseen)
        unseen.remove(root)
        component = {root}
        queue = deque([root])
        while queue:
            current = queue.popleft()
            for nxt in adjacency[current]:
                if nxt in unseen:
                    unseen.remove(nxt)
                    component.add(nxt)
                    queue.append(nxt)
        result.append(component)
    return result


def reconstruct_cycle(vertices: list[str], edges: set[tuple[str, str]], depot: str) -> tuple[str, ...] | None:
    adjacency: dict[str, list[str]] = defaultdict(list)
    for a, b in edges:
        adjacency[a].append(b)
        adjacency[b].append(a)
    if any(len(adjacency[v]) != 2 for v in vertices):
        return None
    route = [depot]
    previous: str | None = None
    current = depot
    while True:
        options = sorted(adjacency[current])
        nxt = options[0] if previous is None else (options[0] if options[1] == previous else options[1])
        if nxt == depot:
            break
        if nxt in route:
            return None
        route.append(nxt)
        previous, current = current, nxt
        if len(route) > len(vertices):
            return None
    if len(route) != len(vertices):
        return None
    return canonical_cycle(route, depot)


def degree_two_bound(stops: list[str], available: set[tuple[str, str]], costs: dict[tuple[str, str], float]) -> tuple[float | None, dict[str, tuple[tuple[str, str], tuple[str, str]]]]:
    chosen: dict[str, tuple[tuple[str, str], tuple[str, str]]] = {}
    total = 0.0
    for stop in stops:
        incident = sorted(
            (edge for edge in available if stop in edge),
            key=lambda edge: (costs[edge], edge),
        )
        if len(incident) < 2:
            return None, {}
        first, second = incident[:2]
        chosen[stop] = (first, second)
        total += costs[first] + costs[second]
    return total / 2.0, chosen


def local_minimum_union(
    stops: list[str],
    available: set[tuple[str, str]],
    costs: dict[tuple[str, str], float],
    chosen: dict[str, tuple[tuple[str, str], tuple[str, str]]],
    depot: str,
) -> dict[str, Any]:
    selected = {edge for pair_edges in chosen.values() for edge in pair_edges}
    degree = {stop: sum(stop in edge for edge in selected) for stop in stops}
    components = connected_components(stops, selected)
    violations = sum(value != 2 for value in degree.values())
    cycle = reconstruct_cycle(stops, selected, depot) if len(selected) == len(stops) and violations == 0 and len(components) == 1 else None
    return {
        "edges": sorted(selected),
        "cost": sum(costs[edge] for edge in selected),
        "degree": degree,
        "degree_violation_count": violations,
        "component_count": len(components),
        "is_hamiltonian": cycle is not None,
        "cycle": cycle,
    }


def degree_closure(
    stops: list[str],
    available: set[tuple[str, str]],
) -> dict[str, Any]:
    forced: set[tuple[str, str]] = set()
    forbidden: set[tuple[str, str]] = set()
    contradiction = False
    rounds = 0
    while True:
        rounds += 1
        changed = False
        for stop in stops:
            incident = {edge for edge in available if stop in edge}
            current_forced = incident & forced
            unknown = incident - forced - forbidden
            if len(current_forced) > 2 or len(current_forced) + len(unknown) < 2:
                contradiction = True
                break
            need = 2 - len(current_forced)
            if need == 0:
                new = unknown - forbidden
                if new:
                    forbidden |= new
                    changed = True
            elif len(unknown) == need:
                new = unknown - forced
                if new:
                    forced |= new
                    changed = True
        if contradiction:
            break
        adjacency: dict[str, set[str]] = defaultdict(set)
        for a, b in forced:
            adjacency[a].add(b)
            adjacency[b].add(a)
        for component in connected_components(stops, forced):
            if len(component) >= len(stops):
                continue
            endpoints = sorted(v for v in component if len(adjacency[v]) == 1)
            if len(endpoints) == 2:
                closing = pair(endpoints[0], endpoints[1])
                if closing in available and closing not in forced and closing not in forbidden:
                    forbidden.add(closing)
                    changed = True
            if component and all(len(adjacency[v]) == 2 for v in component):
                contradiction = True
                break
        if contradiction or not changed or rounds > len(available) + len(stops) + 5:
            break
    return {
        "forced": sorted(forced),
        "forbidden": sorted(forbidden),
        "rounds": rounds,
        "contradiction": contradiction,
    }


def evaluate_instance(rb: dict[str, Any], instance_id: str) -> dict[str, Any]:
    graph = graph_for_instance(rb, instance_id)
    stops: list[str] = graph["stops"]
    depot: str = graph["depot"]
    costs: dict[tuple[str, str], float] = graph["costs"]
    available: set[tuple[str, str]] = graph["available"]
    others = tuple(stop for stop in stops if stop != depot)
    tours: list[tuple[tuple[str, ...], float, set[tuple[str, str]]]] = []
    total_classes = 0
    for perm in itertools.permutations(others):
        if perm > tuple(reversed(perm)):
            continue
        total_classes += 1
        route = (depot,) + perm
        edges = set(route_pairs(route))
        if not edges <= available:
            continue
        cost = sum(costs[edge] for edge in edges)
        tours.append((route, cost, edges))
    expected_total = factorial(max(len(stops) - 1, 0)) // 2 if len(stops) > 2 else 1
    if total_classes != expected_total:
        raise AssertionError(f"{instance_id}: canonical class count {total_classes} != {expected_total}")
    feasible_costs = sorted({cost for _, cost, _ in tours})
    optimum = feasible_costs[0] if feasible_costs else None
    optimal = [canonical_cycle(route, depot) for route, cost, _ in tours if cost == optimum]
    optimal = sorted(set(optimal))
    second_best = feasible_costs[1] if len(feasible_costs) > 1 else None
    lower, chosen = degree_two_bound(stops, available, costs)
    union = local_minimum_union(stops, available, costs, chosen, depot) if lower is not None else {
        "edges": [], "cost": None, "degree": {}, "degree_violation_count": len(stops), "component_count": len(stops), "is_hamiltonian": False, "cycle": None,
    }
    closure = degree_closure(stops, available)
    forced, forbidden = set(closure["forced"]), set(closure["forbidden"])
    residual = [entry for entry in tours if forced <= entry[2] and not (forbidden & entry[2])]
    route_closed = bool(union["is_hamiltonian"] and optimum is not None and union["cost"] == optimum)
    if not route_closed and len(residual) == 1:
        route_closed = True
    value_closed = bool(optimum is not None and lower is not None and abs(lower - optimum) < 1e-9)
    if route_closed:
        value_closed = True
    residual_costs = sorted({cost for _, cost, _ in residual})
    residual_class_count = 1 if route_closed else len(residual)
    residual_value_count = 1 if value_closed else len(residual_costs)
    residual_optimal_count = 1 if route_closed else sum(cost == optimum for _, cost, _ in residual)
    if value_closed:
        branch_warrant = "REJECTED_VALUE_ALREADY_CERTIFIED"
    elif residual_value_count > 1:
        branch_warrant = "REQUIRED_FOR_VALUE"
    elif residual_class_count > 1:
        branch_warrant = "REJECTED_VALUE_CLOSED_CHOICE_ONLY"
    else:
        branch_warrant = "NOT_REQUIRED"
    payload: dict[str, Any] = {
        "instance_id": instance_id,
        "stop_order": stops,
        "depot_stop": depot,
        "total_class_count": total_classes,
        "feasible_class_count": len(tours),
        "optimum_cost": optimum,
        "optimal_routes": [list(route) for route in optimal],
        "optimal_class_count": len(optimal),
        "second_best_cost": second_best,
        "degree_two_lower_bound": lower,
        "degree_two_gap": None if optimum is None or lower is None else optimum - lower,
        "local_union_edges": [list(edge) for edge in union["edges"]],
        "local_union_cost": union["cost"],
        "local_union_component_count": union["component_count"],
        "local_union_degree_violation_count": union["degree_violation_count"],
        "local_union_is_hamiltonian": union["is_hamiltonian"],
        "local_union_cycle": list(union["cycle"]) if union["cycle"] else None,
        "forced_edges": [list(edge) for edge in closure["forced"]],
        "forbidden_edges": [list(edge) for edge in closure["forbidden"]],
        "closure_round_count": closure["rounds"],
        "closure_contradiction": closure["contradiction"],
        "deterministic_residual_class_count": residual_class_count,
        "deterministic_residual_value_count": residual_value_count,
        "deterministic_residual_optimal_count": residual_optimal_count,
        "deterministic_value_closed": value_closed,
        "deterministic_route_closed": route_closed,
        "branch_warrant_status": branch_warrant,
        "distinct_feasible_value_count": len(feasible_costs),
    }
    canonical = json.dumps(payload, sort_keys=True, separators=(",", ":"))
    payload["oracle_checksum"] = hashlib.sha256(canonical.encode()).hexdigest()
    return payload


def evaluate_all(rb: dict[str, Any], instance_ids: Iterable[str] | None = None) -> dict[str, dict[str, Any]]:
    if instance_ids is None:
        instance_ids = [
            row["TSPInstanceId"]
            for row in rows(rb, "TSPInstances")
            if row.get("Status") != "NEGATIVE_NORMALIZATION_FIXTURE"
        ]
    result: dict[str, dict[str, Any]] = {}
    for instance_id in instance_ids:
        result[instance_id] = evaluate_instance(rb, instance_id)
    return result
