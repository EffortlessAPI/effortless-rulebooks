"""Unit tests for the Python substrate's transitive-closure engine.

The contract these pin down is the one Postgres already implements as a
cycle-safe WITH RECURSIVE view (see any generated postgres/03-create-views.sql,
vw_<entity>_closure):

    columns: from_id, to_id, hop_distance, is_inferred
    hop_distance  = MIN over all derivations (shortest path)
    is_inferred   = TRUE iff no directly-asserted hop-1 edge states the pair
    cycle-safe    = a node already on the path is never revisited

Two edge shapes exist in the rulebook:
  * edge-table      — EdgeTable + FromColumn + ToColumn (e.g. StepPrecedence)
  * self-referential — ToColumn only, FK on the entity's own rows (e.g. Roles)

Run: python3 -m pytest execution-substrates/python/tests/ -v
"""

import sys
from pathlib import Path

import pytest

SUBSTRATE_DIR = Path(__file__).resolve().parent.parent
REPO_ROOT = SUBSTRATE_DIR.parent.parent
sys.path.insert(0, str(SUBSTRATE_DIR))
sys.path.insert(0, str(REPO_ROOT))

from python_only_erb_simulator import compute_closure_relation


def as_tuples(rows):
    """Normalize closure rows to a comparable sorted tuple set."""
    return sorted(
        (r["from_id"], r["to_id"], r["hop_distance"], r["is_inferred"]) for r in rows
    )


class TestSelfReferentialClosure:
    """ToColumn-only shape: the FK lives on the entity's own rows."""

    def test_simple_chain_infers_transitive_pair(self):
        # A -> B -> C asserts 2 edges and implies A -> C.
        records = [
            {"role_id": "A", "delegates_to": "B"},
            {"role_id": "B", "delegates_to": "C"},
            {"role_id": "C", "delegates_to": ""},
        ]
        rows = compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        )
        assert as_tuples(rows) == [
            ("A", "B", 1, False),
            ("A", "C", 2, True),
            ("B", "C", 1, False),
        ]

    def test_empty_string_fk_is_not_an_edge(self):
        # The transpiler stores absent relationships as '' rather than NULL.
        records = [
            {"role_id": "A", "delegates_to": ""},
            {"role_id": "B", "delegates_to": ""},
        ]
        assert compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        ) == []

    def test_null_fk_is_not_an_edge(self):
        records = [
            {"role_id": "A", "delegates_to": None},
            {"role_id": "B", "delegates_to": None},
        ]
        assert compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        ) == []

    def test_cycle_terminates_and_is_cycle_safe(self):
        # A -> B -> A must terminate, never revisiting a node on the path.
        records = [
            {"role_id": "A", "delegates_to": "B"},
            {"role_id": "B", "delegates_to": "A"},
        ]
        rows = compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        )
        assert as_tuples(rows) == [
            ("A", "A", 2, True),
            ("A", "B", 1, False),
            ("B", "A", 1, False),
            ("B", "B", 2, True),
        ]

    def test_self_loop_terminates(self):
        records = [{"role_id": "A", "delegates_to": "A"}]
        rows = compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        )
        assert as_tuples(rows) == [("A", "A", 1, False)]

    def test_missing_pk_row_is_skipped(self):
        records = [
            {"role_id": "", "delegates_to": "B"},
            {"role_id": "A", "delegates_to": "B"},
        ]
        rows = compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        )
        assert as_tuples(rows) == [("A", "B", 1, False)]


