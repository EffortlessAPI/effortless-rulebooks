#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$PROJECT_ROOT"

PROJECT_NAME='cmcc-core-r0'
EXPERIENCE_DESCRIPTION='CMCC-Core Representation Theorem conformance-matrix explorer'
API_PORT=43306
WEB_PORT=43106
PRIMARY_URL="http://localhost:${WEB_PORT}"
HEALTH_URL="http://localhost:${API_PORT}/api/views"
export PGHOST="${PGHOST:-localhost}"
export PGUSER="${PGUSER:-postgres}"
export PGPORT="${PGPORT:-5432}"
export PGDATABASE="${PGDATABASE:-erb_cmcc_core_r0}"
export PGPASSWORD="${PGPASSWORD:-postgres}"

die() { echo "[start] ERROR: $*" >&2; exit 1; }

for command in python3 npm psql lsof; do
  command -v "$command" >/dev/null 2>&1 || die "$command is required"
done
for file in app/package.json app/server.js app/vite.config.js \
  effortless-rulebook/effortless-rulebook.json conformance/run.py conformance/ingest_conformance.py; do
  [ -f "$file" ] || die "missing required file: $PROJECT_ROOT/$file"
done

# ----------------------------------------------------------------------
# 1. The project's actual "build": rebuild the transparent/tangled
#    substrates from r0/spec.json, regenerate the expected traces (the
#    reference evaluator cross-checked against the independent SQLite
#    oracle), rebuild conformance/matrix.json, then run the 21-test suite
#    against the freshly rebuilt pipeline. This is what run.sh used to do,
#    unchanged.
# ----------------------------------------------------------------------
echo "[start] rebuilding substrates + conformance/matrix.json"
python3 conformance/run.py
echo "[start] running the 21-test pytest suite"
python3 -m pytest tests -q

# ----------------------------------------------------------------------
# 2. Push the freshly computed matrix.json into the Postgres witness
#    tables (answer_key_results, reflection_results). Direction of truth:
#    the Python pipeline computes, the database is a witness store fed
#    from it -- never the other way around. Substrates.AnswerKeyOk /
#    ReflectionOk / Conformant are then a rulebook rollup over these rows.
# ----------------------------------------------------------------------
view_ready="$(psql -d "$PGDATABASE" -v ON_ERROR_STOP=1 -tAc \
  "SELECT 1 FROM information_schema.views WHERE table_schema='public' AND table_name LIKE 'vw\\_%' LIMIT 1" \
  2>/dev/null || true)"
[ "$view_ready" = "1" ] \
  || die "database $PGDATABASE at $PGHOST:$PGPORT is unavailable or has no vw_* views; run ./init-db.sh first"

echo "[start] ingesting conformance/matrix.json into $PGDATABASE"
python3 conformance/ingest_conformance.py

for port in "$API_PORT" "$WEB_PORT"; do
  pids="$(lsof -nP -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)"
  if [ -n "$pids" ]; then
    echo "[start] freeing declared port $port (PIDs: $(echo "$pids" | tr '\n' ' '))"
    # shellcheck disable=SC2086
    kill $pids
    sleep 1
  fi
  pids="$(lsof -nP -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)"
  if [ -n "$pids" ]; then
    # shellcheck disable=SC2086
    kill -KILL $pids
    sleep 1
  fi
  [ -z "$(lsof -nP -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)" ] \
    || die "port $port is still occupied"
done

[ -d app/node_modules ] || (cd app && npm install --no-audit --no-fund)

echo "[start] project: $PROJECT_NAME"
echo "[start] starting: $EXPERIENCE_DESCRIPTION"
echo "[start] primary:  $PRIMARY_URL"
echo "[start] API:      http://localhost:${API_PORT}"
echo "[start] health:   $HEALTH_URL"
exec npm --prefix app run dev
