#!/usr/bin/env python3
"""Push a fresh conformance/matrix.json into the rulebook's Postgres witness
tables (answer_key_results, reflection_results).

Direction of truth: conformance/run.py (the reference evaluator + the
independent SQLite oracle) computes matrix.json. This script only copies
those already-computed outcomes into the database as witnessed rows -- it
never re-derives R0 semantics, and it never touches Substrates, which is
computed FROM these rows by the rulebook's own AND/COUNT rollup formulas
(AnswerKeyOk / ReflectionOk / Conformant on vw_substrates).

Usage:
    python3 conformance/ingest_conformance.py [path/to/matrix.json]

Connects with the same PG* environment variables as start.sh / the app
(PGHOST, PGPORT, PGUSER, PGPASSWORD, PGDATABASE; default database
erb_cmcc_core_r0). Requires only psql on PATH -- no Python DB driver, to
match this project's "no other dependencies" posture.
"""
from __future__ import annotations

import json
import os
import subprocess
import sys

PROJECT_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEFAULT_MATRIX = os.path.join(PROJECT_DIR, "conformance", "matrix.json")

CONDITION_KEYS = (
    "1_constructor_preservation",
    "2_dependency_preservation",
    "3_addressability",
    "4_local_adequacy",
    "5_injectivity",
    "6_bounded_locality",
)


def sql_quote(s: str) -> str:
    return "'" + s.replace("'", "''") + "'"


def outcome_of(ok: bool) -> str:
    return "pass" if ok else "fail"


def reflection_outcome_of(value) -> str:
    if value is None:
        return "delegated"
    return "pass" if value else "fail"


def build_statements(matrix: dict) -> list[str]:
    statements = ["BEGIN;"]
    for substrate_id, entry in matrix["substrates"].items():
        for input_history_id, ak in entry["answer_key"].items():
            row_id = f"{substrate_id}__{input_history_id}"
            statements.append(
                "INSERT INTO answer_key_results "
                "(answer_key_result_id, substrate, input_history, traces_produced, permitted, outcome) "
                f"VALUES ({sql_quote(row_id)}, {sql_quote(substrate_id)}, {sql_quote(input_history_id)}, "
                f"{int(ak['traces_produced'])}, {int(ak['permitted'])}, {sql_quote(outcome_of(bool(ak['ok'])))}) "
                "ON CONFLICT (answer_key_result_id) DO UPDATE SET "
                "substrate = EXCLUDED.substrate, input_history = EXCLUDED.input_history, "
                "traces_produced = EXCLUDED.traces_produced, permitted = EXCLUDED.permitted, "
                "outcome = EXCLUDED.outcome;"
            )
        conditions = entry["reflection"]["conditions"]
        for condition_id in CONDITION_KEYS:
            row_id = f"{substrate_id}__{condition_id}"
            outcome = reflection_outcome_of(conditions.get(condition_id))
            statements.append(
                "INSERT INTO reflection_results "
                "(reflection_result_id, substrate, reflection_condition, outcome) "
                f"VALUES ({sql_quote(row_id)}, {sql_quote(substrate_id)}, {sql_quote(condition_id)}, "
                f"{sql_quote(outcome)}) "
                "ON CONFLICT (reflection_result_id) DO UPDATE SET "
                "substrate = EXCLUDED.substrate, reflection_condition = EXCLUDED.reflection_condition, "
                "outcome = EXCLUDED.outcome;"
            )
    statements.append("COMMIT;")
    return statements


def main() -> int:
    matrix_path = sys.argv[1] if len(sys.argv) > 1 else DEFAULT_MATRIX
    if not os.path.isfile(matrix_path):
        print(f"ingest_conformance: no such file: {matrix_path}", file=sys.stderr)
        print("run python3 conformance/run.py first", file=sys.stderr)
        return 1
    matrix = json.load(open(matrix_path))

    statements = build_statements(matrix)
    sql = "\n".join(statements) + "\n"

    env = dict(os.environ)
    env.setdefault("PGHOST", "localhost")
    env.setdefault("PGUSER", "postgres")
    env.setdefault("PGPASSWORD", "postgres")
    env.setdefault("PGPORT", "5432")
    env.setdefault("PGDATABASE", "erb_cmcc_core_r0")

    result = subprocess.run(
        ["psql", "-v", "ON_ERROR_STOP=1", "-q", "-f", "-"],
        input=sql, text=True, env=env,
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
    )
    print(result.stdout, end="")
    if result.returncode != 0:
        print(f"ingest_conformance: psql failed (exit {result.returncode}) against "
              f"{env['PGDATABASE']}@{env['PGHOST']}:{env['PGPORT']}", file=sys.stderr)
        return result.returncode

    row_count = sum(
        len(entry["answer_key"]) + len(CONDITION_KEYS)
        for entry in matrix["substrates"].values()
    )
    print(f"ingest_conformance: wrote {row_count} rows "
          f"({len(matrix['substrates'])} substrates) into "
          f"{env['PGDATABASE']}.answer_key_results / .reflection_results")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
