#!/usr/bin/env python3
"""Loop-81: identity witness infrastructure — refresh IdentityClusterSummaries + close loop-80 hypotheses."""
from __future__ import annotations

import json
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"


import re


def compact_data_rows(rb: dict) -> str:
    """Compact leaf data arrays to one object per line (with valid JSON commas)."""
    pretty = json.dumps(rb, indent=2, ensure_ascii=False)
    lines = pretty.splitlines()
    out: list[str] = []
    in_data = False
    depth = 0
    buf: list[str] = []

    for line in lines:
        if re.match(r'^\s+"data": \[\s*$', line):
            in_data = True
            depth = 0
            buf = []
            out.append(line)
            continue
        if in_data:
            stripped = line.strip()
            if stripped == "{":
                buf = [stripped]
                depth = 1
            elif buf:
                buf.append(stripped)
                depth += stripped.count("{") - stripped.count("}")
                if depth <= 0 and buf:
                    row = " ".join(buf).rstrip(",")
                    out.append("      " + row + ",")
                    buf = []
            elif stripped.startswith("{"):
                row = stripped.rstrip(",")
                out.append("      " + row + ",")
            elif re.match(r"^\s+\],?\s*$", line):
                in_data = False
                if out and out[-1].endswith(","):
                    out[-1] = out[-1][:-1]
                out.append(line)
            continue
        out.append(line)

    text = "\n".join(out) + "\n"
    json.loads(text)
    return text


def main() -> None:
    rb = json.loads(RB_PATH.read_text())

    loops = rb["Loops"]["data"]
    loop_80 = next(l for l in loops if l["LoopId"] == "loop-80")
    loop_80["MockDataNote"] = (
        "Ontology live at N=238: 15 identities, 238 identity maps, 233 real stratum variables. "
        "Cluster witness SQL deferred to loop-81."
    )
    loop_80["NextSuggestion"] = "loop-81: identity witness infrastructure"

    if not any(l["LoopId"] == "loop-81" for l in loops):
        idx = next(i for i, l in enumerate(loops) if l["LoopId"] == "loop-80")
        loops.insert(idx + 1, {
            "LoopId": "loop-81",
            "Title": "Identity witness infrastructure — refresh IdentityClusterSummaries + close loop-80 hypotheses",
            "Status": "complete",
            "NewConcept": (
                "refresh_identity_cluster_summaries() in 05b-customize-data.sql; "
                "ModelSummary identity rollup fields (AgeIdentityManifestFlipRate, AgeIdentityStudyCount, "
                "AgeIdentityLatentFractionAmongTypeD, SeverityIdentityLatentFractionAmongTypeD, "
                "IdentityMapCoverageRate); calc_discovery_findings branches for loop-80 hypotheses; "
                "extend inv-theorem-consistency-confirmed with H-identity-map-coverage; witness conc-33."
            ),
            "DomainQuestion": (
                "When cluster summaries are populated from live joins, do loop-80 hypotheses witness correctly — "
                "and does H-severity-vs-age-latent fail as ad-hoc SQL predicted?"
            ),
            "MockDataNote": "Pending post-build witness.",
            "NextSuggestion": "loop-82: supersede refuted H-severity-vs-age-latent with identity×domain cells",
            "TraditionId": "tradition-dag",
        })

    ms_schema = rb["ModelSummary"]["schema"]
    names = {f["name"] for f in ms_schema}
    extra_ms = [
        {
            "name": "AgeIdentityManifestFlipRate",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Manifest flip rate for id-age-composition cluster (loop-81).",
            "formula": '=LOOKUP("id-age-composition", IdentityClusterSummaries[ConfounderIdentity], IdentityClusterSummaries[ManifestFlipRate])',
        },
        {
            "name": "AgeIdentityStudyCount",
            "datatype": "integer",
            "type": "calculated",
            "nullable": True,
            "Description": "Real-study count in id-age-composition cluster (loop-81).",
            "formula": '=LOOKUP("id-age-composition", IdentityClusterSummaries[ConfounderIdentity], IdentityClusterSummaries[StudyCount])',
        },
        {
            "name": "AgeIdentityLatentFractionAmongTypeD",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Type-D latent fraction for id-age-composition cluster (loop-81).",
            "formula": '=LOOKUP("id-age-composition", IdentityClusterSummaries[ConfounderIdentity], IdentityClusterSummaries[LatentFractionAmongTypeD])',
        },
        {
            "name": "SeverityIdentityLatentFractionAmongTypeD",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Type-D latent fraction for id-disease-severity cluster (loop-81).",
            "formula": '=LOOKUP("id-disease-severity", IdentityClusterSummaries[ConfounderIdentity], IdentityClusterSummaries[LatentFractionAmongTypeD])',
        },
        {
            "name": "IdentityMapCoverageRate",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Fraction of real-study StratumVariables with a ConfounderIdentity map (loop-81 customize SQL).",
            "formula": "=IF({{RealStudyCount}}=0, \"\", {{MappedStratumVariableCount}} / {{RealStudyCount}})",
        },
    ]
    for field in extra_ms:
        if field["name"] not in names:
            ms_schema.append(field)

    for row in rb["InvariantChecks"]["data"]:
        if row["InvariantCheckId"] == "inv-theorem-consistency-confirmed":
            row["SqlFilter"] = (
                "hypothesis_id IN ('H-corrected-gap-invariant', 'H-explained-bidirectional', "
                "'H-collider-no-manifest-theorem', 'H-theorem-portfolio', 'H-identity-map-coverage')"
            )
            row["PassCount"] = 5
            row["NaturalLanguage"] = (
                "Theorem wave + identity consistency checks: CorrectedGap invariance, "
                "explained↔confounder biconditional, collider no-manifest, theorem portfolio, "
                "identity map coverage."
            )

    RB_PATH.write_text(compact_data_rows(rb), encoding="utf-8")
    print(f"Updated {RB_PATH}")


if __name__ == "__main__":
    main()
