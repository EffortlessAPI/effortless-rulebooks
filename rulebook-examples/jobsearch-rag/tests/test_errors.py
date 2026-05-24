"""
Actionable error hierarchy tests.

Tests that the error factory methods produce correct, structured,
recoverable errors per the actionable-error philosophy.

Spec classes:
    TestErrorFactoryMethods
    TestSuggestionPreservation
    TestAIGuidanceToDict
    TestActionableErrorToDict
    TestValidationFactory
    TestFromExceptionClassifier
"""

from __future__ import annotations

from jobsearch_rag.errors import ActionableError, AIGuidance, ErrorType


class TestErrorFactoryMethods:
    """
    REQUIREMENT: Factory methods produce structured errors with embedded guidance.

    WHO: Any component catching exceptions and wrapping them for consumers
    WHAT: (1) The system returns an authentication error that includes a suggestion and troubleshooting guidance so the operator can re-authenticate.
          (2) The system returns a configuration error that names the invalid field and provides recovery steps.
          (3) The system returns a connection error that includes the URL and connectivity troubleshooting guidance.
          (4) The system returns an embedding error that names the model and provides pull and verification guidance.
          (5) The system returns an index error that names the collection and provides rebuild guidance.
          (6) The system returns a parse error that names the board and selector and provides inspection steps.
          (7) The system returns a decision error that names the job ID and provides lookup guidance.
          (8) The system omits all None-valued fields when converting a factory-produced error to a dictionary.
          (9) The system sets success to False on every factory-produced error so callers never treat an error as a success.
    WHY: Opaque errors halt autonomous recovery — every error must carry
         its own recovery path

    MOCK BOUNDARY:
        Mock: nothing — pure object construction
        Real: ActionableError factory methods, to_dict()
        Never: Patch error internals or ErrorType enum
    """

    def test_authentication_factory_provides_recovery_guidance(self) -> None:
        """
        GIVEN an authentication failure
        WHEN authentication() factory is called
        THEN the error includes suggestion and troubleshooting so the operator can re-authenticate.
        """
        # When: create an authentication error
        err = ActionableError.authentication("ziprecruiter", "session expired")

        # Then: error has correct type and recovery guidance
        assert err.error_type == ErrorType.AUTHENTICATION, "Error type should be AUTHENTICATION"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_config_error_names_the_field_with_recovery_steps(self) -> None:
        """
        GIVEN a config field validation failure
        WHEN config() factory is called
        THEN the error names the field and provides recovery steps.
        """
        # When: create a config error
        err = ActionableError.config("scoring.archetype_weight", "must be between 0.0 and 1.0")

        # Then: error names the field and includes guidance
        assert "archetype_weight" in err.error, "Error should name the config field"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_connection_error_includes_url_and_connectivity_steps(self) -> None:
        """
        GIVEN a connection failure with a URL
        WHEN connection() factory is called
        THEN the error includes the URL and connectivity troubleshooting.
        """
        # When: create a connection error
        err = ActionableError.connection("Ollama", "http://localhost:11434", "refused")

        # Then: error includes URL and guidance
        assert "http://localhost:11434" in err.error, "Error should include the URL"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_embedding_error_names_model_with_pull_guidance(self) -> None:
        """
        GIVEN an embedding model failure
        WHEN embedding() factory is called
        THEN the error names the model and provides pull/check guidance.
        """
        # When: create an embedding error
        err = ActionableError.embedding("nomic-embed-text", "timeout after 3 retries")

        # Then: error names the model and includes guidance
        assert "nomic-embed-text" in err.error, "Error should name the embedding model"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_index_error_names_collection_with_rebuild_guidance(self) -> None:
        """
        GIVEN an index/collection failure
        WHEN index() factory is called
        THEN the error names the collection and provides rebuild guidance.
        """
        # When: create an index error
        err = ActionableError.index("resume")

        # Then: error names the collection and includes guidance
        assert "resume" in err.error, "Error should name the collection"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_parse_error_names_board_and_selector_with_inspection_steps(self) -> None:
        """
        GIVEN a parse failure with board and selector
        WHEN parse() factory is called
        THEN the error names board + selector and provides inspection steps.
        """
        # When: create a parse error
        err = ActionableError.parse("ziprecruiter", ".job-title", "element not found")

        # Then: error names board and selector with guidance
        assert "ziprecruiter" in err.error, "Error should name the board"
        assert ".job-title" in err.error, "Error should name the selector"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_decision_error_names_job_id_with_lookup_guidance(self) -> None:
        """
        GIVEN a decision error for a specific job_id
        WHEN decision() factory is called
        THEN the error names the job_id and provides lookup guidance.
        """
        # When: create a decision error
        err = ActionableError.decision("abc-123")

        # Then: error names the job_id and includes guidance
        assert "abc-123" in err.error, "Error should name the job_id"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
        assert len(err.troubleshooting.steps) > 0, "Troubleshooting should have at least one step"

    def test_to_dict_excludes_none_values(self) -> None:
        """
        GIVEN an error produced by a factory method
        WHEN to_dict() is called
        THEN no values in the dict are None — clean for logging and API responses.
        """
        # When: create error and serialize
        err = ActionableError.config("weight", "too high")
        d = err.to_dict()

        # Then: no None values in output
        assert None not in d.values(), "to_dict() should exclude None-valued keys"

    def test_all_factories_set_success_false(self) -> None:
        """
        GIVEN an error produced by any factory method
        WHEN success is checked
        THEN it is False so callers never accidentally treat an error as a success.
        """
        # When: create an error
        err = ActionableError.unexpected("test", "op", "boom")

        # Then: success is False
        assert err.success is False, "Factory errors should always have success=False"


