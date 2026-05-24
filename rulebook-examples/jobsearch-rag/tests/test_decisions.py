"""
Decision history tests.

Maps to BDD spec: TestDecisionRecording

Spec classes:
    TestDecisionRecording
"""

from __future__ import annotations

import json as json_mod
import tempfile
from pathlib import Path
from typing import TYPE_CHECKING

import pytest

from jobsearch_rag.errors import ActionableError, ErrorType
from jobsearch_rag.rag.decisions import DecisionRecorder
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.store import VectorStore
from tests.conftest import make_mock_ollama_client

if TYPE_CHECKING:
    from collections.abc import Iterator

EMBED_TEST = [0.5, 0.5, 0.5, 0.5, 0.5]


@pytest.fixture
def store() -> Iterator[VectorStore]:
    """Yield a temporary VectorStore for test isolation."""
    with tempfile.TemporaryDirectory() as tmpdir:
        yield VectorStore(persist_dir=tmpdir)


@pytest.fixture
def mock_embedder() -> Embedder:
    """Real Embedder with ollama client stubbed at the I/O boundary."""
    mock_client = make_mock_ollama_client(embed_vector=EMBED_TEST)
    embedder = Embedder(
        base_url="http://localhost:11434",
        embed_model="nomic-embed-text",
        llm_model="mistral:7b",
        max_retries=1,
        base_delay=0.0,
    )
    embedder._client = mock_client  # type: ignore[attr-defined]
    return embedder


@pytest.fixture
def recorder(store: VectorStore, mock_embedder: Embedder) -> Iterator[DecisionRecorder]:
    """Yield a DecisionRecorder backed by temporary storage."""
    with tempfile.TemporaryDirectory() as decisions_dir:
        # Ensure decisions collection exists
        store.get_or_create_collection("decisions")
        yield DecisionRecorder(store=store, embedder=mock_embedder, decisions_dir=decisions_dir)


