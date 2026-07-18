#!/usr/bin/env python3
"""Execute TSP loops 611-623 as an atom/operator refinement experiment.

The rulebook remains canonical.  All loops are registered as PLANNED before
execution, then each loop is closed and committed separately.  The experiment
asks whether the loop-610 eight-predicate basis itself factors into a smaller
set of relational atoms plus reusable operators, then stress-tests that factor
on an asymmetric four-stop neighborhood without adding a new physical table.
"""
from __future__ import annotations

import json
import os
import subprocess
import sys
from collections import Counter, defaultdict
from pathlib import Path
from typing import Any, Iterable

import apply_loops_597_610_convergence_v2 as base

REPO = base.REPO
DOMAIN = base.DOMAIN
RULEBOOK = base.RULEBOOK
CONTRACT = base.CONTRACT
README = base.README
VALIDATOR_V3 = base.VALIDATOR_V3
VALIDATOR_V5 = DOMAIN / "scripts" / "validate_rulebook_v5.py"
SUMMARY_V5 = DOMAIN / "scripts" / "validate_summary_alignment_v5.py"
VALIDATOR_WRAPPER = base.VALIDATOR_WRAPPER
VALIDATION_WORKFLOW = base.VALIDATION_WORKFLOW
TARGET_BRANCH = os.environ.get("TARGET_BRANCH", "agent/tsp-semantic-foundation")

OLD_PRIMITIVES = {
    "concept-membership": "MEMBERSHIP",
    "concept-incidence": "INCIDENCE",
    "concept-cardinality": "CARDINALITY",
    "concept-order": "ORDER",
    "concept-weight": "WEIGHT",
    "concept-commitment": "COMMITMENT",
    "concept-connectivity": "CONNECTIVITY",
    "concept-provenance": "PROVENANCE",
}

LOOPS: dict[int, dict[str, str]] = {
    611: {
        "name": "Predicate/operator split",
        "term": "Predicate–Operator Split",
        "rule": "tsp-rule-atom-operator-refinement",
        "before": "Loop 610 records eight canonical predicates, but relational atoms and reusable closure operators are not distinguished.",
        "criterion": "Classify the eight historical predicates as candidate atoms or operator-derived concepts, preserve their loop-610 provenance, and record a reduction plan without changing the active basis yet.",
        "next": "Test whether membership and incidence share one typed attachment atom.",
    },
    612: {
        "name": "Attachment atom",
        "term": "Attachment Atom",
        "rule": "tsp-rule-atom-operator-refinement",
        "before": "Membership and incidence are separate primitives even though both attach one object to a typed scope or endpoint.",
        "criterion": "Introduce ATTACHMENT(subject, object, role), reduce MEMBERSHIP and INCIDENCE to typed attachment roles, and preserve every historical surface definition.",
        "next": "Test whether cardinality, order, and weight share one valuation atom.",
    },
    613: {
        "name": "Valuation atom",
        "term": "Valuation Atom",
        "rule": "tsp-rule-atom-operator-refinement",
        "before": "Cardinality, order, and weight remain separate primitives despite all assigning values under a named measure.",
        "criterion": "Introduce VALUATION(subject, measure, value), reduce count, ordinal, and cost predicates to valuation forms, and retain aggregation as an explicit pending operator.",
        "next": "Test whether commitment and provenance share one warrant atom.",
    },
    614: {
        "name": "Warrant atom",
        "term": "Warrant Atom",
        "rule": "tsp-rule-atom-operator-refinement",
        "before": "Commitment and provenance are separate primitives even though every commitment is a scoped conclusion warranted by sources and rules.",
        "criterion": "Introduce WARRANT(conclusion, source, modality), reduce commitment and provenance to warrant projections, and preserve necessity scope distinctions.",
        "next": "Move connectivity from the atom basis into a closure operator.",
    },
    615: {
        "name": "Closure operator",
        "term": "Closure Operator",
        "rule": "tsp-rule-operator-lift",
        "before": "Connectivity is still treated as a primitive rather than reachability closure over typed attachments.",
        "criterion": "Register CLOSURE as an operator over attachment relations, reduce CONNECTIVITY to closure, and leave exactly three active relational atoms.",
        "next": "Lift count, sum, minimum, and parity into one aggregation operator.",
    },
    616: {
        "name": "Aggregate operator",
        "term": "Aggregate Operator",
        "rule": "tsp-rule-operator-lift",
        "before": "Counts, sums, minima, degrees, and repair totals are expressed through many surface formulas without one operator identity.",
        "criterion": "Register AGGREGATE as the common count/sum/min/max/parity operator over attachments and valuations, with no new atom.",
        "next": "Lift symmetry and dominance contraction into one quotient operator.",
    },
    617: {
        "name": "Quotient operator",
        "term": "Quotient Operator",
        "rule": "tsp-rule-operator-lift",
        "before": "Semantic Quotient and Component Quotient are coined predicates but quotienting is not represented as a reusable operator.",
        "criterion": "Register QUOTIENT as the operator that identifies witnesses under an explicit equivalence relation and retains representatives plus expansion provenance.",
        "next": "Lift repeated closure rounds into one fixed-point operator.",
    },
    618: {
        "name": "Fixed-point operator",
        "term": "Fixed-Point Operator",
        "rule": "tsp-rule-operator-lift",
        "before": "Closure events and constraint rounds expose repeated application, but fixed-point iteration is not explicit in the semantic basis.",
        "criterion": "Register FIXPOINT as repeated warranted closure until no commitment changes, and map inference rounds and closure events onto it.",
        "next": "Certify the current three-atom/four-operator factorization.",
    },
    619: {
        "name": "Three-atom basis",
        "term": "Three-Atom Basis",
        "rule": "tsp-rule-atom-operator-refinement",
        "before": "Three active atoms and four operators have emerged incrementally, but their coverage of the historical vocabulary is not certified.",
        "criterion": "Certify ATTACHMENT, VALUATION, and WARRANT as the current atoms; CLOSURE, AGGREGATE, QUOTIENT, and FIXPOINT as operators; and express every registered concept without the old eight primitive tokens.",
        "next": "Stress the reduced basis on a general boundary-port interface.",
    },
    620: {
        "name": "Boundary port",
        "term": "Boundary Port",
        "rule": "tsp-rule-boundary-fiber",
        "before": "Entry and exit stops remain special cluster fields rather than typed attachments with a reusable port contract.",
        "criterion": "Represent every existing cluster path through typed entry/exit ports, a common port contract, orientation multiplicity, and a boundary-fiber key.",
        "next": "Build a non-symmetric four-stop neighborhood and group its path witnesses by boundary fiber.",
    },
    621: {
        "name": "Boundary fiber",
        "term": "Boundary Fiber",
        "rule": "tsp-rule-boundary-fiber",
        "before": "The three-stop fixture has one path per unordered boundary, so it cannot distinguish quotienting by symmetry from competing internal valuations.",
        "criterion": "Represent an asymmetric four-stop complete neighborhood with twelve reversal-quotient path states grouped into six boundary fibers, retaining twenty-four directed orientations and explicit expansion members.",
        "next": "Collapse each boundary fiber by valuation dominance.",
    },
    622: {
        "name": "Fiber minimum",
        "term": "Fiber Minimum",
        "rule": "tsp-rule-fiber-dominance",
        "before": "Each asymmetric boundary fiber contains two non-equivalent internal path valuations and no canonical survivor.",
        "criterion": "Within each of six boundary fibers, warrant the unique minimum-cost path, mark the other path dominated, and record the exact dominance delta.",
        "next": "Certify the combined reversal and valuation quotient.",
    },
    623: {
        "name": "Asymmetric semantic quotient",
        "term": "Asymmetric Quotient",
        "rule": "tsp-rule-fiber-dominance",
        "before": "The original contraction certificate only demonstrates three-stop reversal symmetry and does not test the reduced basis against asymmetric valuations.",
        "criterion": "Certify 24 directed paths to 12 reversal classes to 6 boundary-fiber minima, with expansion, coverage, and cost preservation, while the physical table count remains unchanged.",
        "next": "Generalize region repair across more than two quotient components, then expose a genuine residual branch orbit.",
    },
}

