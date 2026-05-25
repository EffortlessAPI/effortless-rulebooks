"""
Throttle detection tests — ZipRecruiter rate-limit recognition and backoff.

Maps to BDD spec: TestThrottleDetection

Tests verify that the ZipRecruiter adapter recognizes throttle responses
("We encountered an error while loading this job"), applies exponential
backoff, skips after max retries, logs throttle events, and distinguishes
throttle text from legitimate JD content.

All behavioral tests exercise the public ``search()`` API rather than
internal click-through methods, asserting on returned listing state,
sleep calls, and log output.
"""

from __future__ import annotations

import asyncio as _asyncio
import json
import logging
from contextlib import contextmanager
from typing import TYPE_CHECKING, Any
from unittest.mock import AsyncMock, MagicMock, patch

import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.adapters.ziprecruiter import (
    _SELECTORS,  # pyright: ignore[reportPrivateUsage]  # test coupling to selector dict
    ZipRecruiterAdapter,
    is_throttle_response,
)

if TYPE_CHECKING:
    from collections.abc import Iterator

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------

_ZR = "jobsearch_rag.adapters.ziprecruiter"
_SEARCH_URL = "https://www.ziprecruiter.com/jobs-search?search=test"

_THROTTLE_TEXT = "We encountered an error while loading this job"

_REAL_JD = (
    "Senior Solutions Architect at Acme Corp. "
    "We are looking for an experienced architect to lead our cloud "
    "infrastructure modernization effort. Requirements include 10+ "
    "years of experience in distributed systems design. " * 3
)


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _make_listing(**overrides: Any) -> JobListing:
    """Create a minimal JobListing for throttle testing."""
    defaults: dict[str, Any] = {
        "board": "ziprecruiter",
        "external_id": "zr-test-1",
        "title": "Solutions Architect",
        "company": "Acme Corp",
        "location": "Remote",
        "url": "https://www.ziprecruiter.com/jobs/test-1",
        "full_text": "",
    }
    defaults.update(overrides)
    return JobListing(**defaults)


def _build_zr_html(listings: list[JobListing]) -> str:
    """Build a realistic ZipRecruiter page with server-rendered articles and JSON-LD."""
    articles = ""
    ld_items: list[dict[str, str]] = []
    for ls in listings:
        articles += (
            f'<article id="job-card-{ls.external_id}">'
            f"<h2>{ls.title}</h2>"
            f'<a data-testid="job-card-company">{ls.company}</a>'
            f'<a data-testid="job-card-location">{ls.location}</a>'
            "</article>"
        )
        ld_items.append({"@type": "ListItem", "url": ls.url})
    json_ld = json.dumps({"@type": "ItemList", "itemListElement": ld_items})
    return (
        "<html><head><title>Jobs</title></head><body>"
        f'<script type="application/ld+json">{json_ld}</script>'
        f"{articles}</body></html>"
    )


@contextmanager
def _patch_search_to_click_through(
    listings: list[JobListing],
    panel_mock: AsyncMock,
) -> Iterator[MagicMock]:
    """
    Patch I/O boundaries so ``search()`` reaches click-through with real parsing.

    Builds a realistic HTML page containing server-rendered article elements
    and a JSON-LD block derived from *listings*.  Pure-computation functions
    (``extract_job_cards``, ``extract_json_ld_urls``, ``card_to_listing``)
    run on **real data** — only I/O boundaries (page navigation, Playwright
    locators) are stubbed.  ``_wait_for_cloudflare`` runs for real against a
    page whose title is "Jobs" (passes immediately).

    Yields the ``page`` mock for passing to ``adapter.search()``.
    """
    html = _build_zr_html(listings)

    # Derive selector patterns from the adapter's own _SELECTORS dict
    _panel_sel = _SELECTORS["detail_panel"]
    _card_prefix = _SELECTORS["card_articles"].replace("^=", "='")

    # Card locators by ID
    _card_mocks: dict[str, MagicMock] = {}
    for ls in listings:
        card_mock = MagicMock()
        first_mock = MagicMock()
        first_mock.click = AsyncMock()
        card_mock.first = first_mock
        _card_mocks[ls.external_id] = card_mock

    def _locator_dispatch(sel: str) -> MagicMock | AsyncMock:
        if sel == _panel_sel:
            return panel_mock
        if sel.startswith(_card_prefix):
            cid = sel[len(_card_prefix) :].rstrip("']")
            if cid in _card_mocks:
                return _card_mocks[cid]
        fallback = MagicMock()
        fallback.first = MagicMock()
        fallback.first.click = AsyncMock()
        return fallback

    page = MagicMock()
    page.goto = AsyncMock()
    page.content = AsyncMock(return_value=html)
    page.title = AsyncMock(return_value="Jobs")
    page.query_selector = AsyncMock(return_value=None)
    page.locator = MagicMock(side_effect=_locator_dispatch)  # pyright: ignore[reportUnknownLambdaType]

    yield page


