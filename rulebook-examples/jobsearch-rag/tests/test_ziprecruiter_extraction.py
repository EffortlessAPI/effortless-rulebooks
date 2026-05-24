"""
ZipRecruiter adapter extraction tests.

Spec classes:
    TestZipRecruiterDomExtraction — DOM-based extraction from Next.js SERP
    TestHtmlToText — HTML-to-plain-text conversion for embedding
    TestSalaryParsing — Salary text parsing into numeric ranges
    TestRealWorldExtraction — Regression guard against production HTML
    TestAuthenticate — Session verification and Cloudflare/CAPTCHA detection
    TestSearch — SERP navigation, card extraction, and click-through enrichment
    TestExtractDetailPassthrough — extract_detail passthrough when full_text populated

Validates the DOM-based extraction strategy against ZipRecruiter's
Next.js SERP structure where job data is rendered in server-side
``<article>`` elements with ``data-testid`` attributes, and URLs
are provided via JSON-LD structured data.
"""

from __future__ import annotations

from pathlib import Path
from typing import Any
from unittest.mock import AsyncMock, MagicMock, patch

import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.adapters.ziprecruiter import (
    _SELECTORS,  # pyright: ignore[reportPrivateUsage]  # test coupling to selector dict
    ZipRecruiterAdapter,
    card_to_listing,
    extract_job_cards,
    extract_json_ld_urls,
    html_to_text,
    parse_salary_text,
)
from jobsearch_rag.errors import ActionableError

# ---------------------------------------------------------------------------
# Fixture paths
# ---------------------------------------------------------------------------

_FIXTURES = Path(__file__).parent / "fixtures"
_SERP_FIXTURE = _FIXTURES / "ziprecruiter_results.html"
_JD_FIXTURE = _FIXTURES / "ziprecruiter_jd.html"
_REAL_FIXTURE = _FIXTURES / "ziprecruiter_serp_real.html"


# ---------------------------------------------------------------------------
# TestZipRecruiterDomExtraction
# ---------------------------------------------------------------------------


