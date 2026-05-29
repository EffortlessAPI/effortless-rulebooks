#!/usr/bin/env bash
# Start the First Valley Bank portal: Express backend + Vite frontend.
# Installs deps on first run, then runs both in parallel. Ctrl-C stops both.
# Always frees its ports first, so this is a clean stop-then-restart.

set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/webapp"

BACKEND_PORT=8375
FRONTEND_PORT=8376
export PORT="$BACKEND_PORT"
export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_effortless_banking}"

# Kill anything already on the ports (clean restart).
for port in "$BACKEND_PORT" "$FRONTEND_PORT"; do
  pids_on_port=$(lsof -ti "tcp:$port" 2>/dev/null || true)
  if [ -n "$pids_on_port" ]; then
    echo "[start] freeing port $port (killing: $pids_on_port)"
    # shellcheck disable=SC2086
    kill $pids_on_port 2>/dev/null || true
    sleep 1
    pids_on_port=$(lsof -ti "tcp:$port" 2>/dev/null || true)
    [ -n "$pids_on_port" ] && kill -9 $pids_on_port 2>/dev/null || true
  fi
done

[ -d backend/node_modules ]  || ( echo "[start] installing backend deps…"  && cd backend  && npm install --no-audit --no-fund )
[ -d frontend/node_modules ] || ( echo "[start] installing frontend deps…" && cd frontend && npm install --no-audit --no-fund )

pids=()
cleanup() { echo; echo "[start] shutting down…"; for p in "${pids[@]}"; do kill "$p" 2>/dev/null || true; done; wait 2>/dev/null || true; }
trap cleanup INT TERM EXIT

echo "[start] backend  → http://localhost:8375"
( cd backend  && npm start ) & pids+=($!)

echo "[start] frontend → http://localhost:8376"
( cd frontend && npm run dev ) & pids+=($!)

wait
