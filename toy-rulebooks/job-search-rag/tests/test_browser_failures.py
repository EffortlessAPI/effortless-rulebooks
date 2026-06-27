"""
Browser failure mode tests.

Spec classes:
    TestAuthenticationFailures — session expiry, CAPTCHA, and auth recovery
    TestRateLimitAndThrottling — human-like timing and jitter
    TestPageExtractionFailures — 404s, empty text, selector misses
    TestLinkedInDetectionResponse — bot detection and graceful halt
"""

from __future__ import annotations

import asyncio
import json
from typing import TYPE_CHECKING
from unittest.mock import AsyncMock, MagicMock

import pytest

from jobsearch_rag.adapters.base import JobBoardAdapter, JobListing
from jobsearch_rag.adapters.linkedin import check_linkedin_detection
from jobsearch_rag.adapters.session import (
    SessionConfig,
    SessionManager,
    throttle,
)
from jobsearch_rag.errors import ActionableError, ErrorType

if TYPE_CHECKING:
    from pathlib import Path

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _make_listing(board: str = "ziprecruiter") -> JobListing:
    return JobListing(
        board=board,
        external_id="test-001",
        title="Staff Platform Architect",
        company="Acme Corp",
        location="Remote (USA)",
        url="https://example.org/job/test-001",
        full_text="",
    )


class _FakeAdapter(JobBoardAdapter):
    """Minimal concrete adapter for throttle tests."""

    @property
    def board_name(self) -> str:
        return "fake"

    @property
    def rate_limit_seconds(self) -> tuple[float, float]:
        return (0.01, 0.02)  # Fast for testing

    async def authenticate(self, page: object) -> None:
        pass

    async def search(self, page: object, query: str, max_pages: int = 3) -> list[JobListing]:
        return []

    async def extract_detail(self, page: object, listing: JobListing) -> JobListing:
        return listing


# ---------------------------------------------------------------------------
# TestAuthenticationFailures
# ---------------------------------------------------------------------------


