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
   to concept tables (``Things``, one table per class, one link table per
   many-valued property), and injecting that gives back every stored row of
   her model: the things, the properties and their wiring, the axioms, every
   fact that is stated, listed or a literal value, and every table the page
   reads. What it leaves out is what the candidate generator writes from the
   cast (``app/lib/candidates.mjs``: the unstated links between things,
   ``Firings``, ``DisjointChecks``); the generator run on the result names
   exactly the rows her file holds.

Usage (the two tools in ``effortless-tools/`` call the same functions):

    python3 orchestration/ontology_form.py inject  <concept.json>  <ontology.json>
    python3 orchestration/ontology_form.py extract <ontology.json> <concept.json>
    python3 orchestration/ontology_form.py roundtrip <rulebook.json>
"""

from __future__ import annotations

import json
import sys
from pathlib import Path

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


def _form_meta(rulebook: dict):
    meta = rulebook.get(META_TABLE)
    if not is_table(meta):
        return None
    for row in meta.get("data", []):
        if row.get("MetaKey") == FORM_META_KEY:
            return json.loads(row["JsonValue"])
    return None


# --------------------------------------------------------------------------- inject

def inject(rulebook: dict, template: dict | None = None) -> dict:
    """Concept form -> ontology form."""
    template = template if template is not None else load_template()
    if _form_meta(rulebook) is not None:
        return _inject_extracted_ontology(rulebook, template)
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


# ---- an ontology that was never a concept rulebook (South Park)

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


def _extract_native(onto: dict) -> dict:
    things_t = onto["Things"]
    properties = onto["Properties"]["data"]
    type_pid = _type_property(properties)
    axioms = onto.get("PropertyAxioms", {}).get("data", [])
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
    statement_t = onto["Statements"]

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

    ordered_props = sorted((p for p in properties if p["PropertyId"] != type_pid),
                           key=lambda p: p["SortOrder"])
    columns = {}          # table -> [property] held as one column
    link_tables = []      # properties held as a table of their own
    for p in ordered_props:
        if p.get("HasOneCurrentValue"):
            home = p["DomainClass"] if p.get("DomainClass") in member else "Things"
            columns.setdefault(home, []).append(p)
        else:
            link_tables.append(p)

    names_taken = set()
    passthrough = [k for k, v in onto.items() if is_table(v) and k not in
                   (META_TABLE, "Things", "Properties", "PropertyAxioms", "Statements",
                    *GENERATED_TABLES)]
    for name in ["Things", *class_ids, *(p["PropertyId"] for p in link_tables), *passthrough]:
        if name in names_taken:
            raise OntologyFormError(f"Two concept tables would be named {name!r}")
        names_taken.add(name)

    def ref_table(klass):
        return klass if klass in member else "Things"

    def column_field(p):
        field = {"name": p["PropertyId"], "datatype": _concept_datatype(p.get("Datatype")),
                 "type": "relationship" if p["PropertyKind"] == "object" else "raw",
                 "nullable": True, "Description": f"{p['Label']} (property {p['PropertyId']})."}
        if p["PropertyKind"] == "object":
            field["RelatedTo"] = ref_table(p.get("RangeClass"))
        return field

    def fact_value(s, p):
        return s["ObjectThing"] if p["PropertyKind"] == "object" else _typed(s["ObjectValue"], p.get("Datatype"))

    one = {}
    for s in facts:
        p = prop[s["Property"]]
        if s["Property"] != type_pid and p.get("HasOneCurrentValue"):
            key = (s["Subject"], s["Property"])
            if key in one:
                raise OntologyFormError(f"{key} has two facts in effect but one current value")
            one[key] = s

    out = {}
    for k in ("$schema", "Name", "Description"):
        if k in onto:
            out[k] = onto[k]

    # Things: every individual, with the stored columns of her Things table.
    thing_fields = [dict(f) for f in stored_fields(things_t) if f["name"] != "Kind"]
    thing_rows = []
    for t in individuals:
        row = {f["name"]: t[f["name"]] for f in thing_fields}
        for p in columns.get("Things", []):
            s = one.get((t["ThingId"], p["PropertyId"]))
            row[p["PropertyId"]] = fact_value(s, p) if s else None
        thing_rows.append(row)
    out["Things"] = {
        "Description": "Every individual of the ontology (owl:Thing). A class's table lists "
                       "the individuals that belong to it.",
        "schema": thing_fields + [column_field(p) for p in columns.get("Things", [])],
        "data": thing_rows,
    }
    for c in classes:
        cid = c["ThingId"]
        key = f"{cid}Id"
        fields = [
            {"name": key, "datatype": "string", "type": "raw", "nullable": False,
             "Description": f"The individual (its ThingId)."},
            {"name": "Thing", "datatype": "string", "type": "relationship", "nullable": False,
             "Description": "The individual's row in Things.", "RelatedTo": "Things"},
            {"name": "IsAsserted", "datatype": "boolean", "type": "raw", "nullable": False,
             "Description": f"True when a fact states it is a {c['Label']}; false when only a "
                            f"property's domain or range implies it."},
        ] + [column_field(p) for p in columns.get(cid, [])]
        rows = []
        for t in individuals:
            if t["ThingId"] not in member[cid]:
                continue
            row = {key: t["ThingId"], "Thing": t["ThingId"],
                   "IsAsserted": asserted.get((t["ThingId"], cid), False)}
            for p in columns.get(cid, []):
                s = one.get((t["ThingId"], p["PropertyId"]))
                row[p["PropertyId"]] = fact_value(s, p) if s else None
            rows.append(row)
        out[cid] = {"Description": f"The class {c['Label']}.", "schema": fields, "data": rows}
    for p in link_tables:
        pid = p["PropertyId"]
        obj = p["PropertyKind"] == "object"
        fields = [
            {"name": f"{pid}Id", "datatype": "string", "type": "raw", "nullable": False,
             "Description": "The fact (its StatementId)."},
            {"name": "Subject", "datatype": "string", "type": "relationship", "nullable": False,
             "Description": "Who the fact is about.", "RelatedTo": ref_table(p.get("DomainClass"))},
            ({"name": "Object", "datatype": "string", "type": "relationship", "nullable": False,
              "Description": "What it links to.", "RelatedTo": ref_table(p.get("RangeClass"))}
             if obj else
             {"name": "Value", "datatype": _concept_datatype(p.get("Datatype")), "type": "raw",
              "nullable": False, "Description": "The value."}),
        ]
        rows = [{f"{pid}Id": s["StatementId"], "Subject": s["Subject"],
                 ("Object" if obj else "Value"): fact_value(s, p)}
                for s in facts if s["Property"] == pid]
        out[pid] = {"Description": f"{p['Label']}: one row per fact in effect.",
                    "schema": fields, "data": rows}
    for name in passthrough:
        out[name] = onto[name]

    # What the concept tables cannot say, in one __meta__ row: each property's wiring,
    # the axioms, each class's own row, and every fact that is not the generator's to
    # write: the listed and the stated ones with their page state, and every literal
    # value (a candidate such as a grade a button moves to is data; only links between
    # things are generated from the cast).
    page_state = [_stored_row(statement_t, s) for s in statements
                  if s["IsInEffect"] or s.get("IsListed") or s.get("IsStated")
                  or s.get("ObjectThing") is None]
    form = {
        "form": "concepts-of-an-ontology",
        "order": list(onto.keys()),
        "typeProperty": prop[type_pid],
        "classes": classes,
        "properties": [prop[p["PropertyId"]] for p in ordered_props],
        "columns": {table: [p["PropertyId"] for p in ps] for table, ps in columns.items()},
        "axioms": [_stored_row(onto["PropertyAxioms"], a) for a in axioms],
        "statements": page_state,
        "schemas": {name: onto[name] | {"data": []}
                    for name in ("Things", "Properties", "PropertyAxioms", "Statements",
                                 *GENERATED_TABLES) if is_table(onto.get(name))},
        "metaTableAdded": not is_table(onto.get(META_TABLE)),
    }
    meta = json.loads(_dumps(onto[META_TABLE])) if is_table(onto.get(META_TABLE)) else \
        _model_schema(load_template(), META_TABLE, with_carriers=False)
    meta["data"].append({"MetaKey": FORM_META_KEY, "ValueType": "object", "StringValue": None,
                         "JsonValue": _dumps(form), "Name": FORM_META_KEY})
    return {**{k: out[k] for k in ("$schema", "Name", "Description") if k in out},
            META_TABLE: meta, **{k: v for k, v in out.items()
                                 if k not in ("$schema", "Name", "Description")}}


def _inject_extracted_ontology(concepts: dict, template: dict) -> dict:
    """The concept tables of an ontology -> that ontology, every stored row of it."""
    form = _form_meta(concepts)
    if form.get("form") != "concepts-of-an-ontology":
        raise OntologyFormError(f"Unknown {FORM_META_KEY} form {form.get('form')!r}")
    type_pid = form["typeProperty"]["PropertyId"]
    classes = form["classes"]
    class_ids = [c["ThingId"] for c in classes]
    props = {p["PropertyId"]: p for p in form["properties"]}
    schemas = form["schemas"]
    page_state = {s["StatementId"]: s for s in form["statements"]}

    # Things: the individuals from the Things table, then her own row order.
    things_schema = schemas["Things"]
    things = []
    for row in concepts["Things"]["data"]:
        thing = {"Kind": "individual"}
        thing.update({k: v for k, v in row.items() if k not in form["columns"].get("Things", [])})
        things.append({f["name"]: thing.get(f["name"]) for f in stored_fields(things_schema)})
    things.extend(classes)
    things.sort(key=lambda t: t["SortOrder"])

    graph_id = next(iter(page_state.values()))["Graph"] if page_state else \
        concepts["Graphs"]["data"][0]["GraphId"]
    next_sort = max([s["SortOrder"] for s in page_state.values()] + [0])

    statements = []

    def state(subject, pid, obj, value):
        nonlocal next_sort
        sid = statement_id(subject, pid, obj, value)
        if sid in page_state:
            statements.append(page_state[sid])
            return
        next_sort += 1
        statements.append({"StatementId": sid, "Graph": graph_id, "Subject": subject,
                           "Property": pid, "ObjectThing": obj, "ObjectValue": value,
                           "IsListed": True, "IsBuiltIn": False, "IsDetail": False,
                           "IsStated": True, "ActivationSeq": next_sort, "SortOrder": next_sort})

    for cid in class_ids:
        for row in concepts[cid]["data"]:
            if row["IsAsserted"]:
                state(row[f"{cid}Id"], type_pid, cid, None)
    for table, pids in form["columns"].items():
        key = "ThingId" if table == "Things" else f"{table}Id"
        for row in concepts[table]["data"]:
            for pid in pids:
                value = row.get(pid)
                if value is None:
                    continue
                if props[pid]["PropertyKind"] == "object":
                    state(row[key], pid, value, None)
                else:
                    state(row[key], pid, None, _untyped(value))
    for pid, p in props.items():
        if pid in concepts and not any(pid in v for v in form["columns"].values()):
            for row in concepts[pid]["data"]:
                if p["PropertyKind"] == "object":
                    state(row["Subject"], pid, row["Object"], None)
                else:
                    state(row["Subject"], pid, None, _untyped(row["Value"]))
    # Her facts in the order her file holds them (listed facts that are switched off and
    # stated facts a later one replaced included), then any fact added in the concept form.
    added = [s for s in statements if s["StatementId"] not in page_state]
    statements = [*page_state.values(), *added]

    rebuilt = {
        "Things": things,
        "Properties": [*form["properties"], form["typeProperty"]],
        "PropertyAxioms": form["axioms"],
        "Statements": statements,
    }
    rebuilt["Properties"].sort(key=lambda p: p["SortOrder"])
    out = {}
    for k in form["order"]:
        if k == META_TABLE:
            if not form["metaTableAdded"]:
                meta = json.loads(_dumps(concepts[META_TABLE]))
                meta["data"] = [r for r in meta["data"] if r.get("MetaKey") != FORM_META_KEY]
                out[k] = meta
        elif k in rebuilt:
            out[k] = schemas[k] | {"data": rebuilt[k]}
        elif k in GENERATED_TABLES:
            out[k] = schemas[k] | {"data": []}
        else:
            out[k] = concepts[k]
    return out


# --------------------------------------------------------------------------- files

def read_rulebook(path: Path) -> dict:
    return json.loads(Path(path).read_text(encoding="utf-8"))


def write_rulebook(document: dict, path: Path) -> None:
    Path(path).write_text(json.dumps(document, indent=2, ensure_ascii=False) + "\n",
                          encoding="utf-8")


def canonical(document: dict) -> str:
    return json.dumps(document, indent=2, ensure_ascii=False)


def main(argv: list) -> int:
    if len(argv) >= 2 and argv[0] in ("inject", "extract") and len(argv) == 3:
        source = read_rulebook(Path(argv[1]))
        result = inject(source) if argv[0] == "inject" else extract(source)
        write_rulebook(result, Path(argv[2]))
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
