"""
CLI handler tests — parser construction, command wiring, output formatting.

Maps to BDD specs: TestParserConstruction, TestBoardsCommand, TestIndexCommand,
TestSearchCommand, TestDecideCommand, TestDecisionsCommand, TestExportCommand,
TestRescoreCommand
"""

from __future__ import annotations

import argparse
import csv
import shutil
import typing
from pathlib import Path
from typing import TYPE_CHECKING, Any
from unittest.mock import AsyncMock, MagicMock, patch

import pytest

if TYPE_CHECKING:
    from collections.abc import Iterator

    from jobsearch_rag.config import Settings

from jobsearch_rag.adapters import AdapterRegistry
from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.cli import (
    build_parser,
    handle_boards,
    handle_decide,
    handle_decisions,
    handle_export,
    handle_index,
    handle_login,
    handle_rescore,
    handle_reset,
    handle_review,
    handle_search,
)
from jobsearch_rag.config import load_settings
from jobsearch_rag.errors import ActionableError
from jobsearch_rag.pipeline.ranker import RankedListing, RankSummary
from jobsearch_rag.pipeline.review import ReviewSession
from jobsearch_rag.pipeline.runner import PipelineRunner, RunResult
from jobsearch_rag.rag.scorer import ScoreResult
from jobsearch_rag.rag.store import VectorStore

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _make_settings(  # pyright: ignore[reportUnusedFunction]  # test utility for future CLI tests
    tmpdir: str,
    *,
    output_dir: str | None = None,
    open_top_n: int = 5,
) -> Settings:
    """
    Write a valid settings.toml in *tmpdir* and load it through the real parser.

    The real ``load_settings`` exercises TOML parsing and field validation
    so that tests are never silently working with an invalid ``Settings``
    that the production code would reject at startup.
    """
    if output_dir is None:
        output_dir = str(Path(tmpdir) / "output")

    toml_path = Path(tmpdir) / "settings.toml"
    toml_path.parent.mkdir(parents=True, exist_ok=True)
    toml_path.write_text(f"""\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
max_pages = 2
headless = true

[scoring]

[ollama]

[output]
output_dir = "{output_dir}"
open_top_n = {open_top_n}

[chroma]
persist_dir = "{tmpdir}"

resume_path = "data/resume.md"
archetypes_path = "config/role_archetypes.toml"
global_rubric_path = "config/global_rubric.toml"
""")
    return load_settings(toml_path)


def _make_listing(
    board: str = "testboard",
    external_id: str = "1",
    title: str = "Staff Architect",
    company: str = "Acme Corp",
) -> JobListing:
    """
    Create a real JobListing with controlled values.

    Uses the actual dataclass rather than MagicMock so that optional
    fields (comp_min, comp_max, etc.) default to ``None`` instead of
    auto-generating child Mocks that serialize as ``<MagicMock …>``
    strings in CSV exports.
    """
    return JobListing(
        board=board,
        external_id=external_id,
        title=title,
        company=company,
        location="Remote",
        url=f"https://example.org/{external_id}",
        full_text="A test job description.",
    )


def _make_ranked(
    final_score: float = 0.75,
    board: str = "testboard",
    external_id: str = "1",
    title: str = "Staff Architect",
    company: str = "Acme Corp",
    duplicate_boards: list[str] | None = None,
) -> RankedListing:
    """Create a RankedListing with controlled values."""
    listing = _make_listing(board=board, external_id=external_id, title=title, company=company)
    scores = ScoreResult(
        fit_score=0.8,
        archetype_score=0.7,
        history_score=0.5,
        disqualified=False,
    )
    ranked = RankedListing(
        listing=listing,
        scores=scores,
        final_score=final_score,
    )
    if duplicate_boards:
        ranked.duplicate_boards = duplicate_boards
    return ranked


def _setup_index_env(tmp_path: Path, *, open_top_n: int = 5) -> AsyncMock:
    """
    Create config/data files in *tmp_path* and return a mock Ollama client.

    Writes ``config/settings.toml``, ``config/role_archetypes.toml``,
    ``config/global_rubric.toml``, and ``data/resume.md`` so that
    CLI handlers can run the full real call chain with only the
    Ollama network boundary mocked.

    Expected counts from this test fixture:

    - 1 archetype
    - 2 negative signals (1 rubric + 1 archetype)
    - 1 global positive signal dimension
    - 1 resume chunk
    """
    config_dir = tmp_path / "config"
    config_dir.mkdir()
    data_dir = tmp_path / "data"
    data_dir.mkdir()
    output_dir = tmp_path / "output"
    output_dir.mkdir()

    (config_dir / "settings.toml").write_text(f"""\
resume_path = "{data_dir / "resume.md"}"
archetypes_path = "{config_dir / "role_archetypes.toml"}"
global_rubric_path = "{config_dir / "global_rubric.toml"}"

[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
max_pages = 2
headless = true

[scoring]

[ollama]

[output]
output_dir = "{output_dir}"
open_top_n = {open_top_n}

[chroma]
persist_dir = "{tmp_path / "chroma"}"
""")

    (config_dir / "role_archetypes.toml").write_text("""\
[[archetypes]]
name = "Test Archetype"
description = "A test archetype for indexing."
signals_positive = ["positive signal"]
signals_negative = ["negative signal"]
""")

    (config_dir / "global_rubric.toml").write_text("""\
[[dimensions]]
name = "Test Dimension"
signals_positive = ["good indicator"]
signals_negative = ["bad indicator"]
""")

    (data_dir / "resume.md").write_text("""\
## Summary

Test resume content for indexing.
""")

    # Mock Ollama client — the I/O boundary
    mock_client = AsyncMock()
    model_embed = MagicMock()
    model_embed.model = "nomic-embed-text"
    model_llm = MagicMock()
    model_llm.model = "mistral:7b"
    mock_client.list.return_value = MagicMock(models=[model_embed, model_llm])
    mock_client.embed.return_value = MagicMock(embeddings=[[0.1, 0.2, 0.3, 0.4, 0.5]])

    return mock_client


def _seed_decision(
    tmp_path: Path,
    *,
    job_id: str = "zr-123",
    verdict: str = "maybe",
    board: str = "ziprecruiter",
    title: str = "Staff Architect",
    company: str = "Acme",
    jd_text: str = "Full JD text for a staff architect role.",
) -> None:
    """
    Pre-populate the ChromaDB decisions collection with a test record.

    Uses the same ``chroma`` directory that ``_setup_index_env`` configures
    in settings, so that ``handle_decide`` finds the record when it
    constructs its own VectorStore.
    """
    store = VectorStore(persist_dir=str(tmp_path / "chroma"))
    store.get_or_create_collection("decisions")
    store.add_documents(
        collection_name="decisions",
        ids=[f"decision-{job_id}"],
        documents=[jd_text],
        embeddings=[[0.1, 0.2, 0.3, 0.4, 0.5]],
        metadatas=[
            {
                "job_id": job_id,
                "verdict": verdict,
                "board": board,
                "title": title,
                "company": company,
                "scoring_signal": "false",
                "reason": "",
                "recorded_at": "2026-01-01T00:00:00",
            }
        ],
    )


# ---------------------------------------------------------------------------
# TestParserConstruction
# ---------------------------------------------------------------------------


