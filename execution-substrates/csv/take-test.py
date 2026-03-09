#!/usr/bin/env python3
"""
Read test answers from the CSV substrate.

This script reads the csv file and populates test-answers.json
by matching CSV columns to JSON fields.

Unlike the xlsx substrate which reads from rulebook.xlsx, this substrate
reads from the flat CSV export, demonstrating that the CSV format contains
all the information needed to populate test answers.
"""

import csv
import json
import re
import sys
from pathlib import Path


def to_snake_case(name):
    """Convert PascalCase or camelCase to snake_case.

    Also handles fields with existing underscores: Bio_HockettScore -> bio_hockett_score
    """
    # Use [^_] to avoid doubling underscores when input already has them
    s1 = re.sub('([^_])([A-Z][a-z]+)', r'\1_\2', name)
    return re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()


def convert_csv_value(value, field_name=None):
    """Convert CSV string value to appropriate Python type."""
    if value is None or value == '':
        return None

    # Handle boolean strings
    if value.lower() == 'true':
        return True
    elif value.lower() == 'false':
        return False

    # Try numeric conversion
    try:
        if '.' in value:
            return float(value)
        return int(value)
    except (ValueError, TypeError):
        pass

    return value


def load_csv_as_lookup(csv_path, pk_field):
    """Load CSV file and return a lookup dict keyed by primary key.

    Args:
        csv_path: Path to the CSV file
        pk_field: Name of the primary key field (snake_case)

    Returns:
        Dict mapping primary key values to row dicts
    """
    lookup = {}

    with open(csv_path, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)

        for row in reader:
            # Convert all values to appropriate types
            converted_row = {}
            for field_name, value in row.items():
                converted_row[field_name] = convert_csv_value(value, field_name)

            # Get primary key value
            pk_value = converted_row.get(pk_field)
            if pk_value is not None:
                lookup[pk_value] = converted_row

    return lookup


def fill_null_fields_from_csv(csv_path, answers_path):
    """Read test-answers.json and fill null fields from CSV values.

    Args:
        csv_path: Path to the .csv file
        answers_path: Path to test-answers.json
    """
    # Load test answers
    with open(answers_path, 'r', encoding='utf-8') as f:
        answers = json.load(f)

    if not answers:
        print("Error: test-answers.json is empty")
        sys.exit(1)

    json_fields = list(answers[0].keys())
    print(f"Found {len(json_fields)} fields in test answers")

    # Find primary key field (first field ending in _id)
    pk_field = next((f for f in json_fields if f.endswith('_id')), json_fields[0])
    print(f"Primary key field: {pk_field}")

    # Load CSV as lookup
    csv_lookup = load_csv_as_lookup(csv_path, pk_field)
    print(f"Loaded {len(csv_lookup)} rows from CSV")

    # Fill null fields
    fields_filled = 0
    records_updated = 0

    for answer in answers:
        pk_value = answer.get(pk_field)
        if pk_value is None or pk_value not in csv_lookup:
            continue

        csv_row = csv_lookup[pk_value]
        record_updated = False

        for field in json_fields:
            if answer.get(field) is None and csv_row.get(field) is not None:
                answer[field] = csv_row[field]
                fields_filled += 1
                record_updated = True

        if record_updated:
            records_updated += 1

    print(f"Filled {fields_filled} null fields across {records_updated} records")

    # Write updated answers
    with open(answers_path, 'w', encoding='utf-8') as f:
        json.dump(answers, f, indent=2)

    print(f"Updated {answers_path}")


def find_csv_for_entity(script_dir, entity):
    """Find CSV file for an entity in test-data/ directory."""
    test_data_dir = script_dir / 'test-data'

    # Try exact match first
    csv_path = test_data_dir / f'{entity}.csv'
    if csv_path.exists():
        return csv_path

    # Try with _ to - conversion
    csv_path = test_data_dir / f'{entity.replace("_", "-")}.csv'
    if csv_path.exists():
        return csv_path

    # Try all csv files and match by content
    if test_data_dir.exists():
        for csv_file in test_data_dir.glob('*.csv'):
            if csv_file.name.startswith('_'):
                continue
            # Check if this might be the right CSV by looking at headers
            try:
                with open(csv_file, 'r', encoding='utf-8') as f:
                    reader = csv.reader(f)
                    headers = next(reader, [])
                    # If entity pk field exists in headers, this is likely the file
                    pk_field = f'{entity.rstrip("s")}_id'  # Simple singularize
                    if pk_field in headers:
                        return csv_file
            except:
                pass

    return None


def run_multi_entity(script_dir):
    """Process all entity files from shared testing/blank-tests/ directory."""
    import shutil

    # Use shared blank-tests directory at project root
    project_root = script_dir.parent.parent
    blank_tests_dir = project_root / 'testing' / 'blank-tests'
    test_answers_dir = script_dir / 'test-answers'

    if not blank_tests_dir.is_dir():
        print(f"Error: {blank_tests_dir} not found")
        sys.exit(1)

    # Ensure output directory exists
    test_answers_dir.mkdir(exist_ok=True)

    # Process each entity file (skip metadata files)
    total_filled = 0
    entity_count = 0

    for blank_test_path in sorted(blank_tests_dir.glob("*.json")):
        filename = blank_test_path.name

        # Skip metadata files
        if filename.startswith('_'):
            continue

        entity = filename.replace('.json', '')
        output_path = test_answers_dir / filename

        # Find CSV file for this entity
        csv_path = find_csv_for_entity(script_dir, entity)

        if csv_path is None:
            print(f"  Warning: No CSV file found for {entity}")
            # Copy blank test as-is
            shutil.copy(blank_test_path, output_path)
            continue

        print(f"\n  Processing {entity} from {csv_path.name}...")

        # Copy blank test to output path first
        shutil.copy(blank_test_path, output_path)

        try:
            fill_null_fields_from_csv(csv_path, output_path)
            entity_count += 1
        except Exception as e:
            print(f"  Warning: Could not process {entity}: {e}")

    print(f"\ncsv: Processed {entity_count} entities")


def main():
    script_dir = Path(__file__).parent
    run_multi_entity(script_dir)


if __name__ == "__main__":
    main()
