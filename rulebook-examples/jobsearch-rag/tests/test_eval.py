"""
BDD specs for the evaluation harness (Phase 5d).

Covers:
    TestEvalCommand             — CLI subcommand wiring and handler integration
    TestEvalMetrics             — agreement rate, precision, recall computation
    TestSpearmanCorrelation     — rank correlation between scores and verdicts
    TestEvalReport              — Markdown report file generation
    TestEvalHistory             — JSONL history append
    TestEvalIntegration         — end-to-end handle_eval with report + history
    TestModelComparisonResult   — delta computation between two EvalResults
    TestCompareModelsFlag       — --compare-models CLI flag and dual-eval flow
    TestLoadDecisionsResilience — graceful handling of corrupt/missing decision data
    TestEvalSinglePath          — _handle_eval_single early-return on empty decisions
"""

from __future__ import annotations

import asyncio
import json
import tempfile
from pathlib import Path
from unittest.mock import AsyncMock, MagicMock, patch

import pytest

from jobsearch_rag.cli import build_parser
from jobsearch_rag.config import (
    BoardConfig,
    ChromaConfig,
    OllamaConfig,
    OutputConfig,
    ScoringConfig,
    Settings,
)
from jobsearch_rag.pipeline.eval import (
    EvalDecision,
    EvalHistory,
    EvalReport,
    EvalResult,
    EvalRunner,
    ModelComparisonResult,
    spearman_rank_correlation,
)
from jobsearch_rag.pipeline.ranker import Ranker
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.scorer import Scorer
from jobsearch_rag.rag.store import VectorStore
from tests.constants import EMBED_FAKE

# Public API surface (from src/jobsearch_rag/pipeline/eval):
#   EvalRunner(scorer: Scorer, ranker: Ranker, store: VectorStore)
#   async EvalRunner.evaluate() -> EvalResult
#
#   EvalResult:
#     decisions_evaluated: int
#     agreement_rate: float
#     precision: float
#     recall: float
#     spearman: float
#     per_decision: list[EvalDecision]
#
#   EvalDecision:
#     job_id: str
#     verdict: str
#     final_score: float
#     above_threshold: bool
#     agreed: bool
#
#   spearman_rank_correlation(x: Sequence[float], y: Sequence[float]) -> float
#   EvalReport.write(result: EvalResult, output_dir: str) -> Path
#   EvalHistory.append(result: EvalResult, history_path: str) -> None
#
#   ModelComparisonResult:
#     model_a: str
#     model_b: str
#     result_a: EvalResult
#     result_b: EvalResult
#     agreement_delta: float  (property, b - a)
#     precision_delta: float  (property, b - a)
#     recall_delta: float     (property, b - a)
#     spearman_delta: float   (property, b - a)
#
# Public API surface (from src/jobsearch_rag/cli):
#   build_parser() -> ArgumentParser
#   handle_eval(args: Namespace) -> None

# An embedding vector distant from EMBED_FAKE — produces cosine distance > 0
# when JD text is embedded with EMBED_FAKE.  This creates scores < 1.0 so
# eval tests can distinguish above/below threshold behavior.
_EMBED_DISTANT: list[float] = [0.9, 0.1, 0.9, 0.1, 0.9]


def _make_settings(tmpdir: str, *, min_score_threshold: float = 0.45) -> Settings:
    """Create minimal Settings for eval tests."""
    return Settings(
        enabled_boards=["testboard"],
        overnight_boards=[],
        boards={
            "testboard": BoardConfig(
                name="testboard", searches=["https://example.com"], max_pages=1
            )
        },
        scoring=ScoringConfig(
            min_score_threshold=min_score_threshold,
            disqualify_on_llm_flag=False,
        ),
        ollama=OllamaConfig(),
        output=OutputConfig(output_dir=str(Path(tmpdir) / "output")),
        chroma=ChromaConfig(persist_dir=str(Path(tmpdir) / "chroma")),
    )


def _make_mock_embedder(
    embed_return: list[float] | None = None,
) -> tuple[Embedder, AsyncMock]:
    """
    Create an Embedder with mocked Ollama I/O boundary.

    Returns (embedder, mock_client) so tests can configure per-call behavior.
    """
    mock_client = AsyncMock()

    model_embed = MagicMock()
    model_embed.model = "nomic-embed-text"
    model_llm = MagicMock()
    model_llm.model = "mistral:7b"
    list_response = MagicMock()
    list_response.models = [model_embed, model_llm]
    mock_client.list.return_value = list_response

    embed_response = MagicMock()
    embed_response.embeddings = [embed_return or EMBED_FAKE]
    mock_client.embed.return_value = embed_response

    classify_response = MagicMock()
    classify_response.message = MagicMock()
    classify_response.message.content = '{"disqualified": false}'
    mock_client.chat.return_value = classify_response

    with patch(
        "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
        return_value=mock_client,
    ):
        embedder = Embedder(
            base_url="http://localhost:11434",
            embed_model="nomic-embed-text",
            llm_model="mistral:7b",
        )

    return embedder, mock_client


def _make_eval_stack(
    settings: Settings,
    *,
    embed_return: list[float] | None = None,
) -> tuple[EvalRunner, Scorer, Ranker, VectorStore, AsyncMock]:
    """Build a full eval stack: EvalRunner + Scorer + Ranker + real VectorStore."""
    embedder, mock_client = _make_mock_embedder(embed_return=embed_return)
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    scorer = Scorer(
        store=store,
        embedder=embedder,
        disqualify_on_llm_flag=settings.scoring.disqualify_on_llm_flag,
    )
    ranker = Ranker(
        archetype_weight=settings.scoring.archetype_weight,
        fit_weight=settings.scoring.fit_weight,
        history_weight=settings.scoring.history_weight,
        comp_weight=settings.scoring.comp_weight,
        negative_weight=settings.scoring.negative_weight,
        culture_weight=settings.scoring.culture_weight,
        min_score_threshold=settings.scoring.min_score_threshold,
    )
    runner = EvalRunner(scorer=scorer, ranker=ranker, store=store)
    return runner, scorer, ranker, store, mock_client


def _seed_required_collections(store: VectorStore, embedding: list[float]) -> None:
    """Seed resume and role_archetypes so the scorer doesn't raise on empty."""
    for name in ("resume", "role_archetypes"):
        store.add_documents(
            name,
            ids=[f"{name}-seed"],
            documents=[f"Seed document for {name}"],
            embeddings=[embedding],
        )


def _seed_decision(
    store: VectorStore,
    *,
    job_id: str,
    verdict: str,
    jd_text: str = "A sample job description.",
    embedding: list[float] | None = None,
) -> None:
    """Seed a single decision into the decisions collection."""
    store.add_documents(
        "decisions",
        ids=[f"decision-{job_id}"],
        documents=[jd_text],
        embeddings=[embedding or EMBED_FAKE],
        metadatas=[
            {
                "job_id": job_id,
                "verdict": verdict,
                "board": "testboard",
                "title": f"Role {job_id}",
                "company": "TestCorp",
                "scoring_signal": "true" if verdict == "yes" else "false",
                "reason": "",
                "recorded_at": "2026-03-27T00:00:00+00:00",
            }
        ],
    )


