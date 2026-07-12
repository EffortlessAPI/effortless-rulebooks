#!/usr/bin/env bash
# ============================================================================
# generate-details-pdf.sh
# ============================================================================
# Writes simpsons-paradox-details.pdf — expanded companion to
# simpsons-paradox-summary.pdf with full hypothesis-level detail (expected /
# observed values), live-status contradiction banners, and tiered invariant
# framing. Requires a populated local DB.
#
# Run automatically at the end of init-db.sh, or standalone:
#   ./effortless-postgres/generate-details-pdf.sh
#   DATABASE_URL=postgresql://... ./effortless-postgres/generate-details-pdf.sh
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
DEFAULT_CONN="postgresql://postgres@localhost:5432/simpsons_paradox"
DATABASE_URL="${DATABASE_URL:-${1:-$DEFAULT_CONN}}"
OUT="${DETAILS_PDF_PATH:-$PROJECT_DIR/simpsons-paradox-details.pdf}"

echo "[details-pdf] source: $DATABASE_URL"
echo "[details-pdf] output: $OUT"

cd "$PROJECT_DIR/app/backend"
if [ ! -d node_modules/pdfkit ]; then
  echo "[details-pdf] installing backend deps…"
  npm install --silent
fi

DATABASE_URL="$DATABASE_URL" DETAILS_PDF_PATH="$OUT" npx tsx generate-details-pdf-cli.ts
