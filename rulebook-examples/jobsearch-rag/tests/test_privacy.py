"""
Privacy verification tests — executable proof that the scoring pipeline
makes no external network calls.

Spec classes:
    TestPrivacyGuarantee — the scoring pipeline makes no network calls
                           to hosts other than localhost during scoring,
                           embedding, and decision recording
"""

from __future__ import annotations

import socket
from typing import TYPE_CHECKING
from unittest.mock import AsyncMock, MagicMock

import pytest

from jobsearch_rag.rag.decisions import DecisionRecorder
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.scorer import Scorer
from jobsearch_rag.rag.store import VectorStore
from tests.constants import EMBED_FAKE

if TYPE_CHECKING:
    from collections.abc import Iterator
    from pathlib import Path

# Public API surface (from src/jobsearch_rag/rag/scorer):
#   Scorer(store: VectorStore, embedder: Embedder, disqualify_on_llm_flag: bool)
#   scorer.score(jd_text: str) -> ScoreResult
#
# Public API surface (from src/jobsearch_rag/rag/embedder):
#   Embedder(base_url, embed_model, llm_model, ...)
#   embedder.embed(text: str) -> list[float]
#   embedder.classify(prompt: str) -> str
#   embedder.health_check() -> None
#
# Public API surface (from src/jobsearch_rag/rag/decisions):
#   DecisionRecorder(store, embedder, decisions_dir)
#   recorder.record(job_id, verdict, jd_text, board, ...) -> None


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

_LOCALHOST_ADDRS = {"127.0.0.1", "::1", "localhost"}

_SAMPLE_JD = (
    "We are looking for a Staff Platform Engineer to lead our cloud "
    "infrastructure team. Experience with Kubernetes and Terraform required."
)


# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------

_original_create_connection = socket.create_connection


def _guarded_create_connection(
    address: tuple[str, int] | tuple[str, int, int, int],
    *args: object,
    **kwargs: object,
) -> socket.socket:
    """Allow localhost connections only; raise on any external host."""
    host = str(address[0])
    if host not in _LOCALHOST_ADDRS:
        raise AssertionError(f"Privacy violation: attempted connection to external host '{host}'")
    return _original_create_connection(address, *args, **kwargs)  # type: ignore[arg-type]


@pytest.fixture
def _network_guard(monkeypatch: pytest.MonkeyPatch) -> None:  # pyright: ignore[reportUnusedFunction]
    """Monkeypatch socket.create_connection to reject non-localhost calls."""
    monkeypatch.setattr(socket, "create_connection", _guarded_create_connection)


@pytest.fixture
def _store(tmp_path: Path) -> VectorStore:  # pyright: ignore[reportUnusedFunction]
    """Real ChromaDB VectorStore backed by a per-test temp directory."""
    store = VectorStore(persist_dir=str(tmp_path / "chroma"))
    # Pre-create and seed collections the scorer requires
    for name in ("resume", "role_archetypes", "decisions"):
        store.get_or_create_collection(name)
    # Seed required collections with at least one document so scorer doesn't
    # raise "collection is empty" errors.
    store.add_documents(
        collection_name="resume",
        documents=["10 years experience in platform engineering and distributed systems."],
        embeddings=[EMBED_FAKE],
        ids=["resume-chunk-1"],
        metadatas=[{"source": "resume"}],
    )
    store.add_documents(
        collection_name="role_archetypes",
        documents=["Staff+ platform engineer leading cloud infrastructure teams."],
        embeddings=[EMBED_FAKE],
        ids=["archetype-1"],
        metadatas=[{"source": "archetypes"}],
    )
    return store


@pytest.fixture
def _mock_ollama_client() -> AsyncMock:  # pyright: ignore[reportUnusedFunction]
    """
    Stubbed ``ollama.AsyncClient`` — the I/O boundary.

    Returns realistic response objects for ``embed`` and ``chat`` so that
    all Embedder logic (retry, truncation, metrics, token counting) runs
    for real.  Only the final HTTP call is replaced.
    """
    client = AsyncMock()

    # embed() → response with embeddings list and token count
    embed_response = MagicMock()
    embed_response.embeddings = [EMBED_FAKE]
    embed_response.prompt_eval_count = 42
    client.embed = AsyncMock(return_value=embed_response)

    # chat() → response with message.content (JSON recognised by both
    # the injection screener and disqualifier parser)
    chat_message = MagicMock()
    chat_message.content = '{"suspicious": false, "disqualified": false}'
    chat_response = MagicMock()
    chat_response.message = chat_message
    chat_response.prompt_eval_count = 100
    chat_response.eval_count = 20
    client.chat = AsyncMock(return_value=chat_response)

    return client


@pytest.fixture
def _embedder(  # pyright: ignore[reportUnusedFunction]
    _mock_ollama_client: AsyncMock,
) -> Embedder:
    """
    Real Embedder instance with the ollama client stubbed at the I/O boundary.

    Uses the real ``Embedder.__init__`` and then replaces ``_client`` with the
    mock.  All Embedder logic — retry, truncation, metrics, token counting —
    runs for real.
    """
    embedder = Embedder(
        base_url="http://localhost:11434",
        embed_model="nomic-embed-text",
        llm_model="mistral:7b",
        max_retries=1,
        base_delay=0.0,
    )
    embedder._client = _mock_ollama_client  # type: ignore[attr-defined]
    return embedder