class TestZipRecruiterDomExtraction:
    """
    REQUIREMENT: Job data is extracted from ZipRecruiter's Next.js SERP DOM.

    WHO: The ZipRecruiter adapter extraction pipeline
    WHAT: (1) The system extracts 3 job cards from the synthetic SERP fixture.
          (2) The system deduplicates responsive-layout article duplicates by external_id.
          (3) The system extracts 3 JSON-LD URLs from the synthetic SERP fixture.
          (4) Each card maps correctly to a ``JobListing`` with the right title.
          (5) card_to_listing extracts the company from the card dict.
          (6) card_to_listing maps location from the card dict.
          (7) card_to_listing uses external_id as the listing external_id.
          (8) card_to_listing assigns the URL from matching JSON-LD data.
          (9) card_to_listing sets the board field to 'ziprecruiter'.
          (10) card_to_listing full_text is empty after extraction (populated during search).
          (11) salary metadata is included when salary text is present, omitted when absent.
          (12) card_to_listing omits salary metadata when salary_text is present but unparseable.
          (13) extract_job_cards returns empty list when no article elements exist.
          (14) extract_json_ld_urls returns empty list when no JSON-LD script exists.
          (15) extract_json_ld_urls handles malformed JSON gracefully.
    WHY: ZipRecruiter uses a Next.js SERP with server-rendered article
         elements and JSON-LD structured data — extraction must parse
         both data sources reliably.

    MOCK BOUNDARY:
        Mock:  (none — pure functions operating on fixture HTML)
        Real:  extract_job_cards, extract_json_ld_urls, card_to_listing,
               parse_salary_text
        Never: Patch extraction functions or fixture file contents
    """

    def test_extract_job_cards_from_synthetic_fixture(self) -> None:
        """
        GIVEN a synthetic SERP HTML fixture with 3 job cards (6 articles, 3 duplicates)
        WHEN extract_job_cards is called
        THEN exactly 3 deduplicated cards are returned.
        """
        # Given: synthetic fixture HTML
        html = _SERP_FIXTURE.read_text()

        # When: extract
        cards = extract_job_cards(html)

        # Then: 3 deduplicated cards
        assert len(cards) == 3, f"Expected 3 cards, got {len(cards)}"

    def test_extract_job_cards_deduplicates_responsive_articles(self) -> None:
        """
        GIVEN HTML with two articles sharing the same job-card ID
        WHEN extract_job_cards is called
        THEN only one card is returned for that ID.
        """
        # Given: HTML with duplicate article IDs
        html = """
        <article id="job-card-dup1"><h2>Role A</h2>
        <a data-testid="job-card-company">Co A</a>
        <a data-testid="job-card-location">Remote</a></article>
        <article id="job-card-dup1"><h2>Role A</h2>
        <a data-testid="job-card-company">Co A</a>
        <a data-testid="job-card-location">Remote</a></article>
        """

        # When: extract
        cards = extract_job_cards(html)

        # Then: 1 card (deduplicated)
        assert len(cards) == 1, f"Expected 1 deduplicated card, got {len(cards)}"
        assert cards[0]["external_id"] == "dup1", (
            f"Expected 'dup1', got {cards[0]['external_id']!r}"
        )

    def test_extract_json_ld_urls_from_synthetic_fixture(self) -> None:
        """
        GIVEN a synthetic SERP fixture with JSON-LD containing 3 items
        WHEN extract_json_ld_urls is called
        THEN 3 URLs are returned in position order.
        """
        # Given: synthetic fixture HTML
        html = _SERP_FIXTURE.read_text()

        # When: extract
        urls = extract_json_ld_urls(html)

        # Then: 3 URLs
        assert len(urls) == 3, f"Expected 3 URLs, got {len(urls)}"
        assert "Acme-Corp" in urls[0], f"First URL should contain Acme-Corp: {urls[0]!r}"

    def test_card_to_listing_maps_title(self) -> None:
        """
        GIVEN a card dict from extract_job_cards with a title
        WHEN card_to_listing is called
        THEN the title maps to 'Staff Platform Architect'.
        """
        # Given: first card from fixture
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)
        urls = extract_json_ld_urls(html)
        cards[0]["url"] = urls[0]

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: title matches
        assert listing.title == "Staff Platform Architect", (
            f"Expected 'Staff Platform Architect', got {listing.title!r}"
        )

    def test_card_to_listing_maps_company(self) -> None:
        """
        GIVEN a card dict with a company field
        WHEN card_to_listing is called
        THEN the company name is mapped correctly.
        """
        # Given: first card from fixture
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: company matches
        assert listing.company == "Acme Corp", f"Expected 'Acme Corp', got {listing.company!r}"

    def test_card_to_listing_maps_location(self) -> None:
        """
        GIVEN a card dict with a location field
        WHEN card_to_listing is called
        THEN the location is mapped correctly.
        """
        # Given: first card from fixture
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: location matches
        assert listing.location == "Remote (USA)", (
            f"Expected 'Remote (USA)', got {listing.location!r}"
        )

    def test_card_to_listing_maps_external_id(self) -> None:
        """
        GIVEN a card dict with an external_id from the article ID
        WHEN card_to_listing is called
        THEN the external_id is mapped correctly.
        """
        # Given: first card from fixture
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: external_id matches article ID suffix
        assert listing.external_id == "abc123key", (
            f"Expected 'abc123key', got {listing.external_id!r}"
        )

    def test_card_to_listing_assigns_url_from_json_ld(self) -> None:
        """
        GIVEN a card dict with a URL added from JSON-LD matching
        WHEN card_to_listing is called
        THEN the URL is set correctly.
        """
        # Given: card with URL from JSON-LD
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)
        urls = extract_json_ld_urls(html)
        cards[0]["url"] = urls[0]

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: URL matches JSON-LD
        assert listing.url.startswith("https://www.ziprecruiter.com/c/Acme-Corp/"), (
            f"Unexpected URL prefix: {listing.url!r}"
        )

    def test_card_to_listing_sets_board_name(self) -> None:
        """
        GIVEN any card dict
        WHEN card_to_listing is called
        THEN board is set to 'ziprecruiter'.
        """
        # Given: first card from fixture
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: board is ziprecruiter
        assert listing.board == "ziprecruiter", f"Expected 'ziprecruiter', got {listing.board!r}"

    def test_card_to_listing_full_text_is_empty(self) -> None:
        """
        GIVEN a card dict from search results
        WHEN card_to_listing is called
        THEN full_text is empty (detail extraction is a separate step).
        """
        # Given: first card from fixture
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: empty full_text
        assert listing.full_text == "", f"Expected empty full_text, got {listing.full_text!r}"

    def test_card_to_listing_includes_salary_metadata(self) -> None:
        """
        GIVEN a card dict with salary text '$210K - $250K/yr'
        WHEN card_to_listing is called
        THEN salary_range is included in metadata.
        """
        # Given: first card from fixture (has salary)
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert
        listing = card_to_listing(cards[0])

        # Then: salary_range present with correct values
        assert "salary_range" in listing.metadata, "Missing salary_range metadata"
        assert "$210,000" in listing.metadata["salary_range"], (
            f"Missing $210,000 in salary_range: {listing.metadata['salary_range']!r}"
        )
        assert "$250,000" in listing.metadata["salary_range"], (
            f"Missing $250,000 in salary_range: {listing.metadata['salary_range']!r}"
        )

    def test_card_to_listing_omits_salary_when_missing(self) -> None:
        """
        GIVEN a card dict without salary text
        WHEN card_to_listing is called
        THEN salary_range is absent from metadata.
        """
        # Given: third card (no salary)
        html = _SERP_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert card without salary
        listing = card_to_listing(cards[2])

        # Then: no salary_range
        assert "salary_range" not in listing.metadata, (
            f"salary_range should be absent, got {listing.metadata.get('salary_range')!r}"
        )

    def test_card_to_listing_omits_salary_when_text_unparseable(self) -> None:
        """
        GIVEN a card dict with salary_text that does not match the salary regex
        WHEN card_to_listing is called
        THEN salary_range is absent, comp_min and comp_max are None.
        """
        # Given: card with unparseable salary text
        card = {
            "external_id": "unparseable1",
            "title": "Staff Engineer",
            "company": "Acme Corp",
            "location": "Remote",
            "url": "https://www.ziprecruiter.com/jobs/acme/staff",
            "salary_text": "Competitive salary",
        }

        # When: convert card
        listing = card_to_listing(card)

        # Then: no salary metadata, comp fields are None
        assert "salary_range" not in listing.metadata, (
            f"salary_range should be absent for unparseable text, "
            f"got {listing.metadata.get('salary_range')!r}"
        )
        assert listing.comp_min is None, f"comp_min should be None, got {listing.comp_min}"
        assert listing.comp_max is None, f"comp_max should be None, got {listing.comp_max}"

    def test_extract_job_cards_returns_empty_for_no_articles(self) -> None:
        """
        GIVEN HTML without any article elements
        WHEN extract_job_cards is called
        THEN an empty list is returned.
        """
        # Given: HTML with no articles
        html = "<html><body><p>No jobs here</p></body></html>"

        # When: extract
        cards = extract_job_cards(html)

        # Then: empty
        assert cards == [], f"Expected empty list, got {cards}"

    def test_extract_json_ld_urls_returns_empty_for_no_script(self) -> None:
        """
        GIVEN HTML without a JSON-LD script tag
        WHEN extract_json_ld_urls is called
        THEN an empty list is returned.
        """
        # Given: HTML with no JSON-LD
        html = "<html><body><p>No JSON-LD here</p></body></html>"

        # When: extract
        urls = extract_json_ld_urls(html)

        # Then: empty
        assert urls == [], f"Expected empty list, got {urls}"

    def test_extract_json_ld_urls_handles_malformed_json(self) -> None:
        """
        GIVEN HTML with a JSON-LD script tag containing invalid JSON
        WHEN extract_json_ld_urls is called
        THEN an empty list is returned gracefully.
        """
        # Given: malformed JSON-LD
        html = '<script type="application/ld+json">{not valid json</script>'

        # When: extract
        urls = extract_json_ld_urls(html)

        # Then: empty (graceful)
        assert urls == [], f"Expected empty list, got {urls}"