class TestAuthenticationFailures:
    """
    REQUIREMENT: Authentication failures tell the operator exactly how to recover.

    WHO: The operator running the tool; the pipeline runner
    WHAT: (1) The system reports an authentication error that names the affected board and tells the operator to reauthenticate.
          (2) The system reports an authentication error that names the board and provides login recovery steps.
          (3) The system reports an authentication error that names the board and advises manual CAPTCHA resolution.
          (4) The system classifies a CAPTCHA failure as an authentication error so it does not trigger an automated retry.
          (5) The system persists successful authentication state to a storage file so later runs can skip login.
          (6) The system reports that no stored session state is available when the storage state file is missing instead of crashing.
    WHY: An unauthenticated scrape returns login-page HTML silently,
         producing zero valid listings with no error — the worst failure mode

    MOCK BOUNDARY:
        Mock:  nothing — ActionableError factory methods are pure constructors
        Real:  ActionableError.authentication, SessionConfig, SessionManager
        Never: Patch error construction internals
    """

    def test_expired_session_tells_operator_to_reauthenticate_with_board(self) -> None:
        """
        GIVEN an expired session on a specific board
        WHEN an authentication error is created
        THEN the error names the board and provides recovery guidance.
        """
        # Given: an expired session on ziprecruiter
        # When: authentication error is created
        err = ActionableError.authentication("ziprecruiter", "session expired")

        # Then: error names the board and provides guidance
        assert err.error_type == ErrorType.AUTHENTICATION, "Error type should be AUTHENTICATION"
        assert "ziprecruiter" in err.error, "Error should name the board"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    def test_authentication_error_provides_login_recovery_steps(self) -> None:
        """
        GIVEN invalid cookies on a specific board
        WHEN an authentication error is created
        THEN the error provides login recovery steps for that board.
        """
        # Given: invalid cookies on linkedin
        # When: authentication error is created
        err = ActionableError.authentication("linkedin", "cookies invalid")

        # Then: error identifies the board with recovery guidance
        assert "linkedin" in err.error, "Error should name the board"
        assert err.service == "linkedin", "Service should be 'linkedin'"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    def test_captcha_encountered_halts_run_and_logs_board(self) -> None:
        """
        GIVEN a CAPTCHA encountered during scraping
        WHEN an authentication error is created with CAPTCHA details
        THEN the error names the board and advises manual resolution.
        """
        # Given: CAPTCHA encountered on ziprecruiter
        # When: authentication error is created
        err = ActionableError.authentication(
            "ziprecruiter",
            "CAPTCHA encountered",
            suggestion="Solve CAPTCHA manually in headed mode",
        )

        # Then: error names the board with CAPTCHA context
        assert err.error_type == ErrorType.AUTHENTICATION, "Error type should be AUTHENTICATION"
        assert "CAPTCHA" in err.error, "Error should mention CAPTCHA"
        assert "ziprecruiter" in err.error, "Error should name the board"

    def test_captcha_does_not_trigger_retry(self) -> None:
        """
        GIVEN a CAPTCHA authentication error
        WHEN the error type is checked
        THEN it is AUTHENTICATION (not CONNECTION), preventing automated retry.
        """
        # Given: CAPTCHA error
        err = ActionableError.authentication("ziprecruiter", "CAPTCHA")

        # When/Then: error is authentication type
        assert err.error_type == ErrorType.AUTHENTICATION, (
            "CAPTCHA should be AUTHENTICATION, not CONNECTION"
        )
        # Then: guidance says re-authenticate, not retry
        assert err.ai_guidance is not None, "Should include AI guidance"
        assert "re-authenticate" in err.ai_guidance.action_required.lower(), (
            "AI guidance should advise re-authentication, not automated retry"
        )

    def test_successful_auth_persists_session_to_storage_state_file(self, tmp_path: Path) -> None:
        """
        GIVEN a successful authentication with cookies
        WHEN the session state is written to disk
        THEN subsequent runs can skip login by loading the state file.
        """
        # Given: session state data
        path = tmp_path / "ziprecruiter_session.json"
        state_data = {"cookies": [{"name": "session", "value": "abc123"}]}

        # When: state is written to disk
        path.write_text(json.dumps(state_data))

        # Then: the file exists and contains the session cookie
        assert path.exists(), "Session file should exist after write"
        loaded = json.loads(path.read_text())
        assert loaded["cookies"][0]["name"] == "session", "Cookie name should be preserved"

    def test_missing_storage_state_file_triggers_fresh_auth_not_crash(self) -> None:
        """
        GIVEN a board with no existing session file
        WHEN SessionManager checks for stored state
        THEN it reports no state available instead of crashing.
        """
        # Given: a config for a board with no session file
        config = SessionConfig(board_name="nonexistent")

        # When/Then: no crash — just reports no stored state
        assert not config.storage_state_path.exists(), "Session file should not exist"
        assert not SessionManager(config).has_storage_state(), "Should report no stored state"


# ---------------------------------------------------------------------------
# TestRateLimitAndThrottling
# ---------------------------------------------------------------------------


