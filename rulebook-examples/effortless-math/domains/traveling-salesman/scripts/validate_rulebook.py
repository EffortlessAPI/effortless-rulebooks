#!/usr/bin/env python3
"""Fail-loud structural validator for the TSP starter rulebook."""
from __future__ import annotations

import json
import re
import sys
from collections import defaultdict, deque
from pathlib import Path

HERE = Path(__file__).resolve().parent
PROJECT = HERE.parent
RULEBOOK = PROJECT / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = PROJECT / "problem-contract.json"

sys.path.insert(0, str(HERE))
from reference_model import evaluate_graph, evaluate_tours  # noqa: E402


EXPECTED_TABLES = {
    "Cities",
    "Neighborhoods",
    "Addresses",
    "TSPInstances",
    "InstanceStops",
    "TravelEdges",
    "CandidateTours",
    "TourStops",
    "TourLegs",
    "TSPInferenceRules",
    "TSPLoops",
    "SearchMetrics",
    "TSPInvariantChecks",
}


def fail(message: str) -> None:
    raise AssertionError(message)


def load_json(path: Path) -> dict:
    if not path.is_file():
        raise FileNotFoundError(f"missing required file: {path}")
    try:
        return json.loads(path.read_text())
    except json.JSONDecodeError as exc:
        raise ValueError(f"invalid JSON in {path}: {exc}") from exc


def validate_table_shape(name: str, table: dict) -> None:
    if not table.get("Description"):
        fail(f"{name}: missing Description")
    schema = table.get("schema")
    data = table.get("data")
    if not isinstance(schema, list) or not isinstance(data, list):
        fail(f"{name}: schema and data must be arrays")
    if not schema:
        fail(f"{name}: empty schema")
    expected_ids = {
        "Cities": "CityId",
        "Neighborhoods": "NeighborhoodId",
        "Addresses": "AddressId",
        "TSPInstances": "TSPInstanceId",
        "InstanceStops": "InstanceStopId",
        "TravelEdges": "TravelEdgeId",
        "CandidateTours": "CandidateTourId",
        "TourStops": "TourStopId",
        "TourLegs": "TourLegId",
        "TSPInferenceRules": "TSPInferenceRuleId",
        "TSPLoops": "TSPLoopId",
        "SearchMetrics": "SearchMetricId",
        "TSPInvariantChecks": "TSPInvariantCheckId",
    }
    expected_id = expected_ids[name]
    first = schema[0]
    if first.get("name") != expected_id or first.get("type") != "raw":
        fail(f"{name}: first field must be raw {expected_id}, found {first}")
    names = [f.get("name") for f in schema]
    if len(names) != len(set(names)):
        fail(f"{name}: duplicate schema field name")
    if "Name" not in names:
        fail(f"{name}: missing calculated Name field")
    name_field = next(f for f in schema if f.get("name") == "Name")
    if name_field.get("type") != "calculated":
        fail(f"{name}.Name must be calculated")
    for f in schema:
        if not f.get("Description"):
            fail(f"{name}.{f.get('name')}: missing Description")
        if f.get("type") == "relationship" and not f.get("RelatedTo"):
            fail(f"{name}.{f.get('name')}: relationship missing RelatedTo")
        if f.get("type") in {"calculated", "lookup", "aggregation"} and not f.get("formula"):
            fail(f"{name}.{f.get('name')}: derived field missing formula")
    ids = [row.get(expected_id) for row in data]
    if any(v is None for v in ids):
        fail(f"{name}: data row missing {expected_id}")
    if len(ids) != len(set(ids)):
        fail(f"{name}: duplicate {expected_id}")


ALLOWED_FORMULA_FUNCTIONS = {
    "ABS", "AND", "AVERAGEIFS", "COALESCE", "CONCAT", "COUNT", "COUNTIFS",
    "FALSE", "FIND", "IF", "IFERROR", "INDEX", "LEFT", "LEN", "LOG",
    "LOG10", "LOWER", "MATCH", "MAX", "MID", "MIN", "NOT", "OR",
    "POWER", "RIGHT", "ROUND", "SEARCH", "SUBSTITUTE", "SUM", "SUMIFS",
    "TEXT", "TRIM", "TRUE", "UPPER", "VALUE",
}


def validate_formulas(rb: dict) -> None:
    """Verify every formula only names declared fields/tables and supported functions."""
    table_fields = {
        table_name: {field["name"] for field in rb[table_name]["schema"]}
        for table_name in EXPECTED_TABLES
    }
    cross_ref_re = re.compile(r"([A-Za-z][A-Za-z0-9_]*)!\{\{([A-Za-z][A-Za-z0-9_]*)\}\}")
    local_ref_re = re.compile(r"\{\{([A-Za-z][A-Za-z0-9_]*)\}\}")
    function_re = re.compile(r"(?<![A-Za-z0-9_])([A-Z][A-Z0-9_]*)\s*\(")

    for table_name in EXPECTED_TABLES:
        for field in rb[table_name]["schema"]:
            formula = field.get("formula")
            if not formula:
                continue
            location = f"{table_name}.{field['name']}"
            if not formula.startswith("="):
                fail(f"{location}: formula must begin with '='")

            for target_table, target_field in cross_ref_re.findall(formula):
                if target_table not in table_fields:
                    fail(f"{location}: formula references unknown table {target_table}")
                if target_field not in table_fields[target_table]:
                    fail(
                        f"{location}: formula references unknown field "
                        f"{target_table}.{target_field}"
                    )

            without_cross_refs = cross_ref_re.sub("", formula)
            for local_field in local_ref_re.findall(without_cross_refs):
                if local_field not in table_fields[table_name]:
                    fail(f"{location}: formula references unknown local field {local_field}")

            unknown_functions = sorted(set(function_re.findall(formula)) - ALLOWED_FORMULA_FUNCTIONS)
            if unknown_functions:
                fail(f"{location}: unsupported formula functions {unknown_functions}")


