#!/usr/bin/env python3
"""Fail-loud structural and semantic validator for the TSP rulebook."""
from __future__ import annotations

import hashlib
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
from reference_model import (  # noqa: E402
    evaluate_graph,
    evaluate_instance_lower_bounds,
    evaluate_local_degree_bounds,
    evaluate_optimality_certificates,
    evaluate_search_metrics,
    evaluate_tours,
)

EXPECTED_TABLES = {
    "Cities",
    "Neighborhoods",
    "Addresses",
    "TSPInstances",
    "TSPGraphInvariantChecks",
    "InstanceStops",
    "TravelEdges",
    "CandidateTours",
    "TourStops",
    "TourLegs",
    "LocalDegreeBounds",
    "IncidentDominanceChecks",
    "InstanceLowerBounds",
    "OptimalityCertificates",
    "TSPInferenceRules",
    "TSPFrontierObligations",
    "TSPLoops",
    "SearchMetrics",
    "TSPInvariantChecks",
}

EXPECTED_ID_FIELDS = {
    "Cities": "CityId",
    "Neighborhoods": "NeighborhoodId",
    "Addresses": "AddressId",
    "TSPInstances": "TSPInstanceId",
    "TSPGraphInvariantChecks": "TSPGraphInvariantCheckId",
    "InstanceStops": "InstanceStopId",
    "TravelEdges": "TravelEdgeId",
    "CandidateTours": "CandidateTourId",
    "TourStops": "TourStopId",
    "TourLegs": "TourLegId",
    "LocalDegreeBounds": "LocalDegreeBoundId",
    "IncidentDominanceChecks": "IncidentDominanceCheckId",
    "InstanceLowerBounds": "InstanceLowerBoundId",
    "OptimalityCertificates": "OptimalityCertificateId",
    "TSPInferenceRules": "TSPInferenceRuleId",
    "TSPFrontierObligations": "TSPFrontierObligationId",
    "TSPLoops": "TSPLoopId",
    "SearchMetrics": "SearchMetricId",
    "TSPInvariantChecks": "TSPInvariantCheckId",
}

