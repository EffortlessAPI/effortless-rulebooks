"""
Pipeline runner — orchestrates adapters → RAG → rank.

The PipelineRunner is the top-level orchestrator that ties the entire
system together:

1. Load and validate settings
2. Health-check Ollama (fail fast before browser work)
3. For each enabled board: authenticate → search → extract details
4. Score each listing through the RAG pipeline
5. Rank, deduplicate, and filter
6. Export results

The runner owns the control flow but delegates all domain logic to
specialized components (adapters, scorer, ranker, exporters).
"""

from __future__ import annotations

import asyncio
import logging
import statistics
from dataclasses import dataclass, field
from pathlib import Path
from typing import TYPE_CHECKING

from jobsearch_rag.adapters import AdapterRegistry
from jobsearch_rag.adapters.session import SessionConfig, SessionManager, throttle
from jobsearch_rag.errors import ActionableError
from jobsearch_rag.logging import configure_session_logging, log_event, session_logger
from jobsearch_rag.pipeline.ranker import RankedListing, Ranker, RankSummary
from jobsearch_rag.rag.comp_parser import compute_comp_score, parse_compensation
from jobsearch_rag.rag.decisions import DecisionRecorder
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.indexer import Indexer
from jobsearch_rag.rag.scorer import Scorer
from jobsearch_rag.rag.store import VectorStore

if TYPE_CHECKING:
    from jobsearch_rag.adapters.base import JobListing
    from jobsearch_rag.config import Settings
    from jobsearch_rag.rag.scorer import ScoreResult

logger = logging.getLogger(__name__)


@dataclass
class RunResult:
    """Results from a pipeline run, consumed by exporters and CLI."""

    ranked_listings: list[RankedListing] = field(default_factory=lambda: [])
    summary: RankSummary = field(default_factory=RankSummary)
    failed_listings: int = 0
    skipped_decisions: int = 0
    boards_searched: list[str] = field(default_factory=list[str])
    errors: list[ActionableError] = field(default_factory=list[ActionableError])


