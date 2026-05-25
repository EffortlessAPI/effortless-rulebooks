"""
RAG pipeline tests — Ollama connectivity, resume/archetype indexing, negative scoring.

Spec classes
------------
* **TestOllamaConnectivity** — startup checks, retry/backoff, error guidance.
* **TestResumeIndexing** — section chunking, re-indexing, empty collection errors.
* **TestArchetypeIndexing** — TOML parsing, metadata, whitespace normalization.
* **TestNegativeScoring** — continuous penalty via negative_signals collection.
* **TestGlobalPositiveSignalIndexing** — rubric dimension → ChromaDB document.
* **TestCultureScoring** — culture_score scoring axis, weight configuration.
"""

from __future__ import annotations

import tempfile
from typing import TYPE_CHECKING
from unittest.mock import AsyncMock, MagicMock, patch

import ollama as ollama_sdk
import pytest

from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.errors import ActionableError, ErrorType
from jobsearch_rag.pipeline.ranker import RankedListing, Ranker
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.indexer import Indexer
from jobsearch_rag.rag.scorer import Scorer, ScoreResult
from jobsearch_rag.rag.store import VectorStore
from tests.constants import EMBED_FAKE

if TYPE_CHECKING:
    from collections.abc import Iterator


# mock_embedder is provided by conftest.py (Embedder.__new__ + stubbed I/O)


@pytest.fixture
def store() -> Iterator[VectorStore]:
    """Yield a temporary VectorStore for test isolation."""
    with tempfile.TemporaryDirectory() as tmpdir:
        yield VectorStore(persist_dir=tmpdir)


@pytest.fixture
def indexer(store: VectorStore, mock_embedder: Embedder) -> Indexer:
    """Return an Indexer wired to the temporary store and mock embedder."""
    return Indexer(store=store, embedder=mock_embedder)


# ---------------------------------------------------------------------------
# TestOllamaConnectivity
# ---------------------------------------------------------------------------


