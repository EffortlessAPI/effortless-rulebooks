"""
Score fusion, deduplication, and final ranking.

The Ranker is the bridge between raw scoring outputs and the final ranked
shortlist the operator reviews.  It performs three operations in sequence:

1. **Score fusion** — combine fit / archetype / history / culture scores
   using configurable weights from ``settings.toml`` into a single
   ``final_score``.  Disqualified roles are zeroed.

2. **Deduplication** — the same job often appears on multiple boards.
   Exact matches (same ``external_id`` + ``board``) are collapsed
   unconditionally.  Near-duplicates (cosine similarity > 0.95 on
   ``full_text`` embeddings) are collapsed, keeping the highest-scored
   instance and noting which other boards carried it.

3. **Threshold filtering** — roles scoring below ``min_score_threshold``
   are excluded from the final output entirely.
"""

from __future__ import annotations

import logging
from dataclasses import dataclass, field
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from jobsearch_rag.adapters.base import JobListing
    from jobsearch_rag.rag.scorer import ScoreResult

logger = logging.getLogger(__name__)


@dataclass
class RankedListing:
    """A listing enriched with its computed scores and final rank."""

    listing: JobListing
    scores: ScoreResult
    final_score: float
    duplicate_boards: list[str] = field(default_factory=list[str])

    def score_explanation(self) -> str:
        """
        Human-readable score breakdown for export output.

        Uses ``·`` (middle dot) as separator instead of ``|`` to avoid
        breaking Markdown table column boundaries.
        """
        parts = [
            f"Archetype: {self.scores.archetype_score:.2f}",
            f"Fit: {self.scores.fit_score:.2f}",
            f"History: {self.scores.history_score:.2f}",
            f"Comp: {self.scores.comp_score:.2f}",
            f"Culture: {self.scores.culture_score:.2f}",
            f"Negative: {self.scores.negative_score:.2f}",
        ]
        if self.scores.disqualified:
            parts.append(f"DISQUALIFIED: {self.scores.disqualifier_reason}")
        else:
            parts.append("Not disqualified")
        return " · ".join(parts)


@dataclass
class RankSummary:
    """Statistics from a ranking run, used in export summaries."""

    total_found: int = 0
    total_scored: int = 0
    total_excluded: int = 0
    total_deduplicated: int = 0


