#!/usr/bin/env bash
set -euo pipefail

PROJECT_NAME="Talisman's South Park Ontology Explainer"
EXPERIENCE_DESCRIPTION="Jessica Talisman's South Park explainer, every conclusion read from the rulebook's Postgres views"
START_COMMAND='./start.sh'
PORT=43210
PRIMARY_URL="http://localhost:${PORT}/"
HEALTH_URL="http://localhost:${PORT}/api/health"
export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_talismans_southpark_ontology}"

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LOG_DIR="$PROJECT_ROOT/.run"
LOG_FILE="$LOG_DIR/start.log"

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

for command_name in node npm psql lsof curl; do
  command -v "$command_name" >/dev/null 2>&1 || fail "Required command '$command_name' was not found."
done
[[ -f "$PROJECT_ROOT/app/server.js" ]] || fail "The app is missing: $PROJECT_ROOT/app/server.js"
[[ -f "$PROJECT_ROOT/explainer/rulebook-explainer-dag/explainer-dag.js" ]] \
  || fail "The explainer is missing: run 'effortless build' in $PROJECT_ROOT"
view_ready="$(psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -tAc "SELECT 1 FROM information_schema.views WHERE table_name = 'vw_statements'" 2>/dev/null || true)"
[[ "$view_ready" == "1" ]] \
  || fail "The database at $DATABASE_URL has no vw_statements view. Load it with: python3 ../../scripts/erb_project.py talismans-southpark-ontology --reset-db"

existing_pids="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null || true)"
if [[ -n "$existing_pids" ]]; then
  kill $existing_pids
  sleep 1
  remaining_pids="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null || true)"
  [[ -z "$remaining_pids" ]] || kill -9 $remaining_pids
fi

[[ -d "$PROJECT_ROOT/app/node_modules" ]] || (cd "$PROJECT_ROOT/app" && npm install --no-audit --no-fund)

mkdir -p "$LOG_DIR"
(cd "$PROJECT_ROOT/app" && nohup node server.js >"$LOG_FILE" 2>&1 &)

for _ in $(seq 1 50); do
  curl -fsS "$HEALTH_URL" >/dev/null 2>&1 && break
  sleep 0.2
done
curl -fsS "$HEALTH_URL" >/dev/null 2>&1 || fail "The app failed to start. See $LOG_FILE"

printf '%s\n' \
  "Project: $PROJECT_NAME" \
  "Experience: $EXPERIENCE_DESCRIPTION" \
  "App: $PRIMARY_URL" \
  "Health: $HEALTH_URL" \
  "Log: $LOG_FILE" \
  "Stop it with: kill \$(lsof -tiTCP:${PORT} -sTCP:LISTEN)"
