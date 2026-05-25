#!/usr/bin/env bash
# Stops any running instances of this app and starts two fresh ones on
# fixed (chosen-once) ports so VS Code can Ctrl+click the printed URLs.
set -euo pipefail

cd "$(dirname "$0")"

PORT_A=47821
PORT_B=47822

# Use Node 20 if it's available via nvm — package requires >=18.
if [ -x "$HOME/.nvm/versions/node/v20.20.2/bin/node" ]; then
  export PATH="$HOME/.nvm/versions/node/v20.20.2/bin:$PATH"
fi

stop_port() {
  local port="$1"
  local pids
  pids="$(lsof -ti tcp:"$port" || true)"
  if [ -n "$pids" ]; then
    echo "Stopping process(es) on port $port: $pids"
    kill $pids 2>/dev/null || true
    sleep 0.3
    pids="$(lsof -ti tcp:"$port" || true)"
    [ -n "$pids" ] && kill -9 $pids 2>/dev/null || true
  fi
}

stop_port "$PORT_A"
stop_port "$PORT_B"
pkill -f "node src/server.js" 2>/dev/null || true

mkdir -p .run
PORT="$PORT_A" nohup node src/server.js >.run/server-a.log 2>&1 &
echo $! >.run/server-a.pid
PORT="$PORT_B" nohup node src/server.js >.run/server-b.log 2>&1 &
echo $! >.run/server-b.pid

sleep 0.5

echo
echo "naked-claude-demo running:"
echo "  http://localhost:${PORT_A}/"
echo "  http://localhost:${PORT_B}/"
echo
echo "Logs: .run/server-a.log  .run/server-b.log"
