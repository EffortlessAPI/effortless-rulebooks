r"""
Actionable error hierarchy for the Job Search RAG Assistant.

Errors are classified by **recovery path**, not by origin.
Each error type carries structured guidance for three audiences:
  - The calling code (typed ``error_type`` for routing)
  - The human operator (``suggestion`` + ``troubleshooting`` steps)
  - An AI agent (``ai_guidance`` with concrete next actions)

\Built on the ``actionable-errors`` shared library, extended with
RAG-specific error types and domain-aware factory methods.
"""

from __future__ import annotations

from enum import StrEnum

# Re-export base types from the shared library so every consumer
# can keep importing from ``jobsearch_rag.errors``.
from actionable_errors import ActionableError as _ActionableError, AIGuidance, Troubleshooting

__all__ = ["AIGuidance", "ActionableError", "ErrorType", "Troubleshooting"]

# ---------------------------------------------------------------------------
# RAG-specific error classification
# ---------------------------------------------------------------------------


class ErrorType(StrEnum):
    """
    Recovery-path categories — what to *do*, not where it came from.

    Extends the base ``actionable_errors.ErrorType`` with RAG-specific
    values (EMBEDDING, INDEX, PARSE, DECISION).
    """

    AUTHENTICATION = "authentication"
    CONFIG = "config"
    CONNECTION = "connection"
    EMBEDDING = "embedding"
    INDEX = "index"
    PARSE = "parse"
    DECISION = "decision"
    VALIDATION = "validation"
    UNEXPECTED = "unexpected"


# ---------------------------------------------------------------------------
# RAG-specific actionable error
# ---------------------------------------------------------------------------


