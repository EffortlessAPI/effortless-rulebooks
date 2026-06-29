#!/usr/bin/env bash
# Create local milestone tags for Simpson's Paradox loop landing commits (discovery replay).
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(git -C "$SCRIPT_DIR" rev-parse --show-toplevel)"

tag_commit() {
  local sha="$1"
  local tag="$2"
  if git rev-parse "$tag" >/dev/null 2>&1; then
    local existing
    existing="$(git rev-parse --short "$tag")"
    if [[ "$existing" != "$sha" ]]; then
      echo "ERROR: tag $tag exists at $existing, expected $sha" >&2
      exit 1
    fi
    echo "OK: $tag -> $sha (already exists)"
  else
    git tag -a "$tag" "$sha" -m "Simpson's Paradox loop milestone: $tag"
    echo "Created: $tag -> $sha"
  fi
}

cd "$REPO_ROOT"
tag_commit b96e0fe sp-loop-milestone-01-loops-01-10
tag_commit 29337f5 sp-loop-milestone-02-loop-10-amend
tag_commit 49f01e7 sp-loop-milestone-03-loops-11-15
tag_commit ca31095 sp-loop-milestone-04-loops-16-31
tag_commit a6e0725 sp-loop-milestone-05-loops-23b-33
tag_commit 5a14dcb sp-loop-milestone-06-loops-34-57
tag_commit 4172aae sp-loop-milestone-07-loops-58-60
tag_commit 7b3938e sp-loop-milestone-08-loop-61
tag_commit 6118290 sp-loop-milestone-09-schema-polish

echo ""
echo "Replay example:"
echo "  git checkout sp-loop-milestone-04-loops-16-31"
echo "  cd rulebook-examples/simpsons-paradox && effortless build && ./start.sh"
