"""The reference evaluator, the SQLite oracle, and the committed expected
trace sets agree; and the traces show the behaviours the paper's proof
depends on."""
import json
import os

import engine
import reference_eval as R
import sqlite_oracle as O
import gen_expected


def _run(spec, inp, derive, observer=None):
    return engine.run(engine.SpecModel(spec, derive, observer), inp)


def test_reference_and_oracle_agree_on_every_visited_state(spec, inputs):
    visited = []
    for inp in inputs.values():
        _run(spec, inp, R.derived_state, lambda h, now, cmd, D: visited.append((h, now, cmd, D)))
    assert len(visited) > 30
    for hist, now, cmd, D in visited:
        D2 = O.derived_state(spec, hist, now, cmd)
        for p in D:
            if p.startswith("Hist_") or p.startswith("Cmd_") or p == "Now":
                continue
            assert D[p] == D2[p], (p, now, cmd)


def test_oracle_driven_run_gives_same_trace_sets(spec, inputs):
    for inp in inputs.values():
        assert engine.trace_set(_run(spec, inp, R.derived_state)) == engine.trace_set(_run(spec, inp, O.derived_state))


def test_committed_expected_files_are_current(spec, inputs, root):
    for name, inp in inputs.items():
        with open(os.path.join(root, "r0", "expected", f"{name}.json")) as f:
            committed = engine.trace_set(json.load(f))
        assert committed == engine.trace_set(gen_expected.expected_for(spec, inp))


def test_nondeterministic_command_yields_two_permitted_traces(spec, inputs):
    res = _run(spec, inputs["i2-bitemporal"], R.derived_state)
    assert len(res["traces"]) == 2
    finals = {json.dumps(t[-1]["obs"]["reachable"]) for t in res["traces"]}
    assert len(finals) == 2, "the two successors must be observably different"


def test_constraint_rejections(spec, inputs):
    t = _run(spec, inputs["i1-routes"], R.derived_state)["traces"][0]
    assert t[2]["outcome"] == "rejected(constraint:SelfRoute)"
    assert t[3]["outcome"] == "rejected(constraint:NegativeCap)"
    assert t[3]["obs"] == t[1]["obs"], "a rejected transition leaves the state unchanged"


def test_empty_group_aggregate_semantics(spec, inputs):
    obs = _run(spec, inputs["i1-routes"], R.derived_state)["traces"][0][0]["obs"]
    assert ["depot", 0] in obs["fanout"], "COUNT over an empty group is 0 (section 5)"
    assert not any(row[0] == "depot" for row in obs["maxcap"]), "MAX over an empty group derives nothing"


def test_retraction_removes_tuple_and_recomputes_recursion(spec, inputs):
    t = _run(spec, inputs["i1-routes"], R.derived_state)["traces"][0]
    assert ["hub", "b"] in t[4]["obs"]["reachable"]
    assert ["hub", "b"] not in t[6]["obs"]["reachable"]
    assert ["b"] in t[6]["obs"]["isolated"]


def test_bitemporal_correction_is_visible_by_valid_and_knowledge_time(spec, inputs):
    t = _run(spec, inputs["i2-bitemporal"], R.derived_state)["traces"][0]
    assert t[1]["obs"]["shipment"] == [], "valid 2, known 4: corrected record starts at valid 3"
    assert t[2]["obs"]["shipment"] == [["s1", "a", 12]], "valid 5, known 4: the correction"
    assert t[3]["obs"]["shipment"] == [["s1", "a", 8]], "valid 5, as known at 1: the original"
    assert t[3]["obs"]["known_at"] == [["s1", 8, 0], ["s1", 12, 4]], "knowledge history is never destroyed"
