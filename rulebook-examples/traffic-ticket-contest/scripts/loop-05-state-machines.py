#!/usr/bin/env python3
"""
LOOP 5 — Complete all 4 state machines. Adds missing MachineStates, fills out the
StateTransitionRules (every rule has from->to, a guard referencing a BusinessRule,
who may fire it, and the API endpoint that triggers it), and seeds representative
StateTransitions + SubjectStateInstances. Adds two raw schema fields to
StateTransitionRules: TriggerEndpoint (FK-ish to APIEndpoints by id, stored as
text) and RuleRefs (CSV of BusinessRule codes the guard enforces).

Merge-by-id everywhere. Existing states/rules untouched.

  python3 scripts/loop-05-state-machines.py --dry-run
  python3 scripts/loop-05-state-machines.py
"""
import json, os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv
NOW = "2026-06-07T19:15:00Z"
WHO = "admin-example"

# New states to add: (machine, statekey-suffix, Title, OrderIndex, IsInitial, IsTerminal)
NEW_STATES = [
    ("citation-lifecycle", "dismissed", "Dismissed", 5, False, True),
    ("contest-track", "withdrawn", "Withdrawn", 5, False, True),
    ("contest-track", "upheld", "Upheld", 6, False, True),
    ("contest-track", "dismissed", "Dismissed", 7, False, True),
    ("contest-track", "reduced", "Reduced", 8, False, True),
    ("license-track", "pointsassessed", "Points Assessed", 4, False, False),
    ("license-track", "reinstated", "Reinstated", 5, False, True),
]

# New transition rules: (machine, from-suffix, to-suffix, guard, role, endpoint, rulerefs)
NEW_RULES = [
    # citation-lifecycle
    ("citation-lifecycle", "incontest", "dismissed", "Contest succeeds; citation dismissed and points reversed.", "manager", "citation-dismiss", "TT-CONTEST-03,TT-DETERMINATION-02" ),
    ("citation-lifecycle", "adjudicated", "closed", "All consequences (fees, points) settled; case closed.", "manager", "citation-advance-state", "TT-DETERMINATION-01"),
    # contest-track
    ("contest-track", "notcontested", "hearingrequested", "Driver/representative opens a contest within the filing window.", "representative", "citation-contest", "TT-CONTEST-01,TT-DEADLINE-01"),
    ("contest-track", "hearingrequested", "withdrawn", "Representative withdraws the contest before the hearing.", "representative", "citation-withdraw-contest", "TT-CONTEST-01"),
    ("contest-track", "scheduled", "withdrawn", "Contest withdrawn after scheduling but before hearing.", "representative", "citation-withdraw-contest", "TT-CONTEST-01"),
    ("contest-track", "heard", "upheld", "Hearing determination upholds the citation in full.", "manager", "determination-issue", "TT-DETERMINATION-01"),
    ("contest-track", "heard", "dismissed", "Hearing determination dismisses the citation.", "manager", "determination-issue", "TT-DETERMINATION-01,TT-DETERMINATION-02"),
    ("contest-track", "heard", "reduced", "Hearing determination reduces the violation or fine.", "manager", "determination-issue", "TT-DETERMINATION-01,TT-FEE-01"),
    # payment-track
    ("payment-track", "late", "collections", "Payment remains unpaid past the collections threshold.", "manager", "late-penalty-accrue", "TT-PAYMENT-02,TT-DEADLINE-01"),
    ("payment-track", "collections", "paid", "Driver settles the outstanding balance in collections.", "representative", "payment-checkout", "TT-PAYMENT-01"),
    ("payment-track", "due", "paid", "Driver pays in full before the due date.", "representative", "payment-checkout", "TT-PAYMENT-01"),
    ("payment-track", "paid", "notowed", "Citation dismissed after payment; refund issued.", "manager", "payment-refund", "TT-PAYMENT-02,TT-DETERMINATION-02"),
    # license-track
    ("license-track", "valid", "pointsassessed", "Adjudicated citation assesses license points.", "manager", "points-assess", "TT-LICENSE-01"),
    ("license-track", "pointsassessed", "warning", "Accumulated points cross the warning threshold.", "manager", "points-recompute", "TT-LICENSE-01,TT-LICENSE-02"),
    ("license-track", "warning", "suspended", "Accumulated points cross the suspension threshold.", "admin", "license-suspension-check", "TT-LICENSE-02"),
    ("license-track", "suspended", "reinstated", "Points reduced (dismissal/time) below threshold; license reinstated.", "admin", "points-recompute", "TT-LICENSE-02"),
]

