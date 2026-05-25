#!/bin/bash
# ============================================================================
# start.sh — Launch the Job Search RAG admin stack
# ============================================================================
# Starts three services:
#   1. PostgreSQL database init (if needed)
#   2. Express API server   (port 3001)
#   3. React/Vite dev server (port 5173)
# ============================================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# ---------- colors ----------
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

info()  { echo -e "${GREEN}[start]${NC} $*"; }
warn()  { echo -e "${YELLOW}[start]${NC} $*"; }

# ---------- dependency checks ----------
for cmd in node npm psql; do
  if ! command -v "$cmd" &>/dev/null; then
    warn "Required command '$cmd' not found. Please install it first."
    exit 1
  fi
done

# ---------- install node deps if needed ----------
if [ ! -d "$SCRIPT_DIR/admin/node_modules" ]; then
  info "Installing admin server dependencies..."
  (cd "$SCRIPT_DIR/admin" && npm install)
fi

if [ ! -d "$SCRIPT_DIR/admin/client/node_modules" ]; then
  info "Installing admin client dependencies..."
  (cd "$SCRIPT_DIR/admin/client" && npm install)
fi

# ---------- init database ----------
DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/demo}"

info "Initializing database..."
bash "$SCRIPT_DIR/postgres/init-db.sh" "$DATABASE_URL" || {
  warn "Database init failed — is PostgreSQL running?"
  exit 1
}

# ---------- cleanup on exit ----------
cleanup() {
  info "Shutting down..."
  kill $SERVER_PID $CLIENT_PID 2>/dev/null
  wait $SERVER_PID $CLIENT_PID 2>/dev/null
  info "Done."
}
trap cleanup EXIT INT TERM

# ---------- start Express API server ----------
info "Starting Express API server on :3001..."
(cd "$SCRIPT_DIR/admin" && DATABASE_URL="$DATABASE_URL" npm run server) &
SERVER_PID=$!

# ---------- start Vite dev server ----------
info "Starting React dev server on :5173..."
(cd "$SCRIPT_DIR/admin/client" && npm run dev) &
CLIENT_PID=$!

echo ""
info "============================================"
info "  Admin UI:   http://localhost:5173"
info "  API server: http://localhost:3001"
info "============================================"
info "Press Ctrl+C to stop all services."
echo ""

# Wait for either process to exit
wait $SERVER_PID $CLIENT_PID
