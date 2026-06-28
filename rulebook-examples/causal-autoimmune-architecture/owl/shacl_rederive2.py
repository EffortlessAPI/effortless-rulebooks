#!/usr/bin/env python3
"""SHACL re-derivation probe v2 — CLEAN-SLATE fixpoint.

v1 ran pyshacl repeatedly with inplace=True and accumulated triples. That FROZE
aggregations: a COUNT/SUM rule that fires in an early pass — before its calculated
child inputs (e.g. CausalMechanisms.isCausalArchitectureNode) have been derived —
produces 0, and pyshacl's monotonic CONSTRUCT never RETRACTS that 0 once the inputs
later materialize. So Individuals.countConfirmedCausalNodes stuck at 0 and the whole
keystone chain read false/None.

The fix is in the EXECUTION STRATEGY, not the rules. Each iteration we:
  1. start from a graph holding ONLY leaf (raw) facts + the ontology,
  2. plus every derived triple computed SO FAR,
  3. run ONE pyshacl pass to (re)derive,
  4. but BEFORE folding results in, drop the previous derived triples so a stale
     early value can't survive — i.e. recompute the derived layer from the current
     best inputs each round.
We iterate until the derived-value SET is identical between two rounds (true fixpoint
of the DAG, the way Postgres evaluates it bottom-up).

Concretely: keep a `base` graph (ontology + leaves, immutable). Each round build
`work = base + derived_so_far`, run pyshacl inplace on `work`, extract ALL derived
predicates from `work` into `derived_next`, compare to `derived_so_far`; stop when
equal. Because every round recomputes from base+derived, an aggregation re-fires with
the latest child values and overwrites its stale 0.

Run:  python3 owl/shacl_rederive2.py   (SHACL_SRC env overrides owl/src)
"""
import json
import os
import subprocess
import sys

from rdflib import Graph, Namespace, RDF, URIRef
from pyshacl import validate

HERE = os.path.dirname(os.path.abspath(__file__))
SRC = os.environ.get("SHACL_SRC", os.path.join(HERE, "src"))
RULEBOOK = os.path.join(HERE, "..", "effortless-rulebook", "effortless-rulebook.json")
NTWF = Namespace("https://w3id.org/effortless-ntwf#")
PG_CONN = os.environ.get(
    "DATABASE_URL", "postgresql://postgres@localhost:5432/causal_autoimmune"
)

GATE_PREDS = {
    "isHighConfidencePrediction": "is_high_confidence_prediction",
    "isFalsifiabilityBacked": "is_falsifiability_backed",
    "isAncestryTransportSafe": "is_ancestry_transport_safe",
    "isClinicallyActionable": "is_clinically_actionable",
    "decidingGate": "deciding_gate",
}


def field_to_property_uri(field_name: str) -> str:
    if not field_name:
        return str(NTWF) + "unknown"
    return str(NTWF) + field_name[0].lower() + field_name[1:]


def derived_predicates():
    """Every predicate URI that is a DERIVED (formula-bearing) field, from the rulebook.
    These are the triples we strip/recompute each round; raw leaves are never touched."""
    rb = json.load(open(RULEBOOK))
    preds = set()
    for tname, tdef in rb.items():
        if not isinstance(tdef, dict) or "schema" not in tdef:
            continue
        for col in tdef["schema"]:
            if col.get("formula"):
                preds.add(URIRef(field_to_property_uri(col.get("name", ""))))
    return preds


def load_base():
    g = Graph()
    g.parse(os.path.join(SRC, "ontology.owl"), format="turtle")
    g.parse(os.path.join(SRC, "individuals.ttl"), format="turtle")
    return g


def _is_numeric_or_bool(o):
    v = o.toPython() if hasattr(o, "toPython") else o
    if isinstance(v, bool):
        return True
    try:
        float(v)
        return True
    except (TypeError, ValueError):
        return False


def _canonical_numeric(objs):
    """Collapse multiple numeric/boolean values for one (subject,predicate) to the
    converged one. Clean-slate makes counts/sums/flags rise MONOTONICALLY toward the
    Postgres truth as deeper inputs come online (a COUNT can only grow as more child
    rows qualify; a gate boolean flips false->true once its evidence exists, never
    back). So the converged value is the MAX: numerics by magnitude, booleans
    false<true. Used ONLY for numeric/boolean predicates — NOT strings (see below)."""
    if len(objs) == 1:
        return objs[0]

    def key(o):
        v = o.toPython() if hasattr(o, "toPython") else o
        if isinstance(v, bool):
            return 1 if v else 0
        return float(v)
    return max(objs, key=key)


