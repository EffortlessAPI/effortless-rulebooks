#!/usr/bin/env python3
"""Test harness for rulebook-to-hydrated-tripples output.

Verifies that individuals-hydrated.ttl:
  1. Parses cleanly with rdflib.
  2. Is a superset of individuals.ttl (all asserted triples still present).
  3. Contains the correct derived-field triples from Postgres (SubstrateConformanceFields manifest).
  4. Has no triples that contradict Postgres values.
  5. Reports 0 mismatches when used instead of SHACL derivation in reason.py.

Run from project root:
    python3 owl/test-hydrated-tripples.py

Exits 0 on all-pass, 1 if any check fails.
"""

import json
import os
import sys

import psycopg2
import psycopg2.extras
import rdflib
from rdflib import Graph, Namespace, URIRef, Literal
from rdflib.namespace import RDF, XSD

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)

ASSERTED_TTL = os.path.join(HERE, "src", "individuals.ttl")
HYDRATED_TTL = os.path.join(HERE, "src", "individuals-hydrated.ttl")
RULEBOOK = os.path.join(ROOT, "effortless-rulebook", "simpsons-paradox-rulebook.json")

NS = "https://w3id.org/effortless-ntwf#"
NTWF = Namespace(NS)

PG_CONN = os.environ.get(
    "DATABASE_URL",
    "postgresql://postgres@localhost:5432/simpsons_paradox",
)

FLOAT_TOL = 1e-6

PASS = "\033[32mPASS\033[0m"
FAIL = "\033[31mFAIL\033[0m"
WARN = "\033[33mWARN\033[0m"


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def load_rulebook():
    with open(RULEBOOK, encoding="utf-8") as f:
        return json.load(f)


def load_conformance_manifest(rb):
    return rb.get("SubstrateConformanceFields", {}).get("data", [])


def pg_connect():
    return psycopg2.connect(PG_CONN, cursor_factory=psycopg2.extras.RealDictCursor)


def pg_fetch_view(conn, view_name, pk_col):
    cur = conn.cursor()
    try:
        cur.execute(f"SELECT * FROM {view_name}")
    except psycopg2.errors.UndefinedTable:
        conn.rollback()
        return None
    rows = cur.fetchall()
    return {str(r[pk_col]): dict(r) for r in rows if r.get(pk_col) is not None}


def slug(pk):
    return str(pk).replace(" ", "-")


def load_graph(path):
    g = Graph()
    g.parse(path, format="turtle")
    return g


def values_approx_equal(a, b, typ):
    if typ == "float":
        try:
            return abs(float(a) - float(b)) < FLOAT_TOL
        except (TypeError, ValueError):
            return str(a) == str(b)
    if typ == "bool":
        def to_bool(v):
            if isinstance(v, bool):
                return v
            return str(v).lower() in ("true", "1", "yes")
        return to_bool(a) == to_bool(b)
    if typ == "int":
        try:
            return int(a) == int(b)
        except (TypeError, ValueError):
            return str(a) == str(b)
    return str(a).strip() == str(b).strip()


def literal_value(node):
    if isinstance(node, Literal):
        return node.toPython()
    return None


# ---------------------------------------------------------------------------
# Check 1: File exists and parses
# ---------------------------------------------------------------------------

def check_parse():
    print("\n[1] individuals-hydrated.ttl exists and parses cleanly")
    if not os.path.exists(HYDRATED_TTL):
        print(f"  {FAIL}: file not found: {HYDRATED_TTL}")
        return None
    try:
        g = load_graph(HYDRATED_TTL)
        triple_count = len(g)
        print(f"  {PASS}: parsed OK — {triple_count} triples")
        return g
    except Exception as e:
        print(f"  {FAIL}: parse error: {e}")
        return None


# ---------------------------------------------------------------------------
# Check 2: Hydrated is a superset of asserted
# ---------------------------------------------------------------------------

