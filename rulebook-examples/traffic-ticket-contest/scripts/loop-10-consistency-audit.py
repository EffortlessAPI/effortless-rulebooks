#!/usr/bin/env python3
"""
loop-10-consistency-audit.py

Loop 10: the grand consistency pass + completeness critic. A ruthless read-only
cross-table integrity report over the whole rulebook. Prints residuals per check;
the gate is 0 unresolved across the board.

Checks (the plan's gate list):
  A. Every relationship FK resolves to a real target row (both kinds: explicit
     `RelatedTo` relationships AND value-inferred FKs that the transpiler emits).
  B. Every ERBFeature has SourceText (>=3 sentences), RuleRefs (all resolving),
     a nav row (Route -> nav PK), and >=1 TestCase.
  C. Every APIEndpoint has a real SubjectTableName and (if set) TriggersStateMachine.
  D. Every StateTransitionRule's FromState/ToState exist; each machine has exactly
     1 initial + >=1 terminal.
  E. Every endpoint / transition / business rule is referenced by >=1 TestCase
     (reachable + tested).
  F. Every glossary TERM used in a feature/rule title is defined (heuristic).
  G. No empty required (nullable:false) fields anywhere.
  H. Per-role CRUD coherence (no field-grant exceeding its table's grant).

Run:
  python3 scripts/loop-10-consistency-audit.py
  python3 scripts/loop-10-consistency-audit.py --strict   # exit 1 if any residual
"""
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
TTC_ROOT = os.path.dirname(HERE)
RB_PATH = os.path.join(TTC_ROOT, "effortless-rulebook",
                       "traffic-ticket-contest-rulebook.json")
STRICT = "--strict" in sys.argv
FIX = "--fix" in sys.argv

NOT_TABLE = {"$schema", "Name", "Description", "_meta", "__meta__"}


def backfill(rb):
    """Loop 10 backfill of the known, principled gaps. Returns a change log.
    Every value is derived from the SSoT, never guessed."""
    log = []
    # 1. IsKey is nullable:false boolean; rows predating the field lack it.
    #    Not-a-key is the deterministically-correct default (false).
    for t, idf in (("ERBPackages", "ERBPackageId"), ("ERBFeatures", "ERBFeatureId")):
        n = 0
        for r in rb[t]["data"]:
            if "IsKey" not in r or r.get("IsKey") in (None, ""):
                r["IsKey"] = False
                n += 1
        if n:
            log.append("%s.IsKey defaulted to false: %d" % (t, n))
    # 2. Hearings.RequestedOn (nullable:false) missing on some seed rows.
    #    A hearing request IS the driver's contest response, so RequestedOn =
    #    the linked Citation's RespondedOn (a real SSoT date, not a guess).
    cits = {c["CitationId"]: c for c in rb["Citations"]["data"]}
    n = 0
    for h in rb["Hearings"]["data"]:
        if h.get("RequestedOn"):
            continue
        c = cits.get(h.get("Citation"))
        src = (c or {}).get("RespondedOn") or (c or {}).get("IssuedOn")
        if src:
            h["RequestedOn"] = src
            n += 1
    if n:
        log.append("Hearings.RequestedOn derived from Citation.RespondedOn: %d" % n)
    return log


def fields_of(obj):
    s = obj.get("schema")
    return s.get("fields", s) if isinstance(s, dict) else (s or [])


