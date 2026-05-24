"""
Configuration loading and validation.

Loads ``settings.toml`` and validates all fields at startup, before any
browser sessions are opened or expensive work begins.  A mid-run config
failure after 10 minutes of browser work is far more costly than a
startup validation failure.

The validated config is exposed as a :class:`Settings` dataclass with
typed fields for each section: ``boards``, ``scoring``, ``ollama``,
``output``, and ``chroma``.
"""

from __future__ import annotations

import tomllib
from dataclasses import dataclass
from pathlib import Path
from typing import Any, cast

from jobsearch_rag.errors import ActionableError

# Type alias for TOML-parsed dicts — values are heterogeneous.
_TOMLDict = dict[str, Any]

# ---------------------------------------------------------------------------
# Config dataclasses
# ---------------------------------------------------------------------------


@dataclass
class BoardConfig:
    """Per-board configuration from ``[boards.<name>]``."""

    name: str
    searches: list[str]
    max_pages: int = 3
    headless: bool = True
    browser_channel: str | None = None


@dataclass
class ScoringConfig:
    """Scoring weights and thresholds from ``[scoring]``."""

    archetype_weight: float = 0.5
    fit_weight: float = 0.3
    history_weight: float = 0.2
    comp_weight: float = 0.15
    negative_weight: float = 0.4
    culture_weight: float = 0.2
    base_salary: float = 220_000
    disqualify_on_llm_flag: bool = True
    min_score_threshold: float = 0.45


@dataclass
class OllamaConfig:
    """Ollama connection settings from ``[ollama]``."""

    base_url: str = "http://localhost:11434"
    llm_model: str = "mistral:7b"
    embed_model: str = "nomic-embed-text"
    slow_llm_threshold_ms: int = 30_000


@dataclass
class OutputConfig:
    """Output settings from ``[output]``."""

    default_format: str = "markdown"
    output_dir: str = "./output"
    open_top_n: int = 5


@dataclass
class ChromaConfig:
    """ChromaDB settings from ``[chroma]``."""

    persist_dir: str = "./data/chroma_db"


@dataclass
class Settings:
    """Top-level validated configuration."""

    enabled_boards: list[str]
    overnight_boards: list[str]
    boards: dict[str, BoardConfig]
    scoring: ScoringConfig
    ollama: OllamaConfig
    output: OutputConfig
    chroma: ChromaConfig
    resume_path: str = "data/resume.md"
    archetypes_path: str = "config/role_archetypes.toml"
    global_rubric_path: str = "config/global_rubric.toml"


# ---------------------------------------------------------------------------
# Default settings path
# ---------------------------------------------------------------------------

DEFAULT_SETTINGS_PATH = Path("config/settings.toml")


# ---------------------------------------------------------------------------
# Loading and validation
# ---------------------------------------------------------------------------


def load_settings(path: str | Path = DEFAULT_SETTINGS_PATH) -> Settings:
    """
    Load and validate settings from a TOML file.

    Raises :class:`~jobsearch_rag.errors.ActionableError`:
      - CONFIG if the file is missing or a required field is absent
      - VALIDATION if field values are out of range
      - PARSE if the TOML is malformed

    Returns a fully validated :class:`Settings` instance.
    """
    filepath = Path(path)
    if not filepath.exists():
        raise ActionableError.config(
            field_name="settings_path",
            reason=f"Settings file not found: {filepath}",
            suggestion=f"Create {filepath} or copy from config/settings.toml.example",
        )

    raw_text = filepath.read_text(encoding="utf-8")
    try:
        data = tomllib.loads(raw_text)
    except tomllib.TOMLDecodeError as exc:
        raise ActionableError.parse(
            board="settings",
            selector="TOML syntax",
            raw_error=str(exc),
            suggestion=f"Fix TOML syntax in {filepath}",
        ) from None

    return _validate(data, filepath)


