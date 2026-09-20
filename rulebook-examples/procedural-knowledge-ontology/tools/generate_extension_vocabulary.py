#!/usr/bin/env python3
"""Publish the extension vocabulary so every term this model mints can be looked up.

161 terms in this register are not defined by PKO 2.0.0. They carry IRIs in this project's
own namespace, and until those IRIs resolve they are private strings wearing the costume of
an IRI. This emits the document they resolve to.

Everything on the page is SELECTed from `vw_semantic_mappings`, `vw_ontology_profiles`,
`vw_rulebook_tables` and `vw_rulebook_fields`. Nothing is restated here and there is no
fallback: a missing view or column raises, naming what was expected.

The namespace is a hash namespace, so one document defines every term and
`https://<host>/ns/pko-extension` is what a client fetches. GitHub Pages cannot content
-negotiate, so the document is HTML carrying its own RDF as an embedded JSON-LD block --
which is how PKO itself publishes (`https://w3id.org/pko#` answers `text/html with embedded
JSON-LD`), and what `tools/check_namespace_resolution.py` measures. Turtle and JSON-LD are
written alongside it and linked as alternates.

    python3 tools/generate_extension_vocabulary.py --out <dir>
"""
from __future__ import annotations

import argparse
import datetime as dt
import html
import json
import os
from pathlib import Path

import psycopg2
import psycopg2.extras

PROFILE = "erb-pko-extension-1.0.0"
DB = os.environ.get("DATABASE_URL") or "postgresql://postgres@localhost:5432/erb_procedural_knowledge_ontology"


def rows(cur, sql, args=()):
    cur.execute(sql, args)
    return [dict(r) for r in cur.fetchall()]


def local_name(iri: str, namespace: str) -> str:
    if not iri.startswith(namespace):
        raise SystemExit(f"{iri!r} is not in the published namespace {namespace!r}; the vocabulary "
                         f"document would define a term it does not own")
    return iri[len(namespace):]


def collect():
    with psycopg2.connect(DB) as conn:
        with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cur:
            profile = rows(cur, "SELECT * FROM vw_ontology_profiles WHERE ontology_profile_id = %s",
                           (PROFILE,))
            if len(profile) != 1:
                raise SystemExit(f"expected exactly one {PROFILE} profile row, found {len(profile)}")
            profile = profile[0]
            terms = rows(cur, """
                SELECT semantic_mapping_id, source_path, mapping_kind, target_iri, notes,
                       available_standard_iri, reinvents_standard_term
                  FROM vw_semantic_mappings
                 WHERE ontology_profile = %s
                 ORDER BY target_iri
            """, (PROFILE,))
            tables = {r["table_name"]: r for r in rows(cur, """
                SELECT table_name, subject_area, field_count, measured_row_count, semantic_mapping_count
                  FROM vw_rulebook_tables
            """)}
            fields = {f"{r['target_table']}.{r['field_name']}": r for r in rows(cur, """
                SELECT target_table, field_name, datatype, field_type, related_to, formula
                  FROM vw_rulebook_fields
            """)}
    return profile, terms, tables, fields


def describe(term, tables, fields):
    """The prose for a term, drawn from the register rather than written here."""
    path = term["source_path"]
    parts = []
    if term["mapping_kind"] == "class":
        t = tables.get(path)
        if t is None:
            raise SystemExit(f"{term['semantic_mapping_id']}: no RulebookTables row for {path!r}")
        parts.append(f"One row of the {path} table in the {t['subject_area']} area of the register.")
        if t["measured_row_count"] is not None:
            parts.append(f"The register holds {t['measured_row_count']} of them, "
                         f"described by {t['field_count']} fields.")
    else:
        f = fields.get(path)
        if f is None:
            raise SystemExit(f"{term['semantic_mapping_id']}: no RulebookFields row for {path!r}")
        parts.append(f"The {f['field_name']} property of {f['target_table']}"
                     + (f", which names a {f['related_to']}." if f["related_to"] else f" ({f['datatype']})."))
    if term["notes"]:
        parts.append(term["notes"])
    return " ".join(parts)


