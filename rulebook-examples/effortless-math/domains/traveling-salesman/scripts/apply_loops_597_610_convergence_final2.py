#!/usr/bin/env python3
"""State-aware final executor for TSP convergence loops 597-610."""
from __future__ import annotations

import importlib
import sys
from typing import Any

import apply_loops_597_610_convergence_v2 as base
import apply_loops_597_610_convergence_final as final


def validate_convergence_state(rb: dict[str, Any], contract: dict[str, Any], reference_model) -> None:
    loops = {int(row["LoopOrder"]): row for row in base.rows(rb, "TSPLoops")}
    planned = base.meta_int(rb, "last_planned_loop")
    if sorted(loops) != list(range(577, planned + 1)):
        raise AssertionError(f"loop sequence is not contiguous through {planned}")
    contract_loops = {int(row["LoopOrder"]): row for row in contract["Loops"]}
    if set(contract_loops) != set(loops):
        raise AssertionError("contract and rulebook loop sets disagree")
    for order in range(597, planned + 1):
        row = loops[order]
        if not row.get("BeforeState") or not row.get("PlannedClosureCriterion"):
            raise AssertionError(f"loop {order}: missing before-state or closure criterion")
        if contract_loops[order]["Status"] != row["Status"]:
            raise AssertionError(f"loop {order}: rulebook/contract status mismatch")
        if row["Status"] == "CLOSED" and not row.get("AfterState"):
            raise AssertionError(f"loop {order}: closed without after-state")

    graph = reference_model.evaluate_graph(rb)
    tours = reference_model.evaluate_tours(rb)
    if not graph["tsp-gridville-5"]["is_complete_undirected_graph"]:
        raise AssertionError("Gridville complete-graph certificate drift")
    grid = tours["tour-reference-ring"]
    if not grid.is_hamiltonian_cycle_witness or not grid.is_optimality_proved or grid.total_travel_cost != 14:
        raise AssertionError(f"Gridville witness drift: {grid}")

    closed = lambda order: loops.get(order, {}).get("Status") == "CLOSED"
    if closed(597):
        measurements = base.rows(rb, "TSPConvergenceMeasurements")
        if not any(row["TSPLoop"] == "tsp-loop-597" for row in measurements):
            raise AssertionError("loop 597 convergence baseline missing")
    if closed(598):
        concepts = base.rows(rb, "TSPConceptRegistry")
        primitives = [row for row in concepts if row["ConceptKind"] == "PRIMITIVE"]
        surfaces = [row for row in concepts if row["TSPConceptId"].startswith("surface-")]
        if len(primitives) != 8 or len(surfaces) != 32:
            raise AssertionError("loop 598 predicate basis mismatch")
        if any(not row["BasisExpression"] for row in surfaces):
            raise AssertionError("surface predicate without basis expression")
    if closed(599):
        fields = {item["name"] for item in rb["TSPEdgeStates"]["schema"]}
        expected = {"CommitmentRank", "CommitmentPolarity", "NecessityScope", "IsTerminalCommitment"}
        if not expected <= fields:
            raise AssertionError("commitment lattice fields missing")
    if closed(600):
        stop_profiles = [row for row in base.rows(rb, "TSPDefectProfiles") if row["SubjectKind"] == "STOP"]
        if len(stop_profiles) != 11:
            raise AssertionError(f"expected 11 incidence budgets, got {len(stop_profiles)}")
        if any(row["RequiredIncidence"] != row["ObservedIncidence"] for row in stop_profiles):
            raise AssertionError("stop incidence budget failed")
    if closed(601):
        profiles = base.table_index(rb, "TSPDefectProfiles")
        g = profiles["defect-vector-gridville-cycle"]
        t = profiles["defect-vector-twin-local-two-factor"]
        if (g["ComponentCount"], g["LowerBoundCost"], g["UpperBoundCost"]) != (1, 14, 14):
            raise AssertionError("Gridville defect vector mismatch")
        expected_twin_lower = 24 if closed(603) else 6
        if (t["ComponentCount"], t["RequiredBoundaryCrossings"], t["LowerBoundCost"], t["UpperBoundCost"]) != (2, 2, expected_twin_lower, 24):
            raise AssertionError("twin defect vector mismatch")
    if closed(602):
        term = base.table_index(rb, "TSPBoundTerms")["bound-term-twin-cut-parity"]
        if term["ConstraintValue"] != 2 or not term["IsCertified"]:
            raise AssertionError("cut parity certificate mismatch")
    if closed(603):
        bounds = reference_model.evaluate_instance_lower_bounds(rb)
        twin_bound = bounds["degree-two-lower-bound-twin-triangles-6"]
        if twin_bound.lower_bound_cost != 24 or not twin_bound.is_certified:
            raise AssertionError(f"component-repair lower bound mismatch: {twin_bound}")
        signed = sum(
            row["Quantity"] * row["UnitWeight"] * row["Sign"]
            for row in base.rows(rb, "TSPBoundTerms")
            if row["BoundCertificate"] == "degree-two-lower-bound-twin-triangles-6"
            and row["CountsTowardAdjustment"]
            and row["IsCertified"]
        )
        if signed != 18:
            raise AssertionError(f"component-repair signed adjustment mismatch: {signed}")
    if closed(604):
        twin = tours["tour-twin-triangles-feasible-24"]
        if not twin.is_hamiltonian_cycle_witness or not twin.is_optimality_proved or twin.total_travel_cost != 24:
            raise AssertionError(f"twin bound-sandwich mismatch: {twin}")
        if "optimality-twin-triangles-component-repair" not in base.table_index(rb, "OptimalityCertificates"):
            raise AssertionError("twin optimality certificate missing")
    if closed(605):
        normals = base.table_index(rb, "TSPWitnessNormalForms")
        supplied = normals["normal-cycle-gridville-supplied"]
        reconstructed = normals["normal-cycle-gridville-reconstructed"]
        fields = ["WitnessShape", "TSPInstance", "CoveredStopCount", "RequiredStopCount", "EdgeCount", "TotalCost", "IncidenceDefect", "ConnectivityDefect", "OrderDefect", "BoundarySignature"]
        if any(supplied[name] != reconstructed[name] for name in fields):
            raise AssertionError("supplied/reconstructed Gridville normal forms disagree")
        if supplied["OriginKind"] == reconstructed["OriginKind"]:
            raise AssertionError("witness provenance distinction disappeared")
    if closed(606):
        paths = [row for row in base.rows(rb, "TSPWitnessNormalForms") if row["WitnessShape"] == "PATH"]
        if len(paths) != 6 or any(row["EdgeCount"] != 2 or row["TotalCost"] != 2 for row in paths):
            raise AssertionError("boundary-signature normal forms mismatch")
    if closed(607):
        counts = sorted(row["QuotientClassCount"] for row in base.rows(rb, "TSPClusterContractionCertificates"))
        if counts != [3, 3, 9]:
            raise AssertionError(f"semantic quotient counts mismatch: {counts}")
    if closed(608):
        quotient_nodes = [row for row in base.rows(rb, "Neighborhoods") if row.get("IsQuotientNode")]
        if len(quotient_nodes) != 2 or any(row["RequiredBoundaryDegree"] != 2 for row in quotient_nodes):
            raise AssertionError("component quotient mismatch")
    if closed(609):
        events = [row for row in base.rows(rb, "TSPInferenceApplications") if row.get("EventKind")]
        if len(events) < 14:
            raise AssertionError(f"closure-event count too small: {len(events)}")
        if any(row.get("AntecedentCount") is None or row.get("DecisionCount") is None for row in events):
            raise AssertionError("closure event lacks antecedent/decision accounting")
    if closed(610):
        tables = base.canonical_tables(rb)
        if len(tables) != 45:
            raise AssertionError(f"expected 45 physical tables, got {len(tables)}")
        concepts = base.rows(rb, "TSPConceptRegistry")
        primitives = sum(row["ConceptKind"] == "PRIMITIVE" for row in concepts)
        coined = sum(row["ConceptKind"] == "COINED_PREDICATE" for row in concepts)
        if primitives != 8 or coined != 14:
            raise AssertionError(f"convergence counts mismatch: primitives={primitives}, coined={coined}")
        final_measurement = base.table_index(rb, "TSPConvergenceMeasurements")["convergence-loop-610"]
        if final_measurement["PrimitiveCountAfter"] != 8 or final_measurement["NewPrimitiveCount"] != 0:
            raise AssertionError("final basis-stability measurement mismatch")
        claims = contract["Claims"]
        if claims["SemanticCompressionPct"] != 75 or claims["ConceptConvergenceProved"] is not False:
            raise AssertionError("convergence epistemic status mismatch")


