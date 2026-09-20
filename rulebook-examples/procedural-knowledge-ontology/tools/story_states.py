#!/usr/bin/env python3
"""Play the whole video series through the running app, as each person, in episode order.

One script, two uses:

  --check     the end-to-end test that everything a person does on camera is still doable,
              that each write moves the derived value the episode says it moves, and that
              the writes the series says are REFUSED really are refused by the database.
  (default)   the same run, also writing each watched value to story-states/NN-name.json,
              which the video stage reads so that no frame can disagree with the app.

It needs a freshly reset database (`bash init-db.sh`) and the API running. It talks only to
the HTTP API, so it exercises sign-in, the role schema, RLS and the column grants exactly as
the app does. Nothing is computed here: every value asserted is read back from a view.

  API=http://localhost:8099 python3 tools/story_states.py --check

State 10 (episode 2's on-camera formula edit, 120 to 60 and back) is a model change and
needs a build, so it is not replayed here; do it by hand with tools/quick_build.sh.
"""
import json
import os
import sys
import urllib.error
import urllib.request

API = os.environ.get("API", "http://localhost:8099")
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(os.path.dirname(HERE), "story-states")
CHECK = "--check" in sys.argv
failures, states = [], []


def call(method, path, token=None, body=None):
    req = urllib.request.Request(API + path, method=method, data=json.dumps(body).encode() if body is not None else None,
                                 headers={"content-type": "application/json", **({"authorization": f"Bearer {token}"} if token else {})})
    try:
        with urllib.request.urlopen(req) as r:
            return r.status, json.loads(r.read())
    except urllib.error.HTTPError as e:
        return e.code, json.loads(e.read() or b"{}")


def sign_in(agent, role):
    status, out = call("POST", "/api/auth/sign-in", body={"appUserId": f"user-{agent}", "principalId": f"principal-{role}"})
    if status != 200:
        raise SystemExit(f"FATAL: cannot sign in as {agent}/{role}: {out}")
    return out["token"]


def rows(token, table, **filters):
    q = "&".join(f"{k}={str(v).lower() if isinstance(v, bool) else v}" for k, v in filters.items())
    status, out = call("GET", f"/api/app/rows/{table}" + (f"?{q}" if q else ""), token)
    if status != 200:
        raise SystemExit(f"FATAL: {table}: {out}")
    return out["rows"]


def state(n, name, who, what, expect_before, expect_after, result):
    status, out = result
    w = (out.get("watched") or {}) if status == 200 else {}
    ok = status == 200 and w.get("before") == expect_before and w.get("after") == expect_after
    line = f"{n:>3}  {who:<16} {what:<52} {w.get('field', '?')}: {w.get('before')} -> {w.get('after')}"
    print(("  ok " if ok else "FAIL ") + line + ("" if ok else f"   expected {expect_before} -> {expect_after}; got HTTP {status} {out.get('detail', '')}"))
    if not ok:
        failures.append(line)
    states.append({"state": n, "name": name, "who": who, "action": what, "http": status, "result": out})
    return out


def refused(n, who, what, result, want_status, want_error):
    status, out = result
    ok = status == want_status and out.get("error") == want_error
    print(("  ok " if ok else "FAIL ") + f"{n:>3}  {who:<16} {what:<52} refused: {out.get('error')} ({status})")
    if not ok:
        failures.append(f"{n} {what}: expected {want_status} {want_error}, got {status} {out}")
    states.append({"state": n, "name": "refused", "who": who, "action": what, "http": status, "result": out})


