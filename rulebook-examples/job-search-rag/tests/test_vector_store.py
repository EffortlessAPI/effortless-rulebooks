"""
VectorStore tests — ChromaDB collection management and querying.

Maps to BDD specs: TestCollectionLifecycle, TestDocumentOperations,
TestSimilarityQuery, TestStoreErrors

Spec classes:
    TestCollectionLifecycle
    TestDocumentOperations
    TestSimilarityQuery
    TestStoreErrors
    TestMetadataQuery
"""

from __future__ import annotations

import tempfile
from typing import TYPE_CHECKING

import pytest

if TYPE_CHECKING:
    from collections.abc import Iterator

from jobsearch_rag.errors import ActionableError, ErrorType
from jobsearch_rag.rag.store import VectorStore

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

SAMPLE_DOCS = [
    "Staff Platform Architect designing distributed systems",
    "Principal Data Engineer building streaming pipelines",
    "Developer Relations evangelist creating SDK documentation",
]

SAMPLE_IDS = ["doc-1", "doc-2", "doc-3"]

SAMPLE_METADATA = [
    {"source": "resume", "section": "experience"},
    {"source": "resume", "section": "experience"},
    {"source": "resume", "section": "skills"},
]

# Fake embedding vectors — 5 dimensions is enough for tests.
# Vectors are directionally meaningful so similarity tests work:
#   doc-1 and doc-3 are somewhat similar (both about leadership)
#   doc-2 points in a different direction (data engineering)
EMBED_DIM = 5
EMBED_1 = [0.9, 0.1, 0.2, 0.0, 0.3]  # architect / leadership
EMBED_2 = [0.1, 0.8, 0.1, 0.7, 0.0]  # data engineering
EMBED_3 = [0.7, 0.2, 0.3, 0.0, 0.4]  # devrel / leadership-adjacent
SAMPLE_EMBEDDINGS = [EMBED_1, EMBED_2, EMBED_3]


@pytest.fixture
def store() -> Iterator[VectorStore]:
    """Create a VectorStore backed by a temporary directory."""
    with tempfile.TemporaryDirectory() as tmpdir:
        yield VectorStore(persist_dir=tmpdir)


@pytest.fixture
def populated_store(store: VectorStore) -> VectorStore:
    """A VectorStore with three documents already added to 'test_collection'."""
    store.add_documents(
        collection_name="test_collection",
        ids=SAMPLE_IDS,
        documents=SAMPLE_DOCS,
        embeddings=SAMPLE_EMBEDDINGS,
        metadatas=SAMPLE_METADATA,
    )
    return store


# ---------------------------------------------------------------------------
# TestCollectionLifecycle
# ---------------------------------------------------------------------------


