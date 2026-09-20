#!/usr/bin/env python3
"""Check every provenance explainer in the role app against the real field catalog.

The app puts a `ƒ` beside a value and names the (table, column) pair behind it. The
API answers that pair from `vw_rulebook_fields`, and refuses loudly when the catalog
has never heard of it. That refusal is correct but late: it shows up as a red error in
front of whoever signed in. This is the same check, before anyone looks.

For every explainer in `app/mobile/src`:

  * the table must have a generated `vw_<table>`,
  * the column must really exist on it (information_schema is the authority),
  * some RulebookFields row must claim that column.

A pair that fails any of the three is a defect in the app, and this exits non-zero
naming the file and line. There is no tolerance setting: an explainer that cannot
explain anything is worse than no explainer, because it says "this value came from
nowhere" when the truth is that the app asked the wrong question.

It also reports what the explainers actually cover — how many are derived fields,
how many are witnesses, and which tables in each role's reach carry none — so
"provenance is everywhere" is a number rather than a claim.

`--live` goes further and asks the API, signed in as the person whose page each
explainer is on. That is a different question, and the one that decides whether a popup
opens: provenance obeys the same boundary as the data, so a column the role's own schema
does not carry is a 404 no matter how real the field is. Only the running app can say.

  python3 tools/check_explainer_coverage.py            # the gate
  python3 tools/check_explainer_coverage.py --report   # + per-file and per-table detail
  python3 tools/check_explainer_coverage.py --live     # + open every popup as its own role
"""
from __future__ import annotations

import json
import os
import re
import subprocess
import sys
import urllib.error
import urllib.request
from collections import defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SRC = ROOT / "app" / "mobile" / "src"
DB = os.environ.get("PGDATABASE") or "erb_procedural_knowledge_ontology"
API = os.environ.get("API_URL") or "http://localhost:8099"

# The components that carry an explainer. `f` is the column; `t` names the table when
# the block it sits in does not.
EXPLAINERS = ("Why", "Fact", "Val", "Th", "Big", "Tag")
# The components that put every explainer inside them onto one table.
SCOPES = ("Explains", "KV")


def psql(sql: str) -> list[list[str]]:
    out = subprocess.run(
        ["psql", "-X", "-qtA", "-F", "\x1f", "-v", "ON_ERROR_STOP=1", "-d", DB, "-c", sql],
        capture_output=True, text=True)
    if out.returncode != 0:
        raise SystemExit(f"psql failed against {DB}:\n{sql[:400]}\n{out.stderr}")
    return [line.split("\x1f") for line in out.stdout.splitlines() if line]


def norm(name: str) -> str:
    return re.sub(r"[^a-z0-9]", "", name.lower())


def load_catalog() -> tuple[dict[str, set[str]], dict[tuple[str, str], dict]]:
    """Real view columns, and the catalog row that claims each one.

    The resolution is the API's: the transpiler's snake_case candidate first, then an
    underscore-stripped match, which absorbs any disagreement about where word breaks
    fall. Nothing is guessed — a column with no catalog row simply has none.
    """
    cols: dict[str, set[str]] = defaultdict(set)
    for view, col in psql(
            "SELECT table_name, column_name FROM information_schema.columns "
            "WHERE table_schema='public' AND table_name LIKE 'vw\\_%'"):
        cols[view].add(col)

    by_col: dict[tuple[str, str], dict] = {}
    for tbl, fname, ftype, derived, witness, question in psql(
            "SELECT target_table, field_name, field_type, is_derived, is_witness, "
            "coalesce(invented_for_question, '') FROM vw_rulebook_fields"):
        view = "vw_" + re.sub(r"([a-z0-9])([A-Z])", r"\1_\2",
                              re.sub(r"([A-Z]+)([A-Z][a-z])", r"\1_\2", tbl)).lower()
        have = cols.get(view)
        if not have:
            continue
        cand = re.sub(r"([a-z0-9])([A-Z])", r"\1_\2",
                      re.sub(r"([A-Z]+)([A-Z][a-z])", r"\1_\2", fname)).lower()
        col = cand if cand in have else next(
            (c for c in have if norm(c) == norm(fname)), None)
        if col:
            by_col[(view[3:], col)] = {
                "field": f"{tbl}.{fname}", "type": ftype,
                "derived": derived == "t", "witness": witness == "t", "question": question}
    return {v[3:]: c for v, c in cols.items()}, by_col


