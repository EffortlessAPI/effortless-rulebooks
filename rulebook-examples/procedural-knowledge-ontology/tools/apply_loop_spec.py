#!/usr/bin/env python3
"""Apply one witness-loop spec module to the rulebook.

A spec module (tools/loops/loopNN_*.py) declares, as plain data:

  LOOP       dict: WitnessLoopId, LoopNumber, Title, Premise
  ROLES      rows for Roles (and AGENTS for Agents) the loop's questions need
  TABLES     [(name, description, subject_area, [fields])]      new tables
  FIELDS     {table: [fields]}                                   fields added to existing tables
  RELATE     [(table, field, target)]                            raw columns turned into references
  QUESTIONS  [(question_id, asking_role, text, why, [Table.Field, ...])]
  ROWS       {table: [row dicts]}                                upserted by primary key
  MAPPINGS   [(id, source_path, kind, iri, relation, profile, notes)]

Every field named under a question gets InventedForQuestion = that question. A field
added by the loop that no question names is refused: the doctrine is that a derived
field exists because a role asked for it. Raw input columns may be listed under the
question that needs them too.

Idempotent. Re-read, insert only this loop's keys, write indent=1 / ensure_ascii=False.

Usage: tools/apply_loop_spec.py tools/loops/loop06_structure.py [...]
"""
from __future__ import annotations

import importlib.util
import re
import sys
from collections import OrderedDict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from rulebook_edit import EXT, dump, ensure_fields, ensure_table, load, mapping, set_provenance, upsert_rows  # noqa: E402


