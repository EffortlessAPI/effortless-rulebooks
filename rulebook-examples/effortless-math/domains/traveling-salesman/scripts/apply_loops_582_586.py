#!/usr/bin/env python3
"""Apply TSP semantic loops 582-586 to the canonical rulebook.

The migration is intentionally one-shot and fails if the canonical rulebook is not at loop 581. It upgrades the initial representation
with:

582  a typed frontier-obligation ledger;
583  a global cycle-coverage certificate and adversarial tour witness;
584  canonical edge-pair completeness and a count-preserving broken graph witness;
585  local degree-two lower-bound witnesses;
586  a bound-equality optimality certificate for the Gridville five-stop fixture.

This script is a migration/replay artifact. The resulting rulebook remains the
canonical semantic asset.
"""
from __future__ import annotations

import hashlib
import json
import os
import subprocess
from pathlib import Path
from typing import Any

HERE = Path(__file__).resolve().parent
PROJECT = HERE.parent
RULEBOOK = PROJECT / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = PROJECT / "problem-contract.json"
DOMAIN_CLAUDE = PROJECT / "CLAUDE.md"
README = PROJECT / "README.md"
PARENT_CLAUDE = PROJECT.parents[1] / "CLAUDE.md"
FORMATTER = Path(
    os.environ.get(
        "TSP_FORMATTER_PATH",
        str(PROJECT.parents[1] / "scripts" / "format-rulebook.py"),
    )
)


def load_json(path: Path) -> dict[str, Any]:
    if not path.is_file():
        raise FileNotFoundError(f"missing required file: {path}")
    return json.loads(path.read_text())


def schema_field(
    name: str,
    datatype: str,
    field_type: str,
    nullable: bool,
    description: str,
    *,
    formula: str | None = None,
    related_to: str | None = None,
) -> dict[str, Any]:
    row: dict[str, Any] = {
        "name": name,
        "datatype": datatype,
        "type": field_type,
        "nullable": nullable,
        "Description": description,
    }
    if formula is not None:
        row["formula"] = formula
    if related_to is not None:
        row["RelatedTo"] = related_to
    return row


def table(description: str, schema: list[dict[str, Any]], data: list[dict[str, Any]]) -> dict[str, Any]:
    return {"Description": description, "schema": schema, "data": data}


def insert_table_after(rb: dict[str, Any], after: str, name: str, value: dict[str, Any]) -> None:
    if name in rb:
        rb[name] = value
        return
    rebuilt: dict[str, Any] = {}
    inserted = False
    for key, current in rb.items():
        rebuilt[key] = current
        if key == after:
            rebuilt[name] = value
            inserted = True
    if not inserted:
        raise KeyError(f"cannot insert {name}: missing anchor table {after}")
    rb.clear()
    rb.update(rebuilt)


def upsert_schema_fields(table_obj: dict[str, Any], fields: list[dict[str, Any]], *, before: str | None = None) -> None:
    existing = {field["name"]: field for field in table_obj["schema"]}
    for field in fields:
        existing[field["name"]] = field
    ordered: list[dict[str, Any]] = []
    inserted = False
    new_names = {field["name"] for field in fields}
    for old in table_obj["schema"]:
        if before is not None and old["name"] == before and not inserted:
            ordered.extend(existing[name] for name in [f["name"] for f in fields])
            inserted = True
        if old["name"] not in new_names:
            ordered.append(existing[old["name"]])
    if not inserted:
        ordered.extend(existing[name] for name in [f["name"] for f in fields])
    table_obj["schema"] = ordered


def upsert_rows(table_obj: dict[str, Any], id_field: str, rows: list[dict[str, Any]]) -> None:
    by_id = {row[id_field]: row for row in table_obj["data"]}
    for row in rows:
        by_id[row[id_field]] = row
    original_ids = [row[id_field] for row in table_obj["data"]]
    ordered_ids = original_ids + [row[id_field] for row in rows if row[id_field] not in original_ids]
    table_obj["data"] = [by_id[row_id] for row_id in ordered_ids]


def meta_row(rb: dict[str, Any], key: str) -> dict[str, Any]:
    for row in rb["__meta__"]["data"]:
        if row["MetaKey"] == key:
            return row
    raise KeyError(f"missing __meta__ row: {key}")


def add_or_replace_meta(rb: dict[str, Any], row: dict[str, Any]) -> None:
    rows = rb["__meta__"]["data"]
    for index, current in enumerate(rows):
        if current["MetaKey"] == row["MetaKey"]:
            rows[index] = row
            return
    rows.append(row)


