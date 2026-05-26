"""
CLI entry point for the Job Search RAG Assistant.

This module is a thin shim — all command logic lives in :mod:`jobsearch_rag.cli`.
"""

from __future__ import annotations

import sys
from typing import TYPE_CHECKING

from jobsearch_rag.cli import (
    build_parser,
    handle_boards,
    handle_decide,
    handle_decisions,
    handle_eval,
    handle_export,
    handle_index,
    handle_login,
    handle_rescore,
    handle_reset,
    handle_review,
    handle_search,
)
from jobsearch_rag.errors import ActionableError

if TYPE_CHECKING:
    from collections.abc import Callable

HANDLERS: dict[str, Callable[..., None]] = {
    "boards": handle_boards,
    "index": handle_index,
    "search": handle_search,
    "decide": handle_decide,
    "decisions": handle_decisions,
    "review": handle_review,
    "export": handle_export,
    "rescore": handle_rescore,
    "eval": handle_eval,
    "login": handle_login,
    "reset": handle_reset,
}


def main() -> None:
    """Parse CLI arguments and dispatch to the appropriate handler."""
    parser = build_parser()
    args = parser.parse_args()

    try:
        handler = HANDLERS[args.command]
        if args.command == "boards":
            handler()
        else:
            handler(args)
    except Exception as exc:
        # ActionableError instances have rich context
        if isinstance(exc, ActionableError):
            print(f"\nError [{exc.error_type}]: {exc.error}", file=sys.stderr)
            if exc.suggestion:
                print(f"  Suggestion: {exc.suggestion}", file=sys.stderr)
        else:
            print(f"\nUnexpected error: {exc}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