class TestCollectionLifecycle:
    """
    REQUIREMENT: Collections are created, retrieved, and reset reliably.

    WHO: The indexer managing ChromaDB collections
    WHAT: (1) The system creates and returns a new collection when get_or_create_collection is called for an empty store.
          (2) The system returns the existing collection instead of creating a duplicate when get_or_create_collection is called again with the same name.
          (3) The system reports zero documents for a freshly created collection.
          (4) The system reports a document count of 3 after 3 documents are added to a collection.
          (5) The system drops all documents and reports a document count of zero when reset_collection is called on a populated collection.
          (6) The system performs a safe no-op without raising an exception when reset_collection is called for a nonexistent collection.
    WHY: Stale or phantom collections lead to scoring against outdated data —
         a silent correctness bug that's hard to diagnose

    MOCK BOUNDARY:
        Mock: nothing — uses real ChromaDB via tmpdir
        Real: VectorStore, get_or_create_collection, reset_collection, collection_count
        Never: Patch ChromaDB internals
    """

    def test_get_or_create_returns_collection(self, store: VectorStore) -> None:
        """
        GIVEN an empty store
        WHEN get_or_create_collection is called
        THEN a new collection is created and returned.
        """
        # When: create a collection
        collection = store.get_or_create_collection("new_collection")

        # Then: collection is returned
        assert collection is not None, "Should return a usable collection"

    def test_get_or_create_is_idempotent(self, store: VectorStore) -> None:
        """
        GIVEN a collection already created
        WHEN get_or_create_collection is called again with the same name
        THEN the same collection is returned, not a duplicate.
        """
        # Given: first creation
        c1 = store.get_or_create_collection("same_name")

        # When: second creation with same name
        c2 = store.get_or_create_collection("same_name")

        # Then: same collection returned
        assert c1.name == c2.name, "Should return the same collection, not a duplicate"

    def test_new_collection_has_zero_documents(self, store: VectorStore) -> None:
        """
        GIVEN a freshly created collection
        WHEN collection_count is checked
        THEN the document count is zero.
        """
        # Given: new collection
        store.get_or_create_collection("empty")

        # Then: count is zero
        assert store.collection_count("empty") == 0, "New collection should have zero documents"

    def test_collection_count_reflects_added_documents(self, populated_store: VectorStore) -> None:
        """
        GIVEN a collection with 3 documents added
        WHEN collection_count is checked
        THEN it returns 3.
        """
        # Then: count matches added documents
        assert populated_store.collection_count("test_collection") == 3, (
            "Count should match number of added documents"
        )

    def test_reset_drops_all_documents(self, populated_store: VectorStore) -> None:
        """
        GIVEN a populated collection
        WHEN reset_collection is called
        THEN all documents are dropped and count returns to zero.
        """
        # When: reset the collection
        populated_store.reset_collection("test_collection")

        # Then: count is zero
        assert populated_store.collection_count("test_collection") == 0, (
            "Reset should drop all documents"
        )

    def test_reset_nonexistent_collection_does_not_raise(self, store: VectorStore) -> None:
        """
        GIVEN a collection name that doesn't exist
        WHEN reset_collection is called
        THEN it is a safe no-op — no exception is raised.
        """
        # When/Then: reset non-existent collection (should not raise)
        store.reset_collection("never_existed")


# ---------------------------------------------------------------------------
# TestDocumentOperations
# ---------------------------------------------------------------------------


class TestDocumentOperations:
    """
    REQUIREMENT: Documents can be added and retrieved with metadata.

    WHO: The indexer populating collections with resume chunks and archetypes
    WHAT: (1) The system returns the matching document when get_documents is called with a specific ID.
          (2) The system preserves and returns the correct metadata when a document is retrieved by ID.
          (3) The system updates an existing document instead of creating a duplicate when add_documents is called with the same ID.
          (4) The system raises a validation error that identifies the mismatch when add_documents is called with mismatched ID and document lengths.
    WHY: Duplicate documents inflate similarity results; lost metadata
         prevents score explanation and debugging

    MOCK BOUNDARY:
        Mock: nothing — uses real ChromaDB via tmpdir
        Real: VectorStore.add_documents, get_documents, collection_count
        Never: Patch ChromaDB internals or embedding storage
    """

    def test_documents_are_retrievable_by_id(self, populated_store: VectorStore) -> None:
        """
        GIVEN a populated collection
        WHEN get_documents is called with a specific ID
        THEN the matching document is returned.
        """
        # When: retrieve by ID
        result = populated_store.get_documents("test_collection", ids=["doc-1"])

        # Then: document is returned
        assert len(result["documents"]) == 1, "Should return exactly one document"
        assert "Staff Platform Architect" in result["documents"][0], (
            "Returned document should match the original"
        )

    def test_metadata_is_preserved(self, populated_store: VectorStore) -> None:
        """
        GIVEN a document added with metadata
        WHEN retrieved by ID
        THEN the metadata is preserved and correct.
        """
        # When: retrieve document
        result = populated_store.get_documents("test_collection", ids=["doc-3"])

        # Then: metadata preserved
        assert result["metadatas"][0]["section"] == "skills", (
            "Metadata should be preserved on retrieval"
        )

    def test_add_with_duplicate_id_updates_document(self, populated_store: VectorStore) -> None:
        """
        GIVEN a document already in the collection
        WHEN add_documents is called with the same ID
        THEN it updates rather than creating a duplicate.
        """
        # When: add with existing ID
        populated_store.add_documents(
            collection_name="test_collection",
            ids=["doc-1"],
            documents=["Updated architect description"],
            embeddings=[EMBED_1],
            metadatas=[{"source": "resume", "section": "updated"}],
        )

        # Then: count unchanged, document updated
        assert populated_store.collection_count("test_collection") == 3, (
            "Duplicate ID should update, not append"
        )
        result = populated_store.get_documents("test_collection", ids=["doc-1"])
        assert "Updated" in result["documents"][0], "Document text should be updated"

    def test_add_documents_with_mismatched_lengths_names_the_mismatch(
        self, store: VectorStore
    ) -> None:
        """
        GIVEN mismatched lengths between IDs and documents
        WHEN add_documents is called
        THEN a VALIDATION error is raised naming the mismatch.
        """
        # When/Then: mismatched lengths raise ActionableError
        with pytest.raises(ActionableError) as exc_info:
            store.add_documents(
                collection_name="test_collection",
                ids=["doc-1"],
                documents=["one", "two"],  # 2 docs but only 1 id
                embeddings=[EMBED_1, EMBED_2],
            )

        # Then: error is VALIDATION with guidance
        err = exc_info.value
        assert err.error_type == ErrorType.VALIDATION, "Error type should be VALIDATION"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"


