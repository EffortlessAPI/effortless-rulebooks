#!/usr/bin/env python3
from __future__ import annotations

import hashlib
import json
import sqlite3
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open('rb') as f:
        while True:
            chunk = f.read(1024 * 1024)
            if not chunk:
                break
            h.update(chunk)
    return h.hexdigest()


def require(path: Path) -> Path:
    if not path.exists():
        raise FileNotFoundError(f'Required path does not exist: {path}')
    return path


rulebook_path = require(ROOT / 'effortless-rulebook' / 'effortless-math-rulebook.json')
rulebook = json.loads(rulebook_path.read_text(encoding='utf-8'))

for key in ['$schema', 'Name', 'Description', 'Theorems', 'TheoremDependencies', '__meta__']:
    if key not in rulebook:
        raise ValueError(f'Missing top-level rulebook key: {key}')

for name, value in rulebook.items():
    if name in {'$schema', 'Name', 'Description'}:
        continue
    if not isinstance(value, dict) or set(value) != {'Description', 'schema', 'data'}:
        raise ValueError(f'Table {name} does not have exactly Description/schema/data')
    if not isinstance(value['schema'], list) or not isinstance(value['data'], list):
        raise ValueError(f'Table {name} schema/data must be lists')

theorem_rows = rulebook['Theorems']['data']
theorem_ids = {row['TheoremId'] for row in theorem_rows}
# 8 migrated v21 theorems (FLT + 7 foundation providers), the 4 fully internalized
# natural-number-arithmetic theorems (+, -, *, /) added in loops 572-575, and the
# 1 gate-level bit-calculator provider theorem (a second, disjoint zero-import
# witness of the same four flagship facts).
if len(theorem_ids) != 13:
    raise ValueError(f'Expected 13 theorem IDs, found {len(theorem_ids)}')

dependencies = rulebook['TheoremDependencies']['data']
for dependency in dependencies:
    if dependency['ConsumerTheoremId'] not in theorem_ids:
        raise ValueError(f"Unknown consumer theorem: {dependency['ConsumerTheoremId']}")
    if dependency['ProviderTheoremId'] not in theorem_ids:
        raise ValueError(f"Unknown provider theorem: {dependency['ProviderTheoremId']}")
    require(ROOT / dependency['ProviderContractPath'])

# The 7 LOAD-BEARING dependencies are still exactly the FLT foundation providers;
# nothing above the Peano or logic-gate kernels is load-bearing. The bit-calculator
# adds 4 non-load-bearing CORROBORATING-WITNESS dependencies onto the four Peano
# theorems -- these do not change any consumer's zero-import status.
load_bearing = [d for d in dependencies if d['LoadBearing']]
witness = [d for d in dependencies if not d['LoadBearing']]
if len(load_bearing) != 7:
    raise ValueError(f'Expected 7 load-bearing FLT dependencies, found {len(load_bearing)}')
if len(witness) != 4:
    raise ValueError(f'Expected 4 corroborating-witness dependencies, found {len(witness)}')
for d in witness:
    if d['IsImported'] or d['IsSharedKernel']:
        raise ValueError(f"Witness dependency {d['DependencyId']} must not be imported/shared-kernel")

for theorem in theorem_rows:
    require(ROOT / theorem['ContractPath'])
    if theorem['ProofStatusId'] == 'FULLY_INTERNALIZED_FOR_SCOPE' and theorem['ActiveImportCount'] > 0:
        raise ValueError(f"Theorem {theorem['TheoremId']} is fully internalized but still has imports")

config = json.loads(require(ROOT / 'effortless.json').read_text(encoding='utf-8'))
transpiler_names = {row['Name'] for row in config['ProjectTranspilers']}
if 'rulebooktorulespeak' not in transpiler_names:
    raise ValueError('effortless.json is missing required rulebooktorulespeak transpiler')

expected = json.loads(require(ROOT / 'testing' / 'expected-state.json').read_text(encoding='utf-8'))
archive = require(ROOT / 'source-artifacts' / 'fermat_witness_lab_v21.zip')
sqlite_path = require(ROOT / 'domains' / 'fermats-last-theorem' / 'source-artifacts' / 'fermat_witness_v21.sqlite')
if sha256(archive) != expected['V21ArchiveSha256']:
    raise ValueError('v21 archive SHA-256 mismatch')
if sha256(sqlite_path) != expected['V21SqliteSha256']:
    raise ValueError('v21 SQLite SHA-256 mismatch')

con = sqlite3.connect(sqlite_path)
try:
    observed = {
        'V21LoopCount': con.execute('SELECT COUNT(*) FROM loops').fetchone()[0],
        'V21ProofFactCount': con.execute('SELECT COUNT(*) FROM proof_trace').fetchone()[0],
        'V21InvariantCount': con.execute('SELECT COUNT(*) FROM invariant_checks').fetchone()[0],
        'V21ContradictionCount': con.execute('SELECT COUNT(*) FROM proof_trace WHERE contradiction=1').fetchone()[0],
        'V21ActiveImportCount': con.execute('SELECT COUNT(*) FROM import_surface WHERE load_bearing=1').fetchone()[0],
    }
finally:
    con.close()

for key, value in observed.items():
    if value != expected[key]:
        raise ValueError(f'{key}: expected {expected[key]}, observed {value}')

critical_failures = [
    row for row in rulebook['InvariantChecks']['data']
    if row['Status'] == 'FAIL' or row['FailCount'] > 0
]
if critical_failures:
    raise ValueError(f'Rulebook contains invariant failures: {critical_failures[:3]}')

print('Effortless Math starter validation: PASS')
print(f"Theorems: {len(theorem_ids)}")
print(f"Load-bearing FLT provider dependencies: {len(load_bearing)}")
print(f"Corroborating-witness dependencies (bit-calculator): {len(witness)}")
print(f"v21 loops/proof facts/invariants: {observed['V21LoopCount']}/{observed['V21ProofFactCount']}/{observed['V21InvariantCount']}")
print(f"v21 contradiction rows: {observed['V21ContradictionCount']}")
print(f"v21 active foundation kernels: {observed['V21ActiveImportCount']}")
