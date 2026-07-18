#!/usr/bin/env python3
"""Run the standalone loop-587 finalizer with an idempotent README transition."""
from __future__ import annotations

import complete_loop_587_final as final

_original_readme_project = final.readme_project
_extra_blocked_projection = (
    "\n**Postgres commissioning: BLOCKED.** The generated backend and peer comparisons "
    "have executed successfully in retry 8, but the canonical substrate obligation "
    "remains blocked until the success certificate, execution history, artifact hashes, "
    "and summary projections are durably sealed and validated.\n"
)


def readme_project(text: str) -> str:
    return _original_readme_project(text.replace(_extra_blocked_projection, "\n"))


final.readme_project = readme_project

if __name__ == "__main__":
    final.main()
