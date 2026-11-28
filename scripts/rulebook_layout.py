#!/usr/bin/env python3
"""The rulebook's on-disk layout: standard JSON with single-line leaves.

Every table's `schema` and `data` arrays hold one field or one row per line;
everything around them (the top-level keys, each table's own keys) stays
pretty-printed. One changed row is one changed line in a diff, and
`grep '"ThingId": "stan"'` finds the whole row.

This is the one implementation in this repo. It writes, byte for byte, what the
generated rulebook editor's Save writes (`serializeRulebook` in the
rulebook-to-node-postgres-api tool), so a file saved by the editor and a file
saved here never flip each other's lines:

  {
    "$schema": "...",
    "Name": "...",
    "Things": {
      "Description": "...",
      "schema": [
        {"name": "ThingId", "datatype": "string", ...},
        ...
      ],
      "data": [
        {"ThingId": "stan", ...},
        ...
      ]
    }
  }

A top-level value that is not a table ($schema, Name, Description, _meta, or
anything without a `schema` array) is written as indented JSON. Numbers are
written exactly as they were read (1.0 stays 1.0, 2e15 stays 2e15), and text
is written as UTF-8, never escaped to \\uXXXX.

    python3 scripts/rulebook_layout.py <rulebook.json>...         rewrite each file in the layout
    python3 scripts/rulebook_layout.py --check <rulebook.json>... exit 1 if one is not in it
    python3 scripts/rulebook_layout.py -                          JSON on stdin, the layout on stdout

From Python: `format_document(doc)`, `format_text(text)`, `write_rulebook(doc, path)`.
"""

from __future__ import annotations

import json
import sys
from pathlib import Path

META_KEYS = {"$schema", "Name", "Description", "_meta"}


class Number(float):
    """A JSON number with the text it was written as, so writing it changes nothing."""

    def __new__(cls, text: str):
        number = super().__new__(cls, text)
        number.text = text
        return number


def parse(text: str):
    """JSON text as Python values, each number keeping its own text."""
    return json.loads(text, parse_float=Number)


def is_table(key: str, value) -> bool:
    return key not in META_KEYS and isinstance(value, dict) and isinstance(value.get("schema"), list)


def _scalar(value) -> str:
    if isinstance(value, Number):
        return value.text
    if value is True:
        return "true"
    if value is False:
        return "false"
    if value is None:
        return "null"
    if isinstance(value, int):
        return str(value)
    if isinstance(value, float):
        if value != value or value in (float("inf"), float("-inf")):
            raise ValueError(f"{value} is not a JSON number")
        return repr(int(value)) if value.is_integer() and abs(value) < 1e21 else repr(value)
    if isinstance(value, str):
        return json.dumps(value, ensure_ascii=False)
    raise TypeError(f"{type(value).__name__} is not a JSON value: {value!r}")


def one_line(value) -> str:
    """One value on one line, with ', ' and ': ' between its parts."""
    if isinstance(value, list):
        return "[" + ", ".join(one_line(v) for v in value) + "]"
    if isinstance(value, dict):
        return "{" + ", ".join(f"{json.dumps(k, ensure_ascii=False)}: {one_line(v)}" for k, v in value.items()) + "}"
    return _scalar(value)


def indented(value, depth: int = 0) -> str:
    """One value as two-space indented JSON, starting at the given depth."""
    pad, inner = "  " * depth, "  " * (depth + 1)
    if isinstance(value, list) and value:
        return "[\n" + ",\n".join(inner + indented(v, depth + 1) for v in value) + "\n" + pad + "]"
    if isinstance(value, dict) and value:
        return "{\n" + ",\n".join(f"{inner}{json.dumps(k, ensure_ascii=False)}: {indented(v, depth + 1)}"
                                   for k, v in value.items()) + "\n" + pad + "}"
    return one_line(value)


def format_document(doc: dict) -> str:
    """A rulebook as the text of its file."""
    if not isinstance(doc, dict):
        raise ValueError(f"A rulebook is a JSON object, not {type(doc).__name__}")
    entries = []
    for key, value in doc.items():
        name = json.dumps(key, ensure_ascii=False)
        if not is_table(key, value):
            entries.append(f"  {name}: {indented(value, 1)}")
            continue
        parts = []
        for part, inner in value.items():
            label = json.dumps(part, ensure_ascii=False)
            if part in ("schema", "data") and isinstance(inner, list) and inner:
                parts.append(f"    {label}: [\n" + ",\n".join("      " + one_line(row) for row in inner) + "\n    ]")
            else:
                parts.append(f"    {label}: {one_line(inner)}")
        entries.append(f"  {name}: {{\n" + ",\n".join(parts) + "\n  }")
    text = "{\n" + ",\n".join(entries) + "\n}\n"
    if json.loads(text) != json.loads(json.dumps(doc, ensure_ascii=False)):
        raise AssertionError("The layout changed what the rulebook says; nothing was written.")
    return text


def format_text(text: str) -> str:
    return format_document(parse(text))


def write_rulebook(doc: dict, path) -> None:
    Path(path).write_text(format_document(doc), encoding="utf-8")


def main(argv: list[str]) -> int:
    check = "--check" in argv
    paths = [a for a in argv if a != "--check"]
    if paths == ["-"] and not check:
        sys.stdout.write(format_text(sys.stdin.read()))
        return 0
    if not paths or "-" in paths:
        raise SystemExit(__doc__.split("\n\n")[-2])
    off = 0
    for arg in paths:
        path = Path(arg)
        original = path.read_text(encoding="utf-8")
        laid_out = format_text(original)
        if laid_out == original:
            print(f"in the layout: {path}")
        elif check:
            off += 1
            print(f"NOT in the layout (python3 scripts/rulebook_layout.py {path}): {path}")
        else:
            path.write_text(laid_out, encoding="utf-8")
            print(f"laid out: {path}")
    return 1 if off else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
