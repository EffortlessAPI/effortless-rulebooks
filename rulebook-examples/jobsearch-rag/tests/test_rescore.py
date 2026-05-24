"""
Rescore pipeline tests — JD file loading and re-scoring workflow.

Maps to BDD specs: TestJdFileLoading, TestRescoreWorkflow
"""

from __future__ import annotations

import textwrap
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from pathlib import Path

    from jobsearch_rag.rag.store import VectorStore

import pytest

from jobsearch_rag.pipeline.ranker import Ranker
from jobsearch_rag.pipeline.rescorer import (
    Rescorer,
    RescoreResult,
    load_jd_files,
)
from jobsearch_rag.rag.scorer import Scorer
from tests.constants import EMBED_FAKE

# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------

_SAMPLE_JD_CONTENT = textwrap.dedent("""\
    # Staff Platform Architect

    **Company:** Acme Corp
    **Location:** Remote
    **Board:** ziprecruiter
    **URL:** https://example.org/job/42

    ## Score

    - **Rank:** #1
    - **Final Score:** 0.85
    - **Fit Score:** 0.80
    - **Archetype Score:** 0.90
    - **History Score:** 0.70
    - **Comp Score:** 0.50
    - **Negative Score:** 0.10

    ## Job Description

    We are looking for a Staff Platform Architect to lead our cloud
    infrastructure team.  You will design distributed systems and
    mentor senior engineers.  Experience with Kubernetes and Terraform
    is required.
""")


@pytest.fixture()
def jd_dir(tmp_path: Path) -> Path:
    """Create a temp directory with a sample JD file."""
    d = tmp_path / "jds"
    d.mkdir()
    (d / "001_acme-corp_staff-platform-architect.md").write_text(_SAMPLE_JD_CONTENT)
    return d


# ---------------------------------------------------------------------------
# TestJdFileLoading
# ---------------------------------------------------------------------------


