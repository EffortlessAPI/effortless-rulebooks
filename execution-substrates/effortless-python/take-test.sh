#!/bin/bash
set -e  # Exit immediately on ANY error - FAIL LOUDLY!
set -o pipefail  # Catch errors in pipes

# take-test.sh for the effortless-python execution substrate
#
# Executes the COMMERCIAL rulebook-to-python output for the active domain. The
# licensed counterpart to the open-source `python` substrate, mirroring how
# effortless-postgres relates to postgres.
#
# The generated library and its vendored `orchestration` package are read from
# $ERB_DOMAIN_DIR/effortless-python/ — see take-test.py for why that directory
# must win the import over the repo's own open-source orchestration/.
#
# This script WILL fail loudly if:
#   - the commercial tool's output is missing (run `effortless build`)
#   - the generated library cannot compute an entity
#   - output cannot be written

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
LOG_FILE="$SCRIPT_DIR/.last-run.log"
cd "$SCRIPT_DIR"

{
    echo "=== Effortless Python Substrate Test Run ==="
    echo ""

    echo "effortless-python: Starting test..."

    if [[ ! -f "take-test.py" ]]; then
        echo "FATAL: take-test.py not found!" >&2
        exit 1
    fi

    echo "effortless-python: Executing the generated calculation library..."
    python3 take-test.py

    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-python: test completed successfully"

# Generate substrate report
python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" effortless-python --elapsed "$SECONDS" --log "$LOG_FILE"
