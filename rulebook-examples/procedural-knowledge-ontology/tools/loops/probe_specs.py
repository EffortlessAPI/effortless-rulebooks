#!/usr/bin/env python3
"""Validate loop specs against a scratch copy of the rulebook, in Postgres, in about a minute.

Nothing here touches the project rulebook or the project database:

  1. copy the rulebook to $TMPDIR/pko-probe-<name>/ and apply the given specs to the copy
  2. build that copy with rulebook-to-postgres only (the full project build runs ten tools)
  3. fail on any "Formula translation failed" (a green build does not mean a formula ran)
  4. load a scratch database erb_pko_probe_<name> from 00-05 plus the project's NNb-customize seams
  5. probe every view, then report every field the specs added: booleans as true/false/null,
     other fields as distinct values; a question's witness that cannot discriminate is VACUOUS

Several authors can probe at once: each name gets its own directory and database.

Usage: tools/loops/probe_specs.py --name collection tools/loops/loop06_foundation.py tools/loops/loop07_collection.py
Exit 1 on a translation failure, a load error, a view that does not answer, or a vacuous derived boolean witness.
"""
from __future__ import annotations

import argparse
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
import uuid
from pathlib import Path

PROJECT = Path(__file__).resolve().parents[2]
RULEBOOK = PROJECT / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json"


def norm(n: str) -> str:
    return re.sub(r"[^a-z0-9]", "", n.lower())


def run(cmd, **kw):
    return subprocess.run(cmd, capture_output=True, text=True, **kw)


