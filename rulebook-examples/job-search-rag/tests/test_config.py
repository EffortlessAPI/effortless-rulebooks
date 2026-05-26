"""
Configuration validation tests.

Maps to BDD spec: TestSettingsValidation

Tests verify that configuration errors are caught at startup with
actionable messages, valid configs load successfully, and optional
fields use documented defaults.

Spec classes:
    TestSettingsValidation
"""

from __future__ import annotations

import tempfile
from pathlib import Path

import pytest

from jobsearch_rag.config import load_settings
from jobsearch_rag.errors import ActionableError, ErrorType

# A minimal valid settings TOML for tests
_VALID_SETTINGS = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
max_pages = 2
headless = true

[scoring]
archetype_weight = 0.5
fit_weight = 0.3
history_weight = 0.2
negative_weight = 0.4
disqualify_on_llm_flag = true
min_score_threshold = 0.45

[ollama]
base_url = "http://localhost:11434"
llm_model = "mistral:7b"
embed_model = "nomic-embed-text"

[output]
default_format = "markdown"
output_dir = "./output"
open_top_n = 5

[chroma]
persist_dir = "./data/chroma_db"
"""


def _write_settings(tmpdir: str, content: str) -> Path:
    """Write settings content to a temp file and return the path."""
    path = Path(tmpdir) / "settings.toml"
    path.write_text(content, encoding="utf-8")
    return path


class TestSettingsValidation:
    """
    REQUIREMENT: Configuration errors tell the operator exactly what to fix.

    WHO: The operator who misconfigured settings.toml
    WHAT: (1) The system reports that the `boards` section is missing and tells the operator how to add it.
          (2) The system rejects an out-of-range `archetype_weight` value and states the valid range with guidance.
          (3) The system rejects an out-of-range `fit_weight` value and states the valid range with guidance.
          (4) The system reports the missing enabled board configuration and tells the operator to add that board section.
          (5) The system rejects an Ollama URL without a scheme and suggests adding the `http://` prefix.
          (6) The system loads a valid settings file and makes all parsed fields accessible.
          (7) The system applies documented default values for optional fields that are absent.
          (8) The system parses `browser_channel` from a board configuration and exposes it on `BoardConfig`.
          (9) The system defaults `comp_weight` to `0.15` when it is absent.
          (10) The system defaults `base_salary` to `220000` when it is absent.
          (11) The system rejects a negative `base_salary` value and states that it must be positive.
          (12) The system reports that the settings file is not found and tells the operator to create it.
          (13) The system raises a parse error for malformed TOML syntax and suggests fixing the file.
          (14) The system rejects an empty `boards.enabled` list and tells the operator to add board names.
          (15) The system reports a board section that is not a TOML table and tells the operator to define it as a table.
          (16) The system parses overnight board configurations into `BoardConfig` entries.
          (17) The system uses default scoring values when the `scoring` section is not a table.
          (18) The system uses default Ollama values when the `ollama` section is not a table.
          (19) The system uses default output values when the `output` section is not a table.
          (20) The system uses default Chroma values when the `chroma` section is not a table.
          (21) The system reports that the `enabled` field is missing from `boards` and tells the operator to add it.
          (22) The system defaults `negative_weight` to `0.4` when it is absent.
          (23) The system rejects an out-of-range `negative_weight` value above the limit and states the valid range with guidance.
          (24) The system rejects an out-of-range `negative_weight` value below the limit and states the valid range with guidance.
          (25) The system defaults `global_rubric_path` to `config/global_rubric.toml` when it is absent.
          (26) The system reports a nonexistent `global_rubric_path` file and provides recovery guidance.
          (27) The system defaults `culture_weight` to `0.2` when it is absent.
          (28) The system rejects an out-of-range `culture_weight` value above the limit and states the valid range with guidance.
          (29) The system rejects an out-of-range `culture_weight` value below the limit and states the valid range with guidance.
          (30) The system uses the enabled board's config when an overnight board is also in enabled boards.
          (31) The system skips an overnight board whose section is not a TOML table.
    WHY: A mid-run config failure after 10 minutes of browser work is
         far more costly than a startup validation failure — and an error
         that says "validation failed" without telling the operator what
         to fix is nearly as costly as a mid-run crash

    MOCK BOUNDARY:
        Mock:  nothing — uses real filesystem via tempfile.TemporaryDirectory
        Real:  load_settings, TOML parsing, validation, ActionableError
        Never: Patch config internals — exercise the real validation stack
    """

    def test_missing_boards_section_names_the_field_and_tells_operator_to_add_it(self) -> None:
        """
        Given a settings file missing the [boards] section
        When load_settings is called
        Then the error names 'boards' and includes recovery guidance
        """
        # Given: settings TOML with no [boards] section
        bad_toml = """\
