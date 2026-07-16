#!/usr/bin/env python3
"""Postgres/Python conformance test for the initial TSP domain."""
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
from reference_model import evaluate_graph, evaluate_tours, load_rulebook  # noqa: E402

DB = os.environ.get("TSP_DB", "erb_traveling_salesman")
PGHOST = os.environ.get("PGHOST", "localhost")
PGUSER = os.environ.get("PGUSER", "postgres")

PG16 = "/opt/homebrew/opt/postgresql@16/bin"
if os.path.isdir(PG16):
    os.environ["PATH"] = PG16 + os.pathsep + os.environ["PATH"]


def psql(sql: str) -> list[str]:
    proc = subprocess.run(
        ["psql", "-h", PGHOST, "-U", PGUSER, "-d", DB, "-v", "ON_ERROR_STOP=1", "-tAF,", "-c", sql],
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
    rb = load_rulebook(RULEBOOK)
    py_graph = evaluate_graph(rb)
    py_tours = evaluate_tours(rb)
    failures: list[str] = []

    graph_rows = psql(
        "SELECT tsp_instance_id, count_of_stops, count_of_required_stops, "
        "count_of_travel_edges, expected_undirected_edge_count, is_complete_undirected_graph "
        "FROM vw_tsp_instances ORDER BY tsp_instance_id"
    )
    if len(graph_rows) != 1:
        failures.append(f"[graph] expected 1 instance row, got {len(graph_rows)}")
    else:
        iid, stops, required, edges, expected, complete = graph_rows[0].split(",")
        pg_graph = {
            "count_of_stops": int(stops),
            "count_of_required_stops": int(required),
            "count_of_travel_edges": int(edges),
            "expected_undirected_edge_count": int(expected),
            "is_complete_undirected_graph": pg_bool(complete),
        }
        if pg_graph != py_graph.get(iid):
            failures.append(f"[graph-equivalence] postgres={pg_graph} python={py_graph.get(iid)}")

    tour_rows = psql(
        "SELECT candidate_tour_id, required_stop_count, count_of_tour_stops, "
        "count_of_tour_legs, total_travel_cost, is_hamiltonian_cycle_witness, "
        "is_optimality_proved, residual_claim "
        "FROM vw_candidate_tours ORDER BY candidate_tour_id"
    )
    agreements = 0
    for row in tour_rows:
        (
            tid,
            required,
            stop_count,
            leg_count,
            cost,
            valid,
            optimal,
            claim,
        ) = row.split(",")
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
            agreements += 1
        else:
            failures.append(f"[tour-equivalence] {tid}: postgres row={row!r}, python={py!r}")

    missing = set(py_tours) - {row.split(",", 1)[0] for row in tour_rows}
    if missing:
        failures.append(f"[tour] missing Postgres candidates: {sorted(missing)}")

    invariant_rows = psql(
        "SELECT tsp_invariant_check_id, is_passing "
        "FROM vw_tsp_invariant_checks ORDER BY tsp_invariant_check_id"
    )
    invariant_passes = sum(pg_bool(row.split(",")[1]) for row in invariant_rows)
    if invariant_passes != len(invariant_rows):
        failures.append(f"[invariants] {invariant_passes}/{len(invariant_rows)} passing")

    metric_rows = psql(
        "SELECT search_metric_id, branch_count_before, branch_count_after, "
        "search_elimination_pct, residual_ambiguity_count "
        "FROM vw_search_metrics ORDER BY search_metric_id"
    )
    if metric_rows != ["search-baseline-gridville-5,12,12,0.00,12"] and metric_rows != ["search-baseline-gridville-5,12,12,0,12"]:
        failures.append(f"[search-metrics] unexpected baseline rows: {metric_rows}")

    print(f"graph substrate agreement: {0 if failures and any('[graph' in f for f in failures) else 1}/1")
    print(f"tour substrate agreement: {agreements}/{len(py_tours)}")
    print(f"rulebook invariants passing: {invariant_passes}/{len(invariant_rows)}")
    print("search elimination: 0% (12 -> 12)")
    print("optimality claims: 0")

    if failures:
        print("\nFAIL:")
        for failure in failures:
            print("  " + failure)
        raise SystemExit(1)
    print("\ntraveling-salesman Postgres conformance: PASS")


if __name__ == "__main__":
    main()
