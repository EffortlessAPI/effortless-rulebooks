"""
Interactive review tests — batch decision recording on undecided listings.

Spec classes:
    TestInteractiveReview — ranked review workflow, verdict recording, progress
    TestListingDisplayDisqualified — disqualification warning in review display
"""

from __future__ import annotations

from typing import TYPE_CHECKING
from unittest.mock import patch

import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.pipeline.ranker import RankedListing
from jobsearch_rag.pipeline.review import ReviewSession
from jobsearch_rag.rag.scorer import ScoreResult
from tests.constants import EMBED_FAKE

if TYPE_CHECKING:
    from jobsearch_rag.rag.decisions import DecisionRecorder
    from jobsearch_rag.rag.store import VectorStore

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _make_ranked(
    *,
    title: str = "Solutions Architect",
    company: str = "Acme Corp",
    board: str = "ziprecruiter",
    external_id: str = "zr-1",
    final_score: float = 0.82,
    fit: float = 0.85,
    archetype: float = 0.90,
    history: float = 0.50,
    comp: float = 0.60,
    comp_min: float | None = 180_000,
    comp_max: float | None = 250_000,
    url: str = "https://www.ziprecruiter.com/jobs/test-1",
    disqualified: bool = False,
    disqualifier_reason: str | None = None,
) -> RankedListing:
    listing = JobListing(
        board=board,
        external_id=external_id,
        title=title,
        company=company,
        location="Remote",
        url=url,
        full_text="Full JD text for testing." * 10,
        comp_min=comp_min,
        comp_max=comp_max,
    )
    scores = ScoreResult(
        fit_score=fit,
        archetype_score=archetype,
        history_score=history,
        disqualified=disqualified,
        disqualifier_reason=disqualifier_reason,
        comp_score=comp,
    )
    return RankedListing(listing=listing, scores=scores, final_score=final_score)


def _seed_decisions(
    store: VectorStore,
    decided_ids: set[str],
) -> None:
    """Pre-populate the decisions collection so ``get_decision`` finds them."""
    store.get_or_create_collection("decisions")
    for jid in decided_ids:
        store.add_documents(
            collection_name="decisions",
            ids=[f"decision-{jid}"],
            documents=["Seeded JD text."],
            embeddings=[EMBED_FAKE],
            metadatas=[
                {
                    "job_id": jid,
                    "verdict": "yes",
                    "board": "test",
                    "title": "Seeded",
                    "company": "Test Corp",
                    "scoring_signal": "true",
                    "reason": "",
                    "recorded_at": "2024-01-01T00:00:00+00:00",
                }
            ],
        )