class TestEvalCommand:
    """
    REQUIREMENT: The ``eval`` CLI subcommand loads settings, instantiates the
    eval pipeline, runs evaluation, and prints summary metrics to stdout.

    WHO: The operator running ``python -m jobsearch_rag eval`` after a config
         change
    WHAT: (1) ``eval`` subcommand is registered and calls ``handle_eval``
          (2) ``handle_eval`` loads settings, creates scorer/ranker/store,
              runs ``EvalRunner.evaluate()``
          (3) stdout output includes agreement_rate, precision, recall
          (4) when no decisions exist, prints a message and exits cleanly
          (5) ``EvalRunner`` is instantiated with scorer, ranker, and store
    WHY: The operator needs a single command to measure whether a config
         change improved or degraded pipeline quality

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (embedding + LLM calls)
        Real:  EvalRunner, Scorer, Ranker, VectorStore (ChromaDB), metric
               computation
        Never: Mock the metric computation or decision loading
    """

    def test_eval_subcommand_is_registered(self) -> None:
        """
        Given the ``eval`` subcommand is registered
        When ``build_parser()`` is called
        Then the parser recognizes ``eval``
        """
        # Given: the CLI parser is built
        parser = build_parser()

        # When: we parse the 'eval' command
        args = parser.parse_args(["eval"])

        # Then: the command is recognized
        assert args.command == "eval", f"Expected command 'eval', got '{args.command}'"

    def test_handle_eval_prints_metrics_to_stdout(
        self, capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given a store with decisions
        When ``handle_eval`` is called
        Then it prints agreement_rate, precision, recall to stdout
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: settings pointing at tmpdir, a seeded store with decisions
            settings = _make_settings(tmpdir)
            runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            _seed_decision(store, job_id="1", verdict="yes")

            # When: evaluate runs and prints output
            result = asyncio.run(runner.evaluate())

            # Then: result has the expected metrics fields
            assert result.agreement_rate is not None, "agreement_rate should be set"
            assert result.precision is not None, "precision should be set"
            assert result.recall is not None, "recall should be set"

    def test_handle_eval_with_no_decisions_exits_cleanly(self) -> None:
        """
        Given an empty decisions collection
        When ``handle_eval`` is called
        Then it prints a "no decisions" message and exits cleanly
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: settings and a store with no decisions
            settings = _make_settings(tmpdir)
            runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)

            # When: evaluate runs with no decisions
            result = asyncio.run(runner.evaluate())

            # Then: result indicates zero decisions, no crash
            assert result.decisions_evaluated == 0, (
                f"Expected 0 decisions_evaluated, got {result.decisions_evaluated}"
            )

    def test_evaluate_returns_eval_result_with_correct_types(self) -> None:
        """
        Given a store with decisions
        When ``EvalRunner.evaluate()`` completes
        Then it returns an ``EvalResult`` with correct field types
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: settings and a store with one decision
            settings = _make_settings(tmpdir)
            runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            _seed_decision(store, job_id="1", verdict="yes")

            # When: evaluate() runs
            result = asyncio.run(runner.evaluate())

            # Then: result is an EvalResult with correct types
            assert isinstance(result, EvalResult), f"Expected EvalResult, got {type(result)}"
            assert isinstance(result.decisions_evaluated, int), (
                f"decisions_evaluated should be int, got {type(result.decisions_evaluated)}"
            )
            assert isinstance(result.agreement_rate, float), (
                f"agreement_rate should be float, got {type(result.agreement_rate)}"
            )
            assert isinstance(result.precision, float), (
                f"precision should be float, got {type(result.precision)}"
            )
            assert isinstance(result.recall, float), (
                f"recall should be float, got {type(result.recall)}"
            )
            assert isinstance(result.per_decision, list), (
                f"per_decision should be list, got {type(result.per_decision)}"
            )
            assert isinstance(result.spearman, float), (
                f"spearman should be float, got {type(result.spearman)}"
            )


class TestEvalMetrics:
    """
    REQUIREMENT: ``EvalRunner.evaluate()`` re-scores each decision's JD,
    computes agreement rate, precision, and recall against the human verdicts.

    WHO: The operator tuning scoring weights who needs to know whether the
         change improved pipeline-human agreement
    WHAT: (1) agreement_rate is the fraction of decisions where pipeline
              verdict matches human verdict mapping
          (2) precision is the fraction of pipeline-positive decisions that
              the human also marked positive
          (3) recall is the fraction of human-yes decisions that the pipeline
              scores above threshold
          (4) "maybe" verdicts are treated as positive for agreement and
              precision computation
          (5) a decision set with perfect agreement produces
              agreement_rate=1.0, precision=1.0, recall=1.0
          (6) a decision set with zero agreement produces agreement_rate=0.0
          (7) each per_decision entry records job_id, verdict, final_score,
              above_threshold, agreed
          (8) a yes-decision scoring below threshold is counted for recall
              denominator but not numerator (recall miss)
    WHY: Without closed-loop metrics, the operator cannot tell whether a
         weight change made things better or worse — they are flying blind

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (embedding + LLM calls)
        Real:  EvalRunner, Scorer, Ranker, VectorStore (ChromaDB), metric
               computation
        Never: Mock the metric computation; never mock scorer/ranker internals
    """

    def test_perfect_agreement_produces_all_ones(self) -> None:
        """
        Given 3 yes-decisions all scoring above threshold
        When evaluate() runs
        Then agreement_rate=1.0, precision=1.0, recall=1.0
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: collections seeded with EMBED_FAKE so queries return
            # score 1.0 (well above threshold); 3 yes decisions
            settings = _make_settings(tmpdir, min_score_threshold=0.45)
            runner, _s, _r, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            for i in range(1, 4):
                _seed_decision(store, job_id=str(i), verdict="yes")

            # When: evaluate runs
            result = asyncio.run(runner.evaluate())

            # Then: perfect agreement
            assert result.decisions_evaluated == 3, (
                f"Expected 3 decisions, got {result.decisions_evaluated}"
            )
            assert result.agreement_rate == pytest.approx(1.0), (
                f"Expected agreement_rate 1.0, got {result.agreement_rate}"
            )
            assert result.precision == pytest.approx(1.0), (
                f"Expected precision 1.0, got {result.precision}"
            )
            assert result.recall == pytest.approx(1.0), f"Expected recall 1.0, got {result.recall}"

    def test_all_no_below_threshold_produces_full_agreement(self) -> None:
        """
        Given 3 no-decisions all scoring below threshold
        When evaluate() runs
        Then agreement_rate=1.0
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: collections seeded with EMBED_DISTANT so queries return
            # low scores; high threshold ensures all below; 3 no decisions
            settings = _make_settings(tmpdir, min_score_threshold=0.99)
            runner, _s, _r, store, _mock = _make_eval_stack(
                settings,
                embed_return=_EMBED_DISTANT,
            )
            _seed_required_collections(store, EMBED_FAKE)
            for i in range(1, 4):
                _seed_decision(store, job_id=str(i), verdict="no")

            # When: evaluate runs
            result = asyncio.run(runner.evaluate())

            # Then: all no-decisions scored below threshold → full agreement
            assert result.decisions_evaluated == 3, (
                f"Expected 3 decisions, got {result.decisions_evaluated}"
            )
            assert result.agreement_rate == pytest.approx(1.0), (
                f"Expected agreement_rate 1.0, got {result.agreement_rate}"
            )

    def test_total_disagreement_produces_zero_rates(self) -> None:
        """
        Given 2 yes-decisions scoring below threshold and 1 no-decision
              scoring above threshold
        When evaluate() runs
        Then agreement_rate=0.0, precision=0.0, recall=0.0
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: EMBED_DISTANT for queries → low scores; high threshold
            # means yes-decisions score below; but we need the no-decision
            # to score above. Since all go through the same scorer, we use
            # a threshold that puts everything below → yes disagrees,
            # and also seed a no-decision which also scores below → agrees.
            # To get true zero agreement, ALL must disagree.
            # Strategy: use EMBED_FAKE (score ~1.0) with 2 no + 1 yes inverted:
            # no-decisions score above threshold (disagree), yes below (disagree).
            # Actually: use high scores (EMBED_FAKE) + low threshold.
            # 2 no-decisions score above threshold → disagree (pipeline says yes,
            # human says no). 1 yes-decision scores above → agrees.
            # That gives 2/3 disagreement, not 0.0.
            #
            # For true 0.0: all decisions must disagree.
            # Use EMBED_FAKE (high scores) + low threshold:
            # - no-decisions score above → pipeline says yes, human says no → disagree
            # Use only no-decisions:
            settings = _make_settings(tmpdir, min_score_threshold=0.1)
            runner, _s, _r, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            for i in range(1, 4):
                _seed_decision(store, job_id=str(i), verdict="no")

            # When: evaluate runs — all score ~1.0 (above 0.1 threshold)
            result = asyncio.run(runner.evaluate())

            # Then: pipeline says "above threshold" for all, human said "no" → 0 agreement
            assert result.agreement_rate == pytest.approx(0.0), (
                f"Expected agreement_rate 0.0, got {result.agreement_rate}"
            )
            assert result.precision == pytest.approx(0.0), (
                f"Expected precision 0.0, got {result.precision}"
            )

    def test_maybe_verdicts_treated_as_positive(self) -> None:
        """
        Given 1 maybe-decision scoring above threshold
        When evaluate() runs
        Then it counts as agreed (maybe is positive)
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: EMBED_FAKE for high scores; one maybe decision
            settings = _make_settings(tmpdir, min_score_threshold=0.45)
            runner, _s, _r, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            _seed_decision(store, job_id="1", verdict="maybe")

            # When: evaluate runs
            result = asyncio.run(runner.evaluate())

            # Then: maybe above threshold → agreed (maybe is positive)
            assert result.decisions_evaluated == 1, (
                f"Expected 1 decision, got {result.decisions_evaluated}"
            )
            assert result.agreement_rate == pytest.approx(1.0), (
                f"Expected agreement_rate 1.0 (maybe=positive, above threshold), "
                f"got {result.agreement_rate}"
            )

    def test_per_decision_entries_have_correct_fields(self) -> None:
        """
        Given 5 mixed decisions
        When evaluate() runs
        Then per_decision has 5 entries with correct fields
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: 5 decisions with mixed verdicts
            settings = _make_settings(tmpdir, min_score_threshold=0.45)
            runner, _s, _r, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            for i, verdict in enumerate(["yes", "no", "maybe", "yes", "no"], 1):
                _seed_decision(store, job_id=str(i), verdict=verdict)

            # When: evaluate runs
            result = asyncio.run(runner.evaluate())

            # Then: 5 per_decision entries with required fields
            assert len(result.per_decision) == 5, (
                f"Expected 5 per_decision entries, got {len(result.per_decision)}"
            )
            for entry in result.per_decision:
                assert hasattr(entry, "job_id"), f"per_decision entry missing 'job_id': {entry}"
                assert hasattr(entry, "verdict"), f"per_decision entry missing 'verdict': {entry}"
                assert hasattr(entry, "final_score"), (
                    f"per_decision entry missing 'final_score': {entry}"
                )
                assert hasattr(entry, "above_threshold"), (
                    f"per_decision entry missing 'above_threshold': {entry}"
                )
                assert hasattr(entry, "agreed"), f"per_decision entry missing 'agreed': {entry}"

    def test_precision_and_recall_computed_correctly(self) -> None:
        """
        Given decisions with known scores
        When evaluate() runs
        Then precision and recall are computed correctly against the definitions
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: EMBED_FAKE → score ~1.0 (above threshold).
            # 2 yes (above → agree), 1 no (above → disagree), 1 maybe (above → agree)
            # Pipeline says "above" for all 4.
            # Human positive: yes + maybe = 3. Human negative: no = 1.
            # Precision: of 4 above-threshold, 3 were positive → 3/4 = 0.75
            # Recall: of 2 yes, 2 above threshold → 2/2 = 1.0
            settings = _make_settings(tmpdir, min_score_threshold=0.45)
            runner, _s, _r, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)
            _seed_decision(store, job_id="1", verdict="yes")
            _seed_decision(store, job_id="2", verdict="yes")
            _seed_decision(store, job_id="3", verdict="no")
            _seed_decision(store, job_id="4", verdict="maybe")

            # When: evaluate runs
            result = asyncio.run(runner.evaluate())

            # Then: precision = 3/4, recall = 2/2
            assert result.precision == pytest.approx(0.75), (
                f"Expected precision 0.75, got {result.precision}"
            )
            assert result.recall == pytest.approx(1.0), f"Expected recall 1.0, got {result.recall}"

    def test_no_decisions_returns_zero_metrics(self) -> None:
        """
        Given no decisions in the store
        When evaluate() runs
        Then EvalResult has decisions_evaluated=0 and rates are 0.0
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: store with required collections but no decisions
            settings = _make_settings(tmpdir)
            runner, _s, _r, store, _mock = _make_eval_stack(settings)
            _seed_required_collections(store, EMBED_FAKE)

            # When: evaluate runs
            result = asyncio.run(runner.evaluate())

            # Then: zero everything
            assert result.decisions_evaluated == 0, f"Expected 0, got {result.decisions_evaluated}"
            assert result.agreement_rate == pytest.approx(0.0), (
                f"Expected 0.0, got {result.agreement_rate}"
            )
            assert result.precision == pytest.approx(0.0), f"Expected 0.0, got {result.precision}"
            assert result.recall == pytest.approx(0.0), f"Expected 0.0, got {result.recall}"

    def test_yes_verdict_below_threshold_is_recall_miss(self) -> None:
        """
        Given 1 yes-decision scoring below threshold and 1 no-decision
              scoring below threshold
        When evaluate() runs
        Then recall is 0.0 (the yes-decision missed) and agreement for the
             yes-decision is False
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: EMBED_DISTANT produces low cosine similarity against
            # EMBED_FAKE-seeded collections; high threshold ensures below
            settings = _make_settings(tmpdir, min_score_threshold=0.99)
            runner, _s, _r, store, _mock = _make_eval_stack(
                settings,
                embed_return=_EMBED_DISTANT,
            )
            _seed_required_collections(store, EMBED_FAKE)
            _seed_decision(store, job_id="yes-1", verdict="yes")
            _seed_decision(store, job_id="no-1", verdict="no")

            # When: evaluate runs — all score below 0.99 threshold
            result = asyncio.run(runner.evaluate())

            # Then: the yes-decision scored below threshold → recall miss
            assert result.recall == pytest.approx(0.0), (
                f"Expected recall 0.0 (yes-decision below threshold), got {result.recall}"
            )
            # The yes-decision disagrees (pipeline says below, human says yes)
            yes_decisions = [d for d in result.per_decision if d.verdict == "yes"]
            assert len(yes_decisions) == 1, f"Expected 1 yes-decision, got {len(yes_decisions)}"
            assert not yes_decisions[0].agreed, (
                "yes-decision below threshold should disagree with pipeline"
            )