# Representative StateTransitions (logged examples): (machine, subjtable, subjid, from-suffix, to-suffix, role, reason)
NEW_TRANSITIONS = [
    ("contest-track", "Citations", "tc-2026-0002", "notcontested", "hearingrequested", "representative", "Representative filed a contest on signage grounds."),
    ("contest-track", "Citations", "tc-2026-0002", "hearingrequested", "scheduled", "manager", "Hearing scheduled for the contested citation."),
    ("payment-track", "Citations", "tc-2026-0001", "due", "paid", "representative", "Driver paid the fine in full."),
    ("license-track", "Drivers", "d1234567", "valid", "pointsassessed", "manager", "Points assessed for an adjudicated citation."),
]

# Current-state instances: (subjtable, subjid, machine, statekey-suffix, seq)
NEW_INSTANCES = [
    ("Citations", "tc-2026-0002", "contest-track", "scheduled", 2),
    ("Citations", "tc-2026-0001", "payment-track", "paid", 1),
    ("Drivers", "d1234567", "license-track", "pointsassessed", 1),
]


def audit(row):
    row.setdefault("CreatedAt", NOW); row.setdefault("CreatedBy", WHO)
    row.setdefault("ModifiedAt", NOW); row.setdefault("ModifiedBy", WHO)
    return row


def add_raw_field(table_obj, name, datatype, desc):
    sch = table_obj["schema"]; fs = sch["fields"] if isinstance(sch, dict) else sch
    if any(f.get("name") == name for f in fs):
        return False
    field = {"name": name, "datatype": datatype, "type": "raw", "nullable": True, "Description": desc}
    idx = next((i for i, f in enumerate(fs) if f.get("name") == "GuardDescription"), len(fs))
    fs.insert(idx + 1, field)
    return True


# Determination + state rules that the lifecycles reference but Loop 2 didn't create.
# (machine area is DETERMINATION / STATE; category 'rules-engine' exists.)
SUPP_RULES = [
    ("TT-DETERMINATION-01", "Determination is recorded", "rules-engine",
     "A hearing determination (upheld / dismissed / reduced) must be recorded on the Citation before the citation-lifecycle may advance to adjudicated, and it writes a CaseEvents entry.",
     "Citations.DeterminationResult"),
    ("TT-DETERMINATION-02", "Dismissal reverses consequences", "rules-engine",
     "When a determination dismisses a Citation, any assessed license points are reversed and any paid amount is refunded, restoring the Driver to make-whole state.",
     "Citations.AmountDue"),
    ("TT-STATE-01", "Lifecycle transitions are guarded", "platform",
     "Every state transition is gated by its guard condition and the firing role; the state-machine backbone rejects illegal transitions and logs each accepted one to CaseEvents.",
     "StateTransitionRules.GuardDescription"),
]


def ensure_supp_rules(rb):
    rules = rb["BusinessRules"]["data"]
    cats = {c["BusinessRuleCategoryId"] for c in rb["BusinessRuleCategories"]["data"]}
    have = {r["RuleCode"] for r in rules if r.get("RuleCode")}
    added = 0
    base_sort = max((r.get("SortOrder") or 0) for r in rules) + 1
    for i, (code, title, cat, desc, loc) in enumerate(SUPP_RULES):
        if code in have:
            continue
        catid = cat if cat in cats else "rules-engine"
        rules.append(audit({
            "BusinessRuleId": code, "RuleCode": code, "Title": title,
            "Category": catid, "SortOrder": base_sort + i,
            "Description": desc, "SchemaLocation": loc,
        }))
        have.add(code); added += 1
    return added


