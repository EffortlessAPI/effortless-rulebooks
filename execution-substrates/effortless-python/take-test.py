#!/usr/bin/env python3
"""
effortless-python substrate — grades the COMMERCIAL rulebook-to-python output.

This is the licensed counterpart to the open-source `python` substrate, in the
same way effortless-postgres is to postgres. The open-source bus tool serves
the toys; the commercial tool is what compiles the hard domains, and this
harness executes whatever the commercial tool emitted for the active domain.

WHERE THE CODE COMES FROM
    `effortless build` writes the commercial tool's output into
    <domain>/effortless-python/:

        python_only_erb_simulator.py     the generated calculation library
        orchestration/shared.py          runtime dependencies, shipped by the
        orchestration/formula_parser.py  tool so the bundle is self-contained

    That directory goes on sys.path FIRST, so both the library and its
    `orchestration` package resolve out of the generated bundle. This matters:
    the repo's own orchestration/ is the OPEN-SOURCE version, which lacks the
    features the hard domains need. If it won the import, the module would load
    cleanly and then compute wrong answers — a silent conformance failure that
    looks like a substrate bug.

Fails loudly: a missing bundle, a missing library, or an entity the generated
dispatcher does not know about all raise. Nothing here substitutes a default
for a value the substrate could not compute.
"""

import glob
import json
import os
import sys
from pathlib import Path

SCRIPT_DIR = Path(__file__).resolve().parent
SUBSTRATE_NAME = SCRIPT_DIR.name


def _generated_bundle_dir() -> Path:
    """Locate the commercial tool's output for the active domain."""
    domain_dir = os.environ.get("ERB_DOMAIN_DIR")
    if not domain_dir:
        raise RuntimeError(
            "ERB_DOMAIN_DIR is not set. take-test.py must be invoked by the "
            "orchestrator with ERB_DOMAIN_DIR pointing at the active domain."
        )
    bundle = Path(domain_dir) / "effortless-python"
    if not bundle.is_dir():
        raise RuntimeError(
            f"No commercial rulebook-to-python output at {bundle}. Register the "
            f"tool in this project's effortless.json with RelativePath "
            f"/effortless-python and run `effortless build`."
        )
    library = bundle / "python_only_erb_simulator.py"
    if not library.is_file():
        raise RuntimeError(
            f"Bundle at {bundle} has no python_only_erb_simulator.py — the "
            f"transpiler did not produce its generated library."
        )
    return bundle


def _testing_paths() -> tuple:
    erb_testing = os.environ.get("ERB_TESTING_DIR")
    if not erb_testing:
        raise RuntimeError(
            "ERB_TESTING_DIR is not set. take-test.py must be invoked by the "
            "orchestrator with ERB_TESTING_DIR pointing at the active domain's "
            "testing/ directory."
        )
    return (Path(erb_testing) / "blank-tests",
            Path(erb_testing) / SUBSTRATE_NAME / "test-answers")


def main():
    bundle = _generated_bundle_dir()
    # FIRST on the path, ahead of the repo's open-source orchestration/.
    sys.path.insert(0, str(bundle))

    from python_only_erb_simulator import (
        compute_all_calculated_fields,
        compute_aggregations,
        compute_closures,
        compute_lookups,
    )
    from orchestration.shared import load_rulebook

    print(f"effortless-python: executing generated library from {bundle}")

    blank_tests_dir, test_answers_dir = _testing_paths()
    if not blank_tests_dir.is_dir():
        raise RuntimeError(f"blank-tests not found: {blank_tests_dir}")
    test_answers_dir.mkdir(parents=True, exist_ok=True)

    rulebook = load_rulebook()

    # Materialize every closure relation once; aggregations across entities
    # address them by view name.
    closures = compute_closures(rulebook, bundle)
    for view_name, rows in sorted(closures.items()):
        print(f"  -> {view_name}: {len(rows)} pairs")

    total_records = 0
    entity_count = 0

    for input_path in sorted(glob.glob(str(blank_tests_dir / "*.json"))):
        filename = os.path.basename(input_path)
        if filename.startswith("_"):
            continue

        entity = filename.replace(".json", "")
        with open(input_path, "r", encoding="utf-8") as f:
            records = json.load(f)

        records = compute_lookups(records, entity, rulebook, bundle)
        records = compute_aggregations(records, entity, rulebook, bundle, closures)
        computed = [compute_all_calculated_fields(r, entity) for r in records]

        with open(test_answers_dir / filename, "w", encoding="utf-8") as f:
            json.dump(computed, f, indent=2)

        total_records += len(computed)
        entity_count += 1
        print(f"  -> {entity}: {len(computed)} records")

    print(f"effortless-python: Processed {entity_count} entities, "
          f"{total_records} total records")


if __name__ == "__main__":
    main()
