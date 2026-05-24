"""Markdown table export."""

from __future__ import annotations

import logging
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from jobsearch_rag.pipeline.ranker import RankedListing, RankSummary

logger = logging.getLogger(__name__)


class MarkdownExporter:
    """Renders ranked listings as a human-readable Markdown report."""

    def export(
        self,
        listings: list[RankedListing],
        output_path: str,
        *,
        summary: RankSummary | None = None,
    ) -> None:
        """
        Write a Markdown file with run summary and ranked listing table.

        Disqualified listings (``final_score == 0.0`` and ``disqualified``)
        are excluded.  Results are sorted descending by ``final_score``.
        """
        lines: list[str] = []

        # --- Run summary ---
        lines.append("# Run Summary\n")
        if summary is not None:
            lines.append(f"- **Total found:** {summary.total_found}")
            lines.append(f"- **Total scored:** {summary.total_scored}")
            lines.append(f"- **Excluded:** {summary.total_excluded}")
            lines.append(f"- **Deduplicated:** {summary.total_deduplicated}")
        lines.append("")

        # Filter and sort
        qualified = [r for r in listings if not (r.scores.disqualified and r.final_score == 0.0)]
        qualified.sort(key=lambda r: r.final_score, reverse=True)

        if not qualified:
            lines.append("No results to display.\n")
            with open(output_path, "w") as f:
                f.write("\n".join(lines))
            return

        # --- Listing table ---
        lines.append("## Ranked Listings\n")
        lines.append("| # | Title | Company | Board | Score | Breakdown | URL |")
        lines.append("|---|-------|---------|-------|-------|-----------|-----|")

        for rank, r in enumerate(qualified, start=1):
            explanation = r.score_explanation()
            url = r.listing.url
            lines.append(
                f"| {rank} "
                f"| {r.listing.title} "
                f"| {r.listing.company} "
                f"| {r.listing.board} "
                f"| {r.final_score:.2f} "
                f"| {explanation} "
                f"| {url} |"
            )

        lines.append("")

        with open(output_path, "w") as f:
            f.write("\n".join(lines))
