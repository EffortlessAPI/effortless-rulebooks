"""
Observability tests — structured session tracing, per-call tracing,
inference metrics, and retrieval quality metrics.

Maps to BDD spec: TestSessionTracing, TestOllamaCallTracing,
                  TestInferenceMetrics, TestRetrievalMetrics
"""

from __future__ import annotations

import asyncio
import json
import tempfile
import time as _time
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
# Public API surface (from src/jobsearch_rag/logging):
#   new_session_id() -> str
#   configure_session_logging(log_dir: str, session_id: str, *, level: int) -> logging.FileHandler
#
# Public API surface (from src/jobsearch_rag/pipeline/runner):
#   PipelineRunner(settings: Settings) -> None
#   PipelineRunner.run(boards, *, overnight, force_rescore) -> RunResult
#   RunResult.ranked_listings, .summary, .failed_listings, .errors, .boards_searched
#
# Events emitted by structured logging:
#   "score_computed" — one per scored listing, includes component scores
#   "session_summary" — one per run, closes the session; includes
#       jobs_found, jobs_scored, jobs_excluded, jobs_deduplicated,
#       failed_listings, skipped_decisions, boards_searched,
#       embed_calls, llm_calls, embed_tokens_total, llm_tokens_total,
#       llm_latency_ms_total, slow_llm_calls
#   "embed_call" — one per Ollama embed() call, includes model, input_chars,
#       latency_ms, tokens (prompt_eval_count or estimate)
#   "classify_call" — one per classify() call, includes model, input_chars,
#       latency_ms, tokens (prompt_eval_count + eval_count or estimate)
#   "disqualifier_call" — one per disqualify() call, includes model, input_chars, outcome
#   "retrieval_summary" — one per collection per run; includes collection,
#       n_scored, score_min, score_p50, score_p90, score_max, below_threshold
# ---------------------------------------------------------------------------


# ---------------------------------------------------------------------------
# Helpers — same patterns as test_runner.py
# ---------------------------------------------------------------------------


def _make_settings(
    tmpdir: str,
    enabled_boards: list[str] | None = None,
    *,
    disqualify_on_llm_flag: bool = False,
    slow_llm_threshold_ms: int | None = None,
    min_score_threshold: float | None = None,
) -> Settings:
    """Create a Settings with temp ChromaDB dir and configurable boards."""
    boards = enabled_boards or ["testboard"]
    board_configs: dict[str, BoardConfig] = {}
    for name in boards:
        board_configs[name] = BoardConfig(
            name=name,
            searches=[f"https://{name}.com/search"],
            max_pages=1,
            headless=True,
        )
    ollama_kwargs: dict[str, object] = {}
    if slow_llm_threshold_ms is not None:
        ollama_kwargs["slow_llm_threshold_ms"] = slow_llm_threshold_ms
    scoring_kwargs: dict[str, object] = {"disqualify_on_llm_flag": disqualify_on_llm_flag}
    if min_score_threshold is not None:
        scoring_kwargs["min_score_threshold"] = min_score_threshold
    return Settings(
        enabled_boards=boards,
        overnight_boards=[],
        boards=board_configs,
        scoring=ScoringConfig(**scoring_kwargs),  # type: ignore[arg-type]
        ollama=OllamaConfig(**ollama_kwargs),  # type: ignore[arg-type]
        output=OutputConfig(output_dir=str(Path(tmpdir) / "output")),
        chroma=ChromaConfig(persist_dir=str(Path(tmpdir) / "chroma_db")),
    )


def _make_listing(
    board: str = "testboard",
    external_id: str = "1",
    title: str = "Staff Architect",
) -> JobListing:
    return JobListing(
        board=board,
        external_id=external_id,
        title=title,
        company="Acme Corp",
        location="Remote",
        url=f"https://{board}.com/{external_id}",
        full_text="A detailed job description for a staff architect role.",
    )


def _make_runner_with_real_stack(
    settings: Settings,
) -> tuple[PipelineRunner, AsyncMock]:
    """
    Create a PipelineRunner with real stack and mocked Ollama client.

    Only ``ollama_sdk.AsyncClient`` is mocked — the I/O boundary.
    """
    mock_client = AsyncMock()

    model_embed = MagicMock()
    model_embed.model = settings.ollama.embed_model
    model_llm = MagicMock()
    model_llm.model = settings.ollama.llm_model
    list_response = MagicMock()
    list_response.models = [model_embed, model_llm]
    mock_client.list.return_value = list_response

    embed_response = MagicMock()
    embed_response.embeddings = [EMBED_FAKE]
    mock_client.embed.return_value = embed_response

    with patch(
        "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
        return_value=mock_client,
    ):
        runner = PipelineRunner(settings)

    _populate_store(runner._store)  # pyright: ignore[reportPrivateUsage]

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


# An embedding vector distant from EMBED_FAKE — produces cosine distance > 0
# when JD text is embedded with EMBED_FAKE.  This creates non-trivial score
# distributions (< 1.0) so retrieval_summary tests exercise real metric computation.
_EMBED_DISTANT: list[float] = [0.9, 0.1, 0.9, 0.1, 0.9]

