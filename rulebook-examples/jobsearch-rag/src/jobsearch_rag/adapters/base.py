"""Shared data contract and abstract base class for job board adapters."""

from __future__ import annotations

import re
from abc import ABC, abstractmethod
from dataclasses import dataclass, field
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from datetime import datetime

    from playwright.async_api import Page

_MAX_FULL_TEXT = 250_000

_FILENAME_UNSAFE_RE = re.compile(r'[<>:"|?*\x00-\x1f]')
_PATH_SEP_RE = re.compile(r"[/\\]")
_TRAVERSAL_RE = re.compile(r"\.\.")


def _sanitize_filename_field(value: str) -> str:
    """Remove path traversal sequences and filesystem-unsafe characters."""
    value = _PATH_SEP_RE.sub("-", value)
    value = _TRAVERSAL_RE.sub("", value)
    return _FILENAME_UNSAFE_RE.sub("", value).strip()


@dataclass
class JobListing:
    """
    Board-agnostic data contract consumed by the RAG scorer, ranker, and exporter.

    Required fields are always populated after adapter extraction.
    Optional fields degrade gracefully when absent.
    """

    board: str
    external_id: str
    title: str
    company: str
    location: str
    url: str
    full_text: str
    posted_at: datetime | None = None
    raw_html: str | None = None
    comp_min: float | None = None
    comp_max: float | None = None
    comp_source: str | None = None
    comp_text: str | None = None
    metadata: dict[str, str] = field(default_factory=dict[str, str])

    def __post_init__(self) -> None:
        """Validate field bounds and sanitize filename-unsafe content."""
        if self.full_text and len(self.full_text) > _MAX_FULL_TEXT:
            msg = f"full_text exceeds maximum length: {len(self.full_text)} chars"
            raise ValueError(msg)
        self.title = _sanitize_filename_field(self.title)
        self.company = _sanitize_filename_field(self.company)


class JobBoardAdapter(ABC):
    """
    Strategy interface for job board integration.

    Each adapter owns: authentication, search result pagination,
    and full JD extraction. Nothing else.
    """

    @property
    @abstractmethod
    def board_name(self) -> str:
        """Unique identifier string for this board."""
        ...

    @abstractmethod
    async def authenticate(self, page: Page) -> None:
        """
        Establish an authenticated session.

        Uses Playwright ``storage_state`` for cookie persistence
        so this is a no-op on subsequent runs.
        """
        ...

    @abstractmethod
    async def search(
        self,
        page: Page,
        query: str,
        max_pages: int = 3,
    ) -> list[JobListing]:
        """
        Navigate search results and return shallow listings.

        Full text is fetched separately via :meth:`extract_detail`.
        """
        ...

    @abstractmethod
    async def extract_detail(
        self,
        page: Page,
        listing: JobListing,
    ) -> JobListing:
        """Navigate to ``listing.url`` and populate ``full_text``."""
        ...

    @property
    def rate_limit_seconds(self) -> tuple[float, float]:
        """
        (min, max) seconds to sleep between page loads.

        Override in board-specific adapters. Defaults to human-like range.
        """
        return (1.5, 3.5)
