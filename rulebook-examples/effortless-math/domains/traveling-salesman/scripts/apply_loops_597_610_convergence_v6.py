#!/usr/bin/env python3
"""Final convergence driver with rulebook-derived Postgres frontier accounting."""
from __future__ import annotations

import re

import apply_loops_597_610_convergence_v2 as base
import apply_loops_597_610_convergence_v3 as hard
import apply_loops_597_610_convergence_v4 as reload_driver
import apply_loops_597_610_convergence_v5 as adaptive


def patch_take_test_frontier() -> None:
    path = base.TAKE_TEST
    text = path.read_text()
    marker = "# TSP_RULEBOOK_DERIVED_FRONTIER_EXPECTATION_V1"
    if marker in text:
        return
    start = text.find("    frontier_rows = psql(")
    end = text.find("    metric_rows = psql(", start)
    if start < 0 or end < 0:
        raise AssertionError("take-test frontier block not found")
    replacement = '''    # TSP_RULEBOOK_DERIVED_FRONTIER_EXPECTATION_V1\n    frontier_rows = psql(\n        "SELECT count(*), count(*) FILTER (WHERE is_imported_dependency), "\n        "count(*) FILTER (WHERE is_closed) FROM vw_tsp_frontier_obligations"\n    )\n    expected_frontier_total = len(rulebook["TSPFrontierObligations"]["data"])\n    expected_frontier_imported = sum(\n        row.get("IsImportedDependency") is True\n        for row in rulebook["TSPFrontierObligations"]["data"]\n    )\n    expected_frontier_closed = sum(\n        row.get("Status") == "CLOSED"\n        for row in rulebook["TSPFrontierObligations"]["data"]\n    )\n    expected_frontier = [\n        f"{expected_frontier_total},{expected_frontier_imported},{expected_frontier_closed}"\n    ]\n    if frontier_rows != expected_frontier:\n        failures.append(\n            f"[frontier] expected {expected_frontier}, got {frontier_rows}"\n        )\n\n'''
    text = text[:start] + replacement + text[end:]
    text = re.sub(
        r'    print\("frontier obligations:.*?"\)\n',
        '    print(\n        f"frontier obligations: {expected_frontier_total} total, "\n        f"{expected_frontier_imported} imported, {expected_frontier_closed} closed"\n    )\n',
        text,
        count=1,
    )
    path.write_text(text)


def loop_610_patched(rb, contract):
    patch_take_test_frontier()
    return base.loop_610(rb, contract)


def write_validation_files() -> None:
    adaptive.write_validation_files()
    workflow = base.VALIDATION_WORKFLOW.read_text()
    if 'apply_loops_597_610_convergence_v6.py' not in workflow:
        workflow = workflow.replace(
            '"$domain/scripts/apply_loops_597_610_convergence_v5.py" \\\n',
            '"$domain/scripts/apply_loops_597_610_convergence_v5.py" \\\n            "$domain/scripts/apply_loops_597_610_convergence_v6.py" \\\n',
        )
    base.VALIDATION_WORKFLOW.write_text(workflow)


base.patch_reference_model_for_terms = adaptive.append_reference_override
base.LOOP_FUNCS[603] = hard.loop_603_fixed
base.LOOP_FUNCS[607] = adaptive.loop_607_adaptive
base.LOOP_FUNCS[609] = adaptive.loop_609_adaptive
base.LOOP_FUNCS[610] = loop_610_patched
base.validate_repository_state = adaptive.validate_repository_state
base.write_validation_files = write_validation_files


if __name__ == "__main__":
    base.main()
