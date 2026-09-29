"""r0-lint: decide whether a spec is a member of R0.

Checks the restrictions the CMCC-Core proof depends on (paper section 21):

  RR   range restriction (section 3)
  VG   no value generation in recursive strata (section 3)
  ST   a stratification exists (section 4)
  NN   no nulls: syntactically impossible in this format; reported as a pass
  AO   append-only history: h0 sequence numbers strictly increase, ops valid
  TR   transitions, constraints and observables reference existing relations
       with the right arities (sections 8, 9)

Usage:  python r0_lint.py spec.json      (exit 1 on any violation)
"""
from __future__ import annotations

import json
import sys

import r0lang as L


def _bound_vars_of_body(items, viol, rule_id, arities, base_or_idb):
    """Walk a body left to right, returning the set of variables bound by
    positive atoms, and vars bound by 'let' separately."""
    pos = set()
    let = set()
    for it in items:
        if "atom" in it:
            q, *args = it["atom"]
            _check_atom(q, args, viol, rule_id, arities, base_or_idb)
            for a in args:
                if L.is_var(a):
                    pos.add(a)
        elif "not" in it:
            q, *args = it["not"]
            _check_atom(q, args, viol, rule_id, arities, base_or_idb)
            for a in args:
                if L.is_var(a) and a not in pos and a not in let:
                    viol.append(("RR", rule_id, f"variable {a} in negated atom {q} not bound by a positive atom"))
        elif "cmp" in it:
            op, e1, e2 = it["cmp"]
            if op not in L.CMP_OPS:
                viol.append(("SYN", rule_id, f"unknown comparison {op}"))
            for v in L.expr_vars(e1) | L.expr_vars(e2):
                if v not in pos and v not in let:
                    viol.append(("RR", rule_id, f"variable {v} in comparison not bound by a positive atom"))
        elif "let" in it:
            v, e = it["let"]
            for w in L.expr_vars(e):
                if w not in pos and w not in let:
                    viol.append(("RR", rule_id, f"variable {w} in let not bound by a positive atom"))
            let.add(v)
        else:
            viol.append(("SYN", rule_id, f"unknown body item {it}"))
    return pos, let


def _check_atom(q, args, viol, rule_id, arities, known):
    if q not in known:
        viol.append(("TR", rule_id, f"unknown relation {q}"))
        return
    if len(args) != arities[q]:
        viol.append(("TR", rule_id, f"{q} used with arity {len(args)}, declared {arities[q]}"))
    for a in args:
        if not (L.is_var(a) or L.is_anon(a) or L.is_const(a)):
            viol.append(("SYN", rule_id, f"bad argument {a!r} in {q}"))


