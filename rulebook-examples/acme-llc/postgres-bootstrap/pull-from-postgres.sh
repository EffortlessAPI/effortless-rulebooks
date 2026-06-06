#!/usr/bin/env bash
# Export raw table data as JSON for the postgres-calculated-to-rulebook transpiler.
# Queries every base table (not views) and writes .pg-raw-data.json.
# The JSON file is the exchange format the transpiler reads — no SQL parsing needed.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
[ -f "$SCRIPT_DIR/effortless.env" ] && source "$SCRIPT_DIR/effortless.env"

DATABASE_URL="${DATABASE_URL:-}"
[ -z "$DATABASE_URL" ] && { echo "pull-from-postgres.sh: DATABASE_URL not set" >&2; exit 1; }

OUTPUT="$SCRIPT_DIR/.pg-raw-data.json"

TABLES=$(psql "$DATABASE_URL" -t -A -c \
  "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE' ORDER BY 1")

if [ -z "$TABLES" ]; then
    echo "pull-from-postgres.sh: no base tables found in database" >&2
    exit 1
fi

printf '{' > "$OUTPUT"
FIRST=true
while IFS= read -r TABLE; do
    [ -z "$TABLE" ] && continue
    [ "$FIRST" = false ] && printf ',' >> "$OUTPUT"
    FIRST=false
    printf '"%s":' "$TABLE" >> "$OUTPUT"
    psql "$DATABASE_URL" -t -A -c \
      "SELECT COALESCE(json_agg(row_to_json(t)), '[]'::json) FROM $TABLE t" >> "$OUTPUT"
done <<< "$TABLES"
printf '}' >> "$OUTPUT"

COUNT=$(echo "$TABLES" | grep -c .)
echo "Exported $COUNT table(s) to $OUTPUT" >&2
