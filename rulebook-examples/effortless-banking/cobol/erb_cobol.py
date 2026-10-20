"""
erb_cobol.py - build and run the COBOL that rulebook-to-cobol generated in src/.

Shared by run_cobol.py and side_by_side.py. It only moves data in and out:

  1. the rulebook's raw facts become one fixed-length record file per table
     (DD ERBInn), laid out the way src/erb_manifest.json describes (the same
     layout as the copybooks in src/); every computed field starts NULL;
  2. src/ERBBATCH.cbl and src/ERBRULES.cbl are compiled with the command in
     the manifest and ERBBATCH runs;
  3. the ERBOnn files it writes are read back into rows.

Every lookup, aggregation and calculated field in those rows was computed by
the compiled COBOL. Record layout per field (erb_manifest.json storage):
  ni     PIC X                    "Y" = NULL
  len    PIC S9(4) COMP           text length, 2-byte big-endian
  value  PIC X(n) text | PIC X "Y"/"N" boolean | PIC S9(d)V9(s) COMP-3 packed
"""

import json
import re
import shutil
import subprocess
import tempfile
import time
from decimal import Decimal, ROUND_DOWN, ROUND_HALF_UP
from pathlib import Path

HERE = Path(__file__).resolve().parent
RAW_LIKE_TYPES = {'raw', 'relationship'}
_BARE_DATE = re.compile(r'^\d{4}-\d{2}-\d{2}$')


class CobolError(RuntimeError):
    pass


def _pack(value: Decimal, digits: int, scale: int) -> bytes:
    q = int((value * Decimal(10) ** scale).to_integral_value(rounding=ROUND_DOWN))
    if q != value * Decimal(10) ** scale:
        raise CobolError(f"{value} has more than {scale} decimal places")
    text = str(abs(q))
    if len(text) > digits + scale:
        raise CobolError(f"{value} does not fit S9({digits})V9({scale})")
    nibbles = text.rjust(digits + scale, '0') + ('D' if q < 0 else 'C')
    return bytes.fromhex(('0' + nibbles) if len(nibbles) % 2 else nibbles)


def _unpack(data: bytes, scale: int) -> Decimal:
    nibbles = data.hex().upper()
    value = Decimal(int(nibbles[:-1] or '0')) / Decimal(10) ** scale
    return -value if nibbles[-1] in ('D', 'B') else value


def _typed(field, value):
    if value is None:
        return None
    kind = field['kind']
    if kind in ('string', 'any'):
        return json.dumps(value) if isinstance(value, (dict, list)) else str(value)
    if kind == 'bool':
        if isinstance(value, str):
            text = value.strip().lower()
            return None if text == '' else text == 'true'
        return bool(value)
    d = Decimal(repr(value)) if isinstance(value, float) else Decimal(str(value).strip())
    return d.quantize(Decimal(1), rounding=ROUND_HALF_UP) if kind == 'int' else d


def encode_row(table, row) -> bytes:
    out = bytearray()
    for field in table['fields']:
        value = _typed(field, row.get(field['name']))
        for part in field['storage']:
            if part['part'] == 'ni':
                out += b'Y' if value is None else b'N'
            elif part['part'] == 'len':
                out += len(('' if value is None else value).encode('utf-8')).to_bytes(2, 'big', signed=True)
            elif field['kind'] in ('string', 'any'):
                data = ('' if value is None else value).encode('utf-8')
                if len(data) > part['bytes']:
                    raise CobolError(f"{table['name']}.{field['name']}: text longer than {part['bytes']} bytes")
                out += data.ljust(part['bytes'], b' ')
            elif field['kind'] == 'bool':
                out += b'Y' if value else b'N'
            else:
                out += _pack(value if value is not None else Decimal(0), field['digits'], field['scale'])
    return bytes(out)


def decode_row(table, data: bytes):
    row = {}
    for field in table['fields']:
        is_null, length, value = False, 0, None
        for part in field['storage']:
            chunk = data[part['offset']:part['offset'] + part['bytes']]
            if part['part'] == 'ni':
                is_null = chunk == b'Y'
            elif part['part'] == 'len':
                length = int.from_bytes(chunk, 'big', signed=True)
            elif field['kind'] in ('string', 'any'):
                value = chunk[:length].decode('utf-8', errors='replace')
            elif field['kind'] == 'bool':
                value = chunk == b'Y'
            else:
                d = _unpack(chunk, field['scale'])
                value = int(d) if d == d.to_integral_value() else float(d)
        row[field['snake']] = None if is_null else value
    return row


def run(rulebook: dict, now: str = ''):
    """Compile src/ and run it on the rulebook's raw facts.
    Returns ({snake table: [rows]}, {'compile': s, 'run': s, 'status': line, 'errors': [...]})."""
    src = HERE / 'src'
    if not (src / 'erb_manifest.json').exists() or not (src / 'ERBRULES.cbl').exists():
        raise CobolError("src/ holds no generated rules module. Run `effortless build` (rulebook-to-cobol "
                         "v2026.10.20 or later writes ERBRULES.cbl, ERBBATCH.cbl and the copybooks).")
    if not shutil.which('cobc'):
        raise CobolError("GnuCOBOL (cobc) is not installed: brew install gnucobol / apt-get install gnucobol")
    manifest = json.loads((src / 'erb_manifest.json').read_text(encoding='utf-8'))
    with tempfile.TemporaryDirectory(prefix='erb-cobol-') as tmp:
        build, data = Path(tmp) / 'src', Path(tmp) / 'run'
        shutil.copytree(src, build)
        data.mkdir()
        for table in manifest['tables']:
            entity = rulebook.get(table['name']) or {}
            schema = {f['name']: f for f in entity.get('schema') or []}
            with open(data / table['ddIn'], 'wb') as f:
                for row in entity.get('data') or []:
                    fact = {}
                    for field in table['fields']:
                        spec = schema.get(field['name'], {})
                        value = row.get(field['name']) if spec.get('type', 'raw') in RAW_LIKE_TYPES else None
                        if spec.get('datatype') in ('date', 'datetime') and isinstance(value, str) and _BARE_DATE.match(value):
                            value = f'{value} 00:00:00+00'
                        fact[field['name']] = value
                    f.write(encode_row(table, fact))
        cards = ([f'NOW={now}'] if now else []) + ['CHARSET=U']
        (data / 'ERBPARM').write_bytes(b''.join(c.encode('ascii').ljust(80, b' ') for c in cards))
        started = time.time()
        comp = subprocess.run(manifest['build'], cwd=build, capture_output=True, text=True)
        compile_s = time.time() - started
        if comp.returncode != 0:
            raise CobolError(f"cobc could not compile the program:\n{(comp.stderr or comp.stdout)[-4000:]}")
        started = time.time()
        proc = subprocess.run([str(build / 'ERBBATCH')], cwd=data, capture_output=True, text=True)
        run_s = time.time() - started
        if proc.returncode != 0:
            raise CobolError(f"ERBBATCH failed (exit {proc.returncode}):\n{proc.stdout[-3000:]}")
        tables = {}
        for table in manifest['tables']:
            blob = (data / table['ddOut']).read_bytes()
            size = table['recordLength']
            tables[table['snake']] = [decode_row(table, blob[i:i + size]) for i in range(0, len(blob), size)]
    lines = proc.stdout.splitlines()
    return tables, {'compile': round(compile_s, 2), 'run': round(run_s, 2),
                    'status': next((l for l in lines if l.startswith('ERBBATCH:')), ''),
                    'errors': [l for l in lines if l.startswith('ERBERR|')],
                    'compileErrors': manifest.get('compileErrors') or []}
