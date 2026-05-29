#!/usr/bin/env python3
"""
Generates Rails models from an Effortless rulebook.

Models read from PostgreSQL views (vw_<entity>) where all computed fields
are materialized. Models are read-only.

Usage:
  ERB_RULEBOOK_PATH=/path/to/rulebook.json ERB_OUTPUT_DIR=/path/to/rails python rulebook-to-rails-models.py
"""

import json
import os
import sys
import re
from pathlib import Path


def to_snake_case(name: str) -> str:
    """Convert PascalCase to snake_case."""
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
    return re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()


def main():
    rulebook_path = os.environ.get('ERB_RULEBOOK_PATH')
    output_dir = os.environ.get('ERB_OUTPUT_DIR')

    if not rulebook_path or not output_dir:
        print("Error: ERB_RULEBOOK_PATH and ERB_OUTPUT_DIR must be set", file=sys.stderr)
        sys.exit(1)

    with open(rulebook_path) as f:
        rulebook = json.load(f)

    Path(f"{output_dir}/app/models").mkdir(parents=True, exist_ok=True)

    for table_name, table_data in rulebook.items():
        if table_name.startswith('_'):
            continue

        schema = table_data.get('schema', [])
        raw_fields = [f for f in schema if f['type'] == 'raw']

        if not raw_fields:
            continue

        model_code = generate_model(table_name, raw_fields)
        model_file = f"{output_dir}/app/models/{to_snake_case(table_name)}.rb"

        with open(model_file, 'w') as f:
            f.write(model_code)
        print(f"Generated {model_file}")


def generate_model(table_name: str, raw_fields: list) -> str:
    """Generate a Rails model that reads from vw_<entity>."""
    snake_name = to_snake_case(table_name)
    pk_field = raw_fields[0] if raw_fields else None
    pk_attr = to_snake_case(pk_field['name']) if pk_field else 'id'

    code = f"""# {table_name} - Effortless Entity
# This model reads from vw_{snake_name}, which materializes all calculated/lookup/aggregation fields.
# The view IS the contract — never re-derive a computed value in application code.

class {table_name} < ApplicationRecord
  self.table_name = 'vw_{snake_name}'
  self.primary_key = '{pk_attr}'

  # This model is read-only (views are read-only in Rails)
  def readonly?
    true
  end
"""

    # Add validations for non-nullable raw fields
    for field in raw_fields:
        if not field.get('nullable', True):
            attr_name = to_snake_case(field['name'])
            code += f"  validates :{attr_name}, presence: true\n"

    code += "\nend\n"
    return code


if __name__ == '__main__':
    main()