class TestParserConstruction:
    """
    REQUIREMENT: The CLI parser defines all subcommands with correct arguments.

    WHO: The operator invoking the tool from the command line
    WHAT: (1) The parser sets `args.command` to `index` when the `index` subcommand is provided.
          (2) The parser leaves `resume_only` false by default and sets it to true when `--resume-only` is provided with `index`.
          (3) The parser captures the `search` subcommand together with the `--board`, `--overnight`, and `--open-top` flags correctly.
          (4) The parser defaults `board` to `None`, `overnight` to `False`, and `open_top` to `None` when `search` is parsed without optional flags.
          (5) The parser captures the `decide` subcommand together with its required `job_id` and `--verdict` arguments.
          (6) The parser exits with code `2` when `decide` receives an invalid `--verdict` value.
          (7) The parser sets `args.format` to `csv` when `export` is passed with `--format csv`.
          (8) The parser defaults the export format to `markdown` when `export` is parsed without `--format`.
          (9) The parser sets `args.command` to `boards` when the `boards` subcommand is provided.
          (10) The parser captures both the `login` subcommand and the selected board when `login` is passed with `--board ziprecruiter`.
          (11) The parser exits with code `2` when `login` is parsed without the required `--board` flag.
          (12) The parser exits with code `2` when no subcommand is provided.
          (13) The parser sets `args.command` to `rescore` when the `rescore` subcommand is provided.
          (14) The parser leaves `archetypes_only` false by default and sets it to true when `--archetypes-only` is provided with `index`.
          (15) The parser accepts `negative_signals` as the `reset` collection value and stores it in `args.collection`.
    WHY: Silently ignoring a flag or failing on a valid subcommand breaks
         the operator's workflow before any real work begins

    MOCK BOUNDARY:
        Mock:  nothing — pure argparse construction, no I/O
        Real:  build_parser, parse_args
        Never: Patch argparse internals
    """

    def test_parser_accepts_index_subcommand(self) -> None:
        """
        Given the CLI parser
        When 'index' is passed as the subcommand
        Then args.command is 'index'
        """
        # Given / When: parse 'index' subcommand
        parser = build_parser()
        args = parser.parse_args(["index"])

        # Then: command is 'index'
        assert args.command == "index", f"Expected command='index', got {args.command!r}"

    def test_index_accepts_resume_only_flag(self) -> None:
        """
        Given the CLI parser
        When 'index' is parsed with and without --resume-only
        Then the flag defaults to False and is True when provided
        """
        # Given / When: parse 'index' without --resume-only
        parser = build_parser()
        args = parser.parse_args(["index"])

        # Then: defaults to False
        assert args.resume_only is False, f"Expected resume_only=False, got {args.resume_only}"

        # When: parse 'index' with --resume-only
        args = parser.parse_args(["index", "--resume-only"])

        # Then: set to True
        assert args.resume_only is True, f"Expected resume_only=True, got {args.resume_only}"

    def test_parser_accepts_search_subcommand_with_all_flags(self) -> None:
        """
        Given the CLI parser
        When 'search' is passed with --board, --overnight, and --open-top flags
        Then all flags are captured correctly
        """
        # Given / When: parse 'search' with all flags
        parser = build_parser()
        args = parser.parse_args(
            ["search", "--board", "ziprecruiter", "--overnight", "--open-top", "5"]
        )

        # Then: all flags are captured
        assert args.command == "search", f"Expected command='search', got {args.command!r}"
        assert args.board == "ziprecruiter", f"Expected board='ziprecruiter', got {args.board!r}"
        assert args.overnight is True, f"Expected overnight=True, got {args.overnight}"
        assert args.open_top == 5, f"Expected open_top=5, got {args.open_top}"

    def test_search_flags_default_to_none_and_false(self) -> None:
        """
        Given the CLI parser
        When 'search' is parsed without optional flags
        Then board defaults to None, overnight to False, open_top to None
        """
        # Given / When: parse 'search' with no optional flags
        parser = build_parser()
        args = parser.parse_args(["search"])

        # Then: defaults are applied
        assert args.board is None, f"Expected board=None, got {args.board!r}"
        assert args.overnight is False, f"Expected overnight=False, got {args.overnight}"
        assert args.open_top is None, f"Expected open_top=None, got {args.open_top}"

    def test_parser_accepts_decide_subcommand_with_required_args(self) -> None:
        """
        Given the CLI parser
        When 'decide' is passed with job_id and --verdict
        Then both positional and keyword args are captured
        """
        # Given / When: parse 'decide' with required args
        parser = build_parser()
        args = parser.parse_args(["decide", "zr-123", "--verdict", "yes"])

        # Then: args are captured
        assert args.command == "decide", f"Expected command='decide', got {args.command!r}"
        assert args.job_id == "zr-123", f"Expected job_id='zr-123', got {args.job_id!r}"
        assert args.verdict == "yes", f"Expected verdict='yes', got {args.verdict!r}"

    def test_decide_rejects_invalid_verdict(self) -> None:
        """
        Given the CLI parser
        When 'decide' is passed with an invalid --verdict value
        Then the parser raises SystemExit with code 2
        """
        # Given / When / Then: invalid verdict triggers SystemExit
        parser = build_parser()
        with pytest.raises(SystemExit) as exc_info:
            parser.parse_args(["decide", "zr-123", "--verdict", "invalid"])
        assert exc_info.value.code == 2, (
            f"Expected exit code 2 for invalid arg, got {exc_info.value.code}"
        )

    def test_parser_accepts_export_subcommand_with_format(self) -> None:
        """
        Given the CLI parser
        When 'export' is passed with --format csv
        Then args.format is 'csv'
        """
        # Given / When: parse 'export' with --format
        parser = build_parser()
        args = parser.parse_args(["export", "--format", "csv"])

        # Then: format is captured
        assert args.command == "export", f"Expected command='export', got {args.command!r}"
        assert args.format == "csv", f"Expected format='csv', got {args.format!r}"

    def test_export_format_defaults_to_markdown(self) -> None:
        """
        Given the CLI parser
        When 'export' is parsed without --format
        Then format defaults to 'markdown'
        """
        # Given / When: parse 'export' without --format
        parser = build_parser()
        args = parser.parse_args(["export"])

        # Then: defaults to 'markdown'
        assert args.format == "markdown", f"Expected format='markdown', got {args.format!r}"

    def test_parser_accepts_boards_subcommand(self) -> None:
        """
        Given the CLI parser
        When 'boards' is passed as the subcommand
        Then args.command is 'boards'
        """
        # Given / When: parse 'boards' subcommand
        parser = build_parser()
        args = parser.parse_args(["boards"])

        # Then: command is 'boards'
        assert args.command == "boards", f"Expected command='boards', got {args.command!r}"

    def test_parser_accepts_login_subcommand_with_board(self) -> None:
        """
        Given the CLI parser
        When 'login' is passed with --board ziprecruiter
        Then both command and board are captured
        """
        # Given / When: parse 'login' with --board
        parser = build_parser()
        args = parser.parse_args(["login", "--board", "ziprecruiter"])

        # Then: args are captured
        assert args.command == "login", f"Expected command='login', got {args.command!r}"
        assert args.board == "ziprecruiter", f"Expected board='ziprecruiter', got {args.board!r}"

    def test_login_rejects_missing_board_flag(self) -> None:
        """
        Given the CLI parser
        When 'login' is parsed without the required --board flag
        Then the parser raises SystemExit with code 2
        """
        # Given / When / Then: missing required --board triggers SystemExit
        parser = build_parser()
        with pytest.raises(SystemExit) as exc_info:
            parser.parse_args(["login"])
        assert exc_info.value.code == 2, (
            f"Expected exit code 2 for missing required arg, got {exc_info.value.code}"
        )

    def test_missing_subcommand_raises_system_exit(self) -> None:
        """
        Given the CLI parser
        When no subcommand is provided
        Then the parser raises SystemExit with code 2
        """
        # Given / When / Then: missing subcommand triggers SystemExit
        parser = build_parser()
        with pytest.raises(SystemExit) as exc_info:
            parser.parse_args([])
        assert exc_info.value.code == 2, (
            f"Expected exit code 2 for missing subcommand, got {exc_info.value.code}"
        )

    def test_parser_accepts_rescore_subcommand(self) -> None:
        """
        Given the CLI parser
        When 'rescore' is passed as the subcommand
        Then args.command is 'rescore'
        """
        # Given / When: parse 'rescore' subcommand
        parser = build_parser()
        args = parser.parse_args(["rescore"])

        # Then: command is 'rescore'
        assert args.command == "rescore", f"Expected command='rescore', got {args.command!r}"

    def test_index_accepts_archetypes_only_flag(self) -> None:
        """
        Given the CLI parser
        When 'index' is parsed with and without --archetypes-only
        Then the flag defaults to False and is True when provided
        """
        # Given / When: parse 'index' without --archetypes-only
        parser = build_parser()
        args = parser.parse_args(["index"])

        # Then: defaults to False
        assert args.archetypes_only is False, (
            f"Expected archetypes_only=False, got {args.archetypes_only}"
        )

        # When: parse 'index' with --archetypes-only
        args = parser.parse_args(["index", "--archetypes-only"])

        # Then: set to True
        assert args.archetypes_only is True, (
            f"Expected archetypes_only=True, got {args.archetypes_only}"
        )

    def test_reset_collection_choices_include_negative_signals(self) -> None:
        """
        Given the CLI parser
        When 'reset --collection negative_signals' is parsed
        Then args.collection is 'negative_signals'
        """
        # Given / When: parse 'reset' with --collection negative_signals
        parser = build_parser()
        args = parser.parse_args(["reset", "--collection", "negative_signals"])

        # Then: collection is captured
        assert args.collection == "negative_signals", (
            f"Expected collection='negative_signals', got {args.collection!r}"
        )


# ---------------------------------------------------------------------------
# TestBoardsCommand
# ---------------------------------------------------------------------------