class TestDecisionRecording:
    """
    REQUIREMENT: User decisions are recorded and build the history signal over time.

    WHO: The scorer computing history_score on future runs
    WHAT: (1) The system persists a recorded yes verdict and includes it in the history signal for future scoring.
          (2) The system persists the reason alongside the recorded verdict so the operator's reasoning is preserved.
          (3) The system stores an empty string for the reason when no reason is provided so the field is always present.
          (4) The system sends the JD text plus the reason to the embedder so the embedding vector captures operator intent.
          (5) The system sends only the bare JD text to the embedder when no reason is provided.
          (6) The system stores a recorded no verdict for audit purposes but excludes it from the scoring history signal.
          (7) The system stores a recorded maybe verdict but excludes it from scoring so only clear yes signals contribute.
          (8) The system raises a DECISION error that names the job ID and tells the operator to check the latest output.
          (9) The system increases the history collection count by one for each recorded decision.
          (10) The system overwrites an existing decision for the same job ID instead of appending a duplicate.
          (11) The system raises a VALIDATION error that tells the operator to provide JD content when the JD text is empty.
          (12) The system writes the reason field to the daily JSONL audit file alongside the verdict.
          (13) The system returns None from get_decision() when the decisions collection is missing instead of raising an error.
          (14) The system returns None from get_decision() when the decisions collection exists but no matching document is found.
          (15) The system returns 0 from history_count() when the decisions collection is missing instead of raising an error.
          (16) The system returns an empty list from audit_decisions() when the decisions collection is missing instead of raising an error.
    WHY: If 'no' decisions contributed to scoring, roles similar to rejected ones
         would score lower — but the signal we want is 'what did I like',
         not 'what did I reject' (rejections have too many confounding reasons)

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O — the only I/O boundary)
        Real:  Embedder (embed, retry, truncation, metrics), DecisionRecorder,
               VectorStore (via tmpdir), JSONL file I/O
        Never: Patch Embedder methods, DecisionRecorder internals, or verdict
               classification logic
    """

    async def test_yes_verdict_is_stored_in_history_collection(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a recorder with an empty history
        WHEN a 'yes' verdict is recorded
        THEN the decision is persisted and becomes part of the history signal for future scoring.
        """
        # When: record a yes verdict
        await recorder.record(
            job_id="zr-123",
            verdict="yes",
            jd_text="Staff Platform Architect role at great company.",
            board="ziprecruiter",
            title="Staff Architect",
            company="Acme",
        )

        # Then: decision is retrievable with correct verdict and scoring flag
        decision = recorder.get_decision("zr-123")
        assert decision is not None, "Decision should be retrievable after recording"
        assert decision["verdict"] == "yes", "Verdict should be stored as 'yes'"
        assert decision["scoring_signal"] == "true", "Yes verdict should contribute to scoring"

    async def test_reason_is_stored_in_chromadb_metadata(self, recorder: DecisionRecorder) -> None:
        """
        GIVEN a verdict with an explicit reason
        WHEN the decision is recorded
        THEN the reason is persisted alongside the verdict so the operator's reasoning is preserved.
        """
        # When: record a verdict with a reason
        await recorder.record(
            job_id="zr-reason",
            verdict="no",
            jd_text="Interesting role but fully on-site.",
            board="ziprecruiter",
            title="SRE",
            company="OnSite Corp",
            reason="Requires on-site 5 days/week, no remote option",
        )

        # Then: reason is preserved in the decision metadata
        decision = recorder.get_decision("zr-reason")
        assert decision is not None, "Decision should be retrievable after recording"
        assert decision["reason"] == "Requires on-site 5 days/week, no remote option", (
            "Reason should be stored verbatim"
        )

    async def test_empty_reason_stored_when_not_provided(self, recorder: DecisionRecorder) -> None:
        """
        GIVEN a verdict with no reason provided
        WHEN the decision is recorded
        THEN an empty string is stored — the field is always present.
        """
        # When: record a verdict without a reason
        await recorder.record(
            job_id="zr-noreason",
            verdict="yes",
            jd_text="Great remote role.",
            board="ziprecruiter",
            title="Staff Architect",
            company="Remote Co",
        )

        # Then: reason field contains empty string
        decision = recorder.get_decision("zr-noreason")
        assert decision is not None, "Decision should be retrievable after recording"
        assert decision["reason"] == "", "Reason should default to empty string when not provided"

    async def test_reason_enriches_embedding_vector(
        self, recorder: DecisionRecorder, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a verdict with a reason
        WHEN the decision is recorded
        THEN the embedder receives JD text + reason so the vector captures operator intent.
        """
        # Given: a JD and an explicit reason
        jd = "Staff Platform Architect role at Acme Corp."
        reason = "Fully remote with architecture leadership"

        # When: record is called with both
        await recorder.record(
            job_id="zr-enrich",
            verdict="yes",
            jd_text=jd,
            board="ziprecruiter",
            reason=reason,
        )

        # Then: embedder receives enriched text
        embed_input = mock_embedder._client.embed.call_args.kwargs["input"]  # type: ignore[union-attr]
        expected = f"{jd}\n\nOperator reasoning: {reason}"
        assert embed_input == expected, f"Expected enriched text {expected!r}, got {embed_input!r}"

    async def test_empty_reason_does_not_enrich_embedding(
        self, recorder: DecisionRecorder, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a verdict with no reason
        WHEN the decision is recorded
        THEN the embedder receives only the bare JD text — no enrichment suffix.
        """
        # Given: a JD with no reason
        jd = "Staff Platform Architect role at Acme Corp."

        # When: record is called without a reason
        await recorder.record(
            job_id="zr-bare",
            verdict="yes",
            jd_text=jd,
            board="ziprecruiter",
        )

        # Then: embedder receives bare JD text only
        embed_input = mock_embedder._client.embed.call_args.kwargs["input"]  # type: ignore[union-attr]
        assert embed_input == jd, f"Expected bare JD text {jd!r}, got {embed_input!r}"

    async def test_no_verdict_is_stored_but_excluded_from_scoring_signal(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a 'no' verdict
        WHEN the decision is recorded
        THEN it is stored for audit but excluded from history_score — rejections have confounding reasons.
        """
        # When: record a 'no' verdict
        await recorder.record(
            job_id="zr-456",
            verdict="no",
            jd_text="SRE on-call role, not a fit.",
            board="ziprecruiter",
            title="SRE",
            company="Other Co",
        )

        # Then: decision stored with scoring_signal=false
        decision = recorder.get_decision("zr-456")
        assert decision is not None, "Decision should be retrievable after recording"
        assert decision["verdict"] == "no", "Verdict should be stored as 'no'"
        assert decision["scoring_signal"] == "false", "No verdict should be excluded from scoring"

    async def test_maybe_verdict_is_stored_but_excluded_from_scoring_signal(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a 'maybe' verdict
        WHEN the decision is recorded
        THEN it is stored but excluded from scoring — only clear 'yes' signals are useful.
        """
        # When: record a 'maybe' verdict
        await recorder.record(
            job_id="zr-789",
            verdict="maybe",
            jd_text="Interesting but unclear role.",
            board="ziprecruiter",
            title="Ambiguous Role",
            company="Maybe Co",
        )

        # Then: decision stored with scoring_signal=false
        decision = recorder.get_decision("zr-789")
        assert decision is not None, "Decision should be retrievable after recording"
        assert decision["verdict"] == "maybe", "Verdict should be stored as 'maybe'"
        assert decision["scoring_signal"] == "false", (
            "Maybe verdict should be excluded from scoring"
        )

    async def test_unknown_job_id_names_the_id_and_suggests_checking_latest_output(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN an invalid verdict value
        WHEN the decision is recorded
        THEN a DECISION error is raised naming the ID and suggesting the operator check the latest output.
        """
        # When/Then: invalid verdict raises ActionableError
        with pytest.raises(ActionableError) as exc_info:
            await recorder.record(
                job_id="zr-000",
                verdict="invalid_verdict",
                jd_text="Some text.",
                board="test",
            )

        # Then: error contains actionable guidance
        err = exc_info.value
        assert err.error_type == ErrorType.DECISION, "Error type should be DECISION"
        assert err.suggestion is not None, "Error should include a suggestion"
        assert err.troubleshooting is not None, "Error should include troubleshooting steps"

    async def test_history_collection_count_increases_after_each_decision(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN an empty history collection
        WHEN decisions are recorded
        THEN the history count increases by one for each decision.
        """
        # Given: initial count
        initial = recorder.history_count()

        # When: first decision recorded
        await recorder.record(
            job_id="job-a",
            verdict="yes",
            jd_text="First role description.",
            board="test",
        )

        # Then: count increased by 1
        assert recorder.history_count() == initial + 1, (
            "Count should increase by 1 after first decision"
        )

        # When: second decision recorded
        await recorder.record(
            job_id="job-b",
            verdict="no",
            jd_text="Second role description.",
            board="test",
        )

        # Then: count increased by 2 total
        assert recorder.history_count() == initial + 2, (
            "Count should increase by 2 after second decision"
        )

    async def test_duplicate_decision_on_same_job_id_overwrites_not_appends(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a decision already recorded for a job_id
        WHEN the same job_id is recorded again with a different verdict
        THEN the previous decision is overwritten rather than duplicated.
        """
        # Given: initial decision recorded
        await recorder.record(
            job_id="zr-dup",
            verdict="maybe",
            jd_text="A role I was unsure about.",
            board="ziprecruiter",
        )
        count_after_first = recorder.history_count()

        # When: same job_id recorded with different verdict
        await recorder.record(
            job_id="zr-dup",
            verdict="yes",
            jd_text="A role I was unsure about.",
            board="ziprecruiter",
        )

        # Then: count unchanged and verdict updated
        assert recorder.history_count() == count_after_first, (
            "Duplicate should overwrite, not append"
        )
        decision = recorder.get_decision("zr-dup")
        assert decision is not None, "Decision should still be retrievable"
        assert decision["verdict"] == "yes", "Verdict should be updated to 'yes'"

    async def test_empty_jd_text_tells_operator_to_provide_content(
        self, recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a verdict with whitespace-only JD text
        WHEN the decision is recorded
        THEN a VALIDATION error is raised telling the operator to provide content.
        """
        # When/Then: empty JD text raises ActionableError
        with pytest.raises(ActionableError) as exc_info:
            await recorder.record(
                job_id="zr-empty",
                verdict="yes",
                jd_text="   ",
                board="ziprecruiter",
            )

        # Then: error is VALIDATION with actionable guidance
        err = exc_info.value
        assert err.error_type == ErrorType.VALIDATION, "Error type should be VALIDATION"
        assert err.suggestion is not None, "Error should include a suggestion"
        assert err.troubleshooting is not None, "Error should include troubleshooting steps"

    async def test_reason_is_written_to_jsonl_audit_log(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a verdict with a reason
        WHEN the decision is recorded
        THEN the reason field appears in the daily JSONL audit file alongside the verdict.
        """
        with tempfile.TemporaryDirectory() as decisions_dir:
            # Given: a recorder with a fresh decisions directory
            store.get_or_create_collection("decisions")
            rec = DecisionRecorder(
                store=store, embedder=mock_embedder, decisions_dir=decisions_dir
            )

            # When: record a decision with a reason
            await rec.record(
                job_id="zr-audit",
                verdict="no",
                jd_text="On-site only role.",
                board="ziprecruiter",
                title="SRE",
                company="Corp",
                reason="No remote option",
            )

            # Then: JSONL file contains the reason
            jsonl_files = list(Path(decisions_dir).glob("*.jsonl"))
            assert len(jsonl_files) == 1, "Exactly one JSONL audit file should exist"
            records = [
                json_mod.loads(line) for line in jsonl_files[0].read_text().strip().splitlines()
            ]
            assert len(records) == 1, "JSONL file should contain exactly one record"
            assert records[0]["reason"] == "No remote option", (
                "Reason should be preserved in JSONL audit log"
            )

    def test_get_decision_returns_none_when_collection_missing(
        self, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a store with no decisions collection
        WHEN get_decision() is called
        THEN None is returned instead of raising.
        """
        # Given: a fresh store with no decisions collection
        with tempfile.TemporaryDirectory() as tmpdir:
            empty_store = VectorStore(persist_dir=tmpdir)
            recorder = DecisionRecorder(store=empty_store, embedder=mock_embedder)

            # When/Then: get_decision returns None gracefully
            assert recorder.get_decision("nonexistent-job") is None, (
                "Should return None when collection is missing"
            )

    def test_get_decision_returns_none_when_no_results_found(
        self, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a decisions collection that exists but has no matching document
        WHEN get_decision() is called
        THEN None is returned.
        """
        # Given: a store with an empty decisions collection
        with tempfile.TemporaryDirectory() as tmpdir:
            store = VectorStore(persist_dir=tmpdir)
            store.get_or_create_collection("decisions")
            recorder = DecisionRecorder(store=store, embedder=mock_embedder)

            # When/Then: get_decision returns None
            assert recorder.get_decision("unknown-id") is None, (
                "Should return None when no matching document exists"
            )

    def test_history_count_returns_zero_when_collection_missing(
        self, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a store where the decisions collection does not exist
        WHEN history_count() is called
        THEN 0 is returned instead of raising.
        """
        # Given: a fresh store with no decisions collection
        with tempfile.TemporaryDirectory() as tmpdir:
            empty_store = VectorStore(persist_dir=tmpdir)
            recorder = DecisionRecorder(store=empty_store, embedder=mock_embedder)

            # When/Then: history_count returns 0 gracefully
            assert recorder.history_count() == 0, "Should return 0 when collection is missing"

    def test_audit_decisions_returns_empty_list_when_collection_missing(
        self, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a store where the decisions collection does not exist
        WHEN audit_decisions() is called
        THEN an empty list is returned instead of raising.
        """
        # Given: a fresh store with no decisions collection
        with tempfile.TemporaryDirectory() as tmpdir:
            empty_store = VectorStore(persist_dir=tmpdir)
            recorder = DecisionRecorder(store=empty_store, embedder=mock_embedder)

            # When: audit_decisions is called
            results = recorder.audit_decisions()

            # Then: empty list returned gracefully
            assert results == [], (
                f"Should return empty list when collection is missing, got: {results}"
            )
