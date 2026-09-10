#!/usr/bin/env python3
"""
Take Test - OWL Execution Substrate

GUARD: This substrate must compute calculated fields using its own native
engine: the generated SHACL-AF SPARQL rules, executed by a SPARQL 1.1 engine
(pyoxigraph). It must NOT import python_only_erb_simulator or call
compute_lookups / compute_aggregations from any orchestration helper. Fields it
cannot compute natively (SHACL rules that fail to compile, or formula classes
the rulebook-to-owl translator doesn't yet support) must be left null and
counted as failures by the grader.

This script executes the SHACL-AF rules to compute derived values:
1. Loads the generated ontology, individuals and SHACL rules
2. Executes every sh:SPARQLRule once, in ascending sh:order (the injector emits
   each rule's depth in the formula DAG), with $this bound to each focus node
3. Extracts results to test-answers/ directory

The computation happens in the SPARQL engine, not in Python code; Python only
sequences the rules. pyshacl was dropped because it evaluates every rule once per
focus node and repeats whole passes until nothing changes, which on a large
rulebook takes hours and never converges within its pass cap.
"""

import json
import os
import re
import subprocess
import sys
from pathlib import Path

# Auto-install dependencies if needed
def ensure_dependencies():
    """Install required packages if not present."""
    try:
        import rdflib
        import pyoxigraph
    except ImportError:
        print("Installing dependencies...")
        subprocess.check_call([
            sys.executable, "-m", "pip", "install",
            "rdflib", "pyoxigraph", "--quiet"
        ])

ensure_dependencies()

from rdflib import Graph, Namespace, Literal, URIRef
from rdflib.namespace import RDF, RDFS, XSD
import pyoxigraph

# Add project root to path for shared imports
sys.path.insert(0, str(Path(__file__).resolve().parent.parent.parent))

from orchestration.shared import load_rulebook

# Script directory
script_dir = Path(__file__).parent.resolve()


# =============================================================================
# NAMESPACES
# =============================================================================

# Ontology namespace — MUST match inject-into-owl.py's ONT_NS exactly. The
# injector mints every individual as effortless-ntwf:<pk-slug>; the extractor
# rebuilds the SAME IRI to read computed values back off it. If these drift, the
# graph lookups silently return None and every computed field reads blank — so
# this is a single, shared contract, not two independent constants.
NTWF = Namespace("https://w3id.org/effortless-ntwf#")
SH = Namespace("http://www.w3.org/ns/shacl#")


# The extractor must rebuild the exact IRI the injector minted, so it uses the
# injector's own minting function rather than a copy that could drift.
import importlib.util as _ilu
_inj_spec = _ilu.spec_from_file_location("owl_injector", script_dir / "inject-into-owl.py")
owl_injector = _ilu.module_from_spec(_inj_spec)
_inj_spec.loader.exec_module(owl_injector)


def individual_uri(table_name: str, pk_value) -> URIRef:
    local = owl_injector.individual_iri(table_name, pk_value)[len(owl_injector.NS):]
    return NTWF[local]


def _primary_key_field(schema: list):
    """The PK field = the first raw column (mirror inject-into-owl.py:get_pk_field)."""
    for col in schema:
        if col.get('type', 'raw') == 'raw':
            return col.get('name')
    return schema[0].get('name') if schema else None


# =============================================================================
# UTILITY FUNCTIONS
# =============================================================================

def field_to_property_uri(field_name: str) -> str:
    """Convert field name to property URI (camelCase) - must match injector."""
    if field_name:
        return field_name[0].lower() + field_name[1:]
    return 'unknown'


def camel_to_snake(name: str) -> str:
    """Convert CamelCase to snake_case for output compatibility."""
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
    s2 = re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()
    # Normalize consecutive underscores to single underscore
    return re.sub('_+', '_', s2)