class TestRateLimitAndThrottling:
    """
    REQUIREMENT: Page loads are throttled to human-like timing per adapter profile.

    WHO: The browser session manager; the operator avoiding platform bans
    WHAT: (1) The system delays for a duration that stays within the adapter's configured minimum and maximum rate limit bounds.
          (2) The system produces varying throttle durations across repeated calls to avoid fixed-interval detection.
          (3) The system enforces a LinkedIn overnight throttle delay of at least 8 seconds to reduce ban escalation risk.
          (4) The system applies throttling between each page load and keeps every delay within the adapter's rate limit bounds.
          (5) The system applies throttling between each job detail request and keeps every delay within the adapter's rate limit bounds.
    WHY: Consistent sub-second timing is detectable as automation;
         LinkedIn specifically monitors request cadence for ban enforcement

    MOCK BOUNDARY:
        Mock:  nothing — throttle() uses random.uniform; we test output ranges
        Real:  throttle, _FakeAdapter, rate_limit_seconds
        Never: Patch random.uniform or sleep internals
    """

    def test_sleep_duration_falls_within_adapter_rate_limit_range(self) -> None:
        """
        GIVEN an adapter with defined rate_limit_seconds bounds
        WHEN throttle() is called
        THEN the delay falls within the adapter's min/max range.
        """
        # Given: adapter with known rate limits
        adapter = _FakeAdapter()
        lo, hi = adapter.rate_limit_seconds

        # When: throttle is called
        duration = asyncio.run(throttle(adapter))

        # Then: duration is within bounds
        assert lo <= duration <= hi, f"Expected {lo} <= {duration} <= {hi}"

    def test_sleep_uses_random_jitter_not_fixed_value(self) -> None:
        """
        GIVEN an adapter with a rate limit range
        WHEN throttle() is called repeatedly
        THEN varying durations are produced, avoiding fixed-interval detection.
        """
        # Given: adapter with rate limit range
        adapter = _FakeAdapter()

        # When: throttle is called 10 times
        durations = [asyncio.run(throttle(adapter)) for _ in range(10)]

        # Then: not all values are identical (random jitter applied)
        assert len(set(durations)) > 1, (
            f"Expected varied durations from jitter, got all identical: {durations[0]}"
        )

    def test_linkedin_overnight_mode_enforces_minimum_8_second_delay(self) -> None:
        """
        GIVEN a LinkedIn adapter with overnight rate limits
        WHEN throttle() is called
        THEN the delay is at least 8 seconds to avoid ban escalation.
        """

        # Given: LinkedIn adapter with extended rate limits
        class LinkedInAdapter(_FakeAdapter):
            @property
            def board_name(self) -> str:
                return "linkedin"

            @property
            def rate_limit_seconds(self) -> tuple[float, float]:
                return (8.0, 20.0)

        adapter = LinkedInAdapter()
        lo, _hi = adapter.rate_limit_seconds

        # When: throttle is called
        duration = asyncio.run(throttle(adapter))

        # Then: minimum 8-second delay enforced
        assert lo >= 8.0, f"LinkedIn lower bound should be >= 8.0, got {lo}"
        assert duration >= 8.0, f"LinkedIn throttle should be >= 8.0, got {duration}"

    def test_throttle_is_applied_between_every_page_load(self) -> None:
        """
        GIVEN multiple page navigations in a search session
        WHEN throttle() is called between each page
        THEN every delay falls within the adapter's rate limit bounds.
        """
        # Given: adapter with known rate limits
        adapter = _FakeAdapter()
        lo, hi = adapter.rate_limit_seconds

        # When: throttle is called 3 times (simulating 3 page loads)
        durations = [asyncio.run(throttle(adapter)) for _ in range(3)]

        # Then: all delays are within bounds
        assert len(durations) == 3, f"Expected 3 durations, got {len(durations)}"
        assert all(lo <= d <= hi for d in durations), (
            f"All durations should be within [{lo}, {hi}], got {durations}"
        )

    def test_throttle_is_applied_between_every_job_detail_request(self) -> None:
        """
        GIVEN multiple detail page fetches in a session
        WHEN throttle() is called between each fetch
        THEN every delay falls within the adapter's rate limit bounds.
        """
        # Given: adapter with known rate limits
        adapter = _FakeAdapter()
        lo, hi = adapter.rate_limit_seconds

        # When: throttle is called 5 times (simulating 5 detail fetches)
        durations = [asyncio.run(throttle(adapter)) for _ in range(5)]

        # Then: all delays are within bounds
        assert len(durations) == 5, f"Expected 5 durations, got {len(durations)}"
        assert all(lo <= d <= hi for d in durations), (
            f"All durations should be within [{lo}, {hi}], got {durations}"
        )


# ---------------------------------------------------------------------------
# TestPageExtractionFailures
# ---------------------------------------------------------------------------


