#!/usr/bin/env bash
# refresh-facts-from-postgres.sh
#
# Pulls the *raw* fact rows out of a live Postgres database and merges them
# into the project's effortless-rulebook.json, emitting a sidecar file:
#
#   effortless-rulebook.with-facts.json
#
# The sidecar is what you feed to rulebook-to-xlsx (and to the explainer-dag
# transpilers) when you want the generated artifacts to reflect *production*
# data instead of the seed data baked into the SSOT.
#
# IMPORTANT — RAW FACTS ONLY
#   This script only reads from base tables (NOT views). The explainer DAG
#   recomputes every calculated / lookup / aggregation field client-side from
#   the raw facts, so pulling calculated data would be redundant (and would
#   actually break the provenance story).
#
# Usage:
#   ./refresh-facts-from-postgres.sh \
#       --rulebook ../effortless-rulebook/effortless-rulebook.json \
#       --db "postgresql://user:pass@host:5432/dbname"
#
# Env-var equivalents (used if a flag is omitted):
#   EFFORTLESS_RULEBOOK_PATH
#   DATABASE_URL
#
# Requires: psql, jq

set -euo pipefail

RULEBOOK_PATH="${EFFORTLESS_RULEBOOK_PATH:-}"
DATABASE_URL="${DATABASE_URL:-}"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --rulebook) RULEBOOK_PATH="$2"; shift 2 ;;
    --db)       DATABASE_URL="$2";  shift 2 ;;
    -h|--help)
      sed -n '2,30p' "$0"
      exit 0
      ;;
    *) echo "Unknown arg: $1" >&2; exit 2 ;;
  esac
done

if [[ -z "$RULEBOOK_PATH" || -z "$DATABASE_URL" ]]; then
  echo "Error: --rulebook and --db are required (or set EFFORTLESS_RULEBOOK_PATH / DATABASE_URL)." >&2
  exit 2
fi

if [[ ! -f "$RULEBOOK_PATH" ]]; then
  echo "Error: rulebook not found at $RULEBOOK_PATH" >&2
  exit 1
fi

command -v psql >/dev/null || { echo "psql is required" >&2; exit 1; }
command -v jq   >/dev/null || { echo "jq is required" >&2; exit 1; }

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
DUMP_SQL="${SCRIPT_DIR}/dump-raw-facts.sql"

OUT_DIR="$( dirname "$RULEBOOK_PATH" )"
OUT_PATH="${OUT_DIR}/effortless-rulebook.with-facts.json"

TMP_FACTS="$(mktemp -t erb-facts.XXXXXX.json)"
trap 'rm -f "$TMP_FACTS"' EXIT

echo "→ Reading raw facts from $DATABASE_URL"
psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -At -f "$DUMP_SQL" > "$TMP_FACTS"

echo "→ Merging facts into $OUT_PATH"
# Replace each table's `data` array in the rulebook with the array pulled from
# Postgres, indexed by table name. Tables present in the rulebook but absent
# from the DB are left with their original seed data (and a warning is logged).
jq --slurpfile facts "$TMP_FACTS" '
  ($facts[0]) as $f
  | reduce (keys_unsorted[]) as $k (
      .;
      if (.[$k] | type) == "object" and (.[$k] | has("schema"))
      then
        if ($f | has($k))
        then .[$k].data = $f[$k]
        else .
        end
      else .
      end
    )
' "$RULEBOOK_PATH" > "$OUT_PATH"

echo "✓ Wrote $OUT_PATH"
echo ""
echo "Next steps:"
echo "  1. Feed $OUT_PATH to rulebook-to-xlsx → produces rulebook.xlsx with live data"
echo "  2. Feed $OUT_PATH to rulebook-to-react-explainer-dag → DAG sees live facts"