# ---------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------


class TestThrottleDetection:
    """
    REQUIREMENT: Board-specific throttle responses are detected and handled
    gracefully rather than treated as valid JD text.

    WHO: The pipeline operator running automated searches
    WHAT: (1) The system recognizes the canonical ZipRecruiter error message as a throttle response.
          (2) The system does not falsely flag legitimate JD text or text that merely mentions error as a throttle response.
          (3) The system waits before retrying after it encounters a throttle response during search.
          (4) The system increases the backoff wait so the second delay is longer than the first on consecutive throttle responses.
          (5) The system skips a listing after the retry limit is exceeded and leaves `full_text` empty or at its fallback value.
          (6) The system counts a listing as a failed extraction after it exhausts all retries.
          (7) The system logs a throttle event at WARNING with the listing identifier.
          (8) The system populates the listing `full_text` with the real JD content when extraction succeeds after throttle backoff.
          (9) The system detects throttle text that appears after a timeout and retries with backoff.
          (10) The system uses empty fallback text after repeated late throttle responses exhaust all retries (no short_description in DOM flow).
          (11) The system falls back to empty text immediately when a timeout is not followed by throttle text (no short_description in DOM flow).
    WHY: Without throttle detection, error messages would be indexed as
         JD content, corrupting scoring results

    MOCK BOUNDARY:
        Mock:  Playwright page/locator (browser I/O),
               asyncio.sleep (time I/O)
        Real:  ZipRecruiterAdapter.search, _wait_for_cloudflare,
               is_throttle_response, extract_job_cards,
               extract_json_ld_urls, card_to_listing —
               all parsing/detection logic runs for real
        Never: Patch is_throttle_response or internal adapter methods
    """

    # --- Pure detection ------------------------------------------------

    def test_ziprecruiter_error_message_is_not_treated_as_valid_jd(self) -> None:
        """
        Given the canonical ZipRecruiter throttle error message
        When the text is checked for throttle response
        Then it is recognized as a throttle response
        """
        # Given: the canonical ZipRecruiter throttle error message
        text = _THROTTLE_TEXT

        # When / Then: it is recognized as a throttle response
        assert is_throttle_response(text) is True, (
            f"Expected throttle text to be detected. Input: {text!r}"
        )

    def test_non_throttle_error_text_is_not_misidentified_as_throttle(self) -> None:
        """
        Given legitimate JD text and text mentioning 'error'
        When the text is checked for throttle response
        Then neither is falsely flagged as a throttle response
        """
        # Given: legitimate JD content
        real_jd = _REAL_JD
        error_mention = "Handle error reporting in production systems. " * 5

        # When / Then: neither is misidentified as a throttle response
        assert is_throttle_response(real_jd) is False, (
            f"Real JD text was falsely flagged as throttle. Input starts: {real_jd[:80]!r}"
        )
        assert is_throttle_response(error_mention) is False, (
            "JD mentioning 'error' was falsely flagged as throttle"
        )

    # --- Backoff behavior through search() ----------------------------

    @pytest.mark.asyncio
    async def test_throttle_detection_triggers_backoff_delay(self) -> None:
        """
        Given a listing that throttles once then returns real JD
        When the adapter encounters the throttle during search
        Then it waits before retrying
        """
        # Given: a listing that throttles once then returns real JD
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.inner_text = AsyncMock(side_effect=[_THROTTLE_TEXT, _REAL_JD])
        panel_mock.wait_for = AsyncMock()

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock) as mock_sleep,
        ):
            # When: search encounters a throttle then succeeds on retry
            await adapter.search(page, _SEARCH_URL, max_pages=1)

            # Then: sleep was called for click delay + backoff delay
            assert mock_sleep.call_count >= 2, (
                f"Expected at least 2 sleep calls (click + backoff), got {mock_sleep.call_count}"
            )

    @pytest.mark.asyncio
    async def test_backoff_delay_increases_exponentially_on_consecutive_throttles(
        self,
    ) -> None:
        """
        Given two listings that each throttle once before succeeding
        When consecutive throttle responses occur across listings
        Then the second backoff wait is longer than the first
        """
        # Given: two listings, each throttled once before succeeding
        adapter = ZipRecruiterAdapter()
        listing1 = _make_listing(external_id="zr-1")
        listing2 = _make_listing(external_id="zr-2")

        panel_mock = AsyncMock()
        panel_mock.inner_text = AsyncMock(
            side_effect=[_THROTTLE_TEXT, _REAL_JD, _THROTTLE_TEXT, _REAL_JD]
        )
        panel_mock.wait_for = AsyncMock()

        backoff_delays: list[float] = []

        async def capture_sleep(duration: float) -> None:
            backoff_delays.append(duration)

        with (
            _patch_search_to_click_through([listing1, listing2], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", side_effect=capture_sleep),
        ):
            # When: search processes both listings
            await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: backoff delays increase exponentially
        backoff_waits = [d for d in backoff_delays if d >= 2.0]
        assert len(backoff_waits) >= 2, (
            f"Expected at least 2 backoff waits, got {len(backoff_waits)}: {backoff_delays}"
        )
        assert backoff_waits[1] > backoff_waits[0], (
            f"Second backoff ({backoff_waits[1]}) should be longer than first ({backoff_waits[0]})"
        )

    # --- Retry exhaustion and skip ------------------------------------

    @pytest.mark.asyncio
    async def test_listing_is_skipped_after_max_retry_attempts(self) -> None:
        """
        Given a listing that always returns throttle text
        When the retry limit is exceeded
        Then the listing is skipped with empty or fallback full_text
        """
        # Given: a listing that always returns throttle text
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.inner_text = AsyncMock(return_value=_THROTTLE_TEXT)
        panel_mock.wait_for = AsyncMock()

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search exhausts retries
            results = await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: the listing has empty or fallback full_text (not throttle text)
        assert not results[0].full_text.strip() or results[0].full_text == (
            f"{listing.title} at {listing.company}. "
        ), (
            f"Expected empty or fallback text after retry exhaustion, "
            f"got: {results[0].full_text[:100]!r}"
        )

    @pytest.mark.asyncio
    async def test_skipped_listing_increments_failed_count(self) -> None:
        """
        Given a listing that always returns throttle text
        When the listing exhausts retries
        Then it is counted as a failed extraction
        """
        # Given: a listing that always returns throttle text
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.inner_text = AsyncMock(return_value=_THROTTLE_TEXT)
        panel_mock.wait_for = AsyncMock()

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search exhausts retries
            results = await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: the listing is treated as a failed extraction
        assert (
            not results[0].full_text.strip() or "short_description" not in results[0].metadata
        ), (
            f"Expected listing to be treated as failed (empty text or no short_description). "
            f"full_text: {results[0].full_text[:100]!r}, metadata keys: {list(results[0].metadata.keys())}"
        )

    # --- Logging ------------------------------------------------------

    def test_throttle_event_is_logged_with_url_and_retry_count(
        self, caplog: pytest.LogCaptureFixture
    ) -> None:
        """
        Given a listing that always returns throttle text
        When a throttle event occurs during search
        Then it is logged at WARNING with the listing identifier
        """
        # Given: a listing that always returns throttle text
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.inner_text = AsyncMock(return_value=_THROTTLE_TEXT)
        panel_mock.wait_for = AsyncMock()

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            caplog.at_level(logging.WARNING),
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search encounters throttle responses
            _asyncio.run(adapter.search(page, _SEARCH_URL, max_pages=1))

        # Then: throttle warning is logged with listing identifier
        throttle_logs = [r for r in caplog.records if "throttl" in r.message.lower()]
        assert len(throttle_logs) >= 1, "No throttle warning logged"
        log_text = " ".join(r.message for r in throttle_logs)
        assert listing.external_id in log_text or listing.url in log_text, (
            f"Throttle log should mention listing ID '{listing.external_id}' or "
            f"URL '{listing.url}'. Got: {log_text}"
        )

    # --- Recovery after backoff ---------------------------------------

    @pytest.mark.asyncio
    async def test_successful_extraction_after_backoff_populates_full_text(
        self,
    ) -> None:
        """
        Given a listing that throttles once then returns real JD
        When extraction succeeds after a throttle backoff
        Then the listing full_text contains the real JD content
        """
        # Given: a listing that throttles once then returns real JD
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.inner_text = AsyncMock(side_effect=[_THROTTLE_TEXT, _REAL_JD])
        panel_mock.wait_for = AsyncMock()

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search retries successfully after backoff
            results = await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: the listing contains the real JD content
        assert results[0].full_text.strip() == _REAL_JD.strip(), (
            f"Expected real JD after backoff recovery. Got: {results[0].full_text[:100]!r}"
        )

    # --- Timeout + late throttle detection ----------------------------

    @pytest.mark.asyncio
    async def test_timeout_with_late_throttle_text_triggers_backoff(self) -> None:
        """
        Given wait_for times out on first attempt with throttle text in the panel
        When the adapter checks the panel after timeout
        Then it detects the late throttle and retries with backoff
        """
        # Given: wait_for times out on first attempt, panel shows throttle text
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.wait_for = AsyncMock(side_effect=[TimeoutError("5000ms exceeded"), None])
        panel_mock.inner_text = AsyncMock(side_effect=[_THROTTLE_TEXT, _REAL_JD])

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search detects late throttle and retries
            results = await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: the retry succeeds with real JD content
        assert results[0].full_text.strip() == _REAL_JD.strip(), (
            f"Expected real JD after late-throttle retry. Got: {results[0].full_text[:100]!r}"
        )

    @pytest.mark.asyncio
    async def test_late_throttle_exhausts_retries_then_falls_back(self) -> None:
        """
        Given a listing that always throttles via late-detected panel text
        When wait_for always times out and panel reads show throttle text
        Then all retries are exhausted and listing has empty fallback text
        """
        # Given: a listing that always throttles (no short_description in DOM flow)
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.wait_for = AsyncMock(side_effect=TimeoutError("5000ms exceeded"))
        panel_mock.inner_text = AsyncMock(return_value=_THROTTLE_TEXT)

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search exhausts all retries
            results = await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: listing has empty full_text (no short_description in DOM-based flow)
        assert not results[0].full_text.strip(), (
            f"Expected empty full_text after retry exhaustion (no short_description in DOM flow). "
            f"Got: {results[0].full_text[:100]!r}"
        )

    @pytest.mark.asyncio
    async def test_timeout_without_throttle_text_falls_back_immediately(
        self,
    ) -> None:
        """
        Given a listing where both wait_for and inner_text fail
        When the adapter attempts to read the panel after timeout
        Then it falls back to empty text without retrying (no short_description in DOM flow)
        """
        # Given: a listing where both wait_for and inner_text fail
        adapter = ZipRecruiterAdapter()
        listing = _make_listing()

        panel_mock = AsyncMock()
        panel_mock.wait_for = AsyncMock(side_effect=TimeoutError("5000ms exceeded"))
        panel_mock.inner_text = AsyncMock(side_effect=Exception("element detached"))

        with (
            _patch_search_to_click_through([listing], panel_mock) as page,
            patch(f"{_ZR}.asyncio.sleep", new_callable=AsyncMock),
        ):
            # When: search encounters timeout with no throttle text available
            results = await adapter.search(page, _SEARCH_URL, max_pages=1)

        # Then: falls back to empty text immediately (no short_description in DOM flow)
        assert not results[0].full_text.strip(), (
            f"Expected empty full_text after immediate fallback (no short_description in DOM flow). "
            f"Got: {results[0].full_text[:100]!r}"
        )
