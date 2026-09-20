#!/usr/bin/env python3
"""Move the ERB-PKO extension vocabulary to a different namespace IRI.

Every concept this model needs that PKO 2.0.0 does not define is minted in one namespace and
carried on each row as `SemanticTypeIri`, and on each `SemanticMappings` row as `TargetIri`.
That namespace started as `urn:effortless:pko-extension#`, which is a name and not an address:
a urn cannot be looked up, so the terms were not published in the Linked Data sense at all
(the LOT methodology's publication step, and article claim pkm4-s13).

This moves it. It rewrites, in one pass:

  * the rulebook's data (~32k `SemanticTypeIri` / `TargetIri` values),
  * the `OntologyProfiles` row for the extension profile (NamespaceIri, VersionIri),
  * the hand-written source that mints the IRI (tools/, app/) and the prose that quotes it.

Generated trees are NOT touched -- they are rewritten by the next `effortless build`.

Idempotent: running it twice is a no-op. Safe: the rulebook is re-read immediately before
writing, is written back with indent=1 / ensure_ascii=False (the file's on-disk format), and
the rewrite is refused unless the parsed document is identical apart from the namespace
strings. Fails loudly; substitutes nothing.

    tools/set_extension_namespace.py --to https://example.org/ns/pko-extension#
    tools/set_extension_namespace.py --to ... --dry-run
"""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

PROJECT = Path(__file__).resolve().parent.parent
RULEBOOK = PROJECT / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json"
EXTENSION_PROFILE_ID = "erb-pko-extension-1.0.0"

# Hand-written trees only. Everything else under the project is emitted by a transpiler and is
# rewritten by the next build; rewriting it here would just create a diff the build discards.
SOURCE_GLOBS = [
    "tools/**/*.py",
    "app/backend/**/*.js",
    "app/frontend/src/**/*.js",
    "app/mobile/src/**/*.ts",
    "app/mobile/src/**/*.tsx",
    "schemas/*.json",
    "*.md",
]
SKIP_PARTS = {"node_modules", "dist", "build", "__pycache__", "generated", "bootstrap",
              "postgres-bootstrap", "testing", "rulespeak", "effortless-python", "effortless-golang",
              "effortless-typescript", "effortless-entity-framework", "effortless-owl", "effortless-xlsx"}


def current_namespace(rb: dict) -> str:
    """The namespace in force, read from the extension profile itself -- not guessed."""
    profiles = rb["OntologyProfiles"]["data"]
    row = next((r for r in profiles if r.get("OntologyProfileId") == EXTENSION_PROFILE_ID), None)
    if row is None:
        raise SystemExit(f"OntologyProfiles has no row {EXTENSION_PROFILE_ID!r}; cannot tell which "
                         f"namespace is in force.")
    ns = row.get("NamespaceIri")
    if not ns:
        raise SystemExit(f"{EXTENSION_PROFILE_ID}.NamespaceIri is empty; cannot tell which namespace "
                         f"is in force.")
    return ns


def source_files() -> list[Path]:
    me = Path(__file__).resolve()
    seen: list[Path] = []
    for pattern in SOURCE_GLOBS:
        for path in PROJECT.glob(pattern):
            # This script's own docstring quotes the original urn as history; rewriting it would
            # destroy the explanation of why the move happened.
            if path.resolve() == me:
                continue
            if path.is_file() and not (set(path.relative_to(PROJECT).parts) & SKIP_PARTS):
                seen.append(path)
    return sorted(set(seen))


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--to", required=True, help="the new namespace IRI, ending in # or /")
    ap.add_argument("--version-iri", help="VersionIri for the extension profile (default: --to without "
                                          "the trailing separator)")
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    new = args.to
    if not new.endswith(("#", "/")):
        raise SystemExit(f"--to must end in '#' or '/': {new!r}")

    rb = json.loads(RULEBOOK.read_text())
    old = current_namespace(rb)
    if old == new:
        print(f"already {new} -- nothing to do")
        return 0

    raw = RULEBOOK.read_text()
    occurrences = raw.count(old)
    if occurrences == 0:
        raise SystemExit(f"the extension profile says the namespace is {old!r} but no value in the "
                         f"rulebook uses it; refusing to guess.")

    rewritten = raw.replace(old, new)
    after = json.loads(rewritten)

    # The document must differ ONLY in those strings. Prove it by mapping the new namespace back.
    def unmap(node):
        if isinstance(node, str):
            return node.replace(new, old)
        if isinstance(node, list):
            return [unmap(x) for x in node]
        if isinstance(node, dict):
            return {k: unmap(v) for k, v in node.items()}
        return node

    if unmap(after) != rb:
        raise SystemExit("the rewrite would change the document beyond the namespace strings; refusing.")

    profile = next(r for r in after["OntologyProfiles"]["data"]
                   if r.get("OntologyProfileId") == EXTENSION_PROFILE_ID)
    profile["NamespaceIri"] = new
    profile["VersionIri"] = args.version_iri or new.rstrip("#/")

    touched = []
    for path in source_files():
        text = path.read_text(encoding="utf-8")
        if old in text:
            touched.append((path, text.count(old), text.replace(old, new)))

    print(f"{old}  ->  {new}")
    print(f"  rulebook: {occurrences} values, profile {EXTENSION_PROFILE_ID} namespace + version IRI")
    for path, count, _ in touched:
        print(f"  {path.relative_to(PROJECT)}: {count}")
    if args.dry_run:
        print("dry run -- nothing written")
        return 0

    # Re-read immediately before writing: the rulebook is a contended file.
    if RULEBOOK.read_text() != raw:
        raise SystemExit("the rulebook changed while this script was running; re-run it.")
    RULEBOOK.write_text(json.dumps(after, indent=1, ensure_ascii=False) + "\n")
    for path, _, text in touched:
        path.write_text(text, encoding="utf-8")
    print(f"written: rulebook + {len(touched)} source file(s)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
