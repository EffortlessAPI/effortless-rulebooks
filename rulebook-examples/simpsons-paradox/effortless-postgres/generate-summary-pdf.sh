#!/usr/bin/env bash
# ============================================================================
# generate-summary-pdf.sh
# ============================================================================
# Writes simpsons-paradox-summary.pdf — corpus snapshot from live vw_* views,
# with deductive/empirical framing. Requires a populated local DB.
#
# Run automatically at the end of init-db.sh, or standalone:
#   ./effortless-postgres/generate-summary-pdf.sh
#   DATABASE_URL=postgresql://... ./effortless-postgres/generate-summary-pdf.sh
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
DEFAULT_CONN="postgresql://postgres@localhost:5432/simpsons_paradox"
DATABASE_URL="${DATABASE_URL:-${1:-$DEFAULT_CONN}}"
OUT="${SUMMARY_PDF_PATH:-$PROJECT_DIR/simpsons-paradox-summary.pdf}"

echo "[summary-pdf] source: $DATABASE_URL"
echo "[summary-pdf] output: $OUT"

cd "$PROJECT_DIR/app/backend"
if [ ! -d node_modules/pdfkit ]; then
  echo "[summary-pdf] installing backend deps…"
  npm install --silent
fi

DATABASE_URL="$DATABASE_URL" SUMMARY_PDF_PATH="$OUT" npx tsx generate-summary-pdf-cli.ts
