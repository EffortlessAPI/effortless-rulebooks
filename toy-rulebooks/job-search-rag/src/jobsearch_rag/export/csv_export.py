"""CSV export."""

from __future__ import annotations

import csv
import logging
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from jobsearch_rag.pipeline.ranker import RankedListing, RankSummary

logger = logging.getLogger(__name__)

# Columns included in CSV output.  ``full_text`` is deliberately excluded
# because it is too large for spreadsheet cells.
_COLUMNS = [
    "title",
    "company",
    "board",
    "location",
    "final_score",
    "fit_score",
    "archetype_score",
    "history_score",
    "comp_score",
    "culture_score",
    "negative_score",
    "comp_min",
    "comp_max",
    "disqualified",
    "disqualifier_reason",
    "url",
]


class CSVExporter:
    """Renders ranked listings as a CSV file suitable for spreadsheet import."""

    def export(
        self,
        listings: list[RankedListing],
        output_path: str,
        *,
        summary: RankSummary | None = None,
    ) -> None:
        """
        Write a CSV with header row.  Full JD text is excluded.

        Disqualified listings are excluded.  Results are sorted descending
        by ``final_score``.
        """
        qualified = [r for r in listings if not (r.scores.disqualified and r.final_score == 0.0)]
        qualified.sort(key=lambda r: r.final_score, reverse=True)

        with open(output_path, "w", newline="") as f:
            writer = csv.writer(f)
            writer.writerow(_COLUMNS)

            for r in qualified:
                writer.writerow(
                    [
                        r.listing.title,
                        r.listing.company,
                        r.listing.board,
                        r.listing.location,
                        f"{r.final_score:.4f}",
                        f"{r.scores.fit_score:.4f}",
                        f"{r.scores.archetype_score:.4f}",
                        f"{r.scores.history_score:.4f}",
                        f"{r.scores.comp_score:.4f}",
                        f"{r.scores.culture_score:.4f}",
                        f"{r.scores.negative_score:.4f}",
                        f"{r.listing.comp_min:.0f}"
                        if isinstance(r.listing.comp_min, int | float)
                        else "",
                        f"{r.listing.comp_max:.0f}"
                        if isinstance(r.listing.comp_max, int | float)
                        else "",
                        r.scores.disqualified,
                        r.scores.disqualifier_reason or "",
                        r.listing.url,
                    ]
                )
