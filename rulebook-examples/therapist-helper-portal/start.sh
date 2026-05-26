#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_therapist_helper_portal}"
export SERVER_PORT="${SERVER_PORT:-3032}"
export WEB_PORT="${WEB_PORT:-5175}"

cmd="${1:-all}"

build_rulebook() {
  echo "==> effortless build"
  effortless build
  # Patch generic transpiler default to project's DB name
  if [ -f postgres-bootstrap/init-db.sh ]; then
    sed -i.bak -E "s|DEFAULT_CONN=postgresql://postgres@localhost:5432/demo|DEFAULT_CONN=postgresql://postgres@localhost:5432/erb_therapist_helper_portal|" postgres-bootstrap/init-db.sh || true
    sed -i.bak -E "s|^# demo - Database Initialization Script|# erb_therapist_helper_portal - Database Initialization Script|" postgres-bootstrap/init-db.sh || true
    rm -f postgres-bootstrap/init-db.sh.bak
    chmod +x postgres-bootstrap/init-db.sh
  fi
}

reset_db() {
  echo "==> drop+create erb_therapist_helper_portal"
  psql -U postgres-bootstrap -h localhost -d postgres-bootstrap -c "DROP DATABASE IF EXISTS erb_therapist_helper_portal" >/dev/null
  psql -U postgres-bootstrap -h localhost -d postgres-bootstrap -c "CREATE DATABASE erb_therapist_helper_portal" >/dev/null
  (cd postgres-bootstrap && DATABASE_URL="$DATABASE_URL" ./init-db.sh)
}

start_server() {
  (cd server && npm install --silent && PORT="$SERVER_PORT" DATABASE_URL="$DATABASE_URL" npx tsx src/index.ts)
}

start_web() {
  (cd web && npm install --silent && SERVER_PORT="$SERVER_PORT" ./node_modules/.bin/vite --port "$WEB_PORT")
}

case "$cmd" in
  build)  build_rulebook ;;
  db)     reset_db ;;
  server) start_server ;;
  web)    start_web ;;
  all)
    start_server &
    SRV_PID=$!
    trap "kill $SRV_PID 2>/dev/null || true" EXIT
    start_web
    ;;
  *) echo "usage: ./start.sh [all|build|db|server|web]"; exit 1 ;;
esac