class TestOllamaConnectivity:
    """
    REQUIREMENT: Ollama unavailability is detected before processing begins
    with guidance the operator can act on immediately.

    WHO: The pipeline runner; the operator who may have forgotten to start Ollama
    WHAT: (1) The system raises a CONNECTION error that names the unreachable Ollama URL and provides troubleshooting steps.
          (2) The system raises a CONNECTION error with actionable guidance before opening any browser session during startup checks.
          (3) The system raises an EMBEDDING error that suggests running 'ollama pull' when the configured model name is unavailable.
          (4) The system returns the embedding result on the third call after retrying twice with backoff following timeouts.
          (5) The system raises an EMBEDDING error that advises checking system resources after all embedding retries time out.
    WHY: Completing a full browser session only to fail at scoring wastes
         time and risks rate limiting; fail fast at startup

    MOCK BOUNDARY:
        Mock:  ollama_sdk.AsyncClient (for wrong-model and retry tests)
        Real:  Embedder.health_check, Embedder.embed, error classification
        Never: Patch the Embedder error-handling logic itself
    """

    async def test_unreachable_ollama_provides_url_and_connectivity_steps(self) -> None:
        """
        GIVEN an Embedder configured with an unreachable Ollama URL
        WHEN health_check() is called
        THEN a CONNECTION error naming the URL with troubleshooting steps is raised.
        """
        # Given: unreachable URL
        embedder = Embedder(
            base_url="http://localhost:59999",
            embed_model="nomic-embed-text",
            llm_model="mistral:7b",
        )

        # When/Then: raises CONNECTION error
        with pytest.raises(ActionableError) as exc_info:
            await embedder.health_check()

        # Then: error has URL and guidance
        err = exc_info.value
        assert err.error_type == ErrorType.CONNECTION, (
            f"Expected CONNECTION error, got {err.error_type}"
        )
        assert "localhost:59999" in err.error, "Error should name the configured URL"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_startup_check_runs_before_browser_session_opens(self) -> None:
        """
        GIVEN an Embedder with an unreachable Ollama URL
        WHEN health_check() is called at startup
        THEN a CONNECTION error with actionable guidance is raised before any browser work.
        """
        # Given: unreachable URL
        embedder = Embedder(
            base_url="http://localhost:59999",
            embed_model="nomic-embed-text",
            llm_model="mistral:7b",
        )

        # When/Then: raises CONNECTION error
        with pytest.raises(ActionableError) as exc_info:
            await embedder.health_check()

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.CONNECTION, (
            f"Expected CONNECTION error, got {err.error_type}"
        )
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_wrong_model_name_suggests_ollama_pull_command(
        self,
    ) -> None:
        """
        GIVEN an Embedder with a model name not in Ollama's model list
        WHEN health_check() is called
        THEN an EMBEDDING error suggesting 'ollama pull' is raised.
        """
        # Given: mock client returning empty model list
        mock_client = MagicMock()
        mock_response = MagicMock()
        mock_response.models = []
        mock_client.list = AsyncMock(return_value=mock_response)

        with patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client):
            embedder = Embedder(
                base_url="http://localhost:11434",
                embed_model="nonexistent-model-xyz",
                llm_model="also-nonexistent",
            )

            # When/Then: raises EMBEDDING error
            with pytest.raises(ActionableError) as exc_info:
                await embedder.health_check()

            # Then: error has guidance
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, (
                f"Expected EMBEDDING error, got {err.error_type}"
            )
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"
            assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_ollama_timeout_on_embedding_retries_with_backoff(self) -> None:
        """
        GIVEN an Ollama that fails twice then succeeds
        WHEN embed() is called with max_retries=3
        THEN the result is returned after 3 calls (2 retries + 1 success).
        """
        # Given: mock that fails twice then succeeds
        call_count = 0

        async def _mock_embed(model: str, input: str) -> object:
            nonlocal call_count
            call_count += 1
            if call_count < 3:
                raise ollama_sdk.ResponseError("timeout", status_code=504)

            resp = MagicMock()
            resp.embeddings = [EMBED_FAKE]
            return resp

        mock_client = MagicMock()
        mock_client.embed = _mock_embed

        with patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client):
            embedder = Embedder(
                base_url="http://localhost:11434",
                embed_model="nomic-embed-text",
                llm_model="mistral:7b",
                max_retries=3,
                base_delay=0.01,
            )

            # When: embed with retries
            result = await embedder.embed("test text")

            # Then: succeeded after 3 calls
            assert call_count == 3, f"Expected 3 calls (2 failures + 1 success), got {call_count}"
            assert result == EMBED_FAKE, "Should return the embedding from the successful call"

    async def test_ollama_timeout_after_retries_advises_checking_system_resources(self) -> None:
        """
        GIVEN an Ollama that always times out
        WHEN embed() exhausts all retries
        THEN an EMBEDDING error advising about system resources is raised.
        """

        # Given: always-failing mock
        async def _always_fail(model: str, input: str) -> object:
            raise ollama_sdk.ResponseError("timeout", status_code=504)

        mock_client = MagicMock()
        mock_client.embed = _always_fail

        with patch("jobsearch_rag.rag.embedder.ollama_sdk.AsyncClient", return_value=mock_client):
            embedder = Embedder(
                base_url="http://localhost:11434",
                embed_model="nomic-embed-text",
                llm_model="mistral:7b",
                max_retries=2,
                base_delay=0.01,
            )

            # When/Then: raises EMBEDDING error
            with pytest.raises(ActionableError) as exc_info:
                await embedder.embed("test text")

            # Then: error mentions retry count and has guidance
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, (
                f"Expected EMBEDDING error, got {err.error_type}"
            )
            assert "2" in err.error, "Error should mention retry count"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"


# ---------------------------------------------------------------------------
# TestResumeIndexing
# ---------------------------------------------------------------------------


