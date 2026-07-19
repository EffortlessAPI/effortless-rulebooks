from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

from pko_rulebook_tool import (  # noqa: E402
    load_rulebook,
    render_financial_sops,
    render_governance,
    render_natural_language_docs,
    render_text_email_policies,
    validate_rulebook,
)
from bpm_process_export_to_pko import transform  # noqa: E402


class PkoRulebookTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.rulebook = load_rulebook(ROOT / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json")

    def test_canonical_rulebook_validates(self) -> None:
        self.assertEqual([], validate_rulebook(self.rulebook))

    def test_specification_and_execution_are_distinct(self) -> None:
        version_ids = {row["ProcedureVersionId"] for row in self.rulebook["ProcedureVersions"]["data"]}
        execution_ids = {row["ProcedureExecutionId"] for row in self.rulebook["ProcedureExecutions"]["data"]}
        self.assertTrue(version_ids)
        self.assertTrue(execution_ids)
        self.assertTrue(version_ids.isdisjoint(execution_ids))

    def test_natural_language_projection(self) -> None:
        rendered = render_natural_language_docs(self.rulebook)
        self.assertIn("Quarter-End Financial Close", rendered)
        self.assertIn("Workforce Policy Change and Employee Notification", rendered)
        self.assertIn("Tacit knowledge", rendered)
        self.assertIn("Execution witnesses", rendered)

    def test_financial_sop_projection(self) -> None:
        rendered = render_financial_sops(self.rulebook)
        self.assertIn("Financial Standard Operating Procedures", rendered)
        self.assertIn("Preparer/approver separation", rendered)
        self.assertIn("CFO approval", rendered)

    def test_communication_policy_projection(self) -> None:
        rendered = render_text_email_policies(self.rulebook)
        self.assertIn("## Email", rendered)
        self.assertIn("## SMS", rendered)
        self.assertIn("**Consent required:** Yes", rendered)
        self.assertIn("Reply STOP", rendered)

    def test_governance_projection(self) -> None:
        rendered = render_governance(self.rulebook)
        self.assertIn("Stewardship and authority", rendered)
        self.assertIn("Knowledge gaps", rendered)
        self.assertIn("Role history", rendered)
        self.assertIn("Operational bindings", rendered)

    def test_bpm_adapter_unlocks_existing_projection(self) -> None:
        source = json.loads((ROOT / "examples" / "bpm-vendor-payment.json").read_text(encoding="utf-8"))
        transformed = transform(source, self.rulebook)
        self.assertEqual([], validate_rulebook(transformed))
        rendered = render_financial_sops(transformed)
        self.assertIn("Vendor Payment Approval", rendered)
        self.assertIn("Independent approval", rendered)

    def test_composite_keys_have_no_delimiter_collision(self) -> None:
        """Composite keys join on "a|b" strings, so an id containing "|" would
        silently produce wrong join results with no error anywhere.

        Several witnesses route around the transpiler's multi-criteria COUNTIFS
        defect by concatenating two ids with a pipe. That works only while no
        identifier contains a pipe. The failure mode is a wrong answer, not a
        crash, so it has to be asserted rather than hoped for.
        """
        offenders = []
        for table, payload in self.rulebook.items():
            if not isinstance(payload, dict) or "schema" not in payload:
                continue
            id_fields = [
                f["name"] for f in payload["schema"]
                if isinstance(f, dict) and f.get("type") in {"raw", "relationship"}
                and f.get("datatype") == "string"
                and (f["name"].endswith("Id") or f.get("RelatedTo"))
            ]
            for row in payload.get("data", []):
                for field in id_fields:
                    value = row.get(field)
                    if isinstance(value, str) and "|" in value:
                        offenders.append(f"{table}.{field} = {value!r}")
        self.assertEqual([], offenders, "identifier contains the composite-key delimiter")

    def test_every_witness_field_traces_to_a_question(self) -> None:
        """RulebookFields.InventedForQuestion must resolve to a real question."""
        question_ids = {q["RoleQuestionId"] for q in self.rulebook["RoleQuestions"]["data"]}
        dangling = sorted(
            row["RulebookFieldId"]
            for row in self.rulebook["RulebookFields"]["data"]
            if row.get("InventedForQuestion")
            and row["InventedForQuestion"] not in question_ids
        )
        self.assertEqual([], dangling, "witness field points at a non-existent question")

    def test_field_catalog_matches_the_real_schemas(self) -> None:
        """The catalog is derived. If it has drifted it is misreporting the model."""
        catalogued = {row["RulebookFieldId"] for row in self.rulebook["RulebookFields"]["data"]}
        actual = {
            f"{table}.{f['name']}"
            for table, payload in self.rulebook.items()
            if isinstance(payload, dict) and "schema" in payload
            for f in payload["schema"] if isinstance(f, dict)
        }
        self.assertEqual(set(), actual - catalogued, "fields missing from the catalog")
        self.assertEqual(set(), catalogued - actual, "catalog lists fields that do not exist")


if __name__ == "__main__":
    unittest.main()
