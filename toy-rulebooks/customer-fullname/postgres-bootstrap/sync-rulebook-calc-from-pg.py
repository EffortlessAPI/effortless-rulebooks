#!/usr/bin/env python3
"""
sync-rulebook-calc-from-pg.py

Write-back step: after the DB has been rebuilt from the rulebook (rulebook-to-postgres
+ init-db.sh), pull every CALCULATED / LOOKUP / AGGREGATION field value out of the
`vw_*` views and write it back into the rulebook's data[] rows.

Doctrine (matches CLAUDE.md + the user rules):
  - The rulebook is HEAD. RAW values are authored by humans and are NEVER touched here.
  - Computed fields are derived; their stored values in data[] are display copies that
    must mirror what Postgres recomputed. This step refreshes those copies.
  - FAIL, do not fall back. If anything is inconsistent (a rulebook row has no PG
    counterpart, a raw value diverges from PG, an expected column is missing), we print
    the exact discrepancy and exit non-zero WITHOUT writing the rulebook. A green build
    must mean the data really matched.

The raw-value verification is the safety interlock behind the user's "ONLY if this
works": if the raws in the rulebook don't match the raws in Postgres, the DB was not
truly rebuilt from the rulebook, so the recomputed calc values can't be trusted -> abort.
"""

import json
import os
import re
import subprocess
import sys
from pathlib import Path

# Field types whose stored data[] value is DERIVED (refreshed from PG) vs. AUTHORED.
COMPUTED_TYPES = {"calculated", "lookup", "aggregation"}
RAW_TYPES = {"raw", "relationship"}

SCRIPT_DIR = Path(__file__).resolve().parent


def die(msg: str) -> "NoReturn":
    sys.stderr.write(f"[sync-calc] FAIL: {msg}\n")
    sys.exit(1)


def snake(name: str) -> str:
    """PascalCase/camelCase -> snake_case, matching rulebook-to-postgres view aliases."""
    s1 = re.sub(r"(.)([A-Z][a-z]+)", r"\1_\2", name)
    s2 = re.sub(r"([a-z0-9])([A-Z])", r"\1_\2", s1)
    return s2.lower()


def norm(v):
    """Normalize a value for raw-equality comparison across the JSON<->PG boundary."""
    if v is None:
        return None
    if isinstance(v, bool):
        return v
    if isinstance(v, (int, float)):
        return float(v)
    return str(v)


def run_psql(database_url: str, sql: str) -> str:
    proc = subprocess.run(
        ["psql", database_url, "-At", "-v", "ON_ERROR_STOP=1", "-c", sql],
        capture_output=True,
        text=True,
    )
    if proc.returncode != 0:
        die(f"psql failed for SQL:\n  {sql}\n{proc.stderr.strip()}")
    return proc.stdout


def find_rulebook() -> Path:
    candidates = sorted((SCRIPT_DIR.parent / "effortless-rulebook").glob("*-rulebook.json"))
    if len(candidates) == 0:
        die("no *-rulebook.json found in ../effortless-rulebook")
    if len(candidates) > 1:
        die(f"expected exactly one *-rulebook.json, found {len(candidates)}: "
            + ", ".join(c.name for c in candidates))
    return candidates[0]


def is_table(key: str, value) -> bool:
    # Metadata keys start with '_' (e.g. __meta__) and are NOT materialized in Postgres.
    if key.startswith("_"):
        return False
    return isinstance(value, dict) and "schema" in value and "data" in value


def primary_key_column(database_url: str, table_snake: str) -> str:
    sql = (
        "SELECT a.attname FROM pg_index i "
        "JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey) "
        f"WHERE i.indrelid = 'public.{table_snake}'::regclass AND i.indisprimary"
    )
    cols = [c for c in run_psql(database_url, sql).splitlines() if c.strip()]
    if len(cols) == 0:
        die(f"table '{table_snake}' has no primary key in Postgres")
    if len(cols) > 1:
        die(f"table '{table_snake}' has a compound primary key {cols}; "
            "this sync only supports single-column PKs")
    return cols[0]


def dump_view(database_url: str, view: str) -> list:
    raw = run_psql(database_url, f"SELECT COALESCE(json_agg(t), '[]') FROM {view} t").strip()
    if not raw:
        die(f"empty response dumping {view}")
    return json.loads(raw)


