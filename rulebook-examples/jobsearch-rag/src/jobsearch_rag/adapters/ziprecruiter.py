"""
ZipRecruiter adapter — auth, search pagination, JD extraction.

ZipRecruiter renders its SERP as a Next.js application with server-side
rendered article elements and JSON-LD structured data.

Extraction strategy:
  1. Navigate to the search URL and wait for DOM content.
  2. Wait for any Cloudflare challenge to resolve (important: Cloudflare
     presents a "Just a moment..." interstitial on first visit).
  3. Parse job card metadata from ``<article id="job-card-{id}">``
     elements server-rendered in the HTML.
  4. Extract job URLs from ``<script type="application/ld+json">``
     structured data (schema.org ItemList).
  5. **Click each card article** on the SERP to update the detail panel
     (``[data-testid='job-details-scroll-container']``) and read the
     full JD text via ``inner_text()``.

The adapter stays on the SERP page and uses click-through extraction
rather than navigating to individual detail URLs, which avoids triggering
fresh Cloudflare challenges per listing.

.. important::

   ZipRecruiter uses Cloudflare bot protection.  Headless Chromium is
   typically blocked.  Set ``headless = false`` in ``settings.toml``
   for reliable operation.  Use ``jobsearch-rag login --board ziprecruiter``
   to establish a session interactively before running headless searches.
"""

from __future__ import annotations

import asyncio
import json
import random
import re
from html.parser import HTMLParser
from typing import TYPE_CHECKING, Any

from jobsearch_rag.adapters.base import JobBoardAdapter, JobListing
from jobsearch_rag.adapters.registry import AdapterRegistry
from jobsearch_rag.errors import ActionableError
from jobsearch_rag.logging import logger

if TYPE_CHECKING:
    from playwright.async_api import Page

# ---------------------------------------------------------------------------
# Selectors — only the handful we still need against the actual DOM
# ---------------------------------------------------------------------------

_SELECTORS = {
    "captcha_indicator": "iframe[src*='captcha'], div.g-recaptcha",
    "card_articles": "article[id^='job-card-']",
    "detail_panel": "[data-testid='job-details-scroll-container']",
}

# Base URL for relative canonical paths
_BASE_URL = "https://www.ziprecruiter.com"

# Maximum seconds to wait for Cloudflare challenge to resolve
_CF_WAIT_TIMEOUT = 15

# Click-through timing (seconds) — human-like pause between card clicks
_CLICK_DELAY = (0.5, 1.5)

# Throttle detection — ZR returns this error message when rate-limited
_THROTTLE_PHRASES = [
    "we encountered an error while loading this job",
]

# Throttle backoff parameters
_THROTTLE_MAX_RETRIES = 3
_THROTTLE_BASE_DELAY = 2.0  # seconds; doubles each retry


def is_throttle_response(text: str) -> bool:
    """
    Return True if *text* matches a known ZipRecruiter throttle response.

    Checks for known error phrases that ZR displays in the detail panel
    when rate-limiting a session.  The check is case-insensitive and
    only triggers on short text (< 200 chars) to avoid false positives
    when a legitimate JD mentions 'error'.
    """
    if not text or len(text.strip()) > 200:
        return False
    lower = text.strip().lower()
    return any(phrase in lower for phrase in _THROTTLE_PHRASES)


# ---------------------------------------------------------------------------
# HTML → plain-text helper
# ---------------------------------------------------------------------------


class _HTMLTextExtractor(HTMLParser):
    """Lightweight HTML-to-text converter for JD descriptions."""

    def __init__(self) -> None:
        super().__init__()
        self._parts: list[str] = []

    def handle_data(self, data: str) -> None:
        self._parts.append(data)

    def get_text(self) -> str:
        return " ".join(self._parts)


def html_to_text(html: str) -> str:
    """Strip HTML tags and return plain text."""
    extractor = _HTMLTextExtractor()
    extractor.feed(html)
    return re.sub(r"\s+", " ", extractor.get_text()).strip()


# ---------------------------------------------------------------------------
# DOM extraction helpers
# ---------------------------------------------------------------------------


def extract_json_ld_urls(html: str) -> list[str]:
    """
    Extract job URLs from JSON-LD ItemList structured data.

    ZipRecruiter embeds a ``<script type="application/ld+json">`` block
    with a schema.org ``ItemList`` containing job URLs, titles, and
    positions.  This function parses that block and returns the URLs
    in position order.

    Args:
        html: Full page HTML source.

    Returns:
        List of absolute job URLs in position order, or empty list if
        JSON-LD is missing or malformed.

    """
    pattern = r'<script[^>]*type="application/ld\+json"[^>]*>(.*?)</script>'
    match = re.search(pattern, html, re.DOTALL)
    if not match:
        return []
    try:
        data = json.loads(match.group(1))
    except json.JSONDecodeError:
        return []
    items: list[dict[str, Any]] = data.get("itemListElement", [])
    return [item["url"] for item in items if "url" in item]


