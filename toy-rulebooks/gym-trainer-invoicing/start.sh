#!/usr/bin/env bash
# Gym Trainer Invoicing — interactive launcher.
# Usage:
#   ./start.sh           # boot server + web (default)
#   ./start.sh all       # same
#   ./start.sh server    # just the server
#   ./start.sh web       # just the web (assumes server is running)
#   ./start.sh db        # drop + re-init the local Postgres DB
#   ./start.sh build     # effortless build (rebuild SQL + reset DB)

set -e
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$HERE"

DB=erb_gym_trainer_invoicing
PG="postgresql://postgres@localhost:5432/${DB}"
SERVER_PORT=3032
WEB_PORT=5175

cmd_db() {
  echo "[start] drop+create $DB"
  psql -U postgres-bootstrap -h localhost -c "DROP DATABASE IF EXISTS $DB" >/dev/null
  psql -U postgres-bootstrap -h localhost -c "CREATE DATABASE $DB" >/dev/null
  ( cd postgres-bootstrap && DATABASE_URL="$PG" ./init-db.sh "$PG" )
}

cmd_build() {
  echo "[start] effortless build"
  effortless build
}

cmd_server() {
  ( cd server && [ -d node_modules ] || npm install --silent )
  ( cd server && DATABASE_URL="$PG" PORT="$SERVER_PORT" npm run dev )
}

cmd_web() {
  ( cd web && [ -d node_modules ] || npm install --silent )
  ( cd web && npm run dev )
}

cmd_all() {
  ( cd server && [ -d node_modules ] || npm install --silent )
  ( cd web && [ -d node_modules ] || npm install --silent )

  echo "[start] starting server on :$SERVER_PORT"
  ( cd server && DATABASE_URL="$PG" PORT="$SERVER_PORT" npm run dev ) &
  SERVER_PID=$!
  trap 'kill $SERVER_PID 2>/dev/null; exit' INT TERM
  sleep 1

  echo "[start] starting web on :$WEB_PORT"
  echo "[start] open http://localhost:$WEB_PORT"
  ( cd web && npm run dev )
  kill $SERVER_PID 2>/dev/null || true
}

case "${1:-all}" in
  all|"") cmd_all ;;
  server) cmd_server ;;
  web)    cmd_web ;;
  db)     cmd_db ;;
  build)  cmd_build ;;
  *) echo "unknown command: $1"; exit 1 ;;
esac
