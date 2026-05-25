#!/usr/bin/env bash
# Start the First Valley Bank portal: Express backend + Vite frontend.
# Installs deps on first run, then runs both in parallel. Ctrl-C stops both.

set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")"

export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/first_valley_bank}"

[ -d backend/node_modules ]  || ( echo "[start] installing backend deps…"  && cd backend  && npm install --no-audit --no-fund )
[ -d frontend/node_modules ] || ( echo "[start] installing frontend deps…" && cd frontend && npm install --no-audit --no-fund )

pids=()
cleanup() { echo; echo "[start] shutting down…"; for p in "${pids[@]}"; do kill "$p" 2>/dev/null || true; done; wait 2>/dev/null || true; }
trap cleanup INT TERM EXIT

echo "[start] backend  → http://localhost:4000"
( cd backend  && npm start ) & pids+=($!)

echo "[start] frontend → http://localhost:5173"
( cd frontend && npm run dev ) & pids+=($!)

wait
