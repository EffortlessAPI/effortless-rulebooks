#!/usr/bin/env python3
"""Execute the TestCases rows against the substrate and record what it said.

The substrate is the oracle. Every verdict here comes from Postgres or from the
generated SQL — nothing is recomputed in Python to decide whether a formula is
right. The runner's only job is to ask the question the row states and write
down the answer.

Outcomes:
  PASS  the assertion holds
  WARN  advisory: the model cannot state something it arguably should be able
        to (a witness that cannot fire on this seed). Not a failure — forcing
        these red would create pressure to fabricate data.
  FAIL  the assertion does not hold. A blocking FAIL means the model is
        asserting something false.
  SKIP  the check could not run (its target does not exist in the substrate)

Usage:
    tools/run_test_suite.py [--write] [--kind KIND] [--json]
"""
from __future__ import annotations

import argparse
import json
import re
import subprocess
import sys
from collections import OrderedDict
from datetime import datetime, timezone
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
FUNCS = Path("postgres-bootstrap/02-create-functions.sql")
DB = "erb_procedural_knowledge_ontology"


def snake_candidates(name: str) -> list[str]:
    """Every plausible snake_case rendering of a PascalCase name.

    Guessing the transpiler's exact casing rule cost three rounds of wrong
    answers: splitting on every capital turns FAQs into f_a_qs, and NOT
    splitting turns HasLostAHolder into has_lost_aholder. Both renderings are
    correct for some names and wrong for others.

    Rather than keep guessing, generate the candidates and let the substrate
    say which one it actually emitted. A check whose target cannot be resolved
    is not evidence of anything, so this resolution has to be right.
    """
    keep_runs = re.sub(r"([a-z0-9])([A-Z])", r"\1_\2", name).lower()
    split_runs = re.sub(
        r"([a-z0-9])([A-Z])", r"\1_\2",
        re.sub(r"([A-Z]+)([A-Z][a-z])", r"\1_\2", name)).lower()
    every_capital = re.sub(r"(?<!^)(?=[A-Z])", "_", name).lower()
    seen, out = set(), []
    for c in (keep_runs, split_runs, every_capital):
        if c not in seen:
            seen.add(c)
            out.append(c)
    return out


def snake(name: str) -> str:
    """The most common rendering — used only where the target is then verified."""
    return snake_candidates(name)[0]


def resolve_view(table: str, views: set[str]) -> str | None:
    for c in snake_candidates(table):
        if f"vw_{c}" in views:
            return f"vw_{c}"
    return None


def resolve_column(view: str, field: str, columns: set[str]) -> str | None:
    for c in snake_candidates(field):
        if f"{view}.{c}" in columns:
            return c
    return None


def pk_of(table: str, rb=None) -> str:
    """The primary-key field name for a table.

    Read from the rulebook when available: FAQs keys on FaqId, not FAQId, and
    deriving the name from the table name produced a check that silently could
    not run. The schema is the authority on its own key.
    """
    if rb is not None and table in rb:
        sch = rb[table].get("schema", [])
        if sch and isinstance(sch[0], dict):
            return sch[0]["name"]
    return f"{singular(table)}Id"


def singular(table: str) -> str:
    """Naive plural -> singular, matching how the rulebook names its keys.

    CommunitiesOfPractice keys on CommunityOfPracticeId, so trimming the
    trailing character is not enough.
    """
    special = {
        "CommunitiesOfPractice": "CommunityOfPractice",
        "FAQs": "FAQ",
    }
    if table in special:
        return special[table]
    if table.endswith("ies"):
        return table[:-3] + "y"
    if table.endswith("s") and not table.endswith("ss"):
        return table[:-1]
    return table


class Substrate:
    """One connection's worth of facts, read once and reused."""

    def __init__(self) -> None:
        self.views = set(self._q(
            "SELECT table_name FROM information_schema.views WHERE table_schema='public'"))
        self.columns = set(self._q(
            "SELECT table_name || '.' || column_name FROM information_schema.columns "
            "WHERE table_schema='public'"))
        self.failed_translations = self._failed_translations()

    @staticmethod
    def _q(sql: str) -> list[str]:
        out = subprocess.run(["psql", "-qtA", "-d", DB, "-c", sql],
                             capture_output=True, text=True)
        if out.returncode != 0:
            raise SystemExit(f"psql failed:\n{out.stderr}")
        return [l for l in out.stdout.strip().splitlines() if l]

    @staticmethod
    def _failed_translations() -> set[str]:
        """Function names the transpiler could not translate.

        These do not fail the build — the generated body carries a warning
        comment and returns NULL, so the column reads as a clean false forever.
        """
        if not FUNCS.exists():
            return set()
        text = FUNCS.read_text()
        bad = set()
        for block in text.split("CREATE OR REPLACE FUNCTION ")[1:]:
            name = block.split("(")[0].strip()
            body = block.split("$$")[1] if "$$" in block else ""
            if "Formula translation failed" in body:
                bad.add(name)
        return bad

    def scalar(self, sql: str) -> str | None:
        rows = self._q(sql)
        return rows[0] if rows else None

    def try_scalar(self, sql: str) -> tuple[bool, str]:
        out = subprocess.run(["psql", "-qtA", "-d", DB, "-c", sql],
                             capture_output=True, text=True)
        if out.returncode != 0:
            return False, out.stderr.strip().splitlines()[0] if out.stderr else "query failed"
        return True, out.stdout.strip()


