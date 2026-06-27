#!/usr/bin/env bash
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
FILE="$SCRIPT_DIR/simpsons-paradox-explorer.html"
echo "Opening $FILE"
open "$FILE"
