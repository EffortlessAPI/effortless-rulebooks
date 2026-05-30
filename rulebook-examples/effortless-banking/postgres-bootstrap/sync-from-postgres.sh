#!/usr/bin/env bash
# Full sync: export raw Postgres data as JSON, then merge into rulebook.
#   1. pull-from-postgres.sh  — queries each base table, writes .pg-raw-data.json
#   2. inject-into-postgres-calculated-to-rulebook.py — reads JSON, updates rulebook

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../../../../" && pwd)"
TRANSPILER="$REPO_ROOT/execution-substrates/postgres-calculated-to-rulebook/inject-into-postgres-calculated-to-rulebook.py"

RULEBOOK_PATH="${ERB_RULEBOOK_PATH:-}"
if [ -z "$RULEBOOK_PATH" ]; then
    CANDIDATES=("$SCRIPT_DIR"/../effortless-rulebook/*-rulebook.json)
    [ ${#CANDIDATES[@]} -eq 0 ] && { echo "sync-from-postgres.sh: no rulebook found" >&2; exit 1; }
    RULEBOOK_PATH="${CANDIDATES[0]}"
fi
[ -f "$RULEBOOK_PATH" ] || { echo "sync-from-postgres.sh: rulebook not found: $RULEBOOK_PATH" >&2; exit 1; }
[ -f "$TRANSPILER" ] || { echo "sync-from-postgres.sh: transpiler missing: $TRANSPILER" >&2; exit 1; }

echo "Step 1: exporting raw Postgres data to JSON..." >&2
bash "$SCRIPT_DIR/pull-from-postgres.sh"

echo "Step 2: merging JSON into rulebook..." >&2
ERB_RULEBOOK_PATH="$RULEBOOK_PATH" python3 "$TRANSPILER"

echo "Done." >&2
