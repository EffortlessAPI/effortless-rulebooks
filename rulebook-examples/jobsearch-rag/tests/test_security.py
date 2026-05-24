"""
Input validation and security boundary tests.

Spec classes:
    TestJobListingValidation — construction rejects malformed input at the
                               adapter boundary where untrusted web content
                               enters the system
    TestDecisionAudit        — operator can inspect, audit, and surgically
                               remove individual decisions without losing
                               the append-only audit trail
"""

from __future__ import annotations

import json
import tempfile
from pathlib import Path
from typing import TYPE_CHECKING

import pytest

from jobsearch_rag.adapters.base import JobListing

if TYPE_CHECKING:
    from collections.abc import Iterator

from jobsearch_rag.rag.decisions import DecisionRecorder
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.store import VectorStore
from tests.conftest import make_mock_ollama_client

# Public API surface (from src/jobsearch_rag/adapters/base):
#   JobListing(board, external_id, title, company, location, url, full_text,
#              posted_at=None, raw_html=None, comp_min=None, comp_max=None,
#              comp_source=None, comp_text=None, metadata={})
#   _sanitize_filename_field(value: str) -> str   (module-level helper)
#
# Public API surface (from src/jobsearch_rag/cli — Phase 6d):
#   handle_decisions_show(args: argparse.Namespace) -> None
#   handle_decisions_remove(args: argparse.Namespace) -> None
#   handle_decisions_audit(args: argparse.Namespace) -> None
#
# Public API surface (from src/jobsearch_rag/rag/store):
#   VectorStore.delete_by_id(collection_name: str, *, ids: list[str]) -> None
#
# Public API surface (from src/jobsearch_rag/rag/decisions):
#   DecisionRecorder.record(job_id, verdict, jd_text, board, ...) -> None
#   DecisionRecorder.get_decision(job_id) -> dict | None
#   DecisionRecorder.audit_decisions() -> list[dict[str, str]]
#   DecisionRecorder.remove_decision(job_id) -> bool


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

_REQUIRED = {
    "board": "test-board",
    "external_id": "sec-001",
    "title": "Staff Platform Architect",
    "company": "Acme Corp",
    "location": "Remote (USA)",
    "url": "https://example.org/job/sec-001",
    "full_text": "A normal job description.",
}


def _make(**overrides: object) -> JobListing:
    """Build a JobListing with sensible defaults; override any field."""
    fields = {**_REQUIRED, **overrides}
    return JobListing(**fields)  # type: ignore[arg-type]


# ---------------------------------------------------------------------------
# TestJobListingValidation
# ---------------------------------------------------------------------------