[scoring]
archetype_weight = 0.5
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the missing section with recovery guidance
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "boards" in err.error.lower(), f"Error should name 'boards'. Got: {err.error}"
            assert err.suggestion is not None, "Missing suggestion"
            assert "boards" in err.suggestion.lower(), (
                f"Suggestion should mention 'boards'. Got: {err.suggestion}"
            )
            assert err.troubleshooting is not None, "Missing troubleshooting"
            assert len(err.troubleshooting.steps) > 0, "Troubleshooting has no steps"

    def test_weight_above_range_names_the_field_and_valid_range(self) -> None:
        """
        Given archetype_weight is set to 1.5 (above valid range)
        When load_settings is called
        Then the error names 'archetype_weight' and includes guidance
        """
        # Given: archetype_weight exceeds valid range [0.0, 1.0]
        bad_toml = _VALID_SETTINGS.replace("archetype_weight = 0.5", "archetype_weight = 1.5")
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the out-of-range field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "archetype_weight" in err.error, (
                f"Error should name 'archetype_weight'. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_weight_below_range_names_the_field_and_valid_range(self) -> None:
        """
        Given fit_weight is set to -0.1 (below valid range)
        When load_settings is called
        Then the error names 'fit_weight' and includes guidance
        """
        # Given: fit_weight is negative (below valid range [0.0, 1.0])
        bad_toml = _VALID_SETTINGS.replace("fit_weight = 0.3", "fit_weight = -0.1")
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the out-of-range field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "fit_weight" in err.error, f"Error should name 'fit_weight'. Got: {err.error}"
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_missing_board_config_names_the_board_and_section_to_add(self) -> None:
        """
        Given a board listed in enabled that has no config section
        When load_settings is called
        Then the error names the missing board and tells operator to add it
        """
        # Given: 'nonexistent_board' in enabled but no [boards.nonexistent_board]
        bad_toml = """\
[boards]
enabled = ["nonexistent_board"]

[scoring]
archetype_weight = 0.5
fit_weight = 0.3
history_weight = 0.2

[ollama]
base_url = "http://localhost:11434"

[chroma]
persist_dir = "./data/chroma_db"
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error names the missing board
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "nonexistent_board" in err.error, (
                f"Error should name 'nonexistent_board'. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert "nonexistent_board" in err.suggestion, (
                f"Suggestion should name the board. Got: {err.suggestion}"
            )
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_ollama_url_without_scheme_suggests_adding_http_prefix(self) -> None:
        """
        Given the Ollama URL lacks an http:// scheme
        When load_settings is called
        Then the error mentions 'scheme' and suggests adding the prefix
        """
        # Given: base_url missing scheme
        bad_toml = _VALID_SETTINGS.replace(
            'base_url = "http://localhost:11434"',
            'base_url = "localhost:11434"',
        )
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the scheme issue
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "scheme" in err.error.lower(), (
                f"Error should mention 'scheme'. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_valid_settings_load_without_error(self) -> None:
        """
        Given a complete, valid settings.toml file
        When the settings are loaded
        Then all fields are parsed and accessible
        """
        # Given: a complete, valid settings file
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, _VALID_SETTINGS)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: key fields are accessible
            assert settings.enabled_boards == ["testboard"], (
                f"Expected ['testboard'], got {settings.enabled_boards}"
            )
            assert settings.scoring.archetype_weight == 0.5, (
                f"Expected archetype_weight=0.5, got {settings.scoring.archetype_weight}"
            )
            assert settings.ollama.base_url == "http://localhost:11434", (
                f"Expected base_url='http://localhost:11434', got {settings.ollama.base_url}"
            )
            assert settings.chroma.persist_dir == "./data/chroma_db", (
                f"Expected persist_dir='./data/chroma_db', got {settings.chroma.persist_dir}"
            )

    def test_optional_fields_use_documented_defaults_when_absent(self) -> None:
        """
        Given a minimal settings file with only required fields
        When load_settings is called
        Then optional fields use their documented defaults
        """
        # Given: minimal TOML with only boards config
        minimal_toml = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, minimal_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: scoring defaults are applied
            assert settings.scoring.archetype_weight == 0.5, (
                f"Expected archetype_weight default 0.5, got {settings.scoring.archetype_weight}"
            )
            assert settings.scoring.fit_weight == 0.3, (
                f"Expected fit_weight default 0.3, got {settings.scoring.fit_weight}"
            )
            assert settings.scoring.history_weight == 0.2, (
                f"Expected history_weight default 0.2, got {settings.scoring.history_weight}"
            )
            assert settings.scoring.min_score_threshold == 0.45, (
                f"Expected min_score_threshold default 0.45, got {settings.scoring.min_score_threshold}"
            )
            # Then: ollama defaults are applied
            assert settings.ollama.base_url == "http://localhost:11434", (
                f"Expected base_url default, got {settings.ollama.base_url}"
            )
            assert settings.ollama.llm_model == "mistral:7b", (
                f"Expected llm_model default, got {settings.ollama.llm_model}"
            )
            # Then: output defaults are applied
            assert settings.output.default_format == "markdown", (
                f"Expected default_format='markdown', got {settings.output.default_format}"
            )
            assert settings.output.open_top_n == 5, (
                f"Expected open_top_n default 5, got {settings.output.open_top_n}"
            )
            # Then: board defaults are applied
            assert settings.boards["testboard"].browser_channel is None, (
                f"Expected browser_channel=None, got {settings.boards['testboard'].browser_channel}"
            )

    def test_browser_channel_is_parsed_from_board_config(self) -> None:
        """
        Given a board config with browser_channel = 'msedge'
        When load_settings is called
        Then the browser_channel value is accessible on BoardConfig
        """
        # Given: settings with browser_channel in the board section
        channel_toml = _VALID_SETTINGS.replace(
            "headless = true",
            'headless = true\nbrowser_channel = "msedge"',
        )
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, channel_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: browser_channel is parsed
            assert settings.boards["testboard"].browser_channel == "msedge", (
                f"Expected browser_channel='msedge', got {settings.boards['testboard'].browser_channel}"
            )

    def test_comp_weight_defaults_to_zero_point_fifteen_when_absent(self) -> None:
        """
        Given comp_weight is not specified in settings.toml
        When load_settings is called
        Then comp_weight defaults to 0.15
        """
        # Given: minimal TOML without comp_weight
        minimal_toml = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, minimal_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: comp_weight uses the default
            assert settings.scoring.comp_weight == 0.15, (
                f"Expected comp_weight default 0.15, got {settings.scoring.comp_weight}"
            )

    def test_base_salary_defaults_to_220000_when_absent(self) -> None:
        """
        Given base_salary is not specified in settings.toml
        When load_settings is called
        Then base_salary defaults to 220000
        """
        # Given: minimal TOML without base_salary
        minimal_toml = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, minimal_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: base_salary uses the default
            assert settings.scoring.base_salary == 220_000, (
                f"Expected base_salary default 220000, got {settings.scoring.base_salary}"
            )

    def test_negative_base_salary_names_the_field_and_constraint(self) -> None:
        """
        Given base_salary is set to -50000
        When load_settings is called
        Then the error names 'base_salary' and the positive-number constraint
        """
        # Given: negative base_salary in scoring section
        bad_toml = _VALID_SETTINGS.replace(
            "min_score_threshold = 0.45",
            "min_score_threshold = 0.45\nbase_salary = -50000",
        )
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the invalid field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "base_salary" in err.error, f"Error should name 'base_salary'. Got: {err.error}"
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_missing_file_tells_operator_to_create_settings(self) -> None:
        """
        Given the settings file does not exist
        When load_settings is called
        Then the error says 'not found' with creation guidance
        """
        # Given: a nonexistent settings path
        with tempfile.TemporaryDirectory() as tmpdir:
            missing = Path(tmpdir) / "nonexistent.toml"

            # When: load is attempted
            with pytest.raises(ActionableError) as exc_info:
                load_settings(missing)

            # Then: error identifies the missing file
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "not found" in err.error.lower(), (
                f"Error should say 'not found'. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_malformed_toml_identifies_syntax_error_and_suggests_fix(self) -> None:
        """
        Given the settings file contains invalid TOML syntax
        When load_settings is called
        Then a PARSE error is raised with a fix suggestion
        """
        # Given: syntactically broken TOML
        bad_toml = "[boards\nenabled = broken"
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: load is attempted
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: PARSE error with guidance
            err = exc_info.value
            assert err.error_type == ErrorType.PARSE, f"Expected PARSE error, got {err.error_type}"
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_empty_enabled_boards_tells_operator_to_add_board_names(self) -> None:
        """
        Given boards.enabled is an empty list
        When load_settings is called
        Then the error names 'boards.enabled' and tells operator to add names
        """
        # Given: empty enabled list
        bad_toml = _VALID_SETTINGS.replace('enabled = ["testboard"]', "enabled = []")
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error names the empty field
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "boards.enabled" in err.error, (
                f"Error should name 'boards.enabled'. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_board_section_not_a_table_tells_operator_to_define_as_table(self) -> None:
        """
        Given a board value is a scalar instead of a TOML table
        When load_settings is called
        Then the error mentions 'table' and tells operator to restructure
        """
        # Given: board defined as a string instead of a table
        bad_toml = """\
[boards]
enabled = ["testboard"]
testboard = "not a table"
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error mentions the table requirement
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "table" in err.error.lower(), f"Error should mention 'table'. Got: {err.error}"
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_overnight_board_configs_are_parsed(self) -> None:
        """
        Given a settings file with overnight_boards that have config sections
        When load_settings is called
        Then overnight boards have parsed BoardConfig entries
        """
        # Given: settings with an overnight board config
        overnight_toml = """\
[boards]
enabled = ["testboard"]
overnight_boards = ["nightboard"]

[boards.testboard]
searches = ["https://example.org/search"]

[boards.nightboard]
searches = ["https://night.org/search"]
max_pages = 5
headless = false
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, overnight_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: overnight board config is accessible
            assert "nightboard" in settings.boards, (
                f"Expected 'nightboard' in boards. Got: {list(settings.boards.keys())}"
            )
            assert settings.boards["nightboard"].max_pages == 5, (
                f"Expected max_pages=5, got {settings.boards['nightboard'].max_pages}"
            )
            assert settings.boards["nightboard"].headless is False, (
                f"Expected headless=False, got {settings.boards['nightboard'].headless}"
            )

    def test_overnight_board_already_in_enabled_uses_enabled_config(self) -> None:
        """
        Given an overnight board that is also listed in enabled boards
        When load_settings is called
        Then the enabled board's config is used and no duplicate entry is created.
        """
        # Given: same board in both enabled and overnight lists
        overlap_toml = """\
[boards]
enabled = ["testboard"]
overnight_boards = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
max_pages = 7
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, overlap_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: the enabled board config is preserved (max_pages from enabled config)
            assert "testboard" in settings.boards, (
                f"Expected 'testboard' in boards. Got: {list(settings.boards.keys())}"
            )
            assert settings.boards["testboard"].max_pages == 7, (
                f"Expected max_pages=7 from enabled config, got "
                f"{settings.boards['testboard'].max_pages}"
            )

    def test_overnight_board_non_dict_section_is_skipped(self) -> None:
        """
        Given an overnight board whose section in [boards] is a scalar, not a table
        When load_settings is called
        Then the non-dict section is skipped and no BoardConfig is created for it.
        """
        # Given: overnight board section is a string scalar
        bad_section_toml = """\
[boards]
enabled = ["testboard"]
overnight_boards = ["nightboard"]
nightboard = "not a table"

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_section_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: nightboard has no BoardConfig (non-dict section was skipped)
            assert "nightboard" not in settings.boards, (
                f"Non-dict section should be skipped. Got boards: {list(settings.boards.keys())}"
            )

    def test_scoring_section_non_dict_uses_defaults(self) -> None:
        """
        Given scoring is a scalar instead of a table
        When load_settings is called
        Then scoring uses default values
        """
        # Given: scoring defined as a string
        bad_toml = """\
scoring = "not a dict"

[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: scoring falls back to defaults
            assert settings.scoring.archetype_weight == 0.5, (
                f"Expected default archetype_weight=0.5, got {settings.scoring.archetype_weight}"
            )

    def test_ollama_section_non_dict_uses_defaults(self) -> None:
        """
        Given ollama is a scalar instead of a table
        When load_settings is called
        Then ollama uses default values
        """
        # Given: ollama defined as a string
        bad_toml = """\
ollama = "not a dict"

[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: ollama falls back to defaults
            assert settings.ollama.base_url == "http://localhost:11434", (
                f"Expected default base_url, got {settings.ollama.base_url}"
            )

    def test_output_section_non_dict_uses_defaults(self) -> None:
        """
        Given output is a scalar instead of a table
        When load_settings is called
        Then output uses default values
        """
        # Given: output defined as a string
        bad_toml = """\
output = "not a dict"

[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: output falls back to defaults
            assert settings.output.default_format == "markdown", (
                f"Expected default default_format='markdown', got {settings.output.default_format}"
            )

    def test_chroma_section_non_dict_uses_defaults(self) -> None:
        """
        Given chroma is a scalar instead of a table
        When load_settings is called
        Then chroma uses default values
        """
        # Given: chroma defined as a string
        bad_toml = """\
chroma = "not a dict"

[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: chroma falls back to defaults
            assert settings.chroma.persist_dir == "./data/chroma_db", (
                f"Expected default persist_dir, got {settings.chroma.persist_dir}"
            )

    def test_missing_enabled_field_tells_operator_which_field_to_add(self) -> None:
        """
        Given the 'enabled' field is missing from [boards]
        When load_settings is called
        Then the error names 'enabled' and tells operator to add it
        """
        # Given: boards section without 'enabled' field
        bad_toml = """\
[boards]
overnight_boards = []

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error names the missing field
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "enabled" in err.error.lower(), f"Error should name 'enabled'. Got: {err.error}"
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_negative_weight_defaults_to_zero_point_four_when_absent(self) -> None:
        """
        Given negative_weight is not specified in settings.toml
        When load_settings is called
        Then negative_weight defaults to 0.4
        """
        # Given: minimal TOML without negative_weight
        minimal_toml = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, minimal_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: negative_weight uses the default
            assert settings.scoring.negative_weight == 0.4, (
                f"Expected negative_weight default 0.4, got {settings.scoring.negative_weight}"
            )

    def test_negative_weight_above_range_names_the_field_and_valid_range(self) -> None:
        """
        Given negative_weight is set to 1.5 (above valid range)
        When load_settings is called
        Then the error names 'negative_weight' and includes guidance
        """
        # Given: negative_weight exceeds valid range
        bad_toml = _VALID_SETTINGS.replace("negative_weight = 0.4", "negative_weight = 1.5")
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the out-of-range field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "negative_weight" in err.error, (
                f"Error should name negative_weight. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_negative_weight_below_range_names_the_field_and_valid_range(self) -> None:
        """
        Given negative_weight is set to -0.1 (below valid range)
        When load_settings is called
        Then the error names 'negative_weight' and includes guidance
        """
        # Given: negative_weight is below valid range
        bad_toml = _VALID_SETTINGS.replace("negative_weight = 0.4", "negative_weight = -0.1")
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the out-of-range field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "negative_weight" in err.error, (
                f"Error should name negative_weight. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_global_rubric_path_defaults_when_absent(self) -> None:
        """
        Given global_rubric_path is not specified
        When load_settings is called
        Then it defaults to 'config/global_rubric.toml'
        """
        # Given: minimal TOML without global_rubric_path
        minimal_toml = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, minimal_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: default path is used
            assert settings.global_rubric_path == "config/global_rubric.toml", (
                f"Expected default global_rubric_path, got {settings.global_rubric_path}"
            )

    def test_missing_global_rubric_path_names_field_and_creation_guidance(self) -> None:
        """
        Given global_rubric_path points to a nonexistent file
        When load_settings is called
        Then a CONFIG error names the field with recovery guidance
        """
        # Given: global_rubric_path pointing to a nonexistent file
        bad_toml = 'global_rubric_path = "/nonexistent/global_rubric.toml"\n\n' + _VALID_SETTINGS
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the missing rubric file
            err = exc_info.value
            assert err.error_type == ErrorType.CONFIG, (
                f"Expected CONFIG error, got {err.error_type}"
            )
            assert "global_rubric_path" in err.error, (
                f"Error should name 'global_rubric_path'. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_culture_weight_defaults_to_zero_point_two_when_absent(self) -> None:
        """
        Given culture_weight is not specified in settings.toml
        When load_settings is called
        Then culture_weight defaults to 0.2
        """
        # Given: minimal TOML without culture_weight
        minimal_toml = """\
[boards]
enabled = ["testboard"]

[boards.testboard]
searches = ["https://example.org/search"]
"""
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, minimal_toml)

            # When: settings are loaded
            settings = load_settings(path)

            # Then: culture_weight uses the default
            assert settings.scoring.culture_weight == 0.2, (
                f"Expected culture_weight default 0.2, got {settings.scoring.culture_weight}"
            )

    def test_culture_weight_above_range_names_the_field_and_valid_range(self) -> None:
        """
        Given culture_weight is set to 1.5 (above valid range)
        When load_settings is called
        Then the error names 'culture_weight' and includes guidance
        """
        # Given: culture_weight exceeds valid range
        bad_toml = _VALID_SETTINGS.replace(
            "negative_weight = 0.4",
            "negative_weight = 0.4\nculture_weight = 1.5",
        )
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the out-of-range field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "culture_weight" in err.error, (
                f"Error should name culture_weight. Got: {err.error}"
            )
            assert err.suggestion is not None, "Missing suggestion"
            assert err.troubleshooting is not None, "Missing troubleshooting"

    def test_culture_weight_below_range_names_the_field_and_valid_range(self) -> None:
        """
        Given culture_weight is set to -0.1 (below valid range)
        When load_settings is called
        Then the error names 'culture_weight' and includes guidance
        """
        # Given: culture_weight is below valid range
        bad_toml = _VALID_SETTINGS.replace(
            "negative_weight = 0.4",
            "negative_weight = 0.4\nculture_weight = -0.1",
        )
        with tempfile.TemporaryDirectory() as tmpdir:
            path = _write_settings(tmpdir, bad_toml)

            # When: the settings are loaded
            with pytest.raises(ActionableError) as exc_info:
                load_settings(path)

            # Then: error identifies the out-of-range field
            err = exc_info.value
            assert err.error_type == ErrorType.VALIDATION, (
                f"Expected VALIDATION error, got {err.error_type}"
            )
            assert "culture_weight" in err.error, (
                f"Error should name culture_weight. Got: {err.error}"
            )
