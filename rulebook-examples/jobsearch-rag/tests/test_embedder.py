"""
Embedder tests — Ollama wrapper with retry and error handling.

Spec classes:
    TestEmbedding — text-to-vector embedding via Ollama
    TestClassification — LLM classification prompts via Ollama chat
    TestHealthCheck — Ollama reachability and model availability
    TestRetryLogic — transient failure retry with exponential backoff
"""

from __future__ import annotations

from unittest.mock import AsyncMock, MagicMock, patch

import pytest
from ollama import ResponseError

from jobsearch_rag.errors import ActionableError, ErrorType
from jobsearch_rag.rag.embedder import Embedder

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

FAKE_EMBEDDING = [0.1, 0.2, 0.3, 0.4, 0.5]
BASE_URL = "http://localhost:11434"
EMBED_MODEL = "nomic-embed-text"
LLM_MODEL = "mistral:7b"


def _mock_embed_response(embeddings: list[list[float]]) -> MagicMock:
    """Create a mock EmbedResponse with the given embeddings."""
    resp = MagicMock()
    resp.embeddings = embeddings
    return resp


def _mock_chat_response(content: str) -> MagicMock:
    """Create a mock ChatResponse with the given message content."""
    resp = MagicMock()
    resp.message = MagicMock()
    resp.message.content = content
    return resp


def _mock_list_response(model_names: list[str]) -> MagicMock:
    """Create a mock ListResponse with the given model names."""
    models: list[MagicMock] = []
    for name in model_names:
        m = MagicMock()
        m.model = name
        models.append(m)
    resp = MagicMock()
    resp.models = models
    return resp


@pytest.fixture
def embedder() -> Embedder:
    """An Embedder configured for testing."""
    return Embedder(base_url=BASE_URL, embed_model=EMBED_MODEL, llm_model=LLM_MODEL)


# ---------------------------------------------------------------------------
# TestEmbedding
# ---------------------------------------------------------------------------


