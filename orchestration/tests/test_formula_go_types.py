"""Type-aware comparison in the Go backend.

Go models nullable scalars as pointers, so every comparison needs a nil-safe
accessor — but which one depends on the field's declared datatype. The
backend chose by node shape instead: field-vs-field comparison always wrapped
both sides in boolVal(), which does not compile when the fields are *int:

    invalid operation: boolVal(tc.MonthsSinceModified) > boolVal(...)
        (operator > not defined on bool)
    cannot use tc.MonthsSinceModified (variable of type *int)
        as *bool value in argument to boolVal

The injector already supplies accurate datatypes via build_field_types; the
backend simply has to consult them. Non-nullable fields are plain values, not
pointers, so they must not be wrapped at all.

These tests assert on generated source rather than behavior — the Go
substrate's own erb_closure_test.go covers runtime behavior.

Run: python3 -m pytest orchestration/tests/ -v
"""

import sys
from pathlib import Path

import pytest

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
sys.path.insert(0, str(REPO_ROOT))

from orchestration.formula_parser import compile_to_go, parse_formula

INT_FIELDS = {"Months": "integer", "Threshold": "integer"}
BOOL_FIELDS = {"IsStale": "boolean", "IsActive": "boolean"}
STR_FIELDS = {"Name": "string", "Label": "string"}


def go(formula, field_types):
    return compile_to_go(parse_formula(formula), "tc", field_types)


class TestIntegerComparison:
    def test_int_field_vs_int_field_does_not_use_boolval(self):
        code = go("={{Months}} > {{Threshold}}", INT_FIELDS)
        assert "boolVal" not in code

    def test_int_field_vs_int_field_compares_numerically(self):
        code = go("={{Months}} > {{Threshold}}", INT_FIELDS)
        assert ">" in code
        # Pointer fields must be dereferenced or nil-guarded, never compared raw.
        assert "nil" in code or "intVal" in code

    def test_int_arithmetic_does_not_use_boolval(self):
        code = go("={{Months}} + {{Threshold}}", INT_FIELDS)
        assert "boolVal" not in code


class TestBooleanComparison:
    def test_bool_field_vs_bool_field_uses_boolval(self):
        code = go("={{IsStale}} = {{IsActive}}", BOOL_FIELDS)
        assert "boolVal" in code

    def test_bool_field_vs_literal_uses_boolval(self):
        code = go("={{IsStale}} = TRUE", BOOL_FIELDS)
        assert "boolVal" in code


class TestStringComparison:
    def test_string_field_vs_string_field_uses_stringval(self):
        code = go("={{Name}} = {{Label}}", STR_FIELDS)
        assert "stringVal" in code
        assert "boolVal" not in code

    def test_string_field_vs_literal_uses_stringval(self):
        code = go('={{Name}} = "x"', STR_FIELDS)
        assert "stringVal" in code


class TestUnknownFieldsFallBackSafely:
    def test_missing_datatype_still_compiles(self):
        # An unknown field must not crash generation.
        assert go("={{Mystery}} = {{Other}}", {})


class TestRealTalismanFormulas:
    """Shapes that actually broke the Talisman build."""

    def test_is_stale_comparison(self):
        code = go(
            "={{MonthsSinceModified}} > {{StalenessThresholdMonths}}",
            {
                "MonthsSinceModified": "integer",
                "StalenessThresholdMonths": "integer",
            },
        )
        assert "boolVal" not in code

    def test_closure_pair_sum(self):
        code = go(
            "={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}}",
            {
                "CountAssertedPrecedencePairs": "integer",
                "CountInferredPrecedencePairs": "integer",
            },
        )
        assert "boolVal" not in code

    def test_count_minus_literal(self):
        code = go(
            "={{CountOfNonProposedSteps}} - 1",
            {"CountOfNonProposedSteps": "integer"},
        )
        assert "boolVal" not in code
