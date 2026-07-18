#!/usr/bin/env python3
"""Live Postgres acceptance checks for the loop-610 convergence projection."""
from __future__ import annotations

import os
import subprocess
from decimal import Decimal


DB = os.environ.get("TSP_DB", "erb_traveling_salesman")
PGHOST = os.environ.get("PGHOST", "localhost")
PGUSER = os.environ.get("PGUSER", "postgres")


def psql(sql: str) -> list[str]:
    proc = subprocess.run(
        [
            "psql", "-h", PGHOST, "-U", PGUSER, "-d", DB,
            "-v", "ON_ERROR_STOP=1", "-tAF,", "-c", sql,
        ],
        capture_output=True,
        text=True,
    )
    if proc.returncode != 0:
        raise RuntimeError(proc.stderr.strip())
    return [line for line in proc.stdout.splitlines() if line and "NOTICE" not in line]


def scalar(sql: str) -> str:
    rows = psql(sql)
    if len(rows) != 1:
        raise AssertionError(f"expected one row, got {rows}")
    return rows[0]


def numeric(value: str) -> Decimal:
    return Decimal(value)


def main() -> None:
    concepts = scalar(
        "SELECT "
        "count(*) FILTER (WHERE concept_kind='LEGACY_SURFACE'),"
        "count(*) FILTER (WHERE concept_kind='CANONICAL_PRIMITIVE'),"
        "count(*) FILTER (WHERE concept_kind='LEGACY_SURFACE' AND status='COLLAPSED'),"
        "count(*) FILTER (WHERE concept_kind='INVENTED_PREDICATE') "
        "FROM vw_tsp_concepts"
    )
    if concepts != "32,8,32,6":
        raise AssertionError(f"concept convergence mismatch: {concepts}")

    patched = scalar(
        "SELECT total_cost::text, component_count, proper_subtour_count, status "
        "FROM vw_tsp_traversal_states "
        "WHERE tsp_traversal_state_id='traversal-twin-patched-cycle'"
    ).split(",")
    if numeric(patched[0]) != Decimal("24") or patched[1:] != ["1", "0", "CONNECTED_CYCLE"]:
        raise AssertionError(f"patched traversal mismatch: {patched}")

    exchange = scalar(
        "SELECT "
        "count(*) FILTER (WHERE source_kind='BALANCED_EDGE_EXCHANGE' AND modality='ADDED'),"
        "count(*) FILTER (WHERE source_kind='BALANCED_EDGE_EXCHANGE' AND modality='REMOVED') "
        "FROM vw_tsp_edge_commitments"
    )
    if exchange != "2,2":
        raise AssertionError(f"balanced exchange mismatch: {exchange}")

    costs = dict(
        row.split(",", 1)
        for row in psql(
            "SELECT tsp_cost_certificate_id, value::text "
            "FROM vw_tsp_cost_certificates "
            "WHERE tsp_cost_certificate_id IN "
            "('cost-twin-component-debt','cost-twin-repaired-lower','cost-twin-exact')"
        )
    )
    expected_costs = {
        "cost-twin-component-debt": Decimal("18"),
        "cost-twin-repaired-lower": Decimal("24"),
        "cost-twin-exact": Decimal("24"),
    }
    if {key: numeric(value) for key, value in costs.items()} != expected_costs:
        raise AssertionError(f"cost-certificate mismatch: {costs}")

    twin = scalar(
        "SELECT is_hamiltonian_cycle_witness::text, is_optimality_proved::text, "
        "total_travel_cost::text FROM vw_candidate_tours "
        "WHERE candidate_tour_id='tour-twin-triangles-feasible-24'"
    ).split(",")
    if twin[0:2] != ["true", "true"] or numeric(twin[2]) != Decimal("24"):
        raise AssertionError(f"twin optimality mismatch: {twin}")

    defects = scalar(
        "SELECT "
        "count(*) FILTER (WHERE tsp_instance='tsp-twin-triangles-6' AND status='OPEN'),"
        "count(*) FILTER (WHERE tsp_instance='tsp-twin-triangles-6' AND status='DISCHARGED'),"
        "count(*) FILTER (WHERE tsp_instance='tsp-twin-triangles-6' AND status='SYMMETRY_ONLY') "
        "FROM vw_tsp_defects"
    )
    if defects != "0,5,1":
        raise AssertionError(f"defect ledger mismatch: {defects}")

    regions = scalar("SELECT count(*) FROM vw_tsp_regions")
    if regions != "6":
        raise AssertionError(f"region count mismatch: {regions}")

    kernel = scalar(
        "SELECT initial_route_class_count, surviving_route_class_count, "
        "branch_decision_count, residual_ambiguity_count "
        "FROM vw_tsp_search_certificates "
        "WHERE tsp_search_certificate_id='search-twin-boundary-kernel'"
    )
    if kernel != "60,9,0,9":
        raise AssertionError(f"residual kernel mismatch: {kernel}")

    gauge = scalar(
        "SELECT initial_route_class_count, surviving_route_class_count, "
        "branch_decision_count, residual_ambiguity_count "
        "FROM vw_tsp_search_certificates "
        "WHERE tsp_search_certificate_id='search-twin-gauge-orbit'"
    )
    if gauge != "9,1,0,0":
        raise AssertionError(f"gauge ambiguity mismatch: {gauge}")

    closure = scalar(
        "SELECT alternative_count, residual_ambiguity_count, branch_decision_count, status "
        "FROM vw_tsp_inference_states "
        "WHERE tsp_inference_state_id='state-twin-gauge-fixed-point'"
    )
    if closure != "1,0,0,FIXED_POINT":
        raise AssertionError(f"final closure mismatch: {closure}")

    print("traveling-salesman convergence Postgres conformance: PASS")
    print("concepts: 32 surface -> 8 canonical; invented predicates: 6")
    print("twin triangles: lower=24 upper=24 optimal=true")
    print("residual kernel: 60 -> 9 -> 1 orbit; branch decisions=0")


if __name__ == "__main__":
    main()