class ActionableError(_ActionableError):
    """
    Structured error with embedded recovery guidance.

    Inherits dataclass fields, ``__post_init__``, ``to_dict()``, and
    serialization from the shared library.  Factory classmethods below
    encode RAG domain knowledge so callers don't have to.
    """

    # -- factory methods -----------------------------------------------------

    @classmethod
    def authentication(  # type: ignore[override]
        cls,
        board: str,
        raw_error: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Expired session, CAPTCHA, or login failure."""
        return cls(
            error=f"Authentication failed for {board}: {raw_error}",
            error_type=ErrorType.AUTHENTICATION,
            service=board,
            suggestion=suggestion or f"Re-authenticate with {board} (session may have expired)",
            ai_guidance=AIGuidance(
                action_required="Operator must re-authenticate in a headed browser session",
                checks=[
                    f"Check if {board} storage_state JSON exists and is not expired",
                    "Determine whether a CAPTCHA was encountered",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    f"1. Run: python -m jobsearch_rag search --board {board} (headed mode)",
                    "2. Complete login manually when the browser opens",
                    "3. Session cookies will be saved automatically",
                    "4. Re-run the search",
                ]
            ),
        )

    @classmethod
    def config(
        cls,
        field_name: str,
        reason: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Missing or invalid configuration in settings.toml."""
        return cls(
            error=f"Configuration error — {field_name}: {reason}",
            error_type=ErrorType.CONFIG,
            service="settings.toml",
            suggestion=suggestion or f"Fix '{field_name}' in config/settings.toml",
            ai_guidance=AIGuidance(
                action_required=f"Correct the '{field_name}' value in config/settings.toml",
                checks=[
                    "Verify config/settings.toml exists",
                    f"Verify '{field_name}' is present and valid",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    "1. Open config/settings.toml",
                    f"2. Locate the '{field_name}' setting",
                    f"3. Fix the issue: {reason}",
                    "4. Save and re-run",
                ]
            ),
        )

    @classmethod
    def connection(  # type: ignore[override]
        cls,
        service: str,
        url: str,
        raw_error: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Service unreachable (Ollama, ChromaDB, job board)."""
        return cls(
            error=f"Cannot connect to {service} at {url}: {raw_error}",
            error_type=ErrorType.CONNECTION,
            service=service,
            suggestion=suggestion or f"Verify {service} is running at {url}",
            ai_guidance=AIGuidance(
                action_required=f"Verify {service} is reachable",
                command=f"curl -s {url}",
                checks=[
                    f"Is {service} running?",
                    f"Is the URL {url} correct in settings.toml?",
                    "Is a VPN or firewall blocking the connection?",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    f"1. Verify {service} is running",
                    f"2. Test connectivity: curl -s {url}",
                    "3. Check the URL in config/settings.toml matches the running service",
                    "4. Re-run the command",
                ]
            ),
        )

    @classmethod
    def embedding(
        cls,
        model: str,
        raw_error: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Ollama embedding or LLM call failure after retries."""
        return cls(
            error=f"Embedding/LLM call failed for model '{model}': {raw_error}",
            error_type=ErrorType.EMBEDDING,
            service="Ollama",
            suggestion=suggestion or f"Verify model '{model}' is pulled and Ollama is responsive",
            ai_guidance=AIGuidance(
                action_required="Verify Ollama model availability",
                command=f"ollama list | grep {model}",
                checks=[
                    "Is Ollama running?",
                    f"Is model '{model}' pulled? Run: ollama pull {model}",
                    "Is the system under memory pressure?",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    "1. Check Ollama is running: ollama list",
                    f"2. If model missing: ollama pull {model}",
                    "3. If OOM: close other applications and retry",
                    "4. Re-run the command",
                ]
            ),
        )

    @classmethod
    def index(
        cls,
        collection: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """ChromaDB collection missing or empty when scoring needs it."""
        return cls(
            error=f"Collection '{collection}' is empty or missing — run indexing first",
            error_type=ErrorType.INDEX,
            service="ChromaDB",
            suggestion=suggestion
            or f"Run 'python -m jobsearch_rag index' to populate '{collection}'",
            ai_guidance=AIGuidance(
                action_required=f"Index the '{collection}' collection before scoring",
                command="python -m jobsearch_rag index",
                checks=[
                    f"Does data/chroma_db contain the '{collection}' collection?",
                    "Has resume.md or role_archetypes.toml been updated since last index?",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    "1. Run: python -m jobsearch_rag index",
                    f"2. Verify the '{collection}' collection was created",
                    "3. Re-run the search",
                ]
            ),
        )

    @classmethod
    def parse(
        cls,
        board: str,
        selector: str,
        raw_error: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Page structure changed — selector no longer matches."""
        return cls(
            error=f"Parse failure on {board} — selector '{selector}': {raw_error}",
            error_type=ErrorType.PARSE,
            service=board,
            suggestion=suggestion
            or f"The {board} page structure may have changed; update the selector",
            ai_guidance=AIGuidance(
                action_required=f"Inspect {board} page and update selector '{selector}'",
                checks=[
                    f"Open a {board} search result page in a real browser",
                    f"Verify the CSS selector '{selector}' still matches",
                    "Update the adapter if the page structure changed",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    f"1. Open a {board} job listing in your browser",
                    "2. Inspect the page structure with DevTools",
                    f"3. Verify the selector '{selector}' still exists",
                    f"4. Update the {board} adapter if needed",
                ]
            ),
        )

    @classmethod
    def decision(
        cls,
        job_id: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Decision recorded for an unknown job_id."""
        return cls(
            error=f"Cannot record decision — job_id '{job_id}' not found in scored results",
            error_type=ErrorType.DECISION,
            service="decision_history",
            suggestion=suggestion or "Verify the job_id from the latest search output",
            ai_guidance=AIGuidance(
                action_required="Use a valid job_id from the most recent search run",
                discovery_tool="python -m jobsearch_rag export --format csv",
                checks=[
                    "Is the job_id copied correctly?",
                    "Has a search been run since the last export?",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    "1. Check the latest output file for valid job IDs",
                    f"2. Verify '{job_id}' appears in that output",
                    "3. Re-run with the correct job_id",
                ]
            ),
        )

    @classmethod
    def validation(  # type: ignore[override]
        cls,
        field_name: str,
        reason: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Input validation failure (TOML, CLI args, etc.)."""
        return cls(
            error=f"Validation error — {field_name}: {reason}",
            error_type=ErrorType.VALIDATION,
            service="validation",
            suggestion=suggestion or f"Fix '{field_name}': {reason}",
            ai_guidance=AIGuidance(
                action_required=f"Correct the value for '{field_name}'",
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    f"1. Check the value of '{field_name}'",
                    f"2. Issue: {reason}",
                    "3. Correct and retry",
                ]
            ),
        )

    @classmethod
    def unexpected(
        cls,
        service: str,
        operation: str,
        raw_error: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """Catch-all for truly unexpected failures."""
        return cls(
            error=f"Unexpected error in {service} during {operation}: {raw_error}",
            error_type=ErrorType.UNEXPECTED,
            service=service,
            suggestion=suggestion or "This is an unexpected error — check logs for details",
            ai_guidance=AIGuidance(
                action_required="Analyze the error and escalate if needed",
                checks=[
                    "Check the full traceback in logs",
                    f"Is {service} in a known-good state?",
                ],
            ),
            troubleshooting=Troubleshooting(
                steps=[
                    f"1. Check logs for full traceback of the {operation} failure",
                    f"2. Verify {service} is running and in a known-good state",
                    "3. If the error persists, report it with the full traceback",
                ]
            ),
        )

    @classmethod
    def from_exception(
        cls,
        error: Exception,
        service: str,
        operation: str,
        *,
        suggestion: str | None = None,
    ) -> ActionableError:
        """
        Auto-classify an exception by keyword patterns.

        A caller-supplied ``suggestion`` is always preserved — it carries
        context the generic classifier cannot infer.
        """
        error_str = str(error).lower()
        raw_error = str(error)

        if any(kw in error_str for kw in ("unauthorized", "401", "credential", "login")):
            return cls.authentication(service, raw_error, suggestion=suggestion)

        if any(kw in error_str for kw in ("timeout", "timed out")):
            return cls.connection(service, "", raw_error, suggestion=suggestion)

        if any(kw in error_str for kw in ("connection refused", "unreachable", "resolve")):
            return cls.connection(service, "", raw_error, suggestion=suggestion)

        if any(kw in error_str for kw in ("not found", "404", "no such")):
            return cls.unexpected(service, operation, raw_error, suggestion=suggestion)

        return cls.unexpected(service, operation, raw_error, suggestion=suggestion)
