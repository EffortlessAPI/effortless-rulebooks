#!/usr/bin/env bash
#
# Driver for the Talisman's Special Solutions OWL project.
#
#   ./run.sh reason      load TBox+ABox, run SHACL rules to fixpoint + OWL-RL,
#                        print the headline inferences (transitive closure etc.)
#   ./run.sh test        run the OWL <-> Postgres conformance suite
#   ./run.sh api         serve the SPARQL-backed competency-question API
#   ./run.sh all         reason, then test (the default)
#
# The ontology under src/ is GENERATED from the rulebook by
# ../../../execution-substrates/owl/inject-into-owl.py (effortless build).
# This script only consumes it.
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$HERE"

# Domain-derived default; DATABASE_URL only overrides (see CLAUDE.md doctrine).
export ERB_DOMAIN="${ERB_DOMAIN:-talismans-special-solutions}"
export DATABASE_URL="${DATABASE_URL:-postgresql://postgres@localhost:5432/erb_${ERB_DOMAIN//-/_}}"

cmd="${1:-all}"

reason() {
  python3 - <<'PY'
import graph
from rdflib import Namespace
ERB = Namespace("http://example.org/erb#")
g = graph.reasoned_graph()
print(f"Reasoned graph: {len(g)} triples (TBox + ABox + SHACL fixpoint + OWL-RL)\n")

print("Step-precedence transitive closure (asserted edges -> full reachability):")
for row in graph.step_precedence_closure():
    tag = "  inferred" if row["is_inferred"] else "  asserted"
    print(f"  {row['from_id']} -> {row['to_id']}{tag}")

print("\nDelegation closure (Release Manager, transitive):")
for r in graph.delegation_closure("ntwf-release-manager-role"):
    print(f"  -> {r}")

print("\nDisjoint agent classes:")
for d in graph.disjoint_classes():
    print(f"  {d['class_a']}  <disjoint>  {d['class_b']}")
PY
}

case "$cmd" in
  reason) reason ;;
  test)   python3 test/conformance_test.py ;;
  api)    exec python3 -m uvicorn api:app --port "${PORT:-8077}" --reload ;;
  all)    reason; echo; echo "=== conformance ==="; python3 test/conformance_test.py ;;
  *) echo "usage: ./run.sh [reason|test|api|all]" >&2; exit 2 ;;
esac
