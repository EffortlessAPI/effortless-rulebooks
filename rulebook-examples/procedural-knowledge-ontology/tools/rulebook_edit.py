"""Shared helpers for loop scripts that edit the rulebook.

Every loop script re-reads the rulebook immediately before writing, inserts only its
own keys, and writes with indent=1 / ensure_ascii=False (the file's on-disk format;
see the project CLAUDE.md, "Concurrent writes to the rulebook").

Helpers are idempotent: adding a field that exists replaces nothing and raises if the
existing definition differs, so a re-run never silently rewrites someone's formula.
"""
from __future__ import annotations

import json
from collections import OrderedDict
from pathlib import Path

import os

# The project rulebook. PKO_RULEBOOK points a probe (tools/loops/probe_specs.py) at a scratch copy.
RB = Path(os.environ.get("PKO_RULEBOOK") or
          Path(__file__).resolve().parent.parent / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json")
EXT = "urn:effortless:pko-extension#"


def load() -> OrderedDict:
    with RB.open() as fh:
        return json.load(fh, object_pairs_hook=OrderedDict)


def dump(rb: OrderedDict) -> None:
    text = json.dumps(rb, indent=1, ensure_ascii=False) + "\n"
    RB.write_text(text)


def _field(name, datatype, ftype, desc, **extra):
    f = OrderedDict([("name", name), ("datatype", datatype), ("type", ftype),
                     ("nullable", extra.pop("nullable", True)), ("Description", desc)])
    f.update(extra)
    return f


def raw(name, datatype, desc, nullable=True):
    return _field(name, datatype, "raw", desc, nullable=nullable)


def calc(name, datatype, desc, formula):
    assert "IIF(" not in formula and "ISBLANK(" not in formula, f"{name}: forbidden function"
    return _field(name, datatype, "calculated", desc, formula=formula)


def agg(name, datatype, desc, formula):
    assert "IIF(" not in formula and "ISBLANK(" not in formula, f"{name}: forbidden function"
    return _field(name, datatype, "aggregation", desc, formula=formula)


def lookup(name, datatype, desc, formula):
    return _field(name, datatype, "lookup", desc, formula=formula)


def rel(name, target, desc, nullable=True):
    return _field(name, "string", "relationship", desc, nullable=nullable, RelatedTo=target)


def idx(target, field, fk, target_pk=None):
    """INDEX/MATCH lookup on the target's primary key (the only shape the transpiler supports)."""
    target_pk = target_pk or f"{target[:-1] if target.endswith('s') else target}Id"
    return f"=INDEX({target}!{{{{{field}}}}}, MATCH({{{{{fk}}}}}, {target}!{{{{{target_pk}}}}}, 0))"


def ensure_table(rb, name, description, fields, subject_area, is_extension=True):
    """Create a table if absent. Returns True if created."""
    if name in rb:
        if not rb[name]["schema"]:  # an owner script cleared it to rewrite the schema
            rb[name]["schema"] = fields
            rb[name]["Description"] = description
        return False
    assert fields[0]["name"] == f"{name[:-1] if name.endswith('s') else name}Id" or fields[0]["name"].endswith("Id"), \
        f"{name}: first field must be the <Entity>Id key (cr-25)"
    rb[name] = OrderedDict([("Description", description), ("important", True),
                            ("schema", fields), ("data", [])])
    tables = rb["RulebookTables"]["data"]
    if not any(r["RulebookTableId"] == name for r in tables):
        tables.append(OrderedDict([
            ("RulebookTableId", name), ("TableName", name), ("PhysicalTable", None),
            ("PhysicalView", None), ("SubjectArea", subject_area), ("IsExtension", is_extension),
            ("SemanticTypeIri", f"{EXT}RulebookTable")]))
    return True


def ensure_fields(rb, table, fields, after=None):
    """Append (or insert after `after`) fields that are not yet present."""
    schema = rb[table]["schema"]
    names = [f["name"] for f in schema]
    pos = names.index(after) + 1 if after else len(schema)
    added = []
    for f in fields:
        if f["name"] in names:
            cur = schema[names.index(f["name"])]
            for k in ("type", "formula", "RelatedTo"):
                if cur.get(k) != f.get(k):
                    raise SystemExit(f"{table}.{f['name']} exists with a different {k}: "
                                     f"{cur.get(k)!r} != {f.get(k)!r}")
            continue
        schema.insert(pos, f)
        names.insert(pos, f["name"])
        pos += 1
        added.append(f["name"])
    return added


def upsert_rows(rb, table, rows, key=None):
    """Insert rows whose key is absent; update authored keys on rows that exist."""
    key = key or rb[table]["schema"][0]["name"]
    data = rb[table]["data"]
    by = {r.get(key): r for r in data}
    n_new = 0
    for row in rows:
        cur = by.get(row[key])
        if cur is None:
            data.append(OrderedDict(row))
            by[row[key]] = data[-1]
            n_new += 1
        else:
            cur.update(row)
    return n_new


def set_provenance(rb, field_ids, question_id):
    """Record InventedForQuestion for fields; reconcile_field_catalog.py fills the rest."""
    catalog = rb["RulebookFields"]["data"]
    by = {r["RulebookFieldId"]: r for r in catalog}
    for fid in field_ids:
        if fid in by:
            current = by[fid].get("InventedForQuestion")
            if current and current != question_id:
                raise SystemExit(f"{fid} was invented for {current}; a later question cannot claim it. "
                                 f"Add a new field for {question_id} instead.")
            by[fid]["InventedForQuestion"] = question_id
        else:
            t, f = fid.split(".", 1)
            catalog.append(OrderedDict([("RulebookFieldId", fid), ("TargetTable", t),
                                        ("FieldName", f), ("InventedForQuestion", question_id)]))


def mapping(rb, sm_id, path, kind, iri, relation, profile, notes):
    upsert_rows(rb, "SemanticMappings", [OrderedDict([
        ("SemanticMappingId", sm_id), ("SourcePath", path), ("MappingKind", kind),
        ("TargetIri", iri), ("MappingRelation", relation), ("OntologyProfile", profile),
        ("Notes", notes)])])