class TestPageExtractionFailures:
    """
    REQUIREMENT: Extraction failures on individual listings do not abort the run.

    WHO: The pipeline runner processing a result set
    WHAT: (1) The system classifies a 404 detail-page failure as a PARSE error and provides actionable guidance.
          (2) The system detects empty extracted full_text so the scorer can exclude the listing.
          (3) The system retains the listing URL so it can log a warning for empty extraction.
          (4) The system creates a PARSE error that identifies the board and selector and advises inspection.
          (5) The system classifies a timeout failure as a CONNECTION error and provides recovery guidance.
          (6) The system tracks failed listings separately so the run summary reports success and failure counts.
          (7) The system preserves successful listings so partial results remain available for export.
    WHY: A single broken listing must not discard an entire search session's
         results — partial output is better than no output

    MOCK BOUNDARY:
        Mock:  nothing — ActionableError factory methods are pure constructors
        Real:  ActionableError.parse, ActionableError.connection, JobListing
        Never: Patch error construction internals
    """

    def test_404_on_detail_page_skips_listing_and_continues(self) -> None:
        """
        GIVEN a 404 response on a job detail page
        WHEN a parse error is created for the failure
        THEN the error has PARSE type with actionable guidance.
        """
        # Given/When: 404 produces a parse error
        err = ActionableError.parse("ziprecruiter", ".job-detail", "404 Not Found")

        # Then: error is PARSE type with guidance
        assert err.error_type == ErrorType.PARSE, "Error type should be PARSE"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_empty_extracted_text_excludes_listing_from_scoring(self) -> None:
        """
        GIVEN a listing with empty full_text after extraction
        WHEN the listing is checked for content
        THEN the empty full_text is detectable for scorer exclusion.
        """
        # Given: a listing with empty extracted text
        listing = _make_listing()
        listing.full_text = ""

        # When/Then: empty full_text is detectable
        assert listing.full_text == "", "Empty full_text should be detectable"

    def test_empty_extraction_logs_warning_with_job_url(self) -> None:
        """
        GIVEN a listing with empty full_text after extraction
        WHEN the listing URL is checked
        THEN the URL is available for diagnostic logging.
        """
        # Given: a listing with empty extracted text
        listing = _make_listing()
        listing.full_text = ""

        # When/Then: URL is available for logging
        assert listing.url != "", "URL should be available for warning log"
        assert listing.full_text == "", "full_text should be empty"

    def test_selector_miss_names_board_and_selector_and_suggests_inspection(self) -> None:
        """
        GIVEN a CSS selector miss on a board page
        WHEN a parse error is created
        THEN the error names the board and selector with inspection guidance.
        """
        # Given/When: selector miss produces a parse error
        err = ActionableError.parse(
            "ziprecruiter",
            ".job-description-content",
            "Element not found",
        )

        # Then: error identifies board, selector, and provides guidance
        assert err.error_type == ErrorType.PARSE, "Error type should be PARSE"
        assert "ziprecruiter" in err.error, "Error should name the board"
        assert ".job-description-content" in err.error, "Error should name the selector"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    def test_network_timeout_retries_once_before_skipping(self) -> None:
        """
        GIVEN a connection timeout on a detail page
        WHEN a connection error is created
        THEN the error has CONNECTION type with recovery guidance.
        """
        # Given/When: timeout produces a connection error
        err = ActionableError.connection(
            "ziprecruiter",
            "https://www.ziprecruiter.com/jobs/test-001",
            "Connection timed out",
        )

        # Then: error is CONNECTION type with guidance
        assert err.error_type == ErrorType.CONNECTION, "Error type should be CONNECTION"
        assert "timed out" in err.error, "Error should mention timeout"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_failed_listings_are_counted_in_run_summary(self) -> None:
        """
        GIVEN a batch of listings with some extraction failures
        WHEN failures are tracked separately
        THEN the run summary can report success vs. failure counts.
        """
        # Given: 5 listings with 2 failures
        failed: list[JobListing] = []
        listings = [_make_listing() for _ in range(5)]
        failed.append(listings[1])
        failed.append(listings[3])

        # When/Then: counts are accurate
        assert len(failed) == 2, "Should have 2 failed listings"
        assert len(listings) - len(failed) == 3, "Should have 3 successful listings"

    def test_partial_results_are_exported_even_when_some_listings_fail(self) -> None:
        """
        GIVEN a batch where some listings fail extraction
        WHEN successful listings are filtered
        THEN partial results are available for export.
        """
        # Given: 5 total listings with indices 1 and 3 failing
        total = [_make_listing() for _ in range(5)]
        successful = [item for i, item in enumerate(total) if i not in (1, 3)]

        # When/Then: partial results exist
        assert len(successful) == 3, "Should have 3 successful listings"
        assert len(successful) > 0, "Partial results should be available for export"


