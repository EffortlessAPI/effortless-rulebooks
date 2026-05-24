#!/usr/bin/env bash
# Entry point for Tale of Two Claudes matched-pair experiments.
#
# The interactive flow is handled entirely by tale-orchestrator.mjs:
#   1. New test or continue existing?
#   2. If continuing — pick the timestamp.
#   3. Pick which phases to run (only forward progress).
#   Both flavors always run in parallel — no option to run just one.
#
# Usage:
#   ./orchestrate.sh              # Interactive
#   ./orchestrate.sh --dry-run    # Show plan, don't execute
#   ./orchestrate.sh --no-claude  # Scaffold only, skip Claude CLI

set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
exec node "$SCRIPT_DIR/tale-orchestrator.mjs" "$@"
