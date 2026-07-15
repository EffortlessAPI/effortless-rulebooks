#!/usr/bin/env python3
"""Normalize a rulebook to *standard JSON with single-line leaves*.

Doctrine (see effortless-math/CLAUDE.md > "Rulebook JSON formatting"):

  - Standard JSON: no comments, no trailing commas, round-trips through json.load.
  - Single-line leaves: every LEAF record is emitted on exactly one line.
    A leaf record is:
      * an individual object inside a table's `schema` array (one field definition), and
      * an individual object inside a table's `data` array (one row).
    Container structure (top-level keys, each table's Description/schema/data,
    the arrays themselves) stays pretty-printed and indented; only the innermost
    row / field objects collapse to a single line each.

Why: one changed row => one changed git-diff line, and the file stays greppable
(`grep '"TheoremId": "2-plus-2-equals-4"'` matches the whole row).

Usage:
    python3 scripts/format-rulebook.py [path-to-rulebook.json]

Defaults to effortless-rulebook/effortless-math-rulebook.json relative to repo
root. Fails loudly on invalid JSON or an unexpected top-level shape.
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT = ROOT / "effortless-rulebook" / "effortless-math-rulebook.json"

INDENT = "  "
# Top-level keys that are plain scalars/metadata, not tables.
NON_TABLE_KEYS = {"$schema", "Name", "Description"}


def leaf(obj) -> str:
    """A leaf record printed compact on a single line (stable key order preserved)."""
    return json.dumps(obj, ensure_ascii=False, separators=(", ", ": "))


def emit_array_of_leaves(items, depth: int) -> str:
    """Render a `schema`/`data` array: one leaf object per line."""
    pad = INDENT * depth
    inner = INDENT * (depth + 1)
    if not items:
        return "[]"
    lines = [f"{inner}{leaf(item)}" for item in items]
    return "[\n" + ",\n".join(lines) + f"\n{pad}]"


def emit_table(table: dict, depth: int) -> str:
    """Render one table object: Description scalar + schema/data leaf arrays."""
    pad = INDENT * depth
    inner = INDENT * (depth + 1)
    parts = []
    for key, value in table.items():
        if key in ("schema", "data"):
            rendered = emit_array_of_leaves(value, depth + 1)
            parts.append(f'{inner}{json.dumps(key)}: {rendered}')
        else:
            parts.append(f'{inner}{json.dumps(key)}: {leaf(value)}')
    return "{\n" + ",\n".join(parts) + f"\n{pad}}}"


def format_rulebook(rb: dict) -> str:
    top_parts = []
    for key, value in rb.items():
        if key in NON_TABLE_KEYS:
            top_parts.append(f'{INDENT}{json.dumps(key)}: {leaf(value)}')
            continue
        if not (isinstance(value, dict) and {"schema", "data"} <= set(value)):
            raise ValueError(
                f'Top-level table "{key}" is not a Description/schema/data object; '
                f'refusing to guess its shape. Migrate the rulebook first.'
            )
        top_parts.append(f'{INDENT}{json.dumps(key)}: {emit_table(value, 1)}')
    return "{\n" + ",\n".join(top_parts) + "\n}\n"


def main(argv: list[str]) -> int:
    path = Path(argv[1]).resolve() if len(argv) > 1 else DEFAULT
    if not path.exists():
        raise FileNotFoundError(f"Rulebook does not exist: {path}")
    original = path.read_text(encoding="utf-8")
    rb = json.loads(original)  # raises loudly on invalid JSON
    formatted = format_rulebook(rb)
    # Sanity: the formatted text must parse back to an identical object.
    if json.loads(formatted) != rb:
        raise AssertionError("Formatter changed rulebook semantics; aborting.")
    path.write_text(formatted, encoding="utf-8")
    changed = formatted != original
    print(f"{'formatted' if changed else 'already normalized'}: {path}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