def _resolve_multipass_value(values, field_info, table_name, field_name, pk_value):
    """Pick the converged value when a property carries several (multi-pass).

    The reasoner adds, never replaces, so an aggregation that depends on a
    derived child field leaves a trail: a premature value (counted before the
    dependency materialized) plus the converged value. We resolve by the field's
    convergence semantics, NOT by an arbitrary pick:

      • aggregation / calculated NUMERIC → the converged value is the MAX. A
        COUNT/SUM is monotonic non-decreasing as its dependency rows fill in
        across passes, so the largest is the fixpoint.
      • everything else → distinct multiplicity is unexpected; raise loudly
        rather than silently choosing one (CLAUDE.md "Avoid Silent Fallbacks").
    """
    distinct = []
    for v in values:
        if v not in distinct:
            distinct.append(v)
    if len(distinct) == 1:
        return distinct[0]

    ftype = field_info.get('type')
    numeric = [v for v in distinct if isinstance(v, (int, float)) and not isinstance(v, bool)]
    if ftype in ('aggregation', 'calculated') and len(numeric) == len(distinct):
        return max(numeric)

    raise ValueError(
        f"{table_name}.{field_name} (id={pk_value!r}) has conflicting multi-pass "
        f"values {distinct!r} that cannot be converged by a known rule. The SHACL "
        f"rule likely fired on a not-yet-stable dependency; fix the rule or the "
        f"dependency ordering rather than picking a value arbitrarily."
    )


def rdf_value_to_python(value):
    """Convert an RDF value to Python native type."""
    if value is None:
        return None

    if isinstance(value, Literal):
        py_val = value.toPython()
        if isinstance(py_val, bool):
            return py_val
        if isinstance(py_val, (int, float)):
            return py_val
        return str(py_val)

    if isinstance(value, URIRef):
        # An FK / lookup that resolved to an individual comes back as that
        # individual's IRI (effortless-ntwf:<pk> = https://w3id.org/effortless-ntwf#<pk>).
        # The conformance answer is the bare PK — what Postgres stores in the
        # FK text column — so strip the namespace to the local name. This is the
        # OWL analogue of selecting the id column, not the row's IRI. (Robust to
        # any namespace: splits on the last '#' or '/'.)
        iri = str(value)
        local = re.split(r'[#/]', iri)[-1]
        return local

    return str(value)


# =============================================================================
# SHACL REASONING
# =============================================================================

THIS_VAR = re.compile(r'\$this\b')


def load_sparql_rules(shacl_graph: Graph) -> list:
    """Every sh:SPARQLRule as (order, label, target_class, construct), sorted by sh:order."""
    rules = []
    for shape, target in shacl_graph.subject_objects(SH.targetClass):
        for rule in shacl_graph.objects(shape, SH.rule):
            construct = shacl_graph.value(rule, SH.construct)
            if construct is None:
                continue
            order = shacl_graph.value(rule, SH.order)
            label = shacl_graph.value(rule, RDFS.label)
            rules.append((int(order.toPython()) if order is not None else 0,
                          str(label), str(target), str(construct)))
    rules.sort(key=lambda r: (r[0], r[1]))
    return rules


def run_shacl_reasoning(store: "pyoxigraph.Store", rules: list) -> int:
    """Execute each SHACL-AF rule once, in sh:order, inserting what it constructs.

    SHACL-AF pre-binds $this to the focus node everywhere in the query, including
    inside sub-SELECTs. A rule without a sub-SELECT is equivalent to one query with
    $this as a free variable (its WHERE starts with `$this a <targetClass>`); a rule
    WITH one must be evaluated per focus node, or its aggregate stops being
    correlated with the row and counts the whole table.

    Returns the number of distinct sh:order levels executed.
    """
    for order, label, target, construct in rules:
        if re.search(r'WHERE\s*\{.*\bSELECT\b', construct, re.S):
            focus = [s['n'] for s in store.query(f'SELECT ?n WHERE {{ ?n a <{target}> }}')]
            triples = []
            for node in focus:
                triples.extend(store.query(THIS_VAR.sub(f'<{node.value}>', construct)))
        else:
            triples = list(store.query(THIS_VAR.sub('?this', construct)))
        store.extend([pyoxigraph.Quad(t.subject, t.predicate, t.object, pyoxigraph.DefaultGraph())
                      for t in triples])
    return len({r[0] for r in rules})