def psql(db, sql):
    out = run(["psql", "-X", "-qtA", "-F", "\x1f", "-d", db, "-c", sql])
    if out.returncode != 0:
        raise RuntimeError(out.stderr.strip())
    return [l.split("\x1f") for l in out.stdout.splitlines() if l]


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--name", required=True, help="probe name: scratch dir and database suffix")
    ap.add_argument("specs", nargs="+")
    args = ap.parse_args()
    name = re.sub(r"[^a-z0-9_]", "_", args.name.lower())
    work = Path(tempfile.gettempdir()) / f"pko-probe-{name}"
    db = f"erb_pko_probe_{name}"

    if work.exists():
        shutil.rmtree(work)
    (work / "effortless-rulebook").mkdir(parents=True)
    (work / "postgres-bootstrap").mkdir()
    copy = work / "effortless-rulebook" / RULEBOOK.name
    shutil.copy(RULEBOOK, copy)
    before = json.loads(copy.read_text())
    before_fields = {f"{t}.{f['name']}" for t, v in before.items() if isinstance(v, dict) and "schema" in v
                     for f in v["schema"]}

    env = dict(os.environ, PKO_RULEBOOK=str(copy))
    out = run([sys.executable, str(PROJECT / "tools" / "apply_loop_spec.py"),
               *[str(Path(s).resolve()) for s in args.specs]], env=env)
    print(out.stdout.strip())
    if out.returncode != 0:
        print(out.stderr.strip() or out.stdout.strip())
        return 1
    chk = run([sys.executable, str(PROJECT / "tools" / "apply_loop_spec.py"), "--check",
               str(PROJECT / "tools" / "loops" / "loop05_coverage.py")], env=env)
    if chk.returncode != 0:
        print(chk.stdout.strip())
        return 1

    # The project's own ERB-PKO profile validation, including the acyclic relationship graph.
    val = run([sys.executable, str(PROJECT / "tools" / "pko_rulebook_tool.py"), "validate", "-i", str(copy)])
    if val.returncode != 0:
        print("PROFILE VALIDATION FAILED (tools/pko_rulebook_tool.py validate):")
        print("\n".join(l for l in val.stdout.splitlines() if l.startswith("- ")) or val.stdout[-2000:])
        return 1

    after = json.loads(copy.read_text())
    tables = {t: v for t, v in after.items() if isinstance(v, dict) and "schema" in v}
    added = [(t, f) for t, v in tables.items() for f in v["schema"] if f"{t}.{f['name']}" not in before_fields]
    witness = {r["RulebookFieldId"] for r in after["RulebookFields"]["data"] if r.get("InventedForQuestion")}

    project_cfg = json.loads((PROJECT / "effortless.json").read_text())
    settings = [s for s in project_cfg["ProjectSettings"] if s["Name"] != "project-name"]
    settings.append({"Name": "project-name", "Value": f"pko-probe-{name}"})
    pg = next(t for t in project_cfg["ProjectTranspilers"] if t["Name"] == "rulebooktopostgres")
    (work / "effortless.json").write_text(json.dumps({
        "SSoTmeProjectId": str(uuid.uuid5(uuid.NAMESPACE_URL, f"pko-probe-{name}")),
        "Name": f"pko-probe-{name}", "ProjectSettings": settings,
        "ProjectTranspilers": [dict(pg)]}, indent=1))
    for seam in (PROJECT / "postgres-bootstrap").glob("[0-9][0-9]b-customize-*.sql"):
        shutil.copy(seam, work / "postgres-bootstrap" / seam.name)

    print(f"==> rulebook-to-postgres ({work})")
    b = run(["effortless", "build"], cwd=work)
    if b.returncode != 0:
        print(b.stdout[-3000:], b.stderr[-2000:])
        return 1
    fn_sql = (work / "postgres-bootstrap" / "02-create-functions.sql").read_text()
    failures = sorted(set(re.findall(r"FUNCTION (calc_[a-z0-9_]+)\([^)]*\)[^$]*?\$\$[^$]*?Formula translation failed",
                                     fn_sql, flags=re.S)))
    if "Formula translation failed" in fn_sql:
        print("TRANSLATION FAILURES (these columns would silently read NULL):")
        for m in re.finditer(r"Formula translation failed[^\n]*", fn_sql):
            start = fn_sql.rfind("FUNCTION ", 0, m.start())
            print("  ", fn_sql[start:start + 90].split("(")[0], "|", m.group(0)[:160])
        return 1

    print(f"==> load {db}")
    run(["dropdb", "--if-exists", db])
    c = run(["createdb", db])
    if c.returncode != 0:
        print(c.stderr)
        return 1
    order = sorted(p.name for p in (work / "postgres-bootstrap").glob("[0-9][0-9]*-*.sql")
                   if not p.name.startswith(("06-", "99-")))
    errors = []
    for fname in order:
        strict = "b-" not in fname
        r = run(["psql", "-X", "-q", "-d", db, *(["-v", "ON_ERROR_STOP=1"] if strict else []),
                 "-f", str(work / "postgres-bootstrap" / fname)])
        errs = [l for l in r.stderr.splitlines() if "ERROR" in l]
        if r.returncode != 0 or errs:
            errors.append((fname, errs[:5] or [r.stderr[-400:]]))
            if strict:
                break
    if errors:
        print("LOAD ERRORS:")
        for fname, errs in errors:
            print(f"  {fname}:")
            for e in errs:
                print(f"    {e}")
        return 1

    views = {norm(v[0][3:]): v[0] for v in psql(db, "SELECT table_name FROM information_schema.views "
                                                  "WHERE table_schema='public' AND table_name LIKE 'vw\\_%' "
                                                  "AND table_name NOT LIKE '%\\_closure%'")}
    cols = {}
    for v, col in psql(db, "SELECT table_name, column_name FROM information_schema.columns "
                           "WHERE table_schema='public' AND table_name LIKE 'vw\\_%'"):
        cols.setdefault(v, {})[norm(col)] = col

    bad_views = []
    for view in views.values():
        try:
            psql(db, f"SELECT count(*) FROM {view}")
        except RuntimeError as e:
            bad_views.append((view, str(e)[:300]))
    if bad_views:
        print("VIEWS THAT DO NOT ANSWER:")
        for v, e in bad_views:
            print(f"  {v}: {e}")
        return 1

    print(f"==> {len(added)} added fields")
    vacuous = 0
    by_table: dict[str, list] = {}
    for t, f in added:
        by_table.setdefault(t, []).append(f)
    for t, fields in by_table.items():
        view = views.get(norm(t))
        if view is None:
            print(f"  {t}: no view")
            continue
        (n,) = psql(db, f"SELECT count(*) FROM {view}")[0]
        print(f"  {t} ({n} rows)")
        for f in fields:
            if f.get("type") == "closure":
                continue
            col = cols[view].get(norm(f["name"]))
            if col is None:
                print(f"    {f['name']}: NO COLUMN")
                vacuous += 1
                continue
            fid = f"{t}.{f['name']}"
            if f.get("datatype") == "boolean":
                tr, fa, nu = psql(db, f'SELECT count(*) FILTER (WHERE "{col}"), count(*) FILTER (WHERE NOT "{col}"), '
                                      f'count(*) FILTER (WHERE "{col}" IS NULL) FROM {view}')[0]
                ok = int(tr) > 0 and (int(fa) > 0 or int(nu) > 0)
                shown = f"t={tr} f={fa} null={nu}"
            else:
                (dv, sub) = psql(db, f"SELECT count(DISTINCT coalesce(NULLIF(\"{col}\"::text, ''), '<blank>')), "
                                     f"count(*) FILTER (WHERE \"{col}\" IS NOT NULL AND \"{col}\"::text NOT IN ('', 'false', 'f') "
                                     f"AND \"{col}\"::text !~ '^-?0+(\\.0+)?$') FROM {view}")[0]
                ok = int(dv) >= 2 or f.get("type") == "raw"
                shown = f"distinct={dv} substantive={sub}"
            flag = ""
            # A derived boolean is the witness shape a prescription relies on; counts and references
            # that feed it are allowed to be uniform.
            if fid in witness and f.get("datatype") == "boolean" and f.get("type") not in ("raw", "relationship") and not ok:
                flag = "  VACUOUS"
                vacuous += 1
            print(f"    {f['name']:44s} {f.get('type', ''):12s} {shown}{flag}")
    print(f"==> {vacuous} vacuous witness field(s)")
    return 1 if vacuous else 0


if __name__ == "__main__":
    raise SystemExit(main())
