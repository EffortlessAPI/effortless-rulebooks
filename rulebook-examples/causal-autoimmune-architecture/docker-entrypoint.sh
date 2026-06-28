#!/usr/bin/env bash
# ============================================================================
#  app-entrypoint.sh — boot Postgres, seed it, then run the Node app.
# ============================================================================
#  Runs as root, drops to the `postgres` user for everything (the postgres:16
#  base image ships that user + initdb/pg_ctl). One container, one command:
#
#      1. initialize the Postgres data dir (first run only)
#      2. start Postgres locally, wait until it accepts connections
#      3. create the `causal_autoimmune` database if it doesn't exist
#      4. run postgres/init-db.sh — applies the generated schema + seed data
#      5. exec the Express server (serves the React SPA + /api on $PORT)
#
#  Re-runnable: if a data volume is mounted, steps 1 & (mostly) 4 are no-ops
#  on subsequent boots because init-db.sh's SQL is idempotent.
# ============================================================================
set -euo pipefail

PGDATA="${PGDATA:-/var/lib/postgresql/data}"
PGUSER_LOCAL="postgres"
DB_NAME="causal_autoimmune"
APP_DIR="/app/admin-app"

log() { echo "[entrypoint] $*" >&2; }

# ----------------------------------------------------------------------------
# The postgres data dir is a root-owned mount on first run; hand it to the
# `postgres` user so initdb/pg_ctl can write there.
# ----------------------------------------------------------------------------
mkdir -p "$PGDATA"
chown -R "$PGUSER_LOCAL:$PGUSER_LOCAL" "$PGDATA"
chmod 700 "$PGDATA"

# Helper: run a command as the postgres user.
as_pg() { gosu "$PGUSER_LOCAL" "$@"; }

# ----------------------------------------------------------------------------
# 1. Initialize the cluster if the data dir is empty.
# ----------------------------------------------------------------------------
if [ ! -s "$PGDATA/PG_VERSION" ]; then
  log "initializing fresh Postgres cluster at $PGDATA"
  as_pg initdb --username="$PGUSER_LOCAL" --auth-local=trust --auth-host=trust -D "$PGDATA" >/dev/null
fi

# ----------------------------------------------------------------------------
# 2. Start Postgres (background) and wait until it's ready.
#    Listen only on localhost + the unix socket — the app connects locally.
# ----------------------------------------------------------------------------
log "starting Postgres…"
as_pg pg_ctl -D "$PGDATA" \
  -o "-c listen_addresses='127.0.0.1' -c unix_socket_directories='/var/run/postgresql'" \
  -w -t 60 start

# pg_ctl -w already waits for readiness, but double-check with pg_isready.
until as_pg pg_isready -q -h 127.0.0.1 -U "$PGUSER_LOCAL"; do
  log "waiting for Postgres to accept connections…"
  sleep 1
done
log "Postgres is up."

# ----------------------------------------------------------------------------
# 3. Create the target database if it doesn't already exist.
# ----------------------------------------------------------------------------
if ! as_pg psql -h 127.0.0.1 -U "$PGUSER_LOCAL" -tAc \
      "SELECT 1 FROM pg_database WHERE datname='${DB_NAME}'" | grep -q 1; then
  log "creating database ${DB_NAME}"
  as_pg createdb -h 127.0.0.1 -U "$PGUSER_LOCAL" "$DB_NAME"
else
  log "database ${DB_NAME} already exists — re-applying idempotent seed"
fi

# ----------------------------------------------------------------------------
# 4. Seed: apply the generated schema + data via the project's init-db.sh.
#    SNAPSHOT_RULEBOOK=false (set in the Dockerfile) keeps it from writing
#    back into the read-only rulebook copy.
# ----------------------------------------------------------------------------
log "seeding via postgres/init-db.sh"
as_pg env \
  DATABASE_URL="postgresql://${PGUSER_LOCAL}@127.0.0.1:5432/${DB_NAME}" \
  SNAPSHOT_RULEBOOK=false \
  bash /app/postgres/init-db.sh

# ----------------------------------------------------------------------------
# 5. Run the app in the foreground (PID 1-ish so signals/stop work).
#    DATABASE_URL + PORT come from the Dockerfile ENV.
# ----------------------------------------------------------------------------
log "starting Express app on :${PORT:-6348} (UI + API)…"
cd "$APP_DIR"
exec gosu "$PGUSER_LOCAL" env \
  DATABASE_URL="postgresql://${PGUSER_LOCAL}@127.0.0.1:5432/${DB_NAME}" \
  PORT="${PORT:-6348}" \
  node server/index.js
