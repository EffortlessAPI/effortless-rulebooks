"""Null-safe ordering comparisons in the Python backend.

The interpreter (_eval_binop) returns False whenever either side of <, <=, >
or >= is None. compile_to_python emitted a bare Python comparison, which
raises TypeError: '>' not supported between instances of 'NoneType' and 'int'
as soon as a nullable numeric column is empty.

Equality is deliberately excluded: `=` and `<>` compare None normally in both
the interpreter and Python, so None = None is True and None <> 1 is True.

Run: python3 -m pytest orchestration/tests/ -v
"""

import sys
from pathlib import Path

import pytest

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
sys.path.insert(0, str(REPO_ROOT))

from orchestration.formula_parser import compile_to_python, evaluate, parse_formula


def py(formula):
    return compile_to_python(parse_formula(formula))


ORDERING = ["<", "<=", ">", ">="]


class TestOrderingComparisonsAreNullSafe:
    @pytest.mark.parametrize("op", ORDERING)
    def test_none_on_left_is_false(self, op):
        code = py(f"={{{{Count}}}} {op} 0")
        assert eval(code, {}, {"count": None}) is False

    @pytest.mark.parametrize("op", ORDERING)
    def test_none_on_right_is_false(self, op):
        code = py(f"=0 {op} {{{{Count}}}}")
        assert eval(code, {}, {"count": None}) is False

    @pytest.mark.parametrize("op", ORDERING)
    def test_both_none_is_false(self, op):
        code = py(f"={{{{Alpha}}}} {op} {{{{Beta}}}}")
        assert eval(code, {}, {"alpha": None, "beta": None}) is False

    def test_real_values_still_compare(self):
        assert eval(py("={{Count}} > 0"), {}, {"count": 1}) is True
        assert eval(py("={{Count}} > 0"), {}, {"count": 0}) is False
        assert eval(py("={{Count}} <= 2"), {}, {"count": 2}) is True

    def test_escalation_violation_idiom(self):
        # Roles.EscalationViolation: a role that fills a gate but delegates
        # to nobody. FillsApprovalGate is a rollup that can be None.
        code = py("=AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}}))")
        assert eval(code, {}, {"fills_approval_gate": None, "delegates_to": ""}) is False
        assert eval(code, {}, {"fills_approval_gate": 1, "delegates_to": ""}) is True
        assert eval(code, {}, {"fills_approval_gate": 1, "delegates_to": "cto"}) is False


class TestEqualityIsUnchanged:
    def test_none_equality_is_not_forced_false(self):
        assert eval(py("={{Alpha}} = {{Beta}}"), {}, {"alpha": None, "beta": None}) is True
        assert eval(py("={{Alpha}} <> 1"), {}, {"alpha": None}) is True


class TestParityWithInterpreter:
    @pytest.mark.parametrize("op", ORDERING)
    @pytest.mark.parametrize("value", [None, 0, 3])
    def test_compiled_matches_interpreter(self, op, value):
        formula = f"={{{{Count}}}} {op} 1"
        assert eval(py(formula), {}, {"count": value}) == evaluate(
            formula, {"Count": value}
        )
