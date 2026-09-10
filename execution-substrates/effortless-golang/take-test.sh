#!/bin/bash
set -e  # Exit immediately on ANY error - FAIL LOUDLY!
set -o pipefail  # Catch errors in pipes

# take-test.sh for the effortless-golang execution substrate
#
# Compiles and runs the COMMERCIAL rulebook-to-go output for the active domain.
# The licensed counterpart to the open-source `golang` substrate, mirroring how
# effortless-python relates to python and effortless-postgres to postgres.
#
# The generated module is read from $ERB_DOMAIN_DIR/effortless-golang/ and built
# there — `go build` is the first real gate. The open-source transpiler emits a
# module that does not compile on this domain at all; that difference IS the
# product difference, so a compile failure here is a genuine substrate result
# and must not be swallowed.
#
# This script WILL fail loudly if:
#   - the commercial tool's output is missing (run `effortless build`)
#   - the generated Go does not compile
#   - the module exits non-zero on any entity

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SUBSTRATE_NAME="$(basename "$SCRIPT_DIR")"
LOG_FILE="$SCRIPT_DIR/.last-run.log"

if [ -z "$ERB_DOMAIN_DIR" ] || [ ! -d "$ERB_DOMAIN_DIR" ]; then
    echo "FATAL: ERB_DOMAIN_DIR is not set or does not exist." >&2
    echo "  ERB_DOMAIN_DIR=${ERB_DOMAIN_DIR:-<unset>}" >&2
    exit 1
fi

GO_OUT_DIR="$ERB_DOMAIN_DIR/effortless-golang"

{
    echo "=== Effortless Golang Substrate Test Run ==="
    echo ""
    echo "effortless-golang: Starting test..."
    echo "  Module: $GO_OUT_DIR"

    if [[ ! -f "$GO_OUT_DIR/erb_sdk.go" ]]; then
        echo "FATAL: $GO_OUT_DIR/erb_sdk.go not found." >&2
        echo "  Register rulebook-to-go in this project's effortless.json with" >&2
        echo "  RelativePath /effortless-golang and run \`effortless build\`." >&2
        exit 1
    fi

    cd "$GO_OUT_DIR"

    echo "effortless-golang: Compiling..."
    go build ./...

    echo "effortless-golang: Running..."
    ERB_SUBSTRATE_NAME="$SUBSTRATE_NAME" go run .

    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-golang: test completed successfully"

# Generate substrate report
python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" "$SUBSTRATE_NAME" --elapsed "$SECONDS" --log "$LOG_FILE"
