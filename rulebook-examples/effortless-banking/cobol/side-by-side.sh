#!/bin/bash
# The banking rules computed by Postgres and by COBOL from the same rulebook and
# data, compared cell by cell; writes cobol/side-by-side.html. See side_by_side.py.
set -e
cd "$(dirname "$0")"
exec python3 side_by_side.py "$@"
