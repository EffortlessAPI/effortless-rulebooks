"""
Ollama embedding wrapper with retry logic.

Wraps the ``ollama`` Python SDK's :class:`AsyncClient` to provide:

- **Embedding**: text → float vector via ``nomic-embed-text``
- **Classification**: prompt → LLM response via ``mistral:7b``
- **Health check**: verify Ollama + models are available at startup
- **Retry with backoff**: transient 5xx errors are retried up to
  ``max_retries`` times with exponential backoff before giving up

All errors are converted to :class:`~jobsearch_rag.errors.ActionableError`
with operator-friendly guidance.
"""

from __future__ import annotations

import asyncio
import time
from dataclasses import dataclass
from typing import TYPE_CHECKING, TypeVar

if TYPE_CHECKING:
    from collections.abc import Awaitable, Callable

import ollama as ollama_sdk

from jobsearch_rag.errors import ActionableError
from jobsearch_rag.logging import log_event, logger

_T = TypeVar("_T")

# Status codes that warrant a retry (server overloaded / temporary failure)
_RETRYABLE_STATUS_CODES = {408, 429, 500, 502, 503, 504}

# nomic-embed-text context window is 8192 tokens.  Real-world JDs contain
# newlines, bullet points, dollar signs, and Unicode chars that tokenise at
# ~1 char/token rather than the ~4 average for clean English prose.  Empirical
# testing shows formatted JD text breaks at ~9 200 chars; 8 000 gives safe
# headroom.  The scorer chunks long JDs at this size so no content is lost.
_DEFAULT_MAX_EMBED_CHARS = 8_000

# When truncation is needed, keep head (title, company, overview) and tail
# (hands-on work, comp range, specific tech) — the middle is typically "about
# us", values statements, and repeated boilerplate.  60/40 split.
_HEAD_RATIO = 0.6
_TRUNCATION_MARKER = "\n[…]\n"


@dataclass
class InferenceMetrics:
    """Accumulated inference metrics for a single session."""

    embed_calls: int = 0
    embed_tokens_total: int = 0
    llm_calls: int = 0
    llm_tokens_total: int = 0
    llm_latency_ms_total: int = 0
    slow_llm_calls: int = 0


