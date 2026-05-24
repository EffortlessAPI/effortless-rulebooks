"""
Playwright session manager with storage_state persistence and throttling.

Owns browser lifecycle, cookie persistence, and human-like rate limiting.
Adapters receive a ``Page`` — they never launch browsers themselves.

When ``browser_channel`` is set (e.g. ``msedge``), the session manager
launches the real system browser as a subprocess and connects to it via
the Chrome DevTools Protocol (CDP).  This avoids Playwright's automation
flags (``--enable-automation``, ``navigator.webdriver``) that Cloudflare
bot detection uses to block automated browsers.
"""

from __future__ import annotations

import asyncio
import json
import random
import shutil
import signal
import socket
import subprocess
import tempfile
import urllib.request
from dataclasses import dataclass
from pathlib import Path
from typing import TYPE_CHECKING

from playwright.async_api import async_playwright

from jobsearch_rag.errors import ActionableError
from jobsearch_rag.logging import logger

if TYPE_CHECKING:
    from playwright.async_api import Browser, BrowserContext, Page, Playwright

    from jobsearch_rag.adapters.base import JobBoardAdapter


# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

_STORAGE_DIR = Path("data")

# Known browser binary paths on macOS
_BROWSER_PATHS: dict[str, list[str]] = {
    "msedge": [
        "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge",
    ],
    "chrome": [
        "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
    ],
    "chromium": [
        "/Applications/Chromium.app/Contents/MacOS/Chromium",
    ],
}


@dataclass
class SessionConfig:
    """Per-board browser session configuration."""

    board_name: str
    headless: bool = True
    user_agent: str | None = None
    viewport_width: int = 1440
    viewport_height: int = 900
    stealth: bool = False
    overnight: bool = False
    browser_channel: str | None = None

    @property
    def storage_state_path(self) -> Path:
        """Path to the cookie/session JSON for this board."""
        return _STORAGE_DIR / f"{self.board_name}_session.json"


# ---------------------------------------------------------------------------
# CDP helpers
# ---------------------------------------------------------------------------


def _find_browser_binary(channel: str) -> str | None:
    """
    Resolve a browser channel name to an executable path.

    First checks the known paths in ``_BROWSER_PATHS``, then falls back
    to ``shutil.which()`` for PATH-based lookup.
    """
    for path in _BROWSER_PATHS.get(channel, []):
        if Path(path).exists():
            return path

    # Fallback: try finding the binary on $PATH
    which = shutil.which(channel)
    if which:
        return which

    return None


def _find_free_port() -> int:
    """Ask the OS for a free TCP port."""
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind(("127.0.0.1", 0))
        port: int = s.getsockname()[1]
        return port


async def _wait_for_cdp(
    cdp_url: str,
    *,
    timeout: float = 15.0,
    poll_interval: float = 0.3,
) -> None:
    """Poll until the CDP ``/json/version`` endpoint responds."""
    deadline = asyncio.get_event_loop().time() + timeout
    while True:
        try:
            urllib.request.urlopen(f"{cdp_url}/json/version", timeout=2)
            logger.debug("CDP endpoint ready at %s", cdp_url)
            return
        except OSError:
            if asyncio.get_event_loop().time() > deadline:
                raise TimeoutError(
                    f"CDP endpoint at {cdp_url} did not start within {timeout}s"
                ) from None
            await asyncio.sleep(poll_interval)


def _terminate_process(proc: subprocess.Popen[bytes]) -> None:
    """Gracefully stop the CDP browser subprocess."""
    if proc.poll() is not None:
        return  # already exited
    try:
        proc.send_signal(signal.SIGTERM)
        proc.wait(timeout=5)
    except (OSError, subprocess.TimeoutExpired):
        logger.warning("Browser subprocess did not exit cleanly — killing")
        proc.kill()
        proc.wait(timeout=3)


# ---------------------------------------------------------------------------
# Throttle
# ---------------------------------------------------------------------------


async def throttle(adapter: JobBoardAdapter) -> float:
    """
    Sleep for a random duration within the adapter's rate limit range.

    Returns the actual duration slept (useful for assertions).
    """
    lo, hi = adapter.rate_limit_seconds
    duration = random.uniform(lo, hi)
    logger.debug("Throttle: sleeping %.2fs for %s", duration, adapter.board_name)
    await asyncio.sleep(duration)
    return duration


# ---------------------------------------------------------------------------
# Session manager
# ---------------------------------------------------------------------------


