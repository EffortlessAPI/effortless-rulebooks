#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# start.sh — Customer Tracker (Naked-Claude build)
#
# What this does:
#   1. Installs server + client dependencies (npm install).
#   2. Runs the DB migration (create table, seed 5 customers if empty).
#   3. Builds the React frontend with Vite.
#   4. Starts the Express server in the FOREGROUND.
#      The server serves the built frontend as static files on port 3000.
#
# To stop:  Ctrl-C (or kill the process).
# ─────────────────────────────────────────────────────────────────────────────

set -e

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
SERVER_DIR="$PROJECT_DIR/server"
CLIENT_DIR="$PROJECT_DIR/client"

echo ""
echo "══════════════════════════════════════════════"
echo "  Customer Tracker — startup"
echo "══════════════════════════════════════════════"

# ── 1. Install server dependencies ──────────────────────────────────────────
echo ""
echo "▶ Installing server dependencies …"
cd "$SERVER_DIR"
npm install --prefer-offline 2>&1 | tail -3

# ── 2. Install client dependencies ──────────────────────────────────────────
echo ""
echo "▶ Installing client dependencies …"
cd "$CLIENT_DIR"
npm install --prefer-offline 2>&1 | tail -3

# ── 3. Run DB migrations ─────────────────────────────────────────────────────
echo ""
echo "▶ Running database migrations …"
cd "$SERVER_DIR"
node migrate.js

# ── 4. Build the frontend ─────────────────────────────────────────────────────
echo ""
echo "▶ Building frontend …"
cd "$CLIENT_DIR"
npm run build

# ── 5. Start the server (foreground) ─────────────────────────────────────────
echo ""
echo "▶ Starting server on http://localhost:${PORT:-3000}"
echo "  (Ctrl-C to stop)"
echo ""
cd "$SERVER_DIR"
exec node index.js
