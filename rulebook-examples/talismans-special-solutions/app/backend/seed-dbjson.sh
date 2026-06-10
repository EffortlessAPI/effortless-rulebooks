#!/usr/bin/env bash
# ============================================================================
# Reseed the reasoner's data store (app/backend/db.json) from the rulebook.
#
# WHY THIS EXISTS (and why it runs on every build):
#   The reasoner substrate reasons over app/backend/db.json — its data store,
#   the OWL analogue of the Postgres tables. Postgres gets reset on every build
#   (effortless.json: "-exec ./init-db.sh" drops + reloads from the rulebook).
#   db.json had NO such step, so it drifted: when a raw field with a default was
#   ADDED to the rulebook (e.g. RenewalWindowMonths = 2), the stale db.json never
#   gained the value, the reasoner read null, the renewal window was never
#   subtracted, and the UI showed COMPLIANT when it was AT RISK. A restart does
#   not fix this — start.sh never touches db.json. Only a reseed does.
#
#   This script is the missing leg: it makes db.json a build artifact, reset from
#   the rulebook on every build exactly like the Postgres store. Wire it into
#   effortless.json as an "-exec ./app/backend/seed-dbjson.sh" step AFTER
#   init-db.sh (the authoritative raw/derived classification reads the live DB's
#   columns, which init-db.sh has just created).
#
# This file is HAND-MAINTAINED and only RUN by the build — not regenerated.
# ============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Domain-derived default; ERB_RULEBOOK_PATH (consumed by export-cli) only
# overrides. export-cli also derives DATABASE_URL the same domain-aware way.
ERB_DOMAIN="${ERB_DOMAIN:-talismans-special-solutions}"
export ERB_DOMAIN

echo "seed-dbjson: reseeding app/backend/db.json from the rulebook (ERB_DOMAIN=$ERB_DOMAIN)"
# export-cli runs under node + the tsx loader, exactly as start.sh runs the
# server — see control.ts. It reseeds db.json from the rulebook using the live
# DB's columns as the raw/derived authority.
node --import tsx dal/export-cli.ts seed-dbjson
