#!/usr/bin/env python3
"""Fix loop-68 adversarial study cell geometry after encode review."""

from __future__ import annotations

import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

CELL_PATCHES = {
    "hernandez-diaz-schisterman-hernan-2006": {
        "low-birth-weight": {"A": (800, 1000), "B": (900, 1000)},
        "normal-birth-weight": {"A": (650, 1000), "B": (500, 1000)},
    },
    "wade-boggs-vs-don-mattingly": {
        "1984": {"A": (32, 80), "B": (70, 200)},
        "1985": {"A": (180, 600), "B": (58, 200)},
    },
}


def load_bulk():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)
    return bulk


def main() -> None:
    bulk = load_bulk()
    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))

    for study_id, strata in CELL_PATCHES.items():
        for cell in rb["CaseCells"]["data"]:
            if cell["Study"] != study_id:
                continue
            slabel = cell["StratumLabel"]
            tlabel = cell["TreatmentLabel"]
            succ, cases = strata[slabel][tlabel]
            cell["Successes"] = succ
            cell["Cases"] = cases

        total_cases = sum(s[t][1] for s in strata.values() for t in ("A", "B"))
        for study in rb["Studies"]["data"]:
            if study["StudyId"] == study_id:
                study["TotalCases"] = total_cases
                break

        rb["StratumSummaries"]["data"] = [
            row for row in rb["StratumSummaries"]["data"] if row["Study"] != study_id
        ]
        for row in rb["StratumSummaries"]["data"]:
            pass
        # Re-stub stratum summary rows (calc columns come from views after build)
        for slabel in strata:
            for tlabel in ("A", "B"):
                rb["StratumSummaries"]["data"].append({
                    "StratumSummaryId": f"{study_id}-{slabel}-{tlabel}",
                    "Study": study_id,
                    "StratumLabel": slabel,
                    "TreatmentLabel": tlabel,
                    "Name": f"{study_id}-{slabel}-{tlabel}",
                })

    spec = importlib.util.spec_from_file_location("sweep", ROOT / "scripts" / "generate-allocation-sweep-all.py")
    sweep_mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(sweep_mod)

    cells = rb["CaseCells"]["data"]
    configs = []
    for row in rb["SweepStudyConfig"]["data"]:
        sid = row["StudyId"]
        configs.append(sweep_mod.compute_config(sid, cells))
    rb["SweepStudyConfig"]["data"] = configs

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Patched case cells in {RB_PATH}")


if __name__ == "__main__":
    main()
