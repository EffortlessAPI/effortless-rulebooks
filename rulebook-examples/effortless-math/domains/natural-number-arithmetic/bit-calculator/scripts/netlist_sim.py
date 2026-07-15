#!/usr/bin/env python3
"""Data-driven netlist simulator for the bit-calculator rulebook.

This is a *second substrate* (alongside the Postgres recursive view). It contains
NO arithmetic: it only

  1. looks up a gate's output in GateTruthRows (a table lookup), and
  2. follows ComponentConnections from one port to another.

Given values on a component's input pins, it settles every internal and output
wire by recursively evaluating sub-instances until it hits primitive gates.

It exists to VALIDATE each netlist as data — before we trust the rulebook — by
evaluating a component across an exhaustive set of inputs and comparing the
settled output pins to known-good arithmetic (the comparison lives only in the
validator harness, never in the engine).

Usage:
    from netlist_sim import Netlist
    nl = Netlist("../effortless-rulebook/bit-calculator-rulebook.json")
    out = nl.evaluate("comp-half-adder", {"a": 1, "b": 1})   # -> {"sum":0,"carry":1}
"""
import json
import sys


class Netlist:
    def __init__(self, rulebook_path):
        with open(rulebook_path) as f:
            rb = json.load(f)
        self.rb = rb
        # gate truth tables: (gate_type_id, in0, in1) -> out_bit   (in1 None for NOT)
        self.truth = {}
        for r in rb["GateTruthRows"]["data"]:
            self.truth[(r["gate_type_id"], r["in0"], r["in1"])] = r["out_bit"]
        self.gate_arity = {g["gate_type_id"]: g["arity"] for g in rb["GateTypes"]["data"]}
        self.gate_outport = {g["gate_type_id"]: g["out_port"] for g in rb["GateTypes"]["data"]}
        # instances per component: component_id -> {instance_name: instance_row}
        self.instances = {}
        for r in rb["ComponentInstances"]["data"]:
            self.instances.setdefault(r["component_id"], {})[r["instance_name"]] = r
        # connections per component: component_id -> list of connection rows
        self.connections = {}
        for r in rb["ComponentConnections"]["data"]:
            self.connections.setdefault(r["component_id"], []).append(r)
        # pins per component: component_id -> {"in":[names], "out":[names]}
        self.pins = {}
        for r in rb["ComponentPins"]["data"]:
            d = self.pins.setdefault(r["component_id"], {"in": [], "out": []})
            d[r["direction"]].append(r["pin_name"])

    def eval_gate(self, gate_type_id, inputs):
        """inputs: dict of port->bit. Ports for binary gates are 'left','right';
        for NOT it's 'in'. Returns the single out bit."""
        arity = self.gate_arity[gate_type_id]
        if arity == 2:
            key = (gate_type_id, inputs["left"], inputs["right"])
        elif arity == 1:
            key = (gate_type_id, inputs["in"], None)
        else:
            raise ValueError(f"unsupported gate arity {arity} for {gate_type_id}")
        if key not in self.truth:
            raise KeyError(f"no truth row for {key}")
        return self.truth[key]

    def evaluate(self, component_id, input_values):
        """Settle a component. input_values: {pin_name: bit}. Returns {out_pin: bit}.

        Wires are addressed by (node, port); node '' is the component boundary.
        We iterate to a fixed point: repeatedly propagate along connections and
        fire any instance whose inputs are all known. Pure data-following — no
        arithmetic, no knowledge of what the circuit 'means'.
        """
        conns = self.connections.get(component_id, [])
        insts = self.instances.get(component_id, {})
        # wire values keyed by (node, port)
        wires = {}
        for pin, bit in input_values.items():
            wires[("", pin)] = bit
        # constant sources: a connection may originate at node 'const' with
        # port '0' or '1' to drive a hardwired bit (e.g. two's-complement cin=1).
        wires[("const", "0")] = 0
        wires[("const", "1")] = 1

        # instance output-port name lookup
        def inst_outport(inst_name):
            inst = insts[inst_name]
            if inst["instance_kind"] == "gate":
                return [self.gate_outport[inst["gate_type_id"]]]
            else:
                return self.pins[inst["sub_component_id"]]["out"]

        progressed = True
        guard = 0
        while progressed:
            progressed = False
            guard += 1
            if guard > 10000:
                raise RuntimeError(f"{component_id}: netlist did not settle (cycle?)")
            # 1) propagate every connection whose source is known to its dest
            for c in conns:
                src = (c["from_node"], c["from_port"])
                dst = (c["to_node"], c["to_port"])
                if src in wires and dst not in wires:
                    wires[dst] = wires[src]
                    progressed = True
            # 2) fire any instance whose input pins are all present
            for name, inst in insts.items():
                if inst["instance_kind"] == "gate":
                    arity = self.gate_arity[inst["gate_type_id"]]
                    inports = ["left", "right"] if arity == 2 else ["in"]
                    if all((name, p) in wires for p in inports):
                        outp = self.gate_outport[inst["gate_type_id"]]
                        if (name, outp) not in wires:
                            gi = {p: wires[(name, p)] for p in inports}
                            wires[(name, outp)] = self.eval_gate(inst["gate_type_id"], gi)
                            progressed = True
                else:  # sub-component
                    sub = inst["sub_component_id"]
                    sub_in = self.pins[sub]["in"]
                    if all((name, p) in wires for p in sub_in):
                        if not all((name, op) in wires for op in inst_outport(name)):
                            sub_inputs = {p: wires[(name, p)] for p in sub_in}
                            sub_out = self.evaluate(sub, sub_inputs)
                            for op, bit in sub_out.items():
                                wires[(name, op)] = bit
                            progressed = True

        out_pins = self.pins[component_id]["out"]
        result = {}
        for op in out_pins:
            if ("", op) not in wires:
                raise RuntimeError(f"{component_id}: output pin '{op}' never settled")
            result[("", op)] = wires[("", op)]
        return {op: result[("", op)] for op in out_pins}


if __name__ == "__main__":
    rb = sys.argv[1] if len(sys.argv) > 1 else "../effortless-rulebook/bit-calculator-rulebook.json"
    nl = Netlist(rb)
    print("half_adder(1,1) =", nl.evaluate("comp-half-adder", {"a": 1, "b": 1}))
    print("full_adder(1,1,cin=1) =", nl.evaluate("comp-full-adder", {"a": 1, "b": 1, "cin": 1}))
