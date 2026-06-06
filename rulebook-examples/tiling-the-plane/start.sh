#!/usr/bin/env bash
# ============================================================================
# Tiling the Plane — control-panel launcher
# ============================================================================
# Subcommands:
#   ./start.sh all      (default) boot the API + web app together
#   ./start.sh server   API only
#   ./start.sh web      web app only
#   ./start.sh stop     kill anything on the ports (clean shutdown)
#   ./start.sh restart  stop then start all
#   ./start.sh db       re-init the Postgres DB from the generated SQL
#   ./start.sh build    effortless build (regenerate SQL) then re-init the DB
#
# Always kills whatever is on its ports first — restart is one command.
# ============================================================================
set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")"

SERVER_PORT="${SERVER_PORT:-3032}"
WEB_PORT="${WEB_PORT:-5175}"
export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_tiling_the_plane}"
export PROJECT_NAME="${PROJECT_NAME:-tiling-the-plane}"
export SERVER_PORT WEB_PORT
# Ensure the effortless CLI is on PATH for the Excel-export endpoint (it shells out
# to `effortless rulebook-to-xlsx`). Prepend the dir of whatever `effortless` resolves to.
if command -v effortless >/dev/null 2>&1; then
  export PATH="$(dirname "$(command -v effortless)"):$PATH"
fi

kill_port() {
  local p="$1"
  local pids
  pids="$(lsof -ti:"$p" 2>/dev/null || true)"
  [ -n "$pids" ] && { echo "[start] freeing port $p"; kill $pids 2>/dev/null || true; sleep 0.4; }
  return 0
}

ensure_deps() {
  [ -d "$1/node_modules" ] || { echo "[start] installing deps in $1"; (cd "$1" && npm install --silent); }
}

cmd_db() {
  echo "[start] re-initializing $DATABASE_URL"
  ( cd postgres-bootstrap && chmod +x init-db.sh && ./init-db.sh )
}

cmd_build() {
  echo "[start] effortless build (regenerate SQL + re-init DB)"
  effortless build
}

cmd_server() {
  kill_port "$SERVER_PORT"
  ensure_deps server
  echo "[start] API → http://localhost:$SERVER_PORT"
  ( cd server && PORT="$SERVER_PORT" npm run dev )
}

cmd_web() {
  kill_port "$WEB_PORT"
  ensure_deps web
  echo "[start] web → http://localhost:$WEB_PORT"
  ( cd web && npm run dev )
}

cmd_stop() {
  kill_port "$SERVER_PORT"
  kill_port "$WEB_PORT"
  echo "[start] stopped"
}

cmd_all() {
  kill_port "$SERVER_PORT"
  kill_port "$WEB_PORT"
  ensure_deps server
  ensure_deps web
  echo "[start] API → http://localhost:$SERVER_PORT   web → http://localhost:$WEB_PORT"
  ( cd server && PORT="$SERVER_PORT" npm run dev ) &
  SERVER_PID=$!
  trap 'kill $SERVER_PID 2>/dev/null || true' EXIT INT TERM
  ( cd web && npm run dev )
}

case "${1:-all}" in
  all) cmd_all ;;
  server) cmd_server ;;
  web) cmd_web ;;
  stop) cmd_stop ;;
  restart) cmd_stop; cmd_all ;;
  db) cmd_db ;;
  build) cmd_build ;;
  *) echo "usage: ./start.sh [all|server|web|stop|restart|db|build]"; exit 1 ;;
esac
