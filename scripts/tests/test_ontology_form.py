#!/usr/bin/env python3
"""The concept form and the ontology form are one rulebook, both ways.

orchestration/ontology_form.py maps any rulebook into the ontology form of
Jessica Talisman's South Park explainer (rulebook-examples/talismans-southpark-
ontology: classes and individuals in Things, fields in Properties, cells in
Statements) and back. The two tools in effortless-tools/ call it:
oss-rulebook-to-ontology-rulebook and oss-ontology-rulebook-to-rulebook.

What holds here:

* Every rulebook in the corpus (each project's hub and the root's) goes to the
  ontology form and back to the same document, key order and JSON types included,
  through a real write and read of the ontology file in between.
* The ontology form holds the rulebook as rows, not as a copy: its tables, fields,
  records and links are classes, properties, individuals and statements, and a
  changed statement changes the rulebook that comes back.
* South Park itself, an ontology that was never a concept rulebook, extracts to
  concept tables (People, School, a table per many-valued property) and injects
  back to every stored row of her model, and her candidate generator
  (app/lib/candidates.mjs) names exactly the candidate rows her file holds.
* The two tools work under the CLI's script contract, and the output of one fed
  to the other is the input byte for byte.

    python3 scripts/tests/test_ontology_form.py        (or pytest)
"""
import json
import os
import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "orchestration"))
import ontology_form as of  # noqa: E402

TEMPLATE = of.load_template()


def corpus() -> list:
    """Each project's hub rulebook, and the root's."""
    hubs = []
    for area in ("rulebook-examples", "toy-rulebooks"):
        for project in sorted((REPO / area).iterdir()):
            folder = project / "effortless-rulebook"
            if not folder.is_dir():
                continue
            for f in sorted(folder.glob("*.json")):
                if f.name == "effortless-rulebook.json" or f.name.endswith("-rulebook.json"):
                    hubs.append(f)
    hubs.append(REPO / "effortless-rulebook" / "effortless-rulebook.json")
    return hubs


def through_file(document: dict) -> dict:
    """What a reader of the written file gets back."""
    return json.loads(of.canonical(document))


class CorpusRoundTrip(unittest.TestCase):
    def test_every_rulebook_comes_back_unchanged(self):
        hubs = corpus()
        self.assertGreater(len(hubs), 30, "the corpus was not found")
        for path in hubs:
            with self.subTest(rulebook=str(path.relative_to(REPO))):
                source = of.read_rulebook(path)
                ontology = through_file(of.inject(source, TEMPLATE))
                back = of.extract(ontology)
                self.assertEqual(of.canonical(back), of.canonical(source))
                # And the ontology an inject wrote is a fixed point the other way round.
                self.assertEqual(of.canonical(of.inject(back, TEMPLATE)), of.canonical(ontology))


