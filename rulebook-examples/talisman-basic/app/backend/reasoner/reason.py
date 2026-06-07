#!/usr/bin/env python3
"""
The computation engine for the app.

  stdin / argv[1]  : a JSON db (raw rows only — see abox_from_json.py)
  stdout           : a JSON document of EVERYTHING the reasoner derived

This is the only place computation happens. Express never computes a workflow
field; it calls this. The pipeline is identical to the substrate's owl/graph.py:

  1. Load the GENERATED TBox  (owl/src/ontology.owl)      — classes + properties
  2. Load the GENERATED SHACL (owl/src/rules.shacl.ttl)   — the rulebook formulas
  3. Build the ABox from the JSON db                       — the editable rows
  4. Run SHACL rules to a FIXPOINT                         — calc/lookup fields
  5. Run OWL-RL deductive closure                          — transitive/inverse

The .ttl files (TBox + SHACL rules) ARE the computation. The JSON only supplies
the raw individuals. So "JSON is the db, the ttl handles the computation" is
literally how this works: nothing in JSON is derived, nothing derived is in JSON.

Output shape:
{
  "reasoned_triples": <int>,
  "individuals": { "<ClassName>": [ {<all props incl. derived>}, ... ] },
  "competency": {
     "precedence_closure": {count, inferred, asserted, pairs:[...]},
     "delegation": { "<roleId>": [reachable...] },
     "disjoint_classes": [...],
     "roles_filled_by": { "<roleId>": {agent, agent_type} }
  }
}

We RAISE on any missing artifact or non-converging rule chain. There is no
empty-graph fallback — a silent empty answer would mask a broken build as a
passing one (CLAUDE.md: Avoid Silent Fallbacks).
"""
from __future__ import annotations

import json
import re
import sys
from collections import defaultdict
from pathlib import Path
from typing import Any, Dict, List

from rdflib import Graph, Literal, Namespace, URIRef
import owlrl
import pyshacl

# app/backend/reasoner/reason.py -> talisman-basic/  is parents[3]
PROJECT_ROOT = Path(__file__).resolve().parents[3]
OWL_SRC = PROJECT_ROOT / "owl" / "src"
TBOX = OWL_SRC / "ontology.owl"
SHACL = OWL_SRC / "rules.shacl.ttl"

ERB = Namespace("http://example.org/erb#")

sys.path.insert(0, str(Path(__file__).resolve().parent))
from abox_from_json import json_db_to_turtle, load_db  # noqa: E402


def _local(uri: Any) -> str:
    s = str(uri)
    return s.split("#", 1)[1] if "#" in s else s


def _shacl_construct_predicates(shapes: Graph) -> List[URIRef]:
    """The erb: predicates that SHACL rules CONSTRUCT (`$this erb:<prop> ?_result`),
    restricted to DATATYPE properties — the scalar calc/lookup/aggregation outputs
    (counts, booleans, derived strings). We deliberately EXCLUDE object properties:
    a SHACL-derived object edge like erb:precedesStep also carries the OWL-RL
    transitive CLOSURE we computed in stage 2, and clearing it before stage 3
    would throw the inferred pairs away. Datatype outputs have no such closure —
    they are safe to wipe and recompute. We read each rule's CONSTRUCT target out
    of the sh:construct text and keep those declared owl:DatatypeProperty in the
    TBox.
    """
    OWL = Namespace("http://www.w3.org/2002/07/owl#")
    tbox = Graph()
    tbox.parse(str(TBOX), format="turtle")
    datatype_props = {
        s for s in tbox.subjects(
            URIRef("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"),
            OWL.DatatypeProperty,
        )
    }
    SH = Namespace("http://www.w3.org/ns/shacl#")
    preds: List[URIRef] = []
    seen = set()
    for _, _, construct in shapes.triples((None, SH.construct, None)):
        # Each CONSTRUCT body is `... CONSTRUCT { $this erb:<prop> ?_result . } ...`
        for m in re.finditer(r"\$this\s+erb:(\w+)\s+\?_result", str(construct)):
            uri = ERB[m.group(1)]
            if uri in datatype_props and uri not in seen:
                seen.add(uri)
                preds.append(uri)
    return preds


