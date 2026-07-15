#!/usr/bin/env python3
"""Add a spread of computations beyond the four flagship, so the engine is
exercised across the operand space (not just 2+2 etc.). Keeps the four flagship
rows (is_flagship=1) and adds a representative spread (is_flagship=0).

Result-width per op matches the netlist output pins:
  add/sub -> W bits ; mul -> 2W bits ; div -> W bits (quotient).
The expected_* fields are the ANSWER KEY. They are computed HERE, in the authoring
tool, purely to seed the invariant target -- the engine never sees them; it settles
from gates and the view compares. (Same role as expected values in any test.)
"""
import json

W = 4
BASE = "../effortless-rulebook/bit-calculator-rulebook.json"

# top component per op (the fully-driven wrapper for add)
TOP = {"add": "comp-add4", "sub": "comp-sub4", "mul": "comp-mul4", "div": "comp-div4"}
RESW = {"add": W, "sub": W, "mul": 2 * W, "div": W}


def bits(v, w):
    return format(v & ((1 << w) - 1), f"0{w}b")


def expected(op, a, b):
    # The view reads the W-bit result bus (s/q) for add/sub/div and the 2W-bit
    # product bus (p) for mul. add spread pairs are kept within 0..2^W-1 so the
    # W-bit sum bus holds the full answer (cout=0); overflow is exercised only by
    # mul, whose bus is wide enough.
    if op == "add":
        return bits(a + b, W), a + b
    if op == "sub":
        return bits((a - b) & ((1 << W) - 1), W), (a - b) & ((1 << W) - 1)
    if op == "mul":
        return bits(a * b, 2 * W), a * b
    if op == "div":
        return bits(a // b, W), a // b


# spread: pairs per op exercising carries/borrows. add pairs sum to <= 15 so the
# W-bit sum bus holds the answer; mul exercises the wide product bus.
PAIRS = {
    "add": [(1, 1), (3, 5), (7, 8), (6, 9), (10, 5)],
    "sub": [(5, 3), (9, 4), (8, 8), (2, 1), (15, 7)],
    "mul": [(3, 3), (2, 5), (4, 4), (7, 2), (6, 2)],
    "div": [(9, 3), (7, 2), (8, 4), (15, 5), (6, 6)],
}

rb = json.load(open(BASE))
existing = {r["computation_id"] for r in rb["Computations"]["data"]}
added = 0
for op, pairs in PAIRS.items():
    for a, b in pairs:
        cid = f"compute--{op}--{a}--{b}"
        if cid in existing:
            continue
        eb, ev = expected(op, a, b)
        rb["Computations"]["data"].append({
            "computation_id": cid, "op": op, "component_id": TOP[op],
            "a_value": a, "b_value": b, "a_bits": bits(a, W), "b_bits": bits(b, W),
            "expected_bits": eb, "expected_value": ev, "is_flagship": 0,
        })
        existing.add(cid)
        added += 1

json.dump(rb, open(BASE, "w"), indent=1)
print(f"added {added} spread computations (total {len(rb['Computations']['data'])})")
