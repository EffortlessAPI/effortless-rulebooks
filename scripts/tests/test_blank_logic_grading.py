#!/usr/bin/env python3
"""The harness grades a blank by the project's erbBlankLogic.

Under `coerce` (every project today) a blank in a predicate is FALSE, so a
substrate answering false where the answer key holds a blank computed the same
thing. Under `propagate` (three-valued logic, the open world) a blank means "not
known": a substrate that answers false for it gave a different answer, and the
harness must fail it. The same for numbers: under coerce a blank is 0, under
propagate it is not. Before 2026-11-27 compare_values counted a blank as false or
zero whatever the project said, so an open-world project could never fail on it.

    python3 scripts/tests/test_blank_logic_grading.py        (or pytest)
"""
import importlib.util
import json
import os
import tempfile
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]


def load_harness(domain: str):
    os.environ["ERB_DOMAIN"] = domain
    spec = importlib.util.spec_from_file_location(
        f"test_orchestrator_{domain.replace('-', '_')}", REPO / "orchestration" / "test-orchestrator.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class BlankLogicGrading(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.harness = load_harness("talismans-southpark-ontology")
        cls.compare = staticmethod(cls.harness.compare_values)

    def test_the_project_setting_is_read(self):
        self.assertEqual(self.harness.BLANK_LOGIC, "coerce")

    def test_coerce_a_blank_boolean_is_false(self):
        for blank in (None, ""):
            self.assertTrue(self.compare(False, blank, "boolean", "coerce"))
            self.assertTrue(self.compare(blank, False, "boolean", "coerce"))
            self.assertFalse(self.compare(True, blank, "boolean", "coerce"))

    def test_propagate_a_blank_is_not_known(self):
        for blank in (None, ""):
            self.assertFalse(self.compare(False, blank, "boolean", "propagate"))
            self.assertFalse(self.compare(blank, False, "boolean", "propagate"))
            self.assertFalse(self.compare(True, blank, "boolean", "propagate"))
            self.assertTrue(self.compare(blank, None, "boolean", "propagate"))
        self.assertTrue(self.compare(False, "false", "boolean", "propagate"))
        self.assertTrue(self.compare(True, "t", "boolean", "propagate"))

    def test_coerce_a_blank_number_is_zero(self):
        self.assertTrue(self.compare(0, None, "integer", "coerce"))
        self.assertTrue(self.compare("0.00", "", "number", "coerce"))

    def test_propagate_a_blank_number_is_not_zero(self):
        self.assertFalse(self.compare(0, None, "integer", "propagate"))
        self.assertFalse(self.compare(None, "0E-20", "number", "propagate"))
        self.assertTrue(self.compare(None, "", "number", "propagate"))
        self.assertTrue(self.compare(None, "NULL", "number", "propagate"))
        self.assertTrue(self.compare(0, "0.00", "number", "propagate"))

    def test_an_unknown_setting_is_refused(self):
        with self.assertRaises(ValueError):
            self.compare(True, True, "boolean", "maybe")

    def test_a_propagate_project_grades_under_propagate(self):
        # A temporary project folder under toy-rulebooks, removed afterwards.
        domain = "zz-blank-logic-probe"
        folder = REPO / "toy-rulebooks" / domain
        self.assertFalse(folder.exists(), f"{folder} already exists")
        try:
            (folder / "effortless-rulebook").mkdir(parents=True)
            (folder / "effortless-rulebook" / f"{domain}-rulebook.json").write_text("{}")
            (folder / "effortless.json").write_text(json.dumps(
                {"ProjectSettings": [{"Name": "erbBlankLogic", "Value": "propagate"}]}))
            harness = load_harness(domain)
            self.assertEqual(harness.BLANK_LOGIC, "propagate")
            self.assertFalse(harness.compare_values(False, None, "boolean"))
        finally:
            for p in sorted(folder.rglob("*"), reverse=True):
                p.unlink() if p.is_file() else p.rmdir()
            folder.rmdir()


if __name__ == "__main__":
    unittest.main()
