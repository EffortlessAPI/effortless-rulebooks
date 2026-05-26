"""
Global test configuration — shared fixtures and safety guards.

This conftest provides:

1. **Output guard** — makes the real ``output/`` directory read-only so
   tests that forget to use ``tmp_path`` get an immediate ``PermissionError``.

2. **Shared I/O-boundary fixtures** — ``mock_embedder`` (real Embedder with
   ollama client stubbed at the I/O boundary), ``mock_ollama_client`` (the
   raw mock), ``vector_store`` (real ChromaDB backed by ``tmp_path``), and
   ``decision_recorder`` (real recorder wired to the above).  Individual
   test files may shadow these with local fixtures that use different
   return values.
"""

from __future__ import annotations

import contextlib
import stat
from pathlib import Path
from typing import TYPE_CHECKING
from unittest.mock import AsyncMock, MagicMock

import pytest

from jobsearch_rag.rag.decisions import DecisionRecorder
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.store import VectorStore
from tests.constants import EMBED_FAKE as EMBED_FAKE  # re-export for fixtures below

if TYPE_CHECKING:
    from collections.abc import Iterator

_PROJECT_OUTPUT = Path(__file__).resolve().parent.parent / "output"


# ---------------------------------------------------------------------------
# Shared helpers
# ---------------------------------------------------------------------------


def make_mock_ollama_client(
    embed_vector: list[float] | None = None,
    classify_response: str = '{"disqualified": false}',
) -> AsyncMock:
    """
    Build a stubbed ``ollama.AsyncClient`` — the I/O boundary.

    Returns realistic response objects for ``embed`` and ``chat`` so that
    all Embedder logic (retry, truncation, metrics, token counting) runs
    for real.  Only the final HTTP call is replaced.

    The ``embed_vector`` defaults to ``EMBED_FAKE`` if not provided.
    """
    if embed_vector is None:
        embed_vector = list(EMBED_FAKE)  # copy to avoid mutation

    client = AsyncMock()

    # embed() → response with embeddings list and token count
    embed_response = MagicMock()
    embed_response.embeddings = [embed_vector]
    embed_response.prompt_eval_count = 42
    client.embed = AsyncMock(return_value=embed_response)

    # chat() → response with message.content
    chat_message = MagicMock()
    chat_message.content = classify_response
    chat_response = MagicMock()
    chat_response.message = chat_message
    chat_response.prompt_eval_count = 100
    chat_response.eval_count = 20
    client.chat = AsyncMock(return_value=chat_response)

    # list() → response with available models (for health_check)
    model_embed = MagicMock()
    model_embed.model = "nomic-embed-text"
    model_llm = MagicMock()
    model_llm.model = "mistral:7b"
    client.list = AsyncMock(return_value=MagicMock(models=[model_embed, model_llm]))

    return client


# ---------------------------------------------------------------------------
# Shared I/O-boundary fixtures
# ---------------------------------------------------------------------------


@pytest.fixture
def mock_ollama_client() -> AsyncMock:  # pyright: ignore[reportUnusedFunction]
    """Stubbed ollama.AsyncClient — the I/O boundary."""
    return make_mock_ollama_client()


@pytest.fixture
def mock_embedder(mock_ollama_client: AsyncMock) -> Embedder:
    """
    Real Embedder with ollama client stubbed at the I/O boundary.

    All Embedder logic — retry, truncation, metrics, token counting —
    runs for real.  Only the ollama HTTP call is replaced.
    """
    embedder = Embedder(
        base_url="http://localhost:11434",
        embed_model="nomic-embed-text",
        llm_model="mistral:7b",
        max_retries=1,
        base_delay=0.0,
    )
    embedder._client = mock_ollama_client  # type: ignore[attr-defined]
    return embedder


@pytest.fixture
def vector_store(tmp_path: Path) -> VectorStore:
    """Real ChromaDB VectorStore backed by a per-test temp directory."""
    return VectorStore(persist_dir=str(tmp_path / "chroma"))


@pytest.fixture
def decision_recorder(
    vector_store: VectorStore,
    mock_embedder: Embedder,
    tmp_path: Path,
) -> DecisionRecorder:
    """
    Real DecisionRecorder with real ChromaDB and stubbed Embedder.

    The ``decisions`` collection is pre-created so ``get_decision``
    works even before the first ``record()`` call.
    """
    vector_store.get_or_create_collection("decisions")
    return DecisionRecorder(
        store=vector_store,
        embedder=mock_embedder,
        decisions_dir=tmp_path / "decisions",
    )


# ---------------------------------------------------------------------------
# Output safety guard
# ---------------------------------------------------------------------------


@pytest.fixture(autouse=True, scope="session")
def _guard_real_output_dir() -> Iterator[None]:  # pyright: ignore[reportUnusedFunction]  # autouse fixture
    """
    Make the real output/ directory read-only during tests.

    Restores original permissions after the session, even on failure.
    If the directory does not exist the guard is silently skipped —
    CI environments may not have it.
    """
    if not _PROJECT_OUTPUT.is_dir():
        yield
        return

    # Save original permissions for output/ and key subdirs
    dirs_to_guard = [_PROJECT_OUTPUT]
    for child in _PROJECT_OUTPUT.iterdir():
        if child.is_dir():
            dirs_to_guard.append(child)

    original_modes: dict[Path, int] = {}
    for d in dirs_to_guard:
        original_modes[d] = d.stat().st_mode
        # Remove write permission (owner, group, other)
        d.chmod(original_modes[d] & ~(stat.S_IWUSR | stat.S_IWGRP | stat.S_IWOTH))

    try:
        yield
    finally:
        # Restore original permissions
        for d, mode in original_modes.items():
            with contextlib.suppress(OSError):
                d.chmod(mode)