# ---------------------------------------------------------------------------
# TestHtmlToText
# ---------------------------------------------------------------------------


class TestHtmlToText:
    """
    REQUIREMENT: HTML job descriptions are converted to clean plain text.

    WHO: The JD extraction pipeline before RAG embedding
    WHAT: (1) The system removes all HTML tags and leaves only the text content.
          (2) The system collapses consecutive whitespace into single spaces.
          (3) The system preserves the text content of all list items in nested list HTML.
          (4) The system returns an empty string when given an empty HTML string.
          (5) The system returns plain text unchanged when no HTML tags are present.
    WHY: RAG embeddings work best on clean text — HTML artifacts degrade
         retrieval quality

    MOCK BOUNDARY:
        Mock:  (none — pure function tests)
        Real:  html_to_text
        Never: Patch html_to_text internals
    """

    def test_strips_html_tags(self) -> None:
        """
        GIVEN HTML with strong and em tags
        WHEN html_to_text is called
        THEN all tags are removed, leaving only text.
        """
        # Given: HTML with tags
        html = "<div><strong>Hello</strong> <em>world</em></div>"

        # When: convert
        result = html_to_text(html)

        # Then: no HTML tags remain
        assert "<" not in result, f"HTML tags should be stripped: {result!r}"
        assert "Hello" in result, "Missing 'Hello' in result"
        assert "world" in result, "Missing 'world' in result"

    def test_normalizes_whitespace(self) -> None:
        """
        GIVEN HTML with multiple consecutive whitespace characters
        WHEN html_to_text is called
        THEN whitespace is collapsed to single spaces.
        """
        # Given: HTML with excess whitespace
        html = "<p>Hello    world</p>"

        # When: convert
        result = html_to_text(html)

        # Then: normalized
        assert "  " not in result, f"Double spaces remain: {result!r}"
        assert "Hello world" in result, f"Expected 'Hello world' in: {result!r}"

    def test_handles_nested_lists(self) -> None:
        """
        GIVEN HTML with ul/li list items
        WHEN html_to_text is called
        THEN all list item text is preserved.
        """
        # Given: HTML list
        html = "<ul><li>Item one</li><li>Item two</li></ul>"

        # When: convert
        result = html_to_text(html)

        # Then: text preserved
        assert "Item one" in result, f"Missing 'Item one' in: {result!r}"
        assert "Item two" in result, f"Missing 'Item two' in: {result!r}"

    def test_handles_empty_string(self) -> None:
        """
        GIVEN an empty HTML string
        WHEN html_to_text is called
        THEN an empty string is returned.
        """
        # Then: empty in, empty out
        assert html_to_text("") == "", "Empty HTML should produce empty text"

    def test_handles_plain_text_passthrough(self) -> None:
        """
        GIVEN plain text without any HTML tags
        WHEN html_to_text is called
        THEN the text passes through unchanged.
        """
        # Given: plain text
        text = "Just plain text, nothing fancy."

        # Then: passthrough
        assert html_to_text(text) == text, "Plain text should pass through unchanged"


# ---------------------------------------------------------------------------
# TestSalaryParsing
# ---------------------------------------------------------------------------


class TestSalaryParsing:
    """
    REQUIREMENT: Salary text from SERP cards is parsed into numeric ranges.

    WHO: The adapter salary extraction pipeline
    WHAT: (1) The system parses '$185K - $240K/yr' into (185000, 240000).
          (2) The system parses decimal values like '$114.30K - $157K/yr'.
          (3) The system returns (None, None) when no salary pattern is found.
          (4) The system parses values without K suffix (e.g. '$60 - $100').
          (5) The system parses M suffix (e.g. '$1.5M - $2M/yr') into millions.
    WHY: Salary data appears as display text in ZipRecruiter's Next.js SERP
         cards — numeric parsing enables comp scoring and filtering

    MOCK BOUNDARY:
        Mock:  (none — pure function tests)
        Real:  parse_salary_text
        Never: Patch parse_salary_text internals
    """

    def test_parses_standard_k_suffix(self) -> None:
        """
        GIVEN salary text '$185K - $240K/yr'
        WHEN parse_salary_text is called
        THEN (185000.0, 240000.0) is returned.
        """
        min_val, max_val = parse_salary_text("$185K - $240K/yr")

        assert min_val == 185_000.0, f"Expected 185000, got {min_val}"
        assert max_val == 240_000.0, f"Expected 240000, got {max_val}"

    def test_parses_decimal_k_suffix(self) -> None:
        """
        GIVEN salary text '$114.30K - $157K/yr'
        WHEN parse_salary_text is called
        THEN (114300.0, 157000.0) is returned.
        """
        min_val, max_val = parse_salary_text("$114.30K - $157K/yr")

        assert min_val == 114_300.0, f"Expected 114300, got {min_val}"
        assert max_val == 157_000.0, f"Expected 157000, got {max_val}"

    def test_returns_none_for_no_pattern(self) -> None:
        """
        GIVEN text without a salary pattern
        WHEN parse_salary_text is called
        THEN (None, None) is returned.
        """
        min_val, max_val = parse_salary_text("No salary info here")

        assert min_val is None, f"Expected None, got {min_val}"
        assert max_val is None, f"Expected None, got {max_val}"

    def test_parses_values_without_suffix(self) -> None:
        """
        GIVEN salary text '$60 - $100' (hourly, no suffix)
        WHEN parse_salary_text is called
        THEN (60.0, 100.0) is returned.
        """
        min_val, max_val = parse_salary_text("$60 - $100")

        assert min_val == 60.0, f"Expected 60.0, got {min_val}"
        assert max_val == 100.0, f"Expected 100.0, got {max_val}"

    def test_parses_m_suffix(self) -> None:
        """
        GIVEN salary text '$1.5M - $2M/yr'
        WHEN parse_salary_text is called
        THEN (1500000.0, 2000000.0) is returned.
        """
        min_val, max_val = parse_salary_text("$1.5M - $2M/yr")

        assert min_val == 1_500_000.0, f"Expected 1500000, got {min_val}"
        assert max_val == 2_000_000.0, f"Expected 2000000, got {max_val}"


