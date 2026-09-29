"""Runtime for the transparent substrate: the CMCC-Core executor (HOW)
driving a generated module (WHAT).  Reads nothing from the spec JSON; every
domain question is answered by calling a c_ function of the module."""
from __future__ import annotations

import importlib.util
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "..", "tools"))
import engine  # noqa: E402


def load_module(path):
    spec = importlib.util.spec_from_file_location("generated_model", path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


class ModuleModel:
    def __init__(self, mod):
        self.mod = mod
        self.M = mod.MODEL
        self.transitions = [(t["name"], t["command"]) for t in self.M["transitions"]]

    def h0(self):
        return engine.history_from(self.M["h0"])

    def _state(self, hist, now, cmd):
        S = {f"Hist_{r}": set() for r in self.M["edb"]}
        for (n, op, rel, cols, vf, vt, k) in hist:
            S[f"Hist_{rel}"].add((n, op) + tuple(cols) + (vf, vt, k))
        S["Now"] = {tuple(now)}
        for c in self.M["commands"]:
            S[f"Cmd_{c}"] = set()
        if cmd is not None:
            S[f"Cmd_{cmd[0]}"] = {tuple(cmd[1])}
        return S

    def evaluate(self, hist, now, cmd):
        S = self._state(hist, now, cmd)
        mod, M = self.mod, self.M

        class Ev:
            def _t(self, name):
                return getattr(mod, f"c_t_{name}")(S)

            def guard(self, name):
                return self._t(name)["guard"]

            def effects(self, name):
                return self._t(name)["effects"]

            def events(self, name):
                return self._t(name)["events"]

            def violations(self):
                return [c for c in M["constraints"] if getattr(mod, f"c_constraint_{c}")(S)]

            def observations(self):
                return {n: getattr(mod, f"c_obs_{n}")(S) for n in M["observables"]}
        return Ev()


def run(module_path, inputs, choose="first"):
    return engine.run(ModuleModel(load_module(module_path)), inputs, choose=choose)
