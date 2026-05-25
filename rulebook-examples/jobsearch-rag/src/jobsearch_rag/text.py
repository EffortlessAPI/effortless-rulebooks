"""
Shared text-processing utilities.

Pure functions with no domain dependencies — safe to import from any
layer (CLI, pipeline, export, RAG).
"""

from __future__ import annotations

import re

MAX_SLUG_LEN = 80


def slugify(text: str, *, max_len: int = MAX_SLUG_LEN) -> str:
    """
    Convert *text* to a filesystem-safe slug.

    Lowercases, strips non-alphanumeric characters (except hyphens),
    collapses whitespace/underscores to single hyphens, and truncates
    to *max_len* characters.

    >>> slugify("Senior Staff Engineer — Platform (Remote)")
    'senior-staff-engineer-platform-remote'
    """
    slug = text.lower()
    slug = re.sub(r"[^\w\s-]", "", slug)
    slug = re.sub(r"[\s_]+", "-", slug)
    slug = slug.strip("-")
    return slug[:max_len]
