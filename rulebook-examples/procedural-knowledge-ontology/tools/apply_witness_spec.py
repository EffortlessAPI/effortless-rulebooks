#!/usr/bin/env python3
"""Apply one role's witness spec to the rulebook.

Adds the role's RoleQuestions rows, any new tables the spec declares, and the
predicate fields themselves — then records provenance so every invented field
points back at the question that motivated it.

Fails loudly rather than guessing:
  - unknown target table
  - duplicate field name
  - a formula referencing a field that does not exist on its own table
  - a lookup/aggregation naming a table that does not exist

Usage:
    tools/apply_witness_spec.py .witness-specs/<role>.json [--loop 1] [--dry-run]
"""
from __future__ import annotations

import argparse
import json
import re
import sys
from collections import OrderedDict
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
QUESTION_IRI = "urn:effortless:pko-extension#RoleQuestion"

# Fields a formula may reference that are not columns.
FORMULA_FUNCS = {
    "IF", "IIF", "AND", "OR", "NOT", "LEFT", "RIGHT", "MID", "LEN", "CONCAT",
    "DATETIME_DIFF", "DATEADD", "NOW", "TODAY", "INDEX", "MATCH", "COUNTIFS",
    "SUMIFS", "COUNTA", "COUNT", "SUM", "MIN", "MAX", "ROUND", "ABS", "TRUE",
    "FALSE", "VALUE", "TEXT", "UPPER", "LOWER", "TRIM", "SUBSTITUTE", "COALESCE",
    "AVERAGE", "AVERAGEIFS", "MAXIFS", "MINIFS", "ISNUMBER", "SEARCH", "FIND",
}


def load(path: Path):
    with path.open() as fh:
        return json.load(fh, object_pairs_hook=OrderedDict)


def table_names(rb) -> set[str]:
    return {k for k, v in rb.items() if isinstance(v, dict) and "schema" in v}


def field_names(rb, table: str) -> list[str]:
    return [f["name"] for f in rb[table]["schema"] if isinstance(f, dict)]


