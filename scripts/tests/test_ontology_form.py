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
  its concept rulebook: Things, a table per class, a table or a column per
  property, and ERBTables and ERBFields for what the structure cannot say. Nothing
  rides in a blob. Laid back over her ontology unchanged, it gives her file back,
  and her candidate generator (app/lib/candidates.mjs) names exactly the candidate
  rows her file holds.
* An edit in the concept rulebook (a new person, a fact, a class, a relationship, a
  column, a symmetric or an inverse property) is the matching change of the
  ontology's rows and nothing else changes.
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
        self.assertEqual(c["hasAge"]["data"][0]["Value"], 10)
        # The concept rulebook holds concepts: none of the model's or the page's tables.
        tables = [k for k, v in c.items() if of.is_table(v)]
        self.assertEqual(tables, ["__meta__", "Things", "Person", "School", "isParentOf", "isChildOf",
                                  "isSpouseOf", "isFriendOf", "attends", "hasAge", "isGrandparentOf",
                                  "Portraits", "ERBTables", "ERBFields"])

    def test_the_structure_says_what_it_can(self):
        c = self.concepts
        attends = {f["name"]: f.get("RelatedTo") for f in c["attends"]["schema"]}
        self.assertEqual((attends["Subject"], attends["Object"]), ("Person", "School"))   # domain, range
        parent = {f["name"]: f.get("RelatedTo") for f in c["isParentOf"]["schema"]}
        self.assertEqual((parent["Subject"], parent["Object"]), ("Things", "Things"))     # neither
        value = next(f for f in c["hasAge"]["schema"] if f["name"] == "Value")
        self.assertEqual((value["type"], value["datatype"]), ("raw", "integer"))          # a datatype property
        described = of.describe(c, TEMPLATE)
        roles = {t["name"]: t["role"] for t in described["tables"]}
        self.assertEqual((roles["Person"], roles["isParentOf"], roles["hasAge"], roles["Things"]),
                         ("class", "relationship", "values", "individuals"))
        grade = next(p for p in described["properties"] if p["id"] == "hasGradeLevel")
        self.assertEqual((grade["home"], grade["table"], grade["single"], grade["domain"], grade["datatype"]),
                         ("column", "Person", True, "Person", "integer"))
        # So no attribute column says it again: what a property is, is not a cell.
        for name in ("ERBTables", "ERBFields"):
            names = {f["name"] for f in c[name]["schema"]}
            self.assertFalse(names & {"PropertyKind", "HasOneCurrentValue", "DomainClass", "RangeClass", "Kind"}, name)

    def test_what_it_cannot_say_is_rows_of_erbtables_and_erbfields_not_a_blob(self):
        c = self.concepts
        tables = {r["ERBTableId"]: r for r in c["ERBTables"]["data"]}
        self.assertEqual((tables["isParentOf"]["Label"], tables["isParentOf"]["InverseOf"],
                          tables["isParentOf"]["DisjointWith"]), ("is parent of", "isChildOf", "isSpouseOf"))
        self.assertTrue(tables["isSpouseOf"]["IsSymmetric"])
        self.assertFalse(tables["isParentOf"]["IsSymmetric"])
        self.assertEqual((tables["isGrandparentOf"]["ChainFirst"], tables["isGrandparentOf"]["ChainSecond"]),
                         ("isParentOf", "isParentOf"))
        self.assertEqual((tables["Person"]["Label"], tables["Person"]["SortOrder"]), ("Person", 8))
        fields = {r["ERBFieldId"]: r for r in c["ERBFields"]["data"]}
        self.assertEqual([k for k, r in fields.items() if r["Label"]], ["Person.hasGradeLevel", "Person.hasRole"])
        self.assertEqual(fields["Person.hasGradeLevel"]["CurrentValueWording"], "grade level")
        # The one thing __meta__ says is which form this is, as a string.
        marker = [r for r in c["__meta__"]["data"] if r["MetaKey"] == of.FORM_META_KEY]
        self.assertEqual([(r["ValueType"], r["StringValue"], r["JsonValue"]) for r in marker],
                         [("string", of.FORM_NAME, None)])
        for row in c["__meta__"]["data"]:
            self.assertNotIn("properties", row.get("JsonValue") or "")

    def test_erbtables_and_erbfields_are_the_platforms_schema_module(self):
        """One row per table and one per field, with the module's own columns first: its
        Sync, and whatever points at these rows, work on the concept rulebook as on any other."""
        c = self.concepts
        named = [k for k, v in c.items() if of.is_table(v) and k != "__meta__"]
        self.assertEqual([r["ERBTableId"] for r in c["ERBTables"]["data"]], named)
        self.assertEqual([r["ERBFieldId"] for r in c["ERBFields"]["data"]],
                         [f"{t}.{f['name']}" for t in named for f in c[t]["schema"]])
        self.assertEqual([f["name"] for f in c["ERBTables"]["schema"]][:8],
                         ["ERBTableId", "TableName", "Description", "IsMetaTable", "IsTenantScoped",
                          "TenantParentTable", "TenantParentField", "SortOrder"])
        self.assertEqual([f["name"] for f in c["ERBFields"]["schema"]][:7],
                         ["ERBFieldId", "ERBTable", "FieldName", "FieldType", "Datatype", "IsPrimaryKey", "Description"])
        self.assertEqual([r["ERBTableId"] for r in c["ERBTables"]["data"] if r["IsMetaTable"]], ["ERBTables", "ERBFields"])
        for row in c["ERBFields"]["data"]:
            field = next(f for f in c[row["ERBTable"]]["schema"] if f["name"] == row["FieldName"])
            self.assertEqual((row["FieldType"], row["Datatype"], row["Description"]),
                             (field["type"], field["datatype"], field["Description"]), row["ERBFieldId"])
            self.assertEqual(row["IsPrimaryKey"], c[row["ERBTable"]]["schema"][0] is field)
        for row in c["ERBTables"]["data"]:
            self.assertEqual(row["Description"], c[row["ERBTableId"]]["Description"])

    def test_a_mirror_row_that_is_missing_or_stale_says_nothing(self):
        """The module's rows drift until its Sync runs; only the ontology's own cells count."""
        doc = json.loads(of.canonical(self.concepts))
        doc["ERBFields"]["data"] = [r for r in doc["ERBFields"]["data"] if r["Label"]]       # only the property columns
        doc["ERBTables"]["data"] = [r for r in doc["ERBTables"]["data"] if r["ERBTableId"] != "Portraits"]
        doc["ERBTables"]["data"].append({**doc["ERBTables"]["data"][0], "ERBTableId": "Gone", "TableName": "Gone"})
        doc["ERBFields"]["data"].append({**doc["ERBFields"]["data"][0], "ERBFieldId": "Gone.Field", "ERBTable": "Gone"})
        self.assertEqual(of.canonical(of.inject(doc, TEMPLATE)), of.canonical(TEMPLATE))

    def test_unchanged_it_gives_her_ontology_back_whole(self):
        self.assertEqual(of.canonical(self.back), of.canonical(TEMPLATE))

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

    def test_an_earlier_blob_is_refused_by_name(self):
        old = json.loads(of.canonical(self.concepts))
        for row in old["__meta__"]["data"]:
            if row["MetaKey"] == of.FORM_META_KEY:
                row.update(ValueType="object", StringValue=None, JsonValue='{"form":"concepts-of-an-ontology"}')
        with self.assertRaisesRegex(of.OntologyFormError, "earlier version"):
            of.inject(old, TEMPLATE)