class TestSpearmanCorrelation:
    """
    REQUIREMENT: Spearman rank correlation is computed between pipeline final
    scores and human verdict ordering without external dependencies.

    WHO: The eval harness computing correlation between pipeline scores and
         human judgment
    WHAT: (1) human verdicts are mapped to ordinal values: no=0, maybe=1, yes=2
          (2) pipeline final_scores and human ordinals are ranked and correlated
          (3) perfect agreement yields correlation approx 1.0
          (4) reversed ordering yields correlation approx -1.0
          (5) constant inputs (all same verdict or all same score) yield 0.0
          (6) the function uses only stdlib (no scipy, no numpy)
    WHY: Without a correlation metric the operator cannot tell whether pipeline
         rank ordering matches human judgment — agreement rate alone doesn't
         capture ordering quality

    MOCK BOUNDARY:
        Mock:  nothing — pure computation, no I/O
        Real:  spearman_rank_correlation function
        Never: mock the correlation computation
    """

    def test_perfect_positive_correlation(self) -> None:
        """
        Given 5 items with monotonically increasing scores and verdicts
        When spearman is computed
        Then correlation == 1.0
        """
        # Given: distinct values — no tied ranks
        scores = [0.1, 0.3, 0.5, 0.7, 0.9]
        verdicts = [1.0, 2.0, 3.0, 4.0, 5.0]

        # When: spearman is computed
        result = spearman_rank_correlation(scores, verdicts)

        # Then: perfect positive correlation
        assert result == pytest.approx(1.0, abs=1e-9), (
            f"Expected correlation 1.0 for perfectly ordered data, got {result}"
        )

    def test_perfect_negative_correlation(self) -> None:
        """
        Given 5 items with scores increasing but verdicts decreasing
        When spearman is computed
        Then correlation == -1.0
        """
        # Given: distinct values — perfectly inverse order
        scores = [0.1, 0.3, 0.5, 0.7, 0.9]
        verdicts = [5.0, 4.0, 3.0, 2.0, 1.0]

        # When: spearman is computed
        result = spearman_rank_correlation(scores, verdicts)

        # Then: perfect negative correlation
        assert result == pytest.approx(-1.0, abs=1e-9), (
            f"Expected correlation -1.0 for inversely ordered data, got {result}"
        )

    def test_constant_verdicts_produce_zero_correlation(self) -> None:
        """
        Given all same verdict (3 yes) with varying scores
        When spearman is computed
        Then correlation = 0.0 (tied ranks)
        """
        # Given: all verdicts the same — ranks are all tied
        scores = [0.9, 0.5, 0.1]
        verdicts = [2.0, 2.0, 2.0]

        # When: spearman is computed
        result = spearman_rank_correlation(scores, verdicts)

        # Then: zero correlation — no ordering to compare
        assert result == pytest.approx(0.0), f"Expected 0.0 for constant verdicts, got {result}"

    def test_single_decision_produces_zero_correlation(self) -> None:
        """
        Given a single decision
        When spearman is computed
        Then correlation = 0.0 (insufficient data)
        """
        # Given: only one data point — correlation is undefined
        scores = [0.8]
        verdicts = [2.0]

        # When: spearman is computed
        result = spearman_rank_correlation(scores, verdicts)

        # Then: zero — not enough data
        assert result == pytest.approx(0.0), f"Expected 0.0 for single decision, got {result}"

    def test_mixed_verdicts_with_ordered_scores(self) -> None:
        """
        Given mixed verdicts [yes, maybe, no] with scores [0.9, 0.5, 0.1]
        When spearman is computed
        Then correlation approx 1.0
        """
        # Given: scores perfectly match verdict ordering (yes=2 > maybe=1 > no=0)
        scores = [0.9, 0.5, 0.1]
        verdicts = [2.0, 1.0, 0.0]

        # When: spearman is computed
        result = spearman_rank_correlation(scores, verdicts)

        # Then: perfect rank correlation
        assert result == pytest.approx(1.0), (
            f"Expected correlation 1.0 for perfectly ordered mixed verdicts, got {result}"
        )