# The four collections that contribute to scoring — required and optional.
# negative_signals and decisions are optional; if empty, scorer returns 0.0.
_RETRIEVAL_COLLECTIONS = (
    "resume",
    "role_archetypes",
    "global_positive_signals",
    "negative_signals",
)


def _populate_store_with_distant_embeddings(store: VectorStore) -> None:
    """
    Seed collections with distant embeddings so score distributions are non-trivial.

    Uses _EMBED_DISTANT for stored documents while the mock Ollama returns
    EMBED_FAKE for JD queries — producing cosine distances > 0 and scores < 1.0.
    """
    for name in _RETRIEVAL_COLLECTIONS:
        store.add_documents(
            name,
            ids=[f"{name}-seed"],
            documents=[f"Seed document for {name}"],
            embeddings=[_EMBED_DISTANT],
        )


def _make_runner_with_distant_store(
    settings: Settings,
) -> tuple[PipelineRunner, AsyncMock]:
    """
    Like _make_runner_with_real_stack but seeds collections with distant embeddings.

    Produces non-trivial score distributions for retrieval_summary testing.
    """
    mock_client = AsyncMock()

    model_embed = MagicMock()
    model_embed.model = settings.ollama.embed_model
    model_llm = MagicMock()
    model_llm.model = settings.ollama.llm_model
    list_response = MagicMock()
    list_response.models = [model_embed, model_llm]
    mock_client.list.return_value = list_response

    embed_response = MagicMock()
    embed_response.embeddings = [EMBED_FAKE]
    mock_client.embed.return_value = embed_response

    with patch(
        "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
        return_value=mock_client,
    ):
        runner = PipelineRunner(settings)

    _populate_store_with_distant_embeddings(runner._store)  # pyright: ignore[reportPrivateUsage]

    return runner, mock_client


def _mock_playwright_boundary() -> tuple[MagicMock, MagicMock]:
    """Mock async_playwright — the Playwright I/O boundary."""
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
    board_name: str = "testboard",
    *,
    search_results: list[JobListing] | None = None,
) -> MagicMock:
    """Create a test adapter with configurable search results."""
    adapter = MagicMock()
    adapter.board_name = board_name
    adapter.rate_limit_seconds = (0.0, 0.0)
    adapter.authenticate = AsyncMock()
    adapter.search = AsyncMock(return_value=search_results if search_results is not None else [])
    adapter.extract_detail = AsyncMock()
    return adapter


def _run_pipeline_and_read_logs(
    tmpdir: str,
    listings: list[JobListing],
    mock_client: AsyncMock,
    runner: PipelineRunner,
    *,
    exclude_files: set[str] | None = None,
    disqualifier_response: str = '{"disqualified": false}',
    chat_latency_s: float = 0.0,
) -> list[dict[str, object]]:
    """
    Run the pipeline against the given listings and return parsed log entries.

    Sets up the classify mock to return a configurable disqualifier response,
    patches Playwright and adapter boundaries, runs the pipeline,
    then reads and parses the JSON-lines log file.

    *exclude_files* is a set of filenames from prior runs to skip.
    *disqualifier_response* is the raw JSON string the LLM mock returns.
    *chat_latency_s* adds a sleep to the chat mock to simulate inference time.
    """
    chat_response = MagicMock(
        message=MagicMock(content=disqualifier_response),
    )
    if chat_latency_s > 0:

        async def _slow_chat(**_kwargs: object) -> MagicMock:  # type: ignore[type-arg]
            _time.sleep(chat_latency_s)
            return chat_response

        mock_client.chat.side_effect = _slow_chat
    else:
        mock_client.chat.return_value = chat_response

    mock_async_pw, _mock_page = _mock_playwright_boundary()
    adapter = _make_test_adapter(search_results=listings)

    log_dir = Path(tmpdir) / "logs"

    with (
        patch("jobsearch_rag.adapters.session.async_playwright", mock_async_pw),
        patch.dict(
            AdapterRegistry._registry,  # pyright: ignore[reportPrivateUsage]
            {"testboard": type(adapter)},
        ),
        patch(
            "jobsearch_rag.adapters.AdapterRegistry.get",
            return_value=adapter,
        ),
        patch("jobsearch_rag.adapters.session.asyncio.sleep", new_callable=AsyncMock),
    ):
        asyncio.run(runner.run())

    # Parse JSON-lines log files (only new ones)
    skip = exclude_files or set()
    entries: list[dict[str, object]] = []
    for log_file in sorted(log_dir.glob("*.jsonl")):
        if log_file.name in skip:
            continue
        for line in log_file.read_text().splitlines():
            line = line.strip()
            if line:
                entries.append(json.loads(line))

    return entries


# ---------------------------------------------------------------------------
# TestSessionTracing
# ---------------------------------------------------------------------------


