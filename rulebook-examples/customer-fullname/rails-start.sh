#!/bin/bash
set -e

# Ports: ODD = API/Rails, EVEN = Client (if needed in future)
API_PORT=4801
CLIENT_PORT=4802

RAILS_DIR="$(dirname "$0")/execution-substrates/rails"

if [ ! -d "$RAILS_DIR" ]; then
  echo "Error: Rails directory not found at $RAILS_DIR"
  exit 1
fi

cd "$RAILS_DIR"

# Kill any existing instances on either port
echo "Cleaning up ports $API_PORT and $CLIENT_PORT..."
for PORT in "$API_PORT" "$CLIENT_PORT"; do
  if lsof -Pi :"$PORT" -sTCP:LISTEN -t >/dev/null 2>&1; then
    PID=$(lsof -Pi :"$PORT" -sTCP:LISTEN -t)
    kill -9 "$PID" 2>/dev/null || true
    echo "Killed process on port $PORT (PID: $PID)"
  fi
done

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
echo "🚀 Starting Rails on port $API_PORT"
echo "================================"
echo ""
echo "Visit: http://localhost:$API_PORT"
echo ""

# Start Rails using Puma directly
bundle exec puma -b tcp://0.0.0.0:$API_PORT -e development &
RAILS_PID=$!

# Give Rails a moment to start
sleep 4

# Check if server is running
if ! lsof -Pi :$API_PORT -sTCP:LISTEN -t >/dev/null 2>&1; then
  echo "❌ Rails failed to start on port $API_PORT"
  exit 1
fi

echo "✅ Rails is running on http://localhost:$API_PORT"
echo ""

# Open Chrome
echo "Opening Chrome..."
if command -v open &> /dev/null; then
  open -a "Google Chrome" "http://localhost:$API_PORT" 2>/dev/null || open "http://localhost:$API_PORT"
elif command -v chrome &> /dev/null; then
  chrome "http://localhost:$API_PORT" &
elif command -v google-chrome &> /dev/null; then
  google-chrome "http://localhost:$API_PORT" &
fi

# Keep the script running
wait $RAILS_PID
