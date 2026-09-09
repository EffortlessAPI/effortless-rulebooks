"""DATETIME_DIFF / NOW / TODAY in the Python backend.

These were implemented in the runtime interpreter but not in
compile_to_python, so a rulebook using them generated a function that raised
NotImplementedError.

The compiled form delegates to the interpreter's own helpers rather than
reimplementing the date math. Month counting must follow Postgres AGE()
semantics (whole elapsed calendar months, not days//30) or the substrate
drifts from the oracle on month boundaries — the interpreter already
encodes that, and duplicating it invites exactly that drift.

FORMULA_NOW pins NOW() so answer keys stay reproducible.

Run: python3 -m pytest orchestration/tests/ -v
"""

import os
import sys
from pathlib import Path

import pytest

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
sys.path.insert(0, str(REPO_ROOT))

from orchestration import formula_parser
from orchestration.formula_parser import compile_to_python, evaluate, parse_formula


def run(formula, ctx):
    """Compile to Python and execute with the interpreter helpers in scope."""
    code = compile_to_python(parse_formula(formula))
    scope = {"_erb": formula_parser}
    scope.update(ctx)
    return eval(code, {"_erb": formula_parser}, scope)


@pytest.fixture
def pinned_now(monkeypatch):
    monkeypatch.setenv("FORMULA_NOW", "2026-09-09T00:00:00")
    return "2026-09-09T00:00:00"


class TestDatetimeDiffCompiles:
    def test_months_between_dates(self, pinned_now):
        formula = '=DATETIME_DIFF(NOW(), {{Modified}}, "months")'
        assert run(formula, {"modified": "2026-04-09T00:00:00"}) == 5

    def test_partial_month_is_not_counted(self, pinned_now):
        # AGE() does not count the final partial month: one day short of the
        # anniversary is still 4 months, not 5.
        formula = '=DATETIME_DIFF(NOW(), {{Modified}}, "months")'
        assert run(formula, {"modified": "2026-04-10T00:00:00"}) == 4

    def test_days_unit(self, pinned_now):
        formula = '=DATETIME_DIFF(NOW(), {{Modified}}, "days")'
        assert run(formula, {"modified": "2026-09-01T00:00:00"}) == 8

    def test_null_input_yields_none(self, pinned_now):
        formula = '=DATETIME_DIFF(NOW(), {{Modified}}, "months")'
        assert run(formula, {"modified": None}) is None

    def test_timezone_aware_input_does_not_raise(self, pinned_now):
        formula = '=DATETIME_DIFF(NOW(), {{Modified}}, "months")'
        assert run(formula, {"modified": "2026-03-31T00:00:00-05:00"}) == 5


class TestParityWithInterpreter:
    @pytest.mark.parametrize(
        "modified",
        ["2026-04-09T00:00:00", "2026-04-10T00:00:00", "2026-01-01T00:00:00"],
    )
    @pytest.mark.parametrize("unit", ["months", "days"])
    def test_compiled_matches_interpreter(self, modified, unit, pinned_now):
        formula = f'=DATETIME_DIFF(NOW(), {{{{Modified}}}}, "{unit}")'
        assert run(formula, {"modified": modified}) == evaluate(
            formula, {"Modified": modified}
        )


class TestNowIsPinnable:
    def test_formula_now_controls_the_clock(self, monkeypatch):
        monkeypatch.setenv("FORMULA_NOW", "2030-01-01T00:00:00")
        formula = '=DATETIME_DIFF(NOW(), {{Modified}}, "years")'
        assert run(formula, {"modified": "2020-01-01T00:00:00"}) == 10