def main():
    rb = json.load(open(RB))
    supp = ensure_supp_rules(rb)
    rule_codes = {r["RuleCode"] for r in rb["BusinessRules"]["data"] if r.get("RuleCode")}
    ep_ids = {e["APIEndpointId"] for e in rb["APIEndpoints"]["data"]}
    machines = {s.get("StateMachineId") for s in rb["StateMachines"]["data"]}

    # schema: add TriggerEndpoint + RuleRefs to StateTransitionRules
    sfa = add_raw_field(rb["StateTransitionRules"], "TriggerEndpoint", "string", "APIEndpoints id whose action fires this transition.")
    sfb = add_raw_field(rb["StateTransitionRules"], "RuleRefs", "string", "CSV of BusinessRule codes the guard enforces.")

    states = rb["MachineStates"]["data"]
    have_state = {s["MachineStateId"] for s in states}
    sadded = 0
    for (m, suf, title, order, ini, term) in NEW_STATES:
        sid = f"{m}--{suf}"
        if sid in have_state:
            continue
        states.append(audit({
            "MachineStateId": sid, "StateMachine": m, "StateKey": suf,
            "Title": title, "OrderIndex": order, "IsInitial": ini, "IsTerminal": term,
        }))
        have_state.add(sid); sadded += 1

    rules = rb["StateTransitionRules"]["data"]
    have_rule = {r["StateTransitionRuleId"] for r in rules}
    radded = 0
    for (m, fs_, ts_, guard, role, ep, refs) in NEW_RULES:
        rid = f"{m}--{fs_}->{ts_}"
        if rid in have_rule:
            continue
        rules.append(audit({
            "StateTransitionRuleId": rid, "StateMachine": m,
            "FromState": f"{m}--{fs_}", "ToState": f"{m}--{ts_}",
            "GuardDescription": guard, "TriggeredByRole": role,
            "TriggerEndpoint": ep, "RuleRefs": refs,
        }))
        have_rule.add(rid); radded += 1

    trans = rb["StateTransitions"]["data"]
    have_tr = {t["StateTransitionId"] for t in trans}
    tadded = 0
    for i, (m, subt, subid, fs_, ts_, role, reason) in enumerate(NEW_TRANSITIONS):
        tid = f"{m}--{subid}--{fs_}->{ts_}"
        if tid in have_tr:
            continue
        trans.append(audit({
            "StateTransitionId": tid, "SubjectTableName": subt, "SubjectId": subid,
            "FromStateKey": fs_, "ToStateKey": ts_,
            "TransitionAt": NOW, "TriggeredByRole": role, "Reason": reason,
        }))
        have_tr.add(tid); tadded += 1

    insts = rb["SubjectStateInstances"]["data"]
    have_inst = {i["SubjectStateInstanceId"] for i in insts}
    iadded = 0
    for (subt, subid, m, suf, seq) in NEW_INSTANCES:
        iid = f"{subid}--{m}--{suf}"
        if iid in have_inst:
            continue
        insts.append(audit({
            "SubjectStateInstanceId": iid, "SubjectTableName": subt, "SubjectId": subid,
            "StateKey": suf, "EnteredAt": NOW, "ExitedAt": None, "SequenceIndex": seq,
        }))
        have_inst.add(iid); iadded += 1

    # consistency
    bad = []
    for r in rules:
        if r.get("FromState") and r["FromState"] not in have_state:
            bad.append(("rule.FromState", r["StateTransitionRuleId"], r["FromState"]))
        if r.get("ToState") and r["ToState"] not in have_state:
            bad.append(("rule.ToState", r["StateTransitionRuleId"], r["ToState"]))
        if r.get("TriggerEndpoint") and r["TriggerEndpoint"] not in ep_ids:
            bad.append(("rule.TriggerEndpoint", r["StateTransitionRuleId"], r["TriggerEndpoint"]))
        for c in (r.get("RuleRefs") or "").split(","):
            if c.strip() and c.strip() not in rule_codes:
                bad.append(("rule.RuleRefs", r["StateTransitionRuleId"], c.strip()))
    # each machine: exactly one initial, >=1 terminal
    for m in machines:
        ms = [s for s in states if s.get("StateMachine") == m]
        ini = sum(1 for s in ms if s.get("IsInitial"))
        term = sum(1 for s in ms if s.get("IsTerminal"))
        if ini != 1 or term < 1:
            bad.append(("machine.init/term", m, f"init={ini} term={term}"))

    print("=== LOOP 5: state machines ===")
    print("supplemental rules added:", supp)
    print("schema fields added:", int(sfa) + int(sfb))
    print("states +%d (%d) | rules +%d (%d) | transitions +%d (%d) | instances +%d (%d)" % (
        sadded, len(states), radded, len(rules), tadded, len(trans), iadded, len(insts)))
    for m in sorted(machines):
        ms = [s for s in states if s.get("StateMachine") == m]
        rs = [r for r in rules if r.get("StateMachine") == m]
        print(f"   {m}: {len(ms)} states, {len(rs)} rules")
    print("consistency unresolved:", len(bad), bad[:10])
    if bad:
        print("\nABORT."); sys.exit(1)
    if DRY:
        print("\n[dry-run] no write"); return
    with open(RB, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB)


if __name__ == "__main__":
    main()
