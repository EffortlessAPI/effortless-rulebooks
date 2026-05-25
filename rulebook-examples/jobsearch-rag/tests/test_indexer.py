"""
Indexer tests — resume chunking, archetype ingestion, and negative signal indexing.

Spec classes:
    TestResumeChunking — resume split on ## headings for semantic coherence
    TestResumeIndexing — resume embedded and stored in ChromaDB
    TestArchetypeIndexing — TOML archetypes loaded and embedded
    TestArchetypeEmbeddingSynthesis — description + signals combined
    TestGlobalRubricLoading — rubric TOML parsed for negative signals
    TestNegativeSignalIndexing — rubric + archetype negatives combined
"""

from __future__ import annotations

import tempfile
import textwrap
from typing import TYPE_CHECKING

import pytest

from jobsearch_rag.errors import ActionableError, ErrorType
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.indexer import Indexer, build_archetype_embedding_text
from jobsearch_rag.rag.store import VectorStore
from tests.conftest import make_mock_ollama_client

if TYPE_CHECKING:
    from collections.abc import Iterator
    from pathlib import Path

# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------

FAKE_EMBEDDING = [0.1, 0.2, 0.3, 0.4, 0.5]

SAMPLE_RESUME = textwrap.dedent("""\
    # Jack Pines

    Contact info line

    ## Summary

    Principal-level Systems Architect specializing in distributed systems.
    Fifteen years of experience designing cloud architectures.

    ## Core Strengths

    - Distributed Systems & Data Platforms
    - Cloud & Infrastructure
    - AI-Integrated Tooling

    ## Experience

    ### Acme Corp — Staff Engineer

    _Remote | Jan 2023 - Present_

    - Led platform architecture for streaming data systems.
    - Mentored junior engineers on system design.

    ### Beta Inc — Senior Engineer

    _Remote | Jun 2020 - Dec 2022_

    - Built ingestion pipelines for real-time analytics.

    ## Earlier Roles

    - Previous Company — Engineer (2018-2020)
""")

SAMPLE_ARCHETYPES_TOML = textwrap.dedent("""\
    [[archetypes]]
    name = "Staff Platform Architect"
    description = \"\"\"
    Defines technical strategy for distributed systems.
    Cross-team influence. Cloud-native infrastructure.
    \"\"\"

    [[archetypes]]
    name = "Principal Data Engineer"
    description = \"\"\"
    Leads data pipeline architecture and platform governance.
    Deep experience with cloud data stacks.
    \"\"\"
""")

SAMPLE_ARCHETYPES_WITH_SIGNALS_TOML = textwrap.dedent("""\
    [[archetypes]]
    name = "Staff Platform Architect"
    description = \"\"\"
    Defines technical strategy for distributed systems.
    Cross-team influence. Cloud-native infrastructure.
    \"\"\"
    signals_positive = [
        "Cross-team architecture ownership",
        "Distributed systems design",
    ]
    signals_negative = [
        "Primarily hands-on feature development",
        "No cross-team scope mentioned",
    ]

    [[archetypes]]
    name = "Principal Data Engineer"
    description = \"\"\"
    Leads data pipeline architecture and platform governance.
    Deep experience with cloud data stacks.
    \"\"\"
    signals_positive = [
        "Data pipeline architecture ownership",
        "Cloud data stack expertise",
    ]
    signals_negative = [
        "Data analyst or BI developer role",
        "Junior data engineering with no architecture",
    ]
""")

SAMPLE_RUBRIC_TOML = textwrap.dedent("""\
    [[dimensions]]
    name = "Role Scope"
    signals_negative = [
        "IC coding role disguised with architect title",
        "No cross-team influence mentioned",
    ]

    [[dimensions]]
    name = "Industry Alignment"
    signals_negative = [
        "Adtech or surveillance platform",
        "Gambling technology",
    ]
""")


@pytest.fixture
def store() -> Iterator[VectorStore]:
    """A VectorStore backed by a temporary directory."""
    with tempfile.TemporaryDirectory() as tmpdir:
        yield VectorStore(persist_dir=tmpdir)


@pytest.fixture
def mock_embedder() -> Embedder:
    """Real Embedder with ollama client stubbed at the I/O boundary."""
    mock_client = make_mock_ollama_client(embed_vector=FAKE_EMBEDDING)
    embedder = Embedder(
        base_url="http://localhost:11434",
        embed_model="nomic-embed-text",
        llm_model="mistral:7b",
        max_retries=1,
        base_delay=0.0,
    )
    embedder._client = mock_client  # type: ignore[attr-defined]
    return embedder