def build_reasoned_graph(db: Dict[str, Any]) -> Graph:
    """TBox + (JSON-derived) ABox + SHACL fixpoint + OWL-RL closure."""
    for f in (TBOX, SHACL):
        if not f.exists():
            raise FileNotFoundError(
                f"Required generated ontology artifact missing: {f}. "
                f"Run `effortless build` (the OWL transpiler) first."
            )

    g = Graph()
    g.parse(str(TBOX), format="turtle")
    g.parse(data=json_db_to_turtle(db), format="turtle")

    # Three-stage pipeline: SHACL fixpoint → OWL-RL closure (once) → SHACL
    # fixpoint again. The two systems feed each other in one direction that
    # matters here, so a single ordered pass each suffices — we do NOT interleave
    # to a combined fixpoint (OWL-RL re-expanding the whole graph every pass is
    # both very slow and, with pyshacl's churn, non-convergent).
    #
    #   Stage 1  SHACL to fixpoint — derive every calc/lookup field (the FK
    #            lookups, isStale, etc.) and the ASSERTED precedesStep edges.
    #   Stage 2  OWL-RL once — materialize the transitive/inverse/type closure
    #            (the 6 INFERRED precedesStep pairs, delegatesTo closure, ...).
    #   Stage 3  SHACL to fixpoint AGAIN — now the rules that COUNT or READ
    #            OWL-RL output see it. This is what makes
    #              CountInferredPrecedencePairs = COUNTIFS(closure, IsInferred=TRUE)
    #            return 6 instead of 0: in a SHACL-then-OWL-RL-only pipeline that
    #            count fires while only the 4 asserted edges exist.
    #
    # Convergence is on the graph's CONTENT, not its length (a rule that corrects
    # a boolean object swaps a triple's object without changing the count, so a
    # length-only loop stops one pass early and serves the stale value).
    shapes = Graph()
    shapes.parse(str(SHACL), format="turtle")

    def snapshot(graph):
        return frozenset((str(s), str(p), str(o)) for s, p, o in graph)

    def shacl_to_fixpoint(stage):
        prev, iterations = None, 0
        while iterations < 50:
            iterations += 1
            pyshacl.validate(g, shacl_graph=shapes, advanced=True, inplace=True)
            cur = snapshot(g)
            if cur == prev:
                return
            prev = cur
        raise RuntimeError(
            f"SHACL rule application ({stage}) did not reach a fixpoint in 50 "
            f"iterations — a rule chain may be cyclic. Refusing to serve a partial graph."
        )

    # The predicates that SHACL rules DERIVE (their CONSTRUCT targets). A SHACL
    # CONSTRUCT only ADDS triples; it never retracts the old object. So if a
    # derived field's value CHANGES between stage 1 and stage 3 (e.g. an inferred-
    # pair count that was 0 before OWL-RL and 6 after), the subject would end up
    # carrying BOTH objects — a spurious multi-value. Before re-running SHACL in
    # stage 3 we therefore REMOVE every derived predicate's triples, so stage 3
    # rebuilds them cleanly from the now-complete graph. This is safe because
    # those predicates are PURE functions of the raw data + the OWL-RL closure —
    # stage 3 re-derives every one. (We do NOT touch raw or OWL-RL-inferred
    # triples, only the SHACL-derived calc/lookup/aggregation predicates.)
    derived_predicates = _shacl_construct_predicates(shapes)

    def clear_derived():
        for p in derived_predicates:
            for s, o in list(g.subject_objects(p)):
                g.remove((s, p, o))

    shacl_to_fixpoint("stage 1")                                    # calc/lookup + asserted edges
    owlrl.DeductiveClosure(owlrl.OWLRL_Semantics).expand(g)          # transitive/inverse/type closure
    clear_derived()                                                 # drop stage-1 derived values
    shacl_to_fixpoint("stage 3")                                    # recompute over the closure
    return g


def _individuals(g: Graph) -> Dict[str, List[Dict[str, Any]]]:
    """Every erb: individual with ALL its properties (raw + derived), grouped
    by its asserted/inferred erb: class. This is the OWL analogue of selecting
    `SELECT * FROM vw_<entity>` — the row already carries the computed columns."""
    # subject -> {prop_local: [values]}
    by_subject: Dict[URIRef, Dict[str, List[Any]]] = defaultdict(lambda: defaultdict(list))
    subject_classes: Dict[URIRef, List[str]] = defaultdict(list)

    rdf_type = URIRef("http://www.w3.org/1999/02/22-rdf-syntax-ns#type")
    for s, p, o in g:
        if not isinstance(s, URIRef) or not str(s).startswith(str(ERB)):
            continue
        if p == rdf_type:
            if str(o).startswith(str(ERB)):
                subject_classes[s].append(_local(o))
            continue
        if str(p).startswith(str(ERB)):
            val = _local(o) if (isinstance(o, URIRef) and str(o).startswith(str(ERB))) else (
                o.toPython() if isinstance(o, Literal) else str(o)
            )
            by_subject[s][_local(p)].append(val)

    out: Dict[str, List[Dict[str, Any]]] = defaultdict(list)
    for s, classes in subject_classes.items():
        # Pick the most specific erb: class that is one of the rulebook tables.
        # OWL-RL can attach extra inferred types; we group under the declared
        # data class (the one that also appears as a JSON db key set), but to
        # stay domain-agnostic we just take the first erb: class deterministically.
        primary = sorted(classes)[0]
        row: Dict[str, Any] = {"_iri": _local(s), "_classes": sorted(classes)}
        for prop, vals in by_subject[s].items():
            row[prop] = vals[0] if len(vals) == 1 else sorted(vals, key=str)
        out[primary].append(row)
    for cls in out:
        out[cls].sort(key=lambda r: r.get("_iri", ""))
    return dict(out)


