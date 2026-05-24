#!/usr/bin/env bash
# Build-time patcher for licensed EF dataclass sources.
#
# The transpiler emits bool formula getters as:
#     public bool X { get => nav != null ? nav.Y : null; set { } }
# which doesn't compile (null is not a bool). String formulas in the
# same shape compile fine because strings are nullable. We can't edit
# the licensed sources, so we patch a copy here before compile.
#
# Idempotent. Operates on every *.cs found under the source dir; writes
# patched copies (preserving directory structure) under the dest dir.
#
# Usage: patch-sources.sh <src-root> <dst-root>

set -euo pipefail

SRC="$1"
DST="$2"

mkdir -p "$DST"
rm -rf "$DST"/*

while IFS= read -r -d '' file; do
    rel="${file#"$SRC"/}"
    out="$DST/$rel"
    mkdir -p "$(dirname "$out")"
    # Perl in slurp mode: only `public bool ... : null; set { } }` blocks
    # are touched. String/object formulas are left as-is because the type
    # check `public bool` is part of the match.
    perl -0pe '
        s{
            (public\s+bool\s+\w+\s*\{\s*get\s*=>[^;]*?)
            \s*:\s*null
            (\s*;\s*set\s*\{\s*\}\s*\})
        }{$1 : false$2}gsx
    ' "$file" > "$out"
done < <(find "$SRC" -type f -name '*.cs' -print0)
