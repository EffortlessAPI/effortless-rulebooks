"""
WeWorkRemotely adapter extraction tests — remote-first job board.

Spec classes:
    TestWeWorkRemotelyAuthenticate — session or public-access verification
    TestWeWorkRemotelySearch — job search and listing extraction
    TestWeWorkRemotelyExtractDetail — full JD extraction from listing pages

All tests are xfail until the adapter is implemented. The specs define
the delivery contract — each test becomes a regression gate once the
adapter is delivered.

WeWorkRemotely specifics:
    - Remote-only job board — all listings are remote positions
    - Simpler anti-bot measures than LinkedIn or Indeed
    - May support unauthenticated browsing for public listings
    - Category-based navigation (e.g., programming, devops)
"""

from __future__ import annotations

from unittest.mock import MagicMock

import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.adapters.weworkremotely import WeWorkRemotelyAdapter
from jobsearch_rag.errors import ActionableError

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

_XFAIL = pytest.mark.xfail(
    reason="WeWorkRemotely adapter not yet implemented",
)


def _make_listing(
    *,
    full_text: str = "",
    external_id: str = "wwr-001",
) -> JobListing:
    """Create a minimal WeWorkRemotely JobListing for testing."""
    return JobListing(
        board="weworkremotely",
        external_id=external_id,
        title="Senior Backend Engineer",
        company="RemoteCo",
        location="Remote",
        url="https://weworkremotely.com/remote-jobs/remoteco-senior-backend-engineer",
        full_text=full_text,
    )


# ---------------------------------------------------------------------------
# TestWeWorkRemotelyAuthenticate
# ---------------------------------------------------------------------------


class TestWeWorkRemotelyAuthenticate:
    """
    REQUIREMENT: WeWorkRemotely session verification handles auth and access.

    WHO: The pipeline runner during the authenticate step
    WHAT: (1) The system completes authentication without error when WeWorkRemotely loads without blocking.
          (2) The system raises an ActionableError with a wait or retry suggestion when authentication encounters a rate-limit or block response.
    WHY: Even lighter-touch boards can block automated access —
         authenticate must verify access before search begins

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: WeWorkRemotelyAdapter.authenticate
        Never: Patch detection logic internals
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_succeeds_on_valid_access(self) -> None:
        """
        Given a mock page that loads WeWorkRemotely without blocking
        When authenticate is called
        Then it completes without error.
        """
        # Given: valid access page
        adapter = WeWorkRemotelyAdapter()
        page = MagicMock()

        # When: authenticate
        await adapter.authenticate(page)

        # Then: no error raised (implicit)

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_rate_limit_raises_with_wait_suggestion(self) -> None:
        """
        Given a page showing a rate-limit or block response
        When authenticate is called
        Then an ActionableError with wait/retry guidance is raised.
        """
        # Given: rate-limited page
        adapter = WeWorkRemotelyAdapter()
        page = MagicMock()

        # When/Then: raises ActionableError with rate-limit guidance
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        # Then: actionable error with guidance
        err = exc_info.value
        assert err.suggestion is not None, "Should include suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"


# ---------------------------------------------------------------------------
# TestWeWorkRemotelySearch
# ---------------------------------------------------------------------------


class TestWeWorkRemotelySearch:
    """
    REQUIREMENT: search() navigates WeWorkRemotely and extracts remote listings.

    WHO: The pipeline runner collecting listings from WeWorkRemotely
    WHAT: (1) The system returns a list of JobListing objects when it searches WeWorkRemotely results.
          (2) The system populates each returned WeWorkRemotely listing with the board, title, company, and url fields.
          (3) The system processes only the first page of WeWorkRemotely results when `max_pages` is set to 1.
    WHY: WeWorkRemotely is the primary remote-only board — all listings
         are guaranteed remote positions, simplifying location filtering

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: WeWorkRemotelyAdapter.search
        Never: Patch extraction logic internals
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_returns_job_listings(self) -> None:
        """
        Given a mock page with WeWorkRemotely search results
        When search is called
        Then a list of JobListings is returned.
        """
        # Given: adapter + mock page
        adapter = WeWorkRemotelyAdapter()
        page = MagicMock()

        # When: search
        result = await adapter.search(
            page, "https://weworkremotely.com/remote-jobs/search?term=engineer"
        )

        # Then: returns a list of JobListings
        assert isinstance(result, list), f"Expected list, got {type(result)}"
        for listing in result:
            assert isinstance(listing, JobListing), f"Expected JobListing, got {type(listing)}"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_listings_have_required_fields(self) -> None:
        """
        Given a mock page with WeWorkRemotely job results
        When search returns listings
        Then each listing has board, title, company, and url populated.
        """
        # Given: adapter + mock page
        adapter = WeWorkRemotelyAdapter()
        page = MagicMock()

        # When: search
        listings = await adapter.search(
            page, "https://weworkremotely.com/remote-jobs/search?term=engineer"
        )

        # Then: required fields populated
        for i, listing in enumerate(listings):
            assert listing.board == "weworkremotely", (
                f"Listing {i} board should be 'weworkremotely', got {listing.board!r}"
            )
            assert listing.title, f"Listing {i} title should be non-empty"
            assert listing.company, f"Listing {i} company should be non-empty"
            assert listing.url, f"Listing {i} url should be non-empty"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_respects_max_pages(self) -> None:
        """
        Given a search with max_pages=1
        When search is called
        Then only the first page of results is processed.
        """
        # Given: adapter + mock page
        adapter = WeWorkRemotelyAdapter()
        page = MagicMock()

        # When: search with max_pages=1
        listings = await adapter.search(
            page,
            "https://weworkremotely.com/remote-jobs/search?term=engineer",
            max_pages=1,
        )

        # Then: results are from a single page
        assert isinstance(listings, list), f"Expected list, got {type(listings)}"