# ---------------------------------------------------------------------------
# TestRealWorldExtraction
# ---------------------------------------------------------------------------


class TestRealWorldExtraction:
    """
    REQUIREMENT: Extraction works correctly against real ZipRecruiter data.

    WHO: The adapter maintainer validating against production HTML
    WHAT: (1) The system produces exactly 20 listings from the real ZipRecruiter SERP fixture.
          (2) The system converts the first real SERP card into an Anchorage Digital listing with a platform engineering title.
          (3) The system assigns every real listing a non-empty unique external_id.
          (4) The system extracts 20 JSON-LD URLs matching the 20 DOM cards.
          (5) The system assigns every real listing a URL that starts with the ZipRecruiter base URL.
          (6) The system captures a salary_range for the Pearly listing that includes $185,000-$240,000.
    WHY: Synthetic fixtures can drift from production reality — these tests
         serve as a regression guard against ZipRecruiter structure changes

    MOCK BOUNDARY:
        Mock:  (none — pure functions operating on real fixture HTML)
        Real:  extract_job_cards, extract_json_ld_urls, card_to_listing
        Never: Patch extraction functions or modify fixture data
    """

    def test_real_serp_produces_20_listings(self) -> None:
        """
        GIVEN the real ZipRecruiter SERP fixture
        WHEN all cards are converted to listings
        THEN exactly 20 listings are produced.
        """
        # Given: real fixture
        html = _REAL_FIXTURE.read_text()
        cards = extract_job_cards(html)
        urls = extract_json_ld_urls(html)

        # When: convert all
        for i, c in enumerate(cards):
            c["url"] = urls[i] if i < len(urls) else ""
        listings = [card_to_listing(c) for c in cards]

        # Then: 20 listings
        assert len(listings) == 20, f"Expected 20 listings, got {len(listings)}"

    def test_real_first_listing_is_anchorage_digital(self) -> None:
        """
        GIVEN the real SERP fixture
        WHEN the first card is converted
        THEN the company is Anchorage Digital with a platform engineering title.
        """
        # Given: real fixture, first card
        html = _REAL_FIXTURE.read_text()
        cards = extract_job_cards(html)

        # When: convert first
        listing = card_to_listing(cards[0])

        # Then: Anchorage Digital
        assert listing.company == "Anchorage Digital", (
            f"Expected 'Anchorage Digital', got {listing.company!r}"
        )
        assert "Platform Engineering" in listing.title, (
            f"Expected title with 'Platform Engineering', got {listing.title!r}"
        )

    def test_real_listings_have_valid_external_ids(self) -> None:
        """
        GIVEN all 20 real listings
        WHEN external_ids are checked
        THEN every listing has a non-empty unique external_id.
        """
        # Given: all real listings
        html = _REAL_FIXTURE.read_text()
        cards = extract_job_cards(html)
        listings = [card_to_listing(c) for c in cards]

        # Then: all non-empty and unique
        assert all(item.external_id for item in listings), "All listings must have external_id"
        assert len(set(item.external_id for item in listings)) == 20, (
            "All 20 external_ids should be unique"
        )

    def test_real_json_ld_urls_match_card_count(self) -> None:
        """
        GIVEN the real SERP fixture
        WHEN JSON-LD URLs and DOM cards are extracted
        THEN both produce exactly 20 items.
        """
        # Given: real fixture
        html = _REAL_FIXTURE.read_text()
        cards = extract_job_cards(html)
        urls = extract_json_ld_urls(html)

        # Then: both 20
        assert len(cards) == 20, f"Expected 20 cards, got {len(cards)}"
        assert len(urls) == 20, f"Expected 20 URLs, got {len(urls)}"

    def test_real_listings_have_valid_urls(self) -> None:
        """
        GIVEN all 20 real listings with JSON-LD URLs matched
        WHEN URLs are checked
        THEN every URL starts with the ZipRecruiter base URL.
        """
        # Given: all real listings with URLs
        html = _REAL_FIXTURE.read_text()
        cards = extract_job_cards(html)
        urls = extract_json_ld_urls(html)
        for i, c in enumerate(cards):
            c["url"] = urls[i] if i < len(urls) else ""
        listings = [card_to_listing(c) for c in cards]

        # Then: all URLs valid
        assert all(item.url.startswith("https://www.ziprecruiter.com/c/") for item in listings), (
            "All URLs should start with ZipRecruiter base"
        )

    def test_real_pearly_salary_range(self) -> None:
        """
        GIVEN the Pearly listing (3rd card) from the real fixture
        WHEN salary metadata is checked
        THEN salary_range includes $185,000-$240,000.
        """
        # Given: Pearly listing (3rd card)
        html = _REAL_FIXTURE.read_text()
        cards = extract_job_cards(html)
        listing = card_to_listing(cards[2])

        # Then: salary range present
        assert listing.company == "Pearly", f"Expected 'Pearly', got {listing.company!r}"
        assert "salary_range" in listing.metadata, "Missing salary_range metadata"
        assert "$185,000" in listing.metadata["salary_range"], (
            f"Missing $185,000: {listing.metadata['salary_range']!r}"
        )
        assert "$240,000" in listing.metadata["salary_range"], (
            f"Missing $240,000: {listing.metadata['salary_range']!r}"
        )


# ---------------------------------------------------------------------------
# Test helpers
# ---------------------------------------------------------------------------


