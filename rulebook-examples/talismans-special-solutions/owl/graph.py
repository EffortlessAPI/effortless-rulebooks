"""
Reasoned-graph loader for the Talisman's Special Solutions OWL substrate.

Loads the generated TBox (src/ontology.owl) + ABox (src/individuals.ttl),
applies an OWL-RL reasoner (rdflib + owlrl), and exposes the materialized graph
plus a few competency-question helpers. The API and the conformance test both
import from here so there is exactly ONE definition of "the reasoned graph".

The ontology is GENERATED from the rulebook by ../../execution-substrates/owl/
inject-into-owl.py — never hand-edited. This module only reasons over it.
"""
from __future__ import annotations

from pathlib import Path
from functools import lru_cache
from typing import List, Dict

from rdflib import Graph, Namespace
import owlrl
import pyshacl

ERB = Namespace("http://example.org/erb#")
SRC = Path(__file__).resolve().parent / "src"

# Files the substrate generates. If any is missing, that is a hard error — we do
# NOT fall back to an empty graph (an empty graph would let every query return
# "nothing", masking a broken build as a passing one). Fail loudly instead.
TBOX = SRC / "ontology.owl"
ABOX = SRC / "individuals.ttl"
SHACL = SRC / "rules.shacl.ttl"


def _local(uri) -> str:
    """The local name of an erb: IRI (everything after '#')."""
    s = str(uri)
    return s.split("#", 1)[1] if "#" in s else s


@lru_cache(maxsize=1)
def reasoned_graph() -> Graph:
    """Parse TBox + ABox, run the SHACL rules to fixpoint, then materialize the
    OWL-RL closure. Cached for the process lifetime.

    Two reasoners compose here, each doing the part it is good at:

    1. SHACL-SPARQL rules (rules.shacl.ttl) compute the *calculated/lookup*
       fields — RelativePath, Iri, ExecutingAgentType, the staleness booleans,
       etc. — exactly as the rulebook formulas specify. These have a dependency
       chain (a child's RelativePath needs its parent's RelativePath first), so
       we iterate the rule engine to a FIXPOINT: re-run until the triple count
       stops growing. The DAG depth is small and bounded, so this converges in a
       handful of passes (empirically 4).

    2. OWL-RL (owlrl) then computes the *ontological* closure — transitive
       precedesStep / delegatesTo, inverseOf reverse edges, class membership.
       This is what fires the article's headline inferences (step-1 → step-5).

    The cache is principled materialization: the SSoT is the .ttl on disk;
    delete the cache and it regenerates identically. A rebuild restarts the
    process, clearing it.
    """
    for f in (TBOX, ABOX, SHACL):
        if not f.exists():
            raise FileNotFoundError(
                f"Required ontology artifact missing: {f}. "
                f"Run the OWL transpiler (effortless build) first."
            )
    g = Graph()
    g.parse(str(TBOX), format="turtle")
    g.parse(str(ABOX), format="turtle")

    # Stage 1: SHACL rules to fixpoint (calculated/lookup fields, chained).
    # Iterate on CONTENT, not length: a boolean field a later rule corrects
    # (e.g. isAtComplianceRisk flips once its isStale input resolves) changes a
    # triple's object without changing the count, so a length-based loop would
    # stop one pass early and serve the stale value. Hash the triple set and
    # stop only when a full pass leaves it unchanged.
    shapes = Graph()
    shapes.parse(str(SHACL), format="turtle")

    def graph_fingerprint(graph):
        return hash(frozenset((str(s), str(p), str(o)) for s, p, o in graph))

    prev_fp, iterations = None, 0
    while iterations < 50:
        iterations += 1
        pyshacl.validate(g, shacl_graph=shapes, advanced=True, inplace=True)
        fp = graph_fingerprint(g)
        if fp == prev_fp:
            break
        prev_fp = fp
    else:
        raise RuntimeError(
            "SHACL rule application did not reach a fixpoint in 50 iterations — "
            "a rule chain may be cyclic. Refusing to serve a partially-computed graph."
        )

    # Stage 2: OWL-RL ontological closure (transitive, inverseOf, types).
    owlrl.DeductiveClosure(owlrl.OWLRL_Semantics).expand(g)
    return g


