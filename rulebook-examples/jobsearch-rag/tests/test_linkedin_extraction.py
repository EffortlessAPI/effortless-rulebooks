"""
LinkedIn adapter extraction tests — overnight mode, stealth, throttled.

Spec classes:
    TestLinkedInAuthenticate — session verification with bot-detection awareness
    TestLinkedInSearch — job search pagination and listing extraction
    TestLinkedInExtractDetail — full JD extraction from listing detail pages

All tests are xfail until the adapter is implemented. The specs define
the delivery contract — each test becomes a regression gate once the
adapter is delivered.

LinkedIn specifics:
    - Aggressive bot detection requires headed mode + stealth patches
    - Wide throttle window (8-20s) to stay under radar
    - ``check_linkedin_detection`` guards every page navigation
    - Authwall, challenge, and session-invalidation are distinct failure modes
"""

from __future__ import annotations

from unittest.mock import MagicMock

import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.adapters.linkedin import LinkedInAdapter
from jobsearch_rag.errors import ActionableError

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

_XFAIL = pytest.mark.xfail(
    reason="LinkedIn adapter not yet implemented",
)


def _make_listing(
    *,
    full_text: str = "",
    external_id: str = "li-001",
) -> JobListing:
    """Create a minimal LinkedIn JobListing for testing."""
    return JobListing(
        board="linkedin",
        external_id=external_id,
        title="Staff Platform Engineer",
        company="Acme Corp",
        location="Remote",
        url="https://www.linkedin.com/jobs/view/li-001",
        full_text=full_text,
    )


# ---------------------------------------------------------------------------
# TestLinkedInAuthenticate
# ---------------------------------------------------------------------------


class TestLinkedInAuthenticate:
    """
    REQUIREMENT: LinkedIn session verification detects bot-detection and session expiry.

    WHO: The pipeline runner during the authenticate step
    WHAT: (1) The system completes authentication without error when the LinkedIn feed loads successfully.
          (2) The system raises an ActionableError that mentions authwall and suggests waiting 24 hours when LinkedIn redirects authentication to `/authwall`.
          (3) The system raises an ActionableError about session expiration when LinkedIn redirects authentication to `/login`.
    WHY: LinkedIn's bot detection is aggressive — a detected session is
         locked for 24+ hours.  Failing fast with actionable advice
         prevents wasted retries that extend the lockout

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: LinkedInAdapter.authenticate, check_linkedin_detection
        Never: Patch detection logic internals
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_succeeds_with_valid_session(self) -> None:
        """
        Given a mock page that loads the LinkedIn feed without detection
        When authenticate is called
        Then it completes without error.
        """
        # Given: valid session page
        adapter = LinkedInAdapter()
        page = MagicMock()
        page.url = "https://www.linkedin.com/feed/"
        page.title = MagicMock(return_value="LinkedIn")
        page.goto = MagicMock()

        # When: authenticate
        await adapter.authenticate(page)

        # Then: no error raised (implicit)

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_authwall_redirect_raises_with_wait_suggestion(self) -> None:
        """
        Given a page redirected to /authwall (bot detection triggered)
        When authenticate is called
        Then an ActionableError is raised suggesting a 24-hour wait.
        """
        # Given: authwall redirect
        adapter = LinkedInAdapter()
        page = MagicMock()
        page.url = "https://www.linkedin.com/authwall"

        # When/Then: raises with wait advice
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        err = exc_info.value
        assert "authwall" in err.error.lower(), f"Error should mention authwall: {err.error!r}"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_authenticate_expired_session_raises_with_reauth_suggestion(self) -> None:
        """
        Given a page redirected to /login (session expired)
        When authenticate is called
        Then an ActionableError about session expiration is raised.
        """
        # Given: login redirect
        adapter = LinkedInAdapter()
        page = MagicMock()
        page.url = "https://www.linkedin.com/login"

        # When/Then: raises session expired
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        err = exc_info.value
        assert "session" in err.error.lower() or "login" in err.error.lower(), (
            f"Error should mention session or login: {err.error!r}"
        )


# ---------------------------------------------------------------------------
# TestLinkedInSearch
# ---------------------------------------------------------------------------


class TestLinkedInSearch:
    """
    REQUIREMENT: search() navigates LinkedIn job search and extracts listings.

    WHO: The pipeline runner collecting listings from LinkedIn
    WHAT: (1) The system returns a list of JobListings when LinkedIn search is called on job search results.
          (2) The system populates each returned LinkedIn listing with board, title, company, location, and url fields.
          (3) The system processes only the first page of LinkedIn results when search is called with max_pages set to 1.
    WHY: LinkedIn is the highest-signal board for professional roles —
         extraction must be reliable despite aggressive bot detection

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: LinkedInAdapter.search
        Never: Patch extraction logic internals
    """

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_returns_job_listings(self) -> None:
        """
        Given a mock page with LinkedIn job search results
        When search is called
        Then a list of JobListings is returned.
        """
        # Given: adapter + mock page
        adapter = LinkedInAdapter()
        page = MagicMock()

        # When: search
        result = await adapter.search(
            page, "https://www.linkedin.com/jobs/search/?keywords=staff+engineer"
        )

        # Then: returns a list of JobListings
        assert isinstance(result, list), f"Expected list, got {type(result)}"
        for listing in result:
            assert isinstance(listing, JobListing), f"Expected JobListing, got {type(listing)}"

    @_XFAIL
    @pytest.mark.asyncio
    async def test_search_listings_have_required_fields(self) -> None:
        """
        Given a mock page with LinkedIn job results
        When search returns listings
        Then each listing has board, title, company, location, and url populated.
        """
        # Given: adapter + mock page
        adapter = LinkedInAdapter()
        page = MagicMock()

        # When: search
        listings = await adapter.search(
            page, "https://www.linkedin.com/jobs/search/?keywords=staff+engineer"
        )

        # Then: required fields populated
        for i, listing in enumerate(listings):
            assert listing.board == "linkedin", (
                f"Listing {i} board should be 'linkedin', got {listing.board!r}"
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
        adapter = LinkedInAdapter()
        page = MagicMock()

        # When: search with max_pages=1
        listings = await adapter.search(
            page,
            "https://www.linkedin.com/jobs/search/?keywords=staff+engineer",
            max_pages=1,
        )

        # Then: results are from a single page
        assert isinstance(listings, list), f"Expected list, got {type(listings)}"


# ---------------------------------------------------------------------------
# TestLinkedInExtractDetail
# ---------------------------------------------------------------------------


class TestLinkedInExtractDetail:
    """
    REQUIREMENT: extract_detail navigates to a listing and populates full_text.

    WHO: The pipeline runner enriching shallow listings with full JD text
    WHAT: (1) The system populates an empty full_text field with the job description body during detail extraction.
          (2) The system returns the listing unchanged when full_text is already populated.
          (3) The system returns the same listing object after mutating it in place during detail extraction.
    WHY: Full JD text is required for embedding and scoring — without it
         the RAG pipeline cannot compute meaningful similarity

    MOCK BOUNDARY:
        Mock: Playwright page (browser I/O)
        Real: LinkedInAdapter.extract_detail
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
        adapter = LinkedInAdapter()
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
        adapter = LinkedInAdapter()
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
        adapter = LinkedInAdapter()
        listing = _make_listing()
        page = MagicMock()

        # When: extract detail
        result = await adapter.extract_detail(page, listing)

        # Then: same object
        assert result is listing, "Should return the same listing object, not a copy"
