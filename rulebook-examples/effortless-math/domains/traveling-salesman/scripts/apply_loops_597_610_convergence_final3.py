#!/usr/bin/env python3
"""Final convergence executor with the LocalDegreeBoundResult field adapter fixed."""
from __future__ import annotations

import apply_loops_597_610_convergence_final2 as driver
import apply_loops_597_610_convergence_final as final
import apply_loops_597_610_convergence_v2 as base


def append_reference_override_fixed() -> None:
    """Install the final lower-bound override, then normalize its dataclass field name."""
    final.append_reference_override()
    text = base.REFERENCE_MODEL.read_text()
    old = "item.tsp_instance == iid"
    new = "item.tsp_instance_id == iid"
    if old in text:
        text = text.replace(old, new)
        base.REFERENCE_MODEL.write_text(text)
    elif new not in text:
        raise AssertionError("component-repair override lacks a recognized TSP instance field")


base.patch_reference_model_for_terms = append_reference_override_fixed


if __name__ == "__main__":
    base.main()
