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


if __name__ == "__main__":
    unittest.main()
