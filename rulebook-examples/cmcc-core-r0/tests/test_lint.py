"""r0-lint decides membership in R0 (paper section 21's restrictions)."""
import copy
import json
import os

import r0_lint


def test_spec_is_in_r0(spec):
    cert = r0_lint.lint(spec)
    assert cert["ok"], cert["violations"]
    assert any(s["recursive"] for s in cert["strata"]), "the spec must exercise recursion"


def test_value_generation_in_recursion_is_rejected(root):
    with open(os.path.join(root, "r0", "not-r0.json")) as f:
        cert = r0_lint.lint(json.load(f))
    assert not cert["ok"]
    assert {v[0] for v in cert["violations"]} == {"VG"}


def test_negation_inside_recursion_is_unstratifiable(spec):
    bad = copy.deepcopy(spec)
    bad["rules"].append({"id": "cycle", "head": ["Reachable", "x", "y"],
                         "body": [{"atom": ["Warehouse", "x"]}, {"atom": ["Warehouse", "y"]},
                                  {"not": ["Reachable", "y", "x"]}]})
    cert = r0_lint.lint(bad)
    assert not cert["ok"]
    assert any(v[0] == "ST" for v in cert["violations"])


def test_aggregate_over_own_stratum_is_unstratifiable(spec):
    bad = copy.deepcopy(spec)
    bad["rules"].append({"id": "selfagg", "head": ["Reachable", "x", "n"],
                         "agg": {"op": "COUNT", "expr": "y", "keys": [{"atom": ["Warehouse", "x"]}],
                                 "over": [{"atom": ["Reachable", "x", "y"]}]}})
    cert = r0_lint.lint(bad)
    assert any(v[0] == "ST" for v in cert["violations"])


def test_range_restriction_violation(spec):
    bad = copy.deepcopy(spec)
    bad["rules"].append({"id": "unsafe", "head": ["Big", "z"], "body": [{"atom": ["Warehouse", "x"]}]})
    cert = r0_lint.lint(bad)
    assert any(v[0] == "RR" and v[1] == "unsafe" for v in cert["violations"])


def test_append_only_history_sequence(spec):
    bad = copy.deepcopy(spec)
    bad["h0"][3]["n"] = 1
    cert = r0_lint.lint(bad)
    assert any(v[0] == "AO" for v in cert["violations"])
