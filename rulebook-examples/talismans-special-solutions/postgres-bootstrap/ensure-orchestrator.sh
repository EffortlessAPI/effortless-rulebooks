#!/bin/bash
# =============================================================================
# ENSURE-ORCHESTRATOR.SH
# =============================================================================
# Guarantees the effortless CLI's local transpiler host is running on
# 127.0.0.1:4242, so the repo-local http://127.0.0.1:4242/* tools this project
# depends on (oss-rulebook-to-owl, oss-postgres-calculated-to-rulebook, etc.)
# can be reached during `effortless build`.
#
# The host is `effortless serve`, which serves every tool under the repo root's
# effortless-tools/ directory. It replaced the hand-rolled ssotme-proxy: the CLI
# hands each tool its input directly, so nothing has to infer the calling
# project by inspecting the build process any more.
#
# IDEMPOTENT: if the host already answers GET / it does nothing. Only when the
# port is silent does it boot one in the background. It does NOT restart a
# healthy host, so a routine build stays cheap when the bus is already up.
# =============================================================================
set -e

PORT=4242
# This script lives in <repo>/rulebook-examples/talismans-special-solutions/postgres-bootstrap,
# so the repo root is three levels up.
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"

# The host binds the literal HttpListener prefix http://127.0.0.1:<port>/, so a
# "localhost" Host header 404s. Always address it as 127.0.0.1.
ping_host() {
  python3 - "$PORT" <<'PY' 2>/dev/null
import sys, urllib.request
port = sys.argv[1]
try:
    urllib.request.urlopen(f"http://127.0.0.1:{port}/", timeout=2)
    sys.exit(0)
except Exception:
    sys.exit(1)
PY
}

if ping_host; then
  echo "[ensure-orchestrator] local transpiler host already live on :$PORT"
  exit 0
fi

echo "[ensure-orchestrator] local transpiler host not responding on :$PORT — booting it"
if [ ! -d "$REPO_ROOT/effortless-tools" ]; then
  echo "[ensure-orchestrator] ERROR: $REPO_ROOT/effortless-tools not found" >&2
  exit 1
fi

# Boot detached so the build can proceed; log at the repo root.
(cd "$REPO_ROOT" && nohup effortless serve -port "$PORT" >"$REPO_ROOT/.effortless/serve.log" 2>&1 &)

# Wait (up to ~10s) for it to come online; fail loudly if it never does.
for _ in $(seq 1 20); do
  if ping_host; then
    echo "[ensure-orchestrator] local transpiler host is up on :$PORT"
    exit 0
  fi
  sleep 0.5
done

echo "[ensure-orchestrator] ERROR: local transpiler host failed to come online on :$PORT (see $REPO_ROOT/.effortless/serve.log)" >&2
exit 1
