#!/usr/bin/env bash
# start.sh -- stop/restart the traffic-ticket-contest rulebook editor app.
#
# Delegates to the GENERATED effortless-rulebook/edit-rulebook.sh, which is
# self-updating and already handles collision-proof container naming (by
# install-path fingerprint) and -- via effortless-rulebook/ports.env, which
# pins the three URLs below -- fixed host ports. Do not hand-patch this
# script or fork the generated one; if the entrypoint contract changes
# upstream, edit-rulebook.sh's self-update picks up the fix on next run.
#
# UI:   http://localhost:42452
# API:  http://localhost:42451/api/docs
# PG:   postgresql://postgres:postgres@localhost:5452/effortless-rulebook

set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/effortless-rulebook"
exec bash edit-rulebook.sh "$@"
