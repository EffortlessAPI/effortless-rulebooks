"""
Type stub for ``_pytest.python_api`` — fills in Unknown parameter types on ``approx``.

Pytest's own inline types leave the ``approx`` parameters untyped, which causes
``reportUnknownMemberType`` under Pyright strict mode.  This stub provides the
real signatures derived from the runtime source so that callers in the test suite
can use ``pytest.approx`` without pragmas.
"""

from collections.abc import Mapping, Sequence

class ApproxBase:
    def __eq__(self, actual: object) -> bool: ...
    def __ne__(self, actual: object) -> bool: ...
    def __repr__(self) -> str: ...

def approx(
    expected: float | int | Sequence[float | int] | Mapping[str, float | int],
    rel: float | None = ...,
    abs: float | None = ...,
    nan_ok: bool = ...,
) -> ApproxBase: ...
