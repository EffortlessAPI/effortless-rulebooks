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

    # EF_TOOL_DIR is propagated to MSBuild via -p so the csproj can resolve
    # LicensedRoot to the active domain's generated DataClasses. Always
    # clear obj/Patched first so the previous domain's patched copies don't
    # leak into this build (a leftover BaseClass file referencing a now-
    # removed entity would fail compilation).
    rm -rf obj/Patched
    # The orchestrator runs this script directly (not via inject-substrate.sh), so
    # the tool dir is derived from the active project when not handed in. Without
    # it the csproj fell back to a global legacy path that no longer exists.
    if [ -z "$EF_TOOL_DIR" ]; then
        : "${ERB_DOMAIN_DIR:?ERB_DOMAIN_DIR must be set to the active project directory}"
        EF_TOOL_DIR="$ERB_DOMAIN_DIR/effortless-entity-framework"
    fi
    if [ ! -f "$EF_TOOL_DIR/DataClasses/SoAEntityBase.cs" ]; then
        echo "FATAL: no rulebook-to-entity-framework output at $EF_TOOL_DIR (run effortless build)" >&2
        exit 1
    fi
    EF_PROP_ARG="-p:EF_TOOL_DIR=$EF_TOOL_DIR"

    echo "Building runner..."
    dotnet build EffortlessEntityFrameworkRunner.csproj --nologo -v quiet $EF_PROP_ARG
    echo ""

    echo "Running runner..."
    dotnet run --project EffortlessEntityFrameworkRunner.csproj --no-build $EF_PROP_ARG
    echo ""
} 2>&1 | tee "$LOG_FILE"

echo "effortless-entity-framework: test completed"

python3 "$PROJECT_ROOT/orchestration/grade-and-record.py" effortless-entity-framework --elapsed "$SECONDS" --log "$LOG_FILE"
