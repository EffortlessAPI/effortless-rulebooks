"""Adapter registry — IoC loader and factory for job board adapters."""

from __future__ import annotations

from typing import TYPE_CHECKING, ClassVar

if TYPE_CHECKING:
    from jobsearch_rag.adapters.base import JobBoardAdapter


class AdapterRegistry:
    """
    Decorator-based registry that maps board name strings to adapter classes.

    Usage::

        @AdapterRegistry.register
        class ZipRecruiterAdapter(JobBoardAdapter):
            @property
            def board_name(self) -> str:
                return "ziprecruiter"
            ...
    """

    _registry: ClassVar[dict[str, type[JobBoardAdapter]]] = {}

    @classmethod
    def register(cls, adapter_class: type[JobBoardAdapter]) -> type[JobBoardAdapter]:
        """Class decorator — registers an adapter by its ``board_name``."""
        instance = adapter_class.__new__(adapter_class)
        cls._registry[instance.board_name] = adapter_class
        return adapter_class

    @classmethod
    def get(cls, board_name: str) -> JobBoardAdapter:
        """Return a new instance of the adapter registered under *board_name*."""
        if board_name not in cls._registry:
            msg = f"No adapter registered for board: '{board_name}'"
            raise ValueError(msg)
        return cls._registry[board_name]()

    @classmethod
    def list_registered(cls) -> list[str]:
        """Return all registered board name strings."""
        return list(cls._registry.keys())
