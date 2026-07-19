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


# VERIFIED unsupported by rulebook-to-postgres. These do not fail the build —
# they emit a warning comment, return NULL, and the column reads as a clean
# false forever. Reject them at authoring time instead.
BANNED_FUNCS = {"IIF": "use IF(cond, a, b)"}


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

    for bad, hint in BANNED_FUNCS.items():
        if re.search(rf"\b{bad}\s*\(", formula):
            problems.append(f"uses {bad}(), which the transpiler silently returns NULL for — {hint}")

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
    skipped_existing: list[str] = []
    merged_columns: list[str] = []
    declared_here: set[tuple[str, str]] = set()
    new_questions: list[OrderedDict] = []
    new_fields: list[tuple[str, OrderedDict, str]] = []  # (table, field, question_id)
    new_tables: list[str] = []

    # 1. New tables the spec declares. Drafting agents used three different keys
    # for the table name and two for the seed rows, so accept all of them rather
    # than crashing on a shape variant.
    for q in spec["questions"]:
        for nt in ([q["needs_table"]] if isinstance(q.get("needs_table"), dict)
                   else q.get("needs_table") or []):
            name = nt.get("table") or nt.get("name") or nt.get("table_name")
            if not name:
                problems.append(f"{q['id']}: needs_table has no table/name/table_name key")
                continue
            if name in rb:
                # Two roles can both need the same new table, and the second
                # spec's version may declare columns the first did not. Merge
                # them in rather than silently dropping the difference.
                have = set(field_names(rb, name))
                sch = rb[name]["schema"]
                for f in nt["schema"]:
                    if isinstance(f, dict) and f["name"] not in have:
                        idx = ([x["name"] for x in sch].index("SemanticTypeIri")
                               if "SemanticTypeIri" in [x["name"] for x in sch] else len(sch))
                        sch.insert(idx, f)
                        merged_columns.append(f"{name}.{f['name']}")
                continue
            schema = list(nt["schema"])
            # Some specs separate the derived columns from the raw schema.
            for extra in nt.get("derived_predicates", []):
                schema.append(OrderedDict([
                    ("name", extra["field_name"]),
                    ("datatype", extra["datatype"]),
                    ("type", extra["field_type"]),
                    ("nullable", True),
                    ("Description", extra.get("description", "")),
                ] + ([("formula", extra["formula"])] if extra.get("formula") else [])))
            rb[name] = OrderedDict([
                ("Description", nt.get("description") or nt.get("why")
                 or f"{name} (added by witness loop {args.loop})."),
                ("schema", schema),
                ("data", nt.get("data") or nt.get("seed_rows") or []),
            ])
            new_tables.append(name)

    # 1b. Some specs express a new table implicitly: rather than a needs_table
    # block, they emit predicates whose target_table does not exist yet, the
    # first of which is the primary key. Synthesize the table from those columns
    # so the spec's own ordering defines the schema.
    for q in spec["questions"]:
        for p in q.get("predicates", []):
            tbl = p["target_table"]
            if tbl in rb or tbl in new_tables:
                continue
            cols = [pp for qq in spec["questions"] for pp in qq.get("predicates", [])
                    if pp["target_table"] == tbl]
            pk = f"{tbl[:-1] if tbl.endswith('s') else tbl}Id"
            if not any(c["field_name"] == pk for c in cols):
                continue  # no primary key among them — a genuine typo, let it fail
            rb[tbl] = OrderedDict([
                ("Description", f"{tbl} (added by witness loop {args.loop})."),
                ("schema", []),
                ("data", []),
            ])
            new_tables.append(tbl)

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
            if (tbl, fname) in declared_here:
                # The spec declared the same field twice. The transpiler happily
                # emits both, and Postgres then rejects the view with "column
                # specified more than once".
                problems.append(f"{tbl}.{fname}: declared twice within this spec")
                continue
            declared_here.add((tbl, fname))
            if fname in field_names(rb, tbl):
                # Several roles independently need the same atom (the drafting
                # agents flagged these as "declare once"). The first role to be
                # applied owns it; later roles simply reuse the existing column.
                skipped_existing.append(f"{tbl}.{fname}")
                continue
            for msg in check_formula(rb, tbl, p.get("formula", ""), pending_by_table):
                problems.append(f"{tbl}.{fname}: {msg}")

            if p["field_type"] == "relationship" and not p.get("related_to"):
                problems.append(f"{tbl}.{fname}: relationship field needs related_to "
                                "(or should be raw/lookup if it would create a table cycle)")
                continue
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
    if merged_columns:
        print(f"  merged {len(merged_columns)} column(s) into a table an earlier role created: "
              f"{', '.join(merged_columns[:6])}")
    if skipped_existing:
        print(f"  reusing {len(skipped_existing)} shared predicate(s) already declared "
              f"by an earlier role: {', '.join(skipped_existing[:6])}"
              + (" ..." if len(skipped_existing) > 6 else ""))
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