class TestJobListingValidation:
    """
    REQUIREMENT: JobListing construction rejects malformed input at the
    boundary where untrusted web content enters the system.

    WHO: The scoring pipeline; the exporter constructing output filenames
    WHAT: (1) a listing whose full_text exceeds 250,000 characters is rejected with a ValueError
          (2) full_text at exactly 250,000 characters constructs without error
          (3) path traversal sequences in title are replaced, leaving no path separators
          (4) path traversal sequences in company are replaced, leaving no path separators
          (5) filesystem-unsafe characters (< > : " | ? *) are stripped from title
          (6) well-formed content constructs without error and all fields are accessible
          (7) sanitisation does not affect optional fields, which remain at their defaults
    WHY: An oversized JD causes an Ollama out-of-memory crash with no
         actionable error. A title containing '../' can write files outside
         the output directory — a path traversal vulnerability even in a
         local single-user context

    MOCK BOUNDARY:
        Mock:  nothing — JobListing is a data class with validation in __init__
        Real:  JobListing constructor called with explicit field values
        Never: Bypass the constructor; test all validation paths through
               JobListing(...) with adversarial input strings
    """

    def test_full_text_exceeding_250k_chars_raises_value_error(self) -> None:
        """
        GIVEN a JobListing constructed with full_text of 250,001 characters
        WHEN the constructor executes
        THEN a ValueError is raised.
        """
        # Given: oversized full_text
        oversized = "x" * 250_001

        # Then: construction raises ValueError
        with pytest.raises(ValueError, match="full_text exceeds maximum length"):
            _make(full_text=oversized)

    def test_full_text_at_250k_chars_constructs_without_error(self) -> None:
        """
        GIVEN a JobListing constructed with full_text of exactly 250,000 characters
        WHEN the constructor executes
        THEN no error is raised.
        """
        # Given: exactly at the boundary
        at_limit = "x" * 250_000

        # When: construction succeeds
        listing = _make(full_text=at_limit)

        # Then: full_text is preserved
        assert len(listing.full_text) == 250_000, (
            f"Expected full_text length 250,000, got {len(listing.full_text)}"
        )

    def test_path_traversal_in_title_is_removed(self) -> None:
        r"""
        GIVEN a JobListing constructed with title containing '../'
        WHEN the listing is inspected
        THEN the title field contains no '/' or '\' characters.
        """
        # Given: path traversal in title
        listing = _make(title="../../etc/passwd Engineer")

        # Then: no path separators remain
        assert "/" not in listing.title, "title must not contain '/'"
        assert "\\" not in listing.title, "title must not contain '\\'"

    def test_path_traversal_in_company_name_is_removed(self) -> None:
        """
        GIVEN a JobListing constructed with company containing '../../etc'
        WHEN the listing is inspected
        THEN the company field contains no path separator characters.
        """
        # Given: path traversal in company
        listing = _make(company="../../etc/shadow Corp")

        # Then: no path separators remain
        assert "/" not in listing.company, "company must not contain '/'"
        assert "\\" not in listing.company, "company must not contain '\\'"

    def test_filesystem_unsafe_characters_are_stripped_from_title(self) -> None:
        """
        GIVEN a title containing characters from the set < > : " | ? *
        WHEN the listing is constructed
        THEN the title field contains none of those characters.
        """
        # Given: filesystem-unsafe characters in title
        listing = _make(title='Staff <Eng> "Platform" | Arch? *Senior*')

        # Then: none of the unsafe chars remain
        unsafe = set('<>:"|?*')
        remaining = unsafe & set(listing.title)
        assert not remaining, f"unsafe characters remain: {remaining}"

    def test_well_formed_listing_constructs_without_error(self) -> None:
        """
        GIVEN all required fields with normal content
        WHEN a JobListing is constructed
        THEN no error is raised and all fields are accessible.
        """
        # When: normal construction
        listing = _make()

        # Then: all required fields accessible
        assert listing.board == "test-board", f"Expected board 'test-board', got {listing.board!r}"
        assert listing.external_id == "sec-001", (
            f"Expected external_id 'sec-001', got {listing.external_id!r}"
        )
        assert listing.title == "Staff Platform Architect", (
            f"Expected title 'Staff Platform Architect', got {listing.title!r}"
        )
        assert listing.company == "Acme Corp", (
            f"Expected company 'Acme Corp', got {listing.company!r}"
        )
        assert listing.location == "Remote (USA)", (
            f"Expected location 'Remote (USA)', got {listing.location!r}"
        )
        assert listing.url == "https://example.org/job/sec-001", (
            f"Expected url 'https://example.org/job/sec-001', got {listing.url!r}"
        )
        assert listing.full_text == "A normal job description.", (
            f"Expected full_text 'A normal job description.', got {listing.full_text!r}"
        )

    def test_sanitisation_does_not_affect_optional_fields(self) -> None:
        """
        GIVEN a listing with None for optional fields
        WHEN the listing is constructed
        THEN optional fields remain None without error.
        """
        # When: listing with defaults for optional fields
        listing = _make()

        # Then: optional fields are their defaults
        assert listing.posted_at is None, f"Expected posted_at=None, got {listing.posted_at!r}"
        assert listing.raw_html is None, f"Expected raw_html=None, got {listing.raw_html!r}"
        assert listing.comp_min is None, f"Expected comp_min=None, got {listing.comp_min!r}"
        assert listing.comp_max is None, f"Expected comp_max=None, got {listing.comp_max!r}"
        assert listing.comp_source is None, (
            f"Expected comp_source=None, got {listing.comp_source!r}"
        )
        assert listing.comp_text is None, f"Expected comp_text=None, got {listing.comp_text!r}"
        assert listing.metadata == {}, f"Expected metadata={{}}, got {listing.metadata!r}"


