"""
ChromaDB collection management.

Provides a thin wrapper around ChromaDB's client, adding:
- Consistent error handling via ActionableError
- Collection lifecycle (create, count, reset, query)
- Input validation for document operations

ChromaDB is an **embedded** vector database — like SQLite for vectors.
It stores documents alongside their embedding vectors and supports
similarity queries: "give me the N documents most similar to this vector."

Three collections serve distinct scoring purposes:

  - ``resume``         — resume chunks for fit_score
  - ``role_archetypes`` — ideal role descriptions for archetype_score
  - ``decisions``       — past accept/reject choices for history_score
"""

from __future__ import annotations

from typing import Any

import chromadb

from jobsearch_rag.errors import ActionableError
from jobsearch_rag.logging import logger


class VectorStore:
    """
    Manages ChromaDB collections for resume, archetypes, and decisions.

    Usage::

        store = VectorStore(persist_dir="./data/chroma_db")
        store.get_or_create_collection("resume")
        store.add_documents("resume", ids=[...], documents=[...], embeddings=[...])
        results = store.query("resume", query_embedding=[...], n_results=5)
    """

    def __init__(self, persist_dir: str) -> None:
        """Initialize ChromaDB client at *persist_dir*."""
        self.persist_dir = persist_dir
        self._client = chromadb.PersistentClient(path=persist_dir)
        logger.debug("ChromaDB client initialized at %s", persist_dir)

    # -- Collection lifecycle ------------------------------------------------

    def get_or_create_collection(self, name: str) -> chromadb.Collection:
        """
        Return the named ChromaDB collection, creating if necessary.

        Uses cosine similarity as the distance function — the natural
        choice for comparing text embeddings.
        """
        collection = self._client.get_or_create_collection(
            name=name,
            metadata={"hnsw:space": "cosine"},
        )
        logger.debug("Collection '%s' ready (%d documents)", name, collection.count())
        return collection

    def collection_count(self, name: str) -> int:
        """
        Return the document count for collection *name*.

        Raises :class:`~jobsearch_rag.errors.ActionableError` (INDEX)
        if the collection does not exist.
        """
        collection = self._get_existing_collection(name)
        return collection.count()

    def reset_collection(self, name: str) -> None:
        """
        Drop and recreate the named collection.

        Safe to call on nonexistent collections (no-op for the delete,
        but always ensures the collection exists and is empty afterward).
        """
        try:
            self._client.delete_collection(name)
            logger.info("Collection '%s' deleted", name)
        except chromadb.errors.NotFoundError:  # pyright: ignore[reportAttributeAccessIssue, reportUnknownMemberType]
            logger.debug("Collection '%s' does not exist — nothing to reset", name)
        # Recreate empty so callers can immediately use the collection
        self.get_or_create_collection(name)
        logger.debug("Collection '%s' recreated empty", name)

    # -- Document operations -------------------------------------------------

    def add_documents(
        self,
        collection_name: str,
        *,
        ids: list[str],
        documents: list[str],
        embeddings: list[list[float]],
        metadatas: list[dict[str, Any]] | None = None,
    ) -> None:
        """
        Add (or update) documents with pre-computed embeddings.

        All list arguments must have the same length. Documents with
        existing IDs are **upserted** (updated in place).

        Args:
            collection_name: Target collection (created if absent).
            ids: Unique document identifiers.
            documents: Raw text content.
            embeddings: Pre-computed embedding vectors.
            metadatas: Optional per-document metadata dicts.

        """
        # Validate lengths match
        lengths = {"ids": len(ids), "documents": len(documents), "embeddings": len(embeddings)}
        if metadatas is not None:
            lengths["metadatas"] = len(metadatas)
        unique_lengths = set(lengths.values())
        if len(unique_lengths) > 1:
            raise ActionableError.validation(
                field_name="input_lengths",
                reason=f"Mismatched input lengths: {lengths}",
                suggestion="Ensure ids, documents, embeddings, and metadatas all have the same length",
            )

        collection = self.get_or_create_collection(collection_name)
        collection.upsert(
            ids=ids,
            documents=documents,
            embeddings=embeddings,  # type: ignore[arg-type]
            metadatas=metadatas,  # type: ignore[arg-type]
        )
        logger.info(
            "Upserted %d documents into '%s' (total: %d)",
            len(ids),
            collection_name,
            collection.count(),
        )

    def get_documents(
        self,
        collection_name: str,
        *,
        ids: list[str],
    ) -> dict[str, Any]:
        """
        Retrieve documents by ID from a collection.

        Returns a dict with ``ids``, ``documents``, ``metadatas`` keys
        matching ChromaDB's native format.

        Raises :class:`~jobsearch_rag.errors.ActionableError` (INDEX)
        if the collection does not exist.
        """
        collection = self._get_existing_collection(collection_name)
        result = collection.get(ids=ids, include=["documents", "metadatas"])
        return dict(result)

    def get_by_metadata(
        self,
        collection_name: str,
        *,
        where: dict[str, Any],
        include: list[str] | None = None,
    ) -> dict[str, Any]:
        """
        Retrieve documents matching a metadata filter.

        Uses ChromaDB's ``where`` filter syntax (e.g.
        ``{"verdict": "no"}``).  Returns a dict with keys matching
        ChromaDB's native format (``ids``, ``metadatas``, etc.).

        Raises :class:`~jobsearch_rag.errors.ActionableError` (INDEX)
        if the collection does not exist.
        """
        collection = self._get_existing_collection(collection_name)
        include = include or ["metadatas"]
        result = collection.get(where=where, include=include)  # type: ignore[arg-type]
        return dict(result)

    # -- Similarity query ----------------------------------------------------

    def delete_by_id(
        self,
        collection_name: str,
        *,
        ids: list[str],
    ) -> None:
        """
        Delete documents by ID from a collection.

        Silently ignores IDs that do not exist in the collection.

        Raises :class:`~jobsearch_rag.errors.ActionableError` (INDEX)
        if the collection does not exist.
        """
        collection = self._get_existing_collection(collection_name)
        collection.delete(ids=ids)
        logger.info(
            "Deleted %d document(s) from '%s' (total: %d)",
            len(ids),
            collection_name,
            collection.count(),
        )

    # -- Similarity query (continued) ----------------------------------------

    def query(
        self,
        collection_name: str,
        *,
        query_embedding: list[float],
        n_results: int = 5,
    ) -> dict[str, Any]:
        """
        Find the *n_results* most similar documents to *query_embedding*.

        Returns a dict with ``ids``, ``documents``, ``metadatas``,
        ``distances`` keys. Distances are cosine distances (lower = more
        similar; 0.0 = identical direction).

        Raises :class:`~jobsearch_rag.errors.ActionableError` (INDEX)
        if the collection does not exist.
        """
        collection = self._get_existing_collection(collection_name)

        # ChromaDB raises if n_results > count; clamp to available
        count = collection.count()
        if count == 0:
            return {"ids": [[]], "documents": [[]], "metadatas": [[]], "distances": [[]]}

        effective_n = min(n_results, count)
        result = collection.query(
            query_embeddings=[query_embedding],  # type: ignore[arg-type]
            n_results=effective_n,
            include=["documents", "metadatas", "distances"],
        )
        return dict(result)

    # -- Internal helpers ----------------------------------------------------

    def _get_existing_collection(self, name: str) -> chromadb.Collection:
        """
        Retrieve a collection that must already exist.

        Raises :class:`~jobsearch_rag.errors.ActionableError` (INDEX)
        if the collection has not been created.
        """
        try:
            return self._client.get_collection(name)
        except chromadb.errors.NotFoundError:  # pyright: ignore[reportAttributeAccessIssue, reportUnknownMemberType]
            raise ActionableError.index(name) from None