class TestSuggestionPreservation:
    """
    REQUIREMENT: Custom suggestions are always preserved.

    WHO: Callers providing operation-specific context
    WHAT: (1) The system preserves a caller-provided suggestion verbatim when authentication() creates the error instead of using a generic default.
          (2) The system forwards a caller-provided suggestion when from_exception() creates the error instead of generating a generic one.
    WHY: Callers have context that generic classifiers cannot infer

    MOCK BOUNDARY:
        Mock: nothing — pure object construction
        Real: ActionableError.authentication(), from_exception()
        Never: Patch suggestion defaults
    """

    def test_authentication_preserves_custom_suggestion(self) -> None:
        """
        GIVEN a caller-provided suggestion
        WHEN authentication() factory is called with that suggestion
        THEN the suggestion is preserved verbatim, overriding any generic default.
        """
        # When: create error with custom suggestion
        err = ActionableError.authentication(
            "linkedin", "401", suggestion="Delete data/linkedin_session.json and re-auth"
        )

        # Then: custom suggestion preserved
        assert err.suggestion == "Delete data/linkedin_session.json and re-auth", (
            "Custom suggestion should be preserved verbatim"
        )

    def test_from_exception_preserves_caller_suggestion(self) -> None:
        """
        GIVEN a caller-provided suggestion
        WHEN from_exception() is called with that suggestion
        THEN the suggestion is forwarded rather than generating a generic one.
        """
        # When: create error from exception with custom suggestion
        err = ActionableError.from_exception(
            ValueError("401 unauthorized"),
            "test",
            "test_op",
            suggestion="Custom hint",
        )

        # Then: custom suggestion preserved
        assert err.suggestion == "Custom hint", "Custom suggestion should be preserved"


