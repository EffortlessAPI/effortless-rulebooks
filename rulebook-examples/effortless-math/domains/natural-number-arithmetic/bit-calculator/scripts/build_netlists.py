#!/usr/bin/env python3
"""Emit the top-level component netlists (adder4, sub4, mul4, div4) as rulebook rows.

Every row this produces is plain DATA that goes into the rulebook. This script is a
one-time authoring aid (like a schematic capture tool) — it is NOT part of the runtime
engine and contains no arithmetic that the circuit relies on. The circuits it wires are
validated exhaustively by netlist_sim.py, which DOES only follow the data.

Bit convention: bit strings are MSB-first (index 0 = most significant). Internally we
work LSB-first (bit position 0 = least significant, the ripple-carry start).

Run: python3 build_netlists.py   # writes generated_netlists.json (rows to splice in)
"""
import json

W = 4  # operand bit width

components = []   # Components rows
pins = []         # ComponentPins rows
instances = []    # ComponentInstances rows
connections = []  # ComponentConnections rows


def comp(cid, desc, bit_width=W):
    components.append({"component_id": cid, "kind": "circuit", "bit_width": bit_width,
                       "description": desc})


def pin(cid, direction, name, bit_index=None, desc=None):
    pins.append({"pin_id": f"{cid}--{direction}--{name}", "component_id": cid,
                 "direction": direction, "pin_name": name, "bit_index": bit_index,
                 "description": desc or name})


def gate_inst(cid, name, gate_type_id):
    instances.append({"instance_id": f"{cid}--inst--{name}", "component_id": cid,
                      "instance_name": name, "instance_kind": "gate",
                      "gate_type_id": gate_type_id, "sub_component_id": None})


def sub_inst(cid, name, sub_component_id):
    instances.append({"instance_id": f"{cid}--inst--{name}", "component_id": cid,
                      "instance_name": name, "instance_kind": "component",
                      "gate_type_id": None, "sub_component_id": sub_component_id})


def conn(cid, fn, fp, tn, tp):
    fnl = fn if fn else ""
    tnl = tn if tn else ""
    ftag = f"{fnl}.{fp}" if fnl else fp
    ttag = f"{tnl}.{tp}" if tnl else tp
    connections.append({"connection_id": f"{cid}--conn--{ftag}--to--{ttag}",
                        "component_id": cid, "from_node": fnl, "from_port": fp,
                        "to_node": tnl, "to_port": tp})


# ---------------------------------------------------------------------------
# adder4 : W-bit ripple-carry adder from W full_adders.
#   inputs  : a0..a{W-1}, b0..b{W-1}, cin      (bit i = value 2^i)
#   outputs : s0..s{W-1}, cout
# ---------------------------------------------------------------------------
def build_adder(cid=f"comp-adder{W}", width=W, desc=None):
    comp(cid, desc or f"{width}-bit ripple-carry adder from {width} full adders.", width)
    for i in range(width):
        pin(cid, "in", f"a{i}", i, f"Operand A bit {i}")
        pin(cid, "in", f"b{i}", i, f"Operand B bit {i}")
        pin(cid, "out", f"s{i}", i, f"Sum bit {i}")
    pin(cid, "in", "cin", None, "Carry-in")
    pin(cid, "out", "cout", None, "Carry-out")
    for i in range(width):
        sub_inst(cid, f"fa{i}", "comp-full-adder")
    for i in range(width):
        conn(cid, "", f"a{i}", f"fa{i}", "a")
        conn(cid, "", f"b{i}", f"fa{i}", "b")
        if i == 0:
            conn(cid, "", "cin", "fa0", "cin")
        else:
            conn(cid, f"fa{i-1}", "carry", f"fa{i}", "cin")
        conn(cid, f"fa{i}", "sum", "", f"s{i}")
    conn(cid, f"fa{width-1}", "carry", "", "cout")
    return cid


build_adder()