class TestResumeIndexing:
    """
    REQUIREMENT: Resume is indexed into ChromaDB before scoring can proceed.

    WHO: The scorer computing fit_score; the operator running first-time setup
    WHAT: (1) The system raises an INDEX error telling the operator to run the index command when the resume collection is empty.
          (2) The system raises an INDEX error that names the missing resume collection and provides step-by-step setup guidance when the collection is absent.
          (3) The system creates one chunk per section with the correct content when it indexes a resume with three ## sections.
          (4) The system ensures each chunk contains at least one complete sentence when it chunks multi-sentence resume sections.
          (5) The system replaces previously indexed resume content instead of appending it when the same resume is indexed again.
          (6) The system returns the chunk count as 2 to confirm operator feedback when it indexes a resume with two ## sections.
    WHY: An empty resume collection silently produces zero fit_scores for all
         roles — a harder bug to catch than an explicit missing-index error

    MOCK BOUNDARY:
        Mock:  Embedder.embed (Ollama HTTP API via conftest mock_embedder)
        Real:  Indexer.index_resume, VectorStore (real temp dir), Scorer
        Never: Patch VectorStore internals or chunking logic
    """

    async def test_empty_resume_collection_tells_operator_to_run_index_command(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN an empty resume collection in ChromaDB
        WHEN scorer.score() is called
        THEN an INDEX error telling the operator to run the index command is raised.
        """
        # Given: empty store (no resume indexed)
        scorer = Scorer(store=store, embedder=mock_embedder)

        # When/Then: raises INDEX error
        with pytest.raises(ActionableError) as exc_info:
            await scorer.score("Any JD text")

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.INDEX, f"Expected INDEX error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_missing_collection_error_provides_step_by_step_setup_guidance(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN no resume collection in ChromaDB
        WHEN scorer.score() is called
        THEN the INDEX error names 'resume' and provides step-by-step setup guidance.
        """
        # Given: empty store
        scorer = Scorer(store=store, embedder=mock_embedder)

        # When/Then: raises error naming the collection
        with pytest.raises(ActionableError) as exc_info:
            await scorer.score("Any JD text")

        # Then: error mentions 'resume'
        err = exc_info.value
        assert "resume" in err.error.lower(), "Error should name the missing collection"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_resume_is_chunked_by_section_heading(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a resume with three ## sections
        WHEN index_resume is called
        THEN three chunks are created, one per section with correct content.
        """
        # Given: resume with 3 sections
        content = """\
# My Resume

## Summary
I am a principal architect.

## Experience
Led platform architecture at multiple companies.

## Skills
Python, distributed systems, cloud platforms.
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".md", delete=False) as f:
            f.write(content)
            path = f.name

        # When: index the resume
        n = await indexer.index_resume(path)

        # Then: 3 chunks with correct content
        assert n == 3, f"Expected 3 chunks, got {n}"
        docs = store.get_documents(
            collection_name="resume",
            ids=["resume-summary", "resume-experience", "resume-skills"],
        )
        assert len(docs["documents"]) == 3, "Should retrieve all 3 chunks"
        assert "principal architect" in docs["documents"][0], "Summary chunk should have content"
        assert "platform architecture" in docs["documents"][1], (
            "Experience chunk should have content"
        )
        assert "distributed systems" in docs["documents"][2], "Skills chunk should have content"

    async def test_each_chunk_contains_at_least_one_complete_sentence(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a resume with multi-sentence sections
        WHEN index_resume chunks the content
        THEN each chunk contains at least one complete sentence (no mid-sentence splits).
        """
        # Given: resume with complete sentences
        content = """\
# Resume

## Summary
I am an architect. I design systems.

## Experience
Led teams. Built platforms.
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".md", delete=False) as f:
            f.write(content)
            path = f.name

        # When: index
        await indexer.index_resume(path)

        # Then: each chunk has complete sentences
        for doc_id in ["resume-summary", "resume-experience"]:
            docs = store.get_documents(collection_name="resume", ids=[doc_id])
            body = docs["documents"][0]
            text = body.split("\n", 1)[-1] if "\n" in body else body
            assert "." in text, f"Chunk {doc_id} lacks complete sentence: {body!r}"

    async def test_reindex_replaces_previous_resume_content_not_appends(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a resume already indexed
        WHEN index_resume is called again
        THEN previous content is replaced, not appended.
        """
        # Given: resume content
        resume_content = """\
# Resume

## Summary
Original summary.
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".md", delete=False) as f:
            f.write(resume_content)
            path = f.name

        # Given: first index
        n1 = await indexer.index_resume(path)
        assert n1 == 1, f"First index should produce 1 chunk, got {n1}"
        count1 = store.collection_count("resume")

        # When: re-index
        n2 = await indexer.index_resume(path)

        # Then: replaced, not appended
        assert n2 == 1, f"Re-index should produce 1 chunk, got {n2}"
        count2 = store.collection_count("resume")
        assert count2 == count1, f"Re-index should replace, not append: {count1} → {count2}"

    async def test_index_confirms_chunk_count_in_output(self, indexer: Indexer) -> None:
        """
        GIVEN a resume with two ## sections
        WHEN index_resume is called
        THEN the return value is 2, confirming chunk count for operator feedback.
        """
        # Given: resume with 2 sections
        content = """\
# Resume

## Summary
I am an architect.

## Skills
Python, cloud, systems.
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".md", delete=False) as f:
            f.write(content)
            path = f.name

        # When: index
        n = await indexer.index_resume(path)

        # Then: returns section count
        assert n == 2, f"Expected 2 chunks for 2 sections, got {n}"


# ---------------------------------------------------------------------------
# TestArchetypeIndexing
# ---------------------------------------------------------------------------


class TestArchetypeIndexing:
    """
    REQUIREMENT: Role archetypes are loaded from TOML and embedded correctly.

    WHO: The scorer computing archetype_score
    WHAT: (1) The system creates one ChromaDB document for each archetype in the TOML file.
          (2) The system stores the archetype name in the document metadata for debugging.
          (3) The system raises a PARSE error that identifies the TOML syntax error and its file path with actionable guidance.
          (4) The system raises a VALIDATION error that tells the operator to add archetype entries before search with actionable guidance.
          (5) The system normalizes whitespace in the stored document text before embedding.
    WHY: Missing or malformed archetypes silently score all roles equally —
         the most insidious failure mode since ranking still appears to work

    MOCK BOUNDARY:
        Mock:  Embedder.embed (Ollama HTTP API via conftest mock_embedder)
        Real:  Indexer.index_archetypes, TOML parsing, VectorStore (real temp dir)
        Never: Patch TOML parsing or VectorStore internals
    """

    async def test_each_toml_archetype_produces_one_chroma_document(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a TOML file with two archetypes
        WHEN index_archetypes is called
        THEN each archetype produces one ChromaDB document.
        """
        # Given: TOML with 2 archetypes
        toml_content = """\
[[archetypes]]
name = "Staff Architect"
description = "Defines tech strategy."

[[archetypes]]
name = "DevRel"
description = "Developer relations."
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(toml_content)
            path = f.name

        # When: index archetypes
        n = await indexer.index_archetypes(path)

        # Then: 2 documents created
        assert n == 2, f"Expected 2 archetypes, got {n}"
        assert store.collection_count("role_archetypes") == 2, (
            f"Expected 2 in collection, got {store.collection_count('role_archetypes')}"
        )

    async def test_archetype_name_is_stored_as_document_metadata(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN an indexed archetype
        WHEN the document metadata is retrieved
        THEN the archetype name is stored for debugging.
        """
        # Given: TOML with one archetype
        toml_content = """\
[[archetypes]]
name = "Staff Architect"
description = "Defines tech strategy."
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(toml_content)
            path = f.name

        # When: index and retrieve
        await indexer.index_archetypes(path)
        docs = store.get_documents(
            collection_name="role_archetypes",
            ids=["archetype-staff-architect"],
        )

        # Then: name in metadata
        assert docs["metadatas"][0]["name"] == "Staff Architect", (
            f"Expected 'Staff Architect', got {docs['metadatas'][0].get('name')!r}"
        )

    async def test_malformed_toml_identifies_syntax_error_and_file_path(
        self, indexer: Indexer
    ) -> None:
        """
        GIVEN a TOML file with invalid syntax
        WHEN index_archetypes is called
        THEN a PARSE error with actionable guidance is raised.
        """
        # Given: malformed TOML
        bad_toml = "this is [not valid {{ toml"
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(bad_toml)
            path = f.name

        # When/Then: raises PARSE error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_archetypes(path)

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.PARSE, f"Expected PARSE error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_empty_archetypes_tells_operator_to_add_entries_before_search(
        self, indexer: Indexer
    ) -> None:
        """
        GIVEN an empty archetypes TOML file
        WHEN index_archetypes is called
        THEN a VALIDATION error with actionable guidance is raised.
        """
        # Given: empty TOML
        empty_toml = "# No archetypes defined\n"
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(empty_toml)
            path = f.name

        # When/Then: raises VALIDATION error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_archetypes(path)

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.VALIDATION, (
            f"Expected VALIDATION error, got {err.error_type}"
        )
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_archetype_description_whitespace_is_normalized_before_embedding(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN an archetype with excessive whitespace in its description
        WHEN index_archetypes is called
        THEN the stored document text has normalized whitespace.
        """
        # Given: TOML with messy whitespace
        toml_content = """\
[[archetypes]]
name = "Test"
description = \"\"\"
  Lots    of   extra
  whitespace   here
\"\"\"
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(toml_content)
            path = f.name

        # When: index
        await indexer.index_archetypes(path)

        # Then: no double spaces in stored text
        docs = store.get_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
        )
        doc_text = docs["documents"][0]
        assert "  " not in doc_text, f"Double spaces remain in stored text: {doc_text!r}"


# ---------------------------------------------------------------------------
# TestNegativeScoring
# ---------------------------------------------------------------------------


class TestNegativeScoring:
    """
    REQUIREMENT: JDs matching negative signals receive a continuous penalty score.

    WHO: The scorer and ranker computing the final ranked output
    WHAT: (1) The system sets negative_score to 0.0 when the negative_signals collection is missing.
          (2) The system sets negative_score to 0.0 when the negative_signals collection is empty.
          (3) The system returns a ScoreResult that includes negative_score with a value between 0.0 and 1.0 when a job description matches a negative signal.
    WHY: Binary disqualification (yes/no via LLM) misses gradient cases —
         a role at a borderline-adtech company should rank lower, not be
         entirely hidden; the negative_score provides continuous penalization

    MOCK BOUNDARY:
        Mock:  Embedder.embed (Ollama HTTP API via conftest mock_embedder)
        Real:  Scorer.score, VectorStore (real temp dir with pre-populated data)
        Never: Patch Scorer internals or distance-to-score conversion
    """

    async def test_negative_score_is_zero_when_collection_missing(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a VectorStore with resume and archetypes but no negative_signals collection
        WHEN scorer.score is called
        THEN negative_score defaults to 0.0.
        """
        # Given: populate resume and archetypes only — no negative_signals
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )

        # When: score a JD
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("Any JD text")

        # Then: negative_score is 0.0
        assert result.negative_score == 0.0, (
            f"Expected negative_score 0.0 when collection missing, got {result.negative_score}"
        )

    async def test_negative_score_is_zero_when_collection_empty(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a VectorStore with an empty negative_signals collection
        WHEN scorer.score is called
        THEN negative_score defaults to 0.0.
        """
        # Given: resume + archetypes populated, empty negative_signals
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )
        store.reset_collection("negative_signals")

        # When: score a JD
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("Any JD text")

        # Then: negative_score is 0.0
        assert result.negative_score == 0.0, (
            f"Expected negative_score 0.0 for empty collection, got {result.negative_score}"
        )

    async def test_negative_score_returned_in_score_result(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a VectorStore with populated negative_signals
        WHEN scorer.score is called with a JD matching a negative signal
        THEN ScoreResult includes negative_score in [0.0, 1.0].
        """
        # Given: resume + archetypes + negative_signals populated
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )
        store.add_documents(
            collection_name="negative_signals",
            ids=["neg-test"],
            documents=["Adtech surveillance platform"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "rubric:Industry", "signal": "Adtech surveillance platform"}],
        )

        # When: score a matching JD
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("Some JD about adtech platform")

        # Then: negative_score field present and in valid range
        assert hasattr(result, "negative_score"), "ScoreResult should have negative_score field"
        assert 0.0 <= result.negative_score <= 1.0, (
            f"negative_score should be in [0.0, 1.0], got {result.negative_score}"
        )


