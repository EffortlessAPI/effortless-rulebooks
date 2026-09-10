#!/usr/bin/env bash
# ============================================================================
# pull-from-postgres.sh  (repo-root, domain-agnostic)
# ============================================================================
# Export every vw_<entity> row as JSON — the exchange format
# inject-into-postgres-calculated-to-rulebook.py reads. Views, not base tables:
# the view row carries the calculated/lookup/aggregation columns the SQL
# transpiler emitted from the rulebook formulas, and adopting those is the whole
# point.
#
# Per-project copies of this script exist in the 26 projects whose Postgres
# output lands in postgres-bootstrap/, and nowhere else — projects using
# postgres/ or effortless-postgres/ never had one. This root copy is the one the
# root answer-key regeneration uses, so no project needs to ship its own.
#
# Usage:
#   DATABASE_URL=... bash orchestration/pull-from-postgres.sh <output-json-path>
# ============================================================================

set -euo pipefail

OUTPUT="${1:?usage: pull-from-postgres.sh <output-json-path>}"
: "${DATABASE_URL:?DATABASE_URL must be set}"

VIEWS=$(psql "$DATABASE_URL" -t -A -c \
  "SELECT table_name FROM information_schema.views
    WHERE table_schema='public' AND table_name LIKE 'vw\\_%' ORDER BY 1")

if [ -z "$VIEWS" ]; then
    echo "pull-from-postgres.sh: no vw_* views found in $DATABASE_URL" >&2
    exit 1
fi

# The injector keys off the ENTITY name, so strip the vw_ prefix.
printf '{' > "$OUTPUT"
FIRST=true
while IFS= read -r VIEW; do
    [ -z "$VIEW" ] && continue
    [ "$FIRST" = false ] && printf ',' >> "$OUTPUT"
    FIRST=false
    printf '"%s":' "${VIEW#vw_}" >> "$OUTPUT"
    psql "$DATABASE_URL" -t -A -c \
      "SELECT COALESCE(json_agg(row_to_json(t)), '[]'::json) FROM \"$VIEW\" t" >> "$OUTPUT"
done <<< "$VIEWS"
printf '}' >> "$OUTPUT"

COUNT=$(echo "$VIEWS" | grep -c .)
echo "Exported $COUNT view(s) to $OUTPUT" >&2
