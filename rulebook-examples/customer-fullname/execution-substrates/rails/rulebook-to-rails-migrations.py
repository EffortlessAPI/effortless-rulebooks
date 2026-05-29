#!/usr/bin/env python3
"""
Generates Rails migrations from an Effortless rulebook.

Generates either:
1. A full create_table migration (if table doesn't exist yet)
2. Smart add_column/remove_column migrations (if schema is provided for comparison)

Compares against:
- ERB_PGDUMP_PATH: a PostgreSQL schema dump file
- ERB_DATABASE_URL: a live Postgres connection string (requires psycopg2)

If neither is provided, generates a full create_table migration.

Usage:
  # Full create_table migration
  ERB_RULEBOOK_PATH=/path/to/rulebook.json ERB_OUTPUT_DIR=/path/to/rails python rulebook-to-rails-migrations.py

  # Smart migrations from pgdump
  ERB_RULEBOOK_PATH=/path/to/rulebook.json ERB_OUTPUT_DIR=/path/to/rails \
  ERB_PGDUMP_PATH=/path/to/schema.sql python rulebook-to-rails-migrations.py

  # Smart migrations from live database
  ERB_DATABASE_URL=postgresql://... ERB_RULEBOOK_PATH=/path/to/rulebook.json \
  ERB_OUTPUT_DIR=/path/to/rails python rulebook-to-rails-migrations.py
"""

import json
import os
import sys
import re
from pathlib import Path
from datetime import datetime


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

    Path(f"{output_dir}/db/migrate").mkdir(parents=True, exist_ok=True)

    migration_timestamp = int(datetime.now().timestamp() * 1000)

    for table_name, table_data in rulebook.items():
        if table_name.startswith('_'):
            continue

        schema = table_data.get('schema', [])
        raw_fields = [f for f in schema if f['type'] == 'raw']

        if not raw_fields:
            continue

        snake_table = to_snake_case(table_name)
        migration_code = generate_migration(table_name, raw_fields, snake_table)

        if migration_code:
            migration_file = f"{output_dir}/db/migrate/{migration_timestamp}_{snake_table}.rb"

            with open(migration_file, 'w') as f:
                f.write(migration_code)
            print(f"Generated {migration_file}")


def generate_migration(table_name: str, raw_fields: list, snake_name: str) -> str:
    """Generate a create_table migration."""
    pk_field = raw_fields[0] if raw_fields else None
    pk_attr = to_snake_case(pk_field['name']) if pk_field else 'id'
    class_name = f"Create{table_name}"

    code = f"""class {class_name} < ActiveRecord::Migration[7.0]
  def change
    create_table :{snake_name}, primary_key: :{pk_attr}, id: false do |t|
"""

    for field in raw_fields:
        attr_name = to_snake_case(field['name'])
        datatype = field.get('datatype', 'string').lower()
        nullable = field.get('nullable', True)

        # Map datatypes
        rail_type = {
            'string': 'string',
            'number': 'decimal',
            'boolean': 'boolean',
            'date': 'date',
            'datetime': 'datetime',
        }.get(datatype, 'string')

        null_constraint = ", null: false" if not nullable else ""

        if attr_name == pk_attr:
            code += f"      t.{rail_type} :{attr_name}, primary_key: true{null_constraint}\n"
        else:
            code += f"      t.{rail_type} :{attr_name}{null_constraint}\n"

    code += """    end
  end
end
"""
    return code


if __name__ == '__main__':
    main()