# ---------------------------------------------------------------------------
# TestSimilarityQuery
# ---------------------------------------------------------------------------


class TestSimilarityQuery:
    """
    REQUIREMENT: Similarity queries return documents ranked by closeness.

    WHO: The scorer computing fit, archetype, and history scores
    WHAT: (1) The system returns the architect document first when the query vector is most similar to it.
          (2) The system includes similarity distance scores in the query results as a list of floats.
          (3) The system limits the query output to one result when `n_results=1`.
          (4) The system returns an empty result for an empty collection instead of raising an error.
          (5) The system includes the original document text and metadata in the query results.
    WHY: Incorrect similarity ordering would silently invert job rankings —
         the most dangerous class of bug in the system

    MOCK BOUNDARY:
        Mock: nothing — uses real ChromaDB via tmpdir
        Real: VectorStore.query, similarity ranking, distance computation
        Never: Patch ChromaDB query internals or distance functions
    """

    def test_query_returns_most_similar_document_first(self, populated_store: VectorStore) -> None:
        """
        GIVEN a populated collection with directional embeddings
        WHEN querying with an architect-like vector
        THEN the architect document is returned first.
        """
        # When: query with architect-direction vector
        results = populated_store.query(
            collection_name="test_collection",
            query_embedding=EMBED_1,
            n_results=3,
        )

        # Then: doc-1 (architect) is most similar
        assert results["ids"][0][0] == "doc-1", "Architect document should be most similar"

    def test_query_returns_similarity_distances(self, populated_store: VectorStore) -> None:
        """
        GIVEN a populated collection
        WHEN a similarity query is run
        THEN results include distance scores as a list of floats.
        """
        # When: query
        results = populated_store.query(
            collection_name="test_collection",
            query_embedding=EMBED_1,
            n_results=2,
        )

        # Then: distances are floats
        distances = results["distances"][0]
        assert len(distances) == 2, "Should return 2 distance values"
        assert all(isinstance(d, float) for d in distances), "Distances should be floats"

    def test_n_results_limits_output(self, populated_store: VectorStore) -> None:
        """
        GIVEN a collection with 3 documents
        WHEN querying with n_results=1
        THEN only 1 result is returned.
        """
        # When: query with limit
        results = populated_store.query(
            collection_name="test_collection",
            query_embedding=EMBED_1,
            n_results=1,
        )

        # Then: exactly 1 result
        assert len(results["ids"][0]) == 1, "Should return exactly 1 result"

    def test_query_empty_collection_returns_empty(self, store: VectorStore) -> None:
        """
        GIVEN an empty collection
        WHEN a similarity query is run
        THEN an empty result is returned, not an error.
        """
        # Given: empty collection
        store.get_or_create_collection("empty")

        # When: query empty collection
        results = store.query(
            collection_name="empty",
            query_embedding=EMBED_1,
            n_results=5,
        )

        # Then: empty results
        assert results["ids"][0] == [], "Empty collection should return empty results"

    def test_query_includes_document_text_and_metadata(self, populated_store: VectorStore) -> None:
        """
        GIVEN a populated collection
        WHEN a similarity query is run
        THEN results include the original document text and metadata.
        """
        # When: query
        results = populated_store.query(
            collection_name="test_collection",
            query_embedding=EMBED_1,
            n_results=1,
        )

        # Then: document and metadata present
        assert results["documents"][0][0] is not None, "Document text should be included"
        assert results["metadatas"][0][0] is not None, "Metadata should be included"


