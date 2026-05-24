"""
Interactive review session — batch decision recording.

Provides :class:`ReviewSession` which loads ranked listings, filters
out already-decided jobs, and exposes methods for formatting display,
recording verdicts, and tracking progress.  The actual input loop
lives in the CLI handler; this module owns the domain logic.
"""

from __future__ import annotations

import webbrowser
from pathlib import Path
from typing import TYPE_CHECKING

from jobsearch_rag.text import slugify

if TYPE_CHECKING:
    from jobsearch_rag.pipeline.ranker import RankedListing
    from jobsearch_rag.rag.decisions import DecisionRecorder

# Verdict key → stored value
_VERDICT_MAP = {"y": "yes", "n": "no", "m": "maybe"}


class ReviewSession:
    """
    Manages the state and logic for an interactive review pass.

    Constructed with ranked listings and a :class:`DecisionRecorder`.
    The CLI feeds user input into :meth:`record_verdict`,
    :meth:`should_record`, and :meth:`open_listing`.
    """

    def __init__(
        self,
        ranked_listings: list[RankedListing],
        recorder: DecisionRecorder,
        *,
        jd_dir: str = "output/jds",
    ) -> None:
        """Initialize with ranked listings and a decision recorder."""
        self._listings = ranked_listings
        self._recorder = recorder
        self._jd_dir = jd_dir

    def undecided_listings(self) -> list[RankedListing]:
        """
        Return ranked listings that have no recorded decision.

        Sorted by final_score descending (best first).
        """
        undecided = [
            r for r in self._listings if self._recorder.get_decision(r.listing.external_id) is None
        ]
        undecided.sort(key=lambda r: r.final_score, reverse=True)
        return undecided

    def format_listing(
        self,
        ranked: RankedListing,
        *,
        rank: int,
        total: int,
    ) -> str:
        """
        Format a single listing for terminal display.

        Includes rank, title, company, URL, final score, component
        breakdown, and compensation range when available.
        """
        listing = ranked.listing
        scores = ranked.scores
        lines = [
            f"\n{'=' * 60}",
            self.format_progress(current=rank, total=total),
            f"  {listing.title}",
            f"  {listing.company} | {listing.board} | {listing.location}",
            f"  {listing.url}",
            f"  Score: {ranked.final_score:.2f}",
            f"    Fit: {scores.fit_score:.2f}  "
            f"Archetype: {scores.archetype_score:.2f}  "
            f"History: {scores.history_score:.2f}  "
            f"Comp: {scores.comp_score:.2f}",
        ]

        if listing.comp_min or listing.comp_max:
            comp_min = f"${listing.comp_min:,.0f}" if listing.comp_min else "?"
            comp_max = f"${listing.comp_max:,.0f}" if listing.comp_max else "?"
            lines.append(f"  Compensation: {comp_min} - {comp_max}")

        if scores.disqualified:
            lines.append(f"  ⚠ DISQUALIFIED: {scores.disqualifier_reason}")

        lines.append(f"{'=' * 60}")
        return "\n".join(lines)

    def format_progress(self, *, current: int, total: int) -> str:
        """Return a progress indicator like '[3/28]'."""
        return f"  [{current}/{total}]"

    @staticmethod
    def should_record(key: str) -> bool:
        """Return True if *key* maps to a recordable verdict ('y'/'n'/'m')."""
        return key.lower() in _VERDICT_MAP

    async def record_verdict(self, ranked: RankedListing, key: str, *, reason: str = "") -> None:
        """
        Record a verdict for the given listing.

        Args:
            ranked: The listing to record a decision for.
            key: One of 'y', 'n', 'm'.
            reason: Optional free-text explaining why this verdict was made.

        """
        verdict = _VERDICT_MAP[key.lower()]
        listing = ranked.listing
        await self._recorder.record(
            job_id=listing.external_id,
            verdict=verdict,
            jd_text=listing.full_text,
            board=listing.board,
            title=listing.title,
            company=listing.company,
            reason=reason,
        )

    def open_listing(self, ranked: RankedListing, *, rank: int = 0) -> None:
        """
        Open the listing's JD file or URL in the system browser.

        When *rank* is provided the JD file is located using the
        ``{rank:03d}_{company_slug}_{title_slug}.md`` convention.
        Falls back to the listing URL if the file does not exist.
        """
        listing = ranked.listing
        if rank:
            company_slug = slugify(listing.company)
            title_slug = slugify(listing.title)
            filename = f"{rank:03d}_{company_slug}_{title_slug}.md"
            jd_path = Path(self._jd_dir) / filename
        else:
            # Legacy fallback — external_id-based lookup
            jd_path = Path(self._jd_dir) / f"{listing.external_id}.md"
        if jd_path.exists():
            webbrowser.open(str(jd_path))
        else:
            webbrowser.open(listing.url)