class Embedder:
    """
    Wraps Ollama embedding and LLM calls with backoff and error handling.

    Usage::

        embedder = Embedder(
            base_url="http://localhost:11434",
            embed_model="nomic-embed-text",
            llm_model="mistral:7b",
        )
        await embedder.health_check()          # fail fast if Ollama is down
        vec = await embedder.embed("some text") # → list[float]
        answer = await embedder.classify("Is this role suitable?")
    """

    MAX_EMBED_CHARS: int = _DEFAULT_MAX_EMBED_CHARS
    """
    Maximum character count accepted by the embedding model.

    The scorer reads this to size its overlapping chunks so that no
    single chunk exceeds the model's context window.
    """

    def __init__(
        self,
        base_url: str,
        embed_model: str,
        llm_model: str,
        *,
        max_retries: int = 3,
        base_delay: float = 1.0,
        slow_llm_threshold_ms: int = 30_000,
    ) -> None:
        """Initialize with Ollama connection and model settings."""
        self.base_url = base_url
        self.embed_model = embed_model
        self.llm_model = llm_model
        self.max_retries = max_retries
        self.base_delay = base_delay
        self._client = ollama_sdk.AsyncClient(host=base_url)
        self._slow_llm_threshold_ms = slow_llm_threshold_ms
        self._metrics = InferenceMetrics()

    # -- Public API ----------------------------------------------------------

    @property
    def metrics(self) -> InferenceMetrics:
        """Accumulated inference metrics for the current session."""
        return self._metrics

    async def embed(self, text: str) -> list[float]:
        """
        Return the embedding vector for *text*.

        Strips whitespace before embedding. Raises VALIDATION for empty
        input; retries transient Ollama errors with exponential backoff.

        Text longer than :attr:`MAX_EMBED_CHARS` is truncated using a
        **head + tail** strategy to avoid exceeding the model's context
        window.  The head captures title, company, and overview.  The
        tail preserves hands-on work details and compensation ranges —
        signals that real-world JDs place in the last third.  The dropped
        middle is typically "about us" boilerplate.
        """
        cleaned = text.strip()
        if not cleaned:
            raise ActionableError.validation(
                field_name="text",
                reason="Cannot embed empty text",
                suggestion="Provide non-empty text to embed",
            )

        if len(cleaned) > self.MAX_EMBED_CHARS:
            logger.debug(
                "Truncating embed input from %d to %d chars (head+tail)",
                len(cleaned),
                self.MAX_EMBED_CHARS,
            )
            budget = self.MAX_EMBED_CHARS - len(_TRUNCATION_MARKER)
            head_len = int(budget * _HEAD_RATIO)
            tail_len = budget - head_len
            cleaned = cleaned[:head_len] + _TRUNCATION_MARKER + cleaned[-tail_len:]

        embed_tokens = 0

        async def _call() -> list[float]:
            nonlocal embed_tokens
            response = await self._client.embed(model=self.embed_model, input=cleaned)
            raw = getattr(response, "prompt_eval_count", None)
            embed_tokens = raw if isinstance(raw, int) and raw > 0 else len(cleaned) // 4
            return list(response.embeddings[0])

        t0 = time.monotonic()
        result = await self._with_retry(_call, operation="embed")
        latency_ms = int((time.monotonic() - t0) * 1000)
        self._metrics.embed_calls += 1
        self._metrics.embed_tokens_total += embed_tokens
        log_event(
            "embed_call",
            model=self.embed_model,
            input_chars=len(cleaned),
            latency_ms=latency_ms,
            tokens=embed_tokens,
        )
        return result

    async def classify(self, prompt: str) -> str:
        """
        Send a classification prompt to the LLM and return the raw response.

        Uses a system message to establish the classification role.
        Retries transient errors with exponential backoff.
        """
        llm_tokens = 0

        async def _call() -> str:
            nonlocal llm_tokens
            response = await self._client.chat(  # pyright: ignore[reportUnknownMemberType]
                model=self.llm_model,
                messages=[
                    {
                        "role": "system",
                        "content": (
                            "You are a job listing classifier. "
                            "Respond concisely with your classification."
                        ),
                    },
                    {"role": "user", "content": prompt},
                ],
            )
            raw_prompt = getattr(response, "prompt_eval_count", None)
            raw_eval = getattr(response, "eval_count", None)
            prompt_tokens = raw_prompt if isinstance(raw_prompt, int) else 0
            eval_tokens = raw_eval if isinstance(raw_eval, int) else 0
            llm_tokens = (prompt_tokens + eval_tokens) or len(prompt) // 4
            return response.message.content  # type: ignore[return-value]

        t0 = time.monotonic()
        result = await self._with_retry(_call, operation="classify")
        latency_ms = int((time.monotonic() - t0) * 1000)
        self._metrics.llm_calls += 1
        self._metrics.llm_tokens_total += llm_tokens
        self._metrics.llm_latency_ms_total += latency_ms
        if latency_ms > self._slow_llm_threshold_ms:
            self._metrics.slow_llm_calls += 1
        log_event(
            "classify_call",
            model=self.llm_model,
            input_chars=len(prompt),
            latency_ms=latency_ms,
            tokens=llm_tokens,
        )
        return result

    async def health_check(self) -> None:
        """
        Verify Ollama is reachable and the configured models are available.

        Raises :class:`~jobsearch_rag.errors.ActionableError`:
          - CONNECTION if Ollama is unreachable
          - EMBEDDING if a required model is not pulled

        Must be called **before** any browser session is opened.
        """
        # 1. Check connectivity
        try:
            response = await self._client.list()
        except (ConnectionError, OSError) as exc:
            raise ActionableError.connection(
                service="Ollama",
                url=self.base_url,
                raw_error=str(exc),
            ) from None

        # 2. Check both models are available
        available = {m.model for m in response.models if m.model}
        # Ollama model names may include :latest suffix — normalise
        available_base = {name.split(":")[0] for name in available}
        available_all = available | available_base

        for model in (self.embed_model, self.llm_model):
            model_base = model.split(":")[0]
            if model not in available_all and model_base not in available_all:
                raise ActionableError.embedding(
                    model=model,
                    raw_error=f"Model '{model}' is not pulled in Ollama",
                    suggestion=f"Run: ollama pull {model}",
                )

        logger.info(
            "Ollama health check passed — %s and %s available",
            self.embed_model,
            self.llm_model,
        )

    # -- Retry logic ---------------------------------------------------------

    async def _with_retry(
        self,
        fn: Callable[[], Awaitable[_T]],
        *,
        operation: str,
    ) -> _T:
        """
        Call *fn* with exponential backoff on retryable errors.

        Non-retryable errors (e.g. 404 model not found) fail immediately.
        After ``max_retries`` attempts, raises an EMBEDDING error.
        """
        last_error: Exception | None = None

        for attempt in range(1, self.max_retries + 1):
            try:
                return await fn()
            except ollama_sdk.ResponseError as exc:
                last_error = exc
                if exc.status_code not in _RETRYABLE_STATUS_CODES:
                    # Non-retryable — fail immediately
                    raise ActionableError.embedding(
                        model=self.embed_model if operation == "embed" else self.llm_model,
                        raw_error=str(exc),
                    ) from None

                delay = self.base_delay * (2 ** (attempt - 1))
                logger.warning(
                    "Ollama %s attempt %d/%d failed (status %d), retrying in %.1fs: %s",
                    operation,
                    attempt,
                    self.max_retries,
                    exc.status_code,
                    delay,
                    exc,
                )
                await asyncio.sleep(delay)
            except (ConnectionError, OSError) as exc:
                last_error = exc
                delay = self.base_delay * (2 ** (attempt - 1))
                logger.warning(
                    "Ollama %s attempt %d/%d connection failed, retrying in %.1fs: %s",
                    operation,
                    attempt,
                    self.max_retries,
                    delay,
                    exc,
                )
                await asyncio.sleep(delay)

        # All retries exhausted
        raise ActionableError.embedding(
            model=self.embed_model if operation == "embed" else self.llm_model,
            raw_error=f"Failed after {self.max_retries} attempts: {last_error}",
            suggestion="Ollama may be overloaded — check resources and retry",
        )
