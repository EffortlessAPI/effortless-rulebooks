#!/usr/bin/env bash
# ./start.sh            the explainer app (foreground; Ctrl-C stops it)
# ./start.sh --editor   the generated rulebook editor, in its own container
set -euo pipefail

PROJECT_NAME="Talisman's South Park Ontology Explainer"
EXPERIENCE_DESCRIPTION="Jessica Talisman's South Park explainer on one rulebook, with interchangeable Postgres, TypeScript and OWL engines and an admin"
START_COMMAND='./start.sh'
PORT=43210
PRIMARY_URL="http://localhost:${PORT}/"
HEALTH_URL="http://localhost:${PORT}/api/health"
# The rulebook editor's ports are pinned here; the app's admin links to the UI port.
EDITOR_API_PORT=43611
EDITOR_UI_PORT=43612
EDITOR_PG_PORT=43613
export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_talismans_southpark_ontology}"

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

free_port() {
  local port="$1" pids
  pids="$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)"
  if [[ -n "$pids" ]]; then
    kill $pids
    sleep 1
    pids="$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)"
    [[ -z "$pids" ]] || kill -9 $pids
  fi
}

if [[ "${1:-}" == "--editor" ]]; then
  command -v docker >/dev/null 2>&1 || fail "Required command 'docker' was not found."
  [[ -f "$PROJECT_ROOT/effortless-rulebook/edit-rulebook.sh" ]] \
    || fail "The rulebook editor is not generated: run 'effortless build' in $PROJECT_ROOT"
  printf '%s\n' "Rulebook editor: http://localhost:${EDITOR_UI_PORT}  (API http://localhost:${EDITOR_API_PORT})"
  cd "$PROJECT_ROOT/effortless-rulebook"
  RULEBOOK_EDITOR_API_PORT="$EDITOR_API_PORT" RULEBOOK_EDITOR_UI_PORT="$EDITOR_UI_PORT" RULEBOOK_EDITOR_PG_PORT="$EDITOR_PG_PORT" \
    exec bash edit-rulebook.sh
fi

for command_name in node npm python3 lsof; do
  command -v "$command_name" >/dev/null 2>&1 || fail "Required command '$command_name' was not found."
done
[[ -f "$PROJECT_ROOT/app/server.js" ]] || fail "The app is missing: $PROJECT_ROOT/app/server.js"
[[ -f "$PROJECT_ROOT/explainer/rulebook-explainer-dag/explainer-dag.js" ]] \
  || fail "The explainer is missing: run 'effortless build' in $PROJECT_ROOT"

free_port "$PORT"
[[ -d "$PROJECT_ROOT/app/node_modules" ]] || (cd "$PROJECT_ROOT/app" && npm install --no-audit --no-fund)

printf '%s\n' \
  "Project: $PROJECT_NAME" \
  "Experience: $EXPERIENCE_DESCRIPTION" \
  "App: $PRIMARY_URL" \
  "Admin: ${PRIMARY_URL}admin.html" \
  "Health: $HEALTH_URL" \
  "Rulebook editor (optional): ./start.sh --editor  ->  http://localhost:${EDITOR_UI_PORT}"
# The server stays in the foreground: it reports each engine's state itself (a
# missing database only switches the Postgres engine off), and a detached process
# is refused by Postgres.app's client check on this kind of machine.
cd "$PROJECT_ROOT/app"
RULEBOOK_EDITOR_UI_PORT="$EDITOR_UI_PORT" PORT="$PORT" exec node server.js
