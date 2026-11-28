#!/usr/bin/env python3
"""A rulebook file is written in one layout: standard JSON with single-line leaves.

scripts/rulebook_layout.py is the one implementation. What holds here:

* It writes, byte for byte, what the generated rulebook editor's Save writes
  (`serializeRulebook` in rulebook-to-node-postgres-api). The expected text in
  fixtures/rulebook-layout/as-the-editor-writes-it.json was written by that
  function from input.json, so a file saved by the editor and a file saved by
  this repo never flip each other's lines.
* It changes the layout, never what the file says: numbers keep their text
  (0.0 stays 0.0), text stays UTF-8, and laying out twice changes nothing.
* Every writer in the repo that writes a rulebook file in this layout goes
  through it: orchestration/ontology_form.py's write_rulebook and
  effortless-math's scripts/format-rulebook.py.
* The South Park project's two rulebook files (its hub and its concept
  rulebook) are in the layout as committed.

    python3 scripts/tests/test_rulebook_layout.py        (or pytest)
"""
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "scripts"))
sys.path.insert(0, str(REPO / "orchestration"))
import rulebook_layout  # noqa: E402
import ontology_form  # noqa: E402

FIXTURES = Path(__file__).resolve().parent / "fixtures" / "rulebook-layout"
SOUTH_PARK = REPO / "rulebook-examples" / "talismans-southpark-ontology"
SOUTH_PARK_FILES = [
    SOUTH_PARK / "effortless-rulebook" / "talismans-southpark-ontology-rulebook.json",
    SOUTH_PARK / "concept-rulebook" / "talismans-southpark-ontology-concepts.json",
]


class Layout(unittest.TestCase):
    def test_writes_what_the_rulebook_editor_writes(self):
        given = (FIXTURES / "input.json").read_text(encoding="utf-8")
        expected = (FIXTURES / "as-the-editor-writes-it.json").read_text(encoding="utf-8")
        self.assertEqual(rulebook_layout.format_text(given), expected)

    def test_one_field_or_one_row_per_line(self):
        text = (FIXTURES / "as-the-editor-writes-it.json").read_text(encoding="utf-8")
        lines = text.splitlines()
        self.assertIn('      {"ThingId": "stan", "Score": 2.5, "Tags": ["x", 1, true, null], '
                      '"Note": "line\\nbreak \\"quoted\\" ✓"},', lines)
        self.assertIn('      {"name": "ThingId", "datatype": "string", "type": "raw", "nullable": false},', lines)

    def test_numbers_keep_their_text_and_text_stays_utf8(self):
        out = rulebook_layout.format_text('{"T": {"schema": [], "data": [{"A": 0.0, "B": 2e15, "C": 1.50, "D": "é"}]}}')
        self.assertIn('{"A": 0.0, "B": 2e15, "C": 1.50, "D": "é"}', out)
        self.assertNotIn("\\u", out)

    def test_laying_out_twice_changes_nothing(self):
        once = rulebook_layout.format_text((FIXTURES / "input.json").read_text(encoding="utf-8"))
        self.assertEqual(rulebook_layout.format_text(once), once)

    def test_refuses_what_is_not_a_rulebook(self):
        with self.assertRaises(ValueError):
            rulebook_layout.format_text("[1, 2]")
        with self.assertRaises(json.JSONDecodeError):
            rulebook_layout.format_text("{not json")

    def test_check_says_which_file_is_not_in_the_layout(self):
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "r.json"
            path.write_text(json.dumps(json.loads((FIXTURES / "input.json").read_text(encoding="utf-8")), indent=1),
                            encoding="utf-8")
            run = lambda *a: subprocess.run([sys.executable, str(REPO / "scripts" / "rulebook_layout.py"), *a],
                                            capture_output=True, text=True)
            self.assertEqual(run("--check", str(path)).returncode, 1)
            self.assertEqual(run(str(path)).returncode, 0)
            self.assertEqual(run("--check", str(path)).returncode, 0)
            self.assertEqual(path.read_text(encoding="utf-8"),
                             (FIXTURES / "as-the-editor-writes-it.json").read_text(encoding="utf-8"))


class EveryWriterUsesIt(unittest.TestCase):
    def test_ontology_form_writes_the_layout(self):
        doc = json.loads((FIXTURES / "input.json").read_text(encoding="utf-8"))
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "r.json"
            ontology_form.write_rulebook(doc, path)
            self.assertEqual(path.read_text(encoding="utf-8"),
                             (FIXTURES / "as-the-editor-writes-it.json").read_text(encoding="utf-8"))

    def test_effortless_math_formatter_writes_the_layout(self):
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "r.json"
            path.write_text((FIXTURES / "input.json").read_text(encoding="utf-8"), encoding="utf-8")
            subprocess.run([sys.executable, str(REPO / "rulebook-examples" / "effortless-math" / "scripts" / "format-rulebook.py"),
                            str(path)], check=True, capture_output=True)
            self.assertEqual(path.read_text(encoding="utf-8"),
                             (FIXTURES / "as-the-editor-writes-it.json").read_text(encoding="utf-8"))


class SouthParkFiles(unittest.TestCase):
    def test_both_rulebook_files_are_in_the_layout(self):
        for path in SOUTH_PARK_FILES:
            text = path.read_text(encoding="utf-8")
            self.assertEqual(rulebook_layout.format_text(text), text,
                             f"{path.relative_to(REPO)} is not in the layout: python3 scripts/rulebook_layout.py {path.relative_to(REPO)}")


if __name__ == "__main__":
    unittest.main()