class PipelineRunner:
    """
    Top-level pipeline orchestrator.

    Loads adapters, runs browser sessions, feeds results through
    the RAG scorer, and hands off to the ranker.
    """

    def __init__(self, settings: Settings) -> None:
        """Initialize pipeline components from application settings."""
        self._settings = settings
        self._embedder = Embedder(
            base_url=settings.ollama.base_url,
            embed_model=settings.ollama.embed_model,
            llm_model=settings.ollama.llm_model,
            slow_llm_threshold_ms=settings.ollama.slow_llm_threshold_ms,
        )
        self._store = VectorStore(persist_dir=settings.chroma.persist_dir)
        self._scorer = Scorer(
            store=self._store,
            embedder=self._embedder,
            disqualify_on_llm_flag=settings.scoring.disqualify_on_llm_flag,
        )
        self._ranker = Ranker(
            archetype_weight=settings.scoring.archetype_weight,
            fit_weight=settings.scoring.fit_weight,
            history_weight=settings.scoring.history_weight,
            comp_weight=settings.scoring.comp_weight,
            negative_weight=settings.scoring.negative_weight,
            culture_weight=settings.scoring.culture_weight,
            min_score_threshold=settings.scoring.min_score_threshold,
        )
        self._base_salary = settings.scoring.base_salary
        self._decision_recorder = DecisionRecorder(store=self._store, embedder=self._embedder)

    async def run(
        self,
        boards: list[str] | None = None,
        *,
        overnight: bool = False,
        force_rescore: bool = False,
    ) -> RunResult:
        """
        Execute a full search-score-rank pipeline.

        Args:
            boards: Specific board names to search.  ``None`` = all enabled.
            overnight: If ``True``, enforce extended throttling for stealth boards.
            force_rescore: If ``True``, re-score existing JDs instead of searching.

        Returns:
            A :class:`RunResult` with ranked listings and summary statistics.

        """
        # Step 0: Set up structured session logging
        log_dir = str(Path(self._settings.chroma.persist_dir).parent / "logs")
        session_handler, session_id = configure_session_logging(log_dir)

        try:
            return await self._run_inner(
                boards=boards,
                overnight=overnight,
                force_rescore=force_rescore,
                session_id=session_id,
            )
        finally:
            session_logger.removeHandler(session_handler)
            session_handler.close()

    async def _run_inner(
        self,
        boards: list[str] | None = None,
        *,
        overnight: bool = False,
        force_rescore: bool = False,
        session_id: str = "",
    ) -> RunResult:
        """Inner run logic, called after session logging is configured."""
        # Step 1: Health check Ollama before any browser work
        await self._embedder.health_check()
        logger.info("Ollama health check passed")

        # Step 1b: Auto-index if collections are empty (first run or post-reset)
        await self._ensure_indexed()

        # Step 2: Determine which boards to search
        board_names = boards or list(self._settings.enabled_boards)
        if overnight:
            # In overnight mode, also include overnight-only boards
            for b in self._settings.overnight_boards:
                if b not in board_names:
                    board_names.append(b)

        logger.info("Searching boards: %s", ", ".join(board_names))

        # Step 3: Collect listings from all boards (concurrently)
        all_listings: list[JobListing] = []
        failed_count = 0
        surfaced_errors: list[ActionableError] = []

        async def _search_one(board_name: str) -> tuple[str, list[JobListing], int]:
            """
            Search a single board, returning (name, listings, failures).

            Board-level errors are caught here so one failure
            doesn't cancel the other concurrent searches.
            """
            try:
                board_listings, board_failures, board_errors = await self._search_board(
                    board_name, overnight=overnight
                )
                surfaced_errors.extend(board_errors)
                logger.info(
                    "Board '%s': %d listings collected, %d failures",
                    board_name,
                    len(board_listings),
                    board_failures,
                )
                return board_name, board_listings, board_failures
            except ActionableError as exc:
                logger.error("Board '%s' failed entirely: %s", board_name, exc.error)
                surfaced_errors.append(exc)
                return board_name, [], 0

        results = await asyncio.gather(*[_search_one(b) for b in board_names])

        for _name, board_listings, board_failures in results:
            all_listings.extend(board_listings)
            failed_count += board_failures

        if not all_listings:
            logger.warning("No listings collected from any board")
            m = self._embedder.metrics
            log_event(
                "session_summary",
                jobs_found=0,
                jobs_scored=0,
                jobs_excluded=0,
                jobs_deduplicated=0,
                failed_listings=failed_count,
                skipped_decisions=0,
                boards_searched=board_names,
                embed_calls=m.embed_calls,
                embed_tokens_total=m.embed_tokens_total,
                llm_calls=m.llm_calls,
                llm_tokens_total=m.llm_tokens_total,
                llm_latency_ms_total=m.llm_latency_ms_total,
                slow_llm_calls=m.slow_llm_calls,
            )
            return RunResult(
                boards_searched=board_names,
                failed_listings=failed_count,
                errors=surfaced_errors,
            )

        # Step 4: Score each listing (skip already-decided unless forced)
        scored: list[tuple[JobListing, ScoreResult]] = []
        embeddings: dict[str, list[float]] = {}
        skipped_decisions = 0

        for listing in all_listings:
            # Cross-run dedup: skip listings with existing decisions
            if not force_rescore:
                decision = self._decision_recorder.get_decision(listing.external_id)
                if decision is not None:
                    logger.info(
                        "Skipping %s (%s) — already decided: %s",
                        listing.title,
                        listing.external_id,
                        decision.get("verdict", "unknown"),
                    )
                    skipped_decisions += 1
                    continue

            try:
                score_result = await self._scorer.score(listing.full_text)

                # Parse compensation from JD text and compute comp_score
                comp = parse_compensation(listing.full_text)
                if comp is not None:
                    listing.comp_min = comp.comp_min
                    listing.comp_max = comp.comp_max
                    listing.comp_source = comp.comp_source
                    listing.comp_text = comp.comp_text
                score_result.comp_score = compute_comp_score(listing.comp_max, self._base_salary)

                scored.append((listing, score_result))

                # Emit structured score_computed event
                final = self._ranker.compute_final_score(score_result)
                log_event(
                    "score_computed",
                    job_id=listing.external_id,
                    archetype=score_result.archetype_score,
                    fit=score_result.fit_score,
                    culture=score_result.culture_score,
                    history=score_result.history_score,
                    negative=score_result.negative_score,
                    comp=score_result.comp_score,
                    final=final,
                )

                # Cache the embedding for deduplication
                embedding = await self._embedder.embed(listing.full_text)
                embeddings[listing.url] = embedding
            except ActionableError as exc:
                logger.warning(
                    "Scoring failed for %s (%s): %s",
                    listing.title,
                    listing.url,
                    exc.error,
                )
                surfaced_errors.append(exc)
                failed_count += 1
            except Exception as exc:
                logger.error(
                    "Unexpected scoring error for %s (%s): %s",
                    listing.title,
                    listing.url,
                    exc,
                )
                surfaced_errors.append(ActionableError.from_exception(exc, "scorer", listing.url))
                failed_count += 1

        # Step 4b: Emit per-collection retrieval quality metrics
        threshold = self._settings.scoring.min_score_threshold
        for coll_name, scores in self._scorer.collection_scores.items():
            if not scores:
                continue
            sorted_scores = sorted(scores)
            n = len(sorted_scores)
            if n >= 2:
                quantiles = statistics.quantiles(sorted_scores, n=100)
                p50 = quantiles[49]
                p90 = quantiles[89]
            else:
                p50 = sorted_scores[0]
                p90 = sorted_scores[0]
            below = sum(1 for s in sorted_scores if s < threshold) if threshold else 0
            log_event(
                "retrieval_summary",
                collection=coll_name,
                n_scored=n,
                score_min=sorted_scores[0],
                score_p50=p50,
                score_p90=p90,
                score_max=sorted_scores[-1],
                below_threshold=below,
            )

        # Step 5: Rank, deduplicate, filter
        ranked, summary = self._ranker.rank(scored, embeddings)

        # Emit structured session_summary event
        m = self._embedder.metrics
        log_event(
            "session_summary",
            jobs_found=len(all_listings),
            jobs_scored=len(scored),
            jobs_excluded=summary.total_excluded,
            jobs_deduplicated=summary.total_deduplicated,
            failed_listings=failed_count,
            skipped_decisions=skipped_decisions,
            boards_searched=board_names,
            embed_calls=m.embed_calls,
            embed_tokens_total=m.embed_tokens_total,
            llm_calls=m.llm_calls,
            llm_tokens_total=m.llm_tokens_total,
            llm_latency_ms_total=m.llm_latency_ms_total,
            slow_llm_calls=m.slow_llm_calls,
        )

        return RunResult(
            ranked_listings=ranked,
            summary=summary,
            failed_listings=failed_count,
            skipped_decisions=skipped_decisions,
            boards_searched=board_names,
            errors=surfaced_errors,
        )

    async def _ensure_indexed(self) -> None:
        """
        Auto-index resume, archetypes, and positive signals if collections are empty.

        After a ``reset`` or on first run, the ``resume``,
        ``role_archetypes``, and ``global_positive_signals`` collections
        will be empty.  Rather than failing every scoring call, detect
        this and run the indexer automatically — it's the only sensible
        recovery.
        """
        needs_resume = self._collection_empty("resume")
        needs_archetypes = self._collection_empty("role_archetypes")
        needs_positive = self._collection_empty("global_positive_signals")

        if not needs_resume and not needs_archetypes and not needs_positive:
            return

        logger.info("Empty collections detected — auto-indexing before scoring")
        indexer = Indexer(store=self._store, embedder=self._embedder)

        if needs_archetypes:
            n = await indexer.index_archetypes(self._settings.archetypes_path)
            logger.info("Auto-indexed %d archetypes", n)

        if needs_resume:
            n = await indexer.index_resume(self._settings.resume_path)
            logger.info("Auto-indexed %d resume chunks", n)

        if needs_positive:
            n = await indexer.index_global_positive_signals(self._settings.global_rubric_path)
            logger.info("Auto-indexed %d global positive signal dimensions", n)

    def _collection_empty(self, name: str) -> bool:
        """Return True if the named collection is missing or has zero documents."""
        try:
            return self._store.collection_count(name) == 0
        except ActionableError:
            return True

    async def _search_board(
        self,
        board_name: str,
        *,
        overnight: bool = False,
    ) -> tuple[list[JobListing], int, list[ActionableError]]:
        """
        Search a single board and return (listings, failure_count, errors).

        Manages the browser session lifecycle for this board.
        """
        adapter = AdapterRegistry.get(board_name)
        board_cfg = self._settings.boards.get(board_name)

        if board_cfg is None:
            logger.warning("No config section for board '%s' — skipping", board_name)
            return [], 0, []

        is_overnight = overnight or board_name in self._settings.overnight_boards
        config = SessionConfig(
            board_name=board_name,
            headless=board_cfg.headless,
            stealth=board_name == "linkedin",
            overnight=is_overnight,
            browser_channel=board_cfg.browser_channel,
        )

        listings: list[JobListing] = []
        failed = 0
        caught_errors: list[ActionableError] = []

        async with SessionManager(config) as session:
            page = await session.new_page()

            # Authenticate
            await adapter.authenticate(page)
            await session.save_storage_state()

            # Search each configured URL
            for search_url in board_cfg.searches:
                await throttle(adapter)
                try:
                    results = await adapter.search(page, search_url, max_pages=board_cfg.max_pages)
                except ActionableError as exc:
                    logger.warning(
                        "Search failed for %s @ %s: %s",
                        board_name,
                        search_url,
                        exc.error,
                    )
                    continue

                # Extract details for each listing (skip if already enriched)
                for listing in results:
                    if listing.full_text.strip():
                        # Already enriched during search (e.g. click-through)
                        listings.append(listing)
                        continue

                    await throttle(adapter)
                    try:
                        enriched = await adapter.extract_detail(page, listing)
                        if enriched.full_text.strip():
                            listings.append(enriched)
                        else:
                            logger.warning(
                                "Empty JD text for %s — skipping",
                                listing.url,
                            )
                            failed += 1
                    except ActionableError as exc:
                        logger.warning(
                            "Detail extraction failed for %s: %s",
                            listing.url,
                            exc.error,
                        )
                        caught_errors.append(exc)
                        failed += 1
                    except Exception as exc:
                        logger.exception(
                            "Unexpected error extracting %s",
                            listing.url,
                        )
                        caught_errors.append(
                            ActionableError.from_exception(exc, board_name, listing.url)
                        )
                        failed += 1

        return listings, failed, caught_errors