def extract_entity_results(
    data_graph: Graph,
    table_name: str,
    schema: list,
    data: list
) -> list:
    """
    Extract computed results from RDF graph for a single entity type.

    Returns list of records with all fields (raw + computed) in snake_case.
    """
    records = []

    # Build field info map and partition into raw vs computed.
    # Conformance-cleanup fix 2026-05-12: only RAW field values are carried
    # forward from the rulebook's data array. Computed/lookup/aggregation
    # fields must come from the RDF graph (i.e. from SHACL inference) or
    # remain absent — otherwise we were silently passing the answer key
    # through the supposed reasoner.
    all_fields = []
    raw_snake_keys = set()
    for col in schema:
        col_type = col.get('type', 'raw')
        snake_name = camel_to_snake(col.get('name', ''))
        all_fields.append({
            'name': col.get('name', ''),
            'datatype': col.get('datatype', 'string'),
            'type': col_type,
        })
        if col_type == 'raw':
            raw_snake_keys.add(snake_name)

    # The individual's IRI is keyed by PRIMARY KEY (first raw column), matching
    # the injector. We rebuild effortless-ntwf:<pk-slug> per row to read its
    # computed values back. A row with no PK value would yield a fabricated IRI
    # that matches nothing — surface it loudly rather than silently reading None.
    pk_field = _primary_key_field(schema)

    # Extract each individual
    for i, original_row in enumerate(data):
        pk_value = original_row.get(pk_field) if pk_field else None
        if pk_value is None or str(pk_value).strip() == '':
            raise ValueError(
                f"{table_name}[{i}] has no primary-key value in field "
                f"'{pk_field}'; cannot locate its individual in the graph. The "
                f"OWL injector keys every individual by PK — fix the rulebook row."
            )
        ind_uri = individual_uri(table_name, pk_value)

        # Start with RAW data only; never carry pre-computed values through.
        record = {}
        for key, value in original_row.items():
            snake_key = camel_to_snake(key)
            if snake_key in raw_snake_keys:
                record[snake_key] = value

        # Query each field from the graph (includes computed values)
        for field_info in all_fields:
            field_name = field_info['name']
            prop_name = field_to_property_uri(field_name)
            prop_uri = NTWF[prop_name]

            # A computed field can carry MULTIPLE values when its SHACL rule
            # fired across several reasoning passes: an aggregation that depends
            # on a derived CHILD field counts 0 in the pass before the child's
            # value materialized, then the true count in a later pass. SHACL
            # CONSTRUCT only ADDS triples, so both the stale 0 and the converged
            # value coexist. data_graph.value() would pick one arbitrarily — and
            # picking the stale 0 is exactly the bug. Resolve multiplicity by the
            # field's convergence semantics instead of guessing.
            objs = list(data_graph.objects(ind_uri, prop_uri))
            if not objs:
                continue
            py_values = [rdf_value_to_python(o) for o in objs]
            snake_key = camel_to_snake(field_name)

            if len(py_values) == 1:
                py_value = py_values[0]
            else:
                py_value = _resolve_multipass_value(
                    py_values, field_info, table_name, field_name, pk_value)

            if py_value is not None:
                record[snake_key] = py_value

        # Normalize empty strings to None for all string fields
        # (matches semantic intent - empty string means "no value")
        for key, value in record.items():
            if value == "":
                record[key] = None

        records.append(record)

    return records


# =============================================================================
# MAIN
# =============================================================================

def _get_testing_paths():
    """Resolve blank-tests and test-answers dirs. ERB_TESTING_DIR is required.

    There is no implicit per-substrate testing dir — running with no env var
    silently mixed results across domains. The orchestrator MUST set
    ERB_TESTING_DIR before invoking this substrate.
    """
    erb_testing = os.environ.get("ERB_TESTING_DIR")
    if not erb_testing:
        raise RuntimeError(
            "ERB_TESTING_DIR is not set. take-test.py must be invoked by the "
            "orchestrator with ERB_TESTING_DIR pointing at the active domain's "
            "testing/ directory."
        )
    substrate_name = Path(script_dir).name
    return Path(erb_testing) / "blank-tests", Path(erb_testing) / substrate_name / "test-answers"