# ---------------------------------------------------------------------------
# add4 : op-wrapper around adder4 that ties cin low, so the computation's top
# component is fully driven by a/b alone (the Chinese-room engine supplies only
# a/b bits). adder4 itself keeps its cin pin because the divider reuses it.
#   inputs  : a0..a{W-1}, b0..b{W-1}   outputs : s0..s{W-1}, cout
# ---------------------------------------------------------------------------
def build_add_wrapper(cid=f"comp-add{W}", width=W):
    comp(cid, f"{width}-bit adder op (adder{width} with carry-in tied to 0).", width)
    for i in range(width):
        pin(cid, "in", f"a{i}", i, f"Operand A bit {i}")
        pin(cid, "in", f"b{i}", i, f"Operand B bit {i}")
        pin(cid, "out", f"s{i}", i, f"Sum bit {i}")
    pin(cid, "out", "cout", None, "Carry-out")
    sub_inst(cid, "add", f"comp-adder{width}")
    for i in range(width):
        conn(cid, "", f"a{i}", "add", f"a{i}")
        conn(cid, "", f"b{i}", "add", f"b{i}")
        conn(cid, "add", f"s{i}", "", f"s{i}")
    conn(cid, "const", "0", "add", "cin")
    conn(cid, "add", "cout", "", "cout")
    return cid


build_add_wrapper()


# ---------------------------------------------------------------------------
# sub4 : two's-complement subtractor.  A - B = A + (~B) + 1.
#   NOT each b bit, feed a & ~b into an adder4 with cin=1.
#   inputs  : a0..a{W-1}, b0..b{W-1}
#   outputs : s0..s{W-1}, borrow  (borrow = NOT cout ; 1 means A < B)
# ---------------------------------------------------------------------------
def build_sub(cid=f"comp-sub{W}", width=W):
    comp(cid, f"{width}-bit two's-complement subtractor (adder{width} + NOT on B, cin=1).", width)
    for i in range(width):
        pin(cid, "in", f"a{i}", i, f"Operand A bit {i}")
        pin(cid, "in", f"b{i}", i, f"Operand B bit {i}")
        pin(cid, "out", f"s{i}", i, f"Difference bit {i}")
    pin(cid, "out", "borrow", None, "Borrow-out (1 => A < B)")
    # NOT gates on each b bit
    for i in range(width):
        gate_inst(cid, f"notb{i}", "gate-not")
    sub_inst(cid, "add", f"comp-adder{width}")
    gate_inst(cid, "borrow_not", "gate-not")   # borrow = NOT cout
    for i in range(width):
        conn(cid, "", f"b{i}", f"notb{i}", "in")
        conn(cid, f"notb{i}", "out", "add", f"b{i}")
        conn(cid, "", f"a{i}", "add", f"a{i}")
        conn(cid, "add", f"s{i}", "", f"s{i}")
    conn(cid, "const", "1", "add", "cin")          # +1 for two's complement
    conn(cid, "add", "cout", "borrow_not", "in")
    conn(cid, "borrow_not", "out", "", "borrow")
    return cid


build_sub()


