#!/usr/bin/env python3
from __future__ import annotations

import json
import sqlite3
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DB = ROOT / 'domains' / 'fermats-last-theorem' / 'source-artifacts' / 'fermat_witness_v21.sqlite'
OUT = ROOT / 'testing' / 'v21-extract'

if not DB.exists():
    raise FileNotFoundError(f'Missing v21 database: {DB}')
OUT.mkdir(parents=True, exist_ok=True)

con = sqlite3.connect(DB)
con.row_factory = sqlite3.Row
try:
    tables = {
        'foundation-kernels.json': 'SELECT * FROM foundation_kernels_v21 ORDER BY kernel_order',
        'proof-trace.json': 'SELECT * FROM proof_trace ORDER BY step_no',
        'loops.json': 'SELECT * FROM loops ORDER BY loop_order',
        'invariants.json': 'SELECT * FROM invariant_checks ORDER BY tier,check_id',
        'sources.json': 'SELECT * FROM sources ORDER BY source_id',
    }
    for filename, sql in tables.items():
        rows = [dict(row) for row in con.execute(sql).fetchall()]
        (OUT / filename).write_text(json.dumps(rows, indent=2) + '\n', encoding='utf-8')
finally:
    con.close()

print(f'Extracted v21 semantic tables to {OUT}')