def stored(table: dict) -> dict:
    key = table["schema"][0]["name"]
    return {row[key]: of._stored_row(table, row) for row in table["data"]}


def difference(before: dict, after: dict) -> dict:
    """Which stored rows differ, per table: {table: {"+": [...], "-": [...], "~": {key: {field: (old, new)}}}}."""
    out = {}
    for name, table in before.items():
        if not of.is_table(table):
            continue
        old, new = stored(table), stored(after[name])
        entry = {"+": sorted(k for k in new if k not in old), "-": sorted(k for k in old if k not in new),
                 "~": {k: {f: (old[k][f], new[k][f]) for f in new[k] if old[k][f] != new[k][f]}
                       for k in new if k in old and old[k] != new[k]}}
        if any(entry.values()):
            out[name] = {k: v for k, v in entry.items() if v}
    return out


class EditsInTheConceptRulebookAreChangesOfTheOntology(unittest.TestCase):
    """Each edit is made in the concept rulebook; what changed is read off the ontology."""

    def setUp(self):
        self.concepts = through_file(of.extract(TEMPLATE))
        graph = TEMPLATE["Graphs"]["data"][0]
        self.seq, self.listed = graph["NextActivationSeq"], graph["NextListedSortOrder"]

    def after(self, *edits):
        edited = of.edit_concepts(self.concepts, list(edits), TEMPLATE)
        return through_file(of.inject(through_file(edited), TEMPLATE))

    def changed(self, *edits):
        return difference(TEMPLATE, self.after(*edits))

    def test_a_new_person_with_a_friend(self):
        d = self.changed(
            {"op": "insert", "table": "Things", "values": {"ThingId": "Butters", "Label": "Butters"}},
            {"op": "insert", "table": "Person", "values": {"Thing": "Butters"}},
            {"op": "insert", "table": "isFriendOf", "values": {"Subject": "Butters", "Object": "Stan"}})
        self.assertEqual(sorted(d), ["Statements", "Things"])
        self.assertEqual(d["Things"], {"+": ["Butters"]})
        self.assertEqual(d["Statements"], {"+": ["Butters|isA|Person", "Butters|isFriendOf|Stan"]})
        after = self.after(
            {"op": "insert", "table": "Things", "values": {"ThingId": "Butters", "Label": "Butters"}},
            {"op": "insert", "table": "Person", "values": {"Thing": "Butters"}},
            {"op": "insert", "table": "isFriendOf", "values": {"Subject": "Butters", "Object": "Stan"}})
        butters = stored(after["Things"])["Butters"]
        self.assertEqual((butters["Kind"], butters["SortOrder"]), ("individual", 10))   # after the 9 there
        friend = stored(after["Statements"])["Butters|isFriendOf|Stan"]
        self.assertEqual((friend["Subject"], friend["Property"], friend["ObjectThing"], friend["ObjectValue"]),
                         ("Butters", "isFriendOf", "Stan", None))
        # Stated by the reader, at the base's own next numbers (the membership took the first).
        self.assertEqual((friend["IsStated"], friend["IsListed"], friend["IsBuiltIn"]), (True, True, False))
        self.assertEqual((friend["ActivationSeq"], friend["SortOrder"]), (self.seq + 1, self.listed + 1))

    def test_a_fact_removed_is_switched_off_and_stays_in_the_pages_list(self):
        d = self.changed({"op": "remove", "table": "isParentOf", "key": "Marvin|isParentOf|Randy"})
        self.assertEqual(d, {"Statements": {"~": {"Marvin|isParentOf|Randy": {"IsStated": (True, False)}}}})

    def test_a_fact_that_was_a_candidate_is_switched_on_and_listed(self):
        d = self.changed({"op": "insert", "table": "isParentOf", "values": {"Subject": "Sharon", "Object": "Stan"}})
        row = d["Statements"]["~"]["Sharon|isParentOf|Stan"]
        self.assertEqual(d, {"Statements": {"~": {"Sharon|isParentOf|Stan": row}}})
        self.assertEqual((row["IsStated"], row["IsListed"], row["ActivationSeq"]),
                         ((False, True), (False, True), (0, self.seq)))
        self.assertEqual(row["SortOrder"][1], self.listed)

    def test_a_cell_changed_replaces_the_one_current_value(self):
        d = self.changed({"op": "patch", "table": "Person", "key": "Stan", "values": {"hasGradeLevel": 5}})
        self.assertEqual(sorted(d["Statements"]["~"]), ["Stan|hasGradeLevel|4", "Stan|hasGradeLevel|5"])
        self.assertEqual(d["Statements"]["~"]["Stan|hasGradeLevel|4"], {"IsStated": (True, False)})
        self.assertEqual(d["Statements"]["~"]["Stan|hasGradeLevel|5"]["IsStated"], (False, True))
        self.assertEqual(list(d), ["Statements"])

    def test_a_new_class_and_a_relationship_to_it(self):
        after = self.after(
            {"op": "addClass", "name": "Concert", "label": "Concert"},
            {"op": "addProperty", "name": "performsAt", "label": "performs at", "object": "Concert"},
            {"op": "insert", "table": "Things", "values": {"ThingId": "Lollapalooza"}},
            {"op": "insert", "table": "performsAt", "values": {"Subject": "Kyle", "Object": "Lollapalooza"}})
        d = difference(TEMPLATE, after)
        self.assertEqual(sorted(d), ["Properties", "Statements", "Things"])
        self.assertEqual(d["Things"], {"+": ["Concert", "Lollapalooza"]})
        concert = stored(after["Things"])["Concert"]
        self.assertEqual((concert["Kind"], concert["Label"]), ("class", "Concert"))
        self.assertEqual(d["Properties"], {"+": ["performsAt"]})
        prop = stored(after["Properties"])["performsAt"]
        self.assertEqual((prop["PropertyKind"], prop["Label"], prop["HasOneCurrentValue"], prop["DomainClass"],
                          prop["RangeClass"], prop["IsUserAddable"], prop["IsSymmetric"], prop["SortOrder"]),
                         ("object", "performs at", False, None, "Concert", True, False, 11))
        self.assertEqual(d["Statements"], {"+": ["Kyle|performsAt|Lollapalooza"]})
        # And the concept rulebook of that ontology reads the same again (the facts aside:
        # which are in effect is the next build's to compute).
        again = of.describe(of.edit_concepts(self.concepts, [
            {"op": "addClass", "name": "Concert"},
            {"op": "addProperty", "name": "performsAt", "object": "Concert"}], TEMPLATE), TEMPLATE)
        performs = next(p for p in again["properties"] if p["id"] == "performsAt")
        self.assertEqual((performs["kind"], performs["range"], performs["single"]), ("object", "Concert", False))

    def test_a_link_pointed_at_things_loses_its_range(self):
        d = self.changed({"op": "retarget", "table": "attends", "field": "Object", "relatedTo": "Things"})
        self.assertEqual(d, {"Properties": {"~": {"attends": {"RangeClass": ("School", None)}}}})

    def test_symmetric_and_inverse_are_cells_of_erbtables(self):
        d = self.changed(
            {"op": "patch", "table": "ERBTables", "key": "isFriendOf", "values": {"IsSymmetric": False}},
            {"op": "addProperty", "name": "employs", "label": "employs"},
            {"op": "addProperty", "name": "worksFor", "label": "works for"},
            {"op": "patch", "table": "ERBTables", "key": "employs", "values": {"InverseOf": "worksFor"}},
            {"op": "patch", "table": "ERBTables", "key": "worksFor", "values": {"InverseOf": "employs"}})
        self.assertEqual(d["Properties"]["~"], {"isFriendOf": {"IsSymmetric": (True, False)}})
        self.assertEqual(d["Properties"]["+"], ["employs", "worksFor"])
        self.assertEqual(d["PropertyAxioms"], {"+": ["inverse:employs", "inverse:worksFor"]})
        after = self.after(
            {"op": "addProperty", "name": "employs"}, {"op": "addProperty", "name": "worksFor"},
            {"op": "patch", "table": "ERBTables", "key": "employs", "values": {"InverseOf": "worksFor"}})
        axiom = stored(after["PropertyAxioms"])["inverse:employs"]
        self.assertEqual((axiom["Kind"], axiom["Property"], axiom["First"], axiom["Second"]),
                         ("inverse", "employs", "worksFor", None))

    def test_a_new_column_is_a_property_with_one_current_value(self):
        after = self.after(
            {"op": "addColumn", "table": "Person", "name": "favoriteFood", "label": "favorite food", "datatype": "string"},
            {"op": "patch", "table": "Person", "key": "Stan", "values": {"favoriteFood": "Cheesy Poofs"}})
        d = difference(TEMPLATE, after)
        self.assertEqual(d["Properties"], {"+": ["favoriteFood"]})
        prop = stored(after["Properties"])["favoriteFood"]
        self.assertEqual((prop["PropertyKind"], prop["Datatype"], prop["HasOneCurrentValue"], prop["DomainClass"],
                          prop["Label"]), ("datatype", "xsd:string", True, "Person", "favorite food"))
        self.assertEqual(d["Statements"], {"+": ["Stan|favoriteFood|Cheesy Poofs"]})
        self.assertEqual(sorted(d), ["Properties", "Statements"])

    def test_a_property_removed_takes_its_axioms_and_facts_and_no_others(self):
        d = self.changed({"op": "removeTable", "table": "isChildOf"})
        self.assertEqual(d["Properties"], {"-": ["isChildOf"]})
        self.assertEqual(d["PropertyAxioms"], {"-": ["inverse:isChildOf", "inverse:isParentOf"]})
        self.assertTrue(d["Statements"]["-"])
        self.assertTrue(all("|isChildOf|" in sid for sid in d["Statements"]["-"]))
        self.assertEqual(sorted(d), ["Properties", "PropertyAxioms", "Statements"])

    def test_an_individual_removed_takes_what_was_said_of_it(self):
        d = self.changed({"op": "remove", "table": "Things", "key": "Kyle"})
        self.assertEqual(d["Things"], {"-": ["Kyle"]})
        self.assertIn("Stan|isFriendOf|Kyle", d["Statements"]["-"])
        self.assertTrue(all("Kyle" in sid.split("|")[0::2] for sid in d["Statements"]["-"]))
        self.assertEqual(sorted(d), ["Statements", "Things"])

    def test_a_class_removed_leaves_its_links_pointing_at_any_thing(self):
        d = self.changed({"op": "removeTable", "table": "School"})
        self.assertEqual(d["Things"], {"-": ["School"]})
        self.assertEqual(d["Properties"], {"~": {"attends": {"RangeClass": ("School", None)}}})

    def test_what_the_ontology_cannot_hold_is_refused_in_words(self):
        def broken(change, message):
            doc = json.loads(of.canonical(self.concepts))
            change(doc)
            with self.assertRaisesRegex(of.OntologyFormError, message):
                of.inject(doc, TEMPLATE)

        broken(lambda c: c.update(Notes={"schema": [{"name": "NoteId", "type": "raw", "datatype": "string"}], "data": []}),
               "neither a class nor a property")
        broken(lambda c: c["isParentOf"]["data"].append({"isParentOfId": "x", "Subject": "Nobody", "Object": "Stan"}),
               "'Nobody', which is not in Things")
        broken(lambda c: c["Person"]["schema"].append({"name": "Twice", "type": "calculated", "datatype": "integer",
                                                        "formula": "={{hasGradeLevel}} * 2"}),
               "no place for a formula")
        broken(lambda c: c["ERBTables"]["schema"].append({"name": "IsTransitive", "type": "raw", "datatype": "boolean"}),
               "the model has no IsTransitive")
        broken(lambda c: c["ERBTables"]["data"][3].update(InverseOf="isSiblingOf"),
               "'isSiblingOf', which is not a property")
        broken(lambda c: c["ERBFields"]["schema"].pop(), "ERBFields lacks DisjointWith")
        with self.assertRaisesRegex(of.OntologyFormError, "IsInEffect"):
            of.inject(self.concepts, {**TEMPLATE, "Statements": {**TEMPLATE["Statements"], "data": [
                {k: v for k, v in row.items() if k != "IsInEffect"} for row in TEMPLATE["Statements"]["data"]]}})
        with self.assertRaisesRegex(of.OntologyFormError, "cannot be changed"):
            of.edit_concepts(self.concepts, [{"op": "patch", "table": "Things", "key": "Stan",
                                              "values": {"ThingId": "Stanley"}}], TEMPLATE)
        # A copy of what the structure says is not where to change it.
        with self.assertRaisesRegex(of.OntologyFormError, "follows the table or field"):
            of.edit_concepts(self.concepts, [{"op": "patch", "table": "ERBFields", "key": "hasAge.Value",
                                              "values": {"Datatype": "string"}}], TEMPLATE)
        with self.assertRaisesRegex(of.OntologyFormError, "no place for it"):
            of.edit_concepts(self.concepts, [{"op": "patch", "table": "ERBTables", "key": "Person",
                                              "values": {"IsTenantScoped": True}}], TEMPLATE)


