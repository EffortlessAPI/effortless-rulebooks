"""Conformance runner: produces conformance/matrix.json from scratch.

Two gates per substrate (paper sections 11 and 13):

  answer_key   every trace the substrate produces is in Obs_R0(c, i, h)
               (inclusion, not equality: paper section 20)
  reflection   the artifact satisfies constructor reflection

The matrix is regenerated, never edited: run this file.
"""
from __future__ import annotations

import glob
import json
import os
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.join(HERE, "..")
sys.path.insert(0, os.path.join(ROOT, "tools"))
sys.path.insert(0, os.path.join(ROOT, "substrates", "transparent"))
sys.path.insert(0, os.path.join(ROOT, "substrates", "interpreter"))
import engine  # noqa: E402
import r0lang as L  # noqa: E402
import reflection_check  # noqa: E402
import r0_lint  # noqa: E402
import runtime as transparent_rt  # noqa: E402
import interpreter  # noqa: E402

SPEC = os.path.join(ROOT, "r0", "spec.json")
INPUTS = sorted(glob.glob(os.path.join(ROOT, "r0", "inputs", "*.json")))
EXPECTED = os.path.join(ROOT, "r0", "expected")
TRANSPARENT = os.path.join(ROOT, "substrates", "transparent", "generated_warehouse_network.py")
TANGLED = os.path.join(ROOT, "substrates", "tangled", "generated_warehouse_network_tangled.py")
INTERPRETER = os.path.join(ROOT, "substrates", "interpreter", "interpreter.py")


def build():
    subprocess.run([sys.executable, os.path.join(ROOT, "substrates", "transparent", "transpile.py"), SPEC, TRANSPARENT], check=True)
    subprocess.run([sys.executable, os.path.join(ROOT, "substrates", "tangled", "make_tangled.py"), TRANSPARENT, TANGLED], check=True)
    subprocess.run([sys.executable, os.path.join(ROOT, "tools", "gen_expected.py"), SPEC, os.path.join(ROOT, "r0", "inputs"), EXPECTED], check=True)


def substrates():
    return {
        "transparent": {"artifact": TRANSPARENT, "run": lambda inp: transparent_rt.run(TRANSPARENT, inp, choose="first")},
        "tangled": {"artifact": TANGLED, "run": lambda inp: transparent_rt.run(TANGLED, inp, choose="first")},
        "interpreter": {"artifact": INTERPRETER, "run": lambda inp: interpreter.run(SPEC, inp, choose="first")},
    }


def main():
    build()
    spec = L.load(SPEC)
    matrix = {"spec": spec["name"], "lint": r0_lint.lint(spec)["ok"], "substrates": {}}
    for name, sub in substrates().items():
        row = {"answer_key": {}, "reflection": None}
        for path in INPUTS:
            with open(path) as f:
                inp = json.load(f)
            with open(os.path.join(EXPECTED, os.path.basename(path))) as f:
                exp = engine.trace_set(json.load(f))
            got = engine.trace_set(sub["run"](inp))
            row["answer_key"][inp["name"]] = {"traces_produced": len(got), "permitted": len(exp), "ok": got <= exp}
        rep = reflection_check.check(spec, sub["artifact"])
        row["reflection"] = {"ok": rep["ok"], "recoverable_G_R": rep["recoverable_G_R"],
                             "conditions": {k: v["ok"] for k, v in rep["conditions"].items()}}
        row["answer_key_ok"] = all(v["ok"] for v in row["answer_key"].values())
        row["conformant"] = row["answer_key_ok"] and rep["ok"]
        matrix["substrates"][name] = row
    with open(os.path.join(HERE, "matrix.json"), "w") as f:
        json.dump(matrix, f, indent=1)
    print(f"{'substrate':12} {'answer key':11} {'reflection':11} conformant")
    for name, row in matrix["substrates"].items():
        print(f"{name:12} {str(row['answer_key_ok']):11} {str(row['reflection']['ok']):11} {row['conformant']}")


if __name__ == "__main__":
    main()
