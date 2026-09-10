#!/bin/bash
set -e
set -o pipefail

# inject-substrate.sh for the effortless-python execution substrate
#
# Regenerates the COMMERCIAL rulebook-to-python output for the active domain,
# then runs the test. Mirrors effortless-postgres: `effortless -buildLocal` from
# inside the transpiler's own output directory rebuilds just this substrate,
# without rebuilding every other one.
#
# Structure:
#   <domain>/effortless-python/                  - commercial transpiler output
#   execution-substrates/effortless-python/      - this test runner

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# ERB_DOMAIN_DIR is required. Guessing a domain would silently grade one
# project's code against another project's answer key and report it as a score.
if [ -z "$ERB_DOMAIN_DIR" ] || [ ! -d "$ERB_DOMAIN_DIR" ]; then
    echo "FATAL: ERB_DOMAIN_DIR is not set or does not exist." >&2
    echo "  ERB_DOMAIN_DIR=${ERB_DOMAIN_DIR:-<unset>}" >&2
    echo "  Invoke this script via the orchestrator, which sets it to the" >&2
    echo "  active domain's directory." >&2
    exit 1
fi

PYTHON_OUT_DIR="$ERB_DOMAIN_DIR/effortless-python"

echo "=== Effortless-Python Substrate: Regenerating from rulebook ==="
echo "  Output dir: $PYTHON_OUT_DIR"

if ! command -v effortless &> /dev/null; then
    echo "FATAL: the effortless CLI is not installed, so the commercial" >&2
    echo "  transpiler cannot run. This substrate grades that tool's output;" >&2
    echo "  without it there is nothing to grade." >&2
    exit 1
fi

# -buildLocal resolves the transpiler from this directory's entry in the
# project's effortless.json, so the directory has to exist before the first run.
mkdir -p "$PYTHON_OUT_DIR"
cd "$PYTHON_OUT_DIR"
effortless -buildLocal

cd "$SCRIPT_DIR"

# Run the test for this substrate
"$SCRIPT_DIR/take-test.sh"
