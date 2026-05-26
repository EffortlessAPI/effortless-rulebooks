"""
File logging tests — persistent log files for post-run diagnosis.

Maps to BDD spec: TestFileLogging, TestSessionLogging

Tests verify that run logs are persisted to disk under data/logs/ with
timestamped filenames, configurable log level, and without suppressing
stderr output.  Session logging tests verify JSON-lines formatting
and session ID correlation.
"""

from __future__ import annotations

import json
import logging
import re
from typing import TYPE_CHECKING

from jobsearch_rag.logging import (
    configure_file_logging,
    configure_session_logging,
    log_event,
    logger,
    new_session_id,
    session_logger,
)

if TYPE_CHECKING:
    from pathlib import Path

    import pytest


class TestFileLogging:
    """
    REQUIREMENT: Run logs are persisted to disk for post-run diagnosis.

    WHO: The operator diagnosing a failed or unexpected pipeline run
    WHAT: (1) The system creates a log file under the specified logs directory when file logging is enabled and a message is logged.
          (2) The system names the log file with the `jobsearch-rag_YYYY-MM-DDTHH-MM-SS.log` timestamp pattern.
          (3) The system writes the logged warning message and its level marker to the log file.
          (4) The system creates the log directory automatically when the configured path does not already exist.
          (5) The system keeps the existing stderr `StreamHandler` active when file logging is enabled.
          (6) The system writes a debug message and its `DEBUG` level marker to the log file when the log level is configured as `DEBUG`.
    WHY: Without persistent file logs, transient console output is lost
         after the terminal closes, making post-run diagnosis impossible

    MOCK BOUNDARY:
        Mock:  nothing — uses tmp_path for real filesystem I/O
        Real:  configure_file_logging, logger, file handlers, log directory
        Never: Patch logging internals — exercise the real logging stack
    """

    def test_run_creates_log_file_in_data_logs_directory(self, tmp_path: Path) -> None:
        """
        Given a temporary logs directory
        When file logging is enabled and a message is logged
        Then a log file appears under the specified logs directory
        """
        # Given: a temporary logs directory
        log_dir = tmp_path / "logs"
        handler = configure_file_logging(log_dir=str(log_dir))
        try:
            # When: a message is logged
            logger.info("test message")

            # Then: exactly one log file is created
            log_files = list(log_dir.glob("*.log"))
            assert len(log_files) == 1, (
                f"Expected 1 log file, found {len(log_files)}: {[f.name for f in log_files]}"
            )
        finally:
            logger.removeHandler(handler)

    def test_log_file_name_includes_timestamp(self, tmp_path: Path) -> None:
        """
        Given file logging is enabled in a temporary directory
        When a message is logged
        Then the log filename follows jobsearch-rag_YYYY-MM-DDTHH-MM-SS.log
        """
        # Given: a temporary logs directory
        log_dir = tmp_path / "logs"
        handler = configure_file_logging(log_dir=str(log_dir))
        try:
            # When: a message is logged
            logger.info("timestamp check")

            # Then: the filename matches the timestamp pattern
            log_files = list(log_dir.glob("*.log"))
            assert len(log_files) == 1, f"Expected 1 log file, found {len(log_files)}"
            name = log_files[0].name
            assert re.match(r"jobsearch-rag_\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}\.log$", name), (
                f"Log filename '{name}' does not match timestamp pattern"
            )
        finally:
            logger.removeHandler(handler)

    def test_log_file_contains_same_messages_as_stderr(
        self, tmp_path: Path, capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given file logging is enabled in a temporary directory
        When a warning is logged with file logging enabled
        Then the file contains the message text and log level
        """
        # Given: file logging enabled in a temporary directory
        log_dir = tmp_path / "logs"
        handler = configure_file_logging(log_dir=str(log_dir))
        try:
            # When: a warning message is logged
            logger.warning("duplicate check message")

            # Then: the log file contains the message and level
            log_files = list(log_dir.glob("*.log"))
            assert len(log_files) == 1, f"Expected 1 log file, found {len(log_files)}"
            content = log_files[0].read_text()
            assert "duplicate check message" in content, (
                f"Expected 'duplicate check message' in log content. Got: {content[:200]}"
            )
            assert "WARNING" in content, (
                f"Expected 'WARNING' level in log content. Got: {content[:200]}"
            )
        finally:
            logger.removeHandler(handler)

    def test_log_directory_is_created_if_absent(self, tmp_path: Path) -> None:
        """
        Given a log directory path that does not exist
        When file logging is configured
        Then the directory is created automatically
        """
        # Given: a deeply nested directory that doesn't exist
        log_dir = tmp_path / "nested" / "deep" / "logs"
        assert not log_dir.exists(), (
            f"Expected log_dir to not exist before configuration, but it does: {log_dir}"
        )

        # When: file logging is configured
        handler = configure_file_logging(log_dir=str(log_dir))
        try:
            logger.info("auto-create dir")

            # Then: the directory was created and contains a log file
            assert log_dir.exists(), f"Expected log_dir to be created automatically: {log_dir}"
            log_files = list(log_dir.glob("*.log"))
            assert len(log_files) == 1, (
                f"Expected 1 log file after auto-creation, found {len(log_files)}"
            )
        finally:
            logger.removeHandler(handler)

    def test_stderr_output_is_not_suppressed_when_file_logging_enabled(
        self, tmp_path: Path
    ) -> None:
        """
        Given file logging is enabled alongside stderr output
        When file logging is enabled
        Then the existing stderr StreamHandler remains active
        """
        # Given: file logging enabled
        log_dir = tmp_path / "logs"
        handler = configure_file_logging(log_dir=str(log_dir))
        try:
            # When: inspecting the logger's handlers
            handler_types = [type(h) for h in logger.handlers]

            # Then: a StreamHandler (stderr) is still present
            assert logging.StreamHandler in handler_types, (
                "stderr StreamHandler was removed when file logging was enabled. "
                f"Active handlers: {logger.handlers}"
            )
        finally:
            logger.removeHandler(handler)

    def test_log_level_is_configurable(self, tmp_path: Path) -> None:
        """
        Given file logging configured with DEBUG level
        When a debug message is logged
        Then the file contains the debug message and DEBUG level marker
        """
        # Given: file logging at DEBUG level
        log_dir = tmp_path / "logs"
        handler = configure_file_logging(log_dir=str(log_dir), level=logging.DEBUG)
        try:
            # When: a debug message is logged
            logger.debug("debug-level message")

            # Then: the log file captures the debug output
            log_files = list(log_dir.glob("*.log"))
            assert len(log_files) == 1, f"Expected 1 log file, found {len(log_files)}"
            content = log_files[0].read_text()
            assert "debug-level message" in content, (
                f"Expected 'debug-level message' in log content. Got: {content[:200]}"
            )
            assert "DEBUG" in content, (
                f"Expected 'DEBUG' level in log content. Got: {content[:200]}"
            )
        finally:
            logger.removeHandler(handler)


class TestSessionLogging:
    """
    REQUIREMENT: Session logging writes JSON-lines entries correlated by
    a session ID for structured observability.

    WHO: The operator diagnosing unexpected scores or slow inference by
         reading structured log entries
    WHAT: (1) new_session_id produces an 8-character hex string
          (2) configure_session_logging creates a .jsonl file and returns the session ID
          (3) log_event writes a JSON entry with the event name and extra data
          (4) a plain session_logger.info call (no structured_data) produces
              valid JSON without extra fields
          (5) configure_session_logging auto-generates a session ID when none is provided
          (6) configure_session_logging uses the provided session ID when given
    WHY: Without structured session logs, diagnosing pipeline behaviour
         requires grepping unstructured text

    MOCK BOUNDARY:
        Mock:  nothing — uses tmp_path for real filesystem I/O
        Real:  configure_session_logging, log_event, session_logger, new_session_id
        Never: Patch logging internals — exercise the real logging stack
    """

    def test_new_session_id_produces_8_char_hex_string(self) -> None:
        """
        When new_session_id is called
        Then the result is an 8-character lowercase hex string
        """
        # When: generating a session ID
        sid = new_session_id()

        # Then: it's 8 hex chars
        assert len(sid) == 8, f"Expected 8-char session ID, got {len(sid)}: {sid}"
        assert re.match(r"^[0-9a-f]{8}$", sid), f"Session ID should be lowercase hex, got: {sid}"

    def test_configure_session_logging_creates_jsonl_file(self, tmp_path: Path) -> None:
        """
        Given a temporary logs directory
        When session logging is configured and an event is logged
        Then a .jsonl file appears under the specified directory
        """
        # Given: a temporary directory
        log_dir = tmp_path / "logs"

        # When: session logging is configured and an event emitted
        handler, sid = configure_session_logging(str(log_dir))
        try:
            log_event("test_event", key="value")

            # Then: a .jsonl file exists
            jsonl_files = list(log_dir.glob("*.jsonl"))
            assert len(jsonl_files) == 1, f"Expected 1 .jsonl file, found {len(jsonl_files)}"
            assert sid in jsonl_files[0].name, (
                f"Expected session ID '{sid}' in filename: {jsonl_files[0].name}"
            )
        finally:
            session_logger.removeHandler(handler)

    def test_log_event_writes_json_with_event_name_and_extra_data(self, tmp_path: Path) -> None:
        """
        Given session logging is configured
        When log_event is called with an event name and keyword arguments
        Then the JSON entry contains the event name and all keyword data
        """
        # Given: session logging configured
        log_dir = tmp_path / "logs"
        handler, _sid = configure_session_logging(str(log_dir))
        try:
            # When: an event is logged with extra data
            log_event("score_computed", job_id="abc123", score=0.75)

            # Then: the JSON entry contains event + extra data
            jsonl_files = list(log_dir.glob("*.jsonl"))
            content = jsonl_files[0].read_text().strip()
            entry = json.loads(content)
            assert entry["event"] == "score_computed", (
                f"Expected event 'score_computed', got: {entry.get('event')}"
            )
            assert entry["job_id"] == "abc123", (
                f"Expected job_id 'abc123', got: {entry.get('job_id')}"
            )
            assert entry["score"] == 0.75, f"Expected score 0.75, got: {entry.get('score')}"
        finally:
            session_logger.removeHandler(handler)

    def test_plain_session_log_produces_valid_json_without_extra_fields(
        self,
        tmp_path: Path,
    ) -> None:
        """
        Given session logging is configured
        When a plain session_logger.info call is made (no structured_data)
        Then the entry is valid JSON with event defaulting to 'log'
        And no extra structured fields are merged
        """
        # Given: session logging configured
        log_dir = tmp_path / "logs"
        handler, sid = configure_session_logging(str(log_dir))
        try:
            # When: a plain info message is logged (not via log_event)
            session_logger.info("plain message")

            # Then: valid JSON with default event
            jsonl_files = list(log_dir.glob("*.jsonl"))
            content = jsonl_files[0].read_text().strip()
            entry = json.loads(content)
            assert entry["event"] == "log", (
                f"Expected default event 'log', got: {entry.get('event')}"
            )
            assert entry["session"] == sid, (
                f"Expected session '{sid}', got: {entry.get('session')}"
            )
            assert entry["message"] == "plain message", (
                f"Expected message 'plain message', got: {entry.get('message')}"
            )
        finally:
            session_logger.removeHandler(handler)

    def test_configure_session_logging_auto_generates_id_when_none_provided(
        self,
        tmp_path: Path,
    ) -> None:
        """
        Given no session_id argument is passed
        When configure_session_logging is called
        Then a valid 8-char hex session ID is returned
        """
        # Given/When: session logging with no explicit ID
        log_dir = tmp_path / "logs"
        handler, sid = configure_session_logging(str(log_dir))
        try:
            # Then: auto-generated session ID is valid
            assert re.match(r"^[0-9a-f]{8}$", sid), (
                f"Auto-generated session ID should be 8 hex chars, got: {sid}"
            )
        finally:
            session_logger.removeHandler(handler)

    def test_configure_session_logging_uses_provided_session_id(
        self,
        tmp_path: Path,
    ) -> None:
        """
        Given a specific session_id is passed
        When configure_session_logging is called
        Then the returned session ID matches what was provided
        """
        # Given: a known session ID
        log_dir = tmp_path / "logs"
        handler, sid = configure_session_logging(str(log_dir), session_id="cafebabe")
        try:
            # Then: returned ID matches
            assert sid == "cafebabe", f"Expected 'cafebabe', got: {sid}"
        finally:
            session_logger.removeHandler(handler)
