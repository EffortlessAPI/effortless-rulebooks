#!/usr/bin/env bash
# Build step: cross-substrate conformance receipt for the state-machine closure.
# Runs the local OWL-RL reasoner (owl/reason.py) over the just-generated ontology +
# individuals and diffs its reachability closure against the Postgres
# vw_state_transition_rules_closure. Exits non-zero (failing the build) if the two
# substrates disagree - so the receipt is witnessed on every `effortless build`,
# not a claim. Part of `effortless build`; runs right after rulebook-to-owl, which
# runs right after postgres init-db (so the closure view exists).
set -euo pipefail
cd "$(dirname "$0")"

if [[ ! -f src/ontology.owl ]]; then
  echo "[conformance] ERROR: owl/src not generated - did rulebook-to-owl run first?" >&2
  exit 1
fi

python3 reason.py