class SessionManager:
    """
    Manages Playwright browser sessions with storage_state persistence.

    Two launch modes:

    **Playwright-managed** (default):
      ``playwright.chromium.launch()`` — Playwright starts its bundled
      Chromium.  Simple and fast, but Cloudflare detects the injected
      automation flags.

    **CDP mode** (when ``browser_channel`` is set):
      The real system browser (e.g. Edge) is launched as a subprocess
      with ``--remote-debugging-port``.  Playwright then connects via
      ``connect_over_cdp()``.  Because the browser was *not* launched
      by Playwright, it has no automation flags—Cloudflare sees it as
      a normal browser.

    Usage::

        async with SessionManager(config) as session:
            page = await session.new_page()
            await adapter.authenticate(page)
            await session.save_storage_state()
            results = await adapter.search(page, query)
    """

    def __init__(self, config: SessionConfig) -> None:
        """Initialize with browser session configuration."""
        self.config = config
        self._playwright: Playwright | None = None
        self._browser: Browser | None = None
        self._context: BrowserContext | None = None
        self._cdp_process: subprocess.Popen[bytes] | None = None
        self._cdp_tmpdir: str | None = None

    async def __aenter__(self) -> SessionManager:
        """Start Playwright, launch the browser, and apply stealth patches."""
        self._playwright = await async_playwright().start()

        if self.config.browser_channel:
            await self._launch_cdp()
        else:
            await self._launch_playwright()

        # Load existing session cookies if available
        storage_path = self.config.storage_state_path
        storage_state: str | None = None
        if storage_path.exists():
            logger.info("Loading session state from %s", storage_path)
            storage_state = str(storage_path)
        else:
            logger.info(
                "No existing session state for %s — fresh session",
                self.config.board_name,
            )

        self._context = await self._browser.new_context(  # type: ignore[union-attr]
            viewport={"width": self.config.viewport_width, "height": self.config.viewport_height},
            user_agent=self.config.user_agent if self.config.user_agent else None,
            storage_state=storage_state,
        )

        # Apply stealth patches if requested (LinkedIn)
        if self.config.stealth:
            try:
                from playwright_stealth import Stealth  # pyright: ignore[reportMissingTypeStubs] # optional dependency, no stubs available  # noqa: I001, PLC0415

                await Stealth().apply_stealth_async(self._context)
                logger.info("Stealth patches applied for %s", self.config.board_name)
            except ImportError:
                logger.warning("playwright-stealth not installed — stealth mode unavailable")

        return self

    async def _launch_playwright(self) -> None:
        """Launch browser via Playwright (standard mode)."""
        assert self._playwright is not None
        self._browser = await self._playwright.chromium.launch(
            headless=self.config.headless,
        )

    async def _launch_cdp(self) -> None:
        """
        Launch system browser as subprocess and connect via CDP.

        This avoids Playwright's automation detection flags, allowing
        the browser to pass Cloudflare bot protection.
        """
        assert self._playwright is not None
        channel = self.config.browser_channel or "msedge"

        # Find the browser binary
        binary = _find_browser_binary(channel)
        if not binary:
            raise ActionableError.config(
                field_name="browser_channel",
                reason=f"Could not find '{channel}' browser binary",
                suggestion=(
                    f"Install {channel} or set browser_channel to an installed browser "
                    "(msedge, chrome, chromium)"
                ),
            )

        # Find a free port for CDP
        port = _find_free_port()

        # Use a temp dir for the browser profile to avoid conflicts
        # with the user's running browser
        self._cdp_tmpdir = tempfile.mkdtemp(prefix=f"jobsearch-{channel}-")

        cmd = [
            binary,
            f"--remote-debugging-port={port}",
            f"--user-data-dir={self._cdp_tmpdir}",
            "--no-first-run",
            "--no-default-browser-check",
        ]

        if self.config.headless:
            cmd.append("--headless=new")

        # Open to about:blank so the initial window is invisible noise,
        # not a distracting new-tab page.
        cmd.append("about:blank")

        logger.info(
            "Launching %s via CDP on port %d for %s",
            channel,
            port,
            self.config.board_name,
        )
        self._cdp_process = subprocess.Popen(
            cmd,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
        )

        # Wait for the CDP endpoint to become available
        cdp_url = f"http://localhost:{port}"
        await _wait_for_cdp(cdp_url)

        self._browser = await self._playwright.chromium.connect_over_cdp(cdp_url)

        # Close the initial blank page that Edge opens at startup so the
        # user sees only the Playwright-controlled window.
        for context in self._browser.contexts:
            for page in context.pages:
                await page.close()

    async def __aexit__(
        self,
        exc_type: type[BaseException] | None,
        exc_val: BaseException | None,
        exc_tb: object,
    ) -> None:
        """Close browser, Playwright, and clean up CDP resources."""
        if self._context:
            await self._context.close()
        if self._browser:
            await self._browser.close()
        if self._playwright:
            await self._playwright.stop()

        # Clean up CDP subprocess and temp dir
        if self._cdp_process:
            _terminate_process(self._cdp_process)
            self._cdp_process = None
        if self._cdp_tmpdir:
            shutil.rmtree(self._cdp_tmpdir, ignore_errors=True)
            self._cdp_tmpdir = None

    async def new_page(self) -> Page:
        """Create a new page in the managed browser context."""
        if self._context is None:
            msg = "SessionManager not entered — use 'async with'"
            raise RuntimeError(msg)
        return await self._context.new_page()

    async def save_storage_state(self) -> Path:
        """Persist cookies/session to disk for reuse on next run."""
        if self._context is None:
            msg = "SessionManager not entered — use 'async with'"
            raise RuntimeError(msg)

        path = self.config.storage_state_path
        path.parent.mkdir(parents=True, exist_ok=True)
        state = await self._context.storage_state()
        path.write_text(json.dumps(state, indent=2))
        logger.info("Session state saved to %s", path)
        return path

    def has_storage_state(self) -> bool:
        """Check whether a persisted session exists for this board."""
        return self.config.storage_state_path.exists()
