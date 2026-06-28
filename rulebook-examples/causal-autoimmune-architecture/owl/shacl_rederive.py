#!/usr/bin/env python3
"""SHACL re-derivation probe — the linchpin of the OWL-power keystone proof.

The `rulebook-to-owl` transpiler emits rules.shacl.ttl: 197 sh:construct rules that
ARE the calculated-field DAG (gates, rollups, keystone) re-expressed as SPARQL CONSTRUCTs.
individuals.ttl carries ONLY leaf facts for the patients (no derived gate/keystone values
are asserted — verified: isClinicallyActionable appears 0x in individuals.ttl).

This probe runs pyshacl with advanced=True repeatedly over (ontology + rules + individuals),
iterating to a FIXPOINT (sh:construct rules don't chain in dependency order in a single pass,
so multi-hop leaf->gate->keystone derivations need several passes to settle). It then extracts
the SHACL-derived isClinicallyActionable + the four gate values per IndividualPredictions and
diffs them against the Postgres vw_individual_predictions truth.

This is a PROBE (run standalone): proves the second substrate re-derives the keystone from
leaf facts before we wire any UI. It does not yet gate the build.

Run:  python3 owl/shacl_rederive.py
"""
import os
import subprocess
import sys

import rdflib
from rdflib import Graph, Namespace, URIRef, Literal
from pyshacl import validate

HERE = os.path.dirname(os.path.abspath(__file__))
SRC = os.path.join(HERE, "src")
NTWF = Namespace("https://w3id.org/effortless-ntwf#")
PG_CONN = os.environ.get(
    "DATABASE_URL", "postgresql://postgres@localhost:5432/causal_autoimmune"
)

# Predicate local-names we want to compare against Postgres columns.
GATE_PREDS = {
    "isHighConfidencePrediction": "is_high_confidence_prediction",
    "isFalsifiabilityBacked": "is_falsifiability_backed",
    "isAncestryTransportSafe": "is_ancestry_transport_safe",
    "isClinicallyActionable": "is_clinically_actionable",
    "decidingGate": "deciding_gate",
}


def load_base():
    g = Graph()
    g.parse(os.path.join(SRC, "ontology.owl"), format="turtle")
    g.parse(os.path.join(SRC, "individuals.ttl"), format="turtle")
    return g


def rules_graph():
    sg = Graph()
    sg.parse(os.path.join(SRC, "rules.shacl.ttl"), format="turtle")
    return sg


def rederive_to_fixpoint(max_passes=12):
    """Run pyshacl advanced (sh:rule execution) until the triple count stops growing."""
    data = load_base()
    shapes = rules_graph()
    prev = -1
    passes = 0
    for i in range(max_passes):
        passes = i + 1
        # advanced=True executes sh:rule (SPARQLRule CONSTRUCTs); inplace mutates `data`.
        validate(
            data,
            shacl_graph=shapes,
            advanced=True,
            inplace=True,
            iterate_rules=True,
            do_owl_imports=False,
        )
        n = len(data)
        if n == prev:
            break
        prev = n
    return data, passes, len(data)


def shacl_predictions(data):
    """Pull derived gate+keystone values per IndividualPredictions from the closed graph."""
    out = {}
    for s in data.subjects(rdflib.RDF.type, NTWF.IndividualPredictions):
        pid = data.value(s, NTWF.individualPredictionId)
        if pid is None:
            continue
        row = {}
        for local, col in GATE_PREDS.items():
            v = data.value(s, URIRef(str(NTWF) + local))
            row[col] = None if v is None else v.toPython()
        out[str(pid)] = row
    return out


def pg_predictions():
    cols = "individual_prediction_id," + ",".join(GATE_PREDS.values())
    sql = f"SELECT {cols} FROM vw_individual_predictions ORDER BY 1;"
    out = subprocess.run(
        ["psql", PG_CONN, "-tAF", "\t", "-c", sql],
        capture_output=True, text=True, check=True,
    ).stdout.strip()
    rows = {}
    for line in out.splitlines():
        if not line.strip():
            continue
        parts = line.split("\t")
        pid = parts[0]
        vals = dict(zip(GATE_PREDS.values(), parts[1:]))
        rows[pid] = vals
    return rows


def norm(v):
    """Normalize a value to a comparable token (bool/None/string)."""
    if v is None or v == "":
        return None
    if isinstance(v, bool):
        return v
    s = str(v).strip()
    if s in ("t", "true", "True"):
        return True
    if s in ("f", "false", "False"):
        return False
    return s


def main():
    print("=" * 74)
    print("SHACL re-derivation probe: keystone + four gates, OWL/SHACL vs Postgres")
    print("=" * 74)
    data, passes, ntriples = rederive_to_fixpoint()
    print(f"  fixpoint reached after {passes} pass(es); {ntriples} triples in closed graph")
    print()

    shacl = shacl_predictions(data)
    pg = pg_predictions()

    ids = sorted(set(shacl) | set(pg))
    print(f"  {len(ids)} IndividualPredictions compared")
    print()
    hdr = f"  {'pred':<8} {'gate':<26} {'SHACL':<14} {'PG':<14} {'ok'}"
    mismatches = 0
    derived_keystones = 0
    for pid in ids:
        srow = shacl.get(pid, {})
        prow = pg.get(pid, {})
        first = True
        for col in GATE_PREDS.values():
            sv = norm(srow.get(col))
            pv = norm(prow.get(col))
            if col == "is_clinically_actionable" and sv is not None:
                derived_keystones += 1
            ok = sv == pv
            if not ok:
                mismatches += 1
            tag = "OK" if ok else "*** MISMATCH"
            label = pid if first else ""
            print(f"  {label:<8} {col:<26} {str(sv):<14} {str(pv):<14} {tag}")
            first = False
        print()

    print("-" * 74)
    print(f"  keystones SHACL-derived from leaf facts: {derived_keystones}/{len(ids)}")
    print(f"  total cell mismatches: {mismatches}")
    if mismatches == 0 and derived_keystones == len(ids):
        print("  RECEIPT: OWL/SHACL re-derives every keystone + gate, identical to Postgres.")
        return 0
    print("  PROBE INCOMPLETE: see mismatches / undrived keystones above.")
    return 1


if __name__ == "__main__":
    sys.exit(main())
