#!/usr/bin/env bash
# Cross-substrate conformance receipt for Simpson's Paradox formula fields.
#
# Runs the local OWL-SHACL reasoner (owl/reason.py) over the just-generated
# ontology + individuals, asserts Postgres-computed aggregation values as leaf
# facts, runs pyshacl fixpoint derivation, and diffs the SHACL-derived formula
# fields against vw_treatment_rankings / vw_stratum_summaries / vw_stratum_variables.
#
# Exits non-zero (failing the build) if the two substrates disagree, so the
# receipt is witnessed on every `effortless build`, not a claim.
#
# Part of `effortless build`; runs right after rulebook-to-owl and init-db.sh.
set -euo pipefail
cd "$(dirname "$0")/.."

if [[ ! -f owl/src/ontology.owl ]]; then
  echo "[conformance] ERROR: owl/src not generated — did rulebook-to-owl run first?" >&2
  exit 1
fi

python3 owl/reason.py