class TestAIGuidanceToDict:
    """
    REQUIREMENT: AIGuidance.to_dict() includes only non-None optional fields.

    WHO: Logging and API consumers deserializing error guidance
    WHAT: (1) The system includes the `command` key in the dictionary when `command` is set.
          (2) The system includes the `discovery_tool` key in the dictionary when `discovery_tool` is set.
          (3) The system includes the `checks` list in the dictionary when `checks` is set.
          (4) The system includes the `steps` list in the dictionary when `steps` is set.
          (5) The system excludes optional fields with `None` values from the dictionary and returns only `action_required` when it is the only populated field.
    WHY: Including None values creates noisy logs and confuses downstream
         tooling that treats presence as meaningful

    MOCK BOUNDARY:
        Mock: nothing — pure object construction
        Real: AIGuidance dataclass, to_dict()
        Never: Patch dataclass fields
    """

    def test_to_dict_includes_command_when_set(self) -> None:
        """
        GIVEN AIGuidance with command populated
        WHEN to_dict() is called
        THEN the dict includes the 'command' key.
        """
        # Given: guidance with command
        g = AIGuidance(action_required="fix it", command="run fix")

        # When: serialize
        d = g.to_dict()

        # Then: command is present
        assert d["command"] == "run fix", "Command should be included in dict"

    def test_to_dict_includes_discovery_tool_when_set(self) -> None:
        """
        GIVEN AIGuidance with discovery_tool populated
        WHEN to_dict() is called
        THEN the dict includes the 'discovery_tool' key.
        """
        # Given: guidance with discovery_tool
        g = AIGuidance(action_required="fix it", discovery_tool="tool_x")

        # When: serialize
        d = g.to_dict()

        # Then: discovery_tool is present
        assert d["discovery_tool"] == "tool_x", "Discovery tool should be included in dict"

    def test_to_dict_includes_checks_when_set(self) -> None:
        """
        GIVEN AIGuidance with checks populated
        WHEN to_dict() is called
        THEN the dict includes the 'checks' list.
        """
        # Given: guidance with checks
        g = AIGuidance(action_required="fix it", checks=["check1", "check2"])

        # When: serialize
        d = g.to_dict()

        # Then: checks are present
        assert d["checks"] == ["check1", "check2"], "Checks list should be included in dict"

    def test_to_dict_includes_steps_when_set(self) -> None:
        """
        GIVEN AIGuidance with steps populated
        WHEN to_dict() is called
        THEN the dict includes the 'steps' list.
        """
        # Given: guidance with steps
        g = AIGuidance(action_required="fix it", steps=["step1", "step2"])

        # When: serialize
        d = g.to_dict()

        # Then: steps are present
        assert d["steps"] == ["step1", "step2"], "Steps list should be included in dict"

    def test_to_dict_excludes_none_optional_fields(self) -> None:
        """
        GIVEN AIGuidance with only action_required set
        WHEN to_dict() is called
        THEN the dict has no extra keys beyond action_required.
        """
        # Given: minimal guidance
        g = AIGuidance(action_required="fix it")

        # When: serialize
        d = g.to_dict()

        # Then: only action_required present
        assert d == {"action_required": "fix it"}, "Only action_required should be in dict"


class TestActionableErrorToDict:
    """
    REQUIREMENT: ActionableError.to_dict() includes troubleshooting and context when set.

    WHO: Error serialization consumers (logs, API responses, AI agents)
    WHAT: (1) The system includes the troubleshooting dictionary with its steps when converting an error with troubleshooting steps to a dictionary.
          (2) The system includes the context dictionary when converting an error with populated context to a dictionary.
    WHY: Structured errors enable automated recovery by downstream agents

    MOCK BOUNDARY:
        Mock: nothing — pure object construction
        Real: ActionableError factory methods, to_dict()
        Never: Patch serialization internals
    """

    def test_to_dict_includes_troubleshooting_when_set(self) -> None:
        """
        GIVEN an error with troubleshooting steps
        WHEN to_dict() is called
        THEN the dict includes the troubleshooting dict with steps.
        """
        # When: create error with troubleshooting
        err = ActionableError.authentication("test_board", "session expired")
        assert err.troubleshooting is not None, "Factory should provide troubleshooting"

        # Then: to_dict includes troubleshooting
        d = err.to_dict()
        assert "troubleshooting" in d, "to_dict should include troubleshooting key"
        assert "steps" in d["troubleshooting"], "Troubleshooting should include steps"

    def test_to_dict_includes_context_when_set(self) -> None:
        """
        GIVEN an error with context dict populated
        WHEN to_dict() is called
        THEN the dict includes the context dict.
        """
        # Given: error with context
        err = ActionableError(
            error="test error",
            error_type=ErrorType.UNEXPECTED,
            service="test",
            context={"key": "value"},
        )

        # When: serialize
        d = err.to_dict()

        # Then: context is preserved
        assert d["context"] == {"key": "value"}, (
            "Context dict should be included in to_dict output"
        )


class TestValidationFactory:
    """
    REQUIREMENT: validation() factory produces VALIDATION-typed errors.

    WHO: Config validation and input parsing code
    WHAT: (1) The system returns a VALIDATION error that names the field and provides suggestion and troubleshooting guidance.
    WHY: Validation errors need distinct routing from config or connection errors

    MOCK BOUNDARY:
        Mock: nothing — pure object construction
        Real: ActionableError.validation()
        Never: Patch validation logic
    """

    def test_validation_factory_provides_field_and_recovery_guidance(self) -> None:
        """
        GIVEN a field validation failure
        WHEN validation() factory is called
        THEN the error names the field and provides suggestion and troubleshooting.
        """
        # When: create a validation error
        err = ActionableError.validation("email", "must contain @")

        # Then: error has correct type and includes field details
        assert err.error_type == ErrorType.VALIDATION, "Error type should be VALIDATION"
        assert "email" in err.error, "Error should name the field"
        assert "must contain @" in err.error, "Error should include the reason"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"


