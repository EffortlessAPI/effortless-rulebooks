#!/bin/bash
# Start the Rails app for Customer FullName demo
# Usage: ./rails-start.sh [PORT]   (default 8002; e.g. ./rails-start.sh 8003)

set -e

RAILS_DIR="$(dirname "$0")/execution-substrates/rails"

if [ ! -d "$RAILS_DIR" ]; then
  echo "Error: Rails directory not found at $RAILS_DIR"
  exit 1
fi

echo "Starting Rails app..."
cd "$RAILS_DIR"
bash start.sh "$@"