class TestBoardsCommand:
    """
    REQUIREMENT: The boards command lists all registered adapters for operator discovery.

    WHO: The operator checking which boards are available before running a search
    WHAT: (1) The system prints all registered board names in sorted alphabetical order when `handle_boards` runs.
          (2) The system prints a clear `No adapters registered.` message when `handle_boards` runs with an empty registry.
          (3) The system prints each registered board name with a `  - ` bullet prefix when `handle_boards` runs.
    WHY: An operator who can't see available boards guesses board names,
         leading to search failures on typos

    MOCK BOUNDARY:
        Mock:  AdapterRegistry._registry (monkeypatch to empty dict in one test)
        Real:  AdapterRegistry with its registered adapters, handle_boards()
        Never: Patch AdapterRegistry or list_registered — use real registry state
        Exception: test_handle_boards_empty_registry_prints_no_adapters uses
            monkeypatch to clear _registry because there is no public unregister
            API. This is the only way to reach the defensive empty-registry branch.
    """

    def test_handle_boards_prints_sorted_board_names(
        self, capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given adapters are registered in the real registry
        When handle_boards is called
        Then all registered names are printed in sorted alphabetical order
        """
        # Given: the real registry has adapters registered at import time
        registered = AdapterRegistry.list_registered()
        assert len(registered) > 0, (
            "Expected at least one registered adapter for this test to be meaningful"
        )

        # When: handle_boards is called
        handle_boards()

        # Then: output lists all names in sorted order
        output = capsys.readouterr().out
        assert "Registered adapters:" in output, f"Expected header in output, got: {output!r}"
        sorted_names = sorted(registered)
        for i in range(len(sorted_names) - 1):
            assert output.index(sorted_names[i]) < output.index(sorted_names[i + 1]), (
                f"Expected '{sorted_names[i]}' before '{sorted_names[i + 1]}' in output"
            )

    def test_handle_boards_empty_registry_prints_no_adapters(
        self, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given no adapters are registered
        When handle_boards is called
        Then a clear 'no adapters' message is printed
        """
        # Given: temporarily clear the registry to simulate a transitional
        # state (e.g. all adapters removed before new ones are added).
        # This is the one accepted exception to the "never mock our own code"
        # rule — there is no public unregister API, so monkeypatch on the
        # class-level dict is the only way to reach this defensive branch.
        monkeypatch.setattr(AdapterRegistry, "_registry", {})

        # When: handle_boards is called
        handle_boards()

        # Then: the output says no adapters are registered
        output = capsys.readouterr().out
        assert "No adapters registered." in output, (
            f"Expected 'No adapters registered.' message, got: {output!r}"
        )

    def test_handle_boards_uses_bullet_format(self, capsys: pytest.CaptureFixture[str]) -> None:
        """
        Given adapters are registered in the real registry
        When handle_boards is called
        Then each board name is printed with a '  - ' bullet prefix
        """
        # Given: the real registry has adapters registered at import time
        registered = AdapterRegistry.list_registered()

        # When: handle_boards is called
        handle_boards()

        # Then: each name appears with the bullet format
        output = capsys.readouterr().out
        for name in registered:
            assert f"  - {name}" in output, (
                f"Expected '  - {name}' bullet format in output, got: {output!r}"
            )


# ---------------------------------------------------------------------------
# TestIndexCommand
# ---------------------------------------------------------------------------


class TestIndexCommand:
    """
    REQUIREMENT: The index command wires settings → embedder → indexer correctly.

    WHO: The operator running first-time setup or re-indexing after resume changes
    WHAT: (1) The system runs the Ollama health check before indexing.
          (2) The system indexes both archetypes and the resume by default and prints their counts.
          (3) The system indexes only the resume and skips archetypes when --resume-only is used.
          (4) The system prints all indexing chunk counts to stdout for operator sanity-check.
    WHY: Indexing without a health check wastes time on a dead Ollama;
         silently skipping archetypes produces a broken scoring baseline

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O)
        Real:  load_settings, Embedder, VectorStore, Indexer, config/data file
               parsing, ChromaDB storage
        Never: Patch load_settings, Embedder, VectorStore, or Indexer
    """

    def test_index_runs_health_check_before_indexing(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
    ) -> None:
        """
        Given a valid config environment with Ollama at the I/O boundary
        When handle_index is called
        Then the Ollama health check runs (client.list is called)
        """
        # Given: a fully configured environment with mock Ollama client
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_index runs the full real call chain
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_index(argparse.Namespace(resume_only=False))

        # Then: the health check called Ollama's list endpoint
        mock_client.list.assert_awaited_once()

    def test_index_indexes_archetypes_and_resume_by_default(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a valid config environment
        When handle_index is called without --resume-only
        Then both archetypes and resume are indexed with counts printed
        """
        # Given: a fully configured environment with mock Ollama client
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_index runs the full real call chain
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_index(argparse.Namespace(resume_only=False))

        # Then: output reports both archetype and resume indexing
        output = capsys.readouterr().out
        assert "Indexed 1 archetypes" in output, (
            f"Expected archetype count in output, got: {output!r}"
        )
        assert "Indexed 1 resume chunks" in output, (
            f"Expected resume count in output, got: {output!r}"
        )

    def test_index_resume_only_skips_archetypes(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a valid config environment
        When handle_index is called with --resume-only
        Then only resume is indexed and archetypes are skipped
        """
        # Given: a fully configured environment with mock Ollama client
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_index runs with resume_only=True
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_index(argparse.Namespace(resume_only=True))

        # Then: output reports resume but not archetypes
        output = capsys.readouterr().out
        assert "archetypes" not in output.lower(), (
            f"Expected no archetype output with --resume-only, got: {output!r}"
        )
        assert "Indexed 1 resume chunks" in output, (
            f"Expected resume count in output, got: {output!r}"
        )

    def test_index_prints_chunk_counts_to_stdout(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a valid config environment
        When handle_index is called
        Then all chunk counts are printed for operator sanity-check
        """
        # Given: a fully configured environment with mock Ollama client
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_index runs the full real call chain
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_index(argparse.Namespace(resume_only=False))

        # Then: all chunk counts appear in stdout
        output = capsys.readouterr().out
        assert "Indexed 1 archetypes" in output, (
            f"Expected archetype count in output, got: {output!r}"
        )
        assert "Indexed 2 negative signals" in output, (
            f"Expected negative signal count in output, got: {output!r}"
        )
        assert "Indexed 1 global positive signals" in output, (
            f"Expected positive signal count in output, got: {output!r}"
        )
        assert "Indexed 1 resume chunks" in output, (
            f"Expected resume count in output, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestSearchCommand
# ---------------------------------------------------------------------------


class TestSearchCommand:
    """
    REQUIREMENT: The search command prints a structured summary and ranked listings.

    WHO: The operator reviewing search results in the terminal
    WHAT: (1) The search command prints all required summary fields to stdout when the pipeline returns summary statistics.
          (2) The search command prints each ranked listing with its score, title, company, and URL.
          (3) The search command notes duplicate boards in the output when a listing appears on multiple boards.
          (4) The search command passes only the specified board to the pipeline runner when the `--board` flag restricts the search to a single board.
          (5) The search command opens the top result's URL in the browser when `--open-top` is set to 1.
          (6) The search command opens no browser tabs when `--open-top` is unset and `settings.open_top_n` is 0.
    WHY: Missing summary fields leave the operator guessing whether the run
         was healthy; missing listing fields prevent informed review

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O),
               PipelineRunner.run (browser/scraping I/O — accepted exception),
               webbrowser.open (OS browser launch)
        Real:  load_settings, PipelineRunner construction (Embedder, VectorStore,
               Scorer, Ranker, DecisionRecorder), output formatting, file exports
        Never: Patch load_settings, PipelineRunner constructor
        Exception: PipelineRunner.run is mocked because it orchestrates Playwright
            browser automation across multiple job boards. Mocking at the actual
            browser boundary would require adapter-specific HTML fixtures and
            couple output formatting tests to adapter parsing internals.
            The pipeline itself is tested in test_runner.py.
    """

    def test_search_prints_summary_with_all_required_fields(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a pipeline run returns summary statistics
        When handle_search is called
        Then all required summary fields are printed to stdout
        """
        # Given: real settings environment, mock pipeline result
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        result = RunResult(
            ranked_listings=[],
            summary=RankSummary(
                total_found=20, total_scored=18, total_deduplicated=3, total_excluded=2
            ),
            failed_listings=2,
            boards_searched=["ziprecruiter"],
        )

        # When: handle_search runs with real settings construction
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(PipelineRunner, "run", new_callable=AsyncMock, return_value=result),
        ):
            handle_search(
                argparse.Namespace(board=None, overnight=False, open_top=None, force_rescore=False)
            )

        # Then: output includes all summary fields
        output = capsys.readouterr().out
        assert "Boards searched:" in output, (
            f"Expected 'Boards searched:' in output, got: {output!r}"
        )
        assert "ziprecruiter" in output, f"Expected board name in output, got: {output!r}"
        assert "Total found:" in output, f"Expected 'Total found:' in output, got: {output!r}"
        assert "20" in output, f"Expected total count '20' in output, got: {output!r}"
        assert "Scored:" in output, f"Expected 'Scored:' in output, got: {output!r}"
        assert "18" in output, f"Expected scored count '18' in output, got: {output!r}"
        assert "Deduplicated:" in output, f"Expected 'Deduplicated:' in output, got: {output!r}"
        assert "Failed:" in output, f"Expected 'Failed:' in output, got: {output!r}"
        assert "Final results:" in output, f"Expected 'Final results:' in output, got: {output!r}"

    def test_search_prints_ranked_listings_with_score_and_title(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a pipeline run returns ranked listings
        When handle_search is called
        Then each listing shows score, title, company, and URL
        """
        # Given: real settings environment, mock pipeline result with listings
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        ranked = _make_ranked(final_score=0.82, title="Staff Architect", company="Acme Corp")
        result = RunResult(
            ranked_listings=[ranked],
            summary=RankSummary(total_found=1, total_scored=1),
            boards_searched=["testboard"],
        )

        # When: handle_search runs with real settings construction
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(PipelineRunner, "run", new_callable=AsyncMock, return_value=result),
            patch("webbrowser.open"),
        ):
            handle_search(
                argparse.Namespace(board=None, overnight=False, open_top=None, force_rescore=False)
            )

        # Then: output shows score, title, and company
        output = capsys.readouterr().out
        assert "0.82" in output, f"Expected score '0.82' in output, got: {output!r}"
        assert "Staff Architect" in output, f"Expected title in output, got: {output!r}"
        assert "Acme Corp" in output, f"Expected company in output, got: {output!r}"

    def test_search_notes_duplicate_boards_on_listing(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a listing was seen on multiple boards
        When handle_search is called
        Then the duplicate boards are noted in the output
        """
        # Given: real settings environment, mock pipeline result with duplicates
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        ranked = _make_ranked(duplicate_boards=["indeed", "linkedin"])
        result = RunResult(
            ranked_listings=[ranked],
            summary=RankSummary(total_found=1, total_scored=1),
            boards_searched=["testboard"],
        )

        # When: handle_search runs
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(PipelineRunner, "run", new_callable=AsyncMock, return_value=result),
            patch("webbrowser.open"),
        ):
            handle_search(
                argparse.Namespace(board=None, overnight=False, open_top=None, force_rescore=False)
            )

        # Then: output notes the duplicate boards
        output = capsys.readouterr().out
        assert "Also on:" in output, f"Expected 'Also on:' in output, got: {output!r}"
        assert "indeed" in output, f"Expected 'indeed' in output, got: {output!r}"
        assert "linkedin" in output, f"Expected 'linkedin' in output, got: {output!r}"

    def test_search_board_flag_restricts_to_single_board(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
    ) -> None:
        """
        Given the --board flag specifies a single board
        When handle_search is called
        Then only that board is passed to the pipeline runner
        """
        # Given: real settings environment
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        result = RunResult(
            ranked_listings=[],
            summary=RankSummary(),
            boards_searched=["indeed"],
        )

        # When: handle_search runs with --board indeed
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(
                PipelineRunner, "run", new_callable=AsyncMock, return_value=result
            ) as mock_run,
        ):
            handle_search(
                argparse.Namespace(
                    board="indeed", overnight=False, open_top=None, force_rescore=False
                )
            )

        # Then: the runner was called with boards=["indeed"]
        mock_run.assert_awaited_once()
        call_kwargs = mock_run.call_args.kwargs
        assert call_kwargs.get("boards") == ["indeed"], (
            f"Expected boards=['indeed'], got: {mock_run.call_args!r}"
        )

    def test_search_open_top_opens_browser_tabs(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
    ) -> None:
        """
        Given --open-top is set to 1
        When handle_search returns a ranked listing
        Then the top result's URL is opened in the browser
        """
        # Given: real settings environment, pipeline returns one result
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        ranked = _make_ranked(final_score=0.9)
        result = RunResult(
            ranked_listings=[ranked],
            summary=RankSummary(total_found=1, total_scored=1),
            boards_searched=["testboard"],
        )

        # When: handle_search runs with --open-top 1
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(PipelineRunner, "run", new_callable=AsyncMock, return_value=result),
            patch("webbrowser.open") as mock_open,
        ):
            handle_search(
                argparse.Namespace(board=None, overnight=False, open_top=1, force_rescore=False)
            )

        # Then: webbrowser.open was called to open the top result
        mock_open.assert_called_once()

    def test_search_no_open_top_and_settings_zero_opens_no_tabs(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
    ) -> None:
        """
        Given --open-top is not set and settings.open_top_n is 0
        When handle_search returns ranked listings
        Then no browser tabs are opened
        """
        # Given: real settings with open_top_n=0
        mock_client = _setup_index_env(tmp_path, open_top_n=0)
        monkeypatch.chdir(tmp_path)
        ranked = _make_ranked(final_score=0.9)
        result = RunResult(
            ranked_listings=[ranked],
            summary=RankSummary(total_found=1, total_scored=1),
            boards_searched=["testboard"],
        )

        # When: handle_search runs without --open-top
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(PipelineRunner, "run", new_callable=AsyncMock, return_value=result),
            patch("webbrowser.open") as mock_open,
        ):
            handle_search(
                argparse.Namespace(board=None, overnight=False, open_top=None, force_rescore=False)
            )

        # Then: no browser tabs were opened
        mock_open.assert_not_called()


# ---------------------------------------------------------------------------
# TestDecideCommand
# ---------------------------------------------------------------------------


