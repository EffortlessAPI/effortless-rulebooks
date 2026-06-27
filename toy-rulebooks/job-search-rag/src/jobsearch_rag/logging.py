"""
Logging configuration for jobsearch-rag.

Sets up standard logging to stderr with a consistent format.
Import the ``logger`` instance from this module throughout the codebase.

Call :func:`configure_file_logging` to add a timestamped file handler
under ``data/logs/`` for post-run diagnosis.

Call :func:`configure_session_logging` to add a JSON-lines file handler
correlated by a session ID for structured observability.
"""

from __future__ import annotations

import json
import logging
import sys
import uuid
from datetime import UTC, datetime
from pathlib import Path

# Create logger
logger = logging.getLogger("jobsearch-rag")
logger.setLevel(logging.INFO)

# Create handler to stderr
handler = logging.StreamHandler(sys.stderr)
handler.setLevel(logging.INFO)

# Create formatter (shared between stderr and file handlers)
_LOG_FORMAT = "%(asctime)s - %(name)s - %(levelname)s - %(message)s"
_LOG_DATEFMT = "%Y-%m-%d %H:%M:%S"

formatter = logging.Formatter(_LOG_FORMAT, datefmt=_LOG_DATEFMT)
handler.setFormatter(formatter)

# Add handler to logger
logger.addHandler(handler)

# Default log directory
DEFAULT_LOG_DIR = "data/logs"


def configure_file_logging(
    log_dir: str = DEFAULT_LOG_DIR,
    *,
    level: int = logging.INFO,
) -> logging.FileHandler:
    """
    Add a timestamped file handler to the logger.

    Creates ``log_dir`` if it does not exist.  Returns the handler so
    callers (or tests) can remove it later.

    Args:
        log_dir: Directory for log files.  Created automatically.
        level: Logging level for the file handler (default: INFO).

    Returns:
        The :class:`logging.FileHandler` that was added.

    """
    log_path = Path(log_dir)
    log_path.mkdir(parents=True, exist_ok=True)

    timestamp = datetime.now().strftime("%Y-%m-%dT%H-%M-%S")
    filename = log_path / f"jobsearch-rag_{timestamp}.log"

    file_handler = logging.FileHandler(str(filename), encoding="utf-8")
    file_handler.setLevel(level)
    file_handler.setFormatter(logging.Formatter(_LOG_FORMAT, datefmt=_LOG_DATEFMT))

    # Ensure the logger captures messages at the lowest requested level
    if level < logger.level:
        logger.setLevel(level)

    logger.addHandler(file_handler)
    return file_handler


def new_session_id() -> str:
    """Generate a short session ID for log correlation."""
    return uuid.uuid4().hex[:8]


class _JsonLinesFormatter(logging.Formatter):
    """Format log records as JSON-lines with a fixed session ID."""

    def __init__(self, session_id: str) -> None:
        super().__init__()
        self._session_id = session_id

    def format(self, record: logging.LogRecord) -> str:
        entry: dict[str, object] = {
            "ts": datetime.fromtimestamp(record.created, tz=UTC)
            .isoformat()
            .replace("+00:00", "Z"),
            "session": self._session_id,
            "level": record.levelname,
            "event": getattr(record, "event", "log"),
            "message": record.getMessage(),
        }
        # Merge any structured extra data set via log_event()
        extra: dict[str, object] | None = getattr(record, "structured_data", None)
        if extra:
            entry.update(extra)
        return json.dumps(entry, default=str)


# Session logger — separate from the stderr logger so session events
# only go to the JSON-lines file and don't clutter console output.
session_logger = logging.getLogger("jobsearch-rag.session")
session_logger.propagate = False


def configure_session_logging(
    log_dir: str = DEFAULT_LOG_DIR,
    session_id: str | None = None,
    *,
    level: int = logging.INFO,
) -> tuple[logging.FileHandler, str]:
    """
    Add a JSON-lines file handler correlated by *session_id*.

    Creates ``log_dir`` if it does not exist.  Returns the handler and
    the session ID that was used (auto-generated if *session_id* is ``None``).
    """
    sid = session_id or new_session_id()
    log_path = Path(log_dir)
    log_path.mkdir(parents=True, exist_ok=True)

    timestamp = datetime.now().strftime("%Y-%m-%dT%H-%M-%S")
    filename = log_path / f"session_{sid}_{timestamp}.jsonl"

    file_handler = logging.FileHandler(str(filename), encoding="utf-8")
    file_handler.setLevel(level)
    file_handler.setFormatter(_JsonLinesFormatter(sid))

    if level < session_logger.level or session_logger.level == logging.NOTSET:
        session_logger.setLevel(level)

    session_logger.addHandler(file_handler)
    return file_handler, sid


def log_event(event: str, **data: object) -> None:
    """
    Emit a structured event to the session logger.

    The *event* name and all keyword arguments are serialized as JSON
    fields in the log entry.  Only written if a session handler has been
    configured via :func:`configure_session_logging`.
    """
    record = session_logger.makeRecord(
        name=session_logger.name,
        level=logging.INFO,
        fn="",
        lno=0,
        msg=event,
        args=(),
        exc_info=None,
    )
    record.event = event  # type: ignore[attr-defined]  # dynamic attribute for _JsonLinesFormatter
    record.structured_data = data  # type: ignore[attr-defined]  # dynamic attribute for _JsonLinesFormatter
    session_logger.handle(record)


__all__ = [
    "configure_file_logging",
    "configure_session_logging",
    "log_event",
    "logger",
    "new_session_id",
    "session_logger",
]
