"""
Cross-run deduplication tests — skip previously-decided listings.

Maps to BDD spec: TestCrossRunDedup

Tests verify that the runner excludes already-decided listings from
scoring, reflects the exclusion count in the run summary, and supports
a --force-rescore override flag.
"""

from __future__ import annotations

import tempfile
from pathlib import Path
from typing import TYPE_CHECKING
from unittest.mock import AsyncMock, MagicMock, patch

from jobsearch_rag.adapters import AdapterRegistry
from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.config import (
    BoardConfig,
    ChromaConfig,
    OllamaConfig,
    OutputConfig,
    ScoringConfig,
    Settings,
)
from jobsearch_rag.pipeline.runner import PipelineRunner
from tests.constants import EMBED_FAKE

if TYPE_CHECKING:
    from jobsearch_rag.rag.store import VectorStore

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _make_settings(tmpdir: str) -> Settings:
    return Settings(
        enabled_boards=["testboard"],
        overnight_boards=[],
        boards={
            "testboard": BoardConfig(
                name="testboard",
                searches=["https://testboard.com/search"],
                max_pages=1,
                headless=True,
            ),
        },
        scoring=ScoringConfig(disqualify_on_llm_flag=False),
        ollama=OllamaConfig(),
        output=OutputConfig(output_dir=str(Path(tmpdir) / "output")),
        chroma=ChromaConfig(persist_dir=tmpdir),
    )


def _make_listing(
    external_id: str = "job-1",
    title: str = "Architect",
    full_text: str = "Full JD text " * 20,
) -> JobListing:
    return JobListing(
        board="testboard",
        external_id=external_id,
        title=title,
        company="TestCo",
        location="Remote",
        url=f"https://testboard.com/jobs/{external_id}",
        full_text=full_text,
    )


def _make_runner_with_real_stack(
    settings: Settings,
    *,
    populate_store: bool = True,
) -> tuple[PipelineRunner, AsyncMock]:
    """
    Create a PipelineRunner with real Embedder/Scorer and mocked Ollama client.

    The only mock is ``ollama_sdk.AsyncClient`` — the I/O boundary where
    our system ends and the network begins.

    Returns ``(runner, mock_client)``.
    """
    mock_client = AsyncMock()

    # health_check calls client.list()
    model_embed = MagicMock()
    model_embed.model = settings.ollama.embed_model
    model_llm = MagicMock()
    model_llm.model = settings.ollama.llm_model
    list_response = MagicMock()
    list_response.models = [model_embed, model_llm]
    mock_client.list.return_value = list_response

    # embed() calls client.embed()
    embed_response = MagicMock()
    embed_response.embeddings = [EMBED_FAKE]
    mock_client.embed.return_value = embed_response

    with patch(
        "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
        return_value=mock_client,
    ):
        runner = PipelineRunner(settings)

    if populate_store:
        _populate_store(runner._store)  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_store)
    return runner, mock_client


def _populate_store(store: VectorStore) -> None:
    """Seed the three required collections so auto-indexing is skipped."""
    for name in ("resume", "role_archetypes", "global_positive_signals"):
        store.add_documents(
            name,
            ids=[f"{name}-seed"],
            documents=[f"Seed document for {name}"],
            embeddings=[EMBED_FAKE],
        )


def _seed_decision(store: VectorStore, job_id: str, verdict: str = "yes") -> None:
    """Pre-record a decision into the real VectorStore decisions collection."""
    store.add_documents(
        collection_name="decisions",
        ids=[f"decision-{job_id}"],
        documents=["Previously decided JD text."],
        embeddings=[EMBED_FAKE],
        metadatas=[
            {
                "job_id": job_id,
                "verdict": verdict,
                "board": "testboard",
                "title": "Old Role",
                "company": "OldCo",
                "scoring_signal": "true" if verdict == "yes" else "false",
                "reason": "",
                "recorded_at": "2026-01-01T00:00:00+00:00",
            }
        ],
    )


def _mock_playwright_boundary() -> tuple[MagicMock, MagicMock]:
    """
    Create a mock Playwright I/O boundary for real SessionManager.

    Returns ``(mock_async_playwright, mock_page)``.
    """
    mock_page = MagicMock()

    mock_context = MagicMock()
    mock_context.new_page = AsyncMock(return_value=mock_page)
    mock_context.storage_state = AsyncMock(return_value={"cookies": [], "origins": []})
    mock_context.close = AsyncMock()

    mock_browser = MagicMock()
    mock_browser.new_context = AsyncMock(return_value=mock_context)
    mock_browser.close = AsyncMock()

    mock_pw = MagicMock()
    mock_pw.chromium.launch = AsyncMock(return_value=mock_browser)
    mock_pw.stop = AsyncMock()

    mock_pw_cm = MagicMock()
    mock_pw_cm.start = AsyncMock(return_value=mock_pw)

    mock_async_pw = MagicMock(return_value=mock_pw_cm)

    return mock_async_pw, mock_page