@pytest.fixture
def indexer(store: VectorStore, mock_embedder: Embedder) -> Indexer:
    """An Indexer wired to a real VectorStore and a mocked Embedder."""
    return Indexer(store=store, embedder=mock_embedder)


@pytest.fixture
def resume_path(tmp_path: Path) -> Path:
    """Write sample resume to a temp file and return its path."""
    p = tmp_path / "resume.md"
    p.write_text(SAMPLE_RESUME)
    return p


@pytest.fixture
def archetypes_path(tmp_path: Path) -> Path:
    """Write sample archetypes to a temp file and return its path."""
    p = tmp_path / "role_archetypes.toml"
    p.write_text(SAMPLE_ARCHETYPES_TOML)
    return p


@pytest.fixture
def archetypes_with_signals_path(tmp_path: Path) -> Path:
    """Write sample archetypes with signals to a temp file and return its path."""
    p = tmp_path / "role_archetypes_signals.toml"
    p.write_text(SAMPLE_ARCHETYPES_WITH_SIGNALS_TOML)
    return p


@pytest.fixture
def rubric_path(tmp_path: Path) -> Path:
    """Write sample global rubric to a temp file and return its path."""
    p = tmp_path / "global_rubric.toml"
    p.write_text(SAMPLE_RUBRIC_TOML)
    return p


# ---------------------------------------------------------------------------
# TestResumeChunking
# ---------------------------------------------------------------------------


