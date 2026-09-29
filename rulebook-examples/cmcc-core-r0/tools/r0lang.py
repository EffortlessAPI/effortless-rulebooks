"""R0 language support shared by the linter, evaluators, transpilers and checks.

R0 is the reference language of the CMCC-Core paper (sections 2-9). This
module knows the *syntax* of an R0 spec and its static structure
(stratification, the constructor graph G_R). It contains no evaluator.

Spec JSON shape (see r0/spec.json for a full example):

  edb:          {Rel: [col, ...]}                       extensional kinds
  commands:     {Cmd: [param, ...]}                      external inputs
  rules:        [rule, ...]                              stratified program P
  transitions:  [{name, command, guard, effects, events}]
  constraints:  [IdbName, ...]      each must be EMPTY in a committed state
  observables:  {name: IdbName}     the interface O
  h0:           [event, ...]        initial history

A rule is {"id", "head": [P, arg...], "body": [item...]} or an aggregate
rule {"id", "head": [P, g..., r], "agg": {"op", "expr", "keys": body,
"over": body}}.  Body items: {"atom": [Q, arg...]}, {"not": [Q, arg...]},
{"cmp": [op, e1, e2]}, {"let": [var, expr]}.  An arg is a variable
(identifier string), "_" (anonymous), a number, or {"$": "string"}.

Every event history row for kind R is exposed to rules as the relation
Hist_R(n, op, col..., vfrom, vto, k); the current bitemporal snapshot R
and the helper Retracted_R are *generated* rules (tau_rules), not
primitives -- paper section 7.
"""
from __future__ import annotations

import json
import re
from collections import defaultdict

INF = 2 ** 62          # open-ended valid time (section 7)
VAR_RE = re.compile(r"^[a-z_][A-Za-z0-9_]*$")
AGG_OPS = {"COUNT", "SUM", "MIN", "MAX"}
CMP_OPS = {"=", "!=", "<", "<=", ">", ">="}
ARITH_OPS = {"+", "-", "*"}


class R0Error(Exception):
    pass


# ---------------------------------------------------------------- terms

def is_var(a) -> bool:
    return isinstance(a, str) and a != "_" and VAR_RE.match(a) is not None


def is_anon(a) -> bool:
    return a == "_"


def const_value(a):
    """Return the constant value of a non-variable arg, else raise."""
    if isinstance(a, bool):
        raise R0Error("booleans are not R0 values")
    if isinstance(a, (int, float)):
        return int(a)
    if isinstance(a, dict) and "$" in a:
        return a["$"]
    raise R0Error(f"not a constant: {a!r}")


def is_const(a) -> bool:
    return (isinstance(a, (int, float)) and not isinstance(a, bool)) or (
        isinstance(a, dict) and "$" in a)


def expr_vars(e) -> set:
    if is_var(e):
        return {e}
    if isinstance(e, list):
        return set().union(*(expr_vars(x) for x in e[1:]))
    return set()


# ---------------------------------------------------------------- spec

def load(path) -> dict:
    with open(path) as f:
        spec = json.load(f)
    return normalize(spec)


def normalize(spec: dict) -> dict:
    spec = dict(spec)
    spec.setdefault("rules", [])
    spec.setdefault("transitions", [])
    spec.setdefault("constraints", [])
    spec.setdefault("observables", {})
    spec.setdefault("h0", [])
    spec.setdefault("commands", {})
    for i, r in enumerate(spec["rules"]):
        r.setdefault("id", f"r{i}")
    return spec


def hist_cols(spec, rel) -> list:
    return ["n", "op"] + list(spec["edb"][rel]) + ["vfrom", "vto", "k"]


def base_relations(spec) -> dict:
    """Relations supplied by the executor at each step: Hist_R, Now, Cmd_X."""
    rels = {f"Hist_{r}": hist_cols(spec, r) for r in spec["edb"]}
    rels["Now"] = ["v", "k"]
    for c, params in spec["commands"].items():
        rels[f"Cmd_{c}"] = list(params)
    return rels


def tau_rules(spec) -> list:
    """The bitemporal snapshot as stratified rules (paper section 7)."""
    out = []
    for rel, cols in spec["edb"].items():
        cs = list(cols)
        out.append({
            "id": f"tau_Retracted_{rel}",
            "head": [f"Retracted_{rel}", "n"],
            "body": [
                {"atom": [f"Hist_{rel}", "n", {"$": "assert"}] + cs + ["vf", "vt", "k1"]},
                {"atom": [f"Hist_{rel}", "m", {"$": "retract"}] + cs + ["vf", "vt", "k2"]},
                {"atom": ["Now", "v", "k"]},
                {"cmp": [">", "m", "n"]},
                {"cmp": ["<=", "k2", "k"]},
            ],
        })
        out.append({
            "id": f"tau_{rel}",
            "head": [rel] + cs,
            "body": [
                {"atom": [f"Hist_{rel}", "n", {"$": "assert"}] + cs + ["vf", "vt", "k1"]},
                {"atom": ["Now", "v", "k"]},
                {"cmp": ["<=", "k1", "k"]},
                {"cmp": ["<=", "vf", "v"]},
                {"cmp": ["<", "v", "vt"]},
                {"not": [f"Retracted_{rel}", "n"]},
            ],
        })
    return out


def all_rules(spec) -> list:
    return tau_rules(spec) + list(spec["rules"])


def rule_body_items(rule) -> list:
    if "agg" in rule:
        return list(rule["agg"].get("keys", [])) + list(rule["agg"]["over"])
    return list(rule["body"])


