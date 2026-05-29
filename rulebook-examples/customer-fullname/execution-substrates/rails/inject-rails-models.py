#!/usr/bin/env python3
"""
Rulebook-to-Rails transpiler

Reads an Effortless rulebook and generates Rails models, migrations, and views.
Compares against current Postgres schema to generate only necessary migrations.
The generated Rails app follows Effortless doctrine:
  - Views are the contract (all computed columns materialized)
  - Models read from vw_<entity> by default
  - Migrations create raw tables and views

Usage:
  ERB_RULEBOOK_PATH=/path/to/rulebook.json ERB_OUTPUT_DIR=/path/to/rails \
  ERB_PGDUMP_PATH=/path/to/schema.sql python inject-rails-models.py

Or query a live Postgres database:
  ERB_DATABASE_URL=postgresql://... ERB_OUTPUT_DIR=/path/to/rails python inject-rails-models.py
"""

import json
import os
import sys
import re
import subprocess
from pathlib import Path
from datetime import datetime


def to_snake_case(name: str) -> str:
    """Convert PascalCase to snake_case."""
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
    return re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()


def convert_formula_to_sql(formula: str) -> str:
    """Convert Excel-style formula to PostgreSQL SQL."""
    if not formula:
        return "NULL"

    # Remove leading =
    if formula.startswith('='):
        formula = formula[1:]

    # Replace {{FieldName}} with "field_name"
    def replace_field(match):
        field_name = match.group(1)
        sql_name = to_snake_case(field_name)
        return f'"{sql_name}"'

    sql = re.sub(r'\{\{(\w+)\}\}', replace_field, formula)

    # Replace Excel functions with SQL equivalents
    sql = sql.replace(' & ', ' || ')  # String concatenation (keep spaces)
    sql = sql.replace('&', '||')       # String concatenation (no spaces)

    # LEFT(str, n) -> SUBSTRING(str FROM 1 FOR n)
    sql = re.sub(r'LEFT\(([^,]+),\s*(\d+)\)', r'SUBSTRING(\1 FROM 1 FOR \2)', sql)

    # RIGHT(str, n) -> SUBSTRING(str FROM LENGTH(str) - n + 1)
    sql = re.sub(r'RIGHT\(([^,]+),\s*(\d+)\)', r'SUBSTRING(\1 FROM LENGTH(\1) - \2 + 1)', sql)

    return sql


def get_current_schema() -> dict:
    """Extract current Postgres schema from pgdump file or live database."""
    pgdump_path = os.environ.get('ERB_PGDUMP_PATH')
    database_url = os.environ.get('ERB_DATABASE_URL')

    schema = {}

    if pgdump_path and os.path.exists(pgdump_path):
        # Parse pgdump file
        with open(pgdump_path) as f:
            schema = parse_pgdump(f.read())
    elif database_url:
        # Query live Postgres database
        schema = query_postgres_schema(database_url)

    return schema


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


def main():
    rulebook_path = os.environ.get('ERB_RULEBOOK_PATH')
    output_dir = os.environ.get('ERB_OUTPUT_DIR')

    if not rulebook_path or not output_dir:
        print("Error: ERB_RULEBOOK_PATH and ERB_OUTPUT_DIR must be set", file=sys.stderr)
        sys.exit(1)

    # Read rulebook
    with open(rulebook_path) as f:
        rulebook = json.load(f)

    # Get current schema if available
    current_schema = get_current_schema()

    # Create output directories
    Path(f"{output_dir}/app/models").mkdir(parents=True, exist_ok=True)
    Path(f"{output_dir}/db/migrate").mkdir(parents=True, exist_ok=True)
    Path(f"{output_dir}/db/views").mkdir(parents=True, exist_ok=True)

    migration_timestamp = int(datetime.now().timestamp() * 1000)

    for table_name, table_data in rulebook.items():
        if table_name.startswith('_'):
            continue

        schema = table_data.get('schema', [])
        data = table_data.get('data', [])

        # Separate raw and computed fields
        raw_fields = [f for f in schema if f['type'] == 'raw']
        computed_fields = [f for f in schema if f['type'] in ('calculated', 'lookup', 'aggregation')]

        # Generate model
        model_name = table_name
        model_code = generate_model(model_name, raw_fields, computed_fields)

        model_file = f"{output_dir}/app/models/{to_snake_case(model_name)}.rb"
        with open(model_file, 'w') as f:
            f.write(model_code)
        print(f"Generated {model_file}")

        # Generate migration (comparing to current schema if available)
        snake_table = to_snake_case(table_name)
        current_table_schema = current_schema.get(snake_table, {})
        migration_code = generate_smart_migration(model_name, raw_fields, computed_fields, current_table_schema)

        if migration_code:  # Only write if there are changes
            migration_file = f"{output_dir}/db/migrate/{migration_timestamp}_{snake_table}.rb"
            with open(migration_file, 'w') as f:
                f.write(migration_code)
            print(f"Generated {migration_file}")

        # Generate view SQL
        view_sql = generate_view_sql(table_name, schema)
        views_file = f"{output_dir}/db/views/{snake_table}.sql"

        with open(views_file, 'w') as f:
            f.write(view_sql)
        print(f"Generated {views_file}")

        # Generate seeds
        if data:
            seeds_code = generate_seeds(model_name, raw_fields, data)
            seeds_file = f"{output_dir}/db/seeds/{snake_table}.rb"

            Path(f"{output_dir}/db/seeds").mkdir(parents=True, exist_ok=True)
            with open(seeds_file, 'w') as f:
                f.write(seeds_code)
            print(f"Generated {seeds_file}")

    # Generate main seeds.rb that loads all seed files
    seeds_main = """# This file loads all entity-specific seeds.
Dir[File.join(__dir__, 'seeds', '*.rb')].each { |file| load file }
"""
    with open(f"{output_dir}/db/seeds.rb", 'w') as f:
        f.write(seeds_main)
    print(f"Generated {output_dir}/db/seeds.rb")


