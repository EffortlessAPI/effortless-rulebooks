#!/usr/bin/env python3
"""One rulebook, two shapes: the concept form and the ontology form.

A rulebook in the CONCEPT form is the ordinary kind: a table per concept, a
column per field, a row per record (``Customers``, ``Orders``...). The ONTOLOGY
form is the model Jessica Talisman's South Park explainer runs on
(``rulebook-examples/talismans-southpark-ontology``): every concept is a class
row of ``Things``, every record an individual row of ``Things``, every field a
row of ``Properties``, every cell a row of ``Statements``, and the reasoning
(inverse, symmetric, property chain, domain and range, disjoint) is the model's
own formulas over those rows.

This module is the ONE implementation of the mapping, both directions in one
file so they cannot drift apart:

* ``inject(rulebook)``  : concept form  -> ontology form
* ``extract(rulebook)`` : ontology form -> concept form

The ontology model is not copied here: its tables, fields and formulas are read
at run time from the South Park rulebook (``TEMPLATE``), the single source of
truth for what the ontology form is. This module adds the four carrier columns
the mapping needs (``CARRIER_FIELDS``): ``Description``, ``FieldType`` and
``Formula`` say what a field is that the five-rule vocabulary cannot (option 1:
a formula outside the vocabulary rides along verbatim, the ontology does not
reason over it), and ``Carry`` holds the format detail a rulebook file has and
an ontology does not (key order, keys no tool reads, a value whose JSON type
differs from its field's datatype). Nothing else is carried: every table,
field, record and value is a row of the model.

The guarantees, each held by ``scripts/tests/test_ontology_form.py``:

1. ``extract(inject(R)) == R`` for every rulebook R in the corpus: the same
   document, key order and JSON types included.
2. ``inject`` is a pure function, so ``inject(extract(O)) == O`` for every
   ontology O that ``inject`` wrote.
3. An ontology that was never a concept rulebook (South Park itself) extracts
   to its concept rulebook: ``Things``, a table per class, a table per
   many-valued property, a column per property with one current value, and
   ``ERBTables`` and ``ERBFields`` for what the structure cannot say (labels,
   symmetry, the inverse, chain and disjoint axioms). Nothing rides in a blob:
   a class is a table with ``Thing`` and ``IsAsserted``, a property a table with
   ``Subject`` and ``Object`` or ``Value``, its domain and range the tables
   those link to. The page's own tables and its record of each fact are not
   concepts and stay in the ontology, so injecting a concept rulebook lays it
   over a BASE ontology (by default the South Park rulebook) and gives that
   ontology with its things, properties, axioms and facts brought in line.
   Unchanged, that is the base itself, row for row.
4. An edit made in the concept rulebook (a new individual, class, property or
   column, a fact added or removed, a property made symmetric or given an
   inverse) is the matching change of the ontology's rows, and nothing else
   changes. ``edit_concepts`` makes such edits, one grid action each.

A fact is switched on or off in the base by comparing the concept tables with the
base's own computed ``IsInEffect``, and a new fact takes the base's computed next
numbers (``Graphs.NextActivationSeq``, ``NextListedSortOrder``): no rule is
recomputed here, so the base must carry its computed values.

Usage (the two tools in ``effortless-tools/`` call the same functions):

    python3 orchestration/ontology_form.py inject  <concept.json>  <ontology.json> [--base <ontology.json>]
    python3 orchestration/ontology_form.py extract <ontology.json> <concept.json>
    python3 orchestration/ontology_form.py roundtrip <rulebook.json>
    python3 orchestration/ontology_form.py view <ontology.json>              (prints the concept rulebook and what each table is)
    python3 orchestration/ontology_form.py edit <ontology.json> <edits.json> (prints the ontology after the edits)
"""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "scripts"))
import rulebook_layout  # noqa: E402  the platform's one rulebook file layout

REPO_ROOT = Path(__file__).resolve().parent.parent
TEMPLATE = (
    REPO_ROOT / "rulebook-examples" / "talismans-southpark-ontology"
    / "effortless-rulebook" / "talismans-southpark-ontology-rulebook.json"
)

# The ontology model's tables, as the template names them. Portraits and
# PortraitAliases come along because Things.Portrait links to them: without them
# the model would name a table it does not have.
MODEL_TABLES = (
    "Graphs", "Rules", "FiringKinds", "Things", "Portraits", "PortraitAliases",
    "Properties", "PropertyAxioms", "Statements", "Firings", "DisjointChecks",
)
# What the candidate generator writes (app/lib/candidates.mjs). Neither
# direction writes these; the generator does.
GENERATED_TABLES = ("Firings", "DisjointChecks")

META_TABLE = "__meta__"
FORM_META_KEY = "ontology-form"

CARRIER_FIELDS = {
    "Graphs": [
        ("Description", "The concept rulebook's own Description."),
        ("Carry", "JSON: the concept rulebook's top-level key order and its top-level "
                  "keys that are not tables."),
    ],
    "Things": [
        ("Description", "A class: its concept table's Description."),
        ("Carry", "JSON: a class's table keys and their order; an individual's row key "
                  "order, keys outside the schema, and values whose JSON type differs "
                  "from the field's datatype. Blank when there is nothing to carry."),
    ],
    "Properties": [
        ("Description", "The concept field's Description."),
        ("FieldType", "The concept field's type: raw, relationship, calculated, lookup, "
                      "aggregation or closure."),
        ("Formula", "The concept field's formula, carried verbatim: a formula the "
                    "ontology's rule vocabulary does not express rides along here and is "
                    "not reasoned over."),
        ("Carry", "JSON: the concept field's keys and their order, with the keys no "
                  "column above holds."),
    ],
}

# A rulebook datatype and its XSD name. One to one, so the trip back is exact; a
# datatype outside the table travels as erb:<datatype>.
XSD = {
    "string": "xsd:string", "boolean": "xsd:boolean", "integer": "xsd:integer",
    "number": "xsd:double", "decimal": "xsd:decimal", "float": "xsd:float",
    "datetime": "xsd:dateTime", "date": "xsd:date",
}
FROM_XSD = {v: k for k, v in XSD.items()}
NUMERIC = {"integer", "number", "decimal", "float"}
STATED_TYPES = {"raw", "relationship"}
LINK_SEPARATOR = ", "


class OntologyFormError(ValueError):
    """The input cannot be mapped without losing something. Never swallowed."""


# --------------------------------------------------------------------------- helpers

def _dumps(value) -> str:
    return json.dumps(value, ensure_ascii=False, separators=(",", ":"))


def is_table(value) -> bool:
    return isinstance(value, dict) and "schema" in value


def stored_fields(table: dict) -> list:
    """A table's stored columns: the ones a build never recomputes."""
    return [f for f in table["schema"] if f.get("type") in STATED_TYPES]


def statement_id(subject: str, prop: str, obj, value) -> str:
    """The candidate generator's key, `${s}|${p}|${o ?? value}`, character for character."""
    tail = obj if obj is not None else value
    return f"{subject}|{prop}|{'null' if tail is None else tail}"


def to_xsd(datatype):
    if datatype is None:
        return None
    if not isinstance(datatype, str):
        raise OntologyFormError(f"A field's datatype must be a string, found {datatype!r}")
    return XSD.get(datatype, f"erb:{datatype}")


def from_xsd(xsd):
    if xsd is None:
        return None
    if xsd in FROM_XSD:
        return FROM_XSD[xsd]
    if xsd.startswith("erb:"):
        return xsd[4:]
    raise OntologyFormError(f"Datatype {xsd!r} was not written by this mapping")


def _value_kind(datatype) -> str:
    if datatype in NUMERIC:
        return "number"
    if datatype == "boolean":
        return "boolean"
    return "string"


def encode_literal(value, datatype):
    """A cell's value as Statements.ObjectValue text, and whether its JSON type was carried."""
    kind = _value_kind(datatype)
    if kind == "string" and isinstance(value, str):
        return value, False
    if kind == "number" and type(value) in (int, float):
        return _dumps(value), False
    if kind == "boolean" and isinstance(value, bool):
        return ("true" if value else "false"), False
    return _dumps(value), True


def decode_literal(text, datatype, carried_json: bool):
    if text is None:
        return None
    if carried_json:
        return json.loads(text)
    kind = _value_kind(datatype)
    if kind == "string":
        return text
    if kind == "number":
        return json.loads(text)
    if text == "true":
        return True
    if text == "false":
        return False
    raise OntologyFormError(f"A boolean statement reads {text!r}, not true or false")


def load_template(path: Path = TEMPLATE) -> dict:
    if not path.is_file():
        raise OntologyFormError(f"The ontology model is read from {path}, which does not exist")
    return json.loads(path.read_text(encoding="utf-8"))


def _model_schema(template: dict, name: str, with_carriers: bool) -> dict:
    table = template[name]
    schema = [dict(f) for f in table["schema"]]
    if with_carriers:
        have = {f["name"] for f in schema}
        for field, description in CARRIER_FIELDS.get(name, []):
            if field in have:
                raise OntologyFormError(f"The model's {name} already has a {field} field")
            schema.append({"name": field, "datatype": "string", "type": "raw",
                           "nullable": True, "Description": description})
    out = {}
    if "Description" in table:
        out["Description"] = table["Description"]
    out["schema"] = schema
    out["data"] = []
    return out


def _stored_row(table: dict, row: dict) -> dict:
    """A row cut to its table's stored columns, in schema order."""
    return {f["name"]: row.get(f["name"]) for f in stored_fields(table)}


def _type_property(properties: list) -> str:
    ids = [p["PropertyId"] for p in properties if p.get("PropertyKind") == "type"]
    if len(ids) != 1:
        raise OntologyFormError(
            f"The ontology must have exactly one property of kind 'type' (class "
            f"membership), found {len(ids)}")
    return ids[0]


# --------------------------------------------------------------------------- inject