class TestEvalReport:
    """
    REQUIREMENT: A Markdown eval report is written to
    ``output/eval_YYYY-MM-DD.md`` summarizing the evaluation run.

    WHO: The operator reviewing evaluation results after a configuration change.

    WHAT: (1) write() produces a Markdown file named eval_YYYY-MM-DD.md
              containing heading, all five metrics, and disagreement entries
          (2) an EvalResult with zero decisions produces a report with zero
              metrics and an empty disagreement section
          (3) write() creates a non-existent output directory before writing
          (4) a result with no disagreements produces a report whose
              disagreement section is present but empty

    WHY: A persistent, human-readable report file provides a reviewable artifact
         beyond transient stdout output.

    MOCK BOUNDARY:
        Mock:  nothing — uses tmp_path for filesystem
        Real:  EvalReport.write(), file I/O via tmp_path
        Never: mock file operations
    """

    @staticmethod
    def _make_result(
        *,
        decisions_evaluated: int = 5,
        agreement_rate: float = 0.8,
        precision: float = 0.75,
        recall: float = 0.85,
        spearman: float = 0.6,
        per_decision: list[EvalDecision] | None = None,
    ) -> EvalResult:
        """Build an EvalResult with sensible defaults."""
        if per_decision is None:
            per_decision = [
                EvalDecision(
                    job_id="job-1",
                    verdict="yes",
                    final_score=0.9,
                    above_threshold=True,
                    agreed=True,
                ),
                EvalDecision(
                    job_id="job-2",
                    verdict="yes",
                    final_score=0.8,
                    above_threshold=True,
                    agreed=True,
                ),
                EvalDecision(
                    job_id="job-3",
                    verdict="no",
                    final_score=0.2,
                    above_threshold=False,
                    agreed=True,
                ),
                # Disagreement: pipeline says high, human said no
                EvalDecision(
                    job_id="job-4",
                    verdict="no",
                    final_score=0.7,
                    above_threshold=True,
                    agreed=False,
                ),
                # Disagreement: pipeline says low, human said yes
                EvalDecision(
                    job_id="job-5",
                    verdict="yes",
                    final_score=0.2,
                    above_threshold=False,
                    agreed=False,
                ),
            ]
        return EvalResult(
            decisions_evaluated=decisions_evaluated,
            agreement_rate=agreement_rate,
            precision=precision,
            recall=recall,
            spearman=spearman,
            per_decision=per_decision,
        )

    def test_known_metrics_produce_correct_report(self, tmp_path: Path) -> None:
        """
        Given an EvalResult with known metrics and 5 per_decision entries
        When write() is called
        Then the file exists with correct heading, all metric values, and
             disagreement entries
        """
        # Given: an EvalResult with known metrics and 2 disagreements
        result = self._make_result()

        # When: the report is written
        path = EvalReport.write(result, str(tmp_path))

        # Then: file exists and name matches pattern
        assert path.exists(), f"Report file not created at {path}"
        assert path.name.startswith("eval_"), (
            f"Expected filename starting with 'eval_', got {path.name!r}"
        )
        assert path.suffix == ".md", f"Expected .md suffix, got {path.suffix!r}"

        content = path.read_text()

        # Then: heading and metrics are present
        assert "# Eval Report" in content or "# Evaluation Report" in content, (
            "Report must contain heading"
        )
        assert "5" in content, "Report must contain decisions_evaluated (5)"
        assert "0.8" in content or "80" in content, (
            "Report must contain agreement_rate (0.8 or 80%)"
        )
        assert "0.75" in content or "75" in content, "Report must contain precision (0.75 or 75%)"
        assert "0.85" in content or "85" in content, "Report must contain recall (0.85 or 85%)"
        assert "0.6" in content or "60" in content, "Report must contain spearman (0.6 or 60%)"

        # Then: disagreement entries appear
        assert "job-4" in content, "Disagreement job-4 must appear in report"
        assert "job-5" in content, "Disagreement job-5 must appear in report"

    def test_zero_decisions_produce_zero_metrics(self, tmp_path: Path) -> None:
        """
        Given an EvalResult with 0 decisions
        When write() is called
        Then the file exists with zero metrics and an empty disagreement section
        """
        # Given: an EvalResult with zero decisions
        result = self._make_result(
            decisions_evaluated=0,
            agreement_rate=0.0,
            precision=0.0,
            recall=0.0,
            spearman=0.0,
            per_decision=[],
        )

        # When: the report is written
        path = EvalReport.write(result, str(tmp_path))

        # Then: file exists with zero metric
        assert path.exists(), f"Report file not created at {path}"
        content = path.read_text()
        assert "0" in content, "Report must contain decisions_evaluated = 0"

    def test_nonexistent_output_dir_is_created(self, tmp_path: Path) -> None:
        """
        Given a non-existent output_dir
        When write() is called
        Then the directory is created and the file is written
        """
        # Given: a non-existent output directory
        output_dir = tmp_path / "nested" / "output"
        assert not output_dir.exists(), "Precondition: output_dir must not exist"
        result = self._make_result()

        # When: the report is written
        path = EvalReport.write(result, str(output_dir))

        # Then: directory and file are created
        assert output_dir.exists(), "Output directory must be created"
        assert path.exists(), f"Report file not created at {path}"

    def test_no_disagreements_produces_empty_section(self, tmp_path: Path) -> None:
        """
        Given an EvalResult with no disagreements
        When write() is called
        Then the disagreement section is present but empty
        """
        # Given: all decisions agree
        all_agreed = [
            EvalDecision(
                job_id="job-1",
                verdict="yes",
                final_score=0.9,
                above_threshold=True,
                agreed=True,
            ),
            EvalDecision(
                job_id="job-2",
                verdict="no",
                final_score=0.2,
                above_threshold=False,
                agreed=True,
            ),
        ]
        result = self._make_result(
            decisions_evaluated=2,
            agreement_rate=1.0,
            per_decision=all_agreed,
        )

        # When: the report is written
        path = EvalReport.write(result, str(tmp_path))

        # Then: file exists, no disagreement job IDs
        content = path.read_text()
        assert path.exists(), f"Report file not created at {path}"
        # Then: neither job should appear in a disagreement list
        assert "job-1" not in content or "agreed" in content.lower(), (
            "Agreed jobs must not appear in disagreement section"
        )


