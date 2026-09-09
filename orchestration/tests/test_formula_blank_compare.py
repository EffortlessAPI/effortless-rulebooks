"""Null-safe blank comparison: {{X}} <> "" and {{X}} = "".

The dialect's blank-check idiom is a bare comparison against the empty
string, and the doctrine calls it null-safe: a column that is NULL is blank
just as an empty string is. Postgres agrees — vw_workflow_steps resolves
sequence_position to the inferred value when the override is NULL.

Python's `None != ""` is True, so a NULL override read as "present" and the
override branch returned None instead of falling through. This is the
override/resolve idiom:

    =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}},
        {{InferredSequencePosition}})

Ordinary equality between two values is unaffected; only comparison against
an empty-string literal is a blank check.

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


OVERRIDE = (
    '=IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, '
    "{{InferredSequencePosition}})"
)


class TestBlankCheckIsNullSafe:
    @pytest.mark.parametrize("blank", [None, ""])
    def test_not_blank_is_false_for_blank_values(self, blank):
        assert eval(py('={{X}} <> ""'), {}, {"x": blank}) is False

    @pytest.mark.parametrize("blank", [None, ""])
    def test_is_blank_is_true_for_blank_values(self, blank):
        assert eval(py('={{X}} = ""'), {}, {"x": blank}) is True

    def test_present_value_is_not_blank(self):
        assert eval(py('={{X}} <> ""'), {}, {"x": "v"}) is True
        assert eval(py('={{X}} <> ""'), {}, {"x": 3}) is True
        assert eval(py('={{X}} = ""'), {}, {"x": 3}) is False

    def test_zero_is_present_not_blank(self):
        # 0 is a real value; only None and "" are blank.
        assert eval(py('={{X}} <> ""'), {}, {"x": 0}) is True


class TestOverrideResolveIdiom:
    @pytest.mark.parametrize("override", [None, ""])
    def test_blank_override_falls_through_to_inferred(self, override):
        ctx = {
            "sequence_position_override": override,
            "inferred_sequence_position": 4,
        }
        assert eval(py(OVERRIDE), {}, ctx) == 4

    def test_present_override_wins(self):
        ctx = {"sequence_position_override": 9, "inferred_sequence_position": 4}
        assert eval(py(OVERRIDE), {}, ctx) == 9


class TestOrdinaryEqualityUnaffected:
    def test_none_still_equals_none(self):
        assert eval(py("={{A}} = {{B}}"), {}, {"a": None, "b": None}) is True

    def test_string_comparison_unaffected(self):
        assert eval(py('={{A}} = "x"'), {}, {"a": "x"}) is True
        assert eval(py('={{A}} = "x"'), {}, {"a": "y"}) is False


class TestParityWithInterpreter:
    @pytest.mark.parametrize("override", [None, "", 9])
    def test_compiled_matches_interpreter(self, override):
        compiled = eval(
            py(OVERRIDE),
            {},
            {
                "sequence_position_override": override,
                "inferred_sequence_position": 4,
            },
        )
        interpreted = evaluate(
            OVERRIDE,
            {"SequencePositionOverride": override, "InferredSequencePosition": 4},
        )
        assert compiled == interpreted
