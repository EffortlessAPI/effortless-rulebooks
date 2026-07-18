#!/usr/bin/env python3
"""Execute loops 597-610 and push every semantic commit immediately.

The underlying convergence program still owns planning, validation, and one
commit per loop. This wrapper changes only publication durability: after each
successful commit, the branch is pushed before the next loop begins. A later
failure therefore cannot erase already-validated loop history from the runner.
"""
from __future__ import annotations

import apply_loops_597_610_convergence_final3  # installs the corrected adapters
import apply_loops_597_610_convergence_v2 as base

_original_commit = base.commit


def durable_commit(message: str, paths: list[base.Path]) -> None:
    _original_commit(message, paths)
    base.run(["git", "push", "origin", f"HEAD:{base.TARGET_BRANCH}"])


base.commit = durable_commit


if __name__ == "__main__":
    base.main()