class TheConceptRulebookFileOfSouthPark(unittest.TestCase):
    """rulebook-examples/talismans-southpark-ontology/concept-rulebook/ holds her ontology's
    concept rulebook as a file: what the generated rulebook editor opens there
    (./start.sh --concept-editor). The build's concept-rulebook step writes it
    (scripts/extract-concept-rulebook.py) and so does the app's Save."""

    PROJECT = REPO / "rulebook-examples" / "talismans-southpark-ontology"
    FILE = PROJECT / "concept-rulebook" / "talismans-southpark-ontology-concepts.json"

    def test_the_committed_file_is_the_hubs_concept_form(self):
        self.assertTrue(self.FILE.is_file(), f"{self.FILE} is missing")
        committed = of.read_rulebook(self.FILE)
        self.assertEqual(
            of.canonical(of.inject(committed, TEMPLATE)), of.canonical(TEMPLATE),
            "The concept rulebook file no longer matches the hub: laying it over the hub would "
            "change the ontology. Run scripts/extract-concept-rulebook.py from "
            f"{self.FILE.parent.relative_to(REPO)} (the build's concept-rulebook step), or Save in the app.")

    def test_the_build_step_writes_that_file_and_refuses_the_hubs_folder(self):
        script = REPO / "scripts" / "extract-concept-rulebook.py"
        mode = subprocess.run(["git", "ls-files", "-s", "scripts/extract-concept-rulebook.py"], cwd=REPO,
                              capture_output=True, text=True, check=True).stdout.split(" ")[0]
        self.assertEqual(mode, "100755", "scripts/extract-concept-rulebook.py must be committed executable: -exec runs it directly")
        steps = json.loads((self.PROJECT / "effortless.json").read_text())["ProjectTranspilers"]
        step = next(s for s in steps if s["Name"] == "concept-rulebook")
        self.assertEqual((step["RelativePath"], step["CommandLine"]), ("/concept-rulebook", "-exec ../../../scripts/extract-concept-rulebook.py"))
        names = [s["Name"] for s in steps]
        self.assertLess(names.index("compile-rulebook"), names.index("concept-rulebook"),
                        "the mapping reads the computed values compile-rulebook bakes")
        before = self.FILE.read_text(encoding="utf-8")
        try:
            done = subprocess.run([str(script)], cwd=self.FILE.parent, capture_output=True, text=True)
            self.assertEqual(done.returncode, 0, done.stderr)
            self.assertEqual(json.loads(self.FILE.read_text(encoding="utf-8")), through_file(of.extract(TEMPLATE)))
        finally:
            self.FILE.write_text(before, encoding="utf-8")
        refused = subprocess.run([str(script)], cwd=self.PROJECT / "effortless-rulebook", capture_output=True, text=True)
        self.assertNotEqual(refused.returncode, 0)
        self.assertIn("never beside or over the hub", refused.stderr)


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

    def test_her_ontology_goes_through_both_tools_and_comes_back(self):
        source = of.TEMPLATE
        with tempfile.TemporaryDirectory() as tmp:
            tmp = Path(tmp)
            concepts = self.run_tool("oss-ontology-rulebook-to-rulebook", source, tmp)
            self.assertEqual(concepts.name, f"{source.stem}.concepts.json")
            self.assertNotIn("Statements", json.loads(concepts.read_text()))
            back = self.run_tool("oss-rulebook-to-ontology-rulebook", concepts, tmp)
            self.assertEqual(json.loads(back.read_text()), json.loads(source.read_text()))

    def test_the_command_line_shows_and_edits_the_concept_rulebook(self):
        run = lambda *args: subprocess.run([sys.executable, str(REPO / "orchestration/ontology_form.py"), *map(str, args)],
                                           capture_output=True, text=True)
        shown = run("view", of.TEMPLATE)
        self.assertEqual(shown.returncode, 0, shown.stderr)
        view = json.loads(shown.stdout)
        self.assertEqual(view["description"]["form"], of.FORM_NAME)
        self.assertIn("isParentOf", view["concepts"])
        with tempfile.TemporaryDirectory() as tmp:
            edits = Path(tmp) / "edits.json"
            edits.write_text(json.dumps([{"op": "addClass", "name": "Concert"}]))
            edited = run("edit", of.TEMPLATE, edits)
            self.assertEqual(edited.returncode, 0, edited.stderr)
            things = {t["ThingId"]: t for t in json.loads(edited.stdout)["ontology"]["Things"]["data"]}
            self.assertEqual(things["Concert"]["Kind"], "class")
            edits.write_text(json.dumps([{"op": "addClass", "name": "Person"}]))
            refused = run("edit", of.TEMPLATE, edits)
            self.assertEqual((refused.returncode, refused.stdout), (1, ""))
            self.assertIn("already is a table Person", refused.stderr)


if __name__ == "__main__":
    unittest.main()
