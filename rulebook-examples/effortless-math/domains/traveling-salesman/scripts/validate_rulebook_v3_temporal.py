#!/usr/bin/env python3
"""Run the loop-596 validator against the correct historical semantic projection.

The current rulebook contains conclusions added after loop 596. In particular,
loop 604 legitimately adds the twin-triangle optimality certificate that loop
593 explicitly did not yet have. Shape, relationship, and formula validation
always run against the current canonical rulebook. Only the old semantic check
is replayed against a projection that removes conclusions introduced later.
"""
from __future__ import annotations

import copy

import validate_rulebook_v3 as v3


def main() -> None:
    rb = v3.load(v3.RULEBOOK)
    contract = v3.load(v3.CONTRACT)
    if rb.get("$schema") != "https://example.com/cmcc-schema/v1":
        v3.fail("rulebook schema mismatch")
    if contract.get("Status") != "RESEARCH_PROGRAM":
        v3.fail("contract must remain RESEARCH_PROGRAM")
    if contract.get("Claims", {}).get("PEqualsNP") is not False:
        v3.fail("P=NP non-claim missing")

    # Current-state structural validation is never weakened.
    v3.validate_shapes(rb)
    v3.validate_relationships(rb)
    v3.validate_formulas(rb)

    loop_rows = {int(row["LoopOrder"]): row for row in rb["TSPLoops"]["data"]}
    historical = copy.deepcopy(rb)
    if 604 in loop_rows and v3.completed(loop_rows, 604):
        loop_593 = loop_rows[593]
        history_text = " ".join(
            str(loop_593.get(key) or "")
            for key in ("AfterState", "WitnessSummary")
        ).lower()
        if "no optimality" not in history_text:
            v3.fail("loop 593 historical no-optimality result is missing")

        # Reconstruct the semantic state visible at loop 593. The later
        # certificate remains present in the canonical rulebook and is checked
        # by the loop-604+ validators; it is omitted only for this replay.
        historical["OptimalityCertificates"]["data"] = [
            row
            for row in historical["OptimalityCertificates"]["data"]
            if row.get("CandidateTour") != "tour-twin-triangles-feasible-24"
        ]

    v3.validate_semantics(historical, contract)
    print("traveling-salesman temporal v3 validation: PASS")


if __name__ == "__main__":
    main()
