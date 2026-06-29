#!/usr/bin/env python3
"""
Generates PostgreSQL view definitions from an Effortless rulebook.

Each view (vw_<entity>) materializes all fields including calculated, lookup, and aggregation fields.
The view IS the contract — all computed values are pre-computed by SQL.

Generates one .sql file per table with a CREATE OR REPLACE VIEW statement.

Usage:
  ERB_RULEBOOK_PATH=/path/to/rulebook.json ERB_OUTPUT_DIR=/path/to/rails python rulebook-to-rails-views.py
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


def convert_formula_to_sql(formula: str) -> str:
    """Convert Excel-style formula to PostgreSQL SQL."""
    if not formula:
        return "NULL"

    # Remove leading =
    if formula.startswith('='):
        formula = formula[1:]

    # Excel string literals are double-quoted ("a, b"); SQL string literals are
    # single-quoted ('a, b'). Convert these FIRST, before we start emitting
    # double-quoted identifiers for {{Field}} refs — otherwise the two uses of
    # the double-quote collide and Postgres reads " " as a column name
    # ("column \" \" does not exist"). Escape any embedded single quotes.
    def excel_string_to_sql(match):
        inner = match.group(1).replace("'", "''")
        return f"'{inner}'"

    formula = re.sub(r'"([^"]*)"', excel_string_to_sql, formula)

    # Replace {{FieldName}} with "field_name"
    def replace_field(match):
        field_name = match.group(1)
        sql_name = to_snake_case(field_name)
        return f'"{sql_name}"'

    sql = re.sub(r'\{\{(\w+)\}\}', replace_field, formula)

    # String concatenation: & becomes ||
    sql = sql.replace(' & ', ' || ')
    sql = sql.replace('&', '||')

    # Excel LEFT function
    sql = re.sub(r'LEFT\(([^,]+),\s*(\d+)\)', r'SUBSTRING(\1 FROM 1 FOR \2)', sql)

    # Excel RIGHT function
    sql = re.sub(r'RIGHT\(([^,]+),\s*(\d+)\)', r'SUBSTRING(\1 FROM LENGTH(\1) - \2 + 1)', sql)

    return sql


def main():
    rulebook_path = os.environ.get('ERB_RULEBOOK_PATH')
    output_dir = os.environ.get('ERB_OUTPUT_DIR')

    if not rulebook_path or not output_dir:
        print("Error: ERB_RULEBOOK_PATH and ERB_OUTPUT_DIR must be set", file=sys.stderr)
        sys.exit(1)

    with open(rulebook_path) as f:
        rulebook = json.load(f)

    Path(f"{output_dir}/db/views").mkdir(parents=True, exist_ok=True)

    for table_name, table_data in rulebook.items():
        if table_name.startswith('_'):
            continue

        schema = table_data.get('schema', [])

        view_sql = generate_view_sql(table_name, schema)
        views_file = f"{output_dir}/db/views/{to_snake_case(table_name)}.sql"

        with open(views_file, 'w') as f:
            f.write(view_sql)
        print(f"Generated {views_file}")


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
{','.join(columns)}
FROM {snake_name};
"""
    return view_sql


if __name__ == '__main__':
    main()
