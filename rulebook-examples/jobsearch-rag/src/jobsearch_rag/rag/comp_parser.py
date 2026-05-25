"""
Compensation parsing and scoring.

Extracts salary ranges from job description text via regex, normalizes
hourly rates to annual (x2080), and computes a continuous comp_score
relative to a configurable base_salary target.

No LLM involvement — all extraction is pure regex on raw text.
"""

from __future__ import annotations

import re
from dataclasses import dataclass


@dataclass
class CompResult:
    """Parsed compensation data from a job description."""

    comp_min: float
    comp_max: float
    comp_source: str
    comp_text: str


# ---------------------------------------------------------------------------
# Regex patterns
# ---------------------------------------------------------------------------

# Matches dollar amounts: $180,000  $180000  $180k  $180K  $95.50
_DOLLAR = r"\$\s*(\d[\d,]*(?:\.\d+)?)\s*([kK])?"

# Hourly rate indicators (after the number)
_HOURLY_SUFFIX = r"(?:/\s*(?:hr|hour|h)\b)"

# Annual indicators (after the range or single value)
_ANNUAL_SUFFIX = r"(?:\s*(?:per\s+(?:year|annum)|annually|a\s+year|/\s*(?:yr|year))\b)?"

# Range connectors: " - ", "-", " to "
_RANGE_SEP = r"\s*(?:-|\N{EN DASH}|to)\s*"

# Full range pattern: $NNN - $NNN with optional hourly/annual suffix
_RANGE_PATTERN = re.compile(
    _DOLLAR + _RANGE_SEP + _DOLLAR + r"\s*" + _HOURLY_SUFFIX + "?" + r"\s*" + _ANNUAL_SUFFIX,
    re.IGNORECASE,
)

# Single value: $NNN with optional hourly/annual suffix
_SINGLE_PATTERN = re.compile(
    _DOLLAR + r"\s*" + _HOURLY_SUFFIX + "?" + r"\s*" + _ANNUAL_SUFFIX,
    re.IGNORECASE,
)

# Hourly detection: checks if the matched text contains /hr, /hour, etc.
_HOURLY_RE = re.compile(r"/\s*(?:hr|hour|h)\b", re.IGNORECASE)

# Context words that indicate a number is NOT a salary
_FALSE_POSITIVE_CONTEXT = re.compile(
    r"(?:employees?|people|staff|team\s+members?|offices?|"
    r"(?:billion|million|trillion)\b|"
    r"\bARR\b|\brevenue\b|\bfunding\b|\braised\b)",
    re.IGNORECASE,
)

_HOURS_PER_YEAR = 2080


# ---------------------------------------------------------------------------
# Parsing
# ---------------------------------------------------------------------------


def _parse_dollar(raw_digits: str, k_suffix: str | None) -> float:
    """Convert a captured dollar string to a float."""
    value = float(raw_digits.replace(",", ""))
    if k_suffix:
        value *= 1_000
    return value


def _is_hourly(match_text: str) -> bool:
    """Check whether the matched text indicates an hourly rate."""
    return bool(_HOURLY_RE.search(match_text))


def _is_salary_range(value: float) -> bool:
    """
    Heuristic: a salary number should be in a reasonable range.

    Annual salaries: $20,000 - $1,000,000
    Hourly rates: $10 - $500 (before annualization)
    """
    return 10.0 <= value <= 1_000_000.0


def _has_false_positive_context(text: str, match_start: int, match_end: int) -> bool:
    """Check whether the match is surrounded by false-positive context words."""
    # Look at a window of ~80 chars around the match
    window_start = max(0, match_start - 80)
    window_end = min(len(text), match_end + 80)
    window = text[window_start:window_end]
    return bool(_FALSE_POSITIVE_CONTEXT.search(window))


def parse_compensation(text: str, source: str = "employer") -> CompResult | None:
    """
    Extract compensation data from job description text.

    Returns ``None`` if no salary information is found or if detected
    numbers are likely false positives (employee counts, revenue, etc.).

    Parameters
    ----------
    text:
        Raw JD text to parse.
    source:
        Origin of the salary data — ``"employer"`` (JD body) or
        ``"estimated"`` (board-generated estimate).

    """
    # Try range patterns first (more specific)
    for m in _RANGE_PATTERN.finditer(text):
        low = _parse_dollar(m.group(1), m.group(2))
        high = _parse_dollar(m.group(3), m.group(4))

        if _has_false_positive_context(text, m.start(), m.end()):
            continue

        hourly = _is_hourly(m.group(0))

        if hourly:
            low *= _HOURS_PER_YEAR
            high *= _HOURS_PER_YEAR
        elif not _is_salary_range(low) or not _is_salary_range(high):
            continue

        return CompResult(
            comp_min=low,
            comp_max=high,
            comp_source=source,
            comp_text=m.group(0).strip(),
        )

    # Try single-value patterns
    for m in _SINGLE_PATTERN.finditer(text):
        value = _parse_dollar(m.group(1), m.group(2))

        if _has_false_positive_context(text, m.start(), m.end()):
            continue

        hourly = _is_hourly(m.group(0))

        if hourly:
            value *= _HOURS_PER_YEAR
        elif not _is_salary_range(value):
            continue

        return CompResult(
            comp_min=value,
            comp_max=value,
            comp_source=source,
            comp_text=m.group(0).strip(),
        )

    return None


# ---------------------------------------------------------------------------
# Scoring
# ---------------------------------------------------------------------------


def compute_comp_score(comp_max: float | None, base_salary: float) -> float:
    """
    Compute a continuous compensation score in [0.0, 1.0].

    The score is based on where ``comp_max`` falls relative to
    ``base_salary``:

    - ≥ 100% of base → 1.0
    - 90-100% of base -> 0.7-0.9 (linear interpolation)
    - 77-90% of base -> 0.4-0.7 (linear interpolation)
    - 68-77% of base -> 0.0-0.4 (linear interpolation)
    - < 68% of base → 0.0
    - Missing (None) → 0.5 (neutral)

    This is a *taste signal*, not a gate.  Missing data gets a neutral
    score so it neither rewards nor penalizes.
    """
    if comp_max is None:
        return 0.5

    ratio = comp_max / base_salary

    if ratio >= 1.0:
        return 1.0
    if ratio >= 0.90:
        # Linear interpolation: 0.90 → 0.7, 1.00 → 0.9 (then capped at 1.0 above)
        # score = 0.7 + (ratio - 0.90) / (1.00 - 0.90) * (0.9 - 0.7)
        return 0.7 + (ratio - 0.90) / 0.10 * 0.2
    if ratio >= 0.77:
        # Linear interpolation: 0.77 → 0.4, 0.90 → 0.7
        return 0.4 + (ratio - 0.77) / 0.13 * 0.3
    if ratio >= 0.68:
        # Linear interpolation: 0.68 → 0.0, 0.77 → 0.4
        return 0.0 + (ratio - 0.68) / 0.09 * 0.4
    return 0.0
