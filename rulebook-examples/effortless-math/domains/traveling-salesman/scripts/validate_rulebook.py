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
CALIBRATION_STATUS = DOMAIN / "testing" / "calibration-647-710-status.json"
CALIBRATION_VERIFIED = DOMAIN / "testing" / "calibration-647-710-verified.json"
CALIBRATION_VERIFIER = HERE / "verify_calibration_647_710.py"


def validate_calibration_read_only() -> None:
    if not CALIBRATION_STATUS.is_file() or not CALIBRATION_VERIFIER.is_file():
        return
    status = json.loads(CALIBRATION_STATUS.read_text())
    if status.get("status") != "SUCCEEDED" or int(status.get("last_loop", 0)) != 710:
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
