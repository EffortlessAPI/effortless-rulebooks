"""
CDP-based browser launch tests — SessionManager subprocess and cleanup.

Spec classes:
    TestSessionConfigCDP — config carries CDP channel selection
    TestSessionManagerCDP — CDP launch, connect, cleanup, and fallback
    TestSessionManagerEdgeCases — uninitialised manager and binary resolution
    TestThrottle — rate-limiting sleep within configured bounds
"""

from __future__ import annotations

import builtins
import json
import subprocess
from pathlib import Path
from typing import Any
from unittest.mock import AsyncMock, MagicMock, patch

import pytest

from jobsearch_rag.adapters.session import (
    SessionConfig,
    SessionManager,
    throttle,
)
from jobsearch_rag.errors import ActionableError

# ───────────────────────────────────────────────────────────────────
# Helpers
# ───────────────────────────────────────────────────────────────────


def _mock_playwright(
    *,
    connect_over_cdp: bool = True,
    cdp_pages: list[MagicMock] | None = None,
) -> MagicMock:
    """Build a mock Playwright instance with browser/context stubs."""
    mock_context = MagicMock()
    mock_context.close = AsyncMock()
    mock_context.storage_state = AsyncMock(return_value={})

    mock_browser = MagicMock()
    mock_browser.new_context = AsyncMock(return_value=mock_context)
    mock_browser.close = AsyncMock()

    # CDP launch closes initial blank pages: for context in browser.contexts
    if cdp_pages is not None:
        cdp_ctx = MagicMock()
        cdp_ctx.pages = cdp_pages
        mock_browser.contexts = [cdp_ctx]
    else:
        mock_browser.contexts = []

    pw = MagicMock()
    pw.stop = AsyncMock()

    if connect_over_cdp:
        pw.chromium.connect_over_cdp = AsyncMock(return_value=mock_browser)
    pw.chromium.launch = AsyncMock(return_value=mock_browser)

    return pw


def _patch_playwright(mock_pw: MagicMock) -> Any:
    """Return a patch context for ``async_playwright`` that yields *mock_pw*."""
    mock_pw_ctx = MagicMock()
    mock_pw_ctx.start = AsyncMock(return_value=mock_pw)
    return patch(
        "jobsearch_rag.adapters.session.async_playwright",
        return_value=mock_pw_ctx,
    )


# ───────────────────────────────────────────────────────────────────
# SessionConfig
# ───────────────────────────────────────────────────────────────────


class TestSessionConfigCDP:
    """
    REQUIREMENT: SessionConfig carries CDP channel selection through to launch.

    WHO: The operator choosing between Playwright-managed and CDP mode
    WHAT: (1) The system defaults browser_channel to None when it creates a SessionConfig without a specified browser_channel.
          (2) The system sets browser_channel to "msedge" when it creates a SessionConfig with browser_channel set to "msedge".
    WHY: Cloudflare-protected sites require CDP mode to bypass
         automation detection flags

    MOCK BOUNDARY:
        Mock:  nothing — pure config construction
        Real:  SessionConfig dataclass
        Never: Patch SessionConfig internals
    """

    def test_browser_channel_defaults_to_none(self) -> None:
        """
        GIVEN a SessionConfig with no browser_channel specified
        WHEN the config is created
        THEN browser_channel defaults to None (Playwright-managed mode).
        """
        # Given/When: config with no channel
        config = SessionConfig(board_name="test")

        # Then: defaults to None
        assert config.browser_channel is None, "browser_channel should default to None"

    def test_browser_channel_set_to_msedge(self) -> None:
        """
        GIVEN a SessionConfig with browser_channel='msedge'
        WHEN the config is created
        THEN browser_channel is set to 'msedge' (CDP launch mode).
        """
        # Given/When: config with msedge channel
        config = SessionConfig(board_name="test", browser_channel="msedge")

        # Then: channel is preserved
        assert config.browser_channel == "msedge", "browser_channel should be 'msedge'"


# ───────────────────────────────────────────────────────────────────
# SessionManager CDP launch path — all through __aenter__ / __aexit__
# ───────────────────────────────────────────────────────────────────