def _validate(data: _TOMLDict, filepath: Path) -> Settings:
    """Validate raw TOML data and return a Settings instance."""
    # -- boards section ------------------------------------------------------
    boards_section = _require_section(data, "boards", filepath)
    enabled_boards: list[str] = _require_list(boards_section, "enabled", "boards", filepath)

    overnight_boards: list[str] = boards_section.get("overnight_boards", [])  # type: ignore[assignment]

    # Validate each enabled board has a config section
    board_configs: dict[str, BoardConfig] = {}
    for board_name in enabled_boards:
        if board_name not in boards_section:
            raise ActionableError.config(
                field_name=f"boards.{board_name}",
                reason=f"Board '{board_name}' is in [boards].enabled but has no [boards.{board_name}] section",
                suggestion=f"Add a [boards.{board_name}] section with 'searches' and 'max_pages'",
            )
        board_data = _require_table(boards_section, str(board_name), "boards", filepath)
        searches: list[str] = board_data.get("searches", [])  # type: ignore[assignment]
        board_configs[board_name] = BoardConfig(
            name=board_name,
            searches=searches,
            max_pages=int(board_data.get("max_pages", 3)),
            headless=bool(board_data.get("headless", True)),
            browser_channel=board_data.get("browser_channel") or None,
        )

    # Also parse overnight board configs if they have sections
    for board_name in overnight_boards:
        if board_name in boards_section and board_name not in board_configs:
            board_data_raw = boards_section[board_name]
            if isinstance(board_data_raw, dict):
                bd = cast("_TOMLDict", board_data_raw)
                searches_ov: list[str] = bd.get("searches", [])  # type: ignore[assignment]
                board_configs[board_name] = BoardConfig(
                    name=board_name,
                    searches=searches_ov,
                    max_pages=int(bd.get("max_pages", 2)),
                    headless=bool(bd.get("headless", False)),
                    browser_channel=bd.get("browser_channel") or None,
                )

    # -- scoring section -----------------------------------------------------
    scoring_data = _get_table(data, "scoring")

    scoring = ScoringConfig(
        archetype_weight=float(scoring_data.get("archetype_weight", 0.5)),
        fit_weight=float(scoring_data.get("fit_weight", 0.3)),
        history_weight=float(scoring_data.get("history_weight", 0.2)),
        comp_weight=float(scoring_data.get("comp_weight", 0.15)),
        negative_weight=float(scoring_data.get("negative_weight", 0.4)),
        culture_weight=float(scoring_data.get("culture_weight", 0.2)),
        base_salary=float(scoring_data.get("base_salary", 220_000)),
        disqualify_on_llm_flag=bool(scoring_data.get("disqualify_on_llm_flag", True)),
        min_score_threshold=float(scoring_data.get("min_score_threshold", 0.45)),
    )

    # Validate weight ranges
    for weight_name in (
        "archetype_weight",
        "fit_weight",
        "history_weight",
        "comp_weight",
        "negative_weight",
        "culture_weight",
    ):
        value = getattr(scoring, weight_name)
        if value < 0.0:
            raise ActionableError.validation(
                field_name=f"scoring.{weight_name}",
                reason=f"is {value} — must be >= 0.0",
                suggestion=f"Set [scoring].{weight_name} to a value between 0.0 and 1.0",
            )
        if value > 1.0:
            raise ActionableError.validation(
                field_name=f"scoring.{weight_name}",
                reason=f"is {value} — must be <= 1.0",
                suggestion=f"Set [scoring].{weight_name} to a value between 0.0 and 1.0",
            )

    # Validate base_salary
    if scoring.base_salary <= 0:
        raise ActionableError.validation(
            field_name="scoring.base_salary",
            reason=f"is {scoring.base_salary} — must be > 0",
            suggestion="Set [scoring].base_salary to a positive number",
        )

    # -- ollama section ------------------------------------------------------
    ollama_data = _get_table(data, "ollama")

    base_url = str(ollama_data.get("base_url", "http://localhost:11434"))
    if not base_url.startswith(("http://", "https://")):
        raise ActionableError.validation(
            field_name="ollama.base_url",
            reason=f"'{base_url}' is missing a scheme (http:// or https://)",
            suggestion="Set [ollama].base_url to a URL starting with http:// or https://",
        )

    ollama = OllamaConfig(
        base_url=base_url,
        llm_model=str(ollama_data.get("llm_model", "mistral:7b")),
        embed_model=str(ollama_data.get("embed_model", "nomic-embed-text")),
    )

    # -- output section ------------------------------------------------------
    output_data = _get_table(data, "output")

    output = OutputConfig(
        default_format=str(output_data.get("default_format", "markdown")),
        output_dir=str(output_data.get("output_dir", "./output")),
        open_top_n=int(output_data.get("open_top_n", 5)),
    )

    # -- chroma section ------------------------------------------------------
    chroma_data = _get_table(data, "chroma")

    chroma = ChromaConfig(
        persist_dir=str(chroma_data.get("persist_dir", "./data/chroma_db")),
    )

    # -- shared file paths --------------------------------------------------
    resume_path = str(data.get("resume_path", "data/resume.md"))
    archetypes_path = str(data.get("archetypes_path", "config/role_archetypes.toml"))
    global_rubric_path = str(data.get("global_rubric_path", "config/global_rubric.toml"))

    if not Path(global_rubric_path).exists():
        raise ActionableError.config(
            field_name="global_rubric_path",
            reason=f"Global rubric file not found: {global_rubric_path}",
            suggestion=(
                "Create the file or update global_rubric_path in settings.toml "
                "to a valid TOML file path"
            ),
        )

    return Settings(
        enabled_boards=list(enabled_boards),
        overnight_boards=list(overnight_boards),
        boards=board_configs,
        scoring=scoring,
        ollama=ollama,
        output=output,
        chroma=chroma,
        resume_path=resume_path,
        archetypes_path=archetypes_path,
        global_rubric_path=global_rubric_path,
    )


