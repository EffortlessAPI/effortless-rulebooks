#!/bin/bash
set -e

# Rails app uses port 3001 (completely separate from React 7001/7002)
RAILS_PORT=3001

# Kill any existing instance on this port (restart cleanly).
if lsof -Pi :"$RAILS_PORT" -sTCP:LISTEN -t >/dev/null 2>&1; then
  echo "Stopping existing Rails server on port $RAILS_PORT..."
  lsof -ti:"$RAILS_PORT" | xargs kill -9 2>/dev/null || true
fi
sleep 1

echo "Starting Rails app:"
echo "  Rails -> http://localhost:$RAILS_PORT"

bundle exec rails server -p "$RAILS_PORT"
