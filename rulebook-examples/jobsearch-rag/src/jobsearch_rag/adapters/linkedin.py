"""LinkedIn adapter — overnight mode, stealth, throttled."""

from __future__ import annotations

from typing import TYPE_CHECKING

from jobsearch_rag.adapters.base import JobBoardAdapter, JobListing
from jobsearch_rag.adapters.registry import AdapterRegistry
from jobsearch_rag.errors import ActionableError

if TYPE_CHECKING:
    from playwright.async_api import Page


async def check_linkedin_detection(page: Page) -> None:
    """
    Check current page for LinkedIn bot-detection indicators.

    Raises ``ActionableError.authentication`` if detection is found.
    """
    url = page.url

    # Redirect to authwall
    if "/authwall" in url:
        raise ActionableError.authentication(
            "linkedin",
            "Redirected to /authwall — bot detection triggered",
            suggestion="Wait at least 24 hours before the next LinkedIn run",
        )

    # Challenge interstitial
    title = await page.title()
    if "security" in title.lower() or "challenge" in title.lower():
        raise ActionableError.authentication(
            "linkedin",
            f"Challenge page detected: '{title}'",
            suggestion="Wait at least 24 hours before the next LinkedIn run",
        )

    # Session invalidation (logged out mid-run)
    if "/login" in url or "/uas/login" in url:
        raise ActionableError.authentication(
            "linkedin",
            "Session invalidated — redirected to login page",
            suggestion="Re-authenticate manually, then wait before retrying",
        )


@AdapterRegistry.register
class LinkedInAdapter(JobBoardAdapter):
    """
    Browser automation adapter for LinkedIn.

    Requires headed mode and ``playwright-stealth`` fingerprint patches.
    Run via ``--board linkedin --overnight`` for safe, throttled operation.
    """

    @property
    def board_name(self) -> str:
        """Return the board identifier."""
        return "linkedin"

    @property
    def rate_limit_seconds(self) -> tuple[float, float]:
        """Return the throttle range in seconds for LinkedIn requests."""
        return (8.0, 20.0)

    async def authenticate(self, page: Page) -> None:
        """Authenticate with LinkedIn."""
        raise NotImplementedError

    async def search(
        self,
        page: Page,
        query: str,
        max_pages: int = 3,
    ) -> list[JobListing]:
        """Search LinkedIn for job listings matching *query*."""
        raise NotImplementedError

    async def extract_detail(
        self,
        page: Page,
        listing: JobListing,
    ) -> JobListing:
        """Extract full job details from a LinkedIn listing page."""
        raise NotImplementedError
