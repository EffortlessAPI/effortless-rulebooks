"""Two gates.  Every substrate passes the answer key; only the transparent
one passes constructor reflection.  That divergence is the demonstration."""
import json
import os

import engine
import reflection_check
import runtime as transparent_rt
import interpreter


def _expected(root, name):
    with open(os.path.join(root, "r0", "expected", f"{name}.json")) as f:
        return engine.trace_set(json.load(f))


def _conditions(spec, artifact):
    rep = reflection_check.check(spec, artifact)
    return rep, {k[:1]: v["ok"] for k, v in rep["conditions"].items()}


def test_transparent_passes_answer_key(spec, inputs, root, transparent_module):
    for name, inp in inputs.items():
        got = engine.trace_set(transparent_rt.run(transparent_module, inp, choose="first"))
        assert got <= _expected(root, name)
        # and with all successors explored it reproduces the whole permitted set
        assert engine.trace_set(transparent_rt.run(transparent_module, inp, choose=None)) == _expected(root, name)


def test_transparent_passes_reflection(spec, transparent_module):
    rep, c = _conditions(spec, transparent_module)
    assert rep["ok"] and rep["recoverable_G_R"], rep
    assert rep["summary"]["source_nodes"] == rep["summary"]["image_functions"]


def test_interpreter_passes_answer_key_but_fails_reflection(spec, inputs, root):
    spec_path = os.path.join(root, "r0", "spec.json")
    for name, inp in inputs.items():
        assert engine.trace_set(interpreter.run(spec_path, inp, choose="first")) <= _expected(root, name)
    rep, c = _conditions(spec, os.path.join(root, "substrates", "interpreter", "interpreter.py"))
    assert not rep["ok"]
    assert c["1"] is False and c["3"] is False and c["2"] is False and c["6"] is False
    assert rep["summary"]["image_functions"] == 0


def test_tangled_passes_answer_key_but_fails_reflection(spec, inputs, root, tangled_module):
    for name, inp in inputs.items():
        assert engine.trace_set(transparent_rt.run(tangled_module, inp, choose="first")) <= _expected(root, name)
    rep, c = _conditions(spec, tangled_module)
    assert not rep["ok"]
    assert c["1"] is True and c["3"] is True and c["5"] is True, "images exist"
    assert c["2"] is False, "but the dependencies between them are hidden"
    assert c["6"] is False and rep["conditions"]["6_bounded_locality"]["dynamic_dispatch"]


def test_reflection_detects_a_single_hidden_dependency(spec, transparent_module, tmp_path):
    """Mutation: one rule reads a relation from the state dict directly
    instead of calling its component.  Exactly one source edge is lost."""
    src = open(transparent_module).read()
    mutated = src.replace("for (x, z) in c_pred_Reachable(S):", 'for (x, z) in S["Reachable"]:', 1)
    assert mutated != src
    p = tmp_path / "mutated.py"
    p.write_text(mutated)
    rep = reflection_check.check(spec, str(p))
    lost = rep["conditions"]["2_dependency_preservation"]["lost_edges"]
    assert [list(e) for e in lost] == [["pred:Reachable", "rule:reach_step"]]


def test_reflection_detects_an_extra_dependency(spec, transparent_module, tmp_path):
    """Mutation: a rule acquires a read it does not have in the source."""
    src = open(transparent_module).read()
    mutated = src.replace("def c_rule_late(S):\n    out = set()",
                          "def c_rule_late(S):\n    out = set()\n    c_pred_Reachable(S)", 1)
    assert mutated != src
    p = tmp_path / "mutated.py"
    p.write_text(mutated)
    rep = reflection_check.check(spec, str(p))
    extra = rep["conditions"]["6_bounded_locality"]["extra_edges_between_images"]
    assert [list(e) for e in extra] == [["c_rule_late", "c_pred_Reachable"]]


def test_matrix_is_reproducible(root):
    with open(os.path.join(root, "conformance", "matrix.json")) as f:
        m = json.load(f)
    assert m["lint"] is True
    assert m["substrates"]["transparent"]["conformant"] is True
    assert m["substrates"]["tangled"]["conformant"] is False
    assert m["substrates"]["interpreter"]["conformant"] is False
    for row in m["substrates"].values():
        assert row["answer_key_ok"] is True, "all three pass the answer key; that is the point"