# ---------------------------------------------------------------------------
# TestStoreErrors
# ---------------------------------------------------------------------------


class TestStoreErrors:
    """
    REQUIREMENT: Store errors are actionable and classified correctly.

    WHO: The pipeline runner catching errors to present clear guidance
    WHAT: (1) The system raises an INDEX error that tells the operator to run the index command when `query` is called on a nonexistent collection.
          (2) The system names the nonexistent collection and provides step-by-step guidance in the INDEX error when `query` is called.
          (3) The system raises an INDEX error with actionable guidance when `get_documents` is called on a nonexistent collection.
          (4) The system raises an INDEX error with actionable guidance when `collection_count` is called on a nonexistent collection.
    WHY: Generic exceptions force operators to read stack traces —
         actionable errors tell them exactly what to fix

    MOCK BOUNDARY:
        Mock: nothing — uses real ChromaDB via tmpdir
        Real: VectorStore error paths, ActionableError classification
        Never: Patch error construction or ErrorType enum
    """

    def test_query_nonexistent_collection_tells_operator_to_run_index(
        self, store: VectorStore
    ) -> None:
        """
        GIVEN a collection that doesn't exist
        WHEN query is called
        THEN an INDEX error is raised telling the operator to run the index command.
        """
        # When/Then: query nonexistent collection raises ActionableError
        with pytest.raises(ActionableError) as exc_info:
            store.query(
                collection_name="nonexistent",
                query_embedding=EMBED_1,
                n_results=5,
            )

        # Then: error is INDEX with guidance
        err = exc_info.value
        assert err.error_type == ErrorType.INDEX, "Error type should be INDEX"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have steps"

    def test_index_error_names_collection_and_provides_guidance(self, store: VectorStore) -> None:
        """
        GIVEN a nonexistent collection
        WHEN query is called
        THEN the INDEX error names the collection and provides step-by-step guidance.
        """
        # When/Then: query nonexistent collection
        with pytest.raises(ActionableError) as exc_info:
            store.query(
                collection_name="nonexistent",
                query_embedding=EMBED_1,
                n_results=5,
            )

        # Then: error names the collection
        err = exc_info.value
        assert "nonexistent" in err.error, "Error should name the collection"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_get_documents_nonexistent_collection_provides_guidance(
        self, store: VectorStore
    ) -> None:
        """
        GIVEN a nonexistent collection
        WHEN get_documents is called
        THEN an INDEX error is raised with actionable guidance.
        """
        # When/Then: get from nonexistent collection
        with pytest.raises(ActionableError) as exc_info:
            store.get_documents("nonexistent", ids=["doc-1"])

        # Then: error is INDEX with guidance
        err = exc_info.value
        assert err.error_type == ErrorType.INDEX, "Error type should be INDEX"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_collection_count_nonexistent_provides_guidance(self, store: VectorStore) -> None:
        """
        GIVEN a nonexistent collection
        WHEN collection_count is called
        THEN an INDEX error is raised with actionable guidance.
        """
        # When/Then: count nonexistent collection
        with pytest.raises(ActionableError) as exc_info:
            store.collection_count("nonexistent")

        # Then: error is INDEX with guidance
        err = exc_info.value
        assert err.error_type == ErrorType.INDEX, "Error type should be INDEX"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"


