#!/usr/bin/env python3
"""Conformance test for the bit-calculator gate engine.

Two independent checks, both driven only by rulebook data:

  1. Postgres invariant: every row of vw_computation_answer has value_ok = true,
     i.e. the gate netlist settled to the rulebook's expected answer. The four
     flagship rows (is_flagship=1) must be present and correct.

  2. Substrate equivalence: the Python netlist simulator (scripts/netlist_sim.py)
     and the Postgres engine compute the SAME result for every computation.

Neither the engine nor this test contains arithmetic beyond reading the settled
output bits back into an integer.

Run after the DB is initialized (`./start.sh db`, from the parent folder). Exit
code 0 = PASS.
"""
import json
import os
import re
import subprocess
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
BITCALC = HERE.parent
RULEBOOK = BITCALC / "effortless-rulebook" / "bit-calculator-rulebook.json"
DB = os.environ.get("BITCALC_DB", "erb_bit_calculator")
PGHOST = os.environ.get("PGHOST", "localhost")
PGUSER = os.environ.get("PGUSER", "postgres")

sys.path.insert(0, str(BITCALC / "scripts"))
from netlist_sim import Netlist  # noqa: E402

PG16 = "/opt/homebrew/opt/postgresql@16/bin"
if os.path.isdir(PG16):
    os.environ["PATH"] = PG16 + os.pathsep + os.environ["PATH"]


def psql(sql):
    r = subprocess.run(
        ["psql", "-h", PGHOST, "-U", PGUSER, "-d", DB, "-tAF,", "-c", sql],
        capture_output=True, text=True,
    )
    if r.returncode != 0:
        raise RuntimeError(f"psql failed: {r.stderr}")
    return [ln for ln in r.stdout.strip().splitlines() if ln and "NOTICE" not in ln]


def ab_inputs(a, b, w):
    d = {}
    for i in range(w):
        d[f"a{i}"] = (a >> i) & 1
        d[f"b{i}"] = (b >> i) & 1
    return d


def py_result(nl, row, w):
    a, b = int(row["a_bits"], 2), int(row["b_bits"], 2)
    out = nl.evaluate(row["component_id"], ab_inputs(a, b, w))
    prefix = {"add": "s", "sub": "s", "mul": "p", "div": "q"}[row["op"]]
    val = 0
    for port, bit in out.items():
        m = re.match(rf"^{prefix}(\d+)$", port)
        if m:
            val |= bit << int(m.group(1))
    return val


def main():
    rb = json.loads(RULEBOOK.read_text())
    width = int(next(m["StringValue"] for m in rb["__meta__"]["data"]
                     if m["MetaKey"] == "bit_width"))
    comps = rb["Computations"]["data"]
    nl = Netlist(str(RULEBOOK))

    failures = []

    # ---- check 1: Postgres invariant (value_ok) ----
    pg_rows = psql(
        "SELECT computation_id, op, result_value, expected_value, value_ok, is_flagship "
        "FROM vw_computation_answer")
    pg = {}
    for ln in pg_rows:
        cid, op, rv, ev, ok, flag = ln.split(",")
        pg[cid] = dict(op=op, result=int(rv), expected=int(ev),
                       ok=(ok == "t"), flagship=(flag == "1"))

    for cid, r in pg.items():
        if not r["ok"]:
            failures.append(f"[pg-invariant] {cid}: gate result {r['result']} != expected {r['expected']}")

    flagship_ids = {"compute--add--2--2", "compute--sub--4--2",
                    "compute--mul--2--2", "compute--div--4--2"}
    present_flagship = {cid for cid, r in pg.items() if r["flagship"]}
    missing = flagship_ids - present_flagship
    if missing:
        failures.append(f"[flagship] missing/mislabeled flagship computations: {sorted(missing)}")

    # ---- check 2: substrate equivalence (Python vs Postgres) ----
    agree = 0
    for row in comps:
        cid = row["computation_id"]
        pyv = py_result(nl, row, width)
        pgv = pg.get(cid, {}).get("result")
        if pgv is None:
            failures.append(f"[equiv] {cid}: present in rulebook but not in vw_computation_answer")
        elif pyv != pgv:
            failures.append(f"[equiv] {cid}: python {pyv} != postgres {pgv}")
        else:
            agree += 1

    total = len(comps)
    print(f"computations: {total}")
    print(f"postgres value_ok: {sum(1 for r in pg.values() if r['ok'])}/{len(pg)}")
    print(f"substrate agreement (python==postgres): {agree}/{total}")
    print(f"flagship present: {len(present_flagship & flagship_ids)}/4")

    if failures:
        print("\nFAIL:")
        for f in failures:
            print("  " + f)
        sys.exit(1)
    print("\nbit-calculator conformance: PASS")


if __name__ == "__main__":
    main()
