#!/usr/bin/env python3
"""Fail-loud validator for the TSP rulebook through planned loop 596.

This validator is intentionally state-aware: loops 587-596 may be PLANNED,
BLOCKED, or CLOSED. Every completed loop activates its own acceptance checks,
while the planned state still validates table shape, relationships, formulas,
and before/closure records.
"""
from __future__ import annotations

import json
import math
import re
import sys
from collections import defaultdict, deque
from pathlib import Path
from typing import Any

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"
sys.path.insert(0, str(HERE))
from reference_model import evaluate_graph, evaluate_tours  # noqa: E402

ALLOWED_FUNCTIONS = {
    "ABS", "AND", "AVERAGEIFS", "COALESCE", "CONCAT", "COUNT", "COUNTIFS",
    "FALSE", "FIND", "IF", "IFERROR", "INDEX", "LEFT", "LEN", "LOG",
    "LOG10", "LOWER", "MATCH", "MAX", "MID", "MIN", "NOT", "OR",
    "POWER", "RIGHT", "ROUND", "SEARCH", "SUBSTITUTE", "SUM", "SUMIFS",
    "TEXT", "TRIM", "TRUE", "UPPER", "VALUE",
}


def fail(message: str) -> None:
    raise AssertionError(message)


def load(path: Path) -> dict[str, Any]:
    if not path.is_file():
        raise FileNotFoundError(path)
    return json.loads(path.read_text())


def tables(rb: dict[str, Any]) -> dict[str, dict[str, Any]]:
    return {k: v for k, v in rb.items() if k != "__meta__" and isinstance(v, dict) and isinstance(v.get("schema"), list) and isinstance(v.get("data"), list)}


def id_field(tbl: dict[str, Any]) -> str:
    if not tbl["schema"]:
        fail("empty schema")
    return tbl["schema"][0]["name"]


def table_index(rb: dict[str, Any], name: str) -> dict[str, dict[str, Any]]:
    fld = id_field(rb[name])
    return {r[fld]: r for r in rb[name]["data"]}


def meta_int(rb: dict[str, Any], key: str) -> int:
    for row in rb["__meta__"]["data"]:
        if row["MetaKey"] == key:
            return int(row["IntegerValue"])
    fail(f"missing meta key {key}")


def validate_shapes(rb: dict[str, Any]) -> None:
    all_tables = tables(rb)
    if len(all_tables) < 19:
        fail(f"expected at least 19 tables, got {len(all_tables)}")
    for name, tbl in all_tables.items():
        if not tbl.get("Description"):
            fail(f"{name}: missing Description")
        first = tbl["schema"][0]
        if first.get("type") != "raw" or first.get("nullable") is not False:
            fail(f"{name}: first field must be non-null raw identifier")
        names = [f.get("name") for f in tbl["schema"]]
        if len(names) != len(set(names)):
            fail(f"{name}: duplicate field")
        if "Name" not in names:
            fail(f"{name}: missing Name")
        if next(f for f in tbl["schema"] if f["name"] == "Name").get("type") != "calculated":
            fail(f"{name}.Name must be calculated")
        for f in tbl["schema"]:
            if not f.get("Description"):
                fail(f"{name}.{f.get('name')}: missing Description")
            if f.get("type") == "relationship" and not f.get("RelatedTo"):
                fail(f"{name}.{f.get('name')}: relationship missing RelatedTo")
            if f.get("type") in {"calculated", "lookup", "aggregation"} and not f.get("formula"):
                fail(f"{name}.{f.get('name')}: derived field missing formula")
        identifier = first["name"]
        values = [r.get(identifier) for r in tbl["data"]]
        if any(v is None for v in values):
            fail(f"{name}: row missing {identifier}")
        if len(values) != len(set(values)):
            fail(f"{name}: duplicate {identifier}")