def main():
    print("=" * 70)
    print("OWL Execution Substrate - SHACL Reasoning Test")
    print("=" * 70)
    print()

    # Check required files exist. inject-into-owl.py writes the TBox/ABox/SHACL
    # source artifacts under src/ (test-answers.json/test-results.md stay at the
    # substrate root); read from the same place they're written.
    ontology_path = script_dir / "src" / "ontology.owl"
    individuals_path = script_dir / "src" / "individuals.ttl"
    rules_path = script_dir / "src" / "rules.shacl.ttl"

    for path in [ontology_path, individuals_path, rules_path]:
        if not path.exists():
            print(f"ERROR: Required file not found: {path}")
            print("Run: python inject-into-owl.py first")
            sys.exit(1)

    # Load rulebook to get schema info
    print("Loading rulebook...")
    try:
        rulebook = load_rulebook()
    except FileNotFoundError as e:
        print(f"ERROR: {e}")
        sys.exit(1)

    # Filter to just table definitions
    tables = {k: v for k, v in rulebook.items()
              if isinstance(v, dict) and 'schema' in v}

    # Identify which tables have computed columns
    tables_with_computed = {}
    for table_name, table_def in tables.items():
        if table_name.startswith('_') or table_name.startswith('$'):
            continue
        schema = table_def.get('schema', [])
        computed_cols = [c['name'] for c in schema if c.get('formula')]
        if computed_cols:
            tables_with_computed[table_name] = computed_cols

    print(f"Tables with computed columns: {', '.join(tables_with_computed.keys())}")
    owl_injector.register_pk_collisions(tables)

    # Load ontology + individuals into a single store
    print("\nLoading ontology and data...")
    store = pyoxigraph.Store()
    for path in (ontology_path, individuals_path):
        store.load(path=str(path), format=pyoxigraph.RdfFormat.TURTLE)
        print(f"   Loaded: {path}")
    print(f"   Total triples: {len(store)}")

    # Load SHACL rules
    print("\nLoading SHACL rules...")
    shacl_graph = Graph()
    shacl_graph.parse(rules_path, format='turtle')
    rules = load_sparql_rules(shacl_graph)
    print(f"   Loaded: {rules_path} ({len(rules)} rules)")

    print("\nExecuting SHACL-AF rules in sh:order...")
    try:
        levels = run_shacl_reasoning(store, rules)
        print(f"   Completed {len(rules)} rules across {levels} sh:order levels")
        print(f"   Final triple count: {len(store)}")
    except Exception as e:
        print(f"   ERROR: SHACL rule execution failed: {e}")
        sys.exit(1)

    data_graph = Graph()
    data_graph.parse(data=store.dump(format=pyoxigraph.RdfFormat.N_TRIPLES,
                                     from_graph=pyoxigraph.DefaultGraph()),
                     format='nt')

    # Extract results and save to test-answers/
    print("\nExtracting computed values...")

    _, test_answers_dir = _get_testing_paths()
    test_answers_dir.mkdir(parents=True, exist_ok=True)

    total_records = 0

    for table_name in tables_with_computed:
        table_def = tables[table_name]
        schema = table_def.get('schema', [])
        data = table_def.get('data', [])

        if not schema or not data:
            continue

        records = extract_entity_results(data_graph, table_name, schema, data)
        total_records += len(records)

        # Convert table name to snake_case for filename
        filename = camel_to_snake(table_name) + ".json"
        output_path = test_answers_dir / filename

        with open(output_path, "w", encoding='utf-8') as f:
            json.dump(records, f, indent=2)

        computed_cols = tables_with_computed[table_name]
        print(f"   {table_name}: {len(records)} records ({len(computed_cols)} computed fields)")

    print(f"\nTotal: {total_records} records extracted")
    print(f"Output: {test_answers_dir}/")

    print("\n" + "=" * 70)
    print("SHACL reasoning complete!")
    print("=" * 70)


if __name__ == "__main__":
    main()
