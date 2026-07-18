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

    old_filter = "item.tsp_instance == iid"
    new_filter = "item.tsp_instance_id == iid"
    if old_filter in text:
        text = text.replace(old_filter, new_filter)
    elif new_filter not in text:
        raise AssertionError("component-repair override lacks a recognized TSP instance field")

    old_mapping = '            "tsp_instance": iid,\n'
    new_mapping = (
        '            "tsp_instance": iid,\n'
        '            "tsp_instance_id": iid,\n'
    )
    if '            "tsp_instance_id": iid,\n' not in text:
        if old_mapping not in text:
            raise AssertionError("component-repair override lacks the instance result mapping")
        text = text.replace(old_mapping, new_mapping, 1)

    base.REFERENCE_MODEL.write_text(text)


base.patch_reference_model_for_terms = append_reference_override_fixed


if __name__ == "__main__":
    base.main()
