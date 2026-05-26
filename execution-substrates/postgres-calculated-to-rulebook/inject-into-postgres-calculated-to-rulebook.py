#!/usr/bin/env python3
"""
postgres-calculated-to-rulebook  (reverse spoke — refreshes DERIVED fields only)

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
  - Standalone (./inject-into-postgres-calculated-to-rulebook.py from a <domain>/<sub>/ dir):
    we discover ../effortless-rulebook/*-rulebook.json relative to cwd.

DATABASE_URL resolution (matches init-db.sh / orchestrate.sh):
  - postgres-bootstrap/effortless.env's DATABASE_URL (the domain's configured DB,
    the SAME file init-db.sh sources) wins, else an explicit DATABASE_URL env
    override, else the SSoT-derived default
    postgresql://postgres@localhost:5432/erb_<domain-with-underscores>.
  This must agree with init-db.sh or we'd read a different DB than the one the
  build just rebuilt.
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
    sys.stderr.write(f"[postgres-calculated-to-rulebook] FAIL: {msg}\n")
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


def read_env_database_url(rulebook_path: Path) -> "str | None":
    """Read DATABASE_URL from the domain's postgres-bootstrap/effortless.env —
    the SAME file init-db.sh sources. This is the single source of truth for the
    domain's DB connection; without it we'd hit a different DB than the one the
    build just rebuilt (e.g. erb_customer_crm vs the configured erb_customer_crm_demo).
    Looks in cwd first (where the proxy runs us = postgres-bootstrap/), then the
    deterministic <domain>/postgres-bootstrap/effortless.env next to the rulebook.
    """
    candidates = [
        Path.cwd() / "effortless.env",
        rulebook_path.parent.parent / "postgres-bootstrap" / "effortless.env",
    ]
    for env_file in candidates:
        if not env_file.is_file():
            continue
        for line in env_file.read_text().splitlines():
            line = line.strip()
            if line.startswith("#") or "=" not in line:
                continue
            key, _, val = line.partition("=")
            if key.strip().replace("export ", "").strip() == "DATABASE_URL":
                val = val.strip().strip('"').strip("'")
                if val:
                    return val
    return None


def resolve_database_url(domain: str, rulebook_path: Path) -> str:
    # Same precedence as init-db.sh: effortless.env (the domain's configured DB)
    # wins, then an explicit env override, then the SSoT-derived default.
    from_env_file = read_env_database_url(rulebook_path)
    if from_env_file:
        return from_env_file
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


def env_truthy(name: str) -> bool:
    return os.environ.get(name, "").strip().lower() in ("1", "true", "yes", "on")


def main() -> None:
    rulebook_path = resolve_rulebook()
    domain = domain_from_rulebook(rulebook_path)
    database_url = resolve_database_url(domain, rulebook_path)
    # ADOPT mode (opt-in, explicit): write RAW values AND add new PG rows back
    # into the rulebook — i.e. "adopt the current Postgres state as the new seed".
    # This is the ONLY path that writes raws, and it is never on by default. It is
    # meant for the portal's explicit, user-confirmed "Import from Postgres" action
    # after data was edited in the app/portal. To honor "never silently remove
    # data", it does NOT delete rulebook rows that are absent from Postgres; it
    # warns about them instead.
    adopt = env_truthy("ERB_ADOPT_RAWS")
    mode = "ADOPT (raws + new rows)" if adopt else "refresh (derived only)"
    print(f"[postgres-calculated-to-rulebook] mode={mode} domain={domain} db={database_url} rulebook={rulebook_path}")

    rulebook = json.loads(rulebook_path.read_text())

    raw_mismatches = []
    pending_updates = []          # derived-field display-copy refreshes (both modes)
    raw_adopts = []               # raw-field overwrites (adopt mode only)
    added_rows = []               # (table_name, pk_val) new rows pulled from PG (adopt mode)
    orphan_rulebook_rows = []     # (table_name, pk_val) in rulebook but not PG (adopt mode: warn, keep)
    tables_touched = 0

    for table_name, table in rulebook.items():
        if not is_table(table_name, table):
            continue
        schema = table["schema"]
        computed = [f for f in schema if f.get("type") in COMPUTED_TYPES]
        raws = [f for f in schema if f.get("type") in RAW_TYPES]
        # Default mode only cares about tables with derived fields. Adopt mode
        # processes every table (raw-only tables can have edited/added data too).
        if not computed and not adopt:
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

        rulebook_pks = set()
        for row in table["data"]:
            if pk_field not in row:
                die(f"rulebook '{table_name}' data row missing PK field '{pk_field}': {row}")
            pk_val = row[pk_field]
            rulebook_pks.add(pk_val)
            pg_row = pg_by_pk.get(pk_val)
            if pg_row is None:
                if adopt:
                    # Row exists in the rulebook but not in PG (deleted in the app).
                    # Keep it and warn — removal requires an explicit, separate decision.
                    orphan_rulebook_rows.append((table_name, pk_val))
                    continue
                die(f"rulebook '{table_name}' row '{pk_val}' has NO counterpart in {view} "
                    "— DB is out of sync with the rulebook")

            for f in raws:
                col = snake(f["name"])
                if col not in pg_row:
                    die(f"{view} missing expected raw column '{col}' for field '{f['name']}'")
                if norm(row.get(f["name"])) != norm(pg_row[col]):
                    if adopt:
                        raw_adopts.append((row, f["name"], pg_row[col], row.get(f["name"]), table_name, pk_val))
                    else:
                        raw_mismatches.append(
                            (table_name, pk_val, f["name"], row.get(f["name"]), pg_row[col])
                        )

            for f in computed:
                col = snake(f["name"])
                if col not in pg_row:
                    die(f"{view} missing expected computed column '{col}' for field '{f['name']}'")
                if norm(row.get(f["name"])) != norm(pg_row[col]):
                    pending_updates.append((row, f["name"], pg_row[col], row.get(f["name"]), table_name, pk_val))

        if adopt:
            # New rows present in PG but not the rulebook (added in the app).
            for pk_val, pg_row in pg_by_pk.items():
                if pk_val in rulebook_pks:
                    continue
                new_row = {}
                for f in schema:
                    col = snake(f["name"])
                    if col in pg_row:
                        new_row[f["name"]] = pg_row[col]
                table["data"].append(new_row)
                added_rows.append((table_name, pk_val))

        tables_touched += 1

    if raw_mismatches:
        sys.stderr.write(
            "[postgres-calculated-to-rulebook] FAIL: RAW values diverge between the rulebook and Postgres.\n"
            "[postgres-calculated-to-rulebook]       The DB was not rebuilt from the rulebook (raws must match\n"
            "[postgres-calculated-to-rulebook]       before derived fields can be trusted). Refusing to write.\n"
            "[postgres-calculated-to-rulebook]       (To adopt the current Postgres data AS the new seed, re-run\n"
            "[postgres-calculated-to-rulebook]        with ERB_ADOPT_RAWS=true — explicit, raws-overwriting import.)\n"
        )
        for (t, pk, field, rb_v, pg_v) in raw_mismatches:
            sys.stderr.write(f"[postgres-calculated-to-rulebook]   {t}[{pk}].{field}: rulebook={rb_v!r} postgres={pg_v!r}\n")
        sys.exit(1)

    for (table_name, pk_val) in orphan_rulebook_rows:
        sys.stderr.write(f"[postgres-calculated-to-rulebook]   WARN: {table_name}[{pk_val}] is in the rulebook but not in Postgres "
                         "(kept — adopt mode never deletes rows).\n")

    total_changes = len(pending_updates) + len(raw_adopts) + len(added_rows)
    if total_changes == 0:
        print(f"[postgres-calculated-to-rulebook] up to date — {tables_touched} table(s) checked, nothing changed.")
        return

    for (row, field_name, new_val, _old, _t, _pk) in pending_updates:
        row[field_name] = new_val
    for (row, field_name, new_val, _old, _t, _pk) in raw_adopts:
        row[field_name] = new_val

    tmp = rulebook_path.with_suffix(".json.tmp")
    tmp.write_text(json.dumps(rulebook, indent=2, ensure_ascii=False) + "\n")
    os.replace(tmp, rulebook_path)

    summary = f"{len(pending_updates)} derived field(s)"
    if raw_adopts:
        summary += f", {len(raw_adopts)} raw field(s) adopted"
    if added_rows:
        summary += f", {len(added_rows)} new row(s) added"
    print(f"[postgres-calculated-to-rulebook] updated {summary} across {tables_touched} table(s) from Postgres:")
    for (_row, field_name, new_val, old_val, t, pk) in pending_updates:
        print(f"[postgres-calculated-to-rulebook]   {t}[{pk}].{field_name}: {old_val!r} -> {new_val!r}  (derived)")
    for (_row, field_name, new_val, old_val, t, pk) in raw_adopts:
        print(f"[postgres-calculated-to-rulebook]   {t}[{pk}].{field_name}: {old_val!r} -> {new_val!r}  (RAW adopted)")
    for (t, pk) in added_rows:
        print(f"[postgres-calculated-to-rulebook]   {t}[{pk}]: + new row from Postgres")


if __name__ == "__main__":
    main()
