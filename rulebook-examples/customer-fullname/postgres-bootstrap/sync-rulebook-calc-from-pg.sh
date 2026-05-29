#!/usr/bin/env bash
# ============================================================================
# sync-rulebook-calc-from-pg.sh
# ============================================================================
# Run AFTER init-db.sh. Sources DATABASE_URL the same way init-db.sh does, then
# runs sync-rulebook-calc-from-pg.py, which refreshes the rulebook's calculated
# field values from the vw_* views. RAW values are verified, never written.
#
# This step is READ-ONLY against Postgres; the only thing it writes is the
# rulebook JSON, and only when every raw value already matches the DB.
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [ -f "$SCRIPT_DIR/effortless.env" ]; then
    # shellcheck disable=SC1090
    source "$SCRIPT_DIR/effortless.env"
fi

# DATABASE_URL is the single source of truth. No local default — fail loudly.
export DATABASE_URL="${DATABASE_URL:-${1:-}}"
if [ -z "$DATABASE_URL" ]; then
    echo "sync-rulebook-calc-from-pg.sh: DATABASE_URL is not set." >&2
    echo "  Set it in effortless.env (see effortless.env.example), in your shell," >&2
    echo "  or pass it as the first argument to this script." >&2
    exit 1
fi

exec python3 "$SCRIPT_DIR/sync-rulebook-calc-from-pg.py"
