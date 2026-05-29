#!/usr/bin/env python3
"""
postgres-to-rulebook  (reverse spoke — refreshes DERIVED fields only)

Shared ssotme-proxy transpiler. After the DB has been rebuilt from the rulebook
(rulebook-to-postgres + init-db.sh), this pulls every CALCULATED / LOOKUP /
AGGREGATION field value out of the `vw_*` views and writes it back into the
rulebook's data[] rows, so the rulebook's stored display copies of derived fields
mirror exactly what Postgres recomputed.

Doctrine (CLAUDE.md + user rules):
  - The rulebook is HEAD. RAW values are human-authored and are NEVER written here.
  - Derived field values stored in data[] are display copies; this refreshes them.
  - FAIL, do not fall back. Any inconsistency (rulebook row with no PG counterpart,
    a raw value diverging from PG, a missing expected column) prints the exact
    discrepancy and exits non-zero WITHOUT writing the rulebook.

Invocation contexts (both deterministic — no guessing):
  - Via ssotme-proxy: the proxy sets ERB_RULEBOOK_PATH (and runs us from the
    substrate folder). That env var is the rulebook.
  - Standalone (./inject-into-postgres-to-rulebook.py from a <domain>/<sub>/ dir):
    we discover ../effortless-rulebook/*-rulebook.json relative to cwd.

DATABASE_URL resolution (matches orchestrate.sh / effortless.env):
  - DATABASE_URL env var if set (override), else the SSoT-derived default
    postgresql://postgres@localhost:5432/erb_<domain-with-underscores>.
"""

import json
import os
import re
import subprocess
import sys
from pathlib import Path

COMPUTED_TYPES = {"calculated", "lookup", "aggregation"}
RAW_TYPES = {"raw", "relationship"}


def die(msg: str) -> "NoReturn":
    sys.stderr.write(f"[postgres-to-rulebook] FAIL: {msg}\n")
    sys.exit(1)


def snake(name: str) -> str:
    """PascalCase/camelCase -> snake_case, matching rulebook-to-postgres view aliases."""
    s1 = re.sub(r"(.)([A-Z][a-z]+)", r"\1_\2", name)
    s2 = re.sub(r"([a-z0-9])([A-Z])", r"\1_\2", s1)
    return s2.lower()


def norm(v):
    if v is None:
        return None
    if isinstance(v, bool):
        return v
    if isinstance(v, (int, float)):
        return float(v)
    return str(v)


def resolve_rulebook() -> Path:
    env_path = os.environ.get("ERB_RULEBOOK_PATH")
    if env_path:
        p = Path(env_path)
        if not p.is_file():
            die(f"ERB_RULEBOOK_PATH points at a non-file: {p}")
        return p.resolve()
    # Standalone: discover ../effortless-rulebook/*-rulebook.json relative to cwd.
    candidates = sorted((Path.cwd().parent / "effortless-rulebook").glob("*-rulebook.json"))
    if len(candidates) == 0:
        die("no ERB_RULEBOOK_PATH set and no ../effortless-rulebook/*-rulebook.json found")
    if len(candidates) > 1:
        die("multiple *-rulebook.json found; set ERB_RULEBOOK_PATH to disambiguate: "
            + ", ".join(c.name for c in candidates))
    return candidates[0].resolve()


def domain_from_rulebook(rulebook_path: Path) -> str:
    stem = rulebook_path.stem
    if not stem.endswith("-rulebook"):
        die(f"rulebook filename '{rulebook_path.name}' does not match '<domain>-rulebook.json'")
    return stem[: -len("-rulebook")]


def resolve_database_url(domain: str) -> str:
    override = os.environ.get("DATABASE_URL")
    if override:
        return override
    return f"postgresql://postgres@localhost:5432/erb_{domain.replace('-', '_')}"


def run_psql(database_url: str, sql: str) -> str:
    proc = subprocess.run(
        ["psql", database_url, "-At", "-v", "ON_ERROR_STOP=1", "-c", sql],
        capture_output=True, text=True,
    )
    if proc.returncode != 0:
        die(f"psql failed for SQL:\n  {sql}\n{proc.stderr.strip()}")
    return proc.stdout


