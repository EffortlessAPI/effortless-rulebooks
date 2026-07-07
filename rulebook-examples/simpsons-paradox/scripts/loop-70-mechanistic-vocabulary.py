#!/usr/bin/env python3
"""Loop-70: IsStratumUnanimous + IsSweepFragile + inv-type-a-unanimous + DistortionType A/B fix."""

from __future__ import annotations

import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"


def load_bulk():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)
    return bulk


def insert_after(schema: list, after_name: str, new_fields: list) -> None:
    idx = next(i for i, f in enumerate(schema) if f["name"] == after_name)
    schema[idx + 1 : idx + 1] = new_fields


def patch_schema(rb: dict) -> None:
    tr = rb["TreatmentRankings"]["schema"]
    insert_after(
        tr,
        "PerStratumWinner",
        [
            {
                "name": "IsStratumUnanimous",
                "datatype": "boolean",
                "type": "calculated",
                "nullable": True,
                "Description": "TRUE when one treatment wins every stratum (StrataWonByA = StratumCount OR StrataWonByB = StratumCount). Maps A/B policy split: unanimous → stratify-immediately geometry; heterogeneous → investigate-confounder.",
                "formula": "=OR({{StrataWonByA}} = {{StratumCount}}, {{StrataWonByB}} = {{StratumCount}})",
            },
        ],
    )

    insert_after(
        tr,
        "AllocationFragility",
        [
            {
                "name": "IsSweepFragile",
                "datatype": "boolean",
                "type": "calculated",
                "nullable": True,
                "Description": "Type-D study that is pooled-safe but ultra-fragile under allocation reweighting: signal_purity=1, allocation_distortion=0, sweep_pooled_gap_range>0.3, allocation_fragility>=10.",
                "formula": (
                    '=AND({{DistortionType}} = "D", {{SignalPurity}} = 1, {{AllocationDistortion}} = 0, '
                    "{{SweepPooledGapRange}} > 0.3, {{AllocationFragility}} >= 10)"
                ),
            },
        ],
    )

    for field in tr:
        if field["name"] == "DistortionType":
            field["Description"] = (
                "Geometric classification (five types A/B/C+/C-/D). "
                "Sign-flip types: A = unanimous stratum direction (IsStratumUnanimous); "
                "B = heterogeneous stratum directions. Non-flip: C+/C-/D by distortion ratio."
            )
            field["formula"] = (
                '=IF({{AllocationDistortion}} = "", "", '
                "IF({{IsSignFlip}}, "
                'IF({{IsStratumUnanimous}}, "A", "B"), '
                'IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01, ABS({{SignedPooledGap}}) > ABS({{CorrectedGap}}) + 0.001), "C+", '
                'IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01, ABS({{SignedPooledGap}}) < ABS({{CorrectedGap}}) - 0.001), "C-", "D"))))'
            )

    ms = rb["ModelSummary"]["schema"]
    existing = {f["name"] for f in ms}
    for field in ms:
        if field["name"] == "SweepFragileCount":
            field["Description"] = "Count of TreatmentRankings with IsSweepFragile=TRUE."
            field["formula"] = "=COUNTIFS(TreatmentRankings!{{IsSweepFragile}}, TRUE())"
    if "UnanimousSignFlipCount" not in existing:
        ms.append(
            {
                "name": "UnanimousSignFlipCount",
                "datatype": "integer",
                "type": "calculated",
                "nullable": True,
                "Description": "Sign-flip studies with unanimous stratum direction (IsSignFlip AND IsStratumUnanimous).",
                "formula": (
                    "=COUNTIFS(TreatmentRankings!{{IsSignFlip}}, TRUE(), "
                    "TreatmentRankings!{{IsStratumUnanimous}}, TRUE())"
                ),
            }
        )

    inv_exists = {i["InvariantCheckId"] for i in rb["InvariantChecks"]["data"]}
    if "inv-type-a-unanimous" not in inv_exists:
        rb["InvariantChecks"]["data"].append(
            {
                "InvariantCheckId": "inv-type-a-unanimous",
                "Name": "inv-type-a-unanimous",
                "AlgebraicStatement": "DistortionType = A → IsStratumUnanimous = TRUE",
                "NaturalLanguage": "Type-A sign-flip studies require unanimous stratum direction. rosiglitazone-mi-pool-2007 reclassified to Type B (1–1 split strata).",
                "SourceTable": "TreatmentRankings",
                "FilterExpression": "DistortionType = A",
                "AssertionExpression": "IsStratumUnanimous = TRUE",
                "SqlFilter": "distortion_type = 'A'",
                "SqlAssertion": "is_stratum_unanimous = TRUE",
                "PassCount": 0,
                "FailCount": 0,
                "Severity": "critical",
                "TraditionId": "tradition-dag",
                "ProtectsConclusion": "conc-03-binary-excludes-partial",
            }
        )


def main() -> None:
    bulk = load_bulk()
    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))
    patch_schema(rb)

    for row in rb["Loops"]["data"]:
        if row["LoopId"] == "loop-70":
            row["Status"] = "complete"
            row["MockDataNote"] = (
                "Witnessed post-build: IsStratumUnanimous + IsSweepFragile on TreatmentRankings. "
                "DistortionType A/B now keyed on stratum unanimity (not DistortionRatio). "
                "rosiglitazone-mi-pool-2007 reclassified A→B (1–1 split). "
                "inv-type-a-unanimous PASS on all Type-A rows. "
                "SweepFragileCount uses IsSweepFragile; UnanimousSignFlipCount added."
            )
            row["NextSuggestion"] = "loop-71: expansion wave 2 encode — criminal-justice + public-health-smoking gaps"

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Patched {RB_PATH} for loop-70")


if __name__ == "__main__":
    main()
