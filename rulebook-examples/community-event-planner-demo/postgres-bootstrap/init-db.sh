#!/bin/bash

# community_events - Database Initialization Script
# Drops and recreates the database, then applies all SQL scripts

set -e

# Configuration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Source effortless.env if it exists (provides DATABASE_URL default)
if [ -f "$SCRIPT_DIR/effortless.env" ]; then
    # shellcheck disable=SC1090
    source "$SCRIPT_DIR/effortless.env"
fi

# DATABASE_URL is the single source of truth — set it in effortless.env (or your
# shell). There is NO local default: a missing value should fail loudly so it's
# obvious you need to copy effortless.env.example -> effortless.env.
if [ -z "${DATABASE_URL:-}" ]; then
    echo "init-db.sh: DATABASE_URL is not set." >&2
    echo "  Set it in effortless.env (see effortless.env.example) or in your shell." >&2
    exit 1
fi
DB_CONN="$DATABASE_URL"

# Extract database name from connection string
DB_NAME=$(echo "$DB_CONN" | sed -n 's|.*//[^/]*/\([^?]*\).*|\1|p')

if [ -z "$DB_NAME" ]; then
  echo "Error: Could not extract database name from $DB_CONN"
  exit 1
fi

echo "Ensuring database exists: $DB_NAME"

# Create database if it doesn't exist
psql "${DB_CONN%/*}" -U postgres -c "CREATE DATABASE IF NOT EXISTS \"$DB_NAME\";" 2>/dev/null || true

echo "Database created. Running migrations..."

# Run all SQL files in order (following ERB pattern)
for sql_file in $(dirname "$0")/{01-drop-and-create-tables,01b-customize-schema,02-create-functions,02b-customize-functions,03-create-views,03b-customize-views,04-create-policies,04b-customize-policies,05-insert-data,05b-customize-data,99-fk-constraints}.sql; do
  if [ -f "$sql_file" ]; then
    echo "  Applying $(basename "$sql_file")..."
    psql "$DB_CONN" -f "$sql_file" > /dev/null
  fi
done

echo "✓ Database initialized successfully"
psql "$DB_CONN" -c "SELECT COUNT(*) as table_count FROM information_schema.tables WHERE table_schema='public';" 2>/dev/null | head -3