# ---------------------------------------------------------------------------
# mul4 : W x W unsigned array multiplier -> 2W-bit product (shift-and-add).
#   partial product bit pp_i_j = a_j AND b_i  (row i shifted left by i)
#   Sum rows with adder{W} instances, carrying between stages.
#   inputs  : a0..a{W-1}, b0..b{W-1}
#   outputs : p0..p{2W-1}
# Structure (classic ripple array):
#   acc starts as row 0 (a AND b0) in bits [0..W-1], high bits 0.
#   For i in 1..W-1: add (a AND b_i) shifted to align at bit i.
# We implement it as: p0 = pp_0_0. Then a running (W+1)-bit accumulator that
# absorbs one partial-product row per stage, emitting one product bit each stage.
# ---------------------------------------------------------------------------
def build_mul(cid=f"comp-mul{W}", width=W):
    comp(cid, f"{width}x{width} unsigned array multiplier ({width} shift-and-add rows).",
         width, )
    # override bit_width note: product is 2*width bits
    for i in range(width):
        pin(cid, "in", f"a{i}", i, f"Operand A bit {i}")
        pin(cid, "in", f"b{i}", i, f"Operand B bit {i}")
    for k in range(2 * width):
        pin(cid, "out", f"p{k}", k, f"Product bit {k}")

    # AND gates for every partial product pp_i_j = a_j AND b_i
    for i in range(width):
        for j in range(width):
            gate_inst(cid, f"pp_{i}_{j}", "gate-and")
            conn(cid, "", f"a{j}", f"pp_{i}_{j}", "left")
            conn(cid, "", f"b{i}", f"pp_{i}_{j}", "right")

    # Row 0 partial product occupies bits 0..width-1 of the accumulator.
    # p0 is pp_0_0 directly.
    conn(cid, "pp_0_0", "out", "", "p0")

    # acc[k] after absorbing row r: we keep a symbolic map of the current
    # accumulator high bits (positions 1..) as (node, port) references.
    # Start: acc bits at positions 1..width-1 come from pp_0_1..pp_0_{width-1},
    # positions width.. are 0 (const 0).
    acc = {}          # position -> (node, port) currently holding that bit
    for j in range(1, width):
        acc[j] = ("pp_0_" + str(j), "out")
    top = width - 1   # highest currently-meaningful position

    for i in range(1, width):
        # Add partial-product row i (pp_i_0..pp_i_{width-1}) aligned so pp_i_j sits
        # at position i+j. Emit product bit p{i} = acc[i] + pp_i_0 (+carry chain).
        # Use one adder{width} instance per row over positions i..i+width-1.
        add_name = f"addrow{i}"
        sub_inst(cid, add_name, f"comp-adder{width}")
        # adder A input bits = current acc at positions i..i+width-1 (0 if absent)
        # adder B input bits = pp_i_0..pp_i_{width-1}
        for j in range(width):
            pos = i + j
            # A input a{j}: current acc bit at pos, or const 0
            if pos in acc:
                fn, fp = acc[pos]
                conn(cid, fn, fp, add_name, f"a{j}")
            else:
                conn(cid, "const", "0", add_name, f"a{j}")
            # B input b{j}: partial product pp_i_j
            conn(cid, f"pp_{i}_{j}", "out", add_name, f"b{j}")
        conn(cid, "const", "0", add_name, "cin")
        # adder sum bit 0 becomes product bit p{i}
        conn(cid, add_name, "s0", "", f"p{i}")
        # remaining sum bits s1..s{width-1} and cout become the new acc high bits
        newacc = {}
        for j in range(1, width):
            newacc[i + j] = (add_name, f"s{j}")
        newacc[i + width] = (add_name, "cout")
        acc = newacc
        top = i + width

    # After the last row, drain remaining acc bits to product outputs p{width}..p{2W-1}
    for k in range(width, 2 * width):
        if k in acc:
            fn, fp = acc[k]
            conn(cid, fn, fp, "", f"p{k}")
        else:
            conn(cid, "const", "0", "", f"p{k}")
    return cid


build_mul()


# ---------------------------------------------------------------------------
# mux2 : 1-bit 2:1 multiplexer.  out = sel ? d1 : d0
#        = (NOT sel AND d0) OR (sel AND d1)
#   inputs : d0, d1, sel   outputs : out
# ---------------------------------------------------------------------------
def build_mux2(cid="comp-mux2"):
    comp(cid, "1-bit 2:1 multiplexer: out = sel ? d1 : d0.", None)
    pin(cid, "in", "d0", None, "input selected when sel=0")
    pin(cid, "in", "d1", None, "input selected when sel=1")
    pin(cid, "in", "sel", None, "select")
    pin(cid, "out", "out", None, "selected bit")
    gate_inst(cid, "nsel", "gate-not")
    gate_inst(cid, "a0", "gate-and")
    gate_inst(cid, "a1", "gate-and")
    gate_inst(cid, "orr", "gate-or")
    conn(cid, "", "sel", "nsel", "in")
    conn(cid, "nsel", "out", "a0", "left")
    conn(cid, "", "d0", "a0", "right")
    conn(cid, "", "sel", "a1", "left")
    conn(cid, "", "d1", "a1", "right")
    conn(cid, "a0", "out", "orr", "left")
    conn(cid, "a1", "out", "orr", "right")
    conn(cid, "orr", "out", "", "out")
    return cid


build_mux2()


