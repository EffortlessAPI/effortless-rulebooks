#!/usr/bin/env bash
# Postgres-only build: the development loop for this project.
#
# `effortless build` regenerates all seven substrates and takes about seven minutes.
# While building and debugging the app only Postgres matters, so this runs the
# integrity gate, the ONE transpiler that emits postgres-bootstrap/, and the database
# reload. The other substrates (Python, Go, TypeScript, EF, xlsx, OWL) are rebuilt and
# graded once, at the end, with the full `effortless build` + conformance run.
#
# It does not run compile-rulebook, so the derived values baked into the rulebook JSON
# go stale until the next full build. Nothing in the app reads them: the app reads vw_*.
#
#   bash tools/quick_build.sh            # gate + transpile + reload
#   bash tools/quick_build.sh --no-load  # gate + transpile only
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
RB="$ROOT/effortless-rulebook/procedural-knowledge-ontology-rulebook.json"
t0=$SECONDS
python3 "$ROOT/tools/check_rulebook_integrity.py"
echo "== rulebook-to-postgres ($((SECONDS - t0))s)"
( cd "$ROOT/postgres-bootstrap" && effortless rulebook-to-postgres -i "../effortless-rulebook/$(basename "$RB")" )
if grep -l "Formula translation failed" "$ROOT"/postgres-bootstrap/0[0-5]*.sql >/dev/null 2>&1; then
  echo "ERROR: a formula failed to translate (grep 'Formula translation failed' postgres-bootstrap/)" >&2
  grep -n "Formula translation failed" "$ROOT"/postgres-bootstrap/0[0-5]*.sql | head -20 >&2
  exit 1
fi
if [[ "${1:-}" != "--no-load" ]]; then
  echo "== init-db ($((SECONDS - t0))s)"
  bash "$ROOT/init-db.sh"
fi
echo "== quick build done in $((SECONDS - t0))s"