def check(tc, rb, sub: Substrate) -> tuple[str, str]:
    """Return (outcome, detail) for one test case."""
    kind = tc["TestKind"]
    table, field = tc.get("TargetTable"), tc.get("TargetField")

    if kind == "structural":
        sch = rb.get(table, {}).get("schema", [])
        if not sch:
            return "SKIP", "table not found"
        names = [f["name"] for f in sch if isinstance(f, dict)]
        pk = pk_of(table, rb)
        problems = []
        if table not in {"__meta__"}:
            if names and names[0] != pk and pk in names:
                problems.append(f"primary key {pk} is not the first field")
            if pk in names and sch[names.index(pk)].get("nullable"):
                problems.append(f"{pk} is nullable")
            if "Name" not in names:
                problems.append("no calculated Name alias")
        return ("FAIL", "; ".join(problems)) if problems else ("PASS", f"{len(names)} fields")

    if kind == "formula-translates":
        fn = next((f"calc_{t}_{c}"
                   for t in snake_candidates(table) for c in snake_candidates(field)
                   if f"calc_{t}_{c}" in sub.failed_translations), None)
        if fn:
            return "FAIL", f"{fn} returns NULL — the transpiler could not translate this formula"
        return "PASS", "translated"

    if kind == "view-loads":
        view = resolve_view(table, sub.views)
        if not view:
            # ERB bookkeeping tables are deliberately not projected to views.
            if table in {"ERBVersions", "ERBCustomizations"}:
                return "SKIP", "bookkeeping table, intentionally not a view"
            return "FAIL", f"no view found for {table}"
        ok, out = sub.try_scalar(f"SELECT count(*) FROM {view}")
        return ("PASS", f"{out} rows") if ok else ("FAIL", out)

    if kind == "fk-resolves":
        target = tc["Subject"].split("->")[-1].strip()
        child, parent = resolve_view(table, sub.views), resolve_view(target, sub.views)
        if not child or not parent:
            return "FAIL", f"no view found for {table if not child else target}"
        col = resolve_column(child, field, sub.columns)
        pk = resolve_column(parent, pk_of(target, rb), sub.columns)
        if not col or not pk:
            return "FAIL", f"cannot resolve {'child column' if not col else 'parent key'}"
        ok, out = sub.try_scalar(
            f"SELECT count(*) FROM {child} c WHERE c.{col} IS NOT NULL AND c.{col} <> '' "
            f"AND NOT EXISTS (SELECT 1 FROM {parent} p WHERE p.{pk} = c.{col})")
        if not ok:
            return "FAIL", out
        n = int(out or 0)
        return ("PASS", "all values resolve") if n == 0 else ("FAIL", f"{n} dangling value(s)")

    if kind == "witness-discriminates":
        view = resolve_view(table, sub.views)
        col = resolve_column(view, field, sub.columns) if view else None
        if not col:
            return "FAIL", f"{table}.{field} has no column in the substrate"
        ok, out = sub.try_scalar(
            f"SELECT count(*) FILTER (WHERE {col}), count(*) FILTER (WHERE NOT {col}), count(*) "
            f"FROM {view}")
        if not ok:
            return "FAIL", out
        t, f_, total = (int(x) for x in out.split("|"))
        if total == 0:
            return "WARN", "no rows to witness"
        if t == 0:
            return "WARN", f"never true ({f_}/{total} false)"
        if f_ == 0:
            return "WARN", f"always true ({t}/{total})"
        return "PASS", f"{t} true / {f_} false"

    if kind == "question-answered":
        qid = tc["Subject"]
        n = sum(1 for r in rb["RulebookFields"]["data"]
                if r.get("InventedForQuestion") == qid)
        return ("PASS", f"{n} predicates") if n else ("FAIL", "no predicate answers this question")

    if kind == "provenance":
        qids = {q["RoleQuestionId"] for q in rb["RoleQuestions"]["data"]}
        dangling = [r["RulebookFieldId"] for r in rb["RulebookFields"]["data"]
                    if r.get("InventedForQuestion") and r["InventedForQuestion"] not in qids]
        return ("PASS", "all resolve") if not dangling else ("FAIL", f"{len(dangling)} dangling")

    if kind == "catalog-sync":
        catalogued = {r["RulebookFieldId"] for r in rb["RulebookFields"]["data"]}
        actual = {f"{t}.{f['name']}"
                  for t, v in rb.items() if isinstance(v, dict) and "schema" in v
                  for f in v["schema"] if isinstance(f, dict)}
        missing, stale = actual - catalogued, catalogued - actual
        if missing or stale:
            return "FAIL", f"{len(missing)} missing, {len(stale)} stale"
        return "PASS", f"{len(actual)} fields in sync"

    if kind == "invariant":
        return invariant(tc, rb, sub)

    return "SKIP", f"unknown kind {kind}"


