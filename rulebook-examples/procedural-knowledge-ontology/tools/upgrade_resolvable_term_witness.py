#!/usr/bin/env python3
"""Make SemanticMappings.IsNonResolvableTermIri measure resolvability instead of guessing it.

The knowledge engineer's question q10-ke-publication is "is every term published under a
*resolvable* identifier". The field invented to answer it tested the IRI's first four
characters for "http", which a urn fails and a long-dead http namespace passes -- and which
would read all-false the moment the extension namespace moved to https, stating nothing.

It now reads the profile's measured fetch result (tools/check_namespace_resolution.py), and
covers every mapping rather than only the extension ones: a term is non-resolvable if its IRI
is not an HTTP URI at all, or if the namespace it lives in did not answer when fetched. That
is strictly the question the knowledge engineer asked, answered by measurement.

Idempotent; re-reads the rulebook immediately before writing; indent=1 / ensure_ascii=False.
"""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from rulebook_edit import dump, ensure_fields, idx, load, lookup, set_provenance  # noqa: E402

QUESTION = "q10-ke-publication"
NEW_FORMULA = ('=OR(LEFT({{TargetIri}}, 4) <> "http", '
               'AND({{OntologyProfile}} <> "", {{ProfileNamespaceDereferences}} = FALSE))')
NEW_DESCRIPTION = (
    "The term cannot be looked up: either its IRI is not an HTTP URI, or the namespace it lives in "
    "did not answer when it was last fetched. Measured, not inferred from the IRI's spelling."
)


def main() -> int:
    rb = load()
    added = ensure_fields(rb, "SemanticMappings", [
        lookup("ProfileNamespaceDereferences", "boolean",
               "Whether the profile's namespace answered when it was last fetched.",
               idx("OntologyProfiles", "NamespaceDereferences", "OntologyProfile")),
    ], after="OntologyProfile")
    if added:
        set_provenance(rb, ["SemanticMappings.ProfileNamespaceDereferences"], QUESTION)

    field = next(f for f in rb["SemanticMappings"]["schema"] if f["name"] == "IsNonResolvableTermIri")
    changed = field.get("formula") != NEW_FORMULA
    field["formula"] = NEW_FORMULA
    field["Description"] = NEW_DESCRIPTION

    print(f"added: {added or 'nothing'}; IsNonResolvableTermIri formula "
          f"{'updated' if changed else 'already current'}")
    if not added and not changed:
        return 0
    dump(rb)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
