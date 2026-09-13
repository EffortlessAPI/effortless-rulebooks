#!/usr/bin/env python3
"""
postgres-calculated-to-rulebook

Merges current Postgres data back into the rulebook's seed data arrays.

TWO MODES, selected by the ERB_WRITE_COMPUTED env var:

  • DEFAULT (ERB_WRITE_COMPUTED unset/false) — raws-only sync.
    Only raw and relationship fields are updated; computed/lookup/aggregation
    fields are left alone. This is the safe reverse-spoke: it pulls hand-edited
    raw values back from the DB without bloating the rulebook with derived data.

  • ERB_WRITE_COMPUTED=true — COMPUTED-ONLY adoption.
    calculated/lookup/aggregation values are written back; raws and
    relationships are left exactly as the rulebook has them. This is the
    mechanism behind `regenerate-answer-keys.sh`: load rulebook raws → Postgres
    → vw_* compute the correct values → adopt those computed columns → the
    answer-key generator then reads them. For this to refresh computed fields,
    the exchange JSON must come from the VIEWS (pull-from-postgres.sh exports
    vw_*, not base tables).

    This mode used to write the WHOLE view row, raws included. That was wrong
    and lossy: the temp database it reads from was itself loaded FROM those same
    rulebook raws moments earlier, so writing them back carries no information —
    only Postgres's round-trip representation. In practice it rewrote datetimes
    with the running machine's timezone offset (one star-trek build rewrote 1018
    raw values, '2026-01-12T19:46:59Z' -> '2026-01-12T13:46:59-06:00'), which
    made the rulebook hub a function of where the build ran. See cr-24.

Input:  .pg-raw-data.json  — written by pull-from-postgres.sh (vw_* rows)
        rulebook JSON       — the SSoT being updated

Usage (direct):
  python3 inject-into-postgres-calculated-to-rulebook.py <rulebook_path> [<json_path>]

Usage (via the CLI-hosted tool oss-postgres-calculated-to-rulebook):
  orchestration/local_tool_shim.py sets ERB_RULEBOOK_PATH and
  ERB_PG_RAW_DATA_PATH to the copies the CLI unpacked into its run directory,
  and ERB_OUTPUT_DIR to the directory whose files become the step's output.
  The updated rulebook is written THERE under its own file name — never over
  the input copy, which the CLI discards — and the CLI applies it to the real
  rulebook as an in-place upsert. Nothing is emitted when nothing changed.
"""

import json
import os
import re
import sys
from pathlib import Path
from typing import Any, Dict, List, Tuple

COMPUTED_TYPES = {"calculated", "lookup", "aggregation"}
RAW_TYPES = {"raw", "relationship"}

