"""ISBLANK support in the Python and Go formula backends.

ISBLANK was implemented only in the runtime interpreter (_eval_func), not in
compile_to_python or compile_to_go. Any rulebook using it produced a generated
function whose body was `raise NotImplementedError` with an empty parameter
list, so the substrate crashed rather than scoring.

The semantics match what rulebook-to-postgres emits — `IS NULL OR ::text = ''`
— so a NULL and an empty string are both blank. The transpiler stores absent
relationships as '' rather than NULL, so both must count.

Run: python3 -m pytest orchestration/tests/ -v
"""

import sys
from pathlib import Path

import pytest

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
sys.path.insert(0, str(REPO_ROOT))

from orchestration.formula_parser import (
    compile_to_go,
    compile_to_python,
    parse_formula,
)


def py(formula):
    return compile_to_python(parse_formula(formula))


def go(formula):
    return compile_to_go(parse_formula(formula))


class TestPythonBackend:
    def test_isblank_compiles(self):
        assert py("=ISBLANK({{ValidTo}})")

    def test_isblank_treats_none_and_empty_as_blank(self):
        code = py("=ISBLANK({{ValidTo}})")
        assert eval(code, {}, {"valid_to": None}) is True
        assert eval(code, {}, {"valid_to": ""}) is True
        assert eval(code, {}, {"valid_to": "2026-03-01"}) is False

    def test_not_isblank_is_a_presence_check(self):
        code = py("=NOT(ISBLANK({{GateApproverHuman}}))")
        assert eval(code, {}, {"gate_approver_human": ""}) is False
        assert eval(code, {}, {"gate_approver_human": "alice"}) is True

    def test_isblank_inside_and(self):
        code = py("=AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}}))")
        ctx = {"requires_human_approval": True, "executing_human_agent": ""}
        assert eval(code, {}, ctx) is True
        ctx = {"requires_human_approval": True, "executing_human_agent": "bob"}
        assert eval(code, {}, ctx) is False

    def test_isblank_inside_if_chain(self):
        code = py(
            '=IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", '
            'IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", ""))'
        )
        base = {"filled_by_human_agent": "", "filled_by_ai_agent": ""}
        assert eval(code, {}, {**base, "filled_by_human_agent": "amy"}) == "HumanAgent"
        assert eval(code, {}, {**base, "filled_by_ai_agent": "gpt"}) == "AIAgent"
        assert eval(code, {}, base) == ""

    def test_requires_exactly_one_argument(self):
        with pytest.raises(ValueError, match="ISBLANK requires 1 argument"):
            py("=ISBLANK()")


class TestGoBackend:
    def test_isblank_compiles(self):
        assert go("=ISBLANK({{ValidTo}})")

    def test_isblank_checks_both_nil_and_empty(self):
        # Go models absent scalars as pointers, so a blank check must cover
        # the nil pointer and the empty string it points at.
        code = go("=ISBLANK({{ValidTo}})")
        assert "== nil" in code
        assert '""' in code

    def test_not_isblank_negates(self):
        code = go("=NOT(ISBLANK({{GateApproverHuman}}))")
        assert code.startswith("!(") or code.startswith("(!")

    def test_requires_exactly_one_argument(self):
        with pytest.raises(ValueError, match="ISBLANK requires 1 argument"):
            go("=ISBLANK()")


class TestParityWithInterpreter:
    """The compiled backends must agree with the runtime interpreter."""

    @pytest.mark.parametrize("value,expected", [(None, True), ("", True), ("x", False)])
    def test_python_matches_interpreter(self, value, expected):
        from orchestration.formula_parser import evaluate

        # evaluate() resolves {{ValidTo}} against PascalCase keys; the
        # compiled backend binds the snake_case local it emits.
        assert evaluate("=ISBLANK({{ValidTo}})", {"ValidTo": value}) == expected
        assert eval(py("=ISBLANK({{ValidTo}})"), {}, {"valid_to": value}) == expected
