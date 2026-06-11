#!/usr/bin/env bash
# ============================================================================
# Talisman's Special Solutions workflow app — one-command run.
#
# Per repo doctrine, start.sh is the restart story: it kills whatever is on its
# ports first, then boots clean. Two modes:
#
#   ./start.sh          dev:  Express API (:8088) + Vite dev server (:5173)
#                             open http://localhost:5173
#   ./start.sh prod     prod: build the frontend, then Express serves API +
#                             static UI together on :8088
#                             open http://localhost:8088
#
# The reasoner (Python: rdflib/owlrl/pyshacl) is invoked by the backend per
# request — there is no separate reasoner process to manage. It reads the
# GENERATED owl/src/*.ttl, so `effortless build` must have run at least once.
# ============================================================================
set -euo pipefail

# This script lives at the talisman-basic project root; the web app lives in app/.
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$HERE"
BACKEND="$HERE/app/backend"
FRONTEND="$HERE/app/frontend"
MODE="${1:-dev}"
API_PORT="${PORT:-8088}"
WEB_PORT="${WEB_PORT:-5173}"

# --- preflight: the reasoner needs the generated ontology -------------------
for f in ontology.owl rules.shacl.ttl; do
  if [ ! -f "$PROJECT_ROOT/owl/src/$f" ]; then
    echo "ERROR: missing $PROJECT_ROOT/owl/src/$f" >&2
    echo "Run 'effortless build' in $PROJECT_ROOT first — the .ttl IS the computation engine." >&2
    exit 1
  fi
done

# --- preflight: python reasoner deps ---------------------------------------
if ! python3 -c "import rdflib, owlrl, pyshacl" 2>/dev/null; then
  echo "Installing reasoner deps (rdflib/owlrl/pyshacl)…" >&2
  python3 -m pip install -q -r "$BACKEND/reasoner/requirements.txt"
fi

# --- kill anything already on the ports (clean restart) --------------------
kill_port() {
  local port="$1"
  local pids
  pids="$(lsof -ti tcp:"$port" 2>/dev/null || true)"
  if [ -n "$pids" ]; then
    echo "Freeing port $port (killing: $pids)" >&2
    kill $pids 2>/dev/null || true
    sleep 1
  fi
}

# --- deps ------------------------------------------------------------------
[ -d "$BACKEND/node_modules" ]  || (cd "$BACKEND"  && npm install)
[ -d "$FRONTEND/node_modules" ] || (cd "$FRONTEND" && npm install)

if [ "$MODE" = "prod" ]; then
  kill_port "$API_PORT"
  echo "Building frontend…"
  (cd "$FRONTEND" && npm run build)
  echo "Serving API + UI on http://localhost:$API_PORT"
  cd "$BACKEND" && PORT="$API_PORT" exec node server.js
else
  kill_port "$API_PORT"
  kill_port "$WEB_PORT"
  echo "Starting Express API on :$API_PORT and Vite dev server on :$WEB_PORT"
  ( cd "$BACKEND" && PORT="$API_PORT" node server.js ) &
  API_PID=$!
  trap 'kill $API_PID 2>/dev/null || true' EXIT INT TERM
  cd "$FRONTEND" && BACKEND_URL="http://localhost:$API_PORT" exec npx vite --port "$WEB_PORT"
fi
