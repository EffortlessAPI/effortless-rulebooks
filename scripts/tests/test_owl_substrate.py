#!/usr/bin/env python3
"""The open-source OWL substrate names an ontology after ITS rulebook, writes
rules every SHACL engine reads the same way, and is graded without anyone
rebuilding a name.

Until 2026-11-25 every rulebook's classes, properties and individuals were minted
in https://w3id.org/effortless-ntwf#, the namespace of the first rulebook the
injector was written for, and the runner carried the same constant. A rollup's
sub-SELECT read $this without returning it (against SHACL 5.3.1), which is right
only in an engine that pre-binds $this one focus node at a time.

Run: python3 scripts/tests/test_owl_substrate.py     (needs rdflib and pyoxigraph >= 0.5.10)
"""
import importlib.util
import json
import os
import re
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
INJECTOR = ROOT / 'execution-substrates' / 'owl' / 'inject-into-owl.py'
RUNNER = ROOT / 'execution-substrates' / 'owl' / 'take-test.py'
OLD = 'effortless-ntwf'

BOOK = {
    'Name': 'Car Sales',
    'Description': 'Dealers and the cars they hold.',
    'Dealers': {
        'Description': 'Dealers.',
        'schema': [
            {'name': 'DealerId', 'datatype': 'string', 'type': 'raw', 'nullable': False},
            {'name': 'Name', 'datatype': 'string', 'type': 'calculated', 'formula': '={{Title}}'},
            {'name': 'Title', 'datatype': 'string', 'type': 'raw'},
            {'name': 'CarCount', 'datatype': 'integer', 'type': 'aggregation',
             'formula': '=COUNTIFS(Cars!{{Dealer}}, Dealers!{{DealerId}})'},
        ],
        'data': [{'DealerId': 'd1', 'Title': 'North Cars'}, {'DealerId': 'd2', 'Title': 'Sud Auto'},
                 {'DealerId': 'd3', 'Title': 'Empty Lot'}],
    },
    'Cars': {
        'Description': 'Cars.',
        'schema': [
            {'name': 'CarId', 'datatype': 'string', 'type': 'raw', 'nullable': False},
            {'name': 'Name', 'datatype': 'string', 'type': 'calculated', 'formula': '={{Make}} & " " & {{Model}}'},
            {'name': 'Make', 'datatype': 'string', 'type': 'raw'},
            {'name': 'Model', 'datatype': 'string', 'type': 'raw'},
            {'name': 'Dealer', 'datatype': 'string', 'type': 'relationship', 'RelatedTo': 'Dealers'},
        ],
        'data': [{'CarId': 'c1', 'Make': 'Saab', 'Model': '900', 'Dealer': 'd1'},
                 {'CarId': 'c2', 'Make': 'Volvo', 'Model': '240', 'Dealer': 'd1'},
                 {'CarId': 'c3', 'Make': 'Fiat', 'Model': 'Panda', 'Dealer': 'd2'}],
    },
}
META = {
    'Description': 'Project metadata.',
    'schema': [{'name': 'MetaKey', 'datatype': 'string', 'type': 'raw'},
               {'name': 'Name', 'datatype': 'string', 'type': 'calculated', 'formula': '={{MetaKey}}'},
               {'name': 'ValueType', 'datatype': 'string', 'type': 'raw'},
               {'name': 'StringValue', 'datatype': 'string', 'type': 'raw'},
               {'name': 'JsonValue', 'datatype': 'string', 'type': 'raw'}],
    'data': [{'MetaKey': 'ontology_iri', 'ValueType': 'string', 'StringValue': 'https://cars.example.com/ontology'},
             {'MetaKey': 'ontology_prefix', 'ValueType': 'string', 'StringValue': 'cars'}],
}


def build(book, tmp):
    """Run the injector, then the runner, on one rulebook; return (src dir, answers dir)."""
    rulebook = Path(tmp) / 'rulebook.json'
    rulebook.write_text(json.dumps(book), encoding='utf-8')
    env = dict(os.environ, ERB_RULEBOOK_PATH=str(rulebook), ERB_OUTPUT_DIR=tmp,
               ERB_TESTING_DIR=str(Path(tmp) / 'testing'), ERB_DOMAIN='test')
    proc = subprocess.run([sys.executable, str(INJECTOR)], env=env, cwd=tmp, capture_output=True, text=True)
    assert proc.returncode == 0, proc.stderr[-800:] + proc.stdout[-400:]
    code = ("import importlib.util, sys; from pathlib import Path;"
            f"s = importlib.util.spec_from_file_location('t', r'{RUNNER}');"
            "m = importlib.util.module_from_spec(s); s.loader.exec_module(m);"
            f"m.main(Path(r'{tmp}') / 'src', 'owl', 'x')")
    proc = subprocess.run([sys.executable, '-c', code], env=env, cwd=tmp, capture_output=True, text=True)
    assert proc.returncode == 0, proc.stderr[-800:] + proc.stdout[-400:]
    return Path(tmp) / 'src', Path(tmp) / 'testing' / 'owl'


