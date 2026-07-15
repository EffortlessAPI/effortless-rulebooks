#!/usr/bin/env python3
"""Exhaustively validate the generated netlists against real arithmetic.

Loads the base rulebook (gates + half/full adder), splices in generated_netlists.json,
and runs netlist_sim over every 4-bit input pair for each op. The ONLY place real
arithmetic appears is the expected-value comparison in THIS harness — the simulator
and the rulebook never contain it.
"""
import json
import copy
from netlist_sim import Netlist

W = 4
BASE = "../effortless-rulebook/bit-calculator-rulebook.json"
GEN = "generated_netlists.json"


def bits_lsb(value, width):
    return {i: (value >> i) & 1 for i in range(width)}


def merged_netlist():
    with open(BASE) as f:
        rb = json.load(f)
    with open(GEN) as f:
        gen = json.load(f)
    rb = copy.deepcopy(rb)
    for tbl in ("Components", "ComponentPins", "ComponentInstances", "ComponentConnections"):
        existing_ids = {r[list(r)[0]] for r in rb[tbl]["data"]}
        for row in gen[tbl]:
            if row[list(row)[0]] not in existing_ids:
                rb[tbl]["data"].append(row)
    # write a temp merged rulebook the simulator can read
    tmp = "_merged_for_validation.json"
    with open(tmp, "w") as f:
        json.dump(rb, f)
    return Netlist(tmp)


def inputs_ab(a, b, width):
    d = {}
    for i, bit in bits_lsb(a, width).items():
        d[f"a{i}"] = bit
    for i, bit in bits_lsb(b, width).items():
        d[f"b{i}"] = bit
    return d


def read_bus(out, prefix, width, extra=None):
    """Assemble a decimal value from output pins prefix0..prefix{width-1} (+ optional MSB)."""
    v = 0
    for i in range(width):
        v |= out[f"{prefix}{i}"] << i
    if extra is not None and extra in out:
        v |= out[extra] << width
    return v


def validate_adder(nl, cid=f"comp-adder{W}"):
    fails = 0
    for a in range(2 ** W):
        for b in range(2 ** W):
            d = inputs_ab(a, b, W)
            d["cin"] = 0
            out = nl.evaluate(cid, d)
            got = read_bus(out, "s", W, extra="cout")
            if got != a + b:
                fails += 1
                if fails <= 5:
                    print(f"  ADDER FAIL {a}+{b}: got {got} exp {a+b} :: {out}")
    print(f"adder{W}: {'ALL 256 PASS' if fails == 0 else f'{fails} FAILURES'}")
    return fails == 0


def validate_add_wrapper(nl, cid=f"comp-add{W}"):
    """The op-wrapper: driven by a/b ALONE (no cin) -- exactly what the engine feeds."""
    fails = 0
    for a in range(2 ** W):
        for b in range(2 ** W):
            out = nl.evaluate(cid, inputs_ab(a, b, W))   # NO cin supplied
            got = read_bus(out, "s", W, extra="cout")
            if got != a + b:
                fails += 1
                if fails <= 5:
                    print(f"  ADD-WRAP FAIL {a}+{b}: got {got} exp {a+b}")
    print(f"add{W} (a/b only): {'ALL 256 PASS' if fails == 0 else f'{fails} FAILURES'}")
    return fails == 0


def validate_sub(nl, cid=f"comp-sub{W}"):
    fails = 0
    for a in range(2 ** W):
        for b in range(2 ** W):
            out = nl.evaluate(cid, inputs_ab(a, b, W))
            got = read_bus(out, "s", W)              # low W bits = two's-complement result
            exp = (a - b) & ((1 << W) - 1)           # mod 2^W
            exp_borrow = 1 if a < b else 0
            if got != exp or out["borrow"] != exp_borrow:
                fails += 1
                if fails <= 5:
                    print(f"  SUB FAIL {a}-{b}: got {got} borrow {out['borrow']} "
                          f"exp {exp} borrow {exp_borrow}")
    print(f"sub{W}: {'ALL 256 PASS' if fails == 0 else f'{fails} FAILURES'}")
    return fails == 0


def validate_mul(nl, cid=f"comp-mul{W}"):
    fails = 0
    for a in range(2 ** W):
        for b in range(2 ** W):
            out = nl.evaluate(cid, inputs_ab(a, b, W))
            got = 0
            for k in range(2 * W):
                got |= out[f"p{k}"] << k
            if got != a * b:
                fails += 1
                if fails <= 8:
                    print(f"  MUL FAIL {a}*{b}: got {got} exp {a*b}")
    print(f"mul{W}: {'ALL 256 PASS' if fails == 0 else f'{fails} FAILURES'}")
    return fails == 0


def validate_div(nl, cid=f"comp-div{W}"):
    fails = 0
    for a in range(2 ** W):
        for b in range(1, 2 ** W):          # skip divide-by-zero
            out = nl.evaluate(cid, inputs_ab(a, b, W))
            q = read_bus(out, "q", W)
            r = read_bus(out, "r", W)
            if q != a // b or r != a % b:
                fails += 1
                if fails <= 8:
                    print(f"  DIV FAIL {a}/{b}: got q={q} r={r} exp q={a//b} r={a%b}")
    print(f"div{W}: {'ALL 240 PASS' if fails == 0 else f'{fails} FAILURES'}")
    return fails == 0


if __name__ == "__main__":
    nl = merged_netlist()
    present = {c["component_id"] for c in json.load(open(GEN))["Components"]}
    ok = True
    if f"comp-adder{W}" in present:
        ok &= validate_adder(nl)
    if f"comp-add{W}" in present:
        ok &= validate_add_wrapper(nl)
    if f"comp-sub{W}" in present:
        ok &= validate_sub(nl)
    if f"comp-mul{W}" in present:
        ok &= validate_mul(nl)
    if f"comp-div{W}" in present:
        ok &= validate_div(nl)
    print("RESULT:", "OK" if ok else "FAIL")