# ---------------------------------------------------------------------------
# TestMetadataQuery
# ---------------------------------------------------------------------------


class TestMetadataQuery:
    """
    REQUIREMENT: Documents can be retrieved by metadata filter.

    WHO: The scorer retrieving past rejection reasons for disqualifier augmentation
    WHAT: (1) The system returns only documents whose metadata matches the requested value.
          (2) The system returns an empty result when no documents match the requested metadata value.
          (3) The system raises an actionable INDEX error when get_by_metadata is called on a nonexistent collection.
    WHY: The disqualifier prompt needs past 'no' reasons to learn the operator's
         personal rejection patterns — metadata queries make this possible

    MOCK BOUNDARY:
        Mock: nothing — uses real ChromaDB via tmpdir
        Real: VectorStore.get_by_metadata, add_documents, metadata filtering
        Never: Patch ChromaDB metadata internals
    """

    def test_get_by_metadata_returns_matching_documents(
        self, populated_store: VectorStore
    ) -> None:
        """
        GIVEN a collection with mixed metadata values
        WHEN get_by_metadata filters on a specific value
        THEN only matching documents are returned.
        """
        # Given: documents with different verdict metadata
        populated_store.add_documents(
            collection_name="decisions",
            ids=["decision-1", "decision-2", "decision-3"],
            documents=["Great role", "Bad role", "Another bad role"],
            embeddings=[EMBED_1, EMBED_2, EMBED_3],
            metadatas=[
                {"verdict": "yes", "reason": ""},
                {"verdict": "no", "reason": "on-call required"},
                {"verdict": "no", "reason": "fully on-site"},
            ],
        )

        # When: filter by verdict=no
        results = populated_store.get_by_metadata(
            "decisions",
            where={"verdict": "no"},
            include=["metadatas"],
        )

        # Then: only 'no' verdicts returned
        assert len(results["ids"]) == 2, "Should return 2 matching documents"
        reasons = [m["reason"] for m in results["metadatas"]]
        assert "on-call required" in reasons, "First reason should match"
        assert "fully on-site" in reasons, "Second reason should match"

    def test_get_by_metadata_returns_empty_when_no_match(
        self, populated_store: VectorStore
    ) -> None:
        """
        GIVEN a collection with documents
        WHEN get_by_metadata filters on a value that matches nothing
        THEN an empty result is returned.
        """
        # Given: a collection with only 'yes' verdicts
        populated_store.add_documents(
            collection_name="decisions",
            ids=["decision-only-yes"],
            documents=["A good role"],
            embeddings=[EMBED_1],
            metadatas=[{"verdict": "yes", "reason": ""}],
        )

        # When: filter by verdict=no
        results = populated_store.get_by_metadata(
            "decisions",
            where={"verdict": "no"},
            include=["metadatas"],
        )

        # Then: no documents match
        assert len(results["ids"]) == 0, "Should return no matching documents"

    def test_get_by_metadata_nonexistent_collection_raises_index_error(
        self, store: VectorStore
    ) -> None:
        """
        GIVEN a nonexistent collection
        WHEN get_by_metadata is called
        THEN an actionable INDEX error is raised.
        """
        # When/Then: metadata query on nonexistent collection
        with pytest.raises(ActionableError) as exc_info:
            store.get_by_metadata(
                "nonexistent_collection",
                where={"verdict": "no"},
            )

        # Then: error is INDEX with suggestion
        err = exc_info.value
        assert err.error_type == ErrorType.INDEX, "Error type should be INDEX"
        assert err.suggestion is not None, "Should include a suggestion"