def inject(rulebook: dict, template: dict | None = None, base: dict | None = None) -> dict:
    """Concept form -> ontology form.

    The concept rulebook of an ontology (``extract`` wrote it from one) is laid over
    ``base``, that ontology as it is now; without one, over the template.
    """
    template = template if template is not None else load_template()
    form = _form_marker(rulebook)
    if form is not None:
        if form != FORM_NAME:
            raise OntologyFormError(f"Unknown {FORM_META_KEY} {form!r}")
        return _inject_concepts(rulebook, base if base is not None else template)
    model = {name: _model_schema(template, name, with_carriers=True) for name in MODEL_TABLES}
    graph_template = template["Graphs"]["data"][0]
    graph_id = "rulebook"

    tables = [(name, t) for name, t in rulebook.items() if is_table(t) and name != META_TABLE]
    table_names = {name for name, _ in tables}
    top_extra = {k: v for k, v in rulebook.items()
                 if not is_table(v) and k not in ("Name", "Description")}
    graph = _stored_row(model["Graphs"], graph_template)
    graph.update({
        "GraphId": graph_id,
        "Label": rulebook.get("Name") if isinstance(rulebook.get("Name"), str) else "Rulebook",
        "Description": rulebook.get("Description"),
        "Carry": _dumps({"order": list(rulebook.keys()), "extra": top_extra}),
    })
    model["Graphs"]["data"].append(graph)
    for name in ("Rules", "FiringKinds"):
        model[name]["data"] = [_stored_row(template[name], r) for r in template[name]["data"]]

    things, properties, axioms, statements = [], [], [], []
    thing_ids: set = set()
    index: dict = {}          # (table, json of key value) -> ThingId
    sort = 0

    def new_thing_id(tid: str):
        if tid in thing_ids:
            raise OntologyFormError(f"Two things would share the id {tid!r}")
        thing_ids.add(tid)

    # Pass 1: classes, properties and the individuals' ids, so a link can point forward.
    plans = []
    for name, table in tables:
        sort += 1
        new_thing_id(name)
        schema = table["schema"]
        names = [f.get("name") for f in schema]
        class_carry = {"order": list(table.keys()),
                       "extra": {k: v for k, v in table.items()
                                 if k not in ("Description", "schema", "data")}}
        row_order = _row_order(names, table.get("data", []))
        if row_order is not None:
            class_carry["rowOrder"] = row_order
        things.append({
            "ThingId": name, "Label": name, "Kind": "class", "SortOrder": sort,
            "Portrait": None, "Description": table.get("Description"),
            "Carry": _dumps(class_carry),
        })
        if any(not isinstance(n, str) for n in names) or len(set(names)) != len(names):
            raise OntologyFormError(f"Table {name}: every field needs a distinct name, found {names}")
        rows = table.get("data", [])
        if rows and not schema:
            raise OntologyFormError(f"Table {name} has rows but no fields")
        props = []
        for field in schema:
            sort += 1
            pid = f"{name}.{field['name']}"
            relationship = field.get("type") == "relationship"
            related = field.get("RelatedTo")
            rng = related if relationship and related in table_names else None
            props.append((field, pid, rng))
            properties.append({
                "PropertyId": pid, "Label": field["name"],
                "PropertyKind": "object" if relationship else "datatype",
                "Datatype": to_xsd(field.get("datatype")),
                "IsUserAddable": field.get("type") in STATED_TYPES,
                "HasOneCurrentValue": True, "IsSymmetric": False,
                "DomainClass": name, "RangeClass": rng, "SortOrder": sort,
                "ValueNodeLabel": None, "CurrentValueWording": None, "InputName": None,
                "RecordKind": None, "RecordOrder": None,
                "Description": field.get("Description"), "FieldType": field.get("type"),
                "Formula": field.get("formula"),
                "Carry": _dumps({"order": list(field.keys()),
                                 "extra": {k: v for k, v in field.items()
                                           if k not in ("name", "datatype", "type",
                                                        "Description", "formula")}}),
            })
        key = names[0] if names else None
        ids = []
        for row in rows:
            if key not in row:
                raise OntologyFormError(f"Table {name}: a row has no {key} (its key)")
            value = row[key]
            if value is None or isinstance(value, (dict, list, bool)):
                raise OntologyFormError(f"Table {name}: key {key} is {value!r}; a key must be a "
                                        f"string or a number")
            text = value if isinstance(value, str) else _dumps(value)
            tid = f"{name}/{text}"
            new_thing_id(tid)
            lookup = (name, _dumps(value))
            if lookup in index:
                raise OntologyFormError(f"Table {name}: two rows have the key {value!r}")
            index[lookup] = tid
            ids.append(tid)
        plans.append((name, table, props, rows, ids, row_order or names))

    pid_of = {p["PropertyId"]: p for p in properties}
    for name, table, props, _, _, _ in plans:
        for field, pid, rng in props:
            inverse = field.get("InverseField")
            if rng and isinstance(inverse, str):
                other = pid_of.get(f"{rng}.{inverse}")
                if other is not None and other["PropertyKind"] == "object":
                    axioms.append({"PropertyAxiomId": f"inverse:{pid}", "Kind": "inverse",
                                   "Property": pid, "First": other["PropertyId"], "Second": None})

    type_pid = "isA"
    sort += 1
    properties.append({
        "PropertyId": type_pid, "Label": "is a", "PropertyKind": "type", "Datatype": "",
        "IsUserAddable": False, "HasOneCurrentValue": False, "IsSymmetric": False,
        "DomainClass": None, "RangeClass": None, "SortOrder": sort,
        "ValueNodeLabel": None, "CurrentValueWording": None, "InputName": None,
        "RecordKind": None, "RecordOrder": None, "Description": None, "FieldType": None,
        "Formula": None, "Carry": None,
    })

    # Pass 2: individuals and their statements.
    statement_ids: set = set()
    many_valued: set = set()

    def state(subject, prop, obj, value, stated):
        nonlocal sort
        sort += 1
        sid = statement_id(subject, prop, obj, value)
        if sid in statement_ids:
            raise OntologyFormError(f"Two statements would share the id {sid!r}")
        statement_ids.add(sid)
        statements.append({
            "StatementId": sid, "Graph": graph_id, "Subject": subject, "Property": prop,
            "ObjectThing": obj, "ObjectValue": value if obj is None else None,
            "IsListed": stated, "IsBuiltIn": True, "IsDetail": not stated, "IsStated": stated,
            "ActivationSeq": sort if stated else 0, "SortOrder": sort,
        })

    for name, table, props, rows, ids, row_order in plans:
        names = [f["name"] for f, _, _ in props]
        for row, tid in zip(rows, ids):
            sort += 1
            carry = {}
            if list(row.keys()) != _ordered(row_order, row):
                carry["order"] = list(row.keys())
            extra = {k: v for k, v in row.items() if k not in names}
            if extra:
                carry["extra"] = extra
            json_typed = []
            label = row.get("Name")
            things.append({
                "ThingId": tid, "Label": label if isinstance(label, str) and label else tid.split("/", 1)[1],
                "Kind": "individual", "SortOrder": sort, "Portrait": None,
                "Description": None, "Carry": None,
            })
            thing_row = things[-1]
            state(tid, type_pid, name, None, True)
            for field, pid, rng in props:
                if field["name"] not in row:
                    continue
                value = row[field["name"]]
                stated = field.get("type") in STATED_TYPES
                targets = _link_targets(value, rng, index) if rng else None
                if targets:
                    if len(targets) > 1:
                        many_valued.add(pid)
                    for target in targets:
                        state(tid, pid, target, None, stated)
                    continue
                if value is None:
                    state(tid, pid, None, None, stated)
                    continue
                text, carried = encode_literal(value, field.get("datatype"))
                if carried:
                    json_typed.append(field["name"])
                state(tid, pid, None, text, stated)
            if json_typed:
                carry["json"] = json_typed
            if carry:
                thing_row["Carry"] = _dumps(carry)

    for p in properties:
        if p["PropertyId"] in many_valued:
            p["HasOneCurrentValue"] = False

    model["Things"]["data"] = things
    model["Properties"]["data"] = properties
    model["PropertyAxioms"]["data"] = axioms
    model["Statements"]["data"] = statements

    out = {"$schema": template.get("$schema")}
    out["Name"] = rulebook.get("Name") if isinstance(rulebook.get("Name"), str) else "Rulebook"
    out["Description"] = (
        "This rulebook in the ontology form: every table is a class row of Things, every "
        "record an individual, every field a row of Properties and every cell a row of "
        "Statements. orchestration/ontology_form.py writes it (extract gives the concept "
        "rulebook back); the reasoning is the South Park ontology model's own formulas.")
    if is_table(rulebook.get(META_TABLE)):
        out[META_TABLE] = rulebook[META_TABLE]
    for name in MODEL_TABLES:
        out[name] = model[name]
    return out


def _ordered(sequence, row) -> list:
    return [k for k in sequence if k in row]


def _row_order(names, rows):
    """The key order a table's rows are written in, when it is not the schema's.

    Many rulebooks write every row in one order of their own; carried once on the
    class, it saves carrying it on every individual. A row that differs from it
    still carries its own.
    """
    for row in rows:
        if list(row.keys()) != _ordered(names, row):
            return list(row.keys())
    return None


def _link_targets(value, rng, index):
    """The individuals a relationship cell names, or None when it is not a clean link.

    A whole value that is a key of the related table is one link. A string that is
    several such keys joined by ', ' (a many-valued link) is one link per key, in
    order; it is read back by joining them again, so it is taken only when that gives
    the same string exactly.
    """
    whole = index.get((rng, _dumps(value)))
    if whole is not None:
        return [whole]
    if isinstance(value, str) and LINK_SEPARATOR in value:
        parts = value.split(LINK_SEPARATOR)
        if all(parts) and len(set(parts)) == len(parts):
            targets = [index.get((rng, _dumps(p))) for p in parts]
            if all(targets):
                return targets
    return None


# --------------------------------------------------------------------------- extract

def extract(ontology: dict) -> dict:
    """Ontology form -> concept form."""
    for name in ("Graphs", "Things", "Properties", "Statements"):
        if not is_table(ontology.get(name)):
            raise OntologyFormError(f"Not in the ontology form: it has no {name} table")
    graphs = ontology["Graphs"]["data"]
    if len(graphs) != 1:
        raise OntologyFormError(f"The ontology form has one graph, found {len(graphs)}")
    if graphs[0].get("Carry") is None:
        return _extract_native(ontology)
    return _extract_injected(ontology)


def _extract_injected(onto: dict) -> dict:
    graph = onto["Graphs"]["data"][0]
    top = json.loads(graph["Carry"])
    things = onto["Things"]["data"]
    properties = onto["Properties"]["data"]
    type_pid = _type_property(properties)
    by_id = {t["ThingId"]: t for t in things}
    classes = sorted((t for t in things if t["Kind"] == "class"), key=lambda t: t["SortOrder"])
    props_of: dict = {}
    for p in sorted(properties, key=lambda p: p["SortOrder"]):
        if p["PropertyId"] != type_pid:
            props_of.setdefault(p["DomainClass"], []).append(p)
    cells: dict = {}
    members: dict = {}
    for s in sorted(onto["Statements"]["data"], key=lambda s: s["SortOrder"]):
        if s["Property"] == type_pid:
            members.setdefault(s["ObjectThing"], []).append(s["Subject"])
        else:
            cells.setdefault((s["Subject"], s["Property"]), []).append(s)
    carry_of = {t["ThingId"]: (json.loads(t["Carry"]) if t.get("Carry") else {}) for t in things}
    owner = {tid: klass for klass, ms in members.items() for tid in ms}

    key_value: dict = {}

    def key_of(tid):
        """An individual's key value: what a link to it reads."""
        if tid not in key_value:
            key_value[tid] = None   # a key that links to itself reads blank, never loops
            first = props_of[owner[tid]][0]
            key_value[tid] = cell(tid, first, carry_of[tid])
        return key_value[tid]

    def cell(tid, prop, carry):
        found = cells[(tid, prop["PropertyId"])]
        if len(found) > 1:
            return LINK_SEPARATOR.join(key_of(s["ObjectThing"]) for s in found)
        s = found[0]
        if s["ObjectThing"] is not None:
            return key_of(s["ObjectThing"])
        return decode_literal(s["ObjectValue"], from_xsd(prop["Datatype"]),
                              prop["Label"] in carry.get("json", []))

    def field_of(p):
        carry = json.loads(p["Carry"])
        source = {"name": p["Label"], "datatype": from_xsd(p["Datatype"]), "type": p["FieldType"],
                  "Description": p["Description"], "formula": p["Formula"]}
        return {k: (source[k] if k in source else carry["extra"][k]) for k in carry["order"]}

    def table_of(c):
        carry = json.loads(c["Carry"])
        props = props_of.get(c["ThingId"], [])
        rows = []
        for tid in sorted(members.get(c["ThingId"], []), key=lambda t: by_id[t]["SortOrder"]):
            rc = carry_of[tid]
            prop_by_name = {p["Label"]: p for p in props}
            extra = rc.get("extra", {})
            present = {p["Label"] for p in props if (tid, p["PropertyId"]) in cells} | set(extra)
            order = rc.get("order") or [k for k in carry.get("rowOrder", list(prop_by_name))
                                        if k in present]
            rows.append({k: (cell(tid, prop_by_name[k], rc) if k in prop_by_name else extra[k])
                         for k in order})
        source = {"Description": c["Description"], "schema": [field_of(p) for p in props],
                  "data": rows}
        return {k: (source[k] if k in source else carry["extra"][k]) for k in carry["order"]}

    tables = {c["ThingId"]: c for c in classes}
    out = {}
    for k in top["order"]:
        if k == "Name":
            out[k] = graph["Label"]
        elif k == "Description":
            out[k] = graph["Description"]
        elif k == META_TABLE and is_table(onto.get(META_TABLE)):
            out[k] = onto[META_TABLE]
        elif k in tables:
            out[k] = table_of(tables[k])
        else:
            out[k] = top["extra"][k]
    return out


