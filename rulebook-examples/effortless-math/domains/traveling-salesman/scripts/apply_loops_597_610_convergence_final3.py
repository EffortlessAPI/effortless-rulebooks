#!/usr/bin/env python3
"""Final convergence executor with LocalDegreeBoundResult adapters normalized."""
from __future__ import annotations

import apply_loops_597_610_convergence_final2 as driver
import apply_loops_597_610_convergence_final as final
import apply_loops_597_610_convergence_v2 as base


def append_reference_override_fixed() -> None:
    """Install the final lower-bound override and normalize its result field mapping."""
    final.append_reference_override()
    text = base.REFERENCE_MODEL.read_text()
    marker = "# TSP_COMPONENT_REPAIR_BOUND_OVERRIDE_FINAL"
    if marker not in text:
        raise AssertionError("component-repair override marker is missing")

    prefix, suffix = text.split(marker, 1)
    suffix = suffix.replace(
        "item.tsp_instance == iid",
        "item.tsp_instance_id == iid",
    )

    old_mapping = '            "tsp_instance": iid,\n'
    mapped = (
        '            "tsp_instance": iid,\n'
        '            "tsp_instance_id": iid,\n'
    )
    if '            "tsp_instance_id": iid,\n' not in suffix:
        if old_mapping not in suffix:
            raise AssertionError("component-repair override lacks the instance result mapping")
        suffix = suffix.replace(old_mapping, mapped, 1)

    repaired = prefix + marker + suffix
    if "item.tsp_instance_id == iid" not in repaired:
        raise AssertionError("component-repair override filter was not repaired")
    base.REFERENCE_MODEL.write_text(repaired)


base.patch_reference_model_for_terms = append_reference_override_fixed


if __name__ == "__main__":
    base.main()
