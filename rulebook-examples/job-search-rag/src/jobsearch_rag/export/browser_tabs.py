"""Browser tab opener for top results."""

from __future__ import annotations

import logging
import webbrowser
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from jobsearch_rag.pipeline.ranker import RankedListing

logger = logging.getLogger(__name__)


class BrowserTabOpener:
    """Opens the top-N ranked listing URLs in the default browser."""

    def open(self, listings: list[RankedListing], top_n: int = 5) -> None:
        """
        Open the first *top_n* listing URLs in browser tabs.

        Disqualified listings are excluded before selecting the top N.
        Listings are sorted descending by ``final_score`` so the best
        matches open first.  Failed opens are logged and skipped.
        """
        if top_n <= 0:
            return

        # Filter out disqualified, sort descending
        qualified = [r for r in listings if not (r.scores.disqualified and r.final_score == 0.0)]
        qualified.sort(key=lambda r: r.final_score, reverse=True)

        if not qualified:
            logger.info("No qualified results to open — 0 tabs opened.")
            return

        to_open = qualified[:top_n]

        for r in to_open:
            url = r.listing.url
            try:
                webbrowser.open(url)
            except OSError:
                logger.warning("Failed to open browser tab for %s", url)
