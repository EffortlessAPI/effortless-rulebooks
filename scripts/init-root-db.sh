#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
INIT_SCRIPT="$REPO_ROOT/postgres/reset-rulebook-db.sh"
DATABASE_NAME="erb_effortless_rulebooks"
DATABASE_URL="postgresql://postgres@localhost:5432/$DATABASE_NAME"

command -v psql >/dev/null 2>&1 || {
  echo "ERROR: psql is required to initialize $DATABASE_NAME." >&2
  exit 1
}

[[ -f "$INIT_SCRIPT" ]] || {
  echo "ERROR: Expected generated initializer is missing: $INIT_SCRIPT" >&2
  exit 1
}

# The generated 01-*.sql runs in check-add mode (CREATE TABLE IF NOT EXISTS, no
# DROP, even with drop_all=true), so rows whose primary keys were removed from the
# rulebook would otherwise survive every rebuild and silently pollute rollups.
# The local database is a derived artifact: recreate it from scratch each time.
echo "Recreating root database: $DATABASE_NAME"
psql -v ON_ERROR_STOP=1 -Atqc "DROP DATABASE IF EXISTS $DATABASE_NAME WITH (FORCE);" postgres
psql -v ON_ERROR_STOP=1 -Atqc "CREATE DATABASE $DATABASE_NAME;" postgres

echo "Initializing root database: $DATABASE_URL"
env DATABASE_URL="$DATABASE_URL" bash "$INIT_SCRIPT"

# The generated editor container watches the rulebook, but Docker Desktop does not
# always propagate host file events through the bind mount, so a host-side build
# can leave the editor API serving stale rows. If the root's editor is running on
# its pinned API port, request an explicit rebuild through the container's trigger.
#
# Every docker call here is BOUNDED. When Docker Desktop is unresponsive
# `docker info` blocks forever with no timeout of its own, and because this
# script is the last step of the root build, that hung probe hangs the whole
# `effortless build`, which the CLI eventually kills with exit 255 — a green
# database reported as a failed build. The database is already reset by this
# point; nudging the editor container is a best-effort cache invalidation, so a
# docker that will not answer is reported and stepped over, never waited on.
EDITOR_API_PORT="42441"
DOCKER_PROBE_TIMEOUT_SECONDS=10

# Run a command with a wall-clock bound. Returns 0 on success, 1 on command
# failure, 124 (the same code GNU `timeout` uses) when it had to be killed.
# macOS has no `timeout` binary, hence the explicit poll.
run_bounded() {
  local seconds="$1"; shift
  "$@" >/dev/null 2>&1 &
  local pid=$!
  local waited=0
  while kill -0 "$pid" 2>/dev/null; do
    if (( waited >= seconds )); then
      kill -9 "$pid" 2>/dev/null || true
      wait "$pid" 2>/dev/null || true
      return 124
    fi
    sleep 1
    waited=$(( waited + 1 ))
  done
  wait "$pid"
}

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not installed; skipping the editor-container rebuild nudge."
else
  # `set -e` is on and `$?` is not readable from inside an `if !` test, so the
  # status is captured explicitly here.
  docker_status=0
  run_bounded "$DOCKER_PROBE_TIMEOUT_SECONDS" docker info || docker_status=$?
fi

if [[ "${docker_status-}" != "" && "$docker_status" != "0" ]]; then
  if (( docker_status == 124 )); then
    echo "WARNING: 'docker info' did not answer within ${DOCKER_PROBE_TIMEOUT_SECONDS}s (Docker Desktop unresponsive)."
    echo "         The root database above is fully reset. Skipping the editor-container rebuild nudge;"
    echo "         if the generated editor on :$EDITOR_API_PORT is running it may serve stale rows until"
    echo "         you run: docker exec \$(docker ps -q --filter publish=$EDITOR_API_PORT) touch /tmp/rebuild-trigger"
  else
    echo "docker is installed but its daemon is not available; skipping the editor-container rebuild nudge."
  fi
elif [[ "${docker_status-}" == "0" ]]; then
  editor_container="$(docker ps -q --filter "publish=$EDITOR_API_PORT")"
  if [[ -n "$editor_container" ]]; then
    echo "Requesting rebuild of the editor container publishing :$EDITOR_API_PORT"
    run_bounded "$DOCKER_PROBE_TIMEOUT_SECONDS" docker exec "$editor_container" touch /tmp/rebuild-trigger \
      || echo "WARNING: could not touch /tmp/rebuild-trigger in $editor_container; the editor may serve stale rows."
  fi
fi
