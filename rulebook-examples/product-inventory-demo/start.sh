#!/bin/bash
set -e

PORT_SERVER="${PORT_SERVER:-3032}"
PORT_WEB="${PORT_WEB:-5175}"
DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_product_inventory_demo}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

stop_port() {
  local port="$1"
  local pids
  pids="$(lsof -ti tcp:"$port" 2>/dev/null || true)"
  if [ -n "$pids" ]; then
    echo "  stopping process(es) on port $port: $pids"
    kill $pids 2>/dev/null || true
    sleep 1
    pids="$(lsof -ti tcp:"$port" 2>/dev/null || true)"
    if [ -n "$pids" ]; then
      kill -9 $pids 2>/dev/null || true
    fi
  fi
}

stop_all() {
  echo "Stopping any running services..."
  stop_port "$PORT_SERVER"
  stop_port "$PORT_WEB"
}

case "${1:-all}" in
  all)
    stop_all
    echo "Starting all services..."
    (cd "$SCRIPT_DIR/server" && PORT=$PORT_SERVER npm run dev) &
    (cd "$SCRIPT_DIR/web" && PORT=$PORT_WEB npm run dev) &
    sleep 2
    echo ""
    echo "✓ Server running on http://localhost:$PORT_SERVER"
    echo "✓ Web app running on http://localhost:$PORT_WEB"
    echo ""
    wait
    ;;
  server)
    stop_port "$PORT_SERVER"
    echo "Starting server on port $PORT_SERVER..."
    cd "$SCRIPT_DIR/server"
    npm install >/dev/null 2>&1 || true
    PORT=$PORT_SERVER npm run dev
    ;;
  web)
    stop_port "$PORT_WEB"
    echo "Starting web on port $PORT_WEB..."
    cd "$SCRIPT_DIR/web"
    npm install >/dev/null 2>&1 || true
    PORT=$PORT_WEB npm run dev
    ;;
  stop)
    stop_all
    echo "✓ All services stopped"
    ;;
  *)
    echo "Usage: $0 {all|server|web|stop}"
    echo ""
    echo "  all    - stop and restart server + web (parallel)"
    echo "  server - stop and restart Express server on port $PORT_SERVER"
    echo "  web    - stop and restart Vite dev server on port $PORT_WEB"
    echo "  stop   - stop server + web"
    echo ""
    echo "Build and DB init are separate (run them yourself):"
    echo "  effortless build           - regenerate postgres-bootstrap/"
    echo "  ./postgres-bootstrap/init-db.sh      - drop/recreate DB and apply schema"
    echo ""
    echo "Environment variables:"
    echo "  PORT_SERVER=$PORT_SERVER"
    echo "  PORT_WEB=$PORT_WEB"
    echo "  DATABASE_URL=$DATABASE_URL"
    exit 1
    ;;
esac