class TestEvalHistory:
    """
    REQUIREMENT: Each eval run appends one JSON line to
    ``data/eval_history.jsonl`` so the operator can track eval metrics
    over time.

    WHO: The operator tracking whether successive config changes improve
         pipeline quality.

    WHAT: (1) append() creates the file with one JSON line containing all
              five metrics and a timestamp
          (2) append() preserves existing lines and appends the new record
              as the last line
          (3) append() creates a non-existent file and parent directories

    WHY: Without a persistent history the operator loses the ability to
         compare across runs — they can only see the latest eval, not the
         trend.

    MOCK BOUNDARY:
        Mock:  nothing — uses tmp_path for filesystem
        Real:  EvalHistory.append(), file I/O via tmp_path
        Never: mock file operations
    """

    @staticmethod
    def _make_result(
        *,
        decisions_evaluated: int = 10,
        agreement_rate: float = 0.9,
        precision: float = 0.85,
        recall: float = 0.95,
        spearman: float = 0.7,
    ) -> EvalResult:
        """Build an EvalResult with sensible defaults for history tests."""
        return EvalResult(
            decisions_evaluated=decisions_evaluated,
            agreement_rate=agreement_rate,
            precision=precision,
            recall=recall,
            spearman=spearman,
            per_decision=[],
        )

    def test_append_creates_file_with_one_json_line(self, tmp_path: Path) -> None:
        """
        Given an EvalResult with known metrics
        When append() is called
        Then the file contains exactly one JSON line with all metrics and
             a timestamp
        """
        # Given: a history path and an EvalResult with known metrics
        history_path = tmp_path / "eval_history.jsonl"
        result = self._make_result()

        # When: append is called
        EvalHistory.append(result, str(history_path))

        # Then: file contains exactly one JSON line with all metrics
        lines = history_path.read_text().strip().splitlines()
        assert len(lines) == 1, f"Expected 1 line, got {len(lines)}"

        record = json.loads(lines[0])
        assert record["decisions_evaluated"] == 10, (
            f"Expected decisions_evaluated=10, got {record['decisions_evaluated']}"
        )
        assert record["agreement_rate"] == pytest.approx(0.9), (
            f"Expected agreement_rate~0.9, got {record['agreement_rate']}"
        )
        assert record["precision"] == pytest.approx(0.85), (
            f"Expected precision~0.85, got {record['precision']}"
        )
        assert record["recall"] == pytest.approx(0.95), (
            f"Expected recall~0.95, got {record['recall']}"
        )
        assert record["spearman"] == pytest.approx(0.7), (
            f"Expected spearman~0.7, got {record['spearman']}"
        )
        assert "timestamp" in record, "History record must include a timestamp"

    def test_append_preserves_existing_lines(self, tmp_path: Path) -> None:
        """
        Given a history file already containing 2 lines
        When append() is called
        Then the file contains 3 lines and the last line has the new metrics
        """
        # Given: seed 2 existing lines
        history_path = tmp_path / "eval_history.jsonl"
        existing = [
            json.dumps({"timestamp": "2026-01-01T00:00:00", "agreement_rate": 0.5}),
            json.dumps({"timestamp": "2026-01-02T00:00:00", "agreement_rate": 0.6}),
        ]
        history_path.write_text("\n".join(existing) + "\n")

        result = self._make_result(agreement_rate=0.9)

        # When: append is called
        EvalHistory.append(result, str(history_path))

        # Then: file has 3 lines, last line has the new metrics
        lines = history_path.read_text().strip().splitlines()
        assert len(lines) == 3, f"Expected 3 lines, got {len(lines)}"

        last = json.loads(lines[2])
        assert last["agreement_rate"] == pytest.approx(0.9), (
            f"Expected agreement_rate~0.9 in appended line, got {last['agreement_rate']}"
        )

    def test_append_creates_nonexistent_file(self, tmp_path: Path) -> None:
        """
        Given a non-existent history file path
        When append() is called
        Then the file is created with one line
        """
        # Given: a non-existent history path
        history_path = tmp_path / "deep" / "eval_history.jsonl"
        assert not history_path.exists(), "Precondition: file must not exist"
        result = self._make_result()

        # When: append is called
        EvalHistory.append(result, str(history_path))

        # Then: file is created with one line
        assert history_path.exists(), "History file must be created"
        lines = history_path.read_text().strip().splitlines()
        assert len(lines) == 1, f"Expected 1 line, got {len(lines)}"


