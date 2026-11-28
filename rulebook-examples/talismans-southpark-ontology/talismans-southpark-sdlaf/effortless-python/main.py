"""
ERB SDK - Python Test Runner (GENERATED - DO NOT EDIT)
=====================================================
Loads every table's blank tests from $ERB_TESTING_DIR/blank-tests, computes
every lookup, aggregation and calculated field, and writes the answers to
$ERB_TESTING_DIR/$ERB_SUBSTRATE_NAME/test-answers. See erb_run in erb_runtime.py.
"""

from erb_runtime import erb_run
from erb_sdk import CALCULATED_FIELD_COUNT, ERB_CLOSURES, ERB_TABLES

if __name__ == "__main__":
    erb_run(ERB_TABLES, ERB_CLOSURES, CALCULATED_FIELD_COUNT)
