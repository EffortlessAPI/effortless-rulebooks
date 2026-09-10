#!/usr/bin/env python3
"""Detect (and optionally undo) ASCII-escaped rulebook JSON.

Python's json.dump() defaults to ensure_ascii=True, and .NET's System.Text.Json
defaults to an encoder that escapes the same way. Either one, run over a
rulebook, rewrites every non-ASCII character as a \\uXXXX escape: an em-dash
becomes \\u2014, a curly quote \\u2019. The JSON means exactly the same thing,
so every tool keeps working and nothing reports an error -- but the file comes
back with hundreds of changed lines, the real edit is buried in them, and the
next writer that DOES keep UTF-8 flips them all back. On 2026-09-12 a one-off
edit script rewrote all 480 such characters in the root rulebook this way.

Every checked-in writer here already passes ensure_ascii=False. This check
exists for the writer that isn't checked in.

    python3 scripts/check-rulebook-encoding.py            # exit 1 if any rulebook is escaped
    python3 scripts/check-rulebook-encoding.py --fix      # rewrite the escapes back to UTF-8

--fix is surgical: it replaces only the escape sequences themselves, so the
file's formatting, key order and every other byte are left exactly as they were.
Escapes that JSON requires (quotes, backslashes, control characters) are ASCII
and are never touched.
"""
import argparse
import json
import re
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent

# A \uXXXX escape for a code point >= U+0080, optionally followed by a low
# surrogate escape (astral characters such as emoji are written as a pair).
# The lookbehind requires an EVEN run of backslashes before the escape, so a
# literal backslash followed by "u2014" inside a string value is not mistaken
# for an escape.
ESCAPE = re.compile(
    r'(?<!\\)((?:\\\\)*)'
    r'\\u([0-9a-fA-F]{4})'
    r'(?:\\u([dD][c-fC-F][0-9a-fA-F]{2}))?'
)


def _decode(match):
    backslashes, first, low = match.group(1), int(match.group(2), 16), match.group(3)
    if first < 0x80:
        return match.group(0)                      # a required ASCII escape: keep it
    if 0xD800 <= first <= 0xDBFF and low:          # surrogate pair -> one astral character
        return backslashes + chr(0x10000 + ((first - 0xD800) << 10) + (int(low, 16) - 0xDC00))
    if 0xD800 <= first <= 0xDFFF:
        return match.group(0)                      # a lone surrogate is not valid UTF-8: keep it
    return backslashes + chr(first) + (f'\\u{low}' if low else '')


def rulebooks():
    """Every governed rulebook hub: the root plus each project's effortless-rulebook/*.json."""
    found = []
    for path in sorted(REPO_ROOT.glob('**/effortless-rulebook/*.json')):
        parts = set(path.relative_to(REPO_ROOT).parts)
        if parts & {'node_modules', '.git', 'docker'}:
            continue
        found.append(path)
    return found


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('paths', nargs='*', type=Path, help='rulebook files (default: every governed rulebook)')
    parser.add_argument('--fix', action='store_true', help='rewrite the escapes back to UTF-8 in place')
    args = parser.parse_args()

    targets = args.paths or rulebooks()
    escaped = 0

    for path in targets:
        text = path.read_text(encoding='utf-8')
        count = sum(1 for m in ESCAPE.finditer(text) if int(m.group(2), 16) >= 0x80)
        if not count:
            continue
        escaped += 1
        rel = path.resolve().relative_to(REPO_ROOT) if path.resolve().is_relative_to(REPO_ROOT) else path

        if not args.fix:
            print(f'ESCAPED  {rel}: {count} non-ASCII character(s) written as \\uXXXX')
            continue

        fixed = ESCAPE.sub(_decode, text)
        # The rewrite must not change what the document MEANS. If it would, that is
        # a bug in this script, and the file is left untouched.
        if json.loads(fixed) != json.loads(text):
            raise SystemExit(f'{rel}: decoding the escapes would change the parsed document -- refusing to write')
        path.write_text(fixed, encoding='utf-8')
        print(f'FIXED    {rel}: {count} escape(s) restored to UTF-8')

    if escaped and not args.fix:
        print(f'\n{escaped} rulebook(s) are ASCII-escaped. Whatever last wrote them serialized with '
              f'ensure_ascii=True (Python) or the default System.Text.Json encoder (.NET). '
              f'Run with --fix to restore UTF-8.', file=sys.stderr)
        return 1
    if not escaped:
        print(f'ok: {len(targets)} rulebook(s), none ASCII-escaped')
    return 0


if __name__ == '__main__':
    sys.exit(main())
