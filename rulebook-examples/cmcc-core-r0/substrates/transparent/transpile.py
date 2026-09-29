"""Transparent substrate: R0 spec -> Python module with one function per
source component (paper sections 12-13).

Naming is the map phi.  Every node of G_R(c) gets exactly one function:

    pred:P        -> c_pred_P
    rule:id       -> c_rule_id
    mu:P__Q       -> c_mu_P__Q      (the mu-component of a recursive stratum)
    t:Name        -> c_t_Name
    constraint:P  -> c_constraint_P
    obs:name      -> c_obs_name

and the only calls between c_ functions are the images of E_R.  A rule
body becomes explicit nested loops; nothing in the emitted module reads the
spec JSON or dispatches on rule syntax.  That is what makes the artifact
inspectable by tools/reflection_check.py.

Usage:  python transpile.py spec.json out.py
"""
from __future__ import annotations

import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "..", "tools"))
import r0lang as L  # noqa: E402


def _py(v):
    return repr(v)


class _Gen:
    def __init__(self):
        self.lines = []
        self.tmp = 0

    def t(self):
        self.tmp += 1
        return f"_t{self.tmp}"

    def emit(self, indent, s):
        self.lines.append("    " * indent + s)


def _expr(e):
    if L.is_var(e):
        return e
    if isinstance(e, list):
        op, x, y = e
        return f"({_expr(x)} {op} {_expr(y)})"
    return _py(L.const_value(e))


def _cmp(op):
    return {"=": "==", "!=": "!=", "<": "<", "<=": "<=", ">": ">", ">=": ">="}[op]


def _pattern(g, args, bound):
    """Return (pattern names, conditions) for destructuring one tuple."""
    names, conds = [], []
    for a in args:
        if L.is_anon(a):
            names.append(g.t())
        elif L.is_var(a):
            if a in bound:
                n = g.t()
                names.append(n)
                conds.append(f"{n} == {a}")
            else:
                names.append(a)
                bound.add(a)
        else:
            n = g.t()
            names.append(n)
            conds.append(f"{n} == {_py(L.const_value(a))}")
    return names, conds


def _body(g, items, indent, bound):
    """Emit the loops/filters for a body; return the innermost indent."""
    for it in items:
        if "atom" in it:
            q, *args = it["atom"]
            names, conds = _pattern(g, args, bound)
            pat = names[0] + "," if len(names) == 1 else ", ".join(names)
            g.emit(indent, f"for ({pat}) in c_pred_{q}(S):")
            indent += 1
            for c in conds:
                g.emit(indent, f"if not ({c}): continue")
        elif "not" in it:
            q, *args = it["not"]
            names, conds = _pattern(g, args, set(bound))
            pat = names[0] + "," if len(names) == 1 else ", ".join(names)
            cond = " and ".join(conds) or "True"
            g.emit(indent, f"if any(True for ({pat}) in c_pred_{q}(S) if {cond}): continue")
        elif "cmp" in it:
            op, e1, e2 = it["cmp"]
            g.emit(indent, f"if not ({_expr(e1)} {_cmp(op)} {_expr(e2)}): continue")
        elif "let" in it:
            v, e = it["let"]
            g.emit(indent, f"{v} = {_expr(e)}")
            bound.add(v)
    return indent


def _head(hargs):
    parts = [a if L.is_var(a) else _py(L.const_value(a)) for a in hargs]
    return "(" + ", ".join(parts) + ("," if len(parts) == 1 else "") + ")"


def _rule(g, r):
    head, *hargs = r["head"]
    g.emit(0, f"def c_rule_{r['id']}(S):")
    g.emit(1, "out = set()")
    if "agg" not in r:
        ind = _body(g, r["body"], 1, set())
        g.emit(ind, f"out.add({_head(hargs)})")
        g.emit(1, "return out")
        g.emit(0, "")
        return
    a = r["agg"]
    op = a["op"]
    gvars = hargs[:-1]
    if "keys" in a:
        bound = set()
        ind = _body(g, a["keys"], 1, bound)
        g.emit(ind, "_vals = set()")
        ind2 = _body(g, a["over"], ind, set(bound))
        g.emit(ind2, f"_vals.add({_expr(a['expr'])})")
        _agg_emit(g, ind, op, gvars)
    else:
        g.emit(1, "_groups = {}")
        ind = _body(g, a["over"], 1, set())
        key = "(" + ", ".join(gvars) + ("," if len(gvars) == 1 else "") + ")"
        g.emit(ind, f"_groups.setdefault({key}, set()).add({_expr(a['expr'])})")
        g.emit(1, "for _key, _vals in _groups.items():")
        _agg_emit(g, 2, op, None)
    g.emit(1, "return out")
    g.emit(0, "")