# ---- an ontology that was never a concept rulebook (South Park): its concept rulebook
#
# The concept rulebook is the ontology's world as ordinary tables, and nothing of the page:
#
#   Things            every individual
#   <Class>           one table per class: the individuals that belong to it
#   <property>        one table per many-valued property: one row per fact in effect
#   a column          each property that has one current value, on its domain's class
#                     table (on Things when it has no domain class)
#   ERBTables         one row per table, as the platform's Schema module keeps it, and on
#                     the row what the table's structure cannot say: a class's label and
#                     place, a property's label, wording, symmetry and its inverse, chain
#                     and disjoint axioms
#   ERBFields         one row per field, and the same attributes for a property that is a
#                     column
#   the tables Things links to (Portraits)
#
# What the structure says is not stored again: a table with Thing and IsAsserted is a
# class; a table with Subject and Object (or Value) is a property; an Object is an object
# property and a Value a datatype property of the Value's datatype; the table a Subject or
# an Object links to is the property's domain or range; a column has one current value.
#
# The page's own tables (its rules, buttons, wording, layouts) and the page's record of
# each fact (listed, built in, when it was stated) are not concepts. They stay in the
# ontology: injecting a concept rulebook lays it over a BASE ontology, and gives that
# ontology with its things, properties, axioms and facts brought in line with the concept
# tables. Unchanged concept tables give the base back, stored row for stored row.

FORM_NAME = "concepts-of-an-ontology"
ERB_TABLES, ERB_FIELDS = "ERBTables", "ERBFields"
NATIVE_TABLES = ("Things", "Properties", "PropertyAxioms", "Statements", *GENERATED_TABLES)
# What a table's structure says, so no attribute column says it again.
THING_STRUCTURE = ("ThingId", "Kind")
PROPERTY_STRUCTURE = ("PropertyId", "PropertyKind", "Datatype", "HasOneCurrentValue",
                      "DomainClass", "RangeClass")
CLASS_FIELDS = ("Thing", "IsAsserted")
FACT_FIELDS = ("Subject", "Object", "Value")
AXIOM_FIELDS = (
    ("InverseOf", "The property that reads this one backwards (OWL inverseOf): when A has "
                  "this link to B, the inverse rule concludes that B has that one to A. "
                  "Names a property: a property's table, or a column that is a property."),
    ("ChainFirst", "With ChainSecond, the property chain that concludes this property (OWL "
                   "propertyChainAxiom): a ChainFirst link followed by a ChainSecond link."),
    ("ChainSecond", "The chain's second link. Set together with ChainFirst."),
    ("DisjointWith", "Properties that never both hold between the same two things (OWL "
                     "propertyDisjointWith). Several are separated by ', '."),
)
AXIOM_NAMES = tuple(name for name, _ in AXIOM_FIELDS)
# A new property is offered in the page's Add fact form unless its row says otherwise.
ATTRIBUTE_DEFAULTS = {"IsUserAddable": True}
NAME_PATTERN = re.compile(r"^[A-Za-z][A-Za-z0-9_]*$")


def _concept_datatype(xsd) -> str:
    return FROM_XSD.get(xsd, "string") if xsd else "string"


def _typed(text, xsd):
    """A literal's text as the column's JSON value, only when the text comes back exactly."""
    kind = _value_kind(_concept_datatype(xsd))
    if text is None or kind == "string":
        return text
    try:
        value = json.loads(text)
    except ValueError:
        return text
    if kind == "number" and type(value) in (int, float) and _dumps(value) == text:
        return value
    if kind == "boolean" and isinstance(value, bool):
        return value
    return text


def _untyped(value):
    if value is None or isinstance(value, str):
        return value
    if isinstance(value, bool):
        return "true" if value else "false"
    return _dumps(value)


def _blank(value) -> bool:
    return value is None or value == ""


def _keep_blank(new, old):
    """A blank is a blank: where the ontology spells one "" it stays ""."""
    return old if _blank(new) and _blank(old) else new


def _native(onto: dict, what: str) -> dict:
    """The parts of a native ontology both directions read."""
    for name in ("Graphs", "Things", "Properties", "PropertyAxioms", "Statements"):
        if not is_table(onto.get(name)):
            raise OntologyFormError(f"{what} is not in the ontology form: it has no {name} table")
    graphs = onto["Graphs"]["data"]
    if len(graphs) != 1:
        raise OntologyFormError(f"{what} has {len(graphs)} graphs; the ontology form has one")
    thing_names = [f["name"] for f in stored_fields(onto["Things"])]
    prop_names = [f["name"] for f in stored_fields(onto["Properties"])]
    for need, have, table in ((THING_STRUCTURE, thing_names, "Things"),
                              (PROPERTY_STRUCTURE, prop_names, "Properties")):
        missing = [n for n in need if n not in have]
        if missing:
            raise OntologyFormError(f"{what}: {table} has no {', '.join(missing)} field")
    class_attrs = [f for f in stored_fields(onto["Things"]) if f["name"] not in THING_STRUCTURE]
    prop_attrs = [f for f in stored_fields(onto["Properties"]) if f["name"] not in PROPERTY_STRUCTURE]
    for f in prop_attrs:
        if f["name"] in AXIOM_NAMES:
            raise OntologyFormError(f"{what}: Properties already has a {f['name']} field")
    return {
        "graph": graphs[0],
        "type": _type_property(onto["Properties"]["data"]),
        "thing_fields": [f for f in stored_fields(onto["Things"]) if f["name"] != "Kind"],
        "class_attrs": class_attrs,
        "prop_attrs": prop_attrs,
        "support": _support_tables(onto, class_attrs + prop_attrs),
    }


def _support_tables(onto: dict, attribute_fields: list) -> list:
    """The tables a thing or a property links to (Portraits), in the ontology's order."""
    found, queue = [], [f["RelatedTo"] for f in attribute_fields if f.get("RelatedTo")]
    while queue:
        name = queue.pop(0)
        if name in found:
            continue
        if name in NATIVE_TABLES or not is_table(onto.get(name)):
            raise OntologyFormError(f"A thing or a property links to {name}, which the concept "
                                    f"form cannot hold")
        found.append(name)
        queue.extend(f["RelatedTo"] for f in stored_fields(onto[name]) if f.get("RelatedTo"))
    return [name for name in onto if name in found]


# ERBTables and ERBFields are the platform's Schema module (the generated rulebook editor's
# Modules > Schema; GET /api/erb/schema on a running editor lists it): a mirror of the
# rulebook, one row per table and one per field, that permissions and reports point at and
# the editor's Sync keeps current. The concept rulebook keeps that shape, so the module works
# on it as on any rulebook, and adds the ontology's attributes as further columns.
SCHEMA_MODULE_TABLE_FIELDS = (
    {"name": "ERBTableId", "datatype": "string", "type": "raw", "nullable": False,
     "Description": "The concept table's name: this row's key."},
    {"name": "TableName", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "The table's name again, as a plain column."},
    {"name": "Description", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "What the table holds. Follows the table."},
    {"name": "IsMetaTable", "datatype": "boolean", "type": "raw", "nullable": True,
     "Description": "True for a table that serves the rulebook itself (ERBTables, ERBFields) "
                    "and not the ontology."},
    {"name": "IsTenantScoped", "datatype": "boolean", "type": "raw", "nullable": True,
     "Description": "The Schema module's: rows belong to one tenant. Not used by the ontology."},
    {"name": "TenantParentTable", "datatype": "string", "type": "relationship", "nullable": True,
     "Description": "The Schema module's: the table tenancy is reached through. Not used by "
                    "the ontology.", "RelatedTo": "ERBTables"},
    {"name": "TenantParentField", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "The Schema module's: the field that points at TenantParentTable. Not "
                    "used by the ontology."},
    {"name": "SortOrder", "datatype": "integer", "type": "raw", "nullable": True,
     "Description": "Display order."},
)
SCHEMA_MODULE_FIELD_FIELDS = (
    {"name": "ERBFieldId", "datatype": "string", "type": "raw", "nullable": False,
     "Description": "Table.Field: this row's key."},
    {"name": "ERBTable", "datatype": "string", "type": "relationship", "nullable": True,
     "Description": "The table the field is on.", "RelatedTo": "ERBTables"},
    {"name": "FieldName", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "The field's name. For a column that is a property: the property's id."},
    {"name": "FieldType", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "raw or relationship. Follows the field."},
    {"name": "Datatype", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "The rulebook datatype. Follows the field."},
    {"name": "IsPrimaryKey", "datatype": "boolean", "type": "raw", "nullable": True,
     "Description": "True for the field that identifies a row of its table."},
    {"name": "Description", "datatype": "string", "type": "raw", "nullable": True,
     "Description": "What the field means. Follows the field."},
)
# What follows the table or field a row is about: read from the structure, never from the row.
ERB_MIRRORED = ("ERBTableId", "TableName", "ERBFieldId", "ERBTable", "FieldName", "FieldType",
                "Datatype", "IsPrimaryKey", "IsMetaTable", "Description")


