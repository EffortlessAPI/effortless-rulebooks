"""Generate r0/expected/<input>.json: the trace SET Obs_R0(c, i, h).

The set is produced by the reference evaluator and every derived state it
visits is recomputed by the SQLite oracle; any disagreement aborts.  So an
expected file is never hand-authored and never the product of one
evaluation mechanism alone.

Usage:  python gen_expected.py spec.json inputs_dir expected_dir
"""
from __future__ import annotations

import glob
import json
import os
import sys

import engine
import r0lang as L
import reference_eval as R
import sqlite_oracle as O

SKIP = ("Hist_", "Cmd_", "Now")


def expected_for(spec, inputs) -> dict:
    visited = []
    model = engine.SpecModel(spec, R.derived_state,
                             observer=lambda h, now, cmd, D: visited.append((h, now, cmd, D)))
    result = engine.run(model, inputs)
    mismatches = 0
    for hist, now, cmd, D in visited:
        D2 = O.derived_state(spec, hist, now, cmd)
        for p, tuples in D.items():
            if p.startswith(SKIP[:2]) or p == "Now":
                continue
            if tuples != D2[p]:
                mismatches += 1
    if mismatches:
        raise SystemExit(f"reference/oracle disagreement on {mismatches} relations; not writing expected file")
    return {"input": inputs["name"], "states_cross_checked": len(visited),
            "trace_count": len(result["traces"]), "traces": result["traces"]}


def main(argv):
    spec = L.load(argv[1])
    os.makedirs(argv[3], exist_ok=True)
    for path in sorted(glob.glob(os.path.join(argv[2], "*.json"))):
        with open(path) as f:
            inputs = json.load(f)
        out = expected_for(spec, inputs)
        dst = os.path.join(argv[3], os.path.basename(path))
        with open(dst, "w") as f:
            json.dump(out, f, indent=1, sort_keys=True)
        print(f"{dst}: {out['trace_count']} trace(s), {out['states_cross_checked']} states cross-checked")


if __name__ == "__main__":
    main(sys.argv)