class TestEdgeTableClosure:
    """EdgeTable + FromColumn + ToColumn shape: edges in a junction table."""

    def test_five_node_chain_yields_ten_pairs(self):
        # 1->2->3->4->5: 4 asserted edges imply the full 10-pair ordering.
        edges = [
            {"from_step": "1", "to_step": "2"},
            {"from_step": "2", "to_step": "3"},
            {"from_step": "3", "to_step": "4"},
            {"from_step": "4", "to_step": "5"},
        ]
        rows = compute_closure_relation(
            edges, from_field="from_step", to_field="to_step"
        )
        assert len(rows) == 10
        by_pair = {(r["from_id"], r["to_id"]): r for r in rows}
        # The headline never-asserted inference.
        assert by_pair[("1", "5")]["hop_distance"] == 4
        assert by_pair[("1", "5")]["is_inferred"] is True
        # Directly asserted edges are hop 1 and not inferred.
        assert by_pair[("1", "2")] == {
            "from_id": "1",
            "to_id": "2",
            "hop_distance": 1,
            "is_inferred": False,
        }
        assert sum(1 for r in rows if not r["is_inferred"]) == 4
        assert sum(1 for r in rows if r["is_inferred"]) == 6

    def test_diamond_takes_shortest_hop_distance(self):
        # A->B->D and A->C->D, plus a direct A->D: the direct edge wins.
        edges = [
            {"f": "A", "t": "B"},
            {"f": "A", "t": "C"},
            {"f": "B", "t": "D"},
            {"f": "C", "t": "D"},
            {"f": "A", "t": "D"},
        ]
        rows = compute_closure_relation(edges, from_field="f", to_field="t")
        by_pair = {(r["from_id"], r["to_id"]): r for r in rows}
        assert by_pair[("A", "D")]["hop_distance"] == 1
        assert by_pair[("A", "D")]["is_inferred"] is False

    def test_asserted_pair_also_reachable_transitively_is_not_inferred(self):
        # A->C is asserted AND reachable via A->B->C. Assertion wins.
        edges = [
            {"f": "A", "t": "B"},
            {"f": "B", "t": "C"},
            {"f": "A", "t": "C"},
        ]
        rows = compute_closure_relation(edges, from_field="f", to_field="t")
        by_pair = {(r["from_id"], r["to_id"]): r for r in rows}
        assert by_pair[("A", "C")]["hop_distance"] == 1
        assert by_pair[("A", "C")]["is_inferred"] is False

    def test_disconnected_fragments_do_not_cross(self):
        edges = [
            {"f": "A", "t": "B"},
            {"f": "X", "t": "Y"},
        ]
        rows = compute_closure_relation(edges, from_field="f", to_field="t")
        assert as_tuples(rows) == [("A", "B", 1, False), ("X", "Y", 1, False)]

    def test_empty_edge_set(self):
        assert compute_closure_relation([], from_field="f", to_field="t") == []


class TestGoldenFilesFromPostgresOracle:
    """Reproduce the real Postgres views byte-for-byte on Talisman's data.

    The fixtures in testing/closure-golden/ were dumped from the live
    vw_*_closure views; Postgres is the designated oracle for this domain.
    """

    GOLDEN_DIR = REPO_ROOT / "testing" / "closure-golden"
    DOMAIN = REPO_ROOT / "rulebook-examples" / "talismans-special-solutions"

    def _rulebook(self):
        import json

        path = (
            self.DOMAIN
            / "effortless-rulebook"
            / "talismans-special-solutions-rulebook.json"
        )
        with open(path) as f:
            return json.load(f)

    def _golden(self, view_name):
        import json

        with open(self.GOLDEN_DIR / f"{view_name}.json") as f:
            return json.load(f)

    def test_step_precedence_closure_matches_postgres(self):
        rb = self._rulebook()
        edges = [
            {"from_step": r.get("FromStep"), "to_step": r.get("ToStep")}
            for r in rb["StepPrecedence"]["data"]
        ]
        rows = compute_closure_relation(
            edges, from_field="from_step", to_field="to_step"
        )
        assert as_tuples(rows) == as_tuples(
            self._golden("vw_step_precedence_closure")
        )

    def test_roles_closure_matches_postgres(self):
        rb = self._rulebook()
        records = [
            {"role_id": r.get("RoleId"), "delegates_to": r.get("DelegatesTo")}
            for r in rb["Roles"]["data"]
        ]
        rows = compute_closure_relation(
            records, pk_field="role_id", to_field="delegates_to"
        )
        assert as_tuples(rows) == as_tuples(self._golden("vw_roles_closure"))

    def test_workflow_artifacts_closure_matches_postgres(self):
        rb = self._rulebook()
        records = [
            {
                "artifact_id": r.get("ArtifactId"),
                "derived_from_artifact": r.get("DerivedFromArtifact"),
            }
            for r in rb["WorkflowArtifacts"]["data"]
        ]
        rows = compute_closure_relation(
            records, pk_field="artifact_id", to_field="derived_from_artifact"
        )
        assert as_tuples(rows) == as_tuples(
            self._golden("vw_workflow_artifacts_closure")
        )