class OntologyFormHoldsTheRulebookAsRows(unittest.TestCase):
    def setUp(self):
        self.source = of.read_rulebook(
            REPO / "toy-rulebooks/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json")
        self.ontology = through_file(of.inject(self.source, TEMPLATE))

    def test_the_model_is_the_south_park_model_plus_the_carriers(self):
        for name in of.MODEL_TABLES:
            mine = [f["name"] for f in self.ontology[name]["schema"]]
            hers = [f["name"] for f in TEMPLATE[name]["schema"]]
            added = [f for f, _ in of.CARRIER_FIELDS.get(name, [])]
            self.assertEqual(mine, hers + added, name)

    def test_tables_records_fields_and_cells_are_rows(self):
        o, s = self.ontology, self.source
        tables = [k for k, v in s.items() if of.is_table(v) and k != of.META_TABLE]
        classes = [t["ThingId"] for t in o["Things"]["data"] if t["Kind"] == "class"]
        self.assertEqual(classes, tables)
        records = sum(len(s[t]["data"]) for t in tables)
        self.assertEqual(sum(t["Kind"] == "individual" for t in o["Things"]["data"]), records)
        fields = sum(len(s[t]["schema"]) for t in tables)
        self.assertEqual(len(o["Properties"]["data"]), fields + 1)  # + isA
        cells = sum(len(r) for t in tables for r in s[t]["data"])
        statements = o["Statements"]["data"]
        self.assertGreaterEqual(len(statements), cells)  # a many-valued link is one per target

    def test_links_are_links_and_inverse_fields_are_inverse_axioms(self):
        things = {t["ThingId"] for t in self.ontology["Things"]["data"]}
        links = [x for x in self.ontology["Statements"]["data"]
                 if x["ObjectThing"] and x["Property"] != "isA"]
        self.assertTrue(links)
        for x in links:
            self.assertIn(x["ObjectThing"], things)
            self.assertIsNone(x["ObjectValue"])
        axioms = {(a["Property"], a["First"]) for a in self.ontology["PropertyAxioms"]["data"]}
        self.assertIn(("Projects.ApprovedBy", "Employees.Projects"), axioms)

    def test_a_formula_rides_along_and_its_value_is_not_stated(self):
        p = {x["PropertyId"]: x for x in self.ontology["Properties"]["data"]}
        computed = [x for x in p.values() if x["Formula"]]
        self.assertTrue(computed)
        ids = {x["PropertyId"] for x in computed}
        for x in self.ontology["Statements"]["data"]:
            if x["Property"] in ids:
                self.assertFalse(x["IsStated"], x["StatementId"])

    def test_a_changed_statement_changes_the_rulebook(self):
        changed = json.loads(of.canonical(self.ontology))
        for x in changed["Statements"]["data"]:
            if x["Property"] == "Projects.Name":
                x["ObjectValue"] = "Renamed in the ontology"
                break
        back = of.extract(changed)
        names = [r["Name"] for r in back["Projects"]["data"]]
        self.assertIn("Renamed in the ontology", names)


class EdgeCases(unittest.TestCase):
    def rulebook(self, rows, schema=None, **table):
        schema = schema or [
            {"name": "ThingyId", "datatype": "string", "type": "raw"},
            {"name": "Count", "datatype": "integer", "type": "raw"},
            {"name": "Ratio", "datatype": "number", "type": "raw"},
            {"name": "On", "datatype": "boolean", "type": "raw"},
            {"name": "Parent", "datatype": "string", "type": "relationship", "RelatedTo": "Thingies"},
            {"name": "Doubled", "datatype": "integer", "type": "calculated", "formula": "={{Count}} * 2"},
        ]
        return {"Name": "Edge", "Thingies": {"schema": schema, "data": rows, **table}}

    def roundtrip(self, rulebook):
        back = of.extract(through_file(of.inject(rulebook, TEMPLATE)))
        self.assertEqual(of.canonical(back), of.canonical(rulebook))

    def test_types_nulls_missing_keys_order_and_extra_keys(self):
        self.roundtrip(self.rulebook([
            {"ThingyId": "a", "Count": 3, "Ratio": 3.0, "On": True, "Parent": None, "Doubled": 6},
            {"Count": "7", "ThingyId": "b", "Ratio": 1e-05, "On": False, "Parent": "a"},
            {"ThingyId": "c", "Count": None, "Ratio": [1, 2], "On": "yes", "Parent": "a, b",
             "Unlisted": {"x": 1}},
            {"ThingyId": "d", "Parent": "nobody", "Ratio": 0},
            {"ThingyId": "e", "Parent": "a, a", "Count": True},
        ], important=True))

    def test_numeric_keys_and_a_link_to_one(self):
        schema = [{"name": "N", "datatype": "integer", "type": "raw"},
                  {"name": "Next", "datatype": "integer", "type": "relationship", "RelatedTo": "Thingies"}]
        self.roundtrip(self.rulebook([{"N": 1, "Next": 2}, {"N": 2, "Next": "1"}], schema=schema))

    def test_a_duplicate_key_is_refused(self):
        with self.assertRaises(of.OntologyFormError):
            of.inject(self.rulebook([{"ThingyId": "a"}, {"ThingyId": "a"}]), TEMPLATE)

    def test_a_missing_key_is_refused(self):
        with self.assertRaises(of.OntologyFormError):
            of.inject(self.rulebook([{"Count": 1}]), TEMPLATE)