ALLOWED_FORMULA_FUNCTIONS = {
    "ABS", "AND", "AVERAGEIFS", "COALESCE", "CONCAT", "COUNT", "COUNTIFS",
    "FALSE", "FIND", "IF", "IFERROR", "INDEX", "LEFT", "LEN", "LOG",
    "LOG10", "LOWER", "MATCH", "MAX", "MID", "MIN", "NOT", "OR",
    "POWER", "RIGHT", "ROUND", "SEARCH", "SUBSTITUTE", "SUM", "SUMIFS",
    "TEXT", "TRIM", "TRUE", "UPPER", "VALUE",
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
    expected_id = EXPECTED_ID_FIELDS[name]
    first = schema[0]
    if first.get("name") != expected_id or first.get("type") != "raw":
        fail(f"{name}: first field must be raw {expected_id}, found {first}")
    names = [field.get("name") for field in schema]
    if len(names) != len(set(names)):
        fail(f"{name}: duplicate schema field name")
    if "Name" not in names:
        fail(f"{name}: missing calculated Name field")
    name_field = next(field for field in schema if field.get("name") == "Name")
    if name_field.get("type") != "calculated":
        fail(f"{name}.Name must be calculated")
    for field in schema:
        if not field.get("Description"):
            fail(f"{name}.{field.get('name')}: missing Description")
        if field.get("type") == "relationship" and not field.get("RelatedTo"):
            fail(f"{name}.{field.get('name')}: relationship missing RelatedTo")
        if field.get("type") in {"calculated", "lookup", "aggregation"} and not field.get("formula"):
            fail(f"{name}.{field.get('name')}: derived field missing formula")
    ids = [row.get(expected_id) for row in data]
    if any(value is None for value in ids):
        fail(f"{name}: data row missing {expected_id}")
    if len(ids) != len(set(ids)):
        fail(f"{name}: duplicate {expected_id}")


def validate_formulas(rulebook: dict) -> None:
    table_fields = {
        table_name: {field["name"] for field in rulebook[table_name]["schema"]}
        for table_name in EXPECTED_TABLES
    }
    cross_ref_re = re.compile(r"([A-Za-z][A-Za-z0-9_]*)!\{\{([A-Za-z][A-Za-z0-9_]*)\}\}")
    local_ref_re = re.compile(r"\{\{([A-Za-z][A-Za-z0-9_]*)\}\}")
    function_re = re.compile(r"(?<![A-Za-z0-9_])([A-Z][A-Z0-9_]*)\s*\(")

    for table_name in EXPECTED_TABLES:
        for field in rulebook[table_name]["schema"]:
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
                    fail(f"{location}: formula references unknown field {target_table}.{target_field}")
            without_cross_refs = cross_ref_re.sub("", formula)
            for local_field in local_ref_re.findall(without_cross_refs):
                if local_field not in table_fields[table_name]:
                    fail(f"{location}: formula references unknown local field {local_field}")
            unknown_functions = sorted(set(function_re.findall(formula)) - ALLOWED_FORMULA_FUNCTIONS)
            if unknown_functions:
                fail(f"{location}: unsupported formula functions {unknown_functions}")


def validate_relationships(rulebook: dict) -> None:
    id_fields: dict[str, str] = {}
    rows_by_id: dict[str, set] = {}
    graph: dict[str, set[str]] = defaultdict(set)
    for name in EXPECTED_TABLES:
        id_field = rulebook[name]["schema"][0]["name"]
        id_fields[name] = id_field
        rows_by_id[name] = {row[id_field] for row in rulebook[name]["data"]}

    for name in EXPECTED_TABLES:
        for field in rulebook[name]["schema"]:
            if field["type"] != "relationship":
                continue
            target = field["RelatedTo"]
            if target not in EXPECTED_TABLES:
                fail(f"{name}.{field['name']}: unknown relationship target {target}")
            graph[name].add(target)
            for row in rulebook[name]["data"]:
                value = row.get(field["name"])
                if value is None and not field["nullable"]:
                    fail(f"{name}.{field['name']}: null non-null relationship in {row}")
                if value is not None and value not in rows_by_id[target]:
                    fail(
                        f"{name}.{field['name']}: {value!r} does not match "
                        f"{target}.{id_fields[target]}"
                    )

    indegree = {node: 0 for node in EXPECTED_TABLES}
    reverse: dict[str, set[str]] = defaultdict(set)
    for child, parents in graph.items():
        for parent in parents:
            indegree[parent] += 1
            reverse[child].add(parent)
    queue = deque(node for node, degree in indegree.items() if degree == 0)
    seen = 0
    while queue:
        node = queue.popleft()
        seen += 1
        for parent in reverse[node]:
            indegree[parent] -= 1
            if indegree[parent] == 0:
                queue.append(parent)
    if seen != len(EXPECTED_TABLES):
        cyclic = sorted(node for node, degree in indegree.items() if degree)
        fail(f"relationship graph contains a cycle: {cyclic}")


def validate_pair_keys(rulebook: dict) -> None:
    for row in rulebook["TravelEdges"]["data"]:
        expected = "|".join(sorted((row["FromStop"], row["ToStop"])))
        if row["CanonicalPairKey"] != expected:
            fail(f"{row['TravelEdgeId']}: CanonicalPairKey {row['CanonicalPairKey']!r} != {expected!r}")


def validate_frontier(rulebook: dict) -> None:
    allowed_kinds = {
        "IMPORTED_DEPENDENCY",
        "INFERENCE_OBLIGATION",
        "CERTIFICATE_OBLIGATION",
        "SUBSTRATE_OBLIGATION",
        "GENERALIZATION_OBLIGATION",
        "RESIDUAL_SEARCH",
    }
    allowed_statuses = {"OPEN", "CLOSED", "BLOCKED", "DEFERRED", "FALSIFIED"}
    rows = rulebook["TSPFrontierObligations"]["data"]
    for row in rows:
        if row["ObligationKind"] not in allowed_kinds:
            fail(f"unknown frontier obligation kind: {row}")
        if row["Status"] not in allowed_statuses:
            fail(f"unknown frontier status: {row}")
        if row["Status"] == "CLOSED" and not row.get("ClosedByLoop"):
            fail(f"closed frontier row lacks ClosedByLoop: {row['TSPFrontierObligationId']}")
        if row["Status"] != "CLOSED" and row.get("ClosedByLoop") is not None:
            fail(f"open frontier row carries ClosedByLoop: {row['TSPFrontierObligationId']}")
        if row["ObligationKind"] == "IMPORTED_DEPENDENCY" and row["TrustDisposition"] != "EXTERNAL_PROVIDER_CONTRACT":
            fail(f"imported dependency lacks external provider trust disposition: {row}")
    imported = [row for row in rows if row["ObligationKind"] == "IMPORTED_DEPENDENCY"]
    if imported:
        fail(f"initial TSP domain should have zero active imported dependencies, found {imported}")


def validate_contract(rulebook_hash: str, contract: dict) -> None:
    if contract.get("Status") != "RESEARCH_PROGRAM":
        fail("problem contract must remain RESEARCH_PROGRAM")
    claims = contract.get("Claims", {})
    if claims.get("PEqualsNP") is not False:
        fail("problem contract must explicitly deny a P=NP claim")
    if claims.get("UniversalPolynomialNormalization") is not False:
        fail("problem contract must deny universal polynomial normalization")
    if claims.get("GeneralOptimalityProved") is not False:
        fail("problem contract must deny general TSP optimality")
    if claims.get("Gridville5OptimalityProved") is not True:
        fail("problem contract must record the finite Gridville certificate")
    if claims.get("ActiveImportedDependencyCount") != 0:
        fail("problem contract imported-dependency count must be zero")
    expected_hash = f"sha256:{rulebook_hash}"
    if contract.get("ArtifactHashes", {}).get("rulebook") != expected_hash:
        fail(f"contract rulebook hash mismatch; expected {expected_hash}")


def main() -> None:
    rulebook = load_json(RULEBOOK)
    contract = load_json(CONTRACT)
    if rulebook.get("$schema") != "https://example.com/cmcc-schema/v1":
        fail("rulebook $schema mismatch")

    actual_tables = {
        key
        for key, value in rulebook.items()
        if isinstance(value, dict) and "schema" in value and key != "__meta__"
    }
    if actual_tables != EXPECTED_TABLES:
        fail(
            f"table set mismatch; missing={sorted(EXPECTED_TABLES-actual_tables)} "
            f"extra={sorted(actual_tables-EXPECTED_TABLES)}"
        )
    for name in sorted(EXPECTED_TABLES):
        validate_table_shape(name, rulebook[name])
    validate_relationships(rulebook)
    validate_formulas(rulebook)
    validate_pair_keys(rulebook)
    validate_frontier(rulebook)

    loop_orders = [row["LoopOrder"] for row in rulebook["TSPLoops"]["data"]]
    if loop_orders != list(range(577, 587)):
        fail(f"unexpected loop sequence: {loop_orders}")

    graph = evaluate_graph(rulebook)
    expected_graph = {
        "tsp-gridville-5": {
            "count_of_stops": 5,
            "count_of_required_stops": 5,
            "count_of_travel_edges": 10,
            "count_of_inadmissible_edges": 0,
            "count_of_non_unique_edge_pair_rows": 0,
            "expected_undirected_edge_count": 10,
            "is_complete_undirected_graph": True,
        },
        "tsp-gridville-broken-3": {
            "count_of_stops": 3,
            "count_of_required_stops": 3,
            "count_of_travel_edges": 3,
            "count_of_inadmissible_edges": 2,
            "count_of_non_unique_edge_pair_rows": 2,
            "expected_undirected_edge_count": 3,
            "is_complete_undirected_graph": False,
        },
    }
    if graph != expected_graph:
        fail(f"unexpected graph results: {graph}")

    tours = evaluate_tours(rulebook)
    ref = tours.get("tour-reference-ring")
    duplicate = tours.get("tour-duplicate-b")
    missing_close = tours.get("tour-missing-close-duplicate-ab")
    if ref is None or duplicate is None or missing_close is None:
        fail("missing seeded tour witnesses")
    if not ref.is_hamiltonian_cycle_witness or ref.total_travel_cost != 14:
        fail(f"reference witness mismatch: {ref}")
    if not ref.is_optimality_proved or ref.residual_claim != "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE":
        fail(f"finite optimality certificate mismatch: {ref}")
    if duplicate.is_hamiltonian_cycle_witness or duplicate.total_travel_cost != 17:
        fail(f"duplicate-stop witness mismatch: {duplicate}")
    if missing_close.is_hamiltonian_cycle_witness or missing_close.total_travel_cost != 13:
        fail(f"global-cycle negative witness mismatch: {missing_close}")
    if duplicate.is_optimality_proved or missing_close.is_optimality_proved:
        fail("negative candidates must not carry optimality")

    local_bounds = evaluate_local_degree_bounds(rulebook)
    if len(local_bounds) != 5 or not all(row.is_two_cheapest_witness for row in local_bounds.values()):
        fail(f"local degree-bound witnesses mismatch: {local_bounds}")
    local_costs = sorted(row.local_bound_cost for row in local_bounds.values())
    if local_costs != [5.0, 5.0, 6.0, 6.0, 6.0]:
        fail(f"unexpected local degree-bound costs: {local_costs}")

    instance_bounds = evaluate_instance_lower_bounds(rulebook)
    bound = instance_bounds.get("degree-two-lower-bound-gridville-5")
    if bound is None or not bound.is_certified or bound.lower_bound_cost != 14:
        fail(f"instance lower-bound mismatch: {bound}")
    if bound.total_local_degree_bound_cost != 28:
        fail(f"double-counting sum mismatch: {bound}")

    certificates = evaluate_optimality_certificates(rulebook)
    certificate = certificates.get("optimality-gridville-reference-ring")
    if certificate is None or not certificate.is_passing:
        fail(f"optimality certificate mismatch: {certificate}")
    if certificate.candidate_travel_cost != 14 or certificate.lower_bound_cost != 14:
        fail(f"bound equality mismatch: {certificate}")

    search = evaluate_search_metrics(rulebook)
    if search != {
        "search-baseline-gridville-5": {
            "search_question": "DISCOVER_ROUTE_WITHOUT_SUPPLIED_CANDIDATE",
            "branch_count_before": 12,
            "branch_count_after": 12,
            "search_elimination_pct": 0.0,
            "residual_ambiguity_count": 12,
        },
        "search-optimality-verification-gridville-5": {
            "search_question": "VERIFY_OPTIMALITY_OF_SUPPLIED_CANDIDATE",
            "branch_count_before": 12,
            "branch_count_after": 0,
            "search_elimination_pct": 100.0,
            "residual_ambiguity_count": 0,
        },
    }:
        fail(f"unexpected search metrics: {search}")

    rulebook_hash = hashlib.sha256(RULEBOOK.read_bytes()).hexdigest()
    validate_contract(rulebook_hash, contract)

    print("traveling-salesman rulebook validation: PASS")
    print(f"tables: {len(EXPECTED_TABLES)}")
    print(f"loops: {loop_orders[0]}-{loop_orders[-1]}")
    print("frontier obligations: 8 (active imported dependencies: 0)")
    print("graph fixtures: complete Gridville=true; count-preserving duplicate-pair graph=false")
    print("tour-reference-ring: valid=true, cost=14, optimal-for-instance=true")
    print("negative tours: duplicate-stop=false; missing-close=false")
    print("degree-two lower bound: (5+5+6+6+6)/2 = 14")
    print("route discovery: 0% eliminated (12 -> 12)")
    print("supplied-candidate optimality verification: 100% enumeration avoided (12 -> 0)")


if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print(f"traveling-salesman validation: FAIL: {exc}", file=sys.stderr)
        raise