class TestEmbedding:
    """
    REQUIREMENT: Text is embedded into a vector via Ollama.

    WHO: The indexer converting resume chunks and archetypes to vectors
    WHAT: (1) The system returns the embedding vector as a list of floats.
          (2) The system passes the configured embed model to Ollama when generating an embedding.
          (3) The system strips leading and trailing whitespace before sending text to Ollama.
          (4) The system raises a validation error with guidance when the input text is empty.
          (5) The system raises a validation error with guidance when the input text contains only whitespace.
          (6) The system truncates text that exceeds the model context window before sending it to Ollama.
          (7) The system preserves both the beginning and the end of truncated text and inserts an ellipsis marker between them.
          (8) The system passes text through unchanged when it fits within the context window limit.
    WHY: Wrong model or mangled vectors would silently corrupt all similarity
         scores — the entire scoring pipeline depends on correct embeddings

    MOCK BOUNDARY:
        Mock: embedder._client (ollama.AsyncClient) — Ollama HTTP API
        Real: Embedder.embed, truncation logic, whitespace handling, validation
        Never: Patch Embedder internals or bypass embed()
    """

    async def test_embed_returns_float_vector(self, embedder: Embedder) -> None:
        """
        GIVEN text to embed
        WHEN embed() is called
        THEN a list of floats is returned.
        """
        # Given: mock Ollama client returning a known embedding
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(return_value=_mock_embed_response([FAKE_EMBEDDING]))

            # When: embed text
            result = await embedder.embed("Staff Platform Architect")

            # Then: returns float vector
            assert result == FAKE_EMBEDDING, "Should return the embedding vector"
            assert all(isinstance(v, float) for v in result), "All values should be floats"

    async def test_embed_uses_configured_model(self, embedder: Embedder) -> None:
        """
        GIVEN an embedder with a configured model
        WHEN embed() is called
        THEN the configured embed_model is passed to Ollama.
        """
        # Given: mock Ollama client
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(return_value=_mock_embed_response([FAKE_EMBEDDING]))

            # When: embed text
            await embedder.embed("some text")

            # Then: correct model used
            mock_client.embed.assert_called_once_with(model=EMBED_MODEL, input="some text")

    async def test_embed_strips_whitespace_before_sending(self, embedder: Embedder) -> None:
        """
        GIVEN text with leading/trailing whitespace
        WHEN embed() is called
        THEN the whitespace is stripped before sending to Ollama.
        """
        # Given: mock Ollama client
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(return_value=_mock_embed_response([FAKE_EMBEDDING]))

            # When: embed padded text
            await embedder.embed("  padded text  ")

            # Then: stripped text sent
            mock_client.embed.assert_called_once_with(model=EMBED_MODEL, input="padded text")

    async def test_embed_empty_string_tells_caller_to_provide_content(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN an empty string
        WHEN embed() is called
        THEN a VALIDATION error with guidance is raised.
        """
        # When/Then: embedding empty string raises VALIDATION error
        with pytest.raises(ActionableError) as exc_info:
            await embedder.embed("")

        # Then: error has proper classification and guidance
        err = exc_info.value
        assert err.error_type == ErrorType.VALIDATION, "Error type should be VALIDATION"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_embed_whitespace_only_tells_caller_to_provide_content(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN whitespace-only text
        WHEN embed() is called
        THEN a VALIDATION error with guidance is raised.
        """
        # When/Then: embedding whitespace raises VALIDATION error
        with pytest.raises(ActionableError) as exc_info:
            await embedder.embed("   \n\t  ")

        # Then: error has proper classification and guidance
        err = exc_info.value
        assert err.error_type == ErrorType.VALIDATION, "Error type should be VALIDATION"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_embed_truncates_text_exceeding_context_window(self, embedder: Embedder) -> None:
        """
        GIVEN text exceeding the model context window
        WHEN embed() is called
        THEN the text is truncated before sending to Ollama.
        """
        # Given: text clearly exceeding any context window
        long_text = "x" * 20_000

        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(return_value=_mock_embed_response([FAKE_EMBEDDING]))

            # When: embed long text
            await embedder.embed(long_text)

            # Then: sent text is truncated
            call_args = mock_client.embed.call_args
            sent_text = call_args.kwargs.get("input") or call_args[1].get("input")
            assert len(sent_text) < len(long_text), (
                f"Text should be truncated. Sent {len(sent_text)} chars of {len(long_text)}"
            )

    async def test_embed_truncation_preserves_head_and_tail(self, embedder: Embedder) -> None:
        """
        GIVEN text with distinctive head and tail content
        WHEN embed() truncates it
        THEN both head and tail are preserved with an ellipsis marker.
        """
        # Given: text with identifiable head, expendable middle, identifiable tail
        head_content = "HEAD_SIGNAL_" * 3000
        middle_content = "M" * 20_000
        tail_content = "TAIL_SIGNAL_" * 3000
        long_text = head_content + middle_content + tail_content

        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(return_value=_mock_embed_response([FAKE_EMBEDDING]))

            # When: embed long text
            await embedder.embed(long_text)

            # Then: head, tail, and marker preserved
            call_args = mock_client.embed.call_args
            sent_text = call_args.kwargs.get("input") or call_args[1].get("input")
            assert sent_text.startswith("HEAD_SIGNAL_"), (
                f"Truncation should preserve head. Got: {sent_text[:50]}..."
            )
            assert sent_text.endswith("TAIL_SIGNAL_"), (
                f"Truncation should preserve tail. Got: ...{sent_text[-50:]}"
            )
            assert "\u2026" in sent_text, (
                "Truncated text should contain an ellipsis marker between head and tail"
            )
            assert len(sent_text) < len(long_text), (
                f"Text should be truncated. Sent {len(sent_text)} of {len(long_text)}"
            )

    async def test_embed_text_within_limit_is_not_truncated(self, embedder: Embedder) -> None:
        """
        GIVEN text within the context window limit
        WHEN embed() is called
        THEN the text is passed through unchanged.
        """
        # Given: normal-length text
        normal_text = "Staff Platform Architect for distributed systems"

        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(return_value=_mock_embed_response([FAKE_EMBEDDING]))

            # When: embed normal text
            await embedder.embed(normal_text)

            # Then: text sent unchanged
            call_args = mock_client.embed.call_args
            sent_text = call_args.kwargs.get("input") or call_args[1].get("input")
            assert sent_text == normal_text, "Text within limit should not be truncated"


# ---------------------------------------------------------------------------
# TestClassification
# ---------------------------------------------------------------------------


class TestClassification:
    """
    REQUIREMENT: LLM classification prompts are sent via Ollama chat.

    WHO: The scorer's disqualifier checking for hard-no signals
    WHAT: (1) The system returns the raw LLM response content.
          (2) The system passes the configured `llm_model` to Ollama.
          (3) The system sends the classification prompt as a single user message in the chat conversation.
    WHY: The disqualifier must receive unmodified LLM output to make
         accept/reject decisions — any transformation could flip the result

    MOCK BOUNDARY:
        Mock: embedder._client (ollama.AsyncClient) — Ollama HTTP API
        Real: Embedder.classify, message formatting, model selection
        Never: Patch classify() or message construction
    """

    async def test_classify_returns_llm_response_text(self, embedder: Embedder) -> None:
        """
        GIVEN a classification prompt
        WHEN classify() is called
        THEN the raw LLM response content is returned.
        """
        # Given: mock LLM response
        with patch.object(embedder, "_client") as mock_client:
            mock_client.chat = AsyncMock(
                return_value=_mock_chat_response("DISQUALIFIED: requires clearance")
            )

            # When: classify
            result = await embedder.classify("Is this job suitable?")

            # Then: raw LLM output returned
            assert result == "DISQUALIFIED: requires clearance", "Should return raw LLM response"

    async def test_classify_uses_configured_llm_model(self, embedder: Embedder) -> None:
        """
        GIVEN an embedder with a configured LLM model
        WHEN classify() is called
        THEN the configured llm_model is passed to Ollama.
        """
        # Given: mock chat response
        with patch.object(embedder, "_client") as mock_client:
            mock_client.chat = AsyncMock(return_value=_mock_chat_response("OK"))

            # When: classify
            await embedder.classify("some prompt")

            # Then: correct model used
            call_kwargs = mock_client.chat.call_args
            assert call_kwargs.kwargs["model"] == LLM_MODEL, "Should use configured LLM model"

    async def test_classify_sends_user_message(self, embedder: Embedder) -> None:
        """
        GIVEN a classification prompt
        WHEN classify() is called
        THEN the prompt is sent as a user message in the chat conversation.
        """
        # Given: mock chat response
        with patch.object(embedder, "_client") as mock_client:
            mock_client.chat = AsyncMock(return_value=_mock_chat_response("OK"))

            # When: classify with a specific prompt
            await embedder.classify("evaluate this listing")

            # Then: user message contains the prompt
            call_kwargs = mock_client.chat.call_args
            messages = call_kwargs.kwargs["messages"]
            user_msgs = [m for m in messages if m["role"] == "user"]
            assert len(user_msgs) == 1, "Should send exactly one user message"
            assert user_msgs[0]["content"] == "evaluate this listing", (
                "User message should match prompt"
            )


# ---------------------------------------------------------------------------
# TestHealthCheck
# ---------------------------------------------------------------------------


class TestHealthCheck:
    """
    REQUIREMENT: Ollama unavailability is detected before processing begins.

    WHO: The pipeline runner; the operator who may have forgotten to start Ollama
    WHAT: (1) The system completes the health check without raising an error when both models are available in Ollama.
          (2) The system raises a CONNECTION error that includes the Ollama URL and troubleshooting steps when Ollama is unreachable.
          (3) The system raises an EMBEDDING error that names the missing embed model and provides ollama pull guidance.
          (4) The system raises an EMBEDDING error that names the missing LLM model and provides ollama pull guidance.
    WHY: Completing a full browser session only to fail at scoring wastes
         time and risks rate limiting; fail fast at startup

    MOCK BOUNDARY:
        Mock: embedder._client (ollama.AsyncClient) — Ollama HTTP API
        Real: Embedder.health_check, model validation, error classification
        Never: Patch health_check() or error construction
    """

    async def test_health_check_passes_when_both_models_available(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN both embed and LLM models available in Ollama
        WHEN health_check() is called
        THEN no error is raised.
        """
        # Given: mock list response with both models
        with patch.object(embedder, "_client") as mock_client:
            mock_client.list = AsyncMock(
                return_value=_mock_list_response([EMBED_MODEL, LLM_MODEL])
            )

            # When/Then: health check passes without raising
            await embedder.health_check()

    async def test_unreachable_ollama_provides_url_and_connectivity_steps(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN Ollama is unreachable
        WHEN health_check() is called
        THEN a CONNECTION error with URL and troubleshooting steps is raised.
        """
        # Given: connection error from Ollama
        with patch.object(embedder, "_client") as mock_client:
            mock_client.list = AsyncMock(side_effect=ConnectionError("could not connect"))

            # When/Then: health check raises CONNECTION error
            with pytest.raises(ActionableError) as exc_info:
                await embedder.health_check()

            # Then: error includes URL and guidance
            err = exc_info.value
            assert err.error_type == ErrorType.CONNECTION, "Error type should be CONNECTION"
            assert BASE_URL in err.error, "Error should include the Ollama URL"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"
            assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_missing_embed_model_suggests_ollama_pull(self, embedder: Embedder) -> None:
        """
        GIVEN the embed model is not available in Ollama
        WHEN health_check() is called
        THEN an EMBEDDING error naming the model with pull guidance is raised.
        """
        # Given: only LLM model available
        with patch.object(embedder, "_client") as mock_client:
            mock_client.list = AsyncMock(return_value=_mock_list_response([LLM_MODEL]))

            # When/Then: health check raises EMBEDDING error
            with pytest.raises(ActionableError) as exc_info:
                await embedder.health_check()

            # Then: error names the missing model
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, "Error type should be EMBEDDING"
            assert EMBED_MODEL in err.error, "Error should name the missing embed model"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"
            assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"

    async def test_missing_llm_model_suggests_ollama_pull(self, embedder: Embedder) -> None:
        """
        GIVEN the LLM model is not available in Ollama
        WHEN health_check() is called
        THEN an EMBEDDING error naming the model with pull guidance is raised.
        """
        # Given: only embed model available
        with patch.object(embedder, "_client") as mock_client:
            mock_client.list = AsyncMock(return_value=_mock_list_response([EMBED_MODEL]))

            # When/Then: health check raises EMBEDDING error
            with pytest.raises(ActionableError) as exc_info:
                await embedder.health_check()

            # Then: error names the missing LLM model
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, "Error type should be EMBEDDING"
            assert LLM_MODEL in err.error, "Error should name the missing LLM model"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"
            assert len(err.troubleshooting.steps) > 0, "Should have troubleshooting steps"


# ---------------------------------------------------------------------------
# TestRetryLogic
# ---------------------------------------------------------------------------


class TestRetryLogic:
    """
    REQUIREMENT: Transient Ollama failures are retried with backoff.

    WHO: The embedding/classification caller during scoring
    WHAT: (1) The system retries `embed()` after a transient 503 error and returns the embedding on the second attempt.
          (2) The system raises an `EMBEDDING` error that advises checking system resources after persistent 503 errors exhaust all retries.
          (3) The system raises an `EMBEDDING` error with model guidance without retrying when `embed()` encounters a non-retryable 404 model-not-found error.
          (4) The system retries `classify()` after a transient 503 error and returns the classification on the second attempt.
          (5) The system retries `embed()` after a `ConnectionError` and returns the embedding on the second attempt.
          (6) The system raises an `EMBEDDING` error with the retry count and Ollama guidance after persistent `ConnectionError` failures exhaust all retries.
    WHY: Ollama can be slow under load — a single timeout shouldn't abort
         a scoring run that's already invested browser-scraping time

    MOCK BOUNDARY:
        Mock: embedder._client (ollama.AsyncClient) — Ollama HTTP API
        Real: Embedder retry logic, error classification, backoff timing
        Never: Patch retry internals or backoff delays directly
    """

    async def test_transient_error_is_retried(self, embedder: Embedder) -> None:
        """
        GIVEN a transient 503 error on the first embed call
        WHEN embed() is called
        THEN the call is retried and succeeds on the second attempt.
        """
        # Given: first call fails with 503, second succeeds
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(
                side_effect=[
                    ResponseError("server busy", status_code=503),
                    _mock_embed_response([FAKE_EMBEDDING]),
                ]
            )

            # When: embed is called
            result = await embedder.embed("retry me")

            # Then: result comes from the retry
            assert result == FAKE_EMBEDDING, "Should return embedding from retry"
            assert mock_client.embed.call_count == 2, "Should have called embed twice"

    async def test_max_retries_exhausted_advises_checking_system_resources(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN persistent 503 errors on every embed attempt
        WHEN all retries are exhausted
        THEN an EMBEDDING error advising system resource checks is raised.
        """
        # Given: every call fails with 503
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(
                side_effect=ResponseError("server busy", status_code=503)
            )

            # When/Then: retries are exhausted and EMBEDDING error is raised
            with pytest.raises(ActionableError) as exc_info:
                await embedder.embed("fail forever")

            # Then: error includes guidance
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, "Error type should be EMBEDDING"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_non_retryable_error_provides_model_guidance(self, embedder: Embedder) -> None:
        """
        GIVEN a non-retryable 404 error (model not found)
        WHEN embed() is called
        THEN an EMBEDDING error with model guidance is raised without retrying.
        """
        # Given: model-not-found error (non-retryable)
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(
                side_effect=ResponseError("model 'fake' not found", status_code=404)
            )

            # When/Then: EMBEDDING error raised immediately
            with pytest.raises(ActionableError) as exc_info:
                await embedder.embed("bad model")

            # Then: no retry attempted, guidance provided
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, "Error type should be EMBEDDING"
            assert mock_client.embed.call_count == 1, "Should not retry non-retryable errors"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"

    async def test_classify_retries_on_transient_error(self, embedder: Embedder) -> None:
        """
        GIVEN a transient 503 error on the first classify call
        WHEN classify() is called
        THEN the call is retried and succeeds on the second attempt.
        """
        # Given: first chat call fails with 503, second succeeds
        with patch.object(embedder, "_client") as mock_client:
            mock_client.chat = AsyncMock(
                side_effect=[
                    ResponseError("server busy", status_code=503),
                    _mock_chat_response("OK"),
                ]
            )

            # When: classify is called
            result = await embedder.classify("retry me")

            # Then: result comes from the retry
            assert result == "OK", "Should return classification from retry"
            assert mock_client.chat.call_count == 2, "Should have called chat twice"

    async def test_connection_error_is_retried(self, embedder: Embedder) -> None:
        """
        GIVEN a ConnectionError on the first embed call
        WHEN embed() is called
        THEN the call is retried and succeeds on the second attempt.
        """
        # Given: first call gets connection refused, second succeeds
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(
                side_effect=[
                    ConnectionError("Connection refused"),
                    _mock_embed_response([FAKE_EMBEDDING]),
                ]
            )

            # When: embed is called
            result = await embedder.embed("retry connection")

            # Then: result comes from the retry
            assert result == FAKE_EMBEDDING, "Should return embedding from retry"
            assert mock_client.embed.call_count == 2, "Should have called embed twice"

    async def test_connection_error_exhaustion_advises_checking_ollama(
        self, embedder: Embedder
    ) -> None:
        """
        GIVEN persistent ConnectionError on every embed attempt
        WHEN all retries are exhausted
        THEN an EMBEDDING error with retry count and Ollama guidance is raised.
        """
        # Given: every call gets connection refused
        with patch.object(embedder, "_client") as mock_client:
            mock_client.embed = AsyncMock(side_effect=ConnectionError("Connection refused"))

            # When/Then: retries are exhausted and EMBEDDING error is raised
            with pytest.raises(ActionableError) as exc_info:
                await embedder.embed("fail forever")

            # Then: error includes retry count and guidance
            err = exc_info.value
            assert err.error_type == ErrorType.EMBEDDING, "Error type should be EMBEDDING"
            assert "3 attempts" in err.error, "Error should mention the number of attempts"
            assert err.suggestion is not None, "Should include a suggestion"
            assert err.troubleshooting is not None, "Should include troubleshooting"
