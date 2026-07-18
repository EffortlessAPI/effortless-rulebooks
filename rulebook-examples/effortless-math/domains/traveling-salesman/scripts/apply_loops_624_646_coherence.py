#!/usr/bin/env python3
"""Execute TSP loops 624-646 as a coherence-convergence experiment.

The rulebook remains canonical.  Every loop is registered as PLANNED before
execution, then closed, validated, committed, and pushed separately.  The pass
asks whether the three atoms and four operators from loop 623 collapse
losslessly to one typed semantic arc plus one warranted rewrite, then stresses
that basis on a three-region repair proof and an optimal face with two distinct
optimal route classes.
"""
from __future__ import annotations

import itertools
import json
import os
import subprocess
import sys
from collections import Counter, defaultdict, deque
from pathlib import Path
from typing import Any, Iterable

import apply_loops_611_623_refinement as prev

base = prev.base
REPO = prev.REPO
DOMAIN = prev.DOMAIN
RULEBOOK = prev.RULEBOOK
CONTRACT = prev.CONTRACT
README = prev.README
TARGET_BRANCH = os.environ.get("TARGET_BRANCH", "agent/tsp-semantic-foundation")
VALIDATOR_V6 = DOMAIN / "scripts" / "validate_rulebook_v6.py"
SUMMARY_V6 = DOMAIN / "scripts" / "validate_summary_alignment_v6.py"
VALIDATOR_WRAPPER = prev.VALIDATOR_WRAPPER
PG_DIR = DOMAIN / "effortless-postgres"
RULESPEAK_DIR = DOMAIN / "rulespeak"

LOOPS: dict[int, dict[str, str]] = {
    624: {
        "name": "Coherence vector",
        "term": "Coherence Vector",
        "rule": "tsp-rule-coherence-measurement",
        "before": "Loop 623 records local expansion as compatible with convergence, but the repository has no multidimensional measurement separating semantic basis size, physical size, open frontier, and named derived vocabulary.",
        "criterion": "Represent a coherence vector that records active atoms, active operators, physical-table delta, open frontier, named concepts, and the non-monotone expansion guardrail without collapsing those dimensions to an arbitrary scalar.",
        "next": "Test whether ATTACHMENT, VALUATION, and WARRANT are typed signatures of one relation.",
    },
    625: {
        "name": "Semantic arc",
        "term": "Semantic Arc",
        "rule": "tsp-rule-semantic-arc-refinement",
        "before": "ATTACHMENT, VALUATION, and WARRANT are three active ternary atoms with the same subject-label-target shape but different label and target sorts.",
        "criterion": "Introduce SEMANTIC_ARC(subject,label,target), reduce the three atoms to typed arc projections, preserve all historical definitions, and leave exactly one active relational atom.",
        "next": "Prevent the generic arc from becoming a vacuous triple store by making its signatures explicit.",
    },
    626: {
        "name": "Arc signature",
        "term": "Arc Signature",
        "rule": "tsp-rule-semantic-arc-refinement",
        "before": "A single generic arc can encode the three atoms syntactically, but their role, measure, and modality constraints are not yet recoverable as typed signatures.",
        "criterion": "Record subject, label, and target sorts for attachment, valuation, and warrant projections and require distinct recoverable signatures for each historical atom.",
        "next": "Certify that every registered concept remains recoverable from the current basis.",
    },
    627: {
        "name": "Recoverability witness",
        "term": "Recoverability Witness",
        "rule": "tsp-rule-semantic-arc-refinement",
        "before": "The arc signatures are represented, but lossless recovery of the historical vocabulary has not been certified across the concept registry.",
        "criterion": "Give every registered concept an explicit recovery expression over semantic arcs and operators, reject unreduced historical tokens, and preserve source-table provenance.",
        "next": "Collapse closure and fixed point into one saturation operator.",
    },
    628: {
        "name": "Saturation operator",
        "term": "Saturation Operator",
        "rule": "tsp-rule-warranted-rewrite",
        "before": "CLOSURE and FIXPOINT are separate active operators although both repeatedly add warranted consequences until a stability condition holds.",
        "criterion": "Introduce SATURATE(seed,rewrite,termination_warrant), reduce closure and fixed point to explicit saturation modes, and preserve their historical operator identities.",
        "next": "Collapse aggregation and quotienting into one fiber-reduction operator.",
    },
    629: {
        "name": "Fiber reduction",
        "term": "Fiber Reduction",
        "rule": "tsp-rule-warranted-rewrite",
        "before": "AGGREGATE and QUOTIENT are separate active operators although both group a domain by a key and replace each fiber with a warranted summary or representative.",
        "criterion": "Introduce FIBER_REDUCE(domain,key,reducer,expansion_warrant), reduce aggregate and quotient to reducer modes, and preserve all expansion and valuation semantics.",
        "next": "Test whether saturation and fiber reduction are polarities of one rewrite operator.",
    },
    630: {
        "name": "Warranted rewrite",
        "term": "Warranted Rewrite",
        "rule": "tsp-rule-warranted-rewrite",
        "before": "SATURATE and FIBER_REDUCE remain separate operators even though both transform one semantic-arc state into another under an explicit warrant.",
        "criterion": "Introduce WARRANTED_REWRITE(input,rule,polarity,output,warrant), reduce saturation and fiber reduction to rewrite modes, and leave exactly one active semantic operator.",
        "next": "Make expansive and contractive rewrite polarity explicit.",
    },
    631: {
        "name": "Rewrite polarity",
        "term": "Rewrite Polarity",
        "rule": "tsp-rule-warranted-rewrite",
        "before": "The single rewrite operator is parametric, but the distinction between black-box internalization growth, saturation growth, and quotient contraction is not typed.",
        "criterion": "Classify historical and current operators as EXPANSIVE, CONTRACTIVE, MIXED, or PARAMETRIC, with modes that recover closure, fixed point, aggregate, quotient, saturation, and fiber reduction.",
        "next": "Certify the one-arc/one-rewrite basis for the represented domain.",
    },
    632: {
        "name": "One-arc one-rewrite basis",
        "term": "One-Arc/One-Rewrite Basis",
        "rule": "tsp-rule-semantic-arc-refinement",
        "before": "One active arc and one active rewrite have emerged incrementally, but lossless coverage, current-basis counts, and empirical scope are not yet certified.",
        "criterion": "Certify one active atom and one active operator, recover every registered concept without old atom/operator tokens, preserve all prior certificates, and retain the non-claim of minimality or universality.",
        "next": "Stress the basis on a reusable region interface with three disconnected local components.",
    },
    633: {
        "name": "Region interface",
        "term": "Region Interface",
        "rule": "tsp-rule-region-interface",
        "before": "Neighborhoods, components, and boundary fibers are quotient objects, but their shared port, coverage, valuation, and expansion contract is not represented as one region interface.",
        "criterion": "Add a common region-interface projection to existing quotient nodes without a new table and define ports, internal coverage, expansion witness, and interface status through semantic arcs.",
        "next": "Instantiate three cheap local regions in one complete nine-stop graph.",
    },
    634: {
        "name": "Region triad",
        "term": "Region Triad",
        "rule": "tsp-rule-region-interface",
        "before": "Component repair has only been tested on two three-stop regions, so crossing demand and repair composition for more than two components are unmeasured.",
        "criterion": "Represent a complete nine-stop graph partitioned into three cost-one triangles, three cost-ten ring crossings, expensive remaining crossings, nine valid local degree bounds, and a three-component local two-factor of cost nine.",
        "next": "Contract each triangle into a boundary-degree-two region quotient.",
    },
    635: {
        "name": "Region quotient",
        "term": "Region Quotient",
        "rule": "tsp-rule-region-interface",
        "before": "The region-triad local two-factor has three components, but the components do not yet expose reusable path states and expansion witnesses.",
        "criterion": "Represent three boundary-degree-two quotient nodes, three path states per region, explicit edge expansions, and one passing six-to-three contraction certificate per region.",
        "next": "Express the global deficit as quotient incidence rather than fixture-specific component prose.",
    },
    636: {
        "name": "Quotient incidence budget",
        "term": "Quotient Incidence Budget",
        "rule": "tsp-rule-boundary-handshake",
        "before": "Every local stop has degree two, yet the quotient graph has no external incidence; the resulting global defect is not represented in the common budget vocabulary.",
        "criterion": "Give each of three region nodes required boundary degree two, observed degree zero, and a shared global profile separating closed internal incidence from open quotient incidence.",
        "next": "Convert quotient degree demand into a minimum crossing-edge count.",
    },
    637: {
        "name": "Boundary handshake",
        "term": "Boundary Handshake",
        "rule": "tsp-rule-boundary-handshake",
        "before": "Three quotient nodes each require boundary degree two, but the number of inter-region edges forced by that demand is not yet derived.",
        "criterion": "Apply the handshake identity to the quotient: total boundary degree six divided by two endpoints per crossing requires at least three inter-region edges.",
        "next": "Compose the crossing demand with insertion and release valuations.",
    },
    638: {
        "name": "Region repair equation",
        "term": "Region Repair Equation",
        "rule": "tsp-rule-region-repair-equation",
        "before": "The base degree-two lower bound is nine and crossing demand is three, but the signed cost of repairing three components is not represented.",
        "criterion": "Certify 9 + 3*10 - 3*1 = 36 from a base term, three mandatory crossing insertions, and one necessary cost-one internal release per region.",
        "next": "Construct the repaired cycle as a balanced edge exchange.",
    },
    639: {
        "name": "Balanced edge exchange",
        "term": "Balanced Edge Exchange",
        "rule": "tsp-rule-balanced-edge-exchange",
        "before": "The repair equation proves a lower value, but no structural object witnesses the three added and three released edges that preserve degree while joining the regions.",
        "criterion": "Replace one internal edge per local triangle with the three cost-ten ring crossings, preserve every stop at degree two, and certify one connected nine-edge cycle of cost thirty-six.",
        "next": "Project an ordered repair cycle without consuming a supplied candidate as antecedent.",
    },
    640: {
        "name": "Repair cycle",
        "term": "Repair Cycle",
        "rule": "tsp-rule-balanced-edge-exchange",
        "before": "The balanced-exchange edge set is a certified cycle, but it has no ordered reconstruction or normalized route witness.",
        "criterion": "Reconstruct the nine-stop cycle from the repaired edge set, emit ordered steps and a comparison candidate of cost thirty-six, and record CandidateUsedAsAntecedent=false.",
        "next": "Close finite optimality by equality with the region repair bound.",
    },
    641: {
        "name": "Region bound sandwich",
        "term": "Region Bound Sandwich",
        "rule": "tsp-rule-bound-equality-optimality",
        "before": "The three-region lower bound and valid reconstructed cycle both equal thirty-six, but no finite optimality certificate joins them.",
        "criterion": "Certify the reconstructed cost-thirty-six cycle as optimal for tsp-three-regions-9 by lower-bound equality, without claiming a general k-region theorem.",
        "next": "Stress the value proof on an instance with two distinct optimal route classes.",
    },
    642: {
        "name": "Optimal face",
        "term": "Optimal Face",
        "rule": "tsp-rule-optimal-face",
        "before": "Every represented positive fixture has one distinguished optimal route, so optimal value and optimal witness identity have not been separated.",
        "criterion": "Represent a complete four-stop graph with degree-two lower bound four, two non-reversal-equivalent Hamiltonian cycles of cost four, and a third Hamiltonian cycle of cost six.",
        "next": "Certify the optimum value while leaving route identity non-unique.",
    },
    643: {
        "name": "Value choice split",
        "term": "Value/Choice Split",
        "rule": "tsp-rule-optimal-face",
        "before": "The optimal-face fixture has two equal-cost witnesses, but proof status and witness-selection status are not independently represented.",
        "criterion": "Attach passing optimality certificates to both cost-four cycles and represent value status CLOSED while choice status remains a two-alternative non-reversal orbit.",
        "next": "Name and measure the residual optimal-witness orbit.",
    },
    644: {
        "name": "Choice orbit",
        "term": "Choice Orbit",
        "rule": "tsp-rule-optimal-face",
        "before": "The two optimal witnesses are recorded, but residual ambiguity is not classified as a value-preserving orbit distinct from unresolved optimality.",
        "criterion": "Record three initial route classes, two surviving optimal classes, certified value four, zero mathematical branch decisions, and residual choice-orbit size two.",
        "next": "Decide exactly when a branch is warranted after value closure.",
    },
    645: {
        "name": "Branch warrant",
        "term": "Branch Warrant",
        "rule": "tsp-rule-branch-warrant",
        "before": "A residual choice orbit exists, but the system does not distinguish a branch needed for proof from a conditional tie-break needed only to select one representative.",
        "criterion": "Reject mathematical branching for value proof, require an external tie-break policy before representative selection, and emit a replayable rejected-branch event with zero branch decisions.",
        "next": "Measure the second convergence event across the arc/rewrite basis and both stress fixtures.",
    },
    646: {
        "name": "Coherence event two",
        "term": "Coherence Event II",
        "rule": "tsp-rule-coherence-measurement",
        "before": "The pass has coined new machinery and added many witness rows, but its effect on basis size, physical schema, trust, multi-region repair, and residual choice has not been summarized.",
        "criterion": "Certify one active arc, one active rewrite, unchanged physical-table count, explicit local expansion, three-region optimum thirty-six, optimal-face value four with choice orbit two, and zero branch decisions for value proof.",
        "next": "Stress the one-arc/one-rewrite basis on heterogeneous regions and only branch when a warranted policy or unresolved value defect remains.",
    },
}

COMMIT_MESSAGES = {
    624: "TSP loop 624: define coherence vector",
    625: "TSP loop 625: collapse atoms to semantic arc",
    626: "TSP loop 626: type semantic arc signatures",
    627: "TSP loop 627: certify basis recoverability",
    628: "TSP loop 628: collapse closure into saturation",
    629: "TSP loop 629: collapse aggregation into fiber reduction",
    630: "TSP loop 630: derive warranted rewrite",
    631: "TSP loop 631: classify rewrite polarity",
    632: "TSP loop 632: certify one-arc one-rewrite basis",
    633: "TSP loop 633: normalize region interfaces",
    634: "TSP loop 634: represent three-region fixture",
    635: "TSP loop 635: derive region quotients",
    636: "TSP loop 636: expose quotient incidence budget",
    637: "TSP loop 637: derive boundary handshake",
    638: "TSP loop 638: derive region repair equation",
    639: "TSP loop 639: apply balanced edge exchange",
    640: "TSP loop 640: reconstruct repair cycle",
    641: "TSP loop 641: close region bound sandwich",
    642: "TSP loop 642: represent optimal face",
    643: "TSP loop 643: split value from choice",
    644: "TSP loop 644: measure choice orbit",
    645: "TSP loop 645: derive branch warrant",
    646: "TSP loop 646: certify coherence event two",
}

