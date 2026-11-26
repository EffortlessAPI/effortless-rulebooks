#!/usr/bin/env python3
"""The `xlsx` substrate grades the workbook the project's own build produced.

A project's `oss-rulebook-to-xlsx` step writes `<project>/xlsx/rulebook.xlsx`, and
that `/xlsx` step is what makes the harness pick the `xlsx` substrate at all
(shared.get_active_project_substrates). execution-substrates/xlsx/take-test.py
nevertheless read `execution-substrates/xlsx/rulebook.xlsx`, a shared file that is
only right for the project whose injector ran there last. On 2026-11-26 it held
customer-fullname's one-sheet workbook, so star-trek's xlsx score came from
whatever answers an earlier run had left behind (72%), and a clean run scored
1.1% ("Could not find worksheet with column 'CrewAssignmentId'").
effortless-xlsx already resolves its workbook from ERB_DOMAIN_DIR; xlsx now does too.

    python3 scripts/tests/test_xlsx_substrate_reads_project_workbook.py        (or pytest)
"""
import importlib.util
import os
import shutil
import tempfile
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
TAKE_TEST = REPO / "execution-substrates" / "xlsx" / "take-test.py"


def load():
    spec = importlib.util.spec_from_file_location("xlsx_take_test", TAKE_TEST)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


class WhichWorkbook(unittest.TestCase):
    def setUp(self):
        self.project = Path(tempfile.mkdtemp(prefix="erb-xlsx-")).resolve()
        self.addCleanup(shutil.rmtree, self.project, ignore_errors=True)
        self.saved = os.environ.get("ERB_DOMAIN_DIR")
        self.addCleanup(self.restore)

    def restore(self):
        if self.saved is None:
            os.environ.pop("ERB_DOMAIN_DIR", None)
        else:
            os.environ["ERB_DOMAIN_DIR"] = self.saved

    def test_the_project_build_output(self):
        (self.project / "xlsx").mkdir()
        (self.project / "xlsx" / "rulebook.xlsx").write_bytes(b"")
        os.environ["ERB_DOMAIN_DIR"] = str(self.project)
        self.assertEqual(load().workbook_path(), self.project / "xlsx" / "rulebook.xlsx")

    def test_a_project_that_never_built_it_fails_loudly(self):
        os.environ["ERB_DOMAIN_DIR"] = str(self.project)
        with self.assertRaises(SystemExit) as raised:
            load().workbook_path()
        self.assertIn("effortless build", str(raised.exception))

    def test_no_project_named_fails_loudly(self):
        os.environ.pop("ERB_DOMAIN_DIR", None)
        with self.assertRaises(SystemExit):
            load().workbook_path()


if __name__ == "__main__":
    unittest.main()
