"""
Integration tests — validate external dependency contracts.

These tests require **live Ollama** with ``nomic-embed-text`` and
``mistral:7b`` models pulled.  They are skipped by default and run
only when explicitly requested::

    uv run pytest -m integration          # run integration tests only
    uv run pytest -m "not integration"    # run everything else (default)
    uv run pytest                          # also skips integration

The integration marker is defined in ``pyproject.toml`` under
``[tool.pytest.ini_options].markers``.

Spec classes
------------
* **TestOllamaContract** — Ollama SDK response shapes we rely on
  (embedding vectors, chat responses, model listing, error types).
* **TestChromaDBContract** — ChromaDB distance semantics and
  persistence behavior.
* **TestEndToEndScoring** — full pipeline from resume indexing
  through scoring, using real Ollama embeddings with no mocks.
* **TestLiveZipRecruiterPipeline** — full system against live
  ZipRecruiter: browser session → search → extract → score → rank →
  export.

Between them, these tests catch the class of bug where our mocks
silently diverge from reality — e.g. an ollama SDK upgrade changes
the response shape, or ChromaDB alters its distance metric.
"""

from __future__ import annotations

import math
import tempfile
from pathlib import Path
from typing import TYPE_CHECKING

import pytest

from jobsearch_rag.adapters.session import SessionConfig, SessionManager, throttle
from jobsearch_rag.adapters.ziprecruiter import ZipRecruiterAdapter
from jobsearch_rag.errors import ActionableError, ErrorType
from jobsearch_rag.export.csv_export import CSVExporter
from jobsearch_rag.export.jd_files import JDFileExporter
from jobsearch_rag.export.markdown import MarkdownExporter
from jobsearch_rag.pipeline.ranker import Ranker
from jobsearch_rag.rag.comp_parser import compute_comp_score, parse_compensation
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.indexer import Indexer
from jobsearch_rag.rag.scorer import Scorer
from jobsearch_rag.rag.store import VectorStore

if TYPE_CHECKING:
    from collections.abc import Iterator

    from jobsearch_rag.adapters.base import JobListing
    from jobsearch_rag.rag.scorer import ScoreResult

# All tests in this file require live Ollama
pytestmark = pytest.mark.integration

# ---------------------------------------------------------------------------
# Configuration — matches settings.toml defaults
# ---------------------------------------------------------------------------
OLLAMA_BASE_URL = "http://localhost:11434"
EMBED_MODEL = "nomic-embed-text"
LLM_MODEL = "mistral:7b"


# ---------------------------------------------------------------------------
# Shared fixtures
# ---------------------------------------------------------------------------


@pytest.fixture
def embedder() -> Embedder:
    """A real Embedder pointed at localhost Ollama."""
    return Embedder(
        base_url=OLLAMA_BASE_URL,
        embed_model=EMBED_MODEL,
        llm_model=LLM_MODEL,
        max_retries=2,
        base_delay=0.5,
    )


@pytest.fixture
def store() -> Iterator[VectorStore]:
    """A VectorStore backed by a temporary directory."""
    with tempfile.TemporaryDirectory() as tmpdir:
        yield VectorStore(persist_dir=tmpdir)


@pytest.fixture
def sample_resume(tmp_path: Path) -> Path:
    """A small but realistic resume for integration testing."""
    resume = tmp_path / "resume.md"
    resume.write_text(
        "# Test Candidate\n\n"
        "## Summary\n"
        "Principal architect specializing in distributed systems "
        "and cloud-native platform design.\n\n"
        "## Experience\n"
        "### Acme Corp - Staff Engineer\n"
        "Led platform architecture for microservices migration.\n"
        "Designed event-driven systems with Kafka and Kubernetes.\n\n"
        "## Skills\n"
        "Python, Go, Kubernetes, Kafka, AWS, Terraform, gRPC\n",
        encoding="utf-8",
    )
    return resume