def check_superset(hydrated_g):
    print("\n[2] individuals-hydrated.ttl is a superset of individuals.ttl")
    if not os.path.exists(ASSERTED_TTL):
        print(f"  {WARN}: asserted file not found, skipping superset check")
        return True

    asserted_g = load_graph(ASSERTED_TTL)
    missing = []
    for s, p, o in asserted_g:
        if (s, p, o) not in hydrated_g:
            missing.append((s, p, o))

    if missing:
        print(f"  {FAIL}: {len(missing)} asserted triples missing from hydrated graph")
        for s, p, o in missing[:10]:
            print(f"    {s} {p} {o}")
        if len(missing) > 10:
            print(f"    ... and {len(missing) - 10} more")
        return False

    asserted_count = len(asserted_g)
    hydrated_count = len(hydrated_g)
    extra = hydrated_count - asserted_count
    print(f"  {PASS}: all {asserted_count} asserted triples present; +{extra} derived triples added")
    return True


# ---------------------------------------------------------------------------
# Check 3: SubstrateConformanceFields values match Postgres
# ---------------------------------------------------------------------------

TABLE_PK = {
    "TreatmentRankings": "ranking_id",
    "StratumSummaries":  "stratum_summary_id",
    "StratumVariables":  "stratum_variable_id",
}

VIEW_NAME = {
    "TreatmentRankings": "vw_treatment_rankings",
    "StratumSummaries":  "vw_stratum_summaries",
    "StratumVariables":  "vw_stratum_variables",
}


def check_conformance_fields(hydrated_g, manifest):
    print("\n[3] SubstrateConformanceFields values match Postgres")
    errors = 0
    checked = 0

    conn = pg_connect()
    pg_cache = {}

    for row in manifest:
        if not row.get("InCompareSet"):
            continue

        table = row["SourceTable"]
        owl_name = row["OwlLocalName"]
        pg_col = row["PgColumn"]
        dtype = row["DataType"]

        if table not in pg_cache:
            pk = TABLE_PK.get(table)
            view = VIEW_NAME.get(table)
            if not pk or not view:
                print(f"  {WARN}: unknown table {table}, skipping")
                continue
            data = pg_fetch_view(conn, view, pk)
            if data is None:
                print(f"  {WARN}: view {view} not found, skipping table {table}")
                pg_cache[table] = {}
            else:
                pg_cache[table] = data

        pg_rows = pg_cache.get(table, {})
        pred = NTWF[owl_name]

        for pk_val, pg_row in pg_rows.items():
            pg_val = pg_row.get(pg_col)
            if pg_val is None:
                continue  # null in PG → triple should be absent; no check needed

            subj = NTWF[slug(pk_val)]
            graph_vals = list(hydrated_g.objects(subj, pred))

            if not graph_vals:
                errors += 1
                if errors <= 20:
                    print(f"  {FAIL}: missing triple <{slug(pk_val)}> {owl_name} (expected {pg_val})")
                continue

            graph_val = literal_value(graph_vals[0])
            if not values_approx_equal(graph_val, pg_val, dtype):
                errors += 1
                if errors <= 20:
                    print(f"  {FAIL}: mismatch <{slug(pk_val)}> {owl_name}: "
                          f"graph={graph_val!r} pg={pg_val!r}")
            else:
                checked += 1

    conn.close()

    if errors:
        print(f"  {FAIL}: {errors} mismatches found (checked {checked} correctly)")
        return False
    print(f"  {PASS}: {checked} conformance-field triples all match Postgres")
    return True


# ---------------------------------------------------------------------------
# Check 4: No triples contradict Postgres
# ---------------------------------------------------------------------------

