"""Aggregations that address a materialized closure as a pseudo-table.

Closure fields are not graded directly; they feed graded aggregations that
query the closure by view name, e.g.

    =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE())
    =COUNTIFS(vw_step_precedence_closure!{{ToId}}, WorkflowSteps!{{WorkflowStepId}})

The first form counts rows matching a literal boolean; the second counts
rows whose column matches the current record's field. Expected values are
taken from the Postgres oracle's own answer key for Talisman.
"""

import sys
from pathlib import Path

import pytest

SUBSTRATE_DIR = Path(__file__).resolve().parent.parent
REPO_ROOT = SUBSTRATE_DIR.parent.parent
sys.path.insert(0, str(SUBSTRATE_DIR))
sys.path.insert(0, str(REPO_ROOT))

from python_only_erb_simulator import (
    parse_countifs_formula,
    parse_countifs_literal_formula,
    count_closure_rows,
)

GOLDEN = REPO_ROOT / "testing" / "closure-golden"


@pytest.fixture
def step_closure():
    import json

    with open(GOLDEN / "vw_step_precedence_closure.json") as f:
        return json.load(f)


class TestLiteralCriteriaParsing:
    def test_parses_true_literal(self):
        assert parse_countifs_literal_formula(
            "=COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE())"
        ) == ("vw_step_precedence_closure", "IsInferred", True)

    def test_parses_false_literal(self):
        assert parse_countifs_literal_formula(
            "=COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, FALSE())"
        ) == ("vw_step_precedence_closure", "IsInferred", False)

    def test_field_match_form_is_not_a_literal(self):
        assert parse_countifs_literal_formula(
            "=COUNTIFS(vw_step_precedence_closure!{{ToId}}, WorkflowSteps!{{WorkflowStepId}})"
        ) == (None, None, None)

    def test_literal_form_is_not_a_field_match(self):
        # The pre-existing parser must keep rejecting the literal form so the
        # two branches stay unambiguous.
        assert parse_countifs_formula(
            "=COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE())"
        ) == (None, None, None)


class TestCountingAgainstTheClosure:
    def test_counts_inferred_pairs(self, step_closure):
        # 10-pair closure over 4 asserted edges: 6 inferred.
        assert count_closure_rows(step_closure, "is_inferred", True) == 6

    def test_counts_asserted_pairs(self, step_closure):
        assert count_closure_rows(step_closure, "is_inferred", False) == 4

    def test_counts_rows_matching_a_record_value(self, step_closure):
        # PrecedingStepCount: how many pairs reach this step.
        assert count_closure_rows(step_closure, "to_id", "prod-deploy-step-1") == 0
        assert count_closure_rows(step_closure, "to_id", "prod-deploy-step-5") == 4

    def test_preceding_step_counts_yield_sequence_positions(self, step_closure):
        # InferredSequencePosition = PrecedingStepCount + 1, so a clean chain
        # must produce 1..5 with no gaps or ties.
        positions = [
            count_closure_rows(step_closure, "to_id", f"prod-deploy-step-{i}") + 1
            for i in range(1, 6)
        ]
        assert positions == [1, 2, 3, 4, 5]

    def test_unknown_value_counts_zero(self, step_closure):
        assert count_closure_rows(step_closure, "to_id", "no-such-step") == 0

    def test_literal_criteria_also_applies_to_ordinary_tables(self):
        # The literal-criteria shape is not closure-specific:
        # =COUNTIFS(Roles!{{HasExactlyOneFiller}}, FALSE()) counts rows of a
        # real table. Treating a literal criteria as proof of a closure
        # wrongly rejected this formula.
        assert parse_countifs_literal_formula(
            "=COUNTIFS(Roles!{{HasExactlyOneFiller}}, FALSE())"
        ) == ("Roles", "HasExactlyOneFiller", False)

        roles = [
            {"has_exactly_one_filler": True},
            {"has_exactly_one_filler": False},
            {"has_exactly_one_filler": True},
        ]
        assert count_closure_rows(roles, "has_exactly_one_filler", False) == 1
        assert count_closure_rows(roles, "has_exactly_one_filler", True) == 2
