#!/usr/bin/env python3
"""Project article coverage from Postgres into generated/article-coverage.md.

Every number and verdict is read from vw_source_articles, vw_article_claims and
vw_claim_evidence; nothing is recomputed here. The document exists so a skeptic can read
each claim beside the evidence offered for it, the justification, and the computed
verdict, and disagree with a specific link.

Usage: tools/project_article_coverage.py [-o generated/article-coverage.md]
"""
from __future__ import annotations

import argparse
import json
import os
import subprocess
from pathlib import Path

DB = os.environ.get("PGDATABASE") or "erb_procedural_knowledge_ontology"


def rows(sql: str) -> list[dict]:
    out = subprocess.run(["psql", "-X", "-qtA", "-d", DB, "-c", f"SELECT coalesce(json_agg(t), '[]') FROM ({sql}) t"],
                         capture_output=True, text=True)
    if out.returncode != 0:
        raise SystemExit(f"psql failed: {out.stderr}")
    return json.loads(out.stdout)


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("-o", "--output", default="generated/article-coverage.md")
    args = ap.parse_args()

    articles = rows("SELECT source_article_id, title, claim_count, covered_claim_count, uncovered_claim_count, "
                    "coverage_percent, agreed_coverage_percent FROM vw_source_articles ORDER BY source_article_id")
    claims = rows("SELECT article_claim_id, source_article, claim_kind, section_ref, claim_text, required_evidence, "
                  "is_covered, is_agreed, evidence_count FROM vw_article_claims ORDER BY article_claim_id")
    evidence = rows("SELECT article_claim, evidence_kind, coalesce(rulebook_field, rulebook_table, role_question, ontology_profile, "
                    "knowledge_method, procedure) AS target, justification, is_valid, is_contested FROM vw_claim_evidence "
                    "ORDER BY claim_evidence_id")
    by_claim: dict[str, list] = {}
    for e in evidence:
        by_claim.setdefault(e["article_claim"], []).append(e)

    total = sum(a["claim_count"] for a in articles)
    covered = sum(a["covered_claim_count"] for a in articles)
    out = ["# Article coverage", "",
           "Generated from Postgres (`vw_source_articles`, `vw_article_claims`, `vw_claim_evidence`). "
           "Do not edit; edit `tools/article_claims.py` or a theme's `tools/loops/evidence_*.py` and rebuild.", "",
           f"**{covered} of {total} claims covered ({round(100 * covered / total, 1) if total else 0}%).**", "",
           "| Article | Claims | Covered | Coverage | Agreed by every graded substrate |", "|---|---|---|---|---|"]
    for a in articles:
        out.append(f"| {a['source_article_id']}: {a['title']} | {a['claim_count']} | {a['covered_claim_count']} | "
                   f"{a['coverage_percent']}% | {a['agreed_coverage_percent']}% |")
    for a in articles:
        mine = [c for c in claims if c["source_article"] == a["source_article_id"]]
        out += ["", f"## {a['source_article_id']}: {a['title']}", ""]
        uncovered = [c for c in mine if not c["is_covered"]]
        if uncovered:
            out += [f"### Not covered ({len(uncovered)})", ""]
            for c in uncovered:
                out.append(f"- **{c['article_claim_id']}** ({c['claim_kind']}, {c['section_ref']}): {c['claim_text']} "
                           f"Needs {c['required_evidence']}.")
                for e in by_claim.get(c["article_claim_id"], []):
                    out.append(f"  - rejected {e['evidence_kind']} `{e['target']}`: {e['justification']}")
        out += ["", f"### Covered ({len(mine) - len(uncovered)})", ""]
        for c in mine:
            if not c["is_covered"]:
                continue
            out.append(f"- **{c['article_claim_id']}** ({c['claim_kind']}, {c['section_ref']}): {c['claim_text']}")
            for e in by_claim.get(c["article_claim_id"], []):
                verdict = "valid" if e["is_valid"] else "rejected"
                contested = ", disputed by a graded substrate" if e["is_contested"] else ""
                out.append(f"  - {verdict}{contested}: {e['evidence_kind']} `{e['target']}`. {e['justification']}")
    Path(args.output).write_text("\n".join(out) + "\n")
    print(f"wrote {args.output}: {covered}/{total} covered")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