def load_spec(path: str):
    spec = importlib.util.spec_from_file_location(Path(path).stem, path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def apply(rb, mod) -> dict:
    loop = mod.LOOP
    stats = {"tables": 0, "fields": 0, "questions": 0, "rows": 0}
    loop_fields: set[str] = set()

    mapped_paths = {m[1] for m in getattr(mod, "MAPPINGS", [])}
    for name, desc, area, fields in getattr(mod, "TABLES", []):
        if name in rb:
            have = [f["name"] for f in rb[name]["schema"]]
            if have[:len(fields)] != [f["name"] for f in fields][:len(have)]:
                raise SystemExit(f"{loop['WitnessLoopId']}: table {name} already exists with a different schema "
                                 f"(created by another loop?). Add fields through FIELDS instead.")
        created = ensure_table(rb, name, desc, fields, area)
        stats["tables"] += int(created)
        if created:
            loop_fields.update(f"{name}.{f['name']}" for f in fields)
        if name in mapped_paths:
            continue  # the spec maps this table to a real ontology term; it is not an extension
        # The class IRI is SINGULAR -- it is what the rows are typed with and how ontologies
        # name classes (pko:Procedure, not pko:Procedures). It comes from the table's own
        # <Entity>Id, never from stripping a trailing "s": that turns StepProtectiveEquipment
        # into StepProtectiveEquipmen. Minting it from the plural table name left 132 classes
        # declared that no instance was ever a member of; see tools/align_extension_class_iris.py.
        pk = rb[name]["schema"][0]["name"]
        if not pk.endswith("Id"):
            raise SystemExit(f"{name}: leading field {pk!r} is not an <Entity>Id (cr-25); "
                             f"cannot name its class.")
        upsert_rows(rb, "SemanticMappings", [OrderedDict([
            ("SemanticMappingId", f"map-{name}"), ("SourcePath", name), ("MappingKind", "class"),
            ("TargetIri", f"{EXT}{pk[:-2]}"), ("MappingRelation", "extension"),
            ("OntologyProfile", "erb-pko-extension-1.0.0"),
            ("Notes", f"Added by {loop['WitnessLoopId']}; not part of PKO 2.0.0.")])])

    for table, fields in getattr(mod, "FIELDS", {}).items():
        if table not in rb:
            raise SystemExit(f"FIELDS names unknown table {table}")
        added = ensure_fields(rb, table, fields)
        stats["fields"] += len(added)
        loop_fields.update(f"{table}.{n}" for n in added)

    for table, field, target in getattr(mod, "RELATE", []):
        f = next((x for x in rb[table]["schema"] if x["name"] == field), None)
        if f is None or target not in rb:
            raise SystemExit(f"RELATE {table}.{field} -> {target}: field or target missing")
        if f["type"] != "relationship":
            f["type"] = "relationship"
            f["datatype"] = "string"
            f.pop("formula", None)
            f["RelatedTo"] = target

    for row in getattr(mod, "ROLES", []):
        upsert_rows(rb, "Roles", [row])
    for row in getattr(mod, "AGENTS", []):
        upsert_rows(rb, "Agents", [row])

    upsert_rows(rb, "WitnessLoops", [OrderedDict([
        ("WitnessLoopId", loop["WitnessLoopId"]), ("LoopNumber", loop["LoopNumber"]),
        ("Title", loop["Title"]), ("Premise", loop["Premise"]),
        ("SemanticTypeIri", f"{EXT}WitnessLoop")])])

    all_fields = {f"{t}.{f['name']}" for t, v in rb.items() if isinstance(v, dict) and "schema" in v
                  for f in v["schema"]}
    roles = {r["RoleId"] for r in rb["Roles"]["data"]}
    named: set[str] = set()
    for qid, role, text, why, fields in getattr(mod, "QUESTIONS", []):
        if role not in roles:
            raise SystemExit(f"{qid}: asking role {role} does not exist")
        missing = [f for f in fields if f not in all_fields]
        if missing:
            raise SystemExit(f"{qid}: names fields that do not exist: {missing}")
        upsert_rows(rb, "RoleQuestions", [OrderedDict([
            ("RoleQuestionId", qid), ("AskingRole", role), ("WitnessLoop", loop["WitnessLoopId"]),
            ("QuestionText", text), ("WhyItMatters", why), ("AnswerableBefore", False),
            ("SemanticTypeIri", f"{EXT}RoleQuestion")])])
        set_provenance(rb, fields, qid)
        named.update(fields)
        stats["questions"] += 1

    orphans = sorted(f for f in loop_fields - named
                     if not f.endswith((".Name", ".SemanticTypeIri")) and not f.split(".")[1].endswith("Id"))
    if orphans:
        raise SystemExit(f"{loop['WitnessLoopId']}: fields no question asked for: {orphans}")

    for table, rows in getattr(mod, "ROWS", {}).items():
        if table not in rb:
            raise SystemExit(f"ROWS names unknown table {table}")
        cols = {f["name"] for f in rb[table]["schema"]}
        for r in rows:
            bad = [k for k in r if k not in cols]
            if bad:
                raise SystemExit(f"{table} row {next(iter(r.values()))}: unknown columns {bad}")
        stats["rows"] += upsert_rows(rb, table, [OrderedDict(r) for r in rows])

    for m in getattr(mod, "MAPPINGS", []):
        mapping(rb, *m)
    return stats


def main() -> int:
    args = [a for a in sys.argv[1:] if a != "--check"]
    check = "--check" in sys.argv[1:]
    if not args:
        print(__doc__)
        return 2
    rb = load()
    for path in args:
        mod = load_spec(path)
        stats = apply(rb, mod)
        print(f"{mod.LOOP['WitnessLoopId']}: {stats}")
    if check:
        # Validate without writing: dangling references and forbidden formula shapes.
        tables = {k for k, v in rb.items() if isinstance(v, dict) and "schema" in v}
        problems = []
        for t in tables:
            names = {f["name"] for f in rb[t]["schema"]}
            for f in rb[t]["schema"]:
                if f.get("type") == "relationship" and f.get("RelatedTo") not in tables:
                    problems.append(f"{t}.{f['name']}: RelatedTo {f.get('RelatedTo')!r} is not a table")
                if f.get("type") == "closure":
                    edge = rb.get(f.get("EdgeTable"), {}).get("schema", [])
                    kinds = {x["name"]: x.get("type") for x in edge}
                    for end in ("FromColumn", "ToColumn"):
                        if kinds.get(f.get(end)) != "relationship":
                            problems.append(f"{t}.{f['name']}: {end} {f.get(end)!r} must be a relationship column "
                                            f"(rulebook-to-owl refuses any other endpoint)")
                fo = f.get("formula") or ""
                for bad in ("IIF(", "ISBLANK(", "NULLIF(", "LOOKUP("):
                    if bad in fo and not (bad == "LOOKUP(" and "VLOOKUP(" in fo):
                        problems.append(f"{t}.{f['name']}: uses {bad}")
                for ref in re.findall(r"(?<![!A-Za-z_])\{\{([A-Za-z0-9_]+)\}\}", fo):
                    if ref not in names:
                        problems.append(f"{t}.{f['name']}: references unknown local field {ref}")
                for tbl, ref in re.findall(r"([A-Za-z_][A-Za-z0-9_]*)!\{\{([A-Za-z0-9_]+)\}\}", fo):
                    if tbl.startswith("vw_"):
                        continue
                    if tbl not in tables:
                        problems.append(f"{t}.{f['name']}: references unknown table {tbl}")
                    elif ref not in {x["name"] for x in rb[tbl]["schema"]}:
                        problems.append(f"{t}.{f['name']}: references unknown field {tbl}.{ref}")
            for row in rb[t].get("data", []):
                for f in rb[t]["schema"]:
                    if f.get("type") == "relationship" and row.get(f["name"]) not in (None, ""):
                        target = f["RelatedTo"]
                        pk = rb[target]["schema"][0]["name"]
                        if not any(r.get(pk) == row[f["name"]] for r in rb[target].get("data", [])):
                            problems.append(f"{t}.{row.get(rb[t]['schema'][0]['name'])}.{f['name']}: "
                                            f"{row[f['name']]!r} is not a {target} key")
        if problems:
            print(f"{len(problems)} problem(s):")
            print("\n".join("  " + x for x in problems[:200]))
            return 1
        print("check passed (nothing written)")
        return 0
    dump(rb)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