def validate_relationships(rb: dict[str, Any]) -> None:
    all_tables = tables(rb)
    ids = {name: set(table_index(rb, name)) for name in all_tables}
    graph: dict[str, set[str]] = defaultdict(set)
    for name, tbl in all_tables.items():
        for f in tbl["schema"]:
            if f.get("type") != "relationship":
                continue
            target = f["RelatedTo"]
            if target not in all_tables:
                fail(f"{name}.{f['name']}: unknown target {target}")
            if target == name:
                fail(f"{name}.{f['name']}: self relationships are not allowed")
            graph[name].add(target)
            for row in tbl["data"]:
                value = row.get(f["name"])
                if value is None:
                    if not f.get("nullable"):
                        fail(f"{name}.{f['name']}: null non-null relationship")
                elif value not in ids[target]:
                    fail(f"{name}.{f['name']}: {value!r} missing from {target}")
    indegree = {n: 0 for n in all_tables}
    outgoing: dict[str, set[str]] = defaultdict(set)
    for child, parents in graph.items():
        for parent in parents:
            outgoing[child].add(parent)
            indegree[parent] += 1
    q = deque(n for n, d in indegree.items() if d == 0)
    seen = 0
    while q:
        n = q.popleft(); seen += 1
        for parent in outgoing[n]:
            indegree[parent] -= 1
            if indegree[parent] == 0:
                q.append(parent)
    if seen != len(all_tables):
        fail(f"relationship DAG cycle: {sorted(n for n,d in indegree.items() if d)}")


def validate_formulas(rb: dict[str, Any]) -> None:
    all_tables = tables(rb)
    fields = {n: {f["name"] for f in t["schema"]} for n, t in all_tables.items()}
    cross = re.compile(r"([A-Za-z][A-Za-z0-9_]*)!\{\{([A-Za-z][A-Za-z0-9_]*)\}\}")
    local = re.compile(r"\{\{([A-Za-z][A-Za-z0-9_]*)\}\}")
    funcs = re.compile(r"(?<![A-Za-z0-9_])([A-Z][A-Z0-9_]*)\s*\(")
    for name, tbl in all_tables.items():
        for f in tbl["schema"]:
            formula = f.get("formula")
            if not formula:
                continue
            if not formula.startswith("="):
                fail(f"{name}.{f['name']}: formula must start '='")
            for tn, fn in cross.findall(formula):
                if tn not in fields or fn not in fields[tn]:
                    fail(f"{name}.{f['name']}: unknown cross reference {tn}.{fn}")
            stripped = cross.sub("", formula)
            for fn in local.findall(stripped):
                if fn not in fields[name]:
                    fail(f"{name}.{f['name']}: unknown local field {fn}")
            unknown = set(funcs.findall(formula)) - ALLOWED_FUNCTIONS
            if unknown:
                fail(f"{name}.{f['name']}: unsupported functions {sorted(unknown)}")


def raw_tour_cost(rb: dict[str, Any], candidate: str) -> float:
    edge_map = table_index(rb, "TravelEdges")
    return sum(float(edge_map[l["TravelEdge"]]["TravelCost"]) for l in rb["TourLegs"]["data"] if l["CandidateTour"] == candidate)


def components_for_set(rb: dict[str, Any], set_id: str) -> int:
    edge_map = table_index(rb, "TravelEdges")
    members = [r for r in rb["TSPDerivedEdgeSetMembers"]["data"] if r["DerivedEdgeSet"] == set_id]
    vertices: set[str] = set(); adj: dict[str, set[str]] = defaultdict(set)
    for m in members:
        e = edge_map[m["TravelEdge"]]; a,b=e["FromStop"],e["ToStop"]
        vertices.update((a,b)); adj[a].add(b); adj[b].add(a)
    count = 0; unseen = set(vertices)
    while unseen:
        count += 1; root = unseen.pop(); q = [root]
        while q:
            cur = q.pop()
            for nxt in adj[cur]:
                if nxt in unseen: unseen.remove(nxt); q.append(nxt)
    return count


def completed(loop_rows: dict[int, dict[str, Any]], order: int) -> bool:
    return loop_rows[order]["Status"] in {"CLOSED", "BLOCKED", "FALSIFIED", "DEFERRED"}


