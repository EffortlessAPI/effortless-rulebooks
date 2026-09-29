"""NEGATIVE CONTROL: the interpreter encoding (paper sections 6 and 13).

The rulebook is loaded as inert JSON and handed to a universal evaluator
(the SQLite oracle compiles and runs it at request time).  This substrate
passes the answer key: its traces lie inside the expected trace sets.  It
fails the reflection check: there is no component in this file that
corresponds to any constructor of the source, so conditions 1, 2, 3, 5
and 6 of section 13 fail.  Same computation as a transparent substrate,
different artifact -- which is the whole point.
"""
from __future__ import annotations

import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "..", "tools"))
import engine  # noqa: E402
import sqlite_oracle  # noqa: E402


def evaluate(spec, hist, now, cmd):
    """One universal evaluator for every rule of every spec."""
    return sqlite_oracle.derived_state(spec, hist, now, cmd)


def run(spec_path, inputs, choose="first"):
    with open(spec_path) as f:
        program = json.load(f)                  # D = the program, as data
    return engine.run(engine.SpecModel(program, evaluate), inputs, choose=choose)
