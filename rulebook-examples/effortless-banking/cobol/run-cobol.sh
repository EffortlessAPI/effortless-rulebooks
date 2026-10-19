#!/bin/bash
# Compile and run the COBOL program in cobol/src/ against this project's
# rulebook data; what COBOL computed lands in cobol/output/<table>.json.
# Needs GnuCOBOL (cobc). See run_cobol.py.
set -e
cd "$(dirname "$0")"
exec python3 run_cobol.py "$@"
