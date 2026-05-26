#!/bin/bash

set -e

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
APP_DIR="$PROJECT_ROOT/app"

echo "Starting Star Trek Explorer..."
echo

# Kill any existing process on port 3000
echo "Stopping any existing process on port 3000..."
lsof -ti :3000 | xargs kill -9 2>/dev/null || true
echo

# Ensure dependencies are installed
if [ ! -d "$APP_DIR/node_modules" ]; then
  echo "Installing dependencies..."
  cd "$APP_DIR"
  npm install
  echo
fi

# Ensure the database is initialized
if ! psql -U postgres -d erb_star_trek -c "SELECT 1" > /dev/null 2>&1; then
  echo "Initializing database..."
  cd "$PROJECT_ROOT/postgres-bootstrap"
  ./init-db.sh
  echo
fi

# Start the server
cd "$APP_DIR"
echo "Starting server on http://localhost:3000"
npm start
