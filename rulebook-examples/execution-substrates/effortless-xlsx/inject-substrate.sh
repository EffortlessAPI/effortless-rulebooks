#!/bin/bash
set -e
set -o pipefail

# inject-substrate.sh for effortless-xlsx execution substrate
#
# This is a LICENSED Effortless substrate: the heavy lifting is done by the
# rulebook-to-xlsx transpiler (configured in effortless.json with
# RelativePath: "/licensed-effortless-tools/xlsx"), which produces a workbook
# whose formulas exactly mirror the rulebook. We simply trust that workbook
# and read computed values out of it.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
XLSX_TOOL_DIR="$PROJECT_ROOT/licensed-effortless-tools/xlsx"

echo "=== Effortless-XLSX Substrate: Regenerating from rulebook ==="

if command -v effortless &> /dev/null; then
    cd "$XLSX_TOOL_DIR"
    effortless -buildLocal
else
    echo "Warning: effortless CLI not installed, skipping regeneration"
    echo "Install effortless or run 'effortless build' from project root"
fi

cd "$SCRIPT_DIR"

"$SCRIPT_DIR/take-test.sh"