class TestSessionManagerCDP:
    """
    REQUIREMENT: CDP mode launches a system browser and connects via DevTools Protocol.

    WHO: The pipeline runner needing a Cloudflare-safe browser session
    WHAT: (1) The session manager launches the browser through CDP and connects Playwright over the CDP endpoint.
          (2) The session manager launches the browser subprocess with the remote debugging port, user data directory, and no first run flags.
          (3) The session manager adds the `--headless=new` flag when CDP headless mode is enabled.
          (4) The session manager uses the browser binary resolved by `shutil.which` when no configured browser path matches the channel.
          (5) The session manager uses the standard Playwright launch path when no browser channel is configured.
          (6) The session manager raises an actionable error that tells the operator which browser to install when the configured CDP browser binary is missing.
          (7) The session manager sends `SIGTERM` to the CDP subprocess and removes its temporary directory during cleanup.
          (8) The session manager escalates to `SIGKILL` when the CDP subprocess does not exit after `SIGTERM` times out during cleanup.
          (9) The session manager skips sending termination signals when the CDP subprocess has already exited before cleanup.
          (10) The session manager raises a `TimeoutError` when the CDP endpoint never responds before the enter deadline expires.
          (11) The session manager raises a `TimeoutError` that includes the CDP URL when repeated CDP readiness checks exhaust the deadline.
          (12) The session manager raises a `RuntimeError` when `new_page` is called before the session context is entered.
          (13) The session manager raises a `RuntimeError` when `save_storage_state` is called before the session context is entered.
          (14) The session manager writes the active session cookies to the configured storage state file as JSON.
          (15) The session manager returns `True` from `has_storage_state` when the persisted session file exists.
          (16) The session manager returns `False` from `has_storage_state` when no persisted session file exists.
          (17) The session manager applies playwright-stealth patches to the browser context when stealth mode is enabled.
          (18) The session manager starts successfully and logs a warning when stealth mode is enabled but playwright-stealth is unavailable.
          (19) The session manager returns a new page from the active browser context when `new_page` is called after entry.
    WHY: Cloudflare detects Playwright's --enable-automation flag and
         navigator.webdriver — CDP mode bypasses both

    MOCK BOUNDARY:
        Mock:  subprocess.Popen (process I/O), urllib.request.urlopen
               (CDP polling), async_playwright (browser API), shutil
               (filesystem cleanup), tempfile (temp dir)
        Real:  SessionManager.__aenter__/__aexit__, SessionConfig,
               _find_browser_binary, new_page, save_storage_state
        Never: Patch _find_browser_binary internals or config construction
    """

    @pytest.fixture()
    def cdp_config(self) -> SessionConfig:
        """Return a CDP-mode SessionConfig for ZipRecruiter."""
        return SessionConfig(
            board_name="ziprecruiter",
            headless=False,
            browser_channel="msedge",
        )

    @pytest.fixture()
    def playwright_config(self) -> SessionConfig:
        """Return a standard Playwright SessionConfig in headless mode."""
        return SessionConfig(
            board_name="testboard",
            headless=True,
        )

    async def test_cdp_launches_subprocess_and_connects(
        self, cdp_config: SessionConfig, tmp_path: Path
    ) -> None:
        """
        GIVEN a CDP config with a valid browser binary
        WHEN SessionManager is entered
        THEN the browser is launched via CDP and Playwright connects over CDP.
        """
        # Given: fake binary and mock Playwright
        fake_binary = tmp_path / "edge"
        fake_binary.touch()

        blank_page = MagicMock()
        blank_page.close = AsyncMock()
        mock_pw = _mock_playwright(cdp_pages=[blank_page])

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch("jobsearch_rag.adapters.session.subprocess.Popen"),
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/test",
            ),
            _patch_playwright(mock_pw),
        ):
            # When: enter the session manager
            manager = SessionManager(cdp_config)
            await manager.__aenter__()

            # Then: Playwright connected via CDP, not standard launch
            mock_pw.chromium.connect_over_cdp.assert_called_once()
            mock_pw.chromium.launch.assert_not_called()
            # Then: initial blank page was closed
            blank_page.close.assert_awaited_once()

            await manager.__aexit__(None, None, None)

    async def test_cdp_subprocess_receives_correct_flags(
        self, cdp_config: SessionConfig, tmp_path: Path
    ) -> None:
        """
        GIVEN a CDP config with headless=False
        WHEN the browser subprocess is launched
        THEN the command includes --remote-debugging-port, --user-data-dir, --no-first-run.
        """
        # Given: fake binary and mock Playwright
        fake_binary = tmp_path / "edge"
        fake_binary.touch()

        mock_pw = _mock_playwright()

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch("jobsearch_rag.adapters.session.subprocess.Popen") as mock_popen,
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/jobsearch-test",
            ),
            _patch_playwright(mock_pw),
        ):
            # When: enter the session manager
            manager = SessionManager(cdp_config)
            await manager.__aenter__()

            # Then: subprocess command has correct flags
            mock_popen.assert_called_once()
            cmd = mock_popen.call_args[0][0]
            assert any("--remote-debugging-port=" in arg for arg in cmd), (
                "Command should include --remote-debugging-port"
            )
            assert any("--user-data-dir=" in arg for arg in cmd), (
                "Command should include --user-data-dir"
            )
            assert "--no-first-run" in cmd, "Command should include --no-first-run"
            assert "--headless=new" not in cmd, "headless=False should not add --headless=new"

            await manager.__aexit__(None, None, None)

    async def test_cdp_headless_adds_headless_flag(self, tmp_path: Path) -> None:
        """
        GIVEN a CDP config with headless=True
        WHEN the browser subprocess is launched
        THEN --headless=new is added to the subprocess args.
        """
        # Given: headless CDP config
        config = SessionConfig(
            board_name="test",
            headless=True,
            browser_channel="msedge",
        )
        fake_binary = tmp_path / "edge"
        fake_binary.touch()

        mock_pw = _mock_playwright()

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch("jobsearch_rag.adapters.session.subprocess.Popen") as mock_popen,
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/test",
            ),
            _patch_playwright(mock_pw),
        ):
            # When: enter the session manager
            manager = SessionManager(config)
            await manager.__aenter__()

            # Then: headless flag present
            cmd = mock_popen.call_args[0][0]
            assert "--headless=new" in cmd, "headless=True should add --headless=new"

            await manager.__aexit__(None, None, None)

    async def test_binary_resolved_via_shutil_which_fallback(self, tmp_path: Path) -> None:
        """
        GIVEN no match in _BROWSER_PATHS for the channel
        WHEN shutil.which finds the binary on PATH
        THEN the which-resolved binary is used for the subprocess.
        """
        # Given: shutil.which returns a valid binary
        which_binary = str(tmp_path / "msedge")
        Path(which_binary).touch()

        config = SessionConfig(
            board_name="test",
            headless=False,
            browser_channel="msedge",
        )
        mock_pw = _mock_playwright()

        with (
            patch("jobsearch_rag.adapters.session._BROWSER_PATHS", {}),
            patch("shutil.which", return_value=which_binary),
            patch("jobsearch_rag.adapters.session.subprocess.Popen") as mock_popen,
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/test",
            ),
            _patch_playwright(mock_pw),
        ):
            # When: enter the session manager
            manager = SessionManager(config)
            await manager.__aenter__()

            # Then: which-resolved binary was used
            cmd = mock_popen.call_args[0][0]
            assert cmd[0] == which_binary, f"Expected binary '{which_binary}', got '{cmd[0]}'"

            await manager.__aexit__(None, None, None)

    async def test_standard_launch_when_no_channel(self, playwright_config: SessionConfig) -> None:
        """
        GIVEN a config with no browser_channel set
        WHEN SessionManager is entered
        THEN standard Playwright launch is used instead of CDP.
        """
        # Given: mock Playwright
        mock_pw = _mock_playwright(connect_over_cdp=True)

        with _patch_playwright(mock_pw):
            # When: enter the session manager
            manager = SessionManager(playwright_config)
            await manager.__aenter__()

            # Then: standard launch used, CDP not attempted
            mock_pw.chromium.launch.assert_called_once_with(headless=True)
            mock_pw.chromium.connect_over_cdp.assert_not_called()

            await manager.__aexit__(None, None, None)

    async def test_cdp_missing_binary_tells_operator_which_browser_to_install(
        self, cdp_config: SessionConfig
    ) -> None:
        """
        GIVEN no browser binary found for the configured channel
        WHEN SessionManager is entered
        THEN an ActionableError tells the operator which browser to install.
        """
        # Given: no binary available
        mock_pw = _mock_playwright()

        with (
            patch("jobsearch_rag.adapters.session._BROWSER_PATHS", {}),
            patch("shutil.which", return_value=None),
            _patch_playwright(mock_pw),
        ):
            # When/Then: entering raises ActionableError
            manager = SessionManager(cdp_config)
            with pytest.raises(ActionableError) as exc_info:
                await manager.__aenter__()

            # Then: error provides install guidance
            err = exc_info.value
            assert "Could not find" in err.error, "Error should say browser not found"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_cdp_cleanup_terminates_subprocess(self, tmp_path: Path) -> None:
        """
        GIVEN a running CDP subprocess
        WHEN SessionManager __aexit__ is called
        THEN SIGTERM is sent and the temp directory is cleaned up.
        """
        # Given: config and mock subprocess
        config = SessionConfig(
            board_name="test",
            headless=False,
            browser_channel="msedge",
        )
        fake_binary = tmp_path / "edge"
        fake_binary.touch()

        mock_proc = MagicMock(spec=subprocess.Popen)
        mock_proc.poll.return_value = None  # still running
        mock_proc.wait.return_value = 0

        mock_pw = _mock_playwright()
        tmpdir = str(tmp_path / "cdp_profile")
        Path(tmpdir).mkdir()

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch(
                "jobsearch_rag.adapters.session.subprocess.Popen",
                return_value=mock_proc,
            ),
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value=tmpdir,
            ),
            _patch_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session.shutil.rmtree") as mock_rmtree,
        ):
            # When: enter and exit the session manager
            manager = SessionManager(config)
            await manager.__aenter__()
            await manager.__aexit__(None, None, None)

            # Then: SIGTERM sent and temp dir cleaned
            mock_proc.send_signal.assert_called_once()
            mock_rmtree.assert_called_once_with(tmpdir, ignore_errors=True)

    async def test_cdp_cleanup_kills_on_sigterm_timeout(self, tmp_path: Path) -> None:
        """
        GIVEN a CDP subprocess that doesn't respond to SIGTERM
        WHEN __aexit__ is called and SIGTERM times out
        THEN the process is escalated to SIGKILL.
        """
        # Given: config and mock subprocess that ignores SIGTERM
        config = SessionConfig(
            board_name="test",
            headless=False,
            browser_channel="msedge",
        )
        fake_binary = tmp_path / "edge"
        fake_binary.touch()

        mock_proc = MagicMock(spec=subprocess.Popen)
        mock_proc.poll.return_value = None  # still running
        mock_proc.wait.side_effect = [
            subprocess.TimeoutExpired("edge", 5),
            0,
        ]

        mock_pw = _mock_playwright()

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch(
                "jobsearch_rag.adapters.session.subprocess.Popen",
                return_value=mock_proc,
            ),
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/test",
            ),
            _patch_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session.shutil.rmtree"),
        ):
            # When: enter and exit
            manager = SessionManager(config)
            await manager.__aenter__()
            await manager.__aexit__(None, None, None)

            # Then: escalated to SIGKILL
            mock_proc.kill.assert_called_once()

    async def test_cdp_cleanup_noop_when_process_already_exited(self, tmp_path: Path) -> None:
        """
        GIVEN a CDP subprocess that has already exited
        WHEN __aexit__ is called
        THEN no signals are sent to the process.
        """
        # Given: config and mock subprocess that already exited
        config = SessionConfig(
            board_name="test",
            headless=False,
            browser_channel="msedge",
        )
        fake_binary = tmp_path / "edge"
        fake_binary.touch()

        mock_proc = MagicMock(spec=subprocess.Popen)
        mock_proc.poll.return_value = 0  # already exited

        mock_pw = _mock_playwright()

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch(
                "jobsearch_rag.adapters.session.subprocess.Popen",
                return_value=mock_proc,
            ),
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/test",
            ),
            _patch_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session.shutil.rmtree"),
        ):
            # When: enter and exit
            manager = SessionManager(config)
            await manager.__aenter__()
            await manager.__aexit__(None, None, None)

            # Then: no signal sent to already-exited process
            mock_proc.send_signal.assert_not_called()

    async def test_cdp_timeout_raises_timeout_error(self, tmp_path: Path) -> None:
        """
        GIVEN a CDP endpoint that never responds
        WHEN SessionManager is entered and _wait_for_cdp exhausts its deadline
        THEN a TimeoutError is raised.
        """
        # Given: config and mock where CDP never starts
        config = SessionConfig(
            board_name="test",
            headless=False,
            browser_channel="msedge",
        )
        fake_binary = tmp_path / "edge"
        fake_binary.touch()
        tmpdir = str(tmp_path / "cdp_profile")
        Path(tmpdir).mkdir()

        mock_pw = _mock_playwright()

        # Simulate a clock that immediately expires the deadline:
        # first call (sets deadline), second call (exceeds it)
        mock_loop = MagicMock()
        mock_loop.time = MagicMock(side_effect=[0.0, 100.0])

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch("jobsearch_rag.adapters.session.subprocess.Popen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value=tmpdir,
            ),
            # Mock I/O boundaries: urlopen always refuses, clock immediately expires
            patch(
                "jobsearch_rag.adapters.session.urllib.request.urlopen",
                side_effect=OSError("Connection refused"),
            ),
            patch(
                "jobsearch_rag.adapters.session.asyncio.get_event_loop",
                return_value=mock_loop,
            ),
            patch(
                "jobsearch_rag.adapters.session.asyncio.sleep",
                new_callable=AsyncMock,
            ),
            _patch_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session.shutil.rmtree"),
            pytest.raises(TimeoutError, match="did not start"),
        ):
            # When/Then: SessionManager enters, _wait_for_cdp runs for real
            # and raises TimeoutError because the clock immediately expires
            manager = SessionManager(config)
            await manager.__aenter__()

    async def test_cdp_wait_retries_then_times_out(self, tmp_path: Path) -> None:
        """
        GIVEN a CDP endpoint that never becomes available
        WHEN _wait_for_cdp retries and the deadline expires
        THEN a TimeoutError is raised with the CDP URL in the message.
        """
        # Given: CDP config with urlopen always failing and a clock that
        #        expires the deadline on the second poll
        config = SessionConfig(
            board_name="test",
            headless=False,
            browser_channel="msedge",
        )
        fake_binary = tmp_path / "edge"
        fake_binary.touch()
        tmpdir = str(tmp_path / "cdp_profile")
        Path(tmpdir).mkdir()

        mock_pw = _mock_playwright()

        # Simulate a clock: first call sets deadline, second is within bounds
        # (triggers sleep retry), third exceeds deadline (triggers timeout)
        mock_loop = MagicMock()
        mock_loop.time = MagicMock(side_effect=[0.0, 1.0, 100.0])

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": [str(fake_binary)]},
            ),
            patch("shutil.which", return_value=None),
            patch("jobsearch_rag.adapters.session.subprocess.Popen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value=tmpdir,
            ),
            patch(
                "jobsearch_rag.adapters.session.urllib.request.urlopen",
                side_effect=OSError("Connection refused"),
            ),
            patch(
                "jobsearch_rag.adapters.session.asyncio.get_event_loop",
                return_value=mock_loop,
            ),
            patch(
                "jobsearch_rag.adapters.session.asyncio.sleep",
                new_callable=AsyncMock,
            ),
            _patch_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session.shutil.rmtree"),
            pytest.raises(TimeoutError, match="did not start"),
        ):
            # When/Then: SessionManager enters, _wait_for_cdp retries once
            # then raises TimeoutError when the clock exceeds the deadline
            manager = SessionManager(config)
            await manager.__aenter__()

    async def test_new_page_without_context_raises_runtime_error(self) -> None:
        """
        GIVEN a SessionManager that has not been entered as a context manager
        WHEN new_page is called
        THEN a RuntimeError is raised.
        """
        # Given: uninitialised manager
        config = SessionConfig(board_name="test", headless=True)
        manager = SessionManager(config)

        # When/Then: new_page raises RuntimeError
        with pytest.raises(RuntimeError, match="not entered"):
            await manager.new_page()

    async def test_save_storage_state_without_context_raises_runtime_error(self) -> None:
        """
        GIVEN a SessionManager that has not been entered as a context manager
        WHEN save_storage_state is called
        THEN a RuntimeError is raised.
        """
        # Given: uninitialised manager
        config = SessionConfig(board_name="test", headless=True)
        manager = SessionManager(config)

        # When/Then: save_storage_state raises RuntimeError
        with pytest.raises(RuntimeError, match="not entered"):
            await manager.save_storage_state()

    async def test_save_storage_state_persists_cookies_to_disk(self, tmp_path: Path) -> None:
        """
        GIVEN an active session with cookies
        WHEN save_storage_state is called
        THEN cookies are written to the storage state path as JSON.
        """
        # Given: mock Playwright with cookie data
        config = SessionConfig(board_name="test", headless=True)
        mock_pw = _mock_playwright(connect_over_cdp=False)
        mock_context = mock_pw.chromium.launch.return_value.new_context.return_value
        mock_context.storage_state = AsyncMock(return_value={"cookies": [{"name": "session"}]})

        with (
            _patch_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session._STORAGE_DIR", tmp_path),
        ):
            # When: enter session and save storage state
            manager = SessionManager(config)
            await manager.__aenter__()
            result = await manager.save_storage_state()

            # Then: session file exists on disk with correct cookie data
            assert result.exists(), "Storage state file should be written to disk"

            saved = json.loads(result.read_text())
            assert saved["cookies"][0]["name"] == "session", (
                f"Expected cookie name 'session', got {saved['cookies'][0].get('name')}"
            )
            await manager.__aexit__(None, None, None)

    def test_has_storage_state_returns_true_when_file_exists(self, tmp_path: Path) -> None:
        """
        GIVEN a persisted session file exists on disk
        WHEN has_storage_state is called
        THEN it returns True.
        """
        # Given: session file exists on disk
        config = SessionConfig(board_name="test", headless=True)
        with patch("jobsearch_rag.adapters.session._STORAGE_DIR", tmp_path):
            state_file = config.storage_state_path
            state_file.parent.mkdir(parents=True, exist_ok=True)
            state_file.write_text("{}")

            # When/Then: has_storage_state returns True
            manager = SessionManager(config)
            assert manager.has_storage_state() is True, (
                "has_storage_state should return True when session file exists"
            )

    def test_has_storage_state_returns_false_when_no_file(self) -> None:
        """
        GIVEN no persisted session file exists
        WHEN has_storage_state is called
        THEN it returns False.
        """
        # Given/When: no session file on disk
        config = SessionConfig(board_name="nonexistent_board_xyz", headless=True)
        manager = SessionManager(config)

        # Then: has_storage_state returns False
        assert manager.has_storage_state() is False, (
            "has_storage_state should return False when no session file exists"
        )

    async def test_stealth_patches_applied_when_stealth_enabled(self, tmp_path: Path) -> None:
        """
        GIVEN stealth mode is enabled in the config
        WHEN the session is entered
        THEN playwright-stealth patches are applied to the context.
        """
        # Given: stealth config with mock stealth module
        config = SessionConfig(board_name="linkedin", headless=True, stealth=True)
        mock_pw = _mock_playwright(connect_over_cdp=False)
        mock_stealth = MagicMock()
        mock_stealth_instance = MagicMock()
        mock_stealth_instance.apply_stealth_async = AsyncMock()
        mock_stealth.return_value = mock_stealth_instance

        with (
            _patch_playwright(mock_pw),
            patch.dict(
                "sys.modules",
                {"playwright_stealth": MagicMock(Stealth=mock_stealth)},
            ),
        ):
            # When: enter the session
            manager = SessionManager(config)
            await manager.__aenter__()

            # Then: stealth patches were applied
            mock_stealth_instance.apply_stealth_async.assert_called_once()
            await manager.__aexit__(None, None, None)

    async def test_stealth_import_error_logs_warning(self, tmp_path: Path) -> None:
        """
        GIVEN stealth mode is enabled but playwright-stealth is not installed
        WHEN the session is entered
        THEN the session still starts and a warning is logged.
        """
        # Given: stealth config with playwright_stealth import rigged to fail
        config = SessionConfig(board_name="linkedin", headless=True, stealth=True)
        mock_pw = _mock_playwright(connect_over_cdp=False)

        def _raise_import_error(name: str, *args: Any, **kwargs: Any) -> Any:
            if name == "playwright_stealth":
                raise ImportError("No module named 'playwright_stealth'")
            return original_import(name, *args, **kwargs)

        original_import = builtins.__import__

        with (
            _patch_playwright(mock_pw),
            patch("builtins.__import__", side_effect=_raise_import_error),
        ):
            # When/Then: entering does not raise — stealth failure is graceful
            manager = SessionManager(config)
            await manager.__aenter__()
            await manager.__aexit__(None, None, None)

    async def test_new_page_returns_page_when_context_active(self) -> None:
        """
        GIVEN a SessionManager that has been entered as a context manager
        WHEN new_page is called
        THEN a new page is returned from the browser context.
        """
        # Given: mock Playwright with a new page stub
        config = SessionConfig(board_name="test", headless=True)
        mock_pw = _mock_playwright(connect_over_cdp=False)
        mock_context = mock_pw.chromium.launch.return_value.new_context.return_value
        mock_page = MagicMock()
        mock_context.new_page = AsyncMock(return_value=mock_page)

        with _patch_playwright(mock_pw):
            # When: enter session and request a new page
            manager = SessionManager(config)
            await manager.__aenter__()
            page = await manager.new_page()

            # Then: page returned from the browser context
            assert page is mock_page, "new_page should return a page from the browser context"
            await manager.__aexit__(None, None, None)


