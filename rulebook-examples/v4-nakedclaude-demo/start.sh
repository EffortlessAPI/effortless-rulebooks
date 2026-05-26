#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
mkdir -p .run

if [[ -f .run/ports.env ]]; then
  source .run/ports.env
else
  API_PORT=3011
  UI_PORT=$((API_PORT + 1))
  printf 'API_PORT=%s\nUI_PORT=%s\n' "$API_PORT" "$UI_PORT" > .run/ports.env
fi

for p in "$API_PORT" "$UI_PORT"; do
  pids=$(lsof -ti tcp:"$p" 2>/dev/null || true)
  [[ -n "$pids" ]] && kill -9 $pids 2>/dev/null || true
done

if [[ ! -d app/node_modules ]]; then
  ( cd app && npm install --silent )
fi

RUN_DIR="$PWD/.run"
(
  cd app
  PORT="$API_PORT" nohup node server.js > "$RUN_DIR/app.log" 2>&1 &
  echo $! > "$RUN_DIR/app.pid"
  disown
)

for _ in $(seq 1 30); do
  curl -sf "http://localhost:$API_PORT/" >/dev/null && break
  sleep 0.3
done

echo ""
echo "  App ready — Ctrl/Cmd+Click to open:"
echo ""
echo "    http://localhost:$API_PORT/"
echo ""
echo "  (UI port reserved: $UI_PORT)"