def test_a_rulebook_gets_a_namespace_of_its_own():
    with tempfile.TemporaryDirectory() as tmp:
        src, answers = build(BOOK, tmp)
        text = ''.join((src / name).read_text(encoding='utf-8')
                       for name in ('ontology.owl', 'individuals.ttl', 'rules.shacl.ttl', 'manifest.json'))
        assert OLD not in text, "another rulebook's namespace is in the output"
        manifest = json.loads((src / 'manifest.json').read_text(encoding='utf-8'))
        assert manifest['ontology']['namespace'] == 'https://example.org/car-sales#'
        assert manifest['tables']['Cars']['class'] == 'https://example.org/car-sales#Cars'
        assert '<https://example.org/car-sales> a owl:Ontology' in (src / 'ontology.owl').read_text(encoding='utf-8')
        assert 'NTWF' not in text


def test_the_rulebook_can_name_its_ontology():
    with tempfile.TemporaryDirectory() as tmp:
        src, _ = build({**BOOK, '__meta__': META}, tmp)
        manifest = json.loads((src / 'manifest.json').read_text(encoding='utf-8'))
        assert manifest['ontology'] == {'iri': 'https://cars.example.com/ontology',
                                        'namespace': 'https://cars.example.com/ontology#', 'prefix': 'cars'}
        assert 'cars:Dealers' in (src / 'rules.shacl.ttl').read_text(encoding='utf-8')


def test_every_subselect_returns_this_and_the_rollup_is_right():
    with tempfile.TemporaryDirectory() as tmp:
        src, answers = build(BOOK, tmp)
        rules = (src / 'rules.shacl.ttl').read_text(encoding='utf-8')
        projections = re.findall(r'\bSELECT\b([^{]*?)\bWHERE\b', rules, re.S)
        assert projections and all('$this' in p for p in projections), projections
        dealers = {r['dealer_id']: r for r in json.loads((answers / 'test-answers' / 'dealers.json').read_text())}
        assert [dealers[d]['car_count'] for d in ('d1', 'd2', 'd3')] == [2, 1, 0]
        assert dealers['d1']['name'] == 'North Cars'
        cars = {r['car_id']: r for r in json.loads((answers / 'test-answers' / 'cars.json').read_text())}
        assert cars['c1']['name'] == 'Saab 900'
        verdict = json.loads((answers / 'dl-check.json').read_text())
        assert verdict == {'ran': False, 'reason': verdict['reason']} and 'ontology-with-individuals.ttl' in verdict['reason'], \
            "the open-source build writes no DL document, and the check says it did not run"


def test_the_examples_whose_apps_read_a_namespace_declare_it():
    # Their reasoner scripts carry https://w3id.org/effortless-ntwf# as a constant.
    for project, script in (
            ('talismans-special-solutions', 'app/backend/reasoner/reason.py'),
            ('causal-autoimmune-architecture', 'owl/reason.py'),
            ('simpsons-paradox', 'owl/reason.py')):
        folder = ROOT / 'rulebook-examples' / project
        assert 'https://w3id.org/effortless-ntwf#' in (folder / script).read_text(encoding='utf-8')
        rulebooks = [p for p in (folder / 'effortless-rulebook').glob('*.json') if '.derived-' not in p.name]
        book = json.loads(max(rulebooks, key=lambda p: p.stat().st_size).read_text(encoding='utf-8'))
        rows = {r['MetaKey']: (r.get('StringValue') or r.get('Value')) for r in book['__meta__']['data']}
        assert rows.get('ontology_iri') == 'https://w3id.org/effortless-ntwf', project
        assert rows.get('ontology_prefix') == 'effortless-ntwf', project


if __name__ == '__main__':
    for name, fn in list(globals().items()):
        if name.startswith('test_') and callable(fn):
            fn()
            print('ok', name)
