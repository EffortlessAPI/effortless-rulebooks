#!/usr/bin/env python3
"""Final hardened driver: reload the Python peer after loop-local source changes."""
from __future__ import annotations

import importlib
import sys
from typing import Any

import apply_loops_597_610_convergence_v2 as base
import apply_loops_597_610_convergence_v3 as hardened


def validate_repository_state() -> None:
    sys.path.insert(0, str(base.DOMAIN / "scripts"))
    importlib.invalidate_caches()
    import reference_model
    importlib.reload(reference_model)
    rb = base.load(base.RULEBOOK)
    contract = base.load(base.CONTRACT)
    hardened.validate_core(rb)
    base.validate_additional(rb, contract)
    graph = reference_model.evaluate_graph(rb)
    tours = reference_model.evaluate_tours(rb)
    if not graph["tsp-gridville-5"]["is_complete_undirected_graph"]:
        raise AssertionError("Gridville graph drift")
    if not tours["tour-reference-ring"].is_optimality_proved:
        raise AssertionError("Gridville optimality drift")
    loops = {int(row["LoopOrder"]): row for row in base.rows(rb, "TSPLoops")}
    if loops.get(604, {}).get("Status") == "CLOSED":
        twin = tours["tour-twin-triangles-feasible-24"]
        if not twin.is_optimality_proved or twin.total_travel_cost != 24:
            raise AssertionError(f"Twin optimality did not close: {twin}")
    if contract["Acceptance"]["RulebookTables"] != len(base.canonical_tables(rb)):
        raise AssertionError("contract table count drift")
    print("traveling-salesman convergence validation v4: PASS")


def write_validation_files() -> None:
    hardened.write_validation_files()
    base.VALIDATOR_V4.write_text(
        '''#!/usr/bin/env python3\nfrom apply_loops_597_610_convergence_v4 import validate_repository_state\n\nif __name__ == "__main__":\n    validate_repository_state()\n'''
    )
    base.VALIDATOR_WRAPPER.write_text(
        '''#!/usr/bin/env python3\nfrom validate_rulebook_v4 import validate_repository_state\nfrom validate_summary_alignment_v4 import validate_summary_alignment\n\nif __name__ == "__main__":\n    validate_repository_state()\n    validate_summary_alignment()\n'''
    )
    workflow = base.VALIDATION_WORKFLOW.read_text()
    if 'apply_loops_597_610_convergence_v4.py' not in workflow:
        workflow = workflow.replace(
            '"$domain/scripts/apply_loops_597_610_convergence_v3.py" \\\n',
            '"$domain/scripts/apply_loops_597_610_convergence_v3.py" \\\n            "$domain/scripts/apply_loops_597_610_convergence_v4.py" \\\n',
        )
    base.VALIDATION_WORKFLOW.write_text(workflow)


base.patch_reference_model_for_terms = hardened.append_reference_override
base.LOOP_FUNCS[603] = hardened.loop_603_fixed
base.LOOP_FUNCS[607] = hardened.loop_607_fixed
base.LOOP_FUNCS[609] = hardened.loop_609_fixed
base.validate_repository_state = validate_repository_state
base.write_validation_files = write_validation_files


if __name__ == "__main__":
    base.main()