def _make_listing(
    external_id: str = "test-1",
    title: str = "Staff Architect",
    company: str = "Acme Corp",
    full_text: str = "",
    short_description: str = "",
) -> JobListing:
    """Create a test JobListing."""
    metadata: dict[str, str] = {}
    if short_description:
        metadata["short_description"] = short_description
    return JobListing(
        board="ziprecruiter",
        external_id=external_id,
        title=title,
        company=company,
        location="Remote",
        url=f"https://www.ziprecruiter.com/c/{external_id}",
        full_text=full_text,
        metadata=metadata,
    )


def _make_mock_page(
    *,
    content_html: str = "",
    title: str = "Jobs - ZipRecruiter",
    url: str = "https://www.ziprecruiter.com/jobs-search",
    panel_texts: list[str] | None = None,
    captcha: bool = False,
    card_ids: list[str] | None = None,
) -> MagicMock:
    """
    Create a mock Playwright page with configurable I/O responses.

    This is the I/O boundary for all ZipRecruiter adapter tests.
    ``page`` represents the browser automation interface.

    ``card_ids`` should match the external_ids from ``extract_job_cards``
    applied to ``content_html``.  Each card gets a mock locator that
    responds to ``article[id='job-card-{id}']``.
    """
    mock_page = MagicMock()
    mock_page.goto = AsyncMock()
    mock_page.title = AsyncMock(return_value=title)
    mock_page.content = AsyncMock(return_value=content_html)
    mock_page.url = url
    mock_page.query_selector = AsyncMock(return_value=MagicMock() if captcha else None)

    # Panel locator — keyed from the adapter's own selector dict
    _panel_sel = _SELECTORS["detail_panel"]
    mock_panel_locator = MagicMock()
    mock_panel_locator.wait_for = AsyncMock()
    if panel_texts:
        mock_panel_locator.inner_text = AsyncMock(side_effect=panel_texts)
    else:
        mock_panel_locator.inner_text = AsyncMock(return_value="")

    # Card locators by ID — derive the per-card selector prefix from the
    # adapter's ``_SELECTORS["card_articles"]`` so selector changes only
    # need to happen in one place.
    _card_prefix = _SELECTORS["card_articles"].replace("^=", "='")
    # e.g.  "article[id^='job-card-']" → "article[id='job-card-"
    _card_mocks: dict[str, MagicMock] = {}
    if card_ids:
        for cid in card_ids:
            card_mock = MagicMock()
            card_mock.click = AsyncMock()
            first_mock = MagicMock()
            first_mock.click = AsyncMock()
            card_mock.first = first_mock
            _card_mocks[cid] = card_mock

    def _locator_dispatch(sel: str) -> MagicMock:
        if sel == _panel_sel:
            return mock_panel_locator
        # Match per-card selectors derived from _SELECTORS["card_articles"]
        if sel.startswith(_card_prefix):
            cid = sel[len(_card_prefix) :].rstrip("']")
            if cid in _card_mocks:
                return _card_mocks[cid]
        # Fallback mock
        fallback = MagicMock()
        fallback.first = MagicMock()
        fallback.first.click = AsyncMock()
        return fallback

    mock_page.locator.side_effect = _locator_dispatch  # pyright: ignore[reportUnknownLambdaType]

    return mock_page


# ---------------------------------------------------------------------------
# TestAuthenticate
# ---------------------------------------------------------------------------


class TestAuthenticate:
    """
    REQUIREMENT: Session verification detects expired sessions and Cloudflare blocks.

    WHO: The pipeline runner during the authenticate step
    WHAT: (1) The system completes authentication without error when the session is valid.
          (2) The system raises an ActionableError that mentions CAPTCHA and suggests manual solve when it detects a CAPTCHA.
          (3) The system raises an ActionableError that states the session expired when authentication is redirected to `/login`.
          (4) The system raises an ActionableError that states the session expired when authentication is redirected to `/sign-in`.
          (5) The system raises an ActionableError that mentions Cloudflare and suggests headed mode when the Cloudflare challenge persists until timeout.
    WHY: Starting a search against an expired or blocked session wastes
         time and produces zero results — fail fast with actionable advice

    MOCK BOUNDARY:
        Mock:  Playwright page (browser I/O via _make_mock_page helper),
               asyncio.sleep + _CF_WAIT_TIMEOUT (timing control in
               cloudflare timeout test)
        Real:  ZipRecruiterAdapter.authenticate
        Never: Patch internal request/response parsing logic
    """

    @pytest.mark.asyncio
    async def test_authenticate_succeeds_with_valid_session(self) -> None:
        """
        GIVEN a mock page that loads without Cloudflare/CAPTCHA/redirect
        WHEN authenticate is called
        THEN it completes without error.
        """
        # Given: valid mock page
        adapter = ZipRecruiterAdapter()
        page = _make_mock_page()

        # When: authenticate
        await adapter.authenticate(page)

        # Then: page.goto was called
        page.goto.assert_awaited_once()

    @pytest.mark.asyncio
    async def test_authenticate_captcha_suggests_manual_solve(self) -> None:
        """
        GIVEN a mock page with a CAPTCHA element detected
        WHEN authenticate is called
        THEN an ActionableError suggesting manual-solve is raised.
        """
        # Given: page with CAPTCHA
        adapter = ZipRecruiterAdapter()
        page = _make_mock_page(captcha=True)

        # When/Then: raises with CAPTCHA guidance
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        # Then: actionable error
        err = exc_info.value
        assert "CAPTCHA" in err.error, f"Error should mention CAPTCHA: {err.error!r}"
        assert err.suggestion is not None, "Should include suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    @pytest.mark.asyncio
    async def test_authenticate_login_redirect_tells_operator_to_reauthenticate(self) -> None:
        """
        GIVEN a mock page redirected to /login
        WHEN authenticate is called
        THEN an ActionableError about session expiration is raised.
        """
        # Given: page redirected to login
        adapter = ZipRecruiterAdapter()
        page = _make_mock_page(url="https://www.ziprecruiter.com/login")

        # When/Then: raises session expired
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        # Then: actionable error
        err = exc_info.value
        assert "Session expired" in err.error, f"Error should mention expiration: {err.error!r}"
        assert err.suggestion is not None, "Should include suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    @pytest.mark.asyncio
    async def test_authenticate_sign_in_redirect_tells_operator_to_reauthenticate(self) -> None:
        """
        GIVEN a mock page redirected to /sign-in
        WHEN authenticate is called
        THEN an ActionableError about session expiration is raised.
        """
        # Given: page redirected to sign-in
        adapter = ZipRecruiterAdapter()
        page = _make_mock_page(url="https://www.ziprecruiter.com/sign-in")

        # When/Then: raises session expired
        with pytest.raises(ActionableError) as exc_info:
            await adapter.authenticate(page)

        # Then: actionable error
        err = exc_info.value
        assert "Session expired" in err.error, f"Error should mention expiration: {err.error!r}"
        assert err.suggestion is not None, "Should include suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    @pytest.mark.asyncio
    async def test_authenticate_cloudflare_timeout_suggests_headed_mode(self) -> None:
        """
        GIVEN a mock page showing a persistent Cloudflare challenge
        WHEN authenticate times out waiting for resolution
        THEN an ActionableError suggesting headed mode is raised.
        """
        # Given: Cloudflare challenge page
        adapter = ZipRecruiterAdapter()
        page = _make_mock_page(title="Just a moment...")

        # When/Then: raises after timeout (asyncio.sleep mocked so 15 iterations are instant)
        with (
            patch(
                "jobsearch_rag.adapters.ziprecruiter.asyncio.sleep",
                new_callable=AsyncMock,
            ),
            pytest.raises(ActionableError) as exc_info,
        ):
            await adapter.authenticate(page)

        # Then: actionable error about Cloudflare
        err = exc_info.value
        assert "Cloudflare" in err.error, f"Error should mention Cloudflare: {err.error!r}"
        assert err.suggestion is not None, "Should include suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"


