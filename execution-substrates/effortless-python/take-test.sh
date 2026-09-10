#!/bin/bash
set -e  # Exit immediately on ANY error - FAIL LOUDLY!
set -o pipefail  # Catch errors in pipes

# take-test.sh for the effortless-python execution substrate
#
# Runs the COMMERCIAL rulebook-to-python output for the active domain. The
# licensed counterpart to the open-source `python` substrate, mirroring how
# effortless-golang relates to golang and effortless-postgres to postgres.
#
# The generated program is read from $ERB_DOMAIN_DIR/effortless-python/ and run
# there: erb_sdk.py (every formula compiled), erb_runtime.py (the tool's
# runtime) and main.py (the runner). It computes every field over the whole
# dataset on its own — it parses no formula, reads no rulebook, and reads no
# other substrate's answers.
#
# This script WILL fail loudly if:
#   - the commercial tool's output is missing (run `effortless build`)
#   - the generated program exits non-zero

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SUBSTRATE_NAME="$(basename "$SCRIPT_DIR")"
LOG_FILE="$SCRIPT_DIR/.last-run.log"

if [ -z "$ERB_DOMAIN_DIR" ] || [ ! -d "$ERB_DOMAIN_DIR" ]; then
    echo "FATAL: ERB_DOMAIN_DIR is not set or does not exist." >&2
    echo "  ERB_DOMAIN_DIR=${ERB_DOMAIN_DIR:-<unset>}" >&2
    exit 1
fi

PY_OUT_DIR="$ERB_DOMAIN_DIR/effortless-python"

{
    echo "=== Effortless Python Substrate Test Run ==="
    echo ""
    echo "effortless-python: Starting test..."
    echo "  Program: $PY_OUT_DIR"

    if [[ ! -f "$PY_OUT_DIR/erb_sdk.py" ]]; then
        echo "FATAL: $PY_OUT_DIR/erb_sdk.py not found." >&2
        echo "  Register rulebook-to-python in this project's effortless.json with" >&2
        echo "  RelativePath /effortless-python and run \`effortless build\`." >&2
        exit 1
    fi

    cd "$PY_OUT_DIR"

    echo "effortless-python: Running..."
    ERB_SUBSTRATE_NAME="$SUBSTRATE_NAME" PYTHONDONTWRITEBYTECODE=1 python3 main.py

    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-python: test completed successfully"

# Generate substrate report
python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" "$SUBSTRATE_NAME" --elapsed "$SECONDS" --log "$LOG_FILE"