def lint(spec) -> dict:
    viol = []
    spec = L.normalize(spec)
    rules = L.all_rules(spec)
    ids = [r["id"] for r in rules]
    if len(ids) != len(set(ids)):
        viol.append(("SYN", "-", "duplicate rule ids"))
    try:
        arities = L.arities(spec)
    except L.R0Error as e:
        return {"ok": False, "violations": [("SYN", "-", str(e))]}
    known = set(arities)

    # stratification
    strata = None
    try:
        strata = L.stratify(spec)
    except L.R0Error as e:
        viol.append(("ST", "-", str(e)))

    for r in rules:
        rid = r["id"]
        head, *hargs = r["head"]
        if "agg" in r:
            a = r["agg"]
            if a["op"] not in L.AGG_OPS:
                viol.append(("SYN", rid, f"unknown aggregate {a['op']}"))
            kpos, klet = _bound_vars_of_body(a.get("keys", []), viol, rid, arities, known)
            opos, olet = _bound_vars_of_body(a["over"], viol, rid, arities, known)
            for v in L.expr_vars(a["expr"]):
                if v not in kpos | klet | opos | olet:
                    viol.append(("RR", rid, f"aggregate expression variable {v} unbound"))
            for hv in hargs[:-1]:
                if L.is_var(hv) and hv not in kpos | klet:
                    viol.append(("RR", rid, f"group variable {hv} not bound by keys"))
            if strata and strata["strata"][strata["stratum_of"][head]]["recursive"]:
                viol.append(("ST", rid, "aggregate rule in a recursive stratum"))
            continue
        pos, let = _bound_vars_of_body(r["body"], viol, rid, arities, known)
        for hv in hargs:
            if L.is_var(hv) and hv not in pos and hv not in let:
                viol.append(("RR", rid, f"head variable {hv} not bound by a positive atom"))
            if L.is_anon(hv):
                viol.append(("RR", rid, "anonymous variable in head"))
        # VG: a head variable that comes from a let, in a recursive stratum
        if strata and head in strata["stratum_of"]:
            s = strata["strata"][strata["stratum_of"][head]]
            if s["recursive"]:
                for hv in hargs:
                    if L.is_var(hv) and hv in let and hv not in pos:
                        viol.append(("VG", rid, f"head variable {hv} is computed by let in recursive stratum of {head}"))

    # append-only initial history
    last = -1
    for e in spec["h0"]:
        n = e.get("n")
        if not isinstance(n, int) or n <= last:
            viol.append(("AO", "h0", f"event sequence numbers must strictly increase (at n={n})"))
        last = n if isinstance(n, int) else last
        if e.get("op") not in ("assert", "retract"):
            viol.append(("AO", "h0", f"bad op {e.get('op')}"))
        if e.get("rel") not in spec["edb"]:
            viol.append(("AO", "h0", f"unknown kind {e.get('rel')}"))
        elif len(e.get("tuple", [])) != len(spec["edb"][e["rel"]]):
            viol.append(("AO", "h0", f"tuple arity mismatch for {e['rel']} at n={n}"))

    # transitions / constraints / observables
    for t in spec["transitions"]:
        tn = t["name"]
        if t["command"] not in spec["commands"]:
            viol.append(("TR", tn, f"unknown command {t['command']}"))
        if t["guard"] not in known:
            viol.append(("TR", tn, f"unknown guard relation {t['guard']}"))
        for e in t.get("effects", []):
            if e.get("op") not in ("assert", "retract"):
                viol.append(("AO", tn, f"effect op must be assert or retract, got {e.get('op')}"))
            if e.get("rel") not in spec["edb"]:
                viol.append(("TR", tn, f"effect on unknown kind {e.get('rel')}"))
            elif e["from"] not in known:
                viol.append(("TR", tn, f"unknown effect relation {e['from']}"))
            elif arities[e["from"]] != len(spec["edb"][e["rel"]]) + 2:
                viol.append(("TR", tn, f"effect relation {e['from']} must have arity cols+2 (vfrom, vto)"))
        for e in t.get("events", []):
            if e["from"] not in known:
                viol.append(("TR", tn, f"unknown event relation {e['from']}"))
    for c in spec["constraints"]:
        if c not in known:
            viol.append(("TR", c, "unknown constraint relation"))
    for name, p in spec["observables"].items():
        if p not in known:
            viol.append(("TR", name, f"unknown observable relation {p}"))

    cert = {
        "ok": not viol,
        "violations": viol,
        "checks": ["RR", "VG", "ST", "NN", "AO", "TR"],
        "note": "NN passes by construction: this format has no null literal and MIN/MAX derive nothing on empty groups.",
    }
    if strata:
        cert["strata"] = [
            {"index": i, "preds": s["preds"], "recursive": s["recursive"],
             "rules": [r["id"] for r in s["rules"]]}
            for i, s in enumerate(strata["strata"])]
    return cert


def main(argv):
    if len(argv) != 2:
        print(__doc__)
        return 2
    with open(argv[1]) as f:
        cert = lint(json.load(f))
    print(json.dumps(cert, indent=2))
    return 0 if cert["ok"] else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv))
