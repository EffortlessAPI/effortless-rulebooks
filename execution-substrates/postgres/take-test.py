#!/usr/bin/env python3
"""
PostgreSQL Substrate Test Runner
================================
Queries all vw_* views from PostgreSQL and writes results to test-answers/.

This substrate is tested EQUALLY with all other substrates (Python, Go, etc.)
against the canonical answer keys generated directly from the rulebook.

The PostgreSQL views implement the calculated fields using SQL functions
generated from the rulebook formulas. This test verifies that the SQL
implementation matches the canonical specification.
"""

import json
import os
import re
import sys
from pathlib import Path

import psycopg2
from psycopg2.extras import RealDictCursor

# Setup paths
SCRIPT_DIR = Path(__file__).parent.resolve()
PROJECT_ROOT = SCRIPT_DIR.parent.parent
TESTING_DIR = PROJECT_ROOT / "testing"
BLANK_TESTS_DIR = TESTING_DIR / "blank-tests"
TEST_ANSWERS_DIR = SCRIPT_DIR / "test-answers"
POSTGRES_DIR = PROJECT_ROOT / "postgres"

# Add orchestration to path for shared utilities
sys.path.insert(0, str(PROJECT_ROOT / "orchestration"))
from shared import load_rulebook, to_snake_case


def get_db_connection_string():
    """Get database connection string from environment or init-db.sh"""
    conn_str = os.environ.get("DATABASE_URL")
    if conn_str:
        return conn_str

    # Try to infer from init-db.sh
    init_db_path = POSTGRES_DIR / "init-db.sh"
    if init_db_path.exists():
        with open(init_db_path, 'r') as f:
            for line in f:
                if 'DEFAULT_CONN=' in line:
                    match = re.search(r'DEFAULT_CONN="([^"]+)"', line)
                    if match:
                        return match.group(1)

    # Default fallback
    return "postgresql://postgres@localhost:5432/postgres"


def discover_views(conn) -> list:
    """Query postgres for all vw_* views"""
    cur = conn.cursor()
    cur.execute("""
        SELECT table_name
        FROM information_schema.views
        WHERE table_name LIKE 'vw_%'
        ORDER BY table_name
    """)
    views = [row[0] for row in cur.fetchall()]
    cur.close()
    return views


def view_to_entity_name(view_name: str) -> str:
    """Convert view name to entity name: vw_products -> products"""
    return view_name.replace('vw_', '')


def query_view(conn, view_name: str, pk: str = None) -> list:
    """Query a view and return all records as dicts"""
    cur = conn.cursor(cursor_factory=RealDictCursor)

    if pk:
        cur.execute(f"SELECT * FROM {view_name} ORDER BY {pk}")
    else:
        cur.execute(f"SELECT * FROM {view_name}")

    rows = cur.fetchall()
    records = [dict(row) for row in rows]
    cur.close()
    return records


def get_primary_key(rulebook: dict, entity_name: str) -> str:
    """
    Discover the primary key for an entity.
    First non-nullable field, or first field ending in 'Id'.
    """
    # Get schema (handle PascalCase conversion)
    entity_data = None
    for key in rulebook:
        if to_snake_case(key) == entity_name or key == entity_name:
            entity_data = rulebook[key]
            break

    if not entity_data:
        return None

    schema = entity_data.get('schema', [])

    # First try: find first non-nullable field
    for field in schema:
        if field.get('nullable') == False:
            return to_snake_case(field['name'])

    # Second try: find first field ending in 'Id'
    for field in schema:
        if field['name'].endswith('Id'):
            return to_snake_case(field['name'])

    # Fallback: first field
    if schema:
        return to_snake_case(schema[0]['name'])

    return None


def main():
    print("PostgreSQL Substrate: Starting test...")

    # Load rulebook for primary key discovery
    try:
        rulebook = load_rulebook()
    except FileNotFoundError as e:
        print(f"Warning: Could not load rulebook: {e}")
        rulebook = {}

    # Connect to database
    conn_str = get_db_connection_string()
    try:
        conn = psycopg2.connect(conn_str)
        print(f"PostgreSQL Substrate: Connected to database")
    except Exception as e:
        print(f"FATAL: Failed to connect to database: {e}")
        print(f"  Connection string: {conn_str}")
        sys.exit(1)

    # Ensure output directory exists
    TEST_ANSWERS_DIR.mkdir(parents=True, exist_ok=True)

    # Discover and query all views
    views = discover_views(conn)
    print(f"PostgreSQL Substrate: Found {len(views)} views")

    total_records = 0
    for view in views:
        entity = view_to_entity_name(view)
        pk = get_primary_key(rulebook, entity)

        records = query_view(conn, view, pk)

        # Save to test-answers
        output_path = TEST_ANSWERS_DIR / f"{entity}.json"
        with open(output_path, 'w') as f:
            json.dump(records, f, indent=2, default=str)

        total_records += len(records)
        print(f"  -> {entity}: {len(records)} records")

    conn.close()

    print(f"PostgreSQL Substrate: Processed {len(views)} views, {total_records} total records")


if __name__ == "__main__":
    main()
