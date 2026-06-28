"""
Export tests — Markdown, CSV, and browser tab opener.

Maps to BDD specs: TestMarkdownExport, TestCSVExport, TestBrowserTabOpener

The export layer transforms ranked pipeline output into human-consumable
formats: Markdown tables (primary), CSV (spreadsheet import), and browser
tabs (immediate review).  All exporters receive ``RankedListing`` objects
produced by the Ranker — no adapter or RAG dependencies exist here.

Spec classes:
    TestMarkdownExport
    TestCSVExport
    TestBrowserTabOpener
    TestJDFileExport
"""

from __future__ import annotations

import csv
import logging
import webbrowser
from typing import TYPE_CHECKING
from unittest.mock import patch

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.export.browser_tabs import BrowserTabOpener
from jobsearch_rag.export.csv_export import CSVExporter
from jobsearch_rag.export.jd_files import JDFileExporter
from jobsearch_rag.export.markdown import MarkdownExporter
from jobsearch_rag.pipeline.ranker import RankedListing, RankSummary
from jobsearch_rag.rag.scorer import ScoreResult

if TYPE_CHECKING:
    from pathlib import Path

    import pytest

# ---------------------------------------------------------------------------
# Helpers & fixtures
# ---------------------------------------------------------------------------


def _listing(
    *,
    title: str = "Staff Architect",
    company: str = "Acme Corp",
    board: str = "ziprecruiter",
    url: str = "https://example.org/job/1",
    full_text: str = "Build distributed systems.",
    external_id: str = "ext-001",
) -> JobListing:
    return JobListing(
        board=board,
        external_id=external_id,
        title=title,
        company=company,
        location="Remote",
        url=url,
        full_text=full_text,
    )


def _scores(
    *,
    fit: float = 0.74,
    archetype: float = 0.81,
    history: float = 0.62,
    comp: float = 0.5,
    disqualified: bool = False,
    reason: str | None = None,
) -> ScoreResult:
    return ScoreResult(
        fit_score=fit,
        archetype_score=archetype,
        history_score=history,
        comp_score=comp,
        disqualified=disqualified,
        disqualifier_reason=reason,
    )


def _ranked(
    *,
    title: str = "Staff Architect",
    company: str = "Acme Corp",
    board: str = "ziprecruiter",
    url: str = "https://example.org/job/1",
    external_id: str = "ext-001",
    final_score: float = 0.78,
    fit: float = 0.74,
    archetype: float = 0.81,
    history: float = 0.62,
    comp: float = 0.5,
    disqualified: bool = False,
    reason: str | None = None,
    duplicate_boards: list[str] | None = None,
) -> RankedListing:
    return RankedListing(
        listing=_listing(
            title=title,
            company=company,
            board=board,
            url=url,
            external_id=external_id,
        ),
        scores=_scores(
            fit=fit,
            archetype=archetype,
            history=history,
            comp=comp,
            disqualified=disqualified,
            reason=reason,
        ),
        final_score=final_score,
        duplicate_boards=duplicate_boards or [],
    )


def _summary(
    *,
    total_found: int = 10,
    total_scored: int = 8,
    total_excluded: int = 1,
    total_deduplicated: int = 1,
) -> RankSummary:
    return RankSummary(
        total_found=total_found,
        total_scored=total_scored,
        total_excluded=total_excluded,
        total_deduplicated=total_deduplicated,
    )


# ---------------------------------------------------------------------------
# TestMarkdownExport
# ---------------------------------------------------------------------------