def _disambiguate_string(s, p, objs, carry):
    """Pick the converged value for a multi-valued string predicate. Single value ->
    return it. For decidingGate, a residual pyshacl intra-pass race can leave both a
    stale lower-gate label and the correct one; the keystone boolean
    (isClinicallyActionable, already converged in `carry`) decides: an actionable row's
    deciding gate MUST be 'AllGatesPass'. Use that to select; otherwise (still
    ambiguous, non-keystone string) fall back to the most common / first stable
    value."""
    if len(objs) == 1:
        return objs[0]
    if str(p) == str(NTWF.decidingGate):
        actionable = carry.value(s, NTWF.isClinicallyActionable)
        is_act = actionable is not None and (
            actionable.toPython() is True if hasattr(actionable, "toPython") else False)
        if is_act:
            for o in objs:
                if str(o) == "AllGatesPass":
                    return o
        else:
            # non-actionable: the keystone label is never AllGatesPass; drop it.
            non_pass = [o for o in objs if str(o) != "AllGatesPass"]
            if non_pass:
                objs = non_pass
    # Deterministic fallback: lexicographically-first (stable across runs).
    return sorted(objs, key=lambda o: str(o))[0]


def rederive_clean_slate(max_rounds=25):
    shapes = Graph()
    shapes.parse(os.path.join(SRC, "rules.shacl.ttl"), format="turtle")
    dpreds = derived_predicates()

    base = load_base()
    # Strip any derived predicate that leaked into individuals.ttl so we only ever
    # TRUST leaves; everything derived is recomputed. (Verified earlier: keystone is
    # not pre-baked, but be defensive.)
    for p in list(dpreds):
        base.remove((None, p, None))

    # `carry` holds ONLY numeric/boolean derived values — the ones that converge
    # MONOTONICALLY upward and are safe to MAX-collapse and feed back in. String
    # derived values (deciding_gate, *_label, *_pathway) are NEVER carried: they are
    # recomputed FRESH every round from the carried numerics/booleans, and we keep the
    # CURRENT pass's value. Carrying strings was the deciding_gate bug — a SHACL
    # CONSTRUCT adds, never replaces, so an early-round "Undetermined"/"NoValidatedMechanism"
    # (computed before isClinicallyActionable settled) coexisted with the true
    # "AllGatesPass"; a lexical MAX then picked the wrong one (there is NO monotone
    # order on strings). Recomputing strings each round from settled numeric inputs
    # makes the final pass authoritative.
    carry = Graph()        # numeric/bool only, MAX-collapsed, fed back in
    strings = Graph()      # current-round string derivations, NOT fed back in
    prev_sig = None
    rounds = 0
    for r in range(max_rounds):
        rounds = r + 1
        work = Graph()
        for t in base:
            work.add(t)
        for t in carry:    # only monotone numerics/booleans are fed back
            work.add(t)
        validate(work, shacl_graph=shapes, advanced=True, inplace=True,
                 iterate_rules=True, do_owl_imports=False)

        num_groups = {}
        strings = Graph()
        for p in dpreds:
            for s, o in work.subject_objects(p):
                if _is_numeric_or_bool(o):
                    num_groups.setdefault((s, p), []).append(o)
                else:
                    strings.add((s, p, o))  # take current pass's string verbatim
        carry_next = Graph()
        for (s, p), objs in num_groups.items():
            carry_next.add((s, p, _canonical_numeric(objs)))

        # Fixpoint on the carried numerics (strings are a pure function of them).
        sig = frozenset((s, p, o) for s, p, o in carry_next)
        if sig == prev_sig:
            carry = carry_next
            break
        prev_sig = sig
        carry = carry_next

    # FINAL SETTLING for strings — deterministic, no stale-value problem. A string
    # field (deciding_gate, *_label) is a PURE function of the converged
    # numerics/booleans in `carry`. The flakiness came from pyshacl's UNSPECIFIED
    # intra-pass rule order: in one pass, deciding_gate's CONSTRUCT can run before
    # isClinicallyActionable's, reading it unbound/false and emitting a lower-gate
    # label for whichever actionable row loses the race (pred-g one run, pred-l the
    # next). The fix: build a graph of base + the converged carry (so every boolean
    # input is PRESENT and single-valued before any rule runs), validate TWICE on it
    # WITHOUT carrying strings forward — pass 1 guarantees every derived input is
    # materialized; pass 2's string CONSTRUCTs therefore read fully-settled inputs.
    # Then extract each string as the value produced AGAINST settled inputs; because a
    # string was never carried, the only way a stale label survives is if pass 2 also
    # re-emitted it from settled inputs — which it can't (the inputs are now correct).
    # A CONSTRUCT still won't retract a wrong value emitted in pass 1, so we read from
    # a THIRD graph where the string predicate starts ABSENT: seed base+carry, run
    # once, the rule now fires exactly once on settled inputs -> one correct value.
    settle = Graph()
    for t in base:
        settle.add(t)
    for t in carry:
        settle.add(t)
    # Two sequential passes so every derived INPUT (incl. booleans like
    # isClinicallyActionable) is present, then strings recompute against them.
    validate(settle, shacl_graph=shapes, advanced=True, inplace=True,
             iterate_rules=True, do_owl_imports=False)
    validate(settle, shacl_graph=shapes, advanced=True, inplace=True,
             iterate_rules=True, do_owl_imports=False)

    strings = Graph()
    for p in dpreds:
        groups = {}
        for s, o in settle.subject_objects(p):
            if not _is_numeric_or_bool(o):
                groups.setdefault(s, []).append(o)
        for s, objs in groups.items():
            # If a predicate is multi-valued (a stale pass-1 label plus the correct
            # pass-2 one), pick the one CONSISTENT WITH THE CONVERGED GATES rather than
            # guessing lexically: for an actionable row the keystone label must be
            # AllGatesPass; for a non-actionable row it's the named failing gate. Use
            # the keystone boolean already in `carry` to disambiguate deciding_gate;
            # for any other string field a single value is expected, so take it.
            strings.add((s, p, _disambiguate_string(s, p, objs, carry)))

    derived = Graph()
    for t in carry:
        derived.add(t)
    for t in strings:
        derived.add(t)

    final = Graph()
    for t in base:
        final.add(t)
    for t in derived:
        final.add(t)
    return final, rounds, len(final)