def generate_model(model_name: str, raw_fields: list, computed_fields: list) -> str:
    """Generate a Rails model that reads from vw_<entity>."""
    snake_name = to_snake_case(model_name)

    code = f"""# {model_name} - Effortless Entity
# This model reads from vw_{snake_name}, which materializes all calculated/lookup/aggregation fields.
# The view IS the contract — never re-derive a computed value in application code.

class {model_name} < ApplicationRecord
  self.table_name = 'vw_{snake_name}'
  self.primary_key = '{to_snake_case(raw_fields[0]['name'])}'

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


def generate_migration(model_name: str, raw_fields: list, computed_fields: list) -> str:
    """Generate a migration that creates the table and view (if table doesn't exist)."""
    snake_name = to_snake_case(model_name)
    class_name = f"Create{model_name}"

    # Determine primary key
    pk_field = raw_fields[0] if raw_fields else None
    pk_name = to_snake_case(pk_field['name']) if pk_field else 'id'

    code = f"""class {class_name} < ActiveRecord::Migration[7.0]
  def change
    create_table :{snake_name}, primary_key: :{to_snake_case(pk_name)}, id: false do |t|
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

        if attr_name == pk_name:
            code += f"      t.{rail_type} :{attr_name}, primary_key: true{null_constraint}\n"
        else:
            code += f"      t.{rail_type} :{attr_name}{null_constraint}\n"

    code += """    end
  end
end
"""
    return code


def generate_smart_migration(model_name: str, raw_fields: list, computed_fields: list, current_columns: dict) -> str:
    """Generate a migration that syncs current schema to rulebook (add/drop/modify columns)."""
    snake_name = to_snake_case(model_name)

    if not current_columns:
        # Table doesn't exist, use full create_table migration
        return generate_migration(model_name, raw_fields, computed_fields)

    # Table exists, generate alter migrations for differences
    changes = []
    rulebook_attrs = {}

    # Build set of rulebook columns
    for field in raw_fields:
        attr_name = to_snake_case(field['name'])
        rulebook_attrs[attr_name] = field

    # Find columns to add (in rulebook but not in DB)
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

    # Find columns to drop (in DB but not in rulebook)
    for col_name in current_columns:
        if col_name not in rulebook_attrs:
            changes.append(f'    remove_column :{snake_name}, :{col_name}')

    if not changes:
        return ""  # No changes needed

    class_name = f"Update{model_name}"
    code = f"""class {class_name} < ActiveRecord::Migration[7.0]
  def change
"""
    code += '\n'.join(changes) + '\n'
    code += """  end
end
"""
    return code


def generate_view_sql(table_name: str, schema: list) -> str:
    """Generate SQL for the view that includes all computed fields."""
    snake_name = to_snake_case(table_name)

    columns = []
    for field in schema:
        field_name = field['name']
        attr_name = to_snake_case(field_name)

        if field['type'] == 'raw':
            columns.append(f'  "{attr_name}"')
        else:
            # Calculated, lookup, or aggregation field
            formula = field.get('formula', '')
            sql_expr = convert_formula_to_sql(formula)
            columns.append(f'  {sql_expr} AS "{attr_name}"')

    view_sql = f"""-- Computed view for {table_name}
-- The view IS the contract: all calculated/lookup/aggregation columns are materialized here.
-- Application code reads from this view, never from the raw table.

CREATE OR REPLACE VIEW vw_{snake_name} AS
SELECT
{',\n'.join(columns)}
FROM {snake_name};
"""
    return view_sql


def generate_seeds(model_name: str, raw_fields: list, data: list) -> str:
    """Generate Rails seed statements."""
    code = f"# Seed data for {model_name}\n\n"

    for row in data:
        attrs = {}
        for field in raw_fields:
            field_name = field['name']
            attr_name = to_snake_case(field_name)

            if field_name in row:
                value = row[field_name]
                attrs[attr_name] = value

        if attrs:
            attr_str = ', '.join(f'{k}: {repr(v)}' for k, v in attrs.items())
            code += f"{model_name}.create({attr_str})\n"

    return code


if __name__ == '__main__':
    main()
