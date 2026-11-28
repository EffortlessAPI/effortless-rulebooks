#!/usr/bin/env bash
# ./start.sh   the generated rulebook editor on this rulebook, in its own container
#              (its database, its API and its screens; Ctrl-C stops following its log,
#              the container keeps running until the next ./start.sh or `docker rm -f`)
set -euo pipefail

PROJECT_NAME="The South Park ontology, in SDLAF"
START_COMMAND='./start.sh'
# The editor's ports are pinned (the generated script picks random ones otherwise).
EDITOR_API_PORT=43631
EDITOR_UI_PORT=43632
EDITOR_PG_PORT=43633
PRIMARY_URL="http://localhost:${EDITOR_UI_PORT}/"

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

command -v docker >/dev/null 2>&1 || fail "Required command 'docker' was not found."
[[ -f "$PROJECT_ROOT/effortless-rulebook/edit-rulebook.sh" ]] \
  || fail "The rulebook editor is not generated: run 'effortless build' in $PROJECT_ROOT"
printf '%s\n' "${PROJECT_NAME}" \
  "Rulebook editor: ${PRIMARY_URL}  (API http://localhost:${EDITOR_API_PORT})" \
  "Explainer (no server needed): $PROJECT_ROOT/explainer/rulebook-explainer-dag/pages/index.html" \
  "Documentation: $PROJECT_ROOT/rulespeak/rulespeak.html"
cd "$PROJECT_ROOT/effortless-rulebook"
RULEBOOK_EDITOR_API_PORT="$EDITOR_API_PORT" RULEBOOK_EDITOR_UI_PORT="$EDITOR_UI_PORT" RULEBOOK_EDITOR_PG_PORT="$EDITOR_PG_PORT" \
  exec bash edit-rulebook.sh
