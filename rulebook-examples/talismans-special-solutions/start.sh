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

# This script lives at the talismans-special-solutions project root; the web app lives in app/.
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$HERE"
BACKEND="$HERE/app/backend"
FRONTEND="$HERE/app/frontend"
MODE="${1:-dev}"
API_PORT="${PORT:-8088}"
WEB_PORT="${WEB_PORT:-5173}"

# --- preflight: the reasoner needs the generated ontology -------------------
# These artifacts are COMMITTED, so a fresh clone already has them and this block
# never fires. It only triggers if someone deleted the generated owl/src/*.ttl —
# in which case the fix is a rebuild, and we make that recovery one copy/paste,
# not a dead end.
for f in ontology.owl rules.shacl.ttl; do
  if [ ! -f "$PROJECT_ROOT/owl/src/$f" ]; then
    echo "ERROR: missing $PROJECT_ROOT/owl/src/$f" >&2
    echo "These files are normally committed; a clean clone has them. To regenerate:" >&2
    echo "" >&2
    if command -v effortless >/dev/null 2>&1; then
      echo "    cd \"$PROJECT_ROOT\" && effortless build" >&2
    else
      echo "  The 'effortless' CLI is not installed. Install it, then rebuild:" >&2
      echo "    npm install -g ssotme        # ships 'effortless'/'ssotme'/'aic' bins" >&2
      echo "    cd \"$PROJECT_ROOT\" && effortless build" >&2
      echo "" >&2
      echo "  (Or just restore the committed artifacts — no CLI needed:" >&2
      echo "    git checkout -- owl/src/ )" >&2
    fi
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
# Always run npm install (idempotent when satisfied) so newly-added TypeScript /
# router deps are present after a conversion — not just on first boot.
(cd "$BACKEND"  && npm install)
(cd "$FRONTEND" && npm install)

if [ "$MODE" = "prod" ]; then
  kill_port "$API_PORT"
  echo "Building frontend…"
  (cd "$FRONTEND" && npm run build)
  echo "Serving API + UI on http://localhost:$API_PORT"
  cd "$BACKEND" && PORT="$API_PORT" exec npx tsx server.ts
else
  kill_port "$API_PORT"
  kill_port "$WEB_PORT"
  echo "Starting Express API on :$API_PORT and Vite dev server on :$WEB_PORT"
  ( cd "$BACKEND" && PORT="$API_PORT" npx tsx server.ts ) &
  API_PID=$!
  trap 'kill $API_PID 2>/dev/null || true' EXIT INT TERM
  cd "$FRONTEND" && BACKEND_URL="http://localhost:$API_PORT" exec npx vite --port "$WEB_PORT"
fi