def jsonld(profile, terms, tables, fields):
    ns = profile["namespace_iri"]
    graph = [{
        "@id": profile["version_iri"],
        "@type": "owl:Ontology",
        "rdfs:label": profile["name"],
        "owl:versionInfo": profile["version"],
        "dcterms:license": f"https://spdx.org/licenses/{profile['license']}",
        "dcterms:description": profile["scope"],
        "vann:preferredNamespaceUri": ns,
        "owl:imports": [{"@id": "https://w3id.org/pko"}, {"@id": "https://w3id.org/pko/industry"}],
    }]
    for t in terms:
        name = local_name(t["target_iri"], ns)
        node = {
            "@id": t["target_iri"],
            "@type": "owl:Class" if t["mapping_kind"] == "class" else "owl:ObjectProperty",
            "rdfs:label": name,
            "rdfs:comment": describe(t, tables, fields),
            "rdfs:isDefinedBy": {"@id": profile["version_iri"]},
        }
        # A term with a standard equivalent is published saying so, not quietly.
        if t["available_standard_iri"]:
            node["rdfs:seeAlso"] = {"@id": t["available_standard_iri"]}
        graph.append(node)
    return {
        "@context": {
            "owl": "http://www.w3.org/2002/07/owl#",
            "rdfs": "http://www.w3.org/2000/01/rdf-schema#",
            "dcterms": "http://purl.org/dc/terms/",
            "vann": "http://purl.org/vocab/vann/",
        },
        "@graph": graph,
    }


def turtle(doc, profile):
    def lit(s):
        return json.dumps(str(s), ensure_ascii=False)

    out = ["@prefix owl: <http://www.w3.org/2002/07/owl#> .",
           "@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .",
           "@prefix dcterms: <http://purl.org/dc/terms/> .",
           "@prefix vann: <http://purl.org/vocab/vann/> .", ""]
    for node in doc["@graph"]:
        out.append(f"<{node['@id']}> a {node['@type']} ;")
        body = []
        if "rdfs:label" in node:
            body.append(f"    rdfs:label {lit(node['rdfs:label'])}")
        for key in ("rdfs:comment", "dcterms:description", "owl:versionInfo",
                    "vann:preferredNamespaceUri"):
            if key in node:
                body.append(f"    {key} {lit(node[key])}")
        for key in ("rdfs:isDefinedBy", "rdfs:seeAlso", "dcterms:license"):
            val = node.get(key)
            if isinstance(val, dict):
                body.append(f"    {key} <{val['@id']}>")
            elif val:
                body.append(f"    {key} <{val}>")
        for imp in node.get("owl:imports", []):
            body.append(f"    owl:imports <{imp['@id']}>")
        out.append(" ;\n".join(body) + " .\n")
    return "\n".join(out)


def page(profile, terms, doc, tables, fields, built_at):
    ns = profile["namespace_iri"]
    e = html.escape
    classes = [t for t in terms if t["mapping_kind"] == "class"]
    props = [t for t in terms if t["mapping_kind"] != "class"]
    reinvented = [t for t in terms if t["reinvents_standard_term"]]

    def entry(t):
        name = local_name(t["target_iri"], ns)
        also = (f'<p class="also">Compare <a href="{e(t["available_standard_iri"])}">'
                f'{e(t["available_standard_iri"])}</a></p>' if t["available_standard_iri"] else "")
        return (f'<div class="term" id="{e(name)}">'
                f'<h3><a href="#{e(name)}">{e(name)}</a></h3>'
                f'<code>{e(t["target_iri"])}</code>'
                f'<p>{e(describe(t, tables, fields))}</p>{also}</div>')

    return f"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{e(profile['name'])}</title>
