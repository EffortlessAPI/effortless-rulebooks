#!/usr/bin/env python3
"""Postgres/Python conformance test for TSP loops 577-586."""
from __future__ import annotations

import os
import subprocess
import sys
from decimal import Decimal
from pathlib import Path

HERE = Path(__file__).resolve().parent
PROJECT = HERE.parent
RULEBOOK = PROJECT / "effortless-rulebook" / "traveling-salesman-rulebook.json"

sys.path.insert(0, str(PROJECT / "scripts"))
from reference_model import (  # noqa: E402
    evaluate_graph,
    evaluate_instance_lower_bounds,
    evaluate_local_degree_bounds,
    evaluate_optimality_certificates,
    evaluate_search_metrics,
    evaluate_tours,
    load_rulebook,
)

DB = os.environ.get("TSP_DB", "erb_traveling_salesman")
PGHOST = os.environ.get("PGHOST", "localhost")
PGUSER = os.environ.get("PGUSER", "postgres")

PG16 = "/opt/homebrew/opt/postgresql@16/bin"
if os.path.isdir(PG16):
    os.environ["PATH"] = PG16 + os.pathsep + os.environ["PATH"]


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
        raise RuntimeError(f"psql failed for database {DB}: {proc.stderr.strip()}")
    return [line for line in proc.stdout.splitlines() if line and "NOTICE" not in line]


def pg_bool(value: str) -> bool:
    if value == "t":
        return True
    if value == "f":
        return False
    raise ValueError(f"expected Postgres boolean t/f, got {value!r}")


