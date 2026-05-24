"""
Indeed adapter extraction tests — high-volume board with aggressive bot detection.

Spec classes:
    TestIndeedAuthenticate — session verification and bot-detection handling
    TestIndeedSearch — search pagination and card extraction
    TestIndeedExtractDetail — full JD extraction from listing detail pages

All tests are xfail until the adapter is implemented. The specs define
the delivery contract — each test becomes a regression gate once the
adapter is delivered.

Indeed specifics:
    - High-volume board with aggressive bot detection (Akamai, DataDome)
    - May require stealth patches and careful throttling
    - CAPTCHA and challenge pages are common failure modes
    - Session persistence via cookie-based storage_state
"""

from __future__ import annotations

from unittest.mock import MagicMock

import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.adapters.indeed import IndeedAdapter
from jobsearch_rag.errors import ActionableError

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

_XFAIL = pytest.mark.xfail(
    reason="Indeed adapter not yet implemented",
)


def _make_listing(
    *,
    full_text: str = "",
    external_id: str = "indeed-001",
) -> JobListing:
    """Create a minimal Indeed JobListing for testing."""
    return JobListing(
        board="indeed",
        external_id=external_id,
        title="Senior Data Engineer",
        company="DataCo",
        location="New York, NY",
        url="https://www.indeed.com/viewjob?jk=indeed-001",
        full_text=full_text,
    )


# ---------------------------------------------------------------------------
# TestIndeedAuthenticate
# ---------------------------------------------------------------------------


class TestIndeedAuthenticate:
    """
    REQUIREMENT: Indeed session verification detects blocks and session expiry.

    WHO: The pipeline runner during the authenticate step
    WHAT: (1) The system completes authentication without error when the Indeed search page loads without detection.
          (2) The system raises an ActionableError suggesting a manual solve when authentication encounters a CAPTCHA challenge.
          (3) The system raises an ActionableError suggesting re-authentication when authentication is redirected to the Indeed login page because the session has expired.
    WHY: Indeed is aggressive about bot detection — an expired or
         blocked session must fail fast so the operator can intervene
         before wasting search quota

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: IndeedAdapter.authenticate
        Never: Patch detection logic internals
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_succeeds_with_valid_session(self) -> None:
        """
        Given a mock page that loads Indeed search without detection
        When authenticate is called
        Then it completes without error.
        """
        # Given: valid session page
        adapter = IndeedAdapter()
        page = MagicMock()

        # When: authenticate
        await adapter.authenticate(page)

        # Then: no error raised (implicit)

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_captcha_raises_with_manual_suggestion(self) -> None:
        """
        Given a page showing a CAPTCHA challenge
        When authenticate is called
        Then an ActionableError suggesting manual solve is raised.
        """
        # Given: CAPTCHA page
        adapter = IndeedAdapter()
        page = MagicMock()

        # When/Then: raises with CAPTCHA guidance
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        err = exc_info.value
        assert err.suggestion is not None, "Should include suggestion"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_login_redirect_raises_with_reauth_suggestion(self) -> None:
        """
        Given a page redirected to the Indeed login page
        When authenticate is called
        Then an ActionableError about session expiration is raised.
        """
        # Given: login redirect
        adapter = IndeedAdapter()
        page = MagicMock()

        # When/Then: raises session expired
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        err = exc_info.value
        assert err.suggestion is not None, "Should include re-auth suggestion"


# ---------------------------------------------------------------------------
# TestIndeedSearch
# ---------------------------------------------------------------------------


class TestIndeedSearch:
    """
    REQUIREMENT: search() navigates Indeed job search and extracts listings.

    WHO: The pipeline runner collecting listings from Indeed
    WHAT: (1) The system returns a list of JobListing objects when Indeed search is called on search results.
          (2) The system populates each Indeed listing with the board, title, company, location, and url fields.
          (3) The system processes only the first page of Indeed results when max_pages is set to 1.
    WHY: Indeed is the highest-volume general board — extraction must
         handle variable HTML structures and aggressive rate limiting

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: IndeedAdapter.search
        Never: Patch extraction logic internals
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_returns_job_listings(self) -> None:
        """
        Given a mock page with Indeed search results
        When search is called
        Then a list of JobListings is returned.
        """
        # Given: adapter + mock page
        adapter = IndeedAdapter()
        page = MagicMock()

        # When: search
        result = await adapter.search(page, "https://www.indeed.com/jobs?q=data+engineer")

        # Then: returns a list of JobListings
        assert isinstance(result, list), f"Expected list, got {type(result)}"
        for listing in result:
            assert isinstance(listing, JobListing), f"Expected JobListing, got {type(listing)}"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_listings_have_required_fields(self) -> None:
        """
        Given a mock page with Indeed job results
        When search returns listings
        Then each listing has board, title, company, location, and url populated.
        """
        # Given: adapter + mock page
        adapter = IndeedAdapter()
        page = MagicMock()

        # When: search
        listings = await adapter.search(page, "https://www.indeed.com/jobs?q=data+engineer")

        # Then: required fields populated
        for i, listing in enumerate(listings):
            assert listing.board == "indeed", (
                f"Listing {i} board should be 'indeed', got {listing.board!r}"
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
        adapter = IndeedAdapter()
        page = MagicMock()

        # When: search with max_pages=1
        listings = await adapter.search(
            page,
            "https://www.indeed.com/jobs?q=data+engineer",
            max_pages=1,
        )

        # Then: results are from a single page
        assert isinstance(listings, list), f"Expected list, got {type(listings)}"


# ---------------------------------------------------------------------------
# TestIndeedExtractDetail
# ---------------------------------------------------------------------------


class TestIndeedExtractDetail:
    """
    REQUIREMENT: extract_detail navigates to a listing and populates full_text.

    WHO: The pipeline runner enriching shallow listings with full JD text
    WHAT: (1) The system populates full_text with the job description body when full_text is empty.
          (2) The system leaves the listing unchanged when full_text is already populated.
          (3) The system returns the same listing object after mutating it in place.
    WHY: Full JD text is required for embedding and scoring — without it
         the RAG pipeline cannot compute meaningful similarity

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: IndeedAdapter.extract_detail
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
        adapter = IndeedAdapter()
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
        adapter = IndeedAdapter()
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
        adapter = IndeedAdapter()
        listing = _make_listing()
        page = MagicMock()

        # When: extract detail
        result = await adapter.extract_detail(page, listing)

        # Then: same object
        assert result is listing, "Should return the same listing object, not a copy"
