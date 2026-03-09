#!/bin/bash
# =============================================================================
# START.SH - ERB Orchestration Entry Point
# =============================================================================
# Launches the orchestration menu. All functionality (testing, Airtable sync,
# dev-ops) is handled within orchestrate.sh.
# =============================================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Pass through any arguments (like --ci) to orchestrate.sh
exec bash "$SCRIPT_DIR/orchestration/orchestrate.sh" "$@"
