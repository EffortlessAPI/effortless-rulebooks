#!/usr/bin/env bash
# ============================================================================
# customer-fullname — Explainer DAG bundle step
# ============================================================================
# Run as the `-exec` follow-on to the rulebook-to-react-explainer-dag
# transpiler (see effortless.json → "buildexplainerdag"). It is the second
# half of that tool's output: the transpiler regenerates the explainer-dag
# *source* (web/src/explainer-dag/embedded-*.ts etc.) on every `effortless
# build`; this script compiles that source into the served bundle (web/dist)
# with Vite — exactly the way `-exec ./init-db.sh` applies the SQL the
# postgres transpiler just wrote.
#
# Without this step the source follows the rulebook but the *viewed* app does
# not, because nothing re-runs Vite. This closes that gap so the explainer-dag
# "follows along" like every other substrate.
#
# Mirror of init-db.sh's ownership note: this file belongs to the project the
# moment it is created. The transpiler does NOT regenerate or overwrite it —
# customize freely (swap `build` for `build -- --mode staging`, add a deploy
# step, etc.). For active development prefer a live dev server instead:
#   cd web && npm run dev      # HMR live-updates on every effortless build
# ============================================================================
set -euo pipefail

# Run from this script's own directory (web/), where package.json lives, so the
# step is independent of the caller's CWD.
cd "$(dirname "${BASH_SOURCE[0]}")"

# Install deps only when missing (fresh checkout). Steady-state is just the
# ~0.4s vite build. A missing/broken install surfaces loudly from npm itself —
# we don't swallow it.
if [ ! -d node_modules ]; then
  echo "📦 web/node_modules missing — installing dependencies..."
  npm install
fi

echo "🔨 Bundling explainer-dag (vite build) → web/dist ..."
npm run build

echo "✅ Explainer-dag bundle up to date (web/dist)."
