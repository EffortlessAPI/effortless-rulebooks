#!/usr/bin/env python3
import json
import subprocess
import sys
from pathlib import Path

from validate_rulebook_v3_temporal import main as validate_v3
from validate_rulebook_v5_temporal import main as validate_v5
from validate_rulebook_v6 import validate_repository_state as validate_v6
from validate_summary_alignment_v6 import validate_summary_alignment as validate_summary_v6
from validate_rulebook_v7 import validate_repository_state as validate_v7
from validate_summary_alignment_v7 import validate_summary_alignment as validate_summary_v7

HERE = Path(__file__).resolve().parent
DOMAIN = HERE.parent
RULEBOOK = DOMAIN / "effortless-rulebook" / "traveling-salesman-rulebook.json"
CALIBRATION_STATUS = DOMAIN / "testing" / "calibration-647-710-status.json"
CALIBRATION_VERIFIED = DOMAIN / "testing" / "calibration-647-710-verified.json"
CALIBRATION_VERIFIER = HERE / "verify_calibration_647_710.py"
CALIBRATION_HISTORY_MODE = DOMAIN / "testing" / "calibration-history-mode.json"


def load_json(path: Path) -> dict:
    if not path.is_file():
        raise FileNotFoundError(path)
    return json.loads(path.read_text())


def validate_squashed_calibration_ledger() -> None:
    """Verify calibration history from canonical rows after an intentional squash.

    Git commit subjects are useful provenance when available, but they are not the
    mathematical source of truth. After a deliberate squash merge, the versioned
    TSPLoops rows and the retained independent evidence certificate carry that
    history instead.
    """
    mode = load_json(CALIBRATION_HISTORY_MODE)
    if mode.get("mode") != "SQUASHED_CANONICAL_LEDGER":
        raise AssertionError(f"unsupported calibration history mode: {mode!r}")
    if mode.get("detailed_git_commit_history_required") is not False:
        raise AssertionError("squashed history mode must explicitly disable commit-subject dependence")

    status = load_json(CALIBRATION_STATUS)
    verified = load_json(CALIBRATION_VERIFIED)
    rulebook = load_json(RULEBOOK)

    if status.get("status") != "SUCCEEDED" or int(status.get("last_loop", 0)) != 710:
        raise AssertionError(f"calibration executor status is {status!r}")
    if verified.get("status") != "VERIFIED" or verified.get("loops") != "647-710":
        raise AssertionError(f"calibration evidence certificate is {verified!r}")
    if int(verified.get("exact_oracle_coverage_pct", 0)) != 100:
        raise AssertionError("calibration evidence lacks complete declared oracle coverage")

    rows = rulebook["TSPLoops"]["data"]
    loops = {int(row["LoopOrder"]): row for row in rows}
    missing = [order for order in range(647, 711) if order not in loops]
    if missing:
        raise AssertionError(f"canonical calibration loop rows missing: {missing}")
    for order in range(647, 711):
        row = loops[order]
        if row.get("Status") != "CLOSED":
            raise AssertionError(f"canonical loop {order} is {row.get('Status')!r}")
        for field in ("BeforeState", "PlannedClosureCriterion", "AfterState", "WitnessSummary"):
            if not row.get(field):
                raise AssertionError(f"canonical loop {order} lacks {field}")

    print("traveling-salesman calibration canonical-ledger validation: PASS (squashed Git history)")


def validate_calibration_read_only() -> None:
    if not CALIBRATION_STATUS.is_file() or not CALIBRATION_VERIFIER.is_file():
        return
    status = load_json(CALIBRATION_STATUS)
    if status.get("status") != "SUCCEEDED" or int(status.get("last_loop", 0)) != 710:
        return

    if CALIBRATION_HISTORY_MODE.is_file():
        mode = load_json(CALIBRATION_HISTORY_MODE)
        if mode.get("mode") == "SQUASHED_CANONICAL_LEDGER":
            validate_squashed_calibration_ledger()
            return

    prior = CALIBRATION_VERIFIED.read_bytes() if CALIBRATION_VERIFIED.is_file() else None
    try:
        subprocess.run([sys.executable, str(CALIBRATION_VERIFIER)], check=True)
    finally:
        if prior is None:
            CALIBRATION_VERIFIED.unlink(missing_ok=True)
        else:
            CALIBRATION_VERIFIED.write_bytes(prior)


if __name__ == "__main__":
    validate_v3()
    validate_v5()
    validate_v6()
    validate_summary_v6()
    validate_v7()
    validate_summary_v7()
    validate_calibration_read_only()