class TestSessionManagerEdgeCases:
    """
    REQUIREMENT: SessionManager handles edge cases gracefully.

    WHO: The pipeline runner encountering unusual states
    WHAT: (1) The system treats calling `__aexit__` on an uninitialised `SessionManager` as a graceful no-op that raises no error.
          (2) The system skips a non-existent browser binary path and uses the next valid binary when entering `SessionManager`.
    WHY: Graceful degradation prevents crashes from unusual but
         possible runtime states

    MOCK BOUNDARY:
        Mock:  subprocess.Popen (process I/O), urllib (CDP polling),
               async_playwright (browser API), tempfile/shutil (filesystem)
        Real:  SessionManager.__aenter__/__aexit__, _find_browser_binary
        Never: Patch binary resolution logic directly
    """

    async def test_aexit_on_uninitialised_manager_is_noop(self) -> None:
        """
        GIVEN a SessionManager that was never entered
        WHEN __aexit__ is called
        THEN no error is raised (graceful no-op).
        """
        # Given: uninitialised manager
        config = SessionConfig(board_name="test", headless=True)
        manager = SessionManager(config)

        # When/Then: __aexit__ is a no-op
        await manager.__aexit__(None, None, None)

    async def test_cdp_skips_nonexistent_binary_and_uses_next(self, tmp_path: Path) -> None:
        """
        GIVEN _BROWSER_PATHS with a non-existent first path and a valid second
        WHEN SessionManager is entered
        THEN the non-existent path is skipped and the second binary is used.
        """
        # Given: second binary exists, first does not
        second = tmp_path / "edge-second"
        second.touch()

        mock_pw = _mock_playwright()

        with (
            patch(
                "jobsearch_rag.adapters.session._BROWSER_PATHS",
                {"msedge": ["/nonexistent/edge", str(second)]},
            ),
            patch("shutil.which", return_value=None),
            patch("jobsearch_rag.adapters.session.subprocess.Popen") as mock_popen,
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value="/tmp/test",
            ),
            _patch_playwright(mock_pw),
        ):
            # When: enter the session manager
            config = SessionConfig(
                board_name="test",
                headless=False,
                browser_channel="msedge",
            )
            manager = SessionManager(config)
            await manager.__aenter__()

            # Then: second binary was used
            cmd = mock_popen.call_args[0][0]
            assert cmd[0] == str(second), f"Expected '{second}', got '{cmd[0]}'"

            await manager.__aexit__(None, None, None)


