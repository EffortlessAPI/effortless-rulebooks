#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
cd "$SCRIPT_DIR"

# Copy blank-tests from central testing directory
TESTING_BLANK_TESTS="$PROJECT_ROOT/testing/blank-tests"
LOCAL_BLANK_TESTS="$SCRIPT_DIR/blank-tests"

if [ -d "$TESTING_BLANK_TESTS" ] && [ -n "$(ls -A "$TESTING_BLANK_TESTS" 2>/dev/null)" ]; then
    echo "owl: Copying blank-tests from testing directory..."
    rm -rf "$LOCAL_BLANK_TESTS"
    mkdir -p "$LOCAL_BLANK_TESTS"
    cp "$TESTING_BLANK_TESTS"/*.json "$LOCAL_BLANK_TESTS/" 2>/dev/null || true
fi

# Inject data into the owl substrate
python3 "$SCRIPT_DIR/inject-into-owl.py"

# Run the test for this substrate
"$SCRIPT_DIR/take-test.sh"
