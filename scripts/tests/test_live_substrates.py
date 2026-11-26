#!/usr/bin/env python3
"""orchestration/live_substrates.py runs a substrate on rows as they are now, and
says when substrates disagree.

An app over a rulebook changes rows; the conformance harness only ever saw the
rows the rulebook stores. These tests take a real project
(rulebook-examples/talismans-southpark-ontology, whose page a person edits),
change its rows the way the page does, and check that:

  - the generated TypeScript and the generated OWL rules both follow the change
    (a rule switched on produces its conclusions on the live rows);
  - the two agree on every computed cell, by the harness's own compare_values;
  - a wrong answer is reported as a difference, naming the cell;
  - rows for a table the rulebook lacks, or a missing table, are refused.

Run: python3 scripts/tests/test_live_substrates.py
(needs node and npm for the TypeScript substrate, rdflib and pyoxigraph for OWL,
and the project built: effortless-typescript/ and effortless-owl/ present)
"""
import copy
import json
import sys
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "orchestration"))
import live_substrates as live  # noqa: E402

PROJECT = ROOT / "rulebook-examples" / "talismans-southpark-ontology"


def stored_rows(rulebook):
    return {name: [{f: row.get(f) for f in live.stored_fields(table)} for row in table["data"]]
            for name, table in live.tables_of(rulebook).items()}


class LiveSubstrates(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.rulebook = live.load_rulebook(PROJECT)
        cls.seed = stored_rows(cls.rulebook)
        # The page's "tick the Inverse rule": one stored value changes.
        cls.inverse_on = copy.deepcopy(cls.seed)
        for rule in cls.inverse_on["Rules"]:
            if rule["RuleId"] == "inverse":
                rule["IsEnabled"] = True
        cls.answers = {s: live.evaluate(PROJECT, s, cls.inverse_on)["tables"]
                       for s in ("effortless-typescript", "effortless-owl")}

    def conclusions(self, tables):
        return sorted(r["Name"] for r in tables["Statements"] if r["IsConclusion"] in (True, "true"))

    def test_live_rows_are_what_is_computed(self):
        expected = ["Randy is child of Marvin", "Shelly is child of Randy", "Stan is child of Randy"]
        for substrate, tables in self.answers.items():
            self.assertEqual(self.conclusions(tables), expected, substrate)
        seed = live.evaluate(PROJECT, "effortless-typescript", self.seed)["tables"]
        self.assertEqual(self.conclusions(seed), [], "the stored rows have every rule off")

    def test_substrates_agree_cell_by_cell(self):
        result = live.compare(PROJECT, self.answers)
        self.assertGreater(result["cells"], 10000)
        self.assertEqual(result["missingRows"], [])
        self.assertEqual(result["differing"], [], json.dumps(result["differing"][:5], indent=1))

    def test_a_wrong_answer_is_named(self):
        wrong = copy.deepcopy(self.answers)
        row = next(r for r in wrong["effortless-owl"]["Statements"] if r["StatementId"] == "Randy|isChildOf|Marvin")
        row["Holds"] = False
        result = live.compare(PROJECT, wrong)
        self.assertEqual([(d["table"], d["key"], d["field"]) for d in result["differing"]],
                         [("Statements", "Randy|isChildOf|Marvin", "Holds")])

    def test_rows_must_match_the_rulebook(self):
        extra = dict(self.seed, Ghosts=[])
        with self.assertRaisesRegex(live.LiveSubstrateError, "does not have.*Ghosts"):
            live.evaluate(PROJECT, "effortless-typescript", extra)
        fewer = {k: v for k, v in self.seed.items() if k != "Rules"}
        with self.assertRaisesRegex(live.LiveSubstrateError, "missing tables.*Rules"):
            live.evaluate(PROJECT, "effortless-typescript", fewer)


if __name__ == "__main__":
    unittest.main(verbosity=2)