# ---------------------------------------------------------------------------
# div4 : restoring division. A / B -> quotient Q (W bits), remainder R (W bits).
#   MSB-first: for i = W-1..0:
#     R  = (R << 1) | a_i           (R is W+1 bits wide during iteration)
#     D  = R - B                    (compare via sub)
#     q_i = NOT borrow              (borrow=1 means R < B, i.e. subtract failed)
#     R  = q_i ? D : R              (restore if it failed)  -- per-bit mux
#   inputs  : a0..a{W-1}, b0..b{W-1}
#   outputs : q0..q{W-1}, r0..r{W-1}
#
# We use an adder{W+1}-style subtract: sub over W+1 bits. Rather than build a
# separate sub{W+1}, we reuse two's complement inline with an adder over W+1 bits.
# To keep it all from existing components, we build a dedicated (W+1)-bit adder
# and subtractor for the divider's internal width.
# ---------------------------------------------------------------------------
def build_div(cid=f"comp-div{W}", width=W):
    dw = width + 1                      # divider internal remainder width
    # ensure a (dw)-bit adder + subtractor exist
    build_adder(cid=f"comp-adder{dw}", width=dw)
    build_sub(cid=f"comp-sub{dw}", width=dw)

    comp(cid, f"{width}-bit restoring divider -> {width}-bit quotient + remainder.", width)
    for i in range(width):
        pin(cid, "in", f"a{i}", i, f"Dividend bit {i}")
        pin(cid, "in", f"b{i}", i, f"Divisor bit {i}")
        pin(cid, "out", f"q{i}", i, f"Quotient bit {i}")
        pin(cid, "out", f"r{i}", i, f"Remainder bit {i}")

    # Remainder register across stages: rem[pos] -> (node,port). Width dw.
    # Start at 0.
    rem = {p: ("const", "0") for p in range(dw)}

    for step in range(width):
        i = width - 1 - step            # dividend bit index processed this stage
        stage = f"st{step}"
        # 1) shift rem left by 1, bring in a_i at position 0
        shifted = {}
        shifted[0] = ("", f"a{i}")      # component input pin a{i}
        for p in range(1, dw):
            shifted[p] = rem[p - 1]     # old bit p-1 moves up to p
        # top bit rem[dw-1] shifted out is discarded (remainder always < 2^width so it's 0)

        # 2) D = shifted - B   (B is width bits, zero-extended to dw)
        sub_name = f"{stage}_sub"
        sub_inst(cid, sub_name, f"comp-sub{dw}")
        for p in range(dw):
            fn, fp = shifted[p]
            conn(cid, fn, fp, sub_name, f"a{p}")
        for p in range(dw):
            if p < width:
                conn(cid, "", f"b{p}", sub_name, f"b{p}")
            else:
                conn(cid, "const", "0", sub_name, f"b{p}")

        # 3) q_i = NOT borrow
        qgate = f"{stage}_q"
        gate_inst(cid, qgate, "gate-not")
        conn(cid, sub_name, "borrow", qgate, "in")
        conn(cid, qgate, "out", "", f"q{i}")

        # 4) rem = q_i ? D : shifted   (per-bit mux; sel = q_i = NOT borrow)
        newrem = {}
        for p in range(dw):
            mux = f"{stage}_mux{p}"
            sub_inst(cid, mux, "comp-mux2")
            # d0 (sel=0 -> borrow=1 -> restore) = shifted[p]
            fn, fp = shifted[p]
            conn(cid, fn, fp, mux, "d0")
            # d1 (sel=1 -> subtract succeeded) = D bit p
            conn(cid, sub_name, f"s{p}", mux, "d1")
            conn(cid, qgate, "out", mux, "sel")
            newrem[p] = (mux, "out")
        rem = newrem

    # After all stages, remainder low `width` bits are the answer
    for p in range(width):
        fn, fp = rem[p]
        conn(cid, fn, fp, "", f"r{p}")
    return cid


build_div()


out = {"Components": components, "ComponentPins": pins,
       "ComponentInstances": instances, "ComponentConnections": connections}
with open("generated_netlists.json", "w") as f:
    json.dump(out, f, indent=1)
print(f"wrote generated_netlists.json: {len(components)} comps, {len(pins)} pins, "
      f"{len(instances)} instances, {len(connections)} connections")
