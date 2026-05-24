#!/bin/bash

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

DEFAULT_DB="erb_community_event_planner_demo"
DEFAULT_SERVER_PORT="3045"
DEFAULT_WEB_PORT="5188"

# Exported for child processes
export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/$DEFAULT_DB}"
export SERVER_PORT="${SERVER_PORT:-$DEFAULT_SERVER_PORT}"
export WEB_PORT="${WEB_PORT:-$DEFAULT_WEB_PORT}"

show_help() {
  cat <<EOF
Community Event Planner — Interactive launcher

Usage: ./start.sh [all|server|web|db|build]

Commands:
  all     — Start server & web in parallel (recommended first time)
  server  — Start Express server only (port $DEFAULT_SERVER_PORT)
  web     — Start Vite dev server only (port $DEFAULT_WEB_PORT)
  db      — Drop, create, and initialize database
  build   — Run effortless build (regenerates postgres-bootstrap/)
  help    — Show this message

Environment variables:
  DATABASE_URL   (default: $DATABASE_URL)
  SERVER_PORT    (default: $DEFAULT_SERVER_PORT)
  WEB_PORT       (default: $DEFAULT_WEB_PORT)

Examples:
  ./start.sh all          # First time setup (assumes db exists)
  ./start.sh db           # Reinitialize database
  ./start.sh build        # Rebuild postgres-bootstrap/ from rulebook
  ./start.sh server       # Server only
  ./start.sh web          # Web app only

EOF
}

run_build() {
  echo -e "${BLUE}Building postgres-bootstrap schema from rulebook...${NC}"
  npx effortless build
}

run_db() {
  echo -e "${BLUE}Setting up database: $DEFAULT_DB${NC}"

  # Drop if exists
  psql -U postgres-bootstrap -h localhost -c "DROP DATABASE IF EXISTS $DEFAULT_DB;" || true
  psql -U postgres-bootstrap -h localhost -c "CREATE DATABASE $DEFAULT_DB;"

  # Run init script
  if [ -f postgres-bootstrap/init-db.sh ]; then
    chmod +x postgres-bootstrap/init-db.sh
    DATABASE_URL="$DATABASE_URL" ./postgres-bootstrap/init-db.sh
    echo -e "${GREEN}✓ Database initialized${NC}"
  else
    echo "postgres-bootstrap/init-db.sh not found. Run './start.sh build' first."
    exit 1
  fi
}

run_server() {
  echo -e "${BLUE}Starting Express server on port $SERVER_PORT...${NC}"
  # Kill any existing process on this port
  lsof -ti:$SERVER_PORT | xargs kill -9 2>/dev/null || true
  sleep 1
  cd server
  npm install 2>/dev/null || true
  DATABASE_URL="$DATABASE_URL" npm run dev
}

run_web() {
  echo -e "${BLUE}Starting Vite dev server on port $WEB_PORT...${NC}"
  # Kill any existing process on this port
  lsof -ti:$WEB_PORT | xargs kill -9 2>/dev/null || true
  sleep 1
  cd web
  npm install 2>/dev/null || true
  npm run dev -- --port "$WEB_PORT"
}

check_db() {
  psql "$DATABASE_URL" -c "SELECT 1" > /dev/null 2>&1
  return $?
}

check_postgres() {
  psql -U postgres-bootstrap -h localhost -c "SELECT 1" > /dev/null 2>&1
  return $?
}

run_all() {
  echo -e "${GREEN}Starting Community Event Planner${NC}"
  echo ""

  # Check if postgres-bootstrap is running
  if ! check_postgres; then
    echo -e "${RED}ERROR: Postgres is not running on localhost:5432${NC}"
    echo "Start Postgres with: brew services start postgresql (on macOS)"
    exit 1
  fi

  # Check if database is initialized; if not, initialize it
  if ! check_db; then
    echo -e "${BLUE}Database not found. Initializing...${NC}"
    run_db
    echo ""
  fi

  echo -e "${BLUE}Starting services...${NC}"
  echo -e "${BLUE}  Server: http://localhost:$DEFAULT_SERVER_PORT${NC}"
  echo -e "${BLUE}  Web:    http://localhost:$DEFAULT_WEB_PORT${NC}"
  echo ""

  # Start server with visible output
  (
    cd server
    npm install 2>/dev/null || true
    echo -e "${GREEN}Express server starting...${NC}"
    DATABASE_URL="$DATABASE_URL" npm run dev 2>&1 | sed 's/^/[SERVER] /'
  ) &
  SERVER_PID=$!

  sleep 3

  # Start web with visible output
  (
    cd web
    npm install 2>/dev/null || true
    echo -e "${GREEN}Vite dev server starting...${NC}"
    npm run dev -- --port "$WEB_PORT" 2>&1 | sed 's/^/[WEB] /'
  ) &
  WEB_PID=$!

  trap "kill $SERVER_PID $WEB_PID 2>/dev/null; echo -e '${GREEN}Stopped.${NC}'" EXIT

  wait
}

case "${1:-all}" in
  all)
    run_all
    ;;
  server)
    run_server
    ;;
  web)
    run_web
    ;;
  db)
    run_db
    ;;
  build)
    run_build
    ;;
  help)
    show_help
    ;;
  *)
    echo "Unknown command: $1"
    show_help
    exit 1
    ;;
esac
