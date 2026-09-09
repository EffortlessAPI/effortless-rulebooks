"""INDEX/MATCH parsing in the Python substrate.

The MATCH key is a field on the record being computed, so the rulebook writes
it bare:

    =INDEX(WorkflowSteps!{{RelativePath}}, MATCH({{WorkflowStep}}, WorkflowSteps!{{WorkflowStepId}}, 0))

The parser's regex required a table prefix on that key (`\\w+!{{...}}`), which
only ever matched the prefixed spelling. Every lookup written the bare way
silently returned None and took its dependent fields down with it — all 12
lookups in Talisman.

Both spellings are valid and must resolve identically.

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

from python_only_erb_simulator import parse_index_match_formula

DOMAIN = REPO_ROOT / "rulebook-examples" / "talismans-special-solutions"
RULEBOOK = (
    DOMAIN / "effortless-rulebook" / "talismans-special-solutions-rulebook.json"
)

EXPECTED = ("WorkflowSteps", "RelativePath", "WorkflowStep", "WorkflowStepId")


class TestBothSpellingsParse:
    def test_bare_match_key(self):
        assert parse_index_match_formula(
            "=INDEX(WorkflowSteps!{{RelativePath}}, "
            "MATCH({{WorkflowStep}}, WorkflowSteps!{{WorkflowStepId}}, 0))"
        ) == EXPECTED

    def test_table_prefixed_match_key(self):
        assert parse_index_match_formula(
            "=INDEX(WorkflowSteps!{{RelativePath}}, "
            "MATCH(ApprovalGates!{{WorkflowStep}}, WorkflowSteps!{{WorkflowStepId}}, 0))"
        ) == EXPECTED

    def test_both_spellings_agree(self):
        bare = parse_index_match_formula(
            "=INDEX(Roles!{{FilledByHumanAgent}}, "
            "MATCH({{GateRole}}, Roles!{{RoleId}}, 0))"
        )
        prefixed = parse_index_match_formula(
            "=INDEX(Roles!{{FilledByHumanAgent}}, "
            "MATCH(ApprovalGates!{{GateRole}}, Roles!{{RoleId}}, 0))"
        )
        assert bare == prefixed

    def test_extra_whitespace_is_tolerated(self):
        assert parse_index_match_formula(
            "=INDEX( WorkflowSteps!{{RelativePath}} ,  "
            "MATCH( {{WorkflowStep}} , WorkflowSteps!{{WorkflowStepId}} , 0 ) )"
        ) == EXPECTED

    def test_non_lookup_formula_does_not_parse(self):
        assert parse_index_match_formula('=CONCAT({{A}}, "-", {{B}})') == (
            None,
            None,
            None,
            None,
        )


class TestEveryLookupInTheDomainParses:
    """A lookup the parser cannot read returns None for every record."""

    def lookups(self):
        with open(RULEBOOK) as f:
            rb = json.load(f)
        for table, tv in rb.items():
            if not isinstance(tv, dict) or "schema" not in tv:
                continue
            for field in tv["schema"]:
                if field.get("type") == "lookup" and field.get("formula"):
                    yield table, field["name"], field["formula"]

    def test_all_lookups_parse(self):
        unparsed = [
            f"{table}.{name}"
            for table, name, formula in self.lookups()
            if parse_index_match_formula(formula)[0] is None
        ]
        assert unparsed == [], f"unparsed lookups: {unparsed}"

    def test_domain_actually_has_lookups(self):
        # Guards against the test vacuously passing on an empty iterator.
        assert len(list(self.lookups())) >= 12