def main():
    sam = sign_in("sam-adeyemi", "knowledge-engineer")
    aisha = sign_in("aisha-bello", "maintenance-technician")
    ken = sign_in("ken-watanabe", "maintenance-technician")
    lin = sign_in("lin-zhao", "plant-safety-officer")
    hana = sign_in("hana-kowalski", "plant-operations-manager")
    claire = sign_in("claire-dubois", "sourcing-manager")
    nadia = sign_in("nadia-petrova", "ontology-authority")
    elena = sign_in("elena-garcia", "process-steward")
    act = lambda tok, a, **body: call("POST", f"/api/app/action/{a}", tok, body)

    # ---- 00: the story's starting point --------------------------------------------------------
    k = rows(sam, "know_how_carriers", know_how_carrier_id="khc12-tomas-bleed")[0]
    start_ok = k["is_at_risk_of_imminent_loss"] is True and k["days_until_holder_departure"] == 73 and k["repository_entry_count"] == 0
    print(("  ok " if start_ok else "FAIL ") + f" 00  {'(fresh database)':<16} {'Tomas: at risk, 73 days, nothing written down':<52} "
          f"at_risk={k['is_at_risk_of_imminent_loss']} days={k['days_until_holder_departure']} entries={k['repository_entry_count']}")
    if not start_ok:
        failures.append("00: not a freshly reset database. Run `bash init-db.sh` first.")
    states.append({"state": 0, "name": "fresh", "row": k})

    # ---- episode 2: Sam ----------------------------------------------------------------------
    state(11, "written-down", "Sam", "Write this down (Tomas, accumulator bleed)", True, False,
          act(sam, "act-ke-write-down", watch="khc12-tomas-bleed", context={"KnowHow": "khc12-tomas-bleed", "Procedure": "lockout-tagout"},
              values={"Title": "Hearing the press 7 accumulator bleed down", "SourceExpert": "tomas-reyes", "CreditsSourceExpert": True,
                      "WrittenForAudience": "Maintenance technicians", "AuthoredOnAllocatedTime": True}))
    state(12, "handed-over", "Sam", "Record a hand-over, Tomas to Aisha", 0, 1,
          act(sam, "act-ke-hand-over", watch="khc12-tomas-bleed",
              context={"KnowHow": "khc12-tomas-bleed", "FromAgent": "tomas-reyes", "CommunityOfPractice": "plant-maintenance-guild"},
              values={"RecipientAgent": "aisha-bello", "Channel": "Mentoring", "OnAllocatedTime": True}))

    # ---- episode 3: Claire -------------------------------------------------------------------
    state(20, "clause", "Claire", "Meridian: knowledge access clause on", True, False,
          act(claire, "act-sourcing-set-clause", key="pe-hb-meridian", watch="pe-hb-meridian", values={"HasKnowledgeAccessClause": True}))
    state(21, "deliverable", "Claire", "Meridian: require a deliverable", True, False,
          act(claire, "act-sourcing-require-deliverable", watch="pe-hb-meridian", context={"ProviderEngagement": "pe-hb-meridian"},
              values={"Title": "Assembly sequence and brazing parameters for the countertop range", "DueAt": "2026-10-31T17:00"}))
    refused(22, "Claire", "edit the PLANT's Baxter contract",
            act(claire, "act-sourcing-set-clause", key="pe-plant-baxter", values={"HasKnowledgeAccessClause": False}), 403, "refused_by_row_policy")

    # ---- episode 4: Aisha, the Copilot, Lin -----------------------------------------------------
    status, run = act(aisha, "act-tech-start-lockout", values={"ExecutedOnMachine": "conveyor-3", "Facility": "plant-north", "Shift": "Day"})
    if status != 200:
        raise SystemExit(f"FATAL: start lockout: {run}")
    run_id = run["key"]
    states.append({"state": 30, "name": "run-started", "who": "Aisha", "result": run})
    print(f"  ok  30  {'Aisha':<16} {'Start a lockout on conveyor line 3':<52} run {run_id}")
    se = None
    for step in ["loto-01", "loto-02", "loto-03", "loto-04", "loto-04a", "loto-04b", "loto-04c", "loto-05", "loto-06"]:
        if se:
            state(31, f"done-{step}", "Aisha", f"Done ({se[1]})", False, True,
                  act(aisha, "act-tech-complete-step", key=se[0], watch=se[0], context={"VerificationResult": "PASS"}))
        status, out = act(aisha, "act-tech-begin-step", context={"ProcedureExecution": run_id, "Step": step})
        if status != 200:
            raise SystemExit(f"FATAL: begin {step}: {out}")
        se = (out["key"], step)
    status, before = call("POST", "/api/app/copilot/ask", aisha, {"questionId": "next", "stepId": "loto-06", "stepExecutionId": se[0]})
    obs = state(32, "cue-seen", "Aisha", "I see this (gauge needle above zero)", False, True,
                act(aisha, "act-tech-observe-cue", watch=se[0], context={"StepExecution": se[0], "StepCue": "cue-loto06-gauge"}))
    status, after = call("POST", "/api/app/copilot/ask", aisha, {"questionId": "next", "stepId": "loto-06", "stepExecutionId": se[0]})
    status, gauge = call("POST", "/api/app/copilot/ask", aisha, {"questionId": "cue:cue-loto06-gauge", "stepId": "loto-06", "stepExecutionId": se[0]})
    ok = before.get("next") == "loto-07" and after.get("next") == "loto-09" and gauge.get("next") == "loto-09" and len(gauge.get("groundings", [])) == 3
    print(("  ok " if ok else "FAIL ") + f" 32  {'Copilot':<16} {'next step before / after the warning sign':<52} {before.get('next')} -> {after.get('next')}; "
          f"gauge answer cites {len(gauge.get('groundings', []))} rows")
    if not ok:
        failures.append(f"32 copilot: {before} / {after} / {gauge}")
    states.append({"state": 32, "name": "copilot", "before": before, "after": after, "gauge": gauge})
    state(33, "escalated", "Aisha", "Escalate", False, True, act(aisha, "act-tech-escalate", key=obs["key"], watch=obs["key"]))
    state(34, "acknowledged", "Lin", "Acknowledge", True, False, act(lin, "act-safety-acknowledge", key=obs["key"], watch=obs["key"]))
    refused(35, "Aisha", "Sam's action (write this down)",
            act(aisha, "act-ke-write-down", context={"KnowHow": "khc12-tomas-setup", "Procedure": "lockout-tagout"}, values={"Title": "x"}), 403, "not_your_action")
    finance = call("GET", "/api/app/rows/exceptions", aisha)
    refused(36, "Aisha", "read a finance table (exceptions)", finance, 404, "not_in_your_schema")

    # ---- episode 5: the change, the authority, the last day -------------------------------------
    cr = "cr-enc-loto2-gauge-tap"
    refused(40, "Lin", "decide a change request they raised themselves",
            act(lin, "act-safety-decide", key=cr, values={"Status": "Approved"}), 403, "refused_by_database")
    state(41, "handed-up", "Lin", "Hand the decision up to the operations manager", True, False,
          act(lin, "act-safety-hand-up", key=cr, watch=cr, values={"AuthorityRole": "plant-operations-manager"}))
    state(42, "decided", "Hana", "Decide: approved", False, True, act(hana, "act-floor-decide", key=cr, watch=cr, values={"Status": "Approved"}))
    act(sam, "act-ke-add-warning-sign", context={"Step": "loto-06"},
        values={"CueKind": "Indicator", "Description": "Tap the gauge glass before trusting a reading just above zero.",
                "SignalsIncompleteStep": False, "RequiresEscalation": False, "EscalateToRole": "plant-safety-officer"})
    state(43, "implemented", "Sam", "Mark implemented (after adding the tap to step 06)", True, False,
          act(sam, "act-ke-mark-implemented", key=cr, watch=cr))
    state(44, "reviewed", "Nadia", "Record authority review on mcr-13", True, False,
          act(nadia, "act-authority-record-review", key="mcr-13", watch="mcr-13"))
    state(50, "last-day", "Elena (admin)", "Move the date to Tomas's last day", 73, 0,
          act(elena, "act-admin-move-instant", key="eval-current", watch="khc12-tomas-bleed", values={"AsOfInstant": "2026-09-30T17:00:00-05:00"}))
    at_risk = rows(sam, "know_how_carriers", is_at_risk_of_imminent_loss=True)
    ok = len(at_risk) == 0
    print(("  ok " if ok else "FAIL ") + f" 51  {'Sam':<16} {'skills at risk of imminent loss on the last day':<52} {len(at_risk)}")
    if not ok:
        failures.append(f"51: still at risk on the last day: {[r['know_how_carrier_id'] for r in at_risk]}")
    states.append({"state": 51, "name": "nothing-at-risk", "rows": at_risk})

    # ---- "Everything Says Pass" (series 18): the control nobody ever asked ----------------------
    # The zero-energy control reads Inoperative with six PASS marks on its step and a failed check of
    # the condition it enforces. Lin evaluates it against Ken's July 9 run (NotSatisfied), which makes
    # it Asserted; Sam names the column that computes its breach, which makes it Demonstrated. Only
    # the officer may evaluate a control, and she may not name its witness.
    z = "req-loto-zero-energy"
    refused(60, "Sam", "evaluate a control (the safety officer's job)",
            act(sam, "act-safety-evaluate-control", context={"Requirement": z, "StepExecution": "se-loto0709-06"},
                values={"SatisfactionLevel": "Satisfied", "Evidence": "x"}), 403, "not_your_action")
    state(61, "control-evaluated", "Lin", "Evaluate the zero-energy control: it did not hold", True, False,
          act(lin, "act-safety-evaluate-control", watch=z, context={"Requirement": z, "StepExecution": "se-loto0709-06"},
              values={"SatisfactionLevel": "NotSatisfied",
                      "Evidence": "Hiss after the pneumatic valve closed at 04b. At 1:00 AM the check that zero energy "
                                  "had been verified did not hold, and maintenance went ahead."}))
    state(62, "control-witnessed", "Sam", "Name the column that computes the zero-energy control", "Asserted", "Demonstrated",
          act(sam, "act-ke-name-witness", key=z, watch=z, values={"WitnessFieldName": "StepExecutions.ProceededDespiteFailedPrecondition"}))

    # ---- series 18, video 1: the proof run. A no cannot become a done (loop 20) -------------------
    status, run2 = act(ken, "act-tech-start-lockout", values={"ExecutedOnMachine": "press-7", "Facility": "plant-south", "Shift": "Night"})
    if status != 200:
        raise SystemExit(f"FATAL: Ken start lockout: {run2}")
    run2_id = run2["key"]
    states.append({"state": 70, "name": "proof-run-started", "who": "Ken", "result": run2})
    print(f"  ok  70  {'Ken':<16} {'Start a lockout on press 7, again':<52} run {run2_id}")
    se = None
    for step in ["loto-01", "loto-02", "loto-03", "loto-04", "loto-04a", "loto-04b", "loto-04c", "loto-05", "loto-06", "loto-07"]:
        if se:
            state(71, f"proof-done-{step}", "Ken", f"Done ({se[1]})", False, True,
                  act(ken, "act-tech-complete-step", key=se[0], watch=se[0], context={"VerificationResult": "PASS"}))
        status, out = act(ken, "act-tech-begin-step", context={"ProcedureExecution": run2_id, "Step": step})
        if status != 200:
            raise SystemExit(f"FATAL: Ken begin {step}: {out}")
        se = (out["key"], step)
    chk = state(72, "check-said-no", "Ken", "The zero-energy check on step 07: no, it does not hold", False, True,
                act(ken, "act-tech-check-condition", watch=se[0], context={"StepExecution": se[0], "StepCondition": "cond-loto07-pre-zeroenergy"},
                    values={"Held": False}))
    scored = rows(lin, "condition_checks", is_scored_breach=True)
    ok = chk.get("key") in {r["condition_check_id"] for r in scored} and "cc-0709-07-zero" in {r["condition_check_id"] for r in scored}
    print(("  ok " if ok else "FAIL ") + f" 73  {'Lin':<16} {'Rules broken, by the register itself: July 9 and today':<52} {len(scored)} scored breach(es)")
    if not ok:
        failures.append("73 scored breaches")
    state(74, "proof-way-out", "Ken", "Fallback: stop, keep the locks on, escalate (WARN)", False, True,
          act(ken, "act-tech-complete-step", key=se[0], watch=se[0], context={"VerificationResult": "WARN"}))
    status, out = act(ken, "act-tech-begin-step", context={"ProcedureExecution": run2_id, "Step": "loto-09"})
    if status != 200:
        raise SystemExit(f"FATAL: Ken begin loto-09: {out}")

    if not CHECK:
        os.makedirs(OUT, exist_ok=True)
        for s in states:
            json.dump(s, open(os.path.join(OUT, f"{s['state']:02d}-{s['name']}.json"), "w"), indent=1, ensure_ascii=False, default=str)
        print(f"wrote {len(states)} states to {OUT}")
    print(f"\n{len(failures)} failure(s)")
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())
