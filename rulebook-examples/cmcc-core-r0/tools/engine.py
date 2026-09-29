"""Step and trace semantics of R0 (paper sections 8 and 9).

This is the executor's HOW for the transactional invariant chi: it applies
a command to a history, computes the permitted successor set, and records
observations.  It contains no domain knowledge; it talks to a Model:

    model.h0()                       -> initial history
    model.transitions                -> [(name, command), ...]
    model.evaluate(hist, now, cmd)   -> Evaluated

    Evaluated.guard(name)            -> set   (non-empty = enabled)
    Evaluated.effects(name)          -> [(op, rel, set of (cols..., vfrom, vto))]
    Evaluated.events(name)           -> {rel: set}
    Evaluated.violations()           -> [constraint name, ...]
    Evaluated.observations()         -> {obs name: set}

Two Model implementations exist: SpecModel (spec JSON + a derive function:
the reference evaluator or the SQLite oracle) and, in substrates/
transparent/runtime.py, ModuleModel over a generated module.  The engine
is shared so the only thing that differs between substrates is the WHAT
they were handed.

A history is a tuple of events (n, op, rel, cols, vfrom, vto, k).
"""
from __future__ import annotations

import json

import r0lang as L


def history_from(events):
    return tuple(
        (e["n"], e["op"], e["rel"], tuple(e["tuple"]), e.get("vfrom", 0),
         e.get("vto", L.INF), e["k"]) for e in events)


class SpecModel:
    """A spec JSON evaluated by a pluggable derive(spec, hist, now, cmd)."""

    def __init__(self, spec, derive, observer=None):
        self.spec = L.normalize(spec)
        self.derive = derive
        self.observer = observer
        self.transitions = [(t["name"], t["command"]) for t in self.spec["transitions"]]
        self._t = {t["name"]: t for t in self.spec["transitions"]}

    def h0(self):
        return history_from(self.spec["h0"])

    def evaluate(self, hist, now, cmd):
        D = self.derive(self.spec, hist, now, cmd)
        if self.observer:
            self.observer(hist, now, cmd, D)
        spec, T = self.spec, self._t

        class Ev:
            def guard(self, name):
                return D[T[name]["guard"]]

            def effects(self, name):
                return [(e["op"], e["rel"], D[e["from"]]) for e in T[name].get("effects", [])]

            def events(self, name):
                return {e["rel"]: D[e["from"]] for e in T[name].get("events", [])}

            def violations(self):
                return [c for c in spec["constraints"] if D[c]]

            def observations(self):
                return {n: D[p] for n, p in spec["observables"].items()}
        return Ev()


def _next_n(hist):
    return max((e[0] for e in hist), default=-1) + 1


def _json_rel(s):
    return sorted(list(x) for x in s)


def step(model, hist, s):
    """Return the permitted successor list [(outcome, events, hist')]."""
    K = s["k"]
    V = s.get("v", K)
    cmd = (s["command"], tuple(s.get("params", [])))
    ev = model.evaluate(hist, (V, K), cmd)
    enabled = [name for name, c in model.transitions if c == cmd[0] and ev.guard(name)]
    if not enabled:
        return [("rejected(guard)", {}, hist)]
    results = []
    for name in enabled:
        new = list(hist)
        n = _next_n(hist)
        for op, rel, tuples in ev.effects(name):
            for tup in sorted(tuples):
                cols, vf, vt = tuple(tup[:-2]), tup[-2], tup[-1]
                new.append((n, op, rel, cols, vf, vt, K))
                n += 1
        new = tuple(new)
        violated = model.evaluate(new, (V, K), None).violations()
        if violated:
            results.append((f"rejected(constraint:{','.join(violated)})", {}, hist))
        else:
            events = {rel: _json_rel(ts) for rel, ts in ev.events(name).items()}
            results.append(("committed", events, new))
    return results


def observe(model, hist, now):
    return {name: _json_rel(ts) for name, ts in model.evaluate(hist, now, None).observations().items()}


def run(model, inputs, choose=None):
    """choose=None: the full trace SET (all permitted successors).
    choose='first': one trace, always taking the first permitted successor
    (what a deterministic executor might do).  Returns {"traces": [...]}."""
    steps = inputs["steps"][: inputs.get("horizon", len(inputs["steps"]))]
    traces = []

    def go(hist, i, trace):
        if i == len(steps):
            traces.append(trace)
            return
        s = steps[i]
        succ = step(model, hist, s)
        if choose == "first":
            succ = succ[:1]
        for outcome, events, h2 in succ:
            # Observation coordinates: valid time v (default k) and, for an
            # as-of query of what was known earlier, obs_k (default k).
            V, K = s.get("v", s["k"]), s.get("obs_k", s["k"])
            obs = observe(model, h2, (V, K))
            go(h2, i + 1, trace + [{"outcome": outcome, "events": events, "obs": obs}])

    go(model.h0(), 0, [])
    return {"traces": traces}


def canon(trace) -> str:
    return json.dumps(trace, sort_keys=True, separators=(",", ":"))


def trace_set(result) -> set:
    return {canon(t) for t in result["traces"]}
