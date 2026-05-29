#!/usr/bin/env python3
"""
postgres-calculated-to-rulebook

Merges current Postgres raw-table data back into the rulebook's seed data arrays.
Only raw and relationship fields are updated — computed/lookup/aggregation fields
are left alone (they are derived by the transpiler, not authored).

Input:  .pg-raw-data.json  — written by pull-from-postgres.sh
        rulebook JSON       — the SSoT being updated

Usage (direct):
  python3 inject-into-postgres-calculated-to-rulebook.py <rulebook_path> [<json_path>]

Usage (via server.js / ssotme-proxy — set ERB_RULEBOOK_PATH, cwd = postgres-bootstrap/):
  python3 inject-into-postgres-calculated-to-rulebook.py
"""

import json
import os
import re
import sys
from pathlib import Path
from typing import Any, Dict, List, Tuple

COMPUTED_TYPES = {"calculated", "lookup", "aggregation"}
RAW_TYPES = {"raw", "relationship"}


def die(msg: str) -> None:
    sys.stderr.write(f"[postgres-calculated-to-rulebook] FAIL: {msg}\n")
    sys.exit(1)


def warn(msg: str) -> None:
    sys.stderr.write(f"[postgres-calculated-to-rulebook] WARN: {msg}\n")


def log(msg: str) -> None:
    sys.stdout.write(f"[postgres-calculated-to-rulebook] {msg}\n")


def snake(name: str) -> str:
    s1 = re.sub(r"(.)([A-Z][a-z]+)", r"\1_\2", name)
    s2 = re.sub(r"([a-z0-9])([A-Z])", r"\1_\2", s1)
    return s2.lower()


def norm(v: Any) -> Any:
    if v is None:
        return None
    if isinstance(v, bool):
        return v
    if isinstance(v, (int, float)):
        return float(v)
    return str(v)


def load_raw_data(json_path: Path) -> Dict[str, List[Dict[str, Any]]]:
    """Load .pg-raw-data.json written by pull-from-postgres.sh."""
    if not json_path.is_file():
        die(f"raw-data JSON not found: {json_path}\n"
            f"  Run pull-from-postgres.sh first to generate it.")
    return json.loads(json_path.read_text())


def is_table(key: str, value: Any) -> bool:
    if key.startswith("_"):
        return False
    return isinstance(value, dict) and "schema" in value and "data" in value


def main() -> None:
    rulebook_path = None
    json_path = None

    if len(sys.argv) >= 2:
        rulebook_path = Path(sys.argv[1]).resolve()
        json_path = Path(sys.argv[2]).resolve() if len(sys.argv) >= 3 else None
    elif "ERB_RULEBOOK_PATH" in os.environ:
        rulebook_path = Path(os.environ["ERB_RULEBOOK_PATH"]).resolve()

    if not rulebook_path:
        die("rulebook not found (provide as arg or set ERB_RULEBOOK_PATH)")

    if not rulebook_path.is_file():
        die(f"rulebook not found: {rulebook_path}")

    # Locate the JSON data file: explicit arg > cwd > postgres-bootstrap/ sibling
    if json_path is None:
        candidates = [
            Path.cwd() / ".pg-raw-data.json",
            rulebook_path.parent.parent / "postgres-bootstrap" / ".pg-raw-data.json",
        ]
        for c in candidates:
            if c.is_file():
                json_path = c
                break
        if json_path is None:
            die(f".pg-raw-data.json not found (tried: {', '.join(str(c) for c in candidates)})\n"
                f"  Run pull-from-postgres.sh to generate it.")

    log(f"rulebook={rulebook_path}")
    log(f"raw-data={json_path}")

    rulebook = json.loads(rulebook_path.read_text())
    raw_data = load_raw_data(json_path)

    pending_updates: List[Tuple[Dict, str, Any, Any, str, Any]] = []
    tables_touched = 0

    for table_name, table in rulebook.items():
        if not is_table(table_name, table):
            continue

        schema = table["schema"]
        raws = [f for f in schema if f.get("type") in RAW_TYPES]

        if not raws:
            continue

        table_snake = snake(table_name)
        if table_snake not in raw_data:
            warn(f"table '{table_name}' ({table_snake}) not found in raw-data JSON")
            continue

        pg_rows = raw_data[table_snake]
        if not pg_rows:
            warn(f"table '{table_name}' has no rows in raw-data JSON")
            continue

        pk_field = schema[0]["name"] if schema else None
        if not pk_field:
            die(f"table '{table_name}' has no fields")

        pk_col = snake(pk_field)
        pg_by_pk = {}
        for pg_row in pg_rows:
            if pk_col not in pg_row:
                warn(f"{table_snake}: row missing expected PK column '{pk_col}'")
                continue
            pg_by_pk[pg_row[pk_col]] = pg_row

        for row in table.get("data", []):
            if pk_field not in row:
                warn(f"{table_name} rulebook row missing PK field '{pk_field}'")
                continue
            pk_val = row[pk_field]
            pg_row = pg_by_pk.get(pk_val)
            if pg_row is None:
                warn(f"{table_name}[{pk_val}] not in Postgres (deleted?)")
                continue

            for f in raws:
                col = snake(f["name"])
                if col not in pg_row:
                    continue
                new_val = pg_row[col]
                old_val = row.get(f["name"])
                if norm(old_val) != norm(new_val):
                    pending_updates.append((row, f["name"], new_val, old_val, table_name, pk_val))

        tables_touched += 1

    if not pending_updates:
        log(f"up to date — {tables_touched} table(s) checked, nothing changed")
        return

    for (row, field_name, new_val, _old, _t, _pk) in pending_updates:
        row[field_name] = new_val

    tmp = rulebook_path.with_suffix(".json.tmp")
    tmp.write_text(json.dumps(rulebook, indent=2, ensure_ascii=False) + "\n")
    os.replace(tmp, rulebook_path)

    log(f"updated {len(pending_updates)} field value(s) across {tables_touched} table(s)")
    for (_row, field_name, new_val, old_val, t, pk) in pending_updates:
        log(f"  {t}[{pk}].{field_name}: {old_val!r} -> {new_val!r}")


if __name__ == "__main__":
    main()
