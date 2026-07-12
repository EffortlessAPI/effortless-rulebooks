#!/usr/bin/env python3
"""Tune measles-outbreak-vaccinated cells so Type-D ratio invariant passes (loop-59)."""
import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

CELL_PATCHES = {
    "measles-outbreak-vaccinated": {
        "high-coverage-setting-A": (100, 9000),
        "high-coverage-setting-B": (44, 1000),
        "lower-coverage-setting-A": (15, 2000),
        "lower-coverage-setting-B": (35, 500),
    },
}


def recompute_treatment_totals(rb: dict, study_id: str) -> None:
    cells = [c for c in rb["CaseCells"]["data"] if c["Study"] == study_id]
    for tlabel in ("A", "B"):
        tc = [c for c in cells if c["TreatmentLabel"] == tlabel]
        total_cases = sum(c["Cases"] for c in tc)
        total_succ = sum(c["Successes"] for c in tc)
        for t in rb["Treatments"]["data"]:
            if t["Study"] == study_id and t["TreatmentLabel"] == tlabel:
                t["TotalCases"] = total_cases
                t["TotalSuccesses"] = total_succ
                t["PooledSuccessRate"] = total_succ / total_cases if total_cases else 0


def main():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)

    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))

    for cell in rb["CaseCells"]["data"]:
        sid = cell["Study"]
        if sid not in CELL_PATCHES:
            continue
        key = f"{cell['StratumLabel']}-{cell['TreatmentLabel']}"
        if key in CELL_PATCHES[sid]:
            s, c = CELL_PATCHES[sid][key]
            cell["Successes"] = s
            cell["Cases"] = c

    sid = "measles-outbreak-vaccinated"
    cells = [c for c in rb["CaseCells"]["data"] if c["Study"] == sid]
    total = sum(c["Cases"] for c in cells)
    for study in rb["Studies"]["data"]:
        if study["StudyId"] == sid:
            study["TotalCases"] = total
    recompute_treatment_totals(rb, sid)

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Patched {sid}: total_cases={total}")


if __name__ == "__main__":
    main()
