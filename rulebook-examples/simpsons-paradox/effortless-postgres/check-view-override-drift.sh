#!/usr/bin/env bash
# ============================================================================
# check-view-override-drift.sh
# ============================================================================
# Loop-97: build-time conformance check for the exact drift hazard loop-93
# named in its NextSuggestion — a hand-maintained 03b-customize-views.sql
# override (vw_studies, vw_treatment_rankings, vw_model_summary) silently
# falling behind when 03-create-views.sql regenerates with new/renamed
# columns, because nothing diffs the two.
#
# How it works: 03-create-views.sql's native CREATE VIEW statement for each
# overridden view is extracted verbatim (the DROP VIEW + CREATE VIEW pair
# immediately preceding that view's body, to the next standalone DROP VIEW
# line) and re-run against the live DB under a "__native_check" suffix name,
# so Postgres — not a hand-rolled parser — resolves its real output columns
# via information_schema. Those are then diffed against the live (03b-
# overridden) view's columns. Fails loudly if the override dropped a native
# column; passes if the override kept every native column (regardless of
# whatever extra lookup columns it legitimately adds on top).
#
# Run after init-db.sh. Exits non-zero on drift so it can gate CI/build.
# ============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DEFAULT_CONN="postgresql://postgres@localhost:5432/simpsons_paradox"
DATABASE_URL="${DATABASE_URL:-${1:-$DEFAULT_CONN}}"
NATIVE_VIEWS_SQL="${SCRIPT_DIR}/03-create-views.sql"

# Views 03b-customize-views.sql overrides (DROP + CREATE with the same name).
# If a future loop adds another override, add its name here.
OVERRIDDEN_VIEWS=(vw_studies vw_treatment_rankings vw_model_summary)

FAIL=0

for view in "${OVERRIDDEN_VIEWS[@]}"; do
    native_check_view="${view}__native_check"

    # Extract the native CREATE VIEW block verbatim. 03-create-views.sql
    # opens with a bulk "DROP VIEW IF EXISTS <every view> CASCADE;" listing,
    # so a naive "first DROP VIEW line for this view" anchor matches the
    # WRONG occurrence and captures nothing — anchor on the
    # "DROP VIEW ...; CREATE VIEW <view> " PAIR immediately preceding this
    # view's real body instead, then capture to (not including) the next
    # standalone "DROP VIEW IF EXISTS" line.
    block=$(awk -v view="$view" '
        BEGIN { capture = 0 }
        capture && /^DROP VIEW IF EXISTS / { exit }
        $0 ~ ("^CREATE VIEW " view " ") && prev ~ ("^DROP VIEW IF EXISTS " view " CASCADE;$") { capture = 1; print prev; print; next }
        capture { print }
        { prev = $0 }
    ' "$NATIVE_VIEWS_SQL")

    if [ -z "$block" ]; then
        echo "[view-drift] ERROR: could not find native CREATE VIEW block for $view in $NATIVE_VIEWS_SQL" >&2
        FAIL=1
        continue
    fi

    # Rename the view in the extracted block so it doesn't collide with the
    # real (overridden) one, and run it in a throwaway transaction that we
    # always roll back — this never touches persistent state.
    renamed_block="${block//DROP VIEW IF EXISTS ${view} CASCADE;/DROP VIEW IF EXISTS ${native_check_view} CASCADE;}"
    renamed_block="${renamed_block//CREATE VIEW ${view} /CREATE VIEW ${native_check_view} }"

    native_cols=$(psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -qtA <<SQL
BEGIN;
${renamed_block}
SELECT string_agg(column_name, E'\n' ORDER BY column_name)
FROM information_schema.columns
WHERE table_name = '${native_check_view}';
ROLLBACK;
SQL
)

    live_cols=$(psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -qtA -c "
        SELECT string_agg(column_name, E'\n' ORDER BY column_name)
        FROM information_schema.columns
        WHERE table_name = '${view}';
    ")

    missing=$(comm -23 <(echo "$native_cols" | tr -d ' ' | sort) <(echo "$live_cols" | tr -d ' ' | sort))

    if [ -n "$missing" ]; then
        echo "[view-drift] FAIL: ${view} — 03b-customize-views.sql's override is missing columns that 03-create-views.sql's native version has:" >&2
        echo "$missing" | sed 's/^/[view-drift]   - /' >&2
        echo "[view-drift]   Fix: add the missing column(s) to the override in effortless-postgres/03b-customize-views.sql." >&2
        FAIL=1
    else
        echo "[view-drift] PASS: ${view} — override exposes every native column."
    fi
done

if [ "$FAIL" -ne 0 ]; then
    echo "[view-drift] one or more overridden views have drifted from their native definitions. See loop-93/loop-97." >&2
    exit 1
fi

echo "[view-drift] all overridden views are in sync with their native column sets."
