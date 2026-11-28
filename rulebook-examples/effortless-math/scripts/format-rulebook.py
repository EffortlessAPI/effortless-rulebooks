#!/usr/bin/env python3
"""Normalize a rulebook to *standard JSON with single-line leaves*.

Doctrine (see effortless-math/CLAUDE.md > "Rulebook JSON formatting"): every
`schema` field object and every `data` row object is on exactly one line; the
structure around them stays pretty-printed.

The layout itself is the platform's one implementation, the repo root's
scripts/rulebook_layout.py (the same text the generated rulebook editor's Save
writes). This script only keeps this project's default path and its callers.

Usage:
    python3 scripts/format-rulebook.py [path-to-rulebook.json]

Defaults to effortless-rulebook/effortless-math-rulebook.json relative to the
project root. Fails loudly on invalid JSON.
"""
from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT = ROOT / "effortless-rulebook" / "effortless-math-rulebook.json"

sys.path.insert(0, str(ROOT.parents[1] / "scripts"))
import rulebook_layout  # noqa: E402


def main(argv: list[str]) -> int:
    path = Path(argv[1]).resolve() if len(argv) > 1 else DEFAULT
    if not path.exists():
        raise FileNotFoundError(f"Rulebook does not exist: {path}")
    original = path.read_text(encoding="utf-8")
    formatted = rulebook_layout.format_text(original)  # raises loudly on invalid JSON
    path.write_text(formatted, encoding="utf-8")
    print(f"{'formatted' if formatted != original else 'already normalized'}: {path}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