TAG_RE = re.compile(r"<(/?)(" + "|".join(SCOPES + EXPLAINERS) + r")\b")
ATTR_RE = re.compile(r'\b([ft])="([a-z][a-z0-9_]*)"')


def tag_attrs(text: str, start: int) -> tuple[str, bool, int]:
    """The attribute text of the JSX tag opening at `start`, to its real `>`.

    A plain `[^>]*` is wrong here: `<Tag tone={n > 0 ? "a" : "b"} f="n">` carries a `>`
    inside a braced expression, and stopping at it silently drops the explainer. So walk
    the tag, tracking brace depth and string quotes.
    """
    i, depth = start, 0
    while i < len(text):
        c = text[i]
        if c in "\"'" and depth:                      # a string inside an expression
            q = c; i += 1
            while i < len(text) and text[i] != q:
                i += 2 if text[i] == "\\" else 1
        elif c == "{":
            depth += 1
        elif c == "}":
            depth -= 1
        elif c == '"' and not depth:                  # a plain attribute value
            i += 1
            while i < len(text) and text[i] != '"':
                i += 1
        elif c == ">" and not depth:
            return text[start:i], text[i - 1] == "/", i
        i += 1
    raise SystemExit(f"unterminated JSX tag at offset {start}")


def scan(path: Path) -> tuple[list[dict], int, int]:
    """Every explainer in one file, with the table it resolves to.

    Returns the explainers it resolved, how many name their column at runtime
    (generated pages pass real columns from the rows they just read, so those are real
    by construction), and the raw count of literal `f="..."` attributes. The caller
    asserts that every literal one was resolved: a pattern this scanner failed to see
    would otherwise be silently unchecked, which is the failure mode it exists to stop.
    """
    text = path.read_text()
    lineno = lambda pos: text.count("\n", 0, pos) + 1  # noqa: E731

    found: list[dict] = []
    stack: list[str | None] = []
    dyn_col = 0    # <Th f={c}> — the column is a runtime value
    dyn_scope = 0  # literal f="..." under a runtime <Explains t={table}>
    for m in TAG_RE.finditer(text):
        closing, name = m.group(1), m.group(2)
        attrs, selfclose, _end = tag_attrs(text, m.end())
        if name in SCOPES:
            if closing:
                if stack:
                    stack.pop()
            elif not selfclose:
                lit = dict(ATTR_RE.findall(attrs))
                # `t={table}` is a runtime table: the columns under it come from the
                # rows the role's own view returned, so they are real by construction.
                stack.append(lit.get("t"))
            continue
        if closing:
            continue
        a = dict(ATTR_RE.findall(attrs))
        if "f" not in a:
            if 'f={' in attrs:
                dyn_col += 1
            continue
        table = a.get("t") or (stack[-1] if stack else None)
        if table is None:
            if stack and stack[-1] is None:
                dyn_scope += 1
                continue
            raise SystemExit(
                f"{path.relative_to(ROOT)}:{lineno(m.start())}: <{name} f=\"{a['f']}\"> names no "
                f"table, and is not inside an <Explains t=...> or <KV t=...>. An explainer with "
                f"no table can explain nothing.")
        found.append({"file": path.relative_to(ROOT), "line": lineno(m.start()),
                      "component": name, "table": table, "column": a["f"]})
    raw = len(re.findall(r'\bf="[a-z][a-z0-9_]*"', text))
    # Every literal f="..." is either resolved here or sits under a runtime table. A
    # mismatch means this scanner missed a pattern, and silently unchecked explainers
    # are the one thing it must not produce.
    if len(found) + dyn_scope != raw:
        raise SystemExit(
            f"{path.relative_to(ROOT)}: {raw} literal f=\"...\" attributes, but this scanner "
            f"resolved {len(found)} and placed {dyn_scope} under a runtime table. It missed "
            f"{raw - len(found) - dyn_scope}; fix the scanner, not the app.")
    return found, dyn_col + dyn_scope, raw