def check_formula(rb, table: str, formula: str, pending: dict[str, set[str]]) -> list[str]:
    """Return a list of problems with this formula. Empty means it looks sane.

    `pending` maps table -> field names being added by this same spec. A spec
    routinely composes its own new fields across tables (an atomic predicate on
    the child, rolled up by an aggregation on the parent), so those must count
    as existing for validation purposes.
    """
    problems: list[str] = []
    if not formula:
        return problems

    own = set(field_names(rb, table)) | pending.get(table, set())
    tables = table_names(rb)

    # Cross-table references look like TableName!{{Field}}
    for tbl, fld in re.findall(r"(\w+)!\{\{(\w+)\}\}", formula):
        if tbl not in tables:
            problems.append(f"references unknown table {tbl!r}")
        elif fld not in set(field_names(rb, tbl)) | pending.get(tbl, set()):
            problems.append(f"references unknown field {tbl}.{fld}")

    # Bare {{Field}} references must exist on this table. Strip cross-table
    # references first so their field parts are not double-counted.
    stripped = re.sub(r"\w+!\{\{\w+\}\}", "", formula)
    for fld in re.findall(r"\{\{(\w+)\}\}", stripped):
        if fld not in own and fld.upper() not in FORMULA_FUNCS:
            problems.append(f"references unknown field {table}.{fld}")

    return problems


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("spec")
    ap.add_argument("--loop", type=int, default=1)
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    rb = load(RB)
    spec = load(Path(args.spec))
    role = spec["role"]

    if role not in {r["RoleId"] for r in rb["Roles"]["data"]}:
        print(f"error: unknown role {role!r}", file=sys.stderr)
        return 1

    loop_id = f"loop-{args.loop:02d}"
    existing_q = {r["RoleQuestionId"] for r in rb["RoleQuestions"]["data"]}
    problems: list[str] = []
    new_questions: list[OrderedDict] = []
    new_fields: list[tuple[str, OrderedDict, str]] = []  # (table, field, question_id)
    new_tables: list[str] = []

    # 1. New tables the spec declares.
    for q in spec["questions"]:
        for nt in ([q["needs_table"]] if isinstance(q.get("needs_table"), dict)
                   else q.get("needs_table") or []):
            name = nt["table"] if "table" in nt else nt["name"]
            if name in rb:
                continue
            rb[name] = OrderedDict([
                ("Description", nt.get("description", f"{name} (added by witness loop {args.loop}).")),
                ("schema", nt["schema"]),
                ("data", nt.get("data", [])),
            ])
            new_tables.append(name)

    # 2. Questions.
    for q in spec["questions"]:
        qid = q["id"]
        if qid in existing_q:
            problems.append(f"duplicate question id {qid!r}")
            continue
        new_questions.append(OrderedDict([
            ("RoleQuestionId", qid),
            ("AskingRole", role),
            ("WitnessLoop", loop_id),
            ("QuestionText", q["text"]),
            ("WhyItMatters", q.get("why", "")),
            ("AnswerableBefore", bool(q.get("answerable_before", False))),
            ("SemanticTypeIri", QUESTION_IRI),
        ]))

    # 3. Predicates. Validate before mutating so a bad spec changes nothing.
    pending_by_table: dict[str, set[str]] = {}
    for q in spec["questions"]:
        for p in q.get("predicates", []):
            pending_by_table.setdefault(p["target_table"], set()).add(p["field_name"])
    # Tables created above are already in `rb`, but count their columns as
    # pending too so ordering within the spec never matters.
    for name in new_tables:
        pending_by_table.setdefault(name, set()).update(field_names(rb, name))

    # Validate the formulas ON the new tables too. Skipping these let a lookup
    # against a non-existent field (Agents.PrimaryRoleId) through, and
    # rulebook-to-postgres silently emits NO calc function for such a lookup
    # while still generating the view that calls it — green build, dead DB.
    for name in new_tables:
        for f in rb[name]["schema"]:
            if isinstance(f, dict) and f.get("formula"):
                for msg in check_formula(rb, name, f["formula"], pending_by_table):
                    problems.append(f"{name}.{f['name']}: {msg}")

    for q in spec["questions"]:
        for p in q.get("predicates", []):
            tbl, fname = p["target_table"], p["field_name"]
            if tbl not in rb:
                problems.append(f"{fname}: unknown target table {tbl!r}")
                continue
            if fname in field_names(rb, tbl):
                problems.append(f"{tbl}.{fname}: field already exists")
                continue
            for msg in check_formula(rb, tbl, p.get("formula", ""), pending_by_table):
                problems.append(f"{tbl}.{fname}: {msg}")

            field = OrderedDict([
                ("name", fname),
                ("datatype", p["datatype"]),
                ("type", p["field_type"]),
                ("nullable", True),
                ("Description", p["description"]),
            ])
            if p.get("formula"):
                field["formula"] = p["formula"]
            if p.get("related_to"):
                field["RelatedTo"] = p["related_to"]
            new_fields.append((tbl, field, q["id"]))

    if problems:
        print(f"REFUSING to apply {args.spec} — {len(problems)} problem(s):", file=sys.stderr)
        for pb in problems:
            print(f"  {pb}", file=sys.stderr)
        return 1

    print(f"role {role}: {len(new_questions)} questions, {len(new_fields)} predicates, "
          f"{len(new_tables)} new table(s)")
    if args.dry_run:
        for tbl, f, qid in new_fields:
            print(f"  + {tbl}.{f['name']} ({f['type']}) <- {qid}")
        return 0

    # 4. Mutate.
    rb["RoleQuestions"]["data"].extend(new_questions)
    provenance: dict[str, str] = {}
    for tbl, field, qid in new_fields:
        # Place derived fields before the trailing SemanticTypeIri when present.
        sch = rb[tbl]["schema"]
        names = [f["name"] for f in sch if isinstance(f, dict)]
        idx = names.index("SemanticTypeIri") if "SemanticTypeIri" in names else len(sch)
        sch.insert(idx, field)
        provenance[f"{tbl}.{field['name']}"] = qid

    # 5. Record provenance for the catalog reconciler to preserve.
    catalog = rb["RulebookFields"]["data"]
    have = {r["RulebookFieldId"] for r in catalog}
    for fid, qid in provenance.items():
        tbl, fname = fid.split(".", 1)
        if fid in have:
            for r in catalog:
                if r["RulebookFieldId"] == fid:
                    r["InventedForQuestion"] = qid
        else:
            catalog.append(OrderedDict([
                ("RulebookFieldId", fid),
                ("TargetTable", tbl),
                ("FieldName", fname),
                ("InventedForQuestion", qid),
            ]))

    with RB.open("w") as fh:
        json.dump(rb, fh, indent=2)
        fh.write("\n")

    print(f"applied. run: python3 tools/reconcile_field_catalog.py")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
