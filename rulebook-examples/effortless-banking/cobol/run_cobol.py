#!/usr/bin/env python3
"""
Compile and run the COBOL rulebook-to-cobol generated in cobol/src/ against
this project's rulebook data, and write what COBOL computed to
cobol/output/<table>.json.

    ./run-cobol.sh            (or: python3 run_cobol.py [--rulebook <path>] [--now ISO])

Needs GnuCOBOL (`cobc`): brew install gnucobol / apt-get install gnucobol.
This script only moves data in and out (erb_cobol.py); every lookup,
aggregation and calculated field in the output was computed by the compiled
COBOL (src/ERBRULES.cbl, called by the batch driver src/ERBBATCH.cbl).
"""

import argparse
import json
import sys
from pathlib import Path

from erb_cobol import CobolError, HERE, run


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--rulebook', default=str(HERE.parent / 'effortless-rulebook' / 'effortless-banking-rulebook.json'))
    parser.add_argument('--now', default='', help='pin NOW()/TODAY() (ISO-8601); default: the clock')
    args = parser.parse_args()
    rulebook = json.loads(Path(args.rulebook).read_text(encoding='utf-8'))
    try:
        tables, info = run(rulebook, args.now)
    except CobolError as exc:
        print(f"run-cobol: {exc}", file=sys.stderr)
        sys.exit(1)
    out_dir = HERE / 'output'
    out_dir.mkdir(exist_ok=True)
    for snake, rows in tables.items():
        (out_dir / f'{snake}.json').write_text(json.dumps(rows, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
        print(f"run-cobol: {snake}: {len(rows)} rows -> output/{snake}.json")
    print(f"run-cobol: {info['status']} (compile {info['compile']}s, run {info['run']}s)")
    for line in info['errors']:
        print(f"run-cobol: field error: {line}", file=sys.stderr)
    for r in info['compileErrors']:
        print(f"run-cobol: not compiled: {r['table']}.{r['field']}: {r['error']}", file=sys.stderr)


if __name__ == '__main__':
    main()