def iri_of(table_class: str, pk_value: str) -> str:
    """The computed Iri for a given individual, read from the reasoned graph.

    Looks up the individual by its bare-PK IRI (how the ABox names it) and
    returns the SHACL-computed erb:iri value — the path-derived identity that
    matches the Postgres `iri` column. Raises if absent (a missing Iri means the
    rule chain didn't fire; we fail loud rather than return the bare PK).
    """
    g = reasoned_graph()
    vals = list(g.objects(ERB[pk_value], ERB.iri))
    if not vals:
        raise KeyError(f"No computed Iri for {pk_value} — SHACL Iri rule did not fire.")
    return str(vals[0])


# ---------------------------------------------------------------------------
# Competency-question helpers — each is a SPARQL query over the REASONED graph.
# The answers include inferred triples (transitive closure, inverseOf), which is
# the whole point: the reasoner did the work, we just SELECT it.
# ---------------------------------------------------------------------------

def _q(query: str) -> Graph:
    return reasoned_graph().query(
        "PREFIX erb: <http://example.org/erb#>\n" + query
    )


def step_precedence_closure() -> List[Dict[str, str]]:
    """Every (from, to) precedence pair after transitive reasoning.

    Mirrors Postgres vw_step_precedence_closure. `is_inferred` is TRUE when the
    pair is NOT one of the directly-asserted StepPrecedence edges — i.e. the
    reasoner derived it. We recompute is_inferred by checking membership in the
    asserted base set, so it is meaningful per-row.
    """
    # Asserted edges live in the ABox before reasoning; recover them from the
    # junction projection by re-reading the base graph (unreasoned).
    base = Graph()
    base.parse(str(ABOX), format="turtle")
    asserted = {
        (_local(s), _local(o))
        for s, o in base.subject_objects(ERB.precedesStep)
    }
    rows = []
    for r in _q(
        "SELECT ?f ?t WHERE { ?f erb:precedesStep ?t } ORDER BY ?f ?t"
    ):
        f, t = _local(r[0]), _local(r[1])
        rows.append({
            "from_id": f,
            "to_id": t,
            "is_inferred": (f, t) not in asserted,
        })
    return rows


def steps_after(step_id: str) -> List[str]:
    """All steps reachable from `step_id` via precedesStep (incl. inferred)."""
    rows = _q(
        f"SELECT ?t WHERE {{ erb:{step_id} erb:precedesStep ?t }} ORDER BY ?t"
    )
    return [_local(r[0]) for r in rows]


def delegation_closure(role_id: str) -> List[str]:
    """All roles `role_id` can delegate to, transitively."""
    rows = _q(
        f"SELECT ?r WHERE {{ erb:{role_id} erb:delegatesTo ?r }} ORDER BY ?r"
    )
    return [_local(r[0]) for r in rows]


def disjoint_classes() -> List[Dict[str, str]]:
    """The pairwise-disjoint class declarations (the agent-type triad)."""
    rows = _q(
        "SELECT ?a ?b WHERE { ?a owl:disjointWith ?b . "
        "FILTER(STRSTARTS(STR(?a), STR(erb:)) && STRSTARTS(STR(?b), STR(erb:))) } "
        "ORDER BY ?a ?b"
    )
    out = []
    seen = set()
    for r in rows:
        a, b = _local(r[0]), _local(r[1])
        key = tuple(sorted([a, b]))
        if key not in seen:
            seen.add(key)
            out.append({"class_a": key[0], "class_b": key[1]})
    return out


def role_filled_by(role_id: str) -> Dict[str, str]:
    """The single agent (and its type) that fills a role, via the filledBy arms."""
    for arm, atype in (
        ("filledByHumanAgent", "HumanAgent"),
        ("filledByAIAgent", "AIAgent"),
        ("filledByAutomatedPipeline", "AutomatedPipeline"),
    ):
        rows = list(_q(f"SELECT ?a WHERE {{ erb:{role_id} erb:{arm} ?a }}"))
        if rows:
            return {"agent": _local(rows[0][0]), "agent_type": atype}
    return {"agent": None, "agent_type": None}
