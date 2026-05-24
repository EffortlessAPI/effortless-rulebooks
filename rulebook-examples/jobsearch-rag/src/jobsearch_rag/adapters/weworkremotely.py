"""WeWorkRemotely adapter — auth, search pagination, JD extraction."""

from __future__ import annotations

from typing import TYPE_CHECKING

from jobsearch_rag.adapters.base import JobBoardAdapter, JobListing
from jobsearch_rag.adapters.registry import AdapterRegistry

if TYPE_CHECKING:
    from playwright.async_api import Page


@AdapterRegistry.register
class WeWorkRemotelyAdapter(JobBoardAdapter):
    """Browser automation adapter for WeWorkRemotely."""

    @property
    def board_name(self) -> str:
        """Return the board identifier."""
        return "weworkremotely"

    async def authenticate(self, page: Page) -> None:
        """Authenticate with WeWorkRemotely."""
        raise NotImplementedError

    async def search(
        self,
        page: Page,
        query: str,
        max_pages: int = 3,
    ) -> list[JobListing]:
        """Search WeWorkRemotely for job listings matching *query*."""
        raise NotImplementedError

    async def extract_detail(
        self,
        page: Page,
        listing: JobListing,
    ) -> JobListing:
        """Extract full job details from a WeWorkRemotely listing page."""
        raise NotImplementedError
