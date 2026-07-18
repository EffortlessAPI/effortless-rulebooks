#!/usr/bin/env python3
"""Execute loops 611-623 and push every semantic commit immediately.

The assembled refinement module owns the mathematical changes. This wrapper
preserves the existing validation workflow outside semantic commits and makes
each validated plan/loop commit durable before the next loop begins.
"""
from __future__ import annotations

import apply_loops_611_623_refinement as refinement

_original_commit = refinement.commit
_original_write_validation_files = refinement.write_validation_files


def semantic_validation_files_only() -> None:
    workflow_text = (
        refinement.VALIDATION_WORKFLOW.read_text()
        if refinement.VALIDATION_WORKFLOW.is_file()
        else None
    )
    _original_write_validation_files()
    if workflow_text is None:
        refinement.VALIDATION_WORKFLOW.unlink(missing_ok=True)
    else:
        refinement.VALIDATION_WORKFLOW.write_text(workflow_text)


def durable_commit(message: str, paths: list[refinement.Path]) -> None:
    _original_commit(message, paths)
    refinement.run(
        ["git", "push", "origin", f"HEAD:{refinement.TARGET_BRANCH}"],
        cwd=refinement.REPO,
    )


refinement.write_validation_files = semantic_validation_files_only
refinement.commit = durable_commit


if __name__ == "__main__":
    refinement.main()
