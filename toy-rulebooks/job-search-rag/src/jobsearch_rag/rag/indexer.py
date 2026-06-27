"""
Resume, archetype, negative signal, and positive signal ingestion pipeline.

The Indexer coordinates :class:`VectorStore` and :class:`Embedder` to
prepare documents for semantic scoring:

1. **Resume indexing** — splits ``resume.md`` on ``##`` headings, embeds
   each section, and stores it in the ``resume`` collection.  Nested
   ``###`` sub-headings stay with their parent section so context is
   preserved.

2. **Archetype indexing** — loads ``role_archetypes.toml``, synthesizes
   each archetype's description with its positive signals into a richer
   embedding text, embeds it, and stores one document per archetype in
   the ``role_archetypes`` collection.

3. **Negative signal indexing** — loads ``global_rubric.toml`` negatives
   and per-archetype ``signals_negative``, embeds each signal, and
   stores them in the ``negative_signals`` collection for penalty scoring.

4. **Global positive signal indexing** — loads ``global_rubric.toml``
   positive signals, synthesizes each dimension's signals into a single
   embedding, and stores one document per dimension in the
   ``global_positive_signals`` collection for culture scoring.

All operations are **idempotent** — re-indexing resets the collection
first, so documents are replaced rather than accumulated.
"""

from __future__ import annotations

import re
import tomllib
from pathlib import Path
from typing import TYPE_CHECKING, Any, cast

from jobsearch_rag.errors import ActionableError
from jobsearch_rag.logging import logger

if TYPE_CHECKING:
    from jobsearch_rag.rag.embedder import Embedder
    from jobsearch_rag.rag.store import VectorStore

# Regex: match a line starting with ## (but not ###) as a section heading
_SECTION_HEADING_RE = re.compile(r"^## .+", re.MULTILINE)


def _slugify(text: str) -> str:
    """Convert a heading like '## Core Strengths' into 'core-strengths'."""
    text = text.lstrip("#").strip()
    text = re.sub(r"[^a-z0-9]+", "-", text.lower())
    return text.strip("-")


def _normalize_whitespace(text: str) -> str:
    """Collapse runs of whitespace into single spaces, strip edges."""
    return re.sub(r"\s+", " ", text).strip()


def build_archetype_embedding_text(archetype: dict[str, Any]) -> str:
    """
    Synthesize a rich embedding text from an archetype's description and positive signals.

    Combines the normalized description with positive signal phrases so the
    resulting embedding captures both the role narrative and specific signal
    keywords the scorer should match against.

    Parameters
    ----------
    archetype:
        A dict with ``description`` (str) and optionally ``signals_positive``
        (list[str]).

    Returns
    -------
    str
        A single text block suitable for embedding: the normalized description
        followed by each positive signal on its own line.

    """
    description = _normalize_whitespace(str(archetype.get("description", "")))
    raw_signals = archetype.get("signals_positive", [])
    signals: list[str] = cast("list[str]", raw_signals) if isinstance(raw_signals, list) else []
    if not signals:
        return description

    signal_text = "\n".join(f"- {s}" for s in signals)
    return f"{description}\n\nKey signals:\n{signal_text}"


def _chunk_resume(content: str) -> list[tuple[str, str, str]]:
    """
    Split resume markdown into ``(id, heading, body)`` tuples.

    The ``# Title`` block is skipped.  Each ``## Heading`` starts a new
    chunk; ``### Sub-headings`` are included in their parent chunk.
    """
    # Find all ## heading positions
    matches = list(_SECTION_HEADING_RE.finditer(content))
    if not matches:
        return []

    chunks: list[tuple[str, str, str]] = []
    for i, match in enumerate(matches):
        heading = match.group(0)
        start = match.start()
        end = matches[i + 1].start() if i + 1 < len(matches) else len(content)
        body = content[start:end].strip()
        slug = _slugify(heading)
        doc_id = f"resume-{slug}"
        chunks.append((doc_id, heading, body))

    return chunks


