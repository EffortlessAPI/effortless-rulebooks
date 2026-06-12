#!/usr/bin/env bash
# ============================================================================
# regenerate-answer-keys.sh
# ============================================================================
# Refresh the conformance ANSWER KEY from the current rulebook, using Postgres
# as the oracle — the one sanctioned way to compute derived values (CLAUDE.md:
# "conformance keys are computed by a substrate, never re-derived in Python").
#
# The cycle (all against an EPHEMERAL throwaway database, so this is safe to run
# arbitrarily and leaves the dev DB erb_<domain> untouched):
#
#   1. createdb  erb_<domain>_keygen_<pid>        (temp, unique per run)
#   2. init-db.sh against it                       rulebook raws -> tables + vw_*
#   3. pull-from-postgres.sh                       SELECT * FROM each vw_* -> JSON
#   4. postgres-calculated-to-rulebook (ERB_WRITE_COMPUTED=true)
#                                                  write the WHOLE view row
#                                                  (raws + computed) back into
#                                                  the rulebook's data[] arrays
#   5. orchestrator answer-key generation          reads the now-fresh computed
#                                                  values out of the rulebook
#   6. dropdb the temp database                    (always, via trap)
#
# Step 4 is why this works: the view's row IS the row. vw_<entity> already has
# the calc/lookup/aggregation columns computed by the SQL the transpiler emitted
# from the rulebook formulas — we just adopt them.
#
# WARNING — this WRITES to the rulebook JSON (step 4). The rulebook is sacred
# (CLAUDE.md): only computed VALUES change, never formulas/schema/raws. Commit
# or stash any unrelated rulebook edits before running, so the diff is clean
# and attributable.
#
# Usage:
#   bash regenerate-answer-keys.sh
#   ERB_DOMAIN=talismans-special-solutions bash regenerate-answer-keys.sh
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
REPO_ROOT="$(cd "$PROJECT_ROOT/../.." && pwd)"

ERB_DOMAIN="${ERB_DOMAIN:-talismans-special-solutions}"
RULEBOOK="$PROJECT_ROOT/effortless-rulebook/${ERB_DOMAIN}-rulebook.json"
[ -f "$RULEBOOK" ] || { echo "[keygen] rulebook not found: $RULEBOOK" >&2; exit 1; }

# Admin connection (to create/drop the temp DB) and the temp DB name. The temp
# name is unique per run so concurrent invocations don't collide.
ADMIN_URL="${ADMIN_DATABASE_URL:-postgresql://postgres@localhost:5432/postgres}"
TMP_DB="erb_${ERB_DOMAIN//-/_}_keygen_$$"
TMP_URL="postgresql://postgres@localhost:5432/${TMP_DB}"

# ----------------------------------------------------------------------
# Guard: refuse to run if the rulebook already has uncommitted changes we
# did NOT make. Step 4 writes the rulebook; we must be able to attribute the
# resulting diff. (CLAUDE.md: never silently revert/clobber the rulebook.)
# ----------------------------------------------------------------------
if command -v git >/dev/null 2>&1 && git -C "$REPO_ROOT" rev-parse >/dev/null 2>&1; then
    if ! git -C "$REPO_ROOT" diff --quiet -- "$RULEBOOK"; then
        echo "[keygen] REFUSING: $RULEBOOK has uncommitted changes." >&2
        echo "[keygen] Commit or stash them first so the answer-key refresh diff is clean." >&2
        exit 1
    fi
fi

# ----------------------------------------------------------------------
# Always drop the temp DB on exit (success, error, or interrupt).
# ----------------------------------------------------------------------
cleanup() {
    psql "$ADMIN_URL" -v ON_ERROR_STOP=1 -tAc \
        "DROP DATABASE IF EXISTS \"$TMP_DB\" WITH (FORCE);" >/dev/null 2>&1 \
        || dropdb --if-exists --force "$TMP_DB" 2>/dev/null || true
    echo "[keygen] dropped temp DB: $TMP_DB"
}
trap cleanup EXIT

echo "[keygen] domain:   $ERB_DOMAIN"
echo "[keygen] rulebook: $RULEBOOK"
echo "[keygen] temp DB:  $TMP_DB"

# 1. Create the throwaway DB.
echo "[keygen] [1/5] creating temp DB"
psql "$ADMIN_URL" -v ON_ERROR_STOP=1 -tAc "CREATE DATABASE \"$TMP_DB\";" >/dev/null

# 2. Build the schema + views into it from the current SQL (init-db.sh replays
#    the rulebook-derived SQL files; DATABASE_URL override points it at the temp).
echo "[keygen] [2/5] building schema + vw_* into temp DB"
DATABASE_URL="$TMP_URL" ERB_DOMAIN="$ERB_DOMAIN" bash "$SCRIPT_DIR/init-db.sh" "$TMP_URL"

# 3. Export every vw_<entity> (computed columns included) to the exchange JSON.
echo "[keygen] [3/5] exporting vw_* (computed values) to .pg-raw-data.json"
DATABASE_URL="$TMP_URL" bash "$SCRIPT_DIR/pull-from-postgres.sh"

# 4. Adopt the FULL view row (raws + computed) back into the rulebook.
echo "[keygen] [4/5] writing computed values back into the rulebook"
ERB_WRITE_COMPUTED=true python3 \
    "$REPO_ROOT/execution-substrates/postgres-calculated-to-rulebook/inject-into-postgres-calculated-to-rulebook.py" \
    "$RULEBOOK" "$SCRIPT_DIR/.pg-raw-data.json"

# 5. Regenerate the answer keys from the now-fresh rulebook. The orchestrator's
#    generate_all_answer_keys reads computed values straight from the rulebook
#    data[] arrays we just refreshed.
echo "[keygen] [5/5] regenerating answer keys from the refreshed rulebook"
ERB_DOMAIN="$ERB_DOMAIN" \
ERB_TESTING_DIR="${ERB_TESTING_DIR:-$PROJECT_ROOT/testing}" \
python3 - "$REPO_ROOT" "$RULEBOOK" <<'PY'
import sys, importlib.util, json
repo_root, rulebook_path = sys.argv[1], sys.argv[2]
spec = importlib.util.spec_from_file_location(
    "test_orchestrator", f"{repo_root}/orchestration/test-orchestrator.py")
to = importlib.util.module_from_spec(spec); spec.loader.exec_module(to)
rb = json.load(open(rulebook_path))
keys = to.generate_all_answer_keys(rb)
to.generate_all_blank_tests(keys, rb)
print(f"[keygen] wrote answer keys for {len(keys)} entities")
PY

echo "[keygen] DONE — answer keys refreshed. Review the rulebook diff:"
echo "[keygen]   git -C \"$REPO_ROOT\" diff -- \"$RULEBOOK\""
