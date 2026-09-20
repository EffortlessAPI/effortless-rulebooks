#!/usr/bin/env python3
"""Make every extension class the instances use a class the vocabulary actually declares.

The extension vocabulary had grown two parallel naming conventions and nothing ever compared
them:

  * `SemanticMappings.TargetIri` was minted from the TABLE name, which is plural --
    `...#AbundantKnowledgeGaps` (apply_loop_spec.py emitted `EXT + table_name`);
  * every seeded row's `SemanticTypeIri` was authored by hand in the singular, the way
    ontologies name classes and the way PKO names its own -- `...#AbundantKnowledgeGap`.

So the class the TBox declared and the class the ABox used were different IRIs: 132 of the
160 extension class mappings declared a term nothing was ever an instance of, while dozens of
classes that instances *were* typed with were declared nowhere. In RDF that is an undeclared
class plus an unused one, and any reasoner reading the export would have said so.

Singular wins -- it is what the instances already use and what PKO does (`pko:Procedure`,
not `pko:Procedures`). The singular is never guessed here; it is read from the rows' own
`SemanticTypeIri`, so `Facilities` -> `Facility` and `Vocabularies` -> `Vocabulary` come out
right without an inflector.

Two rules, applied over terms rather than tables, because one table may legitimately use more
than one class -- `LifecycleStatuses` holds ten native `pko:` statuses and one seeded
`#LegacyStatus`, and that mixture is the point of the row:

  1. an extension class mapping whose table types all its extension rows with one term is
     retargeted to that term;
  2. every extension term used to type a row is declared by some mapping.

A table mapped `exact` to a native PKO class keeps that mapping; an extension term used
inside it is declared beside it rather than replacing it. An `aligned` mapping likewise says
this class corresponds to a standard class -- it does not say instances carry that IRI.

Idempotent. Re-reads immediately before writing; indent=1 / ensure_ascii=False.
"""
from __future__ import annotations