def apply_rulebook_migration(rb: dict[str, Any]) -> None:
    current_last = meta_row(rb, "last_loop")["IntegerValue"]
    if current_last != 581:
        raise AssertionError(f"expected last_loop 581 before migration, found {current_last}")

    rb["Description"] = (
        "Traveling Salesman research domain at city/neighborhood/address scale. "
        "The rulebook normalizes finite weighted city graphs, validates explicit ordered-tour witnesses, "
        "records typed frontier obligations, derives a degree-two lower bound, and certifies the supplied "
        "Gridville route as optimal for its declared finite instance without claiming a general solver."
    )
    meta_row(rb, "last_loop")["IntegerValue"] = 586
    # Preserve the universal non-claim while making the finite certificate explicit.
    for row in rb["__meta__"]["data"]:
        if row["MetaKey"] == "claims_optimality":
            row["MetaKey"] = "claims_general_optimality"
            row["BooleanValue"] = False
    add_or_replace_meta(
        rb,
        {
            "MetaKey": "claims_gridville_5_optimality",
            "ValueType": "boolean",
            "StringValue": None,
            "IntegerValue": None,
            "BooleanValue": True,
        },
    )
    add_or_replace_meta(
        rb,
        {
            "MetaKey": "active_imported_dependency_count",
            "ValueType": "integer",
            "StringValue": None,
            "IntegerValue": 0,
            "BooleanValue": None,
        },
    )

    # Loop 584: pair identity is first-class data, independently checked against endpoints.
    upsert_schema_fields(
        rb["TravelEdges"],
        [
            schema_field(
                "CanonicalPairKey", "string", "raw", False,
                "Canonical unordered endpoint identity supplied as data and checked against the two endpoint rows.",
            ),
            schema_field(
                "ExpectedCanonicalPairKey", "string", "calculated", True,
                "Endpoint-derived canonical pair key used to verify the supplied pair identity.",
                formula='=IF({{FromStop}} < {{ToStop}}, CONCAT({{FromStop}}, "|", {{ToStop}}), CONCAT({{ToStop}}, "|", {{FromStop}}))',
            ),
            schema_field(
                "IsCanonicalPairKeyCorrect", "boolean", "calculated", True,
                "Whether CanonicalPairKey agrees with the sorted endpoint identities.",
                formula="={{CanonicalPairKey}} = {{ExpectedCanonicalPairKey}}",
            ),
            schema_field(
                "IsCanonicalEndpointOrder", "boolean", "calculated", True,
                "Whether the stored FromStop/ToStop order is the declared canonical lexical order.",
                formula="={{FromStop}} < {{ToStop}}",
            ),
            schema_field(
                "PairMultiplicity", "integer", "aggregation", True,
                "Number of edge rows in this instance carrying the same canonical unordered pair key.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstance}}, TravelEdges!{{CanonicalPairKey}}, {{CanonicalPairKey}})",
            ),
            schema_field(
                "IsUniqueCanonicalPair", "boolean", "calculated", True,
                "Whether exactly one edge row represents this unordered stop pair in the instance.",
                formula="={{PairMultiplicity}} = 1",
            ),
        ],
        before="FromTSPInstance",
    )
    is_admissible = next(f for f in rb["TravelEdges"]["schema"] if f["name"] == "IsAdmissible")
    is_admissible["Description"] = (
        "First-order edge admissibility including availability, instance membership, canonical endpoint order, "
        "correct pair identity, and pair uniqueness."
    )
    is_admissible["formula"] = (
        "=AND({{IsAvailable}}, NOT({{IsSelfLoop}}), {{IsWithinInstance}}, "
        "{{IsCanonicalEndpointOrder}}, {{IsCanonicalPairKeyCorrect}}, {{IsUniqueCanonicalPair}})"
    )

    for edge in rb["TravelEdges"]["data"]:
        if "CanonicalPairKey" not in edge:
            edge["CanonicalPairKey"] = "|".join(sorted((edge["FromStop"], edge["ToStop"])))

    # Incident degree is reusable geometry for lower bounds and later forced-edge closure.
    upsert_schema_fields(
        rb["InstanceStops"],
        [
            schema_field(
                "CountOfCanonicalEdgesFrom", "integer", "aggregation", True,
                "Canonical edge rows whose first endpoint is this stop.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstance}}, TravelEdges!{{FromStop}}, {{InstanceStopId}})",
            ),
            schema_field(
                "CountOfCanonicalEdgesTo", "integer", "aggregation", True,
                "Canonical edge rows whose second endpoint is this stop.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstance}}, TravelEdges!{{ToStop}}, {{InstanceStopId}})",
            ),
            schema_field(
                "CountOfIncidentEdges", "integer", "calculated", True,
                "Total canonical incident edge rows at this stop.",
                formula="={{CountOfCanonicalEdgesFrom}} + {{CountOfCanonicalEdgesTo}}",
            ),
            schema_field(
                "CountOfAdmissibleEdgesFrom", "integer", "aggregation", True,
                "Admissible edge rows whose first endpoint is this stop.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstance}}, TravelEdges!{{FromStop}}, {{InstanceStopId}}, TravelEdges!{{IsAdmissible}}, TRUE())",
            ),
            schema_field(
                "CountOfAdmissibleEdgesTo", "integer", "aggregation", True,
                "Admissible edge rows whose second endpoint is this stop.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstance}}, TravelEdges!{{ToStop}}, {{InstanceStopId}}, TravelEdges!{{IsAdmissible}}, TRUE())",
            ),
            schema_field(
                "CountOfAdmissibleIncidentEdges", "integer", "calculated", True,
                "Total currently admissible edges incident to this stop.",
                formula="={{CountOfAdmissibleEdgesFrom}} + {{CountOfAdmissibleEdgesTo}}",
            ),
        ],
    )

    upsert_schema_fields(
        rb["TSPInstances"],
        [
            schema_field(
                "CountOfInadmissibleEdges", "integer", "aggregation", True,
                "Edge rows failing canonical pair, endpoint, availability, or instance checks.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstanceId}}, TravelEdges!{{IsAdmissible}}, FALSE())",
            ),
            schema_field(
                "CountOfNonUniqueEdgePairRows", "integer", "aggregation", True,
                "Rows participating in a duplicated canonical unordered edge pair.",
                formula="=COUNTIFS(TravelEdges!{{TSPInstance}}, {{TSPInstanceId}}, TravelEdges!{{IsUniqueCanonicalPair}}, FALSE())",
            ),
        ],
        before="ExpectedUndirectedEdgeCount",
    )
    complete = next(f for f in rb["TSPInstances"]["schema"] if f["name"] == "IsCompleteUndirectedGraph")
    complete["Description"] = (
        "Whether the instance has the expected edge count, no inadmissible rows, and no duplicated unordered pair rows."
    )
    complete["formula"] = (
        "=AND({{CountOfTravelEdges}} = {{ExpectedUndirectedEdgeCount}}, "
        "{{CountOfInadmissibleEdges}} = 0, {{CountOfNonUniqueEdgePairRows}} = 0)"
    )

    # Count-preserving broken graph: three rows for three stops, but AB is duplicated and BC is absent.
    upsert_rows(
        rb["TSPInstances"],
        "TSPInstanceId",
        [
            {
                "TSPInstanceId": "tsp-gridville-broken-3",
                "DisplayName": "Gridville Three-Stop Duplicate-Pair Counterexample",
                "City": "gridville",
                "DepotAddress": "addr-a-depot",
                "DistanceModel": "EXPLICIT_SYMMETRIC_COST",
                "IsSymmetric": True,
                "Status": "NEGATIVE_NORMALIZATION_FIXTURE",
                "SearchPolicy": "NO_SEARCH_GRAPH_VALIDATION_ONLY",
                "Notes": "Three edge rows equal n(n-1)/2, but AB appears twice and BC is missing; count alone must not certify completeness.",
            }
        ],
    )
    broken_stops = [
        {"InstanceStopId": "broken-stop-a", "TSPInstance": "tsp-gridville-broken-3", "Address": "addr-a-depot", "IsRequired": True},
        {"InstanceStopId": "broken-stop-b", "TSPInstance": "tsp-gridville-broken-3", "Address": "addr-b-market", "IsRequired": True},
        {"InstanceStopId": "broken-stop-c", "TSPInstance": "tsp-gridville-broken-3", "Address": "addr-c-north", "IsRequired": True},
    ]
    upsert_rows(rb["InstanceStops"], "InstanceStopId", broken_stops)
    broken_edges = [
        {
            "TravelEdgeId": "broken-edge-a-b-1", "TSPInstance": "tsp-gridville-broken-3",
            "FromStop": "broken-stop-a", "ToStop": "broken-stop-b",
            "CanonicalPairKey": "broken-stop-a|broken-stop-b", "DistanceMeters": 1000,
            "TravelCost": 1, "IsAvailable": True, "EdgeSource": "NEGATIVE_FIXTURE",
        },
        {
            "TravelEdgeId": "broken-edge-a-b-2", "TSPInstance": "tsp-gridville-broken-3",
            "FromStop": "broken-stop-a", "ToStop": "broken-stop-b",
            "CanonicalPairKey": "broken-stop-a|broken-stop-b", "DistanceMeters": 2000,
            "TravelCost": 2, "IsAvailable": True, "EdgeSource": "NEGATIVE_FIXTURE",
        },
        {
            "TravelEdgeId": "broken-edge-a-c", "TSPInstance": "tsp-gridville-broken-3",
            "FromStop": "broken-stop-a", "ToStop": "broken-stop-c",
            "CanonicalPairKey": "broken-stop-a|broken-stop-c", "DistanceMeters": 3000,
            "TravelCost": 3, "IsAvailable": True, "EdgeSource": "NEGATIVE_FIXTURE",
        },
    ]
    upsert_rows(rb["TravelEdges"], "TravelEdgeId", broken_edges)

    graph_invariants = table(
        "Graph-normalization acceptance checks, including a count-preserving duplicate-pair counterexample.",
        [
            schema_field("TSPGraphInvariantCheckId", "string", "raw", False, "Stable graph-invariant identifier."),
            schema_field("Name", "string", "calculated", True, "Human-readable invariant label.", formula="={{DisplayName}}"),
            schema_field("DisplayName", "string", "raw", False, "Human-readable graph-invariant name."),
            schema_field("TSPInstance", "string", "relationship", False, "Instance under graph-normalization test.", related_to="TSPInstances"),
            schema_field("ExpectedCompleteUndirectedGraph", "boolean", "raw", False, "Expected graph-completeness status."),
            schema_field("ActualCompleteUndirectedGraph", "boolean", "lookup", True, "Derived graph-completeness status.", formula="=INDEX(TSPInstances!{{IsCompleteUndirectedGraph}}, MATCH({{TSPInstance}}, TSPInstances!{{TSPInstanceId}}, 0))", related_to="TSPInstances"),
            schema_field("ExpectedTravelEdgeCount", "integer", "raw", False, "Expected raw edge-row count."),
            schema_field("ActualTravelEdgeCount", "integer", "lookup", True, "Derived raw edge-row count.", formula="=INDEX(TSPInstances!{{CountOfTravelEdges}}, MATCH({{TSPInstance}}, TSPInstances!{{TSPInstanceId}}, 0))", related_to="TSPInstances"),
            schema_field("ExpectedNonUniqueEdgePairRows", "integer", "raw", False, "Expected number of rows participating in duplicated pair keys."),
            schema_field("ActualNonUniqueEdgePairRows", "integer", "lookup", True, "Derived duplicated-pair row count.", formula="=INDEX(TSPInstances!{{CountOfNonUniqueEdgePairRows}}, MATCH({{TSPInstance}}, TSPInstances!{{TSPInstanceId}}, 0))", related_to="TSPInstances"),
            schema_field("IsPassing", "boolean", "calculated", True, "Whether all graph expectations agree with derived values.", formula="=AND({{ExpectedCompleteUndirectedGraph}} = {{ActualCompleteUndirectedGraph}}, {{ExpectedTravelEdgeCount}} = {{ActualTravelEdgeCount}}, {{ExpectedNonUniqueEdgePairRows}} = {{ActualNonUniqueEdgePairRows}})"),
        ],
        [
            {
                "TSPGraphInvariantCheckId": "check-gridville-complete-graph",
                "DisplayName": "Gridville five-stop graph has one edge per unordered pair",
                "TSPInstance": "tsp-gridville-5",
                "ExpectedCompleteUndirectedGraph": True,
                "ExpectedTravelEdgeCount": 10,
                "ExpectedNonUniqueEdgePairRows": 0,
            },
            {
                "TSPGraphInvariantCheckId": "check-broken-count-preserving-graph",
                "DisplayName": "Expected edge count cannot hide a duplicated pair and omitted pair",
                "TSPInstance": "tsp-gridville-broken-3",
                "ExpectedCompleteUndirectedGraph": False,
                "ExpectedTravelEdgeCount": 3,
                "ExpectedNonUniqueEdgePairRows": 2,
            },
        ],
    )
    insert_table_after(rb, "TSPInstances", "TSPGraphInvariantChecks", graph_invariants)

    # Loop 583: global cycle coverage, not merely locally valid leg rows.
    upsert_schema_fields(
        rb["TourStops"],
        [
            schema_field(
                "CountOfOutgoingLegs", "integer", "aggregation", True,
                "Tour-leg rows in this candidate whose FromTourStop is this ordered visit.",
                formula="=COUNTIFS(TourLegs!{{CandidateTour}}, {{CandidateTour}}, TourLegs!{{FromTourStop}}, {{TourStopId}})",
            ),
            schema_field(
                "CountOfIncomingLegs", "integer", "aggregation", True,
                "Tour-leg rows in this candidate whose ToTourStop is this ordered visit.",
                formula="=COUNTIFS(TourLegs!{{CandidateTour}}, {{CandidateTour}}, TourLegs!{{ToTourStop}}, {{TourStopId}})",
            ),
            schema_field(
                "HasExactlyOneOutgoingLeg", "boolean", "calculated", True,
                "Whether this ordered visit has exactly one outgoing candidate leg.",
                formula="={{CountOfOutgoingLegs}} = 1",
            ),
            schema_field(
                "HasExactlyOneIncomingLeg", "boolean", "calculated", True,
                "Whether this ordered visit has exactly one incoming candidate leg.",
                formula="={{CountOfIncomingLegs}} = 1",
            ),
            schema_field(
                "IsCycleDegreeSatisfied", "boolean", "calculated", True,
                "Whether this ordered visit participates in exactly one incoming and one outgoing transition.",
                formula="=AND({{HasExactlyOneOutgoingLeg}}, {{HasExactlyOneIncomingLeg}})",
            ),
        ],
    )
    upsert_schema_fields(
        rb["TourLegs"],
        [
            schema_field(
                "LegOrderMultiplicity", "integer", "aggregation", True,
                "Number of leg rows in this candidate carrying the same LegOrder.",
                formula="=COUNTIFS(TourLegs!{{CandidateTour}}, {{CandidateTour}}, TourLegs!{{LegOrder}}, {{LegOrder}})",
            ),
            schema_field(
                "IsUniqueLegOrder", "boolean", "calculated", True,
                "Whether this leg order appears exactly once in the candidate.",
                formula="={{LegOrderMultiplicity}} = 1",
            ),
        ],
        before="IsCandidateMembershipValid",
    )
    leg_valid = next(f for f in rb["TourLegs"]["schema"] if f["name"] == "IsValid")
    leg_valid["Description"] = (
        "Whether the leg passes candidate membership, sequence, endpoint, instance, admissibility, and unique-order checks."
    )
    leg_valid["formula"] = (
        "=AND({{IsCandidateMembershipValid}}, {{IsSequenceTransition}}, {{IsEdgeEndpointMatch}}, "
        "{{IsInTourInstance}}, {{EdgeIsAdmissible}}, {{IsUniqueLegOrder}})"
    )

    upsert_schema_fields(
        rb["CandidateTours"],
        [
            schema_field(
                "CountOfCycleDegreeViolations", "integer", "aggregation", True,
                "Ordered visits lacking exactly one incoming and one outgoing candidate leg.",
                formula="=COUNTIFS(TourStops!{{CandidateTour}}, {{CandidateTourId}}, TourStops!{{IsCycleDegreeSatisfied}}, FALSE())",
            ),
            schema_field(
                "CountOfNonUniqueLegOrders", "integer", "aggregation", True,
                "Leg rows whose LegOrder is duplicated within the candidate.",
                formula="=COUNTIFS(TourLegs!{{CandidateTour}}, {{CandidateTourId}}, TourLegs!{{IsUniqueLegOrder}}, FALSE())",
            ),
        ],
        before="TotalTravelCost",
    )
    hamiltonian = next(f for f in rb["CandidateTours"]["schema"] if f["name"] == "IsHamiltonianCycleWitness")
    hamiltonian["Description"] = (
        "Whether the supplied ordered rows form one globally covered Hamiltonian cycle for the declared instance."
    )
    hamiltonian["formula"] = (
        "=AND({{CountOfTourStops}} = {{RequiredStopCount}}, {{CountOfTourLegs}} = {{RequiredStopCount}}, "
        "{{CountOfNonUniqueVisits}} = 0, {{CountOfNonUniqueSequences}} = 0, "
        "{{CountOfOutOfInstanceStops}} = 0, {{CountOfNonRequiredStops}} = 0, "
        "{{CountOfDepotStops}} = 1, {{CountOfDepotPositionViolations}} = 0, "
        "{{CountOfInvalidLegs}} = 0, {{CountOfCycleDegreeViolations}} = 0, "
        "{{CountOfNonUniqueLegOrders}} = 0)"
    )

    # The old local-leg checks would accept every individual row in this candidate;
    # the new global degree certificate must reject its duplicated A->B and missing E->A transition.
    upsert_rows(
        rb["CandidateTours"],
        "CandidateTourId",
        [
            {
                "CandidateTourId": "tour-missing-close-duplicate-ab",
                "DisplayName": "Counterexample with duplicated A-B and missing E-A",
                "TSPInstance": "tsp-gridville-5",
                "CandidateKind": "NEGATIVE_GLOBAL_CYCLE_WITNESS",
                "SearchBranchesExplored": 0,
                "BacktrackCount": 0,
            }
        ],
    )
    global_bad_stops = [
        {"TourStopId": f"gap-stop-{i}", "CandidateTour": "tour-missing-close-duplicate-ab", "InstanceStop": stop, "SequencePosition": i}
        for i, stop in enumerate(["stop-a", "stop-b", "stop-c", "stop-d", "stop-e"], start=1)
    ]
    upsert_rows(rb["TourStops"], "TourStopId", global_bad_stops)
    global_bad_legs = [
        {"TourLegId": "gap-leg-1", "CandidateTour": "tour-missing-close-duplicate-ab", "FromTourStop": "gap-stop-1", "ToTourStop": "gap-stop-2", "TravelEdge": "edge-a-b", "LegOrder": 1},
        {"TourLegId": "gap-leg-2", "CandidateTour": "tour-missing-close-duplicate-ab", "FromTourStop": "gap-stop-1", "ToTourStop": "gap-stop-2", "TravelEdge": "edge-a-b", "LegOrder": 2},
        {"TourLegId": "gap-leg-3", "CandidateTour": "tour-missing-close-duplicate-ab", "FromTourStop": "gap-stop-2", "ToTourStop": "gap-stop-3", "TravelEdge": "edge-b-c", "LegOrder": 3},
        {"TourLegId": "gap-leg-4", "CandidateTour": "tour-missing-close-duplicate-ab", "FromTourStop": "gap-stop-3", "ToTourStop": "gap-stop-4", "TravelEdge": "edge-c-d", "LegOrder": 4},
        {"TourLegId": "gap-leg-5", "CandidateTour": "tour-missing-close-duplicate-ab", "FromTourStop": "gap-stop-4", "ToTourStop": "gap-stop-5", "TravelEdge": "edge-d-e", "LegOrder": 5},
    ]
    upsert_rows(rb["TourLegs"], "TourLegId", global_bad_legs)
    upsert_rows(
        rb["TSPInvariantChecks"],
        "TSPInvariantCheckId",
        [
            {
                "TSPInvariantCheckId": "check-missing-close-duplicate-transition",
                "DisplayName": "Locally legal legs cannot hide duplicated and missing global transitions",
                "CandidateTour": "tour-missing-close-duplicate-ab",
                "ExpectedHamiltonianCycleWitness": False,
                "ExpectedOptimalityProved": False,
                "ExpectedTotalTravelCost": 13,
            }
        ],
    )

    # Loop 585: explicit local two-cheapest incident-edge witnesses.
    local_bounds = table(
        "One degree-two lower-bound witness per required stop. Each row names two incident edges and proves all other incident edges are no cheaper than the second selected edge.",
        [
            schema_field("LocalDegreeBoundId", "string", "raw", False, "Stable local-bound identifier."),
            schema_field("Name", "string", "calculated", True, "Display alias for the local bound.", formula="={{LocalDegreeBoundId}}"),
            schema_field("TSPInstance", "string", "relationship", False, "Instance whose tour degree requirement is being bounded.", related_to="TSPInstances"),
            schema_field("InstanceStop", "string", "relationship", False, "Required stop receiving the local degree-two bound.", related_to="InstanceStops"),
            schema_field("FirstEdge", "string", "relationship", False, "Cheapest selected incident edge.", related_to="TravelEdges"),
            schema_field("SecondEdge", "string", "relationship", False, "Second-cheapest selected incident edge.", related_to="TravelEdges"),
            schema_field("StopTSPInstance", "string", "lookup", True, "Instance owning the bounded stop.", formula="=INDEX(InstanceStops!{{TSPInstance}}, MATCH({{InstanceStop}}, InstanceStops!{{InstanceStopId}}, 0))", related_to="InstanceStops"),
            schema_field("StopAdmissibleIncidentEdgeCount", "integer", "lookup", True, "Current admissible incident-edge count at the stop.", formula="=INDEX(InstanceStops!{{CountOfAdmissibleIncidentEdges}}, MATCH({{InstanceStop}}, InstanceStops!{{InstanceStopId}}, 0))", related_to="InstanceStops"),
            schema_field("FirstEdgeFromStop", "string", "lookup", True, "First selected edge's canonical first endpoint.", formula="=INDEX(TravelEdges!{{FromStop}}, MATCH({{FirstEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("FirstEdgeToStop", "string", "lookup", True, "First selected edge's canonical second endpoint.", formula="=INDEX(TravelEdges!{{ToStop}}, MATCH({{FirstEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("SecondEdgeFromStop", "string", "lookup", True, "Second selected edge's canonical first endpoint.", formula="=INDEX(TravelEdges!{{FromStop}}, MATCH({{SecondEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("SecondEdgeToStop", "string", "lookup", True, "Second selected edge's canonical second endpoint.", formula="=INDEX(TravelEdges!{{ToStop}}, MATCH({{SecondEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("FirstEdgeCost", "number", "lookup", True, "Cost of the first selected edge.", formula="=INDEX(TravelEdges!{{TravelCost}}, MATCH({{FirstEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("SecondEdgeCost", "number", "lookup", True, "Cost of the second selected edge.", formula="=INDEX(TravelEdges!{{TravelCost}}, MATCH({{SecondEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("IsFirstEdgeIncident", "boolean", "calculated", True, "Whether the first selected edge is incident to the bounded stop.", formula="=OR({{InstanceStop}} = {{FirstEdgeFromStop}}, {{InstanceStop}} = {{FirstEdgeToStop}})"),
            schema_field("IsSecondEdgeIncident", "boolean", "calculated", True, "Whether the second selected edge is incident to the bounded stop.", formula="=OR({{InstanceStop}} = {{SecondEdgeFromStop}}, {{InstanceStop}} = {{SecondEdgeToStop}})"),
            schema_field("AreSelectedEdgesDistinct", "boolean", "calculated", True, "Whether the two selected edge rows are distinct.", formula="={{FirstEdge}} <> {{SecondEdge}}"),
            schema_field("AreSelectedCostsOrdered", "boolean", "calculated", True, "Whether the first selected edge is no more expensive than the second.", formula="={{FirstEdgeCost}} <= {{SecondEdgeCost}}"),
            schema_field("RequiredDominanceCheckCount", "integer", "calculated", True, "Number of other incident edges that must be compared against the second selected cost.", formula="={{StopAdmissibleIncidentEdgeCount}} - 2"),
            schema_field("CountOfDominanceChecks", "integer", "aggregation", True, "Represented comparisons against other incident edges.", formula="=COUNTIFS(IncidentDominanceChecks!{{LocalDegreeBound}}, {{LocalDegreeBoundId}})"),
            schema_field("CountOfFailedDominanceChecks", "integer", "aggregation", True, "Dominance comparisons that fail incidence, distinctness, uniqueness, or cost ordering.", formula="=COUNTIFS(IncidentDominanceChecks!{{LocalDegreeBound}}, {{LocalDegreeBoundId}}, IncidentDominanceChecks!{{IsPassing}}, FALSE())"),
            schema_field("IsTwoCheapestWitness", "boolean", "calculated", True, "Whether this row and its child comparisons certify the two cheapest admissible incident edges.", formula="=AND({{TSPInstance}} = {{StopTSPInstance}}, {{IsFirstEdgeIncident}}, {{IsSecondEdgeIncident}}, {{AreSelectedEdgesDistinct}}, {{AreSelectedCostsOrdered}}, {{CountOfDominanceChecks}} = {{RequiredDominanceCheckCount}}, {{CountOfFailedDominanceChecks}} = 0)"),
            schema_field("LocalBoundCost", "number", "calculated", True, "Certified sum of the two cheapest incident edge costs; zero when the witness is invalid.", formula="=IF({{IsTwoCheapestWitness}}, {{FirstEdgeCost}} + {{SecondEdgeCost}}, 0)"),
        ],
        [
            {"LocalDegreeBoundId": "local-bound-stop-a", "TSPInstance": "tsp-gridville-5", "InstanceStop": "stop-a", "FirstEdge": "edge-a-b", "SecondEdge": "edge-a-e"},
            {"LocalDegreeBoundId": "local-bound-stop-b", "TSPInstance": "tsp-gridville-5", "InstanceStop": "stop-b", "FirstEdge": "edge-a-b", "SecondEdge": "edge-b-c"},
            {"LocalDegreeBoundId": "local-bound-stop-c", "TSPInstance": "tsp-gridville-5", "InstanceStop": "stop-c", "FirstEdge": "edge-b-c", "SecondEdge": "edge-c-d"},
            {"LocalDegreeBoundId": "local-bound-stop-d", "TSPInstance": "tsp-gridville-5", "InstanceStop": "stop-d", "FirstEdge": "edge-c-d", "SecondEdge": "edge-d-e"},
            {"LocalDegreeBoundId": "local-bound-stop-e", "TSPInstance": "tsp-gridville-5", "InstanceStop": "stop-e", "FirstEdge": "edge-a-e", "SecondEdge": "edge-d-e"},
        ],
    )
    insert_table_after(rb, "TourLegs", "LocalDegreeBounds", local_bounds)

    dominance_checks = table(
        "Witness rows proving every unselected incident edge is at least as expensive as the second edge named by a LocalDegreeBound.",
        [
            schema_field("IncidentDominanceCheckId", "string", "raw", False, "Stable dominance-check identifier."),
            schema_field("Name", "string", "calculated", True, "Display alias for the dominance check.", formula="={{IncidentDominanceCheckId}}"),
            schema_field("LocalDegreeBound", "string", "relationship", False, "Local lower-bound witness being supported.", related_to="LocalDegreeBounds"),
            schema_field("OtherEdge", "string", "relationship", False, "Unselected incident edge compared against the second selected edge.", related_to="TravelEdges"),
            schema_field("BoundStop", "string", "lookup", True, "Stop bounded by the parent witness.", formula="=INDEX(LocalDegreeBounds!{{InstanceStop}}, MATCH({{LocalDegreeBound}}, LocalDegreeBounds!{{LocalDegreeBoundId}}, 0))", related_to="LocalDegreeBounds"),
            schema_field("FirstSelectedEdge", "string", "lookup", True, "First edge selected by the parent witness.", formula="=INDEX(LocalDegreeBounds!{{FirstEdge}}, MATCH({{LocalDegreeBound}}, LocalDegreeBounds!{{LocalDegreeBoundId}}, 0))", related_to="LocalDegreeBounds"),
            schema_field("SecondSelectedEdge", "string", "lookup", True, "Second edge selected by the parent witness.", formula="=INDEX(LocalDegreeBounds!{{SecondEdge}}, MATCH({{LocalDegreeBound}}, LocalDegreeBounds!{{LocalDegreeBoundId}}, 0))", related_to="LocalDegreeBounds"),
            schema_field("SecondSelectedEdgeCost", "number", "lookup", True, "Cost of the second selected edge.", formula="=INDEX(LocalDegreeBounds!{{SecondEdgeCost}}, MATCH({{LocalDegreeBound}}, LocalDegreeBounds!{{LocalDegreeBoundId}}, 0))", related_to="LocalDegreeBounds"),
            schema_field("OtherEdgeFromStop", "string", "lookup", True, "Other edge's first endpoint.", formula="=INDEX(TravelEdges!{{FromStop}}, MATCH({{OtherEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("OtherEdgeToStop", "string", "lookup", True, "Other edge's second endpoint.", formula="=INDEX(TravelEdges!{{ToStop}}, MATCH({{OtherEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("OtherEdgeCost", "number", "lookup", True, "Cost of the unselected incident edge.", formula="=INDEX(TravelEdges!{{TravelCost}}, MATCH({{OtherEdge}}, TravelEdges!{{TravelEdgeId}}, 0))", related_to="TravelEdges"),
            schema_field("OtherEdgeMultiplicity", "integer", "aggregation", True, "Number of comparison rows in the same local witness naming this other edge.", formula="=COUNTIFS(IncidentDominanceChecks!{{LocalDegreeBound}}, {{LocalDegreeBound}}, IncidentDominanceChecks!{{OtherEdge}}, {{OtherEdge}})"),
            schema_field("IsOtherEdgeIncident", "boolean", "calculated", True, "Whether the compared edge is incident to the bounded stop.", formula="=OR({{BoundStop}} = {{OtherEdgeFromStop}}, {{BoundStop}} = {{OtherEdgeToStop}})"),
            schema_field("IsOtherEdgeUnselected", "boolean", "calculated", True, "Whether the compared edge differs from both selected edges.", formula="=AND({{OtherEdge}} <> {{FirstSelectedEdge}}, {{OtherEdge}} <> {{SecondSelectedEdge}})"),
            schema_field("IsOtherEdgeUnique", "boolean", "calculated", True, "Whether this unselected edge appears exactly once among the parent's comparisons.", formula="={{OtherEdgeMultiplicity}} = 1"),
            schema_field("IsDominatedBySecondCost", "boolean", "calculated", True, "Whether the unselected edge is no cheaper than the second selected edge.", formula="={{OtherEdgeCost}} >= {{SecondSelectedEdgeCost}}"),
            schema_field("IsPassing", "boolean", "calculated", True, "Whether the comparison is incident, unselected, unique, and cost-dominated.", formula="=AND({{IsOtherEdgeIncident}}, {{IsOtherEdgeUnselected}}, {{IsOtherEdgeUnique}}, {{IsDominatedBySecondCost}})"),
        ],
        [
            {"IncidentDominanceCheckId": "dom-a-d", "LocalDegreeBound": "local-bound-stop-a", "OtherEdge": "edge-a-d"},
            {"IncidentDominanceCheckId": "dom-a-c", "LocalDegreeBound": "local-bound-stop-a", "OtherEdge": "edge-a-c"},
            {"IncidentDominanceCheckId": "dom-b-d", "LocalDegreeBound": "local-bound-stop-b", "OtherEdge": "edge-b-d"},
            {"IncidentDominanceCheckId": "dom-b-e", "LocalDegreeBound": "local-bound-stop-b", "OtherEdge": "edge-b-e"},
            {"IncidentDominanceCheckId": "dom-c-e", "LocalDegreeBound": "local-bound-stop-c", "OtherEdge": "edge-c-e"},
            {"IncidentDominanceCheckId": "dom-c-a", "LocalDegreeBound": "local-bound-stop-c", "OtherEdge": "edge-a-c"},
            {"IncidentDominanceCheckId": "dom-d-a", "LocalDegreeBound": "local-bound-stop-d", "OtherEdge": "edge-a-d"},
            {"IncidentDominanceCheckId": "dom-d-b", "LocalDegreeBound": "local-bound-stop-d", "OtherEdge": "edge-b-d"},
            {"IncidentDominanceCheckId": "dom-e-c", "LocalDegreeBound": "local-bound-stop-e", "OtherEdge": "edge-c-e"},
            {"IncidentDominanceCheckId": "dom-e-b", "LocalDegreeBound": "local-bound-stop-e", "OtherEdge": "edge-b-e"},
        ],
    )
    insert_table_after(rb, "LocalDegreeBounds", "IncidentDominanceChecks", dominance_checks)

    instance_bounds = table(
        "Global lower bounds obtained by summing certified local degree-two bounds and dividing by two because each tour edge is incident to two stops.",
        [
            schema_field("InstanceLowerBoundId", "string", "raw", False, "Stable instance-lower-bound identifier."),
            schema_field("Name", "string", "calculated", True, "Display alias for the lower bound.", formula="={{InstanceLowerBoundId}}"),
            schema_field("TSPInstance", "string", "relationship", False, "Instance receiving the lower bound.", related_to="TSPInstances"),
            schema_field("BoundKind", "string", "raw", False, "Kind of reusable lower-bound geometry."),
            schema_field("RequiredStopCount", "integer", "lookup", True, "Required stop count of the instance.", formula="=INDEX(TSPInstances!{{CountOfRequiredStops}}, MATCH({{TSPInstance}}, TSPInstances!{{TSPInstanceId}}, 0))", related_to="TSPInstances"),
            schema_field("CountOfLocalDegreeBounds", "integer", "aggregation", True, "Local degree-bound rows represented for the instance.", formula="=COUNTIFS(LocalDegreeBounds!{{TSPInstance}}, {{TSPInstance}})"),
            schema_field("CountOfInvalidLocalDegreeBounds", "integer", "aggregation", True, "Local degree-bound rows that do not carry a valid two-cheapest witness.", formula="=COUNTIFS(LocalDegreeBounds!{{TSPInstance}}, {{TSPInstance}}, LocalDegreeBounds!{{IsTwoCheapestWitness}}, FALSE())"),
            schema_field("TotalLocalDegreeBoundCost", "number", "aggregation", True, "Sum of the local two-edge lower bounds across all required stops.", formula="=SUMIFS(LocalDegreeBounds!{{LocalBoundCost}}, LocalDegreeBounds!{{TSPInstance}}, {{TSPInstance}})"),
            schema_field("LowerBoundCost", "number", "calculated", True, "Certified global tour lower bound after double-count correction.", formula="={{TotalLocalDegreeBoundCost}} / 2"),
            schema_field("IsCertified", "boolean", "calculated", True, "Whether every required stop has one valid local witness and the global double-counting aggregation is closed.", formula="=AND({{CountOfLocalDegreeBounds}} = {{RequiredStopCount}}, {{CountOfInvalidLocalDegreeBounds}} = 0)"),
        ],
        [
            {
                "InstanceLowerBoundId": "degree-two-lower-bound-gridville-5",
                "TSPInstance": "tsp-gridville-5",
                "BoundKind": "SUM_TWO_CHEAPEST_INCIDENT_EDGES_DIVIDED_BY_TWO",
            }
        ],
    )
    insert_table_after(rb, "IncidentDominanceChecks", "InstanceLowerBounds", instance_bounds)

    # Loop 586: equality of a certified lower bound and a valid candidate cost.
    optimality = table(
        "Finite-instance optimality certificates. Passing requires a valid Hamiltonian witness, a certified lower bound for the same instance, and equality between candidate cost and lower bound.",
        [
            schema_field("OptimalityCertificateId", "string", "raw", False, "Stable optimality-certificate identifier."),
            schema_field("Name", "string", "calculated", True, "Display alias for the certificate.", formula="={{OptimalityCertificateId}}"),
            schema_field("CandidateTour", "string", "relationship", False, "Candidate whose optimality is being certified.", related_to="CandidateTours"),
            schema_field("InstanceLowerBound", "string", "relationship", False, "Certified lower bound compared with the candidate cost.", related_to="InstanceLowerBounds"),
            schema_field("CandidateTSPInstance", "string", "lookup", True, "Instance owned by the candidate.", formula="=INDEX(CandidateTours!{{TSPInstance}}, MATCH({{CandidateTour}}, CandidateTours!{{CandidateTourId}}, 0))", related_to="CandidateTours"),
            schema_field("LowerBoundTSPInstance", "string", "lookup", True, "Instance owned by the lower-bound certificate.", formula="=INDEX(InstanceLowerBounds!{{TSPInstance}}, MATCH({{InstanceLowerBound}}, InstanceLowerBounds!{{InstanceLowerBoundId}}, 0))", related_to="InstanceLowerBounds"),
            schema_field("CandidateTravelCost", "number", "lookup", True, "Derived travel cost of the candidate.", formula="=INDEX(CandidateTours!{{TotalTravelCost}}, MATCH({{CandidateTour}}, CandidateTours!{{CandidateTourId}}, 0))", related_to="CandidateTours"),
            schema_field("LowerBoundCost", "number", "lookup", True, "Certified lower-bound cost.", formula="=INDEX(InstanceLowerBounds!{{LowerBoundCost}}, MATCH({{InstanceLowerBound}}, InstanceLowerBounds!{{InstanceLowerBoundId}}, 0))", related_to="InstanceLowerBounds"),
            schema_field("CandidateIsHamiltonianCycleWitness", "boolean", "lookup", True, "Whether the candidate is a valid Hamiltonian-cycle witness.", formula="=INDEX(CandidateTours!{{IsHamiltonianCycleWitness}}, MATCH({{CandidateTour}}, CandidateTours!{{CandidateTourId}}, 0))", related_to="CandidateTours"),
            schema_field("LowerBoundIsCertified", "boolean", "lookup", True, "Whether the lower-bound construction is certified.", formula="=INDEX(InstanceLowerBounds!{{IsCertified}}, MATCH({{InstanceLowerBound}}, InstanceLowerBounds!{{InstanceLowerBoundId}}, 0))", related_to="InstanceLowerBounds"),
            schema_field("IsSameInstance", "boolean", "calculated", True, "Whether candidate and lower bound refer to the same TSP instance.", formula="={{CandidateTSPInstance}} = {{LowerBoundTSPInstance}}"),
            schema_field("IsBoundTight", "boolean", "calculated", True, "Whether the candidate cost equals the certified lower bound.", formula="={{CandidateTravelCost}} = {{LowerBoundCost}}"),
            schema_field("IsPassing", "boolean", "calculated", True, "Whether bound equality certifies this candidate as optimal for the declared finite instance.", formula="=AND({{CandidateIsHamiltonianCycleWitness}}, {{LowerBoundIsCertified}}, {{IsSameInstance}}, {{IsBoundTight}})"),
            schema_field("ScopeClaim", "string", "calculated", True, "Honest scope of the certificate.", formula='=IF({{IsPassing}}, "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE", "OPTIMALITY_NOT_CERTIFIED")'),
        ],
        [
            {
                "OptimalityCertificateId": "optimality-gridville-reference-ring",
                "CandidateTour": "tour-reference-ring",
                "InstanceLowerBound": "degree-two-lower-bound-gridville-5",
            }
        ],
    )
    insert_table_after(rb, "InstanceLowerBounds", "OptimalityCertificates", optimality)

    upsert_schema_fields(
        rb["CandidateTours"],
        [
            schema_field(
                "CountOfPassingOptimalityCertificates", "integer", "aggregation", True,
                "Passing finite-instance optimality certificates attached to this candidate.",
                formula="=COUNTIFS(OptimalityCertificates!{{CandidateTour}}, {{CandidateTourId}}, OptimalityCertificates!{{IsPassing}}, TRUE())",
            ),
        ],
        before="IsOptimalityProved",
    )
    optimal_field = next(f for f in rb["CandidateTours"]["schema"] if f["name"] == "IsOptimalityProved")
    optimal_field["Description"] = (
        "Whether at least one passing finite-instance optimality certificate closes this candidate's scope."
    )
    optimal_field["formula"] = (
        "=AND({{IsHamiltonianCycleWitness}}, {{CountOfPassingOptimalityCertificates}} > 0)"
    )
    residual = next(f for f in rb["CandidateTours"]["schema"] if f["name"] == "ResidualClaim")
    residual["formula"] = (
        '=IF({{IsOptimalityProved}}, "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE", '
        'IF({{IsHamiltonianCycleWitness}}, "VALID_TOUR_NOT_OPTIMALITY_PROOF", "INVALID_TOUR"))'
    )

    # Typed frontier vocabulary: imported dependency is only one possible obligation kind.
    frontier = table(
        "Typed ledger of open and closed semantic frontier obligations. Imported dependencies are used only when an external provider conclusion is actually consumed.",
        [
            schema_field("TSPFrontierObligationId", "string", "raw", False, "Stable frontier-obligation identifier."),
            schema_field("Name", "string", "calculated", True, "Human-readable obligation label.", formula="={{DisplayName}}"),
            schema_field("DisplayName", "string", "raw", False, "Human-readable frontier-obligation name."),
            schema_field("ObligationKind", "string", "raw", False, "Typed class: imported dependency, inference, certificate, substrate, generalization, or residual search."),
            schema_field("Status", "string", "raw", False, "OPEN, CLOSED, BLOCKED, DEFERRED, or FALSIFIED."),
            schema_field("InferenceRule", "string", "relationship", True, "Inference contract primarily responsible for closure.", related_to="TSPInferenceRules"),
            schema_field("OpenedByLoop", "string", "relationship", False, "Loop that exposed this frontier obligation.", related_to="TSPLoops"),
            schema_field("ClosedByLoop", "string", "relationship", True, "Loop that closed the obligation, when closed.", related_to="TSPLoops"),
            schema_field("TrustDisposition", "string", "raw", False, "How the obligation is expected to close or where trust remains."),
            schema_field("ClosureCriterion", "string", "raw", False, "Concrete, auditable condition required for closure."),
            schema_field("CertificateType", "string", "raw", False, "Certificate expected or emitted by closure."),
            schema_field("IsImportedDependency", "boolean", "calculated", True, "Whether this row consumes an external provider conclusion.", formula='={{ObligationKind}} = "IMPORTED_DEPENDENCY"'),
            schema_field("IsClosed", "boolean", "calculated", True, "Whether the frontier obligation is closed.", formula='={{Status}} = "CLOSED"'),
        ],
        [
            {"TSPFrontierObligationId": "frontier-global-cycle-coverage", "DisplayName": "Global transition coverage", "ObligationKind": "INFERENCE_OBLIGATION", "Status": "CLOSED", "InferenceRule": "tsp-rule-global-cycle-coverage", "OpenedByLoop": "tsp-loop-580", "ClosedByLoop": "tsp-loop-583", "TrustDisposition": "INTERNALIZED_FROM_ORDERED_CYCLE_DEFINITION", "ClosureCriterion": "Every visit has exactly one incoming and one outgoing valid transition; adversarial duplicated-transition witness is rejected.", "CertificateType": "global-cycle-coverage-certificate"},
            {"TSPFrontierObligationId": "frontier-canonical-edge-pairs", "DisplayName": "Canonical unordered edge-pair completeness", "ObligationKind": "INFERENCE_OBLIGATION", "Status": "CLOSED", "InferenceRule": "tsp-rule-weighted-edge-normalization", "OpenedByLoop": "tsp-loop-578", "ClosedByLoop": "tsp-loop-584", "TrustDisposition": "INTERNALIZED_FROM_PAIR_IDENTITY_AND_MULTIPLICITY", "ClosureCriterion": "Expected edge count, pair-key correctness, pair uniqueness, endpoint order, and negative duplicate-pair fixture all agree.", "CertificateType": "complete-undirected-graph-certificate"},
            {"TSPFrontierObligationId": "frontier-postgres-commissioning", "DisplayName": "Live Postgres build and conformance", "ObligationKind": "SUBSTRATE_OBLIGATION", "Status": "OPEN", "InferenceRule": None, "OpenedByLoop": "tsp-loop-580", "ClosedByLoop": None, "TrustDisposition": "EXECUTE_GENERATED_PROJECTION_AND_COMPARE", "ClosureCriterion": "effortless build, database recreation, and all vw_* versus Python conformance checks pass with recorded artifact hashes.", "CertificateType": "substrate-conformance-certificate"},
            {"TSPFrontierObligationId": "frontier-degree-two-lower-bound", "DisplayName": "Degree-two local-to-global lower bound", "ObligationKind": "INFERENCE_OBLIGATION", "Status": "CLOSED", "InferenceRule": "tsp-rule-degree-two-lower-bound", "OpenedByLoop": "tsp-loop-581", "ClosedByLoop": "tsp-loop-585", "TrustDisposition": "INTERNALIZED_FROM_HAMILTONIAN_DEGREE_AND_DOUBLE_COUNTING", "ClosureCriterion": "Two cheapest incident edges are witnessed at every required stop and half their summed cost is emitted as a certified instance lower bound.", "CertificateType": "degree-two-lower-bound-certificate"},
            {"TSPFrontierObligationId": "frontier-gridville-optimality", "DisplayName": "Gridville finite-instance optimality", "ObligationKind": "CERTIFICATE_OBLIGATION", "Status": "CLOSED", "InferenceRule": "tsp-rule-bound-equality-optimality", "OpenedByLoop": "tsp-loop-580", "ClosedByLoop": "tsp-loop-586", "TrustDisposition": "CLOSED_BY_BOUND_EQUALITY", "ClosureCriterion": "A valid candidate cost equals a certified lower bound for the same finite instance.", "CertificateType": "finite-instance-optimality-certificate"},
            {"TSPFrontierObligationId": "frontier-route-reconstruction", "DisplayName": "Route reconstruction from forced structural edges", "ObligationKind": "CERTIFICATE_OBLIGATION", "Status": "OPEN", "InferenceRule": "tsp-rule-route-reconstruction", "OpenedByLoop": "tsp-loop-586", "ClosedByLoop": None, "TrustDisposition": "INTERNALIZE_WITH_CONNECTED_DEGREE_TWO_EDGE_SET", "ClosureCriterion": "Derive the cycle edge set from inference rows, prove one connected component with no proper subtour, then project ordered tour rows.", "CertificateType": "route-reconstruction-certificate"},
            {"TSPFrontierObligationId": "frontier-neighborhood-contraction", "DisplayName": "Neighborhood boundary-state contraction", "ObligationKind": "GENERALIZATION_OBLIGATION", "Status": "OPEN", "InferenceRule": "tsp-rule-neighborhood-boundary-state", "OpenedByLoop": "tsp-loop-577", "ClosedByLoop": None, "TrustDisposition": "EMPIRICAL_RESEARCH_FRONTIER", "ClosureCriterion": "Represent finite boundary states, prove contraction soundness, and measure branch reduction across multiple clustered fixtures.", "CertificateType": "cluster-boundary-state-certificate"},
            {"TSPFrontierObligationId": "frontier-residual-route-discovery-search", "DisplayName": "Residual route-discovery ambiguity", "ObligationKind": "RESIDUAL_SEARCH", "Status": "OPEN", "InferenceRule": "tsp-rule-degree-two-forcing", "OpenedByLoop": "tsp-loop-581", "ClosedByLoop": None, "TrustDisposition": "SEARCH_ONLY_AFTER_CONSTRAINT_CLOSURE", "ClosureCriterion": "All represented deterministic inference reaches closure; any remaining route classes are explicitly branched with replayable branch certificates.", "CertificateType": "residual-search-certificate"},
        ],
    )
    insert_table_after(rb, "TSPInferenceRules", "TSPFrontierObligations", frontier)

    # Inference-rule and loop ledger updates.
    upsert_rows(
        rb["TSPInferenceRules"],
        "TSPInferenceRuleId",
        [
            {"TSPInferenceRuleId": "tsp-rule-frontier-obligation-ledger", "DisplayName": "Typed frontier-obligation ledger", "InferenceLayer": "SEMANTIC_NORMALIZATION", "ImplementationStatus": "EXECUTABLE", "Soundness": "STRUCTURAL_STATUS_ACCOUNTING", "Completeness": "COMPLETE_FOR_CURRENT_DECLARED_FRONTIER", "RuntimeClass": "O(F)", "MemoryClass": "O(F)", "Applicability": "Every unresolved semantic edge is assigned one obligation kind and one explicit closure criterion.", "CertificateType": "frontier-ledger-certificate", "Description": "Distinguishes external imports from internal inference, certificate, substrate, generalization, and residual-search obligations."},
            {"TSPInferenceRuleId": "tsp-rule-global-cycle-coverage", "DisplayName": "Global cycle-transition coverage", "InferenceLayer": "CONSTRAINT_CLOSURE", "ImplementationStatus": "EXECUTABLE", "Soundness": "SOUND_FOR_EXPLICIT_ORDERED_TOUR_WITNESSES", "Completeness": "COMPLETE_FOR_GLOBAL_COVERAGE_OF_SUPPLIED_ORDERED_CYCLE_ROWS", "RuntimeClass": "O(V+E_TOUR)", "MemoryClass": "O(V+E_TOUR)", "Applicability": "Candidate has unique ordered visits and edge-bound leg rows.", "CertificateType": "global-cycle-coverage-certificate", "Description": "Requires every ordered visit to have exactly one incoming and one outgoing valid transition and every leg order to be unique."},
            {"TSPInferenceRuleId": "tsp-rule-degree-two-lower-bound", "DisplayName": "Degree-two incident-edge lower bound", "InferenceLayer": "SECOND_ORDER_INFERENCE", "ImplementationStatus": "EXECUTABLE", "Soundness": "SOUND_BY_HAMILTONIAN_DEGREE_AND_DOUBLE_COUNTING", "Completeness": "LOWER_BOUND_ONLY", "RuntimeClass": "O(VE)_FOR_EXPLICIT_WITNESS_ROWS", "MemoryClass": "O(VE)", "Applicability": "Finite symmetric weighted graph with two witnessed cheapest admissible incident edges at every required stop.", "CertificateType": "degree-two-lower-bound-certificate", "Description": "Sums each stop's two cheapest incident costs, then divides by two because every tour edge contributes at both endpoints."},
            {"TSPInferenceRuleId": "tsp-rule-bound-equality-optimality", "DisplayName": "Bound-equality finite optimality", "InferenceLayer": "RIGIDITY", "ImplementationStatus": "EXECUTABLE", "Soundness": "SOUND_FOR_CERTIFIED_LOWER_BOUND_AND_VALID_CANDIDATE", "Completeness": "SUFFICIENT_NOT_NECESSARY", "RuntimeClass": "O(1)_AFTER_CERTIFICATES", "MemoryClass": "O(1)", "Applicability": "Candidate is Hamiltonian, lower bound is certified for the same instance, and candidate cost equals the bound.", "CertificateType": "finite-instance-optimality-certificate", "Description": "Closes optimality for one declared finite instance without enumerating all route classes."},
            {"TSPInferenceRuleId": "tsp-rule-route-reconstruction", "DisplayName": "Route reconstruction from inferred edge set", "InferenceLayer": "SPECIALIZATION", "ImplementationStatus": "PLANNED", "Soundness": "UNASSESSED", "Completeness": "UNKNOWN", "RuntimeClass": "TARGET_O(V+E)", "MemoryClass": "TARGET_O(V+E)", "Applicability": "Inference has selected a connected degree-two edge set with no proper subtour.", "CertificateType": "route-reconstruction-certificate", "Description": "Future rule: order a structurally derived cycle rather than consuming a supplied candidate route."},
        ],
    )
    weighted = next(r for r in rb["TSPInferenceRules"]["data"] if r["TSPInferenceRuleId"] == "tsp-rule-weighted-edge-normalization")
    weighted["Soundness"] = "SOUND_FOR_DECLARED_SYMMETRIC_GRAPH_WITH_PAIR_IDENTITY_CHECKS"
    weighted["Completeness"] = "COMPLETE_WHEN_EACH_UNORDERED_REQUIRED_STOP_PAIR_HAS_EXACTLY_ONE_ADMISSIBLE_ROW"
    tour_rule = next(r for r in rb["TSPInferenceRules"]["data"] if r["TSPInferenceRuleId"] == "tsp-rule-tour-witness-validation")
    tour_rule["Description"] = (
        "Derives supplied-tour validity from visit uniqueness, sequence uniqueness, depot placement, edge membership, "
        "global one-in/one-out coverage, unique leg order, and route closure."
    )

    upsert_rows(
        rb["TSPLoops"],
        "TSPLoopId",
        [
            {"TSPLoopId": "tsp-loop-582", "LoopOrder": 582, "DisplayName": "Frontier obligation vocabulary", "Status": "CLOSED", "PrimaryInferenceRule": "tsp-rule-frontier-obligation-ledger", "NewConcept": "Typed frontier obligations distinct from imported dependencies and residual search", "WitnessSummary": "Eight current frontier rows classify internal inference, certificates, substrate commissioning, generalization, and residual search; active imported dependencies remain zero.", "NextFrontier": "Harden supplied-cycle validity from local leg checks to global transition coverage."},
            {"TSPLoopId": "tsp-loop-583", "LoopOrder": 583, "DisplayName": "Global cycle coverage", "Status": "CLOSED", "PrimaryInferenceRule": "tsp-rule-global-cycle-coverage", "NewConcept": "Exactly one incoming and outgoing transition per ordered visit", "WitnessSummary": "A candidate with five unique visits and five locally legal legs is rejected because A-B is duplicated and E-A is missing.", "NextFrontier": "Prove graph completeness by unordered pair identity, not edge count alone."},
            {"TSPLoopId": "tsp-loop-584", "LoopOrder": 584, "DisplayName": "Canonical edge-pair completeness", "Status": "CLOSED", "PrimaryInferenceRule": "tsp-rule-weighted-edge-normalization", "NewConcept": "Canonical pair key, pair multiplicity, and duplicate-pair negative graph", "WitnessSummary": "A three-stop graph with the expected three rows is rejected because AB occurs twice and BC is absent.", "NextFrontier": "Construct a reusable lower bound from the Hamiltonian degree-two requirement."},
            {"TSPLoopId": "tsp-loop-585", "LoopOrder": 585, "DisplayName": "Degree-two lower bound", "Status": "CLOSED", "PrimaryInferenceRule": "tsp-rule-degree-two-lower-bound", "NewConcept": "Local two-cheapest incident-edge witnesses and global double-counting bound", "WitnessSummary": "Local bounds 5,5,6,6,6 sum to 28; dividing by two certifies every Gridville tour costs at least 14.", "NextFrontier": "Compare the certified lower bound with a valid candidate upper bound."},
            {"TSPLoopId": "tsp-loop-586", "LoopOrder": 586, "DisplayName": "Gridville finite optimality", "Status": "CLOSED", "PrimaryInferenceRule": "tsp-rule-bound-equality-optimality", "NewConcept": "Finite-instance optimality by lower-bound equality", "WitnessSummary": "The valid reference tour costs 14 and the certified lower bound is 14, so it is optimal for tsp-gridville-5 without route enumeration.", "NextFrontier": "Reconstruct the cycle from inferred structural edges rather than a supplied route."},
        ],
    )

    # Search accounting separates proof verification from route discovery.
    upsert_schema_fields(
        rb["SearchMetrics"],
        [
            schema_field("SearchQuestion", "string", "raw", False, "Exact question whose remaining branch count is being measured."),
            schema_field("CandidateTour", "string", "relationship", True, "Candidate involved when measuring verification rather than route discovery.", related_to="CandidateTours"),
        ],
        before="RawNodeCount",
    )
    baseline = next(r for r in rb["SearchMetrics"]["data"] if r["SearchMetricId"] == "search-baseline-gridville-5")
    baseline["SearchQuestion"] = "DISCOVER_ROUTE_WITHOUT_SUPPLIED_CANDIDATE"
    baseline["CandidateTour"] = None
    baseline["Status"] = "ROUTE_DISCOVERY_BASELINE_NO_CLOSURE"
    upsert_rows(
        rb["SearchMetrics"],
        "SearchMetricId",
        [
            {
                "SearchMetricId": "search-optimality-verification-gridville-5",
                "TSPInstance": "tsp-gridville-5",
                "TSPLoop": "tsp-loop-586",
                "SearchQuestion": "VERIFY_OPTIMALITY_OF_SUPPLIED_CANDIDATE",
                "CandidateTour": "tour-reference-ring",
                "RawNodeCount": 5,
                "RawEdgeCount": 10,
                "ForcedEdgeCount": 0,
                "ForbiddenEdgeCount": 0,
                "SymmetryClassCount": 12,
                "ComponentContractionCount": 0,
                "BranchCountBefore": 12,
                "BranchCountAfter": 0,
                "BacktrackCount": 0,
                "ResidualAmbiguityCount": 0,
                "Status": "OPTIMALITY_VERIFICATION_CLOSED_BY_BOUND_EQUALITY",
            }
        ],
    )

    # Invariant expectations now include the finite optimality certificate.
    reference_check = next(r for r in rb["TSPInvariantChecks"]["data"] if r["TSPInvariantCheckId"] == "check-reference-route")
    reference_check["ExpectedOptimalityProved"] = True
    reference_check["DisplayName"] = "Reference route validates and matches the certified finite-instance lower bound"


def update_contract(rb_hash: str) -> None:
    contract = load_json(CONTRACT)
    contract.update(
        {
            "Version": "0.2.0",
            "Status": "RESEARCH_PROGRAM",
            "Statement": (
                "Given finite city-scale weighted graphs, represent graph normalization, supplied tours, typed frontier obligations, "
                "local-to-global lower bounds, finite optimality certificates, and residual search as executable structural data."
            ),
        }
    )
    contract["ArtifactHashes"] = {"rulebook": f"sha256:{rb_hash}"}
    contract["GeneratedTimestamp"] = None
    CONTRACT.write_text(json.dumps(contract, indent=2) + "\n")


def main() -> None:
    rb = load_json(RULEBOOK)
    apply_rulebook_migration(rb)
    RULEBOOK.write_text(json.dumps(rb, indent=2) + "\n")
    if not FORMATTER.is_file():
        raise FileNotFoundError(f"missing canonical formatter: {FORMATTER}")
    subprocess.run(["python3", str(FORMATTER), str(RULEBOOK)], check=True)
    rb_hash = hashlib.sha256(RULEBOOK.read_bytes()).hexdigest()
    update_contract(rb_hash)
    print(f"applied TSP loops 582-586; rulebook sha256:{rb_hash}")


if __name__ == "__main__":
    main()