def validate_repository_state() -> None:
    sys.path.insert(0, str(base.DOMAIN / "scripts"))
    importlib.invalidate_caches()
    import reference_model
    importlib.reload(reference_model)
    rb = base.load(base.RULEBOOK)
    contract = base.load(base.CONTRACT)
    final.validate_core(rb)
    validate_convergence_state(rb, contract, reference_model)
    if contract["Acceptance"]["RulebookTables"] != len(base.canonical_tables(rb)):
        raise AssertionError("contract table count drift")
    print("traveling-salesman state-aware convergence validation: PASS")


def write_validation_files() -> None:
    final.write_validation_files()
    base.VALIDATOR_V4.write_text(
        '''#!/usr/bin/env python3\nfrom apply_loops_597_610_convergence_final2 import validate_repository_state\n\nif __name__ == "__main__":\n    validate_repository_state()\n'''
    )
    workflow = base.VALIDATION_WORKFLOW.read_text()
    if "apply_loops_597_610_convergence_final2.py" not in workflow:
        workflow = workflow.replace(
            '"$domain/scripts/apply_loops_597_610_convergence_final.py" \\\n',
            '"$domain/scripts/apply_loops_597_610_convergence_final.py" \\\n            "$domain/scripts/apply_loops_597_610_convergence_final2.py" \\\n',
        )
    base.VALIDATION_WORKFLOW.write_text(workflow)


base.patch_reference_model_for_terms = final.append_reference_override
base.LOOP_FUNCS[603] = final.loop_603_final
base.LOOP_FUNCS[607] = final.loop_607_final
base.LOOP_FUNCS[609] = final.loop_609_final
base.LOOP_FUNCS[610] = final.loop_610_final
base.validate_repository_state = validate_repository_state
base.write_validation_files = write_validation_files


if __name__ == "__main__":
    base.main()
