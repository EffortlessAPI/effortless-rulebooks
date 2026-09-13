#!/bin/bash
set -e
set -o pipefail

# take-test.sh for the effortless-owl execution substrate
#
# Runs SHACL-AF reasoning over the COMMERCIAL rulebook-to-owl output for the
# active domain ($ERB_DOMAIN_DIR/effortless-owl/src/). See take-test.py for why
# this is a separate substrate from the open-source `owl`.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SUBSTRATE_NAME="$(basename "$SCRIPT_DIR")"
LOG_FILE="$SCRIPT_DIR/.last-run.log"

if [ -z "$ERB_DOMAIN_DIR" ] || [ ! -d "$ERB_DOMAIN_DIR" ]; then
    echo "FATAL: ERB_DOMAIN_DIR is not set or does not exist." >&2
    echo "  ERB_DOMAIN_DIR=${ERB_DOMAIN_DIR:-<unset>}" >&2
    exit 1
fi

{
    echo "=== Effortless OWL/SHACL Substrate Test Run ==="
    echo ""
    python3 "$SCRIPT_DIR/take-test.py"
    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-owl: test completed"

python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" "$SUBSTRATE_NAME" --elapsed "$SECONDS" --log "$LOG_FILE"