<link rel="alternate" type="text/turtle" href="pko-extension.ttl">
<link rel="alternate" type="application/ld+json" href="pko-extension.jsonld">
<script type="application/ld+json">
{json.dumps(doc, indent=1, ensure_ascii=False)}
</script>
<style>
 body {{ font: 16px/1.6 -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
        max-width: 54rem; margin: 0 auto; padding: 2rem 1.25rem 6rem; color: #1a1a1a; }}
 header {{ border-bottom: 1px solid #e3e3e3; padding-bottom: 1.5rem; margin-bottom: 2rem; }}
 h1 {{ font-size: 1.9rem; margin: 0 0 .35rem; }}
 .sub {{ color: #666; margin: 0; }}
 code {{ background: #f5f5f7; padding: .1rem .35rem; border-radius: 3px;
         font-size: .85em; word-break: break-all; }}
 .term {{ border-top: 1px solid #efefef; padding: 1rem 0; }}
 .term h3 {{ margin: 0 0 .3rem; font-size: 1.05rem; }}
 .term h3 a {{ color: #1a1a1a; text-decoration: none; }}
 .term h3 a:hover {{ color: #0b63ce; }}
 .term p {{ margin: .45rem 0 0; color: #333; }}
 .also {{ font-size: .9em; color: #666; }}
 .meta {{ background: #f8f9fb; border: 1px solid #e6e8ee; border-radius: 8px;
          padding: 1rem 1.25rem; margin: 1.5rem 0; }}
 .meta dl {{ display: grid; grid-template-columns: 12rem 1fr; gap: .4rem 1rem; margin: 0; }}
 .meta dt {{ color: #666; }} .meta dd {{ margin: 0; }}
 footer {{ margin-top: 3rem; padding-top: 1.5rem; border-top: 1px solid #e3e3e3;
           color: #666; font-size: .9em; }}
</style>
</head>
<body>
<header>
<h1>{e(profile['name'])}</h1>
<p class="sub">{e(profile['scope'])}</p>
</header>

<p>This document defines the {len(terms)} terms used by the Procedural Knowledge Ontology
register that <a href="https://w3id.org/pko/2.0.0">PKO 2.0.0</a> does not define. Terms PKO
does define are used directly under their own IRIs and are not repeated here. The
distinction between a native term, a reused external term and an extension is recorded as
data in the register's <code>SemanticMappings</code> table, and this page is generated from
it.</p>

<div class="meta"><dl>
<dt>Namespace</dt><dd><code>{e(ns)}</code></dd>
<dt>Version</dt><dd>{e(profile['version'])}</dd>
<dt>License</dt><dd>{e(profile['license'])}</dd>
<dt>Classes</dt><dd>{len(classes)}</dd>
<dt>Properties</dt><dd>{len(props)}</dd>
<dt>Machine-readable</dt><dd><a href="pko-extension.ttl">Turtle</a> &middot;
 <a href="pko-extension.jsonld">JSON-LD</a> &middot; embedded in this page as JSON-LD</dd>
</dl></div>

<p>{len(reinvented)} of these terms have a close equivalent in an external standard; each one
says which, so the choice to mint a local term stays visible rather than being settled by
the absence of a comparison.</p>

<h2>Classes ({len(classes)})</h2>
{''.join(entry(t) for t in classes)}

<h2>Properties ({len(props)})</h2>
{''.join(entry(t) for t in props)}

<footer>
<p>Generated from the register on {e(built_at)} by
<code>tools/generate_extension_vocabulary.py</code>. Do not edit; edit the rulebook and rebuild.</p>
<p>PKO was created by Valentina Anita Carriero, Mario Scrocca, Ilaria Baroni, Antonia Azzini
and Irene Celino (CC BY 4.0). This is an independent extension aligned to PKO; it is not an
official PKO distribution and implies no endorsement.</p>
</footer>
</body>
</html>
"""


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--out", required=True, help="directory the namespace resolves to")
    args = ap.parse_args()

    profile, terms, tables, fields = collect()
    built_at = dt.datetime.now(dt.timezone.utc).replace(microsecond=0).isoformat()
    doc = jsonld(profile, terms, tables, fields)

    out = Path(args.out)
    out.mkdir(parents=True, exist_ok=True)
    (out / "index.html").write_text(page(profile, terms, doc, tables, fields, built_at))
    (out / "pko-extension.jsonld").write_text(json.dumps(doc, indent=1, ensure_ascii=False) + "\n")
    (out / "pko-extension.ttl").write_text(turtle(doc, profile))
    print(f"{len(terms)} terms -> {out}/index.html (+ .ttl, .jsonld)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
