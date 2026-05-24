#!/bin/bash
set -e
set -o pipefail

# take-test.sh for effortless-entity-framework execution substrate
#
# Builds the C# runner (which compiles the Effortless-generated EF
# DataClasses straight from licensed-effortless-tools/) and runs it. The
# runner uses reflection to populate every dataclass from blank-tests JSON,
# lets the FormulaXxx properties evaluate, and writes test-answers JSON.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
LOG_FILE="$SCRIPT_DIR/.last-run.log"
cd "$SCRIPT_DIR"

mkdir -p "$SCRIPT_DIR/test-answers"

{
    echo "=== Effortless-EntityFramework Substrate Test Run ==="
    echo ""

    if ! command -v dotnet &> /dev/null; then
        echo "FATAL: dotnet SDK not installed."
        echo "  macOS:  brew install --cask dotnet-sdk"
        echo "  Ubuntu: sudo apt install dotnet-sdk-8.0"
        exit 1
    fi

    echo "Building runner..."
    dotnet build EffortlessEntityFrameworkRunner.csproj --nologo -v quiet
    echo ""

    echo "Running runner..."
    dotnet run --project EffortlessEntityFrameworkRunner.csproj --no-build
    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-entity-framework: test completed"

python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" effortless-entity-framework --elapsed "$SECONDS" --log "$LOG_FILE"
