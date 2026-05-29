#!/bin/bash
set -e

# Fixed, uncommon port pair (must match server.js): ODD = API, EVEN = client.
API_PORT=4801
CLIENT_PORT=4802

# Kill any existing instance on either port (restart cleanly).
for PORT in "$API_PORT" "$CLIENT_PORT"; do
  if lsof -Pi :"$PORT" -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo "Stopping existing server on port $PORT..."
    lsof -ti:"$PORT" | xargs kill -9 2>/dev/null || true
  fi
done
sleep 1

# Install dependencies if needed
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies..."
  npm install
fi

echo "Starting demo app:"
echo "  API    -> http://localhost:$API_PORT"
echo "  client -> http://localhost:$CLIENT_PORT"

# Once the client port is actually listening, open it in Chrome (once).
CLIENT_URL="http://localhost:$CLIENT_PORT"
(
  for _ in $(seq 1 40); do
    if lsof -Pi :"$CLIENT_PORT" -sTCP:LISTEN -t >/dev/null 2>&1; then
      if open -a "Google Chrome" "$CLIENT_URL" 2>/dev/null; then
        echo "Opened $CLIENT_URL in Chrome"
      else
        echo "Google Chrome not found — opening $CLIENT_URL in default browser"
        open "$CLIENT_URL" 2>/dev/null || true
      fi
      exit 0
    fi
    sleep 0.25
  done
  echo "Client port $CLIENT_PORT never came up — not opening a browser" >&2
) &

npm start