def api(path: str, token: str | None = None, body: dict | None = None) -> dict:
    req = urllib.request.Request(
        API + path, method="POST" if body else "GET",
        data=json.dumps(body).encode() if body else None,
        headers={"content-type": "application/json",
                 **({"authorization": f"Bearer {token}"} if token else {})})
    try:
        with urllib.request.urlopen(req, timeout=20) as r:
            return json.load(r)
    except urllib.error.HTTPError as e:
        return {"__status": e.code, **(json.loads(e.read() or b"{}") or {})}
    except urllib.error.URLError as e:
        raise SystemExit(f"cannot reach the API at {API} ({e.reason}). Start it with "
                         f"./start.sh, or point API_URL at it.")


ROUTE_RE = re.compile(r'path="/([a-z0-9-]+)[a-z0-9/*-]*"\s+element={(.+?)}\s*/>', re.S)
ADMIN_PAGE = "__admin__"  # any administrator, rather than one named role


def page_roles() -> dict[str, str]:
    """Which role each hand-built page belongs to, read from the app's own routes.

    The first segment of `AppRoleProfiles.HomeRoute` is the domain role, so this stays
    true when a page is re-pointed at a different role — no second list to maintain. A
    route gated on `is_admin` belongs to no single role; it is opened as every
    administrator instead.
    """
    by_page: dict[str, str] = {}
    for path, element in ROUTE_RE.findall((SRC / "App.tsx").read_text()):
        for page in re.findall(r"<([A-Z][A-Za-z]*)\s*/>", element):
            if page in ("Navigate", "Loading"):
                continue
            by_page[page] = ADMIN_PAGE if "is_admin" in element else path
    if not by_page:
        raise SystemExit(f"{(SRC / 'App.tsx').relative_to(ROOT)}: no routes matched — this "
                         f"scanner cannot tell which role each page belongs to any more.")
    return by_page


def live(every: list[dict]) -> int:
    """Open every explainer's popup through the API, as the role whose page it is on."""
    by_page = page_roles()
    sign_ins = api("/api/auth/sign-ins")["signIns"]
    who: dict[str, list[dict]] = defaultdict(list)
    for s in sign_ins:
        who[s["domainRole"]].append(s)
    admins = [s for s in sign_ins if s["isAdministrator"]]

    by_role: dict[str, set[tuple[str, str]]] = defaultdict(set)
    for e in every:
        page = Path(e["file"]).stem
        role = by_page.get(page)
        if role is None:
            # RoleHome (path="*") names its columns at runtime and contributes no static
            # pair, so a page here is one this scanner cannot attribute to anyone.
            raise SystemExit(f"{e['file']}: no route in App.tsx renders this page, so there "
                             f"is no role to open its {len(every)} explainers as.")
        by_role[role].add((e["table"], e["column"]))

    # Each page is opened by the people who actually reach it: one holder per role, and
    # every administrator for the admin console, whose grants differ person to person.
    runs: list[tuple[dict, set[tuple[str, str]]]] = []
    bad: list[str] = []
    for role, pairs in sorted(by_role.items()):
        if role == ADMIN_PAGE:
            if not admins:
                bad.append("the admin console is routed but no AppUsers row is an "
                           "administrator — nobody can open it at all.")
            runs += [(a, pairs) for a in admins]
        elif who.get(role):
            runs.append((who[role][0], pairs))
        else:
            bad.append(f"{role}: no one in AppUsers may act as this role, but a page is "
                       f"routed to it — nobody can open that page at all.")

    # A role schema that is momentarily absent — the database being reloaded under us —
    # makes every column invisible, and would otherwise be reported as hundreds of
    # separate broken explainers. It is one fact about the database, so say that instead.
    populated = {s for (s,) in psql(
        "SELECT table_schema FROM information_schema.columns "
        "WHERE table_schema LIKE 'pko\\_%' GROUP BY table_schema")}
    for person, _ in runs:
        if person["schemaName"] not in populated:
            raise SystemExit(
                f"{person['displayName']}'s role schema {person['schemaName']} has no columns "
                f"in {DB}. The database is mid-reload or the role views were never emitted; "
                f"every popup would read as broken. Load it (bash init-db.sh) and re-run.")

    print(f"\nopening popups through {API}")
    for person, pairs in runs:
        token = api("/api/auth/sign-in", body={
            "appUserId": person["appUserId"], "principalId": person["principalId"]})["token"]
        fails = []
        for table, column in sorted(pairs):
            got = api(f"/api/app/provenance/{table}/{column}", token)
            if got.get("__status"):
                fails.append(f"    as {person['displayName']}: {table}.{column}  "
                             f"{got.get('error')}: {got.get('detail')}")
            elif not got.get("field", {}).get("field_name"):
                fails.append(f"    as {person['displayName']}: {table}.{column}  "
                             f"answered with no field")
        mark = "ok  " if not fails else "FAIL"
        print(f"  {mark} {person['displayName']:<18} {person['domainRole']:<26} "
              f"{len(pairs) - len(fails)}/{len(pairs)} popups")
        bad += fails
    if bad:
        print(f"\n{len(bad)} popup(s) do not open:", file=sys.stderr)
        for b in bad:
            print(b, file=sys.stderr)
        return 1
    return 0