class TestJdFileLoading:
    """
    REQUIREMENT: JD files from output/jds/ are loaded back into JobListing objects.

    WHO: The rescore pipeline; the operator running ``rescore`` after tuning archetypes
    WHAT: (1) The system returns a JobListing with all metadata fields populated when it loads a well-formed JD file.
          (2) The system includes the Job Description body text in the listing's full_text when it parses a JD file.
          (3) The system derives the external_id from the trailing path segment of the JD file URL.
          (4) The system returns an empty list when asked to load JD files from a nonexistent directory.
          (5) The system skips a JD file that lacks a Job Description section.
          (6) The system uses default values for missing metadata fields when a JD file provides only a body.
          (7) The system returns listings in filename-sorted order when it loads multiple JD files from a directory.
    WHY: Without correct JD loading, rescoring would fail silently or produce
         garbled results — the operator would have to re-run full browser sessions

    MOCK BOUNDARY:
        Mock: (none — pure function tests on load_jd_files)
        Real: load_jd_files, filesystem via tmp_path
        Never: Patch file I/O or markdown parsing internals
    """

    def test_loads_listing_from_valid_jd_file(self, jd_dir: Path) -> None:
        """
        GIVEN a directory with a well-formed JD markdown file
        WHEN load_jd_files() reads the directory
        THEN a JobListing is returned with all metadata fields populated.
        """
        # When: load JD files
        listings = load_jd_files(jd_dir)

        # Then: one listing with all metadata
        assert len(listings) == 1, f"Expected 1 listing, got {len(listings)}"
        listing = listings[0]
        assert listing.title == "Staff Platform Architect", (
            f"Expected 'Staff Platform Architect', got {listing.title!r}"
        )
        assert listing.company == "Acme Corp", f"Expected 'Acme Corp', got {listing.company!r}"
        assert listing.board == "ziprecruiter", f"Expected 'ziprecruiter', got {listing.board!r}"
        assert listing.url == "https://example.org/job/42", (
            f"Expected URL to match, got {listing.url!r}"
        )
        assert listing.location == "Remote", f"Expected 'Remote', got {listing.location!r}"

    def test_listing_full_text_contains_jd_body(self, jd_dir: Path) -> None:
        """
        GIVEN a JD file with a Job Description section
        WHEN load_jd_files() parses it
        THEN the listing's full_text contains the JD body text.
        """
        # When: load the listing
        listings = load_jd_files(jd_dir)

        # Then: full_text includes JD content
        assert "Staff Platform Architect to lead" in listings[0].full_text, (
            f"Expected JD opening in full_text, got {listings[0].full_text[:100]!r}"
        )
        assert "Kubernetes and Terraform" in listings[0].full_text, (
            f"Expected tech keywords in full_text, got {listings[0].full_text[:100]!r}"
        )

    def test_external_id_derived_from_url(self, jd_dir: Path) -> None:
        """
        GIVEN a JD file with a URL ending in /42
        WHEN load_jd_files() parses it
        THEN the external_id is '42'.
        """
        # When: load the listing
        listings = load_jd_files(jd_dir)

        # Then: external_id is the last URL segment
        assert listings[0].external_id == "42", (
            f"Expected external_id '42', got {listings[0].external_id!r}"
        )

    def test_nonexistent_directory_returns_empty_list(self, tmp_path: Path) -> None:
        """
        GIVEN a path that does not exist
        WHEN load_jd_files() is called
        THEN an empty list is returned.
        """
        # When: load from non-existent path
        listings = load_jd_files(tmp_path / "does-not-exist")

        # Then: empty list, no error
        assert listings == [], f"Expected empty list, got {listings}"

    def test_file_without_jd_body_is_skipped(self, tmp_path: Path) -> None:
        """
        GIVEN a JD file missing the '## Job Description' section
        WHEN load_jd_files() reads it
        THEN the file is silently skipped.
        """
        # Given: a JD file with no body section
        d = tmp_path / "jds"
        d.mkdir()
        (d / "001_no-body.md").write_text("# Title\n\n**Company:** Co\n")

        # When: load JD files
        listings = load_jd_files(d)

        # Then: no listings (file skipped)
        assert listings == [], f"Expected empty list for body-less file, got {listings}"

    def test_missing_metadata_uses_defaults(self, tmp_path: Path) -> None:
        """
        GIVEN a JD file with only a body and no metadata headers
        WHEN load_jd_files() parses it
        THEN default values are used for missing fields.
        """
        # Given: minimal JD with no metadata
        d = tmp_path / "jds"
        d.mkdir()
        content = textwrap.dedent("""\
            ## Job Description

            Some job description text here.
        """)
        (d / "minimal.md").write_text(content)

        # When: load JD files
        listings = load_jd_files(d)

        # Then: listing loaded with defaults
        assert len(listings) == 1, f"Expected 1 listing, got {len(listings)}"
        assert listings[0].company == "Unknown", (
            f"Expected default company 'Unknown', got {listings[0].company!r}"
        )
        assert listings[0].board == "unknown", (
            f"Expected default board 'unknown', got {listings[0].board!r}"
        )

    def test_multiple_jd_files_loaded_in_sorted_order(self, tmp_path: Path) -> None:
        """
        GIVEN a directory with multiple JD files numbered 001, 002, 003
        WHEN load_jd_files() reads the directory
        THEN listings are returned in filename-sorted order.
        """
        # Given: three JD files with numeric prefixes
        d = tmp_path / "jds"
        d.mkdir()
        for i, (title, company) in enumerate(
            [("Role B", "Beta"), ("Role A", "Alpha"), ("Role C", "Gamma")], 1
        ):
            content = textwrap.dedent(f"""\
                # {title}

                **Company:** {company}
                **Board:** test

                ## Job Description

                Description for {title}.
            """)
            (d / f"{i:03d}_{company.lower()}_{title.lower().replace(' ', '-')}.md").write_text(
                content
            )

        # When: load all JD files
        listings = load_jd_files(d)

        # Then: sorted by filename (001, 002, 003)
        assert len(listings) == 3, f"Expected 3 listings, got {len(listings)}"
        assert listings[0].title == "Role B", (
            f"Expected 'Role B' first (001_), got {listings[0].title!r}"
        )
        assert listings[1].title == "Role A", (
            f"Expected 'Role A' second (002_), got {listings[1].title!r}"
        )
        assert listings[2].title == "Role C", (
            f"Expected 'Role C' third (003_), got {listings[2].title!r}"
        )


# ---------------------------------------------------------------------------
# TestRescoreWorkflow
# ---------------------------------------------------------------------------


