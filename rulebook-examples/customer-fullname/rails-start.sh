#!/bin/bash
set -e

# Rails app uses port 4801 (completely separate from React/Node.js on 7001/7002)
RAILS_PORT=4801

RAILS_DIR="$(dirname "$0")/execution-substrates/rails"

if [ ! -d "$RAILS_DIR" ]; then
  echo "Error: Rails directory not found at $RAILS_DIR"
  exit 1
fi

cd "$RAILS_DIR"

# Kill any existing instance on this port (restart cleanly)
echo "Cleaning up port $RAILS_PORT..."
if lsof -Pi :"$RAILS_PORT" -sTCP:LISTEN -t >/dev/null 2>&1; then
  PID=$(lsof -Pi :"$RAILS_PORT" -sTCP:LISTEN -t)
  kill -9 "$PID" 2>/dev/null || true
  echo "Killed process on port $RAILS_PORT (PID: $PID)"
fi

sleep 1

# Ensure Ruby 3.2 is active
eval "$(rbenv init -)" || true

# Database is managed by effortless build - it should already exist
echo "Checking database..."
if ! psql -h localhost -U postgres -d erb_customer_fullname -c "SELECT 1" 2>/dev/null; then
  echo "ERROR: Database erb_customer_fullname not found."
  echo "Run 'effortless build' from the project root to initialize the database."
  exit 1
fi

# Mark Rails migration as applied (schema is already created by effortless build)
psql -h localhost -U postgres -d erb_customer_fullname << 'EOSQL'
CREATE TABLE IF NOT EXISTS schema_migrations (version varchar PRIMARY KEY);
INSERT INTO schema_migrations (version) VALUES ('1780079121864') ON CONFLICT DO NOTHING;
EOSQL

echo "Database ready."
echo ""
echo "================================"
echo "🚀 Starting Rails on port $RAILS_PORT"
echo "================================"
echo ""
echo "Visit: http://localhost:$RAILS_PORT"
echo ""

# Start Rails using Puma directly
bundle exec puma -b tcp://0.0.0.0:$RAILS_PORT -e development &
RAILS_PID=$!

# Give Rails a moment to start
sleep 4

# Check if server is running
if ! lsof -Pi :$RAILS_PORT -sTCP:LISTEN -t >/dev/null 2>&1; then
  echo "❌ Rails failed to start on port $RAILS_PORT"
  exit 1
fi

echo "✅ Rails is running on http://localhost:$RAILS_PORT"
echo ""

# Open Chrome
echo "Opening Chrome..."
if command -v open &> /dev/null; then
  open -a "Google Chrome" "http://localhost:$RAILS_PORT" 2>/dev/null || open "http://localhost:$RAILS_PORT"
elif command -v chrome &> /dev/null; then
  chrome "http://localhost:$RAILS_PORT" &
elif command -v google-chrome &> /dev/null; then
  google-chrome "http://localhost:$RAILS_PORT" &
fi

# Keep the script running
wait $RAILS_PID