class Ranker:
    """
    Score fuser and deduplicator for ranked job listings.

    Fuses weighted component scores, deduplicates across boards,
    and produces the final ranked shortlist.
    """

    def __init__(
        self,
        archetype_weight: float,
        fit_weight: float,
        history_weight: float,
        comp_weight: float = 0.0,
        negative_weight: float = 0.4,
        culture_weight: float = 0.0,
        min_score_threshold: float = 0.45,
    ) -> None:
        """Initialize with scoring weights and threshold."""
        self.archetype_weight = archetype_weight
        self.fit_weight = fit_weight
        self.history_weight = history_weight
        self.comp_weight = comp_weight
        self.negative_weight = negative_weight
        self.culture_weight = culture_weight
        self.min_score_threshold = min_score_threshold

    def rank(
        self,
        listings: list[tuple[JobListing, ScoreResult]],
        embeddings: dict[str, list[float]] | None = None,
    ) -> tuple[list[RankedListing], RankSummary]:
        """
        Apply score fusion, deduplication, and threshold filtering.

        Args:
            listings: Pairs of (listing, scores) from the scorer.
            embeddings: Optional dict mapping ``listing.url`` to its
                embedding vector, used for near-duplicate detection.
                When absent, only exact-match dedup is performed.

        Returns:
            A tuple of (ranked_listings sorted descending by final_score,
            summary statistics).

        """
        summary = RankSummary(total_found=len(listings))

        # Step 1: Score fusion — compute final_score for each listing
        ranked: list[RankedListing] = []
        for listing, scores in listings:
            final = self.compute_final_score(scores)
            ranked.append(
                RankedListing(
                    listing=listing,
                    scores=scores,
                    final_score=final,
                )
            )

        summary.total_scored = len(ranked)

        # Step 2: Deduplication
        ranked = self._deduplicate_exact(ranked)
        ranked = self._deduplicate_near(ranked, embeddings or {})
        summary.total_deduplicated = summary.total_scored - len(ranked)

        # Step 3: Threshold filtering — exclude below min_score_threshold
        before_filter = len(ranked)
        ranked = [r for r in ranked if r.final_score >= self.min_score_threshold]
        summary.total_excluded = before_filter - len(ranked)

        # Step 4: Sort descending by final_score
        ranked.sort(key=lambda r: r.final_score, reverse=True)

        logger.info(
            "Ranked %d listings: %d scored, %d deduplicated, %d excluded below threshold %.2f",
            len(ranked),
            summary.total_scored,
            summary.total_deduplicated,
            summary.total_excluded,
            self.min_score_threshold,
        )

        return ranked, summary

    def compute_final_score(self, scores: ScoreResult) -> float:
        """
        Weighted sum of component scores minus negative penalty, zeroed if disqualified.

        The formula is:
            positive = (archetype_weight * archetype_score
                       + fit_weight * fit_score
                       + history_weight * history_score
                       + comp_weight * comp_score
                       + culture_weight * culture_score)
            final = max(0.0, positive - negative_weight * negative_score)

        The floor at 0.0 prevents negative final scores.
        """
        if scores.disqualified:
            return 0.0
        positive = (
            self.archetype_weight * scores.archetype_score
            + self.fit_weight * scores.fit_score
            + self.history_weight * scores.history_score
            + self.comp_weight * scores.comp_score
            + self.culture_weight * scores.culture_score
        )
        penalty = self.negative_weight * scores.negative_score
        return max(0.0, positive - penalty)

    # ------------------------------------------------------------------
    # Deduplication internals
    # ------------------------------------------------------------------

    @staticmethod
    def _deduplicate_exact(ranked: list[RankedListing]) -> list[RankedListing]:
        """
        Remove exact duplicates: same external_id on the same board.

        Keeps the first (and only meaningful) occurrence.
        """
        seen: set[tuple[str, str]] = set()
        result: list[RankedListing] = []
        for r in ranked:
            key = (r.listing.board, r.listing.external_id)
            if key in seen:
                continue
            seen.add(key)
            result.append(r)
        return result

    @staticmethod
    def _deduplicate_near(
        ranked: list[RankedListing],
        embeddings: dict[str, list[float]],
    ) -> list[RankedListing]:
        """
        Collapse near-duplicate listings across boards.

        Two listings are near-duplicates if the cosine similarity of
        their ``full_text`` embeddings exceeds 0.95.  The instance with
        the highest ``final_score`` is kept; the others' board names are
        appended to ``duplicate_boards`` on the survivor.
        """
        if not embeddings:
            return ranked

        # Sort by final_score descending so the highest scorer is always
        # the first encountered and thus becomes the "survivor"
        ranked_sorted = sorted(ranked, key=lambda r: r.final_score, reverse=True)
        survivors: list[RankedListing] = []
        consumed: set[int] = set()  # indices into ranked_sorted

        for i, candidate in enumerate(ranked_sorted):
            if i in consumed:
                continue

            cand_embed = embeddings.get(candidate.listing.url)
            if cand_embed is None:
                survivors.append(candidate)
                continue

            # Check remaining items for near-duplication against this candidate
            for j in range(i + 1, len(ranked_sorted)):
                if j in consumed:
                    continue
                other = ranked_sorted[j]
                other_embed = embeddings.get(other.listing.url)
                if other_embed is None:
                    continue

                sim = _cosine_similarity(cand_embed, other_embed)
                if sim > 0.95:
                    # Collapse: candidate survives, other is consumed
                    if other.listing.board not in candidate.duplicate_boards:
                        candidate.duplicate_boards.append(other.listing.board)
                    consumed.add(j)

            survivors.append(candidate)

        return survivors


def _cosine_similarity(a: list[float], b: list[float]) -> float:
    """
    Compute cosine similarity between two vectors.

    Returns a value in [-1.0, 1.0].  For normalized embedding vectors
    this is equivalent to the dot product.
    """
    if len(a) != len(b) or not a:
        return 0.0

    dot: float = sum(x * y for x, y in zip(a, b, strict=True))
    mag_a: float = sum(x * x for x in a) ** 0.5
    mag_b: float = sum(x * x for x in b) ** 0.5

    if mag_a == 0.0 or mag_b == 0.0:
        return 0.0

    result: float = dot / (mag_a * mag_b)
    return result