def invariant(tc, rb, sub: Substrate) -> tuple[str, str]:
    tid = tc["TestCaseId"]

    if tid == "tc-no-delimiter-collision":
        offenders = []
        for t, v in rb.items():
            if not isinstance(v, dict) or "schema" not in v:
                continue
            ids = [f["name"] for f in v["schema"]
                   if isinstance(f, dict) and f.get("datatype") == "string"
                   and (f["name"].endswith("Id") or f.get("RelatedTo"))]
            for row in v.get("data", []):
                for name in ids:
                    val = row.get(name)
                    if isinstance(val, str) and "|" in val:
                        offenders.append(f"{t}.{name}")
        return ("PASS", "no identifier contains '|'") if not offenders \
            else ("FAIL", f"{len(offenders)} identifier(s) contain the delimiter")

    if tid == "tc-inv-spec-exec-separate":
        if "Procedures" in rb and "ProcedureExecutions" in rb:
            return "PASS", "specification and execution are separate tables"
        return "FAIL", "spec/execution separation is broken"

    if tid == "tc-inv-asof-not-wallclock":
        offenders = []
        for t, v in rb.items():
            if not isinstance(v, dict) or "schema" not in v:
                continue
            for f in v["schema"]:
                if isinstance(f, dict) and re.search(r"\bNOW\s*\(\s*\)", str(f.get("formula", ""))):
                    offenders.append(f"{t}.{f['name']}")
        if offenders:
            return "FAIL", f"{len(offenders)} formula(s) still read wall-clock: {', '.join(offenders[:4])}"
        return "PASS", "all time witnesses read the modeled instant"

    if tid == "tc-inv-remediation-preserved":
        resolved = [g for g in rb.get("KnowledgeGaps", {}).get("data", [])
                    if g.get("Status") == "Resolved"]
        return ("PASS", f"{len(resolved)} resolved gaps retained as evidence") if resolved \
            else ("FAIL", "no remediation record survives")

    # The remaining invariants assert a specific witness is computable and can fire.
    table, field = tc.get("TargetTable"), tc.get("TargetField")
    if table and field:
        view = resolve_view(table, sub.views)
        col = resolve_column(view, field, sub.columns) if view else None
        if not col:
            return "FAIL", f"{table}.{field} is not computed in the substrate"
        ok, out = sub.try_scalar(f"SELECT count(*) FILTER (WHERE {col} IS NOT NULL) FROM {view}")
        if not ok:
            return "FAIL", out
        return ("PASS", f"computed on {out} rows") if int(out or 0) else \
            ("WARN", "column exists but is null everywhere")
    return "SKIP", "no target"


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--write", action="store_true", help="record outcomes back into the rulebook")
    ap.add_argument("--kind", help="run only one TestKind")
    ap.add_argument("--json", action="store_true", help="emit results as JSON")
    args = ap.parse_args()

    with RB.open() as fh:
        rb = json.load(fh, object_pairs_hook=OrderedDict)

    sub = Substrate()
    now = datetime.now(timezone.utc).isoformat(timespec="seconds")
    tally: dict[str, int] = {}
    blocking_fails: list[OrderedDict] = []

    for tc in rb["TestCases"]["data"]:
        if args.kind and tc["TestKind"] != args.kind:
            continue
        outcome, detail = check(tc, rb, sub)
        tc["LastOutcome"] = outcome
        tc["LastDetail"] = detail
        tc["LastRunAt"] = now
        tally[outcome] = tally.get(outcome, 0) + 1
        if outcome == "FAIL" and tc.get("Severity") == "blocking":
            blocking_fails.append(tc)

    if args.write:
        with RB.open("w") as fh:
            json.dump(rb, fh, indent=2)
            fh.write("\n")

    if args.json:
        print(json.dumps({"tally": tally,
                          "blockingFails": [dict(t) for t in blocking_fails]}, indent=1))
        return 1 if blocking_fails else 0

    total = sum(tally.values())
    print(f"ran {total} checks")
    for k in ("PASS", "WARN", "FAIL", "SKIP"):
        if tally.get(k):
            print(f"  {tally[k]:5}  {k}")
    if blocking_fails:
        print(f"\n{len(blocking_fails)} BLOCKING failure(s):")
        for t in blocking_fails[:25]:
            print(f"  {t['TestCaseId']}\n      {t['LastDetail']}")
        if len(blocking_fails) > 25:
            print(f"  ... and {len(blocking_fails) - 25} more")
        return 1
    print("\nBOARD IS GREEN — no blocking failure.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