def main() -> None:
    database_url = os.environ.get("DATABASE_URL")
    if not database_url:
        die("DATABASE_URL is not set (source postgres-bootstrap/effortless.env first)")

    rulebook_path = find_rulebook()
    rulebook = json.loads(rulebook_path.read_text())

    raw_mismatches = []   # (table, pk, field, rulebook_value, pg_value)
    pending_updates = []  # (row_dict, field_name, new_value, old_value, table, pk)
    tables_touched = 0

    for table_name, table in rulebook.items():
        if not is_table(table_name, table):
            continue
        schema = table["schema"]
        computed = [f for f in schema if f.get("type") in COMPUTED_TYPES]
        raws = [f for f in schema if f.get("type") in RAW_TYPES]
        if not computed:
            continue  # nothing derived to refresh

        table_snake = snake(table_name)
        view = f"vw_{table_snake}"

        if run_psql(database_url, f"SELECT to_regclass('public.{view}')").strip() in ("", "NULL"):
            die(f"view '{view}' does not exist — was the DB rebuilt (rulebook-to-postgres + init-db)?")

        pk_col = primary_key_column(database_url, table_snake)
        pk_field = next((f["name"] for f in schema if snake(f["name"]) == pk_col), None)
        if pk_field is None:
            die(f"could not map Postgres PK column '{pk_col}' back to a rulebook field in '{table_name}'")

        pg_rows = dump_view(database_url, view)
        pg_by_pk = {}
        for r in pg_rows:
            if pk_col not in r:
                die(f"{view} row missing PK column '{pk_col}': {r}")
            pg_by_pk[r[pk_col]] = r

        for row in table["data"]:
            if pk_field not in row:
                die(f"rulebook '{table_name}' data row missing PK field '{pk_field}': {row}")
            pk_val = row[pk_field]
            pg_row = pg_by_pk.get(pk_val)
            if pg_row is None:
                die(f"rulebook '{table_name}' row '{pk_val}' has NO counterpart in {view} "
                    "— DB is out of sync with the rulebook")

            # Interlock: every RAW value must already match Postgres. If not, the DB was
            # not rebuilt from the rulebook and the calc values are untrustworthy -> abort.
            for f in raws:
                col = snake(f["name"])
                if col not in pg_row:
                    die(f"{view} is missing expected raw column '{col}' for field '{f['name']}'")
                if norm(row.get(f["name"])) != norm(pg_row[col]):
                    raw_mismatches.append(
                        (table_name, pk_val, f["name"], row.get(f["name"]), pg_row[col])
                    )

            # Stage computed-field refreshes (applied only if ALL verification passes).
            for f in computed:
                col = snake(f["name"])
                if col not in pg_row:
                    die(f"{view} is missing expected computed column '{col}' for field '{f['name']}'")
                new_val = pg_row[col]
                old_val = row.get(f["name"])
                if norm(old_val) != norm(new_val):
                    pending_updates.append((row, f["name"], new_val, old_val, table_name, pk_val))

        tables_touched += 1

    if raw_mismatches:
        sys.stderr.write(
            "[sync-calc] FAIL: RAW values diverge between the rulebook and Postgres.\n"
            "[sync-calc]       The DB was not rebuilt from the rulebook (raws must match before\n"
            "[sync-calc]       calc fields can be trusted). Refusing to write the rulebook.\n"
        )
        for (t, pk, field, rb_v, pg_v) in raw_mismatches:
            sys.stderr.write(
                f"[sync-calc]   {t}[{pk}].{field}: rulebook={rb_v!r}  postgres={pg_v!r}\n"
            )
        sys.exit(1)

    if not pending_updates:
        print(f"[sync-calc] up to date — {tables_touched} table(s) checked, no calc fields changed.")
        return

    for (row, field_name, new_val, _old, _t, _pk) in pending_updates:
        row[field_name] = new_val

    tmp = rulebook_path.with_suffix(".json.tmp")
    tmp.write_text(json.dumps(rulebook, indent=2, ensure_ascii=False) + "\n")
    os.replace(tmp, rulebook_path)

    print(f"[sync-calc] updated {len(pending_updates)} calc field(s) "
          f"across {tables_touched} table(s) from Postgres:")
    for (_row, field_name, new_val, old_val, t, pk) in pending_updates:
        print(f"[sync-calc]   {t}[{pk}].{field_name}: {old_val!r} -> {new_val!r}")


if __name__ == "__main__":
    main()