class TestFromExceptionClassifier:
    """
    REQUIREMENT: from_exception() classifies exceptions by keyword patterns.

    WHO: Generic exception handlers wrapping unknown errors
    WHAT: (1) The system classifies an exception containing timeout as CONNECTION and provides suggestion and troubleshooting.
          (2) The system classifies an exception containing timed out as CONNECTION and provides actionable guidance.
          (3) The system classifies an exception containing connection refused as CONNECTION and provides actionable guidance.
          (4) The system classifies an exception containing not found as UNEXPECTED and provides actionable guidance.
          (5) The system classifies an exception with no matching keyword as UNEXPECTED and still provides actionable guidance.
    WHY: Auto-classification provides structured recovery paths for
         exceptions that weren't explicitly caught

    MOCK BOUNDARY:
        Mock: nothing — pure object construction
        Real: ActionableError.from_exception(), keyword classifier
        Never: Patch classification logic or keyword lists
    """

    def test_timeout_classified_as_connection_with_recovery_guidance(self) -> None:
        """
        GIVEN an exception containing 'timeout'
        WHEN from_exception() classifies it
        THEN it is classified as CONNECTION with suggestion and troubleshooting.
        """
        # When: classify a timeout exception
        err = ActionableError.from_exception(
            RuntimeError("request timeout after 30s"),
            "ollama",
            "embed",
        )

        # Then: classified as CONNECTION with guidance
        assert err.error_type == ErrorType.CONNECTION, "Timeout should classify as CONNECTION"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_timed_out_classified_as_connection_with_recovery_guidance(self) -> None:
        """
        GIVEN an exception containing 'timed out'
        WHEN from_exception() classifies it
        THEN it is classified as CONNECTION with actionable guidance.
        """
        # When: classify a timed-out exception
        err = ActionableError.from_exception(
            RuntimeError("connection timed out"),
            "chromadb",
            "query",
        )

        # Then: classified as CONNECTION with guidance
        assert err.error_type == ErrorType.CONNECTION, "'Timed out' should classify as CONNECTION"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_connection_refused_classified_with_recovery_guidance(self) -> None:
        """
        GIVEN an exception containing 'connection refused'
        WHEN from_exception() classifies it
        THEN it is classified as CONNECTION with actionable guidance.
        """
        # When: classify a connection-refused exception
        err = ActionableError.from_exception(
            OSError("connection refused on port 11434"),
            "ollama",
            "health_check",
        )

        # Then: classified as CONNECTION with guidance
        assert err.error_type == ErrorType.CONNECTION, (
            "'Connection refused' should classify as CONNECTION"
        )
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_not_found_classified_as_unexpected_with_guidance(self) -> None:
        """
        GIVEN an exception containing 'not found'
        WHEN from_exception() classifies it
        THEN it is classified as UNEXPECTED with actionable guidance.
        """
        # When: classify a not-found exception
        err = ActionableError.from_exception(
            FileNotFoundError("model not found"),
            "ollama",
            "pull",
        )

        # Then: classified as UNEXPECTED with guidance
        assert err.error_type == ErrorType.UNEXPECTED, "'Not found' should classify as UNEXPECTED"
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"

    def test_unmatched_keyword_classified_unexpected_with_guidance(self) -> None:
        """
        GIVEN an exception with no matching keyword
        WHEN from_exception() classifies it
        THEN it is classified as UNEXPECTED but still provides actionable guidance.
        """
        # When: classify an unmatched exception
        err = ActionableError.from_exception(
            ValueError("some random error"),
            "test",
            "op",
        )

        # Then: classified as UNEXPECTED with guidance
        assert err.error_type == ErrorType.UNEXPECTED, (
            "Unmatched keyword should classify as UNEXPECTED"
        )
        assert err.suggestion is not None, "Should include a suggestion"
        assert err.troubleshooting is not None, "Should include troubleshooting"