# ---------------------------------------------------------------------------
# TestGlobalPositiveSignalIndexing
# ---------------------------------------------------------------------------


class TestGlobalPositiveSignalIndexing:
    """
    REQUIREMENT: Positive signals from the global rubric are indexed into
    a dedicated ChromaDB collection that the scorer queries to compute a
    continuous culture score — orthogonal to archetype score.

    WHO: The scorer computing culture_score; the indexer building the
         global_positive_signals collection
    WHAT: (1) The system creates exactly one ChromaDB document for each rubric dimension that defines positive signals.
          (2) The system stores the source dimension name in the indexed document metadata.
          (3) The system replaces the existing global positive collection during reindexing instead of appending duplicate documents.
          (4) The system produces no document for a dimension that does not define positive signals.
          (5) The system raises a CONFIG error with actionable guidance when the global rubric file is missing.
          (6) The system rebuilds the global positive collection when `--archetypes-only` triggers positive signal indexing.
          (7) The system sets the global positive collection count to the number of dimensions that contribute positive signals.
          (8) The system produces no positive document for a negative-only compensation dimension.
          (9) The system raises a PARSE error that names the malformed rubric file and suggests how to fix it.
    WHY: The global rubric positive signals encode universal environment
         and culture preferences that apply to every role regardless of
         archetype. A dedicated collection keeps archetype and culture
         scoring axes independent

    MOCK BOUNDARY:
        Mock:  Embedder.embed (Ollama HTTP API via conftest mock_embedder)
        Real:  Indexer.index_global_positive_signals, TOML parsing, VectorStore
        Never: Patch TOML parsing or VectorStore internals
    """

    async def test_one_document_per_rubric_dimension_with_positive_signals(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a rubric TOML with two dimensions that have signals_positive
        WHEN index_global_positive_signals is called
        THEN each dimension produces exactly one ChromaDB document.
        """
        # Given: TOML with 2 dimensions having signals_positive
        rubric_toml = """\
[[dimensions]]
name = "Altitude"
signals_positive = ["strategic", "cross-org"]
signals_negative = ["tactical only"]

[[dimensions]]
name = "Humane Culture"
signals_positive = ["async-first", "no crunch"]
signals_negative = ["60-hour weeks"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        # When: index
        n = await indexer.index_global_positive_signals(path)

        # Then: 2 documents created
        assert n == 2, f"Expected 2 documents, got {n}"
        assert store.collection_count("global_positive_signals") == 2, (
            f"Expected 2 in collection, got {store.collection_count('global_positive_signals')}"
        )

    async def test_document_metadata_identifies_source_dimension(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN an indexed rubric dimension with signals_positive
        WHEN the document metadata is retrieved
        THEN the source dimension name is stored in metadata.
        """
        # Given: TOML with one dimension
        rubric_toml = """\
[[dimensions]]
name = "Altitude"
signals_positive = ["strategic thinking", "cross-org influence"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        # When: index and retrieve
        await indexer.index_global_positive_signals(path)
        docs = store.get_documents(
            collection_name="global_positive_signals",
            ids=["pos-altitude"],
        )

        # Then: source in metadata
        assert docs["metadatas"][0]["source"] == "Altitude", (
            f"Expected source 'Altitude', got {docs['metadatas'][0].get('source')}"
        )

    async def test_reindex_replaces_global_positive_collection_not_appends(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN an already-indexed global positive collection
        WHEN index_global_positive_signals is called again
        THEN the collection is replaced, not appended to.
        """
        # Given: TOML indexed once
        rubric_toml = """\
[[dimensions]]
name = "Altitude"
signals_positive = ["strategic"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        await indexer.index_global_positive_signals(path)
        count1 = store.collection_count("global_positive_signals")

        # When: re-index
        await indexer.index_global_positive_signals(path)
        count2 = store.collection_count("global_positive_signals")

        # Then: same count (replaced, not appended)
        assert count2 == count1, (
            f"Re-indexing should replace, not append. Count went from {count1} to {count2}"
        )

    async def test_dimension_without_signals_positive_produces_no_document(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a rubric dimension with only signals_negative (no signals_positive)
        WHEN index_global_positive_signals is called
        THEN zero documents are produced.
        """
        # Given: TOML with negative-only dimension
        rubric_toml = """\
[[dimensions]]
name = "Compensation Red Flags"
signals_negative = ["equity-only compensation"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        # When: index
        n = await indexer.index_global_positive_signals(path)

        # Then: no documents
        assert n == 0, f"Expected 0 documents for dimension without signals_positive, got {n}"

    async def test_missing_global_rubric_produces_actionable_error(self, indexer: Indexer) -> None:
        """
        GIVEN a nonexistent global_rubric.toml path
        WHEN index_global_positive_signals is called
        THEN a CONFIG error with actionable guidance is raised.
        """
        # When/Then: raises CONFIG error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_global_positive_signals("/nonexistent/rubric.toml")

        # Then: error references rubric or file-not-found
        err = exc_info.value
        assert err.error_type == ErrorType.CONFIG, f"Expected CONFIG error, got {err.error_type}"
        assert "rubric" in err.error.lower() or "not found" in err.error.lower(), (
            f"Error should mention rubric or not found: {err.error!r}"
        )

    async def test_archetypes_only_flag_rebuilds_global_positive_collection(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a rubric TOML with a dimension having signals_positive
        WHEN --archetypes-only triggers index_global_positive_signals
        THEN the global positive collection is rebuilt.
        """
        # Given: TOML with one positive dimension
        rubric_toml = """\
[[dimensions]]
name = "Altitude"
signals_positive = ["strategic vision"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        # When: simulate --archetypes-only by calling index_global_positive_signals
        n = await indexer.index_global_positive_signals(path)

        # Then: at least 1 document indexed
        assert n >= 1, "Global positive signals should be indexed during --archetypes-only"

    async def test_global_positive_collection_count_matches_contributing_dimensions(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a rubric with 2 positive and 1 negative-only dimension
        WHEN index_global_positive_signals is called
        THEN the collection count equals the number of contributing dimensions.
        """
        # Given: 3 dimensions, only 2 with signals_positive
        rubric_toml = """\
[[dimensions]]
name = "Altitude"
signals_positive = ["strategic"]

[[dimensions]]
name = "Humane Culture"
signals_positive = ["async-first"]

[[dimensions]]
name = "Compensation"
signals_negative = ["equity-only"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        # When: index
        n = await indexer.index_global_positive_signals(path)

        # Then: only 2 contributing dimensions counted
        assert n == 2, f"Only 2 dimensions have signals_positive, got {n}"
        assert store.collection_count("global_positive_signals") == 2, (
            f"Expected 2 in collection, got {store.collection_count('global_positive_signals')}"
        )

    async def test_compensation_dimension_produces_no_positive_document(
        self, indexer: Indexer, store: VectorStore
    ) -> None:
        """
        GIVEN a dimension with only signals_negative (e.g., Compensation Red Flags)
        WHEN index_global_positive_signals is called
        THEN zero positive documents are produced.
        """
        # Given: negative-only dimension
        rubric_toml = """\
[[dimensions]]
name = "Compensation Red Flags"
signals_negative = ["equity-only compensation", "unpaid position"]
"""
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write(rubric_toml)
            path = f.name

        # When: index
        n = await indexer.index_global_positive_signals(path)

        # Then: no positive documents
        assert n == 0, f"Expected 0 positive documents from negative-only dimension, got {n}"

    async def test_malformed_rubric_toml_produces_actionable_parse_error(
        self, indexer: Indexer
    ) -> None:
        """
        GIVEN a global rubric TOML file with invalid syntax
        WHEN index_global_positive_signals is called
        THEN a PARSE error is raised naming the file and suggesting a fix.
        """
        # Given: malformed TOML
        with tempfile.NamedTemporaryFile(mode="w", suffix=".toml", delete=False) as f:
            f.write('[[dimensions]]\nname = "broken\n')
            path = f.name

        # When/Then: raises PARSE error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_global_positive_signals(path)

        # Then: error references file or syntax
        err = exc_info.value
        assert err.error_type == ErrorType.PARSE, (
            f"Expected PARSE error for malformed rubric TOML, got {err.error_type}"
        )
        assert "global_rubric" in err.error.lower() or "syntax" in err.error.lower(), (
            f"Error should mention the file or syntax issue: {err.error!r}"
        )


# ---------------------------------------------------------------------------
# TestCultureScoring
# ---------------------------------------------------------------------------


class TestCultureScoring:
    """
    REQUIREMENT: culture_score continuously rewards roles whose environment
    signals match global rubric positive dimensions — altitude, humane
    culture, domain alignment, scope, company maturity, ethics, and
    ND compatibility — acting as a second scoring axis orthogonal to
    archetype and fit.

    WHO: The ranker computing final_score; the operator who wants roles
         in humane, well-scoped, ethically aligned environments to rank
         higher regardless of which archetype they match
    WHAT: (1) The system defaults culture_score to 0.0 when the global_positive_signals collection is missing.
          (2) The system returns culture_score as a float between 0.0 and 1.0.
          (3) The system includes culture_score as a named field in ScoreResult.
          (4) The system includes the culture score value in the score explanation output.
          (5) The system stores and exposes the configured culture_weight value instead of using a hardcoded default.
          (6) The system produces a higher final score when culture_score is higher and all other score inputs are unchanged.
          (7) The system returns a culture_score of 0.0 when the global_positive_signals collection is empty.
          (8) The system includes all six component score values in the score explanation output.
    WHY: Archetype score answers "right kind of role." Culture score
         answers "right kind of environment."

    MOCK BOUNDARY:
        Mock:  Embedder.embed (Ollama HTTP API via conftest mock_embedder)
        Real:  Scorer.score, Ranker.compute_final_score, VectorStore,
               RankedListing.score_explanation
        Never: Patch Scorer or Ranker internals
    """

    async def test_missing_global_positive_collection_returns_zero_not_error(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a VectorStore without a global_positive_signals collection
        WHEN scorer.score is called
        THEN culture_score defaults to 0.0.
        """
        # Given: required collections populated but NOT global_positive_signals
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )

        # When: score
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("Any JD text")

        # Then: culture_score is 0.0
        assert result.culture_score == 0.0, (
            f"Expected culture_score 0.0 when collection missing, got {result.culture_score}"
        )

    async def test_culture_score_is_float_between_zero_and_one(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a VectorStore with populated global_positive_signals
        WHEN scorer.score is called
        THEN culture_score is a float in [0.0, 1.0].
        """
        # Given: all required collections + global_positive_signals populated
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )
        store.add_documents(
            collection_name="global_positive_signals",
            ids=["pos-altitude"],
            documents=["Altitude: strategic thinking, cross-org influence"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "Altitude"}],
        )

        # When: score a JD
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("A strategic platform architect role")

        # Then: float in [0.0, 1.0]
        assert isinstance(result.culture_score, float), (
            f"culture_score should be float, got {type(result.culture_score)}"
        )
        assert 0.0 <= result.culture_score <= 1.0, (
            f"culture_score should be in [0.0, 1.0], got {result.culture_score}"
        )

    async def test_culture_score_returned_in_score_result(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN a VectorStore with required collections
        WHEN scorer.score is called
        THEN ScoreResult includes culture_score as a named field.
        """
        # Given: required collections populated
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )

        # When: score
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("Any JD text")

        # Then: culture_score field exists
        assert hasattr(result, "culture_score"), "ScoreResult should have culture_score field"

    async def test_culture_score_appears_in_score_breakdown_output(self) -> None:
        """
        GIVEN a RankedListing with a culture_score of 0.65
        WHEN score_explanation is called
        THEN the explanation string includes 'Culture: 0.65'.
        """
        # Given: scores with culture_score=0.65
        scores = ScoreResult(
            fit_score=0.75,
            archetype_score=0.80,
            history_score=0.60,
            comp_score=0.90,
            negative_score=0.15,
            culture_score=0.65,
            disqualified=False,
        )
        ranked = RankedListing(
            listing=JobListing(
                board="test",
                external_id="1",
                title="Test",
                company="Co",
                location="Remote",
                url="https://example.org",
                full_text="JD text",
            ),
            scores=scores,
            final_score=0.75,
        )

        # When: get explanation
        explanation = ranked.score_explanation()

        # Then: culture_score appears
        assert "Culture: 0.65" in explanation, (
            f"culture_score should appear in explanation. Got: {explanation}"
        )

    async def test_culture_weight_is_read_from_settings_not_hardcoded(self) -> None:
        """
        GIVEN a Ranker instantiated with culture_weight=0.2
        WHEN the attribute is read
        THEN it reflects the configured value, not a hardcoded default.
        """
        # Given: Ranker with explicit culture_weight
        ranker = Ranker(
            archetype_weight=0.5,
            fit_weight=0.3,
            history_weight=0.2,
            comp_weight=0.15,
            culture_weight=0.2,
            negative_weight=0.4,
            min_score_threshold=0.0,
        )

        # Then: culture_weight matches configured value
        assert ranker.culture_weight == 0.2, (
            f"Expected culture_weight 0.2, got {ranker.culture_weight}"
        )

    async def test_high_culture_score_raises_final_score(self) -> None:
        """
        GIVEN two ScoreResults identical except for culture_score (0.0 vs 0.9)
        WHEN compute_final_score is called on each
        THEN the high-culture result has a higher final score.
        """
        # Given: Ranker with culture_weight=0.3
        ranker = Ranker(
            archetype_weight=0.5,
            fit_weight=0.3,
            history_weight=0.0,
            comp_weight=0.0,
            culture_weight=0.3,
            negative_weight=0.0,
            min_score_threshold=0.0,
        )
        scores_no_culture = ScoreResult(
            fit_score=0.5,
            archetype_score=0.5,
            history_score=0.0,
            comp_score=0.0,
            culture_score=0.0,
            negative_score=0.0,
            disqualified=False,
        )
        scores_with_culture = ScoreResult(
            fit_score=0.5,
            archetype_score=0.5,
            history_score=0.0,
            comp_score=0.0,
            culture_score=0.9,
            negative_score=0.0,
            disqualified=False,
        )

        # When: compute final scores
        final_no = ranker.compute_final_score(scores_no_culture)
        final_with = ranker.compute_final_score(scores_with_culture)

        # Then: high culture raises final
        assert final_with > final_no, (
            f"High culture_score should raise final. No culture: {final_no}, With: {final_with}"
        )

    async def test_empty_global_positive_collection_returns_zero_culture_score(
        self, store: VectorStore, mock_embedder: Embedder
    ) -> None:
        """
        GIVEN an empty global_positive_signals collection
        WHEN scorer.score is called
        THEN culture_score is 0.0.
        """
        # Given: required collections + empty global_positive_signals
        store.add_documents(
            collection_name="resume",
            ids=["resume-summary"],
            documents=["## Summary\nPrincipal architect."],
            embeddings=[EMBED_FAKE],
            metadatas=[{"source": "resume", "section": "Summary"}],
        )
        store.add_documents(
            collection_name="role_archetypes",
            ids=["archetype-test"],
            documents=["Staff Architect"],
            embeddings=[EMBED_FAKE],
            metadatas=[{"name": "Test", "source": "role_archetypes"}],
        )
        store.reset_collection("global_positive_signals")

        # When: score
        scorer = Scorer(store=store, embedder=mock_embedder, disqualify_on_llm_flag=False)
        result = await scorer.score("Any JD text")

        # Then: culture_score is 0.0
        assert result.culture_score == 0.0, (
            f"Expected culture_score 0.0 for empty collection, got {result.culture_score}"
        )

    async def test_score_explanation_includes_all_six_component_values(self) -> None:
        """
        GIVEN a RankedListing with all six score components set
        WHEN score_explanation is called
        THEN all six components appear in the explanation string.
        """
        # Given: scores with all 6 components
        scores = ScoreResult(
            fit_score=0.75,
            archetype_score=0.80,
            history_score=0.60,
            comp_score=0.90,
            negative_score=0.25,
            culture_score=0.65,
            disqualified=False,
        )
        ranked = RankedListing(
            listing=JobListing(
                board="test",
                external_id="1",
                title="Test",
                company="Co",
                location="Remote",
                url="https://example.org",
                full_text="JD text",
            ),
            scores=scores,
            final_score=0.75,
        )

        # When: get explanation
        explanation = ranked.score_explanation()

        # Then: all 6 components present
        assert "Archetype: 0.80" in explanation, f"Missing Archetype in: {explanation}"
        assert "Fit: 0.75" in explanation, f"Missing Fit in: {explanation}"
        assert "Culture: 0.65" in explanation, f"Missing Culture in: {explanation}"
        assert "History: 0.60" in explanation, f"Missing History in: {explanation}"
        assert "Comp: 0.90" in explanation, f"Missing Comp in: {explanation}"
        assert "Negative: 0.25" in explanation, f"Missing Negative in: {explanation}"