def _erb_schemas(native: dict) -> tuple:
    """ERBTables and ERBFields: the Schema module's fields, then the model's own attribute
    fields (a class's, a property's), then the axioms."""
    def attribute(f, note):
        field = {"name": f["name"], "datatype": f.get("datatype", "string"), "type": f["type"],
                 "nullable": True, "Description": note}
        if f.get("RelatedTo"):
            field["RelatedTo"] = f["RelatedTo"]
        return field

    def merged(module_fields, owners):
        out = [dict(f) for f in module_fields]
        where = {f["name"]: i for i, f in enumerate(out)}
        for owner, fields in owners:
            for f in fields:
                note = f"For a {owner}: {f.get('Description') or f['name']}"
                if f["name"] in where:
                    first = out[where[f["name"]]]
                    if (first["datatype"], first["type"], first.get("RelatedTo")) != \
                            (f.get("datatype", "string"), f["type"], f.get("RelatedTo")):
                        raise OntologyFormError(
                            f"The model's {f['name']} ({owner}) differs in type from the "
                            f"{f['name']} the attribute tables already have")
                    first["Description"] = f"{first['Description']} {note}"
                    continue
                where[f["name"]] = len(out)
                out.append(attribute(f, note))
        return out + [{"name": name, "datatype": "string", "type": "raw", "nullable": True,
                       "Description": description} for name, description in AXIOM_FIELDS]

    return (merged(SCHEMA_MODULE_TABLE_FIELDS, (("class", native["class_attrs"]), ("property", native["prop_attrs"]))),
            merged(SCHEMA_MODULE_FIELD_FIELDS, (("property", native["prop_attrs"]),)))


def _attribute_names(native: dict) -> tuple:
    """The columns the ontology reads from an ERBTables and from an ERBFields row."""
    props = [f["name"] for f in native["prop_attrs"]]
    return ([f["name"] for f in native["class_attrs"]] + [n for n in props if n not in
            {f["name"] for f in native["class_attrs"]}] + list(AXIOM_NAMES), props + list(AXIOM_NAMES))


def _form_marker(rulebook: dict):
    """The form a rulebook says it is in (its __meta__ row), or None."""
    meta = rulebook.get(META_TABLE)
    if not is_table(meta):
        return None
    for row in meta.get("data", []):
        if row.get("MetaKey") == FORM_META_KEY:
            if row.get("ValueType") != "string":
                raise OntologyFormError(
                    f"This concept rulebook was written by an earlier version of the mapping "
                    f"(its {FORM_META_KEY} row is not a string): extract it from the ontology again")
            return row.get("StringValue")
    return None


def _class_table(cid: str, label: str) -> dict:
    """A class's table: its key, the individual, and whether a fact states the membership."""
    return {
        "Description": f"The class {label}: one row per individual that belongs to it. "
                       f"Its label and place are its {ERB_TABLES} row.",
        "schema": [
            {"name": f"{cid}Id", "datatype": "string", "type": "raw", "nullable": False,
             "Description": "The individual (its ThingId)."},
            {"name": "Thing", "datatype": "string", "type": "relationship", "nullable": False,
             "Description": "The individual's row in Things.", "RelatedTo": "Things"},
            {"name": "IsAsserted", "datatype": "boolean", "type": "raw", "nullable": False,
             "Description": f"True when a fact states it is a {label}. False when only a "
                            f"property's domain or range implies it: such a row follows from "
                            f"the facts and is not added by hand."},
        ],
        "data": [],
    }


def _fact_table(pid: str, label: str, subject: str, obj, datatype) -> dict:
    """A many-valued property's table: one row per fact. obj is the table an Object links
    to (an object property); datatype is the Value's (a datatype property)."""
    return {
        "Description": f"{label}: one row per fact in effect. A property of the ontology; its "
                       f"attributes are its {ERB_TABLES} row.",
        "schema": [
            {"name": f"{pid}Id", "datatype": "string", "type": "raw", "nullable": False,
             "Description": "The fact: subject|property|object."},
            {"name": "Subject", "datatype": "string", "type": "relationship", "nullable": False,
             "Description": "Who the fact is about. The table it links to is the property's "
                            "domain: a class's table, or Things for any thing.",
             "RelatedTo": subject},
            ({"name": "Object", "datatype": "string", "type": "relationship", "nullable": False,
              "Description": "What it links to. The table it links to is the property's range: "
                             "a class's table, or Things for any thing.",
              "RelatedTo": obj}
             if obj is not None else
             {"name": "Value", "datatype": datatype, "type": "raw", "nullable": False,
              "Description": "The value."}),
        ],
        "data": [],
    }


def _column_field(pid: str, label: str, obj, datatype) -> dict:
    """A property with one current value, as a column of its domain's table."""
    field = {"name": pid, "datatype": "string" if obj is not None else datatype,
             "type": "relationship" if obj is not None else "raw", "nullable": True,
             "Description": f"{label}: the one current value. A property of the ontology; its "
                            f"other attributes are its {ERB_FIELDS} row."}
    if obj is not None:
        field["RelatedTo"] = obj
    return field


def _extract_native(onto: dict) -> dict:
    native = _native(onto, "The rulebook")
    if not is_table(onto.get(META_TABLE)):
        raise OntologyFormError(f"The ontology has no {META_TABLE} table; the concept rulebook "
                                f"says there which form it is in")
    things_t = onto["Things"]
    properties = onto["Properties"]["data"]
    type_pid = native["type"]
    statements = onto["Statements"]["data"]
    if any("IsInEffect" not in s for s in statements):
        raise OntologyFormError(
            "Statements rows carry no IsInEffect value: build the ontology rulebook first "
            "(its computed values say which facts are in effect)")
    things = [_stored_row(things_t, t) for t in things_t["data"]]
    classes = sorted((t for t in things if t["Kind"] == "class"), key=lambda t: t["SortOrder"])
    individuals = sorted((t for t in things if t["Kind"] == "individual"), key=lambda t: t["SortOrder"])
    class_ids = [c["ThingId"] for c in classes]
    prop = {p["PropertyId"]: _stored_row(onto["Properties"], p) for p in properties}
    facts = sorted((s for s in statements if s["IsInEffect"]), key=lambda s: s["SortOrder"])

    # Class membership: asserted (an isA fact) or implied by a fact's domain and range.
    asserted, member = {}, {c: set() for c in class_ids}
    for s in facts:
        if s["Property"] == type_pid:
            if s["ObjectThing"] not in member:
                raise OntologyFormError(f"{s['StatementId']} names a class that is not a class")
            member[s["ObjectThing"]].add(s["Subject"])
            asserted[(s["Subject"], s["ObjectThing"])] = True
            continue
        p = prop[s["Property"]]
        if p.get("DomainClass") in member:
            member[p["DomainClass"]].add(s["Subject"])
        if p.get("RangeClass") in member and s["ObjectThing"] is not None:
            member[p["RangeClass"]].add(s["ObjectThing"])

    ordered_props = sorted((prop[p["PropertyId"]] for p in properties if p["PropertyId"] != type_pid),
                           key=lambda p: p["SortOrder"])
    columns = {}          # table -> [property] held as one column
    link_tables = []      # properties held as a table of their own
    for p in ordered_props:
        if p["PropertyKind"] not in ("object", "datatype"):
            raise OntologyFormError(f"Property {p['PropertyId']} is of kind {p['PropertyKind']!r}; "
                                    f"the concept form holds object and datatype properties")
        if p["PropertyKind"] == "datatype" and to_xsd(_concept_datatype(p.get("Datatype"))) != p.get("Datatype"):
            raise OntologyFormError(f"Property {p['PropertyId']} has the datatype {p.get('Datatype')!r}, "
                                    f"which no rulebook datatype is")
        if p["PropertyKind"] == "object" and not _blank(p.get("Datatype")):
            raise OntologyFormError(f"Property {p['PropertyId']} links two things and has the "
                                    f"datatype {p['Datatype']!r}; a column cannot say both")
        if not NAME_PATTERN.match(p["PropertyId"]):
            raise OntologyFormError(f"Property id {p['PropertyId']!r} cannot name a table or a column")
        if p.get("HasOneCurrentValue"):
            home = p["DomainClass"] if p.get("DomainClass") in member else "Things"
            columns.setdefault(home, []).append(p)
        else:
            link_tables.append(p)

    support = native["support"]
    names_taken = set()
    for name in [META_TABLE, "Things", *class_ids, *(p["PropertyId"] for p in link_tables),
                 *support, ERB_TABLES, ERB_FIELDS]:
        if name in names_taken:
            raise OntologyFormError(f"Two concept tables would be named {name!r}")
        names_taken.add(name)

    def ref_table(klass):
        return klass if klass in member else "Things"

    def column_field(p):
        obj = p["PropertyKind"] == "object"
        return _column_field(p["PropertyId"], p["Label"], ref_table(p.get("RangeClass")) if obj else None,
                             None if obj else _concept_datatype(p.get("Datatype")))

    def fact_value(s, p):
        return s["ObjectThing"] if p["PropertyKind"] == "object" else _typed(s["ObjectValue"], p.get("Datatype"))

    def with_columns(table, fixed):
        taken = {f["name"] for f in fixed}
        for p in columns.get(table, []):
            if p["PropertyId"] in taken:
                raise OntologyFormError(f"Property {p['PropertyId']} would be a second "
                                        f"{p['PropertyId']} column of {table}")
            taken.add(p["PropertyId"])
        return fixed + [column_field(p) for p in columns.get(table, [])]

    one = {}
    for s in facts:
        p = prop[s["Property"]]
        if s["Property"] != type_pid and p.get("HasOneCurrentValue"):
            key = (s["Subject"], s["Property"])
            if key in one:
                raise OntologyFormError(f"{key} has two facts in effect but one current value")
            one[key] = s

    tables = {}
    thing_fields = [dict(f) for f in native["thing_fields"]]
    thing_rows = []
    for t in individuals:
        row = {f["name"]: t[f["name"]] for f in thing_fields}
        for p in columns.get("Things", []):
            s = one.get((t["ThingId"], p["PropertyId"]))
            row[p["PropertyId"]] = fact_value(s, p) if s else None
        thing_rows.append(row)
    tables["Things"] = {
        "Description": "Every individual of the ontology (owl:Thing). A class's table lists "
                       "the individuals that belong to it.",
        "schema": with_columns("Things", thing_fields),
        "data": thing_rows,
    }
    for c in classes:
        cid = c["ThingId"]
        if not NAME_PATTERN.match(cid):
            raise OntologyFormError(f"Class id {cid!r} cannot name a table")
        table = _class_table(cid, c["Label"])
        key = table["schema"][0]["name"]
        for t in individuals:
            if t["ThingId"] not in member[cid]:
                continue
            row = {key: t["ThingId"], "Thing": t["ThingId"],
                   "IsAsserted": asserted.get((t["ThingId"], cid), False)}
            for p in columns.get(cid, []):
                s = one.get((t["ThingId"], p["PropertyId"]))
                row[p["PropertyId"]] = fact_value(s, p) if s else None
            table["data"].append(row)
        table["schema"] = with_columns(cid, table["schema"])
        tables[cid] = table
    for p in link_tables:
        pid = p["PropertyId"]
        obj = p["PropertyKind"] == "object"
        table = _fact_table(pid, p["Label"], ref_table(p.get("DomainClass")),
                            ref_table(p.get("RangeClass")) if obj else None,
                            None if obj else _concept_datatype(p.get("Datatype")))
        table["data"] = [{f"{pid}Id": s["StatementId"], "Subject": s["Subject"],
                          ("Object" if obj else "Value"): fact_value(s, p)}
                         for s in facts if s["Property"] == pid]
        tables[pid] = table
    for name in support:
        kept = [f for f in onto[name]["schema"]
                if f.get("type") in STATED_TYPES or "!" not in (f.get("formula") or "")]
        names = [f["name"] for f in kept]
        tables[name] = {**{k: v for k, v in onto[name].items() if k not in ("schema", "data")},
                        "schema": [dict(f) for f in kept],
                        "data": [{k: row[k] for k in names if k in row} for row in onto[name]["data"]]}

    # What the structure cannot say: ERBTables and ERBFields.
    axiom_of = {p["PropertyId"]: dict.fromkeys(AXIOM_NAMES) for p in ordered_props}
    for a in onto["PropertyAxioms"]["data"]:
        kind, pid, first, second = a["Kind"], a["Property"], a["First"], a.get("Second")
        for named in (pid, first, second):
            if named is not None and named not in axiom_of:
                raise OntologyFormError(f"Axiom {a['PropertyAxiomId']} names {named}, which is "
                                        f"not a property the concept form holds")
        mine = axiom_of[pid]
        if kind == "inverse" and second is None and mine["InverseOf"] is None:
            expected, mine["InverseOf"] = f"inverse:{pid}", first
        elif kind == "chain" and second is not None and mine["ChainFirst"] is None:
            expected = f"chain:{pid}"
            mine["ChainFirst"], mine["ChainSecond"] = first, second
        elif kind == "disjoint" and second is None:
            expected = f"disjoint:{pid}~{first}"
            mine["DisjointWith"] = [*(mine["DisjointWith"] or []), first]
        else:
            raise OntologyFormError(
                f"Axiom {a['PropertyAxiomId']} ({kind}) has no column in the concept form: a "
                f"property has one inverse, one two-link chain and its disjoint properties")
        if a["PropertyAxiomId"] != expected:
            raise OntologyFormError(f"Axiom {a['PropertyAxiomId']} would come back as {expected}")

    def property_attributes(p):
        out = {f["name"]: p.get(f["name"]) for f in native["prop_attrs"]}
        mine = axiom_of[p["PropertyId"]]
        out.update({**mine, "DisjointWith": LINK_SEPARATOR.join(mine["DisjointWith"])
                    if mine["DisjointWith"] else None})
        return out

    # The mirror: a row per table and a row per field, with the attributes on the rows of
    # the classes, the properties and the columns that are properties.
    tables_schema, fields_schema = _erb_schemas(native)
    tables[ERB_TABLES] = {"Description": "", "schema": tables_schema, "data": []}
    tables[ERB_FIELDS] = {"Description": "", "schema": fields_schema, "data": []}
    class_row = {c["ThingId"]: {f["name"]: c.get(f["name"]) for f in native["class_attrs"]} for c in classes}
    table_property = {p["PropertyId"]: property_attributes(p) for p in link_tables}
    column_property = {(table, p["PropertyId"]): property_attributes(p)
                       for table, held in columns.items() for p in held}
    blank_table = {f["name"]: None for f in tables_schema}
    blank_field = {f["name"]: None for f in fields_schema}
    erb_tables, erb_fields = [], []
    for name, table in tables.items():
        erb_tables.append({**blank_table, "ERBTableId": name, "TableName": name,
                           "Description": None, "IsMetaTable": name in (ERB_TABLES, ERB_FIELDS),
                           **class_row.get(name, {}), **table_property.get(name, {})})
        for at, f in enumerate(table["schema"]):
            erb_fields.append({**blank_field, "ERBFieldId": f"{name}.{f['name']}", "ERBTable": name,
                               "FieldName": f["name"], "FieldType": f.get("type"),
                               "Datatype": f.get("datatype"), "IsPrimaryKey": at == 0,
                               "Description": f.get("Description"),
                               **column_property.get((name, f["name"]), {})})
    tables[ERB_TABLES].update({
        "Description": "One row per table of this rulebook (the platform's Schema module), and on "
                       "it what the table's structure does not say. A class: its label and its "
                       "place in the cast. A property that is a table: its label, its wording "
                       "on the page, whether it is symmetric, and its inverse, chain and "
                       "disjoint axioms. What a table IS is read from its fields: Thing and "
                       "IsAsserted make a class; Subject with Object or Value make a property, "
                       "whose domain and range are the tables they link to.",
        "data": erb_tables})
    tables[ERB_FIELDS].update({
        "Description": "One row per field of every table (the platform's Schema module), and on "
                       "it the same attributes for a property that is a column: a property with "
                       "one current value, on its domain's class table (on Things when it has "
                       "no domain class).",
        "data": erb_fields})
    for row in erb_tables:
        row["Description"] = tables[row["ERBTableId"]]["Description"]

    meta = json.loads(_dumps(onto[META_TABLE]))
    meta["data"] = [r for r in meta["data"] if r.get("MetaKey") != FORM_META_KEY]
    meta["data"].append({"MetaKey": FORM_META_KEY, "ValueType": "string",
                         "StringValue": FORM_NAME, "JsonValue": None, "Name": FORM_META_KEY})
    title = onto.get("Name") if isinstance(onto.get("Name"), str) else "The ontology"
    out = {}
    if "$schema" in onto:
        out["$schema"] = onto["$schema"]
    out["Name"] = f"{title}: the concept form"
    out["Description"] = (
        "This ontology in the concept form: a table of individuals, a table per class, a table "
        "or a column per property, and ERBTables and ERBFields for what the structure cannot "
        "say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings "
        "the ontology's things, properties, axioms and facts in line with these tables.")
    out[META_TABLE] = meta
    out.update(tables)
    return out


