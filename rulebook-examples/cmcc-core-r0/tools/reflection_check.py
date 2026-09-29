"""reflection-check: does a substrate artifact satisfy constructor
reflection (paper section 13) with respect to its R0 source?

  1  constructor preservation   every G_R node has a function of its kind
  2  dependency preservation    every source edge (u,v) is a call v -> u
  3  addressability             every G_R node has an image
  4  local adequacy             NOT checked here: that is the answer key
  5  injectivity                distinct nodes, distinct functions
  6  bounded locality           every public function is the image of one
                               node; no call edge between images that is
                               not the image of a source edge; private
                               helpers (leading underscore) never call
                               an image function; no function resolves
                               callees by name (globals/getattr/eval)

With 2 and 6 both passing, the call graph restricted to images is
isomorphic to G_R: the source constructor graph is recoverable from the
artifact (section 13, after the list).

Usage:  python reflection_check.py spec.json artifact.py
"""
from __future__ import annotations

import ast
import json
import sys

import r0lang as L

PREFIX = {"pred": "c_pred_", "base": "c_pred_", "rule": "c_rule_", "agg": "c_rule_",
          "mu": "c_mu_", "transition": "c_t_", "constraint": "c_constraint_", "obs": "c_obs_"}
NODE_PREFIX = {"pred:": "c_pred_", "rule:": "c_rule_", "mu:": "c_mu_", "t:": "c_t_",
               "constraint:": "c_constraint_", "obs:": "c_obs_"}


def image_name(node_id):
    for p, f in NODE_PREFIX.items():
        if node_id.startswith(p):
            return f + node_id[len(p):]
    raise ValueError(node_id)


def call_graph(path):
    """Top-level functions and the direct calls between them."""
    tree = ast.parse(open(path).read())
    funcs = {n.name: n for n in tree.body if isinstance(n, ast.FunctionDef)}
    calls = {}
    for name, fn in funcs.items():
        out = set()
        for node in ast.walk(fn):
            if isinstance(node, ast.Call) and isinstance(node.func, ast.Name) and node.func.id in funcs:
                out.add(node.func.id)
        calls[name] = out
    return funcs, calls


DYNAMIC = {"globals", "getattr", "eval", "exec", "__import__", "locals", "vars"}


def _uses_dynamic_dispatch(fn) -> bool:
    """A function that resolves callees by name at run time is an evaluator
    in miniature: its dependencies are data, not edges."""
    for node in ast.walk(fn):
        if isinstance(node, ast.Call) and isinstance(node.func, ast.Name) and node.func.id in DYNAMIC:
            return True
    return False


def check(spec, artifact_path) -> dict:
    G = L.constructor_graph(spec)
    funcs, calls = call_graph(artifact_path)
    report = {"artifact": artifact_path, "conditions": {}, "ok": True}
    phi = {n: image_name(n) for n in G["nodes"]}

    # 1 + 3: image exists and has the right kind prefix
    missing = [n for n, f in phi.items() if f not in funcs]
    wrong_kind = [n for n, f in phi.items() if f in funcs and not f.startswith(PREFIX[G["nodes"][n]])]
    report["conditions"]["1_constructor_preservation"] = {"ok": not missing and not wrong_kind,
                                                          "missing": missing[:10], "wrong_kind": wrong_kind[:10]}
    report["conditions"]["3_addressability"] = {"ok": not missing, "unaddressed": len(missing)}

    # 2: every source edge (u,v) is a call v -> u
    lost = [(u, v) for (u, v) in G["edges"] if phi[v] not in funcs or phi[u] not in calls.get(phi[v], set())]
    report["conditions"]["2_dependency_preservation"] = {"ok": not lost, "lost_edges": lost[:10], "checked": len(G["edges"])}

    # 4: delegated
    report["conditions"]["4_local_adequacy"] = {"ok": None, "note": "checked by the conformance gate (answer key), not structurally"}

    # 5: injectivity
    images = list(phi.values())
    report["conditions"]["5_injectivity"] = {"ok": len(images) == len(set(images))}

    # 6: bounded locality
    image_set = set(images)
    public = [f for f in funcs if not f.startswith("_")]
    extra_public = [f for f in public if f not in image_set]
    helpers_calling_images = [f for f in funcs if f.startswith("_") and calls[f] & image_set]
    dynamic = [f for f, fn in funcs.items() if _uses_dynamic_dispatch(fn)]
    source_edges = {(phi[u], phi[v]) for (u, v) in G["edges"]}
    extra_edges = [(caller, callee) for caller, cs in calls.items() if caller in image_set
                   for callee in cs if callee in image_set and (callee, caller) not in source_edges]
    ok6 = not extra_public and not helpers_calling_images and not extra_edges and not dynamic
    report["conditions"]["6_bounded_locality"] = {"ok": ok6, "public_non_image_functions": extra_public[:10],
                                                  "helpers_calling_images": helpers_calling_images[:10],
                                                  "extra_edges_between_images": extra_edges[:10],
                                                  "dynamic_dispatch": dynamic[:10]}
    iso = report["conditions"]["2_dependency_preservation"]["ok"] and ok6 and not missing
    report["recoverable_G_R"] = iso
    report["summary"] = {"source_nodes": len(G["nodes"]), "source_edges": len(G["edges"]),
                         "artifact_functions": len(funcs), "image_functions": len(image_set & set(funcs))}
    report["ok"] = all(c["ok"] for c in report["conditions"].values() if c["ok"] is not None)
    return report


def main(argv):
    spec = L.load(argv[1])
    rep = check(spec, argv[2])
    print(json.dumps(rep, indent=2))
    return 0 if rep["ok"] else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv))
