#!/bin/bash

set -e

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
APP_DIR="$PROJECT_ROOT/app"

PORT=$((3140 + RANDOM % 1000))

echo "Starting Star Trek Explorer..."
echo

# Kill any existing Node processes
echo "Stopping any existing processes..."
pkill -f "npm start" 2>/dev/null || true
pkill -f "node server.js" 2>/dev/null || true
sleep 1
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
echo "Starting Star Trek Explorer on http://localhost:$PORT"
echo
APP_PORT=$PORT npm start