# ---- reading a concept rulebook of an ontology: what each table and column is

def _read_concepts(concepts: dict, native: dict) -> dict:
    if not is_table(concepts.get("Things")):
        raise OntologyFormError("The concept rulebook has no Things table")
    for name in (ERB_TABLES, ERB_FIELDS):
        if not is_table(concepts.get(name)):
            raise OntologyFormError(f"The concept rulebook has no {name} table: it holds what "
                                    f"the tables' structure cannot say")
    type_pid = native["type"]
    roles, classes, properties = {}, {}, {}
    tables = [(name, t) for name, t in concepts.items() if is_table(t)]

    def fields_of(name, table):
        names = [f.get("name") for f in table["schema"]]
        if any(not isinstance(n, str) for n in names) or len(set(names)) != len(names):
            raise OntologyFormError(f"Table {name}: every field needs a distinct name")
        if not names:
            raise OntologyFormError(f"Table {name} has no fields")
        return {f["name"]: f for f in table["schema"]}

    # Pass 1: what each table is.
    for name, table in tables:
        fields = fields_of(name, table)
        if name == META_TABLE:
            roles[name] = "meta"
        elif name == "Things":
            roles[name] = "individuals"
        elif name in (ERB_TABLES, ERB_FIELDS):
            roles[name] = "attributes"
        else:
            is_class = all(f in fields for f in CLASS_FIELDS)
            is_fact = "Subject" in fields and (("Object" in fields) != ("Value" in fields))
            if is_class and is_fact:
                raise OntologyFormError(f"Table {name} has a class's fields and a property's")
            if is_class:
                roles[name] = "class"
                classes[name] = {"key": table["schema"][0]["name"], "columns": []}
            elif is_fact:
                roles[name] = "relationship" if "Object" in fields else "values"
            elif name in native["support"]:
                roles[name] = "support"
            else:
                raise OntologyFormError(
                    f"Table {name} is neither a class nor a property. A class's table has the "
                    f"fields Thing (linking to Things) and IsAsserted; a property's table has "
                    f"Subject and one of Object and Value")
    for name in native["support"]:
        if roles.get(name) != "support":
            raise OntologyFormError(f"The concept rulebook has no {name} table, which Things links to")

    def target(table, field):
        related = field.get("RelatedTo")
        if field.get("type") != "relationship" or (related != "Things" and related not in classes):
            raise OntologyFormError(f"{table}.{field['name']} must link to Things or to a class's "
                                    f"table, not {related!r}")
        return None if related == "Things" else related

    def datatype(table, field):
        if field.get("type") != "raw":
            raise OntologyFormError(
                f"{table}.{field['name']} is a {field.get('type')} field. The ontology states "
                f"facts and concludes by its rules; it has no place for a formula")
        if field.get("datatype") not in XSD:
            raise OntologyFormError(f"{table}.{field['name']} has the datatype "
                                    f"{field.get('datatype')!r}; use one of {', '.join(XSD)}")
        return XSD[field["datatype"]]

    def add(pid, record):
        if not NAME_PATTERN.match(pid):
            raise OntologyFormError(f"{pid!r} cannot be a property's id: letters, digits and _ only")
        if pid == type_pid or pid in properties:
            raise OntologyFormError(f"Two properties would be named {pid}: a property is one "
                                    f"table or one column")
        properties[pid] = {"id": pid, **record}

    def column(table, field, domain):
        if field.get("type") == "relationship":
            add(field["name"], {"kind": "object", "xsd": None, "single": True, "domain": domain,
                                "range": target(table, field), "table": table, "field": field["name"]})
        else:
            add(field["name"], {"kind": "datatype", "xsd": datatype(table, field), "single": True,
                                "domain": domain, "range": None, "table": table, "field": field["name"]})

    # Pass 2: the properties, in the document's order.
    thing_names = {f["name"] for f in native["thing_fields"]}
    for name, table in tables:
        fields = fields_of(name, table)
        role = roles[name]
        if role == "individuals":
            missing = [n for n in thing_names if n not in fields]
            if missing:
                raise OntologyFormError(f"Things has no {', '.join(missing)} field")
            for f in table["schema"]:
                if f["name"] not in thing_names:
                    column(name, f, None)
        elif role == "class":
            if name == type_pid:
                raise OntologyFormError(f"A class cannot be named {name}")
            thing = fields["Thing"]
            if thing.get("type") != "relationship" or thing.get("RelatedTo") != "Things":
                raise OntologyFormError(f"{name}.Thing must link to Things")
            if table["schema"][0]["name"] in CLASS_FIELDS:
                raise OntologyFormError(f"{name}: its first field is its key, before Thing and IsAsserted")
            for f in table["schema"][1:]:
                if f["name"] not in CLASS_FIELDS:
                    column(name, f, name)
                    classes[name]["columns"].append(f["name"])
        elif role in ("relationship", "values"):
            key = table["schema"][0]["name"]
            extra = [n for n in fields if n != key and n not in FACT_FIELDS]
            if key in FACT_FIELDS or extra:
                raise OntologyFormError(
                    f"Table {name} holds facts: its key, Subject and Object (or Value)"
                    + (f"; it has no place for {', '.join(extra)}" if extra else ""))
            domain = target(name, fields["Subject"])
            if role == "relationship":
                add(name, {"kind": "object", "xsd": None, "single": False, "domain": domain,
                           "range": target(name, fields["Object"]), "table": name, "field": None})
            else:
                add(name, {"kind": "datatype", "xsd": datatype(name, fields["Value"]), "single": False,
                           "domain": domain, "range": None, "table": name, "field": None})

    # The attribute rows. ERBTables and ERBFields mirror every table and field; the ontology
    # reads the rows of its classes, its properties and its property columns, and of those
    # only its own columns. A row about a table or a field that is not there says nothing.
    tables_schema, fields_schema = _erb_schemas(native)
    table_columns, field_columns = _attribute_names(native)
    for name, schema, needed in ((ERB_TABLES, tables_schema, table_columns),
                                 (ERB_FIELDS, fields_schema, field_columns)):
        have = [f["name"] for f in concepts[name]["schema"]]
        known = [f["name"] for f in schema]
        unknown = [n for n in have if n not in known]
        missing = [n for n in [known[0], *needed] if n not in have]
        if unknown or missing:
            raise OntologyFormError(
                f"{name} holds the Schema module's fields and the attributes the ontology model has"
                + (f"; the model has no {', '.join(unknown)}" if unknown else "")
                + (f"; {name} lacks {', '.join(missing)}" if missing else ""))
    table_rows, field_rows = {}, {}
    for row in concepts[ERB_TABLES]["data"]:
        tid = row.get("ERBTableId")
        if tid in table_rows:
            raise OntologyFormError(f"{ERB_TABLES} has two rows for {tid}")
        table_rows[tid] = row
    seen_fields = set()
    for row in concepts[ERB_FIELDS]["data"]:
        fid = row.get("ERBFieldId")
        if fid in seen_fields:
            raise OntologyFormError(f"{ERB_FIELDS} has two rows for {fid}")
        seen_fields.add(fid)
        table, _, field = str(fid).partition(".")
        p = properties.get(field)
        if p is not None and p["field"] is not None and p["table"] == table:
            field_rows[field] = row
    for pid, p in properties.items():
        p["attributes"] = field_rows.get(pid) if p["field"] else table_rows.get(pid)
    for cid, c in classes.items():
        c["attributes"] = table_rows.get(cid)
    return {"roles": roles, "classes": classes, "properties": properties}