COMMIT_MESSAGES = {
    611: "TSP loop 611: split predicates from operators",
    612: "TSP loop 612: derive attachment atom",
    613: "TSP loop 613: derive valuation atom",
    614: "TSP loop 614: derive warrant atom",
    615: "TSP loop 615: lift connectivity to closure",
    616: "TSP loop 616: lift measures to aggregate",
    617: "TSP loop 617: lift contraction to quotient",
    618: "TSP loop 618: lift closure rounds to fixed point",
    619: "TSP loop 619: certify three-atom basis",
    620: "TSP loop 620: normalize boundary ports",
    621: "TSP loop 621: derive asymmetric boundary fibers",
    622: "TSP loop 622: collapse boundary fibers by value",
    623: "TSP loop 623: certify asymmetric quotient",
}

RULE_ROWS = [
    {
        "TSPInferenceRuleId": "tsp-rule-atom-operator-refinement",
        "DisplayName": "Atom/operator semantic refinement",
        "InferenceLayer": "SEMANTIC_REDUCTION",
        "ImplementationStatus": "PLANNED",
        "Soundness": "DEFINITIONAL_REFACTOR_WITH_EXPLICIT_HISTORICAL_PROJECTIONS",
        "Completeness": "EMPIRICAL_FOR_REGISTERED_CURRENT_DOMAIN",
        "RuntimeClass": "O(C)",
        "MemoryClass": "O(C)",
        "Applicability": "Registered concepts admit explicit definitions over fewer relational atoms and named operators.",
        "CertificateType": "atom-operator-basis-certificate",
        "Description": "Separate relational atoms from closure operators while preserving every historical concept as a replayable definition.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-operator-lift",
        "DisplayName": "Semantic operator lift",
        "InferenceLayer": "SEMANTIC_REDUCTION",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_WHEN_OPERATOR_INPUT_OUTPUT_AND_WARRANT_ARE_EXPLICIT",
        "Completeness": "CURRENT_CLOSURE_AGGREGATE_QUOTIENT_FIXPOINT_FAMILIES",
        "RuntimeClass": "OPERATOR_DEPENDENT",
        "MemoryClass": "OPERATOR_DEPENDENT",
        "Applicability": "A recurring derived relation is produced by a reusable transformation over typed atoms.",
        "CertificateType": "semantic-operator-certificate",
        "Description": "Promote closure, aggregation, quotient, and fixed-point transformations to reusable operators rather than primitive predicates.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-boundary-fiber",
        "DisplayName": "Boundary-fiber normalization",
        "InferenceLayer": "SEMANTIC_QUOTIENT",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_FOR_EXPLICIT_HAMILTONIAN_PATH_EXPANSIONS",
        "Completeness": "FINITE_DECLARED_CLUSTER",
        "RuntimeClass": "O(P)",
        "MemoryClass": "O(P)",
        "Applicability": "Internal path witnesses expose a scope, two ports, full coverage, edge members, order, and cost.",
        "CertificateType": "boundary-fiber-certificate",
        "Description": "Group internal path witnesses that share the same typed boundary ports and coverage scope.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-fiber-dominance",
        "DisplayName": "Boundary-fiber valuation dominance",
        "InferenceLayer": "SEMANTIC_QUOTIENT",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_FOR_EQUAL_PORT_EQUAL_COVERAGE_PATHS_WITH_ADDITIVE_COST",
        "Completeness": "FINITE_DECLARED_FIBERS",
        "RuntimeClass": "O(P_LOG_P)",
        "MemoryClass": "O(P)",
        "Applicability": "Two or more valid path witnesses share a boundary fiber and differ only in internal order and valuation.",
        "CertificateType": "fiber-minimum-certificate",
        "Description": "Retain minimum-valued representatives within each boundary fiber and preserve dominated witnesses as expandable provenance.",
    },
]


def run(cmd: list[str], *, cwd: Path = REPO, check: bool = True, capture: bool = False) -> subprocess.CompletedProcess[str]:
    return base.run(cmd, cwd=cwd, check=check, capture=capture)


def load(path: Path) -> dict[str, Any]:
    return base.load(path)


def rows(rb: dict[str, Any], name: str) -> list[dict[str, Any]]:
    return base.rows(rb, name)


def table_index(rb: dict[str, Any], name: str) -> dict[str, dict[str, Any]]:
    return base.table_index(rb, name)


def upsert_rows(tbl: dict[str, Any], ident: str, additions: Iterable[dict[str, Any]]) -> None:
    base.upsert_rows(tbl, ident, additions)


def set_meta(rb: dict[str, Any], key: str, value_type: str, *, string: str | None = None, integer: int | None = None, boolean: bool | None = None) -> None:
    base.set_meta(rb, key, value_type, string=string, integer=integer, boolean=boolean)


def active_primitives(rb: dict[str, Any]) -> list[dict[str, Any]]:
    return [row for row in rows(rb, "TSPConceptRegistry") if row.get("Status") == "ACTIVE_PRIMITIVE"]


def active_operators(rb: dict[str, Any]) -> list[dict[str, Any]]:
    return [row for row in rows(rb, "TSPConceptRegistry") if row.get("Status") == "ACTIVE_OPERATOR"]


def ensure_registry_fields(rb: dict[str, Any]) -> None:
    base.ensure_fields(
        rb["TSPConceptRegistry"],
        [
            base.field("HistoricalConceptKind", "string", "raw", True, "Concept kind at the time of its historical certificate."),
            base.field("SemanticCategory", "string", "raw", True, "ATOM, OPERATOR, DERIVED, or HISTORICAL."),
            base.field("ReducedBasisExpression", "string", "raw", True, "Current definition over active atoms and operators."),
            base.field("OperatorExpression", "string", "raw", True, "Operators required to derive the concept from atoms."),
            base.field("ReductionGeneration", "integer", "raw", True, "Reduction generation introducing the current definition."),
            base.field("IsCurrentBasisMember", "boolean", "calculated", True, "Whether the row is an active atom or operator.", formula='=OR({{Status}} = "ACTIVE_PRIMITIVE", {{Status}} = "ACTIVE_OPERATOR")'),
        ],
    )
    for row in rows(rb, "TSPConceptRegistry"):
        row.setdefault("HistoricalConceptKind", row.get("ConceptKind"))
        row.setdefault("SemanticCategory", "DERIVED")
        row.setdefault("ReducedBasisExpression", row.get("BasisExpression", ""))
        row.setdefault("OperatorExpression", "")
        row.setdefault("ReductionGeneration", 1)


def register_concept(
    rb: dict[str, Any],
    ident: str,
    display: str,
    kind: str,
    expression: str,
    arity: int,
    sources: str,
    loop: int,
    *,
    status: str,
    category: str,
    operator_expression: str = "",
) -> None:
    base.register_concept(rb, ident, display, kind, expression, arity, sources, loop, status=status)
    row = table_index(rb, "TSPConceptRegistry")[ident]
    row["HistoricalConceptKind"] = kind
    row["SemanticCategory"] = category
    row["ReducedBasisExpression"] = expression
    row["OperatorExpression"] = operator_expression
    row["ReductionGeneration"] = 2


def add_measurement(rb: dict[str, Any], order: int, term: str, notes: str, status: str = "EARLY_SUPPORT") -> None:
    upsert_rows(
        rb["TSPConvergenceMeasurements"],
        "TSPConvergenceMeasurementId",
        [
            {
                "TSPConvergenceMeasurementId": f"convergence-loop-{order}",
                "TSPLoop": f"tsp-loop-{order}",
                "MeasurementKind": "ATOM_OPERATOR_REFINEMENT" if order < 623 else "ASYMMETRIC_STRESS_EVENT",
                "SurfaceConceptCountBefore": 32,
                "PrimitiveCountAfter": len(active_primitives(rb)),
                "NewPrimitiveCount": 0 if order not in {612, 613, 614} else 1,
                "DerivedAliasCount": sum(row.get("ConceptKind") not in {"PRIMITIVE", "OPERATOR"} for row in rows(rb, "TSPConceptRegistry")),
                "PhysicalTableCount": len(base.canonical_tables(rb)),
                "NovelTerm": term,
                "PredictionStatus": status,
                "Notes": notes,
            }
        ],
    )