def _agg_emit(g, ind, op, gvars):
    key = ("(" + ", ".join(gvars) + ("," if len(gvars) == 1 else "") + ")") if gvars is not None else "_key"
    if op == "COUNT":
        g.emit(ind, f"out.add({key} + (len(_vals),))")
    elif op == "SUM":
        g.emit(ind, f"out.add({key} + (sum(_vals),))")
    else:                                   # MIN / MAX: nothing on empty
        g.emit(ind, "if _vals:")
        g.emit(ind + 1, f"out.add({key} + ({op.lower()}(_vals),))")


def transpile(spec) -> str:
    spec = L.normalize(spec)
    G = L.constructor_graph(spec)
    st = G["strata"]
    g = _Gen()
    g.emit(0, f'"""Generated transparent substrate for R0 spec {spec.get("name")!r}.')
    g.emit(0, "One function per source component; do not edit by hand.\"\"\"")
    g.emit(0, "")
    for p in st["base"]:
        g.emit(0, f"def c_pred_{p}(S):")
        g.emit(1, f'return S["{p}"]')
        g.emit(0, "")
    for i, s in enumerate(st["strata"]):
        mu = "__".join(s["preds"])
        for r in s["rules"]:
            _rule(g, r)
        if not s["recursive"]:
            for p in s["preds"]:
                g.emit(0, f"def c_pred_{p}(S):")
                g.emit(1, f'if "{p}" not in S:')
                g.emit(2, f'S["{p}"] = set()')
                for r in s["rules"]:
                    if r["head"][0] == p:
                        g.emit(2, f'S["{p}"] |= c_rule_{r["id"]}(S)')
                g.emit(1, f'return S["{p}"]')
                g.emit(0, "")
            continue
        g.emit(0, f"def c_mu_{mu}(S):")
        g.emit(1, f'if "_mu_{mu}" in S: return')
        g.emit(1, f'S["_mu_{mu}"] = "running"')
        for p in s["preds"]:
            g.emit(1, f'S["{p}"] = set()')
        g.emit(1, "changed = True")
        g.emit(1, "while changed:")
        g.emit(2, "changed = False")
        for r in s["rules"]:
            h = r["head"][0]
            g.emit(2, f"_new = c_rule_{r['id']}(S)")
            g.emit(2, f'if not _new <= S["{h}"]:')
            g.emit(3, f'S["{h}"] |= _new')
            g.emit(3, "changed = True")
        g.emit(1, f'S["_mu_{mu}"] = "done"')
        g.emit(0, "")
        for p in s["preds"]:
            g.emit(0, f"def c_pred_{p}(S):")
            g.emit(1, f"c_mu_{mu}(S)")
            g.emit(1, f'return S["{p}"]')
            g.emit(0, "")
    for t in spec["transitions"]:
        g.emit(0, f"def c_t_{t['name']}(S):")
        effs = ", ".join(f'("{e["op"]}", "{e["rel"]}", c_pred_{e["from"]}(S))' for e in t.get("effects", []))
        evs = ", ".join(f'"{e["rel"]}": c_pred_{e["from"]}(S)' for e in t.get("events", []))
        g.emit(1, f'return {{"guard": c_pred_{t["guard"]}(S), "effects": [{effs}], "events": {{{evs}}}}}')
        g.emit(0, "")
    for c in spec["constraints"]:
        g.emit(0, f"def c_constraint_{c}(S):")
        g.emit(1, f"return c_pred_{c}(S)")
        g.emit(0, "")
    for name, p in spec["observables"].items():
        g.emit(0, f"def c_obs_{name}(S):")
        g.emit(1, f"return c_pred_{p}(S)")
        g.emit(0, "")
    # Entry-point registry for the runtime (data, not components).
    g.emit(0, "MODEL = " + json.dumps({
        "name": spec.get("name"),
        "edb": spec["edb"],
        "commands": spec["commands"],
        "transitions": [{"name": t["name"], "command": t["command"]} for t in spec["transitions"]],
        "constraints": spec["constraints"],
        "observables": list(spec["observables"]),
        "h0": spec["h0"],
    }, indent=1))
    g.emit(0, "")
    return "\n".join(g.lines) + "\n"


def main(argv):
    spec = L.load(argv[1])
    src = transpile(spec)
    with open(argv[2], "w") as f:
        f.write(src)
    print(f"wrote {argv[2]} ({src.count(chr(10))} lines)")


if __name__ == "__main__":
    main(sys.argv)
