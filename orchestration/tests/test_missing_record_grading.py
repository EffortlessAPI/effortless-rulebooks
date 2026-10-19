"""Missing-row regressions using the real answer loader and grading module.

Run: python3 -m unittest discover -s orchestration/tests -p 'test_*.py' -v
Requires psycopg2, like test-orchestrator; no database or build is run.
"""
import importlib.util
import json
import os
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch


ROOT = Path(__file__).resolve().parents[2]


class MissingRecordGradingTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        # Configure the full module's import-time domain resolution with the
        # checked-in ACME fixture. Tests below use their own schema and files.
        spec = importlib.util.spec_from_file_location(
            "grading_under_test", ROOT / "orchestration" / "test-orchestrator.py"
        )
        cls.grader = importlib.util.module_from_spec(spec)
        with patch.dict(os.environ, {
            "ERB_DOMAIN": "acme-llc",
            "ERB_RULEBOOK_PATH": str(ROOT / "toy-rulebooks" / "acme-llc"
                / "effortless-rulebook" / "acme-llc-rulebook.json"),
        }):
            spec.loader.exec_module(cls.grader)

    def setUp(self):
        tmp = tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        self.testing_dir = Path(tmp.name)
        patched_path = patch.object(self.grader, "TESTING_DIR", tmp.name)
        patched_path.start()
        self.addCleanup(patched_path.stop)

    def rulebook(self, datatype):
        return {"Probe": {"schema": [
            {"name": "Id", "type": "raw", "datatype": "string", "nullable": False},
            {"name": "Value", "type": "calculated", "datatype": datatype,
             "formula": "=0"},
        ], "data": []}}

    def grade(self, expected, datatype, substrate="probe"):
        return self.grader.grade_substrate(
            substrate, {"probe": [{"id": "r1", "value": expected}]},
            self.rulebook(datatype),
        )

    def write_answers(self, records, substrate="probe"):
        directory = self.testing_dir / substrate / "test-answers"
        directory.mkdir(parents=True, exist_ok=True)
        (directory / "probe.json").write_text(json.dumps(records), encoding="utf-8")

    def test_missing_outputs_fail_even_for_nullish_expected_values(self):
        expected_values = [
            (0, "number"), (False, "boolean"), (None, "number"),
            ("", "string"), ("0", "string"), ("false", "string"),
            ("n/a", "string"), (8, "number"),
        ]
        for scenario in ("missing_directory", "missing_file", "empty_file", "other_row"):
            if scenario == "missing_file":
                (self.testing_dir / scenario / "test-answers").mkdir(parents=True)
            elif scenario == "empty_file":
                self.write_answers([], scenario)
            elif scenario == "other_row":
                self.write_answers([{"id": "other", "value": 0}], scenario)
            for expected, datatype in expected_values:
                with self.subTest(scenario=scenario, expected=expected, datatype=datatype):
                    result = self.grade(expected, datatype, scenario)
                    self.assertEqual(result["total_fields_tested"], 1)
                    self.assertEqual(result["fields_passed"], 0)
                    self.assertEqual(result["fields_failed"], 1)
                    self.assertEqual(result["by_class"]["calculated"]["failed"], 1)
                    failure = result["entities"]["probe"]["failures"][0]
                    self.assertEqual(failure["reason"],
                        "missing entity file" if scenario in
                        ("missing_directory", "missing_file") else "missing record")

    def test_present_record_keeps_existing_value_normalization(self):
        cases = [
            (0, None, "number"), (False, None, "boolean"),
            (None, None, "number"), ("0", None, "string"),
            (0, "0.00", "number"), (1, "1.0", "number"),
            ("Bobby Smith", " BOBBY  SMITH ", "string"),
            ("2026-07-19T13:00:00-05:00", "2026-07-19 18:00:00", "datetime"),
        ]
        for expected, actual, datatype in cases:
            with self.subTest(expected=expected, actual=actual, datatype=datatype):
                self.write_answers([{"id": "r1", "value": actual}])
                result = self.grade(expected, datatype)
                self.assertEqual(result["fields_passed"], 1)
                self.assertEqual(result["fields_failed"], 0)

    def test_wrong_value_in_present_record_still_fails(self):
        self.write_answers([{"id": "r1", "value": 7}])
        result = self.grade(0, "number")
        self.assertEqual(result["fields_failed"], 1)
        self.assertEqual(result["entities"]["probe"]["failures"][0]["reason"],
                         "wrong/null value")

    def test_missing_and_present_rows_are_counted_separately(self):
        self.write_answers([{"id": "r1", "value": 0}])
        result = self.grader.grade_substrate("probe", {"probe": [
            {"id": "r1", "value": 0}, {"id": "r2", "value": 0},
        ]}, self.rulebook("number"))
        self.assertEqual(result["total_fields_tested"], 2)
        self.assertEqual(result["fields_passed"], 1)
        self.assertEqual(result["fields_failed"], 1)

    def test_malformed_answer_json_still_raises(self):
        self.write_answers([])
        path = self.testing_dir / "probe" / "test-answers" / "probe.json"
        path.write_text("{invalid JSON", encoding="utf-8")
        with self.assertRaises(json.JSONDecodeError):
            self.grade(0, "number")


if __name__ == "__main__":
    unittest.main()