def _inject_concepts(concepts: dict, base: dict) -> dict:
    """The concept rulebook of an ontology, laid over that ontology."""
    native = _native(base, "The base ontology")
    reading = _read_concepts(concepts, native)
    classes, properties, type_pid = reading["classes"], reading["properties"], native["type"]
    graph = native["graph"]
    numbers = {}
    for name in ("NextActivationSeq", "NextListedSortOrder"):
        if type(graph.get(name)) is not int:
            raise OntologyFormError(
                f"The base ontology's Graphs row carries no {name}: build the ontology rulebook "
                f"first (its computed values give the next numbers)")
        numbers[name] = graph[name]
    base_statements = base["Statements"]["data"]
    if any("IsInEffect" not in s for s in base_statements):
        raise OntologyFormError(
            "The base ontology's Statements rows carry no IsInEffect value: build the ontology "
            "rulebook first (its computed values say which facts are in effect)")

    def take(name):
        numbers[name] += 1
        return numbers[name] - 1

    # ---- Things: the individuals of the Things table, and a class per class table.
    things_t = base["Things"]
    base_things = {t["ThingId"]: _stored_row(things_t, t) for t in things_t["data"]}
    thing_names = [f["name"] for f in native["thing_fields"]]
    found = {}
    for row in concepts["Things"]["data"]:
        tid = row.get("ThingId")
        if not isinstance(tid, str) or not tid:
            raise OntologyFormError("Things has a row with no ThingId")
        if tid in found or tid in classes:
            raise OntologyFormError(f"Two things would share the id {tid!r}")
        found[tid] = {"Kind": "individual", **{k: row.get(k) for k in thing_names}}
    individuals = list(found)
    for cid, c in classes.items():
        attributes = c["attributes"] or {}
        found[cid] = {"ThingId": cid, "Kind": "class",
                      **{f["name"]: attributes.get(f["name"]) for f in native["class_attrs"]}}
    next_sort = max([t["SortOrder"] for t in [*base_things.values(), *found.values()]
                     if type(t.get("SortOrder")) is int] + [0])
    things = []
    for tid in [*(t for t in base_things if t in found), *(t for t in found if t not in base_things)]:
        thing, old = found[tid], base_things.get(tid, {})
        if _blank(thing.get("Label")):
            thing["Label"] = tid
        if thing.get("SortOrder") is None:
            next_sort += 1
            thing["SortOrder"] = next_sort
        things.append({f["name"]: _keep_blank(thing.get(f["name"]), old.get(f["name"]))
                       for f in stored_fields(things_t)})
    thing_ids = set(found)

    # ---- Properties: the type property as the base has it, and one per table or column.
    props_t = base["Properties"]
    base_props = {p["PropertyId"]: _stored_row(props_t, p) for p in props_t["data"]}
    next_sort = max([p["SortOrder"] for p in base_props.values() if type(p.get("SortOrder")) is int]
                    + [(p["attributes"] or {}).get("SortOrder") or 0 for p in properties.values()])
    built = {type_pid: base_props[type_pid]}
    for pid, p in properties.items():
        attributes, old = p["attributes"] or {}, base_props.get(pid, {})
        row = {"PropertyId": pid, "PropertyKind": p["kind"], "Datatype": p["xsd"],
               "HasOneCurrentValue": p["single"], "DomainClass": p["domain"], "RangeClass": p["range"]}
        for f in native["prop_attrs"]:
            value = attributes.get(f["name"])
            if value is None and f.get("nullable") is False:
                if f["name"] in ATTRIBUTE_DEFAULTS:
                    value = ATTRIBUTE_DEFAULTS[f["name"]]
                elif f.get("datatype") == "boolean":
                    value = False
                elif f["name"] == "SortOrder":
                    next_sort += 1
                    value = next_sort
                elif f.get("datatype", "string") == "string":
                    value = pid
                else:
                    raise OntologyFormError(f"Property {pid} needs a {f['name']}")
            row[f["name"]] = value
        built[pid] = {f["name"]: _keep_blank(row[f["name"]], old.get(f["name"]))
                      for f in stored_fields(props_t)}
    property_rows = [built[pid] for pid in [*(p for p in base_props if p in built),
                                            *(p for p in built if p not in base_props)]]

    # ---- PropertyAxioms: from each property's InverseOf, ChainFirst, ChainSecond, DisjointWith.
    kinds = {k["FiringKindId"] for k in base["FiringKinds"]["data"]} if is_table(base.get("FiringKinds")) else set()
    axioms = {}

    def axiom(aid, kind, pid, first, second):
        for named in (first, second):
            if named is not None and named not in properties:
                raise OntologyFormError(f"{pid}: its {kind} axiom names {named!r}, which is not a "
                                        f"property (a property's table, or a column that is one)")
        if kind not in kinds:
            raise OntologyFormError(f"The ontology model has no {kind} rule (FiringKinds)")
        axioms[aid] = {"PropertyAxiomId": aid, "Kind": kind, "Property": pid, "First": first,
                       "Second": second}

    for row in property_rows:
        pid = row["PropertyId"]
        if pid == type_pid:
            continue
        attributes = properties[pid]["attributes"] or {}
        inverse, first, second = (None if _blank(attributes.get(n)) else attributes[n]
                                  for n in ("InverseOf", "ChainFirst", "ChainSecond"))
        if inverse is not None:
            axiom(f"inverse:{pid}", "inverse", pid, inverse, None)
        if (first is None) != (second is None):
            raise OntologyFormError(f"{pid}: a chain has a ChainFirst and a ChainSecond; one is blank")
        if first is not None:
            axiom(f"chain:{pid}", "chain", pid, first, second)
        disjoint = attributes.get("DisjointWith")
        if not _blank(disjoint):
            others = [part.strip() for part in str(disjoint).split(",")]
            if not all(others) or len(set(others)) != len(others):
                raise OntologyFormError(f"{pid}: DisjointWith reads {disjoint!r}; name each "
                                        f"property once, separated by ', '")
            for other in others:
                axiom(f"disjoint:{pid}~{other}", "disjoint", pid, other, None)
    base_axioms = [a["PropertyAxiomId"] for a in base["PropertyAxioms"]["data"]]
    axiom_rows = [axioms[a] for a in [*(a for a in base_axioms if a in axioms),
                                      *(a for a in axioms if a not in base_axioms)]]

    # ---- The facts the concept tables state.
    world = {}

    def state(where, subject, pid, obj, value):
        if subject not in thing_ids:
            raise OntologyFormError(f"{where} is about {subject!r}, which is not in Things")
        if obj is not None and obj not in thing_ids:
            raise OntologyFormError(f"{where} links to {obj!r}, which is not in Things")
        sid = statement_id(subject, pid, obj, value)
        if sid in world:
            raise OntologyFormError(f"{where}: the fact {sid} is stated twice")
        world[sid] = (subject, pid, obj, value)

    def cells(table, key, row, held):
        for pid in held:
            value = row.get(pid)
            if _blank(value):
                continue
            where = f"{table} row {row[key]!r}, {pid}"
            if properties[pid]["kind"] == "object":
                state(where, row[key], pid, value, None)
            else:
                state(where, row[key], pid, None, _untyped(value))

    for cid, c in classes.items():
        for row in concepts[cid]["data"]:
            thing, key = row.get("Thing"), row.get(c["key"])
            if _blank(thing):
                thing = key
            if _blank(thing) or (not _blank(key) and key != thing):
                raise OntologyFormError(f"{cid}: a row's {c['key']} and its Thing name the same "
                                        f"individual, found {key!r} and {row.get('Thing')!r}")
            if thing not in thing_ids:
                raise OntologyFormError(f"{cid} row {thing!r} is not in Things")
            row = {**row, c["key"]: thing}
            if row.get("IsAsserted") is True:
                state(f"{cid} row {thing!r}", thing, type_pid, cid, None)
            elif row.get("IsAsserted") not in (False, None):
                raise OntologyFormError(f"{cid} row {thing!r}: IsAsserted is true or false")
            cells(cid, c["key"], row, c["columns"])
    thing_columns = [pid for pid, p in properties.items() if p["table"] == "Things" and p["field"]]
    for row in concepts["Things"]["data"]:
        cells("Things", "ThingId", row, thing_columns)
    for pid, p in properties.items():
        if p["field"]:
            continue
        for row in concepts[pid]["data"]:
            subject = row.get("Subject")
            where = f"{pid} row {row.get(concepts[pid]['schema'][0]['name'])!r}"
            if _blank(subject):
                raise OntologyFormError(f"{where} has no Subject")
            if p["kind"] == "object":
                if _blank(row.get("Object")):
                    raise OntologyFormError(f"{where} has no Object")
                state(where, subject, pid, row["Object"], None)
            else:
                if _blank(row.get("Value")):
                    raise OntologyFormError(f"{where} has no Value")
                state(where, subject, pid, None, _untyped(row["Value"]))

    # ---- Statements: the base's, switched on or off where the concept tables differ from
    # what is in effect, then the facts the base has no row for. The page's own record of
    # each fact (listed, built in, a detail) is the base's and is kept.
    statement_t = base["Statements"]
    live = {}
    for s in base_statements:
        dead = (s["Subject"] not in thing_ids or s["Property"] not in built
                or (s["ObjectThing"] is not None and s["ObjectThing"] not in thing_ids))
        if not dead:
            live[s["StatementId"]] = s
    changed, added = {}, []
    for sid, (subject, pid, obj, value) in world.items():
        s = live.get(sid)
        if s is not None and s["IsInEffect"]:
            continue
        row = _stored_row(statement_t, s) if s is not None else {
            "StatementId": sid, "Graph": graph["GraphId"], "Subject": subject, "Property": pid,
            "ObjectThing": obj, "ObjectValue": value, "IsListed": False, "IsBuiltIn": False,
            "IsDetail": False, "IsStated": False, "ActivationSeq": 0, "SortOrder": 0}
        if not row["IsListed"]:
            row["IsListed"], row["SortOrder"] = True, take("NextListedSortOrder")
        row["IsStated"], row["ActivationSeq"] = True, take("NextActivationSeq")
        if s is not None:
            changed[sid] = row
        else:
            added.append({f["name"]: row[f["name"]] for f in stored_fields(statement_t)})
    statement_rows = []
    for sid, s in live.items():
        if sid in changed:
            statement_rows.append(changed[sid])
        elif s["IsInEffect"] and sid not in world:
            statement_rows.append({**_stored_row(statement_t, s), "IsStated": False})
        else:
            statement_rows.append(_stored_row(statement_t, s))
    statement_rows.extend(added)

    # A table whose stored rows did not change is the base's own, computed values and all;
    # a changed one carries stored rows only (its computed values are the next build's).
    def unless_same(table, rows):
        return table if rows == [_stored_row(table, r) for r in table["data"]] else {**table, "data": rows}

    rebuilt = {"Things": things, "Properties": property_rows, "PropertyAxioms": axiom_rows,
               "Statements": statement_rows}
    out = {}
    for name, value in base.items():
        if name in rebuilt:
            out[name] = unless_same(value, rebuilt[name])
        elif name == META_TABLE and is_table(value):
            out[name] = unless_same(value, [_stored_row(value, r) for r in concepts[META_TABLE]["data"]
                                            if r.get("MetaKey") != FORM_META_KEY])
        elif name in native["support"]:
            out[name] = unless_same(value, [_stored_row(value, r) for r in concepts[name]["data"]])
        else:
            out[name] = value
    return out