def is_table(key: str, value) -> bool:
    if key.startswith("_"):  # metadata keys (e.g. __meta__) are not materialized in PG
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
            "this tool only supports single-column PKs")
    return cols[0]


def dump_view(database_url: str, view: str) -> list:
    raw = run_psql(database_url, f"SELECT COALESCE(json_agg(t), '[]') FROM {view} t").strip()
    if not raw:
        die(f"empty response dumping {view}")
    return json.loads(raw)


def main() -> None:
    rulebook_path = resolve_rulebook()
    domain = domain_from_rulebook(rulebook_path)
    database_url = resolve_database_url(domain)
    print(f"[postgres-to-rulebook] domain={domain} db={database_url} rulebook={rulebook_path}")

    rulebook = json.loads(rulebook_path.read_text())

    raw_mismatches = []
    pending_updates = []
    tables_touched = 0

    for table_name, table in rulebook.items():
        if not is_table(table_name, table):
            continue
        schema = table["schema"]
        computed = [f for f in schema if f.get("type") in COMPUTED_TYPES]
        raws = [f for f in schema if f.get("type") in RAW_TYPES]
        if not computed:
            continue

        table_snake = snake(table_name)
        view = f"vw_{table_snake}"
        if run_psql(database_url, f"SELECT to_regclass('public.{view}')").strip() in ("", "NULL"):
            die(f"view '{view}' does not exist — was the DB rebuilt (rulebook-to-postgres + init-db)?")

        pk_col = primary_key_column(database_url, table_snake)
        pk_field = next((f["name"] for f in schema if snake(f["name"]) == pk_col), None)
        if pk_field is None:
            die(f"could not map PK column '{pk_col}' back to a rulebook field in '{table_name}'")

        pg_by_pk = {}
        for r in dump_view(database_url, view):
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

            for f in raws:
                col = snake(f["name"])
                if col not in pg_row:
                    die(f"{view} missing expected raw column '{col}' for field '{f['name']}'")
                if norm(row.get(f["name"])) != norm(pg_row[col]):
                    raw_mismatches.append(
                        (table_name, pk_val, f["name"], row.get(f["name"]), pg_row[col])
                    )

            for f in computed:
                col = snake(f["name"])
                if col not in pg_row:
                    die(f"{view} missing expected computed column '{col}' for field '{f['name']}'")
                if norm(row.get(f["name"])) != norm(pg_row[col]):
                    pending_updates.append((row, f["name"], pg_row[col], row.get(f["name"]), table_name, pk_val))

        tables_touched += 1

    if raw_mismatches:
        sys.stderr.write(
            "[postgres-to-rulebook] FAIL: RAW values diverge between the rulebook and Postgres.\n"
            "[postgres-to-rulebook]       The DB was not rebuilt from the rulebook (raws must match\n"
            "[postgres-to-rulebook]       before derived fields can be trusted). Refusing to write.\n"
        )
        for (t, pk, field, rb_v, pg_v) in raw_mismatches:
            sys.stderr.write(f"[postgres-to-rulebook]   {t}[{pk}].{field}: rulebook={rb_v!r} postgres={pg_v!r}\n")
        sys.exit(1)

    if not pending_updates:
        print(f"[postgres-to-rulebook] up to date — {tables_touched} table(s) checked, no derived fields changed.")
        return

    for (row, field_name, new_val, _old, _t, _pk) in pending_updates:
        row[field_name] = new_val

    tmp = rulebook_path.with_suffix(".json.tmp")
    tmp.write_text(json.dumps(rulebook, indent=2, ensure_ascii=False) + "\n")
    os.replace(tmp, rulebook_path)

    print(f"[postgres-to-rulebook] updated {len(pending_updates)} derived field(s) "
          f"across {tables_touched} table(s) from Postgres:")
    for (_row, field_name, new_val, old_val, t, pk) in pending_updates:
        print(f"[postgres-to-rulebook]   {t}[{pk}].{field_name}: {old_val!r} -> {new_val!r}")


if __name__ == "__main__":
    main()