def parse_salary_text(text: str) -> tuple[float | None, float | None]:
    """
    Parse salary text like ``$185K - $240K/yr`` into annual (min, max).

    Handles K (thousands) and M (millions) suffixes.  Returns
    ``(None, None)`` when no salary pattern is found.
    """
    pattern = r"\$([\d,.]+)([KkMm]?)\s*[-\u2013]\s*\$([\d,.]+)([KkMm]?)(?:/yr)?"
    match = re.search(pattern, text)
    if not match:
        return None, None

    def _to_annual(val_str: str, suffix: str) -> float:
        val = float(val_str.replace(",", ""))
        if suffix.upper() == "K":
            val *= 1_000
        elif suffix.upper() == "M":
            val *= 1_000_000
        return val

    min_val = _to_annual(match.group(1), match.group(2))
    max_val = _to_annual(match.group(3), match.group(4))
    return min_val, max_val


def extract_job_cards(html: str) -> list[dict[str, Any]]:
    """
    Extract job card data from Next.js server-rendered article elements.

    ZipRecruiter's Next.js SERP renders each job as an
    ``<article id="job-card-{external_id}">`` element containing the
    title (``<h2>``), company (``data-testid="job-card-company"``),
    location (``data-testid="job-card-location"``), and optional salary
    text.

    Articles appear twice in the HTML (desktop and mobile responsive
    variants).  This function deduplicates by ``external_id``.

    Returns:
        List of dicts with keys: ``external_id``, ``title``, ``company``,
        ``location``, ``salary_text``.

    """
    cards: list[dict[str, Any]] = []
    seen_ids: set[str] = set()

    for art_match in re.finditer(
        r'<article[^>]*\bid="job-card-([^"]+)"[^>]*>(.*?)</article>',
        html,
        re.DOTALL,
    ):
        ext_id = art_match.group(1)
        if ext_id in seen_ids:
            continue
        seen_ids.add(ext_id)

        content = art_match.group(2)

        # Title from <h2>
        h2_match = re.search(r"<h2[^>]*>(.*?)</h2>", content, re.DOTALL)
        title = re.sub(r"<[^>]+>", "", h2_match.group(1)).strip() if h2_match else ""

        # Company from data-testid="job-card-company"
        company_match = re.search(r'data-testid="job-card-company"[^>]*>([^<]+)', content)
        company = company_match.group(1).strip() if company_match else "Unknown"

        # Location from data-testid="job-card-location"
        loc_match = re.search(r'data-testid="job-card-location"[^>]*>([^<]+)', content)
        location = loc_match.group(1).strip() if loc_match else "Unknown"

        # Salary text (if present)
        sal_match = re.search(
            r"\$([\d,.]+[KkMm]?)\s*[-\u2013]\s*\$([\d,.]+[KkMm]?)(?:/yr)?",
            content,
        )
        salary_text = sal_match.group(0) if sal_match else ""

        cards.append(
            {
                "external_id": ext_id,
                "title": title,
                "company": company,
                "location": location,
                "salary_text": salary_text,
            }
        )

    return cards


def card_to_listing(card: dict[str, Any]) -> JobListing:
    """
    Convert an extracted card dict to a ``JobListing``.

    Expected card keys:
        ``external_id``, ``title``, ``company``, ``location``,
        ``url`` (from JSON-LD matching), ``salary_text`` (optional).
    """
    ext_id: str = card.get("external_id", "")
    title: str = card.get("title", "")
    company: str = card.get("company", "Unknown")
    location: str = card.get("location", "Unknown")
    url: str = card.get("url", "")
    salary_text: str = card.get("salary_text", "")

    metadata: dict[str, str] = {}
    comp_min: float | None = None
    comp_max: float | None = None

    if salary_text:
        comp_min, comp_max = parse_salary_text(salary_text)
        if comp_min is not None and comp_max is not None:
            metadata["salary_range"] = f"${comp_min:,.0f} - ${comp_max:,.0f}"

    return JobListing(
        board="ziprecruiter",
        external_id=ext_id,
        title=title,
        company=company,
        location=location,
        url=url,
        full_text="",  # Populated by click-through
        comp_min=comp_min,
        comp_max=comp_max,
        comp_source="serp" if comp_min is not None else None,
        comp_text=salary_text or None,
        metadata=metadata,
    )


# ---------------------------------------------------------------------------
# Cloudflare challenge helper
# ---------------------------------------------------------------------------


