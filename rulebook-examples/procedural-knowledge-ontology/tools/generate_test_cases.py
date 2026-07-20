#!/usr/bin/env python3
"""Generate the TestCases rows from the model itself.

Every check corresponds to something that actually exists — a formula, a view,
an FK, a witness, a question. Deriving the catalog rather than hand-writing it
means the suite cannot silently stop covering a field somebody added, and the
count of checks is a fact about the model rather than a claim about diligence.

Outcomes are NOT written here; tools/run_test_suite.py executes the rows and
records what the substrate said.

Usage: tools/generate_test_cases.py
"""
from __future__ import annotations

import json
import re
from collections import OrderedDict
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
IRI = "urn:effortless:pko-extension#TestCase"

META_TABLES = {"WitnessLoops", "RoleQuestions", "RulebookFields", "TestCases",
               "TestSuites", "__meta__", "ERBVersions", "ERBCustomizations"}


def snake(name: str) -> str:
    return re.sub(r"(?<!^)(?=[A-Z])", "_", name).lower()


def case(tid, kind, subject, assertion, suite, severity,
         table=None, field=None, question=None):
    return OrderedDict([
        ("TestCaseId", tid),
        ("TestKind", kind),
        ("Subject", subject),
        ("TargetTable", table),
        ("TargetField", field),
        ("Assertion", assertion),
        ("DefendsQuestion", question),
        ("Suite", suite),
        ("Severity", severity),
        ("LastOutcome", None),
        ("LastDetail", None),
        ("LastRunAt", None),
        ("SemanticTypeIri", IRI),
    ])


def main() -> int:
    with RB.open() as fh:
        rb = json.load(fh, object_pairs_hook=OrderedDict)

    tables = [k for k, v in rb.items() if isinstance(v, dict) and "schema" in v]
    catalog = rb["RulebookFields"]["data"]
    by_field = {r["RulebookFieldId"]: r for r in catalog}
    cases: list[OrderedDict] = []

    # --- structure: every table obeys the repo's shape conventions ----------
    for t in tables:
        cases.append(case(
            f"tc-struct-{snake(t)}", "structural", t,
            "Primary key first and non-nullable, and a calculated Name display alias exists.",
            "suite-structure", "blocking", table=t))

    # --- substrate: every formula must translate to a real function ---------
    # A formula the transpiler cannot translate does not fail the build. It
    # returns NULL and the column reads as a clean false forever.
    for fid, row in by_field.items():
        if not row.get("Formula"):
            continue
        cases.append(case(
            f"tc-xlate-{snake(row['TargetTable'])}-{snake(row['FieldName'])}",
            "formula-translates", fid,
            "The transpiler emitted a real SQL function for this formula rather than a NULL stub.",
            "suite-substrate", "blocking",
            table=row["TargetTable"], field=row["FieldName"],
            question=row.get("InventedForQuestion")))

    # --- substrate: every table's view loads and is queryable ---------------
    for t in tables:
        if t in {"__meta__"}:
            continue
        cases.append(case(
            f"tc-view-{snake(t)}", "view-loads", f"vw_{snake(t)}",
            "The view exists and answers a SELECT without error.",
            "suite-substrate", "blocking", table=t))

    # --- integrity: every FK value names a real row -------------------------
    for t in tables:
        for f in rb[t]["schema"]:
            if not isinstance(f, dict) or f.get("type") != "relationship":
                continue
            target = f.get("RelatedTo")
            if not target or target not in rb:
                continue
            cases.append(case(
                f"tc-fk-{snake(t)}-{snake(f['name'])}", "fk-resolves",
                f"{t}.{f['name']} -> {target}",
                f"Every non-null value resolves to an existing {target} row.",
                "suite-integrity", "blocking", table=t, field=f["name"]))

    # --- witness behaviour --------------------------------------------------
    # Vacuity is ADVISORY: a witness that cannot fire on this seed is not
    # necessarily wrong, and scoring it red would create pressure to fabricate
    # data until it went green.
    for fid, row in by_field.items():
        if row.get("Datatype") != "boolean" or not row.get("Formula"):
            continue
        if row["TargetTable"] in META_TABLES:
            continue
        cases.append(case(
            f"tc-witness-{snake(row['TargetTable'])}-{snake(row['FieldName'])}",
            "witness-discriminates", fid,
            "Reads both TRUE and FALSE across the seed data, so it can distinguish cases.",
            "suite-witness", "advisory",
            table=row["TargetTable"], field=row["FieldName"],
            question=row.get("InventedForQuestion")))

    # --- provenance ---------------------------------------------------------
    for q in rb["RoleQuestions"]["data"]:
        cases.append(case(
            f"tc-answered-{q['RoleQuestionId']}", "question-answered",
            q["RoleQuestionId"],
            "At least one predicate exists to answer this question.",
            "suite-provenance", "blocking", question=q["RoleQuestionId"]))

    cases.append(case(
        "tc-provenance-resolves", "provenance", "RulebookFields.InventedForQuestion",
        "Every invented field points at a role question that exists.",
        "suite-provenance", "blocking"))
    cases.append(case(
        "tc-catalog-sync", "catalog-sync", "RulebookFields",
        "The catalog matches the real schemas in both directions — nothing missing, nothing stale.",
        "suite-provenance", "blocking"))
    cases.append(case(
        "tc-no-delimiter-collision", "invariant", "composite keys",
        "No identifier contains the '|' delimiter that composite join keys rely on.",
        "suite-invariant", "blocking"))

    # --- domain invariants: things the procedures themselves require --------
    for tid, subject, assertion, table, field in [
        ("tc-inv-spec-exec-separate", "Procedures vs ProcedureExecutions",
         "Specifications and executions remain separate tables linked by hasExecutedProcedure.",
         "ProcedureExecutions", None),
        ("tc-inv-blocking-unmet-visible", "RequirementSatisfactions.IsBlockingAndUnmet",
         "A blocking requirement recorded below Satisfied is visible as a witness, not merely as text.",
         "RequirementSatisfactions", "IsBlockingAndUnmet"),
        ("tc-inv-sod-computable", "StepExecutions.ViolatesSeparationOfDuties",
         "Segregation of duties is computed from execution data rather than asserted in prose.",
         "StepExecutions", "ViolatesSeparationOfDuties"),
        ("tc-inv-consent-computable", "MessageDeliveries.IsConsentViolation",
         "A send to a non-consenting recipient is distinguishable from a correctly suppressed one.",
         "MessageDeliveries", "IsConsentViolation"),
        ("tc-inv-asof-not-wallclock", "time-dependent witnesses",
         "No time-dependent witness compares against NOW(); all read the modeled evaluation instant.",
         None, None),
        ("tc-inv-remediation-preserved", "KnowledgeGaps",
         "Seeded violations were remediated in model rather than deleted — resolved gaps remain as evidence.",
         "KnowledgeGaps", None),
    ]:
        cases.append(case(tid, "invariant", subject, assertion,
                          "suite-invariant", "blocking", table=table, field=field))

    rb["TestCases"]["data"] = cases

    with RB.open("w") as fh:
        json.dump(rb, fh, indent=2)
        fh.write("\n")

    kinds: dict[str, int] = {}
    for c in cases:
        kinds[c["TestKind"]] = kinds.get(c["TestKind"], 0) + 1
    print(f"generated {len(cases)} test cases")
    for k, n in sorted(kinds.items(), key=lambda kv: -kv[1]):
        print(f"  {n:5}  {k}")
    blocking = sum(1 for c in cases if c["Severity"] == "blocking")
    print(f"  {blocking} blocking / {len(cases) - blocking} advisory")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
