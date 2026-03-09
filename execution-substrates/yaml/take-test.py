#!/usr/bin/env python3
"""
YAML Substrate Test Runner
===========================
Computes all calculated fields using the shared erb_calc.py library.

Supports two modes:
1. Multi-entity (--multi-entity): Processes all files in blank-tests/ -> test-answers/
2. Legacy: Processes single test-answers.json file

The schema.yaml serves as documentation and contract — this script
uses the shared calculation library to ensure identical results
with the Python substrate.
"""

import argparse
import glob
import json
import os
import sys

# Get script directory
script_dir = os.path.dirname(os.path.abspath(__file__))

# Add Python substrate directory to path for shared library
python_substrate_dir = os.path.join(script_dir, "..", "python")
sys.path.insert(0, python_substrate_dir)

# Try to import yaml for schema reading (optional - for logging purposes)
try:
    import yaml
    YAML_AVAILABLE = True
except ImportError:
    YAML_AVAILABLE = False
    print("Warning: PyYAML not installed. Schema details will not be logged.")

from erb_calc import compute_all_calculated_fields


def load_schema(schema_path: str) -> dict:
    """Load and parse the YAML schema."""
    if not YAML_AVAILABLE:
        return {}

    with open(schema_path, 'r') as f:
        return yaml.safe_load(f)


def get_calculated_fields_from_schema(schema: dict) -> list:
    """
    Extract the list of calculated field names from the schema.
    Returns fields in DAG order (level 1, then level 2, then level 3).
    """
    calculated_fields = []

    if 'entities' not in schema:
        return calculated_fields

    for entity_name, entity_def in schema['entities'].items():
        if 'calculated_fields' in entity_def:
            # Sort by dag_level if present
            fields_with_level = []
            for field_name, field_def in entity_def['calculated_fields'].items():
                if isinstance(field_def, dict):
                    level = field_def.get('dag_level', 99)
                    fields_with_level.append((level, field_name, field_def))

            fields_with_level.sort(key=lambda x: x[0])
            calculated_fields.extend([(name, defn) for level, name, defn in fields_with_level])

    return calculated_fields


def log_schema_info():
    """Log schema information if available."""
    schema_path = os.path.join(script_dir, "schema.yaml")

    if YAML_AVAILABLE and os.path.exists(schema_path):
        schema = load_schema(schema_path)
        calculated_fields = get_calculated_fields_from_schema(schema)

        print(f"YAML substrate: Loaded schema with {len(calculated_fields)} calculated fields")
        for name, defn in calculated_fields[:5]:  # Show first 5 only
            formula = defn.get('formula', 'N/A') if isinstance(defn, dict) else 'N/A'
            formula_str = str(formula).replace('\n', ' ').strip()
            if len(formula_str) > 50:
                formula_str = formula_str[:47] + "..."
            print(f"  - {name}: {formula_str}")
        if len(calculated_fields) > 5:
            print(f"  ... and {len(calculated_fields) - 5} more")


def process_entity(input_path: str, output_path: str, entity_name: str) -> int:
    """Process a single entity file, computing all calculated fields."""
    with open(input_path, 'r') as f:
        records = json.load(f)

    # Compute all calculated fields for each record
    computed_records = []
    for record in records:
        computed = compute_all_calculated_fields(record, entity_name)
        computed_records.append(computed)

    # Save results
    with open(output_path, 'w') as f:
        json.dump(computed_records, f, indent=2)

    return len(computed_records)


def run_multi_entity():
    """Process all entity files from shared testing/blank-tests/ directory."""
    # Use shared blank-tests directory at project root
    project_root = os.path.join(script_dir, "..", "..")
    blank_tests_dir = os.path.join(project_root, "testing", "blank-tests")
    test_answers_dir = os.path.join(script_dir, "test-answers")

    if not os.path.isdir(blank_tests_dir):
        print(f"Error: {blank_tests_dir} not found")
        sys.exit(1)

    # Ensure output directory exists
    os.makedirs(test_answers_dir, exist_ok=True)

    # Log schema info once
    log_schema_info()

    # Process each entity file (skip metadata files starting with _)
    total_records = 0
    entity_count = 0

    for input_path in sorted(glob.glob(os.path.join(blank_tests_dir, "*.json"))):
        filename = os.path.basename(input_path)

        # Skip metadata files
        if filename.startswith('_'):
            continue

        entity = filename.replace('.json', '')
        output_path = os.path.join(test_answers_dir, filename)

        count = process_entity(input_path, output_path, entity)
        total_records += count
        entity_count += 1

        print(f"  -> {entity}: {count} records")

    print(f"YAML substrate: Processed {entity_count} entities, {total_records} total records")


def run_legacy():
    """Process single test-answers.json file (legacy mode)."""
    input_path = os.path.join(script_dir, "test-answers.json")
    output_path = os.path.join(script_dir, "test-answers.json")

    if not os.path.exists(input_path):
        print(f"Error: {input_path} not found")
        sys.exit(1)

    # Log schema info
    log_schema_info()

    # Load test data
    with open(input_path, 'r') as f:
        records = json.load(f)

    print(f"YAML substrate: Processing {len(records)} records...")

    # Compute all calculated fields for each record using shared library
    computed_records = []
    for record in records:
        computed = compute_all_calculated_fields(record)
        computed_records.append(computed)

    # Save results
    with open(output_path, 'w') as f:
        json.dump(computed_records, f, indent=2)

    print(f"YAML substrate: Saved results to {output_path}")


def main():
    run_multi_entity()


if __name__ == "__main__":
    main()