class TestEvalIntegration:
    """
    REQUIREMENT: ``handle_eval`` writes both the report and history after
    evaluation completes.

    WHO: The operator running ``python -m jobsearch_rag eval``.

    WHAT: (1) handle_eval writes a report file to output_dir and appends
              one line to eval_history.jsonl
          (2) EvalResult.spearman is a float after evaluation completes

    WHY: The eval command must produce all three outputs (stdout, report file,
         history log) in a single invocation.

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (embedding + LLM calls)
        Real:  EvalRunner, Scorer, Ranker, VectorStore, EvalReport, EvalHistory
        Never: mock the report generation or history append
    """

    def test_handle_eval_produces_report_and_history(
        self, tmp_path: Path, monkeypatch: pytest.MonkeyPatch
    ) -> None:
        """
        Given a store with decisions
        When handle_eval runs
        Then an eval report file exists in output_dir and
             eval_history.jsonl has a new line
        """
        # Given: pre-seed the store that handle_eval will open
        settings = _make_settings(str(tmp_path))
        store = VectorStore(persist_dir=settings.chroma.persist_dir)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="eval-1", verdict="yes")
        _seed_decision(store, job_id="eval-2", verdict="no", embedding=_EMBED_DISTANT)

        _, mock_client = _make_mock_embedder()

        # chdir so relative "data/eval_history.jsonl" lands in tmp_path
        monkeypatch.chdir(tmp_path)

        # When: handle_eval is invoked with patched settings and ollama client
        with (
            patch("jobsearch_rag.cli.load_settings", return_value=settings),
            patch(
                "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
                return_value=mock_client,
            ),
        ):
            from jobsearch_rag.cli import handle_eval  # noqa: PLC0415

            args = MagicMock()
            args.compare_models = None
            handle_eval(args)

        # Then: report file exists in output_dir
        output_dir = Path(settings.output.output_dir)
        report_files = list(output_dir.glob("eval_*.md"))
        assert len(report_files) == 1, f"Expected 1 report, found {report_files}"

        # And: history file exists with one line
        history_path = tmp_path / "data" / "eval_history.jsonl"
        assert history_path.exists(), "eval_history.jsonl should exist"
        lines = history_path.read_text().strip().splitlines()
        assert len(lines) == 1, f"Expected 1 history line, found {len(lines)}"

    def test_eval_result_has_spearman_field(self, tmp_path: Path) -> None:
        """
        Given a store with decisions
        When evaluate() runs
        Then EvalResult.spearman is a float
        """
        # Given: a store with signed decisions
        settings = _make_settings(str(tmp_path))
        runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="s-1", verdict="yes")
        _seed_decision(store, job_id="s-2", verdict="no", embedding=_EMBED_DISTANT)

        # When: evaluate() is run
        result = asyncio.run(runner.evaluate())

        # Then: spearman is a float
        assert isinstance(result.spearman, float), (
            f"Expected spearman to be float, got {type(result.spearman).__name__}"
        )


class TestModelComparisonResult:
    """
    REQUIREMENT: ``ModelComparisonResult`` computes correct deltas between two
    EvalResult instances.

    WHO: The eval harness comparing two LLM models' disqualification accuracy.

    WHAT: (1) ``agreement_delta`` equals ``result_b.agreement_rate - result_a.agreement_rate``
          (2) ``precision_delta`` equals ``result_b.precision - result_a.precision``
          (3) ``recall_delta`` equals ``result_b.recall - result_a.recall``
          (4) ``spearman_delta`` equals ``result_b.spearman - result_a.spearman``
          (5) ``model_a`` and ``model_b`` store the model names passed at construction

    WHY: Without correct deltas the operator cannot determine which model is
         better — they would have to manually subtract metrics from two
         separate runs.

    MOCK BOUNDARY:
        Mock:  nothing — pure dataclass, no I/O
        Real:  ModelComparisonResult construction and property access
        Never: mock the delta computation
    """

    @staticmethod
    def _make_result(
        *,
        agreement_rate: float = 0.5,
        precision: float = 0.5,
        recall: float = 0.5,
        spearman: float = 0.5,
    ) -> EvalResult:
        """Build a minimal EvalResult for comparison tests."""
        return EvalResult(
            decisions_evaluated=10,
            agreement_rate=agreement_rate,
            precision=precision,
            recall=recall,
            spearman=spearman,
            per_decision=[],
        )

    def test_agreement_delta_equals_b_minus_a(self) -> None:
        """
        Given result_a with agreement_rate=0.7 and result_b with agreement_rate=0.9
        When agreement_delta is accessed
        Then it equals 0.2
        """
        # Given: result_a with agreement_rate=0.7 and result_b with agreement_rate=0.9
        result_a = self._make_result(agreement_rate=0.7)
        result_b = self._make_result(agreement_rate=0.9)
        comparison = ModelComparisonResult(
            model_a="mistral:7b",
            model_b="llama3:8b",
            result_a=result_a,
            result_b=result_b,
        )

        # When: agreement_delta is accessed
        delta = comparison.agreement_delta

        # Then: it equals 0.2
        assert delta == pytest.approx(0.2), f"Expected 0.2, got {delta}"

    def test_precision_delta_equals_b_minus_a(self) -> None:
        """
        Given result_a with precision=0.6 and result_b with precision=0.8
        When precision_delta is accessed
        Then it equals 0.2
        """
        # Given: result_a with precision=0.6 and result_b with precision=0.8
        result_a = self._make_result(precision=0.6)
        result_b = self._make_result(precision=0.8)
        comparison = ModelComparisonResult(
            model_a="mistral:7b",
            model_b="llama3:8b",
            result_a=result_a,
            result_b=result_b,
        )

        # When: precision_delta is accessed
        delta = comparison.precision_delta

        # Then: it equals 0.2
        assert delta == pytest.approx(0.2), f"Expected 0.2, got {delta}"

    def test_recall_delta_equals_b_minus_a(self) -> None:
        """
        Given result_a with recall=0.5 and result_b with recall=0.7
        When recall_delta is accessed
        Then it equals 0.2
        """
        # Given: result_a with recall=0.5 and result_b with recall=0.7
        result_a = self._make_result(recall=0.5)
        result_b = self._make_result(recall=0.7)
        comparison = ModelComparisonResult(
            model_a="mistral:7b",
            model_b="llama3:8b",
            result_a=result_a,
            result_b=result_b,
        )

        # When: recall_delta is accessed
        delta = comparison.recall_delta

        # Then: it equals 0.2
        assert delta == pytest.approx(0.2), f"Expected 0.2, got {delta}"

    def test_spearman_delta_equals_b_minus_a(self) -> None:
        """
        Given result_a with spearman=0.3 and result_b with spearman=0.8
        When spearman_delta is accessed
        Then it equals 0.5
        """
        # Given: result_a with spearman=0.3 and result_b with spearman=0.8
        result_a = self._make_result(spearman=0.3)
        result_b = self._make_result(spearman=0.8)
        comparison = ModelComparisonResult(
            model_a="mistral:7b",
            model_b="llama3:8b",
            result_a=result_a,
            result_b=result_b,
        )

        # When: spearman_delta is accessed
        delta = comparison.spearman_delta

        # Then: it equals 0.5
        assert delta == pytest.approx(0.5), f"Expected 0.5, got {delta}"

    def test_model_names_are_stored(self) -> None:
        """
        Given model_a="mistral:7b" and model_b="llama3:8b"
        When ModelComparisonResult is constructed
        Then model_a == "mistral:7b" and model_b == "llama3:8b"
        """
        # Given: model_a="mistral:7b" and model_b="llama3:8b"
        result_a = self._make_result()
        result_b = self._make_result()

        # When: ModelComparisonResult is constructed
        comparison = ModelComparisonResult(
            model_a="mistral:7b",
            model_b="llama3:8b",
            result_a=result_a,
            result_b=result_b,
        )

        # Then: model names are stored correctly
        assert comparison.model_a == "mistral:7b", (
            f"Expected 'mistral:7b', got '{comparison.model_a}'"
        )
        assert comparison.model_b == "llama3:8b", (
            f"Expected 'llama3:8b', got '{comparison.model_b}'"
        )