# When true, adopt the computed columns from the views. The two modes are
# DISJOINT on purpose: a run either syncs raws back from a database someone has
# been editing, or it adopts freshly computed values from a database that was
# just loaded from the rulebook. No run needs to do both, and doing both is what
# corrupted raw datetimes (cr-24).
WRITE_COMPUTED = os.environ.get("ERB_WRITE_COMPUTED", "").lower() in ("1", "true", "yes")
WRITABLE_TYPES = COMPUTED_TYPES if WRITE_COMPUTED else RAW_TYPES


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
        pull_script = json_path.parent / "pull-from-postgres.sh"
        if not pull_script.exists():
            die(
                f"raw-data JSON not found: {json_path}\n"
                f"  pull-from-postgres.sh is also missing from {json_path.parent}\n"
                f"  Copy it from rulebook-examples/acme-llc/postgres-bootstrap/pull-from-postgres.sh\n"
                f"  then run: bash {pull_script}"
            )
        die(f"raw-data JSON not found: {json_path}\n"
            f"  Run pull-from-postgres.sh first: bash {pull_script}")
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
        if "ERB_PG_RAW_DATA_PATH" in os.environ:
            json_path = Path(os.environ["ERB_PG_RAW_DATA_PATH"]).resolve()
            if not json_path.is_file():
                die(f"ERB_PG_RAW_DATA_PATH={json_path} is not a file")

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
            pg_bootstrap = rulebook_path.parent.parent / "postgres-bootstrap"
            pull_script = pg_bootstrap / "pull-from-postgres.sh"
            if not pull_script.exists():
                die(
                    f".pg-raw-data.json not found (tried: {', '.join(str(c) for c in candidates)})\n"
                    f"\n"
                    f"  pull-from-postgres.sh is also MISSING from {pg_bootstrap}\n"
                    f"\n"
                    f"  This script is not yet generated by the rulebook-to-postgres transpiler.\n"
                    f"  To fix this permanently: add pull-from-postgres.sh generation to the\n"
                    f"  cloud transpiler so every new postgres-bootstrap/ gets it automatically.\n"
                    f"\n"
                    f"  Workaround for now:\n"
                    f"    1. Copy pull-from-postgres.sh from any project that has one:\n"
                    f"       e.g. rulebook-examples/acme-llc/postgres-bootstrap/pull-from-postgres.sh\n"
                    f"    2. Place it at: {pull_script}\n"
                    f"    3. Ensure DATABASE_URL is set (or sourced from effortless.env)\n"
                    f"    4. Run: bash {pull_script}\n"
                    f"    5. Re-run this transpiler.\n"
                )
            die(
                f".pg-raw-data.json not found (tried: {', '.join(str(c) for c in candidates)})\n"
                f"\n"
                f"  The pull-from-postgres.sh script exists at {pull_script}\n"
                f"  but has not been run yet (or the DB is empty).\n"
                f"\n"
                f"  Steps:\n"
                f"    1. Make sure the database is running and initialized:\n"
                f"         bash {pg_bootstrap / 'init-db.sh'}\n"
                f"    2. Pull raw data from Postgres:\n"
                f"         bash {pull_script}\n"
                f"    3. Re-run this transpiler.\n"
            )

    log(f"rulebook={rulebook_path}")
    log(f"raw-data={json_path}")
    log(f"mode={'computed-only (adopt vw_* derived columns)' if WRITE_COMPUTED else 'raws-only (default)'}")

    rulebook_text = rulebook_path.read_text()
    rulebook = json.loads(rulebook_text)
    # Rewrite with the file's own indent: a different indent reflows every line of
    # a contended rulebook and clobbers concurrent edits on the next merge.
    indent_match = re.match(r'[{\[]\n( +)\S', rulebook_text)
    if not indent_match:
        die(f"cannot detect the indent of {rulebook_path}; refusing to reflow it")
    rulebook_indent = len(indent_match.group(1))
    raw_data = load_raw_data(json_path)

    pending_updates: List[Tuple[Dict, str, Any, Any, str, Any]] = []
    tables_touched = 0

    for table_name, table in rulebook.items():
        if not is_table(table_name, table):
            continue

        schema = table["schema"]
        # In default mode: raw + relationship fields. In ERB_WRITE_COMPUTED
        # mode: every field type, so the rulebook adopts the full computed
        # view row (the answer-key refresh path).
        writable = [f for f in schema if f.get("type", "raw") in WRITABLE_TYPES]

        if not writable:
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

            for f in writable:
                col = snake(f["name"])
                if col not in pg_row:
                    continue
                new_val = pg_row[col]
                old_val = row.get(f["name"])
                # A computed NULL is an answer, so it is written even onto a row
                # that never carried the key; skipping it left the key absent and
                # answer-key generation substituted the Python engine's value.
                adopt_null = WRITE_COMPUTED and f["name"] not in row
                if adopt_null or norm(old_val) != norm(new_val):
                    pending_updates.append((row, f["name"], new_val, old_val, table_name, pk_val))

        tables_touched += 1

    if not pending_updates:
        log(f"up to date — {tables_touched} table(s) checked, nothing changed")
        return

    for (row, field_name, new_val, _old, _t, _pk) in pending_updates:
        row[field_name] = new_val

    # Under the CLI the input is a throwaway copy: the result must go to the
    # output directory, under the rulebook's own name, to reach the real file.
    target = (
        Path(os.environ["ERB_OUTPUT_DIR"]) / rulebook_path.name
        if "ERB_OUTPUT_DIR" in os.environ
        else rulebook_path
    )
    tmp = target.with_suffix(".json.tmp")
    tmp.write_text(json.dumps(rulebook, indent=rulebook_indent, ensure_ascii=False) + "\n", encoding="utf-8")
    os.replace(tmp, target)

    log(f"updated {len(pending_updates)} field value(s) across {tables_touched} table(s)")
    for (_row, field_name, new_val, old_val, t, pk) in pending_updates:
        log(f"  {t}[{pk}].{field_name}: {old_val!r} -> {new_val!r}")


if __name__ == "__main__":
    main()