# ---------------------------------------------------------------------------
# Decision audit fixtures
# ---------------------------------------------------------------------------

EMBED_TEST = [0.5, 0.5, 0.5, 0.5, 0.5]


@pytest.fixture
def _store() -> Iterator[VectorStore]:  # pyright: ignore[reportUnusedFunction]
    """Yield a temporary VectorStore for test isolation."""
    with tempfile.TemporaryDirectory() as tmpdir:
        yield VectorStore(persist_dir=tmpdir)


@pytest.fixture
def _mock_embedder() -> object:  # pyright: ignore[reportUnusedFunction]
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
def _decisions_dir(tmp_path: object) -> object:  # pyright: ignore[reportUnusedFunction]
    """Return a temporary decisions directory path."""
    d = Path(str(tmp_path)) / "decisions"
    d.mkdir(parents=True, exist_ok=True)
    return d


@pytest.fixture
def _recorder(  # pyright: ignore[reportUnusedFunction]
    _store: VectorStore,
    _mock_embedder: object,
    _decisions_dir: object,
) -> DecisionRecorder:
    """Yield a DecisionRecorder backed by temporary storage."""
    _store.get_or_create_collection("decisions")
    return DecisionRecorder(store=_store, embedder=_mock_embedder, decisions_dir=_decisions_dir)  # type: ignore[arg-type]


async def _record_decision(
    recorder: DecisionRecorder,
    *,
    job_id: str,
    verdict: str = "yes",
    reason: str = "",
    board: str = "test-board",
    title: str = "Staff Architect",
    company: str = "Acme Corp",
) -> None:
    """Helper to record a decision with sensible defaults."""
    await recorder.record(
        job_id=job_id,
        verdict=verdict,
        jd_text=f"Job description for {job_id}",
        board=board,
        title=title,
        company=company,
        reason=reason,
    )


# ---------------------------------------------------------------------------
# TestDecisionAudit
# ---------------------------------------------------------------------------


