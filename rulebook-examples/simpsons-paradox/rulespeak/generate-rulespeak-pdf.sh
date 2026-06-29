#!/usr/bin/env bash
# Writes rulespeak/rulespeak.pdf from rulespeak/rulespeak.html (requires Chrome/Chromium).
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
HTML="$SCRIPT_DIR/rulespeak.html"
OUT="$SCRIPT_DIR/rulespeak.pdf"

if [ ! -f "$HTML" ]; then
  echo "[rulespeak-pdf] rulespeak.html not found — run effortless build first" >&2
  exit 1
fi

CHROME=""
for candidate in \
  "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" \
  "/Applications/Chromium.app/Contents/MacOS/Chromium" \
  "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge"; do
  if [ -x "$candidate" ]; then
    CHROME="$candidate"
    break
  fi
done

if [ -z "$CHROME" ]; then
  if command -v google-chrome >/dev/null 2>&1; then CHROME="google-chrome"; fi
  if command -v chromium >/dev/null 2>&1; then CHROME="chromium"; fi
fi

if [ -z "$CHROME" ]; then
  echo "[rulespeak-pdf] Chrome/Chromium not found — cannot render PDF" >&2
  exit 1
fi

"$CHROME" --headless=new --disable-gpu --no-pdf-header-footer \
  --print-to-pdf="$OUT" "file://$HTML"

echo "[rulespeak-pdf] wrote $OUT"
