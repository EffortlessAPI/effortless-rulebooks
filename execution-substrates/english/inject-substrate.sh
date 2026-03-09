#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Parse arguments
SKIP_INJECT=false
SKIP_TRAINING=false
for arg in "$@"; do
    case $arg in
        --skip-inject)
            SKIP_INJECT=true
            shift
            ;;
        --skip-training)
            SKIP_TRAINING=true
            shift
            ;;
    esac
done

# Check if English documents already exist
DOCS_EXIST=false
if [[ -f "$SCRIPT_DIR/glossary.md" ]] && [[ -f "$SCRIPT_DIR/specification.md" ]]; then
    DOCS_EXIST=true
fi

# If running interactively and docs exist, ask about TRAINING (the slow LLM step)
if [[ -t 0 ]] && [[ "$SKIP_INJECT" = false ]] && [[ "$DOCS_EXIST" = true ]]; then
    echo ""
    echo "english: English documents already exist (glossary.md, specification.md)."
    echo "english: TRAINING regenerates these docs using LLM and may take 2+ minutes."
    read -p "english: Regenerate English documents (TRAINING)? [y/N] " train_response

    if [[ ! "$train_response" =~ ^[Yy]$ ]]; then
        echo "english: TRAINING skipped - using existing documents"
        SKIP_TRAINING=true
    fi
fi

# Inject data into the english substrate (unless --skip-inject or training skipped)
# This generates the markdown files (specification.md, etc.) using an LLM
if [ "$SKIP_INJECT" = false ] && [ "$SKIP_TRAINING" = false ]; then
    python3 "$SCRIPT_DIR/inject-into-english.py" --regenerate || {
        INJECT_EXIT_CODE=$?
        if [ "$INJECT_EXIT_CODE" != "2" ]; then
            echo "Error during injection (exit code $INJECT_EXIT_CODE)"
            exit $INJECT_EXIT_CODE
        fi
        # Exit code 2 means markdown files already exist - continue to test
    }
fi

# Run the test for this substrate
# This uses an LLM to "execute" the English specification and compute answers
"$SCRIPT_DIR/take-test.sh"
