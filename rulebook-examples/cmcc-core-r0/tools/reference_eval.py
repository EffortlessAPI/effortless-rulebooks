"""Reference evaluator: derived state by naive stratified fixpoint.

Written from the paper's semantics (sections 3-6) and nothing else: a rule
body is a left-to-right binding walk, a stratum is iterated to its least
fixed point, aggregates range over the SET of distinct expression values in
a group.  Deliberately unoptimised so it is easy to read against the paper.
"""
from __future__ import annotations

import r0lang as L


def _ev(e, b):
    if L.is_var(e):
        return b[e]
    if isinstance(e, list):
        op, x, y = e
        x, y = _ev(x, b), _ev(y, b)
        if op == "+":
            return x + y
        if op == "-":
            return x - y
        if op == "*":
            return x * y
        raise L.R0Error(f"unknown arithmetic op {op}")
    return L.const_value(e)


def _cmp(op, x, y):
    return {"=": x == y, "!=": x != y, "<": x < y, "<=": x <= y,
            ">": x > y, ">=": x >= y}[op]


def _unify(args, tup, b):
    b2 = dict(b)
    for a, v in zip(args, tup):
        if L.is_anon(a):
            continue
        if L.is_var(a):
            if a in b2:
                if b2[a] != v:
                    return None
            else:
                b2[a] = v
        elif L.const_value(a) != v:
            return None
    return b2


def eval_body(items, rels, binds=None):
    binds = [{}] if binds is None else list(binds)
    for it in items:
        if "atom" in it:
            q, *args = it["atom"]
            new = []
            for b in binds:
                for tup in rels[q]:
                    b2 = _unify(args, tup, b)
                    if b2 is not None:
                        new.append(b2)
            binds = new
        elif "not" in it:
            q, *args = it["not"]
            binds = [b for b in binds
                     if not any(_unify(args, tup, b) is not None for tup in rels[q])]
        elif "cmp" in it:
            op, e1, e2 = it["cmp"]
            binds = [b for b in binds if _cmp(op, _ev(e1, b), _ev(e2, b))]
        elif "let" in it:
            v, e = it["let"]
            binds = [{**b, v: _ev(e, b)} for b in binds]
    return binds


def _head(hargs, b):
    return tuple(b[a] if L.is_var(a) else L.const_value(a) for a in hargs)


def eval_rule(rule, rels) -> set:
    head, *hargs = rule["head"]
    if "agg" not in rule:
        return {_head(hargs, b) for b in eval_body(rule["body"], rels)}
    a = rule["agg"]
    out = set()
    gvars = hargs[:-1]
    if "keys" in a:
        groups = [(tuple(b[g] for g in gvars), b) for b in eval_body(a["keys"], rels)]
        groups = {k: b for k, b in groups}.items()      # distinct keys
        collect = lambda b: {_ev(a["expr"], x) for x in eval_body(a["over"], rels, [b])}
    else:
        bs = eval_body(a["over"], rels)
        by = {}
        for b in bs:
            by.setdefault(tuple(b[g] for g in gvars), set()).add(_ev(a["expr"], b))
        groups = [(k, None) for k in by]
        collect = lambda b: None
        vals_of = by
    for key, b in groups:
        vals = collect(b) if b is not None else vals_of[key]
        op = a["op"]
        if op == "COUNT":
            r = len(vals)
        elif op == "SUM":
            r = sum(vals)
        elif op in ("MIN", "MAX"):
            if not vals:
                continue                       # section 5: no tuple derived
            r = min(vals) if op == "MIN" else max(vals)
        else:
            raise L.R0Error(op)
        out.add(key + (r,))
    return out


def base_state(spec, hist, now, cmd) -> dict:
    rels = {f"Hist_{r}": set() for r in spec["edb"]}
    for (n, op, rel, cols, vf, vt, k) in hist:
        rels[f"Hist_{rel}"].add((n, op) + tuple(cols) + (vf, vt, k))
    rels["Now"] = {tuple(now)}
    for c in spec["commands"]:
        rels[f"Cmd_{c}"] = set()
    if cmd is not None:
        rels[f"Cmd_{cmd[0]}"] = {tuple(cmd[1])}
    return rels


def derived_state(spec, hist, now, cmd) -> dict:
    rels = base_state(spec, hist, now, cmd)
    st = L.stratify(spec)
    for s in st["strata"]:
        for p in s["preds"]:
            rels.setdefault(p, set())
        if not s["recursive"]:
            for r in s["rules"]:
                rels[r["head"][0]] |= eval_rule(r, rels)
            continue
        changed = True
        while changed:                       # T_j iterated from bottom
            changed = False
            for r in s["rules"]:
                new = eval_rule(r, rels)
                if not new <= rels[r["head"][0]]:
                    rels[r["head"][0]] |= new
                    changed = True
    return rels