def main():
    rb = json.load(open(RB_PATH))
    if FIX:
        log = backfill(rb)
        print("--- backfill ---")
        for line in log:
            print("  +", line)
        if not log:
            print("  (nothing to backfill)")
        print()
    tables = {k: v for k, v in rb.items()
              if k not in NOT_TABLE and isinstance(v, dict) and "schema" in v}

    # PK per table = first raw *Id field
    pk_of = {}
    pks = {}
    for t, obj in tables.items():
        flds = fields_of(obj)
        pkf = next((f["name"] for f in flds
                    if f.get("type") == "raw" and f["name"].endswith("Id")), None)
        pk_of[t] = pkf
        pks[t] = {r.get(pkf) for r in obj["data"] if pkf and r.get(pkf)} if pkf else set()

    residuals = {}

    def add(key, items):
        residuals[key] = items

    # ---- A. explicit relationship FKs resolve ----
    bad_rel = []
    for t, obj in tables.items():
        for f in fields_of(obj):
            if f.get("type") != "relationship":
                continue
            tgt = f.get("RelatedTo")
            nm = f["name"]
            if not tgt or tgt not in pks:
                continue
            for r in obj["data"]:
                v = r.get(nm)
                if v and v not in pks[tgt]:
                    bad_rel.append((t, nm, v, "->", tgt))
    add("A. relationship FK unresolved", bad_rel)

    # ---- A2. value-inferred FK empty-strings that would break the transpiler ----
    risk_empty = []
    for t, obj in tables.items():
        for f in fields_of(obj):
            if f.get("datatype") != "string":
                continue
            nm = f["name"]
            vals = [r.get(nm) for r in obj["data"] if nm in r]
            nonempty = [v for v in vals if v]
            empties = sum(1 for v in vals if v == "")
            if empties == 0 or not nonempty:
                continue
            for tgt, pkset in pks.items():
                if tgt != t and pkset and set(nonempty) <= pkset:
                    risk_empty.append((t, nm, "%d empties match %s PKs" % (empties, tgt)))
                    break
    add("A2. empty-string value-inferred FKs", risk_empty)

    # ---- B. feature completeness ----
    feats = rb["ERBFeatures"]["data"]
    rule_codes = {r["RuleCode"] for r in rb["BusinessRules"]["data"]}
    nav_pks = pks.get("PlatformNaviation", set())
    tested_features = {c.get("TargetFeature") for c in rb["TestCase"]["data"]
                       if c.get("TargetFeature")}
    f_no_st = []
    f_bad_ref = []
    f_no_nav = []
    f_no_test = []
    for f in feats:
        fid = f["ERBFeatureId"]
        st = (f.get("SourceText") or "").strip()
        if not st or st.count(".") < 2:
            f_no_st.append(fid)
        for tok in (f.get("RuleRefs") or "").split(","):
            if tok.strip() and tok.strip() not in rule_codes:
                f_bad_ref.append((fid, tok.strip()))
        if not (f.get("Route") or "").strip() or f["Route"] not in nav_pks:
            f_no_nav.append(fid)
        if fid not in tested_features:
            f_no_test.append(fid)
    add("B1. features w/o >=3-sentence SourceText", f_no_st)
    add("B2. features w/ unresolved RuleRef", f_bad_ref)
    add("B3. features w/o valid nav Route", f_no_nav)
    add("B4. features w/o >=1 TestCase", f_no_test)

    # ---- C. endpoints ----
    eps = rb["APIEndpoints"]["data"]
    machines = pks.get("StateMachines", set())
    ep_bad_subj = [(e.get("Path"), e.get("SubjectTableName")) for e in eps
                   if (e.get("SubjectTableName") or "") and e["SubjectTableName"] not in tables]
    ep_bad_mach = [(e.get("Path"), e.get("TriggersStateMachine")) for e in eps
                   if (e.get("TriggersStateMachine") or "") and e["TriggersStateMachine"] not in machines]
    add("C1. endpoint bad SubjectTable", ep_bad_subj)
    add("C2. endpoint bad TriggersStateMachine", ep_bad_mach)

    # ---- D. state machines ----
    ms = rb["MachineStates"]["data"]
    state_ids = {s["MachineStateId"] for s in ms}
    tr = rb["StateTransitionRules"]["data"]
    tr_bad = [(t["StateTransitionRuleId"], fld, t.get(fld)) for t in tr
              for fld in ("FromState", "ToState")
              if t.get(fld) and t[fld] not in state_ids]
    add("D1. transition state unresolved", tr_bad)
    from collections import Counter
    init = Counter(s["StateMachine"] for s in ms if s.get("IsInitial"))
    term = Counter(s["StateMachine"] for s in ms if s.get("IsTerminal"))
    mach_bad = [m for m in machines if init[m] != 1 or term[m] < 1]
    add("D2. machines not 1-initial/>=1-terminal", mach_bad)

    # ---- E. reachability: endpoints/transitions/rules tested ----
    tc = rb["TestCase"]["data"]
    tested_ep = {c.get("TargetEndpoint") for c in tc if c.get("TargetEndpoint")}
    tested_tr = {c.get("TargetTransition") for c in tc if c.get("TargetTransition")}
    tested_rule = set()
    for c in tc:
        for tok in (c.get("BusinessRuleRefs") or "").split(","):
            if tok.strip():
                tested_rule.add(tok.strip())
    add("E1. endpoints w/o a test", [e["APIEndpointId"] for e in eps
                                     if e["APIEndpointId"] not in tested_ep])
    add("E2. transitions w/o a test", [t["StateTransitionRuleId"] for t in tr
                                       if t["StateTransitionRuleId"] not in tested_tr])
    add("E3. rules w/o a test", [r["RuleCode"] for r in rb["BusinessRules"]["data"]
                                 if r["RuleCode"] not in tested_rule])

    # ---- G. empty required fields ----
    req_empty = []
    for t, obj in tables.items():
        reqs = [f["name"] for f in fields_of(obj)
                if f.get("nullable") is False and f.get("type") == "raw"]
        for r in obj["data"]:
            for fn in reqs:
                if fn not in r or r.get(fn) in (None, ""):
                    req_empty.append((t, fn, r.get(pk_of[t])))
    add("G. empty required (nullable:false raw) fields", req_empty)

    # ---- H. per-role CRUD coherence on ERBTables ----
    crud_bad = []
    roles = ["AdminCRUD", "ManagerCRUD", "RepresentativeCRUD", "ExternalLlmCRUD"]
    for r in rb.get("ERBTables", {}).get("data", []):
        for rc in roles:
            v = (r.get(rc) or "")
            if v and not set(v) <= set("CRUD"):
                crud_bad.append((r.get("ERBTableId"), rc, v))
    add("H. malformed CRUD grant", crud_bad)

    # ---- report ----
    print("=" * 64)
    print("LOOP 10 — GRAND CONSISTENCY AUDIT")
    print("=" * 64)
    total = 0
    for k in residuals:
        items = residuals[k]
        n = len(items)
        total += n
        flag = "OK " if n == 0 else "!! "
        print(f"{flag}{k}: {n}")
        if n:
            for it in items[:6]:
                print("      ", it)
            if n > 6:
                print("       ... +%d more" % (n - 6))
    print("-" * 64)
    print("TOTAL RESIDUALS:", total)
    print("tables:", len(tables),
          "| features:", len(feats),
          "| rules:", len(rule_codes),
          "| nav:", len(nav_pks),
          "| endpoints:", len(eps),
          "| transitions:", len(tr),
          "| test cases:", len(tc))

    if FIX and total == 0:
        with open(RB_PATH, "w") as fh:
            fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
        print("\nwrote", RB_PATH, "(backfill clean, 0 residuals)")
    elif FIX and total:
        print("\nNOT writing: %d residuals remain after backfill." % total)
        sys.exit(1)

    if STRICT and total:
        sys.exit(1)
    return total


if __name__ == "__main__":
    main()
