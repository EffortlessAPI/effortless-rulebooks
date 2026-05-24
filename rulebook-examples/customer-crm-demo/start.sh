#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_customer_crm_demo}"

cmd="${1:-all}"

SERVER_PORT="${SERVER_PORT:-3032}"
WEB_PORT="${WEB_PORT:-5175}"

# Free any lingering server/web processes from a previous run so reruns of
# ./start.sh don't fail with EADDRINUSE.
stop() {
  for port in "$SERVER_PORT" "$WEB_PORT"; do
    pids=$(lsof -ti tcp:"$port" 2>/dev/null || true)
    if [ -n "$pids" ]; then
      echo "[stop] killing pid(s) on port $port: $pids"
      kill $pids 2>/dev/null || true
      sleep 0.3
      pids=$(lsof -ti tcp:"$port" 2>/dev/null || true)
      [ -n "$pids" ] && kill -9 $pids 2>/dev/null || true
    fi
  done
}

build() {
  echo "[build] effortless build"
  effortless build
}

db() {
  echo "[db] re-initializing $DATABASE_URL"
  psql -U postgres-bootstrap -h localhost -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname='erb_customer_crm_demo'" >/dev/null || true
  psql -U postgres-bootstrap -h localhost -c "DROP DATABASE IF EXISTS erb_customer_crm_demo"
  psql -U postgres-bootstrap -h localhost -c "CREATE DATABASE erb_customer_crm_demo"
  chmod +x postgres-bootstrap/init-db.sh
  bash postgres-bootstrap/init-db.sh
}

server() {
  cd server
  [ -d node_modules ] || npm install
  npm run dev
}

web() {
  cd web
  [ -d node_modules ] || npm install
  npm run dev
}

all() {
  stop
  ( server ) &
  spid=$!
  ( web ) &
  wpid=$!
  trap "kill $spid $wpid 2>/dev/null || true; stop" EXIT
  wait
}

refresh_xlsx() {
  node scripts/refresh-rulebook.mjs
}

case "$cmd" in
  build)        build ;;
  db)           db ;;
  server)       stop; server ;;
  web)          stop; web ;;
  all)          all ;;
  stop)         stop ;;
  refresh-xlsx) refresh_xlsx ;;
  *)            echo "usage: $0 {all|server|web|db|build|stop|refresh-xlsx}"; exit 1 ;;
esac
