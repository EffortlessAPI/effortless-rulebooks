#!/usr/bin/env bash
set -euo pipefail

PROJECT_NAME="Talisman's South Park Ontology Explainer"
EXPERIENCE_DESCRIPTION='Generated explainer and documentation for the South Park rulebook (the app is the next slice)'
START_COMMAND='./start.sh'
PORT=43210
PRIMARY_URL="http://localhost:${PORT}/explainer/rulebook-explainer-dag/pages/index.html"
HEALTH_URL="$PRIMARY_URL"

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ARTIFACT_PATH="$PROJECT_ROOT/explainer/rulebook-explainer-dag/pages/index.html"
LOG_DIR="$PROJECT_ROOT/.run"
LOG_FILE="$LOG_DIR/start.log"

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

for command_name in python3 lsof curl; do
  command -v "$command_name" >/dev/null 2>&1 || fail "Required command '$command_name' was not found."
done
[[ -f "$ARTIFACT_PATH" ]] || fail "The explainer is missing: $ARTIFACT_PATH (run 'effortless build' in $PROJECT_ROOT)"
[[ -f "$PROJECT_ROOT/rulespeak/rulespeak.html" ]] || fail "The documentation is missing: $PROJECT_ROOT/rulespeak/rulespeak.html (generated, not committed: run 'effortless build' in $PROJECT_ROOT)"

existing_pids="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null || true)"
if [[ -n "$existing_pids" ]]; then
  kill $existing_pids
  sleep 1
  remaining_pids="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null || true)"
  [[ -z "$remaining_pids" ]] || kill -9 $remaining_pids
fi

mkdir -p "$LOG_DIR"
nohup python3 -m http.server "$PORT" --bind 127.0.0.1 --directory "$PROJECT_ROOT" >"$LOG_FILE" 2>&1 &
server_pid=$!

for _ in $(seq 1 30); do
  curl -fsS "$HEALTH_URL" >/dev/null 2>&1 && break
  sleep 0.2
done
if ! curl -fsS "$HEALTH_URL" >/dev/null 2>&1; then
  kill "$server_pid" 2>/dev/null || true
  fail "Documentation server failed to start. See $LOG_FILE"
fi

printf '%s\n' \
  "Project: $PROJECT_NAME" \
  "Experience: $EXPERIENCE_DESCRIPTION" \
  "Explainer: $PRIMARY_URL" \
  "Documentation: http://localhost:${PORT}/rulespeak/rulespeak.html" \
  "Health: $HEALTH_URL"
