#!/usr/bin/env bash
# Export COMPUTED entity data as JSON for the postgres-calculated-to-rulebook
# transpiler.
#
# Queries every vw_<entity> VIEW (NOT the base tables) and writes
# .pg-raw-data.json. The view's row IS the row (CLAUDE.md "The view IS the
# contract"): each vw_<entity> exposes the raw columns AND the computed
# calc/lookup/aggregation columns side by side, all populated by the SQL the
# transpiler emitted from the rulebook formulas. Exporting the views is what
# lets the pull-back refresh COMPUTED values in the rulebook, not just raws —
# the whole point of regenerating the answer key.
#
# The JSON file is the exchange format the transpiler reads — no SQL parsing
# needed. It is keyed by the ENTITY snake-name (the vw_ prefix stripped), so
# `snake("Roles") == "roles"` lines up with the `vw_roles` export.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
[ -f "$SCRIPT_DIR/effortless.env" ] && source "$SCRIPT_DIR/effortless.env"

DATABASE_URL="${DATABASE_URL:-}"
[ -z "$DATABASE_URL" ] && { echo "pull-from-postgres.sh: DATABASE_URL not set" >&2; exit 1; }

OUTPUT="$SCRIPT_DIR/.pg-raw-data.json"

# Every vw_<entity> view EXCEPT the *_closure views. Closure views
# (vw_roles_closure, vw_step_precedence_closure) materialize a transitive
# relation as their own rowset — they are NOT entity tables and have no
# rulebook data array to merge into, so the transpiler would only warn on them.
VIEWS=$(psql "$DATABASE_URL" -t -A -c \
  "SELECT table_name FROM information_schema.views
   WHERE table_schema='public'
     AND table_name LIKE 'vw_%'
     AND table_name NOT LIKE '%\_closure'
   ORDER BY 1")

if [ -z "$VIEWS" ]; then
    echo "pull-from-postgres.sh: no vw_* views found in database" >&2
    echo "  (did init-db.sh run? views are created by 03-create-views.sql)" >&2
    exit 1
fi

printf '{' > "$OUTPUT"
FIRST=true
while IFS= read -r VIEW; do
    [ -z "$VIEW" ] && continue
    ENTITY="${VIEW#vw_}"                       # vw_roles -> roles (entity snake-name)
    [ "$FIRST" = false ] && printf ',' >> "$OUTPUT"
    FIRST=false
    printf '"%s":' "$ENTITY" >> "$OUTPUT"
    psql "$DATABASE_URL" -t -A -c \
      "SELECT COALESCE(json_agg(row_to_json(t)), '[]'::json) FROM $VIEW t" >> "$OUTPUT"
done <<< "$VIEWS"
printf '}' >> "$OUTPUT"

COUNT=$(echo "$VIEWS" | grep -c .)
echo "Exported $COUNT view(s) to $OUTPUT" >&2