# ---------------------------------------------------------------------------
# Internal helpers
# ---------------------------------------------------------------------------


def _require_section(data: _TOMLDict, name: str, filepath: Path) -> _TOMLDict:
    """Return a required top-level section, or raise CONFIG error."""
    section = data.get(name)
    if section is None or not isinstance(section, dict):
        raise ActionableError.config(
            field_name=name,
            reason=f"Required section [{name}] is missing from {filepath}",
            suggestion=f"Add a [{name}] section to {filepath}",
        )
    return cast("_TOMLDict", section)


def _require_list(
    section: _TOMLDict, field_name: str, section_name: str, filepath: Path
) -> list[str]:
    """Return a required list field, or raise CONFIG error."""
    value = section.get(field_name)
    if not isinstance(value, list) or not value:
        raise ActionableError.config(
            field_name=f"{section_name}.{field_name}",
            reason=f"{section_name}.{field_name} must be a non-empty list",
            suggestion=f"Add at least one item to [{section_name}].{field_name}",
        )
    return cast("list[str]", value)


def _require_table(section: _TOMLDict, key: str, section_name: str, filepath: Path) -> _TOMLDict:
    """Return a required sub-table, or raise CONFIG error."""
    value = section.get(key)
    if not isinstance(value, dict):
        raise ActionableError.config(
            field_name=f"{section_name}.{key}",
            reason=f"[{section_name}.{key}] must be a table",
            suggestion=f"Define [{section_name}.{key}] as a TOML table",
        )
    return cast("_TOMLDict", value)


def _get_table(data: _TOMLDict, name: str) -> _TOMLDict:
    """Return an optional section as a dict (empty if missing/invalid)."""
    section = data.get(name, {})
    if isinstance(section, dict):
        return cast("_TOMLDict", section)
    return {}