def validate_semantics(rb: dict[str, Any], contract: dict[str, Any]) -> None:
    loop_rows = {int(r["LoopOrder"]): r for r in rb["TSPLoops"]["data"]}
    planned = meta_int(rb, "last_planned_loop") if any(r["MetaKey"] == "last_planned_loop" for r in rb["__meta__"]["data"]) else meta_int(rb, "last_loop")
    if sorted(loop_rows) != list(range(577, planned + 1)):
        fail(f"loop sequence is not contiguous through {planned}")
    for order in range(587, planned + 1):
        row = loop_rows[order]
        if not row.get("PlannedClosureCriterion") or not row.get("BeforeState"):
            fail(f"loop {order}: missing planned before/closure record")
        if completed(loop_rows, order) and not row.get("AfterState"):
            fail(f"loop {order}: completed without after-state")
    contract_loops = {int(r["LoopOrder"]): r for r in contract["Loops"]}
    for order in range(587, planned + 1):
        if contract_loops[order]["Status"] != loop_rows[order]["Status"]:
            fail(f"loop {order}: rulebook/contract status disagreement")

    graph = evaluate_graph(rb); tours = evaluate_tours(rb)
    if not graph["tsp-gridville-5"]["is_complete_undirected_graph"]:
        fail("Gridville graph no longer complete")
    ref = tours["tour-reference-ring"]
    if not ref.is_hamiltonian_cycle_witness or ref.total_travel_cost != 14:
        fail(f"Gridville reference witness drift: {ref}")

    if planned < 587:
        return
    if completed(loop_rows, 587):
        run = table_index(rb, "TSPExecutionRuns").get("postgres-commissioning-loop-587")
        if not run or run["Status"] not in {"SUCCEEDED", "FAILED", "BLOCKED"}:
            fail("loop 587 execution run missing")
        if run["Status"] != "SUCCEEDED" and not run.get("FailureReason"):
            fail("blocked/failed commissioning lacks exact reason")
    if completed(loop_rows, 588):
        if len(rb["TSPInferenceApplications"]["data"]) < 5 or len(rb["TSPEdgeSupports"]["data"]) < 10:
            fail("loop 588 inference spine incomplete")
        selected = [r for r in rb["TSPEdgeStates"]["data"] if r["InferenceState"] == "state-gridville-local-bound-selections"]
        if len(selected) != 5 or any(r["DecisionStatus"] != "SELECTED" for r in selected):
            fail("loop 588 must hold five SELECTED, not FORCED, edges")
    if completed(loop_rows, 589):
        members = [r for r in rb["TSPDerivedEdgeSetMembers"]["data"] if r["DerivedEdgeSet"] == "edge-set-gridville-local-bound-union"]
        if len(members) != 5 or not all(r["SelectedAtBothEndpoints"] for r in members):
            fail("loop 589 derived edge union mismatch")
    if completed(loop_rows, 590):
        cert = table_index(rb, "TSPConnectedDegreeTwoCertificates")["connected-cycle-gridville-local-bound-union"]
        if not (cert["EdgeCount"] == 5 and cert["DegreeViolationCount"] == 0 and cert["ComponentCount"] == 1 and cert["ProperSubtourCount"] == 0 and cert["SpanningTreeEdgeCount"] == 4):
            fail("loop 590 connected degree-two certificate mismatch")
    if completed(loop_rows, 591):
        route = table_index(rb, "TSPRouteReconstructions")["reconstruction-gridville-local-bound-union"]
        steps = sorted((r for r in rb["TSPRouteReconstructionSteps"]["data"] if r["RouteReconstruction"] == route["TSPRouteReconstructionId"]), key=lambda r: r["StepOrder"])
        if route["CandidateUsedAsAntecedent"] or len(steps) != 5 or steps[-1]["ToStop"] != route["StartStop"] or route["TotalCost"] != 14:
            fail("loop 591 reconstruction mismatch")
    if completed(loop_rows, 592):
        cert = table_index(rb, "TSPSearchCertificates")["search-gridville-reconstructed-route"]
        if cert["InitialRouteClassCount"] != 12 or cert["SurvivingRouteClassCount"] != 1 or cert["BranchDecisionCount"] != 0 or cert["ResidualAmbiguityCount"] != 0:
            fail("loop 592 derived search accounting mismatch")
    if completed(loop_rows, 593):
        if len([r for r in rb["InstanceStops"]["data"] if r["TSPInstance"] == "tsp-twin-triangles-6"]) != 6:
            fail("loop 593 twin stops mismatch")
        if len([r for r in rb["TravelEdges"]["data"] if r["TSPInstance"] == "tsp-twin-triangles-6"]) != 15:
            fail("loop 593 twin edges mismatch")
        bounds = [r for r in rb["LocalDegreeBounds"]["data"] if r["TSPInstance"] == "tsp-twin-triangles-6"]
        edge_map = table_index(rb, "TravelEdges")
        lower = sum(float(edge_map[b["FirstEdge"]]["TravelCost"]) + float(edge_map[b["SecondEdge"]]["TravelCost"]) for b in bounds) / 2
        if lower != 6 or raw_tour_cost(rb, "tour-twin-triangles-feasible-24") != 24:
            fail("loop 593 lower-bound/candidate mismatch")
        if components_for_set(rb, "edge-set-twin-triangles-local-bound-union") != 2:
            fail("loop 593 must expose two selected components")
        if any(r["CandidateTour"] == "tour-twin-triangles-feasible-24" for r in rb["OptimalityCertificates"]["data"]):
            fail("loop 593 must not emit twin optimality certificate")
    if completed(loop_rows, 594):
        forced = [r for r in rb["TSPConstraintDecisions"]["data"] if r["DecisionStatus"] == "FORCED" and r["ConstraintRound"] == "constraint-round-sparse-forcing-1"]
        if len(forced) != 5 or any(not r["IsDeterministic"] for r in forced):
            fail("loop 594 forced-edge decisions mismatch")
    if completed(loop_rows, 595):
        decisions = table_index(rb, "TSPConstraintDecisions")
        if decisions["forbid-sparse-edge-b-d-degree-saturation"]["ReasonCode"] != "DEGREE_TWO_SATURATION":
            fail("loop 595 degree saturation missing")
        if decisions["forbid-twin-edge-wa-wc-proper-subtour"]["ReasonCode"] != "WOULD_CLOSE_PROPER_SUBTOUR":
            fail("loop 595 subtour forbidding missing")
    if completed(loop_rows, 596):
        states = [r for r in rb["TSPClusterBoundaryStates"]["data"] if r["TSPInstance"] == "tsp-twin-triangles-6"]
        if len(states) != 6 or any(not r["IsHamiltonianPath"] or r["IsDominated"] for r in states):
            fail("loop 596 cluster states mismatch")
        certs = table_index(rb, "TSPClusterContractionCertificates")
        if certs["contraction-twin-triangles-composition"]["ReductionPct"] != 75 or not certs["contraction-twin-triangles-composition"]["IsPassing"]:
            fail("loop 596 contraction certificate mismatch")


def main() -> None:
    rb = load(RULEBOOK); contract = load(CONTRACT)
    if rb.get("$schema") != "https://example.com/cmcc-schema/v1":
        fail("rulebook schema mismatch")
    if contract.get("Status") != "RESEARCH_PROGRAM":
        fail("contract must remain RESEARCH_PROGRAM")
    if contract.get("Claims", {}).get("PEqualsNP") is not False:
        fail("P=NP non-claim missing")
    validate_shapes(rb)
    validate_relationships(rb)
    validate_formulas(rb)
    validate_semantics(rb, contract)
    loop_rows = {int(r["LoopOrder"]): r for r in rb["TSPLoops"]["data"]}
    statuses = ", ".join(f"{n}:{loop_rows[n]['Status']}" for n in sorted(loop_rows) if n >= 587)
    print("traveling-salesman rulebook v3 validation: PASS")
    print(f"tables: {len(tables(rb))}")
    print(f"loops 587-596: {statuses}")


if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print(f"traveling-salesman v3 validation: FAIL: {exc}", file=sys.stderr)
        raise
