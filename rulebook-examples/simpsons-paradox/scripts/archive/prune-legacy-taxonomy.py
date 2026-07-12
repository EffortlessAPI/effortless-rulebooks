#!/usr/bin/env python3
"""Prune legacy v2 reversal meta and duplicate ModelSummary fields from the rulebook."""
from __future__ import annotations

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

TR_REMOVE_FIELDS = {
    "IsReversal_v2",
    "DefinitionDelta",
    "StrictReversalSubtype",
    "IsParadoxExplained_v2",
}

MS_REMOVE_FIELDS = {
    "ZeroStrengthCount",
    "PartialCount",
    "ReversalCountV2",
    "NonReversalCountV2",
    "ExtendedReversalCount",
    "ExplainedCountV2",
    "TypeCCount",
}

IS_REVERSAL_FORMULA = '=IF({{IsSignFlip}} = "", "", {{IsSignFlip}})'
IS_REVERSAL_DESC = (
    "TRUE when the equal-weight pooled signal (WeightedStratumGapSum) and the actual "
    "pooled signal (SignedPooledGap) point in opposite directions — allocation has flipped "
    "the direction of the aggregate conclusion. This is Simpson's Paradox as a derived fact."
)

IS_SIGNFLIP_DESC = (
    "TRUE when WeightedStratumGapSum and SignedPooledGap have opposite signs. "
    "Same condition as IsReversal; retained as the coordinate-level name in the DAG."
)

IS_PARADOX_EXPLAINED_DESC = (
    "TRUE when IsReversal is present AND at least one confirmed confounder exists in the study."
)

REVERSAL_COUNT_DESC = (
    "Number of studies with IsReversal=TRUE (sign-flip / allocation-driven reversal)."
)


def compact_data_rows(rb: dict) -> None:
    pretty = json.dumps(rb, indent=2, ensure_ascii=False)
    lines = pretty.splitlines()
    out: list[str] = []
    in_data = False
    depth = 0
    buf: list[str] = []

    for line in lines:
        if re.match(r'^\s+"data": \[\s*$', line):
            in_data = True
            out.append(line)
            continue
        if in_data:
            if line.strip() == "]":
                if buf:
                    out.append(" ".join(buf))
                    buf = []
                in_data = False
                out.append(line)
                continue
            opens = line.count("{")
            closes = line.count("}")
            if depth == 0 and opens:
                buf = [line.strip()]
                depth += opens - closes
                if depth == 0:
                    out.append(" ".join(buf))
                    buf = []
                continue
            if depth > 0:
                buf.append(line.strip())
                depth += opens - closes
                if depth == 0:
                    out.append(" ".join(buf))
                    buf = []
                continue
        out.append(line)

    RB_PATH.write_text("\n".join(out) + "\n", encoding="utf-8")


def main() -> None:
    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))

    tr_schema = rb["TreatmentRankings"]["schema"]
    rb["TreatmentRankings"]["schema"] = [
        f for f in tr_schema if f.get("name") not in TR_REMOVE_FIELDS
    ]
    for field in rb["TreatmentRankings"]["schema"]:
        name = field.get("name")
        if name == "IsReversal":
            field["formula"] = IS_REVERSAL_FORMULA
            field["Description"] = IS_REVERSAL_DESC
        elif name == "IsSignFlip":
            field["Description"] = IS_SIGNFLIP_DESC
        elif name == "IsParadoxExplained":
            field["Description"] = IS_PARADOX_EXPLAINED_DESC

    for row in rb["TreatmentRankings"]["data"]:
        for key in TR_REMOVE_FIELDS:
            row.pop(key, None)
        if "IsSignFlip" in row:
            row["IsReversal"] = row["IsSignFlip"]

    ms_schema = rb["ModelSummary"]["schema"]
    rb["ModelSummary"]["schema"] = [
        f for f in ms_schema if f.get("name") not in MS_REMOVE_FIELDS
    ]
    for field in rb["ModelSummary"]["schema"]:
        if field.get("name") == "ReversalCount":
            field["Description"] = REVERSAL_COUNT_DESC

    for row in rb["ModelSummary"]["data"]:
        for key in MS_REMOVE_FIELDS:
            row.pop(key, None)

    loops = rb["Loops"]["data"]
    loops.append(
        {
            "LoopId": "loop-60",
            "Title": "Legacy prune: IsReversal = sign-flip; drop v2 meta and presentation layer",
            "Status": "complete",
            "NewConcept": "IsReversal redefined as IsSignFlip (single reversal definition). Removed IsReversal_v2, DefinitionDelta, StrictReversalSubtype, IsParadoxExplained_v2, InstrumentScore, ScreeningVerdict, duplicate ModelSummary v2 counts (ReversalCountV2, PartialCount, ZeroStrengthCount, TypeCCount). Unanimous-vs-partial distinction lives in DistortionType A vs B only.",
            "DomainQuestion": "Can the instrument shed legacy presentation and v2 meta fields without losing load-bearing taxonomy coordinates (DistortionType, SignalPurity, AllocationDirection)?",
            "MockDataNote": "Witnessed post-build: IsReversal equals IsSignFlip on all studies. ReversalCount counts sign-flips. Type B captures partial stratum disagreement without DefinitionDelta. ScreeningTier retained (derived from DistortionType).",
            "NextSuggestion": "loop-61: DiscoveryHypotheses + DiscoveryFindings — first empirical discovery loop on the pruned instrument.",
            "TraditionId": "tradition-dag",
        }
    )

    RB_PATH.write_text(json.dumps(rb, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    rb2 = json.loads(RB_PATH.read_text(encoding="utf-8"))
    compact_data_rows(rb2)
    print(f"Patched {RB_PATH}")


if __name__ == "__main__":
    main()
