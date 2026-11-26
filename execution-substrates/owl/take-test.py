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

This script runs the SHACL-AF rules to compute the derived values:
1. Loads the generated ontology, individuals and SHACL rules
2. Runs every sh:SPARQLRule once, in ascending sh:order (the injector emits each
   rule's depth in the formula DAG); the shapes of one erb:fixpointGroup are
   repeated until they add nothing
3. Reads every row's cells back and writes them to test-answers/

The computation happens in the SPARQL engine, not in Python code; Python only
sequences the rules. pyshacl was dropped because it evaluates every rule once per
focus node and repeats whole passes until nothing changes, which on a large
rulebook takes hours and never converges within its pass cap.

Nothing here knows how a name was minted. src/manifest.json, which both injectors
write, says which class each table is, which property each field is, and which
property holds a row's key. (Until 2026-11-25 this file carried the namespace as
a constant, https://w3id.org/effortless-ntwf#, and rebuilt every IRI from it.)

When the build also wrote ontology-with-individuals.ttl (the commercial tool's
OWL 2 DL document), a description-logic reasoner reads it too: ROBOT with HermiT,
in Docker. See dl_check().
"""

import json
import math
import os
import re
import shutil
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

# pyoxigraph 0.5.9 and older multiply some xsd:decimals wrongly (an error where
# there is a product). An engine that quietly computes other values is worse than
# none, so an old one is refused.
_ENGINE = tuple(int(re.match(r'\d+', part).group(0)) for part in pyoxigraph.__version__.split('.')[:3])
if _ENGINE < (0, 5, 10):
    raise SystemExit(
        f"pyoxigraph {pyoxigraph.__version__} is too old to run these rules: versions before 0.5.10 "
        f"multiply some decimals wrongly. Upgrade it: python3 -m pip install -U pyoxigraph")

# Add project root to path for shared imports
sys.path.insert(0, str(Path(__file__).resolve().parent.parent.parent))

from orchestration.shared import load_rulebook

# Script directory
script_dir = Path(__file__).parent.resolve()


# =============================================================================
# NAMES
# =============================================================================

SH = Namespace("http://www.w3.org/ns/shacl#")
# The generator's own vocabulary (a row's key, a rule's fixpoint group).
ERB = Namespace("https://effortlessapi.com/ns/erb#")


def load_manifest(src_dir: Path) -> dict:
    """src/manifest.json: the class of each table, the property of each field,
    the property that holds a row's key."""
    path = src_dir / "manifest.json"
    if not path.exists():
        raise SystemExit(
            f"{path} is missing. Both OWL injectors write it since 2026-11-25; these files are "
            f"from an older build. Rebuild the project (for effortless-owl, with the published "
            f"rulebook-to-owl of that date or later).")
    return json.loads(path.read_text(encoding='utf-8'))


# =============================================================================
# UTILITY FUNCTIONS
# =============================================================================

def camel_to_snake(name: str) -> str:
    """Convert CamelCase to snake_case for output compatibility."""
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
    s2 = re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()
    # Normalize consecutive underscores to single underscore
    return re.sub('_+', '_', s2)


def _one_value(values, table_name, field_name, pk_value):
    """The single value of a cell. Rules run in sh:order and only add triples, so
    a cell with two values means a rule fired on an input that was not there yet:
    a defect in the rules, reported, never settled by picking one."""
    distinct = []
    for v in values:
        if v not in distinct:
            distinct.append(v)
    if len(distinct) == 1:
        return distinct[0]
    raise ValueError(
        f"{table_name}.{field_name} (id={pk_value!r}) holds several values {distinct!r}. "
        f"A rule fired before the field it reads was computed; fix the rule's sh:order "
        f"rather than choosing a value.")


def rdf_value_to_python(value, graph=None, key_property=None):
    """Convert an RDF value to Python native type."""
    if value is None:
        return None
    if isinstance(value, URIRef) and graph is not None and key_property is not None:
        # A relationship resolved to an individual: that row's key, verbatim.
        key = graph.value(value, key_property)
        if key is not None:
            return str(key)

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
SUBSELECT = re.compile(r'\bSELECT\b([^{]*?)\bWHERE\b', re.S)
XSD_DOUBLE = pyoxigraph.NamedNode('http://www.w3.org/2001/XMLSchema#double')
MAX_FIXPOINT_PASSES = 10000


def _sqrt(value):
    """XPath math:sqrt, which SQRT() compiles to and Oxigraph does not ship."""
    try:
        number = float(value.value)
    except (AttributeError, TypeError, ValueError):
        return None
    return None if number < 0 else pyoxigraph.Literal(repr(math.sqrt(number)), datatype=XSD_DOUBLE)


CUSTOM_FUNCTIONS = {
    pyoxigraph.NamedNode('http://www.w3.org/2005/xpath-functions/math#sqrt'): _sqrt,
}


def load_sparql_rules(shacl_graph: Graph) -> list:
    """Every sh:SPARQLRule as (order, label, target_class, construct, group, derives),
    sorted by sh:order. Both injectors give every rule an sh:order (its depth in
    the formula DAG); a rule without one cannot be sequenced and is refused."""
    rules = []
    for shape, target in shacl_graph.subject_objects(SH.targetClass):
        group = shacl_graph.value(shape, ERB.fixpointGroup)
        derives = shacl_graph.value(shape, ERB.derives)
        for rule in shacl_graph.objects(shape, SH.rule):
            construct = shacl_graph.value(rule, SH.construct)
            if construct is None:
                continue
            label = shacl_graph.value(rule, RDFS.label)
            # sh:order sits on the rule (this repo's injector: several rules to a
            # shape) or on its shape (rulebook-to-owl: one shape per rule).
            order = shacl_graph.value(rule, SH.order)
            if order is None:
                order = shacl_graph.value(shape, SH.order)
            if order is None:
                raise RuntimeError(f"SHACL rule {label} carries no sh:order; it cannot be sequenced.")
            rules.append((int(order.toPython()), str(label), str(target), str(construct),
                          str(group) if group is not None else None,
                          str(derives) if derives is not None else None))
    rules.sort(key=lambda r: (r[0], r[1]))
    return rules


def _fire(store, rule) -> int:
    """Run one rule once; return how many new triples it added.

    A rule whose every sub-SELECT returns $this (SHACL 5.3.1; the commercial tool
    and, since 2026-11-25, this repo's injector) means the same evaluated once for
    all focus nodes, with $this an ordinary variable: one query. Any other rule
    needs $this pre-bound inside its sub-SELECT, one focus node at a time (SPARQL
    SEP-0007 substitution, which is what a SHACL engine does)."""
    _, label, target, construct, _, _ = rule
    query = THIS_VAR.sub('?this', construct)
    where = construct[construct.find('WHERE'):]
    returns_this = all('$this' in projection for projection in SUBSELECT.findall(where))
    try:
        if returns_this:
            triples = list(store.query(query, custom_functions=CUSTOM_FUNCTIONS))
        else:
            this = pyoxigraph.Variable('this')
            triples = []
            for node in [s['n'] for s in store.query(f'SELECT ?n WHERE {{ ?n a <{target}> }}')]:
                triples.extend(store.query(query, substitutions={this: node}, custom_functions=CUSTOM_FUNCTIONS))
    except SyntaxError as exc:
        raise RuntimeError(f"SHACL rule {label} is not valid SPARQL: {exc}") from exc
    before = len(store)
    store.extend([pyoxigraph.Quad(t.subject, t.predicate, t.object, pyoxigraph.DefaultGraph())
                  for t in triples])
    return len(store) - before


def run_shacl_reasoning(store: "pyoxigraph.Store", rules: list) -> int:
    """Run each SHACL-AF rule once, in sh:order, inserting what it constructs. The
    shapes of one erb:fixpointGroup (fields that depend on themselves through
    other rows) are repeated until a pass adds nothing; their rules mark the rows
    they have computed and wait for the rows they read, so that is exact.

    Returns the number of distinct sh:order levels executed.
    """
    by_order = {}
    for rule in rules:
        by_order.setdefault(rule[0], []).append(rule)
    for order in sorted(by_order):
        groups = {}
        for rule in by_order[order]:
            if rule[4] is None:
                _fire(store, rule)
            else:
                groups.setdefault(rule[4], []).append(rule)
        for name in sorted(groups):
            for _ in range(MAX_FIXPOINT_PASSES):
                if sum(_fire(store, rule) for rule in groups[name]) == 0:
                    break
            else:
                raise RuntimeError(f"fixpoint group {name} still added triples after {MAX_FIXPOINT_PASSES} passes")
            for _, label, target, _, _, derives in groups[name]:
                if derives is None:
                    continue
                waiting = [s['n'].value for s in store.query(
                    f'SELECT ?n WHERE {{ ?n a <{target}> . FILTER NOT EXISTS {{ ?n <{ERB.derived}> <{derives}> }} }}')]
                if waiting:
                    raise RuntimeError(
                        f"fixpoint group {name}: {len(waiting)} row(s) of <{target}> never got <{derives}> "
                        f"({label}): the rows they read include themselves, a cycle in the DATA "
                        f"(first: {', '.join(sorted(waiting)[:5])}).")
    return len(by_order)


def extract_entity_results(
    data_graph: Graph,
    table_name: str,
    schema: list,
    data: list,
    manifest: dict,
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

    # A row's individual is found by its table's class and its key, both named by
    # the manifest: the IRI itself is the injector's business (a slug of the key,
    # with the table in it when two tables share a key).
    names = manifest['tables'].get(table_name)
    if names is None:
        raise ValueError(f"manifest.json has no table {table_name!r}: it was made from another rulebook")
    key_property = URIRef(manifest['keyProperty'])
    pk_field = names['keyField']
    by_key = {}
    for subject in data_graph.subjects(RDF.type, URIRef(names['class'])):
        key = data_graph.value(subject, key_property)
        if key is not None:
            by_key.setdefault(str(key), subject)

    # Extract each individual
    for i, original_row in enumerate(data):
        pk_value = original_row.get(pk_field) if pk_field else None
        if pk_value is None or str(pk_value).strip() == '':
            raise ValueError(
                f"{table_name}[{i}] has no primary-key value in field "
                f"'{pk_field}'; cannot locate its individual in the graph. The "
                f"OWL injector keys every individual by PK — fix the rulebook row."
            )
        ind_uri = by_key.get(str(pk_value))
        if ind_uri is None:
            raise ValueError(
                f"{table_name}[{i}] (id={pk_value!r}) has no individual of <{names['class']}> "
                f"carrying that key: the graph and the rulebook are not the same build.")

        # Start with RAW data only; never carry pre-computed values through.
        record = {}
        for key, value in original_row.items():
            snake_key = camel_to_snake(key)
            if snake_key in raw_snake_keys:
                record[snake_key] = value

        # Query each field from the graph (includes computed values)
        for field_info in all_fields:
            field_name = field_info['name']
            prop_uri = URIRef(names['fields'][field_name])

            objs = list(data_graph.objects(ind_uri, prop_uri))
            if not objs:
                continue
            py_values = [rdf_value_to_python(o, data_graph, key_property) for o in objs]
            snake_key = camel_to_snake(field_name)
            py_value = _one_value(py_values, table_name, field_name, pk_value)

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

def _get_testing_paths(substrate_name: str):
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
    return Path(erb_testing) / "blank-tests", Path(erb_testing) / substrate_name / "test-answers"


ROBOT_IMAGE = os.environ.get('ROBOT_IMAGE', 'obolibrary/robot')
DL_DOCUMENT = 'ontology-with-individuals.ttl'


def _robot(src_dir: Path, *args: str):
    return subprocess.run(['docker', 'run', '--rm', '-v', f'{src_dir.resolve()}:/work', ROBOT_IMAGE, 'robot', *args],
                          capture_output=True, text=True, timeout=3600)


def dl_check(src_dir: Path, report_dir: Path) -> None:
    """Put the build through an ontologist's own tools, and say what they say.

    The rules above are SPARQL; they prove agreement between substrates, not with
    a reasoner an ontologist already trusts. The commercial rulebook-to-owl also
    writes ontology-with-individuals.ttl, the ontology and its rows in one OWL 2
    DL document. ROBOT (the tool OBO ontologies are released with), in its Docker
    image, checks that it IS OWL 2 DL and runs HermiT over it: is it consistent,
    and can every class have a member.

    The verdict is written to dl-check.json beside the answers, and a document
    that is outside OWL 2 DL, inconsistent, or holds a class with no possible
    member fails the run. Where the check cannot run (no document: the
    open-source injector does not write one; no Docker; no image) that is said,
    in the log and in dl-check.json. It is never reported as passed."""
    report_dir.mkdir(parents=True, exist_ok=True)
    report = report_dir / 'dl-check.json'
    document = src_dir / DL_DOCUMENT

    def record(verdict: dict) -> None:
        report.write_text(json.dumps(verdict, indent=2) + '\n', encoding='utf-8')

    print("\nDescription-logic check (ROBOT + HermiT)...")
    why_not = None
    if not document.exists():
        why_not = f"this build wrote no {DL_DOCUMENT} (only the commercial rulebook-to-owl does)"
    elif shutil.which('docker') is None:
        why_not = "docker is not installed"
    elif subprocess.run(['docker', 'image', 'inspect', ROBOT_IMAGE], capture_output=True).returncode != 0:
        why_not = f"the Docker image {ROBOT_IMAGE} is not here (docker pull {ROBOT_IMAGE}), or Docker is not running"
    if why_not:
        print(f"   NOT RUN: {why_not}")
        record({'ran': False, 'reason': why_not})
        return

    _robot(src_dir, 'validate-profile', '--input', f'/work/{DL_DOCUMENT}', '--profile', 'DL',
           '--output', '/work/profile-DL.txt')
    profile_lines = [line for line in (src_dir / 'profile-DL.txt').read_text(encoding='utf-8').splitlines() if line.strip()]
    violations = profile_lines[1:] if profile_lines and 'NOT in profile' in profile_lines[0] else []
    verdict = {'ran': True, 'reasoner': 'HermiT (ROBOT)', 'owl2dl': not violations,
               'dlViolations': violations[:20], 'consistent': None, 'unsatisfiable': []}
    if violations:
        record(verdict)
        print(f"   NOT OWL 2 DL: {len(violations)} violation(s). First: {violations[0][:300]}")
        sys.exit(1)

    reasoned = _robot(src_dir, 'reason', '--reasoner', 'hermit', '--input', f'/work/{DL_DOCUMENT}',
                      '--output', '/work/inferred-hermit.ttl')
    log = reasoned.stdout + reasoned.stderr
    verdict['consistent'] = 'inconsistent' not in log.lower()
    verdict['unsatisfiable'] = sorted(set(re.findall(r'unsatisfiable:\s*(\S+)', log)))
    if not verdict['consistent']:
        _robot(src_dir, 'explain', '--reasoner', 'hermit', '--input', f'/work/{DL_DOCUMENT}',
               '-M', 'inconsistency', '--explanation', '/work/inconsistency.md')
        explanation = src_dir / 'inconsistency.md'
        verdict['explanation'] = explanation.read_text(encoding='utf-8')[:4000] if explanation.exists() else ''
    elif reasoned.returncode != 0 and not verdict['unsatisfiable']:
        record({**verdict, 'error': log[-2000:]})
        print(f"   ROBOT failed: {log[-600:]}")
        sys.exit(1)
    record(verdict)
    if not verdict['consistent']:
        print("   INCONSISTENT under HermiT:")
        print('   ' + '\n   '.join(verdict['explanation'].splitlines()[:14]))
        sys.exit(1)
    if verdict['unsatisfiable']:
        print(f"   {len(verdict['unsatisfiable'])} class(es) can have no member: {', '.join(verdict['unsatisfiable'][:8])}")
        sys.exit(1)
    print("   OWL 2 DL, consistent, every class can have a member")


def main(src_dir: Path, substrate_name: str, missing_hint: str):
    """Reason over the TBox/ABox/SHACL triple in src_dir and write answers for
    substrate_name. The oss `owl` substrate passes its own injector's src/;
    `effortless-owl` passes the domain's rulebook-to-owl output. Both share
    this runner because both mint the same effortless-ntwf: IRIs."""
    print("=" * 70)
    print(f"OWL Execution Substrate ({substrate_name}) - SHACL Reasoning Test")
    print("=" * 70)
    print()

    # Check required files exist. inject-into-owl.py writes the TBox/ABox/SHACL
    # source artifacts under src/ (test-answers.json/test-results.md stay at the
    # substrate root); read from the same place they're written.
    ontology_path = src_dir / "ontology.owl"
    individuals_path = src_dir / "individuals.ttl"
    rules_path = src_dir / "rules.shacl.ttl"

    for path in [ontology_path, individuals_path, rules_path]:
        if not path.exists():
            print(f"ERROR: Required file not found: {path}")
            print(missing_hint)
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
    manifest = load_manifest(src_dir)

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

    _, test_answers_dir = _get_testing_paths(substrate_name)
    test_answers_dir.mkdir(parents=True, exist_ok=True)

    total_records = 0

    for table_name in tables_with_computed:
        table_def = tables[table_name]
        schema = table_def.get('schema', [])
        data = table_def.get('data', [])

        if not schema or not data:
            continue

        records = extract_entity_results(data_graph, table_name, schema, data, manifest)
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

    dl_check(src_dir, test_answers_dir.parent)

    print("\n" + "=" * 70)
    print("SHACL reasoning complete!")
    print("=" * 70)


if __name__ == "__main__":
    main(script_dir / "src", "owl", "Run: python inject-into-owl.py first")
