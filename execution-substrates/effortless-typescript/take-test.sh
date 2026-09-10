#!/bin/bash
set -e  # Exit immediately on ANY error - FAIL LOUDLY!
set -o pipefail  # Catch errors in pipes

# take-test.sh for the effortless-typescript execution substrate
#
# Compiles and runs the COMMERCIAL rulebook-to-typescript output for the active
# domain, the way effortless-golang compiles and runs rulebook-to-go's.
#
# The generated program is read from $ERB_DOMAIN_DIR/effortless-typescript/ and
# built there: erb_sdk.ts (every formula compiled), erb_runtime.ts (the tool's
# runtime), main.ts (the runner), package.json and tsconfig.json. `tsc` is the
# first real gate. The program computes every field over the whole dataset on
# its own — it calls no database and no other tool.
#
# This script WILL fail loudly if:
#   - the commercial tool's output is missing (run `effortless build`)
#   - the generated TypeScript does not compile
#   - the program exits non-zero

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SUBSTRATE_NAME="$(basename "$SCRIPT_DIR")"
LOG_FILE="$SCRIPT_DIR/.last-run.log"

if [ -z "$ERB_DOMAIN_DIR" ] || [ ! -d "$ERB_DOMAIN_DIR" ]; then
    echo "FATAL: ERB_DOMAIN_DIR is not set or does not exist." >&2
    echo "  ERB_DOMAIN_DIR=${ERB_DOMAIN_DIR:-<unset>}" >&2
    exit 1
fi

TS_OUT_DIR="$ERB_DOMAIN_DIR/effortless-typescript"

{
    echo "=== Effortless TypeScript Substrate Test Run ==="
    echo ""
    echo "effortless-typescript: Starting test..."
    echo "  Program: $TS_OUT_DIR"

    if [[ ! -f "$TS_OUT_DIR/erb_sdk.ts" ]]; then
        echo "FATAL: $TS_OUT_DIR/erb_sdk.ts not found." >&2
        echo "  Register rulebook-to-typescript in this project's effortless.json with" >&2
        echo "  RelativePath /effortless-typescript and run \`effortless build\`." >&2
        exit 1
    fi

    cd "$TS_OUT_DIR"

    echo "effortless-typescript: Installing the compiler..."
    npm install --no-audit --no-fund --silent

    echo "effortless-typescript: Compiling..."
    npx tsc -p .

    echo "effortless-typescript: Running..."
    ERB_SUBSTRATE_NAME="$SUBSTRATE_NAME" node dist/main.js

    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-typescript: test completed successfully"

# Generate substrate report
python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" "$SUBSTRATE_NAME" --elapsed "$SECONDS" --log "$LOG_FILE"
