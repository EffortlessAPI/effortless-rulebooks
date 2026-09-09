"""Arithmetic operators in the Python and Go formula backends.

compile_to_python and compile_to_go mapped only comparison operators, so any
formula adding, subtracting, multiplying or dividing raised KeyError('+') at
generation time and produced a function whose body was `raise
NotImplementedError`. The COBOL backend and the runtime interpreter both
already handled arithmetic.

The counting idiom this unblocks is common in Talisman:

    =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0)
   + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0)

None is treated as 0, matching the interpreter (_eval_binop).

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
    evaluate,
    parse_formula,
)


def py(formula):
    return compile_to_python(parse_formula(formula))


def go(formula):
    return compile_to_go(parse_formula(formula))


class TestPythonArithmetic:
    @pytest.mark.parametrize(
        "formula,expected",
        [
            ("=1 + 2", 3),
            ("=5 - 3", 2),
            ("=4 * 3", 12),
            ("=10 / 4", 2.5),
        ],
    )
    def test_literal_arithmetic(self, formula, expected):
        assert eval(py(formula), {}, {}) == expected

    def test_field_arithmetic(self):
        code = py("={{Alpha}} + {{Beta}}")
        assert eval(code, {}, {"alpha": 2, "beta": 5}) == 7

    def test_counting_idiom_over_if_calls(self):
        code = py(
            "=IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0)"
            " + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0)"
            " + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0)"
        )
        blank = {
            "filled_by_human_agent": "",
            "filled_by_ai_agent": "",
            "filled_by_automated_pipeline": "",
        }
        assert eval(code, {}, blank) == 0
        assert eval(code, {}, {**blank, "filled_by_ai_agent": "gpt"}) == 1
        assert eval(
            code, {}, {**blank, "filled_by_ai_agent": "gpt", "filled_by_human_agent": "amy"}
        ) == 2

    def test_none_is_treated_as_zero(self):
        # Matches the interpreter, which coerces None to 0 before arithmetic.
        code = py("={{Alpha}} + {{Beta}}")
        assert eval(code, {}, {"alpha": None, "beta": 5}) == 5
        assert eval(code, {}, {"alpha": None, "beta": None}) == 0

    def test_division_by_zero_guard_expression(self):
        # The dialect guards division explicitly; verify the guard compiles
        # and short-circuits rather than raising.
        code = py("=IF({{Denominator}} = 0, 0, {{Numerator}} / {{Denominator}})")
        assert eval(code, {}, {"denominator": 0, "numerator": 5}) == 0
        assert eval(code, {}, {"denominator": 2, "numerator": 5}) == 2.5


class TestGoArithmetic:
    @pytest.mark.parametrize("formula", ["=1 + 2", "=5 - 3", "=4 * 3"])
    def test_arithmetic_compiles(self, formula):
        assert go(formula)

    def test_counting_idiom_compiles(self):
        code = go(
            "=IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0)"
            " + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0)"
        )
        assert "+" in code


class TestParityWithInterpreter:
    @pytest.mark.parametrize(
        "formula,ctx_pascal,ctx_snake",
        [
            ("={{Alpha}} + {{Beta}}", {"Alpha": 2, "Beta": 5}, {"alpha": 2, "beta": 5}),
            ("={{Alpha}} * {{Beta}}", {"Alpha": 3, "Beta": 4}, {"alpha": 3, "beta": 4}),
            ("={{Alpha}} - {{Beta}}", {"Alpha": 9, "Beta": 4}, {"alpha": 9, "beta": 4}),
        ],
    )
    def test_compiled_matches_interpreter(self, formula, ctx_pascal, ctx_snake):
        assert eval(py(formula), {}, ctx_snake) == evaluate(formula, ctx_pascal)