# ---------------------------------------------------------------------------
# TestWeWorkRemotelyExtractDetail
# ---------------------------------------------------------------------------


class TestWeWorkRemotelyExtractDetail:
    """
    REQUIREMENT: extract_detail navigates to a listing and populates full_text.

    WHO: The pipeline runner enriching shallow listings with full JD text
    WHAT: (1) The system populates full_text with the job description body when it extracts details for a listing whose full_text is empty.
          (2) The system leaves the listing unchanged when extract_detail is called on a listing whose full_text is already populated.
          (3) The system returns the same listing object when extract_detail is called and mutates it in place instead of creating a copy.
    WHY: Full JD text is required for embedding and scoring — without it
         the RAG pipeline cannot compute meaningful similarity

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: WeWorkRemotelyAdapter.extract_detail
        Never: Patch extraction logic or full_text assignment
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_extract_detail_populates_full_text(self) -> None:
        """
        Given a shallow listing with empty full_text
        When extract_detail is called
        Then full_text is populated with the JD body.
        """
        # Given: shallow listing
        adapter = WeWorkRemotelyAdapter()
        listing = _make_listing(full_text="")
        page = MagicMock()

        # When: extract detail
        result = await adapter.extract_detail(page, listing)

        # Then: full_text populated
        assert result.full_text != "", f"Expected non-empty full_text, got: {result.full_text!r}"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_extract_detail_preserves_existing_full_text(self) -> None:
        """
        Given a listing with full_text already populated
        When extract_detail is called
        Then it returns the listing unchanged.
        """
        # Given: listing with full_text
        adapter = WeWorkRemotelyAdapter()
        listing = _make_listing(full_text="Already populated JD text")
        page = MagicMock()

        # When: extract detail
        result = await adapter.extract_detail(page, listing)

        # Then: unchanged
        assert result.full_text == "Already populated JD text", (
            f"full_text should be unchanged: {result.full_text!r}"
        )

    @_XFAIL
    @pytest.mark.asyncio
    async def test_extract_detail_returns_same_listing_object(self) -> None:
        """
        Given a listing object
        When extract_detail is called
        Then the same object is returned (mutated in place).
        """
        # Given: adapter + listing
        adapter = WeWorkRemotelyAdapter()
        listing = _make_listing()
        page = MagicMock()

        # When: extract detail
        result = await adapter.extract_detail(page, listing)

        # Then: same object
        assert result is listing, "Should return the same listing object, not a copy"
