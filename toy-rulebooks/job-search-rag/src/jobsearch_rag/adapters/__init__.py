"""
Adapter layer — IoC / Strategy pattern for job board integrations.

Importing this package triggers adapter registration via the
``@AdapterRegistry.register`` decorator on each concrete adapter.
"""

# Import concrete adapters to trigger registration
from jobsearch_rag.adapters import (
    indeed as _indeed,  # noqa: F401  # pyright: ignore[reportUnusedImport]  # side-effect: triggers @register
    linkedin as _linkedin,  # noqa: F401  # pyright: ignore[reportUnusedImport]  # side-effect: triggers @register
    weworkremotely as _wwr,  # noqa: F401  # pyright: ignore[reportUnusedImport]  # side-effect: triggers @register
    ziprecruiter as _zr,  # noqa: F401  # pyright: ignore[reportUnusedImport]  # side-effect: triggers @register
)
from jobsearch_rag.adapters.base import JobBoardAdapter, JobListing
from jobsearch_rag.adapters.registry import AdapterRegistry

__all__ = ["AdapterRegistry", "JobBoardAdapter", "JobListing"]
