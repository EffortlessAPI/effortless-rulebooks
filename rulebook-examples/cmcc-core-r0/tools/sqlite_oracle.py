"""SQLite oracle: a second evaluator that shares no evaluation code with
reference_eval.py.

Each rule is compiled to one SQL statement; SQLite's join engine computes
the stratum, and recursive strata are iterated to a fixed point by
re-running their INSERT statements until no row is added.  Agreement
between this evaluator and the reference evaluator on every derived state
visited during a run is what the conformance suite calls an "independent
oracle check": two different mechanisms, one set of declared semantics.

Deliberately also used as the negative-control substrate (substrates/
interpreter): the SQL is generated and executed at run time and never
materialised as a component-bearing artifact, so there is nothing for the
reflection check to inspect.
"""
from __future__ import annotations

import sqlite3

import r0lang as L


def _lit(v):
    if isinstance(v, str):
        return "'" + v.replace("'", "''") + "'"
    return str(int(v))


class _Ctx:
    def __init__(self, counter):
        self.counter = counter
        self.frm = []
        self.where = []
        self.var = {}

    def alias(self):
        self.counter[0] += 1
        return f"t{self.counter[0]}"


def _expr(e, ctx):
    if L.is_var(e):
        return ctx.var[e]
    if isinstance(e, list):
        op, x, y = e
        return f"({_expr(x, ctx)} {op} {_expr(y, ctx)})"
    return _lit(L.const_value(e))


def _sqlop(op):
    return {"=": "=", "!=": "<>", "<": "<", "<=": "<=", ">": ">", ">=": ">="}[op]


def _compile_body(items, ctx):
    for it in items:
        if "atom" in it:
            q, *args = it["atom"]
            a = ctx.alias()
            ctx.frm.append(f'"{q}" {a}')
            for j, arg in enumerate(args):
                col = f"{a}.c{j}"
                if L.is_anon(arg):
                    continue
                if L.is_var(arg):
                    if arg in ctx.var:
                        ctx.where.append(f"{col} = {ctx.var[arg]}")
                    else:
                        ctx.var[arg] = col
                else:
                    ctx.where.append(f"{col} = {_lit(L.const_value(arg))}")
        elif "not" in it:
            q, *args = it["not"]
            sub = []
            for j, arg in enumerate(args):
                if L.is_anon(arg):
                    continue
                sub.append(f"q.c{j} = {_expr(arg, ctx)}")
            ctx.where.append(
                f'NOT EXISTS (SELECT 1 FROM "{q}" q WHERE {" AND ".join(sub) or "1"})')
        elif "cmp" in it:
            op, e1, e2 = it["cmp"]
            ctx.where.append(f"({_expr(e1, ctx)} {_sqlop(op)} {_expr(e2, ctx)})")
        elif "let" in it:
            v, e = it["let"]
            ctx.var[v] = _expr(e, ctx)


def _select(from_, where):
    s = ""
    if from_:
        s += " FROM " + ", ".join(from_)
    if where:
        s += " WHERE " + " AND ".join(where)
    return s


def compile_rule(rule) -> str:
    head, *hargs = rule["head"]
    counter = [0]
    if "agg" not in rule:
        ctx = _Ctx(counter)
        _compile_body(rule["body"], ctx)
        sel = ", ".join(_expr(a, ctx) for a in hargs)
        return f'INSERT OR IGNORE INTO "{head}" SELECT DISTINCT {sel}{_select(ctx.frm, ctx.where)}'
    a = rule["agg"]
    op = a["op"]
    gvars = hargs[:-1]
    if "keys" in a:
        kctx = _Ctx(counter)
        _compile_body(a["keys"], kctx)
        octx = _Ctx(counter)
        octx.var = dict(kctx.var)             # correlate shared variables
        _compile_body(a["over"], octx)
        sub = f"(SELECT {op}(DISTINCT {_expr(a['expr'], octx)}){_select(octx.frm, octx.where)})"
        where = list(kctx.where)
        if op in ("MIN", "MAX"):
            where.append(f"{sub} IS NOT NULL")
        sel = ", ".join([_expr(g, kctx) for g in gvars] + [sub])
        return f'INSERT OR IGNORE INTO "{head}" SELECT DISTINCT {sel}{_select(kctx.frm, where)}'
    octx = _Ctx(counter)
    _compile_body(a["over"], octx)
    gsel = [_expr(g, octx) for g in gvars]
    sel = ", ".join(gsel + [f"{op}(DISTINCT {_expr(a['expr'], octx)})"])
    grp = (" GROUP BY " + ", ".join(gsel)) if gsel else ""
    return f'INSERT OR IGNORE INTO "{head}" SELECT {sel}{_select(octx.frm, octx.where)}{grp}'


def _create(cur, name, arity):
    cols = ", ".join(f"c{i}" for i in range(arity))
    cur.execute(f'CREATE TABLE "{name}" ({cols}, PRIMARY KEY ({cols}))')


def derived_state(spec, hist, now, cmd) -> dict:
    con = sqlite3.connect(":memory:")
    cur = con.cursor()
    ar = L.arities(spec)
    for name, n in ar.items():
        _create(cur, name, n)
    for (n, op, rel, cols, vf, vt, k) in hist:
        row = (n, op) + tuple(cols) + (vf, vt, k)
        cur.execute(f'INSERT OR IGNORE INTO "Hist_{rel}" VALUES ({",".join("?" * len(row))})', row)
    cur.execute('INSERT INTO "Now" VALUES (?, ?)', tuple(now))
    if cmd is not None:
        ps = tuple(cmd[1])
        cur.execute(f'INSERT INTO "Cmd_{cmd[0]}" VALUES ({",".join("?" * len(ps))})', ps)
    st = L.stratify(spec)
    for s in st["strata"]:
        sqls = [compile_rule(r) for r in s["rules"]]
        if not s["recursive"]:
            for q in sqls:
                cur.execute(q)
            continue
        while True:
            added = 0
            for q in sqls:
                cur.execute(q)
                added += cur.execute("SELECT changes()").fetchone()[0]
            if added == 0:
                break
    out = {}
    for name in ar:
        out[name] = {tuple(r) for r in cur.execute(f'SELECT * FROM "{name}"')}
    con.close()
    return out
