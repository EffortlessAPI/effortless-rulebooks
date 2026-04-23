#!/usr/bin/env python3
"""
YAML Substrate Test Runner
===========================

GUARD: This substrate must compute calculated fields using its own native
semantics. YAML is a schema/data format, NOT a computation engine — it can
express the schema (schema.yaml) but cannot execute formulas. Therefore this
runner does raw-passthrough only. Calculated, lookup, and aggregation fields
are left null and the grader will score them as failures. That is the entire
point of the conformance harness: each substrate computes what it can in its
own engine, and is scored honestly on what it cannot.

It is FORBIDDEN to import python_only_erb_simulator or any cross-table
interpreter from this script. Doing so would re-introduce the cheat that the
2026-04-23 conformance audit eliminated.
"""

import glob
import json
import os
import sys
from pathlib import Path

script_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, str(Path(script_dir).parent.parent))

try:
    import yaml
    YAML_AVAILABLE = True
except ImportError:
    YAML_AVAILABLE = False


def log_schema_info():
    """Read schema.yaml just for visibility; no execution."""
    schema_path = os.path.join(script_dir, "schema.yaml")
    if not (YAML_AVAILABLE and os.path.exists(schema_path)):
        return
    with open(schema_path, 'r') as f:
        schema = yaml.safe_load(f) or {}
    n_entities = len((schema or {}).get('entities', {}) or {})
    print(f"YAML substrate: schema.yaml loaded ({n_entities} entities). "
          f"YAML cannot execute formulas; raw fields will pass through, "
          f"computed fields will be left null.")


def run_multi_entity():
    project_root = Path(script_dir).parent.parent
    blank_tests_dir = project_root / "testing" / "blank-tests"
    test_answers_dir = Path(script_dir) / "test-answers"

    if not blank_tests_dir.is_dir():
        print(f"Error: {blank_tests_dir} not found")
        sys.exit(1)

    test_answers_dir.mkdir(parents=True, exist_ok=True)
    log_schema_info()

    total_records = 0
    entity_count = 0

    for input_path in sorted(glob.glob(str(blank_tests_dir / "*.json"))):
        filename = os.path.basename(input_path)
        if filename.startswith('_'):
            continue

        with open(input_path, 'r') as f:
            records = json.load(f)

        # Raw passthrough only. No computation. Computed fields remain absent
        # (or null if present in input); the grader counts these as failures.
        output_path = test_answers_dir / filename
        with open(output_path, 'w') as f:
            json.dump(records, f, indent=2)

        entity = filename.replace('.json', '')
        total_records += len(records)
        entity_count += 1
        print(f"  -> {entity}: {len(records)} records (raw passthrough)")

    print(f"YAML substrate: Processed {entity_count} entities, {total_records} total records")


if __name__ == "__main__":
    run_multi_entity()
