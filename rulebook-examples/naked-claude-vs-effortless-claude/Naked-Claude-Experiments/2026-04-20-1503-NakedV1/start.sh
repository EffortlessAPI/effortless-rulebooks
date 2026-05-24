#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# ── Cleanup on exit ──────────────────────────────────────────────────────────
BACKEND_PID=""
FRONTEND_PID=""

cleanup() {
  echo ""
  echo "Shutting down…"
  [ -n "$BACKEND_PID"  ] && kill "$BACKEND_PID"  2>/dev/null || true
  [ -n "$FRONTEND_PID" ] && kill "$FRONTEND_PID" 2>/dev/null || true
  wait 2>/dev/null || true
  echo "Done."
}
trap cleanup EXIT INT TERM

# ── 1. Run DB migration ───────────────────────────────────────────────────────
echo "==> Running DB migration…"
psql -d effortless-demo -f "$SCRIPT_DIR/migrations/001_init.sql" \
  || { echo "Migration failed"; exit 1; }
echo "==> Migration complete."

# ── 2. Install backend deps ───────────────────────────────────────────────────
echo "==> Installing backend dependencies…"
cd "$SCRIPT_DIR/backend"
npm install --silent

# ── 3. Start backend ─────────────────────────────────────────────────────────
echo "==> Starting backend on port 3001…"
node server.js &
BACKEND_PID=$!
echo "    Backend PID: $BACKEND_PID"

# Give it a moment to bind
sleep 1

# ── 4. Install frontend deps ──────────────────────────────────────────────────
echo "==> Installing frontend dependencies…"
cd "$SCRIPT_DIR/frontend"
npm install --silent

# ── 5. Start frontend dev server (foreground, runs until ctrl-c) ──────────────
echo "==> Starting frontend on port 3000…"
echo ""
echo "  App:     http://localhost:3000"
echo "  API:     http://localhost:3001"
echo ""
echo "  Press Ctrl-C to stop."
echo ""

# Run vite in the foreground so the script stays alive
npx vite --port 3000

