#!/usr/bin/env python3
from pathlib import Path
import sys

if len(sys.argv) != 2:
    raise SystemExit('usage: patch_calibration_647_710.py <executor>')
path = Path(sys.argv[1])
text = path.read_text()
old = '''def finish_loop(rb: dict[str, Any], contract: dict[str, Any], order: int, after: str, witness: str) -> None:
    base.finish_loop(rb, contract, order, after, witness)
'''
new = '''def finish_loop(rb: dict[str, Any], contract: dict[str, Any], order: int, after: str, witness: str) -> None:
    loop = next(row for row in rows(rb, "TSPLoops") if int(row["LoopOrder"]) == order)
    loop.update(
        {
            "Status": "CLOSED",
            "AfterState": after,
            "WitnessSummary": witness,
            "CompletionDisposition": "CLOSED",
            "NextFrontier": LOOPS[order]["next"],
        }
    )
    contract_loop = next(row for row in contract["Loops"] if int(row["LoopOrder"]) == order)
    contract_loop.update(
        {
            "Status": "CLOSED",
            "AfterState": after,
            "Result": witness,
            "CompletionDisposition": "CLOSED",
        }
    )
    set_meta(rb, "last_loop", "integer", integer=order)
    set_meta(rb, "highest_completed_loop", "integer", integer=order)
'''
if old in text:
    path.write_text(text.replace(old, new))
elif '"NextFrontier": LOOPS[order]["next"]' not in text:
    raise AssertionError('calibration finish_loop patch target missing')
print('calibration finish_loop hotfix: PASS')