def main() -> int:
    report = "--report" in sys.argv
    views, by_col = load_catalog()

    every: list[dict] = []
    dynamic = 0
    for path in sorted(SRC.rglob("*.tsx")):
        found, dyn, _raw = scan(path)
        every += found
        dynamic += dyn

    broken = []
    for e in every:
        if e["table"] not in views:
            broken.append((e, f"no view vw_{e['table']} — the rulebook has no such table, "
                              f"or the database is stale"))
        elif e["column"] not in views[e["table"]]:
            broken.append((e, f"vw_{e['table']} has no column {e['column']}"))
        elif (e["table"], e["column"]) not in by_col:
            broken.append((e, f"vw_{e['table']}.{e['column']} exists but no RulebookFields row "
                              f"claims it — run tools/reconcile_field_catalog.py"))

    pairs = {(e["table"], e["column"]) for e in every}
    cat = [by_col[p] for p in pairs if p in by_col]
    derived = sum(1 for c in cat if c["derived"])
    witnesses = sum(1 for c in cat if c["witness"])
    questions = {c["question"] for c in cat if c["question"]}

    print(f"explainers        {len(every)} in {len({e['file'] for e in every})} files"
          f" + {dynamic} named at runtime")
    print(f"distinct fields   {len(pairs)} across {len({t for t, _ in pairs})} tables")
    print(f"  worked out      {derived}")
    print(f"  witnesses       {witnesses}, answering {len(questions)} role questions")

    if report:
        print("\nper file")
        per = defaultdict(list)
        for e in every:
            per[e["file"]].append(e)
        for f in sorted(per, key=lambda x: -len(per[x])):
            ts = {e["table"] for e in per[f]}
            print(f"  {len(per[f]):4}  {len(ts):3} tables  {f}")
        print("\nper table")
        pt = defaultdict(set)
        for t, c in pairs:
            pt[t].add(c)
        for t in sorted(pt, key=lambda x: -len(pt[x])):
            d = sum(1 for c in pt[t] if by_col.get((t, c), {}).get("derived"))
            print(f"  {len(pt[t]):4}  {d:3} worked out  {t}")

    if broken:
        print(f"\n{len(broken)} explainer(s) cannot be answered:", file=sys.stderr)
        for e, why in broken:
            print(f"  {e['file']}:{e['line']}  <{e['component']} t=\"{e['table']}\" "
                  f"f=\"{e['column']}\">  {why}", file=sys.stderr)
        return 1
    print("\nevery explainer resolves to a catalogued field.")

    if "--live" in sys.argv:
        return live(every)
    return 0


def fail(msg: str) -> int:
    print(f"FAIL: {msg}", file=sys.stderr)
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
