#!/bin/bash
set -e

COMMAND="${1:-all}"
DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_fantasy_football}"

cleanup() {
  echo "Stopping any existing services..."
  pkill -f "tsx watch src/index.ts" 2>/dev/null || true
  pkill -f "npm run dev" 2>/dev/null || true
}

case "$COMMAND" in
  build)
    echo "Running effortless build..."
    effortless build
    ;;
  db)
    echo "Initializing database..."
    export DATABASE_URL
    bash postgres-bootstrap/init-db.sh
    ;;
  server)
    cleanup
    echo "Starting server on port 3045..."
    cd server
    npm install 2>/dev/null || true
    npx tsx watch src/index.ts
    ;;
  web)
    cleanup
    echo "Starting web on port 5188..."
    cd web
    npm install 2>/dev/null || true
    npm run dev
    ;;
  all)
    cleanup
    echo "Starting servers..."
    echo "  Server: http://localhost:3045"
    echo "  Web:    http://localhost:5188"
    echo ""
    ( cd server && npm install 2>/dev/null || true && npx tsx watch src/index.ts ) &
    SERVER_PID=$!
    sleep 2
    ( cd web && npm install 2>/dev/null || true && npm run dev ) &
    WEB_PID=$!
    trap "kill $SERVER_PID $WEB_PID 2>/dev/null; exit 0" SIGINT SIGTERM
    wait
    ;;
  *)
    echo "Usage: $0 {all|build|db|server|web}"
    exit 1
    ;;
esac
