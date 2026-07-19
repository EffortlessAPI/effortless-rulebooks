#!/usr/bin/env python3
"""Execute loops 597-610 and push every semantic commit immediately.

The underlying convergence program still owns planning, validation, and one
commit per loop. This wrapper changes two publication details only:

1. every validated semantic commit is pushed before the next loop begins;
2. the existing GitHub Actions validation workflow is preserved byte-for-byte,
   because the Actions token may not update workflow files without the separate
   `workflows` permission.

Workflow maintenance remains a connector-authored projection change, not part
of a mathematical loop commit.
"""
from __future__ import annotations

import apply_loops_597_610_convergence_final3  # installs corrected adapters
import apply_loops_597_610_convergence_v2 as base

_original_commit = base.commit
_original_write_validation_files = base.write_validation_files


def semantic_validation_files_only() -> None:
    workflow_text = (
        base.VALIDATION_WORKFLOW.read_text()
        if base.VALIDATION_WORKFLOW.is_file()
        else None
    )
    _original_write_validation_files()
    if workflow_text is None:
        base.VALIDATION_WORKFLOW.unlink(missing_ok=True)
    else:
        base.VALIDATION_WORKFLOW.write_text(workflow_text)


def durable_commit(message: str, paths: list[base.Path]) -> None:
    _original_commit(message, paths)
    base.run(["git", "push", "origin", f"HEAD:{base.TARGET_BRANCH}"])


base.write_validation_files = semantic_validation_files_only
base.commit = durable_commit


if __name__ == "__main__":
    base.main()
