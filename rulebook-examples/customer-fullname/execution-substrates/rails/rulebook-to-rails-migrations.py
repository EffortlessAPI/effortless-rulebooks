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


def parse_pgdump(pgdump_text: str) -> dict:
    """Parse CREATE TABLE statements from a pgdump."""
    schema = {}

    # Extract CREATE TABLE blocks
    pattern = r'CREATE TABLE.*?"(\w+)".*?\((.*?)\);'
    matches = re.finditer(pattern, pgdump_text, re.DOTALL)

    for match in matches:
        table_name = match.group(1)
        columns_text = match.group(2)

        columns = {}
        for line in columns_text.split(','):
            line = line.strip()
            if not line or line.startswith('CONSTRAINT'):
                continue

            parts = line.split()
            if len(parts) >= 2:
                col_name = parts[0].strip('"')
                col_type = parts[1]
                columns[col_name] = col_type

        schema[table_name] = columns

    return schema


def query_postgres_schema(database_url: str) -> dict:
    """Query Postgres to get current schema."""
    try:
        import psycopg2
    except ImportError:
        print("Warning: psycopg2 not installed. Skipping live database schema check.", file=sys.stderr)
        return {}

    try:
        conn = psycopg2.connect(database_url)
        cursor = conn.cursor()

        schema = {}

        # Get all tables and their columns
        cursor.execute("""
            SELECT table_name, column_name, data_type
            FROM information_schema.columns
            WHERE table_schema = 'public'
            ORDER BY table_name, ordinal_position
        """)

        current_table = None
        for row in cursor.fetchall():
            table_name, column_name, data_type = row

            if table_name not in schema:
                schema[table_name] = {}

            schema[table_name][column_name] = data_type

        cursor.close()
        conn.close()

        return schema
    except Exception as e:
        print(f"Warning: Could not connect to database: {e}", file=sys.stderr)
        return {}


def get_current_schema() -> dict:
    """Extract current Postgres schema from pgdump file or live database."""
    pgdump_path = os.environ.get('ERB_PGDUMP_PATH')
    database_url = os.environ.get('ERB_DATABASE_URL')

    schema = {}

    if pgdump_path and os.path.exists(pgdump_path):
        with open(pgdump_path) as f:
            schema = parse_pgdump(f.read())
    elif database_url:
        schema = query_postgres_schema(database_url)

    return schema


def main():
    rulebook_path = os.environ.get('ERB_RULEBOOK_PATH')
    output_dir = os.environ.get('ERB_OUTPUT_DIR')

    if not rulebook_path or not output_dir:
        print("Error: ERB_RULEBOOK_PATH and ERB_OUTPUT_DIR must be set", file=sys.stderr)
        sys.exit(1)

    with open(rulebook_path) as f:
        rulebook = json.load(f)

    current_schema = get_current_schema()

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
        current_table_schema = current_schema.get(snake_table, {})

        migration_code = generate_migration(table_name, raw_fields, current_table_schema)

        if migration_code:
            migration_file = f"{output_dir}/db/migrate/{migration_timestamp}_{snake_table}.rb"

            with open(migration_file, 'w') as f:
                f.write(migration_code)
            print(f"Generated {migration_file}")


def generate_migration(table_name: str, raw_fields: list, current_columns: dict) -> str:
    """Generate migration (full create_table or smart alter_table)."""
    snake_name = to_snake_case(table_name)

    if not current_columns:
        # Table doesn't exist, generate full create_table
        return generate_create_table_migration(table_name, raw_fields, snake_name)
    else:
        # Table exists, generate smart alter_table migrations
        return generate_alter_table_migration(table_name, raw_fields, current_columns, snake_name)


def generate_create_table_migration(table_name: str, raw_fields: list, snake_name: str) -> str:
    """Generate a create_table migration."""
    pk_field = raw_fields[0] if raw_fields else None
    pk_attr = to_snake_case(pk_field['name']) if pk_field else 'id'
    class_name = f"Create{table_name}"

    code = f"""class {class_name} < ActiveRecord::Migration[6.0]
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


def generate_alter_table_migration(table_name: str, raw_fields: list, current_columns: dict, snake_name: str) -> str:
    """Generate an alter_table migration (add/drop columns as needed)."""
    changes = []
    rulebook_attrs = {}

    # Build set of rulebook columns
    for field in raw_fields:
        attr_name = to_snake_case(field['name'])
        rulebook_attrs[attr_name] = field

    # Find columns to add
    for attr_name, field in rulebook_attrs.items():
        if attr_name not in current_columns:
            datatype = field.get('datatype', 'string').lower()
            rail_type = {
                'string': 'string',
                'number': 'decimal',
                'boolean': 'boolean',
                'date': 'date',
                'datetime': 'datetime',
            }.get(datatype, 'string')
            nullable = field.get('nullable', True)
            null_constraint = ", null: false" if not nullable else ", null: true"
            changes.append(f'    add_column :{snake_name}, :{attr_name}, :{rail_type}{null_constraint}')

    # Find columns to drop
    for col_name in current_columns:
        if col_name not in rulebook_attrs:
            changes.append(f'    remove_column :{snake_name}, :{col_name}')

    if not changes:
        return ""  # No changes needed

    class_name = f"Update{table_name}"
    code = f"""class {class_name} < ActiveRecord::Migration[6.0]
  def change
"""
    code += '\n'.join(changes) + '\n'
    code += """  end
end
"""
    return code


if __name__ == '__main__':
    main()
