#!/usr/bin/env python3
"""Hardened driver for TSP convergence loops 597-610.

This wraps the v2 migration while replacing three fragile historical surfaces:
- the old validator's loop-593 expectation that twin optimality stays open;
- a textual patch of reference_model.py;
- fixture-specific closure-state identifiers.
"""
from __future__ import annotations

import json
import re
import sys
from collections import defaultdict, deque
from pathlib import Path
from typing import Any

import apply_loops_597_610_convergence_v2 as base

ALLOWED_FUNCTIONS = {
    "ABS", "AND", "AVERAGEIFS", "COALESCE", "CONCAT", "COUNT", "COUNTIFS",
    "FALSE", "FIND", "IF", "IFERROR", "INDEX", "LEFT", "LEN", "LOG",
    "LOG10", "LOWER", "MATCH", "MAX", "MID", "MIN", "NOT", "OR",
    "POWER", "RIGHT", "ROUND", "SEARCH", "SUBSTITUTE", "SUM", "SUMIFS",
    "TEXT", "TRIM", "TRUE", "UPPER", "VALUE",
}


def append_reference_override() -> None:
    text = base.REFERENCE_MODEL.read_text()
    marker = "# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_V1"
    if marker in text:
        return
    text += r'''

# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_V1
# Later definition intentionally replaces the degree-only evaluator while
# preserving the same public result type and every historical row.
def evaluate_instance_lower_bounds(rulebook):
    graph = evaluate_graph(rulebook)
    local = evaluate_local_degree_bounds(rulebook)
    result = {}
    terms_table = rulebook.get("TSPBoundTerms", {}).get("data", [])
    for row in _rows(rulebook, "InstanceLowerBounds"):
        lower_id = row["InstanceLowerBoundId"]
        iid = row["TSPInstance"]
        bounds = [item for item in local.values() if item.tsp_instance == iid]
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
        result[lower_id] = InstanceLowerBoundResult(
            instance_lower_bound_id=lower_id,
            tsp_instance=iid,
            count_of_local_degree_bounds=len(bounds),
            count_of_invalid_local_degree_bounds=invalid,
            total_local_degree_bound_cost=float(total),
            lower_bound_cost=float(lower),
            is_certified=certified,
        )
    return result
'''
    base.REFERENCE_MODEL.write_text(text)


