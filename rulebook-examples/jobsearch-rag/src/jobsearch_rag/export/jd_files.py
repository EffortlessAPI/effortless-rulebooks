"""
Individual JD markdown file export.

Writes each ranked listing's full job description as a standalone
Markdown file under ``output/jds/``.  Each file includes a YAML-style
metadata header (title, company, board, score, URL) followed by the
full JD text — ready for review in a Markdown editor, browser, or
AI assistant (e.g. Edge Copilot).

Files are named ``NNN_company_title.md`` so they sort by rank.
"""

from __future__ import annotations

import logging
from pathlib import Path
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from jobsearch_rag.pipeline.ranker import RankedListing, RankSummary

from jobsearch_rag.text import slugify

logger = logging.getLogger(__name__)


class JDFileExporter:
    """
    Exports each ranked listing as an individual Markdown file.

    Usage::

        exporter = JDFileExporter()
        paths = exporter.export(ranked_listings, "output/jds")
    """

    def export(
        self,
        listings: list[RankedListing],
        output_dir: str,
        *,
        summary: RankSummary | None = None,
    ) -> list[Path]:
        """
        Write individual JD files for qualified listings.

        Returns the list of file paths created.

        Disqualified listings (``final_score == 0.0`` and ``disqualified``)
        are excluded.  Results are sorted descending by ``final_score``.
        Only listings with non-empty ``full_text`` are exported.
        """
        out = Path(output_dir)
        out.mkdir(parents=True, exist_ok=True)

        qualified = [r for r in listings if not (r.scores.disqualified and r.final_score == 0.0)]
        qualified.sort(key=lambda r: r.final_score, reverse=True)

        paths: list[Path] = []
        for rank, r in enumerate(qualified, start=1):
            if not r.listing.full_text.strip():
                logger.debug(
                    "Skipping JD export for %s — no full_text",
                    r.listing.title,
                )
                continue

            company_slug = slugify(r.listing.company)
            title_slug = slugify(r.listing.title)
            filename = f"{rank:03d}_{company_slug}_{title_slug}.md"
            filepath = out / filename

            content = self._render(r, rank)
            filepath.write_text(content)
            paths.append(filepath)

        logger.info("Exported %d JD files to %s", len(paths), out)
        return paths

    def _render(self, r: RankedListing, rank: int) -> str:
        """Render a single JD as a Markdown document."""
        lines: list[str] = []

        # Header
        lines.append(f"# {r.listing.title}")
        lines.append("")
        lines.append(f"**Company:** {r.listing.company}  ")
        lines.append(f"**Location:** {r.listing.location}  ")
        lines.append(f"**Board:** {r.listing.board}  ")
        lines.append(f"**URL:** {r.listing.url}  ")
        lines.append("")

        # Score summary
        lines.append("## Score")
        lines.append("")
        lines.append(f"- **Rank:** #{rank}")
        lines.append(f"- **Final Score:** {r.final_score:.2f}")
        lines.append(f"- **Fit Score:** {r.scores.fit_score:.2f}")
        lines.append(f"- **Archetype Score:** {r.scores.archetype_score:.2f}")
        lines.append(f"- **History Score:** {r.scores.history_score:.2f}")
        lines.append(f"- **Comp Score:** {r.scores.comp_score:.2f}")
        lines.append(f"- **Culture Score:** {r.scores.culture_score:.2f}")
        lines.append(f"- **Negative Score:** {r.scores.negative_score:.2f}")
        if r.scores.disqualified:
            lines.append(f"- **Disqualified:** {r.scores.disqualifier_reason}")
        if r.duplicate_boards:
            lines.append(f"- **Also on:** {', '.join(r.duplicate_boards)}")
        lines.append("")

        # Full JD
        lines.append("## Job Description")
        lines.append("")
        lines.append(r.listing.full_text.strip())
        lines.append("")

        return "\n".join(lines)
