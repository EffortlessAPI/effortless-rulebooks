#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Regenerate Python's erb_calc.py from rulebook (shared by YAML tests)
echo "=== Regenerating shared erb_calc.py from rulebook ==="
python3 "$SCRIPT_DIR/../python/inject-into-python.py"

# YAML schema is static - no generation needed
# This script validates the schema exists
echo "YAML schema available at: execution-substrates/yaml/schema.yaml"

# Run the test for this substrate
"$SCRIPT_DIR/take-test.sh"