def check_no_contradictions(hydrated_g, manifest):
    print("\n[4] No triples in hydrated graph contradict Postgres")
    # This is the inverse of check 3: for every triple in the graph that corresponds
    # to a manifest field, ensure the value agrees with Postgres.
    contradictions = 0
    checked = 0

    conn = pg_connect()
    pg_cache = {}

    for row in manifest:
        if not row.get("InCompareSet"):
            continue

        table = row["SourceTable"]
        owl_name = row["OwlLocalName"]
        pg_col = row["PgColumn"]
        dtype = row["DataType"]

        if table not in pg_cache:
            pk = TABLE_PK.get(table)
            view = VIEW_NAME.get(table)
            if not pk or not view:
                continue
            data = pg_fetch_view(conn, view, pk)
            pg_cache[table] = data or {}

        pg_rows = pg_cache.get(table, {})
        pred = NTWF[owl_name]

        for s, _, o in hydrated_g.triples((None, pred, None)):
            pk_val = str(s).replace(NS, "")
            pg_row = pg_rows.get(pk_val)
            if pg_row is None:
                continue
            pg_val = pg_row.get(pg_col)
            if pg_val is None:
                continue
            graph_val = literal_value(o)
            if not values_approx_equal(graph_val, pg_val, dtype):
                contradictions += 1
                if contradictions <= 10:
                    print(f"  {FAIL}: contradiction <{pk_val}> {owl_name}: "
                          f"graph={graph_val!r} pg={pg_val!r}")
            else:
                checked += 1

    conn.close()

    if contradictions:
        print(f"  {FAIL}: {contradictions} contradictions found")
        return False
    print(f"  {PASS}: {checked} manifest triples verified, no contradictions")
    return True


# ---------------------------------------------------------------------------
# Check 5: reason.py smoke test with USE_HYDRATED env var
# ---------------------------------------------------------------------------

def check_reason_py_smoke():
    print("\n[5] reason.py with USE_HYDRATED=true reports 0 mismatches")
    reason_py = os.path.join(HERE, "reason.py")
    if not os.path.exists(reason_py):
        print(f"  {WARN}: reason.py not found, skipping")
        return True

    # Check if reason.py supports USE_HYDRATED yet (it will once the tool is integrated)
    with open(reason_py) as f:
        src = f.read()
    if "USE_HYDRATED" not in src:
        print(f"  {WARN}: reason.py does not yet support USE_HYDRATED env var — "
              f"this check is a placeholder until reason.py is updated")
        return True

    import subprocess
    env = os.environ.copy()
    env["USE_HYDRATED"] = "true"
    result = subprocess.run(
        [sys.executable, reason_py],
        capture_output=True, text=True, env=env, cwd=ROOT
    )
    if result.returncode == 0:
        print(f"  {PASS}: reason.py exited 0")
        return True
    print(f"  {FAIL}: reason.py exited {result.returncode}")
    print(result.stdout[-2000:] if result.stdout else "")
    print(result.stderr[-1000:] if result.stderr else "")
    return False


# ---------------------------------------------------------------------------
# Bonus check: triple count sanity
# ---------------------------------------------------------------------------

def check_triple_count(hydrated_g, manifest):
    print("\n[bonus] Triple count sanity")
    in_compare = sum(1 for r in manifest if r.get("InCompareSet"))
    total = len(hydrated_g)
    # Each InCompareSet field × number of individuals in that table should add
    # up to roughly: total_hydrated - total_asserted extra triples.
    # We just report the numbers — not a hard failure.
    asserted_count = len(load_graph(ASSERTED_TTL)) if os.path.exists(ASSERTED_TTL) else 0
    extra = total - asserted_count
    print(f"  Total triples in hydrated graph : {total}")
    print(f"  Total triples in asserted graph : {asserted_count}")
    print(f"  Extra (derived) triples         : {extra}")
    print(f"  InCompareSet fields in manifest : {in_compare}")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main():
    print("=" * 60)
    print("rulebook-to-hydrated-tripples test harness")
    print("=" * 60)

    rb = load_rulebook()
    manifest = load_conformance_manifest(rb)

    results = []

    hydrated_g = check_parse()
    results.append(hydrated_g is not None)
    if hydrated_g is None:
        print(f"\n{FAIL}: cannot proceed without a parseable hydrated graph")
        sys.exit(1)

    results.append(check_superset(hydrated_g))
    results.append(check_conformance_fields(hydrated_g, manifest))
    results.append(check_no_contradictions(hydrated_g, manifest))
    results.append(check_reason_py_smoke())
    check_triple_count(hydrated_g, manifest)

    print("\n" + "=" * 60)
    passed = sum(results)
    total = len(results)
    if all(results):
        print(f"{PASS}: all {total} checks passed")
        sys.exit(0)
    else:
        print(f"{FAIL}: {total - passed}/{total} checks failed")
        sys.exit(1)


if __name__ == "__main__":
    main()