# ---- what a concept rulebook of an ontology is, for whoever shows or edits it

def describe(concepts: dict, base: dict) -> dict:
    """Each table's role and each property's reading, as the mapping takes them."""
    native = _native(base, "The base ontology")
    reading = _read_concepts(concepts, native)
    properties = [
        {"id": pid, "home": "column" if p["field"] else "table", "table": p["table"],
         "kind": p["kind"], "datatype": _concept_datatype(p["xsd"]) if p["kind"] == "datatype" else None,
         "single": p["single"], "domain": p["domain"], "range": p["range"],
         "hasAttributes": p["attributes"] is not None}
        for pid, p in reading["properties"].items()]
    tables = []
    for name, table in concepts.items():
        if not is_table(table):
            continue
        tables.append({
            "name": name, "role": reading["roles"][name], "key": table["schema"][0]["name"],
            "columns": [p["id"] for p in properties if p["home"] == "column" and p["table"] == name],
        })
    return {"form": FORM_NAME, "typeProperty": native["type"], "tables": tables,
            "properties": properties, "classes": list(reading["classes"]),
            "datatypes": list(XSD), "axioms": list(AXIOM_NAMES),
            # Columns of ERBTables and ERBFields that are not edited there: they follow the
            # table or field the row is about, or are the Schema module's and not the ontology's.
            "follows": [*ERB_MIRRORED, *ERB_NOT_ONTOLOGY]}


# ---- editing a concept rulebook of an ontology
#
# One edit is one thing a person does in a grid: change a cell, add or remove a row, add a
# class, a property or a column, point a link at another table. The shapes of a class's and
# a property's table are this module's, so the tables an edit adds are made here, and every
# edit leaves a document the mapping reads.

def _coerce(field: dict, value, where: str):
    if value is None or value == "":
        return None
    datatype = field.get("datatype", "string")
    if datatype == "boolean":
        if isinstance(value, bool):
            return value
        if value in ("true", "false"):
            return value == "true"
        raise OntologyFormError(f"{where} is true or false, not {value!r}")
    if datatype in NUMERIC:
        if isinstance(value, bool):
            raise OntologyFormError(f"{where} is a number, not {value!r}")
        try:
            number = value if type(value) in (int, float) else json.loads(str(value))
        except ValueError:
            number = None
        if type(number) not in (int, float) or (datatype == "integer" and number != int(number)):
            raise OntologyFormError(
                f"{where} is a {'whole number' if datatype == 'integer' else 'number'}, not {value!r}")
        return int(number) if datatype == "integer" else number
    return value if isinstance(value, str) else _untyped(value)


def _edit_table(doc: dict, name) -> dict:
    if not isinstance(name, str) or not is_table(doc.get(name)):
        raise OntologyFormError(f"The concept rulebook has no table {name!r}")
    return doc[name]


def _row_index(table: dict, name: str, key) -> int:
    key_field = table["schema"][0]["name"]
    for i, row in enumerate(table["data"]):
        if str(row.get(key_field)) == str(key):
            return i
    raise OntologyFormError(f"{name} has no row {key!r}")


def _clean_values(name: str, table: dict, values) -> dict:
    if not isinstance(values, dict):
        raise OntologyFormError(f"{name}: an edit gives its values as an object")
    fields = {f["name"]: f for f in table["schema"]}
    out = {}
    for field_name, value in values.items():
        field = fields.get(field_name)
        if field is None:
            raise OntologyFormError(f"{name} has no field {field_name}")
        if field.get("type") not in STATED_TYPES:
            raise OntologyFormError(f"{name}.{field_name} is computed ({field.get('type')}); it "
                                    f"cannot be written")
        out[field_name] = _coerce(field, value, f"{name}.{field_name}")
    return out


def _settle_row(doc: dict, reading: dict, name: str, row: dict) -> None:
    """A row's derived parts: a membership's key is its Thing, a fact's key its statement id."""
    role, key = reading["roles"][name], doc[name]["schema"][0]["name"]
    if role == "class":
        if _blank(row.get("Thing")):
            row["Thing"] = row.get(key)
        row[key] = row["Thing"]
        if row.get("IsAsserted") is None:
            row["IsAsserted"] = True
    elif role == "relationship":
        row[key] = statement_id(row.get("Subject"), name, row.get("Object"), None)
    elif role == "values":
        row[key] = statement_id(row.get("Subject"), name, None, _untyped(row.get("Value")))
    if _blank(row.get(key)):
        raise OntologyFormError(f"{name}: a row needs a value in its key field {key}")


def _one_of_a_kind(name: str, table: dict, row: dict, at) -> None:
    key = table["schema"][0]["name"]
    for i, other in enumerate(table["data"]):
        if i != at and str(other.get(key)) == str(row[key]):
            raise OntologyFormError(f"{name} already has a row {row[key]!r}")


def _is_marker(name: str, row: dict) -> bool:
    return name == META_TABLE and row.get("MetaKey") == FORM_META_KEY


# The Schema module's own columns the ontology has no place for: an edit there would be lost.
ERB_NOT_ONTOLOGY = ("IsTenantScoped", "TenantParentTable", "TenantParentField")


def _op_patch(doc, reading, native, op):
    name = op.get("table")
    table = _edit_table(doc, name)
    role = reading["roles"][name]
    at = _row_index(table, name, op.get("key"))
    old = table["data"][at]
    if _is_marker(name, old):
        raise OntologyFormError(f"The {FORM_META_KEY} row says which form this rulebook is in; it stays")
    values = _clean_values(name, table, op.get("values"))
    key = table["schema"][0]["name"]
    if role in ("class", "relationship", "values"):
        values.pop(key, None)                 # derived from the row's other values
    elif key in values and str(values[key]) != str(old.get(key)):
        raise OntologyFormError(f"{name}: a row's key ({key}) cannot be changed; remove the row "
                                f"and add another")
    if role == "attributes":
        for fixed in ERB_MIRRORED:
            if fixed in values and values[fixed] != old.get(fixed):
                raise OntologyFormError(f"{name}.{fixed} follows the table or field the row is "
                                        f"about; change that, not this copy of it")
        for foreign in ERB_NOT_ONTOLOGY:
            if foreign in values and values[foreign] != old.get(foreign):
                raise OntologyFormError(f"{name}.{foreign} is the Schema module's; the ontology "
                                        f"has no place for it, so an edit here would be lost")
    row = {**old, **values}
    _settle_row(doc, reading, name, row)
    _one_of_a_kind(name, table, row, at)
    table["data"][at] = row


def _op_insert(doc, reading, native, op):
    name = op.get("table")
    table = _edit_table(doc, name)
    role = reading["roles"][name]
    values = _clean_values(name, table, op.get("values") or {})
    row = {f["name"]: values.get(f["name"]) for f in table["schema"] if f.get("type") in STATED_TYPES}
    if _is_marker(name, row):
        raise OntologyFormError(f"There is one {FORM_META_KEY} row")
    if name == ERB_TABLES:
        about = row.get("ERBTableId") or row.get("TableName")
        if about not in reading["roles"] or about == META_TABLE:
            raise OntologyFormError(f"{ERB_TABLES} holds a row per table; {about!r} is not one")
        row["ERBTableId"] = row["TableName"] = about
    elif name == ERB_FIELDS:
        table, field = row.get("ERBTable"), row.get("FieldName")
        if not is_table(doc.get(table)) or not any(f["name"] == field for f in doc[table]["schema"]):
            raise OntologyFormError(f"{ERB_FIELDS} holds a row per field; {table}.{field} is not one")
        row["ERBFieldId"] = f"{table}.{field}"
    _settle_row(doc, reading, name, row)
    _one_of_a_kind(name, table, row, None)
    table["data"].append(row)


def _links_to(doc: dict, target: str):
    """Every (table, relationship field) that links to the given table."""
    for name, table in doc.items():
        if is_table(table):
            for f in table["schema"]:
                if f.get("type") == "relationship" and f.get("RelatedTo") == target:
                    yield name, table, f["name"]


def _op_remove(doc, reading, native, op):
    name = op.get("table")
    table = _edit_table(doc, name)
    role = reading["roles"][name]
    at = _row_index(table, name, op.get("key"))
    row = table["data"][at]
    key = row[table["schema"][0]["name"]]
    if role == "attributes":
        raise OntologyFormError(f"A {name} row goes with its table or column: remove that instead")
    if _is_marker(name, row):
        raise OntologyFormError(f"The {FORM_META_KEY} row says which form this rulebook is in; it stays")
    if role == "individuals":
        # The thing goes, and with it what was said of it: its memberships, the facts about
        # it or linking to it, and any one-value link that named it.
        for other, c in reading["classes"].items():
            doc[other]["data"] = [r for r in doc[other]["data"] if r.get("Thing") != key]
        for pid, p in reading["properties"].items():
            if p["field"] is None:
                doc[pid]["data"] = [r for r in doc[pid]["data"]
                                    if r.get("Subject") != key and r.get("Object") != key]
            elif p["kind"] == "object":
                for r in doc[p["table"]]["data"]:
                    if r.get(pid) == key:
                        r[pid] = None
    elif role == "support":
        for other, linking, field in _links_to(doc, name):
            if any(r.get(field) == key for r in linking["data"]):
                raise OntologyFormError(f"{name} row {key!r} is still linked from {other}.{field}")
    table["data"] = [r for i, r in enumerate(table["data"]) if i != at] if role != "individuals" \
        else [r for r in table["data"] if r.get("ThingId") != key]


