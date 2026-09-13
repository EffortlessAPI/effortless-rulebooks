#!/usr/bin/env bash
# Serve this folder and open the TypeScript SDK example in the browser. Ctrl+C to stop.
set -euo pipefail
cd "$(dirname "$0")"

python3 -m http.server 8765 --bind 127.0.0.1 &
sleep 1
open http://127.0.0.1:8765/example-usage.html
wait
