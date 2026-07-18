#!/usr/bin/env python3
"""Independently verify the canonical TSP convergence state through loop 610."""
from __future__ import annotations

import json
import subprocess
from pathlib import Path

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CONTRACT = DOMAIN / "problem-contract.json"
STATUS = DOMAIN / "testing" / "convergence-597-610-status.json"
VERIFIED = DOMAIN / "testing" / "convergence-597-610-verified.json"

EXPECTED_COMMITS = [
    "TSP loops 597-610: register convergence experiment",
    "TSP loop 597: record convergence prediction",
    "TSP loop 598: define predicate basis",
    "TSP loop 599: unify commitment lattice",
    "TSP loop 600: unify incidence budget",
    "TSP loop 601: define defect vector",
    "TSP loop 602: certify cut parity",
    "TSP loop 603: derive component repair bound",
    "TSP loop 604: close twin bound sandwich",
    "TSP loop 605: normalize witness forms",
    "TSP loop 606: define boundary signatures",
    "TSP loop 607: certify semantic quotient",
    "TSP loop 608: unify component quotient",
    "TSP loop 609: normalize closure events",
    "TSP loop 610: record convergence event",
]


def load(path: Path) -> dict:
    if not path.is_file():
        raise FileNotFoundError(path)
    return json.loads(path.read_text())


def term_value(row: dict) -> float:
    return float(row["Quantity"]) * float(row["UnitWeight"]) * int(row["Sign"])


def main() -> None:
    status = load(STATUS)
    if status.get("status") != "SUCCEEDED":
        raise AssertionError(f"executor status is {status.get('status')!r}")

    rb = load(RULEBOOK)
    contract = load(CONTRACT)
    loops = {int(row["LoopOrder"]): row for row in rb["TSPLoops"]["data"]}
    if sorted(loops) != list(range(577, 611)):
        raise AssertionError(
            f"loop sequence mismatch: {min(loops)}..{max(loops)}, count={len(loops)}"
        )
    for order in range(597, 611):
        row = loops[order]
        if row["Status"] != "CLOSED":
            raise AssertionError(f"loop {order} is {row['Status']!r}, not CLOSED")
        if not row.get("BeforeState") or not row.get("AfterState"):
            raise AssertionError(f"loop {order} lacks before/after history")

    concepts = rb["TSPConceptRegistry"]["data"]
    primitive_count = sum(row["ConceptKind"] == "PRIMITIVE" for row in concepts)
    coined_count = sum(row["ConceptKind"] == "COINED_PREDICATE" for row in concepts)
    if primitive_count != 8:
        raise AssertionError(f"primitive basis count is {primitive_count}, expected 8")
    if coined_count != 14:
        raise AssertionError(f"coined predicate count is {coined_count}, expected 14")

    claims = contract["Claims"]
    if claims.get("SemanticCompressionPct") != 75:
        raise AssertionError(f"semantic compression is {claims.get('SemanticCompressionPct')!r}")
    if claims.get("TwinTrianglesOptimalityProved") is not True:
        raise AssertionError("twin-triangles finite optimality certificate is missing")
    if claims.get("ConceptConvergenceProved") is not False:
        raise AssertionError("empirical convergence was promoted to a theorem")

    twin_bound = next(
        row
        for row in rb["InstanceLowerBounds"]["data"]
        if row["InstanceLowerBoundId"] == "degree-two-lower-bound-twin-triangles-6"
    )
    twin_terms = {
        row["TermKind"]: row
        for row in rb["TSPBoundTerms"]["data"]
        if row["BoundCertificate"] == twin_bound["InstanceLowerBoundId"]
    }
    base = term_value(twin_terms["BASE_DEGREE_TWO_BOUND"])
    insertion = term_value(twin_terms["MANDATORY_CROSSING_INSERTION"])
    release = term_value(twin_terms["NECESSARY_INTERNAL_EDGE_RELEASE"])
    supplemental = float(twin_bound["SupplementalBoundAdjustment"])
    final_bound = base + supplemental
    if (base, insertion, release, supplemental, final_bound) != (6.0, 20.0, -2.0, 18.0, 24.0):
        raise AssertionError(
            "twin repair composition mismatch: "
            f"base={base}, insertion={insertion}, release={release}, "
            f"supplemental={supplemental}, final={final_bound}"
        )
    if insertion + release != supplemental:
        raise AssertionError("supplemental adjustment does not equal certified repair terms")
    if any(not row["IsCertified"] for row in twin_terms.values()):
        raise AssertionError("one or more twin repair terms are uncertified")

    optimality_rows = {
        row["OptimalityCertificateId"]: row
        for row in rb["OptimalityCertificates"]["data"]
    }
    if "optimality-twin-triangles-component-repair" not in optimality_rows:
        raise AssertionError("twin optimality-certificate row is missing")

    subjects = subprocess.check_output(
        ["git", "log", "--format=%s", "--reverse"], text=True
    ).splitlines()
    positions: list[int] = []
    for message in EXPECTED_COMMITS:
        if message not in subjects:
            raise AssertionError(f"missing convergence commit: {message}")
        positions.append(subjects.index(message))
    if positions != sorted(positions) or len(set(positions)) != len(positions):
        raise AssertionError(f"convergence commits are not strictly ordered: {positions}")

    payload = {
        "status": "VERIFIED",
        "github_run_id": None,
        "loops": "597-610",
        "primitive_basis_count": primitive_count,
        "surface_predicate_baseline": 32,
        "semantic_compression_pct": 75,
        "coined_predicates": coined_count,
        "twin_optimality": True,
        "twin_bound_composition": {
            "base": base,
            "crossing_insertion": insertion,
            "internal_release": release,
            "supplemental_adjustment": supplemental,
            "final": final_bound,
        },
        "concept_convergence_proved": False,
        "ordered_commit_positions": positions,
    }
    VERIFIED.write_text(json.dumps(payload, indent=2) + "\n")
    print("independent TSP convergence verification: PASS")
    print(f"primitive_basis={primitive_count} coined_predicates={coined_count}")
    print(f"twin_bound={base}+{supplemental}={final_bound}")


if __name__ == "__main__":
    main()