def _new_name(doc, reading, native, name, what) -> str:
    if not isinstance(name, str) or not NAME_PATTERN.match(name):
        raise OntologyFormError(f"A {what}'s name is letters, digits and _ and starts with a "
                                f"letter, not {name!r}")
    if name in doc:
        raise OntologyFormError(f"There already is a table {name}")
    if name == native["type"] or name in reading["properties"]:
        raise OntologyFormError(f"There already is a property {name}")
    return name


def _table_or_things(reading, name, what) -> str:
    name = name or "Things"
    if name != "Things" and name not in reading["classes"]:
        raise OntologyFormError(f"{what} links to Things or to a class's table, not {name!r}")
    return name


def _put_table(doc: dict, name: str, table: dict, reading: dict, after_roles: tuple) -> None:
    """Add a table after the last table of the given roles (before the attribute tables)."""
    last = None
    for existing in doc:
        if reading["roles"].get(existing) in after_roles:
            last = existing
    items = list(doc.items())
    doc.clear()
    for key, value in items:
        doc[key] = value
        if key == last:
            doc[name] = table
    if name not in doc:
        doc[name] = table


def _attribute_row(doc: dict, table: str, identity: dict, label) -> None:
    row = {f["name"]: None for f in doc[table]["schema"]}
    row.update(identity)
    if "Label" in row and not _blank(label):
        row["Label"] = str(label)
    doc[table]["data"].append(row)


def _op_add_class(doc, reading, native, op):
    name = _new_name(doc, reading, native, op.get("name"), "class")
    if any(r.get("ThingId") == name for r in doc["Things"]["data"]):
        raise OntologyFormError(f"{name} is already an individual in Things")
    label = op.get("label") or name
    _put_table(doc, name, _class_table(name, label), reading, ("individuals", "class"))
    _attribute_row(doc, ERB_TABLES, {"ERBTableId": name, "TableName": name}, label)


def _op_add_property(doc, reading, native, op):
    """A many-valued property: a table of facts. With `datatype` its facts hold values; else
    they link to `object` (a class's table, or Things)."""
    name = _new_name(doc, reading, native, op.get("name"), "property")
    label = op.get("label") or name
    subject = _table_or_things(reading, op.get("subject"), "A fact's Subject")
    if op.get("datatype"):
        if op["datatype"] not in XSD:
            raise OntologyFormError(f"A value's datatype is one of {', '.join(XSD)}, not {op['datatype']!r}")
        table = _fact_table(name, label, subject, None, op["datatype"])
    else:
        table = _fact_table(name, label, subject, _table_or_things(reading, op.get("object"), "A fact's Object"), None)
    _put_table(doc, name, table, reading, ("individuals", "class", "relationship", "values"))
    _attribute_row(doc, ERB_TABLES, {"ERBTableId": name, "TableName": name}, label)


def _op_add_column(doc, reading, native, op):
    """A property with one current value: a column of Things or of a class's table. With
    `datatype` it holds a value; else it links to `object`."""
    home = op.get("table")
    table = _edit_table(doc, home)
    if reading["roles"][home] not in ("individuals", "class"):
        raise OntologyFormError(f"A column that is a property goes on Things or on a class's "
                                f"table, not on {home}")
    name = op.get("name")
    if not isinstance(name, str) or not NAME_PATTERN.match(name):
        raise OntologyFormError(f"A column's name is letters, digits and _ and starts with a "
                                f"letter, not {name!r}")
    if any(f["name"] == name for f in table["schema"]):
        raise OntologyFormError(f"{home} already has a field {name}")
    if name == native["type"] or name in reading["properties"]:
        raise OntologyFormError(f"There already is a property {name}")
    label = op.get("label") or name
    if op.get("datatype"):
        if op["datatype"] not in XSD:
            raise OntologyFormError(f"A value's datatype is one of {', '.join(XSD)}, not {op['datatype']!r}")
        field = _column_field(name, label, None, op["datatype"])
    else:
        field = _column_field(name, label, _table_or_things(reading, op.get("object"), "A column's link"), None)
    table["schema"].append(field)
    for row in table["data"]:
        row[name] = None
    _attribute_row(doc, ERB_FIELDS, {"ERBFieldId": f"{home}.{name}", "ERBTable": home, "FieldName": name}, label)


def _forget_property(doc: dict, pid: str) -> None:
    """No other property's axiom names a property that is gone."""
    for table in (ERB_TABLES, ERB_FIELDS):
        for row in doc[table]["data"]:
            if row.get("InverseOf") == pid:
                row["InverseOf"] = None
            if pid in (row.get("ChainFirst"), row.get("ChainSecond")):
                row["ChainFirst"] = row["ChainSecond"] = None
            if not _blank(row.get("DisjointWith")):
                others = [part.strip() for part in str(row["DisjointWith"]).split(",")]
                kept = [o for o in others if o != pid]
                if kept != others:
                    row["DisjointWith"] = LINK_SEPARATOR.join(kept) if kept else None


def _drop_column(doc: dict, home: str, pid: str) -> None:
    table = doc[home]
    table["schema"] = [f for f in table["schema"] if f["name"] != pid]
    for row in table["data"]:
        row.pop(pid, None)
    doc[ERB_FIELDS]["data"] = [r for r in doc[ERB_FIELDS]["data"]
                               if not (r.get("ERBTable") == home and r.get("FieldName") == pid)]
    _forget_property(doc, pid)


def _op_remove_column(doc, reading, native, op):
    home, name = op.get("table"), op.get("name")
    p = reading["properties"].get(name)
    if p is None or p["field"] is None or p["table"] != home:
        raise OntologyFormError(f"{home}.{name} is not a column that is a property")
    _drop_column(doc, home, name)


def _op_remove_table(doc, reading, native, op):
    name = op.get("table")
    _edit_table(doc, name)
    role = reading["roles"][name]
    if role == "class":
        for pid in list(reading["classes"][name]["columns"]):
            _drop_column(doc, name, pid)
        for _, linking, field in list(_links_to(doc, name)):   # a link to the class links to any thing
            for f in linking["schema"]:
                if f["name"] == field:
                    f["RelatedTo"] = "Things"
    elif role in ("relationship", "values"):
        _forget_property(doc, name)
    else:
        raise OntologyFormError(f"{name} is not a class or a property: it cannot be removed here")
    del doc[name]
    doc[ERB_TABLES]["data"] = [r for r in doc[ERB_TABLES]["data"] if r.get("ERBTableId") != name]


def _op_retarget(doc, reading, native, op):
    """Point a Subject, an Object or a link column at another table: the property's domain
    or range becomes that class, or any thing for Things."""
    name, field_name = op.get("table"), op.get("field")
    table = _edit_table(doc, name)
    role = reading["roles"][name]
    p = reading["properties"].get(field_name)
    is_fact_end = role in ("relationship", "values") and field_name in ("Subject", "Object")
    is_link_column = p is not None and p["field"] is not None and p["table"] == name and p["kind"] == "object"
    field = next((f for f in table["schema"] if f["name"] == field_name), None)
    if field is None or field.get("type") != "relationship" or not (is_fact_end or is_link_column):
        raise OntologyFormError(f"{name}.{field_name} is not a property's Subject, Object or link column")
    field["RelatedTo"] = _table_or_things(reading, op.get("relatedTo"), f"{name}.{field_name}")


EDITS = {
    "patch": _op_patch, "insert": _op_insert, "remove": _op_remove,
    "addClass": _op_add_class, "addProperty": _op_add_property, "addColumn": _op_add_column,
    "removeColumn": _op_remove_column, "removeTable": _op_remove_table, "retarget": _op_retarget,
}


def edit_concepts(concepts: dict, edits: list, base: dict) -> dict:
    """The concept rulebook after the given edits, in order. `base` is its ontology."""
    if _form_marker(concepts) != FORM_NAME:
        raise OntologyFormError("Not the concept rulebook of an ontology")
    if not isinstance(edits, list):
        raise OntologyFormError("The edits are a list")
    native = _native(base, "The base ontology")
    doc = json.loads(_dumps(concepts))
    for op in edits:
        if not isinstance(op, dict) or op.get("op") not in EDITS:
            raise OntologyFormError(f"Unknown edit {op!r}; there are {', '.join(EDITS)}")
        EDITS[op["op"]](doc, _read_concepts(doc, native), native, op)
    _read_concepts(doc, native)
    return doc


# --------------------------------------------------------------------------- files

def read_rulebook(path: Path) -> dict:
    return json.loads(Path(path).read_text(encoding="utf-8"))


def write_rulebook(document: dict, path: Path) -> None:
    """A rulebook file in the platform's layout (scripts/rulebook_layout.py): one field or row per line."""
    rulebook_layout.write_rulebook(document, path)


def canonical(document: dict) -> str:
    return json.dumps(document, indent=2, ensure_ascii=False)


def view(ontology: dict) -> dict:
    """An ontology's concept rulebook, and what each of its tables is."""
    concepts = extract(ontology)
    return {"concepts": concepts, "description": describe(concepts, ontology)}


def main(argv: list) -> int:
    try:
        return _main(argv)
    except OntologyFormError as error:
        print(str(error), file=sys.stderr)
        return 1


def _main(argv: list) -> int:
    base = None
    if "--base" in argv:
        at = argv.index("--base")
        if at + 1 >= len(argv):
            raise OntologyFormError("--base needs the ontology rulebook to lay the concepts over")
        base = read_rulebook(Path(argv[at + 1]))
        argv = argv[:at] + argv[at + 2:]
    if len(argv) == 3 and argv[0] in ("inject", "extract"):
        source = read_rulebook(Path(argv[1]))
        result = inject(source, base=base) if argv[0] == "inject" else extract(source)
        write_rulebook(result, Path(argv[2]))
        return 0
    if len(argv) == 2 and argv[0] == "view":
        sys.stdout.write(_dumps(view(read_rulebook(Path(argv[1])))))
        return 0
    if len(argv) == 3 and argv[0] == "edit":
        ontology = read_rulebook(Path(argv[1]))
        edited = edit_concepts(extract(ontology), read_rulebook(Path(argv[2])), ontology)
        sys.stdout.write(_dumps({"ontology": inject(edited, base=ontology)}))
        return 0
    if len(argv) == 2 and argv[0] == "roundtrip":
        source = read_rulebook(Path(argv[1]))
        back = extract(json.loads(canonical(inject(source))))
        if canonical(back) != canonical(source):
            print(f"{argv[1]}: the round trip changed the rulebook", file=sys.stderr)
            return 1
        print(f"{argv[1]}: identical after concept -> ontology -> concept")
        return 0
    print(__doc__.split("Usage", 1)[1], file=sys.stderr)
    return 2


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