def _precedence_closure(g: Graph, db: Dict[str, Any]) -> Dict[str, Any]:
    asserted = set()
    for row in db.get("StepPrecedence", []):
        asserted.add((row.get("fromStep"), row.get("toStep")))
    pairs = []
    q = g.query(
        "PREFIX erb: <http://example.org/erb#> "
        "SELECT ?f ?t WHERE { ?f erb:precedesStep ?t } ORDER BY ?f ?t"
    )
    for r in q:
        f, t = _local(r[0]), _local(r[1])
        pairs.append({"from_id": f, "to_id": t, "is_inferred": (f, t) not in asserted})
    return {
        "count": len(pairs),
        "inferred": sum(1 for p in pairs if p["is_inferred"]),
        "asserted": sum(1 for p in pairs if not p["is_inferred"]),
        "pairs": pairs,
    }


def _delegation(g: Graph, db: Dict[str, Any]) -> Dict[str, List[str]]:
    out: Dict[str, List[str]] = {}
    for row in db.get("Roles", []):
        rid = row.get("roleId")
        q = g.query(
            "PREFIX erb: <http://example.org/erb#> "
            f"SELECT ?r WHERE {{ erb:{rid} erb:delegatesTo ?r }} ORDER BY ?r"
        )
        chain = [_local(r[0]) for r in q]
        if chain:
            out[rid] = chain
    return out


def _disjoint_classes(g: Graph) -> List[Dict[str, str]]:
    q = g.query(
        "PREFIX erb: <http://example.org/erb#> "
        "PREFIX owl: <http://www.w3.org/2002/07/owl#> "
        "SELECT ?a ?b WHERE { ?a owl:disjointWith ?b . "
        "FILTER(STRSTARTS(STR(?a), STR(erb:)) && STRSTARTS(STR(?b), STR(erb:))) } "
        "ORDER BY ?a ?b"
    )
    seen, out = set(), []
    for r in q:
        a, b = _local(r[0]), _local(r[1])
        key = tuple(sorted([a, b]))
        if key not in seen:
            seen.add(key)
            out.append({"class_a": key[0], "class_b": key[1]})
    return out


def _roles_filled_by(g: Graph, db: Dict[str, Any]) -> Dict[str, Dict[str, Any]]:
    out: Dict[str, Dict[str, Any]] = {}
    arms = (
        ("filledByHumanAgent", "HumanAgent"),
        ("filledByAIAgent", "AIAgent"),
        ("filledByAutomatedPipeline", "AutomatedPipeline"),
    )
    for row in db.get("Roles", []):
        rid = row.get("roleId")
        result = {"agent": None, "agent_type": None}
        for arm, atype in arms:
            q = list(g.query(
                "PREFIX erb: <http://example.org/erb#> "
                f"SELECT ?a WHERE {{ erb:{rid} erb:{arm} ?a }}"
            ))
            if q:
                result = {"agent": _local(q[0][0]), "agent_type": atype}
                break
        out[rid] = result
    return out


def reason(db: Dict[str, Any]) -> Dict[str, Any]:
    g = build_reasoned_graph(db)
    return {
        "reasoned_triples": len(g),
        "individuals": _individuals(g),
        "competency": {
            "precedence_closure": _precedence_closure(g, db),
            "delegation": _delegation(g, db),
            "disjoint_classes": _disjoint_classes(g),
            "roles_filled_by": _roles_filled_by(g, db),
        },
    }


def main() -> None:
    if len(sys.argv) > 1 and sys.argv[1] not in ("-", "--stdin"):
        db = load_db(Path(sys.argv[1]))
    else:
        raw = sys.stdin.read()
        if not raw.strip():
            raise ValueError("No JSON db on stdin and no path argument given.")
        db = json.loads(raw)
    json.dump(reason(db), sys.stdout, indent=2, default=str)
    sys.stdout.write("\n")


if __name__ == "__main__":
    main()
