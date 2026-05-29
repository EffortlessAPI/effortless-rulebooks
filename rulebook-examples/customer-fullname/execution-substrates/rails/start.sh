#!/bin/bash
set -e

# Create .env if it doesn't exist
if [ ! -f .env ]; then
  echo "Creating .env from .env.example..."
  cp .env.example .env
fi

# Set defaults (no need to parse .env, just use these)
export RAILS_ENV=${RAILS_ENV:-development}
export DB_HOST=${DB_HOST:-localhost}
export DB_PORT=${DB_PORT:-5432}
export DB_USER=${DB_USER:-postgres}
export DB_PASSWORD=${DB_PASSWORD:-}
export DATABASE_URL="postgresql://${DB_USER}:${DB_PASSWORD}@${DB_HOST}:${DB_PORT}/erb_customer_fullname_${RAILS_ENV}"

# Ensure database exists and migrations are run
echo "Setting up database..."
bundle exec rails db:create || true
bundle exec rails db:migrate
bundle exec rails db:seed

# Start Rails server
echo "Starting Rails server on http://localhost:3000"
bundle exec rails server -b 0.0.0.0 -p 3000
