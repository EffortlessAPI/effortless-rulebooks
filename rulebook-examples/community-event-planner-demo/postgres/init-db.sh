#!/bin/bash

# community_events - Database Initialization Script
# Drops and recreates the database, then applies all SQL scripts

set -e

# Configuration
DEFAULT_CONN="postgresql://postgres@localhost:5432/erb_community_event_planner_demo"
DB_CONN="${DATABASE_URL:-$DEFAULT_CONN}"

# Extract database name from connection string
DB_NAME=$(echo "$DB_CONN" | sed -n 's|.*//[^/]*/\([^?]*\).*|\1|p')

if [ -z "$DB_NAME" ]; then
  echo "Error: Could not extract database name from $DB_CONN"
  exit 1
fi

echo "Initializing database: $DB_NAME"

# Drop and recreate database
psql "${DB_CONN%/*}" -U postgres -c "DROP DATABASE IF EXISTS \"$DB_NAME\";" 2>/dev/null || true
psql "${DB_CONN%/*}" -U postgres -c "CREATE DATABASE \"$DB_NAME\";"

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
