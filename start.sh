#!/bin/bash
# =============================================================================
# START.SH — ERB entry point
# =============================================================================
# Default behaviour: launches the admin portal (web UI) for the active rulebook
# project. Boots ssotme-proxy on :4242, portal backend on :7777, auto-creates
# the editor Postgres DB if missing, opens browser.
#
# Flags:
#   --cli       Launch the legacy CLI orchestrator menu instead.
#   --ci        Pass through to orchestrator (non-interactive mode).
#   --no-open   Don't auto-open the browser (portal mode only).
#   --port=N    Override portal port.
# =============================================================================

set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

MODE="portal"
PASSTHROUGH=()
for arg in "$@"; do
  case $arg in
    --cli)  MODE="cli" ;;
    --ci)   MODE="cli"; PASSTHROUGH+=("--ci") ;;
    *)      PASSTHROUGH+=("$arg") ;;
  esac
done

if [ "$MODE" = "cli" ]; then
  exec bash "$SCRIPT_DIR/orchestration/orchestrate.sh" "${PASSTHROUGH[@]}"
fi

exec bash "$SCRIPT_DIR/run-web-portal.sh" "${PASSTHROUGH[@]}"
