#!/usr/bin/env bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Preflight: fail loudly with the right fix, not with an obscure DB error
# once the servers are already up.
DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/simpsons_paradox}"
if ! psql "$DATABASE_URL" -c '\q' 2>/dev/null; then
  echo "[start] ERROR: cannot connect to $DATABASE_URL" >&2
  echo "[start] Is Postgres running, and has 'effortless build && cd effortless-postgres && ./init-db.sh' been run yet?" >&2
  exit 1
fi
if ! psql "$DATABASE_URL" -tAc "SELECT to_regclass('public.vw_studies')" 2>/dev/null | grep -q vw_studies; then
  echo "[start] ERROR: $DATABASE_URL has no vw_studies view." >&2
  echo "[start] Run 'effortless build' then 'cd effortless-postgres && ./init-db.sh' before starting the app." >&2
  exit 1
fi

# Kill anything on the app ports
for PORT in 3001 5173; do
  pid=$(lsof -ti tcp:"$PORT" 2>/dev/null || true)
  [ -n "$pid" ] && kill -9 $pid 2>/dev/null && echo "[start] killed pid $pid on :$PORT" || true
done

export PROJECT_NAME="simpsons-paradox"
cd "$SCRIPT_DIR/app/backend"
echo "[start] installing backend deps…"
npm install --silent
echo "[start] booting backend on :3001…"
PROJECT_NAME="simpsons-paradox" npx tsx server.ts &
BACKEND_PID=$!

# Frontend
cd "$SCRIPT_DIR/app/frontend"
echo "[start] installing frontend deps…"
npm install --silent
echo "[start] booting Vite on :5173…"
npm run dev &
FRONTEND_PID=$!

echo ""
echo "  Backend  → http://localhost:3001"
echo "  Frontend → http://localhost:5173"
echo ""

# Open browser after a short delay
sleep 2
open "http://localhost:5173" 2>/dev/null || true

# Keep script alive; Ctrl-C kills both
trap "kill $BACKEND_PID $FRONTEND_PID 2>/dev/null; exit 0" INT TERM
wait
