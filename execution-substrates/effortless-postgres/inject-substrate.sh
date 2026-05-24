#!/bin/bash
set -e
set -o pipefail

# inject-substrate.sh for effortless-postgres execution substrate
#
# For Effortless-licensed substrates, we run effortless -buildLocal to regenerate
# from the rulebook. This allows postgres to be rebuilt independently
# without rebuilding all other substrates.
#
# Structure:
#   /licensed-effortless-tools/postgres/     - Effortless transpiler output (SQL files)
#   /execution-substrates/effortless-postgres/  - Test runner (this folder)

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
# Domain-scoped postgres dir: rulebook-examples/<domain>/postgres. Fall back to
# the legacy global location for non-Effortless callers that don't set ERB_DOMAIN_DIR.
if [ -n "$ERB_DOMAIN_DIR" ] && [ -d "$ERB_DOMAIN_DIR/postgres" ]; then
    POSTGRES_SSOTME_DIR="$ERB_DOMAIN_DIR/postgres"
else
    POSTGRES_SSOTME_DIR="$PROJECT_ROOT/licensed-effortless-tools/postgres"
fi

echo "=== Effortless-PostgreSQL Substrate: Regenerating from rulebook ==="
echo "  Postgres dir: $POSTGRES_SSOTME_DIR"

if command -v effortless &> /dev/null; then
    cd "$POSTGRES_SSOTME_DIR"
    effortless -buildLocal
else
    echo "Warning: effortless CLI not installed, skipping regeneration"
    echo "Install effortless or run 'effortless build' from project root"
fi

# Initialise the local Postgres DB from the freshly-generated SQL so the
# vw_* views reflect the current rulebook before take-test queries them.
# init-db.sh targets a per-domain database; create it on demand so each
# domain owns its own pristine schema.
if [ -f "$POSTGRES_SSOTME_DIR/init-db.sh" ]; then
    DB_NAME=$(grep -oE 'localhost:[0-9]+/[a-zA-Z0-9_-]+' "$POSTGRES_SSOTME_DIR/init-db.sh" | head -1 | sed 's/.*\///')
    if [ -n "$DB_NAME" ]; then
        if ! psql -U postgres -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname='$DB_NAME'" 2>/dev/null | grep -q 1; then
            echo "  Creating database '$DB_NAME'..."
            createdb -U postgres "$DB_NAME" || psql -U postgres -d postgres -c "CREATE DATABASE \"$DB_NAME\";"
        fi
    fi
    echo "  Re-initializing database from $POSTGRES_SSOTME_DIR/init-db.sh"
    bash "$POSTGRES_SSOTME_DIR/init-db.sh" 2>&1 | tail -10
fi

cd "$SCRIPT_DIR"

# Run the test for this substrate
"$SCRIPT_DIR/take-test.sh"