def plan_loops(rb: dict[str, Any], contract: dict[str, Any]) -> None:
    upsert_rows(rb["TSPInferenceRules"], "TSPInferenceRuleId", RULE_ROWS)
    existing = {int(row["LoopOrder"]) for row in rows(rb, "TSPLoops")}
    for order, spec in LOOPS.items():
        if order not in existing:
            rows(rb, "TSPLoops").append(
                {
                    "TSPLoopId": f"tsp-loop-{order}",
                    "LoopOrder": order,
                    "DisplayName": spec["name"],
                    "Status": "PLANNED",
                    "PrimaryInferenceRule": spec["rule"],
                    "NewConcept": spec["term"],
                    "WitnessSummary": "Planned predicate-refinement loop; no closure claim yet.",
                    "NextFrontier": spec["next"],
                    "PlannedClosureCriterion": spec["criterion"],
                    "BeforeState": spec["before"],
                    "AfterState": None,
                    "CompletionDisposition": None,
                }
            )
    contract_loops = {int(row["LoopOrder"]): row for row in contract["Loops"]}
    for order, spec in LOOPS.items():
        if order not in contract_loops:
            contract["Loops"].append(
                {
                    "LoopOrder": order,
                    "LoopId": f"tsp-loop-{order}",
                    "Status": "PLANNED",
                    "BeforeState": spec["before"],
                    "ClosureCriterion": spec["criterion"],
                    "AfterState": None,
                    "Result": "Planned; no conclusion recorded.",
                    "CompletionDisposition": None,
                }
            )
    contract["Loops"] = sorted(contract["Loops"], key=lambda row: int(row["LoopOrder"]))
    set_meta(rb, "last_planned_loop", "integer", integer=623)
    set_meta(rb, "refinement_program_status", "string", string="PLANNED_611_623")
    contract["Version"] = "0.5.0-alpha"
    contract.setdefault("Claims", {})["ThreeAtomBasisObserved"] = False
    contract["Claims"]["ThreeAtomBasisProved"] = False


def finish_loop(rb: dict[str, Any], contract: dict[str, Any], order: int, after: str, witness: str) -> None:
    loop = next(row for row in rows(rb, "TSPLoops") if int(row["LoopOrder"]) == order)
    loop.update(
        {
            "Status": "CLOSED",
            "AfterState": after,
            "WitnessSummary": witness,
            "CompletionDisposition": "CLOSED",
            "NextFrontier": LOOPS[order]["next"],
        }
    )
    contract_loop = next(row for row in contract["Loops"] if int(row["LoopOrder"]) == order)
    contract_loop.update(
        {
            "Status": "CLOSED",
            "AfterState": after,
            "Result": witness,
            "CompletionDisposition": "CLOSED",
        }
    )
    set_meta(rb, "last_loop", "integer", integer=order)
    set_meta(rb, "highest_completed_loop", "integer", integer=order)


def transform_expressions(rb: dict[str, Any], replacements: dict[str, str], operator: str | None = None) -> None:
    for row in rows(rb, "TSPConceptRegistry"):
        expression = row.get("ReducedBasisExpression") or row.get("BasisExpression") or ""
        changed = False
        for old, new in replacements.items():
            if old in expression:
                expression = expression.replace(old, new)
                changed = True
        row["ReducedBasisExpression"] = expression
        if changed and operator:
            existing = [item for item in (row.get("OperatorExpression") or "").split("+") if item]
            if operator not in existing:
                existing.append(operator)
            row["OperatorExpression"] = "+".join(existing)
        row.setdefault("ReductionGeneration", 2)


def reduce_old_primitives(rb: dict[str, Any], ids: list[str], replacement: str, generation: int) -> None:
    concepts = table_index(rb, "TSPConceptRegistry")
    for ident in ids:
        row = concepts[ident]
        row["HistoricalConceptKind"] = row.get("HistoricalConceptKind") or "PRIMITIVE"
        row["ConceptKind"] = "HISTORICAL_PRIMITIVE"
        row["Status"] = f"REDUCED_TO_{replacement}"
        row["SemanticCategory"] = "HISTORICAL"
        row["SupersededByConcept"] = f"concept-{replacement.lower()}"
        row["ReductionGeneration"] = generation