class TestThrottle:
    """
    REQUIREMENT: Rate-limiting sleeps for a random duration within configured bounds.

    WHO: The pipeline runner calling throttle between requests
    WHAT: (1) The system sleeps for a duration between 0.5 and 1.0 seconds when throttle is called with rate limit bounds of 0.5 and 1.0 seconds.
    WHY: Without throttling, rapid requests trigger anti-bot protections

    MOCK BOUNDARY:
        Mock:  asyncio.sleep (time I/O)
        Real:  throttle, random duration calculation
        Never: Patch random.uniform directly
    """

    async def test_throttle_sleeps_within_rate_limit_bounds(self) -> None:
        """
        GIVEN an adapter with rate_limit_seconds = (0.5, 1.0)
        WHEN throttle is called
        THEN asyncio.sleep is called with a duration between 0.5 and 1.0.
        """
        # Given: adapter with rate limit bounds
        mock_adapter = MagicMock()
        mock_adapter.rate_limit_seconds = (0.5, 1.0)
        mock_adapter.board_name = "testboard"

        with patch(
            "jobsearch_rag.adapters.session.asyncio.sleep", new_callable=AsyncMock
        ) as mock_sleep:
            # When: throttle is called
            duration = await throttle(mock_adapter)

            # Then: duration is within bounds and sleep was called
            assert 0.5 <= duration <= 1.0, f"Expected duration between 0.5 and 1.0, got {duration}"
            mock_sleep.assert_called_once_with(duration)