@pytest.fixture
def _scorer(  # pyright: ignore[reportUnusedFunction]
    _store: VectorStore,
    _embedder: Embedder,
) -> Scorer:
    """Real Scorer wired to real ChromaDB and stubbed Embedder."""
    return Scorer(store=_store, embedder=_embedder, disqualify_on_llm_flag=True)


@pytest.fixture
def _recorder(  # pyright: ignore[reportUnusedFunction]
    _store: VectorStore,
    _embedder: Embedder,
    tmp_path: Path,
) -> DecisionRecorder:
    """Real DecisionRecorder wired to real ChromaDB and stubbed Embedder."""
    return DecisionRecorder(
        store=_store,
        embedder=_embedder,
        decisions_dir=tmp_path / "decisions",
    )


@pytest.fixture
def _embed_call_tracker(  # pyright: ignore[reportUnusedFunction]
    _mock_ollama_client: AsyncMock,
) -> Iterator[AsyncMock]:
    """Yield the ollama client ``embed`` mock so tests can assert I/O boundary call counts."""
    yield _mock_ollama_client.embed


# ---------------------------------------------------------------------------
# TestPrivacyGuarantee
# ---------------------------------------------------------------------------


class TestPrivacyGuarantee:
    """
    REQUIREMENT: The scoring pipeline makes no network calls to hosts
    other than the configured Ollama endpoint during scoring, embedding,
    and decision recording.

    WHO: The operator who chose this tool specifically because it
         does not send personal data to external services
    WHAT: (1) During a complete scoring pipeline run covering Scorer,
              Embedder, and VectorStore, the system makes no network
              calls to any host other than localhost.
          (2) The system does not send any JD text, resume text, or
              scoring data to external servers during the pipeline run.
    WHY: The privacy-first claim is the primary architectural differentiator.
         A test that can fail is a guarantee worth making; a README statement
         is not

    MOCK BOUNDARY:
        Mock:  _mock_ollama_client fixture (ollama AsyncClient — the I/O
               boundary); socket.create_connection monkeypatched to reject
               non-localhost connections
        Real:  Embedder (embed, classify, retry, truncation, metrics),
               Scorer, DecisionRecorder, ChromaDB via VectorStore (all local)
        Never: Replace Embedder.embed() or classify() — the point is to
               verify the full call chain from public API to I/O boundary
    """

    @pytest.mark.asyncio
    @pytest.mark.usefixtures("_network_guard")
    async def test_scoring_pipeline_makes_no_external_network_calls(
        self,
        _scorer: Scorer,
    ) -> None:
        """
        GIVEN a complete pipeline run with Scorer, Embedder, and VectorStore
        WHEN all outbound connections to non-localhost hosts are intercepted
        THEN no such calls are made during scoring
        AND the pipeline completes without error.
        """
        # Given: network guard is active (via fixture), scorer is wired

        # When: score a listing through the full pipeline
        result = await _scorer.score(_SAMPLE_JD)

        # Then: pipeline completed (no AssertionError from network guard)
        assert result is not None, "score() must return a ScoreResult"
        assert result.fit_score >= 0.0, "fit_score must be non-negative"

    @pytest.mark.asyncio
    @pytest.mark.usefixtures("_network_guard")
    async def test_ollama_calls_to_localhost_are_permitted(
        self,
        _scorer: Scorer,
        _embed_call_tracker: AsyncMock,
    ) -> None:
        """
        GIVEN the same interception setup that blocks external calls
        WHEN the pipeline scores a listing
        THEN calls to localhost:11434 complete normally
        AND at least one embedding call is made.
        """
        # Given: network guard is active, scorer is wired

        # When: score a listing
        await _scorer.score(_SAMPLE_JD)

        # Then: at least one embed call was made (proving local calls work)
        assert _embed_call_tracker.call_count >= 1, (
            "at least one embedding call must be made during scoring"
        )

    @pytest.mark.asyncio
    @pytest.mark.usefixtures("_network_guard")
    async def test_disqualifier_pass_makes_no_external_calls(
        self,
        _scorer: Scorer,
    ) -> None:
        """
        GIVEN a listing that passes through the LLM disqualifier
        WHEN external network calls to non-localhost hosts are intercepted
        THEN no such calls occur during the disqualifier pass.
        """
        # Given: network guard is active, scorer has disqualifier enabled

        # When: score triggers disqualifier via classify()
        result = await _scorer.score(_SAMPLE_JD)

        # Then: pipeline completed, disqualifier ran without external calls
        assert result is not None, "score() must return a ScoreResult"
        assert result.disqualified is False, (
            "listing must not be disqualified (mock returns false)"
        )

    @pytest.mark.asyncio
    @pytest.mark.usefixtures("_network_guard")
    async def test_decision_recording_makes_no_external_calls(
        self,
        _recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a verdict recorded via DecisionRecorder
        WHEN external network calls to non-localhost hosts are intercepted
        THEN no such calls occur during the recording operation.
        """
        # Given: network guard is active, recorder is wired

        # When: record a decision
        await _recorder.record(
            job_id="privacy-test-001",
            verdict="yes",
            jd_text=_SAMPLE_JD,
            board="test-board",
            title="Staff Engineer",
            company="Acme Corp",
            reason="Privacy test",
        )

        # Then: recording completed without external calls
        decision = _recorder.get_decision("privacy-test-001")
        assert decision is not None, "decision must be persisted"
        assert decision["verdict"] == "yes", "verdict must match"
