#!/bin/bash
set -e
set -o pipefail

# inject-substrate.sh for PostgreSQL execution substrate
#
# For SSoT.me-based substrates, we run effortless -buildLocal to regenerate
# from the rulebook. This allows postgres to be rebuilt independently
# without rebuilding all other substrates.
#
# Structure:
#   /postgres/                 - SSoT.me transpiler output (SQL files)
#   /execution-substrates/postgres/  - Test runner (this folder)

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
POSTGRES_SSOTME_DIR="$PROJECT_ROOT/postgres"

echo "=== PostgreSQL Substrate: Regenerating from rulebook ==="

# The postgres transpiler is defined in root ssotme.json with RelativePath: "/postgres"
# Running effortless -buildLocal in /postgres will execute just that transpiler
if command -v effortless &> /dev/null; then
    cd "$POSTGRES_SSOTME_DIR"
    effortless -buildLocal
else
    echo "Warning: effortless CLI not installed, skipping regeneration"
    echo "Install effortless or run 'effortless build' from project root"
fi

cd "$SCRIPT_DIR"

# Run the test for this substrate
"$SCRIPT_DIR/take-test.sh"