# ---------------------------------------------------------------------------
# TestSearch
# ---------------------------------------------------------------------------


class TestSearch:
    """
    REQUIREMENT: search() navigates SERP pages, extracts cards, and enriches via click-through.

    WHO: The pipeline runner collecting listings from ZipRecruiter
    WHAT: (1) The system returns three listings from the fixture and includes 'Staff Platform Architect' as the first title.
          (2) The system enriches all cards with click-through panel text.
          (3) The system falls back to title/company context when panel text is too short.
          (4) The system falls back to title/company context when click-through fails.
          (5) The system stops pagination when a later page has no cards and returns listings only from earlier populated pages.
          (6) The system returns an empty listing list when it cannot find any article elements.
          (7) The system skips an unparseable card and returns the remaining valid listings.
          (8) The system appends `&page=2` when paginating a search URL that already contains a query string.
          (9) The system appends `?page=2` when paginating a search URL that has no query string.
          (10) The system respects the `max_pages` argument by navigating only the requested number of pages.
          (11) The system preserves existing full_text and skips click-through for already-populated listings.
    WHY: ZipRecruiter is a Next.js SERP — card metadata comes from
         server-rendered article elements and JSON-LD.  Click-through on
         the SERP avoids Cloudflare challenges that would block per-URL navigation.

    MOCK BOUNDARY:
        Mock:  Playwright page (browser I/O via _make_mock_page helper),
               random.uniform (delay control)
        Real:  ZipRecruiterAdapter.search, extract_job_cards,
               extract_json_ld_urls, card_to_listing
        Never: Patch internal extraction logic (except card_to_listing in
               one resilience test that verifies per-card error isolation)
    """

    @pytest.mark.asyncio
    async def test_search_returns_listings_from_fixture(self) -> None:
        """
        GIVEN a mock page with the synthetic SERP fixture and 3 card articles
        WHEN search is called with max_pages=1
        THEN 3 listings are returned with the first titled 'Staff Platform Architect'.
        """
        # Given: mock page with fixture HTML
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        panel_text = "A detailed job description for testing " * 10  # > 100 chars

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[panel_text, panel_text, panel_text],
        )

        # When: search
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Then: 3 listings with correct first title
        assert len(listings) == 3, f"Expected 3 listings, got {len(listings)}"
        assert listings[0].title == "Staff Platform Architect", (
            f"Expected 'Staff Platform Architect', got {listings[0].title!r}"
        )

    @pytest.mark.asyncio
    async def test_search_all_cards_enriched_by_click_through(self) -> None:
        """
        GIVEN a mock page with 3 cards and panel texts for all cards
        WHEN search is called
        THEN all cards are enriched via click-through panel text.
        """
        # Given: mock page with distinct panel text per card
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        panel_a_text = "Role A detailed job description for panel " * 10
        panel_b_text = "Role B detailed job description for panel " * 10
        panel_c_text = "Role C detailed job description for panel " * 10

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[panel_a_text, panel_b_text, panel_c_text],
        )

        # When: search
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Then: all panel texts enriched
        assert "Role A" in listings[0].full_text, "Card 0 should have panel A text"
        assert "Role B" in listings[1].full_text, "Card 1 should have panel B text"
        assert "Role C" in listings[2].full_text, "Card 2 should have panel C text"

    @pytest.mark.asyncio
    async def test_search_falls_back_when_panel_too_short(self) -> None:
        """
        GIVEN a mock page where panel text is under 100 chars
        WHEN search is called
        THEN title/company fallback is used for those cards.
        """
        # Given: mock page with short panel text
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=["Too short", "Also short", "Nope"],
        )

        # When: search
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Then: all cards have empty or fallback text (no short_description in new fixture)
        assert len(listings) == 3, f"Expected 3 listings, got {len(listings)}"

    @pytest.mark.asyncio
    async def test_search_falls_back_on_click_failure(self) -> None:
        """
        GIVEN a mock page where clicking the second card raises TimeoutError
        WHEN search is called
        THEN the failed card's full_text is empty or fallback, while others are enriched.
        """
        # Given: mock page with failing card click
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
        )

        # Make the second card's click raise TimeoutError
        fail_mock = MagicMock()
        fail_first = MagicMock()
        fail_first.click = AsyncMock(side_effect=TimeoutError("click timeout"))
        fail_mock.first = fail_first

        # Override locator for def456key
        original_dispatch = page.locator.side_effect
        _card_prefix = _SELECTORS["card_articles"].replace("^=", "='")
        _fail_sel = f"{_card_prefix}def456key']"

        def _patched_dispatch(sel: str) -> MagicMock:
            if sel == _fail_sel:
                return fail_mock
            return original_dispatch(sel)

        page.locator.side_effect = _patched_dispatch  # pyright: ignore[reportUnknownLambdaType]

        panel_text = "Panel text for other cards with enough detail " * 10
        panel_locator = page.locator(_SELECTORS["detail_panel"])
        panel_locator.inner_text = AsyncMock(return_value=panel_text)

        # When: search
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Then: 3 listings returned, card 1 has empty/fallback text
        assert len(listings) == 3, f"Expected 3 listings, got {len(listings)}"

    @pytest.mark.asyncio
    async def test_search_stops_when_no_cards_on_page(self) -> None:
        """
        GIVEN a multi-page SERP where page 2 has zero cards
        WHEN search is called with max_pages=3
        THEN pagination stops after 2 pages with only page 1 listings.
        """
        # Given: page 1 has cards, page 2 has zero cards
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        no_cards_html = "<html><head><title>Jobs</title></head><body></body></html>"

        call_count = {"goto": 0}

        async def _tracked_goto(url: str, **kwargs: object) -> None:
            call_count["goto"] += 1

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[
                "Long panel text " * 20,
                "Long panel text " * 20,
                "Long panel text " * 20,
            ],
        )
        page.goto = _tracked_goto
        # After first page, return no-cards HTML
        page.content = AsyncMock(side_effect=[fixture_html, no_cards_html])

        # When: search is called with max_pages=3
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=3)

        # Then: pagination stops (2 goto calls: page 1 + page 2 with no cards)
        assert call_count["goto"] == 2, f"Expected 2 page navigations, got {call_count['goto']}"
        assert len(listings) == 3, f"Expected 3 listings from page 1 only, got {len(listings)}"

    @pytest.mark.asyncio
    async def test_search_returns_empty_when_no_articles(self) -> None:
        """
        GIVEN a page with no article elements
        WHEN search is called
        THEN an empty listing list is returned.
        """
        # Given: HTML with no articles
        adapter = ZipRecruiterAdapter()
        no_articles_html = "<html><head><title>Jobs</title></head><body>No jobs here</body></html>"

        page = _make_mock_page(content_html=no_articles_html)

        # When: search
        listings = await adapter.search(page, "https://zr.com/search", max_pages=3)

        # Then: empty
        assert listings == [], f"Expected empty list, got {len(listings)} listings"

    @pytest.mark.asyncio
    async def test_search_skips_unparseable_card(self) -> None:
        """
        GIVEN a mock page where card_to_listing raises on the second card
        WHEN search is called
        THEN the unparseable card is skipped and 2 listings are returned.
        """
        # Given: mock page with 3 cards, one will fail
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        panel_text = "Long panel text for testing with enough chars " * 10

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[panel_text, panel_text, panel_text],
        )

        # Make card_to_listing raise on the second card
        original_card_to_listing = card_to_listing

        call_count = {"calls": 0}

        def _failing_card_to_listing(card: dict[str, Any]) -> JobListing:
            call_count["calls"] += 1
            if call_count["calls"] == 2:
                raise ValueError("Unparseable card")
            return original_card_to_listing(card)

        # When: search with failing card_to_listing
        with (
            patch(
                "jobsearch_rag.adapters.ziprecruiter.card_to_listing",
                side_effect=_failing_card_to_listing,
            ),
            patch(
                "jobsearch_rag.adapters.ziprecruiter.random.uniform",
                return_value=0.0,
            ),
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Then: 3 cards, 1 failed → 2 listings
        assert len(listings) == 2, f"Expected 2 listings (1 skipped), got {len(listings)}"

    @pytest.mark.asyncio
    async def test_search_paginates_url_with_ampersand(self) -> None:
        """
        GIVEN a query URL containing '?'
        WHEN search navigates to page 2
        THEN page 2 URL appends '&page=2'.
        """
        # Given: a query URL with existing query string
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        empty_html = "<html><head><title>Jobs</title></head><body></body></html>"

        goto_urls: list[str] = []

        async def _track_goto(url: str, **kwargs: object) -> None:
            goto_urls.append(url)

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[
                "Long panel text " * 20,
                "Long panel text " * 20,
                "Long panel text " * 20,
            ],
        )
        page.goto = _track_goto
        page.content = AsyncMock(side_effect=[fixture_html, empty_html])

        # When: search is called with max_pages=2
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            await adapter.search(page, "https://zr.com/search?q=architect", max_pages=2)

        # Then: page 2 URL appends '&page=2' since URL already has '?'
        assert goto_urls[0] == "https://zr.com/search?q=architect", (
            f"Expected page 1 URL unchanged, got {goto_urls[0]}"
        )
        assert goto_urls[1] == "https://zr.com/search?q=architect&page=2", (
            f"Expected '&page=2' appended, got {goto_urls[1]}"
        )

    @pytest.mark.asyncio
    async def test_search_paginates_url_with_question_mark(self) -> None:
        """
        GIVEN a query URL without '?' (no query string)
        WHEN search paginates to page 2
        THEN '?page=2' is used as the separator.
        """
        # Given: a query URL without '?'
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        empty_html = "<html><head><title>Jobs</title></head><body></body></html>"

        goto_urls: list[str] = []

        async def _track_goto(url: str, **kwargs: object) -> None:
            goto_urls.append(url)

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[
                "Long panel text " * 20,
                "Long panel text " * 20,
                "Long panel text " * 20,
            ],
        )
        page.goto = _track_goto
        page.content = AsyncMock(side_effect=[fixture_html, empty_html])

        # When: search paginates to page 2
        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            await adapter.search(page, "https://zr.com/search", max_pages=2)

        # Then: '?page=2' is used since URL has no existing query string
        assert goto_urls[1] == "https://zr.com/search?page=2", (
            f"Expected '?page=2' separator, got {goto_urls[1]}"
        )

    @pytest.mark.asyncio
    async def test_search_respects_max_pages_argument(self) -> None:
        """
        GIVEN a fixture with 3 cards
        WHEN search is called with max_pages=1
        THEN only 1 page is navigated.
        """
        # Given: fixture with cards, but max_pages=1
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        panel_text = "Full detail text with enough content for tests " * 5

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[panel_text, panel_text, panel_text],
        )

        with patch(
            "jobsearch_rag.adapters.ziprecruiter.random.uniform",
            return_value=0.0,
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Only 1 page navigated
        page.goto.assert_awaited_once()
        assert len(listings) == 3, f"Expected 3 listings, got {len(listings)}"

    @pytest.mark.asyncio
    async def test_search_preserves_prepopulated_full_text(self) -> None:
        """
        GIVEN a listing whose full_text is already populated before click-through
        WHEN search enriches cards via click-through
        THEN the pre-populated listing keeps its original full_text.
        """
        # Given: fixture with 3 cards; we pre-populate one listing's full_text
        adapter = ZipRecruiterAdapter()
        fixture_html = _SERP_FIXTURE.read_text()
        panel_text = "Panel text from click-through enrichment " * 10
        prepopulated_text = "Original text that should be preserved"

        page = _make_mock_page(
            content_html=fixture_html,
            card_ids=["abc123key", "def456key", "ghi789key"],
            panel_texts=[panel_text, panel_text, panel_text],
        )

        # Patch card_to_listing so the first card arrives with full_text set
        original_ctl = card_to_listing
        call_count = {"n": 0}

        def _ctl_with_prepopulated(card: dict[str, Any]) -> JobListing:
            listing = original_ctl(card)
            call_count["n"] += 1
            if call_count["n"] == 1:
                listing.full_text = prepopulated_text
            return listing

        # When: search
        with (
            patch(
                "jobsearch_rag.adapters.ziprecruiter.card_to_listing",
                side_effect=_ctl_with_prepopulated,
            ),
            patch(
                "jobsearch_rag.adapters.ziprecruiter.random.uniform",
                return_value=0.0,
            ),
        ):
            listings = await adapter.search(page, "https://zr.com/search", max_pages=1)

        # Then: first listing preserves its original text, others got panel text
        assert listings[0].full_text == prepopulated_text, (
            f"Pre-populated text should be preserved, got {listings[0].full_text[:80]!r}"
        )
        assert "Panel text" in listings[1].full_text, "Card 1 should have panel text"
        assert "Panel text" in listings[2].full_text, "Card 2 should have panel text"


# ---------------------------------------------------------------------------
# TestExtractDetailPassthrough
# ---------------------------------------------------------------------------


class TestExtractDetailPassthrough:
    """
    REQUIREMENT: extract_detail is a passthrough when full_text is populated.

    WHO: The pipeline runner calling extract_detail after search
    WHAT: (1) The system returns the listing unchanged when full_text is already populated.
          (2) The system populates full_text from the short_description fallback when full_text is empty.
          (3) The system leaves full_text empty when no short_description fallback is available.
    WHY: SERP click-through makes per-URL extraction unnecessary — the
         runner calls extract_detail out of protocol compliance, but
         ZipRecruiter does all extraction during search()

    MOCK BOUNDARY:
        Mock:  Playwright page (unused — passthrough behaviour)
        Real:  ZipRecruiterAdapter.extract_detail
        Never: Patch extract_detail internals or full_text assignment
    """

    @pytest.mark.asyncio
    async def test_passthrough_when_full_text_present(self) -> None:
        """
        GIVEN a listing with full_text already populated
        WHEN extract_detail is called
        THEN the listing is returned unchanged.
        """
        # Given: listing with full_text
        adapter = ZipRecruiterAdapter()
        listing = _make_listing(full_text="Full JD already populated")
        mock_page = MagicMock()

        # When: extract_detail
        result = await adapter.extract_detail(mock_page, listing)

        # Then: unchanged
        assert result is listing, "Should return same object"
        assert result.full_text == "Full JD already populated", (
            f"full_text should be unchanged: {result.full_text!r}"
        )

    @pytest.mark.asyncio
    async def test_fallback_when_full_text_empty(self) -> None:
        """
        GIVEN a listing with empty full_text and a short_description in metadata
        WHEN extract_detail is called
        THEN the shortDescription fallback populates full_text.
        """
        # Given: listing with empty full_text, short_description in metadata
        adapter = ZipRecruiterAdapter()
        listing = _make_listing(full_text="", short_description="Python role at company")
        mock_page = MagicMock()

        # When: extract_detail
        result = await adapter.extract_detail(mock_page, listing)

        # Then: fallback applied
        assert "Python role at company" in result.full_text, (
            f"short_description fallback missing: {result.full_text!r}"
        )
        assert "Staff Architect at Acme Corp" in result.full_text, (
            f"Title/company context missing: {result.full_text!r}"
        )

    @pytest.mark.asyncio
    async def test_empty_when_no_fallback_available(self) -> None:
        """
        GIVEN a listing with empty full_text and no short_description
        WHEN extract_detail is called
        THEN full_text remains empty.
        """
        # Given: listing with empty full_text, no fallback
        adapter = ZipRecruiterAdapter()
        listing = _make_listing(full_text="")
        mock_page = MagicMock()

        # When: extract_detail
        result = await adapter.extract_detail(mock_page, listing)

        # Then: still empty
        assert result.full_text == "", f"Expected empty full_text, got {result.full_text!r}"