class TestMarkdownExport:
    """
    REQUIREMENT: Markdown output is human-readable and complete.

    WHO: The operator reviewing results in Obsidian or a text editor
    WHAT: (1) The system includes each listing's title, company, board, scores, disqualifier status, and URL in the Markdown output.
          (2) The system orders listings by descending final score in the Markdown output.
          (3) The system excludes disqualified roles with a score of 0.0 from the Markdown output.
          (4) The system shows fit, archetype, history, and comp scores in the Markdown output.
          (5) The system includes the culture score in the Markdown output.
          (6) The system includes the negative score in the Markdown output.
          (7) The system places the run summary before any listing in the Markdown output.
          (8) The system reports total found, scored, excluded, and deduplicated counts in the run summary.
          (9) The system includes a non-empty URL for every listing in the Markdown output.
          (10) The system produces a valid Markdown file with a summary and no listing table when the result set is empty.
          (11) The system omits total-found/scored/excluded/deduplicated stats when no summary is provided.
    WHY: The ranked list is the primary product of every run —
         missing fields or wrong sort order defeats the purpose

    MOCK BOUNDARY:
        Mock: nothing — pure file I/O via tmp_path
        Real: MarkdownExporter.export(), RankedListing, RankSummary
        Never: Patch exporter internals or file writing
    """

    def test_output_includes_all_required_fields_per_listing(self, tmp_path: Path) -> None:
        """
        GIVEN a single ranked listing
        WHEN exported to Markdown
        THEN the output includes title, company, board, scores, disqualifier status, and URL.
        """
        # Given: a single listing
        listings = [_ranked()]
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=_summary())
        content = out.read_text()

        # Then: all required fields are present
        assert "Staff Architect" in content, "Title should appear in output"
        assert "Acme Corp" in content, "Company should appear in output"
        assert "ziprecruiter" in content, "Board should appear in output"
        assert "0.78" in content, "Final score should appear in output"
        assert "example.org/job/1" in content, "URL should appear in output"
        assert "Not disqualified" in content, "Disqualifier status should appear"

    def test_listings_are_sorted_descending_by_final_score(self, tmp_path: Path) -> None:
        """
        GIVEN listings with different final scores
        WHEN exported to Markdown
        THEN they appear in descending score order so the best matches are reviewed first.
        """
        # Given: three listings with different scores
        listings = [
            _ranked(title="Low", final_score=0.40),
            _ranked(
                title="High",
                final_score=0.90,
                external_id="ext-002",
                url="https://example.org/job/2",
            ),
            _ranked(
                title="Mid",
                final_score=0.65,
                external_id="ext-003",
                url="https://example.org/job/3",
            ),
        ]
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=_summary())
        content = out.read_text()

        # Then: High appears before Mid, Mid before Low
        high_pos = content.index("High")
        mid_pos = content.index("Mid")
        low_pos = content.index("Low")
        assert high_pos < mid_pos < low_pos, "Listings should be sorted descending by score"

    def test_disqualified_roles_are_not_present_in_output(self, tmp_path: Path) -> None:
        """
        GIVEN a mix of qualified and disqualified listings
        WHEN exported to Markdown
        THEN disqualified roles (score 0.0) are excluded entirely.
        """
        # Given: one good, one disqualified
        listings = [
            _ranked(title="Good Role", final_score=0.80),
            _ranked(
                title="Bad Role",
                final_score=0.0,
                disqualified=True,
                reason="IC role",
                external_id="ext-002",
                url="https://example.org/job/2",
            ),
        ]
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=_summary())
        content = out.read_text()

        # Then: only qualified role appears
        assert "Good Role" in content, "Qualified role should appear"
        assert "Bad Role" not in content, "Disqualified role should be excluded"

    def test_score_explanation_shows_all_three_component_scores(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with specific component scores
        WHEN exported to Markdown
        THEN the output shows fit, archetype, history, and comp values for transparency.
        """
        # Given: listing with known component scores
        listings = [_ranked(fit=0.74, archetype=0.81, history=0.62, comp=0.90)]
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=_summary())
        content = out.read_text()

        # Then: all component scores appear
        assert "0.74" in content, "Fit score should appear"
        assert "0.81" in content, "Archetype score should appear"
        assert "0.62" in content, "History score should appear"
        assert "0.90" in content, "Comp score should appear"

    def test_score_explanation_includes_culture_score(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with culture_score set
        WHEN exported to Markdown
        THEN the output includes the culture score for environment-quality transparency.
        """
        # Given: listing with culture_score
        scores = _scores(fit=0.74, archetype=0.81, history=0.62, comp=0.90)
        scores.culture_score = 0.65
        listing = RankedListing(
            listing=_listing(),
            scores=scores,
            final_score=0.78,
        )
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export([listing], str(out), summary=_summary())
        content = out.read_text()

        # Then: culture score appears
        assert "Culture" in content, (
            f"culture_score should appear in markdown output. Got: {content}"
        )
        assert "0.65" in content, (
            f"culture_score value 0.65 should appear in output. Got: {content}"
        )

    def test_score_explanation_includes_negative_score(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with negative_score set
        WHEN exported to Markdown
        THEN the output includes the negative score for penalty signal transparency.
        """
        # Given: listing with negative_score
        scores = _scores(fit=0.74, archetype=0.81, history=0.62, comp=0.90)
        scores.negative_score = 0.25
        listing = RankedListing(
            listing=_listing(),
            scores=scores,
            final_score=0.78,
        )
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export([listing], str(out), summary=_summary())
        content = out.read_text()

        # Then: negative score appears
        assert "Negative" in content, (
            f"negative_score should appear in markdown output. Got: {content}"
        )
        assert "0.25" in content, (
            f"negative_score value 0.25 should appear in output. Got: {content}"
        )

    def test_run_summary_appears_at_top_of_output(self, tmp_path: Path) -> None:
        """
        GIVEN listings with a run summary
        WHEN exported to Markdown
        THEN the summary header appears before any listing, providing immediate run context.
        """
        # Given: a listing with summary
        listings = [_ranked()]
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=_summary())
        content = out.read_text()

        # Then: summary appears before listings
        summary_pos = content.index("Summary")
        listing_pos = content.index("Staff Architect")
        assert summary_pos < listing_pos, "Summary should appear before listings"

    def test_run_summary_includes_total_found_scored_excluded_deduplicated(
        self, tmp_path: Path
    ) -> None:
        """
        GIVEN specific run summary counts
        WHEN exported to Markdown
        THEN the output reports total found, scored, excluded, and deduplicated counts.
        """
        # Given: summary with specific counts
        listings = [_ranked()]
        summary = _summary(total_found=10, total_scored=8, total_excluded=1, total_deduplicated=1)
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=summary)
        content = out.read_text()

        # Then: counts appear in output
        assert "10" in content, "Total found should appear"
        assert "8" in content, "Total scored should appear"
        assert "1" in content, "Excluded/deduplicated counts should appear"

    def test_url_is_present_and_non_empty_for_every_listing(self, tmp_path: Path) -> None:
        """
        GIVEN multiple listings with distinct URLs
        WHEN exported to Markdown
        THEN every listing has its URL present so the operator can click through.
        """
        # Given: two listings with different URLs
        listings = [
            _ranked(url="https://example.org/job/1"),
            _ranked(url="https://example.org/job/2", external_id="ext-002"),
        ]
        out = tmp_path / "results.md"

        # When: export to Markdown
        MarkdownExporter().export(listings, str(out), summary=_summary())
        content = out.read_text()

        # Then: both URLs appear
        assert "https://example.org/job/1" in content, "First URL should appear"
        assert "https://example.org/job/2" in content, "Second URL should appear"

    def test_empty_result_set_produces_output_with_summary_and_no_table(
        self, tmp_path: Path
    ) -> None:
        """
        GIVEN an empty result set
        WHEN exported to Markdown
        THEN a valid file is produced with a summary but no listing table.
        """
        # Given: empty listings
        out = tmp_path / "results.md"
        summary = _summary(total_found=0, total_scored=0, total_excluded=0, total_deduplicated=0)

        # When: export to Markdown
        MarkdownExporter().export([], str(out), summary=summary)
        content = out.read_text()

        # Then: summary present, no listings
        assert "Summary" in content, "Summary should appear even with no results"
        assert "No results" in content or "0" in content, "Should indicate no results"

    def test_export_without_summary_omits_summary_stats(self, tmp_path: Path) -> None:
        """
        GIVEN a listing set with no summary provided
        WHEN exported to Markdown
        THEN the output contains the summary heading but no total-found/scored lines.
        """
        # Given: listings with no summary
        out = tmp_path / "results.md"
        listings = [_ranked()]

        # When: export to Markdown with summary=None (the default)
        MarkdownExporter().export(listings, str(out))
        content = out.read_text()

        # Then: summary heading present but no stats
        assert "Summary" in content, "Summary heading should appear even without stats"
        assert "Total found" not in content, (
            f"Should not contain 'Total found' when no summary given, got: {content!r}"
        )


# ---------------------------------------------------------------------------
# TestCSVExport
# ---------------------------------------------------------------------------


class TestCSVExport:
    """
    REQUIREMENT: CSV export is valid and importable by standard tools.

    WHO: The operator importing results into a spreadsheet or ATS tracker
    WHAT: (1) The system starts the CSV file with a header row that contains all required column names.
          (2) The system includes the `comp_score`, `comp_min`, and `comp_max` columns in the CSV header.
          (3) The system excludes `full_text` from the CSV output.
          (4) The system quotes company names that contain commas so they remain intact across CSV imports.
          (5) The system produces a valid CSV containing only the header row when there are no results.
          (6) The system writes one data row for each non-disqualified listing in the CSV output.
          (7) The system includes a `culture_score` column in the CSV header.
          (8) The system includes a `negative_score` column in the CSV header.
    WHY: A CSV with unescaped commas or missing headers silently corrupts
         on import — the operator may not notice

    MOCK BOUNDARY:
        Mock: nothing — pure file I/O via tmp_path
        Real: CSVExporter.export(), csv.reader round-trip
        Never: Patch CSV writer internals
    """

    def test_csv_has_header_row_with_all_required_columns(self, tmp_path: Path) -> None:
        """
        GIVEN a single ranked listing
        WHEN exported to CSV
        THEN the file starts with a header row containing all required column names.
        """
        # Given: a single listing
        listings = [_ranked()]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: header contains required columns
        with open(out) as f:
            reader = csv.reader(f)
            header = next(reader)
        required = ["title", "company", "board", "final_score", "url"]
        for col in required:
            assert col in header, f"Missing required column: {col}"

    def test_csv_includes_comp_score_comp_min_comp_max_columns(self, tmp_path: Path) -> None:
        """
        GIVEN a ranked listing
        WHEN exported to CSV
        THEN the header includes comp_score, comp_min, and comp_max columns.
        """
        # Given: a single listing
        listings = [_ranked()]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: comp columns present
        with open(out) as f:
            reader = csv.reader(f)
            header = next(reader)
        for col in ["comp_score", "comp_min", "comp_max"]:
            assert col in header, f"Missing comp column: {col}"

    def test_full_jd_text_is_not_included_in_csv_output(self, tmp_path: Path) -> None:
        """
        GIVEN a ranked listing with full_text
        WHEN exported to CSV
        THEN full_text is omitted since it is too large for spreadsheet cells.
        """
        # Given: a listing with full_text
        listings = [_ranked()]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: full_text column is absent
        with open(out) as f:
            reader = csv.reader(f)
            header = next(reader)
        assert "full_text" not in header, "full_text should be excluded from CSV"

    def test_company_names_with_commas_are_properly_quoted(self, tmp_path: Path) -> None:
        """
        GIVEN a company name containing commas
        WHEN exported to CSV
        THEN the name is properly quoted so it doesn't split across columns on import.
        """
        # Given: listing with comma in company name
        listings = [_ranked(company="Acme, Inc.")]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: company survives CSV round-trip intact
        with open(out) as f:
            reader = csv.reader(f)
            _header = next(reader)
            row = next(reader)
        assert "Acme, Inc." in row, "Company name with commas should survive CSV round-trip"

    def test_empty_result_set_produces_header_only_csv(self, tmp_path: Path) -> None:
        """
        GIVEN an empty result set
        WHEN exported to CSV
        THEN the file contains headers only, remaining valid for import tools.
        """
        # Given: empty listings
        out = tmp_path / "results.csv"
        summary = _summary(total_found=0, total_scored=0, total_excluded=0, total_deduplicated=0)

        # When: export to CSV
        CSVExporter().export([], str(out), summary=summary)

        # Then: header present, no data rows
        with open(out) as f:
            reader = csv.reader(f)
            header = next(reader)
            rows = list(reader)
        assert len(header) > 0, "Header row should be present"
        assert len(rows) == 0, "No data rows should exist for empty result set"

    def test_csv_row_count_matches_scored_listing_count(self, tmp_path: Path) -> None:
        """
        GIVEN a mix of scored and disqualified listings
        WHEN exported to CSV
        THEN the data row count equals the number of non-disqualified listings.
        """
        # Given: 2 scored + 1 disqualified
        listings = [
            _ranked(title="Role A", external_id="ext-001"),
            _ranked(title="Role B", external_id="ext-002", url="https://example.org/job/2"),
            _ranked(
                title="Disqualified",
                final_score=0.0,
                disqualified=True,
                reason="IC role",
                external_id="ext-003",
                url="https://example.org/job/3",
            ),
        ]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: only 2 data rows (disqualified excluded)
        with open(out) as f:
            reader = csv.reader(f)
            _header = next(reader)
            rows = list(reader)
        assert len(rows) == 2, "Only non-disqualified listings should appear as data rows"

    def test_csv_includes_culture_score_column(self, tmp_path: Path) -> None:
        """
        GIVEN a ranked listing
        WHEN exported to CSV
        THEN the header includes a culture_score column.
        """
        # Given: a listing
        listings = [_ranked()]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: culture_score column present
        with open(out) as f:
            reader = csv.reader(f)
            header = next(reader)
        assert "culture_score" in header, f"Missing culture_score column. Header: {header}"

    def test_csv_includes_negative_score_column(self, tmp_path: Path) -> None:
        """
        GIVEN a ranked listing
        WHEN exported to CSV
        THEN the header includes a negative_score column.
        """
        # Given: a listing
        listings = [_ranked()]
        out = tmp_path / "results.csv"

        # When: export to CSV
        CSVExporter().export(listings, str(out), summary=_summary())

        # Then: negative_score column present
        with open(out) as f:
            reader = csv.reader(f)
            header = next(reader)
        assert "negative_score" in header, f"Missing negative_score column. Header: {header}"


# ---------------------------------------------------------------------------
# TestBrowserTabOpener
# ---------------------------------------------------------------------------


class TestBrowserTabOpener:
    """
    REQUIREMENT: Top-ranked results open as browser tabs in score order.

    WHO: The operator who wants to review shortlisted roles without manually
         clicking through the ranked output
    WHAT: (1) The system opens browser tabs in descending score order so the highest-scored result appears first.
          (2) The system opens exactly the number of tabs specified by the `--open-top` CLI flag.
          (3) The system uses the `open_top_n` value from settings when the CLI does not provide `--open-top`.
          (4) The system opens all available results without error when fewer results exist than the requested top N.
          (5) The system excludes disqualified roles from browser tabs regardless of their position in the results.
          (6) The system logs a failed URL and continues opening the remaining tabs when one tab open raises an `OSError`.
          (7) The system opens no tabs and logs an advisory message when all results have zero scores.
          (8) The system opens tabs in the default system browser instead of the Playwright automation session.
          (9) The system opens no tabs and raises no error when `--open-top` is set to 0.
    WHY: The tab opener is the last mile of the workflow — an error here
         that aborts remaining tabs wastes the entire run's value;
         opening in wrong order defeats the purpose of ranking

    MOCK BOUNDARY:
        Mock: webbrowser.open (system browser I/O)
        Real: BrowserTabOpener.open(), sort/filter logic
        Never: Patch internal sort or filter methods
    """

    @patch("webbrowser.open")
    def test_tabs_open_in_descending_score_order(self, mock_open: object) -> None:
        """
        GIVEN listings with different scores
        WHEN tabs are opened
        THEN browser tabs open highest-scored first so the best match is immediately visible.
        """
        # Given: three listings with different scores
        listings = [
            _ranked(title="Low", final_score=0.40, url="https://example.org/low"),
            _ranked(
                title="High",
                final_score=0.90,
                url="https://example.org/high",
                external_id="ext-002",
            ),
            _ranked(
                title="Mid", final_score=0.65, url="https://example.org/mid", external_id="ext-003"
            ),
        ]

        # When: open all tabs
        BrowserTabOpener().open(listings, top_n=3)

        # Then: tabs opened in descending score order
        calls = [c.args[0] for c in webbrowser.open.call_args_list]  # type: ignore[attr-defined]
        assert calls == [
            "https://example.org/high",
            "https://example.org/mid",
            "https://example.org/low",
        ], "Tabs should open in descending score order"

    @patch("webbrowser.open")
    def test_tab_count_respects_open_top_n_from_cli(self, mock_open: object) -> None:
        """
        GIVEN 5 listings
        WHEN the --open-top N CLI flag limits to 2
        THEN exactly 2 tabs are opened.
        """
        # Given: 5 listings
        listings = [
            _ranked(
                title=f"Role {i}",
                final_score=0.9 - i * 0.1,
                external_id=f"ext-{i}",
                url=f"https://example.org/{i}",
            )
            for i in range(5)
        ]

        # When: open with top_n=2
        BrowserTabOpener().open(listings, top_n=2)

        # Then: exactly 2 tabs opened
        assert webbrowser.open.call_count == 2, "Should open exactly 2 tabs"  # type: ignore[attr-defined]

    @patch("webbrowser.open")
    def test_tab_count_respects_open_top_n_from_settings_when_cli_not_provided(
        self, mock_open: object
    ) -> None:
        """
        GIVEN 10 listings and no CLI --open-top specified
        WHEN tabs are opened with the default
        THEN the default from settings.toml (5) is used.
        """
        # Given: 10 listings
        listings = [
            _ranked(
                title=f"Role {i}",
                final_score=0.9 - i * 0.1,
                external_id=f"ext-{i}",
                url=f"https://example.org/{i}",
            )
            for i in range(10)
        ]

        # When: open with default top_n
        BrowserTabOpener().open(listings)

        # Then: default 5 tabs opened
        assert webbrowser.open.call_count == 5, "Default should open 5 tabs"  # type: ignore[attr-defined]

    @patch("webbrowser.open")
    def test_fewer_results_than_n_opens_all_available_without_error(
        self, mock_open: object
    ) -> None:
        """
        GIVEN fewer results than N
        WHEN tabs are opened with a large top_n
        THEN all available are opened without raising on the shortfall.
        """
        # Given: only 1 listing
        listings = [
            _ranked(title="Only One", url="https://example.org/only"),
        ]

        # When: request 10 tabs
        BrowserTabOpener().open(listings, top_n=10)

        # Then: only 1 tab opened
        assert webbrowser.open.call_count == 1, "Should open only available results"  # type: ignore[attr-defined]

    @patch("webbrowser.open")
    def test_disqualified_roles_are_never_opened_as_tabs(self, mock_open: object) -> None:
        """
        GIVEN a mix of qualified and disqualified listings
        WHEN tabs are opened
        THEN disqualified roles are excluded regardless of position.
        """
        # Given: one qualified, one disqualified
        listings = [
            _ranked(title="Good", final_score=0.80, url="https://example.org/good"),
            _ranked(
                title="Bad",
                final_score=0.0,
                disqualified=True,
                reason="IC role",
                url="https://example.org/bad",
                external_id="ext-002",
            ),
        ]

        # When: open tabs
        BrowserTabOpener().open(listings, top_n=5)

        # Then: only qualified listing opened
        calls = [c.args[0] for c in webbrowser.open.call_args_list]  # type: ignore[attr-defined]
        assert "https://example.org/good" in calls, "Qualified listing should be opened"
        assert "https://example.org/bad" not in calls, "Disqualified listing should not be opened"

    @patch("webbrowser.open", side_effect=[OSError("browser failed"), None])
    def test_failed_tab_open_logs_url_and_continues_to_next(
        self, mock_open: object, caplog: pytest.LogCaptureFixture
    ) -> None:
        """
        GIVEN the first tab open fails with OSError
        WHEN tabs are opened
        THEN the failed URL is logged and the next tab still opens.
        """
        # Given: two listings, first will fail
        listings = [
            _ranked(title="Fail", final_score=0.90, url="https://example.org/fail"),
            _ranked(
                title="OK", final_score=0.80, url="https://example.org/ok", external_id="ext-002"
            ),
        ]

        # When: open tabs (first fails)
        with caplog.at_level(logging.WARNING):
            BrowserTabOpener().open(listings, top_n=2)

        # Then: failed URL is logged
        assert "example.org/fail" in caplog.text, "Failed URL should be logged"

    @patch("webbrowser.open")
    def test_zero_scored_results_opens_no_tabs_and_logs_advisory(
        self, mock_open: object, caplog: pytest.LogCaptureFixture
    ) -> None:
        """
        GIVEN all results score zero (disqualified)
        WHEN tabs are opened
        THEN no tabs open and an advisory message is logged.
        """
        # Given: only disqualified listings
        listings = [
            _ranked(title="Zero", final_score=0.0, disqualified=True, reason="IC role"),
        ]

        # When: attempt to open tabs
        with caplog.at_level(logging.INFO):
            BrowserTabOpener().open(listings, top_n=5)

        # Then: no tabs opened, advisory logged
        assert webbrowser.open.call_count == 0, "No tabs should open for zero-scored results"  # type: ignore[attr-defined]
        assert "no" in caplog.text.lower() or "0" in caplog.text, "Advisory should be logged"

    @patch("webbrowser.open")
    def test_tabs_use_default_system_browser_not_playwright_session(
        self, mock_open: object
    ) -> None:
        """
        GIVEN a listing
        WHEN a tab is opened
        THEN it opens in the system browser, not the Playwright automation session.
        """
        # Given: a single listing
        listings = [_ranked()]

        # When: open 1 tab
        BrowserTabOpener().open(listings, top_n=1)

        # Then: webbrowser.open was called (not Playwright)
        webbrowser.open.assert_called_once()  # type: ignore[attr-defined]

    @patch("webbrowser.open")
    def test_open_top_zero_opens_no_tabs_without_error(self, mock_open: object) -> None:
        """
        GIVEN a listing
        WHEN --open-top 0 is specified
        THEN no tabs are opened and no error is raised.
        """
        # Given: a listing
        listings = [_ranked()]

        # When: open with top_n=0
        BrowserTabOpener().open(listings, top_n=0)

        # Then: no tabs opened
        assert webbrowser.open.call_count == 0, "top_n=0 should skip tab opening"  # type: ignore[attr-defined]


# ---------------------------------------------------------------------------
# JD File Export
# ---------------------------------------------------------------------------


class TestJDFileExport:
    """
    REQUIREMENT: Individual JD files are exported for standalone review.

    WHO:  Operator reviewing JDs with external tools (e.g. Edge Copilot)
    WHAT: (1) The system creates a separate Markdown file for each qualified listing it exports.
          (2) The system names each exported JD file in `NNN_company_title.md` format for natural sort order.
          (3) The system includes the listing title plus company, URL, and score information in the exported file header.
          (4) The system places the full job description text after the score section in the exported file.
          (5) The system skips listings with empty full_text and creates no JD file for them.
          (6) The system excludes disqualified listings with a final score of 0 from JD file export.
          (7) The system assigns file numbers by descending final score instead of insertion order.
          (8) The system creates the output directory automatically when it does not already exist.
          (9) The system shows final, fit, archetype, history, and compensation scores in the score section.
          (10) The system includes an `Also on:` line that lists duplicate boards in the exported JD file.
          (11) The system includes a `Disqualified:` line with the disqualification reason in the exported JD file.
    WHY:  Standalone files are easier to feed to AI assistants for red-flag
          analysis than a single large table

    MOCK BOUNDARY:
        Mock: nothing — pure file I/O via tmp_path
        Real: JDFileExporter.export(), file naming, sorting, filtering
        Never: Patch file writing internals
    """

    def test_creates_individual_files_for_each_listing(self, tmp_path: Path) -> None:
        """
        GIVEN multiple qualified listings
        WHEN exported as JD files
        THEN each listing gets its own Markdown file.
        """
        # Given: two listings
        listings = [
            _ranked(title="Staff Architect", company="Acme Corp", final_score=0.80),
            _ranked(
                title="Principal Engineer",
                company="Beta Inc",
                final_score=0.75,
                external_id="ext-002",
                url="https://example.org/job/2",
            ),
        ]

        # When: export JD files
        paths = JDFileExporter().export(listings, str(tmp_path))

        # Then: one file per listing
        assert len(paths) == 2, "Should create one file per listing"
        assert all(p.exists() for p in paths), "All files should exist"
        assert all(p.suffix == ".md" for p in paths), "All files should be Markdown"

    def test_files_are_named_by_rank_company_title(self, tmp_path: Path) -> None:
        """
        GIVEN a listing
        WHEN exported as a JD file
        THEN the filename follows NNN_company_title.md format for natural sort order.
        """
        # Given: a single listing
        listings = [
            _ranked(title="Staff Architect", company="Acme Corp", final_score=0.80),
        ]

        # When: export JD files
        paths = JDFileExporter().export(listings, str(tmp_path))

        # Then: filename follows naming convention
        name = paths[0].name
        assert name.startswith("001_"), "Filename should start with rank number"
        assert "acme-corp" in name, "Filename should contain slugified company"
        assert "staff-architect" in name, "Filename should contain slugified title"

    def test_file_contains_metadata_header(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with specific metadata
        WHEN exported as a JD file
        THEN the file includes company, URL, and score section.
        """
        # Given: a listing
        listings = [
            _ranked(
                title="Staff Architect",
                company="Acme Corp",
                url="https://example.org/job/1",
            ),
        ]

        # When: export JD files
        JDFileExporter().export(listings, str(tmp_path))
        content = (tmp_path / "001_acme-corp_staff-architect.md").read_text()

        # Then: metadata header present
        assert "# Staff Architect" in content, "Title heading should appear"
        assert "**Company:** Acme Corp" in content, "Company should appear"
        assert "**URL:** https://example.org/job/1" in content, "URL should appear"
        assert "## Score" in content, "Score section should appear"
        assert "**Rank:** #1" in content, "Rank should appear"

    def test_file_contains_full_jd_text(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with full_text
        WHEN exported as a JD file
        THEN the full job description text appears after the score section.
        """
        # Given: a listing with full_text
        listings = [_ranked()]

        # When: export JD files
        JDFileExporter().export(listings, str(tmp_path))
        files = list(tmp_path.glob("*.md"))
        content = files[0].read_text()

        # Then: JD text appears
        assert "## Job Description" in content, "JD section header should appear"
        assert "Build distributed systems." in content, "Full JD text should appear"

    def test_skips_listings_without_full_text(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with empty full_text
        WHEN exported as JD files
        THEN the listing is skipped and no file is created.
        """
        # Given: listing with empty full_text
        listing = _ranked(title="Empty JD")
        listing.listing.full_text = ""

        # When: export JD files
        paths = JDFileExporter().export([listing], str(tmp_path))

        # Then: no files created
        assert len(paths) == 0, "Listings without full_text should be skipped"

    def test_excludes_disqualified_listings(self, tmp_path: Path) -> None:
        """
        GIVEN a mix of qualified and disqualified listings
        WHEN exported as JD files
        THEN disqualified listings with final_score=0 are excluded.
        """
        # Given: one qualified, one disqualified
        listings = [
            _ranked(title="Good Role", final_score=0.80),
            _ranked(
                title="Bad Role",
                final_score=0.0,
                disqualified=True,
                reason="Location mismatch",
                external_id="ext-003",
                url="https://example.org/job/3",
            ),
        ]

        # When: export JD files
        paths = JDFileExporter().export(listings, str(tmp_path))

        # Then: only qualified listing exported
        assert len(paths) == 1, "Only qualified listings should be exported"
        assert "good-role" in paths[0].name, "Qualified listing should be exported"

    def test_sorts_by_final_score_descending(self, tmp_path: Path) -> None:
        """
        GIVEN listings with different scores
        WHEN exported as JD files
        THEN files are numbered by score rank, not insertion order.
        """
        # Given: two listings, lower-scored first
        listings = [
            _ranked(
                title="Lower Score",
                company="Beta",
                final_score=0.60,
                external_id="ext-002",
                url="https://example.org/job/2",
            ),
            _ranked(title="Higher Score", company="Alpha", final_score=0.80),
        ]

        # When: export JD files
        paths = JDFileExporter().export(listings, str(tmp_path))

        # Then: higher score gets rank 001
        assert "001_" in paths[0].name and "higher-score" in paths[0].name, (
            "Higher score should be ranked first"
        )
        assert "002_" in paths[1].name and "lower-score" in paths[1].name, (
            "Lower score should be ranked second"
        )

    def test_creates_output_directory_if_missing(self, tmp_path: Path) -> None:
        """
        GIVEN a nested output directory that doesn't exist
        WHEN JD files are exported
        THEN the directory is created automatically.
        """
        # Given: a non-existent nested directory
        nested = tmp_path / "deep" / "nested" / "jds"
        listings = [_ranked()]

        # When: export JD files
        paths = JDFileExporter().export(listings, str(nested))

        # Then: directory created and file exists
        assert len(paths) == 1, "Should create one file"
        assert nested.exists(), "Output directory should be created"

    def test_score_section_includes_all_components(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with specific component scores
        WHEN exported as a JD file
        THEN the score section shows fit, archetype, history, and comp scores.
        """
        # Given: listing with known scores
        listings = [
            _ranked(fit=0.74, archetype=0.81, history=0.62, comp=0.90, final_score=0.78),
        ]

        # When: export JD files
        JDFileExporter().export(listings, str(tmp_path))
        files = list(tmp_path.glob("*.md"))
        content = files[0].read_text()

        # Then: all component scores appear
        assert "**Final Score:** 0.78" in content, "Final score should appear"
        assert "**Fit Score:** 0.74" in content, "Fit score should appear"
        assert "**Archetype Score:** 0.81" in content, "Archetype score should appear"
        assert "**History Score:** 0.62" in content, "History score should appear"
        assert "**Comp Score:** 0.90" in content, "Comp score should appear"

    def test_duplicate_boards_shown_in_jd_file(self, tmp_path: Path) -> None:
        """
        GIVEN a listing with duplicate_boards populated
        WHEN the JD file is exported
        THEN the file contains an 'Also on:' line listing the other boards.
        """
        # Given: listing with duplicate boards
        listings = [
            _ranked(
                title="Staff Architect",
                company="Acme Corp",
                final_score=0.80,
                duplicate_boards=["indeed", "linkedin"],
            ),
        ]

        # When: export JD files
        JDFileExporter().export(listings, str(tmp_path))
        files = list(tmp_path.glob("*.md"))
        content = files[0].read_text()

        # Then: duplicate boards shown
        assert "**Also on:** indeed, linkedin" in content, (
            "Duplicate boards should appear in JD file"
        )

    def test_disqualified_listing_shows_reason_in_jd_file(self, tmp_path: Path) -> None:
        """
        GIVEN a listing that is disqualified but has a non-zero final score
        WHEN the JD file is exported
        THEN the file contains a 'Disqualified:' line with the reason.
        """
        # Given: disqualified listing with non-zero score
        listings = [
            _ranked(
                title="Backend Engineer",
                company="Acme Corp",
                final_score=0.40,
                disqualified=True,
                reason="lacks cloud experience",
            ),
        ]

        # When: export JD files
        JDFileExporter().export(listings, str(tmp_path))
        files = list(tmp_path.glob("*.md"))
        content = files[0].read_text()

        # Then: disqualification reason shown
        assert "**Disqualified:** lacks cloud experience" in content, (
            "Disqualification reason should appear in JD file"
        )
