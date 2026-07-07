#!/usr/bin/env python3
"""Merge data/acquisition/expansion-wave-1-manifest.json into CandidateStudyCatalog."""

from __future__ import annotations

import json
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
RULEBOOK = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"
MANIFEST = ROOT / "data" / "acquisition" / "expansion-wave-1-manifest.json"

STATUS_MAP = {
    "downloaded": "downloaded",
    "portal_manual": "manual_only",
    "landing_saved": "manual_only",
    "metadata_only": "manual_only",
    "manual_only": "manual_only",
    "blocked": "blocked",
}


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


def main() -> None:
    if not MANIFEST.exists():
        raise SystemExit(f"Missing manifest: {MANIFEST}")

    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    by_candidate = {s["candidate_id"]: s for s in manifest["studies"]}
    rb = json.loads(RULEBOOK.read_text(encoding="utf-8"))

    updated = 0
    missing = []
    for row in rb["CandidateStudyCatalog"]["data"]:
        entry = by_candidate.get(row["CandidateId"])
        if not entry:
            continue
        new_status = STATUS_MAP.get(entry["acquisition_status"])
        if not new_status:
            raise SystemExit(
                f"Unknown acquisition_status {entry['acquisition_status']!r} for {row['CandidateId']}"
            )
        if row.get("DataAcquisitionStatus") != new_status:
            row["DataAcquisitionStatus"] = new_status
            updated += 1

    manifest_ids = set(by_candidate)
    catalog_expansion = {
        r["CandidateId"]
        for r in rb["CandidateStudyCatalog"]["data"]
        if r.get("ExpansionWave") == "expansion-wave-1"
    }
    missing = sorted(manifest_ids - catalog_expansion)

    RULEBOOK.write_text(compact_json(rb), encoding="utf-8")

    manifest["merged_at"] = datetime.now(timezone.utc).replace(microsecond=0).isoformat()
    manifest["note"] = "Merged into CandidateStudyCatalog.DataAcquisitionStatus after loops 68–72."
    MANIFEST.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    print(f"Updated DataAcquisitionStatus on {updated} catalog rows.")
    if missing:
        print(f"Warning: {len(missing)} manifest rows not in catalog: {missing[:5]}...")


if __name__ == "__main__":
    main()