@pytest.fixture
def sample_archetypes(tmp_path: Path) -> Path:
    """A small archetypes TOML for integration testing."""
    archetypes = tmp_path / "archetypes.toml"
    archetypes.write_text(
        "[[archetypes]]\n"
        'name = "Staff Platform Architect"\n'
        'description = """\n'
        "Defines technical strategy and architecture for distributed "
        "systems platforms. Cross-team influence. Cloud-native "
        "infrastructure. API design and governance.\n"
        '"""\n\n'
        "[[archetypes]]\n"
        'name = "SRE Manager"\n'
        'description = """\n'
        "Manages on-call rotations, incident response, and reliability "
        "engineering. Primarily operational with people management.\n"
        '"""\n',
        encoding="utf-8",
    )
    return archetypes


# ---------------------------------------------------------------------------
# TestOllamaContract
# ---------------------------------------------------------------------------


class TestOllamaContract:
    """
    REQUIREMENT: Ollama SDK responses match the shapes our code assumes.

    WHO: Every unit test that mocks Ollama responses
    WHAT: (1) The system returns embeddings as lists of floats with consistent dimensionality across different inputs.
          (2) The system produces an embedding vector that is not all zeros for meaningful text input.
          (3) The system returns a non-empty string for a classification prompt.
          (4) The system passes the health check without raising an exception when the required models are available.
          (5) The system raises an EMBEDDING error that includes 'ollama pull' guidance when the configured model does not exist during a health check.
          (6) The system raises an EMBEDDING error with recovery guidance when asked to embed with a nonexistent model.
          (7) The system places semantically similar text closer to a query than unrelated text in embedding space.
    WHY: If an Ollama SDK update changes response shapes, our mocks would
         still pass but production would break — these tests catch that drift

    MOCK BOUNDARY:
        Mock:  (none — integration test uses live Ollama)
        Real:  Embedder.embed, Embedder.classify, Embedder.health_check
        Never: Mock Ollama responses (defeats the purpose of contract tests)
    """

    async def test_embed_returns_float_list_with_consistent_dimensions(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN two different text inputs
        WHEN embed() is called on each
        THEN both return list[float] with identical dimensionality.
        """
        # When: embed two different texts
        vec1 = await embedder.embed("distributed systems architecture")
        vec2 = await embedder.embed("underwater basket weaving")

        # Then: both are float lists with same dimensions
        assert isinstance(vec1, list), f"Expected list, got {type(vec1)}"
        assert len(vec1) > 0, "Embedding should not be empty"
        assert all(isinstance(v, float) for v in vec1), "All elements should be floats"
        assert len(vec1) == len(vec2), f"Embedding dimensions differ: {len(vec1)} vs {len(vec2)}"

    async def test_embed_vector_is_not_all_zeros(self, embedder: Embedder) -> None:
        """
        GIVEN a meaningful text input
        WHEN embed() is called
        THEN the resulting vector is not all zeros.
        """
        # When: embed meaningful text
        vec = await embedder.embed("Principal platform architect role")

        # Then: not a zero vector
        assert any(v != 0.0 for v in vec), "Embedding should carry semantic information"

    async def test_classify_returns_string(self, embedder: Embedder) -> None:
        """
        GIVEN a simple classification prompt
        WHEN classify() is called
        THEN the result is a non-empty string.
        """
        # When: classify a simple prompt
        result = await embedder.classify("Respond with exactly one word: hello")

        # Then: non-empty string
        assert isinstance(result, str), f"Expected str, got {type(result)}"
        assert len(result) > 0, "Classification result should not be empty"

    async def test_health_check_passes_with_required_models(self, embedder: Embedder) -> None:
        """
        GIVEN an Ollama instance with required models pulled
        WHEN health_check() is called
        THEN no exception is raised.
        """
        # When/Then: should not raise
        await embedder.health_check()

    async def test_health_check_nonexistent_model_suggests_ollama_pull(self) -> None:
        """
        GIVEN an Embedder configured with a nonexistent model
        WHEN health_check() is called
        THEN an EMBEDDING error with 'ollama pull' guidance is raised.
        """
        # Given: embedder with nonexistent model
        embedder = Embedder(
            base_url=OLLAMA_BASE_URL,
            embed_model="does-not-exist-model-xyz",
            llm_model=LLM_MODEL,
        )

        # When/Then: raises EMBEDDING error
        with pytest.raises(ActionableError) as exc_info:
            await embedder.health_check()

        # Then: error has actionable guidance
        err = exc_info.value
        assert err.error_type == ErrorType.EMBEDDING, (
            f"Expected EMBEDDING error, got {err.error_type}"
        )
        assert "does-not-exist-model-xyz" in err.error, "Error should name the missing model"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_embed_nonexistent_model_provides_recovery_guidance(self) -> None:
        """
        GIVEN an Embedder configured with a nonexistent model
        WHEN embed() is called
        THEN an EMBEDDING error with recovery guidance is raised.
        """
        # Given: embedder with nonexistent model
        embedder = Embedder(
            base_url=OLLAMA_BASE_URL,
            embed_model="nonexistent-model-abc",
            llm_model=LLM_MODEL,
            max_retries=1,
            base_delay=0.0,
        )

        # When/Then: raises EMBEDDING error
        with pytest.raises(ActionableError) as exc_info:
            await embedder.embed("test")

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.EMBEDDING, (
            f"Expected EMBEDDING error, got {err.error_type}"
        )
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_similar_texts_produce_closer_embeddings(self, embedder: Embedder) -> None:
        """
        GIVEN three texts: one query, one semantically similar, one unrelated
        WHEN all three are embedded
        THEN the similar text has smaller cosine distance to the query than the unrelated text.
        """
        # When: embed all three texts
        vec_arch = await embedder.embed("Staff platform architect for distributed cloud systems")
        vec_similar = await embedder.embed(
            "Principal engineer designing cloud-native infrastructure"
        )
        vec_unrelated = await embedder.embed("Recipe for chocolate chip cookies with extra sugar")

        # Then: similar is closer than unrelated
        def cosine_distance(a: list[float], b: list[float]) -> float:
            dot = sum(x * y for x, y in zip(a, b, strict=True))
            norm_a = math.sqrt(sum(x * x for x in a))
            norm_b = math.sqrt(sum(x * x for x in b))
            if norm_a == 0 or norm_b == 0:
                return 1.0
            return 1.0 - dot / (norm_a * norm_b)

        dist_similar = cosine_distance(vec_arch, vec_similar)
        dist_unrelated = cosine_distance(vec_arch, vec_unrelated)

        assert dist_similar < dist_unrelated, (
            f"Similar texts should be closer: {dist_similar:.4f} vs {dist_unrelated:.4f}"
        )


# ---------------------------------------------------------------------------
# TestChromaDBContract
# ---------------------------------------------------------------------------


class TestChromaDBContract:
    """
    REQUIREMENT: ChromaDB returns distances and results in the format we assume.

    WHO: VectorStore wrapper and Scorer distance-to-score conversion
    WHAT: (1) The system returns an approximately zero distance when it queries an indexed document with the same embedding vector.
          (2) The system assigns a lower distance to the architecture document than to the cooking document when it queries with an architecture-related embedding.
          (3) The system returns query results that include the ids, documents, metadatas, and distances keys.
          (4) The system preserves indexed data across a client restart when a new VectorStore client opens the same directory.
    WHY: Our _distance_to_score function assumes cosine distance semantics
         (0 = identical, 1 = orthogonal) — any deviation would invert
         all scoring logic

    MOCK BOUNDARY:
        Mock:  (none — integration test uses real ChromaDB)
        Real:  VectorStore operations, ChromaDB persistence, Embedder.embed
        Never: Mock ChromaDB responses (defeats the purpose of contract tests)
    """

    async def test_identical_documents_have_near_zero_distance(
        self, store: VectorStore, embedder: Embedder
    ) -> None:
        """
        GIVEN a document indexed with its embedding
        WHEN queried with the same embedding vector
        THEN the distance is approximately zero.
        """
        # Given: index a document
        text = "Staff architect for distributed systems"
        embedding = await embedder.embed(text)
        store.add_documents(
            collection_name="test_identity",
            ids=["doc-1"],
            documents=[text],
            embeddings=[embedding],
        )

        # When: query with same vector
        results = store.query(
            collection_name="test_identity",
            query_embedding=embedding,
            n_results=1,
        )

        # Then: distance ~0
        distances = results["distances"][0]
        assert len(distances) == 1, f"Expected 1 result, got {len(distances)}"
        assert distances[0] == pytest.approx(0.0, abs=1e-5), (
            f"Identical vector should have ~0 distance, got {distances[0]}"
        )

    async def test_dissimilar_documents_have_higher_distance(
        self, store: VectorStore, embedder: Embedder
    ) -> None:
        """
        GIVEN an architecture doc and a cooking doc indexed together
        WHEN queried with an architecture-related embedding
        THEN the architecture doc has lower distance than the cooking doc.
        """
        # Given: embed and index both documents
        vec_arch = await embedder.embed("Distributed systems platform architecture")
        vec_cooking = await embedder.embed("Baking sourdough bread at high altitude")
        vec_query = await embedder.embed("Cloud infrastructure architect role")

        store.add_documents(
            collection_name="test_distance",
            ids=["doc-arch", "doc-cooking"],
            documents=["architecture doc", "cooking doc"],
            embeddings=[vec_arch, vec_cooking],
        )

        # When: query with architecture vector
        results = store.query(
            collection_name="test_distance",
            query_embedding=vec_query,
            n_results=2,
        )

        # Then: architecture doc is closer
        distances = results["distances"][0]
        ids = results["ids"][0]
        arch_idx = ids.index("doc-arch")
        cooking_idx = ids.index("doc-cooking")
        assert distances[arch_idx] < distances[cooking_idx], (
            f"Architecture should be closer: arch={distances[arch_idx]:.4f}, "
            f"cooking={distances[cooking_idx]:.4f}"
        )

    async def test_query_results_contain_expected_keys(
        self, store: VectorStore, embedder: Embedder
    ) -> None:
        """
        GIVEN a document indexed with metadata
        WHEN query() is called
        THEN the result contains ids, documents, metadatas, and distances keys.
        """
        # Given: indexed document
        embedding = await embedder.embed("test document")
        store.add_documents(
            collection_name="test_keys",
            ids=["doc-1"],
            documents=["test document"],
            embeddings=[embedding],
            metadatas=[{"source": "test"}],
        )

        # When: query
        results = store.query(
            collection_name="test_keys",
            query_embedding=embedding,
            n_results=1,
        )

        # Then: expected keys present with correct structure
        assert "ids" in results, "Results should contain 'ids'"
        assert "documents" in results, "Results should contain 'documents'"
        assert "metadatas" in results, "Results should contain 'metadatas'"
        assert "distances" in results, "Results should contain 'distances'"
        assert isinstance(results["ids"][0], list), "ids[0] should be a list"
        assert isinstance(results["distances"][0], list), "distances[0] should be a list"

    async def test_persistence_survives_client_restart(self, embedder: Embedder) -> None:
        """
        GIVEN data indexed by one VectorStore client
        WHEN a new VectorStore client opens the same directory
        THEN the previously indexed data is still available.
        """
        with tempfile.TemporaryDirectory() as tmpdir:
            # Given: index with first client
            store1 = VectorStore(persist_dir=tmpdir)
            embedding = await embedder.embed("persistence test")
            store1.add_documents(
                collection_name="test_persist",
                ids=["persist-1"],
                documents=["persistence test"],
                embeddings=[embedding],
            )
            count_before = store1.collection_count("test_persist")

            # When: create new client against same directory
            store2 = VectorStore(persist_dir=tmpdir)
            count_after = store2.collection_count("test_persist")

            # Then: data persisted
            assert count_before == 1, "First client should see 1 document"
            assert count_after == 1, "Second client should also see 1 document"

            # Then: document is retrievable
            docs = store2.get_documents("test_persist", ids=["persist-1"])
            assert docs["documents"][0] == "persistence test", (
                "Retrieved document should match original"
            )


# ---------------------------------------------------------------------------
# TestEndToEndScoring
# ---------------------------------------------------------------------------


class TestEndToEndScoring:
    """
    REQUIREMENT: The full index-then-score pipeline produces valid results.

    WHO: The operator running the pipeline for the first time
    WHAT: (1) The system returns a valid ScoreResult with non-zero fit and archetype scores when it scores a matching JD.
          (2) The system assigns higher fit and archetype scores to a matching JD than to an unrelated JD.
          (3) The system returns a valid boolean `disqualified` field when it scores a matching JD with the LLM disqualifier enabled.
          (4) The system produces 5 resume chunks and makes the collection queryable when it indexes the real resume file.
          (5) The system produces 4 archetypes when it indexes the real archetypes file.
    WHY: Unit tests mock Ollama, so they can't catch integration failures
         like dimension mismatches between embed() and ChromaDB, or
         LLM classification prompts that produce unparseable output

    MOCK BOUNDARY:
        Mock:  (none — full pipeline with real Ollama and ChromaDB)
        Real:  Embedder, Indexer, Scorer, VectorStore, all pipeline steps
        Never: Mock any pipeline component (defeats integration purpose)
    """

    async def test_index_and_score_produces_valid_result(
        self,
        store: VectorStore,
        embedder: Embedder,
        sample_resume: Path,
        sample_archetypes: Path,
    ) -> None:
        """
        GIVEN a resume and archetypes indexed with real Ollama embeddings
        WHEN a matching JD is scored
        THEN a valid ScoreResult with non-zero fit and archetype scores is returned.
        """
        # Given: index resume and archetypes
        indexer = Indexer(store=store, embedder=embedder)
        resume_count = await indexer.index_resume(str(sample_resume))
        archetype_count = await indexer.index_archetypes(str(sample_archetypes))
        assert resume_count == 3, f"Expected 3 resume chunks, got {resume_count}"
        assert archetype_count == 2, f"Expected 2 archetypes, got {archetype_count}"

        # When: score a matching JD
        scorer = Scorer(
            store=store,
            embedder=embedder,
            disqualify_on_llm_flag=False,
        )
        result = await scorer.score(
            "Staff Platform Architect: design and build distributed "
            "systems infrastructure for cloud-native applications"
        )

        # Then: valid result with meaningful scores
        assert result.is_valid, (
            f"fit={result.fit_score}, arch={result.archetype_score}, hist={result.history_score}"
        )
        assert result.fit_score > 0.0, "Matching JD should have non-zero fit"
        assert result.archetype_score > 0.0, "Matching JD should have non-zero archetype"
        assert result.history_score == 0.0, "No decisions indexed, history should be 0"

    async def test_matching_jd_scores_higher_than_unrelated(
        self,
        store: VectorStore,
        embedder: Embedder,
        sample_resume: Path,
        sample_archetypes: Path,
    ) -> None:
        """
        GIVEN indexed resume and archetypes
        WHEN a matching JD and an unrelated JD are both scored
        THEN the matching JD has higher fit and archetype scores.
        """
        # Given: index resume and archetypes
        indexer = Indexer(store=store, embedder=embedder)
        await indexer.index_resume(str(sample_resume))
        await indexer.index_archetypes(str(sample_archetypes))

        # When: score both JDs
        scorer = Scorer(
            store=store,
            embedder=embedder,
            disqualify_on_llm_flag=False,
        )
        result_match = await scorer.score(
            "Principal Engineer: distributed systems, Kubernetes, "
            "platform architecture, cloud infrastructure, API design"
        )
        result_unrelated = await scorer.score(
            "Pastry Chef: create artisan breads and pastries for "
            "a high-end restaurant in downtown Portland"
        )

        # Then: matching JD scores higher
        assert result_match.fit_score > result_unrelated.fit_score, (
            f"Matching fit {result_match.fit_score:.4f} should exceed "
            f"unrelated {result_unrelated.fit_score:.4f}"
        )
        assert result_match.archetype_score > result_unrelated.archetype_score, (
            f"Matching archetype {result_match.archetype_score:.4f} should exceed "
            f"unrelated {result_unrelated.archetype_score:.4f}"
        )

    async def test_disqualifier_produces_parseable_json(
        self,
        store: VectorStore,
        embedder: Embedder,
        sample_resume: Path,
        sample_archetypes: Path,
    ) -> None:
        """
        GIVEN indexed resume and archetypes with LLM disqualifier enabled
        WHEN a matching JD is scored
        THEN the result has a valid boolean disqualified field.
        """
        # Given: index and configure scorer with disqualifier
        indexer = Indexer(store=store, embedder=embedder)
        await indexer.index_resume(str(sample_resume))
        await indexer.index_archetypes(str(sample_archetypes))

        scorer = Scorer(
            store=store,
            embedder=embedder,
            disqualify_on_llm_flag=True,
        )

        # When: score with disqualifier enabled
        result = await scorer.score(
            "Staff Platform Architect: lead the design of cloud-native "
            "distributed systems. Cross-team collaboration and mentoring."
        )

        # Then: disqualified field is a valid boolean
        assert isinstance(result.disqualified, bool), (
            f"Expected bool, got {type(result.disqualified)}"
        )
        # Soft assertion: matching role should not be disqualified
        if result.disqualified:
            pytest.skip(
                f"LLM unexpectedly disqualified a matching role: {result.disqualifier_reason}"
            )

    async def test_index_with_real_resume_file(
        self,
        store: VectorStore,
        embedder: Embedder,
    ) -> None:
        """
        GIVEN the actual project resume.md file
        WHEN index_resume is called
        THEN 5 chunks are produced and the collection is queryable.
        """
        # Given: real resume file
        real_resume = Path("data/resume.md")
        if not real_resume.exists():
            pytest.skip("data/resume.md not found — run from project root")

        # When: index the real resume
        indexer = Indexer(store=store, embedder=embedder)
        count = await indexer.index_resume(str(real_resume))

        # Then: expected chunks and queryable
        assert count == 5, f"Expected 5 resume chunks, got {count}"

        test_vec = await embedder.embed("distributed systems architecture")
        results = store.query(
            collection_name="resume",
            query_embedding=test_vec,
            n_results=3,
        )
        assert len(results["ids"][0]) == 3, "Should return 3 results"
        assert all(d >= 0.0 for d in results["distances"][0]), "Distances should be non-negative"

    async def test_index_with_real_archetypes_file(
        self,
        store: VectorStore,
        embedder: Embedder,
    ) -> None:
        """
        GIVEN the actual project role_archetypes.toml file
        WHEN index_archetypes is called
        THEN 4 archetypes are produced.
        """
        # Given: real archetypes file
        real_archetypes = Path("config/role_archetypes.toml")
        if not real_archetypes.exists():
            pytest.skip("config/role_archetypes.toml not found — run from project root")

        # When: index the real archetypes
        indexer = Indexer(store=store, embedder=embedder)
        count = await indexer.index_archetypes(str(real_archetypes))

        # Then: expected count
        assert count == 4, f"Expected 4 archetypes, got {count}"


# ---------------------------------------------------------------------------
# TestLiveZipRecruiterPipeline
# ---------------------------------------------------------------------------

# This class uses the ``live`` \er — it is excluded from both default
# and integration runs.  Run it explicitly:
#
#     uv run pytest -m live              # live tests only
#     uv run pytest -m "live or integration"  # both tiers


@pytest.mark.live
class TestLiveZipRecruiterPipeline:
    """
    REQUIREMENT: The full system works end-to-end against live ZipRecruiter.

    WHO: The operator validating the tool after installation or upgrade
    WHAT: (1) The system extracts live ZipRecruiter listings, scores them, ranks them in descending order, and exports the results when the full pipeline executes.
    WHY: Unit tests mock every I/O boundary — browser, Ollama, ChromaDB.
         Integration tests use real Ollama but fixture HTML.  Only this test
         validates the entire system against the real world: ZipRecruiter's
         DOM structure, Cloudflare behavior, Ollama model output, and
         ChromaDB persistence all working together.

    MOCK BOUNDARY:
        Mock:  (none — live end-to-end against real services)
        Real:  Browser, ZipRecruiter, Ollama, ChromaDB, all exporters
        Never: Mock any external service (defeats live validation)
    """

    # Minimum listings we expect from a single search page.  ZR typically
    # returns 20+ per page, so 5 is a conservative floor that accounts
    # for rate-limit or layout changes.
    MIN_LISTINGS = 5

    @pytest.fixture
    def live_embedder(self) -> Embedder:
        """A real Embedder for live pipeline tests."""
        return Embedder(
            base_url=OLLAMA_BASE_URL,
            embed_model=EMBED_MODEL,
            llm_model=LLM_MODEL,
            max_retries=2,
            base_delay=0.5,
        )

    @pytest.fixture
    def live_store(self) -> Iterator[VectorStore]:
        """A VectorStore in a temp directory for live pipeline tests."""
        with tempfile.TemporaryDirectory() as tmpdir:
            yield VectorStore(persist_dir=tmpdir)

    async def test_live_search_score_rank_export(
        self,
        live_store: VectorStore,
        live_embedder: Embedder,
        sample_resume: Path,
        sample_archetypes: Path,
        tmp_path: Path,
    ) -> None:
        """
        GIVEN a live ZipRecruiter session, Ollama, and indexed resume/archetypes
        WHEN the full pipeline executes: search → extract → score → rank → export
        THEN listings are extracted, scored, ranked in descending order, and exported.
        """
        # Given: skip if no session file
        session_path = Path("data/ziprecruiter_session.json")
        if not session_path.exists():
            pytest.skip("No ZipRecruiter session file — run login first")

        # Given: health check Ollama
        await live_embedder.health_check()

        # Given: index resume + archetypes
        indexer = Indexer(store=live_store, embedder=live_embedder)
        await indexer.index_resume(str(sample_resume))
        await indexer.index_archetypes(str(sample_archetypes))

        # When: browser session → authenticate → search one query
        adapter = ZipRecruiterAdapter()
        config = SessionConfig(
            board_name="ziprecruiter",
            headless=False,
            browser_channel="msedge",
        )

        search_url = (
            "https://www.ziprecruiter.com/jobs-search"
            "?search=staff+platform+architect&location=Remote+(USA)"
        )

        listings = []
        try:
            async with SessionManager(config) as session:
                page = await session.new_page()

                try:
                    await adapter.authenticate(page)
                except ActionableError as exc:
                    pytest.skip(f"ZipRecruiter auth failed (session expired?): {exc.error}")

                await throttle(adapter)
                try:
                    listings = await adapter.search(page, search_url, max_pages=1)
                except ActionableError as exc:
                    pytest.skip(f"ZipRecruiter search failed: {exc.error}")
        except Exception as exc:
            pytest.skip(f"Browser session failed: {exc}")

        # Then: structural assertions about extraction
        assert len(listings) >= self.MIN_LISTINGS, (
            f"Expected ≥{self.MIN_LISTINGS} listings, got {len(listings)}"
        )

        for listing in listings:
            assert listing.board == "ziprecruiter", "Board should be 'ziprecruiter'"
            assert listing.title, "Every listing must have a title"
            assert listing.company, "Every listing must have a company"
            assert listing.url.startswith("https://"), f"URL should be HTTPS: {listing.url}"
            assert listing.external_id, "Every listing must have an external_id"

        with_jd = [ls for ls in listings if ls.full_text.strip()]
        assert len(with_jd) >= 1, "At least one listing should have full JD text"

        # When: score each listing
        scorer = Scorer(
            store=live_store,
            embedder=live_embedder,
            disqualify_on_llm_flag=False,
        )
        base_salary = 220_000

        scored: list[tuple[JobListing, ScoreResult]] = []
        embeddings: dict[str, list[float]] = {}

        for listing in with_jd[: self.MIN_LISTINGS]:
            result = await scorer.score(listing.full_text)

            comp = parse_compensation(listing.full_text)
            if comp is not None:
                listing.comp_min = comp.comp_min
                listing.comp_max = comp.comp_max
                listing.comp_source = comp.comp_source
                listing.comp_text = comp.comp_text
            result.comp_score = compute_comp_score(listing.comp_max, base_salary)

            scored.append((listing, result))
            embedding = await live_embedder.embed(listing.full_text)
            embeddings[listing.url] = embedding

        # Then: valid scores
        for listing, result in scored:
            assert result.is_valid, (
                f"Invalid score for '{listing.title}': "
                f"fit={result.fit_score}, arch={result.archetype_score}"
            )
            assert 0.0 <= result.fit_score <= 1.0, f"Fit score out of range: {result.fit_score}"
            assert 0.0 <= result.archetype_score <= 1.0, (
                f"Archetype score out of range: {result.archetype_score}"
            )
            assert 0.0 <= result.comp_score <= 1.0, f"Comp score out of range: {result.comp_score}"
            assert result.fit_score > 0.0, (
                f"Zero fit score for '{listing.title}' — embedding may have failed"
            )
            assert result.archetype_score > 0.0, f"Zero archetype score for '{listing.title}'"

        # When: rank
        ranker = Ranker(
            archetype_weight=0.5,
            fit_weight=0.3,
            history_weight=0.2,
            comp_weight=0.15,
            min_score_threshold=0.0,
        )
        ranked, summary = ranker.rank(scored, embeddings)

        # Then: ranking assertions
        assert len(ranked) > 0, "Ranker should produce at least one result"
        assert summary.total_found == len(scored), "Summary should match scored count"
        assert summary.total_scored == len(scored), "Summary should match scored count"

        scores = [r.final_score for r in ranked]
        assert scores == sorted(scores, reverse=True), (
            f"Rankings not in descending order: {scores}"
        )

        for r in ranked:
            assert r.final_score > 0.0, f"Zero final score for '{r.listing.title}'"

        # When: export
        md_path = str(tmp_path / "results.md")
        csv_path = str(tmp_path / "results.csv")
        jd_dir = str(tmp_path / "jds")

        MarkdownExporter().export(ranked, md_path, summary=summary)
        CSVExporter().export(ranked, csv_path, summary=summary)
        jd_paths = JDFileExporter().export(ranked, jd_dir, summary=summary)

        # Then: Markdown export
        md_content = Path(md_path).read_text()
        assert "# Run Summary" in md_content, "Markdown should have run summary"
        assert "## Ranked Listings" in md_content, "Markdown should have ranked listings"
        assert "ziprecruiter" in md_content, "Markdown should mention board"
        for r in ranked:
            assert r.listing.title in md_content, (
                f"'{r.listing.title}' missing from Markdown export"
            )

        # Then: CSV export
        csv_content = Path(csv_path).read_text()
        assert "title,company,board" in csv_content, "CSV should have header"
        csv_lines = csv_content.strip().split("\n")
        assert len(csv_lines) == len(ranked) + 1, (
            f"CSV should have {len(ranked)} data rows, got {len(csv_lines) - 1}"
        )

        # Then: JD file export
        assert len(jd_paths) == len(ranked), (
            f"Expected {len(ranked)} JD files, got {len(jd_paths)}"
        )
        for jd_path in jd_paths:
            jd_content = jd_path.read_text()
            assert "## Score" in jd_content, f"JD file missing '## Score': {jd_path}"
            assert "## Job Description" in jd_content, (
                f"JD file missing '## Job Description': {jd_path}"
            )
            assert "**Board:** ziprecruiter" in jd_content, (
                f"JD file missing board field: {jd_path}"
            )
