#!/bin/bash
# =============================================================================
# RUN-WEB-PORTAL.SH — Boot ssotme-proxy + ERB admin portal + open browser
# =============================================================================
# Started by ./start.sh (default). Supervises:
#   1. ssotme-proxy on :4242   (substrate transpiler HTTP server)
#   2. admin-portal backend on :7777  (Express + static frontend)
# Auto-creates the editor Postgres database on first boot via the backend.
#
# Flags:
#   --no-open    Don't auto-open browser
#   --port=N     Override portal port (default 7777)
# =============================================================================

set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PLATFORM_DIR="$SCRIPT_DIR/effortless-platform"
PORTAL_DIR="$PLATFORM_DIR/admin-portal"
PROXY_DIR="$PLATFORM_DIR/ssotme-proxy"

PORTAL_PORT=7777
PROXY_PORT=4242
OPEN_BROWSER=true

for arg in "$@"; do
  case $arg in
    --no-open) OPEN_BROWSER=false ;;
    --port=*)  PORTAL_PORT="${arg#--port=}" ;;
  esac
done

cyan() { printf "\033[36m%s\033[0m\n" "$*"; }
yellow() { printf "\033[33m%s\033[0m\n" "$*"; }
red() { printf "\033[31m%s\033[0m\n" "$*"; }

# ----------------------------------------------------------------------------
# Prereq checks
# ----------------------------------------------------------------------------
if ! command -v node >/dev/null 2>&1; then
  red "node not found. Install Node 18+ to run the admin portal."
  exit 1
fi
if ! command -v psql >/dev/null 2>&1; then
  yellow "psql not found — portal will boot but editor DB features will fail."
fi

# ----------------------------------------------------------------------------
# Install backend deps if missing
# ----------------------------------------------------------------------------
if [ ! -d "$PORTAL_DIR/node_modules" ]; then
  cyan "[portal] installing backend dependencies (one-time)…"
  (cd "$PORTAL_DIR" && npm install --no-audit --no-fund --loglevel=error)
fi

# ----------------------------------------------------------------------------
# Kill anything already on the ports (clean restart)
# ----------------------------------------------------------------------------
for port in $PROXY_PORT $PORTAL_PORT; do
  existing=$(lsof -ti :$port 2>/dev/null || true)
  if [ -n "$existing" ]; then
    yellow "[portal] killing existing process on :$port (pid=$existing)"
    kill $existing 2>/dev/null || true
    sleep 0.5
  fi
done

# ----------------------------------------------------------------------------
# Start ssotme-proxy (background)
# ----------------------------------------------------------------------------
cyan "[portal] starting ssotme-proxy on :$PROXY_PORT"
python3 "$PROXY_DIR/server.py" >"$PORTAL_DIR/.proxy.log" 2>&1 &
PROXY_PID=$!

# ----------------------------------------------------------------------------
# Start backend (foreground — main process)
# ----------------------------------------------------------------------------
trap 'echo; cyan "[portal] shutting down…"; kill $PROXY_PID 2>/dev/null || true; exit 0' INT TERM

if $OPEN_BROWSER; then
  (sleep 1.5 && (command -v open >/dev/null && open "http://localhost:$PORTAL_PORT" \
                  || command -v xdg-open >/dev/null && xdg-open "http://localhost:$PORTAL_PORT" \
                  || true)) &
fi

cyan "[portal] starting admin backend on :$PORTAL_PORT"
cyan "[portal] →  http://localhost:$PORTAL_PORT"
cyan "[portal] (proxy log: $PORTAL_DIR/.proxy.log)"
export PORTAL_PORT
export PROXY_URL="http://localhost:$PROXY_PORT"
exec node "$PORTAL_DIR/server.js"