class TestRescoreWorkflow:
    """
    REQUIREMENT: The rescore pipeline re-scores JDs through updated RAG collections.

    WHO: The operator iterating on archetype tuning or negative signal refinement
    WHAT: (1) The system returns ranked listings without failures when it rescoring a directory of valid JD files with a populated scoring stack.
          (2) The system returns an empty RescoreResult when it processes a non-existent JD directory.
          (3) The system counts failed listings and completes the run without crashing when required scoring data is missing.
          (4) The system initializes a fresh RescoreResult with zero and empty default field values.
    WHY: Without rescoring, the operator would need to re-run full browser
         sessions after every archetype or rubric tweak — a 20-minute wait
         instead of seconds

    MOCK BOUNDARY:
        Mock: Embedder I/O (embed, classify, health_check — Ollama HTTP)
        Real: Scorer, VectorStore (ChromaDB in tmp_path), Ranker, Rescorer,
              load_jd_files, RescoreResult
        Never: Patch Scorer, Ranker, or Rescorer internals
    """

    @staticmethod
    def _make_scorer(store: VectorStore, mock_embedder: object) -> Scorer:
        """Create a real Scorer wired to a populated store and I/O-stubbed embedder."""
        return Scorer(
            store=store,
            embedder=mock_embedder,  # type: ignore[arg-type]
            disqualify_on_llm_flag=False,
        )

    @staticmethod
    def _populate_store(store: VectorStore) -> None:
        """Seed a VectorStore with resume and archetype collections."""
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["Principal architect specializing in distributed systems."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-staff"],
            documents=["Staff Platform Architect: distributed systems."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Staff Platform Architect", "source": "role_archetypes"}],
        )

    @staticmethod
    def _make_ranker() -> Ranker:
        """Create a real Ranker with default weights."""
        return Ranker(
            archetype_weight=0.5,
            fit_weight=0.3,
            history_weight=0.2,
            comp_weight=0.0,
            negative_weight=0.4,
            min_score_threshold=0.0,
        )

    async def test_rescore_returns_ranked_listings(
        self, jd_dir: Path, vector_store: VectorStore, mock_embedder: object
    ) -> None:
        """
        GIVEN a directory with valid JD files and a populated scoring stack
        WHEN the Rescorer processes the directory
        THEN it returns ranked listings with no failures.
        """
        # Given: store populated with resume + archetypes
        self._populate_store(vector_store)
        scorer = self._make_scorer(vector_store, mock_embedder)
        ranker = self._make_ranker()
        rescorer = Rescorer(scorer=scorer, ranker=ranker)

        # When: rescore the JD directory
        result = await rescorer.rescore(str(jd_dir))

        # Then: one listing loaded, ranked, no failures
        assert result.total_loaded == 1, f"Expected 1 loaded, got {result.total_loaded}"
        assert len(result.ranked_listings) == 1, (
            f"Expected 1 ranked listing, got {len(result.ranked_listings)}"
        )
        assert result.failed_listings == 0, f"Expected 0 failures, got {result.failed_listings}"

    async def test_rescore_empty_directory_returns_empty_result(
        self, tmp_path: Path, vector_store: VectorStore, mock_embedder: object
    ) -> None:
        """
        GIVEN a non-existent JD directory
        WHEN the Rescorer processes it
        THEN an empty RescoreResult is returned.
        """
        # Given: scorer with populated store (irrelevant — no JDs to score)
        self._populate_store(vector_store)
        scorer = self._make_scorer(vector_store, mock_embedder)
        ranker = self._make_ranker()
        rescorer = Rescorer(scorer=scorer, ranker=ranker)

        # When: rescore a non-existent directory
        result = await rescorer.rescore(str(tmp_path / "empty"))

        # Then: empty result
        assert result.total_loaded == 0, f"Expected 0 loaded, got {result.total_loaded}"
        assert len(result.ranked_listings) == 0, (
            f"Expected 0 ranked listings, got {len(result.ranked_listings)}"
        )

    async def test_rescore_counts_failed_listings(
        self, jd_dir: Path, vector_store: VectorStore, mock_embedder: object
    ) -> None:
        """
        GIVEN a Scorer whose store lacks the required resume collection
        WHEN the Rescorer attempts to score JD files
        THEN failures are counted and the run completes without crashing.
        """
        # Given: empty store (no resume collection) → Scorer.score raises ActionableError
        scorer = self._make_scorer(vector_store, mock_embedder)
        ranker = self._make_ranker()
        rescorer = Rescorer(scorer=scorer, ranker=ranker)

        # When: rescore the JD directory
        result = await rescorer.rescore(str(jd_dir))

        # Then: listing was loaded but scoring failed
        assert result.total_loaded == 1, f"Expected 1 loaded, got {result.total_loaded}"
        assert result.failed_listings == 1, f"Expected 1 failure, got {result.failed_listings}"
        assert len(result.ranked_listings) == 0, (
            f"Expected 0 ranked listings, got {len(result.ranked_listings)}"
        )

    async def test_rescore_result_has_default_fields(self) -> None:
        """
        GIVEN no arguments
        WHEN a fresh RescoreResult is created
        THEN all fields have sensible zero/empty defaults.
        """
        # When: create a default RescoreResult
        result = RescoreResult()

        # Then: all fields are zeroed
        assert result.total_loaded == 0, f"Expected 0 loaded, got {result.total_loaded}"
        assert result.failed_listings == 0, f"Expected 0 failures, got {result.failed_listings}"
        assert result.ranked_listings == [], f"Expected empty list, got {result.ranked_listings}"
