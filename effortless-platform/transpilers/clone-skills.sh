#!/usr/bin/env bash
# Launcher — delegates to the real script in docs/skills/
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
exec "$SCRIPT_DIR/../../docs/skills/clone-skills.sh" "$@"
