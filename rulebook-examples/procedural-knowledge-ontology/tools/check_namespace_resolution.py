#!/usr/bin/env python3
"""Fetch every ontology namespace this model depends on and record what came back.

The knowledge engineer's question q10-ke-publication asks whether every term is published
under a *resolvable* identifier. Until now that was answered syntactically -- does the IRI
start with "http" -- which a urn fails and a dead http URL passes. This measures it instead:
each `OntologyProfiles.NamespaceIri` is fetched, with an RDF Accept header, and the HTTP
status and whether RDF came back are written onto the profile row as raw measurements. The
rulebook's own formulas turn those into `NamespaceDereferences` and
`PublishesFollowingLinkedDataPrinciples`.

The measurement is the substrate of the answer; nothing here decides whether a profile passes.

No fallbacks. A connection failure is recorded as status 0 -- that IS the measurement, the
namespace did not resolve -- but if nearly everything fails the run aborts instead of writing
a wall of zeroes that would look like a finding and actually be a dead network.

    tools/check_namespace_resolution.py
    tools/check_namespace_resolution.py --dry-run
"""
from __future__ import annotations

import argparse
import datetime as dt
import json
import ssl
import urllib.error
import urllib.request
from collections import OrderedDict
from pathlib import Path

import certifi

# This interpreter has no OpenSSL trust store of its own (the python.org build leaves
# ssl.get_default_verify_paths() pointing at a cert.pem it never installs), so every HTTPS
# fetch raised CERTIFICATE_VERIFY_FAILED and was recorded as "did not resolve" -- 26 of 27
# vocabularies would have been reported dead when the only dead thing was the trust store.
# Name the bundle explicitly. If certifi is absent the import above fails loudly, which is
# the correct outcome: an unverified fetch is not a measurement of anything.
SSL_CONTEXT = ssl.create_default_context(cafile=certifi.where())

RULEBOOK = (Path(__file__).resolve().parent.parent / "effortless-rulebook"
            / "procedural-knowledge-ontology-rulebook.json")

ACCEPT = ("text/turtle;q=1.0, application/rdf+xml;q=0.9, application/ld+json;q=0.9, "
          "text/n3;q=0.8, application/n-triples;q=0.8, text/html;q=0.5, */*;q=0.1")
RDF_CONTENT_TYPES = ("text/turtle", "application/rdf+xml", "application/ld+json", "text/n3",
                     "application/n-triples", "application/trig", "text/rdf+n3", "application/owl+xml")
# An HTML document that carries a JSON-LD block has served RDF to the machine that asked.
EMBEDDED_RDF_MARKERS = ('application/ld+json',)
TIMEOUT = 20


def fetch(url: str) -> tuple[int, bool, str]:
    """Return (http_status, served_rdf, note). Status 0 means the fetch itself failed."""
    req = urllib.request.Request(url, headers={
        "Accept": ACCEPT,
        "User-Agent": "effortless-rulebooks-namespace-check/1.0 (+https://github.com/EffortlessAPI/effortless-rulebooks)",
    })
    try:
        with urllib.request.urlopen(req, timeout=TIMEOUT, context=SSL_CONTEXT) as resp:
            status = resp.status
            ctype = (resp.headers.get("Content-Type") or "").split(";")[0].strip().lower()
            body = resp.read(400_000)
    except urllib.error.HTTPError as exc:
        return exc.code, False, f"HTTP {exc.code}"
    except Exception as exc:  # noqa: BLE001 -- the failure itself is the measurement
        return 0, False, f"{type(exc).__name__}: {exc}"

    if ctype in RDF_CONTENT_TYPES:
        return status, True, ctype
    text = body.decode("utf-8", errors="replace")
    if any(marker in text for marker in EMBEDDED_RDF_MARKERS):
        return status, True, f"{ctype} with embedded JSON-LD"
    return status, False, ctype or "no content-type"


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    rb = json.loads(RULEBOOK.read_text(), object_pairs_hook=OrderedDict)
    profiles = rb["OntologyProfiles"]["data"]
    checked_at = dt.datetime.now(dt.timezone.utc).replace(microsecond=0).isoformat()

    results: list[tuple[str, int, bool, str]] = []
    attempted = 0
    for row in profiles:
        ns = (row.get("NamespaceIri") or "").strip()
        if not ns.lower().startswith(("http://", "https://")):
            # Not an HTTP URI at all: there is nothing to fetch, and the rulebook's
            # NamespaceIsHttp already says so. Record the absence, do not invent a status.
            results.append((row["OntologyProfileId"], 0, False, "not an HTTP URI"))
            row["NamespaceCheckedAt"] = checked_at
            row["NamespaceHttpStatus"] = 0
            row["NamespaceServesRdf"] = False
            continue
        attempted += 1
        status, rdf, note = fetch(ns)
        results.append((row["OntologyProfileId"], status, rdf, note))
        row["NamespaceCheckedAt"] = checked_at
        row["NamespaceHttpStatus"] = status
        row["NamespaceServesRdf"] = rdf

    failures = sum(1 for _, status, _, _ in results if status == 0) - (len(results) - attempted)
    if attempted and failures >= attempted * 0.8:
        raise SystemExit(
            f"{failures} of {attempted} namespace fetches failed outright. That is far more likely to be "
            f"this machine's network than {failures} dead vocabularies; refusing to write the results. "
            f"Check connectivity and re-run.")

    width = max(len(r[0]) for r in results)
    for pid, status, rdf, note in sorted(results):
        print(f"  {pid:{width}s}  {status:3d}  rdf={'yes' if rdf else 'no ':3s}  {note}")
    ok = sum(1 for _, s, r, _ in results if s == 200 and r)
    print(f"==> {len(results)} profiles; {ok} resolve and serve RDF; {failures} did not resolve at all")

    if args.dry_run:
        print("dry run -- nothing written")
        return 0

    # Re-read immediately before writing; insert only these three columns.
    fresh = json.loads(RULEBOOK.read_text(), object_pairs_hook=OrderedDict)
    measured = {r["OntologyProfileId"]: r for r in profiles}
    for row in fresh["OntologyProfiles"]["data"]:
        src = measured.get(row["OntologyProfileId"])
        if src is None:
            continue
        for col in ("NamespaceCheckedAt", "NamespaceHttpStatus", "NamespaceServesRdf"):
            row[col] = src[col]
    RULEBOOK.write_text(json.dumps(fresh, indent=1, ensure_ascii=False) + "\n")
    print(f"written to {RULEBOOK.name}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
