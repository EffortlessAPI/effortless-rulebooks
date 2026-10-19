#!/usr/bin/env python3
"""
Compile and run the COBOL program rulebook-to-cobol generated in cobol/src/,
against this project's rulebook data, and write what COBOL computed to
cobol/output/<table>.json.

    ./run-cobol.sh            (or: python3 run_cobol.py [--rulebook <path>])

Needs GnuCOBOL (`cobc`): brew install gnucobol / apt-get install gnucobol.

This script only moves data in and out. Every lookup, aggregation and
calculated field in the output was computed by the compiled COBOL program
(src/erb_main.cbl): this script writes the rulebook's raw facts in the
program's interchange format (src/erb_manifest.json gives the table numbers
and field order), runs it, and turns its output back into JSON.
"""

import argparse
import json
import re
import shutil
import subprocess
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
RAW_LIKE_TYPES = {"raw", "relationship"}
_BARE_DATE_RE = re.compile(r'^\d{4}-\d{2}-\d{2}$')
_UNESCAPE = {'t': '\t', 'n': '\n', 'r': '\r', '\\': '\\', 'p': '|'}


def fail(msg: str):
    print(f"run-cobol: {msg}", file=sys.stderr)
    sys.exit(1)


def encode(value, datatype) -> str:
    """One value in the interchange format (see src/erb_runtime_ws.cpy)."""
    if datatype in ('date', 'datetime') and isinstance(value, str) and _BARE_DATE_RE.match(value):
        value = f'{value} 00:00:00+00'
    if value is None:
        return '~'
    if isinstance(value, bool):
        return 'b1' if value else 'b0'
    if isinstance(value, int):
        return f'i{value}'
    if isinstance(value, float):
        return 'f' + (format(value, 'f') if 'e' in repr(value) else repr(value))
    if isinstance(value, (dict, list)):
        value = json.dumps(value)
    out = []
    for ch in str(value):
        if ch == '\\':
            out.append('\\\\')
        elif ch == '|':
            out.append('\\p')
        elif ch in '\t\n\r':
            out.append({'\t': '\\t', '\n': '\\n', '\r': '\\r'}[ch])
        elif ord(ch) < 32:
            out.append('\\x%02X' % ord(ch))
        else:
            out.append(ch)
    return 's' + ''.join(out)


def decode(token: str):
    tag, body = token[:1], token[1:]
    if tag == '~':
        return None
    if tag == 'b':
        return body == '1'
    if tag == 'i':
        return int(body)
    if tag == 'f':
        return float(body)
    if tag == 's':
        out, i = [], 0
        while i < len(body):
            if body[i] == '\\' and i + 1 < len(body):
                if body[i + 1] == 'x':
                    out.append(bytes.fromhex(body[i + 2:i + 4]).decode('latin-1'))
                    i += 4
                    continue
                out.append(_UNESCAPE.get(body[i + 1], body[i + 1]))
                i += 2
            else:
                out.append(body[i])
                i += 1
        return ''.join(out)
    raise ValueError(f"unknown value tag in the COBOL output: {token[:40]!r}")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--rulebook', default=str(HERE.parent / 'effortless-rulebook' / 'effortless-banking-rulebook.json'))
    args = parser.parse_args()

    src = HERE / 'src'
    for name in ('erb_main.cbl', 'erb_runtime.cpy', 'erb_runtime_ws.cpy', 'erb_manifest.json'):
        if not (src / name).exists():
            fail(f"src/{name} is missing. Run `effortless build` in cobol/ first "
                 f"(rulebook-to-cobol v2026.10.18 or later).")
    cobc = shutil.which('cobc')
    if not cobc:
        fail("GnuCOBOL (cobc) is not installed: brew install gnucobol (macOS) or apt-get install gnucobol")

    rulebook = json.loads(Path(args.rulebook).read_text(encoding='utf-8'))
    manifest = json.loads((src / 'erb_manifest.json').read_text(encoding='utf-8'))

    build = HERE / '.build'
    if build.exists():
        shutil.rmtree(build)
    build.mkdir()
    for p in src.iterdir():
        if p.is_file():
            shutil.copy(p, build / p.name)

    # 1. the rulebook's raw facts; every computed field starts NULL
    lines, seeded = [], {}
    for table in manifest['tables']:
        entity = rulebook.get(table['name']) or {}
        schema = {f['name']: f for f in entity.get('schema') or []}
        rows = entity.get('data') or []
        seeded[table['number']] = len(rows)
        for row in rows:
            tokens = [str(table['number'])]
            for field in table['fields']:
                spec = schema.get(field['name'], {})
                if spec.get('type', 'raw') in RAW_LIKE_TYPES:
                    tokens.append(encode(row.get(field['name']), spec.get('datatype')))
                else:
                    tokens.append('~')
            tokens.append('.')
            lines.append('|'.join(tokens))
    (build / 'erb_in.txt').write_text('\n'.join(lines) + '\n', encoding='utf-8')

    # 2. compile and run
    print("run-cobol: compiling src/erb_main.cbl with cobc ...")
    comp = subprocess.run([cobc, '-free', '-x', '-o', 'erb_main', 'erb_main.cbl'], cwd=build,
                          capture_output=True, text=True)
    if comp.returncode != 0:
        fail(f"cobc could not compile erb_main.cbl:\n{(comp.stderr or comp.stdout)[-4000:]}")
    run = subprocess.run(['./erb_main'], cwd=build, capture_output=True, text=True)
    errors = [l.split('\t')[1:] for l in run.stderr.splitlines() if l.startswith('ERBERR\t')]
    status = [l for l in run.stderr.splitlines() if l.strip() and not l.startswith('ERBERR\t')]
    if run.returncode != 0:
        fail(f"erb_main failed (exit {run.returncode}):\n{run.stderr[-3000:]}")

    # 3. what COBOL computed, one JSON file per table
    by_number = {t['number']: t for t in manifest['tables']}
    results = {t['number']: [] for t in manifest['tables']}
    for line in (build / 'erb_out.txt').read_text(encoding='utf-8').split('\n'):
        if not line.strip():
            continue
        tokens = line.split('|')
        if tokens[-1] != '.':
            fail(f"truncated record in the COBOL output: {line[:120]!r}")
        table = by_number[int(tokens[0])]
        results[table['number']].append({f['snake']: decode(v) for f, v in zip(table['fields'], tokens[1:-1])})

    out_dir = HERE / 'output'
    out_dir.mkdir(exist_ok=True)
    for number, rows in results.items():
        table = by_number[number]
        if len(rows) != seeded[number]:
            fail(f"{table['name']}: COBOL returned {len(rows)} rows, expected {seeded[number]}")
        (out_dir / f"{table['snake']}.json").write_text(json.dumps(rows, indent=2) + '\n', encoding='utf-8')
        print(f"run-cobol: {table['name']}: {len(rows)} rows -> output/{table['snake']}.json")
    for line in status:
        print(f"run-cobol: {line}")
    for name, error in ((f"{e[0]}.{e[1]} row {e[2]}", e[3]) for e in errors if len(e) == 4):
        print(f"run-cobol: field error: {name}: {error}", file=sys.stderr)
    refused = manifest.get('compileErrors') or []
    for r in refused:
        print(f"run-cobol: not compiled: {r['table']}.{r['field']}: {r['error']}", file=sys.stderr)


if __name__ == '__main__':
    main()