def validate_relationships(rb: dict) -> None:
    id_fields = {}
    rows_by_id = {}
    graph = defaultdict(set)
    for name in EXPECTED_TABLES:
        table = rb[name]
        id_field = table["schema"][0]["name"]
        id_fields[name] = id_field
        rows_by_id[name] = {r[id_field] for r in table["data"]}

    for name in EXPECTED_TABLES:
        fields = {f["name"]: f for f in rb[name]["schema"]}
        for f in fields.values():
            if f["type"] != "relationship":
                continue
            target = f["RelatedTo"]
            if target not in EXPECTED_TABLES:
                fail(f"{name}.{f['name']}: unknown relationship target {target}")
            graph[name].add(target)
            for row in rb[name]["data"]:
                value = row.get(f["name"])
                if value is None and not f["nullable"]:
                    fail(f"{name}.{f['name']}: null non-null relationship in {row}")
                if value is not None and value not in rows_by_id[target]:
                    fail(
                        f"{name}.{f['name']}: {value!r} does not match "
                        f"{target}.{id_fields[target]}"
                    )

    # Relationship edges must be acyclic.
    indegree = {n: 0 for n in EXPECTED_TABLES}
    reverse = defaultdict(set)
    for child, parents in graph.items():
        for parent in parents:
            indegree[parent] += 1
            reverse[child].add(parent)
    q = deque(n for n, d in indegree.items() if d == 0)
    seen = 0
    while q:
        n = q.popleft()
        seen += 1
        for parent in reverse[n]:
            indegree[parent] -= 1
            if indegree[parent] == 0:
                q.append(parent)
    if seen != len(EXPECTED_TABLES):
        cyclic = sorted(n for n, d in indegree.items() if d)
        fail(f"relationship graph contains a cycle: {cyclic}")


def main() -> None:
    rb = load_json(RULEBOOK)
    contract = load_json(CONTRACT)
    if rb.get("$schema") != "https://example.com/cmcc-schema/v1":
        fail("rulebook $schema mismatch")
    if contract.get("Status") != "RESEARCH_PROGRAM":
        fail("problem contract must remain RESEARCH_PROGRAM")
    if contract.get("Claims", {}).get("PEqualsNP") is not False:
        fail("problem contract must explicitly deny a P=NP claim")
    if contract.get("Claims", {}).get("OptimalityProved") is not False:
        fail("problem contract must explicitly deny an optimality proof")

    actual_tables = {k for k, v in rb.items() if isinstance(v, dict) and "schema" in v and k != "__meta__"}
    if actual_tables != EXPECTED_TABLES:
        fail(
            f"table set mismatch; missing={sorted(EXPECTED_TABLES-actual_tables)} "
            f"extra={sorted(actual_tables-EXPECTED_TABLES)}"
        )
    for name in sorted(EXPECTED_TABLES):
        validate_table_shape(name, rb[name])
    validate_relationships(rb)
    validate_formulas(rb)

    loop_orders = [r["LoopOrder"] for r in rb["TSPLoops"]["data"]]
    if loop_orders != [577, 578, 579, 580, 581]:
        fail(f"unexpected loop sequence: {loop_orders}")

    graph = evaluate_graph(rb)
    instance = graph.get("tsp-gridville-5")
    if instance is None:
        fail("missing tsp-gridville-5 graph result")
    if instance != {
        "count_of_stops": 5,
        "count_of_required_stops": 5,
        "count_of_travel_edges": 10,
        "expected_undirected_edge_count": 10,
        "is_complete_undirected_graph": True,
    }:
        fail(f"unexpected graph result: {instance}")

    tours = evaluate_tours(rb)
    ref = tours.get("tour-reference-ring")
    bad = tours.get("tour-duplicate-b")
    if ref is None or bad is None:
        fail("missing seeded tour witnesses")
    if not ref.is_hamiltonian_cycle_witness or ref.total_travel_cost != 14:
        fail(f"reference witness mismatch: {ref}")
    if bad.is_hamiltonian_cycle_witness or bad.total_travel_cost != 17:
        fail(f"negative witness mismatch: {bad}")
    if ref.is_optimality_proved or bad.is_optimality_proved:
        fail("initial architecture must not claim optimality")

    print("traveling-salesman rulebook validation: PASS")
    print(f"tables: {len(EXPECTED_TABLES)}")
    print(f"loops: {loop_orders[0]}-{loop_orders[-1]}")
    print("graph: 5 required stops, 10 canonical edges, complete=true")
    print("tour-reference-ring: valid=true, cost=14, optimality=false")
    print("tour-duplicate-b: valid=false, cost=17, optimality=false")
    print("search eliminated: 0% (12 -> 12 residual route classes)")


if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print(f"traveling-salesman validation: FAIL: {exc}", file=sys.stderr)
        raise