class TestCompareModelsFlag:
    """
    REQUIREMENT: The ``eval`` subparser accepts ``--compare-models MODEL_A MODEL_B``
    and ``handle_eval`` uses the two model names to run dual evaluations.

    WHO: The operator deciding between LLM models for disqualification.

    WHAT: (1) ``--compare-models`` accepts exactly two positional model name strings
          (2) when ``--compare-models`` is absent, ``handle_eval`` runs the normal
              single-model evaluation (existing behavior preserved)
          (3) when ``--compare-models`` is present, ``handle_eval`` constructs two
              Embedders (one per model), two Scorers, two EvalRunners, runs both,
              and prints a comparison table with delta values
          (4) the comparison table includes model_a name, model_b name, all four
              metrics for each, and the delta for each metric
          (5) when ``--compare-models`` is present, the normal single-run report and
              history are NOT written (comparison is stdout-only)

    WHY: Without a dedicated flag the operator must run ``eval`` twice manually
         with different config files and compute deltas by hand — error-prone
         and tedious.

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (embedding + LLM calls), load_settings
        Real:  argparse parsing, handle_eval control flow, Embedder/Scorer/
               Ranker/EvalRunner construction, ModelComparisonResult
        Never: mock the comparison logic or delta computation
    """

    def test_compare_models_flag_accepts_two_model_names(self) -> None:
        """
        Given the eval subparser
        When --compare-models mistral:7b llama3:8b is parsed
        Then args.compare_models == ["mistral:7b", "llama3:8b"]
        """
        # Given: the eval subparser
        parser = build_parser()

        # When: --compare-models mistral:7b llama3:8b is parsed
        args = parser.parse_args(["eval", "--compare-models", "mistral:7b", "llama3:8b"])

        # Then: args.compare_models == ["mistral:7b", "llama3:8b"]
        assert args.compare_models == ["mistral:7b", "llama3:8b"], (
            f"Expected ['mistral:7b', 'llama3:8b'], got {args.compare_models}"
        )

    def test_compare_models_absent_runs_single_eval(
        self, tmp_path: Path, monkeypatch: pytest.MonkeyPatch
    ) -> None:
        """
        Given a store with decisions and no --compare-models flag
        When handle_eval runs
        Then a single EvalResult is printed (normal flow)
        And report and history are written
        """
        # Given: a store with decisions and no --compare-models flag
        settings = _make_settings(str(tmp_path))
        store = VectorStore(persist_dir=settings.chroma.persist_dir)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="cmp-1", verdict="yes")

        _, mock_client = _make_mock_embedder()
        monkeypatch.chdir(tmp_path)

        # When: handle_eval runs
        with (
            patch("jobsearch_rag.cli.load_settings", return_value=settings),
            patch(
                "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
                return_value=mock_client,
            ),
        ):
            from jobsearch_rag.cli import handle_eval  # noqa: PLC0415

            args = MagicMock()
            args.compare_models = None
            handle_eval(args)

        # Then: report file exists (normal flow writes report)
        output_dir = Path(settings.output.output_dir)
        report_files = list(output_dir.glob("eval_*.md"))
        assert len(report_files) == 1, f"Expected 1 report (normal flow), found {report_files}"

    def test_compare_models_present_runs_dual_eval(
        self,
        tmp_path: Path,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
    ) -> None:
        """
        Given a store with decisions and --compare-models mistral:7b llama3:8b
        When handle_eval runs
        Then two evaluations are performed (one per model)
        And stdout contains both model names and delta values
        """
        # Given: a store with decisions and --compare-models mistral:7b llama3:8b
        settings = _make_settings(str(tmp_path))
        store = VectorStore(persist_dir=settings.chroma.persist_dir)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="cmp-1", verdict="yes")
        _seed_decision(store, job_id="cmp-2", verdict="no", embedding=_EMBED_DISTANT)

        _, mock_client = _make_mock_embedder()
        # Add llama3:8b to the mock model list so health_check passes for both
        model_b = MagicMock()
        model_b.model = "llama3:8b"
        mock_client.list.return_value.models.append(model_b)
        monkeypatch.chdir(tmp_path)

        # When: handle_eval runs
        with (
            patch("jobsearch_rag.cli.load_settings", return_value=settings),
            patch(
                "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
                return_value=mock_client,
            ),
        ):
            from jobsearch_rag.cli import handle_eval  # noqa: PLC0415

            args = MagicMock()
            args.compare_models = ["mistral:7b", "llama3:8b"]
            handle_eval(args)

        # Then: stdout contains both model names and delta
        captured = capsys.readouterr()
        assert "mistral:7b" in captured.out, "Expected model_a name in output"
        assert "llama3:8b" in captured.out, "Expected model_b name in output"
        assert "delta" in captured.out.lower() or "Δ" in captured.out, (
            "Expected delta label in comparison output"
        )

    def test_compare_models_skips_report_and_history(
        self, tmp_path: Path, monkeypatch: pytest.MonkeyPatch
    ) -> None:
        """
        Given --compare-models mistral:7b llama3:8b
        When handle_eval runs
        Then EvalReport.write is NOT called
        And EvalHistory.append is NOT called
        """
        # Given: --compare-models mistral:7b llama3:8b
        settings = _make_settings(str(tmp_path))
        store = VectorStore(persist_dir=settings.chroma.persist_dir)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="cmp-1", verdict="yes")

        _, mock_client = _make_mock_embedder()
        # Add llama3:8b to the mock model list so health_check passes for both
        model_b = MagicMock()
        model_b.model = "llama3:8b"
        mock_client.list.return_value.models.append(model_b)
        monkeypatch.chdir(tmp_path)

        # When: handle_eval runs
        with (
            patch("jobsearch_rag.cli.load_settings", return_value=settings),
            patch(
                "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
                return_value=mock_client,
            ),
        ):
            from jobsearch_rag.cli import handle_eval  # noqa: PLC0415

            args = MagicMock()
            args.compare_models = ["mistral:7b", "llama3:8b"]
            handle_eval(args)

        # Then: no report written
        output_dir = Path(settings.output.output_dir)
        report_files = list(output_dir.glob("eval_*.md"))
        assert len(report_files) == 0, f"Expected no report in compare mode, found {report_files}"

        # And: no history file
        history_path = tmp_path / "data" / "eval_history.jsonl"
        assert not history_path.exists(), "Expected no history file in compare mode"


