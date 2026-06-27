#!/usr/bin/env python3
"""Cross-substrate conformance referee for the state-machine reachability closure.

Two independent substrates compute the SAME thing from the SAME asserted edges:

  * Postgres : vw_state_transition_rules_closure  (a WITH RECURSIVE view emitted by
               rulebook-to-postgres from the `closure` field) -> (from_id, to_id, ...)
  * OWL-RL   : owlrl.DeductiveClosure over ontology.owl + individuals.ttl, where the
               `progression` object property is declared an owl:TransitiveProperty, so
               the reasoner deduces every reachable (from, to) pair on its own.

This script runs the OWL reasoner, extracts its closed `progression` pairs, pulls the
Postgres closure, and DIFFS the two reachability sets. Set equality is the conformance
receipt: the same reachability ordering falls out of a recursive SQL view and an OWL-RL
deductive closure, from one declared model. Exit 0 iff they agree.

Deps (all already present): rdflib, owlrl, psycopg2/psql.
Run:  python3 owl/reason.py   (from the project root)
"""
import subprocess
import sys
import os

import rdflib
from rdflib import Graph, URIRef
import owlrl

HERE = os.path.dirname(os.path.abspath(__file__))
SRC = os.path.join(HERE, "src")
NS = "https://w3id.org/effortless-ntwf#"   # effortless-ntwf: prefix used in the emitted TTL
PROGRESSION = URIRef(NS + "progression")
PG_CONN = os.environ.get(
    "DATABASE_URL",
    "postgresql://postgres@localhost:5432/causal_autoimmune",
)


def short(uri):
    """Local name after the namespace, for readable diffs."""
    s = str(uri)
    return s[len(NS):] if s.startswith(NS) else s


def owl_reachability():
    """Load ontology + individuals, run OWL-RL deductive closure, return the set of
    (from, to) local-name pairs linked by `progression` AFTER closure."""
    g = Graph()
    # ontology.owl is Turtle despite the .owl extension (it opens with @prefix).
    g.parse(os.path.join(SRC, "ontology.owl"), format="turtle")
    g.parse(os.path.join(SRC, "individuals.ttl"), format="turtle")

    asserted = {(short(s), short(o)) for s, o in g.subject_objects(PROGRESSION)}

    # Materialize the OWL-RL deductive closure in place. The TransitiveProperty axiom
    # on `progression` makes the reasoner add every reachable pair.
    owlrl.DeductiveClosure(owlrl.OWLRL_Semantics).expand(g)

    closed = {(short(s), short(o)) for s, o in g.subject_objects(PROGRESSION)}
    return asserted, closed


def pg_reachability():
    """Pull (from_id, to_id) and the is_inferred flag from the Postgres closure view."""
    sql = ("SELECT from_id, to_id, is_inferred "
           "FROM vw_state_transition_rules_closure;")
    out = subprocess.run(
        ["psql", PG_CONN, "-tAF", "\t", "-c", sql],
        capture_output=True, text=True, check=True,
    ).stdout.strip()

    all_pairs, asserted = set(), set()
    for line in out.splitlines():
        if not line.strip():
            continue
        frm, to, inferred = line.split("\t")
        all_pairs.add((frm, to))
        if inferred in ("f", "false", "0"):
            asserted.add((frm, to))
    return asserted, all_pairs


def main():
    owl_asserted, owl_closed = owl_reachability()
    pg_asserted, pg_all = pg_reachability()

    print("=" * 70)
    print("Cross-substrate conformance: state-machine reachability closure")
    print("=" * 70)
    print(f"  asserted edges      OWL={len(owl_asserted):>3}   PG={len(pg_asserted):>3}")
    print(f"  full reachability   OWL={len(owl_closed):>3}   PG={len(pg_all):>3}")
    print(f"  inferred (closed-asserted)  OWL={len(owl_closed - owl_asserted):>3}"
          f"   PG={len(pg_all - pg_asserted):>3}")
    print()

    only_owl = owl_closed - pg_all
    only_pg = pg_all - owl_closed

    # Headline example: the long-range inferred pair the CR draft named.
    headline = ("diagnosis-lifecycle--intake", "diagnosis-lifecycle--actionable")
    print("  headline long-range pair  intake -> actionable:")
    print(f"     in OWL closure: {headline in owl_closed}    in PG closure: {headline in pg_all}")
    print()

    if not only_owl and not only_pg:
        print("RECEIPT: the two substrates agree EXACTLY on the reachability set.")
        print(f"  {len(owl_closed)} (from,to) pairs, identical in OWL-RL and Postgres.")
        return 0

    print("MISMATCH: the substrates disagree.")
    if only_owl:
        print(f"  {len(only_owl)} pair(s) in OWL but not Postgres:")
        for p in sorted(only_owl)[:20]:
            print("    +OWL", p)
    if only_pg:
        print(f"  {len(only_pg)} pair(s) in Postgres but not OWL:")
        for p in sorted(only_pg)[:20]:
            print("    +PG ", p)
    return 1


if __name__ == "__main__":
    sys.exit(main())
