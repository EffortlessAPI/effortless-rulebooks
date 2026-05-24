#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

DB_URL="${DATABASE_URL:-postgresql://localhost/2026-04-20-1700-naked}"
BACKEND_PORT="${BACKEND_PORT:-3011}"
FRONTEND_PORT="${FRONTEND_PORT:-5173}"

# ── 1. Run DB migrations in order ───────────────────────────────────────────
echo "==> Running database migrations…"
psql "$DB_URL" -f migrations/001_initial.sql
psql "$DB_URL" -f migrations/002_v2.sql
psql "$DB_URL" -f migrations/003_v3.sql
psql "$DB_URL" -f migrations/004_v4.sql
psql "$DB_URL" -f migrations/005_v5.sql
psql "$DB_URL" -f migrations/006_v6.sql
echo "==> Migrations done."

# ── 2. Install backend deps ──────────────────────────────────────────────────
echo "==> Installing backend dependencies…"
cd "$SCRIPT_DIR/backend"
npm install --silent

# ── 3. Install frontend deps ─────────────────────────────────────────────────
echo "==> Installing frontend dependencies…"
cd "$SCRIPT_DIR/frontend"
npm install --silent

# ── 4. Start backend in background ──────────────────────────────────────────
echo "==> Starting backend on port $BACKEND_PORT…"
cd "$SCRIPT_DIR/backend"
DATABASE_URL="$DB_URL" PORT="$BACKEND_PORT" node server.js &
BACKEND_PID=$!
echo "    Backend PID: $BACKEND_PID"

# Wait for backend to be ready
for i in $(seq 1 20); do
  if curl -sf "http://localhost:$BACKEND_PORT/api/summary" > /dev/null 2>&1; then
    echo "    Backend is ready."
    break
  fi
  sleep 0.5
done

# ── 5. Start frontend dev server (foreground) ────────────────────────────────
echo "==> Starting frontend on port $FRONTEND_PORT…"
echo "    App will be available at: http://localhost:$FRONTEND_PORT"
echo ""
echo "    Press Ctrl+C to stop both servers."
echo ""

cd "$SCRIPT_DIR/frontend"

# Trap Ctrl+C to kill backend too
trap "echo ''; echo 'Stopping…'; kill $BACKEND_PID 2>/dev/null; exit 0" INT TERM

VITE_BACKEND_PORT="$BACKEND_PORT" npx vite --port "$FRONTEND_PORT" --host

# If vite exits normally, kill backend
kill $BACKEND_PID 2>/dev/null || true
