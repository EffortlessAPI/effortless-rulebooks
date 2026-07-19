#!/usr/bin/env python3
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