class TestInteractiveReview:
    """
    REQUIREMENT: The operator can review and decide on all undecided
    listings in a single interactive session.

    WHO: The operator reviewing search results after a pipeline run
    WHAT: (1) The review session orders undecided listings by descending score when it loads the latest results.
          (2) The system returns only listings without prior decisions when it retrieves undecided listings.
          (3) The system includes the listing's rank, title, company, and score when it formats a listing.
          (4) The system shows every component score in the listing output when it formats a ranked listing.
          (5) The system displays the compensation range in the listing output when compensation data is available.
          (6) The system persists a recorded 'y' verdict as a yes decision.
          (7) The system persists the provided reason together with a recorded no verdict.
          (8) The system stores an empty string as the reason when it records a verdict without a reason.
          (9) The system persists a recorded 'n' verdict as a no decision.
          (10) The system persists a recorded 'm' verdict as a maybe decision.
          (11) The system skips recording a verdict and leaves the listing undecided when the operator enters 's'.
          (12) The system launches the JD file in the default system viewer when the operator invokes 'o'.
          (13) The system preserves all previously recorded verdicts when the operator quits mid-review.
          (14) The system shows the current position and total count as '[3/28]' in the progress indicator.
          (15) The system returns an empty list when every listing already has a decision.
          (16) The system returns an empty list when no ranked listings exist.
    WHY: Without batch review, the operator must invoke individual CLI
         commands per listing, making the review workflow impractical
         for large result sets

    MOCK BOUNDARY:
        Mock: webbrowser.open (I/O boundary, 1 test)
        Real: ReviewSession, DecisionRecorder, VectorStore (via conftest tmpdir)
        Never: Patch ReviewSession internals or DecisionRecorder
    """

    def test_review_loads_latest_results_in_ranked_order(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN listings with different scores
        WHEN a ReviewSession is created
        THEN undecided listings are in descending score order.
        """
        # Given: three listings with different scores
        high = _make_ranked(title="High Score", final_score=0.95, external_id="h1")
        low = _make_ranked(title="Low Score", final_score=0.55, external_id="l1")
        mid = _make_ranked(title="Mid Score", final_score=0.75, external_id="m1")

        # When: create session and get undecided
        session = ReviewSession(
            ranked_listings=[low, high, mid],
            recorder=decision_recorder,
        )
        undecided = session.undecided_listings()

        # Then: ordered descending by score
        assert [r.listing.title for r in undecided] == [
            "High Score",
            "Mid Score",
            "Low Score",
        ], "Listings should be in descending score order"

    def test_already_decided_listings_are_excluded(
        self,
        vector_store: VectorStore,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN listings where some already have decisions
        WHEN undecided_listings() is called
        THEN only undecided listings are returned.
        """
        # Given: one decided, one undecided
        r1 = _make_ranked(external_id="decided-1")
        r2 = _make_ranked(external_id="undecided-1")
        _seed_decisions(vector_store, decided_ids={"decided-1"})

        # When: get undecided
        session = ReviewSession(ranked_listings=[r1, r2], recorder=decision_recorder)
        undecided = session.undecided_listings()

        # Then: only undecided listing remains
        assert len(undecided) == 1, "Should return only undecided listings"
        assert undecided[0].listing.external_id == "undecided-1", "Should be the undecided listing"

    def test_listing_display_shows_rank_title_company_score(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a ranked listing
        WHEN format_listing() is called
        THEN the output includes rank, title, company, and score.
        """
        # Given: a ranked listing
        ranked = _make_ranked(title="Staff Architect", company="TechCo", final_score=0.88)
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: format the listing
        output = session.format_listing(ranked, rank=1, total=5)

        # Then: essential context is present
        assert "1" in output, "Should include rank"
        assert "Staff Architect" in output, "Should include title"
        assert "TechCo" in output, "Should include company"
        assert "0.88" in output, "Should include score"

    def test_listing_display_shows_component_score_breakdown(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a ranked listing with individual component scores
        WHEN format_listing() is called
        THEN all component scores appear in the output.
        """
        # Given: listing with specific component scores
        ranked = _make_ranked(fit=0.85, archetype=0.90, history=0.50, comp=0.60)
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: format the listing
        output = session.format_listing(ranked, rank=1, total=1)

        # Then: all component scores present
        assert "0.85" in output, "Should include fit score"
        assert "0.90" in output, "Should include archetype score"
        assert "0.50" in output, "Should include history score"
        assert "0.60" in output, "Should include comp score"

    def test_listing_display_shows_comp_range_when_available(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a listing with compensation data
        WHEN format_listing() is called
        THEN the compensation range appears in the output.
        """
        # Given: listing with comp range
        ranked = _make_ranked(comp_min=180_000, comp_max=250_000)
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: format the listing
        output = session.format_listing(ranked, rank=1, total=1)

        # Then: comp range visible
        assert "180" in output, "Should include comp_min"
        assert "250" in output, "Should include comp_max"

    @pytest.mark.asyncio
    async def test_yes_verdict_records_via_decision_recorder(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session with a listing
        WHEN 'y' verdict is recorded
        THEN the decision is persisted with verdict='yes'.
        """
        # Given: session with one listing
        ranked = _make_ranked(external_id="job-1")
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: record 'y' verdict
        await session.record_verdict(ranked, "y")

        # Then: decision persisted
        decision = decision_recorder.get_decision("job-1")
        assert decision is not None, "Decision should be persisted"
        assert decision["verdict"] == "yes", "Verdict should be 'yes'"
        assert decision["job_id"] == "job-1", "Job ID should match"

    @pytest.mark.asyncio
    async def test_verdict_with_reason_passes_reason_to_recorder(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session with a listing
        WHEN a 'no' verdict with a reason is recorded
        THEN the reason is persisted alongside the verdict.
        """
        # Given: session with one listing
        ranked = _make_ranked(external_id="job-reason")
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: record 'n' with reason
        await session.record_verdict(ranked, "n", reason="Requires 5 years Kubernetes experience")

        # Then: verdict and reason persisted
        decision = decision_recorder.get_decision("job-reason")
        assert decision is not None, "Decision should be persisted"
        assert decision["verdict"] == "no", "Verdict should be 'no'"
        assert decision["reason"] == "Requires 5 years Kubernetes experience", (
            "Reason should match"
        )

    @pytest.mark.asyncio
    async def test_verdict_without_reason_passes_empty_string(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session with a listing
        WHEN a verdict is recorded without a reason
        THEN an empty string is stored as the reason.
        """
        # Given: session with one listing
        ranked = _make_ranked(external_id="job-noreason")
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: record 'y' without reason
        await session.record_verdict(ranked, "y")

        # Then: reason is empty string
        decision = decision_recorder.get_decision("job-noreason")
        assert decision is not None, "Decision should be persisted"
        assert decision["reason"] == "", "Reason should be empty string when not provided"

    @pytest.mark.asyncio
    async def test_no_verdict_records_via_decision_recorder(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session with a listing
        WHEN 'n' verdict is recorded
        THEN the decision is persisted with verdict='no'.
        """
        # Given: session with one listing
        ranked = _make_ranked(external_id="job-2")
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: record 'n' verdict
        await session.record_verdict(ranked, "n")

        # Then: 'no' decision persisted
        decision = decision_recorder.get_decision("job-2")
        assert decision is not None, "Decision should be persisted"
        assert decision["verdict"] == "no", "Verdict should be 'no'"

    @pytest.mark.asyncio
    async def test_maybe_verdict_records_via_decision_recorder(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session with a listing
        WHEN 'm' verdict is recorded
        THEN the decision is persisted with verdict='maybe'.
        """
        # Given: session with one listing
        ranked = _make_ranked(external_id="job-3")
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: record 'm' verdict
        await session.record_verdict(ranked, "m")

        # Then: 'maybe' decision persisted
        decision = decision_recorder.get_decision("job-3")
        assert decision is not None, "Decision should be persisted"
        assert decision["verdict"] == "maybe", "Verdict should be 'maybe'"

    def test_skip_advances_without_recording(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session
        WHEN the operator enters 's' (skip)
        THEN no verdict is recorded and the listing remains undecided.
        """
        # Given: session with one listing
        ranked = _make_ranked(external_id="skip-me")
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: check if 's' should record
        result = session.should_record("s")

        # Then: skip does not record
        assert result is False, "'s' should not trigger recording"

    @pytest.mark.asyncio
    async def test_open_launches_jd_file_in_system_viewer(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a listing with a JD file
        WHEN 'o' (open) is invoked
        THEN the system default viewer is launched via webbrowser.open.
        """
        # Given: session with jd_dir configured
        ranked = _make_ranked(external_id="open-me")
        session = ReviewSession(
            ranked_listings=[ranked],
            recorder=decision_recorder,
            jd_dir="output/jds",
        )

        # When: open listing
        with patch("jobsearch_rag.pipeline.review.webbrowser.open") as mock_open:
            session.open_listing(ranked)

            # Then: webbrowser.open called
            mock_open.assert_called_once()

    @pytest.mark.asyncio
    async def test_quit_preserves_all_previously_recorded_verdicts(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN verdicts recorded during a review session
        WHEN the operator quits mid-review
        THEN all previously recorded verdicts are preserved.
        """
        # Given: session with three listings
        r1 = _make_ranked(external_id="keep-1")
        r2 = _make_ranked(external_id="keep-2")
        r3 = _make_ranked(external_id="quit-before")
        session = ReviewSession(ranked_listings=[r1, r2, r3], recorder=decision_recorder)

        # When: record two verdicts then simulate quit
        await session.record_verdict(r1, "y")
        await session.record_verdict(r2, "n")
        # Don't record r3 — simulates quitting

        # Then: first two verdicts persist, third is absent
        assert decision_recorder.get_decision("keep-1") is not None, "First verdict should persist"
        assert decision_recorder.get_decision("keep-2") is not None, (
            "Second verdict should persist"
        )
        assert decision_recorder.get_decision("quit-before") is None, (
            "Unrecorded listing should be absent"
        )

    def test_progress_indicator_shows_current_position_and_total(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a review session with 28 listings
        WHEN format_progress(current=3, total=28) is called
        THEN the output contains '[3/28]'.
        """
        # Given: session with 28 listings
        session = ReviewSession(
            ranked_listings=[_make_ranked(external_id=f"p{i}") for i in range(28)],
            recorder=decision_recorder,
        )

        # When: format progress at position 3
        progress = session.format_progress(current=3, total=28)

        # Then: progress indicator present
        assert "[3/28]" in progress, "Should show current/total progress"

    def test_no_undecided_listings_prints_message_and_exits(
        self,
        vector_store: VectorStore,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN all listings already have decisions
        WHEN undecided_listings() is called
        THEN an empty list is returned.
        """
        # Given: all listings decided
        r1 = _make_ranked(external_id="done-1")
        _seed_decisions(vector_store, decided_ids={"done-1"})
        session = ReviewSession(ranked_listings=[r1], recorder=decision_recorder)

        # When: get undecided
        undecided = session.undecided_listings()

        # Then: empty list
        assert len(undecided) == 0, "Should have no undecided listings"

    def test_no_results_file_prints_message_and_exits(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN no ranked listings exist
        WHEN undecided_listings() is called
        THEN an empty list is returned.
        """
        # Given: empty session
        session = ReviewSession(ranked_listings=[], recorder=decision_recorder)

        # When: get undecided
        undecided = session.undecided_listings()

        # Then: empty
        assert len(undecided) == 0, "Should have no listings"


# ---------------------------------------------------------------------------
# TestListingDisplayDisqualified
# ---------------------------------------------------------------------------


class TestListingDisplayDisqualified:
    """
    REQUIREMENT: Disqualified listings show a visible warning in the
    review display.

    WHO: The operator reviewing a listing that was auto-disqualified
    WHAT: (1) The system includes a '⚠ DISQUALIFIED' warning with the reason in the formatted listing output for a disqualified listing.
    WHY: Without a visible warning, the operator may unknowingly spend
         time evaluating a role that was already flagged as unsuitable

    MOCK BOUNDARY:
        Mock: nothing — pure display formatting
        Real: ReviewSession.format_listing, RankedListing construction
        Never: Patch format internals
    """

    def test_disqualified_listing_shows_warning_in_display(
        self,
        decision_recorder: DecisionRecorder,
    ) -> None:
        """
        GIVEN a listing with disqualified=True and a reason
        WHEN format_listing() is called
        THEN the output includes a '⚠ DISQUALIFIED' warning with the reason.
        """
        # Given: disqualified listing
        ranked = _make_ranked(
            disqualified=True,
            disqualifier_reason="Requires active security clearance",
        )
        session = ReviewSession(ranked_listings=[ranked], recorder=decision_recorder)

        # When: format the listing
        output = session.format_listing(ranked, rank=1, total=1)

        # Then: warning and reason present
        assert "⚠ DISQUALIFIED" in output, "Should show disqualification warning"
        assert "Requires active security clearance" in output, "Should show reason"
