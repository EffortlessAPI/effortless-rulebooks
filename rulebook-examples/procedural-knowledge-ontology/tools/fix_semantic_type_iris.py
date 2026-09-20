#!/usr/bin/env python3
"""Repair two kinds of malformed `SemanticTypeIri` the model could detect about itself.

**A prefixed name is not an IRI.** Every `Agents` row was typed `prov:Agent`,
`prov:SoftwareAgent` or `prov:Organization`. A CURIE only means something next to a prefix
declaration, and there is none in a data column; exported as RDF it is a relative IRI
resolved against the document, not PROV-O at all. The prefix is expanded from the PROV-O
profile's own `NamespaceIri` in this rulebook -- not from a table of prefixes invented here.

**An instance is not an instance of a property.** Five reified relationship tables typed
their rows with the property IRI the relationship corresponds to -- `StepResources` rows were
`dcterms:references`, `ProcedureAdoptions` rows were `pko:isAdoptedBy`. Those IRIs denote
properties, and this model says so itself: each is the target of a `MappingKind: property`
row in `SemanticMappings`. A row of a junction table is an individual standing for the
relationship, so it is typed with the class for that relationship and the property mapping
stays where it belongs, on the property.

The class name is taken from the table's own primary key (`<Entity>Id` -> `Entity`), never by
stripping a trailing "s" -- that would turn `StepProtectiveEquipment` into
`StepProtectiveEquipmen`.

Run `tools/align_extension_class_iris.py` afterwards to declare the classes this introduces.

Idempotent. Re-reads immediately before writing; indent=1 / ensure_ascii=False.
"""
from __future__ import annotations

import argparse
import sys
from collections import OrderedDict, Counter
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from rulebook_edit import EXT, dump, load  # noqa: E402

CURIE_PROFILE = {"prov": "prov-o"}  # prefix -> the OntologyProfiles row that defines its namespace

# Properties this model uses but never declared as properties itself, so the self-check below
# cannot recognise them. Each is a property in the standard that defines it:
#   skos:semanticRelation -- owl:ObjectProperty in SKOS, the superproperty of related/broader.
EXTERNAL_PROPERTY_IRIS = {
    "http://www.w3.org/2004/02/skos/core#semanticRelation",
}


def namespace_of(rb, profile_id: str) -> str:
    row = next((r for r in rb["OntologyProfiles"]["data"]
                if r.get("OntologyProfileId") == profile_id), None)
    if row is None or not row.get("NamespaceIri"):
        raise SystemExit(f"OntologyProfiles row {profile_id!r} is missing or has no NamespaceIri; "
                         f"cannot expand its prefix.")
    return row["NamespaceIri"]


def class_name(rb, table: str) -> str:
    pk = rb[table]["schema"][0]["name"]
    if not pk.endswith("Id"):
        raise SystemExit(f"{table}: leading field {pk!r} is not an <Entity>Id; cannot name its class.")
    return pk[:-2]


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    rb = load()
    tables = [k for k, v in rb.items() if isinstance(v, dict) and "schema" in v]

    # StepResources is the one reified relationship with no property mapping at all: its rows
    # carried dcterms:references as their type and nothing recorded it as the property. Record
    # it BEFORE the scan, so the scan recognises that IRI as a property like the others.
    mappings = rb["SemanticMappings"]["data"]
    ids = {r.get("SemanticMappingId") for r in mappings}
    added_mapping = "map-StepResources-references" not in ids
    if added_mapping:
        mappings.append(OrderedDict([
            ("SemanticMappingId", "map-StepResources-references"), ("SourcePath", "StepResources"),
            ("MappingKind", "property"), ("TargetIri", "http://purl.org/dc/terms/references"),
            ("MappingRelation", "aligned"), ("OntologyProfile", "dcterms"),
            ("Notes", "The relationship a StepResource reifies: the step references the resource. "
                      "Recorded as a property, where it belongs; the row itself is a StepResource."),
        ]))

    property_iris = {r.get("TargetIri") for r in mappings
                     if r.get("MappingKind") in ("property", "objectProperty", "datatypeProperty")}
    property_iris |= EXTERNAL_PROPERTY_IRIS

    expanded, retyped = Counter(), Counter()
    for table in tables:
        for row in rb[table].get("data", []):
            iri = row.get("SemanticTypeIri")
            if not isinstance(iri, str) or not iri:
                continue
            if not iri.startswith(("http://", "https://", "urn:")) and ":" in iri:
                prefix, local = iri.split(":", 1)
                if prefix not in CURIE_PROFILE:
                    raise SystemExit(f"{table}: row typed {iri!r} uses prefix {prefix!r}, which no "
                                     f"OntologyProfiles row is registered for here.")
                row["SemanticTypeIri"] = namespace_of(rb, CURIE_PROFILE[prefix]) + local
                expanded[f"{table}: {iri}"] += 1
                continue
            if iri in property_iris:
                row["SemanticTypeIri"] = f"{EXT}{class_name(rb, table)}"
                retyped[f"{table}: {iri} -> {class_name(rb, table)}"] += 1

    # Nothing property-shaped may survive. A class IRI's local name is capitalised by universal
    # RDF convention; anything left lowercase is either a property this pass did not recognise or
    # a genuine naming mistake, and either way it must be looked at rather than shipped.
    leftover = sorted({
        f"{table}: {row['SemanticTypeIri']}"
        for table in tables for row in rb[table].get("data", [])
        if isinstance(row.get("SemanticTypeIri"), str) and row["SemanticTypeIri"]
        and (lambda lo: lo and lo[0].islower())(
            row["SemanticTypeIri"].rsplit("#", 1)[-1].rsplit("/", 1)[-1])
    })
    if leftover:
        raise SystemExit("instances still typed with a property-shaped IRI; add each to "
                         "EXTERNAL_PROPERTY_IRIS or fix the model:\n  " + "\n  ".join(leftover))

    print("== prefixed names expanded to absolute IRIs ==")
    for k, n in sorted(expanded.items()):
        print(f"  {k}  x{n}")
    print("== instances retyped off a property IRI onto their own class ==")
    for k, n in sorted(retyped.items()):
        print(f"  {k}  x{n}")
    print(f"== StepResources property mapping: {'added' if added_mapping else 'already present'}")

    if args.dry_run:
        print("dry run -- nothing written")
        return 0
    if not expanded and not retyped and not added_mapping:
        print("nothing to do")
        return 0
    dump(rb)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