class SouthParkItself(unittest.TestCase):
    def setUp(self):
        self.concepts = through_file(of.extract(TEMPLATE))
        self.back = through_file(of.inject(self.concepts, TEMPLATE))

    def test_her_ontology_becomes_concept_tables(self):
        c = self.concepts
        self.assertEqual([r["PersonId"] for r in c["Person"]["data"]], ["Stan"])
        self.assertEqual(c["Person"]["data"][0]["hasGradeLevel"], 4)
        self.assertEqual(c["Person"]["data"][0]["hasRole"], "Student")
        self.assertEqual([(r["SchoolId"], r["IsAsserted"]) for r in c["School"]["data"]],
                         [("SouthParkElementary", False)])
        self.assertEqual([(r["Subject"], r["Object"]) for r in c["isParentOf"]["data"]],
                         [("Marvin", "Randy"), ("Randy", "Stan"), ("Randy", "Shelly")])
        attends = {f["name"]: f.get("RelatedTo") for f in c["attends"]["schema"]}
        self.assertEqual((attends["Subject"], attends["Object"]), ("Person", "School"))
        self.assertEqual(c["hasAge"]["data"][0]["Value"], 10)
        for generated in ("Statements", "Firings", "DisjointChecks", "Properties"):
            self.assertNotIn(generated, c)

    def test_and_back_every_stored_row_of_her_model(self):
        sp, back = TEMPLATE, self.back
        self.assertEqual(list(back), list(sp))
        for name, table in sp.items():
            with self.subTest(table=name):
                if not of.is_table(table):
                    self.assertEqual(back[name], table)
                elif name == "Statements":
                    kept = [of._stored_row(table, r) for r in table["data"]
                            if r["IsListed"] or r["IsStated"] or r["ObjectThing"] is None]
                    self.assertEqual(back[name]["data"], kept)
                    self.assertEqual(back[name]["schema"], table["schema"])
                elif name in of.GENERATED_TABLES:
                    self.assertEqual(back[name]["schema"], table["schema"])
                elif name in ("Things", "Properties", "PropertyAxioms"):
                    self.assertEqual(back[name]["data"], [of._stored_row(table, r) for r in table["data"]])
                    self.assertEqual(back[name]["schema"], table["schema"])
                else:
                    self.assertEqual(back[name], table)

    @unittest.skipUnless(shutil.which("node"), "node is needed to run her candidate generator")
    def test_her_generator_names_the_candidate_rows_her_file_holds(self):
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "back.json"
            of.write_rulebook(self.back, path)
            out = subprocess.run(["node", str(REPO / "scripts/ontology-form-candidates.mjs"), str(path)],
                                 capture_output=True, text=True, check=True)
        generated = json.loads(out.stdout)
        keys = {"Statements": "StatementId", "Firings": "FiringId", "DisjointChecks": "DisjointCheckId"}
        for name, key in keys.items():
            self.assertEqual(sorted(generated[name]), sorted(r[key] for r in TEMPLATE[name]["data"]), name)


class TheTools(unittest.TestCase):
    def run_tool(self, tool, source: Path, workdir: Path) -> Path:
        inp, out = workdir / f"{tool}-in", workdir / f"{tool}-out"
        inp.mkdir()
        out.mkdir()
        shutil.copy(source, inp / source.name)
        env = {**os.environ, "EFFORTLESS_INPUT_DIR": str(inp), "EFFORTLESS_OUTPUT_DIR": str(out)}
        subprocess.run([sys.executable, str(REPO / "effortless-tools" / tool / "transpiler.py")],
                       env=env, check=True)
        written = [p for p in out.iterdir() if p.name != "effortless-overwrite-modes.json"]
        self.assertEqual(len(written), 1, [p.name for p in out.iterdir()])
        modes = json.loads((out / "effortless-overwrite-modes.json").read_text())
        self.assertEqual(modes, {"**/*": "Always"})
        return written[0]

    def test_one_tool_undoes_the_other_byte_for_byte(self):
        source = REPO / "toy-rulebooks/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json"
        with tempfile.TemporaryDirectory() as tmp:
            tmp = Path(tmp)
            ontology = self.run_tool("oss-rulebook-to-ontology-rulebook", source, tmp)
            self.assertEqual(ontology.name, "acme-corporation-rulebook.ontology.json")
            back = self.run_tool("oss-ontology-rulebook-to-rulebook", ontology, tmp)
            self.assertEqual(back.read_bytes(), source.read_bytes())


if __name__ == "__main__":
    unittest.main()
