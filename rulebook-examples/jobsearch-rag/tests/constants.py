"""
Shared test constants.

Values that multiple test modules reference but that are not fixtures.
Fixtures belong in ``conftest.py``; plain data belongs here.
"""

from __future__ import annotations

# Canonical fake embedding used across test files.  Individual tests that
# need a different vector can define their own constant.
EMBED_FAKE: list[float] = [0.1, 0.2, 0.3, 0.4, 0.5]