class TestDecisionAudit:
    """
    REQUIREMENT: The operator can inspect, audit, and surgically remove
    individual entries from the decisions collection without losing the
    append-only audit trail.

    WHO: The operator who suspects a poisoned or erroneous decision is
         skewing future scoring
    WHAT: (1) `decisions audit` lists all recorded decisions that have a non-empty reason field with their job_id, verdict, and reason.
          (2) The audit output includes job_id, verdict, and reason for each listed decision.
          (3) The system prints an advisory when no decisions have reasons to audit.
          (4) `decisions show <job_id>` prints metadata for the specified decision.
          (5) `decisions remove <job_id>` deletes the entry from the ChromaDB collection.
          (6) `decisions remove` preserves existing JSONL entries (append-only).
          (7) After removal, `decisions audit` no longer lists the removed entry.
          (8) `decisions remove` with a nonexistent job_id prints a clear message rather than silently succeeding.
          (9) `decisions remove` appends a verdict: "removed" entry to the JSONL audit log for full replay.
    WHY: The decisions collection is the system's persistent memory. Any
         poisoned entry influences all future scoring until removed.
         The JSONL audit log must remain unmodified as the forensic record

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O via conftest make_mock_ollama_client)
        Real:  DecisionRecorder, Embedder, ChromaDB via vector_store fixture,
               JSONL audit log in tmp_path via output directory guard
        Never: Insert or delete ChromaDB documents directly; always use
               DecisionRecorder.record() to add decisions and verify removal
               through DecisionRecorder.get_decision() returning None
    """

    @pytest.mark.asyncio
    async def test_audit_lists_decisions_with_non_empty_reasons(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN two recorded decisions, one with a reason and one without
        WHEN audit_decisions() is called
        THEN only the decision with a reason appears in the result.
        """
        # Given: two decisions — one with reason, one without
        await _record_decision(
            _recorder, job_id="zr_001", verdict="yes", reason="Great culture fit"
        )
        await _record_decision(_recorder, job_id="zr_002", verdict="no", reason="")

        # When: audit decisions
        results = _recorder.audit_decisions()

        # Then: only the one with a reason appears
        job_ids = [d["job_id"] for d in results]
        assert "zr_001" in job_ids, "decision with reason must appear in audit"
        assert "zr_002" not in job_ids, "decision without reason must not appear in audit"

    @pytest.mark.asyncio
    async def test_audit_output_includes_job_id_verdict_and_reason(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a recorded decision with a reason
        WHEN audit_decisions() is called
        THEN the result entry contains job_id, verdict, and reason.
        """
        # Given: a decision with a reason
        await _record_decision(_recorder, job_id="zr_100", verdict="yes", reason="Strong match")

        # When: audit decisions
        results = _recorder.audit_decisions()

        # Then: the entry has all three fields
        assert len(results) >= 1, "audit must return at least one result"
        entry = next(d for d in results if d["job_id"] == "zr_100")
        assert entry["verdict"] == "yes", "verdict must be present"
        assert entry["reason"] == "Strong match", "reason must be present"

    @pytest.mark.asyncio
    async def test_audit_with_no_decisions_having_reasons_prints_advisory(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN decisions exist but none have reasons
        WHEN audit_decisions() is called
        THEN the result is an empty list.
        """
        # Given: decisions with no reasons
        await _record_decision(_recorder, job_id="zr_200", verdict="yes", reason="")
        await _record_decision(_recorder, job_id="zr_201", verdict="no", reason="")

        # When: audit decisions
        results = _recorder.audit_decisions()

        # Then: no results — caller renders advisory
        assert results == [], "audit must return empty list when no decisions have reasons"

    @pytest.mark.asyncio
    async def test_show_prints_metadata_for_the_given_job_id(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a recorded decision for job_id 'zr_12345'
        WHEN get_decision('zr_12345') is called
        THEN the result includes the verdict and recorded_at timestamp.
        """
        # Given: a recorded decision
        await _record_decision(_recorder, job_id="zr_12345", verdict="yes", reason="Good fit")

        # When: show the decision
        result = _recorder.get_decision("zr_12345")

        # Then: metadata is present
        assert result is not None, "decision must be found"
        assert result["verdict"] == "yes", "verdict must be present"
        assert "recorded_at" in result, "timestamp must be present"

    @pytest.mark.asyncio
    async def test_remove_deletes_entry_from_chroma_collection(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a recorded decision for job_id 'zr_12345'
        WHEN remove_decision('zr_12345') is called
        THEN get_decision('zr_12345') returns None.
        """
        # Given: a recorded decision
        await _record_decision(_recorder, job_id="zr_12345", verdict="yes", reason="Good fit")

        # When: remove the decision
        removed = _recorder.remove_decision("zr_12345")

        # Then: the decision is gone
        assert removed is True, "remove_decision must return True for existing entries"
        assert _recorder.get_decision("zr_12345") is None, "decision must be gone after removal"

    @pytest.mark.asyncio
    async def test_remove_preserves_original_jsonl_and_appends_removed_entry(
        self, _recorder: DecisionRecorder, _decisions_dir: object
    ) -> None:
        """
        GIVEN a recorded decision that was also written to the JSONL audit log
        WHEN remove_decision is called for that job_id
        THEN the JSONL audit log still contains the original entry
        AND a new entry with verdict "removed" is appended.
        """
        # Given: a recorded decision (which writes to JSONL)
        await _record_decision(_recorder, job_id="zr_12345", verdict="yes", reason="Good fit")

        # Verify JSONL exists with the entry
        decisions_path = Path(str(_decisions_dir))
        jsonl_files = list(decisions_path.glob("*.jsonl"))
        assert len(jsonl_files) >= 1, "JSONL file must exist after recording"
        original_lines = jsonl_files[0].read_text().strip().splitlines()
        assert any("zr_12345" in line for line in original_lines), (
            "JSONL must contain the recorded decision"
        )
        original_count = len(original_lines)

        # When: remove the decision from ChromaDB
        _recorder.remove_decision("zr_12345")

        # Then: original entry is preserved and a "removed" entry is appended
        after_lines = jsonl_files[0].read_text().strip().splitlines()
        assert len(after_lines) == original_count + 1, (
            "JSONL must have one additional line after removal"
        )
        # Original lines are unchanged
        for i, orig in enumerate(original_lines):
            assert after_lines[i] == orig, f"original JSONL line {i} must be unchanged"
        # New line is a "removed" entry
        removed_entry = json.loads(after_lines[-1])
        assert removed_entry["job_id"] == "zr_12345", "removed entry must have correct job_id"
        assert removed_entry["verdict"] == "removed", "removed entry must have verdict 'removed'"

    @pytest.mark.asyncio
    async def test_remove_followed_by_audit_no_longer_lists_removed_entry(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN a recorded decision with a reason
        WHEN remove_decision is called for that job_id
        AND audit_decisions() is called afterwards
        THEN the removed entry does not appear in audit output.
        """
        # Given: a decision with a reason
        await _record_decision(_recorder, job_id="zr_777", verdict="yes", reason="Culture match")

        # When: remove and then audit
        _recorder.remove_decision("zr_777")
        results = _recorder.audit_decisions()

        # Then: removed entry is not in audit
        job_ids = [d["job_id"] for d in results]
        assert "zr_777" not in job_ids, "removed decision must not appear in audit"

    @pytest.mark.asyncio
    async def test_remove_nonexistent_job_id_returns_false(
        self, _recorder: DecisionRecorder
    ) -> None:
        """
        GIVEN no decision recorded for job_id 'zr_99999'
        WHEN remove_decision('zr_99999') is called
        THEN it returns False without raising an exception.
        """
        # When: remove a nonexistent decision
        removed = _recorder.remove_decision("zr_99999")

        # Then: returns False, no exception
        assert removed is False, "remove_decision must return False for nonexistent entries"

    @pytest.mark.asyncio
    async def test_remove_jsonl_entry_contains_job_id_verdict_and_timestamp(
        self, _recorder: DecisionRecorder, _decisions_dir: object
    ) -> None:
        """
        GIVEN a recorded decision for job_id 'zr_12345'
        WHEN remove_decision('zr_12345') is called
        THEN the appended JSONL entry contains job_id 'zr_12345',
        verdict 'removed', and a recorded_at timestamp.
        """
        # Given: a recorded decision
        await _record_decision(_recorder, job_id="zr_12345", verdict="yes", reason="Good fit")

        # When: remove the decision
        _recorder.remove_decision("zr_12345")

        # Then: the appended JSONL entry has the right fields
        decisions_path = Path(str(_decisions_dir))
        jsonl_files = list(decisions_path.glob("*.jsonl"))
        assert len(jsonl_files) >= 1, "JSONL file must exist"
        lines = jsonl_files[0].read_text().strip().splitlines()
        removed_entry = json.loads(lines[-1])
        assert removed_entry["job_id"] == "zr_12345", "job_id must match"
        assert removed_entry["verdict"] == "removed", "verdict must be 'removed'"
        assert "recorded_at" in removed_entry, "recorded_at timestamp must be present"