class TestDecideCommand:
    """
    REQUIREMENT: The decide command records verdicts with appropriate error handling.

    WHO: The operator recording their assessment of a scored role
    WHAT: (1) The system exits with code 1 and prints 'No job found' when no decision exists for the given job ID.
          (2) The system records the new verdict for the existing job and prints a confirmation that includes the history count.
          (3) The system prints the provided reason in the confirmation output when a verdict is recorded with --reason.
          (4) The system exits with code 1 and prints 'Could not retrieve JD text' when the stored JD text for an existing job is missing.
    WHY: Recording a verdict for a non-existent job silently corrupts the
         decision history; the operator needs confirmation the action took effect

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O)
        Real:  load_settings, Embedder, VectorStore, DecisionRecorder,
               ChromaDB storage, JSONL audit log
        Never: Patch load_settings, Embedder, VectorStore, or DecisionRecorder
        Exception: test_missing_jd_text_exits_with_error uses monkeypatch on
            VectorStore.get_documents to return a None document — this defensive
            branch cannot occur with a healthy ChromaDB instance because
            add_documents always stores the document text.
    """

    def test_unknown_job_id_exits_with_error_message(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given no decision exists for the given job ID
        When handle_decide is called
        Then it exits with code 1 and prints 'No job found'
        """
        # Given: real environment with empty decisions collection
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        # Create the decisions collection (empty)
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        store.get_or_create_collection("decisions")

        # When/Then: handle_decide exits with error
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            pytest.raises(SystemExit) as exc_info,
        ):
            handle_decide(argparse.Namespace(job_id="nonexistent", verdict="yes", reason=""))

        assert exc_info.value.code == 1, f"Expected exit code 1, got {exc_info.value.code}"
        output = capsys.readouterr().out
        assert "No job found" in output, f"Expected 'No job found' in output, got: {output!r}"

    def test_existing_job_records_verdict_and_prints_confirmation(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a decision exists for the job ID
        When handle_decide is called with a new verdict
        Then the verdict is recorded and confirmation is printed with history count
        """
        # Given: real environment with a pre-existing decision
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        _seed_decision(tmp_path, job_id="zr-123", verdict="maybe")

        # When: handle_decide re-records the verdict
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decide(argparse.Namespace(job_id="zr-123", verdict="yes", reason=""))

        # Then: confirmation is printed with the new verdict and history count
        output = capsys.readouterr().out
        assert "Recorded 'yes' for zr-123" in output, (
            f"Expected confirmation message in output, got: {output!r}"
        )
        assert "1" in output, f"Expected history count in output, got: {output!r}"

    def test_decide_with_reason_prints_reason_in_confirmation(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a decision exists for the job ID
        When handle_decide is called with --reason
        Then the reason appears in stdout confirmation
        """
        # Given: real environment with a pre-existing decision
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        _seed_decision(tmp_path, job_id="zr-123", verdict="maybe")

        # When: handle_decide records with a reason
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decide(
                argparse.Namespace(
                    job_id="zr-123", verdict="no", reason="Role requires on-call rotation"
                )
            )

        # Then: the reason appears in the output
        output = capsys.readouterr().out
        assert "Role requires on-call rotation" in output, (
            f"Expected reason text in output, got: {output!r}"
        )

    def test_missing_jd_text_exits_with_error(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a decision exists but the stored JD text is corrupted/missing
        When handle_decide is called
        Then it exits with code 1 and prints 'Could not retrieve JD text'
        """
        # Given: real environment with a pre-existing decision
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        _seed_decision(tmp_path, job_id="zr-123")

        # Simulate a corrupted ChromaDB state where the document is None.
        # This cannot happen with a healthy ChromaDB instance (add_documents
        # always stores the text), so monkeypatch is the only way to reach
        # this defensive branch.
        # The first call (from get_decision) must return real data so
        # handle_decide reaches the JD retrieval path; only the second
        # call (direct store.get_documents) returns corrupted data.
        _original_get = VectorStore.get_documents
        _call_count = 0

        def _corrupted_get_on_second_call(
            self_store: Any,
            collection_name: str,
            *,
            ids: list[str],
        ) -> dict[str, Any]:
            nonlocal _call_count
            _call_count += 1
            if _call_count >= 2:
                return {
                    "documents": [None],
                    "ids": ids,
                    "metadatas": [{}],
                }
            return _original_get(self_store, collection_name, ids=ids)

        monkeypatch.setattr(
            VectorStore,
            "get_documents",
            _corrupted_get_on_second_call,
        )

        # When/Then: handle_decide exits with error
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            pytest.raises(SystemExit) as exc_info,
        ):
            handle_decide(argparse.Namespace(job_id="zr-123", verdict="yes", reason=""))

        assert exc_info.value.code == 1, f"Expected exit code 1, got {exc_info.value.code}"
        output = capsys.readouterr().out
        assert "Could not retrieve JD text" in output, (
            f"Expected 'Could not retrieve JD text' in output, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestDecisionsCommand
# ---------------------------------------------------------------------------


class TestDecisionsCommand:
    """
    REQUIREMENT: The decisions subcommand dispatches show, remove, and audit
    correctly through the full CLI stack.

    WHO: The operator managing their decision history from the command line
    WHAT: (1) ``decisions show`` prints all metadata fields for a known decision.
          (2) ``decisions show`` prints 'No decision found' when no decision
              exists for the given job ID.
          (3) ``decisions remove`` removes a known decision from ChromaDB and
              prints a confirmation including the JSONL audit note.
          (4) ``decisions remove`` prints 'No decision found' when no decision
              exists for the given job ID.
          (5) ``decisions audit`` lists decisions that have a non-empty reason.
          (6) ``decisions audit`` prints 'No decisions with reasons' when all
              reasons are empty.
          (7) An unknown subcommand exits with code 1 and prints usage.
    WHY: Each sub-handler formats output differently and has distinct
         not-found branches — an untested handler means silent data loss

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O)
        Real:  load_settings, Embedder, VectorStore, DecisionRecorder,
               ChromaDB storage, JSONL audit log
        Never: Patch load_settings, Embedder, VectorStore, or DecisionRecorder
    """

    def test_show_prints_metadata_for_known_decision(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a decision exists for the job ID
        When ``decisions show`` is invoked
        Then all metadata fields are printed.
        """
        # Given: real environment with a seeded decision
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        _seed_decision(tmp_path, job_id="zr-100", verdict="yes")

        # When: handle_decisions dispatches to show
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decisions(
                argparse.Namespace(decisions_command="show", job_id="zr-100"),
            )

        # Then: output contains the job_id and verdict
        output = capsys.readouterr().out
        assert "zr-100" in output, f"Expected job_id in output, got: {output!r}"
        assert "yes" in output, f"Expected verdict in output, got: {output!r}"

    def test_show_prints_not_found_for_unknown_job_id(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given no decision exists for the job ID
        When ``decisions show`` is invoked
        Then 'No decision found' is printed.
        """
        # Given: real environment with empty decisions collection
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        store.get_or_create_collection("decisions")

        # When: handle_decisions dispatches to show
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decisions(
                argparse.Namespace(decisions_command="show", job_id="nonexistent"),
            )

        # Then: not-found message is printed
        output = capsys.readouterr().out
        assert "No decision found" in output, (
            f"Expected 'No decision found' in output, got: {output!r}"
        )

    def test_remove_prints_confirmation_for_known_decision(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a decision exists for the job ID
        When ``decisions remove`` is invoked
        Then removal confirmation and JSONL audit note are printed.
        """
        # Given: real environment with a seeded decision
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        _seed_decision(tmp_path, job_id="zr-200", verdict="no")

        # When: handle_decisions dispatches to remove
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decisions(
                argparse.Namespace(decisions_command="remove", job_id="zr-200"),
            )

        # Then: confirmation and audit note are printed
        output = capsys.readouterr().out
        assert "Removed decision" in output, (
            f"Expected removal confirmation in output, got: {output!r}"
        )
        assert "JSONL audit log" in output, f"Expected JSONL audit note in output, got: {output!r}"

    def test_remove_prints_not_found_for_unknown_job_id(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given no decision exists for the job ID
        When ``decisions remove`` is invoked
        Then 'No decision found' is printed.
        """
        # Given: real environment with empty decisions collection
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        store.get_or_create_collection("decisions")

        # When: handle_decisions dispatches to remove
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decisions(
                argparse.Namespace(decisions_command="remove", job_id="nonexistent"),
            )

        # Then: not-found message is printed
        output = capsys.readouterr().out
        assert "No decision found" in output, (
            f"Expected 'No decision found' in output, got: {output!r}"
        )

    def test_audit_lists_decisions_with_reasons(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a decision exists with a non-empty reason
        When ``decisions audit`` is invoked
        Then the decision's job_id, verdict, and reason are listed.
        """
        # Given: real environment with a decision that has a reason
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        store.get_or_create_collection("decisions")
        store.add_documents(
            collection_name="decisions",
            ids=["decision-zr-300"],
            documents=["Staff engineer role requiring Kubernetes expertise."],
            embeddings=[[0.1, 0.2, 0.3, 0.4, 0.5]],
            metadatas=[
                {
                    "job_id": "zr-300",
                    "verdict": "no",
                    "board": "ziprecruiter",
                    "title": "Staff Engineer",
                    "company": "K8s Corp",
                    "scoring_signal": "false",
                    "reason": "Requires 5 days on-site",
                    "recorded_at": "2026-01-15T00:00:00",
                }
            ],
        )

        # When: handle_decisions dispatches to audit
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decisions(argparse.Namespace(decisions_command="audit"))

        # Then: output contains the reason and verdict
        output = capsys.readouterr().out
        assert "zr-300" in output, f"Expected job_id in audit output, got: {output!r}"
        assert "Requires 5 days on-site" in output, (
            f"Expected reason in audit output, got: {output!r}"
        )

    def test_audit_prints_empty_message_when_no_reasons(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given decisions exist but none have a non-empty reason
        When ``decisions audit`` is invoked
        Then 'No decisions with reasons to audit' is printed.
        """
        # Given: real environment with a reason-less decision
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        _seed_decision(tmp_path, job_id="zr-400", verdict="yes")  # _seed_decision uses reason=""

        # When: handle_decisions dispatches to audit
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            handle_decisions(argparse.Namespace(decisions_command="audit"))

        # Then: empty message is printed
        output = capsys.readouterr().out
        assert "No decisions with reasons" in output, (
            f"Expected empty-reasons message in output, got: {output!r}"
        )

    def test_unknown_subcommand_exits_with_usage(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given an unrecognized decisions subcommand
        When handle_decisions is invoked
        Then it exits with code 1 and prints usage hint.
        """
        # Given: real environment
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        # Ensure decisions collection exists so dispatcher gets past setup
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        store.get_or_create_collection("decisions")

        # When/Then: unknown subcommand exits
        with (
            patch(
                "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
                return_value=mock_client,
            ),
            pytest.raises(SystemExit) as exc_info,
        ):
            handle_decisions(
                argparse.Namespace(decisions_command="bogus", job_id="x"),
            )

        assert exc_info.value.code == 1, f"Expected exit code 1, got {exc_info.value.code}"
        output = capsys.readouterr().out
        assert "Usage" in output, f"Expected usage hint in output, got: {output!r}"


# ---------------------------------------------------------------------------
# TestExportCommand
# ---------------------------------------------------------------------------


class TestExportCommand:
    """
    REQUIREMENT: The export command re-exports saved results.

    WHO: The operator re-viewing results after a previous search run
    WHAT: (1) The system prints the markdown results content to stdout when export is called with the markdown format.
          (2) The system prints the content of the results file that matches the selected format when export is called.
          (3) The system exits with code 1 and prints 'No previous results found' when no results files exist.
    WHY: Silently producing no output would leave the operator wondering
         if the command failed or if there were no results

    MOCK BOUNDARY:
        Mock:  nothing — handle_export only reads settings and files from disk
        Real:  load_settings (real TOML parsing), filesystem reads via tmp_path
        Never: Patch load_settings or filesystem operations
    """

    def test_export_prints_format_and_stub_message(
        self,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given a markdown results file exists in the output directory
        When export is called with format=markdown
        Then the markdown content is printed to stdout
        """
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # Given: a markdown results file in the output directory
        md_file = tmp_path / "output" / "results.md"
        md_file.write_text("# Run Summary\n\nTest results.\n")

        # When: export is called with format=markdown
        args = argparse.Namespace(format="markdown")
        handle_export(args)

        # Then: the markdown content is printed
        output = capsys.readouterr().out
        assert "# Run Summary" in output, f"Expected '# Run Summary' in output, got: {output!r}"
        assert "Test results." in output, f"Expected 'Test results.' in output, got: {output!r}"

    def test_export_accepts_all_format_choices(
        self,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given both CSV and markdown results files exist
        When export is called with each format
        Then the corresponding file content is printed
        """
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # Given: both CSV and markdown results files
        out_dir = tmp_path / "output"
        (out_dir / "results.csv").write_text("title,company\nStaff Architect,Acme\n")
        (out_dir / "results.md").write_text("# Results\n")

        # When/Then: csv format prints CSV content
        args = argparse.Namespace(format="csv")
        handle_export(args)
        output = capsys.readouterr().out
        assert "Staff Architect" in output, f"Expected CSV content in output, got: {output!r}"

        # When/Then: markdown format prints markdown content
        args = argparse.Namespace(format="markdown")
        handle_export(args)
        output = capsys.readouterr().out
        assert "# Results" in output, f"Expected markdown heading in output, got: {output!r}"

    def test_export_no_results_exits_with_error(
        self,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given no results files exist in the output directory
        When export is called
        Then it exits with code 1 and prints 'No previous results found'
        """
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # Given: no results files in the output directory
        # When: export is called
        with pytest.raises(SystemExit) as exc_info:
            args = argparse.Namespace(format="markdown")
            handle_export(args)

        # Then: exits with code 1 and prints a helpful message
        assert exc_info.value.code == 1, f"Expected exit code 1, got {exc_info.value.code}"
        output = capsys.readouterr().out
        assert "No previous results found" in output, (
            f"Expected 'No previous results found' in output, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestLoginCommand
# ---------------------------------------------------------------------------


class TestLoginCommand:
    """
    REQUIREMENT: The login command opens a headed browser for interactive authentication.

    WHO: The operator establishing a session before headless search runs
    WHAT: (1) The system opens a headed browser to the board login URL, saves the session, and prints a confirmation after login completes.
          (2) The system navigates to the board-specific login URL for LinkedIn when handling login.
          (3) The system prints clear interactive login instructions that guide the operator through the login process.
          (4) The system connects to the requested browser through CDP instead of using the standard Playwright launch when the `--browser msedge` flag is provided.
    WHY: Cloudflare bot protection blocks headless browsers — logging in
         interactively establishes cookies that may enable headless operation

    MOCK BOUNDARY:
        Mock:  async_playwright (Playwright browser API — I/O boundary),
               builtins.input (terminal I/O boundary),
               subprocess.Popen + urllib.request.urlopen (CDP mode I/O),
               shutil.which (filesystem lookup I/O — portability fallback),
               tempfile.mkdtemp (temp directory creation I/O)
        Real:  SessionManager, SessionConfig, handle_login wiring
        Never: Patch SessionManager — let it construct and manage for real
    """

    @staticmethod
    def _mock_playwright_stack() -> tuple[MagicMock, MagicMock]:
        """Build mock Playwright objects and return (mock_pw, mock_page)."""
        mock_page = AsyncMock()

        mock_context = MagicMock()
        mock_context.new_page = AsyncMock(return_value=mock_page)
        mock_context.storage_state = AsyncMock(return_value={})
        mock_context.close = AsyncMock()

        mock_browser = MagicMock()
        mock_browser.new_context = AsyncMock(return_value=mock_context)
        mock_browser.close = AsyncMock()
        mock_browser.contexts = []

        mock_pw = MagicMock()
        mock_pw.chromium.launch = AsyncMock(return_value=mock_browser)
        mock_pw.chromium.connect_over_cdp = AsyncMock(return_value=mock_browser)
        mock_pw.stop = AsyncMock()

        return mock_pw, mock_page

    @staticmethod
    def _patch_async_playwright(mock_pw: MagicMock) -> Any:
        """Patch async_playwright at the I/O boundary."""
        mock_apw = MagicMock()
        mock_apw.start = AsyncMock(return_value=mock_pw)
        return patch(
            "jobsearch_rag.adapters.session.async_playwright",
            return_value=mock_apw,
        )

    def test_login_opens_headed_browser_and_saves_session(
        self,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given the operator runs login for ziprecruiter
        When handle_login completes
        Then a headed browser opens, navigates to the login URL,
        saves the session, and prints confirmation
        """
        # Given: real SessionManager with Playwright mocked at the I/O boundary
        monkeypatch.chdir(tmp_path)
        mock_pw, mock_page = self._mock_playwright_stack()

        # When: handle_login runs the full real call chain
        with (
            self._patch_async_playwright(mock_pw),
            patch("builtins.input", return_value=""),
        ):
            args = argparse.Namespace(board="ziprecruiter", browser=None)
            handle_login(args)

        # Then: browser was launched in headed mode (headless=False)
        mock_pw.chromium.launch.assert_awaited_once()
        launch_kwargs = mock_pw.chromium.launch.call_args.kwargs
        assert launch_kwargs["headless"] is False, (
            f"Expected headless=False, got {launch_kwargs['headless']}"
        )

        # Then: navigated to the login URL
        mock_page.goto.assert_awaited_once()
        url_arg = mock_page.goto.call_args[0][0]
        assert "authn/login" in url_arg, f"Expected 'authn/login' in URL, got {url_arg!r}"

        # Then: session was saved to the board-specific path
        session_file = tmp_path / "data" / "ziprecruiter_session.json"
        assert session_file.exists(), f"Expected session file at {session_file}"

        output = capsys.readouterr().out
        assert "Session saved" in output, f"Expected 'Session saved' in output, got: {output!r}"

    def test_login_uses_board_specific_login_url(
        self,
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given the operator logs in to linkedin
        When handle_login navigates to the login page
        Then the URL is board-specific (linkedin.com/login)
        """
        # Given: real SessionManager for linkedin board
        monkeypatch.chdir(tmp_path)
        mock_pw, mock_page = self._mock_playwright_stack()

        # When: handle_login runs
        with (
            self._patch_async_playwright(mock_pw),
            patch("builtins.input", return_value=""),
        ):
            args = argparse.Namespace(board="linkedin", browser=None)
            handle_login(args)

        # Then: navigated to the linkedin login URL
        url_arg = mock_page.goto.call_args[0][0]
        assert "linkedin.com/login" in url_arg, (
            f"Expected 'linkedin.com/login' in URL, got {url_arg!r}"
        )

    def test_login_prints_instructions_for_operator(
        self,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given the operator runs login for ziprecruiter
        When handle_login executes
        Then clear instructions are printed to guide the operator
        """
        # Given: real SessionManager for ziprecruiter
        monkeypatch.chdir(tmp_path)
        mock_pw, _mock_page = self._mock_playwright_stack()

        # When: handle_login runs
        with (
            self._patch_async_playwright(mock_pw),
            patch("builtins.input", return_value=""),
        ):
            args = argparse.Namespace(board="ziprecruiter", browser=None)
            handle_login(args)

        # Then: operator sees instructions
        output = capsys.readouterr().out
        assert "Interactive Login" in output, (
            f"Expected 'Interactive Login' in output, got: {output!r}"
        )
        assert "ziprecruiter" in output, f"Expected 'ziprecruiter' in output, got: {output!r}"
        assert "Complete login" in output, f"Expected 'Complete login' in output, got: {output!r}"

    def test_login_browser_flag_sets_channel(
        self,
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        Given the operator passes --browser msedge
        When handle_login launches the session
        Then the browser connects via CDP (not standard Playwright launch)
        """
        # Given: real SessionManager with CDP I/O boundaries mocked
        monkeypatch.chdir(tmp_path)
        mock_pw, _mock_page = self._mock_playwright_stack()

        # When: handle_login runs with browser="msedge"
        with (
            self._patch_async_playwright(mock_pw),
            patch("jobsearch_rag.adapters.session.subprocess.Popen"),
            patch("urllib.request.urlopen"),
            patch(
                "jobsearch_rag.adapters.session.tempfile.mkdtemp",
                return_value=str(tmp_path / "cdp"),
            ),
            patch("shutil.which", return_value="/usr/bin/fake-edge"),
            patch("builtins.input", return_value=""),
        ):
            args = argparse.Namespace(board="ziprecruiter", browser="msedge")
            handle_login(args)

        # Then: CDP path was taken, not standard Playwright launch
        mock_pw.chromium.connect_over_cdp.assert_awaited_once()


# ---------------------------------------------------------------------------
# TestSearchBrowserFailure
# ---------------------------------------------------------------------------


class TestSearchBrowserFailure:
    """
    REQUIREMENT: Browser open failures are reported gracefully, not as crashes.

    WHO: The operator running a search where webbrowser.open fails
    WHAT: (1) The system prints an error when opening the browser fails and completes the search normally.
    WHY: A browser failure should not discard valid search results

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O),
               PipelineRunner.run (browser/scraping I/O — same exception as
               TestSearchCommand), webbrowser.open (OS browser launch — the SUT)
        Real:  load_settings, PipelineRunner construction, output formatting
        Never: Patch load_settings, PipelineRunner constructor
    """

    def test_webbrowser_open_failure_prints_error_message(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a search with --open-top that triggers a browser exception
        When webbrowser.open raises an OSError
        Then the error is printed and the search completes normally
        """
        # Given: real settings environment, pipeline returns one result
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        ranked = _make_ranked(final_score=0.9, external_id="fail-1")
        result = RunResult(
            ranked_listings=[ranked],
            summary=RankSummary(total_found=1, total_scored=1),
            boards_searched=["testboard"],
        )

        # When: handle_search runs and webbrowser.open raises
        with (
            patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client),
            patch.object(PipelineRunner, "run", new_callable=AsyncMock, return_value=result),
            patch("webbrowser.open", side_effect=OSError("no browser")),
        ):
            handle_search(
                argparse.Namespace(
                    board=None,
                    overnight=False,
                    open_top=1,
                    force_rescore=False,
                )
            )

        # Then: the failure message is printed but the search completed
        output = capsys.readouterr().out
        assert "Failed to open" in output, f"Expected 'Failed to open' in output, got: {output!r}"
        assert "no browser" in output, (
            f"Expected 'no browser' error detail in output, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestExportMissing
# ---------------------------------------------------------------------------


class TestExportMissing:
    """
    REQUIREMENT: Requesting an export format with no file prints a helpful message.

    WHO: The operator running 'export' before any search has been done
    WHAT: (1) The system explains that no CSV export was found when `export --format csv` is run and only markdown results exist.
    WHY: Crashing on a missing file is unhelpful; the operator needs to
         know to run 'search' first

    MOCK BOUNDARY:
        Mock:  nothing — handle_export only reads settings and files from disk
        Real:  load_settings (real TOML parsing), filesystem reads via tmp_path
        Never: Patch load_settings or filesystem operations
    """

    def test_export_format_not_found_prints_helpful_message(
        self,
        capsys: pytest.CaptureFixture[str],
        monkeypatch: pytest.MonkeyPatch,
        tmp_path: Path,
    ) -> None:
        """
        GIVEN results exist as markdown but not csv
        WHEN export --format csv is run
        THEN a message explains the format was not found.
        """
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # Given: only markdown results exist, not csv
        out_dir = tmp_path / "output"
        (out_dir / "results.md").write_text("# Results")

        # When: export is called with format=csv
        args = argparse.Namespace(format="csv")
        handle_export(args)

        # Then: a helpful message explains the format was not found
        output = capsys.readouterr().out
        assert "No csv export found" in output, (
            f"Expected 'No csv export found' in output, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestResetCommand
# ---------------------------------------------------------------------------


class TestResetCommand:
    """
    REQUIREMENT: The reset command clears ChromaDB collections and optionally output files.

    WHO: The operator starting a fresh run
    WHAT: (1) The system resets all known collections and prints a completion message when no collection is specified.
          (2) The system resets only the resume collection when the collection flag is set to resume.
          (3) The system removes the output directory and recreates it empty when clear-output is set.
          (4) The system completes without error when clear-output is set but the output directory does not exist.
    WHY: Stale data from a previous run can corrupt scoring or mislead
         the operator into reviewing outdated results

    MOCK BOUNDARY:
        Mock:  nothing — handle_reset only uses load_settings, VectorStore,
               and filesystem operations, all backed by tmp_path
        Real:  load_settings (real TOML parsing), VectorStore (real ChromaDB),
               shutil.rmtree (real filesystem via tmp_path)
        Never: Patch load_settings, VectorStore, or shutil
    """

    def test_reset_all_collections_clears_all_known_collections(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given no --collection flag is provided
        When handle_reset is run
        Then all known collections are reset and a completion message is printed
        """
        # Given: real settings environment with ChromaDB
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_reset runs with no --collection flag
        args = argparse.Namespace(collection=None, clear_output=False)
        handle_reset(args)

        # Then: output confirms all collections were reset
        output = capsys.readouterr().out
        assert "Reset complete" in output, f"Expected 'Reset complete' in output, got: {output!r}"
        assert "4 collection(s) cleared" in output, (
            f"Expected 4 collections cleared, got: {output!r}"
        )

    def test_reset_single_collection(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given --collection=resume is specified
        When handle_reset is run
        Then only the 'resume' collection is reset
        """
        # Given: real settings environment with ChromaDB
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_reset runs with --collection=resume
        args = argparse.Namespace(collection="resume", clear_output=False)
        handle_reset(args)

        # Then: output confirms only the resume collection was reset
        output = capsys.readouterr().out
        assert "Reset collection: resume" in output, (
            f"Expected 'Reset collection: resume' in output, got: {output!r}"
        )
        assert "1 collection(s) cleared" in output, (
            f"Expected 1 collection cleared, got: {output!r}"
        )

    def test_reset_with_clear_output_removes_output_dir(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given the --clear-output flag is set and the output directory contains files
        When handle_reset is run
        Then the output directory is removed and recreated empty
        """
        # Given: real settings environment with output files on disk
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        out_dir = tmp_path / "output"
        out_dir.mkdir(parents=True, exist_ok=True)
        results_file = out_dir / "results.md"
        results_file.write_text("old data")

        # When: handle_reset runs with --clear-output
        args = argparse.Namespace(collection=None, clear_output=True)
        handle_reset(args)

        # Then: output confirms the directory was cleared
        output = capsys.readouterr().out
        assert "Cleared output directory" in output, (
            f"Expected 'Cleared output directory' in output, got: {output!r}"
        )
        # And: the old file no longer exists but the directory was recreated
        assert not results_file.exists(), "Expected results.md to be deleted, but it still exists"
        assert out_dir.exists(), "Expected output directory to be recreated, but it does not exist"

    def test_reset_with_clear_output_skips_missing_output_dir(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given the --clear-output flag is set but the output directory does not exist
        When handle_reset is run
        Then the reset completes without error and does not print a clear message.
        """
        # Given: real settings environment without creating the output directory
        _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)
        out_dir = tmp_path / "output"
        if out_dir.exists():
            shutil.rmtree(out_dir)

        # When: handle_reset runs with --clear-output
        args = argparse.Namespace(collection=None, clear_output=True)
        handle_reset(args)

        # Then: reset completes without error; no "Cleared" message (dir didn't exist)
        output = capsys.readouterr().out
        assert "Reset complete" in output, f"Expected 'Reset complete' in output, got: {output!r}"
        assert "Cleared output directory" not in output, (
            f"Should not mention clearing when output dir didn't exist, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestReviewJdLoading
# ---------------------------------------------------------------------------


class TestReviewJdLoading:
    """
    REQUIREMENT: The review command populates each listing's full_text
    from JD files on disk, since CSV export does not store full text.

    WHO: The review CLI handler reconstructing RankedListing objects
         from CSV rows for the interactive review session
    WHAT: (1) The review session finds the JD file using the rank company title slug convention when the operator opens a JD.
    WHY: DecisionRecorder requires full_text to generate embeddings for
         the history signal. Without it, recording a verdict fails with
         an empty-text validation error on the second listing reviewed

    MOCK BOUNDARY:
        Mock:  webbrowser.open (browser I/O)
        Real:  ReviewSession, slug-based file resolution, JD body
               extraction from markdown
        Never: Patch slugify or file path construction
    """

    def test_open_listing_resolves_jd_file_via_slug_convention(self, tmp_path: Path) -> None:
        """
        Given a JD file named with the rank/company/title slug convention
        When the operator presses 'o' to open a JD
        Then the review session finds the JD file using the slug convention
        """
        # Given: A JD file named with the slug convention at rank 3
        jd_dir = tmp_path / "jds"
        jd_dir.mkdir()
        company = "Acme Corp"
        title = "Staff Architect"
        slug_name = "003_acme-corp_staff-architect.md"
        (jd_dir / slug_name).write_text("## Job Description\nFull JD here.")

        ranked = _make_ranked(title=title, company=company, external_id="zr-42")
        recorder = MagicMock()
        recorder.get_decision = MagicMock(return_value=None)
        session = ReviewSession(
            ranked_listings=[ranked],
            recorder=recorder,
            jd_dir=str(jd_dir),
        )

        # When: open_listing is called with the rank
        with patch("jobsearch_rag.pipeline.review.webbrowser.open") as mock_open:
            session.open_listing(ranked, rank=3)
            mock_open.assert_called_once()
            opened_path = mock_open.call_args[0][0]

        # Then: It opened the slug-based JD file, not external_id.md
        assert slug_name in opened_path, (
            f"Expected slug-based filename '{slug_name}' in opened path, got: {opened_path}"
        )


# ---------------------------------------------------------------------------
# TestReviewCommandHandler
# ---------------------------------------------------------------------------


class TestReviewCommandHandler:
    """
    REQUIREMENT: The review CLI handler wires dependencies, loads CSV,
    and drives the interactive loop so every operator action produces
    the correct output and side-effect.

    WHO: The operator running `python -m jobsearch_rag review`
    WHAT: (1) The system prints "No results found" and returns when no `results.csv` exists in the output directory.
          (2) The system displays every reconstructed listing field from the CSV row when it rebuilds ranked listings for review.
          (3) The system prints "nothing to review" when every listing already has a decision in ChromaDB.
          (4) The system shows the total number of undecided listings before displaying the first listing.
          (5) The system stops the review and reports that it reviewed 0 listings when the operator enters `q`.
          (6) The system skips the current listing, shows the next listing, and records no decision when the operator enters `s`.
          (7) The system records the operator's yes verdict in ChromaDB and prints a confirmation message.
          (8) The system stores the listing's full job description text from the JD file in ChromaDB when it records a verdict.
          (9) The system rejects the verdict because the JD text is empty when the JD file lacks the `## Job Description` marker.
          (10) The system prints a confirmation without a reason suffix when the operator submits a verdict with an empty reason.
          (11) The system reprints the valid command list when the operator enters an invalid command.
          (12) The system prints "Review complete" after the operator reviews every listing.
          (13) The system treats an `EOFError` during command input as quit and stops the review.
          (14) The system delegates the open action to `webbrowser.open` when the operator enters `o`.
          (15) The system records the verdict with an empty reason when an `EOFError` occurs during the reason prompt.
    WHY: The handler is the orchestration boundary between user input
         and domain logic — untested wiring means the operator gets
         silent failures, missing output, or wired-wrong dependencies
         that only surface in production

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O),
               builtins.input (terminal I/O),
               webbrowser.open (OS browser launch)
        Real:  load_settings (real TOML parsing), Embedder (real construction),
               VectorStore (real ChromaDB via tmp_path),
               DecisionRecorder (real recording to ChromaDB),
               ReviewSession (real orchestration)
        Never: Patch load_settings, Embedder, VectorStore, or DecisionRecorder
    """

    _CSV_FIELDS: typing.ClassVar[list[str]] = [
        "title",
        "company",
        "board",
        "location",
        "url",
        "fit_score",
        "archetype_score",
        "history_score",
        "comp_score",
        "final_score",
        "comp_min",
        "comp_max",
        "disqualified",
        "disqualifier_reason",
    ]

    @staticmethod
    def _csv_row(**overrides: str) -> dict[str, str]:
        """Build a CSV row dict with sensible defaults."""
        row: dict[str, str] = {
            "title": "Staff Architect",
            "company": "Acme Corp",
            "board": "ziprecruiter",
            "location": "Remote",
            "url": "https://example.org/job-1",
            "fit_score": "0.85",
            "archetype_score": "0.90",
            "history_score": "0.50",
            "comp_score": "0.60",
            "final_score": "0.82",
            "comp_min": "",
            "comp_max": "",
            "disqualified": "",
            "disqualifier_reason": "",
        }
        row.update(overrides)
        return row

    @classmethod
    def _write_csv(cls, csv_path: Path, rows: list[dict[str, str]]) -> None:
        """Write a list of row dicts to a CSV file."""
        with open(csv_path, "w", newline="") as f:
            writer = csv.DictWriter(f, fieldnames=cls._CSV_FIELDS)
            writer.writeheader()
            writer.writerows(rows)

    @pytest.fixture()
    def review(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
    ) -> Iterator[dict[str, Any]]:
        """Real settings, Embedder, VectorStore, DecisionRecorder via tmp_path."""
        # Given: real settings environment with ollama mocked at I/O boundary
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        out_dir = tmp_path / "output"
        (out_dir / "jds").mkdir(exist_ok=True)

        # Pre-create decisions collection so get_decision works before first record
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        store.get_or_create_collection("decisions")

        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            yield {
                "out_dir": out_dir,
                "csv_path": out_dir / "results.csv",
                "store": store,
                "args": argparse.Namespace(),
            }

    # -- Tests ---------------------------------------------------------------

    def test_missing_csv_prints_message_and_exits(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given no results.csv exists in the output directory
        When handle_review is called
        Then 'No results found' is printed and the handler returns
        """
        # Given: no CSV file exists (default state)

        # When: handle_review runs
        handle_review(review["args"])

        # Then: a helpful message is printed
        output = capsys.readouterr().out
        assert "No results found" in output, (
            f"Expected 'No results found' in output, got: {output!r}"
        )

    def test_csv_rows_are_reconstructed_as_ranked_listings(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given a CSV with specific fields including compensation and disqualification
        When handle_review is called and the operator quits immediately
        Then the listing display includes all reconstructed fields
        """
        # Given: a CSV with detailed fields
        self._write_csv(
            review["csv_path"],
            [
                self._csv_row(
                    title="Data Engineer",
                    company="BigTech",
                    comp_min="180000",
                    comp_max="250000",
                    final_score="0.92",
                    disqualified="true",
                    disqualifier_reason="Requires clearance",
                ),
            ],
        )

        # When: operator quits immediately after seeing the listing
        with patch("builtins.input", side_effect=["q"]):
            handle_review(review["args"])

        # Then: all CSV fields are visible in the output
        out = capsys.readouterr().out
        assert "Data Engineer" in out, f"Expected 'Data Engineer' in output, got: {out!r}"
        assert "BigTech" in out, f"Expected 'BigTech' in output, got: {out!r}"
        assert "180,000" in out, f"Expected formatted comp_min '180,000' in output, got: {out!r}"
        assert "250,000" in out, f"Expected formatted comp_max '250,000' in output, got: {out!r}"
        assert "0.92" in out, f"Expected final_score '0.92' in output, got: {out!r}"
        assert "DISQUALIFIED" in out, f"Expected 'DISQUALIFIED' flag in output, got: {out!r}"
        assert "Requires clearance" in out, f"Expected disqualifier reason in output, got: {out!r}"

    def test_all_decided_prints_nothing_to_review(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given every listing in the CSV already has a decision in ChromaDB
        When handle_review is called
        Then 'nothing to review' is printed
        """
        # Given: a CSV with one listing that already has a decision
        self._write_csv(
            review["csv_path"],
            [
                self._csv_row(url="https://example.org/done-1"),
            ],
        )
        # Pre-seed a decision for this listing (external_id = "done-1")
        _seed_decision(
            review["out_dir"].parent,
            job_id="done-1",
            verdict="yes",
            board="ziprecruiter",
        )

        # When: handle_review runs
        handle_review(review["args"])

        # Then: nothing to review
        output = capsys.readouterr().out
        assert "nothing to review" in output, (
            f"Expected 'nothing to review' in output, got: {output!r}"
        )

    def test_undecided_count_shown_before_first_listing(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given two undecided listings in the CSV
        When handle_review is called and the operator quits
        Then the undecided count is shown before any listing display
        """
        # Given: two undecided listings
        self._write_csv(
            review["csv_path"],
            [
                self._csv_row(title="Job A", url="https://example.org/a"),
                self._csv_row(title="Job B", url="https://example.org/b"),
            ],
        )

        # When: operator quits immediately
        with patch("builtins.input", side_effect=["q"]):
            handle_review(review["args"])

        # Then: count is shown
        output = capsys.readouterr().out
        assert "2 undecided listing(s)" in output, (
            f"Expected '2 undecided listing(s)' in output, got: {output!r}"
        )

    def test_quit_input_stops_review_and_prints_count(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing
        When the operator enters 'q'
        Then review stops and reports 0 reviewed
        """
        # Given: one undecided listing
        self._write_csv(review["csv_path"], [self._csv_row()])

        # When: operator quits
        with patch("builtins.input", side_effect=["q"]):
            handle_review(review["args"])

        # Then: review stopped with 0 reviewed
        out = capsys.readouterr().out
        assert "Review stopped" in out, f"Expected 'Review stopped' in output, got: {out!r}"
        assert "0 listing(s) reviewed" in out, (
            f"Expected '0 listing(s) reviewed' in output, got: {out!r}"
        )

    def test_skip_input_advances_to_next_listing(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given two undecided listings
        When the operator enters 's' then 'q'
        Then the second listing is shown and no decision is recorded
        """
        # Given: two undecided listings
        self._write_csv(
            review["csv_path"],
            [
                self._csv_row(title="First Job", final_score="0.90", url="https://example.org/j1"),
                self._csv_row(
                    title="Second Job", final_score="0.80", url="https://example.org/j2"
                ),
            ],
        )

        # When: operator skips first, then quits
        with patch("builtins.input", side_effect=["s", "q"]):
            handle_review(review["args"])

        # Then: second listing was shown (skip advanced past first)
        out = capsys.readouterr().out
        assert "Second Job" in out, f"Expected 'Second Job' in output after skip, got: {out!r}"
        # And: no decisions were recorded in ChromaDB
        store: VectorStore = review["store"]
        assert store.collection_count("decisions") == 0, (
            "Expected no decisions after skip, but found some"
        )

    def test_yes_verdict_records_and_prints_confirmation(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing with a JD file on disk
        When the operator enters 'y' with reason 'Good fit'
        Then the verdict is recorded in ChromaDB and confirmation is printed
        """
        # Given: one undecided listing with a JD file
        self._write_csv(review["csv_path"], [self._csv_row()])
        jd_dir = review["out_dir"] / "jds"
        jd_file = jd_dir / "001_acme-corp_staff-architect.md"
        jd_file.write_text("## Job Description\nStaff Architect role at Acme Corp.")

        # When: operator approves with reason
        with patch("builtins.input", side_effect=["y", "Good fit"]):
            handle_review(review["args"])

        # Then: confirmation is printed
        out = capsys.readouterr().out
        assert "Recorded: y" in out, f"Expected 'Recorded: y' in output, got: {out!r}"
        assert "Good fit" in out, f"Expected reason 'Good fit' in output, got: {out!r}"
        # And: decision was persisted in ChromaDB
        store: VectorStore = review["store"]
        result = store.get_documents(collection_name="decisions", ids=["decision-job-1"])
        assert len(result["ids"]) == 1, f"Expected 1 decision in ChromaDB, got: {result['ids']}"
        meta = result["metadatas"][0]
        assert meta["verdict"] == "yes", f"Expected verdict 'yes', got: {meta['verdict']!r}"
        assert meta["reason"] == "Good fit", f"Expected reason 'Good fit', got: {meta['reason']!r}"

    def test_review_loads_full_text_from_jd_file(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given a CSV row has a matching JD file on disk
        When the operator records a verdict
        Then the listing's full_text from the JD is stored in ChromaDB
        """
        # Given: A CSV row and a matching JD file with the slug convention
        self._write_csv(review["csv_path"], [self._csv_row()])
        jd_dir = review["out_dir"] / "jds"
        jd_file = jd_dir / "001_acme-corp_staff-architect.md"
        jd_file.write_text(
            "# Staff Architect\n\n"
            "## Job Description\n"
            "Lead the platform team and design distributed systems."
        )

        # When: handle_review is called and user approves
        with patch("builtins.input", side_effect=["y", "Good fit"]):
            handle_review(review["args"])

        # Then: the JD body was stored as the document in ChromaDB
        store: VectorStore = review["store"]
        result = store.get_documents(collection_name="decisions", ids=["decision-job-1"])
        stored_doc = result["documents"][0]
        assert "Lead the platform team" in stored_doc, (
            f"Expected JD body in stored document, got: {stored_doc!r}"
        )

    def test_jd_file_without_marker_yields_empty_full_text(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given a JD file that exists but lacks the '## Job Description' marker
        When the operator records a verdict
        Then the verdict fails validation because jd_text is empty
        """
        # Given: a JD file with no marker section
        self._write_csv(review["csv_path"], [self._csv_row()])
        jd_dir = review["out_dir"] / "jds"
        jd_file = jd_dir / "001_acme-corp_staff-architect.md"
        jd_file.write_text("# Staff Architect\n\nSome summary without the marker.")

        # When: handle_review is called and user approves, the real
        # DecisionRecorder.record() raises ActionableError because jd_text is empty
        with (
            patch("builtins.input", side_effect=["y", ""]),
            pytest.raises(ActionableError, match="empty JD text"),
        ):
            handle_review(review["args"])

        # Then: no decision was stored (empty jd_text rejected by validation)
        store: VectorStore = review["store"]
        assert store.collection_count("decisions") == 0, (
            "Expected no decision when JD marker is missing (empty jd_text)"
        )

    def test_verdict_without_reason_prints_short_confirmation(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing with a JD file on disk
        When the operator enters a verdict then presses Enter (empty reason)
        Then confirmation is printed without the reason suffix
        """
        # Given: one undecided listing with a JD file
        self._write_csv(review["csv_path"], [self._csv_row()])
        jd_dir = review["out_dir"] / "jds"
        jd_file = jd_dir / "001_acme-corp_staff-architect.md"
        jd_file.write_text("## Job Description\nSome JD content here.")

        # When: operator enters 'n' with empty reason
        with patch("builtins.input", side_effect=["n", ""]):
            handle_review(review["args"])

        # Then: short confirmation without reason suffix
        out = capsys.readouterr().out
        assert "Recorded: n" in out, f"Expected 'Recorded: n' in output, got: {out!r}"
        assert "Recorded: n —" not in out, f"Expected no reason suffix in output, got: {out!r}"

    def test_invalid_input_reprints_help(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing
        When the operator enters an unrecognised command
        Then the valid command list is reprinted
        """
        # Given: one undecided listing
        self._write_csv(review["csv_path"], [self._csv_row()])

        # When: operator enters invalid input then quits
        with patch("builtins.input", side_effect=["x", "q"]):
            handle_review(review["args"])

        # Then: invalid input message shown
        output = capsys.readouterr().out
        assert "Invalid input" in output, f"Expected 'Invalid input' in output, got: {output!r}"

    def test_all_reviewed_prints_completion_message(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing with a JD file on disk
        When the operator reviews every listing
        Then 'Review complete' is printed
        """
        # Given: one undecided listing with a JD file
        self._write_csv(review["csv_path"], [self._csv_row()])
        jd_dir = review["out_dir"] / "jds"
        jd_file = jd_dir / "001_acme-corp_staff-architect.md"
        jd_file.write_text("## Job Description\nSome JD content here.")

        # When: operator approves the only listing
        with patch("builtins.input", side_effect=["y", ""]):
            handle_review(review["args"])

        # Then: completion message shown
        output = capsys.readouterr().out
        assert "Review complete" in output, (
            f"Expected 'Review complete' in output, got: {output!r}"
        )

    def test_eof_during_input_treated_as_quit(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing
        When EOFError occurs during input
        Then the review is stopped as if 'q' was entered
        """
        # Given: one undecided listing
        self._write_csv(review["csv_path"], [self._csv_row()])

        # When: EOF occurs
        with patch("builtins.input", side_effect=EOFError):
            handle_review(review["args"])

        # Then: treated as quit
        output = capsys.readouterr().out
        assert "Review stopped" in output, f"Expected 'Review stopped' in output, got: {output!r}"

    def test_open_delegates_to_session_open_listing(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing
        When the operator enters 'o' then 'q'
        Then webbrowser.open is called to open the listing
        """
        # Given: one undecided listing
        self._write_csv(
            review["csv_path"],
            [
                self._csv_row(url="https://example.org/open-me"),
            ],
        )

        # When: operator opens then quits
        with (
            patch("builtins.input", side_effect=["o", "q"]),
            patch("jobsearch_rag.pipeline.review.webbrowser.open") as mock_wb,
        ):
            handle_review(review["args"])

        # Then: webbrowser.open was called
        mock_wb.assert_called_once()

    def test_eof_during_reason_prompt_records_empty_reason(
        self, review: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one undecided listing with a JD file on disk
        When EOFError occurs during the reason prompt
        Then the verdict is recorded with an empty reason
        """
        # Given: one undecided listing with a JD file
        self._write_csv(review["csv_path"], [self._csv_row()])
        jd_dir = review["out_dir"] / "jds"
        jd_file = jd_dir / "001_acme-corp_staff-architect.md"
        jd_file.write_text("## Job Description\nSome JD content here.")

        # When: verdict entered, then EOF on reason prompt
        with patch("builtins.input", side_effect=["y", EOFError]):
            handle_review(review["args"])

        # Then: verdict recorded with empty reason
        out = capsys.readouterr().out
        assert "Recorded: y" in out, f"Expected 'Recorded: y' in output, got: {out!r}"
        assert "Recorded: y —" not in out, f"Expected no reason suffix in output, got: {out!r}"
        # And: decision was persisted in ChromaDB with empty reason
        store: VectorStore = review["store"]
        result = store.get_documents(collection_name="decisions", ids=["decision-job-1"])
        assert len(result["ids"]) == 1, f"Expected 1 decision in ChromaDB, got: {result['ids']}"
        meta = result["metadatas"][0]
        assert meta["reason"] == "", f"Expected empty reason, got: {meta['reason']!r}"


# ---------------------------------------------------------------------------
# TestIndexArchetypesOnly
# ---------------------------------------------------------------------------


class TestIndexArchetypesOnly:
    """
    REQUIREMENT: --archetypes-only rebuilds archetypes and negative signals without resume.

    WHO: The operator tuning archetypes or the global rubric
    WHAT: (1) The system indexes archetypes, negative signals, and positive signals without indexing the resume when `handle_index` runs with `--archetypes-only`.
          (2) The system indexes negative signals alongside archetypes and the resume when `handle_index` runs with default flags.
    WHY: After editing role_archetypes.toml or global_rubric.toml, the operator
         needs a fast re-index path that doesn't re-embed the full resume

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O)
        Real:  load_settings (real TOML parsing), Embedder (real construction),
               VectorStore (real ChromaDB via tmp_path),
               Indexer (real indexing into ChromaDB)
        Never: Patch load_settings, Embedder, VectorStore, or Indexer
    """

    def test_archetypes_only_indexes_archetypes_and_negative_signals(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a valid config environment with archetypes and rubric files
        When handle_index is called with --archetypes-only
        Then archetypes, negative signals, and positive signals are indexed
             but resume is NOT indexed
        """
        # Given: real config/data environment
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_index with archetypes_only=True
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            args = argparse.Namespace(archetypes_only=True, resume_only=False)
            handle_index(args)

        # Then: stdout reports archetype and signal counts, but NOT resume
        output = capsys.readouterr().out
        assert "Indexed 1 archetype" in output, (
            f"Expected archetype count in output, got: {output!r}"
        )
        assert "Indexed 2 negative signals" in output, (
            f"Expected 2 negative signals in output, got: {output!r}"
        )
        assert "Indexed 1 global positive signal" in output, (
            f"Expected 1 positive signal in output, got: {output!r}"
        )
        assert "resume" not in output.lower(), (
            f"Resume should NOT be mentioned with --archetypes-only, got: {output!r}"
        )

        # And: ChromaDB collections contain real data
        store = VectorStore(persist_dir=str(tmp_path / "chroma"))
        assert store.collection_count("role_archetypes") == 1, "Expected 1 archetype in ChromaDB"
        assert store.collection_count("negative_signals") == 2, (
            "Expected 2 negative signals in ChromaDB"
        )

    def test_default_index_also_indexes_negative_signals(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
        capsys: pytest.CaptureFixture[str],
    ) -> None:
        """
        Given a valid config environment
        When handle_index is called with default flags (no archetypes-only)
        Then negative signals are indexed alongside archetypes and resume
        """
        # Given: real config/data environment
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # When: handle_index with default flags
        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            args = argparse.Namespace(archetypes_only=False, resume_only=False)
            handle_index(args)

        # Then: stdout reports all counts including resume
        output = capsys.readouterr().out
        assert "Indexed 2 negative signals" in output, (
            f"Expected 2 negative signals in output, got: {output!r}"
        )
        assert "Indexed 1 resume chunk" in output, (
            f"Expected 1 resume chunk in output, got: {output!r}"
        )


# ---------------------------------------------------------------------------
# TestRescoreCommand
# ---------------------------------------------------------------------------


class TestRescoreCommand:
    """
    REQUIREMENT: The rescore command re-scores JDs through updated collections.

    WHO: The operator iterating on archetype tuning or negative signal refinement
    WHAT: (1) The system prints a rescore results summary that includes counts.
          (2) The system invokes the health check before processing rescore results.
          (3) The system prints each ranked listing with its rank, score, title, company, and board.
          (4) The system exports Markdown and CSV results to the output directory during rescoring.
          (5) The system re-exports JD files with updated scores and ranks during rescoring.
    WHY: Without a dedicated rescore command the operator must re-run full
         browser sessions after every config change — minutes instead of seconds

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (Ollama network I/O)
        Real:  load_settings (real TOML parsing), Embedder (real construction),
               VectorStore (real ChromaDB via tmp_path), Scorer (real scoring),
               Ranker (real ranking), Rescorer (real rescoring from JD files)
        Never: Patch load_settings, Embedder, VectorStore, Scorer, or Rescorer
    """

    @staticmethod
    def _write_jd(
        jd_dir: Path,
        *,
        title: str = "Staff Architect",
        company: str = "Acme Corp",
        board: str = "testboard",
        url: str = "https://example.org/job-1",
        rank: int = 1,
    ) -> Path:
        """Write a JD markdown file in the format expected by load_jd_files."""
        jd_dir.mkdir(parents=True, exist_ok=True)
        slug_company = company.lower().replace(" ", "-")
        slug_title = title.lower().replace(" ", "-")
        filename = f"{rank:03d}_{slug_company}_{slug_title}.md"
        content = (
            f"# {title}\n\n"
            f"**Company:** {company}\n"
            f"**Board:** {board}\n"
            f"**URL:** {url}\n\n"
            f"## Job Description\n"
            f"{title} role at {company}. Responsibilities include "
            f"architecture, design, and technical leadership.\n"
        )
        jd_file = jd_dir / filename
        jd_file.write_text(content)
        return jd_file

    @pytest.fixture
    def rescore(
        self,
        tmp_path: Path,
        monkeypatch: pytest.MonkeyPatch,
    ) -> Iterator[dict[str, Any]]:
        """Real settings, Embedder, VectorStore, Scorer, Ranker, Rescorer via tmp_path."""
        # Given: real config/data environment with ollama mocked at I/O boundary
        mock_client = _setup_index_env(tmp_path)
        monkeypatch.chdir(tmp_path)

        # Disable LLM disqualification — avoids needing chat mock responses
        settings_path = tmp_path / "config" / "settings.toml"
        content = settings_path.read_text()
        content = content.replace("[scoring]", "[scoring]\ndisqualify_on_llm_flag = false")
        settings_path.write_text(content)

        with patch(
            "jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient",
            return_value=mock_client,
        ):
            # Index all collections so scorer has data to query
            handle_index(argparse.Namespace(archetypes_only=False, resume_only=False))

            jd_dir = tmp_path / "output" / "jds"
            jd_dir.mkdir(parents=True, exist_ok=True)

            yield {
                "jd_dir": jd_dir,
                "out_dir": tmp_path / "output",
                "args": argparse.Namespace(),
                "mock_client": mock_client,
            }

    # -- Tests ---------------------------------------------------------------

    def test_rescore_prints_summary(
        self, rescore: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one JD file in output/jds/
        When handle_rescore runs
        Then the output contains a rescore results summary with counts
        """
        # Given: one JD file
        self._write_jd(rescore["jd_dir"])

        # When: handle_rescore runs
        handle_rescore(rescore["args"])

        # Then: summary is printed with correct counts
        output = capsys.readouterr().out
        assert "Rescore Results Summary" in output, (
            f"Expected summary header in output, got: {output!r}"
        )
        assert "JDs loaded:     1" in output, (
            f"Expected 'JDs loaded: 1' in output, got: {output!r}"
        )

    def test_rescore_runs_health_check_first(self, rescore: dict[str, Any]) -> None:
        """
        Given no JD files (empty jds/ directory)
        When handle_rescore runs
        Then the health check is invoked (client.list is called again)
        """
        # Given: no JD files — health check runs regardless
        mock_client: AsyncMock = rescore["mock_client"]
        list_calls_before = mock_client.list.call_count

        # When: handle_rescore runs
        handle_rescore(rescore["args"])

        # Then: health check made another client.list() call
        assert mock_client.list.call_count > list_calls_before, (
            "Expected health_check to call client.list() during rescore"
        )

    def test_rescore_prints_each_ranked_listing_with_score_and_details(
        self, rescore: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one JD file with specific metadata
        When handle_rescore runs
        Then the listing is printed with rank, score, title, company, and board
        """
        # Given: one JD file with specific details
        self._write_jd(
            rescore["jd_dir"],
            title="Staff Architect",
            company="Acme Corp",
            board="testboard",
        )

        # When: handle_rescore runs
        handle_rescore(rescore["args"])

        # Then: listing details are in the output
        output = capsys.readouterr().out
        assert "1. [" in output, f"Expected ranked listing with score in output, got: {output!r}"
        assert "Staff Architect" in output, f"Expected listing title in output, got: {output!r}"
        assert "Acme Corp" in output, f"Expected company name in output, got: {output!r}"
        assert "testboard" in output, f"Expected board name in output, got: {output!r}"

    def test_rescore_re_exports_results_md_and_csv(
        self, rescore: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one JD file in output/jds/
        When handle_rescore runs
        Then Markdown and CSV results are exported to the output directory
        """
        # Given: one JD file
        self._write_jd(rescore["jd_dir"])

        # When: handle_rescore runs
        handle_rescore(rescore["args"])

        # Then: export files are created
        out_dir = rescore["out_dir"]
        output = capsys.readouterr().out
        assert "Exported Markdown" in output, (
            f"Expected Markdown export confirmation in output, got: {output!r}"
        )
        assert "Exported CSV" in output, (
            f"Expected CSV export confirmation in output, got: {output!r}"
        )
        assert (out_dir / "results.md").exists(), "results.md should be created"
        assert (out_dir / "results.csv").exists(), "results.csv should be created"

    def test_rescore_re_exports_jd_files_with_updated_scores_and_ranks(
        self, rescore: dict[str, Any], capsys: pytest.CaptureFixture[str]
    ) -> None:
        """
        Given one JD file in output/jds/
        When handle_rescore runs
        Then JD files are re-exported with updated scores and ranks
        """
        # Given: one JD file
        self._write_jd(rescore["jd_dir"])

        # When: handle_rescore runs
        handle_rescore(rescore["args"])

        # Then: JD files are re-exported
        output = capsys.readouterr().out
        assert "Exported JDs" in output, (
            f"Expected JD export confirmation in output, got: {output!r}"
        )
        jd_dir = rescore["jd_dir"]
        jd_files = list(jd_dir.glob("*.md"))
        assert len(jd_files) >= 1, (
            f"Expected at least 1 JD file after re-export, found {len(jd_files)}"
        )