class TestSessionTracing:
    """
    REQUIREMENT: Every run produces a structured log file whose entries
    are correlated by a session ID so the operator can reconstruct exactly
    what happened in any given run.

    WHO: The operator diagnosing unexpected scores, missed listings, or
         slow inference after a run completes
    WHAT: (1) A session ID is generated at the start of each CLI invocation
              and appears in every log entry written during that run
          (2) log entries are written in JSON-lines format
          (3) the session log is written to data/logs/ alongside stderr output
          (4) a run that processes three listings produces at least three
              score_computed log entries all sharing the same session ID
          (5) a second run produces a different session ID from the first
          (6) the session ID appears in the session_summary entry that
              closes every run
          (7) a score_computed entry contains all six component scores
    WHY: Mixed log output from concurrent runs or re-runs is unreadable
         without a correlation key

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama HTTP); async_playwright
               (Playwright I/O); asyncio.sleep for throttle bypass
        Real:  PipelineRunner, logging infrastructure, log file in tmp_path
        Never: Mock the logger or inject session IDs directly; run the
               real pipeline and verify the session ID by parsing the
               actual log file written to tmp_path
    """

    def test_every_log_entry_in_a_run_shares_the_same_session_id(self) -> None:
        """
        Given a pipeline run that processes two listings
        When the run completes and the log file is read
        Then every JSON-lines entry contains a 'session' field
        And all 'session' values in that file are identical
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner and two listings
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Engineer A"),
                _make_listing(external_id="2", title="Engineer B"),
            ]

            # When: pipeline runs and log file is read
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: every entry has a 'session' field and all values match
            assert len(entries) > 0, "Expected at least one log entry"
            session_ids = {e["session"] for e in entries if "session" in e}
            assert len(session_ids) == 1, (
                f"Expected all entries to share one session ID, got {session_ids}"
            )
            for entry in entries:
                assert "session" in entry, f"Entry missing 'session' field: {entry}"

    def test_session_id_appears_in_session_summary_log_entry(self) -> None:
        """
        Given a completed pipeline run
        When the log file is read
        Then the entry whose 'event' is 'session_summary' contains
        the same session ID as all other entries in that file
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: session_summary entry exists with matching session ID
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, f"Expected 1 session_summary entry, got {len(summaries)}"
            all_sessions = {e["session"] for e in entries}
            assert len(all_sessions) == 1, f"Expected uniform session ID, got {all_sessions}"
            assert summaries[0]["session"] in all_sessions, (
                "session_summary session ID does not match other entries"
            )

    def test_two_consecutive_runs_produce_different_session_ids(self) -> None:
        """
        Given two sequential pipeline runs against the same input
        When both log files are read
        Then the session IDs in the two files are different
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing, run twice
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: two runs produce two sets of entries
            entries_1 = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)
            # Track files from run 1 so run 2 only reads its own
            log_dir = Path(tmpdir) / "logs"
            run1_files = {f.name for f in log_dir.glob("*.jsonl")}
            entries_2 = _run_pipeline_and_read_logs(
                tmpdir,
                listings,
                mock_client,
                runner,
                exclude_files=run1_files,
            )

            # Then: session IDs differ
            sessions_1 = {e["session"] for e in entries_1}
            sessions_2 = {e["session"] for e in entries_2}
            assert len(sessions_1) == 1, f"Run 1 had multiple session IDs: {sessions_1}"
            assert len(sessions_2) == 1, f"Run 2 had multiple session IDs: {sessions_2}"
            assert sessions_1 != sessions_2, (
                f"Two runs should have different session IDs, both got {sessions_1}"
            )

    def test_score_computed_entries_appear_once_per_scored_listing(self) -> None:
        """
        Given a pipeline run that scores three listings
        When the log file is read
        Then exactly three entries with event 'score_computed' are present
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with three listings
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
                _make_listing(external_id="3", title="Role C"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: exactly three score_computed entries
            score_entries = [e for e in entries if e.get("event") == "score_computed"]
            assert len(score_entries) == 3, (
                f"Expected 3 score_computed entries, got {len(score_entries)}"
            )

    def test_score_computed_entry_contains_all_six_component_scores(self) -> None:
        """
        Given a completed pipeline run
        When a 'score_computed' log entry is read
        Then it contains numeric fields for archetype, fit, culture,
        history, negative, and final scores
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: score_computed entry has all six numeric fields
            score_entries = [e for e in entries if e.get("event") == "score_computed"]
            assert len(score_entries) >= 1, "Expected at least one score_computed entry"
            entry = score_entries[0]
            expected_fields = ["archetype", "fit", "culture", "history", "negative", "final"]
            for field in expected_fields:
                assert field in entry, f"score_computed entry missing '{field}': {entry}"
                assert isinstance(entry[field], (int, float)), (
                    f"'{field}' should be numeric, got {type(entry[field])}: {entry[field]}"
                )

    def test_log_file_is_written_to_data_logs_directory(self) -> None:
        """
        Given a pipeline run using a configured data directory
        When the run completes
        Then a log file exists under data/logs/ within that directory
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with tmp data dir
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: log file exists under logs/
            log_dir = Path(tmpdir) / "logs"
            log_files = list(log_dir.glob("*.jsonl"))
            assert len(log_files) >= 1, (
                f"Expected at least one .jsonl log file in {log_dir}, found none"
            )

    def test_log_entries_are_valid_json_lines(self) -> None:
        """
        Given a completed pipeline run
        When each line of the log file is parsed as JSON
        Then every line parses without error
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: every line in every log file is valid JSON
            log_dir = Path(tmpdir) / "logs"
            log_files = list(log_dir.glob("*.jsonl"))
            assert len(log_files) >= 1, f"Expected at least one .jsonl log file in {log_dir}"
            for log_file in log_files:
                for i, line in enumerate(log_file.read_text().splitlines()):
                    line = line.strip()
                    if not line:
                        continue
                    try:
                        json.loads(line)
                    except json.JSONDecodeError as exc:
                        msg = f"Line {i + 1} in {log_file.name} is not valid JSON: {exc}"
                        raise AssertionError(msg) from exc


# ---------------------------------------------------------------------------
# TestOllamaCallTracing
# ---------------------------------------------------------------------------


class TestOllamaCallTracing:
    """
    REQUIREMENT: Every Ollama API call (embedding and LLM classification)
    is logged as a structured event with model, input size, and latency
    so the operator can diagnose slow inference and understand per-call
    costs.

    WHO: The operator diagnosing slow runs or unexpected scores after a
         pipeline completes
    WHAT: (1) each embed call during scoring produces an 'embed_call' log
              entry with 'model' and 'input_chars' fields
          (2) a disqualifier call produces a 'disqualifier_call' log entry
              with 'model', 'input_chars', and 'outcome' fields
          (3) the 'outcome' field is 'disqualified' or 'not_disqualified'
              based on the LLM response
          (4) all 'embed_call' and 'disqualifier_call' entries share the
              same session ID as 'score_computed' entries in the same run
          (5) a run scoring N listings with disqualification enabled
              produces at least N 'embed_call' entries and N
              'disqualifier_call' entries
    WHY: Without per-call logging, the operator cannot distinguish whether
         a slow run was caused by one expensive LLM call or many slow
         embedding calls — the session summary alone is not enough

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama HTTP); async_playwright
               (Playwright I/O); asyncio.sleep for throttle bypass
        Real:  PipelineRunner, Embedder, Scorer, logging infrastructure,
               log file in tmp_path
        Never: Mock the logger or log_event; run the real pipeline and
               verify events by parsing the actual log file
    """

    def test_embed_call_entry_has_model_and_input_chars(self) -> None:
        """
        Given a pipeline run that scores one listing
        When the log file is read
        Then at least one entry with event 'embed_call' is present
        And it contains 'model' matching the configured embed model
        And it contains 'input_chars' as a positive integer
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing and disqualification off
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: at least one embed_call entry with model and input_chars
            embed_calls = [e for e in entries if e.get("event") == "embed_call"]
            assert len(embed_calls) >= 1, (
                f"Expected at least one embed_call entry, got {len(embed_calls)}. "
                f"Events found: {[e.get('event') for e in entries]}"
            )
            entry = embed_calls[0]
            assert entry.get("model") == settings.ollama.embed_model, (
                f"Expected model '{settings.ollama.embed_model}', got '{entry.get('model')}'"
            )
            assert isinstance(entry.get("input_chars"), int), (
                f"Expected 'input_chars' as int, got {type(entry.get('input_chars'))}: "
                f"{entry.get('input_chars')}"
            )
            assert entry["input_chars"] > 0, (  # type: ignore[operator]
                f"Expected positive input_chars, got {entry['input_chars']}"
            )

    def test_disqualifier_call_entry_has_model_input_chars_and_outcome(self) -> None:
        """
        Given a pipeline run with disqualification enabled that scores one listing
        When the log file is read
        Then at least one entry with event 'disqualifier_call' is present
        And it contains 'model' matching the configured LLM model
        And it contains 'input_chars' as a positive integer
        And it contains 'outcome' as a string
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with disqualification enabled
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs with disqualification
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: at least one disqualifier_call with model, input_chars, outcome
            dq_calls = [e for e in entries if e.get("event") == "disqualifier_call"]
            assert len(dq_calls) >= 1, (
                f"Expected at least one disqualifier_call entry, got {len(dq_calls)}. "
                f"Events found: {[e.get('event') for e in entries]}"
            )
            entry = dq_calls[0]
            assert entry.get("model") == settings.ollama.llm_model, (
                f"Expected model '{settings.ollama.llm_model}', got '{entry.get('model')}'"
            )
            assert isinstance(entry.get("input_chars"), int), (
                f"Expected 'input_chars' as int, got {type(entry.get('input_chars'))}"
            )
            assert entry["input_chars"] > 0, (  # type: ignore[operator]
                f"Expected positive input_chars, got {entry['input_chars']}"
            )
            assert isinstance(entry.get("outcome"), str), (
                f"Expected 'outcome' as string, got {type(entry.get('outcome'))}"
            )

    def test_disqualifier_outcome_reflects_llm_response(self) -> None:
        """
        Given a pipeline run where the LLM returns disqualified=true
        When the log file is read
        Then the disqualifier_call entry's 'outcome' field is 'disqualified'
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with disqualification enabled and a disqualifying response
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs with a disqualified=true LLM response
            entries = _run_pipeline_and_read_logs(
                tmpdir,
                listings,
                mock_client,
                runner,
                disqualifier_response='{"disqualified": true, "reason": "staffing agency"}',
            )

            # Then: the disqualifier_call outcome is 'disqualified'
            dq_calls = [e for e in entries if e.get("event") == "disqualifier_call"]
            assert len(dq_calls) >= 1, (
                f"Expected at least one disqualifier_call entry, got {len(dq_calls)}"
            )
            assert dq_calls[0].get("outcome") == "disqualified", (
                f"Expected outcome 'disqualified', got '{dq_calls[0].get('outcome')}'"
            )

    def test_ollama_call_entries_share_session_id_with_score_computed(self) -> None:
        """
        Given a completed pipeline run
        When the log file is read
        Then embed_call, disqualifier_call, and score_computed entries
        all contain the same 'session' value
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with disqualification enabled
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: all event types are present and share the same session ID
            embed_sessions = {e["session"] for e in entries if e.get("event") == "embed_call"}
            dq_sessions = {e["session"] for e in entries if e.get("event") == "disqualifier_call"}
            score_sessions = {e["session"] for e in entries if e.get("event") == "score_computed"}
            assert embed_sessions, "Expected at least one embed_call entry, found none"
            assert dq_sessions, "Expected at least one disqualifier_call entry, found none"
            assert score_sessions, "Expected at least one score_computed entry, found none"
            all_sessions = embed_sessions | dq_sessions | score_sessions
            assert len(all_sessions) == 1, (
                f"Expected all Ollama call events to share one session ID, "
                f"got embed={embed_sessions}, dq={dq_sessions}, score={score_sessions}"
            )

    def test_n_listings_produce_at_least_n_embed_and_n_disqualifier_entries(self) -> None:
        """
        Given a pipeline run that scores three listings with disqualification enabled
        When the log file is read
        Then there are at least 3 embed_call entries
        And there are exactly 3 disqualifier_call entries
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: three listings with disqualification enabled
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
                _make_listing(external_id="3", title="Role C"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: at least 3 embed_call and exactly 3 disqualifier_call entries
            embed_calls = [e for e in entries if e.get("event") == "embed_call"]
            dq_calls = [e for e in entries if e.get("event") == "disqualifier_call"]
            assert len(embed_calls) >= 3, (
                f"Expected at least 3 embed_call entries (one per listing), got {len(embed_calls)}"
            )
            assert len(dq_calls) == 3, (
                f"Expected exactly 3 disqualifier_call entries (one per listing), "
                f"got {len(dq_calls)}"
            )


# ---------------------------------------------------------------------------
# TestInferenceMetrics
# ---------------------------------------------------------------------------


class TestInferenceMetrics:
    """
    REQUIREMENT: Each run produces a summary of inference activity so the
    operator can track efficiency and detect slow calls.

    WHO: The operator tuning prompt length and model selection;
         the operator monitoring inference time on constrained hardware
    WHAT: (1) a session_summary log entry is written at the end of every
              run and is the last entry in the log file
          (2) session_summary includes embed_calls as a count of embedding
              API calls made during the run
          (3) session_summary includes llm_calls as a count of disqualifier
              LLM calls made during the run
          (4) session_summary includes llm_latency_ms_total as the
              cumulative inference wall-clock time
          (5) session_summary includes embed_tokens_total as the cumulative
              embedding token count
          (6) session_summary includes llm_tokens_total as the cumulative
              LLM token count
          (7) slow_llm_calls counts LLM calls that exceeded
              slow_llm_threshold_ms
          (8) slow_llm_calls is zero when no calls exceed the threshold
          (9) the slow_llm_threshold_ms setting is respected — different
              values produce different slow_llm_calls counts
    WHY: Without token and latency tracking, the operator has no signal for
         when the disqualifier prompt has grown too large for comfortable
         local inference

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama HTTP); async_playwright
               (Playwright I/O); asyncio.sleep for throttle bypass
        Real:  PipelineRunner, logging infrastructure, log file in tmp_path
        Never: Mock the metrics collection; verify by parsing the real
               session_summary entry from the log file written to tmp_path
    """

    def test_session_summary_entry_is_written_at_end_of_run(self) -> None:
        """
        Given a completed pipeline run
        When the log file is read
        Then exactly one entry with event 'session_summary' is present
        And it is the last entry in the file
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: exactly one session_summary entry, and it is the last entry
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, (
                f"Expected exactly 1 session_summary entry, got {len(summaries)}"
            )
            assert entries[-1].get("event") == "session_summary", (
                f"Expected session_summary to be the last entry, "
                f"but last entry has event '{entries[-1].get('event')}'"
            )

    def test_session_summary_includes_embed_call_count(self) -> None:
        """
        Given a run that scores two listings
        When the session_summary log entry is read
        Then it contains an 'embed_calls' field with a value >= 2
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with two listings
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: session_summary contains embed_calls >= 2
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "embed_calls" in summary, (
                f"session_summary missing 'embed_calls' field: {summary}"
            )
            assert isinstance(summary["embed_calls"], int), (
                f"Expected 'embed_calls' as int, got {type(summary['embed_calls'])}"
            )
            assert summary["embed_calls"] >= 2, (  # type: ignore[operator]
                f"Expected embed_calls >= 2, got {summary['embed_calls']}"
            )

    def test_session_summary_includes_llm_call_count(self) -> None:
        """
        Given a run that scores two listings through the disqualifier
        When the session_summary log entry is read
        Then it contains an 'llm_calls' field equal to 4 (2 screening + 2 disqualifier)
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with two listings and disqualification enabled
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: session_summary contains llm_calls == 4
            # (2 listings x 2 classify calls each: injection screening + disqualifier)
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "llm_calls" in summary, f"session_summary missing 'llm_calls' field: {summary}"
            assert summary["llm_calls"] == 4, (
                f"Expected llm_calls == 4, got {summary['llm_calls']}"
            )

    def test_session_summary_includes_total_inference_latency(self) -> None:
        """
        Given a completed pipeline run
        When the session_summary log entry is read
        Then it contains an 'llm_latency_ms_total' field with a non-negative integer value
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with one listing and disqualification enabled
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: session_summary contains llm_latency_ms_total as non-negative int
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "llm_latency_ms_total" in summary, (
                f"session_summary missing 'llm_latency_ms_total' field: {summary}"
            )
            assert isinstance(summary["llm_latency_ms_total"], int), (
                f"Expected 'llm_latency_ms_total' as int, "
                f"got {type(summary['llm_latency_ms_total'])}"
            )
            assert summary["llm_latency_ms_total"] >= 0, (  # type: ignore[operator]
                f"Expected non-negative llm_latency_ms_total, "
                f"got {summary['llm_latency_ms_total']}"
            )

    def test_session_summary_includes_embed_tokens_total(self) -> None:
        """
        Given a run that scores two listings
        When the session_summary log entry is read
        Then it contains an 'embed_tokens_total' field with a positive integer value
        Note: value comes from Ollama prompt_eval_count when available,
              otherwise falls back to len(text) // 4 estimate
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with two listings
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: session_summary contains embed_tokens_total as positive int
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "embed_tokens_total" in summary, (
                f"session_summary missing 'embed_tokens_total' field: {summary}"
            )
            assert isinstance(summary["embed_tokens_total"], int), (
                f"Expected 'embed_tokens_total' as int, got {type(summary['embed_tokens_total'])}"
            )
            assert summary["embed_tokens_total"] > 0, (  # type: ignore[operator]
                f"Expected positive embed_tokens_total, got {summary['embed_tokens_total']}"
            )

    def test_session_summary_includes_llm_tokens_total(self) -> None:
        """
        Given a run that scores two listings through the disqualifier
        When the session_summary log entry is read
        Then it contains an 'llm_tokens_total' field with a positive integer value
        Note: value comes from Ollama prompt_eval_count + eval_count when
              available, otherwise falls back to len(text) // 4 estimate
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with two listings and disqualification enabled
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: session_summary contains llm_tokens_total as positive int
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "llm_tokens_total" in summary, (
                f"session_summary missing 'llm_tokens_total' field: {summary}"
            )
            assert isinstance(summary["llm_tokens_total"], int), (
                f"Expected 'llm_tokens_total' as int, got {type(summary['llm_tokens_total'])}"
            )
            assert summary["llm_tokens_total"] > 0, (  # type: ignore[operator]
                f"Expected positive llm_tokens_total, got {summary['llm_tokens_total']}"
            )

    def test_slow_llm_calls_counted_when_threshold_exceeded(self) -> None:
        """
        Given slow_llm_threshold_ms is set to 1
        And a run processes at least one listing through the disqualifier
        When the session_summary log entry is read
        Then 'slow_llm_calls' is greater than zero
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: threshold of 1ms with a chat mock that sleeps 5ms
            settings = _make_settings(tmpdir, disqualify_on_llm_flag=True, slow_llm_threshold_ms=1)
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs with measurable chat latency
            entries = _run_pipeline_and_read_logs(
                tmpdir, listings, mock_client, runner, chat_latency_s=0.005
            )

            # Then: slow_llm_calls > 0
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "slow_llm_calls" in summary, (
                f"session_summary missing 'slow_llm_calls' field: {summary}"
            )
            assert summary["slow_llm_calls"] > 0, (  # type: ignore[operator]
                f"Expected slow_llm_calls > 0 with threshold 1ms, got {summary['slow_llm_calls']}"
            )

    def test_slow_llm_calls_is_zero_when_no_calls_exceed_threshold(self) -> None:
        """
        Given slow_llm_threshold_ms is set to an unreachably high value
        And a run completes
        When the session_summary log entry is read
        Then 'slow_llm_calls' is zero
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: threshold so high no call can exceed it
            settings = _make_settings(
                tmpdir,
                disqualify_on_llm_flag=True,
                slow_llm_threshold_ms=999_999_999,
            )
            runner, mock_client = _make_runner_with_real_stack(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: slow_llm_calls == 0
            summaries = [e for e in entries if e.get("event") == "session_summary"]
            assert len(summaries) == 1, "Expected exactly 1 session_summary entry"
            summary = summaries[0]
            assert "slow_llm_calls" in summary, (
                f"session_summary missing 'slow_llm_calls' field: {summary}"
            )
            assert summary["slow_llm_calls"] == 0, (
                f"Expected slow_llm_calls == 0 with unreachable threshold, "
                f"got {summary['slow_llm_calls']}"
            )

    def test_slow_llm_threshold_is_configurable_in_settings(self) -> None:
        """
        Given two runs with different slow_llm_threshold_ms values
        When both session_summary entries are read
        Then the slow_llm_calls counts differ between the two runs
        for the same underlying inference time
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: run 1 with threshold=1 (everything is slow) and 5ms chat latency
            settings_low = _make_settings(
                tmpdir, disqualify_on_llm_flag=True, slow_llm_threshold_ms=1
            )
            runner_low, mock_client_low = _make_runner_with_real_stack(settings_low)
            listings = [_make_listing()]

            entries_low = _run_pipeline_and_read_logs(
                tmpdir, listings, mock_client_low, runner_low, chat_latency_s=0.005
            )
            summaries_low = [e for e in entries_low if e.get("event") == "session_summary"]
            assert len(summaries_low) == 1, "Expected 1 session_summary for low-threshold run"

        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: run 2 with threshold=999999999 (nothing is slow)
            settings_high = _make_settings(
                tmpdir, disqualify_on_llm_flag=True, slow_llm_threshold_ms=999_999_999
            )
            runner_high, mock_client_high = _make_runner_with_real_stack(settings_high)

            entries_high = _run_pipeline_and_read_logs(
                tmpdir, listings, mock_client_high, runner_high
            )
            summaries_high = [e for e in entries_high if e.get("event") == "session_summary"]
            assert len(summaries_high) == 1, "Expected 1 session_summary for high-threshold run"

        # Then: slow_llm_calls differs between the two runs
        slow_low = summaries_low[0].get("slow_llm_calls")
        slow_high = summaries_high[0].get("slow_llm_calls")
        assert slow_low != slow_high, (
            f"Expected different slow_llm_calls for threshold=1 vs threshold=999999999, "
            f"both got {slow_low}"
        )


# ---------------------------------------------------------------------------
# TestRetrievalMetrics
# ---------------------------------------------------------------------------


class TestRetrievalMetrics:
    """
    REQUIREMENT: After each scoring run, the distribution of scores per
    collection is logged so the operator can detect stale indexes and
    calibrate weights over time.

    WHO: The operator who re-indexed and wants to confirm the change had
         the expected effect; the operator tuning collection weights
    WHAT: (1) one retrieval_summary log entry is written per collection
              that contributed to scoring
          (2) each retrieval_summary names the collection via a
              'collection' field
          (3) each retrieval_summary includes score_min, score_p50,
              score_p90, score_max, and below_threshold as numeric fields
          (4) below_threshold reflects the count of scores below
              min_score_threshold from settings
          (5) a run scoring a single listing still produces one
              retrieval_summary per collection
          (6) a collection_scores entry with an empty list is skipped
              gracefully
    WHY: A collection whose median score is 0.92 for every role it sees is
         not discriminating — it is noise. Score distribution logging makes
         this visible

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama HTTP); async_playwright
               (Playwright I/O); asyncio.sleep for throttle bypass
        Real:  PipelineRunner, ChromaDB via VectorStore with real indexed
               collections using distant embeddings, log file in tmp_path
        Never: Mock the retrieval summary computation; index real content
               via distant embeddings before running the pipeline so score
               distributions reflect genuine similarity queries
    """

    def test_one_retrieval_summary_per_collection_is_written_per_run(self) -> None:
        """
        Given a run scoring listings against four collections
        When the log file is read
        Then exactly four entries with event 'retrieval_summary' are present
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with distant embeddings (4 collections) and one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_distant_store(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: exactly 4 retrieval_summary entries (one per collection)
            summaries = [e for e in entries if e.get("event") == "retrieval_summary"]
            assert len(summaries) == len(_RETRIEVAL_COLLECTIONS), (
                f"Expected {len(_RETRIEVAL_COLLECTIONS)} retrieval_summary entries, "
                f"got {len(summaries)}. Events: {[e.get('event') for e in entries]}"
            )

    def test_retrieval_summary_names_the_collection(self) -> None:
        """
        Given a completed run
        When a retrieval_summary log entry is read
        Then it contains a 'collection' field matching a known collection name
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with distant embeddings and one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_distant_store(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: every retrieval_summary has a 'collection' field from _RETRIEVAL_COLLECTIONS
            summaries = [e for e in entries if e.get("event") == "retrieval_summary"]
            assert len(summaries) > 0, "Expected at least one retrieval_summary entry"
            collection_names = {s["collection"] for s in summaries}
            for name in collection_names:
                assert name in _RETRIEVAL_COLLECTIONS, (
                    f"Unexpected collection name '{name}', "
                    f"expected one of {_RETRIEVAL_COLLECTIONS}"
                )

    def test_retrieval_summary_includes_score_distribution_fields(self) -> None:
        """
        Given a completed run scoring at least two listings
        When a retrieval_summary log entry is read
        Then it contains numeric fields for score_min, score_p50,
        score_p90, score_max, and below_threshold
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with distant embeddings and two listings
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_distant_store(settings)
            listings = [
                _make_listing(external_id="1", title="Role A"),
                _make_listing(external_id="2", title="Role B"),
            ]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: each retrieval_summary has distribution fields
            summaries = [e for e in entries if e.get("event") == "retrieval_summary"]
            assert len(summaries) > 0, "Expected at least one retrieval_summary entry"
            for summary in summaries:
                for field in ("score_min", "score_p50", "score_p90", "score_max"):
                    assert field in summary, (
                        f"retrieval_summary for '{summary.get('collection')}' "
                        f"missing '{field}' field: {summary}"
                    )
                    assert isinstance(summary[field], (int, float)), (
                        f"'{field}' should be numeric, got {type(summary[field])}: "
                        f"{summary[field]}"
                    )
                assert "below_threshold" in summary, (
                    f"retrieval_summary for '{summary.get('collection')}' "
                    f"missing 'below_threshold' field: {summary}"
                )
                assert isinstance(summary["below_threshold"], int), (
                    f"'below_threshold' should be int, "
                    f"got {type(summary['below_threshold'])}: {summary['below_threshold']}"
                )

    def test_below_threshold_count_reflects_settings_min_score_threshold(self) -> None:
        """
        Given min_score_threshold is set to 0.9
        And a run scores five listings all below 0.9 on one collection
        When that collection's retrieval_summary entry is read
        Then below_threshold is 5
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: high threshold (0.9) — distant embeddings produce scores < 0.9
            settings = _make_settings(tmpdir, min_score_threshold=0.9)
            runner, mock_client = _make_runner_with_distant_store(settings)
            listings = [_make_listing(external_id=str(i), title=f"Role {i}") for i in range(1, 6)]

            # When: pipeline runs five listings
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: at least one collection has below_threshold == 5
            summaries = [e for e in entries if e.get("event") == "retrieval_summary"]
            assert len(summaries) > 0, "Expected at least one retrieval_summary entry"
            # With distant embeddings, all scores on a collection should be < 0.9
            # Check a required collection (resume) which always has scores
            resume_summaries = [s for s in summaries if s.get("collection") == "resume"]
            assert len(resume_summaries) == 1, (
                f"Expected 1 resume retrieval_summary, got {len(resume_summaries)}"
            )
            assert resume_summaries[0]["below_threshold"] == 5, (
                f"Expected below_threshold == 5 with threshold 0.9 and distant embeddings, "
                f"got {resume_summaries[0]['below_threshold']}"
            )

    def test_single_listing_run_produces_retrieval_summary_per_collection(self) -> None:
        """
        Given a run that scores exactly one listing
        When the log file is read
        Then retrieval_summary entries are present (one per collection)
        and each contains valid numeric fields
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with distant embeddings and exactly one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_distant_store(settings)
            listings = [_make_listing()]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: retrieval_summary entries with valid numeric fields
            summaries = [e for e in entries if e.get("event") == "retrieval_summary"]
            assert len(summaries) == len(_RETRIEVAL_COLLECTIONS), (
                f"Expected {len(_RETRIEVAL_COLLECTIONS)} retrieval_summary entries "
                f"for single listing, got {len(summaries)}"
            )
            for summary in summaries:
                assert "n_scored" in summary, f"retrieval_summary missing 'n_scored': {summary}"
                assert summary["n_scored"] >= 1, (  # type: ignore[operator]
                    f"Expected n_scored >= 1, got {summary['n_scored']}"
                )
                for field in ("score_min", "score_p50", "score_p90", "score_max"):
                    assert isinstance(summary.get(field), (int, float)), (
                        f"'{field}' should be numeric in {summary.get('collection')}: {summary}"
                    )

    def test_empty_collection_scores_entry_is_skipped(self) -> None:
        """
        Given a scorer whose collection_scores contains a phantom entry
              with an empty list
        When the pipeline runs and emits retrieval_summary entries
        Then no retrieval_summary is emitted for the phantom collection
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: a runner with distant embeddings and one listing
            settings = _make_settings(tmpdir)
            runner, mock_client = _make_runner_with_distant_store(settings)
            listings = [_make_listing()]

            # Override: inject an empty-list entry into the scorer's accumulator.
            # This exercises the defensive guard (if not scores: continue).
            runner._scorer._collection_scores["phantom_empty"] = []  # pyright: ignore[reportPrivateUsage]

            # When: pipeline runs
            entries = _run_pipeline_and_read_logs(tmpdir, listings, mock_client, runner)

            # Then: no retrieval_summary for the phantom collection
            summaries = [e for e in entries if e.get("event") == "retrieval_summary"]
            phantom_summaries = [s for s in summaries if s.get("collection") == "phantom_empty"]
            assert len(phantom_summaries) == 0, (
                f"Expected no retrieval_summary for phantom_empty, got {phantom_summaries}"
            )
            # And: real collections still have summaries
            assert len(summaries) >= 1, (
                f"Expected at least 1 real retrieval_summary, got {len(summaries)}"
            )
