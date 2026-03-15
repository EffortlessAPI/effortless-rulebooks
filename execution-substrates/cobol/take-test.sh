#!/bin/bash
set -e  # Exit immediately on ANY error - FAIL LOUDLY!
set -o pipefail  # Catch errors in pipes

# take-test.sh for COBOL execution substrate
# Compiles and runs actual COBOL code to compute test answers
#
# IMPORTANT: This script WILL fail loudly if:
#   - GnuCOBOL (cobc) is not installed
#   - COBOL compilation fails
#   - COBOL execution fails
#   - Input files are missing

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
LOG_FILE="$SCRIPT_DIR/.last-run.log"
cd "$SCRIPT_DIR"

# Capture output for the substrate report
{
    echo "=== COBOL Substrate Test Run ==="
    echo ""

    # Verify required source files exist
    if [[ ! -f "erb_main.cbl" ]]; then
        echo "FATAL: erb_main.cbl not found! Run inject-into-cobol.py first." >&2
        exit 1
    fi

    # Ensure test-answers directory exists
    mkdir -p "$SCRIPT_DIR/test-answers"

    echo "cobol: Starting test (using GnuCOBOL)..."
    python3 "$SCRIPT_DIR/take-test.py"
    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "cobol: test completed successfully"

# Generate substrate report
python3 "$PROJECT_ROOT/orchestration/create-substrate-report.py" cobol --log "$LOG_FILE"
