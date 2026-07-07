#!/usr/bin/env python3
"""Merge PDF table extraction side-loop artifacts into CandidateStudyCatalog.

Updates ONLY catalog metadata (DataAcquisitionStatus, DataSourceNote, SourceUrl)
for the 10 studies in data/extraction/pdf-table-extraction-manifest.json.
Does NOT encode CaseCells or touch Studies — safe parallel to loops 70–72.
"""

from __future__ import annotations

import json
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
RULEBOOK = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"
MANIFEST = ROOT / "data" / "extraction" / "pdf-table-extraction-manifest.json"
RAW = ROOT / "data" / "raw"

PDF_NOTE_PREFIX = "PDF side-loop:"


def compact_json(obj: dict) -> str:
    lines = ["{"]
    top_keys = list(obj.keys())
    for ki, key in enumerate(top_keys):
        val = obj[key]
        trail = "," if ki < len(top_keys) - 1 else ""
        if isinstance(val, dict) and "data" in val:
            head = json.dumps({k: v for k, v in val.items() if k != "data"}, indent=2, ensure_ascii=False)
            if head.endswith("\n}"):
                head = head[:-2]
            elif head.endswith("}"):
                head = head[:-1]
            data_rows = ",\n".join("      " + json.dumps(r, ensure_ascii=False) for r in val["data"])
            block = f'  "{key}": {head},\n    "data": [\n{data_rows}\n    ]\n  }}'
            lines.append(block + trail)
        else:
            chunk = json.dumps(val, indent=2, ensure_ascii=False)
            lines.append(f'  "{key}": {chunk}{trail}')
    lines.append("}")
    return "\n".join(lines) + "\n"


def build_pdf_note(study_id: str, source_url: str, table_count: int) -> str:
    rel = f"data/raw/{study_id}/table-extract.json"
    return (
        f"{PDF_NOTE_PREFIX} verified manuscript PDF; "
        f"{table_count} table(s) in `{rel}`; "
        f"manuscript={source_url}"
    )


def merge_note(existing: str, pdf_note: str) -> str:
    if PDF_NOTE_PREFIX in existing:
        before = existing.split(PDF_NOTE_PREFIX)[0].rstrip(" |")
        return f"{before} | {pdf_note}" if before else pdf_note
    return f"{existing} | {pdf_note}" if existing else pdf_note


def main() -> None:
    if not MANIFEST.exists():
        raise SystemExit(f"Missing extraction manifest: {MANIFEST}")

    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    by_study = {s["study_id"]: s for s in manifest.get("studies", [])}
    if not by_study:
        raise SystemExit("Extraction manifest has no studies — run pdf-table-extraction-side-loop.py first")

    rb = json.loads(RULEBOOK.read_text(encoding="utf-8"))
    updated = 0
    missing = []

    for row in rb["CandidateStudyCatalog"]["data"]:
        study_id = row.get("ProposedStudyId")
        entry = by_study.get(study_id)
        if not entry:
            continue

        extract_path = RAW / study_id / "table-extract.json"
        if not extract_path.exists():
            missing.append(study_id)
            continue

        extract = json.loads(extract_path.read_text(encoding="utf-8"))
        source_url = entry.get("manuscript_source_url") or extract.get("manuscript_source_url", "")
        table_count = entry.get("table_count") or len(extract.get("tables") or [])
        pdf_note = build_pdf_note(study_id, source_url, table_count)

        changed = False
        if row.get("DataAcquisitionStatus") != "downloaded":
            row["DataAcquisitionStatus"] = "downloaded"
            changed = True

        new_note = merge_note(row.get("DataSourceNote") or "", pdf_note)
        if row.get("DataSourceNote") != new_note:
            row["DataSourceNote"] = new_note
            changed = True

        if source_url and row.get("SourceUrl") != source_url:
            row["SourceUrl"] = source_url
            changed = True

        if changed:
            updated += 1

    if missing:
        raise SystemExit(f"Missing table-extract.json for: {missing}")

    RULEBOOK.write_text(compact_json(rb), encoding="utf-8")

    manifest["merged_at"] = datetime.now(timezone.utc).replace(microsecond=0).isoformat()
    manifest["merge_note"] = (
        "Merged into CandidateStudyCatalog (DataAcquisitionStatus, DataSourceNote, SourceUrl). "
        "CaseCells encode deferred — no overlap with loop-71 criminal-justice batch."
    )
    MANIFEST.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    print(f"Updated {updated} catalog rows from PDF extraction manifest ({len(by_study)} studies).")
    print("Rulebook catalog only — Studies/CaseCells untouched.")


if __name__ == "__main__":
    main()
