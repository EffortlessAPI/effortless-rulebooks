#!/usr/bin/env python3
"""Save what people did in the app into the rulebook ("Save to the book").

The app is a normal Postgres app: an action writes a base table and stays there. The
rulebook is still the single source of truth, so a session is not part of the model until
it is saved. This tool carries the difference across, for exactly the tables the app can
write (AppActions.TargetTable) and exactly the columns it can write (AppActionFields):

  - a row in Postgres whose key is not in the rulebook is ADDED (stored columns only)
  - a row in both whose WRITABLE columns differ is UPDATED (those columns only)
  - nothing is ever deleted, and no derived column is ever written

Postgres is the oracle for what the rows are; nothing is computed here. After saving, the
next build recomputes every derived value from the saved facts.

  python3 tools/save_session_to_book.py --dry-run [--json]   what would be saved
  python3 tools/save_session_to_book.py                      save it
"""
import json
import os
import re
import subprocess
import sys
from datetime import datetime, timezone

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
# PKO_RULEBOOK points a test at a scratch copy, as it does for tools/rulebook_edit.py.
RB = os.environ.get("PKO_RULEBOOK") or os.path.join(ROOT, "effortless-rulebook", "procedural-knowledge-ontology-rulebook.json")
DB = os.environ.get("DATABASE_URL", "postgresql://postgres@localhost:5432/erb_procedural_knowledge_ontology")
IRI = "https://effortlessapi.github.io/effortless-rulebooks/ns/pko-extension#"


def snake(n):
    return re.sub(r"_+", "_", re.sub(r"(?<!^)(?=[A-Z])", "_", n).lower())


def live_rows(physical, columns):
    sql = (f"select coalesce(json_agg(t), '[]'::json) from "
           f"(select {', '.join(columns)} from public.{physical}) t")
    r = subprocess.run(["psql", DB, "-tA", "-v", "ON_ERROR_STOP=1", "-c", sql], capture_output=True, text=True)
    if r.returncode:
        raise SystemExit(f"FATAL: cannot read public.{physical}: {r.stderr}")
    return json.loads(r.stdout)


def same(a, b, datatype):
    """Equality by value, not spelling: a blank is a blank, an instant is an instant."""
    blank = (None, "")
    if a in blank and b in blank:
        return True
    if a in blank or b in blank:
        return False
    if datatype in ("datetime", "date"):
        def inst(v):
            d = datetime.fromisoformat(str(v).replace("Z", "+00:00"))
            return d if d.tzinfo else d.replace(tzinfo=timezone.utc)
        return inst(a) == inst(b)
    if datatype in ("integer", "number"):
        return float(a) == float(b)
    if datatype == "boolean":
        return (a is True or a == "true") == (b is True or b == "true")
    return str(a) == str(b)


def main():
    dry, as_json = "--dry-run" in sys.argv, "--json" in sys.argv
    rb = json.load(open(RB))
    phys = {t["TableName"]: t.get("PhysicalTable") for t in rb["RulebookTables"]["data"]}
    action_table = {a["AppActionId"]: a["TargetTable"] for a in rb["AppActions"]["data"]}
    writable = {}
    for af in rb["AppActionFields"]["data"]:
        t, f = af["TargetField"].split(".", 1)
        assert t == action_table[af["AppAction"]], af["AppActionFieldId"]
        writable.setdefault(t, set()).add(f)

    added, updated = [], []
    for table in sorted(writable):
        if not phys.get(table):
            raise SystemExit(f"FATAL: {table} has no PhysicalTable in RulebookTables")
        stored = [f for f in rb[table]["schema"] if f["type"] in ("raw", "relationship")]
        pk = rb[table]["schema"][0]["name"]
        dtype = {f["name"]: f.get("datatype") for f in stored}
        rows = live_rows(phys[table], [snake(f["name"]) for f in stored])
        by_key = {r[pk]: r for r in rb[table]["data"]}
        for live in rows:
            row = {f["name"]: live[snake(f["name"])] for f in stored}
            cur = by_key.get(row[pk])
            if cur is None:
                added.append((table, {k: v for k, v in row.items() if v is not None}))
                continue
            diff = {c: row[c] for c in sorted(writable[table])
                    if c != pk and not same(cur.get(c), row[c], dtype.get(c))}
            if diff:
                updated.append((table, row[pk], diff))

    summary = {"added": [{"table": t, "key": next(iter(r.values())), "row": r} for t, r in added],
               "updated": [{"table": t, "key": k, "changes": d} for t, k, d in updated]}
    if as_json:
        print(json.dumps(summary, default=str))
    else:
        for a in summary["added"]:
            print(f"  + {a['table']}  {a['key']}")
        for u in summary["updated"]:
            print(f"  ~ {u['table']}  {u['key']}  {u['changes']}")
        print(f"{len(added)} row(s) to add, {len(updated)} to update" + ("  (dry run, nothing written)" if dry else ""))
    if dry or not (added or updated):
        return 0

    rb = json.load(open(RB))  # re-read: contended file; touch only our rows
    for table, row in added:
        if not any(r.get(rb[table]["schema"][0]["name"]) == next(iter(row.values())) for r in rb[table]["data"]):
            rb[table]["data"].append(row)
    for table, key, diff in updated:
        pk = rb[table]["schema"][0]["name"]
        next(r for r in rb[table]["data"] if r.get(pk) == key).update(diff)

    # The data has a version of its own, separate from the model's release.
    snap = subprocess.run(["psql", DB, "-tA", "-c",
                           "select to_char(as_of_instant at time zone 'UTC', 'YYYY-MM-DD\"T\"HH24:MI:SS\"+00:00\"'), "
                           "to_char(as_of_instant at time zone 'UTC', 'YYYY.MM.DD') from vw_evaluation_contexts where is_current"],
                          capture_output=True, text=True).stdout.strip().split("|")
    n = 1 + sum(1 for v in rb["InstanceDataVersions"]["data"] if v["InstanceDataVersionId"].startswith(f"idv-app-{snap[1]}"))
    latest = rb["InstanceDataVersions"]["data"][-1]
    rb["InstanceDataVersions"]["data"].append({
        "InstanceDataVersionId": f"idv-app-{snap[1]}-{n}", "GovernedModel": latest["GovernedModel"],
        "DataVersionLabel": f"data-{snap[1]}.app{n}", "SnapshotAt": snap[0],
        "ConformsToRelease": latest["ConformsToRelease"], "SemanticTypeIri": IRI + "InstanceDataVersion"})

    tmp = RB + ".tmp"
    json.dump(rb, open(tmp, "w"), indent=1, ensure_ascii=False)
    os.replace(tmp, RB)
    print(f"saved to the book as data version data-{snap[1]}.app{n}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