async def _wait_for_cloudflare(page: Page, *, timeout: int = _CF_WAIT_TIMEOUT) -> None:
    """
    Wait for a Cloudflare "Just a moment..." challenge to resolve.

    Cloudflare inserts an interstitial page that runs a JS challenge.
    In headed mode it resolves in < 1 s.  In headless mode it usually
    blocks indefinitely (headless = false is recommended).

    This function polls the page title and returns as soon as the
    challenge clears, or raises :class:`ActionableError` on timeout.
    """
    for _ in range(timeout):
        title = await page.title()
        if "just a moment" not in title.lower():
            return
        await asyncio.sleep(1)

    raise ActionableError.authentication(
        "ziprecruiter",
        f"Cloudflare challenge did not resolve within {timeout}s",
        suggestion=(
            "Set headless = false in settings.toml for ziprecruiter. "
            "Cloudflare blocks headless browsers."
        ),
    )


# ---------------------------------------------------------------------------
# Adapter
# ---------------------------------------------------------------------------


@AdapterRegistry.register
class ZipRecruiterAdapter(JobBoardAdapter):
    """
    Browser automation adapter for ZipRecruiter.

    Extracts job card metadata from server-rendered Next.js article
    elements and JSON-LD structured data.  Full JD text is obtained via
    SERP click-through: clicking each card article updates the detail
    panel, avoiding per-URL navigation and Cloudflare challenges.
    """

    @property
    def board_name(self) -> str:
        """Return the board identifier."""
        return "ziprecruiter"

    async def authenticate(self, page: Page) -> None:
        """
        Verify an active session by checking for login/CAPTCHA indicators.

        ZipRecruiter auth is cookie-based via ``storage_state``.  This method
        validates the session is still alive rather than performing login.

        Raises:
            ActionableError: If the session is expired or a CAPTCHA is present.

        """
        await page.goto(
            "https://www.ziprecruiter.com/jobs-search",
            wait_until="domcontentloaded",
        )

        # Wait for Cloudflare challenge if present
        await _wait_for_cloudflare(page)

        # Check for CAPTCHA first — it takes priority
        captcha = await page.query_selector(_SELECTORS["captcha_indicator"])
        if captcha:
            raise ActionableError.authentication(
                self.board_name,
                "CAPTCHA encountered — cannot proceed automatically",
                suggestion="Solve CAPTCHA manually in headed mode, then re-run",
            )

        # Check if we landed on a login page instead of search
        url = page.url
        if "/login" in url or "/sign-in" in url:
            raise ActionableError.authentication(
                self.board_name,
                "Session expired — redirected to login page",
                suggestion=(
                    "Run in headed mode (headless=false) to authenticate manually. "
                    "Session cookies will be saved automatically."
                ),
            )

        logger.info("ZipRecruiter session verified")

    async def search(
        self,
        page: Page,
        query: str,
        max_pages: int = 3,
    ) -> list[JobListing]:
        """
        Navigate search results, click through cards, and return enriched listings.

        Extracts job card metadata from server-rendered article elements
        and JSON-LD structured data, then clicks through each card on the
        SERP to read full JD text from the detail panel.

        Args:
            page: Playwright page with an active session.
            query: Full search URL (from settings.toml).
            max_pages: Maximum number of result pages to paginate through.

        Returns:
            List of ``JobListing`` with ``full_text`` already populated
            via click-through.  ``extract_detail`` will be a no-op.

        """
        listings: list[JobListing] = []
        pages_processed = 0

        logger.info("Searching ZipRecruiter: %s (max %d pages)", query, max_pages)

        for page_num in range(1, max_pages + 1):
            # Build paginated URL
            sep = "&" if "?" in query else "?"
            page_url = f"{query}{sep}page={page_num}" if page_num > 1 else query
            await page.goto(page_url, wait_until="domcontentloaded")
            await _wait_for_cloudflare(page)

            logger.info("Processing search results page %d", page_num)

            html = await page.content()
            cards = extract_job_cards(html)
            if not cards:
                logger.info("No job cards on page %d — stopping", page_num)
                break

            # Match URLs from JSON-LD structured data
            urls = extract_json_ld_urls(html)

            # Parse card metadata into listings
            page_listings: list[JobListing] = []
            for i, card in enumerate(cards):
                try:
                    card["url"] = urls[i] if i < len(urls) else ""
                    listing = card_to_listing(card)
                    page_listings.append(listing)
                except Exception as exc:
                    logger.warning("Failed to parse a job card: %s", exc)

            # Click through all cards to extract full JD from detail panel
            await self._click_through_cards(page, page_listings)

            listings.extend(page_listings)
            pages_processed = page_num

        enriched = sum(1 for ls in listings if ls.full_text.strip())
        logger.info(
            "Found %d listings across %d page(s) (%d with full JD)",
            len(listings),
            pages_processed,
            enriched,
        )
        return listings

    async def _click_through_cards(
        self,
        page: Page,
        listings: list[JobListing],
    ) -> None:
        """
        Click each card article on the SERP and read JD from the detail panel.

        Skips listings that already have ``full_text`` populated.  Falls
        back to title/company context when the panel text is too short.

        Detects ZipRecruiter throttle responses (error messages instead of
        real JD text) and retries with exponential backoff before skipping.
        """
        panel_locator = page.locator(_SELECTORS["detail_panel"])

        consecutive_throttles = 0

        for i, listing in enumerate(listings):
            # Skip if already populated
            if listing.full_text.strip():
                continue

            # Locate this card by its external_id
            card_locator = page.locator(f"article[id='job-card-{listing.external_id}']").first

            retry = 0
            while retry <= _THROTTLE_MAX_RETRIES:
                try:
                    await card_locator.click()

                    # Human-like delay
                    delay = random.uniform(*_CLICK_DELAY)
                    await asyncio.sleep(delay)

                    # Wait for panel to be visible and read text
                    await panel_locator.wait_for(state="visible", timeout=5000)
                    panel_text = await panel_locator.inner_text()

                    # Check for throttle response
                    if is_throttle_response(panel_text):
                        consecutive_throttles += 1
                        backoff = _THROTTLE_BASE_DELAY * (2 ** (consecutive_throttles - 1))
                        logger.warning(
                            "Throttle detected for %s (retry %d/%d, backoff %.1fs): %s",
                            listing.external_id,
                            retry + 1,
                            _THROTTLE_MAX_RETRIES,
                            backoff,
                            listing.url,
                        )
                        retry += 1
                        if retry <= _THROTTLE_MAX_RETRIES:
                            await asyncio.sleep(backoff)
                        continue

                    # Real content — accept it
                    if panel_text and len(panel_text.strip()) > 100:
                        listing.full_text = panel_text.strip()
                        logger.debug(
                            "Card %d/%d: %d chars from detail panel",
                            i + 1,
                            len(listings),
                            len(listing.full_text),
                        )
                    else:
                        # Too short — not throttle, just sparse content
                        self._apply_short_description_fallback(listing)
                        logger.debug(
                            "Card %d/%d: panel text too short, used fallback",
                            i + 1,
                            len(listings),
                        )
                    break

                except Exception as exc:
                    try:
                        late_text = await panel_locator.inner_text()
                    except Exception:
                        late_text = ""

                    if is_throttle_response(late_text):
                        consecutive_throttles += 1
                        backoff = _THROTTLE_BASE_DELAY * (2 ** (consecutive_throttles - 1))
                        logger.warning(
                            "Throttle detected (late) for %s (retry %d/%d, backoff %.1fs): %s",
                            listing.external_id,
                            retry + 1,
                            _THROTTLE_MAX_RETRIES,
                            backoff,
                            listing.url,
                        )
                        retry += 1
                        if retry <= _THROTTLE_MAX_RETRIES:
                            await asyncio.sleep(backoff)
                        continue

                    logger.warning(
                        "Click-through failed for card %d (%s): %s",
                        i + 1,
                        listing.external_id,
                        exc,
                    )
                    self._apply_short_description_fallback(listing)
                    break
            else:
                # Retry budget exhausted — fall back to short description
                logger.warning(
                    "Max retries exhausted for %s — skipping",
                    listing.external_id,
                )
                self._apply_short_description_fallback(listing)

    @staticmethod
    def _apply_short_description_fallback(listing: JobListing) -> None:
        """Populate full_text from the SERP card's shortDescription snippet."""
        short_desc = listing.metadata.get("short_description", "")
        if short_desc:
            listing.full_text = f"{listing.title} at {listing.company}. {short_desc}"
        else:
            listing.full_text = ""

    async def extract_detail(
        self,
        page: Page,
        listing: JobListing,
    ) -> JobListing:
        """
        Return the listing — full JD is already populated by click-through.

        The ``search()`` method populates ``full_text`` during SERP
        click-through, so this method is a passthrough.  If ``full_text``
        is somehow still empty (e.g. click-through was skipped), falls
        back to the ``shortDescription`` captured from the SERP card.

        Returns the same listing object, unchanged or with fallback text.
        """
        if listing.full_text.strip():
            return listing

        # Fallback: use the short description from the SERP card
        logger.debug(
            "extract_detail called with empty full_text for %s — applying fallback",
            listing.external_id,
        )
        self._apply_short_description_fallback(listing)
        return listing