def main() -> None:
    rulebook = load_rulebook(RULEBOOK)
    py_graph = evaluate_graph(rulebook)
    py_tours = evaluate_tours(rulebook)
    py_local = evaluate_local_degree_bounds(rulebook)
    py_bounds = evaluate_instance_lower_bounds(rulebook)
    py_optimality = evaluate_optimality_certificates(rulebook)
    py_search = evaluate_search_metrics(rulebook)
    failures: list[str] = []

    graph_rows = psql(
        "SELECT tsp_instance_id, count_of_stops, count_of_required_stops, "
        "count_of_travel_edges, count_of_inadmissible_edges, "
        "count_of_non_unique_edge_pair_rows, expected_undirected_edge_count, "
        "is_complete_undirected_graph FROM vw_tsp_instances ORDER BY tsp_instance_id"
    )
    graph_agreements = 0
    for row in graph_rows:
        iid, stops, required, edges, inadmissible, non_unique, expected, complete = row.split(",")
        pg = {
            "count_of_stops": int(stops),
            "count_of_required_stops": int(required),
            "count_of_travel_edges": int(edges),
            "count_of_inadmissible_edges": int(inadmissible),
            "count_of_non_unique_edge_pair_rows": int(non_unique),
            "expected_undirected_edge_count": int(expected),
            "is_complete_undirected_graph": pg_bool(complete),
        }
        if pg == py_graph.get(iid):
            graph_agreements += 1
        else:
            failures.append(f"[graph-equivalence] {iid}: postgres={pg} python={py_graph.get(iid)}")
    missing_graph = set(py_graph) - {row.split(",", 1)[0] for row in graph_rows}
    if missing_graph:
        failures.append(f"[graph] missing Postgres instances: {sorted(missing_graph)}")

    tour_rows = psql(
        "SELECT candidate_tour_id, required_stop_count, count_of_tour_stops, "
        "count_of_tour_legs, total_travel_cost, is_hamiltonian_cycle_witness, "
        "is_optimality_proved, residual_claim FROM vw_candidate_tours "
        "ORDER BY candidate_tour_id"
    )
    tour_agreements = 0
    for row in tour_rows:
        tid, required, stop_count, leg_count, cost, valid, optimal, claim = row.split(",")
        py = py_tours.get(tid)
        if py is None:
            failures.append(f"[tour] Postgres returned unknown candidate {tid}")
            continue
        same = (
            int(required) == py.required_stop_count
            and int(stop_count) == py.tour_stop_count
            and int(leg_count) == py.tour_leg_count
            and Decimal(cost) == Decimal(str(py.total_travel_cost))
            and pg_bool(valid) == py.is_hamiltonian_cycle_witness
            and pg_bool(optimal) == py.is_optimality_proved
            and claim == py.residual_claim
        )
        if same:
            tour_agreements += 1
        else:
            failures.append(f"[tour-equivalence] {tid}: postgres={row!r}, python={py!r}")
    missing_tours = set(py_tours) - {row.split(",", 1)[0] for row in tour_rows}
    if missing_tours:
        failures.append(f"[tour] missing Postgres candidates: {sorted(missing_tours)}")

    local_rows = psql(
        "SELECT local_degree_bound_id, required_dominance_check_count, "
        "count_of_dominance_checks, count_of_failed_dominance_checks, "
        "is_two_cheapest_witness, local_bound_cost FROM vw_local_degree_bounds "
        "ORDER BY local_degree_bound_id"
    )
    local_agreements = 0
    for row in local_rows:
        bid, required, count, failed, valid, cost = row.split(",")
        py = py_local.get(bid)
        if py is None:
            failures.append(f"[local-bound] Postgres returned unknown bound {bid}")
            continue
        same = (
            int(required) == py.required_dominance_check_count
            and int(count) == py.count_of_dominance_checks
            and int(failed) == py.count_of_failed_dominance_checks
            and pg_bool(valid) == py.is_two_cheapest_witness
            and Decimal(cost) == Decimal(str(py.local_bound_cost))
        )
        if same:
            local_agreements += 1
        else:
            failures.append(f"[local-bound-equivalence] {bid}: postgres={row!r}, python={py!r}")

    bound_rows = psql(
        "SELECT instance_lower_bound_id, count_of_local_degree_bounds, "
        "count_of_invalid_local_degree_bounds, total_local_degree_bound_cost, "
        "lower_bound_cost, is_certified FROM vw_instance_lower_bounds "
        "ORDER BY instance_lower_bound_id"
    )
    bound_agreements = 0
    for row in bound_rows:
        lower_id, count, invalid, total, lower, certified = row.split(",")
        py = py_bounds.get(lower_id)
        if py is None:
            failures.append(f"[instance-bound] Postgres returned unknown bound {lower_id}")
            continue
        same = (
            int(count) == py.count_of_local_degree_bounds
            and int(invalid) == py.count_of_invalid_local_degree_bounds
            and Decimal(total) == Decimal(str(py.total_local_degree_bound_cost))
            and Decimal(lower) == Decimal(str(py.lower_bound_cost))
            and pg_bool(certified) == py.is_certified
        )
        if same:
            bound_agreements += 1
        else:
            failures.append(f"[instance-bound-equivalence] {lower_id}: postgres={row!r}, python={py!r}")

    certificate_rows = psql(
        "SELECT optimality_certificate_id, candidate_travel_cost, lower_bound_cost, "
        "is_same_instance, is_bound_tight, is_passing, scope_claim "
        "FROM vw_optimality_certificates ORDER BY optimality_certificate_id"
    )
    certificate_agreements = 0
    for row in certificate_rows:
        cid, candidate_cost, lower_cost, same_instance, tight, passing, claim = row.split(",")
        py = py_optimality.get(cid)
        if py is None:
            failures.append(f"[optimality] Postgres returned unknown certificate {cid}")
            continue
        same = (
            Decimal(candidate_cost) == Decimal(str(py.candidate_travel_cost))
            and Decimal(lower_cost) == Decimal(str(py.lower_bound_cost))
            and pg_bool(same_instance) == py.is_same_instance
            and pg_bool(tight) == py.is_bound_tight
            and pg_bool(passing) == py.is_passing
            and claim == py.scope_claim
        )
        if same:
            certificate_agreements += 1
        else:
            failures.append(f"[optimality-equivalence] {cid}: postgres={row!r}, python={py!r}")

    invariant_rows = psql(
        "SELECT tsp_invariant_check_id, is_passing FROM vw_tsp_invariant_checks "
        "ORDER BY tsp_invariant_check_id"
    )
    invariant_passes = sum(pg_bool(row.split(",")[1]) for row in invariant_rows)
    if invariant_passes != len(invariant_rows):
        failures.append(f"[tour-invariants] {invariant_passes}/{len(invariant_rows)} passing")

    graph_invariant_rows = psql(
        "SELECT tsp_graph_invariant_check_id, is_passing "
        "FROM vw_tsp_graph_invariant_checks ORDER BY tsp_graph_invariant_check_id"
    )
    graph_invariant_passes = sum(pg_bool(row.split(",")[1]) for row in graph_invariant_rows)
    if graph_invariant_passes != len(graph_invariant_rows):
        failures.append(
            f"[graph-invariants] {graph_invariant_passes}/{len(graph_invariant_rows)} passing"
        )

    frontier_rows = psql(
        "SELECT count(*), count(*) FILTER (WHERE is_imported_dependency), "
        "count(*) FILTER (WHERE is_closed) FROM vw_tsp_frontier_obligations"
    )
    if frontier_rows != ["8,0,4"]:
        failures.append(f"[frontier] expected 8 total / 0 imported / 4 closed, got {frontier_rows}")

    metric_rows = psql(
        "SELECT search_metric_id, search_question, branch_count_before, "
        "branch_count_after, search_elimination_pct, residual_ambiguity_count "
        "FROM vw_search_metrics ORDER BY search_metric_id"
    )
    search_agreements = 0
    for row in metric_rows:
        metric_id, question, before, after, pct, residual = row.split(",")
        py = py_search.get(metric_id)
        if py is None:
            failures.append(f"[search] Postgres returned unknown metric {metric_id}")
            continue
        same = (
            question == py["search_question"]
            and int(before) == py["branch_count_before"]
            and int(after) == py["branch_count_after"]
            and Decimal(pct) == Decimal(str(py["search_elimination_pct"]))
            and int(residual) == py["residual_ambiguity_count"]
        )
        if same:
            search_agreements += 1
        else:
            failures.append(f"[search-equivalence] {metric_id}: postgres={row!r}, python={py!r}")

    print(f"graph substrate agreement: {graph_agreements}/{len(py_graph)}")
    print(f"tour substrate agreement: {tour_agreements}/{len(py_tours)}")
    print(f"local-bound substrate agreement: {local_agreements}/{len(py_local)}")
    print(f"instance-bound substrate agreement: {bound_agreements}/{len(py_bounds)}")
    print(f"optimality-certificate agreement: {certificate_agreements}/{len(py_optimality)}")
    print(f"tour invariants passing: {invariant_passes}/{len(invariant_rows)}")
    print(f"graph invariants passing: {graph_invariant_passes}/{len(graph_invariant_rows)}")
    print(f"search substrate agreement: {search_agreements}/{len(py_search)}")
    print("frontier obligations: 8 total, 0 imported, 4 closed")

    if failures:
        print("\nFAIL:")
        for failure in failures:
            print("  " + failure)
        raise SystemExit(1)
    print("\ntraveling-salesman Postgres conformance: PASS")


if __name__ == "__main__":
    main()