def loop_611(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_registry_fields(rb)
    for ident in OLD_PRIMITIVES:
        row = table_index(rb, "TSPConceptRegistry")[ident]
        row["HistoricalConceptKind"] = "PRIMITIVE_V1"
        row["SemanticCategory"] = "CANDIDATE_ATOM" if ident != "concept-connectivity" else "CANDIDATE_OPERATOR_DERIVED"
        row["ReducedBasisExpression"] = OLD_PRIMITIVES[ident]
        row["ReductionGeneration"] = 1
    register_concept(rb, "coined-predicate-operator-split", "Predicate–Operator Split", "COINED_PREDICATE", "WARRANT(Classification)+VALUATION(BasisCardinality)", 1, "TSPConceptRegistry,TSPConvergenceMeasurements", 611, status="ACTIVE_DERIVED", category="DERIVED")
    add_measurement(rb, 611, "Predicate–Operator Split", "The loop-610 basis is preserved as history while candidates for atoms and operators are explicitly classified.")
    return (
        "Separated the historical eight-predicate basis into candidate relational atoms and operator-derived concepts without changing the active basis.",
        "All eight loop-610 primitives retain their historical identity and receive explicit semantic-category and reduced-expression fields; current primitive count remains eight.",
    )


def loop_612(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_concept(rb, "concept-attachment", "Attachment", "PRIMITIVE", "ATTACHMENT(subject,object,role)", 3, "Cities,Neighborhoods,Addresses,InstanceStops,TravelEdges,TourStops,TSPClusterBoundaryStates", 612, status="ACTIVE_PRIMITIVE", category="ATOM")
    reduce_old_primitives(rb, ["concept-membership", "concept-incidence"], "ATTACHMENT", 2)
    transform_expressions(rb, {"MEMBERSHIP": "ATTACHMENT", "INCIDENCE": "ATTACHMENT"})
    register_concept(rb, "coined-attachment-atom", "Attachment Atom", "COINED_PREDICATE", "ATTACHMENT(subject,object,role)+WARRANT(role)", 1, "TSPConceptRegistry", 612, status="ACTIVE_DERIVED", category="DERIVED")
    add_measurement(rb, 612, "Attachment Atom", "Membership and incidence are role-specialized attachments; active primitive count falls from eight to seven.")
    return (
        "Collapsed membership and incidence into ATTACHMENT with typed roles such as member, endpoint, entry, exit, and uses.",
        "Every historical membership/incidence surface retains a reduced expression, while the active primitive count becomes seven.",
    )


def loop_613(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_concept(rb, "concept-valuation", "Valuation", "PRIMITIVE", "VALUATION(subject,measure,value)", 3, "TravelEdges,TourStops,LocalDegreeBounds,InstanceLowerBounds,TSPBoundTerms,TSPConvergenceMeasurements", 613, status="ACTIVE_PRIMITIVE", category="ATOM")
    reduce_old_primitives(rb, ["concept-cardinality", "concept-order", "concept-weight"], "VALUATION", 2)
    transform_expressions(rb, {"CARDINALITY": "VALUATION", "ORDER": "VALUATION", "WEIGHT": "VALUATION"}, operator="AGGREGATE_PENDING")
    register_concept(rb, "coined-valuation-atom", "Valuation Atom", "COINED_PREDICATE", "VALUATION(subject,measure,value)+WARRANT(measure)", 1, "TSPConceptRegistry", 613, status="ACTIVE_DERIVED", category="DERIVED")
    add_measurement(rb, 613, "Valuation Atom", "Counts, ordinals, costs, bounds, ranks, and deltas become named valuations; active primitive count falls to five.")
    return (
        "Collapsed cardinality, order, and weight into VALUATION under explicit measures.",
        "Degree is a valuation under incident-count, sequence is a valuation under ordinal-position, and cost is a valuation under the objective measure; active primitive count becomes five.",
    )


def loop_614(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_concept(rb, "concept-warrant", "Warrant", "PRIMITIVE", "WARRANT(conclusion,source,modality)", 3, "TSPInferenceRules,TSPInferenceApplications,TSPInferenceAntecedents,TSPFrontierObligations,TSPEdgeStates", 614, status="ACTIVE_PRIMITIVE", category="ATOM")
    reduce_old_primitives(rb, ["concept-commitment", "concept-provenance"], "WARRANT", 2)
    transform_expressions(rb, {"COMMITMENT": "WARRANT", "PROVENANCE": "WARRANT"})
    register_concept(rb, "coined-warrant-atom", "Warrant Atom", "COINED_PREDICATE", "WARRANT(conclusion,source,modality)", 1, "TSPConceptRegistry,TSPInferenceApplications,TSPEdgeStates", 614, status="ACTIVE_DERIVED", category="DERIVED")
    add_measurement(rb, 614, "Warrant Atom", "Commitment and provenance become one scoped warrant relation without erasing selected-versus-forced modality; active primitives fall to four.")
    return (
        "Collapsed commitment and provenance into WARRANT with explicit source, modality, and scope.",
        "SELECTED remains a current-witness warrant, FORCED remains an all-feasible-tours warrant, and historical provenance remains queryable; active primitive count becomes four.",
    )


def register_operator(rb: dict[str, Any], ident: str, display: str, expression: str, loop: int, sources: str) -> None:
    register_concept(rb, ident, display, "OPERATOR", expression, 1, sources, loop, status="ACTIVE_OPERATOR", category="OPERATOR", operator_expression=display.upper().replace("-", "_"))


def loop_615(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_operator(rb, "operator-closure", "Closure", "CLOSURE(ATTACHMENT,scope)", 615, "TravelEdges,TSPDerivedEdgeSets,TSPSpanningTreeEdges,TSPDefectProfiles")
    reduce_old_primitives(rb, ["concept-connectivity"], "CLOSURE", 2)
    transform_expressions(rb, {"CONNECTIVITY": "CLOSURE(ATTACHMENT)"}, operator="CLOSURE")
    register_concept(rb, "coined-closure-operator", "Closure Operator", "COINED_PREDICATE", "CLOSURE(ATTACHMENT)+WARRANT(scope)", 1, "TSPConceptRegistry,TSPDerivedEdgeSets", 615, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="CLOSURE")
    add_measurement(rb, 615, "Closure Operator", "Connectivity becomes reachability closure over attachments; active relational primitives are ATTACHMENT, VALUATION, and WARRANT.")
    return (
        "Lifted connectivity from the primitive basis into CLOSURE over typed attachments.",
        "The current active atom count is three: ATTACHMENT, VALUATION, WARRANT. Connectedness, components, cuts, and reachability are closure results.",
    )


def loop_616(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_operator(rb, "operator-aggregate", "Aggregate", "AGGREGATE(multiset,measure,reducer)", 616, "TSPConvergenceMeasurements,LocalDegreeBounds,InstanceLowerBounds,TSPBoundTerms,SearchMetrics")
    for row in rows(rb, "TSPConceptRegistry"):
        expression = row.get("ReducedBasisExpression") or ""
        if any(token in (row.get("BasisExpression") or "") for token in ("CARDINALITY", "WEIGHT")):
            row["OperatorExpression"] = "+".join(dict.fromkeys([item for item in (row.get("OperatorExpression") or "").replace("AGGREGATE_PENDING", "AGGREGATE").split("+") if item] + ["AGGREGATE"]))
    register_concept(rb, "coined-aggregate-operator", "Aggregate Operator", "COINED_PREDICATE", "AGGREGATE(ATTACHMENT,VALUATION,reducer)+WARRANT", 1, "TSPConceptRegistry,InstanceLowerBounds,TSPBoundTerms", 616, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="AGGREGATE")
    add_measurement(rb, 616, "Aggregate Operator", "Counts, sums, minima, maxima, parity, degree, and repair totals share one operator over attachments and valuations.")
    return (
        "Lifted count, sum, minimum, maximum, parity, and signed repair arithmetic into AGGREGATE.",
        "Degree, edge count, lower-bound sum, fiber minimum, and convergence measurement now differ by measure and reducer, not by primitive type.",
    )


def loop_617(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_operator(rb, "operator-quotient", "Quotient", "QUOTIENT(witnesses,equivalence,warrant)", 617, "TSPClusterBoundaryStates,TSPClusterContractionCertificates,TSPWitnessNormalForms,Neighborhoods")
    for ident in ("coined-semantic-quotient", "coined-component-quotient", "coined-boundary-signature"):
        if ident in table_index(rb, "TSPConceptRegistry"):
            row = table_index(rb, "TSPConceptRegistry")[ident]
            row["OperatorExpression"] = "+".join(dict.fromkeys([item for item in (row.get("OperatorExpression") or "").split("+") if item] + ["QUOTIENT"]))
            row["ReducedBasisExpression"] = "QUOTIENT(ATTACHMENT,VALUATION,WARRANT)"
    register_concept(rb, "coined-quotient-operator", "Quotient Operator", "COINED_PREDICATE", "QUOTIENT(ATTACHMENT,VALUATION,WARRANT)", 1, "TSPConceptRegistry,TSPClusterContractionCertificates", 617, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="QUOTIENT")
    add_measurement(rb, 617, "Quotient Operator", "Symmetry classes, dominated witnesses, and component contraction become applications of one explicit equivalence/representative operator.")
    return (
        "Lifted semantic and component contraction into QUOTIENT with explicit equivalence, representatives, and expansion warrant.",
        "Path reversal, component abstraction, and later valuation dominance now share one operator contract.",
    )


def loop_618(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_operator(rb, "operator-fixpoint", "Fixpoint", "FIXPOINT(rule,state,until_stable)", 618, "TSPInferenceApplications,TSPInferenceStates,TSPConstraintRounds,TSPConstraintDecisions")
    for ident in ("coined-closure-event", "coined-commitment-lattice"):
        if ident in table_index(rb, "TSPConceptRegistry"):
            row = table_index(rb, "TSPConceptRegistry")[ident]
            row["OperatorExpression"] = "+".join(dict.fromkeys([item for item in (row.get("OperatorExpression") or "").split("+") if item] + ["FIXPOINT"]))
            if ident == "coined-closure-event":
                row["ReducedBasisExpression"] = "FIXPOINT(WARRANT,ATTACHMENT,VALUATION)"
    register_concept(rb, "coined-fixpoint-operator", "Fixed-Point Operator", "COINED_PREDICATE", "FIXPOINT(WARRANT,ATTACHMENT,VALUATION)", 1, "TSPInferenceApplications,TSPConstraintRounds", 618, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="FIXPOINT")
    add_measurement(rb, 618, "Fixed-Point Operator", "Inference rounds and closure events share repeated warranted state transformation until no commitment changes.")
    return (
        "Lifted repeated constraint and inference closure into FIXPOINT.",
        "Forcing, forbidding, repair, and quotient events can now be described as warranted transformations iterated until state stability.",
    )


def loop_619(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    concepts = rows(rb, "TSPConceptRegistry")
    old_tokens = tuple(OLD_PRIMITIVES.values())
    unresolved = [row["TSPConceptId"] for row in concepts if any(token in (row.get("ReducedBasisExpression") or "") for token in old_tokens)]
    if unresolved:
        raise AssertionError(f"unreduced concepts before three-atom certificate: {unresolved[:10]}")
    register_concept(rb, "coined-three-atom-basis", "Three-Atom Basis", "COINED_PREDICATE", "ATTACHMENT+VALUATION+WARRANT+CLOSURE+AGGREGATE+QUOTIENT+FIXPOINT", 1, "TSPConceptRegistry,TSPConvergenceMeasurements", 619, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="CLOSURE+AGGREGATE+QUOTIENT+FIXPOINT")
    add_measurement(rb, 619, "Three-Atom Basis", "All registered concepts reduce to three atoms and four operators; the historical eight-predicate basis remains versioned data.", status="SUPPORTED_FOR_CURRENT_DOMAIN")
    set_meta(rb, "active_predicate_atom_count", "integer", integer=3)
    set_meta(rb, "active_semantic_operator_count", "integer", integer=4)
    set_meta(rb, "semantic_compression_pct_current", "string", string="90.63")
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "ThreeAtomBasisObserved": True,
            "ThreeAtomBasisProved": False,
            "CurrentPredicateAtomCount": 3,
            "CurrentSemanticOperatorCount": 4,
            "HistoricalLoop610PredicateBasisCount": 8,
            "CurrentSemanticCompressionPct": 90.63,
        }
    )
    acceptance = contract.setdefault("Acceptance", {})
    acceptance.update(
        {
            "CurrentPredicateAtomCount": 3,
            "CurrentSemanticOperatorCount": 4,
            "HistoricalLoop610PredicateBasisCount": 8,
            "CurrentSemanticCompressionPct": 90.63,
        }
    )
    return (
        "Certified a current basis of three atoms and four operators while preserving the historical eight-predicate certificate.",
        "Atoms = ATTACHMENT, VALUATION, WARRANT. Operators = CLOSURE, AGGREGATE, QUOTIENT, FIXPOINT. Thirty-two baseline surfaces compress to three active atoms (90.63%) for the represented domain.",
    )


def ensure_boundary_fields(rb: dict[str, Any]) -> None:
    base.ensure_fields(
        rb["TSPClusterBoundaryStates"],
        [
            base.field("SecondInternalViaStop", "string", "relationship", True, "Second internal stop for clusters larger than three.", related_to="InstanceStops"),
            base.field("EntryPortRole", "string", "raw", True, "Typed role of the entry boundary attachment."),
            base.field("ExitPortRole", "string", "raw", True, "Typed role of the exit boundary attachment."),
            base.field("PortContract", "string", "raw", True, "Coverage and incidence contract exposed at the boundary."),
            base.field("BoundaryFiberKey", "string", "raw", True, "Scope plus canonical unordered boundary ports."),
            base.field("InternalOrderKey", "string", "raw", True, "Canonical represented internal order."),
            base.field("RawOrientationMultiplicity", "integer", "raw", True, "Directed orientations represented by this quotient row."),
            base.field("DominatedByStateId", "string", "raw", True, "Canonical lower-valued representative in the same fiber."),
            base.field("DominanceDelta", "number", "raw", True, "Cost difference from the fiber minimum."),
            base.field("IsFiberMinimum", "boolean", "calculated", True, "Whether the state is the surviving minimum in its fiber.", formula="=AND({{IsQuotientRepresentative}}, NOT({{IsDominated}}))"),
        ],
    )


def loop_620(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_boundary_fields(rb)
    for state in rows(rb, "TSPClusterBoundaryStates"):
        entry, exit_ = state["EntryStop"], state["ExitStop"]
        state["EntryPortRole"] = "BOUNDARY_ENTRY"
        state["ExitPortRole"] = "BOUNDARY_EXIT"
        state["PortContract"] = "ONE_ENTRY_ONE_EXIT_FULL_INTERNAL_COVERAGE"
        state["BoundaryFiberKey"] = f"{state['Neighborhood']}|{min(entry, exit_)}|{max(entry, exit_)}"
        state["InternalOrderKey"] = f"{entry}>{state['InternalViaStop']}>{exit_}"
        state["RawOrientationMultiplicity"] = state.get("OrientationMultiplicity") or 2
        state["DominatedByStateId"] = None
        state["DominanceDelta"] = 0
    register_concept(rb, "coined-boundary-port", "Boundary Port", "COINED_PREDICATE", "ATTACHMENT(witness,scope,entry_or_exit)+VALUATION(port_degree)+WARRANT(contract)", 1, "TSPClusterBoundaryStates,TSPWitnessNormalForms", 620, status="ACTIVE_DERIVED", category="DERIVED")
    add_measurement(rb, 620, "Boundary Port", "Entry and exit become typed attachment roles under one port contract; no table is added.")
    return (
        "Normalized existing cluster entries and exits as typed boundary ports over ATTACHMENT, VALUATION, and WARRANT.",
        "All six existing triangle path states expose one entry, one exit, full internal coverage, orientation multiplicity two, and a canonical boundary-fiber key.",
    )


ASYM_EDGES = {
    ("asym-stop-1", "asym-stop-2"): ("asym-edge-1-2", 1),
    ("asym-stop-1", "asym-stop-3"): ("asym-edge-1-3", 4),
    ("asym-stop-1", "asym-stop-4"): ("asym-edge-1-4", 8),
    ("asym-stop-2", "asym-stop-3"): ("asym-edge-2-3", 2),
    ("asym-stop-2", "asym-stop-4"): ("asym-edge-2-4", 5),
    ("asym-stop-3", "asym-stop-4"): ("asym-edge-3-4", 3),
}


def edge_for(a: str, b: str) -> tuple[str, int]:
    return ASYM_EDGES[tuple(sorted((a, b)))]


def asym_path_specs() -> list[tuple[str, str, tuple[str, str]]]:
    nodes = [f"asym-stop-{i}" for i in range(1, 5)]
    result: list[tuple[str, str, tuple[str, str]]] = []
    for i, entry in enumerate(nodes):
        for exit_ in nodes[i + 1 :]:
            internal = [node for node in nodes if node not in {entry, exit_}]
            result.append((entry, exit_, (internal[0], internal[1])))
            result.append((entry, exit_, (internal[1], internal[0])))
    return result


def loop_621(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_boundary_fields(rb)
    upsert_rows(
        rb["Neighborhoods"],
        "NeighborhoodId",
        [
            {
                "NeighborhoodId": "gridville-asymmetric-four",
                "DisplayName": "Asymmetric Four",
                "City": "gridville",
                "ClusterKind": "ASYMMETRIC_BOUNDARY_FIBER_FIXTURE",
                "IsQuotientNode": True,
                "QuotientNodeKind": "BOUNDARY_FIBER_REGION",
                "RequiredBoundaryDegree": 2,
                "QuotientScopeId": "tsp-asymmetric-four-4",
            }
        ],
    )
    addresses = []
    for i in range(1, 5):
        addresses.append(
            {
                "AddressId": f"addr-asym-{i}",
                "StreetLabel": f"Asymmetric {i}",
                "Neighborhood": "gridville-asymmetric-four",
                "XCoordinate": 30 + i,
                "YCoordinate": (i * i) % 5,
                "IsDepotCandidate": i == 1,
            }
        )
    upsert_rows(rb["Addresses"], "AddressId", addresses)
    upsert_rows(
        rb["TSPInstances"],
        "TSPInstanceId",
        [
            {
                "TSPInstanceId": "tsp-asymmetric-four-4",
                "DisplayName": "Asymmetric Four-Stop Boundary Fiber",
                "City": "gridville",
                "DepotAddress": "addr-asym-1",
                "DistanceModel": "EXPLICIT_SYMMETRIC_COST",
                "IsSymmetric": True,
                "Status": "NORMALIZED",
                "SearchPolicy": "LOCAL_QUOTIENT_BEFORE_CITY_SEARCH",
                "Notes": "Four-stop complete internal cluster with unequal path valuations used to separate reversal quotienting from fiber dominance.",
            }
        ],
    )
    upsert_rows(
        rb["InstanceStops"],
        "InstanceStopId",
        [
            {
                "InstanceStopId": f"asym-stop-{i}",
                "TSPInstance": "tsp-asymmetric-four-4",
                "Address": f"addr-asym-{i}",
                "IsRequired": True,
            }
            for i in range(1, 5)
        ],
    )
    edge_rows = []
    for (a, b), (edge_id, cost) in ASYM_EDGES.items():
        edge_rows.append(
            {
                "TravelEdgeId": edge_id,
                "TSPInstance": "tsp-asymmetric-four-4",
                "FromStop": a,
                "ToStop": b,
                "DistanceMeters": cost * 1000,
                "TravelCost": cost,
                "IsAvailable": True,
                "EdgeSource": "ASYMMETRIC_FIBER_FIXTURE",
                "CanonicalPairKey": f"{a}|{b}",
            }
        )
    upsert_rows(rb["TravelEdges"], "TravelEdgeId", edge_rows)

    state_rows = []
    member_rows = []
    for entry, exit_, internal in asym_path_specs():
        first, second = internal
        sequence = [entry, first, second, exit_]
        members = [edge_for(sequence[index], sequence[index + 1]) for index in range(3)]
        cost = sum(item[1] for item in members)
        suffix = "-".join(node.rsplit("-", 1)[-1] for node in sequence)
        state_id = f"cluster-state-asym-{suffix}"
        fiber = f"gridville-asymmetric-four|{entry}|{exit_}"
        state_rows.append(
            {
                "TSPClusterBoundaryStateId": state_id,
                "TSPInstance": "tsp-asymmetric-four-4",
                "Neighborhood": "gridville-asymmetric-four",
                "EntryStop": entry,
                "ExitStop": exit_,
                "InternalViaStop": first,
                "SecondInternalViaStop": second,
                "InternalPathCost": cost,
                "InternalStopCount": 4,
                "IsHamiltonianPath": True,
                "IsDominated": False,
                "Status": "FIBER_CANDIDATE",
                "OrientationMultiplicity": 2,
                "IsQuotientRepresentative": True,
                "EntryPortRole": "BOUNDARY_ENTRY",
                "ExitPortRole": "BOUNDARY_EXIT",
                "PortContract": "ONE_ENTRY_ONE_EXIT_FULL_INTERNAL_COVERAGE",
                "BoundaryFiberKey": fiber,
                "InternalOrderKey": ">".join(sequence),
                "RawOrientationMultiplicity": 2,
                "DominatedByStateId": None,
                "DominanceDelta": 0,
            }
        )
        for order, (edge_id, _) in enumerate(members, start=1):
            member_rows.append(
                {
                    "TSPClusterBoundaryStateMemberId": f"member-{state_id}-{order}",
                    "ClusterBoundaryState": state_id,
                    "TravelEdge": edge_id,
                    "MemberOrder": order,
                }
            )
    upsert_rows(rb["TSPClusterBoundaryStates"], "TSPClusterBoundaryStateId", state_rows)
    upsert_rows(rb["TSPClusterBoundaryStateMembers"], "TSPClusterBoundaryStateMemberId", member_rows)
    register_concept(rb, "coined-boundary-fiber", "Boundary Fiber", "COINED_PREDICATE", "QUOTIENT(ATTACHMENT(paths,ports),equal_ports_and_coverage)+VALUATION(cost)+WARRANT(expansion)", 1, "TSPClusterBoundaryStates,TSPClusterBoundaryStateMembers", 621, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="QUOTIENT")
    add_measurement(rb, 621, "Boundary Fiber", "An asymmetric four-stop cluster yields six fibers, two internal-order representatives per fiber, and twenty-four raw directed orientations without adding a table.")
    return (
        "Represented an asymmetric four-stop complete cluster as six boundary fibers containing twelve reversal-quotient path witnesses.",
        "The twelve path rows retain twenty-four raw directed orientations, full four-stop coverage, explicit three-edge expansions, and unequal costs within every boundary fiber.",
    )


def loop_622(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    states = [row for row in rows(rb, "TSPClusterBoundaryStates") if row["TSPInstance"] == "tsp-asymmetric-four-4"]
    fibers: dict[str, list[dict[str, Any]]] = defaultdict(list)
    for state in states:
        fibers[state["BoundaryFiberKey"]].append(state)
    if len(fibers) != 6 or any(len(group) != 2 for group in fibers.values()):
        raise AssertionError("asymmetric boundary fibers must contain six pairs")
    for group in fibers.values():
        ordered = sorted(group, key=lambda row: (row["InternalPathCost"], row["InternalOrderKey"]))
        survivor, dominated = ordered
        if survivor["InternalPathCost"] == dominated["InternalPathCost"]:
            raise AssertionError("fixture must expose strict fiber dominance")
        survivor.update({"IsDominated": False, "Status": "SURVIVES", "DominatedByStateId": None, "DominanceDelta": 0})
        dominated.update(
            {
                "IsDominated": True,
                "Status": "DOMINATED",
                "DominatedByStateId": survivor["TSPClusterBoundaryStateId"],
                "DominanceDelta": dominated["InternalPathCost"] - survivor["InternalPathCost"],
            }
        )
    register_concept(rb, "coined-fiber-minimum", "Fiber Minimum", "COINED_PREDICATE", "AGGREGATE(VALUATION(path,cost),MIN) within BoundaryFiber + WARRANT(dominance)", 1, "TSPClusterBoundaryStates", 622, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="AGGREGATE+QUOTIENT")
    add_measurement(rb, 622, "Fiber Minimum", "Six strict minimum-cost representatives survive and six higher-valued paths remain as dominated expansion provenance.")
    return (
        "Collapsed each asymmetric boundary fiber to its unique minimum-valued path while preserving the dominated alternative.",
        "Six fibers each contain one SURVIVES row and one DOMINATED row; dominance deltas are 1, 6, 5, 5, 6, and 1 under the declared edge costs.",
    )


def loop_623(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    upsert_rows(
        rb["TSPClusterContractionCertificates"],
        "TSPClusterContractionCertificateId",
        [
            {
                "TSPClusterContractionCertificateId": "contraction-asymmetric-four-boundary-fibers",
                "TSPInstance": "tsp-asymmetric-four-4",
                "Neighborhood": "gridville-asymmetric-four",
                "ScopeKind": "NEIGHBORHOOD",
                "RawInternalOrderCount": 24,
                "SurvivingBoundaryStateCount": 6,
                "RawCombinationCount": None,
                "ContractedCombinationCount": None,
                "ReductionPct": 75,
                "IsPassing": True,
                "ScopeClaim": "CERTIFIED_FOR_ASYMMETRIC_FOUR_STOP_CLUSTER",
                "EquivalenceRelation": "PATH_REVERSAL_THEN_EQUAL_PORT_EQUAL_COVERAGE_COST_DOMINANCE",
                "QuotientClassCount": 12,
            }
        ],
    )
    register_concept(rb, "coined-asymmetric-quotient", "Asymmetric Quotient", "COINED_PREDICATE", "QUOTIENT(raw_orientations,reversal) then QUOTIENT(boundary_fiber,minimum_VALUATION)+WARRANT(expansion)", 1, "TSPClusterBoundaryStates,TSPClusterContractionCertificates", 623, status="ACTIVE_DERIVED", category="DERIVED", operator_expression="QUOTIENT+AGGREGATE")
    add_measurement(rb, 623, "Asymmetric Quotient", "Twenty-four directed paths reduce to twelve reversal classes and six valuation-minimal boundary states with the same three atoms and four operators.", status="SUPPORTED_FOR_CURRENT_DOMAIN")
    set_meta(rb, "coined_predicate_count_611_623", "integer", integer=13)
    set_meta(rb, "current_predicate_atom_count", "integer", integer=3)
    set_meta(rb, "current_operator_count", "integer", integer=4)
    set_meta(rb, "physical_table_count_loop_623", "integer", integer=len(base.canonical_tables(rb)))
    contract["Version"] = "0.5.0"
    contract["Scope"] = "The represented scope includes the loop-610 eight-predicate historical basis, a current three-atom/four-operator factorization, and asymmetric four-stop boundary-fiber quotienting. No universal convergence or general TSP claim is made."
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "ThreeAtomBasisObserved": True,
            "ThreeAtomBasisProved": False,
            "CurrentPredicateAtomCount": 3,
            "CurrentSemanticOperatorCount": 4,
            "HistoricalLoop610PredicateBasisCount": 8,
            "CurrentSemanticCompressionPct": 90.63,
            "NovelTermsCoined611To623": 13,
            "AsymmetricFourStopQuotientCertified": True,
        }
    )
    acceptance = contract.setdefault("Acceptance", {})
    acceptance.update(
        {
            "CurrentPredicateAtomCount": 3,
            "CurrentSemanticOperatorCount": 4,
            "HistoricalLoop610PredicateBasisCount": 8,
            "CurrentSemanticCompressionPct": 90.63,
            "NovelTermsCoined611To623": 13,
            "AsymmetricRawDirectedOrders": 24,
            "AsymmetricReversalClasses": 12,
            "AsymmetricFiberMinima": 6,
            "AsymmetricQuotientReductionPct": 75,
        }
    )
    certs = {row["CertificateId"]: row for row in contract.setdefault("CurrentCertificates", [])}
    certs["tsp-three-atom-basis"] = {
        "CertificateId": "tsp-three-atom-basis",
        "Kind": "atom-operator-basis-certificate",
        "Conclusion": "The current registered TSP vocabulary is explicitly defined over ATTACHMENT, VALUATION, WARRANT and four named operators; this is empirical for the represented domain.",
    }
    certs["asymmetric-four-boundary-quotient"] = {
        "CertificateId": "asymmetric-four-boundary-quotient",
        "Kind": "asymmetric-semantic-quotient-certificate",
        "Conclusion": "Twenty-four directed Hamiltonian paths reduce to twelve reversal classes and six strict boundary-fiber minima with expansion provenance.",
    }
    contract["CurrentCertificates"] = list(certs.values())
    contract["RemainingFrontier"] = [
        "generalize repair equations to three or more quotient components",
        "represent compatibility constraints between boundary fibers without introducing a new primitive",
        "emit a branch orbit only when fixed-point closure leaves multiple non-equivalent minimum warrants",
    ]
    append_readme_section()
    return (
        "Certified an asymmetric semantic quotient: 24 directed internal paths to 12 reversal classes to 6 fiber minima, with no physical table growth.",
        "The current semantic vocabulary remains three atoms plus four operators; asymmetric costs coin new predicates without adding a primitive. The certificate is finite and scoped to tsp-asymmetric-four-4.",
    )


LOOP_FUNCS = {
    611: loop_611,
    612: loop_612,
    613: loop_613,
    614: loop_614,
    615: loop_615,
    616: loop_616,
    617: loop_617,
    618: loop_618,
    619: loop_619,
    620: loop_620,
    621: loop_621,
    622: loop_622,
    623: loop_623,
}


def append_readme_section() -> None:
    text = README.read_text()
    marker = "## Loops 611–623 — atoms, operators, and asymmetric fibers"
    if marker in text:
        return
    text += f'''\n\n{marker}\n\nThe loop-610 eight-predicate basis was retained as a historical certificate and factored again. The current candidate basis is:\n\n```text\nATOMS:      ATTACHMENT   VALUATION   WARRANT\nOPERATORS:  CLOSURE      AGGREGATE   QUOTIENT   FIXPOINT\n```\n\nThis is **90.63% surface-to-atom compression** from the original 32 recurring surface predicates. It remains empirical for the represented TSP domain, not a theorem of universal minimality.\n\nThe asymmetric four-stop stress fixture separates two reductions that were conflated by the three-stop triangles:\n\n```text\n24 directed Hamiltonian paths\n        ↓ path-reversal quotient\n12 unordered-boundary path classes\n        ↓ minimum valuation within each boundary fiber\n 6 surviving port-to-port states\n```\n\nNo new physical table is introduced by loops 611–623. Historical projections remain replayable while the active semantic basis shrinks.\n'''
    README.write_text(text)


def validate_state(rb: dict[str, Any], contract: dict[str, Any]) -> None:
    loop_map = {int(row["LoopOrder"]): row for row in rows(rb, "TSPLoops")}
    planned = base.meta_int(rb, "last_planned_loop")
    if sorted(loop_map) != list(range(577, planned + 1)):
        raise AssertionError(f"loop sequence is not contiguous through {planned}")
    contract_map = {int(row["LoopOrder"]): row for row in contract["Loops"]}
    if set(loop_map) != set(contract_map):
        raise AssertionError("contract/rulebook loop set mismatch")
    for order in range(611, planned + 1):
        row = loop_map[order]
        if not row.get("BeforeState") or not row.get("PlannedClosureCriterion"):
            raise AssertionError(f"loop {order} missing before-state or criterion")
        if contract_map[order]["Status"] != row["Status"]:
            raise AssertionError(f"loop {order} status mismatch")
        if row["Status"] == "CLOSED" and not row.get("AfterState"):
            raise AssertionError(f"loop {order} closed without after-state")

    if 611 in loop_map and loop_map[611]["Status"] == "CLOSED":
        required = {"HistoricalConceptKind", "SemanticCategory", "ReducedBasisExpression", "OperatorExpression", "ReductionGeneration", "IsCurrentBasisMember"}
        fields = {field["name"] for field in rb["TSPConceptRegistry"]["schema"]}
        if not required <= fields:
            raise AssertionError("concept-refinement fields missing")
    expected_active = {612: 7, 613: 5, 614: 4, 615: 3}
    for order, expected in expected_active.items():
        if loop_map.get(order, {}).get("Status") == "CLOSED" and len(active_primitives(rb)) != (3 if any(loop_map.get(later, {}).get("Status") == "CLOSED" for later in range(order + 1, planned + 1) if later <= 615) else expected):
            # Later reductions legitimately lower the current count; only reject growth or incomplete reduction.
            current = len(active_primitives(rb))
            if current > expected or current < 3:
                raise AssertionError(f"active primitive count invalid after loop {order}: {current}")
    if loop_map.get(615, {}).get("Status") == "CLOSED" and len(active_primitives(rb)) != 3:
        raise AssertionError("closure lift must leave three active primitives")
    operator_expected = {615: 1, 616: 2, 617: 3, 618: 4}
    for order, expected in operator_expected.items():
        if loop_map.get(order, {}).get("Status") == "CLOSED":
            current = len(active_operators(rb))
            if current < expected or current > 4:
                raise AssertionError(f"operator count invalid at loop {order}: {current}")
    if loop_map.get(619, {}).get("Status") == "CLOSED":
        if {row["DisplayName"] for row in active_primitives(rb)} != {"Attachment", "Valuation", "Warrant"}:
            raise AssertionError("three-atom basis mismatch")
        if {row["DisplayName"] for row in active_operators(rb)} != {"Closure", "Aggregate", "Quotient", "Fixpoint"}:
            raise AssertionError("four-operator basis mismatch")
        old_tokens = tuple(OLD_PRIMITIVES.values())
        unresolved = [row["TSPConceptId"] for row in rows(rb, "TSPConceptRegistry") if any(token in (row.get("ReducedBasisExpression") or "") for token in old_tokens)]
        if unresolved:
            raise AssertionError(f"unreduced historical tokens remain: {unresolved[:10]}")
    if loop_map.get(620, {}).get("Status") == "CLOSED":
        states = [row for row in rows(rb, "TSPClusterBoundaryStates") if row["TSPInstance"] == "tsp-twin-triangles-6"]
        if len(states) != 6 or any(not row.get("BoundaryFiberKey") or row.get("RawOrientationMultiplicity") != 2 for row in states):
            raise AssertionError("boundary-port normalization mismatch")
    if loop_map.get(621, {}).get("Status") == "CLOSED":
        stops = [row for row in rows(rb, "InstanceStops") if row["TSPInstance"] == "tsp-asymmetric-four-4"]
        edges = [row for row in rows(rb, "TravelEdges") if row["TSPInstance"] == "tsp-asymmetric-four-4"]
        states = [row for row in rows(rb, "TSPClusterBoundaryStates") if row["TSPInstance"] == "tsp-asymmetric-four-4"]
        members = [row for row in rows(rb, "TSPClusterBoundaryStateMembers") if row["ClusterBoundaryState"] in {state["TSPClusterBoundaryStateId"] for state in states}]
        if (len(stops), len(edges), len(states), len(members)) != (4, 6, 12, 36):
            raise AssertionError(f"asymmetric fixture mismatch: {(len(stops), len(edges), len(states), len(members))}")
        if sum(int(row.get("RawOrientationMultiplicity") or 0) for row in states) != 24:
            raise AssertionError("asymmetric raw orientation count mismatch")
    if loop_map.get(622, {}).get("Status") == "CLOSED":
        states = [row for row in rows(rb, "TSPClusterBoundaryStates") if row["TSPInstance"] == "tsp-asymmetric-four-4"]
        fibers: dict[str, list[dict[str, Any]]] = defaultdict(list)
        for state in states:
            fibers[state["BoundaryFiberKey"]].append(state)
        if len(fibers) != 6:
            raise AssertionError("asymmetric fiber count mismatch")
        for key, group in fibers.items():
            survivors = [row for row in group if not row["IsDominated"]]
            dominated = [row for row in group if row["IsDominated"]]
            if len(survivors) != 1 or len(dominated) != 1:
                raise AssertionError(f"fiber {key} lacks one survivor and one dominated state")
            if dominated[0]["DominatedByStateId"] != survivors[0]["TSPClusterBoundaryStateId"]:
                raise AssertionError(f"fiber {key} dominance provenance mismatch")
            if dominated[0]["DominanceDelta"] <= 0:
                raise AssertionError(f"fiber {key} nonpositive dominance delta")
    if loop_map.get(623, {}).get("Status") == "CLOSED":
        cert = table_index(rb, "TSPClusterContractionCertificates")["contraction-asymmetric-four-boundary-fibers"]
        if (cert["RawInternalOrderCount"], cert["QuotientClassCount"], cert["SurvivingBoundaryStateCount"], cert["ReductionPct"]) != (24, 12, 6, 75):
            raise AssertionError("asymmetric quotient certificate mismatch")
        if len(base.canonical_tables(rb)) != 45:
            raise AssertionError(f"refinement should not add a physical table: {len(base.canonical_tables(rb))}")
        if contract["Claims"].get("ThreeAtomBasisProved") is not False:
            raise AssertionError("three-atom basis must remain empirical")


def validate_repository_state() -> None:
    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    validate_state(rb, contract)
    print("traveling-salesman atom/operator refinement validation: PASS")
    print(f"loops={len(rows(rb, 'TSPLoops'))} tables={len(base.canonical_tables(rb))} atoms={len(active_primitives(rb))} operators={len(active_operators(rb))}")


def validate_summary_alignment() -> None:
    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    loops = {int(row["LoopOrder"]): row for row in rows(rb, "TSPLoops")}
    contract_loops = {int(row["LoopOrder"]): row for row in contract["Loops"]}
    if set(loops) != set(contract_loops):
        raise AssertionError("summary loop set mismatch")
    for order, row in loops.items():
        if contract_loops[order]["Status"] != row["Status"]:
            raise AssertionError(f"summary loop status mismatch at {order}")
    if loops.get(623, {}).get("Status") == "CLOSED":
        text = README.read_text()
        for marker in ("Loops 611–623", "ATTACHMENT", "Boundary Fiber", "24 directed Hamiltonian paths"):
            if marker not in text:
                raise AssertionError(f"README missing {marker!r}")
        claims = contract["Claims"]
        if claims.get("CurrentPredicateAtomCount") != 3 or claims.get("CurrentSemanticOperatorCount") != 4:
            raise AssertionError("contract current basis mismatch")
        if claims.get("ThreeAtomBasisProved") is not False:
            raise AssertionError("summary promoted an empirical basis to theorem")
    print("traveling-salesman atom/operator summary alignment: PASS")


def write_validation_files() -> None:
    VALIDATOR_V5.write_text(
        '''#!/usr/bin/env python3\nfrom apply_loops_611_623_refinement import validate_repository_state\n\nif __name__ == "__main__":\n    validate_repository_state()\n'''
    )
    SUMMARY_V5.write_text(
        '''#!/usr/bin/env python3\nfrom apply_loops_611_623_refinement import validate_summary_alignment\n\nif __name__ == "__main__":\n    validate_summary_alignment()\n'''
    )
    VALIDATOR_WRAPPER.write_text(
        '''#!/usr/bin/env python3\nfrom validate_rulebook_v3 import main as validate_v3\nfrom validate_rulebook_v5 import validate_repository_state\nfrom validate_summary_alignment_v5 import validate_summary_alignment\n\nif __name__ == "__main__":\n    validate_v3()\n    validate_repository_state()\n    validate_summary_alignment()\n'''
    )
    expected = [
        "TSP loops 611-623: register atom-operator refinement",
        *[COMMIT_MESSAGES[order] for order in range(611, 624)],
    ]
    expected_literal = "\n".join(f"              {message!r}," for message in expected)
    VALIDATION_WORKFLOW.write_text(
        f'''name: Validate TSP domain\n\non:\n  push:\n    branches:\n      - agent/tsp-semantic-foundation\n    paths:\n      - .github/workflows/validate-tsp-domain.yml\n      - rulebook-examples/effortless-math/domains/traveling-salesman/**\n  pull_request:\n    branches:\n      - main\n    paths:\n      - rulebook-examples/effortless-math/domains/traveling-salesman/**\n      - .github/workflows/validate-tsp-domain.yml\n\npermissions:\n  contents: read\n\njobs:\n  validate:\n    runs-on: ubuntu-latest\n    timeout-minutes: 25\n    steps:\n      - uses: actions/checkout@v4\n        with:\n          ref: ${{{{ github.head_ref || github.ref_name }}}}\n          fetch-depth: 0\n\n      - name: Validate semantic rulebook and summary projections\n        shell: bash\n        run: |\n          set -euo pipefail\n          domain='rulebook-examples/effortless-math/domains/traveling-salesman'\n          python3 "$domain/scripts/validate_rulebook.py"\n\n      - name: Compile replay and refinement programs\n        shell: bash\n        run: |\n          set -euo pipefail\n          domain='rulebook-examples/effortless-math/domains/traveling-salesman'\n          python3 -m py_compile \\\n            "$domain/scripts/apply_loops_597_610_convergence_v2.py" \\\n            "$domain/scripts/apply_loops_597_610_convergence_final3.py" \\\n            "$domain/scripts/apply_loops_611_623_refinement.py" \\\n            "$domain/scripts/reference_model.py" \\\n            "$domain/scripts/validate_rulebook.py" \\\n            "$domain/scripts/validate_rulebook_v3.py" \\\n            "$domain/scripts/validate_rulebook_v5.py" \\\n            "$domain/scripts/validate_summary_alignment_v5.py" \\\n            "$domain/testing/take-test.py"\n          bash -n "$domain/start.sh"\n\n      - name: Verify ordered refinement commits\n        shell: bash\n        run: |\n          set -euo pipefail\n          python3 - <<'PY2'\n          import subprocess\n          subjects = subprocess.check_output(['git', 'log', '--format=%s', '--reverse'], text=True).splitlines()\n          expected = [\n{expected_literal}\n          ]\n          positions = []\n          for message in expected:\n              if message not in subjects:\n                  raise AssertionError(f'missing commit: {{message}}')\n              positions.append(subjects.index(message))\n          if positions != sorted(positions) or len(set(positions)) != len(positions):\n              raise AssertionError(f'refinement commits are not in strict order: {{positions}}')\n          print('ordered TSP refinement commits: PASS')\n          PY2\n\n      - name: Ensure validation is read-only\n        shell: bash\n        run: |\n          test -z "$(git status --porcelain)"\n'''
    )


def save(rb: dict[str, Any], contract: dict[str, Any]) -> None:
    base.save(rb, contract)


def commit(message: str, paths: list[Path]) -> None:
    base.commit(message, paths)


def main() -> None:
    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    if base.meta_int(rb, "last_loop") < 610:
        raise AssertionError("loops 597-610 must be verified and canonical before refinement")
    loop_map = {int(row["LoopOrder"]): row for row in rows(rb, "TSPLoops")}
    if 611 not in loop_map:
        plan_loops(rb, contract)
        write_validation_files()
        save(rb, contract)
        validate_repository_state()
        commit(
            "TSP loops 611-623: register atom-operator refinement",
            [RULEBOOK, CONTRACT, VALIDATOR_V5, SUMMARY_V5, VALIDATOR_WRAPPER, VALIDATION_WORKFLOW],
        )
    for order in range(611, 624):
        rb = load(RULEBOOK)
        contract = load(CONTRACT)
        loop = next(row for row in rows(rb, "TSPLoops") if int(row["LoopOrder"]) == order)
        if loop["Status"] == "CLOSED":
            continue
        after, witness = LOOP_FUNCS[order](rb, contract)
        finish_loop(rb, contract, order, after, witness)
        save(rb, contract)
        validate_repository_state()
        paths = [RULEBOOK, CONTRACT]
        if order == 623:
            validate_summary_alignment()
            base.rebuild_postgres_and_record(contract)
            paths = [DOMAIN]
        commit(COMMIT_MESSAGES[order], paths)
    validate_repository_state()
    validate_summary_alignment()
    print("TSP atom/operator refinement loops 611-623: PASS")
    print("  current atoms: ATTACHMENT, VALUATION, WARRANT")
    print("  operators: CLOSURE, AGGREGATE, QUOTIENT, FIXPOINT")
    print("  asymmetric paths: 24 -> 12 -> 6")


if __name__ == "__main__":
    main()