def rule_deps(rule) -> list:
    """[(pred, kind)] where kind in {'pos','neg','agg'}."""
    deps = []
    if "agg" in rule:
        for it in rule["agg"].get("keys", []):
            if "atom" in it:
                deps.append((it["atom"][0], "pos"))
            elif "not" in it:
                deps.append((it["not"][0], "neg"))
        for it in rule["agg"]["over"]:
            if "atom" in it:
                deps.append((it["atom"][0], "agg"))
            elif "not" in it:
                deps.append((it["not"][0], "neg"))
        return deps
    for it in rule["body"]:
        if "atom" in it:
            deps.append((it["atom"][0], "pos"))
        elif "not" in it:
            deps.append((it["not"][0], "neg"))
    return deps


def arities(spec) -> dict:
    ar = {r: len(c) for r, c in base_relations(spec).items()}
    for r in all_rules(spec):
        h = r["head"]
        ar.setdefault(h[0], len(h) - 1)
        if ar[h[0]] != len(h) - 1:
            raise R0Error(f"arity mismatch for {h[0]} in rule {r['id']}")
    return ar


# ---------------------------------------------------------------- stratification

def _tarjan(nodes, succ):
    index = {}
    low = {}
    stack = []
    on = set()
    out = []
    counter = [0]

    def visit(v):
        index[v] = low[v] = counter[0]
        counter[0] += 1
        stack.append(v)
        on.add(v)
        for w in sorted(succ.get(v, ())):   # deterministic across processes
            if w not in index:
                visit(w)
                low[v] = min(low[v], low[w])
            elif w in on:
                low[v] = min(low[v], index[w])
        if low[v] == index[v]:
            comp = []
            while True:
                w = stack.pop()
                on.discard(w)
                comp.append(w)
                if w == v:
                    break
            out.append(sorted(comp))

    for v in sorted(nodes):
        if v not in index:
            visit(v)
    return out


def stratify(spec) -> dict:
    """Return {'strata': [ {preds, rules, recursive} ...] in evaluation order,
    'stratum_of': {pred: index}}.  Raises R0Error if no stratification exists
    (a negative or aggregate edge inside a recursive component)."""
    rules = all_rules(spec)
    base = base_relations(spec)
    preds = set(base)
    edges = defaultdict(set)      # body pred -> head pred
    strict = set()                # (body, head) edges that must be strict
    for r in rules:
        h = r["head"][0]
        preds.add(h)
        for q, kind in rule_deps(r):
            preds.add(q)
            edges[q].add(h)
            if kind != "pos":
                strict.add((q, h))
    comps = _tarjan(preds, edges)
    comp_of = {}
    for i, c in enumerate(comps):
        for p in c:
            comp_of[p] = i
    for (q, h) in strict:
        if comp_of[q] == comp_of[h]:
            raise R0Error(
                f"not stratifiable: negation/aggregation over {q} inside the "
                f"recursive component of {h}")
    # Tarjan emits components in reverse topological order of the edge
    # direction body->head, i.e. heads first; reverse to get bodies first.
    order = list(reversed(comps))
    strata = []
    stratum_of = {}
    for comp in order:
        rs = [r for r in rules if r["head"][0] in comp]
        if not rs:
            continue                       # base relations form no stratum
        recursive = len(comp) > 1 or any(
            q in comp for r in rs for q, _ in rule_deps(r))
        idx = len(strata)
        for p in comp:
            stratum_of[p] = idx
        strata.append({"preds": sorted(comp), "rules": rs, "recursive": recursive})
    return {"strata": strata, "stratum_of": stratum_of, "base": base}


# ---------------------------------------------------------------- G_R

def constructor_graph(spec) -> dict:
    """The source constructor graph G_R(c) of paper section 12.

    Nodes are typed ids: 'pred:P', 'rule:id', 'mu:<preds joined by __>',
    't:<transition>', 'constraint:<pred>', 'obs:<name>'.  Edges (u, v) mean
    v depends on u.  For a recursive stratum S the head edges go rule -> mu
    and mu -> pred (the mu-component of section 11); for a non-recursive
    rule they go rule -> pred directly."""
    st = stratify(spec)
    nodes = {}
    edges = set()
    for p in st["base"]:
        nodes[f"pred:{p}"] = "base"
    for i, s in enumerate(st["strata"]):
        for p in s["preds"]:
            nodes[f"pred:{p}"] = "pred"
        mu = f"mu:{'__'.join(s['preds'])}"
        if s["recursive"]:
            nodes[mu] = "mu"
        for r in s["rules"]:
            rid = f"rule:{r['id']}"
            nodes[rid] = "agg" if "agg" in r else "rule"
            for q, _ in rule_deps(r):
                edges.add((f"pred:{q}", rid))
            if s["recursive"]:
                edges.add((rid, mu))
            else:
                edges.add((rid, f"pred:{r['head'][0]}"))
        if s["recursive"]:
            for p in s["preds"]:
                edges.add((mu, f"pred:{p}"))
    for t in spec["transitions"]:
        tid = f"t:{t['name']}"
        nodes[tid] = "transition"
        edges.add((f"pred:{t['guard']}", tid))
        for e in t.get("effects", []):
            edges.add((f"pred:{e['from']}", tid))
        for e in t.get("events", []):
            edges.add((f"pred:{e['from']}", tid))
    for c in spec["constraints"]:
        nodes[f"constraint:{c}"] = "constraint"
        edges.add((f"pred:{c}", f"constraint:{c}"))
    for name, p in spec["observables"].items():
        nodes[f"obs:{name}"] = "obs"
        edges.add((f"pred:{p}", f"obs:{name}"))
    return {"nodes": nodes, "edges": sorted(edges), "strata": st}
