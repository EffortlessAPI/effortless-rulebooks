#!/usr/bin/env bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Kill anything on the app ports
for PORT in 3001 5173; do
  pid=$(lsof -ti tcp:"$PORT" 2>/dev/null || true)
  [ -n "$pid" ] && kill -9 $pid 2>/dev/null && echo "[start] killed pid $pid on :$PORT" || true
done

# Backend
cd "$SCRIPT_DIR/app/backend"
echo "[start] installing backend deps…"
npm install --silent
echo "[start] booting backend on :3001…"
npx tsx server.ts &
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
