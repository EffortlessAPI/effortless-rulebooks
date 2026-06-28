#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT"

APP_DIR="$ROOT/admin-app"

# Fixed ports:  backend (Express API) 6347  <-  frontend (Vite/React) 6348
API_PORT=6347
CLIENT_PORT=6348

# Kill whatever holds a TCP port, so a re-run is a clean restart, not a clash.
free_port() {
  local port="$1" pids
  pids="$(lsof -ti tcp:"$port" 2>/dev/null || true)"
  if [ -n "$pids" ]; then
    echo "[start] freeing port $port (killing: $pids)" >&2
    kill $pids 2>/dev/null || true
    sleep 1
    pids="$(lsof -ti tcp:"$port" 2>/dev/null || true)"
    [ -n "$pids" ] && kill -9 $pids 2>/dev/null || true
  fi
}

case "${1:-app}" in
  app)
    # Stop, then restart both servers. Nothing else.
    free_port "$API_PORT"
    free_port "$CLIENT_PORT"
    echo "[start] starting API (:$API_PORT) + React client (:$CLIENT_PORT)…" >&2
    echo "[start] open http://localhost:$CLIENT_PORT" >&2
    exec env PORT="$API_PORT" CLIENT_PORT="$CLIENT_PORT" \
      npm --prefix "$APP_DIR" run dev
    ;;
  stop)
    free_port "$API_PORT"
    free_port "$CLIENT_PORT"
    ;;
  *)
    echo "Usage: ./start.sh [app|stop]" >&2
    echo "  app   stop, then restart backend (:6347) + frontend (:6348)" >&2
    echo "  stop  kill whatever is on :6347 and :6348" >&2
    exit 1
    ;;
esac