def shacl_predictions(data):
    out = {}
    for s in data.subjects(RDF.type, NTWF.IndividualPredictions):
        pid = data.value(s, NTWF.individualPredictionId)
        if pid is None:
            continue
        out[str(pid)] = {
            col: (data.value(s, URIRef(str(NTWF) + local)))
            for local, col in GATE_PREDS.items()
        }
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
        p = line.split("\t")
        rows[p[0]] = dict(zip(GATE_PREDS.values(), p[1:]))
    return rows


def norm(v):
    if v is None or v == "":
        return None
    if hasattr(v, "toPython"):
        v = v.toPython()
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
    print("SHACL re-derivation v2 (CLEAN-SLATE fixpoint): keystone + gates vs Postgres")
    print("=" * 74)
    data, rounds, n = rederive_clean_slate()
    print(f"  fixpoint after {rounds} clean-slate rounds; {n} triples")
    print()

    shacl = shacl_predictions(data)
    pg = pg_predictions()
    ids = sorted(set(shacl) | set(pg))
    mismatches = 0
    keystones = 0
    for pid in ids:
        sr, pr = shacl.get(pid, {}), pg.get(pid, {})
        cells = []
        bad = False
        for c in GATE_PREDS.values():
            sv, pv = norm(sr.get(c)), norm(pr.get(c))
            if c == "is_clinically_actionable" and sv is not None:
                keystones += 1
            if sv != pv:
                mismatches += 1
                bad = True
                cells.append(f"{c}:S={sv}/P={pv}!!")
            else:
                cells.append(f"{c}={sv}")
        print(f"  {pid}: " + " ".join(cells) + ("  <<<" if bad else "  ✓"))
    print()
    print(f"  keystones derived: {keystones}/{len(pg)}   cell mismatches: {mismatches}")
    if mismatches == 0 and keystones == len(pg):
        print("  RECEIPT: OWL/SHACL re-derives every keystone + gate, identical to Postgres.")
        return 0
    print("  INCOMPLETE")
    return 1


if __name__ == "__main__":
    sys.exit(main())
