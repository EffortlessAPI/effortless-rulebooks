"""
Rescore pipeline — re-score existing JDs without browser automation.

The Rescorer loads previously exported JD files from ``output/jds/``,
re-scores each through the current RAG collections (which may have
updated archetypes, negative signals, or decision history), re-ranks,
and re-exports all results.

This enables fast iteration on archetype tuning and negative signal
refinement without re-running browser sessions.
"""

from __future__ import annotations

import logging
import re
from dataclasses import dataclass, field
from pathlib import Path
from typing import TYPE_CHECKING

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.errors import ActionableError
from jobsearch_rag.pipeline.ranker import RankedListing, Ranker, RankSummary
from jobsearch_rag.rag.comp_parser import compute_comp_score, parse_compensation

if TYPE_CHECKING:
    from jobsearch_rag.rag.scorer import Scorer, ScoreResult

logger = logging.getLogger(__name__)


@dataclass
class RescoreResult:
    """Results from a rescore run."""

    ranked_listings: list[RankedListing] = field(default_factory=lambda: [])
    summary: RankSummary = field(default_factory=RankSummary)
    failed_listings: int = 0
    total_loaded: int = 0


def _parse_jd_header(content: str) -> dict[str, str]:
    """
    Extract metadata from a JD file's YAML-style header.

    Expected format::

        # Title

        **Company:** Acme Corp
        **Location:** Remote
        **Board:** ziprecruiter
        **URL:** https://...
    """
    meta: dict[str, str] = {}

    # Title from first heading
    title_match = re.search(r"^# (.+)$", content, re.MULTILINE)
    if title_match:
        meta["title"] = title_match.group(1).strip()

    # Key-value pairs from bold labels
    for match in re.finditer(r"\*\*(\w+):\*\*\s*(.+)", content):
        key = match.group(1).lower()
        value = match.group(2).strip()
        meta[key] = value

    return meta


def _extract_jd_body(content: str) -> str:
    """Extract the JD body text after the ``## Job Description`` marker."""
    marker = "## Job Description\n"
    idx = content.find(marker)
    if idx == -1:
        return ""
    return content[idx + len(marker) :].strip()


def load_jd_files(jd_dir: str | Path) -> list[JobListing]:
    """
    Load JobListing objects from exported JD markdown files.

    Reads all ``.md`` files from ``jd_dir``, parses their metadata
    headers and JD bodies, and reconstructs JobListing objects for
    re-scoring.

    Returns an empty list if the directory doesn't exist or contains
    no valid JD files.
    """
    jd_path = Path(jd_dir)
    if not jd_path.is_dir():
        return []

    listings: list[JobListing] = []
    for md_file in sorted(jd_path.glob("*.md")):
        content = md_file.read_text(encoding="utf-8")
        meta = _parse_jd_header(content)
        body = _extract_jd_body(content)

        if not body:
            logger.debug("Skipping %s — no JD body found", md_file.name)
            continue

        title = meta.get("title", md_file.stem)
        company = meta.get("company", "Unknown")
        board = meta.get("board", "unknown")
        url = meta.get("url", "")
        location = meta.get("location", "")

        # Generate a stable external_id from the URL or filename
        external_id = url.rstrip("/").rsplit("/", 1)[-1] if url else md_file.stem

        # Parse compensation from body text
        comp_result = parse_compensation(body)
        comp_min = comp_result.comp_min if comp_result else None
        comp_max = comp_result.comp_max if comp_result else None

        listing = JobListing(
            board=board,
            external_id=external_id,
            title=title,
            company=company,
            location=location,
            url=url,
            full_text=body,
            comp_min=comp_min,
            comp_max=comp_max,
        )
        listings.append(listing)

    return listings


class Rescorer:
    """
    Re-scores existing JD files through the current RAG collections.

    Usage::

        rescorer = Rescorer(scorer=scorer, ranker=ranker, base_salary=220000)
        result = await rescorer.rescore(jd_dir="output/jds")
    """

    def __init__(
        self,
        *,
        scorer: Scorer,
        ranker: Ranker,
        base_salary: float = 220_000,
    ) -> None:
        """Initialize with a scorer, ranker, and base salary for compensation scoring."""
        self._scorer = scorer
        self._ranker = ranker
        self._base_salary = base_salary

    async def rescore(self, jd_dir: str | Path) -> RescoreResult:
        """
        Load JDs from disk, score, rank, and return results.

        Args:
            jd_dir: Path to the directory containing exported JD files.

        Returns:
            A :class:`RescoreResult` with the newly ranked listings.

        """
        listings = load_jd_files(jd_dir)
        if not listings:
            logger.warning("No JD files found in %s", jd_dir)
            return RescoreResult()

        result = RescoreResult(total_loaded=len(listings))

        scored: list[tuple[JobListing, ScoreResult]] = []
        embeddings: dict[str, list[float]] = {}

        for listing in listings:
            try:
                scores = await self._scorer.score(listing.full_text)
                # Compute comp_score from listing compensation data
                scores.comp_score = compute_comp_score(
                    listing.comp_max,
                    self._base_salary,
                )
                scored.append((listing, scores))
            except ActionableError as exc:
                logger.error(
                    "Failed to score '%s' at %s: %s",
                    listing.title,
                    listing.company,
                    exc.error,
                )
                result.failed_listings += 1

        if scored:
            ranked, summary = self._ranker.rank(scored, embeddings)
            result.ranked_listings = ranked
            result.summary = summary

        logger.info(
            "Rescored %d listings: %d ranked, %d failed",
            result.total_loaded,
            len(result.ranked_listings),
            result.failed_listings,
        )
        return result
