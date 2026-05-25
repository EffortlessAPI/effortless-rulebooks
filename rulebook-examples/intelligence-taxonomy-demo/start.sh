#!/usr/bin/env bash
# Taxonomy of Intelligence — interactive launcher.
# Usage: ./start.sh [all|server|web|db|db-reset|build|stop]

set -euo pipefail

ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
DB_NAME="${DB_NAME:-mark_acosta_demo}"
DB_USER="${DB_USER:-postgres}"
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DATABASE_URL="${DATABASE_URL:-postgresql://${DB_USER}@${DB_HOST}:${DB_PORT}/${DB_NAME}}"
SERVER_PORT="${SERVER_PORT:-3032}"
WEB_PORT="${WEB_PORT:-5175}"

cmd="${1:-}"

prompt_cmd() {
  echo "What would you like to do?"
  echo "  1) all      — build, apply db updates, then start server + web"
  echo "  2) server   — start the Express API (port ${SERVER_PORT})"
  echo "  3) web      — start the Vite dev server (port ${WEB_PORT})"
  echo "  4) db       — apply schema/data updates in place (idempotent; preserves edits)"
  echo "  5) db-reset — TRUNCATE all tables and re-seed from the rulebook (wipes edits)"
  echo "  6) build    — regenerate postgres/ + explainer-dag/ from the rulebook"
  echo "  7) stop     — stop anything listening on :${SERVER_PORT} and :${WEB_PORT}"
  read -rp "Choice [all]: " choice
  case "${choice:-all}" in
    1|all) cmd="all" ;;
    2|server) cmd="server" ;;
    3|web) cmd="web" ;;
    4|db) cmd="db" ;;
    5|db-reset) cmd="db-reset" ;;
    6|build) cmd="build" ;;
    7|stop) cmd="stop" ;;
    *) echo "unknown choice: $choice" >&2; exit 1 ;;
  esac
}

[[ -z "$cmd" ]] && prompt_cmd

build() {
  cd "$ROOT"
  echo "[build] effortless build"
  effortless build
}

port_pids() {
  # All pids with this TCP port open (listeners or established). One per line.
  lsof -nP -iTCP:"$1" -t 2>/dev/null || true
}

kill_port() {
  local port="$1"
  local pids attempt
  for attempt in 1 2 3 4 5 6; do
    pids="$(port_pids "$port")"
    [[ -z "$pids" ]] && return 0
    local sig="-TERM"
    (( attempt >= 4 )) && sig="-KILL"
    echo "[stop] $sig pid(s) on :${port} -> $(echo "$pids" | tr '\n' ' ')"
    # shellcheck disable=SC2086
    kill $sig $pids 2>/dev/null || true
    sleep 0.5
  done
  # Final check; report if still occupied so the caller can see.
  pids="$(port_pids "$port")"
  if [[ -n "$pids" ]]; then
    echo "[stop] WARNING: :${port} still held by $(echo "$pids" | tr '\n' ' ')" >&2
    return 1
  fi
  return 0
}

stop_server() {
  # Kill the tsx watch supervisor first — otherwise tsx respawns the node
  # listener as soon as we kill its child and the port never frees up.
  # Matching by absolute node_modules path scopes this to THIS project.
  pkill -f "${ROOT}/server/node_modules" 2>/dev/null || true
  kill_port "$SERVER_PORT"
}

stop_web() {
  pkill -f "${ROOT}/web/node_modules" 2>/dev/null || true
  kill_port "$WEB_PORT"
}

stop_services() {
  stop_server
  stop_web
}

ensure_db_exists() {
  if ! psql -U "$DB_USER" -h "$DB_HOST" -p "$DB_PORT" -d postgres -tAc \
        "SELECT 1 FROM pg_database WHERE datname='${DB_NAME}'" | grep -q 1 ; then
    echo "[db] database ${DB_NAME} does not exist — creating"
    psql -U "$DB_USER" -h "$DB_HOST" -p "$DB_PORT" -d postgres \
      -c "CREATE DATABASE ${DB_NAME};"
  fi
}

db() {
  cd "$ROOT"
  ensure_db_exists
  echo "[db] applying schema + data in place (idempotent)"
  chmod +x postgres/init-db.sh
  DATABASE_URL="$DATABASE_URL" ./postgres/init-db.sh
}

db_reset() {
  cd "$ROOT"
  ensure_db_exists
  echo "[db-reset] TRUNCATE assessments, intelligences, capabilities CASCADE"
  psql "$DATABASE_URL" -c "TRUNCATE assessments, intelligences, capabilities CASCADE;" || true
  chmod +x postgres/init-db.sh
  echo "[db-reset] re-seeding from rulebook"
  DATABASE_URL="$DATABASE_URL" ./postgres/init-db.sh
}

server() {
  stop_server
  cd "$ROOT/server"
  if [[ ! -d node_modules ]]; then
    echo "[server] npm install"
    npm install
  fi
  echo "[server] starting on :${SERVER_PORT}"
  DATABASE_URL="$DATABASE_URL" PORT="$SERVER_PORT" npm run dev
}

web() {
  stop_web
  cd "$ROOT/web"
  if [[ ! -d node_modules ]]; then
    echo "[web] npm install"
    npm install
  fi
  echo "[web] starting on :${WEB_PORT}"
  VITE_API_PORT="$SERVER_PORT" PORT="$WEB_PORT" npm run dev -- --port "$WEB_PORT" --strictPort
}

all() {
  build
  db
  stop_services
  echo "[all] launching server and web in parallel (Ctrl-C to stop both)"
  (server) &
  PID_SERVER=$!
  trap 'kill $PID_SERVER 2>/dev/null || true; stop_services' EXIT INT TERM
  sleep 1
  web
}

case "$cmd" in
  build)    build ;;
  db)       db ;;
  db-reset) db_reset ;;
  server)   server ;;
  web)      web ;;
  all)      all ;;
  stop)     stop_services ;;
  *) echo "unknown command: $cmd" >&2; exit 1 ;;
esac