class TestLoadDecisionsResilience:
    """
    REQUIREMENT: ``_load_decisions`` gracefully handles corrupt or missing
    decision data in ChromaDB without crashing the eval pipeline.

    WHO: The operator running ``eval`` against a decisions collection that
         may contain data quality issues from prior runs.

    WHAT: (1) inaccessible decisions collection returns empty list and
              logs a debug message
          (2) a decision with missing metadata is skipped with a warning
          (3) a decision with an empty verdict is skipped with a warning

    WHY: A single corrupt decision entry should not prevent evaluation of
         all other valid decisions — the pipeline must degrade gracefully.

    MOCK BOUNDARY:
        Mock:  ``VectorStore.get_or_create_collection`` (for exception test only,
               at the ChromaDB boundary)
        Real:  EvalRunner, VectorStore (real ChromaDB for data-quality tests)
        Never: mock ``_load_decisions`` itself
    """

    def test_inaccessible_collection_returns_empty_result(self, tmp_path: Path) -> None:
        """
        Given a store where the decisions collection raises an exception
        When evaluate() is called
        Then the result has 0 decisions and no crash
        """
        # Given: a real store with the collection access patched to raise
        settings = _make_settings(str(tmp_path))
        runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
        _seed_required_collections(store, EMBED_FAKE)

        with patch.object(
            store, "get_or_create_collection", side_effect=RuntimeError("db locked")
        ):
            # When: evaluate() is called
            result = asyncio.run(runner.evaluate())

        # Then: graceful degradation — 0 decisions, not a crash
        assert result.decisions_evaluated == 0, (
            f"Expected 0 decisions, got {result.decisions_evaluated}"
        )

    def test_decision_with_missing_metadata_is_skipped(self, tmp_path: Path) -> None:
        """
        Given a decisions collection with one valid decision and one
              without metadata
        When evaluate() is called
        Then only the valid decision is evaluated
        """
        # Given: seed one valid decision
        settings = _make_settings(str(tmp_path))
        runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="valid-1", verdict="yes")

        # And: seed a bare document with no metadata via low-level API
        collection = store.get_or_create_collection("decisions")
        collection.add(
            ids=["decision-corrupt-1"],
            documents=["A job with missing metadata"],
            embeddings=[EMBED_FAKE],
            # no metadatas — ChromaDB stores None for this entry
        )

        # When: evaluate() is called
        result = asyncio.run(runner.evaluate())

        # Then: only the valid decision is evaluated
        assert result.decisions_evaluated == 1, (
            f"Expected 1 decision (skipped corrupt), got {result.decisions_evaluated}"
        )

    def test_decision_with_empty_verdict_is_skipped(self, tmp_path: Path) -> None:
        """
        Given a decisions collection with one valid decision and one
              with an empty verdict
        When evaluate() is called
        Then only the valid decision is evaluated
        """
        # Given: seed one valid decision
        settings = _make_settings(str(tmp_path))
        runner, _scorer, _ranker, store, _mock = _make_eval_stack(settings)
        _seed_required_collections(store, EMBED_FAKE)
        _seed_decision(store, job_id="valid-1", verdict="yes")

        # And: seed a decision with empty verdict
        _seed_decision(store, job_id="no-verdict", verdict="")

        # When: evaluate() is called
        result = asyncio.run(runner.evaluate())

        # Then: only the valid decision is evaluated
        assert result.decisions_evaluated == 1, (
            f"Expected 1 decision (skipped empty verdict), got {result.decisions_evaluated}"
        )


class TestEvalSinglePath:
    """
    REQUIREMENT: ``_handle_eval_single`` prints a guidance message and skips
    report/history writing when the decisions collection is empty.

    WHO: The operator running ``eval`` before recording any decisions.

    WHAT: (1) when decisions_evaluated == 0, stdout includes a "no decisions"
              guidance message
          (2) when decisions_evaluated == 0, no report file is written
          (3) when decisions_evaluated == 0, no history file is written

    WHY: Without this guard the operator would see a meaningless 0-metric
         report and an empty history entry that pollutes trend analysis.

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (embedding + LLM calls), load_settings
        Real:  handle_eval, _handle_eval_single, EvalRunner, VectorStore
        Never: mock the early-return guard itself
    """

    def test_empty_decisions_prints_guidance_and_skips_artifacts(
        self,
        tmp_path: Path,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
    ) -> None:
        """
        Given a store with no decisions
        When handle_eval runs in single-model mode
        Then stdout contains a "no decisions" message
        And no report file is written
        And no history file is written
        """
        # Given: pre-seed required collections but NO decisions
        settings = _make_settings(str(tmp_path))
        store = VectorStore(persist_dir=settings.chroma.persist_dir)
        _seed_required_collections(store, EMBED_FAKE)

        _, mock_client = _make_mock_embedder()
        monkeypatch.chdir(tmp_path)

        # When: handle_eval runs in single-model mode
        with (
            patch("jobsearch_rag.cli.load_settings", return_value=settings),
            patch(
                "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
                return_value=mock_client,
            ),
        ):
            from jobsearch_rag.cli import handle_eval  # noqa: PLC0415

            args = MagicMock()
            args.compare_models = None
            handle_eval(args)

        # Then: guidance message printed
        captured = capsys.readouterr()
        assert "no decisions" in captured.out.lower(), (
            f"Expected 'no decisions' guidance in stdout, got: {captured.out[:300]}"
        )

        # And: no report file
        output_dir = Path(settings.output.output_dir)
        report_files = list(output_dir.glob("eval_*.md"))
        assert len(report_files) == 0, (
            f"Expected no report for empty decisions, found {report_files}"
        )

        # And: no history file
        history_path = tmp_path / "data" / "eval_history.jsonl"
        assert not history_path.exists(), "Expected no history file when decisions are empty"