def loop_603_fixed(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    after, witness = base.loop_603(rb, contract)
    # Make the adjustment raw witnessed data.  TSPBoundTerms independently
    # proves that 20-2=18, and the Python peer checks equality.  This avoids
    # asking every substrate to aggregate a calculated child expression.
    for item in rb["InstanceLowerBounds"]["schema"]:
        if item["name"] == "SupplementalBoundAdjustment":
            item["type"] = "raw"
            item["nullable"] = False
            item.pop("formula", None)
    for row in base.rows(rb, "InstanceLowerBounds"):
        row["SupplementalBoundAdjustment"] = 18 if row["TSPInstance"] == "tsp-twin-triangles-6" else 0
    return after, witness


def loop_607_fixed(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    base.ensure_fields(
        rb["TSPClusterBoundaryStates"],
        [
            base.field("SemanticQuotientKey", "string", "calculated", True, "Equivalence key preserving boundary signature and cost.", formula='=CONCAT({{Neighborhood}}, "|", {{EntryStop}}, "|", {{ExitStop}}, "|", {{InternalStopCount}}, "|", {{InternalPathCost}})'),
            base.field("OrientationMultiplicity", "integer", "raw", False, "Raw directed orientations represented by the undirected state."),
            base.field("IsQuotientRepresentative", "boolean", "raw", False, "Whether this state is the canonical quotient representative."),
        ],
    )
    for state in base.rows(rb, "TSPClusterBoundaryStates"):
        state["OrientationMultiplicity"] = 2
        state["IsQuotientRepresentative"] = True
    base.ensure_fields(
        rb["TSPClusterContractionCertificates"],
        [
            base.field("EquivalenceRelation", "string", "raw", False, "Explicit semantic equivalence relation."),
            base.field("QuotientClassCount", "integer", "raw", False, "Number of equivalence classes."),
        ],
    )
    for cert in base.rows(rb, "TSPClusterContractionCertificates"):
        payload = json.dumps(cert, sort_keys=True)
        local = "gridville-west-triangle" in payload or "gridville-east-triangle" in payload
        cert["EquivalenceRelation"] = (
            "PATH_REVERSAL_WITH_FIXED_UNORDERED_BOUNDARY_AND_COST"
            if local else "PRODUCT_OF_NEIGHBORHOOD_PATH_REVERSAL_QUOTIENTS"
        )
        cert["QuotientClassCount"] = 3 if local else 9
    base.table_index(rb, "TSPInferenceRules")["tsp-rule-semantic-quotient"]["ImplementationStatus"] = "EXECUTABLE"
    base.register_concept(rb, "coined-semantic-quotient", "Semantic Quotient", "COINED_PREDICATE", "PROVENANCE(Equivalence)+CARDINALITY(Classes)+BOUNDARY_SIGNATURE", 1, "TSPClusterBoundaryStates,TSPClusterContractionCertificates", 607)
    base.add_measurement(rb, 607, primitive_count=8, new_primitives=0, derived_aliases=42, prediction="EARLY_SUPPORT", notes="The previous count reduction now has an explicit equivalence relation and representative contract.")
    after = "Recast neighborhood contraction as a semantic quotient by path reversal with fixed unordered boundary, coverage, and cost."
    witness = "Each neighborhood has six directed orders / orientation multiplicity two = three quotient classes; the product quotient has 3 x 3 = 9 classes."
    return after, witness


def ensure_state(rb: dict[str, Any], ident: str, instance: str, loop: int, kind: str, description: str) -> None:
    base.upsert_rows(
        rb["TSPInferenceStates"],
        "TSPInferenceStateId",
        [{
            "TSPInferenceStateId": ident,
            "TSPInstance": instance,
            "TSPLoop": f"tsp-loop-{loop}",
            "StateKind": kind,
            "ParentStateId": None,
            "Status": "CLOSED",
            "Description": description,
        }],
    )


def loop_609_fixed(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    base.ensure_fields(
        rb["TSPInferenceApplications"],
        [
            base.field("EventKind", "string", "raw", True, "Normalized closure-event kind."),
            base.field("InputStateId", "string", "raw", True, "Input state identifier."),
            base.field("OutputStateId", "string", "raw", True, "Output state identifier."),
            base.field("AntecedentCount", "integer", "raw", True, "Antecedent fact count."),
            base.field("DecisionCount", "integer", "raw", True, "Decisions emitted by the event."),
            base.field("EventStatus", "string", "raw", True, "APPLIED, FIXED_POINT, or REJECTED."),
        ],
    )
    for app in base.rows(rb, "TSPInferenceApplications"):
        app["EventKind"] = app.get("EventKind") or "LOCAL_SELECTION"
        app["InputStateId"] = app.get("InputStateId") or "local-degree-bound-witness"
        app["OutputStateId"] = app.get("OutputStateId") or app["InferenceState"]
        app["AntecedentCount"] = app.get("AntecedentCount") or 1
        app["DecisionCount"] = app.get("DecisionCount") or 2
        app["EventStatus"] = app.get("EventStatus") or "APPLIED"
    specs = [
        ("event-gridville-edge-union", "tsp-gridville-5", 589, "EDGE_UNION", 10, 5, "edge-set-gridville-local-bound-union"),
        ("event-gridville-rigidity", "tsp-gridville-5", 590, "RIGIDITY", 10, 1, "connected-cycle-gridville-local-bound-union"),
        ("event-gridville-reconstruction", "tsp-gridville-5", 591, "RECONSTRUCTION", 1, 5, "reconstruction-gridville-local-bound-union"),
        ("event-sparse-forcing", "tsp-sparse-degree-two-5", 594, "FORCING", 5, 5, "sparse-forcing"),
        ("event-sparse-forbidding", "tsp-sparse-degree-two-5", 595, "FORBIDDING", 2, 1, "sparse-forbidding"),
        ("event-twin-cut-parity", "tsp-twin-triangles-6", 602, "CUT_PARITY", 2, 1, "bound-term-twin-cut-parity"),
        ("event-twin-repair-bound", "tsp-twin-triangles-6", 603, "BOUND_REPAIR", 3, 2, "degree-two-lower-bound-twin-triangles-6"),
        ("event-twin-bound-sandwich", "tsp-twin-triangles-6", 604, "RIGIDITY", 2, 1, "optimality-twin-triangles-component-repair"),
        ("event-twin-component-quotient", "tsp-twin-triangles-6", 608, "QUOTIENT", 6, 2, "tsp-twin-triangles-6"),
    ]
    additions = []
    for ident, instance, loop, kind, antecedents, decisions, subject in specs:
        state = f"state-closure-{ident}"
        ensure_state(rb, state, instance, loop, kind, f"Normalized closure state for {ident}.")
        additions.append({
            "TSPInferenceApplicationId": ident,
            "InferenceState": state,
            "InferenceRule": "tsp-rule-closure-event",
            "TSPLoop": f"tsp-loop-{loop}",
            "SubjectType": "CLOSURE_EVENT_SUBJECT",
            "SubjectId": subject,
            "ApplicabilityPassed": True,
            "Conclusion": f"{kind} event applied with explicit input/output/provenance accounting.",
            "CertificateType": "closure-event-certificate",
            "EventKind": kind,
            "InputStateId": f"input-{ident}",
            "OutputStateId": state,
            "AntecedentCount": antecedents,
            "DecisionCount": decisions,
            "EventStatus": "APPLIED",
        })
    base.upsert_rows(rb["TSPInferenceApplications"], "TSPInferenceApplicationId", additions)
    base.table_index(rb, "TSPInferenceRules")["tsp-rule-closure-event"]["ImplementationStatus"] = "EXECUTABLE"
    base.register_concept(rb, "coined-closure-event", "Closure Event", "COINED_PREDICATE", "PROVENANCE(Rule,Input,Output)+COMMITMENT(Decisions)+CARDINALITY(Antecedents)", 1, "TSPInferenceApplications,TSPInferenceStates,TSPInferenceAntecedents,TSPEdgeSupports,TSPConstraintRounds,TSPConstraintDecisions", 609)
    for concept_id in {"surface-inference-antecedent", "surface-inference-application", "surface-certificate", "surface-trust-status"}:
        base.table_index(rb, "TSPConceptRegistry")[concept_id]["SupersededByConcept"] = "coined-closure-event"
    base.add_measurement(rb, 609, primitive_count=8, new_primitives=0, derived_aliases=44, prediction="EARLY_SUPPORT", notes="Six execution projections now share one event schema with explicit input, output, rule, antecedent count, decision count, and certificate.")
    base.add_frontier(rb, "frontier-closure-event-normalization", "Closure-event execution normal form", 588, 609, "tsp-rule-closure-event", "Representative inference and constraint steps project to one event schema without losing provenance.", "closure-event-certificate")
    after = "Collapsed applications, states, antecedents, supports, rounds, and decisions into a common closure-event projection."
    witness = f"{len(base.rows(rb, 'TSPInferenceApplications'))} closure events cover selection, union, rigidity, reconstruction, forcing, forbidding, cut parity, repair, optimality, and quotienting."
    return after, witness


def validate_core(rb: dict[str, Any]) -> None:
    tables = base.canonical_tables(rb)
    ids: dict[str, set[str]] = {}
    for name, tbl in tables.items():
        if not tbl.get("Description") or not tbl.get("schema"):
            raise AssertionError(f"{name}: incomplete table shape")
        first = tbl["schema"][0]
        if first.get("type") != "raw" or first.get("nullable") is not False:
            raise AssertionError(f"{name}: invalid identifier field")
        field_names = [item["name"] for item in tbl["schema"]]
        if len(field_names) != len(set(field_names)) or "Name" not in field_names:
            raise AssertionError(f"{name}: duplicate fields or missing Name")
        ident = first["name"]
        values = [row.get(ident) for row in tbl["data"]]
        if any(value is None for value in values) or len(values) != len(set(values)):
            raise AssertionError(f"{name}: invalid identifiers")
        ids[name] = set(values)
    graph: dict[str, set[str]] = defaultdict(set)
    for name, tbl in tables.items():
        for item in tbl["schema"]:
            if item.get("type") == "relationship":
                target = item.get("RelatedTo")
                if target not in tables or target == name:
                    raise AssertionError(f"{name}.{item['name']}: invalid relationship target {target}")
                graph[name].add(target)
                for row in tbl["data"]:
                    value = row.get(item["name"])
                    if value is None:
                        if item.get("nullable") is False:
                            raise AssertionError(f"{name}.{item['name']}: null relationship")
                    elif value not in ids[target]:
                        raise AssertionError(f"{name}.{item['name']}: missing {value!r} in {target}")
            formula = item.get("formula")
            if formula:
                if not formula.startswith("="):
                    raise AssertionError(f"{name}.{item['name']}: formula must start =")
                funcs = set(re.findall(r"(?<![A-Za-z0-9_])([A-Z][A-Z0-9_]*)\s*\(", formula))
                unknown = funcs - ALLOWED_FUNCTIONS
                if unknown:
                    raise AssertionError(f"{name}.{item['name']}: unsupported functions {sorted(unknown)}")
    indegree = {name: 0 for name in tables}
    outgoing: dict[str, set[str]] = defaultdict(set)
    for child, parents in graph.items():
        for parent in parents:
            outgoing[child].add(parent)
            indegree[parent] += 1
    queue = deque(name for name, value in indegree.items() if value == 0)
    seen = 0
    while queue:
        current = queue.popleft()
        seen += 1
        for parent in outgoing[current]:
            indegree[parent] -= 1
            if indegree[parent] == 0:
                queue.append(parent)
    if seen != len(tables):
        raise AssertionError("relationship DAG contains a cycle")


def validate_repository_state() -> None:
    sys.path.insert(0, str(base.DOMAIN / "scripts"))
    rb = base.load(base.RULEBOOK)
    contract = base.load(base.CONTRACT)
    validate_core(rb)
    base.validate_additional(rb, contract)
    from reference_model import evaluate_graph, evaluate_tours
    graph = evaluate_graph(rb)
    tours = evaluate_tours(rb)
    if not graph["tsp-gridville-5"]["is_complete_undirected_graph"]:
        raise AssertionError("Gridville graph drift")
    if not tours["tour-reference-ring"].is_optimality_proved:
        raise AssertionError("Gridville optimality drift")
    loops = {int(row["LoopOrder"]): row for row in base.rows(rb, "TSPLoops")}
    if loops.get(604, {}).get("Status") == "CLOSED" and not tours["tour-twin-triangles-feasible-24"].is_optimality_proved:
        raise AssertionError("Twin optimality did not close")
    if contract["Acceptance"]["RulebookTables"] != len(base.canonical_tables(rb)):
        raise AssertionError("contract table count drift")
    print("traveling-salesman convergence validation v3: PASS")


def write_validation_files() -> None:
    base.write_validation_files()
    base.VALIDATOR_V4.write_text(
        '''#!/usr/bin/env python3\nfrom apply_loops_597_610_convergence_v3 import validate_repository_state\n\nif __name__ == "__main__":\n    validate_repository_state()\n'''
    )
    base.VALIDATOR_WRAPPER.write_text(
        '''#!/usr/bin/env python3\nfrom validate_rulebook_v4 import validate_repository_state\nfrom validate_summary_alignment_v4 import validate_summary_alignment\n\nif __name__ == "__main__":\n    validate_repository_state()\n    validate_summary_alignment()\n'''
    )
    workflow = base.VALIDATION_WORKFLOW.read_text()
    workflow = workflow.replace(
        '"$domain/scripts/apply_loops_597_610_convergence_v2.py" \\\n',
        '"$domain/scripts/apply_loops_597_610_convergence_v2.py" \\\n            "$domain/scripts/apply_loops_597_610_convergence_v3.py" \\\n',
    )
    base.VALIDATION_WORKFLOW.write_text(workflow)


# Monkey-patch the v2 driver before invoking it.
base.patch_reference_model_for_terms = append_reference_override
base.LOOP_FUNCS[603] = loop_603_fixed
base.LOOP_FUNCS[607] = loop_607_fixed
base.LOOP_FUNCS[609] = loop_609_fixed
base.validate_repository_state = validate_repository_state
base.write_validation_files = write_validation_files


if __name__ == "__main__":
    base.main()
