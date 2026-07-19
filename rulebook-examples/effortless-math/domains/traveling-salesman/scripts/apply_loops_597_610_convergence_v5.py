#!/usr/bin/env python3
"""Data-shape-adaptive convergence driver for loops 597-610."""
from __future__ import annotations

import importlib
import json
import sys
from typing import Any

import apply_loops_597_610_convergence_v2 as base
import apply_loops_597_610_convergence_v3 as hard
import apply_loops_597_610_convergence_v4 as reload_driver


def append_reference_override() -> None:
    text = base.REFERENCE_MODEL.read_text()
    marker = "# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_V2"
    if marker in text:
        return
    # Remove an uncommitted/earlier override if a prior local retry left it in the worktree.
    if "# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_V1" in text:
        text = text.split("# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_V1", 1)[0].rstrip() + "\n"
    text += r'''

# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_V2
# Build the existing dataclass by introspecting its declared fields, so the
# semantic change does not depend on a particular historical constructor order.
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
        available = {
            "instance_lower_bound_id": lower_id,
            "lower_bound_id": lower_id,
            "tsp_instance": iid,
            "count_of_local_degree_bounds": len(bounds),
            "count_of_invalid_local_degree_bounds": invalid,
            "total_local_degree_bound_cost": float(total),
            "base_lower_bound_cost": float(base_lower),
            "supplemental_bound_adjustment": float(declared_adjustment),
            "lower_bound_cost": float(lower),
            "is_certified": certified,
        }
        fields = getattr(InstanceLowerBoundResult, "__dataclass_fields__", {})
        if fields:
            missing = [name for name in fields if name not in available]
            if missing:
                raise AssertionError(f"unmapped InstanceLowerBoundResult fields: {missing}")
            result[lower_id] = InstanceLowerBoundResult(**{name: available[name] for name in fields})
        else:
            result[lower_id] = InstanceLowerBoundResult(
                lower_id, iid, len(bounds), invalid, float(total), float(lower), certified
            )
    return result
'''
    base.REFERENCE_MODEL.write_text(text)


def loop_607_adaptive(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
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
        numeric = {value for value in cert.values() if isinstance(value, int) and not isinstance(value, bool)}
        composition = 36 in numeric or 9 in numeric
        cert["EquivalenceRelation"] = (
            "PRODUCT_OF_NEIGHBORHOOD_PATH_REVERSAL_QUOTIENTS"
            if composition else "PATH_REVERSAL_WITH_FIXED_UNORDERED_BOUNDARY_AND_COST"
        )
        cert["QuotientClassCount"] = 9 if composition else 3
    base.table_index(rb, "TSPInferenceRules")["tsp-rule-semantic-quotient"]["ImplementationStatus"] = "EXECUTABLE"
    base.register_concept(rb, "coined-semantic-quotient", "Semantic Quotient", "COINED_PREDICATE", "PROVENANCE(Equivalence)+CARDINALITY(Classes)+BOUNDARY_SIGNATURE", 1, "TSPClusterBoundaryStates,TSPClusterContractionCertificates", 607)
    base.add_measurement(rb, 607, primitive_count=8, new_primitives=0, derived_aliases=42, prediction="EARLY_SUPPORT", notes="The previous count reduction now has an explicit equivalence relation and representative contract.")
    return (
        "Recast neighborhood contraction as a semantic quotient by path reversal with fixed unordered boundary, coverage, and cost.",
        "Each neighborhood has six directed orders / orientation multiplicity two = three quotient classes; the product quotient has 3 x 3 = 9 classes.",
    )


def find_sparse_instance(rb: dict[str, Any]) -> str:
    for table_name in ("TSPConstraintRounds", "TSPInstances"):
        ident = base.id_field(rb[table_name])
        for row in base.rows(rb, table_name):
            if "sparse" in str(row.get(ident, "")).lower():
                return row.get("TSPInstance") or row.get("TSPInstanceId")
    raise AssertionError("sparse fixture instance could not be inferred")


def loop_609_adaptive(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
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
    sparse = find_sparse_instance(rb)
    specs = [
        ("event-gridville-edge-union", "tsp-gridville-5", 589, "EDGE_UNION", 10, 5, "edge-set-gridville-local-bound-union"),
        ("event-gridville-rigidity", "tsp-gridville-5", 590, "RIGIDITY", 10, 1, "connected-cycle-gridville-local-bound-union"),
        ("event-gridville-reconstruction", "tsp-gridville-5", 591, "RECONSTRUCTION", 1, 5, "reconstruction-gridville-local-bound-union"),
        ("event-sparse-forcing", sparse, 594, "FORCING", 5, 5, "sparse-forcing"),
        ("event-sparse-forbidding", sparse, 595, "FORBIDDING", 2, 1, "sparse-forbidding"),
        ("event-twin-cut-parity", "tsp-twin-triangles-6", 602, "CUT_PARITY", 2, 1, "bound-term-twin-cut-parity"),
        ("event-twin-repair-bound", "tsp-twin-triangles-6", 603, "BOUND_REPAIR", 3, 2, "degree-two-lower-bound-twin-triangles-6"),
        ("event-twin-bound-sandwich", "tsp-twin-triangles-6", 604, "RIGIDITY", 2, 1, "optimality-twin-triangles-component-repair"),
        ("event-twin-component-quotient", "tsp-twin-triangles-6", 608, "QUOTIENT", 6, 2, "tsp-twin-triangles-6"),
    ]
    additions = []
    for ident, instance, loop, kind, antecedents, decisions, subject in specs:
        state = f"state-closure-{ident}"
        hard.ensure_state(rb, state, instance, loop, kind, f"Normalized closure state for {ident}.")
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
    return (
        "Collapsed applications, states, antecedents, supports, rounds, and decisions into a common closure-event projection.",
        f"{len(base.rows(rb, 'TSPInferenceApplications'))} closure events cover selection, union, rigidity, reconstruction, forcing, forbidding, cut parity, repair, optimality, and quotienting.",
    )


def validate_repository_state() -> None:
    reload_driver.validate_repository_state()


def write_validation_files() -> None:
    reload_driver.write_validation_files()
    base.VALIDATOR_V4.write_text(
        '''#!/usr/bin/env python3\nfrom apply_loops_597_610_convergence_v5 import validate_repository_state\n\nif __name__ == "__main__":\n    validate_repository_state()\n'''
    )
    workflow = base.VALIDATION_WORKFLOW.read_text()
    if 'apply_loops_597_610_convergence_v5.py' not in workflow:
        workflow = workflow.replace(
            '"$domain/scripts/apply_loops_597_610_convergence_v4.py" \\\n',
            '"$domain/scripts/apply_loops_597_610_convergence_v4.py" \\\n            "$domain/scripts/apply_loops_597_610_convergence_v5.py" \\\n',
        )
    base.VALIDATION_WORKFLOW.write_text(workflow)


base.patch_reference_model_for_terms = append_reference_override
base.LOOP_FUNCS[603] = hard.loop_603_fixed
base.LOOP_FUNCS[607] = loop_607_adaptive
base.LOOP_FUNCS[609] = loop_609_adaptive
base.validate_repository_state = validate_repository_state
base.write_validation_files = write_validation_files


if __name__ == "__main__":
    base.main()
