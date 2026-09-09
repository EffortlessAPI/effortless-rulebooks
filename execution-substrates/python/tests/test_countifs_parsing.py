"""General COUNTIFS parsing: N criteria pairs, literal or field criteria.

COUNTIFS is variadic — (range, criteria) repeated. Earlier parsing used one
regex per shape, so anything they did not anticipate silently returned None
and every dependent field with it:

    =COUNTIFS(Child!{{FK}}, Parent!{{Id}})                       one pair, field
    =COUNTIFS(vw_x_closure!{{IsInferred}}, TRUE())               one pair, literal
    =COUNTIFS(RoleAssignments!{{IsAgentTypeChange}}, TRUE)       bare TRUE, no parens
    =COUNTIFS(Steps!{{Workflow}}, Workflows!{{Id}},
              Steps!{{IsExecutedByAI}}, TRUE)                    two pairs, mixed

All ranges must name the same table: COUNTIFS counts rows of one table.

Run: python3 -m pytest execution-substrates/python/tests/ -v
"""

import json
import sys
from pathlib import Path

import pytest

SUBSTRATE_DIR = Path(__file__).resolve().parent.parent
REPO_ROOT = SUBSTRATE_DIR.parent.parent
sys.path.insert(0, str(SUBSTRATE_DIR))
sys.path.insert(0, str(REPO_ROOT))

from python_only_erb_simulator import parse_countifs, count_matching_rows

DOMAIN = REPO_ROOT / "rulebook-examples" / "talismans-special-solutions"
RULEBOOK = (
    DOMAIN / "effortless-rulebook" / "talismans-special-solutions-rulebook.json"
)


class TestParsing:
    def test_single_field_criteria(self):
        table, criteria = parse_countifs(
            "=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})"
        )
        assert table == "WorkflowSteps"
        assert criteria == [("Workflow", ("field", "WorkflowId"))]

    def test_literal_true_with_parens(self):
        table, criteria = parse_countifs(
            "=COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE())"
        )
        assert table == "vw_step_precedence_closure"
        assert criteria == [("IsInferred", ("literal", True))]

    def test_bare_true_without_parens(self):
        table, criteria = parse_countifs(
            "=COUNTIFS(RoleAssignments!{{IsAgentTypeChange}}, TRUE)"
        )
        assert table == "RoleAssignments"
        assert criteria == [("IsAgentTypeChange", ("literal", True))]

    def test_literal_false(self):
        _, criteria = parse_countifs(
            "=COUNTIFS(Roles!{{HasExactlyOneFiller}}, FALSE())"
        )
        assert criteria == [("HasExactlyOneFiller", ("literal", False))]

    def test_two_criteria_pairs(self):
        table, criteria = parse_countifs(
            "=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, "
            "WorkflowSteps!{{IsExecutedByAI}}, TRUE)"
        )
        assert table == "WorkflowSteps"
        assert criteria == [
            ("Workflow", ("field", "WorkflowId")),
            ("IsExecutedByAI", ("literal", True)),
        ]

    def test_quoted_string_criteria(self):
        _, criteria = parse_countifs(
            '=COUNTIFS(Steps!{{OwningDepartment}}, "legal")'
        )
        assert criteria == [("OwningDepartment", ("literal", "legal"))]

    def test_numeric_criteria(self):
        _, criteria = parse_countifs("=COUNTIFS(Steps!{{HopDistance}}, 2)")
        assert criteria == [("HopDistance", ("literal", 2))]

    def test_mismatched_range_tables_is_an_error(self):
        # COUNTIFS counts rows of ONE table; two different ranges is nonsense.
        with pytest.raises(ValueError, match="same table"):
            parse_countifs(
                "=COUNTIFS(A!{{X}}, Parent!{{Id}}, B!{{Y}}, TRUE)"
            )

    def test_odd_argument_count_is_an_error(self):
        with pytest.raises(ValueError, match="pairs"):
            parse_countifs("=COUNTIFS(Steps!{{Workflow}})")

    def test_non_countifs_returns_none(self):
        assert parse_countifs("=SUMIFS(A!{{X}}, A!{{Y}}, B!{{Z}})") == (None, None)


class TestCounting:
    ROWS = [
        {"workflow": "w1", "is_executed_by_ai": True, "owning_department": "legal"},
        {"workflow": "w1", "is_executed_by_ai": False, "owning_department": "eng"},
        {"workflow": "w1", "is_executed_by_ai": True, "owning_department": "eng"},
        {"workflow": "w2", "is_executed_by_ai": True, "owning_department": "eng"},
    ]

    def test_single_field_criteria(self):
        criteria = [("Workflow", ("field", "WorkflowId"))]
        assert count_matching_rows(self.ROWS, criteria, {"workflow_id": "w1"}) == 3

    def test_two_criteria_pairs_narrow_the_count(self):
        criteria = [
            ("Workflow", ("field", "WorkflowId")),
            ("IsExecutedByAI", ("literal", True)),
        ]
        assert count_matching_rows(self.ROWS, criteria, {"workflow_id": "w1"}) == 2

    def test_literal_only_counts_whole_table(self):
        criteria = [("IsExecutedByAI", ("literal", True))]
        assert count_matching_rows(self.ROWS, criteria, {}) == 3

    def test_no_match_is_zero(self):
        criteria = [("Workflow", ("field", "WorkflowId"))]
        assert count_matching_rows(self.ROWS, criteria, {"workflow_id": "nope"}) == 0


class TestEveryAggregationInTheDomainParses:
    def countifs_fields(self):
        with open(RULEBOOK) as f:
            rb = json.load(f)
        for table, tv in rb.items():
            if not isinstance(tv, dict) or "schema" not in tv:
                continue
            for field in tv["schema"]:
                formula = field.get("formula") or ""
                if field.get("type") == "aggregation" and "COUNTIFS" in formula:
                    yield table, field["name"], formula

    def test_all_countifs_parse(self):
        unparsed = [
            f"{table}.{name}"
            for table, name, formula in self.countifs_fields()
            if parse_countifs(formula)[0] is None
        ]
        assert unparsed == [], f"unparsed COUNTIFS: {unparsed}"

    def test_domain_actually_has_countifs(self):
        assert len(list(self.countifs_fields())) >= 10