class TestResumeChunking:
    """
    REQUIREMENT: Resume is chunked by ## headings for semantic coherence.

    WHO: The indexer preparing resume content for embedding
    WHAT: (1) The system produces one chunk for each ## section heading in a resume.
          (2) The system starts each retrieved chunk with its corresponding ## heading for semantic context.
          (3) The system includes ### sub-headings in the chunk for their parent ## section.
          (4) The system retrieves both chunks successfully by slugified heading IDs.
          (5) The system preserves all sentences in a section chunk without mid-sentence truncation.
          (6) The system excludes the # title line and creates chunks only for ## sections.
          (7) The system produces zero chunks for a resume with no ## section headings.
    WHY: Chunking at section boundaries preserves semantic coherence —
         embedding a mix of "Core Strengths" and "Experience" dilutes both

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O)
        Real:  Indexer.index_resume, _chunk_resume, Embedder (embed, retry,
               truncation), VectorStore (real temp dir)
        Never: Patch chunking logic, Embedder methods, or VectorStore internals
    """

    async def test_resume_is_chunked_by_section_heading(
        self, indexer: Indexer, resume_path: Path
    ) -> None:
        """
        GIVEN a resume with 4 ## section headings
        WHEN index_resume is called
        THEN 4 chunks are produced (one per section).
        """
        # When: index the resume
        count = await indexer.index_resume(str(resume_path))

        # Then: 4 sections = 4 chunks
        assert count == 4, f"Expected 4 chunks, got {count}"

    async def test_each_chunk_starts_with_section_heading(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume indexed into chunks
        WHEN a chunk is retrieved by ID
        THEN it starts with its ## heading for semantic context.
        """
        # Given: indexed resume
        await indexer.index_resume(str(resume_path))

        # When: retrieve the summary chunk
        result = store.get_documents("resume", ids=["resume-summary"])

        # Then: chunk starts with its heading
        assert result["documents"][0].startswith("## Summary"), (
            f"Expected chunk to start with '## Summary', got: {result['documents'][0][:30]!r}"
        )

    async def test_nested_headings_stay_with_parent_section(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume with ### sub-headings under ## Experience
        WHEN the Experience chunk is retrieved
        THEN ### sub-headings are included in the parent section's chunk.
        """
        # Given: indexed resume
        await indexer.index_resume(str(resume_path))

        # When: retrieve the experience chunk
        result = store.get_documents("resume", ids=["resume-experience"])
        doc = result["documents"][0]

        # Then: nested headings present
        assert "### Acme Corp" in doc, "Sub-heading '### Acme Corp' should be in experience chunk"
        assert "### Beta Inc" in doc, "Sub-heading '### Beta Inc' should be in experience chunk"

    async def test_chunk_ids_are_derived_from_heading(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume indexed into chunks
        WHEN chunks are retrieved by slugified heading IDs
        THEN both chunks are found (stable, readable identifiers).
        """
        # Given: indexed resume
        await indexer.index_resume(str(resume_path))

        # When: retrieve by predictable IDs
        result = store.get_documents("resume", ids=["resume-summary", "resume-core-strengths"])

        # Then: both chunks found
        assert len(result["documents"]) == 2, (
            f"Expected 2 chunks by ID, got {len(result['documents'])}"
        )

    async def test_chunks_contain_at_least_one_complete_sentence(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume section with multiple sentences
        WHEN the section chunk is retrieved
        THEN all sentences are present (no mid-sentence truncation).
        """
        # Given: indexed resume
        await indexer.index_resume(str(resume_path))

        # When: retrieve the summary chunk
        result = store.get_documents("resume", ids=["resume-summary"])
        doc = result["documents"][0]

        # Then: both sentences preserved
        assert "distributed systems" in doc, "First sentence content should be present"
        assert "cloud architectures" in doc, "Second sentence content should be present"

    async def test_title_line_is_excluded_from_chunks(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume with a # Name title line
        WHEN index_resume is called
        THEN only ## section chunks are created (title excluded).
        """
        # When: index the resume
        count = await indexer.index_resume(str(resume_path))

        # Then: 4 sections, not 5 (title excluded)
        assert count == 4, f"Expected 4 chunks (title excluded), got {count}"

    async def test_resume_with_no_section_headings_returns_zero_chunks(
        self, indexer: Indexer, store: VectorStore, tmp_path: Path
    ) -> None:
        """
        GIVEN a resume with no ## headings
        WHEN index_resume is called
        THEN zero chunks are produced.
        """
        # Given: flat resume with no sections
        flat_resume = tmp_path / "flat.md"
        flat_resume.write_text("# Just a Title\n\nSome text without section headings.\n")

        # When: index the flat resume
        count = await indexer.index_resume(str(flat_resume))

        # Then: zero chunks
        assert count == 0, f"Expected 0 chunks for headingless resume, got {count}"


# ---------------------------------------------------------------------------
# TestResumeIndexing
# ---------------------------------------------------------------------------


class TestResumeIndexing:
    """
    REQUIREMENT: Resume is indexed into ChromaDB before scoring can proceed.

    WHO: The scorer computing fit_score; the operator running first-time setup
    WHAT: (1) The system stores one document per chunk in the resume collection when it indexes a resume.
          (2) The system replaces previously indexed resume content instead of duplicating it during reindexing.
          (3) The system returns the number of chunks it creates as an integer when it indexes a resume.
          (4) The system records "resume" in each chunk's source metadata for traceability.
          (5) The system raises a CONFIG error that tells the operator to create the missing resume file.
    WHY: An empty resume collection silently produces zero fit_scores for all
         roles — a harder bug to catch than an explicit missing-index error

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O)
        Real:  Indexer.index_resume, Embedder (embed, retry, truncation),
               VectorStore (real temp dir)
        Never: Patch Embedder methods or VectorStore internals
    """

    async def test_embedder_is_called_for_each_chunk(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume with multiple sections
        WHEN index_resume is called
        THEN the returned count matches the number of documents stored.
        """
        # When: index the resume
        count = await indexer.index_resume(str(resume_path))

        # Then: collection has exactly count documents
        assert store.collection_count("resume") == count, (
            f"Expected {count} documents in resume collection, "
            f"got {store.collection_count('resume')}"
        )

    async def test_reindex_replaces_previous_content(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume already indexed
        WHEN index_resume is called again
        THEN previous content is replaced, not duplicated.
        """
        # Given: first index
        await indexer.index_resume(str(resume_path))
        assert store.collection_count("resume") == 4, "First index should produce 4 chunks"

        # When: re-index
        await indexer.index_resume(str(resume_path))

        # Then: count stays the same
        assert store.collection_count("resume") == 4, "Re-index should replace, not duplicate"

    async def test_index_returns_chunk_count(self, indexer: Indexer, resume_path: Path) -> None:
        """
        GIVEN a valid resume file
        WHEN index_resume is called
        THEN it returns the number of chunks created as an integer.
        """
        # When: index the resume
        count = await indexer.index_resume(str(resume_path))

        # Then: returns positive integer
        assert isinstance(count, int), f"Expected int, got {type(count)}"
        assert count > 0, "Should produce at least one chunk"

    async def test_chunk_metadata_records_source(
        self, indexer: Indexer, store: VectorStore, resume_path: Path
    ) -> None:
        """
        GIVEN a resume indexed into chunks
        WHEN chunk metadata is inspected
        THEN the source field records 'resume' for traceability.
        """
        # Given: indexed resume
        await indexer.index_resume(str(resume_path))

        # When: retrieve metadata
        result = store.get_documents("resume", ids=["resume-summary"])

        # Then: source is 'resume'
        assert result["metadatas"][0]["source"] == "resume", (
            f"Expected source='resume', got {result['metadatas'][0].get('source')!r}"
        )

    async def test_missing_resume_file_tells_operator_to_create_it(self, indexer: Indexer) -> None:
        """
        GIVEN a nonexistent resume file path
        WHEN index_resume is called
        THEN a CONFIG error with actionable guidance is raised.
        """
        # When/Then: missing file raises CONFIG error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_resume("/nonexistent/resume.md")

        # Then: error provides guidance
        err = exc_info.value
        assert err.error_type == ErrorType.CONFIG, f"Expected CONFIG error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"


# ---------------------------------------------------------------------------
# TestArchetypeIndexing
# ---------------------------------------------------------------------------


class TestArchetypeIndexing:
    """
    REQUIREMENT: Role archetypes are loaded from TOML and embedded correctly.

    WHO: The scorer computing archetype_score
    WHAT: (1) The system creates exactly one ChromaDB document for each archetype entry in the TOML file.
          (2) The system stores the archetype name in document metadata for debugging.
          (3) The system stores the normalized archetype description as the document text.
          (4) The system normalizes archetype description whitespace so the embedded text has no leading or trailing whitespace and no double spaces.
          (5) The system raises a PARSE error with actionable guidance when the archetypes TOML file contains invalid syntax.
          (6) The system raises a VALIDATION error with actionable guidance when the archetypes TOML file contains no entries.
          (7) The system raises a CONFIG error with actionable guidance when the archetypes file path does not exist.
          (8) The system replaces previously indexed archetypes during reindexing instead of duplicating them.
    WHY: Missing or malformed archetypes silently score all roles equally —
         the most insidious failure mode since ranking still appears to work

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O)
        Real:  Indexer.index_archetypes, TOML parsing, Embedder (embed, retry,
               truncation), VectorStore (real temp dir)
        Never: Patch TOML parsing, Embedder methods, or VectorStore internals
    """

    async def test_each_toml_archetype_produces_one_chroma_document(
        self, indexer: Indexer, store: VectorStore, archetypes_path: Path
    ) -> None:
        """
        GIVEN a TOML file with two archetype entries
        WHEN index_archetypes is called
        THEN each entry produces exactly one ChromaDB document.
        """
        # When: index archetypes
        count = await indexer.index_archetypes(str(archetypes_path))

        # Then: one document per archetype
        assert count == 2, f"Expected 2 archetypes indexed, got {count}"
        assert store.collection_count("role_archetypes") == 2, (
            f"Expected 2 documents in collection, got {store.collection_count('role_archetypes')}"
        )

    async def test_archetype_name_is_stored_as_document_metadata(
        self, indexer: Indexer, store: VectorStore, archetypes_path: Path
    ) -> None:
        """
        GIVEN an indexed archetype collection
        WHEN a document is retrieved by ID
        THEN the archetype name is stored in metadata for debugging.
        """
        # Given: indexed archetypes
        await indexer.index_archetypes(str(archetypes_path))

        # When: retrieve by ID
        result = store.get_documents("role_archetypes", ids=["archetype-staff-platform-architect"])

        # Then: name in metadata
        assert result["metadatas"][0]["name"] == "Staff Platform Architect", (
            f"Expected name='Staff Platform Architect', got {result['metadatas'][0].get('name')!r}"
        )

    async def test_archetype_description_is_the_document_text(
        self, indexer: Indexer, store: VectorStore, archetypes_path: Path
    ) -> None:
        """
        GIVEN an indexed archetype
        WHEN the document text is retrieved
        THEN the normalized description is stored as document content.
        """
        # Given: indexed archetypes
        await indexer.index_archetypes(str(archetypes_path))

        # When: retrieve document text
        result = store.get_documents("role_archetypes", ids=["archetype-staff-platform-architect"])
        doc = result["documents"][0]

        # Then: description content present
        assert "distributed systems" in doc, "Description should contain 'distributed systems'"
        assert "Cloud-native" in doc, "Description should contain 'Cloud-native'"

    async def test_archetype_description_whitespace_is_normalized(
        self, indexer: Indexer, mock_embedder: Embedder, archetypes_path: Path
    ) -> None:
        """
        GIVEN archetype descriptions with extra whitespace
        WHEN index_archetypes embeds them
        THEN no leading/trailing whitespace or double spaces remain.
        """
        # When: index archetypes
        await indexer.index_archetypes(str(archetypes_path))

        # Then: all embedded texts are normalized
        for call in mock_embedder._client.embed.call_args_list:  # type: ignore[union-attr]
            text = str(call.kwargs.get("input", ""))  # pyright: ignore[reportUnknownMemberType, reportUnknownArgumentType]
            assert text == text.strip(), f"Leading/trailing whitespace found: {text!r}"
            assert "  " not in text, f"Double spaces found: {text!r}"
            assert "\n\n" not in text, f"Double newlines found: {text!r}"

    async def test_malformed_toml_identifies_syntax_error_and_file_path(
        self, indexer: Indexer, tmp_path: Path
    ) -> None:
        """
        GIVEN a TOML file with invalid syntax
        WHEN index_archetypes is called
        THEN a PARSE error with actionable guidance is raised.
        """
        # Given: malformed TOML
        bad_toml = tmp_path / "bad.toml"
        bad_toml.write_text("[[archetypes]\nname = broken")  # missing closing bracket

        # When/Then: raises PARSE error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_archetypes(str(bad_toml))

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.PARSE, f"Expected PARSE error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_empty_archetypes_tells_operator_to_add_entries(
        self, indexer: Indexer, tmp_path: Path
    ) -> None:
        """
        GIVEN an empty archetypes TOML file
        WHEN index_archetypes is called
        THEN a VALIDATION error with actionable guidance is raised.
        """
        # Given: empty file
        empty = tmp_path / "empty.toml"
        empty.write_text("")

        # When/Then: raises VALIDATION error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_archetypes(str(empty))

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.VALIDATION, (
            f"Expected VALIDATION error, got {err.error_type}"
        )
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_missing_archetypes_file_tells_operator_to_create_it(
        self, indexer: Indexer
    ) -> None:
        """
        GIVEN a nonexistent archetypes file path
        WHEN index_archetypes is called
        THEN a CONFIG error with actionable guidance is raised.
        """
        # When/Then: raises CONFIG error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_archetypes("/nonexistent/archetypes.toml")

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.CONFIG, f"Expected CONFIG error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_reindex_replaces_previous_archetypes(
        self, indexer: Indexer, store: VectorStore, archetypes_path: Path
    ) -> None:
        """
        GIVEN archetypes already indexed
        WHEN index_archetypes is called again
        THEN previous content is replaced, not duplicated.
        """
        # Given: first index
        await indexer.index_archetypes(str(archetypes_path))
        assert store.collection_count("role_archetypes") == 2, "First index should produce 2"

        # When: re-index
        await indexer.index_archetypes(str(archetypes_path))

        # Then: count stays the same
        assert store.collection_count("role_archetypes") == 2, (
            "Re-index should replace, not duplicate"
        )


# ---------------------------------------------------------------------------
# TestArchetypeEmbeddingSynthesis
# ---------------------------------------------------------------------------


class TestArchetypeEmbeddingSynthesis:
    """
    REQUIREMENT: Archetype embeddings synthesize description + positive signals.

    WHO: The indexer preparing archetype documents for embedding
    WHAT: (1) The system includes the archetype description and every positive signal in the synthesized embedding text.
          (2) The system normalizes extra whitespace in the archetype description before synthesizing embedding text.
          (3) The system returns only the normalized description when the archetype has no positive signals key.
          (4) The system returns only the description when the archetype has an empty positive signals list.
          (5) The system stores synthesized archetype text that includes both description and positive signal content during indexing.
    WHY: Pure prose descriptions embed well for general similarity but miss
         specific keyword anchors the scorer needs to distinguish roles —
         e.g. "cross-team architecture ownership" as a distinct signal

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O, for index_archetypes test)
        Real:  build_archetype_embedding_text, Indexer.index_archetypes,
               Embedder (embed, retry, truncation), VectorStore (real temp dir)
        Never: Patch synthesis logic or whitespace normalization
    """

    def test_synthesis_includes_description_and_signals(self) -> None:
        """
        GIVEN an archetype dict with description and positive signals
        WHEN build_archetype_embedding_text is called
        THEN the result includes both description and all signals.
        """
        # Given: archetype with description and signals
        archetype: dict[str, object] = {
            "description": "  Distributed systems architect.  ",
            "signals_positive": ["Cross-team influence", "API governance"],
        }

        # When: synthesize embedding text
        result = build_archetype_embedding_text(archetype)

        # Then: both description and signals present
        assert "Distributed systems architect." in result, (
            f"Description missing from synthesized text: {result!r}"
        )
        assert "Cross-team influence" in result, (
            f"Signal 'Cross-team influence' missing from synthesized text: {result!r}"
        )
        assert "API governance" in result, (
            f"Signal 'API governance' missing from synthesized text: {result!r}"
        )

    def test_synthesis_normalizes_description_whitespace(self) -> None:
        """
        GIVEN an archetype description with extra whitespace
        WHEN build_archetype_embedding_text is called
        THEN the description whitespace is normalized before synthesis.
        """
        # Given: archetype with messy whitespace
        archetype: dict[str, object] = {
            "description": "  Lots    of   whitespace   here  ",
            "signals_positive": ["Signal A"],
        }

        # When: synthesize
        result = build_archetype_embedding_text(archetype)

        # Then: no double spaces in description line
        assert "  " not in result.split("\n")[0], (
            f"Double spaces remain in description: {result!r}"
        )

    def test_synthesis_without_signals_returns_description_only(self) -> None:
        """
        GIVEN an archetype with no signals_positive key
        WHEN build_archetype_embedding_text is called
        THEN only the normalized description is returned.
        """
        # Given: archetype without signals
        archetype: dict[str, object] = {"description": "  A simple role.  "}

        # When: synthesize
        result = build_archetype_embedding_text(archetype)

        # Then: clean description only
        assert result == "A simple role.", f"Expected clean description only, got: {result!r}"

    def test_synthesis_with_empty_signals_returns_description_only(self) -> None:
        """
        GIVEN an archetype with an empty signals_positive list
        WHEN build_archetype_embedding_text is called
        THEN only the description is returned.
        """
        # Given: archetype with empty signals list
        archetype: dict[str, object] = {"description": "A role.", "signals_positive": []}

        # When: synthesize
        result = build_archetype_embedding_text(archetype)

        # Then: description only
        assert result == "A role.", f"Expected description only for empty signals, got: {result!r}"

    async def test_index_archetypes_uses_synthesized_text(
        self, indexer: Indexer, store: VectorStore, archetypes_with_signals_path: Path
    ) -> None:
        """
        GIVEN archetypes with positive signals defined
        WHEN index_archetypes stores documents
        THEN the stored text includes both description and signal content.
        """
        # When: index archetypes with signals
        await indexer.index_archetypes(str(archetypes_with_signals_path))

        # Then: stored document includes synthesized content
        result = store.get_documents("role_archetypes", ids=["archetype-staff-platform-architect"])
        doc = result["documents"][0]
        assert "Cross-team architecture ownership" in doc, (
            f"Positive signal missing from stored document: {doc!r}"
        )
        assert "distributed systems" in doc.lower(), (
            f"Description content missing from stored document: {doc!r}"
        )


# ---------------------------------------------------------------------------
# TestGlobalRubricLoading
# ---------------------------------------------------------------------------


class TestGlobalRubricLoading:
    """
    REQUIREMENT: Global rubric TOML is loaded and parsed for negative signal extraction.

    WHO: The indexer building the negative_signals collection
    WHAT: (1) The system indexes all negative signals from both the global rubric and the archetypes into the collection.
          (2) The system raises a CONFIG error with actionable guidance when the rubric file is missing.
          (3) The system raises a PARSE error with actionable guidance when the rubric file contains invalid TOML syntax.
    WHY: The global rubric defines universal evaluation criteria that
         apply to all listings regardless of archetype — without it,
         negative scoring is incomplete

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O)
        Real:  Indexer.index_negative_signals, TOML parsing, Embedder (embed,
               retry, truncation), VectorStore
        Never: Patch TOML parsing, Embedder methods, or signal extraction
    """

    async def test_rubric_signals_are_loaded_and_indexed(
        self,
        indexer: Indexer,
        store: VectorStore,
        rubric_path: Path,
        archetypes_with_signals_path: Path,
    ) -> None:
        """
        GIVEN a rubric with 4 negative signals and archetypes with 4
        WHEN index_negative_signals is called
        THEN all 8 signals are indexed into the collection.
        """
        # When: index negative signals
        count = await indexer.index_negative_signals(
            str(rubric_path), str(archetypes_with_signals_path)
        )

        # Then: 4 rubric + 4 archetype = 8 total
        assert count == 8, f"Expected 8 negative signals (4 rubric + 4 archetype), got {count}"
        assert store.collection_count("negative_signals") == 8, (
            f"Expected 8 in collection, got {store.collection_count('negative_signals')}"
        )

    async def test_missing_rubric_file_tells_operator_to_create_it(
        self, indexer: Indexer, archetypes_with_signals_path: Path
    ) -> None:
        """
        GIVEN a nonexistent rubric file path
        WHEN index_negative_signals is called
        THEN a CONFIG error with actionable guidance is raised.
        """
        # When/Then: raises CONFIG error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_negative_signals(
                "/nonexistent/rubric.toml", str(archetypes_with_signals_path)
            )

        # Then: error provides guidance
        err = exc_info.value
        assert err.error_type == ErrorType.CONFIG, f"Expected CONFIG error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_malformed_rubric_identifies_syntax_error(
        self, indexer: Indexer, tmp_path: Path, archetypes_with_signals_path: Path
    ) -> None:
        """
        GIVEN a rubric TOML file with invalid syntax
        WHEN index_negative_signals is called
        THEN a PARSE error with actionable guidance is raised.
        """
        # Given: malformed rubric
        bad_rubric = tmp_path / "bad_rubric.toml"
        bad_rubric.write_text("[[dimensions]\nname = broken")

        # When/Then: raises PARSE error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_negative_signals(
                str(bad_rubric), str(archetypes_with_signals_path)
            )

        # Then: error has guidance
        err = exc_info.value
        assert err.error_type == ErrorType.PARSE, f"Expected PARSE error, got {err.error_type}"
        assert err.suggestion is not None, "Should include a suggestion"


# ---------------------------------------------------------------------------
# TestNegativeSignalIndexing
# ---------------------------------------------------------------------------


class TestNegativeSignalIndexing:
    """
    REQUIREMENT: Negative signals from rubric and archetypes are indexed for penalty scoring.

    WHO: The scorer computing negative_score for each listing
    WHAT: (1) The system indexes negative signals from both the rubric and the archetypes into the collection.
          (2) The system stores one document per signal in the negative_signals collection.
          (3) The system records each indexed negative signal with source metadata prefixed by `rubric:` or `archetype:`.
          (4) The system replaces previously indexed negative signals when reindexing instead of appending duplicates.
          (5) The system indexes only archetype negative signals when the rubric has no dimensions.
          (6) The system raises a CONFIG error with actionable guidance when the archetypes file path does not exist.
          (7) The system raises a PARSE error that names the file and suggests a fix when the archetypes TOML is malformed.
          (8) The system returns 0 indexed negative signals and leaves the collection empty when both sources provide none.
    WHY: A JD about adtech should score high on negative signals regardless
         of which archetype it matches — the global rubric catches universal
         dealbreakers while archetype negatives catch role-specific mismatches

    MOCK BOUNDARY:
        Mock:  ollama.AsyncClient (Ollama HTTP I/O)
        Real:  Indexer.index_negative_signals, signal combining logic,
               Embedder (embed, retry, truncation), VectorStore (real temp dir)
        Never: Patch signal extraction, Embedder methods, or VectorStore internals
    """

    async def test_rubric_and_archetype_negatives_are_combined(
        self,
        indexer: Indexer,
        store: VectorStore,
        rubric_path: Path,
        archetypes_with_signals_path: Path,
    ) -> None:
        """
        GIVEN a rubric and archetypes with negative signals
        WHEN index_negative_signals is called
        THEN both sources contribute to the collection.
        """
        # When: index negative signals
        count = await indexer.index_negative_signals(
            str(rubric_path), str(archetypes_with_signals_path)
        )

        # Then: signals from both sources
        assert count > 0, "Expected at least one negative signal indexed"
        assert store.collection_count("negative_signals") == count, (
            f"Collection count {store.collection_count('negative_signals')} != returned {count}"
        )

    async def test_each_signal_is_individually_embedded(
        self,
        indexer: Indexer,
        store: VectorStore,
        rubric_path: Path,
        archetypes_with_signals_path: Path,
    ) -> None:
        """
        GIVEN a rubric and archetypes with negative signals
        WHEN index_negative_signals is called
        THEN the returned count matches the number of documents stored.
        """
        # When: index negative signals
        count = await indexer.index_negative_signals(
            str(rubric_path), str(archetypes_with_signals_path)
        )

        # Then: collection has exactly count documents
        assert store.collection_count("negative_signals") == count, (
            f"Expected {count} documents in negative_signals collection, "
            f"got {store.collection_count('negative_signals')}"
        )

    async def test_signal_metadata_records_source(
        self,
        indexer: Indexer,
        store: VectorStore,
        rubric_path: Path,
        archetypes_with_signals_path: Path,
    ) -> None:
        """
        GIVEN indexed negative signals from rubric and archetypes
        WHEN signal metadata is inspected
        THEN each source starts with 'rubric:' or 'archetype:'.
        """
        # Given: indexed signals
        await indexer.index_negative_signals(str(rubric_path), str(archetypes_with_signals_path))

        # When: retrieve all metadata
        collection = store.get_or_create_collection("negative_signals")
        result = collection.get(include=["metadatas"])
        metadatas = result["metadatas"]

        # Then: each has correct source prefix
        assert metadatas is not None, "Expected metadatas in collection result"
        for meta in metadatas:
            source = str(meta.get("source", ""))
            assert source.startswith(("rubric:", "archetype:")), (
                f"Signal source should start with 'rubric:' or 'archetype:', got: {source!r}"
            )

    async def test_reindex_replaces_previous_signals(
        self,
        indexer: Indexer,
        store: VectorStore,
        rubric_path: Path,
        archetypes_with_signals_path: Path,
    ) -> None:
        """
        GIVEN negative signals already indexed
        WHEN index_negative_signals is called again
        THEN previous content is replaced, not appended.
        """
        # Given: first index
        count1 = await indexer.index_negative_signals(
            str(rubric_path), str(archetypes_with_signals_path)
        )

        # When: re-index
        count2 = await indexer.index_negative_signals(
            str(rubric_path), str(archetypes_with_signals_path)
        )

        # Then: counts unchanged
        assert count1 == count2, f"Signal count changed on re-index: {count1} → {count2}"
        assert store.collection_count("negative_signals") == count1, (
            "Collection should not grow on re-index"
        )

    async def test_empty_rubric_dimensions_indexes_only_archetype_negatives(
        self,
        indexer: Indexer,
        store: VectorStore,
        tmp_path: Path,
        archetypes_with_signals_path: Path,
    ) -> None:
        """
        GIVEN a rubric with no dimensions
        WHEN index_negative_signals is called
        THEN only archetype negative signals are indexed.
        """
        # Given: empty rubric
        empty_rubric = tmp_path / "empty_rubric.toml"
        empty_rubric.write_text("# No dimensions\n")

        # When: index with empty rubric
        count = await indexer.index_negative_signals(
            str(empty_rubric), str(archetypes_with_signals_path)
        )

        # Then: only archetype negatives (2 per archetype x 2 archetypes)
        assert count == 4, f"Expected 4 archetype-only negative signals, got {count}"

    async def test_missing_archetypes_file_tells_operator(
        self, indexer: Indexer, rubric_path: Path
    ) -> None:
        """
        GIVEN a nonexistent archetypes file path
        WHEN index_negative_signals is called
        THEN a CONFIG error with actionable guidance is raised.
        """
        # When/Then: raises CONFIG error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_negative_signals(str(rubric_path), "/nonexistent/archetypes.toml")

        # Then: correct error type
        err = exc_info.value
        assert err.error_type == ErrorType.CONFIG, f"Expected CONFIG error, got {err.error_type}"

    async def test_malformed_archetypes_toml_produces_actionable_parse_error(
        self, indexer: Indexer, rubric_path: Path, tmp_path: Path
    ) -> None:
        """
        GIVEN an archetypes file with invalid TOML syntax
        WHEN index_negative_signals is called
        THEN a PARSE error is raised naming the file and suggesting a fix.
        """
        # Given: malformed archetypes TOML
        bad_toml = tmp_path / "bad_archetypes.toml"
        bad_toml.write_text('[[archetypes]]\nname = "broken\n')

        # When/Then: raises PARSE error
        with pytest.raises(ActionableError) as exc_info:
            await indexer.index_negative_signals(str(rubric_path), str(bad_toml))

        # Then: error details
        err = exc_info.value
        assert err.error_type == ErrorType.PARSE, (
            f"Expected PARSE error for malformed archetypes TOML, got {err.error_type}"
        )
        assert "role_archetypes" in err.error.lower() or "syntax" in err.error.lower(), (
            f"Error should mention the file or syntax issue: {err.error!r}"
        )

    async def test_zero_signals_from_both_sources_returns_empty_collection(
        self, indexer: Indexer, store: VectorStore, tmp_path: Path
    ) -> None:
        """
        GIVEN a rubric and archetypes with no negative signals
        WHEN index_negative_signals is called
        THEN the method returns 0 and the collection is empty.
        """
        # Given: rubric with dimensions but no negative signals
        empty_rubric = tmp_path / "no_neg_rubric.toml"
        empty_rubric.write_text(
            '[[dimensions]]\nname = "Culture"\nsignals_positive = ["async-first"]\n'
        )
        # Given: archetypes with no negative signals
        empty_archetypes = tmp_path / "no_neg_archetypes.toml"
        empty_archetypes.write_text(
            '[[archetypes]]\nname = "Architect"\ndescription = "Designs systems."\n'
        )

        # When: index negative signals
        count = await indexer.index_negative_signals(str(empty_rubric), str(empty_archetypes))

        # Then: empty collection
        assert count == 0, f"Expected 0 negative signals when both sources are empty, got {count}"
        assert store.collection_count("negative_signals") == 0, (
            "Collection should be empty when no negative signals exist"
        )