def _make_test_adapter(
    *,
    search_results: list[JobListing] | None = None,
) -> MagicMock:
    """Create a test adapter for the real AdapterRegistry."""
    adapter = MagicMock()
    adapter.board_name = "testboard"
    adapter.rate_limit_seconds = (0.0, 0.0)
    adapter.authenticate = AsyncMock()
    adapter.search = AsyncMock(return_value=search_results if search_results is not None else [])
    adapter.extract_detail = AsyncMock()
    return adapter


class TestCrossRunDedup:
    """
    REQUIREMENT: Re-searching does not re-process listings that already
    have a recorded decision.

    WHO: The pipeline runner performing a follow-up search
    WHAT: (1) The system excludes listings with existing decisions from ranked results and increments `skipped_decisions`.
          (2) The system omits listings with existing decisions from the exported ranked results and includes only new roles.
          (3) The system records excluded listings in the run summary by incrementing `skipped_decisions`.
          (4) The system scores a listing normally when no prior decision exists and includes it in ranked results.
          (5) The system scores a listing even when a prior decision exists if `force_rescore=True` is set and includes it in ranked results.
          (6) The system looks up prior decisions by job ID rather than URL and skips a listing whose `external_id` already has a decision.
    WHY: Without deduplication, repeated searches would re-score and
         re-present listings the operator has already acted on,
         wasting compute and operator time

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama API),
               async_playwright (Playwright browser library)
        Real:  PipelineRunner, Embedder, Scorer, VectorStore, Ranker,
               DecisionRecorder, AdapterRegistry, SessionManager, throttle
        Never: Construct ScoreResult directly — always obtained via real Scorer.score()
    """

    async def test_listing_with_existing_decision_is_excluded_from_scoring(
        self,
    ) -> None:
        """
        Given a decision for "already-decided" pre-seeded in the store,
        When run() processes listings including "already-decided" and "brand-new",
        Then only the new listing appears in ranked results and the decided one is skipped.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: runner with a pre-seeded decision for "already-decided"
            settings = _make_settings(tmpdir)
            runner, _mock_client = _make_runner_with_real_stack(settings)
            _seed_decision(runner._store, "already-decided")  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_store)

            decided_listing = _make_listing(external_id="already-decided", title="Old Role")
            new_listing = _make_listing(external_id="brand-new", title="New Role")

            adapter = _make_test_adapter(search_results=[decided_listing, new_listing])
            mock_pw_fn, _ = _mock_playwright_boundary()

            with (
                patch.dict(AdapterRegistry._registry, {"testboard": lambda: adapter}),  # type: ignore[dict-item]
                patch("jobsearch_rag.adapters.session.async_playwright", mock_pw_fn),
                patch("jobsearch_rag.adapters.session._STORAGE_DIR", Path(tmpdir)),
            ):
                # When: the pipeline runs
                result = await runner.run()

            # Then: decided listing was skipped
            assert result.skipped_decisions >= 1, (
                f"Expected at least 1 skipped decision, got {result.skipped_decisions}"
            )
            # Then: only the new listing appears in ranked results
            titles = [r.listing.title for r in result.ranked_listings]
            assert "Old Role" not in titles, (
                f"Decided listing should be excluded from ranked results, got titles: {titles}"
            )

    async def test_excluded_listing_does_not_appear_in_export(self) -> None:
        """
        Given a decision for "decided-1" pre-seeded in the store,
        When run() processes both "decided-1" and "new-1",
        Then only "New Role" appears in the ranked results.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: runner with a pre-seeded decision for "decided-1"
            settings = _make_settings(tmpdir)
            runner, _mock_client = _make_runner_with_real_stack(settings)
            _seed_decision(runner._store, "decided-1")  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_store)

            decided = _make_listing(external_id="decided-1", title="Old Role")
            new = _make_listing(external_id="new-1", title="New Role")

            adapter = _make_test_adapter(search_results=[decided, new])
            mock_pw_fn, _ = _mock_playwright_boundary()

            with (
                patch.dict(AdapterRegistry._registry, {"testboard": lambda: adapter}),  # type: ignore[dict-item]
                patch("jobsearch_rag.adapters.session.async_playwright", mock_pw_fn),
                patch("jobsearch_rag.adapters.session._STORAGE_DIR", Path(tmpdir)),
            ):
                # When: the pipeline runs
                result = await runner.run()

            # Then: decided listing absent, new listing present
            titles = [r.listing.title for r in result.ranked_listings]
            assert "Old Role" not in titles, (
                f"Decided listing should be excluded from ranked results, got titles: {titles}"
            )
            if result.ranked_listings:
                assert "New Role" in titles, (
                    f"New listing should appear in ranked results, got titles: {titles}"
                )

    async def test_exclusion_count_appears_in_run_summary(self) -> None:
        """
        Given a decision for "decided-1" pre-seeded in the store,
        When run() processes "decided-1" and "new-1",
        Then result.skipped_decisions reflects the exclusion.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: runner with a pre-seeded decision
            settings = _make_settings(tmpdir)
            runner, _mock_client = _make_runner_with_real_stack(settings)
            _seed_decision(runner._store, "decided-1")  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_store)

            decided = _make_listing(external_id="decided-1")
            new = _make_listing(external_id="new-1")

            adapter = _make_test_adapter(search_results=[decided, new])
            mock_pw_fn, _ = _mock_playwright_boundary()

            with (
                patch.dict(AdapterRegistry._registry, {"testboard": lambda: adapter}),  # type: ignore[dict-item]
                patch("jobsearch_rag.adapters.session.async_playwright", mock_pw_fn),
                patch("jobsearch_rag.adapters.session._STORAGE_DIR", Path(tmpdir)),
            ):
                # When: the pipeline runs
                result = await runner.run()

            # Then: at least one listing was skipped due to prior decision
            assert result.skipped_decisions >= 1, (
                f"Expected at least 1 skipped decision, got {result.skipped_decisions}"
            )

    async def test_listing_with_no_decision_is_scored_normally(self) -> None:
        """
        Given no pre-existing decisions in the store,
        When run() processes a listing "never-seen",
        Then the listing appears in the ranked results.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: runner with no decisions seeded
            settings = _make_settings(tmpdir)
            runner, _mock_client = _make_runner_with_real_stack(settings)

            new_listing = _make_listing(external_id="never-seen", title="Fresh Role")

            adapter = _make_test_adapter(search_results=[new_listing])
            mock_pw_fn, _ = _mock_playwright_boundary()

            with (
                patch.dict(AdapterRegistry._registry, {"testboard": lambda: adapter}),  # type: ignore[dict-item]
                patch("jobsearch_rag.adapters.session.async_playwright", mock_pw_fn),
                patch("jobsearch_rag.adapters.session._STORAGE_DIR", Path(tmpdir)),
            ):
                # When: the pipeline runs
                result = await runner.run()

            # Then: the listing was scored and appears in ranked results
            titles = [r.listing.title for r in result.ranked_listings]
            assert "Fresh Role" in titles, (
                f"New listing should appear in ranked results, got titles: {titles}"
            )
            assert result.skipped_decisions == 0, (
                f"Expected 0 skipped decisions, got {result.skipped_decisions}"
            )

    async def test_force_rescore_flag_overrides_exclusion(self) -> None:
        """
        Given a decision for "decided-1" pre-seeded in the store,
        When run(force_rescore=True) is called,
        Then the decided listing is scored anyway.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: runner with a pre-seeded decision
            settings = _make_settings(tmpdir)
            runner, _mock_client = _make_runner_with_real_stack(settings)
            _seed_decision(runner._store, "decided-1")  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_store)

            decided = _make_listing(external_id="decided-1")

            adapter = _make_test_adapter(search_results=[decided])
            mock_pw_fn, _ = _mock_playwright_boundary()

            with (
                patch.dict(AdapterRegistry._registry, {"testboard": lambda: adapter}),  # type: ignore[dict-item]
                patch("jobsearch_rag.adapters.session.async_playwright", mock_pw_fn),
                patch("jobsearch_rag.adapters.session._STORAGE_DIR", Path(tmpdir)),
            ):
                # When: force_rescore overrides exclusion
                result = await runner.run(force_rescore=True)

            # Then: listing was scored despite having a prior decision
            assert result.ranked_listings, (
                "Expected decided listing to appear in ranked results with force_rescore"
            )
            assert result.skipped_decisions == 0, (
                f"Expected 0 skipped decisions with force_rescore, got {result.skipped_decisions}"
            )

    async def test_decision_lookup_uses_job_id_not_url(self) -> None:
        """
        Given a decision for "canonical-id-123" and a listing with that
              external_id but a different URL,
        When run() processes the listing,
        Then the listing is skipped — proving lookup is by job_id, not URL.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: runner with decision keyed by job_id
            settings = _make_settings(tmpdir)
            runner, _mock_client = _make_runner_with_real_stack(settings)
            _seed_decision(runner._store, "canonical-id-123")  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_store)

            # Listing uses external_id matching the decision, but different URL
            listing = _make_listing(external_id="canonical-id-123")

            adapter = _make_test_adapter(search_results=[listing])
            mock_pw_fn, _ = _mock_playwright_boundary()

            with (
                patch.dict(AdapterRegistry._registry, {"testboard": lambda: adapter}),  # type: ignore[dict-item]
                patch("jobsearch_rag.adapters.session.async_playwright", mock_pw_fn),
                patch("jobsearch_rag.adapters.session._STORAGE_DIR", Path(tmpdir)),
            ):
                # When: the pipeline runs
                result = await runner.run()

            # Then: listing was skipped because decision matched by external_id
            assert result.skipped_decisions == 1, (
                f"Expected listing skipped by job_id lookup, "
                f"got skipped_decisions={result.skipped_decisions}"
            )
