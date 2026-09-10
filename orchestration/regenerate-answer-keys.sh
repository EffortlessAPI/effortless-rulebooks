#!/usr/bin/env bash
# ============================================================================
# regenerate-answer-keys.sh  (repo-root, domain-agnostic)
# ============================================================================
# Refresh one project's conformance ANSWER KEYS using Postgres as the oracle —
# the one sanctioned way to compute derived values. CLAUDE.md: "conformance keys
# are computed by a substrate, never re-derived in Python." The Python engine in
# orchestration/formula_parser.py has no INDEX/MATCH/COUNTIFS at all and is not
# supposed to; when a project's rulebook carries no stored value for a
# cross-table field, answer-key generation fails with "NO computable value" and
# THIS is the fix.
#
# The cycle runs entirely against an EPHEMERAL database, so it is safe to run
# arbitrarily and leaves the dev DB erb_<domain> untouched:
#
#   1. createdb erb_<domain>_keygen_<pid>     temp, unique per run
#   2. reset-rulebook-db.sh against it        rulebook raws -> tables + vw_*
#   3. orchestration/pull-from-postgres.sh    SELECT * FROM each vw_* -> JSON
#   4. inject (ERB_WRITE_COMPUTED=true)       adopt the COMPUTED columns only
#   5. answer-key generation                  reads the now-fresh values
#   6. dropdb the temp database               always, via trap
#
# Step 4 is computed-only. It used to adopt the whole view row, raws included,
# which was lossy: the temp DB was itself loaded FROM those raws in step 2, so
# writing them back carried no information — only Postgres's round-trip
# representation, which rewrote datetimes with the running machine's timezone
# (cr-24). Raws are never written here.
#
# This was promoted from rulebook-examples/talismans-special-solutions, which was
# the only project that had it. Everything domain-specific is resolved from
# ERB_DOMAIN and the project's own effortless.json — including which directory
# rulebook-to-postgres writes to (postgres/, postgres-bootstrap/ and
# effortless-postgres/ are all in use) and which of the two valid hub filenames
# the project uses.
#
# Usage:
#   ERB_DOMAIN=<slug> bash orchestration/regenerate-answer-keys.sh
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

: "${ERB_DOMAIN:?ERB_DOMAIN must be set to the project slug}"

# Resolve the project's paths from its own manifest rather than assuming a
# layout. Fails loudly on a missing/ambiguous hub or a project with no Postgres.
read -r PROJECT_ROOT RULEBOOK PG_DIR <<<"$(python3 - "$ERB_DOMAIN" <<'PY'
import sys, os
sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(sys.argv[0])) or ".", "scripts"))
sys.path.insert(0, "scripts")
from erb_project import find_domain_dir, rulebook_path, postgres_output_dir
slug = sys.argv[1]
d = find_domain_dir(slug)
pg = postgres_output_dir(d)
if pg is None:
    raise SystemExit(f"{slug} registers no enabled rulebook-to-postgres step — "
                     f"there is no substrate to compute answer keys with.")
print(d, rulebook_path(slug, d), pg)
PY
)"

ADMIN_URL="${ADMIN_DATABASE_URL:-postgresql://postgres@localhost:5432/postgres}"
TMP_DB="erb_${ERB_DOMAIN//-/_}_keygen_$$"
TMP_URL="postgresql://postgres@localhost:5432/${TMP_DB}"

# Step 4 writes the rulebook, so the resulting diff must be attributable.
# CLAUDE.md: never silently clobber a rulebook carrying edits you did not make.
if command -v git >/dev/null 2>&1 && git -C "$REPO_ROOT" rev-parse >/dev/null 2>&1; then
    if ! git -C "$REPO_ROOT" diff --quiet -- "$RULEBOOK"; then
        echo "[keygen] REFUSING: $RULEBOOK has uncommitted changes." >&2
        echo "[keygen] Commit or stash them first so the answer-key refresh diff is clean." >&2
        exit 1
    fi
fi

cleanup() {
    psql "$ADMIN_URL" -v ON_ERROR_STOP=1 -tAc \
        "DROP DATABASE IF EXISTS \"$TMP_DB\" WITH (FORCE);" >/dev/null 2>&1 \
        || dropdb --if-exists --force "$TMP_DB" 2>/dev/null || true
    echo "[keygen] dropped temp DB: $TMP_DB"
}
trap cleanup EXIT

echo "[keygen] domain:   $ERB_DOMAIN"
echo "[keygen] rulebook: $RULEBOOK"
echo "[keygen] postgres: $PG_DIR"
echo "[keygen] temp DB:  $TMP_DB"

echo "[keygen] [1/5] creating temp DB"
psql "$ADMIN_URL" -v ON_ERROR_STOP=1 -tAc "CREATE DATABASE \"$TMP_DB\";" >/dev/null

# reset-rulebook-db.sh is regenerated every build and routinely loses its
# executable bit, so it is invoked through bash, never as ./script.
echo "[keygen] [2/5] building schema + vw_* into temp DB"
DATABASE_URL="$TMP_URL" ERB_DOMAIN="$ERB_DOMAIN" bash "$PG_DIR/reset-rulebook-db.sh" "$TMP_URL" >/dev/null

echo "[keygen] [3/5] exporting vw_* (computed values) to .pg-raw-data.json"
DATABASE_URL="$TMP_URL" bash "$SCRIPT_DIR/pull-from-postgres.sh" "$PG_DIR/.pg-raw-data.json"

echo "[keygen] [4/5] adopting the computed columns into the rulebook (raws untouched)"
ERB_WRITE_COMPUTED=true python3 \
    "$REPO_ROOT/execution-substrates/postgres-calculated-to-rulebook/inject-into-postgres-calculated-to-rulebook.py" \
    "$RULEBOOK" "$PG_DIR/.pg-raw-data.json"

echo "[keygen] [5/5] regenerating answer keys from the refreshed rulebook"
ERB_DOMAIN="$ERB_DOMAIN" \
ERB_RULEBOOK_PATH="$RULEBOOK" \
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