import argparse
import sys
from collections import OrderedDict, defaultdict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from rulebook_edit import EXT, dump, load  # noqa: E402


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    rb = load()
    tables = [k for k, v in rb.items() if isinstance(v, dict) and "schema" in v]
    mappings = rb["SemanticMappings"]["data"]

    # Which extension terms does each table's data actually use?
    used: dict[str, set[str]] = {}
    term_tables: dict[str, list[str]] = defaultdict(list)
    for table in tables:
        terms = {row.get("SemanticTypeIri") for row in rb[table].get("data", [])}
        terms = {t[len(EXT):] for t in terms if isinstance(t, str) and t.startswith(EXT)}
        if terms:
            used[table] = terms
            for term in terms:
                term_tables[term].append(table)

    class_maps: dict[str, list[dict]] = defaultdict(list)
    for row in mappings:
        if row.get("MappingKind") == "class":
            class_maps[row.get("SourcePath")].append(row)

    # Rule 1 -- retarget an extension class mapping onto the term its table's rows use.
    retargeted, already = [], 0
    for table, terms in used.items():
        ext_rows = [r for r in class_maps.get(table, []) if r.get("MappingRelation") == "extension"]
        if not ext_rows or len(terms) != 1:
            continue
        wanted = f"{EXT}{next(iter(terms))}"
        for row in ext_rows:
            if row.get("TargetIri") == wanted:
                already += 1
            else:
                retargeted.append((table, row["TargetIri"], wanted))
                row["TargetIri"] = wanted

    # Rule 2 -- declare every extension term that types a row and that nothing declares.
    declared_iris = {r.get("TargetIri") for r in mappings if r.get("MappingKind") == "class"}
    existing_ids = {r.get("SemanticMappingId") for r in mappings}
    declared = []
    for term in sorted(term_tables):
        iri = f"{EXT}{term}"
        if iri in declared_iris:
            continue
        home = sorted(term_tables[term])[0]
        beside = [r for r in class_maps.get(home, []) if r.get("MappingRelation") in ("exact", "aligned")]
        mid = f"map-{home}" if f"map-{home}" not in existing_ids else f"map-{home}-{term}"
        existing_ids.add(mid)
        declared_iris.add(iri)
        note = "Class its instances are typed with. Not part of PKO 2.0.0."
        if beside:
            note = (f"Class these instances are typed with, declared beside the "
                    f"{beside[0].get('MappingRelation')} mapping to "
                    f"{beside[0].get('TargetIri')}. Not part of PKO 2.0.0.")
        mappings.append(OrderedDict([
            ("SemanticMappingId", mid), ("SourcePath", home), ("MappingKind", "class"),
            ("TargetIri", iri), ("MappingRelation", "extension"),
            ("OntologyProfile", "erb-pko-extension-1.0.0"), ("Notes", note),
        ]))
        declared.append((home, term))

    # Rule 3 -- the same obligation for the classes that are NOT ours. A row typed
    # prov:SoftwareAgent or pko:ProcedureTarget states a class the mapping census never
    # recorded, so SemanticMappings was not the complete account of the model's semantics it
    # claims to be. These are real external classes, correctly used; they are declared with
    # the relation their namespace earns -- a PKO namespace is `exact`, any other reused
    # standard is `aligned` -- never as an extension.
    profiles = sorted(rb["OntologyProfiles"]["data"],
                      key=lambda p: -len(p.get("NamespaceIri") or ""))
    external: dict[str, list[str]] = defaultdict(list)
    for table in tables:
        for row in rb[table].get("data", []):
            iri = row.get("SemanticTypeIri")
            if isinstance(iri, str) and iri.startswith(("http://", "https://")) and not iri.startswith(EXT):
                if table not in external[iri]:
                    external[iri].append(table)

    declared_external = []
    for iri in sorted(external):
        if iri in declared_iris:
            continue
        profile = next((p for p in profiles
                        if (p.get("NamespaceIri") or "") and iri.startswith(p["NamespaceIri"])), None)
        if profile is None:
            raise SystemExit(f"{iri} types rows in {external[iri]} but falls under no OntologyProfiles "
                             f"namespace. Add the profile; do not guess the vocabulary it belongs to.")
        relation = "exact" if profile["OntologyProfileId"].startswith("pko-") else "aligned"
        home = sorted(external[iri])[0]
        term = iri.rsplit("#", 1)[-1].rsplit("/", 1)[-1]
        mid = f"map-{home}-{term}"
        if mid in existing_ids:
            mid = f"map-{home}-{term}-type"
        existing_ids.add(mid)
        declared_iris.add(iri)
        mappings.append(OrderedDict([
            ("SemanticMappingId", mid), ("SourcePath", home), ("MappingKind", "class"),
            ("TargetIri", iri), ("MappingRelation", relation),
            ("OntologyProfile", profile["OntologyProfileId"]),
            ("Notes", f"Class {', '.join(external[iri])} rows are typed with."),
        ]))
        declared_external.append((home, iri, relation))

    # Nothing may now declare a term no instance uses, or use a term nothing declares.
    used_iris = {f"{EXT}{t}" for t in term_tables}
    ext_declared = {r.get("TargetIri") for r in mappings
                    if r.get("MappingKind") == "class" and r.get("MappingRelation") == "extension"}
    orphan_declarations = sorted(ext_declared - used_iris)
    undeclared_uses = sorted(used_iris - ext_declared)

    print(f"already aligned    : {already}")
    print(f"retargeted         : {len(retargeted)}")
    for table, was, now in retargeted[:5]:
        print(f"    {table}: {was.rsplit('#', 1)[-1]} -> {now.rsplit('#', 1)[-1]}")
    if len(retargeted) > 5:
        print(f"    ... and {len(retargeted) - 5} more")
    print(f"newly declared     : {len(declared)}")
    for home, term in declared[:5]:
        print(f"    {term}  (from {home})")
    if len(declared) > 5:
        print(f"    ... and {len(declared) - 5} more")
    print(f"external declared  : {len(declared_external)}")
    for home, iri, relation in declared_external:
        print(f"    {relation:8s} {iri}  (from {home})")
    print(f"declared but unused: {len(orphan_declarations)}")
    for iri in orphan_declarations[:10]:
        print(f"    {iri.rsplit('#', 1)[-1]}")
    print(f"used but undeclared: {len(undeclared_uses)}")
    if undeclared_uses:
        raise SystemExit(f"still undeclared after the pass: {undeclared_uses[:5]}")

    if args.dry_run:
        print("dry run -- nothing written")
        return 0
    if not retargeted and not declared and not declared_external:
        print("nothing to do")
        return 0
    dump(rb)
    print(f"written: {len(mappings)} semantic mappings")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