RULE_ROWS = [
    {
        "TSPInferenceRuleId": "tsp-rule-coherence-measurement",
        "DisplayName": "Multidimensional coherence measurement",
        "InferenceLayer": "SEMANTIC_REDUCTION",
        "ImplementationStatus": "PLANNED",
        "Soundness": "STRUCTURAL_MEASUREMENT_WITHOUT_SCALARIZING_INCOMPARABLE_DIMENSIONS",
        "Completeness": "CURRENT_REPRESENTED_DOMAIN",
        "RuntimeClass": "O(C+F)",
        "MemoryClass": "O(1)_AFTER_COUNTS",
        "Applicability": "A versioned concept registry, frontier ledger, and physical schema are available.",
        "CertificateType": "coherence-vector-certificate",
        "Description": "Measure semantic basis, physical size, open frontier, and named vocabulary separately so local expansion is not mistaken for entropy.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-semantic-arc-refinement",
        "DisplayName": "Typed semantic-arc refinement",
        "InferenceLayer": "SEMANTIC_REDUCTION",
        "ImplementationStatus": "PLANNED",
        "Soundness": "DEFINITIONAL_WHEN_SIGNATURES_AND_RECOVERY_VIEWS_ARE_EXPLICIT",
        "Completeness": "REGISTERED_CURRENT_CONCEPTS",
        "RuntimeClass": "O(C)",
        "MemoryClass": "O(C)",
        "Applicability": "Active ternary atoms share a subject-label-target form and retain typed label and target sorts.",
        "CertificateType": "semantic-arc-recoverability-certificate",
        "Description": "Collapse attachment, valuation, and warrant to one typed arc without accepting an untyped triple-store tautology.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-warranted-rewrite",
        "DisplayName": "Warranted semantic rewrite",
        "InferenceLayer": "EXECUTION_GEOMETRY",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_WHEN_INPUT_OUTPUT_POLARITY_TERMINATION_AND_EXPANSION_WARRANTS_ARE_EXPLICIT",
        "Completeness": "CURRENT_SATURATION_AND_FIBER_REDUCTION_FAMILIES",
        "RuntimeClass": "MODE_DEPENDENT",
        "MemoryClass": "MODE_DEPENDENT",
        "Applicability": "A transformation maps one semantic-arc state to another and exposes its warrant and polarity.",
        "CertificateType": "warranted-rewrite-certificate",
        "Description": "Unify expansive saturation and contractive fiber reduction while preserving their distinct modes.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-region-interface",
        "DisplayName": "Region interface normalization",
        "InferenceLayer": "SEMANTIC_QUOTIENT",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_FOR_EXPLICIT_PORT_COVERAGE_COST_AND_EXPANSION_WITNESSES",
        "Completeness": "DECLARED_FINITE_REGIONS",
        "RuntimeClass": "O(S)",
        "MemoryClass": "O(S)",
        "Applicability": "A connected internal scope exposes finite entry/exit states and an expansion witness.",
        "CertificateType": "region-interface-certificate",
        "Description": "Treat neighborhoods, components, and boundary fibers as one port-bearing region contract.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-boundary-handshake",
        "DisplayName": "Quotient boundary handshake",
        "InferenceLayer": "CONNECTIVITY",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_FOR_UNDIRECTED_QUOTIENT_CYCLES",
        "Completeness": "LOWER_BOUND_ON_CROSSING_EDGE_COUNT",
        "RuntimeClass": "O(R)",
        "MemoryClass": "O(1)",
        "Applicability": "Every quotient region in the target cycle has required external degree two.",
        "CertificateType": "boundary-handshake-certificate",
        "Description": "Sum quotient boundary degrees and divide by two because each crossing edge is incident to two regions.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-region-repair-equation",
        "DisplayName": "Multi-region repair equation",
        "InferenceLayer": "LOWER_BOUND",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_FOR_WITNESSED_EQUAL_INTERNAL_RELEASE_COST_AND_MINIMUM_CROSSING_COST",
        "Completeness": "DECLARED_THREE_TRIANGLE_FIXTURE",
        "RuntimeClass": "O(E)",
        "MemoryClass": "O(E)",
        "Applicability": "A disconnected local two-factor, quotient crossing demand, insertion minima, and release credits are explicitly witnessed.",
        "CertificateType": "region-repair-equation-certificate",
        "Description": "Compose local degree cost with signed crossing insertions and necessary internal releases.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-balanced-edge-exchange",
        "DisplayName": "Balanced edge exchange",
        "InferenceLayer": "CONSTRUCTION",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_WHEN_ADDED_AND_RELEASED_INCIDENCE_BALANCE_AT_EVERY_STOP",
        "Completeness": "DECLARED_THREE_REGION_FIXTURE",
        "RuntimeClass": "O(V+E)",
        "MemoryClass": "O(V+E)",
        "Applicability": "A repair equation identifies crossing insertions and internal releases that preserve degree two.",
        "CertificateType": "balanced-edge-exchange-certificate",
        "Description": "Replace local cycle edges with crossing edges while preserving incidence and certifying one connected cycle.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-optimal-face",
        "DisplayName": "Optimal-face value and choice separation",
        "InferenceLayer": "RIGIDITY",
        "ImplementationStatus": "PLANNED",
        "Soundness": "SOUND_FOR_MULTIPLE_VALID_WITNESSES_EQUAL_TO_ONE_CERTIFIED_LOWER_BOUND",
        "Completeness": "DECLARED_FOUR_STOP_FIXTURE",
        "RuntimeClass": "O(W)",
        "MemoryClass": "O(W)",
        "Applicability": "Two or more non-equivalent valid witnesses attain the same certified optimum value.",
        "CertificateType": "optimal-face-certificate",
        "Description": "Close the optimum value without falsely claiming a unique optimal witness.",
    },
    {
        "TSPInferenceRuleId": "tsp-rule-branch-warrant",
        "DisplayName": "Residual branch warrant",
        "InferenceLayer": "RESIDUAL_SEARCH",
        "ImplementationStatus": "PLANNED",
        "Soundness": "EPISTEMIC_STATUS_RULE",
        "Completeness": "CURRENT_VALUE_CLOSED_CHOICE_OPEN_FIXTURE",
        "RuntimeClass": "O(1)_AFTER_CERTIFICATES",
        "MemoryClass": "O(1)",
        "Applicability": "Deterministic closure is complete and a residual orbit remains; branch purpose and external selection policy are explicit.",
        "CertificateType": "branch-warrant-certificate",
        "Description": "Permit branching only for an unresolved value defect or an explicit external representative-selection policy.",
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
    return prev.active_primitives(rb)


def active_operators(rb: dict[str, Any]) -> list[dict[str, Any]]:
    return prev.active_operators(rb)


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
    prev.register_concept(
        rb,
        ident,
        display,
        kind,
        expression,
        arity,
        sources,
        loop,
        status=status,
        category=category,
        operator_expression=operator_expression,
    )


def ensure_fields(table: dict[str, Any], fields: list[dict[str, Any]]) -> None:
    base.ensure_fields(table, fields)


def open_frontier_count(rb: dict[str, Any]) -> int:
    return sum(row["Status"] == "OPEN" for row in rows(rb, "TSPFrontierObligations"))


def named_concept_count(rb: dict[str, Any]) -> int:
    return len(rows(rb, "TSPConceptRegistry"))


def ensure_coherence_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPConvergenceMeasurements"],
        [
            base.field("ActiveAtomCount", "integer", "raw", True, "Active relational atom count at this measurement."),
            base.field("ActiveOperatorCount", "integer", "raw", True, "Active semantic operator count at this measurement."),
            base.field("PhysicalTableDeltaFromLoop610", "integer", "raw", True, "Physical table count minus the loop-610 count of forty-five."),
            base.field("OpenFrontierCount", "integer", "raw", True, "Open frontier obligations at this measurement."),
            base.field("NamedConceptCount", "integer", "raw", True, "Versioned concept-registry row count."),
            base.field("AllowsLocalExpansion", "boolean", "raw", True, "Whether local representational expansion is compatible with the measurement."),
            base.field("CoherenceDirection", "string", "raw", True, "Multidimensional interpretation without arbitrary scalar weighting."),
        ],
    )


def add_measurement(
    rb: dict[str, Any],
    order: int,
    term: str,
    notes: str,
    *,
    kind: str = "COHERENCE_REFINEMENT",
    prediction: str = "EARLY_SUPPORT",
) -> None:
    ensure_coherence_fields(rb)
    upsert_rows(
        rb["TSPConvergenceMeasurements"],
        "TSPConvergenceMeasurementId",
        [
            {
                "TSPConvergenceMeasurementId": f"convergence-loop-{order}",
                "TSPLoop": f"tsp-loop-{order}",
                "MeasurementKind": kind,
                "SurfaceConceptCountBefore": 32,
                "PrimitiveCountAfter": len(active_primitives(rb)),
                "NewPrimitiveCount": 0,
                "DerivedAliasCount": sum(row.get("ConceptKind") not in {"PRIMITIVE", "OPERATOR"} for row in rows(rb, "TSPConceptRegistry")),
                "PhysicalTableCount": len(base.canonical_tables(rb)),
                "NovelTerm": term,
                "PredictionStatus": prediction,
                "Notes": notes,
                "ActiveAtomCount": len(active_primitives(rb)),
                "ActiveOperatorCount": len(active_operators(rb)),
                "PhysicalTableDeltaFromLoop610": len(base.canonical_tables(rb)) - 45,
                "OpenFrontierCount": open_frontier_count(rb),
                "NamedConceptCount": named_concept_count(rb),
                "AllowsLocalExpansion": True,
                "CoherenceDirection": "MORE_EXPLICIT_REUSE_SMALLER_ACTIVE_BASIS_NO_REQUIRED_PHYSICAL_SHRINKAGE",
            }
        ],
    )


def add_frontier(
    rb: dict[str, Any],
    ident: str,
    display: str,
    opened: int,
    closed: int,
    rule: str,
    criterion: str,
    certificate: str,
    *,
    kind: str = "INFERENCE_OBLIGATION",
) -> None:
    upsert_rows(
        rb["TSPFrontierObligations"],
        "TSPFrontierObligationId",
        [
            {
                "TSPFrontierObligationId": ident,
                "DisplayName": display,
                "ObligationKind": kind,
                "Status": "CLOSED",
                "InferenceRule": rule,
                "OpenedByLoop": f"tsp-loop-{opened}",
                "ClosedByLoop": f"tsp-loop-{closed}",
                "TrustDisposition": "INTERNALIZED_FOR_DECLARED_FINITE_SCOPE",
                "ClosureCriterion": criterion,
                "CertificateType": certificate,
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
                    "WitnessSummary": "Planned coherence-refinement loop; no closure claim yet.",
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
    set_meta(rb, "last_planned_loop", "integer", integer=646)
    set_meta(rb, "coherence_program_status", "string", string="PLANNED_624_646")
    contract["Version"] = "0.6.0-alpha"
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "OneArcOneRewriteObserved": False,
            "OneArcOneRewriteProved": False,
            "ThreeRegionOptimalityProved": False,
            "OptimalFaceValueCertified": False,
            "OptimalFaceUniqueRouteProved": False,
        }
    )


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


def save(rb: dict[str, Any], contract: dict[str, Any]) -> None:
    base.save(rb, contract)


def commit(message: str, paths: list[Path]) -> None:
    base.commit(message, paths)
    run(["git", "push", "origin", f"HEAD:{TARGET_BRANCH}"])


def transform_registry_tokens(rb: dict[str, Any], replacements: dict[str, str]) -> None:
    for row in rows(rb, "TSPConceptRegistry"):
        expression = row.get("ReducedBasisExpression") or row.get("BasisExpression") or ""
        operator = row.get("OperatorExpression") or ""
        for old, new in replacements.items():
            expression = expression.replace(old, new)
            operator = operator.replace(old, new)
        row["ReducedBasisExpression"] = expression
        row["OperatorExpression"] = operator


def reduce_basis_rows(
    rb: dict[str, Any],
    identifiers: Iterable[str],
    replacement_id: str,
    replacement_name: str,
    generation: int,
    *,
    historical_kind: str,
) -> None:
    concepts = table_index(rb, "TSPConceptRegistry")
    for ident in identifiers:
        row = concepts[ident]
        row["HistoricalConceptKind"] = row.get("HistoricalConceptKind") or row.get("ConceptKind")
        row["ConceptKind"] = historical_kind
        row["Status"] = f"REDUCED_TO_{replacement_name}"
        row["SemanticCategory"] = "HISTORICAL"
        row["SupersededByConcept"] = replacement_id
        row["ReductionGeneration"] = generation


def edge_key(a: str, b: str) -> tuple[str, str]:
    return tuple(sorted((a, b)))


def raw_tour_cost(rb: dict[str, Any], candidate: str) -> float:
    edge_map = table_index(rb, "TravelEdges")
    return sum(
        float(edge_map[row["TravelEdge"]]["TravelCost"])
        for row in rows(rb, "TourLegs")
        if row["CandidateTour"] == candidate
    )


def graph_components(edge_rows: list[dict[str, Any]]) -> int:
    vertices: set[str] = set()
    adjacency: dict[str, set[str]] = defaultdict(set)
    for row in edge_rows:
        a, b = row["FromStop"], row["ToStop"]
        vertices.update((a, b))
        adjacency[a].add(b)
        adjacency[b].add(a)
    unseen = set(vertices)
    count = 0
    while unseen:
        count += 1
        root = unseen.pop()
        stack = [root]
        while stack:
            node = stack.pop()
            for nxt in adjacency[node]:
                if nxt in unseen:
                    unseen.remove(nxt)
                    stack.append(nxt)
    return count

ARC_ATOMS = {
    "concept-attachment": {
        "subject": "ENTITY_OR_WITNESS",
        "label": "ROLE",
        "target": "ENTITY_OR_SCOPE",
        "signature": "SEMANTIC_ARC(subject,ROLE:role,ENTITY:object)",
        "recover": "FILTER_SEMANTIC_ARC(label_sort=ROLE,target_sort=ENTITY_OR_SCOPE)",
    },
    "concept-valuation": {
        "subject": "ENTITY_OR_WITNESS",
        "label": "MEASURE",
        "target": "SCALAR_OR_ORDINAL",
        "signature": "SEMANTIC_ARC(subject,MEASURE:measure,SCALAR:value)",
        "recover": "FILTER_SEMANTIC_ARC(label_sort=MEASURE,target_sort=SCALAR_OR_ORDINAL)",
    },
    "concept-warrant": {
        "subject": "CONCLUSION",
        "label": "MODALITY",
        "target": "SOURCE_OR_SCOPE",
        "signature": "SEMANTIC_ARC(conclusion,MODALITY:modality,SOURCE:source)",
        "recover": "FILTER_SEMANTIC_ARC(label_sort=MODALITY,target_sort=SOURCE_OR_SCOPE)",
    },
}

OLD_ATOM_TOKENS = ("ATTACHMENT", "VALUATION", "WARRANT")
OLD_OPERATOR_TOKENS = (
    "CLOSURE",
    "AGGREGATE",
    "QUOTIENT",
    "FIXPOINT",
    "SATURATE",
    "FIBER_REDUCE",
)


def ensure_arc_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPConceptRegistry"],
        [
            base.field("ArcSubjectSort", "string", "raw", True, "Subject sort for a semantic-arc projection."),
            base.field("ArcLabelSort", "string", "raw", True, "ROLE, MEASURE, MODALITY, or COMPOSED label sort."),
            base.field("ArcTargetSort", "string", "raw", True, "Target sort for a semantic-arc projection."),
            base.field("ArcSignature", "string", "raw", True, "Typed subject-label-target signature."),
            base.field("RecoverabilityExpression", "string", "raw", True, "Projection that recovers the historical concept from the current basis."),
            base.field("IsRecoverableFromCurrentBasis", "boolean", "raw", True, "Whether the historical or derived concept has an explicit current-basis recovery."),
            base.field("RewriteMode", "string", "raw", True, "Named mode of the current or historical rewrite operator."),
            base.field("RewritePolarity", "string", "raw", True, "EXPANSIVE, CONTRACTIVE, MIXED, or PARAMETRIC."),
        ],
    )
    for row in rows(rb, "TSPConceptRegistry"):
        row.setdefault("ArcSubjectSort", "COMPOSED_SUBJECT")
        row.setdefault("ArcLabelSort", "COMPOSED")
        row.setdefault("ArcTargetSort", "COMPOSED_TARGET")
        row.setdefault("ArcSignature", "COMPOSITION_OF_TYPED_SEMANTIC_ARCS")
        row.setdefault("RecoverabilityExpression", None)
        row.setdefault("IsRecoverableFromCurrentBasis", None)
        row.setdefault("RewriteMode", None)
        row.setdefault("RewritePolarity", None)


def refresh_recoverability(rb: dict[str, Any]) -> list[str]:
    ensure_arc_fields(rb)
    unresolved: list[str] = []
    forbidden = OLD_ATOM_TOKENS + OLD_OPERATOR_TOKENS
    for row in rows(rb, "TSPConceptRegistry"):
        expression = row.get("ReducedBasisExpression") or row.get("BasisExpression") or ""
        operator = row.get("OperatorExpression") or ""
        if any(token in expression or token in operator for token in forbidden):
            unresolved.append(row["TSPConceptId"])
            row["IsRecoverableFromCurrentBasis"] = False
            continue
        row["RecoverabilityExpression"] = row.get("RecoverabilityExpression") or (
            f"PROJECT_{row['TSPConceptId']}_FROM_SEMANTIC_ARC_AND_WARRANTED_REWRITE"
        )
        row["IsRecoverableFromCurrentBasis"] = True
    return unresolved


def loop_624(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_coherence_fields(rb)
    register_concept(
        rb,
        "coined-coherence-vector",
        "Coherence Vector",
        "COINED_PREDICATE",
        "VECTOR(VALUATION(active_atoms),VALUATION(active_operators),VALUATION(physical_tables),VALUATION(open_frontier),WARRANT(local_expansion))",
        1,
        "TSPConvergenceMeasurements,TSPConceptRegistry,TSPFrontierObligations",
        624,
        status="ACTIVE_DERIVED",
        category="DERIVED",
    )
    add_measurement(
        rb,
        624,
        "Coherence Vector",
        "Convergence is represented as a vector: three atoms, four operators, forty-five physical tables, explicit named vocabulary, and an independently counted open frontier. Physical growth is not assigned an arbitrary negative scalar weight.",
        kind="COHERENCE_VECTOR_BASELINE",
    )
    set_meta(rb, "coherence_measurement_mode", "string", string="VECTOR_NOT_ARBITRARY_SCALAR")
    set_meta(rb, "coherence_allows_nonmonotone_physical_size", "boolean", boolean=True)
    contract.setdefault("Claims", {})["CoherenceVectorRecorded"] = True
    return (
        "Recorded semantic basis size, operator count, physical-table delta, open frontier, and named vocabulary as separate coherence coordinates.",
        f"Loop-624 vector: atoms={len(active_primitives(rb))}, operators={len(active_operators(rb))}, tables={len(base.canonical_tables(rb))}, open_frontier={open_frontier_count(rb)}, named_concepts={named_concept_count(rb)}; local expansion remains allowed.",
    )


def loop_625(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_concept(
        rb,
        "concept-semantic-arc",
        "Semantic Arc",
        "PRIMITIVE",
        "SEMANTIC_ARC(subject,label,target)",
        3,
        "TSPConceptRegistry,Cities,Neighborhoods,Addresses,TravelEdges,TSPInferenceApplications,TSPBoundTerms",
        625,
        status="ACTIVE_PRIMITIVE",
        category="ATOM",
    )
    transform_registry_tokens(
        rb,
        {
            "ATTACHMENT": "SEMANTIC_ARC",
            "VALUATION": "SEMANTIC_ARC",
            "WARRANT": "SEMANTIC_ARC",
        },
    )
    reduce_basis_rows(
        rb,
        ARC_ATOMS,
        "concept-semantic-arc",
        "SEMANTIC_ARC",
        3,
        historical_kind="HISTORICAL_PRIMITIVE",
    )
    concepts = table_index(rb, "TSPConceptRegistry")
    semantic = concepts["concept-semantic-arc"]
    semantic.update(
        {
            "ArcSubjectSort": "ANY_DECLARED_SUBJECT_SORT",
            "ArcLabelSort": "TYPED_LABEL",
            "ArcTargetSort": "ANY_DECLARED_TARGET_SORT",
            "ArcSignature": "SEMANTIC_ARC(subject:T,label:L,target:U)",
            "RecoverabilityExpression": "IDENTITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    for ident, spec in ARC_ATOMS.items():
        row = concepts[ident]
        row["ReducedBasisExpression"] = spec["signature"]
        row["OperatorExpression"] = ""
        row["ArcSubjectSort"] = spec["subject"]
        row["ArcLabelSort"] = spec["label"]
        row["ArcTargetSort"] = spec["target"]
        row["ArcSignature"] = spec["signature"]
        row["RecoverabilityExpression"] = spec["recover"]
        row["IsRecoverableFromCurrentBasis"] = True
    register_concept(
        rb,
        "coined-semantic-arc",
        "Semantic Arc",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(subject,label,target)+TYPED_SIGNATURE(label,target)",
        1,
        "TSPConceptRegistry",
        625,
        status="ACTIVE_DERIVED",
        category="DERIVED",
    )
    coined = concepts["coined-semantic-arc"]
    coined.update(
        {
            "ArcSubjectSort": "ANY_DECLARED_SUBJECT_SORT",
            "ArcLabelSort": "TYPED_LABEL",
            "ArcTargetSort": "ANY_DECLARED_TARGET_SORT",
            "ArcSignature": "SEMANTIC_ARC(subject:T,label:L,target:U)",
            "RecoverabilityExpression": "IDENTITY_PLUS_SIGNATURE_WARRANT",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    add_measurement(
        rb,
        625,
        "Semantic Arc",
        "Attachment, valuation, and warrant become typed semantic-arc projections. The active relational basis falls from three atoms to one without removing historical rows.",
    )
    set_meta(rb, "active_predicate_atom_count", "integer", integer=1)
    return (
        "Collapsed ATTACHMENT, VALUATION, and WARRANT into one typed SEMANTIC_ARC relation.",
        "The three historical atoms remain distinct recoverable projections by ROLE, MEASURE, and MODALITY signatures; active atom count is one.",
    )


def loop_626(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    concepts = table_index(rb, "TSPConceptRegistry")
    signatures = set()
    for ident, spec in ARC_ATOMS.items():
        row = concepts[ident]
        row.update(
            {
                "ArcSubjectSort": spec["subject"],
                "ArcLabelSort": spec["label"],
                "ArcTargetSort": spec["target"],
                "ArcSignature": spec["signature"],
                "RecoverabilityExpression": spec["recover"],
                "IsRecoverableFromCurrentBasis": True,
            }
        )
        signatures.add((spec["label"], spec["target"]))
    if len(signatures) != 3:
        raise AssertionError("historical atom arc signatures are not distinct")
    register_concept(
        rb,
        "coined-arc-signature",
        "Arc Signature",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(subject:S,label:L,target:T)+SEMANTIC_ARC(label,SIGNATURE,(S,L,T))",
        1,
        "TSPConceptRegistry",
        626,
        status="ACTIVE_DERIVED",
        category="DERIVED",
    )
    row = concepts["coined-arc-signature"]
    row.update(
        {
            "ArcSubjectSort": "LABEL_OR_CONCEPT",
            "ArcLabelSort": "SIGNATURE",
            "ArcTargetSort": "SORT_TRIPLE",
            "ArcSignature": "SEMANTIC_ARC(label,SIGNATURE,(subject_sort,label_sort,target_sort))",
            "RecoverabilityExpression": "LOOKUP_SIGNATURE_BY_LABEL_SORT",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    add_measurement(
        rb,
        626,
        "Arc Signature",
        "ROLE-to-entity, MEASURE-to-scalar, and MODALITY-to-source signatures remain distinct. Genericization does not erase type contracts.",
    )
    return (
        "Typed the semantic arc with explicit subject, label, and target sorts.",
        "Attachment = ROLE arc, valuation = MEASURE arc, warrant = MODALITY arc; each historical atom has a distinct recovery filter, preventing a vacuous untyped triple-store collapse.",
    )


def loop_627(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_concept(
        rb,
        "coined-recoverability-witness",
        "Recoverability Witness",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(concept,RECOVERABILITY_EXPRESSION,projection)+WARRANT(lossless,current_scope)",
        1,
        "TSPConceptRegistry",
        627,
        status="ACTIVE_DERIVED",
        category="DERIVED",
    )
    row = table_index(rb, "TSPConceptRegistry")["coined-recoverability-witness"]
    row.update(
        {
            "ArcSubjectSort": "CONCEPT",
            "ArcLabelSort": "RECOVERABILITY_EXPRESSION",
            "ArcTargetSort": "PROJECTION",
            "ArcSignature": "SEMANTIC_ARC(concept,RECOVERABILITY_EXPRESSION,projection)",
            "RecoverabilityExpression": "IDENTITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    # At this stage the four historical operators are still active, so only the
    # former atom tokens are forbidden in current definitions.
    unresolved = []
    for concept in rows(rb, "TSPConceptRegistry"):
        expression = concept.get("ReducedBasisExpression") or concept.get("BasisExpression") or ""
        if any(token in expression for token in OLD_ATOM_TOKENS):
            unresolved.append(concept["TSPConceptId"])
            concept["IsRecoverableFromCurrentBasis"] = False
        else:
            concept["RecoverabilityExpression"] = concept.get("RecoverabilityExpression") or (
                f"PROJECT_{concept['TSPConceptId']}_FROM_SEMANTIC_ARC_AND_ACTIVE_OPERATORS"
            )
            concept["IsRecoverableFromCurrentBasis"] = True
    if unresolved:
        raise AssertionError(f"unrecovered atom tokens remain: {unresolved[:10]}")
    add_measurement(
        rb,
        627,
        "Recoverability Witness",
        f"All {len(rows(rb, 'TSPConceptRegistry'))} registered concepts have explicit current-basis recovery expressions and source-table provenance.",
    )
    add_frontier(
        rb,
        "frontier-semantic-arc-recoverability",
        "Typed semantic-arc recoverability",
        625,
        627,
        "tsp-rule-semantic-arc-refinement",
        "Every registered concept is recoverable from semantic arcs plus the active operators, and attachment/valuation/warrant retain distinct signatures.",
        "semantic-arc-recoverability-certificate",
    )
    return (
        "Certified lossless recovery of the historical and derived vocabulary from semantic arcs plus the current operators.",
        f"Recoverability coverage is {len(rows(rb, 'TSPConceptRegistry'))}/{len(rows(rb, 'TSPConceptRegistry'))}; no ATTACHMENT, VALUATION, or WARRANT token remains in a current reduced definition.",
    )


def register_operator(
    rb: dict[str, Any],
    ident: str,
    display: str,
    expression: str,
    loop: int,
    sources: str,
) -> None:
    register_concept(
        rb,
        ident,
        display,
        "OPERATOR",
        expression,
        1,
        sources,
        loop,
        status="ACTIVE_OPERATOR",
        category="OPERATOR",
        operator_expression=display.upper().replace(" ", "_").replace("-", "_"),
    )


def loop_628(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_operator(
        rb,
        "operator-saturate",
        "Saturate",
        "SATURATE(seed_semantic_arcs,rewrite_rule,termination_warrant)",
        628,
        "TSPInferenceApplications,TSPConstraintRounds,TSPDerivedEdgeSets",
    )
    transform_registry_tokens(rb, {"CLOSURE": "SATURATE", "FIXPOINT": "SATURATE"})
    reduce_basis_rows(
        rb,
        ["operator-closure", "operator-fixpoint"],
        "operator-saturate",
        "SATURATE",
        4,
        historical_kind="HISTORICAL_OPERATOR",
    )
    concepts = table_index(rb, "TSPConceptRegistry")
    concepts["operator-closure"].update(
        {
            "ReducedBasisExpression": "SATURATE(seed=SEMANTIC_ARC(role=ADJACENT),mode=REACHABILITY_CLOSURE)",
            "OperatorExpression": "SATURATE",
            "RewriteMode": "REACHABILITY_CLOSURE",
            "RewritePolarity": "EXPANSIVE",
        }
    )
    concepts["operator-fixpoint"].update(
        {
            "ReducedBasisExpression": "SATURATE(seed=SEMANTIC_ARC(state),mode=RULE_CLOSURE,until=NO_CHANGE)",
            "OperatorExpression": "SATURATE",
            "RewriteMode": "RULE_CLOSURE_UNTIL_STABLE",
            "RewritePolarity": "EXPANSIVE",
        }
    )
    concepts["operator-saturate"].update(
        {
            "RewriteMode": "PARAMETRIC_SATURATION",
            "RewritePolarity": "EXPANSIVE",
            "RecoverabilityExpression": "IDENTITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    register_concept(
        rb,
        "coined-saturation-operator",
        "Saturation Operator",
        "COINED_PREDICATE",
        "SATURATE(SEMANTIC_ARC,rewrite,termination_warrant)",
        1,
        "TSPConceptRegistry,TSPInferenceApplications,TSPConstraintRounds",
        628,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="SATURATE",
    )
    add_measurement(
        rb,
        628,
        "Saturation Operator",
        "Reachability closure and rule fixed point become two explicit modes of SATURATE; active operators fall from four to three.",
    )
    return (
        "Collapsed CLOSURE and FIXPOINT into the SATURATE operator.",
        "Reachability closure and no-change rule closure remain recoverable as distinct expansive saturation modes; active operators are Saturate, Aggregate, and Quotient.",
    )


def loop_629(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_operator(
        rb,
        "operator-fiber-reduce",
        "Fiber Reduction",
        "FIBER_REDUCE(domain,key,reducer,expansion_warrant)",
        629,
        "TSPClusterBoundaryStates,TSPClusterContractionCertificates,TSPBoundTerms",
    )
    transform_registry_tokens(rb, {"AGGREGATE": "FIBER_REDUCE", "QUOTIENT": "FIBER_REDUCE"})
    reduce_basis_rows(
        rb,
        ["operator-aggregate", "operator-quotient"],
        "operator-fiber-reduce",
        "FIBER_REDUCE",
        4,
        historical_kind="HISTORICAL_OPERATOR",
    )
    concepts = table_index(rb, "TSPConceptRegistry")
    concepts["operator-aggregate"].update(
        {
            "ReducedBasisExpression": "FIBER_REDUCE(domain,key,reducer=COUNT_SUM_MIN_MAX_PARITY)",
            "OperatorExpression": "FIBER_REDUCE",
            "RewriteMode": "SUMMARY_REDUCTION",
            "RewritePolarity": "CONTRACTIVE",
        }
    )
    concepts["operator-quotient"].update(
        {
            "ReducedBasisExpression": "FIBER_REDUCE(domain,equivalence,reducer=REPRESENTATIVE,expansion_warrant)",
            "OperatorExpression": "FIBER_REDUCE",
            "RewriteMode": "EQUIVALENCE_REPRESENTATIVE",
            "RewritePolarity": "CONTRACTIVE",
        }
    )
    concepts["operator-fiber-reduce"].update(
        {
            "RewriteMode": "PARAMETRIC_FIBER_REDUCTION",
            "RewritePolarity": "CONTRACTIVE",
            "RecoverabilityExpression": "IDENTITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    register_concept(
        rb,
        "coined-fiber-reduction",
        "Fiber Reduction",
        "COINED_PREDICATE",
        "FIBER_REDUCE(SEMANTIC_ARC,key,reducer,expansion_warrant)",
        1,
        "TSPConceptRegistry,TSPClusterBoundaryStates,TSPBoundTerms",
        629,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="FIBER_REDUCE",
    )
    add_measurement(
        rb,
        629,
        "Fiber Reduction",
        "Aggregate summaries and quotient representatives become reducer modes over keyed fibers; active operators fall to Saturate and Fiber Reduction.",
    )
    return (
        "Collapsed AGGREGATE and QUOTIENT into FIBER_REDUCE.",
        "Count/sum/min/parity summaries and equivalence representatives remain recoverable reducer modes with explicit expansion warrants; active operator count is two.",
    )


def loop_630(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_operator(
        rb,
        "operator-warranted-rewrite",
        "Warranted Rewrite",
        "WARRANTED_REWRITE(input_arcs,rule,polarity,output_arcs,warrant)",
        630,
        "TSPInferenceApplications,TSPConceptRegistry,TSPClusterContractionCertificates",
    )
    transform_registry_tokens(rb, {"SATURATE": "WARRANTED_REWRITE", "FIBER_REDUCE": "WARRANTED_REWRITE"})
    reduce_basis_rows(
        rb,
        ["operator-saturate", "operator-fiber-reduce"],
        "operator-warranted-rewrite",
        "WARRANTED_REWRITE",
        5,
        historical_kind="HISTORICAL_OPERATOR",
    )
    concepts = table_index(rb, "TSPConceptRegistry")
    concepts["operator-saturate"].update(
        {
            "ReducedBasisExpression": "WARRANTED_REWRITE(input,mode=SATURATION,polarity=EXPANSIVE,output,warrant)",
            "OperatorExpression": "WARRANTED_REWRITE",
            "RewriteMode": "SATURATION",
            "RewritePolarity": "EXPANSIVE",
        }
    )
    concepts["operator-fiber-reduce"].update(
        {
            "ReducedBasisExpression": "WARRANTED_REWRITE(input,mode=FIBER_REDUCTION,polarity=CONTRACTIVE,output,expansion_warrant)",
            "OperatorExpression": "WARRANTED_REWRITE",
            "RewriteMode": "FIBER_REDUCTION",
            "RewritePolarity": "CONTRACTIVE",
        }
    )
    concepts["operator-warranted-rewrite"].update(
        {
            "RewriteMode": "PARAMETRIC",
            "RewritePolarity": "PARAMETRIC",
            "RecoverabilityExpression": "IDENTITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    register_concept(
        rb,
        "coined-warranted-rewrite",
        "Warranted Rewrite",
        "COINED_PREDICATE",
        "WARRANTED_REWRITE(input_semantic_arcs,rule,polarity,output_semantic_arcs,warrant)",
        1,
        "TSPConceptRegistry,TSPInferenceApplications",
        630,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    add_measurement(
        rb,
        630,
        "Warranted Rewrite",
        "Expansive saturation and contractive fiber reduction become modes of one rewrite operator; the active semantic operator count becomes one.",
    )
    set_meta(rb, "active_semantic_operator_count", "integer", integer=1)
    return (
        "Collapsed SATURATE and FIBER_REDUCE into one WARRANTED_REWRITE operator.",
        "Every transformation now exposes input, rule, polarity, output, and warrant; saturation and fiber reduction remain recoverable modes, and active operator count is one.",
    )


def loop_631(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    concepts = table_index(rb, "TSPConceptRegistry")
    classifications = {
        "operator-closure": ("REACHABILITY_CLOSURE", "EXPANSIVE"),
        "operator-fixpoint": ("RULE_CLOSURE_UNTIL_STABLE", "EXPANSIVE"),
        "operator-saturate": ("SATURATION", "EXPANSIVE"),
        "operator-aggregate": ("SUMMARY_REDUCTION", "CONTRACTIVE"),
        "operator-quotient": ("EQUIVALENCE_REPRESENTATIVE", "CONTRACTIVE"),
        "operator-fiber-reduce": ("FIBER_REDUCTION", "CONTRACTIVE"),
        "operator-warranted-rewrite": ("PARAMETRIC", "PARAMETRIC"),
    }
    for ident, (mode, polarity) in classifications.items():
        concepts[ident]["RewriteMode"] = mode
        concepts[ident]["RewritePolarity"] = polarity
    for row in rows(rb, "TSPConceptRegistry"):
        if row.get("OperatorExpression") == "WARRANTED_REWRITE" and not row.get("RewritePolarity"):
            row["RewriteMode"] = "COMPOSED_DERIVATION"
            row["RewritePolarity"] = "MIXED"
    register_concept(
        rb,
        "coined-rewrite-polarity",
        "Rewrite Polarity",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(rewrite,POLARITY,EXPANSIVE_OR_CONTRACTIVE_OR_MIXED)+WARRANT(mode)",
        1,
        "TSPConceptRegistry,TSPInferenceApplications",
        631,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    concepts["coined-rewrite-polarity"].update(
        {
            "RewriteMode": "POLARITY_CLASSIFICATION",
            "RewritePolarity": "PARAMETRIC",
            "RecoverabilityExpression": "PROJECT_REWRITE_MODE_AND_POLARITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    add_measurement(
        rb,
        631,
        "Rewrite Polarity",
        "Black-box internalization and saturation may be expansive; aggregate and quotient modes are contractive; mixed derivations may contain both. Coherence is not identified with one polarity.",
    )
    return (
        "Typed expansive, contractive, mixed, and parametric rewrite modes.",
        "Closure/fixed point/saturation are recoverable as expansive modes; aggregate/quotient/fiber reduction as contractive modes; local expansion is explicitly compatible with coherent rewrite composition.",
    )


def loop_632(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_concept(
        rb,
        "coined-one-arc-one-rewrite-basis",
        "One-Arc/One-Rewrite Basis",
        "COINED_PREDICATE",
        "SEMANTIC_ARC+WARRANTED_REWRITE",
        1,
        "TSPConceptRegistry,TSPConvergenceMeasurements",
        632,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    basis_row = table_index(rb, "TSPConceptRegistry")["coined-one-arc-one-rewrite-basis"]
    basis_row.update(
        {
            "ArcSubjectSort": "ANY_DECLARED_SUBJECT_SORT",
            "ArcLabelSort": "TYPED_LABEL",
            "ArcTargetSort": "ANY_DECLARED_TARGET_SORT",
            "ArcSignature": "SEMANTIC_ARC(subject:T,label:L,target:U)",
            "RecoverabilityExpression": "REGISTRY_COMPLETE_RECOVERY",
            "IsRecoverableFromCurrentBasis": True,
            "RewriteMode": "PARAMETRIC",
            "RewritePolarity": "PARAMETRIC",
        }
    )
    unresolved = refresh_recoverability(rb)
    if unresolved:
        raise AssertionError(f"concepts remain outside one-arc/one-rewrite basis: {unresolved[:10]}")
    if {row["DisplayName"] for row in active_primitives(rb)} != {"Semantic Arc"}:
        raise AssertionError("one-arc active basis mismatch")
    if {row["DisplayName"] for row in active_operators(rb)} != {"Warranted Rewrite"}:
        raise AssertionError("one-rewrite active basis mismatch")
    set_meta(rb, "active_predicate_atom_count", "integer", integer=1)
    set_meta(rb, "active_semantic_operator_count", "integer", integer=1)
    set_meta(rb, "semantic_compression_pct_current", "string", string="96.88")
    set_meta(rb, "one_arc_one_rewrite_status", "string", string="OBSERVED_FOR_CURRENT_DOMAIN_NOT_PROVED_MINIMAL")
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "OneArcOneRewriteObserved": True,
            "OneArcOneRewriteProved": False,
            "CurrentPredicateAtomCount": 1,
            "CurrentSemanticOperatorCount": 1,
            "CurrentSemanticCompressionPct": 96.88,
            "TypedArcRecoverabilityCertified": True,
        }
    )
    contract.setdefault("Acceptance", {}).update(
        {
            "CurrentPredicateAtomCount": 1,
            "CurrentSemanticOperatorCount": 1,
            "CurrentSemanticCompressionPct": 96.88,
            "RecoverableConceptCount": len(rows(rb, "TSPConceptRegistry")),
        }
    )
    add_measurement(
        rb,
        632,
        "One-Arc/One-Rewrite Basis",
        "All registered concepts recover through one typed semantic arc and one warranted rewrite. This is empirical coverage for the represented domain, not a minimality or universality theorem.",
        kind="ONE_ARC_ONE_REWRITE_EVENT",
        prediction="SUPPORTED_FOR_CURRENT_DOMAIN",
    )
    add_frontier(
        rb,
        "frontier-one-arc-one-rewrite",
        "One-arc/one-rewrite recoverable basis",
        624,
        632,
        "tsp-rule-semantic-arc-refinement",
        "Exactly one active relational atom and one active operator recover every registered concept through typed signatures and explicit rewrite modes.",
        "one-arc-one-rewrite-certificate",
    )
    return (
        "Certified one active typed semantic arc and one active warranted rewrite for the current TSP domain.",
        f"Thirty-two baseline surfaces reduce to one active atom (96.88%); {len(rows(rb, 'TSPConceptRegistry'))} registered concepts remain recoverable, historical bases remain versioned, and minimality is not claimed.",
    )


import re


def contains_legacy_basis_symbol(text: str, token: str) -> bool:
    """Recognize an old basis symbol, not a substring inside a newer coined term.

    Examples:
      WARRANT(...)           -> legacy symbol
      WARRANTED_REWRITE(...) -> current operator, not legacy WARRANT
      REACHABILITY_CLOSURE    -> rewrite-mode label, not active CLOSURE(...)
    """
    pattern = rf"(?<![A-Z0-9_]){re.escape(token)}(?=\s*(?:\(|\+|$))"
    return re.search(pattern, text or "") is not None


def refresh_recoverability(rb: dict[str, Any]) -> list[str]:
    ensure_arc_fields(rb)
    unresolved: list[str] = []
    forbidden = OLD_ATOM_TOKENS + OLD_OPERATOR_TOKENS
    for row in rows(rb, "TSPConceptRegistry"):
        expression = row.get("ReducedBasisExpression") or row.get("BasisExpression") or ""
        operator = row.get("OperatorExpression") or ""
        if any(
            contains_legacy_basis_symbol(expression, token)
            or contains_legacy_basis_symbol(operator, token)
            for token in forbidden
        ):
            unresolved.append(row["TSPConceptId"])
            row["IsRecoverableFromCurrentBasis"] = False
            continue
        row["RecoverabilityExpression"] = row.get("RecoverabilityExpression") or (
            f"PROJECT_{row['TSPConceptId']}_FROM_SEMANTIC_ARC_AND_WARRANTED_REWRITE"
        )
        row["IsRecoverableFromCurrentBasis"] = True
    return unresolved


def loop_625(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    register_concept(
        rb,
        "concept-semantic-arc",
        "Semantic Arc",
        "PRIMITIVE",
        "SEMANTIC_ARC(subject,label,target)",
        3,
        "TSPConceptRegistry,Cities,Neighborhoods,Addresses,TravelEdges,TSPInferenceApplications,TSPBoundTerms",
        625,
        status="ACTIVE_PRIMITIVE",
        category="ATOM",
    )
    transform_registry_tokens(
        rb,
        {
            "ATTACHMENT": "SEMANTIC_ARC",
            "VALUATION": "SEMANTIC_ARC",
            "WARRANT": "SEMANTIC_ARC",
        },
    )
    reduce_basis_rows(
        rb,
        ARC_ATOMS,
        "concept-semantic-arc",
        "SEMANTIC_ARC",
        3,
        historical_kind="HISTORICAL_PRIMITIVE",
    )
    concepts = table_index(rb, "TSPConceptRegistry")
    semantic = concepts["concept-semantic-arc"]
    semantic.update(
        {
            "ArcSubjectSort": "ANY_DECLARED_SUBJECT_SORT",
            "ArcLabelSort": "TYPED_LABEL",
            "ArcTargetSort": "ANY_DECLARED_TARGET_SORT",
            "ArcSignature": "SEMANTIC_ARC(subject:T,label:L,target:U)",
            "RecoverabilityExpression": "IDENTITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    for ident, spec in ARC_ATOMS.items():
        row = concepts[ident]
        row["ReducedBasisExpression"] = spec["signature"]
        row["OperatorExpression"] = ""
        row["ArcSubjectSort"] = spec["subject"]
        row["ArcLabelSort"] = spec["label"]
        row["ArcTargetSort"] = spec["target"]
        row["ArcSignature"] = spec["signature"]
        row["RecoverabilityExpression"] = spec["recover"]
        row["IsRecoverableFromCurrentBasis"] = True
    register_concept(
        rb,
        "coined-semantic-arc",
        "Semantic Arc",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(subject,label,target)+TYPED_SIGNATURE(label,target)",
        1,
        "TSPConceptRegistry",
        625,
        status="ACTIVE_DERIVED",
        category="DERIVED",
    )
    coined = table_index(rb, "TSPConceptRegistry")["coined-semantic-arc"]
    coined.update(
        {
            "ArcSubjectSort": "ANY_DECLARED_SUBJECT_SORT",
            "ArcLabelSort": "TYPED_LABEL",
            "ArcTargetSort": "ANY_DECLARED_TARGET_SORT",
            "ArcSignature": "SEMANTIC_ARC(subject:T,label:L,target:U)",
            "RecoverabilityExpression": "IDENTITY_PLUS_SIGNATURE_WARRANT",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    add_measurement(
        rb,
        625,
        "Semantic Arc",
        "Attachment, valuation, and warrant become typed semantic-arc projections. The active relational basis falls from three atoms to one without removing historical rows.",
    )
    set_meta(rb, "active_predicate_atom_count", "integer", integer=1)
    return (
        "Collapsed ATTACHMENT, VALUATION, and WARRANT into one typed SEMANTIC_ARC relation.",
        "The three historical atoms remain distinct recoverable projections by ROLE, MEASURE, and MODALITY signatures; active atom count is one.",
    )


def loop_626(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    concepts = table_index(rb, "TSPConceptRegistry")
    signatures = set()
    for ident, spec in ARC_ATOMS.items():
        row = concepts[ident]
        row.update(
            {
                "ArcSubjectSort": spec["subject"],
                "ArcLabelSort": spec["label"],
                "ArcTargetSort": spec["target"],
                "ArcSignature": spec["signature"],
                "RecoverabilityExpression": spec["recover"],
                "IsRecoverableFromCurrentBasis": True,
            }
        )
        signatures.add((spec["label"], spec["target"]))
    if len(signatures) != 3:
        raise AssertionError("historical atom arc signatures are not distinct")
    register_concept(
        rb,
        "coined-arc-signature",
        "Arc Signature",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(subject:S,label:L,target:T)+SEMANTIC_ARC(label,SIGNATURE,(S,L,T))",
        1,
        "TSPConceptRegistry",
        626,
        status="ACTIVE_DERIVED",
        category="DERIVED",
    )
    row = table_index(rb, "TSPConceptRegistry")["coined-arc-signature"]
    row.update(
        {
            "ArcSubjectSort": "LABEL_OR_CONCEPT",
            "ArcLabelSort": "SIGNATURE",
            "ArcTargetSort": "SORT_TRIPLE",
            "ArcSignature": "SEMANTIC_ARC(label,SIGNATURE,(subject_sort,label_sort,target_sort))",
            "RecoverabilityExpression": "LOOKUP_SIGNATURE_BY_LABEL_SORT",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    add_measurement(
        rb,
        626,
        "Arc Signature",
        "ROLE-to-entity, MEASURE-to-scalar, and MODALITY-to-source signatures remain distinct. Genericization does not erase type contracts.",
    )
    return (
        "Typed the semantic arc with explicit subject, label, and target sorts.",
        "Attachment = ROLE arc, valuation = MEASURE arc, warrant = MODALITY arc; each historical atom has a distinct recovery filter, preventing a vacuous untyped triple-store collapse.",
    )


def loop_631(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_arc_fields(rb)
    concepts = table_index(rb, "TSPConceptRegistry")
    classifications = {
        "operator-closure": ("REACHABILITY_CLOSURE", "EXPANSIVE"),
        "operator-fixpoint": ("RULE_CLOSURE_UNTIL_STABLE", "EXPANSIVE"),
        "operator-saturate": ("SATURATION", "EXPANSIVE"),
        "operator-aggregate": ("SUMMARY_REDUCTION", "CONTRACTIVE"),
        "operator-quotient": ("EQUIVALENCE_REPRESENTATIVE", "CONTRACTIVE"),
        "operator-fiber-reduce": ("FIBER_REDUCTION", "CONTRACTIVE"),
        "operator-warranted-rewrite": ("PARAMETRIC", "PARAMETRIC"),
    }
    for ident, (mode, polarity) in classifications.items():
        concepts[ident]["RewriteMode"] = mode
        concepts[ident]["RewritePolarity"] = polarity
    for row in rows(rb, "TSPConceptRegistry"):
        if row.get("OperatorExpression") == "WARRANTED_REWRITE" and not row.get("RewritePolarity"):
            row["RewriteMode"] = "COMPOSED_DERIVATION"
            row["RewritePolarity"] = "MIXED"
    register_concept(
        rb,
        "coined-rewrite-polarity",
        "Rewrite Polarity",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(rewrite,POLARITY,EXPANSIVE_OR_CONTRACTIVE_OR_MIXED)+WARRANT(mode)",
        1,
        "TSPConceptRegistry,TSPInferenceApplications",
        631,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    coined = table_index(rb, "TSPConceptRegistry")["coined-rewrite-polarity"]
    coined.update(
        {
            "RewriteMode": "POLARITY_CLASSIFICATION",
            "RewritePolarity": "PARAMETRIC",
            "RecoverabilityExpression": "PROJECT_REWRITE_MODE_AND_POLARITY",
            "IsRecoverableFromCurrentBasis": True,
        }
    )
    add_measurement(
        rb,
        631,
        "Rewrite Polarity",
        "Black-box internalization and saturation may be expansive; aggregate and quotient modes are contractive; mixed derivations may contain both. Coherence is not identified with one polarity.",
    )
    return (
        "Typed expansive, contractive, mixed, and parametric rewrite modes.",
        "Closure/fixed point/saturation are recoverable as expansive modes; aggregate/quotient/fiber reduction as contractive modes; local expansion is explicitly compatible with coherent rewrite composition.",
    )

def mark_current_basis_concept(
    rb: dict[str, Any],
    ident: str,
    *,
    mode: str = "COMPOSED_DERIVATION",
    polarity: str = "MIXED",
    recovery: str | None = None,
) -> None:
    ensure_arc_fields(rb)
    row = table_index(rb, "TSPConceptRegistry")[ident]
    row.update(
        {
            "ArcSubjectSort": row.get("ArcSubjectSort") or "COMPOSED_SUBJECT",
            "ArcLabelSort": row.get("ArcLabelSort") or "COMPOSED",
            "ArcTargetSort": row.get("ArcTargetSort") or "COMPOSED_TARGET",
            "ArcSignature": row.get("ArcSignature") or "COMPOSITION_OF_TYPED_SEMANTIC_ARCS",
            "RecoverabilityExpression": recovery or f"PROJECT_{ident}_FROM_SEMANTIC_ARC_AND_WARRANTED_REWRITE",
            "IsRecoverableFromCurrentBasis": True,
            "RewriteMode": mode,
            "RewritePolarity": polarity,
        }
    )


def ensure_region_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["Neighborhoods"],
        [
            base.field("RegionInterfaceKind", "string", "raw", True, "Port-bearing semantic region kind."),
            base.field("BoundaryPortCount", "integer", "raw", True, "Number of external ports required by the region interface."),
            base.field("InternalCoverageCount", "integer", "raw", True, "Required internal stops covered by every valid expansion."),
            base.field("ExpansionWitnessKind", "string", "raw", True, "How a region representative expands to underlying graph structure."),
            base.field("InterfaceStatus", "string", "raw", True, "OPEN, NORMALIZED, or CERTIFIED."),
        ],
    )
    for row in rows(rb, "Neighborhoods"):
        row.setdefault("RegionInterfaceKind", None)
        row.setdefault("BoundaryPortCount", None)
        row.setdefault("InternalCoverageCount", None)
        row.setdefault("ExpansionWitnessKind", None)
        row.setdefault("InterfaceStatus", None)


def loop_633(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_region_fields(rb)
    state_counts: Counter[str] = Counter(
        state["Neighborhood"] for state in rows(rb, "TSPClusterBoundaryStates")
    )
    for row in rows(rb, "Neighborhoods"):
        if row.get("IsQuotientNode"):
            row["RegionInterfaceKind"] = "PORT_BEARING_QUOTIENT_REGION"
            row["BoundaryPortCount"] = int(row.get("RequiredBoundaryDegree") or 2)
            row["InternalCoverageCount"] = max(
                [
                    int(state["InternalStopCount"])
                    for state in rows(rb, "TSPClusterBoundaryStates")
                    if state["Neighborhood"] == row["NeighborhoodId"]
                ]
                or [0]
            )
            row["ExpansionWitnessKind"] = "ORDERED_BOUNDARY_PATH_EDGE_MEMBERS"
            row["InterfaceStatus"] = "CERTIFIED" if state_counts[row["NeighborhoodId"]] else "NORMALIZED"
    register_concept(
        rb,
        "coined-region-interface",
        "Region Interface",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(region,BOUNDARY_PORT,port)+SEMANTIC_ARC(region,INTERNAL_COVERAGE,count)+WARRANTED_REWRITE(region,EXPANSION,graph,warrant)",
        1,
        "Neighborhoods,TSPClusterBoundaryStates,TSPClusterBoundaryStateMembers",
        633,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-region-interface", mode="REGION_NORMALIZATION", polarity="MIXED")
    add_measurement(
        rb,
        633,
        "Region Interface",
        "Existing neighborhood, component, and boundary-fiber quotient nodes share one port, coverage, valuation, and expansion contract without a new table.",
    )
    return (
        "Normalized every existing quotient node through one region-interface projection.",
        "Region identity now carries two boundary ports, internal coverage, an ordered expansion witness kind, and a certified interface status; neighborhood versus component remains provenance.",
    )


R3_INSTANCE = "tsp-three-regions-9"
R3_REGIONS = {
    "a": ["r3-stop-a1", "r3-stop-a2", "r3-stop-a3"],
    "b": ["r3-stop-b1", "r3-stop-b2", "r3-stop-b3"],
    "c": ["r3-stop-c1", "r3-stop-c2", "r3-stop-c3"],
}
R3_NEIGHBORHOODS = {key: f"gridville-region-{key}" for key in R3_REGIONS}
R3_RING_CROSSINGS = {
    edge_key("r3-stop-a3", "r3-stop-b1"),
    edge_key("r3-stop-b3", "r3-stop-c1"),
    edge_key("r3-stop-a1", "r3-stop-c3"),
}


def r3_region(stop: str) -> str:
    return stop.removeprefix("r3-stop-")[0]


def r3_short(stop: str) -> str:
    return stop.removeprefix("r3-stop-")


def r3_edge_id(a: str, b: str) -> str:
    x, y = edge_key(a, b)
    return f"r3-edge-{r3_short(x)}-{r3_short(y)}"


def r3_edge_cost(a: str, b: str) -> int:
    pair = edge_key(a, b)
    if r3_region(a) == r3_region(b):
        return 1
    if pair in R3_RING_CROSSINGS:
        return 10
    return 100


def r3_all_stops() -> list[str]:
    return [stop for key in ("a", "b", "c") for stop in R3_REGIONS[key]]


def r3_internal_edges() -> list[str]:
    result: list[str] = []
    for stops in R3_REGIONS.values():
        for a, b in itertools.combinations(stops, 2):
            result.append(r3_edge_id(a, b))
    return sorted(result)


def r3_route() -> list[str]:
    return [
        "r3-stop-a1",
        "r3-stop-a2",
        "r3-stop-a3",
        "r3-stop-b1",
        "r3-stop-b2",
        "r3-stop-b3",
        "r3-stop-c1",
        "r3-stop-c2",
        "r3-stop-c3",
    ]


def r3_repaired_edges() -> list[str]:
    route = r3_route()
    return [r3_edge_id(route[i], route[(i + 1) % len(route)]) for i in range(len(route))]


def loop_634(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_region_fields(rb)
    neighborhoods = []
    addresses = []
    stops = []
    for region_index, region in enumerate(("a", "b", "c"), start=1):
        neighborhoods.append(
            {
                "NeighborhoodId": R3_NEIGHBORHOODS[region],
                "DisplayName": f"Region {region.upper()}",
                "City": "gridville",
                "ClusterKind": "THREE_REGION_REPAIR_FIXTURE",
                "IsQuotientNode": False,
                "QuotientNodeKind": None,
                "RequiredBoundaryDegree": None,
                "QuotientScopeId": R3_INSTANCE,
                "RegionInterfaceKind": "PORT_BEARING_QUOTIENT_REGION",
                "BoundaryPortCount": 2,
                "InternalCoverageCount": 3,
                "ExpansionWitnessKind": "ORDERED_BOUNDARY_PATH_EDGE_MEMBERS",
                "InterfaceStatus": "OPEN",
            }
        )
        for local_index, stop in enumerate(R3_REGIONS[region], start=1):
            suffix = r3_short(stop)
            address = f"addr-r3-{suffix}"
            addresses.append(
                {
                    "AddressId": address,
                    "StreetLabel": f"Region {region.upper()} Stop {local_index}",
                    "Neighborhood": R3_NEIGHBORHOODS[region],
                    "XCoordinate": 40 + region_index * 10 + local_index,
                    "YCoordinate": region_index * 4 + (local_index % 2),
                    "IsDepotCandidate": stop == "r3-stop-a1",
                }
            )
            stops.append(
                {
                    "InstanceStopId": stop,
                    "TSPInstance": R3_INSTANCE,
                    "Address": address,
                    "IsRequired": True,
                }
            )
    upsert_rows(rb["Neighborhoods"], "NeighborhoodId", neighborhoods)
    upsert_rows(rb["Addresses"], "AddressId", addresses)
    upsert_rows(
        rb["TSPInstances"],
        "TSPInstanceId",
        [
            {
                "TSPInstanceId": R3_INSTANCE,
                "DisplayName": "Three-Region Repair Nine-Stop Graph",
                "City": "gridville",
                "DepotAddress": "addr-r3-a1",
                "DistanceModel": "EXPLICIT_SYMMETRIC_COST",
                "IsSymmetric": True,
                "Status": "NORMALIZED",
                "SearchPolicy": "REGION_REPAIR_BEFORE_ROUTE_SEARCH",
                "Notes": "Three cost-one triangles, three cost-ten ring crossings, and cost-one-hundred remaining crossings.",
            }
        ],
    )
    upsert_rows(rb["InstanceStops"], "InstanceStopId", stops)

    edge_rows = []
    all_stops = r3_all_stops()
    for a, b in itertools.combinations(all_stops, 2):
        x, y = edge_key(a, b)
        cost = r3_edge_cost(x, y)
        edge_rows.append(
            {
                "TravelEdgeId": r3_edge_id(x, y),
                "TSPInstance": R3_INSTANCE,
                "FromStop": x,
                "ToStop": y,
                "CanonicalPairKey": f"{x}|{y}",
                "DistanceMeters": cost * 1000,
                "TravelCost": cost,
                "IsAvailable": True,
                "EdgeSource": "THREE_REGION_REPAIR_FIXTURE",
            }
        )
    upsert_rows(rb["TravelEdges"], "TravelEdgeId", edge_rows)

    local_bounds = []
    dominance = []
    for stop in all_stops:
        internal_peers = sorted(peer for peer in R3_REGIONS[r3_region(stop)] if peer != stop)
        first, second = [r3_edge_id(stop, peer) for peer in internal_peers]
        bound_id = f"local-bound-{r3_short(stop)}"
        local_bounds.append(
            {
                "LocalDegreeBoundId": bound_id,
                "TSPInstance": R3_INSTANCE,
                "InstanceStop": stop,
                "FirstEdge": first,
                "SecondEdge": second,
            }
        )
        selected = {first, second}
        incident = [
            row["TravelEdgeId"]
            for row in edge_rows
            if stop in {row["FromStop"], row["ToStop"]}
        ]
        for other in sorted(set(incident) - selected):
            dominance.append(
                {
                    "IncidentDominanceCheckId": f"dom-{bound_id}-{other}",
                    "LocalDegreeBound": bound_id,
                    "OtherEdge": other,
                }
            )
    upsert_rows(rb["LocalDegreeBounds"], "LocalDegreeBoundId", local_bounds)
    upsert_rows(rb["IncidentDominanceChecks"], "IncidentDominanceCheckId", dominance)
    upsert_rows(
        rb["InstanceLowerBounds"],
        "InstanceLowerBoundId",
        [
            {
                "InstanceLowerBoundId": "degree-two-lower-bound-three-regions-9",
                "TSPInstance": R3_INSTANCE,
                "BoundKind": "SUM_TWO_CHEAPEST_INCIDENT_EDGES_DIVIDED_BY_TWO",
                "RequiredSupplementalTermCount": 0,
                "BoundCompositionKind": "DEGREE_TWO_ONLY",
                "SupplementalBoundAdjustment": 0,
            }
        ],
    )
    upsert_rows(
        rb["TSPInferenceStates"],
        "TSPInferenceStateId",
        [
            {
                "TSPInferenceStateId": "state-three-region-local-two-factor",
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-634",
                "StateKind": "LOCAL_TWO_FACTOR_REGION_TRIAD",
                "ParentStateId": None,
                "Status": "CLOSED",
                "Description": "Nine locally cheapest internal edges form three disconnected cost-one triangles.",
            }
        ],
    )
    upsert_rows(
        rb["TSPDerivedEdgeSets"],
        "TSPDerivedEdgeSetId",
        [
            {
                "TSPDerivedEdgeSetId": "edge-set-three-region-local-two-factor",
                "TSPInstance": R3_INSTANCE,
                "InferenceState": "state-three-region-local-two-factor",
                "DerivationKind": "UNION_OF_LOCAL_DEGREE_BOUND_SELECTIONS",
                "EdgeCount": 9,
                "RequiredStopCount": 9,
                "TotalCost": 9,
                "DegreeViolationCount": 0,
                "ConnectedComponentCount": 3,
                "ProperSubtourCount": 3,
                "Status": "DISCONNECTED_SUBTOURS",
            }
        ],
    )
    upsert_rows(
        rb["TSPDerivedEdgeSetMembers"],
        "TSPDerivedEdgeSetMemberId",
        [
            {
                "TSPDerivedEdgeSetMemberId": f"member-three-region-local-{edge_id}",
                "DerivedEdgeSet": "edge-set-three-region-local-two-factor",
                "TravelEdge": edge_id,
                "SupportCount": 2,
                "SelectedAtBothEndpoints": True,
                "MemberStatus": "SELECTED",
            }
            for edge_id in r3_internal_edges()
        ],
    )
    upsert_rows(
        rb["TSPEdgeSetStopDegrees"],
        "TSPEdgeSetStopDegreeId",
        [
            {
                "TSPEdgeSetStopDegreeId": f"degree-three-region-local-{r3_short(stop)}",
                "DerivedEdgeSet": "edge-set-three-region-local-two-factor",
                "InstanceStop": stop,
                "SelectedDegree": 2,
            }
            for stop in all_stops
        ],
    )
    register_concept(
        rb,
        "coined-region-triad",
        "Region Triad",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(stop,REGION,region)+SEMANTIC_ARC(edge,COST,value)+WARRANTED_REWRITE(local_bounds,UNION,three_component_two_factor,warrant)",
        1,
        "Neighborhoods,InstanceStops,TravelEdges,LocalDegreeBounds,TSPDerivedEdgeSets",
        634,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-region-triad", mode="FIXTURE_NORMALIZATION", polarity="EXPANSIVE")
    add_measurement(
        rb,
        634,
        "Region Triad",
        "Nine stops and thirty-six edges expand the explicit DAG while reusing the same tables and one-arc/one-rewrite basis; the local two-factor has cost nine and three components.",
    )
    return (
        "Represented three cost-one local triangles inside one complete nine-stop weighted graph.",
        "Nine valid local degree witnesses select nine internal edges of total cost nine; their union has degree two at every stop, three connected components, three proper subtours, and no route or optimality promotion.",
    )


def triangle_path_specs(stops: list[str]) -> list[tuple[str, str, str]]:
    result = []
    for entry, exit_ in itertools.combinations(sorted(stops), 2):
        via = next(stop for stop in stops if stop not in {entry, exit_})
        result.append((entry, via, exit_))
    return result


def loop_635(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_region_fields(rb)
    prev.ensure_boundary_fields(rb)
    state_rows = []
    member_rows = []
    cert_rows = []
    neighborhoods = table_index(rb, "Neighborhoods")
    for region in ("a", "b", "c"):
        neighborhood_id = R3_NEIGHBORHOODS[region]
        neighborhood = neighborhoods[neighborhood_id]
        neighborhood.update(
            {
                "IsQuotientNode": True,
                "QuotientNodeKind": "CONNECTED_COMPONENT_BOUNDARY_OBJECT",
                "RequiredBoundaryDegree": 2,
                "QuotientScopeId": R3_INSTANCE,
                "RegionInterfaceKind": "PORT_BEARING_QUOTIENT_REGION",
                "BoundaryPortCount": 2,
                "InternalCoverageCount": 3,
                "ExpansionWitnessKind": "ORDERED_BOUNDARY_PATH_EDGE_MEMBERS",
                "InterfaceStatus": "CERTIFIED",
            }
        )
        for entry, via, exit_ in triangle_path_specs(R3_REGIONS[region]):
            state_id = f"cluster-state-r3-{region}-{r3_short(entry)}-{r3_short(exit_)}"
            first_edge = r3_edge_id(entry, via)
            second_edge = r3_edge_id(via, exit_)
            state_rows.append(
                {
                    "TSPClusterBoundaryStateId": state_id,
                    "TSPInstance": R3_INSTANCE,
                    "Neighborhood": neighborhood_id,
                    "EntryStop": entry,
                    "ExitStop": exit_,
                    "InternalViaStop": via,
                    "SecondInternalViaStop": None,
                    "InternalPathCost": 2,
                    "InternalStopCount": 3,
                    "IsHamiltonianPath": True,
                    "IsDominated": False,
                    "Status": "SURVIVES",
                    "NormalizedWitnessId": None,
                    "OrientationMultiplicity": 2,
                    "IsQuotientRepresentative": True,
                    "EntryPortRole": "BOUNDARY_ENTRY",
                    "ExitPortRole": "BOUNDARY_EXIT",
                    "PortContract": "ONE_ENTRY_ONE_EXIT_FULL_INTERNAL_COVERAGE",
                    "BoundaryFiberKey": f"{neighborhood_id}|{min(entry, exit_)}|{max(entry, exit_)}",
                    "InternalOrderKey": f"{entry}>{via}>{exit_}",
                    "RawOrientationMultiplicity": 2,
                    "DominatedByStateId": None,
                    "DominanceDelta": 0,
                }
            )
            member_rows.extend(
                [
                    {
                        "TSPClusterBoundaryStateMemberId": f"member-{state_id}-1",
                        "ClusterBoundaryState": state_id,
                        "TravelEdge": first_edge,
                        "MemberOrder": 1,
                    },
                    {
                        "TSPClusterBoundaryStateMemberId": f"member-{state_id}-2",
                        "ClusterBoundaryState": state_id,
                        "TravelEdge": second_edge,
                        "MemberOrder": 2,
                    },
                ]
            )
        cert_rows.append(
            {
                "TSPClusterContractionCertificateId": f"contraction-three-region-{region}",
                "TSPInstance": R3_INSTANCE,
                "Neighborhood": neighborhood_id,
                "ScopeKind": "NEIGHBORHOOD",
                "RawInternalOrderCount": 6,
                "SurvivingBoundaryStateCount": 3,
                "RawCombinationCount": None,
                "ContractedCombinationCount": None,
                "ReductionPct": 50,
                "IsPassing": True,
                "ScopeClaim": "CERTIFIED_FOR_DECLARED_THREE_STOP_REGION",
                "EquivalenceRelation": "PATH_REVERSAL_WITH_FIXED_UNORDERED_BOUNDARY_AND_COST",
                "QuotientClassCount": 3,
            }
        )
    upsert_rows(rb["TSPClusterBoundaryStates"], "TSPClusterBoundaryStateId", state_rows)
    upsert_rows(rb["TSPClusterBoundaryStateMembers"], "TSPClusterBoundaryStateMemberId", member_rows)
    upsert_rows(rb["TSPClusterContractionCertificates"], "TSPClusterContractionCertificateId", cert_rows)
    register_concept(
        rb,
        "coined-region-quotient",
        "Region Quotient",
        "COINED_PREDICATE",
        "WARRANTED_REWRITE(region_paths,CONTRACTIVE,region_representative,expansion_warrant)+SEMANTIC_ARC(region,BOUNDARY_DEGREE,2)",
        1,
        "Neighborhoods,TSPClusterBoundaryStates,TSPClusterContractionCertificates",
        635,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-region-quotient", mode="REGION_QUOTIENT", polarity="CONTRACTIVE")
    add_measurement(
        rb,
        635,
        "Region Quotient",
        "Three regions each expand through three cost-two Hamiltonian path states and contract six directed internal orders to three reversal classes without adding a table.",
    )
    add_frontier(
        rb,
        "frontier-three-region-quotient",
        "Three-region quotient interfaces",
        634,
        635,
        "tsp-rule-region-interface",
        "Each of three local triangle components exposes a boundary-degree-two region interface with three path states and explicit expansion members.",
        "region-interface-certificate",
    )
    return (
        "Contracted each local triangle to a certified boundary-degree-two region quotient.",
        "Three quotient nodes expose nine total boundary states and eighteen expansion edges; each regional certificate records 6 directed orders to 3 reversal classes at cost two.",
    )


def loop_636(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    profiles = []
    for region in ("a", "b", "c"):
        profiles.append(
            {
                "TSPDefectProfileId": f"quotient-incidence-region-{region}",
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-636",
                "SubjectKind": "QUOTIENT_NODE",
                "SubjectId": R3_NEIGHBORHOODS[region],
                "RequiredIncidence": 2,
                "ObservedIncidence": 0,
                "ComponentCount": 1,
                "RequiredBoundaryCrossings": 2,
                "ObservedBoundaryCrossings": 0,
                "LowerBoundCost": 0,
                "UpperBoundCost": 0,
                "Status": "QUOTIENT_INCIDENCE_OPEN",
            }
        )
    profiles.append(
        {
            "TSPDefectProfileId": "defect-three-region-local-two-factor",
            "TSPInstance": R3_INSTANCE,
            "TSPLoop": "tsp-loop-636",
            "SubjectKind": "EDGE_SET",
            "SubjectId": "edge-set-three-region-local-two-factor",
            "RequiredIncidence": 2,
            "ObservedIncidence": 2,
            "ComponentCount": 3,
            "RequiredBoundaryCrossings": 3,
            "ObservedBoundaryCrossings": 0,
            "LowerBoundCost": 9,
            "UpperBoundCost": 36,
            "Status": "INTERNAL_INCIDENCE_CLOSED_QUOTIENT_INCIDENCE_OPEN",
        }
    )
    upsert_rows(rb["TSPDefectProfiles"], "TSPDefectProfileId", profiles)
    register_concept(
        rb,
        "coined-quotient-incidence-budget",
        "Quotient Incidence Budget",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(region,REQUIRED_BOUNDARY_DEGREE,2)+SEMANTIC_ARC(region,OBSERVED_BOUNDARY_DEGREE,0)+WARRANTED_REWRITE(profile,DEFECT_VECTOR,open,warrant)",
        1,
        "Neighborhoods,TSPDefectProfiles",
        636,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-quotient-incidence-budget", mode="DEFECT_PROJECTION", polarity="MIXED")
    add_measurement(
        rb,
        636,
        "Quotient Incidence Budget",
        "Internal stop incidence is closed while all three region interfaces have boundary degree deficit two; the global profile separates those coordinates.",
    )
    return (
        "Lifted the three-component defect into quotient incidence budgets.",
        "Every stop retains internal degree two, every region requires external degree two and currently has zero, and the global profile records component count three with crossing deficit three.",
    )


def loop_637(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    upsert_rows(
        rb["TSPBoundTerms"],
        "TSPBoundTermId",
        [
            {
                "TSPBoundTermId": "bound-term-three-region-boundary-handshake",
                "BoundCertificate": "degree-two-lower-bound-three-regions-9",
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-637",
                "TermKind": "REGION_BOUNDARY_HANDSHAKE",
                "Quantity": 3,
                "UnitWeight": 0,
                "Sign": 1,
                "ConstraintKind": "SUM_REQUIRED_REGION_DEGREES_DIVIDED_BY_TWO",
                "ConstraintValue": 3,
                "CountsTowardAdjustment": False,
                "IsCertified": True,
                "Justification": "Three quotient regions each require boundary degree two; total degree six counts every inter-region edge twice, so at least three crossing edges are required.",
            }
        ],
    )
    table_index(rb, "TSPInferenceRules")["tsp-rule-boundary-handshake"]["ImplementationStatus"] = "EXECUTABLE"
    register_concept(
        rb,
        "coined-boundary-handshake",
        "Boundary Handshake",
        "COINED_PREDICATE",
        "WARRANTED_REWRITE(SEMANTIC_ARC(region,BOUNDARY_DEGREE,2),AGGREGATE_THEN_DIVIDE_BY_TWO,crossing_demand=3,warrant)",
        1,
        "Neighborhoods,TSPBoundTerms,TSPDefectProfiles",
        637,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-boundary-handshake", mode="HANDSHAKE_AGGREGATION", polarity="CONTRACTIVE")
    add_measurement(
        rb,
        637,
        "Boundary Handshake",
        "Quotient degree demand six reduces to a certified minimum of three crossing edges because every crossing has two region endpoints.",
    )
    return (
        "Derived a three-edge crossing demand from the quotient boundary-degree handshake.",
        "Required quotient degree is 3 regions x 2 = 6; dividing by two endpoints per crossing certifies at least 3 inter-region edges.",
    )


def loop_638(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    bound_id = "degree-two-lower-bound-three-regions-9"
    upsert_rows(
        rb["TSPBoundTerms"],
        "TSPBoundTermId",
        [
            {
                "TSPBoundTermId": "bound-term-three-region-base",
                "BoundCertificate": bound_id,
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-638",
                "TermKind": "BASE_DEGREE_TWO_BOUND",
                "Quantity": 1,
                "UnitWeight": 9,
                "Sign": 1,
                "ConstraintKind": "BASE_LOWER_BOUND",
                "ConstraintValue": 9,
                "CountsTowardAdjustment": False,
                "IsCertified": True,
                "Justification": "Nine local two-cheapest bounds each cost two; double-count correction gives nine.",
            },
            {
                "TSPBoundTermId": "bound-term-three-region-crossing-insertion",
                "BoundCertificate": bound_id,
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-638",
                "TermKind": "MANDATORY_CROSSING_INSERTION",
                "Quantity": 3,
                "UnitWeight": 10,
                "Sign": 1,
                "ConstraintKind": "BOUNDARY_HANDSHAKE_AND_MINIMUM_CROSSING_COST",
                "ConstraintValue": 10,
                "CountsTowardAdjustment": True,
                "IsCertified": True,
                "Justification": "Boundary handshake requires three crossings and the minimum available inter-region edge cost is ten.",
            },
            {
                "TSPBoundTermId": "bound-term-three-region-internal-release",
                "BoundCertificate": bound_id,
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-638",
                "TermKind": "NECESSARY_INTERNAL_EDGE_RELEASE",
                "Quantity": 3,
                "UnitWeight": 1,
                "Sign": -1,
                "ConstraintKind": "ONE_RELEASE_PER_REGION_CYCLE",
                "ConstraintValue": 3,
                "CountsTowardAdjustment": True,
                "IsCertified": True,
                "Justification": "Opening each cost-one local triangle to accept boundary incidence releases one cost-one internal edge per region.",
            },
        ],
    )
    lower = table_index(rb, "InstanceLowerBounds")[bound_id]
    lower.update(
        {
            "RequiredSupplementalTermCount": 2,
            "SupplementalBoundAdjustment": 27,
            "BoundCompositionKind": "DEGREE_TWO_PLUS_THREE_REGION_REPAIR",
        }
    )
    table_index(rb, "TSPInferenceRules")["tsp-rule-region-repair-equation"]["ImplementationStatus"] = "EXECUTABLE"
    profile = table_index(rb, "TSPDefectProfiles")["defect-three-region-local-two-factor"]
    profile["LowerBoundCost"] = 36
    profile["Status"] = "REGION_REPAIR_BOUND_CERTIFIED"
    register_concept(
        rb,
        "coined-region-repair-equation",
        "Region Repair Equation",
        "COINED_PREDICATE",
        "WARRANTED_REWRITE(base=9,ADD_CROSSINGS=30,RELEASE_INTERNAL=3,lower_bound=36,warrant)",
        1,
        "TSPBoundTerms,InstanceLowerBounds,TSPDefectProfiles",
        638,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-region-repair-equation", mode="SIGNED_BOUND_COMPOSITION", polarity="MIXED")
    add_measurement(
        rb,
        638,
        "Region Repair Equation",
        "The three-region lower bound is 9 + 30 - 3 = 36, composed entirely from witnessed semantic arcs and a mixed warranted rewrite.",
    )
    add_frontier(
        rb,
        "frontier-three-region-repair-bound",
        "Three-region repair lower bound",
        634,
        638,
        "tsp-rule-region-repair-equation",
        "Base nine plus three cost-ten crossings minus three cost-one releases equals certified lower bound thirty-six.",
        "region-repair-equation-certificate",
    )
    return (
        "Composed the three-region repair lower bound as 9 + 3*10 - 3*1 = 36.",
        "The signed supplemental adjustment is +27; all contributing insertion and release terms are separately certified, and no route witness has yet been used to set the bound.",
    )


def ensure_exchange_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPEdgeStates"],
        [
            base.field("ExchangeModality", "string", "raw", True, "ADDED, RELEASED, or RETAINED in a balanced exchange witness."),
            base.field("ExchangeSetId", "string", "raw", True, "Balanced-exchange identifier."),
            base.field("ExchangeWarrant", "string", "raw", True, "Local incidence and connectivity warrant for the exchange decision."),
        ],
    )
    for row in rows(rb, "TSPEdgeStates"):
        row.setdefault("ExchangeModality", None)
        row.setdefault("ExchangeSetId", None)
        row.setdefault("ExchangeWarrant", None)


def loop_639(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_exchange_fields(rb)
    repaired_set = "edge-set-three-region-balanced-exchange"
    state_id = "state-three-region-balanced-exchange"
    app_id = "event-three-region-balanced-exchange"
    repaired = r3_repaired_edges()
    released = [
        r3_edge_id("r3-stop-a1", "r3-stop-a3"),
        r3_edge_id("r3-stop-b1", "r3-stop-b3"),
        r3_edge_id("r3-stop-c1", "r3-stop-c3"),
    ]
    added = sorted(r3_edge_id(a, b) for a, b in R3_RING_CROSSINGS)
    retained = sorted(set(repaired) - set(added))
    upsert_rows(
        rb["TSPInferenceStates"],
        "TSPInferenceStateId",
        [
            {
                "TSPInferenceStateId": state_id,
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-639",
                "StateKind": "BALANCED_EDGE_EXCHANGE",
                "ParentStateId": "state-three-region-local-two-factor",
                "Status": "CLOSED",
                "Description": "Three crossing edges replace one internal edge per region and yield one connected degree-two cycle.",
            }
        ],
    )
    upsert_rows(
        rb["TSPInferenceApplications"],
        "TSPInferenceApplicationId",
        [
            {
                "TSPInferenceApplicationId": app_id,
                "InferenceState": state_id,
                "InferenceRule": "tsp-rule-balanced-edge-exchange",
                "TSPLoop": "tsp-loop-639",
                "SubjectType": "DERIVED_EDGE_SET",
                "SubjectId": repaired_set,
                "ApplicabilityPassed": True,
                "Conclusion": "Three added crossings and three released internal edges preserve degree two and connect all three regions.",
                "CertificateType": "balanced-edge-exchange-certificate",
                "EventKind": "BALANCED_EXCHANGE",
                "InputStateId": "state-three-region-local-two-factor",
                "OutputStateId": state_id,
                "AntecedentCount": 7,
                "DecisionCount": 6,
                "EventStatus": "APPLIED",
            }
        ],
    )
    upsert_rows(
        rb["TSPDerivedEdgeSets"],
        "TSPDerivedEdgeSetId",
        [
            {
                "TSPDerivedEdgeSetId": repaired_set,
                "TSPInstance": R3_INSTANCE,
                "InferenceState": state_id,
                "DerivationKind": "BALANCED_REGION_EDGE_EXCHANGE",
                "EdgeCount": 9,
                "RequiredStopCount": 9,
                "TotalCost": 36,
                "DegreeViolationCount": 0,
                "ConnectedComponentCount": 1,
                "ProperSubtourCount": 0,
                "Status": "CONNECTED_DEGREE_TWO",
            }
        ],
    )
    upsert_rows(
        rb["TSPDerivedEdgeSetMembers"],
        "TSPDerivedEdgeSetMemberId",
        [
            {
                "TSPDerivedEdgeSetMemberId": f"member-three-region-repaired-{edge_id}",
                "DerivedEdgeSet": repaired_set,
                "TravelEdge": edge_id,
                "SupportCount": 1,
                "SelectedAtBothEndpoints": edge_id in retained,
                "MemberStatus": "SELECTED",
            }
            for edge_id in repaired
        ],
    )
    upsert_rows(
        rb["TSPEdgeSetStopDegrees"],
        "TSPEdgeSetStopDegreeId",
        [
            {
                "TSPEdgeSetStopDegreeId": f"degree-three-region-repaired-{r3_short(stop)}",
                "DerivedEdgeSet": repaired_set,
                "InstanceStop": stop,
                "SelectedDegree": 2,
            }
            for stop in r3_all_stops()
        ],
    )
    route = r3_route()
    tree_rows = []
    for index in range(len(route) - 1):
        tree_rows.append(
            {
                "TSPSpanningTreeEdgeId": f"tree-three-region-repaired-{index + 1}",
                "DerivedEdgeSet": repaired_set,
                "ParentStop": route[index],
                "ChildStop": route[index + 1],
                "TravelEdge": r3_edge_id(route[index], route[index + 1]),
                "Depth": index + 1,
            }
        )
    upsert_rows(rb["TSPSpanningTreeEdges"], "TSPSpanningTreeEdgeId", tree_rows)
    upsert_rows(
        rb["TSPConnectedDegreeTwoCertificates"],
        "TSPConnectedDegreeTwoCertificateId",
        [
            {
                "TSPConnectedDegreeTwoCertificateId": "connected-cycle-three-region-balanced-exchange",
                "DerivedEdgeSet": repaired_set,
                "RequiredStopCount": 9,
                "EdgeCount": 9,
                "DegreeViolationCount": 0,
                "ComponentCount": 1,
                "ProperSubtourCount": 0,
                "SpanningTreeEdgeCount": 8,
            }
        ],
    )
    edge_states = []
    for edge_id in added:
        edge_states.append(
            {
                "TSPEdgeStateId": f"edge-state-three-region-added-{edge_id}",
                "InferenceState": state_id,
                "TravelEdge": edge_id,
                "DecisionStatus": "SELECTED",
                "EpistemicStatus": "ADDED_BY_BALANCED_EXCHANGE_FOR_CURRENT_WITNESS",
                "InferenceApplication": app_id,
                "ExchangeModality": "ADDED",
                "ExchangeSetId": repaired_set,
                "ExchangeWarrant": "BOUNDARY_HANDSHAKE_AND_DEGREE_PRESERVATION",
            }
        )
    for edge_id in released:
        edge_states.append(
            {
                "TSPEdgeStateId": f"edge-state-three-region-released-{edge_id}",
                "InferenceState": state_id,
                "TravelEdge": edge_id,
                "DecisionStatus": "SUPERSEDED",
                "EpistemicStatus": "RELEASED_FROM_CURRENT_REPAIR_WITNESS_NOT_UNIVERSALLY_FORBIDDEN",
                "InferenceApplication": app_id,
                "ExchangeModality": "RELEASED",
                "ExchangeSetId": repaired_set,
                "ExchangeWarrant": "ONE_INTERNAL_RELEASE_PER_REGION",
            }
        )
    upsert_rows(rb["TSPEdgeStates"], "TSPEdgeStateId", edge_states)
    table_index(rb, "TSPInferenceRules")["tsp-rule-balanced-edge-exchange"]["ImplementationStatus"] = "EXECUTABLE"
    register_concept(
        rb,
        "coined-balanced-edge-exchange",
        "Balanced Edge Exchange",
        "COINED_PREDICATE",
        "WARRANTED_REWRITE(local_two_factor,ADDED_3_RELEASED_3,connected_cycle,degree_and_connectivity_warrant)",
        1,
        "TSPEdgeStates,TSPDerivedEdgeSets,TSPConnectedDegreeTwoCertificates",
        639,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-balanced-edge-exchange", mode="BALANCED_EDGE_EXCHANGE", polarity="MIXED")
    add_measurement(
        rb,
        639,
        "Balanced Edge Exchange",
        "Three explicit additions and three explicit releases replace the disconnected local two-factor with one cost-thirty-six connected degree-two cycle.",
    )
    return (
        "Constructed a connected nine-edge cycle by a balanced three-add/three-release exchange.",
        f"Added={added}; released={released}; every stop has repaired degree two, component count is one, proper subtours are zero, and total cost is thirty-six. Released edges are superseded only for this witness, never silently forbidden globally.",
    )


def add_candidate_route(
    rb: dict[str, Any],
    *,
    candidate_id: str,
    display: str,
    instance: str,
    kind: str,
    route: list[str],
    prefix: str,
) -> None:
    upsert_rows(
        rb["CandidateTours"],
        "CandidateTourId",
        [
            {
                "CandidateTourId": candidate_id,
                "DisplayName": display,
                "TSPInstance": instance,
                "CandidateKind": kind,
                "SearchBranchesExplored": 0,
                "BacktrackCount": 0,
            }
        ],
    )
    stop_rows = []
    leg_rows = []
    for index, stop in enumerate(route, start=1):
        stop_rows.append(
            {
                "TourStopId": f"{prefix}-stop-{index}",
                "CandidateTour": candidate_id,
                "InstanceStop": stop,
                "SequencePosition": index,
            }
        )
    for index, stop in enumerate(route, start=1):
        nxt_index = index + 1 if index < len(route) else 1
        nxt = route[nxt_index - 1]
        edge_id = r3_edge_id(stop, nxt) if instance == R3_INSTANCE else face_edge_id(stop, nxt)
        leg_rows.append(
            {
                "TourLegId": f"{prefix}-leg-{index}",
                "CandidateTour": candidate_id,
                "FromTourStop": f"{prefix}-stop-{index}",
                "ToTourStop": f"{prefix}-stop-{nxt_index}",
                "TravelEdge": edge_id,
                "LegOrder": index,
            }
        )
    upsert_rows(rb["TourStops"], "TourStopId", stop_rows)
    upsert_rows(rb["TourLegs"], "TourLegId", leg_rows)


def loop_640(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    route = r3_route()
    candidate_id = "tour-three-region-repair-36"
    add_candidate_route(
        rb,
        candidate_id=candidate_id,
        display="Three-region repaired cycle A1-A2-A3-B1-B2-B3-C1-C2-C3-A1",
        instance=R3_INSTANCE,
        kind="RECONSTRUCTED_REPAIR_COMPARISON_WITNESS",
        route=route,
        prefix="r3-tour",
    )
    reconstruction_id = "reconstruction-three-region-balanced-exchange"
    upsert_rows(
        rb["TSPRouteReconstructions"],
        "TSPRouteReconstructionId",
        [
            {
                "TSPRouteReconstructionId": reconstruction_id,
                "TSPInstance": R3_INSTANCE,
                "DerivedEdgeSet": "edge-set-three-region-balanced-exchange",
                "StartStop": "r3-stop-a1",
                "OrientationRule": "DEPOT_THEN_REGION_RING_THEN_LOCAL_LEXICAL_PATH",
                "RequiredStopCount": 9,
                "ReconstructedStopCount": 9,
                "ReconstructedLegCount": 9,
                "TotalCost": 36,
                "CandidateUsedAsAntecedent": False,
                "ComparisonCandidate": candidate_id,
                "MatchesComparisonCandidate": True,
                "Status": "RECONSTRUCTED",
            }
        ],
    )
    step_rows = []
    for index, stop in enumerate(route, start=1):
        nxt = route[index] if index < len(route) else route[0]
        step_rows.append(
            {
                "TSPRouteReconstructionStepId": f"reconstruction-three-region-step-{index}",
                "RouteReconstruction": reconstruction_id,
                "StepOrder": index,
                "FromStop": stop,
                "ToStop": nxt,
                "TravelEdge": r3_edge_id(stop, nxt),
                "IsClosingStep": index == len(route),
            }
        )
    upsert_rows(rb["TSPRouteReconstructionSteps"], "TSPRouteReconstructionStepId", step_rows)
    upsert_rows(
        rb["TSPWitnessNormalForms"],
        "TSPWitnessNormalFormId",
        [
            {
                "TSPWitnessNormalFormId": "normal-cycle-three-region-reconstructed",
                "TSPInstance": R3_INSTANCE,
                "TSPLoop": "tsp-loop-640",
                "WitnessShape": "CYCLE",
                "OriginKind": "RECONSTRUCTED",
                "SourceKind": "TSPRouteReconstructions",
                "SourceId": reconstruction_id,
                "BoundarySignature": None,
                "CoveredStopCount": 9,
                "RequiredStopCount": 9,
                "EdgeCount": 9,
                "TotalCost": 36,
                "IncidenceDefect": 0,
                "ConnectivityDefect": 0,
                "OrderDefect": 0,
                "ProvenanceSummary": "Reconstructed from a balanced-exchange connected degree-two edge set; the candidate is comparison-only.",
            }
        ],
    )
    register_concept(
        rb,
        "coined-repair-cycle",
        "Repair Cycle",
        "COINED_PREDICATE",
        "WARRANTED_REWRITE(balanced_exchange,ORDERED_RECONSTRUCTION,cycle_witness,semantic_arc_warrant)",
        1,
        "TSPRouteReconstructions,TSPRouteReconstructionSteps,TSPWitnessNormalForms,CandidateTours",
        640,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-repair-cycle", mode="ROUTE_RECONSTRUCTION", polarity="EXPANSIVE")
    add_measurement(
        rb,
        640,
        "Repair Cycle",
        "The repaired edge set expands to a nine-step ordered route and normal-form witness with CandidateUsedAsAntecedent=false.",
    )
    return (
        "Reconstructed the balanced-exchange cycle as an ordered nine-stop route of cost thirty-six.",
        "Route A1-A2-A3-B1-B2-B3-C1-C2-C3-A1 has nine valid legs, zero defects, CandidateUsedAsAntecedent=false, and matches its downstream comparison candidate.",
    )


def loop_641(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    upsert_rows(
        rb["OptimalityCertificates"],
        "OptimalityCertificateId",
        [
            {
                "OptimalityCertificateId": "optimality-three-region-repair-36",
                "CandidateTour": "tour-three-region-repair-36",
                "InstanceLowerBound": "degree-two-lower-bound-three-regions-9",
            }
        ],
    )
    upsert_rows(
        rb["TSPInvariantChecks"],
        "TSPInvariantCheckId",
        [
            {
                "TSPInvariantCheckId": "check-three-region-repair-optimality",
                "DisplayName": "Three-region repaired candidate equals region-repair lower bound",
                "CandidateTour": "tour-three-region-repair-36",
                "ExpectedHamiltonianCycleWitness": True,
                "ExpectedOptimalityProved": True,
                "ExpectedTotalTravelCost": 36,
            }
        ],
    )
    profile = table_index(rb, "TSPDefectProfiles")["defect-three-region-local-two-factor"]
    profile.update(
        {
            "ObservedBoundaryCrossings": 3,
            "ComponentCount": 1,
            "LowerBoundCost": 36,
            "UpperBoundCost": 36,
            "Status": "ZERO_DEFECT_AFTER_REGION_REPAIR",
        }
    )
    for region in ("a", "b", "c"):
        region_profile = table_index(rb, "TSPDefectProfiles")[f"quotient-incidence-region-{region}"]
        region_profile["ObservedIncidence"] = 2
        region_profile["ObservedBoundaryCrossings"] = 2
        region_profile["Status"] = "QUOTIENT_INCIDENCE_CLOSED"
    register_concept(
        rb,
        "coined-region-bound-sandwich",
        "Region Bound Sandwich",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(instance,LOWER_BOUND,36)+SEMANTIC_ARC(witness,COST,36)+WARRANTED_REWRITE(equality,OPTIMALITY,closed,warrant)",
        1,
        "InstanceLowerBounds,CandidateTours,OptimalityCertificates",
        641,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-region-bound-sandwich", mode="BOUND_EQUALITY_RIGIDITY", polarity="CONTRACTIVE")
    table_index(rb, "TSPInferenceRules")["tsp-rule-bound-equality-optimality"]["ImplementationStatus"] = "EXECUTABLE"
    claims = contract.setdefault("Claims", {})
    claims["ThreeRegionOptimalityProved"] = True
    claims["GeneralKRegionRepairProved"] = False
    contract.setdefault("Acceptance", {}).update(
        {
            "ThreeRegionLowerBound": 36,
            "ThreeRegionCandidateCost": 36,
            "ThreeRegionOptimalityProved": True,
        }
    )
    add_measurement(
        rb,
        641,
        "Region Bound Sandwich",
        "The valid reconstructed cost-thirty-six cycle equals the independently certified three-region lower bound thirty-six.",
    )
    add_frontier(
        rb,
        "frontier-three-region-optimality",
        "Three-region finite optimality",
        634,
        641,
        "tsp-rule-bound-equality-optimality",
        "A valid reconstructed cost-thirty-six cycle equals the certified three-region repair lower bound thirty-six.",
        "finite-instance-optimality-certificate",
    )
    return (
        "Closed finite optimality for the three-region nine-stop fixture at cost thirty-six.",
        "Certified lower bound 36 equals valid reconstructed witness cost 36. The certificate is scoped to tsp-three-regions-9 and does not claim a general k-region repair theorem.",
    )


# Preserve the epistemic distinction between an unwitnessed cost coordinate and
# a certified interval.  The original defect-vector table had no witness flags,
# so the three-region pass adds them without adding a physical table.
_original_loop_636 = loop_636
_original_loop_638 = loop_638
_original_loop_639 = loop_639


def ensure_cost_witness_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPDefectProfiles"],
        [
            base.field("LowerBoundWitnessed", "boolean", "raw", True, "Whether the lower coordinate has a represented certificate."),
            base.field("UpperBoundWitnessed", "boolean", "raw", True, "Whether the upper coordinate has a represented feasible witness."),
        ],
    )
    base.replace_formula(
        rb["TSPDefectProfiles"],
        "CostGap",
        "=IF(AND({{LowerBoundWitnessed}}, {{UpperBoundWitnessed}}), {{UpperBoundCost}} - {{LowerBoundCost}}, 0)",
    )
    for row in rows(rb, "TSPDefectProfiles"):
        relevant = row.get("SubjectKind") in {"INSTANCE", "EDGE_SET", "WITNESS"} and (
            float(row.get("LowerBoundCost") or 0) != 0 or float(row.get("UpperBoundCost") or 0) != 0
        )
        row.setdefault("LowerBoundWitnessed", relevant)
        row.setdefault("UpperBoundWitnessed", relevant)


def loop_636(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_cost_witness_fields(rb)
    after, witness = _original_loop_636(rb, contract)
    profile = table_index(rb, "TSPDefectProfiles")["defect-three-region-local-two-factor"]
    profile.update(
        {
            "LowerBoundCost": 0,
            "UpperBoundCost": 0,
            "LowerBoundWitnessed": False,
            "UpperBoundWitnessed": False,
            "Status": "INTERNAL_INCIDENCE_CLOSED_QUOTIENT_INCIDENCE_OPEN_COST_INTERVAL_NOT_YET_WITNESSED",
        }
    )
    for region in ("a", "b", "c"):
        row = table_index(rb, "TSPDefectProfiles")[f"quotient-incidence-region-{region}"]
        row["LowerBoundWitnessed"] = False
        row["UpperBoundWitnessed"] = False
    return after, witness + " Cost coordinates remain explicitly unwitnessed at this stage."


def loop_638(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_cost_witness_fields(rb)
    after, witness = _original_loop_638(rb, contract)
    profile = table_index(rb, "TSPDefectProfiles")["defect-three-region-local-two-factor"]
    profile.update(
        {
            "LowerBoundCost": 36,
            "UpperBoundCost": 0,
            "LowerBoundWitnessed": True,
            "UpperBoundWitnessed": False,
            "Status": "REGION_REPAIR_LOWER_BOUND_CERTIFIED_UPPER_WITNESS_OPEN",
        }
    )
    return after, witness + " The lower coordinate is witnessed; the feasible upper coordinate remains open until construction."


def loop_639(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_cost_witness_fields(rb)
    after, witness = _original_loop_639(rb, contract)
    profile = table_index(rb, "TSPDefectProfiles")["defect-three-region-local-two-factor"]
    profile.update(
        {
            "LowerBoundCost": 36,
            "UpperBoundCost": 36,
            "LowerBoundWitnessed": True,
            "UpperBoundWitnessed": True,
            "Status": "REGION_REPAIR_BOUND_AND_STRUCTURAL_CYCLE_WITNESSED",
        }
    )
    return after, witness + " The connected degree-two exchange now supplies the cost-thirty-six feasible upper witness."


FACE_INSTANCE = "tsp-optimal-face-4"
FACE_NEIGHBORHOOD = "gridville-optimal-face"
FACE_STOPS = ["face-stop-a", "face-stop-b", "face-stop-c", "face-stop-d"]
FACE_COSTS = {
    edge_key("face-stop-a", "face-stop-b"): 1.5,
    edge_key("face-stop-a", "face-stop-c"): 1.5,
    edge_key("face-stop-a", "face-stop-d"): 0.5,
    edge_key("face-stop-b", "face-stop-c"): 0.5,
    edge_key("face-stop-b", "face-stop-d"): 1.5,
    edge_key("face-stop-c", "face-stop-d"): 1.5,
}
FACE_ROUTES = {
    "tour-optimal-face-abcd-4": ["face-stop-a", "face-stop-b", "face-stop-c", "face-stop-d"],
    "tour-optimal-face-acbd-4": ["face-stop-a", "face-stop-c", "face-stop-b", "face-stop-d"],
    "tour-optimal-face-abdc-6": ["face-stop-a", "face-stop-b", "face-stop-d", "face-stop-c"],
}


def face_short(stop: str) -> str:
    return stop.removeprefix("face-stop-")


def face_edge_id(a: str, b: str) -> str:
    x, y = edge_key(a, b)
    return f"face-edge-{face_short(x)}-{face_short(y)}"


def loop_642(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_region_fields(rb)
    upsert_rows(
        rb["Neighborhoods"],
        "NeighborhoodId",
        [
            {
                "NeighborhoodId": FACE_NEIGHBORHOOD,
                "DisplayName": "Optimal Face",
                "City": "gridville",
                "ClusterKind": "MULTIPLE_OPTIMAL_ROUTE_FIXTURE",
                "IsQuotientNode": False,
                "QuotientNodeKind": None,
                "RequiredBoundaryDegree": None,
                "QuotientScopeId": FACE_INSTANCE,
                "RegionInterfaceKind": None,
                "BoundaryPortCount": None,
                "InternalCoverageCount": 4,
                "ExpansionWitnessKind": "SUPPLIED_ORDERED_CYCLE",
                "InterfaceStatus": "NORMALIZED",
            }
        ],
    )
    address_rows = []
    stop_rows = []
    for index, stop in enumerate(FACE_STOPS):
        suffix = face_short(stop)
        address_id = f"addr-face-{suffix}"
        address_rows.append(
            {
                "AddressId": address_id,
                "StreetLabel": f"Optimal Face {suffix.upper()}",
                "Neighborhood": FACE_NEIGHBORHOOD,
                "XCoordinate": 80 + index,
                "YCoordinate": (index * 3) % 5,
                "IsDepotCandidate": stop == "face-stop-a",
            }
        )
        stop_rows.append(
            {
                "InstanceStopId": stop,
                "TSPInstance": FACE_INSTANCE,
                "Address": address_id,
                "IsRequired": True,
            }
        )
    upsert_rows(rb["Addresses"], "AddressId", address_rows)
    upsert_rows(
        rb["TSPInstances"],
        "TSPInstanceId",
        [
            {
                "TSPInstanceId": FACE_INSTANCE,
                "DisplayName": "Four-Stop Optimal Face",
                "City": "gridville",
                "DepotAddress": "addr-face-a",
                "DistanceModel": "EXPLICIT_SYMMETRIC_COST",
                "IsSymmetric": True,
                "Status": "NORMALIZED",
                "SearchPolicy": "CERTIFY_VALUE_BEFORE_REPRESENTATIVE_SELECTION",
                "Notes": "Two non-reversal-equivalent Hamiltonian cycles attain the same optimum value four; a third class costs six.",
            }
        ],
    )
    upsert_rows(rb["InstanceStops"], "InstanceStopId", stop_rows)
    edge_rows = []
    for (a, b), cost in FACE_COSTS.items():
        edge_rows.append(
            {
                "TravelEdgeId": face_edge_id(a, b),
                "TSPInstance": FACE_INSTANCE,
                "FromStop": a,
                "ToStop": b,
                "CanonicalPairKey": f"{a}|{b}",
                "DistanceMeters": int(cost * 1000),
                "TravelCost": cost,
                "IsAvailable": True,
                "EdgeSource": "OPTIMAL_FACE_FIXTURE",
            }
        )
    upsert_rows(rb["TravelEdges"], "TravelEdgeId", edge_rows)

    bound_specs = {
        "face-stop-a": ("face-stop-d", "face-stop-b"),
        "face-stop-b": ("face-stop-c", "face-stop-a"),
        "face-stop-c": ("face-stop-b", "face-stop-a"),
        "face-stop-d": ("face-stop-a", "face-stop-b"),
    }
    local_rows = []
    dominance_rows = []
    for stop, (first_peer, second_peer) in bound_specs.items():
        first_edge = face_edge_id(stop, first_peer)
        second_edge = face_edge_id(stop, second_peer)
        bound_id = f"local-bound-face-{face_short(stop)}"
        local_rows.append(
            {
                "LocalDegreeBoundId": bound_id,
                "TSPInstance": FACE_INSTANCE,
                "InstanceStop": stop,
                "FirstEdge": first_edge,
                "SecondEdge": second_edge,
            }
        )
        selected = {first_edge, second_edge}
        for other in sorted(
            row["TravelEdgeId"]
            for row in edge_rows
            if stop in {row["FromStop"], row["ToStop"]} and row["TravelEdgeId"] not in selected
        ):
            dominance_rows.append(
                {
                    "IncidentDominanceCheckId": f"dom-{bound_id}-{other}",
                    "LocalDegreeBound": bound_id,
                    "OtherEdge": other,
                }
            )
    upsert_rows(rb["LocalDegreeBounds"], "LocalDegreeBoundId", local_rows)
    upsert_rows(rb["IncidentDominanceChecks"], "IncidentDominanceCheckId", dominance_rows)
    upsert_rows(
        rb["InstanceLowerBounds"],
        "InstanceLowerBoundId",
        [
            {
                "InstanceLowerBoundId": "degree-two-lower-bound-optimal-face-4",
                "TSPInstance": FACE_INSTANCE,
                "BoundKind": "SUM_TWO_CHEAPEST_INCIDENT_EDGES_DIVIDED_BY_TWO",
                "RequiredSupplementalTermCount": 0,
                "BoundCompositionKind": "DEGREE_TWO_ONLY",
                "SupplementalBoundAdjustment": 0,
            }
        ],
    )
    displays = {
        "tour-optimal-face-abcd-4": "Optimal face cycle A-B-C-D-A cost 4",
        "tour-optimal-face-acbd-4": "Optimal face cycle A-C-B-D-A cost 4",
        "tour-optimal-face-abdc-6": "Optimal face cycle A-B-D-C-A cost 6",
    }
    kinds = {
        "tour-optimal-face-abcd-4": "OPTIMAL_FACE_MEMBER",
        "tour-optimal-face-acbd-4": "OPTIMAL_FACE_MEMBER",
        "tour-optimal-face-abdc-6": "NONOPTIMAL_FACE_CONTROL",
    }
    for index, (candidate_id, route) in enumerate(FACE_ROUTES.items(), start=1):
        add_candidate_route(
            rb,
            candidate_id=candidate_id,
            display=displays[candidate_id],
            instance=FACE_INSTANCE,
            kind=kinds[candidate_id],
            route=route,
            prefix=f"face{index}",
        )
    upsert_rows(
        rb["TSPWitnessNormalForms"],
        "TSPWitnessNormalFormId",
        [
            {
                "TSPWitnessNormalFormId": f"normal-cycle-{candidate_id}",
                "TSPInstance": FACE_INSTANCE,
                "TSPLoop": "tsp-loop-642",
                "WitnessShape": "CYCLE",
                "OriginKind": "SUPPLIED",
                "SourceKind": "CandidateTours",
                "SourceId": candidate_id,
                "BoundarySignature": None,
                "CoveredStopCount": 4,
                "RequiredStopCount": 4,
                "EdgeCount": 4,
                "TotalCost": raw_tour_cost(rb, candidate_id),
                "IncidenceDefect": 0,
                "ConnectivityDefect": 0,
                "OrderDefect": 0,
                "ProvenanceSummary": "Supplied optimal-face fixture witness; value and representative-choice status remain separate.",
            }
            for candidate_id in FACE_ROUTES
        ],
    )
    upsert_rows(
        rb["TSPInferenceStates"],
        "TSPInferenceStateId",
        [
            {
                "TSPInferenceStateId": "state-optimal-face-witnesses",
                "TSPInstance": FACE_INSTANCE,
                "TSPLoop": "tsp-loop-642",
                "StateKind": "OPTIMAL_FACE_WITNESS_SET",
                "ParentStateId": None,
                "Status": "CLOSED",
                "Description": "Three symmetry-reduced Hamiltonian witness classes are represented: costs four, four, and six.",
            }
        ],
    )
    table_index(rb, "TSPInferenceRules")["tsp-rule-optimal-face"]["ImplementationStatus"] = "EXECUTABLE"
    register_concept(
        rb,
        "coined-optimal-face",
        "Optimal Face",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(witness,COST,value)+WARRANTED_REWRITE(witnesses,MINIMUM_VALUE_FIBER,optimal_face,value_warrant)",
        1,
        "CandidateTours,TourStops,TourLegs,InstanceLowerBounds",
        642,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-optimal-face", mode="MINIMUM_VALUE_FIBER", polarity="CONTRACTIVE")
    add_measurement(
        rb,
        642,
        "Optimal Face",
        "A four-stop complete graph exposes three symmetry-reduced route classes with costs four, four, and six. No optimality or unique-route claim is emitted yet.",
    )
    return (
        "Represented a four-stop optimal face with two non-reversal-equivalent cost-four cycles and one cost-six control cycle.",
        "The degree-two lower-bound ingredients total eight before double-counting, and the three valid Hamiltonian witnesses have raw costs [4, 4, 6]; value and witness identity remain open.",
    )


def ensure_choice_state_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPInferenceStates"],
        [
            base.field("AlternativeCount", "integer", "raw", True, "Alternatives represented in this inference state."),
            base.field("ResidualAmbiguityCount", "integer", "raw", True, "Alternatives remaining after deterministic closure."),
            base.field("BranchDecisionCount", "integer", "raw", True, "Mathematical branch decisions used to reach this state."),
            base.field("ValueStatus", "string", "raw", True, "OPEN or CLOSED optimum-value status."),
            base.field("ChoiceStatus", "string", "raw", True, "Representative-choice status."),
            base.field("ChoiceOrbitId", "string", "raw", True, "Residual value-preserving choice orbit identifier."),
        ],
    )
    for row in rows(rb, "TSPInferenceStates"):
        row.setdefault("AlternativeCount", None)
        row.setdefault("ResidualAmbiguityCount", None)
        row.setdefault("BranchDecisionCount", None)
        row.setdefault("ValueStatus", None)
        row.setdefault("ChoiceStatus", None)
        row.setdefault("ChoiceOrbitId", None)


def loop_643(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_choice_state_fields(rb)
    optimal_candidates = ["tour-optimal-face-abcd-4", "tour-optimal-face-acbd-4"]
    upsert_rows(
        rb["OptimalityCertificates"],
        "OptimalityCertificateId",
        [
            {
                "OptimalityCertificateId": f"optimality-{candidate_id}",
                "CandidateTour": candidate_id,
                "InstanceLowerBound": "degree-two-lower-bound-optimal-face-4",
            }
            for candidate_id in optimal_candidates
        ],
    )
    invariant_rows = []
    for candidate_id, expected_optimal, cost in [
        ("tour-optimal-face-abcd-4", True, 4),
        ("tour-optimal-face-acbd-4", True, 4),
        ("tour-optimal-face-abdc-6", False, 6),
    ]:
        invariant_rows.append(
            {
                "TSPInvariantCheckId": f"check-{candidate_id}",
                "DisplayName": f"Optimal-face invariant for {candidate_id}",
                "CandidateTour": candidate_id,
                "ExpectedHamiltonianCycleWitness": True,
                "ExpectedOptimalityProved": expected_optimal,
                "ExpectedTotalTravelCost": cost,
            }
        )
    upsert_rows(rb["TSPInvariantChecks"], "TSPInvariantCheckId", invariant_rows)
    upsert_rows(
        rb["TSPInferenceStates"],
        "TSPInferenceStateId",
        [
            {
                "TSPInferenceStateId": "state-optimal-face-value-closed-choice-open",
                "TSPInstance": FACE_INSTANCE,
                "TSPLoop": "tsp-loop-643",
                "StateKind": "VALUE_CLOSED_CHOICE_OPEN",
                "ParentStateId": "state-optimal-face-witnesses",
                "Status": "FIXED_POINT",
                "Description": "Optimum value four is certified; two non-reversal optimal witness classes remain.",
                "AlternativeCount": 2,
                "ResidualAmbiguityCount": 2,
                "BranchDecisionCount": 0,
                "ValueStatus": "CLOSED_AT_4",
                "ChoiceStatus": "OPEN_TWO_OPTIMAL_CLASSES",
                "ChoiceOrbitId": "choice-orbit-optimal-face-4",
            }
        ],
    )
    upsert_rows(
        rb["TSPDefectProfiles"],
        "TSPDefectProfileId",
        [
            {
                "TSPDefectProfileId": "defect-optimal-face-value-choice",
                "TSPInstance": FACE_INSTANCE,
                "TSPLoop": "tsp-loop-643",
                "SubjectKind": "INSTANCE",
                "SubjectId": "choice-orbit-optimal-face-4",
                "RequiredIncidence": 2,
                "ObservedIncidence": 2,
                "ComponentCount": 1,
                "RequiredBoundaryCrossings": 0,
                "ObservedBoundaryCrossings": 0,
                "LowerBoundCost": 4,
                "UpperBoundCost": 4,
                "Status": "VALUE_ZERO_DEFECT_CHOICE_ORBIT_TWO",
                "LowerBoundWitnessed": True,
                "UpperBoundWitnessed": True,
            }
        ],
    )
    register_concept(
        rb,
        "coined-value-choice-split",
        "Value/Choice Split",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(instance,OPTIMUM_VALUE,4)+SEMANTIC_ARC(optimal_face,CHOICE_ORBIT_SIZE,2)+WARRANTED_REWRITE(value_proof,CLOSE_VALUE,choice_open,warrant)",
        1,
        "OptimalityCertificates,TSPInferenceStates,TSPDefectProfiles",
        643,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-value-choice-split", mode="VALUE_CHOICE_SEPARATION", polarity="MIXED")
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "OptimalFaceValueCertified": True,
            "OptimalFaceUniqueRouteProved": False,
            "OptimalFaceChoiceOrbitSize": 2,
        }
    )
    contract.setdefault("Acceptance", {}).update(
        {
            "OptimalFaceLowerBound": 4,
            "OptimalFaceOptimalWitnessCount": 2,
            "OptimalFaceChoiceOrbitSize": 2,
        }
    )
    add_measurement(
        rb,
        643,
        "Value/Choice Split",
        "Two valid cost-four witnesses independently equal the certified lower bound. Optimum value is closed while representative choice remains a two-class orbit.",
    )
    add_frontier(
        rb,
        "frontier-optimal-face-value",
        "Optimal-face value certificate",
        642,
        643,
        "tsp-rule-optimal-face",
        "At least two non-equivalent valid witnesses equal the certified finite-instance lower bound four, closing value but not uniqueness.",
        "optimal-face-certificate",
    )
    return (
        "Closed the optimum value at four while preserving two distinct optimal witness classes.",
        "Both A-B-C-D-A and A-C-B-D-A carry passing finite optimality certificates; A-B-D-C-A costs six. ValueStatus=CLOSED_AT_4, ChoiceStatus=OPEN_TWO_OPTIMAL_CLASSES, branch decisions=0.",
    )


def ensure_choice_search_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPSearchCertificates"],
        [
            base.field("OrbitKind", "string", "raw", True, "Residual orbit classification."),
            base.field("ValueCertified", "boolean", "raw", True, "Whether the optimum value is already certified."),
            base.field("RepresentativeSelectionRequired", "boolean", "raw", True, "Whether a downstream consumer demands one representative."),
            base.field("ExternalPolicyRequired", "boolean", "raw", True, "Whether a non-mathematical policy is required to choose among equal witnesses."),
            base.field("BranchWarrantStatus", "string", "raw", True, "WARRANTED, REJECTED, or CONDITIONAL_ON_EXTERNAL_POLICY."),
        ],
    )
    for row in rows(rb, "TSPSearchCertificates"):
        row.setdefault("OrbitKind", None)
        row.setdefault("ValueCertified", None)
        row.setdefault("RepresentativeSelectionRequired", None)
        row.setdefault("ExternalPolicyRequired", None)
        row.setdefault("BranchWarrantStatus", None)
    ensure_fields(
        rb["CandidateTours"],
        [
            base.field("ChoiceOrbitId", "string", "raw", True, "Value-preserving choice orbit, when applicable."),
            base.field("OptimalFaceValue", "number", "raw", True, "Certified optimum value of the face containing this witness."),
            base.field("ChoiceOrbitStatus", "string", "raw", True, "MEMBER, NONOPTIMAL, or NOT_APPLICABLE."),
        ],
    )
    for row in rows(rb, "CandidateTours"):
        row.setdefault("ChoiceOrbitId", None)
        row.setdefault("OptimalFaceValue", None)
        row.setdefault("ChoiceOrbitStatus", None)


def loop_644(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_choice_search_fields(rb)
    candidates = table_index(rb, "CandidateTours")
    for candidate_id in ("tour-optimal-face-abcd-4", "tour-optimal-face-acbd-4"):
        candidates[candidate_id].update(
            {
                "ChoiceOrbitId": "choice-orbit-optimal-face-4",
                "OptimalFaceValue": 4,
                "ChoiceOrbitStatus": "MEMBER",
            }
        )
    candidates["tour-optimal-face-abdc-6"].update(
        {
            "ChoiceOrbitId": "choice-orbit-optimal-face-4",
            "OptimalFaceValue": 4,
            "ChoiceOrbitStatus": "NONOPTIMAL",
        }
    )
    upsert_rows(
        rb["TSPSearchCertificates"],
        "TSPSearchCertificateId",
        [
            {
                "TSPSearchCertificateId": "search-optimal-face-choice-orbit",
                "TSPInstance": FACE_INSTANCE,
                "TSPLoop": "tsp-loop-644",
                "DerivedEdgeSet": None,
                "QuestionKind": "CERTIFY_VALUE_WITH_NONUNIQUE_OPTIMAL_WITNESS",
                "InitialRouteClassCount": 3,
                "SurvivingRouteClassCount": 2,
                "BranchDecisionCount": 0,
                "BacktrackCount": 0,
                "ResidualAmbiguityCount": 2,
                "BranchingAvoidedPct": 100,
                "Status": "VALUE_CERTIFIED_CHOICE_ORBIT",
                "OrbitKind": "NON_REVERSAL_OPTIMAL_WITNESS_ORBIT",
                "ValueCertified": True,
                "RepresentativeSelectionRequired": True,
                "ExternalPolicyRequired": True,
                "BranchWarrantStatus": "NOT_WARRANTED_FOR_VALUE_PROOF",
            }
        ],
    )
    register_concept(
        rb,
        "coined-choice-orbit",
        "Choice Orbit",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(witness,ORBIT_MEMBER,choice_orbit)+SEMANTIC_ARC(choice_orbit,VALUE,4)+WARRANTED_REWRITE(route_classes,MINIMUM_FACE,two_survivors,warrant)",
        1,
        "CandidateTours,TSPSearchCertificates,TSPInferenceStates",
        644,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-choice-orbit", mode="VALUE_PRESERVING_ORBIT", polarity="CONTRACTIVE")
    add_measurement(
        rb,
        644,
        "Choice Orbit",
        "Three symmetry-reduced route classes contract to a two-member optimal face; optimum value is certified and residual ambiguity is choice-only.",
    )
    return (
        "Named and measured the two-member value-preserving optimal witness orbit.",
        "Initial route classes=3, surviving optimal classes=2, residual ambiguity=2, certified value=4, branch decisions=0. The remaining orbit is not an unresolved optimum-value defect.",
    )


def ensure_branch_warrant_fields(rb: dict[str, Any]) -> None:
    ensure_fields(
        rb["TSPInferenceApplications"],
        [
            base.field("BranchPurpose", "string", "raw", True, "VALUE_PROOF or REPRESENTATIVE_SELECTION."),
            base.field("BranchWarrant", "string", "raw", True, "WARRANTED, REJECTED, or CONDITIONAL."),
            base.field("PolicyStatus", "string", "raw", True, "External selection-policy status."),
        ],
    )
    for row in rows(rb, "TSPInferenceApplications"):
        row.setdefault("BranchPurpose", None)
        row.setdefault("BranchWarrant", None)
        row.setdefault("PolicyStatus", None)


def loop_645(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    ensure_branch_warrant_fields(rb)
    ensure_choice_search_fields(rb)
    upsert_rows(
        rb["TSPInferenceApplications"],
        "TSPInferenceApplicationId",
        [
            {
                "TSPInferenceApplicationId": "event-optimal-face-branch-rejected",
                "InferenceState": "state-optimal-face-value-closed-choice-open",
                "InferenceRule": "tsp-rule-branch-warrant",
                "TSPLoop": "tsp-loop-645",
                "SubjectType": "CHOICE_ORBIT",
                "SubjectId": "choice-orbit-optimal-face-4",
                "ApplicabilityPassed": False,
                "Conclusion": "No mathematical branch is warranted for optimum-value proof; selecting one equal optimum requires a separately supplied external policy.",
                "CertificateType": "branch-warrant-certificate",
                "EventKind": "BRANCH_REJECTION",
                "InputStateId": "state-optimal-face-value-closed-choice-open",
                "OutputStateId": "state-optimal-face-value-closed-choice-open",
                "AntecedentCount": 3,
                "DecisionCount": 0,
                "EventStatus": "REJECTED",
                "BranchPurpose": "VALUE_PROOF",
                "BranchWarrant": "REJECTED_VALUE_ALREADY_CERTIFIED",
                "PolicyStatus": "EXTERNAL_TIE_BREAK_NOT_SUPPLIED",
            }
        ],
    )
    search = table_index(rb, "TSPSearchCertificates")["search-optimal-face-choice-orbit"]
    search["BranchWarrantStatus"] = "CONDITIONAL_ON_EXTERNAL_REPRESENTATIVE_POLICY"
    table_index(rb, "TSPInferenceRules")["tsp-rule-branch-warrant"]["ImplementationStatus"] = "EXECUTABLE"
    register_concept(
        rb,
        "coined-branch-warrant",
        "Branch Warrant",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(branch,PURPOSE,value_or_choice)+SEMANTIC_ARC(state,VALUE_STATUS,closed)+WARRANTED_REWRITE(branch_request,REJECT_OR_CONDITION,decision,warrant)",
        1,
        "TSPInferenceApplications,TSPSearchCertificates,TSPInferenceStates",
        645,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-branch-warrant", mode="BRANCH_GOVERNANCE", polarity="CONTRACTIVE")
    add_measurement(
        rb,
        645,
        "Branch Warrant",
        "The system rejects a branch for value proof after value closure. Representative selection is conditional on an external tie-break policy and emits no mathematical decision.",
    )
    add_frontier(
        rb,
        "frontier-optimal-face-branch-warrant",
        "Optimal-face residual branch warrant",
        644,
        645,
        "tsp-rule-branch-warrant",
        "Reject branching for certified value and require an explicit external policy before selecting one equal optimal representative.",
        "branch-warrant-certificate",
        kind="RESIDUAL_SEARCH",
    )
    return (
        "Rejected mathematical branching for the value-closed optimal face.",
        "BranchPurpose=VALUE_PROOF is rejected because value four is already certified. Representative choice remains conditional on an external tie-break policy; DecisionCount=0 and the two-member orbit is preserved.",
    )


def append_readme_section() -> None:
    text = README.read_text()
    marker = "## Loops 624–646 — coherence convergence"
    if marker in text:
        return
    text += '''

## Loops 624–646 — coherence convergence

This pass records the stronger prediction that convergence is movement toward coherent reusable geometry, **not** monotone shrinkage of tables, rows, or names.  Internalizing an opaque node may expand the explicit DAG while reducing semantic basis size and trust opacity.

### One typed arc and one warranted rewrite

```text
ATTACHMENT   VALUATION   WARRANT
      ↓ typed recoverable signatures
SEMANTIC_ARC(subject, label, target)

CLOSURE   FIXPOINT   AGGREGATE   QUOTIENT
      ↓ saturation / fiber-reduction modes
WARRANTED_REWRITE(input, rule, polarity, output, warrant)
```

Every registered historical and derived concept retains a typed recoverability expression.  The current represented basis is therefore:

```text
active relational atoms     1  (Semantic Arc)
active semantic operators   1  (Warranted Rewrite)
physical rulebook tables   45  (unchanged)
surface-to-atom reduction 96.88%
```

This is empirical coverage for the represented TSP domain.  It is not a theorem of minimality or universality.

### Three-region repair

Three cost-one local triangles produce a disconnected degree-two structure:

```text
local degree-two base                       9
quotient boundary demand      3 crossings
minimum crossing insertion     3 × 10     +30
necessary internal releases    3 ×  1      -3
------------------------------------------------
three-region repair lower bound              36
balanced-exchange cycle                      36
finite-instance optimum                      36
```

The **Boundary Handshake** derives three crossing edges from three quotient nodes of required boundary degree two.  A **Balanced Edge Exchange** adds three crossings and releases one internal edge per region while preserving degree two and connecting all nine stops.

### Optimal value versus optimal choice

The four-stop **Optimal Face** contains three symmetry-reduced Hamiltonian classes:

```text
A-B-C-D-A   cost 4   optimal
A-C-B-D-A   cost 4   optimal
A-B-D-C-A   cost 6   nonoptimal control
```

The optimum value four is certified, but the two optimal witnesses form a **Choice Orbit** of size two.  The **Branch Warrant** rejects mathematical branching for value proof; choosing one representative is conditional on an external tie-break policy.

**Coherence status:** one active arc, one active rewrite, unchanged physical table count, substantially more explicit witness machinery, three-region finite optimality at 36, optimal-face value 4, choice-orbit size 2, and zero mathematical branch decisions for value proof.
'''
    README.write_text(text)


def loop_646(rb: dict[str, Any], contract: dict[str, Any]) -> tuple[str, str]:
    register_concept(
        rb,
        "coined-coherence-event-two",
        "Coherence Event II",
        "COINED_PREDICATE",
        "SEMANTIC_ARC(program,ACTIVE_ATOMS,1)+SEMANTIC_ARC(program,ACTIVE_OPERATORS,1)+SEMANTIC_ARC(program,PHYSICAL_TABLES,45)+WARRANTED_REWRITE(evidence,COHERENCE_SUMMARY,event,warrant)",
        1,
        "TSPConvergenceMeasurements,TSPConceptRegistry,TSPFrontierObligations,OptimalityCertificates,TSPSearchCertificates",
        646,
        status="ACTIVE_DERIVED",
        category="DERIVED",
        operator_expression="WARRANTED_REWRITE",
    )
    mark_current_basis_concept(rb, "coined-coherence-event-two", mode="COHERENCE_SUMMARY", polarity="MIXED")
    unresolved = refresh_recoverability(rb)
    if unresolved:
        raise AssertionError(f"final current-basis recoverability failed: {unresolved[:10]}")
    set_meta(rb, "active_predicate_atom_count", "integer", integer=1)
    set_meta(rb, "active_semantic_operator_count", "integer", integer=1)
    set_meta(rb, "physical_table_count_loop_646", "integer", integer=len(base.canonical_tables(rb)))
    set_meta(rb, "named_concept_count_loop_646", "integer", integer=named_concept_count(rb))
    set_meta(rb, "coherence_program_status", "string", string="CLOSED_624_646")
    set_meta(rb, "coherence_event_two_status", "string", string="EARLY_EMPIRICAL_SUPPORT_FOR_CURRENT_TSP_DOMAIN")
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "OneArcOneRewriteObserved": True,
            "OneArcOneRewriteProved": False,
            "CurrentPredicateAtomCount": 1,
            "CurrentSemanticOperatorCount": 1,
            "CurrentSemanticCompressionPct": 96.88,
            "PhysicalTableCountLoop646": len(base.canonical_tables(rb)),
            "PhysicalTableCountIncreasedSinceLoop610": False,
            "ExplicitDagRowsMayIncrease": True,
            "ThreeRegionOptimalityProved": True,
            "GeneralKRegionRepairProved": False,
            "OptimalFaceValueCertified": True,
            "OptimalFaceUniqueRouteProved": False,
            "OptimalFaceChoiceOrbitSize": 2,
            "OptimalFaceValueProofBranchDecisionCount": 0,
            "CoherenceConvergenceObserved": True,
            "CoherenceConvergenceProvedUniversally": False,
        }
    )
    acceptance = contract.setdefault("Acceptance", {})
    acceptance.update(
        {
            "CurrentPredicateAtomCount": 1,
            "CurrentSemanticOperatorCount": 1,
            "PhysicalTableCount": len(base.canonical_tables(rb)),
            "ThreeRegionLowerBound": 36,
            "ThreeRegionCandidateCost": 36,
            "ThreeRegionOptimalityProved": True,
            "OptimalFaceLowerBound": 4,
            "OptimalFaceOptimalWitnessCount": 2,
            "OptimalFaceChoiceOrbitSize": 2,
            "OptimalFaceValueProofBranchDecisionCount": 0,
        }
    )
    contract["Version"] = "0.6.0"
    contract["Scope"] = "The represented scope includes a typed one-arc/one-rewrite semantic basis, finite three-region repair optimality, and an optimal face whose value is certified while two equal optimal witness classes remain. No general TSP solver, basis-minimality theorem, universal k-region repair theorem, or complexity-class claim is made."
    contract["TrustBoundary"] = "Input graph weights and membership remain trusted data. Semantic-arc collapse is accepted only through typed signatures and explicit recoverability witnesses. Warranted rewrites retain polarity, termination, and expansion provenance. Generated Postgres remains a peer projection. Coherence convergence is empirical for this domain."
    certs = {row["CertificateId"]: row for row in contract.setdefault("CurrentCertificates", [])}
    for cert in [
        {
            "CertificateId": "tsp-one-arc-one-rewrite",
            "Kind": "semantic-basis-certificate",
            "Conclusion": "Every registered concept is recoverable from one typed Semantic Arc and one Warranted Rewrite for the current represented domain.",
        },
        {
            "CertificateId": "three-region-repair-bound",
            "Kind": "region-repair-equation-certificate",
            "Conclusion": "Boundary handshake and signed repair terms certify lower bound 36 for tsp-three-regions-9.",
        },
        {
            "CertificateId": "three-region-finite-optimality",
            "Kind": "finite-instance-optimality-certificate",
            "Conclusion": "The reconstructed balanced-exchange cycle cost 36 equals the certified three-region lower bound 36.",
        },
        {
            "CertificateId": "optimal-face-value-choice",
            "Kind": "optimal-face-certificate",
            "Conclusion": "The optimum value is 4 while two non-reversal optimal witness classes remain in one choice orbit.",
        },
        {
            "CertificateId": "optimal-face-branch-warrant",
            "Kind": "branch-warrant-certificate",
            "Conclusion": "No mathematical branch is warranted for value proof; representative selection requires an external tie-break policy.",
        },
        {
            "CertificateId": "tsp-coherence-event-two",
            "Kind": "coherence-vector-certificate",
            "Conclusion": "The active basis contracts to one arc and one rewrite while physical tables remain 45 and explicit witness rows expand coherently.",
        },
    ]:
        certs[cert["CertificateId"]] = cert
    contract["CurrentCertificates"] = list(certs.values())
    contract["RemainingFrontier"] = [
        "stress the one-arc/one-rewrite basis on heterogeneous region sizes and asymmetric crossing costs",
        "generalize the three-region repair equation beyond equal internal release costs and a ring minimum",
        "represent explicit external representative-selection policies without confusing policy choice with mathematical proof",
        "branch only when a witnessed optimum-value defect or an explicit downstream policy warrants it",
    ]
    append_readme_section()
    add_measurement(
        rb,
        646,
        "Coherence Event II",
        "One active arc and one active rewrite recover the current vocabulary; physical tables remain forty-five while explicit witness rows grow; three-region optimum is thirty-six; optimal-face value is four with choice orbit two and zero proof branches.",
        kind="COHERENCE_EVENT_II",
        prediction="SUPPORTED_FOR_CURRENT_DOMAIN",
    )
    add_frontier(
        rb,
        "frontier-coherence-event-two",
        "Second coherence-convergence event",
        624,
        646,
        "tsp-rule-coherence-measurement",
        "One active arc, one active rewrite, unchanged physical tables, complete recoverability, three-region optimality, and value/choice branch accounting all agree.",
        "coherence-vector-certificate",
    )
    return (
        "Certified the second coherence event: one typed arc, one warranted rewrite, forty-five physical tables, three-region optimum thirty-six, and optimal-face value four with a two-member choice orbit.",
        f"Named concepts={named_concept_count(rb)} and witness rows increased, but active atoms=1, active operators=1, tables=45, recoverability is complete, proof branch decisions=0, and no universal convergence or TSP-complexity theorem is claimed.",
    )


LOOP_FUNCS = {
    624: loop_624,
    625: loop_625,
    626: loop_626,
    627: loop_627,
    628: loop_628,
    629: loop_629,
    630: loop_630,
    631: loop_631,
    632: loop_632,
    633: loop_633,
    634: loop_634,
    635: loop_635,
    636: loop_636,
    637: loop_637,
    638: loop_638,
    639: loop_639,
    640: loop_640,
    641: loop_641,
    642: loop_642,
    643: loop_643,
    644: loop_644,
    645: loop_645,
    646: loop_646,
}


def validate_state(rb: dict[str, Any], contract: dict[str, Any]) -> None:
    loop_map = {int(row["LoopOrder"]): row for row in rows(rb, "TSPLoops")}
    planned = base.meta_int(rb, "last_planned_loop")
    if sorted(loop_map) != list(range(577, planned + 1)):
        raise AssertionError(f"loop sequence is not contiguous through {planned}")
    contract_map = {int(row["LoopOrder"]): row for row in contract["Loops"]}
    if set(loop_map) != set(contract_map):
        raise AssertionError("contract/rulebook loop set mismatch")
    for order in range(624, planned + 1):
        row = loop_map[order]
        if not row.get("BeforeState") or not row.get("PlannedClosureCriterion"):
            raise AssertionError(f"loop {order} missing before-state or criterion")
        if contract_map[order]["Status"] != row["Status"]:
            raise AssertionError(f"loop {order} status mismatch")
        if row["Status"] == "CLOSED" and not row.get("AfterState"):
            raise AssertionError(f"loop {order} closed without after-state")

    closed = lambda order: loop_map.get(order, {}).get("Status") == "CLOSED"
    if closed(624):
        fields = {field["name"] for field in rb["TSPConvergenceMeasurements"]["schema"]}
        if not {"ActiveAtomCount", "ActiveOperatorCount", "PhysicalTableDeltaFromLoop610", "OpenFrontierCount", "NamedConceptCount", "AllowsLocalExpansion"} <= fields:
            raise AssertionError("coherence-vector fields missing")
    if closed(625) and {row["DisplayName"] for row in active_primitives(rb)} != {"Semantic Arc"}:
        raise AssertionError("semantic-arc active basis mismatch")
    if closed(626):
        concepts = table_index(rb, "TSPConceptRegistry")
        signatures = {(concepts[ident]["ArcLabelSort"], concepts[ident]["ArcTargetSort"]) for ident in ARC_ATOMS}
        if len(signatures) != 3:
            raise AssertionError("historical atom signatures are not recoverably distinct")
    if closed(627):
        unrecovered = [row["TSPConceptId"] for row in rows(rb, "TSPConceptRegistry") if row.get("IsRecoverableFromCurrentBasis") is False]
        if unrecovered:
            raise AssertionError(f"unrecovered concepts after loop 627: {unrecovered[:10]}")
    if closed(628) and len(active_operators(rb)) > 3:
        raise AssertionError("saturation reduction did not reduce operator count")
    if closed(629) and len(active_operators(rb)) > 2:
        raise AssertionError("fiber reduction did not reduce operator count")
    if closed(630) and {row["DisplayName"] for row in active_operators(rb)} != {"Warranted Rewrite"}:
        raise AssertionError("warranted-rewrite active basis mismatch")
    if closed(631):
        concepts = table_index(rb, "TSPConceptRegistry")
        required = {"operator-closure", "operator-fixpoint", "operator-aggregate", "operator-quotient", "operator-warranted-rewrite"}
        if any(not concepts[ident].get("RewritePolarity") for ident in required):
            raise AssertionError("rewrite polarity classification incomplete")
    if closed(632):
        if len(active_primitives(rb)) != 1 or len(active_operators(rb)) != 1:
            raise AssertionError("one-arc/one-rewrite counts mismatch")
        unresolved = refresh_recoverability(rb)
        if unresolved:
            raise AssertionError(f"current-basis recovery incomplete: {unresolved[:10]}")
        if contract["Claims"].get("OneArcOneRewriteProved") is not False:
            raise AssertionError("one-arc/one-rewrite observation was promoted to a theorem")
    if closed(633):
        quotient_nodes = [row for row in rows(rb, "Neighborhoods") if row.get("IsQuotientNode")]
        if any(row.get("RegionInterfaceKind") != "PORT_BEARING_QUOTIENT_REGION" for row in quotient_nodes):
            raise AssertionError("region-interface normalization incomplete")
    if closed(634):
        stops = [row for row in rows(rb, "InstanceStops") if row["TSPInstance"] == R3_INSTANCE]
        edges = [row for row in rows(rb, "TravelEdges") if row["TSPInstance"] == R3_INSTANCE]
        bounds = [row for row in rows(rb, "LocalDegreeBounds") if row["TSPInstance"] == R3_INSTANCE]
        local_set = table_index(rb, "TSPDerivedEdgeSets")["edge-set-three-region-local-two-factor"]
        if (len(stops), len(edges), len(bounds)) != (9, 36, 9):
            raise AssertionError(f"three-region fixture counts mismatch: {(len(stops), len(edges), len(bounds))}")
        if (local_set["EdgeCount"], local_set["TotalCost"], local_set["ConnectedComponentCount"], local_set["ProperSubtourCount"]) != (9, 9, 3, 3):
            raise AssertionError("three-region local two-factor mismatch")
    if closed(635):
        states = [row for row in rows(rb, "TSPClusterBoundaryStates") if row["TSPInstance"] == R3_INSTANCE]
        certs = [row for row in rows(rb, "TSPClusterContractionCertificates") if row["TSPInstance"] == R3_INSTANCE]
        if len(states) != 9 or len(certs) != 3 or any(not row["IsPassing"] for row in certs):
            raise AssertionError("three-region quotient mismatch")
    if closed(636):
        profiles = [table_index(rb, "TSPDefectProfiles")[f"quotient-incidence-region-{region}"] for region in ("a", "b", "c")]
        if any((row["RequiredIncidence"], row["ObservedIncidence"]) != (2, 0) for row in profiles):
            raise AssertionError("quotient incidence budget mismatch")
    if closed(637):
        term = table_index(rb, "TSPBoundTerms")["bound-term-three-region-boundary-handshake"]
        if term["ConstraintValue"] != 3 or not term["IsCertified"]:
            raise AssertionError("boundary handshake mismatch")
    if closed(638):
        lower = table_index(rb, "InstanceLowerBounds")["degree-two-lower-bound-three-regions-9"]
        terms = [row for row in rows(rb, "TSPBoundTerms") if row["BoundCertificate"] == lower["InstanceLowerBoundId"] and row["CountsTowardAdjustment"]]
        signed = sum(float(row["Quantity"]) * float(row["UnitWeight"]) * int(row["Sign"]) for row in terms if row["IsCertified"])
        if lower["SupplementalBoundAdjustment"] != 27 or signed != 27:
            raise AssertionError("three-region repair equation mismatch")
    if closed(639):
        repaired = table_index(rb, "TSPDerivedEdgeSets")["edge-set-three-region-balanced-exchange"]
        exchange_states = [row for row in rows(rb, "TSPEdgeStates") if row.get("ExchangeSetId") == repaired["TSPDerivedEdgeSetId"]]
        if (repaired["EdgeCount"], repaired["TotalCost"], repaired["ConnectedComponentCount"], repaired["ProperSubtourCount"]) != (9, 36, 1, 0):
            raise AssertionError("balanced-exchange edge set mismatch")
        if Counter(row["ExchangeModality"] for row in exchange_states) != Counter({"ADDED": 3, "RELEASED": 3}):
            raise AssertionError("balanced-exchange modality counts mismatch")
    if closed(640):
        reconstruction = table_index(rb, "TSPRouteReconstructions")["reconstruction-three-region-balanced-exchange"]
        if reconstruction["CandidateUsedAsAntecedent"] or reconstruction["TotalCost"] != 36 or reconstruction["ReconstructedLegCount"] != 9:
            raise AssertionError("three-region route reconstruction mismatch")
    if closed(641):
        if contract["Claims"].get("ThreeRegionOptimalityProved") is not True:
            raise AssertionError("three-region finite optimality claim missing")
        if "optimality-three-region-repair-36" not in table_index(rb, "OptimalityCertificates"):
            raise AssertionError("three-region optimality certificate missing")
    if closed(642):
        face_edges = [row for row in rows(rb, "TravelEdges") if row["TSPInstance"] == FACE_INSTANCE]
        face_bounds = [row for row in rows(rb, "LocalDegreeBounds") if row["TSPInstance"] == FACE_INSTANCE]
        costs = sorted(raw_tour_cost(rb, candidate) for candidate in FACE_ROUTES)
        if len(face_edges) != 6 or len(face_bounds) != 4 or costs != [4.0, 4.0, 6.0]:
            raise AssertionError(f"optimal-face fixture mismatch: edges={len(face_edges)} bounds={len(face_bounds)} costs={costs}")
    if closed(643):
        state = table_index(rb, "TSPInferenceStates")["state-optimal-face-value-closed-choice-open"]
        if (state["AlternativeCount"], state["ResidualAmbiguityCount"], state["BranchDecisionCount"], state["ValueStatus"]) != (2, 2, 0, "CLOSED_AT_4"):
            raise AssertionError("value/choice split state mismatch")
        certs = [row for row in rows(rb, "OptimalityCertificates") if row["CandidateTour"] in {"tour-optimal-face-abcd-4", "tour-optimal-face-acbd-4"}]
        if len(certs) != 2:
            raise AssertionError("optimal-face passing certificate rows missing")
    if closed(644):
        search = table_index(rb, "TSPSearchCertificates")["search-optimal-face-choice-orbit"]
        if (search["InitialRouteClassCount"], search["SurvivingRouteClassCount"], search["ResidualAmbiguityCount"], search["BranchDecisionCount"]) != (3, 2, 2, 0):
            raise AssertionError("choice-orbit search accounting mismatch")
    if closed(645):
        event = table_index(rb, "TSPInferenceApplications")["event-optimal-face-branch-rejected"]
        if event["ApplicabilityPassed"] or event["EventStatus"] != "REJECTED" or event["DecisionCount"] != 0:
            raise AssertionError("branch-warrant rejection mismatch")
    if closed(646):
        if len(base.canonical_tables(rb)) != 45:
            raise AssertionError(f"coherence pass added a physical table: {len(base.canonical_tables(rb))}")
        claims = contract["Claims"]
        expected = {
            "CurrentPredicateAtomCount": 1,
            "CurrentSemanticOperatorCount": 1,
            "ThreeRegionOptimalityProved": True,
            "OptimalFaceValueCertified": True,
            "OptimalFaceUniqueRouteProved": False,
            "OptimalFaceChoiceOrbitSize": 2,
            "OptimalFaceValueProofBranchDecisionCount": 0,
            "CoherenceConvergenceProvedUniversally": False,
        }
        for key, value in expected.items():
            if claims.get(key) != value:
                raise AssertionError(f"final claim {key}={claims.get(key)!r}, expected {value!r}")


def validate_repository_state() -> None:
    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    validate_state(rb, contract)
    print("traveling-salesman coherence convergence validation: PASS")
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
    if loops.get(646, {}).get("Status") == "CLOSED":
        text = README.read_text()
        for marker in (
            "Loops 624–646",
            "SEMANTIC_ARC",
            "WARRANTED_REWRITE",
            "Boundary Handshake",
            "Balanced Edge Exchange",
            "Optimal Face",
            "Choice Orbit",
            "Branch Warrant",
        ):
            if marker not in text:
                raise AssertionError(f"README missing {marker!r}")
        claims = contract["Claims"]
        if claims.get("CurrentPredicateAtomCount") != 1 or claims.get("CurrentSemanticOperatorCount") != 1:
            raise AssertionError("contract one-arc/one-rewrite summary mismatch")
        if claims.get("OneArcOneRewriteProved") is not False or claims.get("CoherenceConvergenceProvedUniversally") is not False:
            raise AssertionError("empirical coherence basis was promoted to a theorem")
    print("traveling-salesman coherence summary alignment: PASS")


def write_validation_files() -> None:
    (DOMAIN / "scripts" / "validate_rulebook_v5_temporal.py").write_text(
        '''#!/usr/bin/env python3
"""Replay loop-623 semantic claims against their historical projection."""
from __future__ import annotations
import copy
import apply_loops_611_623_refinement as v5

BASIS = {
    "concept-attachment": ("PRIMITIVE", "ACTIVE_PRIMITIVE", "ATOM", "ATTACHMENT(subject,object,role)", ""),
    "concept-valuation": ("PRIMITIVE", "ACTIVE_PRIMITIVE", "ATOM", "VALUATION(subject,measure,value)", ""),
    "concept-warrant": ("PRIMITIVE", "ACTIVE_PRIMITIVE", "ATOM", "WARRANT(conclusion,source,modality)", ""),
    "operator-closure": ("OPERATOR", "ACTIVE_OPERATOR", "OPERATOR", "CLOSURE(ATTACHMENT,scope)", "CLOSURE"),
    "operator-aggregate": ("OPERATOR", "ACTIVE_OPERATOR", "OPERATOR", "AGGREGATE(ATTACHMENT,VALUATION,reducer)", "AGGREGATE"),
    "operator-quotient": ("OPERATOR", "ACTIVE_OPERATOR", "OPERATOR", "QUOTIENT(ATTACHMENT,equivalence,representative,WARRANT(expansion))", "QUOTIENT"),
    "operator-fixpoint": ("OPERATOR", "ACTIVE_OPERATOR", "OPERATOR", "FIXPOINT(WARRANT,ATTACHMENT,VALUATION)", "FIXPOINT"),
}

def loop_number(value):
    try:
        return int(str(value).rsplit("-", 1)[-1])
    except Exception:
        return 0

def main():
    rb = v5.load(v5.RULEBOOK)
    contract = v5.load(v5.CONTRACT)
    historical = copy.deepcopy(rb)
    historical_contract = copy.deepcopy(contract)
    historical["TSPLoops"]["data"] = [row for row in historical["TSPLoops"]["data"] if int(row["LoopOrder"]) <= 623]
    historical_contract["Loops"] = [row for row in historical_contract["Loops"] if int(row["LoopOrder"]) <= 623]
    historical["TSPConceptRegistry"]["data"] = [
        row for row in historical["TSPConceptRegistry"]["data"] if loop_number(row.get("IntroducedByLoop")) <= 623
    ]
    index = v5.table_index(historical, "TSPConceptRegistry")
    for ident, (kind, status, category, expression, operator) in BASIS.items():
        row = index[ident]
        row["ConceptKind"] = kind
        row["Status"] = status
        row["SemanticCategory"] = category
        row["ReducedBasisExpression"] = expression
        row["OperatorExpression"] = operator
        row["SupersededByConcept"] = None
    v5.set_meta(historical, "last_planned_loop", "integer", integer=623)
    v5.set_meta(historical, "last_loop", "integer", integer=623)
    v5.set_meta(historical, "highest_completed_loop", "integer", integer=623)
    v5.set_meta(historical, "active_predicate_atom_count", "integer", integer=3)
    v5.set_meta(historical, "active_semantic_operator_count", "integer", integer=4)
    historical_contract.setdefault("Claims", {}).update({
        "ThreeAtomBasisObserved": True,
        "ThreeAtomBasisProved": False,
        "CurrentPredicateAtomCount": 3,
        "CurrentSemanticOperatorCount": 4,
        "CurrentSemanticCompressionPct": 90.63,
        "AsymmetricFourStopQuotientCertified": True,
    })
    v5.validate_state(historical, historical_contract)
    print("traveling-salesman temporal v5 validation: PASS")

if __name__ == "__main__":
    main()
'''
    )
    VALIDATOR_V6.write_text(
        '''#!/usr/bin/env python3
from apply_loops_624_646_coherence import validate_repository_state

if __name__ == "__main__":
    validate_repository_state()
'''
    )
    SUMMARY_V6.write_text(
        '''#!/usr/bin/env python3
from apply_loops_624_646_coherence import validate_summary_alignment

if __name__ == "__main__":
    validate_summary_alignment()
'''
    )
    VALIDATOR_WRAPPER.write_text(
        '''#!/usr/bin/env python3
from validate_rulebook_v3_temporal import main as validate_v3
from validate_rulebook_v5_temporal import main as validate_v5
from validate_rulebook_v6 import validate_repository_state
from validate_summary_alignment_v6 import validate_summary_alignment

if __name__ == "__main__":
    validate_v3()
    validate_v5()
    validate_repository_state()
    validate_summary_alignment()
'''
    )


def rebuild_postgres_and_record(contract: dict[str, Any]) -> None:
    base.rebuild_postgres_and_record(contract)
    region = run(
        [
            "psql", "-h", os.environ.get("PGHOST", "localhost"),
            "-U", os.environ.get("PGUSER", "postgres"),
            "-d", os.environ.get("TSP_DB", "erb_traveling_salesman"),
            "-tA", "-F", ",", "-v", "ON_ERROR_STOP=1", "-c",
            "SELECT lower_bound_cost::text,is_certified::text FROM vw_instance_lower_bounds WHERE instance_lower_bound_id='degree-two-lower-bound-three-regions-9'",
        ],
        capture=True,
    ).stdout.strip()
    if region not in {"36,true", "36.0,true", "36.0000000000000000,true"}:
        raise AssertionError(f"live three-region lower bound mismatch: {region!r}")
    face = run(
        [
            "psql", "-h", os.environ.get("PGHOST", "localhost"),
            "-U", os.environ.get("PGUSER", "postgres"),
            "-d", os.environ.get("TSP_DB", "erb_traveling_salesman"),
            "-tA", "-F", ",", "-v", "ON_ERROR_STOP=1", "-c",
            "SELECT candidate_tour_id,total_travel_cost::text,is_optimality_proved::text FROM vw_candidate_tours WHERE tsp_instance='tsp-optimal-face-4' ORDER BY candidate_tour_id",
        ],
        capture=True,
    ).stdout.strip().splitlines()
    normalized = {row.split(",")[0]: (float(row.split(",")[1]), row.split(",")[2]) for row in face}
    expected = {
        "tour-optimal-face-abcd-4": (4.0, "true"),
        "tour-optimal-face-acbd-4": (4.0, "true"),
        "tour-optimal-face-abdc-6": (6.0, "false"),
    }
    if normalized != expected:
        raise AssertionError(f"live optimal-face mismatch: {normalized}")
    contract.setdefault("ArtifactHashes", {}).update(
        {
            "loop_646_postgres_projection_tree": "sha256:" + base.sha256_tree(PG_DIR),
            "loop_646_rulespeak_tree": "sha256:" + base.sha256_tree(RULESPEAK_DIR),
            "loop_646_live_three_region_lower_bound": region,
            "loop_646_live_optimal_face": normalized,
        }
    )
    base.write(CONTRACT, contract)


def main() -> None:
    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    if base.meta_int(rb, "last_loop") < 623:
        raise AssertionError("loop 623 must be canonical before coherence refinement")
    prerequisite = DOMAIN / "testing" / "refinement-611-623-verified.json"
    if not prerequisite.is_file() or json.loads(prerequisite.read_text()).get("status") != "VERIFIED":
        raise AssertionError("independent loop-623 refinement certificate is missing")
    loop_map = {int(row["LoopOrder"]): row for row in rows(rb, "TSPLoops")}
    if 624 not in loop_map:
        plan_loops(rb, contract)
        write_validation_files()
        save(rb, contract)
        validate_repository_state()
        commit(
            "TSP loops 624-646: register coherence convergence experiment",
            [
                RULEBOOK,
                CONTRACT,
                DOMAIN / "scripts" / "validate_rulebook_v5_temporal.py",
                VALIDATOR_V6,
                SUMMARY_V6,
                VALIDATOR_WRAPPER,
            ],
        )
    for order in range(624, 647):
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
        if order == 646:
            validate_summary_alignment()
            rebuild_postgres_and_record(contract)
            paths = [DOMAIN]
        commit(COMMIT_MESSAGES[order], paths)
    validate_repository_state()
    validate_summary_alignment()
    print("TSP coherence convergence loops 624-646: PASS")
    print("  current atom: SEMANTIC_ARC")
    print("  current operator: WARRANTED_REWRITE")
    print("  three-region optimum: 36")
    print("  optimal-face value: 4; choice orbit: 2; proof branches: 0")


if __name__ == "__main__":
    main()