class Indexer:
    """
    Chunks and indexes resume and role archetypes into ChromaDB.

    Usage::

        indexer = Indexer(store=vector_store, embedder=embedder)
        n_resume = await indexer.index_resume("data/resume.md")
        n_arch   = await indexer.index_archetypes("config/role_archetypes.toml")
    """

    def __init__(self, store: VectorStore, embedder: Embedder) -> None:
        """Initialize with a vector store and embedder."""
        self._store = store
        self._embedder = embedder

    # -- Resume ingestion ----------------------------------------------------

    async def index_resume(self, resume_path: str) -> int:
        """
        Chunk resume by ``##`` headings and index into the ``resume`` collection.

        Re-indexing resets the collection first (replaces, not appends).
        Returns the number of chunks indexed.

        Raises :class:`~jobsearch_rag.errors.ActionableError`:
          - CONFIG if the resume file doesn't exist
        """
        path = Path(resume_path)
        if not path.exists():
            raise ActionableError.config(
                field_name="resume_path",
                reason=f"Resume file not found: {resume_path}",
                suggestion=f"Create {resume_path} or update the path in settings.toml",
            )

        content = path.read_text(encoding="utf-8")
        chunks = _chunk_resume(content)

        # Reset collection for idempotent re-indexing
        self._store.reset_collection("resume")

        ids: list[str] = []
        documents: list[str] = []
        embeddings: list[list[float]] = []
        metadatas: list[dict[str, str]] = []

        for doc_id, heading, body in chunks:
            embedding = await self._embedder.embed(body)
            ids.append(doc_id)
            documents.append(body)
            embeddings.append(embedding)
            metadatas.append(
                {
                    "source": "resume",
                    "section": heading.lstrip("#").strip(),
                }
            )

        if ids:
            self._store.add_documents(
                collection_name="resume",
                ids=ids,
                documents=documents,
                embeddings=embeddings,
                metadatas=metadatas,
            )

        logger.info("Indexed %d resume chunks from %s", len(ids), resume_path)
        return len(ids)

    # -- Archetype ingestion -------------------------------------------------

    async def index_archetypes(self, archetypes_path: str) -> int:
        """
        Load archetypes from TOML and index into the ``role_archetypes`` collection.

        Each archetype produces one ChromaDB document with the name stored
        as metadata and the normalized description as the document text.
        Re-indexing resets the collection first.
        Returns the number of archetypes indexed.

        Raises :class:`~jobsearch_rag.errors.ActionableError`:
          - CONFIG if the file doesn't exist
          - PARSE if the TOML is malformed
          - VALIDATION if no archetypes are found
        """
        path = Path(archetypes_path)
        if not path.exists():
            raise ActionableError.config(
                field_name="archetypes_path",
                reason=f"Archetypes file not found: {archetypes_path}",
                suggestion=f"Create {archetypes_path} or update the path in settings.toml",
            )

        raw = path.read_text(encoding="utf-8")

        try:
            data = tomllib.loads(raw)
        except tomllib.TOMLDecodeError as exc:
            raise ActionableError.parse(
                board="role_archetypes",
                selector="TOML syntax",
                raw_error=str(exc),
                suggestion=f"Fix TOML syntax in {archetypes_path}",
            ) from None

        archetypes = data.get("archetypes", [])
        if not archetypes:
            raise ActionableError.validation(
                field_name="archetypes",
                reason=f"No archetypes found in {archetypes_path}",
                suggestion="Add at least one [[archetypes]] entry",
            )

        # Reset collection for idempotent re-indexing
        self._store.reset_collection("role_archetypes")

        ids: list[str] = []
        documents: list[str] = []
        embeddings: list[list[float]] = []
        metadatas: list[dict[str, str]] = []

        for arch in archetypes:
            name = arch["name"]
            description = build_archetype_embedding_text(arch)
            slug = _slugify(name)
            doc_id = f"archetype-{slug}"

            embedding = await self._embedder.embed(description)
            ids.append(doc_id)
            documents.append(description)
            embeddings.append(embedding)
            metadatas.append({"name": name, "source": "role_archetypes"})

        self._store.add_documents(
            collection_name="role_archetypes",
            ids=ids,
            documents=documents,
            embeddings=embeddings,
            metadatas=metadatas,
        )

        logger.info("Indexed %d archetypes from %s", len(ids), archetypes_path)
        return len(ids)

    # -- Negative signal ingestion -------------------------------------------

    async def index_negative_signals(
        self,
        rubric_path: str,
        archetypes_path: str,
    ) -> int:
        """
        Load negative signals from global rubric and archetype files, embed, and index.

        Signals are drawn from two sources:

        1. ``global_rubric.toml`` — ``[[dimensions]]`` entries with ``signals_negative``
        2. ``role_archetypes.toml`` — per-archetype ``signals_negative``

        Each signal becomes one document in the ``negative_signals`` collection.
        Metadata records the source (dimension name or archetype name).
        Re-indexing resets the collection first.

        Returns the number of negative signals indexed.

        Raises :class:`~jobsearch_rag.errors.ActionableError`:
          - CONFIG if either file doesn't exist
          - PARSE if TOML is malformed
        """
        # -- Load global rubric signals --
        rubric_file = Path(rubric_path)
        if not rubric_file.exists():
            raise ActionableError.config(
                field_name="global_rubric_path",
                reason=f"Global rubric file not found: {rubric_path}",
                suggestion=f"Create {rubric_path} with [[dimensions]] entries",
            )

        rubric_raw = rubric_file.read_text(encoding="utf-8")
        try:
            rubric_data = tomllib.loads(rubric_raw)
        except tomllib.TOMLDecodeError as exc:
            raise ActionableError.parse(
                board="global_rubric",
                selector="TOML syntax",
                raw_error=str(exc),
                suggestion=f"Fix TOML syntax in {rubric_path}",
            ) from None

        # -- Load archetype negative signals --
        arch_file = Path(archetypes_path)
        if not arch_file.exists():
            raise ActionableError.config(
                field_name="archetypes_path",
                reason=f"Archetypes file not found: {archetypes_path}",
                suggestion=f"Create {archetypes_path} with [[archetypes]] entries",
            )

        arch_raw = arch_file.read_text(encoding="utf-8")
        try:
            arch_data = tomllib.loads(arch_raw)
        except tomllib.TOMLDecodeError as exc:
            raise ActionableError.parse(
                board="role_archetypes",
                selector="TOML syntax",
                raw_error=str(exc),
                suggestion=f"Fix TOML syntax in {archetypes_path}",
            ) from None

        # -- Collect all negative signals --
        signals: list[tuple[str, str, str]] = []  # (id, text, source)

        dimensions = rubric_data.get("dimensions", [])
        for dim in dimensions:
            dim_name = dim.get("name", "unknown")
            for signal in dim.get("signals_negative", []):
                slug = _slugify(signal)
                doc_id = f"neg-{_slugify(dim_name)}-{slug}"
                signals.append((doc_id, signal, f"rubric:{dim_name}"))

        archetypes = arch_data.get("archetypes", [])
        for arch in archetypes:
            arch_name = arch.get("name", "unknown")
            for signal in arch.get("signals_negative", []):
                slug = _slugify(signal)
                doc_id = f"neg-{_slugify(arch_name)}-{slug}"
                signals.append((doc_id, signal, f"archetype:{arch_name}"))

        # Reset collection for idempotent re-indexing
        self._store.reset_collection("negative_signals")

        if not signals:
            logger.info("No negative signals found — collection is empty")
            return 0

        ids: list[str] = []
        documents: list[str] = []
        embeddings: list[list[float]] = []
        metadatas: list[dict[str, str]] = []

        for doc_id, text, source in signals:
            embedding = await self._embedder.embed(text)
            ids.append(doc_id)
            documents.append(text)
            embeddings.append(embedding)
            metadatas.append({"source": source, "signal": text})

        self._store.add_documents(
            collection_name="negative_signals",
            ids=ids,
            documents=documents,
            embeddings=embeddings,
            metadatas=metadatas,
        )

        logger.info(
            "Indexed %d negative signals from %s and %s",
            len(ids),
            rubric_path,
            archetypes_path,
        )
        return len(ids)

    # -- Global positive signal ingestion ------------------------------------

    async def index_global_positive_signals(self, rubric_path: str) -> int:
        """
        Load positive signals from the global rubric and index into ``global_positive_signals``.

        Each dimension that has a ``signals_positive`` list produces one
        ChromaDB document.  The signals are synthesized into a single
        embedding text (dimension name + signal phrases) so the scorer can
        query a JD against the whole rubric dimension in one shot.

        Dimensions that lack ``signals_positive`` are silently skipped.
        Re-indexing resets the collection first.

        Returns the number of dimensions indexed.

        Raises :class:`~jobsearch_rag.errors.ActionableError`:
          - CONFIG if the rubric file doesn't exist
          - PARSE if the TOML is malformed
        """
        path = Path(rubric_path)
        if not path.exists():
            raise ActionableError.config(
                field_name="global_rubric_path",
                reason=f"Global rubric file not found: {rubric_path}",
                suggestion=f"Create {rubric_path} with [[dimensions]] entries",
            )

        raw = path.read_text(encoding="utf-8")
        try:
            data = tomllib.loads(raw)
        except tomllib.TOMLDecodeError as exc:
            raise ActionableError.parse(
                board="global_rubric",
                selector="TOML syntax",
                raw_error=str(exc),
                suggestion=f"Fix TOML syntax in {rubric_path}",
            ) from None

        dimensions = data.get("dimensions", [])

        # Reset collection for idempotent re-indexing
        self._store.reset_collection("global_positive_signals")

        ids: list[str] = []
        documents: list[str] = []
        embeddings: list[list[float]] = []
        metadatas: list[dict[str, str]] = []

        for dim in dimensions:
            dim_name = dim.get("name", "unknown")
            signals = dim.get("signals_positive", [])
            if not signals:
                continue

            # Synthesize embedding text: dimension name + signal phrases
            signal_text = "\n".join(f"- {s}" for s in signals)
            doc_text = f"{dim_name}\n\nPositive signals:\n{signal_text}"

            slug = _slugify(dim_name)
            doc_id = f"pos-{slug}"

            embedding = await self._embedder.embed(doc_text)
            ids.append(doc_id)
            documents.append(doc_text)
            embeddings.append(embedding)
            metadatas.append({"source": dim_name, "signal_count": str(len(signals))})

        if ids:
            self._store.add_documents(
                collection_name="global_positive_signals",
                ids=ids,
                documents=documents,
                embeddings=embeddings,
                metadatas=metadatas,
            )

        logger.info(
            "Indexed %d global positive signal dimensions from %s",
            len(ids),
            rubric_path,
        )
        return len(ids)