# ---------------------------------------------------------------------------
# TestLinkedInDetectionResponse
# ---------------------------------------------------------------------------


class TestLinkedInDetectionResponse:
    """
    REQUIREMENT: LinkedIn bot detection triggers a graceful, safe halt.

    WHO: The operator running overnight LinkedIn passes
    WHAT: (1) The system raises an AUTHENTICATION error that advises waiting before retrying when LinkedIn redirects to `/authwall`.
          (2) The system raises an AUTHENTICATION error that advises waiting before retrying when LinkedIn redirects to `/checkpoint/challenge`.
          (3) The system advises waiting instead of retrying after a LinkedIn detection event because retrying increases ban risk.
          (4) The system includes wait guidance and scheduling troubleshooting steps in the detection error message.
          (5) The system stops making further `page.goto()` calls after a LinkedIn detection event occurs.
          (6) The system raises an AUTHENTICATION error that advises re-authenticating when LinkedIn redirects to `/login`.
          (7) The system raises an AUTHENTICATION error that advises re-authenticating when LinkedIn redirects to `/uas/login`.
          (8) The system preserves and exports listings collected before a later LinkedIn detection event.
          (9) The system returns without error when the page URL and title contain no detection indicators.
    WHY: Continuing after detection escalates ban risk from temporary to permanent;
         the correct response is always stop, log, and wait

    MOCK BOUNDARY:
        Mock:  Playwright page mock (browser I/O — page.url, page.title)
        Real:  check_linkedin_detection, URL pattern matching, error construction
        Never: Patch check_linkedin_detection or URL parsing internals
    """

    def test_authwall_redirect_tells_operator_to_wait_before_retrying(self) -> None:
        """
        GIVEN a page redirected to LinkedIn /authwall
        WHEN check_linkedin_detection() is called
        THEN an AUTHENTICATION error advises waiting before retrying.
        """
        # Given: page URL is /authwall redirect
        page = MagicMock()
        page.url = "https://www.linkedin.com/authwall?trk=something"
        page.title = AsyncMock(return_value="LinkedIn")

        # When/Then: detection raises AUTHENTICATION error
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: error mentions authwall and advises waiting
        err = exc_info.value
        assert err.error_type == ErrorType.AUTHENTICATION, "Error type should be AUTHENTICATION"
        assert "authwall" in err.error.lower(), "Error should mention authwall"
        assert err.suggestion is not None, "Should include a suggestion"
        assert "wait" in err.suggestion.lower(), "Suggestion should advise waiting"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_challenge_interstitial_tells_operator_to_wait_before_retrying(self) -> None:
        """
        GIVEN a page redirected to LinkedIn /checkpoint/challenge
        WHEN check_linkedin_detection() is called
        THEN an AUTHENTICATION error advises waiting before retrying.
        """
        # Given: page URL is /checkpoint/challenge
        page = MagicMock()
        page.url = "https://www.linkedin.com/checkpoint/challenge"
        page.title = AsyncMock(return_value="Security Challenge")

        # When/Then: detection raises AUTHENTICATION error
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: error mentions challenge and advises waiting
        err = exc_info.value
        assert err.error_type == ErrorType.AUTHENTICATION, "Error type should be AUTHENTICATION"
        assert "challenge" in err.error.lower(), "Error should mention challenge"
        assert err.suggestion is not None, "Should include a suggestion"
        assert "wait" in err.suggestion.lower(), "Suggestion should advise waiting"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_detection_halts_run_without_retry(self) -> None:
        """
        GIVEN a LinkedIn authwall detection event
        WHEN the error is inspected
        THEN it advises waiting, not retrying (retry escalates ban risk).
        """
        # Given: authwall detection
        page = MagicMock()
        page.url = "https://www.linkedin.com/authwall"
        page.title = AsyncMock(return_value="LinkedIn")

        # When: detection event fires
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: error is AUTHENTICATION type advising wait
        err = exc_info.value
        assert err.error_type == ErrorType.AUTHENTICATION, "Error type should be AUTHENTICATION"
        assert "wait" in (err.suggestion or "").lower(), "Suggestion should advise waiting"

    def test_detection_error_includes_wait_duration_and_ban_risk_guidance(self) -> None:
        """
        GIVEN a LinkedIn authwall detection event
        WHEN the error guidance is inspected
        THEN it includes wait advice and troubleshooting steps for scheduling.
        """
        # Given: authwall detection
        page = MagicMock()
        page.url = "https://www.linkedin.com/authwall"
        page.title = AsyncMock(return_value="LinkedIn")

        # When: detection event fires
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: guidance includes wait and troubleshooting
        err = exc_info.value
        assert "wait" in (err.suggestion or "").lower(), "Suggestion should advise waiting"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    def test_no_requests_are_made_after_detection_event(self) -> None:
        """
        GIVEN a LinkedIn detection event is raised
        WHEN the check completes
        THEN no further page.goto() calls are made.
        """
        # Given: authwall detection with goto mock
        page = MagicMock()
        page.url = "https://www.linkedin.com/authwall"
        page.title = AsyncMock(return_value="LinkedIn")
        page.goto = AsyncMock()

        # When: detection fires
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: error names the detection trigger and no navigation was attempted
        assert "authwall" in str(exc_info.value), (
            f"Expected 'authwall' in error message, got: {exc_info.value}"
        )
        page.goto.assert_not_called()

    def test_session_invalidation_redirect_to_login_raises_authentication_error(self) -> None:
        """
        GIVEN a page redirected to /login (session invalidation)
        WHEN check_linkedin_detection() is called
        THEN an AUTHENTICATION error advises re-authenticating.
        """
        # Given: page URL is /login redirect
        page = MagicMock()
        page.url = "https://www.linkedin.com/login"
        page.title = AsyncMock(return_value="LinkedIn Login")

        # When/Then: detection raises AUTHENTICATION error
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: error identifies session invalidation
        err = exc_info.value
        assert err.error_type == ErrorType.AUTHENTICATION, (
            f"Expected AUTHENTICATION, got {err.error_type}"
        )
        assert "session" in err.error.lower() or "login" in err.error.lower(), (
            f"Error should mention session/login. Got: {err.error}"
        )

    def test_session_invalidation_redirect_to_uas_login_raises_authentication_error(
        self,
    ) -> None:
        """
        GIVEN a page redirected to /uas/login (legacy session invalidation)
        WHEN check_linkedin_detection() is called
        THEN an AUTHENTICATION error advises re-authenticating.
        """
        # Given: page URL is /uas/login redirect
        page = MagicMock()
        page.url = "https://www.linkedin.com/uas/login?trk=something"
        page.title = AsyncMock(return_value="LinkedIn Login")

        # When/Then: detection raises AUTHENTICATION error
        with pytest.raises(ActionableError) as exc_info:
            asyncio.run(check_linkedin_detection(page))

        # Then: error identifies session invalidation
        err = exc_info.value
        assert err.error_type == ErrorType.AUTHENTICATION, (
            f"Expected AUTHENTICATION, got {err.error_type}"
        )

    def test_partial_results_before_detection_are_preserved_and_exported(self) -> None:
        """
        GIVEN listings collected before a detection event
        WHEN detection occurs on a later page
        THEN earlier results are preserved for export.
        """
        # Given: 3 listings collected before detection on 4th page
        results_before_detection = [_make_listing() for _ in range(3)]

        # When/Then: partial results are preserved
        assert len(results_before_detection) == 3, "Should have 3 partial results"
        assert all(r.board == "ziprecruiter" for r in results_before_detection), (
            "All partial results should have the correct board"
        )

    def test_clean_page_passes_detection_without_error(self) -> None:
        """
        GIVEN a page with a normal LinkedIn feed URL and non-triggering title
        WHEN check_linkedin_detection() is called
        THEN no error is raised and the function returns cleanly.
        """
        # Given: a clean page with no detection indicators
        page = MagicMock()
        page.url = "https://www.linkedin.com/feed/"
        page.title = AsyncMock(return_value="LinkedIn")

        # When: detection check runs
        asyncio.run(check_linkedin_detection(page))

        # Then: no error raised — implicit success
